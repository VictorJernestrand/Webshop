using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Webshop.Models;
using Webshop.Services;

namespace Webshop.Controllers
{
    [Authorize(Roles = "Admin")]
    public class BrandController : Controller
    {
        private readonly WebAPIHandler webAPI;
        private readonly WebAPIToken webAPIToken;

        public Brand BrandModel = new Brand();

        public BrandController(WebAPIHandler webAPI, WebAPIToken webAPIToken, IHttpClientFactory clientFactory)
        {
            this.webAPI = webAPI;
            this.webAPIToken = webAPIToken;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int? id)
        {
            BrandModel.BrandsCollection = await GetAllBrands();

            if (id != null)
            {
                var brand = BrandModel.BrandsCollection.Where(x => x.Id == (int)id).FirstOrDefault();
                BrandModel.Id = brand.Id;
                BrandModel.Name = brand.Name;
            }

            return View(BrandModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit([Bind]Brand model)
        {
            model.BrandsCollection = await GetAllBrands();

            if (ModelState.IsValid)
            {
                if (model.Id > 0)
                {
                    var brand = model.BrandsCollection.Where(x => x.Id == model.Id).FirstOrDefault();
                    brand.Name = model.Name;

                    var token = await webAPIToken.New();
                    var response = await webAPI.UpdateAsync(brand, ApiURL.BRANDS, token);
                }
                else
                {
                    // Does brand already exist?
                    if (model.BrandsCollection.Any(x => x.Name == model.Name))
                    {
                        ModelState.AddModelError("Name", "Tillverkaren finns redan registrerad!");
                        return View("index", model);
                    }

                    // Create new brand
                    var brand = new Brand() { Name = model.Name };

                    // Post to API
                    var token = await webAPIToken.New();
                    var response = await webAPI.PostAsync(brand, ApiURL.BRANDS, token);

                    TempData["NewBrand"] = "Ny tillverkare har skapats!";
                }

                return RedirectToAction("index", "Brand");
            }
            else
            {
                return View("index", model);
            }

        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var brand = await GetBrandById(id);
            return View(brand);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var brand = await GetBrandById(id);

            var token = await webAPIToken.New();
            var response = await webAPI.DeleteAsync(ApiURL.BRANDS + id, token);

            if (response)
                TempData["DeletedBrand"] = "Tillverkaren " + brand.Name + " och alla produkter har readerats";
            else
                TempData["DeletedBrandFail"] = "Kunde inte radera " + brand.Name + ". Kontakta support om problemet kvarstår!";

            return RedirectToAction("index", "Brand");
        }

        // Get Brand by Id
        private async Task<Brand> GetBrandById(int id)
            => await webAPI.GetOneAsync<Brand>(ApiURL.BRANDS + id);

        // Get all brands
        private async Task<List<Brand>> GetAllBrands()
            => await webAPI.GetAllAsync<Brand>(ApiURL.BRANDS);

    }
}