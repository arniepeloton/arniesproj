@Code
    ViewData("Title") = "Delivery Logger Alerts"
    Layout = "~/Views/Main/Layout.vbhtml"
End Code
    <script type="text/javascript">
        var mData = @Html.Raw(ViewData("Data"));
    </script>
    @If ProWeb.Common.UseMinifiedJavascripts Then
        @<text>
        <script src="/Scripts/minified/DeliveryLoggerAlerts.js" type="text/javascript"></script>
        </text>
    Else
        @<text>
        <script src="/Scripts/sources/Project/DeliveryLoggerAlerts.js" type="text/javascript"></script>
        </text>
    End If
    <style type="text/css">

    </style>
<div id="divInput">
    <div id="divResult">
    </div>
    Show logs with the following reasons:<br />
    <input id="chkNormal" type="checkbox" /><label for="chkNormal" class="CheckLabel">Normal</label>
    <input id="chkNewSeal" type="checkbox" checked="checked" /><label for="chkNewSeal" class="CheckLabel">New Seal</label>
    <input id="chkSerialMismatch" type="checkbox" checked="checked" /><label for="chkSerialMismatch" class="CheckLabel">Serial Mismatch</label>
    <input id="chkTampered" type="checkbox" checked="checked" /><label for="chkTampered" class="CheckLabel">Tampered</label>
    <div class="clear">
    </div>
    <button id="butRefresh">Refresh</button>
</div>
<div id="divData" style="height: 400px">
</div>
<span id="spanCount"></span>