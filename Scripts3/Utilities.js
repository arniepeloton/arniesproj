require(['common'
    ], function () {

    $(function () {

        var mCbOrgs = $('#cbOrgs, #cbDLZreadOrgs, #cbForceFranchiseeDatabase, #cbForceFranchisorDatabase');
        var mTxtDatabase = $('#txtDatabase');
        var mDivSynonymLogs = $('#divSynonymLogs');
        var mZReadCacheID = NewID();

        $('#butGetOrganizations, #butDLZReadGetOrganizations').click(function(e) {
            e.preventDefault();
            var pPassword = $('#txtSrvPassword').val();
            if (pPassword.trim() == '') {
                MessageBox('Please enter secret password.');
                return;
            }

            AjaxSend({
                url: '/Utilities/GetOrganizations',
                data: {
                    Password: pPassword
                },
                success: function(vData) {
                    if (vData.Error)
                        MessageBox(vData.Error);
                    else
                        DisplayOrganizations(vData);
                }
            }); // END AjaxSend({
        }); // END  $('#butGetOrganizations').click(function() {

        function DisplayOrganizations(vData) {
            var pItem;
            var pDR;

            mCbOrgs.html('');
            pItem = new Option('none (use default values)', '');
            $('.AllowBlank').append(pItem);
            for (var p = 0; p <= vData.Rows.length - 1; p++) {
                pDR = vData.Rows[p];
                pItem = new Option(pDR.CompanyName, pDR.ID);
                mCbOrgs.append(pItem);
            }

            mCbOrgs.selectmenu();
            mCbOrgs.selectmenu('refresh', true);
        }

        $('#butGetSrv').click(function(e) {
            e.preventDefault();
            GetSrv();
        });

        function GetSrv() {
            var pPassword = $('#txtSrvPassword').val();
            if (pPassword.trim() == '') {
                MessageBox('Please enter secret password.');
                return;
            }
            if (!IsNumeric($('#txtSrvAmount').val())) {
                $('#txtSrvAmount').focus();
                MessageBox('SRV amount must be a valid amount');
            }

            var pOrgID;
            pOrgID = mCbOrgs.val();

            AjaxSend({
                url: '/Utilities/GetByAmount',
                data: {
                    Password: pPassword,
                    OrgID: pOrgID,
                    Amount: $('#txtSrvAmount').val()
                },
                success: function(vData) {
                    if (vData.Error) {
                        MessageBox(vData.Error);
                        return;
                    }
                    $('#divSrvResult').text(JSON.stringify(vData.data.Rows, null, 4));
                    
                }
            });
        }

        $('#butDownloadExcel').click(function(e) {
            var pPassword = $('#txtSrvPassword').val();
            if (pPassword.trim() == '') {
                MessageBox('Please enter secret password.');
                return;
            }
            var pOrgID;
            pOrgID = mCbOrgs.val();

            $('#divLastInvoiceResult').text('Please wait');
            AjaxSend({
                url: '/Utilities/LastInvoicePerUser',
                data: {
                    Password: pPassword,
                    OrgID: pOrgID,
                    Month: $('#txtMonth').val(),
                    Year: $('#txtYear').val()
                },
                success: function(vData) {
                    if (vData.Error)
                        $('#divLastInvoiceResult').text(vData.Error);
                    else if (vData.Link)
                        $('#divLastInvoiceResult')
                            .html('')
                            .append($('<a>')
                                .attr('href', '/Download/' + vData.Link)
                                .text('Download ' + vData.Link)
                            );
                    else 
                        $('#divLastInvoiceResult').text('No result');
                }
            });
        }); // END $('#butDownloadExcel').click(function(e) {

        $('#butGetInvoices').click(function(e) {
            var pPassword = $('#txtSrvPassword').val();
            if (pPassword.trim() == '') {
                MessageBox('Please enter secret password.');
                return;
            }

            AjaxSend({
                url: '/Utilities/GetInvoices',
                data: {
                    Password: pPassword,
                    InvoiceNum: $('#txtInvoiceNum').val()
                },
                success: function(vData) {
                    if (vData.Error) {
                        $('#divInvoices').text(vData.Error);
                        return;
                    }
                    $('#divInvoices').text(JSON.stringify(vData.data.Rows, null, 4));
                }
            });
        }); // END $('#butGetInvoices').click(function(e) {

        $('#butChangeInvoiceDate').click(function(e) {
            var pPassword = $('#txtSrvPassword').val();
            if (pPassword.trim() == '') {
                MessageBox('Please enter secret password.');
                return;
            }

            AjaxSend({
                url: '/Utilities/ChangeInvoiceDate',
                data: {
                    Password: pPassword,
                    InvoiceID: $('#txtInvoiceID').val(),
                    NewInvoiceDate: $('#txtNewInvoiceDate').val()
                },
                success: function(vData) {
                    if (vData.Error) {
                        MessageBox(vData.Error);
                        return;
                    }
                    if (vData.RowCount)
                        MessageBox('RowCount: ' + vData.RowCount);
                    else
                        MessageBox('Unexpected result');
                }
            });
        }); // END $('#butChangeInvoiceDate').click(function(e) {

        $('#butUntagReceivingAsPayable').click(function(e) {
            var pPassword = $('#txtSrvPassword').val();
            if (pPassword.trim() == '') {
                MessageBox('Please enter secret password.');
                return;
            }
            var pReceivingNum = $('#txtReceivingNum').val();
            AjaxSend({
                url: '/Utilities/UntagReceivingAsPayable',
                data: {
                    ReceivingNum: pReceivingNum,
                    Password: pPassword
                },
                success: function(vData) {
                    if (vData.Error) {
                        MessageBox(vData.Error);
                        return;
                    }
                    if (vData.Result == 'ok')
                        MessageBox('ok');
                    else
                        MessageBox(vData.Result);
                }
            });
        });

        $('#butSetJit').click(function() {

            var pPassword = $('#txtSrvPassword').val();
            if (pPassword.trim() == '') {
                MessageBox('Please enter secret password.');
                return;
            }

            var pOrgID;
            pOrgID = mCbOrgs.val();

            AjaxSend({
                url: '/Utilities/SetJit',
                data: {
                    Password: pPassword,
                    OrgID: pOrgID
                },
                success: function(vData) {
                    if (vData.Error)
                        MessageBox(vData.Error);
                    else if (vData.Result)
                        MessageBox('No. of affected records: ' + vData.Result);
                    else 
                        MessageBox('No result');
                }
            });
        });

        //========ZRead===============

        var mCbFOWST = $('#cbFOWST');

        $('#butGetFOWST').click(function (e) {
            e.preventDefault();
            
            var pOrgID;
            pOrgID = mCbOrgs.val();

            AjaxSend({
                url: '/Browser/LibraryFowst',
                data: {
                    OrgID: pOrgID
                },
                success: function (vData) {
                    if (vData.Error)
                        MessageBox(vData.Error);
                    else
                        DisplayFOWST(vData.Results);
                }
            }); // END AjaxSend({
        }); // END  $('#butGetOrganizations').click(function() {

        function DisplayFOWST(vData) {
            var pItem;
            var pDR;

            mCbFOWST.html('');
            for (var p = 0; p <= vData.Rows.length - 1; p++) {
                pDR = vData.Rows[p];
                pItem = new Option(pDR.FowstName, pDR.ID);
                mCbFOWST.append(pItem);
            }

            mCbFOWST.selectmenu();
            mCbFOWST.selectmenu('refresh', true);
        }

        var mCbDevices = $('#cbDevices');

        $('#butGetDevices').click(function (e) {
            e.preventDefault();
            
            var pOrgID;
            pOrgID = mCbOrgs.val();

            var pFOWSTID;
            pFOWSTID = mCbFOWST.val();


            AjaxSend({
                url: '/Browser/LibraryDeviceFowst',
                data: {
                    OrgID: pOrgID,
                    FOWSTID: pFOWSTID
                },
                success: function (vData) {
                    if (vData.Error)
                        MessageBox(vData.Error);
                    else
                        DisplayDevices(vData.Results);
                }
            }); // END AjaxSend({
        }); // END  $('#butGetOrganizations').click(function() {

        function DisplayDevices(vData) {
            var pItem;
            var pDR;

            mCbDevices.html('');
            for (var p = 0; p <= vData.Rows.length - 1; p++) {
                pDR = vData.Rows[p];
                pItem = new Option(pDR.PhoneName + '-' + pDR.POSRegistrationNo, pDR.DeviceID);
                mCbDevices.append(pItem);
            }

            mCbDevices.selectmenu();
            mCbDevices.selectmenu('refresh', true);
        }

        $('#butDLZreadGetFOWST').click(function (e) {
            e.preventDefault();

            var pOrgID;
            pOrgID = $('#cbDLZreadOrgs option:selected').val();
            $('#cbDLZreadFOWST').empty();
            AjaxSend({
                url: '/Browser/LibraryFOWST',
                data: {
                    OrgID: pOrgID,
                    IsAll: true,
                    IncludeInactiveStore: true,
                },
                success: function (vData) {
                    if (vData.Error)
                        MessageBox(vData.Error);
                    else {
                        for (var p = 0; p <= vData.Results.Rows.length - 1; p++) {
                            pDR = vData.Results.Rows[p];
                            if (CStr2(pDR.FowstName) == '') continue;
                            pItem = new Option(pDR.FowstName + (CStr2(pDR.Code) == '' ? '' : ' - ' + CStr2(pDR.Code)), pDR.ID);
                            $('#cbDLZreadFOWST').append(pItem);
                        }

                        $('#cbDLZreadFOWST').selectmenu('refresh', true);
                    }
                        
                }
            }); // END AjaxSend({
        });

        $('#cbDLZreadFOWST').change(function () {
            if ($('#cbDLZreadFOWST option:selected').val() == '') MessageBox('Please select FOWST first');

            var pOrgID;
            pOrgID = $('#cbDLZreadOrgs option:selected').val();
            $('#cbDLZreadDevices').empty();
            AjaxSend({
                url: '/Browser/LibraryDeviceFowst',
                data: {
                    OrgID: pOrgID,
                    FOWSTID: $('#cbDLZreadFOWST option:selected').val(),
                    IsAll: true
                },
                success: function (vData) {
                    if (vData.Error)
                        MessageBox(vData.Error);
                    else {
                        for (var p = 0; p <= vData.Results.Rows.length - 1; p++) {
                            pDR = vData.Results.Rows[p];
                            pItem = new Option(pDR.PhoneName + ' - ' + pDR.UUID + ' - ' + pDR.DeviceID + ' (' + pDR.Status + ')', pDR.DeviceID);
                            $('#cbDLZreadDevices').append(pItem);
                        }

                        $('#cbDLZreadDevices').selectmenu('refresh', true).change();
                    }

                }
            });
        })

        $('#butZReadGenerateNewCacheID').click(function () {
            mZReadCacheID = NewID();
            $('#butZReadGenerateNewCacheID').text('generating...').button('disable').button('refresh');
            setTimeout(function() {
                $('#butZReadGenerateNewCacheID').text('Generate New Cache ID').button('enable').button('refresh');
            }, 1000);
        });

        $('#butDLZreadGetRecomputeZRead, #butDLCachedZread').click(function () {
            var pParams = {
                OrgID: $('#cbDLZreadOrgs option:selected').val(),
                FOWSTID: $('#cbDLZreadFOWST option:selected').val(),
                DeviceID: $('#cbDLZreadDevices option:selected').val(),
                From: $('#txtDLZreadFrom').val(),
                To: $('#txtDLZreadTo').val(),
                GenerateZeroSales: $('#chbZReadZeroSales').is(':checked'),
                ForceNONVAT: !$('#chbZReadForceNONVAT').is(':checked'),
                DownloadDataOnly: false,
                ExportType: $('[name="radZRead"]:checked').val(),
                ExcludePTUDetails: $('[name="radZReadPTU"]').is(':checked'),
                GetFromRawData: $(this).attr('id') == 'butDLZreadGetRecomputeZRead'
            };
            if ($('#chkZReadUseCache').is(':checked'))
                pParams.cacheid = mZReadCacheID;
            else
                mZReadCacheID = NewID();
            var pUrl = MergeUrlParameters(pParams, '/Utilities/DownloadZRead');
            window.open(pUrl, '_blank');
        })

        $('.butRecacheXZData').click(function () {
            var dis = $(this);
            var ctr = 1, cashiertransDate = [];
            var pParams = {
                OrgID: $('#cbDLZreadOrgs option:selected').val(),
                FOWSTID: $('#cbDLZreadFOWST option:selected').val(),
                FowstID: $('#cbDLZreadFOWST option:selected').val(),
                DeviceID: $('#cbDLZreadDevices option:selected').val(),
                UserID: $('#selXReadCashier option:selected').val(),
                RangeType: 'Day',
                RangeFrom: $.format.date(new Date($('#txtDLZreadFrom').val()), 'MM-dd-yyyy'),
                RangeTo: $.format.date(new Date($('#txtDLZreadTo').val()), 'MM-dd-yyyy'),
                RangeYear: 'Day',
                GenerateZeroSales: false,
                DownloadDataOnly: true,
                timeout: 600000, //10 minutes
                ForceNONVAT: true,
                GetFromRawData: true,
                ReportType: $(this).attr('report')
            };

            AjaxSend({
                url: 'Utilities/GenerateCacheDatabase',
                data: pParams,
                success: function (e) {
                    function ErrorDuringOfCachingData(datefrom) {
                        dis.parent().next().html('<b style="color:red">Error retrieving sales data for ' + $.format.date(new Date(datefrom), 'MM-dd-yyyy') + '.</b><br>');
                        dis.parent().next().append($('<a data-role="none" href="#">')
                            .text('Click here to retry.')
                            .click(function (e) {
                                e.preventDefault();
                                if (pParams.ReportType == 'XRead') {
                                    PopulateCacheDatabase(cashiertransDate[CInt2(ctr) - 1].Date, cashiertransDate[CInt2(ctr) - 1].Date)
                                } else {
                                    datefrom = new Date(datefrom);
                                    PopulateCacheDatabase(datefrom, datefrom)
                                }
                            })
                        )
                    }

                    function PopulateCacheDatabase(datefrom, dateto) {
                        var data = $.extend(JSON.parse(JSON.stringify(pParams)), {
                            RangeFrom: $.format.date(new Date(datefrom), 'MM-dd-yyyy') + ' 12:00:00 AM',
                            RangeTo: $.format.date(new Date(dateto), 'MM-dd-yyyy') + ' 11:59:59 PM'
                        })

                        dis.parent().next().html('<b style="color:orange;">DO NOT RELOAD THE PAGE!</b><br>Retrieving sales data for: ' + $.format.date(new Date(datefrom), 'MM-dd-yyyy') + ' <br>' + ctr + ' of ' + (pParams.ReportType == 'XRead' ? cashiertransDate.length : ((new Date(pParams.RangeTo) - new Date(pParams.RangeFrom)) / (1000 * 60 * 60 * 24) + 1)));

                        AjaxSend({
                            url: MergeUrlParameters(data, 'Utilities/PopulateCacheDatabase'),
                            data: data,
                            success: function (e) {
                                if (CStr2(e.Error) != '') {
                                    console.log(e.Error);
                                    ErrorDuringOfCachingData(datefrom);
                                    return;
                                }

                                if (e.Result == 'ok') {
                                    ctr += 1;

                                    if ($.format.date(new Date(datefrom), 'MM-dd-yyyy') == pParams.RangeTo || (CInt2(ctr) - 1) == cashiertransDate.length) {
                                        dis.parent().next().html('<b style="color:green">' + pParams.ReportType + ' data cached successfully.</b> <br>')
                                        dis.parent().next().append($('<a data-role="none" href="#">')
                                            .text('Click here to download cached database.')
                                            .click(function (e) {
                                                e.preventDefault();
                                                var pParams = {
                                                    OrgID: $('#cbDLZreadOrgs option:selected').val(),
                                                    FOWSTID: $('#cbDLZreadFOWST option:selected').val(),
                                                    DeviceID: $('#cbDLZreadDevices option:selected').val()
                                                };
                                                if ($('#chkZReadUseCache').is(':checked'))
                                                    pParams.cacheid = mZReadCacheID;
                                                else
                                                    mZReadCacheID = NewID();
                                                var pUrl = MergeUrlParameters(pParams, '/Utilities/DownloadCacheDatabase');
                                                window.open(pUrl, '_blank');
                                            })
                                        )
                                    } else {
                                        if (pParams.ReportType == 'XRead') {
                                            PopulateCacheDatabase(cashiertransDate[CInt2(ctr) - 1].Date, cashiertransDate[CInt2(ctr) - 1].Date)
                                        } else {
                                            datefrom = new Date(datefrom);
                                            datefrom = new Date(datefrom.setDate(new Date(datefrom.getDate() + 1)));
                                            PopulateCacheDatabase(datefrom, datefrom)
                                        }
                                    }
                                }
                            }, error: function () {
                                ErrorDuringOfCachingData(datefrom);
                            }
                        })
                    }

                    if (pParams.ReportType == 'XRead') {
                        AjaxSend({
                            url: 'Utilities/GetCashierTransDate',
                            data: pParams,
                            success: function (e) {
                                cashiertransDate = e.Result.Rows;
                                if (cashiertransDate.length > 0)
                                    PopulateCacheDatabase(cashiertransDate[0].Date, cashiertransDate[0].Date)
                            }
                        })
                    } else {
                        PopulateCacheDatabase(pParams.RangeFrom, pParams.RangeFrom)
                    }                    
                }
            })
        })

        $('#butDLXread').click(function () {
            var pParams = {
                OrgID: $('#cbDLZreadOrgs option:selected').val(),
                FOWSTID: $('#cbDLZreadFOWST option:selected').val(),
                DeviceID: $('#cbDLZreadDevices option:selected').val(),
                UserID: $('#selXReadCashier option:selected').val(),
                RangeType: 'Day',
                RangeFrom: $.format.date(new Date($('#txtDLZreadFrom').val()), 'MM-dd-yyyy') + ' 12:00:00 AM',
                RangeTo: $.format.date(new Date($('#txtDLZreadTo').val()), 'MM-dd-yyyy') + ' 11:59:59 PM',
                RangeYear: ''
            };
            var pUrl = MergeUrlParameters(pParams, '/Utilities/DownloadXRead');
            window.open(pUrl, '_blank');
        })

        $('#butDLNewEJournal').click(function () {
            var pParams = {
                OrgID: $('#cbDLZreadOrgs option:selected').val(),
                FOWSTID: $('#cbDLZreadFOWST option:selected').val(),
                FowstID: $('#cbDLZreadFOWST option:selected').val(), //for CSR
                DeviceID: $('#cbDLZreadDevices option:selected').val(),
                From: $('#txtDLZreadFrom').val(),
                To: $('#txtDLZreadTo').val(),
                GenerateZeroSales: $('#chbZReadZeroSales').is(':checked'),
                ForceNONVAT: !$('#chbZReadForceNONVAT').is(':checked'),
                DownloadDataOnly: true,
                GetFromRawData: true,
                ExcludeInvoice: $('#chbchbEjournalInvoice').is(':checked'),
                ExcludeZRead: $('#chbchbEjournalZRead').is(':checked'),
                ExcludeXRead: $('#chbchbEjournalXRead').is(':checked'),
                ExcludeReturns: $('#chbchbEjournalReturns').is(':checked')
            }

            AjaxSend({
                url: 'ExternalServices/DownloadNewEJournal',
                data: pParams,
                timeout: 600000,
                success: function (vData) {
                    if (vData != null) {
                        window.location = GetPathFormat("/ReportsPOS/DownloadReportFile?OrgID=" + $('#cbDLZreadOrgs option:selected').val() + '&ReportLink=' + vData.ReportPOS);
                    }
                }
            })
        })

        $('#butDLEJournal').click(function () {
            AjaxSend({
                url: 'ExternalServices/DownloadEJournal',
                data: {
                    OrgID: $('#cbDLZreadOrgs option:selected').val(),
                    LocationID: $('#cbDLZreadFOWST option:selected').val(),
                    DeviceID: $('#cbDLZreadDevices option:selected').val(),
                    DateFrom: $('#txtDLZreadFrom').val(),
                    DateTo: $('#txtDLZreadTo').val()
                },
                timeout: 600000,
                success: function (vData) {
                    if (vData != null) {
                        window.location = GetPathFormat("/ReportsPOS/DownloadReportFile?OrgID=" + $('#cbDLZreadOrgs option:selected').val() + '&ReportLink=' + vData.ReportPOS);
                    }
                }
            })
        })

        $('#butDLFixedEJournal').click(function () {
            AjaxSend({
                url: 'ExternalServices/DownloadAllEJournalInSelectedDate',
                data: {
                    OrgID: $('#cbDLZreadOrgs option:selected').val(),
                    LocationID: $('#cbDLZreadFOWST option:selected').val(),
                    DeviceID: $('#cbDLZreadDevices option:selected').val(),
                    DateFrom: $('#txtDLZreadFrom').val(),
                    DateTo: $('#txtDLZreadTo').val()
                },
                timeout: 600000,
                success: function (vData) {
                    if (vData != null) {
                        //window.location = GetPathFormat("/ReportsPOS/DownloadReportFile?OrgID=" + $('#cbDLZreadOrgs option:selected').val() + '&ReportLink=' + vData.ReportPOS);
                        aler('Done')
                    }
                }
            })
        })        

        $('#butDLNagaEJournal').click(function () {
            var pParams = {
                OrgID: $('#cbDLZreadOrgs option:selected').val(),
                FOWSTID: $('#cbDLZreadFOWST option:selected').val(),
                DeviceID: $('#cbDLZreadDevices option:selected').val(),
                From: $('#txtDLZreadFrom').val(),
                To: $('#txtDLZreadTo').val(),
                GenerateZeroSales: $('#chbZReadZeroSales').is(':checked'),
                ForceNONVAT: $('#chbZReadForceNONVAT').is(':checked'),
                ExcludeSales: $('#chbBIRReportSales').is(':checked'),
                ExcludeSC: $('#chbBIRReportSC').is(':checked'),
                ExcludePWD: $('#chbBIRReportPWD').is(':checked'),
                ExcludeNAAC: $('#chbBIRReportNAAC').is(':checked'),
                ExcludeSP: $('#chbBIRReportSP').is(':checked'),
                DownloadDataOnly: false,
                Format: $('[name="radBIRReportFileType"]:checked').val()
            };
            if ($('#chkZReadUseCache').is(':checked'))
                pParams.cacheid = mZReadCacheID;
            else
                mZReadCacheID = NewID();
            window.location = MergeUrlParameters(pParams, '/Utilities/DownloadNagaEJournal');
        })

        $('#butGenEmpComID').click(function () {
            AjaxSend({
                url: 'Utilities/GenerateEmployeeCompanyID',
                data: {
                    OrgID: $('#cbDLZreadOrgs option:selected').val()
                },
                timeout: 600000,
                success: function (vData) {
                    alert(vData.Result)
                }
            })
        })

        $('#butGetRecomputeZRead').click(function() {
            var From = $('#txtFrom').val();
            var To = $('#txtTo').val();

            var pOrgID;
            pOrgID = mCbOrgs.val();

            var pFOWSTID;
            pFOWSTID = mCbFOWST.val();

            var pDeviceID;
            pDeviceID = mCbDevices.val();

            if (!IsDate(new Date(From))) {
                $('#lblError').text('Date From is not a valid date');
                return;
            }

            if (!IsDate(new Date(To))) {
                $('#lblError').text('Date To is not a valid date');
                return;
                
            }

            AjaxSend({
                url: '/Utilities/RecomputeZRead',
                data: {
                    OrgID: pOrgID,
                    FOWSTID: pFOWSTID,
                    DeviceID: pDeviceID,
                    From: From,
                    To: To
                },
                success: function(vData) {
                    if (vData.Error)
                        $('#lblError').text('Error in recomputing Z-Read');
                    else
                        $('#lblError').text('Z-Read Recomputed Successfully');
                }
            }); // END AjaxSend({
        });

        $('#butRecacheZRead').click(function () {
            AjaxSend({
                url: 'Utilities/RecacheZread',
                data: {
                    OrgID: $('#cbDLZreadOrgs option:selected').val(),
                    LocationID: $('#cbDLZreadFOWST option:selected').val(),
                    DeviceID: $('#cbDLZreadDevices option:selected').val(),
                    DateFrom: $('#txtDLZreadFrom').val(),
                    DateTo: $('#txtDLZreadTo').val()
                },
                timeout: 600000,
                success: function (vData) {
                    if (vData.Result == 'success') {
                        alert('Success');
                    } else {
                        alert(vData.Error)
                    }
                }
            })
        })

        $('#butRecacheZReadByInvoiceID').click(function () {
            AjaxSend({
                url: '/Utilities/RecacheZReadByInvoiceID',
                data: { OrgID: $('#cbDLZreadOrgs option:selected').val(), InvoiceID: $('#txtZReadInvoiceID').val() },
                success: function (e) {
                    if (e.Result == 'ok') alert('success')
                    else alert('error')
                }
            })
        });

        $('#butScan').click(function() {
            mobile.Scan({
                Success: function (vData) {
                    $('#divScanResult').text(vData.Value);
                }
            });
        });

        $('#butDbDownloadFakeOrgID').click(function() {

            var pOrgID = mCbOrgs.val();
            var pData = {
                OriginalOrgID: pOrgID
            };
            AjaxSend({
                url: '/Utilities/FakeOrgID',
                data: pData,
                success: function(vData) {
                    if (vData.Error)
                        NewMessageBox(vData.Error);
                    else if (vData.Result)
                        NewMessageBox(vData.Result);
                }
            });
            
        }); // END $('#butFlush').click(function() {

        async function Print(vDevice) {
            var pText = 'test print - ' + new Date().toLocaleString();
            pText = printText(pText);
            var pResult = await vDevice.open();
            await vDevice.selectConfiguration(1);
            await vDevice.claimInterface(0);
            pResult = await vDevice.transferOut(1, pText);
            console.log(pResult);
        }

        function printText(text) {
            return new Uint8Array(text.split('').map(char => char.charCodeAt(0)))
        }

        $('#butFlush').click(function() {
            
            var pPassword = $('#txtSrvPassword').val();
            var pOrgID = $('#txtFlushOrgID').val();
            localStorage.FlushOrgID = pOrgID;
            var pDeviceID = $('#txtFlushDeviceID').val();
            localStorage.FlushDeviceID = pDeviceID;

            var pData = {
                Password: pPassword,
                FlushOrgID: pOrgID,
                FlushDeviceID: pDeviceID
            };
            AjaxSend({
                url: '/Utilities/ReimportFlush',
                data: pData,
                success: function(vData) {
                    if (vData.Error)
                        NewMessageBox(vData.Error);
                    else if (vData.Result)
                        $('#divFlushResult').text(vData.Result);
                }
            });
            
        }); // END $('#butFlush').click(function() {

        $('#txtFlushOrgID').val(localStorage.FlushOrgID);
        $('#txtFlushDeviceID').val(localStorage.FlushDeviceID);
        
        var resultContainer = document.getElementById('qr-reader-results');
        var lastResult, countResults = 0;

        function onScanSuccess(decodedText, decodedResult) {
            if (decodedText !== lastResult) {
                ++countResults;
                lastResult = decodedText;
                // Handle on success condition with the decoded message.
                alert(decodedText);
        }
    }

        $('#butJsonParser').click(function() {

            var pInput = $('#txtJsonParserInput').val();
            do {
                try {
                    if (pInput.hasOwnProperty("d")) 
                        pInput = pInput.d;
                    else if (pInput.hasOwnProperty("Error")) 
                        pInput = pInput.Error;
                    
                    var pJson = JSON.parse(pInput);
                    if (pJson.hasOwnProperty("Error")) {
                        pInput = pJson.Error;
                        continue;
                    }
                    if (pJson.hasOwnProperty("d")) {
                        pInput = pJson.d;
                        continue;
                    }
                } catch (e) {
                    $('#txtJsonParserOuput').val(pInput);
                    break;
                }
            } while (true)
        });

        var html5QrcodeScanner = new Html5QrcodeScanner(
            "qr-reader", { fps: 10, qrbox: 250 });
            html5QrcodeScanner.render(onScanSuccess);

        var mInvoiceTypeChangerBaseFilename;
        var mInvoiceTypeChangerErrorCount;
        $('#linkInvoiceTypeChangerAnalysisDownload').hide();
        $('#linkInvoiceTypeChangerResultDownload').hide();
        $('#formInvoiceTypeChangerUpload').submit(function(e) {
            e.preventDefault();
            mInvoiceTypeChangerBaseFilename = null;
            var pFormData = new FormData($(this)[0]);
            pFormData.append('OrgID', mCbOrgs.val());
            pFormData.append('Password', $('#txtSrvPassword').val());
            $('#divInvoiceTypeChanger').text('uploading...');
            $.ajax({
                url: '/Utilities/InvoiceTypeChangerUpload',
                type: 'POST',
                data: pFormData,
                async: false,
                cache: false,
                contentType: false,
                processData: false,
                success: function (vData) {
                    try {
                        vData = JSON.parse(vData);
                        if (vData.d)
                            vData = vData.d;
                    } catch (e) {
                        NewMessageBox('An error occured while parsing the server response to JSON: ' + e.message);
                        return;
                    }
                    if (vData.Error)
                        NewMessageBox(vData.Error);
                    else if (vData.Warning)
                        NewMessageBox(vData.Warning);
                    else if (vData.Result == 'ok') {
                        InvoiceTypeChangerAnalyze(vData.BaseFilename);
                    } else
                        NewMessageBox('Unknown status. ref jsHCTRGFCAGZ!');
                    //mOut.AfterSave(); // commented 2025_05_20__14_32_59_1428
                },
                error: function(vData) {
                    $('#divInvoiceTypeChanger').text('Error: ' + vData);
                }
            });
        });

        function InvoiceTypeChangerAnalyze(vBaseFilename) {
            $('#divInvoiceTypeChanger').text('analyzing...');
            AjaxSend({
                url: '/Utilities/InvoiceTypeChangerAnalyze',
                data: {
                    Password: $('#txtSrvPassword').val(),
                    BaseFilename: vBaseFilename,
                    OrgID: mCbOrgs.val()
                },
                success: function(vData) {
                    if (vData.Error) {
                        $('#divInvoiceTypeChanger').text('Error: ' + vData.Error);
                        return;
                    }
                    mInvoiceTypeChangerErrorCount = vData.ErrorCount;
                    mInvoiceTypeChangerBaseFilename = vData.BaseFilename;
                    $('#linkInvoiceTypeChangerAnalysisDownload')
                        .attr('href', 'Utilities/DownloadInvoiceTypeChangerAnalyzeResult?BaseFilename=' + mInvoiceTypeChangerBaseFilename)
                        .show();
                    $('#divInvoiceTypeChanger').text('Done: ' + vData);
                }
            }); // END AjaxSend({
        }

        $('#butInvoiceTypeChanger').click(function() {
            if (CStr2(mInvoiceTypeChangerBaseFilename).trim() == '') {
                NewMessageBox('Please upload a valid Excel file first.');
                return;
            }
            if (CInt2(mInvoiceTypeChangerErrorCount) > 0) {
                NewMessageBox('The analysis found ' + mInvoiceTypeChangerErrorCount + ' issue[s].');
                return;
            }
            NewMessageBox('Are you sure you want to apply changes from ' + mInvoiceTypeChangerBaseFilename + '? Make sure to download and review the analysis first.', 'Apply Changes?', [
                {
                    Text: 'Yes',
                    Click: function() {
                        InvoiceTypeChangerApply();
                    }
                },
                {
                    Text: 'No',
                    Click: function() {
                    }
                }
            ]);
        });

        function InvoiceTypeChangerApply() {
            AjaxSend({
                url: '/Utilities/InvoiceTypeChangerApply',
                data: {
                    Password: $('#txtSrvPassword').val(),
                    BaseFilename: mInvoiceTypeChangerBaseFilename,
                    OrgID: mCbOrgs.val()
                },
                success: function(vData) {
                    if (vData.Error) {
                        $('#divInvoiceTypeChanger').text('Error: ' + vData.Error);
                        return;
                    }
                    mInvoiceTypeChangerErrorCount = vData.ErrorCount;
                    mInvoiceTypeChangerBaseFilename = vData.BaseFilename;
                    $('#linkInvoiceTypeChangerResultDownload')
                        .attr('href', 'Utilities/DownloadInvoiceTypeChangerApplyResult?BaseFilename=' + mInvoiceTypeChangerBaseFilename)
                        .show();
                    $('#divInvoiceTypeChanger').text('Done: ' + vData);
                }
            }); // END AjaxSend({
        }

        $('#butDownloadReceipts').click(function() {
            var pPassword = $('#txtSrvPassword').val();
            if (pPassword.trim() == '') {
                MessageBox('Please enter secret password.');
                return;
            }

            var pOrgID = $('#cbDLZreadOrgs').val();
            if (pOrgID == '') {
                MessageBox('Please select org.');
                return;
            }

            var pFowstID = $('#cbDLZreadFOWST').val();
            if (CStr2(pFowstID) == '') {
                MessageBox('Please select fowst.');
                return;
            }

            var pDeviceID = $('#cbDLZreadDevices').val();
            if (CStr2(pDeviceID) == '') {
                MessageBox('Please select device.');
                return;
            }

            var pUrl = MergeUrlParameters({
                FowstID: pFowstID,
                DeviceID: pDeviceID,
                OrgID: pOrgID,
                InvoiceDateFrom: $('#txtDLZreadFrom').val(),
                InvoiceDateTo: $('#txtDLZreadTo').val()
            }, '/Utilities/GenerateSoftcopyInvoices');
            window.open(pUrl, '_blank');
        });

        $('#butCacheInventoryMovement').click(function() {
            var pPassword = $('#txtSrvPassword').val();
            if (pPassword.trim() == '') {
                MessageBox('Please enter secret password.');
                return;
            }

            var pOrgID = $('#cbDLZreadOrgs').val();
            if (pOrgID == '') {
                MessageBox('Please select org.');
                return;
            }

            var pFowstID = $('#cbDLZreadFOWST').val();
            if (CStr2(pFowstID) == '') {
                MessageBox('Please select fowst.');
                return;
            }

            AjaxSend({
                url: '/Utilities/CacheInventoryMovement',
                data: {
                    Password: $('#txtSrvPassword').val(),
                    FowstID: pFowstID,
                    OrgID: pOrgID,
                    InvoiceDateFrom: $('#txtDLZreadFrom').val(),
                    InvoiceDateTo: $('#txtDLZreadTo').val()
                },
                success: function(vData) {
                    if (vData.Error) {
                        MessageBox(vData.Error);
                        return;
                    }
                    if (vData.Result == 'ok')
                        MessageBox('affected records: ' + vData.AffectedRecords);
                    else
                        MessageBox('unknown reponse');
                }
            }); // END AjaxSend({
        });
        $('#butFranchiseeDataExporter').click(function() {
            var pPassword = $('#txtSrvPassword').val();
            if (pPassword.trim() == '') {
                MessageBox('Please enter secret password.');
                return;
            }

            var pOrgID = $('#cbDLZreadOrgs').val();
            if (pOrgID == '') {
                MessageBox('Please select org.');
                return;
            }

            AjaxSend({
                url: '/Utilities/FranchiseeDataExporter',
                data: {
                    Password: $('#txtSrvPassword').val(),
                    OrgID: pOrgID,
                    FranchiseeFowstID: $('#cbDLZreadFOWST option:selected').val(), // optional
                    InvoiceDateFrom: $('#txtDLZreadFrom').val(),
                    InvoiceDateTo: $('#txtDLZreadTo').val(),
                    DontStart: $('#chkFranchiseeDataExporter').is(':checked'),
                    DetectInvoicesWithNoItems: $('#chkFranchiseeDataExporterDetectInvoicesWithNoItems').is(':checked'),
                    StartFresh: $('#chkFranchiseeDataExporterStartFresh').is(':checked'),
                    ForceFranchiseeDatabaseID: $('#cbForceFranchiseeDatabase option:selected').val(), // optional
                    ForceFranchisorDatabaseID: $('#cbForceFranchisorDatabase option:selected').val(), // optional
                },
                success: function(vData) {
                    if (vData.Error) {
                        MessageBox(vData.Error);
                        return;
                    }
                    
                    if (vData.Result == 'ok')
                        MessageBox('FranchiseeDataExporter request was successfully submitted.');
                    else
                        MessageBox('unknown reponse');
                }
            }); // END AjaxSend({
        });

        $('#butRecacheData').click(function(){

            var pOrgID = $('#cbDLZreadOrgs').val();

            AjaxSend({
                type: 'GET',
                url: '/test/CacheInvoicesDaily?OrgID=' + pOrgID + '&invoicedatefrom=' + $.format.date(new Date((new Date()).setDate(new Date().getDate() - 4)),'yyyy-MM-dd') + '&invoicedateto=' + $.format.date(new Date((new Date()).setDate(new Date().getDate() - 1)),'yyyy-MM-dd') + '&days=99999',
                timeout: 600000,
                success: function(vData) {
                    $('#txtCacheDataResult').append(vData);

                    AjaxSend({
                        type: 'GET',
                        url: '/test/CacheInvoicesHourly?OrgID=' + pOrgID + '&invoicedatefrom=' + $.format.date(new Date((new Date()).setDate(new Date().getDate() - 4)),'yyyy-MM-dd') + '&invoicedateto=' + $.format.date(new Date((new Date()).setDate(new Date().getDate() - 1)),'yyyy-MM-dd') + '&days=99999',
                        timeout: 600000,
                        success: function(vData) {
                            $('#txtCacheDataResult').append(vData);

                            AjaxSend({
                                type: 'GET',
                                url: '/test/CacheInvoiceFinishedProducts?OrgID=' + pOrgID + '&invoicedatefrom=' + $.format.date(new Date((new Date()).setDate(new Date().getDate() - 4)),'yyyy-MM-dd') + '&invoicedateto=' + $.format.date(new Date((new Date()).setDate(new Date().getDate() - 1)),'yyyy-MM-dd') + '&days=99999',
                                timeout: 600000,
                                success: function(vData) {
                                    $('#txtCacheDataResult').append(vData);

                                    AjaxSend({
                                        type: 'GET',
                                        url: '/test/CacheInvoiceFlavors?OrgID=' + pOrgID + '&invoicedatefrom=' + $.format.date(new Date((new Date()).setDate(new Date().getDate() - 4)),'yyyy-MM-dd') + '&invoicedateto=' + $.format.date(new Date((new Date()).setDate(new Date().getDate() - 1)),'yyyy-MM-dd') + '&days=99999',
                                        timeout: 600000,
                                        success: function(vData) {
                                            $('#txtCacheDataResult').append(vData);
                                        },
                                        error: function (vError) {
                                            $('#txtCacheDataResult').append('Error in CacheInvoiceFlavors');
                                        }
                                    }); // END AjaxSend({
                                },
                                error: function (vError) {
                                    $('#txtCacheDataResult').append('Error in CacheInvoiceFinishedProducts');
                                }
                            }); // END AjaxSend({

                        },
                        error: function (vError) {
                            $('#txtCacheDataResult').append('Error in CacheInvoicesHourly');
                        }
                    }); // END AjaxSend({

                },
                error: function (vError) {
                    $('#txtCacheDataResult').append('Error in CacheInvoicesDaily');
                }
            }); // END AjaxSend({

        });

        $('#butLongProcess').click(function() {
            $('#butLongProcess').text('processing...');
            
            AjaxSend({
                url: '/test/LongProcess',
                data: {
                    
                },
                success: function(vData) {
                    $('#butLongProcess').text('done');
                },
                error: function(vError) {
                    MessageBox('Error: ' + vError);
                }
            }); // END AjaxSend({
        });

        $('#cbDLZreadFOWST').change(function () {
            CheckBegEndTrans()
        })

        $('#cbDLZreadDevices').change(function () {
            CheckBegEndTrans();            
        })

        $('#butGenerateDecal').click(function () {
            var pParams = {
                OrgID: $('#cbDLZreadOrgs option:selected').val(),
                FOWSTID: $('#cbDLZreadFOWST option:selected').val(),
                DeviceID: $('#cbDLZreadDevices option:selected').val()
            };
            var pUrl = MergeUrlParameters(pParams, '/Utilities/GetDeviceFowstPTUDetails');
            window.open(pUrl, '_blank');
        })

        function CheckBegEndTrans() {
            AjaxSend({
                url: 'Utilities/CheckBegEndTrans',
                data: {
                    OrgID: $('#cbDLZreadOrgs').val(),
                    FowstID: $('#cbDLZreadFOWST').val(),
                    DeviceID: $('#cbDLZreadDevices').val(),
                }, success: function (vData) {
                    $('#divBegEndTrans').text('The beginning and ending invoice details for the selected store and device: ' + JSON.stringify(vData.Result.Rows, null, 4));                    
                    DeviceFowstPTUDetails();
                }
            })
        }

        function DeviceFowstPTUDetails() {
            if ($('#cbDLZreadFOWST').val() == '' && $('#cbDLZreadDevices').val()) return

            AjaxSend({
                url: MergeUrlParameters({
                    OrgID: $('#cbDLZreadOrgs option:selected').val(),
                    FOWSTID: $('#cbDLZreadFOWST option:selected').val(),
                    DeviceID: $('#cbDLZreadDevices option:selected').val(),
                    IsReadOnly: 1
                }, '/Utilities/GetDeviceFowstPTUDetails'),
                success: function (vData) {
                    $('#divBegEndTrans').append('<br>PTU Details: ' + JSON.stringify(vData.Result.Rows, null, 4));
                    GetStoreCashiers();
                }
            })
        }

        function GetStoreCashiers(vFowstid) {
            var selXReadCashier = $('#selXReadCashier').empty();

            selXReadCashier.append(new Option('Please Select Cashier', ''));

            if ($('#cbDLZreadDevices').val() == '') return;

            AjaxSend({
                url: 'Browser/LibraryFOWSTRealCashier',
                data: { OrgID: $('#cbDLZreadOrgs').val(), FowstID: $('#cbDLZreadFOWST').val(), DeviceID: $('#cbDLZreadDevices').val() }, success: function (vData) {
                    var vRows = vData.Results.Rows;

                    for (var i in vRows) {
                        selXReadCashier.append(new Option(vRows[i].CashierName, vRows[i].ID));
                    }

                    selXReadCashier.selectmenu('refresh');
                }
            })
        }

        $('#butGenerateCopyCacheInvoicesSql').click(function() {
            $('#butLongProcess').text('processing...');
            
            var pOrgID = $('#cbDLZreadOrgs option:selected').val();
            AjaxSend({
                url: '/Utilities/GenerateCopyCacheInvoicesSql',
                data: {
                    Password: $('#txtSrvPassword').val(),
                    OrgID: pOrgID,
                    FranchiseeFowstID: $('#cbDLZreadFOWST option:selected').val(), // optional
                    InvoiceDateFrom: $('#txtDLZreadFrom').val(),
                    InvoiceDateTo: $('#txtDLZreadTo').val(),
                    DontStart: $('#chkFranchiseeDataExporter').is(':checked'),
                    DetectInvoicesWithNoItems: $('#chkFranchiseeDataExporterDetectInvoicesWithNoItems').is(':checked'),
                    StartFresh: $('#chkFranchiseeDataExporterStartFresh').is(':checked'),
                    ForceFranchiseeDatabaseID: $('#cbForceFranchiseeDatabase option:selected').val(), // optional
                    ForceFranchisorDatabaseID: $('#cbForceFranchisorDatabase option:selected').val(), // optional
                },
                success: function(vData) {
                    if (vData.Error) {
                        MessageBox(vData.Error);
                        return;
                    }
                    
                    if (vData.Result)
                        $('#txtGenerateCopyCacheInvoicesSql').val(vData.Result);
                    else
                        MessageBox('unknown reponse');
                }
            }); // END AjaxSend({
        });

        $('#butUpdateCacheInvoiceDaily').click(function() {
            var pOrgID = $('#cbDLZreadOrgs option:selected').val();
            AjaxSend({
                url: '/Utilities/UpdateCacheInvoiceDaily',
                data: {
                    Password: $('#txtSrvPassword').val(),
                    OrgID: pOrgID,
                    InvoiceDateFrom: $('#txtDLZreadFrom').val(),
                    InvoiceDateTo: $('#txtDLZreadTo').val()
                },
                success: function(vData) {
                    if (vData.Error) {
                        MessageBox(vData.Error);
                        return;
                    }
                    
                    if (vData.Result)
                        $('#txtGenerateCopyCacheInvoicesSql').val(vData.Result);
                    else
                        MessageBox('unknown reponse');
                }
            }); // END AjaxSend({
        }); // END $('#butUpdateCacheInvoiceDaily').click(function() {

        $('#butUpdateCacheInvoiceHourly').click(function() {
            var pOrgID = $('#cbDLZreadOrgs option:selected').val();
            AjaxSend({
                url: '/Utilities/UpdateCacheInvoiceHourly',
                data: {
                    Password: $('#txtSrvPassword').val(),
                    OrgID: pOrgID,
                    InvoiceDateFrom: $('#txtDLZreadFrom').val(),
                    InvoiceDateTo: $('#txtDLZreadTo').val()
                },
                success: function(vData) {
                    if (vData.Error) {
                        MessageBox(vData.Error);
                        return;
                    }
                    
                    if (vData.Result)
                        $('#txtGenerateCopyCacheInvoicesSql').val(vData.Result);
                    else
                        MessageBox('unknown reponse');
                }
            }); // END AjaxSend({
        }); // END $('#butUpdateCacheInvoiceHourly').click(function() {

        $('#butCacheSalesTables').click(function() {
            var pOrgID = $('#cbDLZreadOrgs option:selected').val();
            var pFowstID;
            if (!$('#chkDLZreadAllFowst').is(':checked'))
                pFowstID = $('#cbDLZreadFOWST option:selected').val();
            AjaxSend({
                url: '/Utilities/CacheSalesTables',
                data: {
                    Password: $('#txtSrvPassword').val(),
                    OrgID: pOrgID,
                    InvoiceDateFrom: $('#txtDLZreadFrom').val(),
                    InvoiceDateTo: $('#txtDLZreadTo').val(),
                    FowstID: pFowstID
                },
                success: function(vData) {
                    if (vData.Error) {
                        MessageBox(vData.Error);
                        return;
                    }
                    
                    if (vData.Result)
                        $('#txtGenerateCopyCacheInvoicesSql').val(vData.Result);
                    else
                        MessageBox('unknown reponse');
                }
            }); // END AjaxSend({
        }); // END $('#butCacheSalesTables').click(function() {

        $('#butDeleteCacheInvoicesDailyDuplicates').click(function() {
            var pOrgID = $('#cbDLZreadOrgs option:selected').val();
            AjaxSend({
                url: '/Utilities/DeleteCacheInvoicesDailyDuplicates',
                data: {
                    Password: $('#txtSrvPassword').val(),
                    OrgID: pOrgID
                },
                success: function(vData) {
                    if (vData.Error) {
                        MessageBox(vData.Error);
                        return;
                    }
                    if (vData.Result)
                        $('#txtGenerateCopyCacheInvoicesSql').val(vData.Result);
                    else
                        MessageBox('unknown reponse');
                }
            }); // END AjaxSend({
        }); // END $('#butUpdateCacheInvoiceDaily').click(function() {

        $('#butSendSalesTables').click(function() {
            var pOrgID = $('#cbDLZreadOrgs option:selected').val();
            var pFowstID;
            if (!$('#chkDLZreadAllFowst').is(':checked'))
                pFowstID = $('#cbDLZreadFOWST option:selected').val();
            AjaxSend({
                url: '/Utilities/SendSalesTables',
                data: {
                    Password: $('#txtSrvPassword').val(),
                    OrgID: pOrgID,
                    InvoiceDateFrom: $('#txtDLZreadFrom').val(),
                    InvoiceDateTo: $('#txtDLZreadTo').val(),
                    FowstID: pFowstID
                },
                success: function(vData) {
                    if (vData.Error) {
                        MessageBox(vData.Error);
                        return;
                    }
                    if (vData.Result) {
                        $('#txtGenerateCopyCacheInvoicesSql').val(vData.Result);
                        MessageBox(vData.Result);
                    } else
                        MessageBox('unknown reponse');
                }
            }); // END AjaxSend({
        }); // END $('#butSendSalesTables').click(function() {
    }); // $(function() {

});