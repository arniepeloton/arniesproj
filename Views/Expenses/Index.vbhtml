@Code
    ViewData("Title") = "Expenses"
    Layout = "~/Views/Shared/BootstrapFiveLayout.vbhtml"
End Code
@Section head    
    @*<link rel="stylesheet" href="/Content/flatui-css-custom.css">
    <link rel="stylesheet" href="/Content/bootstrap-custom.css">
    <link rel="stylesheet" href="/Content/Ecommerce/css/bootstrap.min.css">*@
    
    <script>
        require(['bootstrapjs'], function () {
            require(['/Content/bootstrap/js/bootstrap_modal.js?v='], function () {
                require([GetPathFormat('/Scripts/sources/Expenses/Expenses.js?crc=')]);
            })
        })        
    </script>

    <style type="text/css">
        body {
            font-size:18px !important;
            background-color: white !important;
        }
        .buttonExpense {
            height:auto !important;
        }
        .buttonExpense > img{
            height: 35px;
            text-overflow: clip;
            margin-bottom: 10px;
            margin-top: 10px;
        }

        .form-control{
            height:40px;
        }

        .header{
            position: fixed;
            top: 0;
            width: 100%;
            z-index: 1000;
        }

        .header~.container{
            margin-top:50px;
        }

        .header > .container{
            display:flex;
        }

        .header > .container > div{
            float: left;
            margin-bottom: 0px;
            overflow: hidden;
            font-size: 25px;
            white-space: nowrap;
            text-overflow: ellipsis;
        }

        .alert-neutral {
            background-color: #f2f2f2;
            border-color: #d6d6d6;
            color: #000;
        }
    </style>

    @*<link rel="stylesheet" href="/Content/Ecommerce/css/main.css">
    <link rel="stylesheet" href="/Content/Ecommerce/css/blue.css">*@
End Section

<div id="pageExpenseMain" custom-data-role="page" style="display:block;">
    <nav class="navbar navbar-dark bg-primary sticky-top text-light pl-3 pr-3 mb-3">
        <div>
            <button id="btnExpenseBackToMenu" data-role="none" class="btn btn-primary">
                <span class="bi-chevron-left"></span>
            </button>
            Routes & Expenses
        </div>
    </nav>

    <div class="container-fluid">
        <table data-role="none" class="mb-3 d-none">
            <tr>
                <td width="110px" class="text-center">
                    @*<img id="imgProfilePic" src="/Content/images/NoPhoto.jpg" alt="No Photo" width="100px" height="100px" />*@
                    <span class="bi-person-square" style="color:#a7a7a7; font-size:100px;"></span>
                </td>
                <td>
                    <h2 id="lblCurrentUserFullName">
                    </h2>
                </td>
            </tr>
        </table>

        <div class="row row-cols-2 row-cols-sm-2 row-cols-md-3 row-cols-lg-4 row-cols-xl-5 row-cols-xxl-6 text-center">
            <div class="col mb-3">
                <div class="dashboardGrid bg-light border p-3 h-100 rounded erp-main-grid" id="btnExpenseCheckIn">
                    <div class="bi-arrow-down-square rounded mb-3 module-buttons"></div>
                    Check-in
                </div>
            </div>
            <div class="col mb-3">
                <div class="dashboardGrid bg-light border p-3 h-100 rounded erp-main-grid" id="btnExpenseCheckOut">
                    <div class="bi-arrow-up-square rounded mb-3 module-buttons"></div>
                    Check-out
                </div>
            </div>
            <div class="col mb-3">
                <div class="dashboardGrid bg-light border p-3 h-100 rounded erp-main-grid btnLogExpense">
                    <div class="bi-cash-coin rounded mb-3 module-buttons"></div>
                    Log Expense
                </div>
            </div>
            <div class="col mb-3">
                <div class="dashboardGrid bg-light border p-3 h-100 rounded erp-main-grid" id="btnExpenseReceiveFunds">
                    <div class="bi-wallet2 rounded mb-3 module-buttons"></div>
                    Receive Funds
                </div>
            </div>
            <div class="col mb-3">
                <div class="dashboardGrid bg-light border p-3 h-100 rounded erp-main-grid" id="btnFundsReports">
                    <div class="bi-graph-up rounded mb-3 module-buttons"></div>
                    Funds Report
                </div>
            </div>
            <div class="col mb-3">
                <div class="dashboardGrid bg-light border p-3 h-100 rounded erp-main-grid" id="btnExpenseCheckinReports">
                    <div class="bi-pin-map rounded mb-3 module-buttons"></div>
                    Check-in Reports
                </div>
            </div>
            <div class="col mb-3">
                <div class="dashboardGrid bg-light border p-3 h-100 rounded erp-main-grid" id="btnSyncData">
                    <div class="bi-cloud-arrow-down rounded mb-3 module-buttons"></div>
                    Get My Data
                </div>
            </div>
        </div>
    </div>
</div>

<div id="pageSyncData" custom-data-role="page" style="display:none;">
    <nav class="navbar navbar-dark bg-primary sticky-top text-light pl-3 pr-3 mb-3">
        <div>
            <button data-role="none" id="btnSyncDataBack" class="btn btn-primary">
                <span class="bi-chevron-left"></span>
            </button>
            Get My Data
        </div>
    </nav>

    <div class="container">
        This feature allows you to get your latest data from server and save to your local database. 
        Please tap the button below to get started.
        <br /><br /><br />
        <button data-role="none" id="btnSyncDataNow" class="btn btn-success btn-block btn-lg">
            Get My Data Now
        </button>
    </div>
