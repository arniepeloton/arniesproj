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
@If Common.WebSession.LoggedIn Then
    @<text>
    <script>
        require(['Common/Common'], function () {
            Common.PostKey = '@(Common.WebSession.PostKey)';
        });
    </script>
    </text>
End If