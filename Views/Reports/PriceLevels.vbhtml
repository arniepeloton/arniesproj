@Code
    ViewData("Title") = "Price Levels"
    Layout = "~/Views/Main/Layout.vbhtml"
End Code

    <script type="text/javascript">
    
    </script>
    @If ProWeb.Common.UseMinifiedJavascripts Then
        @<text>
        <script src="/Scripts/minified/ReportPriceLevels.js" type="text/javascript"></script>
        </text>
    Else
        @<text>
        <script src="/Scripts/sources/Project/ReportPriceLevels.js" type="text/javascript"></script>
        </text>
    End If
    <style type="text/css">
        
    </style>

<div id="divMain">
<div id="divResult">
</div>
<button name="butRefresh">Refresh Price Levels</button>
<div name="divPrices" style="margin-bottom:30px"></div>
<div name="divCustomers"></div>
</div>
