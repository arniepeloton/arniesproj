<!DOCTYPE html>
<html>
<head>
    <title>@ViewData("Title")</title>
    @RenderPage("~/Views/Shared/Head.vbhtml")
</head>
<body class="tundra">
    <div id="divBody">
        @RenderBody()
    </div>
    <div id="divBodyLoading">
        <div class="claro dijitTextBoxHover dijitButtonNode">
            <div>
            </div>
            Loading...<br />
            Please wait
        </div>
    </div>
</body>
</html>
@If WebProj.WebSession.Instance.UserID.Length > 0 Then
    @<text>
    <script>
        require(['Common/Common'], function () {
            Common.PostKey = '@(WebProj.WebSession.Instance.PostKey)';
        });
    </script>
    </text>
End If