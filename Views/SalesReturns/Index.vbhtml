@Code
    ViewData("Title") = "Sales Returns"
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head
<link rel="stylesheet" href="/Content/flatui-css-custom.css" />

<script>
    require([GetPathFormat('/Scripts3/SalesReturns.js?crc=B1A2DB83')]);
</script>

<style type="text/css">
    .ui-btn-up-green {
        border: 1px solid #3A7214;
        background: #5BAB25;
        color: #fff;
        text-shadow: 0 -1px 1px #000;
        background-image: -moz-linear-gradient(top, #7EC54E, #41AB25);
        background-image: -webkit-gradient(linear,left top,left bottom,
            color-stop(0, #7EC54E),
            color-stop(1, #41AB25));
        -ms-filter: "progid:DXImageTransform.Microsoft.gradient(startColorStr='#7EC54E', EndColorStr='#41AB25')";
    }
    .ui-btn-up-green a.ui-link-inherit {
        color: #fff;
    }
    .ui-btn-hover-green {
        border: 1px solid #176E00;
        background: #6EB64B;
        color: #fff;
        text-shadow: 0 -1px 1px #000;
        background-image: -moz-linear-gradient(top, 
            #99D472, 
            #6EB64B);
        background-image: -webkit-gradient(linear,left top,left bottom,
            color-stop(0, #99D472),
            color-stop(1, #6EB64B));
        -ms-filter: "progid:DXImageTransform.Microsoft.gradient(startColorStr='#99D472', EndColorStr='#6EB64B')";
    }
    .ui-btn-hover-green a.ui-link-inherit {
        color: #fff;
    }
    .ui-btn-down-green {
        border: 1px solid #347722;
        background: #7EC54E;
        color: #fff;
        text-shadow: 0 -1px 1px #000;
        background-image: -moz-linear-gradient(top, #569E39, #7EC54E);
        background-image: -webkit-gradient(linear,left top,left bottom,
            color-stop(0, #569E39),
            color-stop(1, #7EC54E));
        -ms-filter: "progid:DXImageTransform.Microsoft.gradient(startColorStr='#569E39', EndColorStr='#7EC54E')";
    }
    .ui-btn-down-green a.ui-link-inherit {
        color: #fff;
    }
    .ui-btn-up-green,
    .ui-btn-hover-green,
    .ui-btn-down-green {
        font-family: Helvetica, Arial, sans-serif;
        text-decoration: none;
    }
</style>
End Section    

<div data-role="page">
    <div data-role="header" id="divHeaderReceiving" data-position="fixed1" style="background: none white; border: none">        
        @*<h1 id="MainPageTitle">@ViewData("Title")</h1>
        <a href="#" data-icon="arrow-l" id="btnBack">Back</a>*@
        <div class="ui-grid-d" >
            <div class="ui-block-a">
                <div class="btnHome" style="margin:0px; height: 80px;" data-role="button" data-theme="b" data-corners="false" class="HotButtons">
                    <div><img alt="home" src="/Content/images/home.png" width="30px" align="center" /><br /></div>
                    <div style="font-size:13px; margin-top:10px;">Home</div>
                </div>
            </div>
            <div class="ui-block-b">
                <div id="btnFOWST" style="margin:0px; height: 80px;" data-role="button" data-theme="b" data-corners="false" class="HotButtons">
                    <span id="lnkPosBranch" style="margin-top:5px; font-size:13px;">FOWST</span>
                </div>
            </div>
            <div class="ui-block-c">
                <div style="margin:0px; height: 80px;" data-role="button" data-theme="b" data-corners="false" class="HotButtons">
                    @*<span style="margin-top:5px;">Home</span>*@
                </div>
            </div>            
            <div class="ui-block-d">
                <div style="margin:0px; height: 80px;" data-role="button" data-theme="b" data-corners="false" class="HotButtons">
                    @*<span style="margin-top:5px;">Record Search</span>*@
                </div>
            </div>
            <div class="ui-block-e">
                <div id="butCheckOutHeader"style="margin:0px; height: 80px;" data-role="button" data-theme="green" data-corners="false">                    
                    <div><img alt="home" src="/Content/images/Update_Cart.png" width="30px" align="center" /><br /></div>
                    <div style="font-size:13px; margin-top:10px;">Check Out Returns Cart</div>
                </div>
            </div>
        </div>

        <div>
            <div style="margin: 0px;" data-role="button" data-theme="b" data-corners="false">
                <div style="float:right">
                    <span id="clockSalesReturn"></span>
                </div>
                <div style="clear:both"></div>            
            </div>
        </div>
        
    </div>

    <div data-role="content">
        <h1 id="divTitle">
            Sales Returns
        </h1>

        @*@If Common.WebRequest.QueryString("IsMobile") <> "1" Then
                @<applet name="jzebra" code="jzebra.PrintApplet.class" archive="/Scripts/jzebra.jar" width="10" height="10">
                        </applet>
            End If*@
        @*<div id="divPosBranch" style="margin-bottom: 13px;">
            <a id="lnkPosBranch" style="text-decoration: none;" href="#">Please choose a branch &#x25BC;</a>
        </div>*@      
        
        <form id="formSearch">
            <div id="divDiv" data-role="fieldcontain">
                <label for="btnDateFrom">Date From of Original Sales Transaction:</label>
                <div class="custom-ui-button">
                    <button id="btnDateFrom"></button>
                </div>                
                @*<fieldset id="txtDateFrom" data-role="DateCombo">
                    <legend>Date From of Original Sales Transaction:</legend>
                </fieldset>*@
            </div>
            <div id="divDiv" data-role="fieldcontain">
                <label for="btnDateTo">Date To of Original Sales Transaction:</label>
                <div class="custom-ui-button">
                    <button id="btnDateTo"></button>
                </div>                
                @*<fieldset id="txtDateTo" data-role="DateCombo">
                    <legend>Date To of Original Sales Transaction:</legend>
                </fieldset>*@
            </div>
            <div data-role="fieldcontain">
                <label for="txtSearch">
                    Invoice no.:
                </label>
                <input id="txtSearch" type="search" placeholder="Please enter receipt number" autofocus />
                <button id="butBarCode">Scan Barcode / QR Code</button>
            </div>
        </form>
        @*<div data-role="collapsible-set">
                <div data-role="collapsible" data-collapsed="false">
                    <h3>
                        Enter Invoice</h3>
                    <div data-role="content">
                    </div>
                </div>
                <div data-role="collapsible">
                    <h3>
                        Customer Name</h3>
                    <div data-role="content">
                        <form id="formSearchCustomer">
                        <div data-role="fieldcontain">
                            <label for="txtSearchCustomer">
                                Customer:</label>
                            <input id="txtSearchCustomer" type="search" />
                        </div>
                        </form>
                        <form id="formSearchItem">
                        <div data-role="fieldcontain">
                            <label for="txtSearchItem">
                                Invoice no. or product:</label>
                            <input id="txtSearchItem" type="search" />
                        </div>
                        </form>
                    </div>
                </div>
            </div>
        *@
        <style>
            .MiniNavBar.ui-navbar li .ui-btn .ui-btn-inner {
                padding: 0;
            }
        </style>
        <div data-role="navbar" class="MiniNavBar1">
            <ul>
                <li><a href="#" data-href="Nav1a">Receipt Contents</a></li>
                <li><a href="#" data-href="Nav1b">Return Cart</a></li>
            </ul>
        </div>
        <div id="Nav1a" class="content_div">
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
            </div>
        </div>
        <div id="Nav1b" class="content_div">
            Return Cart
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
                @*<button id="butCheckOut">Check Out Returns Cart</button>*@
                <button id="butPrint">Print</button>
                <button id="butApprove">Approve</button>
                <button id="butDeny">Disapprove</button>
            </div>
            <div id="divCartEmpty" class="Empty">
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

    <div id="divCheckOut" data-role="page">
        <div data-role="header">
            <a href="#" data-icon="arrow-l" data-rel="back">Back</a>
            <h1>
                Returns Check Out
            </h1>
            @* @If Common.WebRequest.QueryString("IsMobile") <> "1" Then
                    @<applet name="jzebra" code="jzebra.PrintApplet.class" archive="/Scripts/jzebra.jar" width="10" height="10">
                            </applet>
                End If*@
        </div>
        <div data-role="content">
            <div id="divCheckOutMessage">
            </div>
            <div id="divCheckOutResultMessage">
            </div>
            <a id="butCheckOutNow" name="butCheckOutNow" data-role="button">
                <h3>Make A Merchandise Exchange</h3>
                <p style="font-weight: normal; margin-top: -15px;">
                    Issue a Sales Return Voucher (SRV) to this customer
                </p>
            </a>
            <a id="butCheckOutRefund" name="butCheckOutNow" data-role="button">
                <h3>Issue A Refund</h3>
                <p style="font-weight: normal; margin-top: -15px;">
                    This function is currently applicable to pure cash purchases only. You must have sufficient authorization to issue a refund.
                </p>
            </a>
            <a id="divCheckOutBack" data-role="button">
                <h3>Back</h3>
                <p style="font-weight: normal; margin-top: -15px;">
                    Go back to the previous page
                </p>
            </a>
        </div>
    </div>
    <div id="divSalesReturnRequiresApproval" data-role="page">
        <div data-role="header">
            <h1>Requires Approval</h1>
        </div>
        <div data-role="content">
            Request for approval has been saved and sent. Please ask your manager to approve SR <span id="spanMemoNum"></span>.
            <br /><br />
            <button id="butApproveNow">Approve now by entering an authorized username and password</button>
            <br />
            <button id="butGetApprovalStatus">Check Approval Status<div id="divGetApprovalResult"></div></button>
            <br />
            <button id="butGotoSalesReturn">Process another sales return<div>You can check on the status of your requests by going to "Requests and Approvals</div></button>
            <br />
            <button id="butGotoPos">Process another sales transaction<div>You can check on the status of your requests by going to "Requests and Approvals</div></button>
            <br />
            <button id="butGotoMainMenu">Process another sales transaction<div>You can check on the status of your requests by going to "Requests and Approvals</div></button>
            <br />
        </div>
    </div>
