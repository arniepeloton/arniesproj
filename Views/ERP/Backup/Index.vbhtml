@Code
    ViewData("Title") = "Mobiz EAS"
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code
@Section head
    <link rel="stylesheet" href="/Content/flatui-css-custom.css" />
    <link rel="stylesheet" href="/Content/bootstrap-custom.css" />
    <link rel="stylesheet" href="/Content/jquery.dataTables2.css" />
    <link rel="stylesheet" href="/Content/jquery.buttons.dataTables.min.css" />
    <script>
        require([GetPathFormat('/Scripts/sources/ERP/ERP.js?crc=')]);
    </script>

    <style type="text/css">
        .samplebtn {
            height: 100px;
            text-align: center;
            vertical-align: middle;
            font-size: 18px;
            border-radius: 3px;
            cursor: pointer;
            border: 1px solid #f2f2f2;
            display: flex;
            justify-content: center;
            align-items: center;
        }

        .erp-main-grid {
            text-align: center;
            margin-top: 15px;
            padding: 5px;
            border: 3px solid transparent;
        }

            .erp-main-grid:hover {
                border: 3px solid #CCC;
                border-radius: 2px;
                cursor: pointer;
            }

        .btn-trail {
            padding: 3px;
            display: inline-block;
            margin-right: 5px;
            text-align: center;
            text-shadow: none !important;
            font-weight: bold;
            color: #000;
            background-color: #FFF;
        }

            .btn-trail > span {
                font-size: 10px;
            }

            .btn-trail:hover {
                background-color: #3498db;
                color: #fff;
                cursor: pointer;
            }

        .elementsForErp {
            display: inline !important;
        }

        .dashboardGrid {
            background-color: #fffafa;
            height: 150px;
            padding: 10px;
            border-radius: 2px;
            border: 2px solid rgba(0,0,0, 0);
        }

            .dashboardGrid:hover {
                border: 2px solid #3498db;
                cursor: pointer;
            }

            .dashboardGrid > .gHeader {
                font-weight: bold;
                color: #3498db;
            }

            .dashboardGrid > .gBody {
                font-size: 25px;
                word-break: break-word;
                margin: 15px 0;
                text-align: center;
                color: #2c3e50;
            }

            .dashboardGrid > .gFooter {
                color: #b5b5b5;
                text-align: center;
                /*bottom:10px;
            position:absolute;*/
            }
    </style>
End Section

<div>
    <div id="popupImageGallery" custom-data-role="popup">
        <div class="popup-md">
            <div class="popup-header">
                File Attachments
                <a class="popup-close">x</a>
            </div>

            <div class="popup-content">
            </div>

            <div class="popup-footer">
                <button id="" data-role="none" class="btn custom-btn-theme-c btnCircle popup-close">
                    <img alt="Options" src="/Content/images/close-black.png" />
                </button>
            </div>
        </div>
    </div>

    <div data-role="popup" id="popupNewAdjustment" data-overlay-theme="a" data-theme="a" data-corners="false">
        <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
        <center><h3 style="margin-left:10px; margin-right:10px;">New Account Adjustment</h3></center>
        <div data-role="content" class="ui-content ui-body-d jPopupContent" style="width:1000px">
            <div class="ui-grid-a">
                <div class="ui-block-a">
                    <div class="custom-ui-label">
                        <div>Invoice to Adjust: <span class="required">*</span></div>
                        <button id="butInvoiceToAdjust">Select</button>
                        @*<span id="butInvoiceToAdjust">Select</span>*@
                    </div>

                    <div class="custom-ui-label">
                        <div>Invoice to Date:</div>
                        <div><span id="spanAdjustmentInvoiceToDate"></span></div>
                    </div>

                    <div class="custom-ui-label">
                        <div>Customer name:</div>
                        <div><span id="spanAdjustmentCustomer"></span></div>
                    </div>

                    <div class="custom-ui-label">
                        <div>Reason for Adjustment: <span class="required">*</span></div>
                        <div>
                            <select id="cbReasonForAdjustment" data-role="none" class="form-control">
                                <option value="">Please select a reason</option>
                            </select>
                        </div>
                    </div>

                    <div id="divAccountAdjustmentChecks" style="margin-bottom:20px">
                        <table name="table">
                            <thead>
                                <tr>
                                    <th>Bank</th>
                                    <th>Check Number</th>
                                    <th>Check Amount</th>
                                    <th>Check Date</th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr>
                                    <td name="bank"></td>
                                    <td name="checkNumber"></td>
                                    <td name="checkAmount"></td>
                                    <td name="checkDate"></td>
                                </tr>
                            </tbody>
                        </table>

                        <button name="butAdd" data-role="none" class="btn custom-btn-theme-b">Add</button>
                    </div>

                    <div class="custom-ui-label">
                        <div>Adjustment Amount: <span class="required">*</span></div>
                        <div><input id="txtAdjustmentAmount" type="number" data-role="none" class="form-control" /></div>
                    </div>

                    <div class="custom-ui-label">
                        <div>Remarks:</div>
                        <div><textarea id="txtAdjustmentRemarks" data-role="none" class="form-control"></textarea></div>
                    </div>
                </div>
                <div class="ui-block-b" style="padding-left:60px">
                    <div class="custom-ui-label">
                        <div>Invoice Gross Amount:</div>
                        <div><span id="spanAccountAdjustmentGrossInvoiceAmount">WTH36466Q5</span></div>
                    </div>
                    <div class="custom-ui-label">
                        <div>Invoice Net Amount (Of Adjustments):</div>
                        <div><span id="spanAccountAdjustmentNetInvoiceAmount">WTH36466Q5</span></div>
                    </div>
                    <div class="custom-ui-label">
                        <div>Net Invoice Balance:</div>
                        <div><span id="spanAccountAdjustmentNetInvoiceBalance">WTH36466Q5</span></div>
                    </div>
                </div>
            </div>
                    <br /><br />
                    <button id="btnAddAdjustment" data-role="none" class="btn custom-btn-theme-b">Save</button>
                    <br /><br />
                    <a data-role="none" data-rel="back" href="#" class="btn custom-btn-theme-c">Cancel</a>
                </div>
            </div>

        </div>

