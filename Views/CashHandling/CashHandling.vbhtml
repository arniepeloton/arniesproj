@Code
    ViewData("Title") = "Cash Handling"
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head
   
<link rel="stylesheet" href="/Content/flatui-css-custom.css" />
<link rel="stylesheet" href="/Content/metro-calendar.css" />

    <script>
        require([GetPathFormat('/Scripts/sources/CashHandling/CashHandling.js?crc=')]);
    </script>

End Section

@*Header*@

<div id="pageMain" data-role="page">
    <div data-role="header">
        <h1 id="MainPageTitle">@ViewData("Title")</h1>
    </div>

    <div>
        <div class="ui-grid-a">         
            <div class="ui-block-a">
                <div id="btnFOWST" class="ui-bar ui-bar-c" data-role="button" data-corners="false" style="height:60px; text-align:center; margin:0px">
                    FOWST 
                    <span class="ui-icon ui-icon-arrow-d" style="display:inline-block; margin-left:5px;"></span>
                    <br />
                    <span class="lblCashierSelectedFowstGroup2" style="font-weight:normal; font-size:14px">-</span>
                </div>
            </div>
            <div class="ui-block-b">
                <div id="btnReportDate" class="ui-bar ui-bar-c" data-role="button" data-corners="false" style="height:60px; text-align:center; margin:0px">
                    Date
                    <span class="ui-icon ui-icon-arrow-d" style="display:inline-block; margin-left:5px;"></span>
                    <br />
                    <span class="lblReportDate2" style="font-weight:normal; font-size:14px">Today</span>
                </div>
            </div>
        </div>
    </div>

    <div data-role="content">
        @*style="align-content:center"*@
        <div id="divOnPrintPOSSalesReport">

        </div>
    </div>

</div>
