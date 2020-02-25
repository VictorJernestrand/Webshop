using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Webshop.Context;
using Webshop.Models;

namespace Webshop.Controllers
{
    public class ProductController : Controller
    {
        private readonly WebshopContext context;

        // Create product from model CreateProductModel
        public CreateProductModel createProductModel { get; set; }


        private DatabaseCRUD databaseCRUD;

        public ProductController(WebshopContext context)
        {
            this.context = context;
            databaseCRUD = new DatabaseCRUD(context);
        }
        //This mtd display the Products based on Passed in CategoryId
        public IActionResult Index(int catid)
        {
           
                var query = (from product in context.Products
                             where product.CategoryId==catid
                             select product).ToList();
                return View(query);           
                
        }
        [Authorize(Roles = "Admin")]
        public  IActionResult CreateProduct()
        {
            
            //createProductModel = databaseCRUD.GetAllCategoriesAsync();
           //  createProductModel.categories = databaseCRUD.GetAllCategoriesAsync();
            // var result = context.Categories.ToList();

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
                        Price = Convert.ToDecimal(model.Price),
                        Quantity = model.Quantity,
                        CategoryId = model.CategoryId,
                        BrandId = model.BrandId,
                        Description=model.Description,
                        Photo=model.Photo
                    };

                    //var folderPath = "Images / " + newProduct.CategoryId;
                    //if (!Directory.Exists(folderPath))
                    //{
                    //    Directory.CreateDirectory(folderPath);
                    //}

                    await databaseCRUD.InsertAsync<Product>(newProduct);
                }               

               else
                {                    
                        StringBuilder result = new StringBuilder();

                        foreach (var item in ModelState)
                        {
                            string key = item.Key;
                            var errors = item.Value.Errors;

                            foreach (var error in errors)
                            {
                                result.Append(key + " " + error.ErrorMessage);
                            }
                        }

                        TempData["Errors"] = result.ToString();
                    return View(model);
                    
                }                         

                
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