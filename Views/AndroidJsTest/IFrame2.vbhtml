@Code
    ViewData("Title") = "Android JS Test IFrame"
    Layout = "~/Views/Shared/Layout3.vbhtml"
End Code
@Section head
    <style>
        body
        {
            overflow: hidden;
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
    require([GetPathFormat('/Scripts3/AndroidJsTestIframe2.js?crc=')]);
</script>
<iframe id="iFrameMain"></iframe>