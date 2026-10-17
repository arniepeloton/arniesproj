require(['common', GetPathFormat('/Scripts/paginathing.js')], function () {
    $('document').ready(function () {
        GetNewCompany();

        var domain = (location.hostname == 'localhost' ? 'http://192.168.1.110:8080' : 'http://app.mobiz-apps.com');

        $("#menu-toggle").click(function (e) {
            e.preventDefault();
            $("#wrapper").toggleClass("toggled");
        });

        $('#btnDashboard').click(function () {
            GetNewCompany();
        })

        $('#btnLogout').click(function () {
            location.href = '/MobizPortal/Logout';
        })

        $('#txtSearch').keypress(function (e) {
            if(e.which == 13){
                GetNewCompany();
            }
        })

        function GetNewCompany() {
            var tbodyNewCompany = $('#tbodyNewCompany')

            tbodyNewCompany.empty().append($('<tr>')
                    .append($('<td colspan="6" style="text-align:center;">').text('Loading ...'))
                );

            AjaxSend({
                url: 'MobizPortal/GetNewCompany',
                data:{
                    Search: $.trim($('#txtSearch').val())
                },
                success: function (e) {
                    var vRows = e.Results.Rows;
                    
                    tbodyNewCompany.empty();

                    if(vRows.length > 0){
                        for (var i = 0; i < vRows.length; i++){
                            tbodyNewCompany.append($('<tr>')
                                    .append($('<td style="color:#28a745">').append(vRows[i].TransferStatus == 'Pending' ?
                                            $('<button data-role="none" class="btn btn-danger btn-sm">')
                                                .text('Activate')
                                                .data('data', vRows[i])
                                                .click(function (e) {
                                                    var btn = $(this);
                                                    var data = btn.data('data');

                                                    $('#modalMessage').modal('show');
                                                    $('#lblCompanyName').text(data.CompanyName);

                                                    $('#btnCreateDatabaseNow').unbind('click').click(function () {
                                                        $('#modalMessage').modal('hide');
                                                        btn.text('Activating').attr('disabled', true);

                                                        AjaxSend({
                                                            url: 'MobizPortal/Activate',
                                                            data: {
                                                                PeopleID: data.SuperAdminID
                                                            },
                                                            success: function (e) {
                                                                if (e.Results == 'confirmed') {
                                                                    AjaxSend({
                                                                        ExactUrl: domain + '/Subscription/DownloadNewSubscriber',
                                                                        success: function (e) {
                                                                            //console.log('okay')
                                                                        }
                                                                    })

                                                                    setTimeout(FollowUpStatusFromServer(), 3000);
                                                                } else {
                                                                    btn.text('Activate').removeAttr('disabled');
                                                                    $('#divToastMsg').text(e.Results);
                                                                    $('#divToast').toast('show')
                                                                }
                                                            }
                                                        })
                                                    })
                                                })
                                            : 'Activated'))
                                    .append($('<td>').text((i + 1) + '.'))
                                    .append($('<td>').text(vRows[i].CompanyName))
                                    .append($('<td>').text(vRows[i].Administrator))
                                    .append($('<td>')
                                        .append($('<span class="fa fa-envelope-o">'))
                                        .append(': ' + vRows[i].EmailAddress)
                                        .append($('<br>'))
                                        .append($('<span class="fa fa-phone">'))
                                        .append(': ' + vRows[i].MobileNo)
                                    )
                                    .append($('<td>').text(vRows[i].SubscriptionDate))                                    
                                )
                        }

                        tbodyNewCompany.parent().next().remove()
                        tbodyNewCompany.paginathing({
                            perPage: 10,
                            containerClass: 'panel-footer',
                            limitPagination: (vRows.length > 100 ? 10 : false),
                            insertAfter: '#tblNewCompany',
                            ulClass: 'pagination justify-content-center'
                        })
                    } else {
                        tbodyNewCompany.append($('<tr>')
                            .append($('<td colspan="6" style="text-align:center;">').text('No result(s) found'))
                        );
                    }
                }, error: function (e) {
                    console.log('error')
                }
            })
        }

        function FollowUpStatusFromServer() {
            AjaxSend({
                ExactUrl: domain + '/Subscription/GetStatus',
                success: function (e) {
                    console.log(e.Status)
                    if (CStr2(e.Status) == 'Success' || CStr2(e.Status) == '') {
                        GetNewCompany();
                    } else {
                        setTimeout(FollowUpStatusFromServer(), 3000);
                    }
                }
            })
        }
    })
})