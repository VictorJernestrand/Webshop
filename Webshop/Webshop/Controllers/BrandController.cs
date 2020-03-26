using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Webshop.Context;
using Webshop.Models;
using Webshop.Services;

namespace Webshop.Controllers
{
    [Authorize(Roles = "Admin")]
    public class BrandController : Controller
    {
        private readonly WebAPIHandler webAPI;
        private readonly WebAPIToken webAPIToken;
        EditBrandModel BrandModel = new EditBrandModel();

        public BrandController(WebAPIHandler webAPI, WebAPIToken webAPIToken, IHttpClientFactory clientFactory)
        {
            this.webAPI = webAPI;
            this.webAPIToken = webAPIToken;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int? id)
        {
            BrandModel.BrandsCollection = await webAPI.GetAllAsync<Brand>("https://localhost:44305/api/brands");

            if (id != null)
            {
                var brand = BrandModel.BrandsCollection.Where(x => x.Id == (int)id).FirstOrDefault();
                BrandModel.Id = brand.Id;
                BrandModel.Name = brand.Name;
            }
            
            return View(BrandModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit([Bind]EditBrandModel model)
        {
            //model.BrandsCollection = context.Brands.ToList();
            model.BrandsCollection = await webAPI.GetAllAsync<Brand>("https://localhost:44305/api/brands");

            if (ModelState.IsValid)
            {
                //try
                //{
                    // If Brand contains an Id, update it, else create new brand!
                    if (model.Id > 0)
                    {
                        var brand = model.BrandsCollection.Where(x => x.Id == model.Id).FirstOrDefault();
                        brand.Name = model.Name;

                        var token = await webAPIToken.New();
                        var response = await webAPI.UpdateAsync(brand, "https://localhost:44305/api/brands/", token);
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
                        var response = await webAPI.PostAsync(brand, "https://localhost:44305/api/brands/", token);

                        TempData["NewBrand"] = "Ny tillverkare har skapats!";
                    }
                //}
                //catch (ArgumentNullException)
                //{
                //    TempData["TokenError"] = "Shit! Autensieringen misslyckades!";
                //}

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
            var brand = await webAPI.GetOneAsync<Brand>("https://localhost:44305/api/brands/" + id);
            return View(brand);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var brand = await webAPI.GetOneAsync<Brand>("https://localhost:44305/api/brands/" + id);

            var token = await webAPIToken.New();
            var response = await webAPI.DeleteAsync("https://localhost:44305/api/brands/" + id, token);

            if (response)
                TempData["DeletedBrand"] = "Tillverkaren " + brand.Name + " och alla produkter har readerats";
            else
                TempData["DeletedBrandFail"] = "Kunde inte radera " + brand.Name + ". Kontakta support om problemet kvarstår!";

            return RedirectToAction("index", "Brand");
        }

    }
}