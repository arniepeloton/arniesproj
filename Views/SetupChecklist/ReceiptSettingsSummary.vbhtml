@Code
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head

    <script>
        require(['/Scripts/sources/ReceiptSettings/ReceiptSettingsSummary.js']);
    </script>
    
End Section

<div id="pageReceiptSettings" data-role="page">

    <div id"divHeader" data-role="header">
        <h1>Receipt Settings Summary</h1>
    </div>

    <div data-role="content" >

        <div>
            <ul data-role="listview" data-inset="true">

				<li data-role="list-divider">
                    <div data-role="fieldcontain" style="height:5px;">
                        Receipt Settings
                        <a href='ReceiptSettingsInfo' data-role='button' data-mini="true" style="float:right; margin-top:-15px;">Edit</a>
                    </div>
                </li>
                <li>
                    <div data-role="fieldcontain">
	                    <input type="text" readonly style="display:none;" id="txtID"/>                
                    </div>

                    <div data-role="fieldcontain">
                        <fieldset data-role="controlgroup" class="ui-corner-all ui-controlgroup ui-controlgroup-vertical">
	                        <div><label for="btnOrganization">Organization: <span style="color: #FF0000;" >*</span></label></div>
                            <div><input type="text" readonly style="display:none;" id="txtOrganizationID"/></div>
                            <div><input type="text" readonly id="txtOrganization"/></div>
                        </fieldset>
                    </div>

                    <div data-role="fieldcontain">
                        <fieldset data-role="controlgroup" class="ui-corner-all ui-controlgroup ui-controlgroup-vertical">
	                        <div><label for="btnOrganizationTradeName">Business or Trade Name: <span style="color: #FF0000;" >*</span></label></div>
                            <div><input type="text" readonly style="display:none;" id="txtOrganizationTradeNameID"/></div>
                            <div><input type="text" readonly id="txtOrganizationTradeName"/></div>
                        </fieldset>
                    </div>

                    <div data-role="fieldcontain">
                        <fieldset data-role="controlgroup" class="ui-corner-all ui-controlgroup ui-controlgroup-vertical">
	                        <div><label for="txtCorporateTIN">Tax Identification No. (TIN): <span style="color: #FF0000;" >*</span></label></div>
	                        <div><input type="text" readonly id="txtCorporateTIN"/></div>  
                        </fieldset>
                    </div>

                </li>

                <li data-role="list-divider">
                    <div data-role="fieldcontain" style="height:5px;">
                        Message List
                        @*<a href='ReceiptSettingsMessage' data-role='button' data-mini="true" style="float:right; margin-top:-15px;">Edit</a>*@
                    </div>
                </li>
                <li>
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
            <div style="text-align:center"> <a id="btnReceiptSettingsSummaryBack" data-role="button">Done</a> </div>
        </div>
    </div>

</div>

