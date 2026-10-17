@Code
    ViewData("Title") = "Canvass"
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code
@Section head
<link rel="stylesheet" href="/Content/bootstrap.css" />
    <link rel="stylesheet" href="/Content/flatui-css-custom.css" />
    <link rel="stylesheet" href="/Content/bootstrap-custom.css" />
    <link rel="stylesheet" href="/Content/jquery.dataTables2.css" />
    <link rel="stylesheet" href="/Content/jquery.buttons.dataTables.min.css" />
    <script>
        require([GetPathFormat('/Scripts/sources/Canvass/Canvass.js?crc=')]);
    </script>
End Section

<div style="width: 100%; height: 300px; background-color: #335F8E; display:block; position:fixed;">

</div>

<div data-role="popup" id="popCanvassCashDiscount" data-overlay-theme="a" data-theme="a" data-corners="false" data-position-to="window">
    <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
    <center><h3>Cash Discount</h3></center>
    <div data-role="content" class="ui-content ui-body-d jPopupContent">
        <form id="formCanvassCashDiscount" method="post">
            Fill the required fields with <span class="required">*</span>
            <br /><br />
            <div class="row">
                <div class="col-sm-4">Cash Discount Rate (%): <span class="required">*</span></div>
                <div class="col-sm-8"><input type="text" id="txtCanvassCDRate" data-role="none" class="form-control number-only" /></div>
            </div>
            <br />
            <div class="row">
                <div class="col-sm-4">If paid in: (days) <span class="required">*</span></div>
                <div class="col-sm-8"><input type="text" id="txtCanvassCDIfPaidIn" data-role="none" class="form-control number-only" /></div>
            </div>
            <br />
            <div class="row">
                <div class="col-sm-4">Net amount to be paid in: (days) <span class="required">*</span></div>
                <div class="col-sm-8"><input type="text" id="txtCanvassCDNetAmount" data-role="none" class="form-control number-only" /></div>
            </div>
            <br /><br />
            <button type="submit" data-role="none" href="#" id="btnOkCanvassCashDiscount" class="btn custom-btn-theme-b">Ok</button>
            <br /><br />
            <a data-role="none" href="#" data-rel="back" class="btn custom-btn-theme-c">Cancel</a>
        </form>
    </div>
</div>

<div data-role="popup" id="popCanvassTradeDiscount" data-overlay-theme="a" data-theme="a" data-corners="false" data-position-to="window">
    <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
    <center><h3>Trade Discount</h3></center>
    <div data-role="content" class="ui-content ui-body-d jPopupContent">
        <i>
            Please enter the trade discounts (excluding volume and cash discounts for this SKU). Separate trade discounts with commas. <br />
            Example: Less 5,2,3
        </i>
        <br /><br />
        <div class="divTable" style="font-size: inherit !important;">
            <div class="divTableCell" style="width:60px;">Less: <span class="required">*</span></div>
            <div class="divTableCell"><input type="text" data-role="none" class="form-control" id="txtTradeDiscountLess" /></div>
            <div class="divTableCell" style="width:50px;">%</div>
        </div>
        <br />
        <div>
            Net Discount Factor: <span id="lblTradeDiscountFactor"></span>
        </div>
        <br />
        <div>
            Net Discount Rate: <span id="lblTradeDiscountRate"></span>
        </div>
        <br />
        <h4>Remarks:</h4>
        <br />
        <textarea data-role="none" id="txtTradeDiscountRemarks" class="form-control" style="height:100px; resize:none"></textarea>
        <br /><br />
        <a data-role="none" href="#" id="btnOkCanvassTradeDiscount" class="btn custom-btn-theme-b">Ok</a>
        <br /><br />
        <a data-role="none" href="#" data-rel="back" class="btn custom-btn-theme-c">Cancel</a>
    </div>
</div>

<div data-role="popup" id="popCanvassComment" data-overlay-theme="a" data-theme="a" data-corners="false" data-position-to="window">
    <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
    <center><h3>Comment</h3></center>
    <div data-role="content" class="ui-content ui-body-d jPopupContent">
        <form id="formCanvassComment" method="post">
            <textarea data-role="none" id="txtCanvassComment" class="form-control" style="height:200px; resize:none;"></textarea>
            <br />
            <button data-role="none" type="submit" class="btn custom-btn-theme-b">Ok</button>
            <br /><br />
            <a data-rel="back" data-role="none" class="btn custom-btn-theme-c">Cancel</a>
        </form>
    </div>
</div>