</div>

<div id="pageCheckInOut" custom-data-role="page" style="display:none;">
    <nav class="navbar navbar-dark bg-primary sticky-top text-light pl-3 pr-3 mb-3">
        <div>
            <button id="btnCkeckInOutBack" data-role="none" class="btn btn-primary">
                <span class="bi-chevron-left"></span>
            </button>
            Check <span id="lblCheckType"></span> Details
        </div>
    </nav>

    <div class="container-fluid">
        <div class="mb-3">
            <label class="form-label" for="txtExpenseLocationName">Location Name: <span class="required">*</span></label>
            <textarea data-role="none" class="form-control" id="txtExpenseLocationName" readonly placeholder="Select Location" style="resize:none; height:80px;"></textarea>
        </div>
        <div class="mb-3">
            <label class="form-label" for="txtExpenseCheckinoutGPSCoordinates">GPS Coordinates:</label>
            <input data-role="none" type="text" readonly class="form-control" id="txtExpenseCheckinoutGPSCoordinates">
        </div>
        <div class="mb-3">
            <label class="form-label" for="txtExpenseCheckIn">Check-in Date/Time:</label>
            <input data-role="none" type="text" readonly class="form-control" id="txtExpenseCheckIn">
        </div>
        <div class="mb-3">
            <label class="form-label" for="txtExpenseCheckOut">Check-out Date/Time:</label>
            <input data-role="none" type="text" readonly class="form-control" id="txtExpenseCheckOut">
        </div>
        <div class="mb-3">
            <label class="form-label" for="txtExpenseCheckinoutRemarks">Purpose / Remarks:</label>
            <textarea data-role="none" type="text" class="form-control" id="txtExpenseCheckinoutRemarks" style="height:100px;"></textarea>
        </div>
        <div class="row mt-4 mb-4">
            <div class="col">
                <button data-role="none" id="btnCheckInOutLogExpense" class="btn btn-secondary btn-block btn-lg">Log Expense</button>
            </div>
            <div class="col">
                <button data-role="none" id="btnSaveCheckinout" class="btn btn-success btn-block btn-lg">Ok</button>
            </div>
        </div>
    </div>    
</div>

<div id="pageExpenseReceiveFunds" custom-data-role="page" style="display:none;">
    <nav class="navbar navbar-dark bg-primary sticky-top text-light pl-3 pr-3 mb-3">
        <div>
            <button id="btnExpenseReceiveFundsBack" data-role="none" class="btn btn-primary">
                <span class="bi-chevron-left"></span>
            </button>
            Receive Funds
        </div>
    </nav>

    <div class="container-fluid">
        <div class="mb-3">
            <i>Note: Fields with <b><span class="required">*</span></b> are required.</i>
        </div>

        <div class="mb-3">
            <label class="form-label" for="txtFundReceivingDate">Fund Receiving Date <span class="required">*</span></label>
            <input data-role="none" type="date" class="form-control" id="txtFundReceivingDate">
        </div>

        <div class="mb-3">
            <label class="form-label" for="txtFundReceivingDate">Fund Received In Branch: <span class="required">*</span></label>
            <select data-role="none" class="form-select" id="selFundReceivingFowst"></select>
        </div>

        <div class="row mb-3 align-items-center">
            <label class="col" for="btnFundReceivedFrom">Fund Received From <span class="required">*</span></label>
            <div class="col-auto text-right">
                <input data-role="none" type="button" value="Add Employee" class="btn btn-secondary" id="btnFundReceivedFrom">
            </div>            
        </div>

        <div id="divReceiveFrom"></div>

        <div class="mb-3">
            <label class="form-label" for="txtFundReceivedBy">Fund Received By <span class="required">*</span></label>
            <input data-role="none" type="text" readonly class="form-control" id="txtFundReceivedBy">
        </div>

        <div class="mb-3">
            <label class="form-label" for="txtFundReceivedAmount">Fund Received Amount <span class="required">*</span></label>
            <input data-role="none" type="number" class="form-control" id="txtFundReceivedAmount">
        </div>

        <div class="mb-3">
            <label class="form-label" for="txtFundReceivingComment">Comment</label>
            <textarea data-role="none" type="text" class="form-control" id="txtFundReceivingComment" style="height:100px;"></textarea>
        </div>
        
        <div class="row mt-4 mb-4">
            <div class="col">
                <button data-role="none" id="btnCancelReceiveFund" class="btn btn-secondary btn-block btn-lg">Cancel</button>
            </div>
            <div class="col">
                <button data-role="none" id="btnSaveReceiveFund" class="btn btn-success btn-block btn-lg">Ok</button>
            </div>
        </div>
    </div>
</div>

