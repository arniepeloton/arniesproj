@Code
    ViewData("Title") = "Purchasing"
    Layout = "~/Views/Shared/BootstrapFiveLayout.vbhtml"
End Code
@Section head
    <link rel="stylesheet" href="/Content/jquery.buttons.dataTables.min.css" />
    <link rel="stylesheet" href="/Content/jquery.dataTables2.css" />
    <script>
        require(['bootstrapjs'], function () {
            require(['/Content/bootstrap/js/bootstrap_modal.js?v=' + assetversion], function () {
                require(['common'], function () {
                    AjaxSend({
                        ExactUrl: '/Scripts/sources/ERP/Disbursement/dashboard/view.html?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile + '&proj=eas&pid=' + sessionStorage.PeopleID + '&v=' + assetversion,
                        type: 'GET',
                        dataType: 'text',
                        success: function (vData) {
                            var openPO = $('<div>').append(vData);

                            if ($('body').find('#popDisbursementOpenPurchaseOrders').length > 0) {
                                openPO.find('#popDisbursementOpenPurchaseOrders').remove();
                            }

                            if ($('body').find('#popupVendorAdminApproval').length > 0) {
                                openPO.find('#popupVendorAdminApproval').remove();
                            }

                            $('#divMain').append(openPO.find('> div').hide());

                            require(['promise!/Scripts/sources/ERP/PurchaseOrder/settings/controller.js?v=' + assetversion]);
                        }
                    });
                })                
                //require(['promise!/Scripts/sources/ERP/OrderProcessing/dashboard/controller.js?v=' + assetversion])
            })
        })
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

    div[custom-data-role="popup"] {
        display:none !important;
    }
</style>

<nav class="navbar navbar-dark bg-primary sticky-top text-light pl-3 pr-3 mb-3">
    <div style="white-space:nowrap; overflow:hidden; text-overflow:ellipsis;">
        <a href="javascript:history.back()" data-role="none" class="btn btn-primary">
            <span class="bi-chevron-left"></span>
        </a>
        <span>Purchasing</span>
    </div>
</nav>

