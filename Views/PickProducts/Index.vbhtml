@Code
    ViewData("Title") = "Pick Products"
    Layout = "~/Views/Shared/Plain.vbhtml"
End Code
@Section head
    <script>
        require([GetPathFormat('/Scripts/sources/PickProducts/PickProducts.js?crc=')]);
    </script> 
End Section
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


</style>
      <div data-role="header" data-position="fixed1" style="background: none white; border: none">
        <div class="ui-grid-c" >
            <div class="ui-block-a">
                <div id="btnHome" style="margin:0px; height: 80px;" data-role="button" data-theme="b" data-corners="false" class="HotButtons">
                <span style="margin-top:5px;">Home</span>
                </div>
            </div>
            <div class="ui-block-b">
                <div id="btnFowst" style="margin:0px; height: 80px;" data-role="button" data-theme="b" data-corners="false">
                    FOWST:<br />
                    <span id="lnkPosBranch"></span>
                </div>
            </div>
            <div class="ui-block-c">
                <div id="btnViewHistory" style="margin:0px; height: 80px;" data-role="button" data-theme="b" data-corners="false">
                    View <br />Ready for Invoicing
                </div>
            </div>
            <div class="ui-block-d">
                <div id="btnRefresh"style="margin:0px; height: 80px;" data-role="button" data-theme="b" data-corners="false" class="HotButtons">
                    Refresh
                </div>
            </div>
        </div>
        <div>
            <div style="margin:0px;" data-role="button" data-theme="b" data-corners="false">
                <span id="clock"></span>
            </div>
        </div>
   </div>

<div data-role="content">
    <div style="text-align: center;padding: 20px">
        <label style="font-size: 48px;">You have <label id="lblPendingOrders">0</label> pending <label id="lblOrderTxt">orders</label></label>
        <br /><br />
    </div>
            <div style="text-align: center">
                <a id="btnStart" data-role="button" style="display:none">
                <h3>
                   Start</h3>
                </a>
          
            </div>

      
</div>

<br /><br />

@Section OtherPages
    <div id="pageBin" data-role="page">
        <div data-role="header">
            <a href="#" data-icon="arrow-l" id="lnkPageBinBack">Back</a>
            <h1>
                Bin List</h1>
        </div>
      
        <div data-role="content">
            <div>
    
                <h2 id="hdrProductName"></h2>
                 <h3>Qty Ordered&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;: <label id="lblQtyOrdered"></label></h3>
                 <h3>Total Qty Picked : <label id="lblQtyPicked"></label></h3>
                 <h3 id ="lblRemarksReplacement"></h3>
             </div >
         
            <div >
                <input id="txtBinName" type="search" value="" placeholder="Enter bin name" />
@*                 <button id="btnAddBin">Add Bin</button>*@
                <button id="butBinName">Scan Bin QR Code</button>
                <ul data-role="listview" id="lstBinContainer" data-inset="true">   
			        <li data-role="list-divider">Bin List</li>
                    <li>
                    
                        @*<div data-role="fieldcontain"><a id="btnAddOrgFOWSTMobile" data-role="button" data-icon="plus" data-inline="true">Add Basket</a></div>*@
                        <div>
                            <ul data-role="listview" id="lstBin" data-inset="true">
                            </ul>
                        </div>
                    </li>
                </ul>
            </div>  
           
                <div>
                      <a id="btnBinListDone" data-role="button">
                        <h3>Done</h3>
                        </a>
                </div>
                <div>
                     <a id="btnImNotDone" data-role="button">
                    <h3>Not Yet Done</h3>
                    </a>
                </div>

          
 
        </div>


        @*<div data-role="collapsible" data-collapsed="false" data-theme="c" data-content-theme="d"
            data-collapsed-icon="arrow-r" data-expanded-icon="arrow-d" data-inset="true"  style="padding: 20px;">
            <h3>Replace Item</h3>
     
            <div>
                <input id="txtAlternativeItemSearch" type="search" placeholder="Search for an alternative item" value="" />
            </div>
            <div id="divAlternativeItem">
                <ul data-role="listview" id="lstAlternativeProductContainer" data-inset="true">
                    <li data-role="list-divider">Alternative Item List</li>
                    <li>
                        <div>
                            <ul data-role="listview" id="lstAlternativeItem" data-inset="true">
                            </ul>
                        </div>
                    </li>
                </ul>
            </div>
              
        </div>*@

        
        <br />
        <br />
        
    </div>
    <div id="pageScanBasket" data-role="page">
        <div data-role="header">
            <a href="#" data-icon="arrow-l" data-rel="back">Back</a>
            <h1>
                Scan Basket</h1>
        </div>
        <div data-role="content">
            <div id="divBasket">
                <input id="txtBasketName" value="" />
                 <button id="btnAddBasket">Add Basket</button>
                <button id="butBasketName">Scan Barcode</button>
                <ul data-role="listview" id="lstBasketContainer" data-inset="true">   
			        <li data-role="list-divider">Basket List</li>
                    <li>
                    
                        @*<div data-role="fieldcontain"><a id="btnAddOrgFOWSTMobile" data-role="button" data-icon="plus" data-inline="true">Add Basket</a></div>*@
                        <div>
                            <ul data-role="listview" id="lstBasket" data-inset="true">
                            </ul>
                        </div>
                    </li>
                </ul>
            </div>  

        </div>

    </div>
    <div id="pageSalesOrder" data-role="page">
        <div data-role="header">
            <a href="#" data-icon="arrow-l" id="lnkSalesOrderBack">Back</a>
            <h1>
                Pick Products</h1>
        </div>
        <div data-role="content">
            <div id="divSalesOrderProcessing">
            </div>
        </div>

    </div>

End Section
 
