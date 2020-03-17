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
           List<Product> categoryList = context.Products.Include("Brand").Include("Category").ToList();
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

        /*
        [Authorize(Roles = "Admin")]
        public IActionResult EditSpecifications(int id)
        {
            // Get product from database
            var product = context.Products.Find(id);

            productSpecification.Product = product;

            // Are there any specifications?
            if (product.Specification != null)
            {
                // Deserialize Json data from product specifications
                productSpecification.SpecificationsList = JsonSerializer.Deserialize<List<SpecificationInfo>>(product.Specification);
            }

            return View(productSpecification);
        }
        */

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
                        Specification = model.Specification
                    };

                    // Insert new product in database
                    await databaseCRUD.InsertAsync<Product>(newProduct);

                    if (file != null)
                    {
                        var folderName = databaseCRUD.GetCategoryName(model.CategoryId);
                        ProductImage image = new ProductImage(environment.WebRootPath, folderName, file);

                        // Update newProduct with path to new photo
                        newProduct.Photo = image.StoreImage(newProduct.Id);

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
            var products = context.Products.Include("Brand").Include("Category").ToList();
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

            // Remove image
            ProductImage image = new ProductImage();
            image.RemoveImage(pathToProductImage);

            // Remove from database
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

        [Authorize(Roles = "Admin")]
        public IActionResult EditProduct(int id)
        {

            var result = context.Products.Include("Brand")
                .Include("Category")
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
                   Specification = x.Specification
                   
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
                        Specification = model.Specification
                    };

                    // Update image
                    if (file != null)
                    {
                        var folderName = databaseCRUD.GetCategoryName(model.CategoryId);
                        ProductImage image = new ProductImage(environment.WebRootPath, folderName, file);

                        // Update newProduct with path to new photo
                        editproduct.Photo = image.StoreImage(editproduct.Id);

                        // Update product in databse with path to product photo
                        await databaseCRUD.UpdateAsync<Product>(editproduct);

                    }

                    // If category was changed, move existing product photo to selected folder
                    else if(productInDB.CategoryId != model.CategoryId)
                    {
                        var newFolderLocation = databaseCRUD.GetCategoryName(model.CategoryId);
                        ProductImage image = new ProductImage(environment.WebRootPath, newFolderLocation, editproduct.Photo);
                        editproduct.Photo = image.MoveImage();
                    }

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

        /*
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditSpecifications([Bind]SpecificationModel specificationModel)
        {
            // Get product from database
            var product = context.Products.Find(specificationModel.Product.Id);

            // Check if specifications exist
            if (specificationModel.SpecificationsList != null)
            {
                // Loop through the specifications collection in the specs object
                for (var i = 0; i < specificationModel.SpecificationsList.Count; i++)
                {
                    // If SpecInfo or SpecName is null or empty...
                    if (string.IsNullOrEmpty(specificationModel.SpecificationsList[i].SpecInfo) || string.IsNullOrEmpty(specificationModel.SpecificationsList[i].SpecTitle))
                    {
                        // Remove current index from collection
                        specificationModel.SpecificationsList.RemoveAt(i);

                        // Counter must be reset since the collection will be re-indexed after an index has been removed
                        i = 0;
                    }
                }
            }

            // If collection contains values, Serialize to Json, else set as null
            product.Specification = (specificationModel.SpecificationsList != null && specificationModel.SpecificationsList.Count > 0)
                ? JsonSerializer.Serialize(specificationModel.SpecificationsList)
                : null;

            // update product
            context.Update<Product>(product);
            context.SaveChanges();

            // Temp message
            TempData["SpecpsUpdated"] = "Specifikationerna har uppdaterats";

            // Reload page
            return RedirectToAction("EditSpecifications", "Product", new { id = product.Id });
        }*/
    }
}