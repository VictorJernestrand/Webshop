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
using Webshop.Models.Data;
using System.Text.Json;
using Webshop.Services;

namespace Webshop.Controllers
{
    public class ProductController : Controller
    {
        private readonly WebshopContext context;
        public CreateProductModel createProductModel = new CreateProductModel();
        public SpecificationModel productSpecification = new SpecificationModel();
        private IWebHostEnvironment environment;
        private DatabaseCRUD databaseCRUD;

        public EditProductModel EditProductModel { get; set; }

        public ProductController(WebshopContext context, IWebHostEnvironment env)
        {
            this.context = context;
            databaseCRUD = new DatabaseCRUD(context);
            this.environment = env;
        }

        //This mtd display the Products based on Passed in CategoryId
        public IActionResult Index(int catid)
        {
           List<Product> categoryList = context.Products.Include(x => x.Brand).Include(x=> x.Category).ToList();

           List<AllProductsViewModel> categoryViewList = categoryList.Select(x => new AllProductsViewModel(x))
                                   .Where(x => x.CategoryId == catid).OrderBy(c => c.Name).ToList();

            return View(categoryViewList);                
        }

        [Authorize(Roles = "Admin")]
        public  IActionResult CreateProduct()
        {
            createProductModel.categoryVM = context.Categories.ToList();
            createProductModel.brandVM = context.Brands.ToList();

            return View(createProductModel);           
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateProduct(IFormFile file,[Bind]CreateProductModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Instantiate new product
                    Product newProduct = new Product()
                    {
                        Name = model.Name,
                        Price = Convert.ToDecimal(model.PriceToConvert.Replace('.', ',')),
                        Quantity = model.Quantity,
                        CategoryId = model.CategoryId,
                        BrandId = model.BrandId,
                        Description = model.Description,
                        FullDescription = model.FullDescription,
                        Specification = model.Specification,
                        Discount = Convert.ToSingle(model.DiscountToConvert.ToString().Replace('.', ','))
                    };

                    // Insert new product in database
                    await databaseCRUD.InsertAsync<Product>(newProduct);

                    if (file != null)
                    {
                        // Set category folder name
                        var folderName = databaseCRUD.GetCategoryName(model.CategoryId);

                        // Store new image
                        ProductImage productImage = new ProductImage(environment.WebRootPath, folderName, file);

                        // Update newProduct with path to new photo
                        newProduct.Photo = productImage.StoreImage(newProduct.Id); ;

                        // Update product in databse with path to product photo
                        await databaseCRUD.UpdateAsync<Product>(newProduct);
                    }
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

                TempData["Succesmsg"] = $"Great!! {model.Name} skapad i databasen"; 
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

            var products = context.Products.Include(x => x.Brand).Include(x => x.Category).ToList();

            List<AllProductsViewModel> allProducts = products.Select(x => new AllProductsViewModel(x)).OrderBy(p => p.Name).ToList();

            return View(allProducts);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteProductConfirmed(int? Id)
        {
            //Product product = await context.Products.FirstOrDefaultAsync(p => p.Id == Id);
            Product product = await context.Products.FindAsync(Id);

            // Get full path to wwwroot folder and concatenate product image from database
            var pathToProductImage = environment.WebRootPath + @"\Image\" + product.Photo;

            // Check if the product image exist...
            if (System.IO.File.Exists(pathToProductImage))
            {
                // Remove product Image from Image folder
                System.IO.File.Delete(pathToProductImage);
            }

            context.Products.Remove(product);
            context.SaveChanges();

            if (product != null)
            {
                TempData["Deleted"] = $"{product.Name} är nu borttagen!";
                return RedirectToAction("AllProducts", "Product");
            }
            else
            {
                return Content("Det sket sig.");
            }

        }

        
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteProduct(int Id)
        {
            var query = context.Products.Include(x => x.Brand).Include(x => x.Category).FirstOrDefault(p => p.Id == Id);

            if (query == null)
                return NotFound();

            return View(query);
        }

        public IActionResult ProductDetail(int Id)
        {
            var query = context.Products.Include(x=> x.Brand).Include(x => x.Category).FirstOrDefault(p => p.Id == Id);
            if (query == null)
                return NotFound();          
            return View(query);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult EditProduct(int id)
        {

            var result = context.Products.Include(x => x.Brand)
                .Include(x => x.Category)
                .Select(x => new EditProductModel
                {
                   Id = x.Id,
                   Name = x.Name,
                   Description = x.Description,
                   Price = x.Price,
                   PriceToConvert = x.Price.ToString(),
                   Quantity = x.Quantity,
                   Photo = x.Photo,
                   CategoryId = x.CategoryId,
                   BrandId = x.BrandId,
                   FullDescription = x.FullDescription,
                   Specification = x.Specification,

                   DiscountToConvert = x.Discount.ToString()
                 
                   

                })
                .FirstOrDefault(p => p.Id == id);

            EditProductModel = result;

            EditProductModel.categoryVM = context.Categories.ToList();
            EditProductModel.brandVM = context.Brands.ToList();
            return View(EditProductModel);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditProduct(IFormFile file, [Bind]EditProductModel model)
        {
            try
            {
                // Get information about the stored info from database to compare with new info
                var productInDB = await context.Products.AsNoTracking().FirstOrDefaultAsync(x => x.Id == model.Id);

                if (ModelState.IsValid)
                {
                    Product editproduct = new Product()
                    {
                        Id = model.Id,
                        Name = model.Name,
                        Price = Convert.ToDecimal(model.PriceToConvert.ToString().Replace('.', ',')),
                        Quantity = model.Quantity,
                        CategoryId = model.CategoryId,
                        BrandId = model.BrandId,
                        Description = model.Description,
                        Photo = model.Photo,
                        FullDescription = model.FullDescription,
                        Specification = model.Specification,
                        Discount = Convert.ToSingle(model.DiscountToConvert.ToString().Replace('.', ','))
                    };

                    // Update image
                    if (file != null)
                    {
                        // Set category folder name
                        var folderName = databaseCRUD.GetCategoryName(model.CategoryId);

                        // Remove old image and store new image
                        ProductImage productImage = new ProductImage(environment.WebRootPath, folderName, file);
                        productImage.RemoveImage(editproduct.Photo);
                        editproduct.Photo = productImage.StoreImage(editproduct.Id);
                    }

                    // If category was changed, move existing product photo to selected folder
                    else if(productInDB.CategoryId != model.CategoryId)
                    {
                        // Set category folder name
                        var newFolderName = databaseCRUD.GetCategoryName(model.CategoryId);

                        // Move image to new location
                        ProductImage productImage = new ProductImage(environment.WebRootPath, newFolderName, editproduct.Photo);
                        editproduct.Photo = productImage.MoveImage();
                    }

                    // save
                    await databaseCRUD.UpdateAsync<Product>(editproduct);
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

                TempData["EditSuccesmsg"] = $"Great!! {model.Name} uppdateras i databasen";
                return RedirectToAction("AllProducts", "Product");
            }
            catch
            {
                TempData["EditDatabase error"] = "Sorry!! Något gick fel när du lägger Data till databasen";
                return RedirectToAction("EditProduct", "Product");
            }
        }
    }
}