<div id="divEasHeader" style="display:none;">
    <div class="custom-theme-a">
        <div id="HeaderDesktopVersion2" class="container">
            <div class="ui-grid-a" style="height:53px;">
                <div class="ui-block-a" style="padding-right:9px; overflow:hidden; display:flex;">
                    <div style="float:left; margin-top:10px; margin-bottom:0px; overflow:auto;">
                        <div class="OrganizationNameHeader" style="text-overflow:ellipsis; font-size:25px; white-space:nowrap; overflow:hidden;">
                            Company Name
                        </div>
                    </div>
                </div>
                <div class="ui-block-b" style="padding:0px 0px 0px 20px;">
                    <div class="ui-grid-a">
                        <div class="ui-block-a">
                            <div style="margin-top:10px; margin-bottom:0px; text-overflow:ellipsis; white-space:nowrap; overflow:hidden;">
                                <label>FOWST:</label> <br />
                                <a data-role="none" id="lblWorkFowstName" class="custom-link" style="color:#FFF;"></a>
                            </div>
                        </div>
                        <div class="ui-block-b">
                            <div style="float:right; cursor:pointer; margin-top:7px;" id="btnERPPeopleMenu">
                                <span class="ui-icon ui-icon-arrow-d" style="display:inline-block; float:right; margin-top:11px; margin-left:10px;"></span>
                                <div id="txtLoggedUserName" style="margin-top:10px; margin-left:20px; float:right;"></div>

                                <div style="height:40px; width:40px; float:right; margin-left: 20px; margin-top: 0px;">
                                    <img id="imgProfilePic" src="/Content/images/NoPhoto.jpg" alt="No Photo" width="40px" height="40px" />
                                </div>
                            </div>
                            <div class="dropdownMenu" style="box-shadow:0px 10px 10px #CCC; display:none; top:50px;">
                                <div class="ui-grid-a" style=" color:#FFF;">
                                    <div class="ui-block-a sixtyfourty-block-b" style="padding:15px;" align="center">
                                        <img id="imgProfilePicLarge" src="/Content/images/NoPhoto.jpg" alt="No Photo" width="100px" height="100px" style="cursor:pointer;" />
                                    </div>
                                    <div class="ui-block-b sixtyfourty-block-a" style="padding:15px;">
                                        <div id="btnViewProfile" style="cursor:pointer;">View Profile</div>
                                            <br />
                                            @*<div id="btnDBSettings" style="cursor:pointer;">Settings</div>
    <br />*@
                                        <div id="btnERPLogOut" style="cursor:pointer;">Log Out</div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div id="HeaderMobileVersion2" style="display:none;">
            <div class="ui-grid-solo">
                <div class="ui-block-a OrganizationNameHeader txt-center" style="white-space:nowrap; overflow:hidden; text-overflow:ellipsis;">
                    Company Name Here
                </div>
            </div>
            <div class="ui-grid-b">
                <div class="ui-block-a">
                    <a class="btn custom-btn-theme-a btnHome">
                        <span class="ui-icon ui-icon-home" style="display:inline-block;">
                            <p style="margin:16px 0px 0px 0px; position:relative; transform: translate(-50%, 50%); font-size:12px;">Home</p>
                        </span>
                    </a>
                </div>
                <div class="ui-block-b">
                    <a id="btnSearchMobile" class="btn custom-btn-theme-a">
                        <span class="ui-icon ui-icon-search" style="display:inline-block;">
                            <p style="margin:16px 0px 0px 0px; position:relative; transform: translate(-50%, 50%); font-size:12px;">Search</p>
                        </span>
                    </a>
                </div>
                <div class="ui-block-c">
                    <a class="btn custom-btn-theme-a btnSettings">
                        <span class="ui-icon ui-icon-gear" style="display:inline-block;">
                            <p style="margin:16px 0px 0px 0px; position:relative; transform: translate(-50%, 50%); font-size:12px;">Settings</p>
                        </span>
                    </a>
                </div>
            </div>
            <div class="contentSearchEngine">
                <div class="ui-grid-solo ui-responsive">
                    <div class="ui-block-a" style="padding:6px;">
                        <input type="text" placeholder="Search Here" class="txtSearchProspect" />
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div class="BreadCrumbs">
        <div class="container">
            <div class="ItemsBreadCrumbs">
                Home
            </div>
        </div>
    </div>

    <br />
</div>

