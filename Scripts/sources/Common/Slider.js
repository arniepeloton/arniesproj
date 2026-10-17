(function ($) {
    // the object should be the viewport DIV
    $.fn.Slider = function(vParams) {
        
        var mDefaults = {
            Container: null,    // div containing the inner divs
            Interval: 3000,      // (optional) in milliseconds
            // values for Style:
            // 'horizontal' (default), 'vertical', 'fade'
            Style: 'horizontal',    // optional
            Buttons: null,          // optional
            ButtonClass: null,      // optional
            ButtonActiveClass: null,
            Auto: true
        };
        var $this = $(this)
            .css('overflow', 'hidden');
        var mTimeout;
        var mSlide;
        var mSlides;

        $.extend(mDefaults, vParams);
        mSlides = mDefaults.Container.children();
        switch (mDefaults.Style) {
            case 'vertical':
                SetHeight();
                break;

            case 'fade':
                $this.css('position', 'relative');
                mSlides.css('position', 'absolute');
                mSlides.css('left', 0);
                mSlides.css('top', 0);
                mSlides.css('opacity', 0);
                $(mSlides[0]).css('opacity', 1);
                break;

            default: // horizontal
                SetWidth();

        }; // END switch (mDefaults.Style)

        if (mDefaults.Buttons) {
            mDefaults.Buttons.click(function(e) {
                SetSlide($(this).index());
            });        
        };
        SetSlide(0);

        function SlideNext() {

            var pIndex;

            if (mSlide == undefined) mSlide = $(mSlides[0]);
            pIndex = mSlide.index();
            if (mSlides.length - 1 < pIndex + 1)
                pIndex = 0;
            else
                pIndex++;
            SetSlide(pIndex);

        }; // END SlideNext

        function SetSlide(vIndex) {

            var pSlide;
            var pTop;
            var pDistance ;
            var pAnimate;
            var pSubject; // element to animate
            var pCallback;

            pSlide = $(mSlides[vIndex]);
            clearTimeout(mTimeout);
            mDefaults.Container.stop(true, true);
            switch (mDefaults.Style) {
                case 'vertical':
                    pSubject = mDefaults.Container;
                    pDistance = pSlide.offset().top - mDefaults.Container.offset().top;
                    pAnimate = { 'margin-top': -pDistance };
                    break;

                case 'fade':
                    pSubject = pSlide;
                    mSlides.css('opacity', 0);
                    mSlides.css('z-index', 0);
                    if (mSlide) mSlide.css('opacity', 1);
                    pSlide.css('z-index', 1);
                    pAnimate = {'opacity': 1};
                    break;

                default: // horizontal
                    pSubject = mDefaults.Container;
                    pDistance = pSlide.offset().left - mDefaults.Container.offset().left;
                    pAnimate = { 'margin-left': -pDistance };

            }; // END switch (mDefaults.Style)

            // easeInOutBack
            mSlide = pSlide;
            if (mDefaults.Auto) 
                pCallback = function () {
                    mTimeout = setTimeout(
                        function() {
                            SlideNext();
                        }, mDefaults.Interval);  // comment to prevent auto-slide
                };
            pSubject.animate(pAnimate, 1000, '', pCallback);

            if (mDefaults.Buttons) {
                if (mDefaults.ButtonClass) 
                    mDefaults.Buttons.addClass(mDefaults.ButtonClass);
                if (mDefaults.ButtonActiveClass) {
                    mDefaults.Buttons.removeClass(mDefaults.ButtonActiveClass);
                    if (mDefaults.Buttons[vIndex])
                        $(mDefaults.Buttons[vIndex]).addClass(mDefaults.ButtonActiveClass);
                };
            };
        }; // END SetSlide

        function SetWidth() {

            var pWidth = 0;

            mSlides.each(function (vIndex, vItem) {
                pWidth += $(vItem).width();
            });
            mDefaults.Container.width(pWidth);

        }; // END SetWidth

        function SetHeight() {
            
            var pHeight = 0;

            mSlides.each(function (vIndex, vItem) {
                pHeight += $(vItem).height();
            });
            mDefaults.Container.height(pHeight);
        }; // END SetHeight
    }; // $.fn.Slider
})(jQuery);