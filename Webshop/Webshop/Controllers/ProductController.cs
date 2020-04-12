using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Webshop.Context;
using Webshop.Models;
using Microsoft.AspNetCore.Hosting;
using Webshop.Services;

namespace Webshop.Controllers
{
    public class ProductController : Controller
    {
        private readonly WebAPIToken webAPIToken;
        private readonly WebAPIHandler webAPI;
        private readonly IWebHostEnvironment environment;

        public AllProductsViewModel allProductsViewModel = new AllProductsViewModel();

        // TODO: Safe to delete this file? 
        //public EditProductModel EditProductModel { get; set; }

        public ProductController(WebAPIHandler webAPI, WebAPIToken webAPIToken, IWebHostEnvironment env)
        {
            //this.context = context;
            this.webAPIToken = webAPIToken;
            this.environment = env;
            this.webAPI = webAPI;
        }

        // Get all products based on category Id
        public async Task<ActionResult<List<AllProductsViewModel>>> Index(int catid)
        {
            List<AllProductsViewModel> categoryViewList = await webAPI.GetAllAsync<AllProductsViewModel>(ApiURL.PRODUCTS_IN_CAT + catid);
            return View(categoryViewList);                
        }

        // Get all products
        public async Task<ActionResult<IEnumerable<AllProductsViewModel>>> AllProducts()
        {
            var allProducts = await webAPI.GetAllAsync<AllProductsViewModel>(ApiURL.PRODUCTS);
            return View(allProducts);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteProduct(int Id)
        {
            var query = await webAPI.GetOneAsync<AllProductsViewModel>(ApiURL.PRODUCTS + Id);

            if (query == null)
                return NotFound();

            return View(query);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateProduct()
        {
            allProductsViewModel.Categories = await webAPI.GetAllAsync<Category>(ApiURL.CATEGORIES);
            allProductsViewModel.Brands     = await webAPI.GetAllAsync<Brand>(ApiURL.BRANDS);
            return View(allProductsViewModel);           
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateProduct(IFormFile file, [Bind]AllProductsViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Instantiate new product
                    Product newProduct = new Product()
                    {
                        Name = model.Name,
                        Price = model.Price,
                        Quantity = model.Quantity,
                        CategoryId = model.CategoryId,
                        BrandId = model.BrandId,
                        Description = model.Description,
                        FullDescription = model.FullDescription,
                        Specification = model.Specification,
                        Discount = model.Discount,
                        ActiveProduct = model.ActiveProduct
                    };

                    // Request token
                    var token = await webAPIToken.New();

                    // Store product
                    var apiResonse = await webAPI.PostAsync(newProduct, ApiURL.PRODUCTS, token);

                    // Deserialize API response content and get ID of newly created product
                    var newProductId = webAPI.DeserializeJSON<AllProductsViewModel>(apiResonse.ResponseContent).Id;
                    newProduct.Id = newProductId;

                    // Store image in www root folder with unique product Id
                    if (file != null)
                    {
                        // Set category folder name
                        var folderName = await GetCategoryName(model.CategoryId);

                        // Store new image
                        ProductImage productImage = new ProductImage(environment.WebRootPath, folderName, file);
                        newProduct.Photo = productImage.StoreImage(newProduct.Id);
                    }

                    // Update product with image
                    var response = webAPI.UpdateAsync<Product>(newProduct, ApiURL.PRODUCTS + newProduct.Id, token);
                }               

               else
               {
                    model.Categories = await webAPI.GetAllAsync<Category>(ApiURL.CATEGORIES);
                    model.Brands = await webAPI.GetAllAsync<Brand>(ApiURL.BRANDS);
                    TempData["Errors"] = "Fyll i formuläret ordentligt";
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
       
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteProductConfirmed(int? Id)
        {
            // Get product so we can get the image path
            Product product = await webAPI.GetOneAsync<Product>(ApiURL.PRODUCTS + Id);

            // Send to API and delete product
            var token = await webAPIToken.New();
            var apiResonse = await webAPI.DeleteAsync(ApiURL.PRODUCTS + Id, token);

            ProductImage deleteImage = new ProductImage(environment.WebRootPath);
            deleteImage.DeleteImage(product.Photo);

            if (product != null)
            {
                TempData["Deleted"] = $"{product.Name} är borttagen!";
                return RedirectToAction("AllProducts", "Product");
            }
            else
            {
                return Content("Det sket sig.");
            }

        }

        public async Task<IActionResult> ProductDetail(int id)
        {
            // Get product and ratings by product id
            var product = await webAPI.GetOneAsync<AllProductsViewModel>(ApiURL.PRODUCTS + id);
            product.Ratings = await webAPI.GetAllAsync<Rating>(ApiURL.RATINGS_BY_PRODUCT_ID + id);

            return View(product);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditProduct(int id)
        {
            // Get data from API
            var EditProductModel        = await webAPI.GetOneAsync<AllProductsViewModel>(ApiURL.PRODUCTS + id);
            EditProductModel.Categories = await webAPI.GetAllAsync<Category>(ApiURL.CATEGORIES);
            EditProductModel.Brands     = await webAPI.GetAllAsync<Brand>(ApiURL.BRANDS);

            return View(EditProductModel);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditProduct(IFormFile file, [Bind]AllProductsViewModel model)
        {
            try
            {
                // Get product by id from API
                var productInDB = await webAPI.GetOneAsync<AllProductsViewModel>(ApiURL.PRODUCTS + model.Id);

                if (ModelState.IsValid)
                {
                    Product editproduct = new Product()
                    {
                        Id = model.Id,
                        Name = model.Name,
                        Price = Convert.ToDecimal(model.Price.ToString().Replace('.', ',')),
                        Quantity = model.Quantity,
                        CategoryId = model.CategoryId,
                        BrandId = model.BrandId,
                        Description = model.Description,
                        Photo = model.Photo,
                        FullDescription = model.FullDescription,
                        Specification = model.Specification,
                        Discount = Convert.ToSingle(model.Discount.ToString().Replace('.', ',')),
                        ActiveProduct = model.ActiveProduct
                    };

                    // Set category folder name
                    var folderName = await GetCategoryName(model.CategoryId);

                    // Update image
                    if (file != null)
                    {
                        // Remove old image and store new image
                        ProductImage productImage = new ProductImage(environment.WebRootPath, folderName, file);
                        productImage.DeleteImage(editproduct.Photo);
                        editproduct.Photo = productImage.StoreImage(editproduct.Id);
                    }

                    // If category was changed, move existing product photo to selected folder
                    else if(productInDB.CategoryId != model.CategoryId)
                    {
                        // Move image to new location
                        ProductImage productImage = new ProductImage(environment.WebRootPath, folderName, editproduct.Photo);
                        editproduct.Photo = productImage.MoveImage();
                    }

                    // Send to API and update product
                    var token = await webAPIToken.New();
                    var apiResonse = await webAPI.UpdateAsync(editproduct, ApiURL.PRODUCTS + editproduct.Id, token);
                }

                else
                {
                    // TODO: Separate this to its own method?
                    model.Categories = await webAPI.GetAllAsync<Category>(ApiURL.CATEGORIES);
                    model.Brands     = await webAPI.GetAllAsync<Brand>(ApiURL.BRANDS);
                    return View(model);
                }

                TempData["EditSuccesmsg"] = $"{model.Name} har uppdaterats!";
                return RedirectToAction("AllProducts", "Product");
            }
            catch
            {
                TempData["EditDatabase error"] = "Oops! Något gick fel. Försök igen eller kontakta support om problemet kvarstår.";
                return RedirectToAction("EditProduct", "Product");
            }
        }


        // Get category name by Id
        private async Task<string> GetCategoryName(int id)
        { 
            var category = await webAPI.GetOneAsync<Category>(ApiURL.CATEGORIES + id);
            return category.Name;
        }
    }
}