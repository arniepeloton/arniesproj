require(['common'],
function () {

    $('#divPopupUrlPage').append($('<div id="popupMoreDetails" class="modal fade" tabindex="-1">')
            .append($('<div class="modal-dialog modal-fullscreen">')
                .append($('<div class="modal-content">')
                    .append($('<div class="modal-header">')
                        .append($('<h5 class="modal-title" id="moredetailsTitle">'))
                        .append('<button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>')
                    )                    
                    .append($('<div class="modal-body" id="divMoreDetailsContent">'))
                    .append($('<div class="modal-footer">')
                        .append('<button data-role="none" type="button" class="btn btn-secondary btn-lg" data-dismiss="modal">Close</button>')
                    )
                )
            )
        )

    window.RenderGrid = function (containerDiv, arrGrid) {
        var container = $(containerDiv);
        var row = $('<div class="row">'), ctrGrid = 0;

        container.empty();

        for (var i = 0; i < arrGrid.length; i++) {

            row.append($('<div class="col-xs-12 col-sm-6 col-md-3">')
                    .append($('<div class="dashboardGrid">')
                        .data('data', arrGrid[i])
                        .append($('<div class="gHeader">')
                            .html(arrGrid[i].Header)
                        )
                        .append($('<div class="gBody">')
                            .html(arrGrid[i].Body)
                        )
                        .append($('<div class="gFooter">')
                            .html(CStr2(arrGrid[i].Footer))
                        )
                        .click(function () {
                            var data = $(this).data('data');
                            if (data.Click != undefined) {
                                data.Click(data);
                                $('#popupMoreDetails').show();
                            }
                        })
                    ).append('<br><br>')
                )

            ctrGrid += 1;

            if (ctrGrid == 4 || (i + 1) == arrGrid.length) {
                container.append(row);
                row = $('<div class="row">');
                ctrGrid = 0;
            }
        }
    }

    window.PopupUrlPage = function (title, content, callback) {
        $('#popupMoreDetails').modal('show');
        $('#popupMoreDetails').find('#moredetailsTitle').html(title);
        $('#popupMoreDetails').find('.modal-body').empty()
        $('#popupMoreDetails').find('.modal-body').append(content)

        if (typeof callback != "undefined") {
            callback()
        }
    }
})