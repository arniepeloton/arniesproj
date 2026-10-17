@Code
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head

    <script>
        require(['/Scripts/sources/Device/DevicePlaces.js?crc=']);
    </script>
    
End Section

<div id="pageDevicePlaces" data-role="page">

    <div id"divHeader" data-role="header">
        <a id="btnGoTo" href="#popupMenu" data-rel="popup" data-inline="true" class="ui-btn-right">Go To</a>
        <h1>Places</h1>
    </div>

    <div data-role="content" >

        <div data-role="fieldcontain">
	        <input type="text" readonly style="display:none;" id="txtDeviceID"/>                
        </div>

        <div>
            <ul data-role="listview" id="lstPlacesContainer" data-inset="true">   
				<li data-role="list-divider">Places</li>
                <li>
                    <div data-role="fieldcontain"><a href="#" id="btnAddPlaces" data-role="button" data-icon="plus" data-inline="true">Add Places</a></div>
                    <div data-role="fieldcontain" id="divRecordCountPlaces"></div>
                    <div>
                        <ul data-role="listview" id="lstPlaces" data-inset="true" data-split-icon="delete" data-split-theme="d">   
                        </ul>
                    </div>
                </li>
            </ul>
        </div>

        <div>
            <br />
            <br />
	        <div style="text-align:center"> <a id="btnDevicePlacesBack" data-role="button">Back</a> </div>
	        <div style="text-align:center"> <a id="btnDevicePlacesDone" data-role="button">Done</a> </div>
        </div>

        <div id="popupMenu" data-role="popup">
            <ul data-role="listview" data-inset="true" style="min-width: 210px;">
                <li data-role="divider">Menu</li>
                <li><a href="DeviceInfo">Device Information</a></li>
            </ul>
        </div>

    </div>

</div>
