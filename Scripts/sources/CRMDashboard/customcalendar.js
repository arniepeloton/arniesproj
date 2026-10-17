require(['common']
, function () {
    $(document).ready(function () {

        $.fn.customcalendar = function (eventsData) {
            var label_months = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];
            var label_day = ["Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat"];
            var counter_day = 0;

            var divCalendar = $(this);

            var current_fulldate = new Date();
            var current_month = current_fulldate.getMonth();
            var current_year = current_fulldate.getFullYear();
            var eventSetDate;
            var isFinishSettingPMTime;
            var isValidSetTime = true;
            var FromHour;
            var FromMinute;
            var FromMeredian;

            var ToHour;
            var ToMinute;
            var ToMeredian;

            if(eventsData == undefined){
                eventsData = [];
                DisplayCalendar(0, 0, current_month, current_year);
            }
            
            DisplayEvents(eventsData);

            $('body').on('click', '.btnSetEvent', function () {
                var chosen_month = $(this).attr("month");
                var chosen_date = $(this).attr("date");
                var chosen_year = $(this).attr("year");
                var chosen_day = $(this).attr("day");

                eventSetDate = chosen_month + ' ' + chosen_date + ', ' + chosen_year;

                sessionStorage.CurrentCalendarDate = JSON.stringify({
                    From: $.format.date(new Date(chosen_month + ' 1, ' + chosen_year), 'MM-dd-yyyy'),
                    To: $.format.date(new Date(new Date(eventSetDate).setMonth(new Date(eventSetDate).getMonth() + 1, 0)), 'MM-dd-yyyy')
                })

                sessionStorage.CurrentCalendarSelectedDate = eventSetDate;

                if ($(this).find('div').find('ul').find('li').length > 0) {
                    var dis = $(this);
                    PromptUserAccess('View Schedule', {
                        OnClickOKSuccess: function () {
                            var arrListData = [];

                            dis.find('div').find('ul').find('li:not(.exclude)').each(function () {
                                arrListData.push($(this).data())
                            })
                            DisplayListOfEvents(arrListData);
                        }
                    });                    
                }
                else {                    
                    PromptUserAccess('Add Schedule', {
                        OnClickOKSuccess: function () {
                            if ((new Date(new Date(eventSetDate).toDateString()) - new Date(new Date().toDateString())) < 0) {
                                WebToast('Invalid schedule date.', 'top');
                                return;
                            }

                            sessionStorage.StateSched = 'add';
                            $('#pageCRMNewSched').pageshow();
                            $('#txtCRMDateTimeFrom').val(eventSetDate);
                            $('#txtCRMDateTimeTo').val(eventSetDate);
                        }
                    });
                }
            })

            function DisplayEvents(events) {
                var removeListEvents = true;
                var previousListDate = "";
                var currentListDate = "";
                var countEventList;

                $('.listEvent').empty();

                for (var i = 0; i < events.length; i++) {
                    var events_fulldate = new Date(events[i].eventWhen);
                    var event_month = events_fulldate.getMonth();
                    var event_date = events_fulldate.getDate();
                    var event_year = events_fulldate.getFullYear();
                    var defaultColor = {};

                    if(events[i].Detail.SchedActionName == "Meeting"){
                        defaultColor = {
                            'background-color': '#FFC107',
                            'border' : '1px solid #FF9800'
                        }
                    }
                    else if(events[i].Detail.SchedActionName == "Call"){
                        defaultColor = {
                            'background-color': '#5A6BC5',
                            'border' : '1px solid #1D2E90',
                            'color' : '#FFF'
                        }
                    }
                    else if(events[i].Detail.SchedActionName == "Email"){
                        defaultColor = {
                            'background-color': '#2196F3',
                            'border' : '1px solid #2A5E75',
                            'color' : '#FFF'
                        }
                    }
                    else if(events[i].Detail.SchedActionName == "SMS"){
                        defaultColor = {
                            'background-color': '#4CAF50',
                            'border' : '1px solid #233C24'
                        }
                    }

                    currentListDate = event_month + " " + event_date + ", " + event_year;

                   if (previousListDate != currentListDate) {

                       if (removeListEvents) {
                           $('#list-' + label_months[event_month] + '-' + event_date + '-' + event_year).empty();
                           removeListEvents = false;
                       }
                   }
                   else {
                       //removeListEvents = true;
                       countEventList = countEventList + 1;
                   }

                   $('#list-' + label_months[event_month] + '-' + event_date + '-' + event_year)
                       .append($('<li>')
                                    .data(events[i])
                                    .append($('<div>')
                                       .addClass('calendar_eventStyleList')
                                       .css(defaultColor)
                                       .text(events[i].eventName)
                                   )
                       )

                   countEventList = $('#list-' + label_months[event_month] + '-' + event_date + '-' + event_year + ' li:not(.exclude)').length;
                   if (countEventList > 2) {
                       $('#list-' + label_months[event_month] + '-' + event_date + '-' + event_year + ' li:not(:nth-child(1)):not(:nth-child(2))')
                           .css('visiblity', 'hidden')
                           .css('display', 'none');

                       $('#list-' + label_months[event_month] + '-' + event_date + '-' + event_year + ':last-child')
                           .append($('<li>')
                                .addClass('exclude')
                                .append($('<a class="custom-link">')
                                       .attr('href', '#')
                                       .html('<span style="font-size:12px;">+' + (countEventList - 2) + ' more</span>')
                                   )
                               )
                   }

                    previousListDate = event_month + " " + event_date + ", " + event_year;
                }
            }

            function DisplayCalendar(ChosenMonth, ChosenYear, CurrentMonth, CurrentYear) {
                CurrentMonth = CurrentMonth + ChosenMonth;
                CurrentYear = CurrentYear + ChosenYear;

                var display_fulldate = new Date(CurrentYear, CurrentMonth);
                var display_fullyear = display_fulldate.getFullYear();
                var display_month = display_fulldate.getMonth();
                var display_date = display_fulldate.getDate();
                var display_monthname = label_months[display_fulldate.getMonth()];
                var display_dayofweek = display_fulldate.getDay(); // first day of the month

                display_fulldate.setMonth(CurrentMonth + 1, 0); // to get the last date of the month

                var display_lastdate = display_fulldate.getDate();

                var date = 1;

                var show_calendar = $(divCalendar);
                var calendar_table = $('<table>').css('table-layout', 'fixed').css('width', '100%').addClass('customcalendar');

                show_calendar.empty();

                var calendar_primaryheader = $('<thead>')
									.append(
										$('<tr>')
										.append($('<th>')
											.append($('<a>')
												.html('&#9001;&#9001;')
												.css('cursor', 'pointer')
												.addClass("calendar_nav")
												.attr('title', 'Previous Year')
												.click(function () {
												    var dateFilter = DisplayCalendar(0, -1, display_month, display_fullyear);
                                                    GetSchedule(dateFilter);
												})
											)
										)
										.append($('<th>')
											.append($('<a>')
												.html('&#9001;')
												.css('cursor', 'pointer')
												.addClass("calendar_nav")
												.attr('title', 'Previous Month')
												.click(function () {
												    var dateFilter = DisplayCalendar(-1, 0, display_month, display_fullyear);
												    GetSchedule(dateFilter);
												})
											)
										)
										.append($('<th>')
											.css('font-size', '20px')
											.attr('colspan', '3')
											.append($('<a>')
												.text(display_monthname + ' ' + display_fullyear)
											)
										)
										.append($('<th>')
											.append($('<a>')
												.html('&#9002;')
												.css('cursor', 'pointer')
												.addClass("calendar_nav")
												.attr('title', 'Next Month')
												.click(function () {
												    var dateFilter = DisplayCalendar(1, 0, display_month, display_fullyear);
												    GetSchedule(dateFilter);
												})
											)
										)
										.append($('<th>')
											.append($('<a>')
												.html('&#9002;&#9002;')
												.css('cursor', 'pointer')
												.addClass("calendar_nav")
												.attr('title', 'Next Year')
												.click(function () {
												    var dateFilter = DisplayCalendar(0, 1, display_month, display_fullyear);
												    GetSchedule(dateFilter);
												})
											)
										)
									)
									.append(
										$('<tr>')
										.append(
											$('<th>').text('SUN')
										)
										.append(
											$('<th>').text('MON')
										)
										.append(
											$('<th>').text('TUE')
										)
										.append(
											$('<th>').text('WED')
										)
										.append(
											$('<th>').text('THU')
										)
										.append(
											$('<th>').text('FRI')
										)
										.append(
											$('<th>').text('SAT')
										)
									)

                var calendar_body = "";

                // 42 because total number of cells in calendar
                for (var i = 0; i < 42; i++) {
                    if (i % 7 == 0) {
                        calendar_body = calendar_body + "</tr><tr>";
                        counter_day = 0;
                    }

                    if ((i >= display_dayofweek) && (date <= display_lastdate)) {
                        if (date == current_fulldate.getDate() && display_month == current_fulldate.getMonth() && display_fullyear == current_year) {
                            calendar_body = calendar_body + "<td class='hasDate CurrentDate btnSetEvent' month='" + display_monthname + "' date='" + date + "' year='" + display_fullyear + "' day='" + label_day[counter_day] + "'>" +
							"<div class='calendar_cellStyle'>" +
								'<div class="date CurrentDate">' + date + '</div>' +
								"<ul id='list-" + display_monthname + "-" + date + "-" + display_fullyear + "' class='calendar_eventStyle listEvent'>" +
                            // "<li class='btnEvent'><div style='border:1px solid #000; margin-top:1px;'>event 1</div></li><li><div style='border:1px solid #000; margin-top:1px;'>event 2</div></li>"+
								"</ul>" +
							"</div>" +
					"</td>";
                        }
                        else {
                            calendar_body = calendar_body + "<td class='hasDate btnSetEvent' month='" + display_monthname + "' date='" + date + "' year='" + display_fullyear + "' day='" + label_day[counter_day] + "'>" +
							"<div class='calendar_cellStyle'>" +
								'<div class="date">' + date + '</div>' +
								"<ul id='list-" + display_monthname + "-" + date + "-" + display_fullyear + "' class='calendar_eventStyle listEvent'>" +
								"</ul>" +
							"</div>" +
					"</td>";
                        }

                        date = date + 1;
                    }
                    else {
                        calendar_body = calendar_body + "<td></td>";
                    }

                    counter_day = counter_day + 1;
                }

                show_calendar.append(calendar_table
								.append(
									calendar_primaryheader
								)
								.append(
									calendar_body
								)
							)
                sessionStorage.CurrentCalendarDate = JSON.stringify({
                    From: $.format.date(new Date(display_monthname + ' 1, ' + display_fulldate.getFullYear()), 'MM-dd-yyyy'),
                    To: $.format.date(new Date(display_fulldate), 'MM-dd-yyyy')
                })

                return {
                    From: $.format.date(new Date(display_monthname + ' 1, ' + display_fulldate.getFullYear()), 'MM-dd-yyyy'),
                    To: $.format.date(new Date(display_fulldate), 'MM-dd-yyyy')
                }
            }

            function GetSchedule(dates){
                var xhr;
                dates.OrgID = sessionStorage.OrgID;
                var fn = function(){
                    if(xhr && xhr.readystate != 4){
                        xhr.abort();
                    }
                    xhr = AjaxSend({
                        url: 'CRMDashboard/GetSchedule',
                        data: dates,
                        success: function(vData){
                            var arrEvent = [];
                            var vRows = vData.Results.Rows;

                            if(vRows.length > 0){
                                for(var i = 0; i < vRows.length; i++){
                                    var dateFrom = new Date($.format.date(new Date(vRows[i].SchedActionDateFrom), 'MMM dd, yyyy'));
                                    var dateTo = new Date($.format.date(new Date(vRows[i].SchedActionDateTo), 'MMM dd, yyyy'));
                                    var datecompare = dateTo <= dateFrom;

                                    if(datecompare){
                                        arrEvent.push({
                                            Detail: vRows[i],
                                            eventName: vRows[i].Subject,
                                            eventWhen: $.format.date(new Date(dateFrom), 'MMMM dd, yyyy')
                                        })
                                    }
                                    else {
                                        while (dateFrom <= dateTo) {
                                            arrEvent.push({
                                                Detail: vRows[i],
                                                eventName: vRows[i].Subject,
                                                eventWhen: $.format.date(new Date(dateFrom), 'MMMM dd, yyyy')
                                            })
                                            dateFrom.setDate(dateFrom.getDate() + 1);
                                        }
                                    }
                                }
                            }
                            DisplayEvents(arrEvent);
                        }
                    })
                };

                fn();
            }
        }

    })
})