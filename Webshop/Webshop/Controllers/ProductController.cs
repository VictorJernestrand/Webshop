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
                        string wwwRootImage = environment.WebRootPath + @"\Image\";

                        // Get Id from product database insert
                        int productId = newProduct.Id;

                        // Set category folder name
                        var folderName = databaseCRUD.GetCategoryName(model.CategoryId);

                        // Create folder if doesn't exist
                        if (!Directory.Exists(wwwRootImage + folderName))
                            Directory.CreateDirectory(wwwRootImage + folderName);

                        // Get file-extension from file
                        //var fileExtension = Path.GetExtension(file.FileName);

                        // Get name of file. Validate file before using it!
                        var fileName = productId + "_" + file.FileName; // fileExtension; // ToString Path.GetFileName(file.FileName);

                        // Set path to point to 
                        var filePath = Path.Combine(folderName, fileName);

                        //var fullfilepath = Path.Combine(wwwRoot, @"Image\" + folderName, fileName);
                        var fullFilePath = Path.Combine(wwwRootImage + folderName, fileName);

                        // Move file to new location (wwwroot) folder
                        using (var fileStream = new FileStream(fullFilePath, FileMode.Create))
                        {
                            await file.CopyToAsync(fileStream);
                        }

                        // Update newProduct with path to new photo
                        newProduct.Photo = filePath;

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

                // Get path to wwwroot folder
                var wwwRoot = environment.WebRootPath;
                string wwwRootImage = environment.WebRootPath + @"\Image\";

                // Get folderName
                var folderName = databaseCRUD.GetCategoryName(model.CategoryId);

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

                    // TODO: Add product-Id to the filename

                    // Update image
                    if (file != null)
                    {
                        // Create folder for storing product images if it doesn't exist
                        if (!Directory.Exists(wwwRootImage + folderName))
                            Directory.CreateDirectory(wwwRootImage + folderName);

                        // Get file-extension from file
                        var fileExtension = Path.GetExtension(file.FileName);

                        // Get name of file. Validate file before using it!
                        var fileName = model.Id + "_" + file.FileName; // fileExtension; // ToString Path.GetFileName(file.FileName);

                        // Set path to point to 
                        var filePath = Path.Combine(folderName, fileName);

                        //var fullfilepath = Path.Combine(wwwRoot, @"Image\" + folderName, fileName);
                        var fullfilepath = Path.Combine(wwwRootImage + folderName, fileName);

                        // Move file to new location (wwwroot) folder
                        using (var fileStream = new FileStream(fullfilepath, FileMode.Create))
                        {
                            await file.CopyToAsync(fileStream);
                        }

                        editproduct.Photo = filePath;
                    }

                    // If category was changed, move existing product photo to selected folder
                    else if(productInDB.CategoryId != model.CategoryId)
                    {
                        var newPath = folderName + @"\" + Path.GetFileName(productInDB.Photo);
                        var fullPathToFile = wwwRootImage + newPath;

                        // Check if folder exist
                        try
                        {
                            if (!Directory.Exists(wwwRootImage + folderName))
                                Directory.CreateDirectory(wwwRootImage + folderName);

                            var currentPath = wwwRootImage + productInDB.Photo;
                            System.IO.File.Move(currentPath, fullPathToFile);
                            
                        }
                        catch { }

                        editproduct.Photo = newPath;
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