@Code
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head

    <script>
        require([GetPathFormat('/Scripts/sources/ProductGroup/MenuList.js?crc=')
        ]);
    </script>
    
End Section
<div id="pagePimMenu" data-role="page">

    <div id="divHeader" data-role="header">
        <h1>Hot Buttons Settings</h1>
    </div>

    <div data-role="content" >

        <div>
            <ul data-role="listview" id="lstOrgEmployeeContainer" data-inset="true">   
				<li data-role="list-divider">Hot Buttons Menu List</li>
                <li>
                    <div data-role="fieldcontain"><a id="btnAddHotButtonTemplate" data-role="button" data-icon="plus" data-inline="true">Add Hot Buttons Menu</a></div>

                    <form id="frmHotButtonSearch">
                        <div><input id="txtHotButtonSearch" type="search" value="" /></div>
                    </form>

                    <div data-role="fieldcontain" id="divHotButtonTemplate"></div>
                    <div>
                        <ul data-role="listview" id="lstHotButtonTemplate" data-inset="true">   
                        </ul>
                    </div>
                </li>
            </ul>
        </div>

        <div>
            <br />
	        <div style="text-align:center"> <a id="btnHotButtonsDone" data-role="button">Done</a> </div>
        </div>

    </div>

</div>

<div id="pageCreateName" data-role="page">
    <div id="divHeader" data-role="header">
        <h1>Hot Buttons Settings</h1>
    </div>

    <div data-role="content" >

        <div data-role="fieldcontain">
            <label for="txtTemplateName">Menu Template Name:</label>
	        <input type="text" id="txtTemplateName" maxlength="50"/>
        </div>

        <div>

	        <div style="text-align:center"> <a id="btnCreateTemplate" data-role="button">Create New Menu Template</a> </div>
        </div>
    </div>

    
</div>

<div id="pageAssignToFOWST" data-role="page">
    <ul data-role="listview" data-inset="true">
		<li data-role="list-divider">
            <div data-role="fieldcontain" style="height:5px;">
                Home
                <a href="#popupMenu" id="btnAddNew" data-rel="popup" data-role="button" data-inline="true" style="float:right; margin-top:-15px;">Add Buttons</a>

            </div>
        </li>
        <li>
        <div id="divProductGroup">
            <div name="divMain_ProductGroup" data-role="collapsible_clone" data-theme="b" data-content-theme="d" data-collapsed-icon="arrow-r" data-expanded-icon="arrow-d">
                <h3>Main</h3>
          
                <a id='btnSumPeopleInfoName' data-role='button' data-mini="true" style="float:right; margin-top:-48px; z-index:1; ">Add Buttons</a>
            </div>
        </div>
        </li>
     </ul>

     
</div>