<div id="pageLogExpenseMain" custom-data-role="page" style="display:none;">
    <nav class="navbar navbar-dark bg-primary sticky-top text-light pl-3 pr-3 mb-3">
        <div>
            <button id="btnLogExpenseMainBack" data-role="none" class="btn btn-primary">
                <span class="bi-chevron-left"></span>
            </button>
            Log Expense
        </div>
    </nav>

    <div class="container-fluid">
        <div class="mb-3"><b>Choose expense type:</b></div>

        <div class="row">
            <div class="col mb-3">
                <button data-role="none" value="Store" class="btn btn-light btn-block buttonExpense border btnLogExpenseType" style="height:auto !important;">
                    <img src="/Content/images/StoreExpense.png" />
                    <br />
                    Store
                </button>
            </div>
            <div class="col mb-3">
                <button data-role="none" value="Representation" class="btn btn-light btn-block buttonExpense border btnLogExpenseType" style="height:auto !important;">
                    <img src="/Content/images/RepresentationExpense.png" />
                    <br />
                    Representation
                </button>
            </div>
        </div>

        <div class="row">
            <div class="col mb-3">
                <button data-role="none" value="Jeepney Fare" class="btn btn-light btn-block buttonExpense border btnLogExpenseType" style="height:auto !important;">
                    <img src="/Content/images/JeepneyFareExpense.png" />
                    <br />
                    Jeepney Fare
                </button>
            </div>
            <div class="col mb-3">
                <button data-role="none" value="Bus Fare" class="btn btn-light btn-block buttonExpense border btnLogExpenseType" style="height:auto !important;">
                    <img src="/Content/images/BusFareExpense.png" />
                    <br />
                    Bus Fare
                </button>
            </div>            
        </div>

        <div class="row">
            <div class="col mb-3">
                <button data-role="none" value="Taxi Fare" class="btn btn-light btn-block buttonExpense border btnLogExpenseType" style="height:auto !important;">
                    <img src="/Content/images/TaxiFareExpense.png" />
                    <br />
                    Taxi Fare
                </button>
            </div>
            <div class="col mb-3">
                <button data-role="none" value="Van Fare" class="btn btn-light btn-block buttonExpense border btnLogExpenseType" style="height:auto !important;">
                    <img src="/Content/images/VanFareExpense.png" />
                    <br />
                    Van Fare
                </button>
            </div>            
        </div>

        <div class="row">
            <div class="col mb-3">
                <button data-role="none" value="Fuel" class="btn btn-light btn-block buttonExpense border btnLogExpenseType" style="height:auto !important;">
                    <img src="/Content/images/GasFareExpense.png" />
                    <br />
                    Fuel
                </button>
            </div>
            <div class="col mb-3">
                <button data-role="none" value="Tricycle Fare" class="btn btn-light btn-block buttonExpense border btnLogExpenseType" style="height:auto !important;">
                    <img src="/Content/images/TricycleFareExpense.png" />
                    <br />
                    Tricycle Fare
                </button>
            </div>                        
        </div>

        <div class="row">
            <div class="col mb-3">
                <button data-role="none" value="Air Fare" class="btn btn-light btn-block buttonExpense border btnLogExpenseType" style="height:auto !important;">
                    <img src="/Content/images/AirFareExpense.png" />
                    <br />
                    Air Fare
                </button>
            </div>
            <div class="col mb-3">
                <button data-role="none" value="Sea Fare" class="btn btn-light btn-block buttonExpense border btnLogExpenseType" style="height:auto !important;">
                    <img src="/Content/images/SeaFareExpense.png" />
                    <br />
                    Sea Fare
                </button>
            </div>                        
        </div>

        <div class="row">
            <div class="col mb-3">
                <button data-role="none" value="Train Fare" class="btn btn-light btn-block buttonExpense border btnLogExpenseType" style="height:auto !important;">
                    <img src="/Content/images/TrainFareExpense.png" />
                    <br />
                    Train Fare
                </button>
            </div>
            <div class="col mb-3">
                <button data-role="none" value="Telco" class="btn btn-light btn-block buttonExpense border btnLogExpenseType" style="height:auto !important;">
                    <img src="/Content/images/TelcoExpense.png" />
                    <br />
                    Telco
                </button>
            </div>                        
        </div>

        <div class="row">
            <div class="col mb-3">
                <button data-role="none" value="Internet" class="btn btn-light btn-block buttonExpense border btnLogExpenseType" style="height:auto !important;">
                    <img src="/Content/images/InternetExpense.png" />
                    <br />
                    Internet
                </button>
            </div>
            <div class="col mb-3">
                <button data-role="none" value="Meal" class="btn btn-light btn-block buttonExpense border btnLogExpenseType" style="height:auto !important;">
                    <img src="/Content/images/MealExpense.png" />
                    <br />
                    Meal
                </button>
            </div>                        
        </div>

        <div class="row">
            <div class="col mb-3">
                <button data-role="none" value="Lodging" class="btn btn-light btn-block buttonExpense border btnLogExpenseType" style="height:auto !important;">
                    <img src="/Content/images/LodgingExpense.png" />
                    <br />
                    Lodging
                </button>
            </div>
            <div class="col mb-3">
                <button data-role="none" value="Other Expense" class="btn btn-light btn-block buttonExpense border btnLogExpenseType" style="height:auto !important;">
                    <img src="/Content/images/OtherExpense.png" />
                    <br />
                    Other Expense
                </button>
            </div>
        </div>
    </div>
</div>

