﻿<!DOCTYPE html>
<html>
<head>
    <title>@ViewData("Title")</title>
    <meta name="viewport" content="width=device-width,initial-scale=1,user-scalable=no,maximum-scale=1">
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
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
    <link rel="stylesheet" href="/Content/bootstrap-5.0.0/css/bootstrap.css">
    <link rel="stylesheet" href="/Content/bootstrap-icons.css">
    <script src="/Scripts3/common/init.js?crc="></script>
    <script src="/Content/bootstrap/js/bootstrap_modal.js"></script>
    @*<script type="text/javascript" src="/Content/bootstrap-5.0.0/js/bootstrap.js"></script>*@
    @*<script type="text/javascript">
        require(['/Content/bootstrap-5.0.0/js/bootstrap.js']);
    </script>*@
    <style>
        .ui-loader {
            display: none !important;
        }
        .ui-page-active{
            outline:none;
        }
    </style>
    @RenderSection("head", False)
</head>
<body class="cnt-home" data-role="none">
    <nav class="navbar sticky-top navbar-expand-lg navbar-dark bg-dark">
        <div class="container-fluid">
            <a data-role="none" class="navbar-brand" href="#">DMS</a>
            <button data-role="none" class="navbar-toggler" type="button" data-toggle="collapse" data-target="#navbarSupportedContent" aria-controls="navbarSupportedContent" aria-expanded="false" aria-label="Toggle navigation">
                <span class="navbar-toggler-icon"></span>
            </button>
            <div class="collapse navbar-collapse" id="navbarSupportedContent">
                <ul data-role="none" class="navbar-nav mr-auto mb-2 mb-lg-0">
                    <li class="nav-item">
                        <a data-role="none" class="nav-link" aria-current="page" href="/DMS/CheckIn">Check-in Docs</a>
                    </li>
                    <li class="nav-item">
                        <a data-role="none" class="nav-link" href="/DMS/VerifyRecords">Verify Records</a>
                    </li>
                    <li class="nav-item">
                        <a data-role="none" class="nav-link" href="/DMS/Settings">Settings</a>
                    </li>
                </ul>
                <form class="d-flex">
                    <input data-role="none" class="form-control mr-2" type="search" placeholder="Search" aria-label="Search">
                    <button data-role="none" class="btn btn-outline-success" type="submit">Search</button>
                </form>
            </div>
        </div>
    </nav>

    <div style="margin-bottom:80px;">
        @RenderBody()
    </div>

    <script type="text/javascript">
        var link = document.querySelectorAll('[href="' + location.pathname + '"]')[0];

        link.parentElement.classList.add('active');
        link.classList.add('active');
    </script>
</body>
</html>