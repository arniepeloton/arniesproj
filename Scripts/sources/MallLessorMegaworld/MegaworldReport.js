require([
    'common',
    //GetPathFormat('/Scripts3/Browser/LibraryFowst.js?crc=')
    (OfflineMode ? 'promise!/Scripts/sources/MallLessorMegaworld/RobinsonsReport_offline.js' : null)
], function () {

        $(document).ready(function () {

            var arrSelectedSalesDate = [], ctrSend = 0, arrUnsuccessful = [];
            var pData = { FilterFowst: 1, Mall: 'Robinson' };
            if (OfflineMode) {
                $('#divPOSMachine').hide();
                if (Web.HandleOrganization() == false)
                    return;
                var vData = OfflineBrowser.LibraryFowst.GetData(pData).Rows;
                DisplayFowst(vData);
            } else {
                $('#divPOSMachine').show();
                //GetFowsts();
            }            

            (function PopMallDropDown() {
                $('#selMegaWorldMall')
                    .append(new Option('Select Mall'))
                    .append(new Option('Megaworld', 'Megaworld'))
                    .append(new Option('Robinson', 'Robinson'))
                    .val('')
                    .change(GetFowsts);
            })();

            $('#selMegaworldFowst').change(function (e) {
                e.preventDefault();
                if (OfflineMode) {
                    GetFowstDeviceReports();
                } else {
                    GetFOWSTDevices($(this).val());
                }                
            });

            $('#selMegaworldDevices, #selMegaworldMonth, #selMegaworldYear').change(function (e) {
                e.preventDefault();
                GetFowstDeviceReports();
            });

            $('#btnTextfilesRefresh').click(function () {
                GetFowstDeviceReports();
            })

            function GetFowsts() {
                var pData = { FilterFowst: 1, Mall: $('#selMegaWorldMall').val() };
                Bs.Loading('show');
                AjaxSend({
                    url: 'Browser/LibraryFowst',
                    data: pData,
                    success: function (vFowstData) {
                        var vRows = vFowstData.Results.Rows;
                        DisplayFowst(vRows);
                    }
                });
            }

            function DisplayFowst(vRows) {
                var selMegaworldFowst = $('#selMegaworldFowst').empty();
                selMegaworldFowst.append(new Option('Please select', ''));
                for (var i in vRows) {
                    selMegaworldFowst.append(new Option(vRows[i].FowstName, vRows[i].ID));
                }

                if (vRows.length > 0) GetFOWSTDevices(vRows[i].ID);

                Bs.Loading('hide');

                for (var i = 1; i <= 12; i++) {
                    $('#selMegaworldMonth').append(new Option($.format.date(new Date(i + '-01-2023'), 'MMM'), $.format.number(i, '00')));
                }
                $('#selMegaworldMonth').val($.format.number(new Date().getMonth() + 1, '00'));

                var year = new Date().getFullYear()
                for (var i = year; i >= 2020; i--) {
                    $('#selMegaworldYear').append(new Option(i, i));
                }
            }

            function AjaxSend_MallLessorMegaworld_GetFowstDeviceReports(vParams) {
                if (IsMobile) {
                    var pSql = 'SELECT * \n' +
                        ',SUBSTR(filename, 2) AS Filename2' + '\n' + 
                        'FROM TenantMonitoringLogs\n' +
                        'WHERE FOWSTID = ' + ReadyString($('#selMegaworldFowst').val()) + '\n' +
                        ' AND DeviceID = ' + ReadyString(Web.DeviceID()) + '\n' +
                        ' AND strftime(\'%m\', SalesDate) = ' + ReadyString($('#selMegaworldMonth').val()) + '\n' +
                        ' AND strftime(\'%Y\', SalesDate) = ' + ReadyString($('#selMegaworldYear').val()) + '\n' +
                        ' ORDER BY SalesDate DESC';
                    var pDT = Sql.Execute(pSql);
                    vParams.success({
                        Results: pDT
                    });
                } else
                    AjaxSend($.extend(vParams, {
                        url: MergeUrlParameters({
                            OrgID: sessionStorage.OrgID,
                            FowstID: $('#selMegaworldFowst').val(),
                            DeviceID: (OfflineMode ? Web.DeviceID() : $('#selMegaworldDevices').val()),
                            Month: $('#selMegaworldMonth').val(),
                            Year: $('#selMegaworldYear').val(),
                        }, '/MallLessorMegaworld/GetFowstDeviceReports')
                    }));
            } // AjaxSend_MallLessorMegaworld_GetFowstDeviceReports
            
            function GetFowstDeviceReports() {
                var tblMegaworldReport = $('#tblMegaworldReport');
                tblMegaworldReport.find('tbody').empty();

                if ($.fn.DataTable.isDataTable("#tblMegaworldReport")) {
                    //check if datatable is created 
                    tblMegaworldReport.DataTable().clear().destroy();
                }

                if ($('#selMegaworldFowst').val() == '') return;
                Bs.Loading('show');

                sessionStorage.MallReportFowstID = $('#selMegaworldFowst').val();
                sessionStorage.MallReportDeviceID = OfflineMode ? Web.DeviceID() : $('#selMegaworldDevices').val();
                AjaxSend_MallLessorMegaworld_GetFowstDeviceReports({
                    url: MergeUrlParameters({
                        OrgID: sessionStorage.OrgID,
                        FowstID: sessionStorage.MallReportFowstID,
                        DeviceID: sessionStorage.MallReportDeviceID,
                        Month: $('#selMegaworldMonth').val(),
                        Year: $('#selMegaworldYear').val(),
                    }, '/MallLessorMegaworld/GetFowstDeviceReports'),
                    success: function (vData) {
                        Bs.Loading('hide');
                        var ctr = 0;

                        tblMegaworldReport.find('thead input[type="checkbox"]').prop('checked', 0);

                        tblMegaworldReport.DataTable({
                            "pageLength": 50,
                            data: vData.Results.Rows,
                            "autoWidth": false,
                            "columns": [
                                {
                                    "data": function (data, type, full, meta) {
                                        return '<input type="checkbox" data-role="none" class="form-check-input" />';
                                    }, className: 'text-center'
                                },
                                {
                                    "data": function (data, type, full, meta) {
                                        ctr += 1;
                                        return ctr + '.';
                                    }
                                },
                                {
                                    "data": function (data, type, full, meta) {
                                        return $.format.date(CJsonDate(data.SalesDate), 'MM-dd-yyyy');
                                    }
                                },
                                {
                                    "data": function (data, type, full, meta) {
                                        return data.ReportDate == null ? '' : $.format.date(CJsonDate(data.ReportDate), 'MM-dd-yyyy hh:mm a');
                                    }, className: 'text-center'
                                },
                                {
                                    "data": function (data, type, full, meta) {
                                        if (window.IsMobile 
                                            && ($('#selMegaWorldMall').val() == 'Robinson')) {

                                            if (
                                                    (CStr2(data.FileStatus) == 'ftp-send')
                                                    && (CStr2(data.ErrorMessage) == '')
                                                )
                                                return 'Successful <a href="#">View</a>';
                                            else 
                                                return 'Unsuccessful <a href="#">View</a>';
                                        }
                                        return CStr2(data.ErrorMessage) == '' ? 'Successful' : 'Unsuccessful';
                                    }, className: 'text-center'
                                },
                                //{
                                //    "data": null,
                                //    createdCell: function (td, cellData, rowData, row, col) {
                                //        var btn = $('<button data-role="none" class="btn btn-success">')
                                //            .text('Resend')
                                //            .data('data', rowData)
                                //            .click(function (e) {
                                //                e.stopPropagation();
                                //                e.preventDefault();
                                //                var data = $(this).data('data');

                                //                Bs.Loading('show');
                                //                Resend(data);
                                //            })
                                //        $(td).html(btn)
                                //    }
                                //}
                            ],
                            rowCallback: function (row, data) {
                                $(row).data('data', data);
                                $(row).find('input[type="checkbox"]').data('data', data);

                                // this is the View link for robinson
                                $(row).find('a').click(function() {
                                    RowClicked(data);
                                });
                            },
                            "initComplete": function () {
                                tblMegaworldReport.removeClass('dataTable');

                                var chbSalesDate = tblMegaworldReport.find('tbody input[type="checkbox"]');

                                tblMegaworldReport.find('thead input[type="checkbox"]').unbind('change').change(function () {
                                    if ($(this).is(':checked')) {
                                        chbSalesDate.prop('checked', 1);
                                    } else {
                                        chbSalesDate.prop('checked', 0);
                                    }
                                })

                                $('#btnTextfilesResend').unbind('click').click(function () {
                                    arrSelectedSalesDate = [];
                                    arrUnsuccessful = [];
                                    ctrSend = 0;

                                    chbSalesDate.each(function () {
                                        if ($(this).is(':checked')) {
                                            arrSelectedSalesDate.push($(this).data('data'));
                                        }
                                    })

                                    if (arrSelectedSalesDate.length > 0) {
                                        Resend(arrSelectedSalesDate[ctrSend]);
                                    } else {
                                        NewMessageBox('Please select Sales Date first before to resend the textfiles.');
                                    }
                                })
                            },
                            "aoColumnDefs": [{ "bSortable": false, "aTargets": ['_all'] }],
                            "responsive": true,
                            "columnDefs": [{ "targets": 0, "orderable": false }],
                            "sDom": '<"table-fixed-container"<"table-fixed"t>>'
                            //"stateSave": true
                        })
                    }
                })
            }

            function Resend(data) {
                Bs.Loading('show', (ctrSend + 1) + ' of ' + arrSelectedSalesDate.length + ' <br> Resending textfiles for ' + $.format.date(CJsonDate(data.SalesDate), 'MMM dd') + ' <br><br> Please do not reload the page.');
                if (OfflineMode) {
                    var pIDs = [];
                    for(var p = 0; p <= arrSelectedSalesDate.length - 1; p++) {
                        pIDs.push(arrSelectedSalesDate[p].ID);
                    }
                    Reports.Robinsons.SetUnsent(pIDs);
                    Reports.Robinsons.SendUnsentLogs({
                        ResetMessages: true,
                        Callback: function() {
                            GetFowstDeviceReports();
                            // dont message here kay ma doble
                            //NewMessageBox('Sales file successfully sent to RLC server.', 'Success', null, {
                            //    Close: function() {
                            //    }
                            //});
                        }
                    });
                    Bs.Loading('hide');
                } else
                    AjaxSend({
                        url: 'MallLessorMegaworld/Resend',
                        data: {
                            FowstID: $('#selMegaworldFowst').val(),
                            DeviceID: data.DeviceID,
                            SalesDate: $.format.date(CJsonDate(data.SalesDate), 'MM-dd-yyyy')
                        }, success: function (e) {
                            if (e.Results == 'ok') {
                                if (arrSelectedSalesDate.length == (ctrSend + 1)) {
                                    Bs.Loading('hide');
                                    GetFowstDeviceReports();
                                    WebToast($('#selMegaWorldMall option:selected').val() + '\'s textfiles has been regenerated successfully');
                                    if (CStr2(e.Filename) != '') {
                                        var pUrl = MergeUrlParameters({
                                            OrgID: sessionStorage.OrgID,
                                            FowstID: $('#selMegaworldFowst').val(),
                                            DeviceID: data.DeviceID,
                                            Filename: e.Filename
                                        }, 'MallLessorMegaWorld/Download');
                                        window.open(pUrl, '_blank');
                                    }
                                } else {
                                    ctrSend += 1;
                                    Resend(arrSelectedSalesDate[ctrSend]);                                
                                }              
                            } else {
                                arrUnsuccessful.push(data);                            

                                if (arrSelectedSalesDate.length == (ctrSend + 1)) {
                                    Bs.Loading('hide');
                                    if (arrUnsuccessful.length > 0) {
                                        var txtSalesDate = '';
                                        for (var i in arrUnsuccessful) {
                                            txtSalesDate += $.format.date(CJsonDate(arrUnsuccessful[i].SalesDate), 'MMM dd') + ' <br>'
                                        }
                                        NewMessageBox('There are files that have not been sent. Do you want to resend them? <br><br>' + txtSalesDate, 'Warning', [
                                            {
                                                Text: 'Yes', Click: function () {
                                                    arrSelectedSalesDate = JSON.parse(JSON.stringify(arrUnsuccessful));
                                                    arrUnsuccessful = [];
                                                    ctrSend = 0;

                                                    Resend(arrSelectedSalesDate[ctrSend]);
                                                }
                                            }, { Text: 'No, maybe next time' }
                                        ])
                                    }
                                } else {       
                                    ctrSend += 1;
                                    Resend(arrSelectedSalesDate[ctrSend]);
                                }
                            }
                        }
                    })
            }

            function GetFOWSTDevices(fowstid) {
                if (CBool(sessionStorage.PrintingOfflineData))
                    return;
                Bs.Loading('show');
                AjaxSend({
                    url: 'Browser/LibraryDeviceFowst',
                    data: { FOWSTID: fowstid },
                    success: function (vFowstData) {
                        var vRows = vFowstData.Results.Rows;
                        var selMegaworldDevices = $('#selMegaworldDevices').empty();
                        //selMegaworldDevices.append(new Option('Please select', ''));
                        for (var i in vRows) {
                            selMegaworldDevices.append(new Option(vRows[i].PhoneName, vRows[i].DeviceID));
                        }
                        Bs.Loading('hide');
                        GetFowstDeviceReports();
                    }
                });
            }
        
            function RowClicked(vData) {
                
                var pFilename = 'RLC Files/' + vData.Filename2;
                var pFileText = Transfers.ReadFile(pFilename);
                if (CStr2(pFileText) == '') {
                    Reports.Robinsons.CreateRlcFile({
                        FowstID: vData.FOWSTID,
                        DeviceID: vData.DeviceID,
                        SalesDate: vData.SalesDate,
                        DontUpload: true,
                        Callback: function() {
                            RowClicked(vData);
                        }
                    });
                    return;

                    // dont delete commented code 2024_09_11__10_03_12_3559
                    //var pUrl = MergeUrlParameters({
                    //    OrgID: sessionStorage.OrgID,
                    //    FowstID: $('#selMegaworldFowst').val(),
                    //    DeviceID: vData.DeviceID,
                    //    Filename: vData.Filename
                    //}, '/MallLessorMegaWorld/Download');
                    //Transfers.Download({
                    //    Source: pUrl,
                    //    Destination: pFilename,
                    //    Success: function(vDownloadData) {
                    //        RowClicked(vData);
                    //    },
                    //    Error: function(vErrorData) {
                    //        NewMessageBox('This file cannot be viewed at this time. Try again later.');
                    //        Android.AddLog('jsSGHY8KPRA3! An error occured while viewing file: ' + vErrorData);
                    //    }
                    //});
                    //return;
                }
                
                pFileText = pFileText.replace(/(?:\r\n|\r|\n)/g, '<br>'); // https://stackoverflow.com/a/784547/334045
                pFileText = '<pre>' + pFileText + '</pre>';
                NewMessageBox(pFileText, vData.Filename2);
                //AjaxSend({
                //    url: pUrl,
                //    type: 'GET',
                //    success: function(vFileData) {
                //        console.log(vFileData);
                //    }
                //});
                //console.log(vData);
            }

            (function AutoSelectMallFowstDevice() {
                if (!OfflineMode)
                    return;

                var pOrgID = Transfers.ReadFile('LastOrgID.txt');
                if (!IsGuid(pOrgID))
                    return;
                var pFowstID = Transfers.ReadFile('LastFowstID.txt');
                if (!IsGuid(pFowstID))
                    return;

                Web.SetOrgID(pOrgID);
                if (!Web.HandleOrganization()) {
                    return;
                }

                require(['promise!/Scripts3/Browser/LibraryFowst.js?crc=',
                    '/Scripts3/SalesMenu_offline.js?crc='], function() {
                    var pDtFowsts = OfflineBrowser.LibraryFowst.GetData({
                        ID: pFowstID,
                        IsThirdPartyOrganization: true,
                        ThirdPartyOrganizationID: pOrgID
                    });
                    if (pDtFowsts.Rows.length == 0)
                        return;
                    var pDrFowst = pDtFowsts.Rows[0];
                    if (pDrFowst.FOWSTMallName != 'Robinson')
                        return;
                    $('#selMegaWorldMall').val(pDrFowst.FOWSTMallName);
                    $('#selMegaworldFowst').val(pFowstID);
                    GetFowstDeviceReports();
                    Reports.Robinsons.SendUnsentLogs({
                        Callback: function() {
                            GetFowstDeviceReports();
                            // dont message here kay ma doble
                            //NewMessageBox('Sales file successfully sent to RLC server.', 'Success', null, {
                            //    Close: function() {
                            //    }
                            //});
                        }
                    });
                });
            })(); // END AutoSelectMallFowstDevice
        });
    });