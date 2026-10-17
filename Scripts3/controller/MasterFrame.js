define(['common'], function () {

    //var mDeferred = $.Deferred();

    (function staticInit() {

    })();

    CreateStringObject('App.controller.MasterFrame', null, function () {

        var mOut = {};

        mOut.hasBackgroundIframe = function() {

            return window.parent.$('iframe#iFrameMain2').length > 0;

        };

        mOut.runExporter = function() {
            
            window.parent.$('iframe#iFrameMain2').attr('src', '/People/PeopleMenu?IsMobile=1#pagePopUpMenuTroubleShootingTools#FlushDeviceData');

        };

        return mOut;

    }); // END CreateStringObject('App.controller.SignaturePage', null, function() {

    App.controller.MasterFrame = new App.controller.MasterFrame(); // singleton

    //return mDeferred.promise();

});