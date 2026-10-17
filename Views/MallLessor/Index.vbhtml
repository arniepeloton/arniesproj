@Code
    ViewData("Title") = "Mall Lessor Automatic Report Setup"
    Layout = "~/Views/Shared/Layout3Single.vbhtml"
End Code
<script>
    require([GetPathFormat('/Scripts3/MallLessor.js?crc=')]);
</script>
<h3>
    Mall Lessor Automatic Report Setup</h3>
<p>
    Step 1: Select your organization</p>
<div id="divOrganization" style="font-weight: bold;">
</div>
<button id="butOrganization">Browse</button>
<p>
    Step 2: Select your FOWST</p>
<div id="divFowst" style="font-weight: bold;">
</div>
<button id="butFowst">Browse</button>
<p>
    Step 3: Select your mall</p>
<select id="cbMall">
    <option value="ayala">Ayala</option>
    <option value="sm">SM</option>
</select>
<div style="height: 30px">
</div>
<button id="butOk">Ok</button> <a href="/People/PeopleMenu" data-role="button">Cancel</a>