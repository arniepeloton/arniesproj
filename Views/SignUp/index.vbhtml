@Code
    ViewData("Title") = "Sign Up"
    Layout = "~/Views/Shared/_Layout.vbhtml"
End Code
<script data-main="/Scripts3/common/init" src="/Scripts3/others/require.js"></script>
    <script>

        function GetWindowPathFormat(vString) {
            // alert(vString + '. Please use GetPathFormat');
            // change GetWindowPathFormat to GetPathFormat
            // debugger;
            console.log('change GetWindowPathFormat to GetPathFormat: ' + vString);
            return GetPathFormat(vString);
        };

        function GetPathFormat(vString) {

            if (window.location.protocol.substring(0, 4) != 'file')
                return vString;

            function parseUrl(url) {
                // http://stackoverflow.com/a/6644749/334045
                var a = document.createElement('a');
                a.href = url;
                return a;
            }; // function parseUrl

            function GetExtension(vPathName) {

                var pMatch = vPathName.match(/.+\.([\w]+)$/im);
                if (pMatch != null)
                    return pMatch[1];
                else
                    return '';

            }; // function GetExtension

            var pCurrent = window.location.pathname.toLowerCase();
            var pIndex = pCurrent.indexOf('www/') + 4;
            var pRoot = pCurrent.substring(0, pIndex); // path to www
            var pIndex = pCurrent.lastIndexOf('/') + 1; // to folder

            if (vString.substring(0, 1) == '/')
                vString = vString.substring(1); // remove leading slash

            var pParsed = parseUrl(vString);
            var pOut = pRoot + pParsed.pathname.substring(pIndex);

            if (GetExtension(pOut) == '') {
                pOut += '.html';
                pOut += pParsed.search + pParsed.hash;
                return pOut.toLowerCase();
            } else {
                pOut += pParsed.search + pParsed.hash;
                return pOut;
            };
        }; // function GetPathFormat

    </script>
    <script src="/Scripts3/common/init.js?crc="></script>

    <script>
        require([GetPathFormat('/Scripts/sources/SignUp/SignUpInfo.js?crc=' + new Date().getTime())]);
    </script>

    <style>
    .focus:focus { border: 2px solid red; }

.hoverChecked { outline:2px solid red; }
.hoverChecked:hover { outline:none; }
.hoverChecked:active { outline:none; }
 
 
.form-validation-error{
    border: 1px solid Red;
    box-shadow: 0 0 3px Red;
}


