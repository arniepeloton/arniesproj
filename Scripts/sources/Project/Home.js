$(function () {

    var pViewPort = $('.divSlider:first').children('div:first');
    pViewPort.Slider({
        Container: pViewPort.children('div:first'),
        Style: 'horizontal',
        Buttons: $('.divSlideButton'),
        ButtonClass: 'divSlideButton',
        ButtonActiveClass: 'divSlideButtonActive'
    });

});