<div id="divErpMain" subpage="divErpMain" class="container" style="display:none;">
    <div class="row">
        <div class="col-sm-3">
            <div class="erp-main-grid" id="btnErpRevenue">
                <img src="/Content/images/revenue.png" />
                <br /><br />
                Revenue
            </div>
        </div>
        <div class="col-sm-3">
            <div class="erp-main-grid" id="btnErpDisbursement">
                <img src="/Content/images/disbursement.png" />
                <br /><br />
                Disbursement
            </div>
        </div>
        <div class="col-sm-3">
            <div class="erp-main-grid" id="btnErpInventoryHandling">
                <img src="/Content/images/inventorycontrol.png" />
                <br /><br />
                Inventory Handling & Controls
            </div>
        </div>
        <div class="col-sm-3">
            <div class="erp-main-grid" id="btnErpDTR">
                <img src="/Content/images/dtr.png" />
                <br /><br />
                DTR & HR
            </div>
        </div>
    </div>

    <div class="row">
        <div class="col-sm-3">
            <div class="erp-main-grid" id="btnErpProjectManagement">
                <img src="/Content/images/projectmgmt.png" />
                <br /><br />
                Project Management
            </div>
        </div>
        <div class="col-sm-3">
            <div class="erp-main-grid" id="btnErpCustomerSupport">
                <img src="/Content/images/rev-marketing.png" />
                <br /><br />
                CRM
            </div>
        </div>
        <div class="col-sm-3">
            <div class="erp-main-grid" id="btnErpAssetsMgmt">
                <img src="/Content/images/AssetManagement.png" style="width:96px;" />
                <br /><br />
                Asset Management
            </div>
        </div>
        <div class="col-sm-3">
            <div class="erp-main-grid" id="btnErpGPS">
                <img src="/Content/images/track.png" />
                <br /><br />
                Fleet Tracking & Management
            </div>
        </div>
    </div>

    <div class="row">
        <div class="col-sm-3">
            <div class="erp-main-grid btnOngoing">
                <img src="/Content/images/financialstmt.png" />
                <br /><br />
                Financial Statements
            </div>
        </div>
        <div class="col-sm-3">
            <div class="erp-main-grid" id="btnErpSettings">
                <img src="/Content/images/Settings (2).png" />
                <br /><br />
                System Settings
            </div>
        </div>
    </div>
</div>

<div id="divInventoryHandling" subpage="divErpMain" class="container" style="display:none;">

</div>

<div id="divDailyTimeRecord" subpage="divErpMain" class="container" style="display:none;">

</div>

<div id="divCustomerSupport" subpage="divErpMain" class="container" style="display:none;">

</div>

<div id="divGPS" subpage="divErpMain" class="container" style="display:none;">

</div>

<div id="divSystemSettings" subpage="divErpMain" class="container" style="display:none;">

</div>

<div id="divProjectManagement" subpage="divErpMain" class="container" style="display:none;">

</div>

