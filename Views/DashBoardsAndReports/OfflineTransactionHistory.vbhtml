<script>
    require([GetPathFormat('/Scripts/sources/DashBoardsAndReports/OfflineTransactionHistory.js?crc=' + assetversion)]);
</script>
<style>
    table.myTable {
        border-collapse: collapse;
        width: 100%;
    }

    .ui-table-reflow.ui-responsive {
        display: table;
    }

    .SearchText {
        margin: 0px;
        border: 0px;
        text-shadow: 0px;
        background: #FFF;
        height: 40px;
        border-radius: 0px;
    }

    .ui-focus {
        -moz-box-shadow: none !important;
        -webkit-box-shadow: none !important;
        box-shadow: none !important;
    }

    .ui-shadow-inset {
        box-shadow: none !important;
    }

    .ui-dialog-contain {
        max-width: 700px;
    }

    .ui-dialog-background {
        opacity: 0.5;
        display: block !important;
        -webkit-transition: opacity 0.5s ease-in;
    }

        .ui-dialog-background.pop.in {
            opacity: 1;
            -webkit-transition: opacity 0.5s ease-in;
        }

    .ui-dialog {
        min-height: 100% !important;
        background: transparent !important;
    }
</style>

<div id="divOfflineTransactionMain" subpage="divDBOfflineTransactionHistory" style="display:block;">
    <div class="mb-3">
        <select id="cbOfflineTransactionHistoryFilterFowst" data-role="none" class="form-control form-select fixed"></select>
    </div>

    <div id="divOfflineTransactionHistoryList" style="overflow:auto;"></div>

</div>

<div id="divOfflineTransactionDetails" subpage="divDBOfflineTransactionHistory" style="display:none; overflow:auto;">
    <div id="divOfflineTransactionDetailsProducts" style="overflow:auto;"></div>

    <div class="mt-4 text-center">
        <button id="btnOfflineTransactionDetailsBack" data-role="none" class="btn btn-less-width custom-btn-theme-c btn-secondary btn-lg">
            Back
        </button>
    </div>
</div>