<div id="pageLogExpense" custom-data-role="page" style="display:none;">
    <nav class="navbar navbar-dark bg-primary sticky-top text-light pl-3 pr-3 mb-3">
        <div>
            <button data-role="none" class="btn btn-primary btnLogExpenseBack">
                <span class="bi-chevron-left"></span>
            </button>
            Log Expense
        </div>
    </nav>

    <div class="container-fluid">
        <div class="mb-3">
            <i>Note: Fields with <b><span class="required">*</span></b> are required.</i>
        </div>

        <div class="mb-3">
            <label class="form-label" for="selExpenseType">Expense Type: <span class="required">*</span></label>
            <select data-role="none" id="selExpenseType" class="form-select">
                <option value="Store">Store</option>
                <option value="Representation">Representation</option>
                <option value="Jeepney Fare">Jeepney Fare</option>
                <option value="Bus Fare">Bus Fare</option>
                <option value="Taxi Fare">Taxi Fare</option>
                <option value="Van Fare">Van Fare</option>
                <option value="Fuel">Fuel</option>
                <option value="Tricycle Fare">Tricycle Fare</option>
                <option value="Air Fare">Air Fare</option>
                <option value="Sea Fare">Sea Fare</option>
                <option value="Train Fare">Train Fare</option>
                <option value="Telco">Telco</option>
                <option value="Internet">Internet</option>
                <option value="Meal">Meal</option>
                <option value="Lodging">Lodging</option>                
                <option value="Other Expense">Other Expense</option>
            </select>
        </div>

        <div class="mb-3">
            <label class="form-label" for="txtExpenseFrom">Expense Date: <span class="required">*</span></label>
            <input data-role="none" type="date" class="form-control" id="txtExpenseDate">
        </div>

        <div class="mb-3 divforExpenseTypeFare" style="display:none;">
            <label class="form-label" for="txtExpenseFrom">Location From: <span class="required">*</span></label>
            <input data-role="none" type="text" class="form-control" id="txtExpenseFrom">
        </div>

        <div class="mb-3 divforExpenseTypeFare" style="display:none;">
            <label class="form-label" for="txtExpenseTo">Location To: <span class="required">*</span></label>
            <input data-role="none" type="text" class="form-control" id="txtExpenseTo">
        </div>

        <div class="mb-3">
            <label class="form-label" for="txtExpenseGPSCoordinates">GPS Coordinates:</label>
            <input data-role="none" type="text" readonly class="form-control" id="txtExpenseGPSCoordinates">
        </div>

        <div class="mb-3">
            <label class="form-label" for="txtExpenseAmount">Amount: <span class="required">*</span></label>
            <input data-role="none" type="number" class="form-control" id="txtExpenseAmount">
        </div>

        <div class="row mb-3 align-items-center">
            <label class="col" for="btnExpenseAddClient">Associate to:</label>
            <div class="col-auto text-right">
                <button data-role="none" id="btnExpenseAddClient" class="btn btn-secondary btn-block">Add Client / Vendor</button>
            </div>
        </div>

        <div id="divAssociatedTo"></div>

        <div class="mb-3">
            <label class="form-label" for="selExpenseCostCenterLocation">Cost Center Location: <span class="required">*</span></label>
            <select data-role="none" id="selExpenseCostCenterLocation" class="form-select">
                <option>Please select</option>
            </select>
        </div>

        <div class="mb-3">
            <label for="selExpenseCostCenter">Cost Center: </label>
            <select data-role="none" id="selExpenseCostCenter" class="form-select">
                <option>Please select</option>
            </select>
        </div>

        <div class="mb-3">
            <label for="txtExpenseRemarks">Remarks: <span id="lblLogExpenseRemarksField" class="required" style="display:none;">*</span></label>
            <textarea data-role="none" type="text" class="form-control" id="txtExpenseRemarks" style="height:100px;"></textarea>
        </div>        

        @*<div class="mb-3 form-inline">
            <label for="btnExpensePicture">Picture:</label>
            <input data-role="none" type="button" value="Take Picture" class="btn btn-light btn-block" style="float:right;" id="btnExpensePicture">
            <input data-role="none" type="file" id="fileExpensePicture" />
        </div>*@

        <div class="row mt-4 mb-4">
            <div class="col">
                <button data-role="none" class="btn btn-secondary btn-block btn-lg btnLogExpenseBack">Cancel</button>
            </div>
            <div class="col">
                <button data-role="none" id="btnSaveLogExpense" class="btn btn-success btn-block btn-lg">Ok</button>
            </div>
        </div>
    </div>
</div>

<div id="pageFundsReports" custom-data-role="page" style="display:none;">
    <nav class="navbar navbar-dark bg-primary sticky-top text-light pl-3 pr-3 mb-3">
        <div>
            <button id="btnFundsReportsBack" data-role="none" class="btn btn-primary">
                <span class="bi-chevron-left"></span>
            </button>
            Funds <span class="lblFundsDateCovered"></span>
        </div>
    </nav>

    <div class="container-fluid mb-5">
        <div class="mb-5">
            <label class="form-label">Reports for Branch:</label>
            <select data-role="none" class="form-select" id="selFundReportFowst"></select>
        </div>

        <div id="btnReceivedFundDetails" class="alert alert-neutral" role="alert">
            <div style="display:table; width:100%;">
                <div style="display:table-cell;">Fund receipts for the month</div>
                <div class="lblTotalReceiveFundsHeader" style="display:table-cell; text-align:right; vertical-align:middle;">-</div>
                <div style="display:table-cell; text-align:right; width: 25px; vertical-align:middle;">
                    <span class="bi-chevron-right"></span>
                </div>
            </div>
        </div>

        <div id="btnCarryOverFunds" class="alert alert-neutral" role="alert">
            <div style="display:table; width:100%;">
                <div style="display:table-cell;">Carry over funds from prev. periods</div>
                <div id="lblCarryOverFund" style="display:table-cell; text-align:right; vertical-align:middle;">-</div>
                <div style="display:table-cell; text-align:right; width: 25px; vertical-align:middle;">
                    <span class="bi-chevron-right"></span>
                </div>
            </div>
        </div>

        <div class="alert alert-neutral" role="alert">
            <div style="display:table; width:100%; font-weight:bold;">
                <div style="display:table-cell;">Total beg. fund bal. for the month</div>
                <div id="lblTotalBegFundBal" style="display:table-cell; text-align:right; vertical-align:middle;">-</div>
            </div>
        </div>

        <div id="btnExpenseReports" class="alert alert-neutral" role="alert">
            <div style="display:table; width:100%;">
                <div style="display:table-cell;">Total expenses for the month</div>
                <div class="lblTotalExpenseHeader" style="display:table-cell; text-align:right; vertical-align:middle;">-</div>
                <div style="display:table-cell; text-align:right; width: 25px; vertical-align:middle;">
                    <span class="bi-chevron-right"></span>
                </div>
            </div>
        </div>
        <br /><br />
    </div>
    <div class="fixed-bottom bg-light">
        <div class="container">
            <div style="display:table; width:100%; font-size:18px;">
                <div style="display:table-cell; vertical-align:middle; font-size:18px; padding: 10px 0px;">Net fund balance for the month:</div>
                <div id="lblNetFundBalance" style="display:table-cell; vertical-align:middle; font-size:25px; padding: 10px 0px; text-align:right;">-</div>
            </div>
        </div>
    </div>
