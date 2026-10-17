@Code
    ViewData("Title") = "Add User"
    Layout = "~/Views/Shared/Layout3Single.vbhtml"
End Code
<script>
    require([GetPathFormat('/Scripts3/AddUserAdd.js?crc=')]);
</script>
<div data-role="fieldcontain">
    <fieldset data-role="controlgroup" data-type="horizontal">
        <legend>Employee:</legend>
        <div data-role="controlgroup">
            <p id="spanEmployeeName">
            </p>
            <button id="butEmployee">Browse Employee...</button>
        </div>
    </fieldset>
</div>
<div data-role="fieldcontain">
    <fieldset data-role="controlgroup" data-type="horizontal">
        <legend>Security Level:</legend>
        <div data-role="controlgroup">
            <p id="spanSecurityLevel">
            </p>
            <button id="butSecurityLevel">Browse Security Level...</button>
        </div>
    </fieldset>
</div>
<div data-role="fieldcontain">
    <label for="txtEmail">
        Email:</label>
    <input id="txtEmail" maxlength="300" />
    <p>You may enter multiple email addresses. Separate each email address with a space, comma, colon, semicolon, slash, or backslash</p>
</div>

<button id="butOk">Ok</button> 