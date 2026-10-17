@Code
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head

    <script>
        require([GetPathFormat('/Scripts/sources/People/PeopleContactNo.js?crc=')]);
    </script>
    
End Section

<div id="pagePeopleContactList" data-role="page">
    <div data-role="header">
        <a class="btnProfileBackToHome" data-icon="home">Home</a>
        <a id="btnPeopleContactPopUpMenu" data-rel="popup" data-inline="true" class="ui-btn-right">Go To</a>
        <h1>Person Contact List</h1>
    </div>

    <div data-role="content">

        <ul data-role="listview" data-inset="true" style="min-width: 210px;">
            <li data-role="list-divider">Telephone</li>
            <li>
                <div data-role="fieldcontain"><a id="btnPeopleContactNoTelAdd" data-role="button" data-icon="plus" data-inline="true">Add Telephone Contact</a></div>
                <div data-role="fieldcontain" id="divRecordCountTel"></div>
                <div>
                    <ul data-role="listview" id="lstPeopleContactTel" data-inset="true">
                    </ul>
                </div>
            </li>

            <li data-role="list-divider">Mobile</li>
            <li>
                <div data-role="fieldcontain"><a id="btnPeopleContactNoMobAdd" data-role="button" data-icon="plus" data-inline="true">Add Mobile Contact</a></div>
                <div data-role="fieldcontain" id="divRecordCountMob"></div>
                <div>
                    <ul data-role="listview" id="lstPeopleContactMob" data-inset="true">
                    </ul>
                </div>
            </li>

            <li data-role="list-divider">Email</li>
            <li>
                <div data-role="fieldcontain"><a id="btnPeopleContactEmailAdd" data-role="button" data-icon="plus" data-inline="true">Add Email</a></div>
                <div data-role="fieldcontain" id="divRecordCountEmail"></div>
                <div>
                    <ul data-role="listview" id="lstPeopleContactEmail" data-inset="true">
                    </ul>
                </div>
            </li>
        </ul>

        <div>
            <br />
            <br />
	        <div style="text-align:center"> <a id="btnPeopleContactDone" data-role="button">Done</a> </div>
        </div>

        <div id="divPeopleContactPopUpMenu" data-role="popup">
            <ul data-role="listview" data-inset="true" style="min-width: 210px;">
                <li data-role="divider">Menu</li>
                <li><a id="popPeopleContactPeopleInfoName">Name</a></li>
                <li><a id="popPeopleContactPeopleInfoAlias">Alias</a></li>
                <li><a id="popPeopleContactPeopleInfoAddress">Address</a></li>
                <li><a id="popPeopleContactPeoplePersonalInfo">Personal Information</a></li>
                <li><a id="popPeopleContactPeopleEducation">Educational Background</a></li>
                <li><a id="popPeopleContactPeopleEmpHistory">Employment History</a></li>
                <li><a id="popPeopleContactPeopleRelatedPerson">Related Person</a></li>
                <li><a id="popPeopleContactPeoplePrimeIDReference">Primary ID Reference</a></li>
                <li><a id="popPeopleContactPeopleUserAccount">User Account</a></li>
            </ul>
        </div>

    </div>

</div>

<div id="pagePeopleContactTelInfo" data-role="page">

    <div data-role="header">
        <h1>Contact Telephone Info</h1>
    </div>

    <div data-role="content">

        <div data-role="fieldcontain" style="display:none;">
	        <input type="text" readonly id="txtContactTelID"/>                
        </div>

        <div data-role="fieldcontain">

            <div style="display:none;">
                <input type="text" readonly id="txtTelCountryID"/>
                <input type="text" readonly id="txtTelAreaCodeID"/>
            </div>

            <div class="ui-grid-a">
                <div class="ui-block-a">
                    <button id="btnTelCountryCode" data-role="button">Country Code</button>
                </div>

                <div class="ui-block-b">
                    <button id="btnTelAreaCode" data-role="button">Area Code</button>
                </div>
            </div>

        </div>

        <div data-role="fieldcontain">
            <label for="txtContactNoTel">Telephone No.: <span style="color: #FF0000;" >*</span></label>
            <input type="number" id="txtContactNoTel" maxlength="7"/>
        </div>

        <div data-role="fieldcontain">
            <label for="txtContactExtension">Extension or Local No.:</label>
            <input type="number" id="txtContactExtension" maxlength="10"/>
        </div>

        <div data-role="fieldcontain">
	        <label for="selContactTypeTel" class="select">Contact Type: <span style="color: #FF0000;" >*</span></label>
            <select id="selContactTypeTel">
	            <option value="">Please Select</option>
	            <option value="Work Telephone">Work Telephone</option>
				<option value="Personal Telephone">Personal Telephone</option>
				<option value="Third Party Telephone">Third Party Telephone</option>
            </select>
        </div>

        <div>  
            <br />
            <br />
	        <div style="text-align:center"> <a id="btnPeopleContactTelSave" data-role="button">Ok</a> </div>
	        <div style="text-align:center"> <a id="btnPeopleContactTelRemove" data-role="button">Remove</a> </div>
	        <div style="text-align:center"> <a id="btnPeopleContactTelCancel" data-role="button">Cancel</a> </div>
        </div>

    </div>

