@Code
    ViewData("Title") = "Home"
    'Layout = "~/Views/Shared/Guest.vbhtml"
    
End Code
    <script type="text/javascript">
    </script>
    @If ProWeb.Common.UseMinifiedJavascripts Then
        @<text>
        @*<script src="/Scripts/minified/Home.js" type="text/javascript"></script>*@
        </text>
    Else
        @<text>
        @*<script src="/Scripts/sources/Project/Home.js" type="text/javascript"></script>*@
        </text>
    End If
    <link type="text/css" rel="stylesheet" href="/Content/flatui-css-custom.css" />
    <style type="text/css">
        .button{
            height: 100px;
            background: none;
            border: 3px solid #fff;
            border-radius: 8px;
            font-size: 31px;
            padding: 30px;
            color: #fff;
            cursor:pointer;
        }

        .apps{
            cursor:pointer;
        }

        .button:hover{
            border: 3px solid #131095;
        }

        body {
            background: rgb(19,16,149);
background: linear-gradient(0deg, rgba(19,16,149,1) 0%, rgba(60,174,250,1) 100%); top: 0; left: 0; right: 0; bottom: 0; color:#fff; font-family: monospace;
        }
    </style>

<script type="text/javascript">
    function MCR() {
        location = location.origin + '/signin'
    }

    function EAS() {
        location = location.origin + '/signin/erp'
    }

    function SIDocScanner() {
        location = location.origin + '/SIDocScanner/Signin'
    }

    function AppDownload(app) {
        try {
            var http = new XMLHttpRequest();
            http.open('HEAD', "http://app.mobiz-apps.com", false);
            http.send();

            window.open("http://app.mobiz-apps.com/androidjstest/" + app, "_self");
        } catch (ex) {
            alert('Can\'t connect to the server');
        }
    }
</script>
<div>
    <div style="">
        <table style="width:100%; color:#fff;">
            <tr>
                <td style="text-align: center; font-size: 40px; padding: 20px;">
                    Hello Welcome to Mobiz
                </td>
            </tr>
            <tr>
                <td style="text-align: center; font-size: 20px; padding: 20px;">
                    Where do you want to go?
                </td>
            </tr>
            <tr>
                <td style="text-align: center; font-size: 20px; padding: 10px;">
                    <button onclick="MCR()" class="button">Complete Retail</button>
                </td>
            </tr>
            <tr>
                <td style="text-align: center; font-size: 20px; padding: 10px;">
                    or
                </td>
            </tr>
            <tr>
                <td style="text-align: center; font-size: 20px; padding: 10px;">
                    <button onclick="EAS()" class="button">Enterprise Application System</button>
                </td>
            </tr>
            <tr>
                <td style="text-align: center; font-size: 20px; padding: 50px;">
                    Companion Applications:
                    <br /><br />
                    <div class="divTable" style="width: 600px; margin:auto;">
                        <div class="divTableRow">
                            <div class="divTableCell apps" onclick="AppDownload('mobizcompleteretail')">
                                <img src="/Content/images/icons/mcr.png" />
                                <br /><br />
                                Mobiz Complete Retail
                            </div>
                            <div class="divTableCell apps" onclick="AppDownload('mobizfc')">
                                <img src="/Content/images/icons/fc.png" style="width:48px;" />
                                <br /><br />
                                Mobiz Fulfillment Center
                            </div>
                            <div class="divTableCell apps" onclick="AppDownload('MobizDTR')">
                                <img src="/Content/images/icons/dtr.png" style="width:48px;" />
                                <br /><br />
                                Mobiz DTR
                            </div>
                            <div class="divTableCell apps" onclick="SIDocScanner()">
                                <img src="/Content/images/erp-attachment_white.png" style="width:48px;" />
                                <br /><br />
                                SI Document Scanner
                            </div>
                        </div>
                    </div>
                </td>
            </tr>
        </table>
    </div>
</div>