using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Lucuma.Libs.Config;
using Lucuma.Helper;

namespace Lucuma.Libs
{

    public class CloudMadeConfig: GeoProviderConfig, IGeoProviderConfig
    {
        // required for each config class to define its default url pattern.. Can be overriden
        public override void SetDefaults()
        {
            UrlPattern = "http://geocoding.cloudmade.com/{API-KEY}/geocoding/v2/find.js"; // default 
            this.SetSearchQuery("query");
            this.SetKeyRoute("API-KEY");
        }


    }

    public class CloudMade : GeoProvider, IGeoProvider
    {

        public CloudMade()
        {
            Config = new CloudMadeConfig();
        }
        public CloudMade(IGeoProviderConfig gc)
        {
            Config = gc;
        }

        public IGeoCodeResult GetCoordinates(string search)
        {
            GeoCodeResult gResult = new GeoCodeResult();
            gResult.Library = this.GetType().ToString();
            try
            {
                string url = BuildSearchQuery(search); //1600+Amphitheatre+Parkway,+Mountain+View,+CA
                string json = new WebRequester(url).GetData();
                if (!String.IsNullOrEmpty(json))
                {
                    JObject result = (JObject)JsonConvert.DeserializeObject(json);
                    if (result["found"].Value<int>() > 0)
                    {
                        foreach (var loc in result["features"])
                        {
                            Location Location = new Location();

                            var properties = loc["properties"];
                            if (properties != null)
                            {
                                Location.Name = properties["name"].GetStringValue();

                                Location.Address = Location.Name;
                                Location.Type = properties["place"].GetStringValue();
                 
                            }

                            var centroid = loc["centroid"];
                            if (centroid != null)
                            {
                                var coordinates = centroid["coordinates"];
                                if (coordinates != null && coordinates.Count() > 1)
                                {
                                    Location.Latitude = coordinates[0].GetDoubleValue();
                                    Location.Longitude = coordinates[1].GetDoubleValue();
                                }

                            }
                        
                            gResult.Locations.Add(Location);
                        }
                        

                    }
                    else
                    {
                        gResult.Error = "No results";
                    }
                }

            }
            catch (Exception e)
            {
                gResult.Error = e.Message;
            }

            return gResult;
        }
    }
}
