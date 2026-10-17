require(['common'], 
    function () {
        $(document).ready(function () {
            var datePeriod = '', isExclude = false, vRows;

            window.ActivityReport = {
                LoadData: function () {
                    LoadRequiredData();
                }
            }

            LoadRequiredData();

            //Activity Report
            $('body')
            .on('click', '#butActivityDateRange', function () {
                PopupPeriod({
                    Click: function (e) {
                        if(e.Period == 'Custom'){
                            $('#butActivityDateRange').text(e.From + ' to ' + e.To);//.button('refresh');
                        } else {
                            $('#butActivityDateRange').text(e.Period);//.button('refresh');                            
                        }

                        datePeriod = e.From + '-' + e.To;
                    }
                })
            })
            .on('change', '#selActivityTeamName', function () {
                if ($(this).val() != "") {
                    GetAllTeamMembers($(this).val())
                } else {
                    $('#selActivityTeamUser').empty().append(new Option('Please Select', ''));//.selectmenu('refresh');
                }
            })
            .on('change', '#selActivityDateRange', function () {
                if ($(this).val() == "custom") {
                    $(this).val('');//.selectmenu('refresh');
                    PopupPeriod({
                        Click: function (e) {
                            $('#selActivityDateRange').val('custom');//.selectmenu('refresh');
                            datePeriod = e.From + '-' + e.To;
                        }
                    })
                } else {
                    datePeriod = '';
                }
            })
            .on('change', '[name="chbExclude"]', function () {
                isExclude = $(this).is(':checked');
                DisplayReport(vRows);
            })
            .on('click', '#butActivityGenerate', function () {
                //if ($.trim($('#selActivityTeamName').val()) == "") {
                //    WebToast('Please select team', 'top')
                //    return;
                //}

                if ($.trim($('#selActivityTeamUser').val()) == "") {
                    WebToast('Please select user', 'top')
                    return;
                }

                //if ($.trim($('#selActivityDateRange').val()) == "") {
                //    WebToast('Please select date range', 'top')
                //    return;
                //}

                if (datePeriod == "") {
                    WebToast('Please select date range', 'top')
                    return;
                }

                GetActivityReport();
            })

            function GetActivityReport() {
                var pData = {
                    Team: $('#selActivityTeamName').val(),
                    Territory: '', //$('#selActivityTerritory').val(),
                    User: $('#selActivityTeamUser').val(),
                    //DateRange: ($('#selActivityDateRange').val() == 'custom' ? datePeriod : $('#selActivityDateRange').val())
                    DateRange: datePeriod
                }

                AjaxSend({
                    url: 'CRMDashboard/GetActivityReport',
                    data: pData,
                    success: function (vData) {
                        vRows = vData.Results.Rows;
                        DisplayReport(vRows);
                    }
                })
            }

            function DisplayReport(vRows){
                var tdCallActual = 0,
                            tdNoteActual = 0,
                            tdPhysicalAttendedActual = 0,
                            tdVirtualAttendedActual = 0,
                            tdEmailActual = 0,
                            tdSmsActual = 0,
                            tdSchedCallActual = 0,
                            tdSchedPhysicalActual = 0,
                            tdSchedVirtualActual = 0,
                            tdSchedEmailActual = 0,
                            tdSchedSmsActual = 0,

                            tdCallTarget = 0,
                            tdNoteTarget = 0,
                            tdPhysicalAttendedTarget = 0,
                            tdVirtualAttendedTarget = 0,
                            tdEmailTarget = 0,
                            tdSmsTarget = 0,
                            tdSchedCallTarget = 0,
                            tdSchedPhysicalTarget = 0,
                            tdSchedVirtualTarget = 0,
                            tdSchedEmailTarget = 0,
                            tdSchedSmsTarget = 0,

                            tdCallBar = 0,
                            tdNoteBar = 0,
                            tdPhysicalAttendedBar = 0,
                            tdVirtualAttendedBar = 0,
                            tdEmailBar = 0,
                            tdSmsBar = 0,
                            tdSchedCallBar = 0,
                            tdSchedPhysicalBar = 0,
                            tdSchedVirtualBar = 0,
                            tdSchedEmailBar = 0,
                            tdSchedSmsBar = 0;

                ClearFields();

                if (vRows.length > 0) {
                    for (var i = 0; i < vRows.length; i++) {
                        if (CBool(vRows[i].HasQuota) || !CBool(isExclude)) {
                            tdCallActual = tdCallActual + CDec2(vRows[i].CallActual);
                            tdNoteActual = tdNoteActual + CDec2(vRows[i].NoteActual);
                            tdPhysicalAttendedActual = tdPhysicalAttendedActual + CDec2(vRows[i].PhysicalAttendedActual);
                            tdVirtualAttendedActual = tdVirtualAttendedActual + CDec2(vRows[i].VirtualAttendedActual);
                            tdEmailActual = tdEmailActual + CDec2(vRows[i].EmailActual);
                            tdSmsActual = tdSmsActual + CDec2(vRows[i].SmsActual);
                            tdSchedCallActual = tdSchedCallActual + CDec2(vRows[i].SchedCallActual);
                            tdSchedPhysicalActual = tdSchedPhysicalActual + CDec2(vRows[i].SchedPhysicalActual);
                            tdSchedVirtualActual = tdSchedVirtualActual + CDec2(vRows[i].SchedVirtualActual);
                            tdSchedEmailActual = tdSchedEmailActual + CDec2(vRows[i].SchedEmailActual);
                            tdSchedSmsActual = tdSchedSmsActual + CDec2(vRows[i].SchedSmsActual);

                            tdCallTarget = tdCallTarget + CDec2(vRows[i].CallTarget);
                            tdNoteTarget = tdNoteTarget + CDec2(vRows[i].NoteTarget);
                            tdPhysicalAttendedTarget = tdPhysicalAttendedTarget + CDec2(vRows[i].PhysicalAttendedTarget);
                            tdVirtualAttendedTarget = tdVirtualAttendedTarget + CDec2(vRows[i].VirtualAttendedTarget);
                            tdEmailTarget = tdEmailTarget + CDec2(vRows[i].EmailTarget);
                            tdSmsTarget = tdSmsTarget + CDec2(vRows[i].SmsTarget);
                            tdSchedCallTarget = tdSchedCallTarget + CDec2(vRows[i].SchedCallTarget);
                            tdSchedPhysicalTarget = tdSchedPhysicalTarget + CDec2(vRows[i].SchedPhysicalTarget);
                            tdSchedVirtualTarget = tdSchedVirtualTarget + CDec2(vRows[i].SchedVirtualTarget);
                            tdSchedEmailTarget = tdSchedEmailTarget + CDec2(vRows[i].SchedEmailTarget);
                            tdSchedSmsTarget = tdSchedSmsTarget + CDec2(vRows[i].SchedSmsTarget);
                        }
                    }

                    $('#tdCallActual').text(tdCallActual)
                    $('#tdNoteActual').text(tdNoteActual)
                    $('#tdPhysicalAttendedActual').text(tdPhysicalAttendedActual)
                    $('#tdVirtualAttendedActual').text(tdVirtualAttendedActual)
                    $('#tdEmailActual').text(tdEmailActual)
                    $('#tdSmsActual').text(tdSmsActual)
                    $('#tdSchedCallActual').text(tdSchedCallActual)
                    $('#tdSchedPhysicalActual').text(tdSchedPhysicalActual)
                    $('#tdSchedVirtualActual').text(tdSchedVirtualActual)
                    $('#tdSchedEmailActual').text(tdSchedEmailActual)
                    $('#tdSchedSmsActual').text(tdSchedSmsActual)

                    $('#tdCallTarget').text(CDec2(tdCallTarget))
                    $('#tdNoteTarget').text(CDec2(tdNoteTarget))
                    $('#tdPhysicalAttendedTarget').text(CDec2(tdPhysicalAttendedTarget))
                    $('#tdVirtualAttendedTarget').text(CDec2(tdVirtualAttendedTarget))
                    $('#tdEmailTarget').text(CDec2(tdEmailTarget))
                    $('#tdSmsTarget').text(CDec2(tdSmsTarget))
                    $('#tdSchedCallTarget').text(CDec2(tdSchedCallTarget))
                    $('#tdSchedPhysicalTarget').text(CDec2(tdSchedPhysicalTarget))
                    $('#tdSchedVirtualTarget').text(CDec2(tdSchedVirtualTarget))
                    $('#tdSchedEmailTarget').text(CDec2(tdSchedEmailTarget))
                    $('#tdSchedSmsTarget').text(CDec2(tdSchedSmsTarget))

                    tdCallBar = AccountingFormat(tdCallActual / tdCallTarget * 100);
                    tdNoteBar = AccountingFormat(tdNoteActual / tdCallTarget * 100);
                    tdPhysicalAttendedBar = AccountingFormat(tdPhysicalAttendedActual / tdCallTarget * 100);
                    tdVirtualAttendedBar = AccountingFormat(tdVirtualAttendedActual / tdCallTarget * 100);
                    tdEmailBar = AccountingFormat(tdEmailActual / tdCallTarget * 100);
                    tdSmsBar = AccountingFormat(tdSmsActual / tdCallTarget * 100);
                    tdSchedCallBar = AccountingFormat(tdSchedCallActual / tdCallTarget * 100);
                    tdSchedPhysicalBar = AccountingFormat(tdSchedPhysicalActual / tdCallTarget * 100);
                    tdSchedVirtualBar = AccountingFormat(tdSchedVirtualActual / tdCallTarget * 100);
                    tdSchedEmailBar = AccountingFormat(tdSchedEmailActual / tdCallTarget * 100);
                    tdSchedSmsBar = AccountingFormat(tdSchedSmsActual / tdCallTarget * 100);

                    $('#tdCallActualTarget').text(tdCallBar + ' %')
                    $('#tdNoteActualTarget').text(tdNoteBar + ' %')
                    $('#tdPhysicalAttendedActualTarget').text(tdPhysicalAttendedBar + ' %')
                    $('#tdVirtualAttendedActualTarget').text(tdVirtualAttendedBar + ' %')
                    $('#tdEmailActualTarget').text(tdEmailBar + ' %')
                    $('#tdSmsActualTarget').text(tdSmsBar + ' %')
                    $('#tdSchedCallActualTarget').text(tdSchedCallBar + ' %')
                    $('#tdSchedPhysicalActualTarget').text(tdSchedPhysicalBar + ' %')
                    $('#tdSchedVirtualActualTarget').text(tdSchedVirtualBar + ' %')
                    $('#tdSchedEmailActualTarget').text(tdSchedEmailBar + ' %')
                    $('#tdSchedSmsActualTarget').text(tdSchedSmsBar + ' %')

                    $('#tdCallBar').horizontalbar(tdCallBar)
                    $('#tdNoteBar').horizontalbar(tdNoteBar)
                    $('#tdPhysicalAttendedBar').horizontalbar(tdPhysicalAttendedBar)
                    $('#tdVirtualAttendedBar').horizontalbar(tdVirtualAttendedBar)
                    $('#tdEmailBar').horizontalbar(tdEmailBar)
                    $('#tdSmsBar').horizontalbar(tdSmsBar)
                    $('#tdSchedCallBar').horizontalbar(tdSchedCallBar)
                    $('#tdSchedPhysicalBar').horizontalbar(tdSchedPhysicalBar)
                    $('#tdSchedVirtualBar').horizontalbar(tdSchedVirtualBar)
                    $('#tdSchedEmailBar').horizontalbar(tdSchedEmailBar)
                    $('#tdSchedSmsBar').horizontalbar(tdSchedSmsBar)
                }
            }

            function LoadRequiredData() {
                $('#divMainActivityReportPage').pageshow();
                GetAllTeam();
                ClearFields();

                $('#selActivityTeamUser').empty().append($('<optgroup label="User Logged In">')
                        .append(new Option(sessionStorage.CurrentUserFullName, sessionStorage.PeopleID))
                    )
                $('#selActivityTeamUser').val(sessionStorage.PeopleID);//.selectmenu('refresh');

                datePeriod = $.format.date(new Date(), 'MMM dd, yyyy') + '-' + $.format.date(new Date(), 'MMM dd, yyyy');
                $('#butActivityDateRange').text('Today');//.button('refresh');
                GetActivityReport();
            }

            function GetAllTeam() {
                AjaxSend({
                    url: 'Browser/LibraryTeam',
                    success: function (vData) {
                        var vRows = vData.Results.Rows;
                        var selActivityTeamName = $('#selActivityTeamName');

                        selActivityTeamName.empty();
                        selActivityTeamName.append(new Option('Please Select', ''))

                        for (var i = 0; i < vRows.length; i++) {
                            selActivityTeamName.append(new Option(vRows[i].Name, vRows[i].ID))
                        }

                        selActivityTeamName;//.selectmenu('refresh')
                    }
                })
            }

            function GetAllTeamMembers(id) {
                AjaxSend({
                    url: 'CRM/GetTeamMembers',
                    data: { ID: id },
                    success: function (vData) {
                        var vRows = vData.Results.Rows;
                        var selActivityTeamUser = $('#selActivityTeamUser');

                        selActivityTeamUser.empty();
                        selActivityTeamUser.append(new Option('Please Select', ''))

                        for (var i = 0; i < vRows.length; i++) {
                            selActivityTeamUser.append(new Option(vRows[i].Name, vRows[i].ID))
                        }

                        selActivityTeamUser.append($('<optgroup label="User Logged In">')
                            .append(new Option(sessionStorage.CurrentUserFullName, sessionStorage.PeopleID))
                        )

                        selActivityTeamUser;//.selectmenu('refresh')

                        GetAvailableTeamDatePeriod(id)
                    }
                })
            }

            function GetAvailableTeamDatePeriod(id) {
                AjaxSend({
                    url: 'CRMDashboard/GetAvailableTeamDatePeriod',
                    data: { ID: id },
                    success: function (vData) {
                        var vRows = vData.Results.Rows;
                        var selActivityDateRange = $('#selActivityDateRange');

                        selActivityDateRange.empty();
                        selActivityDateRange.append(new Option('Please Select', ''))

                        selActivityDateRange.append(new Option('Custom Period', 'custom'))

                        for (var i = 0; i < vRows.length; i++) {
                            selActivityDateRange.append(new Option($.format.date(new Date(vRows[i].DateFrom), 'MMM dd, yyyy') + ' to ' + $.format.date(new Date(vRows[i].DateTo), 'MMM dd, yyyy'), $.format.date(new Date(vRows[i].DateFrom), 'MMM dd, yyyy') + '-' + $.format.date(new Date(vRows[i].DateTo), 'MMM dd, yyyy')))
                        }                        

                        selActivityDateRange;//.selectmenu('refresh')
                    }
                })
            }

            function ClearFields() {                
                tdCallTarget = 0;
                tdNoteTarget = 0;
                tdPhysicalAttendedTarget = 0;
                tdVirtualAttendedTarget = 0;
                tdEmailTarget = 0;
                tdSmsTarget = 0;
                tdSchedCallTarget = 0;
                tdSchedPhysicalTarget = 0;
                tdSchedVirtualTarget = 0;
                tdSchedEmailTarget = 0;
                tdSchedSmsTarget = 0;

                $('#tbodyActivityReport tr').each(function () {
                    $(this).find('td:eq(1)').text('-'); // Actual Col
                    $(this).find('td:eq(2)').text('-'); // Target Col
                    $(this).find('td:eq(3)').text('NA'); // Actual-Target Col
                    $(this).find('td:eq(4) > div').removeAttr('style');
                })
            }
        })
    }
)