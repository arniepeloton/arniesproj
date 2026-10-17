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
        iframe
        {
            border: none;
        }
    </style>
End Section
<script>
    require([GetPathFormat('/Scripts3/AndroidJsTestIframe3.js?crc=')]);
</script>
<div style="overflow:auto">
    <iframe id="iFrameMain" style="min-height: 100px; position: fixed; top: 0px; left: 0px; bottom: 0px; right: 0px; width: 100%; height: 100%; border: none; margin: 0; padding: 0; overflow: scroll; z-index: 999999;"></iframe>
</div>
<iframe id="iFrameMain2" style="display: none;"></iframe>
