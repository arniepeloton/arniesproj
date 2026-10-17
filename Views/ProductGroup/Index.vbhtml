<script>
    require([GetPathFormat('/Scripts/sources/ProductGroup/MenuList.js?v=' + assetversion)
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
    <h1 class="display-6 mb-4">Hot Button Template</h1>

    <div data-role="content">

        <div class="row mb-3">
            <label class="col-sm-2 mb-3 col-form-label" for="txtTemplateName">Template Name:</label>
            <div class="col-sm-10">
                <input type="text" data-role="none" class="form-control" id="txtTemplateName" maxlength="50" placeholder="Enter here" />
            </div>
        </div>

        <div id="divTemplateVDMModel" style="display:none;">
            <div class="row mb-3">
                <label class="col-sm-2 mb-3 col-form-label" for="txtTemplateName">Is this template used for VDM?</label>
                <div class="col-sm-10">
                    <div class="form-check form-switch mb-3" style="font-size:25px;">
                        <input class="form-check-input" data-role="none" type="checkbox" id="selTemplateVDMActivate">
                    </div>
                </div>
            </div>

            <div id="divTemplateVDMModelOption" style="display:none;">
                <div class="row mb-3">
                    <label class="col-sm-2 mb-3 col-form-label" for="txtTemplateName">Vending Machine Model: <span class="required">*</span></label>
                    <div class="col-sm-10">
                        <select data-role="none" class="form-select fixed" id="selTemplateVDMModel"></select>
                    </div>
                </div>
            </div>            
        </div>        

        <div class="text-center mt-4">
            <button data-role="none" id="btnCancelCreateTemplate" class="btn btn-secondary btn-lg">Cancel</button>
            <button data-role="none" id="btnCreateTemplate" class="btn btn-success btn-lg">Create New Template</button>
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