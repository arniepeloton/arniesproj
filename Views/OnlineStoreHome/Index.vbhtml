@Code
    ViewData("Title") = "Welcome"
    Layout = "~/Views/Shared/Plain.vbhtml"
End Code

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
                @*background: #e3e3e3;*@
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

    </style>


    <script>
        require([GetPathFormat('/Scripts/sources/OnlineStoreHome/OnlineStoreHome.js?crc=')]);
    </script>

    <div id="pagePeopleMenu" data-role="header" data-position="not-fixed" style="background: none white;
        border: none;padding: 15px 15px 0px 15px;">
        <div>
            <a type="button" id="btnOnlineStorePopUpMenu" data-rel="popup" data-icon="grid" data-inline="true"
                style="float: right;margin:0px;">Menu</a>
        </div> 
        <div class="ui-grid-a">
            <div class="ui-block-a" style="width: 125px;">
                <a id="btnPeopleMenuPeopleImage" style="cursor: pointer;">
                    <img src="/Content/images/NoPhoto.jpg" />
                </a>
            </div>
            <div class="ui-block-b" id="photoGrid">
                <div>
                    <a id="lblPersonName" style="cursor: pointer;">Name Goes Here...</a>                                
                </div>
                <div>
@*                <label id="lblEmployer">
                        Currently employed at...</label></div>*@
            </div>
        </div>
    </div>
    <br />
    <div data-role="header" data-position="fixed1" style="background: none white; border: none">
        <div class="ui-grid-d" >
            <div class="ui-block-a" style="width: 25%;">
                <div id="btnMyCompanyList" style="margin:0px; height: 80px;" data-role="button" data-theme="b" data-corners="false" class="HotButtons">
                <span style="margin-top:5px;">My Company List</span>
                </div>
            </div>
            <div class="ui-block-b" style="width: 25%;">
                <div id="btnMyList" style="margin:0px; height: 80px;" data-role="button" data-theme="b" data-corners="false" class="HotButtons">
                    <span style="margin-top:5px;">My List</span>
                </div>
            </div>
            <div class="ui-block-c" style="width: 25%;">
                <div id="btnOrders" style="margin:0px; height: 80px;" data-role="button" data-theme="b" data-corners="false" class="HotButtons">
                    <span style="margin-top:5px;">Orders</span>
                </div>
            </div>
            <div class="ui-block-e" style="width: 25%;">
                <div id="lnkCheckout"style="margin:0px; height: 80px;" data-role="button" data-theme="green" data-corners="false">
                    <span id="lblPayNow">Save <br />Order</span>
                </div>
            </div>
        </div>
   </div>

    <div data-role="content" style="padding:5px;">
 
        <div id="divOnlineStoreMenuPopUp" data-role="popup">
            <ul data-role="listview" data-inset="true" style="min-width: 210px;">
                <li data-role="divider">Menu</li>
                <li><a id="popOnlineStoreProfile">Profile</a></li>
                <li><a id="popOnlineStoreSettings">Settings</a></li>
                <li><a id="btnOnlineStoreSignOut">Sign Out</a></li>
            </ul>
        </div>

        <div class="ui-grid-a" style="margin:0px; padding: 0px;">
            <div class="ui-block-a" style="width:78%;height: 50px;padding: 0px;margin-bottom:5px;">
                <form id="formSearchProduct">
                    <input id="txtSearchProduct" type="search" placeholder="Search products" class="SearchText" style="height: 40px;padding:0px;" autofocus />
            
                </form>

            </div>
            <div class="ui-block-b" style="width: 22%;height: 50px;padding: 0px;margin:0px;">
                <center><div >
                    <img src="/Content/images/barcode96x96.png" class="detailimage2" id="butProductBarCode" data-role="button" data-corners="false"/>
                </div>
                </center>
            </div>
        </div>
 
    </div>

   <br />
   <br />
   <br />
