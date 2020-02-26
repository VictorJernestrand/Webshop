using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Context;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly WebAPIContext _context;

        public ProductController(WebAPIContext context)
        {
            this._context = context;
        }

        [HttpGet("All")]
        public IEnumerable<ProductModel> Get()
        {
            var products = _context.Products.OrderBy(x => x.Price);
            return products;
        }

        [HttpGet]
        public ProductModel Get(int id)
        {
            var product = _context.Products.Where(x => x.Id == id).FirstOrDefault();
            return product;
        }
    }
}