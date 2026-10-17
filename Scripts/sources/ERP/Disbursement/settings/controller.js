require(['require'],
function () {
    $(document).ready(function () {
        LoadRequiredData();

        $('#btnFAAddEmployee').click(function () {
            BrowsePopup('LibraryOrganizationEmployee', 'Search Employee', [
                {
                    SelectMultiple: true,
                    AutoSearch: true,
                    DisplayText: [{ Column: 'PeopleName' }],
                    Click: function (e) {
                        SaveEmployeesToApproveFA(e);
                    }
                }
            ])
        })

        $('#btnDSRemarksMessages').click(function () {
            Bs.Loading('show');
            require(['promise!/Scripts/sources/ERP/RemarksMessages/controller.js?v=' + assetversion], function () {
                Bs.Loading('hide');
                ERP.RemarksMessages.main.controller.show({ module: 'disbursement' });
            })
        })

        function SaveEmployeesToApproveFA(data) {
            AjaxSend({
                url: 'Expenses/SaveEmployeesToApproveFA',
                data: { data: data },
                success: function (e) {
                    if(e.Results == 'saved'){
                        GetEmployeesToApproveFA();
                        WebToast('Successfully added', 'top');
                    } else {
                        NewMessageBox(e.Results, 'Message');
                    }
                }
            })
        }

        function GetEmployeesToApproveFA() {
            AjaxSend({
                url: 'Expenses/GetEmployeesToApproveFA',
                success: function (e) {
                    var vRows = e.Results.Rows;
                    var listFundAdjustmentsToApprove = $('#listFundAdjustmentsToApprove');

                    listFundAdjustmentsToApprove.empty();

                    if (vRows.length > 0) {
                        for (var i = 0; i < vRows.length; i++){
                            listFundAdjustmentsToApprove.append($('<li class="list-group-item">')
                                .append($('<div style="display:table; width:100%;">')
                                    .append($('<div style="display:table-cell; vertical-align:middle;">')
                                        .append(vRows[i].PeopleName)
                                    )
                                    .append($('<div style="display: table-cell; text-align:right; padding:5px; vertical-align:middle; width:50px;">')
                                        .append($('<button data-role="none" class="btn btn-danger">')
                                            .data('data', vRows[i])
                                            .append($('<span class="bi bi-trash">'))
                                            .click(function (e) {
                                                e.preventDefault();
                                                e.stopPropagation();

                                                var dis = $(this);
                                                var data = dis.data('data');

                                                NewMessageBox('Are you sure you want to remove record <b>' + data.PeopleName + '</b>?', 'Message', [
                                                    {
                                                        Text: 'Yes', Click: function () {
                                                            
                                                        }
                                                    }, { Text: 'No' }
                                                    ])
                                            })
                                        )
                                    )
                                )
                                //.hover(function () {
                                //    $(this).find('button').css('visibility', 'visible');
                                //}, function () {
                                //    $(this).find('button').css('visibility', 'hidden');
                                //})
                            )
                        }
                    }
                }
            })
        }

        function LoadRequiredData() {
            GetEmployeesToApproveFA()
        }
    })
})