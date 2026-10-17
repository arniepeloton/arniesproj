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
    require([GetPathFormat('/Scripts/sources/SignUp/SignUpInfo.js?crc=' + new Date().getTime())]);

</script>

 
  <div style="width:100%;">
     <div class="container">

       <div class="row">
            <div class="col-md-12">
              
                 <h1 align="left" style="color:#4E4D4F">Welcome to Mobiz!</h1>
                

                <p>Please <a id="btnSignin">sign in</a> to continue.</p>
         
            </div>
        </div>
        
     </div>
     <br /><br /><br /><br /><br /><br /><br /><br /> <br /><br /><br /><br />
 </div>

