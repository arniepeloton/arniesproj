@Code
    ViewData("Title") = "Import Product From Excel File"
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head

    <script>
        require([GetPathFormat('/Scripts/sources/PIMTemplate/PIMTemplate.js?crc=')]);
    </script>
    
End Section

<div id="pagePIMTemplate" data-role="page">
    <div data-role="header">
        <h1>@ViewData("Title")</h1>
    </div>

    <div data-role="content">

        <div>
            <p>Something went wrong while trying to save your data. Upon validating, the system has detected either some required fields that were left blank or packing size units/quantities that are repeated.
             Please <a id="lnkDownloadReturnTemplate" style="cursor:pointer; font-size:16px;">download your file</a> to see  
            the problem rows highlighted in red.</p>
            <p>Once you have corrected your file, please <a id="lnkUploadTemplate" style="cursor:pointer; font-size:16px;">upload</a> it back. Thank you.</p>
        </div>

        <div>
            <br />
            <br />
	        <div style="text-align:center"> <a id="btnPIMGenTemplateBackToHome" data-role="button" data-rel="back">Back To Home</a></div>
        </div>

    </div>


</div>