</div>

<div id="pageFundReceivedReportsHeaderDetails" custom-data-role="page" style="display:none;">
    <nav class="navbar navbar-dark bg-primary sticky-top text-light pl-3 pr-3 mb-3">
        <div>
            <button id="btnFundReceivedReportsHeaderDetailsBack" data-role="none" class="btn btn-primary">
                <span class="bi-chevron-left"></span>
            </button>
            Funds Received <span class="lblFundsDateCovered"></span>
        </div>
    </nav>

    <div class="container-fluid">
        <div id="btnReceivedFundMoreDetails" class="alert alert-success" role="alert"><span class="glyphicon glyphicon-info-sign"></span> Tap here to retrieve received funds details from server.</div>
        <ul id="listFundReceivedReportHeaderDetails" class="list-group"></ul>
        <br /><br />
    </div>
    <div class="fixed-bottom bg-light">
        <div class="container-fluid">
            <div style="display:table; width:100%; font-size:18px;">
                <div style="display:table-cell; vertical-align:middle; font-size:20px; padding: 5px 0px;">Total Received Funds for the month:</div>
                <div id="lblSubTotalReceivedFunds" style="display:table-cell; vertical-align:middle; font-size:25px; padding: 5px 0px; text-align:right;"></div>
            </div>
            @*<div style="display:table; width:100%; font-size:18px;">
                <div style="display:table-cell; vertical-align:middle; font-size:20px; padding: 5px 0px;">Total Received Funds for the month:</div>
                <div id="lblNetTotalReceivedFunds" style="display:table-cell; vertical-align:middle; font-size:20px; padding: 10px 0px; text-align:right;"></div>
            </div>*@
        </div>
    </div>
</div>

<div id="pageExpenseReports" custom-data-role="page" style="display:none;">
    <nav class="navbar navbar-dark bg-primary sticky-top text-light pl-3 pr-3 mb-3">
        <div>
            <button id="btnExpenseReportsBack" data-role="none" class="btn btn-primary">
                <span class="bi-chevron-left"></span>
            </button>
            Expense <span id="lblExpenseDateCovered"></span>
        </div>
    </nav>

    <div class="container-fluid">
        <div class="mb-3">
            Expenses for branch: <span class="lblSelectedBranchReport"></span>
        </div>

        <div id="divExpenseReportHeader"></div>
    </div>
    <div class="fixed-bottom bg-light">
        <div class="container-fluid">
            <div style="display:table; width:100%; font-size:18px;">
                <div style="display:table-cell; vertical-align:middle; font-size:25px; padding: 10px 0px;">Total:</div>
                <div class="lblTotalExpenseHeader" style="display:table-cell; vertical-align:middle; font-size:25px; padding: 10px 0px; text-align:right;"></div>
            </div>
        </div>
    </div>
</div>

<div id="pageExpenseReportsHeaderDetails" custom-data-role="page" style="display:none;">
    <nav class="navbar navbar-dark bg-primary sticky-top text-light pl-3 pr-3 mb-3">
        <div>
            <button id="btnExpenseReportsHeaderDetailsBack" data-role="none" class="btn btn-primary">
                <span class="bi-chevron-left"></span>
            </button>
            <span id="lblExpenseTypeHeaderDetails"></span> Expense
        </div>
    </nav>

    <div class="container-fluid">
        Expenses for <span id="lblMonthName"></span><br />
        Branch: <span class="lblSelectedBranchReport"></span><br /><br />
        <div id="btnLogExpenseMoreDetails" class="alert alert-success" role="alert"><span class="glyphicon glyphicon-info-sign"></span> Tap here to retrieve expense details from server.</div>
        <table data-role="none" style="width:100%;">
            <tr style="height:50px;">
                <td style="width:70px;">Period:</td>
                <td>
                    <select id="selExpensePeriod" data-role="none" class="form-select">
                        <option value="today">Today</option>
                        <option value="week">Week-To-Date</option>
                        <option value="month">Month-To-Date</option>
                    </select>
                </td>                
            </tr>
            <tr style="height:50px;">
                <td style="width:70px;">Sort by:</td>
                <td>
                    <select id="selExpenseSorting" data-role="none" class="form-select">
                        <option value="ASC">Oldest to Latest</option>
                        <option value="DESC">Latest to Oldest</option>
                    </select>
                </td>
            </tr>
        </table>
        <br />
        <ul id="listExpenseReportHeaderDetails" class="list-group"></ul>
        <br />
    </div>
    <div class="fixed-bottom bg-light">
        <div class="container">
            <div style="display:table; width:100%; font-size:18px;">
                <div id="lblExpensePeriod" style="display:table-cell; vertical-align:middle; font-size:25px; padding: 5px 0px;">Today's Total:</div>
                <div id="lblTotalPerExpenseTypePerPeriod" style="display:table-cell; vertical-align:middle; font-size:25px; padding: 5px 0px; text-align:right;"></div>
            </div>
            @*<div style="display:table; width:100%; font-size:18px;">
                <div style="display:table-cell; vertical-align:middle; font-size:20px; padding: 5px 0px;">Month-To-Date Total:</div>
                <div id="lblTotalPerExpenseType" style="display:table-cell; vertical-align:middle; font-size:20px; padding: 5px 0px; text-align:right;"></div>
            </div>*@
        </div>
    </div>
