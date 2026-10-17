@Code
    ViewData("Title") = "Login"
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head

    <script>
        require([GetPathFormat('/Scripts/sources/People/PeopleLogger.js?crc=')]);
    </script>
    
End Section

<div id="pagePeopleMenu" data-role="page">
    <div data-role="header">
        <h1>@ViewData("Title")</h1>
    </div>

    <div data-role="content">

        <div style="text-align: center; margin: auto">
            <div><img src="/Content/images/Mobiz.png" /></div>
            <span style="font-style:italic; font-size:18px">Business-Grade Apps In Your Pocket</span>
        </div>
        
        <form id="frmPeopleLogger">
            <div data-role="fieldcontain">
	            <label for="txtUsername">Username: </label>
	            <input type="text" id="txtUsername" autofocus/>
            </div>

            <div data-role="fieldcontain">
	            <label for="txtPassword">Passsword: </label>
	            <input type="password" id="txtPassword"/>
            </div>
        <div><input id="btnSignIn" type="submit" value="Sign In"/></div>
        </form>

        <br />
        <br />
        <br />
	    <div style="text-align:center;"><legend>Don't have a Mobiz Personal Profile yet?</legend></div>
        <br />
        <div style="text-align:center;"><a id="btnSignUp" style="cursor:pointer;">Create a Mobiz Personal Profile</a></div>
        <br />
        <div style="text-align:center;"><a id="btnForgotPassword" style="cursor:pointer;">I can't access my Mobiz Personal Profile.</a></div>

    </div>

</div>

<div id="pagePeopleName" data-role="page">

    <div data-role="header">
        <h1>People Name</h1>
    </div>

    <div data-role="content">

        <div data-role="fieldcontain">
            <label for="txtSurname">Surname (Last Name): <span style="color: #FF0000;" >*</span></label>
            <input type="text" id="txtSurname"/>
            </fieldset>
        </div>

        <div data-role="fieldcontain">
            <label for="txtFirstName">First Name: <span style="color: #FF0000;" >*</span></label>
            <input type="text" id="txtFirstName"/>
        </div>

        <div data-role="fieldcontain">
            <label for="txtMiddleName">Full Middle Name: <span style="color: #FF0000;" >*</span></label>
            <input type="text" id="txtMiddleName"/>
        </div>

        <div data-role="fieldcontain">
            <fieldset data-role="controlgroup" data-type="horizontal">
	            <div><label>Date Of Birth: <span style="color: #FF0000;" >*</span></label></div>
                <div>
                    <select id="selDOBMonth">
	                    <option value="">MMM</option>
                    </select>
                    <select id="selDOBDay">
	                    <option value="">DD</option>
                    </select>                
                    <select id="selDOBYear">
	                    <option value="">YYYY</option>
                    </select>                
                 </div>            
            </fieldset>
        </div>

        <div>
            <ul data-role="listview" data-inset="true">
			    <li data-role="list-divider">Mother's Maiden Name</li>
				<li>
                    <div data-role="fieldcontain">
                        <label for="txtMotherMaidenSurname">Mother's Maiden Surname (Last Name): <span style="color: #FF0000;" >*</span></label>
                        <input type="text" id="txtMotherMaidenSurname"/>
                    </div>

                    <div data-role="fieldcontain">
                        <label for="txtMotherMaidenFirstName">Mother's First Name: <span style="color: #FF0000;" >*</span></label>
                        <input type="text" id="txtMotherMaidenFirstName"/>
                    </div>

                    <div data-role="fieldcontain">
                        <label for="txtMotherMaidenMiddleName">Mother's Maiden Middle Name:</label>
                        <input type="text" id="txtMotherMaidenMiddleName"/>
                    </div>
				</li>

			</ul>
        </div>

        <div>
            <br />
            <br />
	        <div style="text-align:center"> <a id="btnPeopleNameOk" data-role="button">Next</a> </div>
	        <div style="text-align:center"> <a id="btnPeopleNameCancel" data-role="button">Back</a> </div>
        </div>

    </div>

</div>

<div id="pageSecurityQuestion" data-role="page">

    <div data-role="header">
        <h1>People Name</h1>
    </div>

    <div data-role="content">

	    <div style="display:none;"><input type="text" id="txtPeopleID"/></div>
        <div>
            <ul data-role="listview" data-inset="true">
			    <li id="lstQ1Divider" data-role="list-divider" style="display:none;">Question No. 1</li>
				<li id="lstQ1" style="display:none;">
                    <div data-role="fieldcontain">
                        <fieldset data-role="controlgroup">
	                        <div><input type="text" id="txtQ1ID" style="display:none;"/></div>
	                        <div><label id="lblQuestion1" for="txtQuestion1Answer">Quaestion 1 Goes Here? </label></div>
	                        <div><input type="text" id="txtQuestion1Answer"/></div>  
                        </fieldset>
                    </div>
				</li>
			    <li id="lstQ2Divider" data-role="list-divider" style="display:none;">Question No. 2</li>
				<li id="lstQ2" style="display:none;">
                    <div data-role="fieldcontain">
                        <fieldset data-role="controlgroup">
	                        <div><input type="text" id="txtQ2ID" style="display:none;"/></div>
	                        <div><label id="lblQuestion2" for="txtQuestion2Answer">Quaestion 2 Goes Here? </label></div>
	                        <div><input type="text" id="txtQuestion2Answer"/></div>  
                        </fieldset>
                    </div>
				</li>
			</ul>
        </div>

        <div>
            <br />
            <br />
	        <div style="text-align:center"> <a id="btnSecurityOk" data-role="button">Done</a> </div>
	        <div style="text-align:center"> <a id="btnSecurityCancel" data-role="button">Back</a> </div>
        </div>

    </div>

</div>

<div id="pageSetUserAccount" data-role="page">

    <div data-role="header">
        <h1>User Account</h1>
    </div>

    <div data-role="content">

        <div data-role="fieldcontain">
            <label for="txtSetUsername">Username: <span style="color: #FF0000;" >*</span></label>
            <input type="text" id="txtSetUsername" maxlength="25"/>
        </div>

        <div data-role="fieldcontain">
            <label for="txtSetPassword">Password: <span style="color: #FF0000;" >*</span></label>
            <input type="password" id="txtSetPassword"/ maxlength="25">
        </div>

        <div data-role="fieldcontain">
            <label for="txtSetConfirmPassword">Confirm Password: <span style="color: #FF0000;" >*</span></label>
            <input type="password" id="txtSetConfirmPassword" maxlength="25"/>
        </div>

        <div>
            <br />
            <br />
	        <div style="text-align:center"> <a id="btnUserAccountOk" data-role="button">Next</a> </div>
	        <div style="text-align:center"> <a id="btnUserAccountCancel" data-role="button">Back</a> </div>
        </div>

    </div>

</div>

<div id="pageSetSecurityQuestion" data-role="page">

    <div data-role="header">
        <h1>Security Question</h1>
    </div>
    
    <div data-role="content">

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
            <label for="txtQ1Answer">Answer: <span style="color: #FF0000;" >*</span></label>
            <input type="text" id="txtQ1Answer"/>
        </div>

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
            <label for="txtQ2Answer">Answer: <span style="color: #FF0000;" >*</span></label>
            <input type="text" id="txtQ2Answer"/>
        </div>
        <div>
            <br />
            <br />
	        <div style="text-align:center"> <a id="btnSetSecurityOk" data-role="button">Done</a> </div>
	        <div style="text-align:center"> <a id="btnSetSecurityCancel" data-role="button">Back</a> </div>
        </div>

    </div>

</div>