<div id="divErpRevenue" subpage="divErpMain" class="container" style="display:none;">
    <div id="divRevMainMenu" subpage="divErpRevenue" style="display:none;">
        <div id="divTabRevenue" class="custom-tab">
            <ul>
                <li id="btnTabRevenueDashboard" class="active" custom-tab-content="#tabRevenueDashboard">Revenue Module Dashboard</li>
                <li custom-tab-content="#tabRevenueWork">Revenue Modules</li>
                <li custom-tab-content="#tabRevenueSettings">Revenue Module Settings</li>
            </ul>
        </div>
        <div id="tabRevenueDashboard" custom-tab-role="content" style="display:block;">
            <div class="row">
                <div class="col-xs-12 col-sm-6 col-md-3">
                    <div class="dashboardGrid" id="btnReportNetSales">
                        <div class="gHeader">
                            NET SALES
                        </div>
                        <div class="gBody" id="lblNetSalesBody">
                            0
                        </div>
                        <div class="gFooter" id="lblNetSalesFooter">

                        </div>
                    </div>
                </div>
                <div class="col-xs-12 col-sm-6 col-md-3">
                    <div class="dashboardGrid" id="btnReportCac">
                        <div class="gHeader">
                            Credit and Collection
                        </div>
                        <div class="gBody" id="lblCacBody">
                            0
                        </div>
                        <div class="gFooter" id="lblCacFooter">

                        </div>
                    </div>
                </div>
                <div class="col-xs-12 col-sm-6 col-md-3">
                    <div class="dashboardGrid" id="btnReportSopa">
                        <div class="gHeader">
                            S.O.P.A
                        </div>
                        <div class="gBody" id="lblSopaBody">
                            0
                        </div>
                        <div class="gFooter" id="lblSopaFooter">

                        </div>
                    </div>
                </div>

                <div class="col-xs-12 col-sm-6 col-md-3">
                    <div class="dashboardGrid" id="butReportProfit">
                        <div class="gHeader">
                            Gross Profitability Report
                        </div>
                        <div class="gBody" name="lblSopaBody">
                            0
                        </div>
                        <div class="gFooter" name="lblSopaFooter">

                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div id="tabRevenueWork" custom-tab-role="content" style="display:none;">
            <div class="row">
                @*<div class="col-sm-3">
                        <div class="erp-main-grid" id="btnRevMarketing">
                            <img src="/Content/images/rev-marketing.png" />
                            <br /><br />
                            Marketing CRM
                        </div>
                    </div>*@
                <div class="col-sm-3">
                    <div class="erp-main-grid" id="btnRevQuotation">
                        <span class="erp-quote new"></span>
                        <br /><br />
                        Quotation
                    </div>
                </div>
                <div class="col-sm-3">
                    <div class="erp-main-grid" id="btnRevOrderProcessing">
                        <img src="/Content/images/rev-orderprocessing.png" />
                        <br /><br />
                        Order Processing
                    </div>
                </div>
                <div class="col-sm-3">
                    <div class="erp-main-grid" id="btnRevOrderFulfillment">
                        <img src="/Content/images/rev-fulfillment.png" />
                        <br /><br />
                        Order Fulfillment
                    </div>
                </div>
                <div class="col-sm-3">
                    <div class="erp-main-grid" id="btnRevInvoicing">
                        <img src="/Content/images/rev-invoicing.png" />
                        <br /><br />
                        Invoicing
                    </div>
                </div>
            </div>

            <div class="row">

                <div class="col-sm-3">
                    <div class="erp-main-grid btnOngoing">
                        <img src="/Content/images/rev-truck.png" />
                        <br /><br />
                        Delivery Tracking
                    </div>
                </div>
                <div class="col-sm-3">
                    <div class="erp-main-grid" id="btnRevCreditAndCollection">
                        <img src="/Content/images/rev-cac.png" />
                        <br /><br />
                        Credit and Collection
                    </div>
                </div>
                <div class="col-sm-3">
                    <div class="erp-main-grid" id="butRevDeposits">
                        <img src="/Content/images/deposits.png" />
                        <br /><br />
                        Deposits
                    </div>
                </div>
                <div class="col-sm-3">
                    <div class="erp-main-grid" id="btnRevenuePriceCanvassing">
                        <img src="/Content/images/disburse-pricecanvass.png" />
                        <br /><br />
                        Competitor Price Canvassing
                    </div>
                </div>
                @*<div class="col-sm-3">
                    <div class="erp-main-grid btnOngoing">
                        <img src="/Content/images/cheque.png" />
                        <br /><br />
                        Cheque Clearing
                    </div>
                </div>*@
            </div>
        </div>

        <div id="tabRevenueSettings" custom-tab-role="content" style="display:none;">

        </div>
    </div>


    <div id="divOrderFulfillment" subpage="divErpRevenue" style="display:none;">
        <div id="divOrderFulfillmentMain" subpage="divOrderFulfillment" style="display:block;">
            <div id="divOrderFulfillmentTab" class="custom-tab">
                <ul>
                    <li class="active" custom-tab-content="#tabOFDashboard">Order Fulfillment Dashboard</li>
                    <li custom-tab-content="#tabOFWork">Order Fulfillment</li>
                    <li custom-tab-content="#tabOFSettings">Order Fulfillment Settings</li>
                </ul>
            </div>
            <div id="tabOFDashboard" custom-tab-role="content" style="display:block;">
                
            </div>
            <div id="tabOFWork" custom-tab-role="content" style="display:none;">
                <div id="divOrderFulfillmentWork" style="padding-top:20px"></div>
                @*<div>
                    <div title="New" class="custom-btn-theme-c btn-trail">
                        <img src="/Content/images/neworder.png" width="40px" /><br />
                        <span>New</span>
                    </div>
                    <div title="Lookup" class="custom-btn-theme-c btn-trail">
                        <img src="/Content/images/lookuporder.png" width="40px" /><br />
                        <span>Lookup</span>
                    </div>
                    <div title="Save & Finalize" class="custom-btn-theme-c btn-trail">
                        <img src="/Content/images/finalsave.png" width="40px" /><br />
                        <span>Save & Finalize</span>
                    </div>
                    <div title="Save a Copy" class="custom-btn-theme-c btn-trail">
                        <img src="/Content/images/draftsave.png" width="40px" /><br />
                        <span>Save a Copy</span>
                    </div>
                    <div title="Save as Draft" class="custom-btn-theme-c btn-trail">
                        <img src="/Content/images/draftsave.png" width="40px" /><br />
                        <span>Save as Draft</span>
                    </div>
                    <div title="Output" class="custom-btn-theme-c btn-trail">
                        <img src="/Content/images/cacprint.png" width="40px" /><br />
                        <span>Output</span>
                    </div>
                </div>

                <br /><br />

                <div id="accRevOrderHeader" custom-data-role="accordion">
                    <div class="accordion-header active">
                        Order Header
                    </div>
                    <div class="accordion-content">
                        <div id="divOFWorkTab" class="custom-tab">
                            <ul>
                                <li class="active" custom-tab-content="#tabOFWorkBasicInfo">Basic Info</li>
                            </ul>
                        </div>

                        <div id="tabOFWorkBasicInfo" custom-tab-role="content" style="display:block;">
                            <div class="row">
                                <div class="col-sm-8">
                                    <div class="row">
                                        <div class="col-sm-6">
                                            <div class="form-group">
                                                <label>Customer: <span class="required">*</span></label>
                                                <input data-role="none" type="text" class="form-control" />
                                            </div>
                                        </div>
                                        <div class="col-sm-6">
                                            <div class="form-group">
                                                <label>Sales Partner:</label>
                                                <input data-role="none" type="text" class="form-control" />
                                            </div>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-sm-6">
                                            <div class="form-group">
                                                <label>Quote Number:</label>
                                                <input data-role="none" type="text" class="form-control" />
                                            </div>
                                        </div>
                                        <div class="col-sm-6">
                                            <div class="form-group">
                                                <label>Sales Rep:</label>
                                                <input data-role="none" type="text" class="form-control" />
                                            </div>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-sm-6">
                                            <div class="form-group">
                                                <label>Sales Order Number:</label>
                                                <input data-role="none" type="text" class="form-control" />
                                            </div>
                                        </div>
                                        <div class="col-sm-6">
                                            <div class="form-group">
                                                <label>Payment Terms:</label>
                                                <input data-role="none" type="text" class="form-control" />
                                            </div>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-sm-6">
                                            <div class="row">
                                                <div class="col-sm-6">
                                                    <div class="form-group">
                                                        <label>PO Number: <span class="required">**</span></label>
                                                        <input data-role="none" type="text" class="form-control" />
                                                    </div>
                                                </div>
                                                <div class="col-sm-6">
                                                    <div class="form-group">
                                                        <label>PO Date: <span class="required">**</span></label>
                                                        <input data-role="none" type="text" id="btnOFPODate" class="form-control" readonly />
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="col-sm-6">
                                            <div class="form-group">
                                                <label>Date Needed:</label>
                                                <input data-role="none" type="text" id="btnOFDateNeeded" class="form-control" readonly />
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-sm-4">
                                    <div class="form-group">
                                        <label>Billing Address:</label>
                                        <div style="margin-top:-5px; display:inline-block; float:right;">
                                            <button data-role="none" class="btn custom-btn-theme-b btn-less-width btn-xs" style="float:right;"><img class="img-plus-white" /></button>
                                            <button data-role="none" class="btn custom-btn-theme-b btn-less-width btn-xs" style="float:right;"><img class="img-search-white" /></button>
                                        </div>
                                        <textarea data-role="none" class="form-control" style="height:90px; max-height:90px; resize:none"></textarea>
                                    </div>
                                    <div class="form-group">
                                        <label>Delivery Address:</label>
                                        <div style="margin-top:-5px; display:inline-block; float:right;">
                                            <button data-role="none" class="btn custom-btn-theme-b btn-less-width btn-xs" style="float:right;"><img class="img-plus-white" /></button>
                                            <button data-role="none" class="btn custom-btn-theme-b btn-less-width btn-xs" style="float:right;"><img class="img-search-white" /></button>
                                        </div>
                                        <textarea data-role="none" class="form-control" style="height:90px; max-height:90px; resize:none"></textarea>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <br /><br />

                <div id="accRevOrderLine" custom-data-role="accordion">
                    <div class="accordion-header active">
                        Order Line Items
                    </div>
                    <div class="accordion-content">
                        <div style="overflow:auto;">
                            <table data-role="table" data-role="reflow" style="margin-right:5px; display:table;" class="ui-table ui-responsive table-stroke table-stripe custom-tbl-responsive">
                                <thead>
                                    <tr class="custom-theme-f fixed-cell-width">
                                        <th class="txt-center"> SKU # </th>
                                        <th class="txt-center"> Product Description </th>
                                        <th class="txt-center"> Order Qty </th>
                                        <th class="txt-center"> Packing Size </th>
                                        <th class="txt-center"> Unit Price </th>
                                        <th class="txt-center"> Line Total </th>
                                        <th class="txt-center"> Payment Terms </th>
                                    </tr>
                                </thead>
                                <tbody></tbody>
                            </table>
                        </div>
                    </div>
                </div>

                <br /><br />

                <div id="accRevOrderLine" custom-data-role="accordion">
                    <div class="accordion-header active">
                        Order Footer
                    </div>
                    <div class="accordion-content">
                        <div class="row">
                            <div class="col-sm-4">
                                <div class="form-group">
                                    <label>Remarks:</label>
                                    <textarea data-role="none" class="form-control" style="max-height:55px; resize:none"></textarea>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>*@

            </div>
            <div id="tabOFSettings" custom-tab-role="content" style="display:none;">
                Settings
            </div>
        </div>
    </div>


    <div id="divCreditAndCollection" subpage="divErpRevenue" style="display:none;">

    </div>

    <div id="divRevenuePriceCanvass" subpage="divErpRevenue" style="display:none;">
        <div id="divRevenuePriceCanvassMain" subpage="divRevenuePriceCanvass" style="display:block;">
            Loading ...
        </div>
    </div>
