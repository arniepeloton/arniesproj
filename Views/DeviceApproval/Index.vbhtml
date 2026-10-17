@Code
    ViewData("Title") = "Device Approval"
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code
@Section head
    <script>
        require([GetPathFormat('/Scripts/sources/DeviceMDMConfig/DeviceRegistrationApproval.js?crc=')]);
    </script>
End Section
<div data-role="page" data-close-btn="none" id="dialog">
    <div data-role="header" data-theme="d">
        <h1>
            Registration Approval</h1>
    </div>
    <div data-role="content" data-theme="c">
        <p>
            <strong>Phone Name: </strong>
            <label id="lblPhoneName">
            </label>
        </p>
        <p>
            <strong>Phone Model: </strong>
            <label id="lblPhoneModel">
            </label>
        </p>
        <p>
            <strong>IMEI: </strong>
            <label id="lblIMEI">
            </label>
        </p>
        <p>
            <strong>UUID: </strong>
            <label id="lblUUID">
            </label>
        </p>
        <p>
            <strong>OS: </strong>
            <label id="lblOS">
            </label>
        </p>
        <p>
            <strong>Device Number: </strong>
            <label id="lblDeviceNo">
            </label>
        </p>
        <br />
        <a id="btnApprove" data-role="button" data-theme="c">Approve</a> <a data-role="button"
           id="btnApproveBack"  data-theme="c">Back to Dashboard</a>
    </div>
    <div id="pageMain" data-role="dialog">
        <div data-role="header">
            <h1>@ViewData("Title")</h1>
        </div>
        <div data-role="content" data-dom-cache="true">
            <div>
                <input id="txtSearch" type="search" value="" placeholder="Search" /></div>
            <div>
                <ul id="lstApproval" data-role="listview" data-inset="true" style="min-width: 210px;">
                </ul>
            </div>
        </div>
    </div>
</div>
