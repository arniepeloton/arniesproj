@Code
    ViewData("Title") = "Account Management"
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
    require([GetPathFormat('/Scripts/sources/SignIn/AccountManagementInfo.js?crc=')]);

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
                
                 <h1 align="left" style="color:#4E4D4F">Account Management</h1>
              
                    <form action="" id="" method="POST">
                         <span id="lblMsg" style="display:none;">Oops! You are not authorized to view this page. <br /><br />
                            <a href="/People/PeopleMenu">Back</a>
                         
                                <br />  <br />  <br />  <br />  <br />  <br />  <br />  <br />  <br />
                            </span>
 
                              
                             <div id="divUserAccount" style="display:none;">
                                 <p>Hi <span id="lblUser" ></span>! This account was created for you by an administrator. To start using Mobiz, you must reset your user name and password.</p>
                                  <fieldset>
                                     
                            <legend>User Account</legend>
                             <div class="form-horizontal" role="form">
                             <div class="form-group">
                                <label for="txtUserNameManage" class="col-sm-3 control-label" style="font-weight:500; font-size:16px; text-align:left">User Name: <span style="color:Red">*</span></label>
                                <div class="col-sm-9">
                                    <input type="text" class="form-control" id="txtUserNameManage" maxlength="150" required>
                                    <span style="font-size:small; color:Red;display:none" id="spanUserNameManage" >This field is required</span>
                                     <span style="font-size:small; color:Red;display:none" id="spanUserNameManageChars" >User Name must be at least 8 characters long</span>
                                       <span style="font-size:small; color:Red;display:none" id="spanUserNameManageAlphanum" >User Name must contain only alphanumeric and underscore characters</span>
                                </div>
                            </div>
				             <div class="form-group">
                                <label for="txtPasswordManage" class="col-sm-3 control-label" style="font-weight:500; font-size:16px; text-align:left">Password: <span style="color:Red">*</span></label>
                                <div class="col-sm-9">
                                    <input type="password" class="form-control" id="txtPasswordManage" maxlength="150" required>
                                    <span style="font-size:small; color:Red;display:none" id="spanPasswordManage" >This field is required</span>
                                     <span style="font-size:small; color:Red;display:none" id="spanPasswordManageChars" >Password must be at least 8 characters long</span>
                                  
                                </div>
                            </div>
                            <div class="form-group">
                                <label for="txtConfirmPasswordManage" class="col-sm-3 control-label" style="font-weight:500; font-size:16px; text-align:left">Confirm Password: <span style="color:Red">*</span></label>
                                <div class="col-sm-9">
                                    <input type="password" class="form-control" id="txtConfirmPasswordManage" maxlength="150" required>
                                    <span style="font-size:small; color:Red;display:none" id="spanConfirmPasswordManage" >This field is required</span>
                                     <span style="font-size:small; color:Red;display:none" id="spanPasswordRepeatManage" >The password you entered did not match</span>
                                  
                                </div>
                            </div>
                      
                        <legend id="lblSecurity">Security Question   <br />  <span style="font-size:small;">Note: You must remember the answers to the security questions you selected. This will be used to reset your account in case you forget your password.</span></legend>
                 
                         <div class="form-group" id="secSecurity1">
                                <label for="selQuestion1Manage" class="col-sm-3 control-label" style="font-weight:500; font-size:16px; text-align:left">Question No. 1: <span style="color:Red">*</span></label>
                                <div class="col-sm-5">
                                   <select id="selQuestion1Manage" data-role="none" class="form-control" >
                                      <option value="">Please Select</option>
				                                <option value="Where is your mother's birthplace?">Where is your mother's birthplace?</option>
				                                <option value="Who is your best childhood friend?">Who is your best childhood friend?</option>
				                                <option value="What is the name of your first pet?">What is the name of your first pet?</option>
				                                <option value="Who is your favorite teacher?">Who is your favorite teacher?</option>
				                                <option value="Who is your favorite historical person?">Who is your favorite historical person?</option>
	                                            <option value="What is your grandfather's occupation?">What is your grandfather's occupation?</option>
                                        </select>
                                     <span style="font-size:small; color:Red;display:none" id="spanAnswer1Manage" >This field is required</span>
                                    <span style="font-size:small; color:Red;display:none" id="spanQuestion1Manage" >Please choose a different question</span>

                                </div>
                                  <div class="col-sm-4">
                               
                                     <input type="text" class="form-control" id="txtQ1AnswerManage" placeholder="Answer for question no. 1" maxlength="150" required>
                           
                                </div>
                            </div>
                             <div class="form-group" id="secSecurity2">
                                <label for="selQuestion2Manage" class="col-sm-3 control-label" style="font-weight:500; font-size:16px; text-align:left">Question No. 2: <span style="color:Red">*</span></label>
                                <div class="col-sm-5">
                                   <select id="selQuestion2Manage" data-role="none" class="form-control" >
                                      <option value="">Please Select</option>
				                                <option value="Where is your mother's birthplace?">Where is your mother's birthplace?</option>
				                                <option value="Who is your best childhood friend?">Who is your best childhood friend?</option>
				                                <option value="What is the name of your first pet?">What is the name of your first pet?</option>
				                                <option value="Who is your favorite teacher?">Who is your favorite teacher?</option>
				                                <option value="Who is your favorite historical person?">Who is your favorite historical person?</option>
	                                            <option value="What is your grandfather's occupation?">What is your grandfather's occupation?</option>
                                        </select>
                                        <span style="font-size:small; color:Red;display:none" id="spanAnswer2Manage" >This field is required</span>
                                    <span style="font-size:small; color:Red;display:none" id="spanQuestion2Manage" >Please choose a different question</span>

                                </div>
                                  <div class="col-sm-4">
                               
                                     <input type="text" class="form-control" id="txtQ2AnswerManage" maxlength="150" placeholder="Answer for question no. 2" required>
                                
                                </div>
                            </div>
                              <div class="form-group">
                                    <div class="col-sm-offset-3 col-sm-9">
                                        <button id="btnSubmitManage" data-role="none" class="btn btn-primary">Save</button>
                                        <br />
                                        <span style="font-size:small; color:Red;display:none" id="spanAlert" >Oops! You might have missed filling in some required information.  Please check the sign up form above and type in the correct information inside data fields with an asterisk (*) sign.</span>
                                    </div>
                                </div>
                                <br /><br />
                                </div>
                                   </fieldset>
                            </div>

                   </form> 
         
                <input type="hidden" id="hdnPage" value="accountManagement">
                <input type="hidden" id="txtPeopleID">

            </div>
        </div>


        
     </div>
     <br /><br />
 </div>

