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
    //
      public class YahooPlaceFinderConfig : GeoProviderConfig, IGeoProviderConfig
    {
        // required for each config class to define its default url pattern.. Can be overriden
        public override void SetDefaults()
        {
            UrlPattern = "http://where.yahooapis.com/geocode?flags=J"; // default 
            this.SetSearchQuery("q");
            this.SetKey("apiid");
        }

        
    }

    public class YahooPlaceFinder :  GeoProvider, IGeoProvider
    {
       
        public YahooPlaceFinder()
        {
            Config = new YahooPlaceFinderConfig();
        }
        public YahooPlaceFinder(IGeoProviderConfig gc)
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
                    if (result["ResultSet"]["Found"].Value<int>() > 0)
                    {
                        foreach (var loc in result["ResultSet"]["Results"])
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
                            Location.Accuracy = loc["quality"].GetStringValue();
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


