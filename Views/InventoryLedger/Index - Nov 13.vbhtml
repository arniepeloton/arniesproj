@Code
    ViewData("Title") = "Inventory Ledger"
    Layout = "~/Views/Shared/Mobiz.vbhtml"
End Code
<script>
@Section Header

End Section
    require([GetPathFormat('/Scripts3/InventoryLedger.js?crc=')]);
</script>
<h1>@ViewData("Title")</h1>
<div data-role="collapsible" data-collapsed="false" data-theme="c" data-content-theme="d" data-collapsed-icon="arrow-r" data-expanded-icon="arrow-d" data-inset="true">
    <h2>
        <label>
            Display Type</label>
    </h2>
    <div data-role="fieldcontain">
        <label>
            <input name="optDisplayType" type="radio" value="ledger" checked="checked">Ledger
                <div style="font-size: smaller; font-weight: normal">
                    List all entries</div>
            </input>
        </label>
        <label>
            <input name="optDisplayType" type="radio" value="bin">Bin Balance<div style="font-size: smaller; font-weight: normal">
                List all balance of each bin</div>
            </input>
        </label>
        <label>
            <input name="optDisplayType" type="radio" value="fowst">Fowst Balance<div style="font-size: smaller; font-weight: normal">
                List all balance of each fowst</div>
            </input>
        </label>
        <label>
            <input name="optDisplayType" type="radio" value="product">Product Balance<div style="font-size: smaller; font-weight: normal">
                List all balance of each product</div>
            </input>
        </label>
        <input type="text" id="txtSearchItem" data-type="search" placeholder="Search products"/>
    </div>
</div>
<button id="butDisplay">Display</button>
<div id="divTable">
</div>
