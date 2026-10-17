@Code
    ViewData("Title") = "Perfect Orders"
    Layout = "~/Views/Main/Layout.vbhtml"
End Code

    <script type="text/javascript">
    
    </script>
    @If ProWeb.Common.UseMinifiedJavascripts Then
        @<text>
        <script src="/Scripts/minified/PerfectOrdersReport.js" type="text/javascript"></script>
        </text>
    Else
        @<text>
        <script src="/Scripts/sources/Project/PerfectOrdersReport.js" type="text/javascript"></script>
        </text>
    End If
    <style type="text/css">
        
    </style>


<div id="divPerfectOrdersReport">
<button name="butRefresh">Refresh</button>
<div name="divData"></div>
</div>