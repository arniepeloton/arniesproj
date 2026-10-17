@Code
    ViewData("Title") = "Welcome"
    Layout = "~/Views/Shared/Plain.vbhtml"
End Code
<link rel="stylesheet" href="/Content/flatui-css-custom.css" />
<style>
    /*Green button*/
    .ui-btn-up-green {
        border: 1px solid #3A7214;
        background: #5BAB25;
        color: #fff;
        text-shadow: 0 -1px 1px #000;
        background-image: -moz-linear-gradient(top, #7EC54E, #41AB25);
        background-image: -webkit-gradient(linear,left top,left bottom,
            color-stop(0, #7EC54E),
            color-stop(1, #41AB25));
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
        background-image: -moz-linear-gradient(top, 
            #99D472, 
            #6EB64B);
        background-image: -webkit-gradient(linear,left top,left bottom,
            color-stop(0, #99D472),
            color-stop(1, #6EB64B));
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
        background-image: -webkit-gradient(linear,left top,left bottom,
            color-stop(0, #569E39),
            color-stop(1, #7EC54E));
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
        background-image: -webkit-gradient(linear,left top,left bottom,
            color-stop(0, #AAABAA),
            color-stop(1, #8D8E8D));
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
        background-image: -moz-linear-gradient(top, 
            #B5B8B5, 
            #A1A0A0);
        background-image: -webkit-gradient(linear,left top,left bottom,
            color-stop(0, #B5B8B5),
            color-stop(1, #A1A0A0));
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
        background-image: -webkit-gradient(linear,left top,left bottom,
            color-stop(0, #999B99),
            color-stop(1, #AAABAA));
        -ms-filter: "progid:DXImageTransform.Microsoft.gradient(startColorStr='#999B99', EndColorStr='#AAABAA')";
    }
    .ui-btn-down-gray a.ui-link-inherit {
        color: #fff;
    }
    .ui-btn-up-gray,
    .ui-btn-hover-gray,
    .ui-btn-down-gry {
        font-family: Helvetica, Arial, sans-serif;
        text-decoration: none;
    }
    
    /* Button */
    .HotButtons .ui-btn-inner {
        
        padding-top: 1.4em;
    }
    
    
    .HotButtonsText .ui-btn-text {
        color:White;   
    }
    

    #spanName
        {
            font-weight: bold;
            font-style: italic;
        }
        .ScrollWrapper
        {
            overflow: hidden;
            z-index: 1;
            position: relative;
            height: 240px;
            -webkit-border-radius: 10px;
            -moz-border-radius: 10px;
            -o-border-radius: 10px;
            border-radius: 10px;
        }
        .ScrollWrapper > div
        {
            width: 1650px;
            height: 100%;
            padding: 0;
            position: absolute;
            z-index: 1;
        }
        .ScrollWrapper > div > div
        {
            float: left;
            margin: 5px 0px 5px 5px;
            width: 150px;
            text-align: center;
        }
        .ScrollWrapper > div > div:last-child
        {
            margin: 5px;
        }
        .ScrollWrapper img
        {
            width: 144px;
            height: 144px;
        }
        .ScrollWrapper > div > div h3
        {
            margin: 0px;
        }
        .SectionHeader h2 {
            margin-top: 1px;
            margin-bottom: 1px;
            color: #bbbbbb;
        }
        .SectionHeader > div:nth-child(2) {
            text-align: right;
        }
        .SectionHeader a {
            text-decoration: none;
        }
        .Empty {
            padding: 10px;
            text-align: center;
        }

        .content_div {
            display: none;
            background-color:white;
            padding-top:10px;
        }
        .content_div:first-child {
            display: block;
        }

        table.myTable { 
            border-collapse:collapse; 
        }
        .ui-table-reflow.ui-responsive {
            display: table;
        }
        table.myTable th {
	        border-width: 1px;
	        padding: 8px;
	        border-style: solid;
	        border-color: #666666;
	        background-color: #dedede;
        }
        table.myTable td {
	        border-width: 1px;
	        padding: 8px;
	        border-style: solid;
	        border-color: #666666;
	        background-color: #ffffff;
        }

        .tableUnits td
    {
        padding-right: 10px;
        text-align: right;
    }


    .SearchText {
        margin:0px;
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
        box-shadow:none !important;
    }
     .ui-page-active{
      padding-top:0px;
     }
     .detailimage {
        width: 80%;
        text-align: center;
        margin: 0;
        padding-top:0px;
    }

    .detailimage img {
        width: 100%;
    }
    .detailimage2 {

        text-align: center;
        margin: 0;

    }

    .detailimage2 img {
        width: 100%;
    }

    .detailimage3 img {
        width: 99%;
    }

</style>

<script>
    require([GetPathFormat('/Scripts3/WarehouseReleasingTracker.js?crc=')]);
</script>

<div data-role="header" data-position="fixed1" style="background: none white; border: none">
    <div class="ui-grid-a" >
        <div class="ui-block-a" style="width: 12%;">
            <div id="btnHome" style="margin:0px; height: 80px;" data-role="button" data-theme="b" data-corners="false" class="HotButtons">
            <span style="margin-top:5px;">Home</span>
            </div>
        </div>
        <div class="ui-block-b" style="width: 22%;">
            <div id="btnCustomer" style="margin:0px; height: 80px;" data-role="button" data-theme="b" data-corners="false">
                <span id="lblLabel">Customer:<br /></span>
                <span id="lblCustomerName"></span>
            </div>
        </div>
        <div class="ui-block-c" style="width: 22%;">
            <div id="btnPOSMode" style="margin:0px; height: 80px;" data-role="button" data-theme="b" data-corners="false">
                <span id="IsPOSHeader">POS Mode:<br /></span>
                <span id="lblCurrentPOSMode"></span>
            </div>
        </div>
        <div class="ui-block-d" style="width: 22%;">
            <div id="btnMoreOptions" style="margin:0px; height: 80px;" data-role="button" data-theme="b" data-corners="false">
                <span>More <br />Options</span>
            </div>
        </div>
        <div class="ui-block-e" style="width: 22%;">
            <div id="lnkCheckout"style="margin:0px; height: 80px;" data-role="button" data-theme="green" data-corners="false">
                <span id="lblPayNow">Pay <br />Now</span>
            </div>
        </div>
    </div>
    <div>
        <div style="margin:0px;" data-role="button" data-theme="b" data-corners="false">
            <span id="clock"></span>
        </div>
    </div>
</div>


<div data-role="popup" id="popupMessage" data-dismissible="false" data-theme="b" class="ui-corner-all" style="width:auto; padding: 0.5em;">
	<center><h3 id="popupMessageHeader"></h3></center>
    <div data-role="content">
        <div id="refNum" class="ui-grid-a" style="margin:0px; padding: 0px;width: inherit;">
        </div>
        <div class="ui-grid-b" style="margin:0px; padding: 0px;width: inherit;">
            <div class="ui-block-a" style="width: 35%;">
                <p style="font-weight: bold;">Released To: </p>
            </div>
            <div class="ui-block-b" style="width: 65%;">
                <p id="releasedTo" style="font-weight: normal;"></p>
            </div>
        </div>
        <div class="ui-grid-c" style="margin:0px; padding: 0px;width: inherit;">
            <div class="ui-block-a" style="width: 35%;">
                <p style="font-weight: bold;">On: </p>
            </div>
            <div class="ui-block-b" style="width: 65%;">
                <p id="releasedOn" style="font-weight: normal;"></p>
            </div>
        </div>
        <div class="ui-grid-d" style="margin:0px; padding: 0px;width: inherit;">
            <div class="ui-block-a" style="width: 35%;">
                <p style="font-weight: bold;">By: </p>
            </div>
            <div class="ui-block-b" style="width: 65%;">
                <p id="releasedBy" style="font-weight: normal;width: inherit;"></p>
            </div>
        </div>
        <div style="width: inherit;">
            <br />
            <div id="errorNotice" style="display: none">
                <div style="border-style: solid; text-align:center">
                    <p style="font-weight:bold; padding: 0.5em;">
                        PLEASE DO NOT RELEASE STOCKS ASSOCIATED WITH THIS<br />
                        DOCUMENT TO PREVENT DOUBLE-RELEASING AND<br />
                        INVENTORY SHRINKAGE
                    </p>
                </div>
            </div>
            <br />
            <button id="btnSuccess" data-role="none" class="btn btn-primary">Ok</button>
        </div>
    </div>
</div>

<div data-role="content" id="divReleasingPanel">
    <h1 id="divTitle">
        Warehouse Releasing Tracker
    </h1>
    <div style="width: 50%; display: none;">
            <label for="releaseDate">
                Release Date:
            </label>
            <select id="releaseDate" class="ui-disabled">
                <option id="releaseClock"></option>
            </select>
    </div>
    <div style="width: 50%">
        <form id="personReleaseSearch">
            <label for="personSearch">
                Release stocks to:
            </label>
            <input id="personSearch" type="search" placeholder="Search person or customer" class="SearchText" style="height: 40px;padding:0px;" autofocus />
        </form>
        <div id="personFreeType" style="display: none;">
            <label for="personName">
                Manually type the person or customer's name whom you are releasing to:
            </label>
            <input type="text" name="personName" id="personName" placeholder="Person or customer's name'"/>
        </div>
    </div>
    <br />
    <br />
    <div style="width: 50%">
        <div class="ui-block-a" style="width: 85%">
            <form id="documentReleaseSearch">
                <label for="documentSearch">
                    Releasing Document:
                </label>
                <input id="documentSearch" type="search" placeholder="Search document" class="SearchText" style="height: 40px; padding:0px;" autofocus readonly />
            </form>
            <label id="lblManual" style="width: 35%; left: 1em; display: none;">
                <input style="display: none;"  type="checkbox" name="manualBarcode" id="chkManual">Manual input
            </label>
            <br />
            <a id="btnSubmitWrt" style="text-align:center;" data-role="button">Submit</a>
        </div>
        <div class="ui-block-b" style="width: 15%; padding: 0px;margin:0px;">
            <center>
                <div>
                    <img src="/Content/images/barcode96x96.png" style="top:1em; height: 50px;" class="detailimage2" id="butBarCodeWrt" data-role="button" data-corners="false" />
                </div>
            </center>
        </div>
    </div>
</div>

<div data-role="content" id="divRecordsPanel" style="display:none;">
    <h1 id="divTitle">
        Warehouse Releasing Tracker
    </h1>
    <form id="documentRecordsSearch" style="width: 50%">
        <label for="recordsSearch">
            Releasing Document:
        </label>
        <input id="recordsSearch" type="search" placeholder="Search records" class="SearchText" style="height: 40px; padding:0px;" autofocus />
    </form>
    <br />
    <br />
    <table data-role="table" data-mode="reflow" style="margin-right:5px; display:table;" class="ui-table ui-shadow ui-responsive table-stroke table-stripe">
        <thead>
            <tr id="tblHeader" class="custom-theme-f txt-center">
                <th>Release Date and Time</th>
                <th>Releasing Document</th>
                <th>Customer Name</th>
                <th>Released To</th>
            </tr>
        </thead>
        <tbody id="tblBody"></tbody>
    </table>
</div>