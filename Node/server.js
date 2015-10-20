var http = require('http')
var express = require('express');
var GeoCoder = require('./geocoder.js');
var port = process.env.PORT || 1337;
var bodyParser = require('body-parser');
var app =  express();

var g = new GeoCoder();

app.use(express.static(__dirname + '/public'))
 app.use(bodyParser());

app.post('/GeoCode/Query', function(req, res){
    //Query, Providers, Key
  var config, provider;
  var k = req.body.Key;
  var q = req.body.Query;
  var p = req.body.Providers;
  var g;
  switch (p) {
      case 'Google':
          config = new GoogleGmapProviderConfig();
          provider = new GoogleGmap();
          break;
      case 'MapQuestKEY':
          config = new MapQuestProviderConfig().SetKey(k);
          provider = new MapQuest(config);
          break;
      case 'Open Streets':
          config = new OpenStreetsProviderConfig();
          provider = new OpenStreets(config);
          break;
      case 'CloudMakeKEY':
          config = new CloudMadeProviderConfig().SetKey(k);
          provider = new CloudMade(config);
          break;
      case 'YahooPlacesKEY':
          config = new YahooPlaceFinderProviderConfig().SetKey(k);
          provider = new YahooPlaceFinder(config);
          break;
      case "BingKEY":
          config = new BingMapProviderConfig().SetKey(k);
          provider = new BingMap(config);
          break;
  }
  g = new GeoCoder(provider);

  g.GetCoordinates(q, function(data){
  		res.send(data.data);
  })
  
});

app.listen(port);

