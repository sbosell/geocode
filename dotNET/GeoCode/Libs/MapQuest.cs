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
    public class MapQuestConfig : GeoProviderConfig, IGeoProviderConfig
    {

        public override void SetDefaults()
        {
            UrlPattern = "http://www.mapquestapi.com/geocoding/v1/address?inFormat=kvp&outFormat=json"; // default
            this.SetKeyQuery("key");
            this.SetSearchQuery("location");
        }

    }

    public class MapQuestMap : GeoProvider,IGeoProvider
    {
        
        public MapQuestMap()
        {
            Config = new MapQuestConfig();
        }
        public MapQuestMap(IGeoProviderConfig bconfig)
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
                        if (result["results"][0]["locations"].Count() > 0)
                        {
                            foreach (var loc in result["results"][0]["locations"])
                            {
                                try
                                {
                                    Location l = new Location();
                                    var latLng = loc["latLng"];
                                    if (latLng != null)
                                    {
                                        l.Latitude = latLng["lat"].GetDoubleValue();
                                        l.Longitude = latLng["lng"].GetDoubleValue();

                                    }
                                    
                                    l.Accuracy = loc["geocodeQuality"].GetStringValue();

                                    String[] AddressTemp = new string[]
                                    {
                                        loc["street"].GetStringValue(),
                                        loc["adminArea5"].GetStringValue(),
                                        loc["adminArea4"].GetStringValue(),
                                        loc["postalCode"].GetStringValue(),
                                        loc["adminArea1"].GetStringValue()

                                    };

                                    l.Type = loc["type"].GetStringValue();
                                    l.Address = String.Join(",", AddressTemp.Where(s => !String.IsNullOrEmpty(s)));
                                    l.Name = l.Address;

                                    gResult.Locations.Add(l);
                                }
                                catch
                                {

                                }
                            }
                         
                        }
                        //gResult.Count = 1;

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

