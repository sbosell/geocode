using Lucuma;
using Lucuma.Libs;
using Lucuma.Libs.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace GeoCodeExample.Controllers
{

    public class GeoQuery
    {
        public string Providers { get; set; }
        public string Key { get; set; }
        public string Query { get; set; }


    }
    public class GeoController : ApiController
    {
        // GET api/<controller>
        public IGeoCodeResult Query(GeoQuery q)
        {
            string key = string.Empty;
            string query = string.Empty;
            string provider = "Google";
            IGeoProviderConfig config;
            IGeoProvider GeoProvider = null;

            IGeoCodeResult result = new GeoCodeResult();


            if (!String.IsNullOrEmpty(q.Query) && !String.IsNullOrEmpty(q.Providers))
            {
                query = q.Query;
                // we have a search
                provider = q.Providers.Replace("KEY", "");
                if (q.Providers.Contains("KEY"))
                {
                    key = q.Key;
                }

                switch (provider)
                {
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
            return result;
        }

        
    }
}