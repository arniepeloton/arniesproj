@Code
    ViewData("Title") = "Planned Purchase Order"
    Layout = "~/Views/Shared/Drp.vbhtml"
End Code
<style>
    .tableFields td 
    {
        vertical-align:top;
        padding-bottom: 10px;
    }
    .tableFields tr td:nth-child(1)
    {
        width:150px;
    }
</style>
<script>
    require(['Drp/DrpPpo']);
</script>
<div id="tabDrpPpo">
</div>
<h2>@ViewData("Title")</h2>
<button id="butRefresh" data-dojo-type="dijit.form.Button" data-dojo-props="iconClass:'dijitIconNewTask'">
    Refresh</button>    
<button id="butCreatePo" data-dojo-type="dijit.form.Button" data-dojo-props="iconClass:'dijitIconNewTask'">
    Make this a PO</button>
<table style="width: 100%" class="tableFields">
    <tr>
        <td>
            PO Date:
        </td>
        <td>
            <span id="spanPoDate" style="font-size: 12pt; font-weight: bold"></span>
        </td>
        <td id="tdPastDue" rowspan="6" style="width: 120px;display:none" >
            <div style="text-align: center">
                <img src="/Content/images/time_frame_128.png" /><br />
                Past Due
            </div>
        </td>
    </tr>
    <tr>
        <td>
            Vendor:
        </td>
        <td>
            <span id="spanSupplier" style="font-size: 12pt; font-weight: bold"></span>
        </td>
    </tr>
    <tr>
        <td>
            Vendor address:
        </td>
        <td>
            <span id="spanSupplierAddress"></span>
        </td>
    </tr>
    <tr>
        <td>
            Vendor contact numbers:
        </td>
        <td>
            <span id="spanSupplierContactNumber"></span>
        </td>
    </tr>
    <tr>
        <td>
            Shipper:
        </td>
        <td>
            <span id="spanShipper"></span>
        </td>
    </tr>
    <tr>
        <td>
            Remarks:
        </td>
        <td>
            <input id="txtRemarks" dojotype="dijit.form.Textarea" maxlength="500" style="width: 400px" />
        </td>
    </tr>
</table>

<div id="divStocks">
</div>
<div style="font-size: 14pt">
Transaction Value: P <span id="spanTransactionValue"></span>
</div>