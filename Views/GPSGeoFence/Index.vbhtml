@Code
    ViewData("Title") = "GPS GEoFence"
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code
@Section head
    <script type="text/javascript" src="http://maps.google.com/maps/api/js?libraries=geometry&sensor=false&language=en"></script>
    <script src="http://maps.googleapis.com/maps/api/js?v=3.exp&key=AIzaSyDY0kkJiTPVd2U7aTOAwhc9ySH6oHxOIYM&sensor=false">
    </script>
    <script>
        require([GetPathFormat('/Scripts/sources/GPS/GPSGeoFence.js?crc=')]);
    </script>
    <style>
    #map {
        height: 100%; 
        font-family: Arial, sans-serif; 
        font-size: .9em; 
        color: #fff;
    }
    #note { 
        text-align: center;
        padding: .3em; 10px; 
        background: #009ee0; 
    }
    .bool {
        font-style: italic;
        color: #313131;
    }
    .info {
        display: inline-block;
        width: 40%;
        text-align: center;
    }
    .infowin {
        color: #313131;
    }
    #title,
    .bool{
        font-weight: bold;
    }
    </style>
End Section
<div id="pageMain" data-role="page">
    <div data-role="header">
        <h1>@ViewData("Title")</h1>
    </div>
    <div id="note">
        <span id="title">&raquo;Inside the circle?&laquo;</span><hr />
        <span class="info">Marker <strong>A</strong>: <span id="a" class="bool"></span></span>
        &larr;&diams;&rarr; <span class="info">Marker <strong>B</strong>: <span id="b" class="bool">
        </span></span>
    </div>
    <div id="map" style="width: 100%; height: 600px;">
        test</div>
</div>
