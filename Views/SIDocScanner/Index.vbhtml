@Code
    ViewData("Title") = "SI Document Scanner"
    Layout = "~/Views/Shared/BootstrapFiveLayout.vbhtml"
End Code

@Section head 

<link rel="stylesheet" href="/Content/jquery.dataTables2.css" />

    <script>
        require(['bootstrapjs'], function () {
            require(['/Content/bootstrap/js/bootstrap_modal.js?v=' + assetversion], function () {
                require([GetPathFormat('/Scripts/sources/SIDocScanner/SIDocScanner.js?crc=' + assetversion)]);
            })
        })
    </script>

    <style>
        body {
            background-color: #f3f3f3 !important;
        }
    </style>
End Section

<div class="container">
    <div id="pageHome" subpage="scanner" style="display:block;">
        <div class="text-center m-3">
            <h1 class="text-primary">Choose Options</h1>
        </div>

        <div class="offset-lg-3 col-lg-6 mt-5">
            <div id="btnCustomerSI" class="border-dark rounded bg-white p-4 text-center text-dark mb-5" style="border:2px dashed; cursor:pointer;">
                <span class="bi-people" style="font-size:50px;"></span>
                <br /><br />
                <span style="font-size:20px;">Customer Invoice Photo Scanning</span>
            </div>

            <div id="btnVendorSI" class="border-dark rounded bg-white p-4 text-center text-dark mb-5" style="border:2px dashed; cursor:pointer;">
                <span class="bi-shop" style="font-size:50px;"></span>
                <br /><br />
                <span style="font-size:20px;">Vendor Invoice Photo Scanning</span>
            </div>
        </div>        
    </div>

    <div id="pageCustomerSI" subpage="scanner" class="offset-lg-3 col-lg-6 mb-5" style="display:none;">
        <div class="row mt-3 mb-3 ml-0 mr-0">
            <div class="col-auto">
                <button data-role="none" class="btn btn-lg btnBackHome"><span class="bi bi-chevron-left"></span></button>
            </div>
            <div class="col">
                <h1 class="text-primary">Customer Invoice Photo Scanning</h1>
            </div>
        </div>

        <div id="pageSearch" subpage="sidocscanner" style="display:block;">
            <form id="formSearch">
                <input data-role="none" type="search" id="txtSearch" class="form-control form-control-lg text-center" placeholder="Search Invoice Number" />
            </form>

            <div id="divResults" class="list-group mt-3">

            </div>
        </div>

        <div id="pageUpload" subpage="sidocscanner" style="display:none;">
            <input data-role="none" type="file" id="fileAttach" accept="image/*" multiple style="margin-left:-1000px; visibility:hidden; display:none;" />
            <div class="text-center">
                Invoice No:
                <br />
                <h3 id="lblInvoice"></h3>
                <br />
                <div id="btnUpload" class="border-primary rounded bg-white p-4 text-center text-primary" style="border:2px dashed; cursor:pointer;">
                    <span class="bi-upload" style="font-size:50px;"></span>
                    <br /><br />
                    <span style="font-size:20px;">Upload file</span>
                </div>
                <br />
                <button data-role="none" id="btnSearchAgain" class="btn btn-light border">Search again</button>
            </div>
        </div>
    </div>    
    
    <div id="pageVendorSI" subpage="scanner" class="offset-lg-3 col-lg-6 mb-5" style="display:none;">
        <div class="row mt-3 mb-3 ml-0 mr-0">
            <div class="col-auto">
                <button data-role="none" class="btn btn-lg btnBackHome"><span class="bi bi-chevron-left"></span></button>
            </div>
            <div class="col">
                <h1 class="text-primary">Vendor Invoice Photo Scanning</h1>
            </div>
        </div>

        <div class="mb-3">
            <label class="form-label" for="btnVendor">Vendor Name: <span class="required">*</span></label>
            <button data-role="none" id="btnVendor" class="btn btn-secondary btn-lg btn-block">Search Vendor</button>
        </div>

        <div class="mb-3">
            <label class="form-label" for="txtVendorInvoiceRefNo">Invoice Reference No.: <span class="required">*</span></label>
            <input data-role="none" type="text" id="txtVendorInvoiceRefNo" class="form-control form-control-lg" />
        </div>

        <div class="mb-3">
            <label class="form-label" for="txtVendorInvoiceRefDate">Invoice Date: <span class="required">*</span></label>
            <input data-role="none" type="date" id="txtVendorInvoiceRefDate" class="form-control form-control-lg" />
        </div>

        <div class="mb-3">
            <label class="form-label" for="txtVendorNoOfLineItems">No. of Line Items: <span class="required">*</span></label>
            <input data-role="none" type="number" id="txtVendorNoOfLineItems" class="form-control form-control-lg" />
        </div>

        <div class="mb-3">
            <label class="form-label" for="txtVendorInvoiceRefAmount">Invoice Amount: <span class="required">*</span></label>
            <input data-role="none" type="number" id="txtVendorInvoiceRefAmount" class="form-control form-control-lg" />
        </div>

        <div class="mb-3">
            <label class="form-label" for="txtVendorInvoiceRefRemarks">Remarks:</label>
            <textarea data-role="none" id="txtVendorInvoiceRefRemarks" class="form-control" style="resize:none; height:150px;"></textarea>
        </div>

        <input data-role="none" type="file" id="VendorfileAttach" accept="image/*" style="margin-left:-1000px; visibility:hidden; display:none;" />
        <div id="btnVendorUpload" class="border-primary rounded bg-white p-4 text-center text-primary" style="border:2px dashed; cursor:pointer;">
            <span class="bi-upload" style="font-size:50px;"></span>
            <br /><br />
            <span style="font-size:20px;">Upload file</span>
        </div>
    </div>    
</div>