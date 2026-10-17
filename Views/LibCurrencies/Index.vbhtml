@Code
    ViewData("Title") = "Library - Currencies"
    Layout = "~/Views/Main/Layout.vbhtml"
End Code

    <script type="text/javascript">
    
    </script>
    @If ProWeb.Common.UseMinifiedJavascripts Then
        @<text>
        <script src="/Scripts/minified/LibCurrencies.js" type="text/javascript"></script>
        </text>
    Else
        @<text>
        <script src="/Scripts/sources/Project/LibCurrencies.js" type="text/javascript"></script>
        </text>
    End If
    <style type="text/css">
        .NegativeNumber
        {
            background-color: Red;
            color: White;
        }
    </style>

<div id="divLibCurrencies">
    <div name="divResult"></div>
    
    <button name="butRefresh">Refresh</button><br />
    <div name="divTable">
    </div>
    <hr />
    <a href="http://www.currency-iso.org/iso_index/iso_tables/iso_tables_a1.htm">Source of format</a>
    <div name="divUploader"></div>
</div>
