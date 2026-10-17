<script>
    require([GetPathFormat('/Scripts/sources/OrganizationShort/OrganizationRepresentative.js?crc='),
            GetPathFormat('/Scripts3/Browser/LibraryCountries.js?crc='),
            GetPathFormat('/Scripts3/Browser/LibraryAreaCodes.js?crc='),
            GetPathFormat('/Scripts3/Browser/LibraryTelcoPrefixes.js?crc=')
        ]);
</script>

<div id="pageRepList" style="display:block;">

    <div data-role="header">
        <h2>Organization Representative List</h2>
        @*<a href="#" data-icon="arrow-l" data-rel="back">Back</a>*@
    </div>

    <div data-role="content">
        <div data-role="fieldcontain"><a id="btnOrgRepAdd" data-role="button" data-icon="plus" data-inline="true">Add Representative</a></div>
        <div data-role="fieldcontain" id="divOrgRepRecordCount"></div>
        <div>
            <ul data-role="listview" id="lstOrgRepresentative" data-inset="true">
            </ul>
        </div>
        <div>
            <br />
            <br />
            <div style="text-align: center">
                <a id="btnOrgRepDone" data-role="button"><h3>Done</h3></a>
            </div>
        </div>

    </div>

</div>

<div id="pageOrgRepInfo" style="display:none;">

    <div data-role="header">
        <h2>Organization Representative</h2>
        @*<a href="#" data-icon="arrow-l" data-rel="back">Back</a>*@
    </div>

    <div data-role="content">

        <div data-role="fieldcontain" style="display:none;">
	        <input type="text" readonly id="txtOrgRepID"/>                
        </div>

        <div data-role="fieldcontain">
            <label for="txtOrgRepTitle">Title:</label>
	        <input type="text" id="txtOrgRepTitle"/>
           
        </div>

        <div data-role="fieldcontain">
            <label for="txtOrgRepLastName">Last Name: <span style="color: #FF0000;" >*</span></label>
	        <input type="text" id="txtOrgRepLastName"/>
        </div>

        <div data-role="fieldcontain">
            <label for="txtOrgRepFirstName">First Name: <span style="color: #FF0000;" >*</span></label>
	        <input type="text" id="txtOrgRepFirstName"/>
        </div>

        <div data-role="fieldcontain">
            <label for="txtOrgRepMiddleName">Middle Name: </label>
	        <input type="text" id="txtOrgRepMiddleName"/>

        </div>

        <div data-role="fieldcontain">
	        <label for="selOrgRepGenSuffixName" class="select">Generational Suffix (Name):</label>
            <select id="selOrgRepGenSuffixName">
	            <option value="">Please Select</option>
				<option value="Sr.">Senior (Sr.)</option>
				<option value="Jr.">Junior (Jr.)</option>
				<option value="II">II</option>
				<option value="III">III</option>
				<option value="IV">IV</option>
	            <option value="V">V</option>
				<option value="VI">VI</option>
	            <option value="VII">VII</option>
				<option value="VIII">VIII</option>
				<option value="IX">IX</option>
				<option value="X">X</option>
            </select>
        </div>

        <div data-role="fieldcontain">
	        <label for="selOrgRepIsActive">Is this representative is active?</label>
            <select id="selOrgRepIsActive" data-role="slider">
				<option value="0">No</option>
	            <option value="1">Yes</option>
            </select>
        </div>

        <ul data-role="listview" data-inset="true" style="min-width: 210px;">
            <li data-role="list-divider">Telephone</li>
            <li>
                <div data-role="fieldcontain"><a id="btnOrgRepContactTelAdd" data-role="button" data-icon="plus" data-inline="true">Add Telephone Contact</a></div>
                <div data-role="fieldcontain" id="divOrgRepRecordCountTel"></div>
                <div>
                    <ul data-role="listview" id="lstOrgRepContactTel" data-inset="true">
                    </ul>
                </div>
            </li>

            <li data-role="list-divider">Mobile</li>
            <li>
                <div data-role="fieldcontain"><a id="btnOrgRepContactMobAdd" data-role="button" data-icon="plus" data-inline="true">Add Mobile Contact</a></div>
                <div data-role="fieldcontain" id="divOrgRepRecordCountMob"></div>
                <div>
                    <ul data-role="listview" id="lstOrgRepContactMob" data-inset="true">
                    </ul>
                </div>
            </li>

            <li data-role="list-divider">Email</li>
            <li>
                <div data-role="fieldcontain"><a id="btnOrgRepContactEmailAdd" data-role="button" data-icon="plus" data-inline="true">Add Email</a></div>
                <div data-role="fieldcontain" id="divOrgRepRecordCountEmail"></div>
                <div>
                    <ul data-role="listview" id="lstOrgRepContactEmail" data-inset="true">
                    </ul>
                </div>
            </li>

        </ul>

        <div>
            <br />
            <br />
            <div style="text-align: center">
                <a id="btnOrgRepSave" data-role="button"><h3>Ok</h3></a>
            </div>
            <div style="text-align: center">
                <a id="btnOrgRepCancel" data-role="button"><h3>Cancel</h3></a>
            </div>
        </div>

    </div>

</div>

