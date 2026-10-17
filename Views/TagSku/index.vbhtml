@Code
    ViewData("Title") = "Tag SKU"
    Layout = "~/Views/Shared/Layout3Single.vbhtml"
End Code
<script>
    require([GetPathFormat('/Scripts3/TagSku.js?crc=2165169F')]);
</script>
<style>
    .DataGrid td
    {
        padding: 3px;
        vertical-align: top;
        text-align: right;
    }
</style>
Stock numbers must be separated by space, comma, semicolon, or new line.
<textarea id="txtStockNumbers"></textarea>
<button id="butTag">Tag for Export</button> 