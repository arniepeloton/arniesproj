require([
    'common'
    ], function () {

        var ScheduleID = '';
        var IsNew = 0;
        var ID = '';

        $(function () {

            //            GetAllFowst();
            GetAllSchedule();
            GetAllEmployeeSchedule();

            if (GetUrlParameters().EmployeeID != undefined) {
                $('#txtEmployeeName').attr('name', GetUrlParameters().EmployeeID);
                $('#txtEmployeeName').val(GetUrlParameters().EmployeeName);
            }

            if (GetUrlParameters().FOWSTID != undefined) {
                $('#txtFOWST').attr('name', GetUrlParameters().FOWSTID);
                $('#txtFOWST').val(GetUrlParameters().FOWSTName);
            }

            $('#btnDone').click(function () {
                window.location = MergeUrlParameters({
                    OrgID: sessionStorage.OrgID,
                    IsMobile: CBool(IsMobile) ? 1 : 0
                }, '../Organization/OrganizationEmployee');
            });

            $('#txtName').keypress(function (e) {
                if (e.keyCode == 13) {
                    SearchEmployee();
                }
            });

            $('#btnAddScheduleToEmployee').click(function () {
                SaveEmployeeSchedule();
            });

            var m_date = new Date();

            $('#txtEffectivityDateFrom').DateCombo('val', $.format.date(new Date((m_date.getMonth() + 1) + '/' + (m_date.getDate() + 1) + '/' + m_date.getFullYear()), "MM/dd/yyyy"));
            $('#txtEffectivityDateto').DateCombo('val', $.format.date(new Date((m_date.getMonth() + 1) + '/' + (m_date.getDate() + 1) + '/' + m_date.getFullYear()), "MM/dd/yyyy"));

        });

        function SaveEmployeeSchedule() {

            if ($('#selSchedule').val() == '') {
                MessageBox("Please schedule for this employee.",
                            "Work Schedule",
                            [
                                { Text: 'Ok' }
                            ]
                        );
            }
            else {
                var IsInfinite = 0;

                if ($('#chkInfiniteEffectivityDate:checked').val() == 'on') {
                    IsInfinite = 1;
                }

                data = { PeopleID: $('#txtEmployeeName').attr('name'),
                    FOWSTID: $('#txtFOWST').attr('name'), //$('#selFowst').val(),
                    ScheduleID: $('#selSchedule').val(),
                    EffectivityDateFrom: $('#txtEffectivityDateFrom').DateCombo('val'),
                    EffectivityDateTo: $('#txtEffectivityDateto').DateCombo('val'),
                    IsInfiniteEffectivityDate: IsInfinite
                }

                AjaxSend({
                    url: 'Organization/SaveEmployeeWorkSchedule',
                    data: data,
                    success: function (vData) {
                        if (vData.Results.indexOf('Error')>=0) {
                            var pString = vData.Results.split(': ');

                            MessageBox(pString[1],
                            "Work Schedule",
                            [
                                { Text: 'Ok' }
                            ]
                        );
                        } else {
                            GetAllEmployeeSchedule();
                        }
                    }
                });
            }
        }

        function SearchEmployee() {

        }

        function DisplayAllSchedule(vRows) {
            pLV = $('#lstOrgEmployee');
            pLV.empty();

            for (var i = 0; i <= vRows.length - 1; i++) {

                pItem = $('<li>')
                    .append($('<a>')
                        .attr('ID', vRows[i].ID)
                        .attr('href', '#')
                        .append($('<h3>')
                            .text(vRows[i].WorkScheduleName)
                        )
                        .append($('<p>')
                            .text('FOWST: ' + vRows[i].FOWSTName + ' Effectivity Date: ' + vRows[i].EffectivityDateFrom + ' -' + vRows[i].EffectivityDateTo)
                        )
                        .click(function () {

                        })
                    )
                pLV.append(pItem);
            }
            pLV.listview('refresh');
        }

        function GetAllEmployeeSchedule() {
            var data = { PeopleID: GetUrlParameters().EmployeeID }
            AjaxSend({
                url: 'Organization/GetEmployeeSchedule',
                data: data,
                success: function (vData) {

                    var pLV = $('#lstOrgEmployeeScheduleList');
                    pLV.empty();

                    var vRows = vData.Results.Rows;

                    if (vRows.length > 0) {
                        pItem = $('<li>')
                                .attr('data-role', 'list-divider')
                                .append($('<h3>')
                                    .text('Employee Schedule List')
                            )
                        pLV.append(pItem);

                        for (var i = 0; i <= vRows.length - 1; i++) {
                            pItem = $('<li>')
                            .append($('<a>')
                                .attr('ID', vRows[i].ID)
                                .attr('href', '#')
                                .append($('<h3>')
                                    .text(vRows[i].WorkScheduleName)
                                )
                                .append($('<p>')
                                    .text(vRows[i].Description)
                                )
                            )
                            .append($('<a>')
                                .attr('ID', vRows[i].ID)
                                .click(function () {
                                    ScheduleID = $(this).attr("ID");
                                    MessageBox("Are you sure you want to remove this schedule?",
                                            "Employee",
                                            [
                                                { Text: 'Yes',
                                                    Click: function () {
                                                        RemoveEmployeeSchedule();
                                                        $('.ui-dialog').dialog('close');
                                                    }
                                                },
                                                { Text: 'No' }
                                            ]
                                        );
                                })
                            )
                            pLV.append(pItem);
                        }
                        pLV.listview('refresh');
                    }
                }
            })
        }

        function RemoveEmployeeSchedule() {
            data = { EmployeeScheduleID: ScheduleID }
            AjaxSend({
                url: 'Organization/RemoveEmployeeWorkSchedule',
                data: data,
                success: function (vData) {
                    GetAllEmployeeSchedule();
                }
            });
        }

        function GetAllSchedule() {

            AjaxSend({
                url: 'Browser/LibraryWorkSchedule',
                success: function (vData) {
                    $('#selSchedule').empty();
                    $('#selSchedule').append(new Option('Select Schedule', '', false));

                    for (var i = 0; i <= vData.Results.Rows.length - 1; i++) {
                        $('#selSchedule').append(new Option(vData.Results.Rows[i].WorkScheduleName, vData.Results.Rows[i].ID, false));
                    }

                    //                    if (sessionStorage.ScheduleID != '') {
                    //                        $('#selSchedule').val(sessionStorage.ScheduleID);
                    //                    }
                    //                    else {

                    //                        data = { PeopleID: GetUrlParameters().EmployeeID,
                    //                            FOWSTID: GetUrlParameters().FOWSTID //$('#selFowst').val(),
                    //                        }

                    //                        AjaxSend({
                    //                            url: 'Organization/GetEmployeeSchedule',
                    //                            data: data,
                    //                            success: function (vData) {
                    //                                if (vData.Results.length > 0) {
                    //                                    $('#selSchedule').val(vData.Results).selectmenu('refresh');
                    //                                }
                    //                            }
                    //                        });
                    //                    }


                    try {
                        $('#selSchedule').selectmenu('refresh');
                    }
                    catch (e) {
                        $('#selSchedule').selectmenu();
                    }

                }
            });
        }
    });
