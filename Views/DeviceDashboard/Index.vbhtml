@Code
    ViewData("Title") = "Device Dashboard"
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code
@Section head
    <style>
        #pageMain
        {
            font-size: small;
        }
          .form-validation-error
        {
            border: 1px solid Red;
            box-shadow: 0 0 3px Red;
        }
        
        
        .form-validation-error:active
        {
            box-shadow: 0px 1px 3px rgba(0, 0, 0, 0.2) inset, 0px 0px 3px rgba(0, 0, 0, 0.2);
        }
        .smallfontRed
        {
            background-color: Red;
            color: White;
            margin-left: 8px;
            font-size: small;
        }
    </style>
    <script>
        require([GetPathFormat('/Scripts/sources/DeviceMDMConfig/DeviceDashboard.js?crc=')]);
        //require("http://maps.googleapis.com/maps/api/js?sensor=false");


    </script>
    <script type="text/javascript" src="http://maps.googleapis.com/maps/api/js?sensor=false">
    </script>
End Section
<div id="pageMain" data-role="page">
    <div data-role="header">
        <h1>@ViewData("Title")</h1>
    </div>
    <div data-role="content" data-dom-cache="true">
        <div>
            <input id="txtSearch" type="search" value="" placeholder="Search" /></div>
        <br />
        <div id="divRecordCount">
        </div>
        <table data-role="table" id="tblDashboard" data-mode="reflow" class="ui-responsive">
            <thead>
                <tr>
                    <th>
                    </th>
                    <th>
                        Device Name
                    </th>
                    <th>
                        UUID
                    </th>
                    <th>
                        IMEI
                    </th>
                    @*  <th>
                        OS
                    </th>
                    <th>
                        Registered Date
                    </th>
                    <th>*@ @*    Registered By
                    </th>*@
                    <th>
                        Status
                    </th>
                    @*  <th>
                        Policy Updated
                    </th>*@
                    <th>
                        Action
                    </th>
                </tr>
            </thead>
            <tbody id='tblDashboardBody' data-theme="c">
            </tbody>
        </table>
        <br />
    </div>
    <a data-role="button" id="btnRefreshDashboard" data-theme="c">Refresh</a>
</div>
<div id="pageApplication" data-role="page">
    <div data-role="header">
        <h1>
            Installed Applications</h1>
    </div>
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
                <legend>Current Policy </legend>
                <label id="lblCurrentPolicy" data-role="field-contain">
                    Current Policy
                </label>
            </fieldset>
        </div>
        <div data-role="collapsible" data-collapsed="false" data-theme="c" data-content-theme="d"
            data-collapsed-icon="" data-expanded-icon="" data-inset="true">
            @* <img class="ui-li-icon ui-li-thumb" id="imgH" src="/Content/images/icons/whitelist2.png" alt="This application is already in the Whitelist"> -*@
            <h2>
                <label id="lbltitleOrgWL">
                    Select Application to add to WhiteList.
                </label>
            </h2>
            <div data-role="content">
                <label id="divNotif">
                    No Application Installed.
                </label>
                <ul id="divListApps" data-role="listview" data-inset="true">
                </ul>
            </div>
        </div>
        <a data-role="button" id="btnWAppRefresh" data-theme="c">Refresh</a> <a data-role="button"
            data-rel="back" data-theme="c">Back</a>
    </div>
</div>
<div id="pageFindMap" data-role="page">
    <div data-role="header">
        <h1>
            Device Location</h1>
    </div>
    <div id="divMap">
    </div>
    <a data-role="button" id="btnPolicyProfileSave" data-theme="c">Save</a> <a data-role="button"
        data-rel="back" data-theme="c">Back</a>
