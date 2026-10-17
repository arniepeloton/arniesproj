@Code
    ViewData("Title") = "Generate Barcode Labels"
    Layout = "~/Views/Shared/Layout4.vbhtml"
End Code

@Section head

    <script type="text/javascript" src="/Scripts/sources/crm/jspdf.js"></script>
    <script type="text/javascript" src="/Scripts/sources/crm/html2canvas.js"></script>
    <script type="text/javascript" src="/Scripts/sources/crm/jspdf.min.js"></script>
    <script type="text/javascript" src="/Scripts/sources/crm/CODE128.js"></script>
    @*<script type="text/javascript" src="/Scripts/sources/crm/JsBarcode.js"></script>*@
    <script type="text/javascript" src="/Scripts/sources/KnowledgeBase/GeneratePDFKb.js?v=8.11.22"></script>

End Section

<style type="text/css">
    @@media print{
        .footer{
            display: table-footer-group;
        }
    }

    @@media screen{
        .footer{
            display:none;
        }
    }

    @@page{
        margin:100px;
    }
</style>
<div id="dvMain" data-role="page">
    <div style="width:100%; text-align:center; margin-top:100px;">
        Article Name: <b id="lblFileName"></b>
        <br /><br />
        Generating <span id="txtTo">0</span> %, please wait ...
        <br /><progress id="progBar" value="0" max="0"></progress>
    </div>
    
    <div id="BarCodeCanvass" style="visibility:hidden">
    </div>
    @*<canvas id="myCanvas" style="width:100%; height:auto;"></canvas>*@

    <div id="listAnswerDetails" style="display:none; padding:10px 0px 10px 0px;">

    </div>
</div>