@Code
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head

    <script>
        require([GetPathFormat('/Scripts/sources/People/PeoplePrimeIDReference.js?crc=')]);
    </script>
    
End Section

<div id="pagePeoplePrimeIDReference" data-role="page">

    <div data-role="header">
        <a class="btnProfileBackToHome" data-icon="home">Home</a>
        <a id="btnPeoplePrimeIDPopUpMenu" data-rel="popup" data-inline="true" class="ui-btn-right">Go To</a>
        <h1>Person Primary ID Reference</h1>
    </div>

    <div data-role="content" >

        <div data-role="fieldcontain" style="display:none;">
	        <input type="text" readonly id="txtID"/>                
        </div>

        <div data-role="fieldcontain">
	        <label for="btnResidenceCertNo">Residence Certification No.:</label>
            <button id="btnResidenceCertNo" data-role="button">Please Input</button>
        </div>

        <div data-role="fieldcontain">
	        <label for="txtSSSNo">SSS No.:</label>
            <input type="text" id="txtSSSNo"/>  
        </div>

        <div data-role="fieldcontain">
	        <label for="txtPhilHealthNo">Phil. Health No.:</label>
            <input type="text" id="txtPhilHealthNo"/>    
        </div>

        <div data-role="fieldcontain">
	        <label for="txtTIN">TIN:</label>
            <input type="text" id="txtTIN"/>
        </div>

        <div data-role="fieldcontain">
	        <label for="txtHDMFNo">HDMF No.:</label>
            <input type="text" id="txtHDMFNo"/>
        </div>

        <div data-role="fieldcontain">
	        <label for="txtNBINo">NBI No.:</label>
            <input type="text" id="txtNBINo"/>
        </div>

        <div data-role="fieldcontain">
	        <label for="btnPassportNo">Passport No.:</label>
            <button id="btnPassportNo" data-role="button">Please Input</button>
   
        </div>

        <div>
            <br />
            <br />
	        <div style="text-align:center"> <a id="btnPrimeIDDone" data-role="button">Done</a> </div>
        </div>

        <div id="divPeoplePrimeIDPopUpMenu" data-role="popup">
            <ul data-role="listview" data-inset="true" style="min-width: 210px;">
                <li data-role="divider">Menu</li>
                <li><a id="popPeoplePrimeIDPeopleInfoName">Name</a></li>
                <li><a id="popPeoplePrimeIDPeopleInfoAlias">Alias</a></li>
                <li><a id="popPeoplePrimeIDPeopleInfoAddress">Address</a></li>
                <li><a id="popPeoplePrimeIDPeoplePersonalInfo">Personal Information</a></li>
                <li><a id="popPeoplePrimeIDPeopleContact">Contact list</a></li>
                <li><a id="popPeoplePrimeIDPeopleEducation">Educational Background</a></li>
                <li><a id="popPeoplePrimeIDPeopleEmpHistory">Employment History</a></li>
                <li><a id="popPeoplePrimeIDPeopleRelatedPerson">Related Person</a></li>
                <li><a id="popPeoplePrimeIDPeopleUserAccount">User Account</a></li>
            </ul>
        </div>

    </div>

</div>

<div id="pagePeopleResCertDetails" data-role="page">
    <div data-role="header">
        <h1>Residence Certification Details</h1>
    </div>

    <div data-role="content">

        <div data-role="fieldcontain">
	        <label for="txtResCertNo">Certification No.: <span style="color: #FF0000;" >*</span></label>
	        <input type="text" id="txtResCertNo"/>         
        </div>

        <div data-role="fieldcontain">
	        <label for="btnResCertIssuedAt">Issued At: <span style="color: #FF0000;" >*</span></label>
            <div style="display:none;"><input type="text" readonly id="txtResCertIssuedAtID"/></div>
            <button id="btnResCertIssuedAt" data-role="button">Please Select</button>
        </div>
        
        <div data-role="fieldcontain">
	          <label for="selResCertMonth">Issued On: <span style="color: #FF0000;" >*</span></label>
              <fieldset data-role="controlgroup" data-type="horizontal">
                    <select id="selResCertMonth">
	                    <option value="">MMM</option>
                    </select>
                    <select id="selResCertDay">
	                    <option value="">DD</option>
                    </select>                
                    <select id="selResCertYear">
	                    <option value="">YYYY</option>
                    </select>                
            
            </fieldset>
        </div>

        <div>
            <br />
            <br />
	        <div style="text-align:center"><a id="btnResCertOk" data-role="button">Ok</a></div>
	        <div style="text-align:center"> <a id="btnResCertCancel" data-role="button">Cancel</a> </div>
        </div>

    </div>

</div>


<div id="pagePeoplePassportDetails" data-role="page">
    <div data-role="header">
        <h1>Passport Details</h1>
    </div>

    <div data-role="content">

        <div data-role="fieldcontain">
	        <label for="txtPassportNo">Passport No.: <span style="color: #FF0000;" >*</span></label>
	        <input type="text" id="txtPassportNo"/>          
        </div>

        <div data-role="fieldcontain">
	        <div><label for="btnPassportIssuedAt">Issued At: <span style="color: #FF0000;" >*</span></label></div>
            <div style="display:none;"><input type="text" readonly id="txtPassportIssuedAtID"/></div>
            <fieldset data-role="controlgroup">
                <button id="btnPassportIssuedAt" data-role="button">Please Select</button>
            </fieldset>
        </div>
        
        <div data-role="fieldcontain">
            
	            <label for="selPassportMonth">Issued On: <span style="color: #FF0000;" >*</span></label>
                <fieldset data-role="controlgroup" data-type="horizontal">
                    <select id="selPassportMonth">
	                    <option value="">MMM</option>
                    </select>
                    <select id="selPassportDay">
	                    <option value="">DD</option>
                    </select>                
                    <select id="selPassportYear">
	                    <option value="">YYYY</option>
                    </select>                          
            </fieldset>
        </div>

        <div data-role="fieldcontain">
	            <label for="selPassportExpMonth">Expiration Date: <span style="color: #FF0000;" >*</span></label>
                <fieldset data-role="controlgroup" data-type="horizontal">
                    <select id="selPassportExpMonth">
	                    <option value="">MMM</option>
                    </select>
                    <select id="selPassportExpDay">
	                    <option value="">DD</option>
                    </select>                
                    <select id="selPassportExpYear">
	                    <option value="">YYYY</option>
                    </select>                        
            </fieldset>
        </div>

        <div>
            <br />
            <br />
	        <div style="text-align:center"><a id="btnPassportOk" data-role="button">Ok</a></div>
	        <div style="text-align:center"> <a id="btnPassportCancel" data-role="button">Cancel</a> </div>
        </div>

    </div>

</div>
