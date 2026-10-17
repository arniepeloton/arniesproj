$(function () {
    var txtDateFrom = $('[name=txtDateFrom]')
        .datepicker({
            changeMonth: true,
            changeYear: true
        });

    var txtDateTo = $('[name=txtDateTo]')
        .datepicker({
            changeMonth: true,
            changeYear: true
        });

    var mDescriptionGroup = $('#HomeFaderDescriptionGroup');
    var mDescriptionHeight = mDescriptionGroup.height();
    
    $('#HomeFadeViewPort').hover(
        function (e) {
            mDescriptionGroup.stop();
            mDescriptionGroup.animate({'bottom': 0}, 'fast', 'easeInOutBack');
        }, HomeFadeViewPort_MouseOut
    );

    function HomeFadeViewPort_MouseOut() {
        mDescriptionGroup.stop();
        mDescriptionGroup.animate({'bottom': -mDescriptionHeight}, 'slow', 'easeInOutBack');
    };
    HomeFadeViewPort_MouseOut();

    $('#HomeFadeViewPort').Slider({
        Container: $('#HomeFadeContainer'),
        Style: 'fade',
        Buttons: $('#HomeFaderButtons').children('div'),
        ButtonClass: 'HomeFaderButton',
        ButtonActiveClass: 'HomeFaderButtonActive'
    });

    $('#HomeSlideViewPort').Slider({
        Container: $('#HomeSlideContainer'),
        Style: 'vertical',
        Buttons: $('#HomeSliderButtons').children('div'),
        ButtonClass: 'HomeFaderButton',
        ButtonActiveClass: 'HomeFaderButtonActive',
        Auto: false
    });
});