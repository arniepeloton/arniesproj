<script>
    require([GetPathFormat('/Scripts/sources/CRMDashboard/Settings.js?crc=')
            ]);
</script>

<div>
    <div class="popup-md" id="popNewScript" custom-data-role="popup">
        <div class="popup-header">
            New Script
            <a class="popup-close">x</a>
        </div>

        <div class="popup-content">
            <input type="text" data-role="none" style="display:none;" id="txtScriptID" />
            <span style="font-size:12px;">Fields marked with <span style="color:#F00;">*</span> are required.</span>
            <br /><br />
            <div class="custom-ui-label">
                <div>Name: <span style="color:#F00;">*</span></div>
                <div><input type="text" id="txtScriptName" /></div>                            
            </div>
            <div class="custom-ui-label">
                <div>Language: <span style="color:#F00;">*</span></div>
                <div>
                    <select id="selScriptLanguage">
                        <option value="">Please Select</option>
                        <option value="English">English</option>
                        <option value="Tagalog">Tagalog</option>
                        <option>--Add New Language--</option>
                    </select>
                </div>                            
            </div>
            <div class="custom-ui-label">
                <div>Purpose:</div>
                <div><textarea id="txtScriptPurpose" style="height:100px;"></textarea></div>                            
            </div>
            <div class="custom-ui-label">
                <div>Script: <span style="color:#F00;">*</span></div>
                <div><textarea id="txtScript" style="height:100px;"></textarea></div>                            
            </div>            
        </div>

        <div class="popup-footer align-right">
            <div style="text-align:center;">
                <button id="btnCancelNewScripts" data-role="none" class="btn custom-btn-theme-c btnCircle">
                    <img alt="Options" src="/Content/images/arrow-left.png" />
                </button>
                <button id="btnOkNewScripts" title="Ok" data-role="none" class="btn custom-btn-theme-b btnCircle">
                    <img alt="Options" src="/Content/images/check.png" />
                </button>
            </div>
        </div>
    </div>
</div>

<div id="pageSettingForm" subpage="form">
    <div data-role="content" class="ui-content ui-body-d" role="main">
        <div style="overflow:auto;">
            <table data-role="table" data-role="reflow" style="margin-right:5px; display:table;" class="ui-table ui-responsive table-stroke table-stripe custom-tbl-responsive">
                <thead>
                    <tr class="custom-theme-f">
                        <th data-sort="accounting" style="cursor:pointer;"> # </th>
                        <th data-sort="string" style="cursor:pointer;"> Form Name </th>
                        <th data-sort="string" style="cursor:pointer;"> Description </th>
                        <th data-sort="string" style="cursor:pointer;"> Modified By </th>
                        <th data-sort="string" style="cursor:pointer;"> Modified Date </th>
                    </tr>
                </thead>
                <tbody id="tblFormbody">
                </tbody>
            </table>
        </div>

        <div class="fixed-ActionBtn">
            <button id="btnAddForm" data-role="none" class="btn custom-btn-theme-d btnCircle">
                <img alt="Add" src="/Content/images/plus-white.png" style="width:18px;" />
            </button>
        </div>
    </div>    
</div>

