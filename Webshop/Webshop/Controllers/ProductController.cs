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
using Microsoft.EntityFrameworkCore;
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
           List<Product> categoryList = context.Products.Include("Brand").Include("Category").ToList();

           List<CategoryViewModel> categoryViewList = categoryList.Select(x => new CategoryViewModel(x))
                                   .Where(x => x.CategoryId == catid).ToList();

            return View(categoryViewList);                
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

                TempData["Succesmsg"] = "Great!! Product is added to the database"; 
                return RedirectToAction("AllProducts", "Product");


            }
            catch
            {
                
                return Content("its Inside catch block, some error in adding product");
            }
        }
        
        public IActionResult AllProducts()
        {
            //var query = context.Products.ToList();
            //return View(query);

            ///////////////////////////////////
            var products = context.Products.Include("Brand").Include("Category").ToList();

            List<AllProductsViewModel> allProducts = products.Select(x => new AllProductsViewModel(x)).ToList();

            return View(allProducts);
        }
        [HttpPost, ActionName("DeleteProduct")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteProductConfirmed(int? Id)
        {
            Product product = context.Products.FirstOrDefault(p => p.Id == Id);

            context.Products.Remove(product);
            context.SaveChanges();

            if (product != null)
            {
                TempData["Deleted"] = $"{product.Name} är nu borttagen!";
                return RedirectToAction("AllProducts", "Product");
            }
            if (product == null)
            {
                return Content("Det sket sig.");
            }
            return View();
        }
        public IActionResult DeleteProduct(int Id)
        {
            var query = context.Products.Include("Brand").Include("Category").FirstOrDefault(p => p.Id == Id);
            if (query == null)
                return NotFound();
            return View(query);

        }
        public IActionResult ProductDetail(int Id)
        {

            var query = context.Products.Include("Brand").Include("Category").FirstOrDefault(p => p.Id == Id);
            if (query == null)
                return NotFound();
            return View(query);

        }

        

      
    }
}