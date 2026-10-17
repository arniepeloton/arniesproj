@Code
    'ViewData("Title") = "ShopperTrak"
    'Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head

End Section

<script>
    require([GetPathFormat('/Scripts/sources/ShopperTrak/ShopperTrak.js?crc=' + assetversion)]);
</script>

<div id="pageShopperTrak" data-role="page">
    <div class="row">
        <div class="col-sm-1 mb-3">From:</div>
        <div class="col-sm-2 mb-3">
            <select id="selShopperTrakByDateFromMonth" data-role="none" class="form-select">
                <option value="">MMM</option>
            </select>
        </div>
        <div class="col-sm-2 mb-3">
            <select id="selShopperTrakByDateFromDay" data-role="none" class="form-select">
                <option value="">DD</option>
            </select>
        </div>
        <div class="col-sm-2 mb-3">
            <select id="selShopperTrakByDateFromYear" data-role="none" class="form-select">
                <option value="">YYYY</option>
            </select>
        </div>
    </div>

    <div class="row">
        <div class="col-sm-1 mb-3">To:</div>
        <div class="col-sm-2 mb-3">
            <select id="selShopperTrakByDateToMonth" data-role="none" class="form-select">
                <option value="">MMM</option>
            </select>
        </div>
        <div class="col-sm-2 mb-3">
            <select id="selShopperTrakByDateToDay" data-role="none" class="form-select">
                <option value="">DD</option>
            </select>
        </div>
        <div class="col-sm-2 mb-3">
            <select id="selShopperTrakByDateToYear" data-role="none" class="form-select">
                <option value="">YYYY</option>
            </select>
        </div>
    </div>

    <div class="mt-4 text-center">
        <button id="btnShopperTrakReportSalesData" data-role="none" class="btn btn-secondary btn-lg mb-3">Generate Sales (POS) Data</button>
        <a id="btnShopperTrakReportLaborData" data-role="none" class="btn btn-success btn-lg mb-3">Generate Labor Data</a>
    </div>
</div>