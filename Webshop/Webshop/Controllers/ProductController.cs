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

namespace Webshop.Controllers
{
    public class ProductController : Controller
    {

        private readonly WebshopContext context;
        public CreateProductModel createProductModel = new CreateProductModel();

        public EditProductModel EditProductModel { get; set; }

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

       

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateProduct(IFormFile file,[Bind]CreateProductModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string filePath = null;

                    if (file != null)
                    {
                        // Get path to wwwroot folder
                        var wwwRoot = environment.WebRootPath;

                        var folderName = databaseCRUD.GetCategoryName(model.CategoryId);

                        // Create folder for storing product images if it's not exists
                        if (!Directory.Exists(wwwRoot + @"\Image\" + folderName))
                            Directory.CreateDirectory(wwwRoot + @"\Image\" + folderName);

                        // Get name of file. Validate file before using it!
                        var fileName = System.IO.Path.GetFileName(file.FileName);

                        // Set the path to point to 
                        filePath = Path.Combine(folderName, fileName);

                        var fullfilepath = Path.Combine(wwwRoot, @"Image\" + folderName, fileName);

                        using (var fileStream = new FileStream(fullfilepath, FileMode.Create))
                        {
                            await file.CopyToAsync(fileStream);
                        }
                    }

                    Product newProduct = new Product()
                    {
                        Name = model.Name,
                        Price = Convert.ToDecimal(model.PriceToConvert.Replace('.',',')),
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

        [HttpPost, ActionName("DeleteProduct")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteProductConfirmed(int? Id)
        {
            Product product = await context.Products.FirstOrDefaultAsync(p => p.Id == Id);

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
            if (product == null)
            {
                return Content("Det sket sig.");
            }
            return View();

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
           // Product product = new Product();

            //product = context.Products.Include("Brand").
            //           Include("Category").FirstOrDefault(p => p.Id == id);

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
                   BrandId = x.BrandId
                })
                .FirstOrDefault(p => p.Id == id);

            EditProductModel = result;

            EditProductModel.categoryVM = context.Categories.ToList();
            EditProductModel.brandVM = context.Brands.ToList();
            return View(EditProductModel);

            //EditProductModel editProductModel = new EditProductModel(product);

            //editProductModel.categoryVM = context.Categories.ToList();
            //editProductModel.brandVM = context.Brands.ToList();

            //return View(editProductModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditProduct(IFormFile file, [Bind]EditProductModel model)
        {
            try
            {
                //model.Price = Convert.ToDecimal(model.Price.ToString().Replace(',', '.'));
                if (ModelState.IsValid)
                {

                    Product editproduct = new Product()
                    {
                        Id = model.Id,
                        Name = model.Name,
                        Price = Convert.ToDecimal(model.PriceToConvert.ToString().Replace('.', ',')),//.ToString().Replace(',','.')),
                        Quantity = model.Quantity,
                        CategoryId = model.CategoryId,
                        BrandId = model.BrandId,
                        Description = model.Description,
                        Photo = model.Photo
                    };

                    if (file != null)
                    {
                        // Get path to wwwroot folder
                        var wwwRoot = environment.WebRootPath;

                        var folderName = databaseCRUD.GetCategoryName(model.CategoryId);

                        // Create folder for storing product images if it's not exists
                        if (!Directory.Exists(wwwRoot + @"\Image\" + folderName))
                            Directory.CreateDirectory(wwwRoot + @"\Image\" + folderName);

                        // Get name of file. Validate file before using it!
                        var fileName = System.IO.Path.GetFileName(file.FileName);

                        // Set the path to point to 
                        var filePath = Path.Combine(folderName, fileName);

                        var fullfilepath = Path.Combine(wwwRoot, @"Image\" + folderName, fileName);

                        using (var fileStream = new FileStream(fullfilepath, FileMode.Create))
                        {
                            await file.CopyToAsync(fileStream);
                        }

                        editproduct.Photo = filePath;
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public void AddToCart(int id)
        {
            // Generate a unique id
            Guid guid = Guid.NewGuid();

            // Check if session cookie exist
            if (HttpContext.Session.GetString("CustomerCartSessionId") == null)
            {
                // Set session cookie with Guid Id
                HttpContext.Session.SetString("CustomerCartSessionId", guid.ToString());
            }

            // Get product form database
            Product product = context.Products.Find(id);

            // Cart Id
            Guid cartId = Guid.Parse(HttpContext.Session.GetString("CustomerCartSessionId"));

            // Are there any products left ot buy
            //if (product.Quantity > 0)
            //{
                // Does product allready exist in shoppingcart??
                var cartItem = context.ShoppingCart.Where(x => x.CartId == cartId && x.ProductId == id).FirstOrDefault();
                if (cartItem != null)
                {
                    cartItem.Amount++; // add one more
                }
                else
                {
                    // Instantiate a new schoppingcart item with the selected prodcut id
                    ShoppingCart shoppingCart = new ShoppingCart()
                    {
                        CartId = cartId,
                        ProductId = id,
                        Amount = 1
                    };

                    // All good, put item in shoppingcart
                    //var result = await databaseCRUD.InsertAsync<ShoppingCart>(shoppingCart);
                    context.Add<ShoppingCart>(shoppingCart);
                }

                // Update timestamp 
                List<ShoppingCart> carts = context.ShoppingCart.Where(x => x.CartId == cartId).ToList();
                carts.ForEach(x => x.TimeStamp = DateTime.Now);



                // Update product quantity
                //product.Quantity--;



                // save you fool!
                context.SaveChanges();

            /*}
            else
                return NotFound();*/
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public void RemoveFromCart(int id)
        {
            // Is there a session cookie? Remove product from cart!!
            if (HttpContext.Session.GetString("CustomerCartSessionId") != null)
            {

                // Get item from shoppingcart to be removed
                var cartProductItem = context.ShoppingCart.Find(id);

                // Are there anything to be removed?
                if (cartProductItem.Amount > 0)
                {
                    // Update database quantity
                    var productInStock = context.Products.Find(cartProductItem.ProductId);

                    // Put item back
                    productInStock.Quantity++;

                    // Remove item from cart
                    cartProductItem.Amount--;

                    // Update database
                    context.SaveChanges();
                }

            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public void DeleteItemFromCart(int id)
        {
            var cartItem = context.ShoppingCart.Find(id);
            context.Remove(cartItem);
            context.SaveChanges();


        }


        [HttpGet]
        [Produces("application/json")]
        public CartButtonInfoModel GetCartContent()
        {
            // Is there a session cookie?
            if (HttpContext.Session.GetString("CustomerCartSessionId") == null)
                return new CartButtonInfoModel();

            // Get cart contents
            var cartContent = context.ShoppingCart.Include(x => x.Product)
                .Where(x => x.CartId == Guid.Parse(HttpContext.Session.GetString("CustomerCartSessionId")))
                .ToList()
                .GroupBy(x => new { x.CartId })
                .Select(x => new CartButtonInfoModel
                {
                    TotalItems = x.Sum(x => x.Amount),
                    TotalCost = x.Sum(a => a.Product.Price * a.Amount).ToString("C0")

                }).FirstOrDefault();

            // If cartContent is null return new CartButtonmodel with default values
            return (cartContent != null) ? cartContent : new CartButtonInfoModel();
        }

    }
}