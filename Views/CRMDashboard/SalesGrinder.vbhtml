@*@Code
    ViewData("Title") = "CRM Dashboard"
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code
@Section head
    <link rel="stylesheet" href="/Content/flatui-css-custom.css" />
    <link rel="stylesheet" href="/Content/jquery.dataTables2.css" />
    <script src=""></script>
    <script src="/Scripts/sources/CRMDashboard/customcalendar.js"></script>
    <script src="/Scripts/sources/crm/qrcodeNew.js"></script>
End Section*@
<script>
    require([GetPathFormat('/Scripts/sources/CRMDashboard/CRMDashboard.js?crc='),
            GetPathFormat('/Scripts/sources/CRMDashboard/SmSUsage.js?crc=')
            ]);
</script>

<div id="divLoading" style="display:block; padding:10px;">Loading, please wait ...</div>

<div id="divMainPage" style="display:none">

    @*Start Pop Ups*@
    <div>
        <div data-role="popup" id="popFilterBy" data-overlay-theme="a" data-theme="a" data-corners="false"> 
            <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
            <center><h3>Filter By</h3></center>
            <div data-role="content" class="ui-content ui-body-d">
                <div style="padding: 8px 15px 16px 15px;">
                    <ul data-role="listview" class="li-theme-c noBorderList custom-list custom-list-sm">
                        <li id="li2DaysAgo"><a>as</a></li>
                    </ul>
                </div>
                <br />

                <button data-role="none" id="btnOkContact2" class="btn custom-btn-theme-b">Ok</button>
                <br /><br />
                <button data-role="none" id="btnCancelContact2" class="btn custom-btn-theme-c">Cancel</button>
            </div>       
        </div>

        <div class="popup-md" id="popContactPersonProfile" custom-data-role="popup">
            <div class="popup-header">
                Person Profile
                <a class="popup-close">x</a>
            </div>

            <div class="popup-content">
                <input type="text" id="txtPeopleID" style="display:none;" data-role="none" />
                <input type="text" id="txtContactPersonID" style="display:none;" data-role="none" />

                <div class="custom-ui-label">
                    <div>Full Name:</div>
                    <div>
                        <input type="text" id="txtContactFirstName" placeholder="Firstname" />
                        <span id="spanerrorContactFname" style="color:#F00; display:none;">This field is required<br /><br /></span>
                        <input type="text" id="txtContactMiddleName" placeholder="Middlename" />
                        <input type="text" id="txtContactLastName" placeholder="Lastname" />
                    </div>
                </div>

                <div class="custom-ui-label">
                    <div>Position/Title: </div>
                    <div>
                        <input type="text" id="txtContactPersonPosition" />
                    </div>
                </div>

                <div class="custom-ui-label">
                    <div>Position/Title Description: </div>
                    <div>
                        <input type="text" id="txtContactPersonPositionDescription" />
                    </div>
                </div>
                <br />

                <div class="panel">
                    <div class="panel-title custom-theme-h">
                        Address
                    </div>

                    <div class="panel-content txt-left">
                        <input data-role="none" type="text" id="txtContactPersonAddressID" style="display:none;" />
                        <div class="custom-ui-label">
                            <div>House or Building No:</div>
                            <div><input type="text" id="txtContactPersonHouseBuildingNo" /></div>
                        </div>

                        <div class="custom-ui-label">
                            <div>Street:</div>
                            <div><input type="text" id="txtContactPersonStreet" /></div>
                        </div>

                        <div class="custom-ui-label">
                            <div>City Or Town:</div>
                            <div><select id="selContactPersonCity"></select></div>
                        </div>

                        <div class="custom-ui-label">
                            <div>Country:</div>
                            <div><select id="selContactPersonCountry"></select></div>
                        </div>

                        <div class="custom-ui-label">
                            <div>Type of Address:</div>
                            <div>
                                <select id="selContactPersonTypeAddress">
                                    <option value="Permanent Address">Permanent Address</option>
                                    <option value="Non-Permanent Address">Non-Permanent Address</option>
                                    <option value="Provincial Address">Provincial Address</option>
                                </select>
                            </div>
                        </div>
                    </div>
                </div>

                <br /><br />
                <label style="font-size:1.5em; font-weight:bold;">Person Contact Info</label>
                <br /><br />
                <div class="panel">
                    <div class="panel-title custom-theme-h">
                        Telephone Number
                    </div>

                    <div class="panel-content txt-left">
                        <div data-role="fieldcontain">
                            <div id="divContactPersonTel" subpage="contactpersontelpage" style="display:block;">
                                <div data-role="fieldcontain" style="text-align:right;">
                                    <button id="btnAddContactPersonTelephone" data-role="none" class="btn custom-btn-theme-b btnCircle">
                                        <img alt="Add" src="/Content/images/plus-white.png" style="width:18px;" />
                                    </button>
                                </div>
                                <div style="padding: 8px 15px 0px 15px;">
                                    <ul data-role="listview" id="lstContactPersonTelephone" class="li-theme-c noBorderList custom-list custom-list-sm">
                                    </ul>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <br /><br /><br />
                <div class="panel">
                    <div class="panel-title custom-theme-h">
                        Mobile Number
                    </div>

                    <div class="panel-content txt-left">
                        <div data-role="fieldcontain">
                            <div id="divContactPersonMobileHome" subpage="contactpersonmobilepage" style="display:block;">
                                <div data-role="fieldcontain" style="text-align:right;">
                                    <button id="btnAddContactPersonMobile" data-role="none" class="btn custom-btn-theme-b btnCircle">
                                        <img alt="Add" src="/Content/images/plus-white.png" style="width:18px;" />
                                    </button>
                                </div>
                                <div style="padding: 8px 15px 0px 15px;">
                                    <ul data-role="listview" id="lstContactPersonMobile" class="li-theme-c noBorderList custom-list custom-list-sm">
                                    </ul>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <br /><br /><br />
                <div class="panel">
                    <div class="panel-title custom-theme-h">
                        Email Address
                    </div>

                    <div class="panel-content txt-left">
                        <div data-role="fieldcontain">
                            <div id="divContactPersonEmailHome" subpage="contactpersonemailpage" style="display:block;">
                                <div data-role="fieldcontain" style="text-align:right;">
                                    <button id="btnAddContactPersonEmail" data-role="none" class="btn custom-btn-theme-b btnCircle">
                                        <img alt="Add" src="/Content/images/plus-white.png" style="width:18px;" />
                                    </button>
                                </div>
                                <div style="padding: 8px 15px 0px 15px;">
                                    <ul data-role="listview" id="lstContactPersonEmail" class="li-theme-c noBorderList custom-list custom-list-sm">
                                    </ul>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <div class="popup-footer align-right">
                <div style="text-align:right;">
                    <button id="btnCancelContact" data-role="none" class="btn custom-btn-theme-c btnCircle">
                        <img alt="Options" src="/Content/images/close-black.png" />
                    </button>
                    <button id="btnOkContact" data-role="none" class="btn custom-btn-theme-b btnCircle">
                        <img alt="Options" src="/Content/images/check.png" />
                    </button>
                </div>
            </div>
        </div>

        <div class="popup" id="popSMSLogs" custom-data-role="popup">
            <div class="popup-header">
                SMS Logs
                <a class="popup-close">x</a>
            </div>

            <div class="popup-content">
                <table data-role="table" data-role="reflow" style="margin-right:5px; display:table;" class="ui-table ui-responsive table-stroke table-stripe custom-tbl-responsive">
                    <thead>
                        <tr class="custom-theme-f">
                           
                            <th style="cursor:pointer;width:50%">  </th>
                            <th style="cursor:pointer;text-align:right; width:16%"> Device To Modem </th>
                            <th style="cursor:pointer;text-align:right; width:16%"> Modem To Telco </th>
                            <th style="cursor:pointer;text-align:right; width:18%"> </th>

                        </tr>
                    </thead>
                    <tbody id="tblSmSUsagebody"></tbody>
                </table>

                <br /> <br /> 

                <table id="dataTableSMSLogs" data-role="table" data-role="reflow" style="margin-right:5px; display:table;" class="ui-table ui-responsive table-stroke table-stripe custom-tbl-responsive">
                    <thead>
                        <tr class="custom-theme-f">
                           
                            <th style="cursor:pointer; width:200px;"> Sent To </th>
                            <th style="cursor:pointer;text-align:left; width:300px;"> SMS Message </th>
                            <th style="cursor:pointer;text-align:left"> Date and Time </th>
                            <th style="cursor:pointer;text-align:left"> Sent By </th>

                        </tr>
                    </thead>
                    <tbody id="tblSmSUsageDetailbody"></tbody>
                </table>
            </div>

            <div class="popup-footer align-right">
                <div style="text-align:right;">
                    <button data-role="none" class="popup-close btn custom-btn-theme-c btnCircle">
                        <img alt="Options" src="/Content/images/close-black.png" />
                    </button>
                </div>
            </div>
        </div>

        <div class="popup" id="popInteractionHistoryDetails" custom-data-role="popup">
            <div class="popup-header">
                Interaction Details
                <a class="popup-close">x</a>
            </div>

            <div class="popup-content">
                <div class="ui-grid-a ui-responsive">
                    <div class="ui-block-a">
                        <div class="custom-ui-label">
                            <div>Client Name:</div>
                            <div id="txtInteractionHistoryDetailsClientName" style="text-transform:capitalize"></div>
                        </div>
                        <div class="custom-ui-label">
                            <div>Called By:</div>
                            <div id="txtInteractionHistoryDetailsCalledBy" style="text-transform:capitalize"></div>
                        </div>
                        <div class="custom-ui-label">
                            <div>Call Duration:</div>
                            <div id="txtInteractionHistoryDetailsCallDuration"></div>
                        </div>
                    </div>
                    <div class="ui-block-b">
                        <div class="custom-ui-label">
                            <div>Start:</div>
                            <div id="txtInteractionHistoryDetailsStart"></div>
                        </div>
                        <div class="custom-ui-label">
                            <div>End:</div>
                            <div id="txtInteractionHistoryDetailsEnd"></div>
                        </div>
                    </div>
                </div>

                <br />
                <div>
                    <label>Notes</label>
                    <br /><br />
                    <div id="txtInteractionHistoryDetailsNotes" style="padding:10px; border:1px solid #CCC; border-radius:5px; background-color:#F0F0F0;">
                    
                    </div>
                </div>
            </div>

            <div class="popup-footer align-right">
                <div style="text-align:center;">
                    <button id="btnInteractionHistoryDetailsOk" title="Ok" data-role="none" class="btn custom-btn-theme-b btnCircle">
                        <img alt="Options" src="/Content/images/check.png" />
                    </button>
                </div>
            </div>
        </div>

        <div id="divtooltipNotes" class="tooltip-window">
            <div style="color:#CCC;">Date and Time: <span id="tooltipDateAndTime" style="color:#FFF;"></span></div>
            <br />
            <div class="tooltip-content">
                <span style="color:#CCC;">Notes:</span>
                <br />
                <div id="tooltipNotes" style="padding: 10px;"></div>
            </div>
            <div style="text-align:right;">
                <button data-role="none" class="btn custom-btn-theme-c btn-less-width btnCircle-sm noshadow btnCloseToolTip"><img src="/Content/images/close-black.png" alt="Close" /></button>
            </div>
        </div>

        <div class="popup-md" id="popNewToDo" custom-data-role="popup">
            <div class="popup-header">
                New To-Do
                <a class="popup-close">x</a>
            </div>

            <div class="popup-content">
            
                <span style="font-size:12px;">Fields marked with <span style="color:#F00;">*</span> are required.</span>
                <br /><br />
            
                <div class="custom-ui-label">
                    <div>To-Do: <span style="color:#F00;">*</span> <br /> <span id="spanerrorToDo" style="color:#F00; display:none;">This field is required</span></div>
                    <div><textarea id="" style="height:100px; max-height:100px; overflow:auto;"></textarea></div>                            
                </div>
                <div class="custom-ui-label">
                    <div>Due Date:</div>
                    <div><button id="btnToDoSetDate2">Set Date</button></div>                            
                </div>            
            </div>

            <div class="popup-footer align-right">
                <div style="text-align:right;">
                    <button id="btnCancelToDo" data-role="none" class="btn custom-btn-theme-c btnCircle">
                        <img alt="Options" src="/Content/images/arrow-left.png" />
                    </button>
                    <button id="" title="Ok" data-role="none" class="btn custom-btn-theme-b btnCircle">
                        <img alt="Options" src="/Content/images/check.png" />
                    </button>
                </div>
            </div>
        </div>

        <div data-role="popup" id="popToDo" data-overlay-theme="a" data-theme="a" data-corners="false"> 
            <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
            <center><h3>Edit To-Do</h3></center>
            <div data-role="content" class="ui-content ui-body-d">
                <input type="text" data-role="none" style="display:none;" id="txtToDoID" />
                <div class="custom-ui-label">
                    <div>To-Do Name:</div>
                    <div><input type="text" id="txtEditToDo" placeholder="To-Do Name" /></div>
                </div>

                <div class="custom-ui-label">
                    <div>Due Date:</div>
                    <div><button id="btnEditToDoSetDate">Set Date</button></div>
                </div>

                <br />
                <button id="btnOkEditToDo" data-role="none" class="btn custom-btn-theme-b">Ok</button>
                <br /><br />
                <button id="btnCancelEditToDo" data-role="none" class="btn custom-btn-theme-c">Cancel</button>
            </div>       
        </div>

        <div id="divAffiliation" class="tooltip-window">
            <div style="color:#CCC;">Name: <span id="txtFormName" style="color:#FFF;"></span></div>
            <br />
            <div class="tooltip-content" id="divContentAffiliation">
                <ol id="listFormAffiliation" style="margin:0 auto;"></ol>
            </div>
            <div style="text-align:right;">
                <button data-role="none" class="btn custom-btn-theme-c btn-less-width btnCircle-sm noshadow btnCloseToolTip"><img src="/Content/images/close-black.png" alt="Close" /></button>
            </div>
        </div>

        <div class="popup-md" id="popPersonTelInfo" custom-data-role="popup">
            <div class="popup-header">
                Telephone Info
                <a class="popup-close">x</a>
            </div>

            <div class="popup-content">
                <div data-role="fieldcontain" style="display:none;">
                    <input type="text" readonly id="txtPersonTelCountryID" value="e783ed13-aa98-4cc1-918e-d49b05c0167e"/>
	                <input type="text" readonly id="txtPersonTelID"/>                
	                <input type="text" readonly id="txtPersonTelAreaCodeID"/>                
                </div>

                <div data-role="fieldcontain">
                    <fieldset data-role="controlgroup" data-type="horizontal">
                        <legend>Country Code: <span style="color: #FF0000;" >*</span>
                            <span id="spanerrorPersonTelCountryCode" style="color:#F00; display:none;">This field is required</span>
                        </legend>
                        <button id="btnPersonTelCountryCodeDisable" data-role="none" class="btn custom-btn-theme-c">
                            <span id="txtPersonCountryCode">PHL - 63</span>
                            <span class="ui-icon ui-icon-arrow-d icon-right"></span>
                        </button>                                    
                    </fieldset>
                </div>

                <div data-role="fieldcontain">
                    <fieldset data-role="controlgroup" data-type="horizontal">
                        <legend>Area Code: <span style="color: #FF0000;" >*</span>
                            <br /><br /> <span id="spanerrorPersonAreaCode" style="color:#F00; display:none;">This field is required</span>
                        </legend>
                        <button id="btnPersonTelAreaCode" data-role="none" class="btn custom-btn-theme-c">
                            <span id="txtPersonAreaCode">Select</span>
                            <span class="ui-icon ui-icon-arrow-d icon-right"></span>
                        </button>
                    </fieldset>
                </div>

                <div data-role="fieldcontain">
                    <label for="txtPersonTelephoneNum">Telephone Number: <span style="color:#F00">*</span></label>
                    <input type="text" id="txtPersonTelephoneNum" maxlength="7" />
                    <br />
                    <span id="spanerrorPersonTelephoneNum" style="color:#F00; display:none;">This field is required</span>
                    <span id="spanerrorPersonTelephoneNumValidate" style="color:#F00; display:none;">Enter 7 digit number</span>
                </div>

                <div data-role="fieldcontain">
                    <label for="txtPersonTelephoneExt">Extension or Local No.:</label>
                    <input type="text" id="txtPersonTelephoneExt" maxlength="10" />
                </div>

                <div data-role="fieldcontain">
	                <label for="selPersonTelType">Contact Type: <span style="color: #FF0000;" >*</span></label>
                    <select id="selPersonTelType">
	                    <option value="">Please Select</option>
	                    <option value="Work Telephone">Work Telephone</option>
				        <option value="Personal Telephone">Personal Telephone</option>
				        <option value="Third Party Telephone">Third Party Telephone</option>
                    </select>
                    <br />
                    <span id="spanerrorPersonTelType" style="color:#F00; display:none;">This field is required</span>
                </div>
            </div>

            <div class="popup-footer align-right">
                <div style="text-align:right;">
                    <button id="btnCancelPersonTelephone" data-role="none" class="btn custom-btn-theme-c btnCircle">
                        <img alt="Options" src="/Content/images/close-black.png" />
                    </button>
                    <button id="btnSavePersonTelephone" data-role="none" class="btn custom-btn-theme-b btnCircle">
                        <img alt="Options" src="/Content/images/check.png" />
                    </button>
                </div>
            </div>
        </div>

        <div class="popup-md" id="popPersonMobileInfo" custom-data-role="popup">
            <div class="popup-header">
                Mobile Info
                <a class="popup-close">x</a>
            </div>

            <div class="popup-content">
                <div data-role="fieldcontain" style="display:none;">
                    <input type="text" readonly id="txtPersonMobileCountryID" value="e783ed13-aa98-4cc1-918e-d49b05c0167e"/>
                    <input type="text" readonly id="txtPersonMobileID"/>	                            
	                <input type="text" readonly id="txtPersonTelCoPrefixID"/>                
                </div>

                <div data-role="fieldcontain">
                    <fieldset data-role="controlgroup" data-type="horizontal">
                        <legend>Country Code: <span style="color: #FF0000;" >*</span>
                            <br /><br /><span id="spanerrorPersonMobileCountryCode" style="color:#F00; display:none;">This field is required</span>
                        </legend>
                        <button id="btnPersonMobileCountryCodeDisable" data-role="none" class="btn custom-btn-theme-c">
                            <span id="txtPersonMobileCountryCode">PHL - 63</span>
                            <span class="ui-icon ui-icon-arrow-d icon-right"></span>
                        </button>                                    
                    </fieldset>
                </div>

                <div data-role="fieldcontain">
                    <fieldset data-role="controlgroup" data-type="horizontal">
                        <legend>Tel Code: <span style="color: #FF0000;" >*</span>
                            <br /><br /> <span id="spanerrorPersonMobileTelCode" style="color:#F00; display:none;">This field is required</span>
                        </legend>
                        <button id="btnPersonMobileTelCode" data-role="none" class="btn custom-btn-theme-c">
                            <span id="txtPersonTelCode">Select</span>
                            <span class="ui-icon ui-icon-arrow-d icon-right"></span>
                        </button>
                    </fieldset>
                </div>

                <div data-role="fieldcontain">
                    <label for="txtPersonMobileNo">Mobile Number: <span style="color:#F00">*</span></label>
                    <input type="text" id="txtPersonMobileNo" maxlength="7" />
                    <br />
                    <span id="spanerrorPersonMobileNum" style="color:#F00; display:none;">This field is required</span>
                    <span id="spanerrorPersonMobileNumValidate" style="color:#F00; display:none;">Enter 7 digit number</span>
                </div>

                <div data-role="fieldcontain">
	                <label for="selPersonMobileType">Contact Type: <span style="color: #FF0000;" >*</span></label>
                    <select id="selPersonMobileType">
	                    <option value="">Please Select</option>
	                    <option value="Work Mobile">Work Mobile</option>
				        <option value="Personal Mobile">Personal Mobile</option>
				        <option value="Third Party Mobile">Third Party Mobile</option>
                    </select>
                    <br />
                    <span id="spanerrorPersonMobileType" style="color:#F00; display:none;">This field is required</span>
                </div>
            </div>

            <div class="popup-footer align-right">
                <div style="text-align:right;">
                    <button id="btnCancelPersonMobile" data-role="none" class="btn custom-btn-theme-c btnCircle">
                        <img alt="Options" src="/Content/images/close-black.png" />
                    </button>
                    <button id="btnSavePersonMobile" data-role="none" class="btn custom-btn-theme-b btnCircle">
                        <img alt="Options" src="/Content/images/check.png" />
                    </button>
                </div>
            </div>
        </div>

        <div class="popup-md" id="popPersonEmailInfo" custom-data-role="popup">
            <div class="popup-header">
                Email Info
                <a class="popup-close">x</a>
            </div>

            <div class="popup-content">
                <div data-role="fieldcontain" style="display:none;">
                    <input type="text" readonly id="txtPersonEmailID"/>                
                </div>

                <div data-role="fieldcontain">
                    <label for="txtPersonEmailAdd">Email Address: <span style="color:#F00">*</span></label>
                    <input type="text" id="txtPersonEmailAdd" />
                    <br />
                    <span id="spanerrorPersonEmailAddress" style="color:#F00; display:none;">This field is required</span>
                    <span id="spanerrorPersonEmailAddressValidate" style="color:#F00; display:none;">Enter valid email address</span>
                </div>

                <div data-role="fieldcontain">
	                <label for="selPersonEmailType">Email Type: <span style="color: #FF0000;" >*</span></label>
                    <select id="selPersonEmailType">
	                    <option value="">Please Select</option>
	                    <option value="Work Email">Work Email</option>
				        <option value="Personal Email">Personal Email</option>
				        <option value="Third Party Email">Third Party Email</option>
                    </select>
                    <br />
                    <span id="spanerrorPersonEmailType" style="color:#F00; display:none;">This field is required</span>
                </div>
            </div>

            <div class="popup-footer align-right">
                <div style="text-align:right;">
                    <button id="btnCancelPersonEmailAddress" data-role="none" class="btn custom-btn-theme-c btnCircle">
                        <img alt="Options" src="/Content/images/close-black.png" />
                    </button>
                    <button id="btnSavePersonEmailAddress" data-role="none" class="btn custom-btn-theme-b btnCircle">
                        <img alt="Options" src="/Content/images/check.png" />
                    </button>
                </div>
            </div>
        </div>

        <div class="popup-md" id="popOrgTelInfo" custom-data-role="popup">
            <div class="popup-header">
                Telephone Info
                <a class="popup-close">x</a>
            </div>

            <div class="popup-content">
                <div data-role="fieldcontain" style="display:none;">
                    <input type="text" readonly id="txtOrgTelCountryID" value="e783ed13-aa98-4cc1-918e-d49b05c0167e"/>
	                <input type="text" readonly id="txtOrgTelID"/>                
	                <input type="text" readonly id="txtOrgTelAreaCodeID"/>                
                </div>

                <div data-role="fieldcontain">
                    <fieldset data-role="controlgroup" data-type="horizontal">
                        <legend>Country Code: <span style="color: #FF0000;" >*</span>
                            <span id="spanerrorTelCountryCode" style="color:#F00; display:none;">This field is required</span>
                        </legend>
                        <button id="btnOrgTelCountryCodeDisable" data-role="none" class="btn custom-btn-theme-c">
                            <span id="txtCountryCode">PHL - 63</span>
                            <span class="ui-icon ui-icon-arrow-d icon-right"></span>
                        </button>                                    
                    </fieldset>
                </div>

                <div data-role="fieldcontain">
                    <fieldset data-role="controlgroup" data-type="horizontal">
                        <legend>Area Code: <span style="color: #FF0000;" >*</span>
                            <br /><br /> <span id="spanerrorAreaCode" style="color:#F00; display:none;">This field is required</span>
                        </legend>
                        <button id="btnOrgTelAreaCode" data-role="none" class="btn custom-btn-theme-c">
                            <span id="txtAreaCode">Select</span>
                            <span class="ui-icon ui-icon-arrow-d icon-right"></span>
                        </button>
                    </fieldset>
                </div>

                <div data-role="fieldcontain">
                    <label for="txtTelephoneNum">Telephone Number: <span style="color:#F00">*</span></label>
                    <input type="text" id="txtTelephoneNum" maxlength="7" />
                    <br />
                    <span id="spanerrorTelephoneNum" style="color:#F00; display:none;">This field is required</span>
                    <span id="spanerrorTelephoneNumValidate" style="color:#F00; display:none;">Enter 7 digit number</span>
                </div>

                <div data-role="fieldcontain">
                    <label for="txtTelephoneExt">Extension or Local No.:</label>
                    <input type="text" id="txtTelephoneExt" maxlength="10" />
                </div>

                <div data-role="fieldcontain">
	                <label for="selOrgTelType">Contact Type: <span style="color: #FF0000;" >*</span></label>
                    <select id="selOrgTelType">
	                    <option value="">Please Select</option>
	                    <option value="Work Telephone">Work Telephone</option>
				        <option value="Personal Telephone">Personal Telephone</option>
				        <option value="Third Party Telephone">Third Party Telephone</option>
                    </select>
                    <br />
                    <span id="spanerrorTelType" style="color:#F00; display:none;">This field is required</span>
                </div>
            </div>

            <div class="popup-footer align-right">
                <div style="text-align:right;">
                    <button id="btnCancelOrgTelephone" data-role="none" class="btn custom-btn-theme-c btnCircle">
                        <img alt="Options" src="/Content/images/close-black.png" />
                    </button>
                    <button id="btnSaveOrgTelephone" data-role="none" class="btn custom-btn-theme-b btnCircle">
                        <img alt="Options" src="/Content/images/check.png" />
                    </button>
                </div>
            </div>
        </div>

        <div class="popup-md" id="popOrgMobileInfo" custom-data-role="popup">
            <div class="popup-header">
                Mobile Info
                <a class="popup-close">x</a>
            </div>

            <div class="popup-content">
                <div data-role="fieldcontain" style="display:none;">
                    <input type="text" readonly id="txtOrgMobileCountryID" value="e783ed13-aa98-4cc1-918e-d49b05c0167e"/>
                    <input type="text" readonly id="txtOrgMobileID"/>	                            
	                <input type="text" readonly id="txtOrgTelCoPrefixID"/>                
                </div>

                <div data-role="fieldcontain">
                    <fieldset data-role="controlgroup" data-type="horizontal">
                        <legend>Country Code: <span style="color: #FF0000;" >*</span>
                            <br /><br /><span id="spanerrorMobileCountryCode" style="color:#F00; display:none;">This field is required</span>
                        </legend>
                        <button id="btnOrgMobileCountryCodeDisable" data-role="none" class="btn custom-btn-theme-c">
                            <span id="txtMobileCountryCode">PHL - 63</span>
                            <span class="ui-icon ui-icon-arrow-d icon-right"></span>
                        </button>                                    
                    </fieldset>
                </div>

                <div data-role="fieldcontain">
                    <fieldset data-role="controlgroup" data-type="horizontal">
                        <legend>Tel Code: <span style="color: #FF0000;" >*</span>
                            <br /><br /> <span id="spanerrorMobileTelCode" style="color:#F00; display:none;">This field is required</span>
                        </legend>
                        <button id="btnOrgMobileTelCode" data-role="none" class="btn custom-btn-theme-c">
                            <span id="txtTelCode">Select</span>
                            <span class="ui-icon ui-icon-arrow-d icon-right"></span>
                        </button>
                    </fieldset>
                </div>

                <div data-role="fieldcontain">
                    <label for="txtMobileNo">Mobile Number: <span style="color:#F00">*</span></label>
                    <input type="text" id="txtMobileNo" maxlength="7" />
                    <br />
                    <span id="spanerrorMobileNum" style="color:#F00; display:none;">This field is required</span>
                    <span id="spanerrorMobileNumValidate" style="color:#F00; display:none;">Enter 7 digit number</span>
                </div>

                <div data-role="fieldcontain">
	                <label for="selOrgMobileType">Contact Type: <span style="color: #FF0000;" >*</span></label>
                    <select id="selOrgMobileType">
	                    <option value="">Please Select</option>
	                    <option value="Work Mobile">Work Mobile</option>
				        <option value="Personal Mobile">Personal Mobile</option>
				        <option value="Third Party Mobile">Third Party Mobile</option>
                    </select>
                    <br />
                    <span id="spanerrorMobileType" style="color:#F00; display:none;">This field is required</span>
                </div>
            </div>

            <div class="popup-footer align-right">
                <div style="text-align:right;">
                    <button id="btnCancelOrgMobile" data-role="none" class="btn custom-btn-theme-c btnCircle">
                        <img alt="Options" src="/Content/images/close-black.png" />
                    </button>
                    <button id="btnSaveOrgMobile" data-role="none" class="btn custom-btn-theme-b btnCircle">
                        <img alt="Options" src="/Content/images/check.png" />
                    </button>
                </div>
            </div>
        </div>

        <div class="popup-md" id="popOrgEmailInfo" custom-data-role="popup">
            <div class="popup-header">
                Email Info
                <a class="popup-close">x</a>
            </div>

            <div class="popup-content">
                <div data-role="fieldcontain" style="display:none;">
                    <input type="text" readonly id="txtOrgEmailID"/>                
                </div>

                <div data-role="fieldcontain">
                    <label for="txtEmailAdd">Email Address: <span style="color:#F00">*</span></label>
                    <input type="text" id="txtEmailAdd" />
                    <br />
                    <span id="spanerrorEmailAddress" style="color:#F00; display:none;">This field is required</span>
                    <span id="spanerrorEmailAddressValidate" style="color:#F00; display:none;">Enter valid email address</span>
                </div>

                <div data-role="fieldcontain">
	                <label for="selOrgEmailType">Email Type: <span style="color: #FF0000;" >*</span></label>
                    <select id="selOrgEmailType">
	                    <option value="">Please Select</option>
	                    <option value="Work Email">Work Email</option>
				        <option value="Personal Email">Personal Email</option>
				        <option value="Third Party Email">Third Party Email</option>
                    </select>
                    <br />
                    <span id="spanerrorEmailType" style="color:#F00; display:none;">This field is required</span>
                </div>
            </div>

            <div class="popup-footer align-right">
                <div style="text-align:right;">
                    <button id="btnCancelOrgEmailAddress" data-role="none" class="btn custom-btn-theme-c btnCircle">
                        <img alt="Options" src="/Content/images/close-black.png" />
                    </button>
                    <button id="btnSaveOrgEmailAddress" data-role="none" class="btn custom-btn-theme-b btnCircle">
                        <img alt="Options" src="/Content/images/check.png" />
                    </button>
                </div>
            </div>
        </div>

        <div class="popup-md" id="popOrgWebsiteInfo" custom-data-role="popup">
            <div class="popup-header">
                Website Info
                <a class="popup-close">x</a>
            </div>

            <div class="popup-content">
                <div data-role="fieldcontain" style="display:none;">
                    <input type="text" readonly id="txtOrgWebsiteID"/>                
                </div>

                <div data-role="fieldcontain">
                    <label for="txtWebsiteUrl">Website URL: <span style="color:#F00">*</span></label>
                    <input type="text" id="txtWebsiteUrl" />
                    <br />
                    <span id="spanerrorWebsiteUrl" style="color:#F00; display:none;">This field is required</span>
                </div>
            </div>

            <div class="popup-footer align-right">
                <div style="text-align:right;">
                    <button id="btnCancelOrgWebsite" data-role="none" class="btn custom-btn-theme-c btnCircle">
                        <img alt="Options" src="/Content/images/close-black.png" />
                    </button>
                    <button id="btnSaveOrgWebsite" data-role="none" class="btn custom-btn-theme-b btnCircle">
                        <img alt="Options" src="/Content/images/check.png" />
                    </button>
                </div>
            </div>
        </div>

        <div class="popup-md" id="popContactPersonTelInfo" custom-data-role="popup">
            <div class="popup-header">
                Telephone Info
                <a class="popup-close">x</a>
            </div>

            <div class="popup-content">
                <div data-role="fieldcontain" style="display:none;">
                    <input type="text" readonly id="txtContactPersonTelCountryID" value="e783ed13-aa98-4cc1-918e-d49b05c0167e"/>
	                <input type="text" readonly id="txtContactPersonTelID"/>                
	                <input type="text" readonly id="txtContactPersonTelAreaCodeID"/>                
                </div>

                <div data-role="fieldcontain">
                    <fieldset data-role="controlgroup" data-type="horizontal">
                        <legend>Country Code: <span style="color: #FF0000;" >*</span>
                            <span id="spanerrorContactPersonTelCountryCode" style="color:#F00; display:none;">This field is required</span>
                        </legend>
                        <button id="btnContactPersonTelCountryCodeDisable" data-role="none" class="btn custom-btn-theme-c">
                            <span id="txtContactPersonCountryCode">PHL - 63</span>
                            <span class="ui-icon ui-icon-arrow-d icon-right"></span>
                        </button>                                    
                    </fieldset>
                </div>

                <div data-role="fieldcontain">
                    <fieldset data-role="controlgroup" data-type="horizontal">
                        <legend>Area Code: <span style="color: #FF0000;" >*</span>
                            <br /><br /> <span id="spanerrorContactPersonAreaCode" style="color:#F00; display:none;">This field is required</span>
                        </legend>
                        <button id="btnContactPersonTelAreaCode" data-role="none" class="btn custom-btn-theme-c">
                            <span id="txtContactPersonAreaCode">Select</span>
                            <span class="ui-icon ui-icon-arrow-d icon-right"></span>
                        </button>
                    </fieldset>
                </div>

                <div data-role="fieldcontain">
                    <label for="txtContactPersonTelephoneNum">Telephone Number: <span style="color:#F00">*</span></label>
                    <input type="text" id="txtContactPersonTelephoneNum" maxlength="7" />
                    <br />
                    <span id="spanerrorContactPersonTelephoneNum" style="color:#F00; display:none;">This field is required</span>
                    <span id="spanerrorContactPersonTelephoneNumValidate" style="color:#F00; display:none;">Enter 7 digit number</span>
                </div>

                <div data-role="fieldcontain">
                    <label for="txtContactPersonTelephoneExt">Extension or Local No.:</label>
                    <input type="text" id="txtContactPersonTelephoneExt" maxlength="10" />
                </div>

                <div data-role="fieldcontain">
	                <label for="selContactPersonTelType">Contact Type: <span style="color: #FF0000;" >*</span></label>
                    <select id="selContactPersonTelType">
	                    <option value="">Please Select</option>
	                    <option value="Work Telephone">Work Telephone</option>
				        <option value="Personal Telephone">Personal Telephone</option>
				        <option value="Third Party Telephone">Third Party Telephone</option>
                    </select>
                    <br />
                    <span id="spanerrorContactPersonTelType" style="color:#F00; display:none;">This field is required</span>
                </div>
            </div>

            <div class="popup-footer align-right">
                <div style="text-align:right;">
                    <button id="btnCancelContactPersonTelephone" data-role="none" class="btn custom-btn-theme-c btnCircle">
                        <img alt="Options" src="/Content/images/close-black.png" />
                    </button>
                    <button id="btnSaveContactPersonTelephone" data-role="none" class="btn custom-btn-theme-b btnCircle">
                        <img alt="Options" src="/Content/images/check.png" />
                    </button>
                </div>
            </div>
        </div>

        <div class="popup-md" id="popContactPersonMobileInfo" custom-data-role="popup">
            <div class="popup-header">
                Mobile Info
                <a class="popup-close">x</a>
            </div>

            <div class="popup-content">
                <div data-role="fieldcontain" style="display:none;">
                    <input type="text" readonly id="txtContactPersonMobileCountryID" value="e783ed13-aa98-4cc1-918e-d49b05c0167e"/>
                    <input type="text" readonly id="txtContactPersonMobileID"/>	                            
	                <input type="text" readonly id="txtContactPersonTelCoPrefixID"/>                
                </div>

                <div data-role="fieldcontain">
                    <fieldset data-role="controlgroup" data-type="horizontal">
                        <legend>Country Code: <span style="color: #FF0000;" >*</span>
                            <br /><br /><span id="spanerrorContactPersonMobileCountryCode" style="color:#F00; display:none;">This field is required</span>
                        </legend>
                        <button id="btnContactPersonMobileCountryCodeDisable" data-role="none" class="btn custom-btn-theme-c">
                            <span id="txtContactPersonMobileCountryCode">PHL - 63</span>
                            <span class="ui-icon ui-icon-arrow-d icon-right"></span>
                        </button>                                    
                    </fieldset>
                </div>

                <div data-role="fieldcontain">
                    <fieldset data-role="controlgroup" data-type="horizontal">
                        <legend>Tel Code: <span style="color: #FF0000;" >*</span>
                            <br /><br /> <span id="spanerrorContactPersonMobileTelCode" style="color:#F00; display:none;">This field is required</span>
                        </legend>
                        <button id="btnContactPersonMobileTelCode" data-role="none" class="btn custom-btn-theme-c">
                            <span id="txtContactPersonTelCode">Select</span>
                            <span class="ui-icon ui-icon-arrow-d icon-right"></span>
                        </button>
                    </fieldset>
                </div>

                <div data-role="fieldcontain">
                    <label for="txtContactPersonMobileNo">Mobile Number: <span style="color:#F00">*</span></label>
                    <input type="text" id="txtContactPersonMobileNo" maxlength="7" />
                    <br />
                    <span id="spanerrorContactPersonMobileNum" style="color:#F00; display:none;">This field is required</span>
                    <span id="spanerrorContactPersonMobileNumValidate" style="color:#F00; display:none;">Enter 7 digit number</span>
                </div>

                <div data-role="fieldcontain">
	                <label for="selContactPersonMobileType">Contact Type: <span style="color: #FF0000;" >*</span></label>
                    <select id="selContactPersonMobileType">
	                    <option value="">Please Select</option>
	                    <option value="Work Mobile">Work Mobile</option>
				        <option value="Personal Mobile">Personal Mobile</option>
				        <option value="Third Party Mobile">Third Party Mobile</option>
                    </select>
                    <br />
                    <span id="spanerrorContactPersonMobileType" style="color:#F00; display:none;">This field is required</span>
                </div>
            </div>

            <div class="popup-footer align-right">
                <div style="text-align:right;">
                    <button id="btnCancelContactPersonMobile" data-role="none" class="btn custom-btn-theme-c btnCircle">
                        <img alt="Options" src="/Content/images/close-black.png" />
                    </button>
                    <button id="btnSaveContactPersonMobile" data-role="none" class="btn custom-btn-theme-b btnCircle">
                        <img alt="Options" src="/Content/images/check.png" />
                    </button>
                </div>
            </div>
        </div>

        <div class="popup-md" id="popContactPersonEmailInfo" custom-data-role="popup">
            <div class="popup-header">
                Email Info
                <a class="popup-close">x</a>
            </div>

            <div class="popup-content">
                <div data-role="fieldcontain" style="display:none;">
                    <input type="text" readonly id="txtContactPersonEmailID"/>                
                </div>

                <div data-role="fieldcontain">
                    <label for="txtContactPersonEmailAdd">Email Address: <span style="color:#F00">*</span></label>
                    <input type="text" id="txtContactPersonEmailAdd" />
                    <br />
                    <span id="spanerrorContactPersonEmailAddress" style="color:#F00; display:none;">This field is required</span>
                    <span id="spanerrorContactPersonEmailAddressValidate" style="color:#F00; display:none;">Enter valid email address</span>
                </div>

                <div data-role="fieldcontain">
	                <label for="selContactPersonEmailType">Email Type: <span style="color: #FF0000;" >*</span></label>
                    <select id="selContactPersonEmailType">
	                    <option value="">Please Select</option>
	                    <option value="Work Email">Work Email</option>
				        <option value="Personal Email">Personal Email</option>
				        <option value="Third Party Email">Third Party Email</option>
                    </select>
                    <br />
                    <span id="spanerrorContactPersonEmailType" style="color:#F00; display:none;">This field is required</span>
                </div>
            </div>

            <div class="popup-footer align-right">
                <div style="text-align:right;">
                    <button id="btnCancelContactPersonEmailAddress" data-role="none" class="btn custom-btn-theme-c btnCircle">
                        <img alt="Options" src="/Content/images/close-black.png" />
                    </button>
                    <button id="btnSaveContactPersonEmailAddress" data-role="none" class="btn custom-btn-theme-b btnCircle">
                        <img alt="Options" src="/Content/images/check.png" />
                    </button>
                </div>
            </div>
        </div>

        <div class="popup-md" id="popSMS" custom-data-role="popup">
            <div class="popup-header">
                SMS
                <a class="popup-close">x</a>
            </div>

            <div class="popup-content">
                <label>Recipients: <b id="txtRecipientsCount"></b></label> 
                <button id="btnAddNewNumber" data-role="none" class="btn custom-btn-theme-d btnCircle-xs" style="float:right;"><img alt="Add" src="/Content/images/plus-white.png" style="width:18px;" /></button>
                <button id="btnSearchContacts" data-role="none" class="btn custom-btn-theme-c btnCircle-xs" style="float:right; margin-right:5px;"><img alt="Add" src="/Content/images/search.png" style="width:18px;" /></button>
                <button id="btnViewRecipientsDetails" data-role="none" class="btn custom-btn-theme-e btnCircle-xs" style="float:right; margin-right:5px;"><img alt="View Detail" src="/Content/images/eye-open.png" style="width:18px;" /></button>
                
                <br /><br />
                <div id="listofRecipient" style="max-height: 150px; overflow:auto;">
                    
                </div>
                <br />
                <span id="charNum"></span>
                <textarea id="txtSMSMessage" max-length="160"></textarea>
            </div>

            <div class="popup-footer align-right">
                <div style="text-align:right;">
                    <button id="btnCancelSMS" data-role="none" class="btn custom-btn-theme-c btnCircle">
                        <img alt="Options" src="/Content/images/close-black.png" />
                    </button>
                    <button id="btnSendSMS" data-role="none" class="btn custom-btn-theme-b btnCircle">
                        <img alt="Options" src="/Content/images/send.png" />
                    </button>
                </div>
            </div>
        </div>

        <div class="popup-md" id="popSMSRecipient" custom-data-role="popup">
            <div class="popup-header">
                List of Contacts
                <a class="popup-close">x</a>
            </div>

            <div class="popup-content">
                <ul data-role="listview" id="listSelectedReceipient" class="listContact li-theme-c noBorderList custom-list custom-list-sm">
                        
                </ul>
            </div>

            <div class="popup-footer align-right">
                <div style="text-align:center;">
                    <button id="btnOkSMSRecipient" data-role="none" class="btn custom-btn-theme-b btnCircle">
                        <img alt="Ok" src="/Content/images/check.png" />
                    </button>
                </div>
            </div>
        </div>

        <div data-role="popup" id="popNewNumber" data-overlay-theme="a" data-theme="a" data-corners="false"> 
            <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
            <center><h3>New Number</h3></center>
            <div data-role="content" class="ui-content ui-body-d">
                <input type="text" data-role="none" style="display:none;" id="txtToDoID" />
                <div class="custom-ui-label">
                    <div>Country Code:</div>
                    <div><button disabled>PHL - 63</button></div>
                </div>

                <div class="custom-ui-label">
                    <div>Telco Prefrix:</div>
                    <div><button id="btnNewNumberTelcoPrefix">Please Select</button></div>
                </div>

                <div class="custom-ui-label">
                    <div>Enter 7 digit no.:</div>
                    <div><input type="text" id="txtNewNumber" maxlength="7" /></div>
                </div>

                <br />
                <button id="btnOkNewNumber" data-role="none" class="btn custom-btn-theme-b">Ok</button>
                <br /><br />
                <button id="btnCancelNewNumber" data-role="none" class="btn custom-btn-theme-c">Cancel</button>
            </div>       
        </div>

        <div class="popup-md" id="popSearchContacts" custom-data-role="popup">
            <div class="popup-header">
                Search Contacts
                <a class="popup-close">x</a>
            </div>

            <div class="popup-content">
                <input type="search" id="txtSearchContacts" placeholder="Search Name ..." />
                <div style="overflow:auto;">
                    <div style="float:right; padding:5px;"><a data-role="none" id="lnkSelectAllSearchRecords" class="custom-link"></a></div>
                    <table data-role="table" data-role="reflow" style="margin-right:5px; display:table;" class="ui-table ui-responsive table-stroke table-stripe custom-tbl-responsive">
                        <thead>
                            <tr class="custom-theme-f">
                                <th class="bulk" style="cursor:pointer; width:20px;">
                                    <label class="checkbox-only" for="chbbulk">
                                        <input type="checkbox" name="chbbulk" id="chbbulk" data-iconpos="notext"/>
                                    </label>
                                </th>
                                <th data-sort="accounting" style="cursor:pointer; width:20px;"> # </th>
                                <th style="cursor:pointer; width:20px;"></th>
                                <th data-sort="string" style="cursor:pointer;"> Name </th>
                            </tr>
                        </thead>
                        <tbody id="tblProspectbody">
                        </tbody>
                    </table>
                </div>
            </div>

            <div class="popup-footer align-right">
                <div style="text-align:right;">
                    <button id="" data-role="none" class="btn custom-btn-theme-b btnCircle popup-close">
                        <img alt="Ok" src="/Content/images/check.png" />
                    </button>
                </div>
            </div>
        </div>
    </div>
    @*End Pop Ups*@

    <div>
    
        <div subpage="pageLeadGen" id="pageMainDashboard">
            <div data-role="content">
                <div>
                    <div style="float:left;">
                        @*<button data-role="none" id="btnAddTopic" class="btn custom-btn-theme-b btn-less-width">Add Topic</button>
                        <button data-role="none" id="btnFilterForm1" class="btn custom-btn-theme-c btn-less-width">
                            Filtered by: All
                            <span class="ui-icon ui-icon-arrow-d" style="display:inline-block; margin-left:10px;"></span>
                        </button>*@
                        @*<select id="selFilterForm" data-role="none" class="custom-theme-c select">
                            <option value="10">10</option>
                            <option value="25">25</option>
                            <option value="50">50</option>
                            <option value="100">100</option>
                        </select>*@
                        @*<select id="selCity" data-role="none" class="custom-theme-c select" style="display:none;">
                        
                        </select>*@
                    </div>                    
                    <div style="float:right;">                        
                        <button id="btnBulkAction" data-role="none" class="btn custom-btn-theme-b btn-less-width">
                            Bulk Actions
                        </button>
                    </div>
                    <div style="clear:both;"></div>
                </div>                
                <br />

                <form id="formupload" enctype="multipart/form-data" method="post" style="display:none;">
                    <div style="display:none;"><input type="text" id="OrgID" name="OrgID" value="28b8a792-cb65-4d92-988f-d721123c93b9" style="display:none;"></div>
                    <input type="file" id="objFile" name="objFile">
                    <input type="submit" id="btnUploadExcel" value="Upload File">
                </form>

                <div id="divCountRow"></div>
                @*<br />*@
                <div style="overflow:auto;">
                    <table id="tblDataTable" data-role="table" data-role="reflow" style="margin-right:5px; display:table;" class="ui-table ui-responsive table-stroke table-stripe custom-tbl-responsive">
                        <thead>
                            <tr class="custom-theme-f">
                                <th style="width:20px !important;"> </th>
                                <th style="cursor:pointer;"> Name </th>
                                <th style="cursor:pointer;"> Business Industry </th>
                                <th style="cursor:pointer;"> Contact Person </th>
                                <th style="cursor:pointer;"> Contact Info </th>
                                <th style="cursor:pointer;"> Address </th>
                                <th style="cursor:pointer;"> Organization Affiliation </th>
                                <th style="cursor:pointer;"> Date Created/Modified </th>
                                <th style="cursor:pointer;"> Last Interaction Date </th>
                            </tr>
                        </thead>
                        <tbody>
                        </tbody>
                    </table>
                </div>                                   

                @*<div id="btnAddProspect" class="floatingButton custom-btn-theme-d" title="Options"><img src="/Content/images/plus-white.png" alt="Add"></div>*@
                <div class="fixed-ActionBtn" style="display:block;">
                    <button data-role="none" class="btn custom-btn-theme-d btnCircle">
                        <img alt="Add" src="/Content/images/plus-white.png" style="width:18px;" />
                    </button>
                    <ul>
                        <li><button data-role="none" id="btnAddOrgProspect" class="custom-btn-theme-e" data-title="Organization"><img alt="Organization" src="/Content/images/Home.png" style="width:18px;" /></button></li>
                        <li><button data-role="none" id="btnAddPeopleProspect" class="custom-btn-theme-b" data-title="Person"><img alt="Person" src="/Content/images/Customer.png" style="width:18px;" /></button></li>
                        <li id="lisAddExcelProspect"><button data-role="none" id="btnAddExcelProspect" class="custom-btn-theme-a" data-title="Upload Contacts Via Excel file"><img alt="Upload" src="/Content/images/upload.png" style="width:18px;" /></button></li>
                        <li id="lisDownloadExcelProspect"><button data-role="none" id="btnDownloadExcelProspect" class="custom-btn-theme-f" data-title="Download Contacts Excel template"><img alt="Upload" src="/Content/images/download2.png" style="width:18px;" /></button></li>
                        @*<li><button data-role="none" id="btnAddGoogleProspect" class="custom-btn-theme-f" title="Google">Google</button></li>
                        <li><button data-role="none" id="btnAddMicrosoftProspect" class="custom-btn-theme-g" title="Microsoft">Microsoft</button></li>*@
                    </ul>
                    <span id="spanbtnSMS">
                        <ol class="secondaryActionBtn">
                            @*<li class="spanbtnSMS" style="display:none;">
                                <button id="btnCancelSMSSelectedReceipient" title="Cancel SMS" data-role="none" class="btn custom-btn-theme-c btnCircle-sm">
                                    <img alt="Add" src="/Content/images/close-black.png" style="width:18px;" />
                                </button>
                            </li>
                            <li>
                                <button id="btnViewSMSSelectedReceipient" title="View Selected Items" data-role="none" class="btn custom-btn-theme-g btnCircle-sm">
                                    <img alt="Add" src="/Content/images/eye-open.png" style="width:18px;" />
                                </button>
                            </li>
                            <li>
                                <button id="btnComposeSMSSelectedReceipient" title="Compose Message" data-role="none" class="btn custom-btn-theme-a btnCircle-sm">
                                    <img alt="Add" src="/Content/images/message.png" style="width:18px;" />
                                </button>
                            </li>
                            <li>
                                <button id="btnSMSLogs" title="SMS Logs" data-role="none" class="btn custom-btn-theme-e btnCircle-sm">
                                    <img alt="Add" src="/Content/images/paper.png" style="width:18px;" />
                                </button>
                            </li>*@
                            <li>
                                <button id="btnBackToTop" title="Back to top" data-role="none" style="display:none;" class="btn custom-btn-theme-h btnCircle-sm">
                                    <img alt="Add" src="/Content/images/arrow-up.png" style="width:18px;" />
                                </button>
                            </li>
                        </ol>
                    </span>
                </div>            
            </div>
        </div>

        <div subpage="pageLeadGen" id="pageAddOrgProspect">
            <div data-role="content">
                <div id="divCompanyProfile" style="display:block;">
                    <label style="font-size:1.5em; font-weight:bold;">Organization Profile</label>
                    <input data-role="none" type="text" id="txtProspectOrgID" style="display:none;" />
                    <br />
                    <span style="font-size:12px;">Fields marked with <span style="color:#F00;">*</span> are required.</span>
                    <br /><br />
                    <div class="custom-ui-label">
                        <div>Organization Name: <span style="color: #FF0000;" >*</span></div>
                        <div>
                            @*<button id="btnProspectCompanyName">Search Company</button>*@
                            <input type="text" id="txtProspectCompanyName" />
                        </div>
                    </div>

                    <div class="custom-ui-label">
                        <div>Organization Address:</div>
                        <div>
                            <input type="text" id="txtProspectHouseBldgNo" placeholder="Building No." />
                            <input type="text" id="txtProspectStreetName" placeholder="Street" />
                            <select id="selProspectCities">
                                <option>Please Select City</option>
                            </select>
                            <select id="selProspectOrgRegion">
                                
                            </select>
                            <select id="selProspectIsland">
                                <option value="">Please Select Island</option>
                                <option value="Luzon">Luzon</option>
                                <option value="Visayas">Visayas</option>
                                <option value="Mindanao">Mindanao</option>
                            </select>
                            <select id="selProspectCountries">
                                <option>Please Select Country</option>
                            </select>
                        </div>
                    </div>

                    <div class="custom-ui-label">
                        <div>Organization Background:</div>
                        <div>
                            <textarea id="txtProspectCompanyBackground"></textarea>
                        </div>
                    </div>

                    <div class="custom-ui-label">
                        <div>Organization Mission:</div>
                        <div>
                            <textarea id="txtProspectCompanyMission"></textarea>
                        </div>
                    </div>

                    <div class="custom-ui-label">
                        <div>Organization Vision:</div>
                        <div>
                            <textarea id="txtProspectCompanyVision"></textarea>
                        </div>
                    </div>

                    <div class="custom-ui-label">
                        <div>Founding Date/Year:</div>
                        <div>
                            <input type="text" id="txtProspectCompanyFounding" />
                        </div>
                    </div>

                    <div class="custom-ui-label">
                        <div>Business Industry:</div>
                        <div>
                            <input type="text" data-role="none" id="txtBusIndustryID" style="display:none;" />
                            <button id="btnProspectCompanyIndustry">Search here</button>
                        </div>
                    </div>

                    <div class="panel">
                        <div class="panel-title custom-theme-h">
                            Organization Affiliation
                        </div>

                        <div class="panel-content txt-left">
                            <div data-role="fieldcontain">
                                <div>
                                    <div data-role="fieldcontain" style="text-align:right;">
                                        <button id="btnAddOrgAffiliation" data-role="none" class="btn custom-btn-theme-b btnCircle">
                                            <img alt="Add" src="/Content/images/plus-white.png" style="width:18px;" />
                                        </button>
                                    </div>
                                    <div style="padding: 8px 15px 0px 15px;">
                                        <ul data-role="listview" id="lstOrgAffiliation" class="li-theme-c noBorderList custom-list custom-list-sm">
                                        </ul>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <br /><br />
                    <div class="panel">
                        <div class="panel-title custom-theme-h">
                            Affiliated Person
                        </div>

                        <div class="panel-content txt-left">
                            <div data-role="fieldcontain">
                                <div>
                                    <div data-role="fieldcontain" style="text-align:right;">
                                        <button id="btnAddAffiliatedPerson" data-role="none" class="btn custom-btn-theme-b btnCircle">
                                            <img alt="Add" src="/Content/images/plus-white.png" style="width:18px;" />
                                        </button>
                                    </div>
                                    <div style="padding: 8px 15px 0px 15px;">
                                        <ul data-role="listview" id="lstAffiliatedPerson" class="li-theme-c noBorderList custom-list custom-list-sm">
                                        </ul>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <br /><br />
                    <label style="font-size:1.5em; font-weight:bold;">Organization Contact Info</label>
                    <br /><br />
                    <div class="panel">
                        <div class="panel-title custom-theme-h">
                            Telephone Number
                        </div>

                        <div class="panel-content txt-left">
                            <div data-role="fieldcontain">
                                <div id="divOrgTel" subpage="telpage" style="display:block;">
                                    <div data-role="fieldcontain" style="text-align:right;">
                                        <button id="btnAddOrgTelephone" data-role="none" class="btn custom-btn-theme-b btnCircle">
                                            <img alt="Add" src="/Content/images/plus-white.png" style="width:18px;" />
                                        </button>
                                    </div>
                                    <div style="padding: 8px 15px 0px 15px;">
                                        <ul data-role="listview" id="lstOrgTelephone" class="li-theme-c noBorderList custom-list custom-list-sm">
                                        </ul>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <br /><br /><br />
                    <div class="panel">
                        <div class="panel-title custom-theme-h">
                            Mobile Number
                        </div>

                        <div class="panel-content txt-left">
                            <div data-role="fieldcontain">
                                <div id="divMobileHome" subpage="mobilepage" style="display:block;">
                                    <div data-role="fieldcontain" style="text-align:right;">
                                        <button id="btnAddOrgMobile" data-role="none" class="btn custom-btn-theme-b btnCircle">
                                            <img alt="Add" src="/Content/images/plus-white.png" style="width:18px;" />
                                        </button>
                                    </div>
                                    <div style="padding: 8px 15px 0px 15px;">
                                        <ul data-role="listview" id="lstOrgMobile" class="li-theme-c noBorderList custom-list custom-list-sm">
                                        </ul>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <br /><br /><br />
                    <div class="panel">
                        <div class="panel-title custom-theme-h">
                            Email Address
                        </div>

                        <div class="panel-content txt-left">
                            <div data-role="fieldcontain">
                                <div id="divEmailHome" subpage="emailpage" style="display:block;">
                                    <div data-role="fieldcontain" style="text-align:right;">
                                        <button id="btnAddOrgEmail" data-role="none" class="btn custom-btn-theme-b btnCircle">
                                            <img alt="Add" src="/Content/images/plus-white.png" style="width:18px;" />
                                        </button>
                                    </div>
                                    <div style="padding: 8px 15px 0px 15px;">
                                        <ul data-role="listview" id="lstOrgEmail" class="li-theme-c noBorderList custom-list custom-list-sm">
                                        </ul>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <br /><br /><br />
                    <div class="panel">
                        <div class="panel-title custom-theme-h">
                            Website
                        </div>

                        <div class="panel-content txt-left">
                            <div data-role="fieldcontain">
                                <div id="divWebsiteHome" subpage="websitepage" style="display:block;">
                                    <div data-role="fieldcontain" style="text-align:right;">
                                        <button id="btnAddOrgWebsite" data-role="none" class="btn custom-btn-theme-b btnCircle">
                                            <img alt="Add" src="/Content/images/plus-white.png" style="width:18px;" />
                                        </button>
                                    </div>
                                    <div style="padding: 8px 15px 0px 15px;">
                                        <ul data-role="listview" id="lstOrgWebsite" class="li-theme-c noBorderList custom-list custom-list-sm">
                                        </ul>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <br /><br />
                </div>
            
                <div id="divCompanyContactPerson">
                    <label style="font-size:1.5em; font-weight:bold;">Company Contact Person</label>
                    <br /><br />

                    <div class="panel">
                        <div class="panel-title custom-theme-h">
                            &nbsp;
                        </div>

                        <div class="panel-content txt-left">
                            <div data-role="fieldcontain">
                                <div id="divContactPersonHome" style="display:block;">
                                    <div data-role="fieldcontain" style="text-align:right;">
                                        <button id="btnAddContactPerson" data-role="none" class="btn custom-btn-theme-b btnCircle">
                                            <img alt="Add" src="/Content/images/plus-white.png" style="width:18px;" />
                                        </button>
                                    </div>
                                    <div style="padding: 8px 15px 0px 15px;">
                                        <ul data-role="listview" id="lstOrgContactPerson" class="li-theme-c noBorderList custom-list custom-list-sm">
                                        </ul>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>            

                <div id="divProspectMainButton" class="floatingDiv-bottomcenter">
                    <button data-role="none" id="btnCancelAddingContact" class="btn custom-btn-theme-c btnCircle btnShadow">
                        <img alt="Cancel" src="/Content/images/arrow-left.png" style="width:18px;" />
                    </button>
                    <button data-role="none" id="btnSaveAddingContact" class="btn custom-btn-theme-d btnCircle btnShadow">
                        <img alt="Save" src="/Content/images/save.png" style="width:18px;" />
                    </button>
                </div>
            </div>
        </div>

        <div subpage="pageLeadGen" id="pageAddPersonProspect">
            <div data-role="content">
                <label style="font-size:1.5em; font-weight:bold;">Person Profile</label>
                <input data-role="none" type="text" id="txtProspectPersonID" style="display:none;" />
                <br />
                <span style="font-size:12px;">Fields marked with <span style="color:#F00;">*</span> are required.</span>
                <br /><br />
                <div class="custom-ui-label">
                    <div>First Name: <span style="color:#F00;">*</span></div>
                    <div>
                        <input type="text" id="txtProspectPersonFirstName" />
                    </div>
                </div>
                <div class="custom-ui-label">
                    <div>Middle Name: </div>
                    <div>
                        <input type="text" id="txtProspectPersonMiddleName" />
                    </div>
                </div>
                <div class="custom-ui-label">
                    <div>Last Name: </div>
                    <div>
                        <input type="text" id="txtProspectPersonLastName" />
                    </div>
                </div>

                <div class="custom-ui-label">
                    <div>Position/Title: </div>
                    <div>
                        <input type="text" id="txtProspectPersonPosition" />
                    </div>
                </div>

                <div class="custom-ui-label">
                    <div>Position/Title Description: </div>
                    <div>
                        <input type="text" id="txtProspectPersonPositionDescription" />
                    </div>
                </div>

                <div class="panel">
                    <div class="panel-title custom-theme-h">
                        Address
                    </div>

                    <div class="panel-content txt-left">
                        <input data-role="none" type="text" id="txtPersonAddressID" style="display:none;" />
                        <div class="custom-ui-label">
                            <div>House or Building No:</div>
                            <div><input type="text" id="txtPersonHouseBuildingNo" /></div>
                        </div>

                        <div class="custom-ui-label">
                            <div>Street:</div>
                            <div><input type="text" id="txtPersonStreet" /></div>
                        </div>

                        <div class="custom-ui-label">
                            <div>City Or Town:</div>
                            <div><select id="selPersonCity"></select></div>
                        </div>

                        <div class="custom-ui-label">
                            <div>Region:</div>
                            <div><select id="selPersonRegion"></select></div>
                        </div>

                        <div class="custom-ui-label">
                            <div>Island Group:</div>
                            <div>
                                <select id="selPersonIsland">
                                    <option value="">Please Select Island</option>
                                    <option value="Luzon">Luzon</option>
                                    <option value="Visayas">Visayas</option>
                                    <option value="Mindanao">Mindanao</option>
                                </select>
                            </div>
                        </div>

                        <div class="custom-ui-label">
                            <div>Country:</div>
                            <div><select id="selPersonCountry"></select></div>
                        </div>

                        <div class="custom-ui-label">
                            <div>Type of Address:</div>
                            <div>
                                <select id="selPersonTypeAddress">
                                    <option value="Permanent Address">Permanent Address</option>
                                    <option value="Non-Permanent Address">Non-Permanent Address</option>
                                    <option value="Provincial Address">Provincial Address</option>
                                </select>
                            </div>
                        </div>
                    </div>
                </div>

                <br /><br />
                <div class="panel">
                    <div class="panel-title custom-theme-h">
                        Organization Affiliation
                    </div>

                    <div class="panel-content txt-left">
                        <div data-role="fieldcontain">
                            <div>
                                <div data-role="fieldcontain" style="text-align:right;">
                                    <button id="btnAddPersonOrgAffiliation" data-role="none" class="btn custom-btn-theme-b btnCircle">
                                        <img alt="Add" src="/Content/images/plus-white.png" style="width:18px;" />
                                    </button>
                                </div>
                                <div style="padding: 8px 15px 0px 15px;">
                                    <ul data-role="listview" id="lstPersonOrgAffiliation" class="li-theme-c noBorderList custom-list custom-list-sm">
                                    </ul>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <br />
                <br />
                <label style="font-size:1.5em; font-weight:bold;">Person Contact Info</label>
                <br /><br />
                <div class="panel">
                    <div class="panel-title custom-theme-h">
                        Telephone Number
                    </div>

                    <div class="panel-content txt-left">
                        <div data-role="fieldcontain">
                            <div id="divPersonTel" subpage="telpage" style="display:block;">
                                <div data-role="fieldcontain" style="text-align:right;">
                                    <button id="btnAddPersonTelephone" data-role="none" class="btn custom-btn-theme-b btnCircle">
                                        <img alt="Add" src="/Content/images/plus-white.png" style="width:18px;" />
                                    </button>
                                </div>
                                <div style="padding: 8px 15px 0px 15px;">
                                    <ul data-role="listview" id="lstPersonTelephone" class="li-theme-c noBorderList custom-list custom-list-sm">
                                    </ul>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <br /><br /><br />
                <div class="panel">
                    <div class="panel-title custom-theme-h">
                        Mobile Number
                    </div>

                    <div class="panel-content txt-left">
                        <div data-role="fieldcontain">
                            <div id="divPersonMobileHome" subpage="mobilepage" style="display:block;">
                                <div data-role="fieldcontain" style="text-align:right;">
                                    <button id="btnAddPersonMobile" data-role="none" class="btn custom-btn-theme-b btnCircle">
                                        <img alt="Add" src="/Content/images/plus-white.png" style="width:18px;" />
                                    </button>
                                </div>
                                <div style="padding: 8px 15px 0px 15px;">
                                    <ul data-role="listview" id="lstPersonMobile" class="li-theme-c noBorderList custom-list custom-list-sm">
                                    </ul>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <br /><br /><br />
                <div class="panel">
                    <div class="panel-title custom-theme-h">
                        Email Address
                    </div>

                    <div class="panel-content txt-left">
                        <div data-role="fieldcontain">
                            <div id="divPersonEmailHome" subpage="emailpage" style="display:block;">
                                <div data-role="fieldcontain" style="text-align:right;">
                                    <button id="btnAddPersonEmail" data-role="none" class="btn custom-btn-theme-b btnCircle">
                                        <img alt="Add" src="/Content/images/plus-white.png" style="width:18px;" />
                                    </button>
                                </div>
                                <div style="padding: 8px 15px 0px 15px;">
                                    <ul data-role="listview" id="lstPersonEmail" class="li-theme-c noBorderList custom-list custom-list-sm">
                                    </ul>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <div id="divProspectPersonMainButton" class="floatingDiv-bottomcenter">
                    <button data-role="none" id="btnCancelPersonAddingContact" class="btn custom-btn-theme-c btnCircle btnShadow">
                        <img alt="Cancel" src="/Content/images/arrow-left.png" style="width:18px;" />
                    </button>
                    <button data-role="none" id="btnSavePersonAddingContact" class="btn custom-btn-theme-d btnCircle btnShadow">
                        <img alt="Save" src="/Content/images/save.png" style="width:18px;" />
                    </button>                
                </div>
            </div>
        </div>

        <div subpage="pageLeadGen" id="pageSettings">
        
        </div>

        <div id="pageActions"  subpage="pageLeadGen" style="width:100%;">
            <div data-role="content">
                <div class="ui-grid-a ui-responsive">
                    <div class="ui-block-a thirtyseventy-block-a" style="padding:5px;">
                        <div style="background-color:#F7F7F7;">
                            <div data-toggle="listAction" class="custom-list-menu-toggle">
                            
                            </div>
                            <ul id="listAction" class="custom-list-menu list-menu txt-left">
                                <li><a id="btnPageColdCall" data-gotopage="pageColdCall" class="active-menu">Cold Call</a></li>
                                <li><a id="btnPageToDo" data-gotopage="pageToDo">To-Do</a></li> 
                                <li><a id="btnPageSMS" data-gotopage="pageSMS">Send SMS</a></li>
                                <li><a id="btnPageInteractionHistory" data-gotopage="pageInteractionHistory">Interaction History</a></li>
                                <li><a id="btnPageTakeANote" data-gotopage="pageTakeANote">Take A Note</a></li>
                                <li><a id="btnPageScheduleAnAction" data-gotopage="pageScheduleAnAction">Schedule An Action</a></li>
                            </ul>
                        </div>
                    </div>

                    <div class="ui-block-b thirtyseventy-block-b" style="padding:5px;">
                        <div subpage="actions" style="display:block" id="pageColdCall">
                            <div id="divMainColdCal" subpage="coldcall">
                                <div data-role="content">            
                                    <div class="ui-grid-a ui-responsive">
                                        <div class="ui-block-a thirtyseventy-block-a">
                                            <div>
                                                <label for="txtCallScript">Call Duration</label>
                                                <div style="font-size:25px;">
                                                    <span id="sw_h">00</span>:
                                                    <span id="sw_m">00</span>:
                                                    <span id="sw_s">00</span>:
                                                    <span id="sw_ms">00</span>
                                                </div>
                                                <div>
                                                    <button data-role="none" id="btnStartColdCall" title="Start Call" class="btn custom-btn-theme-d btnCircle-sm btnShadow">
                                                        <img alt="Start Call" src="/Content/images/phoneup.png" style="width:18px;" />
                                                    </button>
                                                    <button data-role="none" id="btnEndColdCall" title="End Call" class="btn custom-btn-theme-h btnCircle-sm btnShadow">
                                                        <img alt="End Call" src="/Content/images/phonedown.png" style="width:18px; transform: rotate(45deg);" />
                                                    </button>
                                                    <br /><br />
                                                </div>
                                            </div>

                                            <div id="divQrCodeTel">
                                                <img alt="barcode" src="/Content/images/qrcode.jpg" />
                                            </div>
                                            <br />
                                            <b id="txtCPName"></b>
                                            <ol id="lstContactDetails" style="margin: 0 auto; padding-left:25px; list-style-type:decimal;">
                        
                                            </ol>
                                            <br />
                                        </div>
                                        <div class="ui-block-b thirtyseventy-block-b">
                                            <div class="custom-ui-label">
                                                <div>Client Name:</div>
                                                <div class="txtActionClientName" id="btnCCName" style="font-weight:bold; text-transform:capitalize; cursor:pointer;"></div>
                                            </div>
                                            <div id="divColdCallContactPerson" class="custom-ui-label" style="display:block;">
                                                <div>Contact Person:</div>
                                                <div style="text-transform:capitalize;">
                                                    <ol id="lstContactPerson" style="margin: 0 auto; padding-left:20px; list-style-type:decimal;">
                        
                                                    </ol>
                                                </div>
                                            </div>
                                            <div style="display:none;">
                                                <label for="selCallScript">Choose Script</label>
                                                <select id="selCallScript">
                                                    <option>Please Select</option>
                                                </select>
                                            </div>
                                            <div style="display:none;">
                                                <label for="txtCallScript">Call Script</label>
                                                <textarea id="txtCallScript" style="max-height:200px; min-height:100px;"></textarea>
                                            </div>
                                            <br />
                                            <div>
                                                <label for="txtCallNotes">Call Notes</label>
                                                <textarea id="txtCallNotes" style="min-height:100px;"></textarea>
                                            </div>                        
                    
                                            @*<br />
                                            <div>
                                                <a data-role="none" class="custom-link">Set Appointment</a>
                                            </div>
                                            <br />*@
                                        </div>                
                                    </div>

                                    <div class="floatingDiv-bottomcenter">
                                        <div>
                                            <button data-role="none" id="btnCancelColdCall" data-title="Back" class="btn custom-btn-theme-c btnCircle btnShadow">
                                                <img alt="Cancel" src="/Content/images/arrow-left.png" style="width:18px;" />
                                            </button>
                                            <button data-role="none" id="btnOptionColdCall" data-title="More Option" class="btn custom-btn-theme-e btnCircle btnShadow">
                                                <img alt="Option" src="/Content/images/More_Options.png" style="width:18px;" />
                                            </button>
                                            <button data-role="none" id="btnSaveColdCall" data-title="Save" class="btn custom-btn-theme-b btnCircle btnShadow">
                                                <img alt="Save" src="/Content/images/save.png" style="width:18px;" />
                                            </button>                    
                                        </div>                
                                    </div>
                                </div>  
                            </div>
                        </div>
    
                        <div subpage="actions" id="pageInteractionHistory" style="display:none">
                            <div>
                                <div data-role="content">
                                    <div class="custom-ui-label">
                                        <div>Client Name:</div>
                                        <div class="txtActionClientName" style="font-weight:bold; text-transform:capitalize;"></div>
                                    </div>

                                    <div style="overflow:auto;">
                                        <table data-role="table" data-role="reflow" style="margin-right:5px; display:table;" class="ui-table ui-responsive table-stroke table-stripe custom-tbl-responsive">
                                            <thead>
                                                <tr class="custom-theme-f">
                                                    <th data-sort="accounting" style="cursor:pointer;"> # </th>
                                                    <th data-sort="date" style="cursor:pointer;"> Interaction Date/Time </th>
                                                    <th data-sort="string" style="cursor:pointer;"> Interaction Type </th>                            
                                                    <th data-sort="string" style="cursor:pointer;"> Interacted With </th>
                                                    <th data-sort="string" style="cursor:pointer;"> Interaction Duration</th>
                                                    <th>  </th>
                                                </tr>
                                            </thead>
                                            <tbody id="tblInteractionHistory">
                                            </tbody>
                                        </table>
                                    </div>
                                </div>
                            </div>

                            <div class="floatingDiv-bottomcenter">
                                <div>
                                    <button data-role="none" id="btnInteractionHistoryBack" title="Back" class="btn custom-btn-theme-c btnCircle btnShadow">
                                        <img alt="Cancel" src="/Content/images/arrow-left.png" style="width:18px;" />
                                    </button>                  
                                </div>                
                            </div>
                        </div>

                        <div subpage="actions" style="display:none" id="pageToDo">
                            <div>
                                <div data-role="content">

                                    <div class="custom-ui-label">
                                        <div>Client Name:</div>
                                        <div class="txtActionClientName" style="font-weight:bold; text-transform:capitalize;"></div>
                                    </div>

                                    <div style="height:auto; overflow:hidden;">
                                        <div style="float:right; width:120px; text-align:right;">
                                            <button data-role="none" id="btnToDoSetDate" title="Set Date" class="btn custom-btn-theme-b btnCircle-sm btnShadow">
                                                <img alt="Calendar" src="/Content/images/calendar.png" />
                                            </button>
                                            <button data-role="none" id="btnOkToDo" title="Add" class="btn custom-btn-theme-d btnCircle-sm btnShadow">
                                                <img alt="Add" src="/Content/images/plus-white.png" />
                                            </button>                        
                                        </div>
                                        <div style="float:none; width:auto; overflow:hidden;">
                                            <input type="text" id="txtToDo" placeholder="To-Do Name" />
                                        </div>                    
                                    </div>
                                    <br />
                                    <ul id="listToDo" data-role="none" style="list-style-type: none; margin:0 auto; padding:0;">
                                    </ul>                

                                    <br /><br />

                                    <div class="emptyItem" style="display:none;">
                    
                                    </div>

                                    <div class="floatingDiv-bottomcenter">
                                        <button data-role="none" id="btnBackToDo" title="Back" class="btn custom-btn-theme-c btnCircle btnShadow">
                                            <img alt="Cancel" src="/Content/images/arrow-left.png" style="width:18px;" />
                                        </button>                
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div subpage="actions" style="display:none" id="pageSMS">
                            <div data-role="content">
                                <div class="custom-ui-label">
                                    <div>Recipients:</div>
                                    <div>
                                        <span style="font-weight:bold; text-transform:capitalize;" class="txtActionClientName"></span>
                                        <br /><br />
                                        <div id="listofManualRecipient" style="max-height: 150px; overflow:auto;"></div>
                                    </div>                                    
                                </div>
                                <br />
                                <span id="charNum2"></span>
                                <textarea id="txtSMSMessageAction" max-length="160"></textarea>

                                <div class="floatingDiv-bottomcenter">
                                    <div>
                                        <button data-role="none" id="btnCancelSendSMS" data-title="Back" class="btn custom-btn-theme-c btnCircle btnShadow">
                                            <img alt="Cancel" src="/Content/images/arrow-left.png" style="width:18px;" />
                                        </button>
                                        <button data-role="none" id="btnActionSendSMS" data-title="Save" class="btn custom-btn-theme-b btnCircle btnShadow">
                                            <img alt="Save" src="/Content/images/send.png" style="width:18px;" />
                                        </button>                    
                                    </div>                
                                </div>
                            </div>
                        </div>

                        <div subpage="actions" style="display:none" id="pageTakeANote">
                            <div data-role="content">
                                <div class="custom-ui-label">
                                    <div>Client Name:</div>
                                    <div class="txtActionClientName" style="font-weight:bold; text-transform:capitalize;"></div>
                                </div>                                

                                <div subpage="takeanote" id="pageListOfNote" style="display:block;">
                                    <div style="float:right; width:120px; text-align:right;">
                                        <button data-role="none" id="btnAddNote" title="Add" class="btn custom-btn-theme-d btnCircle-sm btnShadow">
                                            <img alt="Add" src="/Content/images/plus-white.png" />
                                        </button>                        
                                    </div>
                                    <br /><br /><br />
                                    <ul id="lstTakenNotes" class="custom-list"></ul>

                                    <div class="floatingDiv-bottomcenter">
                                        <button data-role="none" id="btnBackTakeANote" title="Back" class="btn custom-btn-theme-c btnCircle btnShadow">
                                            <img alt="Cancel" src="/Content/images/arrow-left.png" style="width:18px;" />
                                        </button>                
                                    </div>
                                </div>

                                <div subpage="takeanote" id="pageAddNote" style="display:none;">
                                    <textarea placeholder="Type your note ..." id="txtTakeANote" style="max-height:300px;"></textarea>

                                    <div class="floatingDiv-bottomcenter">
                                        <div>
                                            <button data-role="none" id="btnCancelTakeANote" data-title="Back" class="btn custom-btn-theme-c btnCircle btnShadow">
                                                <img alt="Cancel" src="/Content/images/arrow-left.png" style="width:18px;" />
                                            </button>
                                            <button data-role="none" id="btnSaveTakeANote" data-title="Save" class="btn custom-btn-theme-b btnCircle btnShadow">
                                                <img alt="Save" src="/Content/images/save.png" style="width:18px;" />
                                            </button>                    
                                        </div>                
                                    </div>
                                </div>
                                
                            </div>
                        </div>

                        <div subpage="actions" style="display:none" id="pageScheduleAnAction">
                            <div data-role="content">
                                <div class="custom-ui-label">
                                    <div>Client Name:</div>
                                    <div class="txtActionClientName" style="font-weight:bold; text-transform:capitalize;"></div>
                                </div>

                                <div subpage="schedanaction" id="pageSchedule" style="display:block;">
                                    <div id="divCalendar" style="width:100%;"></div>

                                    <div class="floatingDiv-bottomcenter">
                                        <button data-role="none" id="btnBackCalendar" title="Back" class="btn custom-btn-theme-c btnCircle btnShadow">
                                            <img alt="Cancel" src="/Content/images/arrow-left.png" style="width:18px;" />
                                        </button>                
                                    </div>
                                </div>

                                <div subpage="schedanaction" id="pageNewSched" style="display:none;">
                                    <input data-role="none" type="text" id="txtSchedAnActionID" style="display:none;" />
                                    <div class="custom-ui-label">
                                        <div>Next Action:</div>
                                        <div>
                                            <select id="selSchedAnAction">
                                                <option value="Call">Call</option>
                                                <option value="Meeting">Meeting</option>
                                                <option value="Email">Email</option>
                                                <option value="SMS">SMS</option>
                                            </select>
                                        </div>
                                    </div>

                                    <div class="custom-ui-label">
                                        <div>Subject: <span style="color:#F00;">*</span></div>
                                        <div><input type="text" id="txtSchedAnActionSubject" /></div>
                                    </div>

                                    <div class="custom-ui-label">
                                        <div>Time:</div>
                                        <div>
                                            <div class="custom-checkbox">
                                                <label class="checkbox-only">
                                                    <input type="checkbox" name="chbAllDay" data-iconpos="notext"/>
                                                </label>
                                                <div>All Day</div>
                                            </div>
                                            <div class="ui-grid-a">
                                                <div class="ui-block-a" style="width:60%">
                                                    <input type="text" id="txtDateTimeFrom" readonly />
                                                </div>
                                                <div class="ui-block-b" style="width:40%">
                                                    <button id="btnDateTimeFrom">1:00 AM</button>
                                                </div>
                                            </div>
                                            <div class="ui-grid-a">
                                                <div class="ui-block-a" style="width:60%">
                                                    <input type="text" id="txtDateTimeTo" readonly />
                                                </div>
                                                <div class="ui-block-b" style="width:40%">
                                                    <button id="btnDateTimeTo">2:00 AM</button>
                                                </div>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="custom-ui-label">
                                        <div>Location:</div>
                                        <div><input type="text" id="txtNextActionLocation" /></div>
                                    </div>

                                    <div class="custom-ui-label">
                                        <div>Notes:</div>
                                        <div><textarea id="txtNextActionNotes"></textarea></div>
                                    </div>

                                    <div class="custom-ui-label">
                                        <div>Assign To:</div>
                                        <div>
                                            <select id="selNextActionTeam">
                                                <option>Select Team</option>
                                            </select>
                                        </div>
                                    </div>

                                    <div class="floatingDiv-bottomcenter">
                                        <div>
                                            <button data-role="none" id="btnBackSchedAnAction" data-title="Back" class="btn custom-btn-theme-c btnCircle btnShadow">
                                                <img alt="Cancel" src="/Content/images/arrow-left.png" style="width:18px;" />
                                            </button>
                                            <button data-role="none" id="btnSaveSchedAnAction" data-title="Save" class="btn custom-btn-theme-b btnCircle btnShadow">
                                                <img alt="Save" src="/Content/images/save.png" style="width:18px;" />
                                            </button>                    
                                        </div>                
                                    </div>
                                </div>

                                <div subpage="schedanaction" id="pageListofEvents" style="display:block;">
                                    <div style="float:right; width:120px; text-align:right;">
                                        <button data-role="none" id="btnAddSched" title="Add" class="btn custom-btn-theme-d btnCircle-sm btnShadow">
                                            <img alt="Add" src="/Content/images/plus-white.png" />
                                        </button>                        
                                    </div>
                                    <br /><br /><br />

                                    <ul id="lstEvents" data-role="none" style="list-style-type: none; margin:0 auto; padding:0;">
                                        
                                    </ul>

                                    <div class="floatingDiv-bottomcenter">
                                        <div>
                                            <button data-role="none" id="btnBackListOfEvents" data-title="Back" class="btn custom-btn-theme-c btnCircle btnShadow">
                                                <img alt="Cancel" src="/Content/images/arrow-left.png" style="width:18px;" />
                                            </button>
                                        </div>                
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>        
        </div>
        <div style="clear:both;"></div>
    </div>
</div>
<br /><br />
<div id="toTop" title="Back to Top"><img src="/Content/images/arrow-up.png" alt="Back to Top" /></div>