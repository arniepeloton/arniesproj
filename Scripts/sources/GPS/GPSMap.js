
require([
    'common'
    ], function () {
        var centerpoint;
        var marker;
        var map;
        var circle;

        window.plotLocationToMap = function (lat, long) {
            $('#txtLat').val(lat);
            $('#txtLong').val(long);
            //  DisplayMapWithCoordinates(lat, long);
            PlotTrack();
        }


        $(document).ready(function () {

            var paramLat = getParameterByName("Lat");
            var paramLong = getParameterByName("Long");
            var paramDeviceID = getParameterByName("DeviceID");

            if (paramDeviceID.length > 0) {
                GetGPSCurrentLocation(paramDeviceID);
                ShowDeviceDetails(paramDeviceID);
            }


            $('#btnGoChangeDateRange').unbind().click(function () {
                //ClearTrack();
                if (paramDeviceID.length > 0) {
                    PlotTrack(paramDeviceID);
                } else {
                    MessageBox("No device ID.",
                           "Device Location",
                            [
                                { Text: 'Ok'
                                }
                            ]);
                }
            });

            $('#btnClearLocation').unbind().click(function () {
                ClearTrack();
            });




        });



        //        function DisplayMapWithCoordinates(lat, long) {

        //            centerpoint = new google.maps.LatLng(lat, long);
        //            //initialization
        //            var mapProp = {
        //                center: centerpoint,
        //                zoom: 25,
        //                mapTypeId: google.maps.MapTypeId.ROADMAP
        //            };
        //            map = new google.maps.Map(document.getElementById("googleMap"), mapProp);
        //            DisplayMarker();
        //        }

        //        function DisplayMap() {

        //            //initialization
        //            var mapProp = {
        //                center: centerpoint,
        //                zoom: 20,
        //                mapTypeId: google.maps.MapTypeId.ROADMAP
        //            };
        //            map = new google.maps.Map(document.getElementById("googleMap"), mapProp);

        //        }



        function getParameterByName(name) {
            name = name.replace(/[\[]/, "\\\[").replace(/[\]]/, "\\\]");
            var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
        results = regex.exec(location.search);
            return results == null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
        }






        //prereq: DisplayMapWithCoordinates
        function DisplayMarker(centerpoint) {
            marker = new google.maps.Marker({
                position: centerpoint,
                animation: google.maps.Animation.BOUNCE
            });
            marker.setMap(map);
        }

        //prereq: DisplayMapWithCoordinates, DisplayMarker, 
        function DisplayGeoFenceCircle() {
            circle = new google.maps.Circle({
                fillColor: '#00f',
                fillOpacity: 0.1,
                strokeColor: '#00f',
                strokeOpacity: 0.3,
                strokeWeight: 1
            });
            circle.bindTo('center', marker, 'position');
            circle.setRadius(150);
            circle.setMap(map);
        }









        //prereq: DisplayMapWithCoordinates
        function GetGPSCurrentLocation(devID) {
            var pData = { DeviceID: devID };
            AjaxSend({
                url: 'GPS/getGPSCurrentLocation',
                data: pData,
                success: function (vData) {
                    DisplayCurrent(vData);
                },
                error: function () {
                    $.mobile.changePage('#pageNoMap');
                    return;
                }

            });
        }

        function DisplayCurrent(vData) {
            var vRows = vData.Track.Rows;
            if (vRows.length <= 0) {

                $('#googleMap').append($('#divNoMapInfo')); //'<h3>').attr("style", "white-space:normal").text('good'));
                $('#googleMap').trigger("create");
                return;

            }


            centerpoint = new google.maps.LatLng(vRows[0].Latitude, vRows[0].Longitude);
            //initialization
            var mapProp = {
                center: centerpoint,
                zoom: 25,
                mapTypeId: google.maps.MapTypeId.ROADMAP
            };
            map = new google.maps.Map(document.getElementById("googleMap"), mapProp);
            DisplayMarker(centerpoint);

        }



        function PlotTrack(paramDeviceID) {

            var pData = { DateFrom: $.format.date($('#txtDateFrom').DateCombo('val'), 'M/d/yyyy h:mm:ss a'), DateTo: $.format.date($('#txtDateTo').DateCombo('val'), 'M/d/yyyy h:mm:ss a'), DeviceID: paramDeviceID };
            AjaxSend({
                url: 'GPS/getGPSTrackList',
                data: pData,
                success: function (vData) {
                    DisplayTrack(vData);
                }
            });
        }



        function DisplayTrack(vData) {

            var vRows = vData.Track.Rows;

            //initialization
            var mapProp = {
                center: new google.maps.LatLng(vRows[0].Latitude, vRows[0].Longitude),
                zoom: 20,
                mapTypeId: google.maps.MapTypeId.ROADMAP
            };
            map = new google.maps.Map(document.getElementById("googleMap"), mapProp);




            var flightPlanCoordinates = new Array();
            if (vRows.length > 0) {
                for (var ctr = 0; ctr <= vRows.length - 1; ctr++) {
                    flightPlanCoordinates.push(new google.maps.LatLng(vRows[ctr].Latitude, vRows[ctr].Longitude));
                }
            }


            var flightPath = new google.maps.Polyline({
                path: flightPlanCoordinates,
                strokeColor: "#FF0000",
                strokeOpacity: 1.0,
                strokeWeight: 2
            });

            flightPath.setMap(map);

        }





        function ClearTrack() {
            var paramDeviceID = getParameterByName("DeviceID");

            if (paramDeviceID.length > 0) {
                GetGPSCurrentLocation(paramDeviceID);
            }


            //initialization
            var mapProp = {

                zoom: 20,
                mapTypeId: google.maps.MapTypeId.ROADMAP
            };
            map = new google.maps.Map(document.getElementById("googleMap"), mapProp);

            var flightPath = new google.maps.Polyline({
                path: [],
                strokeColor: "#FF0000",
                strokeOpacity: 1.0,
                strokeWeight: 2
            });

            flightPath.setMap(map);

        }


        function ShowDeviceDetails(DeviceID) {
            var data = { ID: DeviceID }

            AjaxSend({
                url: 'DeviceDashboard/getDeviceListByID',
                data: data,
                success: function (vData) {
                    var vRow = vData.Results.Rows;
                    if (vRow) {
                        $('#lblDeviceName').text(vRow[0].PhoneName);
                        $('#lblUUID').text(vRow[0].UUID);
                        $('#lblIMEI').text(vRow[0].IMEI);
                        $('#lblCurrentPolicy').text(vRow[0].OS);
                    }
                }
            });
        }




    });




    
       