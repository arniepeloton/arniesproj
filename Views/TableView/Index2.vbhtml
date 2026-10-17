@Code
    ViewData("Title") = "Welcome"
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code
<style>
.ui-page-header-fixed {
    padding-top: 0px;
}

.ui-page .ui-body-c .ui-page-footer-fixed .ui-page-header-fixed .ui-page-active
{
    padding-bottom: 0px;
}
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
</style>
@Section head
<link type="text/css" href="/Content/flatui-css-custom.css" rel="Stylesheet" />
<script>
    require([GetPathFormat('/Scripts3/TableView2.js?crc=')]);
</script>

End Section


<div data-role="page" id="TableView">
    <div class="ui-grid-d" id="divPaymentHeader">
        <div class="ui-block-a">
            <div id="btnBack" style="margin: 0px; height: 80px;" data-role="button" data-theme="b" data-corners="false">
                <div style="height: 25px; vertical-align: middle; display: table-cell;"><img alt="home" src="/Content/images/Home.png" width="30px" align="center" /><br /></div>
                <div style="font-size: 13px; margin-top: 10px;">Home</div>
            </div>
        </div>
        <div class="ui-block-b">
            <div name="" style="margin: 0px; height: 80px;" data-role="button" data-theme="b" data-corners="false">
                
            </div>
        </div>
        <div class="ui-block-c">
            <div id="" style="margin: 0px; height: 80px;" data-role="button" data-theme="b" data-corners="false">
                
            </div>
        </div>
        <div class="ui-block-d">
            <div id="" style="margin: 0px; height: 80px;" data-role="button" data-theme="b" data-corners="false">
                
            </div>
        </div>
        <div class="ui-block-e">
            <div id="butViewMode" style="margin: 0px; height: 80px;" data-role="button" data-theme="green" data-corners="false">
                <div style="height: 25px; vertical-align: middle; display: table-cell;">
                    <img alt="More Options" src="/Content/images/View_Mode.png" width="30px" align="center" /><br />
                </div>
                <div style="font-size: 13px; margin-top: 10px;">Table View</div>
            </div>
        </div>
    </div>

    <div data-role="content" data-theme="c">
        <div id="tblTables">
            <div class="ui-grid-c">
                <div class="ui-block-a"><div class="tablestatuscolor1" style="height:60px; margin:5px" name="btnTableNumber"><span id="tbl1"></span></div></div>
                <div class="ui-block-b"><div class="tablestatuscolor2" style="height:60px; margin:5px" name="btnTableNumber"><span id="tbl2"></span></div></div>
                <div class="ui-block-c"><div class="tablestatuscolor3" style="height:60px; margin:5px" name="btnTableNumber"><span id="tbl3"></span></div></div>
                <div class="ui-block-d"><div class="tablestatuscolor4" style="height:60px; margin:5px" name="btnTableNumber"><span id="tbl4"></span></div></div>
            </div>
            <div class="ui-grid-c">
                <div class="ui-block-a"><div class="tablestatuscolor5" style="height:60px; margin:5px" name="btnTableNumber"><span id="tbl5"></span></div></div>
                <div class="ui-block-b"><div class="tablestatuscolor6" style="height:60px; margin:5px" name="btnTableNumber"><span id="tbl6"></span></div></div>
                <div class="ui-block-c"><div class="tablestatuscolor7" style="height:60px; margin:5px" name="btnTableNumber"><span id="tbl7"></span></div></div>
                <div class="ui-block-d"><div class="tablestatuscolor8" style="height:60px; margin:5px" name="btnTableNumber"><span id="tbl8"></span></div></div>
            </div>
            <div class="ui-grid-c">
                <div class="ui-block-a"><div class="tablestatuscolor9" style="height:60px; margin:5px" name="btnTableNumber"><span id="tbl9"></span></div></div>
                <div class="ui-block-b"><div class="tablestatuscolor10" style="height:60px; margin:5px" name="btnTableNumber"><span id="tbl10"></span></div></div>
                <div class="ui-block-c"><div class="tablestatuscolor11" style="height:60px; margin:5px" name="btnTableNumber"><span id="tbl11"></span></div></div>
                <div class="ui-block-d"><div class="tablestatuscolor12" style="height:60px; margin:5px" name="btnTableNumber"><span id="tbl12"></span></div></div>
            </div>
            <div class="ui-grid-c">
                <div class="ui-block-a"><div class="tablestatuscolor13" style="height:60px; margin:5px" name="btnTableNumber"><span id="tbl13"></span></div></div>
                <div class="ui-block-b"><div class="tablestatuscolor14" style="height:60px; margin:5px" name="btnTableNumber"><span id="tbl14"></span></div></div>
                <div class="ui-block-c"><div class="tablestatuscolor15" style="height:60px; margin:5px" name="btnTableNumber"><span id="tbl15"></span></div></div>
                <div class="ui-block-d"><div class="tablestatuscolor16" style="height:60px; margin:5px" name="btnTableNumber"><span id="tbl16"></span></div></div>
            </div>
            <div class="ui-grid-c">
                <div class="ui-block-a"><div class="tablestatuscolor17" style="height:60px; margin:5px" name="btnTableNumber"><span id="tbl17"></span></div></div>
                <div class="ui-block-b"><div class="tablestatuscolor18" style="height:60px; margin:5px" name="btnTableNumber"><span id="tbl18"></span></div></div>
                <div class="ui-block-c"><div class="tablestatuscolor19" style="height:60px; margin:5px" name="btnTableNumber"><span id="tbl19"></span></div></div>
                <div class="ui-block-d"><div class="tablestatuscolor20" style="height:60px; margin:5px" name="btnTableNumber"><span id="tbl20"></span></div></div>
            </div>            
        </div>
        <br /> <br /> 
        <div>  
            <p style="text-align:center; font-weight:bold; font-size:larger">View Tables</p>
        </div>
        <div>  
             <div class="ui-grid-d">
                <div class="ui-block-a"><div name="btnTableMultiplier" id="mtp0">1-20</div></div>
                <div class="ui-block-b"><div name="btnTableMultiplier" id="mtp1">21-40</div></div>
                <div class="ui-block-c"><div name="btnTableMultiplier" id="mtp2">41-60</div></div>
                <div class="ui-block-d"><div name="btnTableMultiplier" id="mtp3">61-80</div></div>
                <div class="ui-block-e"><div name="btnTableMultiplier" id="mtp4">81-100</div></div>
            </div>    
        </div>
    </div>

    <div id="popupTotalNumberOfCustomer" custom-data-role="popup">
        <div class="popup-md">
            <div class="popup-header">
                Guest Count <a data-role="none" class="popup-close">X</a>
            </div>
            <div class="popup-content">
                @*<label for="txtTotalNumberOfCustomer">No. of Guest(Dinners)<span style="color: #FF0000;">*</span></label>*@
                <input type="number" id="txtTotalNumberOfCustomer" maxlength="3" style="text-align: center" value="1" />

                <div id="divProductSalesPersonContainer" >
                    <div>
                        <input id="txtProductSalesRepSearch" type="search" placeholder="Search a Sales Representative" value="" />
                    </div>
                    <div id="divProductSalesPerson">
                        <ul data-role="listview" id="lstSalesRepContainer" data-inset="true">
                            <li data-role="list-divider">Sales Representative List</li>
                            <li>
                                <div>
                                    <ul data-role="listview" id="lstSalesRep" data-inset="true"></ul>
                                </div>
                            </li>
                        </ul>
                    </div>
                    <br />
                </div>

            </div>

            <div class="popup-footer align-right">

                <button id="btnSaveTotalNumberOfCustomer" type="submit">Save</button>
            </div>
        </div>
    </div>

    @*<div data-role="popup" id="popupTotalNumberOfCustomer" class="ui-corner-all" style="padding:20px;">

        <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
        <center><h3>Guest Count</h3></center>

        <div data-role="content" style="text-align: center; ">
            <label for="txtTotalNumberOfCustomer">No. of Guest(Dinners)<span style="color: #FF0000;">*</span></label>
            <input type="number" id="txtTotalNumberOfCustomer" maxlength="3" style="text-align: center" value="1" />
        </div>

        <button id="btnSaveTotalNumberOfCustomer" type="submit">Save</button>

    </div>*@
</div>
