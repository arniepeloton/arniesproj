@Code
    ViewData("Title") = "Credit And Collection"
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code
@Section head
    <link rel="stylesheet" href="/Content/flatui-css-custom.css" />
    <script>
        require([GetPathFormat('/Scripts/sources/CreditAndCollection/Payments.js?crc=')]);
    </script>
End Section

<style type="text/css">
    .dataTables_processing {
        /*Overwrite the Process Loading in datatables*/
        top: 0 !important;
        line-height: 50vh !important;
        height: 100% !important;
        margin-top: -20px !important;
    }

    .right{
        float:right;
        width: 240px;
    }

    .left{
        overflow:hidden; 
        float:none;
    }

    .fixed-header-width > th {
        white-space:nowrap;
    }

    /* copied from \Views\CreditAndCollection\Index.vbhtml */
    .divLeft {
        float:none;
        width:auto;
        overflow:auto;
        padding:3px;
    }

    /* copied from \Views\CreditAndCollection\Index.vbhtml */
    .divRight{
        width:250px;
        float:right;
        padding:3px;
    }
</style>

<div id="divCaCPaymentLoading" subpage="pageCacPayment" style="display:block;">Loading, please wait ...</div>

<div id="divCaCPaymentMainPage" subpage="pageCacPayment" style="display:none">    
    <div class="right">
        @*<div class="custom-ui-label">
            <div>Amount Tendered</div>
            <div>0.00</div>
        </div>
        <div class="custom-ui-label">
            <div>Unapplied Credits</div>
            <div>0.00</div>
        </div>
        <div class="custom-ui-label">
            <div>Surcharges</div>
            <div>0.00</div>
        </div>

        <div class="custom-ui-label">
            <div>Total Amount Due</div>
            <div>0.00</div>
        </div>*@
    </div>
    <div class="left">
        <div class="divTable">
            <div class="divTableRow">
                <div class="divTableCell txt-left txt-middle" style="width:150px; font-size:16px;">
                    Receipt Type:
                </div>
                <div class="divTableCell">
                    <select><option>Please Select</option></select>
                </div>
                <div class="divTableCell txt-right txt-middle" style="width:150px; font-size:16px;">
                    Payment Date
                </div>
                <div class="divTableCell">
                    <button>Set Date</button>
                </div>
            </div>
            <div class="divTableRow">
                <div class="divTableCell txt-left txt-middle" style="width:150px; font-size:16px;">
                    Receipt No.:
                </div>
                <div class="divTableCell">
                    <input type="text" />
                </div>
                <div class="divTableCell txt-left txt-middle" style="width:150px; font-size:16px;">

                </div>
                <div class="divTableCell">

                </div>
            </div>
        </div>

        <div class="divTable">
            <div class="divTableRow">
                <div class="divTableCell txt-left txt-middle" style="width:150px; font-size:16px;">
                    Customer:
                </div>
                <div class="divTableCell">
                    <input type="text" />
                </div>
                <div class="divTableCell txt-left txt-middle" style="width:150px; font-size:16px;">

                </div>
                <div class="divTableCell">

                </div>
            </div>
        </div>

        <div class="divTable">
            <div class="divTableRow">
                <div class="divTableCell txt-left txt-middle" style="width:150px; font-size:16px;">
                    Remarks:
                </div>
                <div class="divTableCell">
                    <input type="text" />
                </div>
                <div class="divTableCell txt-left txt-middle" style="width:150px; font-size:16px;">

                </div>
                <div class="divTableCell">

                </div>
            </div>
        </div>

        @*<div class="divTable">
                <div class="divTableRow">
                    <div class="divTableCell txt-left txt-middle" style="width:150px; font-size:16px;">
                    </div>
                    <div class="divTableCell">
                        <button>Supplier Invoices</button>
                    </div>
                    <div class="divTableCell">
                        <button>Shipper Information</button>
                    </div>
                    <div class="divTableCell">
                        <button>Compute Freight</button>
                    </div>
                </div>
            </div>*@
        <br />
        <div style="overflow:auto;">
            <table data-role="table" data-role="reflow" style="margin-right:5px; display:table;" class="ui-table ui-responsive table-stroke table-stripe custom-tbl-responsive">
                <thead>
                    <tr class="custom-theme-f fixed-header-width">
                        <th data-sort="" style="cursor:pointer;"> Invoice Date </th>
                        <th data-sort="" style="cursor:pointer;"> Invoice Number </th>
                        <th data-sort="" style="cursor:pointer;"> Due Date </th>
                        <th data-sort="" style="cursor:pointer;"> Invoice Amount </th>
                        <th data-sort="" style="cursor:pointer;"> DM/CM </th>
                        <th data-sort="" style="cursor:pointer;"> PO Unit </th>
                        <th data-sort="" style="cursor:pointer;"> Factory Cost </th>
                        <th data-sort="" style="cursor:pointer;"> Freight Cost </th>
                        <th data-sort="" style="cursor:pointer;"> Modified Freight Cost</th>
                        <th data-sort="" style="cursor:pointer;"> Landed Cost </th>
                        <th data-sort="" style="cursor:pointer;"> Amount </th>
                        <th data-sort="" style="cursor:pointer;"> Remarks </th>
                    </tr>
                </thead>
                <tbody id="tblInteractionHistory"></tbody>
            </table>
        </div>
    </div>
</div>