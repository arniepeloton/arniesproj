require(['common'
], function () {

    $(function () {

        window.mobile.DontFollowUp = true;

        window.MasterFrame = {
            Contents: null,
            OnLoad: function () {
            },
            Child$: null,
            $: $
        };

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

        $('#iFrameMain').load(function (e) {
            console.log('js7P4V527T9R! iFrameMain loaded');
        }).attr('src', pUrl)
        .css('float', 'left');

        $('#iFrameMain2').load(function (e) {
            sessionStorage.OrgID = 'cfe39df6-8809-4973-9515-810df75aed12';
            console.log('js2AE8ZJ65V9! iFrameMain2 loaded');
        }); //.attr('src', '/People/PeopleMenu?IsMobile=1#pagePopUpMenuTroubleShootingTools#FlushDeviceData');

//        $(window).scroll(function (e) {
//            $(window).scrollTop(0);
//        });

        function onResize() {

            var pPage = $($('[data-role=page]')[0]);
            pPage.css('padding', 0);

            var pIframe = $('#iFrameMain');
            var pIframe2 = $('#iFrameMain2');

//            pIframe.width($('body').innerWidth());
//            pIframe.height($('body').innerHeight());

//            pIframe.width($('body').innerWidth() / 2);
//            pIframe.height($('body').innerHeight());

//            pIframe2.width($('body').innerWidth() / 2);
//            pIframe2.height($('body').innerHeight());
        }; // END ResizeIframe

        onResize();

        $(window).on('resize', onResize);

        window.LoadFlushing = function() {
            
        };

    }); // END $(function () {

});       // END require