@Code
    ViewData("Title") = "Account Recovery"
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
    require([GetPathFormat('/Scripts/sources/SignUp/SignUpInfo.js?crc=')]);

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
                
                    <h1 align="left" style="color:#4E4D4F">Account Recovery</h1>
            <form action="" id="" method="POST">
            
                <div id="divFullDetails" style="border:solid 1px #aaaaaa;padding:12px">
                    <p>You can recover your password by answering "Security Questions" or via email. Please choose one method below.</p>
                    <br />
                    <fieldset>
                        <legend>OPTION 1: Reset password via “Security Questions”. This is useful when you don’t have an email address.</legend>
                        <div class="alert alert-danger alert-dismissible" role="alert" id="warning" style="display:none;">
                            <button type="button" class="close" data-dismiss="alert"><span aria-hidden="true">&times;</span><span class="sr-only">Close</span></button>
                            <span class="glyphicon glyphicon-warning-sign"></span>&nbsp; Please check the required fields.
                        </div>
                        <div class="form-horizontal" role="form">
                            <div class="form-group">
                                <label for="txtFirstName" class="col-sm-3 control-label" style="font-weight:500; font-size:16px; text-align:left">Full Name: <span style="color:Red">*</span></label>
                                <div class="col-sm-3">
                                    <input type="text" class="form-control" id="txtFirstName" maxlength="150" placeholder="First Name" required>
                                    <span style="font-size:small; color:Red;display:none" id="spanName">This field is required</span>
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
                                    <select class="form-control" id="selPersonalSuffixName" data-role="none">
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
                                    <span style="font-size:small; color:Red;display:none" id="spanSuffix">This field is required</span>
                                </div>

                                <div class="col-sm-6">
                                    <input type="checkbox" id="chbNoSuffix" data-role="none" value="None" name="suffix">
                                    <label for="chbNoSuffix" data-role="none" style="font-weight:normal;font-size:16px">I don't have a generational suffix</label>
                                </div>

                            </div>

                            <div class="form-group">
                                <label for="selDOBMonth" class="col-sm-3 control-label" style="font-weight:500; font-size:16px; text-align:left">Date of Birth: <span style="color:Red">*</span></label>
                                <div class="col-sm-3">
                                    <select id="selDOBMonth" class="form-control" data-role="none">
                                        <option value="">MMM</option>
                                    </select>
                                    <span style="font-size:small; color:Red;display:none" id="spanDateOfBirth">This field is required</span>
                                    <span style="font-size:small; color:Red;display:none" id="spanDOBGreater">Date of Birth must not be greater than the date today</span>
                                    <span style="font-size:small; color:Red;display:none" id="spanDOB18">You must be at least 18 years old to register as a super administrator</span>

                                </div>
                                <div class="col-sm-3">
                                    <select id="selDOBDay" class="form-control" data-role="none">
                                        <option value="">DD</option>
                                    </select>
                                </div>
                                <div class="col-sm-3">
                                    <select id="selDOBYear" class="form-control" data-role="none">
                                        <option value="">YYYY</option>
                                    </select>
                                </div>

                            </div>

                            <div class="form-group">
                                <label for="txtMotherMaidenFirstName" class="col-sm-3 control-label" style="font-weight:500; font-size:16px; text-align:left">Mother's Maiden Name: <span style="color:Red">*</span></label>
                                <div class="col-sm-3">
                                    <input type="text" class="form-control" id="txtMotherMaidenFirstName" maxlength="150" placeholder="Mother's First Name" required>
                                    <span style="font-size:small; color:Red;display:none" id="spanMotherName">This field is required</span>
                                </div>
                                <div class="col-sm-3">
                                    <input type="text" class="form-control" id="txtMotherMaidenMiddleName" maxlength="150" placeholder="Mother's Maiden Middle Name" required>
                                </div>
                                <div class="col-sm-3">
                                    <input type="text" class="form-control" id="txtMotherMaidenSurname" maxlength="150" placeholder="Mother's Maiden Last Name" required>
                                </div>
                            </div>

                            <div class="form-group">
                                <div class="col-sm-offset-3 col-sm-9">
                                    <button id="btnSubmit" data-role="none" class="btn btn-primary">Submit</button>
                                    <br />
                                    <span style="font-size:small; color:Red;display:none" id="spanAlert">Oops! You might have missed filling in some required information.  Please check the sign up form above and type in the correct information inside data fields with an asterisk (*) sign.</span>
                                </div>
                            </div>
                        </div>

                    </fieldset>

                </div>	

                <div id="divSecurityQuestion" style="display:none;">
                    <br />
                     <fieldset>
                            <legend>Security Question</legend>
                            <div class="alert alert-danger alert-dismissible" role="alert" id="warning" style="display:none;">
                                <button type="button" class="close" data-dismiss="alert"><span aria-hidden="true">&times;</span><span class="sr-only">Close</span></button>
                                <span class="glyphicon glyphicon-warning-sign"></span>&nbsp; Please check the required fields.
                            </div>
   <div class="form-horizontal" role="form">
                     <div class="form-group">
                            <label for="txtQ1AnswerAccountRecovery" class="col-sm-5 control-label" style="font-weight:500; font-size:16px; text-align:left"><span id="lblQuestion1"></span> <span style="color:Red">*</span></label>
                            
                              <div class="col-sm-7">
                               
                                 <input type="text" class="form-control" id="txtQ1AnswerAccountRecovery" maxlength="150" required>
                           <span style="font-size:small; color:Red;display:none" id="spanAnswer1AccountRecovery" >This field is required</span>
                            </div>
                        </div>
                        
                         <div class="form-group">
                            <label for="txtQ2AnswerAccountRecovery" class="col-sm-5 control-label" style="font-weight:500; font-size:16px; text-align:left"><span id="lblQuestion2"></span> <span style="color:Red">*</span></label>
                            
                              <div class="col-sm-7">
                               
                                 <input type="text" class="form-control" id="txtQ2AnswerAccountRecovery" maxlength="150"  required>
                                <span style="font-size:small; color:Red;display:none" id="spanAnswer2AccountRecovery" >This field is required</span>
                            </div>
                        </div>
                           
                         <div class="form-group">
                            <div class="col-sm-offset-5 col-sm-7">
                                <button id="btnSubmitSecurity" data-role="none" class="btn btn-primary">Submit</button>
                                <br />
                                
                            </div>
                        </div>

                    </div>

                       <br /><br />
                   </fieldset>
                 </div>       
                 <div id="divUserAccount" style="display:none;">
                   <br />
                      <fieldset>
                        <legend>Reset Account</legend>
                          <p id="pUserAccountMessage">Almost there! Please reset your user name and password to start using Mobiz.</p>
                        <br />
                        <div class="alert alert-danger alert-dismissible" role="alert" id="warning" style="display:none;">
                            <button type="button" class="close" data-dismiss="alert"><span aria-hidden="true">&times;</span><span class="sr-only">Close</span></button>
                            <span class="glyphicon glyphicon-warning-sign"></span>&nbsp; Please check the required fields.
                        </div>
                        <div class="form-horizontal" role="form">
                            <div id="divUserAccountUsername" class="form-group">
                                <label for="txtUserNameUsrAcct" class="col-sm-3 control-label" style="font-weight:500; font-size:16px; text-align:left;">User Name: <span style="color:Red">*</span></label>
                                <div class="col-sm-9">
                                    <input type="text" class="form-control" id="txtUserNameUsrAcct" maxlength="150" required>
                                    <span style="font-size:small; color:Red;display:none" id="spanUserNameUsrAcct">This field is required</span>
                                    <span style="font-size:small; color:Red;display:none" id="spanUserNameUsrAcctChars">User Name must be at least 8 characters long</span>
                                    <span style="font-size:small; color:Red;display:none" id="spanUserNameUsrAcctAlphanum">User Name must contain only alphanumeric and underscore characters</span>
                                </div>
                            </div>
                       
				         <div class="form-group">
                            <label for="txtPasswordUsrAcct" class="col-sm-3 control-label" style="font-weight:500; font-size:16px; text-align:left;">Password: <span style="color:Red">*</span></label>
                            <div class="col-sm-9">
                                <input type="password" class="form-control" id="txtPasswordUsrAcct" maxlength="150" required>
                                <span style="font-size:small; color:Red;display:none" id="spanPasswordUsrAcct" >This field is required</span>
                                 <span style="font-size:small; color:Red;display:none" id="spanPasswordUsrAcctChars" >Password must be at least 8 characters long</span>
                                  
                            </div>
                        </div>
                       
                        <div class="form-group">
                            <label for="txtConfirmPasswordUsrAcct" class="col-sm-3 control-label" style="font-weight:500; font-size:16px; text-align:left;">Confirm Password: <span style="color:Red">*</span></label>
                            <div class="col-sm-9">
                                <input type="password" class="form-control" id="txtConfirmPasswordUsrAcct" maxlength="150" required>
                                <span style="font-size:small; color:Red;display:none" id="spanConfirmPasswordUsrAcct" >This field is required</span>
                                 <span style="font-size:small; color:Red;display:none" id="spanPasswordRepeatUsrAcct" >The password you entered did not match</span>
                                <span style="font-size:small; color:Red;display:none" id="spanUserAccountPasswordRepeatProgress"></span>
                            </div>
                        </div>
                      
               
                             <div class="form-group">
                                <div class="col-sm-offset-3 col-sm-9">
                                    <button id="btnSubmitUsrAcct" data-role="none" class="btn btn-primary">Submit</button>
                                    <br />
                                </div>
                            </div>
                        </div>

                       </fieldset>
                    
                  </div> 
                   
                   </form> 
         
                        <input type="hidden" id="hdnPage" value="signUp">
                        <input type="hidden" id="txtPeopleID">
                        <input type="hidden" id="txtQ1ID">
                        <input type="hidden" id="txtQ2ID">
                        <input type="hidden" id="hdnAgree">     
    
            <form action="" id="" method="POST">

                <div id="divAccountRecoveryEmail" style="border:solid 1px #aaaaaa;padding:12px; margin-top:12px">
                    <br />
                    <fieldset>
                        <legend>OPTION 2: Reset password via Email.  We'll send a reset link to your email address on record.</legend>
                        <p>Please provide the email address you used during Mobiz registration</p>
                        <br />
                        <div class="alert alert-danger alert-dismissible" role="alert" id="warning" style="display:none;">
                            <button type="button" class="close" data-dismiss="alert"><span aria-hidden="true">&times;</span><span class="sr-only">Close</span></button>
                            <span class="glyphicon glyphicon-warning-sign"></span>&nbsp; Please check the required fields.
                        </div>
                        <div class="form-horizontal" role="form">
                            <div class="form-group">
                                <label for="txtAccountRecoveryEmail" class="col-sm-3 control-label" style="font-weight:500; font-size:16px; text-align:left;">Email: <span style="color:Red">*</span></label>
                                <div class="col-sm-9">
                                    <input type="text" class="form-control" id="txtAccountRecoveryEmail" maxlength="150" required>
                                    <span style="font-size:small; color:Red;display:none" id="spanAccountRecoveryEmail">This field is required</span>
                                    <span style="font-size:small; color:Red;display:none" id="spanAccountRecoveryEmailInvalid">This field must be a valid email address</span>
                                    <span style="font-size:small; display:none" id="spanAccountRecoveryEmailProgress">Sending account recovery instruction via email. Please wait.</span>
                                    <span style="font-size:small; display:none" id="spanAccountRecoveryEmailResult"></span>
                                </div>
                            </div>

                            <div class="form-group">
                                <div class="col-sm-offset-3 col-sm-9">
                                    <button id="butAccountRecoverySubmitEmail" data-role="none" class="btn btn-primary">Submit</button>
                                    <br />
                                </div>
                            </div>
                        </div>

                    </fieldset>

                </div>
            
            </form>
            </div>
        </div>


        
     </div>
     <br /><br />
 </div>



