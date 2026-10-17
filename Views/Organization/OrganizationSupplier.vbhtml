<script>
    require([GetPathFormat('/Scripts/sources/Organization/OrganizationSupplier.js?crc='),
                 GetPathFormat('/Scripts3/Browser/LibraryOrganization.js?crc=')
        ]);
</script>

<div id="pageOrgSupplier" style="display:none;">

    @*<div id"divHeader" data-role="header">
        <h1>Supplier List</h1>
    </div>*@

     <div data-role="header" data-position="fixed1" style="background: none white; border: none">
        <h1>Supplier List</h1>

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
                <div id="btnDataLibraries" style="margin: 0px; height: 55px;" data-role="button" data-theme="b" data-corners="false">
                    <span>Data Libraries</span>
                </div>
            </div>
            <div class="ui-block-d" style="width: 30%;">
                <div id="btnCompanySupplier" style="margin: 0px; height: 55px;" data-role="button" data-theme="b" data-corners="false">
                    <span>Supplier/Vendor</span>
                </div>
            </div>
        </div>

    </div>

    <div data-role="content" >

        <div>
            <ul data-role="listview" id="lstOrgSupplierContainer" data-inset="true">   
				<li data-role="list-divider">Supplier List</li>
                <li>
                    <div data-role="fieldcontain"><a id="btnOrgSupAddSupplier" data-role="button" data-icon="plus" data-inline="true">Add Supplier</a></div>
                    <div data-role="fieldcontain" id="divOrgSupRecordCountSupplier"></div>
                    <div>
                        <ul data-role="listview" id="lstOrgSupplier" data-inset="true">   
                        </ul>
                    </div>
                </li>
            </ul>
        </div>

        <div>
            <br />
            <br />
            <div style="text-align: center">
                <a id="btnOrgSupplierDone" data-role="button"><h3>Go Back to Main</h3></a>
            </div>
            <div style="text-align: center">
                <a id="btnOrgSupplierGoToSetupChecklist" data-role="button"><h3>Go to Setup Checklist</h3></a>
            </div>
        </div>

    </div>

</div>

<div id="pageOrgSupplierInfo" style="display:block;">

    @*<div data-role="header">
        <h1>Customer Info</h1>
    </div>*@

    <div data-role="content">

        <div data-role="fieldcontain" style="display:none;">
	        <input type="text" readonly id="txtOrgSupplierID"/>                
        </div>

        <div data-role="fieldcontain">
	        <label for="btnOrgSupSupplier">Supplier:</label>
            <div style="display:none;"><input type="text" readonly id="txtOrgSupSupplierID"/></div>
            <button id="btnOrgSupSupplier" data-role="button">Please Select</button>
            
        </div>
        
        <div data-role="fieldcontain">
            <label for="butOrgSupplierCreditTerm">Credit Term:</label>
            <button id="butOrgSupplierCreditTerm" data-role="button">Please Select</button>
        </div>

        <div data-role="fieldcontain">
            <label for="txtOrgSupplierCreditLimit">Credit Limit:</label>
            <input id="txtOrgSupplierCreditLimit" />
        </div>

        <div data-role="fieldcontain">
            <label for="txtOrgSupplierBeginningBalance">Beginning Balance:</label>
            <input id="txtOrgSupplierBeginningBalance" />
        </div>

        <div data-role="fieldcontain">
            <label for="butOrgSupplierBeginningBalanceAsOf">Beginning Balance As Of:</label>
            <button id="butOrgSupplierBeginningBalanceAsOf" data-role="button">Please Select</button>
        </div>

         <div data-role="fieldcontain">
	        <label for="selOrgSupActivationDateMonth">Activation Date: <span style="color: #FF0000;" >*</span></label>
            <fieldset data-role="controlgroup" data-type="horizontal">
                <select id="selOrgSupActivationDateMonth">
	                <option value="">MMM</option>
                </select>
                <select id="selOrgSupActivationDateDay">
	                <option value="">DD</option>
                </select>                
                <select id="selOrgSupActivationDateYear">
	                <option value="">YYYY</option>
                </select>                
            </fieldset>
          </div>

          <div data-role="fieldcontain">
	        <label for="selOrgSupDeactivate">Deactivate record?</label>
            <select id="selOrgSupDeactivate" data-role="slider">
			    <option value="0">No</option>
	            <option value="1">Yes</option>
            </select>
        </div>

        <div id="divOrgSupDeactivateDetails" style="display:none;">

	        <label for="selOrgSupDateInActiveMonth">Deactivation Date: <span style="color: #FF0000;" >*</span></label>
            <fieldset data-role="controlgroup" data-type="horizontal">
                <div>
                    <select id="selOrgSupDateInActiveMonth">
	                    <option value="">MMM</option>
                    </select>
                    <select id="selOrgSupDateInActiveDay">
	                    <option value="">DD</option>
                    </select>                
                    <select id="selOrgSupDateInActiveYear">
	                    <option value="">YYYY</option>
                    </select>                
                    </div>            
            </fieldset>

            <div data-role="fieldcontain">
                <label for="txtOrgSupInActiveReason">Reason for deactivating:</label>
	            <textarea id="txtOrgSupInActiveReason"></textarea>
            </div>

            <div data-role="fieldcontain">
                <label for="txtOrgSupComments">Comments:</label>
	            <textarea id="txtOrgSupComments"></textarea>
            </div>

	        <label for="selOrgSupBan">Ban this record for future transactions?</label>
            <select id="selOrgSupBan" data-role="slider">
				<option value="0">No</option>
	            <option value="1">Yes</option>
            </select>

        </div>

        <div>
            <br />
            <br />
            <div style="text-align: center">
                <a id="btnOrgSupplierInfoSave" data-role="button"><h3>Ok</h3></a>
            </div>
            <div style="text-align: center">
                <a id="btnOrgSupplierInfoCancel" data-role="button"><h3>Cancel</h3></a>
            </div>
        </div>

    </div>

</div>
