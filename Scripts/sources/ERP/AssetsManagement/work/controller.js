define([
    'promise!/Scripts/sources/ERP/AssetsManagement/work/model.js?v=' + assetversion,
    'promise!/Scripts/sources/ERP/AccountDetails/controller.js?v=' + assetversion,
    '/Scripts/sources/CreditAndCollection/CacPaymentPopup.js?v=' + assetversion,
    'promise!/Scripts/jquery.filedownload.min.js',
], function () {

    var mDeferred = $.Deferred();
    var mHtml;

    (function staticInit() {

        AjaxSend({
            ExactUrl: '/Scripts/sources/ERP/AssetsManagement/work/view.html?v=' + assetversion,
            type: 'GET',
            dataType: 'text',
            success: function (vData) {
                mHtml = vData;
                mDeferred.resolve();
            }
        });
    })();

    CreateStringObject('ERP.AssetsManagement.work.controller', null, function () {

        var mOut = {};
        var mElement, dtAssetsMgmt, workModel = new ERP.AssetsManagement.work.model();
        var clientID = '', dateFrom, dateTo, action, assetData, selectedAssetData, assetID = '', transID = '', orgid = '', dateofbirth = '', emailid = '', addressid = '', mobileid = '';
        var isLoadAssetType = false, isFromExistingClient = false, arrSearchMonth = [], ctrSearchMonth = 1;
        var isFromReserved = false, isFromSearchClient = false, isTransferAsset = false;
        var cardCount = 0, currentCardCount = 1, countMessage = '1st';
        var CardID, loadassets = true, tbody;
        var mCardCreationInterval, arrCustomerRequest = [], CustomerRequestIndex = null;
        var arrCustomer = [], isAddNewCustomer = true, CustomerIndex, isEmptyCustomer = true;
        var dsrSelectedDate;

        mElement = $(mHtml);
        mOut.element = mElement;

        mOut.CheckOudatedReservationDate = function (year, month) {
            var tbody = (isTransferAsset ? $('#tbodyTransferAvailability') : $('#tbodyAvailability'));
            var selAvailabilityMonth = $('#selAvailabilityMonth');
            var txtAvailabilityYear = $('#txtAvailabilityYear');
            month = selAvailabilityMonth.val();
            year = txtAvailabilityYear.val();

            tbody.find('td').removeClass('finishday');

            for (var i = 1; i <= 31; i++) {
                var curDate = CInt2($.format.date(new Date(), 'yyyyMMdd'));
                var curLoopDate = CInt2($.format.date(new Date(year, month, i), 'yyyyMMdd'));
                if (curLoopDate < curDate) {
                    if (CBool(AssetMgmtSettingsController.IsAllowOutdatedReservation) && !isTransferAsset) { //Disable prev dates when transfer an assets
                        tbody.find('td[date="' + i + '"]').removeClass('finishday');
                    } else {
                        tbody.find('td[date="' + i + '"]').addClass('finishday');
                    }                    
                } else if (curLoopDate == curDate) {
                    $('th[date="' + i + '"]').addClass('curDay')
                }
            }
        }        

        $('#tabAssetsMgmtWork').append(mElement).trigger('create');

        $('#btnMoreOptions').click(function () {
            var arrOptions = [];

            arrOptions = [
                               {
                                   Text: 'Go to Credit and Collection ',
                                   Click: function () {
                                       PromptUserAccess('Credit and Collection (EAS)', {
                                           OnClickOKSuccess: function () {
                                               $('#divErpRevenue').pageshow();
                                               GotoCac('AR');
                                           },
                                           OnFailed: function () {
                                               console.log('jsCFDTPD3GXC!');
                                           }
                                       });
                                   }
                               }
            ]

            arrOptions.push({
                Text: 'Read Guest Card Content',
                Click: function () {
                    $('#popupCardInformation').modal('show');

                    mOut.GetGuestCardInformation();
                }
            })

            arrOptions.push({
                Text: 'Daily Sales Report',
                Click: function (e) {
                    $('button[data-target="#tabDSRCheckIns"]').tab('show');
                    dsrSelectedDate = $.format.date(new Date(), 'MMM dd, yyyy');
                    $('#btnDSRSelectedDate').text(dsrSelectedDate)
                    GetCheckInsMadeToday();
                    return
                    $('#txtSalesReportDate').attr('max', $.format.date(new Date(), 'yyyy-MM-dd')).val($.format.date(new Date(), 'yyyy-MM-dd'))                                        
                }
            })

            PopupButtonList('', 'More Options', arrOptions, { AutoDismiss: true });
        })

        $('#btnDownloadDailySalesReport').unbind('click').click(function () {
            Bs.Loading('show');
            AjaxSend({
                url: MergeUrlParameters({
                    OrgID: sessionStorage.OrgID,
                    Date: '03-22-2024'
                }, '/AssetsManagement/DownloadSalesReport'),
                success: function (e) {
                    Bs.Loading('hide');
                    var pUrl = MergeUrlParameters({
                        Filename: e.success
                    }, '/AssetsManagement/PreviewReports');
                    window.open(pUrl, '_blank',
                        'width=' + window.outerWidth +
                        ',height=' + window.outerHeight +
                        ',scrollbars=auto');
                    NewMessageBox('You should be viewing the PDF by now. If you are unable to view the PDF, please check your web browser\'s popup settings.');
                }
            })

            return
            $.fileDownload(MergeUrlParameters({
                OrgID: sessionStorage.OrgID,
                Date: $('#txtSalesReportDate').val()
            }, '/AssetsManagement/DownloadSalesReport'), {
                prepareCallback: function () {
                    Bs.Loading('show', 'Preparing your report. Please wait ...');
                },
                successCallback: function () {
                    Bs.Loading('hide');
                    NewMessageBox('Your report has been generated. Please download the file to your computer.')
                },
                failCallback: function (responseHtml, url, error) {
                    var res = JSON.parse(responseHtml).d.Results;
                    NewMessageBox(res, 'Message');
                    Bs.Loading('hide');
                }
            });
        })

        $('#btnDSRSelectedDate').click(function () {
            Bs.Calendar({
                Format: 'MMM dd, yyyy',
                Click: function (e) {
                    dsrSelectedDate = e;
                    $('#btnDSRSelectedDate').text(dsrSelectedDate);
                    GetCheckInsMadeToday();
                }
            })
        })

        $('#btnGetCardInformation').click(function () {
            mOut.GetGuestCardInformation();
        })

        $('#btnDateReservation').click(function () {
            if (action == 'reserve') {
                var data = {
                    Title: 'Reservation Date',
                    ShowTime: 't',                    
                    SetFromDate: dateFrom,
                    SetToDate: dateTo,
                    SetToTime: (dateTo == '' ? AssetMgmtSettingsController.DefaultCheckOutTime.Time : $.format.date(new Date(dateTo), 'hh:mm a')),
                    Click: function (e) {
                        dateFrom = e.From + ' ' + $.format.date(new Date(), 'hh:mm a');
                        dateTo = e.To;
                        $('#btnDateReservation').text(dateFrom + ' to ' + dateTo);
                        $('#popupAssetNewCustomer').modal('show');
                        $('#lblNoOfDays').text(Math.abs((new Date($.format.date(new Date(dateFrom), 'MMM dd, yyyy')) - new Date($.format.date(new Date(dateTo), 'MMM dd, yyyy'))) / (1000 * 60 * 60 * 24)));
                    },
                    Close: function () {
                        $('#popupAssetNewCustomer').modal('show')
                    }
                }

                if (!CBool(AssetMgmtSettingsController.IsAllowOutdatedReservation)) { // disable prev dates
                    data.MinFromDate = $.format.date(new Date(), 'MMM dd, yyyy');
                    data.MinToDate = $.format.date(new Date(), 'MMM dd, yyyy');
                }

                DualCalendar(data);
            } else {
                var data = {
                    Title: 'Check Out / Return Asset Date & Time',
                    ShowTime: true,                   
                    SetDate: dateTo,
                    SetTime: (dateTo == '' ? AssetMgmtSettingsController.DefaultCheckOutTime.Time : $.format.date(new Date(dateTo), 'hh:mm a')),
                    Click: function (e) {
                        dateTo = e;
                        $('#btnDateReservation').text(dateTo);
                        $('#popupAssetNewCustomer').modal('show');
                        $('#lblNoOfDays').text(Math.abs((new Date($.format.date(new Date(dateFrom), 'MMM dd, yyyy')) - new Date($.format.date(new Date(dateTo), 'MMM dd, yyyy'))) / (1000 * 60 * 60 * 24)));
                    },
                    Close: function () {
                        $('#popupAssetNewCustomer').modal('show')
                    }
                }

                if (!CBool(AssetMgmtSettingsController.IsAllowOutdatedReservation)) { // disable prev dates
                    data.MinFromDate = $.format.date(new Date(), 'MMM dd, yyyy');
                }

                SingleCalendar(data);
            }            
        })

        $('#btnDateOfBirth').click(function () {
            if ($('#btnDateOfBirth')[0].hasAttribute('disabled')) return;
            $('#popAssetCustomerInfo').unbind('hidden.bs.modal');
            SingleCalendar({
                Title: 'Set Birth Date',
                MinDate: true,
                Click: function (e) {
                    dateofbirth = e;
                    $('#btnDateOfBirth').text(e);
                },
                Close: function () {
                    $('#popAssetCustomerInfo').modal('show').on('hidden.bs.modal', function () {
                        $('#popupAssetNewCustomer').modal('show');
                    })
                }
            })
        })

        $('#btnCheckAssetAvailability').click(function () {
            isTransferAsset = false;
            CheckAvailability();
        })

        $('#chbShowPastCheckins').change(function () {
            isTransferAsset = false;
            CheckAvailability();
        })

        $('#btnSaveAssetCus').click(function () {
            mOut.SaveAssetCustomer();
        })

        $('#selAssetStatus, #selAssetType').change(function () {
            GetListOfAssets();
        })

        $('#txtSearchAsset').keypress(function (e) {
            if(e.which == 13){
                GetListOfAssets();
            }            
        })

        $('#btnSearchAssetOrg, #txtAssetOrgName').click(function () {
            BrowsePopup('CRMDashboard/GetAllClientList', 'Select Organization', [
                    {
                        DisplayText: [{ Column: 'ClientName' }],
                        Search: $.trim($(this).val()),
                        Click: function (e) {
                            orgid = e.ID;
                            $('#txtAssetOrgName').val(e.ClientName);
                        },
                        Close: function () {
                            $('#popupAssetNewCustomer').modal('show')
                        }
                    }
            ], { Type: 'Org' })
        })

        $('#btnClearExistingCustomer').click(function () {
            if (!isAddNewCustomer) {
                NewMessageBox('Clearing and filling new customer information will be considered as adding new customer. Do you want to continue?', 'Message', [
                    {
                        Text: 'Yes, clear and fill new customer info',
                        Click: function (e) {
                            ClearCustomer();
                        }
                    }, { Text: 'No' }
                ])
            } else {
                ClearCustomer();
            }
            //clientID = '', addressid = '', mobileid = '', emailid = '', dateofbirth = '';
            //$('.formppol').val('');
            //$('#btnDateOfBirth').text('Set Date');
            //$('#selAssetCusCity').val('316a03d3-611a-4376-a2b2-7d0dfd7a4fbc'); //  default to Davao City
            //$('#selAssetCusCountry, #selAssetCusCountryCode').val('e783ed13-aa98-4cc1-918e-d49b05c0167e'); // default to Philippines
        })

        $('#btnSearchExistingCustomer').click(function () { //CRMDashboard/GetAllClientList
            if (assetData.Status == 'cancelled') {
                NewMessageBox('This reservation has already been cancelled. There is no need to modify.');
                return;
            }

            $('#popAssetCustomerInfo').unbind('hidden.bs.modal');
            if (!isAddNewCustomer) {
                NewMessageBox('Selected search customer will be considered as adding new customer. Do you want to continue?', 'Message', [
                    {
                        Text: 'Yes, search new customer',
                        Click: function (e) {
                            SearchCustomer();
                        }
                    }, {
                        Text: 'No',
                        Click: function (e) {
                            $('#popAssetCustomerInfo').modal('show').unbind('hidden.bs.modal').on('hidden.bs.modal', function () {
                                $('#popAssetCustomerInfo').unbind('hidden.bs.modal');
                                $('#popupAssetNewCustomer').modal('show');
                            });;
                        }
                    }
                ], {
                    AutoDismiss: false
                })
            } else {
                SearchCustomer();
            }
        })

        $('#btnRemoveAssetOrg').click(function () {
            orgid = '';
            $('#txtAssetOrgName').val('');
        })
                
        $('#btnAvailabilityPrev, #btnTransferAvailabilityPrev').click(function () {
            IsTransfer();
            if (arrSearchMonth.length > 0) {
                SearchNav('prev');
            } else {
                NavigateDate('prev');
            }            
        })

        $('#btnAvailabilityNext, #btnTransferAvailabilityNext').click(function () {
            IsTransfer();
            if (arrSearchMonth.length > 0) {
                SearchNav('next');
            } else {
                NavigateDate('next');
            }
        })

        $('#txtAvailabilityYear, #selAvailabilityMonth, #txtTransferAvailabilityYear, #selTransferAvailabilityMonth').change(function (e) {
            var year = $.trim($('#txtAvailabilityYear').val());
            var month = $('#selAvailabilityMonth').val();
            var enddate = FormatDate(new Date(CInt2(year), CInt2(month) + 1, 0), { Format: 'd' });
            IsTransfer();
            GetAsset(month, year, enddate);
        })

        $('.txtAssetSearchClient').keypress(function (e) {
            if (e.which == 13) {
                if ($.trim($(this).val()) != "") {
                    $('.txtAssetSearchClient').val($(this).val());
                    SearchClient($(this).val());
                }                
            }
        })

        $('#btnBackSearchAssetMgmt').click(function () {
            $('#txtAssetSearchClient').val('');
            $('#divMainAssetMgmt').pageshow();
        })

        $('#tabAssetsMgmtWork').opentabmenu();

        $('#btnOutOfOrderSave').click(function () {
            if (new Date($('#txtOOODateFrom').val()) > new Date($('#txtOOODateTo').val())) {
                WebToast('Invalid date range', 'top');
                return;
            }

            workModel.Save({
                data: {
                    TransID: CStr2(assetData.TransID),
                    AssetID: (assetData.AssetID || assetData.ID),
                    Action: 'outoforder',
                    DateFrom: $('#txtOOODateFrom').val(),
                    DateTo: $('#txtOOODateTo').val(),
                    OutOfOrderReason: $('#txtOOOReason').val(),
                    IsOutOfOrder: true
                },
                success: function (e) {
                    WebToast(assetData.AssetName + ' was tagged as Out of Order', 'top');
                    CheckAvailability(function () {
                        AssetMgmtDashboardController.GetDashboardData();
                    });
                    mOut.ClearFieldsNewCustomer();
                    NewMessageBoxClose();
                }
            })
        })

        $('#btnAddAssetCustomerRequest').click(function () {
            Bs.NestedModal('#popAssetCustomerInfo', '#popAssetCustomerRequest', {
                Hidden: function () {
                    Bs.NestedModal('#popupAssetNewCustomer', '#popAssetCustomerInfo');
                },
                Shown: function () {
                    $('#txtAssetCustomerRequest').val('').focus();
                    CustomerRequestIndex = null;
                }
            });
        })

        $('#btnOkAssetCustomerRequest').click(function () {
            var txtAssetCustomerRequest = $('#txtAssetCustomerRequest').val();

            if ($.trim(txtAssetCustomerRequest) == '') {
                WebToast('Please enter guest\'s request', 'top');
                return;
            }

            if(CustomerRequestIndex == null){
                arrCustomerRequest.push({
                    ID: '',
                    Request: $.trim(txtAssetCustomerRequest),
                    State: 'new'
                })
            } else {
                if (CStr2(arrCustomerRequest[CustomerRequestIndex].ID) != '' && (arrCustomerRequest[CustomerRequestIndex].Request != $.trim(txtAssetCustomerRequest))) {
                    arrCustomerRequest[CustomerRequestIndex].State = 'modified';
                }

                arrCustomerRequest[CustomerRequestIndex].Request = $.trim(txtAssetCustomerRequest);                                
            }

            $('#popAssetCustomerRequest').modal('hide');
            DisplayRequest();
        })

        $('#btnAddAssetCustomerPreviousRequest').click(function () {
            Bs.NestedModal('#popAssetCustomerInfo', '#popupCustomerPreviousRequest', {
                Hidden: function () {
                    Bs.NestedModal('#popupAssetNewCustomer', '#popAssetCustomerInfo');
                }
            });
        })

        $('#btnMakeGuestCard').click(function () {
            currentCardCount = 1;
            mOut.CreateCard(1000);
            $('#popupCardCreation').modal('show');
            $('#dvMessage').text('Please put card on top of the card reader');
        })

        $('#btnCancelGuestCard').click(function () {
            $('#popupCardCreationConfirmationMessage').modal('show');
        });

        $('#btnAddGuest').click(function () {
            mOut.ClearCustomerInfoFields();
            isAddNewCustomer = true;
            //$('#popAssetCustomerInfo').modal('show');
            $('#btnSearchExistingCustomer').click();
        })

        $('#btnOkAssetCustomer').click(function () {
            var txtAssetCusFirstName = $.trim($('#txtAssetCusFirstName').val());
            var txtAssetCusMiddleName = $.trim($('#txtAssetCusMiddleName').val());
            var txtAssetCusLastName = $.trim($('#txtAssetCusLastName').val());
            var txtAssetCusHomeNo = $.trim($('#txtAssetCusHomeNo').val());
            var txtAssetCusStreetName = $.trim($('#txtAssetCusStreetName').val());
            var selAssetCusCity = $.trim($('#selAssetCusCity').val());
            var selAssetCusCountry = $.trim($('#selAssetCusCountry').val());
            var selAssetCusCountryCode = $.trim($('#selAssetCusCountryCode').val());
            //var selAssetCusTelcoPrefix = $.trim($('#selAssetCusTelcoPrefix').val()); // // 2017_10_26__09_03_55_0587 dont use dropdown for mobile number
            var txtAssetCusMobileNo = $.trim($('#txtAssetCusMobileNo').val());
            var txtAssetCusEmail = $.trim($('#txtAssetCusEmail').val());

            if (txtAssetCusFirstName == '' || txtAssetCusLastName == '') {
                $('#tabCusPersonalInfo').opentabmenu();
                WebToast('Personal Info is required. Please fill the required fields.', 'top');
                return;
            }

            if (!IsNumeric(txtAssetCusMobileNo) || txtAssetCusMobileNo == '' || txtAssetCusMobileNo.length < 10) {
                $('#tabCusPersonalInfo').opentabmenu();
                $('#txtAssetCusMobileNo').focus();
                WebToast('Please enter valid mobile number', 'top');
                return;
            }

            if (txtAssetCusEmail != '') {
                if (!IsEmail(txtAssetCusEmail)) {
                    $('#tabCusPersonalInfo').opentabmenu();
                    WebToast('Please enter valid email', 'top');
                    return;
                }
            }

            var data = {
                ClientID: (clientID || ''),
                FirstName: txtAssetCusFirstName,
                MiddleName: txtAssetCusMiddleName,
                Surname: txtAssetCusLastName,
                AddressID: CStr2(addressid),
                HouseBuildingNo: txtAssetCusHomeNo,
                Street: txtAssetCusStreetName,
                CityID: selAssetCusCity,
                CountryID: selAssetCusCountry,
                MobileID: CStr2(mobileid),
                CountryCodeID: selAssetCusCountryCode,
                // TelCoPrefixID: selAssetCusTelcoPrefix, // // 2017_10_26__09_03_55_0587 dont use dropdown for mobile number
                ContactNo: txtAssetCusMobileNo,
                EmailID: CStr2(emailid),
                EmailAddress: txtAssetCusEmail,
                DateOfBirth: dateofbirth,
                Request: (arrCustomerRequest || [])
            }

            if (isAddNewCustomer) {
                arrCustomer.push(data)
            } else {
                arrCustomer[CustomerIndex] = $.extend(arrCustomer[CustomerIndex], data);
            }

            DisplayCustomer();
            $('#popAssetCustomerInfo').modal('hide');
        })

        LoadRequiredData();

        function GetCheckInsMadeToday() {
            Bs.Loading('show');
            AjaxSend({
                url: '/AssetsManagement/CheckInsMadeToday',
                data: { SalesDate: dsrSelectedDate },
                success: function (e) {
                    Bs.Loading('hide');

                    var tbody = $('#tblDSRCheckIns tbody').empty();
                    var vRows = e.Results.Rows;

                    for (var i in vRows) {
                        var data = vRows[i];

                        tbody.append($('<tr>')
                            .append('<td>' + (CInt2(i) + 1) + '.</td>')
                            .append('<td>' + data.RoomNo + '</td>')
                            .append('<td>' + data.CustomerName + '</td>')
                            .append('<td class="text-center">' + $.format.date(new Date(data.CheckedInDate), 'MM-dd-yyyy') + '</td>')
                            .append('<td class="text-center">' + $.format.date(new Date(data.CheckedOutDate), 'MM-dd-yyyy') + '</td>')
                            .append('<td class="text-center">' + data.NoOfNights + '</td>')
                            .append('<td class="text-right">' + AccountingFormat(CDec2(data.Rate)) + '</td>')
                            .append('<td class="text-right">' + AccountingFormat(CDec2(data.Rate) * CDec2(data.NoOfNights)) + '</td>')
                            .append('<td>' + data.OnlineTravelAgenyName + '</td>')
                        )
                    }

                    $('#popupDailySalesReport').modal('show');
                }
            })            
        }

        function SearchCustomer() {
            BrowsePopup('/AssetsManagement/SearchExistingGuest', 'Select ' + (AssetMgmtSettingsController.LabelSetting == 'hotel industry' ? 'Guest' : AssetMgmtSettingsController.CustomersLabel), [
                    {
                    DisplayText: [
                        { Column: 'CustomerName' },
                        {
                            Column: function (data) {
                                var lbl = '';

                                if (IsDate(data.LastCheckedIn)) {
                                    lbl = 'Last ' + (AssetMgmtSettingsController.LabelSetting == 'hotel industry' ? 'Checked-in' : 'Used') + ': '
                                    lbl += $.format.date(new Date(data.LastCheckedIn), 'MMM dd, yyyy hh:mm a')
                                }

                                return lbl;
                            }
                        }
                    ],
                    ShowAddButton: function () {
                        isAddNewCustomer = true;
                        mOut.ClearCustomerInfoFields();
                    },
                    Click: function (p) {
                        isAddNewCustomer = true;
                        mOut.ClearCustomerInfoFields();

                        workModel.GetExistingCustomerDetails({
                            data: { ID: p.ID },
                            success: function (e) {
                                isFromExistingClient = true;
                                clientID = p.ID;
                                dateofbirth = (CStr2(p.PersonDateOfBirth) == '' ? '' : FormatDate(ConvertDate(p.PersonDateOfBirth), { Format: 'MMM dd, yyyy' }))
                                $('#txtAssetCusFirstName').val(p.PersonFirstName).prop('disabled', 1);
                                $('#txtAssetCusMiddleName').val(CStr2(p.PersonMiddleName)).prop('disabled', 1);
                                $('#txtAssetCusLastName').val(p.PersonSurname).prop('disabled', 1);
                                $('#btnDateOfBirth').text((dateofbirth || 'Set Birth Date')).prop('disabled', 1);

                                if (e.Address.length > 0) {
                                    addressid = e.Address[0].ID;  // select only first row
                                    $('#txtAssetCusHomeNo').val(e.Address[0].HouseBldgNo).prop('disabled', 1);
                                    $('#txtAssetCusStreetName').val(e.Address[0].Street).prop('disabled', 1);
                                    $('#selAssetCusCity').val(e.Address[0].CityID).prop('disabled', 1); //  default to Davao City
                                    $('#txtAssetCusCountry').val(e.Address[0].CountryID).prop('disabled', 1); // default to Philippines
                                }

                                if (e.Mobile.length > 0) {
                                    mobileid = e.Mobile[0].MobileID; // select only first row
                                    $('#selAssetCusCountryCode').val(e.Mobile[0].CountryCodeID).prop('disabled', 1);
                                    // $('#selAssetCusTelcoPrefix').val(e.Mobile[0].TelCodeID); // 2017_10_26__09_03_55_0587 dont use dropdown for mobile number
                                    $('#txtAssetCusMobileNo').val(e.Mobile[0].MobileNo).prop('disabled', 1);
                                }

                                if (e.Email.length > 0) {
                                    emailid = e.Email[0].EmailAddID; // select only first row
                                    $('#txtAssetCusEmail').val(CStr2(e.Email[0].EmailAdd)).prop('disabled', 1);
                                }

                                AjaxSend({
                                    url: 'AssetsManagement/GetCustomerRequestHistory',
                                    data: { ClientID: clientID },
                                    success: function (e) {
                                        DisplayPreviousRequest(e.Results.Rows);
                                    }
                                })
                            }
                        })
                    },
                    Close: function () {
                        $('#popAssetCustomerInfo').modal('show').unbind('hidden.bs.modal').on('hidden.bs.modal', function () {
                            $('#popAssetCustomerInfo').unbind('hidden.bs.modal');
                            $('#popupAssetNewCustomer').modal('show');
                        });
                    }
                }
            ], { FilterBy: 'Person' })
        }

        function ClearCustomer() {
            isAddNewCustomer = true;
            mOut.ClearCustomerInfoFields();
            WebToast('Cleared personal info', 'top');
        }

        function DisplayCustomer() {
            var tbodyAssetCustomerList = $('#tbodyAssetCustomerList'), no = 0;
            isEmptyCustomer = true;

            tbodyAssetCustomerList.empty();

            if(arrCustomer.length > 0){
                for (var i = 0; i < arrCustomer.length; i++) {
                    arrCustomer[i].Index = i;

                    if (arrCustomer[i].Status != 'removed') {
                        isEmptyCustomer = false;
                        no += 1;
                        tbodyAssetCustomerList.append($('<tr>')
                            .append($('<td style="width:50px;">').text(no + '.'))
                            .append($('<td>').text($.trim(arrCustomer[i].FirstName + ' ' + arrCustomer[i].MiddleName + ' ' + arrCustomer[i].Surname)))
                            .append($('<td style="width:100px; text-align:right;">')
                                .append($('<button data-role="none" data-toggle="tooltip" data-placement="top" title="Edit" class="btn btn-warning">')
                                    .css('display', (CBool(assetData.IsPastCheckins) ? 'none' : 'unset'))
                                    .append($('<span class="bi-pencil-square">'))
                                    .data('data', arrCustomer[i])
                                    .click(function () {
                                        if (assetData.Status == 'cancelled') {
                                            NewMessageBox('This reservation has already been cancelled. There is no need to modify.');
                                            return;
                                        }

                                        var data = $(this).data('data');                                        
                                        mOut.ClearCustomerInfoFields();
                                        isAddNewCustomer = false;
                                        $('#popupAssetNewCustomer').modal('hide');
                                        $('#popAssetCustomerInfo').modal('show').unbind('hidden.bs.modal').on('hidden.bs.modal', function () {
                                            $('#popAssetCustomerInfo').unbind('hidden.bs.modal');
                                            $('#popupAssetNewCustomer').modal('show');
                                        });

                                        CustomerIndex = data.Index;
                                        clientID = CStr2(data.ClientID);
                                        emailid = CStr2(data.EmailID);
                                        mobileid = CStr2(data.MobileID);
                                        addressid = CStr2(data.AddressID);
                                        arrCustomerRequest = (data.Request || []);
                                        dateofbirth = (CStr2(data.PersonDateOfBirth) == '' ? '' : data.PersonDateOfBirth);
                                        $('#txtAssetCusFirstName').val(data.FirstName);
                                        $('#txtAssetCusMiddleName').val(CStr2(data.MiddleName));
                                        $('#txtAssetCusLastName').val(data.Surname);
                                        $('#txtAssetCusHomeNo').val(data.HouseBuildingNo);
                                        $('#txtAssetCusStreetName').val(data.Street);
                                        $('#selAssetCusCity').val(data.CityID); //  default to Davao City
                                        $('#txtAssetCusCountry').val(data.CountryID); // default to Philippines
                                        $('#selAssetCusCountryCode').val(data.CountryCodeID || 'e783ed13-aa98-4cc1-918e-d49b05c0167e');
                                        // $('#selAssetCusTelcoPrefix').val(data.TelCoPrefixID); // 2017_10_26__09_03_55_0587 dont use dropdown for mobile number
                                        $('#txtAssetCusMobileNo').val(CStr2(data.Prefix) + CStr2(data.ContactNo));
                                        $('#txtAssetCusEmail').val(CStr2(data.EmailAddress));
                                        $('#btnDateOfBirth').text((dateofbirth || 'Set Birth Date'));
                                    })
                                )
                                .append('&nbsp;')
                                .append($('<button data-role="none" data-toggle="tooltip" data-placement="top" title="Remove" class="btn btn-danger">')
                                    .css('display', (CBool(assetData.IsPastCheckins) ? 'none' : 'unset'))
                                    .append($('<span class="bi-trash">'))
                                    .data('data', arrCustomer[i])
                                    .click(function () {
                                        if (assetData.Status == 'cancelled') {
                                            NewMessageBox('This reservation has already been cancelled. There is no need to modify.');
                                            return;
                                        }

                                        var data = $(this).data('data');

                                        NewPromptUserAccess('Allow removal of guest(s)/ lessee(s) assigned to an asset',
                                            {
                                                Message: 'You are not allowed to remove guest(s)/ lessee(s).',
                                                OnClickOKSuccess: function (e) {
                                                    NewMessageBox('Are you sure you want to remove ' + data.FirstName + ' ' + data.Surname + '?', 'Message', [
                                                        {
                                                            Text: 'Yes',
                                                            Click: function () {
                                                                data.Status = 'removed';
                                                                DisplayCustomer();
                                                            }
                                                        }, { Text: 'No' }
                                                    ]);
                                                }
                                            }
                                        )                                        
                                    })
                                )
                            )
                        )
                    }
                }
            }

            if(isEmptyCustomer){
                tbodyAssetCustomerList.append($('<tr>')
                        .append($('<td colspan="3" style="text-align:center;">').text('No result(s) found'))
                    )
            }
        }

        function DisplayPreviousRequest(data) {
            var tbodyCustomerPreviousRequest = $('#tbodyCustomerPreviousRequest');

            tbodyCustomerPreviousRequest.empty();

            if(data.length > 0){
                for (var i = 0; i < data.length; i++){
                    tbodyCustomerPreviousRequest.append($('<tr>')
                            .append($('<td>').text(i + 1 + '.'))
                            .append($('<td>')
                                .append($('<p style="white-space: pre-line">')
                                    .append(data[i].Request)
                                )
                            )
                        )
                }
            } else {
                tbodyCustomerPreviousRequest.append($('<tr>')
                        .append($('<td colspan="2" style="text-align:center;">').text('No previous request(s)'))
                    )
            }
        }

        function DisplayRequest() {
            var listAssetCustomerRequest = $('#listAssetCustomerRequest');

            listAssetCustomerRequest.empty();

            for (var i = 0; i < arrCustomerRequest.length; i++) {
                var status = $('<tr>').append($('<td>')
                                .text(arrCustomerRequest[i].Request)
                                .append((arrCustomerRequest[i].State != 'new' ? '<br><br>' +  arrCustomerRequest[i].Action + ' by ' + arrCustomerRequest[i].ModifiedBy + ' on ' + $.format.date(new Date(arrCustomerRequest[i].ModifiedDate), 'MMM dd, yyyy hh:mm a') : ''))
                            );

                arrCustomerRequest[i].Index = i;

                if (arrCustomerRequest[i].State != 'new') {
                    status.append($('<td style="width:50px; text-align:center;">')
                            .append($('<button data-role="none" data-toggle="tooltip" title="Show History" class="btn btn-secondary">')
                                .data('data', arrCustomerRequest[i])
                                .append($('<span class="bi-clock">'))
                                .click(function (e) {
                                    e.stopPropagation();
                                    e.preventDefault();

                                    var data = $(this).data('data');
                                    GetHistoryOfSelectedRequest(data);
                                })
                            )
                        )
                }

                status.append($('<td style="width:50px; text-align:center;">')
                        .append($('<button data-role="none" data-toggle="tooltip" title="Delete" class="btn btn-danger">')
                            .data('data', arrCustomerRequest[i])
                            .append($('<span class="bi-trash">'))
                            .click(function (e) {
                                e.stopPropagation();
                                e.preventDefault();
                                var data = $(this).data('data');
                                DeleteSelectedRequest(data);
                            })
                        )
                    )

                if(arrCustomerRequest[i].State != 'Deleted'){
                    listAssetCustomerRequest.append($('<li class="list-group-item">')
                        .append($('<table style="width:100%">')
                            .data('data', arrCustomerRequest[i])
                            .append(status)
                            .click(function () {
                                var data = $(this).data('data');
                                CustomerRequestIndex = data.Index;
                                $('#popAssetCustomerRequest').modal('show');
                                $('#txtAssetCustomerRequest').val(data.Request).focus();
                            })
                        )
                    )
                }                
            }
        }

        function GetHistoryOfSelectedRequest(data) {
            var tbodyRequestHistory = $('#tbodyRequestHistory');
            tbodyRequestHistory.empty();

            AjaxSend({
                url: 'AssetsManagement/GetHistoryOfSelectedRequest',
                data: {
                    ID: (CStr2(data.ParentHistoryID) == '' ? data.ID : data.ParentHistoryID)
                },
                success: function (e) {
                    var vRows = e.Results.Rows;

                    if(vRows.length > 0){
                        for (var i = 0; i < vRows.length; i++){
                            tbodyRequestHistory.append($('<tr>')
                                    .data('data', vRows[i])
                                    .append($('<td>')
                                        .text(vRows[i].Request)
                                    )
                                    .append($('<td>')
                                        .text(vRows[i].ModifiedBy)
                                    )
                                    .append($('<td>')
                                        .text($.format.date(new Date(vRows[i].ModifiedDate), 'MMM dd, yyyy hh:mm a'))
                                    )
                                    .append($('<td>')
                                        .text(vRows[i].Action)
                                    )
                                    .click(function () {

                                    })
                                )
                        }

                        Bs.NestedModal('#popupCustomerRequestHistoryDetails', '#popAssetCustomerInfo', {
                            Hidden: function () {
                                $('#popAssetCustomerInfo').on('hidden.bs.modal', function () {
                                    $('#popupAssetNewCustomer').modal('show');
                                })
                            }
                        });
                    }
                }
            })
        }

        function DeleteSelectedRequest(data) {
            NewMessageBox('Are you sure you want delete the request?', 'Message', [
                {
                    Text: 'Yes', Click: function () {
                        if (data.State == 'new') {
                            arrCustomerRequest.splice(data.Index, 1);
                        } else {
                            arrCustomerRequest[data.Index].State = "Deleted";
                        }
                        DisplayRequest();
                    }
                }, { Text: 'No' }
            ])            
        }

        function SearchClient(s) {
            Bs.Loading('show', 'Searching... please wait');
            workModel.SearchClient({
                data: { Search: $.trim(s) },
                success: function (e) {                    
                    var listAMSearchClient = $('#listAMSearchClient');

                    listAMSearchClient.empty();

                    if (e.length > 0) {
                        for (var i = 0; i < e.length; i++) {
                            var table = $('<table class="table table-light table-striped table-hover">')
                                            .append($('<thead>')
                                                .append($('<tr class="custom-theme-h text-center">')
                                                    .append($('<th style="width:50px;">').text('Status'))
                                                    .append($('<th style="width:150px;">').text('Asset Name'))
                                                    .append($('<th>').text('From'))
                                                    .append($('<th>').text('To'))
                                                    .append($('<th style="width:200px;">').text('Date Created'))
                                                    .append($('<th style="width:200px;">').text('Last Updated'))
                                                    .append($('<th style="width:200px;">').text('Modified By'))
                                                )
                                            );
                            var body = $('<tbody>');
                            var row;

                            for (var t = 0; t < e[i].Trans.length; t++) {
                                var gridcolor;

                                if (e[i].Trans[t].Status == 'unavailable') {
                                    gridcolor = 'gridunavailable';
                                } else if (e[i].Trans[t].Status == 'reserved') {
                                    gridcolor = 'gridreserve';
                                } else if (e[i].Trans[t].Status == 'pastcheckin') {
                                    gridcolor = 'gridpastcheckins';
                                } else if (e[i].Trans[t].Status == 'cancelled') {
                                    gridcolor = 'gridcancelled';
                                }

                                row = $('<tr>')
                                    .data('data', e[i].Trans[t])
                                    .append($('<td>')
                                        .append($('<button data-role="none" class="btn">')
                                            .addClass(gridcolor)
                                            .append('&nbsp;')
                                        )
                                    )
                                    .append($('<td>').text(e[i].Trans[t].AssetName))
                                    .click(function (e) {
                                        assetData = $(this).data('data');
                                        mOut.ShowMenu();
                                        transID = assetData.ID;
                                        isFromSearchClient = true;
                                    })

                                if (CStr2(e[i].Trans[t].CheckedInUsedOn) != '') {
                                    row.append($('<td>').text(FormatDate(new Date(e[i].Trans[t].CheckedInUsedOn), { Format: 'MMM dd, yyyy hh:mm a' })))
                                        .append($('<td>').text(FormatDate(new Date(e[i].Trans[t].CheckedOutReturnedOn), { Format: 'MMM dd, yyyy hh:mm a' })))
                                } else {
                                    row.append($('<td>').text(FormatDate(new Date(e[i].Trans[t].ReservedFrom), { Format: 'MMM dd, yyyy hh:mm a' })))
                                        .append($('<td>').text(FormatDate(new Date(e[i].Trans[t].ReservedUntil), { Format: 'MMM dd, yyyy hh:mm a' })))
                                }

                                row.append($('<td>').text(FormatDate(new Date(e[i].Trans[t].CreatedDate), { Format: 'MMM dd, yyyy hh:mm a' })))
                                    .append($('<td>').text(FormatDate(new Date(e[i].Trans[t].LastUpdated), { Format: 'MMM dd, yyyy' })))
                                    .append($('<td>').text(e[i].Trans[t].UpdatedBy))

                                body.append(row);
                            }

                            table.append(body)

                            listAMSearchClient.append($('<li class="mb-5">')
                                    .append($('<h3>').text(e[i].CustomerName))
                                    .append(table)
                                )
                        }
                    } else {
                        listAMSearchClient.append($('<li>')
                                .append($('<h3>').text('No results found'))
                            )
                    }

                    $('#divSearchAssetMgmt').pageshow();
                    Bs.Loading('hide');
                    return;
                    arrSearchMonth = [];

                    if (e.length > 0) {
                        $('.lblAssetForSearch').show();
                        $('.navcal').hide();
                        for (var i = 0; i < e.length; i++) {
                            var month = $.format.date(new Date((e[i].CheckedInUsedOn != undefined ? e[i].CheckedInUsedOn : e[i].ReservedFrom)), "M yyyy");
                            var datesplit = month.split(' ');
                            var curEndDate = FormatDate(new Date(CInt2(datesplit[1]), CInt2(CInt2(datesplit[0]) - 1) + 1, 0), { Format: 'd' });

                            if (!($.inArray(month + ' ' + curEndDate, arrSearchMonth) >= 0)) {
                                arrSearchMonth.push(month + ' ' + curEndDate);
                            }
                        }

                        arrSearchMonth = arrSearchMonth.sort();
                        SearchNav('prev'); // default to prev
                    } else {
                        $('.lblAssetForSearch').hide();
                        $('.navcal').show();
                        WebToast('No Result(s) Found', 'top');
                    }
                }
            })
        }

        function SearchNav(nav) {
            if(nav == 'prev'){
                ctrSearchMonth -= 1;
            } else {
                ctrSearchMonth += 1;
            }

            if (ctrSearchMonth >= arrSearchMonth.length) {
                WebToast('No more result(s) found', 'top');
                ctrSearchMonth = arrSearchMonth.length - 1;
                return;
            }

            if (ctrSearchMonth < 0) {
                WebToast('No more result(s) found', 'top');
                ctrSearchMonth = 0;
                return;
            }

            var date = (arrSearchMonth[ctrSearchMonth]).split(" ");

            if(isTransferAsset){
                $('#selTransferAvailabilityMonth').val(CInt2(date[0]) - 1);
                $('#txtTransferAvailabilityYear').val(date[1]);
            } else {
                $('#selAvailabilityMonth').val(CInt2(date[0]) - 1);
                $('#txtAvailabilityYear').val(date[1]);
            }
            
            GetAsset(CInt2(date[0]) - 1, date[1], date[2]);            
        }
        
        function CheckAvailability(callback) {
            var isShowPastCheckins = $('#chbShowPastCheckins').is(':checked');
            var selAvailabilityMonth = $('#selAvailabilityMonth');
            var selTransferAvailabilityMonth = $('#selTransferAvailabilityMonth');
            var thAvailabilityDate = $('#thAvailabilityDate');
            var thTransferAvailabilityDate = $('#thTransferAvailabilityDate');
            var txtAvailabilityYear = $('#txtAvailabilityYear');
            var curDate = new Date();
            var curMonth = (isShowPastCheckins ? selAvailabilityMonth.val() : curDate.getMonth());
            var curYear = (isShowPastCheckins ? txtAvailabilityYear.val() : curDate.getFullYear());
            var curEndDate = FormatDate(new Date(CInt2(curYear), CInt2(curMonth) + 1, 0), { Format: 'd' });
            var isShowPastCheckin = $('#chbShowPastCheckins').is(':checked');

            if (isTransferAsset) {
                tbody = $('#tbodyTransferAvailability');                
                isShowPastCheckin = false; // always para dili mogawas ang past checkin sa transfer popup
            } else {
                tbody = $('#tbodyAvailability');
            }
            //Bs.Loading('show');
            if (loadassets) {
                if (isTransferAsset) {
                    thTransferAvailabilityDate.empty().append('<th rowspan="3" class="text-center bg-secondary text-white" style="min-width:180px !important; max-width:180px !important;">Asset Name</th>');
                    AppendDate('head', thTransferAvailabilityDate);
                } else {
                    thAvailabilityDate.empty().append('<th rowspan="3" class="text-center bg-secondary text-white" style="min-width:180px !important; max-width:180px !important;">Asset Name</th>');
                    AppendDate('head', thAvailabilityDate);
                }

                tbody.empty();
                tbody.append($('<tr>').append($('<td colspan="32" style="text-align:center; width:100%; position:absolute;">').text('Getting assets ... please wait')));
            }

            selAvailabilityMonth.val(curMonth);
            selTransferAvailabilityMonth.val(curMonth);
            $('#txtAvailabilityYear, #txtTransferAvailabilityYear').val(FormatDate(new Date(), { Format: 'yyyy' }));
            thAvailabilityDate.css('top', $('#divEasHeader').height() + 'px');
            
            workModel.LibraryAssets({
                data: { Month: curMonth, Year: curYear, EndDate: curEndDate, isShowPastCheckin: isShowPastCheckin },
                success: function (e) {
                    if (loadassets) {
                        tbody.empty();
                    }
                    
                    if (e.length > 0) {
                        if (loadassets) {
                            var arrOthers;
                            for (var i = 0; i < e.length; i++) {
                                if (e[i].Category == 'Other(s)') { //ipaulahi lang ang OTHERs
                                    arrOthers = e[i];
                                } else {
                                    DisplayAssets(e[i]);
                                }
                            }

                            if (arrOthers != undefined) {
                                DisplayAssets(arrOthers);
                            }
                        }                        

                        DisplayDate(curMonth, curYear, curEndDate);
                        GetAllTransactions(curMonth, curYear, curEndDate, isShowPastCheckin);
                        loadassets = false;
                        //SetDateColor(e);
                    } else {
                        tbody.append($('<div style="text-align:center;font-size: 16px; padding: 10px; background-color: #f2f2f2;">')
                                    .text('No Result(s) Found')
                                )
                    }
                    //Bs.Loading('hide');
                    if (typeof callback == 'function') {
                        callback();
                    }                    
                }
            })            
            //$('#popupCheckAvailabity').modal('show');

            function DisplayAssets(e) {
                var row;
                row = $('<tr>')
                        .append($('<td class="tdfreeze thfreeze" style="font-weight:bold; background-color:#f2f2f2;">').html('<span class="required">*</span> ' + e.Category))
                        .append($('<td colspan="32" style="background-color:#f2f2f2;">'))

                tbody.append(row)

                var newrows = document.createDocumentFragment();
                var assLen = e.Assets.length
                for (var m = 0; m < assLen; m++) {
                    //row = $('<tr assetid="' + e.Assets[m].ID + '" class="assetrow">')
                    //    .data('data', e.Assets[m])
                    //    .append($('<td>').html(' - ' + e.Assets[m].AssetName + (CBool(AssetMgmtSettingsController.IsDisplayProductNoInAssets) ? ' <i>(' + e.Assets[m].AssetNo + ')</i>' : '')))

                    //AppendDate('body', row, e.Assets[m])
                    //tbody.append(row)
                    //continue
                    var tr = document.createElement("tr");
                    var td = document.createElement("td");
                    tr.className = "assetrow"
                    tr.setAttribute('assetid', e.Assets[m].ID);

                    td.textContent = ' - ' + e.Assets[m].AssetName + (CBool(AssetMgmtSettingsController.IsDisplayProductNoInAssets) ? ' <i>(' + e.Assets[m].AssetNo + ')</i>' : '');
                    tr.appendChild(td);

                    var year = $.trim($('#txtAvailabilityYear').val());
                    var month = $('#selAvailabilityMonth').val();
                    
                    e.Assets[m].Status = 'available';

                    for (var c = 0; c < 31; c++) {
                        e.Assets[m].GridDate = FormatDate(new Date(year, month, c), { Format: 'MMM dd, yyyy' });
                        td = document.createElement("td");
                        td.className = "griddate"
                        td.setAttribute('date', (c + 1));
                        td.style.minWidth = '15px';
                        td.style.position = 'relative';
                        $(td).data('data', e.Assets[m])
                        //td.dataset.data = JSON.stringify(e.Assets[m]);
                        td.onclick = function () {
                            var data = $(this).data('data');
                            year = $.trim($('#txtAvailabilityYear').val());
                            month = $('#selAvailabilityMonth').val();
                            data.GridDate = FormatDate(new Date(year, month, $(this).attr('date')), { Format: 'MMM dd, yyyy' });
                            data.Status = 'available';
                            //console.log(data);

                            IsTransfer();

                            if (isTransferAsset) {
                                selectedAssetData = data;
                                if ($(this).hasClass('finishday')) {
                                    NewMessageBox('You\'re not allowed to transfer checked-in/ used asset in previous or future dates.', 'Message');
                                    return
                                }

                                if (assetData.Status == 'reserved') {
                                    dateFrom = selectedAssetData.GridDate + ' ' + $.format.date(new Date(), 'hh:mm a');
                                    assetData.ReservedFrom = dateFrom; // change this based on select date to transfer
                                } else {
                                    assetData.TransferCheckedInDate = selectedAssetData.GridDate + ' ' + $.format.date(new Date(), 'hh:mm a');
                                }
                                mOut.TransferMenu();
                            } else {
                                assetData = $(this).data('data');
                                isTransferAsset = false;
                                var defaultcheckoutdate = FormatDate(new Date(new Date(assetData.GridDate).setDate(new Date(assetData.GridDate).getDate() + 1)), { Format: 'MMM dd, yyyy' });
                                if (!(new Date(assetData.GridDate) < new Date($.format.date(new Date(), 'MMM dd, yyyy'))) || CBool(AssetMgmtSettingsController.IsAllowOutdatedReservation)) { // Do not show menu option for prev dates when transfer an assets
                                    dateFrom = assetData.GridDate + ' ' + $.format.date(new Date(), 'hh:mm a');
                                    dateTo = defaultcheckoutdate + ' ' + AssetMgmtSettingsController.DefaultCheckOutTime.Time;
                                    mOut.ShowMenu(false);
                                }
                            }
                        }
                        td.onmouseenter = function () {
                            $((isTransferAsset ? '#thTransferAvailabilityDate' : '#thAvailabilityDate') + ' th').eq(($(this).index() + 1) - 1).addClass('gridhover');
                            $(this).parent().find('td:eq(0)').addClass('gridhover');
                            $(this).addClass('gridhover');
                        }
                        td.onmouseout = function () {
                            $((isTransferAsset ? '#thTransferAvailabilityDate' : '#thAvailabilityDate') + ' th').eq(($(this).index() + 1) - 1).removeClass('gridhover');
                            $(this).parent().find('td:eq(0)').removeClass('gridhover');
                            $(this).removeClass('gridhover')
                        }
                        tr.appendChild(td);
                    }

                    $(tr).data('data', e.Assets[m])
                    newrows.appendChild(tr);
                }

                document.getElementById(isTransferAsset ? 'tbodyTransferAvailability' : 'tbodyAvailability').appendChild(newrows)
            }
        }

        function GetAllTransactions(curMonth, curYear, curEndDate, isShowPastCheckin) {
            Bs.Loading('show');
            setTimeout(function () {
                AjaxSend({
                    url: 'AssetsManagement/GetAllTransactions',
                    data: { Month: curMonth, Year: curYear, EndDate: curEndDate, isShowPastCheckin: isShowPastCheckin, PreviousMonth: $.format.date(new Date(new Date().setMonth(new Date((CInt2(curMonth) + 1) + ' 1 ' + curYear).getMonth() - 1)), 'yyyyMM') },
                    success: function (e) {
                        if (e.Results != undefined) {
                            SetDateColor(e.Results.Rows);
                        }
                    }
                })
            }, 200)            
        }

        function GetAsset(curMonth, curYear, curEndDate) {
            Bs.Loading('show', 'Getting assets ... please wait');
            workModel.LibraryAssets({
                data: { Month: curMonth, Year: curYear, EndDate: curEndDate, isShowPastCheckin: $('#chbShowPastCheckins').is(':checked') },
                success: function (e) {
                    DisplayDate(curMonth, curYear, curEndDate);
                    GetAllTransactions(curMonth, curYear, curEndDate, $('#chbShowPastCheckins').is(':checked'));
                    //SetDateColor(e);
                    Bs.Loading('hide');
                }
            })
        }

        function DisplayDate(month, year, curEndDate) {
            //var curEndDate = FormatDate(new Date(CInt2(year), CInt2(month) + 1, 0), { Format: 'd' });
            
            if(isTransferAsset){
                $('#txtTransferAvailabilityYear').val(FormatDate(new Date(year, month, 1), { Format: 'yyyy' }));
                $('.griddate').show();
                $('#tbodyTransferAvailability td').removeClass('gridreserve gridunavailable').find('div').remove();
                $('#thTransferAvailabilityDate th, #tbodyTransferAvailability td').removeClass('finishday curDay gridhover');

                //for (i = CInt2(month) ; i <= CInt2(month) ; i++) { // disable prev dates
                //    $('#tbodyTransferAvailability td[date="' + i + '"]').hide();
                //}

                if(assetData.Status == 'unavailable'){
                    for (i = CInt2(new Date().getDate()) ; 31 >= i ; i++) { // disable future dates when checked-in
                        if (CInt2(new Date().getDate()) != i) {
                            $('#tbodyTransferAvailability td[date="' + i + '"]').addClass('finishday');
                        }                        
                    }
                }
            } else {
                $('#thLblMonth').text(FormatDate(new Date(year, month, 1), { Format: 'MMMM yyyy' }));
                $('#txtAvailabilityYear').val(FormatDate(new Date(year, month, 1), { Format: 'yyyy' }));
                $('.griddate').show();
                $('#tbodyAvailability td').removeClass('gridreserve gridunavailable').find('div').remove();
                $('#thAvailabilityDate th, #tbodyAvailability td').removeClass('finishday curDay gridhover');
            }            

            for (i = CInt2(curEndDate) + 1 ; i <= 31 ; i++) {
                $('[date="' + i + '"]').hide();
            }

            mOut.CheckOudatedReservationDate(year, month);
            UpdateGridDateData();
        }

        function AppendDate(appendTo, row, data) {
            var to, cols;
            var year = $.trim($('#txtAvailabilityYear').val());
            var month = $('#selAvailabilityMonth').val();

            for (var i = 0; i < 31 ; i++) {
                var date = (i + 1);                
                if (appendTo == 'head') {
                    row.append($('<th class="griddate custom-theme-c text-center" date="' + date + '" style="min-width:15px;">').text(date))
                } else {
                    var data = JSON.parse(JSON.stringify(data));
                    data.GridDate = FormatDate(new Date(year, month, date), { Format: 'MMM dd, yyyy' });
                    data.Status = 'available';
                    row.append($('<td class="griddate" date="' + date + '" style="min-width:15px;">')
                            .css('position', 'relative')
                            .removeData('data')
                            .data('data', data)
                            .click(function (e) {
                                IsTransfer();

                                if (isTransferAsset) {
                                    selectedAssetData = JSON.parse(JSON.stringify($(this).data('data')));
                                    if($(this).hasClass('finishday')){
                                        NewMessageBox('You\'re not allowed to transfer checked-in/ used asset in previous or future dates.', 'Message');
                                        return
                                    }

                                    if(assetData.Status == 'reserved'){
                                        dateFrom = selectedAssetData.GridDate + ' ' + $.format.date(new Date(), 'hh:mm a');
                                        assetData.ReservedFrom = dateFrom; // change this based on select date to transfer
                                    } else {
                                        assetData.TransferCheckedInDate = selectedAssetData.GridDate + ' ' + $.format.date(new Date(), 'hh:mm a');
                                    }
                                    mOut.TransferMenu();
                                } else {
                                    assetData = JSON.parse(JSON.stringify($(this).data('data')));
                                    isTransferAsset = false;
                                    var defaultcheckoutdate = FormatDate(new Date(new Date(assetData.GridDate).setDate(new Date(assetData.GridDate).getDate() + 1)), { Format: 'MMM dd, yyyy' });
                                    if (!(new Date(assetData.GridDate) < new Date($.format.date(new Date(), 'MMM dd, yyyy'))) || CBool(AssetMgmtSettingsController.IsAllowOutdatedReservation)) { // Do not show menu option for prev dates when transfer an assets
                                        dateFrom = assetData.GridDate + ' ' + $.format.date(new Date(), 'hh:mm a');
                                        dateTo = defaultcheckoutdate + ' ' + AssetMgmtSettingsController.DefaultCheckOutTime.Time;
                                        mOut.ShowMenu(false);
                                    }
                                }                                
                            })
                            .hover(function () {
                                $('#thAvailabilityDate th').eq(($(this).index() + 1) - 1).addClass('gridhover');
                                $(this).parent().find('td:eq(0)').addClass('gridhover');
                                $(this).addClass('gridhover');
                            }, function () {
                                $('#thAvailabilityDate th').eq(($(this).index() + 1) - 1).removeClass('gridhover');
                                $(this).parent().find('td:eq(0)').removeClass('gridhover');
                                $(this).removeClass('gridhover')
                            })
                        )
                }
            }
        }

        function UpdateGridDateData(data) {
            var year = $.trim((isTransferAsset ? $('#txtTransferAvailabilityYear').val() : $('#txtAvailabilityYear').val()));
            var month = (isTransferAsset ? $('#selTransferAvailabilityMonth').val() : $('#selAvailabilityMonth').val());
            var curEndDate = FormatDate(new Date(year, month + 1, 0), { Format: 'd' });

            for (var i = 0; i < curEndDate; i++) {
                var date = i + 1;
                var newdate = FormatDate(new Date(year, month, date), { Format: 'MMM dd, yyyy' });
                //$('td[date="'+ date +'"]').each(function () {
                //    $(this).data('data').GridDate = newdate;
                //})
            }
        }

        function SetDateColor(e) {
            var tbodyAvailability = (isTransferAsset ? $('#tbodyTransferAvailability') : $('#tbodyAvailability'));
            var curMonth = CInt2((isTransferAsset ? $('#selTransferAvailabilityMonth').val() : $('#selAvailabilityMonth').val()));
            var curYear = CInt2((isTransferAsset ? $('#txtTransferAvailabilityYear').val() : $('#txtAvailabilityYear').val()));
            var transLen = e.length;
            tbody.find('.gridcontainer').remove();

            for (var m = 0; m < transLen; m++) {
                var row = tbodyAvailability.find('tr[assetid="' + e[m].AssetID + '"]');

                if (row.length == 0)
                    continue;

                var assdata = row.data('data');
                var trans = e[m], color, tempdate, gridwidth = 0, startdate, enddate, startmonth, startday, startyear, endmonth, endday, endyear;
                var forcleaning = (!CBool(trans.IsCancelled) && CBool(trans.IsDone) && !CBool(trans.IsDoneCleaning) ? true : false);
                var outoforder = CBool(trans.IsOutOfOrder);

                if (!CBool(trans.IsOutOfOrder)) {
                    if (CStr2(trans.CheckedInUsedOn) != '') {
                        color = 'gridunavailable';
                        startdate = new Date(trans.CheckedInUsedOn);
                        startmonth = startdate.getMonth();
                        startday = startdate.getDate();
                        startyear = startdate.getFullYear();
                        enddate = new Date(trans.CheckedOutReturnedOn)
                        endmonth = enddate.getMonth();
                        endday = enddate.getDate();
                        endyear = enddate.getFullYear();
                    } else {
                        color = 'gridreserve';
                        startdate = new Date(trans.ReservedFrom);
                        startmonth = startdate.getMonth();
                        startday = startdate.getDate();
                        startyear = startdate.getFullYear();
                        enddate = new Date(trans.ReservedUntil)
                        endmonth = enddate.getMonth();
                        endday = enddate.getDate();
                        endyear = enddate.getFullYear();
                    }

                    trans.Status = (CStr2(trans.CheckedInUsedOn) != '' ? 'unavailable' : 'reserved');
                    trans.StartDate = FormatDate(startdate, { Format: 'MMM dd, yyyy' });

                    if (CBool(trans.IsDone) && !CBool(trans.IsCancelled) && $('#chbShowPastCheckins').is(':checked')) {
                        forcleaning = false;
                        trans.Status = 'pastcheckin'
                        trans.IsPastCheckins = true;
                        color = 'gridpastcheckins';
                        startdate = new Date(trans.CheckedInUsedOn);
                        startmonth = startdate.getMonth();
                        startday = startdate.getDate();
                        startyear = startdate.getFullYear();
                        enddate = new Date((CStr2(trans.TransferDate) != '' ? trans.CheckedOutReturnedOn : trans.ActualCheckedOutReturnedOn));
                        endmonth = enddate.getMonth();
                        endday = enddate.getDate();
                        endyear = enddate.getFullYear();

                        if (CStr2(trans.TransferDate) != '') {
                            color = 'gridpastcheckinstransfer';
                        }
                    }
                } else {
                    color = 'gridoutoforder';
                    startdate = new Date(trans.OutOfOrderFrom);
                    startmonth = startdate.getMonth();
                    startday = startdate.getDate();
                    startyear = startdate.getFullYear();
                    enddate = new Date(trans.OutOfOrderTo)
                    endmonth = enddate.getMonth();
                    endday = enddate.getDate() + 1; //para maapil jud ang TO date
                    endyear = enddate.getFullYear();
                }

                trans.AssetName = assdata.AssetName;
                trans.AssetNo = assdata.AssetNo;
                trans.AssetType = assdata.AssetType;
                trans.ProductID = assdata.ProductID;
                trans.CapacityUnit = assdata.CapacityUnit;

                if (curMonth < endmonth) { // nitabok ug months
                    endday = new Date(new Date(curYear, curMonth).getFullYear(), new Date(curYear, curMonth).getMonth() + 1, 0).getDate();
                } else if ((startday < endday || startday > endday) && startmonth < endmonth) { // nitabok ug year
                    startday = 1;
                    endday = endday - 1;
                } else {
                    endday = endday - 1;
                }

                if (trans.Status == 'pastcheckin') {
                    if (startday > endday) {
                        endday += 1;
                    }
                }

                if (startyear < endyear) {
                    if (endyear > curYear) {
                        endday = new Date(new Date(curYear, curMonth).getFullYear(), new Date(curYear, curMonth).getMonth() + 1, 0).getDate();

                        if (curMonth != startmonth) {
                            startday = 1;
                        }
                    } else if (endyear == curYear) {
                        startday = 1;
                    }
                } else {
                    if (curMonth > startmonth) startday = 1;
                }                

                tempdate = startday;

                if (forcleaning) {
                    endday = new Date().getDate() + 1;
                }

                for (startday; startday <= endday; startday++) {
                    var th = (isTransferAsset ? $('#thTransferAvailabilityDate th') : $('#thAvailabilityDate th'))
                    var vRow = row.find('td[date="' + (forcleaning ? new Date().getDate() : startday) + '"]');
                    var cell = vRow.find('> div');
                    var thWidth = th.eq((vRow.index() + 1) - 1).innerWidth() - 1;
                    var attr = {
                        'data-toggle': 'tooltip',
                        'data-placement': 'top',
                        'title': trans.CustomerName,
                        'transid': trans.ID,
                        'cusctr': 1
                    }
                    trans.GridDate = FormatDate(new Date(curYear, curMonth, startday), { Format: 'MMM dd, yyyy' });
                    assdata.GridDate = trans.GridDate;

                    vRow.css('padding', '0px')

                    if (cell.length >= 1 && trans.Status == 'pastcheckin') {
                        var curData = cell.data('data');
                        var curCtr = CInt2(cell.attr('cusctr'));

                        if (curCtr > 1) {
                            curData.push(trans)
                        } else if (curCtr == 1) {
                            var objdata = [];
                            objdata.push(curData);
                            objdata.push(trans);
                            cell.data('data', objdata);
                        }

                        cell.attr({
                            'title': 'Multiple Check-ins',
                            'cusctr': curCtr + 1
                        })

                        cell.text(curCtr + 1);
                        continue;
                    }

                    if (startday < 10) {
                        gridwidth += vRow.innerWidth();
                    } else {
                        gridwidth += vRow.innerWidth();
                    }

                    if (forcleaning) {
                        gridwidth = vRow.innerWidth();
                    }

                    if (startday == endday) {
                        if (isTransferAsset && assetData.ID == trans.ID) {
                            color = 'gridtransfer';
                        }

                        if (forcleaning) {
                            color = 'gridforcleaning';
                            attr = {};
                        }

                        if (outoforder) {
                            color = 'gridoutoforder';
                            attr = {};
                        }

                        row.find('td[date="' + (forcleaning ? new Date().getDate() : tempdate) + '"]')
                            .append($('<div class="gridcontainer">')
                                .data('data', trans)
                                .data('assetdata', assdata)
                                .css({ 'width': gridwidth + 'px' })
                                .attr(attr)
                                .addClass('text-center ' + color)
                                .text(forcleaning || outoforder ? '' : trans.CustomerName)
                                .click(function (e) {
                                    e.stopPropagation();
                                    e.preventDefault();
                                    var dis = $(this);
                                    var data = dis.data('data');
                                    var assetInfoData = dis.data('assetdata');

                                    function GetDetails(vData) {
                                        AjaxSend({
                                            url: 'AssetsManagement/GetAssetDetails',
                                            data: {
                                                TransID: vData.ID,
                                                IsOutOfOrder: vData.IsOutOfOrder
                                            },
                                            success: function (e) {
                                                e.Details[0].AssetName = assetInfoData.AssetName;
                                                e.Details[0].AssetNo = assetInfoData.AssetNo;
                                                e.Details[0].AssetType = assetInfoData.AssetType;
                                                e.Details[0].ProductID = assetInfoData.ProductID;
                                                e.Details[0].CapacityUnit = assetInfoData.CapacityUnit;
                                                e.Details[0].Status = vData.Status;
                                                e.Details[0].StartDate = vData.StartDate;
                                                e.Details[0].IsPastCheckins = CBool(vData.IsPastCheckins);

                                                IsTransfer();
                                                assetData = JSON.parse(JSON.stringify(e.Details[0]));

                                                if (dis.hasClass('gridforcleaning')) {
                                                    NewMessageBox('Is <b>' + vData.AssetName + '</b> is ready to use?', 'Message', [
                                                        {
                                                            Text: 'Yes', Click: function () {
                                                                workModel.Save({
                                                                    data: {
                                                                        TransID: vData.ID,
                                                                        AssetID: vData.AssetID,
                                                                        Action: 'donecleaning'
                                                                    },
                                                                    success: function (e) {
                                                                        WebToast(vData.AssetName + ' is ready to use', 'top');
                                                                        CheckAvailability(function () {
                                                                            AssetMgmtDashboardController.GetDashboardData();
                                                                        });
                                                                        mOut.ClearFieldsNewCustomer();
                                                                    }
                                                                })
                                                            }
                                                        }, { Text: 'No' }
                                                    ])
                                                    return;
                                                }

                                                if (dis.hasClass('gridoutoforder') && !isTransferAsset) {
                                                    PopupButtonList('Asset Name: <b>' + vData.AssetName + '</b>', 'Out of Order', [
                                                        {
                                                            Text: 'Edit Details', Click: function () {
                                                                NewMessageBoxClose();
                                                                $('#popAssetOutOfOrder').modal('show');
                                                                $('#lblOutOfOrderAssetName').text(vData.AssetName);
                                                                $('#txtOOODateFrom').val($.format.date(new Date(vData.OutOfOrderFrom), 'yyyy-MM-dd'));
                                                                $('#txtOOODateTo').val($.format.date(new Date(vData.OutOfOrderTo), 'yyyy-MM-dd'));
                                                                $('#txtOOOReason').val(vData.OutOfOrderReason);
                                                            }
                                                        },
                                                        {
                                                            Text: 'Ready to Use', Click: function () {
                                                                NewMessageBox('Are you sure that <b>' + vData.AssetName + '</b> is ready to use?', 'Message', [
                                                                    {
                                                                        Text: 'Yes', Click: function () {
                                                                            workModel.Save({
                                                                                data: {
                                                                                    TransID: vData.TransID,
                                                                                    AssetID: vData.AssetID,
                                                                                    Action: 'outoforder',
                                                                                    DateFrom: vData.OutOfOrderFrom,
                                                                                    DateTo: vData.OutOfOrderTo,
                                                                                    OutOfOrderReason: CStr2(vData.OutOfOrderReason),
                                                                                    IsOutOfOrder: false
                                                                                },
                                                                                success: function (e) {
                                                                                    WebToast(vData.AssetName + ' is ready to use', 'top');
                                                                                    CheckAvailability(function () {
                                                                                        AssetMgmtDashboardController.GetDashboardData();
                                                                                    });
                                                                                    mOut.ClearFieldsNewCustomer();
                                                                                }
                                                                            })
                                                                        }
                                                                    }, { Text: 'No' }
                                                                ])
                                                            }
                                                        }
                                                    ], { AutoDismiss: true });
                                                    return;
                                                }

                                                if (isTransferAsset) {
                                                    if (!dis.hasClass('gridtransfer') &&
                                                        !dis.hasClass('gridunavailable') &&
                                                        !dis.hasClass('gridreserve') &&
                                                        !dis.hasClass('gridforcleaning') &&
                                                        !dis.hasClass('gridoutoforder')) {
                                                        selectedAssetData = JSON.parse(JSON.stringify(data));
                                                        mOut.TransferMenu();
                                                    }
                                                } else {
                                                    mOut.ShowMenu();
                                                }
                                                transID = assetData.ID;
                                                isFromSearchClient = false;
                                            }
                                        })
                                    }

                                    if (CInt2(dis.attr('cusctr')) > 1) {
                                        var menuOption = [];
                                        for (var i = 0; i < data.length; i++) {
                                            //var arrCus = [];
                                            //for (var c = 0; c < data[i].Customer.length; c++) {
                                            //    arrCus.push($.trim(data[i].Customer[c].CustomerName));
                                            //}
                                            menuOption.push({
                                                data: data[i],
                                                Text: data[i].CustomerName + '<br><span style="font-size: 12px; color:#696969; font-weight: normal; text-shadow: none;">' +
                                                    'Checked-in: ' + $.format.date(new Date(data[i].CheckedInUsedOn), 'MM/dd/yyyy hh:mm a') + '<br>' +
                                                    (CBool(data[i].IsDone) ? 'Checked-out: ' : 'Check-out: ') + $.format.date(new Date(data[i].ActualCheckedOutReturnedOn || data[i].CheckedOutReturnedOn), 'MM/dd/yyyy hh:mm a') +
                                                    '</span>',//arrCus.join(', '),
                                                Click: function (e) {
                                                    assetData = e;
                                                    GetDetails(assetData);
                                                },
                                                attr: {
                                                    style: 'border-left: 5px solid #' + (data[i].Status == 'unavailable' ? 'dc3545' : '795548')
                                                }
                                            })
                                        }
                                        PopupButtonList('<b>Selected Date:</b> ' + assetInfoData.GridDate +
                                            '<br><br> <b>Asset Name:</b> ' + assetInfoData.AssetName, 'Multiple Check-ins', menuOption,
                                            { AutoDismiss: true });
                                        return;
                                    } else {
                                        GetDetails(data);
                                    }
                                })
                            )

                        if (CStr2(trans.TransferDate) != '' && (!CBool(trans.IsDone) && !trans.IsDoneCleaning)) {
                            row.find('td[date="' + (startday + 1) + '"]')
                                .css('padding', '0px')
                                .append($('<div class="gridcontainer">')
                                .data('data', trans)
                                .css({ 'width': row.find('td[date="' + (startday + 1) + '"]').innerWidth() + 'px' })
                                .addClass('text-center gridforcleaning')
                                .click(function (e) {
                                    e.stopPropagation();
                                    e.preventDefault();
                                    var data = $(this).data('data');
                                    NewMessageBox('Is <b>' + data.AssetName + '</b> is ready to use?', 'Message', [
                                        {
                                            Text: 'Yes', Click: function () {
                                                workModel.Save({
                                                    data: {
                                                        TransID: data.ID,
                                                        AssetID: data.AssetID,
                                                        Action: 'donecleaning'
                                                    },
                                                    success: function (e) {
                                                        WebToast(data.AssetName + ' is ready to use', 'top');
                                                        CheckAvailability(function () {
                                                            AssetMgmtDashboardController.GetDashboardData();
                                                        });
                                                        mOut.ClearFieldsNewCustomer();
                                                    }
                                                })
                                            }
                                        }, { Text: 'No' }
                                    ])
                                })
                            )
                        }
                    }
                }
            }

            Bs.Loading('hide');
            return
            for (var i = 0; i < e.length; i++) {                
                for (var x = 0; x < e[i].Assets.length; x++){
                    var assdata = e[i].Assets[x];
                    var row = tbodyAvailability.find('tr[assetid="' + assdata.ID + '"]');
                    for (var m = 0; m < assdata.Trans.length; m++) {
                        var trans = assdata.Trans[m], color, tempdate, gridwidth = 0, startdate, enddate, startmonth, startday, startyear, endmonth, endday, endyear;
                        var forcleaning = (!CBool(trans.IsCancelled) && CBool(trans.IsDone) && !CBool(trans.IsDoneCleaning) ? true : false);
                        var outoforder = CBool(trans.IsOutOfOrder);

                        if (!CBool(trans.IsOutOfOrder)) {
                            if (CStr2(trans.CheckedInUsedOn) != '') {
                                color = 'gridunavailable';
                                startdate = new Date(trans.CheckedInUsedOn);
                                startmonth = startdate.getMonth();
                                startday = startdate.getDate();
                                startyear = startdate.getFullYear();
                                enddate = new Date(trans.CheckedOutReturnedOn)
                                endmonth = enddate.getMonth();
                                endday = enddate.getDate();
                                endyear = enddate.getFullYear();
                            } else {
                                color = 'gridreserve';
                                startdate = new Date(trans.ReservedFrom);
                                startmonth = startdate.getMonth();
                                startday = startdate.getDate();
                                startyear = startdate.getFullYear();
                                enddate = new Date(trans.ReservedUntil)
                                endmonth = enddate.getMonth();
                                endday = enddate.getDate();
                                endyear = enddate.getFullYear();
                            }                            

                            trans.Status = (CStr2(trans.CheckedInUsedOn) != '' ? 'unavailable' : 'reserved');
                            trans.StartDate = FormatDate(startdate, { Format: 'MMM dd, yyyy' });

                            if (CBool(trans.IsDone) && !CBool(trans.IsCancelled) && $('#chbShowPastCheckins').is(':checked')) {
                                forcleaning = false;
                                trans.Status = 'pastcheckin'
                                trans.IsPastCheckins = true;
                                color = 'gridpastcheckins';
                                startdate = new Date(trans.CheckedInUsedOn);
                                startmonth = startdate.getMonth();
                                startday = startdate.getDate();
                                startyear = startdate.getFullYear();
                                enddate = new Date(trans.ActualCheckedOutReturnedOn);
                                endmonth = enddate.getMonth();
                                endday = enddate.getDate();
                                endyear = enddate.getFullYear();
                            }
                        } else {
                            color = 'gridoutoforder';
                            startdate = new Date(trans.OutOfOrderFrom);
                            startmonth = startdate.getMonth();
                            startday = startdate.getDate();
                            startyear = startdate.getFullYear();
                            enddate = new Date(trans.OutOfOrderTo)
                            endmonth = enddate.getMonth();
                            endday = enddate.getDate() + 1; //para maapil jud ang TO date
                            endyear = enddate.getFullYear();
                        }

                        trans.AssetName = assdata.AssetName;
                        trans.AssetNo = assdata.AssetNo;
                        trans.AssetType = assdata.AssetType;
                        trans.ProductID = assdata.ProductID;
                        trans.CapacityUnit = assdata.CapacityUnit;

                        if (curMonth < endmonth) { // nitabok ug months
                            endday = new Date(new Date(curYear, curMonth).getFullYear(), new Date(curYear, curMonth).getMonth() + 1, 0).getDate();
                        } else if ((startday < endday || startday > endday) && startmonth < endmonth) { // nitabok ug year
                            startday = 1;
                            endday = endday - 1;
                        } else {
                            endday = endday - 1;
                        }

                        if (trans.Status == 'pastcheckin') {
                            if (startday > endday) {
                                endday += 1;
                            }
                        }

                        if (startyear < endyear) {
                            if (endyear > curYear) {
                                endday = new Date(new Date(curYear, curMonth).getFullYear(), new Date(curYear, curMonth).getMonth() + 1, 0).getDate();

                                if (curMonth != startmonth) {
                                    startday = 1;
                                }
                            } else if (endyear == curYear) {
                                startday = 1;
                            }
                        }

                        tempdate = startday;

                        if (forcleaning) {
                            endday = new Date().getDate() + 1;
                        }

                        for (startday; startday <= endday; startday++) {
                            var th = (isTransferAsset ? $('#thTransferAvailabilityDate th') : $('#thAvailabilityDate th'))
                            var vRow = row.find('td[date="' + (forcleaning ? new Date().getDate() : startday) + '"]');
                            var cell = vRow.find('> div');
                            var thWidth = th.eq((vRow.index() + 1) - 1).innerWidth() - 1;
                            var attr = {
                                'data-toggle': 'tooltip',
                                'data-placement': 'top',
                                'title': trans.CustomerName,
                                'transid': trans.ID,
                                'cusctr': 1
                            }
                            trans.GridDate = FormatDate(new Date(curYear, curMonth, startday), { Format: 'MMM dd, yyyy' });
                            assdata.GridDate = trans.GridDate;

                            vRow.css('padding', '0px')

                            if (cell.length >= 1 && trans.Status == 'pastcheckin') {
                                var curData = cell.data('data');
                                var curCtr = CInt2(cell.attr('cusctr'));                                

                                if (curCtr > 1) {
                                    curData.push(trans)                                    
                                } else if(curCtr == 1){
                                    var objdata = [];
                                    objdata.push(curData);
                                    objdata.push(trans);
                                    cell.data('data', objdata);
                                }

                                cell.attr({
                                    'title': 'Multiple Check-ins',
                                    'cusctr': curCtr + 1
                                })

                                cell.text(curCtr + 1);
                                continue;
                            }

                            if (startday < 10) {
                                gridwidth += vRow.innerWidth();
                            } else {
                                gridwidth += vRow.innerWidth();
                            }

                            if (forcleaning) {
                                gridwidth = vRow.innerWidth();
                            }

                            if (startday == endday) {
                                if (isTransferAsset && assetData.ID == trans.ID) {
                                    color = 'gridtransfer';
                                }

                                if (forcleaning) {
                                    color = 'gridforcleaning';
                                    attr = {};
                                }

                                if (outoforder) {
                                    color = 'gridoutoforder';
                                    attr = {};
                                }

                                row.find('td[date="' + (forcleaning ? new Date().getDate() : tempdate) + '"]')
                                    .append($('<div class="gridcontainer">')
                                        .data('data', trans)
                                        .data('assetdata', assdata)
                                        .css({ 'width': gridwidth + 'px' })
                                        .attr(attr)
                                        .addClass('text-center ' + color)
                                        .text(forcleaning || outoforder ? '' : trans.CustomerName)
                                        .click(function (e) {
                                            e.stopPropagation();
                                            e.preventDefault();
                                            var dis = $(this);
                                            var data = dis.data('data');
                                            var assetInfoData = dis.data('assetdata');
                                            AjaxSend({
                                                url: 'AssetsManagement/GetAssetDetails',
                                                data: {
                                                    TransID: data.ID,
                                                    IsOutOfOrder: data.IsOutOfOrder
                                                }, success: function (e) {
                                                    e.Details[0].AssetName = assetInfoData.AssetName;
                                                    e.Details[0].AssetNo = assetInfoData.AssetNo;
                                                    e.Details[0].AssetType = assetInfoData.AssetType;
                                                    e.Details[0].ProductID = assetInfoData.ProductID;
                                                    e.Details[0].CapacityUnit = assetInfoData.CapacityUnit;
                                                    e.Details[0].Status = data.Status;
                                                    e.Details[0].StartDate = data.StartDate;
                                                    e.Details[0].IsPastCheckins = CBool(data.IsPastCheckins);
                                                    
                                                    IsTransfer();                                                    
                                                    assetData = JSON.parse(JSON.stringify(e.Details[0]));

                                                    if (CInt2(dis.attr('cusctr')) > 1) {
                                                        var menuOption = [];
                                                        for (var i = 0; i < data.length; i++) {
                                                            var arrCus = [];
                                                            for (var c = 0; c < data[i].Customer.length; c++) {
                                                                arrCus.push($.trim(data[i].Customer[c].CustomerName));
                                                            }
                                                            menuOption.push({
                                                                data: data[i],
                                                                Text: '<span style="font-size: 12px; color:#696969; font-weight: normal; text-shadow: none;">' +
                                                                    'Checked-in: ' + $.format.date(new Date(data[i].CheckedInUsedOn), 'MM/dd/yyyy hh:mm a') + '<br>' +
                                                                    (CBool(data[i].IsDone) ? 'Checked-out: ' : 'Check-out: ') + $.format.date(new Date(data[i].ActualCheckedOutReturnedOn || data[i].CheckedOutReturnedOn), 'MM/dd/yyyy hh:mm a') +
                                                                    '</span><br><br>' + arrCus.join(', '),
                                                                Click: function (e) {
                                                                    assetData = e;
                                                                    mOut.ShowMenu();
                                                                    transID = assetData.ID;
                                                                    isFromSearchClient = false;
                                                                },
                                                                attr: {
                                                                    style: 'border-left: 3px solid #' + (data[i].Status == 'unavailable' ? 'ff6961' : '795548')
                                                                }
                                                            })
                                                        }
                                                        PopupButtonList('<b>Selected Date:</b> ' + assetInfoData.GridDate +
                                                                '<br><br> <b>Asset Name:</b> ' + assetInfoData.AssetName, 'Multiple Check-ins', menuOption,
                                                                { AutoDismiss: true });
                                                        return;
                                                    }

                                                    if (dis.hasClass('gridforcleaning')) {
                                                        NewMessageBox('Is <b>' + data.AssetName + '</b> is ready to use?', 'Message', [
                                                            {
                                                                Text: 'Yes', Click: function () {
                                                                    workModel.Save({
                                                                        data: {
                                                                            TransID: data.ID,
                                                                            AssetID: data.AssetID,
                                                                            Action: 'donecleaning'
                                                                        },
                                                                        success: function (e) {
                                                                            WebToast(data.AssetName + ' is ready to use', 'top');
                                                                            CheckAvailability(function () {
                                                                                AssetMgmtDashboardController.GetDashboardData();
                                                                            });
                                                                            mOut.ClearFieldsNewCustomer();
                                                                        }
                                                                    })
                                                                }
                                                            }, { Text: 'No' }
                                                        ])
                                                        return;
                                                    }

                                                    if (dis.hasClass('gridoutoforder') && !isTransferAsset) {
                                                        PopupButtonList('Asset Name: <b>' + data.AssetName + '</b>', 'Out of Order', [
                                                            {
                                                                Text: 'Edit Details', Click: function () {
                                                                    NewMessageBoxClose();
                                                                    $('#popAssetOutOfOrder').modal('show');
                                                                    $('#lblOutOfOrderAssetName').text(data.AssetName);
                                                                    $('#txtOOODateFrom').val($.format.date(new Date(data.OutOfOrderFrom), 'yyyy-MM-dd'));
                                                                    $('#txtOOODateTo').val($.format.date(new Date(data.OutOfOrderTo), 'yyyy-MM-dd'));
                                                                    $('#txtOOOReason').val(data.OutOfOrderReason);
                                                                }
                                                            },
                                                            {
                                                                Text: 'Ready to Use', Click: function () {
                                                                    NewMessageBox('Are you sure that <b>' + data.AssetName + '</b> is ready to use?', 'Message', [
                                                            {
                                                                Text: 'Yes', Click: function () {
                                                                    workModel.Save({
                                                                        data: {
                                                                            TransID: data.TransID,
                                                                            AssetID: data.AssetID,
                                                                            Action: 'outoforder',
                                                                            DateFrom: data.OutOfOrderFrom,
                                                                            DateTo: data.OutOfOrderTo,
                                                                            OutOfOrderReason: data.OutOfOrderReason,
                                                                            IsOutOfOrder: false
                                                                        },
                                                                        success: function (e) {
                                                                            WebToast(data.AssetName + ' is ready to use', 'top');
                                                                            CheckAvailability(function () {
                                                                                AssetMgmtDashboardController.GetDashboardData();
                                                                            });
                                                                            mOut.ClearFieldsNewCustomer();
                                                                        }
                                                                    })
                                                                }
                                                            }, { Text: 'No' }
                                                                    ])
                                                                }
                                                            }
                                                        ], { AutoDismiss: true });
                                                        return;
                                                    }

                                                    if (isTransferAsset) {
                                                        if (!dis.hasClass('gridtransfer') &&
                                                            !dis.hasClass('gridunavailable') &&
                                                            !dis.hasClass('gridreserve') &&
                                                            !dis.hasClass('gridforcleaning') &&
                                                            !dis.hasClass('gridoutoforder')) {
                                                            selectedAssetData = JSON.parse(JSON.stringify(data));
                                                            mOut.TransferMenu();
                                                        }
                                                    } else {
                                                        mOut.ShowMenu();
                                                    }
                                                    transID = assetData.ID;
                                                    isFromSearchClient = false;
                                                }
                                            })
                                        })
                                    )

                                if (CStr2(trans.TransferDate) != '' && (!CBool(trans.IsDone) && !trans.IsDoneCleaning)) {
                                    row.find('td[date="' + (startday + 1) + '"]')
                                        .css('padding', '0px')
                                        .append($('<div class="gridcontainer">')
                                        .data('data', trans)
                                        .css({ 'width': row.find('td[date="' + (startday + 1) + '"]').innerWidth() + 'px' })
                                        .addClass('text-center gridforcleaning')
                                        .click(function (e) {
                                            e.stopPropagation();
                                            e.preventDefault();
                                            var data = $(this).data('data');
                                            NewMessageBox('Is <b>' + data.AssetName + '</b> is ready to use?', 'Message', [
                                                {
                                                    Text: 'Yes', Click: function () {
                                                        workModel.Save({
                                                            data: {
                                                                TransID: data.ID,
                                                                AssetID: data.AssetID,
                                                                Action: 'donecleaning'
                                                            },
                                                            success: function (e) {
                                                                WebToast(data.AssetName + ' is ready to use', 'top');
                                                                CheckAvailability(function () {
                                                                    AssetMgmtDashboardController.GetDashboardData();
                                                                });
                                                                mOut.ClearFieldsNewCustomer();
                                                            }
                                                        })
                                                    }
                                                }, { Text: 'No' }
                                            ])
                                        })
                                    )
                                }
                            }
                        }
                    }
                }                
            }
        }

        function IsTransfer() {
            if ($('#popupTransferAsset').css('display') == 'none') {
                isTransferAsset = false;
            }
        }

        function NavigateDate(nav) {
            var selAvailabilityMonth = $('#selAvailabilityMonth');
            var txtAvailabilityYear = $('#txtAvailabilityYear');
            var curMonth = CInt2(selAvailabilityMonth.val());
            var curYear = CInt2(txtAvailabilityYear.val());            

            if(nav == 'prev'){
                curMonth -= 1;

                if(curMonth < 0){
                    curMonth = 11; // go to december
                    curYear -= 1; // go to prev year
                    txtAvailabilityYear.val(curYear);
                }

                selAvailabilityMonth.val(curMonth);
            } else {
                curMonth += 1;                

                if (curMonth > 11) {
                    curMonth = 0; // go to january
                    curYear += 1; // go to next year
                    txtAvailabilityYear.val(curYear);
                }

                selAvailabilityMonth.val(curMonth);
            }
            var enddate = FormatDate(new Date(CInt2(curYear), CInt2(curMonth) + 1, 0), { Format: 'd' });
            GetAsset(curMonth, curYear, enddate);
        }
        
        function LoadRequiredData() {
            //GetListOfAssets();            

            AjaxSend({
                url: 'Browser/LibraryCities',
                data: { IsAll : true },
                success: function (e) {
                    var vRows = e.Results.Rows;
                    var selAssetCusCity = $('#selAssetCusCity');
                    selAssetCusCity.empty();

                    for (var i = 0; i < vRows.length; i++) {
                        if (vRows[i].Name != "")
                            selAssetCusCity.append(new Option(vRows[i].Name, vRows[i].ID))
                    }

                    selAssetCusCity.val('316a03d3-611a-4376-a2b2-7d0dfd7a4fbc'); // default to Davao City

                    AjaxSend({
                        url: 'Browser/LibraryCountries',
                        data: { IsAll: 1 },
                        success: function (e) {
                            var vRows = e.Results.Rows;
                            var selAssetCusCountry = $('#selAssetCusCountry');
                            var selAssetCusCountryCode = $('#selAssetCusCountryCode');
                            selAssetCusCountry.empty();
                            selAssetCusCountryCode.empty();

                            for (var i = 0; i < vRows.length; i++) {
                                selAssetCusCountry.append(new Option(vRows[i].Name, vRows[i].ID))
                                selAssetCusCountryCode.append(new Option(vRows[i].Code + ' - ' + CStr2(vRows[i].CallingCode), vRows[i].ID))
                            }

                            selAssetCusCountry.val('e783ed13-aa98-4cc1-918e-d49b05c0167e'); // default to Philippines
                            selAssetCusCountryCode.val('e783ed13-aa98-4cc1-918e-d49b05c0167e'); // default to Philippines

                            CheckAvailability();
                        }
                    })
                }
            })            

            // 2017_10_26__09_03_55_0587 dont use dropdown for mobile number
            //AjaxSend({
            //    url: 'Browser/LibraryTelcoPrefixes',
            //    data: { IsAll: 1 },
            //    success: function (e) {
            //        var vRows = e.Results.Rows;
            //        var selAssetCusTelcoPrefix = $('#selAssetCusTelcoPrefix');
            //        selAssetCusTelcoPrefix.empty();

            //        for (var i = 0; i < vRows.length; i++) {
            //            if (vRows[i].Name != "")
            //                selAssetCusTelcoPrefix.append(new Option(vRows[i].Prefix, vRows[i].ID))
            //        }

            //        selAssetCusTelcoPrefix.val('e783ed13-aa98-4cc1-918e-d49b05c0167e'); // default to Philippines
            //    }
            //})
        }

        function GetListOfAssets() {
            workModel.GetListOfAssets({
                data: {
                    Search: $.trim($('#txtSearchAsset').val()),
                    Status: $('#selAssetStatus').val(),
                    Type: $('#selAssetType').val()
                },
                success: function (vRows) {
                    var tblAssetsMgmt = $('#tblAssetsMgmt');

                    if ($.fn.DataTable.isDataTable(tblAssetsMgmt)) {
                        //empty the table first before destroying datatabase para dili magdoble ang header
                        tblAssetsMgmt.empty();
                        dtAssetsMgmt.destroy();
                    }

                    dtAssetsMgmt = tblAssetsMgmt.DataTable({
                        "data": vRows,
                        "pageLength": 50,
                        "columns": [
                            {
                                "data": function (data) {
                                    return '<b class="ui-table-cell-label">Asset Name</b>' + data.AssetName;
                                }, width: '200px'
                            },
                            {
                                "data": function (data) {
                                    return '<b class="ui-table-cell-label">Asset Type</b>' + data.AssetType;
                                }
                            },
                            {
                                "data": function (data) {
                                    return '<b class="ui-table-cell-label">Capacity</b>' + data.Capacity;
                                }
                            },
                            {
                                "data": function (data) {
                                    return '<b class="ui-table-cell-label">Capacity Unit</b>' + data.CapacityUnit;
                                }
                            },
                            {
                                "data": function (data) {
                                    var color;
                                    if (CStr2(data.Status) == 'Available') {
                                        color = '4CAF50';
                                    } else if (CStr2(data.Status) == 'Reserved') {
                                        color = '03A9F4';
                                    } else {
                                        color = 'f44336';
                                    }
                                    return '<b class="ui-table-cell-label">Status</b> <span style="color:#' + color + ';">' + data.Status + '</span>';
                                }
                            },
                            {
                                "data": function (data) {
                                    var cus;
                                    if ((CStr2(data.ReservedFrom) == '' && CStr2(data.CheckedInUsedOn) == '') || CBool(data.IsCancelled)) {
                                        cus = '';
                                    } else {
                                        cus = CStr2(data.CustomerName);
                                    }
                                    return '<b class="ui-table-cell-label">Customer</b>' + cus;
                                }
                            }
                        ],
                        "aoColumnDefs": [{ "bSortable": false, "aTargets": '_all' }],
                        "responsive": true,
                        "columnDefs": [{ "targets": 0, "orderable": true }],
                        "sDom": '<"top">rt<"bottom"ip><"clear">',
                        "initComplete": function () {
                            if (!isLoadAssetType) {
                                var selAssetType = $('#selAssetType');
                                var type = [];
                                isLoadAssetType = true;
                                selAssetType.empty();
                                selAssetType.append(new Option("All", "All"));
                                for (var i = 0; i < vRows.length; i++) {
                                    var cat = vRows[i].AssetType;
                                    var cCat = (cat).toLowerCase();
                                    if ($.inArray(cCat, type) != 0) {
                                        type.push(cCat);
                                        selAssetType.append(new Option(cat, cat));
                                    }
                                }
                            }                            
                        }
                    });

                    $('#tblAssetsMgmt tbody').unbind('click').on('click', 'tr', function () {
                        assetData = dtAssetsMgmt.row(this).data();                        
                        if (assetData != undefined) {
                            mOut.ShowMenu(false);
                        }
                    });

                    tblAssetsMgmt.table('refresh');
                }
            });
        }

        function ConvertDate(date) {
            return new Date(parseInt($.trim(date).replace('/Date(', '')));
        }

        function GetListOtherCharges() {
            var tbodyOC = $('#tbodyOC');
            tbodyOC.empty();

            AjaxSend({
                url: 'AssetsManagement/GetListOtherCharges',
                data: { keywords: '', Status: 1 },
                success: function (e) {
                    var vRows = e.Results.Rows;

                    if (vRows.length > 0) {
                        for (var i = 0; i < vRows.length; i++) {
                            tbodyOC.append($('<tr>')
                                    .addClass('tr-' + vRows[i].ID)
                                    .data('data', vRows[i])
                                    .append($('<td>')
                                        .append($('<div class="form-check">')
                                            .append($('<label for="' + vRows[i].ID + '" class="form-check-label">')
                                                .append($('<input type="checkbox" name="workoc" id="' + vRows[i].ID + '" class="workoc form-check-input" data-iconpos="notext" value="' + vRows[i].ID + '">')
                                                    .click(function (e) {
                                                        e.stopPropagation();
                                                    })
                                                )
                                            )
                                            //.append($('<div>').text('asd'))
                                        )
                                    )
                                    .append($('<td>').text(vRows[i].ProductName))
                                    .append($('<td>').append($('<input type="text" data-role="none" id="txtworkocQty-' + vRows[i].ID + '" class="form-control txtworkocQty">').val(1)
                                            .click(function (e) {
                                                e.stopPropagation();
                                                e.preventDefault();
                                            })
                                        )
                                    )
                                    .append($('<td>').text(AccountingFormat(vRows[i].Price)))
                                    .click(function (e) {
                                        e.stopPropagation();
                                        if (CBool($(this).data('data').IsRequired)) {
                                            return // dont allow to unchecked
                                        }
                                        var isCheck = $(this).find('input[type="checkbox"]').is(':checked');

                                        $(this).find('input[type="checkbox"]').prop('checked', !isCheck)//.checkboxradio('refresh')
                                    })
                                )

                            if(CBool(vRows[i].IsRequired)){
                                $('#' + vRows[i].ID).prop('checked', 1).attr('disabled', true)
                            }                           
                        }

                        if (assetData.OtherCharges != undefined) {
                            for (var i = 0; i < assetData.OtherCharges.length; i++) {
                                $('.tr-' + assetData.OtherCharges[i].OtherChargesID).data('data').TransOCId = assetData.OtherCharges[i].TransOCId;
                                $('#' + assetData.OtherCharges[i].OtherChargesID).prop('checked', true);
                                $('#txtworkocQty-' + assetData.OtherCharges[i].OtherChargesID).val(assetData.OtherCharges[i].RequestQty)
                            }
                        }                        

                        $('.workoc')//.checkboxradio()//.checkboxradio('refresh');
                    } else {
                        tbodyOC.append($('<tr>')
                            .append($('<td colspan="10">').text('No result(s) found'))
                        )
                    }
                }
            })
        }

        mOut.CheckAvailability = CheckAvailability

        mOut.GetGuestCardInformation = function () {
            AjaxSend({
                url: 'AssetsManagement/GetCardInformation',
                data: { ClientID: clientID },
                success: function (e) {
                    if (e.Results.Rows.length > 0) {
                        $('#lblRoomNo').text(e.Results.Rows[0].RoomNo);
                        $('#lblCheckInDate').text(e.Results.Rows[0].CheckInTime);
                        $('#lblCheckOutDate').text(e.Results.Rows[0].CheckOutTime);
                    }                    
                }
            })
        }

        mOut.ConvertDate = function (date) {
            return ConvertDate(date);
        }

        mOut.TransferMenu = function () {
            assetData.IsTransfer = true;
            transID = CStr2(assetData.ID);

            if (assetData.Status == 'reserved') {
                isFromReserved = true;
                mOut.ReserveAsset();
                mOut.EditDetails();                                
                assetData.AssetID = selectedAssetData.ID;
                $('#lblAssetActionTitle').text('Transfer Reservation for ' + assetData.CustomerName);
            } else if(assetData.Status == 'unavailable'){
                isFromReserved = false;
                assetData.TransferToAssetID = selectedAssetData.ID;
                assetData.TransferProductID = selectedAssetData.ProductID;
                mOut.CheckInUseAsset();
                mOut.EditDetails();                
                $('#lblAssetActionTitle').text('Transfer ' + assetData.AssetType + ' for ' + assetData.CustomerName);
            }

            $('#lblTransferTo').text(selectedAssetData.AssetName);
            $('#divTransfer').show();
        }

        var isTransfersClicked = false;
        mOut.ShowMenu = function (isLastDate, curstat, vDontValidate) { // params only used when you want to select the last date

            if (!vDontValidate) {
                AjaxSend({
                    url: 'AssetsManagement/IsValidRentableAsset',
                    data: { ProductID: assetData.ProductID },
                    success: function (e) {
                        if (e.Results == 'ok')
                            mOut.ShowMenu(isLastDate, curstat, true);
                        else
                            NewMessageBox(e.Error);
                    }
                });
                return;
            }

            var arrOptions = [];
            mOut.ClearFieldsNewCustomer();
            isTransferAsset = false;

            if (CBool(assetData.IsPastCheckins)) {
                $('#btnSaveAssetCus').parent().hide();
                arrOptions = mOut.PastCheckinMenu();
            } else {
                $('#btnSaveAssetCus').parent().show();
                if (CStr2(assetData.Status) == 'available' && !CBool(assetData.IsCancelled)) {
                    isFromReserved = false;
                    arrOptions = [
                        {
                            Text: (AssetMgmtSettingsController.LabelSetting == 'hotel industry' ? 'Reserve ' + assetData.AssetType : 'Reserve ' + AssetMgmtSettingsController.AssetsLabel), Click: function () {
                                mOut.ReserveAsset(assetData);
                            }
                        }
                    ]

                    if (!(new Date(assetData.GridDate || assetData.StartDate) > new Date($.format.date(new Date(), 'MMM dd, yyyy'))) || CBool(AssetMgmtSettingsController.IsAllowFutureDatedCheckIns)) {
                        arrOptions.push({
                            Text: (AssetMgmtSettingsController.LabelSetting == 'hotel industry' ? 'Check In / Use Asset' : 'Use ' + AssetMgmtSettingsController.AssetsLabel), Click: function () {
                                mOut.ClearFieldsNewCustomer();
                                clientID = CStr2(assetData.ClientID);
                                mOut.CheckInUseAsset();
                                mOut.EditDetails();
                            }
                        })
                    }

                    arrOptions.push({
                        Text: 'Mark as Out of Order', Click: function () {
                            NewMessageBoxClose();
                            $('#popAssetOutOfOrder').modal('show');
                            $('#lblOutOfOrderAssetName').text(assetData.AssetName);
                            $('#txtOOODateFrom').attr('min', $.format.date(new Date(assetData.GridDate), 'yyyy-MM-dd')).val($.format.date(new Date(assetData.GridDate), 'yyyy-MM-dd'));
                            $('#txtOOODateTo').attr('min', $.format.date(new Date(assetData.GridDate), 'yyyy-MM-dd')).val($.format.date(new Date(new Date().setDate(new Date(assetData.GridDate).getDate() + 1)), 'yyyy-MM-dd'))
                            $('#txtOOOReason').val(assetData.OutOfOrderReason);
                        }
                    })
                    dateFrom = assetData.GridDate + ' ' + $.format.date(new Date(), 'hh:mm a');
                } else if (CStr2(assetData.Status) == 'reserved') {
                    isFromReserved = true;
                    arrOptions = [
                        {
                            Text: 'Cancel Reservation', Click: function () {
                                NewMessageBox('Are you sure you want to cancel reservation of <b>' + assetData.CustomerName + '</b> of ' + assetData.AssetName + '?', 'Message', [
                                    {
                                        Text: 'Yes', Click: function () {
                                            workModel.Save({
                                                data: {
                                                    TransID: assetData.ID,
                                                    AssetID: assetData.AssetID,
                                                    Action: 'cancel'
                                                },
                                                success: function (e) {
                                                    WebToast(assetData.AssetName + ' has been cancelled.', 'top');
                                                    CheckAvailability(function () {
                                                        AssetMgmtDashboardController.GetDashboardData();
                                                    });

                                                    if (isFromSearchClient) {
                                                        SearchClient($('.txtAssetSearchClient').val());
                                                    }
                                                }
                                            })
                                        }
                                    }, { Text: 'No' }
                                ])
                            }
                        },
                        {
                            Text: 'Edit Reservation Details', Click: function () {
                                mOut.ReserveAsset();
                                mOut.EditDetails();
                                transID = assetData.ID;
                                $('#lblAssetActionTitle').text((AssetMgmtSettingsController.LabelSetting == 'hotel industry' ? 'Edit Reservation Details for ' + assetData.AssetName : 'Edit Rented / Leased ' + AssetMgmtSettingsController.AssetsLabel));
                            }
                        }
                    ]

                    if (!(new Date(assetData.StartDate) > new Date($.format.date(new Date(), 'MMM dd, yyyy')))) {
                        arrOptions.push({
                            Text: (AssetMgmtSettingsController.LabelSetting == 'hotel industry' ? 'Check In / Use Asset' : 'Use ' + AssetMgmtSettingsController.AssetsLabel), Click: function () {
                                mOut.ClearFieldsNewCustomer();
                                clientID = CStr2(assetData.ClientID);
                                mOut.CheckInUseAsset();
                                mOut.EditDetails();
                                transID = assetData.ID;
                            }
                        })
                    }
                } else if (CStr2(assetData.Status) == 'cancelled') {
                    isFromReserved = true;
                    arrOptions = [
                        {
                            Text: 'View Details', Click: function () {
                                mOut.ReserveAsset();
                                mOut.EditDetails();
                                transID = assetData.ID;
                                $('#lblAssetActionTitle').text(assetData.AssetName);
                                //reuse this tags
                                $('#lblTransferLabel').text('Status:');
                                $('#lblTransferAsset').text('Cancelled by ' + assetData.UpdatedBy + ' on ' + $.format.date(new Date(assetData.DateCancelled), 'MM-dd-yyyy hh:mm a'));
                                $('#btnSaveAssetCus').parent().hide();
                            }
                        }
                    ]
                } else {
                    isFromReserved = false;
                    arrOptions = [
                        {
                            Text: 'Edit Details', Click: function () {
                                NewPromptUserAccess('Edit checked-in/ used asset details', {
                                    OnClickOKSuccess: function () {
                                        mOut.CheckInUseAsset();
                                        mOut.EditDetails();
                                        transID = assetData.ID;
                                        dateFrom = FormatDate(new Date(assetData.CheckedInUsedOn), { Format: 'MMM dd, yyyy hh:mm a' })
                                        $('#lblAssetActionTitle').text((AssetMgmtSettingsController.LabelSetting == 'hotel industry' ? 'Edit Check Out / Return Asset Details for ' + assetData.AssetName : 'Edit Rented / Leased ' + AssetMgmtSettingsController.AssetsLabel));
                                    }
                                });
                            }
                        },
                        {
                            Text: (AssetMgmtSettingsController.LabelSetting == 'hotel industry' ? 'Check Out / Return Asset' : 'End Lease / Rent'), Click: function () {
                                NewMessageBox('Are you sure you want to check-out this asset of <b>' + assetData.CustomerName + '</b> of ' + assetData.AssetName + '?', 'Confirmation Message', [
                                    {
                                        Text: 'Yes', Click: function () {
                                            workModel.Save({
                                                data: {
                                                    TransID: (assetData.TransferFromID || assetData.ID),
                                                    AssetID: assetData.AssetID,
                                                    Action: 'checkout'
                                                },
                                                success: function (e) {
                                                    CheckAvailability(function () {
                                                        AssetMgmtDashboardController.GetDashboardData();
                                                    });
                                                    mOut.ClearFieldsNewCustomer();

                                                    if (isFromSearchClient) {
                                                        SearchClient($('.txtAssetSearchClient').val());
                                                    }

                                                    NewMessageBox('<b>' + assetData.AssetName + '</b> has been checked-out / returned. <br><br> Do you want to: (Please click as many as needed)', 'Message', [
                                                        {
                                                            Text: 'Download Guest Registration Card', Validation: function () {
                                                                window.location = MergeUrlParameters({
                                                                    TransID: (assetData.TransferFromID || assetData.ID),
                                                                    FileName: 'GuestRegistrationCard',
                                                                    OrgID: sessionStorage.OrgID,
                                                                    FowstID: sessionStorage.FowstID
                                                                }, '/AssetsManagement/DownloadAssetFile');

                                                                return false;
                                                            }
                                                        }, {
                                                            Text: 'Download Guest Folio', Validation: function () {
                                                                window.location = MergeUrlParameters({
                                                                    TransID: (assetData.TransferFromID || assetData.ID),
                                                                    FileName: 'GuestFolio',
                                                                    OrgID: sessionStorage.OrgID,
                                                                    FowstID: sessionStorage.FowstID
                                                                }, '/AssetsManagement/DownloadAssetFile');
                                                                return false;
                                                            }
                                                        }, {
                                                            Text: 'Done and Pay Now', Click: function () {
                                                                //DO NOTHING
                                                            }
                                                        }
                                                    ], {
                                                        Close: function () {
                                                            var pClientID = assetData.ClientID;
                                                            var pCustomerName = assetData.CustomerName;
                                                            if (CStr2(assetData.OrgID).trim() != '') {
                                                                pClientID = assetData.OrgID;
                                                                pCustomerName = assetData.CompanyName;
                                                            }
                                                            console.log(pClientID)
                                                            ERP.AccountDetails.controller.GetCustomerCreditDetails(pClientID, 'has payable');
                                                            $('#popupAccountDetails').find('.lblCustomerName').text(pCustomerName);
                                                            $('#popupAccountDetails').modal('show');
                                                        }
                                                    })
                                                }
                                            })
                                        }
                                    }, { Text: 'No' }
                                ])
                            }
                        }
                    ]
                }

                if (CStr2(assetData.Status) == 'reserved' || CStr2(assetData.Status) == 'unavailable') {
                    arrOptions.push({
                        Text: (AssetMgmtSettingsController.LabelSetting == 'hotel industry' ? 'Transfer ' + assetData.AssetType : 'Transfer to Another ' + AssetMgmtSettingsController.AssetsLabel), Click: function () {
                            isTransferAsset = true;
                            if (!isTransfersClicked) {
                                loadassets = true;
                                isTransfersClicked = true;
                            }
                            $('#popupTransferAsset').modal('show').on('shown.bs.modal', function () {
                                Bs.Loading('show');
                                CheckAvailability();
                            });
                        }
                    })

                    if (CStr2(assetData.Status) == 'unavailable') {
                        arrOptions.push({
                            Text: 'Invoice Products / Service', Validation: function () {
                                WebToast('Not yet implemented', 'top');
                                return false;
                            }
                        })

                        arrOptions.push({
                            Text: 'Check Client Billing', Click: function () {
                                require(['/Scripts/sources/CreditAndCollection/CreditAndCollection.js?v=' + assetversion], function () {
                                    proj.CustomerPaymentID = assetData.ClientID;
                                    ERP.AccountDetails.controller.GetCustomerCreditDetails(proj.CustomerPaymentID, 'has payable');
                                    $('#popupAccountDetails').find('.lblCustomerName').text(assetData.CustomerName);
                                    $('#popupAccountDetails').modal('show');
                                });
                            }
                        })

                        if (AssetMgmtSettingsController.LabelSetting == 'hotel industry') {
                            arrOptions.push({
                                Text: 'Replace Lost/ Malfunctioning Key Card', Click: function () {
                                    if (CBool($('[name="radLostCard"]:checked').val())) {
                                        var splitPrice = $('#selChargeforLostCards option:selected').text().split('-');

                                        NewMessageBox('The client will be charge ' + splitPrice[splitPrice.length - 1] + ' for the lost Key Card. Would you like to continue?', 'Charge for Lost Key Card ', [
                                            {
                                                Text: 'Yes', Click: function () {
                                                    cardCount = 1;
                                                    currentCardCount = 1;
                                                    mOut.CreateCard(1000);

                                                    AjaxSend({
                                                        url: 'AssetsManagement/UpdateLostCardKeyCounter',
                                                        data: {
                                                            TransID: assetData.ID,
                                                            ChargesID: $('#selChargeforLostCards').val()
                                                        },
                                                        success: function (e) {

                                                        }
                                                    });
                                                    $('#popupCardCreation').modal('show');
                                                    $('#dvMessage').text('Please put card on top of the card reader');
                                                }
                                            }, { Text: 'No' }
                                        ])
                                    }
                                }
                            })

                            arrOptions.push({
                                Text: 'Download Guest Registration Card', Click: function () {
                                    window.location = MergeUrlParameters({
                                        TransID: assetData.TransID,
                                        FileName: 'GuestRegistrationCard',
                                        OrgID: sessionStorage.OrgID,
                                        FowstID: sessionStorage.FowstID
                                    }, '/AssetsManagement/DownloadAssetFile');
                                }
                            })

                            arrOptions.push({
                                Text: 'Download Guest Folio', Click: function () {
                                    window.location = MergeUrlParameters({
                                        TransID: assetData.TransID,
                                        FileName: 'GuestFolio',
                                        OrgID: sessionStorage.OrgID,
                                        FowstID: sessionStorage.FowstID
                                    }, '/AssetsManagement/DownloadAssetFile');
                                }
                            })
                        }
                    }

                    arrOptions.push({
                        Text: 'Download Contract/ Letter', Click: function () {
                            BrowsePopup('ContractDesigner/GetByModule', 'Select Document', [
                                {
                                    AutoSearch: true,
                                    WithSearchbox: false,
                                    DisplayText: [{ Column: 'TemplateName' }],
                                    Click: function (e) {
                                        Contract.Export(HtmlDecode(e.Contents), {
                                            'PaperSize': e.PaperSize,
                                            'Lessee/ Tenant/ Guest First Name': CStr2(assetData.Customer[0].FirstName || ''),
                                            'Lessee/ Tenant/ Guest Middle Name': CStr2(assetData.Customer[0].MiddleName || ''),
                                            'Lessee/ Tenant/ Guest Last Name': CStr2(assetData.Customer[0].Surname || ''),
                                            'Lessee/ Tenant/ Guest Company Name': CStr2(assetData.CompanyName || ''),
                                            'Lessee/ Tenant/ Guest Contact Number': 0 + CStr2(assetData.Customer[0].Prefix) + CStr2(assetData.Customer[0].ContactNo),
                                            'Lessee/ Tenant/ Guest Address': CStr2(assetData.Customer[0].HouseBuildingNo || '') + ' ' + CStr2(assetData.Customer[0].Street || '') + ' ' + CStr2(assetData.Customer[0].CityName || '') + ' ' + CStr2(assetData.Customer[0].CountryName || ''),
                                            'Lessee/ Tenant/ Guest Email Address': CStr2(assetData.Customer[0].EmailAddress || ''),
                                            'Renter/ Lessee Company\'s Address': assetData.CompanyAddress || '',
                                            'Name of Asset (to be) Rented/ Leased': assetData.AssetName,
                                            'Lease/ Rent Rate': AccountingFormat(assetData.Rate),
                                            'Lease/ Rent Beginning Date': $.format.date(new Date(assetData.CheckedInUsedOn || assetData.ReservedFrom), 'MMMM dd, yyyy'),
                                            'Lease/ Rent Ending Date': $.format.date(new Date(assetData.CheckedOutReturnedOn || assetData.ReservedUntil), 'MMMM dd, yyyy')
                                        }, JSON.parse(JSON.stringify(e.FreeTypeFields)))
                                    },
                                    ReturnVariable: 'Results'
                                }
                            ], { Module: 'Asset Management' })
                        }
                    })
                }
            }            
            
            PopupButtonList('Asset Name: <b>' + assetData.AssetName + '</b>' + (CStr2(assetData.CustomerName) != '' ? '<br><br>Client Name: <b>' + assetData.CustomerName + '</b>' : ''), 'Options', arrOptions, { AutoDismiss: true });
        }

        mOut.PastCheckinMenu = function () {
            return [
                    {
                        Text: 'View Details', Click: function () {
                            mOut.CheckInUseAsset();
                            mOut.EditDetails();
                            dateFrom = FormatDate(new Date(assetData.CheckedInUsedOn), { Format: 'MMM dd, yyyy hh:mm a' })
                            $('#lblAssetActionTitle').text('View Details for ' + assetData.AssetName);
                            $('#lblAssetDate').text('Checked Out Date');
                        }
                    },
                    {
                        Text: 'Check Client Billing', Click: function () {
                            require(['/Scripts/sources/CreditAndCollection/CreditAndCollection.js?v=' + assetversion], function () {
                                proj.CustomerPaymentID = assetData.ClientID;
                                ERP.AccountDetails.controller.GetCustomerCreditDetails(proj.CustomerPaymentID, 'has payable');
                                $('#popupAccountDetails').find('.lblCustomerName').text(assetData.CustomerName);
                                $('#popupAccountDetails').modal('show');
                            });
                        }
                    },
                    {
                        Text: 'Download Guest Registration Card', Click: function () {
                            window.location = MergeUrlParameters({
                                TransID: assetData.TransID,
                                FileName: 'GuestRegistrationCard',
                                OrgID: sessionStorage.OrgID,
                                FowstID: sessionStorage.FowstID
                            }, '/AssetsManagement/DownloadAssetFile');
                        }
                    },
                    {
                        Text: 'Download Guest Folio', Click: function () {
                            window.location = MergeUrlParameters({
                                TransID: assetData.TransID,
                                FileName: 'GuestFolio',
                                OrgID: sessionStorage.OrgID,
                                FowstID: sessionStorage.FowstID
                            }, '/AssetsManagement/DownloadAssetFile');
                        }
                    }
            ]
        }

        mOut.EditDetails = function () {
            orgid = assetData.OrgID;
            dateFrom = ((assetData.ReservedFrom || assetData.CheckedInUsedOn) == undefined ? dateFrom : FormatDate(new Date((assetData.ReservedFrom || assetData.CheckedInUsedOn)), { Format: 'MMM dd, yyyy' }));
            dateTo = ((assetData.ReservedUntil || assetData.CheckedOutReturnedOn) == undefined ? dateTo : FormatDate(new Date((assetData.ReservedUntil || assetData.CheckedOutReturnedOn)), { Format: 'MMM dd, yyyy hh:mm a' }));
            arrCustomer = (assetData.Customer || []);

            if (action == 'reserve') {
                $('#divCheckedInDate, #divActualCheckedInDate').hide();
                $('#lblActualCheckedInDate').text('');
                $('#btnDateReservation').text(dateFrom + ' to ' + dateTo);
            } else {
                $('#divCheckedInDate').show();

                if (isFromReserved) {
                    $('#divCheckedInDate').show();
                    dateFrom = $.format.date(new Date(), 'MMM dd, yyyy ') + AssetMgmtSettingsController.DefaultCheckOutTime.Time;
                }
                if ($.format.date(new Date(dateTo), 'MMM dd, yyyy') == $.format.date(new Date(), 'MMM dd, yyyy')) {
                    var newDateTo = new Date()
                    dateTo = $.format.date(new Date(newDateTo.setDate(newDateTo.getDate() + 1)), 'MMM dd, yyyy ') + AssetMgmtSettingsController.DefaultCheckOutTime.Time;
                }

                if(assetData.Status == 'unavailable'){
                    dateTo = $.format.date(new Date(assetData.CheckedOutReturnedOn), 'MMM dd, yyyy ') + AssetMgmtSettingsController.DefaultCheckOutTime.Time;
                } else if (assetData.Status == 'pastcheckin') {
                    dateTo = $.format.date(new Date(assetData.ActualCheckedOutReturnedOn), 'MMM dd, yyyy hh:mm a');
                }

                $('#lblCheckedInDate').text(dateFrom);
                $('#btnDateReservation').text(dateTo);

                if (CStr2(assetData.CheckedInUsedOn) != '') {                    
                    $('#lblCheckedInDate').text(FormatDate(new Date(CStr2(assetData.TransferCheckedInDate) == '' ? assetData.CheckedInUsedOn : assetData.TransferCheckedInDate), { Format: 'MMM dd, yyyy hh:mm a' }));

                    if ($.format.date(new Date(assetData.CheckedInUsedOn), 'MMddyyyy') != $.format.date(new Date(assetData.ActualCheckedInUsedOn), 'MMddyyyy')) {
                        $('#lblCheckedInDate').text($.format.date(new Date(assetData.CheckedInUsedOn), 'MMM dd, yyyy') + ' (Late Checked-in)');
                        $('#lblActualCheckedInDate').text($.format.date(new Date(assetData.ActualCheckedInUsedOn), 'MMM dd, yyyy hh:mm a'))
                        $('#divActualCheckedInDate').show();
                    } else {
                        $('#lblActualCheckedInDate').text('');
                        $('#divActualCheckedInDate').hide();
                    }
                }                
            }            
            $('#txtAssetOrgName').val(assetData.CompanyName);            
            $('#txtNoOfAdults').val(assetData.NoOfAdults);
            $('#txtNoOfChild').val(assetData.NoOfChild);
            $('#txtAssetOTAName').val(assetData.OnlineTravelAgenyName);
            $('#txtAssetOTAConfirmationNo').val(assetData.OTAConfirmationNumber);
            $('#txtNoOfGuestCards').val(assetData.NoOfGuestCards);
            $('#lblNoOfDays').text(Math.abs((new Date($.format.date(new Date(dateFrom), 'MMM dd, yyyy')) - new Date($.format.date(new Date(dateTo), 'MMM dd, yyyy'))) / (1000 * 60 * 60 * 24)));

            if (CStr2(assetData.TransferFrom) != '') {
                $('#lblTransferLabel').text('Transfer from:');
                $('#lblTransferAsset').text(assetData.TransferFrom);
            } else if (CStr2(assetData.TransferTo) != '') {
                $('#lblTransferLabel').text('Transfer to:');
                $('#lblTransferAsset').text(assetData.TransferTo);
            } else {
                $('#lblTransferLabel').text('');
                $('#lblTransferAsset').text('');
            }

            if (CStr2(assetData.ReservationWasMadeByName) != '') {
                $('#divReservationWasMadeBy').show();
                $('#lblReservationWasMadeBy').text(assetData.ReservationWasMadeByName + ' on ' + $.format.date(CJsonDate(assetData.DateReserved), 'MMM dd, yyyy hh:mm a'));
            } else {
                $('#divReservationWasMadeBy').hide();
                $('#lblReservationWasMadeBy').text('');
            }

            DisplayRequest();
            DisplayPreviousRequest((assetData.PreviousRequest || []));
            DisplayCustomer();
        }

        mOut.SaveAssetCustomer = function () {
            
            var txtNoOfAdults = $.trim($('#txtNoOfAdults').val());
            var txtNoOfChild = $.trim($('#txtNoOfChild').val());
            var txtNoOfGuestCards = $('#txtNoOfGuestCards');
            var txtAssetOTAName = $.trim($('#txtAssetOTAName').val());
            var txtAssetOTAConfirmationNo = $.trim($('#txtAssetOTAConfirmationNo').val());
            var arrOtherCharges = [];

            if (action == 'reserve') {
                if (dateFrom == '' || dateTo == '') {
                    WebToast('Please select valid reservation date', 'top');
                    return;
                }                
            } else {
                if (dateTo == '') {
                    WebToast('Please select check out date', 'top');
                    return;
                }
            }

            if (isEmptyCustomer) {
                WebToast('Please add at least one customer', 'top');
                return;
            }

            if (AssetMgmtSettingsController.LabelSetting == 'hotel industry') {
                if (!IsNumeric(txtNoOfAdults) && txtNoOfAdults == '') {
                    $('#tabCusAssetInfo').opentabmenu();
                    WebToast('Please enter number of adults', 'top');
                    return;
                }

                if (!IsNumeric(txtNoOfChild) && txtNoOfChild != '') {
                    $('#tabCusAssetInfo').opentabmenu();
                    WebToast('Please enter number of child', 'top');
                    return;
                }

                if ($.trim(txtNoOfGuestCards.val()) != '') {
                    if (!IsNumeric(txtNoOfGuestCards.val())) {
                        $('#tabCusAssetInfo').opentabmenu();
                        WebToast('Please enter number of guest cards to be created', 'top');
                        return;
                    }

                    if ((CDec2(txtNoOfGuestCards.val()) > CDec2(txtNoOfGuestCards.attr('max'))) && CDec2(txtNoOfGuestCards.attr('max')) != 0) {
                        $('#tabCusAssetInfo').opentabmenu();
                        WebToast('Only ' + CDec2(txtNoOfGuestCards.attr('max')) + ' guest card(s) to be created', 'top');
                        return;
                    }
                }
            }            

            if ($('.chbrates:checked').length == 0 && action != 'reserve') {
                $('#tabCusAssetInfo').opentabmenu();
                WebToast('Please select rates', 'top');
                return;
            }

            if (CStr2($('.chbrates:checked').val()) == 'freetype') {
                if ($('#txtAssetFreeTypeRates').val() == '') {
                    WebToast('Please enter custom rates', 'top');
                    return;
                }
            }

            var isOtherChargesValid = true;

            $('.workoc:checked').each(function () {
                var tr = $(this).parents().closest('tr');
                var data = tr.data('data');
                data.RequestQty = CDec2(tr.find('.txtworkocQty').val());

                if (data.RequestQty == 0) {
                    WebToast('Please enter quantity for <b>' + data.ProductName + '</b>', 'top');
                    isOtherChargesValid = false;
                    return false;
                } else {
                    arrOtherCharges.push(data);
                }
            })            

            var data = {
                ClientOrgID: orgid,
                TransID: transID,
                Action: action,
                AssetID: (assetData.AssetID || assetData.ID),
                AssetName: assetData.AssetName,
                IsFromExistingClient: isFromExistingClient,
                IsFromReserved: isFromReserved,
                DateFrom: dateFrom,
                DateTo: dateTo,                
                NoOfAdults: txtNoOfAdults,
                NoOfChild: txtNoOfChild,
                NoOfGuestCards: txtNoOfGuestCards.val(),
                OTAName: txtAssetOTAName,
                OTAConfirmationNo: txtAssetOTAConfirmationNo,
                PriceLevelID: CStr2($('.chbrates:checked').val()),
                CustomRates: CStr2($('#txtAssetFreeTypeRates').val()),
                TransactionNumber: (assetData.TransactionNumber || ''),
                OtherCharges: arrOtherCharges,
                TransferToAssetID: (assetData.TransferToAssetID || ''), // for transfer 'unavailable' asset only
                IsTransfer: (assetData.IsTransfer || false),
                TransferFromID: (assetData.TransferFromID || transID),
                Customer: arrCustomer,
                IsAlreadyCheckin: (CStr2(assetData.ActualCheckedInUsedOn) == '' ? false : true)
            }

            cardCount = txtNoOfGuestCards.val();

            if (!isOtherChargesValid) {
                $('#tabCusAssetInfo').opentabmenu();
                return;
            }

            if (!(new Date($.format.date(new Date(data.DateFrom), 'MMM dd, yyyy')) < new Date($.format.date(new Date(data.DateTo), 'MMM dd, yyyy')))) {
                WebToast('Please select valid Check-out date', 'top');
                return;
            }

            //console.log(data);
            //return;
            Bs.Loading('show');
            workModel.SaveAssetCustomer({
                data: data,
                success: function (e) {
                    Bs.Loading('hide');
                    if (IsGuid(e.Results)) {
                        isTransferAsset = false;
                        $('#popupTransferAsset, #popupAssetNewCustomer').modal('hide');
                        if (action == 'reserve') {
                            WebToast(assetData.AssetName + ' has been successfully reserved from ' + dateFrom + ' to ' + dateTo + '.', 'top');
                        }
                        else {
                            if(data.TransID == ''){
                                NewMessageBox('<b>' + assetData.AssetName + '</b> has been checked in / used. <br><br> Do you want to download the Guest Registration Card now?', 'Message', [
                                {
                                    Text: 'Yes', Click: function () {
                                        window.location = MergeUrlParameters({
                                            TransID: e.Results,
                                            FileName: 'GuestRegistrationCard',
                                            OrgID: sessionStorage.OrgID,
                                            FowstID: sessionStorage.FowstID
                                        }, '/AssetsManagement/DownloadAssetFile');
                                    }
                                }, { Text: 'No' }
                                ], {
                                    Close: function () {
                                        CreateKeyCard();
                                    }
                                })
                            } else {
                                CreateKeyCard();
                            }
                            
                        }

                        transID = ''; // make sure na empty string jud ni na variable after masave ang trans
                        CheckAvailability(function () {
                            AssetMgmtDashboardController.GetDashboardData();
                        });

                        if (isFromSearchClient) {
                            SearchClient($('.txtAssetSearchClient').val());
                        }
                    } else {
                        NewMessageBox(e.Results, 'Error Message');
                    }
                }
            })

            function CreateKeyCard() {
                if (CBool($('[name="radCreateKeyCardAfterCheckIn"]:checked').val())) {
                    //$('#lblGuestName').text(arrCustomer[0].FirstName + ' ' + arrCustomer[0].Surname);
                    //$('#lblGuestCheckInDate').text($.format.date(new Date(dateFrom), 'MMM dd, yyyy'));
                    //$('#lblGuestCheckOutDate').text($.format.date(new Date(dateTo), 'MMM dd, yyyy'));
                    //$('#lblGuestRoomNo').text(assetData.AssetName);
                    //$('#lblNoOfNights').text('Good for ' + $('#lblNoOfDays').text() + ' Night(s)');

                    //$('#popupCardCreationConfirmationMessage').modal('show');

                    NewMessageBox('<div style="text-align:center;">You are about to create a key/guest card(s) for <label style="font-size:1.5em; font-weight:bold">' + arrCustomer[0].FirstName + ' ' + arrCustomer[0].Surname + '</label>' +
                        '<br><br><br>' +
                        '<div style="font-size:1.5em; font-weight:bold;">Good for ' + $('#lblNoOfDays').text() + ' night(s)</div>' +
                        'from <label style="font-size:1.5em; font-weight:bold;">' + $.format.date(new Date(dateFrom), 'MMM dd, yyyy') + ' to ' + $.format.date(new Date(dateTo), 'MMM dd, yyyy') + '</label>' +
                        '<br><br><br>' +
                        'For use in <label style="font-size:1.5em; font-weight:bold;">' + assetData.AssetName + '</label>' +
                        '<br><br><br>' +
                        'Would you like to proceed with the creation of guest/ key card(s)?</div>', 'Guest Room Information', [
                            {
                                Text: 'Yes, make guest card(s)',
                                Click: function () {
                                    currentCardCount = 1;
                                    mOut.CreateCard(1000);
                                    //$('#popupCardCreation').modal('show');
                                    //$('#dvMessage').text('Please put card on top of the card reader');
                                    NewMessageBox('<label style="font-size:1.5em;">Please put card on top of the card reader</label>', 'Message');
                                }
                            }, {
                                Text: 'No, maybe next time', Click: function () {
                                    NewMessageBox('Are you sure you don\'t want to create guest key card?', 'Confirmation Dialog', [
                                        {
                                            Text: 'Yes'
                                        }, {
                                            Text: 'No, create guest key card now', Click: function () {
                                                currentCardCount = 1;
                                                mOut.CreateCard(1000);
                                                //$('#popupCardCreation').modal('show');
                                                //$('#dvMessage').text('Please put card on top of the card reader');
                                                NewMessageBox('<label style="font-size:1.5em;">Please put card on top of the card reader</label>', 'Message');
                                            }
                                        }
                                    ], {
                                        AutoDismiss: false
                                    })
                                }
                            }
                        ], {
                            AutoDismiss: false
                        })
                }
            }
        }        

        mOut.CreateCard = function (mInterval) {

            if (cardCount > 0) {

                CardID = NewID();

                if (dateFrom == undefined) {
                    dateFrom = assetData.CheckedInUsedOn;
                    dateTo = assetData.CheckedOutReturnedOn;
                }

                var cData = {
                    ID: CardID,
                    RoomNo: assetData.AssetNo,
                    CardType: 'Guest Card',
                    CheckInTime: dateFrom,
                    CheckOutTime: dateTo,
                    Status: 'Pending'
                };                

                if (currentCardCount > 1) {
                    cData.IsDuplicate = 1
                } else {
                    cData.IsDuplicate = 0
                }

                workModel.SaveCardCreation({
                    data: cData,
                    success: function (e) {
                        if (e.Results == 'Success') {                            
                            mCardCreationInterval = setInterval(function () {
                                var mData = { ID: CardID }
                                workModel.FollowupCardCreation({
                                    data: mData,
                                    success: function (e) {
                                        if (e.Results.Rows[0].Status == 'Created') {
                                            cardCount--;
                                            currentCardCount++;
                                            clearInterval(mCardCreationInterval);
                                            
                                            if (currentCardCount == 1) {
                                                countMessage = '1st';
                                            } else if (currentCardCount == 2) {
                                                countMessage = '2nd';
                                            } else if (currentCardCount == 3) {
                                                countMessage = '3rd';
                                            } else {
                                                countMessage = currentCardCount + 'th';
                                            }

                                            //$('#dvMessage').text('Please put ' + countMessage + ' card on top of the card reader');
                                            NewMessageBox('<label style="font-size:1.5em;">Please put ' + countMessage + ' card on top of the card reader</label>', 'Message');
                                            mOut.CreateCard(3000);
                                        } else {
                                            if (e.Results.Rows[0].StatusMessage != '') {
                                                //$('#dvMessage').text(e.Results.Rows[0].StatusMessage);
                                                NewMessageBox('<label style="font-size:1.5em;">' + (e.Results.Rows[0].StatusMessage || 'Please put card on top of the card reader') + '</label>', 'Message');
                                            }                                            
                                        }
                                    }
                                });
                            }, mInterval)
                        }
                    }
                });
            } else {
                //$('#popupCardCreation').modal('hide');
                NewMessageBoxClose();
            }
        }

        mOut.ReserveAsset = function () {
            action = 'reserve';
            mOut.ClearFieldsNewCustomer();            
            $('#tabCusClientInfo').opentabmenu();

            if (AssetMgmtSettingsController.LabelSetting == 'hotel industry') {
                $('#lblOTA').text('Online Travel Agency (OTA)');
                $('.lblGuest').text('Guest');
                $('#lblCheckIn').text('Checked In Date');
                $('.fieldsforhotel').show();
            } else {
                $('#lblOTA').text('Online Booking Service');
                $('.lblGuest').text(AssetMgmtSettingsController.CustomersLabel);
                $('.fieldsforhotel').hide();
            }

            $('#divCheckedInDate').hide();
            $('#lblAssetActionTitle').text('Reserve ' + assetData.AssetName);
            $('#lblAssetName').text(assetData.AssetName);
            $('#lblAssetUom').text(assetData.CapacityUnit);
            $('#lblAssetDate').text('Reservation Date:');
            $('#popupAssetNewCustomer').modal('show')
            $('#lblNoOfDays').text(Math.abs((new Date($.format.date(new Date(dateFrom), 'MMM dd, yyyy')) - new Date($.format.date(new Date(dateTo), 'MMM dd, yyyy'))) / (1000 * 60 * 60 * 24)));
            $('#lblActualCheckedInDate, #lblTransferLabel, #lblTransferAsset').text('');
            $('#divActualCheckedInDate').hide();
            mOut.GetAssetRates();
        }

        mOut.GetAssetRates = function () {
            var tbodyRates = $('#tbodyRates');
            tbodyRates.empty();
            workModel.GetAssetRates({
                data: { ProductID: (assetData.TransferProductID || assetData.ProductID) },
                success: function(e){
                    var vRows = e.OrgProductPriceData.Rows;
                    var excludePrices = ['01000012-0003-0000-0000-000000000000', '01000012-0002-0000-0000-000000000000', '01000012-0001-0000-0000-000000000000'] // Landed Cost, Vendor Price, Freight Cost

                    if (vRows.length > 0) {
                        for (var i = 0; i < vRows.length; i++) {
                            if ($.inArray(vRows[i].PriceLevelID, excludePrices) == -1) {
                                tbodyRates.append($('<tr>')
                                    .append($('<td>')
                                        .append($('<div class="form-check">')
                                            .append($('<label for="' + vRows[i].PriceLevelID + '" class="form-check-label">')
                                                .append($('<input type="radio" name="rates" id="' + vRows[i].PriceLevelID + '" class="chbrates form-check-input" data-iconpos="notext" value="' + vRows[i].PriceLevelID + '">'))
                                            )
                                            //.append($('<div>').text('asd'))
                                        )
                                    )
                                    .append($('<td>').text(vRows[i].PriceLevel))
                                    .append($('<td>').text(CStr2(GetStringObject( 'Unit.UnitMeasure', e))))
                                    .append($('<td style="text-align:right;">').text(AccountingFormat(vRows[i].Price)))
                                    .click(function () {
                                        $('[name="rates"]').removeAttr('checked')//.checkboxradio('refresh')
                                        $(this).find('input[type="radio"]').prop('checked', true)//.checkboxradio('refresh')
                                        $('#txtAssetFreeTypeRates').val('');
                                    })
                                )
                            }                            
                        }

                        if(assetData.PriceLevelID != undefined){
                            $('[value="' + assetData.PriceLevelID + '"]').prop('checked', true);
                        }                        
                    }

                    tbodyRates.append($('<tr style="display:none;">')
                                    .append($('<td>')
                                        .append($('<div class="form-check">')
                                            .append($('<label for="chbCustomRates" class="form-check-label">')
                                                .append($('<input type="radio" name="rates" id="chbCustomRates" class="chbrates form-check-input" data-iconpos="notext" value="freetype">'))
                                            )
                                        )
                                    )
                                    .append($('<td colspan="2">').text('Custom Rates'))
                                    .append($('<td>').append($('<input data-role="none" id="txtAssetFreeTypeRates" type="text" class="form-control" style="text-align:right;">')
                                        .keydown(function (e) {
                                            if(!NumberDotOnly($(this), e)){
                                                return;
                                            }
                                        })
                                    ))
                                    .click(function () {
                                        $('[name="rates"]').removeAttr('checked')//.checkboxradio('refresh')
                                        $(this).find('input[type="radio"]').prop('checked', true)//.checkboxradio('refresh')
                                        $('#txtAssetFreeTypeRates').focus();
                                    })
                                )

                    if (CStr2(assetData.PriceLevelID) == '' && assetData.Status != 'available') {
                        $('[value="freetype"]').prop('checked', true);
                        $('#txtAssetFreeTypeRates').val(assetData.CustomRates);
                    }

                    $('.chbrates')//.checkboxradio()//.checkboxradio('refresh');
                    
                    GetListOtherCharges();
                }
            })
        }

        mOut.CheckInUseAsset = function () {
            action = 'checkin';
            $('#tabCusClientInfo').opentabmenu();

            if (AssetMgmtSettingsController.LabelSetting == 'hotel industry') {
                $('#lblAssetActionTitle').text('Check In / Use Asset ' + assetData.AssetName);
                $('#lblAssetDate').text('Expected Check Out / Return Asset Date & Time:');
                $('#lblOTA').text('Online Travel Agency (OTA)');
                $('.lblGuest').text('Guest');
                $('#lblCheckIn').text('Checked In Date');
                $('.fieldsforhotel').show();
            } else {
                $('#lblAssetActionTitle').text('Use ' + AssetMgmtSettingsController.AssetsLabel + ' ' + assetData.AssetName);
                $('#lblAssetDate').text('Rent / Lease End Date & Time:');
                $('#lblOTA').text('Online Booking Service');
                $('.lblGuest').text(AssetMgmtSettingsController.CustomersLabel);
                $('#lblCheckIn').text('Rent / Lease Start Date');
                $('.fieldsforhotel').hide();
            }
            
            $('#lblAssetName').text(assetData.AssetName);
            $('#lblAssetUom').text(assetData.CapacityUnit);

            $('#popupTransferAsset').modal('hide');
            $('#popupAssetNewCustomer').modal('show');
            
            $('#lblActualCheckedInDate, #lblTransferLabel, #lblTransferAsset').text('');
            $('#divActualCheckedInDate').hide();
            mOut.GetAssetRates();
        }

        mOut.ClearFieldsNewCustomer = function () {
            arrCustomer = [], assetID = '', transID = '', orgid = '', isEmptyCustomer = true;
            $('#tabCusPersonalInfo').opentabmenu();
            $('#txtAssetOrgName').val('');            
            $('#txtNoOfAdults').val('1');
            $('#txtNoOfChild, #txtAssetOTAName, #txtAssetOTAConfirmationNo').val('');
            $('#txtNoOfGuestCards').val('1');
            $('#btnDateReservation').text((action == 'reserve' ? $.format.date(new Date(dateFrom), 'MMM dd, yyyy') + ' to ' + dateTo : dateTo));
            $('#divTransfer').hide();
            $('#listAssetCustomerRequest').empty();
            $('#tbodyAssetCustomerList').empty().append($('<tr>')
                        .append($('<td colspan="3" style="text-align:center;">').text('No result(s) found'))
                    )
            mOut.ClearCustomerInfoFields();
        }

        mOut.ClearCustomerInfoFields = function () {
            arrCustomerRequest = [], clientID = '', emailid = '', mobileid = '', dateofbirth = '', isFromExistingClient = false;
            $('#txtAssetCusFirstName').val('').removeAttr('disabled');
            $('#txtAssetCusMiddleName').val('').removeAttr('disabled');
            $('#txtAssetCusLastName').val('').removeAttr('disabled');
            $('#txtAssetCusHomeNo').val('').removeAttr('disabled');
            $('#txtAssetCusStreetName').val('').removeAttr('disabled');
            $('#selAssetCusCity').val('316a03d3-611a-4376-a2b2-7d0dfd7a4fbc').removeAttr('disabled'); //  default to Davao City
            $('#selAssetCusCountry, #selAssetCusCountryCode').val('e783ed13-aa98-4cc1-918e-d49b05c0167e').removeAttr('disabled'); // default to Philippines
            // $('#selAssetCusTelcoPrefix').val(''); // 2017_10_26__09_03_55_0587 dont use dropdown for mobile number
            $('#txtAssetCusMobileNo').val('').removeAttr('disabled');
            $('#txtAssetCusEmail').val('').removeAttr('disabled');
            $('#btnDateOfBirth').text('Set Birth Date').removeAttr('disabled');
            $('#tabCustomerInfo').opentabmenu();
        }

        return mOut;

    }); // END CreateStringObject('App.controller.PriceChange', null, function() {

    return mDeferred.promise();

});