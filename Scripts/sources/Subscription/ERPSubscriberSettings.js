require(['common'],
function () {
    $(document).ready(function () {
        LoadRequiredData();

        $('#txtSearchMobizSubscriber').keydown(function (e) {
            if (e.which == 13) {
                GetMobizSubscriber($.trim($(this).val()));
            }
        })
    })

    function LoadRequiredData() {
        $('#mainContainer').css('margin-top', $('#divHeaderForProjects2').height());
        GetMobizSubscriber('');
        var clear = setInterval(function () {
            if (typeof AddBreadCrumb != 'undefined') {
                AddBreadCrumb({
                    text: 'Home',
                    Click: function () {
                        $('#pageMainErpSettings').pageshow();
                    }
                }, '.ItemsBreadCrumbs');
                clearInterval(clear);
            }
        }, 100)
    }

    function GetMobizSubscriber(s) {
        var container = $('#divOrgList');

        container.empty().html('Getting data. Please wait ...');

        AjaxSend({
            url: 'Subscription/GetSubscriberList',
            data: { keywords: s },
            success: function (e) {
                
                var row = $('<div class="row">'), ctrGrid = 0;
                var arrGrid = e.Results.Rows;

                container.empty();

                if(arrGrid.length > 0){
                    for (var i = 0; i < arrGrid.length; i++) {

                        row.append($('<div class="col-xs-12 col-sm-6 col-md-3">')
                                .append($('<div class="orgGrid">')
                                    .data('data', arrGrid[i])
                                    .append($('<span>')
                                        .text(arrGrid[i].CompanyName)
                                    )
                                    .click(function () {
                                        var data = $(this).data('data');
                                        $('#pageSettings').pageshow();

                                        AddBreadCrumb({
                                            text: data.CompanyName,
                                            Click: function () {
                                                $('#pageSettings').pageshow();
                                            }
                                        }, '.ItemsBreadCrumbs')
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
                } else {
                    container.html('No Result(s) found');
                }                
            }
        })
    }
})