<div id="pageSettingNewForm" subpage="form" style="display:none;">
    <div data-role="content" class="ui-content ui-body-d" role="main">
        <div class="custom-ui-label">
            <div>Form Name:</div>
            <div><input type="text" id="txtNewFormName" /></div>
        </div>

        <div class="custom-ui-label">
            <div>Form Description:</div>
            <div><textarea id="txtNewFormDescription" style="height:100px;"></textarea></div>
        </div>

        <div class="panel">
            <div class="panel-title custom-theme-h">
                Form Scripts
            </div>

            <div class="panel-content txt-left">
                <div id="divAddScriptsHome" style="display:block;">
                    <div data-role="fieldcontain" style="text-align:right;">
                        <button id="btnAddScript" data-role="none" class="btn custom-btn-theme-b btnCircle">
                            <img alt="Add" src="/Content/images/plus-white.png" style="width:18px;" />
                        </button>
                    </div>
                    <div style="padding: 8px 15px 0px 15px;">
                        <ul data-role="listview" id="lstFormScript" class="li-theme-c noBorderList custom-list custom-list-sm">
                        </ul>
                    </div>
                    <br />
                </div>
            </div>
        </div>
        <br /><br />
        <div id="">
            Choose field that you want to display in your form table:
            <br />
            <span style="font-size:12px; display:none;"><b>NOTE:</b> Maximum of 10 fields only</span>
            <br />
            <div class="ui-grid-a">
                <div class="ui-block-a">
                    <label for="chbOrgName">
                        Company Name
                        <input type="checkbox" id="chbOrgName" name="chbProspectingTable" />
                    </label>
                    <label for="chbBusinessName">
                        Company Business Name
                        <input type="checkbox" id="chbBusinessName" name="chbProspectingTable" />
                    </label>
                    <label for="chbBranchName">
                        Company Branch Name
                        <input type="checkbox" id="chbBranchName" name="chbProspectingTable" />
                    </label>
                    <label for="chbOrgAddress">
                        Company Address
                        <input type="checkbox" id="chbOrgAddress" name="chbProspectingTable" />
                    </label>
                    <label for="chbTelephoneNo">
                        Company Telephone No
                        <input type="checkbox" id="chbTelephoneNo" name="chbProspectingTable" />
                    </label>
                    <label for="chbMobileNo">
                        Company Mobile No
                        <input type="checkbox" id="chbMobileNo" name="chbProspectingTable" />
                    </label>
                    <label for="chbEmailAddress">
                        Company Email Address
                        <input type="checkbox" id="chbEmailAddress" name="chbProspectingTable" />
                    </label>
                    <label for="chbFaxNo">
                        Company Fax No
                        <input type="checkbox" id="chbFaxNo" name="chbProspectingTable" />
                    </label>                        
                </div>
                <div class="ui-block-b">
                    <label for="chbWebsite">
                        Company Website
                        <input type="checkbox" id="chbWebsite" name="chbProspectingTable" />
                    </label>
                    <label for="chbBusinessScope">
                        Company Business Scope
                        <input type="checkbox" id="chbBusinessScope" name="chbProspectingTable" />
                    </label>
                    <label for="chbContactPerson">
                        Company Contact Person
                        <input type="checkbox" id="chbContactPerson" name="chbProspectingTable" />
                    </label>
                </div>
            </div>
        </div>

        <div class="floatingDiv-bottomcenter">
            <button data-role="none" id="btnCancelNewForm" class="btn custom-btn-theme-c btnCircle btnShadow">
                <img alt="Cancel" src="/Content/images/arrow-left.png" style="width:18px;" />
            </button>
            <button data-role="none" id="btnSaveNewForm" class="btn custom-btn-theme-d btnCircle btnShadow">
                <img alt="Save" src="/Content/images/save.png" style="width:18px;" />
            </button>
        </div>
    </div>

    <div data-role="content" class="ui-content ui-body-d" role="main" style="display:none;">
        <div style="width:100%;">
            <div style="width:19%; float:left;">
                <div style="background-color:#F7F7F7;">
                    <ul class="custom-list-menu txt-left">
                        <li id="btnSettingForms"><a class="active-menu">Forms</a></li>
                        <li id="btnSettingUserScripts"><a>User Scripts</a></li>
                    </ul>
                </div>
            </div>
            <div style="width:79%; float:right;">
                <div custom-data-role="page" subpage="1" id="divSettingForms">
                    <label style="font-size:2em; font-weight:bold;">Forms</label>
                    <br /><br />

                    Choose field that you want to display in your prospecting table:
                    <br />
                    <span style="font-size:12px; display:none;"><b>NOTE:</b> Maximum of 10 fields only</span>
                    <br />
                    <div class="ui-grid-a">
                        <div class="ui-block-a">
                            <label for="chbOrgName">
                                Company Name
                                <input type="checkbox" id="chbOrgName" name="chbProspectingTable" />
                            </label>
                            <label for="chbBusinessName">
                                Company Business Name
                                <input type="checkbox" id="chbBusinessName" name="chbProspectingTable" />
                            </label>
                            <label for="chbBranchName">
                                Company Branch Name
                                <input type="checkbox" id="chbBranchName" name="chbProspectingTable" />
                            </label>
                            <label for="chbOrgAddress">
                                Company Address
                                <input type="checkbox" id="chbOrgAddress" name="chbProspectingTable" />
                            </label>
                            <label for="chbTelephoneNo">
                                Company Telephone No
                                <input type="checkbox" id="chbTelephoneNo" name="chbProspectingTable" />
                            </label>
                            <label for="chbMobileNo">
                                Company Mobile No
                                <input type="checkbox" id="chbMobileNo" name="chbProspectingTable" />
                            </label>
                            <label for="chbEmailAddress">
                                Company Email Address
                                <input type="checkbox" id="chbEmailAddress" name="chbProspectingTable" />
                            </label>
                            <label for="chbFaxNo">
                                Company Fax No
                                <input type="checkbox" id="chbFaxNo" name="chbProspectingTable" />
                            </label>                        
                        </div>
                        <div class="ui-block-b">
                            <label for="chbWebsite">
                                Company Website
                                <input type="checkbox" id="chbWebsite" name="chbProspectingTable" />
                            </label>
                            <label for="chbBusinessScope">
                                Company Business Scope
                                <input type="checkbox" id="chbBusinessScope" name="chbProspectingTable" />
                            </label>
                            <label for="chbContactPerson">
                                Company Contact Person
                                <input type="checkbox" id="chbContactPerson" name="chbProspectingTable" />
                            </label>
                        </div>
                    </div>
                    <br />
                    <div style="float:right;">
                        <button data-role="none" class="btn custom-btn-theme-b btn-less-width">Save Form</button>
                    </div>
                </div>

                <div custom-data-role="page" subpage="1" id="divSettingUserScripts" style="display:none;">
                    <div id="divMainScripts" subpage="1.2">
                        <label style="font-size:2em; font-weight:bold;">User Scripts</label>
                        <br />
                        <br />
                        <div style="overflow:auto;">
                            <table data-role="table" data-role="reflow" style="margin-right:5px; display:table;" class="ui-table ui-responsive table-stroke table-stripe custom-tbl-responsive">
                                <thead>
                                    <tr class="custom-theme-f">
                                        <th data-sort="string" style="cursor:pointer;"> Name </th>
                                        <th data-sort="string" style="cursor:pointer;"> Purpose </th>
                                        <th data-sort="string" style="cursor:pointer;"> Language </th>
                                        <th data-sort="string" style="cursor:pointer;"> Modified By </th>
                                        <th data-sort="date" style="cursor:pointer;"> Modified Date </th>
                                    </tr>
                                </thead>
                                <tbody id="tblScripts"></tbody>
                            </table>
                        </div>

                        <div id="btnAddScripts" class="floatingButton custom-btn-theme-d" title="Options"><img src="/Content/images/plus-white.png" alt="Add"></div>
                    </div>

                    <div id="divAddScripts" subpage="1.2" style="display:none;">
                        <label style="font-size:2em; font-weight:bold;">Add Script</label>
                        <br /><br />
                        <div data-role="field-contain">
                            <label for="txtScriptName">Name:</label>
                            <input type="text" id="txtScriptName2" />
                        </div>
                        <div data-role="field-contain">
                            <label for="selScriptLanguage2">Language:</label>
                            <select id="selScriptLanguage2">                                
                                <optgroup label="Choose a Language">
                                    <option>English</option>
                                    <option>Tagalog</option>
                                    <option>--Add New Language--</option>
                                </optgroup>
                            </select>
                        </div>
                        <div data-role="field-contain">
                            <label for="txtScript2">Script:</label>
                            <textarea id="txtScript2" style="height:100px;"></textarea>
                        </div>
                        <div data-role="field-contain">
                            <label for="txtScriptPurpose2">Purpose:</label>
                            <textarea id="txtScriptPurpose2"></textarea>
                        </div>
                        <br />
                        <div style="float:right;">
                            <button data-role="none" id="btnCancelScript2" class="btn custom-btn-theme-c btn-less-width">Cancel</button>
                            <button data-role="none" id="btnSaveScript2" class="btn custom-btn-theme-b btn-less-width">Save Script</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>   
</div>
