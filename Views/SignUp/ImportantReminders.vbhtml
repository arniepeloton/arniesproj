@Code
    ViewData("Title") = "Important Reminders"
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
                        <div>
                            <h1 align="left" style="color:#4E4D4F">Important Reminders</h1>
                        </div>
                    </div>
                </div>
                <br />
            
                    <p>Your <strong>Mobiz Account</strong> belongs to you and not to the organization that you are/were connected with. 
                    Your account sticks with you and you can use it whenever Mobiz is used anywhere in the world. 
                    It is your permanent access to all of Mobiz services.</p>

                    <p>An example of where you can find Mobiz is in applications like your company's POS and other 
                    business apps. You can also use your account for Mobiz services such as online shopping, career 
                    hunting, and your payroll and HR services. All you need for access is your user name and password 
                    (and in some sensitive apps, a correctly registered device).</p>

                    <p><strong>As your Mobiz Account is like your personal key to all of Mobiz services, we strongly advise 
                    you to keep your username and password a very good secret.</strong> You can change your user name and/or 
                    password anytime you need or want. On our part, we have implemented a vast amount of ways to 
                    keep your data secure.</p>

                    <p>Finally, your Mobiz Account contains information that's necessary for HR services. Thus, 
                    it is to your best interest to <strong>fill in the information fields as accurately as you can</strong> to make 
                    processing your payroll and benefits a breeze.</p>

                    <p>From the Mobiz Team, thank you and welcome to Mobiz!</p>
            
          <br />

        <input type="checkbox" id="chbAgree" data-role="none" value="yes" name="agree"/><label for="chbAgree" data-role="none" style="font-weight:normal">&nbsp; I understand and agree with these reminders</label>
        <br /><br />
        <input type="hidden" id="hdnPage" value="privacy">
                 

         <button class="btn btn-primary" id="btnProceed" data-role="none" style="width:100%">Create my Mobiz Account</button>
            
 
     
     </div>
     <br /><br />
 </div>

