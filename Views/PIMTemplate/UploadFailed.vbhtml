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
            <p>There was an error in uploading your file. Please make sure that your file is not empty and the template is correct. Please 
                <a id="lnkUploadTemplate" style="cursor:pointer; font-size:16px;">try again</a>.
            </p>
            
        </div>

        <div>
            <br />
            <br />
	        <div style="text-align:center"> <a id="btnPIMGenTemplateBackToHome" data-role="button" data-rel="back">Back To Home</a></div>
        </div>

    </div>


</div>