<div data-role="popup" id="popUserLogin" data-overlay-theme="a" data-theme="a" data-corners="false" data-position-to="window">
    <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
    <center><h3>Security Validation</h3></center>
    <div data-role="content" class="ui-content ui-body-d jPopupContent">
        <form id="formCanvassLogin" method="post" autocomplete="off">
            We want to make sure if you are really the administrator of <b class="lblVendorName"></b>. Please enter your username and password.
            <br /><br />
            <input type="text" data-role="none" autocomplete="off" class="form-control" id="txtUsername" placeholder="Username" />
            <br />
            <input type="password" data-role="none" autocomplete="off" class="form-control" id="txtPassword" placeholder="Password" />
            <br />
            <span style="display:none;" id="errLogin" class="required">Invalid username and password</span>
            <br /><br />
            <button data-role="none" type="submit" class="btn custom-btn-theme-b">Login</button>
            <br /><br />
            <a data-rel="back" data-role="none" class="btn custom-btn-theme-c">Cancel</a>
        </form>
    </div>
</div>

<div id="popCanvassAdjustPrices" custom-data-role="popup">
    <div class="popup-md">
        <div class="popup-header">
            Apply Discounts by Groupings
            <a class="popup-close">x</a>
        </div>

        <div class="popup-content">
            <div>Use this tool if the prices are repetitive.</div>
            <h4>Where do you want to apply the discounts?</h4>
            <div class="row">
                <div class="col-sm-3">Product Category:</div>
                <div class="col-sm-9">
                    <select data-role="none" id="selAdjustPricesCategory" class="form-control">
                        <option value="">All Categories</option>
                    </select>
                </div>
            </div>
            <br />
            <div class="row">
                <div class="col-sm-3">Brand</div>
                <div class="col-sm-9">
                    <select data-role="none" id="selAdjustPricesBrand" class="form-control">
                        <option value="">All Brands</option>
                    </select>
                </div>
            </div>
            <h4>Discount Rates to be Applied:</h4>
            <div class="row">
                <div class="col-sm-3">Trade Discount:</div>
                <div class="col-sm-9">
                    <div class="input-group">
                        <input data-role="none" type="text" id="txtAdjustPricesTradeDiscounts" class="form-control">
                        <span class="input-group-addon"> %</span>
                    </div>
                </div>
            </div>
            <br />
            <div class="row">
                <div class="col-sm-3">Cash Discount:</div>
                <div class="col-sm-9">
                    <input data-role="none" type="text" id="txtAdjustPricesCashDiscounts" readonly class="form-control" />
                </div>
            </div>
            <br />
            <div class="row">
                <div class="col-sm-3">Per PO Volume Discount:</div>
                <div class="col-sm-9">
                    <div class="input-group">
                        <input data-role="none" type="text" id="txtAdjustPricesVolumeDiscount" disabled placeholder="To be implemented" class="form-control">
                        <span class="input-group-addon"> %</span>
                    </div>
                </div>
            </div>
        </div>

        <div class="popup-footer align-right">
            <div style="text-align:right;">
                <button data-role="none" class="btn custom-btn-theme-c btn-less-width popup-close">
                    Cancel
                </button>
                <button id="btnAdjustPricesOk" data-role="none" class="btn custom-btn-theme-b btn-less-width">
                    Apply Discount Rates To This Grouping
                </button>
            </div>
        </div>
    </div>
</div>

