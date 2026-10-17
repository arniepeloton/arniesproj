@Code
    ViewData("Title") = "Login"
    Layout = "~/Views/Main/Layout.vbhtml"
End Code

    <script type="text/javascript">
    
    </script>
    @If ProWeb.Common.UseMinifiedJavascripts Then
        @<text>
        <script src="/Scripts/minified/Login.js" type="text/javascript"></script>
        </text>
    Else
        @<text>
        <script src="/Scripts/sources/Project/Login.js" type="text/javascript"></script>
        </text>
    End If
    <style type="text/css">
        
    </style>

@If ViewData.ContainsKey("Mode") _
    AndAlso ViewData.ContainsKey("SubTrans") _
    AndAlso ViewData.ContainsKey("Trans") Then
    
    @<div class="Error">
        Access Denied<br />
        You need the following access:<br />
        Trans: @ViewData("Trans")
        <br />
        SubTrans: @ViewData("SubTrans")
        <br />
        Mode: @ViewData("Mode")
    </div>
    If CStr2(ProWeb.Common.WebSession.UserID).Length > 0 Then
    @<div>
        <div style="margin: 3px 0">
            Hi, @WebProj.WebSession.Instance.FirstName
        </div>
        <div>
            You are logged in as a/an @WebProj.WebSession.Instance.UserAccess.LevelType and doesn't have permission to the feature that you attempted to access.<br />
            Please try 1 or more of the following solutions:
            <ol>
                <li>Ask your administrator to give access rights to your access level. </li>
                <li>Ask your administrator to give you another access level. </li>
                <li>Log in as a different user. The log in form is provided below. </li>
            </ol>
        </div>
    </div>
    End If
End If
<form id="formLogin">
<div name="divBlock">
    <table class="TableForm">
        <tr>
            <td colspan="2">
                <div name="divResult">
                </div>
            </td>
        </tr>
        <tr>
            <td class="label">
                Username:
            </td>
            <td>
                <input name="txtUsername" title="Your ERP username" />
            </td>
        </tr>
        <tr>
            <td class="label">
                Password:
            </td>
            <td>
                <input name="txtPassword" type="password" title="You ERP password" />
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <button name="butLogIn">Login</button>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <a href="/forgotpassword">Forgot your password</a>
            </td>
        </tr>
    </table>
</div>
</form>
