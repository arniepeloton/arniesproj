require(['common'
    ], function() {

    $(function () {

        var pParams = GetUrlParameters();

        $('#lnkUrl')
            .text(pParams.url)
            .attr('href', pParams.url);
        Android.HideSplash();

    });
});