</div>

<div id="pageExpenseCheckinReports" custom-data-role="page" style="display:none;">
    <nav class="navbar navbar-dark bg-primary sticky-top text-light pl-3 pr-3 mb-3">
        <div>
            <button id="btnExpenseCheckinReportsBack" data-role="none" class="btn btn-primary">
                <span class="bi-chevron-left"></span>
            </button>
            Check-in Reports
        </div>
    </nav>

    <div class="container-fluid">
        <div id="btnCheckinoutMoreDetails" class="alert alert-success" role="alert"><span class="glyphicon glyphicon-info-sign"></span> Tap here to retrieve check in/out details from server.</div>
        Location: <span class="bi-geo-alt-fill" style="color:#dc3545;"></span>
        Check-in: <span class="bi-arrow-down-square-fill" style="color:#4CAF50;"></span>
        Check-out: <span class="bi-arrow-up-square-fill" style="color:#FF9800;"></span>
        Duration: <span class="bi-clock-fill" style="color:#FF5722;"></span>
        Remarks: <span class="bi-chat-text-fill" style="color:#2196f3;"></span>
        <br /><br />
        <table data-role="none" style="width:100%;">
            <tr>
                <td style="width:70px;">Sort by:</td>
                <td>
                    <select id="selCheckinSorting" data-role="none" class="form-select">
                        <option value="ASC">Oldest to Latest</option>
                        <option value="DESC">Latest to Oldest</option>
                    </select>
                </td>
            </tr>
        </table>
        <br />
        <ul id="listExpenseCheckinReports" class="list-group mb-5"></ul>
    </div>
</div>

<div id="pageCarryOverFunds" custom-data-role="page" style="display:none;">
    <nav class="navbar navbar-dark bg-primary sticky-top text-light pl-3 pr-3 mb-3">
        <div>
            <button id="btnCarryOverFundsBack" data-role="none" class="btn btn-primary">
                <span class="bi-chevron-left"></span>
            </button>
            Carry Over Funds
        </div>
    </nav>

    <div class="container-fluid">
        <div class="mb-3" style="font-size:inherit; display:table; width:100%;">
            <div style="display:table-row;">
                <div style="display:table-cell;">
                    For branch: <span class="lblSelectedBranchReport"></span>
                </div>
                <div class="text-right" style="width:120px; display:table-cell;">
                    <button data-role="none" id="btnAdjustFund" class="btn btn-success btn-block">Adjust Fund</button>
                </div>
            </div>
        </div>

        <div class="alert alert-success mb-3" role="alert">
            <div style="font-size:inherit; display:table; width:100%;">
                <div style="display:table-row;">
                    <div style="display:table-cell;">
                        Carry Over funds from <span class="lblCarryOverDateCovered"></span>
                    </div>
                    <div class="text-right" id="lblRemainingFund" style="width:100px; display:table-cell;">
                        0.00
                    </div>
                </div>
            </div>
        </div>

        <div id="divRAF" style="display:none;">
            Requested Adjustment Fund
            <br /><br />
            <div class="alert alert-neutral mb-3">
                <div class="mb-3" style="font-size:inherit; display:table; width:100%;">
                    <div style="display:table-row;">
                        <div style="display:table-cell;">
                            Status:
                        </div>
                        <div class="text-right" id="lblRAFStatus" style="width:120px; display:table-cell;">

                        </div>
                    </div>
                </div>

                <div class="mb-3" style="font-size:inherit; display:table; width:100%;">
                    <div style="display:table-row;">
                        <div style="display:table-cell;">
                            Requested Date:
                        </div>
                        <div class="text-right" id="lblRAFRequestedDate" style="width:120px; display:table-cell;">

                        </div>
                    </div>
                </div>

                <div class="mb-3" style="font-size:inherit; display:table; width:100%;">
                    <div style="display:table-row;">
                        <div style="display:table-cell;">
                            Adjusted Period:
                        </div>
                        <div class="text-right lblCarryOverDateCovered" style="width:120px; display:table-cell;">

                        </div>
                    </div>
                </div>

                <div class="mb-3" style="font-size:inherit; display:table; width:100%;">
                    <div style="display:table-row;">
                        <div style="display:table-cell;">
                            Adjusted Amount:
                        </div>
                        <div class="text-right" id="lblRAFAmount" style="width:120px; display:table-cell;">

                        </div>
                    </div>
                </div>

                <div style="font-size:inherit; display:table; width:100%;">
                    <div style="display:table-row;">
                        <div style="display:table-cell;">
                            <div class="mb-3">Remarks:</div>
                            <span id="lblRAFRemarks" style="margin-left:20px;"></span>
                        </div>
                    </div>
                </div>            
            </div>
            <button data-role="none" id="btnCheckUpdatesRAF" class="btn btn-success btn-lg btn-block mb-3">Check for Approval Updates</button>
            <button data-role="none" id="btnCancelRAF" class="btn btn-danger btn-lg btn-block mb-4">Cancel Request</button>
        </div>
    </div>
