require(['common',
    '/Scripts3/AndroidJsTestIframeTestControl.js?crc='
], function () {

    $(function () {

        var mAutoTest = false;

        window.MasterFrame = {
            Contents: null,
            OnLoad: function () {
            },
            Child$: null
        };

        // start tab
        // http://stackoverflow.com/a/6850059/334045
        $(document).delegate('.ui-navbar ul li > a', 'click', function () {

            //un-highlight and highlight only the buttons in the same navbar widget
            $(this).closest('.ui-navbar').find('a').removeClass('ui-navbar-btn-active');

            //this bit is the same, you could chain it off of the last call by using two `.end()`s
            $(this).addClass('ui-navbar-btn-active');

            //this starts the same but then only selects the sibling `.content_div` elements to hide rather than all in the DOM
            $('#' + $(this).attr('data-href')).show().siblings('.content_div').hide();
        });
        $('.ui-navbar ul li > a:eq(0)').click();
        // end tab

        // determine the URL of the child
        var pSourceParameters = GetUrlParameters();
        var pDestinationParameters = {};
        if (pSourceParameters.IsMobile != undefined)
            pDestinationParameters.IsMobile = pSourceParameters.IsMobile;
        if (pSourceParameters.IsWindows != undefined)
            pDestinationParameters.IsWindows = pSourceParameters.IsWindows;
        if (pSourceParameters.IP != undefined)
            pDestinationParameters.IP = pSourceParameters.IP;
        if (pSourceParameters.NoFlushing != undefined)
            localStorage.NoFlushing = pSourceParameters.NoFlushing;
        var pUrl = MergeUrlParameters(pDestinationParameters, '/SignIn');
        mAutoTest =  CBool(pSourceParameters.AutoTest);

        $('iframe:first').load(function (e) {
            MasterFrame.Contents = $(e.target).contents();
            $('#spanUrl').text(decodeURIComponent(e.target.contentWindow.location.href));
            $(e.target.contentWindow).on('hashchange', function (e) {
                $('#spanUrl').text(decodeURIComponent(e.target.location.href));
            });
            if ($.isFunction(MasterFrame.OnLoad)) {
                function ChildLoaded() {
                    if (e.target.contentWindow.$ != undefined) {
                        MasterFrame.Child$ = e.target.contentWindow.$;
                        if ((MasterFrame.Child$.mobile != undefined) 
                            && (MasterFrame.Child$.mobile.activePage != undefined)) {

                            MasterFrame.OnLoad();
                            AutoTest();
                            return;
                        }
                    }
                    setTimeout(ChildLoaded, 300);
                } // END ChildLoaded
                setTimeout(ChildLoaded, 300);
            }
        }).attr('src', pUrl);

        $(window).scroll(function (e) {
            // $(window).scrollTop(0);
        });

        function ResizeIframe() {

            var pPage = $($('[data-role=page]')[0]);
            pPage.css('padding', 0);
            var pWindow = $(window);
            var pWindowHeight = pWindow.height();

            var pIframe = $('iframe:first');
            var pIframeHeight = pIframe.height();
            var pIframeBottom = pIframe.offset().top + pIframe.height();
            var pDiff = pWindowHeight - pIframeBottom - 30;
            pDiff = Math.max(pDiff, 10);
            pIframe.height(pIframeHeight + pDiff);

        }; // END ResizeIframe

        ResizeIframe();

        $(window).on('resize', ResizeIframe);

        function AutoTest() {
            
            if (!mAutoTest)
                return;
            mAutoTest = false;

            $('#butStartTest').click();

        } // END AutoTest

    }); // END $(function () {

});       // END require