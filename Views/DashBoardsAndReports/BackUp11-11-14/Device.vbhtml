@Code
    ViewData("Title") = "Dashboard Settings"
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head

<script src="http://maps.googleapis.com/maps/api/js?key=AIzaSyDKWvMWWXqdnh_dQEb8OjFQouNO85_-g5w"> </script>
    <script>
        require([GetPathFormat('/Scripts/sources/DashBoardsAndReports/Device.js?crc=')]);        
    </script>
    
    <style>
    
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
        <div class="ui-grid-d" >    
            <div class="ui-block-a">
                <div id="btnHome" style="margin:0px; height: 80px;" data-role="button" data-theme="b" data-corners="false" class="HotButtons">
                    <span>Home</span> <br />                    
                </div>
            </div>
            <div class="ui-block-b">
                <div id="btnDevice" style="margin:0px; height: 80px;" data-role="button" data-theme="b" data-corners="false" class="HotButtons">
                    <span>Device</span> <br />
                    <span id="lblDevice"></span>
                </div>
            </div>
            <div class="ui-block-c">
                <div id="btnPeriod" style="margin:0px; height: 80px;" data-role="button" data-theme="b" data-corners="false" class="HotButtons">
                    <span>Period</span> <br />
                    <span id="lblPeriod"></span>
                </div>
            </div>
            <div class="ui-block-d">
                <div id="btnRedFlags" style="margin:0px; height: 80px;" data-role="button" data-theme="b" data-corners="false" class="HotButtons">
                    <span>Red Flags</span> <br />
                    <span id="lblRedFlags"></span>
                </div>
            </div>            
            <div class="ui-block-e">
                <div id="btnGo"style="margin:0px; height: 80px;" data-role="button" data-theme="green" data-corners="false" class="HotButtons">
                    <span>More</span>
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
                    <select id='selPeriod'> 
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

                    <a id="btnDoneCustomerPeriod" data-role="button" href="#" data-theme="e">Done</a>
                </form>
            </div>       
        </div>

        <div data-role="popup" id="panDevice" data-theme="b" class="ui-corner-all"> 
            <a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>
            <center><h3>Device</h3></center>
            <div data-role="content">
                <form>
                    <select id='selDevice'> </select>
                    <ul data-role="listview" data-inset="true" data-theme="e" id="lstDevice">
                       
                    </ul>
                </form>
            </div>       
        </div>

@* Body Design *@

    <ul data-role="listview" data-inset="true">
            <li data-role="list-divider" id="liMainChart">Map</li>
            <li>
                <div id="googleMap" style="width:100%;height:400px;text-align:center"></div>
               
            </li>
        </ul>    

        @*<a id="btnDisplayTracks" data-role="button" href="#" data-theme="e">Display Tracks</a>*@
        
        <br />

        <ul data-role="listview" data-inset="true">
            <li data-role="list-divider" data-theme="e">&nbsp;Track Details</li>
            <li>
                <br />
                <table data-role="table" data-mode="reflow" class="ui-body-d ui-shadow table-stripe ui-responsive" style="width:100%; display: table"> 
                    <thead>
                        <tr>
                            <th style="text-align:center"> Date </th>
                            <th style="text-align:center"> Time </th>
                            <th style="text-align:center"> Location Coordinates </th>
                            <th style="text-align:center"> Source Of Data </th>
                        </tr>
                    </thead>

                    <tbody id="tblDetails" >
                    </tbody>
                </table>                
             </li>
        </ul>     
    </div>

</div>





