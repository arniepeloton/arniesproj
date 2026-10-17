@Code
    ViewData("Title") = "QR Branches Status"
    Layout = "~/Views/Main/Layout.vbhtml"
End Code
<style>
    #divShowBlank 
    {
        margin-top:20px;
    }
    #divShowBlank label 
    {
        line-height:25px;
    }
</style>
<script type="text/javascript">
    
</script>
@If ProWeb.Common.UseMinifiedJavascripts Then
    @<text>
    <script src="/Scripts/minified/QrBranchesStatus.js" type="text/javascript"></script>
    </text>
Else
    @<text>
    <script src="/Scripts/sources/Project/QrBranchesStatus.js" type="text/javascript"></script>
    </text>
End If
<style type="text/css">
    
</style>
<div id="divQrBranchesStatus">
    <button name="butRefresh">Refresh</button>
    <div name="divData">
    </div>
    <div id="divShowBlank">
        <input type="checkbox" id="chkShowBlank" /><label for="chkShowBlank">Show branches without imported data</label>
    </div>
</div>
