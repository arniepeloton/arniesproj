@Code
    ViewData("Title") = "GPS Dashboard"
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code
@Section head
    <script src="http://maps.googleapis.com/maps/api/js?key=AIzaSyDY0kkJiTPVd2U7aTOAwhc9ySH6oHxOIYM&sensor=false">
    </script>
    <script>
        require([GetPathFormat('/Scripts/sources/GPS/GPSDashboard.js?crc=')]);
    </script>
 
End Section
<div id="note">
    <span id="title">&raquo;Inside the circle?&laquo;</span><hr />
    <span class="info">Marker <strong>A</strong>: <span id="a" class="bool"></span></span>
    &larr;&diams;&rarr; <span class="info">Marker <strong>B</strong>: <span id="b" class="bool">
    </span></span>
</div>
<div id="map" style="width: 100%; height: 600px;">
        test</div>