require([
    'common',
    ], function () {

        var ID = '';
        var DeviceID = '';

        var m_DateFrom;
        var m_DateTo;
        var m_Period = '';
        var m_IsDateRange = 0;

        PageShow();        

        $(function () {

//            $('#btnSendData').click(function () {
//                SendGPSDataToServer();
//            });

            $('#btnHome').click(function (e) {
                e.preventDefault();
                if (CStr2(sessionStorage.system) == 'eas') {
                    location.href = '/erp?OrgID=' + sessionStorage.OrgID;
                } else {
                    location.href = '/People/PeopleMenu';
                }
            });

            $('#btnDevice').click(function(){
                //$('#panDevice').popup('open');    
                BrowsePopup('LibraryDevice', 'Select Device', [
                    {
                        AutoSearch: true,
                        DisplayText: [{ Column: 'PhoneName' }],
                        Click: function (e) {
                            $('#lblDevice').html(e.PhoneName);
                            DeviceID = e.UUID;
                            DisplayTracks();
                        }
                    }
                ], { IsAll: true })
            });

            $('#btnDisplayTracks').click(function () {
                $('#googleMap').html('');
                DisplayTracks();
            });

            $('#btnPeriod').click(function () {
                PopupPeriod({
                    Click: function (e) {
                        if (e.Period == 'Custom') {
                            $('#lblPeriod').text(e.From + ' to ' + e.To);
                        } else {
                            $('#lblPeriod').text(e.Period);
                        }

                        m_DateFrom = e.From;
                        m_DateTo = e.To;
                        DisplayTracks();
                    }
                })
                //$('#panDates').popup('open');
            });

            $('#li2DaysAgo').click(function () {
                $('#panDates').popup('close');
                $('#lblPeriod').text($('#li2DaysAgo').text());
                SetPeriod('2DaysAgo', 0);
                SetSettings();
            });

            $('#liYesterday').click(function () {
                $('#panDates').popup('close');
                $('#lblPeriod').text($('#liYesterday').text());
                SetPeriod('Yesterday', 0);
                SetSettings();
            });

            $('#liToday').click(function () {
                $('#panDates').popup('close');
                $('#lblPeriod').text($('#liToday').text());
                SetPeriod('Today', 0);
                SetSettings();
            });

            $('#liWeekToDate').click(function () {
                $('#panDates').popup('close');
                $('#lblPeriod').text($('#liWeekToDate').text());
                SetPeriod('WeekToDate', 0);
                SetSettings();
            });

            $('#liYearToDate').click(function () {
                $('#panDates').popup('close');
                $('#lblPeriod').text($('#liYearToDate').text());
                SetPeriod('YearToDate', 0);
                SetSettings();
            });

            $('#liMonthToDate').click(function () {
                $('#panDates').popup('close');
                $('#lblPeriod').text($('#liMonthToDate').text());
                SetPeriod('MonthToDate', 0);
                SetSettings();
            });

            $('#liCustomPeriod').click(function () {
                $('#panDates').popup('close');
                $('#panCustomPeriod').popup('open');
            });

            $('#btnDoneCustomerPeriod').click(function () {
                $('#panCustomPeriod').popup('close');
                $('#lblPeriod').text(
                $.format.date($('#txtDateFrom').DateCombo('val'), 'MM.dd.yy') + '--' +
                $.format.date($('#txtDateTo').DateCombo('val'), 'MM.dd.yy')
                );
                SetPeriod('CustomPeriod', 1);
                SetSettings();
            });
                        
            $('#selDevice').change(function () {
                $('#lblDevice').html($('#selDevice :selected').text());
                DeviceID = $('#selDevice :selected').val();
                $('#panDevice').popup('close');
                DisplayTracks();
            });

            $('#selPeriod').change(function () {
                $('#panDates').popup('close');

                if ($('#selPeriod').val() == 'CustomPeriod') {
                    $('#txtDateFrom, #txtDateTo').DateCombo();
                    $('#panDates').popup('close');
                    $('#panCustomPeriod').popup('open');
                }
                else{
                    $('#lblPeriod').text($('#selPeriod :selected').text());
                SetPeriod($('#selPeriod').val(), 0);
                SetSettings();
                }
            });
            
        });

        function PageShow() {

            if (ValidPrevPage(arguments)) {
                GetAllDevices();
                GetAllMonitoringOrganization();
                SetPeriod('Today', 0);
                $('#lblPeriod').text('Today');
            };

            function ValidPrevPage(args) {
                try {
                    return args[1].prevPage.attr('data-role') != 'dialog';
                } catch (e) {
                    return true;
                };
            };
        }

        function SetSettings (){}

        function SetPeriod(mPeriod, mIsDateRange) {
            m_Period = mPeriod;
            m_IsDateRange = mIsDateRange;
            DisplayTracks();
        }

        function GetAllDevices() {
            var data = { FilterFowst: 0
            }

            AjaxSend({
                url: 'Browser/LibraryDevice',
                data: data,
                success: function (vData) {
                    DisplayDevices(vData.Results.Rows);
                }
            });
        }

        function GetAllMonitoringOrganization() {
            var data = { UUID: DeviceID
            }

            if(DeviceID==''){
                return;
            }
            
            AjaxSend({
                url: 'DashboardsAndReports/GetAllMonitoringOrganization',
                data: data,
                success: function (vData) {
                    DisplayOrganization(vData.Results.Rows);
                }
            });
        }

        function DisplayDevices(vRows) {
            pLV = $('#lstDevice');
            
            pLV.empty();
            
            $('#selDevice').empty();
            $('#selDevice').append(new Option('Select Device', '', false));

           for (var i = 0; i <= vRows.length - 1; i++) {
            
            $('#selDevice').append(new Option(vRows[i].PhoneName, vRows[i].UUID, false));

//            pItem = $('<li>')
//                        .append($('<a>')
//                            .attr('ID', vRows[i].ID)
//                            .data('data', vRows[i])
//                            .click(function (e) {
//                                $('#lblDevice').html('[ ' + $(this).data('data').PhoneName + ']');
//                                DeviceID = $(this).data('data').UUID;
//                                $('#panDevice').popup('close');
//                                DisplayTracks();
//                            })
//                            .append($('<h3>')
//                                .text(vRows[i].PhoneName)
//                            )
//                        );

//            pLV.append(pItem);
            }
            //try {
            //    pLV.listview('refresh');
            //    $('#selDevice').selectmenu('refresh');
            //}
            //catch (e) {
            //    pLV.listview();
            //    $('#selDevice').selectmenu('refresh');
            //}
        }

        function DisplayTracks() { 
            //m_DateFrom = new Date();
            //m_DateTo = new Date();

            if(DeviceID==''){
                return;
            }

            $('#googleMap').html('No Data Available');
            //$('#tblDetails').html('');
            
            //if (m_IsDateRange == 1) {
            //    if (!IsDate($('#txtDateFrom').DateCombo('val')) || !IsDate($('#txtDateTo').DateCombo('val'))) {
            //        MessageBox("Please put valid date",
            //                    "FOWST",
            //                    [
            //                        { Text: 'Ok'
            //                        }
            //                    ]
            //                );
            //        return;
            //    }
            //    else {
            //        m_DateFrom = $('#txtDateFrom').DateCombo('val');
            //        m_DateTo = $('#txtDateTo').DateCombo('val');
            //    }
            //}
            //else {
            //    switch (m_Period) {
            //        case "2DaysAgo":
            //            //back date from
            //            m_DateFrom.setDate(m_DateFrom.getDate() - 2);
            //            m_DateTo.setDate(m_DateTo.getDate() - 2);
            //            break; ;
            //        case "Yesterday":
            //            //back date from    
            //            m_DateFrom.setDate(m_DateFrom.getDate() - 1);
            //            m_DateTo.setDate(m_DateTo.getDate() - 1);
            //            break;                    
            //        case "WeekToDate":
            //            m_DateFrom.setDate(m_DateFrom.getDate() - m_DateFrom.getDay());
            //            m_DateTo = (m_DateTo.getMonth() + 1) + '/' + m_DateTo.getDate() + '/' + m_DateTo.getFullYear();
            //            break;
            //        case "MonthToDate":
            //            GroupDateBy = 'Month';
            //            m_DateFrom = (m_DateFrom.getMonth() + 1) + '/1/' + m_DateFrom.getFullYear();
            //            m_DateTo = (m_DateTo.getMonth() + 1) + '/' + m_DateTo.getDate() + '/' + m_DateTo.getFullYear();
            //            break;
            //    }
            //}

            GetAllMonitoringOrganization();
            
            data = {
                    UUID: DeviceID, 
                    From: FormatDate(m_DateFrom), 
                    To: FormatDate(m_DateTo)
                    };

            var datatableDevice;
            AjaxSend({
                url: 'DashboardsAndReports/GetGPSLocations',
                data: data,
                success: function (vData) {

                    if (vData.Results.Rows.length > 0) {
                        
                        var myLocationTemp = [];
                        var myLocation = [];
                        var mCenter = new google.maps.LatLng(vData.Results.Rows[0].Lat, vData.Results.Rows[0].Long);
                        var j = 0;

                        if ($.fn.DataTable.isDataTable($('#tblDetails'))) {
                            $('#tblDetails').empty();
                            $('#tblDetails').DataTable().clear().destroy();
                        }

                        var buttons = [];
                        if (!IsMobile) {
                            buttons.push({ extend: 'csvHtml5', text: 'Download CSV', title: 'Hourly Sales' })
                        }

                        datatableDevice = $('#tblDetails').DataTable({
                            pageLength: 25,
                            "ordering": false,
                            "data": vData.Results.Rows,
                            "columns": [
                                { "data": "RegisteredDay" },
                                { "data": "RegisteredTime" },
                                {
                                    "data": function (data) {
                                        return data.Lat + ' - ' + data.Long
                                    }
                                },
                                { "data": "Source" }
                            ],
                            "sDom": '<"top">rt<"bottom"ip><"clear">',
                            initComplete: function () {
                                $('#tblDetails').removeClass('dataTable');
                            }
                        });

                        datatableDevice.on('click', 'td', function () {
                            var mData = datatableDevice.row(this).data();

                            if (CInt2(mData.Lat) == 0 && CInt2(mData.Long) == 0) {
                                return;
                            }

                            var mCenter = new google.maps.LatLng(mData.Lat, mData.Long);
                            var mapProp = {
                                center: mCenter,
                                zoom: 18,
                                mapTypeId: google.maps.MapTypeId.ROADMAP
                            };

                            var map = new google.maps.Map(document.getElementById("googleMap"), mapProp);

                            var marker = new google.maps.Marker({
                                position: mCenter,
                            });

                            marker.setMap(map);

                            window.location = '#googleMap';
                        });

                        return

                        for (i = 0; i < vData.Results.Rows.length - 1; i++) {
                            if(parseInt(vData.Results.Rows[i].Lat)!=0){
                                myLocationTemp[j] = new google.maps.LatLng(vData.Results.Rows[i].Lat, vData.Results.Rows[i].Long);
                                myLocation[j] = myLocationTemp[j];
                                j++;
                            }

                                tData = $('<tr>')
                                            .append($('<td style="text-align:center">')                                            
                                                .text(vData.Results.Rows[i].RegisteredDay)
                                            )
                                            .append($('<td style="text-align:center">')
                                                .text(vData.Results.Rows[i].RegisteredTime)
                                            )    
                                            .append($('<td style="text-align:center">')    
                                                .text(vData.Results.Rows[i].Lat + ' - ' + vData.Results.Rows[i].Long)
                                            )                                        
                                            .append($('<td>')
                                                .text(vData.Results.Rows[i].Source)
                                            )
                                            .data('data', vData.Results.Rows[i])
                                            .click(function () {
                                                
                                                

                                            })

                                $('#tblDetails').append(tData);                            
                        }

                        var mapProp = {
                            center: mCenter,
                            zoom: 18,
                            mapTypeId: google.maps.MapTypeId.ROADMAP
                        };

                        var map = new google.maps.Map(document.getElementById("googleMap"), mapProp);

                        var flightPath = new google.maps.Polyline({
                            path: myLocation,
                            strokeColor: "#0000FF",
                            strokeOpacity: 0.8,
                            strokeWeight: 2
                        });

                        flightPath.setMap(map);

                            

                        //for (i = 0; i < vData.Results.Rows.length - 1; i++) {
                        //    var marker=new google.maps.Marker({
                        //          position:myLocationTemp[i],
                        //          });

                        //        marker.setMap(map);

                        //        if(i==0){
                        //            var infowindow = new google.maps.InfoWindow({
                        //                    content: 'Start'                                        
                        //                });
                        //                infowindow.open(map,marker);
                        //        }
                        //        else if(i==vData.Results.Rows.length-2){
                        //            var infowindow = new google.maps.InfoWindow({
                        //                    content: 'End'                                        
                        //                });
                        //                infowindow.open(map,marker);
                        //        }
                        //}
                    }
                     
                    
                }
            }); //end ajaxsend
        }
        
        function DisplayOrganization(vData) {
            var tData;

            $('#tblOrganizationDetails').empty();
            
            for (i = 0; i <= vData.length - 1; i++) {
                    tData = $('<tr>')
                        .append($('<td>')
                            .text(vData[i].CompanyName)
                        )
                        .data('data', vData[i])
                        
                        $('#tblOrganizationDetails').append(tData);       
            }
            
        }
    });