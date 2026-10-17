@Code
    ViewData("Title") = "Utilities"
    Layout = "~/Views/Shared/Mobiz.vbhtml"
End Code

<script>
    require([GetPathFormat('/Scripts3/Utilities.js?crc=' + assetversion)]);
    require([GetPathFormat('/Scripts3/html5-qrcode.min.js?crc=' + assetversion)]);
</script>
<h1>@ViewData("Title")</h1>
<div data-role="collapsible" data-collapsed="false" data-content-theme="d" style="margin-top:30px">
    <h3>SRV</h3>
    <div data-role="fieldcontain">
        <label for="txtSrvPassword">
            Secret Password:
        </label>
        <input id="txtSrvPassword" type="password" autofocus value="1qaz2wsx" />
    </div>
    <button id="butGetOrganizations">Get organizations</button>

    Subscriber:
    <select id="cbOrgs"></select>

    SRV Amount:
    <input id="txtSrvAmount" />

    <button id="butGetSrv" type="submit">Query</button>
    <pre id="divSrvResult"></pre>
</div>
<div data-role="collapsible" data-collapsed="true" data-content-theme="d" style="margin-top:30px">
    <h3>Last invoice per user on month</h3>
    Month (1-12):
    <input id="txtMonth" />

    Year:
    <input id="txtYear" />
    <button id="butDownloadExcel">Download Excel</button>
    <div id="divLastInvoiceResult"></div>
</div>

<div data-role="collapsible" data-collapsed="true" data-content-theme="d" style="margin-top:30px">
    <h3>Get wholesale.Invoice Info</h3>
    Invoice No.:
    <input id="txtInvoiceNum" />
    <button id="butGetInvoices">Get Invoices</button>
    <pre id="divInvoices"></pre>
</div>

<div data-role="collapsible" data-collapsed="true" data-content-theme="d" style="margin-top:30px">
    <h3>wholesale: Invoice change date</h3>

    Invoice ID:
    <input id="txtInvoiceID" />

    New Date:
    <input id="txtNewInvoiceDate" />
    <button id="butChangeInvoiceDate">Change Invoice Date</button>
</div>

<div data-role="collapsible" data-collapsed="true" data-content-theme="d" style="margin-top:30px">
    <h3>wholesale: Stock Receiving</h3>

    Receiving No.:
    <input id="txtReceivingNum" />

    <button id="butUntagReceivingAsPayable">Untag Receiving As Payable</button>

    <pre id="divReceivings"></pre>
</div>

<div data-role="collapsible" data-collapsed="true" data-content-theme="d" style="margin-top:30px">
    <h3>Set to JIT</h3>

    <button id="butSetJit">Set to JIT</button>
</div>

<div data-role="collapsible" data-collapsed="true" data-content-theme="d" style="margin-top:30px">
    <h3>Recompute Z-READ</h3>

    <button id="butGetFOWST">Get FOWST</button>

    FOWST:
    <select id="cbFOWST"></select>

    <button id="butGetDevices">Get Devices</button>

    Devices:
    <select id="cbDevices"></select>


    From (mm/dd/yyyy):
    <input id="txtFrom" />

    To (mm/dd/yyyy):
    <input id="txtTo" />

    <pre id="lblError"> </pre>
    <button id="butGetRecomputeZRead">Recompute Z-Read</button>
</div>

<div data-role="collapsible" data-collapsed="true" data-content-theme="d" style="margin-top:30px">
    <h3>Epson Receipt Printing via JavaScript</h3>

    <button id="butPrint">Print</button>
</div>

<div data-role="collapsible" data-collapsed="true" data-content-theme="d" style="margin-top:30px">
    <h3>Reimport Flush</h3>

    OrgID ID:
    <input id="txtFlushOrgID" />

    DeviceID
    <input id="txtFlushDeviceID" />

    Result:
    <div id="divFlushResult"></div>

    <button id="butFlush">Start</button>
