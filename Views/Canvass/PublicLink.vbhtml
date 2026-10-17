@Code
    ViewData("Title") = "Canvass"
    Layout = "~/Views/Shared/VendorPortalLayout.vbhtml"
End Code
@Section head
    <script>
        require(['promise!/Content/Ecommerce/js/bootstrap.min.js',
        'promise!/Content/bootstrap/js/bootstrap_modal.js?v=' + assetversion,
        'promise!/Scripts/jquery.filedownload.min.js',
        'bootstrap-tooltip'], function () {
            setTimeout(function () {
                require([GetPathFormat('/Scripts/sources/Canvass/PublicLink.js?crc=' + assetversion)]);
            }, 200)            
        })        
    </script>
End Section

<div>
    <div id="pageCanvassForm" subpage="pageCanvass" style="display:none;">                
        <div class="BreadCrumbs">
            <div class="container-fluid">
                <div class="ItemsBreadCrumbs">

                </div>
            </div>
        </div>
        <div class="container-fluid">
            
            <br />
            @*<div>
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
            <li>Click DONE button below to submit your prices</li>
        </ol>
    </div>*@

            <div class="alert alert-danger divNeedVendorUser" role="alert" style="display:none;">
                Please signin first before you enter your prices. <a data-role="none" id="btnLoginVendor" class="custom-link">Click here to signin.</a>
            </div>
        </div>
        <div class="container-fluid" style="position:relative">
            <div class="divNeedVendorUser" style="display:none; position: absolute; bottom: 0; top: 0; right: 0; left: 0; background-color: #FFF; opacity: 0.5; display: none; z-index: 1;">

            </div>
            <div id="divCanvassFormLink">

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
</div>