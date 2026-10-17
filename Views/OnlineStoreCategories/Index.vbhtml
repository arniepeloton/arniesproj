<script>
    require([GetPathFormat('/Scripts/sources/OnlineStoreCategories/OnlineStoreCategories.js?crc=')
        ]);
</script>

<div id="pagePimMenu" style="display:none;">

    @*<div id="divHeader" data-role="header">
        <h1>Hot Buttons Settings</h1>
    </div>*@

    <div data-role="content" >

        <div>
            <ul data-role="listview" id="lstOrgEmployeeContainer" data-inset="true">   
				<li data-role="list-divider">Hot Button Template</li>
                <li>
                    <div data-role="fieldcontain"><a id="btnAddHotButtonTemplate" data-role="button" data-icon="plus" data-inline="true">Add New</a></div>

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

<div id="pageCreateName" style="display:block;">
    @*<div id="divHeader" data-role="header">
        <h1>Hot Buttons Settings</h1>
    </div>*@

    <div data-role="content" >

        <div data-role="fieldcontain">
            <label for="txtStoreCategoryTemplateName">Template Name:</label>
            <input type="text" id="txtStoreCategoryTemplateName" maxlength="50" />
        </div>

        <div>
            <br />
            <br />
	        <div style="text-align:center"> 
                <a id="btnCreateStoreCategoryTemplate" data-role="button"><h3>Create New Template</h3></a> 
                <a id="btnCancelStoreCategoryTemplate" data-role="button"><h3>Cancel</h3></a>    
            </div>
        </div>
    </div>

    
</div>

<div id="pageAssignToFOWST" style="display:none;">
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