<div class="container-fluid" style="position:relative;">
    <div style="width: 100%; background-color: #fff; border-bottom-left-radius:5px; border-bottom-right-radius:5px; padding:10px; box-sizing:border-box;">
        <div id="pageCanvassForm" subpage="pageCanvass" style="display:none;">
            <div class="row">
                <div class="col-sm-3">
                    <img alt="Mobiz" src="/Content/images/MobizNewLogo.png" width="100px" />
                </div>
                <div class="col-sm-9">
                    <h1 id="lblSurveyTitle" style="padding:10px; margin:0; color:#335F8E;"></h1>
                </div>
            </div>
            <br />
            <div style="font-size:24px;">Welcome <b class="lblVendorName"></b> to Mobiz Online Bid Form.</div>
            <br />
            <div>
                <b>NOTE:</b><br />
                <ol class="list-padding" style="list-style:decimal;">                    
                    <li>Please enter your prices for each item.</li>
                    <li id="lstNoteExpiration" style="display:none;">
                        This link will expire on <b id="lblEndDate"></b>. You can submit (and resubmit) your prices before the expiration date.
                    </li>
                    <li>
                        If ever the resulting value of the "SRP" less "Trade Discount" is not equal to the "Price Net of Trade Discount", the system will follow the values encoded in the "Price Net of Trade Discount" column.
                    </li>
                    <li>You can use arrow up/down or ENTER keys to navigate to the next field.</li>
                </ol>
            </div>

            <div class="alert alert-danger divNeedVendorUser" role="alert" style="display:none;">
                Please signin first before you enter your prices. <a data-role="none" id="btnLoginVendor" class="custom-link">Click here to signin.</a>
            </div>

            <div style="position:relative">
                <div class="divNeedVendorUser" style="display:none; position: absolute; bottom: 0; top: 0; right: 0; left: 0; background-color: #FFF; opacity: 0.5; display: none; z-index: 1;">

                </div>
                <div id="divTabCanvass" class="custom-tab">
                    <ul>
                        <li class="active" custom-tab-content="#tabCanvassForm">Canvass Form</li>
                        <li custom-tab-content="#tabAttachments">Attachments</li>
                    </ul>
                </div>
                <div id="tabCanvassForm" custom-tab-role="content" style="display:block;">
                    <div class="row">
                        <div class="col-md-3">
                            <br />
                            <button data-role="none" id="btnAdjustPrice" class="btn custom-btn-theme-b btn-sm">Apply Discounts By Groupings</button>
                        </div>
                        <div class="col-md-2">
                            <br />
                            <button data-role="none" id="btnDownloadCanvassForm" class="btn custom-btn-theme-c btn-sm">Download Canvass Form</button>
                        </div>
                        <div class="col-md-2">
                            <br />
                            <button data-role="none" id="btnUploadCanvassForm" class="btn custom-btn-theme-c btn-sm">Upload Canvass Form</button>
                            <form id="formUploadCanvassFormTemplate" method="post">
                                <input data-role="none" id="fileUploadCanvassFormTemplate" type="file" accept=".xls,.xlsx" style="display:none;" />
                            </form>
                        </div>
                    </div>
                    <br />
                    <table id="tableCanvass" data-role="table" data-mode="reflow" style="margin-right:5px; display:table;" class="ui-table ui-shadow ui-responsive table-stroke table-stripe">
                        <thead>
                            <tr>
                                <th style="width:30px;">#</th>
                                <th>Item Description</th>
                                <th>Unit Measure</th>
                                <th style="width:100px;">SRP</th>
                                <th style="width:145px;">Trade Discount (%)</th>
                                <th style="width:145px;">Price Net Of Trade Discount</th>
                                <th style="width:120px;">Cash Discount</th>
                                <th style="width:100px;">Comment</th>
                            </tr>
                        </thead>
                        <tbody id="tbodyCanvassLink"></tbody>
                    </table>

                    <br />
                    <div style="text-align:center;">
                        <button data-role="none" id="btnCanvassLinkSave" class="btn custom-btn-theme-b btn-less-width">Bid Now</button>
                    </div>
                    <br />
                </div>
                <div id="tabAttachments" custom-tab-role="content" style="display:none;">
                    <br />
                    <button data-role="none" id="btnUploadAttachments" class="btn custom-btn-theme-b btn-less-width btn-sm">Upload Files</button>
                    <form id="formUploadAttachments" method="post">
                        <input data-role="none" id="fileUploadAttachments" multiple="multiple" type="file" accept=".xls,.xlsx,.doc,.docx,.pdf,image/*" style="display:none;" />
                    </form>
                    <br />
                    <table id="tableAttachments" data-role="table" data-mode="reflow" style="margin-right:5px; display:table;" class="ui-table ui-shadow ui-responsive table-stroke table-stripe">
                        <thead>
                            <tr class="custom-theme-f">
                                <th style="width:30px; text-align:center;">#</th>
                                <th style="text-align:center;">File Name</th>
                                <th style="width:200px; text-align:center;">Date Uploaded</th>
                                <th style="width:100px; text-align:center;"></th>
                            </tr>
                        </thead>
                        <tbody id="tbodyAttachments"></tbody>
                    </table>
                </div>                
            </div>
        </div>

        <div id="pageTY" subpage="pageCanvass" style="display:none; text-align:center; min-height:300px;">
            <br />
            <span class="glyphicon glyphicon-ok-circle" style="font-size:100px; color:#16a085;"></span>
            <br />
            <span style="font-size:50px; color:#24675a;">Thanks for bidding.</span>
            <br /><br />
            You can edit your bid form by clicking this link <a data-role="none" onclick="location.reload();" class="custom-link">edit now</a>.
        </div>

        <div id="pageError" subpage="pageCanvass" style="display:none; text-align:center; min-height:300px;">
            <br />
            <span class="glyphicon glyphicon-remove-circle" style="font-size:100px; color:#f44336;"></span>
            <br />
            <span id="lblErrorMsg" style="font-size:50px; color:#b71509;">There's an error!</span>
            <br /><br />
            Please contact the mobiz representative about this. Thank you.
        </div>

        <div id="pageBrokenLink" subpage="pageCanvass" style="display:none; text-align:center; min-height:300px;">
            <br />
            <span class="glyphicon glyphicon-ban-circle" style="font-size:100px; color:#ff9800;"></span>
            <br />
            <span style="font-size:50px; color:#ca7a04;">Link is not available</span>
            <br /><br />
            Either the link is not active, invalid or expired.
        </div>

        <div id="pageSigninSupplier" subpage="pageCanvass" style="display:none;">
            
        </div>
    </div>
</div>