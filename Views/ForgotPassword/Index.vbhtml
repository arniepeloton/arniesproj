@Code 
    ViewData("Title") = "Forgot Password"
    Layout = "~/Views/Main/Layout.vbhtml"
End Code
    <script type="text/javascript">
        var mCaptchaKey = '@ViewData("CaptchaKey")';
    </script>
    @If ProWeb.Common.UseMinifiedJavascripts Then
        @<text>
        <script src="/Scripts/minified/Project.js" type="text/javascript"></script>
        </text>
    Else
        @<text>
        <script src="/Scripts/sources/Project/ForgotPassword.js" type="text/javascript"></script>
        </text>
    End If
    <style type="text/css">
        
    </style>
<h2>
    @ViewData("Title")</h2>
<div id="divCaptchaInfo">
    This page is currently for CAPTCHA testing only and does not actually send an email</div>
<div>
    This page allows you to change your password in case you have forgotten it.</div>
<div id="divCaptcha">
    <div id="divResult">
    </div>
    <form id="formCaptcha">
    <table class="TableForm">
        <tr>
            <td>
            </td>
            <td>
                Please type the text that you see below onto the textbox.<br />
            </td>
        </tr>
        <tr>
            <td>
                Image:
            </td>
            <td>
                <img name="imgCaptcha" src="/captcha/@(ViewData("CaptchaKey")).jpg"/>
            </td>
        </tr>
        <tr>
            <td>
                Text:
            </td>
            <td>
                <input name="txtCaptcha" />
            </td>
        </tr>
        <tr>
            <td>
                Email:
            </td>
            <td>
                <input name="txtEmail" />
            </td>
        </tr>
        <tr>
            <td>
                <button id="butSubmit" type="submit">
                    Submit</button>
            </td>
            <td>
            </td>
        </tr>
    </table>
    </form>
</div>
