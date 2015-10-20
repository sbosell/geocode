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


    public enum GoogleResultStatus
    {
        OK = 0,
        ZERO_RESULTS = 1,
        OVER_QUERY_LIMIT = 2,
        REQUEST_DENIED = 3,
        INVALID_REQUEST = 4
    }

    public class GoogleGmapConfig : GeoProviderConfig, IGeoProviderConfig
    {
        // required for each config class to define its default url pattern.. Can be overriden
        public override void SetDefaults()
        {
            UrlPattern = "http://maps.googleapis.com/maps/api/geocode/json?sensor=true"; // default 
            this.SetSearchQuery("address");
        }

        
    }

    public class GoogleGmap :  GeoProvider, IGeoProvider
    {
       
        public GoogleGmap()
        {
            Config = new GoogleGmapConfig();
        }
        public GoogleGmap(IGeoProviderConfig gc)
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
                    if (result["status"].GetStringValue() == GoogleResultStatus.OK.ToString() && result["results"].Count()>0)
                    {
                        
                        foreach (var loc in result["results"])
                        {
                            Location Location = new Location();


                            Location.Address = loc["formatted_address"].GetStringValue();
                           
                            var geometry = loc["geometry"];
                            if (geometry != null)
                            {
                                var location = geometry["location"];
                                if (location != null)
                                {
                                    Location.Latitude = location["lat"].GetDoubleValue();
                                    Location.Longitude = location["lng"].GetDoubleValue();
                                }

                                Location.Type = location["location_type"].GetStringValue(); 
                                
                            }
                            
                            var addresscomponents = loc["address_components"];
                            if (addresscomponents != null && addresscomponents.Count() > 0)
                            {
                                Location.Name = loc["address_components"][0]["long_names"].GetStringValue(); //!String.IsNullOrEmpty(loc["address_components"][0]["long_name"].Value<String>()) ? loc["address_components"][0]["long_name"].Value<String>() : string.Empty;
                            }
                            gResult.Locations.Add(Location);
                        }
                       
                      
                    }
                    else
                    {
                        gResult.Error = result["status"].ToString();
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
