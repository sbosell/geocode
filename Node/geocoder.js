var http = require('http');
var _url = require('url');
var async = require('async');
var OAuth = require('oauth').OAuth;
var crypto = require('crypto');
var bases = require('bases');

var _ = require('underscore');
serialize = function (obj) {
    var str = [];
    for (var p in obj)
        str.push(encodeURIComponent(p) + "=" + encodeURIComponent(obj[p]));
    return str.join("&");
}

function Location() {
    this.accuracy = '';

    this.latitude = 0;
    this.longitude = 0;

    this.type = '';
    this.name = '';
    this.address = '';

}

function Results() {

    this.locations = [];
    this.serviceurl='';
    this.servicereturn = '';
    this.library = '';
    this.address = function() {
        if (this.HasValue()) {
            return this.locations[0].address;
        }
    }
    this.accuracy = function () {
        if (this.HasValue()) {
            return this.locations[0].accuracy;
        }
    }
    this.type = function () {
        if (this.HasValue()) {
            return this.locations[0].type;
        }
    }
    this.HasValue = function () {

        return this.locations.length > 0;
    }

    this.name = function () {
        if (this.HasValue()) {
            
            return this.locations[0].name != null ? this.locations[0].name : '';
        }
    }


    this.latitude = function () {
        var self = this;
        if (self.locations.length > 0)
            return self.locations[0].latitude;

        return 0;
    }
    this.longitude = function () {
        var self = this;
        if (self.locations.length > 0) {
            return self.locations[0].longitude;
        }
        return 0;
    }

    this.hasvalue = function () {
        return this.results.locations.length > 0;

    }

}

function GeoProviderConfig() {
    var self = this;
    this.UrlPattern = '';
    this.Key = '';
    this.SearchQuery = '';
    this.KeyQuery = '';
    this.UserAgent = '';
    this.KeyRoute = '';
    this.Secret = '';
    this.SecretQuery = ''
    this.ConsumerKey = '';
    this.OAuthRequired = false;
    this.UrlValues = {};
    this.QueryString = {};
    this.Url = function () {
        return self.UrlPattern;
    }

    this.SetDefaults = function () {
        // override this bad boy
    }
    this.SetUrl = function (url) {
        this.UrlPattern = url;
        return this;
    }
    this.SetKey = function (key) {
        this.Key = key;

        return this;
    }
    this.SetConsumerKey = function (key) {
        this.ConsumerKey = key;

        return this;
    }

    this.SetSearchQuery = function (searchquery) {
        this.SearchQuery = searchquery;

        return this;
    }
    this.SetKeyQuery = function (keyquery) {
        this.KeyQuery = keyquery;

        return this;
    }
    this.SetKeyRoute = function (keyroute) {
        this.KeyRoute = keyroute;

        return this;
    }
    this.SetUserAgent = function (useragent) {
        this.UserAgent = useragent;
        return this;
    }

    this.AddQueryString = function (querystring, querystringvalue) {
        this.QueryString[querystring] = querystringvalue;
        return this;
    }

    this.SetSecret = function (secret) {
        this.Secret = secret;
        return this;
    }
    this.SetSecretQuery = function (secretquery) {
        this.SecretQuery = secretquery;
        return this;
    }

    this.ToQueryString = function (oathrequired) {
        return serialize(this.QueryString);
    }

    this.SetDefaults();
}

GoogleGmapProviderConfig = function () {
    var self = new GeoProviderConfig();
    self.SetDefaults = function () {
        self.UrlPattern = "http://maps.googleapis.com/maps/api/geocode/json?sensor=true"; // default 
        self.SetSearchQuery("address");
    }
    self.SetDefaults();

    return self;
}