</div>

<div id="pageCarryOverFundAdjustment" custom-data-role="page" style="display:none;">
    <nav class="navbar navbar-dark bg-primary sticky-top text-light pl-3 pr-3 mb-3">
        <div>
            <button data-role="none" id="btnCarryOverFundAdjustmentBack" class="btn btn-primary">
                <span class="bi-chevron-left"></span>
            </button>
            Fund Adjustment
        </div>
    </nav>

    <div class="container-fluid">
        <div class="mb-3">
            <label class="form-label" for="txtFundReceivingDate">Branch to be adjusted: <span class="required">*</span></label>
            <select data-role="none" class="form-select" id="selFowstCarryOverFund"></select>
        </div>

        <div class="mb-3">
            <label class="form-label" for="txtOriginalCarryOverFund">Remaining Fund from <span class="lblCarryOverDateCovered"></span>:</label>
            <input data-role="none" type="number" id="txtOriginalCarryOverFund" class="form-control" readonly />
        </div>

        <div class="mb-3">
            <label class="form-label" for="txtAdjustedCarryOverFund">Adjusted Amount: <span class="required">*</span></label>
            <input data-role="none" type="number" id="txtAdjustedCarryOverFund" class="form-control" />
        </div>

        <div class="mb-3">
            <label class="form-label" for="txtRemarksCarryOverFund">Remarks: <span class="required">*</span></label>
            <textarea data-role="none" type="text" class="form-control" id="txtRemarksCarryOverFund" style="height:100px;"></textarea>
        </div>

        <div class="row mt-4 mb-4">
            <div class="col">
                <button data-role="none" id="btnCancelCarryOverFund" class="btn btn-secondary btn-block btn-lg">Cancel</button>
            </div>
            <div class="col">
                <button data-role="none" id="btnSaveCarryOverFund" class="btn btn-success btn-block btn-lg">Save</button>
            </div>
        </div>
    </div>
</div>

<div id="popLogExpenseMoreDetails" class="modal fade" tabindex="-1">
    <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title">More details from server</h5>
                <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
            </div>
            <div class="modal-body">
                <table data-role="none" style="width:100%;">
                    <tr style="height:50px;">
                        <td style="width:70px;">Period:</td>
                        <td>
                            <select id="selExpenseMoreDetailsPeriod" data-role="none" class="form-select">
                                <option value="today">Today</option>
                                <option value="week">Week-To-Date</option>
                                <option value="month">Month-To-Date</option>
                            </select>
                        </td>
                    </tr>
                    <tr style="height:50px;">
                        <td style="width:70px;">Sort by:</td>
                        <td>
                            <select id="selExpenseMoreDetailsSorting" data-role="none" class="form-select">
                                <option value="ASC">Oldest to Latest</option>
                                <option value="DESC">Latest to Oldest</option>
                            </select>
                        </td>
                    </tr>
                </table>
                <br />
                <ul id="listExpenseReportHeaderMoreDetails" class="list-group" style="margin-bottom: 50px;"></ul>
            </div>
            <div class="modal-footer">
                <div style="display:table; width:100%; font-size:18px;">
                    <div style="display:table-cell; text-align:left; vertical-align:middle; font-size:25px;">Total:</div>
                    <div id="lblTotalPerExpenseTypeMoreDetails" style="display:table-cell; vertical-align:middle; font-size:25px; text-align:right;">0</div>
                </div>
            </div>
        </div>
    </div>
</div>

<div id="popReceiveFundOtherPerson" class="modal fade" tabindex="-1">
    <div class="modal-dialog modal-dialog-centered">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title">Other Person</h5>
                <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
            </div>
            <div class="modal-body">
                <div class="mb-3">
                    <label class="form-label" for="txtFundReceivingDate">Enter Name <span class="required">*</span></label>
                    <input data-role="none" type="text" class="form-control" id="txtFundReceivingOtherPerson">
                </div>
            </div>
            <div class="modal-footer">
                <div class="row">
                    <div class="col">
                        <button data-role="none" class="btn btn-secondary btn-block" data-dismiss="modal">Cancel</button>
                    </div>
                    <div class="col">
                        <button data-role="none" id="btnOkFundOtherPerson" class="btn btn-success btn-block">Ok</button>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>

<div id="popReceivedFundMoreDetails" class="modal fade" tabindex="-1">
    <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title">More details from server</h5>
                <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
            </div>
            <div class="modal-body">
                <ul id="listReceivedFundHeaderMoreDetails" class="list-group" style="margin-bottom: 50px;"></ul>
            </div>
            <div class="modal-footer">
                <div style="display:table; width:100%; font-size:18px;">
                    <div style="display:table-cell; text-align:left; vertical-align:middle; font-size:25px;">Total:</div>
                    <div id="lblTotalReceivedFundMoreDetails" style="display:table-cell; vertical-align:middle; font-size:25px; text-align:right;">0</div>
                </div>
            </div>
        </div>
    </div>