</div>

<div data-role="collapsible" data-collapsed="true" data-content-theme="d" style="margin-top:30px">
    <div id="qr-reader" style="width:500px"></div>
    <div id="qr-reader-results"></div>
    <button id="butScan">Scan</button>
    <div id="divScanResult"></div>
</div>

<div data-role="collapsible" data-collapsed="true" data-content-theme="d" style="margin-top:30px">
    <div>Simulate a fake OrgID for MCR Download Database. This fake OrgID lasts for 1 minute</div>
    <button id="butDbDownloadFakeOrgID">Enable DB Download Fake OrgID</button>
</div>

<div data-role="collapsible" data-collapsed="true" data-content-theme="d" style="margin-top:30px">
    <h2>JSON parser</h2>
    <textarea id="txtJsonParserInput" placeholder="put input here"></textarea>
    <textarea id="txtJsonParserOuput" placeholder="output goes here" readonly></textarea>
    <button id="butJsonParser">Parse</button>
</div>

<div data-role="collapsible" data-collapsed="true" data-content-theme="d" style="margin-top:30px">
    <h2>Invoice Type Changer</h2>
    <div id="divInvoiceTypeChanger"></div>
    <form id="formInvoiceTypeChangerUpload">
        <input data-role="none" name="fileTemplate" type="file" class="form-control" accept=".xls,.xlsx" />
        <button type="submit">Analyze</button>
    </form>
    <a id="linkInvoiceTypeChangerAnalysisDownload" href="#" target="_blank">Download Analysis</a>
    <button id="butInvoiceTypeChanger">Apply Changes</button>
    <a id="linkInvoiceTypeChangerResultDownload" href="#" target="_blank">Download Result</a>

</div>

