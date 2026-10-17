require([
    'common'
], function () {
    $(document).ready(function () {

        $('.btnRentableAsset').click(function () {
            $('#popFilterBy').popup('open');
        })

        $('input[name="radRateCard"]').change(function () {
            $('#txtNo').val('');
        })

        $('#btnRateCardOk').click(function () {
            $('#popFilterBy').popup('close');
            $('#divCart').pageshow();
        })

        $('#btnRateCardCancel').click(function () {
            $('#popFilterBy').popup('close');
        })

        AjaxSend({
            ExactUrl: '/SalesMenu/Header?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile,
            type: 'GET',
            dataType: 'text',
            success: function (vData) {
                $('#divHeader').empty();
                $('#divHeader').append(vData).trigger('create');
            }
        });

        AjaxSend({
            ExactUrl: '/SalesMenu/Cart?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile,
            type: 'GET',
            dataType: 'text',
            success: function (vData) {
                $('#divCart').empty();
                $('#divCart').append(vData).trigger('create');
                $('#divCart').append($('<button>')
                        .append(
                            $('<h3>').text('Back')
                        )
                        .click(function () {
                            $('#divRentableAssets').pageshow();
                        })
                    )
            }
        });
    })
})