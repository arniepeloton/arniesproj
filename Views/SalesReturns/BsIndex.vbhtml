@Code
    ViewData("Title") = "Sales Returns"
    Layout = "~/Views/Shared/BootstrapFiveLayout.vbhtml"
End Code

@Section head

<script>
    require(['bootstrapjs'], function () {
        require(['/Content/bootstrap/js/bootstrap_modal.js?v=' + assetversion], function () {
            require([GetPathFormat('/Scripts3/Bs_SalesReturns.js?crc=B1A2DB83&v=' + assetversion)]);
        })
    })    
</script>
End Section    

<div id="pageSalesReturn" custom-data-role="page" style="display:block">
    <nav class="navbar navbar-dark sticky-md-top bg-primary mb-3" style="padding:0px !important;">
        <div class="row" style="margin:0px !important;">
            <div class="col-3 col-md-2" style="padding:0px !important;">
                <button data-role="none" style="height:100%;" class="btn btn-primary btn-block btn-lg rounded-0 btnHome">
                    <span id="iconHome" class="bi-house-fill"></span>
                    <div id="lblHome" class="menu-description mt-1">Home</div>
                </button>
            </div>
            <div class="col-3 col-md-2" style="padding:0px !important;">
                <button data-role="none" id="btnFOWST" style="height:100%;" class="btn btn-primary btn-block btn-lg rounded-0">
                    <span class="bi-building"></span>
                    <div id="lnkPosBranch" class="menu-description mt-1">FOWST</div>
                </button>
            </div>
            <div class="col-3 col-md-2" style="padding:0px !important;">
                <button data-role="none" id="btnSRHIstory" style="height:100%;" class="btn btn-primary btn-block btn-lg rounded-0">
                    <span class="bi-clock-history"></span>
                    <div id="lnkPosBranch" class="menu-description mt-1">History</div>
                </button>
            </div>
            <div class="col-3 offset-md-3 col-md-3" style="padding:0px !important;">
                <button data-role="none" id="butCheckOutHeader" style="height:100%;" class="btn btn-success btn-block btn-lg rounded-0">
                    <span class="bi-cart4"></span>
                    <div class="menu-description mt-1">Check Out Returns Cart</div>
                </button>
            </div>
        </div>
        <br />
        <div class="row" style="margin:0px; padding:0.5em; width:100%; color:#FFF; background-color:#1969de; font-weight:bold;">
            <div class="col" style="text-overflow:ellipsis; overflow:hidden; text-align:right; white-space:nowrap;">
                <span id="clockSalesReturn">&nbsp;</span>
            </div>
        </div>
    </nav>

    <div class="container-fluid mb-5">
        <h1 id="divTitle" class="mb-3">
            Sales Returns
        </h1>

        @*@If Common.WebRequest.QueryString("IsMobile") <> "1" Then
                @<applet name="jzebra" code="jzebra.PrintApplet.class" archive="/Scripts/jzebra.jar" width="10" height="10">
                        </applet>
            End If*@
        @*<div id="divPosBranch" style="margin-bottom: 13px;">
            <a id="lnkPosBranch" style="text-decoration: none;" href="#">Please choose a branch &#x25BC;</a>
        </div>*@      
        
    <form id="formSearch2">
        <div id="divInvoiceDateRange" style="display: none;">
            <div class="row mb-3">
                <label class="col-md-3 mb-2 mb-md-0" for="btnDateFrom">Date From of Original Sales Transaction:</label>
                <div class="col-md-9">
                    <button data-role="none" class="btn btn-secondary btn-block" id="btnDateFrom">&nbsp;</button>
                </div>
            </div>
            <div class="row mb-3">
                <label class="col-md-3 mb-2 mb-md-0" for="btnDateTo">Date To of Original Sales Transaction:</label>
                <div class="col-md-9">
                    <button data-role="none" class="btn btn-secondary btn-block" id="btnDateTo">&nbsp;</button>
                </div>
            </div>
        </div>
        <div class="row mb-3">
            <label class="col-md-3 mb-2 mb-md-0" for="btnDateTo">Invoice No.:</label>
            <div class="col-md-9">
                <div class="input-group">
                    <input data-role="none" class="form-control" id="txtSearch" type="search" placeholder="Please enter receipt number" autofocus />
                    <button data-role="none" class="btn btn-secondary" id="butBarCode">Scan</button>
                </div>
            </div>
        </div>
        <div class="row mb-3" style="display: none;" id="divInvoiceAmount">
            <label class="col-md-3 mb-2 mb-md-0" for="txtInvoiceAmount">Invoice Amount:</label>
            <div class="col-md-9">
                <div class="input-group">
                    <input data-role="none" class="form-control" id="txtInvoiceAmount" type="number" placeholder="Please enter amount indicated on the bill" />
                    <button data-role="none" class="btn btn-secondary" id="butSearch">Scan</button>
                </div>
            </div>
        </div>
    </form>
        <ul class="nav nav-tabs mt-3">
            <li class="nav-item">
                <a data-role="none" class="nav-link active" data-toggle="tab" data-target="#Nav1a">Receipt Contents</a>
            </li>
            <li class="nav-item">
                <a data-role="none" class="nav-link" data-toggle="tab" data-target="#Nav1b">Return Cart</a>
            </li>
        </ul>
        <div class="tab-content">
            <div id="Nav1a" class="tab-pane fade pt-3 show active">
                <div id="divInvoiceContents">
                    <div id="divReturnables" style="display: none">
                        <h3 name="spanStockDescription" style="white-space: normal"></h3>
                        <table class="tableUnits">
                            <tr name="trTemplate" style="cursor: pointer">
                                <td name="tdQty"></td>
                                <td name="tdUnit"></td>
                                <td name="tdPrice"></td>
                                <td name="tdLineTotal"></td>
                                <td name="tdQuantityReturned"></td>
                            </tr>
                        </table>
                        <div>
                            Item total: <span name="spanTotal"></span>
                        </div>
                    </div>
                    <div class="text-center">Please search invoice first</div>
                </div>
            </div>
            <div id="Nav1b" class="tab-pane fade pt-3">
                <div id="divCartSection" style="display: none">
                    @*<a id="lnkSalesOrder" style="text-decoration: none;" href="#" data-role="button">
                <div name="divNumber">
                </div>
                <div name="divDate">
                </div>
                </a>
                 <button id="">New Cart</button>*@
                    <div id="divCartStock" style="display: none">
                        <h3 name="spanStockDescription" style="white-space: normal"></h3>
                        <table class="tableUnits">
                            <tr name="trTemplate">
                                <td name="tdQty"></td>
                                <td name="tdUnit"></td>
                                <td name="tdPrice"></td>
                                <td name="tdLineTotal"></td>
                            </tr>
                        </table>
                        <div>
                            Item total: <span name="spanTotal"></span>
                        </div>
                    </div>
                    <div style="font-weight: bold; padding-top: 10px;">
                        Total: <span id="spanCartTotal"></span>
                    </div>
                    <div id="divCartList"></div>
                    @*<button id="butCheckOut">Check Out Returns Cart</button>*@
                    <button id="butPrint">Print</button>
                    <button id="butApprove">Approve</button>
                    <button id="butDeny">Disapprove</button>
                </div>
                <div id="divCartEmpty" class="Empty text-center">
                    No items in cart
                </div>
                <div id="divPeopleMenuPopUp" data-role="popup">
                    <ul data-role="listview" data-inset="true" style="min-width: 210px;">
                        <li data-role="divider">Menu</li>
                        @*<li><a href="#dvOrganization" data-rel="popup" data-position-to="window">Download Database</a></li>*@
                        <li><a id="popPeopleMenuPeopleInfoSummary">Profile</a></li>
                        <li><a id="btnSignOut">Log Out</a></li>
                    </ul>
                </div>

            </div>
        </div>        
    </div>    
