using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Configuration;
using Lucuma;
using Lucuma.Libs;
using Lucuma.Libs.Config;

namespace GeoCodeTest
{
    [TestClass]
    public class GeoCodingTests
    {
        public string BingKey { get; set; }
        public string MapQuestKey { get; set; }
        public string YahooAppId { get; set; }
        public string CloudMadeApi { get; set; }
        public string BossKey { get; set; }
        public string BossSecret { get; set; }

        public GeoCodingTests()
        {
            //HACKEY WAY TO SET VALUES
            try
            {
                BingKey = ConfigurationManager.AppSettings["bingkey"];
                MapQuestKey = ConfigurationManager.AppSettings["mapquestkey"];
                YahooAppId = ConfigurationManager.AppSettings["yahooappid"];
                CloudMadeApi = ConfigurationManager.AppSettings["cloudmadeapi"];
                BossKey = ConfigurationManager.AppSettings["bosskey"];
                BossSecret = ConfigurationManager.AppSettings["bosssecret"];
            }
            catch
            {
                BingKey = "YOUR BING KEY";
                MapQuestKey = "YOUR MAPQUEST KEY";
                YahooAppId = "Your Yahoo App ID";
                CloudMadeApi = "Your Cloud Made API";
                BossKey = "Your Boss Key"; // yahoo boss
                BossSecret = "Your Boss Secret";
            }
        }

        [TestMethod]
        public void CloudMade()
        {

            CloudMade g = new CloudMade(new CloudMadeConfig().SetKey(CloudMadeApi));

            var Expected = new { Latitude = -28.85422, Longitude = 151.16864 };

            IGeoCodeResult Target = new GeoCodeResult();

            Target = g.GetCoordinates("Austin, TX");
            Assert.AreEqual(Expected.Latitude, Target.Latitude);
            Assert.AreEqual(Expected.Longitude, Target.Longitude);
        }

        [TestMethod]
        public void YahooPlaceFinder()
        {

            YahooPlaceFinder g = new YahooPlaceFinder();

            var Expected = new { Latitude = 30.267605, Longitude =  -97.742984 };
            
            IGeoCodeResult Target = new GeoCodeResult();

            Target = g.GetCoordinates("Austin, TX");
            Assert.AreEqual(Expected.Latitude, Target.Latitude);
            Assert.AreEqual(Expected.Longitude, Target.Longitude);
        }


        [TestMethod]
        public void YahooPlaceFinderBoss()
        {
            string consumerKey = BossKey;
            string consumerSecret = BossSecret;
            string yahooappid = YahooAppId;



            YahooPlaceFinderBossConfig ypc =
                new YahooPlaceFinderBossConfig()
                .SetConsumerKey(consumerKey)
                .SetSecret(consumerSecret)
                .SetKey(yahooappid);

            YahooPlaceFinderBoss g = new YahooPlaceFinderBoss(ypc);

            var  Expected = new
            {
                Latitude = 30.267599,
                Longitude = -97.742981
            };
            IGeoCodeResult Target = g.GetCoordinates("Austin, TX");
            
            Assert.AreEqual(Expected.Latitude, Target.Latitude);
            Assert.AreEqual(Expected.Longitude, Target.Longitude);
        }

        [TestMethod]
        public void GeoCoderYahooPlaceFinder()
        {
            YahooPlaceFinderConfig yph = new YahooPlaceFinderConfig().SetKey(YahooAppId);
            YahooPlaceFinder ypf = new Lucuma.Libs.YahooPlaceFinder(yph);

            GeoCoder g = new GeoCoder(ypf);

           var Expected = new {
               Latitude = 30.267605,
               Longitude = -97.742984
           };

            IGeoCodeResult Target = new GeoCodeResult();

            Target = g.GetCoordinates("Austin, TX");
            Assert.AreEqual(Expected.Latitude, Target.Latitude);
            Assert.AreEqual(Expected.Longitude, Target.Longitude);
        }

        
        [TestMethod]
        public void GoogleGmap()
        {
            
            GoogleGmap g = new GoogleGmap();
           
            var Expected = new {
            Latitude = 30.267153,
            Longitude = -97.7430608
        };
           
            IGeoCodeResult Target = new GeoCodeResult();

            Target = g.GetCoordinates("Austin, TX");
            Assert.AreEqual(Expected.Latitude, Target.Latitude);
            Assert.AreEqual(Expected.Longitude, Target.Longitude);
           
        }