</div>
<div id="pageCertificateReport" data-role="page">
    <div data-role="header">
        <h1 id="hWLA">
            Certificate Invalidation Log</h1>
    </div>
    <div data-role="content">
        <div>
            <input id="txtSearchCertificateLog" type="search" value="" placeholder="Search" /></div>
        <br />
        <div id="divRecordCountCertificateLog">
        </div>
        <table data-role="table" id="tblCert" data-mode="reflow" class="ui-responsive">
            <thead>
                <tr>
                    <th>
                        Date Reported
                    </th>
                    <th>
                        Report Details
                    </th>
                    <th>
                    @*    User Name
                    </th>
                    <th>
                        Organization Name
                    </th>
                    <th>
                        Device Policy Profile
                    </th>*@
                </tr>
            </thead>
            <tbody id='tblCertBody' data-theme="c">
            </tbody>
        </table>
        <a data-role="button" id="btnCertBack" data-rel="back" data-theme="c">Back</a>
    </div>
</div>
<div id="pageAddWhiteList" data-role="page">
    <div data-role="header">
        <h1 id="headerWL">
            Add White-Listed Applications</h1>
    </div>
    <div data-role="content">
        <div data-role="fieldcontain">
            <fieldset data-role="controlgroup">
                <legend>Application Title <span style="color: #FF0000;">*</span></legend>
                <input type="text" id="txtApplicationName" data-role="field-contain" required>
                <span class="smallfontRed" id="spanApplicationTitle" style="display: none;">This field
                    is required.</span>
            </fieldset>
        </div>
        <div data-role="fieldcontain">
            <fieldset data-role="controlgroup">
                <legend>Application Name <span style="color: #FF0000;">*</span></legend>
                <input type="text" id="txtApplicationDesc" data-role="field-contain">
                <span class="smallfontRed" id="spanApplicationName" style="display: none;">This field
                    is required.</span>
            </fieldset>
        </div>
        @* <div data-role="fieldcontain">
            <fieldset data-role="controlgroup">
                <legend>Directory </legend>
                <input type="text" id="txtKeywords" data-role="field-contain">
            </fieldset>
        </div>*@
    </div>
    <a data-role="button" id="btnAppsWhiteListSave" data-theme="c">Add this app to company-wide
        white list</a> <a data-role="button" id="btnAppsWhiteListSpecificSave" data-theme="c">
            Add this app to policy white list</a> @* <a data-role="button" id="btnAppsWhiteListBack" data-theme="c">View All White-Listed Application</a>*@
    <a data-role="button" id="btnAWLBack" data-rel="back" data-theme="c">Back</a>
</div>
<div id="pageWipeDetails" data-role="page">
    <div data-role="header">
        <h1 id="headerWL">
            Wipe Out Device</h1>
    </div>
    <div data-role="content">
        <div data-role="content">
            <div data-role="fieldcontain">
                <fieldset data-role="controlgroup">
                    <legend>Device Name </legend>
                    <label id="lblDeviceNameW">
                        Device Name
                    </label>
                </fieldset>
            </div>
            <div data-role="fieldcontain">
                <fieldset data-role="controlgroup">
                    <legend>UUID </legend>
                    <label id="lblUUIDW">
                        UUID
                    </label>
                </fieldset>
            </div>
            <div data-role="fieldcontain">
                <fieldset data-role="controlgroup">
                    <legend>IMEI </legend>
                    <label id="lblIMEIW">
                        IMEI
                    </label>
                </fieldset>
            </div>
            <div data-role="fieldcontain">
                <fieldset data-role="controlgroup">
                    <legend>OS </legend>
                    <label id="lblOSW" data-role="field-contain">
                        OS
                    </label>
                </fieldset>
            </div>
        </div>
        <div data-role="fieldcontain">
            <div data-role="content">
                Are you sure you want to wipe out the storage of this device? Deleted files cannot
                be recover once this transaction is commited. To commit this transaction, please
                input the name of the device:
            </div>
            <fieldset data-role="controlgroup">
                <input type="text" id="txtDeviceNameToWipe" data-role="field-contain">
                <br/><span class="smallfontRed" id="spanDeviceNameToWipe" style="display: none;">Device Name not match.</span>
            </fieldset>
        </div>
    </div>
    <a data-role="button" id="btnWipeOut" data-theme="c">Wipe Out this Device</a> <a
        data-role="button" id="btnWipeBack" data-rel="back" data-theme="c">Back</a>
</div>
