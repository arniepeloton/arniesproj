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
    <script src="/Scripts3/others/require.js"></script>
    <script>
        if (location.hostname == 'localhost') {
            window.assetversion = '';
        } else {
            window.assetversion = new Date().getTime();
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
    <link rel="stylesheet" href="/Content/Ecommerce/css/Ecommerce.css">

    <link rel="stylesheet" href="/Content/flatui-css-custom.css">
    <link rel="stylesheet" href="/Content/bootstrap-custom.css">
    <link rel="stylesheet" href="/Content/Ecommerce/css/bootstrap.min.css">
    <link rel="stylesheet" href="/Content/metro-calendar.css">

    <link rel="stylesheet" href="/Content/Ecommerce/css/main.css">
    <link rel="stylesheet" href="/Content/Ecommerce/css/blue.css">    
    <link rel="stylesheet" href="/Content/Ecommerce/css/owl.carousel.css">
    <link rel="stylesheet" href="/Content/Ecommerce/css/owl.transitions.css">
    <link rel="stylesheet" href="/Content/Ecommerce/css/animate.min.css">
    <link rel="stylesheet" href="/Content/Ecommerce/css/rateit.css">
    <link rel="stylesheet" href="/Content/Ecommerce/css/bootstrap-select.min.css">
    <link rel="stylesheet" href="/Content/Ecommerce/css/lightbox.css">

    <link rel="stylesheet" href="/Content/Ecommerce/css/font-awesome.css">
    <link rel="stylesheet" href="/Content/jquery.dataTables2.css" />
    <link rel="stylesheet" href="/Content/jquery.buttons.dataTables.min.css" />
    <script src="/Scripts3/common/init.js?crc="></script>
    

    @RenderSection("head", False)
</head>
<body class="cnt-home">    

    <div style="margin-bottom:80px;">
        @RenderBody()
    </div>

    @*<script src="/Content/Ecommerce/js/jquery-1.11.1.min.js"></script>*@
    @*<script src="/Content/Ecommerce/js/bootstrap.min.js"></script>*@
    <script src="/Content/Ecommerce/js/bootstrap-hover-dropdown.min.js"></script>
    <script src="/Content/Ecommerce/js/owl.carousel.min.js"></script>
    @*<script src="/Content/Ecommerce/js/echo.min.js"></script>*@
    @*<script src="/Content/Ecommerce/js/jquery.easing-1.3.min.js"></script>*@
    <script src="/Content/Ecommerce/js/bootstrap-slider.min.js"></script>
    <script src="/Content/Ecommerce/js/jquery.rateit.min.js"></script>
    @*<script src="/Content/Ecommerce/js/lightbox.min.js"></script>*@
    <script src="/Content/Ecommerce/js/bootstrap-select.min.js"></script>
    <script src="/Content/Ecommerce/js/wow.min.js"></script>
    @*<script src="/Content/Ecommerce/js/scripts.js"></script>*@
</body>
</html>