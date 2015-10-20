var http = require('http');
var url = require('url');
var _ = require('underscore');
var GeoCoder = require('./geocoder.js');
try {
var configvals = require('./config.js');
} catch (err) {
    var configvals = {};
}


if (typeof(configvals.BingKey)=='undefined')
configvals = _.extend(configvals, {
    BingKey: '',
    MapQuestKey: "",
    YahooAppId: "",
    CloudMadeAppId: "",
    BossConsumerKey: "",
    BossSecret: ""
});

console.log(configvals);



function printData(d,msg) {
    //console.log(d.data.name() + ',' + d.data.latitude() + ',' + d.data.longitude());
    console.log('********' + msg + '********')
    console.log(d.data);
    console.log('********' + msg + ' END ********')
}

var g = new GeoCoder();
g.GetCoordinates('Austin, TX', function (data) {
    printData(data, 'Default');
    // return the url that is generated + the json/xml that is returned via service for custom processing
    console.log(g.ServiceUrl + ' - ' + g.ServiceReturn);
    
});


/*var configy = new YahooPlaceFinderBossProviderConfig()
    .SetKey(configvals.YahooAppId)
    .SetConsumerKey(configvals.BossConsumerKey)
    .SetSecret(configvals.BossSecret)

var g9 = new GeoCoder(new YahooPlaceFinderBoss(configy));

g9.GetCoordinates('Austin, TX', function (data) {
    
    if (data.err == null) {
        printData(data, "Yahoo Place Finder BOSS");
        console.log(g9.ServiceUrl())
    }
    else
        console.log('error arg');
    
});

var configy2 = new YahooPlaceFinderProviderConfig()
    .SetKey(configvals.YahooAppId)
    
var g10 = new GeoCoder(new YahooPlaceFinder(configy2));

g10.GetCoordinates('Austin, TX', function (data) {
    
    if (data.err == null)
        printData(data, 'Yahoo Place Finder');
    else
        console.log('error arg');
    
});

var configm = new CloudMadeProviderConfig()
    .SetKey(configvals.CloudMadeAppId);

var g11 = new GeoCoder(new CloudMade(configm));

g11.GetCoordinates('Austin, TX', function (data) {

    if (data.err == null)
        printData(data, 'Cloud Made Finder');
    else
        console.log('error arg');

});



var g = new GeoCoder();
g.GetCoordinates('Austin, TX', function (data) {
    
    printData(data, 'Default');
    
});

var g2 = new GeoCoder(new OpenStreets());
g2.GetCoordinates('Austin, TX', function (data) {
    
    printData(data, 'Open Streets');
    
});

var bingconfig = new BingMapProviderConfig().SetKey(configvals.BingKey);

var g6 = new GeoCoder(new BingMap(bingconfig));

g6.GetCoordinates('Austin, TX', function (data) {
    
    if (data.err == null)
        printData(data, 'Bing');
    else
        console.log('error arg');
    
});


var mapqconfig = new MapQuestProviderConfig().SetKey(configvals.MapQuestKey);

var g7 = new GeoCoder(new MapQuest(mapqconfig));

g7.GetCoordinates('Austin, TX', function (data) {
    
    if (data.err == null)
        printData(data, 'MapQuest');
    else
        console.log('error arg');
    
});


var g5 = new GeoCoder(new GoogleGmap());
g5.GetCoordinates('Austin, TX', function (data) {
    
    printData(data,'Google');
    
});

var gconfig = new GoogleGmapProviderConfig().SetUrl('http://mapds.googleapis.com/maps/api/geocode/json?sensor=true');
var oconfig = new OpenStreetsProviderConfig().SetUrl('http://www.yahoo.com');
var g3 = new GeoCoder().AddProvider(new GoogleGmap(gconfig)).AddProvider(new OpenStreets(oconfig));

g3.GetCoordinates('Austin, TX', function (data) {
    
    if (data.err!=null) {
        printData(data.err,'No result');
    }
    
});

var g4 = new GeoCoder().AddProvider(new GoogleGmap()).AddProvider(new OpenStreets());

g4.GetCoordinates('Austin, TX', function (data) {
    
    if (data.err==null) {
            printData(data, 'Failover Test to Open Streets')
    }
    
}); */
