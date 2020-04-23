using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Webshop.Models;
using Webshop.Services;

namespace Webshop.Controllers
{
    public class ProductController : Controller
    {
        private readonly WebAPIToken webAPIToken;
        private readonly WebAPIHandler webAPI;
        private readonly IWebHostEnvironment environment;

        public AllProductsViewModel allProductsViewModel = new AllProductsViewModel();

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
            // Get all products by category id
            List<AllProductsViewModel> categoryViewList = await webAPI.GetAllAsync<AllProductsViewModel>(ApiURL.PRODUCTS_IN_CAT + catid);

            // Get categoryname
            var category = await webAPI.GetOneAsync<Category>(ApiURL.CATEGORIES + catid);
            ViewBag.CategoryName = category.Name;

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
            allProductsViewModel.Brands = await webAPI.GetAllAsync<Brand>(ApiURL.BRANDS);

            // Add dummy data for demonstration purposes
            allProductsViewModel.FullDescription = DummyDescription();
            allProductsViewModel.Specification = DummySpecifications();

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
                    if (!IsUploadedFileImage(file))
                    {
                        ModelState.AddModelError("Photo", "Filen är ogiltig!");
                        TempData["Errors"] = "Filen är ogiltig!";
                        model.Categories = await webAPI.GetAllAsync<Category>(ApiURL.CATEGORIES);
                        model.Brands = await webAPI.GetAllAsync<Brand>(ApiURL.BRANDS);
                        return View(model);
                    }

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
            var EditProductModel = await webAPI.GetOneAsync<AllProductsViewModel>(ApiURL.PRODUCTS + id);
            EditProductModel.Categories = await webAPI.GetAllAsync<Category>(ApiURL.CATEGORIES);
            EditProductModel.Brands = await webAPI.GetAllAsync<Brand>(ApiURL.BRANDS);

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
                    if (!IsUploadedFileImage(file))
                    {
                        ModelState.AddModelError("Photo", "Filen är ogiltig!");
                        TempData["Errors"] = "Filen är ogiltig!";
                        model.Categories = await webAPI.GetAllAsync<Category>(ApiURL.CATEGORIES);
                        model.Brands = await webAPI.GetAllAsync<Brand>(ApiURL.BRANDS);
                        return View(model);
                    }

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
                    else if (productInDB.CategoryId != model.CategoryId)
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
                    model.Brands = await webAPI.GetAllAsync<Brand>(ApiURL.BRANDS);
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

        public bool IsUploadedFileImage(IFormFile file)
        {
            switch (file.ContentType)
            {
                case "image/jpeg":
                case "image/png":
                    return true;
                default:
                    return false;
            }
        }




        /* 
         * 
         *      DUMMY DATA BELOW FOR DEMONSTRATION PURPOSE
         * 
         * 
         * */

        // Description dummy-data for demonstration purposes
        private static string DummyDescription() 
            => @"<h4>Lorem Ipsum Dolor Dit Amet</h4><p>Lorem ipsum dolor sit amet, consectetur adipiscing elit. Etiam sed lectus ex. Quisque elementum est et leo consectetur, sit amet imperdiet eros pretium. Donec diam ligula, placerat et pharetra in, semper vel eros. Mauris lectus eros, iaculis sit amet turpis a, gravida euismod enim. Sed aliquam eros sed ipsum euismod posuere. Etiam lacinia nisi vel ornare interdum. Integer euismod suscipit faucibus. Phasellus porttitor fringilla mi vel maximus.</p><h4>Maximum Metal</h4><p>Aenean vitae maximus ipsum, eu laoreet ligula. Nulla non neque mattis, tincidunt turpis et, accumsan felis. Nam porttitor cursus sem eget mattis. Integer tristique non lectus nec hendrerit. Fusce vel ipsum et lacus ultricies porta in malesuada velit. Nulla ac feugiat diam, ut dictum lacus. Duis eget lacus suscipit, mollis eros eget, tincidunt ex. Praesent id tellus tincidunt, facilisis orci et, placerat nunc.</p><h4>Hokus Pokus Filiokus</h4><p>Aenean egestas quam purus, non molestie felis dignissim id. Donec porta hendrerit urna, non imperdiet odio commodo et. Ut molestie rutrum nunc, pretium posuere ex porttitor sit amet. Nulla vitae rhoncus est. Praesent et pellentesque justo. Aenean eu arcu a ipsum posuere rutrum a non nunc. Proin euismod nulla et leo semper luctus. Curabitur vitae scelerisque nunc.</p>";

        // Description dummy-data for demonstration purposes
        private static string DummySpecifications()
            => @"<h4>Specifikationer</h4><ul><li><strong>Lorem:</strong> Ipsum</li><li><strong>Dolor:</strong> Sit Amet</li><li><strong>Etiam:</strong> 4-Sed Lectus</li><li><strong>Elementum:</strong> Consectur</li><li><strong>Eros:</strong> Pretium</li></ul><h4>Ännu Mer Specs</h4><ul><li><strong>Aenan:</strong> Vitae</li><li><strong>Maximus:</strong> Ipsum ''C''</li><li><strong>Tincidunt turpis:</strong> 34''</li><li><strong>Cursus:</strong> Sem</li><li><strong>Radius:</strong> 9.5''</li><li><strong>Electrus:</strong> 20 Medium Fjuttus</li><li><strong>Inlaysum:</strong> Blackis Dotsem</li><li><strong>Nut:</strong> Synthum Bonecus</li><li><strong>Widtaem:</strong> 1.625''</li></ul><h4>Exempel Specs</h4><ul><li><strong>Mid Pickem:</strong> Lite skitum textus härus</li><li><strong>Controlsec:</strong> Curabitur vitae scelerisque nunc</li><li><strong>Donev porta:</strong> Integer euismod suscipit</li><li><strong>Aenan:</strong> Etiam sed lectus</li><li><strong>Elementum:</strong> Etiam lacinia nisi vel ornare interdum</li><li><strong>Quisque:</strong> malesuada velit</li></ul>";
    
    
    }
}