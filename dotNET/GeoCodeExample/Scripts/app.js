var app = angular.module('geoApp', ['geoApp.controllers']);

var cApp = angular.module('geoApp.controllers', [])
cApp.controller('MapCtrl', ['$scope', '$http', function ($scope, $http) {
    $scope.providers = providers;
    $scope.hasResults = false;
    $scope.hasMessage = false;
    $scope.address = 'Austin, Tx';

    $scope.getKey = function () {
        return $scope.provider.Value.indexOf('KEY') < 0;
    }

    $scope.go = function () {
        $http.post('/api/Geo/Query', { Query: $scope.address, Providers: $scope.provider.Value, Key: $scope.apiKey}).then(function(data) {
            $scope.data = data.data;
        });
    }
}]);

cApp.directive('map', ['$timeout', function ($timeout) {
    var image = 'http://maps.google.com/mapfiles/ms/icons/blue-dot.png';
    var imagex = 'http://maps.google.com/mapfiles/ms/icons/red-dot.png';
    var mapOptions = {
        zoom: 4,
        center: new google.maps.LatLng(-25.363882, 131.044922),
        mapTypeId: google.maps.MapTypeId.ROADMAP
    };
    var markersArray = [];

    var map = new google.maps.Map(document.getElementById('map'),
         mapOptions);
    function clearOverlays() {
        for (var i = 0; i < markersArray.length; i++) {
            var marker = markersArray[i].marker;
            marker.setMap(null);
        }
        markersArray.length = 0;
    }
    function addOverlays() {
        var bounds = new google.maps.LatLngBounds();
        for (var i = 0; i < markersArray.length; i++) {
            var marker = markersArray[i].marker;
            var infowindow = markersArray[i].overlay;

            var l = new google.maps.LatLng(marker.position.lat(), marker.position.lng());
            if (i == 0) {

                map.setCenter(l);
            }
            bounds.extend(l);

            marker.setMap(map);


            google.maps.event.addListener(marker, 'click', function () {
                return function () {
                    infowindow.overlay.open(map, marker);
                }
            });

        }
        map.fitBounds(bounds);

    }

    function link(scope, element, attrs) {
        scope.$watch('data', function (val) {
            console.log(val);
            if (!val) return;
            var data = scope.data;
            if (data.HasValue) {
                scope.latitude = data.Latitude;
                scope.longitude = data.Longitude;

                clearOverlays();

                for (i = 0; i < data.Locations.length; i++) {
                    var latlng = new google.maps.LatLng(data.Locations[i].Latitude, data.Locations[i].Longitude);
                    var img = image;
                    if (i == 0) {
                        img = imagex;
                    }
                    var marker = new google.maps.Marker({
                        position: latlng,
                        icon: img
                    });

                    var contentString = '<h1>' + data.Locations[i].Name + '</h1>';
                    contentString = contentString + '<div>' + data.Locations[i].Latitude + ',' + data.Locations[i].Longitude + '</div>';
                    contentString = contentString + '<div>Accuracy: ' + data.Locations[i].Accuracy + '</div>';

                    var infowindow = new google.maps.InfoWindow({
                        content: contentString
                    });


                    var markeroverlay = { marker: marker, overlay: infowindow }



                    markersArray.push(markeroverlay);

                }

                addOverlays();



                $timeout(function () {
                    scope.hasResults = true;
                    scope.hasMessage = false;
                    google.maps.event.trigger(map, 'resize');
                }, 500);
                


            } else {
                clearOverlays();
                $timeout(function () {
                    scope.hasMessage = true;
                    scope.hasResults = false;
                    scope.message = 'No query results';
                });
                
                

            }
        });
        
    }

    return {
        restrict: 'C',       
        link: link
    };
}]);