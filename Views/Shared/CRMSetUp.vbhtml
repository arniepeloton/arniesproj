<!DOCTYPE html>
<html>
<head>
    <title>@ViewData("Title")</title>
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <link rel="stylesheet" href="/Content/css/jQueryMobileOverrides.css" />
    <link rel="stylesheet" href="/Scripts3/others/themes/default/jquery.mobile.css" />
    <script src="/Scripts/jQuery.js"></script>
    <script src="/Scripts3/common/init.js"></script>
    <script src="/Scripts3/others/jquery.mobile.js"></script>
    <script src="/Scripts3/others/jquery.format.js"></script>
    <script src="/Scripts3/common/common.js"></script>
    <script src="/Scripts3/common/dom.js"></script>
    @RenderSection("head", False)
    @If Common.WebRequest.QueryString("IsMobile") = "1" Then
        @<text>
        <script src="cordova-2.1.0.js"></script>
        <script src="Scripts3/common/mobile.js"></script>
        </text>
    End If
</head>
<body>
    @RenderBody()
</body>
</html>
