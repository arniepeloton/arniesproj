@Code
    ViewData("Title") = "Rentable Asset View"
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code
@Section head
    <link rel="stylesheet" href="/Content/flatui-css-custom.css" />
    
    <script>
        require([GetPathFormat('/Scripts/sources/SalesMenu/RentableAssetView.js?crc=')]);
    </script>
End Section

<style type="text/css">
    .ui-page-footer-fixed
    {
        padding-bottom: 0 !important;   
    }
</style>

<div>
    <div id="divHeader">

    </div>
    
    <div id="divPopup">
        <div data-role="popup" id="popFilterBy" data-overlay-theme="a" data-theme="a" data-corners="false">
            <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
            <center><h3>Rate Card</h3></center>
            <div data-role="content" class="ui-content ui-body-d">
                <div class="custom-radio">
                    <label class="radio-only">
                        <input type="radio" name="radRateCard" data-iconpos="notext" />                        
                    </label>
                    <div>Night</div>
                </div>
                <div class="custom-radio">
                    <label class="radio-only">
                        <input type="radio" name="radRateCard" data-iconpos="notext" />
                    </label>
                    <div>Week</div>
                </div>
                <div class="custom-radio">
                    <label class="radio-only">
                        <input type="radio" name="radRateCard" data-iconpos="notext" />
                    </label>
                    <div>Month</div>
                </div>
                <div class="custom-radio">
                    <label class="radio-only">
                        <input type="radio" name="radRateCard" data-iconpos="notext" />
                    </label>
                    <div>Year</div>
                </div>
                <br />
                <input type="text" id="txtNo" />

                <br />
                <button data-role="none" id="btnRateCardOk" class="btn custom-btn-theme-b">Ok</button>
                <br /><br />
                <button data-role="none" id="btnRateCardCancel" class="btn custom-btn-theme-c">Cancel</button>
            </div>
        </div>
    </div>    

    <div custom-data-role="page" id="divRentableAssets" style="display:block;">
        <div data-role="content">
            <div class="custom-ui-label">
                <div>Filter</div>
                <div>
                    <select>
                        <option>Please Select</option>
                    </select>
                </div>
            </div>

            <table id="tblRentableAsset" data-role="table" data-mode="reflow" style="margin-right:5px; display:table;" class="ui-table ui-responsive table-stroke table-stripe custom-tbl-responsive">
                <thead>
                    <tr class="custom-theme-f">
                        <th>#</th>
                        <th>Guest Name</th>
                        <th>Rentable Asset Name</th>
                        <th>Rent Type</th>
                        <th>Status</th>
                        <th>Length of Stay</th>
                        <th>Rate</th>
                        <th>Marketing Description</th>
                    </tr>
                </thead>

                <tbody>
                    <tr class="btnRentableAsset">
                        <td>1.</td>
                        <td>Arnie L. Peloton</td>
                        <td>Room 101, Bed 1</td>
                        <td></td>
                        <td>Occupied</td>
                        <td>60 days</td>
                        <td>250 / Night</td>
                        <td>King Size Bed</td>
                    </tr>
                </tbody>
            </table>
        </div>
    </div>

    <div custom-data-role="page" data-role="content" id="divCart" style="display:none;">
    </div>
</div>