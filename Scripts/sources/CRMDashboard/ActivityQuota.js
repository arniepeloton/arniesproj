require(['common'], 
    function () {
        $(document).ready(function () {
            var datePeriod = [];
            var quotaID = '';

            window.ActivityQuota = {
                LoadData: function () {
                    LoadRequiredData();
                },
                ShowData: function () {
                    LoadRequiredData();
                }
            }

            LoadRequiredData();

            $('#tbodyActivityQuota > tr input').each(function () {
                $(this).unbind('keydown').keydown(function (e) {
                    if(!NumberOnly(e)){
                        return;
                    }
                })
            })

            //Activity Report
            $('body')
            .on('click', '#butActivityDateRange', function () {
                PopupPeriod({
                    Click: function (e) {
                        datePeriod = e;
                        if (e.Period == 'Today' || e.Period == 'Yesterday' || e.Period == '2 days ago') {
                            $('#butActivityDateRange').text(e.From);//.button('refresh')
                        } else {
                            $('#butActivityDateRange').text(e.From + ' to ' + e.To);//.button('refresh')
                        }
                    }
                })
            })
            .on('change', '#selActivityTeamName', function () {
                if($(this).val() != ""){
                    //GetAllTeamMembers($(this).val())
                } else {
                    //$('#selActivityTeamUser').empty().append(new Option('Please Select', ''));//.selectmenu('refresh');
                }
            })
            .on('click', '#btnCancelActivityQuota', function (e) {
                window.location.hash = 'LibraryPage';
            })
            .on('click', '#btnSaveActivityQuota', function (e) {
                if ($.trim($('#selActivityTeamName').val()) == "") {
                    WebToast('Please select team');
                    return;
                }

                if ($.trim($('#butActivityDateRange').text().toLowerCase()) == "set date") {
                    WebToast('Please provide date period');
                    return;
                }

                var isInputFieldEmpty = true;
                $('#tbodyActivityQuota > tr input').each(function () {
                    if($.trim($(this).val()) != ""){
                        isInputFieldEmpty = false;
                    }
                })

                if (isInputFieldEmpty) {
                    WebToast('Please enter number at least in one field in Target column');
                    return;
                }

                SaveActivityQuota();
            })

            function SaveActivityQuota() {
                var pData = {
                    QuotaID: quotaID,
                    Team: $('#selActivityTeamName').val(),
                    Date: datePeriod,
                    Call: $('#txtActivityQuotaCallsMade').val(),
                    Note: $('#txtActivityQuotaNotesTaken').val(),
                    PhysicalAttended: $('#txtActivityQuotaPhysicalMeetingAttended').val(),
                    VirtualAttended: $('#txtActivityQuotaVirtualMeetingAttended').val(),
                    Email: $('#txtActivityQuotaEmailsSent').val(),
                    Sms: $('#txtActivityQuotaSMSSent').val(),
                    ScheduleCall: $('#txtActivityQuotaScheduledCalls').val(),
                    SchedulePhysical: $('#txtActivityQuotaScheduledPhysicalMeeting').val(),
                    ScheduleVirtual: $('#txtActivityQuotaScheduledVirtualMeeting').val(),
                    ScheduleEmail: $('#txtActivityQuotaScheduledEmails').val(),
                    ScheduleSms: $('#txtActivityQuotaScheduledSMS').val(),
                    Today: new Date()
                }

                $('#btnSaveActivityQuota, #btnCancelActivityQuota');//.button('disable') // disable button to avoid duplicate row

                AjaxSend({
                    url: 'CRMDashboard/CheckActivityOverlapDate',
                    data: pData,
                    success: function (e) {
                        if (e.IsOverlap) {
                            NewMessageBox('Date Period overlapped with activity no.: <b>' + e.RowNum + '</b>' +
                                    ' where date covered from <b>' + $.format.date(new Date(e.From), 'MMM dd, yyyy') + '</b> to <b>' + $.format.date(new Date(e.To), 'MMM dd, yyyy') + '</b>', 'Message');
                            $('#btnSaveActivityQuota, #btnCancelActivityQuota');//.button('enable');
                        } else {
                            AjaxSend({
                                url: 'CRMDashboard/SaveActivityQuota',
                                data: pData,
                                success: function (e) {
                                    $('#btnSaveActivityQuota, #btnCancelActivityQuota');//.button('enable');

                                    if (e.Results == "saved") {
                                        ClearFields();
                                        window.location.hash = 'LibraryPage';
                                        LibraryTable.ActivityQuota();
                                    } else {
                                        NewMessageBox(e.Results);
                                    }
                                }
                            })
                        }
                    }
                })
            }

            function LoadRequiredData() {
                $('#divMainActivityQuotaPage').pageshow();
                ClearFields();
                GetAllTeam();

                if (sessionStorage.AddViewLibBCName == "View Activity Quota") {
                    GetActivityQuotaData()
                }
            }

            function GetActivityQuotaData() {
                AjaxSend({
                    url: 'CRMDashboard/GetActivityQuotaData',
                    data: { ID: sessionStorage.ViewID },
                    success: function (vData) {
                        ClearFields()
                        var vRows = vData.Results.Rows[0]
                        var datefrom = $.format.date(new Date(vRows.DateFrom), 'MMM dd, yyyy');
                        var dateto = $.format.date(new Date(vRows.DateTo), 'MMM dd, yyyy');

                        datePeriod = {
                            From: datefrom,
                            To: dateto
                        }

                        quotaID = vRows.ID;

                        $('#selActivityTeamName').val(vRows.TeamID);//.selectmenu('refresh');
                        $('#butActivityDateRange').text(datefrom + ' to ' + dateto);//.button('refresh');
                        $('#txtActivityQuotaCallsMade').val(vRows.Call);
                        $('#txtActivityQuotaNotesTaken').val(vRows.Notes);
                        $('#txtActivityQuotaPhysicalMeetingAttended').val(vRows.PhysicalMeetingAttended);
                        $('#txtActivityQuotaVirtualMeetingAttended').val(vRows.VirtualMeetingAttended);
                        $('#txtActivityQuotaEmailsSent').val(vRows.Email);
                        $('#txtActivityQuotaSMSSent').val(vRows.SMS);
                        $('#txtActivityQuotaScheduledCalls').val(vRows.ScheduledCall);
                        $('#txtActivityQuotaScheduledPhysicalMeeting').val(vRows.ScheduledPhysicalMeeting);
                        $('#txtActivityQuotaScheduledVirtualMeeting').val(vRows.ScheduledVirtualMeeting);
                        $('#txtActivityQuotaScheduledEmails').val(vRows.ScheduledEmail);
                        $('#txtActivityQuotaScheduledSMS').val(vRows.ScheduledSMS);
                    }
                })
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

                        selActivityTeamUser;//.selectmenu('refresh')
                    }
                })
            }

            function ClearFields() {
                datePeriod = [];
                quotaID = '';
                $('#selActivityTeamName').val('');//.selectmenu('refresh');
                $('#selActivityTerritory').val('');//.selectmenu('refresh');
                $('#selActivityTeamUser').val('');//.selectmenu('refresh');
                $('#butActivityDateRange').text('Set Date');//.button('refresh');
                $('#txtActivityQuotaCallsMade').val('');
                $('#txtActivityQuotaNotesTaken').val('');
                $('#txtActivityQuotaPhysicalMeetingAttended').val('');
                $('#txtActivityQuotaVirtualMeetingAttended').val('');
                $('#txtActivityQuotaEmailsSent').val('');
                $('#txtActivityQuotaSMSSent').val('');
                $('#txtActivityQuotaScheduledCalls').val('');
                $('#txtActivityQuotaScheduledPhysicalMeeting').val('');
                $('#txtActivityQuotaScheduledVirtualMeeting').val('');
                $('#txtActivityQuotaScheduledEmails').val('');
                $('#txtActivityQuotaScheduledSMS').val('');
            }
        })
    }
)