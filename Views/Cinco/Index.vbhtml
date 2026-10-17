@Code
    ViewData("Title") = "Dashboards and Reports"
    Layout = "~/Views/Shared/BootstrapFiveLayout.vbhtml"
End Code

@Section head
    @*<link rel="stylesheet" href="/Content/css/jquery.jqplot.min.css" />
    <link rel="stylesheet" href="/Content/css/ui.dynatree.css" />
    <link rel="stylesheet" href="/Content/css/qunit.css" />
    <link rel="stylesheet" href="/Content/flatui-css-custom.css" />
    <link rel="stylesheet" href="/Content/bootstrap-custom.css" />
    <link rel="stylesheet" href="/Content/metro-calendar.css" />*@
    <link rel="stylesheet" href="/Content/jquery.buttons.dataTables.min.css" />
    <link rel="stylesheet" href="/Content/jquery.dataTables2.css" />
     
    <script>
        require(['bootstrapjs'], function () {
            require(['/Content/bootstrap/js/bootstrap_modal.js?v='], function () {
                require([GetPathFormat('/Scripts/sources/Cinco/Cinco.js?crc=' + assetversion)]);
            })
        })        
    </script>
    
    <style>    
     .jqplot-xaxis-hide {
            display: none
        }
    .btn-toggle-nav a {
      display: inline-flex;
      padding: .1875rem .5rem;
      margin-top: .125rem;
      margin-left: 1.25rem;
      text-decoration: none;
    }
    .btn-toggle-nav a:hover,
    .btn-toggle-nav a:focus {
      background-color: #1d2124;
    }
    </style>
End Section

@*Header*@