GoogleGmap = function (configuration) {
    var self = this;
    this.config = {};

    if (typeof (configuration) !== 'undefined') {
        self.config = configuration;
    }
    else
        self.config = new GoogleGmapProviderConfig();

    self = new GeoProvider(this.config);

    self.parser = function (str, callback) {
        var me = this;
        //console.log(str);
        var _results = new Results();


        

        // console.log(results);
        try {
            var results = JSON.parse(str);
        if (results.results.length > 0) {
            for (i = 0; i < results.results.length; i++) {
                var loc = results.results[i];
                var location = new Location();;
                location.name = loc["address_components"][0]["long_name"];
                location.latitude = loc.geometry.location.lat;
                location.longitude = loc["geometry"].location.lng;
                location.type = loc.geometry["location_type"];
                location.accuracy = location.type;
                location.address = loc.formatted_address;

                _results.locations.push(location);
            }
        }
        } catch (err)  {
            //console.log('we failed');
        }
        callback(_results);

    }

    return self;
}

OpenStreetsProviderConfig = function () {
    var self = new GeoProviderConfig();

    self.SetDefaults = function () {
        self.UrlPattern = "http://nominatim.openstreetmap.org/search?format=json"; // default 
        self.SetSearchQuery("q");
    }
    self.SetDefaults();

    return self;
}

OpenStreets = function (configuration) {
    var self = this;
    this.config = {};

    if (typeof (configuration) !== 'undefined')
        self.config = configuration;
    else
        self.config = new OpenStreetsProviderConfig();

    self = new GeoProvider(this.config);




    self.parser = function (str, callback) {
        var me = this;
        
        var _results = new Results();
        try {
        if (str != '') {
            var results = JSON.parse(str);
            

            // console.log(results);
            if (results.length > 0) {
                for (i = 0; i < results.length; i++) {
                    var loc = results[i];
                    var location = new Location();;
                    location.name = loc["display_name"];
                    location.latitude = loc["lat"];
                    location.longitude = loc["lon"];
                    location.type = loc["type"];
                    location.address = loc.name;

                    _results.locations.push(location);
                }
            }
        }
        } catch (err) {
                
        }

        //console.log(_results);
        callback(_results);
    }



    return self;
}

BingMapProviderConfig = function () {
    var self = new GeoProviderConfig();

    self.SetDefaults = function () {
        self.UrlPattern = "http://dev.virtualearth.net/REST/v1/Locations"; // default 
        self.SetSearchQuery("q");
        self.SetKeyQuery("key");
    }
    self.SetDefaults();

    return self;
}

BingMap = function (configuration) {
    var self = this;
    this.config = {};

    if (typeof (configuration) !== 'undefined')
        self.config = configuration;
    else
        self.config = new BingMapProviderConfig();

    self = new GeoProvider(this.config);

    self.parser = function (str, callback) {
        var me = this;
        
        var _results = new Results();
        try {
            if (str != '') {
                var results = JSON.parse(str);
               

                // console.log(results);
                if (results["resourceSets"][0]["resources"].length > 0) {
                    
                    for (i = 0; i < results["resourceSets"][0]["resources"].length; i++) {
                        var loc = results["resourceSets"][0]["resources"][i];

                        var location = new Location();;

                        location.name = loc.name;
                        location.latitude = loc.point.coordinates[0];
                        location.longitude = loc.point.coordinates[0];
                        
                        location.accuracy = loc.confidence;
                        location.address = location.name;

                        _results.locations.push(location);
                    }
                }
            }
        } catch (err) {

        }

        //console.log(_results);
        callback(_results);
    }



    return self;
}

MapQuestProviderConfig = function () {
    var self = new GeoProviderConfig();

    self.SetDefaults = function () {
        self.UrlPattern = "http://www.mapquestapi.com/geocoding/v1/address?inFormat=kvp&outFormat=json"; // default 
        self.SetSearchQuery("location");
        self.SetKeyQuery("key");
    }
    self.SetDefaults();

    return self;
}

MapQuest = function (configuration) {
    var self = this;
    this.config = {};

    if (typeof (configuration) !== 'undefined')
        self.config = configuration;
    else
        self.config = new MapQuestProviderConfig();

    self = new GeoProvider(this.config);

    self.parser = function (str, callback) {
        var me = this;

        var _results = new Results();
        try {
            if (str != '') {
                var result = JSON.parse(str);


                // console.log(results);
                if (result["results"][0]["locations"].length > 0) {

                    for (i = 0; i < result["results"][0]["locations"].length; i++) {
                        var loc = result["results"][0]["locations"][i];

                        var location = new Location();;
                        location.accuracy = loc.geocodeQuality
                        location.type = loc.type;
                        location.latitude = loc.latLng["lat"];
                        location.longitude = loc.latLng["lng"];
                        location.name = '';
                        location.address = '';


                        _results.locations.push(location);
                    }
                }
            }
        } catch (err) {

        }

        //console.log(_results);
        callback(_results);
    }



    return self;
}

