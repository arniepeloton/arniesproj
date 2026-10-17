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
    require(['promise!datatable.net', 'promise!datatables.net-buttons'], function () {
        require([GetPathFormat('/Scripts/sources/CRMDashboard/CRMDashboard.js?crc=' + assetversion),
            GetPathFormat('/Scripts/sources/CRMDashboard/SmSUsage.js?crc=' + assetversion),
            GetPathFormat('/Scripts/sources/CRMDashboard/db.js?crc=' + assetversion),
            '/Scripts/sources/ERP/OrderProcessing/orderhistory/controller.js?v=' + assetversion
        ]);
    })    
</script>

<style type="text/css">
    .dataTables_processing {
        /*Overwrite the Process Loading in datatables*/
        top: 0 !important;
        line-height: 50vh !important;
        height: 100% !important;
        margin-top: -20px !important;
    }

    [contenteditable] {
        outline: none;
        overflow: auto;
        border: 1px solid #CCC;
        padding: 5px;
        border-radius: 5px;
    }

    .contactchip {
        display: inline-block;
        padding: 0 25px;
        height: 30px;
        font-size: 18px;
        line-height: 30px;
        border-radius: 30px;
        background-color: #F7F7F7;
        margin:2px;
    }

    .contactchip div{
        float: left;
        margin: 0 10px 0 -25px;
        height: 30px;
        width: 30px;
        border-radius: 50%;
        text-align:center;
        background-color: #2196F3;
        color: #FFF;
        text-shadow: none;
        box-shadow: 0px 2px 3px #797979;
        font-size: 14px;
    }

    .contactclosebtn,
    .chip-plain > .closebtn {
        padding-left: 10px;
        color: #888;
        font-weight: bold;
        float: right;
        font-size: 20px;
        cursor: pointer;
    }

    .chip-plain > .closebtn {
        margin-top: -30px;
        margin-right: -15px;
    }

    .contactclosebtn:hover,
    .chip-plain > .closebtn:hover {
        color: #000;
    }

    .chip-plain {
        display: inline-block;
        padding: 0 25px;
        font-size: 18px;
        line-height: 30px;
        background-color: #F7F7F7;
        margin:2px;
        max-width: 300px;
    }

    .chip-plain > div {
        white-space: nowrap;
        text-overflow: ellipsis;
        overflow: hidden;
    }

    .customcalendar {
	    width: 100%;
	    border-collapse: collapse;
    }

    .customcalendar td.hasDate{
	    border: 1px solid #CCC;
    }

    .customcalendar td.CurrentDate{
	    background-color: #FFF;
    }

    .customcalendar td > div > .date,
    .custom-badge
    {
        width: 20px;
        height: 20px;
        border-radius: 50%;
        vertical-align: middle;
        display: table-cell;
        text-align: center;
        padding: 3px;
        color: #000;
        font-weight: normal;   
    }

    .customcalendar td > div > .CurrentDate{
	    background-color: #F00;
	    color: #FFF;
	    text-shadow: none;
        box-shadow: 0px 3px 3px #A2A0A0;
    }

    .customcalendar thead th{
	    padding: 15px;
    }

    .customcalendar tbody td.hasDate:hover{
	    background-color: #e2e2e2;
	    cursor: pointer;
    }

    .calendar_nav{
	    padding: 10px;
        border-radius: 50%;
        display: table-cell;
        text-align: center;
        vertical-align: middle;
        width: 20px;
    }

    .calendar_nav:hover{
	    background-color: #CCC;
    }

    .calendar_cellStyle{
	    height:80px; 
	    padding: 5px;
    }

    .calendar_eventStyle{
	    list-style-type:none; 
	    padding-left:0px;
	    margin: 0px;
    }

    .calendar_eventStyleList{
	    border:1px solid #000; 
	    margin-top:1px;
	    text-overflow: ellipsis;
	    white-space: nowrap;
	    overflow: hidden;
	    font-size: 12px;
	    padding:2px;
	    text-shadow:none;
    }
    .fixed-ActionBtn {
        position: fixed;
        bottom: 25px;
        float: right;
        right: 25px;
        z-index: 1001;
        width: 50px;
        height: 50px;
    }
    .fixed-ActionBtn > button {
        box-shadow: 0px 2px 3px #7c7c7c;
        position: absolute;
        width: 56px;
        height: 56px;
    }
    .fixed-ActionBtn .secondaryActionBtn {
        display: flex;
        position: relative;
        /* margin-left: -220px; */
        /* margin-right: 0px; */
        list-style-type: none;
        /* transform: translate(50%, 0%); */
        float: right;
        margin-right: 60px;
    }
    .fixed-ActionBtn > ul {
        list-style: none;
        padding: 0;
        margin: 0 auto;
        position: absolute;
        bottom: 65px;
    }
    .fixed-ActionBtn > ul > li > button,
    .fixed-ActionBtn .secondaryActionBtn > li > button,
    .btnCircle-sm
    {
        width:40px;
        height:40px;
        cursor:pointer;
        border-radius:50%;
        text-align:center;
        /*background-color: #CCC;*/
        margin: 10px 5px 0px 5px;
        outline: none;
        font-size:16px;
        line-height: 50%;
        /*color: #000;*/
        text-decoration: none;
        border: none;
        box-shadow: 0px 2px 3px #7C7C7C;
        display: none;
        text-overflow: ellipsis;
        white-space:nowrap;
        overflow:hidden;
    }

    .fixed-ActionBtn .secondaryActionBtn > li > button
    {
        display:block;
        margin-top:10px;   
    }
    .fixed-ActionBtn > ul > li > button[data-title]:after {
        content: attr(data-title);
        padding: 4px 8px;
        color: #EFEBEB;
        position: absolute;
        right: 100%;
        white-space: nowrap;
        z-index: 20;
        -moz-border-radius: 5px;
        -webkit-border-radius: 5px;
        border-radius: 5px;
        padding: 10px;
        background-color: rgba(0,0,0,0.9);
    }
</style>

<div id="divLoading" style="display:block;"><div class="text-center"><div class="spinner-border text-primary" role="status"></div></div></div>