</div>

<div id="divErpDisbursement" subpage="divErpMain" class="container" style="display:none;">
    <div id="divDisburseMainMenu" subpage="divErpDisbursement" style="display:block;">
        <div class="row">
            <div class="col-sm-3">
                <div class="erp-main-grid" id="btnDisbursePriceCanvassing">
                    <img src="/Content/images/disburse-pricecanvass.png" />
                    <br /><br />
                    Vendor Price Canvassing
                </div>
            </div>
            <div class="col-sm-3">
                <div class="erp-main-grid" id="btnDisbursePurchasing">
                    <img src="/Content/images/disburse-po.png" />
                    <br /><br />
                    Purchasing
                </div>
            </div>
            <div class="col-sm-3">
                <div class="erp-main-grid" id="btnDisburseJO">
                    <img src="/Content/images/jo.png" />
                    <br /><br />
                    Job Order
                </div>
            </div>
            <div class="col-sm-3">
                <div class="erp-main-grid" id="butDisbursmentStockReceiving">
                    <img style="width:96px;" src="/Content/images/disburse-stockreceiving.png" />
                    <br /><br />
                    Stock Receiving
                </div>
            </div>
        </div>

        <div class="row">
            <div class="col-sm-3">
                <div class="erp-main-grid" id="butDisbursementAccountsPayable">
                    <img src="/Content/images/disburse-apverify.png" />
                    <br /><br />
                    Accounts Payable Verification
                </div>
            </div>
            <div class="col-sm-3">
                <div class="erp-main-grid">
                    <img src="/Content/images/disburse-main.png" />
                    <br /><br />
                    Disbursements
                </div>
            </div>
            <div class="col-sm-3">
                <div class="erp-main-grid btnOngoing">
                    <img src="/Content/images/cheque.png" />
                    <br /><br />
                    Cheque Clearing
                </div>
            </div>
        </div>
    </div>

    <div id="divPriceCanvass" subpage="divErpDisbursement" style="display:none;">
        <div id="divPriceCanvassMain" subpage="divPriceCanvass" style="display:block;">
            Loading ...
        </div>
    </div>

    <div id="divPurchasing" subpage="divErpDisbursement" style="display:none;">
        <div id="divPurchasingMain" subpage="divPurchasing" style="display:block;">
            <div id="divPurchasingTab" class="custom-tab">
                <ul>
                    <li class="active" custom-tab-content="#tabPurchasingDashboard">Purchasing Dashboard</li>
                    <li custom-tab-content="#tabPurchasingWork">Purchasing</li>
                    <li custom-tab-content="#tabPurchasingSettings">Purchasing Settings</li>
                </ul>
            </div>
            <div id="tabPurchasingDashboard" custom-tab-role="content" style="display:block;">
                Dashboard
            </div>
            <div id="tabPurchasingWork" custom-tab-role="content" style="display:none;">
                <div class="clearfix">
                    <div style="float: left">
                        <div name="butNew" title="New" class="btn-trail">
                            <span class="erp-po new s48"></span><br />
                            <span>New</span>
                        </div>
                        <div name="butLookup" title="Lookup" class="btn-trail">
                            <span class="erp-po search s48"></span><br />
                            <span>Lookup</span>
                        </div>
                        <div name="butSaveAndFinalize" title="Save & Finalize" class="btn-trail">
                            <span class="erp-po finalsave s48"></span><br />
                            <span>Save & Finalize</span>
                        </div>
                        <div name="butSaveACopy" title="Save a Copy" class="btn-trail">
                            <span class="erp-po saveacopy s48"></span><br />
                            <span>Save a Copy</span>
                        </div>
                        <div name="butSaveAsDraft" title="Save as Draft" class="btn-trail">
                            <span class="erp-po saveasdraft s48"></span><br />
                            <span>Save as Draft</span>
                        </div>
                        <div name="butOutput" title="Output" class="btn-trail">
                            <span class="erp-po print s48"></span><br />
                            <span>Output</span>
                        </div>
                        <div name="butPay" title="Pay" class="btn-trail">
                            <span class="erp-po pay s48"></span><br />
                            <span>Pay</span>
                        </div>
                        <div name="butCancel" title="Cancel" class="btn-trail">
                            <span class="erp-invoice-cancel"></span><br />
                            <span>Cancel</span>
                        </div>
                    </div>
                    <div style="float:right; font-size:50px">
                        <div name="divTransNum" style="font-size:30px"></div>
                        <div name="divDraftOrFinalized"></div>
                    </div>
                </div>
                <br /><br />

                <div id="accRevOrderHeader" custom-data-role="accordion">
                    <div class="accordion-header active">
                        Order Header
                    </div>
                    <div class="accordion-content">
                        <div id="divPurchasingWorkTab" class="custom-tab">
                            <ul>
                                <li class="active" custom-tab-content="#tabPurchasingWorkInfo">Basic Info</li>
                            </ul>
                        </div>

                        <div id="tabPurchasingWorkInfo" custom-tab-role="content" style="display:block;">
                            <div class="row">
                                <div class="col-sm-8">
                                    <div class="row">
                                        <div class="col-sm-6">
                                            <div class="form-group">
                                                <label>Vendor: <span class="required">*</span></label>
                                                <input name="txtCustomer" data-role="none" type="text" class="form-control" />
                                            </div>
                                        </div>
                                        <div class="col-sm-6">
                                            <div class="form-group">
                                                <label>Sales Rep:</label>
                                                <input name="txtSalesRepText" data-role="none" type="text" class="form-control" />
                                            </div>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-sm-6">
                                            <div class="form-group">
                                                <label>Canvass Number:</label>
                                                <div style="margin-top:-5px; display:inline-block; float:right;">
                                                    <button name="butPoCanvass" data-role="none" class="btn custom-btn-theme-b btn-less-width btn-xs" style="float:right;"><img class="img-search-white" /></button>
                                                </div>
                                                <input name="txtCanvass" data-role="none" type="text" class="form-control" />
                                            </div>
                                        </div>
                                        <div class="col-sm-6">
                                            <div class="form-group">
                                                <label>PO Number:</label>
                                                <input name="txtPoNumber" readonly data-role="none" placeholder="To be generated after saving as final" type="text" class="form-control" />
                                            </div>
                                        </div>
                                        <div class="col-sm-6">
                                            <div class="form-group">
                                                <label>Payment Terms:</label>
                                                <input name=txtPaymentTerms data-role="none" type="text" class="form-control" />
                                            </div>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-sm-6">
                                            <div class="row">
                                                <div class="col-sm-6">
                                                    <div class="form-group">
                                                        <label>Quote Number:</label>
                                                        <input name=txtQuotationNumText data-role="none" type="text" class="form-control" />
                                                    </div>
                                                </div>
                                                <div class="col-sm-6">
                                                    <div class="form-group">
                                                        <label>Quote Date:</label>
                                                        <input name="txtQuoteDate" data-role="none" type="text" id="btnPurchasingQuoteDate" class="form-control" readonly />
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="col-sm-6">
                                            <div class="form-group">
                                                <label>Date Needed: <span class="required">*</span></label>
                                                <div style="margin-top:-5px; display:inline-block; float:right;">
                                                    <button name="butDateNeeded" data-role="none" class="btn custom-btn-theme-b btn-less-width btn-xs" style="float:right;"><img class="img-search-white" /></button>
                                                </div>
                                                <input name="txtDateNeeded" data-role="none" type="text" idx="btnPurchasingDateNeeded" class="form-control" />
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-sm-4">
                                    <div class="form-group">
                                        <label>Billing Address: <span class="required">*</span></label>
                                        <div style="margin-top:-5px; display:inline-block; float:right;">
                                            <button name="butAddBillingAddress" data-role="none" class="btn custom-btn-theme-b btn-less-width btn-xs" style="float:right;"><img class="img-plus-white" /></button>
                                            <button name="butSearchBillingAddress" data-role="none" class="btn custom-btn-theme-b btn-less-width btn-xs" style="float:right;"><img class="img-search-white" /></button>
                                        </div>
                                        <textarea name="txtBillingAddress" data-role="none" class="form-control" style="max-height:55px; resize:none"></textarea>
                                    </div>
                                    <div class="form-group">
                                        <label>Delivery Address:<span class="required">*</span></label>
                                        <div style="margin-top:-5px; display:inline-block; float:right;">
                                            <button name="butAddDeliveryAddress" data-role="none" class="btn custom-btn-theme-b btn-less-width btn-xs" style="float:right;"><img class="img-plus-white" /></button>
                                            <button name="butSearchDeliveryAddress" data-role="none" class="btn custom-btn-theme-b btn-less-width btn-xs" style="float:right;"><img class="img-search-white" /></button>
                                        </div>
                                        <textarea name="txtDeliveryAddress" data-role="none" class="form-control" style="max-height:55px; resize:none"></textarea>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <br /><br />

                <div id="accRevOrderLine" custom-data-role="accordion">
                    <div class="accordion-header active">
                        Transaction Line Items
                        <span class="txt-right">
                            <a name="butAddProduct" data-role="none" class="custom-link">Add product</a>
                        </span>
                    </div>
                    <div class="accordion-content">
                        <div style="overflow:auto;">
                            <table name="tableProducts" data-role="table" data-role="reflow" style="margin-right: 5px; display: table;" class="ui-table ui-responsive table-stroke table-stripe custom-tbl-responsive">
                                <thead>
                                    <tr class="custom-theme-f fixed-cell-width">
                                        <th class="txt-center"> SKU # </th>
                                        <th class="txt-center"> Product Description </th>
                                        <th class="txt-center"> PO Qty </th>
                                        <th name="thServedQty" class="txt-center"> Received Qty in Good Condition </th>
                                        <th name="thServedBadQty" class="txt-center"> Received Qty in Bad Condition </th>
                                        <th class="txt-center"> Packing Size </th>
                                        <th class="txt-center"> PO Price </th>
                                        <th class="txt-center"> Line Total </th>
                                        <th class="txt-center"> Payment Terms </th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <tr name="data" class="fixed-cell-width">
                                        <td name="ProductNumber" class="txt-center"> SKU # </td>
                                        <td name="ProductDescription" class="txt-center"> Product Description </td>
                                        <td name="OrderQty" class="txt-center"> PO Qty </td>
                                        <td name="ServedQty" class="txt-center"> Received Qty </td>
                                        <td name="ServedBadQty" class="txt-center"> Received Qty </td>
                                        <td name="PackingSize" class="txt-center"> Packing Size </td>
                                        <td name="UnitPrice" class="txt-center"> Unit Price </td>
                                        <td name="LineTotal" class="txt-center"> Line Total </td>
                                        <td name="PaymentTerms" class="txt-center"> Payment Terms </td>
                                    </tr>
                                </tbody>
                                <tfoot>
                                    <tr name="data" class="fixed-cell-width">
                                        <td name="ProductNumber" class="txt-center"></td>
                                        <td name="ProductDescription" class="txt-center"></td>
                                        <td name="OrderQty" class="txt-center"></td>
                                        <td name="ServedQty" class="txt-center"></td>
                                        <td name="PackingSize" class="txt-center"></td>
                                        <td name="UnitPrice" class="txt-center"></td>
                                        <td name="LineTotal" class="txt-center"></td>
                                        <td name="PaymentTerms" class="txt-center"></td>
                                    </tr>
                                </tfoot>
                            </table>
                            @*<button name="butAddProduct">Add Product</button>*@
                        </div>
                    </div>
                </div>

                <br /><br />

                <div id="accRevOrderLine" custom-data-role="accordion">
                    <div class="accordion-header active">
                        Order Footer
                    </div>
                    <div class="accordion-content">
                        <div class="row">
                            <div class="col-sm-4">
                                <div class="form-group">
                                    <label>Remarks:</label>
                                    <textarea name=txtRemarks data-role="none" class="form-control" style="max-height:55px; resize:none"></textarea>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

            </div>
            <div id="tabPurchasingSettings" custom-tab-role="content" style="display:none;">
                Settings
            </div>
        </div>
    </div>

    <div id="divJO" subpage="divErpDisbursement" style="display:none;">
        <div id="divJOMain" subpage="divJO" style="display:block;">
            <div id="divPurchasingTab" class="custom-tab">
                <ul>
                    <li class="active" custom-tab-content="#tabJODashboard">Job Order Dashboard</li>
                    <li custom-tab-content="#tabJOWork">Job Order</li>
                    <li custom-tab-content="#tabJOSettings">Job Order Settings</li>
                </ul>
            </div>
            <div id="tabJODashboard" custom-tab-role="content" style="display:block;">
                Dashboard
            </div>
            <div id="tabJOWork" custom-tab-role="content" style="display:none;">
                <div>
                    <div title="New" class="btn-trail">
                        <span class="erp-jo new s48"></span><br />
                        <span>New</span>
                    </div>
                    <div title="Lookup" class="btn-trail">
                        <span class="erp-jo search s48"></span><br />
                        <span>Lookup</span>
                    </div>
                    <div title="Save & Finalize" class="btn-trail">
                        <span class="erp-jo finalsave s48"></span><br />
                        <span>Save & Finalize</span>
                    </div>
                    <div title="Save a Copy" class="btn-trail">
                        <span class="erp-jo saveacopy s48"></span><br />
                        <span>Save a Copy</span>
                    </div>
                    <div title="Save as Draft" class="btn-trail">
                        <span class="erp-jo saveasdraft s48"></span><br />
                        <span>Save as Draft</span>
                    </div>
                    <div title="Output" class="btn-trail">
                        <span class="erp-jo print s48"></span><br />
                        <span>Output</span>
                    </div>
                    <div title="Pay" class="btn-trail">
                        <span class="erp-jo pay s48"></span><br />
                        <span>Pay</span>
                    </div>
                </div>

                <br /><br />

                <div id="accRevOrderHeader" custom-data-role="accordion">
                    <div class="accordion-header active">
                        Order Header
                    </div>
                    <div class="accordion-content">
                        <div id="divJOWorkTab" class="custom-tab">
                            <ul>
                                <li class="active" custom-tab-content="#tabJOWorkBasicInfo">Basic Info</li>
                            </ul>
                        </div>

                        <div id="tabJOWorkBasicInfo" custom-tab-role="content" style="display:block;">
                            <div class="row">
                                <div class="col-sm-8">
                                    <div class="row">
                                        <div class="col-sm-6">
                                            <div class="form-group">
                                                <label>Vendor: <span class="required">*</span></label>
                                                <input data-role="none" type="text" class="form-control" />
                                            </div>
                                        </div>
                                        <div class="col-sm-6">
                                            <div class="form-group">
                                                <label>Sales Rep:</label>
                                                <input data-role="none" type="text" class="form-control" />
                                            </div>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-sm-6">
                                            <div class="form-group">
                                                <label>PO Number:</label>
                                                <input data-role="none" type="text" class="form-control" />
                                            </div>
                                        </div>
                                        <div class="col-sm-6">
                                            <div class="form-group">
                                                <label>Payment Terms:</label>
                                                <input data-role="none" type="text" class="form-control" />
                                            </div>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-sm-6">
                                            <div class="row">
                                                <div class="col-sm-6">
                                                    <div class="form-group">
                                                        <label>Quote Number:</label>
                                                        <input data-role="none" type="text" class="form-control" />
                                                    </div>
                                                </div>
                                                <div class="col-sm-6">
                                                    <div class="form-group">
                                                        <label>Quote Date:</label>
                                                        <input data-role="none" type="text" id="btnJOQuoteDate" class="form-control" readonly />
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="col-sm-6">
                                            <div class="form-group">
                                                <label>Date Needed: <span class="required">*</span></label>
                                                <input data-role="none" type="text" id="btnJODateNeeded" class="form-control" readonly />
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-sm-4">
                                    <div class="form-group">
                                        <label>Billing Address: <span class="required">*</span></label>
                                        <div style="margin-top:-5px; display:inline-block; float:right;">
                                            <button data-role="none" class="btn custom-btn-theme-b btn-less-width btn-xs" style="float:right;"><img class="img-plus-white" /></button>
                                            <button data-role="none" class="btn custom-btn-theme-b btn-less-width btn-xs" style="float:right;"><img class="img-search-white" /></button>
                                        </div>
                                        <textarea data-role="none" class="form-control" style="max-height:55px; resize:none"></textarea>
                                    </div>
                                    <div class="form-group">
                                        <label>Delivery Address:<span class="required">*</span></label>
                                        <div style="margin-top:-5px; display:inline-block; float:right;">
                                            <button data-role="none" class="btn custom-btn-theme-b btn-less-width btn-xs" style="float:right;"><img class="img-plus-white" /></button>
                                            <button data-role="none" class="btn custom-btn-theme-b btn-less-width btn-xs" style="float:right;"><img class="img-search-white" /></button>
                                        </div>
                                        <textarea data-role="none" class="form-control" style="max-height:55px; resize:none"></textarea>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <br /><br />

                <div id="accRevOrderLine" custom-data-role="accordion">
                    <div class="accordion-header active">
                        Order Line Items
                    </div>
                    <div class="accordion-content">
                        <div style="overflow:auto;">
                            <table data-role="table" data-role="reflow" style="margin-right:5px; display:table;" class="ui-table ui-responsive table-stroke table-stripe custom-tbl-responsive">
                                <thead>
                                    <tr class="custom-theme-f fixed-cell-width">
                                        <th class="txt-center"> SKU # </th>
                                        <th class="txt-center"> Product Description </th>
                                        <th class="txt-center"> Order Qty </th>
                                        <th class="txt-center"> Packing Size </th>
                                        <th class="txt-center"> Unit Price </th>
                                        <th class="txt-center"> Line Total </th>
                                        <th class="txt-center"> Payment Terms </th>
                                    </tr>
                                </thead>
                                <tbody></tbody>
                            </table>
                        </div>
                    </div>
                </div>

                <br /><br />

                <div id="accRevOrderLine" custom-data-role="accordion">
                    <div class="accordion-header active">
                        Order Footer
                    </div>
                    <div class="accordion-content">
                        <div class="row">
                            <div class="col-sm-4">
                                <div class="form-group">
                                    <label>Remarks:</label>
                                    <textarea data-role="none" class="form-control" style="max-height:55px; resize:none"></textarea>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

            </div>
            <div id="tabJOSettings" custom-tab-role="content" style="display:none;">
                Settings
            </div>
        </div>
    </div>
 <div id="divdivErpDisbursementAccountsPayable" subpage="divErpRevenue" style="display:none;">

    </div>
</div>

<div id="divErpDeposits" subpage="divErpMain" class="container" style="display:none;"></div>
<div id="divErpAssetsManagement" subpage="divErpMain" class="container" style="display:none;"></div>

<div id="divPopupUrlPage"></div>