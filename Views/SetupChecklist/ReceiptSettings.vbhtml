@Code
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head

    <script type="text/javascript" src="../../Scripts/sources/ReceiptSettings/ReceiptSettings.js"></script>
    
End Section

<div id="pageReceiptSettings" data-role="page">

    <div id"divHeader" data-role="header">
        <a id="btnGoTo" href="#popupMenu" data-rel="popup" data-inline="true" class="ui-btn-right">Go To</a>
        <h1>Receipt Settings</h1>
    </div>

    <div data-role="content" >

        <div data-role="fieldcontain">
	        <input type="text" readonly style="display:none;" id="txtID"/>                
        </div>

        <div data-role="fieldcontain">
            <fieldset data-role="controlgroup" class="ui-corner-all ui-controlgroup ui-controlgroup-vertical">
	            <div><label for="btnOrganization">Organization: <span style="color: #FF0000;" >*</span></label></div>
                <div><input type="text" readonly style="display:none;" id="txtOrganizationID"/></div>
                <div><button id="btnOrganization" data-role="button">Please Select</button></div>
            </fieldset>
        </div>

        <div data-role="fieldcontain">
            <fieldset data-role="controlgroup" class="ui-corner-all ui-controlgroup ui-controlgroup-vertical">
	            <div><label for="btnOrganizationTradeName">Business or Trade Name: <span style="color: #FF0000;" >*</span></label></div>
                <div><input type="text" readonly style="display:none;" id="txtOrganizationTradeNameID"/></div>
                <div><button id="btnOrganizationTradeName" data-role="button">Please Select</button></div>
            </fieldset>
        </div>

        <div data-role="fieldcontain">
            <fieldset data-role="controlgroup" class="ui-corner-all ui-controlgroup ui-controlgroup-vertical">
	            <div><label for="txtCorporateTIN">Tax Identification No. (TIN): <span style="color: #FF0000;" >*</span></label></div>
	            <div><input type="text" id="txtCorporateTIN"/></div>  
            </fieldset>
        </div>

        <div>
            <ul data-role="listview" id="lstMessageContainer" data-inset="true">   
				<li data-role="list-divider">Receipt Message List</li>
                <li>
                    <div data-role="fieldcontain"><a href="#" id="btnAddMessage" data-role="button" data-icon="plus" data-inline="true">Add Message</a></div>
                    <div>
                        <ul data-role="listview" id="lstMessages" data-inset="true">
                        </ul>
                    </div>
                </li>
            </ul>
        </div>

        <div>
            <br />
            <br />
	        <div style="text-align:center"> <a id="btnReceiptSettingsNext" data-role="button">Next</a> </div>
	        <div style="text-align:center"> <a id="btnReceiptSettingsBack" data-role="button">Back</a> </div>
	        <div style="text-align:center"> <a id="btnReceiptSettingsDone" data-role="button">Done</a> </div>
        </div>

        <div id="popupMenu" data-role="popup">
            <ul data-role="listview" data-inset="true" style="min-width: 210px;">
                <li data-role="divider">Menu</li>
                <li><a href="ReceiptSettings">Receipt Settings</a></li>
                <li><a href="ReceiptDevices">Devices</a></li>
                <li><a href="ReceiptPlaces">Places</a></li>
            </ul>
        </div>

    </div>

</div>

<div id="pageMessages" data-role="page">

    <div data-role="header">
        <h1>Receipt Messages</h1>
    </div>

    <div data-role="content">

        <div data-role="fieldcontain">
	        <input type="text" readonly style="display:none;" id="txtMessageID"/>                
        </div>

        <div data-role="fieldcontain">
            <fieldset data-role="controlgroup" class="ui-corner-all ui-controlgroup ui-controlgroup-vertical">
	            <div><label for="txtMessageFooter">Message Footer: <span style="color: #FF0000;" >*</span></label></div>
	            <div><input type="text" id="txtMessageFooter"/></div>              
            </fieldset>
        </div>

        <div data-role="fieldcontain">
            <fieldset data-role="controlgroup" class="ui-corner-all ui-controlgroup ui-controlgroup-vertical">
	            <div><label for="txtSortOrder">Sort Order: <span style="color: #FF0000;" >*</span></label></div>
	            <div><input type="text" id="txtSortOrder"/></div>              
            </fieldset>
        </div>

        <div>  
            <br />
            <br />
	        <div style="text-align:center"> <a id="btnMessageSave" data-role="button">Ok</a> </div>
	        <div style="text-align:center"> <a id="btnMessageRemove" data-role="button">Remove</a> </div>
	        <div style="text-align:center"> <a id="btnMessageCancel" data-role="button">Cancel</a> </div>
        </div>

    </div>

</div>
