<style>
    /*Green button*/
    .ui-btn-up-green {
        border: 1px solid #3A7214;
        background: #5BAB25;
        color: #fff;
        text-shadow: 0 -1px 1px #000;
        background-image: -moz-linear-gradient(top, #7EC54E, #41AB25);
        background-image: -webkit-gradient(linear,left top,left bottom, color-stop(0, #7EC54E), color-stop(1, #41AB25));
        -ms-filter: "progid:DXImageTransform.Microsoft.gradient(startColorStr='#7EC54E', EndColorStr='#41AB25')";
    }

        .ui-btn-up-green a.ui-link-inherit {
            color: #fff;
        }

    .ui-btn-hover-green {
        border: 1px solid #176E00;
        background: #6EB64B;
        color: #fff;
        text-shadow: 0 -1px 1px #000;
        background-image: -moz-linear-gradient(top, #99D472, #6EB64B);
        background-image: -webkit-gradient(linear,left top,left bottom, color-stop(0, #99D472), color-stop(1, #6EB64B));
        -ms-filter: "progid:DXImageTransform.Microsoft.gradient(startColorStr='#99D472', EndColorStr='#6EB64B')";
    }

        .ui-btn-hover-green a.ui-link-inherit {
            color: #fff;
        }

    .ui-btn-down-green {
        border: 1px solid #347722;
        background: #7EC54E;
        color: #fff;
        text-shadow: 0 -1px 1px #000;
        background-image: -moz-linear-gradient(top, #569E39, #7EC54E);
        background-image: -webkit-gradient(linear,left top,left bottom, color-stop(0, #569E39), color-stop(1, #7EC54E));
        -ms-filter: "progid:DXImageTransform.Microsoft.gradient(startColorStr='#569E39', EndColorStr='#7EC54E')";
    }

        .ui-btn-down-green a.ui-link-inherit {
            color: #fff;
        }

    .ui-btn-up-green,
    .ui-btn-hover-green,
    .ui-btn-down-green {
        font-family: Helvetica, Arial, sans-serif;
        text-decoration: none;
    }

    /*Gray button*/
    .ui-btn-up-gray {
        border: 1px solid #595A57;
        background: #8F908F;
        color: #fff;
        text-shadow: 0 -1px 1px #000;
        background-image: -moz-linear-gradient(top, #AAABAA, #8D8E8D);
        background-image: -webkit-gradient(linear,left top,left bottom, color-stop(0, #AAABAA), color-stop(1, #8D8E8D));
        -ms-filter: "progid:DXImageTransform.Microsoft.gradient(startColorStr='#AAABAA', EndColorStr='#8D8E8D')";
    }

        .ui-btn-up-gray a.ui-link-inherit {
            color: #fff;
        }

    .ui-btn-hover-gray {
        border: 1px solid #646464;
        background: #A1A0A0;
        color: #fff;
        text-shadow: 0 -1px 1px #000;
        background-image: -moz-linear-gradient(top, #B5B8B5, #A1A0A0);
        background-image: -webkit-gradient(linear,left top,left bottom, color-stop(0, #B5B8B5), color-stop(1, #A1A0A0));
        -ms-filter: "progid:DXImageTransform.Microsoft.gradient(startColorStr='#B5B8B5', EndColorStr='#A1A0A0')";
    }

        .ui-btn-hover-gray a.ui-link-inherit {
            color: #fff;
        }

    .ui-btn-down-gray {
        border: 1px solid #777A77;
        background: #AAABAA;
        color: #fff;
        text-shadow: 0 -1px 1px #000;
        background-image: -moz-linear-gradient(top, #999B99, #AAABAA);
        background-image: -webkit-gradient(linear,left top,left bottom, color-stop(0, #999B99), color-stop(1, #AAABAA));
        -ms-filter: "progid:DXImageTransform.Microsoft.gradient(startColorStr='#999B99', EndColorStr='#AAABAA')";
    }

        .ui-btn-down-gray a.ui-link-inherit {
            color: #fff;
        }

    .ui-btn-up-gray,
    .ui-btn-hover-gray,
    .ui-btn-down-gray {
        font-family: Helvetica, Arial, sans-serif;
        text-decoration: none;
    }

    /* Button */
    .HotButtons .ui-btn-inner {        
        border-top:0px !important;
        border-color:none !important;
    }

    .HotButtonsText .ui-btn-text {
        color: White;
    }

    .detailimage3 {
        height: 45px;
        text-align: center;
        margin: 0;
    }

    .detailimage3 img {
        width: 100%;
    }    
</style>
<script>
    require([GetPathFormat('/Scripts3/Bs_HotButtons.js?crc=')]);
</script>

<div id="divHButtonDIV">
    <div class="input-group">
        <input data-role="none" id="txtSearch" class="form-control rounded-0" placeholder="Search Products" autocomplete="off" />
        <button id="butBarCode" title="Barcode Scanner" data-role="none" class="btn btn-secondary divHideBarcodeScan rounded-0"><span class="bi-upc-scan"></span></button>
    </div>
    <div class="row m-0" style="background-color:#FFF;">
        <div class="col-3 p-0">
            <button data-role="none" id="btnHotButtonMain" class="btn btn-outline-success active btn-block rounded-0 hb-navbtn">
                <span id="spanHotButtonMain"></span>
            </button>
        </div>
        <div class="col-3 p-0">
            <button data-role="none" id="btnHotButtonGroup1" class="btn btn-outline-success btn-block rounded-0 no-hover hb-navbtn">
                <span id="spanHotButtonGroup1"></span>
            </button>
        </div>
        <div class="col-3 p-0">
            <button data-role="none" id="btnHotButtonGroup2" class="btn btn-outline-success btn-block rounded-0 no-hover hb-navbtn">
                <span id="spanHotButtonGroup2"></span>
            </button>
        </div>
        <div class="col-3 p-0">
            <button data-role="none" id="btnHotButtonGroup3" class="btn btn-outline-success btn-block rounded-0 no-hover hb-navbtn">
                <span id="spanHotButtonGroup3"></span>
            </button>
        </div>
    </div>

    <div id="divHotButtonLoading" class="mt-3 mb-3" style="width:100%; text-align:center;">
        <div class="spinner-border text-light" role="status">
            <span class="visually-hidden"></span>
        </div>
    </div>

    <div name="divHotButtonsRow" id="divHotButtonsTemplate" class="d-none">
        <div class="row m-0">
            <div style="min-height:80px;" class="col-4 bg-dark p-0 border border-dark darker">
                <button data-role="none" name="btnHotButton0" class="btn btn-dark btn-block rounded-0 hb-navbtn" style="height:100%;">
                    <div name="btnHotButtonItem0" style="word-break:break-word;"></div>
                </button>
            </div>
            <div style="min-height:80px;" class="col-4 bg-dark p-0 border border-dark darker">
                <button data-role="none" name="btnHotButton1" class="btn btn-dark btn-block rounded-0 hb-navbtn" style="height:100%;">
                    <div name="btnHotButtonItem1" style="word-break:break-word;"></div>
                </button>
            </div>
            <div style="min-height:80px;" class="col-4 bg-dark p-0 border border-dark darker">
                <button data-role="none" name="btnHotButton2" class="btn btn-dark btn-block rounded-0 hb-navbtn" style="height:100%;">
                    <div name="btnHotButtonItem2" style="word-break:break-word;"></div>
                </button>
            </div>
        </div>
    </div>

    <div id="divHotButtonsSection">
        <div name="divHotButtonsRowNext">
        </div>
    </div> 

    <br />
    <div data-role="page" id="pageTemplate" style="display:none;">
        <div data-role="content">

            <div>
                <ul data-role="listview" id="lstOrgHotButtonContainer" data-inset="true">
                    <li data-role="list-divider">Select a Template </li>
                    <li>
                        <form id="frmHotButtonSearch">
                            <div><input id="txtHotButtonSearch" type="search" value="" /></div>
                        </form>

                        <div data-role="fieldcontain" id="divHotButtonTemplate"></div>
                        <div>
                            <ul data-role="listview" id="lstHotButtonTemplate" data-inset="true"></ul>
                        </div>
                    </li>
                </ul>
            </div>

            <div>
                <br />
                <div style="text-align:center"> <a id="btnHotButtonsBack" data-role="button">Back</a> </div>
            </div>

        </div>
    </div>
    <div data-role="popup" id="popupPOSMode" data-theme="b" class="ui-corner-all" style="display:none;">
        <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
        <center><h3>POS Mode</h3></center>
        <div data-role="content">
            <div id="divPOSMode">
            </div>
        </div>
    </div>

    <div data-role="popup" id="popupMoreOptions" data-theme="b" class="ui-corner-all" style="width:auto; display:none">
        <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
        <center><h3>More Options</h3></center>
        <div data-role="content">

            <div style="text-align: center">
                <a id="btnFowst" data-role="button" data-theme="e">
                    <h3>Current FOWST Location</h3>
                    <p style="font-weight: normal; margin-top: -15px;">
                        <label id="lnkPosBranchHotButtons" style="cursor:pointer;"></label>
                        <br />
                        Sales, inventory movements and payments will be tagged to this location.
                    </p>
                </a>
            </div>
            <div style="text-align: center">
                <a id="butNewSalesOrder" data-role="button" data-theme="e">
                    <h3>New Cart</h3>
                    <p style="font-weight: normal; margin-top: -15px;">
                        Puts the current cart on hold and displays a new one for the next customer.
                    </p>
                </a>
            </div>
            <div style="text-align: center">
                <a id="lnkSalesOrder" data-role="button" data-theme="e">
                    <h3>Recall Cart</h3>
                    <p style="font-weight: normal; margin-top: -15px;">
                        Used to retrieve carts that were placed on hold.
                    </p>
                </a>
            </div>
            <div style="text-align: center">
                <div style="font-weight: bold; border-radius: 1em; box-shadow: 0px 1px 3px rgba(0, 0, 0, 0.2); border: 1px solid #F4C63F;background: #FFE155; padding: 10px;">

                    <h3>Price Level</h3>
                    <select id="selPriceLevel"></select>
                </div>
            </div>
            <div style="text-align: center" id="divChangeMenuButtons" style="display:none;">
                <a id="btnChangeMenuButtons" data-role="button" data-theme="e">
                    <h3>Change Hot Button Template</h3>
                    <p style="font-weight: normal; margin-top: -15px;">
                        Lets you use a different set of hot buttons. Administrators <br />can customize these buttons online.
                    </p>
                </a>
            </div>


            @*                <div style="text-align: center">
                    <a id="btnTagAsVIPOrder" data-role="button" data-theme="e">
                    <h3>Tag as VIP Order</h3>
                    <p style="font-weight: normal; margin-top: -15px;">
                    For VIP Customers
                    </p>
                    </a>
                </div>*@
            @*                <div style="text-align: center">
                    <div style="font-weight: bold; border-radius: 1em; box-shadow: 0px 1px 3px rgba(0, 0, 0, 0.2); border: 1px solid #F4C63F;background: #FFE155; padding: 10px;">

                        <h3>Payment Mode</h3>
                        <select id="selPaymentMode">
                        </select>
                    </div>
                </div>*@

        </div>

    </div>

    <div data-role="popup" id="popupDecimalInput" class="ui-corner-all" style="padding: 20px; display:none;">
        <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
        <center><h3 id="hdrQty"></h3></center>
        <div data-role="content">
            <div id="divQty" style="text-align: center; font-weight: bold;">
                <label id="lblBOMQty" style="display:none">How many would you like to make?</label>
                <label id="lblOrdinaryQty" style="">Quantity</label>
                <input id="txtDecimalQty" type="text" maxlength="15" style="text-align: center; " placeholder="Enter quantity" />
                <span id="spanErrorQty" style="display:none; color: red; font-size:small;">Please enter a valid quantity</span>
                <button id="btnSaveQty" type="submit">Ok</button>

                <br />
            </div>
        </div>
    </div>


</div>