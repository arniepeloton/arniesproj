require(['common'], function () {
    $(document).ready(function () {
        var arrCategoryList = [
            {
                Name: 'T2', Tenants: [
                    { Name: 'Jose Cruz' },
                    { Name: 'Maria Remedios' },
                    { Name: 'Rico Reyes' },
                    { Name: 'Jacinta Dimagiba' }
                ]
            },
            {
                Name: 'T3', Tenants: [
                      { Name: 'Kim Diaz' },
                      { Name: 'Angelo Marquez' },
                      { Name: 'Flordeliza Martes' },
                      { Name: 'Joseph Tan' }
                ]
            },
            { Name: 'SF' },
            { Name: 'RP' },
            { Name: 'RPA' },
            { Name: 'YL' },
            { Name: '8A' },
            { Name: 'SP' },
            { Name: 'PP' },
            { Name: 'FM' },
            { Name: 'RM' },
            { Name: 'MF' },
            { Name: 'MFA' },
            { Name: 'MB' },
            { Name: 'NP' },
            { Name: 'MFC' },
            { Name: 'NJFS' },
            { Name: 'MLM' },
            { Name: 'Others' }
        ]

        LoadRequiredData();

        $('body')
        .on('click', '#btnTicketsBackToMenu', function () {
            location = MergeUrlParameters({
                IsMobile: 1
            }, '/People/PeopleMenu')
        })
        .on('click', '.btnTicketsBackToMain', function () {
            $('#pageTicketMain').pageshow();
        })
        .on('click', '#btnPaymentBack', function () {
            $('#pageTenant').pageshow();
        })
        .on('click', '#btnTicketsPayNow', function () {
            $('#pagePaymentSuccess').pageshow();
        })

        function LoadRequiredData() {
            DisplayCategories();
            $('#pageTicketMain').pageshow();
        }

        function DisplayCategories() {
            var divCategoryList = $('#divCategoryList');

            divCategoryList.empty();

            if (arrCategoryList.length > 0) {
                for (var i = 0; i < arrCategoryList.length; i++) {
                    divCategoryList.append($('<div class="alert alert-neutral" role="alert">')
                            .append($('<div style="display:table; width:100%; vertical-align:middle;">')
                                .append($('<div style="display: table-cell;">').text(arrCategoryList[i].Name))
                                .append($('<div style="display:table-cell; text-align:right; width: 25px; vertical-align:middle;">')
                                    .append($('<span class="glyphicon glyphicon-chevron-right">'))
                                )
                            )
                            .data('data', arrCategoryList[i])
                            .click(function () {
                                var data = $(this).data('data');
                                DisplayTenants(data);
                                $('#lblAreaName').text(data.Name);
                                $('#pageTenant').pageshow();
                            })
                        )
                }
            }
        }

        function DisplayTenants(data) {
            var divListOfTenants = $('#divListOfTenants');

            divListOfTenants.empty();

            if (data.Tenants.length > 0) {
                for (var i = 0; i < data.Tenants.length; i++) {
                    divListOfTenants.append($('<div class="alert alert-neutral" role="alert">')
                            .append($('<div style="display:table; width:100%; vertical-align:middle;">')
                                .append($('<div style="display: table-cell;">').text(data.Tenants[i].Name))
                                .append($('<div style="display:table-cell; text-align:right; width: 25px; vertical-align:middle;">')
                                    .append($('<span class="glyphicon glyphicon-chevron-right">'))
                                )
                            )
                            .data('data', data.Tenants[i])
                            .click(function () {
                                var data = $(this).data('data');

                                $('#lblTenantName').text(data.Name);
                                $('#pageTenantPayment').pageshow();
                            })
                        )
                }
            }
        }
    })
})