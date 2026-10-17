@Code
    ViewData("Title") = "Fowst Bin"
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head

    <script>
        require([GetPathFormat('/Scripts/sources/FowstBin/FowstBin.js?crc=')]);
    </script>
    
End Section

<div data-role="page">
    <div data-role="header">        
        <h1>@ViewData("Title")</h1>
    </div>

    <div data-role="content">
        <div id="divFowstBinSearchResults">

            <div class="ListViewMarginBottom" id="divFowstBinStock"></div>

            <ul id="lstFowstBinStockList" data-role="listview" data-filter="true">
            </ul>

        </div>        
    </div>

</div>