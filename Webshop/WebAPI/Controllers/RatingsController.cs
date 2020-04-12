using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.Context;
using WebAPI.Models.Data;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RatingsController : ControllerBase
    {
        private readonly WebAPIContext _context;

        public RatingsController(WebAPIContext context)
        {
            _context = context;
        }

        // GET: api/Ratings/product/5
        [Route("product/{productId}")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Rating>>> GetRatingsByProductId(int productId)
        {
            var result = await _context.Ratings.Include(x => x.User)
                .Where(x => x.ProductId == productId)
                .Select(x => new Rating
                {
                    Id = x.Id,
                    UserId = x.UserId,
                    UserEmail = x.User.Email,
                    UserName = x.User.FirstName,
                    ProductId = x.ProductId,
                    Score = x.Score,
                    RateDate = x.RateDate,
                    Comment = x.Comment,
                })
                .OrderByDescending(x => x.RateDate)
                .ToListAsync();

            return result;
        }

        // GET: api/Ratings/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Rating>> GetRatingbyId(int id)
        {
            var rating = await _context.Ratings.FindAsync(id);

            if (rating == null)
            {
                return NotFound();
            }

            return Ok(rating);
        }

        // PUT: api/Ratings/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRating(int id, Rating rating)
        {
            if (id != rating.Id)
            {
                return BadRequest();
            }

            _context.Entry(rating).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RatingExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Ratings
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost]
        public async Task<ActionResult<Rating>> PostRating(Rating rating)
        {
            try
            {
                var userExist = _context.Users.Any(x => x.Email == rating.UserEmail && x.Id == rating.UserId);

                if (userExist)
                {
                    _context.Ratings.Add(rating);
                    await _context.SaveChangesAsync();
                    return CreatedAtAction("GetRatingbyId", new { id = rating.Id }, rating);
                    //return Ok();
                }
            }
            catch (Exception)
            {
                // Handle exception...
            }

            return Unauthorized();

            //return CreatedAtAction("GetRatingbyId", new { id = rating.Id }, rating);
        }

        // DELETE: api/Ratings/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Rating>> DeleteRating(int id)
        {
            var rating = await _context.Ratings.FindAsync(id);
            if (rating == null)
            {
                return NotFound();
            }

            _context.Ratings.Remove(rating);
            await _context.SaveChangesAsync();

            return Ok(rating);
        }

        private bool RatingExists(int id)
        {
            return _context.Ratings.Any(e => e.Id == id);
        }
    }
}
