if (window.proj == undefined) {
    // http://stackoverflow.com/a/901144/334045
    // redundant function
    function GetUrlParameters(vUrl) {
        // PARAMS:
        //  vUrl: [optional]

        vUrl = vUrl || window.location.search.substring(1);
        var urlParams = {};
        var match,
            pl = /\+/g,  // Regex for replacing addition symbol with a space
            search = /([^&=]+)=?([^&]*)/g,
            decode = function (s) { return decodeURIComponent(s.replace(pl, " ")); },
            query = vUrl;

        while (match = search.exec(query))
            urlParams[decode(match[1])] = decode(match[2]);

        return urlParams;

    }; // GetUrlParameters

    window.onerror = function(msg, url, line, col, error) {
        // Note that col & error are new to the HTML 5 spec and may not be 
        // supported in every browser.  It worked for me in Chrome.
        var extra = !col ? '' : '\ncolumn: ' + col;
        extra += !error ? '' : '\nerror: ' + error + '\n' + error.stack;

        // You can view the information in an alert to see things working like this:
        console.error("Error: " + msg + "\nurl: " + url + "\nline: " + line + extra);
        if (typeof Bs == 'object') Bs.Loading('hide')
    };

    requirejs.onError = function (err) {
        console.log('jsS7SHY2QSQG! ' + err.requireType);
        console.log('modules: ' + err.requireModules);
        console.log((err.stack || err));
        // alert('An error occured while loading ' + err.requireModules + '. Please refresh this page.');
        setTimeout(function() {
            //location.reload();
        }, 5000);
        // throw err;
    };

    require.config({
        waitSeconds: 180,
        paths: {
            jquery: GetPathFormat('/Scripts/jquery.js?crc='),
            jqm: GetPathFormat('/Scripts3/others/jquery.mobile.js?crc='),
            'jquery.textchange': GetPathFormat('/Scripts3/others/jquery.textchange.js?crc='),
            'jquery.format': GetPathFormat('/Scripts3/others/jquery.format.js?crc='),
            barcodescanner: GetPathFormat('/Scripts/barcodescanner.js?crc='),
            cordova: GetPathFormat('/Scripts/cordova.js?crc='),
            html2canvas: GetPathFormat('/Scripts/html2canvas.js?crc='),
            'jquery.plugin.html2canvas': GetPathFormat('/Scripts/jquery.plugin.html2canvas.js?crc='),
            jqplot: GetPathFormat('/Scripts3/others/jquery.jqplot.min.js?crc='),
            barRenderer: GetPathFormat('/Scripts3/others/jqplot.barRenderer.min.js?crc='),
            pieRenderer: GetPathFormat('/Scripts3/others/jqplot.pieRenderer.min.js?crc='),
            categoryAxisRenderer: GetPathFormat('/Scripts3/others/jqplot.categoryAxisRenderer.min.js?crc='),
            pointLabels: GetPathFormat('/Scripts3/others/jqplot.pointLabels.min.js?crc='),
            cursor: GetPathFormat('/Scripts3/others/jqplot.cursor.min.js?crc='),
            highlighter: GetPathFormat('/Scripts3/others/jqplot.highlighter.min.js?crc='),
            dynatree: GetPathFormat('/Scripts3/others/jquery.dynatree.js?crc='),
            business: '/Scripts3/Offline/Business/Business.js?crc=',
            mobile: '/Scripts3/common/mobile.js?crc=',
            promise: '/Scripts3/others/requirejs-promise',
            qrcode: GetPathFormat('/Scripts3/common/qrcode.js?crc='),
            jasmine: '/Scripts3/others/jasmine-2.0.0/jasmine.js?crc=',
            jasmineHtml: '/Scripts3/others/jasmine-2.0.0/jasmine-html.js?crc=',
            jasmineBoot: '/Scripts3/others/jasmine-2.0.0/boot.js?crc=',
            tablesort: '/Scripts3/others/stupidtable.js?crc=',
            "datatables.net": '/Scripts/jquery.dataTables.min.js?crc=',
            "datatables.net-buttons": '/Scripts/jquery.dataTables.buttons.flash.min.js?crc=',
            "datatables.net-fixedHeader": '/Scripts/dataTables.fixedHeader.min.js?crc=',
            'bootstrap-tooltip': '/Content/bootstrap-5.0.0/js/popper.min',
            'imagepreview': '/Scripts/imagepreview.min',
            'bootstrapjs': '/Content/bootstrap-5.0.0/js/bootstrap.bundle',
            'chartjs': '/Scripts/chart.min',
            'datalabels': '/Scripts/chartjs-plugin-datalabels',
            'moment': '/Scripts/moment.min'
        },
        map: {
            datalabels: {
                "chart.js": "chartjs"
            }
        },

        shim: {
            jqm: ['mobileinit'],
            'jquery.textchange': ['jquery'],
            'jquery.format': ['jquery'],
            barcodescanner: ['cordova'],
            html2canvas: ['jquery'],
            'jquery.plugin.html2canvas': ['html2canvas'],
            jqplot: ['jquery'],
            barRenderer: ['jqplot'],
            pieRenderer: ['jqplot'],
            categoryAxisRenderer: ['jqplot'],
            pointLabels: ['jqplot'],
            cursor: ['jqplot'],
            highlighter: ['jqplot'],
            dynatree: ['jquery'],
            qrcode: ['jquery'],
            jasmineHtml: ['jasmine'],
            jasmineBoot: ['jasmineHtml'],
            tablesort: ['jquery'],
            'bootstrap-tooltip': ['jquery'],
            'bootstrapjs': ['jquery'],
            'datalabels': ['chartjs']
        }
    }); // require.config

    (function Init() {

        var pIP;

        console.table ? null : console.table = function(){};

        var pNeeded = ['jqm',
            GetPathFormat('/Scripts3/common/dom.js?v=' + assetversion),
            GetPathFormat('/Scripts3/common/proj.js?crc=' + assetversion),
            GetPathFormat('/Scripts3/common/security.js?crc='),
            GetPathFormat('/Scripts3/common/security_offline.js?crc='),
            GetPathFormat('/Scripts/sources/LazyUpdate/LazyUpdate.js?crc='),
            'jquery.textchange',
            'jquery.format',
            '/Scripts3/Deferred.js?crc=',
            '/Scripts3/NewMessageBox.js?crc=' + assetversion // do not promise-require NewMessageBox. signinInfo will not load if NewMessageBox is promise-require
            ];

        if (location.protocol.indexOf('file') == 0) {
            pNeeded.push('barcodescanner');
        };

        window.OfflineMode = false;
        window.InAndroid = false;
        window.InWinRt = false;
        window.IsMobile = false;
        if (window.Android != undefined) {
            InAndroid = true;
        }

        // window.external.notify is always undefined
//        if ((window.external != undefined)
//            && (window.external.notify != undefined))
        if (window.external != undefined)
            try {
                // window.external.notify(); // no argument results in exception
                window.external.notify('');
                InWinRt = true;
            } catch (e) {
            }

        window.init = {
            // the app compiler will replace the following line to the correct server
            server: ''

        };
        if (GetUrlParameters().IsMobile == '0') {
            localStorage.removeItem('InWindows');
            localStorage.removeItem('IsMobile');
        } else if (GetUrlParameters().IsWindows == '0') {
            localStorage.removeItem('InWindows');
        } else if (GetUrlParameters().IsWindows == '1') {
            window.InWindows = true;
        } else if (localStorage.InWindows) {
            window.InWindows = true;
        }

        pIP = GetUrlParameters().IP;
        if (pIP != undefined)
            localStorage.IP = pIP;

        if ((InAndroid)
            || (InWinRt)
            || (GetUrlParameters().IsMobile == '1')
            || localStorage.IsMobile == '1'
            || window.InWindows) {

            window.IsMobile = true;
            pNeeded.push('mobile');
            localStorage.IsMobile = '1';
            OfflineMode = true;

            require(['/Scripts3/common/common.js?v=' + assetversion,
                'mobile'], function () {

                    if (InAndroid || InWinRt) {
                        // if offline, this is mobiz://app;  no trailing slash
                        // if online, this is the 
                        init.server = Android.GetServer();
                    };
                    window.DebugMode = CBool(Android.DebugMode());
                    window.IsOfflineMode = CBool(Android.IsOfflineMode());
                    // bookmark: 2015_12_28__16_01_57_6555: new database naming convention
                    // if (window.DebugMode)
                        window.NewDatabaseNaming = true;
                });
        };

        define('common', pNeeded, function () {
            if (window.InWindows) {
                window.IsMobile = true;
                localStorage.InWindows = 1;
                pIP = GetUrlParameters().IP;
                if (pIP == undefined)
                    pIP = localStorage.IP;
                else
                    localStorage.IP = pIP;
                if (pIP == undefined)
                    pIP = 'http://localhost:8080';
                if (!pIP.startsWith('http'))
                    pIP = 'http://' + pIP;
                init.server = pIP;
            }

            $('body').show();
            if (InAndroid)
                Android.HideSplash();
        }
        );

        if (window.SubMain != undefined)
            require(['common'], SubMain);

    })();          // function Init

    define('mobileinit', ['jquery'], function () {
        $.support.cors = true;
        $(document).bind("mobileinit", function () {
            // http://stackoverflow.com/a/7076422/334045

            var pThemeLetter = 'd';

            //    // Navigation
            //    $.mobile.page.prototype.options.backBtnText = "Go back";
            //    $.mobile.page.prototype.options.addBackBtn      = true;
            //    $.mobile.page.prototype.options.backBtnTheme    = pThemeLetter;

            // Page
            $.mobile.page.prototype.options.headerTheme = pThemeLetter;  // Page header only
            $.mobile.page.prototype.options.contentTheme = pThemeLetter;
            $.mobile.page.prototype.options.footerTheme = pThemeLetter;

            // Listviews
            //    $.mobile.listview.prototype.options.headerTheme = "a";  // Header for nested lists
            //    $.mobile.listview.prototype.options.theme           = "c";  // List items / content
            //    $.mobile.listview.prototype.options.dividerTheme    = "d";  // List divider

            //    $.mobile.listview.prototype.options.splitTheme   = "c";
            //    $.mobile.listview.prototype.options.countTheme   = "c";
            //    $.mobile.listview.prototype.options.filterTheme = "c";
            //    $.mobile.listview.prototype.options.filterPlaceholder = "Filter data...";

            $.mobile.loadingMessageTheme = pThemeLetter;
            //    $.mobile.page.prototype.options.addBackBtn  = true;
            $.mobile.listview.prototype.options.filterPlaceholder = 'Search within results';
            $.mobile.ajaxEnabled = false;
            $.mobile.defaultDialogTransition = 'none';
            $.mobile.defaultPageTransition = 'none';
            $.mobile.allowCrossDomainPages = true; // dli ko sure kng naa ba ni pulos o wla
        });
    });

} // END if (window.proj == undefined)