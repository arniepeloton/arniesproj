@*<!DOCTYPE html>
<html>
<head>
    <title>Cycle Count</title>
    @RenderPage("~/Views/CycleCount/CycleCountHeader.vbhtml")
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
End If*@

@RenderPage("~/Views/CycleCount/CycleCountHeader.vbhtml")

<section id="content" class="container_12">       
    @RenderBody()
</section>