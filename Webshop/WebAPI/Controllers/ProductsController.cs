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
    public class ProductsController : ControllerBase
    {
        private readonly WebAPIContext _context;

        public ProductsController(WebAPIContext context)
        {
            this._context = context;
        }

        [HttpGet("All")]
        public IEnumerable<Products> Get()
        {
            var products = _context.Products.OrderBy(x => x.Price);
            return products;
        }

        [HttpGet]
        public Products Get(int id)
        {
            var product = _context.Products.Where(x => x.Id == id).FirstOrDefault();
            return product;
        }
    }
}