        [TestMethod]
        public void GoogleGmapAmbiguous()
        {

            GoogleGmap g = new GoogleGmap();

            IGeoCodeResult Target = new GeoCodeResult();

            Target = g.GetCoordinates("Paris");
            Assert.IsTrue(Target.Count>0);
        }

        [TestMethod]
        public void GoogleMapNoResults()
        {
            GoogleGmapConfig gmc = new GoogleGmapConfig();

            GoogleGmap g = new GoogleGmap(gmc);

            IGeoCodeResult Target = new GeoCodeResult();

            Target = g.GetCoordinates("alskdfjkaadsflasd");
            Assert.AreEqual(false, Target.HasValue);
            Assert.AreEqual(0, Target.Longitude);

        }
        [TestMethod]
        public void OpenStreetsMap()
        {
            OpenStreetMap o = new OpenStreetMap();

            var Expected = new
            {
                Latitude = 30.1968545,
                Longitude = -97.5975533560703
            };

            IGeoCodeResult Target = new GeoCodeResult();

            Target = o.GetCoordinates("Austin, TX");
            Assert.AreEqual(Expected.Latitude, Target.Latitude);
            Assert.AreEqual(Expected.Longitude, Target.Longitude);

        }

        [TestMethod]
        public void OpenStreetsMapNoResults()
        {
            OpenStreetMap o = new OpenStreetMap();

            IGeoCodeResult Expected = new GeoCodeResult();
            Expected.Latitude = 30.1968545;
            Expected.Longitude = -97.5975533560703;

            IGeoCodeResult Target = new GeoCodeResult();

            Target = o.GetCoordinates("asdfasdfaasdfasdfasd;fja");
            Assert.AreEqual(Target.HasValue, false);

        }

        [TestMethod]
        public void BingMapTest()
        {
            BingMapConfig bmc = new BingMapConfig()
                .SetKey(BingKey);

            BingMap bing = new BingMap(bmc);

            var Expected = new {
                Latitude = 30.267599105834961,
                Longitude = -97.74298095703125
            };
            IGeoCodeResult Target = new GeoCodeResult();

            Target = bing.GetCoordinates("Austin, TX");
            Assert.AreEqual(Expected.Latitude, Target.Latitude);
            Assert.AreEqual(Expected.Longitude, Target.Longitude);

        }

        //http://dev.virtualearth.net/REST/v1/Locations/locationQuery?includeNeighborhood=includeNeighborhood&maxResults=maxResults&include=queryParse&key=BingMapsKey
        [TestMethod]
        public void BingMapTestAlternateUrl()
        {
            BingMapConfig bmc = new BingMapConfig()
                .SetKey(BingKey)
                .SetUrl("http://dev.virtualearth.net/REST/v1/Locations/?maxResults=1")
                .SetSearchQuery("q")
                .SetKeyQuery("key");

            BingMap bing = new BingMap(bmc);

            var Expected = new {
            Latitude = 30.267599105834961,
            Longitude = -97.74298095703125
        };

            IGeoCodeResult Target = new GeoCodeResult();

            Target = bing.GetCoordinates("Austin, TX");
            Assert.AreEqual(Expected.Latitude, Target.Latitude);
            Assert.AreEqual(Expected.Longitude, Target.Longitude);

        }

        [TestMethod]
        public void BingMapNoResults()
        {
            BingMapConfig bmc = new BingMapConfig()
                .SetKey(BingKey);                

            BingMap bing = new BingMap(bmc);

            IGeoCodeResult Target = new GeoCodeResult();

            Target = bing.GetCoordinates("alskdfjkaadsflasd");
            Assert.AreEqual(false, Target.HasValue);
            Assert.AreEqual(0, Target.Longitude);

        }


