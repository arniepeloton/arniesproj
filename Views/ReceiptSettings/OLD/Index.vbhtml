@Code
    ViewData("Title") = "Receipt Settings List"
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head

    <script>
        require([GetPathFormat('/Scripts/sources/ReceiptSettings/ReceiptSettingsMenu.js?crc=')]);
    </script>
    
End Section

<div id="pageReceiptSettingsList" data-role="page">

@*    <div data-role="header">
        <h1>@ViewData("Title")</h1>
    </div>*@

     <div data-role="header" data-position="fixed1" style="background: none white; border: none">
        <h1>@ViewData("Title")</h1>

        <div class="ui-grid-d">
            <div class="ui-block-a" style="width: 10%;">
                <div id="btnHome" style="margin: 0px; height: 55px;" data-role="button" data-theme="b" data-corners="false">
                    <span>Home</span>
                </div>
            </div>
            <div class="ui-block-b" style="width: 30%;">
                <div id="btnSystemAdmin" style="margin: 0px; height: 55px;" data-role="button" data-theme="b" data-corners="false">
                    <span>System Administration</span>
                </div>
            </div>
            <div class="ui-block-c" style="width: 30%;">
                <div id="btnSystemSettings" style="margin: 0px; height: 55px;" data-role="button" data-theme="b" data-corners="false">
                    <span>System Settings</span>
                </div>
            </div>
            <div class="ui-block-d" style="width: 30%;">
                <div id="btnReceiptSettings" style="margin: 0px; height: 55px;" data-role="button" data-theme="b" data-corners="false">
                    <span>Receipt Settings</span>
                </div>
            </div>
        </div>

    </div>

    <div data-role="content">

        <form id="frmReceiptSettingsBrowser" style="margin: 0 -7px">
            <div>
                <input id="txtReceiptSearch" type="search" placeholder="Search a Receipt Template" value="" />
            </div>
        </form>

        <div id="divReceiptSearchResults" style="display:none;">

            <div class="ListViewMarginBottom" id="divReceiptRecordCount"></div>

            <ul id="lstReceiptSettings" data-role="listview" data-filter="true">
            </ul>

        </div>

        <div id="divReceiptSearchNoResults" style="display:none;">
	        <div>
                <div data-role="fieldcontain">
                    <label id="lblReceiptNotFound"></label>
                </div>
            </div>
        </div>

        <div>
            <br />
            <br />
	        <div style="text-align:center;"> 
                <a id="btnAddNewReceiptSettings" data-role="button" style="display:none;"><h3>Add New Receipt Template</h3></a> 
            </div>
            <div style="text-align: center">
                <a id="btnDoneReceiptSettings" data-role="button"><h3>Go Back to Main</h3></a>
            </div>
            <div style="text-align: center">
                <a id="btnReceiptSettingsGoToSetupChecklist" data-role="button"><h3>Go to Setup Checklist</h3></a>
            </div>
        </div>

    </div>

</div>

