@Code
    ViewData("Title") = "Project Management"
    Layout = "~/Views/Shared/BootstrapFiveLayout.vbhtml"
End Code
@Section head
    @*<link rel="stylesheet" href="/Content/css/jquery.jqplot.min.css" />
    <link rel="stylesheet" href="/Content/css/ui.dynatree.css" />
    <link rel="stylesheet" href="/Content/css/qunit.css" />
    <link rel="stylesheet" href="/Content/flatui-css-custom.css" />
    <link rel="stylesheet" href="/Content/metro-calendar.css" />*@

    <script>
        require([GetPathFormat('/Scripts/sources/BOMProjectExcel/BOMMain.js?crc=' + assetversion),
                    GetPathFormat('/Scripts/sources/BOMProjectExcel/BOMProjectExcel.js?crc=' + assetversion),
                    //GetPathFormat('/Scripts/sources/BOMProjectExcel/BOMSystemAdmin.js?crc=')
        ]);
        //require([GetPathFormat('/Scripts/sources/BOMProjectExcel/BOMSystemAdmin.js?crc=')]);
    </script>

    @*<script type="text/javascript" src="/Scripts/sources/SetupChecklist/SystemAdministrationQueries.js"></script>
    <script type="text/javascript" src="/Scripts/sources/SetupChecklist/SystemAdministrationView.js"></script>
    <script type="text/javascript" src="/Scripts/sources/SetupChecklist/NewPromptUser.js"></script>*@

    <style type="text/css">
        .tddraggable{
            cursor: move;
        }

        td.rowMaterials:before {
            content: "(M) ";
            float: left;
            color: #F44336;
        }

        #tBodyProjectBudget td.rowMaterials:before{
            margin-left: -60px;
            position: absolute;
        }

        .markascompleted{
            background-image: url(/Content/images/tick.png);
            background-repeat: no-repeat;
            margin-top:-20px;
            background-position: right;
        }

        /*.markasnotcompleted{
            background-image: url(/Content/images/cross.png);
            background-repeat: no-repeat;
            margin-top:-20px;
            background-position: right;
        }*/

        table .thFormat{
            text-align:center; 
            vertical-align:middle;
            color:#FFF; 
            text-shadow:none;
        }

        .project_new{
            background:url(/Content/images/project_new.png);
            background-repeat: no-repeat;
            display:inline-block;
            background-size: auto 48px;
            width:50px;
            height:50px;
        }

        .btn-trail:hover > .project_new{
            background:url(/Content/images/project_new-white.png);
            background-size: auto 48px;
            background-repeat: no-repeat;
        }

        .project_lookup{
            background:url(/Content/images/project_lookup.png);
            background-repeat: no-repeat;
            display:inline-block;
            background-size: auto 48px;
            width:50px;
            height:50px;
        }

        .btn-trail:hover > .project_lookup{
            background:url(/Content/images/project_lookup-white.png);
            background-size: auto 48px;
            background-repeat: no-repeat;
        }

        .project_edit{
            background:url(/Content/images/project_edit.png);
            background-repeat: no-repeat;
            display:inline-block;
            background-size: auto 48px;
            width:50px;
            height:50px;
        }

        .btn-trail:hover > .project_edit{
            background:url(/Content/images/project_edit-white.png);
            background-size: auto 48px;
            background-repeat: no-repeat;
        }

        .project_save{
            background:url(/Content/images/project_save.png);
            background-repeat: no-repeat;
            display:inline-block;
            background-size: auto 48px;
            width:50px;
            height:50px;
        }

        .btn-trail:hover > .project_save{
            background:url(/Content/images/project_save-white.png);
            background-size: auto 48px;
            background-repeat: no-repeat;
        }

        .project_cancel{
            background:url(/Content/images/project_cancel.png);
            background-repeat: no-repeat;
            display:inline-block;
            background-size: auto 48px;
            width:50px;
            height:50px;
        }

        .btn-trail:hover > .project_cancel{
            background:url(/Content/images/project_cancel-white.png);
            background-size: auto 48px;
            background-repeat: no-repeat;
        }

        #tBodyProjectBudget td,
        #tBodyExcelLike td{
            vertical-align:top;
        }

        /*.tdfreeze{
            position: absolute !important;
            width: 5em;
            left: 0;
            top: auto;
            border-top-width: 1px;
            margin-top: -1px;
        }

        .divProjectExcelLike{
            overflow-x: scroll; 
            margin-left: 25em; 
            overflow-y: visible; 
            padding: 0;
        }*/

        th.cell-timespend{
            background-color:#D2691E;
        }

        th.cell-materialspend{
            background-color:#A0522D;
        }

        th.cell-laborspend{
            background-color:#8B4513;
        }

        .table-scroll {
          position: relative;
          /*min-width: 980px;*/ /* just for testing fixed left column*/
          z-index: 1;
          margin: auto;
          overflow: scroll;
          max-height: 800px;
        }
        .table-scroll table {
          /*width: 100%;
          min-width: 1280px;*/
          margin: auto;
          border-collapse: separate;
          border-spacing: 0;
        }
        .table-scroll thead th {
          /*color: #fff;*/
          position: -webkit-sticky;
          position: sticky;
          top: 0;
            z-index:1;
        }
        /* safari and ios need the tfoot itself to be position:sticky also */
        .table-scroll tfoot,
        .table-scroll tfoot th,
        .table-scroll tfoot td {
          position: -webkit-sticky;
          position: sticky;
          bottom: 0;
          color: #fff;
          z-index:4;
        }/* testing links*/

        .table-scroll th:nth-child(1),
        .table-scroll td:nth-child(1) {
          position: -webkit-sticky;
          position: sticky;
          left: 0;
          z-index: 2;
        }

        .table-scroll th:nth-child(2),
        .table-scroll td:nth-child(2) {
          position: -webkit-sticky;
          position: sticky;
          left: 26px;
          z-index: 2;
        }

        .table-scroll th:nth-child(3),
        .table-scroll td:nth-child(3) {
          position: -webkit-sticky;
          position: sticky;
          left: 120px;
          z-index: 2;
        }

        .table-scroll thead th:nth-child(1),
        .table-scroll thead th:nth-child(2),
        .table-scroll thead th:nth-child(3),
        .table-scroll tfoot th:nth-child(1),
        .table-scroll tfoot th:nth-child(2),
        .table-scroll tfoot th:nth-child(3) {
          z-index: 5;
        }

        .activeCell {
            border: 2px solid #335F8E !important;   
        }

        .inputTextInExcel {
            border: none;
            box-sizing: border-box;
            -webkit-box-sizing:border-box;
            -moz-box-sizing: border-box;
            width: 100%;
            outline: none;
        }

        .hasAssociatedSKU
{
    background-image: url(data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAA/0lEQVQ4T2NkoBAwUqifYRAb4NXDMP3fP4aMf/8ZGJiA7mRiYpixrYQhE93LWL0A0iwpLJARYp3CwMbCxvDrzy+GNUfnMDx/+wHDEKwGeHYx/M8OSGO49WYHw6+/PxjYmDkY1EQ8GKZumMWwvQw13LAa4NHJ8D/QTZXh2cfbDP//MTAwMjEwSPGrMqzfdZthRzkRBngBXWBvC9QINB5mwH9gWBw8zMCwjRgXOFczrJFUYAg2MWZgYGFmYPjzl4HhzFkGhucPGNbubWUIQQ5IXOlADmhIHzs/QzDIZpBLfn4Eay4Can5EjAEgNXJALIqk+DW6ZpDcIE6JxGYyir0AAAoGShE1BKf6AAAAAElFTkSuQmCC);
    background-repeat: no-repeat;
    margin-top:-20px;
    background-position: right;
}

.preferredVendor
{
    background-color: #FDFFBB !important;
    border-bottom: 1px solid #FFE991 !important;   
}

.minPrice 
{
    background-color: #ACDEFD !important;
    border-bottom: 1px solid #91CBEF !important;
}

.preAssociatedSKU
{
    display:none;
    font-family: Helvetica, Arial, sans-serif;
    word-break: break-word;
    font-size: 12px;       
}

.rowSubtotal > td,
.rowSubtotal > td > div
{
    background-color: #CBCBCB !important;
    font-weight: bold !important;   
}

.rowSubtotal > td:nth-child(1) > div,
.rowSubtotal > td:nth-child(2)
{
    text-align:center !important;   
}

.TextCell
{
    text-align: center !important;   
}

.tableProjectBudget td input[type="text"]
{
    width: 100%;
    border: none;
    box-sizing: border-box;
    -webkit-box-sizing:border-box;
    -moz-box-sizing: border-box;
    padding: 5px;
    outline: none;   
}

.tableProjectBudget{
    border-collapse: collapse;
}

.tableProjectBudget td, 
.tableProjectBudget th{
    border: 1px solid #CCC;
}

.tableCell 
{
    cursor: cell;
}


.txtOnlyParent 
{
    text-transform: uppercase;
    word-break: break-word;
    font-weight: bold;
}

.txtOnlyChild 
{
    padding: 0px; 
}

    .context-menu {
    z-index:1000;
    position: absolute;
    background-color:#FAFAFA;
    border: 1px solid #CCC;
    box-shadow: 1px 2px 3px #ccc;
    font-family: Helvetica, Arial, sans-serif;
}

.context-menu ul
{
    list-style-type: none;
    padding: 0px;
}

.context-menu li 
{
    padding: 10px;
    cursor: pointer;
    border-left: 3px solid #FAFAFA;
}

.context-menu li:hover
{
    background-color: #EAEAEA;
    border-left: 3px solid #B3B3B3;
}

    .disableCell
{
    text-decoration: line-through;
    color: #676767;   
}

    .hasAssociatedSKU
{
    background-image: url(data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAA/0lEQVQ4T2NkoBAwUqifYRAb4NXDMP3fP4aMf/8ZGJiA7mRiYpixrYQhE93LWL0A0iwpLJARYp3CwMbCxvDrzy+GNUfnMDx/+wHDEKwGeHYx/M8OSGO49WYHw6+/PxjYmDkY1EQ8GKZumMWwvQw13LAa4NHJ8D/QTZXh2cfbDP//MTAwMjEwSPGrMqzfdZthRzkRBngBXWBvC9QINB5mwH9gWBw8zMCwjRgXOFczrJFUYAg2MWZgYGFmYPjzl4HhzFkGhucPGNbubWUIQQ5IXOlADmhIHzs/QzDIZpBLfn4Eay4Can5EjAEgNXJALIqk+DW6ZpDcIE6JxGYyir0AAAoGShE1BKf6AAAAAElFTkSuQmCC) !important;
    background-repeat: no-repeat;
    margin-top:-20px;
    background-position: right;
}
    </style>
End Section

@*Start Pop Ups*@
<div>
    <div id="popupNewProject" class="modal fade" data-backdrop="static" tabindex="-1">
        <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Project</h5>
                    <!--<button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>-->
                </div>
                <div class="modal-body">
                    <div id="divNewProjectDetails" style="display:block;">
                        <div style="font-size:12px;">
                            Fields marked with <span style="color:#F00">*</span> are required.
                        </div>

                        <h4>Project Details</h4>

                        <div style="display:none">
                            <select id="selFowstType"></select>
                            <input type="text" data-role="none" class="form-control" id="txtProjectID" />
                            <input data-role="none" type="text" id="txtProjectOwnerOrgPersonID" />
                            <input data-role="none" type="text" id="txtClientID" /> @*either Org or People*@
                        </div>

                        <div class="row mb-3">
                            <label class="col-md-3 col-form-label" for="btnClientName">Client: <span class="required">*</span></label>
                            <div class="col-md">
                                <button id="btnClientName" data-role="none" class="btn btn-secondary btn-block">
                                    <span id="txtClientName">Please Select</span>
                                </button>
                            </div>
                        </div>

                        @*dont display the fowst*@
                        <div class="row mb-3" style="display:none;">
                            <label class="col-md-3 col-form-label" for="selFowstLocation">FOWST Location: <br /> (Factory, Office, Warehouse, Store or Truck)</label>
                            <div class="col-md">
                                <select id="selFowstLocation" data-role="none" class="form-select">
                                    <option value="">Please Select</option>
                                </select>
                            </div>
                        </div>

                        <input type="text" data-role="none" class="form-control" id="txtFowstLocationID" style="display:none" />
                        <div class="row mb-3">
                            <label class="col-md-3 col-form-label" for="txtFowstLocation">Project Location Name:</label>
                            <div class="col-md">
                                <input type="text" data-role="none" class="form-control" id="txtFowstLocation" />
                            </div>
                        </div>

                        <div class="row mb-3">
                            <label class="col-md-3 col-form-label" for="txtProjectName">Project Name: <span style="color:#F00">*</span></label>
                            <div class="col-md">
                                <input type="text" data-role="none" class="form-control" id="txtProjectName" />
                            </div>
                        </div>

                        <div class="row mb-3">
                            <label class="col-md-3 col-form-label" for="txtProjectCode">Project Code: <span style="color:#F00">*</span></label>
                            <div class="col-md">
                                <input type="text" data-role="none" class="form-control" id="txtProjectCode" maxlength="30" />
                            </div>
                        </div>

                        <div class="row mb-3">
                            <label class="col-md-3 col-form-label" for="txtProjectDescription">Project Description:</label>
                            <div class="col-md">
                                <input type="text" data-role="none" class="form-control" id="txtProjectDescription" />
                            </div>
                        </div>
                        
                        <div class="accordion mb-3">
                            <div class="card">
                                <div class="card-header p-0">
                                    <button data-target="#" class="btn btn-light btn-block text-left p-3 rounded-0" data-role="none" data-toggle="collapse">
                                        <b>Complete Address</b>
                                    </button>
                                </div>
                                <div id="" class="collapse show">
                                    <div class="card-body">
                                        <div class="row mb-3">
                                            <label class="col-form-label col-md-3" for="txtOrgFOWSTBuildingNo2">House or Building No:</label>
                                            <div class="col-md">
                                                <input type="text" data-role="none" class="form-control" id="txtOrgFOWSTBuildingNo2" />
                                            </div>
                                        </div>

                                        <div class="row mb-3">
                                            <label class="col-form-label col-md-3" for="txtOrgFOWSTStreet2">Street: <span style="color: #FF0000;">*</span></label>
                                            <div class="col-md">
                                                <input type="text" data-role="none" class="form-control" id="txtOrgFOWSTStreet2" />
                                            </div>
                                        </div>

                                        <div style="display:none;"><input type="text" data-role="none" class="form-control" readonly id="txtOrgFOWSTCityID2" /></div>
                                        <div class="row mb-3">
                                            <label class="col-form-label col-md-3" for="btnOrgFOWSTCity2">City Or Town: <span style="color:#F00">*</span></label>
                                            <div class="col-md">
                                                <button id="btnOrgFOWSTCity2" data-role="none" class="btn btn-secondary btn-block">
                                                    <span id="txtCityName">Please Select</span>
                                                </button>
                                            </div>
                                        </div>

                                        <div style="display:none;"><input type="text" data-role="none" class="form-control" readonly id="txtOrgFOWSTCountryID2" /></div>
                                        <div class="row mb-3">
                                            <label class="col-form-label col-md-3" for="btnOrgFOWSTCountry2">Country: <span style="color:#F00">*</span></label>
                                            <div class="col-md">
                                                <button id="btnOrgFOWSTCountry2" data-role="none" class="btn btn-secondary btn-block">
                                                    <span id="txtCountryName">Please Select</span>
                                                </button>
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
                                        <b>Project Members and Roles <span style="color:#F00">*</span></b>
                                    </button>
                                </div>
                                <div id="" class="collapse show">
                                    <div class="card-body" id="divProjectMember" style="display:block;">
                                        <div class="mb-3 text-right">
                                            <button id="btnDefine" data-role="none" class="btn btn-success">
                                                Roles and Permissions
                                            </button>
                                            <button id="btnAddMember" data-role="none" class="btn btn-success">
                                                Add Member
                                            </button>
                                        </div>

                                        <div class="table-responsive">
                                            <table data-role="table" data-mode="none" style="width:100%; display:table;" class="table table-light table-striped table-hover" width="100%">
                                                <thead>
                                                    <tr class="custom-theme-c">
                                                        <th>Member Name</th>
                                                        <th>Role</th>
                                                        <th>Status</th>
                                                        <th></th>
                                                    </tr>
                                                </thead>
                                                <tbody id="tbodyProjectMember"></tbody>
                                            </table>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <h4 class="mb-3">Projected Date</h4>

                        <div class="row mb-3">
                            <label class="col-md-3 col-form-label" for="btnSetDate">Projected Start Date: <span style="color:#F00">*</span></label>
                            <div class="col-md">
                                <button data-role="none" class="btn btn-secondary btn-block btnSetDate" startdate="ProjStartDate" setdateto="ProjStartDate">
                                    <span id="ProjStartDate">Set Date</span>
                                </button>
                            </div>
                        </div>

                        <div class="row mb-3">
                            <label class="col-md-3 col-form-label" for="btnSetDate">Projected Completion Date: <span style="color:#F00">*</span></label>
                            <div class="col-md">
                                <button data-role="none" class="btn btn-secondary btn-block btnSetDate" startdate="ProjStartDate" setdateto="ProjCompDate">
                                    <span id="ProjCompDate">Set Date</span>
                                </button>
                            </div>
                        </div>

                        <div class="row mb-3 align-items-center">
                            <label class="col-md-3 col-form-label" for="txtProjectedTotalNumberofDays">Number of Days: <span style="color:#F00">*</span></label>
                            <div class="col-md">
                                <span id="txtProjectedTotalNumberofDays" style="font-weight:bold;">0</span>
                            </div>
                        </div>

                        <h4 class="mb-3">Actual Date</h4>

                        <div class="row mb-3">
                            <label class="col-md-3 col-form-label" for="btnSetDate">Actual Start Date:</label>
                            <div class="col-md">
                                <button data-role="none" class="btn btn-secondary btn-block btnSetDate" startdate="ActualStartDate" setdateto="ActualStartDate">
                                    <span id="ActualStartDate">Set Date</span>
                                </button>
                            </div>
                        </div>

                        <div class="row mb-3">
                            <label class="col-md-3 col-form-label" for="btnSetDate">Actual Completion Date:</label>
                            <div class="col-md">
                                <button data-role="none" class="btn btn-secondary btn-block btnSetDate" startdate="ActualStartDate" setdateto="actualcompdate">
                                    <span id="ActualCompDate">Set Date</span>
                                </button>
                            </div>
                        </div>

                        <div class="row mb-3 align-items-center">
                            <label class="col-md-3 col-form-label" for="txtActualTotalNumberofDays">Number of Days: <span style="color:#F00">*</span></label>
                            <div class="col-md">
                                <span id="txtActualTotalNumberofDays" style="font-weight:bold;">0</span>
                            </div>
                        </div>

                        <h4 class="mb-3">Contact Details</h4>

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
                                            <button id="btnAddOrgFOWSTTelephone2" data-role="none" class="btn btn-success">
                                                Add Telephone
                                            </button>
                                        </div>

                                        <ul data-role="none" id="lstOrgFOWSTTelephone2" class="list-group"></ul>
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
                                            <button id="btnAddOrgFOWSTMobile2" data-role="none" class="btn btn-success">
                                                Add Mobile
                                            </button>
                                        </div>

                                        <ul data-role="none" id="lstOrgFOWSTMobile2" class="list-group"></ul>
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
                                            <button id="btnAddOrgFOWSTEmail2" data-role="none" class="btn btn-success">
                                                Add Email Address
                                            </button>
                                        </div>

                                        <ul data-role="none" id="lstOrgFOWSTEmail2" class="list-group"></ul>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <div class="row">
                        <div class="col">
                            <button data-role="none" type="button" id="btnCancelNewProject" class="btn btn-secondary btn-lg btn-block">Cancel</button>
                        </div>
                        <div class="col">
                            <button data-role="none" type="button" id="btnSaveNewProject" class="btn btn-success btn-lg btn-block">Save</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div id="popupNewProjectCustomer" class="modal fade" data-backdrop="static" tabindex="-1">
        <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Project</h5>
                    <!--<button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>-->
                </div>
                <div class="modal-body">
                    <div id="divNewProjectCustomer" style="display:block">
                        <div class="mb-3">
                            Please fill in all the required fields (<span style="color:#F00;">*</span>)
                        </div>

                        <div class="row mb-3">
                            <label class="col-md-3 col-form-label" for="selCustomerType">Customer Type:</label>
                            <div class="col-md">
                                <select id="selCustomerType" data-role="none" class="form-select">
                                    <option value="Organization">Organization</option>
                                    <option value="Person">Person</option>
                                </select>
                            </div>
                        </div>                        

                        <div id="divCustomerOrg">
                            <div class="row mb-3">
                                <label class="col-md-3 col-form-label" for="txtProjectCustomerCompanyName">Company Name: <span style="color: #FF0000;">*</span></label>
                                <div class="col-md">
                                    <input type="text" data-role="none" class="form-control" id="txtProjectCustomerCompanyName" />
                                </div>
                            </div>

                            <div class="row mb-3">
                                <label class="col-md-3 col-form-label" for="txtProjectCustomerTIN">Tax Identification Number (TIN): <span style="color: #FF0000;">*</span></label>
                                <div class="col-md">
                                    <input type="number" data-role="none" class="form-control" id="txtProjectCustomerTIN" maxlength="16" />
                                </div>
                            </div>

                            <div class="row mb-3">
                                <label class="col-md-3 col-form-label" for="txtCustomerOrgHouseNo">Address: <span style="color: #FF0000;">*</span></label>
                                <div class="col-md">
                                    <input type="text" data-role="none" class="form-control mb-3" id="txtCustomerOrgHouseNo" placeholder="House / Building No." />
                                    <input type="text" data-role="none" class="form-control mb-3" id="txtCustomerOrgAddress" placeholder="Street" />
                                    <select id="selOrgCity" data-role="none" class="form-select"></select>
                                    <select id="selOrgCountry" data-role="none" class="form-select"></select>
                                </div>
                            </div>
                        </div>

                        <div id="divCustomerPerson" style="display:none;">
                            <div class="row mb-3">
                                <label class="col-md-3 col-form-label" for="txtProjectCustomerFirstName">First Name: <span style="color: #FF0000;">*</span></label>
                                <div class="col-md">
                                    <input type="text" data-role="none" class="form-control" id="txtProjectCustomerFirstName" />
                                </div>
                            </div>

                            <div class="row mb-3">
                                <label class="col-md-3 col-form-label" for="txtProjectCustomerMiddleName">Middle Name: <span style="color: #FF0000;">*</span></label>
                                <div class="col-md">
                                    <input type="text" data-role="none" class="form-control" id="txtProjectCustomerMiddleName" />
                                </div>
                            </div>

                            <div class="row mb-3">
                                <label class="col-md-3 col-form-label" for="txtProjectCustomerSurname">Last Name: <span style="color: #FF0000;">*</span></label>
                                <div class="col-md">
                                    <input type="text" data-role="none" class="form-control" id="txtProjectCustomerSurname" />
                                </div>
                            </div>

                            <div class="row mb-3">
                                <label class="col-md-3 col-form-label" for="txtProjectCustomerPersonTIN">Tax Identification Number (TIN)</label>
                                <div class="col-md">
                                    <input type="number" data-role="none" class="form-control" id="txtProjectCustomerPersonTIN" />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <div class="row">
                        <div class="col">
                            <button data-role="none" type="button" id="btnCustomerNameCancel" class="btn btn-secondary btn-lg btn-block">Cancel</button>
                        </div>
                        <div class="col">
                            <button data-role="none" type="button" id="btnCustomerNameSave" class="btn btn-success btn-lg btn-block">Save</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div id="popDefineRole" class="modal fade" data-backdrop="static" tabindex="-1">
        <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Define Roles and Permissions for this Project</h5>
                    <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                </div>
                <div class="modal-body">
                    <div class="divSelectionRole" style="display:block; overflow:auto;">
                        <div class="row">
                            <div class="col-md">
                                <div class="form-check">
                                    <label class="form-check-label" for="chbJobRole_25fdc489-40c5-4210-8e22-eb9665029f8c">Project Executive</label>
                                    <input data-role="none" type="checkbox" class="chbJobRole form-check-input" id="chbJobRole_25fdc489-40c5-4210-8e22-eb9665029f8c" name="chbRoleName" value="25fdc489-40c5-4210-8e22-eb9665029f8c" />
                                </div>
                                
                                <div class="form-check">
                                    <label class="form-check-label" for="chbJobRole_eb39f2c2-326c-4beb-8af4-a1f30f900640">Project Manager</label>
                                    <input data-role="none" type="checkbox" class="chbJobRole form-check-input" id="chbJobRole_eb39f2c2-326c-4beb-8af4-a1f30f900640" name="chbRoleName" value="eb39f2c2-326c-4beb-8af4-a1f30f900640" />
                                </div>
                                
                                <div class="form-check">
                                    <label class="form-check-label" for="chbJobRole_6ca646ea-6de6-46fe-a25d-60f8d8522495">Planning Engineer</label>
                                    <input data-role="none" type="checkbox" class="chbJobRole form-check-input" id="chbJobRole_6ca646ea-6de6-46fe-a25d-60f8d8522495" name="chbRoleName" value="6ca646ea-6de6-46fe-a25d-60f8d8522495" />
                                </div>
                                
                                <div class="form-check">
                                    <label class="form-check-label" for="chbJobRole_fdf2c645-e529-47a8-b37e-2ee339e1350d">Project Coordinator</label>
                                    <input data-role="none" type="checkbox" class="chbJobRole form-check-input" id="chbJobRole_fdf2c645-e529-47a8-b37e-2ee339e1350d" name="chbRoleName" value="fdf2c645-e529-47a8-b37e-2ee339e1350d" />
                                </div>
                                
                                <div class="form-check">
                                    <label class="form-check-label" for="chbJobRole_2d5b66c5-9a59-4aa1-ad6d-02623a40b03d">Design Manager</label>
                                    <input data-role="none" type="checkbox" class="chbJobRole form-check-input" id="chbJobRole_2d5b66c5-9a59-4aa1-ad6d-02623a40b03d" name="chbRoleName" value="2d5b66c5-9a59-4aa1-ad6d-02623a40b03d" />
                                </div>
                                
                                <div class="form-check">
                                    <label class="form-check-label" for="chbJobRole_73d78d43-b0b2-412c-85c4-171d9556bdcb">Field Engineer</label>
                                    <input data-role="none" type="checkbox" class="chbJobRole form-check-input" id="chbJobRole_73d78d43-b0b2-412c-85c4-171d9556bdcb" name="chbRoleName" value="73d78d43-b0b2-412c-85c4-171d9556bdcb" />
                                </div>
                                
                                <div class="form-check">
                                    <label class="form-check-label" for="chbJobRole_fb39c35b-2933-4f88-8ec6-fec23f484fa1">Office Engineer</label>
                                    <input data-role="none" type="checkbox" class="chbJobRole form-check-input" id="chbJobRole_fb39c35b-2933-4f88-8ec6-fec23f484fa1" name="chbRoleName" value="fb39c35b-2933-4f88-8ec6-fec23f484fa1" />
                                </div>
                                
                                <div class="form-check">
                                    <label class="form-check-label" for="chbJobRole_1f27d07a-1dcd-47fe-9704-0dbac92aebdc">Quantity Surveyor</label>
                                    <input data-role="none" type="checkbox" class="chbJobRole form-check-input" id="chbJobRole_1f27d07a-1dcd-47fe-9704-0dbac92aebdc" name="chbRoleName" value="1f27d07a-1dcd-47fe-9704-0dbac92aebdc" />
                                </div>
                                
                                <div class="form-check">
                                    <label class="form-check-label" for="chbJobRole_a4b992c3-3cde-4526-ac96-a0e27e1748a5">Project Engineer</label>
                                    <input data-role="none" type="checkbox" class="chbJobRole form-check-input" id="chbJobRole_a4b992c3-3cde-4526-ac96-a0e27e1748a5" name="chbRoleName" value="a4b992c3-3cde-4526-ac96-a0e27e1748a5" />
                                </div>                                
                            </div>

                            <div class="col-md">
                                <div class="form-check">
                                    <label class="form-check-label" for="chbJobRole_39c19a0a-1895-4328-baf1-c35ebb9a152a">Area Superintendent</label>
                                    <input data-role="none" type="checkbox" class="chbJobRole form-check-input" id="chbJobRole_39c19a0a-1895-4328-baf1-c35ebb9a152a" name="chbRoleName" value="39c19a0a-1895-4328-baf1-c35ebb9a152a" />
                                </div>
                                
                                <div class="form-check">
                                    <label class="form-check-label" for="chbJobRole_39067037-d160-41be-9cc3-529824332901">Project Superintendent</label>
                                    <input data-role="none" type="checkbox" class="chbJobRole form-check-input" id="chbJobRole_39067037-d160-41be-9cc3-529824332901" name="chbRoleName" value="39067037-d160-41be-9cc3-529824332901" />
                                </div>
                                
                                <div class="form-check">
                                    <label class="form-check-label" for="chbJobRole_dc0e3687-0ce2-484c-bf67-3c5dcd66b549">Estimator</label>
                                    <input data-role="none" type="checkbox" class="chbJobRole form-check-input" id="chbJobRole_dc0e3687-0ce2-484c-bf67-3c5dcd66b549" name="chbRoleName" value="dc0e3687-0ce2-484c-bf67-3c5dcd66b549" />
                                </div>
                                
                                <div class="form-check">
                                    <label class="form-check-label" for="chbJobRole_7fd8b9fa-bcaf-4d91-956e-d49263a1bd31">Lead Estimator</label>
                                    <input data-role="none" type="checkbox" class="chbJobRole form-check-input" id="chbJobRole_7fd8b9fa-bcaf-4d91-956e-d49263a1bd31" name="chbRoleName" value="7fd8b9fa-bcaf-4d91-956e-d49263a1bd31" />
                                </div>
                                
                                <div class="form-check">
                                    <label class="form-check-label" for="chbJobRole_cf5006b6-747b-4d68-b9b2-937072b705dd">Senior Estimator</label>
                                    <input data-role="none" type="checkbox" class="chbJobRole form-check-input" id="chbJobRole_cf5006b6-747b-4d68-b9b2-937072b705dd" name="chbRoleName" value="cf5006b6-747b-4d68-b9b2-937072b705dd" />
                                </div>
                                
                                <div class="form-check">
                                    <label class="form-check-label" for="chbJobRole_0e446a66-8483-40c2-881a-dacf830ec75e">Chief Estimator</label>
                                    <input data-role="none" type="checkbox" class="chbJobRole form-check-input" id="chbJobRole_0e446a66-8483-40c2-881a-dacf830ec75e" name="chbRoleName" value="0e446a66-8483-40c2-881a-dacf830ec75e" />
                                </div>
                                
                                <div class="form-check">
                                    <label class="form-check-label" for="chbJobRole_9ca12493-dba9-43db-a92b-a1ece98bab27">Site Manager</label>
                                    <input data-role="none" type="checkbox" class="chbJobRole form-check-input" id="chbJobRole_9ca12493-dba9-43db-a92b-a1ece98bab27" name="chbRoleName" value="9ca12493-dba9-43db-a92b-a1ece98bab27" />
                                </div>
                                
                                <div class="form-check">
                                    <label class="form-check-label" for="chbJobRole_dd0991d8-627f-4298-973d-f0c1cb4e58aa">Project Document Controller</label>
                                    <input data-role="none" type="checkbox" class="chbJobRole form-check-input" id="chbJobRole_dd0991d8-627f-4298-973d-f0c1cb4e58aa" name="chbRoleName" value="dd0991d8-627f-4298-973d-f0c1cb4e58aa" />
                                </div>
                                
                                <div class="form-check">
                                    <label class="form-check-label" for="chbJobRole_8c215a27-9427-498e-8fb8-505059a65030">Project Designer</label>
                                    <input data-role="none" type="checkbox" class="chbJobRole form-check-input" id="chbJobRole_8c215a27-9427-498e-8fb8-505059a65030" name="chbRoleName" value="8c215a27-9427-498e-8fb8-505059a65030" />
                                </div>                                
                            </div>
                        </div>
                    </div>

                    <div class="divDefineRole" style="display:none;">
                        <div id="divContentDefineRole" style="overflow:auto;">
                            <table data-mode="none" cellpadding="2" cellspacing="0" class="table table-light table-striped table-hover" width="100%">
                                <thead>
                                    <tr class="custom-theme-d" id="theadDefineRole"></tr>
                                </thead>

                                <tbody id="tbodyDefineRole"></tbody>
                            </table>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <div class="row divSelectionRole">
                        <div class="col">
                            <button data-role="none" type="button" id="btnCancelAccessRights" class="btn btn-secondary btn-lg btn-block">Cancel</button>
                        </div>
                        <div class="col">
                            <button data-role="none" type="button" id="btnOkAccessRights" class="btn btn-success btn-lg btn-block">Next</button>
                        </div>
                    </div>
                    <div class="row divDefineRole" style="display:none;">
                        <div class="col">
                            <button data-role="none" type="button" id="btnCancelDefineRole" class="btn btn-secondary btn-lg btn-block">Cancel</button>
                        </div>
                        <div class="col">
                            <button data-role="none" type="button" id="btnSaveDefineRole" class="btn btn-success btn-lg btn-block">Save</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div id="popupProjectTelephone" class="modal fade" tabindex="-1">
        <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Telephone Info</h5>
                    <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                </div>
                <div class="modal-body">
                    <div style="display:none;">
                        <input type="text" data-role="none" class="form-control" readonly id="txtOrgFOWSTTelCountryID2" value="e783ed13-aa98-4cc1-918e-d49b05c0167e" />
                        <input type="text" data-role="none" class="form-control" readonly id="txtOrgFOWSTTelID2" />
                        <input type="text" data-role="none" class="form-control" readonly id="txtOrgFOWSTTelAreaCodeID2" />
                    </div>

                    <div class="mb-3">
                        <label class="form-label" for="btnOrgFOWSTTelCountryCodeDisable">
                            Country Code: <span style="color: #FF0000;">*</span>
                            <span id="spanerrorTelCountryCode" style="color:#F00; display:none;">This field is required</span>
                        </label>
                        <button id="btnOrgFOWSTTelCountryCodeDisable" data-role="none" class="btn btn-secondary btn-block"><span id="txtCountryCode">PHL - 63</span></button>
                    </div>

                    <div class="mb-3">
                        <label class="form-label" for="btnOrgFOWSTTelAreaCode2">
                            Area Code: <span style="color: #FF0000;">*</span>
                            <span id="spanerrorAreaCode" style="color:#F00; display:none;">This field is required</span>
                        </label>
                        <button id="btnOrgFOWSTTelAreaCode2" data-role="none" class="btn btn-secondary btn-block"><span id="txtAreaCode">Please Select</span></button>
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
                        <label class="form-label" for="selOrgFOWSTTelType2">Contact Type: <span style="color: #FF0000;">*</span> <span id="spanerrorTelType" style="color:#F00; display:none;">This field is required</span></label>
                        <select id="selOrgFOWSTTelType2" data-role="none" class="form-select">
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
                            <button data-role="none" type="button" id="btnCancelOrgFOWSTTelephone" class="btn btn-secondary btn-block btn-lg" data-dismiss="modal">Cancel</button>
                        </div>
                        <div class="col-6">
                            <button data-role="none" type="button" id="btnSaveOrgFOWSTTelephone" class="btn btn-success btn-block btn-lg">Ok</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div id="popupProjectMobile" class="modal fade" tabindex="-1">
        <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Mobile Info</h5>
                    <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                </div>
                <div class="modal-body">
                    <div style="display:none;">
                        <input type="text" data-role="none" class="form-control" readonly id="txtOrgFOWSTMobileCountryID2" value="e783ed13-aa98-4cc1-918e-d49b05c0167e" />
                        <input type="text" data-role="none" class="form-control" readonly id="txtOrgFOWSTMobileID2" />
                        <input type="text" data-role="none" class="form-control" readonly id="txtOrgFOWSTTelCoPrefixID2" />
                    </div>

                    <div class="mb-3">
                        <label class="form-label" for="btnOrgFOWSTMobileCountryCodeDisable">
                            Country Code: <span style="color: #FF0000;">*</span>
                            <span id="spanerrorMobileCountryCode" style="color:#F00; display:none;">This field is required</span>
                        </label>
                        <button id="btnOrgFOWSTMobileCountryCodeDisable" data-role="none" class="btn btn-secondary btn-block"><span id="txtMobileCountryCode">PHL - 63</span></button>
                    </div>

                    <div class="mb-3">
                        <label class="form-label" for="btnOrgFOWSTMobileTelCode2">
                            Tel Code: <span style="color: #FF0000;">*</span>
                            <span id="spanerrorMobileTelCode" style="color:#F00; display:none;">This field is required</span>
                        </label>
                        <button id="btnOrgFOWSTMobileTelCode2" data-role="none" class="btn btn-secondary btn-block"><span id="txtTelCode">Select</span></button>
                    </div>

                    <div class="mb-3">
                        <label class="form-label" for="txtMobileNo2">
                            Mobile Number: <span style="color: #FF0000;">*</span>
                            <span id="spanerrorMobileNum" style="color:#F00; display:none;">This field is required</span>
                            <span id="spanerrorMobileNumValidate" style="color:#F00; display:none;">Enter 10 digit number</span>
                        </label>
                        <input type="number" id="txtMobileNo2" data-role="none" class="form-control" max="7" min="7" placeholder="Last 7 digit" />
                    </div>

                    <div class="mb-3">
                        <label class="form-label" for="selOrgFOWSTMobileType2">Contact Type: <span style="color: #FF0000;">*</span> <span id="spanerrorMobileType" style="color:#F00; display:none;">This field is required</span></label>
                        <select id="selOrgFOWSTMobileType2" data-role="none" class="form-select">
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
                            <button data-role="none" type="button" id="btnCancelOrgFOWSTMobile" class="btn btn-secondary btn-block btn-lg" data-dismiss="modal">Cancel</button>
                        </div>
                        <div class="col-6">
                            <button data-role="none" type="button" id="btnSaveOrgFOWSTMobile" class="btn btn-success btn-block btn-lg">Ok</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div id="popupProjectEmailAdd" class="modal fade" tabindex="-1">
        <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">E-mail Info</h5>
                    <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                </div>
                <div class="modal-body">
                    <div style="display:none;">
                        <input type="text" data-role="none" class="form-control" readonly id="txtOrgFOWSTEmailID2" />
                    </div>

                    <div class="mb-3">
                        <label class="form-label" for="txtEmailAdd2">
                            Email Address: <span style="color: #FF0000;">*</span>
                            <span id="spanerrorEmailAddress" style="color:#F00; display:none;">This field is required</span>
                            <span id="spanerrorEmailAddressValidate" style="color:#F00; display:none;">Enter valid email address</span>
                        </label>
                        <input type="email" id="txtEmailAdd2" data-role="none" class="form-control" />
                    </div>

                    <div class="mb-3">
                        <label class="form-label" for="selOrgFOWSTEmailType2">Contact Type: <span style="color: #FF0000;">*</span> <span id="spanerrorEmailType" style="color:#F00; display:none;">This field is required</span></label>
                        <select id="selOrgFOWSTEmailType2" data-role="none" class="form-select">
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
                            <button data-role="none" type="button" id="btnCancelOrgFOWSTEmailAddress" class="btn btn-secondary btn-block btn-lg" data-dismiss="modal">Cancel</button>
                        </div>
                        <div class="col-6">
                            <button data-role="none" type="button" id="btnSaveOrgFOWSTEmailAddress" class="btn btn-success btn-block btn-lg">Ok</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div id="panMoreOption" class="modal fade" tabindex="-1">
        <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">More Options</h5>
                    <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                </div>
                <div class="modal-body">
                    <div class="list-group">
                        <a href="#" id="btnDisplayAssociatedSKU" state="show" class="list-group-item list-group-item-action d-flex justify-content-between align-items-center" arr="0" x="y" style="padding: 1em;">
                            <div style="width:100%; text-align:left;">
                                <div style="font-weight: normal">Show SKU</div>
                            </div>
                            <span class="bi-chevron-right"></span>
                        </a>
                        <a href="#" id="btnPrintCanvassSheets" class="list-group-item list-group-item-action d-flex justify-content-between align-items-center" arr="0" x="y" style="padding: 1em;">
                            <div style="width:100%; text-align:left;">
                                <div style="font-weight: normal">Print Canvass Sheet</div>
                            </div>
                            <span class="bi-chevron-right"></span>
                        </a>
                        <a href="#" id="btnAddNewVendorCanvassSheets" class="list-group-item list-group-item-action d-flex justify-content-between align-items-center" arr="0" x="y" style="padding: 1em;">
                            <div style="width:100%; text-align:left;">
                                <div style="font-weight: normal">Add New Vendor</div>
                            </div>
                            <span class="bi-chevron-right"></span>
                        </a>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div id="panAddChild" class="modal fade" tabindex="-1">
        <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title" id="lblTitleAddChild">Add Sub Task</h5>
                    <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                </div>
                <div class="modal-body">
                    <div class="mb-3 row align-items-center">
                        <div class="col">Description:</div>
                        <div class="col text-right"><button data-role="none" id="btnAddAnotherDescription" class="btn btn-success">Add Description</button></div>
                    </div>

                    <div id="lstChildDescription" class="mb-3">
                        <input type="text" data-role="none" id="txtChildDescription" class="txtChildDescription mb-3 form-control" />
                    </div>
                </div>
                <div class="modal-footer">
                    <div class="row">
                        <div class="col">
                            <button data-role="none" data-dismiss="modal" class="btn btn-secondary btn-block btn-lg">Cancel</button>
                        </div>
                        <div class="col">
                            <button id="btnSaveChild" data-role="none" class="btn btn-success btn-block btn-lg">Ok</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div data-role="popup" id="panSearchLibrary" data-dismissible="false" data-position-to="window" data-overlay-theme="a" data-theme="a" data-corners="false">
        <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right btnPopupClose">Close</a>
        <center><h3 style="margin-left:10px; margin-right:10px;" id="TitleLibrary"></h3></center>
        <div data-role="content" style="max-width:350px; word-wrap: break-word;">
            <div id="divSKUMsg" style="display: none;">
                <span>Please choose 1 specific product or service (SKU) to associate with "<strong id="txtDescriptionName"></strong>".</span>
                <br /><br />
            </div>

            <input type="text" data-role="none" class="form-control" id="txtSearchKeyword" autofocus="autofocus" />

            <div style="padding: 8px 15px 0px 15px; max-height:200px; overflow:auto;">
                <ul id="lstOfLibrary" data-role="listview" class="li-theme-c noBorderList custom-list custom-list-sm">
                    @*<li>
                            <a id="">
                                asd
                            </a>
                        </li>*@
                </ul>
                <br>
            </div>
            <br />
        </div>
    </div>

    <div id="panSearchCanvass" class="modal fade" tabindex="-1">
        <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Canvass</h5>
                    <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                </div>
                <div class="modal-body">
                    <div class="mb-3 row align-items-center" style="text-align:right">
                        <div class="col">
                            Select Vendors to Canvass
                        </div>
                        <div class="col text-right">
                            <button data-role="none" id="btnCreateNewVendor" class="btn btn-secondary">Create New</button>
                        </div>                        
                    </div>
                    
                    <div class="mb-3">
                        Note: Please choose up to 10 vendors that you want to canvass.
                    </div>

                    <input type="text" data-role="none" class="form-control mb-3" id="txtSearchOrgEmp" placeholder="Search Vendor name" />

                    <div class="row mb-3" id="divVendorSelection" style="display:none;">
                        <div class="col divCountVendor">

                        </div>
                        <div class="col" style="text-align:right;">
                            <a data-role="none" id="btnClearVendorSelected" class="link-primary">Clear Selection</a>
                        </div>
                    </div>

                    <div id="divVendorValidation" class="mb-3" style="color:#F00; display:none;">

                    </div>

                    <ul id="lstOfOrgEmp" data-role="none" class="list-group"></ul>
                </div>
                <div class="modal-footer">
                    <div class="row">
                        <div class="col">
                            <button id="btnSearchCanvassCancel" data-role="none" data-dismiss="modal" class="btn btn-secondary btn-block btn-lg">Cancel</button>
                        </div>
                        <div class="col">
                            <button id="btnSearchCanvassOk" data-role="none" class="btn btn-success btn-block btn-lg">Ok</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div id="panListOfVendors" class="modal fade" tabindex="-1">
        <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">List Of Selected Vendor(s)</h5>
                    <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                </div>
                <div class="modal-body">
                    <div class="row mb-3 align-items-center">
                        <div class="col divCountVendor">

                        </div>
                        <div class="col text-right">
                            <button data-role="none" id="btnAddVendor" class="btn btn-success">Add Vendor</button>
                        </div>
                    </div>

                    <ul id="lstOfSelectedVendors" data-role="none" class="list-group">
                        <li class="list-group-item">
                            Empty List
                        </li>
                    </ul>
                </div>
                <div class="modal-footer">
                    <div class="row">
                        <div class="col">
                            <button id="btnCanvassCancel" data-role="none" data-dismiss="modal" class="btn btn-secondary btn-block btn-lg">Cancel</button>
                        </div>
                        <div class="col" id="btnCanvassNow">
                            <button data-role="none" class="btn btn-success btn-block btn-lg">Canvass Now</button>
                        </div>
                        <div class="col" id="btnEditingModeCanvassNow" style="display:none;">
                            <button data-role="none" class="btn btn-success btn-block btn-lg">Canvass Now</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div data-role="popup" id="panMessageBox" data-dismissible="false" data-overlay-theme="a" data-theme="a" data-corners="false">
        @*<a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right btnPopupClose">Close</a>*@
        <center><h3 style="margin-left:10px; margin-right:10px;" id="MsgBoxTitle"></h3></center>
        <div data-role="content" style="max-width:350px; word-wrap: break-word;">

            <div id="MsgBoxMessage" style="font-weight:bold;"></div>

            <br />
            <br />

            <button id="btnOKMsgBox" data-role="none" class="btn custom-btn-theme-b">
                Ok
            </button>

            <div id="divBtnCancel" style="display:none;">
                <br />
                <button id="btnCancelMsgBox" data-role="none" class="btn btn-secondary btn-block">
                    Cancel
                </button>
            </div>
            <br />
        </div>
    </div>

    <div id="panChooseAVendor" class="modal fade" tabindex="-1">
        <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Select a Preferred Vendor</h5>
                    <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                </div>
                <div class="modal-body">
                    <ul id="lstOfVendor" data-role="none" class="list-group"></ul>
                </div>
                <div class="modal-footer">
                    <button data-role="none" data-dismiss="modal" class="btn btn-secondary btn-block btn-lg">Cancel</button>
                </div>
            </div>
        </div>
    </div>

    <div data-role="popup" data-dismissible="false" data-position-to="window" data-overlay-theme="a" data-theme="a" data-corners="false">
        <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right btnPopupClose">Close</a>
        <center><h3 style="margin-left:10px; margin-right:10px;">Canvass From</h3></center>
        <div data-role="content" style="max-width:350px; word-wrap: break-word;">
            How would you like to display materials/labor cost on the Project Budget sheet?
            <br /><br />

            <div style="padding: 8px 15px 0px 15px; max-height:300px; overflow:auto;">
                <ul data-role="listview" class="li-theme-c noBorderList custom-list custom-list-sm">
                    <li>
                        <a id="btnPreferredVendor">
                            Select a preferred vendor
                        </a>
                    </li>
                    <li>
                        <a id="btnDisplayMinPrice">
                            Display the minimum price
                        </a>
                    </li>
                </ul>
                <br>
            </div>
            <br />
        </div>
    </div>

    <div data-role="popup" id="panSKUUnitOfMeasures" data-dismissible="false" data-position-to="window" data-overlay-theme="a" data-theme="a" data-corners="false">
        @*<a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right btnPopupClose">Close</a>*@
        <center><h3 style="margin-left:10px; margin-right:10px;">Unit Of Measure</h3></center>
        <div data-role="content" style="max-width:350px; word-wrap: break-word;">
            <a id="btnSKUUnitMeasureBack" style="margin-right:10px;" data-role="none" class="btn btn-less-width btn-transparent">
                <span class="ui-icon ui-icon-arrow-l icon-left" style="display:inline-block;"></span>
                Back
            </a>
            <div>
                Please choose the typical unit of measure for buying the <strong id="txtSKUName"></strong>
            </div>
            <br />
            <div style="padding: 8px 15px 0px 15px; max-height:300px; overflow:auto;">
                <fieldset data-role="controlgroup" id="radSKUUnitMeasure"></fieldset>
                <br />
            </div>
            <br />
            <div class="ui-grid-a ui-responsive">
                <div class="ui-block-a" style="padding:0px 7px 7px 7px;">
                    <button data-role="none" id="btnSKUUnitMeasureCancel" class="btn btn-secondary btn-block">
                        Cancel
                    </button>
                </div>
                <div class="ui-block-b" style="padding:0px 7px 7px 7px;">
                    <button data-role="none" id="btnSKUUnitMeasureOk" class="btn custom-btn-theme-b">
                        Ok
                    </button>
                </div>
            </div>
        </div>
    </div>

    <div data-role="popup" id="popMultipleSKUUnitOfMeasures" data-dismissible="false" data-position-to="window" data-overlay-theme="a" data-theme="a" data-corners="false">
        @*<a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right btnPopupClose">Close</a>*@
        <center><h3 style="margin-left:10px; margin-right:10px;">Unit Of Measure</h3></center>
        <div data-role="content" class="jPopupContent">
            <div>
                Please choose the typical unit of measure of the following SKU(s).
            </div>
            <br />
            <table data-role="table" data-mode="reflow" style="margin-right:5px; display:table; table-layout:fixed;" class="table table-light table-striped table-hover">
                <thead>
                    <tr class="custom-theme-f">
                        <th> SKU Name </th>
                        <th> Unit Measure </th>
                    </tr>
                </thead>
                <tbody id="tblBodyMultipleSKU"></tbody>
            </table>
            <br />
            <div class="ui-grid-a ui-responsive">
                <div class="ui-block-a" style="padding:0px 7px 7px 7px;">
                    <button data-role="none" id="" class="btn btn-secondary btn-block">
                        Cancel
                    </button>
                </div>
                <div class="ui-block-b" style="padding:0px 7px 7px 7px;">
                    <button data-role="none" id="btnMultipleSKUUnitMeasureOk" class="btn custom-btn-theme-b">
                        Ok
                    </button>
                </div>
            </div>
        </div>
    </div>

    <div data-role="popup" id="panAddLibrary" data-overlay-theme="a" data-theme="a" data-corners="false">
        <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right btnClosePopUp">Close</a>
        <center><h3 style="margin-left:10px; margin-right:10px;">Please type name of <span class="lblPopUpHeaderLibraryName"></span></h3></center>
        <div data-role="content" style="max-width:350px;">

            <input type="text" data-role="none" class="form-control" id="txtSearch" />

            <div id="divNoResultsFound" style="display:none;">
                <br />
                Oops! We can’t find what you’re looking for. Make sure <br /> your search is correctly spelled.
            </div>
            <div style="padding: 8px 15px 0px 15px; max-height:200px; overflow:auto;">
                <ul id="lstSearchFound" data-role="listview" class="li-theme-c noBorderList custom-list custom-list-sm"></ul>
                <br />
            </div>

            <div id="btnAddToLibrary" class="divLibAddOption" style="display:none;">
                <br />
                If you’ve correctly spelled your search but can’t find it, <br /> you might want to <a class="custom-link">add a new <span class="lblAddLinkLibraryName"></span></a>.
                <br /><br />
            </div>
            <br />
            <div class="ui-grid-solo">
                <div class="ui-block-a">
                    <button id="btnSearchLibrary" data-role="none" class="btn custom-btn-theme-b">
                        Search
                    </button>
                    <br /><br />
                    <button id="btnCancelLibrary" data-role="none" class="btn btn-secondary btn-block">
                        Cancel
                    </button>
                </div>
            </div>
            <br />
            @*<div class="ui-grid-solo divAddLibrary" style="display:none;">
                    <div class="ui-block-a">
                        <button id="btnAddToLibrary" data-role="none" class="btn custom-btn-theme-f">
                            Add
                        </button>
                    </div>
                </div>*@
        </div>
    </div>

    <div data-role="popup" id="divAddProductPopup" data-overlay-theme="a" data-theme="a" data-corners="false">
        <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
        <center><h3 style="margin-left:10px; margin-right:10px;" id="divAddProductTitle">Option</center>
        <div data-role="content">
            <div style="padding: 8px 15px 16px 15px;">
                <div id="listForAdding" style="display:none; visibility:hidden;">
                    <ul data-role="listview" class="li-theme-c noBorderList custom-list custom-list-sm">
                        <li>
                            <a id="btnBarcodeSearch">Searching a bar code or product description from the Mobiz public database</a>
                        </li>
                        <li>
                            <a id="btnImportByExcel">Importing data from an Excel file</a>
                        </li>
                        <li>
                            <a id="btnSubscribe">Subscribing to publicly available product lists</a>
                        </li>
                    </ul>
                </div>

                <div id="listForOption" style="display:none; visibility:hidden;">
                    <ul data-role="listview" class="li-theme-c noBorderList custom-list custom-list-sm">
                        <li>
                            <a id="btnAdjustProdPrices">Adjust prices</a>
                        </li>
                        <li>
                            <a id="btnFowstProdList">Create approved product lists by business location</a>
                        </li>
                    </ul>
                </div>
            </div>
        </div>
    </div>

    <div data-role="popup" id="divAdjustProdPricesPopup" data-overlay-theme="a" data-theme="a" data-corners="false">
        <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
        <center><h3 style="margin-left:10px; margin-right:10px;">Adjust Product Prices</center>
        <div data-role="content">
            <div style="padding: 8px 15px 16px 15px;">
                <div id="btnBackToOption" style="display:block; cursor:pointer; text-transform:capitalize;">
                    <span class="ui-icon ui-icon-arrow-l" style="display:inline-block; margin-left:-12px;">
                        <span style="margin-left:25px;"> Back </span>
                    </span>
                </div>
                <br />
                <ul data-role="listview" class="li-theme-c noBorderList custom-list custom-list-sm">
                    <li>
                        <a id="btnSinglePrice">Single Product Price Adjustment</a>
                    </li>
                    <li>
                        <a id="btnBulkPrice">Bulk Price Adjustment</a>
                    </li>
                </ul>
            </div>
        </div>
    </div>

    <div data-role="popup" id="panCanvassOption" data-dismissible="true" data-position-to="window" data-overlay-theme="a" data-theme="a" data-corners="false">
        <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right btnPopupClose">Close</a>
        <center><h3 style="margin-left:10px; margin-right:10px;">Canvass Options</h3></center>
        <div data-role="content" style="max-width:350px; word-wrap: break-word;">
            <div style="padding: 8px 15px 0px 15px; max-height:300px; overflow:auto;">
                <ul data-role="listview" class="li-theme-c noBorderList custom-list custom-list-sm">
                    <li>
                        <a class="btnCanvassMaterials">
                            Canvass Materials
                        </a>
                    </li>
                    <li>
                        <a class="btnCanvassLabor">
                            Canvass Labor
                        </a>
                    </li>
                </ul>
                <br>
            </div>
        </div>
    </div>

    <div id="popupProjectRemarks" class="modal fade" tabindex="-1">
        <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Remarks for <span id="lblRemarksLineDescription"></span></h5>
                    <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                </div>
                <div class="modal-body">
                    <div id="divTabProjectRemarks">
                        <ul class="nav nav-tabs" role="tablist" style="flex-wrap:nowrap;">
                            <li class="nav-item" role="presentation">
                                <button data-role="none" class="nav-link active" data-toggle="tab" data-target="#tabAddProjectRemarks">Remarks</button>
                            </li>
                            <li class="nav-item" role="presentation">
                                <button data-role="none" class="nav-link" data-toggle="tab" data-target="#tabProjectRemarksHistory">History</button>
                            </li>
                        </ul>
                    </div>

                    <div class="tab-content p-3">
                        <div id="tabAddProjectRemarks" class="tab-pane fade show active">
                            <input data-role="none" type="file" id="fileProjectRemarksPhotos" accept="image/*" multiple style="display:none;" />
                            @*<textarea id="txtProjectRemarks" placeholder="Type here ..." style="resize:none; height:250px;"></textarea>*@
                            <div class="mb-3" style="text-align:right;">
                                <button data-role="none" id="btnProjectRemarksAddPhotos" class="btn btn-success">Add Photos</button>
                            </div>

                            <div id="txtProjectRemarks" class="mb-3" contenteditable style="min-height:250px; max-height:250px;" placeholder="Type the remarks here ...">
                            </div>

                            <div class="text-center">
                                <button data-role="none" data-dismiss="modal" class="btn btn-secondary btn-lg">
                                    Cancel
                                </button>
                                <button data-role="none" id="btnProjectAddRemarks" class="btn btn-success btn-lg">
                                    Ok
                                </button>
                            </div>
                        </div>
                        <div id="tabProjectRemarksHistory" class="tab-pane fade">
                            <div class="table-responsive">
                                <table id="" data-role="none" style="width:100%; display:table;" class="table table-light table-striped table-hover">
                                    <thead>
                                        <tr class="custom-theme-f">
                                            <th> Remarks </th>
                                            <th> Last Updated </th>
                                            <th> By </th>
                                        </tr>
                                    </thead>
                                    <tbody id="tblBodyProjectRemarks"></tbody>
                                </table>
                            </div>
                        </div>
                    </div>                    
                </div>
            </div>
        </div>
    </div>

    <div id="popupProjectRemarksDetails" class="modal fade" tabindex="-1">
        <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Remarks Details</h5>
                    <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                </div>
                <div class="modal-body">
                    <div style="text-align:right; color:#828282; font-size:12px;" id="lblRemarksInfo"></div>
                    <br />
                    <div id="divRemarksDetails"></div>
                </div>
            </div>
        </div>
    </div>

    <div id="popProjectAddRemarks" class="modal fade" tabindex="-1">
        <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Remarks</h5>
                    <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                </div>
                <div class="modal-body">
                    <h4 class="mb-3">Remarks for <span id="lblRemarksLineDescription"></span></h4>
                    <textarea data-role="none" class="form-control" id="txtProjectRemarks" placeholder="Type here ..." style="resize:none; height:200px;"></textarea>
                </div>
                <div class="modal-footer">
                    <div class="row">
                        <div class="col">
                            <button data-role="none" data-dismiss="modal" class="btn btn-secondary btn-lg btn-block">Cancel</button>
                        </div>
                        <div class="col">
                            <button data-role="none" id="btnProjectAddRemarks" class="btn btn-success btn-lg btn-block">Ok</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div id="popupProjectMaterialBudgetQty" class="modal fade" tabindex="-1">
        <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Material Budget Qty</h5>
                    <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                </div>
                <div class="modal-body">
                    <div id="divTabMaterialBudgetQty">
                        <ul class="nav nav-tabs" role="tablist" style="flex-wrap:nowrap;">
                            <li class="nav-item" role="presentation">
                                <button data-role="none" class="nav-link active" data-toggle="tab" data-target="#tabAddMaterialBudgetQty">Encode Qty</button>
                            </li>
                            <li class="nav-item" role="presentation">
                                <button data-role="none" class="nav-link" data-toggle="tab" data-target="#tabMaterialAuditLog">History</button>
                            </li>
                        </ul>
                    </div>

                    <div class="tab-content p-3">
                        <div id="tabAddMaterialBudgetQty" class="tab-pane fade show active">
                            <div class="mb-3">
                                <label class="form-label">Quantity: <span class="required">*</span></label>
                                <input type="text" data-role="none" class="form-control mb-3" id="txtMaterialBudgetQty" />
                                <select id="selMaterialBudgetUom" class="form-select" data-role="none" style="display:none;">
                                    <option value="">Please select</option>
                                </select>
                            </div>

                            <div class="text-center">
                                <button data-role="none" data-dismiss="modal" class="btn btn-secondary btn-lg">
                                    Cancel
                                </button>
                                <button data-role="none" id="btnOkMaterialBudgetQty" class="btn btn-success btn-lg">
                                    Ok
                                </button>
                            </div>
                        </div>
                        <div id="tabMaterialAuditLog" class="tab-pane fade">
                            <div class="table-responsive">
                                <table id="" data-role="none" style="width:100%; display:table;" class="table table-light table-striped table-hover">
                                    <thead>
                                        <tr class="custom-theme-f">
                                            <th> Qty </th>
                                            <th> Last Updated </th>
                                            <th> By </th>
                                        </tr>
                                    </thead>
                                    <tbody id="tblBodyMaterialBudgetAuditLog"></tbody>
                                </table>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div id="popupProjectMaterialAccReqQty" class="modal fade" tabindex="-1">
        <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Material Budget Qty</h5>
                    <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                </div>
                <div class="modal-body">
                    <div id="divTabMaterialBudgetQty">
                        <ul class="nav nav-tabs" role="tablist" style="flex-wrap:nowrap;">
                            <li class="nav-item" role="presentation">
                                <button data-role="none" class="nav-link active" data-toggle="tab" data-target="#tabAddMaterialAccReqQty">Request Material</button>
                            </li>
                            <li class="nav-item" role="presentation">
                                <button data-role="none" class="nav-link" data-toggle="tab" data-target="#tabMaterialAccReqQty">History</button>
                            </li>
                        </ul>
                    </div>

                    <div class="tab-content p-3">
                        <div id="tabAddMaterialAccReqQty" class="tab-pane fade show active">
                            <input data-role="none" type="text" id="txtReqByID" style="display:none;" />

                            <div class="mb-3">
                                <label class="form-label" for="txtAccReqQty">Requested Quantity: <span class="required">*</span></label>
                                <input type="text" data-role="none" class="form-control" id="txtAccReqQty" />
                            </div>

                            <div class="mb-3">
                                <label class="form-label" for="selAccReqBy">Requested By: <span class="required">*</span></label>
                                <select id="selAccReqBy" data-role="none" class="selReqByProjectMember form-select"></select>
                            </div>

                            <div class="mb-3">
                                <label class="form-label" for="txtAccReqDateNeeded">Date Needed: <span class="required">*</span></label>
                                <input type="text" data-role="none" class="form-control" id="txtAccReqDateNeeded" readonly />
                            </div>

                            <div class="mb-3">
                                <label class="form-label" for="txtAccReqDateNeeded">Time Needed:</label>
                                <div class="input-group">
                                    <input type="text" data-role="none" class="form-control txtAccReqTime" id="txtAccReqTimeFrom" placeholder="From" readonly />
                                    <input type="text" data-role="none" class="form-control txtAccReqTime" id="txtAccReqTimeTo" placeholder="To" readonly />
                                    <button data-role="none" class="btn btn-secondary" id="btnAccReqClearTime">Clear</button>
                                </div>
                            </div>

                            <div class="mb-3">
                                <label class="form-label" for="txtAccReqDateNeeded">Remarks:</label>
                                <textarea data-role="none" class="form-control" id="txtAccReqRemarks" style="resize:none; height:100px;"></textarea>
                            </div>
                            
                            <div class="text-center">
                                <button data-role="none" data-dismiss="modal" class="btn btn-secondary btn-lg">
                                    Cancel
                                </button>
                                <button data-role="none" id="btnOkMaterialAccReqQty" class="btn btn-success btn-lg">
                                    Ok
                                </button>
                            </div>
                        </div>
                        <div id="tabMaterialAccReqQty" class="tab-pane fade">
                            <div class="table-responsive">
                                <table id="" data-role="none" style="width:100%; display:table;" class="table table-light table-striped table-hover">
                                    <thead>
                                        <tr class="custom-theme-f">
                                            <th> Requisition Date </th>
                                            <th> Requested by </th>
                                            <th> Requested Material </th>
                                            <th> Date Needed </th>
                                            <th> Time Needed </th>
                                            <th> Remarks </th>
                                            <th> Status </th>
                                        </tr>
                                    </thead>
                                    <tbody id="tblBodyAccReqHistory"></tbody>
                                </table>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div id="popupProjectMaterialAccUsedUpQty" class="modal fade" tabindex="-1">
        <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Accumulated Used Up</h5>
                    <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                </div>
                <div class="modal-body">
                    <div id="divTabMaterialBudgetQty">
                        <ul class="nav nav-tabs" role="tablist" style="flex-wrap:nowrap;">
                            <li class="nav-item" role="presentation">
                                <button data-role="none" class="nav-link active" data-toggle="tab" data-target="#tabAddMaterialAccUsedUpQty">Declare Used Up Qty</button>
                            </li>
                            <li class="nav-item" role="presentation">
                                <button data-role="none" class="nav-link" data-toggle="tab" data-target="#tabMaterialAccUsedUpHistory">History</button>
                            </li>
                        </ul>
                    </div>

                    <div class="tab-content p-3">
                        <div id="tabAddMaterialAccUsedUpQty" class="tab-pane fade show active">
                            <div class="mb-3">
                                <label class="form-label" for="txtAccUsedUpQty">Quantity: <span class="required">*</span></label>
                                <input type="text" data-role="none" class="form-control" id="txtAccUsedUpQty" />
                            </div>

                            <div class="mb-3">
                                <label class="form-label" for="txtAccUsedUpRemarks">Remarks:</label>
                                <textarea data-role="none" class="form-control" id="txtAccUsedUpRemarks" style="resize:none; height:100px;"></textarea>
                            </div>

                            <div class="text-center">
                                <button data-role="none" data-dismiss="modal" class="btn btn-secondary btn-lg">
                                    Cancel
                                </button>
                                <button data-role="none" id="btnOkMaterialAccUsedUp" class="btn btn-success btn-lg">
                                    Ok
                                </button>
                            </div>
                        </div>
                        <div id="tabMaterialAccUsedUpHistory" class="tab-pane fade">
                            <div class="table-responsive">
                                <table id="" data-role="none" style="width:100%; display:table;" class="table table-light table-striped table-hover">
                                    <thead>
                                        <tr class="custom-theme-f">
                                            <th> Qty </th>
                                            <th> Usage Date </th>
                                            <th> Encoded by </th>
                                            <th> Remarks </th>
                                        </tr>
                                    </thead>
                                    <tbody id="tblBodyAccUsedUp"></tbody>
                                </table>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div id="popupProjectBudgetedLaborCost" class="modal fade" tabindex="-1">
        <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Budgeted Labor Cost</h5>
                    <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                </div>
                <div class="modal-body">
                    <div class="table-responsive">
                        <table id="" data-role="none" style="width:100%; display:table;" class="table table-light table-striped table-hover">
                            <thead>
                                <tr class="custom-theme-f">
                                    <th> Amount </th>
                                    <th> Last Updated </th>
                                    <th> Encoded by </th>
                                </tr>
                            </thead>
                            <tbody id="tblBodyBudgetedLaborCost"></tbody>
                        </table>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div id="popupProjectAccDisbursedCost" class="modal fade" tabindex="-1">
        <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Disburse Labor Cost</h5>
                    <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                </div>
                <div class="modal-body">
                    <div id="divTabDisbursedLaborCost">
                        <ul class="nav nav-tabs" role="tablist" style="flex-wrap:nowrap;">
                            <li class="nav-item" role="presentation">
                                <button data-role="none" class="nav-link active" data-toggle="tab" data-target="#tabAddDisbursedCost">Pay</button>
                            </li>
                            <li class="nav-item" role="presentation">
                                <button data-role="none" class="nav-link" data-toggle="tab" data-target="#tabDisbursedHistory">History</button>
                            </li>
                        </ul>
                    </div>

                    <div class="tab-content p-3">
                        <div id="tabAddDisbursedCost" class="tab-pane fade show active">
                            <input data-role="none" type="text" id="txtLaborReqByID" style="display:none;" />

                            <div class="mb-3">
                                <label class="form-label" for="txtLaborReqAmount">Requested Amount: <span class="required">*</span></label>
                                <input type="text" data-role="none" class="form-control" id="txtLaborReqAmount" />
                            </div>

                            <div class="mb-3">
                                <label class="form-label" for="selLaborRequestedBy">Requested By: <span class="required">*</span></label>
                                <select id="selLaborRequestedBy" data-role="none" class="selReqByProjectMember form-select"></select>
                            </div>

                            <div class="mb-3">
                                <label class="form-label" for="txtLaborPaymentBalance">Payment Balance:</label>
                                <input type="text" data-role="none" class="form-control" id="txtLaborPaymentBalance" readonly />
                            </div>

                            <div class="mb-3">
                                <label class="form-label" for="txtLaborRemarks">Remarks:</label>
                                <textarea data-role="none" class="form-control" id="txtLaborRemarks" style="resize:none; height:100px;"></textarea>
                            </div>

                            <div class="text-center">
                                <button data-role="none" data-dismiss="modal" class="btn btn-secondary btn-lg">
                                    Cancel
                                </button>
                                <button data-role="none" id="btnOkLaborDisburse" class="btn btn-success btn-lg">
                                    Ok
                                </button>
                            </div>
                        </div>
                        <div id="tabDisbursedHistory" class="tab-pane fade">
                            <div class="table-responsive">
                                <table id="" data-role="none" style="width:100%; display:table;" class="table table-light table-striped table-hover">
                                    <thead>
                                        <tr class="custom-theme-f">
                                            <th> Amount </th>
                                            <th> Requested By </th>
                                            <th> Date of Request </th>
                                            <th> Remarks </th>
                                            <th> Balance </th>
                                            <th> Status </th>
                                        </tr>
                                    </thead>
                                    <tbody id="tblBodyLaborPaymentHistory"></tbody>
                                </table>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div id="popProjectDuration" class="modal fade" tabindex="-1">
        <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Budgeted Duration</h5>
                    <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                </div>
                <div class="modal-body">
                    <div class="mb-3">
                        <label class="form-label" for="txtProjectDuration">Duration: <span class="required">*</span></label>
                        <input type="number" data-role="none" class="form-control" id="txtProjectDuration" />
                    </div>

                    <div class="mb-3">
                        <label class="form-label" for="selProjectStateIn">State in:</label>
                        <select id="selProjectStateIn" data-role="none" class="form-select">
                            <option value="days">Day(s)</option>
                            <option value="hours">Hour(s)</option>
                            <option value="minutes">Minute(s)</option>
                        </select>
                    </div>
                </div>
                <div class="modal-footer">
                    <div class="row">
                        <div class="col">
                            <button data-role="none" data-dismiss="modal" class="btn btn-secondary btn-block btn-lg">Cancel</button>
                        </div>
                        <div class="col">
                            <button id="btnOkProjectDuration" data-role="none" class="btn btn-success btn-block btn-lg">Ok</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div id="popupProjectTaskDependency" class="modal fade" tabindex="-1">
        <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Task Dependency</h5>
                    <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                </div>
                <div class="modal-body">
                    Please select task dependency:
                    <br />
                    <ul data-role="none" id="listTaskDependency" class="list-group"></ul>
                </div>
                <div class="modal-footer">
                    <div class="row">
                        <div class="col">
                            <button data-role="none" data-dismiss="modal" class="btn btn-secondary btn-block btn-lg">Cancel</button>
                        </div>
                        <div class="col">
                            <button id="btnOkDependency" data-role="none" class="btn btn-success btn-block btn-lg">Ok</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div id="popupProjectAttachments" class="modal fade" tabindex="-1">
        <div class="modal-dialog modal-xl modal-dialog-centered modal-dialog-scrollable">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Attachments</h5>
                    <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                </div>
                <div class="modal-body">
                    <input data-role="none" type="file" id="fileProjectFiles" multiple style="display:none;" />
                    <div style="text-align:right;">
                        <button data-role="none" id="btnProjectFiles" class="btn btn-success">Add Files</button>
                    </div>
                    <br />
                    <div class="table-responsive">
                        <table data-mode="none" cellpadding="2" cellspacing="0" class="table table-light table-striped table-hover">
                            <thead>
                                <tr>
                                    <th class="custom-theme-f" style="width:10px;"></th>
                                    <th class="custom-theme-f">File Name</th>
                                    <th class="custom-theme-f">File Description</th>
                                    <th class="custom-theme-f" style="width:70px;">Uploaded By</th>
                                    <th class="custom-theme-f" style="width:70px;">Date Uploaded</th>
                                    <th class="custom-theme-f" style="width:10px;"></th>
                                </tr>
                            </thead>
                            <tbody id="tBodyProjecFiles"></tbody>
                        </table>
                    </div>                    
                </div>
            </div>
        </div>
    </div>

    <div id="popupProjectAttachmentsDetails" class="modal fade" tabindex="-1">
        <div class="modal-dialog modal-xl modal-dialog-centered modal-dialog-scrollable">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Files to be uploaded</h5>
                    <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                </div>
                <div class="modal-body">
                    <input data-role="none" type="file" id="fileAddAnotherProjectFiles" multiple style="display:none;" />
                    <div style="text-align:right;">
                        <button data-role="none" id="btnAddAnotherProjectFiles" class="btn btn-success">Add Files</button>
                    </div>
                    <br />
                    <div class="table-responsive">
                        <table data-mode="none" cellpadding="2" cellspacing="0" class="table table-light table-striped table-hover">
                            <thead>
                                <tr>
                                    <th class="custom-theme-f"></th>
                                    <th class="custom-theme-f">File Name</th>
                                    <th class="custom-theme-f">File Description</th>
                                    <th class="custom-theme-f" style="width:50px;"></th>
                                </tr>
                            </thead>
                            <tbody id="tBodyProjecAttachmentsDetails"></tbody>
                        </table>
                    </div>                    
                </div>
                <div class="modal-footer">
                    <div class="row">
                        <div class="col">
                            <button data-role="none" data-dismiss="modal" class="btn btn-secondary btn-block btn-lg">Cancel</button>
                        </div>
                        <div class="col">
                            <button id="btnProjectAttachmentsUpload" data-role="none" class="btn btn-success btn-block btn-lg">Upload Now</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div id="popupProjectNewClient" class="modal fade" tabindex="-1">
        <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">New Client</h5>
                    <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                </div>
                <div class="modal-body">
                    <div class="row">
                        <div class="col-md-3">
                            Client Type:
                        </div>
                        <div class="col-md-9">
                            <select class="form-select" data-role="none" id="selProjectNewClientType">
                                <option value="org">Organization</option>
                                <option value="person">Person</option>
                            </select>
                        </div>
                    </div>
                    <h4>Basic Information</h4>

                    <div id="divProjectNewClientOrg" subpage="pageProjectNewClient" style="display:block;">
                        <div class="row">
                            <div class="col-md-3">
                                Organization Name: <span class="required">*</span>
                            </div>
                            <div class="col-md-9">
                                <input type="text" data-role="none" class="form-control" id="txtProjectNewClientOrgName" />
                            </div>
                        </div>

                        <br />

                        <div class="row">
                            <div class="col-md-3">
                                TIN Number: <span class="required">*</span>
                            </div>
                            <div class="col-md-9">
                                <input type="text" data-role="none" class="form-control" id="txtProjectNewClientOrgTIN" />
                            </div>
                        </div>
                    </div>

                    <div id="divProjectNewClientPerson" subpage="pageProjectNewClient" style="display:none;">
                        <div class="row">
                            <div class="col-md-3">
                                First Name: <span class="required">*</span>
                            </div>
                            <div class="col-md-9">
                                <input type="text" data-role="none" class="form-control" id="txtProjectNewClientPersonFname" />
                            </div>
                        </div>
                        <br />

                        <div class="row">
                            <div class="col-md-3">
                                Middle Name:
                            </div>
                            <div class="col-md-9">
                                <input type="text" data-role="none" class="form-control" id="txtProjectNewClientPersonMname" />
                            </div>
                        </div>
                        <br />

                        <div class="row">
                            <div class="col-md-3">
                                Last Name: <span class="required">*</span>
                            </div>
                            <div class="col-md-9">
                                <input type="text" data-role="none" class="form-control" id="txtProjectNewClientPersonLname" />
                            </div>
                        </div>
                    </div>
                    <h4>Address</h4>

                    <div class="row">
                        <div class="col-md-3">House / Bldg No.:</div>
                        <div class="col-md-9">
                            <input data-role="none" type="text" class="form-control formppol" id="txtProjectNewClientHomeNo">
                        </div>
                    </div>
                    <br />
                    <div class="row">
                        <div class="col-md-3">Street Name:</div>
                        <div class="col-md-9">
                            <input data-role="none" type="text" class="form-control formppol" id="txtProjectNewClientStreetName">
                        </div>
                    </div>
                    <br />
                    <div class="row">
                        <div class="col-md-3">City:</div>
                        <div class="col-md-9">
                            <select data-role="none" class="form-select" id="selProjectNewClientCity"></select>
                        </div>
                    </div>
                    <br />
                    <div class="row">
                        <div class="col-md-3">Country:</div>
                        <div class="col-md-9">
                            <select data-role="none" class="form-select" id="selProjectNewClientCountry"></select>
                        </div>
                    </div>
                    <h4>Contact Info</h4>
                    <div class="row">
                        <div class="col-md-3">Mobile No.:</div>
                        <div class="col-md-3">
                            <select data-role="none" class="form-select" id="selProjectNewClientCountryCode" disabled>
                                <option value="e783ed13-aa98-4cc1-918e-d49b05c0167e">PHL - 63</option>
                            </select>
                        </div>
                        <div class="col-md-6">
                            <input data-role="none" type="text" maxlength="10" id="txtProjectNewClientMobileNo" placeholder="9*********" class="form-control formppol" />
                        </div>
                    </div>
                    <br />
                    <div class="row">
                        <div class="col-md-3">Email:</div>
                        <div class="col-md-9">
                            <input data-role="none" type="email" class="form-control formppol" id="txtProjectNewClientEmail">
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <div class="row">
                        <div class="col">
                            <button data-role="none" data-dismiss="modal" class="btn btn-secondary btn-block btn-lg">Cancel</button>
                        </div>
                        <div class="col">
                            <button id="btnProjectSaveNewClient" data-role="none" class="btn btn-success btn-block btn-lg">Save</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div id="" custom-data-role="popup">
        <div class="popup-md">
            <div class="popup-header">
                
                <a class="popup-close">x</a>
            </div>
            <div class="popup-content">
                
            </div>
            <div class="popup-footer">
                <button data-role="none" class="btn custom-btn-theme-c btnCircle btnShadow popup-close">
                    <img class="img-cancel" />
                </button>
                <button data-role="none" id="" class="btn custom-btn-theme-b btnCircle btnShadow">
                    <img class="img-save" />
                </button>
            </div>
        </div>
    </div>
</div>
@*End Pop Ups*@

<div id="mainContainer">

    <div id="divProjectMgmtTab">
        <ul class="nav nav-tabs" role="tablist" style="flex-wrap:nowrap;">
            <li class="nav-item" role="presentation">
                <button data-role="none" class="nav-link active" data-toggle="tab" data-target="#tabProjectMgmtDashboard">Dashboard</button>
            </li>
            <li class="nav-item" role="presentation">
                <button data-role="none" id="btnProjectMgmtWork" class="nav-link" data-toggle="tab" data-target="#tabProjectMgmtWork">Project Management</button>
            </li>
            <li class="nav-item" role="presentation">
                <button data-role="none" class="nav-link" data-toggle="tab" data-target="#tabProjectMgmtSettings">Settings</button>
            </li>
        </ul>
    </div>

    <div class="tab-content p-3">
        <div id="tabProjectMgmtDashboard" class="tab-pane fade show active">
            <div data-role="content">
                <div class="row">
                    <div class="col-xs-12 col-sm-6 col-md-3 mb-3">
                        <div class="dashboardGrid bg-light border p-3 rounded h-100 btnLogExpenseReport">
                            <div class="gHeader">
                                Number of All Projects
                            </div>
                            <div class="gBody">
                                0
                            </div>
                            <div class="gFooter"></div>
                        </div>
                    </div>
                    <div class="col-xs-12 col-sm-6 col-md-3 mb-3">
                        <div class="dashboardGrid bg-light border p-3 rounded h-100 btnLogExpenseReport">
                            <div class="gHeader">
                                Number of Open Projects
                            </div>
                            <div class="gBody">
                                0
                            </div>
                            <div class="gFooter"></div>
                        </div>
                    </div>
                    <div class="col-xs-12 col-sm-6 col-md-3 mb-3">
                        <div class="dashboardGrid bg-light border p-3 rounded h-100 btnLogExpenseReport">
                            <div class="gHeader">
                                Number of Completed Projects
                            </div>
                            <div class="gBody">
                                0
                            </div>
                            <div class="gFooter"></div>
                        </div>
                    </div>
                    <div class="col-xs-12 col-sm-6 col-md-3 mb-3">
                        <div class="dashboardGrid bg-light border p-3 rounded h-100 btnLogExpenseReport">
                            <div class="gHeader">
                                Number of Cancelled Projects
                            </div>
                            <div class="gBody">
                                0
                            </div>
                            <div class="gFooter"></div>
                        </div>
                    </div>
                    <div class="col-xs-12 col-sm-6 col-md-3 mb-3">
                        <div class="dashboardGrid bg-light border p-3 rounded h-100 btnLogExpenseReport">
                            <div class="gHeader">
                                Number of Overdue Projects
                            </div>
                            <div class="gBody">
                                0
                            </div>
                            <div class="gFooter"></div>
                        </div>
                    </div>
                    <div class="col-xs-12 col-sm-6 col-md-3 mb-3">
                        <div class="dashboardGrid bg-light border p-3 rounded h-100 btnLogExpenseReport">
                            <div class="gHeader">
                                Number of Completed & Overdue Projects
                            </div>
                            <div class="gBody">
                                0
                            </div>
                            <div class="gFooter"></div>
                        </div>
                    </div>
                    <div class="col-xs-12 col-sm-6 col-md-3 mb-3">
                        <div class="dashboardGrid bg-light border p-3 rounded h-100 btnLogExpenseReport">
                            <div class="gHeader">
                                Number of Completed & Under Projects
                            </div>
                            <div class="gBody">
                                0
                            </div>
                            <div class="gFooter"></div>
                        </div>
                    </div>
                    @*<div class="col-xs-12 col-sm-6 col-md-3 mb-3">
                        <div class="dashboardGrid bg-light border p-3 rounded h-100 btnLogExpenseReport">
                            <div class="gHeader">
                                Number of Cancelled Projects
                            </div>
                            <div class="gBody">
                                0
                            </div>
                            <div class="gFooter"></div>
                        </div>
                    </div>*@
                </div>
            </div>
        </div>

        <div id="tabProjectMgmtWork" class="tab-pane fade">
            <div class="row gx-1 mb-4">
                <div class="col-md-7 col-lg-8 mb-3">
                    <div name="divToolbar" class="btn-group w-100 overflow-scroll" role="group" aria-label="Basic example">
                        <button data-role="none" id="btnProjectNew" class="btn btn-light border h-100 w-100 align-self-auto">
                            <span class="bi-file-earmark-plus" style="font-size:25px;"></span>
                            <div class="menu-description mt-1">New</div>
                        </button>
                        <button data-role="none" id="btnProjectLookUp" class="btn btn-light border h-100 w-100">
                            <span class="bi-search" style="font-size:25px;"></span>
                            <div class="menu-description mt-1">Lookup</div>
                        </button>
                        <button data-role="none" id="btnNewProjectBudget" class="btn btn-light border h-100 w-100">
                            <span class="bi-pencil-square" style="font-size:25px;"></span>
                            <div class="menu-description mt-1">Edit</div>
                        </button>
                        <button data-role="none" id="btnSaveProjectBudget" class="btn btn-light border h-100 w-100">
                            <span class="bi-save" style="font-size:25px;"></span>
                            <div class="menu-description mt-1">Save</div>
                        </button>
                        <button data-role="none" id="btnPrintProjectBudget" class="btn btn-light border h-100 w-100">
                            <span class="bi-printer" style="font-size:25px;"></span>
                            <div class="menu-description mt-1">Output</div>
                        </button>
                        <button data-role="none" id="btnProjectAttachments" class="btn btn-light border h-100 w-100">
                            <span class="bi-paperclip" style="font-size:25px;"></span>
                            <div class="menu-description mt-1">Attachments</div>
                        </button>
                        <button data-role="none" id="btnProjectAlert" class="btn btn-light border h-100 w-100">
                            <span class="bi-bell" style="font-size:25px;"></span>
                            <div class="menu-description mt-1">Alert</div>
                        </button>
                        <button data-role="none" id="butCancel" class="btn btn-light border h-100 w-100">
                            <span class="bi-x-square" style="font-size:25px;"></span>
                            <div class="menu-description mt-1">Cancel</div>
                        </button>
                    </div>
                </div>
                <div id="divProjectTitle" class="col-md-5 col-lg-4 text-right" style="font-size:30px;">
                </div>
            </div>

            <div class="row align-items-center mb-3">
                <div class="col-md">
                    <select id="selFilterProjectSummary" data-role="none" class="form-select fixed">
                        <option value="overall">Overall Summary</option>
                        <option value="timespend">Time Spend</option>
                        <option value="materialspend">Material Spend</option>
                        <option value="laborspend">Labor Spend</option>
                    </select>
                </div>
                <div class="col-md">
                    <div class="divNewProjectBudget text-right" style="display:none;">
                        <button data-role="none" class="btn btn-secondary btnDisplayAssociatedSKU" state="show">Show Linked SKU</button>
                        <div class="form-check form-check-inline">
                            <label class="form-check-label">
                                <input type="checkbox" id="chbShowPopup" data-role="none" class="form-check-input" value="1" />
                                Show popup when click
                            </label>
                        </div>
                    </div>                    
                </div>
            </div>

            <div>
                <div id="divViewProjectBudget" style="font-weight: bold; display:block;">
                    <br />
                    <div class="floatingDiv-bottomcenter" style="display:none;">
                        <button data-role="none" id="btnBackProjectBudget" class="btn custom-btn-theme-c btnCircle btnShadow">
                            <img class="img-arrow-left" style="width:18px;" />
                        </button>
                        <button data-role="none" class="btn custom-btn-theme-e btnCircle btnShadow btnDisplayAssociatedSKU">
                            <img class="img-eye-open" style="width:18px;" />
                        </button>
                        <button data-role="none" id="btnPrintProjectBudget" class="btn custom-btn-theme-d btnCircle btnShadow">
                            <img class="img-printer-white" style="width:18px;" />
                        </button>
                        <button data-role="none" id="" class="btn custom-btn-theme-b btnCircle btnShadow">
                            <img class="img-edit" style="width:18px;" />
                        </button>
                    </div>

                    <div id="divProjectBudgetSummary" class="table-scroll">
                        <div>
                            <table id="tblProjectBudget" data-mode="none" cellpadding="2" cellspacing="0" class="table table-light table-striped table-hover" width="100%">
                                <thead>
                                    <tr>
                                        <th class="custom-theme-f tdfreeze thfreeze" rowspan="3" style="text-align:center; min-width:26px; margin:0;"></th>
                                        <th class="custom-theme-f tdfreeze thfreeze" rowspan="3" style="text-align:center; min-width:100px; max-width:100px; left:26px;">ITEM No.</th>
                                        <th class="custom-theme-f tdfreeze thfreeze" rowspan="3" style="min-width: 300px; max-width:300px; text-align:center; left:120px;">DESCRIPTION</th>
                                        <th class="custom-theme-f" rowspan="3" style="min-width: 150px; max-width:150px; text-align:center;">Point Person</th>
                                        <th class="custom-theme-f" rowspan="3" style="min-width: 150px; max-width:150px; text-align:center;">Remarks</th>
                                        <th class="thFormat cell-timespend" style="min-width: 150px; max-width: 140px; ">Completion Rate</th>
                                        <th class="thFormat cell-timespend" style="min-width: 110px; max-width: 110px;">Dependency</th>
                                        <th class="thFormat cell-timespend" style="min-width: 165px; max-width: 165px;">Budgeted Duration</th>
                                        <th class="thFormat cell-timespend" style="min-width: 140px; max-width: 140px;">Actual Duration</th>
                                        <th class="thFormat cell-timespend" style="min-width: 160px; max-width: 160px;">Date/ Time Started</th>
                                        <th class="thFormat cell-timespend" style="min-width: 190px; max-width: 190px;">Date/ Time Completed</th>
                                        <th class="thFormat cell-timespend" style="min-width: 110px; max-width: 110px;">Importance</th>
                                        <th class="thFormat cell-timespend" style="min-width: 110px; max-width: 110px;">Urgency</th>
                                        <th class="thFormat cell-materialspend" style="min-width: 120px; max-width: 120px;">Unit Measure</th>
                                        <th class="thFormat cell-materialspend" style="min-width: 125px; max-width: 125px;">Budgeted Qty</th>
                                        <th class="thFormat cell-materialspend" style="min-width: 180px; max-width: 180px;">Latest Requested Qty</th>
                                        <th class="thFormat cell-materialspend" style="min-width: 170px; max-width: 170px;">Total Requested Qty</th>
                                        <th class="thFormat cell-materialspend" style="min-width: 170px; max-width: 170px;">Total Delivered Qty</th>
                                        <th class="thFormat cell-materialspend" style="min-width: 170px; max-width: 170px;">Total Used Up Qty</th>
                                        <th class="thFormat cell-laborspend" style="min-width: 175px; max-width: 175px;">Budgeted Labor Cost</th>
                                        <th class="thFormat cell-laborspend" style="min-width: 215px; max-width: 215px;">Total Requested Advances</th>
                                        <th class="thFormat cell-laborspend" style="min-width: 210px; max-width: 210px;">Total Disbursed</th>
                                    </tr>
                                </thead>
                                <tbody id="tBodyProjectBudget"></tbody>
                            </table>
                        </div>
                    </div>

                    <div id="divPrintProjectBudget" style="display:none; visibility:hidden;">
                        <table>
                            <tr>
                                <td>Project Name:</td>
                                <td><span class="spantxtProjectName"></span></td>
                            </tr>
                            <tr>
                                <td>Project Code:</td>
                                <td><span class="spantxtProjectCode"></span></td>
                            </tr>
                            <tr>
                                <td>Project Description:</td>
                                <td><span class="spantxtProjectDescription"></span></td>
                            </tr>
                            <tr>
                                <td>Project Address:</td>
                                <td><span class="spantxtProjectAddress"></span></td>
                            </tr>
                        </table>
                        <br />
                        <table border="1" cellpadding="5" cellspacing="0" width="100%">
                            <thead class="custom-theme-h">
                                <tr>
                                    <th class="TableHeader">Item No.</th>
                                    <th class="TableHeader">Description</th>
                                    <th class="TableHeader">Point Person</th>
                                    <th class="TableHeader">Completion Rate</th>
                                    <th class="TableHeader">Dependency</th>
                                    <th class="TableHeader">Date Started</th>
                                    <th class="TableHeader">Date Completed</th>
                                    <th class="TableHeader">Importance</th>
                                    <th class="TableHeader">Urgency</th>
                                </tr>
                            </thead>

                            <tbody id="tBodyPrintProjectBudget"></tbody>
                        </table>
                    </div>

                </div>

                <div id="divProjectsTab" class="custom-tab sm hidetab" style="display:none;">
                    <ul>
                        <li class="active" custom-tab-content="#tabProjects">Projects</li>
                        <li id="btnTabMaterialsReq" custom-tab-content="#tabMaterialsReq">Materials Requisiton</li>
                        <li id="btnTabProjectPO" custom-tab-content="#tabProjectPO">Purchase Orders</li>
                    </ul>
                </div>

                <div class="divNewProjectBudget" style="display:none;">
                    <br />
                    <div id="SubdivNewProjectBudget" class="table-scroll">
                        <table id="tblNewProjectBudget" data-mode="none" cellpadding="2" cellspacing="0" class="table table-light table-hover" width="100%">
                            <thead>
                                <tr>
                                    <th class="custom-theme-f tdfreeze thfreeze" rowspan="3" style="text-align:center; min-width:26px; margin:0;"></th>
                                    <th class="custom-theme-f tdfreeze thfreeze" rowspan="3" style="text-align:center; min-width:100px; max-width:100px; left:26px;">ITEM No.</th>
                                    <th class="custom-theme-f tdfreeze thfreeze" rowspan="3" style="min-width: 300px; max-width:300px; text-align:center; left:120px;">DESCRIPTION</th>
                                    <th class="custom-theme-f" rowspan="3" style="min-width: 150px; max-width:150px; text-align:center;">Point Person</th>
                                    <th class="custom-theme-f" rowspan="3" style="min-width: 150px; max-width:150px; text-align:center;">Remarks</th>
                                    <th class="thFormat cell-timespend" style="min-width: 150px; max-width: 140px; ">Completion Rate</th>
                                    <th class="thFormat cell-timespend" style="min-width: 110px; max-width: 110px;">Dependency</th>
                                    <th class="thFormat cell-timespend" style="min-width: 165px; max-width: 165px;">Budgeted Duration</th>
                                    <th class="thFormat cell-timespend" style="min-width: 140px; max-width: 140px;">Actual Duration</th>
                                    <th class="thFormat cell-timespend" style="min-width: 160px; max-width: 160px;">Date/ Time Started</th>
                                    <th class="thFormat cell-timespend" style="min-width: 190px; max-width: 190px;">Date/ Time Completed</th>
                                    <th class="thFormat cell-timespend" style="min-width: 110px; max-width: 110px;">Importance</th>
                                    <th class="thFormat cell-timespend" style="min-width: 110px; max-width: 110px;">Urgency</th>
                                    <th class="thFormat cell-materialspend" style="min-width: 120px; max-width: 120px;">Unit Measure</th>
                                    <th class="thFormat cell-materialspend" style="min-width: 125px; max-width: 125px;">Budgeted Qty</th>
                                    <th class="thFormat cell-materialspend" style="min-width: 180px; max-width: 180px;">Latest Requested Qty</th>
                                    <th class="thFormat cell-materialspend" style="min-width: 170px; max-width: 170px;">Total Requested Qty</th>
                                    <th class="thFormat cell-materialspend" style="min-width: 170px; max-width: 170px;">Total Delivered Qty</th>
                                    <th class="thFormat cell-materialspend" style="min-width: 170px; max-width: 170px;">Total Used Up Qty</th>
                                    <th class="thFormat cell-laborspend" style="min-width: 175px; max-width: 175px;">Budgeted Labor Cost</th>
                                    <th class="thFormat cell-laborspend" style="min-width: 215px; max-width: 215px;">Total Requested Advances</th>
                                    <th class="thFormat cell-laborspend" style="min-width: 210px; max-width: 210px;">Total Disbursed</th>
                                </tr>
                            </thead>

                            <tbody id="tBodyExcelLike"></tbody>
                        </table>
                    </div>

                    <div class="floatingDiv-bottomcenter" style="display:none;">
                        <button data-role="none" id="btnCancelProjectBudget" class="btn custom-btn-theme-c btnCircle btnShadow"><img class="img-cancel" style="width:18px;" /></button>
                        <button data-role="none" id="btnSaveProjectBudgetX" class="btn custom-btn-theme-d btnCircle btnShadow"><img class="img-save" style="width:18px;" /></button>
                        <button data-role="none" class="btn custom-btn-theme-b btnCircle btnShadow btnMoreOptionProjectBudget"><img class="img-settings" style="width:18px;" /></button>
                    </div>
                </div>
            </div>

            <div id="tabProjects" custom-tab-role="content" style="display:block;">
                <div subpage="tabProjectMgmtWork" @*custom-data-role="page"*@ id="pageProjectList" style="display:none;">
                    <div data-role="content">

                    </div>
                </div>

                <div subpage="tabProjectMgmtWork" @*custom-data-role="page"*@ id="pageProjectBudget" style="display:block;">

                    <div data-role="content" style="display:none;">
                        <div data-role="fieldcontain">
                            <fieldset data-role="controlgroup" data-type="horizontal">
                                <legend>
                                    Project Name:
                                </legend>
                                <span class="spantxtProjectName" style="font-size:25px;">

                                </span>
                                <a id="btnEditProjectDetails" data-role="none" class="custom-link">Edit Details</a>
                            </fieldset>
                        </div>
                        <div data-role="fieldcontain">
                            <fieldset data-role="controlgroup" data-type="horizontal">
                                <legend>
                                    Project Code:
                                </legend>
                                <span class="spantxtProjectCode" style="font-size:18px;">

                                </span>
                            </fieldset>
                        </div>
                        <div data-role="fieldcontain">
                            <fieldset data-role="controlgroup" data-type="horizontal">
                                <legend>
                                    Project Description:
                                </legend>
                                <span class="spantxtProjectDescription" style="font-size:18px;">

                                </span>
                            </fieldset>
                        </div>
                        <div data-role="fieldcontain">
                            <fieldset data-role="controlgroup" data-type="horizontal">
                                <legend>
                                    Project Address:
                                </legend>
                                <span class="spantxtProjectAddress" style="font-size:18px;">

                                </span>
                            </fieldset>
                        </div>
                    </div>

                    @*<div class="custom-tab">
                        <ul>
                            <li class="active" custom-tab-content="#divProjectBudget">Project Budget</li>
                            <li class="btnCanvassMaterials" custom-tab-content="#divCanvassMaterials">Canvass Materials</li>
                            <li class="btnCanvassLabor" custom-tab-content="#divCanvassLabor">Canvass Labor</li>
                            <li>Timeline</li>
                        </ul>
                    </div>*@

                    <div id="divProjectBudget" custom-tab-role="content" style="display:block">


                        <div id="divButtons" data-role="content" style="display:none;">
                            @*<div id="divTabProjectBudgetEdit" class="custom-tab">
                                <ul>
                                    <li class="active" custom-tab-content="">Project Summary</li>
                                    <li custom-tab-content=".divCanvass">Material Cost</li>
                                    <li custom-tab-content="">Labor Cost</li>
                                    <li custom-tab-content="">Timeline</li>
                                </ul>
                            </div>*@
                            <div style="float:left;">
                                <button data-role="none" id="btnProjectSummary" class="btn custom-btn-theme-b btn-less-width custom-btn-tab">Project Summary</button>
                                <button data-role="none" id="btnSubCanvassMaterials" class="btn custom-btn-theme-c btn-less-width btnCanvassMaterials custom-btn-tab">Material Cost</button>
                                <button data-role="none" id="btnSubCanvassLabor" class="btn custom-btn-theme-c btn-less-width btnCanvassLabor custom-btn-tab">Labor Cost</button>
                                <button data-role="none" id="btnTimeline" class="btn custom-btn-theme-c btn-less-width custom-btn-tab">Timeline</button>
                            </div>
                            <div class="divButtonsRight" style="float:right;">
                                <div style="none">
                                    <button data-role="none" style="display:none;" id="btnComputeParentSubTotal" class="btn custom-btn-theme-b btn-less-width">Compute Sub Total</button>
                                    <button data-role="none" style="display:none;" id="btnPrintCanvassSheets2" class="btn custom-btn-theme-b btn-less-width">Print Canvass Sheets</button>
                                    <button data-role="none" style="display:none;" id="btnTimeline2" class="btn custom-btn-theme-b btn-less-width">Timeline</button>
                                    <button data-role="none" style="display:none;" id="btnCanvassOptions" class="btn custom-btn-theme-b btn-less-width">Canvass</button>
                                    <button data-role="none" style="display:none;" class="btn custom-btn-theme-b btn-less-width btnDisplayAssociatedSKU2">Show SKU</button>
                                </div>
                            </div>
                            <div style="clear:both;"></div>
                        </div>


                    </div>

                    <div class="divCanvass" custom-tab-role="contents" style="display:none;">
                        <div data-role="content">
                            <div>
                                Legends:
                                <span style="padding:10px; text-align:center; background-color:#EAEAEA">Read Only</span>
                                <span style="padding:10px; text-align:center; background-color:#FFE991">Preferred</span>
                                <span style="padding:10px; text-align:center; background-color:#91CBEF">Min Price</span>
                            </div>
                            <br />
                            <div style="overflow:auto; display:block;" id="divCanvassMaterials">
                                <table data-mode="reflow" cellpadding="3" cellspacing="0" class="custom-table table-stroke" width="100%">
                                    <thead id="tHeadCanvassMaterial">
                                        <tr class="custom-theme-f">
                                            <th rowspan="2" style="min-width: 80px; max-width:80px;">ITEM No.</th>
                                            <th rowspan="2" style="min-width: 300px; max-width:300px;">DESCRIPTION</th>
                                            <th rowspan="2" style="min-width: 100px; max-width:100px;">QTY</th>
                                            <th rowspan="2" style="min-width: 100px; min-width:100px;">UNIT</th>
                                        </tr>
                                        <tr style="background-color:#D2691E; color:#FFF; text-shadow:none;" id="tHeadCanvassMaterialRow"></tr>
                                    </thead>

                                    <tbody id="tBodyCanvassMaterial"></tbody>
                                </table>
                            </div>
                            <div style="overflow:auto; display:block;" id="divCanvassLabor">
                                <table data-mode="reflow" cellpadding="3" cellspacing="0" class="custom-table table-stroke" width="100%">
                                    <thead id="tHeadCanvassLabor">
                                        <tr class="custom-theme-f">
                                            <th rowspan="2" style="min-width: 80px; max-width:80px;">ITEM No.</th>
                                            <th rowspan="2" style="min-width: 300px; max-width:300px;">DESCRIPTION</th>
                                            <th rowspan="2" style="min-width: 100px; max-width:100px;">QTY</th>
                                            <th rowspan="2" style="min-width: 100px; min-width:100px;">UNIT</th>
                                        </tr>
                                        <tr style="background-color:#A0522D; color:#FFF; text-shadow:none;" id="tHeadCanvassLaborRow"></tr>
                                    </thead>

                                    <tbody id="tBodyCanvassLabor"></tbody>
                                </table>
                            </div>

                            <div class="divCanvass floatingDiv-bottomcenter" style="display:none">
                                <button data-role="none" id="btnSaveCanvassMaterials" class="btn custom-btn-theme-d btnCircle btnShadow"><img alt="Save" src="/Content/images/check.png" style="width:18px;" /></button>
                                <button data-role="none" class="btn custom-btn-theme-b btnCircle btnShadow btnMoreOptionProjectBudget"><img alt="Option" src="/Content/images/settings.png" style="width:18px;" /></button>
                            </div>
                        </div>
                    </div>

                    <div id="divPrintCanvass" style="display:none; visibility:hidden;">
                        <table style="width:100%;">
                            <tr>
                                <td id="txtCompanyName" rowspan="4" class="CompanyName">ABC Company</td>
                            </tr>
                            <tr>
                                <td id="txtBusinessAddress" class="tdBusinessAddress">Business Address</td>
                            </tr>
                            <tr>
                                <td class="tdBusinessContact">
                                    <span id="txtTelephoneNo">No Tel. no</span> /
                                    <span id="txtFaxNo">No Fax no.</span> /
                                    <span id="txtMobileNum">No Mobile no.</span>
                                </td>
                            </tr>
                            <tr>
                                <td id="txtBusinessEmail" class="tdBusinessEmail">Email Address</td>
                            </tr>
                        </table>
                        <br />
                        <table style="width:100%;">
                            <tr>
                                <td class="tdForPriceQuotation">FOR PRICE QUOTATION</td>
                            </tr>
                        </table>
                        <br />
                        <table style="width:100%; font-size:14px;">
                            <tr>
                                <td class="tdProjectName">Project Name:</td>
                                <td><strong id="txtPrintCanvassProjectName"></strong></td>
                            </tr>
                            <tr>
                                <td class="tdProjectAddress">Project Address:</td>
                                <td><strong id="txtPrintCanvassProjectAddress"></strong></td>
                            </tr>
                            <tr>
                                <td class="tdVendorName">Vendor Name:</td>
                                <td><strong id="txtPrintCanvassVendorName"></strong></td>
                            </tr>
                        </table>
                        <br />
                        <table border="0" cellpadding="5" cellspacing="0" width="100%">
                            <thead>
                                <tr class="custom-theme-f">
                                    <th class="TableHeader FixedTableHeaderWidth">Item No.</th>
                                    <th class="TableHeader">Description</th>
                                    <th class="TableHeader FixedTableHeaderWidth">Qty</th>
                                    <th class="TableHeader FixedTableHeaderWidth">Unit</th>
                                    <th class="TableHeader FixedTableHeaderWidth">Cost</th>
                                </tr>
                            </thead>

                            <tbody id="tBodyPrintCanvass"></tbody>
                        </table>
                    </div>

                    <div class="divTimeline" style="display:none;">
                        <div data-role="content">
                            <div id="divTimelineContainer" style="overflow:auto;">
                                <table id="tblTimeline" data-mode="reflow" cellpadding="3" cellspacing="0" class="custom-table table-stroke NoHighLight" width="100%" style="table-layout:fixed;">
                                    <thead class="custom-theme-d">
                                        <tr>
                                            <th style="width:100px;">Item #</th>
                                            <th style="width:300px;">Description</th>
                                            <th style="width:160px;">Importance</th>
                                            <th style="width:150px;">Urgency</th>
                                            <th style="width:200px;">Time Needed to Complete</th>
                                            <th style="width:200px;">Assigned To</th>
                                            <th style="width:100px;">Status</th>
                                            <th style="width:150px;">Dependency</th>
                                            <th style="width:150px;">Tags</th>
                                        </tr>
                                    </thead>

                                    <tbody id="tBodyTimeline"></tbody>
                                </table>
                            </div>

                            <div class="divTimeline floatingDiv-bottomcenter" style="display:none">
                                <button data-role="none" id="btnTimelineOk" class="btn custom-btn-theme-d btnCircle btnShadow"><img class="img-check-white" style="width:18px;" /></button>
                            </div>
                        </div>
                    </div>

                </div>

                <div subpage="tabProjectMgmtWork" @*custom-data-role="page"*@ id="pageProjectDetails" style="display:none;">
                    <div data-role="content">

                    </div>
                </div>
            </div>

            <div id="tabMaterialsReq" custom-tab-role="content" style="display:none;">
                Loading ...
            </div>

            <div id="tabProjectPO" custom-tab-role="content" style="display:none;">
                Loading ...
            </div>
        </div>

        <div id="tabProjectMgmtSettings" class="tab-pane fade">

        </div>
    </div>       

    <div custom-data-role="page" id="pageSystemAdmin" style="display:none;">
        <div data-role="content">
            <div id="divSystemAdDashboard" style="display:block;">
                <div id="divForRequiredData">
                    <div>
                        <div class="custom-theme-f" style="padding:10px; font-size:24px; font-weight:bold;">
                            Required Data
                        </div>
                        <h4>
                            Please make sure that you provide the correct information on the things below.  Mobiz will not work properly without these required setup.
                        </h4>
                    </div>
                    <div class="ui-grid-a" style="padding:0px; overflow:visible;">
                        <div class="ui-block-a" style="width:60%; float:left;">
                            <div id="btnSetupOrganization" class="btn custom-btn-theme-c border-theme-c divSetupOrganization" style="margin:0px; padding:0px;" data-role="none" data-theme="c" data-corners="false">
                                <div style="padding:10px;">
                                    <div style="text-align:left; margin-top:28px;"><span>Organization</span></div>
                                    <p style="text-align:left; white-space: normal; font-size:smaller; font-weight:lighter;">Click here to add more information about your organization. This information may be published to an online business directory, used for system-generated General Information Sheet (GIS) for the SEC or DTI, and/or other value-added services in the future. Your consent must be given before Mobiz publishes any of these information to the public.</p>
                                </div>
                            </div>
                        </div>
                        <div class="ui-block-b" style="width:40%; float:right;">
                            <div id="btnSetupOrganizationVal" class="custom-theme-c border-theme-c divSetupOrganizationVal" style="margin:0px; padding:0px; height:208px;" data-role="none" data-theme="c" data-corners="false">
                                <div style="text-align:center; padding:10px;">
                                    <div><img id="imgOrg" src="/Content/images/tick.png" style="width: 30px; height: 30px;"></div>
                                    <div><label id="lblOrgLastUpdated">(08/20/2014)</label></div>
                                </div>
                            </div>
                        </div>
                        <div style="clear:both;"></div>
                    </div>
                    <div class="ui-grid-a" style="padding:0px; overflow:visible;">
                        <div class="ui-block-a" style="width:60%; float:left;">
                            <div id="btnSetupOrganizationFOWST" class="btn custom-btn-theme-c border-theme-c divSetupOrganizationFOWST" style="margin:0px; padding:0px;" data-role="none" data-theme="c" data-corners="false">
                                <div style="padding:10px;">
                                    <div style="text-align:left; margin-top:28px;"><span>FOWST Locations</span></div>
                                    <p style="text-align:left; white-space: normal; font-size:smaller; font-weight:lighter;">
                                        At least one (1) record of a FOWST (Factory, Office, Warehouse, Store or Truck) is needed.
                                        Each FOWST location keeps track of its own inventory movements and balances. If you want a
                                        location to have its own inventory ledger (of movements and balances), you must add a FOWST
                                        location record for it.
                                    </p>
                                </div>
                            </div>
                        </div>
                        <div class="ui-block-b" style="width:40%; float:right;">
                            <div id="" class="custom-theme-c border-theme-c divSetupOrganizationFOWSTVal" style="margin:0px; padding:0px;" data-role="none" data-theme="c" data-corners="false">
                                <div style="text-align:center; padding:10px; height:62px;">
                                    <div><img id="imgFOWST" src="/Content/images/cross.png" style="width: 30px; height: 30px;"></div>
                                    <div><label id="lblFOWSTCount">0</label></div>
                                    <div><label id="lblFOWSTLastUpdated">(08/20/2014)</label></div>
                                </div>
                            </div>
                        </div>
                        <div style="clear:both;"></div>
                    </div>
                    <div class="ui-grid-a" style="padding:0px; overflow:visible;">
                        <div class="ui-block-a" style="width:60%; float:left;">
                            <div id="btnSetupSecurity" class="btn custom-btn-theme-c border-theme-c divSetupSecurity" style="margin:0px; padding:0px;" data-role="none" data-theme="c" data-corners="false">
                                <div style="padding:10px;">
                                    <div style="text-align:left; margin-top:28px;"><span>Security Roles</span></div>
                                    <p style="text-align:left; white-space: normal; font-size:smaller; font-weight:lighter;">Add  or edit existing roles in your organization for fine-tuning staff's access rights to Mobiz apps.</p>
                                </div>
                            </div>
                        </div>
                        <div class="ui-block-b" style="width:40%; float:right;">
                            <div id="btnSetupSecurityVal" class="custom-theme-c border-theme-c divSetupSecurityVal" style="margin:0px; padding:0px; height:145px;" data-role="none" data-theme="c" data-corners="false">
                                <div style="text-align:center; padding:10px;">
                                    <div><img id="imgSecurityAccess" src="/Content/images/cross.png" style="width: 30px; height: 30px;"></div>
                                    <div><label id="lblSecurityAccessCount">0</label></div>
                                    <div><label id="lblSecurityAccessLastUpdated">(08/20/2014)</label></div>
                                </div>
                            </div>
                        </div>
                        <div style="clear:both;"></div>
                    </div>
                    <div class="ui-grid-a" style="padding:0px; overflow:visible;">
                        <div class="ui-block-a" style="width:60%; float:left;">
                            <div id="btnSetupEmployee" class="btn custom-btn-theme-c border-theme-c divSetupEmployee" style="margin:0px; padding:0px;" data-role="none" data-theme="c" data-corners="false">
                                <div style="padding:10px;">
                                    <div style="text-align:left; margin-top:28px;"><span>Employees</span></div>
                                    <p style="text-align:left; white-space: normal; font-size:smaller; font-weight:lighter;">Each employee must be assigned his or her own user name/password and Security Role (cashier, manager, etc.) before he/she can use Mobiz apps.</p>
                                </div>
                            </div>
                        </div>
                        <div class="ui-block-b" style="width:40%; float:right;">
                            <div id="btnSetupEmployeeVal" class="custom-theme-c border-theme-c divSetupEmployeeVal" style="margin:0px; padding:0px; height:145px;" data-role="none" data-theme="c" data-corners="false">
                                <div style="text-align:center; padding:10px;">
                                    <div><img id="imgEmployeeAccess" src="/Content/images/cross.png" style="width: 30px; height: 30px;"></div>
                                    <div><label id="lblEmployeeCount">0</label></div>
                                    <div><label id="lblEmployeeLastUpdated">(08/20/2014)</label></div>
                                </div>
                            </div>
                        </div>
                        <div style="clear:both;"></div>
                    </div>
                    <div class="ui-grid-a" style="padding:0px; overflow:visible;">
                        <div class="ui-block-a" style="width:60%; float:left;">
                            <div id="btnSetupProducts" class="btn custom-btn-theme-c border-theme-c divSetupProducts" style="margin:0px; padding:0px;" data-role="none" data-theme="c" data-corners="false">
                                <div style="padding:10px;">
                                    <div style="text-align:left; margin-top:28px;"><span>PIM <span style="font-size:14px;">(Product Information Management)</span></span></div>
                                    <p style="text-align:left; white-space: normal; font-size:smaller; font-weight:lighter;">At least one (1) product is needed. You may add your products via Excel upload or you may choose from the publicly available product database of Mobiz.</p>
                                </div>
                            </div>
                        </div>
                        <div class="ui-block-b" style="width:40%; float:right;">
                            <div id="btnSetupProductsVal" class="custom-theme-c border-theme-c divSetupProductsVal" style="margin:0px; padding:0px; height:145px;" data-role="none" data-theme="c" data-corners="false">
                                <div style="text-align:center; padding:10px;">
                                    <div><img id="imgProduct" src="/Content/images/cross.png" style="width: 30px; height: 30px;"></div>
                                    <div><label id="lblProductCount">0</label></div>
                                    <div><label id="lblProductLastUpdated">(08/20/2014)</label></div>
                                </div>
                            </div>
                        </div>
                        <div style="clear:both;"></div>
                    </div>
                </div>

                <div id="divForOptionalData">
                    <br />
                    <div class="ui-grid-solo" style="padding:0px; overflow:visible;">
                        <div class="ui-block-a">
                            <div class="custom-theme-f" style="padding:10px; font-size:24px; font-weight:bold;">
                                Optional Data
                            </div>
                            <h4>
                                Add these optional data to enhance the functions of Mobiz Complete Retail's different modules.
                            </h4>
                        </div>
                    </div>
                    <div class="ui-grid-a" style="padding:0px; overflow:visible;">
                        <div class="ui-block-a" style="width:60%; float:left;">
                            <div id="btnSetupSupplier" class="btn custom-btn-theme-c border-theme-c" style="margin:0px; padding:0px; height:82px;" data-role="none" data-theme="c" data-corners="false">
                                <div style="padding:10px;">
                                    <p style="text-align:left; white-space: normal;">Supplier / Vendor</p>
                                </div>
                            </div>
                        </div>
                        <div class="ui-block-b" style="width:40%; float:right;">
                            <div id="" class="custom-theme-c border-theme-c" style="margin:0px; padding:0px;" data-role="none" data-theme="c" data-corners="false">
                                <div style="text-align:center; padding:10px; height:62px;">
                                    <div><img id="imgSupplier" src="/Content/images/tick.png" style="width: 30px; height: 30px;"></div>
                                    <div><label id="lblSupplierCount">0</label></div>
                                    <div><label id="lblSupplierLastUpdated">(08/20/2014)</label></div>
                                </div>
                            </div>
                        </div>
                        <div style="clear:both;"></div>
                    </div>
                </div>
            </div>

            <div id="divSecond" style="display:none;">
                <div>
                    <div style="float:left; width: 65%;">
                        <form id="formsearchdesktop" method="post">
                            <input type="search" class="txtSearchdesktop" placeholder="Type to search" />
                        </form>
                    </div>
                    <div style="float:right">
                        <span id="divAddLibrary" style="display:block; float:right">
                            <button id="btnAddLibrary" data-role="none" class="btn custom-btn-theme-b btn-less-width">
                                Add <span class="lblAddLibraryName"></span>
                            </button>
                        </span>
                        <span id="divWorkSchedule" style="display:none; float:right; margin-right:5px;">
                            <button id="btnWorkSchedule" data-role="none" class="btn custom-btn-theme-b btn-less-width">
                                Work Schedule
                            </button>
                        </span>
                        <span id="divOptionAddProduct" style="display:none; float:right; margin-right:5px;">
                            <button id="btnOptionAddProduct" data-role="none" class="btn custom-btn-theme-b btn-less-width">
                                Options
                            </button>
                        </span>
                        <span style="clear:both;"></span>
                        <br /><br />
                    </div>
                    <div style="clear:both;"></div>
                </div>

                <br />
                <div id="divInfoForFowstProdList" style="display:none;">
                    <span style="color:#F00">*</span> Click on the Fowst Location to add an SKU.
                    <br />
                    <br />
                </div>
                <div id="divDownloadFilePIM" style="display:none;">
                    <label id="lblProductRecordcount"></label>
                    <a id="lnkProductList" style="cursor:pointer; font-size:16px;"> Click here to download Excel file to view full company product list.</a>
                    <br />
                    <br />
                </div>
                <div style="width:100%;">
                    <table data-role="table" data-mode="reflow" style="width:100%; display:table;" class="ui-table ui-responsive table-stroke table-stripe">
                        <thead>
                            <tr id="tblHeader" class="custom-theme-f txt-center"></tr>
                        </thead>
                        <tbody id="tblBody"></tbody>
                    </table>
                </div>
            </div>

            <div id="UniversalDiv" style="display:none;">

            </div>
        </div>
    </div>

    <div custom-data-role="page" id="pageMaterialRequisition" data-role="content" style="display:none;">
        Loading, please wait ...
    </div>

    <div custom-data-role="page" id="pagePurchaseOrder" data-role="content" style="display:none;">
        Loading, please wait ...
    </div>

</div>

<br /><br />
<div class="btnBackToTop" title="Back to Top"><img src="/Content/images/arrow-up.png" alt="Back to Top" /></div>