<div id="divCRMMainPage" style="display:none">

    @*Start Pop Ups*@
    <div id="divCRMPopups">
        <div id="popCRMContactPersonProfile" class="modal fade" tabindex="-1">
            <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">Person Profile</h5>
                        <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                    </div>
                    <div class="modal-body">
                        <input type="text" data-role="none" class="form-control" id="txtCRMPeopleID" style="display:none;" />
                        <input type="text" data-role="none" class="form-control" id="txtCRMContactPersonID" style="display:none;" />

                        <div class="row mb-3">
                            <label class="col-form-label col-md-2" for="txtCRMContactFirstName">Full Name:</label>
                            <div class="col-md">
                                <input type="text" data-role="none" class="form-control" id="txtCRMContactFirstName" placeholder="First Name" />
                                <span id="spanCRMerrorContactFname" style="color:#F00; display:none;">This field is required</span>
                            </div>
                            <div class="col-md">
                                <input type="text" data-role="none" class="form-control" id="txtCRMContactMiddleName" placeholder="Middle Name" />
                            </div>
                            <div class="col-md">
                                <input type="text" data-role="none" class="form-control" id="txtCRMContactLastName" placeholder="Last Name" />
                            </div>
                        </div>

                        <div class="row mb-3">
                            <label class="col-form-label col-md-2" for="">Generational Suffix:</label>
                            <div class="col-md">
                                <select id="selCRMContactGenSuffix" data-role="none" class="form-select">
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
                            <label class="col-form-label col-md-2" for="txtCRMContactPersonPosition">Position/Title:</label>
                            <div class="col-md">
                                <input type="text" data-role="none" class="form-control" id="txtCRMContactPersonPosition" />
                            </div>
                        </div>

                        <div class="row mb-3">
                            <label class="col-form-label col-md-2" for="">Position/Title Description:</label>
                            <div class="col-md">
                                <input type="text" data-role="none" class="form-control" id="txtCRMContactPersonPositionDescription" />
                            </div>
                        </div>

                        <div class="accordion mb-3">
                            <div class="card">
                                <div class="card-header p-0">
                                    <button data-target="#" class="btn btn-light btn-block text-left p-3 rounded-0" data-role="none" data-toggle="collapse">
                                        <b>Address</b>
                                    </button>
                                </div>
                                <div id="" class="collapse show">
                                    <div class="card-body">
                                        <input data-role="none" type="text" id="txtCRMContactPersonAddressID" style="display:none;" />
                                        <div class="row mb-3">
                                            <label class="col-form-label col-md-2" for="txtCRMContactPersonHouseBuildingNo">House or Building No:</label>
                                            <div class="col-md">
                                                <input type="text" data-role="none" class="form-control" id="txtCRMContactPersonHouseBuildingNo" />
                                            </div>
                                        </div>

                                        <div class="row mb-3">
                                            <label class="col-form-label col-md-2" for="txtCRMContactPersonStreet">Street:</label>
                                            <div class="col-md">
                                                <input type="text" data-role="none" class="form-control" id="txtCRMContactPersonStreet" />
                                            </div>
                                        </div>

                                        <div class="row mb-3">
                                            <label class="col-form-label col-md-2" for="">City Or Town:</label>
                                            <div class="col-md">
                                                <select id="selCRMContactPersonCity" data-role="none" class="form-select"></select>
                                            </div>
                                        </div>

                                        <div class="row mb-3">
                                            <label class="col-form-label col-md-2" for="selCRMContactPersonCountry">Country:</label>
                                            <div class="col-md">
                                                <select id="selCRMContactPersonCountry" data-role="none" class="form-select"></select>
                                            </div>
                                        </div>

                                        <div class="row mb-3">
                                            <label class="col-form-label col-md-2" for="selCRMContactPersonTypeAddress">Type of Address:</label>
                                            <div class="col-md">
                                                <select id="selCRMContactPersonTypeAddress" data-role="none" class="form-select">
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

                        <h4 class="mb-3">Person Contact Info</h4>

                        <div class="accordion mb-3">
                            <div class="card">
                                <div class="card-header p-0">
                                    <button data-target="#" class="btn btn-light btn-block text-left p-3 rounded-0" data-role="none" data-toggle="collapse">
                                        <b>Telephone Number</b>
                                    </button>
                                </div>
                                <div id="" class="collapse show">
                                    <div class="card-body">
                                        <div class="text-right mb-3">
                                            <button id="btnCRMAddContactPersonTelephone" data-role="none" class="btn btn-success">
                                                Add Telephone
                                            </button>
                                        </div>

                                        <ul data-role="none" id="lstContactPersonTelephone" class="list-group"></ul>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="accordion mb-3">
                            <div class="card">
                                <div class="card-header p-0">
                                    <button data-target="#" class="btn btn-light btn-block text-left p-3 rounded-0" data-role="none" data-toggle="collapse">
                                        <b>Mobile Number</b>
                                    </button>
                                </div>
                                <div id="" class="collapse show">
                                    <div class="card-body">
                                        <div class="text-right mb-3">
                                            <button id="btnCRMAddContactPersonMobile" data-role="none" class="btn btn-success">
                                                Add Mobile
                                            </button>
                                        </div>

                                        <ul data-role="none" id="lstContactPersonMobile" class="list-group"></ul>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="accordion mb-3">
                            <div class="card">
                                <div class="card-header p-0">
                                    <button data-target="#" class="btn btn-light btn-block text-left p-3 rounded-0" data-role="none" data-toggle="collapse">
                                        <b>Email Address</b>
                                    </button>
                                </div>
                                <div id="" class="collapse show">
                                    <div class="card-body">
                                        <div class="text-right mb-3">
                                            <button id="btnCRMAddContactPersonEmail" data-role="none" class="btn btn-success">
                                                Add Email Address
                                            </button>
                                        </div>

                                        <ul data-role="none" id="lstContactPersonEmail" class="list-group"></ul>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <div class="row">
                            <div class="col-md order-1 order-md-0">
                                <button data-role="none" type="button" id="btnCRMCancelContact" class="btn btn-secondary btn-block btn-lg">Cancel</button>
                            </div>
                            <div class="col-md order-1 order-md-0">
                                <button data-role="none" type="button" id="btnCRMOkContact" class="btn btn-success btn-block btn-lg">Ok</button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div id="popCRMSMSLogs" class="modal fade" tabindex="-1">
            <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">SMS Logs</h5>
                        <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                    </div>
                    <div class="modal-body">
                        <div class="table-responsive mb-3">
                            <table data-role="none" style="display:table; width:100%" class="table table-light table-striped table-hover">
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
                        </div>

                        <div class="table-responsive">
                            <table id="dataTableSMSLogs" data-role="none" style="display:table; width:100%" class="table table-light table-striped table-hover">
                                <thead>
                                    <tr class="custom-theme-f">
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
                    </div>
                    <div class="modal-footer">
                        <div class="row">
                            <div class="col-md order-1 order-md-0">
                                <button data-role="none" type="button" class="btn btn-secondary btn-block btn-lg" data-dismiss="modal">Close</button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div id="popCRMInteractionHistoryDetails" class="modal fade" tabindex="-1">
            <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">Interaction Details</h5>
                        <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                    </div>
                    <div class="modal-body">
                        <div class="row mb-3">
                            <label class="col-md-2">Client Name:</label>
                            <div class="col-md" id="txtCRMInteractionHistoryDetailsClientName" style="text-transform:capitalize"></div>
                        </div>

                        <div class="row mb-3">
                            <label class="col-md-2">Called By:</label>
                            <div class="col-md" id="txtCRMInteractionHistoryDetailsCalledBy" style="text-transform:capitalize"></div>
                        </div>

                        <div class="row mb-3">
                            <label class="col-md-2">Call Duration:</label>
                            <div class="col-md" id="txtCRMInteractionHistoryDetailsCallDuration" style="text-transform:capitalize"></div>
                        </div>

                        <div class="row mb-3">
                            <label class="col-md-2">Start:</label>
                            <div class="col-md" id="txtCRMInteractionHistoryDetailsStart" style="text-transform:capitalize"></div>
                        </div>

                        <div class="row mb-3">
                            <label class="col-md-2">End:</label>
                            <div class="col-md" id="txtCRMInteractionHistoryDetailsEnd" style="text-transform:capitalize"></div>
                        </div>

                        <h5 class="mb-3">Notes</h5>

                        <div id="txtCRMInteractionHistoryDetailsNotes" style="padding:10px; border:1px solid #CCC; border-radius:5px; background-color:#F0F0F0;"></div>
                    </div>
                    <div class="modal-footer">
                        <div class="row">
                            <div class="col-md order-1 order-md-0">
                                <button data-role="none" type="button" id="btnCRMInteractionHistoryDetailsOk" class="btn btn-secondary btn-block btn-lg">Close</button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div id="divtooltipNotes" class="modal fade" tabindex="-1">
            <div class="modal-dialog modal-dialog-centered">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">Date and Time: <span id="tooltipDateAndTime"></span></h5>
                        <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                    </div>
                    <div class="modal-body">
                        <span>Notes:</span>
                        <br />
                        <div id="tooltipNotes" style="padding: 10px;"></div>
                    </div>
                </div>
            </div>
        </div>

        <div id="popCRMToDo" class="modal fade" tabindex="-1">
            <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">Edit To-Do</h5>
                        <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                    </div>
                    <div class="modal-body">
                        <div class="mb-3">
                            <label class="form-label" for="txtCRMEditToDo">To-Do Name:</label>
                            <input type="text" data-role="none" class="form-control" id="txtCRMEditToDo" placeholder="To-Do Name" />
                        </div>

                        <div class="mb-3">
                            <label class="form-label" for="btnCRMEditToDoSetDate">Due Date:</label>
                            <div class="col-md">
                                <button data-role="none" class="btn btn-secondary btn-block" id="btnCRMEditToDoSetDate">Set Date</button>
                            </div>
                        </div>

                        <div class="mb-3">
                            <label class="form-label" for="selCRMTagInTodoPopup">File Under:</label>
                            <select data-role="none" class="form-control" id="selCRMTagInTodoPopup">
                                <option value="General">General</option>
                                <option value="Issue Ticket">Issue Ticket</option>
                                <option value="Opportunities">Opportunities</option>
                            </select><br />
                            <span id="lblTaggedAsInTodoPopup"></span>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <div class="row">
                            <div class="col-md">
                                <button data-role="none" type="button" id="btnCRMCancelEditToDo" class="btn btn-secondary btn-block btn-lg">Cancel</button>
                            </div>
                            <div class="col-md">
                                <button data-role="none" type="button" id="btnCRMOkEditToDo" class="btn btn-success btn-block btn-lg">Ok</button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div id="divAffiliation" class="modal fade" tabindex="-1">
            <div class="modal-dialog modal-dialog-centered">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">Name: <span id="txtCRMFormName" style="color:#FFF;"></span></h5>
                        <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                    </div>
                    <div class="modal-body">
                        <ol id="listCRMFormAffiliation" style="margin:0 auto;"></ol>
                    </div>
                </div>
            </div>
        </div>

        <div id="popCRMPersonTelInfo" class="modal fade" tabindex="-1">
            <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">Telephone Info</h5>
                        <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                    </div>
                    <div class="modal-body">
                        <div style="display:none;">
                            <input type="text" data-role="none" class="form-control" readonly id="txtCRMPersonTelCountryID" value="e783ed13-aa98-4cc1-918e-d49b05c0167e" />
                            <input type="text" data-role="none" class="form-control" readonly id="txtCRMPersonTelID" />
                            <input type="text" data-role="none" class="form-control" readonly id="txtCRMPersonTelAreaCodeID" />
                        </div>

                        <div class="mb-3">
                            <label class="form-label" for="btnCRMPersonTelCountryCodeDisable">
                                Country Code: <span style="color: #FF0000;">*</span>
                                <span id="spanCRMerrorPersonTelCountryCode" style="color:#F00; display:none;">This field is required</span>
                            </label>
                            <button id="btnCRMPersonTelCountryCodeDisable" data-role="none" class="btn btn-secondary btn-block"><span id="txtCRMPersonCountryCode">PHL - 63</span></button>
                        </div>

                        <div class="mb-3">
                            <label class="form-label" for="btnCRMPersonTelAreaCode">
                                Area Code: <span style="color: #FF0000;">*</span>
                                <span id="spanCRMerrorPersonAreaCode" style="color:#F00; display:none;">This field is required</span>
                            </label>
                            <button id="btnCRMPersonTelAreaCode" data-role="none" class="btn btn-secondary btn-block"><span id="txtCRMPersonAreaCode">Please Select</span></button>
                        </div>

                        <div class="mb-3">
                            <label class="form-label" for="txtCRMPersonTelephoneNum">
                                Telephone Number: <span style="color: #FF0000;">*</span>
                                <span id="spanCRMerrorPersonTelephoneNum" style="color:#F00; display:none;">This field is required</span>
                                <span id="spanCRMerrorPersonTelephoneNumValidate" style="color:#F00; display:none;">Enter 7 digit number</span>
                            </label>
                            <input type="number" data-role="none" class="form-control" id="txtCRMPersonTelephoneNum" />
                        </div>

                        <div class="mb-3">
                            <label class="form-label" for="txtCRMPersonTelephoneExt">Extension or Local No.:</label>
                            <input type="text" data-role="none" class="form-control" id="txtCRMPersonTelephoneExt" maxlength="10" />
                        </div>

                        <div class="mb-3">
                            <label class="form-label" for="selCRMPersonTelType">Contact Type: <span style="color: #FF0000;">*</span> <span id="spanCRMerrorPersonTelType" style="color:#F00; display:none;">This field is required</span></label>
                            <select id="selCRMPersonTelType" data-role="none" class="form-select">
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
                                <button data-role="none" type="button" id="btnCRMCancelPersonTelephone" class="btn btn-secondary btn-block btn-lg" data-dismiss="modal">Cancel</button>
                            </div>
                            <div class="col-6">
                                <button data-role="none" type="button" id="btnCRMSavePersonTelephone" class="btn btn-success btn-block btn-lg">Ok</button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div id="popCRMPersonMobileInfo" class="modal fade" tabindex="-1">
            <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">Mobile Info</h5>
                        <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                    </div>
                    <div class="modal-body">
                        <div style="display:none;">
                            <input type="text" data-role="none" class="form-control" readonly id="txtCRMPersonMobileCountryID" value="e783ed13-aa98-4cc1-918e-d49b05c0167e" />
                            <input type="text" data-role="none" class="form-control" readonly id="txtCRMPersonMobileID" />
                            <input type="text" data-role="none" class="form-control" readonly id="txtCRMPersonTelCoPrefixID" />
                        </div>

                        <div class="mb-3">
                            <label class="form-label" for="btnCRMPersonMobileCountryCodeDisable">
                                Country Code: <span style="color: #FF0000;">*</span>
                                <span id="spanCRMerrorPersonMobileCountryCode" style="color:#F00; display:none;">This field is required</span>
                            </label>
                            <button id="btnCRMPersonMobileCountryCodeDisable" data-role="none" class="btn btn-secondary btn-block"><span id="txtCRMPersonMobileCountryCode">PHL - 63</span></button>
                        </div>

                        @*<div class="mb-3">
                            <label class="form-label" for="btnPersonMobileCountryCodeDisable">
                                Tel Code: <span style="color: #FF0000;">*</span>
                                <span id="spanerrorPersonMobileTelCode" style="color:#F00; display:none;">This field is required</span>
                            </label>
                            <button id="btnPersonMobileTelCode" data-role="none" class="btn btn-secondary btn-block"><span id="txtPersonTelCode">Select</span></button>
                        </div>*@

                        <div class="mb-3">
                            <label class="form-label" for="txtCRMPersonMobileNo">
                                Mobile Number: <span style="color: #FF0000;">*</span>
                                <span id="spanCRMerrorPersonMobileNum" style="color:#F00; display:none;">This field is required</span>
                                <span id="spanCRMerrorPersonMobileNumValidate" style="color:#F00; display:none;">Enter 10 digit number</span>
                            </label>
                            <input type="number" id="txtCRMPersonMobileNo" data-role="none" class="form-control" placeholder="Eg. 91512345678" />
                        </div>

                        <div class="mb-3">
                            <label class="form-label" for="selCRMPersonMobileType">Contact Type: <span style="color: #FF0000;">*</span> <span id="spanCRMerrorPersonMobileType" style="color:#F00; display:none;">This field is required</span></label>
                            <select id="selCRMPersonMobileType" data-role="none" class="form-select">
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
                                <button data-role="none" type="button" id="btnCRMCancelPersonMobile" class="btn btn-secondary btn-block btn-lg" data-dismiss="modal">Cancel</button>
                            </div>
                            <div class="col-6">
                                <button data-role="none" type="button" id="btnCRMSavePersonMobile" class="btn btn-success btn-block btn-lg">Ok</button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div id="popCRMPersonEmailInfo" class="modal fade" tabindex="-1">
            <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">E-mail Info</h5>
                        <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                    </div>
                    <div class="modal-body">
                        <div style="display:none;">
                            <input type="text" data-role="none" class="form-control" readonly id="txtCRMPersonEmailID" />
                        </div>

                        <div class="mb-3">
                            <label class="form-label" for="txtCRMPersonEmailAdd">
                                Email Address: <span style="color: #FF0000;">*</span>
                                <span id="spanCRMerrorPersonEmailAddress" style="color:#F00; display:none;">This field is required</span>
                                <span id="spanCRMerrorPersonEmailAddressValidate" style="color:#F00; display:none;">Enter valid email address</span>
                            </label>
                            <input type="email" id="txtCRMPersonEmailAdd" data-role="none" class="form-control" />
                        </div>

                        <div class="mb-3">
                            <label class="form-label" for="selCRMPersonEmailType">Contact Type: <span style="color: #FF0000;">*</span> <span id="spanCRMerrorPersonEmailType" style="color:#F00; display:none;">This field is required</span></label>
                            <select id="selCRMPersonEmailType" data-role="none" class="form-select">
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
                                <button data-role="none" type="button" id="btnCRMCancelPersonEmailAddress" class="btn btn-secondary btn-block btn-lg" data-dismiss="modal">Cancel</button>
                            </div>
                            <div class="col-6">
                                <button data-role="none" type="button" id="btnCRMSavePersonEmailAddress" class="btn btn-success btn-block btn-lg">Ok</button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div id="popCRMOrgTelInfo" class="modal fade" tabindex="-1">
            <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">Telephone Info</h5>
                        <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                    </div>
                    <div class="modal-body">
                        <div style="display:none;">
                            <input type="text" data-role="none" class="form-control" readonly id="txtCRMOrgTelCountryID" value="e783ed13-aa98-4cc1-918e-d49b05c0167e" />
                            <input type="text" data-role="none" class="form-control" readonly id="txtCRMOrgTelID" />
                            <input type="text" data-role="none" class="form-control" readonly id="txtCRMOrgTelAreaCodeID" />
                        </div>

                        <div class="mb-3">
                            <label class="form-label" for="btnCRMOrgTelCountryCodeDisable">
                                Country Code: <span style="color: #FF0000;">*</span>
                                <span id="spanCRMerrorTelCountryCode" style="color:#F00; display:none;">This field is required</span>
                            </label>
                            <button id="btnCRMOrgTelCountryCodeDisable" data-role="none" class="btn btn-secondary btn-block"><span id="txtCRMCountryCode">PHL - 63</span></button>
                        </div>

                        <div class="mb-3">
                            <label class="form-label" for="btnCRMOrgTelAreaCode">
                                Area Code: <span style="color: #FF0000;">*</span>
                                <span id="spanCRMerrorAreaCode" style="color:#F00; display:none;">This field is required</span>
                            </label>
                            <button id="btnCRMOrgTelAreaCode" data-role="none" class="btn btn-secondary btn-block"><span id="txtCRMAreaCode">Please Select</span></button>
                        </div>

                        <div class="mb-3">
                            <label class="form-label" for="txtCRMTelephoneNum">
                                Telephone Number: <span style="color: #FF0000;">*</span>
                                <span id="spanCRMerrorTelephoneNum" style="color:#F00; display:none;">This field is required</span>
                                <span id="spanCRMerrorTelephoneNumValidate" style="color:#F00; display:none;">Enter 7 digit number</span>
                            </label>
                            <input type="number" data-role="none" class="form-control" id="txtCRMTelephoneNum" />
                        </div>

                        <div class="mb-3">
                            <label class="form-label" for="txtCRMTelephoneExt">Extension or Local No.:</label>
                            <input type="text" data-role="none" class="form-control" id="txtCRMTelephoneExt" maxlength="10" />
                        </div>

                        <div class="mb-3">
                            <label class="form-label" for="selCRMOrgTelType">Contact Type: <span style="color: #FF0000;">*</span> <span id="spanCRMerrorTelType" style="color:#F00; display:none;">This field is required</span></label>
                            <select id="selCRMOrgTelType" data-role="none" class="form-select">
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
                                <button data-role="none" type="button" id="btnCRMCancelOrgTelephone" class="btn btn-secondary btn-block btn-lg" data-dismiss="modal">Cancel</button>
                            </div>
                            <div class="col-6">
                                <button data-role="none" type="button" id="btnCRMSaveOrgTelephone" class="btn btn-success btn-block btn-lg">Ok</button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div id="popCRMOrgMobileInfo" class="modal fade" tabindex="-1">
            <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">Mobile Info</h5>
                        <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                    </div>
                    <div class="modal-body">
                        <div style="display:none;">
                            <input type="text" data-role="none" class="form-control" readonly id="txtCRMOrgMobileCountryID" value="e783ed13-aa98-4cc1-918e-d49b05c0167e" />
                            <input type="text" data-role="none" class="form-control" readonly id="txtCRMOrgMobileID" />
                            <input type="text" data-role="none" class="form-control" readonly id="txtCRMOrgTelCoPrefixID" />
                        </div>

                        <div class="mb-3">
                            <label class="form-label" for="btnCRMOrgMobileCountryCodeDisable">
                                Country Code: <span style="color: #FF0000;">*</span>
                                <span id="spanCRMerrorMobileCountryCode" style="color:#F00; display:none;">This field is required</span>
                            </label>
                            <button id="btnCRMOrgMobileCountryCodeDisable" data-role="none" class="btn btn-secondary btn-block"><span id="txtCRMPersonMobileCountryCode">PHL - 63</span></button>
                        </div>

                        <div class="mb-3">
                            <label class="form-label" for="txtCRMMobileNo">
                                Mobile Number: <span style="color: #FF0000;">*</span>
                                <span id="spanCRMerrorMobileNum" style="color:#F00; display:none;">This field is required</span>
                                <span id="spanCRMerrorMobileNumValidate" style="color:#F00; display:none;">Enter 10 digit number</span>
                            </label>
                            <input type="number" id="txtCRMMobileNo" data-role="none" class="form-control" placeholder="Eg. 91512345678" />
                        </div>

                        <div class="mb-3">
                            <label class="form-label" for="selCRMOrgMobileType">Contact Type: <span style="color: #FF0000;">*</span> <span id="spanCRMerrorMobileType" style="color:#F00; display:none;">This field is required</span></label>
                            <select id="selCRMOrgMobileType" data-role="none" class="form-select">
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
                                <button data-role="none" type="button" id="btnCRMCancelOrgMobile" class="btn btn-secondary btn-block btn-lg" data-dismiss="modal">Cancel</button>
                            </div>
                            <div class="col-6">
                                <button data-role="none" type="button" id="btnCRMSaveOrgMobile" class="btn btn-success btn-block btn-lg">Ok</button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div id="popCRMOrgEmailInfo" class="modal fade" tabindex="-1">
            <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">E-mail Info</h5>
                        <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                    </div>
                    <div class="modal-body">
                        <div style="display:none;">
                            <input type="text" data-role="none" class="form-control" readonly id="txtCRMOrgEmailID" />
                        </div>

                        <div class="mb-3">
                            <label class="form-label" for="txtCRMEmailAdd">
                                Email Address: <span style="color: #FF0000;">*</span>
                                <span id="spanCRMerrorEmailAddress" style="color:#F00; display:none;">This field is required</span>
                                <span id="spanCRMerrorEmailAddressValidate" style="color:#F00; display:none;">Enter valid email address</span>
                            </label>
                            <input type="email" id="txtCRMEmailAdd" data-role="none" class="form-control" />
                        </div>

                        <div class="mb-3">
                            <label class="form-label" for="selCRMOrgEmailType">Contact Type: <span style="color: #FF0000;">*</span> <span id="spanCRMerrorEmailType" style="color:#F00; display:none;">This field is required</span></label>
                            <select id="selCRMOrgEmailType" data-role="none" class="form-select">
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
                                <button data-role="none" type="button" id="btnCRMCancelOrgEmailAddress" class="btn btn-secondary btn-block btn-lg" data-dismiss="modal">Cancel</button>
                            </div>
                            <div class="col-6">
                                <button data-role="none" type="button" id="btnCRMSaveOrgEmailAddress" class="btn btn-success btn-block btn-lg">Ok</button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div id="popCRMOrgWebsiteInfo" class="modal fade" tabindex="-1">
            <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">Website Info</h5>
                        <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                    </div>
                    <div class="modal-body">
                        <div style="display:none;">
                            <input type="text" data-role="none" class="form-control" readonly id="txtCRMOrgWebsiteID" />
                        </div>

                        <div class="mb-3">
                            <label class="form-label" for="txtCRMWebsiteUrl">
                                Website URL: <span style="color: #FF0000;">*</span>
                                <span id="spanCRMerrorWebsiteUrl" style="color:#F00; display:none;">This field is required</span>
                            </label>
                            <input type="text" id="txtCRMWebsiteUrl" data-role="none" class="form-control" />
                        </div>
                    </div>
                    <div class="modal-footer">
                        <div class="row">
                            <div class="col-6">
                                <button data-role="none" type="button" id="btnCRMCancelOrgWebsite" class="btn btn-secondary btn-block" data-dismiss="modal">Cancel</button>
                            </div>
                            <div class="col-6">
                                <button data-role="none" type="button" id="btnCRMSaveOrgWebsite" class="btn btn-success btn-block">Ok</button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div id="popCRMContactPersonTelInfo" class="modal fade" tabindex="-1">
            <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">Telephone Info</h5>
                        <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                    </div>
                    <div class="modal-body">
                        <div style="display:none;">
                            <input type="text" data-role="none" class="form-control" readonly id="txtCRMContactPersonTelCountryID" value="e783ed13-aa98-4cc1-918e-d49b05c0167e" />
                            <input type="text" data-role="none" class="form-control" readonly id="txtCRMContactPersonTelID" />
                            <input type="text" data-role="none" class="form-control" readonly id="txtCRMContactPersonTelAreaCodeID" />
                        </div>

                        <div class="mb-3">
                            <label class="form-label" for="btnCRMContactPersonTelCountryCodeDisable">
                                Country Code: <span style="color: #FF0000;">*</span>
                                <span id="spanCRMerrorContactPersonTelCountryCode" style="color:#F00; display:none;">This field is required</span>
                            </label>
                            <button id="btnCRMContactPersonTelCountryCodeDisable" data-role="none" class="btn btn-secondary btn-block"><span id="txtCRMContactPersonCountryCode">PHL - 63</span></button>
                        </div>

                        <div class="mb-3">
                            <label class="form-label" for="btnCRMContactPersonTelAreaCode">
                                Area Code: <span style="color: #FF0000;">*</span>
                                <span id="spanCRMerrorContactPersonAreaCode" style="color:#F00; display:none;">This field is required</span>
                            </label>
                            <button id="btnCRMContactPersonTelAreaCode" data-role="none" class="btn btn-secondary btn-block"><span id="txtCRMContactPersonAreaCode">Please Select</span></button>
                        </div>

                        <div class="mb-3">
                            <label class="form-label" for="txtCRMContactPersonTelephoneNum">
                                Telephone Number: <span style="color: #FF0000;">*</span>
                                <span id="spanCRMerrorContactPersonTelephoneNum" style="color:#F00; display:none;">This field is required</span>
                                <span id="spanCRMerrorContactPersonTelephoneNumValidate" style="color:#F00; display:none;">Enter 7 digit number</span>
                            </label>
                            <input type="number" data-role="none" class="form-control" id="txtCRMContactPersonTelephoneNum" />
                        </div>

                        <div class="mb-3">
                            <label class="form-label" for="txtCRMContactPersonTelephoneExt">Extension or Local No.:</label>
                            <input type="text" data-role="none" class="form-control" id="txtCRMContactPersonTelephoneExt" maxlength="10" />
                        </div>

                        <div class="mb-3">
                            <label class="form-label" for="selCRMContactPersonTelType">Contact Type: <span style="color: #FF0000;">*</span> <span id="spanCRMerrorContactPersonTelType" style="color:#F00; display:none;">This field is required</span></label>
                            <select id="selCRMContactPersonTelType" data-role="none" class="form-select">
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
                                <button data-role="none" type="button" id="btnCRMCancelContactPersonTelephone" class="btn btn-secondary btn-block btn-lg" data-dismiss="modal">Cancel</button>
                            </div>
                            <div class="col-6">
                                <button data-role="none" type="button" id="btnCRMSaveContactPersonTelephone" class="btn btn-success btn-block btn-lg">Ok</button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div id="popCRMContactPersonMobileInfo" class="modal fade" tabindex="-1">
            <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">Mobile Info</h5>
                        <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                    </div>
                    <div class="modal-body">
                        <div style="display:none;">
                            <input type="text" data-role="none" class="form-control" readonly id="txtCRMContactPersonMobileCountryID" value="e783ed13-aa98-4cc1-918e-d49b05c0167e" />
                            <input type="text" data-role="none" class="form-control" readonly id="txtCRMContactPersonMobileID" />
                            <input type="text" data-role="none" class="form-control" readonly id="txtCRMContactPersonTelCoPrefixID" />
                        </div>

                        <div class="mb-3">
                            <label class="form-label" for="btnCRMContactPersonMobileCountryCodeDisable">
                                Country Code: <span style="color: #FF0000;">*</span>
                                <span id="spanCRMerrorContactPersonMobileCountryCode" style="color:#F00; display:none;">This field is required</span>
                            </label>
                            <button id="btnCRMContactPersonMobileCountryCodeDisable" data-role="none" class="btn btn-secondary btn-block"><span id="txtCRMContactPersonMobileCountryCode">PHL - 63</span></button>
                        </div>

                        <div class="mb-3">
                            <label class="form-label" for="txtCRMContactPersonMobileNo">
                                Mobile Number: <span style="color: #FF0000;">*</span>
                                <span id="spanCRMerrorContactPersonMobileNum" style="color:#F00; display:none;">This field is required</span>
                                <span id="spanCRMerrorContactPersonMobileNumValidate" style="color:#F00; display:none;">Enter 10 digit number</span>
                            </label>
                            <input type="number" id="txtCRMContactPersonMobileNo" data-role="none" class="form-control" placeholder="Eg. 91512345678" />
                        </div>

                        <div class="mb-3">
                            <label class="form-label" for="selCRMContactPersonMobileType">Contact Type: <span style="color: #FF0000;">*</span> <span id="spanCRMerrorContactPersonMobileType" style="color:#F00; display:none;">This field is required</span></label>
                            <select id="selCRMContactPersonMobileType" data-role="none" class="form-select">
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
                                <button data-role="none" type="button" id="btnCRMCancelContactPersonMobile" class="btn btn-secondary btn-block btn-lg" data-dismiss="modal">Cancel</button>
                            </div>
                            <div class="col-6">
                                <button data-role="none" type="button" id="btnCRMSaveContactPersonMobile" class="btn btn-success btn-block btn-lg">Ok</button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div id="popCRMContactPersonEmailInfo" class="modal fade" tabindex="-1">
            <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">E-mail Info</h5>
                        <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                    </div>
                    <div class="modal-body">
                        <div style="display:none;">
                            <input type="text" data-role="none" class="form-control" readonly id="txtCRMContactPersonEmailID" />
                        </div>

                        <div class="mb-3">
                            <label class="form-label" for="txtCRMContactPersonEmailAdd">
                                Email Address: <span style="color: #FF0000;">*</span>
                                <span id="spanCRMerrorContactPersonEmailAddress" style="color:#F00; display:none;">This field is required</span>
                                <span id="spanCRMerrorContactPersonEmailAddressValidate" style="color:#F00; display:none;">Enter valid email address</span>
                            </label>
                            <input type="email" id="txtCRMContactPersonEmailAdd" data-role="none" class="form-control" />
                        </div>

                        <div class="mb-3">
                            <label class="form-label" for="selCRMContactPersonEmailType">Contact Type: <span style="color: #FF0000;">*</span> <span id="spanCRMerrorContactPersonEmailType" style="color:#F00; display:none;">This field is required</span></label>
                            <select id="selCRMContactPersonEmailType" data-role="none" class="form-select">
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
                                <button data-role="none" type="button" id="btnCRMCancelContactPersonEmailAddress" class="btn btn-secondary btn-block btn-lg" data-dismiss="modal">Cancel</button>
                            </div>
                            <div class="col-6">
                                <button data-role="none" type="button" id="btnCRMSaveContactPersonEmailAddress" class="btn btn-success btn-block btn-lg">Ok</button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div id="popCRMSMS" class="modal fade" tabindex="-1">
            <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">SMS</h5>
                        <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                    </div>
                    <div class="modal-body">
                        <div class="row mb-3 align-items-center">
                            <div class="col-md-auto">
                                <label>Recipients: <b id="txtCRMRecipientsCount"></b></label>
                            </div>
                            <div class="col-md">
                                <button id="btnCRMAddNewNumber" data-role="none" class="btn btn-light border" style="float:right;">Add Number</button>
                                <button id="btnCRMSearchContacts" data-role="none" class="btn btn-light border" style="float:right; margin-right:5px;">Search Contacts</button>
                                <button id="btnCRMViewRecipientsDetails" data-role="none" class="btn btn-light border" style="float:right; margin-right:5px;">View Contacts</button>
                            </div>
                        </div>
                        
                        <div id="listCRMofRecipient" class="mb-3" style="max-height: 150px; overflow:auto;"></div>

                        <span id="charNum" class="mb-3"></span>
                        <textarea id="txtCRMSMSMessage" class="form-control" max-length="160"></textarea>
                    </div>
                    <div class="modal-footer">
                        <div class="row">
                            <div class="col-6">
                                <button data-role="none" type="button" id="btnCRMCancelSMS" class="btn btn-secondary btn-block btn-lg" data-dismiss="modal">Cancel</button>
                            </div>
                            <div class="col-6">
                                <button data-role="none" type="button" id="btnCRMSendSMS" class="btn btn-success btn-block btn-lg">Send SMS</button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div id="popCRMSMSRecipient" class="modal fade" tabindex="-1">
            <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">List of Contacts</h5>
                        <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                    </div>
                    <div class="modal-body">
                        <ul data-role="none" id="listCRMSelectedReceipient" class="listContact list-group"></ul>
                    </div>
                    <div class="modal-footer">
                        <div class="row">
                            <div class="col-6">
                                <button data-role="none" type="button" class="btn btn-secondary btn-block btn-lg" data-dismiss="modal">Cancel</button>
                            </div>
                            <div class="col-6">
                                <button data-role="none" type="button" id="btnCRMOkSMSRecipient" class="btn btn-success btn-block btn-lg">Ok</button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div id="popCRMNewNumber" class="modal fade" tabindex="-1">
            <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">New Number</h5>
                        <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                    </div>
                    <div class="modal-body">
                        <div class="mb-3">
                            <label class="form-label" for="">Country Code:</label>
                            <button data-role="none" class="btn btn-secondary btn-block" disabled>PHL - 63</button>
                        </div>

                        <div class="mb-3">
                            <label class="form-label" for="btnCRMNewNumberTelcoPrefix">Telco Prefrix:</label>
                            <button data-role="none" id="btnCRMNewNumberTelcoPrefix" class="btn btn-secondary btn-block">Please Select</button>
                        </div>

                        <div class="mb-3">
                            <label class="form-label" for="txtCRMNewNumber">Enter 7 digit no.:</label>
                            <input type="text" data-role="none" class="form-control" id="txtCRMNewNumber" maxlength="7" />
                        </div>
                    </div>
                    <div class="modal-footer">
                        <div class="row">
                            <div class="col-6">
                                <button data-role="none" type="button" id="btnCRMCancelNewNumber" class="btn btn-secondary btn-block btn-lg">Cancel</button>
                            </div>
                            <div class="col-6">
                                <button data-role="none" type="button" id="btnCRMOkNewNumber" class="btn btn-success btn-block btn-lg">Ok</button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div id="popCRMSearchContacts" class="modal fade" tabindex="-1">
            <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">Search Contacts</h5>
                        <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                    </div>
                    <div class="modal-body">
                        <input data-role="none" class="form-control mb-3" type="search" id="txtCRMSearchContacts" placeholder="Search Name ..." />
                        <div style="overflow:auto;">
                            <div style="float:right; padding:5px;"><a data-role="none" id="lnkSelectAllSearchRecords" class="custom-link"></a></div>
                            <table data-role="none" style="display:table; width:100%" class="table table-light table-striped table-hover">
                                <thead>
                                    <tr class="custom-theme-f">
                                        <th class="bulk" style="cursor:pointer; width:50px;">
                                            <input type="checkbox" name="chbbulk" id="chbCRMbulk" data-role="none" class="form-check-input" />
                                        </th>
                                        <th data-sort="accounting" style="cursor:pointer; width:20px;"> # </th>
                                        <th style="cursor:pointer; width:20px;"></th>
                                        <th data-sort="string" style="cursor:pointer;"> Name </th>
                                    </tr>
                                </thead>
                                <tbody id="tblProspectbody"></tbody>
                            </table>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <div class="row">
                            <div class="col">
                                <button data-role="none" type="button" class="btn btn-success btn-block btn-lg" data-dismiss="modal">Ok</button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div id="popCRMFilter" class="modal fade" tabindex="-1">
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

                        <div class="mb-3">
                            <label class="form-label" for="selCRMCategory">Category:</label>
                            <select id="selCRMCategory" data-role="none" class="form-select">
                                <option value="">Please Select</option>
                                <option value="Business Industry">Business Industry</option>
                                <option value="Company Size">Company Size</option>
                                <option value="City">City</option>
                                <option value="Survey">Survey</option>
                            </select>
                        </div>

                        <div class="mb-3" subpage="pageFilter" id="divBusinessIndustry" style="display:none;">
                            <label class="form-label" for="btnCRMNewNumberTelcoPrefix">Business Industry:</label>
                            <select id="selCRMFilterByBusinessIndustry" data-role="none" class="form-select"></select>
                        </div>

                        <div subpage="pageFilter" id="divCompanySize" style="display:none;">
                            <div class="mb-3">
                                <label class="form-label" for="selCRMFilterCompanySize">Filter By:</label>
                                <select id="selCRMFilterCompanySize" data-role="none" class="form-select">
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

                            <div class="mb-3" subpage="pageCompanySize" id="divScope" style="display:none;">
                                <label class="form-label" for="txtCRMNewNumber">Scope:</label>
                                <select id="selCRMScope" data-role="none" class="form-select">
                                    <option value="">Please Select Scope</option>
                                    <option value="Regional">Regional</option>
                                    <option value="National">National</option>
                                    <option value="Trans-National">Trans-National</option>
                                </select>
                            </div>

                            <div class="mb-3" subpage="pageCompanySize" id="divOthers" style="display:block;">
                                <label class="form-label" for="txtCRMNewNumber">Condition:</label>
                                <select id="selCRMCondition" data-role="none" class="form-select mb-3">
                                    <option value="gt">( &gt; ) Greater Than</option>
                                    <option value="ge">( &ge; ) Greater Than And Equal To</option>
                                    <option value="e">( &#61; ) Equal To</option>
                                    <option value="lt">( &lt; ) Lesser Than</option>
                                    <option value="le">( &le; ) Lesser Than And Equal To</option>
                                </select>
                                <input type="text" data-role="none" class="form-control" id="txtCRMConditionValue" placeholder="Enter Value" />
                            </div>
                        </div>

                        <div class="mb-3" subpage="pageFilter" id="divCity" style="display:none;">
                            <label class="form-label" for="btnCRMNewNumberTelcoPrefix">City:</label>
                            <select id="selCRMFilterByCity" data-role="none" class="form-select"></select>
                        </div>

                        <div class="mb-3" subpage="pageFilter" id="divFilterSurvey" style="display:none;">
                            <label class="form-label" for="btnCRMNewNumberTelcoPrefix">Survey:</label>
                            <select id="selCRMFilterSurvey" data-role="none" class="form-select"></select>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <div class="row">
                            <div class="col-6">
                                <button data-role="none" type="button" id="btnCRMCancelFilter" class="btn btn-secondary btn-block btn-lg">Cancel</button>
                            </div>
                            <div class="col-6">
                                <button data-role="none" type="button" id="btnCRMOkFilter" class="btn btn-success btn-block btn-lg">Ok</button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div id="popCRMEducation" class="modal fade" tabindex="-1">
            <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title" id="txtCRMAcademicLevel"></h5>
                        <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                    </div>
                    <div class="modal-body">
                        <input type="text" data-role="none" class="form-control" style="display:none;" id="txtCRMEdeHiID" />
                        <div class="mb-3">
                            <label class="form-label" for="txtCRMEdeHiSchoolName">School Name:</label>
                            <input type="text" data-role="none" class="form-control" id="txtCRMEdeHiSchoolName" />
                        </div>

                        <div class="mb-3">
                            <label class="form-label" for="txtCRMEdeHiYearStarted">Year Started:</label>
                            <input type="number" data-role="none" class="form-control" id="" />
                        </div>
                        
                        <div class="mb-3">
                            <label class="form-label" for="txtCRMEdeHiYearGraduated">Year Graduated:</label>
                            <input type="number" data-role="none" class="form-control" id="txtCRMEdeHiYearGraduated" />
                        </div>
                    </div>
                    <div class="modal-footer">
                        <div class="row">
                            <div class="col-6">
                                <button data-role="none" type="button" class="btn btn-secondary btn-block btn-lg" data-dismiss="modal">Cancel</button>
                            </div>
                            <div class="col-6">
                                <button data-role="none" type="button" id="btnCRMOkEduHi" class="btn btn-success btn-block btn-lg">Ok</button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div id="prevCRMOrgProfile" class="profile-preview" style="display:none;">
            <div class="grid grid-a">
                <div id="btnCRMPreviewClose" style="background: rgba(0,0,0,1); padding:15px; position: absolute; top: 0; width: 100%; right: 0; left: 0; color: #FFF; margin-bottom: 10px; cursor:pointer;">
                    Close
                </div>
                <br />
                <div style="background-color:#FFF; padding:15px;">
                    <div id="roCRMOrg" subpage="pageCRMPreview" style="display:block;">
                        <h3>Company Profile</h3>
                        <div class="custom-ui-label">
                            <div>Name:</div>
                            <div id="txtCRMpreviewCompanyName"></div>
                        </div>
                        <div class="custom-ui-label">
                            <div>Address:</div>
                            <div id="txtCRMpreviewCompanyAddress"></div>
                        </div>
                        <div class="custom-ui-label">
                            <div>Background:</div>
                            <div id="txtCRMpreviewCompanyBackground"></div>
                        </div>
                        <div class="custom-ui-label">
                            <div>Mission:</div>
                            <div id="txtCRMpreviewCompanyMission"></div>
                        </div>
                        <div class="custom-ui-label">
                            <div>Vision:</div>
                            <div id="txtCRMpreviewCompanyVision"></div>
                        </div>
                        <div class="custom-ui-label">
                            <div>Founding Date/Year:</div>
                            <div id="txtCRMpreviewCompanyFounding"></div>
                        </div>
                        <div class="custom-ui-label">
                            <div>Business Industry:</div>
                            <div id="txtCRMpreviewCompanyIndustry"></div>
                        </div>
                        <br />
                        <h3>Organization Affiliation</h3>
                        <ol id="listCRMpreviewOrgAff">
                            <li>List here</li>
                        </ol>
                        <br />
                        <h3>Affiliated Person</h3>
                        <ol id="listCRMpreviewAffPerson">
                            <li>List here</li>
                        </ol>
                        <br />
                        <h3>Contact Info</h3>
                        <ul class="list-padding">
                            <li>
                                Telephone
                                <ol class="list-padding" id="listCRMpreviewOrgTel">
                                    <li>List of Telephone here</li>
                                </ol>
                            </li>
                            <li>
                                Mobile
                                <ol class="list-padding" id="listCRMpreviewOrgMobile">
                                    <li>List of Mobile here</li>
                                </ol>
                            </li>
                            <li>
                                Email
                                <ol class="list-padding" id="listCRMpreviewOrgEmail">
                                    <li>List of Email here</li>
                                </ol>
                            </li>
                            <li>
                                Website
                                <ol class="list-padding" id="listCRMpreviewOrgWebsite">
                                    <li>List of Website here</li>
                                </ol>
                            </li>
                        </ul>
                        <br />
                        <h3>Contact Person</h3>
                        <ol class="list-padding" id="listCRMpreviewOrgContactPerson">
                            <li>List here</li>
                        </ol>
                        <br />
                        <h3>Company Size</h3>
                        <div class="custom-ui-label">
                            <div>No. of Employees:</div>
                            <div id="txtCRMprevNoOfEmp"></div>
                        </div>
                        <div class="custom-ui-label">
                            <div>Revenue per Year:</div>
                            <div id="txtCRMprevRevenue"></div>
                        </div>
                        <div class="custom-ui-label">
                            <div>Scope of Operations:</div>
                            <div id="txtCRMprevScope"></div>
                        </div>
                        <div class="custom-ui-label">
                            <div>No. of Factories:</div>
                            <div id="txtCRMprevNoOfFactories"></div>
                        </div>
                        <div class="custom-ui-label">
                            <div>No. of Offices:</div>
                            <div id="txtCRMprevNoOfOffices"></div>
                        </div>
                        <div class="custom-ui-label">
                            <div>No. of Warehouses:</div>
                            <div id="txtCRMprevNoOfWarehouse"></div>
                        </div>
                        <div class="custom-ui-label">
                            <div>No. of Stores:</div>
                            <div id="txtCRMprevNoOfStores"></div>
                        </div>
                        <div class="custom-ui-label">
                            <div>No. of Trucks:</div>
                            <div id="txtCRMprevNoOfTrucks"></div>
                        </div>
                        @*<br />
                            <h3>Revenue</h3>
                            <div class="custom-ui-label">
                                <div>Sales Representative:</div>
                                <div id="txtCRMprevSalesRep"></div>
                            </div>
                            <div class="custom-ui-label">
                                <div>Price Level:</div>
                                <div id="txtCRMprevPriceLvl"></div>
                            </div>
                            <div class="custom-ui-label">
                                <div>Credit Terms:</div>
                                <div id="txtCRMprevCreditTerms"></div>
                            </div>
                            <div class="custom-ui-label">
                                <div>Credit Limit:</div>
                                <div id="txtCRMprevCreditLimit"></div>
                            </div>
                            <div class="custom-ui-label">
                                <div>Credit Beg. Balance:</div>
                                <div id="txtCRMprevCreditBeg"></div>
                            </div>
                            <div class="custom-ui-label">
                                <div>Credit Beg. as of:</div>
                                <div id="txtCRMprevCreditBegAsOf"></div>
                            </div>*@
                    </div>

                    <div id="roCRMPerson" subpage="pageCRMPreview" style="display:none;">
                        <h3>Person Profile</h3>
                        <div class="custom-ui-label">
                            <div>Full Name:</div>
                            <div id="txtCRMpreviewPersonName"></div>
                        </div>
                        <div class="custom-ui-label">
                            <div>Date of Birth:</div>
                            <div id="txtCRMpreviewBirthDate"></div>
                        </div>
                        <div class="custom-ui-label">
                            <div>Position:</div>
                            <div id="txtCRMpreviewPosition"></div>
                        </div>
                        <div class="custom-ui-label">
                            <div>Position Description:</div>
                            <div id="txtCRMpreviewPositionDesc"></div>
                        </div>
                        <div class="custom-ui-label">
                            <div>Address:</div>
                            <div id="txtCRMprevAddress"></div>
                        </div>
                        <br />
                        <h3>Organization Affiliation</h3>
                        <ol id="listCRMpreviewPersonOrgAff">
                            <li>List here</li>
                        </ol>
                        <br />
                        <h3>Contact Info</h3>
                        <ul class="list-padding">
                            <li>
                                Telephone
                                <ol class="list-padding" id="listCRMpreviewPersonTel">
                                    <li>List of Telephone here</li>
                                </ol>
                            </li>
                            <li>
                                Mobile
                                <ol class="list-padding" id="listCRMpreviewPersonMobile">
                                    <li>List of Mobile here</li>
                                </ol>
                            </li>
                            <li>
                                Email
                                <ol class="list-padding" id="listCRMpreviewPersonEmail">
                                    <li>List of Email here</li>
                                </ol>
                            </li>
                        </ul>
                        <br />
                        <h3>Educational Background</h3>
                        <ul class="list-padding">
                            <li>
                                Elementary
                                <ol class="list-padding" id="listCRMpreviewPersonElem">
                                    <li>List of Telephone here</li>
                                </ol>
                            </li>
                            <li>
                                High School
                                <ol class="list-padding" id="listCRMpreviewPersonHS">
                                    <li>List of Mobile here</li>
                                </ol>
                            </li>
                            <li>
                                College
                                <ol class="list-padding" id="listCRMpreviewPersonCollege">
                                    <li>List of Email here</li>
                                </ol>
                            </li>
                        </ul>
                    </div>
                </div>
            </div>
            <div class="grid grid-b">
                <div class="panel-menu">
                    <ul id="listCRMPreviewMenu" class="custom-list-menu txt-left" panel-menu="true">
                        <li><a id="" data-gotopage="pageCRMActions" menu-button="#btnCRMPageProfile" class="btnPreview active-menu ui-link">Profile</a></li>
                        <li><a id="" data-gotopage="pageCRMActions" menu-button="#btnCRMPageOrderHistory" class="btnPreview ui-link">Order History</a></li>
                        <li><a id="" data-gotopage="pageCRMActions" menu-button="#btnCRMPageInteractionHistory" class="btnPreview ui-link">Interaction History</a></li>
                        <li><a id="" data-gotopage="pageCRMActions" menu-button="#btnCRMPageOpportunity" class="btnPreview ui-link">Opportunities</a></li>
                        <li class="btnSupporTicket"><a id="" data-gotopage="pageCRMActions" menu-button="#btnCRMPageTickets" class="btnPreview ui-link">Issues</a></li>
                        <li><a id="" data-gotopage="pageCRMActions" menu-button="#btnCRMPageScheduleAnAction" class="btnPreview ui-link">Scheduled Actions</a></li>
                        <li><a id="" data-gotopage="pageCRMActions" menu-button="#btnCRMPageToDo" class="btnPreview ui-link">To Dos</a></li>
                        <li><a id="" menu-button="#btnCRMPageColdCall" class="btnPreview ui-link">Calls</a></li>
                        <li><a id="" data-gotopage="pageCRMActions" menu-button="#btnCRMPageTakeANote" class="btnPreview ui-link">Notes</a></li>
                        <li><a id="" data-gotopage="pageCRMActions" menu-button="#btnCRMPageSMS" class="btnPreview ui-link">SMS</a></li>
                        <li><a id="" data-gotopage="pageCRMActions" menu-button="#btnCRMPageSurvey" class="btnPreview ui-link">Surveys</a></li>
                        <li><a id="" data-gotopage="pageCRMActions" menu-button="#btnCRMPageFile" class="btnPreview ui-link">Files</a></li>
                        <li><a id="" data-gotopage="pageCRMActions" menu-button="#btnForDTRViolations" class="btnPreview ui-link">Violations and Sanctions</a></li>
                        @*<li><a id="" class="btnTobeDeployed disabled-menu ui-link">Awards and Citations</a></li>
                        <li><a id="" class="btnTobeDeployed disabled-menu ui-link">Cash Advances and Charges</a></li>
                        <li><a id="" class="btnTobeDeployed disabled-menu ui-link">Loans from Government agencies</a></li>
                        <li><a id="" class="btnTobeDeployed disabled-menu ui-link">Issuances of company equipment and paraphernalia</a></li>*@
                    </ul>
                </div>
            </div>
            <div style="clear:both;"></div>
        </div>

        <div id="popCRMTakeSurvey" class="modal fade" tabindex="-1">
            <div class="modal-dialog modal-fullscreen">
                <div class="modal-content">
                    <div class="modal-header">
                        <div class="input-group">
                            <input data-role="none" type="text" id="txtCRMCustomerUrl" class="form-control" onclick="this.select()" readonly="" placeholder="Link here">
                            <button data-role="none" id="btnCRMCustomerUrl" class="btn btn-success" type="button">Copy Link</button>
                        </div>
                    </div>
                    <div class="modal-body">
                        <iframe id="iframeSurvey" style="width:100%; border:none; height:100%;"></iframe>
                    </div>
                    <div class="modal-footer">
                        <button data-role="none" type="button" class="btn btn-secondary btn-lg" data-dismiss="modal">Close</button>
                    </div>
                </div>
            </div>
        </div>

        <div id="popCRMROClientSurvey" class="modal fade" tabindex="-1">
            <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">View Response</h5>
                        <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                    </div>
                    <div class="modal-body">
                        <div class="row mb-3 align-items-center">
                            <div class="col-md-auto">Questionaire Title:</div>
                            <div class="col-md-auto font-weight-bold" id="lblROQTitle"></div>
                            <div class="col-md text-right">
                                <button data-role="none" id="btnCRMRODLReq" class="btn btn-success">Download Req(s)</button>
                            </div>
                        </div>

                        <ul id="listCRMClientQuestionsReadOnly" class="list-group" style="list-style-type:decimal;"></ul>
                    </div>
                    <div class="modal-footer">
                        <button data-role="none" type="button" class="btn btn-secondary btn-lg" data-dismiss="modal">Close</button>
                    </div>
                </div>
            </div>
        </div>

        <div id="popCRMFilterClientSurvey" class="modal fade" tabindex="-1">
            <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">Filter by Survey</h5>
                        <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                    </div>
                    <div class="modal-body">
                        <div class="row mb-3 align-items-center">
                            <div class="col-md-auto">Questionaire Title:</div>
                            <div class="col-md-auto font-weight-bold" id="lblFilterQTitle"></div>
                        </div>

                        <ul id="listCRMFilterQuestionsReadOnly" class="list-group" style="list-style-type:decimal;"></ul>
                    </div>
                    <div class="modal-footer">
                        <div class="row">
                            <div class="col-6">
                                <button data-role="none" type="button" class="btn btn-secondary btn-block btn-lg" data-dismiss="modal">Cancel</button>
                            </div>
                            <div class="col-6">
                                <button data-role="none" type="button" id="btnCRMFilterSurveyOk" class="btn btn-success btn-block btn-lg">Ok</button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div id="popCRMFileExist" class="modal fade" tabindex="-1">
            <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">Rename Existing File(s)</h5>
                        <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                    </div>
                    <div class="modal-body">
                        <div id="divExistingFile"></div>
                    </div>
                    <div class="modal-footer">
                        <div class="row">
                            <div class="col-6 order-md-0 order-1">
                                <button data-role="none" type="button" class="btn btn-secondary btn-block btn-lg" data-dismiss="modal">Cancel</button>
                            </div>
                            <div class="col-6 order-md-1 order-0">
                                <button data-role="none" type="button" id="btnCRMContinueUpload" class="btn btn-success btn-block btn-lg">Continue Upload</button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div id="popCRMAddress" class="modal fade" tabindex="-1">
            <div class="modal-dialog modal-dialog-centered">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">Organization Address</h5>
                        <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                    </div>
                    <div class="modal-body">
                        <label class="form-label" for="txtCRMCompleteAddress">Complete Address:</label>
                        <textarea data-role="none" id="txtCRMCompleteAddress" class="form-control"></textarea>
                    </div>
                    <div class="modal-footer">
                        <div class="row">
                            <div class="col-md order-1 order-md-0">
                                <button data-role="none" type="button" class="btn btn-secondary btn-block btn-lg" data-dismiss="modal">Close</button>
                            </div>
                            <div class="col-md order-1 order-md-0">
                                <button data-role="none" type="button" id="btnCRMOkAddress" class="btn btn-success btn-block btn-lg">Ok</button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div id="divDashboardOpenToDo" data-backdrop="static" class="modal fade" tabindex="-1" style="display:none;">
            <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">To-Do Items of <span id="txtOpenToDoName"></span></h5>
                        <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                    </div>
                    <div class="modal-body">
                        <ul id="listCRMDashboardToDo" data-role="none" class="list-group"></ul>
                    </div>
                    <div class="modal-footer">
                        <button data-role="none" data-dismiss="modal" class="btn btn-secondary btn-lg btn-block">Close</button>
                    </div>
                </div>
            </div>
        </div>
    </div>
    @*End Pop Ups*@
    <div id="divCRMPages">
        <div subpage="pageCRMLeadGen" id="pageCRMMainDashboard">
            <div data-role="content">
                <div id="divCRMDashboardContent" style="display:block;">
                    <div id="tabCrm" style="white-space:nowrap; overflow:auto;">
                        <ul class="nav nav-tabs" role="tablist" style="flex-wrap:nowrap;">
                            <li class="nav-item" role="presentation">
                                <button data-role="none" id="btnCRMTabCrmDashboard" class="nav-link active" data-toggle="tab" data-target="#tabCrmDashboard"><span class="lblCRMModule"></span> CRM Dashboard</button>
                            </li>
                            <li class="nav-item" role="presentation">
                                <button data-role="none" id="btnCRMTabCrmSupport" class="nav-link" data-toggle="tab" data-target="#tabCrmSupportCrm">CRM</button>
                            </li>
                            <li class="nav-item" role="presentation">
                                <button data-role="none" id="btnCRMTabCrmServiceTicket" class="nav-link" data-toggle="tab" data-target="#tabCrmTicketService">Issues</button>
                            </li>
                            <li class="nav-item" role="presentation">
                                <button data-role="none" id="btnCRMTabCrmOpportunities" class="nav-link" data-toggle="tab" data-target="#tabCrmOpportunities">Opportunites</button>
                            </li>
                            <li class="nav-item" role="presentation">
                                <button data-role="none" id="btnCRMTabCrmSurvey" class="nav-link" data-toggle="tab" data-target="#tabCrmSurvey">Survey</button>
                            </li>
                            <li class="nav-item" role="presentation">
                                <button data-role="none" id="btnCRMTabCrmMyReport" class="nav-link" data-toggle="tab" data-target="#tabCrmMyReport">Activity Report</button>
                            </li>
                            <li class="nav-item" role="presentation">
                                <button data-role="none" id="btnCRMTabCrmSettings" class="nav-link" data-toggle="tab" data-target="#tabCrmSettings">Settings</button>
                            </li>
                        </ul>
                    </div>

                    <div class="tab-content p-3">
                        <div id="tabCrmDashboard" class="tab-pane fade show active">
                            <div subpage="pageDashboard" id="divMainDB">
                                <div class="row">
                                    <div class="col-md-4">
                                        <div class="card text-dark bg-warning mb-3">
                                            <div class="card-header">
                                                <div subpage="pageopentodo" class="divopentodolist">
                                                    <div class="d-flex align-items-center">
                                                        <p class="p-0 m-0 flex-grow-1 font-weight-bold">Open To-Do Items</p>
                                                        <span id="btnOpenToDoSearch" class="bi-search"></span>
                                                    </div>
                                                </div>
                                                <div subpage="pageopentodo" class="divopentodosearch" style="display:none;">
                                                    <div class="input-group">
                                                        <input data-role="none" type="text" id="txtSearchOpenToDo" class="form-control" placeholder="Search here" />
                                                        <button data-role="none" id="btnBackOpenToDoSearch" class="btn btn-warning"><span class="bi-x"></span></button>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="card-body">
                                                <div subpage="pageopentodo" class="divopentodolist">
                                                    <ul id="lstOpenToDo" data-role="none" class="list-group list-group-flush mb-3"></ul>
                                                    <button data-role="none" id="btnShowMoreOpenToDo" class="btn btn-warning btn-block">Show More</button>
                                                </div>
                                                <div subpage="pageopentodo" class="divopentodosearch" style="display:none;">
                                                    <ul id="lstOpenToDoSearch" data-role="none" class="list-group list-group-flush mb-3"></ul>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-md-4">
                                        <div class="card text-dark bg-info mb-3">
                                            <div class="card-header">
                                                <div subpage="pagescheduledtask" class="divscheduledtasklist">
                                                    <div class="d-flex align-items-center">
                                                        <p class="p-0 m-0 flex-grow-1 font-weight-bold">Scheduled Tasks</p>
                                                        <span id="btnScheduledSearch" class="bi-search"></span>
                                                    </div>
                                                </div>
                                                <div subpage="pagescheduledtask" class="divscheduledtasksearch" style="display:none;">
                                                    <div class="input-group">
                                                        <input data-role="none" type="text" id="txtSearchScheduledTask" class="form-control" placeholder="Search here" />
                                                        <button data-role="none" id="btnBackScheduleSearch" class="btn btn-warning"><span class="bi-x"></span></button>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="card-body">
                                                <div subpage="pagescheduledtask" class="divscheduledtasklist">
                                                    <ul id="lstSchedule" data-role="none" class="list-group list-group-flush mb-3"></ul>
                                                    <button data-role="none" id="btnShowMoreSched" class="btn btn-info btn-block">Show More</button>
                                                </div>
                                                <div subpage="pagescheduledtask" class="divscheduledtasksearch" style="display:none;">
                                                    <ul id="lstScheduleSearch" data-role="none" class="list-group list-group-flush mb-3"></ul>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-md-4">
                                        <div class="card bg-success mb-3">
                                            <div class="card-header">
                                                <div class="d-flex align-items-center">
                                                    <p class="p-0 m-0 flex-grow-1 font-weight-bold">Special Events</p>
                                                    <span id="btnScheduledSearch" class="bi-search"></span>
                                                </div>
                                            </div>
                                            <div class="card-body">
                                                <ul data-role="none" class="list-group list-group-flush mb-3">
                                                    <li class="list-group-item bg-success">No Special Events</li>
                                                </ul>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <div subpage="pageDashboard" id="divDashboardSched" style="display:none;">
                                Schedule List
                            </div>

                            <div class="fixed-ActionBtn">
                                <button data-role="none" data-toggle="tooltip" data-placement="left" title="Help" class="btn btn-danger btn-lg rounded-circle btnKbHelp">
                                    <span class="bi-question-lg"></span>
                                </button>
                            </div>
                        </div>
                        <div id="tabCrmTicketService" class="tab-pane fade">
                            <div class="row">
                                <div class="col-md-6 mb-3"><input data-role="none" type="text" id="txtSearchTicket" placeholder="Search here" class="form-control" /></div>
                                <div class="col-md-6 text-right">
                                    <button id="btnCRMTicketRefresh" data-toggle="tooltip" data-placement="top" title="Refresh" data-role="none" class="btn btn-secondary">
                                        Refresh List
                                    </button>
                                    <button data-toggle="tooltip" data-placement="top" title="Help" data-role="none" class="btn btn-secondary btnKbHelp">
                                        Help
                                    </button>
                                    <button id="btnDashboardAddTicket" data-role="none" class="btn btn-success">
                                        Add Issue Ticket
                                    </button>
                                </div>
                            </div>
                            <br />
                            <table id="tblDataTableTickets" data-role="none" style="display:table; width:100%" class="table table-light table-striped table-hover">
                                <thead>
                                    <tr class="custom-theme-f">
                                        <th style="width:20px !important;"> # </th>
                                        <th style="cursor:pointer; vertical-align:middle;"> Client Name </th>
                                        <th style="cursor:pointer; width:100px"> Unresolved </th>
                                        <th style="cursor:pointer; width:100px"> Resolved </th>
                                        <th style="cursor:pointer; width:100px"> Cancelled </th>
                                        <th style="cursor:pointer; width:100px"> Total </th>
                                    </tr>
                                </thead>
                                <tbody></tbody>
                            </table>
                        </div>
                        <div id="tabCrmSupportCrm" class="tab-pane fade">
                            <div class="row">
                                <div class="col-md-3">
                                    Show: <select id="selCRMStatus" data-role="none" class="form-select">
                                        <option value="All">All</option>
                                        <option value="1" selected>Active</option>
                                        <option value="0">Inactive</option>
                                    </select>
                                </div>
                                <div class="col-md-3">
                                    Activity: <select id="selInvoiceType" data-role="none" class="form-control">
                                        <option value="Invoice">Sales Invoices</option>
                                        <option value="SalesOrder">Sales Orders</option>
                                        <option value="Quote">Sales Quotes</option>
                                    </select>
                                </div>
                                <div class="col-md-3">
                                    Made Within:
                                    <select id="selInvoiceMadeWith" data-role="none" class="form-control">
                                        <option value="1">Within the last week</option>
                                        <option value="2">Within the last 2 weeks</option>
                                        <option value="3">Within the last 3 weeks</option>
                                        <option value="4">Within the last 4 weeks</option>
                                        <option value="6">Within the last 6 weeks</option>
                                        <option value="8">Within the last 8 weeks</option>
                                        <option value="12">Within the last 12 weeks</option>
                                    </select>
                                </div>
                                <div class="col-md-3">
                                    &nbsp;
                                    <input type="text" data-role="none" class="form-control" id="txtCRMSearchClient" placeholder="Search here" class="form-control" />
                                </div>
                            </div>
                            <br />

                            <form id="formupload" enctype="multipart/form-data" method="post" style="display:none;">
                                <div style="display:none;"><input type="text" data-role="none" class="form-control" id="txtCRMUploadExcelOrgID" name="OrgID" value="28b8a792-cb65-4d92-988f-d721123c93b9" style="display:none;"></div>
                                <input type="file" id="objFile" name="objFile">
                                <input type="submit" id="btnCRMUploadExcel" value="Upload File">
                            </form>

                            <div class="custom-ui-label" id="divAdvanceFilter" style="display:none;">
                                <div>Filtered By:</div>
                                <div id="divAdvanceFilterList">
                                </div>
                            </div>

                            <div id="divCountRow"></div>
                            @*<br />*@
                            <div style="overflow:auto;" class="mb-5">
                                <div subpage="pageMainDashboard" id="divtblLeadGen" style="display:block;">
                                    <table id="tblDataTable" data-role="none" style="display:table; width:100%" class="table table-light table-striped table-hover">
                                        <thead>
                                            <tr class="custom-theme-f" style="font-size:12px; vertical-align:top; text-align:center;">
                                                <th style="width:20px !important;"> </th>
                                                <th style="cursor:pointer;"> Name </th>
                                                <th style="cursor:pointer;"> Business Industry </th>
                                                <th style="cursor:pointer;"> Contact Info </th>
                                                <th style="cursor:pointer;"> Address </th>
                                                <th style="cursor:pointer;"> Organization Affiliation </th>
                                                <th style="cursor:pointer;"> Last Ordered Date </th>
                                                <th style="cursor:pointer;"> Last Invoiced Date </th>
                                                <th style="cursor:pointer;"> For Follow Up Orders </th>
                                                <th style="cursor:pointer;"> Date Created/Modified </th>
                                                <th style="cursor:pointer;"> Last Interaction Date </th>
                                                <th style="cursor:pointer;"> Score </th>
                                            </tr>
                                        </thead>
                                        <tbody></tbody>
                                    </table>
                                </div>
                                <div subpage="pageMainDashboard" id="divtblSalesGrinder" style="display:none;">
                                    <table id="tblDataTableSalesGrinder" data-role="none" style="display:table; width:100%" class="table table-light table-striped table-hover">
                                        <thead>
                                            <tr class="custom-theme-f">
                                                <th style="width:20px !important;"> </th>
                                                <th style="cursor:pointer;"> Name </th>
                                                <th style="cursor:pointer;"> Business Industry </th>
                                                <th style="cursor:pointer;"> Contact Person </th>
                                                <th style="cursor:pointer;"> Gross Sales This Year </th>
                                            </tr>
                                        </thead>
                                        <tbody></tbody>
                                    </table>
                                </div>
                            </div>

                            @*<div id="btnCRMAddProspect" class="floatingButton custom-btn-theme-d" title="Options"><img src="/Content/images/plus-white.png" alt="Add"></div>*@
                            <div class="fixed-ActionBtn" style="display:block;">
                                <button data-role="none" class="btn btn-danger rounded-circle">
                                    <span class="bi-plus-lg"></span>
                                </button>
                                <ul>
                                    <li><button data-role="none" id="btnCRMAddOrgProspect" class="btn btn-dark" data-title="Organization"><span class="bi-building"></span></button></li>
                                    <li><button data-role="none" id="btnCRMAddPeopleProspect" class="btn btn-dark" data-title="Person"><span class="bi-person-fill"></span></button></li>
                                    <li id="lisAddExcelProspect"><button data-role="none" id="btnCRMAddExcelProspect" class="btn btn-dark" data-title="Upload Contacts Via Excel file"><span class="bi-upload"></span></button></li>
                                    <li id="lisDownloadExcelProspect"><button data-role="none" id="btnCRMDownloadExcelProspect" class="btn btn-dark" data-title="Download Contacts Excel template"><span class="bi-download"></span></button></li>
                                    <li id="lisImportToCRM"><button data-role="none" id="btnCRMImportToCRM" class="btn btn-dark" data-title="Import records from customer and vendor libraries"><span class="bi-box-arrow-in-down"></span></button></li>
                                </ul>
                                <ol class="secondaryActionBtn">
                                    <li>
                                        <button id="btnCRMBackToTop" data-toggle="tooltip" data-placement="top" title="Back to top" data-role="none" style="display:none;" class="btn btn-dark">
                                            <span class="bi-chevron-up"></span>
                                        </button>
                                    </li>
                                    <li>
                                        <button id="btnCRMCRMRefresh" data-toggle="tooltip" data-placement="top" title="Refresh" data-role="none" class="btn btn-dark">
                                            <span class="bi-arrow-clockwise"></span>
                                        </button>
                                    </li>
                                    <li>
                                        <button data-toggle="tooltip" data-placement="top" title="Help" data-role="none" class="btn btn-dark btnKbHelp">
                                            <span class="bi-question-lg"></span>
                                        </button>
                                    </li>
                                    <li>
                                        <button id="btnCRMBulkAction" data-toggle="tooltip" data-placement="top" title="Compose Message" data-role="none" class="btn btn-dark">
                                            <span class="bi-envelope-fill"></span>
                                        </button>
                                    </li>
                                    <li>
                                        <button id="btnCRMAddFilter" data-toggle="tooltip" data-placement="top" title="Filter" data-role="none" class="btn btn-dark">
                                            <span class="bi-funnel-fill"></span>
                                        </button>
                                    </li>
                                </ol>
                            </div>
                        </div>
                        <div id="tabCrmOpportunities" class="tab-pane fade">
                            <div class="row">
                                <div class="col-md-6 mb-3"><input data-role="none" type="text" id="txtSearchOpportunity" placeholder="Search here" class="form-control" /></div>
                                <div class="col-md-6 text-right">
                                    <button id="btnCRMOpportunityRefresh" data-toggle="tooltip" data-placement="top" title="Refresh" data-role="none" class="btn btn-secondary">
                                        Refresh List
                                    </button>
                                    <button data-toggle="tooltip" data-placement="top" title="Help" data-role="none" class="btn btn-secondary btnKbHelp">
                                        Help
                                    </button>
                                    <button id="btnDashboardAddOpp" data-role="none" class="btn btn-success">
                                        Add Opportunity
                                    </button>
                                </div>
                            </div>
                            <br />
                            <div class="table-responsive">
                                <table id="dtOpportunity" data-role="none" style="display:table; width:100%" class="table table-light table-striped table-hover">
                                    <thead>
                                        <tr class="custom-theme-f text-center">
                                            <th style="width:20px !important;"> # </th>
                                            <th style="cursor:pointer;"> Client Name </th>
                                            <th style="width:100px !important; cursor:pointer;"> No. of Opportunities</th>
                                            <th style="width:100px !important; cursor:pointer;"> Won </th>
                                            <th style="width:100px !important; cursor:pointer;"> Lost </th>
                                            <th style="width:100px !important; cursor:pointer;"> In Progress </th>
                                            <th style="width:100px !important; cursor:pointer;"> Totals </th>
                                        </tr>
                                    </thead>
                                    <tbody></tbody>
                                </table>
                            </div>
                        </div>
                        <div id="tabCrmSurvey" class="tab-pane fade"><div class="divspinner text-center"><div class="spinner-border text-primary text-center" role="status"><span class="visually-hidden"></span></div></div></div>
                        <div id="tabCrmMyReport" class="tab-pane fade"><div class="divspinner text-center"><div class="spinner-border text-primary text-center" role="status"><span class="visually-hidden"></span></div></div></div>
                        <div id="tabCrmSettings" class="tab-pane fade">
                            <div class="erpItemBCContainer"></div>
                            <div id="divCrmSettings"><div class="divspinner text-center"><div class="spinner-border text-primary text-center" role="status"><span class="visually-hidden"></span></div></div></div>
                        </div>
                    </div>                    
                </div>
            </div>
        </div>

        <div subpage="pageCRMLeadGen" id="pageCRMAddOrgProspect">
            <div data-role="content">

            </div>
        </div>

        <div subpage="pageCRMLeadGen" id="pageCRMAddPersonProspect">
            <div data-role="content">

            </div>
        </div>

        <div subpage="pageCRMLeadGen" id="pageCRMSettings">

        </div>

        <div id="pageCRMActions" subpage="pageCRMLeadGen" style="width:100%;">
            <div data-role="content">
                <div class="row wrapper min-vh-100 flex-column flex-sm-row">
                    <div class="col-12 col-md-2 p-0 bg-light flex-shrink-1">
                        <nav class="navbar navbar-expand-sm navbar-light bg-light align-items-start flex-sm-column flex-row p-2">
                            <a data-role="none" class="navbar-brand" href="#">Menu</a>
                            <a data-role="none" href="#" class="navbar-toggler" data-toggle="collapse" data-target=".sidebarcrm">
                                <span class="navbar-toggler-icon"></span>
                            </a>
                            <div class="collapse navbar-collapse sidebarcrm mt-2">
                                <ul class="flex-column w-100 justify-content-between nav nav-pills">
                                    <li><a class="nav-link active" data-role="none" id="btnCRMPageProfile" data-toggle="tab" href="#pageCRMProfile">Profile</a></li>
                                    <li><a class="nav-link" data-role="none" id="btnCRMPageOrderHistory" data-toggle="tab" href="#pageCRMOrderHistory">Order History</a></li>
                                    <li><a class="nav-link" data-role="none" id="btnCRMPageInteractionHistory" data-toggle="tab" href="#pageCRMInteractionHistory">Interaction History</a></li>
                                    <li><a class="nav-link" data-role="none" id="btnCRMPageOpportunity" data-toggle="tab" href="#pageCRMOpportunity">Opportunities</a></li>
                                    <li class="btnSupporTicket"><a class="nav-link" data-role="none" id="btnCRMPageTickets" data-toggle="tab" href="#pageCRMTickets">Issues</a></li>
                                    <li><a class="nav-link" data-role="none" id="btnCRMPageScheduleAnAction" data-toggle="tab" href="#pageCRMScheduleAnAction">Scheduled Actions</a></li>
                                    <li><a class="nav-link" data-role="none" id="btnCRMPageToDo" data-toggle="tab" href="#pageCRMToDo">To Dos</a></li>
                                    <li><a class="nav-link" data-role="none" id="btnCRMPageColdCall" data-toggle="tab" href="#pageCRMColdCall">Calls</a></li>
                                    <li><a class="nav-link" data-role="none" id="btnCRMPageTakeANote" data-toggle="tab" href="#pageCRMTakeANote">Notes</a></li>
                                    <li><a class="nav-link" data-role="none" id="btnCRMPageSMS" data-toggle="tab" href="#pageCRMSMS">SMS</a></li>
                                    @*<li><a class="nav-link" data-role="none" id="btnCRMPageSurvey" data-toggle="tab" href="#pageCRMSurveys">Surveys</a></li>*@
                                    <li><a class="nav-link" data-role="none" id="btnCRMPageFile" data-toggle="tab" href="#pageCRMFile">Files</a></li>
                                    <li><a class="nav-link" data-role="none" id="" data-toggle="tab" href="#btnForDTRViolations">Violations and Sanctions</a></li>
                                </ul>
                            </div>
                        </nav>
                    </div>

                    <div class="col bg-faded">
                        <div class="tab-content p-3">
                            <div class="tab-pane fade active show" id="pageCRMProfile">
                                <div subpage="pageProfile" id="pageCRMProfileOrg" style="display:none;">
                                    <div id="divCompanyProfile" style="display:block;">
                                        <div id="divTabCompany">
                                            <ul class="nav nav-tabs mb-3">
                                                <li class="nav-item">
                                                    <button data-role="none" class="nav-link active" data-toggle="tab" data-target="#tabCRMCompanyProfile">Profile</button>
                                                </li>
                                                <li class="nav-item">
                                                    <button data-role="none" class="nav-link" data-toggle="tab" data-target="#tabCRMCompanyContactInfo">Contact Info</button>
                                                </li>
                                                <li class="nav-item">
                                                    <button data-role="none" class="nav-link" data-toggle="tab" data-target="#tabCRMCompanyContactPerson">Contact Person</button>
                                                </li>
                                                <li class="nav-item">
                                                    <button data-role="none" class="nav-link" data-toggle="tab" data-target="#tabCRMCompanySize">Company Size</button>
                                                </li>
                                                @*<li class="nav-item">
                                                    <button data-role="none" class="nav-link" data-toggle="tab" data-target="#tabCRMRevenue">Revenue</button>
                                                    do not show this tab anymore. this is only application to eas. comment by arnie 71917
                                                </li>
                                                <li class="nav-item">
                                                    <button data-role="none" class="nav-link" data-toggle="tab" data-target="#tabCRMFowst">FOWST</button>
                                                </li>*@
                                            </ul>
                                        </div>

                                        <div class="tab-content">
                                            <div id="tabCRMCompanyProfile" class="tab-pane fade show active">
                                                <div class="mb-3">
                                                    <input data-role="none" type="text" id="txtCRMProspectOrgID" style="display:none;" />
                                                    <span style="font-size:12px;">Fields marked with <span style="color:#F00;">*</span> are required.</span>
                                                </div>
                                                
                                                <div class="row mb-3">
                                                    <label class="col-form-label col-md-2" for="selCRMOrgStatus">Status:</label>
                                                    <div class="col-md">
                                                        <select id="selCRMOrgStatus" data-role="none" class="form-select">
                                                            <option value="Active">Active</option>
                                                            <option value="Inactive">Inactive</option>
                                                        </select>
                                                    </div>
                                                </div>

                                                <div class="row mb-3">
                                                    <label class="col-form-label col-md-2" for="txtCRMProspectCompanyName">Organization Name: <span style="color: #FF0000;">*</span></label>
                                                    <div class="col-md">
                                                        <input data-role="none" class="form-control" type="text" id="txtCRMProspectCompanyName" />
                                                    </div>
                                                </div>

                                                <div class="row mb-3">
                                                    <label class="col-form-label col-md-2" for="txtCRMProspectHouseBldgNo">Organization Address:</label>
                                                    <div class="col-md">
                                                        <input data-role="none" class="form-control mb-3" type="text" id="txtCRMProspectHouseBldgNo" placeholder="Building No." />
                                                        <input data-role="none" class="form-control mb-3" type="text" id="txtCRMProspectStreetName" placeholder="Street" />
                                                        <select data-role="none" class="form-select mb-3" id="selCRMProspectCities">
                                                            <option>Please Select City</option>
                                                        </select>
                                                        <select data-role="none" class="form-select mb-3" id="selCRMProspectOrgRegion"></select>
                                                        <select data-role="none" class="form-select mb-3" id="selCRMProspectIsland">
                                                            <option value="">Please Select Island</option>
                                                            <option value="Luzon">Luzon</option>
                                                            <option value="Visayas">Visayas</option>
                                                            <option value="Mindanao">Mindanao</option>
                                                        </select>
                                                        <select data-role="none" class="form-select mb-3" id="selCRMProspectCountries">
                                                            <option>Please Select Country</option>
                                                        </select>
                                                    </div>
                                                </div>

                                                <div class="row mb-3">
                                                    <label class="col-form-label col-md-2" for="txtCRMProspectCompanyBackground">Organization Background:</label>
                                                    <div class="col-md">
                                                        <textarea data-role="none" class="form-control" style="resize:none; height:150px;" id="txtCRMProspectCompanyBackground"></textarea>
                                                    </div>
                                                </div>

                                                <div class="row mb-3">
                                                    <label class="col-form-label col-md-2" for="txtCRMProspectCompanyMission">Organization Mission:</label>
                                                    <div class="col-md">
                                                        <textarea data-role="none" class="form-control" style="resize:none; height:150px;" id="txtCRMProspectCompanyMission"></textarea>
                                                    </div>
                                                </div>

                                                <div class="row mb-3">
                                                    <label class="col-form-label col-md-2" for="txtCRMProspectCompanyVision">Organization Background:</label>
                                                    <div class="col-md">
                                                        <textarea data-role="none" class="form-control" style="resize:none; height:150px;" id="txtCRMProspectCompanyVision"></textarea>
                                                    </div>
                                                </div>

                                                <div class="row mb-3">
                                                    <label class="col-form-label col-md-2" for="txtCRMProspectCompanyFounding">Founding Date/Year:</label>
                                                    <div class="col-md">
                                                        <input data-role="none" class="form-control" type="text" id="txtCRMProspectCompanyFounding" />
                                                    </div>
                                                </div>

                                                <div class="row mb-3">
                                                    <label class="col-form-label col-md-2" for="btnCRMProspectCompanyIndustry">Business Industry:</label>
                                                    <div class="col-md">
                                                        <input type="text" data-role="none" class="form-control" id="txtCRMBusIndustryID" style="display:none;" />
                                                        <button data-role="none" class="btn btn-secondary btn-block" id="btnCRMProspectCompanyIndustry">Search here</button>
                                                    </div>
                                                </div>

                                                <div class="accordion mb-3">
                                                    <div class="card">
                                                        <div class="card-header p-0">
                                                            <button data-target="#" class="btn btn-light btn-block text-left p-3 rounded-0" data-role="none" data-toggle="collapse">
                                                                <b>Organization Address</b>
                                                            </button>
                                                        </div>
                                                        <div id="" class="collapse show">
                                                            <div class="card-body">
                                                                <div class="text-right mb-3">
                                                                    <button id="btnCRMAddOrgAddress" data-role="none" class="btn btn-success">
                                                                        Add Address
                                                                    </button>
                                                                </div>

                                                                <ul data-role="none" id="lstOrgAddress" class="list-group"></ul>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>

                                                <div class="accordion mb-3">
                                                    <div class="card">
                                                        <div class="card-header p-0">
                                                            <button data-target="#" class="btn btn-light btn-block text-left p-3 rounded-0" data-role="none" data-toggle="collapse">
                                                                <b>Organization Affiliation</b>
                                                            </button>
                                                        </div>
                                                        <div id="" class="collapse show">
                                                            <div class="card-body">
                                                                <div class="text-right mb-3">
                                                                    <button id="btnCRMAddOrgAffiliation" data-role="none" class="btn btn-success">
                                                                        Add Affiliation
                                                                    </button>
                                                                </div>

                                                                <ul data-role="none" id="lstOrgAffiliation" class="list-group"></ul>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>

                                                <div class="accordion mb-3">
                                                    <div class="card">
                                                        <div class="card-header p-0">
                                                            <button data-target="#" class="btn btn-light btn-block text-left p-3 rounded-0" data-role="none" data-toggle="collapse">
                                                                <b>Affiliated Person</b>
                                                            </button>
                                                        </div>
                                                        <div id="" class="collapse show">
                                                            <div class="card-body">
                                                                <div class="text-right mb-3">
                                                                    <button id="btnCRMAddAffiliatedPerson" data-role="none" class="btn btn-success">
                                                                        Add Affiliated Person
                                                                    </button>
                                                                </div>

                                                                <ul data-role="none" id="lstAffiliatedPerson" class="list-group"></ul>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>

                                            <div id="tabCRMCompanyContactInfo" class="tab-pane fade">
                                                <div class="accordion mb-3">
                                                    <div class="card">
                                                        <div class="card-header p-0">
                                                            <button data-target="#" class="btn btn-light btn-block text-left p-3 rounded-0" data-role="none" data-toggle="collapse">
                                                                <b>Telephone Number</b>
                                                            </button>
                                                        </div>
                                                        <div id="" class="collapse show">
                                                            <div class="card-body">
                                                                <div class="text-right mb-3">
                                                                    <button id="btnCRMAddOrgTelephone" data-role="none" class="btn btn-success">
                                                                        Add Telephone
                                                                    </button>
                                                                </div>

                                                                <ul data-role="none" id="lstCRMOrgTelephone" class="list-group"></ul>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>

                                                <div class="accordion mb-3">
                                                    <div class="card">
                                                        <div class="card-header p-0">
                                                            <button data-target="#" class="btn btn-light btn-block text-left p-3 rounded-0" data-role="none" data-toggle="collapse">
                                                                <b>Mobile Number</b>
                                                            </button>
                                                        </div>
                                                        <div id="" class="collapse show">
                                                            <div class="card-body">
                                                                <div class="text-right mb-3">
                                                                    <button id="btnCRMAddOrgMobile" data-role="none" class="btn btn-success">
                                                                        Add Mobile
                                                                    </button>
                                                                </div>

                                                                <ul data-role="none" id="lstCRMOrgMobile" class="list-group"></ul>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>

                                                <div class="accordion mb-3">
                                                    <div class="card">
                                                        <div class="card-header p-0">
                                                            <button data-target="#" class="btn btn-light btn-block text-left p-3 rounded-0" data-role="none" data-toggle="collapse">
                                                                <b>Email Address</b>
                                                            </button>
                                                        </div>
                                                        <div id="" class="collapse show">
                                                            <div class="card-body">
                                                                <div class="text-right mb-3">
                                                                    <button id="btnCRMAddOrgEmail" data-role="none" class="btn btn-success">
                                                                        Add Email Address
                                                                    </button>
                                                                </div>

                                                                <ul data-role="none" id="lstCRMOrgEmail" class="list-group"></ul>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>

                                                <div class="accordion mb-3">
                                                    <div class="card">
                                                        <div class="card-header p-0">
                                                            <button data-target="#" class="btn btn-light btn-block text-left p-3 rounded-0" data-role="none" data-toggle="collapse">
                                                                <b>Website</b>
                                                            </button>
                                                        </div>
                                                        <div id="" class="collapse show">
                                                            <div class="card-body">
                                                                <div class="text-right mb-3">
                                                                    <button id="btnCRMAddOrgWebsite" data-role="none" class="btn btn-success">
                                                                        Add Website
                                                                    </button>
                                                                </div>

                                                                <ul data-role="none" id="lstCRMOrgWebsite" class="list-group"></ul>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>

                                            <div id="tabCRMCompanyContactPerson" class="tab-pane fade">
                                                <div class="text-right mb-3">
                                                    <button id="btnCRMAddContactPerson" data-role="none" class="btn btn-success">
                                                        Add Contact Person
                                                    </button>
                                                </div>

                                                <ul data-role="none" id="lstCRMOrgContactPerson" class="list-group"></ul>
                                            </div>

                                            <div id="tabCRMCompanySize" class="tab-pane fade">
                                                <div class="row mb-3">
                                                    <label class="col-form-label col-md-2" for="txtCRMProspectNoOfEmployees">Number of Employees:</label>
                                                    <div class="col-md">
                                                        <input data-role="none" class="form-control" type="text" id="txtCRMProspectNoOfEmployees" />
                                                    </div>
                                                </div>

                                                <div class="row mb-3">
                                                    <label class="col-form-label col-md-2" for="txtCRMProspectRevenuePerYear">Revenue per Year:</label>
                                                    <div class="col-md">
                                                        <input data-role="none" class="form-control" type="text" id="txtCRMProspectRevenuePerYear" />
                                                    </div>
                                                </div>

                                                <div class="row mb-3">
                                                    <label class="col-form-label col-md-2" for="selCRMProspectScopeOperation">Scope of Operations:</label>
                                                    <div class="col-md">
                                                        <select id="selCRMProspectScopeOperation" data-role="none" class="form-select">
                                                            <option value="">Please Select</option>
                                                            <option value="Regional">Regional</option>
                                                            <option value="National">National</option>
                                                            <option value="Trans-National">Trans-National</option>
                                                        </select>
                                                    </div>
                                                </div>

                                                <div class="row mb-3">
                                                    <label class="col-form-label col-md-2" for="txtCRMProspectNoFactories">Number of Factories:</label>
                                                    <div class="col-md">
                                                        <input data-role="none" class="form-control" type="text" id="txtCRMProspectNoFactories" />
                                                    </div>
                                                </div>

                                                <div class="row mb-3">
                                                    <label class="col-form-label col-md-2" for="txtCRMProspectNoOffices">Number of Offices:</label>
                                                    <div class="col-md">
                                                        <input data-role="none" class="form-control" type="text" id="txtCRMProspectNoOffices" />
                                                    </div>
                                                </div>

                                                <div class="row mb-3">
                                                    <label class="col-form-label col-md-2" for="txtCRMProspectNoWarehouses">Number of Warehouses:</label>
                                                    <div class="col-md">
                                                        <input data-role="none" class="form-control" type="text" id="txtCRMProspectNoWarehouses" />
                                                    </div>
                                                </div>

                                                <div class="row mb-3">
                                                    <label class="col-form-label col-md-2" for="txtCRMProspectNoStores">Number of Stores:</label>
                                                    <div class="col-md">
                                                        <input data-role="none" class="form-control" type="text" id="txtCRMProspectNoStores" />
                                                    </div>
                                                </div>

                                                <div class="row mb-3">
                                                    <label class="col-form-label col-md-2" for="txtCRMProspectNoTrucks">Number of Trucks:</label>
                                                    <div class="col-md">
                                                        <input data-role="none" class="form-control" type="text" id="txtCRMProspectNoTrucks" />
                                                    </div>
                                                </div>
                                            </div>

                                            <div id="tabCRMRevenue" class="tab-pane fade">
                                                <div class="row mb-3">
                                                    <label class="col-form-label col-md-2" for="txtCRMRevSalesRep">Sales Representative:</label>
                                                    <div class="col-md">
                                                        <input data-role="none" class="form-control" type="text" id="txtCRMRevSalesRep" />
                                                    </div>
                                                </div>

                                                <div class="row mb-3">
                                                    <label class="col-form-label col-md-2" for="txtCRMRevPriceLevel">Prive Level:</label>
                                                    <div class="col-md">
                                                        <input data-role="none" class="form-control" type="text" id="txtCRMRevPriceLevel" />
                                                    </div>
                                                </div>

                                                <div class="row mb-3">
                                                    <label class="col-form-label col-md-2" for="txtCRMRevCreditTerms">Credit Terms:</label>
                                                    <div class="col-md">
                                                        <input data-role="none" class="form-control" type="text" id="txtCRMRevCreditTerms" />
                                                    </div>
                                                </div>

                                                <div class="row mb-3">
                                                    <label class="col-form-label col-md-2" for="txtCRMRevCreditLimit">Credit Limit:</label>
                                                    <div class="col-md">
                                                        <input data-role="none" class="form-control" type="number" id="txtCRMRevCreditLimit" />
                                                    </div>
                                                </div>

                                                <div class="row mb-3">
                                                    <label class="col-form-label col-md-2" for="txtRevCreditBegBal">Credit Beg. Balance:</label>
                                                    <div class="col-md">
                                                        <input data-role="none" class="form-control" type="number" id="txtRevCreditBegBal" />
                                                    </div>
                                                </div>

                                                <div class="row mb-3">
                                                    <label class="col-form-label col-md-2" for="txtRevCreditBegBal">Credit Beg. as of:</label>
                                                    <div class="col-md">
                                                        <button data-role="none" class="btn btn-secondary btn-block" id="txtCRMRevCreditBegAsOf">Set Date</button>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                           
                                        <div id="divProspectMainButton" class="fixed-bottom text-center pb-3" style="display:block; left:50%; transform: translate(-50%, 0); right:auto">
                                            <button data-role="none" id="btnCRMCancelAddingContact" class="btn btn-secondary btn-lg">
                                                Back
                                            </button>
                                            <button data-role="none" id="btnCRMSaveAddingContact" class="btn btn-success btn-lg">
                                                Save
                                            </button>
                                        </div>
                                    </div>                                    
                                </div>
                                <div subpage="pageProfile" id="pageCRMProfilePeople" style="display:none;">
                                    <div id="divTabPerson" class="custom-tab">
                                        <ul class="nav nav-tabs mb-3">
                                            <li class="nav-item">
                                                <button data-role="none" class="nav-link active" data-toggle="tab" data-target="#tabCRMPersonProfile">Profile</button>
                                            </li>
                                            <li class="nav-item">
                                                <button data-role="none" class="nav-link" data-toggle="tab" data-target="#tabCRMPersonContactInfo">Contact Info</button>
                                            </li>
                                            <li class="nav-item">
                                                <button data-role="none" class="nav-link" data-toggle="tab" data-target="#tabCRMPersonEducation">Educational Background</button>
                                            </li>
                                        </ul>
                                    </div>

                                    <div class="tab-content">
                                        <div id="tabCRMPersonProfile" class="tab-pane fade show active">
                                            <div class="mb-3">
                                                <input data-role="none" type="text" id="txtCRMProspectPersonID" style="display:none;" />
                                                <span style="font-size:12px;">Fields marked with <span style="color:#F00;">*</span> are required.</span>
                                            </div>

                                            <div class="row mb-3">
                                                <label class="col-form-label col-md-2" for="selCRMPersonStatus">Status:</label>
                                                <div class="col-md">
                                                    <select id="selCRMPersonStatus" data-role="none" class="form-select">
                                                        <option value="Active">Active</option>
                                                        <option value="Inactive">Inactive</option>
                                                    </select>
                                                </div>
                                            </div>

                                            <div class="row mb-3">
                                                <label class="col-form-label col-md-2" for="txtCRMProspectPersonFirstName">Full Name: <span style="color:#F00;">*</span></label>
                                                <div class="col-md">
                                                    <div class="row">
                                                        <div class="col-md">
                                                            <input data-role="none" class="form-control" type="text" id="txtCRMProspectPersonFirstName" placeholder="First Name" />
                                                        </div>
                                                        <div class="col-md">
                                                            <input data-role="none" class="form-control" type="text" id="txtCRMProspectPersonMiddleName" placeholder="Middle Name" />
                                                        </div>
                                                        <div class="col-md">
                                                            <input data-role="none" class="form-control" type="text" id="txtCRMProspectPersonLastName" placeholder="Last Name" />
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="row mb-3">
                                                <label class="col-form-label col-md-2" for="selCRMProspectPersonGenSuffix">Generational Suffix:</label>
                                                <div class="col-md">
                                                    <select id="selCRMProspectPersonGenSuffix" data-role="none" class="form-select">
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
                                                <label class="col-form-label col-md-2" for="btnCRMProspectPersonBirthday">Date of Birth:</label>
                                                <div class="col-md">
                                                    <button data-role="none" class="btn btn-secondary btn-block" id="btnCRMProspectPersonBirthday">Set Date</button>
                                                </div>
                                            </div>

                                            <div class="row mb-3">
                                                <label class="col-form-label col-md-2" for="txtCRMProspectPersonPosition">Position/Title:</label>
                                                <div class="col-md">
                                                    <input data-role="none" class="form-control" type="text" id="txtCRMProspectPersonPosition" />
                                                </div>
                                            </div>

                                            <div class="row mb-3">
                                                <label class="col-form-label col-md-2" for="txtCRMProspectPersonPositionDescription">Position/Title Description:</label>
                                                <div class="col-md">
                                                    <input data-role="none" class="form-control" type="text" id="txtCRMProspectPersonPositionDescription" />
                                                </div>
                                            </div>

                                            <div class="accordion mb-3">
                                                <div class="card">
                                                    <div class="card-header p-0">
                                                        <button data-target="#" class="btn btn-light btn-block text-left p-3 rounded-0" data-role="none" data-toggle="collapse">
                                                            <b>Address</b>
                                                        </button>
                                                    </div>
                                                    <div id="" class="collapse show">
                                                        <div class="card-body">
                                                            <input data-role="none" type="text" id="txtCRMPersonAddressID" style="display:none;" />
                                                            <div class="row mb-3">
                                                                <label class="col-form-label col-md-2" for="txtCRMPersonHouseBuildingNo">House or Building No.:</label>
                                                                <div class="col-md">
                                                                    <input data-role="none" class="form-control" type="text" id="txtCRMPersonHouseBuildingNo" />
                                                                </div>
                                                            </div>

                                                            <div class="row mb-3">
                                                                <label class="col-form-label col-md-2" for="txtCRMPersonStreet">Street:</label>
                                                                <div class="col-md">
                                                                    <input data-role="none" class="form-control" type="text" id="txtCRMPersonStreet" />
                                                                </div>
                                                            </div>

                                                            <div class="row mb-3">
                                                                <label class="col-form-label col-md-2" for="selCRMPersonCity">Street:</label>
                                                                <div class="col-md">
                                                                    <select id="selCRMPersonCity" data-role="none" class="form-select"></select>
                                                                </div>
                                                            </div>

                                                            <div class="row mb-3">
                                                                <label class="col-form-label col-md-2" for="selCRMPersonRegion">Region:</label>
                                                                <div class="col-md">
                                                                    <select id="selCRMPersonRegion" data-role="none" class="form-select"></select>
                                                                </div>
                                                            </div>

                                                            <div class="row mb-3">
                                                                <label class="col-form-label col-md-2" for="selCRMPersonIsland">Island Group:</label>
                                                                <div class="col-md">
                                                                    <select id="selCRMPersonIsland" data-role="none" class="form-select">
                                                                        <option value="">Please Select Island</option>
                                                                        <option value="Luzon">Luzon</option>
                                                                        <option value="Visayas">Visayas</option>
                                                                        <option value="Mindanao">Mindanao</option>
                                                                    </select>
                                                                </div>
                                                            </div>

                                                            <div class="row mb-3">
                                                                <label class="col-form-label col-md-2" for="selCRMPersonCountry">Country:</label>
                                                                <div class="col-md">
                                                                    <select id="selCRMPersonCountry" data-role="none" class="form-select"></select>
                                                                </div>
                                                            </div>

                                                            <div class="row mb-3">
                                                                <label class="col-form-label col-md-2" for="selCRMPersonTypeAddress">Type of Address:</label>
                                                                <div class="col-md">
                                                                    <select id="selCRMPersonTypeAddress" data-role="none" class="form-select">
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

                                            <div class="accordion mb-3">
                                                <div class="card">
                                                    <div class="card-header p-0">
                                                        <button data-target="#" class="btn btn-light btn-block text-left p-3 rounded-0" data-role="none" data-toggle="collapse">
                                                            <b>Organization Affiliation</b>
                                                        </button>
                                                    </div>
                                                    <div id="" class="collapse show">
                                                        <div class="card-body">
                                                            <div class="text-right mb-3">
                                                                <button id="btnCRMAddPersonOrgAffiliation" data-role="none" class="btn btn-success">
                                                                    Add Affiliation
                                                                </button>
                                                            </div>

                                                            <ul data-role="none" id="lstPersonOrgAffiliation" class="list-group"></ul>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>

                                        <div id="tabCRMPersonContactInfo" class="tab-pane fade">
                                            <div class="accordion mb-3">
                                                <div class="card">
                                                    <div class="card-header p-0">
                                                        <button data-target="#" class="btn btn-light btn-block text-left p-3 rounded-0" data-role="none" data-toggle="collapse">
                                                            <b>Telephone Number</b>
                                                        </button>
                                                    </div>
                                                    <div id="" class="collapse show">
                                                        <div class="card-body">
                                                            <div class="text-right mb-3">
                                                                <button id="btnCRMAddPersonTelephone" data-role="none" class="btn btn-success">
                                                                    Add Telephone
                                                                </button>
                                                            </div>

                                                            <ul data-role="none" id="lstPersonTelephone" class="list-group"></ul>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="accordion mb-3">
                                                <div class="card">
                                                    <div class="card-header p-0">
                                                        <button data-target="#" class="btn btn-light btn-block text-left p-3 rounded-0" data-role="none" data-toggle="collapse">
                                                            <b>Mobile Number</b>
                                                        </button>
                                                    </div>
                                                    <div id="" class="collapse show">
                                                        <div class="card-body">
                                                            <div class="text-right mb-3">
                                                                <button id="btnCRMAddPersonMobile" data-role="none" class="btn btn-success">
                                                                    Add Mobile
                                                                </button>
                                                            </div>

                                                            <ul data-role="none" id="lstPersonMobile" class="list-group"></ul>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="accordion mb-3">
                                                <div class="card">
                                                    <div class="card-header p-0">
                                                        <button data-target="#" class="btn btn-light btn-block text-left p-3 rounded-0" data-role="none" data-toggle="collapse">
                                                            <b>Email Address</b>
                                                        </button>
                                                    </div>
                                                    <div id="" class="collapse show">
                                                        <div class="card-body">
                                                            <div class="text-right mb-3">
                                                                <button id="btnCRMAddPersonEmail" data-role="none" class="btn btn-success">
                                                                    Add Email Address
                                                                </button>
                                                            </div>

                                                            <ul data-role="none" id="lstPersonEmail" class="list-group"></ul>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>

                                        <div id="tabCRMPersonEducation" class="tab-pane fade">
                                            <div class="accordion mb-3">
                                                <div class="card">
                                                    <div class="card-header p-0">
                                                        <button data-target="#" class="btn btn-light btn-block text-left p-3 rounded-0" data-role="none" data-toggle="collapse">
                                                            <b>Elementary</b>
                                                        </button>
                                                    </div>
                                                    <div id="" class="collapse show">
                                                        <div class="card-body">
                                                            <div class="text-right mb-3">
                                                                <button id="btnCRMAddElementary" data-role="none" class="btn btn-success">
                                                                    Add School
                                                                </button>
                                                            </div>

                                                            <ul data-role="none" id="lstElementary" class="list-group"></ul>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="accordion mb-3">
                                                <div class="card">
                                                    <div class="card-header p-0">
                                                        <button data-target="#" class="btn btn-light btn-block text-left p-3 rounded-0" data-role="none" data-toggle="collapse">
                                                            <b>High School</b>
                                                        </button>
                                                    </div>
                                                    <div id="" class="collapse show">
                                                        <div class="card-body">
                                                            <div class="text-right mb-3">
                                                                <button id="btnCRMAddHs" data-role="none" class="btn btn-success">
                                                                    Add School
                                                                </button>
                                                            </div>

                                                            <ul data-role="none" id="lstHs" class="list-group"></ul>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="accordion mb-3">
                                                <div class="card">
                                                    <div class="card-header p-0">
                                                        <button data-target="#" class="btn btn-light btn-block text-left p-3 rounded-0" data-role="none" data-toggle="collapse">
                                                            <b>College</b>
                                                        </button>
                                                    </div>
                                                    <div id="" class="collapse show">
                                                        <div class="card-body">
                                                            <div class="text-right mb-3">
                                                                <button id="btnCRMAddCollege" data-role="none" class="btn btn-success">
                                                                    Add School
                                                                </button>
                                                            </div>

                                                            <ul data-role="none" id="lstCollege" class="list-group"></ul>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>                                    

                                    <div id="divProspectPersonMainButton" class="fixed-bottom text-center pb-3" style="display:block; left:50%; transform: translate(-50%, 0); right:auto">
                                        <button data-role="none" id="btnCRMCancelPersonAddingContact" class="btn btn-secondary btn-lg">
                                            Back
                                        </button>
                                        <button data-role="none" id="btnCRMSavePersonAddingContact" class="btn btn-success btn-lg">
                                            Save
                                        </button>
                                    </div>
                                </div>
                            </div>

                            <div class="tab-pane fade" id="pageCRMColdCall">
                                <div id="divMainColdCal" subpage="coldcall">
                                    <div data-role="content">
                                        <div class="row">
                                            <div class="col-md-2">
                                                <div class="mb-3">
                                                    <label for="txtCallScript">Call Duration</label>
                                                    <div style="font-size:25px;">
                                                        <span id="sw_h">00</span>:
                                                        <span id="sw_m">00</span>:
                                                        <span id="sw_s">00</span>:
                                                        <span id="sw_ms">00</span>
                                                    </div>
                                                    <div>
                                                        <button data-role="none" id="btnCRMStartColdCall" title="Start Call" class="btn btn-success">
                                                            <span class="bi-telephone-fill"></span>
                                                        </button>
                                                        <button data-role="none" id="btnCRMEndColdCall" title="End Call" class="btn btn-danger">
                                                            <span class="bi-telephone-x-fill"></span>
                                                        </button>
                                                    </div>
                                                </div>

                                                <div id="divQrCodeTel" class="mb-3">
                                                    <img class="img-qrcode" />
                                                </div>

                                                <b id="txtCRMCPName"></b>
                                                <ol id="lstContactDetails" style="margin: 0 auto; padding-left:25px; list-style-type:decimal;"></ol>
                                            </div>
                                            <div class="col-md">
                                                <div class="row mb-3">
                                                    <label class="col-md-2">Client Name:</label>
                                                    <div class="col-md txtActionClientName" id="btnCRMCCName" style="font-weight:bold; text-transform:capitalize; cursor:pointer;"></div>
                                                </div>

                                                <div class="row mb-3" id="divColdCallContactPerson">
                                                    <label class="col-form-label col-md-2" for="txtCRMProspectCompanyFounding">Contact Person:</label>
                                                    <div class="col-md">
                                                        <ol id="lstContactPerson" style="margin: 0 auto; padding-left:20px; list-style-type:decimal;"></ol>
                                                    </div>
                                                </div>

                                                <div class="row mb-3">
                                                    <label class="col-form-label col-md-2" for="selCRMCallScript">Choose Script:</label>
                                                    <div class="col-md">
                                                        <select id="selCRMCallScript" data-role="none" class="form-select">
                                                            <option>Please Select</option>
                                                        </select>
                                                    </div>
                                                </div>

                                                <div class="row mb-3">
                                                    <label class="col-form-label col-md-2" for="txtCallScript">Call Script:</label>
                                                    <div class="col-md">
                                                        <textarea data-role="none" class="form-control" id="txtCallScript" style="max-height:200px; min-height:100px;"></textarea>
                                                    </div>
                                                </div>

                                                <div class="row mb-3">
                                                    <label class="col-form-label col-md-2" for="txtCallScript">File Under:</label>
                                                    <div class="col-md">
                                                        <select data-role="none" class="form-select fixed" id="selCRMTagInCall">
                                                            <option value="General">General</option>
                                                            <option value="Issue Ticket">Issue Ticket</option>
                                                            <option value="Opportunities">Opportunities</option>
                                                        </select>
                                                        <span id="lblTaggedAsInCall"></span>
                                                    </div>
                                                </div>

                                                <div class="row mb-3">
                                                    <label class="col-form-label col-md-2" for="txtCRMCallNotes">Call Notes:</label>
                                                    <div class="col-md">
                                                        <textarea data-role="none" class="form-control" id="txtCRMCallNotes" style="min-height:100px;"></textarea>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="fixed-bottom text-center pb-3" style="display:block; left:50%; transform: translate(-50%, 0); right:auto">
                                            <div>
                                                <button data-role="none" id="btnCRMCancelColdCall" data-title="Back" class="btn btn-secondary btn-lg">
                                                    Back
                                                </button>
                                                @*<button data-role="none" id="btnCRMOptionColdCall" data-title="More Option" class="btn custom-btn-theme-e btnCircle btnShadow">
                                <img alt="Option" src="/Content/images/More_Options.png" style="width:18px;" />
                            </button>*@
                                                <button data-role="none" id="btnCRMSaveColdCall" data-title="Save" class="btn btn-success btn-lg">
                                                    Save
                                                </button>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <div class="tab-pane fade" id="pageCRMOrderHistory">

                            </div>

                            <div class="tab-pane fade" id="pageCRMInteractionHistory">
                                <div>
                                    <div data-role="content">
                                        <div class="row mb-3">
                                            <label class="col-md-2" for="">Client Name:</label>
                                            <div class="col-md txtActionClientName" style="font-weight:bold; text-transform:capitalize;"></div>
                                        </div>

                                        <div class="row mb-3">
                                            <label class="col-form-label col-md-2" for="selCRMFilterInteractionHistory">File Under:</label>
                                            <div class="col-md">
                                                <select data-role="none" class="form-select fixed" id="selCRMFilterInteractionHistory">
                                                    <optgroup label="Filter by Interaction Type">
                                                        <option value="All">All</option>
                                                        <option value="Call">Call</option>
                                                        <option value="Comment">Comment</option>
                                                        <option value="Note">Note</option>
                                                        <option value="Opportunity">Opportunity</option>
                                                        <option value="Ticket">Ticket</option>
                                                        <option value="Scheduled Action">Scheduled Action</option>
                                                    </optgroup>
                                                </select>
                                            </div>
                                        </div>

                                        <div style="overflow:auto;">
                                            <table id="tblDataTableInteractionHistory" data-role="none" style="display:table; width:100%" class="table table-light table-striped table-hover">
                                                <thead>
                                                    <tr class="custom-theme-f">
                                                        <th data-sort="accounting" style="cursor:pointer;"> # </th>
                                                        <th data-sort="date" style="cursor:pointer;"> Interaction Date/Time </th>
                                                        <th data-sort="string" style="cursor:pointer;"> Interaction Type </th>
                                                        <th data-sort="string" style="cursor:pointer;"> Interacted With </th>
                                                        @*<th data-sort="string" style="cursor:pointer;"> Interaction Duration</th>*@
                                                    </tr>
                                                </thead>
                                                <tbody id="tblInteractionHistory"></tbody>
                                            </table>
                                        </div>
                                    </div>
                                </div>

                                <div class="fixed-bottom text-center pb-3" style="display:block; left:50%; transform: translate(-50%, 0); right:auto">
                                    <div>
                                        <button data-role="none" id="btnCRMInteractionHistoryBack" title="Back" class="btn btn-secondary btn-lg">
                                            Back
                                        </button>
                                    </div>
                                </div>
                            </div>

                            <div class="tab-pane fade" id="pageCRMToDo">
                                <div data-role="content">
                                    <input type="text" data-role="none" class="form-control d-none" id="txtCRMToDoID" />
                                    <div class="row mb-3">
                                        <label class="col-md-2" for="">Client Name:</label>
                                        <div class="col-md txtActionClientName" style="font-weight:bold; text-transform:capitalize;"></div>
                                    </div>

                                    <div class="mb-3" style="border:1px solid #dedede; padding:8px;">
                                        <div class="row mb-3">
                                            <label class="col-form-label col-md-2" for="selCRMTagInTodo">File Under:</label>
                                            <div class="col-md">
                                                <select data-role="none" class="form-select fixed mb-3" id="selCRMTagInTodo">
                                                    <option value="General">General</option>
                                                    <option value="Issue Ticket">Issue Ticket</option>
                                                    <option value="Opportunities">Opportunities</option>
                                                </select>
                                                <span id="lblTaggedAsInTodo"></span>
                                            </div>
                                        </div>
                                        <div class="row mb-3">
                                            <div class="col-md">
                                                <input type="text" data-role="none" class="form-control" id="txtCRMToDo" placeholder="To-Do Name" />
                                            </div>
                                            <div class="col-md-auto">
                                                <button data-role="none" id="btnCRMToDoSetDate" title="Set Date" class="btn btn-success">
                                                    Set Date
                                                </button>
                                                <button data-role="none" id="btnCRMOkToDo" title="Add" class="btn btn-success">
                                                    Add To Do
                                                </button>
                                            </div>
                                        </div>
                                    </div>
                                    
                                    <div class="row mb-3 align-items-center">
                                        <label class="col-form-label col-md-2" for="selCRMFilterTagInTodo">Filtered by:</label>
                                        <div class="col-md-auto">
                                            <select data-role="none" class="form-select fixed" id="selCRMFilterTagInTodo">
                                                <option value="All">All</option>
                                                <option value="General">General</option>
                                                <option value="Issue Ticket">Issue Ticket</option>
                                                <option value="Opportunities">Opportunities</option>
                                            </select>
                                            <span id="lblTaggedAsInTodo"></span>
                                        </div>
                                        <div class="col-md-auto">
                                            <a data-role="none" class="custom-link" id="lblFilterTagInTodo"></a>
                                        </div>
                                    </div>

                                    <ul id="listCRMToDo" data-role="none" style="list-style-type: none; margin:0 auto; padding:0;"></ul>

                                    <br /><br />

                                    <div class="emptyItem" style="display:none;">

                                    </div>

                                    <div class="fixed-bottom text-center pb-3" style="display:block; left:50%; transform: translate(-50%, 0); right:auto">
                                        <button data-role="none" id="btnCRMBackToDo" title="Back" class="btn btn-secondary btn-lg">
                                            Back
                                        </button>
                                    </div>
                                </div>
                            </div>

                            <div class="tab-pane fade" id="pageCRMSMS">
                                <div data-role="content">
                                    <div class="row mb-3">
                                        <label class="col-md-2" for="">Recipients:</label>
                                        <div class="col-md">
                                            <span style="font-weight:bold; text-transform:capitalize;" class="txtActionClientName mb-3"></span>
                                            <div id="listCRMofManualRecipient" style="max-height: 150px; overflow:auto;"></div>
                                        </div>
                                    </div>

                                    <span id="charNum2" class="mb-3"></span>

                                    <textarea data-role="none" class="form-control" id="txtCRMSMSMessageAction" max-length="160"></textarea>

                                    <div class="fixed-bottom text-center pb-3" style="display:block; left:50%; transform: translate(-50%, 0); right:auto">
                                        <div>
                                            <button data-role="none" id="btnCRMCancelSendSMS" data-title="Back" class="btn btn-secondary btn-lg">
                                                Back
                                            </button>
                                            <button data-role="none" id="btnCRMActionSendSMS" data-title="Save" class="btn btn-success btn-lg">
                                                Send
                                            </button>
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <div class="tab-pane fade" id="pageCRMTakeANote">
                                <div data-role="content">
                                    <div class="row mb-3">
                                        <label class="col-md-2" for="">Client Name:</label>
                                        <div class="col-md txtActionClientName" style="font-weight:bold; text-transform:capitalize;"></div>
                                    </div>

                                    <div subpage="takeanote" id="pageCRMListOfNote" style="display:block;">
                                        <div class="row mb-3 align-items-center">
                                            <label class="col-form-label col-md-2" for="selCRMFilterTagInNotes">Filtered by:</label>
                                            <div class="col-md-auto">
                                                <select data-role="none" class="form-select fixed" id="selCRMFilterTagInNotes">
                                                    <option value="All">All</option>
                                                    <option value="General">General</option>
                                                    <option value="Issue Ticket">Issue Ticket</option>
                                                    <option value="Opportunities">Opportunities</option>
                                                </select>
                                            </div>
                                            <div class="col-md">
                                                <input data-role="none" type="search" class="form-control" id="txtSearchCRMNotes" placeholder="Search here" />
                                            </div>
                                            <div class="col-md-auto">
                                                <a data-role="none" class="custom-link" id="lblFilterTagInNotes"></a>
                                            </div>
                                            <div class="col-md-auto text-right">
                                                <button data-role="none" id="btnCRMAddNote" title="Add" class="btn btn-success">
                                                    Add Note
                                                </button>
                                            </div>
                                        </div>

                                        <ul id="lstTakenNotes" class="list-group" style="list-style-type: none; margin:0 auto; padding:0;"></ul>

                                        <div class="fixed-bottom text-center pb-3" style="display:block; left:50%; transform: translate(-50%, 0); right:auto">
                                            <button data-role="none" id="btnCRMBackTakeANote" title="Back" class="btn btn-secondary btn-lg">
                                                Back
                                            </button>
                                        </div>
                                    </div>

                                    <div subpage="takeanote" id="pageCRMAddNote" style="display:none;">
                                        <form method="post" enctype="multipart/form-data" id="formNotePic">
                                            <input data-role="none" type="file" accept="image/*" name="fileNotePic" id="fileNotePic" style="margin-left:-1500px; position:absolute;">
                                        </form>
                                        
                                        <div class="row mb-3 align-items-center" id="divNoteTag">
                                            <label class="col-form-label col-md-2" for="selCRMTagInNote">File Under:</label>
                                            <div class="col-md-auto">
                                                <select data-role="none" class="form-select fixed" id="selCRMTagInNote">
                                                    <option value="All">All</option>
                                                    <option value="General">General</option>
                                                    <option value="Issue Ticket">Issue Ticket</option>
                                                    <option value="Opportunities">Opportunities</option>
                                                </select>
                                            </div>
                                            <div class="col-md-auto">
                                                <span id="lblTaggedAsInNote"></span>
                                            </div>
                                            <div class="col-md text-right">
                                                <button data-role="none" class="btn btn-success btnNoteIssueTicket">Issue Ticket</button>
                                                <button data-role="none" class="btn btn-success btnNoteAttachImage">Attach Image</button>
                                            </div>
                                        </div>

                                        <div id="txtCRMTakeANote" contenteditable="" style="min-height:250px; max-height:250px;" placeholder="Type the note here ..."></div>
                                        <ul id="lstIssueTickets" class="custom-list"></ul>
                                        <div class="fixed-bottom text-center pb-3" style="display:block; left:50%; transform: translate(-50%, 0); right:auto">
                                            <div>
                                                <button data-role="none" id="btnCRMCancelTakeANote" data-title="Back" class="btn btn-secondary btn-lg">
                                                    Back
                                                </button>
                                                <button data-role="none" id="btnCRMSaveTakeANote" data-title="Save" class="btn btn-success btn-lg">
                                                    Save
                                                </button>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <div class="tab-pane fade" id="pageCRMScheduleAnAction">
                                <div data-role="content">
                                    <div class="row mb-3">
                                        <label class="col-md-2" for="">Client Name:</label>
                                        <div class="col-md txtActionClientName" style="font-weight:bold; text-transform:capitalize;"></div>
                                    </div>

                                    <div subpage="schedanaction" id="pageCRMSchedule" style="display:block;">
                                        <div id="divCalendar" style="width:100%;"></div>

                                        <div class="fixed-bottom text-center pb-3" style="display:block; left:50%; transform: translate(-50%, 0); right:auto">
                                            <button data-role="none" id="btnCRMBackCalendar" title="Back" class="btn btn-secondary btn-lg">
                                                Back
                                            </button>
                                        </div>
                                    </div>

                                    <div subpage="schedanaction" id="pageCRMNewSched" style="display:none;">
                                        <input data-role="none" type="text" id="txtCRMSchedAnActionID" style="display:none;" />
                                        <div class="row mb-3">
                                            <label class="col-md-2" for="selCRMSchedAnAction">Next Action:</label>
                                            <div class="col-md">
                                                <select id="selCRMSchedAnAction" data-role="none" class="form-select">
                                                    <option value="Call">Call</option>
                                                    <option value="Meeting">Meeting</option>
                                                    @*<option value="Physical Meeting">Physical Meeting</option>
            <option value="Virtual Meeting">Virtual Meeting</option>*@
                                                    <option value="Email">Email</option>
                                                    <option value="SMS">SMS</option>
                                                </select>
                                            </div>
                                        </div>

                                        <div class="row mb-3">
                                            <label class="col-md-2" for="txtCRMSchedAnActionSubject">Subject: <span style="color:#F00;">*</span></label>
                                            <div class="col-md">
                                                <input type="text" data-role="none" class="form-control" id="txtCRMSchedAnActionSubject" />
                                            </div>
                                        </div>

                                        <div class="row mb-3">
                                            <label class="col-md-2" for="">Time:</label>
                                            <div class="col-md">
                                                <div class="form-check mb-3">
                                                    <label class="form-check-label">
                                                        <input type="checkbox" name="chbAllDay" data-role="none" class="form-check-input" />
                                                        All Day
                                                    </label>
                                                </div>
                                                <div class="row mb-3">
                                                    <div class="col-md-1">From:</div>
                                                    <div class="col-md-3">
                                                        <div class="input-group">
                                                            <input type="text" data-role="none" class="form-control" id="txtCRMDateTimeFrom" readonly />
                                                            <button data-role="none" class="btn btn-secondary" id="btnCRMDateTimeFrom">1:00 AM</button>
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="row mb-3">
                                                    <div class="col-md-1">To:</div>
                                                    <div class="col-md-3">
                                                        <div class="input-group">
                                                            <input type="text" data-role="none" class="form-control" id="txtCRMDateTimeTo" readonly />
                                                            <button data-role="none" class="btn btn-secondary" id="btnCRMDateTimeTo">2:00 AM</button>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="row mb-3">
                                            <label class="col-md-2" for="txtCRMNextActionLocation">Location:</label>
                                            <div class="col-md">
                                                <input type="text" data-role="none" class="form-control" id="txtCRMNextActionLocation" />
                                            </div>
                                        </div>

                                        <div class="row mb-3">
                                            <label class="col-md-2" for="txtCRMNextActionNotes">Notes:</label>
                                            <div class="col-md">
                                                <textarea data-role="none" class="form-control" style="resize:none; height:150px;" id="txtCRMNextActionNotes"></textarea>
                                            </div>
                                        </div>

                                        <div class="row mb-3">
                                            <label class="col-md-2" for="selCRMNextActionTeam">Assign To:</label>
                                            <div class="col-md">
                                                <select id="selCRMNextActionTeam" data-role="none" class="form-select">
                                                    <option>Select Team</option>
                                                </select>
                                            </div>
                                        </div>

                                        <div class="fixed-bottom text-center pb-3" style="display:block; left:50%; transform: translate(-50%, 0); right:auto">
                                            <div>
                                                <button data-role="none" id="btnCRMBackSchedAnAction" data-title="Back" class="btn btn-secondary btn-lg">
                                                    Back
                                                </button>
                                                <button data-role="none" id="btnCRMSaveSchedAnAction" data-title="Save" class="btn btn-success btn-lg">
                                                    Save
                                                </button>
                                            </div>
                                        </div>
                                    </div>

                                    <div subpage="schedanaction" id="pageCRMListofEvents" style="display:block;">
                                        <div class="text-right mb-3">
                                            <button data-role="none" id="btnCRMAddSched" title="Add" class="btn btn-success">
                                                Add Schedule
                                            </button>
                                        </div>

                                        <ul id="lstEvents" data-role="none" style="list-style-type: none; margin:0 auto; padding:0;"></ul>

                                        <div class="fixed-bottom text-center pb-3" style="display:block; left:50%; transform: translate(-50%, 0); right:auto">
                                            <div>
                                                <button data-role="none" id="btnCRMBackListOfEvents" data-title="Back" class="btn btn-secondary btn-lg">
                                                    Back
                                                </button>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <div class="tab-pane fade" id="pageCRMTickets">
                                <div data-role="content">
                                    <div class="row mb-3">
                                        <label class="col-md-2" for="">Client Name:</label>
                                        <div class="col-md txtActionClientName" style="font-weight:bold; text-transform:capitalize;"></div>
                                    </div>

                                    <div class="row mb-3 align-items-center">
                                        <label class="col-form-label col-md-2" for="selFilterClientTicket">Filtered by:</label>
                                        <div class="col-md-auto">
                                            <select data-role="none" class="form-select fixed" id="selFilterClientTicket">
                                                <option value="All">All</option>
                                                <option value="Unresolved">Unresolved</option>
                                                <option value="Resolved">Resolved</option>
                                                <option value="Cancelled">Cancelled</option>
                                            </select>
                                        </div>
                                        <div class="col-md text-right">
                                            <button data-role="none" title="Add" class="btn btn-success btnNoteIssueTicket">
                                                Add Ticket
                                            </button>
                                        </div>
                                    </div>

                                    <div style="overflow:auto;">
                                        <table id="tblDataTableTicketsByClient" data-role="none" style="display:table; width:100%" class="table table-light table-striped table-hover">
                                            <thead>
                                                <tr class="custom-theme-f">
                                                    <th style="cursor:pointer;"> Ticket Number </th>
                                                    <th style="cursor:pointer;"> Subject </th>
                                                    <th style="cursor:pointer;"> FOWST </th>
                                                    <th style="cursor:pointer;"> Remarks </th>
                                                    <th style="cursor:pointer;"> Status </th>
                                                    <th style="cursor:pointer;"> Due Date </th>
                                                    <th style="cursor:pointer;"> Date Opened </th>
                                                    <th style="cursor:pointer;"> Date Closed </th>
                                                    <th style="cursor:pointer;"> Assigned To </th>
                                                    <th style="cursor:pointer;"> Encoded By </th>
                                                    <th style="cursor:pointer;"> Action </th>
                                                </tr>
                                            </thead>
                                            <tbody id="tbodyTickets"></tbody>
                                        </table>
                                    </div>
                                </div>
                            </div>

                            <div class="tab-pane fade" id="pageCRMOpportunity">
                                <div data-role="content">
                                    <div class="row mb-3">
                                        <label class="col-md-2" for="">Client Name:</label>
                                        <div class="col-md txtActionClientName" style="font-weight:bold; text-transform:capitalize;"></div>
                                    </div>

                                    <div class="mb-3 txt-right">
                                        <button data-role="none" id="btnCRMNewOpportunity" title="Add" class="btn btn-success">
                                            Add Opportunity
                                        </button>
                                    </div>

                                    <div class="table-responsive">
                                        <table id="tblDataTableClientOpportunity" data-role="none" style="display:table; width:100%" class="table table-light table-striped table-hover">
                                            <thead>
                                                <tr class="custom-theme-f">
                                                    <th style="width:20px !important;"> # </th>
                                                    <th style="cursor:pointer;"> Name </th>
                                                    <th style="cursor:pointer;"> Current Stage </th>
                                                    <th style="cursor:pointer;"> Status </th>
                                                    <th style="cursor:pointer;"> Size </th>
                                                    <th style="cursor:pointer;"> Last Updated </th>
                                                    <th></th>
                                                </tr>
                                            </thead>
                                            <tbody id="tbodyClientOpportunity"></tbody>
                                        </table>
                                    </div>
                                </div>
                            </div>

                            <div class="tab-pane fade" id="pageCRMSurveys">
                                <div class="row mb-3">
                                    <label class="col-md-2" for="">Client Name:</label>
                                    <div class="col-md txtActionClientName" style="font-weight:bold; text-transform:capitalize;"></div>
                                </div>

                                <div class="mb-3 txt-right">
                                    <button data-role="none" id="btnCRMTakeSurvey" title="Add" class="btn btn-success">
                                        Add Survey
                                    </button>
                                </div>

                                <div class="table-responsive">
                                    <table id="tblDataTableClientSurvey" data-role="none" style="display:table; width:100%" class="table table-light table-striped table-hover">
                                        <thead>
                                            <tr class="custom-theme-f">
                                                <th style="width:20px !important;"> # </th>
                                                <th style="cursor:pointer;"> Questionaire </th>
                                                <th style="cursor:pointer; width:100px" "> Score </th>
                                                <th style="cursor:pointer; width:150px"> Date Responded </th>
                                            </tr>
                                        </thead>
                                        <tbody id="tbodyClientSurvey"></tbody>
                                    </table>
                                </div>
                            </div>

                            <div class="tab-pane fade" id="pageCRMFile">
                                <div class="row mb-3">
                                    <label class="col-md-2" for="">Client Name:</label>
                                    <div class="col-md txtActionClientName" style="font-weight:bold; text-transform:capitalize;"></div>
                                </div>

                                <div class="mb-3 txt-right">
                                    <button data-role="none" id="btnCRMUploadFileByClient" title="Add" class="btn btn-success">
                                        Upload File
                                    </button>
                                    <input data-role="none" type="file" id="fileUploadByClient" accept=".xlsx,.xls,image/*,.doc, .docx,.ppt, .pptx,.txt,.pdf" multiple style="display:none;" />
                                </div>

                                <div class="table-responsive">
                                    <table id="tblDataTableFile" data-role="none" style="display:table; width:100%" class="table table-light table-striped table-hover">
                                        <thead>
                                            <tr class="custom-theme-f">
                                                <th style="width:20px !important;"> # </th>
                                                <th style="cursor:pointer;"> File Name </th>
                                                <th style="cursor:pointer; width:150px"> Date Uploaded </th>
                                                <th style="cursor:pointer; width:50px"> </th>
                                            </tr>
                                        </thead>
                                        <tbody></tbody>
                                    </table>
                                </div>                                
                            </div>

                            <div class="tab-pane fade" id="pageCRMInternalSalesMarketing">
                                <div data-role="content">
                                    Internal Sales And Marketing Info
                                </div>
                            </div>
                        </div>                        
                    </div>
                </div>
            </div>
        </div>

        <div style="clear:both;"></div>
    </div>

    <div id="divTicketToPrint" style="display:none;">
        <table style="width:100%;">
            <tr>
                <td>
                    <span class="lblPrintTicketOrgName" style="font-size:20px; font-weight:bold;"></span><br />
                    <span class="lblPrintTicketOrgAddress" style="font-size:14px;"></span><br />
                    <span class="lblPrintTicketOrgContacts" style="font-size:14px;"></span>
                </td>
                <td style="text-align:right; width:250px;">
                    <span style="font-size:20px;">CUSTOMER'S COPY</span><br />
                    <span style="font-size:14px;">(Please surrender this ticket upon claiming your product.)</span><br />
                </td>
            </tr>
        </table>
        <br />
        <table style="width:100%; table-layout:fixed;">
            <tr>
                <td style="width:130px; font-size:12px; vertical-align:top;">
                    Ticket Number:
                </td>
                <td class="lblPrintTicketNumber" style="font-weight:bold; font-size:12px; vertical-align:top;"></td>
                <td style="width:130px; font-size:12px; vertical-align:top;">
                    Ticket Assigned To:
                </td>
                <td class="lblPrintTicketAssigned" style="font-weight:bold; font-size:12px; vertical-align:top;"></td>
            </tr>

            <tr>
                <td style="width:130px; font-size:12px; vertical-align:top;">
                    Ticket Type:
                </td>
                <td class="lblPrintTicketType" style="font-weight:bold; font-size:12px; vertical-align:top;"></td>
                <td style="width:130px; font-size:12px; vertical-align:top;">
                    Ticket Due Date:
                </td>
                <td class="lblPrintTicketDueDate" style="font-weight:bold; font-size:12px; vertical-align:top;"></td>
            </tr>

            <tr>
                <td style="width:130px; font-size:12px; vertical-align:top;">
                    Re Product/Service:
                </td>
                <td class="lblPrintTicketProductServices" style="font-weight:bold; font-size:12px; vertical-align:top;"></td>
                <td style="width:130px; font-size:12px; vertical-align:top;">
                    Recommended Action:
                </td>
                <td class="lblPrintTicketRecAction" style="font-weight:bold; font-size:12px; vertical-align:top;"></td>
            </tr>

            <tr>
                <td style="width:130px; font-size:12px; vertical-align:top;">
                    Client Name:
                </td>
                <td class="lblPrintTicketClientName" style="font-weight:bold; font-size:12px; vertical-align:top;"></td>
                <td style="width:130px; font-size:12px; vertical-align:top;">
                    Address to service in case of visit:
                </td>
                <td class="lblPrintTicketAddress" style="font-weight:bold; font-size:12px; vertical-align:top;"></td>
            </tr>

            <tr>
                <td style="width:130px; font-size:12px; vertical-align:top;">
                    Client Contact:
                </td>
                <td class="lblPrintTicketClientContact" style="font-weight:bold; font-size:12px; vertical-align:top;"></td>
                <td style="width:130px; font-size:12px; vertical-align:top;">
                    Best day to service:
                </td>
                <td class="lblPrintTicketDays" style="font-weight:bold; font-size:12px; vertical-align:top;"></td>
            </tr>

            <tr>
                <td style="width:130px; font-size:12px; vertical-align:top;">
                    Contact Number:
                </td>
                <td class="lblPrintTicketContactNumber" style="font-weight:bold; font-size:12px; vertical-align:top;"></td>
                <td style="width:130px; font-size:12px; vertical-align:top;">
                    Best time to service:
                </td>
                <td class="lblPrintTicketTimes" style="font-weight:bold; font-size:12px; vertical-align:top;"></td>
            </tr>

            <tr>
                <td style="width:130px; font-size:12px; vertical-align:top;">
                    Ticket Issued by:
                </td>
                <td class="lblPrintTicketIssuedBy" style="font-weight:bold; font-size:12px; vertical-align:top;"></td>
                <td style="width:130px; font-size:12px; vertical-align:top;">
                    Due Date:
                </td>
                <td class="lblPrintTicketDueDate" style="font-weight:bold; font-size:12px; vertical-align:top;"></td>
            </tr>

            <tr>
                <td style="width:130px; font-size:12px; vertical-align:top;">
                    Ticket Issued on:
                </td>
                <td class="lblPrintTicketIssuedOn" style="font-weight:bold; font-size:12px; vertical-align:top;"></td>
                <td style="width:130px; font-size:12px; vertical-align:top;">
                    Remarks:
                </td>
                <td class="lblPrintTicketRemarks" rowspan="2" style="font-size:12px; vertical-align:top;"></td>
            </tr>

            <tr>
                <td class="lblPrintTicketNotes" colspan="2" rowspan="2" style="font-size:12px; vertical-align:top; border-top:1px solid #000;"></td>
            </tr>

            <tr>
                <td style="font-size:12px; vertical-align:top; text-align:center;">
                    <br /><br /><br /><br />
                    Received Item(s) by:<br />
                    (Serv. Representative)<br /><br />
                    ____________<br />
                    Name and Signature
                </td>

                <td style="font-size:12px; vertical-align:top; text-align:center;">
                    <br /><br /><br /><br />
                    Released Item(s) to:<br />
                    (Client)<br /><br />
                    ____________<br />
                    Name and Signature
                </td>
            </tr>
        </table>
        <br />
        <hr style="border: none; border-top: 3px dotted #000;" />
        <br />
        <table style="width:100%;">
            <tr>
                <td>
                    <span class="lblPrintTicketOrgName" style="font-size:20px; font-weight:bold;"></span><br />
                    <span class="lblPrintTicketOrgAddress" style="font-size:14px;"></span><br />
                    <span class="lblPrintTicketOrgContacts" style="font-size:14px;"></span>
                </td>
                <td style="text-align:right; width:250px;">
                    <span style="font-size:20px;">COMPANY'S COPY</span><br />
                </td>
            </tr>
        </table>
        <br />
        <table style="width:100%; table-layout:fixed;">
            <tr>
                <td style="width:130px; font-size:12px; vertical-align:top;">
                    Ticket Number:
                </td>
                <td class="lblPrintTicketNumber" style="font-weight:bold; font-size:12px; vertical-align:top;"></td>
                <td style="width:130px; font-size:12px; vertical-align:top;">
                    Ticket Assigned To:
                </td>
                <td class="lblPrintTicketAssigned" style="font-weight:bold; font-size:12px; vertical-align:top;"></td>
            </tr>

            <tr>
                <td style="width:130px; font-size:12px; vertical-align:top;">
                    Ticket Type:
                </td>
                <td class="lblPrintTicketType" style="font-weight:bold; font-size:12px; vertical-align:top;"></td>
                <td style="width:130px; font-size:12px; vertical-align:top;">
                    Ticket Due Date:
                </td>
                <td class="lblPrintTicketDueDate" style="font-weight:bold; font-size:12px; vertical-align:top;"></td>
            </tr>

            <tr>
                <td style="width:130px; font-size:12px; vertical-align:top;">
                    Re Product/Service:
                </td>
                <td class="lblPrintTicketProductServices" style="font-weight:bold; font-size:12px; vertical-align:top;"></td>
                <td style="width:130px; font-size:12px; vertical-align:top;">
                    Recommended Action:
                </td>
                <td class="lblPrintTicketRecAction" style="font-weight:bold; font-size:12px; vertical-align:top;"></td>
            </tr>

            <tr>
                <td style="width:130px; font-size:12px; vertical-align:top;">
                    Client Name:
                </td>
                <td class="lblPrintTicketClientName" style="font-weight:bold; font-size:12px; vertical-align:top;"></td>
                <td style="width:130px; font-size:12px; vertical-align:top;">
                    Address to service in case of visit:
                </td>
                <td class="lblPrintTicketAddress" style="font-weight:bold; font-size:12px; vertical-align:top;"></td>
            </tr>

            <tr>
                <td style="width:130px; font-size:12px; vertical-align:top;">
                    Client Contact:
                </td>
                <td class="lblPrintTicketClientContact" style="font-weight:bold; font-size:12px; vertical-align:top;"></td>
                <td style="width:130px; font-size:12px; vertical-align:top;">
                    Best day to service:
                </td>
                <td class="lblPrintTicketDays" style="font-weight:bold; font-size:12px; vertical-align:top;"></td>
            </tr>

            <tr>
                <td style="width:130px; font-size:12px; vertical-align:top;">
                    Contact Number:
                </td>
                <td class="lblPrintTicketContactNumber" style="font-weight:bold; font-size:12px; vertical-align:top;"></td>
                <td style="width:130px; font-size:12px; vertical-align:top;">
                    Best time to service:
                </td>
                <td class="lblPrintTicketTimes" style="font-weight:bold; font-size:12px; vertical-align:top;"></td>
            </tr>

            <tr>
                <td style="width:130px; font-size:12px; vertical-align:top;">
                    Ticket Issued by:
                </td>
                <td class="lblPrintTicketIssuedBy" style="font-weight:bold; font-size:12px; vertical-align:top;"></td>
                <td style="width:130px; font-size:12px; vertical-align:top;">
                    Due Date:
                </td>
                <td class="lblPrintTicketDueDate" style="font-weight:bold; font-size:12px; vertical-align:top;"></td>
            </tr>

            <tr>
                <td style="width:130px; font-size:12px; vertical-align:top;">
                    Ticket Issued on:
                </td>
                <td class="lblPrintTicketIssuedOn" style="font-weight:bold; font-size:12px; vertical-align:top;"></td>
                <td style="width:130px; font-size:12px; vertical-align:top;">
                    Remarks:
                </td>
                <td class="lblPrintTicketRemarks" rowspan="2" style="font-size:12px; vertical-align:top;"></td>
            </tr>

            <tr>
                <td class="lblPrintTicketNotes" colspan="2" rowspan="2" style="font-size:12px; vertical-align:top; border-top:1px solid #000;"></td>
            </tr>

            <tr>
                <td style="font-size:12px; vertical-align:top; text-align:center;">
                    <br /><br /><br /><br />
                    Received Item(s) by:<br />
                    (Serv. Representative)<br /><br />
                    ____________<br />
                    Name and Signature
                </td>

                <td style="font-size:12px; vertical-align:top; text-align:center;">
                    <br /><br /><br /><br />
                    Released Item(s) to:<br />
                    (Client)<br /><br />
                    ____________<br />
                    Name and Signature
                </td>
            </tr>
        </table>
    </div>
</div>