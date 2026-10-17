﻿<!DOCTYPE html>
<html>
<head>
    <title>@ViewData("Title")</title>
    <meta name="viewport" content="width=device-width,initial-scale=1,user-scalable=no,maximum-scale=1">
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <script src="/Scripts3/others/require.js"></script>
    <script>
        if ((location.hostname == 'localhost') 
            || (location.hostname == 'mobiz-apps-x.com')) {
            window.assetversion = '';
        } else {
            window.assetversion = typeof assetversion != 'undefined' ? assetversion : new Date().getTime();
        }

        window.IsBootstrap = true;
        window.IsEcommerce = false;

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
    <link rel="stylesheet" href="/Content/bootstrap-5.0.0/css/bootstrap.css">
    <link rel="stylesheet" href="/Content/bootstrap-5.0.0/bootstrapfive-icons.css?v=@Format(Date.Now, "MMddyyhhmmss")">
    <link rel="stylesheet" href="/Content/bootstrap-5.0.0/css/bootstrapfive-custom.css?v=@Format(Date.Now, "MMddyyhhmmss")">
    <link rel="stylesheet" href="/Content/metro-calendar.css">
    <link rel="stylesheet" href="/Content/jquery.dataTables2.css" />
    <script src="/Scripts3/common/init.js?crc="></script>
    <style>
        .ui-loader {
            display: none !important;
        }
        .ui-page-active{
            outline:none;
        }

        #tblBrowsePopup_wrapper{
            overflow:auto !important;
        }

        .required{
            color: #ff0000;
        }

        a.browseSelected,
        a.browseSelected:hover,
        tr.browseSelected,
        tr.browseSelected:hover{
            background-color: #c1daff !important;
        }
        div[data-role="popup"]{
            display:none !important;
        }
        .hidetab{
            display:none !important;
        }
        body {
            font-family: 'Outfit', sans-serif !important;
        }
    </style>
    @RenderSection("head", False)
</head>
<body class="cnt-home" data-role="none">
    @RenderBody()
</body>
</html>