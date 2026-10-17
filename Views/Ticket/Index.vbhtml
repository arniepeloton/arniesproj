@Code
    ViewData("Title") = "Tickets"
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code
@Section head
    <script>
        require([GetPathFormat('/Scripts/sources/Tickets/Ticket.js?crc=')]);
    </script>

<link rel="stylesheet" href="/Content/flatui-css-custom.css">
<link rel="stylesheet" href="/Content/bootstrap-custom.css">
<link rel="stylesheet" href="/Content/Ecommerce/css/bootstrap.min.css">

<link rel="stylesheet" href="/Content/Ecommerce/css/main.css">
<link rel="stylesheet" href="/Content/Ecommerce/css/blue.css">

    <style type="text/css">
        body {
            background-color: white !important;
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
End Section

<div id="pageTicketMain" custom-data-role="page" style="display:block;">
    <div class="custom-theme-a header">
        <div class="container">
            <div>
                <button id="btnTicketsBackToMenu" data-role="none" class="btn custom-btn-theme-a btn-less-width">
                    <span class="glyphicon glyphicon-chevron-left"></span>
                </button>
                Payment Collections
            </div>
        </div>
    </div>
    <br />
    <div class="container">
        List of Areas:
        <br /><br />
        <div id="divCategoryList"></div>
    </div>
</div>

<div id="pageTenant" custom-data-role="page" style="display:none;">
    <div class="custom-theme-a header">
        <div class="container">
            <div>
                <button data-role="none" class="btn custom-btn-theme-a btn-less-width btnTicketsBackToMain">
                    <span class="glyphicon glyphicon-chevron-left"></span>
                </button>
                Tenants
            </div>
        </div>
    </div>
    <br />
    <div class="container">
        List of Tenants Under Area <b id="lblAreaName"></b>
        <br /><br />
        <div id="divListOfTenants"></div>
    </div>
</div>

<div id="pageTenantPayment" custom-data-role="page" style="display:none;">
    <div class="custom-theme-a header">
        <div class="container">
            <div>
                <button id="btnPaymentBack" data-role="none" class="btn custom-btn-theme-a btn-less-width">
                    <span class="glyphicon glyphicon-chevron-left"></span>
                </button>
                Payment
            </div>
        </div>
    </div>
    <br />
    <div class="container">
        Tenant's Name: <b id="lblTenantName"></b>
        <br /><br />
        <table style="width:100%;">
            <tr>
                <td style="text-align:center"></td>
                <td style="text-align:center; width:150px;">Amount</td>
            </tr>
            <tr>
                <td style="text-align:center">Laray</td>
                <td style="text-align:center; width:150px; padding:10px;">
                    <input data-role="none" type="text" class="form-control" />
                </td>
            </tr>
            <tr>
                <td style="text-align:center">Light</td>
                <td style="text-align:center; width:150px; padding:10px;">
                    <input data-role="none" type="text" class="form-control" />
                </td>
            </tr>
            <tr>
                <td style="text-align:center">Transient Laray</td>
                <td style="text-align:center; width:150px; padding:10px;">
                    <input data-role="none" type="text" class="form-control" />
                </td>
            </tr>
            <tr>
                <td style="text-align:center">Transient Light</td>
                <td style="text-align:center; width:150px; padding:10px;">
                    <input data-role="none" type="text" class="form-control" />
                </td>
            </tr>
            <tr>
                <td style="text-align:center">Others</td>
                <td style="text-align:center; width:150px; padding:10px;">
                    <input data-role="none" type="text" class="form-control" />
                </td>
            </tr>
            <tr>
                <td style="text-align:center; font-weight:bold;">TOTAL AMOUNT RECEIVED</td>
                <td style="text-align:center; width:150px; padding:10px;">
                    0
                </td>
            </tr>
        </table>
        <br /><br />
        <button data-role="none" id="btnTicketsPayNow" class="btn custom-btn-theme-b">
            Pay Now
        </button>
    </div>
</div>

<div id="pagePaymentSuccess" custom-data-role="page" style="display:none;">
    <div class="container txt-center">
        <span class="glyphicon glyphicon-ok-circle" style="color:#16a085; font-size:130px; margin-top:50px;"></span>
        
        <div style="margin:50px 0px; font-size:25px; color:#16a085;">Payment has been saved!</div>
        
        <button data-role="none" class="btn custom-btn-theme-b btnTicketsBackToMain">
            Back to home
        </button>
    </div>
</div>