        [TestMethod]
        public void GeoCoderBingTest()
        {
            GeoCoder gc = new GeoCoder();
            BingMapConfig bmc = new BingMapConfig()
                .SetKey(BingKey);

            gc.AddProvider(new BingMap(bmc));  // bing requires a key

            IGeoCodeResult Target = new GeoCodeResult();

           var Expected = new{
            Latitude = 30.267599105834961,
            Longitude = -97.74298095703125
           };

            Target = gc.GetCoordinates("Austin, TX");
            Assert.AreEqual(Expected.Latitude, Target.Latitude);
            Assert.AreEqual(Expected.Longitude, Target.Longitude);

        }


        [TestMethod]
        public void GeoCoderDefault()
        {
            // Google Assert
            GeoCoder gc = new GeoCoder();
            var Expected = new{
            Latitude = 30.267153,
            Longitude = -97.7430608
            };

           
            IGeoCodeResult Target = gc.GetCoordinates("Austin, TX");
            Assert.AreEqual(Expected.Latitude, Target.Latitude);
            Assert.AreEqual(Expected.Longitude, Target.Longitude);

        }

        [TestMethod]
        public void GeoCoderAlternate()
        {
            var Expected = new
            {
                Latitude = 30.1968545,
                Longitude = -97.5975533560703
            };

            IGeoCodeResult Target = new GeoCodeResult();

            GeoCoder gc = new GeoCoder();
            gc.AddProvider(new OpenStreetMap());
            Target = gc.GetCoordinates("Austin, TX");
            Assert.AreEqual(Expected.Latitude, Target.Latitude);
            Assert.AreEqual(Expected.Longitude, Target.Longitude);

        }

        [TestMethod]
        public void GeoCoderFirstFails()
        {
            var Expected = new {
           Latitude = 30.1968545,
           Longitude = -97.5975533560703
        };

            IGeoCodeResult Target = new GeoCodeResult();

            GoogleGmapConfig gmc =
                new GoogleGmapConfig()
                    .SetUrl("http://madps.googleapis.com/maps/api/geocode/json?address={0}&sensor=true");

            GeoCoder gc = new GeoCoder()
                .AddProvider(new GoogleGmap(gmc))
                .AddProvider(new OpenStreetMap());

            
            Target = gc.GetCoordinates("Austin, TX");
            Assert.AreEqual(Expected.Latitude, Target.Latitude);
            Assert.AreEqual(Expected.Longitude, Target.Longitude);

        }

        [TestMethod]
        public void MapQuestMapTest()
        {
            // mapquest is wierd and always returns some kind of result so not sure this test is valid.
            MapQuestConfig mqc = new MapQuestConfig()
                .SetKey(MapQuestKey);

            MapQuestMap MapQuest = new MapQuestMap(mqc);

            var Expected = new{
            Latitude =30.266899,
            Longitude = -97.742798
            };

            IGeoCodeResult Target = new GeoCodeResult();

            Target = MapQuest.GetCoordinates("Austin, TX");
            Assert.AreEqual(Expected.Latitude, Target.Latitude);
            Assert.AreEqual(Expected.Longitude, Target.Longitude);
            
        }
         [TestMethod]
        public void MapQuestMapNoResultsTest()
        {
            MapQuestConfig mqc = new MapQuestConfig()
                .SetKey(MapQuestKey);

            MapQuestMap MapQuest = new MapQuestMap(mqc);

            IGeoCodeResult Expected = new GeoCodeResult();
            Expected.Latitude = 30.266899;
            Expected.Longitude = -97.742798;

            IGeoCodeResult Target = new GeoCodeResult();

            Target = MapQuest.GetCoordinates("zdkthqqad,aads,a");

            // mapquest seems to always return some result
        }


    }
}
