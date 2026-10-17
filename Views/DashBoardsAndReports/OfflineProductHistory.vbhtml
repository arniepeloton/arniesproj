<script>
    require([GetPathFormat('/Scripts/sources/DashBoardsAndReports/OfflineProductHistory.js?crc=' + assetversion)]);
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

<div style="display:block">
    <div class="mb-3">
        <select id="cbOfflineProductHistoryFilterFowst" data-role="none" class="form-control form-select fixed"></select>
    </div>
</div>

<div id="divOfflineProductHistoryList" style="overflow:auto;"></div>

@*<div id="popApprove" data-role="popup">
    <ul data-role="listview" data-inset="true" style="min-width: 210px;">
        <li data-role="divider">Approve Options</li>
        <li><a name="Approve">Approve</a></li>
        <li><a name="Disapprove">Disapprove</a></li>
        <li><a name="View">View</a></li>
    </ul>
</div>*@