CloudMadeProviderConfig = function () {
    var self = new GeoProviderConfig();

    self.SetDefaults = function () {
        self.UrlPattern = "http://geocoding.cloudmade.com/{API-KEY}/geocoding/v2/find.js"; // default 
        self.SetSearchQuery("query");
        self.SetKeyRoute("API-KEY"); // default 

    }
    self.SetDefaults();

    return self;
}

CloudMade = function (configuration) {
    var self = this;
    this.config = {};

    if (typeof (configuration) !== 'undefined')
        self.config = configuration;
    else
        self.config = new CloudMadeProviderConfig();

    self = new GeoProvider(this.config);

    self.parser = function (str, callback) {
        var me = this;

        var _results = new Results();

        try {
            if (str != '') {
                var result = JSON.parse(str);



                if (result.found > 0) {

                    for (i = 0; i < result["features"].length; i++) {
                        var loc = result["features"][i];

                        var location = new Location();

                        location.accuracy = '';
                        
                        location.latitude = loc.centroid.coordinates[0];
                        location.longitude = loc.centroid.coordinates[1];

                        location.type = loc.properties.place;
                        location.name = loc.properties.name;
                        location.address = '';


                        _results.locations.push(location);
                    }
                }
            }
        } catch (err) {

        }


        callback(_results);
    }



    return self;
}

YahooPlaceFinderProviderConfig = function () {
    var self = new GeoProviderConfig();

    self.SetDefaults = function () {
        self.UrlPattern = "http://where.yahooapis.com/geocode?flags=J"; // default 
        self.SetSearchQuery("q");
        self.SetKey("apiid");

    }
    self.SetDefaults();

    return self;
}

YahooPlaceFinder = function (configuration) {
    var self = this;
    this.config = {};

    if (typeof (configuration) !== 'undefined')
        self.config = configuration;
    else
        self.config = new YahooPlaceFinderProviderConfig();

    self = new GeoProvider(this.config);

    self.parser = function (str, callback) {
        var me = this;

        var _results = new Results();
        
        try {
            if (str != '') {
                var result = JSON.parse(str);


               
                if (result["ResultSet"]["Found"] > 0) {

                    for (i = 0; i < result["ResultSet"]["Results"].length; i++) {
                        var loc = result["ResultSet"]["Results"][i];

                        var location = new Location();;
                        location.accuracy = loc.quality;
                        location.type = loc.woetype;
                        location.latitude = loc.latitude;
                        location.longitude = loc.longitude;
                        location.name = loc.line2 + ',' +loc.line4;
                        location.address = '';


                        _results.locations.push(location);
                    }
                }
            }
        } catch (err) {

        }

        
        callback(_results);
    }



    return self;
}

YahooPlaceFinderBossProviderConfig = function () {
    var self = new GeoProviderConfig();

    self.SetDefaults = function () {
        self.UrlPattern = "http://yboss.yahooapis.com/geo/placefinder?flags=J"; // default 
        self.SetSearchQuery("location");
        self.SetKeyQuery("appid");
        self.OAuthRequired = true;

    }
    self.SetDefaults();

    return self;
}

