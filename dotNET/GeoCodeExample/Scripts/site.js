$(document).ready(function () {
    var $ApiKey = $('#ApiKey');
    var $Address = $('#Address');
    var $Providers = $('#Providers');
    var $btn = $('#doit');
    var $results = $('#results');
    var $lat = $('#latitude', $results);
    var $lng = $('#longitude', $results);
    var $message = $('#message');
    var $apikeytest = $('#apikeytext');
    iset = false;
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
    $Providers.on('change', function (e) {
        if ($(this).val().indexOf('KEY') > 0) {
            $ApiKey.show();
            $apikeytest.hide();
        } else {
            $ApiKey.val('');
            $ApiKey.hide();
            $apikeytest.show();
        }

    });
    $btn.on('click', function(e) {
        e.preventDefault();
        $results.hide();
        $message.hide();
        $.ajax({
            url: '/GeoCode/Query',
            type: 'POST',
            data: { Query: $Address.val(), Providers: $Providers.val(), Key: $ApiKey.val() },
            success: function (data) {
                
                if (data.HasValue) {
                    $lat.text(data.Latitude);
                    $lng.text(data.Longitude);
                    $results.show();
                    $message.hide();

                    clearOverlays();

                    for (i = 0; i < data.Locations.length; i++) {
                        var latlng = new google.maps.LatLng(data.Locations[i].Latitude, data.Locations[i].Longitude);
                        var img = image;
                        if (i == 0) {
                            img = imagex;
                        }
                        var marker = new google.maps.Marker({
                            position: latlng,
                            icon:img
                        });

                        var contentString = '<h1>' + data.Locations[i].Name + '</h1>';
                        contentString = contentString + '<div>' + data.Locations[i].Latitude + ',' + data.Locations[i].Longitude + '</div>';
                        contentString = contentString + '<div>Accuracy: ' + data.Locations[i].Accuracy + '</div>';

                        var infowindow = new google.maps.InfoWindow({
                            content: contentString
                        });


                        var markeroverlay = {marker: marker, overlay: infowindow}

                      

                        markersArray.push(markeroverlay);

                }

                    addOverlays();



                
                    setTimeout(google.maps.event.trigger(map, 'resize'), 1000);


                } else {
                    $results.hide();
                    $message.text('No query results');
                    $message.show();
                    
                }

              
            }
        });
    });

  


    function initialize() {
     

       


        google.maps.event.addListener(map, 'center_changed', function () {
            // 3 seconds after the center of the map has changed, pan back to the
            // marker.
            
        });

       
    }

    google.maps.event.addDomListener(window, 'load', initialize);


});