</div>

<div id="popCheckinoutMoreDetails" class="modal fade" tabindex="-1">
    <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title">More details from server</h5>
                <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
            </div>
            <div class="modal-body">
                Location: <span class="bi-geo-alt-fill" style="color:#dc3545;"></span>
                Check-in: <span class="bi-arrow-down-square-fill" style="color:#4CAF50;"></span>
                Check-out: <span class="bi-arrow-up-square-fill" style="color:#FF9800;"></span>
                Duration: <span class="bi-clock-fill" style="color:#FF5722;"></span>
                Remarks: <span class="bi-chat-text-fill" style="color:#2196f3;"></span>
                <br /><br />
                <table data-role="none" style="width:100%;">
                    <tr>
                        <td style="width:70px;">Sort by:</td>
                        <td>
                            <select id="selMoreDetailsCheckinSorting" data-role="none" class="form-select">
                                <option value="ASC">Oldest to Latest</option>
                                <option value="DESC">Latest to Oldest</option>
                            </select>
                        </td>
                    </tr>
                </table>
                <br />
                <ul id="listCheckinoutHeaderMoreDetails" class="list-group" style="margin-bottom: 50px;"></ul>
            </div>
        </div>
    </div>
</div>

<div id="popSelectLocation" class="modal fade" tabindex="-1">
    <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title">Select Location</h5>
                <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
            </div>
            <div class="modal-body">
                <div class="mb-3">
                    <b>Where are you right now?</b>
                </div>

                <input data-role="none" class="form-control mb-3" id="txtSearchLocation" placeholder="Search here" />

                <div id="divSearchLocationLoading" style="display:none; text-align:center;">
                    <div class="spinner-border text-primary" role="status">
                        <span class="visually-hidden"></span>
                    </div>
                </div>

                <div id="divSchedules" subpage="divLocation" style="display:block;">
                    <div class="btn-group btn-toolbar mb-3">
                        <input data-role="none" type="radio" class="btn-check" name="btnsched" id="btnscheduletoday" autocomplete="off" checked>
                        <label class="btn btn-outline-dark" for="btnscheduletoday">Today</label>
                        
                        <input data-role="none" type="radio" class="btn-check" name="btnsched" id="btncheckedinhistory" autocomplete="off">
                        <label class="btn btn-outline-dark" for="btncheckedinhistory">History</label>
                    </div>

                    <div id="scheduletoday" subpage="divSchedules" style="display:block;">
                        <div id="divTodaySched" class="mb-3">
                            <div class="mb-3"> <b>Your schedule:</b> </div>
                            <ul data-role="none" class="list-group mb-3" id="listTodaysSchedule"></ul>
                        </div>
                    </div>
                    <div id="checkedinhistory" subpage="divSchedules" style="display:none;">
                        <div class="mb-3"><b>Your checked-in history:</b></div>
                        <ul data-role="none" class="list-group" id="listLocation"></ul>
                    </div>
                </div>
                
                <div id="divSearchedList" subpage="divLocation" style="display:none;">
                    <ul data-role="none" class="list-group" id="listSearchedLocation"></ul>
                </div>
            </div>
            <div class="modal-footer btnUserDefinedLocation" id="divForUnsearchedLocation" style="display:none;">
                <div>
                    If you’ve correctly spelled your search but can’t find it, you might want to enter your exact location.
                    <span class="text-primary">Tap here to enter your exact location.</span>
                </div>                
            </div>
        </div>
    </div>
</div>

<div id="popSelectLocationOutlet" class="modal fade" tabindex="-1">
    <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title">Search the exact branch location of <span id="lblSelectedLocation"></span> </h5>
                <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
            </div>
            <div class="modal-body">
                <input data-role="none" class="form-control mb-3" id="txtSearchLocationOutlet" placeholder="Search here" />

                <div id="divSearchLocationOutletLoading" style="display:none; text-align:center;">
                    <div class="spinner-border text-primary" role="status">
                        <span class="visually-hidden"></span>
                    </div>
                </div>

                <div id="divSearchedListOutlet" style="display:none;">                    
                    <ul data-role="none" class="list-group" id="listLocationBranch"></ul>
                </div>
            </div>
            <div class="modal-footer btnUserDefinedLocation" id="divForUnsearchedLocationOutlet" style="display:none;">
                <div>
                    If you can’t find the branch above that you're looking for, you might want to enter your exact location.
                    <span class="text-primary">Tap here to enter your exact location.</span>
                </div>
            </div>
        </div>
    </div>
</div>

<div id="popUserDefinedLocation" class="modal fade" tabindex="-1">
    <div class="modal-dialog modal-dialog-centered">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title">Enter your exact Location</h5>
                <button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
            </div>
            <div class="modal-body">
                <input type="text" data-role="none" class="form-control form-control-lg" id="txtUserDefinedExactLocation" placeholder="Enter here" />
            </div>
            <div class="modal-footer">
                <div class="row">
                    <div class="col">
                        <button data-role="none" class="btn btn-secondary btn-lg btn-block" data-dismiss="modal">Cancel</button>
                    </div>
                    <div class="col">
                        <button data-role="none" id="btnOkUDExactLocation" class="btn btn-success btn-lg btn-block" data-dismiss="modal">Ok</button>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>