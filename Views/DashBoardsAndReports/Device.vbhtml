@Code
    ViewData("Title") = "GPS Device Tracker"
    If Request.IsAjaxRequest() Then
        Layout = ""
    Else
        Layout = "~/Views/Shared/BootstrapFiveLayout.vbhtml"
    End If
End Code

@Section head
    @*<link rel="stylesheet" href="/Content/flatui-css-custom.css" />*@
    <script src="http://maps.googleapis.com/maps/api/js?v=3.exp&key=AIzaSyDKWvMWWXqdnh_dQEb8OjFQouNO85_-g5w&sensor=false"></script>
    <script>
        require([GetPathFormat('/Scripts/sources/DashBoardsAndReports/Device.js?crc=' + assetversion)]);        
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
    
    .elementsForErp{
        display:none;
    }
</style>
    
End Section

<div data-role="page">
    

    <div id="divHeaderGPS" data-role="header" data-position="fixed" style="background: none white; border: none">
        <nav class="navbar navbar-dark bg-primary sticky-top text-light pl-3 pr-3 mb-3">
            <div>
                <button id="btnHome" data-role="none" class="btn btn-primary">
                    <span class="bi-chevron-left"></span>
                </button>
                Fleet Tracking & Management
            </div>
        </nav>
        @*original header
        <div class="row" style="margin:0px !important;">
            <div class="col" style="padding:0px !important;">
                <button data-role="none" id="btnHome" style="height:100%;" class="btn btn-primary btn-block btn-lg rounded-0">
                    <span id="iconHome" class="bi-house-fill"></span>
                    <div id="lblHome" class="menu-description mt-1">Home</div>
                </button>
            </div>
            <div class="col" style="padding:0px !important;">
                <button data-role="none" id="btnDevice" style="height:100%;" class="btn btn-primary btn-block btn-lg rounded-0">
                    <span class="bi-phone"></span>
                    <div class="menu-description mt-1"><span id="lblDevice">Device</span></div>
                </button>
            </div>
            <div class="col" style="padding:0px !important;">
                <button data-role="none" id="btnPeriod" style="height:100%;" class="btn btn-primary btn-block btn-lg rounded-0">
                    <span class="bi-calendar3"></span>
                    <div class="menu-description mt-1" id="lblPeriod"></div>
                </button>
            </div>
            <div class="col-3 col-md-2" style="padding:0px !important;">
                <button data-role="none" id="btnRedFlags" style="height:100%;" class="btn btn-primary btn-block btn-lg rounded-0">
                    <span class="bi-flag-fill text-danger"></span>
                    <div class="menu-description mt-1" id="lblRedFlags">Red Flags</div>
                </button>
            </div>
            <div class="col-3 offset-md-3 col-md-3" style="padding:0px !important;">
                <button data-role="none" id="btnGo" style="height:100%;" class="btn btn-success btn-block btn-lg rounded-0">
                    <span class="bi-three-dots"></span>
                    <div class="menu-description mt-1">More</div>
                </button>
            </div>
        </div>*@
    </div>

    <div data-role="content" class="container-fluid">        
        
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
                        <fieldset id="txtDateFrom" data-theme="c" data-role="DateCombo" data-usecurrent="1"> </fieldset>
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
        <div class="row mb-3">
            <div class="col-md-6 mb-3">
                <table style="width:100%;">
                    <tr>
                        <td class="pr-3" style="width:100px;">Device:</td>
                        <td class="pr-3"><button id="btnDevice" data-role="none" class="btn custom-btn-theme-c btn-less-width btn-secondary btn-block"><span id="lblDevice">Please Select Device</span></button></td>
                    </tr>
                </table>
            </div>
            <div class="col-md-6 mb-3">
                <table style="width:100%;">
                    <tr>
                        <td class="pr-3" style="width:100px;">Period:</td>
                        <td class="pr-3"><button id="btnPeriod" data-role="none" class="btn custom-btn-theme-c btn-less-width btn-secondary btn-block"><span id="lblPeriod">Set Date</span></button></td>
                    </tr>
                </table>
            </div>
        </div>        

        <div class="mb-3">
            <h4>Map</h4>
            <div id="googleMap" style="width:100%;height:400px;text-align:center; border:1px solid #CCC; border-radius:3px;"></div>
        </div>

        <div class="mb-3">
            <h4>This device is being monitored by:</h4>
            <table data-role="table" data-mode="reflow" class="table table-light table-hover table-striped" style="width:100%; display: table">
                @*<thead>
                <tr>
                    <th> Organization Name </th>
                </tr>
            </thead>*@

                <tbody id="tblOrganizationDetails"></tbody>
            </table>
        </div>

        <div class="mb-3">
            <h4>Track Details</h4>
            <table data-role="table" id="tblDetails" data-mode="reflow" class="table table-light table-hover table-striped" style="width:100%; display: table">
                <thead>
                    <tr>
                        <th style="text-align:center"> Date </th>
                        <th style="text-align:center"> Time </th>
                        <th style="text-align:center"> Location Coordinates </th>
                        <th style="text-align:center"> Source Of Data </th>
                    </tr>
                </thead>

                <tbody></tbody>
            </table>
        </div>   
    </div>
</div>