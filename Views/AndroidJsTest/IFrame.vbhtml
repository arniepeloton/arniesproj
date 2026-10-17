@Code
    ViewData("Title") = "Android JS Test IFrame"
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code
@Section head
    <style>
        body
        {
            @*overflow: hidden;*@
        }
        .fluidMedia
        {
            padding: 0;
            margin: 0;
        }
        iframe
        {
            border: none;
            width: 100%;
        }
    </style>
End Section
<script>
    require([GetPathFormat('/Scripts3/AndroidJsTestIframe.js?crc=')]);
</script>
<div data-role="page">
    <div data-role="header" style="border: none">
        <div>@Now | <span id="spanUrl"></span>
        </div>
        <div data-role="navbar">
            <ul>
                <li><a href="#" data-href="a">Main</a></li>
                <li><a href="#" data-href="divTestControl">Test Control</a></li>
                <li><a href="#" data-href="divLogs">Logs</a></li>
            </ul>
        </div>
        <!-- /navbar -->
    </div>
    <div data-role="content" class="fluidMedia">
        <div id="a" class="content_div">
            <iframe id="iFrameMain"></iframe>
        </div>
        <div id="divTestControl" class="content_div">
        </div>
        <div id="divLogs" class="content_div">
            <h2>Logs</h2>
        </div>
    </div>
    @*<div data-role="footer">
        <h2>Footer</h2>
    </div>*@
</div>
