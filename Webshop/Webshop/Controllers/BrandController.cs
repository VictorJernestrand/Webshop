using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Webshop.Context;
using Webshop.Models;

namespace Webshop.Controllers
{
    [Authorize(Roles = "Admin")]
    public class BrandController : Controller
    {
        private readonly WebshopContext context;

        EditBrandModel BrandModel = new EditBrandModel();

        public BrandController(WebshopContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public IActionResult Index(int? id)
        {
            if (id != null)
                BrandModel.Brand = context.Brands.Where(x => x.Id == (int)id).FirstOrDefault();

            BrandModel.BrandsCollection = context.Brands.ToList();
            return View(BrandModel);
        }

        [HttpPost]
        public IActionResult Edit([Bind]EditBrandModel model)
        {
            // If Brand contains an Id, update it, else create new brand!
            if (model.Brand.Id > 0)
                context.Update<Brand>(model.Brand);
            else
                context.Add<Brand>(model.Brand);

            context.SaveChanges();
            return RedirectToAction("index", "Brand");
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