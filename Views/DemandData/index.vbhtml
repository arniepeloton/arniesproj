@Code
    ViewData("Title") = "Demand Data"
    Layout = "~/Views/Shared/Drp.vbhtml"
End Code
<script>
    require(['Drp/DemandData']);
</script>
<div id="bcMain" style="height: 700px; width: 100%" data-dojo-type="dijit.layout.BorderContainer" data-dojo-props="design: 'sidebar'" livesplitters="false">
<div data-dojo-type="dijit.layout.ContentPane" data-dojo-props="region: 'center'">
    <div id="divDemandDataGrid" style="height: 100%; width: 100%">
    </div>
</div>
<div data-dojo-type="dijit.layout.ContentPane" data-dojo-props="splitter: true,region: 'top'">
    <h2>
        Demand Data</h2>
    <button id="butDemandDataResummarize" data-dojo-type="dijit.form.Button" data-dojo-props="iconClass:'dijitIconConfigure'">
        Resummarize</button> 
    <div>
        <form id="formDemandDataSearch">
        Search:<input id="txtDemandDataSearch" dojotype="dijit.form.TextBox" /><button id="butDemandDataSearch" type="submit"
            data-dojo-type="dijit.form.Button" data-dojo-props="iconClass:'dijitIconConfigure'">
            Search</button>
        </form>
    </div>
</div>
<div data-dojo-type="dijit.layout.ContentPane" data-dojo-props="splitter: true,region: 'right'"
    style="width: 50%">
    <div id="bcDemandDataStock" style="height: 100%" data-dojo-type="dijit.layout.BorderContainer"
        data-dojo-props="design: 'sidebar'" livesplitters="false">
        <div data-dojo-type="dijit.layout.ContentPane" data-dojo-props="splitter: false,region: 'top'">
            <div>
                Comparative Time Series Sales Order Data (in <span id="spanDemandDataUnit">UNIT</span>)
            </div>
            <div id="divDemandDataSku" style="font-weight: bold">
                SKU #:</div>
        </div>
        <div data-dojo-type="dijit.layout.ContentPane" data-dojo-props="splitter: true,region: 'center'">
            <div id="divDemandDataGridMonthly">
            </div>
        </div>
        <div data-dojo-type="dijit.layout.ContentPane" data-dojo-props="splitter: true,region: 'bottom'"
            style="height: 50%">
            <div id="divDemandDataChart">
            </div>
        </div>
    </div>
</div>
</div>