<table border="0" cellpadding="0" style="width:100%; height:100%; top:0; bottom:0; position:fixed;">
    <tr>
        <td style="min-width:300px; width:300px; height:100%; vertical-align:top; overflow:auto;">
            <div class="d-flex flex-column p-3 text-dark bg-dark" style="width:100%; height:100%; overflow:auto;">
                <button href="/" data-role="none" class="btn btn-dark btn-block btnhome">
                    <span class="bi-chevron-left"></span>
                    Back to Home
                </button>
                <hr>
                <ul class="nav nav-pills flex-column mb-auto">
                    <li class="nav-item">
                        <a href="#" id="btnCMSalesReport" data-role="none" class="nav-link nav-menu text-white active">
                            <span class="bi-chevron-right"></span>
                            Sales Report
                        </a>
                    </li>
                    <li class="nav-item">
                        <a href="#" id="btnCMCashFlow" data-role="none" class="nav-link nav-menu text-white">
                            <span class="bi-chevron-right"></span>
                            Cash Flow
                        </a>
                    </li>
                    <li class="nav-item">
                        <a href="#" id="btnCMInventoryMovement" data-role="none" class="nav-link nav-menu text-white">
                            <span class="bi-chevron-right"></span>
                            Inventory Movement
                        </a>
                    </li>
                    <li class="nav-item">
                        <a href="#" id="btnCMDeliveryReport" data-role="none" class="nav-link nav-menu text-white">
                            <span class="bi-chevron-right"></span>
                            Delivery Report
                        </a>
                    </li>
                    <li class="nav-item">
                        <a href="#" id="btnCMHourlySalesReport" data-role="none" class="nav-link nav-menu text-white">
                            <span class="bi-chevron-right"></span>
                            Hourly Sales Reports
                        </a>
                    </li>
                    <li class="nav-item">
                        <a href="#" id="btnCMMonthlySalesReport" data-role="none" class="nav-link nav-menu text-white">
                            <span class="bi-chevron-right"></span>
                            Monthly Sales Reports
                        </a>
                    </li>
                    <li class="nav-item">
                        <a href="#" id="btnCMPOSSalesReport" data-role="none" class="nav-link nav-menu text-white">
                            <span class="bi-chevron-right"></span>
                            POS Sales Reports
                        </a>
                    </li>
                    <li class="nav-item">
                        <a href="#" id="btnCMZReadingReport" data-role="none" class="nav-link nav-menu text-white">
                            <span class="bi-chevron-right"></span>
                            Z-Reading Reports
                        </a>
                    </li>
                    <li class="nav-item">
                        <a href="#" id="btnCMSpecialDiscountReport" data-role="none" class="nav-link nav-menu text-white">
                            <span class="bi-chevron-right"></span>
                            Special Discount Report
                        </a>
                    </li>
                    <li class="nav-item">
                        <a href="#" id="btnCMVoidedSalesReport" data-role="none" class="nav-link nav-menu text-white">
                            <span class="bi-chevron-right"></span>
                            Voided Sales Report
                        </a>
                    </li>
                    <li class="nav-item">
                        <a href="#" id="btnCMShiftSummaryReport" data-role="none" class="nav-link nav-menu text-white">
                            <span class="bi-chevron-right"></span>
                            Shift Summary Report
                        </a>
                    </li>
                    <li class="nav-item">
                        <a href="#" id="btnCMPOSAcumaticaSalesReport" data-role="none" class="nav-link nav-menu text-white">
                            <span class="bi-chevron-right"></span>
                            POS Acumatica Sales Reports
                        </a>
                    </li>
                    <li class="nav-item">
                        <a href="#" id="btnCMGenerateEJournalReport" data-role="none" class="nav-link nav-menu text-white">
                            <span class="bi-chevron-right"></span>
                            Generate E-Journal Report
                        </a>
                    </li>
                </ul>
            </div>
        </td>
        <td style="vertical-align:top; position:relative;">
            <div style="height:100%; width:100%; overflow:auto; position:absolute; right:0; left:0;">
                <div class="bg-primary text-white" style="width:100%;">
                    <div class="p-3">
                        <button data-role="none" class="btn btn-primary btn-lg border-white" style="">
                            <span class="bi-justify"></span>
                        </button>
                        <span class="OrganizationNameHeader ml-2" style="text-overflow:ellipsis; font-size:20px; white-space:nowrap; overflow:hidden;"></span>
                    </div>
                    <div class="row" style="margin:0px; padding:0.5em; width:100%; color:#FFF; background-color:#1969de; font-weight:bold;">
                        <div class="BreadCrumbs">
                            <div class="ItemsBreadCrumbs">
                                @*Main Dashboard*@
                            </div>
                        </div>
                    </div>
                </div>

                <div class="container-fluid mt-3 mb-5">
                    <div id="divSalesReport" custom-data-role="page" style="display:block;">
                        <div class="table-responsive mb-3">
                            <table data-role="none" id="tblTransactionHeader" class="table table-light table-hover table-striped">
                                <thead>
                                    <tr>
                                        <th data-sort="date" style="cursor:pointer;"> Store Name </th>
                                        <th data-sort="int" style="cursor:pointer;"> Region </th>
                                        <th data-sort="string" style="cursor:pointer;"> Province </th>
                                        <th data-sort="accounting" style="cursor:pointer;"> Area </th>
                                        <th data-sort="int" style="cursor:pointer;"> Store Type </th>
                                        <th data-sort="string" style="cursor:pointer;"> Location Type </th>

                                        <th data-sort="accounting" style="cursor:pointer;"> Transaction Count </th>
                                        <th data-sort="accounting" style="cursor:pointer;"> Average Check </th>
                                        <th data-sort="accounting" style="cursor:pointer;"> PayMaya </th>
                                        <th data-sort="accounting" style="cursor:pointer;"> GCash </th>
                                        <th data-sort="accounting" style="cursor:pointer;"> Discount </th>
                                        <th data-sort="accounting" style="cursor:pointer;"> Walk In</th>
                                        <th data-sort="accounting" style="cursor:pointer;"> Delivery </th>
                                        <th data-sort="accounting" style="cursor:pointer;"> Net Sales </th>
                                    </tr>
                                </thead>
                                <tbody></tbody>
                            </table>
                        </div>
                    </div>

                    <div id="divCashFlow" custom-data-role="page" style="display:none;">
                        Cash Flow
                    </div>

                    <div id="divInventoryMovement" custom-data-role="page" style="display:none;">
                        Inventory Movement
                    </div>

                    <div id="divDeliveryReport" custom-data-role="page" style="display:none;">
                        Delivery Report
                    </div>

                    <div id="divHourlySalesReport" custom-data-role="page" style="display:none;">
                        Hourly Sales Report
                    </div>

                    <div id="divMonthlySalesReport" custom-data-role="page" style="display:none;">
                        Monthly Sales Report
                    </div>

                    <div id="divPOSSalesReport" custom-data-role="page" style="display:none;">
                        POS Sales Report
                    </div>

                    <div id="divZReadingReport" custom-data-role="page" style="display:none;">
                        Z-Reading Report
                    </div>

                    <div id="divSpecialDiscountReport" custom-data-role="page" style="display:none;">
                        Special Discount Report
                    </div>

                    <div id="divVoidedSalesReport" custom-data-role="page" style="display:none;">
                        Voided Sales Report
                    </div>

                    <div id="divShiftSummaryReport" custom-data-role="page" style="display:none;">
                        Shift Summary Report
                    </div>

                    <div id="divPOSAcumaticaSalesReport" custom-data-role="page" style="display:none;">
                        POS Acumatica Sales Report
                    </div>

                    <div id="divGenerateEJournalReport" custom-data-role="page" style="display:none;">
                        Generate E-Journal Report
                    </div>
                </div>
            </div>            
        </td>
    </tr>
</table>