</div>

<div id="divCheckOut" custom-data-role="page" style="display:none;">
    <nav class="bg-primary text-white mb-3 sticky-top">
        <div class="container-fluid">
            <table style="width:100%;">
                <tr>
                    <td style="width:50px;">
                        <button data-role="none" id="divCheckOutBack" class="btn btn-primary btn-lg btn-block text-white">
                            <span class="bi-chevron-left"></span>
                        </button>
                    </td>
                    <td><div><h5 class="m-0">Returns Check Out</h5></div></td>
                </tr>
            </table>
        </div>
    </nav>

    <div class="container-fluid">
        <div id="divCheckOutMessage">
        </div>
        <div id="divCheckOutResultMessage">
        </div>
        <div id="divCheckOutPrintMessage">
        </div>
        <br />
        <button id="butCheckOutNow" name="butCheckOutNow" data-role="none" class="btn btn-secondary btn-block">
            <h3>Make A Merchandise Exchange</h3><br />
            <p style="font-weight: normal; margin-top: -15px;">
                Issue a Sales Return Voucher (SRV) to this customer
            </p>
        </button>
        <br />
        <button id="butCheckOutRefund" name="butCheckOutNow" data-role="none" class="btn btn-secondary btn-block">
            <h3>Issue A Refund</h3><br />
            @*<p style="font-weight: normal; margin-top: -15px;">
                This function is currently applicable to pure cash purchases only. You must have sufficient authorization to issue a refund.
            </p>*@
        </button>
    </div>
</div>

<div id="divSalesReturnRequiresApproval" custom-data-role="page" style="display:none;">
    <nav class="bg-primary text-white mb-3 sticky-top">
        <div class="container-fluid pt-3 pb-3">
            <h5 class="m-0">Requires Approval</h5>
        </div>
    </nav>

    <div class="container-fluid">
        Request for approval has been saved and sent. Please ask your manager to approve SR <span id="spanMemoNum"></span>.
        <br /><br />
        <button id="butApproveNow" data-role="none" class="btn btn-secondary btn-block">Approve now by entering an authorized username and password</button>
        <br />
        <button id="butGetApprovalStatus" data-role="none" class="btn btn-secondary btn-block">Check Approval Status<div id="divGetApprovalResult"></div></button>
        <br />
        <button id="butGotoSalesReturn" data-role="none" class="btn btn-secondary btn-block">Process another sales return<div>You can check on the status of your requests by going to "Requests and Approvals</div></button>
        <br />
        <button id="butGotoPos" data-role="none" class="btn btn-secondary btn-block">Process another sales transaction<div>You can check on the status of your requests by going to "Requests and Approvals</div></button>
        <br />
        <button id="butGotoMainMenu" data-role="none" class="btn btn-secondary btn-block">Back to Main Menu</button>
        <br />
    </div>
</div>