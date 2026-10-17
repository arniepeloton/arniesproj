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
    require([GetPathFormat('/Scripts/sources/OrganizationShort/NewCustomerPage.js?v=' + assetversion),
            GetPathFormat('/Scripts/sources/CRMDashboard/SmSUsage.js?v=' + assetversion)
            //GetPathFormat('/Scripts/sources/CRMDashboard/SmSUsage.js?crc=')
            ]);
</script>

<style type="text/css">
    .dataTables_processing {
        /*Overwrite the Process Loading in datatables*/
        top: 0 !important;
        line-height: 50vh !important;
        height: 100% !important;
        margin-top: -20px !important;
    }
</style>

<div subpage="pageCustomer" id="divCustomerLoading" style="display:block;">Loading, please wait ...</div>

<div subpage="pageCustomer" id="divCustomerMainPage" style="display:none">

    @*Start Pop Ups*@
    <div>
        <div id="popContactPersonProfile" class="modal fade" tabindex="-1">
            <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">Contact Person Profile</h5>
                        <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                    </div>
                    <div class="modal-body">
                        <input type="text" id="txtPeopleID" style="display:none;" data-role="none" />
                        <input type="text" id="txtContactPersonID" style="display:none;" data-role="none" />

                        <ul class="nav nav-tabs">
                            <li class="nav-item">
                                <a class="nav-link active" data-toggle="tab" href="#tabContactPersonProfile">Profile</a>
                            </li>
                            <li>
                                <a class="nav-link" data-role="none" data-toggle="tab" href="#tabContactPersonPublicLink">E-Commerce</a>
                            </li>
                        </ul>

                        <div class="tab-content">
                            <div id="tabContactPersonProfile" class="tab-pane fade pt-3 show active">
                                <span id="spanerrorContactFname" class="mb-3" style="color:#F00; display:none;">This field is required</span>
                                <div class="row mb-3">
                                    <label class="col-sm-3 mb-3 col-form-label" for="txtContactFirstName">Full Name:</label>
                                    <div class="col-sm-3">
                                        <input type="text" data-role="none" class="form-control" id="txtContactFirstName" placeholder="Firstname" />
                                    </div>
                                    <div class="col-sm-3">
                                        <input type="text" data-role="none" class="form-control" id="txtContactMiddleName" placeholder="Middlename" />
                                    </div>
                                    <div class="col-sm-3">
                                        <input type="text" data-role="none" class="form-control" id="txtContactLastName" placeholder="Lastname" />
                                    </div>
                                </div>

                                <div class="row mb-3">
                                    <label class="col-sm-3 mb-3 col-form-label" for="selContactGenSuffix">Generational Suffix:</label>
                                    <div class="col-sm-9">
                                        <select id="selContactGenSuffix" data-role="none" class="form-select">
                                            <option value="">None</option>
                                            <option value="Jr.">Jr.</option>
                                            <option value="Sr.">Sr.</option>
                                            <option value="I">I</option>
                                            <option value="II">II</option>
                                            <option value="III">III</option>
                                            <option value="IV">IV</option>
                                            <option value="V">V</option>
                                            <option value="VI">VI</option>
                                            <option value="VII">VII</option>
                                            <option value="VIII">VIII</option>
                                            <option value="IX">IX</option>
                                            <option value="X">X</option>
                                        </select>
                                    </div>
                                </div>

                                <div class="row mb-3">
                                    <label class="col-sm-3 mb-3 col-form-label" for="txtContactPersonTIN">TIN:</label>
                                    <div class="col-sm-9">
                                        <input type="number" data-role="none" class="form-control" id="txtContactPersonTIN" />
                                    </div>
                                </div>

                                <div class="row mb-3">
                                    <label class="col-sm-3 mb-3 col-form-label" for="txtContactPersonPosition">Position/Title:</label>
                                    <div class="col-sm-9">
                                        <input type="text" data-role="none" class="form-control" id="txtContactPersonPosition" />
                                    </div>
                                </div>

                                <div class="row mb-3">
                                    <label class="col-sm-3 mb-3 col-form-label" for="txtContactPersonPositionDescription">Position/Title Description:</label>
                                    <div class="col-sm-9">
                                        <input type="text" data-role="none" class="form-control" id="txtContactPersonPositionDescription" />
                                    </div>
                                </div>

                                <div class="accordion mb-3">
                                    <div class="card">
                                        <div class="card-header p-0">
                                            <button data-target="#" class="btn btn-light btn-block text-left p-3 rounded-0" data-role="none" data-toggle="collapse">
                                                Address
                                            </button>
                                        </div>
                                        <div id="" class="collapse show">
                                            <div class="card-body">
                                                <input data-role="none" type="text" id="txtContactPersonAddressID" style="display:none;" />
                                                <div class="row mb-3">
                                                    <label class="col-sm-3 mb-3 col-form-label" for="txtContactPersonHouseBuildingNo">House or Building No:</label>
                                                    <div class="col-sm-9">
                                                        <input type="text" data-role="none" class="form-control" id="txtContactPersonHouseBuildingNo" />
                                                    </div>
                                                </div>

                                                <div class="row mb-3">
                                                    <label class="col-sm-3 mb-3 col-form-label" for="txtContactPersonStreet">Street:</label>
                                                    <div class="col-sm-9">
                                                        <input type="text" data-role="none" class="form-control" id="txtContactPersonStreet" />
                                                    </div>
                                                </div>

                                                <div class="row mb-3">
                                                    <label class="col-sm-3 mb-3 col-form-label" for="selContactPersonCity">City Or Town:</label>
                                                    <div class="col-sm-9">
                                                        <select id="selContactPersonCity" data-role="none" class="form-select"></select>
                                                    </div>
                                                </div>

                                                <div class="row mb-3">
                                                    <label class="col-sm-3 mb-3 col-form-label" for="selContactPersonCountry">Country:</label>
                                                    <div class="col-sm-9">
                                                        <select id="selContactPersonCountry" data-role="none" class="form-select"></select>
                                                    </div>
                                                </div>

                                                <div class="row mb-3">
                                                    <label class="col-sm-3 mb-3 col-form-label" for="selContactPersonTypeAddress">Type of Address:</label>
                                                    <div class="col-sm-9">
                                                        <select id="selContactPersonTypeAddress" data-role="none" class="form-select">
                                                            <option value="Permanent Address">Permanent Address</option>
                                                            <option value="Non-Permanent Address">Non-Permanent Address</option>
                                                            <option value="Provincial Address">Provincial Address</option>
                                                        </select>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <h3 class="mb-3">Person Contact Info</h3>

                                <div>
                                    <div class="row mb-3 align-items-center">
                                        <div class="col-sm-6">
                                            <h5>Telephone Number</h5>
                                        </div>
                                        <div class="col-sm-6 text-right">
                                            <button id="btnAddContactPersonTelephone" data-role="none" class="btn btn-success">
                                                Add Telephone
                                            </button>
                                        </div>
                                    </div>

                                    <div data-role="none" id="lstContactPersonTelephone" class="list-group mb-4"></div>
                                </div>

                                <div>
                                    <div class="row mb-3 align-items-center">
                                        <div class="col-sm-6">
                                            <h5>Mobile Number</h5>
                                        </div>
                                        <div class="col-sm-6 text-right">
                                            <button id="btnAddContactPersonMobile" data-role="none" class="btn btn-success">
                                                Add Mobile
                                            </button>
                                        </div>
                                    </div>

                                    <div data-role="none" id="lstContactPersonMobile" class="list-group mb-4"></div>
                                </div>

                                <div>
                                    <div class="row mb-3 align-items-center">
                                        <div class="col-sm-6">
                                            <h5>E-mail Address</h5>
                                        </div>
                                        <div class="col-sm-6 text-right">
                                            <button id="btnAddContactPersonEmail" data-role="none" class="btn btn-success">
                                                Add E-mail
                                            </button>
                                        </div>
                                    </div>

                                    <div data-role="none" id="lstContactPersonEmail" class="list-group mb-4"></div>
                                </div>
                            </div>

                            <div id="tabContactPersonPublicLink" class="tab-pane fade pt-3">
                                <div class="mb-3">Note: The contact person can revisit this URL anytime whenever he/she has forgotten his/her sigin credentials.</div>
                                
                                <div id="divForGenerateCPLink" class="mb-3" subpage="tabContactPersonPublicLink" style="display:none;">
                                    Please click the button below to generate a Public url for contact person where he/she can create his/her own
                                    sigin credentials for E-Commerce.
                                    <br /><br />
                                    <button data-role="none" class="btn btn-secondary btn-block" id="btnCPGeneratePublicUrl">Generate Public URL</button>
                                </div>
                                <div id="divForGeneratedCPLink" subpage="tabContactPersonPublicLink" style="display:none;">
                                    <div class="row mb-3">
                                        <div class="col-sm-10 mb-3" for="txtContactPersonPositionDescription">
                                            <input type="text" data-role="none" class="form-control" id="txtCPLink" readonly />
                                        </div>
                                        <div class="col-sm-2">
                                            <button data-role="none" class="btn btn-success" id="btnCPCopyUrl">Copy URL</button>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>                        
                    </div>
                    <div class="modal-footer">
                        <div class="row">
                            <div class="col-6">
                                <button data-role="none" type="button" id="btnCancelContact" class="btn btn-secondary btn-block" data-dismiss="modal">Cancel</button>
                            </div>
                            <div class="col-6">
                                <button data-role="none" type="button" id="btnOkContact" class="btn btn-success btn-block">Ok</button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div id="popSMSLogs" class="modal fade" tabindex="-1">
            <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">SMS Logs</h5>
                        <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                    </div>
                    <div class="modal-body">
                        <table data-role="none" class="table table-striped table-hover mb-4">
                            <thead>
                                <tr class="text-center">

                                    <th style="cursor:pointer;width:50%">  </th>
                                    <th style="cursor:pointer;text-align:right; width:16%"> Device To Modem </th>
                                    <th style="cursor:pointer;text-align:right; width:16%"> Modem To Telco </th>
                                    <th style="cursor:pointer;text-align:right; width:18%"> </th>
                                </tr>
                            </thead>
                            <tbody id="tblSmSUsagebody"></tbody>
                        </table>

                        <table data-role="none" class="table table-striped table-hover">
                            <thead>
                                <tr class="text-center">
                                    <th style="cursor:pointer; width:200px;"> Sent To </th>
                                    <th style="cursor:pointer;text-align:left; width:300px;"> SMS Message </th>
                                    <th style="cursor:pointer;text-align:left"> Date and Time </th>
                                    <th style="cursor:pointer;text-align:left"> Sent By </th>
                                    <th style="cursor:pointer;text-align:left"> Status </th>
                                </tr>
                            </thead>
                            <tbody id="tblSmSUsageDetailbody"></tbody>
                        </table>
                    </div>
                    <div class="modal-footer">
                        <div class="row">
                            <div class="col-12">
                                <button data-role="none" type="button" class="btn btn-secondary btn-block" data-dismiss="modal">Close</button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div id="popInteractionHistoryDetails" class="modal fade" tabindex="-1">
            <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">Interaction Details</h5>
                        <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                    </div>
                    <div class="modal-body">
                        <div class="row mb-3">
                            <div class="col-8">
                                <div class="row mb-3">
                                    <div class="col-sm-3 mb-3">Client Name:</div>
                                    <div class="col-sm-9">
                                        <div id="txtInteractionHistoryDetailsClientName" style="text-transform:capitalize"></div>
                                    </div>
                                </div>

                                <div class="row mb-3">
                                    <div class="col-sm-3 mb-3">Called By:</div>
                                    <div class="col-sm-9">
                                        <div id="txtInteractionHistoryDetailsCalledBy" style="text-transform:capitalize"></div>
                                    </div>
                                </div>

                                <div class="row mb-3">
                                    <div class="col-sm-3 mb-3">Call Duration:</div>
                                    <div class="col-sm-9">
                                        <div id="txtInteractionHistoryDetailsCallDuration" style="text-transform:capitalize"></div>
                                    </div>
                                </div>
                            </div>
                            <div class="col-4">
                                <div class="row mb-3">
                                    <div class="col-sm-3 mb-3">Start:</div>
                                    <div class="col-sm-9">
                                        <div id="txtInteractionHistoryDetailsStart" style="text-transform:capitalize"></div>
                                    </div>
                                </div>
                                <div class="row mb-3">
                                    <div class="col-sm-3 mb-3">End:</div>
                                    <div class="col-sm-9">
                                        <div id="txtInteractionHistoryDetailsEnd" style="text-transform:capitalize"></div>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="mb-3">
                            <label class="form-label mb-3">Notes:</label>
                            <div id="txtInteractionHistoryDetailsNotes" style="padding:10px; border:1px solid #CCC; border-radius:5px; background-color:#F0F0F0;"></div>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <div class="row">
                            <div class="col-12">
                                <button data-role="none" type="button" id="btnInteractionHistoryDetailsOk" class="btn btn-secondary btn-block" data-dismiss="modal">Close</button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>      
        
        <div id="divtooltipNotes" class="modal fade" tabindex="-1">
            <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">Date and Time: <span id="tooltipDateAndTime" style="color:#FFF;"></span></h5>
                        <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                    </div>
                    <div class="modal-body">
                        <div class="mb-3">
                            <label class="form-label mb-3">Notes:</label>
                            <div id="tooltipNotes" style="padding:10px; border:1px solid #CCC; border-radius:5px; background-color:#F0F0F0;"></div>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <div class="row">
                            <div class="col-12">
                                <button data-role="none" type="button" id="btnInteractionHistoryDetailsOk" class="btn btn-secondary btn-block" data-dismiss="modal">Close</button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div id="popNewToDo" class="modal fade" tabindex="-1">
            <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">New To-Do</h5>
                        <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                    </div>
                    <div class="modal-body">
                        <span class="mb-3" style="font-size:12px;">Fields marked with <span style="color:#F00;">*</span> are required.</span>
                        
                        <div class="row mb-3">
                            <div class="form-label" for="">
                                To-Do: <span style="color:#F00;">*</span>
                                <br />
                                <span id="spanerrorToDo" class="mb-3" style="color:#F00; display:none;">This field is required</span>
                            </div>
                            <textarea id="" style="height:100px; max-height:100px; overflow:auto;"></textarea>
                        </div>

                        <div class="row mb-3">
                            <div class="form-label" for="">Due Date:</div>
                            <button id="btnToDoSetDate2" data-role="none" class="btn btn-secondary">Set Date</button>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <div class="row">
                            <div class="col-6">
                                <button data-role="none" type="button" id="btnCancelToDo" class="btn btn-secondary btn-block" data-dismiss="modal">Cancel</button>
                            </div>
                            <div class="col-6">
                                <button data-role="none" type="button" id="" class="btn btn-success btn-block">Ok</button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div id="popToDo" class="modal fade" tabindex="-1">
            <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">Edit To-Do</h5>
                        <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                    </div>
                    <div class="modal-body">
                        <input type="text" data-role="none" style="display:none;" id="txtToDoID" />
                        <div class="row mb-3">
                            <div class="form-label" for="txtEditToDo">To-Do Name:</div>
                            <input type="text" id="txtEditToDo" data-role="none" class="form-control" placeholder="To-Do Name" />
                        </div>

                        <div class="row mb-3">
                            <div class="form-label" for="btnEditToDoSetDate">Due Date:</div>
                            <button id="btnEditToDoSetDate" data-role="none" class="btn btn-secondary btn-block">Set Date</button>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <div class="row">
                            <div class="col-6">
                                <button data-role="none" type="button" id="btnCancelEditToDo" class="btn btn-secondary btn-block" data-dismiss="modal">Cancel</button>
                            </div>
                            <div class="col-6">
                                <button data-role="none" type="button" id="btnOkEditToDo" class="btn btn-success btn-block">Ok</button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div id="divAffiliation" class="modal fade" tabindex="-1">
            <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">Name: <span id="txtFormName" style="color:#FFF;"></span></h5>
                        <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                    </div>
                    <div class="modal-body">
                        <ol id="listFormAffiliation" data-role="none" class="list-group"></ol>
                    </div>
                    <div class="modal-footer">
                        <div class="row">
                            <div class="col-12">
                                <button data-role="none" type="button" id="btnInteractionHistoryDetailsOk" class="btn btn-secondary btn-block" data-dismiss="modal">Close</button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div id="popPersonTelInfo" class="modal fade" tabindex="-1">
            <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">Telephone Info</h5>
                        <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                    </div>
                    <div class="modal-body">
                        <div style="display:none;">
                            <input type="text" data-role="none" class="form-control" readonly id="txtPersonTelCountryID" value="e783ed13-aa98-4cc1-918e-d49b05c0167e" />
                            <input type="text" data-role="none" class="form-control" readonly id="txtPersonTelID" />
                            <input type="text" data-role="none" class="form-control" readonly id="txtPersonTelAreaCodeID" />
                        </div>

                        <div class="mb-3">
                            <label class="form-label" for="btnPersonTelCountryCodeDisable">
                                Country Code: <span style="color: #FF0000;">*</span>
                                <span id="spanerrorPersonTelCountryCode" style="color:#F00; display:none;">This field is required</span>
                            </label>
                            <button id="btnPersonTelCountryCodeDisable" data-role="none" class="btn btn-secondary btn-block"><span id="txtPersonCountryCode">PHL - 63</span></button>
                        </div>

                        <div class="mb-3">
                            <label class="form-label" for="btnPersonTelAreaCode">
                                Area Code: <span style="color: #FF0000;">*</span>
                                <span id="spanerrorPersonAreaCode" style="color:#F00; display:none;">This field is required</span>
                            </label>
                            <button id="btnPersonTelAreaCode" data-role="none" class="btn btn-secondary btn-block"><span id="txtPersonAreaCode">Please Select</span></button>
                        </div>

                        <div class="mb-3">
                            <label class="form-label" for="txtPersonTelephoneNum">
                                Telephone Number: <span style="color: #FF0000;">*</span>
                                <span id="spanerrorPersonTelephoneNum" style="color:#F00; display:none;">This field is required</span>
                                <span id="spanerrorPersonTelephoneNumValidate" style="color:#F00; display:none;">Enter 7 digit number</span>
                            </label>
                            <input type="text" data-role="none" class="form-control" id="txtPersonTelephoneNum" />
                        </div>

                        <div class="mb-3">
                            <label class="form-label" for="txtPersonTelephoneExt">Extension or Local No.:</label>
                            <input type="text" data-role="none" class="form-control" id="txtPersonTelephoneExt" maxlength="10" />
                        </div>

                        <div class="mb-3">
                            <label class="form-label" for="selPersonTelType">Contact Type: <span style="color: #FF0000;">*</span> <span id="spanerrorPersonTelType" style="color:#F00; display:none;">This field is required</span></label>
                            <select id="selPersonTelType" data-role="none" class="form-select">
                                <option value="">Please Select</option>
                                <option value="Work Telephone">Work Telephone</option>
                                <option value="Personal Telephone">Personal Telephone</option>
                                <option value="Third Party Telephone">Third Party Telephone</option>
                            </select>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <div class="row">
                            <div class="col-6">
                                <button data-role="none" type="button" id="btnCancelPersonTelephone" class="btn btn-secondary btn-block" data-dismiss="modal">Cancel</button>
                            </div>
                            <div class="col-6">
                                <button data-role="none" type="button" id="btnSavePersonTelephone" class="btn btn-success btn-block">Ok</button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div id="popPersonMobileInfo" class="modal fade" tabindex="-1">
            <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">Mobile Info</h5>
                        <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                    </div>
                    <div class="modal-body">
                        <div style="display:none;">
                            <input type="text" data-role="none" class="form-control" readonly id="txtPersonMobileCountryID" value="e783ed13-aa98-4cc1-918e-d49b05c0167e" />
                            <input type="text" data-role="none" class="form-control" readonly id="txtPersonMobileID" />
                            <input type="text" data-role="none" class="form-control" readonly id="txtPersonTelCoPrefixID" />
                        </div>

                        <div class="mb-3">
                            <label class="form-label" for="btnPersonMobileCountryCodeDisable">
                                Country Code: <span style="color: #FF0000;">*</span>
                                <span id="spanerrorPersonMobileCountryCode" style="color:#F00; display:none;">This field is required</span>
                            </label>
                            <button id="btnPersonMobileCountryCodeDisable" data-role="none" class="btn btn-secondary btn-block"><span id="txtPersonMobileCountryCode">PHL - 63</span></button>
                        </div>

                        @*<div class="mb-3">
                            <label class="form-label" for="btnPersonMobileCountryCodeDisable">
                                Tel Code: <span style="color: #FF0000;">*</span>
                                <span id="spanerrorPersonMobileTelCode" style="color:#F00; display:none;">This field is required</span>
                            </label>
                            <button id="btnPersonMobileTelCode" data-role="none" class="btn btn-secondary btn-block"><span id="txtPersonTelCode">Select</span></button>
                        </div>*@

                        <div class="mb-3">
                            <label class="form-label" for="txtPersonMobileNo">
                                Mobile Number: <span style="color: #FF0000;">*</span>
                                <span id="spanerrorPersonMobileNum" style="color:#F00; display:none;">This field is required</span>
                                <span id="spanerrorPersonMobileNumValidate" style="color:#F00; display:none;">Enter 10 digit number</span>
                            </label>
                            <input type="number" id="txtPersonMobileNo" data-role="none" class="form-control" placeholder="Eg. 91512345678" />
                        </div>

                        <div class="mb-3">
                            <label class="form-label" for="selPersonMobileType">Contact Type: <span style="color: #FF0000;">*</span> <span id="spanerrorPersonMobileType" style="color:#F00; display:none;">This field is required</span></label>
                            <select id="selPersonMobileType" data-role="none" class="form-select">
                                <option value="">Please Select</option>
                                <option value="Work Mobile">Work Mobile</option>
                                <option value="Personal Mobile">Personal Mobile</option>
                                <option value="Third Party Mobile">Third Party Mobile</option>
                            </select>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <div class="row">
                            <div class="col-6">
                                <button data-role="none" type="button" id="btnCancelPersonMobile" class="btn btn-secondary btn-block" data-dismiss="modal">Cancel</button>
                            </div>
                            <div class="col-6">
                                <button data-role="none" type="button" id="btnSavePersonMobile" class="btn btn-success btn-block">Ok</button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>      
        
        <div id="popPersonEmailInfo" class="modal fade" tabindex="-1">
            <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">E-mail Info</h5>
                        <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                    </div>
                    <div class="modal-body">
                        <div style="display:none;">
                            <input type="text" data-role="none" class="form-control" readonly id="txtPersonEmailID" />
                        </div>

                        <div class="mb-3">
                            <label class="form-label" for="txtPersonEmailAdd">
                                Email Address: <span style="color: #FF0000;">*</span>
                                <span id="spanerrorPersonEmailAddress" style="color:#F00; display:none;">This field is required</span>
                                <span id="spanerrorPersonEmailAddressValidate" style="color:#F00; display:none;">Enter valid email address</span>
                            </label>
                            <input type="email" id="txtPersonEmailAdd" data-role="none" class="form-control" />
                        </div>

                        <div class="mb-3">
                            <label class="form-label" for="selPersonEmailType">Contact Type: <span style="color: #FF0000;">*</span> <span id="spanerrorPersonEmailType" style="color:#F00; display:none;">This field is required</span></label>
                            <select id="selPersonEmailType" data-role="none" class="form-select">
                                <option value="">Please Select</option>
                                <option value="Work Email">Work Email</option>
                                <option value="Personal Email">Personal Email</option>
                                <option value="Third Party Email">Third Party Email</option>
                            </select>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <div class="row">
                            <div class="col-6">
                                <button data-role="none" type="button" id="btnCancelPersonEmailAddress" class="btn btn-secondary btn-block" data-dismiss="modal">Cancel</button>
                            </div>
                            <div class="col-6">
                                <button data-role="none" type="button" id="btnSavePersonEmailAddress" class="btn btn-success btn-block">Ok</button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div id="popOrgTelInfo" class="modal fade" tabindex="-1">
            <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">Telephone Info</h5>
                        <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                    </div>
                    <div class="modal-body">
                        <div style="display:none;">
                            <input type="text" data-role="none" class="form-select" readonly id="txtOrgTelCountryID" value="e783ed13-aa98-4cc1-918e-d49b05c0167e" />
                            <input type="text" data-role="none" class="form-select" readonly id="txtOrgTelID" />
                            <input type="text" data-role="none" class="form-select" readonly id="txtOrgTelAreaCodeID" />
                        </div>

                        <div class="mb-3">
                            <label class="form-label" for="btnOrgTelCountryCodeDisable">
                                Country Code: <span style="color: #FF0000;">*</span>
                                <span id="spanerrorTelCountryCode" style="color:#F00; display:none;">This field is required</span>
                            </label>
                            <button id="btnOrgTelCountryCodeDisable" data-role="none" class="btn btn-secondary btn-block"><span id="txtCountryCode">PHL - 63</span></button>
                        </div>

                        <div class="mb-3">
                            <label class="form-label" for="btnOrgTelAreaCode">
                                Area Code: <span style="color: #FF0000;">*</span>
                                <span id="spanerrorAreaCode" style="color:#F00; display:none;">This field is required</span>
                            </label>
                            <button id="btnOrgTelAreaCode" data-role="none" class="btn btn-secondary btn-block"><span id="txtAreaCode">Please Select</span></button>
                        </div>

                        <div class="mb-3">
                            <label class="form-label" for="txtTelephoneNum">
                                Telephone Number: <span style="color: #FF0000;">*</span>
                                <span id="spanerrorTelephoneNum" style="color:#F00; display:none;">This field is required</span>
                                <span id="spanerrorTelephoneNumValidate" style="color:#F00; display:none;">Enter 7 digit number</span>
                            </label>
                            <input type="number" data-role="none" class="form-control" id="txtTelephoneNum" />
                        </div>

                        <div class="mb-3">
                            <label class="form-label" for="txtTelephoneExt">Extension or Local No.:</label>
                            <input type="text" data-role="none" class="form-control" id="txtTelephoneExt" maxlength="10" />
                        </div>

                        <div class="mb-3">
                            <label class="form-label" for="selOrgTelType">Contact Type: <span style="color: #FF0000;">*</span> <span id="spanerrorTelType" style="color:#F00; display:none;">This field is required</span></label>
                            <select id="selOrgTelType" data-role="none" class="form-select">
                                <option value="">Please Select</option>
                                <option value="Work Telephone">Work Telephone</option>
                                <option value="Personal Telephone">Personal Telephone</option>
                                <option value="Third Party Telephone">Third Party Telephone</option>
                            </select>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <div class="row">
                            <div class="col-6">
                                <button data-role="none" type="button" id="btnCancelOrgTelephone" class="btn btn-secondary btn-block" data-dismiss="modal">Cancel</button>
                            </div>
                            <div class="col-6">
                                <button data-role="none" type="button" id="btnSaveOrgTelephone" class="btn btn-success btn-block">Ok</button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div id="popOrgMobileInfo" class="modal fade" tabindex="-1">
            <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">Mobile Info</h5>
                        <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                    </div>
                    <div class="modal-body">
                        <div style="display:none;">
                            <input type="text" data-role="none" class="form-control" readonly id="txtOrgMobileCountryID" value="e783ed13-aa98-4cc1-918e-d49b05c0167e" />
                            <input type="text" data-role="none" class="form-control" readonly id="txtOrgMobileID" />
                            <input type="text" data-role="none" class="form-control" readonly id="txtOrgTelCoPrefixID" />
                        </div>

                        <div class="mb-3">
                            <label class="form-label" for="btnOrgMobileCountryCodeDisable">
                                Country Code: <span style="color: #FF0000;">*</span>
                                <span id="spanerrorMobileCountryCode" style="color:#F00; display:none;">This field is required</span>
                            </label>
                            <button id="btnOrgMobileCountryCodeDisable" data-role="none" class="btn btn-secondary btn-block"><span id="txtMobileCountryCode">PHL - 63</span></button>
                        </div>

                        @*<div class="mb-3">
        <label class="form-label" for="btnOrgMobileTelCode">
            Tel Code: <span style="color: #FF0000;">*</span>
            <span id="spanerrorMobileTelCode" style="color:#F00; display:none;">This field is required</span>
        </label>
        <button id="btnOrgMobileTelCode" data-role="none" class="btn btn-secondary btn-block"><span id="txtTelCode">Please Select</span></button>
    </div>*@

                        <div class="mb-3">
                            <label class="form-label" for="txtMobileNo">
                                Mobile Number: <span style="color: #FF0000;">*</span>
                                <span id="spanerrorMobileNum" style="color:#F00; display:none;">This field is required</span>
                                <span id="spanerrorMobileNumValidate" style="color:#F00; display:none;">Enter 7 digit number</span>
                            </label>
                            <input type="number" id="txtMobileNo" data-role="none" class="form-control" />
                        </div>

                        <div class="mb-3">
                            <label class="form-label" for="selOrgMobileType">Contact Type: <span style="color: #FF0000;">*</span> <span id="spanerrorMobileType" style="color:#F00; display:none;">This field is required</span></label>
                            <select id="selOrgMobileType" data-role="none" class="form-select">
                                <option value="">Please Select</option>
                                <option value="Work Mobile">Work Mobile</option>
                                <option value="Personal Mobile">Personal Mobile</option>
                                <option value="Third Party Mobile">Third Party Mobile</option>
                            </select>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <div class="row">
                            <div class="col-6">
                                <button data-role="none" type="button" id="btnCancelOrgMobile" class="btn btn-secondary btn-block" data-dismiss="modal">Cancel</button>
                            </div>
                            <div class="col-6">
                                <button data-role="none" type="button" id="btnSaveOrgMobile" class="btn btn-success btn-block">Ok</button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div id="popOrgEmailInfo" class="modal fade" tabindex="-1">
            <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">E-mail Info</h5>
                        <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                    </div>
                    <div class="modal-body">
                        <div style="display:none;">
                            <input type="text" data-role="none" class="form-control" readonly id="txtOrgEmailID" />
                        </div>

                        <div class="mb-3">
                            <label class="form-label" for="txtEmailAdd">
                                Email Address: <span style="color: #FF0000;">*</span>
                                <span id="spanerrorEmailAddress" style="color:#F00; display:none;">This field is required</span>
                                <span id="spanerrorEmailAddressValidate" style="color:#F00; display:none;">Enter valid email address</span>
                            </label>
                            <input type="email" id="txtEmailAdd" data-role="none" class="form-control" />
                        </div>

                        <div class="mb-3">
                            <label class="form-label" for="selOrgEmailType">Contact Type: <span style="color: #FF0000;">*</span> <span id="spanerrorEmailType" style="color:#F00; display:none;">This field is required</span></label>
                            <select id="selOrgEmailType" data-role="none" class="form-control">
                                <option value="">Please Select</option>
                                <option value="Work Email">Work Email</option>
                                <option value="Personal Email">Personal Email</option>
                                <option value="Third Party Email">Third Party Email</option>
                            </select>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <div class="row">
                            <div class="col-6">
                                <button data-role="none" type="button" id="btnCancelOrgEmailAddress" class="btn btn-secondary btn-block" data-dismiss="modal">Cancel</button>
                            </div>
                            <div class="col-6">
                                <button data-role="none" type="button" id="btnSaveOrgEmailAddress" class="btn btn-success btn-block">Ok</button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div id="popOrgWebsiteInfo" class="modal fade" tabindex="-1">
            <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">Website Info</h5>
                        <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                    </div>
                    <div class="modal-body">
                        <div style="display:none;">
                            <input type="text" data-role="none" class="form-control" readonly id="txtOrgWebsiteID" />
                        </div>

                        <div class="mb-3">
                            <label class="form-label" for="txtWebsiteUrl">
                                Website URL: <span style="color: #FF0000;">*</span>
                                <span id="spanerrorWebsiteUrl" style="color:#F00; display:none;">This field is required</span>
                            </label>
                            <input type="text" id="txtWebsiteUrl" data-role="none" class="form-control" />
                        </div>
                    </div>
                    <div class="modal-footer">
                        <div class="row">
                            <div class="col-6">
                                <button data-role="none" type="button" id="btnCancelOrgWebsite" class="btn btn-secondary btn-block" data-dismiss="modal">Cancel</button>
                            </div>
                            <div class="col-6">
                                <button data-role="none" type="button" id="btnSaveOrgWebsite" class="btn btn-success btn-block">Ok</button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div id="popContactPersonTelInfo" class="modal fade" tabindex="-1">
            <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">Telephone Info</h5>
                        <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                    </div>
                    <div class="modal-body">
                        <div style="display:none;">
                            <input type="text" data-role="none" class="form-control" readonly id="txtContactPersonTelCountryID" value="e783ed13-aa98-4cc1-918e-d49b05c0167e" />
                            <input type="text" data-role="none" class="form-control" readonly id="txtContactPersonTelID" />
                            <input type="text" data-role="none" class="form-control" readonly id="txtContactPersonTelAreaCodeID" />
                        </div>

                        <div class="mb-3">
                            <label class="form-label" for="btnContactPersonTelCountryCodeDisable">
                                Country Code: <span style="color: #FF0000;">*</span>
                                <span id="spanerrorContactPersonTelCountryCode" style="color:#F00; display:none;">This field is required</span>
                            </label>
                            <button id="btnContactPersonTelCountryCodeDisable" data-role="none" class="btn btn-secondary btn-block"><span id="txtContactPersonCountryCode">PHL - 63</span></button>
                        </div>

                        <div class="mb-3">
                            <label class="form-label" for="btnContactPersonTelAreaCode">
                                Area Code: <span style="color: #FF0000;">*</span>
                                <span id="spanerrorContactPersonAreaCode" style="color:#F00; display:none;">This field is required</span>
                            </label>
                            <button id="btnContactPersonTelAreaCode" data-role="none" class="btn btn-secondary btn-block"><span id="txtContactPersonAreaCode">Please Select</span></button>
                        </div>

                        <div class="mb-3">
                            <label class="form-label" for="txtContactPersonTelephoneNum">
                                Telephone Number: <span style="color: #FF0000;">*</span>
                                <span id="spanerrorContactPersonTelephoneNum" style="color:#F00; display:none;">This field is required</span>
                                <span id="spanerrorContactPersonTelephoneNumValidate" style="color:#F00; display:none;">Enter 7 digit number</span>
                            </label>
                            <input type="number" data-role="none" class="form-control" id="txtContactPersonTelephoneNum" />
                        </div>

                        <div class="mb-3">
                            <label class="form-label" for="txtContactPersonTelephoneExt">Extension or Local No.:</label>
                            <input type="text" data-role="none" class="form-control" id="txtContactPersonTelephoneExt" maxlength="10" />
                        </div>

                        <div class="mb-3">
                            <label class="form-label" for="selContactPersonTelType">Contact Type: <span style="color: #FF0000;">*</span> <span id="spanerrorContactPersonTelType" style="color:#F00; display:none;">This field is required</span></label>
                            <select id="selContactPersonTelType" data-role="none" class="form-select">
                                <option value="">Please Select</option>
                                <option value="Work Telephone">Work Telephone</option>
                                <option value="Personal Telephone">Personal Telephone</option>
                                <option value="Third Party Telephone">Third Party Telephone</option>
                            </select>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <div class="row">
                            <div class="col-6">
                                <button data-role="none" type="button" id="btnCancelContactPersonTelephone" class="btn btn-secondary btn-block" data-dismiss="modal">Cancel</button>
                            </div>
                            <div class="col-6">
                                <button data-role="none" type="button" id="btnSaveContactPersonTelephone" class="btn btn-success btn-block">Ok</button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div id="popContactPersonMobileInfo" class="modal fade" tabindex="-1">
            <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">Mobile Info</h5>
                        <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                    </div>
                    <div class="modal-body">
                        <div style="display:none;">
                            <input type="text" data-role="none" class="form-control" readonly id="txtContactPersonMobileCountryID" value="e783ed13-aa98-4cc1-918e-d49b05c0167e" />
                            <input type="text" data-role="none" class="form-control" readonly id="txtContactPersonMobileID" />
                            <input type="text" data-role="none" class="form-control" readonly id="txtContactPersonTelCoPrefixID" />
                        </div>

                        <div class="mb-3">
                            <label class="form-label" for="btnContactPersonMobileCountryCodeDisable">
                                Country Code: <span style="color: #FF0000;">*</span>
                                <span id="spanerrorContactPersonMobileCountryCode" style="color:#F00; display:none;">This field is required</span>
                            </label>
                            <button id="btnContactPersonMobileCountryCodeDisable" data-role="none" class="btn btn-secondary btn-block"><span id="txtContactPersonMobileCountryCode">PHL - 63</span></button>
                        </div>

                        @*<div class="mb-3">
        <label class="form-label" for="btnContactPersonMobileTelCode">
            Tel Code: <span style="color: #FF0000;">*</span>
            <span id="spanerrorContactPersonMobileTelCode" style="color:#F00; display:none;">This field is required</span>
        </label>
        <button id="btnContactPersonMobileTelCode" data-role="none" class="btn btn-secondary btn-block"><span id="txtContactPersonTelCode">Please Select</span></button>
    </div>*@

                        <div class="mb-3">
                            <label class="form-label" for="txtContactPersonMobileNo">
                                Mobile Number: <span style="color: #FF0000;">*</span>
                                <span id="spanerrorContactPersonMobileNum" style="color:#F00; display:none;">This field is required</span>
                                <span id="spanerrorContactPersonMobileNumValidate" style="color:#F00; display:none;">Enter 10 digit number</span>
                            </label>
                            <input type="number" id="txtContactPersonMobileNo" data-role="none" class="form-control" placeholder="Eg. 91512345678" />
                        </div>

                        <div class="mb-3">
                            <label class="form-label" for="selContactPersonMobileType">Contact Type: <span style="color: #FF0000;">*</span> <span id="spanerrorContactPersonMobileType" style="color:#F00; display:none;">This field is required</span></label>
                            <select id="selContactPersonMobileType" data-role="none" class="form-select">
                                <option value="">Please Select</option>
                                <option value="Work Mobile">Work Mobile</option>
                                <option value="Personal Mobile">Personal Mobile</option>
                                <option value="Third Party Mobile">Third Party Mobile</option>
                            </select>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <div class="row">
                            <div class="col-6">
                                <button data-role="none" type="button" id="btnCancelContactPersonMobile" class="btn btn-secondary btn-block" data-dismiss="modal">Cancel</button>
                            </div>
                            <div class="col-6">
                                <button data-role="none" type="button" id="btnSaveContactPersonMobile" class="btn btn-success btn-block">Ok</button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div id="popContactPersonEmailInfo" class="modal fade" tabindex="-1">
            <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">E-mail Info</h5>
                        <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                    </div>
                    <div class="modal-body">
                        <div style="display:none;">
                            <input type="text" data-role="none" class="form-control" readonly id="txtOrgEmailID" />
                        </div>

                        <div class="mb-3">
                            <label class="form-label" for="txtContactPersonEmailID">
                                Email Address: <span style="color: #FF0000;">*</span>
                                <span id="spanerrorContactPersonEmailAddress" style="color:#F00; display:none;">This field is required</span>
                                <span id="spanerrorContactPersonEmailAddressValidate" style="color:#F00; display:none;">Enter valid email address</span>
                            </label>
                            <input type="email" id="txtContactPersonEmailID" data-role="none" class="form-control" />
                        </div>

                        <div class="mb-3">
                            <label class="form-label" for="selContactPersonEmailType">Contact Type: <span style="color: #FF0000;">*</span> <span id="spanerrorContactPersonEmailType" style="color:#F00; display:none;">This field is required</span></label>
                            <select id="selContactPersonEmailType" data-role="none" class="form-control">
                                <option value="">Please Select</option>
                                <option value="Work Email">Work Email</option>
                                <option value="Personal Email">Personal Email</option>
                                <option value="Third Party Email">Third Party Email</option>
                            </select>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <div class="row">
                            <div class="col-6">
                                <button data-role="none" type="button" id="btnCancelContactPersonEmailAddress" class="btn btn-secondary btn-block" data-dismiss="modal">Cancel</button>
                            </div>
                            <div class="col-6">
                                <button data-role="none" type="button" id="btnSaveContactPersonEmailAddress" class="btn btn-success btn-block">Ok</button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div id="popSMS" class="modal fade" tabindex="-1">
            <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">SMS</h5>
                        <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                    </div>
                    <div class="modal-body">
                        <div style="display:none;">
                            <input type="text" data-role="none" class="form-control" readonly id="txtOrgEmailID" />
                        </div>

                        <div class="row mb-3 align-items-center">
                            <div class="col-8" for="txtRecipientsCount">
                                Recipients: <b id="txtRecipientsCount"></b>
                            </div>
                            <div class="col-4 text-right">
                                <button id="btnAddNewNumber" data-role="none" title="Add New Number" class="btn btn-success"><span class="bi-person-plus-fill"></span></button>
                                <button id="btnSearchContacts" data-role="none" title="Search Contacts" class="btn btn-secondary"><span class="bi-search"></span></button>
                                <button id="btnViewRecipientsDetails" data-role="none" title="View Details" class="btn btn-warning"><span class="bi-eye-fill"></span></button>
                            </div>
                        </div>

                        <div id="listofRecipient" class="mb-3" style="max-height: 150px; overflow:auto;"></div>

                        <span id="charNum" class="mb-3">Characters left: 160</span>
                        <textarea id="txtSMSMessage" max-length="160" style="height:300px; resize:none;"></textarea>
                    </div>
                    <div class="modal-footer">
                        <div class="row">
                            <div class="col-6">
                                <button data-role="none" type="button" id="btnCancelSMS" class="btn btn-secondary btn-block" data-dismiss="modal">Cancel</button>
                            </div>
                            <div class="col-6">
                                <button data-role="none" type="button" id="btnSendSMS" class="btn btn-success btn-block">Send</button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div id="popSMSRecipient" class="modal fade" tabindex="-1">
            <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">List of Contacts</h5>
                        <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                    </div>
                    <div class="modal-body">
                        <ul data-role="none" id="listSelectedReceipient" class="listContact list-group"></ul>
                    </div>
                    <div class="modal-footer">
                        <div class="row">
                            <div class="col-12">
                                <button data-role="none" type="button" id="btnOkSMSRecipient" class="btn btn-secondary btn-block" data-dismiss="modal">Ok</button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div id="popNewNumber" class="modal fade" tabindex="-1">
            <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">New Number</h5>
                        <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                    </div>
                    <div class="modal-body">
                        <div class="row mb-3">
                            <label class="form-label" for="">
                                Country Code: <span style="color: #FF0000;">*</span>
                            </label>
                            <button disabled data-role="none" class="btn btn-secondary btn-block">PHL - 63</button>
                        </div>

                        <div class="row mb-3">
                            <label class="form-label" for="btnNewNumberTelcoPrefix">
                                Telco Prefrix: <span style="color: #FF0000;">*</span>
                            </label>
                            <button id="btnNewNumberTelcoPrefix" data-role="none" class="btn btn-secondary btn-block">Please Select</button>
                        </div>

                        <div class="row mb-3">
                            <label class="form-label" for="btnNewNumberTelcoPrefix">
                                Enter 7 digit no.: <span style="color: #FF0000;">*</span>
                            </label>
                            <input type="number" class="form-control" data-role="none" id="txtNewNumber" maxlength="7" />
                        </div>
                    </div>
                    <div class="modal-footer">
                        <div class="row">
                            <div class="col-6">
                                <button data-role="none" type="button" id="btnCancelNewNumber" class="btn btn-secondary btn-block" data-dismiss="modal">Cancel</button>
                            </div>
                            <div class="col-6">
                                <button data-role="none" type="button" id="btnOkNewNumber" class="btn btn-success btn-block">Send</button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div id="popSearchContacts" class="modal fade" tabindex="-1">
            <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">Search Contacts</h5>
                        <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                    </div>
                    <div class="modal-body">
                        <input type="search" id="txtSearchContacts" data-role="none" class="form-control mb-3" placeholder="Search Name ..." />
                        <div class="text-right mb-3"><a data-role="none" id="lnkSelectAllSearchRecords" class="custom-link"></a></div>
                        <table data-role="none" class="table table-striped table-hover">
                            <thead>
                                <tr>
                                    <th class="bulk" style="cursor:pointer; width:20px;">
                                        <div class="form-check" style="font-size:20px;">
                                            <input class="form-check-input" type="checkbox" name="chbbulk" id="chbbulk">
                                        </div>
                                    </th>
                                    <th data-sort="accounting" style="cursor:pointer; width:20px;"> # </th>
                                    <th style="cursor:pointer; width:20px;"></th>
                                    <th data-sort="string" style="cursor:pointer;"> Name </th>
                                </tr>
                            </thead>
                            <tbody id="tblProspectbody"></tbody>
                        </table>
                    </div>
                    <div class="modal-footer">
                        <div class="row">
                            <div class="col-12">
                                <button data-role="none" type="button" id="" class="btn btn-secondary btn-block" data-dismiss="modal">Ok</button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div id="popFilter" class="modal fade" tabindex="-1">
            <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">Filter</h5>
                        <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                    </div>
                    <div class="modal-body">
                        <div subpage="pageErrorMsg" class="mb-3" id="msgCategory" style="color:#F00; display:none;">Please select category</div>
                        <div subpage="pageErrorMsg" class="mb-3" id="msgBusinessIndustry" style="color:#F00; display:none;">Please select business industry</div>
                        <div subpage="pageErrorMsg" class="mb-3" id="msgScope" style="color:#F00; display:none;">Please select scope</div>
                        <div subpage="pageErrorMsg" class="mb-3" id="msgConditionValue" style="color:#F00; display:none;">Please enter value</div>
                        <div subpage="pageErrorMsg" class="mb-3" id="msgCity" style="color:#F00; display:none;">Please select city</div>

                        <div class="row mb-3">
                            <label class="form-label" for="selCategory">Category:</label>
                            <select id="selCategory" data-role="none" class="form-select">
                                <option value="">Please Select</option>
                                <option value="Business Industry">Business Industry</option>
                                <option value="Company Size">Company Size</option>
                                <option value="City">City</option>
                            </select>
                        </div>

                        <div class="row mb-3" subpage="pageFilter" id="divBusinessIndustry" style="display:none;">
                            <label class="form-label" for="selFilterByBusinessIndustry">Business Industry:</label>
                            <select id="selFilterByBusinessIndustry" data-role="none" class="form-select"></select>
                        </div>

                        <div subpage="pageFilter" id="divCompanySize" style="display:none;">
                            <div class="row mb-3">
                                <label class="form-label" for="selFilterCompanySize">Filter By:</label>
                                <select id="selFilterCompanySize" data-role="none" class="form-select">
                                    <option value="NoOfEmployees">Employee</option>
                                    <option value="RevenuePerYear">Revenue Per Year</option>
                                    <option value="ScopeOfOperations">Scope of Operations</option>
                                    <option value="NoOfFactory">Factory</option>
                                    <option value="NoOfOffice">Office</option>
                                    <option value="NoOfWarehouse">Warehouse</option>
                                    <option value="NoOfStore">Store</option>
                                    <option value="NoOfTruck">Truck</option>
                                </select>
                            </div>

                            <div subpage="pageCompanySize" id="divScope" style="display:none;">
                                <div class="row mb-3">
                                    <label class="form-label" for="selScope">Scope:</label>
                                    <select id="selScope" data-role="none" class="form-select">
                                        <option value="">Please Select Scope</option>
                                        <option value="Regional">Regional</option>
                                        <option value="National">National</option>
                                        <option value="Trans-National">Trans-National</option>
                                    </select>
                                </div>
                            </div>

                            <div subpage="pageCompanySize" id="divOthers" style="display:block;">
                                <div class="row mb-3">
                                    <label class="form-label" for="selCondition">Condition:</label>
                                    <select id="selCondition" data-role="none" class="form-select mb-2">
                                        <option value="gt">( &gt; ) Greater Than</option>
                                        <option value="ge">( &ge; ) Greater Than And Equal To</option>
                                        <option value="e">( &#61; ) Equal To</option>
                                        <option value="lt">( &lt; ) Lesser Than</option>
                                        <option value="le">( &le; ) Lesser Than And Equal To</option>
                                    </select>
                                    <input type="text" id="txtConditionValue" placeholder="Enter Value" />
                                </div>
                            </div>
                        </div>

                        <div class="row mb-3" subpage="pageFilter" id="divCity" style="display:none;">
                            <label class="form-label" for="selFilterByCity">City:</label>
                            <select id="selFilterByCity" data-role="none" class="form-select"></select>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <div class="row">
                            <div class="col-6">
                                <button data-role="none" type="button" id="btnCancelFilter" class="btn btn-secondary btn-block" data-dismiss="modal">Cancel</button>
                            </div>
                            <div class="col-6">
                                <button data-role="none" type="button" id="btnOkFilter" class="btn btn-success btn-block" data-dismiss="modal">Ok</button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div id="popEducation" class="modal fade" tabindex="-1">
            <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">School</h5>
                        <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                    </div>
                    <div class="modal-body">
                        <input type="text" data-role="none" style="display:none;" id="txtEdeHiID" />
                        <div class=" mb-3">
                            <label class="form-label" for="txtEdeHiSchoolName">School Name:</label>
                            <input type="text" data-role="none" class="form-control" id="txtEdeHiSchoolName" />
                        </div>

                        <div class=" mb-3">
                            <label class="form-label" for="txtEdeHiYearStarted">Year Started:</label>
                            <input type="number" data-role="none" class="form-control" id="txtEdeHiYearStarted" />
                        </div>

                        <div class=" mb-3">
                            <label class="form-label" for="txtEdeHiYearGraduated">Year Graduated:</label>
                            <input type="number" data-role="none" class="form-control" id="txtEdeHiYearGraduated" />
                        </div>
                    </div>
                    <div class="modal-footer">
                        <div class="row">
                            <div class="col-6">
                                <button data-role="none" type="button" id="btnCancelEduHi" class="btn btn-secondary btn-block" data-dismiss="modal">Cancel</button>
                            </div>
                            <div class="col-6">
                                <button data-role="none" type="button" id="btnOkEduHi" class="btn btn-success btn-block" data-dismiss="modal">Ok</button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div id="prevOrgProfile" class="modal fade" tabindex="-1">
            <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable">
                <div class="modal-content">
                    <div class="modal-body">
                        <div id="roOrg" subpage="pagePreview" style="display:block;">
                            <h4 class="mb-4">Company Profile</h4>
                            <div class="row mb-3">
                                <div class="col-sm-3" for="txtEdeHiSchoolName">Company Name:</div>
                                <div class="col-sm-9" id="txtpreviewCompanyName"></div>
                            </div>
                            <div class="row mb-3">
                                <div class="col-sm-3">Business Trade Name:</div>
                                <div class="col-sm-9" id="txtpreviewCompanyBusinessTradeName"></div>
                            </div>
                            <div class="row mb-3">
                                <div class="col-sm-3">Address:</div>
                                <div class="col-sm-9" id="txtpreviewCompanyAddress"></div>
                            </div>
                            <div class="row mb-3">
                                <div class="col-sm-3">Background:</div>
                                <div class="col-sm-9" id="txtpreviewCompanyBackground"></div>
                            </div>
                            <div class="row mb-3">
                                <div class="col-sm-3">Mission:</div>
                                <div class="col-sm-9" id="txtpreviewCompanyMission"></div>
                            </div>
                            <div class="row mb-3">
                                <div class="col-sm-3">Vision:</div>
                                <div class="col-sm-9" id="txtpreviewCompanyVision"></div>
                            </div>
                            <div class="row mb-3">
                                <div class="col-sm-3">Founding Date/Year:</div>
                                <div class="col-sm-9" id="txtpreviewCompanyFounding"></div>
                            </div>
                            <div class="row mb-3">
                                <div class="col-sm-3">Business Industry:</div>
                                <div class="col-sm-9" id="txtpreviewCompanyIndustry"></div>
                            </div>
                            <br />
                            <h3>Organization Affiliation</h3>
                            <ol id="listpreviewOrgAff">
                                <li>List here</li>
                            </ol>
                            <br />
                            <h3>Affiliated Person</h3>
                            <ol id="listpreviewAffPerson">
                                <li>List here</li>
                            </ol>
                            <br />
                            <h3>Contact Info</h3>
                            <ul class="list-padding">
                                <li>
                                    Telephone
                                    <ol class="list-padding" id="listpreviewOrgTel">
                                        <li>List of Telephone here</li>
                                    </ol>
                                </li>
                                <li>
                                    Mobile
                                    <ol class="list-padding" id="listpreviewOrgMobile">
                                        <li>List of Mobile here</li>
                                    </ol>
                                </li>
                                <li>
                                    Email
                                    <ol class="list-padding" id="listpreviewOrgEmail">
                                        <li>List of Email here</li>
                                    </ol>
                                </li>
                                <li>
                                    Website
                                    <ol class="list-padding" id="listpreviewOrgWebsite">
                                        <li>List of Website here</li>
                                    </ol>
                                </li>
                            </ul>
                            <br />
                            <h3>Contact Person</h3>
                            <ol class="list-padding" id="listpreviewOrgContactPerson">
                                <li>List here</li>
                            </ol>
                            <br />
                            <h3>Company Size</h3>
                            <div class="row mb-3">
                                <div class="col-sm-3">No. of Employees:</div>
                                <div class="col-sm-9" id="txtprevNoOfEmp"></div>
                            </div>
                            <div class="row mb-3">
                                <div class="col-sm-3">Revenue per Year:</div>
                                <div class="col-sm-9" id="txtprevRevenue"></div>
                            </div>
                            <div class="row mb-3">
                                <div class="col-sm-3">Scope of Operations:</div>
                                <div class="col-sm-9" id="txtprevScope"></div>
                            </div>
                            <div class="row mb-3">
                                <div class="col-sm-3">No. of Factories:</div>
                                <div class="col-sm-9" id="txtprevNoOfFactories"></div>
                            </div>
                            <div class="row mb-3">
                                <div class="col-sm-3">No. of Offices:</div>
                                <div class="col-sm-9" id="txtprevNoOfOffices"></div>
                            </div>
                            <div class="row mb-3">
                                <div class="col-sm-3">No. of Warehouses:</div>
                                <div class="col-sm-9" id="txtprevNoOfWarehouse"></div>
                            </div>
                            <div class="row mb-3">
                                <div class="col-sm-3">No. of Stores:</div>
                                <div class="col-sm-9" id="txtprevNoOfStores"></div>
                            </div>
                            <div class="row mb-3">
                                <div class="col-sm-3">No. of Trucks:</div>
                                <div class="col-sm-9" id="txtprevNoOfTrucks"></div>
                            </div>
                            @*<br />
            <h3>Revenue</h3>
            <div class="row mb-3">
                <div class="col-sm-3">Sales Representative:</div>
                <div class="col-sm-9" id="txtprevSalesRep"></div>
            </div>
            <div class="row mb-3">
                <div class="col-sm-3">Price Level:</div>
                <div class="col-sm-9" id="txtprevPriceLvl"></div>
            </div>
            <div class="row mb-3">
                <div class="col-sm-3">Credit Terms:</div>
                <div class="col-sm-9" id="txtprevCreditTerms"></div>
            </div>
            <div class="row mb-3">
                <div class="col-sm-3">Credit Limit:</div>
                <div class="col-sm-9" id="txtprevCreditLimit"></div>
            </div>
            <div class="row mb-3">
                <div class="col-sm-3">Credit Beg. Balance:</div>
                <div class="col-sm-9" id="txtprevCreditBeg"></div>
            </div>
            <div class="row mb-3">
                <div class="col-sm-3">Credit Beg. as of:</div>
                <div class="col-sm-9" id="txtprevCreditBegAsOf"></div>
            </div>*@
                        </div>

                        <div id="roPerson" subpage="pagePreview" style="display:none;">
                            <h3>Person Profile</h3>
                            <div class="row mb-3">
                                <div class="col-sm-3">Full Name:</div>
                                <div class="col-sm-9" id="txtpreviewPersonName"></div>
                            </div>
                            <div class="row mb-3">
                                <div class="col-sm-3">Date of Birth:</div>
                                <div class="col-sm-9" id="txtpreviewBirthDate"></div>
                            </div>
                            <div class="row mb-3">
                                <div class="col-sm-3">TIN:</div>
                                <div class="col-sm-9" id="txtpreviewPersonTin"></div>
                            </div>
                            <div class="row mb-3">
                                <div class="col-sm-3">Position:</div>
                                <div class="col-sm-9" id="txtpreviewPosition"></div>
                            </div>
                            <div class="row mb-3">
                                <div class="col-sm-3">Position Description:</div>
                                <div class="col-sm-9" id="txtpreviewPositionDesc"></div>
                            </div>
                            <div class="row mb-3">
                                <div class="col-sm-3">Address:</div>
                                <div class="col-sm-9" id="txtprevAddress"></div>
                            </div>
                            <br />
                            <h3>Organization Affiliation</h3>
                            <ol id="listpreviewPersonOrgAff">
                                <li>List here</li>
                            </ol>
                            <br />
                            <h3>Contact Info</h3>
                            <ul class="list-padding">
                                <li>
                                    Telephone
                                    <ol class="list-padding" id="listpreviewPersonTel">
                                        <li>List of Telephone here</li>
                                    </ol>
                                </li>
                                <li>
                                    Mobile
                                    <ol class="list-padding" id="listpreviewPersonMobile">
                                        <li>List of Mobile here</li>
                                    </ol>
                                </li>
                                <li>
                                    Email
                                    <ol class="list-padding" id="listpreviewPersonEmail">
                                        <li>List of Email here</li>
                                    </ol>
                                </li>
                            </ul>
                            <br />
                            <h3>Educational Background</h3>
                            <ul class="list-padding">
                                <li>
                                    Elementary
                                    <ol class="list-padding" id="listpreviewPersonElem">
                                        <li>List of Telephone here</li>
                                    </ol>
                                </li>
                                <li>
                                    High School
                                    <ol class="list-padding" id="listpreviewPersonHS">
                                        <li>List of Mobile here</li>
                                    </ol>
                                </li>
                                <li>
                                    College
                                    <ol class="list-padding" id="listpreviewPersonCollege">
                                        <li>List of Email here</li>
                                    </ol>
                                </li>
                            </ul>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <div class="row">
                            <div class="col-6">
                                <button data-role="none" type="button" class="btn btn-secondary btn-block" data-dismiss="modal">Cancel</button>
                            </div>
                            <div class="col-6">
                                <button data-role="none" type="button" data-gotopage="pageActions" menu-button="#btnPageProfile" class="btn btn-success btn-block btnPreview">View More Details</button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div id="panAddCustomerLibrary" class="modal fade" tabindex="-1">
            <div class="modal-dialog modal-dialog-centered">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">Please type name of Customer</h5>
                        <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                    </div>
                    <div class="modal-body">
                        <input type="text" id="txtCustomerSearch" autocomplete="off" data-role="none" class="form-control mb-3" />

                        <div id="divCustomerNoResultsFound" style="display:none;">
                            Oops! We can’t find what you’re looking for. Make sure your search is correctly spelled.
                        </div>
                        <div style="max-height:200px; overflow:auto;">
                            <ul id="CustomerlstSearchFound" class="list-group"></ul>
                            <br />
                        </div>

                        <div id="btnAddToCustomerLibrary" data-dismiss="modal" class="divLibAddOption" style="display:none;">
                            <br />
                            If you’ve correctly spelled your search but can’t find it, <br /> you might want to <a class="custom-link">add a new Customer</a>.
                            <br /><br />
                        </div>
                    </div>
                    <div class="modal-footer">
                        <div class="row">
                            <div class="col-6">
                                <button data-role="none" type="button" id="btnCancelCustomerLibrary" class="btn btn-secondary btn-block" data-dismiss="modal">Cancel</button>
                            </div>
                            <div class="col-6">
                                <button data-role="none" type="button" id="btnSearchCustomerLibrary" class="btn btn-success btn-block">Search</button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div id="popCompanyOutletDepartment" class="modal fade" tabindex="-1">
            <div class="modal-dialog modal-dialog-centered">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">Department</h5>
                        <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                    </div>
                    <div class="modal-body">
                        <div class="mb-3">
                            <label class="form-label" for="txtCompanyOutletDepartment">Department Name:</label>
                            <input type="text" id="txtCompanyOutletDepartment" data-role="none" class="form-control" />
                        </div>
                       
                        <div class="mb-3">
                            <label class="form-label" for="txtCompanyOutletDepartment3rdPartyID">3rd Party ID:</label>
                            <input type="text" id="txtCompanyOutletDepartment3rdPartyID" data-role="none" class="form-control" />
                        </div>
                    </div>
                    <div class="modal-footer">
                        <div class="row">
                            <div class="col-6">
                                <button data-role="none" type="button" id="btnCancelCompanyOutletDepartment" class="btn btn-secondary btn-block" data-dismiss="modal">Cancel</button>
                            </div>
                            <div class="col-6">
                                <button data-role="none" type="button" id="btnOkCompanyOutletDepartment" class="btn btn-success btn-block">Ok</button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div id="popCompanyOutletExSku" class="modal fade" tabindex="-1">
            <div class="modal-dialog modal-dialog-centered">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">Copy Existing List</h5>
                        <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                    </div>
                    <div class="modal-body">
                        <div class="row mb-3">
                            <label class="form-label" for="txtCompanyOutletDepartment">Organization Name:</label>
                            <select data-role="none" class="form-select">
                                <option>Please Select</option>
                            </select>
                        </div>

                        <div class="row mb-3">
                            <label class="form-label" for="txtCompanyOutletDepartment3rdPartyID">Outlet:</label>
                            <select data-role="none" class="form-select">
                                <option>Please Select</option>
                            </select>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <div class="row">
                            <div class="col-6">
                                <button data-role="none" type="button" class="btn btn-secondary btn-block" data-dismiss="modal">Cancel</button>
                            </div>
                            <div class="col-6">
                                <button data-role="none" type="button" class="btn btn-success btn-block">Ok</button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div id="popCompanyOutlet" class="modal fade" tabindex="-1">
            <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">Outlet</h5>
                        <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                    </div>
                    <div class="modal-body">
                        <input type="text" id="txtPeopleID" style="display:none;" data-role="none" />
                        <input type="text" id="txtContactPersonID" style="display:none;" data-role="none" />

                        <ul class="nav nav-tabs mb-3" role="tablist">
                            <li class="nav-item" role="presentation">
                                <button data-role="none" class="nav-link active" data-toggle="tab" data-target="#tabOutletInfo">Info</button>
                            </li>
                            <li class="nav-item" role="presentation">
                                <button data-role="none" class="nav-link" data-toggle="tab" data-target="#tabOutletDepartment">Department</button>
                            </li>
                            <li class="nav-item" role="presentation">
                                <button data-role="none" class="nav-link" data-toggle="tab" data-target="#tabOutletAuthSku">Authorized SKU</button>
                            </li>
                        </ul>

                        <div class="tab-content">
                            <div id="tabOutletInfo" class="tab-pane fade  pt-3 show active">
                                <div class="row mb-3">
                                    <label class="col-sm-3 mb-3 col-form-label" for="txtCompanyOutlet">Outlet Name:</label>
                                    <div class="col-sm-9">
                                        <input type="text" data-role="none" class="form-control" id="txtCompanyOutlet" />
                                    </div>
                                </div>

                                <div class="row mb-3">
                                    <label class="col-sm-3 mb-3 col-form-label" for="txtCompanyOutlet3rdPartyID">3rd Party ID:</label>
                                    <div class="col-sm-9">
                                        <input type="text" data-role="none" class="form-control" id="txtCompanyOutlet3rdPartyID" />
                                    </div>
                                </div>

                                <div class="accordion mb-3">
                                    <div class="card">
                                        <div class="card-header p-0">
                                            <button data-target="#" class="btn btn-light btn-block text-left p-3 rounded-0" data-role="none" data-toggle="collapse">
                                                Address
                                            </button>
                                        </div>
                                        <div id="" class="collapse show">
                                            <div class="card-body">
                                                <input data-role="none" type="text" id="txtOutletAddressID" style="display:none;" />
                                                <div class="row mb-3">
                                                    <label class="col-sm-3 mb-3 col-form-label" for="txtOutletHouseBuildingNo">House or Building No:</label>
                                                    <div class="col-sm-9">
                                                        <input type="text" data-role="none" class="form-control" id="txtOutletHouseBuildingNo" />
                                                    </div>
                                                </div>

                                                <div class="row mb-3">
                                                    <label class="col-sm-3 mb-3 col-form-label" for="txtOutletStreet">Street:</label>
                                                    <div class="col-sm-9">
                                                        <input type="text" data-role="none" class="form-control" id="txtOutletStreet" />
                                                    </div>
                                                </div>

                                                <div class="row mb-3">
                                                    <label class="col-sm-3 mb-3 col-form-label" for="selOutletCity">City Or Town:</label>
                                                    <div class="col-sm-9">
                                                        <select id="selOutletCity" data-role="none" class="form-select"></select>
                                                    </div>
                                                </div>

                                                <div class="row mb-3">
                                                    <label class="col-sm-3 mb-3 col-form-label" for="selOutletCountry">Country:</label>
                                                    <div class="col-sm-9">
                                                        <select id="selOutletCountry" data-role="none" class="form-select"></select>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <h3 class="mb-3">Outlet Contact Info</h3>

                                <div>
                                    <div class="row mb-3 align-items-center">
                                        <div class="col-sm-6">
                                            <h5>Telephone Number</h5>
                                        </div>
                                        <div class="col-sm-6 text-right">
                                            <button id="btnAddOutletTelephone" data-role="none" class="btn btn-success">
                                                Add Telephone
                                            </button>
                                        </div>
                                    </div>

                                    <div data-role="none" id="lstOutletTelephone" class="list-group mb-4"></div>
                                </div>

                                <div>
                                    <div class="row mb-3 align-items-center">
                                        <div class="col-sm-6">
                                            <h5>Mobile Number</h5>
                                        </div>
                                        <div class="col-sm-6 text-right">
                                            <button id="btnAddOutletMobile" data-role="none" class="btn btn-success">
                                                Add Mobile
                                            </button>
                                        </div>
                                    </div>

                                    <div data-role="none" id="lstOutletMobile" class="list-group mb-4"></div>
                                </div>

                                <div>
                                    <div class="row mb-3 align-items-center">
                                        <div class="col-sm-6">
                                            <h5>E-mail Address</h5>
                                        </div>
                                        <div class="col-sm-6 text-right">
                                            <button id="btnAddOutletEmail" data-role="none" class="btn btn-success">
                                                Add E-mail
                                            </button>
                                        </div>
                                    </div>

                                    <div data-role="none" id="lstOutletEmail" class="list-group mb-4"></div>
                                </div>
                            </div>

                            <div id="tabOutletDepartment" class="tab-pane fade pt-3">
                                <div class="row mb-3 align-items-center">
                                    <div class="col-sm-6">
                                        <table>
                                            <tr>
                                                <td class="pr-3">Show Status:</td>
                                                <td class="pr-3">
                                                    <select id="selCompanyOutletDeptStatus" data-role="none" class="form-select">
                                                        <option value="">All</option>
                                                        <option value="1" selected>Active</option>
                                                        <option value="0">Inactive</option>
                                                    </select>
                                                </td>
                                            </tr>
                                        </table>
                                    </div>
                                    <div class="col-sm-6 text-right">
                                        <button id="btnAddOutletDepartment" data-role="none" class="btn btn-success">
                                            Add Department
                                        </button>
                                    </div>
                                </div>

                                <div data-role="none" id="lstOutletDepartment" class="list-group mb-3"></div>
                            </div>

                            <div id="tabOutletAuthSku" class="tab-pane fade pt-3">
                                <div class="row mb-3">
                                    <div class="col-sm-2">Options:</div>
                                    <div class="col-sm-10">
                                        <div class="form-check form-check-inline" style="font-size:20px;">
                                            <input class="form-check-input" data-role="none" type="radio" name="radAutoSku" value="all" id="outletAllSkus">
                                            <label class="form-check-label" for="outletAllSkus">All SKUs</label>
                                        </div>

                                        <div class="form-check form-check-inline" style="font-size:20px;">
                                            <input class="form-check-input" data-role="none" type="radio" name="radAutoSku" value="some" id="outletSomeSkus">
                                            <label class="form-check-label" for="outletSomeSkus">Some SKUs</label>
                                        </div>
                                    </div>
                                </div>
                                <div id="divForSomeSkus" style="display:none;">
                                    <div class="mb-3 text-right">
                                        <button data-role="none" id="btnOutletCopyExSku" class="btn btn-secondary">Copy Existing List</button>
                                        <button data-role="none" id="btnOutletAddSku" class="btn btn-success">Add SKU</button>
                                    </div>

                                    <table id="tblOutletAuthSku" data-role="none" class="table table-striped table-hover mb-3">
                                        <thead>
                                            <tr>
                                                <th style="width:20px !important;"> SKU # </th>
                                                <th style="width:20px !important;"> Description </th>
                                                <th style="cursor:pointer;"> Barcode # </th>
                                                <th style="cursor:pointer;"> Last Updated </th>
                                                <th style="cursor:pointer;"> Updated By </th>
                                                <th style="cursor:pointer;">  </th>
                                            </tr>
                                        </thead>
                                        <tbody id="tbodyOutletSkus"></tbody>
                                    </table>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <div class="row">
                            <div class="col-6">
                                <button data-role="none" type="button" class="btn btn-secondary btn-block" data-dismiss="modal">Cancel</button>
                            </div>
                            <div class="col-6">
                                <button data-role="none" type="button" id="btnOkCompanyOutlet" class="btn btn-success btn-block">Ok</button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div id="popOutletTelInfo" class="modal fade" tabindex="-1">
            <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">Telephone Info</h5>
                        <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                    </div>
                    <div class="modal-body">
                        <div style="display:none;">
                            <input type="text" data-role="none" class="form-control" readonly id="txtOutletTelCountryID" value="e783ed13-aa98-4cc1-918e-d49b05c0167e" />
                            <input type="text" data-role="none" class="form-control" readonly id="txtOutletTelID" />
                            <input type="text" data-role="none" class="form-control" readonly id="txtOutletTelAreaCodeID" />
                        </div>

                        <div class="mb-3">
                            <label class="form-label" for="btnOutletTelCountryCodeDisable">
                                Country Code: <span style="color: #FF0000;">*</span>
                                <span id="spanerrorOutletTelCountryCode" style="color:#F00; display:none;">This field is required</span>
                            </label>
                            <button id="btnOutletTelCountryCodeDisable" data-role="none" class="btn btn-secondary btn-block"><span id="txtOutletCountryCode">PHL - 63</span></button>
                        </div>

                        <div class="mb-3">
                            <label class="form-label" for="btnOutletTelAreaCode">
                                Area Code: <span style="color: #FF0000;">*</span>
                                <span id="spanerrorOutletAreaCode" style="color:#F00; display:none;">This field is required</span>
                            </label>
                            <button id="btnOutletTelAreaCode" data-role="none" class="btn btn-secondary btn-block"><span id="txtOutletAreaCode">Please Select</span></button>
                        </div>

                        <div class="mb-3">
                            <label class="form-label" for="txtOutletTelephoneNum">
                                Telephone Number: <span style="color: #FF0000;">*</span>
                                <span id="spanerrorOutletTelephoneNum" style="color:#F00; display:none;">This field is required</span>
                                <span id="spanerrorOutletTelephoneNumValidate" style="color:#F00; display:none;">Enter 7 digit number</span>
                            </label>
                            <input type="number" data-role="none" class="form-control" id="txtOutletTelephoneNum" />
                        </div>

                        <div class="mb-3">
                            <label class="form-label" for="txtOutletTelephoneExt">Extension or Local No.:</label>
                            <input type="text" data-role="none" class="form-control" id="txtOutletTelephoneExt" maxlength="10" />
                        </div>

                        <div class="mb-3">
                            <label class="form-label" for="selOutletTelType">Contact Type: <span style="color: #FF0000;">*</span> <span id="spanerrorOutletTelType" style="color:#F00; display:none;">This field is required</span></label>
                            <select id="selOutletTelType" data-role="none" class="form-select">
                                <option value="">Please Select</option>
                                <option value="Work Telephone">Work Telephone</option>
                                <option value="Personal Telephone">Personal Telephone</option>
                                <option value="Third Party Telephone">Third Party Telephone</option>
                            </select>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <div class="row">
                            <div class="col-6">
                                <button data-role="none" type="button" id="btnCancelOutletTelephone" class="btn btn-secondary btn-block" data-dismiss="modal">Cancel</button>
                            </div>
                            <div class="col-6">
                                <button data-role="none" type="button" id="btnSaveOutletTelephone" class="btn btn-success btn-block">Ok</button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div id="popOutletMobileInfo" class="modal fade" tabindex="-1">
            <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">Mobile Info</h5>
                        <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                    </div>
                    <div class="modal-body">
                        <div style="display:none;">
                            <input type="text" data-role="none" class="form-control" readonly id="txtOutletMobileCountryID" value="e783ed13-aa98-4cc1-918e-d49b05c0167e" />
                            <input type="text" data-role="none" class="form-control" readonly id="txtOutletMobileID" />
                            <input type="text" data-role="none" class="form-control" readonly id="txtOutletTelCoPrefixID" />
                        </div>

                        <div class="mb-3">
                            <label class="form-label" for="btnOutletMobileCountryCodeDisable">
                                Country Code: <span style="color: #FF0000;">*</span>
                                <span id="spanerrorMobileCountryCode" style="color:#F00; display:none;">This field is required</span>
                            </label>
                            <button id="btnOutletMobileCountryCodeDisable" data-role="none" class="btn btn-secondary btn-block"><span id="txtOutletMobileCountryCode">PHL - 63</span></button>
                        </div>

                        <div class="mb-3">
                            <label class="form-label" for="btnOutletMobileTelCode">
                                Tel Code: <span style="color: #FF0000;">*</span>
                                <span id="spanerrorOutletMobileTelCode" style="color:#F00; display:none;">This field is required</span>
                            </label>
                            <button id="btnOutletMobileTelCode" data-role="none" class="btn btn-secondary btn-block"><span id="txtOutletTelCode">Please Select</span></button>
                        </div>

                        <div class="mb-3">
                            <label class="form-label" for="txtOutletMobileNo">
                                Mobile Number: <span style="color: #FF0000;">*</span>
                                <span id="spanerrorOutletMobileNum" style="color:#F00; display:none;">This field is required</span>
                                <span id="spanerrorOutletMobileNumValidate" style="color:#F00; display:none;">Enter 7 digit number</span>
                            </label>
                            <input type="number" id="txtOutletMobileNo" data-role="none" class="form-control" />
                        </div>

                        <div class="mb-3">
                            <label class="form-label" for="selOutletMobileType">Contact Type: <span style="color: #FF0000;">*</span> <span id="spanerrorMobileType" style="color:#F00; display:none;">This field is required</span></label>
                            <select id="selOutletMobileType" data-role="none" class="form-select">
                                <option value="">Please Select</option>
                                <option value="Work Mobile">Work Mobile</option>
                                <option value="Personal Mobile">Personal Mobile</option>
                                <option value="Third Party Mobile">Third Party Mobile</option>
                            </select>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <div class="row">
                            <div class="col-6">
                                <button data-role="none" type="button" id="btnCancelOutletMobile" class="btn btn-secondary btn-block" data-dismiss="modal">Cancel</button>
                            </div>
                            <div class="col-6">
                                <button data-role="none" type="button" id="btnSaveOutletMobile" class="btn btn-success btn-block">Ok</button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>          
        
        <div id="popOutletEmailInfo" class="modal fade" tabindex="-1">
            <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">E-mail Info</h5>
                        <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                    </div>
                    <div class="modal-body">
                        <div style="display:none;">
                            <input type="text" data-role="none" class="form-control" readonly id="txtOutletEmailID" />
                        </div>

                        <div class="mb-3">
                            <label class="form-label" for="txtOutletEmailAdd">
                                Email Address: <span style="color: #FF0000;">*</span>
                                <span id="spanerrorOutletEmailAddress" style="color:#F00; display:none;">This field is required</span>
                                <span id="spanerrorOutletEmailAddressValidate" style="color:#F00; display:none;">Enter valid email address</span>
                            </label>
                            <input type="email" id="txtOutletEmailAdd" data-role="none" class="form-control" />
                        </div>

                        <div class="mb-3">
                            <label class="form-label" for="selOutletEmailType">Contact Type: <span style="color: #FF0000;">*</span> <span id="spanerrorOutletEmailType" style="color:#F00; display:none;">This field is required</span></label>
                            <select id="selOutletEmailType" data-role="none" class="form-control">
                                <option value="">Please Select</option>
                                <option value="Work Email">Work Email</option>
                                <option value="Personal Email">Personal Email</option>
                                <option value="Third Party Email">Third Party Email</option>
                            </select>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <div class="row">
                            <div class="col-6">
                                <button data-role="none" type="button" id="btnCancelOutletEmailAddress" class="btn btn-secondary btn-block" data-dismiss="modal">Cancel</button>
                            </div>
                            <div class="col-6">
                                <button data-role="none" type="button" id="btnSaveOutletEmailAddress" class="btn btn-success btn-block">Ok</button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div id="popConfirmCustomerDeactivation" class="modal fade" tabindex="-1">
            <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">Confirm Dialog</h5>
                        <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                    </div>
                    <div class="modal-body">
                        <div class="row mb-3">
                            <label class="form-label" for="txtCustomerDeactivatePassword">
                                Please confirm by entering you password
                            </label>
                            <input type="password" data-role="none" class="form-control" id="txtCustomerDeactivatePassword" placeholder="Enter your password here" />
                        </div>
                    </div>
                    <div class="modal-footer">
                        <div class="row">
                            <div class="col-6">
                                <button data-role="none" type="button" class="btn btn-secondary btn-block" data-dismiss="modal">Cancel</button>
                            </div>
                            <div class="col-6">
                                <button data-role="none" type="button" id="btnDeactivateCustomerNow" class="btn btn-success btn-block">Confirm Now</button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div id="popCusSupFinancialDetails" class="modal fade" tabindex="-1">
            <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">Financial Details</h5>
                        <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                    </div>
                    <div class="modal-body">
                        <span name="spanRequired" class="mb-3" style="color: #ff0000; display: none;">Fields with (<span style="color: #F00">*</span>) are required.</span>
                        <span name="spanMessage" class="mb-3" style="color: #ff0000; display: none;"></span>

                        <div class="mb-3">
                            <label class="form-label" for="butCusSupBank">
                                Bank / Issuing Organization: <span class="required">*</span>
                            </label>
                            <button data-role="none" name="butCusSupBank" class="btn btn-secondary btn-block">Search Bank</button>
                        </div>

                        <div class="mb-3">
                            <label class="form-label" for="txtCusSupBranch">
                                Branch:
                            </label>
                            <input type="text" name="txtCusSupBranch" data-role="none" class="form-control" />
                        </div>

                        <div class="mb-3">
                            <label class="form-label" for="txtCusSupBranch">
                                Account Name: (Issued To)
                            </label>
                            <input type="text" name="txtCusSupAccountName" data-role="none" class="form-control" />
                        </div>

                        <div class="mb-3">
                            <label class="form-label" for="txtCusSupAccountNumber">
                                Account No.: <span class="required">*</span>
                            </label>
                            <input type="text" name="txtCusSupAccountNumber" data-role="none" class="form-control" />
                        </div>

                        <div class="mb-3">
                            <label class="form-label" for="cbCusSupAccountyType">
                                Account Type: <span class="required">*</span>
                            </label>
                            <select name="cbCusSupAccountyType" data-role="none" class="form-control">
                                <option value="cheque">Checking account</option>
                                <option value="online-payment">Online banking account</option>
                            </select>
                        </div>

                        <div class="mb-3">
                            <label class="form-label" for="chkDeactivated">Deactivated:</label>
                            <div class="col-sm-10">
                                <div class="form-check form-switch mb-3" style="font-size:20px;">
                                    <input class="form-check-input" data-role="none" type="checkbox" name="chkDeactivated">
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <div class="row">
                            <div class="col-6">
                                <button data-role="none" type="button" class="btn btn-secondary btn-block" data-dismiss="modal">Cancel</button>
                            </div>
                            <div class="col-6">
                                <button data-role="none" type="button" name="butCusSupSave" class="btn btn-success btn-block">Ok</button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div id="popCusCreditTerms" class="modal fade" tabindex="-1">
            <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">Credit Terms</h5>
                        <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                    </div>
                    <div class="modal-body">
                        <div class="row mb-3" id="divRevenueFowst">
                            <label class="col-md-3" for="btnRevFowst">FOWST Location: <span class="required">*</span></label>
                            <div class="col-md">
                                <button id="btnRevFowst" data-role="none" class="btn btn-secondary btn-block">Search FOWST</button>
                            </div>
                        </div>

                        <div class="row mb-3">
                            <label class="col-md-3" for="btnRevSalesRepresentative">Sales Representative:</label>
                            <div class="col-md">
                                <button id="btnRevSalesRepresentative" data-role="none" class="btn btn-secondary btn-block">Search Representative</button>
                            </div>
                        </div>

                        <div class="row mb-3">
                            <label class="col-md-3" for="btnRevCreditTerms">Credit Terms: <span class="required">*</span></label>
                            <div class="col-md">
                                <button id="btnRevCreditTerms" data-role="none" class="btn btn-secondary btn-block">Search Payment Terms</button>
                            </div>
                        </div>

                        <div class="row mb-3">
                            <label class="col-md-3" for="txtRevCreditLimit">Credit Limit: <span class="required">*</span></label>
                            <div class="col-md">
                                <input type="number" id="txtRevCreditLimit" data-role="none" class="form-control" />
                            </div>
                        </div>

                        <div class="row mb-3">
                            <label class="col-md-3" for="chkRevUnlimitedCredit">Unlimited:</label>
                            <div class="col-md">
                                <div class="form-check form-switch mb-3" style="font-size:20px;">
                                    <input class="form-check-input" data-role="none" type="checkbox" id="chkRevUnlimitedCredit">
                                </div>
                            </div>
                        </div>

                        <div class="row mb-3">
                            <label class="col-md-3" for="chkRevRequireCnCApproval">Sales orders and invoices from this customer requires credit and collection approval:</label>
                            <div class="col-md">
                                <div class="form-check form-switch mb-3" style="font-size:20px;">
                                    <input class="form-check-input" data-role="none" type="checkbox" id="chkRevRequireCnCApproval">
                                </div>
                            </div>
                        </div>

                        <div class="row mb-3">
                            <label class="col-md-3" for="txtRevCreditBegBal">Credit Beg. Balance:</label>
                            <div class="col-md">
                                <input type="number" id="txtRevCreditBegBal" data-role="none" class="form-control" />
                            </div>
                        </div>

                        <div class="row mb-3">
                            <label class="col-md-3" for="btnRevCreditBegAsOf">Credit Beg. as of:</label>
                            <div class="col-md">
                                <button id="btnRevCreditBegAsOf" data-role="none" class="btn btn-secondary btn-block">Set Date</button>
                            </div>
                        </div>

                        <div class="row mb-3">
                            <label class="col-md-3" for="txtRevThirdPartyBegBalance">Third Party Credit Beg. Balance:</label>
                            <div class="col-md">
                                <input type="number" id="txtRevThirdPartyBegBalance" data-role="none" class="form-control" />
                            </div>
                        </div>

                        <div class="row mb-3">
                            <label class="col-md-3" for="btnRevThirdPartyBegBalanceAsOf">Third Party Credit Beg. as of:</label>
                            <div class="col-md">
                                <button id="btnRevThirdPartyBegBalanceAsOf" data-role="none" class="btn btn-secondary btn-block">Set Date</button>
                            </div>
                        </div>

                        <div class="row mb-3">
                            <label class="col-md-3" for="chkRevCustomerRequirePoNum">Require PO No. &amp; Date In Invoice:</label>
                            <div class="col-md">
                                <div class="form-check form-switch mb-3" style="font-size:20px;">
                                    <input class="form-check-input" data-role="none" type="checkbox" id="chkRevCustomerRequirePoNum">
                                </div>
                            </div>
                        </div>

                        <div>
                            <div class="row mb-3 align-items-center">
                                <div class="col-sm-6">
                                    <h3>Price Level</h3>
                                </div>
                                <div class="col-sm-6 text-right">
                                    <button id="btnRevPriceLevel" data-role="none" class="btn btn-success">
                                        Add Price Level
                                    </button>
                                </div>
                            </div>

                            <div data-role="none" id="lstOrgPriceLevel" class="list-group mb-4"></div>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <div class="row">
                            <div class="col-6">
                                <button data-role="none" type="button" class="btn btn-secondary btn-lg btn-block" data-dismiss="modal">Cancel</button>
                            </div>
                            <div class="col-6">
                                <button data-role="none" type="button" name="butCusCreditTerms" class="btn btn-success btn-lg btn-block">Ok</button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    @*End Pop Ups*@
    <div>

        <div subpage="pageLeadGen" id="pageCustomerMainDashboard">
            <div data-role="content">
                <div class="mb-4">
                    <div style="float:left; display:block;">
                        <table>
                            <tr>
                                <td class="pr-3">Show:</td>
                                <td class="pr-3">
                                    <select id="selStatus" data-role="none" class="form-select form-select-lg">
                                        <option value="All">All</option>
                                        <option value="1" selected>Active</option>
                                        <option value="0">Inactive</option>
                                    </select>
                                </td>
                            </tr>
                        </table>
                    </div>

                    <div class="elementsForErp" style="float:left;">
                        <input type="text" data-role="none" id="txtSearchNewCustomer" placeholder="Search here" class="form-control form-control-lg">
                    </div>
                    <div style="float:right">
                        <button id="btnOptionsNewCustomer" data-role="none" class="btn btn-success btn-lg">
                            Options
                        </button>
                        <button id="btnAddNewCustomer" data-role="none" class="btn btn-success btn-lg">
                            Add Customer
                        </button>
                    </div>
                    <div style="clear:both;"></div>
                </div>

                <form id="Customerformupload" enctype="multipart/form-data" method="post" style="display:none;">
                    <div style="display:none;"><input type="text" id="txtCustomerUploadExcelOrgID" name="OrgID" style="display:none;"></div>
                    <input type="file" id="CustomerobjFile" name="objFile">
                    <input data-role="none" type="text" name="IsCustomer" value="1" style="display:none;" />
                    <input type="submit" id="btnCustomerUploadExcel" value="Upload File">
                </form>

                <form id="formUploadCustomerDataMapping" enctype="multipart/form-data" style="display:none;">
                    <input data-role="none" type="file" id="fileCustomerDataMapping" accept=".xls,.xlsx" name="file">
                    <input data-role="none" type="text" id="txtCustomerDataMapping" name="OrgID">
                </form>

                <div class="custom-ui-label" id="divAdvanceFilter" style="display:none;">
                    <div>Filtered By:</div>
                    <div id="divAdvanceFilterList">
                    </div>
                </div>

                <div id="divCountRow"></div>
                @*<br />*@
                <div style="overflow:auto;">
                    <div subpage="pageMainDashboard" id="divtblLeadGen" style="display:block;">
                        <table id="tblDataTableNewCustomer" data-role="none" class="table table-striped table-hover">
                            <thead>
                                <tr class="align-top text-center" style="font-size:14px;">
                                    <th style="width:20px !important;"> # </th>
                                    <th style="width:20px !important;"> </th>
                                    <th style="cursor:pointer;"> Customer Code </th>
                                    <th style="cursor:pointer;"> Name </th>
                                    <th style="cursor:pointer;"> TIN </th>
                                    <th style="cursor:pointer;"> Business Industry </th>
                                    <th style="cursor:pointer;"> Contact Info </th>
                                    <th style="cursor:pointer;"> Address </th>
                                    <th style="cursor:pointer;"> Sales Reps </th>
                                    @*<th style="cursor:pointer;"> Organization Affiliation </th>*@
                                    <th style="cursor:pointer;"> Date Created/Modified </th>
                                    @*<th style="cursor:pointer;"> Last Interaction Date </th>*@
                                </tr>
                            </thead>
                            <tbody></tbody>
                        </table>
                    </div>
                </div>
            </div>
        </div>

        <div subpage="pageLeadGen" id="pageAddOrgProspect">
            <div data-role="content">

            </div>
        </div>

        <div subpage="pageLeadGen" id="pageAddPersonProspect">
            <div data-role="content">

            </div>
        </div>

        <div subpage="pageLeadGen" id="pageSettings">

        </div>

        <div id="pageActions" subpage="pageLeadGen" style="width:100%;">
            <div data-role="content">
                <div class="ui-grid-solo ui-responsive">
                    <div class="thirtyseventy-block-a left-panel-menu" style="padding:5px; display:none;">
                        <div style="background-color:#F7F7F7;">
                            <div data-toggle="listAction" class="custom-list-menu-toggle">

                            </div>
                            <ul id="listAction" class="custom-list-menu list-menu txt-left">
                                <li><a id="btnPageProfile" data-gotopage="pageProfile" class="active-menu">Profile</a></li>
                                <li><a id="btnPageColdCall" data-gotopage="pageColdCall">Cold Call</a></li>
                                <li><a id="btnPageToDo" data-gotopage="pageToDo">To-Do</a></li>
                                <li><a id="btnPageSMS" data-gotopage="pageSMS">Send SMS</a></li>
                                <li><a id="btnPageInteractionHistory" data-gotopage="pageInteractionHistory">Interaction History</a></li>
                                <li><a id="btnPageTakeANote" data-gotopage="pageTakeANote">Take A Note</a></li>
                                <li><a id="btnPageScheduleAnAction" data-gotopage="pageScheduleAnAction">Schedule An Action</a></li>
                                <li><a id="btnPageInternalSalesMarketing" data-gotopage="pageInternalSalesMarketing">Internal Sales And Marketing Info</a></li>
                            </ul>
                        </div>
                    </div>

                    <div class="ui-block-a left-panel-content" style="padding:5px;">
                        <div subpage="actions" style="display:block" id="pageProfile">
                            <div subpage="pageProfile" id="pageProfileOrg" style="display:none;">
                                <div id="divCompanyProfile" style="display:block">
                                    <div class="row wrapper min-vh-100 flex-column flex-md-row">
                                        <div class="col-12 col-md-2 p-0 bg-light flex-shrink-1">
                                            <nav class="navbar navbar-expand-md navbar-light bg-light align-items-start flex-md-column flex-row p-2">
                                                <a data-role="none" class="navbar-brand" href="#">Menu</a>
                                                <a data-role="none" href="#" class="navbar-toggler" data-toggle="collapse" data-target=".sidebarorg">
                                                    <span class="navbar-toggler-icon"></span>
                                                </a>
                                                <div class="collapse navbar-collapse sidebarorg mt-2">
                                                    <ul class="flex-column w-100 justify-content-between nav nav-pills">
                                                        <li class="nav-item"><a class="nav-link active" data-toggle="tab" href="#tabCompanyProfile">Profile</a></li>
                                                        <li><a class="nav-link" data-role="none" data-toggle="tab" href="#tabCompanyContactInfo">Contact Info</a></li>
                                                        <li><a class="nav-link" data-role="none" data-toggle="tab" href="#tabCompanyContactPerson">Contact Person</a></li>
                                                        <li><a class="nav-link" data-role="none" data-toggle="tab" href="#tabCompanyOutlet">Outlet</a></li>
                                                        <li><a class="nav-link" data-role="none" data-toggle="tab" href="#tabCompanySize">Company Size</a></li>
                                                        <li><a class="nav-link" data-role="none" data-toggle="tab" href="#tabBankInfo">Bank Info</a></li>
                                                        <li><a class="nav-link" data-role="none" data-toggle="tab" href="#tabRevenue">Revenue</a></li>
                                                        <li><a class="nav-link" data-role="none" data-toggle="tab" href="#tabDisbursement">Disbursement</a></li>
                                                        <li><a class="nav-link" data-role="none" data-toggle="tab" href="#tabEcommerce">E-Commerce</a></li>
                                                        <li><a class="nav-link" data-role="none" data-toggle="tab" href="#tabVendorPortal">Vendor Portal</a></li>
                                                        <li><a class="nav-link" data-role="none" data-toggle="tab" href="#tabAttachments">Attachments</a></li>
                                                        <li><a class="nav-link" data-role="none" data-toggle="tab" href="#tabVendorSKUs">SKUs</a></li>
                                                        <li><a class="nav-link" data-role="none" data-toggle="tab" href="#tabSalesReps">Sales Reps</a></li>
                                                        <li><a class="nav-link" id="btnCompanyNotes" data-role="none" data-toggle="tab" href="#tabCompanyNotes">Notes</a></li>
                                                        @*<li><a class="nav-link" data-role="none" data-toggle="tab" href="#tabFowst">FOWST</a></li>*@
                                                    </ul>
                                                </div>
                                            </nav>
                                        </div>
                                        <div class="col bg-faded">
                                            <div class="tab-content">
                                                <div id="tabCompanyProfile" class="tab-pane fade show active">
                                                    <input data-role="none" type="text" id="txtCustomerOrgID" style="display:none;" />
                                                    <input data-role="none" type="text" id="txtProspectOrgID" style="display:none;" />
                                                    <input data-role="none" type="text" id="txtExistingCustomerOrgID" style="display:none;" />

                                                    <div class="mb-3">Fields marked with <span style="color:#F00;">*</span> are required.</div>

                                                    <div class="row mb-3">
                                                        <label class="col-sm-2" for="selOrgStatus">Status:</label>
                                                        <div class="col-sm-10">
                                                            <select id="selOrgStatus" data-role="none" class="form-select">
                                                                <option value="Active">Active</option>
                                                                <option value="Deactive">Inactive</option>
                                                            </select>
                                                        </div>
                                                    </div>

                                                    <div class="row mb-3">
                                                        <label class="col-sm-2" for="txtProspectCompanyName">Organization Name: <span style="color: #FF0000;">*</span></label>
                                                        <div class="col-sm-10">
                                                            <input type="text" data-role="none" class="form-control" id="txtProspectCompanyName" />
                                                        </div>
                                                    </div>

                                                    <div class="row mb-3">
                                                        <label class="col-sm-2" for="txtProspectCompanyBusinessTradeName">Business Trade Name:</label>
                                                        <div class="col-sm-10">
                                                            <input type="text" data-role="none" class="form-control" id="txtProspectCompanyBusinessTradeName" />
                                                        </div>
                                                    </div>

                                                    <div class="row mb-3">
                                                        <label class="col-sm-2" for="txtProspectOrgTin">TIN Number: <span style="color: #FF0000;">*</span></label>
                                                        <div class="col-sm-10">
                                                            <input type="number" data-role="none" class="form-control" id="txtProspectOrgTin" />
                                                        </div>
                                                    </div>

                                                    <div class="row mb-3">
                                                        <label class="col-sm-2 mb-3 col-form-label" for="selVatType">VAT Type:</label>
                                                        <div class="col-sm-10">
                                                            <select id="selVatType" data-role="none" class="form-select">
                                                                <option value="VATable Sales/Receipts">VATable Sales/Receipts</option>
                                                                <option value="Exempt Sales/Receipts">Exempt Sales/Receipts</option>
                                                                <option value="Non VATable">Non VATable</option>
                                                                <option value="Zero-Rated Sales/Receipts">Zero-Rated Sales/Receipts</option>
                                                            </select>
                                                        </div>
                                                    </div>

                                                    <div class="accordion mb-3">
                                                        <div class="card">
                                                            <div class="card-header p-0">
                                                                <button data-target="#" class="btn btn-light btn-block text-left p-3 rounded-0" data-role="none" data-toggle="collapse">
                                                                    Organization Address
                                                                </button>
                                                            </div>
                                                            <div id="" class="collapse show">
                                                                <div class="card-body">
                                                                    <div class="row mb-3">
                                                                        <label class="col-sm-2 mb-3 col-form-label" for="txtProspectHouseBldgNo">House or Building No:</label>
                                                                        <div class="col-sm-10">
                                                                            <input type="text" data-role="none" class="form-control" id="txtProspectHouseBldgNo" />
                                                                        </div>
                                                                    </div>

                                                                    <div class="row mb-3">
                                                                        <label class="col-sm-2 mb-3 col-form-label" for="txtProspectStreetName">Street:</label>
                                                                        <div class="col-sm-10">
                                                                            <input type="text" data-role="none" class="form-control" id="txtProspectStreetName" />
                                                                        </div>
                                                                    </div>

                                                                    <div class="row mb-3">
                                                                        <label class="col-sm-2 mb-3 col-form-label" for="selProspectCities">City Or Town:</label>
                                                                        <div class="col-sm-10">
                                                                            <select id="selProspectCities" data-role="none" class="form-select"></select>
                                                                        </div>
                                                                    </div>

                                                                    <div class="row mb-3">
                                                                        <label class="col-sm-2 mb-3 col-form-label" for="selProspectOrgRegion">Region:</label>
                                                                        <div class="col-sm-10">
                                                                            <select id="selProspectOrgRegion" data-role="none" class="form-select"></select>
                                                                        </div>
                                                                    </div>

                                                                    <div class="row mb-3">
                                                                        <label class="col-sm-2 mb-3 col-form-label" for="selProspectIsland">Island:</label>
                                                                        <div class="col-sm-10">
                                                                            <select id="selProspectIsland" data-role="none" class="form-select">
                                                                                <option value="">Please Select Island</option>
                                                                                <option value="Luzon">Luzon</option>
                                                                                <option value="Visayas">Visayas</option>
                                                                                <option value="Mindanao">Mindanao</option>
                                                                            </select>
                                                                        </div>
                                                                    </div>

                                                                    <div class="row mb-3">
                                                                        <label class="col-sm-2 mb-3 col-form-label" for="selProspectCountries">Country:</label>
                                                                        <div class="col-sm-10">
                                                                            <select id="selProspectCountries" data-role="none" class="form-select"></select>
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>

                                                    <div class="row mb-3">
                                                        <label class="col-sm-2 mb-3 col-form-label" for="txtProspectCompanyBackground">Organization Background:</label>
                                                        <div class="col-sm-10">
                                                            <textarea id="txtProspectCompanyBackground" data-role="none" class="form-control" style="height:100px; resize:none;"></textarea>
                                                        </div>
                                                    </div>

                                                    <div class="row mb-3">
                                                        <label class="col-sm-2 mb-3 col-form-label" for="txtProspectCompanyMission">Organization Mission:</label>
                                                        <div class="col-sm-10">
                                                            <textarea id="txtProspectCompanyMission" data-role="none" class="form-control" style="height:100px; resize:none;"></textarea>
                                                        </div>
                                                    </div>

                                                    <div class="row mb-3">
                                                        <label class="col-sm-2 mb-3 col-form-label" for="txtProspectCompanyVision">Organization Vision:</label>
                                                        <div class="col-sm-10">
                                                            <textarea id="txtProspectCompanyVision" data-role="none" class="form-control" style="height:100px; resize:none;"></textarea>
                                                        </div>
                                                    </div>

                                                    <div class="row mb-3">
                                                        <label class="col-sm-2" for="txtProspectCompanyFounding">Founding Date/Year:</label>
                                                        <div class="col-sm-10">
                                                            <input type="text" data-role="none" class="form-control" id="txtProspectCompanyFounding" />
                                                        </div>
                                                    </div>

                                                    <div class="row mb-3">
                                                        <label class="col-sm-2" for="btnProspectCompanyIndustry">Business Industry:</label>
                                                        <div class="col-sm-10">
                                                            <input type="text" data-role="none" class="form-control" id="txtBusIndustryID" style="display:none;" />
                                                            <button id="btnProspectCompanyIndustry" data-role="none" class="btn btn-secondary btn-block">Search here</button>
                                                        </div>
                                                    </div>

                                                    <div class="row mb-3">
                                                        <label class="col-sm-2 mb-3 col-form-label" for="txtBusinessIndustryText">Business Industry Text:</label>
                                                        <div class="col-sm-10">
                                                            <textarea id="txtBusinessIndustryText" maxlength="1000" data-role="none" class="form-control" style="height:100px; resize:none;"></textarea>
                                                        </div>
                                                    </div>

                                                    <div class="row mb-3">
                                                        <label class="col-sm-2" for="butSalesChannel">Sales Channel:</label>
                                                        <div class="col-sm-10">
                                                            <input type="text" data-role="none" class="form-control" id="txtSalesChannel" style="display:none;" />
                                                            <button id="butSalesChannel" data-role="none" class="btn btn-secondary btn-block">Search here</button>
                                                        </div>
                                                    </div>

                                                    <div class="row mb-3">
                                                        <label class="col-sm-2" for="btnProspectCompanyUserDefinedIndustry">User-Defined Business Industry Classification:</label>
                                                        <div class="col-sm-10">
                                                            <input type="text" data-role="none" class="form-control" id="txtUserDefinedIndustryID" style="display:none;" />
                                                            <button id="btnProspectCompanyUserDefinedIndustry" data-role="none" class="btn btn-secondary btn-block">Search here</button>
                                                        </div>
                                                    </div>

                                                    <div class="row mb-3" id="divCompany3rdPartyPlatform" style="display:none;">
                                                        <label class="col-sm-2" for="txtProspectCompany3rdPartyPlatform">3rd Party Platform:</label>
                                                        <div class="col-sm-10">
                                                            <input type="text" data-role="none" class="form-control" id="txtProspectCompany3rdPartyPlatform" />
                                                        </div>
                                                    </div>

                                                    <div class="row mb-3" id="divCompany3rdPartyID" style="display:none;">
                                                        <label class="col-sm-2" for="txtProspectCompany3rdPartyID">3rd Party ID:</label>
                                                        <div class="col-sm-10">
                                                            <input type="text" data-role="none" class="form-control" id="txtProspectCompany3rdPartyID" />
                                                        </div>
                                                    </div>

                                                    <div class="row mb-3">
                                                        <label class="col-sm-2" for="txtCodeNumber"><span id="lblCodeType"></span> Code:</label>
                                                        <div class="col-sm-10">
                                                            <input type="text" data-role="none" class="form-control txtCodeNumber" readonly placeholder="To be generated by the system" />
                                                        </div>
                                                    </div>

                                                    <div class="row mb-3">
                                                        <label class="col-sm-2 mb-3" for="txtPOSHotButtonTemplate">Hot Button Template:</label>
                                                        <div class="col-sm-10">
                                                            <div class="col-sm-10">
                                                                <select id="selDefaultHBTemplate" class="form-control" data-role="none"></select>
                                                            </div>
                                                        </div>
                                                    </div>

                                                    <div>
                                                        <div class="row mb-3 align-items-center">
                                                            <div class="col-sm-6">
                                                                <h3>Organization Affiliation</h3>
                                                            </div>
                                                            <div class="col-sm-6 text-right">
                                                                <button id="btnAddOrgAffiliation" data-role="none" class="btn btn-success">
                                                                    Add Organization Affiliation
                                                                </button>
                                                            </div>
                                                        </div>

                                                        <div data-role="none" id="lstOrgAffiliation" class="list-group mb-4"></div>
                                                    </div>

                                                    <div>
                                                        <div class="row mb-3 align-items-center">
                                                            <div class="col-sm-6">
                                                                <h3>Affiliated Person</h3>
                                                            </div>
                                                            <div class="col-sm-6 text-right">
                                                                <button id="btnAddAffiliatedPerson" data-role="none" class="btn btn-success">
                                                                    Add Affiliated Persion
                                                                </button>
                                                            </div>
                                                        </div>

                                                        <div data-role="none" id="lstAffiliatedPerson" class="list-group mb-4"></div>
                                                    </div>
                                                </div>

                                                <div id="tabCompanyContactInfo" class="tab-pane fade">
                                                    <div>
                                                        <div class="row mb-3 align-items-center">
                                                            <div class="col-sm-6">
                                                                <h3>Telephone Number</h3>
                                                            </div>
                                                            <div class="col-sm-6 text-right">
                                                                <button id="btnAddOrgTelephone" data-role="none" class="btn btn-success">
                                                                    Add Telephone
                                                                </button>
                                                            </div>
                                                        </div>

                                                        <div data-role="none" id="lstOrgTelephone" class="list-group mb-4"></div>
                                                    </div>

                                                    <div>
                                                        <div class="row mb-3 align-items-center">
                                                            <div class="col-sm-6">
                                                                <h3>Mobile Number</h3>
                                                            </div>
                                                            <div class="col-sm-6 text-right">
                                                                <button id="btnAddOrgMobile" data-role="none" class="btn btn-success">
                                                                    Add Mobile
                                                                </button>
                                                            </div>
                                                        </div>

                                                        <div data-role="none" id="lstOrgMobile" class="list-group mb-4"></div>
                                                    </div>

                                                    <div>
                                                        <div class="row mb-3 align-items-center">
                                                            <div class="col-sm-6">
                                                                <h3>E-mail Address</h3>
                                                            </div>
                                                            <div class="col-sm-6 text-right">
                                                                <button id="btnAddOrgEmail" data-role="none" class="btn btn-success">
                                                                    Add E-mail
                                                                </button>
                                                            </div>
                                                        </div>

                                                        <div data-role="none" id="lstOrgEmail" class="list-group mb-4"></div>
                                                    </div>

                                                    <div>
                                                        <div class="row mb-3 align-items-center">
                                                            <div class="col-sm-6">
                                                                <h3>Website</h3>
                                                            </div>
                                                            <div class="col-sm-6 text-right">
                                                                <button id="btnAddOrgWebsite" data-role="none" class="btn btn-success">
                                                                    Add Website
                                                                </button>
                                                            </div>
                                                        </div>

                                                        <div data-role="none" id="lstOrgWebsite" class="list-group mb-4"></div>
                                                    </div>
                                                </div>

                                                <div id="tabCompanyContactPerson" class="tab-pane fade">
                                                    <div>
                                                        <div class="row mb-3 align-items-center">
                                                            <div class="col-sm-6">
                                                                <h3>Contact Person</h3>
                                                            </div>
                                                            <div class="col-sm-6 text-right">
                                                                <button id="btnAddContactPerson" data-role="none" class="btn btn-success">
                                                                    Add Contact Person
                                                                </button>
                                                            </div>
                                                        </div>

                                                        <div data-role="none" id="lstOrgContactPerson" class="list-group mb-4"></div>
                                                    </div>
                                                </div>

                                                <div id="tabSalesReps" class="tab-pane fade">
                                                    <div>
                                                        <div class="row mb-3 align-items-center">
                                                            <div class="col-sm-6">
                                                                <h3>Sales Representatives</h3>
                                                            </div>
                                                            <div class="col-sm-6 text-right">
                                                                <button id="butAddSalesRep" data-role="none" class="btn btn-success">
                                                                    Add Sales Rep
                                                                </button>
                                                            </div>
                                                        </div>

                                                        <div data-role="none" id="lstOrgSalesRep" class="list-group mb-4"></div>
                                                    </div>
                                                </div>

                                                <div id="tabCompanyOutlet" class="tab-pane fade">
                                                    <div>
                                                        <div class="row mb-3 align-items-center">
                                                            <div class="col-sm-3">
                                                                <table>
                                                                    <tr>
                                                                        <td class="pr-3">Show Status:</td>
                                                                        <td class="pr-3">
                                                                            <select id="selCompanyOutletStatus" data-role="none" class="form-select" style="width:auto;">
                                                                                <option value="">All</option>
                                                                                <option value="1" selected>Active</option>
                                                                                <option value="0">Inactive</option>
                                                                            </select>
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                            </div>
                                                            <div class="col-sm-6">
                                                                <input data-role="none" type="text" id="txtCompanyOutletSearch" class="form-control" placeholder="Search list here" />
                                                            </div>
                                                            <div class="col-sm-3 text-right">
                                                                <button id="btnAddCompanyOutlet" data-role="none" class="btn btn-success">
                                                                    Add Outlet
                                                                </button>
                                                            </div>
                                                        </div>

                                                        <div data-role="none" id="lstCompanyOutlet" class="list-group mb-4"></div>
                                                    </div>
                                                </div>

                                                <div id="tabCompanySize" class="tab-pane fade">
                                                    <div class="row mb-3">
                                                        <label class="col-sm-2" for="txtProspectNoOfEmployees">Number of Employees:</label>
                                                        <div class="col-sm-10">
                                                            <input type="text" data-role="none" class="form-control" id="txtProspectNoOfEmployees" onfocus="this.select();" />
                                                        </div>
                                                    </div>

                                                    <div class="row mb-3">
                                                        <label class="col-sm-2" for="txtProspectRevenuePerYear">Revenue per Year:</label>
                                                        <div class="col-sm-10">
                                                            <input type="text" data-role="none" class="form-control" id="txtProspectRevenuePerYear" />
                                                        </div>
                                                    </div>

                                                    <div class="row mb-3">
                                                        <label class="col-sm-2" for="selProspectScopeOperation">Scope of Operations:</label>
                                                        <div class="col-sm-10">
                                                            <select id="selProspectScopeOperation" data-role="none" class="form-select">
                                                                <option value="">Please Select</option>
                                                                <option value="Regional">Regional</option>
                                                                <option value="National">National</option>
                                                                <option value="Trans-National">Trans-National</option>
                                                            </select>
                                                        </div>
                                                    </div>

                                                    <div class="row mb-3">
                                                        <label class="col-sm-2" for="txtProspectNoFactories">Number of Factories:</label>
                                                        <div class="col-sm-10">
                                                            <input type="number" id="txtProspectNoFactories" data-role="none" class="form-control" />
                                                        </div>
                                                    </div>

                                                    <div class="row mb-3">
                                                        <label class="col-sm-2" for="txtProspectNoOffices">Number of Offices:</label>
                                                        <div class="col-sm-10">
                                                            <input type="number" id="txtProspectNoOffices" data-role="none" class="form-control" />
                                                        </div>
                                                    </div>

                                                    <div class="row mb-3">
                                                        <label class="col-sm-2" for="txtProspectNoWarehouses">Number of Warehouses:</label>
                                                        <div class="col-sm-10">
                                                            <input type="number" id="txtProspectNoWarehouses" data-role="none" class="form-control" />
                                                        </div>
                                                    </div>

                                                    <div class="row mb-3">
                                                        <label class="col-sm-2" for="txtProspectNoStores">Number of Stores:</label>
                                                        <div class="col-sm-10">
                                                            <input type="number" id="txtProspectNoStores" data-role="none" class="form-control" />
                                                        </div>
                                                    </div>

                                                    <div class="row mb-3">
                                                        <label class="col-sm-2" for="txtProspectNoTrucks">Number of Trucks:</label>
                                                        <div class="col-sm-10">
                                                            <input type="number" id="txtProspectNoTrucks" data-role="none" class="form-control" />
                                                        </div>
                                                    </div>
                                                </div>

                                                <div id="tabBankInfo" class="tab-pane fade">
                                                    <div class="text-right mb-3"><button data-role="none" id="btnCusSupAddBankInfo" class="btn btn-success">Add Bank Info</button></div>

                                                    <table data-role="none" class="table table-striped table-hover">
                                                        <thead>
                                                            <tr>
                                                                <th class="txt-center" style="width:50px;">#</th>
                                                                <th class="txt-center">Bank</th>
                                                                <th class="txt-center" style="width:200px;">Bank Branch</th>
                                                                <th class="txt-center" style="width:200px;">Account Number</th>
                                                                <th class="txt-center" style="width:200px;">Account Name</th>
                                                                <th class="txt-center" style="width:200px;">Account Type</th>
                                                                <th class="txt-center" style="width:200px;">Is Active?</th>
                                                            </tr>
                                                        </thead>
                                                        <tbody id="tbodyCusSupBankInfo"></tbody><!-- /tbody -->
                                                    </table>
                                                </div>

                                                <div id="tabRevenue" class="tab-pane fade">
                                                    <div>
                                                        <div class="row mb-3 align-items-center">
                                                            <div class="col-sm-6">
                                                                <h3>Credit Terms and Limit</h3>
                                                            </div>

                                                            <div class="col-sm-6 text-right">
                                                                <button data-role="none" id="btnRevAddCreditTerms" class="btn btn-success">Add Credit Terms and Limit</button>
                                                            </div>
                                                        </div>
                                                        <div class="row mb-3">
                                                            <div class="col-sm-4">
                                                                <select id="selRevenueSingleFowst" data-role="none" class="form-select">
                                                                    <option value="single">Single credit term for all FOWSTs</option>
                                                                    <option value="each">Each FOWST have their own credit term</option>
                                                                </select>
                                                            </div>
                                                        </div>

                                                        <div class="table-responsive mb-3">
                                                            <table data-role="none" class="table table-light table-striped table-hover">
                                                                <thead>
                                                                    <tr class="align-top text-center" style="font-size:14px; table-layout:fixed;">
                                                                        <th>FOWST Location</th>
                                                                        <th>Sales Representative</th>
                                                                        <th>Credit Terms <br /> (in days)</th>
                                                                        <th>Credit Limit</th>
                                                                        <th>Encoded By/Date</th>
                                                                        <th>Approved By/Date</th>
                                                                    </tr>
                                                                </thead>
                                                                <tbody id="tbodyRevCreditTerms"></tbody>
                                                            </table>
                                                        </div>
                                                    </div>
                                                </div>

                                                <div id="tabDisbursement" class="tab-pane fade">
                                                    <div class="row mb-3">
                                                        <label class="col-sm-2" for="butOrgSupplierCreditTerm">Credit Term:</label>
                                                        <div class="col-sm-10">
                                                            <button id="butOrgSupplierCreditTerm" data-role="none" class="btn btn-secondary btn-block">Please Select</button>
                                                        </div>
                                                    </div>

                                                    <div class="row mb-3">
                                                        <label class="col-sm-2" for="txtOrgSupplierCreditLimit">Credit Limit:</label>
                                                        <div class="col-sm-10">
                                                            <input type="number" id="txtOrgSupplierCreditLimit" data-role="none" class="form-control" />
                                                        </div>
                                                    </div>

                                                    <div class="row mb-3">
                                                        <label class="col-sm-2" for="txtOrgSupplierBeginningBalance">Beginning Balance:</label>
                                                        <div class="col-sm-10">
                                                            <input type="number" id="txtOrgSupplierBeginningBalance" data-role="none" class="form-control" />
                                                        </div>
                                                    </div>

                                                    <div class="row mb-3">
                                                        <label class="col-sm-2" for="butOrgSupplierBeginningBalanceAsOf">Beginning Balance As Of:</label>
                                                        <div class="col-sm-10">
                                                            <button id="butOrgSupplierBeginningBalanceAsOf" data-role="none" class="btn btn-secondary btn-block">Set Date</button>
                                                        </div>
                                                    </div>

                                                    <div class="row mb-3">
                                                        <label class="col-sm-2" for="butOrgSupActivationDate">Activation Date:</label>
                                                        <div class="col-sm-10">
                                                            <button id="butOrgSupActivationDate" data-role="none" class="btn btn-secondary btn-block">Set Date</button>
                                                        </div>
                                                    </div>

                                                    <div class="row mb-3">
                                                        <label class="col-sm-2" for="selOrgSupDeactivate">Deactivate record?</label>
                                                        <div class="col-sm-10">
                                                            <div class="form-check form-switch mb-3" style="font-size:20px;">
                                                                <input class="form-check-input" data-role="none" type="checkbox" id="selOrgSupDeactivate">
                                                            </div>
                                                        </div>
                                                    </div>

                                                    <div id="divOrgSupDeactivateDetails" style="display:none;">
                                                        <div class="row mb-3">
                                                            <label class="col-sm-2" for="selOrgSupDateInActiveMonth">Deactivation Date: <span style="color: #FF0000;">*</span></label>
                                                            <div class="col-sm-1">
                                                                <select id="selOrgSupDateInActiveMonth" data-role="none" class="form-select">
                                                                    <option value="">MMM</option>
                                                                </select>
                                                            </div>
                                                            <div class="col-sm-1">
                                                                <select id="selOrgSupDateInActiveDay" data-role="none" class="form-select">
                                                                    <option value="">DD</option>
                                                                </select>
                                                            </div>
                                                            <div class="col-sm-1">
                                                                <select id="selOrgSupDateInActiveYear" data-role="none" class="form-select">
                                                                    <option value="">YYYY</option>
                                                                </select>
                                                            </div>
                                                        </div>

                                                        <div class="row mb-3">
                                                            <label class="col-sm-2" for="txtOrgSupInActiveReason">Reason for deactivating:</label>
                                                            <div class="col-sm-10">
                                                                <textarea id="txtOrgSupInActiveReason" data-role="none" class="form-control" style="height:100px; resize:none;"></textarea>
                                                            </div>
                                                        </div>

                                                        <div class="row mb-3">
                                                            <label class="col-sm-2" for="txtOrgSupComments">Comments:</label>
                                                            <div class="col-sm-10">
                                                                <textarea id="txtOrgSupComments" data-role="none" class="form-control" style="height:100px; resize:none;"></textarea>
                                                            </div>
                                                        </div>

                                                        <div class="row mb-3">
                                                            <label class="col-sm-2" for="selOrgSupBan">Ban this record for future transactions?</label>
                                                            <div class="col-sm-10">
                                                                <div class="form-check form-switch mb-3" style="font-size:20px;">
                                                                    <input class="form-check-input" data-role="none" type="checkbox" id="selOrgSupBan">
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>

                                                <div id="tabEcommerce" class="tab-pane fade">
                                                    <div class="row mb-3 align-items-center">
                                                        <div class="col-sm-6">
                                                            <span id="lblEcommerceIntro"></span>
                                                        </div>
                                                        <div class="col-sm-6 text-right">
                                                            <button id="btnSupplierGenerateUrl" data-role="none" class="btn btn-success">
                                                                Generate Public URL
                                                            </button>
                                                        </div>
                                                    </div>

                                                    <table data-role="none" class="table table-striped table-hover">
                                                        <thead>
                                                            <tr>
                                                                <th class="txt-center" style="width:200px;">Date Generated</th>
                                                                <th class="txt-center">URL</th>
                                                                <th class="txt-center" style="width:200px;">Date Used</th>
                                                                <th class="txt-center" style="width:200px;"></th>
                                                            </tr>
                                                        </thead>
                                                        <tbody id="tbodyGeneratedUrl"></tbody>
                                                    </table>
                                                </div>

                                                <div id="tabAttachments" class="tab-pane fade">
                                                    <div class="mb-3 text-right">
                                                        <button data-role="none" id="btnUploadAttachments" class="btn btn-success">Upload Files</button>
                                                        <form id="formUploadAttachments" method="post">
                                                            <input data-role="none" id="fileUploadAttachments" multiple="multiple" type="file" accept=".xls,.xlsx,.doc,.docx,.pdf,image/*" style="display:none;" />
                                                        </form>
                                                    </div>

                                                    <table id="tableAttachments" data-role="none" class="table table-striped table-hover">
                                                        <thead>
                                                            <tr>
                                                                <th style="width:30px; text-align:center;">#</th>
                                                                <th style="text-align:center;">File Name</th>
                                                                <th style="width:200px; text-align:center;">Date Uploaded</th>
                                                                <th style="width:100px; text-align:center;"></th>
                                                            </tr>
                                                        </thead>
                                                        <tbody id="tbodyAttachments"></tbody>
                                                    </table>
                                                </div>

                                                <div id="tabVendorPortal" class="tab-pane fade">
                                                    <div class="row mb-3 align-items-center">
                                                        <div class="col-sm-6">
                                                            <h3>Canvass Forms Available for This Vendor</h3>
                                                        </div>
                                                        <div class="col-sm-6 text-right">
                                                            <button id="btnAddSupplierCanvassForm" data-role="none" class="btn btn-success">
                                                                Add Canvass Form
                                                            </button>
                                                        </div>
                                                    </div>

                                                    <table id="tableSupplierCanvassForm" data-role="none" class="table table-striped table-hover">
                                                        <thead>
                                                            <tr class="custom-theme-f">
                                                                <th style="width:30px; text-align:center;">#</th>
                                                                <th style="text-align:center;">Canvass Form</th>
                                                                <th style="width:100px; text-align:center;"></th>
                                                            </tr>
                                                        </thead>
                                                        <tbody id="tbodySupplierCanvassForm"></tbody>
                                                    </table>
                                                </div>

                                                <div id="tabVendorSKUs" class="tab-pane fade">
                                                    <textarea id="txtVendorSKUCopyToPlainText" style="margin-left:-1000px; position:absolute;"></textarea>
                                                    <div class="row mb-3 align-items-center">
                                                        <div class="col-md-6">
                                                            <h5>List of SKUs from this Vendor</h5>
                                                        </div>
                                                        <div class="col-md-6 text-right">
                                                            <button data-role="none" id="btnCopyVendorSKUs" class="btn btn-success">Export to Plain Text</button>
                                                            <button data-role="none" id="btnDownloadVendorSKUs" class="btn btn-success">Download SKUs List</button>
                                                            <button data-role="none" id="btnUploadVendorSKUs" class="btn btn-success">Upload SKUs List</button>
                                                        </div>
                                                    </div>
                                                    <form id="formUploadVendorSKUs" style="margin-left:-1000px; position:absolute;">
                                                        <input type="file" data-role="none" name="fileUploadVendorSKUs" id="fileUploadVendorSKUs" accept=".csv, application/vnd.openxmlformats-officedocument.spreadsheetml.sheet, application/vnd.ms-excel" />
                                                    </form>
                                                    <div class="table-responsive">
                                                        <table data-role="none" id="tblVendorSKUs" class="table table-light table-striped table-hover">
                                                            <thead>
                                                                <tr>
                                                                    <th class="text-center">#</th>
                                                                    <th class="text-center">Product No.</th>
                                                                    <th class="text-center">Brand</th>
                                                                    <th class="text-center">Product Description</th>
                                                                    <th class="text-center">Vendor Price</th>
                                                                    <th class="text-center">Last Updated</th>
                                                                </tr>
                                                            </thead>
                                                        </table>
                                                    </div>
                                                </div>

                                                <div id="tabCompanyNotes" class="tab-pane fade">

                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>                                

                                <div id="divProspectMainButton" class="fixed-bottom text-center pb-4">
                                    <button data-role="none" id="btnCancelAddingContact" class="btn btn-secondary btn-lg">
                                        Back
                                    </button>
                                    <button data-role="none" id="btnSaveAddingContact" class="btn btn-success btn-lg">
                                        Save
                                    </button>
                                </div>
                            </div>

                            <div subpage="pageProfile" id="pageProfilePeople" style="display:none;">
                                <div class="row wrapper min-vh-100 flex-column flex-sm-row">
                                    <div class="col-12 col-md-2 p-0 bg-light flex-shrink-1">
                                        <nav class="navbar navbar-expand-sm navbar-light bg-light align-items-start flex-sm-column flex-row p-2">
                                            <a data-role="none" class="navbar-brand" href="#">Menu</a>
                                            <a data-role="none" href="#" class="navbar-toggler" data-toggle="collapse" data-target=".sidebarorg">
                                                <span class="navbar-toggler-icon"></span>
                                            </a>
                                            <div class="collapse navbar-collapse sidebarorg mt-2">
                                                <ul class="flex-column w-100 justify-content-between nav nav-pills">
                                                    <li class="nav-item"><a class="nav-link active" data-toggle="tab" href="#tabPersonProfile">Profile</a></li>
                                                    <li><a class="nav-link" data-role="none" data-toggle="tab" href="#tabPersonContactInfo">Contact Info</a></li>
                                                    <li><a class="nav-link" data-role="none" data-toggle="tab" href="#tabPersonEducation">Educational Background</a></li>
                                                    <li><a class="nav-link" data-role="none" data-toggle="tab" href="#tabPersonBankInfo">Bank Info</a></li>
                                                    <li><a class="nav-link" data-role="none" data-toggle="tab" href="#tabPersonRevenue">Revenue</a></li>
                                                    <li><a class="nav-link" data-role="none" data-toggle="tab" href="#tabPersonEcommerce">E-Commerce</a></li>
                                                    @*<li><a class="nav-link" data-role="none" data-toggle="tab" href="#tabFowst">FOWST</a></li>*@
                                                </ul>
                                            </div>
                                        </nav>
                                    </div>
                                    <div class="col bg-faded">
                                        <div class="tab-content">
                                            <div id="tabPersonProfile" class="tab-pane fade show active">
                                                <input data-role="none" type="text" id="txtCustomerPersonID" style="display:none;" />
                                                <input data-role="none" type="text" id="txtExistingCustomerPersonID" style="display:none;" />
                                                <input data-role="none" type="text" id="txtProspectPersonID" style="display:none;" />

                                                <div class="mb-3">Fields marked with <span style="color:#F00;">*</span> are required.</div>

                                                <div class="row mb-3">
                                                    <label class="col-sm-2 mb-3" for="selPersonStatus">Status:</label>
                                                    <div class="col-sm-10">
                                                        <select id="selPersonStatus" data-role="none" class="form-select">
                                                            <option value="Active">Active</option>
                                                            <option value="Deactive">Inactive</option>
                                                        </select>
                                                    </div>
                                                </div>

                                                <div class="row mb-3" id="divPerson3rdPartyID" style="display:none;">
                                                    <label class="col-sm-2 mb-3" for="txtProspectPerson3rdPartyID">3rd Party ID:</label>
                                                    <div class="col-sm-10">
                                                        <input type="text" id="txtProspectPerson3rdPartyID" data-role="none" class="form-control" />
                                                    </div>
                                                </div>

                                                <div class="row mb-3">
                                                    <label class="col-sm-2 mb-3" for="txtProspectPerson3rdPartyID">Customer Code:</label>
                                                    <div class="col-sm-10">
                                                        <input type="text" class="txtCodeNumber form-control" data-role="none" readonly placeholder="To be generated by the system" />
                                                    </div>
                                                </div>

                                                <div class="row mb-3">
                                                    <label class="col-sm-2 mb-3" for="txtProspectPerson3rdPartyID">Full Name: <span style="color:#F00;">*</span></label>
                                                    <div class="col-sm-3 mb-3">
                                                        <input type="text" id="txtProspectPersonFirstName" placeholder="First Name" data-role="none" class="form-control" />
                                                    </div>
                                                    <div class="col-sm-3 mb-3">
                                                        <input type="text" id="txtProspectPersonMiddleName" placeholder="Middle Name" data-role="none" class="form-control" />
                                                    </div>
                                                    <div class="col-sm-3">
                                                        <input type="text" id="txtProspectPersonLastName" placeholder="Last Name" data-role="none" class="form-control" />
                                                    </div>
                                                </div>

                                                <div class="row mb-3">
                                                    <label class="col-sm-2 mb-3" for="txtProspectPersonTinNumber">TIN:</label>
                                                    <div class="col-sm-10">
                                                        <input type="number" class="form-control" id="txtProspectPersonTinNumber" data-role="none" />
                                                    </div>
                                                </div>

                                                <div class="row mb-3">
                                                    <label class="col-sm-2 mb-3" for="txtProspectPersonTinNumber">Generational Suffix:</label>
                                                    <div class="col-sm-10">
                                                        <select id="selProspectPersonGenSuffix" data-role="none" class="form-select">
                                                            <option value="">None</option>
                                                            <option value="Jr.">Jr.</option>
                                                            <option value="Sr.">Sr.</option>
                                                            <option value="I">I</option>
                                                            <option value="II">II</option>
                                                            <option value="III">III</option>
                                                            <option value="IV">IV</option>
                                                            <option value="V">V</option>
                                                            <option value="VI">VI</option>
                                                            <option value="VII">VII</option>
                                                            <option value="VIII">VIII</option>
                                                            <option value="IX">IX</option>
                                                            <option value="X">X</option>
                                                        </select>
                                                    </div>
                                                </div>

                                                <div class="row mb-3">
                                                    <label class="col-sm-2 mb-3" for="btnPersonBirthDate">Date of Birth:</label>
                                                    <div class="col-sm-10">
                                                        <button id="btnPersonBirthDate" data-role="none" class="btn btn-secondary btn-block">Set Date</button>
                                                    </div>
                                                </div>

                                                <div class="row mb-3">
                                                    <label class="col-sm-2 mb-3" for="txtProspectPersonPosition">Position/Title:</label>
                                                    <div class="col-sm-10">
                                                        <input type="text" id="txtProspectPersonPosition" data-role="none" class="form-control" />
                                                    </div>
                                                </div>

                                                <div class="row mb-3">
                                                    <label class="col-sm-2 mb-3" for="txtProspectPersonPositionDescription">Position/Title Description:</label>
                                                    <div class="col-sm-10">
                                                        <input type="text" id="txtProspectPersonPositionDescription" data-role="none" class="form-control" />
                                                    </div>
                                                </div>

                                                <div class="accordion mb-3">
                                                    <div class="card">
                                                        <div class="card-header p-0">
                                                            <button data-target="#" class="btn btn-light btn-block text-left p-3 rounded-0" data-role="none" data-toggle="collapse">
                                                                Address
                                                            </button>
                                                        </div>
                                                        <div id="" class="collapse show">
                                                            <div class="card-body">
                                                                <input data-role="none" type="text" id="txtPersonAddressID" style="display:none;" />
                                                                <div class="row mb-3">
                                                                    <label class="col-sm-2 mb-3 col-form-label" for="txtPersonHouseBuildingNo">House or Building No:</label>
                                                                    <div class="col-sm-10">
                                                                        <input type="text" data-role="none" class="form-control" id="txtPersonHouseBuildingNo" />
                                                                    </div>
                                                                </div>

                                                                <div class="row mb-3">
                                                                    <label class="col-sm-2 mb-3 col-form-label" for="txtPersonStreet">Street:</label>
                                                                    <div class="col-sm-10">
                                                                        <input type="text" data-role="none" class="form-control" id="txtPersonStreet" />
                                                                    </div>
                                                                </div>

                                                                <div class="row mb-3">
                                                                    <label class="col-sm-2 mb-3 col-form-label" for="selPersonCity">City Or Town:</label>
                                                                    <div class="col-sm-10">
                                                                        <select id="selPersonCity" data-role="none" class="form-select"></select>
                                                                    </div>
                                                                </div>

                                                                <div class="row mb-3">
                                                                    <label class="col-sm-2 mb-3 col-form-label" for="selPersonCountry">Country:</label>
                                                                    <div class="col-sm-10">
                                                                        <select id="selPersonCountry" data-role="none" class="form-select"></select>
                                                                    </div>
                                                                </div>

                                                                <div class="row mb-3">
                                                                    <label class="col-sm-2 mb-3 col-form-label" for="selPersonRegion">Region:</label>
                                                                    <div class="col-sm-10">
                                                                        <select id="selPersonRegion" data-role="none" class="form-select"></select>
                                                                    </div>
                                                                </div>

                                                                <div class="row mb-3">
                                                                    <label class="col-sm-2 mb-3 col-form-label" for="selPersonIsland">Island:</label>
                                                                    <div class="col-sm-10">
                                                                        <select id="selPersonIsland" data-role="none" class="form-select">
                                                                            <option value="">Please Select Island</option>
                                                                            <option value="Luzon">Luzon</option>
                                                                            <option value="Visayas">Visayas</option>
                                                                            <option value="Mindanao">Mindanao</option>
                                                                        </select>
                                                                    </div>
                                                                </div>

                                                                <div class="row mb-3">
                                                                    <label class="col-sm-2 mb-3 col-form-label" for="selPersonTypeAddress">Type of Address:</label>
                                                                    <div class="col-sm-10">
                                                                        <select id="selPersonTypeAddress" data-role="none" class="form-select">
                                                                            <option value="Permanent Address">Permanent Address</option>
                                                                            <option value="Non-Permanent Address">Non-Permanent Address</option>
                                                                            <option value="Provincial Address">Provincial Address</option>
                                                                        </select>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>

                                                <div>
                                                    <div class="row mb-3 align-items-center">
                                                        <div class="col-sm-6">
                                                            <h3>Organization Affiliation</h3>
                                                        </div>
                                                        <div class="col-sm-6 text-right">
                                                            <button id="btnAddPersonOrgAffiliation" data-role="none" class="btn btn-success">
                                                                Add Organization Affiliation
                                                            </button>
                                                        </div>
                                                    </div>

                                                    <div data-role="none" id="lstPersonOrgAffiliation" class="list-group mb-4"></div>
                                                </div>
                                            </div>

                                            <div id="tabPersonContactInfo" class="tab-pane fade">
                                                <div>
                                                    <div class="row mb-3 align-items-center">
                                                        <div class="col-sm-6">
                                                            <h3>Telephone Number</h3>
                                                        </div>
                                                        <div class="col-sm-6 text-right">
                                                            <button id="btnAddPersonTelephone" data-role="none" class="btn btn-success">
                                                                Add Telephone
                                                            </button>
                                                        </div>
                                                    </div>

                                                    <div data-role="none" id="lstPersonTelephone" class="list-group mb-4"></div>
                                                </div>

                                                <div>
                                                    <div class="row mb-3 align-items-center">
                                                        <div class="col-sm-6">
                                                            <h3>Mobile Number</h3>
                                                        </div>
                                                        <div class="col-sm-6 text-right">
                                                            <button id="btnAddPersonMobile" data-role="none" class="btn btn-success">
                                                                Add Mobile
                                                            </button>
                                                        </div>
                                                    </div>

                                                    <div data-role="none" id="lstPersonMobile" class="list-group mb-4"></div>
                                                </div>

                                                <div>
                                                    <div class="row mb-3 align-items-center">
                                                        <div class="col-sm-6">
                                                            <h3>E-mail Address</h3>
                                                        </div>
                                                        <div class="col-sm-6 text-right">
                                                            <button id="btnAddPersonEmail" data-role="none" class="btn btn-success">
                                                                Add E-mail
                                                            </button>
                                                        </div>
                                                    </div>

                                                    <div data-role="none" id="lstPersonEmail" class="list-group mb-4"></div>
                                                </div>
                                            </div>

                                            <div id="tabPersonEducation" class="tab-pane fade">
                                                <div>
                                                    <div class="row mb-3 align-items-center">
                                                        <div class="col-sm-6">
                                                            <h3>Elementary</h3>
                                                        </div>
                                                        <div class="col-sm-6 text-right">
                                                            <button id="btnAddElementary" data-role="none" class="btn btn-success">
                                                                Add Elementary School
                                                            </button>
                                                        </div>
                                                    </div>

                                                    <div data-role="none" id="lstElementary" class="list-group mb-4"></div>
                                                </div>

                                                <div>
                                                    <div class="row mb-3 align-items-center">
                                                        <div class="col-sm-6">
                                                            <h3>High School</h3>
                                                        </div>
                                                        <div class="col-sm-6 text-right">
                                                            <button id="btnAddElementary" data-role="none" class="btn btn-success">
                                                                Add High School
                                                            </button>
                                                        </div>
                                                    </div>

                                                    <div data-role="none" id="lstHs" class="list-group mb-4"></div>
                                                </div>

                                                <div>
                                                    <div class="row mb-3 align-items-center">
                                                        <div class="col-sm-6">
                                                            <h3>College</h3>
                                                        </div>
                                                        <div class="col-sm-6 text-right">
                                                            <button id="btnAddCollege" data-role="none" class="btn btn-success">
                                                                Add College
                                                            </button>
                                                        </div>
                                                    </div>

                                                    <div data-role="none" id="lstCollege" class="list-group mb-4"></div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <div id="divProspectPersonMainButton" class="fixed-bottom text-center pb-4">
                                    <button data-role="none" id="btnCancelPersonAddingContact" class="btn btn-secondary btn-lg">
                                        Back
                                    </button>
                                    <button data-role="none" id="btnSavePersonAddingContact" class="btn btn-success btn-lg">
                                        Save
                                    </button>
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