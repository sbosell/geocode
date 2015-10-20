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

       [HttpPost]
        public ActionResult Query(FormCollection collection)
        {
            string key = string.Empty;
            string query = string.Empty;
            string provider = "Google";
            IGeoProviderConfig config;
            IGeoProvider GeoProvider = null;

           IGeoCodeResult result = new GeoCodeResult();
            if (collection.HasKeys() && collection["Query"] != null && collection["Providers"]!=null)
            {
                query = collection["Query"];
                // we have a search
                provider = collection["Providers"].Replace("KEY", "");
                if (collection["Providers"] != null && collection["Providers"].ToString().Contains("KEY"))
                {
                    key = collection["Key"];
                }

                switch (provider)  {
                    case "Google":
                        config = new GoogleGmapConfig();
                        GeoProvider = new GoogleGmap(config);
                    break;
                    case "Bing":
                        config = new BingMapConfig().SetKey(key);
                        GeoProvider = new BingMap(config);
                    break;
                    case "MapQuest":
                    config = new MapQuestConfig().SetKey(key);
                    GeoProvider = new MapQuestMap(config);
                        break;
                    case "Open Streets":
                        config = new OpenStreetMapConfig().SetUserAgent("your email here yo");
                        GeoProvider = new OpenStreetMap(config);
                    break;
                    case "YahooPlaces":
                        config = new YahooPlaceFinderConfig().SetKey(key);
                        GeoProvider = new YahooPlaceFinder(config);
                    break;
                    case "CloudMade":
                        config = new CloudMadeConfig().SetKey(key);
                        GeoProvider = new CloudMade(config);
                    break;

                }

                GeoProvider = GeoProvider != null ? GeoProvider : new GoogleGmap();

                GeoCoder gc = new GeoCoder(GeoProvider);

                result = gc.GetCoordinates(query);
               

            }

            return Json(result);
        }

       

        //
        // GET: /GeoCode/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /GeoCode/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /GeoCode/Create

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /GeoCode/Edit/5

        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /GeoCode/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /GeoCode/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /GeoCode/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
