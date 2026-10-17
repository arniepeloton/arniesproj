@Code
    ViewData("Title") = "Login"
    Layout = "~/Views/Shared/Layout3Single.vbhtml"
End Code
@Section head
    <script>
        require([GetPathFormat('/Scripts3/Login3.js?crc=39927E84')]);
    </script>
End Section
<div id="divNoCookies" data-role="collapsible" data-collapsed="false" style="display:none">
    <h3>
        You've disabled your cookies</h3>
    Please enable cookies in your browser
</div>
<div style="text-align: center; margin: auto">
    <div><img src="Content/images/Mobiz.png" /></div>
    <span style="font-style:italic; font-size:18px">Business-Grade Apps In Your Pocket</span>
</div>
<form id="formLogin">
<div data-role="fieldcontain">
    <label for="txtUsername">
        Username:</label>
    <input id="txtUsername" value="" autofocus />
</div>
<div data-role="fieldcontain">
    <label for="txtPassword">
        Password:</label>
    <input type="password" id="txtPassword" value="" />
</div>
<button type="submit">Login</button>
</form>
<div data-role="fieldcontain" style="text-align:center">
    <a href="#pageForgotPassword">Forgot your username and/or password</a>
</div>
@Section OtherPages
    <div id="pageForgotPassword" data-role="page">
        <div data-role="header">
            <a href="#" data-icon="arrow-l" data-rel="back">Back</a>
            <h1>
                Forgot Password</h1>
        </div>
        <div data-role="content">
            This feature is not yet implemented.
        </div>
    </div>
End Section
