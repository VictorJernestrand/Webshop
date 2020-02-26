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
using Microsoft.AspNetCore.Hosting;


namespace Webshop.Controllers
{
    public class ProductController : Controller
    {

        private readonly WebshopContext context;
        public CreateProductModel createProductModel = new CreateProductModel();

        private IWebHostEnvironment environment;

        private DatabaseCRUD databaseCRUD;



        public ProductController(WebshopContext context, IWebHostEnvironment env)
        {
            this.context = context;
            databaseCRUD = new DatabaseCRUD(context);
            this.environment = env;
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
            createProductModel.categoryVM = context.Categories.ToList();
            createProductModel.brandVM = context.Brands.ToList();

            return View(createProductModel);           
        }

       

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateProduct(IFormFile file,[Bind]CreateProductModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {                    
                    

                    //var folderPath = "Images / " + newProduct.CategoryId;
                    //if (!Directory.Exists(folderPath))
                    //{
                    //    Directory.CreateDirectory(folderPath);
                    //}

                                       

                    // Get path to wwwroot folder
                    var wwwRoot = environment.WebRootPath;

                    //string foldername = null;


                    //var productFolder = "products";
                    var query = from cat in context.Categories
                                where cat.Id == model.CategoryId
                                select cat.Name;

                    var foldername = context.Categories.Where(x => x.Id == model.CategoryId).Select(x => x.Name).FirstOrDefault();

                  

                    // Create folder for storing product images if it does not exist
                    if (!Directory.Exists(wwwRoot + @"\Image\" + foldername))
                        Directory.CreateDirectory(wwwRoot + @"\Image\" + foldername);

                    // Get name of file. Validate file before using it!
                    var fileName = System.IO.Path.GetFileName(file.FileName);

                    // Set the path to point to 
                    var filePath = Path.Combine(foldername, fileName);

                    var fullfilepath= Path.Combine(wwwRoot,@"Image\" + foldername, fileName);

                    using (var fileStream = new FileStream(fullfilepath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }

                    //return View(nameof(TestUploadFile));

                    Product newProduct = new Product()
                    {
                        Name = model.Name,
                        Price = Convert.ToDecimal(model.Price),
                        Quantity = model.Quantity,
                        CategoryId = model.CategoryId,
                        BrandId = model.BrandId,
                        Description = model.Description,
                        Photo = filePath
                    };


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

                    model.categoryVM = context.Categories.ToList();
                    model.brandVM = context.Brands.ToList();
                        TempData["Errors"] = result.ToString();
                    return View(model);
                    
                }

                TempData["Succesmsg"] = $"Great!! {model.Name} uppdateras i databasen"; 
                return RedirectToAction("AllProducts", "Product");


            }
            catch
            {
                TempData["Database error"] = "Sorry!! Något gick fel när du lägger Data till databasen";
                return RedirectToAction("CreateProduct", "Product");              
            }
        }
        
        public IActionResult AllProducts()
        {
            var query = context.Products.ToList();
            return View(query);
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