@Code
    ViewData("Title") = "Add User"
    Layout = "~/Views/Shared/Layout3Single.vbhtml"
End Code
<script>
    require([GetPathFormat('/Scripts3/AddUser.js?crc=')]);
</script>
<div id="divInvalid" style="display:none">
    <h3>Invalid Link</h3>
    <ul>
        <li>Link was already used</li>
    </ul>
</div>
<div id="divValid" style="display:none">
    <div data-role="fieldcontain">
        <fieldset data-role="controlgroup" data-type="horizontal">
            <legend>Employee:</legend>
            <div data-role="controlgroup">
                <span id="spanEmployeeName">
                </span>
            </div>
        </fieldset>
    </div>
    <div data-role="fieldcontain">
        <fieldset data-role="controlgroup" data-type="horizontal">
            <legend>Security Level:</legend>
            <div data-role="controlgroup">
                <span id="spanSecurityLevel">
                </span> 
            </div>
        </fieldset>
    </div>
    <div data-role="fieldcontain">
        <label for="txtUsername">
            Username:</label>
        <input id="txtUsername" maxlength="20" />
    </div>
    <div data-role="fieldcontain">
        <label for="txtPassword">
            Password:</label>
        <input id="txtPassword" type="password"" maxlength="20" />
    </div>
    <div data-role="fieldcontain">
        <label for="txtPassword2">
            Retype password:</label>
        <input id="txtPassword2" type="password"" maxlength="20" />
    </div>
    <button id="butOk">Ok</button>
</div>

<div data-role="fieldcontain">
        <label for="txtUsername">
            Username:</label>
        <input id="txtUsername" maxlength="20" />
    </div>

<div data-role="fieldcontain">
    <fieldset data-role="controlgroup" data-type="horizontal">
        <legend>Current date:</legend>
        <select></select>
        <select></select>
        <select data-role="slider"></select>

    </fieldset>
</div>

<div id="divDiv" data-role="fieldcontain">
    <fieldset id="fieldDate" data-role="DateCombo" data-type="horizontal">
        <legend>Current date:</legend>
     
    </fieldset>
</div>
<div id="divDiv" data-role="fieldcontain">
    <fieldset id="fieldTime" data-role="TimeCombo" data-type="horizontal">
        <legend>Current time:</legend>
     
    </fieldset>
</div>