<div data-role="collapsible" data-collapsed="false" data-content-theme="d" style="margin-top:30px">
    <h3>Download Z-READ</h3>
    <button id="butDLZReadGetOrganizations">Get organizations</button>

    Subscriber:
    <select id="cbDLZreadOrgs"></select>

    FOWST:
    <button id="butDLZreadGetFOWST">Get FOWST</button>
    <select id="cbDLZreadFOWST"></select>
    <input id="chkDLZreadAllFowst" type="checkbox" /> <label for="chkDLZreadAllFowst">All FOWST</label>

    Device:
    <select id="cbDLZreadDevices"></select>

    From (mm/dd/yyyy):
    <input id="txtDLZreadFrom" type="date" />

    To (mm/dd/yyyy):
    <input id="txtDLZreadTo" type="date" />
    <pre id="divBegEndTrans" style="white-space:break-spaces;"></pre>
    <input id="chbZReadForceNONVAT" type="checkbox" /> <label for="chbZReadForceNONVAT">Change Store VAT Type to NON-VAT</label>
    <input id="chbZReadZeroSales" type="checkbox" /> <label for="chbZReadZeroSales">All figures must be zero.</label>
    <input id="chkZReadUseCache" type="checkbox" /> <label for="chkZReadUseCache">Use cached XML</label>
    <button id="butZReadGenerateNewCacheID">Generate New Cache ID</button>
    <button id="butGenerateDecal">Generate Decal</button>
    <pre id="lblError"> </pre>
    <fieldset style="border-width:5px; padding:25px;">
        <legend>ZRead Options</legend>
        <div>
            <label>Export Z-Read:</label>
            <input data-role="none" id="radZReadPerPeriod" name="radZRead" value="period" type="radio" checked /> <label for="radZReadPerPeriod">per Period</label>
            <input data-role="none" id="radZReadPerDay" name="radZRead" value="day" type="radio" /> <label for="radZReadPerDay">per Day</label>
        </div>
        <div>
            <label>Exclude PTU details in header:</label>
            <input data-role="none" id="radZReadPTU" name="radZReadPTU" value="period" type="checkbox" /> <label for="radZReadPerPeriod"></label>
        </div>
        <button id="butDLZreadGetRecomputeZRead">Download Z-Read</button>
        <button class="butRecacheXZData" report="ZRead">Recache Z-Read Data</button>
        <div style="text-align:center"></div>
    </fieldset>

    <fieldset style="border-width:5px; padding:25px;">
        <legend>XRead Options</legend>
        @*<div>
                <label>Export X-Read:</label>
                <input data-role="none" id="radXReadPerPeriod" name="radXRead" value="period" type="radio" checked /> <label for="radXReadPerPeriod">per Period</label>
                <input data-role="none" id="radXReadPerDay" name="radXRead" value="day" type="radio" /> <label for="radXReadPerDay">per Day</label>
            </div>*@
        <div>
            <label>Cashier:</label>
            <select id="selXReadCashier">
                <option>Please select</option>
            </select>
        </div>
        <button id="butDLXread">Download X-Read</button>
        <button class="butRecacheXZData" report="XRead">Recache X-Read Data</button>
        <div style="text-align:center"></div>
    </fieldset>

    <fieldset style="border-width:5px; padding:25px;">
        <legend>E-journal Options</legend>
        <div>
            <label>Exclude the following reports:</label>
            <input data-role="none" id="chbchbEjournalInvoice" name="chbEjournal" value="zread" type="checkbox" /> <label for="chbchbEjournalInvoice">Invoice</label>
            <input data-role="none" id="chbchbEjournalZRead" name="chbEjournal" value="zread" type="checkbox" checked /> <label for="chbchbEjournalZRead">Z-Read</label>
            <input data-role="none" id="chbchbEjournalXRead" name="chbEjournal" value="xread" type="checkbox" checked /> <label for="chbchbEjournalXRead">X-Read</label>
            <input data-role="none" id="chbchbEjournalReturns" name="chbEjournal" value="returns" type="checkbox" checked /> <label for="chbchbEjournalReturns">Returns</label>
        </div>
        @*<button id="butDLEJournal">Download E-Journal</button>*@
        <button id="butDLNewEJournal">Download New E-Journal</button>
    </fieldset>
    @*<button id="butDLCachedZread">Download Z-Read using cached data</button>*@
    <fieldset style="border-width:5px; padding:25px;">
        <legend>BIR Sales Summary Options</legend>
        <div>
            <label>File Type:</label>
            <input data-role="none" id="radBIRReportExcel" name="radBIRReportFileType" value="xlsx" type="radio" checked /> <label for="radBIRReportExcel">Excel</label>
            <input data-role="none" id="radBIRReportPDF" name="radBIRReportFileType" value="pdf" type="radio" /> <label for="radBIRReportPDF">PDF</label>
        </div>
        <div>
            <label>Exclude the following reports:</label>
            <input data-role="none" id="chbBIRReportSales" name="chbBIRReport" value="sales" type="checkbox" /> <label for="chbBIRReportSales">Sales Data</label>
            <input data-role="none" id="chbBIRReportSC" name="chbBIRReport" value="sc" type="checkbox" checked /> <label for="chbBIRReportSC">Senior Citizen</label>
            <input data-role="none" id="chbBIRReportPWD" name="chbBIRReport" value="pwd" type="checkbox" checked /> <label for="chbBIRReportPWD">PWD</label>
            <input data-role="none" id="chbBIRReportNAAC" name="chbBIRReport" value="naac" type="checkbox" checked /> <label for="chbBIRReportNAAC">Athelete</label>
            <input data-role="none" id="chbBIRReportSP" name="chbBIRReport" value="sp" type="checkbox" checked /> <label for="chbBIRReportSP">Solo Parent</label>
        </div>
        <button id="butDLNagaEJournal">Download BIR Sales Summary</button>
        <button id="butDLFixedEJournal">Download BIR Sales Summary with fixed date(hardcoded)</button>
    </fieldset>
    <button id="butDownloadReceipts">Generate Receipt PDF's</button>
    <a id="lnkDownloadDownloadReceiptPdfs" hidden target="_blank">Download Generated Receipt PDF's</a>
    <button id="butCacheInventoryMovement">CacheInventoryMovement</button>

    ForceFranchiseeDatabase (optional. if present, use this as the source of data):
    <select id="cbForceFranchiseeDatabase" class="AllowBlank"></select>

    ForceFranchiseeDatabase (optional. if present, use this as the destination of data):
    <select id="cbForceFranchisorDatabase" class="AllowBlank"></select>

    <input id="chkFranchiseeDataExporterStartFresh" type="checkbox" /> <label for="chkFranchiseeDataExporterStartFresh">Start fresh. Dont resume previous session.</label>
    <input id="chkFranchiseeDataExporterDetectInvoicesWithNoItems" type="checkbox" /> <label for="chkFranchiseeDataExporterDetectInvoicesWithNoItems">Detect invoices with no items (slow)</label>
    <button id="butFranchiseeDataExporter">FranchiseeDataExporter</button>
    <div>The selected Subscriber is the franchisee</div>
    <div><a href="/test/FranchiseeDataExporterDateStarted" target="_blank">Check progress</a></div>
    <div id="divFranchiseeDataExporterLog"></div>
    <button id="butRecacheZRead">Recache the ZRead CurTotalVatablePortionOfSCDTransaction column for all active stores based on the chosen date period</button>
    <button id="butGenerateCopyCacheInvoicesSql">GenerateCopyCacheInvoicesSql</button>
    <textarea id="txtGenerateCopyCacheInvoicesSql" readonly></textarea>
    <br />
    <br />
    Parameter used:<br />
    Subscriber = organization<br />
    From (mm/dd/yyyy) = invoice date from<br />
    To (mm/dd/yyyy) = invoice date to<br />
    <button id="butUpdateCacheInvoiceDaily">Update CacheInvoiceDaily</button>
    <div><a href="/Utilities/UpdateCacheInvoiceDailyProgress" target="_blank">Check progress</a></div>
    <br />
    <br />
    Parameter used:<br />
    Subscriber = organization<br />
    <button id="butDeleteCacheInvoicesDailyDuplicates">DeleteCacheInvoicesDailyDuplicates</button>
    <div><a href="/Utilities/UpdateCacheInvoiceDailyProgress" target="_blank">Check progress</a></div>
    <br />
    <br />
    Parameter used:<br />
    Subscriber = organization<br />
    From (mm/dd/yyyy) = invoice date from<br />
    To (mm/dd/yyyy) = invoice date to<br />
    <button id="butUpdateCacheInvoiceHourly">Update CacheInvoiceHourly</button>

    <br />
    <br />
    Parameter used:<br />
    Subscriber = organization<br />
    Fowst<br />
    From (mm/dd/yyyy) = invoice date from<br />
    To (mm/dd/yyyy) = invoice date to<br />
    <button id="butCacheSalesTables">Cache Sales Tables</button>
    <div><a href="/Utilities/UpdateCacheInvoiceDailyProgress" target="_blank">Check progress</a></div>

    <br />
    <br />
    Parameter used:<br />
    Subscriber = organization<br />
    Fowst<br />
    From (mm/dd/yyyy) = invoice date from<br />
    <button id="butSendSalesTables">Send Sales Tables</button>
    <div><a href="/Utilities/SalesTablesSenderLogs" target="_blank">Check progress</a></div>
</div>


<div data-role="collapsible" data-collapsed="false" data-content-theme="d" style="margin-top:30px">
    <h3>Cache Tables</h3>
    <button id="butDLZReadGetOrganizations">Get organizations</button>

    Subscriber:
    <select id="cbDLZreadOrgs"></select>


    <textarea id="txtCacheDataResult"></textarea>
    <button id="butRecacheData">Recache Data</button>

</div>

    <button id="butLongProcess">Long Process</button>
