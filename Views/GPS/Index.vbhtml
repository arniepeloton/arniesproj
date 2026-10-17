@Code
    ViewData("Title") = "Find Device"
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code
@Section head
    <script src="http://maps.googleapis.com/maps/api/js?v=3.exp&key=AIzaSyDY0kkJiTPVd2U7aTOAwhc9ySH6oHxOIYM&sensor=false">
    </script>
    <script>
        require([GetPathFormat('/Scripts/sources/GPS/GPSMap.js?crc=')]);
    </script>
    <script>
      

    </script>
End Section
<div id="pageMain" data-role="page">
    <div data-role="header">
        <h1>@ViewData("Title")</h1>
    </div>
    <div data-role="content" data-dom-cache="true">
        <div data-role="collapsible" data-collapsed="true" data-theme="c" data-content-theme="d"
            data-collapsed-icon="arrow-r" data-expanded-icon="arrow-d" data-inset="true">
            <h2>
                <label id="lbltitleView">
                    Device Information
                </label>
            </h2>
            <div data-role="content">
                <div data-role="fieldcontain">
                    <fieldset data-role="controlgroup">
                        <legend>Device Name </legend>
                        <label id="lblDeviceName">
                            Device Name
                        </label>
                    </fieldset>
                </div>
                <div data-role="fieldcontain">
                    <fieldset data-role="controlgroup">
                        <legend>UUID </legend>
                        <label id="lblUUID">
                            UUID
                        </label>
                    </fieldset>
                </div>
                <div data-role="fieldcontain">
                    <fieldset data-role="controlgroup">
                        <legend>IMEI </legend>
                        <label id="lblIMEI">
                            IMEI
                        </label>
                    </fieldset>
                </div>
                <div data-role="fieldcontain">
                    <fieldset data-role="controlgroup">
                        <legend>OS </legend>
                        <label id="lblCurrentPolicy" data-role="field-contain">
                            OS
                        </label>
                    </fieldset>
                </div>
            </div>
        </div>
        <div data-role="collapsible" data-collapsed="true" data-theme="c" data-content-theme="d"
            data-collapsed-icon="arrow-r" data-expanded-icon="arrow-d" data-inset="true">
            <h2>
                <label id="lbltitlePolicy">
                    View Location History
                </label>
            </h2>
            <table data-role="table" id="tblInventoryReport" data-mode="reflow" class="ui-responsive">
                <thead>
                    <tr>
                        <th>
                            From
                        </th>
                        <th>
                            To
                        </th>
                        <th>
                            Action
                        </th>
                        <th>
                        </th>
                        <th>
                        </th>
                    </tr>
                </thead>
                <tbody>
                    <tr>
                        <td>
                            <fieldset id="txtDateFrom" data-role="DateCombo" data-usecurrent="1" data-timecombo="txtTimefrom">
                            </fieldset>
                        </td>
                        <td>
                            <fieldset id="txtDateTo" data-role="DateCombo" data-usecurrent="1" data-timecombo="txtTimeto">
                            </fieldset>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <fieldset id="txtTimefrom" data-role="TimeCombo">
                            </fieldset>
                        </td>
                        <td>
                            <fieldset id="txtTimeto" data-role="TimeCombo">
                            </fieldset>
                        </td>
                        <td>
                            <a id="btnGoChangeDateRange" href="#" data-role="button">Go</a>
                        </td>
                        <td>
                            <a id="btnClearLocation" href="#" data-role="button">Clear Location History</a>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
        @*  
         </div>
                            Date From
                        </td>
                        <td>
                            <div data-role="fieldcontain" data-role="controlgroup">
                              
                            </div>
                        </td>
                        <td>
                            <fieldset id="txtTimefrom" data-role="TimeCombo" data-seconds="true">
                            </fieldset>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Date To
                        </td>
                        <td>
                            <div data-role="fieldcontain" data-role="controlgroup">
                                <fieldset id="txtDateTo" data-role="DateCombo" data-usecurrent="1" data-timecombo="txtTimeto">
                                </fieldset>
                            </div>
                        </td>
                        <td>
                            <fieldset id="txtTimeto" data-role="TimeCombo" data-seconds="true">
                            </fieldset>
                        </td>
                    </tr>
                    <tr>
                        <td>
                        </td>
                        <td>
                            <a id="btnGoChangeDateRange" href="#" data-role="button">Go</a>
                        </td>
                        <td>
                            <a id="btnGoChangeDateRange" href="#" data-role="button">Clear Location History</a>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>*@
        <div id="googleMap" style="width: 100%; height: 500px;">
        </div>
    </div>
    @* 
    <div data-role="fieldcontain">
        <fieldset data-role="controlgroup">
            <legend>Latitude </legend>
            <input type="text" id="txtLat" data-role="field-contain" required>
        </fieldset>
    </div>

     <div data-role="fieldcontain">
        <fieldset data-role="controlgroup">
            <legend>Longitude </legend>
            <input type="text" id="txtLong" data-role="field-contain" required>
        </fieldset>
    </div>*@ @* <input id="btnViewCurr" type="button" onclick="window.GpsHandler.getCurrentLocation()"
        style="white-space: normal; text-align: left;" value=" View Current Location" />
    <br />*@ <a data-role="button" id="btnGPSback" data-rel="back" data-theme="c">Back</a>
</div>
<div id="pageNoMap" data-role="page">
    <div id="divNoMapInfo" data-role="content">
        <div style="white-space: normal; text-align: center;" data-role="fieldcontain">
            <h3>
                No information about the location of the device you have selected.
                <br />
                Please verify the following:</h3>
            <p>
                1. MDM application is installed in the device.
                <br />
                2. Internet connection is available to the device.
            </p>
        </div>
    </div>
</div>
