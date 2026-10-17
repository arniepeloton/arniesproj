@Code
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head
    <script>
        require([GetPathFormat('/Scripts/sources/Organization/OrganizationContact.js?crc='),
                 GetPathFormat('/Scripts3/Browser/LibraryCountries.js?crc='),
                 GetPathFormat('/Scripts3/Browser/LibraryAreaCodes.js?crc='),
                 GetPathFormat('/Scripts3/Browser/LibraryTelcoPrefixes.js?crc=')
        ]);
    </script>    
End Section

<div id="pageContactList" data-role="page">
    <div data-role="header">
        <h1>Organization Contact</h1>
    </div>

    <div data-role="content">

        <ul data-role="listview" data-inset="true" style="min-width: 210px;">
            <li data-role="list-divider">Telephone</li>
            <li>
                <div data-role="fieldcontain"><a id="btnOrgContactTelAdd" data-role="button" data-icon="plus" data-inline="true">Add Telephone Contact</a></div>
                <div data-role="fieldcontain" id="divOrgRecordCountTel"></div>
                <div>
                    <ul data-role="listview" id="lstOrgContactTel" data-inset="true">
                    </ul>
                </div>
            </li>

            <li data-role="list-divider">Fax</li>
            <li>
                <div data-role="fieldcontain"><a id="btnOrgContactFaxAdd" data-role="button" data-icon="plus" data-inline="true">Add Fax Contact</a></div>
                <div data-role="fieldcontain" id="divOrgRecordCountFax"></div>
                <div>
                    <ul data-role="listview" id="lstOrgContactFax" data-inset="true">
                    </ul>
                </div>
            </li>

            <li data-role="list-divider">Mobile</li>
            <li>
                <div data-role="fieldcontain"><a id="btnOrgContactMobAdd" data-role="button" data-icon="plus" data-inline="true">Add Mobile Contact</a></div>
                <div data-role="fieldcontain" id="divOrgRecordCountMob"></div>
                <div>
                    <ul data-role="listview" id="lstOrgContactMob" data-inset="true">
                    </ul>
                </div>
            </li>

            <li data-role="list-divider">Email</li>
            <li>
                <div data-role="fieldcontain"><a id="btnOrgContactEmailAdd" data-role="button" data-icon="plus" data-inline="true">Add Email</a></div>
                <div data-role="fieldcontain" id="divOrgRecordCountEmail"></div>
                <div>
                    <ul data-role="listview" id="lstOrgContactEmail" data-inset="true">
                    </ul>
                </div>
            </li>

            <li data-role="list-divider">Website</li>
            <li>
                <div data-role="fieldcontain"><a id="btnOrgContactWebsiteAdd" data-role="button" data-icon="plus" data-inline="true">Add Website</a></div>
                <div data-role="fieldcontain" id="divOrgRecordCountWebsite"></div>
                <div>
                    <ul data-role="listview" id="lstOrgContactWebsite" data-inset="true">
                    </ul>
                </div>
            </li>

        </ul>

        <div>
            <br />
            <br />
            <div style="text-align: center">
                <a id="btnOrgContactDone" data-role="button"><h3>Done</h3></a>
            </div>
        </div>

    </div>

</div>

<div id="pageOrgContactTelInfo" data-role="page">

    <div data-role="header">
        <h1>Contact Telephone Info</h1>
        <a href="#" data-icon="arrow-l" data-rel="back">Back</a>
    </div>

    <div data-role="content">

        <div data-role="fieldcontain" style="display:none;">
	        <input type="text" readonly id="txtOrgContactTelID"/>                
        </div>

        <div data-role="fieldcontain">

            <div style="display:none;"><input type="text" readonly id="txtOrgTelCountryID"/></div>
            <div style="display:none;"><input type="text" readonly id="txtOrgTelAreaCodeID"/></div>
            <div class="ui-grid-a">
                <div class="ui-block-a">
                    <button id="btnOrgTelCountryCode" data-role="button">Country Code</button>
                </div>

                <div class="ui-block-b">
                    <button id="btnOrgTelAreaCode" data-role="button">Area Code</button>
                </div>
            </div>

        </div>

        <div data-role="fieldcontain">
	        <label for="txtOrgContactNoTel">Telephone No.: <span style="color: #FF0000;" >*</span></label>
            <input type="number" id="txtOrgContactNoTel" maxlength="7"/>

        </div>

        <div data-role="fieldcontain">
	        <label for="txtOrgContactExtensionTel">Extension or Local No.:</label>
            <input type="number" id="txtOrgContactExtensionTel" maxlength="10"/>
   
        </div>

        <div>
            <br />
            <br />
            <div style="text-align: center">
                <a id="btnOrgContactTelSave" data-role="button"><h3>Ok</h3></a>
            </div>
            <div style="text-align: center;">
                <a id="btnOrgContactTelRemove" data-role="button" style="display:none;"><h3>Remove</h3></a>
            </div>
            <div style="text-align: center">
                <a id="btnOrgContactTelCancel" data-role="button"><h3>Cancel</h3></a>
            </div>
        </div>

    </div>

</div>

