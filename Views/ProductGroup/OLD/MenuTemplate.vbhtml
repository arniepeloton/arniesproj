@Code
    ViewData("Title") = "Hot Button Settings"
    Layout = "~/Views/Shared/Layout3Single.vbhtml"
End Code
@Section head
    <style>
        
    </style>
End Section
<script>
    require([GetPathFormat('/Scripts3/ProductGroup.js?crc=')]);
</script>

<p>Use this tool to create hot buttons. Tap on "Add Button" to add a new group or specific SKU.</p>

<div class="ui-grid-a">
    <div class="ui-block-a">
        <h2 id="hdrTemplateName"></h2><div id="divEditName" style="display:none;"><input type="text" id="txtEditName" maxlength="50" /></div>
    </div>
    <div class="ui-block-b" style="text-align:right;">
        <a href="" id="btnSaveName" data-role="button" data-inline="true" style="display:none;">Save Name</a>
        <a href="" id="btnEditName" data-role="button" data-inline="true">Edit Name</a>
        <a href="" id="btnCancel" data-role="button" data-inline="true" style="display:none;">Cancel</a>
        <a href="" id="btnDeleteTemplate" data-role="button" data-inline="true">Delete Template</a>
    </div>

</div>

<div id="divProductGroup">
    
    <div name="divMain_ProductGroup" class="col" data-role="collapsible_clone" data-collapsed-icon="arrow-r" data-expanded-icon="arrow-d">
        <h3>Home</h3>
            <br />
            <a href="#popupMenuFirst" name ="btnAddGroupFirst" data-icon="plus" data-rel="popup" data-role="button" data-inline="true" style="float:right; margin-top:-15px;">Add Button</a>
            <a href="#popupMenu" name ="btnAddGroup" data-icon="plus" data-rel="popup" data-role="button" data-inline="true" style="float:right; margin-top:-15px;display: none;">Add Button</a>
            <a href="#popupMenuProduct" name ="btnAddProduct" data-icon="plus" data-rel="popup" data-role="button" data-inline="true" style="float:right; margin-top:-15px; display: none;">Add Button</a>
            <a href="" name ="btnDelete" data-icon="minus" data-role="button" data-inline="true" style="float:right; margin-top:-15px; display: none; ">Delete Group</a>
            <br />
            <br />
            <div name="divSubgroups_ProductGroup">
            </div>

            <div name="divProductList_ProductGroup">
            </div>

    </div>
        <div data-role="popup" id="popupMenuFirst" >
				<ul data-role="listview" data-inset="true" style="min-width:210px;" >
					<li data-role="divider" data-theme="a" >Add Buttons</li>
					<li><a href="#popupNewGroup" name="addGroup" data-rel="popup" data-position-to="window">Add Group</a></li>
					<li><a href="" id ="butAddProduct_ProductGroupFirst" data-rel="popup" data-position-to="window">Add SKU</a></li>
				</ul>
		</div>

        <div data-role="popup" id="popupMenu" >
				<ul data-role="listview" data-inset="true" style="min-width:210px;" >
					<li data-role="divider" data-theme="a" >Add Buttons</li>
					<li><a href="#popupNewGroup" name="addGroup" data-rel="popup" data-position-to="window">Add Group</a></li>
					<li><a href="" id ="butAddProduct_ProductGroup" data-rel="popup" data-position-to="window">Add SKU</a></li>
                    <li><a href="" id ="butDelete_ProductGroup" data-rel="popup" data-position-to="window">Delete</a></li>
				</ul>
		</div>

        <div data-role="popup" id="popupMenuProduct" >
				<ul data-role="listview" data-inset="true" style="min-width:210px;" >
					<li data-role="divider" data-theme="a" >Add Buttons</li>
					<li><a href="" id ="butAddProduct_ProductSKU" data-rel="popup" data-position-to="window">Add SKU</a></li>
                    <li><a href="" id ="butDelete_ProductSKU" data-rel="popup" data-position-to="window">Delete</a></li>
				</ul>
		</div>
        
        <div data-role="popup" id="popupNewGroup" data-theme="b" class="ui-corner-all">
			<form>
				<div style="padding:10px 20px;">
				  <h3>New Group</h3>
		          <label for="txtName_ProductGroup" class="ui-hidden-accessible">Name:</label>
		          <input type="text" id="txtName_ProductGroup" name="groupName" placeholder="Group Name" maxlength="23"/>

		    	  <button id="butSave_ProductGroup">Save</button>
				</div>
			</form>
		</div>

        <div data-role="popup" id="popupNewSKU" class="ui-corner-all">
			<form>
				<div style="padding:10px 20px;">
				  <h3>New SKU</h3>

		          <input type="search" id="txtSKU" value="" placeholder="Search SKU" data-theme="a" />

		    	  <button id="btnSearchSKU" data-theme="b">Go</button>
				</div>
			</form>
		</div>

            <div data-role="popup" id="popupNewSKUName" data-theme="b" class="ui-corner-all" data-dismissible="false">
			<form>
				<div style="padding:10px 20px;">
				  <h3>New SKU</h3>
		          <label for="txtName_SKU">Please enter a short name for <label id="lblFullProductName"></label>:</label>
		          <input type="text" id="txtName_SKU" name="skuName" placeholder="SKU Short Name" maxlength="23"/>
                  <input type="hidden" id="hdnSKUid"/><input type="hidden" id="hdnGroupId"/>
                  
		    	  <button id="butSave_SKU">Save</button>
				</div>
			</form>
		</div>

</div>
    <div>
        <br />
        @*<div style="text-align:center"> <a id="btnAssignToFOWST" data-role="button">Assign this template to FOWST</a> </div>*@
	    <div style="text-align:center"> <a id="btnHotButtonsTemplateDone" data-role="button">Done</a> </div>
    </div>

