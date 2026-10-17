@Code
    ViewData("Title") = "Welcome"
    Layout = "~/Views/Shared/BootstrapFiveLayout.vbhtml"
End Code
<style>
    .menu-description {
        font-size: 13px;
        overflow: hidden;
        text-overflow: ellipsis;
        display: -webkit-box;
        -webkit-line-clamp: 2;
        -webkit-box-orient: vertical;
        height: 40px;
        cursor:pointer;
    }

    .modal, .modal-backdrop {
        bottom: 0 !important;
    }

    .no-hover.btn-outline-danger:hover{
        color: #dc3545 !important;
        background-color: initial !important;
    }

    .no-hover.btn-outline-success:hover{
        color: #28a745 !important;
        background-color: initial !important;
    }

    .required {
        color: #FF0000;
    }

    .tableAvailable {
        height: 60px;
        margin: 5px;
        border-radius: 50px;
        text-align: center;
        line-height: 60px;
        background-color: #16a085;
        color: #fff;
        cursor: pointer;
        font-size: 20px;
        width: 95%;
        border: none;
        outline:none;
        -moz-user-select: none;
        -khtml-user-select: none;
        -webkit-user-select: none;
        user-select: none;
    }

    .tableOccupied {
        background-color: #c1392b;
    }

    .tableDisable {
        display:none;
        position: absolute;
        left: 3px;
        top: 0;
        width: 100%;
        height: 100%;
        background-color: rgba(255,255,255,0.7);
    }

    .tableChecklist{
        display:none;
        position: absolute;
        left: 0;
        top: 0;
        width: 100%;
        height: 100%;
    }

    .tableBadge{
        position: absolute;
        top: 0;
        right: 0;
        background-color: #CCC;
        color: #FFF;
        padding: 4px;
        text-shadow: none;
        max-width: 50%;
        overflow: hidden;
        white-space: nowrap;
        text-overflow: ellipsis;
        border-radius: 3px;
        display:none;
        box-shadow: 0px 1px 1px #404040;
        min-width: 30px;
        text-align:center;
        font-weight:bold;
    }

    .tableBadge-a {
        background-color: #2196f3;
        color: #FFF;
    }

    .tableBadge-b {
        background-color: #ff9800;
    }

    .tableBadge-c {
        background-color: #3f43b5;
    }

    .tableBadge-d {
        background-color: #ffeb3b;
        color: #3c3c3c;
    }

    .tableBadge-e {
        background-color: #795548;
    }

    .tableBadge-f {
        background-color: #673ab7;
    }

    .tableBadge-g {
        background-color: #00bcd4;
    }

    .tableMerge{
        border: 3px solid #000;
    }

    .tabDiningArea li {
        margin-right: 5px;
    }

    /*.tabDiningArea a {
        background-color: #e6e6e6 !important;
    }*/

    .tabDiningArea a.active {
        background-color: #6c757d !important;
        color: #FFF !important;
    }
</style>
@Section head

<script>
    require(['bootstrapjs'], function () {
        require(['/Content/bootstrap/js/bootstrap_modal.js?v='], function () {
            require([GetPathFormat('/Scripts/sources/TableView/Bs_NewTable.js?crc=')]);
        })
    })
</script>

End Section

