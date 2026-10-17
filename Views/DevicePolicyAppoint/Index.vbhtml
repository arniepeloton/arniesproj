@Code
    ViewData("Title") = "Assign Policy to a Device"
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code
@Section head
    <script>
        require([GetPathFormat('/Scripts/sources/DeviceMDMConfig/DevicePolicyAppoint.js?crc=')]);
    </script>
]
End Section
<div id="pageMain" data-role="page">
    <div data-role="header">
        <h1 id="headPolicyDevice">
            Policy</h1>
    </div>
    <div data-role="content" data-dom-cache="true">
        Select Policy to Apply:</h1>
        <div data-role="content">
            <select id="selPolicyAppoint">
                <option value="0">Select Policy </option>
            </select>

        </div>
        <a id="btnSaveDevicePolicyAppointment" data-role="button" data-theme="c">Save</a>
        <a id="btnBackDevicePolicyAppointment" data-role="button" data-theme="c">
            Back</a>
    </div>
    </div>
<div id="pageMainChoices" data-role="page">
    <div data-role="header">
        <h1>@ViewData("Title")</h1>
    </div>
    <div data-role="content" data-dom-cache="true">
        <div>
            <input id="txtSearch" type="search" value="" placeholder="Search" /></div>
        <div>
            <ul id="lstDevicePolicyAppoint" data-role="listview" data-inset="true" style="min-width: 210px;">
            </ul>
        </div>
        <a id="btnBack" data-role="button" data-theme="c">Back</a>
    </div>
</div>

