@Code
    ViewData("Title") = "1Mobiz Inc."
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code

@Section head
    <link rel="Stylesheet" type="text/css" href="/Content/bootstrap/css/bootstrap.css" />
    @*<link rel="Stylesheet" type="text/css" href="/Content/css/SignIn.css" />*@
    <script>
        require([GetPathFormat('/Scripts/sources/SignIn/CustomerDisplay.js?crc=')]);
    </script>
End Section

