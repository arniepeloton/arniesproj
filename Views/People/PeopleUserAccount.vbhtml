@Code
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head

    <script>
        require([GetPathFormat('/Scripts/sources/People/PeopleUserAccount.js?crc=')]);
    </script>

End Section

<div id="pagePeopleUserAccount" data-role="page">

    <div data-role="header">
        <a class="btnProfileBackToHome" data-icon="home">Home</a>
        <a id="btnPeopleUserAccountPopUpMenu" data-rel="popup" data-inline="true" class="ui-btn-right">Go To</a>
        <h1>Person User Account</h1>
    </div>

    <div data-role="content" >

        <div data-role="fieldcontain" style="display:none;">
	        <input type="text" readonly id="txtPeopleID"/>                
        </div>

        <div>
            <ul data-role="listview" data-inset="true">
			    <li data-role="list-divider"">User Account</li>
				<li>                    
                    <div data-role="fieldcontain">
                        <label for="txtSetUsername">New Username: <span style="color: #FF0000;" >*</span></label>
                        <input type="text" id="txtSetUsername" maxlength="25"/>
                    </div>

                    <div data-role="fieldcontain">
                        <label for="txtSetPassword">New Password: <span style="color: #FF0000;" >*</span></label>
                        <input type="password" id="txtSetPassword"/ maxlength="25">
                    </div>

                    <div data-role="fieldcontain">
                        <label for="txtSetConfirmPassword">Confirm New Password: <span style="color: #FF0000;" >*</span></label>
                        <input type="password" id="txtSetConfirmPassword" maxlength="25"/>
                    </div>
				</li>

			    <li data-role="list-divider"">Security Question</li>
				<li>
	                <div style="display:none;"><input type="text" readonly id="txtQ1ID"/></div>
                    <div data-role="fieldcontain">
	                    <label for="selQuestion1" class="select">Question No. 1:</label>
                        <select id="selQuestion1">
	                        <option value="">Please Select</option>
				            <option value="Where is your mother's birthplace?">Where is your mother's birthplace?</option>
				            <option value="Who is your best childhood friend?">Who is your best childhood friend?</option>
				            <option value="What is the name of your first pet?">What is the name of your first pet?</option>
				            <option value="Who is your favorite teacher?">Who is your favorite teacher?</option>
				            <option value="Who is your favorite historical person?">Who is your favorite historical person?</option>
	                        <option value="What is your grandfather's occupation?">What is your grandfather's occupation?</option>
                        </select>
                    </div>
                    <div data-role="fieldcontain">
                        <label for="txtQ1Answer">Answer:</label>
                        <input type="text" id="txtQ1Answer"/>
                    </div>

	                <div style="display:none;"><input type="text" readonly id="txtQ2ID"/></div>
                    <div data-role="fieldcontain">
	                    <label for="selQuestion2" class="select">Question No. 2:</label>
                        <select id="selQuestion2">
	                        <option value="">Please Select</option>
				            <option value="Where is your mother's birthplace?">Where is your mother's birthplace?</option>
				            <option value="Who is your best childhood friend?">Who is your best childhood friend?</option>
				            <option value="What is the name of your first pet?">What is the name of your first pet?</option>
				            <option value="Who is your favorite teacher?">Who is your favorite teacher?</option>
				            <option value="Who is your favorite historical person?">Who is your favorite historical person?</option>
	                        <option value="What is your grandfather's occupation?">What is your grandfather's occupation?</option>
                        </select>
                    </div>
                    <div data-role="fieldcontain">
                        <label for="txtQ2Answer">Answer:</label>
                        <input type="text" id="txtQ2Answer"/>
                    </div>
                </li>

			</ul>
        </div>

        <div>
            <br />
            <br />
	        <div style="text-align:center"> <a id="btnUserAccountDone" data-role="button">Done</a> </div>
	        <div style="text-align:center"> <a id="btnUserAccountCancel" data-role="button">Back</a> </div>
        </div>

        <div id="divPeopleUserAccountPopUpMenu" data-role="popup">
            <ul data-role="listview" data-inset="true" style="min-width: 210px;">
                <li data-role="divider">Menu</li>
                <li><a id="popPeopleUserAccountPeopleInfoName">Name</a></li>
                <li><a id="popPeopleUserAccountPeopleInfoAlias">Alias</a></li>
                <li><a id="popPeopleUserAccountPeopleInfoAddress">Address</a></li>
                <li><a id="popPeopleUserAccountPeoplePersonalInfo">Personal Information</a></li>
                <li><a id="popPeopleUserAccountPeopleContact">Contact List</a></li>
                <li><a id="popPeopleUserAccountPeopleEducation">Educational Background</a></li>
                <li><a id="popPeopleUserAccountPeopleEmpHistory">Employment History</a></li>
                <li><a id="popPeopleUserAccountPeopleRelatedPerson">Related Person</a></li>
                <li><a id="popPeopleUserAccountPeoplePrimeIDReference">Primary ID Reference</a></li>
            </ul>
        </div>

    </div>

</div>

<div id="pagePeopleUserAccountConfirm" data-role="page">
    <div data-role="header">
        <h1>Confirmation Message</h1>
    </div>

    <div data-role="content">
        You've made changes to your account. Please enter your current username and password to confirm.

        <br /><br />

        <form id="formUserAccountConfirm" method="post">
            <div data-role="fieldcontain">
                <label for="txtUsername">Username: <span style="color: #FF0000;">*</span></label>
                <input type="text" id="txtUsername" maxlength="25" />
            </div>

            <div data-role="fieldcontain">
                <label for="txtPassword">Password: <span style="color: #FF0000;">*</span></label>
                <input type="password" id="txtPassword" maxlength="25" />
            </div>

            <div>
                <br />
                <br />
                <button type="submit" data-role="button">Save</button>
                <button id="btnPeopleConflictNo" data-role="button">Cancel</button>
            </div>
        </form>       

    </div>

</div>