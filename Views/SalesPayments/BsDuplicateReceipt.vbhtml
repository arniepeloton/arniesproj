@Code
    ViewData("Title") = "Duplicate Receipt"
    
    If Request.IsAjaxRequest() Then
        Layout = ""
    Else
        Layout = "~/Views/Shared/BootstrapFiveLayout.vbhtml"
    End If
End Code
@Section head
    <script>
        require(['bootstrapjs'], function () {
            require(['/Content/bootstrap/js/bootstrap_modal.js?v='], function () {
                require([GetPathFormat('/Scripts/sources/SalesPayments/Bs_DuplicateReceipt.js?crc='),
                    '/Scripts/sources/SetupChecklist/NewPromptUser.js',
                ]);
            })
        })        
    </script>
End Section

<div id="pageSalesPaymentDuplicateReceipt" subpage="pageDuplicateReceipt" style="display:block;">
    <nav class="navbar navbar-dark bg-primary sticky-sm-top text-center" style="font-size:large; color:#FFF; display:block;">
        Duplicate Receipt
    </nav>
    <div class="container-fluid mt-3">
        <div class="row mb-3">
            <label for="btnTransInvoice" class="col-sm-2">Click to search invoice:</label>
            <div class="col-sm-10">
                <button data-role="none" id="btnTransInvoice" class="btn btn-secondary btn-block">Search Invoice</button>
            </div>
        </div>

        <div class="row mb-3">
            <label for="txtDuplicateReceiptReceiptNo" class="col-sm-2">Receipt No. <span style="color: #FF0000;">*</span></label>
            <div class="col-sm-10">
                <input data-role="none" class="form-control" type="number" id="txtDuplicateReceiptReceiptNo" maxlength="15" />
            </div>
        </div>

        <div class="row">
            <div class="offset-sm-2 col-sm-10">
                <button data-role="none" id="btnDuplicateReceiptPrint" class="btn btn-light btn-lg btn-block mb-3 mt-5">Print Invoice Receipt</button>
                @*HIDE TEMPORARY FOR BIR DEMO<button data-role="none" id="btnDuplicateReceiptQueueNumber" class="btn btn-light btn-lg btn-block mb-3" style="display:none;">Print Queue Number</button>
                <button data-role="none" id="btnDuplicateDeliveryPrint" class="btn btn-light btn-lg btn-block mb-3">Print Delivery Receipt</button>
                <button data-role="none" id="btnDuplicateReceiptEmail" class="btn btn-light btn-lg btn-block mb-3">Email Invoice Receipt</button>
                <button data-role="none" id="btnDuplicateReceiptPrintEmail" class="btn btn-light btn-lg btn-block mb-3">Print and Email Receipt</button>*@
                <button data-role="none" id="btnDuplicateReceiptBack" class="btn btn-light btn-lg btn-block mb-3">Back</button>
            </div>
        </div>
    </div>
</div>

<div id="divDuplicateReceiptPromptEmail" subpage="pageDuplicateReceipt" style="display:none;">
    <nav class="navbar navbar-dark bg-primary sticky-sm-top text-center" style="font-size:large; color:#FFF; display:block;">
        Email Address
    </nav>

    <div class="container-fluid mt-3">
        <div class="mb-3">
            Please check the email address(es) that you want to send an electronic invoice to.
        </div>

        <div id="divDuplicateReceiptEmail" class="row mb-3">
            <div id="divDuplicateReceiptEmailNone" style="display:none; font-weight:bolder; color:Red;"><label>No Available Email</label></div>
            <fieldset id="fldsetDuplicateEmailEmail" data-role="controlgroup"></fieldset>
        </div>

        <div class="row mb-3">
            <div class="col-sm-2 mb-3 col-form-label">
                Other email address:
            </div>
            <div class="col-sm-10">
                <input data-role="none" type="text" id="txtDuplicateReceiptEmailAddress" class="form-control form-control-lg mb-3" placeholder="Enter other email addresses here" />
                (Please separate multiple email addresses with a comma. Ex. sample1@email.com, sample2@email.com)
            </div>
        </div>

        <div class="text-center mt-4">
            <button data-role="none" id="btnDuplicateReceiptEmailCancel" class="btn btn-secondary btn-lg">Cancel</button>
            <button data-role="none" id="btnDuplicateReceiptEmailOK" class="btn btn-success btn-lg">Send</button>
        </div>
    </div>
</div>

<div id="pagePaymentReprint" style="display:none;">

</div>