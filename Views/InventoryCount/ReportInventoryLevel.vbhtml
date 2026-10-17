@*<link rel="stylesheet" href="/Content/flatui-css-custom.css" />
<link rel="stylesheet" href="/Content/bootstrap-custom.css" />
<link rel="stylesheet" href="/Content/jquery.dataTables2.css" />*@

<script>
    require([GetPathFormat('/Scripts/sources/InventoryLedger/InventoryLevel.js?crc=' + assetversion)]);
</script>

<style>
    .btnRequired {
        -webkit-transition: box-shadow 1s linear;
        -moz-transition: box-shadow 1s linear;
        -ms-transition: box-shadow 1s linear;
        -o-transition: box-shadow 1s linear;
        transition: box-shadow 1s linear;
        border: 2px solid #FFF;
    }

    .btnRequired {
        border: 2px solid #F00;
    }

    .btnNotRequired {
        border: 2px solid #FFF;
    }
</style>

<div id="pageInventoryBalanceReport">
    <div data-role="content">
        <div id="divSecond" style="display:block;">            
            <div style="display:block">
                <div class="row">
                    <div class="col-auto mb-3">
                        FOWST: <br />
                        <button id="btnInventoryLevelFowst" style="margin-right:10px;" data-role="none" class="btn btn-less-width custom-btn-theme-c btn-secondary btnSelectFowst">
                            <span class="lblSelectedFowstGroup"> FOWST Location </span>
                        </button>
                    </div>
                </div>

                <div id="dvDetails" style="width:100%; overflow:auto">
                    
                    <table id="tblInventoryLevel" data-role="table" style="margin-right:5px; display:table; width:100%;" class="display ui-table ui-responsive table-stroke table-stripe custom-tbl-responsive table table-light table-striped table-hover">
                        <thead>
                            <tr class="custom-theme-f fixed-cell-width">
                                <th class="txt-center"> Product </th>
                                <th class="txt-center"> Bin </th>
                                <th class="txt-center"> Unit Measure </th>
                                <th class="txt-center"> On Hand </th>
                                <th class="txt-center"> Max Cap </th>
                                <th class="txt-center"> Reorder Point </th>
                                <th class="txt-center"> Reorder Quantity </th>
                                <th class="txt-center"> Remarks </th>
                            </tr>
                        </thead>
                        <tbody></tbody>
                    </table>
                </div>
            </div>
          
        </div>
      
    </div>
</div>