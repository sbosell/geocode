using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Lucuma;
using Lucuma.Libs;
using Lucuma.Libs.Config;

namespace GeoCodeExample.Controllers
{
    public class GeoCodeController : Controller
    {
        //
        // GET: /GeoCode/

        public ActionResult Index()
        {
            List<SelectListItem> Providers = new List<SelectListItem>();
            Providers.Add(new SelectListItem() { Text = "Google", Selected = true, Value = "Google" });
            Providers.Add(new SelectListItem() { Text = "Bing", Selected = false, Value = "BingKEY" });
            Providers.Add(new SelectListItem() { Text = "MapQuest", Selected = false, Value = "MapQuestKEY" });
            Providers.Add(new SelectListItem() { Text = "Open Streets", Selected = false, Value = "Open Streets" });
            Providers.Add(new SelectListItem() { Text = "Cloud Made", Selected = false, Value = "CloudMadeKEY" });
            Providers.Add(new SelectListItem() { Text = "Yahoo Places", Selected = false, Value = "YahooPlacesKEY" });

            ViewBag.Providers = Providers;
            return View();
        }
    }
}