.form-validation-error:active{
    box-shadow: 0px 1px 3px rgba(0, 0, 0, 0.2) inset, 0px 0px 3px rgba(0, 0, 0, 0.2);
}
    </style>

  <div style="width:100%;">
     <div class="container">

       <div class="row">
            <div class="col-md-12">
                <p>
                    <h1 align="left" style="color:#4E4D4F">Sign Up for Your Mobiz Account</h1>
                </p>
                  <br/>
            <input type="checkbox" id="chbFirstPartySignUp" data-role="none" value="yes" name="firstPartySignUp" /><label for="chbFirstPartySignUp" data-role="none" style="font-weight: normal">&nbsp; I am filling this form on behalf of another person. Ask him/her to change username and/or passwords upon first sign in.</label>
            <br /><br />
                <form action="" id="frmSignUp" method="POST">
                <span style="font-size:small;">Fields marked with <span style="color:Red"> * </span> are required.</span>
                    <br /> <br />
                <fieldset>
                    <legend>Profile</legend>
                   
                    <div class="alert alert-danger alert-dismissible" role="alert" id="warning" style="display:none;">
                        <button type="button" class="close" data-dismiss="alert"><span aria-hidden="true">&times;</span><span class="sr-only">Close</span></button>
                        <span class="glyphicon glyphicon-warning-sign"></span>&nbsp; Please check the required fields.
                    </div>
                    <div class="form-horizontal" role="form">
                        <div class="form-group">
                            <label for="txtFirstName" class="col-sm-3 control-label" style="font-weight:500; font-size:16px; text-align:left">Full Name: <span style="color:Red">*</span></label>
                            <div class="col-sm-3">
                                <input type="text" class="form-control" id="txtFirstName" maxlength="150" placeholder="First Name" required>
                                    <span style="font-size:small; color:Red;display:none" id="spanName" >This field is required</span>
                                <!--<input type="text" id="txtName" data-regex="^[a-zA-Z\-\_\s]+$" required="" class="ui-input-text ui-body-c">-->
                            </div>
                            <div class="col-sm-3">
                                <input type="text" class="form-control" id="txtMiddleName" maxlength="150" placeholder="Middle Name" required>
                            </div>
                            <div class="col-sm-3">
                                <input type="text" class="form-control" id="txtSurname" maxlength="150" placeholder="Last Name" required>
                            </div>
                        </div>
                         <div class="form-group">
                            <label for="selPersonalSuffixName" class="col-sm-3 control-label" style="font-weight:500; font-size:16px; text-align:left">Generational Suffix: <span style="color:Red">*</span></label>
                            <div class="col-sm-3">
                                 <select class="form-control" id="selPersonalSuffixName" data-role="none" >
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
                                  <span style="font-size:small; color:Red;display:none" id="spanSuffix" >This field is required</span>
                            </div>
                    
                            <div class="col-sm-6">
                                <input type="checkbox" id="chbNoSuffix" data-role="none" value="None" name="suffix">                     
                                <label for="chbNoSuffix" data-role="none" style="font-weight:normal;font-size:16px">I don't have a generational suffix</label>
                            </div>
                        
                        </div>
                         <div class="form-group">
                            <label for="txtNickname" class="col-sm-3 control-label" style="font-weight:500; font-size:16px; text-align:left">Nickname</label>
                            <div class="col-sm-9">
                                <input type="text" class="form-control" id="txtNickname" maxlength="16">
                                <span style="font-size:small; color:Red;display:none" id="spanRegistrationNumber" >This field is required</span>
                            </div>
                        </div>
                        
                         <div class="form-group">
                            <label for="selDOBMonth" class="col-sm-3 control-label" style="font-weight:500; font-size:16px; text-align:left">Date of Birth: <span style="color:Red">*</span></label>
                            <div class="col-sm-3">
                                 <select id="selDOBMonth" class="form-control" data-role="none">
	                                    <option value="" >MMM</option>
                                    </select>
                                <span style="font-size:small; color:Red;display:none" id="spanDateOfBirth" >This field is required</span>
                                <span style="font-size:small; color:Red;display:none" id="spanDOBGreater" >Date of Birth must not be greater than the date today</span>
                                 <span style="font-size:small; color:Red;display:none" id="spanDOB18" >You must be at least 18 years old to register as a super administrator</span>
                   
                            </div>
                            <div class="col-sm-3">
                             <select id="selDOBDay" class="form-control" data-role="none" >
	                                    <option value="">DD</option>
                                    </select>      
                             </div>
                             <div class="col-sm-3">
                              <select id="selDOBYear" class="form-control" data-role="none" >
	                                    <option value="">YYYY</option>
                                    </select> 
                             </div>
                                 
                        </div>	
                        <legend>Mother's Maiden Name</legend>				
                          <div class="form-group">
                            <label for="txtMotherMaidenFirstName" class="col-sm-3 control-label" style="font-weight:500; font-size:16px; text-align:left">Mother's Maiden Name: <span style="color:Red">*</span></label>
                           <div class="col-sm-3">
                                <input type="text" class="form-control" id="txtMotherMaidenFirstName" maxlength="150" placeholder="Mother's First Name" required>
                                    <span style="font-size:small; color:Red;display:none" id="spanMotherName" >This field is required</span>
                            </div>
                            <div class="col-sm-3">
                                <input type="text" class="form-control" id="txtMotherMaidenMiddleName" maxlength="150" placeholder="Mother's Maiden Middle Name" required>
                            </div>
                            <div class="col-sm-3">
                                <input type="text" class="form-control" id="txtMotherMaidenSurname" maxlength="150" placeholder="Mother's Maiden Last Name" required>
                            </div>
                        </div>
                        <legend>Contact Details  <br /><span style="font-size:small;">Please provide active contact details.  We will use the contact details you provide here to 1.) notify you of important matters and 2.) troubleshoot your Mobiz account.</span></legend>
                        <div class="form-group">

                            <label for="selCallingCodeTel" class="col-sm-3 control-label" style="font-weight:500; font-size:16px; text-align:left">Telephone Number: </label>
                            <div class="col-sm-3">
                                 <select class="form-control" id="selCallingCodeTel" data-role="none" >
                                  <option value="" selected="selected">Please Select Country Code</option>
                                  </select>
                                <span style="font-size:small; color:Red;display:none" id="spanCountryCodeTelNo" >Please select a country code</span>
                            </div>
                            <div class="col-sm-3">
                                <select class="form-control" id="selAreaCode" data-role="none" >
                                  <option value="" selected="selected">Please Select Area Code</option>
                                  </select>
                                  <span style="font-size:small; color:Red;display:none" id="spanAreaCode" >Please select area code</span>
                            </div>
                            <div class="col-sm-3">
                               <input type="text" class="form-control" id="txtTelephone" data-regex="^[0-9]{7}$" maxlength="7" placeholder="Enter your 7-digit mobile number" required>
                                <span style="font-size:small; color:Red;display:none" id="spanTelNoValid" >Please enter a valid telephone number</span>
                            </div>
                        </div>
                        <div class="form-group">

                            <label for="selCallingCodeMobile" class="col-sm-3 control-label" style="font-weight:500; font-size:16px; text-align:left">Mobile Number: </label>
                            <div class="col-sm-3">
                                 <select class="form-control" id="selCallingCodeMobile" data-role="none" >
                                  <option value="" selected="selected">Please Select Country Code</option>
                                  </select>
                                <span style="font-size:small; color:Red;display:none" id="spanCountryCodeMobile" >Please select a country code</span>
                            </div>
                            <div class="col-sm-3">
                                <select class="form-control" id="selTelcoPrefix" data-role="none" >
                                  <option value="" selected="selected">Please Select Mobile Number Prefix</option>
                                  </select>
                                  <span style="font-size:small; color:Red;display:none" id="spanTelcoPrefix" >Please select mobile number prefix</span>
                            </div>
                            <div class="col-sm-3">
                               <input type="text" class="form-control" id="txtMobile" data-regex="^[0-9]{7}$" maxlength="7" placeholder="Enter your 7-digit mobile number" required>
                                <span style="font-size:small; color:Red;display:none" id="spanMobileValid" >Please enter a valid mobile number</span>
                            </div>
                        </div>
                         <div class="form-group">
                            <label for="txtEmail" class="col-sm-3 control-label" style="font-weight:500; font-size:16px; text-align:left">Email Address: </label>
                            <div class="col-sm-3">
                                <input type="email" class="form-control" id="txtEmail" maxlength="150" required>
                                <span style="font-size:small; color:Red;display:none" id="spanEmailReq" >This field is required</span>
                                <span style="font-size:small; color:Red;display:none" id="spanEmail" >Please enter a valid email address</span>
                            </div>
                              <div class="col-sm-6">
                               <input type="checkbox" id="chbNoEmail" data-role="none" value="NoEmail" name="email"/>           
                                <label for="chbNoEmail" data-role="none" style="font-weight:normal;font-size:16px">I don't have an email address</label>
                            </div>
                        
                       
                        </div>
                       
                              
                        <legend>User Account</legend>
                         <div class="form-group">
                            <label for="txtUserName" class="col-sm-3 control-label" style="font-weight:500; font-size:16px; text-align:left">User Name: <span style="color:Red">*</span></label>
                            <div class="col-sm-9">
                                <input type="text" class="form-control" id="txtUserName" maxlength="150" required>
                                <span style="font-size:small; color:Red;display:none" id="spanUserName" >This field is required</span>
                                 <span style="font-size:small; color:Red;display:none" id="spanUserNameChars" >User Name must be at least 8 characters long</span>
                                   <span style="font-size:small; color:Red;display:none" id="spanUserNameAlphanum" >User Name must contain only alphanumeric and underscore characters</span>
                            </div>
                        </div>
				         <div class="form-group">
                            <label for="txtPassword" class="col-sm-3 control-label" style="font-weight:500; font-size:16px; text-align:left">Password: <span style="color:Red">*</span></label>
                            <div class="col-sm-9">
                                <input type="password" class="form-control" id="txtPassword" maxlength="150" required>
                                <span style="font-size:small; color:Red;display:none" id="spanPassword" >This field is required</span>
                                 <span style="font-size:small; color:Red;display:none" id="spanPasswordChars" >Password must be at least 8 characters long</span>
                                  
                            </div>
                        </div>
                        <div class="form-group">
                            <label for="txtConfirmPassword" class="col-sm-3 control-label" style="font-weight:500; font-size:16px; text-align:left">Confirm Password: <span style="color:Red">*</span></label>
                            <div class="col-sm-9">
                                <input type="password" class="form-control" id="txtConfirmPassword" maxlength="150" required>
                                <span style="font-size:small; color:Red;display:none" id="spanConfirmPassword" >This field is required</span>
                                 <span style="font-size:small; color:Red;display:none" id="spanPasswordRepeat" >The password you entered did not match</span>
                                  
                            </div>
                        </div>
                      
                    <legend id="lblSecurity">Security Question   <br />  <span style="font-size:small;">Note: You must remember the answers to the security questions you selected. This will be used to reset your account in case you forget your password.</span></legend>
                 
                     <div class="form-group" id="secSecurity1">
                            <label for="selQuestion1" class="col-sm-3 control-label" style="font-weight:500; font-size:16px; text-align:left">Question No. 1: <span style="color:Red">*</span></label>
                            <div class="col-sm-5">
                               <select id="selQuestion1" data-role="none" class="form-control" >
                                  <option value="">Please Select</option>
				                            <option value="Where is your mother's birthplace?">Where is your mother's birthplace?</option>
				                            <option value="Who is your best childhood friend?">Who is your best childhood friend?</option>
				                            <option value="What is the name of your first pet?">What is the name of your first pet?</option>
				                            <option value="Who is your favorite teacher?">Who is your favorite teacher?</option>
				                            <option value="Who is your favorite historical person?">Who is your favorite historical person?</option>
	                                        <option value="What is your grandfather's occupation?">What is your grandfather's occupation?</option>
                                    </select>
                                         <span style="font-size:small; color:Red;display:none" id="spanAnswer1" >This field is required</span>
                                <span style="font-size:small; color:Red;display:none" id="spanQuestion1" >Please choose a different question</span>

                            </div>
                              <div class="col-sm-4">
                               
                                 <input type="text" class="form-control" id="txtQ1Answer" placeholder="Answer for question no. 1" maxlength="150" required>
                           
                            </div>
                        </div>
                         <div class="form-group" id="secSecurity2">
                            <label for="selQuestion2" class="col-sm-3 control-label" style="font-weight:500; font-size:16px; text-align:left">Question No. 2: <span style="color:Red">*</span></label>
                            <div class="col-sm-5">
                               <select id="selQuestion2" data-role="none" class="form-control" >
                                  <option value="">Please Select</option>
				                            <option value="Where is your mother's birthplace?">Where is your mother's birthplace?</option>
				                            <option value="Who is your best childhood friend?">Who is your best childhood friend?</option>
				                            <option value="What is the name of your first pet?">What is the name of your first pet?</option>
				                            <option value="Who is your favorite teacher?">Who is your favorite teacher?</option>
				                            <option value="Who is your favorite historical person?">Who is your favorite historical person?</option>
	                                        <option value="What is your grandfather's occupation?">What is your grandfather's occupation?</option>
                                    </select>
                                    <span style="font-size:small; color:Red;display:none" id="spanAnswer2" >This field is required</span>
                                <span style="font-size:small; color:Red;display:none" id="spanQuestion2" >Please choose a different question</span>

                            </div>
                              <div class="col-sm-4">
                               
                                 <input type="text" class="form-control" id="txtQ2Answer" maxlength="150" placeholder="Answer for question no. 2" required>
                                
                            </div>
                        </div>

                       <legend>Daily Time Record (DTR) PIN</legend>
                        <div class="form-group">
                            <label for="txtPIN1" class="col-sm-3 control-label" style="font-weight:500; font-size:16px; text-align:left">Enter PIN: <span style="color:Red">*</span></label>
                            <div class="col-sm-9">
                                <input type="password" class="form-control" id="txtPIN1" maxlength="5" placeholder="Enter a 5-digit PIN" required>
                                <span style="font-size:small; color:Red;display:none" id="spanPIN1" >PIN mismatched</span>
                                <span style="font-size:small; color:Red;display:none" id="spanPIN12">Enter a 5-digit pin</span>
                            </div>
                        </div>
                         <div class="form-group">
                            <label for="txtPIN2" class="col-sm-3 control-label" style="font-weight:500; font-size:16px; text-align:left">Re-enter PIN: <span style="color:Red">*</span></label>
                            <div class="col-sm-9">
                                <input type="password" class="form-control" id="txtPIN2" maxlength="5" required>
                                <span style="font-size:small; color:Red;display:none" id="spanPIN2" >PIN mismatched</span>
                            </div>
                        </div>

                         <div class="form-group">
                           <div class="col-sm-3">
                            </div>
                            <div class="col-sm-9">
                                <input type="checkbox" data-role="none" value="yes" name="certified" id="chbCertifiedCorrect">
                                <label for="chbCertifiedCorrect" data-role="none" value="yes" name="certified" data-role="none" style="font-weight:normal;font-size:16px"> I certify that the information provided is correct</label>
                            </div>
                        </div>
                         <div class="form-group">
                            <div class="col-sm-offset-3 col-sm-9">
                                <button id="btnSignUp" data-role="none" class="btn btn-primary">Save my Mobiz Account</button>
                                <br />
                                <span style="font-size:small; color:Red;display:none" id="spanAlert" >Oops! You might have missed filling in some required information.  Please check the sign up form above and type in the correct information inside data fields with an asterisk (*) sign.</span>
                            </div>
                        </div>


                                               
                        <input type="hidden" id="hdnPage" value="signUp">
                        <input type="hidden" id="txtPeopleID">
                        <input type="hidden" id="txtQ1ID">
                        <input type="hidden" id="txtQ2ID">
                        <input type="hidden" id="hdnAgree">   
                </fieldset>
                </form>	
            </div>
        </div>


        
     </div>
     <br /><br />
 </div>