</div>

<div id="pagePeopleContactMobileInfo" data-role="page">

    <div data-role="header">
        <h1>Contact Mobile Info</h1>
    </div>

    <div data-role="content">

        <div data-role="fieldcontain" style="display:none;">
	        <input type="text" readonly id="txtContactMobileID"/>                
        </div>

        <div data-role="fieldcontain">

            <div style="display:none;">
                <input type="text" readonly id="txtMobileCountryID"/>
                <input type="text" readonly id="txtTelCoPrefixID"/>
            </div>

            <div class="ui-grid-a">
                <div class="ui-block-a">
                    <button id="btnMobileCountryCode" data-role="button">Country Code</button>
                </div>

                <div class="ui-block-b">
                    <button id="btnMobileTelCoPrefixCode" data-role="button">Telco Code</button>
                </div>
            </div>

        </div>

        <div data-role="fieldcontain">
            <label for="txtContactNoMobile">Mobile No.: <span style="color: #FF0000;" >*</span></label>
            <input type="number" id="txtContactNoMobile" maxlength="7"/>
        </div>

        <div data-role="fieldcontain">
	        <label for="selContactTypeMobile" class="select">Contact Type: <span style="color: #FF0000;" >*</span></label>
            <select id="selContactTypeMobile">
	            <option value="">Please Select</option>
	            <option value="Work Mobile">Work Mobile</option>
				<option value="Personal Mobile">Personal Mobile</option>
				<option value="Third Party Mobile">Third Party Mobile</option>
            </select>
        </div>

        <div>  
            <br />
            <br />
	        <div style="text-align:center"> <a id="btnPeopleContactMobileSave" data-role="button">Ok</a> </div>
	        <div style="text-align:center"> <a id="btnPeopleContactMobileRemove" data-role="button">Remove</a> </div>
	        <div style="text-align:center"> <a id="btnPeopleContactMobileCancel" data-role="button">Cancel</a> </div>
        </div>

    </div>

</div>

<div id="pagePeopleContactMobileEmail" data-role="page">

    <div data-role="header">
        <h1>Contact Email Info</h1>
    </div>

    <div data-role="content">

        <div data-role="fieldcontain" style="display:none;">
	        <input type="text" readonly id="txtContactEmailID"/>                
        </div>

        <div data-role="fieldcontain">
            <label for="txtContactEmail">Email Address: <span style="color: #FF0000;" >*</span></label>
            <input type="text" id="txtContactEmail"/>
        </div>

        <div data-role="fieldcontain">
	        <label for="selContactTypeEmail" class="select">Email Type: <span style="color: #FF0000;" >*</span></label>
            <select id="selContactTypeEmail">
	            <option value="">Please Select</option>
	            <option value="Work Email">Work Email</option>
				<option value="Personal Email">Personal Email</option>
				<option value="Third Party Email">Third Party Email</option>
            </select>
        </div>

        <div>  
            <br />
            <br />
	        <div style="text-align:center"> <a id="btnPeopleContactEmailSave" data-role="button">Ok</a> </div>
	        <div style="text-align:center"> <a id="btnPeopleContactEmailRemove" data-role="button">Remove</a> </div>
	        <div style="text-align:center"> <a id="btnPeopleContactEmailCancel" data-role="button">Cancel</a> </div>
        </div>

    </div>

</div>
