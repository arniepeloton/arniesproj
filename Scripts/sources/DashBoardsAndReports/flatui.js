require([
    'common'
    ], function () {

        $(document).ready(function () {

            $(window).resize(function () {
                var ns = $('#NetSalesHeight').height(),
                        gs = $('#GrossSalesHeight').height(),
                        et = $('#ExtrasTotalHeight').height();
                if ($(window).width() >= 576) {
                    if (ns > gs) {
                        $('#GrossSalesHeight, #ExtrasTotalHeight').css('height', ns);
                    }
                    else if (ns < gs) {
                        $('#NetSalesHeight, #ExtrasTotalHeight').css('height', gs);
                    }
                    else if (et > ns || et > gs) {
                        $('#NetSalesHeight, #GrossSalesHeight').css('height', et);
                    }
                    else if (ns == gs) {
                        $('#NetSalesHeight, #ExtrasTotalHeight').css('height', gs);
                    }
                }
                else {
                    $('#NetSalesHeight, #GrossSalesHeight, #ExtrasTotalHeight').css('height', 'auto');
                }
            })

            $('#btnSearchMobile').click(function () {
                $('.contentSearchEngine').slideToggle('fast', function () {
                    $('.txtSearch').focus();

                });
                $('#btnSearchMobile').toggleClass('btn-a-active');
            })
        })
    });