require(['common', '/Content/Ecommerce/js/lightbox-2.11.1.js'], function () {
    $(document).ready(function () {
        var modal_imgviewer = $('<div class="modal" id="modal_imgviewer" tabindex="-1" role="dialog">')
                            .append($('<div class="modal-dialog" role="document">')
                                .append($('<div class="modal-content">')
                                    .append($('<div class="modal-body">')
                                        .append($('<div id="divPIMPicPreview" class="bxslider">'))
                                    )
                                    .append($('<div class="modal-footer">')
                                        .append($('<button data-role="none" class="btn btn-primary btn-less-width" data-dismiss="modal">').text('Close'))
                                    )
                                )
                            )
        var carousel = null;

        $('body')
            .append(modal_imgviewer)

        lightbox.option({
            'resizeDuration': 200,
            'wrapAround': true,
            'imageFadeDuration': 300,
            'alwaysShowNavOnTouchDevices': true
        })

        window.ImageViewer = {
            DefaultImage: function (src, data){
                return $('<img data-toggle="tooltip" data-placement="right" title="View Image" style="width:auto;">').attr('src', src)
                            .data('data', data)
                            .click(function () {
                                var data = $(this).data('data');

                                ImageViewer.ShowImages(data.ProductID, data.VendorID);
                            })
            },
            ShowImages: function (productid, vendorid) {
                AjaxSend({
                    url: 'Organization/GetPIMPictures',
                    data: { ProductID: productid, IsFromVendor: (vendorid == undefined ? false : true), VendorID: vendorid },
                    success: function (e) {
                        var vRows = e.Results.Rows;

                        var divPIMPicPreview = $('#divPIMPicPreview');

                        divPIMPicPreview.empty();

                        if (vRows.length > 0) {
                            for (var i = 0; i < vRows.length; i++) {
                                var srcBig = '/Content/MobizImagePIM/' + sessionStorage.OrgID + '/Big/' + (vRows[i].VendorID == undefined ? '' : vRows[i].VendorID + '/') + vRows[i].GroupImageID + '_big.jpg';

                                divPIMPicPreview.append($('<div>')
                                        .append($('<a data-role="none" data-lightbox="skupictures">')
                                            .attr('id', vRows[i].ID)
                                            .attr('href', srcBig)
                                            .append($('<img>')
                                            .data('data', vRows[i]).attr('src', srcBig)
                                        )
                                        )
                                    )
                            }
                            $('#' + vRows[0].ID).click();
                            return;
                            if (carousel) {
                                carousel.destroySlider()
                            }

                            carousel = $('#divPIMPicPreview').bxSlider({
                                auto: false,
                                autoControls: true,
                                stopAutoOnClick: true,
                                pager: true,
                                slideWidth: 400,
                                adaptiveHeight: true,
                                touchEnabled: true,
                                responsive: true,
                                startSlide: 0,
                                onSliderLoad: function () {
                                    $('#divPIMPicPreview').parent().css('height', '390px');
                                    $('#divPIMPicPreview > div').css('width', '390px');
                                }
                            });

                            setTimeout(function () {
                                carousel.goToSlide(0);
                                $('#modal_imgviewer').modal('show');
                            }, 200)
                        }
                    }
                })
            }
        }
    })
})