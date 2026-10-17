@Code
    ViewData("Title") = "Dashboard Settings"
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head

    <script>
        require([GetPathFormat('/Scripts/sources/DashBoardsAndReports/OffTake.js?crc=')]);        
    </script>
    
    <style>
    
    table.myTable { 
            border-collapse:collapse; 
            width: 100%;
        }
        .ui-table-reflow.ui-responsive {
            display: table;
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
    /* Add alternating row stripes */

    .table-stripe tbody tr:hover {
      background-color: #E6E6E6; /* non-RGBA fallback  */

    }
    /* Add stroke to the header and last item */
    .table-stripe thead th,
    .table-stripe tbody tr:hover {
      background-color: #E6E6E6; /* non-RGBA fallback  */

    }
    
    /*Ui Dialog*/
    .ui-dialog-contain 
    {
        max-width: 700px;
    }
    
    .ui-dialog-background {
        opacity: 0.5;
        display: block !important;
        -webkit-transition: opacity 0.5s ease-in;
    }

    .ui-dialog-background.pop.in {
        opacity: 1;
        -webkit-transition: opacity 0.5s ease-in;
    }

    .ui-dialog {
        min-height: 100% !important;
        background: transparent !important;
    }
    
    /*jQPlot*/
     .jqplot-xaxis-hide { 
            display: none
        }
         
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
    

</style>
    
End Section

<div data-role="page">
    

    <div data-role="header" data-position="fixed" style="background: none white; border: none">
        <div class="ui-grid-c" >    
            <div class="ui-block-a">
                <div id="btnHome" style="margin:0px; height: 80px;" data-role="button" data-theme="b" data-corners="false" class="HotButtons">
                    <span>Home</span> <br />                    
                </div>
            </div>            
            <div class="ui-block-b">
                <div id="btnPeriod" style="margin:0px; height: 80px;" data-role="button" data-theme="b" data-corners="false" class="HotButtons">
                    <span>Period</span> <br />
                    <span id="lblPeriod"></span>
                </div>
            </div>
            @*<div class="ui-block-c">
                <div id="btnGraphType" style="margin:0px; height: 80px;" data-role="button" data-theme="b" data-corners="false" class="HotButtons">
                    <span>Graph Type</span> <br />                    
                    <span id="lblGraphTypeDetail"></span>
                    <span id="lblGraphType"></span><br />
                </div>
            </div>*@
            <div class="ui-block-c">
                <div id="btnFOWST" style="margin:0px; height: 80px;" data-role="button" data-theme="b" data-corners="false" class="HotButtons">
                    <span id="btnFOWSTTeamLabel">FOWST</span> <br />
                    <span id="lblFOWST"></span>
                </div>
            </div>            
            <div class="ui-block-d">
                <div id="btnGo"style="margin:0px; height: 80px;" data-role="button" data-theme="green" data-corners="false" class="HotButtons">
                    <span>GO</span>
                </div>
            </div>       
        </div>
    </div>

    <div data-role="content">        
        
        <div data-role="popup" id="panDates" data-theme="b" class="ui-corner-all"> 
            <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
            <center><h3>Period</h3></center>
            <div data-role="content">
                <form>
                    <select id="selPeriod">  
                        <option> Select Period </option>
                        <option value="2DaysAgo"> 2 Days Ago </option>
                        <option value="Yesterday"> Yesterday </option>
                        <option value="Today" selected="selected"> Today </option>
                        <option value="WeekToDate"> Week To Date </option>
                        <option value="MonthToDate"> Month To Date </option>
                        <option value="CustomPeriod">Custom Period </option>
                    </select>

                    <ul data-role="listview" data-inset="true" data-theme="e">
                        @*<li id="li2DaysAgo"><a href="#">2 Days Ago</a></li>
                        <li id="liYesterday"><a href="#">Yesterday</a></li>
                        <li id="liToday"><a href="#">Today</a></li>
                        <li id="liWeekToDate"><a href="#">Week To Date</a></li>                        
                        <li id="liMonthToDate"><a href="#">Month To Date</a></li>
                        <li id="liCustomPeriod"><a href="">Custom Period</a></li>*@
                    </ul>
                </form>
            </div>       
        </div>

        <div data-role="popup" id="panCustomPeriod" data-theme="b" class="ui-corner-all"> 
            <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
            <center><h3>Period</h3></center>
            <div data-role="content">
                <form>
                    <fieldset data-role="controlgroup" data-type="horizontal">
                        <legend><b>From: </b></legend>
                        <fieldset id="txtDateFrom" data-role="DateCombo" data-usecurrent="1"> </fieldset>
                    </fieldset>
            
                    <fieldset data-role="controlgroup" data-type="horizontal">
                        <legend><b>To: </b></legend>
                        <fieldset id="txtDateTo" data-role="DateCombo" data-usecurrent="1"> </fieldset>
                    </fieldset>

                    <a id="btnDoneCustomPeriod" data-role="button" href="#" data-theme="e">Done</a>
                </form>
            </div>       
        </div>

        <div data-role="popup" id="panFOWST" data-theme="b" class="ui-corner-all"> 
            <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
            <center><h3>FOWST</h3></center>
            <div data-role="content">
                <form>
                    <table id="tblFOWST" style="width:100%">                    
                            </table>    
                </form>
            </div>       
        </div>

        <div data-role="popup" id="panSettings" data-theme="b" class="ui-corner-all"> 
            <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
            <center><h3>Graph Type</h3></center>
            <div data-role="content">
                <form>
                    <select id="selSettingsDetails">
                        <option value="optSalesTrendAndDistribution">By Units</option>
                    </select>
                    <br />
                    <a href="#" data-role="button" id="btnDoneSettings" data-theme="e">Done</a>
                </form>
            </div>       
        </div> 

@* Body Design *@
        
        <div>
            <ul data-role="listview" data-inset="true">
                <li data-role="list-divider" id="liMainChart">Product OffTake</li>
                <li>
                    <div style="text-align:center">
                        <label id="IsDataPresent"> -No Data Available- </label>
                    </div>
                
                
                    <div id="dvOffTakeDetails" style="visibility:hidden">
                        
                            <input id="txtSearch" type="search" placeholder="Search Products" class="SearchText" style="padding:0px;" autofocus />            
                        
                        <div class="ui-grid-a" style="margin:0px; padding: 0px;">
                            <div class="ui-block-a" style="width:80%;height: 50px;padding: 0px;margin-bottom:5px;">
                                <select id="selBin"> <option id="optAllBins" value="All"> All Bins </option> </select>
                            </div>
                            <div class="ui-block-b" style="width: 20%;padding: 0px;margin-bottom:5px;">
                               <a data-role="button" data-theme="c" id="btnSearchProductOrBin">Go</a>    
                            </div>
                        </div>
                        <br />
                        <table data-role="table" data-mode="reflow" class="ui-body-d ui-shadow table-stripe ui-responsive" style="display: table"> 
                            <thead>
                                <tr style="cursor: pointer">
                                    <th data-sort="string"> Product </th>
                                    <th data-sort="string"> Bin </th>
                                    <th style="text-align:center" data-sort="float"> Offtake </th>
                                </tr>
                            </thead>
                            <tbody id="tblDetails" >
                            </tbody>
                        </table>                
                        <br /> <br /> 
                        <div style="color:Red; font-weight:bold">*** &nbsp - &nbsp  Figures are not in expected range. Some transactions are not properly reported.</div>
                    </div>                    
                </li>
            </ul>
        </div>
    </div>
</div>

<div data-role="dialog" id="pageTransactionOfftakeDetails">   
    
    <div data-role="header" data-theme="d">
		<h1>Product Offtake Details</h1>
	</div>

    <div data-role="content">
        <div class="ui-grid-a">
            <div class="ui-block-a" style="width:30%">
                Product Name : &nbsp &nbsp
            </div>
            <div class="ui-block-b" style="width:70%">
                <label id="lblProductName"></label>
            </div>
        </div>
        <br />
        <div class="ui-grid-a">
            <div class="ui-block-a" style="width:30%">
                FOWST : &nbsp &nbsp
            </div>
            <div class="ui-block-b" style="width:70%">
                <label id="lblFOWSTName"></label>
            </div>
        </div>
        <br />
        <div class="ui-grid-a">
            <div class="ui-block-a" style="width:30%">
                Bin : &nbsp &nbsp
            </div>
            <div class="ui-block-b" style="width:70%">
                <label id="lblBin"></label>
            </div>
        </div>

        <br /> <br /> 

        <table data-role="table" id="tblOfftakeDetails" class="ui-body-d ui-shadow table-stripe ui-responsive" data-column-btn-theme="b">
            <thead>
                <tr>                   
                    <th>Transaction Type</th>
                    <th>Transaction Date</th>
                    <th style="text-align:right">Quantity</th>
                    <th style="text-align:right">Balance</th>                                    
                </tr>                    
            </thead>                
            <tbody id='tblOfftakeDetailsBody'>
            </tbody>
        </table>

        <br /> <br />
        <a href="#" data-rel="back" data-role="button">Close</a>
    </div>       
</div>