YahooPlaceFinderBoss = function (configuration) {
    var self = this;
    this.config = {};

    if (typeof (configuration) !== 'undefined')
        self.config = configuration;
    else
        self.config = new YahooPlaceFinderBossProviderConfig();

    self = new GeoProvider(this.config);

    self.parser = function (str, callback) {
        var me = this;

        var _results = new Results();
        
        try {
            if (str != '') {
                var result = JSON.parse(str);



                if (result["bossresponse"]["placefinder"]["results"].length > 0) {

                    for (i = 0; i < result["bossresponse"]["placefinder"]["results"].length; i++) {
                        var loc = result["bossresponse"]["placefinder"]["results"][i];

                        var location = new Location();;
                        location.accuracy = loc.quality;
                        location.type = loc.woetype;
                        location.latitude = loc.latitude;
                        location.longitude = loc.longitude;
                        location.name = loc.line2 + ',' + loc.line4;
                        location.address = '';


                        _results.locations.push(location);
                    }
                }
            }
        } catch (err) {

        }


        callback(_results);
    }



    return self;
}
// 
function argsToEncodedData(args) {
    var escapedArgs = {}, keys = [], key, kvPairs = [];
    for (key in args) {
        key = encodeURIComponent(key);
        if (key in escapedArgs) {
            continue;
        }
        escapedArgs[key] = encodeURIComponent(args[key]);
        keys.push(key);
    }

    keys = keys.sort();
    for (var idx in keys) {
        key = keys[idx];
        kvPairs.push(key + '=' + escapedArgs[key]);
    }

    return kvPairs.join('&');
}

// see https://gist.github.com/3095925
function randomStr(length) {
    var maxNum = Math.pow(62, length);
    var numBytes = Math.ceil(Math.log(maxNum) / Math.log(256));
    if (numBytes === Infinity) {
        throw new Error('Length too large; caused overflow: ' + length);
    }

    do {
        var bytes = crypto.randomBytes(numBytes);
        var num = 0
        for (var i = 0; i < bytes.length; i++) {
            num += Math.pow(256, i) * bytes[i];
        }
    } while (num >= maxNum);

    return bases.toBase62(num);
}


function GeoProvider(config) {
    this.Config = {};
    this._serviceurl = '';
    this._servicereturn = '';
    if (typeof (config) != 'undefined') {
        this.Config = config;
    }


    this.ServiceUrl = function() {
        return this._serviceurl;
    }
    
    this.ServiceReturn = function() {
        return this._servicereturn;
    }

    this.BuildSearchQuery = function (search) {
        var self = this;
        url = self.Config.Url;
        param = '';
        uri = '';
        self.Config.QueryString[self.Config.SearchQuery] = search;



        url = self.Config.UrlPattern.indexOf("?")<0 ? self.Config.UrlPattern + "?" : self.Config.UrlPattern + "&";
        // we don't url encode if using oauth
        url = url + self.Config.ToQueryString(!self.Config.OAuthRequired);


        if (self.Config.Key != '' && self.Config.KeyQuery != '') {
            // api keys shouldn't be encoded
            url = url + '&' + self.Config.KeyQuery + '=' + self.Config.Key;
        }

        if (self.Config.KeyRoute != '' && self.Config.Key != '') {
            url = url.replace("{" + self.Config.KeyRoute + "}", self.Config.Key);
        }


        // kudos to https://github.com/addyinc/node-bossgeo
        // for the code for signing below as well as some helper items.
        // much appreciated

        if (self.Config.OAuthRequired)
        {
            //var oAuth= new OAuth(null, null,self.Config.ConsumerKey, self.Config.Secret, "1.0", null, "HMAC-SHA1");
            var args = {
                'oauth_version': '1.0',
                'oauth_consumer_key': self.Config.ConsumerKey,
                'oauth_nonce': randomStr(32),
                'oauth_signature_method': 'HMAC-SHA1',
                'oauth_timestamp': Math.floor(Date.now() / 1000).toString()
            };
            var urlparse = _url.parse(url, true);
            args = _.extend(args, urlparse.query);
            baseUrl = urlparse.protocol + '//' + urlparse.hostname + urlparse.pathname;
            
            var qs = argsToEncodedData(args);
            var sig = 'GET&' + encodeURIComponent(baseUrl) + '&' + encodeURIComponent(qs);
            var key = encodeURIComponent(self.Config.Secret) + '&';
            var hash = crypto.createHmac('sha1', key).update(sig).digest('base64');

            url = baseUrl + '?' + qs +  '&oauth_signature=' + hash
            //var nonce = oAuth._getNonce();
            //var timeStamp = oAuth._getTimestamp();
            
            
        } 


        self._serviceurl = url;
      
        return url;
    }

    this.BuildHttpOptions = function (search) {
        var self = this;
        var urlparse = _url.parse(this.BuildSearchQuery(search));

        var options = {
            host: urlparse.host,
            port: 80,
            path: urlparse.path,
            method: 'GET'
        };
        
        if (self.Config.UserAgent != '') {
            options.Headers.UserAgent = self.config.UserAgent;
        }
        return options;
    }

    this.RequestCallBack = function () {

    }

    this.ExecuteSearch = function (search, callback) {
        var self = this;


        this.GetRequest = function (response) {
            var str = '';


            //another chunk of data has been recieved, so append it to `str`
            response.on('data', function (chunk) {

                str += chunk;
                //console.log(me.str);
            });

            //the whole response has been recieved, so we just print it out here
            response.on('end', function () {
                if (str != '') {
                    self._servicereturn = str;
                    self.parser(str, callback);
                } else {
                    callback();
                }
            });

        }

        var options = self.BuildHttpOptions(search);
        //console.log(options);


        http.request(options, self.GetRequest).on('error', function(){}).end();


    }

    this.GetCoordinates = function (search, callback) {

        this.ExecuteSearch(search, callback);


    }



}