<div id="divMain" class="container-fluid mb-5">
    <div id="divPurchasingMain" subpage="divPurchasing" style="display:block;">
        <div id="divPurchasingTab" style="white-space:nowrap; overflow:auto;">
            <ul class="nav nav-tabs" id="divGatePassTab" role="tablist" style="flex-wrap:nowrap;">
                <li class="nav-item" role="presentation">
                    <button data-role="none" class="nav-link active" data-toggle="tab" data-target="#tabPurchasingDashboard">Dashboard</button>
                </li>
                <li class="nav-item" role="presentation">
                    <button data-role="none" class="nav-link" data-toggle="tab" data-target="#tabPurchasingWork">Purchasing</button>
                </li>
                <li class="nav-item" role="presentation">
                    <button data-role="none" class="nav-link" data-toggle="tab" data-target="#tabPurchasingPoBySku">POs by SKU</button>
                </li>
                <li class="nav-item" role="presentation">
                    <button data-role="none" class="nav-link" data-toggle="tab" data-target="#tabPurchasingPoByBackorder">POs by Backorder</button>
                </li>
                <li class="nav-item" role="presentation">
                    <button data-role="none" class="nav-link" data-toggle="tab" data-target="#tabPurchasingSettings">Settings</button>
                </li>
            </ul>
        </div>

        <div class="tab-content p-3">
            <div id="tabPurchasingDashboard" class="tab-pane fade show active">
                <div class="row row-cols-2 row-cols-sm-2 row-cols-md-3 row-cols-lg-4">
                    <div class="col">
                        <div class="dashboardGrid bg-light border p-3 rounded" name="butDisbursementOpenPurchaseOrders">
                            <div class="gHeader">
                                Open Purchase Orders
                            </div>
                            <div class="gBody" name="lblDisbursementOpenPurchaseOrders" style="font-size:18px;">
                            </div>
                            <div class="gFooter">

                            </div>
                        </div>
                    </div>
                    <div class="col">
                        <div class="dashboardGrid bg-light border p-3 rounded" name="butDisbursementUnapprovedPurchaseOrders">
                            <div class="gHeader">
                                Unapproved Purchase Orders
                            </div>
                            <div class="gBody" name="lblDisbursementUnapprovedPurchaseOrders" style="font-size:18px;">
                            </div>
                            <div class="gFooter">

                            </div>
                        </div>
                    </div>
                    <div class="col">
                        <div class="dashboardGrid bg-light border p-3 rounded" name="butDisbursementPQFollowUp">
                            <div class="gHeader">
                                Price Quotes For Follow Up
                            </div>
                            <div class="gBody" name="lblDisbursementPQFollowUp" style="font-size:18px;">
                            </div>
                            <div class="gFooter">

                            </div>
                        </div>
                    </div>
                    <div class="col">
                        <div class="dashboardGrid bg-light border p-3 rounded" name="butDisbursementVendorPriceHistory">
                            <div class="gHeader">
                                Vendor Price History
                            </div>
                            <div class="gBody" name="" style="font-size:18px;">
                            </div>
                            <div class="gFooter">

                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <!-- div #tabPurchasingWork was moved to \Scripts\sources\ERP\PurchaseOrder\work\view.html -->
            <div id="tabPurchasingPoBySku" class="tab-pane fade">
                PO's By SKU
            </div>
            <div id="tabPurchasingPoByBackorder" class="tab-pane fade">
                PO's By Backorder
            </div>
            <div id="tabPurchasingSettings" class="tab-pane fade">
                Settings
            </div>
        </div>        
    </div>

    <div id="pagePIMUploadAnalyze" style="display:none;">

        <div data-role="header">
            <h2>Analyze Uploaded File</h2>
        </div>

        <div data-role="content">

            <br />
            <div data-role="fieldcontain">
                <fieldset data-role="controlgroup">
                    <div><legend>We have detected a new data column(s) on the Excel file you are attempting to upload. Please set the equivalent Mobiz data field(s) for these columns.</legend></div>
                    <div style="display:none;"><input type="text" id="txtPIMUploadTemplateName" /></div>
                </fieldset>
            </div>
            <br />
            <div id="divPIMUploadAnalyzeFailed" style="display:none;">
                <p>
                    Something went wrong while trying to save your data. Upon validating, the system has detected either some required fields that were left blank.
                    Please <a id="lnkPIMUploadAnalyzeTemplateFailedDL" style="cursor:pointer; font-size:16px;">download your file</a> to see the problem rows highlighted in red.
                </p>
                <p>Once you have corrected your file, please upload it back. Thank you.</p>
            </div>
            <br />
            <div id="divFailedMessages" style="display:none;">
            </div>
            <br />
            <br />

            <div id="divPIMUploadAnalyze">
            </div>

            <div>
                <br />
                <br />
                <div style="text-align:center"> <a id="btnOrgPIMUpload" data-role="button">Upload</a> </div>
                <div style="text-align:center"> <a id="btnOrgPIMUploadCancel" data-role="button">Cancel</a> </div>
            </div>

        </div>

    </div>
    <div data-role="popup" id="divPopupAccountsPayablegOptions" data-overlay-theme="a" data-theme="a" data-corners="false" style="display:none">
        <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
        <center><h3 name="title">Options for Verifying Accounts Payables</h3></center>
        <div data-role="content" class="ui-content ui-body-d">
            <br />
            <br />
            <div data-role="fieldcontain">
                <label for="chkGLEGAS7CZZ">In Unit Price to be Paid field, retrieve and follow the "Vendor Cost" information found in the SKU data library (i.e. from PIM AKA Product Information Management settings).</label>
                <input name="chkFollowVendorCost" id="chkGLEGAS7CZZ" type="checkbox" />
            </div>
            <br />
            <button name="butOptionSave" class="btn custom-btn-theme-b">Save</button>
            <br />
            <br />
        </div>
    </div>
</div>