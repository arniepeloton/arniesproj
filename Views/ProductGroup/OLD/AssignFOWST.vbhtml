@Code
    ViewData("Title") = "Hot Button Settings"
    Layout = "~/Views/Shared/Layout3Single.vbhtml"
End Code
@Section head

End Section
<script>
    require([GetPathFormat('/Scripts/sources/ProductGroup/AssignFOWST.js?crc=')]);
</script>

    <div data-role="content" >

<div data-role="fieldcontain">
	    <label for="selFOWST" class="select">FOWST:</label>
        <select id="selFOWST">
	     
        </select>
     </div>
     <div>
        <div style="text-align:center"> <a id="btnAssignToFOWST" data-role="button">Save</a> </div>
     </div>

     <br />
        <div>
            <ul data-role="listview" id="lstMenuFowstContainer" data-inset="true">   
				<li data-role="list-divider">List of FOWST where template is assigned</li>
                <li>

                    <form id="frmMenuFowstSearch">
                        <div><input id="txtMenuFowstSearch" type="search" value="" /></div>
                    </form>

                    <div data-role="fieldcontain" id="divMenuFowstTemplate"></div>
                    <div>
                        <ul data-role="listview" id="lstMenuFowstTemplate" data-inset="true">   
                        </ul>
                    </div>
                </li>
            </ul>
        </div>

        <div>
        <br />
	        <div style="text-align:center"> <a id="btnAssignFOWSTDone" data-role="button">Done</a> </div>
        </div>

    </div>