<div id="pageOrgContactFaxInfo" data-role="page">

    <div data-role="header">
        <h1>Contact Fax Info</h1>
        <a href="#" data-icon="arrow-l" data-rel="back">Back</a>
    </div>

    <div data-role="content">

        <div data-role="fieldcontain" style="display:none;">
	        <input type="text" readonly id="txtOrgContactFaxID"/>                
        </div>

        <div data-role="fieldcontain">

            <div style="display:none;"><input type="text" readonly id="txtOrgFaxCountryID"/></div>
            <div style="display:none;"><input type="text" readonly id="txtOrgFaxAreaCodeID"/></div>
            <div class="ui-grid-a">
                <div class="ui-block-a">
                    <button id="btnOrgFaxCountryCode" data-role="button">Country Code</button>
                </div>

                <div class="ui-block-b">
                    <button id="btnOrgFaxAreaCode" data-role="button">Area Code</button>
                </div>
            </div>

        </div>

        <div data-role="fieldcontain">
	        <label for="txtOrgContactNoFax">Fax No.: <span style="color: #FF0000;" >*</span></label>
            <input type="number" id="txtOrgContactNoFax" maxlength="7"/>
        </div>

        <div data-role="fieldcontain">
	        <label for="txtOrgContactExtensionFax">Extension or Local No.:</label>
            <input type="number" id="txtOrgContactExtensionFax" maxlength="10"/>
        </div>

        <div>
            <br />
            <br />
            <div style="text-align: center">
                <a id="btnOrgContactFaxSave" data-role="button"><h3>Ok</h3></a>
            </div>
            <div style="text-align: center;">
                <a id="btnOrgContactFaxRemove" data-role="button" style="display:none;"><h3>Remove</h3></a>
            </div>
            <div style="text-align: center">
                <a id="btnOrgContactFaxCancel" data-role="button"><h3>Cancel</h3></a>
            </div>
        </div>

    </div>

</div>

<div id="pageOrgContactMobileInfo" data-role="page">

    <div data-role="header">
        <h1>Contact Mobile Info</h1>
        <a href="#" data-icon="arrow-l" data-rel="back">Back</a>
    </div>

    <div data-role="content">

        <div data-role="fieldcontain" style="display:none;">
	        <input type="text" readonly id="txtOrgContactMobileID"/>                
	        <input type="text" readonly id="txtOrgMobileCountryID"/>                
	        <input type="text" readonly id="txtOrgTelCoPrefixID"/>                
        </div>

        <div data-role="fieldcontain">

            <div class="ui-grid-a">
                <div class="ui-block-a">
                    <button id="btnOrgMobileCountryCode" data-role="button">Country Code</button>
                </div>

                <div class="ui-block-b">
                    <button id="btnOrgMobileTelCoPrefixCode" data-role="button">Telco Code</button>
                </div>
            </div>

        </div>

        <div data-role="fieldcontain">
	        <label for="txtOrgContactNoMobile">Mobile No.: <span style="color: #FF0000;" >*</span></label>
            <input type="number" id="txtOrgContactNoMobile" maxlength="7"/>
        </div>

        <div>
            <br />
            <br />
            <div style="text-align: center">
                <a id="btnOrgContactMobileSave" data-role="button"><h3>Ok</h3></a>
            </div>
            <div style="text-align: center;">
                <a id="btnOrgContactMobileRemove" data-role="button" style="display:none;"><h3>Remove</h3></a>
            </div>
            <div style="text-align: center">
                <a id="btnOrgContactMobileCancel" data-role="button"><h3>Cancel</h3></a>
            </div>
        </div>

    </div>

</div>

<div id="pageOrgContactEmail" data-role="page">

    <div data-role="header">
        <h1>Contact Email Info</h1>
        <a href="#" data-icon="arrow-l" data-rel="back">Back</a>
    </div>

    <div data-role="content">

        <div data-role="fieldcontain" style="display:none;">
	        <input type="text" readonly id="txtOrgContactEmailID"/>                
        </div>

        <div data-role="fieldcontain">
	        <label for="txtOrgContactEmail">Email Address: <span style="color: #FF0000;" >*</span></label>
            <input type="text" id="txtOrgContactEmail"/>
        </div>

        <div>
            <br />
            <br />
            <div style="text-align: center">
                <a id="btnOrgContactEmailSave" data-role="button"><h3>Ok</h3></a>
            </div>
            <div style="text-align: center;">
                <a id="btnOrgContactEmailRemove" data-role="button" style="display:none;"><h3>Remove</h3></a>
            </div>
            <div style="text-align: center">
                <a id="btnOrgContactEmailCancel" data-role="button"><h3>Cancel</h3></a>
            </div>
        </div>

    </div>

</div>

<div id="pageOrgContactWebsite" data-role="page">

    <div data-role="header">
        <h1>Contact Website Info</h1>
        <a href="#" data-icon="arrow-l" data-rel="back">Back</a>
    </div>

    <div data-role="content">

        <div data-role="fieldcontain" style="display:none;">
	        <input type="text" readonly id="txtOrgContactWebsiteID"/>                
        </div>

        <div data-role="fieldcontain">
	        <label for="txtOrgContactWebsite">Website: <span style="color: #FF0000;" >*</span></label>
            <input type="text" id="txtOrgContactWebsite"/>
        </div>

        <div>
            <br />
            <br />
            <div style="text-align: center">
                <a id="btnOrgContactWebSave" data-role="button"><h3>Ok</h3></a>
            </div>
            <div style="text-align: center;">
                <a id="btnOrgContactWebRemove" data-role="button" style="display:none;"><h3>Remove</h3></a>
            </div>
            <div style="text-align: center">
                <a id="btnOrgContactWebCancel" data-role="button"><h3>Cancel</h3></a>
            </div>
        </div>

    </div>

</div>
