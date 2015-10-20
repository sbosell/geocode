using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Lucuma.Helper;
using Lucuma.Libs.Config;

namespace Lucuma.Libs
{

    public class OpenStreetMapConfig : GeoProviderConfig, IGeoProviderConfig
    {
       
        public override void SetDefaults()
        {
            UrlPattern = "http://nominatim.openstreetmap.org/search?format=json"; // default
            this.SetSearchQuery("q");
            UserAgent = "yourname@youremail.com";
        }
    }

    public class OpenStreetMap : GeoProvider, IGeoProvider
    {
      

        public OpenStreetMap()
        {
            Config = new OpenStreetMapConfig();
        }
        public OpenStreetMap(IGeoProviderConfig oconfig)
        {
            Config = oconfig;
        }

        public  IGeoCodeResult GetCoordinates(string search)
        {
            GeoCodeResult gResult = new GeoCodeResult();
            gResult.Library = this.GetType().ToString();
            try
            {
                string url = BuildSearchQuery(search); //135+pilkington+avenue,+birmingham
                string json = new WebRequester(url, Config.UserAgent).GetData();  // Open Street Maps wants the agent name set

                if (!String.IsNullOrEmpty(json))
                {
                    JArray result = (JArray)JsonConvert.DeserializeObject(json);
                    if (result.Count() > 0)
                    {
                        foreach (var loc in result)
                        {
                            Location l = new Location();
                            l.Name = loc["display_name"].GetStringValue();
                            l.Latitude = loc["lat"].GetDoubleValue();
                            l.Longitude = loc["lon"].GetDoubleValue();
                            l.Type = loc["type"].GetStringValue();
                            l.Address = l.Name;
                            gResult.Locations.Add(l);
                        }
                        
                        //gResult.Count = 1;
                    }
                }
            }


            catch
            {

            }
            return gResult;
        }
    }
}
