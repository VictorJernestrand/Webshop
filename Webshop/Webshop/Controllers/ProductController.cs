using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Webshop.Context;
using Webshop.Models;

namespace Webshop.Controllers
{
    public class ProductController : Controller
    {
        private readonly WebshopContext context;

        //public CreateProductModel createProductModel { get; set; }
        public DatabaseCRUD databaseCRUD;
        public ProductController(WebshopContext context)
        {
            this.context = context;
        }
        //This mtd display the Products based on Passed in CategoryId
        public IActionResult Index(int catid)
        {
           List<Product> categoryList = context.Products.Include("Brand").Include("Category").ToList();

           List<CategoryViewModel> categoryViewList = categoryList.Select(x => new CategoryViewModel(x))
                                   .Where(x => x.CategoryId == catid).ToList();

            return View(categoryViewList);                
        }

        [Authorize(Roles = "Admin")]
        public IActionResult CreateProduct()
        {
            CreateProductModel createProductModel = new CreateProductModel();
            return View(createProductModel);
        }
       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateProduct([Bind]CreateProductModel model)
        {
            try
            {
                
                if (ModelState.IsValid)
                {
                    
                    Product newProduct = new Product()
                    {
                        Name = model.Name, 
                        Price = model.Price,
                        Quantity = model.Quantity,
                        CategoryId = model.CategoryId,
                        BrandId = model.BrandId,
                        Description=model.Description,
                        Photo=model.Photo
                    };
                //    DatabaseCRUD databaseCRUD = new DatabaseCRUD(context.Products);
                    
                 // await databaseCRUD.InsertAsync<Product>(newProduct);
                  context.Products.Add(newProduct);
                    context.SaveChanges();
                    
                }
                //this can be used to display summerize the error
                //return View(model);
                // return Content("Successfully added");
                return RedirectToAction("AllProducts", "Product");


            }
            catch
            {
                
                return Content("its Inside catch block, some error in adding product");
            }
        }

        public IActionResult AllProducts()
        {
            var query = context.Products.ToList();
            return View(query);
        }

      
    }
}