<div id="divMainTableView">
    <div id="TableView" custom-data-role="page" style=" display:block;">
        <nav class="navbar navbar-dark bg-primary" subpage="header" id="divMainHeader" style="padding:0px !important; display:block;">
            <div class="row" style="margin:0px !important;">
                <div class="col-3 col-sm-2" style="padding:0px !important;">
                    <button data-role="none" id="btnBack" style="height:100%;" class="btn btn-primary btn-block btn-lg rounded-0">
                        <span class="bi-house-fill"></span>
                        <div class="menu-description mt-1">Home</div>
                    </button>
                </div>
                <div class="col-3 col-sm-2" style="padding:0px !important;">
                    <button data-role="none" id="btnTableFowst" style="height:100%;" class="btn btn-primary btn-block btn-lg rounded-0">
                        <span class="bi-building"></span>
                        <div class="menu-description mt-1" id="txtFowstName"></div>
                    </button>
                </div>
                <div class="col-3 col-sm-2" style="padding:0px !important;">
                    <button data-role="none" id="butTableViewOptions" style="height:100%;" class="btn btn-primary btn-block btn-lg rounded-0">
                        <span class="bi-gear-fill"></span>
                        <div class="menu-description mt-1">More Options</div>
                    </button>
                </div>
                <div class="col-3 offset-sm-3 col-sm-3" style="padding:0px !important;">
                    <button data-role="none" id="butViewMode" style="height:100%;" class="btn btn-success btn-block btn-lg rounded-0">
                        <span class="bi-display"></span>
                        <div class="menu-description mt-1">Table View</div>
                    </button>
                </div>
            </div>
        </nav>

        <nav class="navbar navbar-dark bg-primary" subpage="header" id="divJoinHeader" style="padding:0px !important; display:none;">
            <div class="row" style="margin:0px !important;">
                <div class="col-6 col-sm-3" style="padding:0px !important;">
                    <button data-role="none" id="btnJoinCancel" style="height:100%;" class="btn btn-primary btn-block btn-lg rounded-0">
                        <span class="bi-x-square"></span>
                        <div class="menu-description mt-1">Cancel</div>
                    </button>
                </div>
                <div class="col-6 offset-sm-6 col-sm-3" style="padding:0px !important;">
                    <button data-role="none" id="btnJoinOk" style="height:100%;" class="btn btn-success btn-block btn-lg rounded-0">
                        <span class="bi-union"></span>
                        <div class="menu-description mt-1">Join</div>
                    </button>
                </div>
            </div>

            <div style="padding:15px; background-color:#FFF; width:100%;">
                <b>Note: <span class="required">*</span></b>
                Please select tables that you want to join. Then click "Join" button.
            </div>
        </nav>

        <nav class="navbar navbar-dark bg-primary" subpage="header" id="divTransferTableHeader" style="padding:0px !important; display:none;">
            <div class="row" style="margin:0px !important;">
                <div class="col-6 col-sm-3" style="padding:0px !important;">
                    <button data-role="none" id="butTransferTableCancel" style="height:100%;" class="btn btn-primary btn-block btn-lg rounded-0">
                        <span class="bi-x-square"></span>
                        <div class="menu-description mt-1">Cancel</div>
                    </button>
                </div>
                <div class="col-6 offset-sm-6 col-sm-3" style="padding:0px !important;">
                    <button data-role="none" id="butTransferTableOk" style="height:100%;" class="btn btn-success btn-block btn-lg rounded-0">
                        <span class="bi-box-arrow-right"></span>
                        <div class="menu-description mt-1">Transfer</div>
                    </button>
                </div>
            </div>
        </nav>

        <nav class="navbar navbar-dark bg-primary" subpage="header" id="divSplitHeader" style="padding:0px !important; display:none;">
            <div class="row" style="margin:0px !important;">
                <div class="col-12" style="padding:0px !important;">
                    <button data-role="none" id="btnSplitCancel" style="height:100%;" class="btn btn-primary btn-block btn-lg rounded-0">
                        <span class="bi-check-circle"></span>
                        <div class="menu-description mt-1">Done</div>
                    </button>
                </div>
            </div>
        </nav>

        <nav class="navbar navbar-dark bg-primary" subpage="header" id="divSplitBillHeader" style="padding:0px !important; display:none;">
            <div class="row" style="margin:0px !important;">
                <div class="col-12" style="padding:0px !important;">
                    <button data-role="none" id="btnSplitBillCancel" style="height:100%;" class="btn btn-primary btn-block btn-lg rounded-0">
                        <span class="bi-x-square"></span>
                        <div class="menu-description mt-1">Cancel</div>
                    </button>
                </div>
            </div>

            <div style="padding:15px; background-color:#FFF; width:100%;">
                <b>Note: <span class="required">*</span></b>
                <span id="spanSplitBillInstruction"></span>
            </div>
        </nav>

        <div class="mt-3 container-fluid">
            <div id="divTVLoading" subpage="tv" class="mt-5" style="width:100%; text-align:center;">
                <div class="spinner-border text-primary" role="status">
                    <span class="visually-hidden"></span>
                </div>
            </div>

            <div id="divTVContent" subpage="tv" style="display:none;">
                <div style="display:none;" id="instructionSplit">
                    <b>Note: <span class="required">*</span></b>
                    Click a table to split the joined tables.
                    <br /><br />
                </div>
                <div style="display:none;" id="instructionTransferTable">
                    <b>Note: <span class="required">*</span></b>
                    <span id="spanTransferTableInstruction"></span>
                    <br /><br />
                </div>

                <div id="divDiningArea" style="white-space:nowrap; overflow:auto;">
                    <ul id="lstDiningArea" class="nav nav-tabs tabDiningArea" style="flex-wrap:nowrap;">
                        @*<li class="active" custom-tab-content="#tabCompanyProfile">Profile</li>*@
                    </ul>
                </div>

                <div style="font-size:12px; text-align:center;">(swipe left or right to see more)</div>

                <div id="divTabContent" class="tab-content mt-3">

                </div>
            </div>            
        </div>

        <div id="popSplitJoinedTables" class="modal fade">
            <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">Split Table</h5>
                        <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                    </div>
                    <div class="modal-body">
                        <b>Noted <span class="required">*</span>: </b>
                        To remove a table one at a time, just click the X button at the right of the table name then click OK when you're done. Or to split all tables, just click the 'Split All' button.

                        <div style="padding:15px 15px 0px 15px;">
                            <div id="listOfSelectedJoinedTables" class="list-group"></div>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <div class="row">
                            <div class="col-sm-4 mb-3 order-sm-2">
                                <button data-role="none" id="btnOkTable" class="btn btn-success btn-lg btn-block">Ok</button>
                            </div>                            
                            <div class="col-sm-4 mb-3 order-sm-1">
                                <button data-role="none" id="btnSplitAllTable" class="btn btn-danger btn-lg btn-block">Split All</button>
                            </div>
                            <div class="col-sm-4 mb-3 order-sm-0">
                                <button data-role="none" data-dismiss="modal" class="btn btn-secondary btn-lg btn-block">Cancel</button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div id="popupTotalNumberOfCustomer" class="modal fade">
            <div class="modal-dialog modal-dialog-centered">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">Guest Count</h5>
                        <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                    </div>
                    <div class="modal-body">
                        <div class="row mb-3">
                            <label class="col-4" id="lblTable">Table Number:</label>
                            <div class="col-8" id="lblTableNo"></div>
                        </div>

                        <div id="divForGroupTable" class="row mb-3" style="display:none;">
                            <label class="col-4">Table Number:</label>
                            <div class="col-8" id="lblForGroupTableNo"></div>
                        </div>

                        <div class="row mb-3">
                            <label class="col-sm-4 mb-3">No. of Guest (Diners): <span style="color: #FF0000;">*</span></label>
                            <div class="col-sm-8">
                                <select id="txtTotalNumberOfCustomer" data-role="none" class="form-select"></select>
                            </div>
                        </div>

                        <div id="divProductSalesPersonContainer">
                            <div class="row mb-3">
                                <div class="col-6 col-form-label">
                                    Sales Representative List
                                </div>
                                <div class="col-6 text-right">
                                    <button data-role="none" id="btnAddRepresentative" class="btn btn-secondary">Add Representative</button>
                                </div>
                            </div>
                            <div id="lstSalesRep" class="list-group"></div>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <div class="row">
                            <div class="col-6">
                                <button data-role="none" data-dismiss="modal" class="btn btn-secondary btn-lg btn-block">Cancel</button>
                            </div>
                            <div class="col-6">
                                <button data-role="none" id="btnSaveTotalNumberOfCustomer" class="btn btn-success btn-lg btn-block">Ok</button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div data-role="popup" id="popupTableOptions" class="ui-corner-all" style="padding:20px; display:none;">
            <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
            <center><h3>Options</h3></center>
            <div data-role="content" style="text-align: center; ">
                <button id="butTableOptionsSplitTable">Split Table</button>
                <button id="btnTableOptionJoin">Join Table</button>
                <button id="butTableOptionsTransferTable">Transfer Table</button>
                <button id="butTableOptionsSplitMergeBill">Split / Merge Bill</button>
                <a data-role="button" data-rel="back">Cancel</a>
            </div>
        </div>

        <div id="divPopNumericPassword" class="modal fade">
            <div class="modal-dialog modal-dialog-centered">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">Sales Representative</h5>
                        <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                    </div>
                    <div class="modal-body">
                        Please enter your PIN.
                        <br /><br />
                        @* http://stackoverflow.com/a/25484576/334045 *@
                        <input type="number" data-role="none" class="form-control" pattern="[0-9]*" style="-webkit-text-security:disc" />
                    </div>
                    <div class="modal-footer">
                        <div class="row">
                            <div class="col-6">
                                <button data-role="none" name="close" data-dismiss="modal" class="btn btn-secondary btn-lg btn-block">Cancel</button>
                            </div>
                            <div class="col-6">
                                <button data-role="none" name="ok" class="btn btn-success btn-lg btn-block">Ok</button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>