<div id="pageOrgRepContactTelInfo" style="display:none;">

    <div data-role="header">
        <h2>Contact Telephone Info</h2>
        @*<a href="#" data-icon="arrow-l" data-rel="back">Back</a>*@
    </div>

    <div data-role="content">

        <div data-role="fieldcontain" style="display:none;">
	        <input type="text" readonly id="txtOrgRepContactTelID"/>                
            <input type="text" readonly id="txtOrgRepTelCountryID"/>
            <input type="text" readonly id="txtOrgRepTelAreaCodeID"/>
        </div>

        <div data-role="fieldcontain">

            <div class="ui-grid-a">
                <div class="ui-block-a">
                    <button id="btnOrgRepTelCountryCode" data-role="button">Country Code</button>
                </div>

                <div class="ui-block-b">
                    <button id="btnOrgRepTelAreaCode" data-role="button">Area Code</button>
                </div>
            </div>

        </div>

        <div data-role="fieldcontain">
            <label for="txtOrgRepContactNoTel">Telephone No.: <span style="color: #FF0000;" >*</span></label>
	        <input type="number" id="txtOrgRepContactNoTel" maxlength="7"/>
        </div>

        <div data-role="fieldcontain">
            <label for="txtOrgRepContactExtensionTel">Extension or Local No.:</label>
	        <input type="number" id="txtOrgRepContactExtensionTel" maxlength="10"/>
        </div>

        <div data-role="fieldcontain">
	        <label for="selOrgRepContactTypeTel" class="select">Contact Type: <span style="color: #FF0000;" >*</span></label>
            <select id="selOrgRepContactTypeTel">
	            <option value="">Please Select</option>
	            <option value="Work Telephone">Work Telephone</option>
				<option value="Personal Telephone">Personal Telephone</option>
				<option value="Third Party Telephone">Third Party Telephone</option>
            </select>
        </div>

        <div>
            <br />
            <br />
            <div style="text-align: center">
                <a id="btnOrgRepContactTelSave" data-role="button"><h3>Ok</h3></a>
            </div>
            <div style="text-align: center">
                <a id="btnOrgRepContactTelRemove" data-role="button"><h3>Remove</h3></a>
            </div>
            <div style="text-align: center">
                <a id="btnOrgRepContactTelCancel" data-role="button"><h3>Cancel</h3></a>
            </div>
        </div>

    </div>

</div>

<div id="pageOrgRepContactMobileInfo" style="display:none;">

    <div data-role="header">
        <h2>Contact Mobile Info</h2>
        @*<a href="#" data-icon="arrow-l" data-rel="back">Back</a>*@
    </div>

    <div data-role="content">

        <div data-role="fieldcontain" style="display:none;">
	        <input type="text" readonly id="txtOrgRepContactMobileID"/>                
            <input type="text" readonly id="txtOrgRepMobileCountryID"/>
            <input type="text" readonly id="txtOrgRepTelCoPrefixID"/>
        </div>

        <div data-role="fieldcontain">

            <div class="ui-grid-a">
                <div class="ui-block-a">
                    <button id="btnOrgRepMobileCountryCode" data-role="button">Country Code</button>
                </div>

                <div class="ui-block-b">
                    <button id="btnOrgRepMobileTelCoPrefixCode" data-role="button">Telco Code</button>
                </div>
            </div>

        </div>

        <div data-role="fieldcontain">
            <label for="txtOrgRepContactNoMobile">Mobile No.: <span style="color: #FF0000;" >*</span></label>
	       <input type="number" id="txtOrgRepContactNoMobile" maxlength="7"/>
        </div>

        <div data-role="fieldcontain">
	        <label for="selOrgRepContactTypeMobile" class="select">Contact Type: <span style="color: #FF0000;" >*</span></label>
            <select id="selOrgRepContactTypeMobile">
	            <option value="">Please Select</option>
	            <option value="Work Mobile">Work Mobile</option>
				<option value="Personal Mobile">Personal Mobile</option>
				<option value="Third Party Mobile">Third Party Mobile</option>
            </select>
        </div>

        <div>
            <br />
            <br />
            <div style="text-align: center">
                <a id="btnOrgRepContactMobileSave" data-role="button"><h3>Ok</h3></a>
            </div>
            <div style="text-align: center">
                <a id="btnOrgRepContactMobileRemove" data-role="button"><h3>Remove</h3></a>
            </div>
            <div style="text-align: center">
                <a id="btnOrgRepContactMobileCancel" data-role="button"><h3>Cancel</h3></a>
            </div>
        </div>

    </div>

</div>

<div id="pageOrgRepContactEmail" style="display:none;">

    <div data-role="header">
        <h2>Contact Email Info</h2>
        @*<a href="#" data-icon="arrow-l" data-rel="back">Back</a>*@
    </div>

    <div data-role="content">

        <div data-role="fieldcontain" style="display:none;">
	        <input type="text" readonly id="txtOrgRepContactEmailID"/>                
        </div>

        <div data-role="fieldcontain">
            <label for="txtOrgRepContactEmail">Email Address: <span style="color: #FF0000;" >*</span></label>
	        <input type="text" id="txtOrgRepContactEmail"/>
        </div>

        <div data-role="fieldcontain">
	        <label for="selOrgRepContactTypeEmail" class="select">Email Type: <span style="color: #FF0000;" >*</span></label>
            <select id="selOrgRepContactTypeEmail">
	            <option value="">Please Select</option>
	            <option value="Work Email">Work Email</option>
				<option value="Personal Email">Personal Email</option>
				<option value="Third Party Email">Third Party Email</option>
            </select>
        </div>

        <div>
            <br />
            <br />
            <div style="text-align: center">
                <a id="btnOrgRepContactEmailSave" data-role="button"><h3>Ok</h3></a>
            </div>
            <div style="text-align: center">
                <a id="btnOrgRepContactEmailRemove" data-role="button"><h3>Remove</h3></a>
            </div>
            <div style="text-align: center">
                <a id="btnOrgRepContactEmailCancel" data-role="button"><h3>Cancel</h3></a>
            </div>
        </div>

    </div>
</div>

