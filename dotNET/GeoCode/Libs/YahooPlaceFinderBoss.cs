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
    public class YahooPlaceFinderBossConfig : GeoProviderConfig, IGeoProviderConfig
    {
        // required for each config class to define its default url pattern.. Can be overriden
        public override void SetDefaults()
        {
            UrlPattern = "http://yboss.yahooapis.com/geo/placefinder?flags=J"; // default 
            this.SetSearchQuery("location");
            this.SetKeyQuery("appid");
            this.OAuthRequired = true;

        }


    }
    public class YahooPlaceFinderBoss :  GeoProvider,  IGeoProvider
    {
        public YahooPlaceFinderBoss()
        {
            Config = new YahooPlaceFinderBossConfig();
        }
        public YahooPlaceFinderBoss(IGeoProviderConfig gc)
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
                    if (result["bossresponse"]["placefinder"]["results"].Count()>0)
                    {
                        foreach (var loc in result["bossresponse"]["placefinder"]["results"])
                        {

                            Location Location = new Location();
                            try
                            {
                                string[] AddressStrings = new string[] {
                                loc["house"].GetStringValue(),
                                loc["street"].GetStringValue(),
                                loc["xstreet"].GetStringValue(),
                                loc["unit"].GetStringValue(),
                                loc["unittype"].GetStringValue(),
                                loc["postal"].GetStringValue(),
                                loc["neighborhood"].GetStringValue(),
                                loc["house"].GetStringValue(),
                                loc["city"].GetStringValue(),
                                loc["uzip"].GetStringValue(),
                                loc["county"].GetStringValue(),
                                loc["state"].GetStringValue(),
                                loc["country"].GetStringValue()
                                };
                                Location.Address = String.Join(",", AddressStrings.Where(s => !string.IsNullOrEmpty(s)));
                            }
                            catch { }


                                                        
                            Location.Type = String.Concat(loc["woetype"].GetStringValue());

                            Location.Latitude = loc["latitude"].GetDoubleValue();
                            Location.Longitude = loc["longitude"].GetDoubleValue();
                            Location.Accuracy = loc["quality"].Value<int>().ToString();
                            Location.Name = String.Concat(loc["line2"].GetStringValue(), loc["line4"].GetStringValue());
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