function GeoCoder(provider) {




    this.ServiceUrl = '';
    this.ServiceReturn = '';

    this.providers = [];

    if (typeof (provider) !== 'undefined') {
        this.AddProvider(provider);
    }


}

GeoCoder.prototype.AddProvider = function (provider) {
    if (typeof (provider) !== 'undefined') {
        this.providers.push(provider);
    }
    return this;

}

GeoCoder.results = {};

GeoCoder.prototype.TestGoogle = function (callback) {
    var config = new GoogleGmapProviderConfig();
    var geo = new GoogleGmap(config);
    geo.GetCoordinates('Austin, TX', function (data) {
        //console.log(data);
        //callback(data);
    });

}

GeoCoder.prototype.TestOpenStreets = function (callback) {
    var geo = new OpenStreets();
    geo.GetCoordinates('Austin, TX', function (data) {
        //console.log(data);
        callback(data);
    });

}

GeoCoder.prototype.SetupDefaults = function () {
    this.AddProvider(new GoogleGmap());
}

GeoCoder.prototype.GetCoordinates = function (search, callback) {
    var self = this;
    if (this.providers.length == 0) {
        self.SetupDefaults();
    }
    var hasResult = false;
    providerCount = this.providers.length;
    count = 0;
    async.whilst(
        function () { return !hasResult && count < providerCount; },
        function (callback2) {
            
            
            self.providers[count].GetCoordinates(search, function (data) {
                                    self.ServiceUrl = self.providers[count].ServiceUrl();
                self.ServiceReturn = self.providers[count].ServiceReturn();
                if (data.HasValue()) {
                    hasresult = true;
                    
                    console.log(data);
                    count++;
                    callback({ data: data, err: null, providerindex: count-1 });
                } else {
                    count++;
                    callback2();
                }
            });

           
            
        },
        function (err) {
            // 5 seconds have passed
            callback({data:null, err: 'error'})
        }
);


}

GeoCoder.prototype.GoogleGmap = GoogleGmap;
GeoCoder.prototype.GoogleGmapProviderConfig = GoogleGmapProviderConfig;

GeoCoder.prototype.OpenStreetsProviderConfig = OpenStreetsProviderConfig;
GeoCoder.prototype.OpenStreets = OpenStreets;

GeoCoder.prototype.BingMapProviderConfig = BingMapProviderConfig;
GeoCoder.prototype.BingMap = BingMap;

GeoCoder.prototype.MapQuestProviderConfig = MapQuestProviderConfig;
GeoCoder.prototype.MapQuest = MapQuest;

GeoCoder.prototype.YahooPlaceFinderBossProviderConfig = YahooPlaceFinderBossProviderConfig;
GeoCoder.prototype.YahooPlaceFinderBoss = YahooPlaceFinderBoss;

GeoCoder.prototype.YahooPlaceFinderProviderConfig = YahooPlaceFinderBossProviderConfig;
GeoCoder.prototype.YahooPlaceFinder = YahooPlaceFinder;

GeoCoder.prototype.CloudMadeProviderConfig = CloudMadeProviderConfig;
GeoCoder.prototype.CloudMade = CloudMade;

module.exports = GeoCoder;

