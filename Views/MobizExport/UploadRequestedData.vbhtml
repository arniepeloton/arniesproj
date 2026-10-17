@Code
    ViewData("Title") = ""
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

        <form action="UploadRequestedData?OrgID=9d4b1500-13c0-49f9-a8e5-a16c13ec899e" enctype="multipart/form-data" method="post">
    
            <div style="display:none;"><input type="text" id="OrgID" name="OrgID" style="display:none;"></div>
            <input type="file" id="objFile" name="objFile">
            <input type="submit" value="Upload">
        </form>

    </div>
</div>


