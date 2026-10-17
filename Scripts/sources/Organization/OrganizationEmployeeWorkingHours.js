require([
    'common'
    ], function () {

        var IsAssignEmployee = 0;
        var IsNew = 0;
        var ID = '';
        var isIE = false || !!document.documentMode;
        var isOrgEmpPageLoaded = "";

        $(function () {

            GetAllSchedule();

            window.OrgEmpWorkHours = {
                GetSchedule: function(){
                    GetAllSchedule();
                }
            }

            $('#pageNewSchedule').bind('pageshow', pageShowSchedule)

            $('body').on('click', '#btnNewEmployeeSchedule', function(e){
                IsNew = 1;
                $('#txtName').val('');
                $('#txtDescription').val('');

                //$.mobile.changePage('#pageNewSchedule');
                $('#pageOrganizationEmployeeSchedule').hide();
                $('#pageNewSchedule').show();
            })
            .on('click', '#btnDone', function(e){
                e.preventDefault();
                //window.open(GetPathFormat("/People/PeopleMenu#pagePopUpEmployeeSettings"), "_self");
                window.location.hash = 'LibraryPage';
            })
            .on('click', '#btnApplyTimeToDays', function(e){
                 $('#selSelectDay').val('');//.selectmenu('refresh');

                $('#chkMonday').prop('checked', false);
                $('#chkTuesday').prop('checked', false);
                $('#chkWednesday').prop('checked', false);
                $('#chkThursday').prop('checked', false);
                $('#chkFriday').prop('checked', false);
                $('#chkSaturday').prop('checked', false);
                $('#chkSunday').prop('checked', false);

                try {
                    $("#chkMonday");//.checkboxradio("refresh");
                    $("#chkTuesday");//.checkboxradio("refresh");
                    $("#chkWednesday");//.checkboxradio("refresh");
                    $("#chkThursday");//.checkboxradio("refresh");
                    $("#chkFriday");//.checkboxradio("refresh");
                    $("#chkSaturday");//.checkboxradio("refresh");
                    $("#chkSunday");//.checkboxradio("refresh"); 
                }
                catch (e) {
                    $("#chkMonday");//.checkboxradio();
                    $("#chkTuesday");//.checkboxradio();
                    $("#chkWednesday");//.checkboxradio();
                    $("#chkThursday");//.checkboxradio();
                    $("#chkFriday");//.checkboxradio();
                    $("#chkSaturday");//.checkboxradio();
                    $("#chkSunday");//.checkboxradio();
                }

                $('#lblTimeRange').text('');
            })
            .on('click', '#btnAssignEmployees', function(e){
                IsAssignEmployee = 1;
                SaveTime();
            })
            .on('click', '#btnClear', function(e){
                MessageBox("Are you sure you want to clear all time?",
                            "Work Schedule",
                            [
                                { Text: 'Yes',
                                    Click: function () {
                                        ClearTime();
                                        $('.ui-dialog').dialog('close');
                                    }
                                },
                                { Text: 'No' }
                            ]
                        );
            })
            .on('click', '#btnSave', function(e){
                IsAssignEmployee = 0;
                SaveTime();
            })
            .on('click', '#btnApplyTime', function(e){
                var m_TimeFrom = '';
                var m_DayFrom = '';
                var m_TimeTo = '';
                var m_DayTo = '';

                var m_LunchTimeFrom = '';
                var m_LunchDayFrom = '';
                var m_LunchTimeTo = '';
                var m_LunchDayTo = '';

                var m_DinnerTimeFrom = '';
                var m_DinnerDayFrom = '';
                var m_DinnerTimeTo = '';
                var m_DinnerDayTo = '';

                if ($('#selSelectDay').val() == '') {
                    WebToast('Please select day', 'top');
                }
                else {
                    switch ($('#selSelectDay').val()) {
                        case 'optMonday':
                            m_TimeFrom = $('#selMondayFrom').val();
                            m_DayFrom = $('#selMondayFromDay').val();
                            m_TimeTo = $('#selMondayTo').val();
                            m_DayTo = $('#selMondayToDay').val();

                            m_LunchTimeFrom = $('#selMondayLunchBreakFrom').val();
                            m_LunchDayFrom = $('#selMondayLunchBreakFromDay').val();
                            m_LunchTimeTo = $('#selMondayLunchBreakTo').val();
                            m_LunchDayTo = $('#selMondayLunchBreakToDay').val();

                            m_DinnerTimeFrom = $('#selMondayDinnerBreakFrom').val();
                            m_DinnerDayFrom = $('#selMondayDinnerBreakFromDay').val();
                            m_DinnerTimeTo = $('#selMondayDinnerBreakTo').val();
                            m_DinnerDayTo = $('#selMondayLunchDinnerToDay').val();

                            break;
                        case 'optTuesday':
                            m_TimeFrom = $('#selTuesdayFrom').val();
                            m_DayFrom = $('#selTuesdayFromDay').val();
                            m_TimeTo = $('#selTuesdayTo').val();
                            m_DayTo = $('#selTuesdayToDay').val();

                            m_LunchTimeFrom = $('#selTuesdayLunchBreakFrom').val();
                            m_LunchDayFrom = $('#selTuesdayLunchBreakFromDay').val();
                            m_LunchTimeTo = $('#selTuesdayLunchBreakTo').val();
                            m_LunchDayTo = $('#selTuesdayLunchBreakToDay').val();

                            m_DinnerTimeFrom = $('#selTuesdayDinnerBreakFrom').val();
                            m_DinnerDayFrom = $('#selTuesdayDinnerBreakFromDay').val();
                            m_DinnerTimeTo = $('#selTuesdayDinnerBreakTo').val();
                            m_DinnerDayTo = $('#selTuesdayLunchDinnerToDay').val();
                            break;
                        case 'optWednesday':
                            m_TimeFrom = $('#selWednesdayFrom').val();
                            m_DayFrom = $('#selWednesdayFromDay').val();
                            m_TimeTo = $('#selWednesdayTo').val();
                            m_DayTo = $('#selWednesdayToDay').val();

                            m_LunchTimeFrom = $('#selWednesdayLunchBreakFrom').val();
                            m_LunchDayFrom = $('#selWednesdayLunchBreakFromDay').val();
                            m_LunchTimeTo = $('#selWednesdayLunchBreakTo').val();
                            m_LunchDayTo = $('#selWednesdayLunchBreakToDay').val();

                            m_DinnerTimeFrom = $('#selWednesdayDinnerBreakFrom').val();
                            m_DinnerDayFrom = $('#selWednesdayDinnerBreakFromDay').val();
                            m_DinnerTimeTo = $('#selWednesdayDinnerBreakTo').val();
                            m_DinnerDayTo = $('#selWednesdayLunchDinnerToDay').val();
                            break;
                        case 'optThursday':
                            m_TimeFrom = $('#selThursdayFrom').val();
                            m_DayFrom = $('#selThursdayFromDay').val();
                            m_TimeTo = $('#selThursdayTo').val();
                            m_DayTo = $('#selThursdayToDay').val();

                            m_LunchTimeFrom = $('#selThursdayLunchBreakFrom').val();
                            m_LunchDayFrom = $('#selThursdayLunchBreakFromDay').val();
                            m_LunchTimeTo = $('#selThursdayLunchBreakTo').val();
                            m_LunchDayTo = $('#selThursdayLunchBreakToDay').val();

                            m_DinnerTimeFrom = $('#selThursdayDinnerBreakFrom').val();
                            m_DinnerDayFrom = $('#selThursdayDinnerBreakFromDay').val();
                            m_DinnerTimeTo = $('#selThursdayDinnerBreakTo').val();
                            m_DinnerDayTo = $('#selThursdayLunchDinnerToDay').val();

                            break;
                        case 'optFriday':
                            m_TimeFrom = $('#selFridayFrom').val();
                            m_DayFrom = $('#selFridayFromDay').val();
                            m_TimeTo = $('#selFridayTo').val();
                            m_DayTo = $('#selFridayToDay').val();

                            m_LunchTimeFrom = $('#selFridayLunchBreakFrom').val();
                            m_LunchDayFrom = $('#selFridayLunchBreakFromDay').val();
                            m_LunchTimeTo = $('#selFridayLunchBreakTo').val();
                            m_LunchDayTo = $('#selFridayLunchBreakToDay').val();

                            m_DinnerTimeFrom = $('#selFridayDinnerBreakFrom').val();
                            m_DinnerDayFrom = $('#selFridayDinnerBreakFromDay').val();
                            m_DinnerTimeTo = $('#selFridayDinnerBreakTo').val();
                            m_DinnerDayTo = $('#selFridayLunchDinnerToDay').val();
                            break;
                        case 'optSaturday':
                            m_TimeFrom = $('#selSaturdayFrom').val();
                            m_DayFrom = $('#selSaturdayFromDay').val();
                            m_TimeTo = $('#selSaturdayTo').val();
                            m_DayTo = $('#selSaturdayToDay').val();

                            m_LunchTimeFrom = $('#selSaturdayLunchBreakFrom').val();
                            m_LunchDayFrom = $('#selSaturdayLunchBreakFromDay').val();
                            m_LunchTimeTo = $('#selSaturdayLunchBreakTo').val();
                            m_LunchDayTo = $('#selSaturdayLunchBreakToDay').val();

                            m_DinnerTimeFrom = $('#selSaturdayDinnerBreakFrom').val();
                            m_DinnerDayFrom = $('#selSaturdayDinnerBreakFromDay').val();
                            m_DinnerTimeTo = $('#selSaturdayDinnerBreakTo').val();
                            m_DinnerDayTo = $('#selSaturdayLunchDinnerToDay').val();
                            break;
                        case 'optSunday':
                            m_TimeFrom = $('#selSundayFrom').val();
                            m_DayFrom = $('#selSundayFromDay').val();
                            m_TimeTo = $('#selSundayTo').val();
                            m_DayTo = $('#selSundayToDay').val();

                            m_LunchTimeFrom = $('#selSundayLunchBreakFrom').val();
                            m_LunchDayFrom = $('#selSundayLunchBreakFromDay').val();
                            m_LunchTimeTo = $('#selSundayLunchBreakTo').val();
                            m_LunchDayTo = $('#selSundayLunchBreakToDay').val();

                            m_DinnerTimeFrom = $('#selSundayDinnerBreakFrom').val();
                            m_DinnerDayFrom = $('#selSundayDinnerBreakFromDay').val();
                            m_DinnerTimeTo = $('#selSundayDinnerBreakTo').val();
                            m_DinnerDayTo = $('#selSundayLunchDinnerToDay').val();
                            break;
                    }

                    if ($('#chkMonday:checked').val() == 'on') {
                        $('#selMondayFrom').val(m_TimeFrom);//.selectmenu('refresh');
                        $('#selMondayFromDay').val(m_DayFrom);//.selectmenu('refresh');
                        $('#selMondayTo').val(m_TimeTo);//.selectmenu('refresh');
                        $('#selMondayToDay').val(m_DayTo);//.selectmenu('refresh');

                        $('#selMondayLunchBreakFrom').val(m_LunchTimeFrom);//.selectmenu('refresh');
                        $('#selMondayLunchBreakFromDay').val(m_LunchDayFrom);//.selectmenu('refresh');
                        $('#selMondayLunchBreakTo').val(m_LunchTimeTo);//.selectmenu('refresh');
                        $('#selMondayLunchBreakToDay').val(m_LunchDayTo);//.selectmenu('refresh');

                        $('#selMondayDinnerBreakFrom').val(m_DinnerTimeFrom);//.selectmenu('refresh');
                        $('#selMondayDinnerBreakFromDay').val(m_DinnerDayFrom);//.selectmenu('refresh');
                        $('#selMondayDinnerBreakTo').val(m_DinnerTimeTo);//.selectmenu('refresh');
                        $('#selMondayLunchDinnerToDay').val(m_DinnerDayTo);//.selectmenu('refresh');
                    }
                    if ($('#chkTuesday:checked').val() == 'on') {
                        $('#selTuesdayFrom').val(m_TimeFrom);//.selectmenu('refresh');
                        $('#selTuesdayFromDay').val(m_DayFrom);//.selectmenu('refresh');
                        $('#selTuesdayTo').val(m_TimeTo);//.selectmenu('refresh');
                        $('#selTuesdayToDay').val(m_DayTo);//.selectmenu('refresh');

                        $('#selTuesdayLunchBreakFrom').val(m_LunchTimeFrom);//.selectmenu('refresh');
                        $('#selTuesdayLunchBreakFromDay').val(m_LunchDayFrom);//.selectmenu('refresh');
                        $('#selTuesdayLunchBreakTo').val(m_LunchTimeTo);//.selectmenu('refresh');
                        $('#selTuesdayLunchBreakToDay').val(m_LunchDayTo);//.selectmenu('refresh');

                        $('#selTuesdayDinnerBreakFrom').val(m_DinnerTimeFrom);//.selectmenu('refresh');
                        $('#selTuesdayDinnerBreakFromDay').val(m_DinnerDayFrom);//.selectmenu('refresh');
                        $('#selTuesdayDinnerBreakTo').val(m_DinnerTimeTo);//.selectmenu('refresh');
                        $('#selTuesdayLunchDinnerToDay').val(m_DinnerDayTo);//.selectmenu('refresh');
                    }
                    if ($('#chkWednesday:checked').val() == 'on') {
                        $('#selWednesdayFrom').val(m_TimeFrom);//.selectmenu('refresh');
                        $('#selWednesdayFromDay').val(m_DayFrom);//.selectmenu('refresh');
                        $('#selWednesdayTo').val(m_TimeTo);//.selectmenu('refresh');
                        $('#selWednesdayToDay').val(m_DayTo);//.selectmenu('refresh');

                        $('#selWednesdayLunchBreakFrom').val(m_LunchTimeFrom);//.selectmenu('refresh');
                        $('#selWednesdayLunchBreakFromDay').val(m_LunchDayFrom);//.selectmenu('refresh');
                        $('#selWednesdayLunchBreakTo').val(m_LunchTimeTo);//.selectmenu('refresh');
                        $('#selWednesdayLunchBreakToDay').val(m_LunchDayTo);//.selectmenu('refresh');

                        $('#selWednesdayDinnerBreakFrom').val(m_DinnerTimeFrom);//.selectmenu('refresh');
                        $('#selWednesdayDinnerBreakFromDay').val(m_DinnerDayFrom);//.selectmenu('refresh');
                        $('#selWednesdayDinnerBreakTo').val(m_DinnerTimeTo);//.selectmenu('refresh');
                        $('#selWednesdayLunchDinnerToDay').val(m_DinnerDayTo);//.selectmenu('refresh');
                    }
                    if ($('#chkThursday:checked').val() == 'on') {
                        $('#selThursdayFrom').val(m_TimeFrom);//.selectmenu('refresh');
                        $('#selThursdayFromDay').val(m_DayFrom);//.selectmenu('refresh');
                        $('#selThursdayTo').val(m_TimeTo);//.selectmenu('refresh');
                        $('#selThursdayToDay').val(m_DayTo);//.selectmenu('refresh');

                        $('#selThursdayLunchBreakFrom').val(m_LunchTimeFrom);//.selectmenu('refresh');
                        $('#selThursdayLunchBreakFromDay').val(m_LunchDayFrom);//.selectmenu('refresh');
                        $('#selThursdayLunchBreakTo').val(m_LunchTimeTo);//.selectmenu('refresh');
                        $('#selThursdayLunchBreakToDay').val(m_LunchDayTo);//.selectmenu('refresh');

                        $('#selThursdayDinnerBreakFrom').val(m_DinnerTimeFrom);//.selectmenu('refresh');
                        $('#selThursdayDinnerBreakFromDay').val(m_DinnerDayFrom);//.selectmenu('refresh');
                        $('#selThursdayDinnerBreakTo').val(m_DinnerTimeTo);//.selectmenu('refresh');
                        $('#selThursdayLunchDinnerToDay').val(m_DinnerDayTo);//.selectmenu('refresh');
                    }
                    if ($('#chkFriday:checked').val() == 'on') {
                        $('#selFridayFrom').val(m_TimeFrom);//.selectmenu('refresh');
                        $('#selFridayFromDay').val(m_DayFrom);//.selectmenu('refresh');
                        $('#selFridayTo').val(m_TimeTo);//.selectmenu('refresh');
                        $('#selFridayToDay').val(m_DayTo);//.selectmenu('refresh');

                        $('#selFridayLunchBreakFrom').val(m_LunchTimeFrom);//.selectmenu('refresh');
                        $('#selFridayLunchBreakFromDay').val(m_LunchDayFrom);//.selectmenu('refresh');
                        $('#selFridayLunchBreakTo').val(m_LunchTimeTo);//.selectmenu('refresh');
                        $('#selFridayLunchBreakToDay').val(m_LunchDayTo);//.selectmenu('refresh');

                        $('#selFridayDinnerBreakFrom').val(m_DinnerTimeFrom);//.selectmenu('refresh');
                        $('#selFridayDinnerBreakFromDay').val(m_DinnerDayFrom);//.selectmenu('refresh');
                        $('#selFridayDinnerBreakTo').val(m_DinnerTimeTo);//.selectmenu('refresh');
                        $('#selFridayLunchDinnerToDay').val(m_DinnerDayTo);//.selectmenu('refresh');
                    }
                    if ($('#chkSaturday:checked').val() == 'on') {
                        $('#selSaturdayFrom').val(m_TimeFrom);//.selectmenu('refresh');
                        $('#selSaturdayFromDay').val(m_DayFrom);//.selectmenu('refresh');
                        $('#selSaturdayTo').val(m_TimeTo);//.selectmenu('refresh');
                        $('#selSaturdayToDay').val(m_DayTo);//.selectmenu('refresh');

                        $('#selSaturdayLunchBreakFrom').val(m_LunchTimeFrom);//.selectmenu('refresh');
                        $('#selSaturdayLunchBreakFromDay').val(m_LunchDayFrom);//.selectmenu('refresh');
                        $('#selSaturdayLunchBreakTo').val(m_LunchTimeTo);//.selectmenu('refresh');
                        $('#selSaturdayLunchBreakToDay').val(m_LunchDayTo);//.selectmenu('refresh');

                        $('#selSaturdayDinnerBreakFrom').val(m_DinnerTimeFrom);//.selectmenu('refresh');
                        $('#selSaturdayDinnerBreakFromDay').val(m_DinnerDayFrom);//.selectmenu('refresh');
                        $('#selSaturdayDinnerBreakTo').val(m_DinnerTimeTo);//.selectmenu('refresh');
                        $('#selSaturdayLunchDinnerToDay').val(m_DinnerDayTo);//.selectmenu('refresh');
                    }
                    if ($('#chkSunday:checked').val() == 'on') {
                        $('#selSundayFrom').val(m_TimeFrom);//.selectmenu('refresh')
                        $('#selSundayFromDay').val(m_DayFrom);//.selectmenu('refresh');
                        $('#selSundayTo').val(m_TimeTo);//.selectmenu('refresh');
                        $('#selSundayToDay').val(m_DayTo);//.selectmenu('refresh');

                        $('#selSundayLunchBreakFrom').val(m_LunchTimeFrom);//.selectmenu('refresh');
                        $('#selSundayLunchBreakFromDay').val(m_LunchDayFrom);//.selectmenu('refresh');
                        $('#selSundayLunchBreakTo').val(m_LunchTimeTo);//.selectmenu('refresh');
                        $('#selSundayLunchBreakToDay').val(m_LunchDayTo);//.selectmenu('refresh');

                        $('#selSundayDinnerBreakFrom').val(m_DinnerTimeFrom);//.selectmenu('refresh');
                        $('#selSundayDinnerBreakFromDay').val(m_DinnerDayFrom);//.selectmenu('refresh');
                        $('#selSundayDinnerBreakTo').val(m_DinnerTimeTo);//.selectmenu('refresh');
                        $('#selSundayLunchDinnerToDay').val(m_DinnerDayTo);//.selectmenu('refresh');
                    }

                    $("#panApplyTime").modal("hide");
                }
            })
            .on('click', '#selSelectDay', function(e){
                if ($('#selSelectDay').val() != '') {
                    switch ($('#selSelectDay').val()) {
                        case 'optMonday':
                            $('#lblTimeRange').text('(Work Schedule: ' + $('#selMondayFrom').val() + ' ' + $('#selMondayFromDay').val() + ' - ' + $('#selMondayTo').val() + ' ' + $('#selMondayToDay').val() + ')');
                            $('#lblLunchBreak').text('(Lunch Break: ' + $('#selMondayLunchBreakFrom').val() + ' ' + $('#selMondayLunchBreakFromDay').val() + ' - ' + $('#selMondayLunchBreakTo').val() + ' ' + $('#selMondayLunchBreakToDay').val() + ')');
                            $('#lblDinnerBreak').text('(Dinner Break: ' + $('#selMondayDinnerBreakFrom').val() + ' ' + $('#selMondayDinnerBreakFromDay').val() + ' - ' + $('#selMondayDinnerBreakTo').val() + ' ' + $('#selMondayLunchDinnerToDay').val() + ')');
                            break;
                        case 'optTuesday':
                            $('#lblTimeRange').text('(Work Schedule: ' + $('#selTuesdayFrom').val() + ' ' + $('#selTuesdayFromDay').val() + ' - ' + $('#selTuesdayTo').val() + ' ' + $('#selTuesdayToDay').val() + ')');
                            $('#lblLunchBreak').text('(Lunch Break: ' + $('#selTuesdayLunchBreakFrom').val() + ' ' + $('#selTuesdayLunchBreakFromDay').val() + ' - ' + $('#selTuesdayLunchBreakTo').val() + ' ' + $('#selTuesdayLunchBreakToDay').val() + ')');
                            $('#lblDinnerBreak').text('(Dinner Break: ' + $('#selTuesdayDinnerBreakFrom').val() + ' ' + $('#selTuesdayDinnerBreakFromDay').val() + ' - ' + $('#selTuesdayDinnerBreakTo').val() + ' ' + $('#selTuesdayLunchDinnerToDay').val() + ')');
                            break;
                        case 'optWednesday':
                            $('#lblTimeRange').text('(Work Schedule: ' + $('#selWednesdayFrom').val() + ' ' + $('#selWednesdayFromDay').val() + ' - ' + $('#selWednesdayTo').val() + ' ' + $('#selWednesdayToDay').val() + ')');
                            $('#lblLunchBreak').text('(Lunch Break: ' + $('#selWednesdayLunchBreakFrom').val() + ' ' + $('#selWednesdayLunchBreakFromDay').val() + ' - ' + $('#selWednesdayLunchBreakTo').val() + ' ' + $('#selWednesdayLunchBreakToDay').val() + ')');
                            $('#lblDinnerBreak').text('(Dinner Break: ' + $('#selWednesdayDinnerBreakFrom').val() + ' ' + $('#selWednesdayDinnerBreakFromDay').val() + ' - ' + $('#selWednesdayDinnerBreakTo').val() + ' ' + $('#selWednesdayLunchDinnerToDay').val() + ')');
                            break;
                        case 'optThursday':
                            $('#lblTimeRange').text('(Work Schedule: ' + $('#selThursdayFrom').val() + ' ' + $('#selThursdayFromDay').val() + ' - ' + $('#selThursdayTo').val() + ' ' + $('#selThursdayToDay').val() + ')');
                            $('#lblLunchBreak').text('(Lunch Break: ' + $('#selThursdayLunchBreakFrom').val() + ' ' + $('#selThursdayLunchBreakFromDay').val() + ' - ' + $('#selThursdayLunchBreakTo').val() + ' ' + $('#selThursdayLunchBreakToDay').val() + ')');
                            $('#lblDinnerBreak').text('(Dinner Break: ' + $('#selThursdayDinnerBreakFrom').val() + ' ' + $('#selThursdayDinnerBreakFromDay').val() + ' - ' + $('#selThursdayDinnerBreakTo').val() + ' ' + $('#selThursdayLunchDinnerToDay').val() + ')');
                            break;
                        case 'optFriday':
                            $('#lblTimeRange').text('(Work Schedule: ' + $('#selFridayFrom').val() + ' ' + $('#selFridayFromDay').val() + ' - ' + $('#selFridayTo').val() + ' ' + $('#selFridayToDay').val() + ')');
                            $('#lblLunchBreak').text('(Lunch Break: ' + $('#selFridayLunchBreakFrom').val() + ' ' + $('#selFridayDinnerBreakFromDay').val() + ' - ' + $('#selFridayLunchBreakTo').val() + ' ' + $('#selFridayLunchBreakToDay').val() + ')');
                            $('#lblDinnerBreak').text('(Dinner Break: ' + $('#selFridayDinnerBreakFrom').val() + ' ' + $('#selFridayDinnerBreakFromDay').val() + ' - ' + $('#selFridayDinnerBreakTo').val() + ' ' + $('#selFridayLunchDinnerToDay').val() + ')');
                            break;
                        case 'optSaturday':
                            $('#lblTimeRange').text('(Work Schedule: ' + $('#selSaturdayFrom').val() + ' ' + $('#selSaturdayFromDay').val() + ' - ' + $('#selSaturdayTo').val() + ' ' + $('#selSaturdayToDay').val() + ')');
                            $('#lblLunchBreak').text('(Lunch Break: ' + $('#selSaturdayLunchBreakFrom').val() + ' ' + $('#selSaturdayLunchBreakFromDay').val() + ' - ' + $('#selSaturdayLunchBreakTo').val() + ' ' + $('#selSaturdayLunchBreakToDay').val() + ')');
                            $('#lblDinnerBreak').text('(Dinner Break: ' + $('#selSaturdayDinnerBreakFrom').val() + ' ' + $('#selSaturdayDinnerBreakFromDay').val() + ' - ' + $('#selSaturdayDinnerBreakTo').val() + ' ' + $('#selSaturdayLunchDinnerToDay').val() + ')');
                            break;
                        case 'optSunday':
                            $('#lblTimeRange').text('(Work Schedule: ' + $('#selSundayFrom').val() + ' ' + $('#selSundayFromDay').val() + ' - ' + $('#selSundayTo').val() + ' ' + $('#selSundayToDay').val() + ')');
                            $('#lblLunchBreak').text('(Lunch Break: ' + $('#selSundayLunchBreakFrom').val() + ' ' + $('#selSundayLunchBreakFromDay').val() + ' - ' + $('#selSundayLunchBreakTo').val() + ' ' + $('#selSundayLunchBreakToDay').val() + ')');
                            $('#lblDinnerBreak').text('(Dinner Break: ' + $('#selSundayDinnerBreakFrom').val() + ' ' + $('#selSundayDinnerBreakFromDay').val() + ' - ' + $('#selSundayDinnerBreakTo').val() + ' ' + $('#selSundayLunchDinnerToDay').val() + ')');
                            break;
                    }
                }
            })
            .on('click', '#btnWorkSchedCancel', function(){
                $('#pageOrganizationEmployeeSchedule').show();
                $('#pageNewSchedule').hide();
            })
        });

        function GetAllSchedule() {
            AjaxSend({
                url: 'Organization/GetAllSchedule',
                success: function (vData) {
                    if (vData.Results.Rows.length > 0) {
                        DisplayAllSchedule(vData.Results.Rows);
                    }
                }
            });
        }

        function DisplayAllSchedule(vRows) {
            pLV = $('#lstWorkSchedule');
            pLV.empty();

            for (var i = 0; i <= vRows.length - 1; i++) {

                pItem = $('<a href="" class="list-group-item list-group-item-action">')
                        .attr('ID', vRows[i].ID)
                        .attr('href', '#')
                        .append($('<h5>')
                            .text(vRows[i].WorkScheduleName)
                        )
                        .append($('<div>')
                            .text(vRows[i].Description)
                        )
                        .click(function () {
                            IsNew = 0;
                            //$.mobile.changePage('#pageNewSchedule')
                            $('#pageOrganizationEmployeeSchedule').hide();
                            $('#pageNewSchedule').show();
                            ClearTime();
                            ID = $(this).attr("ID");
                            DisplaySelectedSchedule();
                        })
                pLV.append(pItem);
            }
            //pLV.listview('refresh');
        }

        function DisplaySelectedSchedule() {
            data = { ID: ID }
            AjaxSend({
                url: 'Organization/GetAllSchedule',
                data: data,
                success: function (vData) {
                    var vRows = vData.Results.Rows
                    if (vRows.length > 0) {
                        $('#txtName').val(vRows[0].WorkScheduleName || '');
                        $('#txtDescription').val(vRows[0].Description || '');

                        if (IsDate(vRows[0].MondayFrom)) {
                            $('#selMondayFrom').val($.format.date(new Date(vRows[0].MondayFrom), 'hh:mm') || '');//.selectmenu('refresh');
                            $('#selMondayFromDay').val($.format.date(new Date(vRows[0].MondayFrom), 'a') || '');//.selectmenu('refresh');
                            $('#selMondayTo').val($.format.date(new Date(vRows[0].MondayTo), 'hh:mm') || '');//.selectmenu('refresh');
                            $('#selMondayToDay').val($.format.date(new Date(vRows[0].MondayTo), 'a') || '');//.selectmenu('refresh');
                        }

                        if (IsDate(vRows[0].MondayLunchBreakFrom)) {
                            $('#selMondayLunchBreakFrom').val($.format.date(new Date(vRows[0].MondayLunchBreakFrom), 'hh:mm') || '');//.selectmenu('refresh');
                            $('#selMondayLunchBreakFromDay').val($.format.date(new Date(vRows[0].MondayLunchBreakFrom), 'a') || '');//.selectmenu('refresh');
                            $('#selMondayLunchBreakTo').val($.format.date(new Date(vRows[0].MondayLunchBreakTo), 'hh:mm') || '');//.selectmenu('refresh');
                            $('#selMondayLunchBreakToDay').val($.format.date(new Date(vRows[0].MondayLunchBreakTo), 'a') || '');//.selectmenu('refresh');
                        }

                        if (IsDate(vRows[0].MondayDinnerBreakFrom)) {
                            $('#selMondayDinnerBreakFrom').val($.format.date(new Date(vRows[0].MondayDinnerBreakFrom), 'hh:mm') || '');//.selectmenu('refresh');
                            $('#selMondayDinnerBreakFromDay').val($.format.date(new Date(vRows[0].MondayDinnerBreakFrom), 'a') || '');//.selectmenu('refresh');
                            $('#selMondayDinnerBreakTo').val($.format.date(new Date(vRows[0].MondayDinnerBreakTo), 'hh:mm') || '');//.selectmenu('refresh');
                            $('#selMondayLunchDinnerToDay').val($.format.date(new Date(vRows[0].MondayLunchDinnerTo), 'a') || '');//.selectmenu('refresh');
                        }

                        if (IsDate(vRows[0].TuesdayFrom)) {
                            $('#selTuesdayFrom').val($.format.date(new Date(vRows[0].TuesdayFrom), 'hh:mm') || '');//.selectmenu('refresh');
                            $('#selTuesdayFromDay').val($.format.date(new Date(vRows[0].TuesdayFrom), 'a') || '');//.selectmenu('refresh');
                            $('#selTuesdayTo').val($.format.date(new Date(vRows[0].TuesdayTo), 'hh:mm') || '');//.selectmenu('refresh');
                            $('#selTuesdayToDay').val($.format.date(new Date(vRows[0].TuesdayTo), 'a') || '');//.selectmenu('refresh');
                        }
                        if (IsDate(vRows[0].TuesdayLunchBreakFrom)) {
                            $('#selTuesdayLunchBreakFrom').val($.format.date(new Date(vRows[0].TuesdayLunchBreakFrom), 'hh:mm') || '');//.selectmenu('refresh');
                            $('#selTuesdayLunchBreakFromDay').val($.format.date(new Date(vRows[0].TuesdayLunchBreakFrom), 'a') || '');//.selectmenu('refresh');
                            $('#selTuesdayLunchBreakTo').val($.format.date(new Date(vRows[0].TuesdayLunchBreakTo), 'hh:mm') || '');//.selectmenu('refresh');
                            $('#selTuesdayLunchBreakToDay').val($.format.date(new Date(vRows[0].TuesdayLunchBreakTo), 'a') || '');//.selectmenu('refresh');
                        }
                        if (IsDate(vRows[0].TuesdayDinnerBreakFrom)) {
                            $('#selTuesdayDinnerBreakFrom').val($.format.date(new Date(vRows[0].TuesdayDinnerBreakFrom), 'hh:mm') || '');//.selectmenu('refresh');
                            $('#selTuesdayDinnerBreakFromDay').val($.format.date(new Date(vRows[0].TuesdayDinnerBreakFrom), 'a') || '');//.selectmenu('refresh');
                            $('#selTuesdayDinnerBreakTo').val($.format.date(new Date(vRows[0].TuesdayDinnerBreakTo), 'hh:mm') || '');//.selectmenu('refresh');
                            $('#selTuesdayLunchDinnerToDay').val($.format.date(new Date(vRows[0].TuesdayLunchDinnerTo), 'a') || '');//.selectmenu('refresh');
                        }

                        if (IsDate(vRows[0].WednesdayFrom)) {
                            $('#selWednesdayFrom').val($.format.date(new Date(vRows[0].WednesdayFrom), 'hh:mm') || '');//.selectmenu('refresh');
                            $('#selWednesdayFromDay').val($.format.date(new Date(vRows[0].WednesdayFrom), 'a') || '');//.selectmenu('refresh');
                            $('#selWednesdayTo').val($.format.date(new Date(vRows[0].WednesdayTo), 'hh:mm') || '');//.selectmenu('refresh');
                            $('#selWednesdayToDay').val($.format.date(new Date(vRows[0].WednesdayTo), 'a') || '');//.selectmenu('refresh');
                        }
                        if (IsDate(vRows[0].WednesdayLunchBreakFrom)) {
                            $('#selWednesdayLunchBreakFrom').val($.format.date(new Date(vRows[0].WednesdayLunchBreakFrom), 'hh:mm') || '');//.selectmenu('refresh');
                            $('#selWednesdayLunchBreakFromDay').val($.format.date(new Date(vRows[0].WednesdayLunchBreakFrom), 'a') || '');//.selectmenu('refresh');
                            $('#selWednesdayLunchBreakTo').val($.format.date(new Date(vRows[0].WednesdayLunchBreakTo), 'hh:mm') || '');//.selectmenu('refresh');
                            $('#selWednesdayLunchBreakToDay').val($.format.date(new Date(vRows[0].WednesdayLunchBreakTo), 'a') || '');//.selectmenu('refresh');
                        }
                        if (IsDate(vRows[0].WednesdayDinnerBreakFrom)) {
                            $('#selWednesdayDinnerBreakFrom').val($.format.date(new Date(vRows[0].WednesdayDinnerBreakFrom), 'hh:mm') || '');//.selectmenu('refresh');
                            $('#selWednesdayDinnerBreakFromDay').val($.format.date(new Date(vRows[0].WednesdayDinnerBreakFrom), 'a') || '');//.selectmenu('refresh');
                            $('#selWednesdayDinnerBreakTo').val($.format.date(new Date(vRows[0].WednesdayDinnerBreakTo), 'hh:mm') || '');//.selectmenu('refresh');
                            $('#selWednesdayLunchDinnerToDay').val($.format.date(new Date(vRows[0].WednesdayLunchDinnerTo), 'a') || '');//.selectmenu('refresh');
                        }

                        if (IsDate(vRows[0].ThursdayFrom)) {
                            $('#selThursdayFrom').val($.format.date(new Date(vRows[0].ThursdayFrom), 'hh:mm') || '');//.selectmenu('refresh');
                            $('#selThursdayFromDay').val($.format.date(new Date(vRows[0].ThursdayFrom), 'a') || '');//.selectmenu('refresh');
                            $('#selThursdayTo').val($.format.date(new Date(vRows[0].ThursdayTo), 'hh:mm') || '');//.selectmenu('refresh');
                            $('#selThursdayToDay').val($.format.date(new Date(vRows[0].ThursdayTo), 'a') || '');//.selectmenu('refresh');
                        }
                        if (IsDate(vRows[0].ThursdayLunchBreakFrom)) {
                            $('#selThursdayLunchBreakFrom').val($.format.date(new Date(vRows[0].ThursdayLunchBreakFrom), 'hh:mm') || '');//.selectmenu('refresh');
                            $('#selThursdayLunchBreakFromDay').val($.format.date(new Date(vRows[0].ThursdayLunchBreakFrom), 'a') || '');//.selectmenu('refresh');
                            $('#selThursdayLunchBreakTo').val($.format.date(new Date(vRows[0].ThursdayLunchBreakTo), 'hh:mm') || '');//.selectmenu('refresh');
                            $('#selThursdayLunchBreakToDay').val($.format.date(new Date(vRows[0].ThursdayLunchBreakTo), 'a') || '');//.selectmenu('refresh');
                        }
                        if (IsDate(vRows[0].ThursdayDinnerBreakFrom)) {
                            $('#selThursdayDinnerBreakFrom').val($.format.date(new Date(vRows[0].ThursdayDinnerBreakFrom), 'hh:mm') || '');//.selectmenu('refresh');
                            $('#selThursdayDinnerBreakFromDay').val($.format.date(new Date(vRows[0].ThursdayDinnerBreakFrom), 'a') || '');//.selectmenu('refresh');
                            $('#selThursdayDinnerBreakTo').val($.format.date(new Date(vRows[0].ThursdayDinnerBreakTo), 'hh:mm') || '');//.selectmenu('refresh');
                            $('#selThursdayLunchDinnerToDay').val($.format.date(new Date(vRows[0].ThursdayLunchDinnerTo), 'a') || '');//.selectmenu('refresh');
                        }

                        if (IsDate(vRows[0].FridayFrom)) {
                            $('#selFridayFrom').val($.format.date(new Date(vRows[0].FridayFrom), 'hh:mm') || '');//.selectmenu('refresh');
                            $('#selFridayFromDay').val($.format.date(new Date(vRows[0].FridayFrom), 'a') || '');//.selectmenu('refresh');
                            $('#selFridayTo').val($.format.date(new Date(vRows[0].FridayTo), 'hh:mm') || '');//.selectmenu('refresh');
                            $('#selFridayToDay').val($.format.date(new Date(vRows[0].FridayTo), 'a') || '');//.selectmenu('refresh');
                        }
                        if (IsDate(vRows[0].FridayLunchBreakFrom)) {
                            $('#selFridayLunchBreakFrom').val($.format.date(new Date(vRows[0].FridayLunchBreakFrom), 'hh:mm') || '');//.selectmenu('refresh');
                            $('#selFridayLunchBreakFromDay').val($.format.date(new Date(vRows[0].FridayLunchBreakFrom), 'a') || '');//.selectmenu('refresh');
                            $('#selFridayLunchBreakTo').val($.format.date(new Date(vRows[0].FridayLunchBreakTo), 'hh:mm') || '');//.selectmenu('refresh');
                            $('#selFridayLunchBreakToDay').val($.format.date(new Date(vRows[0].FridayLunchBreakTo), 'a') || '');//.selectmenu('refresh');
                        }
                        if (IsDate(vRows[0].FridayDinnerBreakFrom)) {
                            $('#selFridayDinnerBreakFrom').val($.format.date(new Date(vRows[0].FridayDinnerBreakFrom), 'hh:mm') || '');//.selectmenu('refresh');
                            $('#selFridayDinnerBreakFromDay').val($.format.date(new Date(vRows[0].FridayDinnerBreakFrom), 'a') || '');//.selectmenu('refresh');
                            $('#selFridayDinnerBreakTo').val($.format.date(new Date(vRows[0].FridayDinnerBreakTo), 'hh:mm') || '');//.selectmenu('refresh');
                            $('#selFridayLunchDinnerToDay').val($.format.date(new Date(vRows[0].FridayLunchDinnerTo), 'a') || '');//.selectmenu('refresh');
                        }

                        if (IsDate(vRows[0].SaturdayFrom)) {
                            $('#selSaturdayFrom').val($.format.date(new Date(vRows[0].SaturdayFrom), 'hh:mm') || '');//.selectmenu('refresh');
                            $('#selSaturdayFromDay').val($.format.date(new Date(vRows[0].SaturdayFrom), 'a') || '');//.selectmenu('refresh');
                            $('#selSaturdayTo').val($.format.date(new Date(vRows[0].SaturdayTo), 'hh:mm') || '');//.selectmenu('refresh');
                            $('#selSaturdayToDay').val($.format.date(new Date(vRows[0].SaturdayTo), 'a') || '');//.selectmenu('refresh');
                        }
                        if (IsDate(vRows[0].SaturdayLunchBreakFrom)) {
                            $('#selSaturdayLunchBreakFrom').val($.format.date(new Date(vRows[0].SaturdayLunchBreakFrom), 'hh:mm') || '');//.selectmenu('refresh');
                            $('#selSaturdayLunchBreakFromDay').val($.format.date(new Date(vRows[0].SaturdayLunchBreakFrom), 'a') || '');//.selectmenu('refresh');
                            $('#selSaturdayLunchBreakTo').val($.format.date(new Date(vRows[0].SaturdayLunchBreakTo), 'hh:mm') || '');//.selectmenu('refresh');
                            $('#selSaturdayLunchBreakToDay').val($.format.date(new Date(vRows[0].SaturdayLunchBreakTo), 'a') || '');//.selectmenu('refresh');
                        }
                        if (IsDate(vRows[0].SaturdayDinnerBreakFrom)) {
                            $('#selSaturdayDinnerBreakFrom').val($.format.date(new Date(vRows[0].SaturdayDinnerBreakFrom), 'hh:mm') || '');//.selectmenu('refresh');
                            $('#selSaturdayDinnerBreakFromDay').val($.format.date(new Date(vRows[0].SaturdayDinnerBreakFrom), 'a') || '');//.selectmenu('refresh');
                            $('#selSaturdayDinnerBreakTo').val($.format.date(new Date(vRows[0].SaturdayDinnerBreakTo), 'hh:mm') || '');//.selectmenu('refresh');
                            $('#selSaturdayLunchDinnerToDay').val($.format.date(new Date(vRows[0].SaturdayLunchDinnerTo), 'a') || '');//.selectmenu('refresh');
                        }

                        if (IsDate(vRows[0].SundayFrom)) {
                            $('#selSundayFrom').val($.format.date(new Date(vRows[0].SundayFrom), 'hh:mm') || '');//.selectmenu('refresh');
                            $('#selSundayFromDay').val($.format.date(new Date(vRows[0].SundayFrom), 'a') || '');//.selectmenu('refresh');
                            $('#selSundayTo').val($.format.date(new Date(vRows[0].SundayTo), 'hh:mm') || '');//.selectmenu('refresh');
                            $('#selSundayToDay').val($.format.date(new Date(vRows[0].SundayTo), 'a') || '');//.selectmenu('refresh');
                        }
                        if (IsDate(vRows[0].SundayLunchBreakFrom)) {
                            $('#selSundayLunchBreakFrom').val($.format.date(new Date(vRows[0].SundayLunchBreakFrom), 'hh:mm') || '');//.selectmenu('refresh');
                            $('#selSundayLunchBreakFromDay').val($.format.date(new Date(vRows[0].SundayLunchBreakFrom), 'a') || '');//.selectmenu('refresh');
                            $('#selSundayLunchBreakTo').val($.format.date(new Date(vRows[0].SundayLunchBreakTo), 'hh:mm') || '');//.selectmenu('refresh');
                            $('#selSundayLunchBreakToDay').val($.format.date(new Date(vRows[0].SundayLunchBreakTo), 'a') || '');//.selectmenu('refresh');
                        }
                        if (IsDate(vRows[0].SundayDinnerBreakFrom)) {
                            $('#selSundayDinnerBreakFrom').val($.format.date(new Date(vRows[0].SundayDinnerBreakFrom), 'hh:mm') || '');//.selectmenu('refresh');
                            $('#selSundayDinnerBreakFromDay').val($.format.date(new Date(vRows[0].SundayDinnerBreakFrom), 'a') || '');//.selectmenu('refresh');
                            $('#selSundayDinnerBreakTo').val($.format.date(new Date(vRows[0].SundayDinnerBreakTo), 'hh:mm') || '');//.selectmenu('refresh');
                            $('#selSundayLunchDinnerToDay').val($.format.date(new Date(vRows[0].SundayLunchDinnerTo), 'a') || '');//.selectmenu('refresh');
                        }
                    }
                }
            });
        }

        function SaveTime() {
            
            if ($('#txtName').val() == '') {
                NewMessageBox('Please enter valid name for this schedule.');
                return;
            }


            if (!(($('#selMondayFrom').val() == '' && $('#selMondayFromDay').val() == '' && $('#selMondayTo').val() == '' && $('#selMondayToDay').val() == '') ||
                    ($('#selMondayFrom').val() != '' && $('#selMondayFromDay').val() != '' && $('#selMondayTo').val() != '' && $('#selMondayToDay').val() != ''))
                    ) {
                NewMessageBox('Your monday schedule is incomplete. Please fill a valid time.');
                return;
            }

            if(!(($('#selMondayLunchBreakFrom').val() == '' && $('#selMondayLunchBreakFromDay').val() == '' && $('#selMondayLunchBreakTo').val() == '' && $('#selMondayLunchBreakToDay').val() == '') ||
                    ($('#selMondayLunchBreakFrom').val() != '' && $('#selMondayLunchBreakFromDay').val() != '' && $('#selMondayLunchBreakTo').val() != '' && $('#selMondayLunchBreakToDay').val() != ''))
                    ) {
                NewMessageBox('Your monday lunch break schedule is incomplete. Please fill a valid time.');
                return;
            }

            if(!(($('#selMondayDinnerBreakFrom').val() == '' && $('#selMondayDinnerBreakFromDay').val() == '' && $('#selMondayDinnerBreakTo').val() == '' && $('#selMondayLunchDinnerToDay').val() == '') ||
                ($('#selMondayDinnerBreakFrom').val() != '' && $('#selMondayDinnerBreakFromDay').val() != '' && $('#selMondayDinnerBreakTo').val() != '' && $('#selMondayLunchDinnerToDay').val() != ''))
                ) {
                NewMessageBox('Your monday dinner break schedule is incomplete. Please fill a valid time.');
                return; 
            }

            if (!(($('#selTuesdayFrom').val() == '' && $('#selTuesdayFromDay').val() == '' && $('#selTuesdayTo').val() == '' && $('#selTuesdayToDay').val() == '') ||
                    ($('#selTuesdayFrom').val() != '' && $('#selTuesdayFromDay').val() != '' && $('#selTuesdayTo').val() != '' && $('#selTuesdayToDay').val() != ''))
                    ) {
                NewMessageBox('Your tuesday schedule is incomplete. Please fill valid time.');
                return;
            }

            if(!(($('#selTuesdayLunchBreakFrom').val() == '' && $('#selTuesdayLunchBreakFromDay').val() == '' && $('#selTuesdayLunchBreakTo').val() == '' && $('#selTuesdayLunchBreakToDay').val() == '') ||
                    ($('#selTuesdayLunchBreakFrom').val() != '' && $('#selTuesdayLunchBreakFromDay').val() != '' && $('#selTuesdayLunchBreakTo').val() != '' && $('#selTuesdayLunchBreakToDay').val() != ''))
                    ) {
                NewMessageBox('Your tuesday lunch break schedule is incomplete. Please fill a valid time.');
                return;
            }

            if(!(($('#selTuesdayDinnerBreakFrom').val() == '' && $('#selTuesdayDinnerBreakFromDay').val() == '' && $('#selTuesdayDinnerBreakTo').val() == '' && $('#selTuesdayLunchDinnerToDay').val() == '') ||
                ($('#selTuesdayDinnerBreakFrom').val() != '' && $('#selTuesdayDinnerBreakFromDay').val() != '' && $('#selTuesdayDinnerBreakTo').val() != '' && $('#selTuesdayLunchDinnerToDay').val() != ''))
                ) {
                NewMessageBox('Your tuesday dinner break schedule is incomplete. Please fill a valid time.');
                return;
            }

            if (!(($('#selWednesdayFrom').val() == '' && $('#selWednesdayFromDay').val() == '' && $('#selWednesdayTo').val() == '' && $('#selWednesdayToDay').val() == '') ||
                    ($('#selWednesdayFrom').val() != '' && $('#selWednesdayFromDay').val() != '' && $('#selWednesdayTo').val() != '' && $('#selWednesdayToDay').val() != ''))
                    ) {
                NewMessageBox('Your wednesday schedule is incomplete. Please fill valid time.');
                return;
            }

            if(!(($('#selWednesdayLunchBreakFrom').val() == '' && $('#selWednesdayLunchBreakFromDay').val() == '' && $('#selWednesdayLunchBreakTo').val() == '' && $('#selWednesdayLunchBreakToDay').val() == '') ||
                    ($('#selWednesdayLunchBreakFrom').val() != '' && $('#selWednesdayLunchBreakFromDay').val() != '' && $('#selWednesdayLunchBreakTo').val() != '' && $('#selWednesdayLunchBreakToDay').val() != ''))
                    ) {
                NewMessageBox('Your wednesday lunch break schedule is incomplete. Please fill a valid time.');
                return;
            }

            if(!(($('#selWednesdayDinnerBreakFrom').val() == '' && $('#selWednesdayDinnerBreakFromDay').val() == '' && $('#selWednesdayDinnerBreakTo').val() == '' && $('#selWednesdayLunchDinnerToDay').val() == '') ||
                ($('#selWednesdayDinnerBreakFrom').val() != '' && $('#selWednesdayDinnerBreakFromDay').val() != '' && $('#selWednesdayDinnerBreakTo').val() != '' && $('#selWednesdayLunchDinnerToDay').val() != ''))
                ) {
                NewMessageBox('Your wednesday dinner break schedule is incomplete. Please fill a valid time.');
                return;
            }

            if (!(($('#selThursdayFrom').val() == '' && $('#selThursdayFromDay').val() == '' && $('#selThursdayTo').val() == '' && $('#selThursdayToDay').val() == '') ||
                    ($('#selThursdayFrom').val() != '' && $('#selThursdayFromDay').val() != '' && $('#selThursdayTo').val() != '' && $('#selThursdayToDay').val() != ''))                                       
                    ) {
                NewMessageBox('Your thrusday schedule is incomplete. Please fill valid time.');
                return;
            }

            if(!(($('#selThursdayLunchBreakFrom').val() == '' && $('#selThursdayLunchBreakFromDay').val() == '' && $('#selThursdayLunchBreakTo').val() == '' && $('#selThursdayLunchBreakToDay').val() == '') ||
                    ($('#selThursdayLunchBreakFrom').val() != '' && $('#selThursdayLunchBreakFromDay').val() != '' && $('#selThursdayLunchBreakTo').val() != '' && $('#selThursdayLunchBreakToDay').val() != ''))
                    ) {
                NewMessageBox('Your thursday lunch break schedule is incomplete. Please fill a valid time.');
                return;
            }

            if(!(($('#selThursdayDinnerBreakFrom').val() == '' && $('#selThursdayDinnerBreakFromDay').val() == '' && $('#selThursdayDinnerBreakTo').val() == '' && $('#selThursdayLunchDinnerToDay').val() == '') ||
                ($('#selThursdayDinnerBreakFrom').val() != '' && $('#selThursdayDinnerBreakFromDay').val() != '' && $('#selThursdayDinnerBreakTo').val() != '' && $('#selThursdayLunchDinnerToDay').val() != ''))
                ) {
                NewMessageBox('Your thursday dinner break schedule is incomplete. Please fill a valid time.');
                return;
            }

            if (!(($('#selFridayFrom').val() == '' && $('#selFridayFromDay').val() == '' && $('#selFridayTo').val() == '' && $('#selFridayToDay').val() == '') ||
                    ($('#selFridayFrom').val() != '' && $('#selFridayFromDay').val() != '' && $('#selFridayTo').val() != '' && $('#selFridayToDay').val() != ''))
                    ) {
                NewMessageBox('Your friday schedule is incomplete. Please fill valid time.');
                return;
            }

            if(!(($('#selFridayLunchBreakFrom').val() == '' && $('#selFridayLunchBreakFromDay').val() == '' && $('#selFridayLunchBreakTo').val() == '' && $('#selFridayLunchBreakToDay').val() == '') ||
                    ($('#selFridayLunchBreakFrom').val() != '' && $('#selFridayLunchBreakFromDay').val() != '' && $('#selFridayLunchBreakTo').val() != '' && $('#selFridayLunchBreakToDay').val() != ''))
                    ) {
                NewMessageBox('Your friday lunch break schedule is incomplete. Please fill a valid time.');
                return;
            }

            if(!(($('#selFridayDinnerBreakFrom').val() == '' && $('#selFridayDinnerBreakFromDay').val() == '' && $('#selFridayDinnerBreakTo').val() == '' && $('#selFridayLunchDinnerToDay').val() == '') ||
                ($('#selFridayDinnerBreakFrom').val() != '' && $('#selFridayDinnerBreakFromDay').val() != '' && $('#selFridayDinnerBreakTo').val() != '' && $('#selFridayLunchDinnerToDay').val() != ''))
                ) {
                NewMessageBox('Your friday dinner break schedule is incomplete. Please fill a valid time.');
                return;
            }

            if (!(($('#selSaturdayFrom').val() == '' && $('#selSaturdayFromDay').val() == '' && $('#selSaturdayTo').val() == '' && $('#selSaturdayToDay').val() == '') ||
                    ($('#selSaturdayFrom').val() != '' && $('#selSaturdayFromDay').val() != '' && $('#selSaturdayTo').val() != '' && $('#selSaturdayToDay').val() != ''))
                    ) {
                NewMessageBox('Your satuday schedule is incomplete. Please fill valid time.');
                return;
            }

            if(!(($('#selSaturdayLunchBreakFrom').val() == '' && $('#selSaturdayLunchBreakFromDay').val() == '' && $('#selSaturdayLunchBreakTo').val() == '' && $('#selSaturdayLunchBreakToDay').val() == '') ||
                    ($('#selSaturdayLunchBreakFrom').val() != '' && $('#selSaturdayLunchBreakFromDay').val() != '' && $('#selSaturdayLunchBreakTo').val() != '' && $('#selSaturdayLunchBreakToDay').val() != ''))
                    ) {
                NewMessageBox('Your saturday lunch break schedule is incomplete. Please fill a valid time.');
                return;
            }

            if(!(($('#selSaturdayDinnerBreakFrom').val() == '' && $('#selSaturdayDinnerBreakFromDay').val() == '' && $('#selSaturdayDinnerBreakTo').val() == '' && $('#selSaturdayLunchDinnerToDay').val() == '') ||
                ($('#selSaturdayDinnerBreakFrom').val() != '' && $('#selSaturdayDinnerBreakFromDay').val() != '' && $('#selSaturdayDinnerBreakTo').val() != '' && $('#selSaturdayLunchDinnerToDay').val() != ''))
                ) {
                NewMessageBox('Your saturday dinner break schedule is incomplete. Please fill a valid time.');
                return;
            }

            if (!(($('#selSundayFrom').val() == '' && $('#selSundayFromDay').val() == '' && $('#selSundayTo').val() == '' && $('#selSundayToDay').val() == '') ||
                    ($('#selSundayFrom').val() != '' && $('#selSundayFromDay').val() != '' && $('#selSundayTo').val() != '' && $('#selSundayToDay').val() != ''))
                    ) {
                NewMessageBox('Your sunday schedule is incomplete. Please fill valid time.');
                return;
            }

            if(!(($('#selSundayLunchBreakFrom').val() == '' && $('#selSundayLunchBreakFromDay').val() == '' && $('#selSundayLunchBreakTo').val() == '' && $('#selSundayLunchBreakToDay').val() == '') ||
                    ($('#selSundayLunchBreakFrom').val() != '' && $('#selSundayLunchBreakFromDay').val() != '' && $('#selSundayLunchBreakTo').val() != '' && $('#selSundayLunchBreakToDay').val() != ''))
                    ) {
                NewMessageBox('Your sunday lunch break schedule is incomplete. Please fill a valid time.');
                return;
            }

            if(!(($('#selSundayDinnerBreakFrom').val() == '' && $('#selSundayDinnerBreakFromDay').val() == '' && $('#selSundayDinnerBreakTo').val() == '' && $('#selSundayLunchDinnerToDay').val() == '') ||
                ($('#selSundayDinnerBreakFrom').val() != '' && $('#selSundayDinnerBreakFromDay').val() != '' && $('#selSundayDinnerBreakTo').val() != '' && $('#selSundayLunchDinnerToDay').val() != ''))
                ) {
                NewMessageBox('Your sunday dinner break schedule is incomplete. Please fill a valid time.');
                return;
            }

            var data = { IsNew: IsNew,
                ID: ID,
                Name: $('#txtName').val(),
                Description: $('#txtDescription').val(),
                MondayFrom: '1/1/01 ' + $('#selMondayFrom').val() + ':00 ' + $('#selMondayFromDay').val(),
                MondayTo: '1/1/01 ' + $('#selMondayTo').val() + ':00 ' + $('#selMondayToDay').val(),

                MondayLunchBreakFrom: '1/1/01 ' + $('#selMondayLunchBreakFrom').val() + ':00 ' + $('#selMondayLunchBreakFromDay').val(),
                MondayLunchBreakTo: '1/1/01 ' + $('#selMondayLunchBreakTo').val() + ':00 ' + $('#selMondayLunchBreakToDay').val(),
                MondayDinnerBreakFrom: '1/1/01 ' + $('#selMondayDinnerBreakFrom').val() + ':00 ' + $('#selMondayDinnerBreakFromDay').val(),
                MondayDinnerBreakTo: '1/1/01 ' + $('#selMondayDinnerBreakTo').val() + ':00 ' + $('#selMondayLunchDinnerToDay').val(),

                TuesdayFrom: '1/1/01 ' + $('#selTuesdayFrom').val() + ':00 ' + $('#selTuesdayFromDay').val(),
                TuesdayTo: '1/1/01 ' + $('#selTuesdayTo').val() + ':00 ' + $('#selTuesdayToDay').val(),

                TuesdayLunchBreakFrom: '1/1/01 ' + $('#selTuesdayLunchBreakFrom').val() + ':00 ' + $('#selTuesdayLunchBreakFromDay').val(),
                TuesdayLunchBreakTo: '1/1/01 ' + $('#selTuesdayLunchBreakTo').val() + ':00 ' + $('#selTuesdayLunchBreakToDay').val(),
                TuesdayDinnerBreakFrom: '1/1/01 ' + $('#selTuesdayDinnerBreakFrom').val() + ':00 ' + $('#selTuesdayDinnerBreakFromDay').val(),
                TuesdayDinnerBreakTo: '1/1/01 ' + $('#selTuesdayDinnerBreakTo').val() + ':00 ' + $('#selTuesdayLunchDinnerToDay').val(),

                WednesdayFrom: '1/1/01 ' + $('#selWednesdayFrom').val() + ':00 ' + $('#selWednesdayFromDay').val(),
                WednesdayTo: '1/1/01 ' + $('#selWednesdayTo').val() + ':00 ' + $('#selWednesdayToDay').val(),

                WednesdayLunchBreakFrom: '1/1/01 ' + $('#selWednesdayLunchBreakFrom').val() + ':00 ' + $('#selWednesdayLunchBreakFromDay').val(),
                WednesdayLunchBreakTo: '1/1/01 ' + $('#selWednesdayLunchBreakTo').val() + ':00 ' + $('#selWednesdayLunchBreakToDay').val(),
                WednesdayDinnerBreakFrom: '1/1/01 ' + $('#selWednesdayDinnerBreakFrom').val() + ':00 ' + $('#selWednesdayDinnerBreakFromDay').val(),
                WednesdayDinnerBreakTo: '1/1/01 ' + $('#selWednesdayDinnerBreakTo').val() + ':00 ' + $('#selWednesdayLunchDinnerToDay').val(),

                ThursdayFrom: '1/1/01 ' + $('#selThursdayFrom').val() + ':00 ' + $('#selThursdayFromDay').val(),
                ThursdayTo: '1/1/01 ' + $('#selThursdayTo').val() + ':00 ' + $('#selThursdayToDay').val(),

                ThursdayLunchBreakFrom: '1/1/01 ' + $('#selThursdayLunchBreakFrom').val() + ':00 ' + $('#selThursdayLunchBreakFromDay').val(),
                ThursdayLunchBreakTo: '1/1/01 ' + $('#selThursdayLunchBreakTo').val() + ':00 ' + $('#selThursdayLunchBreakToDay').val(),
                ThursdayDinnerBreakFrom: '1/1/01 ' + $('#selThursdayDinnerBreakFrom').val() + ':00 ' + $('#selThursdayDinnerBreakFromDay').val(),
                ThursdayDinnerBreakTo: '1/1/01 ' + $('#selThursdayDinnerBreakTo').val() + ':00 ' + $('#selThursdayLunchDinnerToDay').val(),

                FridayFrom: '1/1/01 ' + $('#selFridayFrom').val() + ':00 ' + $('#selFridayFromDay').val(),
                FridayTo: '1/1/01 ' + $('#selFridayTo').val() + ':00 ' + $('#selFridayToDay').val(),

                FridayLunchBreakFrom: '1/1/01 ' + $('#selFridayLunchBreakFrom').val() + ':00 ' + $('#selFridayLunchBreakFromDay').val(),
                FridayLunchBreakTo: '1/1/01 ' + $('#selFridayLunchBreakTo').val() + ':00 ' + $('#selFridayLunchBreakToDay').val(),
                FridayDinnerBreakFrom: '1/1/01 ' + $('#selFridayDinnerBreakFrom').val() + ':00 ' + $('#selFridayDinnerBreakFromDay').val(),
                FridayDinnerBreakTo: '1/1/01 ' + $('#selFridayDinnerBreakTo').val() + ':00 ' + $('#selFridayLunchDinnerToDay').val(),

                SaturdayFrom: '1/1/01 ' + $('#selSaturdayFrom').val() + ':00 ' + $('#selSaturdayFromDay').val(),
                SaturdayTo: '1/1/01 ' + $('#selSaturdayTo').val() + ':00 ' + $('#selSaturdayToDay').val(),

                SaturdayLunchBreakFrom: '1/1/01 ' + $('#selSaturdayLunchBreakFrom').val() + ':00 ' + $('#selSaturdayLunchBreakFromDay').val(),
                SaturdayLunchBreakTo: '1/1/01 ' + $('#selSaturdayLunchBreakTo').val() + ':00 ' + $('#selSaturdayLunchBreakToDay').val(),
                SaturdayDinnerBreakFrom: '1/1/01 ' + $('#selSaturdayDinnerBreakFrom').val() + ':00 ' + $('#selSaturdayDinnerBreakFromDay').val(),
                SaturdayDinnerBreakTo: '1/1/01 ' + $('#selSaturdayDinnerBreakTo').val() + ':00 ' + $('#selSaturdayLunchDinnerToDay').val(),

                SundayFrom: '1/1/01 ' + $('#selSundayFrom').val() + ':00 ' + $('#selSundayFromDay').val(),
                SundayTo: '1/1/01 ' + $('#selSundayTo').val() + ':00 ' + $('#selSundayToDay').val(),
                
                SundayLunchBreakFrom: '1/1/01 ' + $('#selSundayLunchBreakFrom').val() + ':00 ' + $('#selSundayLunchBreakFromDay').val(),
                SundayLunchBreakTo: '1/1/01 ' + $('#selSundayLunchBreakTo').val() + ':00 ' + $('#selSundayLunchBreakToDay').val(),
                SundayDinnerBreakFrom: '1/1/01 ' + $('#selSundayDinnerBreakFrom').val() + ':00 ' + $('#selSundayDinnerBreakFromDay').val(),
                SundayDinnerBreakTo: '1/1/01 ' + $('#selSundayDinnerBreakTo').val() + ':00 ' + $('#selSundayLunchDinnerToDay').val(),

            };

            AjaxSend({
                url: 'Organization/SaveWorkingHours',
                data: data,
                success: function (vData) {
                    if (vData.Results.indexOf('error') > 0) {
                        NewMessageBox('An error occured while saving work schedule.');
                    } else {
                        if (IsAssignEmployee == 1) {
                            sessionStorage.ScheduleID = vData.Results;
//                            window.location = MergeUrlParameters({
//                                OrgID: sessionStorage.OrgID,
//                                IsMobile: CBool(IsMobile) ? 1 : 0
//                            }, '../Organization/OrganizationEmployee');
                            if(isOrgEmpPageLoaded == ""){
                                AjaxSend({
                                    ExactUrl: '/Organization/OrganizationEmployee?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile,
                                    type: 'GET',
                                    dataType: 'text',
                                    success: function (vData) {
                                        isOrgEmpPageLoaded = vData;
                                        $('#UniversalDiv').empty();
                                        $('#UniversalDiv').append(vData).trigger('create');
                                        $('#pageOrgEmployee').show();
                                        setTimeout(function(){
                                            OrgEmp.GetEmpList();
                                        }, 2000)
                                        App.Deferred.fulfill('FQ26E5TP76');
                                    }
                                });
                            }
                            else {
                                $('#UniversalDiv').empty();
                                $('#UniversalDiv').append(isOrgEmpPageLoaded).trigger('create');
                                $('#pageOrgEmployee').show();
                                OrgEmp.GetEmpList();
                            }
                        } else {
                            WebToast('Successfully saved');
                            sessionStorage.ScheduleID = vData.Results;
//                            window.location = MergeUrlParameters({
//                                OrgID: sessionStorage.OrgID,
//                                IsMobile: CBool(IsMobile) ? 1 : 0
//                            }, '../Organization/OrganizationEmployeeWorkingHours');
                            $('#pageOrganizationEmployeeSchedule').show();
                            $('#pageNewSchedule').hide();
                            GetAllSchedule();
                        }
                    }
                }
            });
        }

        function pageShowSchedule() {
            if (ValidPrevPage(arguments)) {
                ClearTime();
            }
        }

        function ValidPrevPage(args) {
            try {
                return args[1].prevPage.attr('data-role') != 'dialog';
            } catch (e) {
                return true;
            };

        };

        function ClearTime() {

            $('#selMondayFrom').val('hh:mm');//.selectmenu('refresh');
            $('#selMondayFromDay').val('--');//.selectmenu('refresh');
            $('#selMondayTo').val('hh:mm');//.selectmenu('refresh');
            $('#selMondayToDay').val('--');//.selectmenu('refresh');

            $('#selMondayLunchBreakFrom').val('hh:mm');//.selectmenu('refresh');
            $('#selMondayLunchBreakFromDay').val('--');//.selectmenu('refresh');
            $('#selMondayLunchBreakTo').val('hh:mm');//.selectmenu('refresh');
            $('#selMondayLunchBreakToDay').val('--');//.selectmenu('refresh');

            $('#selMondayDinnerBreakFrom').val('hh:mm');//.selectmenu('refresh');
            $('#selMondayDinnerBreakFromDay').val('--');//.selectmenu('refresh');
            $('#selMondayDinnerBreakTo').val('hh:mm');//.selectmenu('refresh');
            $('#selMondayLunchDinnerToDay').val('--');//.selectmenu('refresh');

            $('#selTuesdayFrom').val('hh:mm');//.selectmenu('refresh');
            $('#selTuesdayFromDay').val('--');//.selectmenu('refresh');
            $('#selTuesdayTo').val('hh:mm');//.selectmenu('refresh');
            $('#selTuesdayToDay').val('--');//.selectmenu('refresh');

            $('#selTuesdayLunchBreakFrom').val('hh:mm');//.selectmenu('refresh');
            $('#selTuesdayLunchBreakFromDay').val('--');//.selectmenu('refresh');
            $('#selTuesdayLunchBreakTo').val('hh:mm');//.selectmenu('refresh');
            $('#selTuesdayLunchBreakToDay').val('--');//.selectmenu('refresh');

            $('#selTuesdayDinnerBreakFrom').val('hh:mm');//.selectmenu('refresh');
            $('#selTuesdayDinnerBreakFromDay').val('--');//.selectmenu('refresh');
            $('#selTuesdayDinnerBreakTo').val('hh:mm');//.selectmenu('refresh');
            $('#selTuesdayLunchDinnerToDay').val('--');//.selectmenu('refresh');

            $('#selWednesdayFrom').val('hh:mm');//.selectmenu('refresh');
            $('#selWednesdayFromDay').val('--');//.selectmenu('refresh');
            $('#selWednesdayTo').val('hh:mm');//.selectmenu('refresh');
            $('#selWednesdayToDay').val('--');//.selectmenu('refresh');
            
            $('#selWednesdayLunchBreakFrom').val('hh:mm');//.selectmenu('refresh');
            $('#selWednesdayLunchBreakFromDay').val('--');//.selectmenu('refresh');
            $('#selWednesdayLunchBreakTo').val('hh:mm');//.selectmenu('refresh');
            $('#selWednesdayLunchBreakToDay').val('--');//.selectmenu('refresh');

            $('#selWednesdayDinnerBreakFrom').val('hh:mm');//.selectmenu('refresh');
            $('#selWednesdayDinnerBreakFromDay').val('--');//.selectmenu('refresh');
            $('#selWednesdayDinnerBreakTo').val('hh:mm');//.selectmenu('refresh');
            $('#selWednesdayLunchDinnerToDay').val('--');//.selectmenu('refresh');

            $('#selThursdayFrom').val('hh:mm');//.selectmenu('refresh');
            $('#selThursdayFromDay').val('--');//.selectmenu('refresh');
            $('#selThursdayTo').val('hh:mm');//.selectmenu('refresh');
            $('#selThursdayToDay').val('--');//.selectmenu('refresh');

            $('#selThursdayLunchBreakFrom').val('hh:mm');//.selectmenu('refresh');
            $('#selThursdayLunchBreakFromDay').val('--');//.selectmenu('refresh');
            $('#selThursdayLunchBreakTo').val('hh:mm');//.selectmenu('refresh');
            $('#selThursdayLunchBreakToDay').val('--');//.selectmenu('refresh');

            $('#selThursdayDinnerBreakFrom').val('hh:mm');//.selectmenu('refresh');
            $('#selThursdayDinnerBreakFromDay').val('--');//.selectmenu('refresh');
            $('#selThursdayDinnerBreakTo').val('hh:mm');//.selectmenu('refresh');
            $('#selThursdayLunchDinnerToDay').val('--');//.selectmenu('refresh');

            $('#selFridayFrom').val('hh:mm');//.selectmenu('refresh');
            $('#selFridayFromDay').val('--');//.selectmenu('refresh');
            $('#selFridayTo').val('hh:mm');//.selectmenu('refresh');
            $('#selFridayToDay').val('--');//.selectmenu('refresh');

            $('#selFridayLunchBreakFrom').val('hh:mm');//.selectmenu('refresh');
            $('#selFridayLunchBreakFromDay').val('--');//.selectmenu('refresh');
            $('#selFridayLunchBreakTo').val('hh:mm');//.selectmenu('refresh');
            $('#selFridayLunchBreakToDay').val('--');//.selectmenu('refresh');

            $('#selFridayDinnerBreakFrom').val('hh:mm');//.selectmenu('refresh');
            $('#selFridayDinnerBreakFromDay').val('--');//.selectmenu('refresh');
            $('#selFridayDinnerBreakTo').val('hh:mm');//.selectmenu('refresh');
            $('#selFridayLunchDinnerToDay').val('--');//.selectmenu('refresh');

            $('#selSaturdayFrom').val('hh:mm');//.selectmenu('refresh');
            $('#selSaturdayFromDay').val('--');//.selectmenu('refresh');
            $('#selSaturdayTo').val('hh:mm');//.selectmenu('refresh');
            $('#selSaturdayToDay').val('--');//.selectmenu('refresh');

            $('#selSaturdayLunchBreakFrom').val('hh:mm');//.selectmenu('refresh');
            $('#selSaturdayLunchBreakFromDay').val('--');//.selectmenu('refresh');
            $('#selSaturdayLunchBreakTo').val('hh:mm');//.selectmenu('refresh');
            $('#selSaturdayLunchBreakToDay').val('--');//.selectmenu('refresh');

            $('#selSaturdayDinnerBreakFrom').val('hh:mm');//.selectmenu('refresh');
            $('#selSaturdayDinnerBreakFromDay').val('--');//.selectmenu('refresh');
            $('#selSaturdayDinnerBreakTo').val('hh:mm');//.selectmenu('refresh');
            $('#selSaturdayLunchDinnerToDay').val('--');//.selectmenu('refresh');

            $('#selSundayFrom').val('hh:mm');//.selectmenu('refresh');
            $('#selSundayFromDay').val('--');//.selectmenu('refresh');
            $('#selSundayTo').val('hh:mm');//.selectmenu('refresh');
            $('#selSundayToDay').val('--');//.selectmenu('refresh');

            $('#selSundayLunchBreakFrom').val('hh:mm');//.selectmenu('refresh');
            $('#selSundayLunchBreakFromDay').val('--');//.selectmenu('refresh');
            $('#selSundayLunchBreakTo').val('hh:mm');//.selectmenu('refresh');
            $('#selSundayLunchBreakToDay').val('--');//.selectmenu('refresh');

            $('#selSundayDinnerBreakFrom').val('hh:mm');//.selectmenu('refresh');
            $('#selSundayDinnerBreakFromDay').val('--');//.selectmenu('refresh');
            $('#selSundayDinnerBreakTo').val('hh:mm');//.selectmenu('refresh');
            $('#selSundayLunchDinnerToDay').val('--');//.selectmenu('refresh');
        }
    });
