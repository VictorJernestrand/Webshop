using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.Context;
using WebAPI.Models;
using WebAPI.Models.Data;

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
        public async Task<ActionResult<IEnumerable<AllProductsViewModel>>> GetAllProducts()
        {

            var test = await _context.Products.Include(x => x.Brand)
                .Include(x => x.Category)
                .Select(x => new AllProductsViewModel
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
                    FullDescription = x.FullDescription,
                    Specification = x.Specification,
                    ActiveProduct = x.ActiveProduct,
                    TotalRatingScore = (float)_context.Ratings.Where(r => r.ProductId == x.Id).Sum(r => r.Score) / _context.Ratings.Count(c => c.ProductId == x.Id)
                })
                .ToListAsync();

            //List<AllProductsViewModel> blalba = test.Select(x => new AllProductsViewModel(x)).OrderBy(p => p.Name).ToList();

            //var products = await _context.Products.Include(x => x.Brand)
            //    .Include(x => x.Category)
            //    .ToListAsync();

            //List<AllProductsViewModel> allProducts = products.Select(x => new AllProductsViewModel(x)).OrderBy(p => p.Name).ToList();

            return test;// allProducts;
        }

        // GET: api/Products/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AllProductsViewModel>> GetProduct(int id)
        {
            //var query = await _context.Products.Include(x => x.Brand)
            //    .Include(x => x.Category)
            //    .Where(x => x.Id == id)
            //    .Select(x => new AllProductsViewModel(x)).FirstOrDefaultAsync();


            var query = await _context.Products.Include(x => x.Brand)
                .Include(x => x.Category)
                .Where(x => x.Id == id)
                .Select(x => new AllProductsViewModel
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
                    FullDescription = x.FullDescription,
                    Specification = x.Specification,
                    ActiveProduct = x.ActiveProduct,
                    TotalRatingScore = (float)_context.Ratings.Where(r => r.ProductId == x.Id).Sum(r => r.Score) / _context.Ratings.Count(c => c.ProductId == x.Id)
                })
                .FirstOrDefaultAsync();

            return query;
        }


        // GET: api/Products/search/tjolahopp
        [Route("search/{searchTerm}")]
        [HttpGet]
        public async Task<ActionResult<AllProductsViewModel>> searchProduct(string searchTerm)
        {
            // TODO: Fix so users can only search on active products!
            searchTerm = searchTerm.ToLower();

            var searchResult = await _context.Products.Include(x => x.Category)
                .Include(x => x.Brand)
                .Where(x => x.ActiveProduct == true &&
                    (x.Name.ToLower().Contains(searchTerm) ||
                    x.Brand.Name.ToLower().Contains(searchTerm) ||
                    x.Category.Name.ToLower().Contains(searchTerm) ||
                    x.Description.ToLower().Contains(searchTerm) ||
                    x.FullDescription.ToLower().Contains(searchTerm) ||
                    x.Specification.ToLower().Contains(searchTerm))
                    )
                .Select(x => new AllProductsViewModel(x))
                .ToListAsync();

            return Ok(searchResult);
        }


        // GET: api/Products/search/tjolahopp
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin")]
        [Route("adminsearch/{searchTerm}")]
        [HttpGet]
        public async Task<ActionResult<AllProductsViewModel>> adminSearchProduct(string searchTerm)
        {
            // TODO: Fix so users can only search on active products!
            searchTerm = searchTerm.ToLower();

            var searchResult = await _context.Products.Include(x => x.Category)
                .Include(x => x.Brand)
                .Where(x => x.Name.ToLower().Contains(searchTerm) ||
                    x.Brand.Name.ToLower().Contains(searchTerm) ||
                    x.Category.Name.ToLower().Contains(searchTerm) ||
                    x.Description.ToLower().Contains(searchTerm) ||
                    x.FullDescription.ToLower().Contains(searchTerm) ||
                    x.Specification.ToLower().Contains(searchTerm))
                .Select(x => new AllProductsViewModel(x))
                .ToListAsync();

            return Ok(searchResult);
        }

        // GET: api/Products/5
        [Route("category/{Id}")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AllProductsViewModel>>> GetAllProductbyCategory(int id)
        {
            //var products = await _context.Products.Include(x => x.Brand).Include(x => x.Category)
            //    .Where(x => x.CategoryId == id)
            //    .ToListAsync();

            //var allProducts = products.Select(x => new AllProductsViewModel(x))
            //    .OrderBy(p => p.Name)
            //    .ToList();

            var allProducts = await _context.Products.Include(x => x.Brand)
                .Include(x => x.Category)
                .Where(x => x.CategoryId == id)
                .Select(x => new AllProductsViewModel
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
                    FullDescription = x.FullDescription,
                    Specification = x.Specification,
                    ActiveProduct = x.ActiveProduct,
                    TotalRatingScore = (float)_context.Ratings.Where(r => r.ProductId == x.Id).Sum(r => r.Score) / _context.Ratings.Count(c => c.ProductId == x.Id)
                })
                .ToListAsync();

            return allProducts;
        }

        // PUT: api/Products/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.

        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin")]
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
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProduct", new { id = product.Id }, product);
        }

        // DELETE: api/Products/5
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin")]
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
