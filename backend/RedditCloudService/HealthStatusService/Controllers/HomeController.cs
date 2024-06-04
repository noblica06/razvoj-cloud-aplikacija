using DataLibrary.HealthCheck;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HealthStatusService.Controllers
{
    public class HomeController : Controller
    {
        HealthCheckRepo healthCheckRepo = new HealthCheckRepo();

        public ActionResult Index()
        {
            List<HealthCheck> lastHour = healthCheckRepo.RetrieveForLastHour().ToList();
            ViewBag.LastHour = lastHour;

            List<HealthCheck> past24Hours = healthCheckRepo.RetrieveForPasth24Hours().ToList();
            List<HealthCheck> okStatus24Hours = healthCheckRepo.RetrieveForPasth24Hours().Where(hc => hc.Status == "OK").ToList();

            double upTimePercentage = (okStatus24Hours.Count * 1.0 / past24Hours.Count * 1.0) * 100;
            ViewBag.UpTimePercetange = upTimePercentage;
            return View();
        }
    }
}