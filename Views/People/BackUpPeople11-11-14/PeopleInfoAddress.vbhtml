@Code
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head

    <script>
        require([GetPathFormat('/Scripts/sources/People/PeopleInfoAddress.js?crc=')]);
    </script>
    
End Section

<div id="pagePeopleAddressList" data-role="page">
    <div data-role="header">
        <a id="btnPeopleAddressPopUpMenu" data-rel="popup" data-inline="true" class="ui-btn-right">Go To</a>
        <h1>Person Address List</h1>
    </div>

    <div data-role="content">
        <div data-role="fieldcontain"><a id="btnPeopleAddressAdd" data-role="button" data-icon="plus" data-inline="true">Add Address</a></div>
        <div data-role="fieldcontain" id="divRecordCount"></div>
        <div>
            <ul data-role="listview" id="lstPeopleAddress" data-inset="true">
            </ul>
        </div>
        <div>
            <br />
            <br />
	        <div style="text-align:center"> <a id="btnPeopleAddressDone" data-role="button">Done</a> </div>
        </div>

        <div id="divPeopleAddressPopUpMenu" data-role="popup">
            <ul data-role="listview" data-inset="true" style="min-width: 210px;">
                <li data-role="divider">Menu</li>
                <li><a id="popPeopleAddressPeopleInfoName">Name</a></li>
                <li><a id="popPeopleAddressPeopleInfoAlias">Alias</a></li>
                <li><a id="popPeopleAddressPeoplePersonalInfo">Personal Information</a></li>
                <li><a id="popPeopleAddressPeopleContact">Contact List</a></li>
                <li><a id="popPeopleAddressPeopleEducation">Educational Background</a></li>
                <li><a id="popPeopleAddressPeopleEmpHistory">Employment History</a></li>
                <li><a id="popPeopleAddressPeopleRelatedPerson">Related Person</a></li>
                <li><a id="popPeopleAddressPeoplePrimeIDReference">Primary ID Reference</a></li>
                <li><a id="popPeopleAddressPeopleUserAccount">User Account</a></li>
            </ul>
        </div>

    </div>

</div>

<div id="pagePeopleAddressName" data-role="page">
    <div data-role="header">
        <h1>Person Address</h1>
    </div>

    <div data-role="content">

        <div data-role="fieldcontain" style="display:none;">
	        <input type="text" readonly id="txtAddressID"/>                
        </div>

        <div data-role="fieldcontain">
            <label for="txtHouseBuildingNo">House or Building No.:</label>
            <input type="text" id="txtHouseBuildingNo"/>
        </div>

        <div data-role="fieldcontain">
            <label for="txtStreet">Street: <span style="color: #FF0000;" >*</span></label>
            <input type="text" id="txtStreet"/>
        </div>

        <div data-role="fieldcontain">
	        <div><label for="btnCity">City or Town: <span style="color: #FF0000;" >*</span></label></div>
            <div style="display:none;"><input type="text" readonly id="txtCityID"/></div>
            <button id="btnCity" data-role="button">Please Select</button>
        </div>

        <div data-role="fieldcontain">
	        <div><label for="btnCountry">Country: <span style="color: #FF0000;" >*</span></label></div>
            <div style="display:none;"><input type="text" readonly id="txtCountryID"/></div>
            <button id="btnCountry" data-role="button">Please Select</button>
        </div>

        <div data-role="fieldcontain">
	        <label for="selTypeOfAddress" class="select">Type Of Address: <span style="color: #FF0000;" >*</span></label>
            <select id="selTypeOfAddress">
	            <option value="">Please Select</option>
	            <option value="Permanent Address">Permanent Address</option>
				<option value="Non-Permanent Address">Non-Permanent Address</option>
				<option value="Provincial Address">Provincial Address</option>
            </select>
        </div>

        <div>
            <br />
            <br />
	        <div style="text-align:center"><a id="btnPeopleAddressSave" data-role="button">Ok</a></div>
	        <div style="text-align:center"><a id="btnPeopleAddressRemove" data-role="button">Remove</a></div>
	        <div style="text-align:center"><a id="btnPeopleAddressCancel" data-role="button">Cancel</a> </div>
        </div>

    </div>

</div>
