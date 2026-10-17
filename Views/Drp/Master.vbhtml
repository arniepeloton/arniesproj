@Code
    ViewData("Title") = "Distribution Resource Planning"
    Layout = "~/Views/Main/Layout.vbhtml"
End Code
    <script type="text/javascript">
    </script>
    @If ProWeb.Common.UseMinifiedJavascripts Then
        @<text>
        </text>
    Else
        @<text>
        </text>
    End If
    <style type="text/css">
        
    </style>
<div id="divDrpTab" class="ui-tabs ui-widget ui-widget-content ui-corner-all">
    <ul class="ui-tabs-nav ui-helper-reset ui-helper-clearfix ui-widget-header ui-corner-all">
        <li class="ui-state-default ui-corner-top"><a href="/drp/StockSched">Planned Orders</a></li>
        <li class="ui-state-default ui-corner-top"><a href="/drp/PoForApproval">PO for Approval</a></li>
        <li class="ui-state-default ui-corner-top"><a href="#">Open POs for Follow Up</a></li>
        <li class="ui-state-default ui-corner-top"><a href="#tabs-3">Cancelled POs</a></li>
        <li class="ui-state-default ui-corner-top"><a href="/drp/Parameters">Parameters</a></li>
    </ul>
    <div class="ui-tabs-panel ui-widget-content ui-corner-bottom" style="padding: 10px">
        <h2>@ViewData("Title2")</h2>
        @RenderBody()
    </div>
</div>
