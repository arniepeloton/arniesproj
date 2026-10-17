@Code
    ViewData("Title2") = "PO for Approval"
    Layout = "Master.vbhtml"
End Code
    <script type="text/javascript">
    
    </script>
    @If ProWeb.Common.UseMinifiedJavascripts Then
        @<text>
        <script src="/Scripts/minified/Drp.js" type="text/javascript"></script>
        </text>
    Else
        @<text>
        <script src="/Scripts/sources/Project/Drp.js" type="text/javascript"></script>
        </text>
    End If
    <style type="text/css">
        
    </style>
<div id="divDrpStocks">

</div>
