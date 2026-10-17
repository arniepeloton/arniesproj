@Code
    ViewData("Title") = "Import Product From Excel File"
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head

    <script>
        require([GetPathFormat('/Scripts/sources/PIMTemplate/PIMTemplate.js?crc=')
            ]);
    </script>
    
End Section

<div id="pagePIMTemplate" data-role="page">
    <div data-role="header">
        <h1>@ViewData("Title")</h1>
    </div>

    <div data-role="content">

        <form action="PIMTemplate/Index" enctype="multipart/form-data" method="post">

            <label>Upload Product Template :</label>
            <div style="display:none;"><input type="text" id="OrgID" name="OrgID" style="display:none;"></div>
            <input type="file" id="objFile" name="objFile">
            <input type="submit" value="Upload">
        </form>

        <div>
            <br />
            <br />
	        <div style="text-align:center"> <a id="btnPIMTemplateBackToHome" data-role="button" data-rel="back">Back To Home</a></div>
        </div>

    </div>

</div>


