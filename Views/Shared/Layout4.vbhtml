﻿<!DOCTYPE html>
<html>
<head>
    <title>@ViewData("Title")</title>
    <meta name="viewport" content="width=device-width,initial-scale=1,user-scalable=no,maximum-scale=1">
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <link rel="stylesheet" href="/Scripts3/others/themes/default/jquery.mobile.css" />
    <link rel="stylesheet" href="/Content/css/jQueryMobileOverrides.css" />
    <script type="text/javascript" src="/Scripts/jquery.js"> </script>
    <script type="text/javascript" src="/Scripts3/others/jquery.mobile.js"> </script>
    @*<script src="/Scripts3/others/require.js" data-main="/Scripts3/common/init.js"></script>*@
    @*<script src="/Scripts3/others/require.js"></script>*@
    <script>
        if ((location.hostname == 'localhost')
            || (location.hostname == 'mobiz-apps-x.com')) {
            window.assetversion = '';
        } else {
            window.assetversion = typeof assetversion != 'undefined' ? assetversion : new Date().getTime();
        }

        function GetWindowPathFormat(vString) {
            //            alert(vString + '. Please use GetPathFormat');
            // change GetWindowPathFormat to GetPathFormat
            // debugger;
            console.log('change GetWindowPathFormat to GetPathFormat: ' + vString);
            return GetPathFormat(vString);
        };

        function GetPathFormat(vString) {

            if (window.location.protocol.substring(0, 4) != 'file')
                return vString;

            function parseUrl(url) {
                // http://stackoverflow.com/a/6644749/334045
                var a = document.createElement('a');
                a.href = url;
                return a;
            }; // function parseUrl

            function GetExtension(vPathName) {

                var pMatch = vPathName.match(/.+\.([\w]+)$/im);
                if (pMatch != null)
                    return pMatch[1];
                else
                    return '';

            }; // function GetExtension

            var pCurrent = window.location.pathname.toLowerCase();
            var pIndex = pCurrent.indexOf('www/') + 4;
            var pRoot = pCurrent.substring(0, pIndex); // path to www
            var pIndex = pCurrent.lastIndexOf('/') + 1; // to folder

            if (vString.substring(0, 1) == '/')
                vString = vString.substring(1); // remove leading slash

            var pParsed = parseUrl(vString);
            var pOut = pRoot + pParsed.pathname.substring(pIndex);

            if (GetExtension(pOut) == '') {
                pOut += '.html';
                pOut += pParsed.search + pParsed.hash;
                return pOut.toLowerCase();
            } else {
                pOut += pParsed.search + pParsed.hash;
                return pOut;
            };
        }; // function GetPathFormat

    </script>

    @*<script src="/Scripts3/common/init.js?crc="></script>*@
    @RenderSection("head", False)
</head>
<body>
    @RenderBody()
    <div style="display:none;" id="divFooterAjax" data-role="footer" data-tap-toggle="false">
        <button id="butFooterAjax" data-icon="arrow-u" data-mini="true" data-inline="false">Loading data from server. If loading time is significantly longer than expected, please reload this page.</button>
        <div id="divFooterAjaxDetails">
            <span>Elapsed time: </span><span id="spanFooterElapsedTime"></span>
            <button id="butFooterAjaxAbort" data-mini="true" data-inline="true">Stop</button>
        </div>
    </div>
</body>
</html>