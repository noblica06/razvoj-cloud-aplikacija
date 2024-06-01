using RSMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RSMVC.Controllers
{
    public class HomeController : Controller
    {
        // Sample data for themes
        private static List<Theme> themes = new List<Theme>
        {
            new Theme { Id = 1, Title = "Dark Mode", Description = "A dark theme for night time browsing.", CreatedDate = DateTime.Now.AddDays(-10) },
            new Theme { Id = 2, Title = "Light Mode", Description = "A light theme for day time browsing.", CreatedDate = DateTime.Now.AddDays(-5) }
        };

        public ActionResult Index()
        {
            ViewBag.Themes = themes;
            //return RedirectToAction("Index", "Home");
            return View();
         }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult CreateTheme()
        {
            // Logic to create a new theme
            return View();
        }

        public ActionResult Details(int id)
        {
            var theme = themes.Find(t => t.Id == id);
            return View(theme);
        }
    }
}