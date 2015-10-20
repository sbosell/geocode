Multiple Serverside Geocoder - Beta Release

NuGet Download: https://nuget.org/packages/Lucuma.GeoCode/

A sample c# mvc site that uses the geocoders: http://multigeocoder.azurewebsites.net/

A sample nodejs site that uses the geocoders: http://multigeocodernode.azurewebsites.net/

This is a serverside wrapper library for .Net and Nodejs (very dirty and alpha) to some common geocoding services that allow the conversion of an address/place into its latitude and longitude from the server (not client/javascript).  This library in its current form returns basic information (latitude and longitude) and only returns some basic information such as accuracy, address, etc when available or applicable.  It will also return a list of the ambiguous results when available.

It currently supports 6 free services (some require api key) and allows you to specify failovers in the event you get throttled.  Many projects and websites show maps and allow users to enter city,state and/or location to display them and it is a general requirement to geocode that information first.  If you need to do it on the server side, this library will help you.

If you find this helpful, please rate it or leave some feedback on the discussion tab.  It would be appreciated.

The current geocoding services that are supported are:

Google Gmap - The default service used since it doesn't require an api key
Open Streets Map - No Api key required
Bing - API key required
MapQuest - API key required
Yahoo Place Finder - API Key required - It seems this service is being deprecated for the Yahoo! BOSS Geoservice
Cloud Made - API Key required
Yahoo BOSS Geo Services - Using the yahoo boss geo service. This one is not free and requires a consumer key, consumer secret, and app id from yahoo as well as some payment information.  See link for details.
If you'd like me to add another service, log it in the discussion or issue tracker and I'll take a look.  Please include a link to the documentation.

On to the code......

## C#

In it's simplest form:

    GeoCoder gc = new GeoCoder();  // defaults to google
    IGeoCodeResult Target = gc.GetCoordinates("Austin, TX");
    if (Target.HasValue) {
       Target.Latitude;  // contains latitude
       Target.Longitude;  // contains longitude
       Target.Locations; // list of all the locations if more than one
    }
    
I've worked on projects that have triggered throttling and required a backup/failover service.  Here is how that can be achieved:

	 GeoCoder gc = new GeoCoder()
  	  .AddProvider(new GoogleGmap())   
  	  .AddProvider(new OpenStreetMap());

	IGeoCodeResult Target = gc.GetCoordinates("Austin, TX");

    // we should get back the openStreetMap result since Google has a bad URL if (Target.HasValue) {   Target.Latitude;   // do something with the values Target.Longitude; }

Using Yahoo! Boss GEO:
 
            string consumerKey = "Your consumer key";
            string consumerSecret = "Your boss secret";
            string yahooappid = "Yahoo App ID";

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
        

## Nodejs

The NodeJs project is a little barren at the moment however the library works and there is a sample. 

    var g = new GeoCoder();
    g.GetCoordinates('Austin, TX', function (data) {
        console.log('Default Test')
        console.log(data);
        console.log('End Default Test');
    });
 

There are more examples on the documentation tab and in the unit tests project.  Please note that each service has its own terms of use.  For instance on the non enterprise free plan, Google throttles you after   requests in a 24 hour period (2012 it probably has changed) and requires that you use their map if you use their geocoding service.   Make sure and review the terms of the service(s) you are using. 

This project may change a bit as there are some things that need to be cleaned up, however it should only be minor cleanup.  

