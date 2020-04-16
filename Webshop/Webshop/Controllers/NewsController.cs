using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Webshop.Models;
using Webshop.Services;

namespace Webshop.Controllers
{
    public class NewsController : Controller
    {

        private readonly WebAPIHandler webAPI;
        private readonly WebAPIToken webAPIToken;

        public NewsController(WebAPIHandler webAPI, WebAPIToken webAPIToken)
        {
            this.webAPI = webAPI;
            this.webAPIToken = webAPIToken;
        }

        public async Task<ActionResult> Index()
        {
            var news = await webAPI.GetAllAsync<News>(ApiURL.NEWS);
            return View(news);
        }

        public IActionResult AddArticle()
        {
            return View();
        }

        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                TempData["ArticleError"] = "Kunde inte radera artikel! Artikel-Id saknas!";
                return View();
            }

            News news = await GetArticleById((int)id);

            return View(news);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id <= 0)
            {
                TempData["ArticleError"] = "Kunde inte radera artikel! Artikel-Id saknas!";
                return RedirectToAction("Index");
            }

            News news = await GetArticleById((int)id);
            return View(news);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Update([Bind]News model)
        {
            var token = await webAPIToken.New();
            var result = await webAPI.UpdateAsync<News>(model, ApiURL.NEWS + model.Id, token);

            TempData["Article"] = "Nyhetsartikeln har uppdaterats";

            return RedirectToAction("Index");
        }


        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> AddArticle([Bind]News model)
        {
            if (ModelState.IsValid)
            {
                // Add current date to article
                model.NewsDate = DateTime.UtcNow;

                var token = await webAPIToken.New();
                var result = await webAPI.PostAsync(model, ApiURL.NEWS, token);

                TempData["Article"] = "Ny artikel har skapats";

                return RedirectToAction("Index");
            }
            else
                return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Delete(News model)
        {
            if (model.Id <= 0)
            {
                TempData["ArticleError"] = "Kunde inte radera artikel! Artikel-Id saknas!";
                return View();
            }

            var token = await webAPIToken.New();
            var result = await webAPI.DeleteAsync(ApiURL.NEWS + model.Id, token);

            if (result)
                TempData["Article"] = "Artikeln har raderats";
            else
                TempData["ArticleError"] = "Ett fel uppstod, kunde inte radera artikeln! kontakta support om felet kvarstår.";

            return RedirectToAction("Index");
        }


        private async Task<News> GetArticleById(int id)
            => await webAPI.GetOneAsync<News>(ApiURL.NEWS + id);
    }
}