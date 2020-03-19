using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Webshop.Context;
using Webshop.Models;
using Webshop.Services;

namespace Webshop.Controllers
{
    [Authorize(Roles = "Admin")]
    public class BrandController : Controller
    {
        private readonly WebshopContext context;
        private readonly WebAPIHandler webApi;

        EditBrandModel BrandModel = new EditBrandModel();

        public BrandController(WebshopContext context, IHttpClientFactory clientFactory)
        {
            this.context = context;
            this.webApi = new WebAPIHandler(clientFactory);
        }

        
        [HttpGet]
        public async Task<IActionResult> Index(int? id)
        {
            if (id != null)
            {
                var brand = context.Brands.Where(x => x.Id == (int)id).FirstOrDefault();
                BrandModel.Id = brand.Id;
                BrandModel.Name = brand.Name;
            }

            BrandModel.BrandsCollection = context.Brands.ToList();

            //BrandModel.BrandsCollection = await webApi.GetAllAsync<Brand>("https://localhost:44305/api/brands");

            if (id != null)
            {
                var brand = BrandModel.BrandsCollection.Where(x => x.Id == (int)id).FirstOrDefault();
                BrandModel.Id = brand.Id;
                BrandModel.Name = brand.Name;
            }
            
            return View(BrandModel);
        }

        [HttpPost]
        public IActionResult Edit([Bind]EditBrandModel model)
        {
            model.BrandsCollection = context.Brands.ToList();

            if (ModelState.IsValid)
            {
                // If Brand contains an Id, update it, else create new brand!
                if (model.Id > 0)
                {
                    var result = context.Brands.Find(model.Id);
                    result.Name = model.Name;

                    context.Update<Brand>(result);   // Update brand
                }
                else
                {
                    // Does brand already exist?
                    if (context.Brands.Any(x => x.Name == model.Name))
                    {
                        ModelState.AddModelError("Name", "Tillverkaren finns redan registrerad!");
                        return View("index", model);
                    }

                    // Create new brand
                    var brand = new Brand()
                    {
                        Id = model.Id,
                        Name = model.Name
                    };

                    // Add new brand
                    context.Add<Brand>(brand);
                }
                    
                // Save changes
                context.SaveChanges();
                return RedirectToAction("index", "Brand");
            }
            else
            {
                return View("index", model);
            }
        }

      
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var brand = context.Brands.Find(id);
            return View(brand);
        }

        [HttpPost]
        public IActionResult DeleteConfirmed(int id)
        {
            var brand = context.Brands.Find(id);

            context.Remove<Brand>(brand);
            context.SaveChanges();

            return RedirectToAction("index", "Brand");
        }

    }
}