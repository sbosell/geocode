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
    public class BingMapConfig : GeoProviderConfig, IGeoProviderConfig
    {
        public override void SetDefaults()
        {
            UrlPattern = "http://dev.virtualearth.net/REST/v1/Locations"; // default
            this.SetSearchQuery("q");
            this.SetKeyQuery("key");
        }
      
    }

    public class BingMap : GeoProvider, IGeoProvider
    {

        public BingMap()
        {
            Config = new BingMapConfig();
        }
        public BingMap(IGeoProviderConfig bconfig)
        {
            Config = bconfig;
        }

        public IGeoCodeResult GetCoordinates(string search)
        {
            GeoCodeResult gResult = new GeoCodeResult();
            gResult.Library = this.GetType().ToString();
            if (!String.IsNullOrEmpty(Config.Key))
            {
                try
                {
                    string url = BuildSearchQuery(search); //135+pilkington+avenue,+birmingham
                    string json = new WebRequester(url).GetData();
                    if (!String.IsNullOrEmpty(json))
                    {
                        JObject result = (JObject)JsonConvert.DeserializeObject(json);
                        if (result["resourceSets"][0]["resources"].Count() > 0)
                        {
                            foreach (var loc in result["resourceSets"][0]["resources"])
                            {
                                Location l = new Location();
                                String AddressTemp = String.Empty;
                                l.Name = loc["name"].GetStringValue();

                                var point = loc["point"];
                                if (point != null && point.Count()>1)
                                {
                                    l.Latitude = point["coordinates"][0].GetDoubleValue();
                                    l.Longitude = point["coordinates"][1].GetDoubleValue();
                                }

                                l.Accuracy = loc["confidence"].GetStringValue();
                                if (loc["address"].HasValues)
                                {
                                    AddressTemp = String.Join(",", loc["address"].Values().Where(add => !string.IsNullOrEmpty(add.Value<string>())));
                                }
                                l.Address = AddressTemp;
                                gResult.Locations.Add(l);
                            }
                            
                            //gResult.Count = 1; // for now we are only dealing with the first result
                        }
                        else
                        {
                            gResult.Error = "No Results";

                        }

                    }
                        
                    
                }
                catch
                {

                }
            }
            return gResult;
        }
    }
}

    