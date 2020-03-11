using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.Context;
using WebsAPI.Models;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly WebAPIContext _context;

        public ProductsController(WebAPIContext context)
        {
            this._context = context;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Product>> GetProducts()
        {
            var products = _context.Products.ToList();
            return Ok(products);
        }

        [HttpGet("{id}/category")]
        public async Task<ActionResult<IEnumerable<Product>>> GetProductsByCategory(int id)
        {
            // Get selected product based on id
            var products = await _context.Products.Where(x => x.CategoryId == id).ToListAsync();

            // If no product was found, return 404 status code (not found)
            if (products.Count() <= 0)
                return NotFound();

            // Product found return product and 200 status message!
            return Ok(products);
        }
        
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            // Get selected product based on id
            var product = await _context.Products.Where(x => x.Id == id).FirstOrDefaultAsync();

            // If no product was found, return 404 status code (not found)
            if (product == null)
                return NotFound();

            // Product found return product and 200 status message!
            return Ok(product);
        }

    }
}