<script>
    require([GetPathFormat('/Scripts/sources/FranchisorCentral/FranchisorCentral.js?v=' + assetversion)]);
</script>
@*<link type="text/css" rel="Stylesheet" href="/Content/flatui-css-custom.css" />*@

<div style="display:none;">
    <div data-role="header">
        <h1>@ViewData("Title")</h1>
    </div>
     
    <div data-role="content">
        <div>
            <ul data-role="listview" data-inset="true">   
				<li data-role="list-divider">Group List</li>
                <li>
                    <div data-role="fieldcontain"><a id="btnFranchiseeGroup" data-role="button" data-icon="plus" data-inline="true">Add Group</a></div>
                                        
                    <div data-role="fieldcontain" id="divOrgRecordCountTeam"></div>
                    <div>
                        <ul data-role="listview" id="lstFranchiseeGroup" data-inset="true">   
                        </ul>
                    </div>
                </li>
            </ul>
        </div>
        
        <div>
            <br />
            <br />
	        <div style="text-align:center"> <a id="btnFranchiseeGroupDone" data-role="button">Done</a> </div>
        </div>

    </div>

</div>

<div id="pageFrancishorGroup" style="display:block">
    @*<div data-role="header">
        <h1>@ViewData("Title")</h1>
    </div>*@

    <div data-role="none" style="display:none;" id="panOwnershipTypeOption" data-dismissible="false" data-overlay-theme="a" data-theme="a" data-corners="false"> 
        <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right btnPopupClose">Close</a>
        <center><h3>Ownership Type</h3></center>
        <div data-role="content" class="ui-content ui-body-d" style="max-width:350px; word-wrap: break-word;">
            <div style="padding: 8px 15px 0px 15px; max-height:200px; overflow:auto;">
                <ul data-role="listview" class="li-theme-c noBorderList custom-list custom-list-sm">
                    <li>
                        <a id="btnCoOwned">Co-Owned</a>
                    </li>
                    <li>
                        <a id="btnFranchised">Franchised</a>
                    </li>
                </ul>
                <br />
            </div>
        </div>       
    </div>

    <div data-role="content">
        <div class="row wrapper min-vh-100 flex-column flex-md-row mb-5">
            <div class="col-12 col-md-2 p-0 bg-light flex-shrink-1 mb-3">
                <nav class="navbar navbar-expand-md navbar-light bg-light align-items-start flex-md-column flex-row p-2">
                    <a data-role="none" class="navbar-brand" href="#">Menu</a>
                    <a data-role="none" href="#" class="navbar-toggler" data-toggle="collapse" data-target=".sidebarGroup">
                        <span class="navbar-toggler-icon"></span>
                    </a>
                    <div class="collapse navbar-collapse sidebarGroup mt-2">
                        <ul class="flex-column w-100 justify-content-between nav nav-pills">
                            <li><a class="nav-link active" role="button" data-role="none" data-toggle="tab" href="#tabGroupInfo">Information</a></li>
                            <li><a class="nav-link" role="button" data-role="none" data-toggle="tab" href="#tabGroupHeads">Heads</a></li>
                            <li><a class="nav-link" role="button" data-role="none" data-toggle="tab" href="#tabGroupBusinessRules">Business Rules</a></li>
                            <li><a class="nav-link" role="button" data-role="none" data-toggle="tab" href="#tabGroupFowstMembers">FOWST Members</a></li>
                            <li><a class="nav-link" role="button" data-role="none" data-toggle="tab" href="#tabGroupOrderCutOff">Order Cut-Off</a></li>
                        </ul>
                    </div>
                </nav>
            </div>

            <div class="col col-md bg-faded">
                <div class="tab-content">
                    <div class="tab-pane fade active show" id="tabGroupInfo">
                        <div class="row mb-3">
                            <label class="col-md-2 col-form-label" for="txtGroupName">Group Name : <span style="color:#F00;">*</span></label>
                            <div class="col-md-10">
                                <input type="text" id="txtGroupName" data-role="none" class="form-control" />
                            </div>
                        </div>

                        <div class="row mb-3">
                            <label class="col-md-2 col-form-label" for="txtGroupDescription">Group Description:</label>
                            <div class="col-md-10">
                                <input type="text" id="txtGroupDescription" data-role="none" class="form-control" />
                            </div>
                        </div>

                        <div class="row mb-3">
                            <label class="col-md-2 col-form-label" for="txtRoyaltyFee">Royalty Fee (%):</label>
                            <div class="col-md-10">
                                <input type="text" id="txtRoyaltyFee" data-role="none" class="form-control" />
                            </div>
                        </div>

                        <div class="row mb-3">
                            <label class="col-md-2 col-form-label" for="txtMarketingFee">Marketing Fee (%):</label>
                            <div class="col-md-10">
                                <input type="text" id="txtMarketingFee" data-role="none" class="form-control" />
                            </div>
                        </div>

                        <div class="row mb-3">
                            <label class="col-md-2 col-form-label" for="txtGroupFOWST">Group Distribution Center: <span style="color:#F00;">*</span></label>
                            <div class="col-md-10">
                                <div class="input-group">
                                    <input type="text" id="txtGroupFOWST" data-role="none" class="form-control" readonly />
                                    <button id="btnSearchFOWST" data-role="none" class="btn btn-secondary">Search FOWST</button>
                                </div>
                            </div>
                        </div>

                        <div class="row mb-3">
                            <label class="col-auto col-md-2 col-form-label" for="chbIsGroupAFranchisee">Is this a franchisee?</label>
                            <div class="col-auto col-md-10">
                                <div class="form-check form-switch" style="font-size:25px;">
                                    <input data-role="none" type="checkbox" class="form-check-input" id="chbIsGroupAFranchisee" />
                                </div>
                            </div>
                        </div>

                        <div class="row mb-3">
                            <label class="col-auto col-md-2 col-form-label" for="chbIsGroupActive">Is active?</label>
                            <div class="col-auto col-md-10">
                                <div class="form-check form-switch" style="font-size:25px;">
                                    <input data-role="none" type="checkbox" class="form-check-input" id="chbIsGroupActive" />
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="tab-pane fade" id="tabGroupHeads">
                        <div class="row mb-3 align-items-center">
                            <div class="col text-right">
                                <button id="btnAddGroupHead" data-role="none" class="btn btn-success">Add Group Head</button>
                            </div>
                        </div>

                        <div data-role="none" id="lstGroupHeads" class="list-group mb-3"></div>
                    </div>

                    <div class="tab-pane fade" id="tabGroupBusinessRules">
                        <div>
                            <h5 class="mb-3">Business Rules for Inventory Levels</h5>

                            <div class="row mb-3">
                                <label class="col-md-2 col-form-label" for="txtMaxInventoryValue">Max Inventory Value:</label>
                                <div class="col-md-10">
                                    <input type="text" id="txtMaxInventoryValue" data-role="none" class="form-control" placeholder="Set Max Inventory Value" />
                                </div>
                            </div>

                            <div class="row mb-3">
                                <label class="col-md-2 col-form-label" for="selInventoryValuePriceLevel">Max Inventory Value Price Level Basis:</label>
                                <div class="col-md-10">
                                    <select id="selInventoryValuePriceLevel" data-role="none" class="form-select">
                                        <option id="optCOGS">COGS</option>
                                        <option id="optRetailPrice">Retail Price</option>
                                    </select>
                                </div>
                            </div>
                        </div>

                        <div>
                            <div class="row mb-4 align-items-center">
                                <div class="col-6 col-form-label">
                                    <h5>Business Rules for Selling Prices</h5>
                                </div>
                                <div class="col-6 text-right">
                                    <button id="btnAddOtherPriceLevel" data-role="none" class="btn btn-success">Add </button>
                                </div>
                            </div>

                            <div class="row mb-3" style="display:none;">
                                <label class="col-md-2 col-form-label" for="selApplyToAllDefaultSellingPrice">Default Selling Price Level:</label>
                                <div class="col-md-10">
                                    <select id="selApplyToAllDefaultSellingPrice" data-role="none" class="form-select"></select>
                                </div>
                            </div>

                            <table data-role="none" class="table table-striped table-hover mb-3">
                                <thead class="custom-theme-c">
                                    <tr>
                                        <th colspan="3">Authorized Selling Prices for this Group</th>
                                    </tr>
                                    <tr>
                                        <th>Price Level</th>
                                        <th>Default</th>
                                        <th></th>
                                    </tr>
                                </thead>
                                <tbody id="tbodyOtherPriceLevel"></tbody>
                            </table>
                        </div>

                        <div>
                            <h5 class="mb-3">Business Rules for Purchase Orders and Branch Requests</h5>

                            <div class="row mb-3">
                                <label class="col-md-2 col-form-label" for="optIsExclusivePO">PO Products are exclusive to Principal:</label>
                                <div class="col-md-10">
                                    <div class="form-check form-switch mb-3" style="font-size:20px;">
                                        <input class="form-check-input" data-role="none" type="checkbox" id="optIsExclusivePO">
                                    </div>
                                </div>
                            </div>

                            <div class="row mb-3">
                                <label class="col-md-2 col-form-label" for="selApplyToAllDefaultPOPrice">PO Price Level(This is the default acquisition price from the principal):</label>
                                <div class="col-md-10">
                                    <select id="selApplyToAllDefaultPOPrice" data-role="none" class="form-select"></select>
                                </div>
                            </div>

                            <div class="row mb-5">
                                <div class="offset-md-2 col-md-10">
                                    <button data-role="none" class="btn btn-secondary btn-block" id="btnApplyToAllFOWST">Apply to FOWST Members</button>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="tab-pane fade" id="tabGroupFowstMembers">
                        <div class="row mb-3 align-items-center">
                            <divc class="col">
                                <input data-role="none" class="form-control" id="txtGroupSearchFranchise" placeholder="Search here" />
                            </divc>
                            <div class="col text-right">
                                <button id="btnAddFranchisee" data-role="none" class="btn btn-success">Add Group / Franchisee Member</button>
                            </div>
                        </div>

                        <div class="table-fixed-container">
                            <div id="dvFOWSTList" class="table-fixed">
                                <table data-role="none" class="table table-light table-striped table-hover">
                                    <thead>
                                        <tr id="tblFOWSTListHeader" class="align-top text-center" style="font-size:14px;">
                                            <th class="fixed-col" data-sort="string" style="cursor:pointer;">FOWST</th>
                                            <th class="fixed-col" data-sort="string" style="cursor:pointer;">FOWST Address</th>
                                            <th class="fixed-col" data-sort="string" style="cursor:pointer;">Organization Name</th>
                                            <th class="fixed-col" data-sort="string" style="cursor:pointer;">Owner Name</th>
                                            <th class="fixed-col" data-sort="string" style="cursor:pointer;">Ownership Type</th>
                                            <th class="fixed-col" data-sort="float" style="cursor:pointer;">Max Inventory Value</th>
                                            <th class="fixed-col" data-sort="string" style="cursor:pointer;">Max Inventory Value Price Level Basis</th>
                                            <th class="fixed-col" data-sort="string" style="cursor:pointer;">Default Selling Price Level</th>
                                            <th class="fixed-col" data-sort="string" style="cursor:pointer;">PO Price Level</th>
                                            <th class="fixed-col" data-sort="string" style="cursor:pointer;">PO Exclusive to Principal</th>
                                            <th class="fixed-col" data-sort="string" style="cursor:pointer;">Remove FOWST</th>
                                        </tr>
                                    </thead>
                                    <tbody id="tblFOWSTListBody"></tbody>
                                </table>
                            </div>
                        </div>                        
                    </div>

                    <div class="tab-pane fade" id="tabGroupOrderCutOff">
                        <div class="row mb-3">
                            <label class="col-md-2 col-form-label" for="selSchedule">Cut-Off Type:</label>
                            <div class="col-md-10">
                                <select id="selSchedule" data-role="none" class="form-select">
                                    <option id="optDaily">Daily</option>
                                    @*<option id="optWeekly">Weekly</option>
                    <option id="optBiWeekly">Every 2 Weeks (BiWeekly)</option>*@
                                    @*<option>Every 4 Weeks</option>*@
                                </select>
                            </div>
                        </div>

                        <div class="row mb-3">
                            <label class="col-md-2 col-form-label" for="txtScheduleName">Cut-Off Name:</label>
                            <div class="col-md-10">
                                <input type="text" id="txtScheduleName" data-role="none" class="form-control" />
                            </div>
                        </div>

                        <div class="row mb-3">
                            <label class="col-md-2 col-form-label" for="selScheduleDay">Cut-Off Day:</label>
                            <div class="col-md-10">
                                <select id="selScheduleDay" data-role="none" class="form-select">
                                    <option id="optMonday">Monday</option>
                                    <option id="optTuesday">Tuesday</option>
                                    <option id="optWednesday">Wednesday</option>
                                    <option id="optThursday">Thursday</option>
                                    <option id="optFriday">Friday</option>
                                    <option id="optSaturday">Saturday</option>
                                    <option id="optSunday">Sunday</option>
                                    @*<option>Every 4 Weeks</option>*@
                                </select>
                            </div>
                        </div>

                        <div class="row mb-3">
                            <label class="col-md-2 col-form-label" for="selScheduleDay">Time:</label>
                            <div class="col-md-10">
                                <div class="input-group">
                                    <select id="selHourFrom" data-role="none" class="form-select">
                                        <option value=""> hh:mm </option>
                                        <option value="01:00"> 1:00 </option>
                                        <option value="01:15"> 1:15 </option>
                                        <option value="01:30"> 1:30 </option>
                                        <option value="01:45"> 1:45 </option>

                                        <option value="02:00"> 2:00 </option>
                                        <option value="02:15"> 2:15 </option>
                                        <option value="02:30"> 2:30 </option>
                                        <option value="02:45"> 2:45 </option>

                                        <option value="03:00"> 3:00 </option>
                                        <option value="03:15"> 3:15 </option>
                                        <option value="03:30"> 3:30 </option>
                                        <option value="03:45"> 3:45 </option>

                                        <option value="04:00"> 4:00 </option>
                                        <option value="04:15"> 4:15 </option>
                                        <option value="04:30"> 4:30 </option>
                                        <option value="04:45"> 4:45 </option>

                                        <option value="05:00"> 5:00 </option>
                                        <option value="05:15"> 5:15 </option>
                                        <option value="05:30"> 5:30 </option>
                                        <option value="05:45"> 5:45 </option>

                                        <option value="06:00"> 6:00 </option>
                                        <option value="06:15"> 6:15 </option>
                                        <option value="06:30"> 6:30 </option>
                                        <option value="06:45"> 6:45 </option>

                                        <option value="07:00"> 7:00 </option>
                                        <option value="07:15"> 7:15 </option>
                                        <option value="07:30"> 7:30 </option>
                                        <option value="07:45"> 7:45 </option>

                                        <option value="08:00"> 8:00 </option>
                                        <option value="08:15"> 8:15 </option>
                                        <option value="08:30"> 8:30 </option>
                                        <option value="08:45"> 8:45 </option>

                                        <option value="09:00"> 9:00 </option>
                                        <option value="09:15"> 9:15 </option>
                                        <option value="09:30"> 9:30 </option>
                                        <option value="09:45"> 9:45 </option>

                                        <option value="10:00"> 10:00 </option>
                                        <option value="10:15"> 10:15 </option>
                                        <option value="10:30"> 10:30 </option>
                                        <option value="10:45"> 10:45 </option>

                                        <option value="11:00"> 11:00 </option>
                                        <option value="11:15"> 11:15 </option>
                                        <option value="11:30"> 11:30 </option>
                                        <option value="11:45"> 11:45 </option>

                                        <option value="12:00"> 12:00 </option>
                                        <option value="12:15"> 12:15 </option>
                                        <option value="12:30"> 12:30 </option>
                                        <option value="12:45"> 12:45 </option>
                                    </select>
                                    <select id="selDayOfHour" data-role="none" class="form-select">
                                        <option value=""> -- </option>
                                        <option value="AM"> AM </option>
                                        <option value="PM"> PM </option>
                                    </select>
                                </div>
                            </div>
                        </div>

                        <div class="row mb-5">
                            <div class="offset-md-2 col-md-10">
                                <button data-role="none" class="btn btn-secondary btn-block" id="btnAddCutOffToList">Add Cut-Off Order</button>
                            </div>
                        </div>

                        <div data-role="none" class="list-group mb-3" id="lstSchedule"></div>
                    </div>
                </div>
            </div>
        </div>

        <div id="panMaxInventoryValue" class="modal fade" tabindex="-1">
            <div class="modal-dialog modal-lg modal-dialog-centered">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">FOWST</h5>
                        <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                    </div>
                    <div class="modal-body">
                        <div class="row mb-3">
                            <label class="col-md-3 col-form-label" for="txtSpecificMaxInventoryValue">Max Inventory Value:</label>
                            <div class="col-md-9">
                                <input type="text" id="txtSpecificMaxInventoryValue" data-role="none" class="form-control" />
                            </div>
                        </div>

                        <div class="row mb-3">
                            <label class="col-md-3 col-form-label" for="selSpecificInventoryValuePriceLevel">Max Inventory Value Price Level:</label>
                            <div class="col-md-9">
                                <select id="selSpecificInventoryValuePriceLevel" data-role="none" class="form-select">
                                    <option id="optCOGS">COGS</option>
                                    <option id="optRetailPrice">Retail Price</option>
                                </select>
                            </div>
                        </div>

                        <div class="row mb-3">
                            <label class="col-md-3 col-form-label" for="selSpecificApplyToAllDefaultSellingPrice">Default Selling Price Level:</label>
                            <div class="col-md-9">
                                <select id="selSpecificApplyToAllDefaultSellingPrice" data-role="none" class="form-select"></select>
                            </div>
                        </div>

                        <div class="row mb-3">
                            <label class="col-md-3 col-form-label" for="optSpecificIsExclusivePO">PO Products are exclusive to Principal:</label>
                            <div class="col-md-9">
                                <div class="form-check form-switch mb-3" style="font-size:20px;">
                                    <input class="form-check-input" data-role="none" type="checkbox" id="optSpecificIsExclusivePO">
                                </div>
                            </div>
                        </div>

                        <div class="row mb-3">
                            <label class="col-md-3 col-form-label" for="selSpecificApplyToAllDefaultPOPrice">PO Price Level(This is the default acquisition price from the principal):</label>
                            <div class="col-md-9">
                                <select id="selSpecificApplyToAllDefaultPOPrice" data-role="none" class="form-select"></select>
                            </div>
                        </div>

                        <div class="row mb-3">
                            <label class="col-md-3 col-form-label" for="selOwnershipType">Ownership Type:</label>
                            <div class="col-md-9">
                                <select id="selOwnershipType" data-role="none" class="form-select"></select>
                            </div>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <button data-role="none" type="button" id="btnSpecificDone" class="btn btn-success">Done</button>
                    </div>
                </div>
            </div>
        </div>

        <div class="fixed-bottom text-center pb-4">
            <button data-role="none" id="btnOrgGroupCancel" class="btn btn-secondary btn-lg">Cancel</button>
            <button data-role="none" id="btnOrgGroupSave" class="btn btn-success btn-lg">Save</button>
        </div>
    </div>
</div>