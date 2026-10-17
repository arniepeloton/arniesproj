<div data-role="page" style="bottom:0; overflow:auto;">
    <div data-role="header">
        <a href="#" data-icon="arrow-l" data-rel="back">Back</a>
        <h1>@ViewData("Title")</h1>
        
    </div>
    <div data-role="content">
        <div class="divBrowserContentTop"></div>
        <form name="formBrowser" style="margin: 0 -7px">
        <div>
            <input name="txtBrowser" type="search" value="" /></div>
        </form>
        <a name="butBarcode" href="#" data-role="button" style="display:none">Barcode / QR Code Scan</a>
 @*       <button name="butBarcode" style="display:none">Barcode / QR Code Scan</button>*@

        <div name="divBrowserResults" style="display:none">
            <div class="ListViewMarginBottom" name="divRecordCount">
            </div>
            <ul name="ulBrowser" data-role="listview" data-filter="true">
            </ul>
            <div class="ListViewMarginTop" name="divRecordCount">
            </div>
        </div>
        <div name="divButRight" style="display:none">
            <a name="butRight" href="#" data-role="button">Add...</a>
        </div>
        <a name="butSearchMore" href="#" data-role="button" style="display:none">Search More</a>
        <a name="butManualEntry" href="#" data-role="button" style="display:none">Manual Entry</a>
    </div>
</div>
