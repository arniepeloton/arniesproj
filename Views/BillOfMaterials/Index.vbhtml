@Code
    ViewData("Title") = "Product Maker"
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code
<link type="text/css" href="/Content/flatui-css-custom.css" rel="Stylesheet" />
<style>
.ui-page-header-fixed {
    padding-top: 0px;
}

.ui-page .ui-body-c .ui-page-footer-fixed .ui-page-header-fixed .ui-page-active
{
    padding-bottom: 0px;
}

</style>
<script>
    require([GetPathFormat('/Scripts3/BillOfMaterials.js?crc=')]);
</script>
<div data-role="page" id="main">
    <div data-role="popup" id="popupMoreOptionsHotButtons" data-theme="b" class="ui-corner-all" style="width:auto;">
        <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
        <center><h3>More Options</h3></center>
        <div data-role="content">

            <div style="text-align: center">
                <a id="btnFowstHotButtons" data-role="button" data-theme="e">
                    <h3>Current FOWST Location</h3>
                    <p style="font-weight: normal; margin-top: -15px;">
                        <label id="lnkPosBranchHotButtons" style="cursor:pointer;"></label>
                        <br />
                        Product maker records will be tagged to this location.
                    </p>
                </a>
            </div>
            <div style="text-align: center">
                <a id="butNewSalesOrderHotButtons" data-role="button" data-theme="e">
                    <h3>New Cart</h3>
                    <p style="font-weight: normal; margin-top: -15px;">
                        Puts the current cart on hold and displays a new one for.
                    </p>
                </a>
            </div>
            <div style="text-align: center">
                <a id="lnkRecallProductMaker" data-role="button" data-theme="e">
                    <h3>Recall Cart</h3>
                    <p style="font-weight: normal; margin-top: -15px;">
                        Used to retrieve carts that were placed on hold.
                    </p>
                </a>
            </div>
            <div style="text-align: center" id="divChangeMenuButtons" style="display:none;">
                <a id="btnChangeMenuButtons" data-role="button" data-theme="e">
                    <h3>Change Hot Button Template</h3>
                    <p style="font-weight: normal; margin-top: -15px;">
                        Lets you use a different set of hot buttons. Administrators <br />can customize these buttons online.
                    </p>
                </a>
            </div>
        </div>

    </div>

        <div class="ui-grid-a" >
            <div class="ui-block-a" style="width: 35%;height: 100%;bottom: 0;position: fixed;overflow: auto;left: 0;">
                <div id="divCart" style="padding: 10px;">

                </div>
            </div>
            <div class="ui-block-b" style="width: 65%; right: 0; position: fixed; bottom: 0; overflow: auto; top: 0;">
                <div id="divHeader" >

                </div>

                <div id="divHotButtons" style="overflow: auto; background-color:#2D2D2D; height:100%; position:relative;">

                </div>
            </div>
        </div>

</div>


