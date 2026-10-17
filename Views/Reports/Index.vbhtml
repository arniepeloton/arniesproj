@Code
    
    ViewData("Title") = "Reports"
    Layout = "~/Views/Main/Layout.vbhtml"
End Code

    <script type="text/javascript">
    
    </script>
    @If ProWeb.Common.UseMinifiedJavascripts Then
        @<text>
        @*<script src="/Scripts/minified/Reports.js" type="text/javascript"></script>*@
        </text>
    Else
        @<text>
        @*<script src="/Scripts/sources/Project/Reports.js" type="text/javascript"></script>*@
        </text>
    End If
    <style type="text/css">
        
    </style>

<ul>
    <li>
        Dashboard
        <ul>
            <li>
                <a href="/Reports/UnreceivedPOs">Unreceived PO's</a>
            </li>
        </ul>
    </li>
    <li>Reports
        <ul>
            <li><a href="/Reports/AmountAndQuantity">Amount and quantity report</a></li>
            <li><a href="/Reports/QrAnalyzer">Quick Response Analyzer</a></li>
            <li><a href="/Reports/PerfectOrders">Perfect Orders</a></li>
            <li><a href="/Reports/QrBranchesStatus">QR Branches Status</a></li>
            <li><a href="/Reports/CustomerServiceLevel">Customer Service Level</a></li>
            <li><a href="/Reports/PriceLevels">Price Levels</a></li>
        </ul>
    </li>
    <li>Delivery Logger
        <ul>
            <li><a href="/DeliveryLogger/Alerts">Alerts</a> </li>
        </ul>
    </li>

    <li>Maintenance
        <ul>
            <li><a href="/Defaults/MBORT/">MBORT</a> </li>
        </ul>
    </li>
</ul>
