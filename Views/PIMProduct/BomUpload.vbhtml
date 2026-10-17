@Code
    ViewData("Title") = "Import Bill of Materials By Excel File"
    Layout = "~/Views/Shared/Plain.vbhtml"
End Code

<script>
    require(['/Scripts3/BomUpload.js?crc='
    ]);
</script>

<div id="divPIMUploadFailed" style="display:none;">
    <p>
        Something went wrong while trying to save your data. Upon validating, the system has detected either some required fields that were left blank.
        Please <a id="lnkPIMUploadTemplateFailedDL" style="cursor:pointer; font-size:16px;">download your file</a> to see the problem rows highlighted in red.
    </p>
    <p>Once you have corrected your file, please upload it back. Thank you.</p>
</div>

<div id="divFailedMessagesMain" style="display:none;">
</div>
<form id="formBomUpload" enctype="multipart/form-data" method="post">
    <input type="file" id="fileBomUpload" name="fileBomUpload">
    <input type="submit" value="Upload File">
</form>

@*<br />
<p>Download the PIM template below to see an example of the format required.</p>
<a data-role="button" id="lnkDownloadGenTemplate">Download PIM Template</a>*@


<div>
    <br />
    <br />
    <div style="text-align:center;"> <a id="btnPIMUploadBackToHome" data-role="button" data-rel="back"><h3>Cancel</h3></a></div>
</div>