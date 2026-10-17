@Code
    ViewData("Title") = "Quick Response Analyzer"
    Layout = "~/Views/Main/Layout.vbhtml"
End Code

    <script type="text/javascript">
        var mBranchData = @html.Raw(viewdata("BranchData"));
    </script>
    @If ProWeb.Common.UseMinifiedJavascripts Then
        @<text>
        <script src="/Scripts/minified/QuickResponseAnalyzer.js" type="text/javascript"></script>
        </text>
    Else
        @<text>
        <script src="/Scripts/sources/Project/QuickResponseAnalyzer.js" type="text/javascript"></script>
        </text>
    End If
    <style type="text/css">
        
    </style>

<div id="divQrAnalyzer">
    <h4>Step 1: Choose branches</h4>
    <div name="divBranches">
    </div>
    <br />
    <h4>Step 2: Choose SKU's</h4>
    <div name="divStocks">
    </div>
    <button name="butStock">Add SKU...</button>
    <div class="clear"></div>
    <br />
    <h4>Step 3: Choose date range</h4>
    <table class="tableForm">
        <tr>    
            <td title="Point of Sale / Cash Ship">POS/CS date from:</td>
            <td><input name="txtDateFrom" /></td>
        </tr>
        <tr>    
            <td title="Point of Sale / Cash Ship">POS/CS date to:</td>
            <td><input name="txtDateTo" /></td>
        </tr>
    </table>
    <br />
    <h4>Step 4: Generate report</h4>
    <button name="butGenerate">Generate Excel Spreadsheet</button>
</div>
