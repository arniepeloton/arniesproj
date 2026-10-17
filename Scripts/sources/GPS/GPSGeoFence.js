


require([
    'common'
    ], function () {



        $(document).ready(function () {



            var 
                    contentCenter = '<span class="infowin">Center Marker (draggable)</span>',
                    contentA = '<span class="infowin">Marker A (draggable)</span>',
                    contentB = '<span class="infowin">Marker B (draggable)</span>';
            var 
                    latLngCenter = new google.maps.LatLng(37.081476, -94.510574),
                    latLngCMarker = new google.maps.LatLng(37.0814, -94.5105),
                    latLngA = new google.maps.LatLng(37.2, -94.1),
                    latLngB = new google.maps.LatLng(38, -93),
                    map = new google.maps.Map(document.getElementById('map'), {
                        zoom: 7,
                        center: latLngCenter,
                        mapTypeId: google.maps.MapTypeId.ROADMAP,
                        mapTypeControl: false
                    }),
                    markerCenter = new google.maps.Marker({
                        position: latLngCMarker,
                        title: 'Location',
                        map: map,
                        draggable: true
                    }),
                    infoCenter = new google.maps.InfoWindow({
                        content: contentCenter
                    }),
                    markerA = new google.maps.Marker({
                        position: latLngA,
                        title: 'Location',
                        map: map,
                        draggable: true
                    }),
                    infoA = new google.maps.InfoWindow({
                        content: contentA
                    }),
                    markerB = new google.maps.Marker({
                        position: latLngB,
                        title: 'Location',
                        map: map,
                        draggable: true
                    }),
                    infoB = new google.maps.InfoWindow({
                        content: contentB
                    })
            // exemplary setup: 
            // Assumes that your map is signed to the var "map"
            // Also assumes that your marker is named "marker"
                    ,
                    circle = new google.maps.Circle({
                        map: map,
                        clickable: false,
                        // metres
                        radius: 100000,
                        fillColor: '#fff',
                        fillOpacity: .6,
                        strokeColor: '#313131',
                        strokeOpacity: .4,
                        strokeWeight: .8
                    });
            // attach circle to marker
            circle.bindTo('center', markerCenter, 'position');

            var 
            // get the Bounds of the circle
                bounds = circle.getBounds()
            // Note spans
                ,
                    noteA = jQuery('.bool#a'),
                    noteB = jQuery('.bool#b');
          
            noteA.text(bounds.contains(latLngA));
            noteB.text(bounds.contains(latLngB));
            //  (google.maps.geometry.spherical.computeDistanceBetween(latLng, center) <= radius))
            //   get some latLng object and Question if it's contained in the circle:
            google.maps.event.addListener(markerCenter, 'dragend', function () {
                latLngCenter = new google.maps.LatLng(markerCenter.position.lat(), markerCenter.position.lng());
                bounds = circle.getBounds();
                noteA.text((bounds.contains(latLngA)) && (google.maps.geometry.spherical.computeDistanceBetween(latLngA, circle.getCenter()) <= circle.radius));
                noteB.text((bounds.contains(latLngB)) && (google.maps.geometry.spherical.computeDistanceBetween(latLngB, circle.getCenter()) <= circle.radius));
            });

            google.maps.event.addListener(markerA, 'dragend', function () {
                latLngA = new google.maps.LatLng(markerA.position.lat(), markerA.position.lng());
                noteA.text((bounds.contains(latLngA)) && (google.maps.geometry.spherical.computeDistanceBetween(latLngA, circle.getCenter()) <= circle.radius));
            });

            google.maps.event.addListener(markerB, 'dragend', function () {
                latLngB = new google.maps.LatLng(markerB.position.lat(), markerB.position.lng());
                noteB.text((bounds.contains(latLngB))&&(google.maps.geometry.spherical.computeDistanceBetween(latLngB, circle.getCenter()) <= circle.radius));
            });

            google.maps.event.addListener(markerCenter, 'click', function () {
                infoCenter.open(map, markerCenter);
            });

            google.maps.event.addListener(markerA, 'click', function () {
                infoA.open(map, markerA);
            });

            google.maps.event.addListener(markerB, 'click', function () {
                infoB.open(map, markerB);
            });

            google.maps.event.addListener(markerCenter, 'drag', function () {
                infoCenter.close();
                noteA.html("draggin&hellip;");
                noteB.html("draggin&hellip;");
            });

            google.maps.event.addListener(markerA, 'drag', function () {
                infoA.close();
                noteA.html("draggin&hellip;");
            });

            google.maps.event.addListener(markerB, 'drag', function () {
                infoB.close();
                noteB.html("draggin&hellip;");
            });
        });




    });




    
       