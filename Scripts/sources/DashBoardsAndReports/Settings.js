require([
    'common',
    ], function () {
        var ID = '';

        $(function () {
            GetOrganizationProductFeatures();

            $('#btnDone').click(function () {
                history.back();
            });

            $('#btnAddCompanyProductFeatures').click(function () {
                $('#panProductFeatures').popup('open');
                GetAllProductFeatures();
            });

            $('#btnAddSelectedFeatures').click(function () {
                var Features = '';
                var i = 0;
                $('#tblProductFeature tr td input').each(function (m_index) {
                    if ($(this).attr("checked") == "checked") {
                        Features += $(this).attr("ID") + "|";
                    }
                });

                data = { Features: Features
                };

                AjaxSend({
                    url: 'DashboardsAndReports/SaveFeatures',
                    data: data,
                    success: function (vData) {

                    }
                }); //end ajaxsend
                $('#panProductFeatures').popup('close');
                GetOrganizationProductFeatures();
            });
        });

        function GetAllProductFeatures() {
            data = { OrgID: sessionStorage.OrgID };

            AjaxSend({
                url: 'DashboardsAndReports/GetAllProductFeatures',
                data: data,
                success: function (vData) {
                    if (vData.Results.Rows.length > 0) {
                        DisplayAllProductFeatures(vData.Results.Rows);
                    }
                }
            });
        }

        function GetOrganizationProductFeatures() {
            AjaxSend({
                url: 'DashboardsAndReports/GetOrganizationProductFeatures',
                success: function (vData) {
                    if (vData.Results.Rows.length > 0) {
                        DisplayOrganizationProductFeatures(vData.Results.Rows);
                    }
                    else {
                        $('#lstOrganizationProductFeature').empty();

                        pLV = $('#lstOrganizationProductFeature');
                        pLV.empty();

                        pItem = $('<li data-role=list-divider>')
                                .append($('<a>')
                                    .append($('<h3>')
                                        .text('Product Features')
                                    )
                                )
                        pLV.append(pItem);

                        try { 
                            $('#lstOrganizationProductFeature').listview('refresh');
                        }
                        catch (e) { 
                        }
                    }
                }
            });
        }

        function DisplayOrganizationProductFeatures(vRows) {
            pLV = $('#lstOrganizationProductFeature');
            pLV.empty();

            pItem = $('<li data-role=list-divider>')
                    .append($('<a>')
                        .append($('<h3>')
                            .text('Product Features')
                        )
                    )
            pLV.append(pItem);


            for (i = 0; i <= vRows.length - 1; i++) {
                pItem = $('<li>')
                    .append($('<a>')
                        .attr('ID', vRows[i].ID)
                        .attr('href', '#')
                        .append($('<h3>')
                            .text(vRows[i].Label)
                        )
                    )
                    .append($('<a>')
                        .attr('ID', vRows[i].ID)
                        .click(function () {
                            ID = $(this).attr('ID');
                            MessageBox("Are you sure you want to remove this feature?",
                               "Dashboard Settings",
                                [
                                    { Text: 'Yes',
                                        Click: function () {
                                            RemoveFeature();
                                            $('.ui-dialog').dialog('close');
                                        }
                                    },
                                    { Text: 'No'
                                    }
                                ]
                            );
                        })
                    )
                pLV.append(pItem);
            }

            try {
                pLV.listview('refresh');
            }
            catch (e) {
                pLV.listview();
            }

        }

        function RemoveFeature() {
            data = { ID: ID };

            AjaxSend({
                url: 'DashboardsAndReports/RemoveFeature',
                data: data,
                success: function (vData) {
                    if (vData.Results == 'True') {
                        GetOrganizationProductFeatures();
                    }
                }
            });
        }

        function DisplayAllProductFeatures(vRows) {
            var pLV = $("#tblProductFeature");

            $('#tblProductFeature tr').empty();

            var m_Ctr = 0;

            for (var i = 0; i <= vRows.length - 1; i++) {

                var checked = undefined;

                if ((i % 3) == 0) {
                    pItem = $('<tr>')
                }
                pItem = pItem
                    .append($('<td>')
                        .append($('<input type=checkbox data-theme=e />')
                            .attr('ID', vRows[i].ID)
                            .attr('checked', checked)
                            .attr('data-mini', 'true')
                            .data('data', vRows[i])
                            .click(function () {

                            })
                        )
                        .append($('<label>')
                            .attr('for', vRows[i].ID)
                            .text(vRows[i].Label)
                        )
                    )//td                    

                if (((i % 3) == 0) || (i == vRows.length - 1)) {
                    pLV.append(pItem);

                    $('#' + vRows[i].ID).checkboxradio();
                    if (i > 0) {
                        $('#' + vRows[i - 1].ID).checkboxradio();
                        $('#' + vRows[i - 2].ID).checkboxradio();
                    }
                }
            }

            //pLV.append(pItem);

            //$('#lstProductFeature').listview('refresh');
        }
    });