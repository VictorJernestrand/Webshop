using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.Context;
using WebAPI.Models.Data;
using Webshop.Models;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly WebAPIContext _context;

        public ProductsController(WebAPIContext context)
        {
            _context = context;
        }

        // GET: api/Products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AllProductsViewModel>>> GetProducts()
        {
            //var products = context.Products.Include(x => x.Brand).Include(x => x.Category).ToList();

            //List<AllProductsViewModel> allProducts = products.Select(x => new AllProductsViewModel(x)).OrderBy(p => p.Name).ToList();


            List<AllProductsViewModel> allProducts = await _context.Products.Include(x => x.Brand).Include(x => x.Category)
                                                           .Select(x=> new AllProductsViewModel()
                                                         {
                                                                Id = x.Id,
                                                                Name = x.Name,
                                                                Price = x.Price,
                                                                Discount = x.Discount,
                                                                DiscountPrice = x.Price - (x.Price * (decimal)x.Discount), //product.DiscountPrice;
                                                                Quantity = x.Quantity,
                                                                CategoryId = x.CategoryId,
                                                                BrandId = x.BrandId,
                                                                Description = x.Description,
                                                                Photo = x.Photo != null ? x.Photo : "",
                                                                BrandName = x.Brand.Name,
                                                                CategoryName = x.Category.Name,
                                                                Category = x.Category,
                                                                Brand = x.Brand,
                                                                FullDescription = x.FullDescription,
                                                                Specification = x.Specification

                                                          })
                                                         .OrderBy(p=>p.Name).ToListAsync();

            return Ok( allProducts);
        }

        // GET: api/Products/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            return product;
        }

        // PUT: api/Products/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(int id, Product product)
        {
            if (id != product.Id)
            {
                return BadRequest();
            }

            _context.Entry(product).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
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

        // POST: api/Products
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProduct", new { id = product.Id }, product);
        }

        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Product>> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return product;
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
