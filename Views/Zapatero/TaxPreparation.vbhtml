@Code
    ViewData("Title") = "Adjustment Tool for E-Journal Entries"
    Layout = "~/Views/Shared/Layout3Single.vbhtml"
End Code

<link rel="stylesheet" href="/Content/bootstrap-custom.css" />
<link rel="stylesheet" href="/Content/flatui-css-custom.css" />
<link rel="stylesheet" href="/Content/jquery.dataTables2.css" />


<script>
    require([GetPathFormat('/Scripts/sources/Zapatero/TaxPreparation.js?crc=')]);
</script>

<div>

    <div class="container" style="margin-top:30px;">
        <div class="row">
            <div class="col-sm-6">
                <div class="row">
                    <div class="col-sm-6">
                        Machine Name:
                    </div>
                    <div class="col-sm-6">
                        <input data-role="none" type="text" id="lblMachineName" class="form-control" />
                    </div>
                </div>
                <br />
                <div class="row">
                    <div class="col-sm-6">
                        Machine ID Number (MIN):
                    </div>
                    <div class="col-sm-6">
                        <input data-role="none" type="text" id="lblMachineNo" class="form-control" disabled />
                    </div>
                </div>
               
                <br />
                <div class="row">
                    <div class="col-sm-6">
                        Select Year:
                    </div>
                    <div class="col-sm-2">
                        <fieldset data-role="controlgroup" data-type="horizontal" style="padding-top:0px; margin-top:0px">
                            <select id="selYear" data-mini="true">
                                <option value="" selected="selected">Year </option>
                                <option value="2013">2013 </option>
                                <option value="2014">2014 </option>
                                <option value="2015">2015 </option>
                                <option value="2016">2016 </option>
                                <option value="2017">2017 </option>
                                <option value="2018">2018 </option>
                                <option value="2018">2019 </option>
                                <option value="2018">2020 </option>
                            </select>
                        </fieldset>     
                    </div>
                    <div class="col-sm-4" style="text-align:left">
                        <button data-role="none" class="btn custom-btn-theme-c" id="btnGo">Go</button>
                    </div>
                </div>
            </div>
            <div class="col-sm-6">
                <div class="row">
                    <div class="row">
                        <div class="col-sm-6">
                            Branch:
                        </div>
                        <div class="col-sm-6">
                            <input data-role="none" type="text" id="lblBranch" class="form-control" disabled />
                        </div>
                    </div>
                    <br />
                    <div class="row">
                        <div class="col-sm-6">
                            PTU:
                        </div>
                        <div class="col-sm-6">
                            <input data-role="none" type="text" id="lblRegistrationNo" class="form-control" disabled />
                        </div>
                    </div>
                </div>
            </div>
            <br />
        @*<div class="row">
            <div class="col-sm-3">
                <button data-role="none" class="btn custom-btn-theme-c" id="btnGetTransactionData">Retrieve Actual Data</button>
            </div>
            <div class="col-sm-1">
                <button data-role="none" class="btn btn-transparent">
                    <span class="glyphicon glyphicon-chevron-right"></span>
                </button>
            </div>
            <div class="col-sm-3">
                <button data-role="none" class="btn custom-btn-theme-c" id="btnRecompute">Adjust Data for Best Fit</button>                 
            </div>
            <div class="col-sm-1">
                <button data-role="none" class="btn btn-transparent">
                    <span class="glyphicon glyphicon-chevron-right"></span>
                </button>
            </div>
            <div class="col-sm-4">
                <button data-role="none" class="btn custom-btn-theme-b" id="btnSaveTransactionData">Save Best Fit Data</button>
            </div>
        </div>
        <br />*@
    </div>
            
    <div style="margin-top:20px; margin-bottom:20px">
        <div style="width:100%; overflow:auto;">
            <table id="tblTransactionDetail" data-role="table" data-mode="reflow" style="margin-right:5px; display:table;" class="ui-table ui-shadow ui-responsive table-stroke table-stripe">
                <thead>
                    <tr style="text-align:center" class="custom-theme-f txt-center">
                        <th>Month</th>
                        <th colspan="2">Actual</th>
                        <th colspan="2">Target</th>
                        <th>D.N.M</th>
                        <th>Computed</th>
                        <th>Action</th>
                    </tr>
                    <tr style="text-align:center" class="custom-theme-f txft-center">
                        <th></th>
                        <th>Ending SI No.</th>
                        <th>Sales</th>
                        @*<th colspan="2">SI</th>*@
                        <th colspan="2">Sales</th>
                        <th></th>
                        @*<th>SI</th>*@
                        <th>Sales</th>
                        <th></th>
                    </tr>
                    <tr id="tblHeaderDT" class="custom-theme-f txt-center">                       
                        <th></th>
                        <th></th>
                        <th></th>
                        @*<th>SI From</th>
                        <th>SI To</th>*@
                        <th>As % of Actual Sales</th>
                        <th>Target Sales Amount</th>
                        <th></th>
                        <th></th>
                        @*<th></th>*@
                        <th></th>
                    </tr>
                </thead>
                <tbody id="tblTransactionDetailsBody"></tbody>
            </table>
        </div>
    </div>

    @*<div>
        <button id="btnSaveTransactionData" data-mini="true">Save changes in transaction data</button>
    </div>*@
           
    <div data-role="popup" id="panSetDiscount" data-overlay-theme="a" data-theme="a" data-corners="false">
        <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right" id="btnClosePopUp">Close</a>
        <center><h3 style="margin-left:10px; margin-right:10px;">Adjust Data for Best Fit<span class="lblPopUpHeaderLibraryName"></span></h3></center>
        <div data-role="content" style="max-width:350px;" class="ui-content ui-body-d" data-theme="a">
            <div style="margin-left:10px; margin-right:10px;">
                
                @*<center><h3 style="margin-left:10px; margin-right:10px;">Enter Discount in percent(%)<span class="lblPopUpHeaderLibraryName"></span></h3></center>
                <input type="number" data-clear-btn="true" id="txtDiscount" value="">*@

                <br />
                <div class="ui-grid-solo">
                    <div class="ui-block-a">
                        <button id="btnAdjustBestFirstByRemovingItems" data-role="none" class="btn custom-btn-theme-b">
                            By Removing Items And Adjusting Qty
                        </button>
                        <br /> <br />
                        <button id="btnAdjustBestFirstByApplyDiscount" data-role="none" class="btn custom-btn-theme-b">
                           By Applying Discount
                        </button>

                    </div>
                </div>
            </div>
            <br />
        </div>
    </div>
</div>
