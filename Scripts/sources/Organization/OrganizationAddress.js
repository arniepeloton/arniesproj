require([
    'common'
    ], function () {

        $(function () {

            var IsNewOrgAddLTO = true;
            var arOrgAddLTO = new Array();
            var intOrgFowstLTOCntr = 0; 
            var intOrgFowstLTOIndex = 0;

            var intOrgFowstLTORecID = 0;
            var intOrgFowstLTOORNo = 1;
            var intOrgFowstLTODateIssuedMonth = 2;
            var intOrgFowstLTODateIssuedDay = 3;
            var intOrgFowstLTODateIssuedYear = 4;
            var intOrgFowstLTODateExpirationMonth = 5;
            var intOrgFowstLTODateExpirationDay = 6;
            var intOrgFowstLTODateExpirationYear = 7;
            var intOrgFowstLTOState = 8;

            var IsNewOrgAddLTFRB = true;
            var arOrgAddLTFRB = new Array();
            var intOrgFowstLTFRBCntr = 0;
            var intOrgFowstLTFRBIndex = 0;

            var intOrgFowstLTFRBRecID = 0;
            var intOrgFowstLTFRBORNo = 1;
            var intOrgFowstLTFRBDateIssuedMonth = 2;
            var intOrgFowstLTFRBDateIssuedDay = 3;
            var intOrgFowstLTFRBDateIssuedYear = 4;
            var intOrgFowstLTFRBDateExpirationMonth = 5;
            var intOrgFowstLTFRBDateExpirationDay = 6;
            var intOrgFowstLTFRBDateExpirationYear = 7;
            var intOrgFowstLTFRBState = 8;

            var IsNewOrgAddSection = true;
            var arOrgAddSection = new Array();
            var intOrgFowstSectionCntr = 0;
            var intOrgFowstSectionIndex = 0;

            var intOrgFowstSecRecID = 0;
            var intOrgFowstSecCode = 1;
            var intOrgFowstSecName = 2;
            var intOrgFowstSecDescription = 3;
            var intOrgFowstSecFloorArea = 4;
            var intOrgFowstSecSpecialFeatures = 5;
            var intOrgFowstSecState = 6;

            var IsNewOrgAddBPermit = true;
            var arOrgAddBPermit = new Array();
            var intOrgFowstBPermitCntr = 0;
            var intOrgFowstBPermitIndex = 0;

            var intOrgFowstBPermitRecID = 0;
            var intOrgFowstBPermitNo = 1;
            var intOrgFowstBPermitBusinessID = 2;
            var intOrgFowstBPermitTradeName = 3;
            var intOrgFowstBPermitBuildingHouseNo = 4;
            var intOrgFowstBPermitStreet = 5;
            var intOrgFowstBPermitCityID = 6;
            var intOrgFowstBPermitCity = 7;
            var intOrgFowstBPermitCountryID = 8;
            var intOrgFowstBPermitCountry = 9;
            var intOrgFowstBPermitORNo = 10;
            var intOrgFowstBPermitBusinessPlateNo = 11;
            var intOrgFowstBPermitDatePaymentMonth = 12;
            var intOrgFowstBPermitDatePaymentDay = 13;
            var intOrgFowstBPermitDatePaymentYear = 14;
            var intOrgFowstBPermitDateIssuedMonth = 15;
            var intOrgFowstBPermitDateIssuedDay = 16;
            var intOrgFowstBPermitDateIssuedYear = 17;
            var intOrgFowstBPermitState = 18;

            var IsNewOrgAddTelContact = true;
            var arOrgAddTel = new Array();
            var intOrgFowstTelCntr = 0;
            var intOrgFowstTelIndex = 0;

            var intOrgFowstTelRecID = 0;
            var intOrgFowstTelCountryID = 1;
            var intOrgFowstTelCountryCode = 2;
            var intOrgFowstTelCallingCode = 3;
            var intOrgFowstTelAreaCodeID = 4;
            var intOrgFowstTelArea = 5;
            var intOrgFowstTelAreaCode = 6;
            var intOrgFowstTelContactNo = 7;
            var intOrgFowstTelExtNo = 8;
            var intOrgFowstTelContactType = 9;
            var intOrgFowstTelState = 10;

            var IsNewOrgAddMobileContact = true;
            var arOrgAddMobile = new Array();
            var intOrgFowstMobileCntr = 0;
            var intOrgFowstMobileIndex = 0;

            var intOrgFowstMobileRecID = 0;
            var intOrgFowstMobileCountryID = 1;
            var intOrgFowstMobileCountryCode = 2;
            var intOrgFowstMobileCallingCode = 3;
            var intOrgFowstMobileTelCoPrefixID = 4;
            var intOrgFowstMobilePrefix = 5;
            var intOrgFowstMobileTelCo = 6;
            var intOrgFowstMobileContactNo = 7;
            var intOrgFowstMobileContactType = 8;
            var intOrgFowstMobileState = 9;

            var IsNewOrgAddEmailContact = true;
            var arOrgAddEmail = new Array();
            var intOrgFowstEmailCntr = 0;
            var intOrgFowstEmailIndex = 0;

            var intOrgFowstEmailRecID = 0;
            var intOrgFowstEmailAddress = 1;
            var intOrgFowstEmailType = 2;
            var intOrgFowstEmailState = 3;

            var IsNewOrgAddOptHR = true;
            var arOrgAddOptHR = new Array();
            var intOrgFowstOptHRCntr = 0;
            var intOrgFowstOptHRIndex = 0;

            var intOrgFowstOptHRRecID = 0;
            var intOrgFowstOptHRWeekDay = 1;
            var intOrgFowstOptHROpeningTimeHR = 2;
            var intOrgFowstOptHROpeningTimeMin = 3;
            var intOrgFowstOptHROpeningTimeAMPM = 4;
            var intOrgFowstOptHRClosingTimeHR = 5;
            var intOrgFowstOptHRClosingTimeMin = 6;
            var intOrgFowstOptHRClosingTimeAMPM = 7;
            var intOrgFowstOptHRIsClosed = 8;

            var intOrgFowstIsLTO = true;

            var mLoaded = false;


            var IsNewPrinter = true;
            var arOrgAddPrinter = new Array();
            var intOrgFowstPrinterCntr = 0;
            var intOrgFowstPrinterIndex = 0;

            var intOrgFowstPrinterID = 0;
            var intOrgFowstPrinterName = 1;
            var intOrgFowstPrinterRole = 2;
            var intOrgFowstPrinterRoleID = 3;
            var intOrgFowstPrinterIPAddress = 4;
            var intOrgFowstPrinterState = 5;
            var isIE = false || !!document.documentMode;
            var mPosVendorID;

            var NameOfBtnForLibrary;

            var libname, lbllibname, addlibname, fowstid, orgname, peopleid;
            var fowstID = '';
            var IsDefaultDispatchPrinter = false;

            var arrFowstTel = [], arrFowstMobile = [], arrFowstEmail = [], arrFowstSection = [], arrFowstBusinessPermit = [];
            var dateofpayment = "", dateissued = "";
            var arrFowstPriceLevels = [];
            var arrRemoveAssignedEmp = [];

            setMonthCollection("selOrgFOWSTContractStartMonth");
            setYearCollection("selOrgFOWSTContractStartYear");

            setMonthCollection("selOrgFOWSTContractEndMonth");
            setYearCollection("selOrgFOWSTContractEndYear");

            setMonthCollection("selOrgFOWSTTRegIssuedMonth");
            setYearCollection("selOrgFOWSTTRegIssuedYear");

            setMonthCollection("selOrgFOWSTTRegExpirationMonth");
            setYearCollection("selOrgFOWSTTRegExpirationYear");

            setMonthCollection("selOrgFOWSTBPermitDateOfPaymentMonth");
            setYearCollection("selOrgFOWSTBPermitDateOfPaymentYear");

            setMonthCollection("selOrgFOWSTBPermitBPlateIssuedMonth");
            setYearCollection("selOrgFOWSTBPermitBPlateIssuedYear");

            setHoursCollection("selOrgFOWSTOptOpenHR");
            setMinutesCollection("selOrgFOWSTOptOpenMin");
            setHoursCollection("selOrgFOWSTOptCloseHR");
            setMinutesCollection("selOrgFOWSTOptCloseMin");
            getOrgAddTradeName(function () {
                getOrgReceiptSettings(function () {
                    getOrgAddAssignedDevices(function () {
                        getOrgAddFowstType(function () {
                            getOrgAddWarehouseType(function () {
                                getOrgAddRetailStoreType(function () {
                                    getOrgAddStoreLocationType(function () {
                                        getOrgFOWSTQuota(function () {
                                            getFOWSTAssignedEmployees(function () {

                                            });
                                        });
                                    });
                                });
                            });
                        });
                    })
                })
            });

            $.mobile.activePage.bind('pageshow', PageShow);
            function PageShow() {
                mLoaded = false;
                //getOrgFOWSTList();
                lbllibname = sessionStorage.LabelLibraryName;
                libname = sessionStorage.LibraryName;
                addlibname = sessionStorage.AddViewLibBCName;
                fowstid = sessionStorage.ViewID;
                orgname = sessionStorage["OrgName"];
                peopleid = sessionStorage["PeopleID"];
                $('#txtFowstTitle').text(sessionStorage.FowstTitle);
                ShowOrgFOWSTInfo(sessionStorage.ViewID || '');
                HideDivExcludedMC();
            };
            if (!mLoaded) PageShow();

            //Navigation Menu
            $('#btnHome').click(function (e) {
                e.preventDefault();

                window.open(GetPathFormat("/People/PeopleMenu"), "_self");
            });

            $('#btnSystemAdmin').click(function (e) {
                e.preventDefault();

                window.open(GetPathFormat("/People/PeopleMenu#pagePopUpMenuSettings"), "_self");
            });

            $('#btnDataLibraries').click(function (e) {
                e.preventDefault();

                window.open(GetPathFormat("/People/PeopleMenu#pagePopDataLibraries"), "_self");
            });

            $('#selIsDispatchPrinter').change(function () {
                if (CBool($('#selIsDispatchPrinter').is(':checked'))) {
                    IsDefaultDispatchPrinter = true;
                    if ($('#selConnectionType').val() != 'TCP/IP') {
                        NewMessageBox('This feature is only availble for TCP/IP or Ethernet Printers.', 'Dispatch Printer')
                        IsDefaultDispatchPrinter = false;
                        try {
                            $('#selIsDispatchPrinter').prop('checked', 0);//.slider('refresh');
                        } catch (e) {
                            $('#selIsDispatchPrinter').prop('checked', 0);//.slider();
                        }

                    }
                } else {
                    IsDefaultDispatchPrinter = false;
                }
            })
            //End Navigation Menu            

            window.Fowst = {
                ShowFowstInfo: function (strid) {
                    ClearOrgFOWSTFields();
                    $('#txtFowstTitle').text(sessionStorage.FowstTitle);
                    setMonthCollection("selOrgFOWSTContractStartMonth");
                    setYearCollection("selOrgFOWSTContractStartYear");

                    setMonthCollection("selOrgFOWSTContractEndMonth");
                    setYearCollection("selOrgFOWSTContractEndYear");

                    setMonthCollection("selOrgFOWSTTRegIssuedMonth");
                    setYearCollection("selOrgFOWSTTRegIssuedYear");

                    setMonthCollection("selOrgFOWSTTRegExpirationMonth");
                    setYearCollection("selOrgFOWSTTRegExpirationYear");

                    setMonthCollection("selOrgFOWSTBPermitDateOfPaymentMonth");
                    setYearCollection("selOrgFOWSTBPermitDateOfPaymentYear");

                    setMonthCollection("selOrgFOWSTBPermitBPlateIssuedMonth");
                    setYearCollection("selOrgFOWSTBPermitBPlateIssuedYear");

                    setHoursCollection("selOrgFOWSTOptOpenHR");
                    setMinutesCollection("selOrgFOWSTOptOpenMin");
                    setHoursCollection("selOrgFOWSTOptCloseHR");
                    setMinutesCollection("selOrgFOWSTOptCloseMin");

                    getOrgAddTradeName(function () {
                        getOrgReceiptSettings(function () {
                            getOrgAddAssignedDevices(function () {
                                getOrgAddFowstType(function () {
                                    getOrgAddWarehouseType(function () {
                                        getOrgAddRetailStoreType(function () {
                                            getOrgAddStoreLocationType(function () {
                                                getOrgFOWSTQuota(function () {
                                                    getFOWSTAssignedEmployees(function () {
                                                        lbllibname = sessionStorage.LabelLibraryName;
                                                        libname = sessionStorage.LibraryName;
                                                        addlibname = sessionStorage.AddViewLibBCName;
                                                        fowstid = sessionStorage.ViewID;
                                                        setTimeout(function () {
                                                            ShowOrgFOWSTInfo(fowstid || '');
                                                        }, 1000);
                                                        HideDivExcludedMC();
                                                    })                                                    
                                                });
                                            });
                                        });
                                    });
                                });
                            })
                        });
                    });
                }
            }

            $('body').on('click', '#btnOrgFOWSTCity', function () {
                BrowsePopup('LibraryCities', 'City', [
                    {
                        DisplayText: [
                            {
                                Column: 'Name'
                            }
                        ],
                        Click: function (vData) {
                            $('#btnOrgFOWSTCity').text(vData.Name);//.button();//.button('refresh');
                            $('#txtOrgFOWSTCityID').val(vData.ID);
                        }
                    }
                ])
            })
            .on('change', '[name=radPosVendor]', function() {
                if ($('[name=radPosVendor]:checked').val())
                    $('#div3rdPartyPosVendor').show();
                else
                    $('#div3rdPartyPosVendor').hide();
            })
            .on('click', '#butPosVendor', function () {
                BrowsePopup('LibraryPosVendors', 'POS Vendor',
                    [
                        {
                            DisplayText:[{Column: 'Name'}],
                            Click: function(e){
                                mPosVendorID = e.ID;
                                $('#butPosVendor').text(e.Name);//.button('refresh');
                            }
                        }
                    ])
            })
            .on('click', '#btnOrgFOWSTCountry', function () {
                BrowsePopup('LibraryCountries', 'Country',
                    [
                        {
                            DisplayText:[{Column: 'Name'}],
                            Click: function(e){
                                $('#txtOrgFOWSTCountryID').val(e.ID);
                                $('#btnOrgFOWSTCountry').text(e.Name);//.button('refresh');
                            }
                        }
                    ])
            })
            .on('click', '#btnAddOrgFOWSTBusinessPermit', function () {
                //$.mobile.changePage('#pageOrgFOWSTBusinessPermitInfo');
                $('body').animate({ scrollTop: 0 });
                $('#pageOrgFOWSTInfo').hide();
                $('#pageOrgFOWSTTruckRegInfo').hide();
                $('#pageOrgFOWSTSectionInfo').hide();
                $('#pageOrgFOWSTBusinessPermitInfo').show();
                $('#pageOrgFOWSTTelInfo').hide();
                $('#pageOrgFOWSTMobileInfo').hide();
                $('#pageOrgFOWSTEmailInfo').hide();
                $('#pageOrgFOWSTOperationHRInfo').hide();
                $('#pageOrgFOWSTPrinterInfo').hide();
                $('#pageOrgFOWSTPrinterIP').hide(); $('#pageAddDevicePrinter').hide();
                $('body').animate({ scrollTop: 0 }, 'fast');

                IsNewOrgAddBPermit = true
                intOrgFowstBPermitCntr = intOrgFowstBPermitCntr + 1;
                intOrgFowstBPermitIndex = intOrgFowstBPermitCntr;

                ClearOrgAddBPermitFields();
            })
            .on('click', '#btnOrgFOWSTBPermitCity', function () {
                BrowsePopup('LibraryCities', 'City',
                    [
                        {
                            DisplayText:[{Column: 'Name'}],
                            Click: function(e){
                                $('#txtOrgFOWSTBPermitCityID').val(e.ID);
                                $('#btnOrgFOWSTBPermitCity').text(e.Name);//.button('refresh');
                            }
                        }
                    ])
            })
            .on('click', '#btnOrgFOWSTBPermitCountry', function () {
                BrowsePopup('LibraryCountries', 'Country',
                    [
                        {
                            DisplayText:[{Column: 'Name'}],
                            Click: function(e){
                                $('#txtOrgFOWSTBPermitCountryID').val(e.ID);
                                $('#btnOrgFOWSTBPermitCountry').text(e.Name);//.button('refresh');
                            }
                        }
                    ])
            })
            .on('click', '#btnOrgFOWSTBPermitSave', function () {
                if ($.trim($("#txtOrgFOWSTBPermitNo").val()) == "") {
                    NewMessageBox('Please enter business permit number.', 'Business Permit No.')
                    return;
                }

                if ($.trim($("#txtOrgFOWSTBPermitTradeName").val()) == "") {
                    NewMessageBox('Please enter trade name.', 'Trade Name');
                    return;
                }

                if ($.trim($("#txtOrgFOWSTBPermitStreet").val()) == "") {
                    NewMessageBox('Please enter street address.', 'Street Address');
                    return;
                }

                if ($.trim($("#txtOrgFOWSTBPermitCityID").val()) == "") {
                    NewMessageBox('Please select city address.', 'City Address');
                    return;
                }

                if ($.trim($("#txtOrgFOWSTBPermitCountryID").val()) == "") {
                    NewMessageBox('Please select country address.', 'Country Address');
                    return;
                }

                if ($.trim($("#txtOrgFOWSTBPermitBPlateNo").val()) == "") {
                    NewMessageBox('Please enter business plate number.', 'Business Plate No.');
                    return;
                }

                if ($.trim($("#selOrgFOWSTBPermitDateOfPaymentMonth").val()) != "" ||
                    $.trim($("#selOrgFOWSTBPermitDateOfPaymentDay").val()) != "" ||
                    $.trim($("#selOrgFOWSTBPermitDateOfPaymentYear").val()) != "") {

                    if ($.trim($("#selOrgFOWSTBPermitDateOfPaymentMonth").val()) == "" ||
                        $.trim($("#selOrgFOWSTBPermitDateOfPaymentDay").val()) == "" ||
                        $.trim($("#selOrgFOWSTBPermitDateOfPaymentYear").val()) == "") {
                        NewMessageBox('Please enter valid date of payment.', 'Date Of Payment');
                        return;
                    }

                }

                if ($.trim($("#selOrgFOWSTBPermitBPlateIssuedMonth").val()) != "" ||
                    $.trim($("#selOrgFOWSTBPermitBPlateIssuedDay").val()) != "" ||
                    $.trim($("#selOrgFOWSTBPermitBPlateIssuedYear").val()) != "") {

                    if ($.trim($("#selOrgFOWSTBPermitBPlateIssuedMonth").val()) == "" ||
                        $.trim($("#selOrgFOWSTBPermitBPlateIssuedDay").val()) == "" ||
                        $.trim($("#selOrgFOWSTBPermitBPlateIssuedYear").val()) == "") {
                        NewMessageBox('Please enter valid issued date.', 'Issued Date');
                        return;
                    }

                }

                SaveOrgFOWSTBPermit();
            })
            .on('click', '#btnOrgFOWSTBPermitRemove', function () {
                NewMessageBox('Remove this record?', 'Remove',
                    [
                        {
                            Text: 'Yes',
                            Click: function(){
                                RemoveOrgFOWSTBPermit();
                            }
                        },
                        {   Text: 'No'}
                    ])
            })
            .on('click', '#btnOrgFOWSTBPermitCancel', function () {
                //$.mobile.changePage('#pageOrgFOWSTInfo');
                $('#pageOrgFOWSTInfo').show();
                $('#pageOrgFOWSTTruckRegInfo').hide();
                $('#pageOrgFOWSTSectionInfo').hide();
                $('#pageOrgFOWSTBusinessPermitInfo').hide();
                $('#pageOrgFOWSTTelInfo').hide();
                $('#pageOrgFOWSTMobileInfo').hide();
                $('#pageOrgFOWSTEmailInfo').hide();
                $('#pageOrgFOWSTOperationHRInfo').hide();
                $('#pageOrgFOWSTPrinterInfo').hide();
                $('#pageOrgFOWSTPrinterIP').hide(); $('#pageAddDevicePrinter').hide();
            })
            .on('click', '#btnAddOrgFOWSTTelephone', function () {
                //$.mobile.changePage('#pageOrgFOWSTTelInfo');
                $('body').animate({ scrollTop: 0 });
                $('#pageOrgFOWSTInfo').hide();
                $('#pageOrgFOWSTTruckRegInfo').hide();
                $('#pageOrgFOWSTSectionInfo').hide();
                $('#pageOrgFOWSTBusinessPermitInfo').hide();
                $('#pageOrgFOWSTTelInfo').show();
                $('#pageOrgFOWSTMobileInfo').hide();
                $('#pageOrgFOWSTEmailInfo').hide();
                $('#pageOrgFOWSTOperationHRInfo').hide();
                $('#pageOrgFOWSTPrinterInfo').hide();
                $('#pageOrgFOWSTPrinterIP').hide(); $('#pageAddDevicePrinter').hide();
                $('body').animate({ scrollTop: 0 }, 'fast');

                IsNewOrgAddTelContact = true
                intOrgFowstTelCntr = intOrgFowstTelCntr + 1;
                intOrgFowstTelIndex = intOrgFowstTelCntr;

                ClearOrgAddTelFields();
            })
            .on('click', '#btnOrgFOWSTTelCountryCode', function () {
                BrowsePopup('LibraryCountries', 'Country',
                    [
                        {
                            DisplayText: [{ Column: 'Name' }],
                            AutoSearch: true,
                            Click: function(e){
                                $('#txtOrgFOWSTTelCountryID').val(e.ID);
                                $('#btnOrgFOWSTTelCountryCode').text(e.Code + ' - ' + e.CallingCode);//.button('refresh');
                            }
                        }
                    ])
            })
            .on('click', '#btnOrgFOWSTTelAreaCode', function () {
                BrowsePopup('LibraryAreaCodes', 'Area Code',
                    [
                        {
                            DisplayText: [{ Column: 'Area' }, { Label: 'Code', Column: 'CallingCode' }],
                            AutoSearch: true,
                            Click: function(e){
                                $('#txtOrgFOWSTTelAreaCodeID').val(e.ID);
                                $('#btnOrgFOWSTTelAreaCode').text(e.Area + ' - ' + e.CallingCode);//.button('refresh');
                            }
                        }
                    ])
            })
            .on('click', '#btnOrgFOWSTMobileCountryCode', function () {
                BrowsePopup('LibraryCountries', 'Country',
                    [
                        {
                            DisplayText: [{ Column: 'Name' }],
                            AutoSearch: true,
                            Click: function(e){
                                $('#txtOrgFOWSTMobileCountryID').val(e.ID);
                                $('#btnOrgFOWSTMobileCountryCode').text(e.Code + ' - ' + e.CallingCode);//.button('refresh');
                            }
                        }
                    ])
            })
            .on('click', '#btnOrgFOWSTMobileTelCoPrefixCode', function () {
                BrowsePopup('LibraryTelcoPrefixes', 'Telco Code',
                    [
                        {
                            DisplayText: [{ Column: 'TelCo' }, { Label: 'Prefix', Column: 'Prefix' }],
                            AutoSearch: true,
                            Click: function(e){
                                $('#txtOrgFOWSTTelCoPrefixID').val(e.ID);
                                $('#btnOrgFOWSTMobileTelCoPrefixCode').text(e.TelCo + ' - ' + e.Prefix);//.button('refresh');
                            }
                        }
                    ])
            })
            .on('click', '#btnAddOrgFOWSTEmail', function () {
                //$.mobile.changePage('#pageOrgFOWSTEmailInfo');
                $('body').animate({ scrollTop: 0 });
                $('#pageOrgFOWSTInfo').hide();
                $('#pageOrgFOWSTTruckRegInfo').hide();
                $('#pageOrgFOWSTSectionInfo').hide();
                $('#pageOrgFOWSTBusinessPermitInfo').hide();
                $('#pageOrgFOWSTTelInfo').hide();
                $('#pageOrgFOWSTMobileInfo').hide();
                $('#pageOrgFOWSTEmailInfo').show();
                $('#pageOrgFOWSTOperationHRInfo').hide();
                $('#pageOrgFOWSTPrinterInfo').hide();
                $('#pageOrgFOWSTPrinterIP').hide(); $('#pageAddDevicePrinter').hide();
                $('body').animate({ scrollTop: 0 }, 'fast');

                IsNewOrgAddEmailContact = true
                intOrgFowstEmailCntr = intOrgFowstEmailCntr + 1;
                intOrgFowstEmailIndex = intOrgFowstEmailCntr;

                ClearOrgAddEmailFields();
            })
            .on('click', '#btnOrgFOWSTEmailSave', function () {
                if ($.trim($("#txtOrgFOWSTEmail").val()) == "") {
                    NewMessageBox('Please enter email address.', 'Email Address');
                    return;
                }

                if (IsEmail($.trim($("#txtOrgFOWSTEmail").val())) == false) {
                    NewMessageBox('Please enter valid email address.', 'Email Address');
                    return;
                }

                if ($.trim($("#selOrgFOWSTEmailType").val()) == "") {
                    NewMessageBox('Please select email type.', 'Email Type');
                    return;
                }

                SaveOrgFOWSTEmail();
            })
            .on('click', '#btnOrgFOWSTEmailRemove', function () {
                NewMessageBox('Remove this record?', 'Remove',
                    [
                        {
                            Text: 'Yes',
                            Click: function(){
                                RemoveOrgFOWSTEmail();
                            }
                        },
                        {   Text: 'No'}
                    ])
            })
            .on('click', '#btnOrgFOWSTEmailCancel', function () {
                //$.mobile.changePage('#pageOrgFOWSTInfo');
                $('#pageOrgFOWSTInfo').show();
                $('#pageOrgFOWSTTruckRegInfo').hide();
                $('#pageOrgFOWSTSectionInfo').hide();
                $('#pageOrgFOWSTBusinessPermitInfo').hide();
                $('#pageOrgFOWSTTelInfo').hide();
                $('#pageOrgFOWSTMobileInfo').hide();
                $('#pageOrgFOWSTEmailInfo').hide();
                $('#pageOrgFOWSTOperationHRInfo').hide();
                $('#pageOrgFOWSTPrinterInfo').hide();
                $('#pageOrgFOWSTPrinterIP').hide(); $('#pageAddDevicePrinter').hide();
            })
            .on('click', '#btnAddOrgFOWSTPrinter', function () {
                //$.mobile.changePage('#pageOrgFOWSTPrinterInfo');
                $('body').animate({ scrollTop: 0 });
                $('#pageOrgFOWSTInfo').hide();
                $('#pageOrgFOWSTTruckRegInfo').hide();
                $('#pageOrgFOWSTSectionInfo').hide();
                $('#pageOrgFOWSTBusinessPermitInfo').hide();
                $('#pageOrgFOWSTTelInfo').hide();
                $('#pageOrgFOWSTMobileInfo').hide();
                $('#pageOrgFOWSTEmailInfo').hide();
                $('#pageOrgFOWSTOperationHRInfo').hide();
                $('#pageOrgFOWSTPrinterInfo').show();
                $('#pageOrgFOWSTPrinterIP').hide(); $('#pageAddDevicePrinter').hide();
                $('body').animate({ scrollTop: 0 }, 'fast');

                IsNewPrinter = true
                intOrgFowstPrinterCntr = intOrgFowstPrinterCntr + 1;
                intOrgFowstPrinterIndex = intOrgFowstPrinterCntr;

                ClearOrgAddPrinterFields();

                GetAllFOWSTPrinterAvailable();
            })
            .on('click', '#btnOrgFOWSTPrinterCancel', function () {
                $('#pageOrgFOWSTInfo').hide();
                $('#pageOrgFOWSTTruckRegInfo').hide();
                $('#pageOrgFOWSTSectionInfo').hide();
                $('#pageOrgFOWSTBusinessPermitInfo').hide();
                $('#pageOrgFOWSTTelInfo').hide();
                $('#pageOrgFOWSTMobileInfo').hide();
                $('#pageOrgFOWSTEmailInfo').hide();
                $('#pageOrgFOWSTOperationHRInfo').hide();                
                $('#pageOrgFOWSTPrinterIP').hide(); $('#pageAddDevicePrinter').hide();
                if (CStr2($("#txtPrinterFowstID").val()) == '') {
                    $('#pageOrgFOWSTPrinterInfo').show();
                } else {
                    $('#pageOrgFOWSTInfo').show();
                }
            })
            .on('click', '#btnOrgFOWSTPrinterSave', function () {
                if ($.trim($("#txtPrinterName").val()) == "") {
                    NewMessageBox('Please enter printer name.', 'Printer Name');
                    return;
                }

                if ($('#selConnectionType').val() == 'TCP/IP') {
                    if ($.trim($("#txtPrinterIPAddress").val()) == "") {
                        NewMessageBox('Please enter printer IP address.', 'Printer IP Address');
                        return;
                    } else {

                        if (!validateIP($.trim($("#txtPrinterIPAddress").val()))) {
                            NewMessageBox('Please enter valid IP address.', 'Printer IP Address');
                            return;

                        }
                    }
                }
                
                if (CBool($('#selIsDispatchPrinter').is(':checked'))) {
                    AjaxSend({
                        url: 'DevicePrinter/CheckDefaultDispatchPrinter',
                        success: function (vData) {
                            if (vData.Results.Rows.length > 0) {
                                NewMessageBox('There is a default dispatch printer assigned to this FOWST(' + vData.Results.Rows[0].PrinterName + '). Would you like to set this printer as new default dispatch printer?', 'Replace dispatch printer',
                                    [
                                        {
                                            Text: 'Yes',
                                            Click: function () {
                                                IsDefaultDispatchPrinter = true;
                                                SaveFOWSTPrinterIP();
                                            }
                                        },
                                        {
                                            Text: 'No',
                                            Click: function () {
                                                SaveFOWSTPrinterIP();
                                            }
                                        }
                                    ])
                            } else {
                                IsDefaultDispatchPrinter = true;
                                SaveFOWSTPrinterIP();
                            }
                        }
                    });
                } else {
                    SaveFOWSTPrinterIP();
                }
                
            })
            .on('click', '#btnOrgFOWSTPrinterRemove', function () {
                NewMessageBox('Are you sure you want to remove this printer?', 'Remove Printer From FOWST',
                    [
                        {
                            Text: 'Yes',
                            Click: function(){
                                RemoveOrgFOWSTPrinter();
                            }
                        },
                        {   Text: 'No'}
                    ])
            })
            .on('click', '#btnAddNewDevicePrinterOrg', function () {
                CreateNewDevicePrinterEntryOrg(1, '');
                $('#pageOrgFOWSTInfo').hide();
                $('#pageOrgFOWSTTruckRegInfo').hide();
                $('#pageOrgFOWSTSectionInfo').hide();
                $('#pageOrgFOWSTBusinessPermitInfo').hide();
                $('#pageOrgFOWSTTelInfo').hide();
                $('#pageOrgFOWSTMobileInfo').hide();
                $('#pageOrgFOWSTEmailInfo').hide();
                $('#pageOrgFOWSTOperationHRInfo').hide();
                $('#pageOrgFOWSTPrinterInfo').hide();
                $('#pageOrgFOWSTPrinterIP').hide();
                $('#pageAddDevicePrinter').show();
                sessionStorage["DevicePrinterInfoUsedby"] = "FOWST";
            })
            .on('click', '#btnDevicePrinterBackToHome', function () {
                //window.open(GetPathFormat("/People/PeopleMenu"), "_self");
                $('#pageOrgFOWSTInfo').show();
                $('#pageOrgFOWSTTruckRegInfo').hide();
                $('#pageOrgFOWSTSectionInfo').hide();
                $('#pageOrgFOWSTBusinessPermitInfo').hide();
                $('#pageOrgFOWSTTelInfo').hide();
                $('#pageOrgFOWSTMobileInfo').hide();
                $('#pageOrgFOWSTEmailInfo').hide();
                $('#pageOrgFOWSTOperationHRInfo').hide();
                $('#pageOrgFOWSTPrinterInfo').hide();
                $('#pageOrgFOWSTPrinterIP').hide();
                $('#pageAddDevicePrinter').hide();
            })
            .on('click', '#btnOrgFOWSTSave', function () {
                if ($.trim($("#selOrgFOWSTType").val()) == "") {
                    NewMessageBox('Please select organization type', 'Organization');
                    return;
                }

                if ($.trim($("#txtOrgFOWSTName").val()) == "") {
                    NewMessageBox('Please enter fowst name', 'Organization');
                    return;
                }

                if ($.trim($("#selOrgFOWSTTradeName").val()) == "") {
                    NewMessageBox('Please select business or trade name.', 'Organization');
                    return;
                }

                if ($.trim($("#selOrgFOWSTReceiptSettings").val()) == "") {
                    NewMessageBox('Please select receipt setting.', 'Organization');
                    return;
                }

                if ($.trim($("#txtOrgFOWSTStreet").val()) == "") {
                    NewMessageBox('Please enter street address', 'Organization');
                    return;
                }

                if ($.trim($("#txtOrgFOWSTCityID").val()) == "") {
                    NewMessageBox('Please select city address', 'Organization');
                    return;
                }

                if ($.trim($("#selOrgFOWSTRegion").val()) == "") {
                    NewMessageBox('Please select your region', 'Organization');
                    return;
                }

                if ($.trim($("#txtOrgFOWSTCountryID").val()) == "") {
                    NewMessageBox('Please select country address', 'Organization');
                    return;
                }

                if ($("#txtTINBranchCode").val().length < 5) {
                    NewMessageBox('Please enter the 5-digit branch code.', 'Organization');
                    return;
                }

                //Franchisee
                if ($("#selOrgAddFranchisee").is(':checked')) {

                    if ($.trim($("#txtOrgFOWSTFranchisorID").val()) == "") {
                        NewMessageBox('Please select franchisor.', 'Franchisor');
                        return;
                    }

                }
                else {
                    $("#txtOrgFOWSTFranchisorID").val('');
                    $("#btnOrgFOWSTFranchisor").text('Please Select');//.button('refresh');
                }

                //Ownership Type
                if ($.trim($("#selOrgFOWSTOwnershipType").val()) == "1") {

                    if ($.trim($("#selOrgFOWSTContractStartMonth").val()) == "" ||
                        $.trim($("#selOrgFOWSTContractStartDay").val()) == "" ||
                        $.trim($("#selOrgFOWSTContractStartYear").val()) == "") {
                        NewMessageBox('Please set the rent or leased contract start date.', 'Contract Start Date');
                        return;
                    }

                    if ($.trim($("#selOrgFOWSTContractEndMonth").val()) == "" ||
                        $.trim($("#selOrgFOWSTContractEndDay").val()) == "" ||
                        $.trim($("#selOrgFOWSTContractEndYear").val()) == "") {
                        NewMessageBox('Please set the rent or leased contract end date.', 'Contract End Date');
                        return;
                    }
                }
                SaveOrgFOWST();
            })
            .on('click', '#btnOrgFOWSTCancel', function () {
                //ChangePage('/Organization/OrganizationAddress');
                //window.location = MergeUrlParameters({
                //    OrgID: sessionStorage.OrgID,
                //    IsMobile: CBool(IsMobile) ? 1 : 0
                //}, '/Organization/OrganizationAddress');
                window.location.hash = 'LibraryPage';
            })
            .on('keypress', '#txtSearchKeyword', function (e) {
                if (e.keyCode == 13) {
                    if ($('#TitleLibrary').text() == "CITY") {
                        SearchCityPopUp($(this).val());
                    }
                    else if ($('#TitleLibrary').text() == "COUNTRY") {
                        SearchCountryPopUp($(this).val());
                    }
                    else if ($('#TitleLibrary').text() == "AREA CODE") {
                        SearchAreaCodePopUp($(this).val());
                    }
                    else if ($('#TitleLibrary').text() == "TELCO CODE") {
                        SearchTelcoCodePopUp($(this).val());
                    }
                    else if ($('#TitleLibrary').text() == "FRANCHISOR") {
                        SearchFranchisorPopUp($(this).val());
                    }
                    else if ($('#TitleLibrary').text() == "BIN") {
                        SearchBinPopUp($(this).val());
                    }
                    else if ($('#TitleLibrary').text() == "SECTION PICKER") {
                        SearchWarehousePickerPopUp($(this).val());
                    }

                }
            })
            .on('click', '#btnOrgFOWSTTelCancel', function () {
                //$.mobile.changePage('#pageOrgFOWSTInfo');
                $('#pageOrgFOWSTInfo').show();
                $('#pageOrgFOWSTTruckRegInfo').hide();
                $('#pageOrgFOWSTSectionInfo').hide();
                $('#pageOrgFOWSTBusinessPermitInfo').hide();
                $('#pageOrgFOWSTTelInfo').hide();
                $('#pageOrgFOWSTMobileInfo').hide();
                $('#pageOrgFOWSTEmailInfo').hide();
                $('#pageOrgFOWSTOperationHRInfo').hide();
                $('#pageOrgFOWSTPrinterInfo').hide();
                $('#pageOrgFOWSTPrinterIP').hide(); $('#pageAddDevicePrinter').hide();
            })
            .on('click', '#btnOrgFOWSTTelSave', function () {
                if ($.trim($("#txtOrgFOWSTTelCountryID").val()) == "") {
                    NewMessageBox('Please select country code.', 'Country Code');
                    return;
                }

                if ($.trim($("#txtOrgFOWSTTelAreaCodeID").val()) == "") {
                    NewMessageBox('Please select area code.', 'Area Code');
                    return;
                }

                if ($.trim($("#txtOrgFOWSTTelNo").val()) == "") {
                    NewMessageBox('Please enter 7 digit telephone number.', 'Telephone Number');
                    return;
                }

                if ($.trim($("#txtOrgFOWSTTelNo").val()).length != 7) {
                    NewMessageBox('Please enter 7 digit telephone number.', 'Telephone Number');
                    return;
                }

                if (!$.isNumeric($("#txtOrgFOWSTTelNo").val())) {
                    NewMessageBox('Please enter valid telephone number.', 'Telephone Number');
                    return;
                }

                if ($.trim($("#txtOrgFOWSTTelExtension").val()).length > 0) {
                    if (!$.isNumeric($("#txtOrgFOWSTTelExtension").val())) {
                        NewMessageBox('Please enter valid extension or local number.', 'Telephone Number');
                        return;
                    }
                }

                if ($.trim($("#selOrgFOWSTTelType").val()) == "") {
                    NewMessageBox('Please select telephone contact type.', 'Telephone Contact Type');
                    return;
                }

                SaveOrgFOWSTTel();
            })
            .on('click', '#btnOrgFOWSTAdd', function () {
                //$.mobile.changePage('#pageOrgFOWSTInfo');
                $('#pageOrgFOWSTInfo').show();
                $('#pageOrgFOWSTTruckRegInfo').hide();
                $('#pageOrgFOWSTSectionInfo').hide();
                $('#pageOrgFOWSTBusinessPermitInfo').hide();
                $('#pageOrgFOWSTTelInfo').hide();
                $('#pageOrgFOWSTMobileInfo').hide();
                $('#pageOrgFOWSTEmailInfo').hide();
                $('#pageOrgFOWSTOperationHRInfo').hide();
                $('#pageOrgFOWSTPrinterInfo').hide();
                $('#pageOrgFOWSTPrinterIP').hide(); $('#pageAddDevicePrinter').hide();
                ClearOrgFOWSTFields();
            })
            .on('change', '#selOrgFOWSTType', function () {
                SetOrgAddSubPropertyAndPLantType();
                HideDivExcludedMC();
            })
            .on('change', '#selOrgAddFranchisee', function () {
                ShowOrgAddHideFranchisorDetails();
            })
            .on('click', '#btnOrgFOWSTFranchisor', function () {
                BrowsePopup('LibraryOrganization', 'Franchisor',
                    [
                        {
                            DisplayText: [{Column: 'CompanyName'}],
                            Click: function(e){
                                $('#txtOrgFOWSTFranchisorID').val(e.ID);
                                $('#btnOrgFOWSTFranchisor').text(e.CompanyName);//.button('refresh');
                            }
                        }
                    ],{FilterFranchisor : 1});
            })
            .on('change', '#selOrgFOWSTOwnershipType', function () {
                SetOrgAddShowHideRentedInfo();
            })
            .on('change', '#selOrgFOWSTContractStartMonth', function () {
                setDayCollection('selOrgFOWSTContractStartMonth', 'selOrgFOWSTContractStartDay', 'selOrgFOWSTContractStartYear', false);
            })
            .on('change', '#selOrgFOWSTContractStartYear', function () {
                setDayCollection('selOrgFOWSTContractStartMonth', 'selOrgFOWSTContractStartDay', 'selOrgFOWSTContractStartYear', false);
            })
            .on('click', '#selOrgFOWSTContractStartYear', function () {
                if ($.trim($('#selOrgFOWSTContractStartYear').val()) == "") {
                    $('#selOrgFOWSTContractStartYear').val('2012');
                    $('#selOrgFOWSTContractStartYear');//.selectmenu('refresh');
                }
            })
            .on('change', '#selOrgFOWSTContractEndMonth', function () {
                setDayCollection('selOrgFOWSTContractEndMonth', 'selOrgFOWSTContractEndDay', 'selOrgFOWSTContractEndYear', false);
            })
            .on('change', '#selOrgFOWSTContractEndYear', function () {
                setDayCollection('selOrgFOWSTContractEndMonth', 'selOrgFOWSTContractEndDay', 'selOrgFOWSTContractEndYear', false);
            })
            .on('click', '#selOrgFOWSTContractEndYear', function () {
                if ($.trim($('#selOrgFOWSTContractEndYear').val()) == "") {
                    $('#selOrgFOWSTContractEndYear').val('2012');
                    $('#selOrgFOWSTContractEndYear');//.selectmenu('refresh');
                }
            })
            .on('change', '#selOrgFOWSTInsideMall', function () {
                if ($("#selOrgFOWSTInsideMall").is(':checked')) {
                    $("#divOrgFOWSTInsideMall").show();
                }
                else {
                    $("#divOrgFOWSTInsideMall").hide();
                }
            })
            .on('change', '#selOrgFOWSTMallName', function () {
                SetOrgAddShowHideMallInfo();
            })
            .on('keydown', '#txtOrgFOWSTSMMallClassCode', function (e) {
                if (!NumberOnly(e)) {
                    return;
                }
            })
            .on('keydown', '#txtOrgFOWSTSMMallOutletNo', function (e) {
                if (!NumberOnly(e)) {
                    return;
                }
            })
            .on('click', '#btnAddOrgFOWSTLTO', function () {
                intOrgFowstIsLTO = true;
                //$.mobile.changePage('#pageOrgFOWSTTruckRegInfo');
                $('body').animate({ scrollTop: 0 });
                $('#pageOrgFOWSTInfo').hide();
                $('#pageOrgFOWSTTruckRegInfo').show();
                $('#pageOrgFOWSTSectionInfo').hide();
                $('#pageOrgFOWSTBusinessPermitInfo').hide();
                $('#pageOrgFOWSTTelInfo').hide();
                $('#pageOrgFOWSTMobileInfo').hide();
                $('#pageOrgFOWSTEmailInfo').hide();
                $('#pageOrgFOWSTOperationHRInfo').hide();
                $('#pageOrgFOWSTPrinterInfo').hide();
                $('#pageOrgFOWSTPrinterIP').hide(); $('#pageAddDevicePrinter').hide();

                IsNewOrgAddLTO = true
                intOrgFowstLTOCntr = intOrgFowstLTOCntr + 1;
                intOrgFowstLTOIndex = intOrgFowstLTOCntr;

                ClearOrgAddTruckFields();
            })
            .on('change', '#selOrgFOWSTBPermitDateOfPaymentMonth', function () {
                setDayCollection('selOrgFOWSTBPermitDateOfPaymentMonth', 'selOrgFOWSTBPermitDateOfPaymentDay', 'selOrgFOWSTBPermitDateOfPaymentYear', true);
            })
            .on('change', '#selOrgFOWSTBPermitDateOfPaymentYear', function () {
                setDayCollection('selOrgFOWSTBPermitDateOfPaymentMonth', 'selOrgFOWSTBPermitDateOfPaymentDay', 'selOrgFOWSTBPermitDateOfPaymentYear', true);
            })
            .on('click', '#selOrgFOWSTBPermitDateOfPaymentYear', function () {
                if ($.trim($('#selOrgFOWSTBPermitDateOfPaymentYear').val()) == "") {
                    $('#selOrgFOWSTBPermitDateOfPaymentYear').val('2012');
                    $('#selOrgFOWSTBPermitDateOfPaymentYear');//.selectmenu('refresh');
                }
            })
            .on('change', '#selOrgFOWSTBPermitBPlateIssuedMonth', function () {
                setDayCollection('selOrgFOWSTBPermitBPlateIssuedMonth', 'selOrgFOWSTBPermitBPlateIssuedDay', 'selOrgFOWSTBPermitBPlateIssuedYear', true);
            })
            .on('change', '#selOrgFOWSTBPermitBPlateIssuedYear', function () {
                setDayCollection('selOrgFOWSTBPermitBPlateIssuedMonth', 'selOrgFOWSTBPermitBPlateIssuedDay', 'selOrgFOWSTBPermitBPlateIssuedYear', true);
            })
            .on('click', '#selOrgFOWSTBPermitBPlateIssuedYear', function () {
                if ($.trim($('#selOrgFOWSTBPermitBPlateIssuedYear').val()) == "") {
                    $('#selOrgFOWSTBPermitBPlateIssuedYear').val('2012');
                    $('#selOrgFOWSTBPermitBPlateIssuedYear');//.selectmenu('refresh');
                }
            })
            .on('keydown', '#txtOrgFOWSTTelNo', function (e) {
                if (!NumberOnly(e)) {
                    return;
                }
            })
            .on('keydown', '#txtOrgFOWSTTelExtension', function (e) {
                if (!NumberOnly(e)) {
                    return;
                }
            })
            .on('keydown', '#txtOrgFOWSTMobileNo', function (e) {
                if (!NumberOnly(e)) {
                    return;
                }
            })
            .on('keydown', '#txtOrgFOWSTMobileNo', function (e) {
                if (!NumberOnly(e)) {
                    return;
                }
            })
            .on('click', '#btnAddOrgFOWSTLTFRB', function () {
                intOrgFowstIsLTO = false;

                //$.mobile.changePage('#pageOrgFOWSTTruckRegInfo');
                $('body').animate({ scrollTop: 0 });
                $('#pageOrgFOWSTInfo').hide();
                $('#pageOrgFOWSTTruckRegInfo').show();
                $('#pageOrgFOWSTSectionInfo').hide();
                $('#pageOrgFOWSTBusinessPermitInfo').hide();
                $('#pageOrgFOWSTTelInfo').hide();
                $('#pageOrgFOWSTMobileInfo').hide();
                $('#pageOrgFOWSTEmailInfo').hide();
                $('#pageOrgFOWSTOperationHRInfo').hide();
                $('#pageOrgFOWSTPrinterInfo').hide();
                $('#pageOrgFOWSTPrinterIP').hide(); $('#pageAddDevicePrinter').hide();

                IsNewOrgAddLTFRB = true
                intOrgFowstLTFRBCntr = intOrgFowstLTFRBCntr + 1;
                intOrgFowstLTFRBIndex = intOrgFowstLTFRBCntr;

                ClearOrgAddTruckFields();
            })
            .on('click', '#btnAddOrgFOWSTSections', function () {
                //$.mobile.changePage('#pageOrgFOWSTSectionInfo');
                $('body').animate({ scrollTop: 0 });
                $('#pageOrgFOWSTInfo').hide();
                $('#pageOrgFOWSTTruckRegInfo').hide();
                $('#pageOrgFOWSTSectionInfo').show();
                $('#pageOrgFOWSTBusinessPermitInfo').hide();
                $('#pageOrgFOWSTTelInfo').hide();
                $('#pageOrgFOWSTMobileInfo').hide();
                $('#pageOrgFOWSTEmailInfo').hide();
                $('#pageOrgFOWSTOperationHRInfo').hide();
                $('#pageOrgFOWSTPrinterInfo').hide();
                $('#pageOrgFOWSTPrinterIP').hide(); $('#pageAddDevicePrinter').hide();

                IsNewOrgAddSection = true
                intOrgFowstSectionCntr = intOrgFowstSectionCntr + 1;
                intOrgFowstSectionIndex = intOrgFowstSectionCntr;

                ClearOrgAddSectionFields();
            })
            .on('click', '#btnAddOrgFOWSTSectionBins', function () {
                BrowsePopup('LibraryFowstBin', 'Bin',
                    [
                        {
                            DisplayText: [{ Column: 'Name'}],
                            Click: function (e) {
                                SaveFowstBinSection(e.ID);
                            }
                        }
                    ], { FowstID: fowstid }
                )
            })
            .on('click', '#btnAddOrgFOWSTSectionPeople', function () {
                 BrowsePopup('LibraryOrganizationEmployeeFOWST', 'Section Picker',
                    [
                        {
                            DisplayText: [{ Column: 'PeopleName'}],
                            Click: function (e) {
                                SaveFowstSectionPeople(e.ID);
                            }
                        }
                    ], { FowstID: fowstid }
                 )
             })

            .on('click', '#btnAddOrgFOWSTMobile', function () {
                //$.mobile.changePage('#pageOrgFOWSTMobileInfo');
                $('body').animate({ scrollTop: 0 });
                $('#pageOrgFOWSTInfo').hide();
                $('#pageOrgFOWSTTruckRegInfo').hide();
                $('#pageOrgFOWSTSectionInfo').hide();
                $('#pageOrgFOWSTBusinessPermitInfo').hide();
                $('#pageOrgFOWSTTelInfo').hide();
                $('#pageOrgFOWSTMobileInfo').show();
                $('#pageOrgFOWSTEmailInfo').hide();
                $('#pageOrgFOWSTOperationHRInfo').hide();
                $('#pageOrgFOWSTPrinterInfo').hide();
                $('#pageOrgFOWSTPrinterIP').hide(); $('#pageAddDevicePrinter').hide();
                $('body').animate({ scrollTop: 0 }, 'fast');

                IsNewOrgAddMobileContact = true
                intOrgFowstMobileCntr = intOrgFowstMobileCntr + 1;
                intOrgFowstMobileIndex = intOrgFowstMobileCntr;

                ClearOrgAddMobileFields();
            })
            .on('click', '#btnOrgFOWSTTRegSave', function () {
                if ($.trim($("#txtOrgFOWSTTRegORNo").val()) == "") {
                    NewMessageBox('Please enter OR number.', 'OR Number');
                    return;
                }

                if ($.trim($("#selOrgFOWSTTRegIssuedMonth").val()) != "" ||
                    $.trim($("#selOrgFOWSTTRegIssuedDay").val()) != "" ||
                    $.trim($("#selOrgFOWSTTRegIssuedYear").val()) != "") {

                    if ($.trim($("#selOrgFOWSTTRegIssuedMonth").val()) == "" ||
                        $.trim($("#selOrgFOWSTTRegIssuedDay").val()) == "" ||
                        $.trim($("#selOrgFOWSTTRegIssuedYear").val()) == "") {
                        NewMessageBox('Please enter valid issued date.', 'ssued Date');
                        return;
                    }

                }

                if ($.trim($("#selOrgFOWSTTRegExpirationMonth").val()) != "" ||
                    $.trim($("#selOrgFOWSTTRegExpirationDay").val()) != "" ||
                    $.trim($("#selOrgFOWSTTRegExpirationYear").val()) != "") {

                    if ($.trim($("#selOrgFOWSTTRegExpirationMonth").val()) == "" ||
                        $.trim($("#selOrgFOWSTTRegExpirationDay").val()) == "" ||
                        $.trim($("#selOrgFOWSTTRegExpirationYear").val()) == "") {
                        NewMessageBox('Please enter valid expriration date.', 'Expiration Date');
                        return;
                    }

                }

                if (intOrgFowstIsLTO) {
                    SaveOrgFOWSTLTO();
                }
                else {
                    SaveOrgFOWSTLTFRB();
                }
            })
            .on('click', '#btnOrgFOWSTTRegRemove', function () {
                NewMessageBox('Remove this record?', 'Remove',
                    [
                        {
                            Text: 'Yes',
                            Click: function(){
                                if (intOrgFowstIsLTO) {
                                    RemoveOrgFOWSTLTO();
                                }
                                else {
                                    RemoveOrgFOWSTLTFRB();
                                }
                            }
                        },
                        { Text: 'No'}
                    ])
            })
            .on('click', '#btnOrgFOWSTTRegCancel', function () {
                $('#pageOrgFOWSTInfo').show();
                $('#pageOrgFOWSTTruckRegInfo').hide();
                $('#pageOrgFOWSTSectionInfo').hide();
                $('#pageOrgFOWSTBusinessPermitInfo').hide();
                $('#pageOrgFOWSTTelInfo').hide();
                $('#pageOrgFOWSTMobileInfo').hide();
                $('#pageOrgFOWSTEmailInfo').hide();
                $('#pageOrgFOWSTOperationHRInfo').hide();
                $('#pageOrgFOWSTPrinterInfo').hide();
                $('#pageOrgFOWSTPrinterIP').hide(); $('#pageAddDevicePrinter').hide();
            })
            .on('click', '#btnOrgFOWSTSecSave', function () {
                if ($.trim($("#txtOrgFOWSTSecName").val()) == "") {
                    NewMessageBox('Please enter section name.', 'Section Name');
                    return;
                }

                SaveOrgFOWSTSection();
            })
            .on('click', '#btnOrgFOWSTSecRemove', function () {
                NewMessageBox('Remove this record?', 'Remove',
                    [
                        {
                            Text: 'Yes',
                            Click: function(){
                                RemoveOrgFOWSTSection();
                            }
                        },
                        { Text: 'No'}
                    ])
            })
            .on('click', '#btnOrgFOWSTSecCancel', function () {
                //$.mobile.changePage('#pageOrgFOWSTInfo');
                $('#pageOrgFOWSTInfo').show();
                $('#pageOrgFOWSTTruckRegInfo').hide();
                $('#pageOrgFOWSTSectionInfo').hide();
                $('#pageOrgFOWSTBusinessPermitInfo').hide();
                $('#pageOrgFOWSTTelInfo').hide();
                $('#pageOrgFOWSTMobileInfo').hide();
                $('#pageOrgFOWSTEmailInfo').hide();
                $('#pageOrgFOWSTOperationHRInfo').hide();
                $('#pageOrgFOWSTPrinterInfo').hide();
                $('#pageOrgFOWSTPrinterIP').hide(); $('#pageAddDevicePrinter').hide();
            })
            .on('click', '#btnOrgFOWSTTelRemove', function () {
                NewMessageBox('Remove this record?', 'Remove',
                    [
                        {
                            Text: 'Yes',
                            Click: function(){
                                RemoveOrgFOWSTTel();
                            }
                        },
                        { Text: 'No'}
                    ])
            })
            .on('click', '#btnOrgFOWSTMobileSave', function () {
                if ($.trim($("#txtOrgFOWSTMobileCountryID").val()) == "") {
                    NewMessageBox('Please select country code.', 'Country Code');
                    return;
                }

                if ($.trim($("#txtOrgFOWSTTelCoPrefixID").val()) == "") {
                    NewMessageBox('Please select prefix number.', 'Prefix Number');
                    return;
                }

                if ($.trim($("#txtOrgFOWSTMobileNo").val()) == "") {
                    NewMessageBox('Please enter 7 digit mobile number.', 'Mobile Number');
                    return;
                }

                if ($.trim($("#txtOrgFOWSTMobileNo").val()).length != 7) {
                    NewMessageBox('Please enter 7 digit mobile number.', 'Mobile No.');
                    return;
                }

                if (!$.isNumeric($("#txtOrgFOWSTMobileNo").val())) {
                    NewMessageBox('Please enter valid mobile number.', 'Mobile Number');
                    return;
                }

                if ($.trim($("#selOrgAddMobileType").val()) == "") {
                    NewMessageBox('Please select mobile contact type.', 'Contact Type');
                    return;
                }

                SaveOrgFOWSTMobile();
            })
            .on('click', '#btnOrgFOWSTMobileRemove', function () {
                NewMessageBox('Remove this record?', 'Remove',
                    [
                        {
                            Text: 'Yes',
                            Click: function(){
                                RemoveOrgFOWSTMobile();
                            }
                        },
                        { Text: 'No'}
                    ])
            })
            .on('click', '#btnOrgFOWSTMobileCancel', function () {
                //$.mobile.changePage('#pageOrgFOWSTInfo');
                $('#pageOrgFOWSTInfo').show();
                $('#pageOrgFOWSTTruckRegInfo').hide();
                $('#pageOrgFOWSTSectionInfo').hide();
                $('#pageOrgFOWSTBusinessPermitInfo').hide();
                $('#pageOrgFOWSTTelInfo').hide();
                $('#pageOrgFOWSTMobileInfo').hide();
                $('#pageOrgFOWSTEmailInfo').hide();
                $('#pageOrgFOWSTOperationHRInfo').hide();
                $('#pageOrgFOWSTPrinterInfo').hide();
                $('#pageOrgFOWSTPrinterIP').hide(); $('#pageAddDevicePrinter').hide();
            })
            .on('click', '#btnOrgFOWSTOptHRSave', function () {
                if ($.trim($("#selOrgFOWSTOptOpenHR").val()) != "" ||
                    $.trim($("#selOrgFOWSTOptOpenMin").val()) != "" ||
                    $.trim($("#selOrgFOWSTOptOpenAMPM").val()) != "") {

                    if ($.trim($("#selOrgFOWSTOptOpenHR").val()) == "" ||
                        $.trim($("#selOrgFOWSTOptOpenMin").val()) == "" ||
                        $.trim($("#selOrgFOWSTOptOpenAMPM").val()) == "") {
                        NewMessageBox('Please set operation opening hours.', 'Opening Hours');
                        return;
                    }
                }

                if ($.trim($("#selOrgFOWSTOptCloseHR").val()) != "" ||
                    $.trim($("#selOrgFOWSTOptCloseMin").val()) != "" ||
                    $.trim($("#selOrgFOWSTOptCloseAMPM").val()) != "") {

                    if ($.trim($("#selOrgFOWSTOptCloseHR").val()) == "" ||
                        $.trim($("#selOrgFOWSTOptCloseMin").val()) == "" ||
                        $.trim($("#selOrgFOWSTOptCloseAMPM").val()) == "") {
                        NewMessageBox('Please set operation closing hours.', 'Opening Hours');
                        return;
                    }
                }

                SaveOrgFOWSTOperationHR();
            })
            .on('click', '#btnOrgFOWSTOptHRCancel', function () {
                //$.mobile.changePage('#pageOrgFOWSTInfo');
                $('#pageOrgFOWSTInfo').show();
                $('#pageOrgFOWSTTruckRegInfo').hide();
                $('#pageOrgFOWSTSectionInfo').hide();
                $('#pageOrgFOWSTBusinessPermitInfo').hide();
                $('#pageOrgFOWSTTelInfo').hide();
                $('#pageOrgFOWSTMobileInfo').hide();
                $('#pageOrgFOWSTEmailInfo').hide();
                $('#pageOrgFOWSTOperationHRInfo').hide();
                $('#pageOrgFOWSTPrinterInfo').hide();
                $('#pageOrgFOWSTPrinterIP').hide(); $('#pageAddDevicePrinter').hide();
            })
            .on('click', '#btnDateOfPayment', function () {
                SingleCalendar({
                    Click: function (e) {
                        $('#btnDateOfPayment').text(e);//.button('refresh');
                        dateofpayment = e;
                    },
                    Clear: function () {
                        $('#btnDateOfPayment').text('Set Date');//.button('refresh');
                        dateofpayment = "";
                    }
                })
            })
            .on('click', '#btnDateIssued', function () {
                SingleCalendar({
                    Click: function (e) {
                        $('#btnDateIssued').text(e);//.button('refresh');
                        dateissued = e;
                    },
                    Clear: function () {
                        $('#btnDateIssued').text('Set Date');//.button('refresh');
                        dateissued = "";
                    }
                })
            })
            .on('click', '#btnRevPriceLevel', function () {
                BrowsePopup('LibraryPIMPriceLevel', 'Search Price Level',
                    [
                        {
                            AutoSearch: true,
                            DisplayText: [{ Column: 'PriceLevel' }],
                            Validation: function (e) {
                                var isExist = false;

                                for (var i = 0; i < arrFowstPriceLevels.length; i++) {
                                    if (arrFowstPriceLevels[i].ID == e.ID) {
                                        isExist = true;
                                    }
                                }

                                if (isExist) {
                                    WebToast('Selected Price Level already exist', 'top');
                                    return false;
                                }

                                return true;
                            },
                            Click: function (e) {
                                e.State = 'add';
                                arrFowstPriceLevels.push(e);
                                DisplayFowstPriceLevels();
                            }
                        }
                    ])
            })

            $('#btnBack').click(function () {
                history.back();
            });

            $('#btnOrgFOWSTDone').click(function (e) {
                e.preventDefault();

                window.open(GetPathFormat("/People/PeopleMenu"), "_self");

            });

            $('#btnOrgFOWSTGoToSetupChecklist').click(function (e) {
                e.preventDefault();

                window.open(GetPathFormat("/SetupChecklist"), "_self");

            });


            $('#btnPrinterBack').click(function (e) {
                history.back();
            });

            $('#btnPrinterIPBack').click(function (e) {
                history.back();
            });

            $('#selConnectionType').click(function (e) {
                if ($('#selConnectionType').val() == 'TCP/IP') {
                    $('#dvPrinterIP').css('visibility', 'visible');
                    $('#dvPrinterIP').css('display', 'block');
                } else {
                    $('#dvPrinterIP').css('visibility', 'hidden');
                    $('#dvPrinterIP').css('display', 'none');

                    if (CBool($('#selIsDispatchPrinter').is(':checked'))) {
                        NewMessageBox('Dispatch printer is only availble for TCP/IP or Ethernet printers.', 'Dispatch Printer')

                        try {
                            $('#selIsDispatchPrinter').prop('checked', 0);//.slider('refresh');
                        } catch (e) {
                            $('#selIsDispatchPrinter').prop('checked', 0);//.slider();
                        }
                    }
                }
            })

            function DisplayFowstPriceLevels() {
                var lstFowstPriceLevel = $('#lstFowstPriceLevel'), isEmpty = true;

                lstFowstPriceLevel.empty();

                for (var i = 0; i < arrFowstPriceLevels.length; i++) {
                    arrFowstPriceLevels[i].index = i;
                    if (arrFowstPriceLevels[i].State != 'remove') {
                        isEmpty = false;
                        lstFowstPriceLevel.append($('<a href="#" class="list-group-item list-group-item-action">')
                            .data(arrFowstPriceLevels[i])
                            .append($('<table style="width:100%;">')
                                .append($('<tr>')
                                    .append($('<td style="word-break:break-all;">')
                                        .html($.trim(arrFowstPriceLevels[i].PriceLevel))
                                    )
                                    .append($('<td style="width:50px; text-align:center;">').append($('<span class="btn btn-xs btn-danger">')
                                            .append($('<span class="bi-trash" aria-hidden="true">'))
                                            .data(arrFowstPriceLevels[i])
                                            .click(function (e) {
                                                var data = $(this).data();
                                                if (data.State == 'add') {
                                                    arrFowstPriceLevels = $.grep(arrFowstPriceLevels, function (e) {
                                                        return e.ID != data.ID;
                                                    })
                                                } else {
                                                    arrFowstPriceLevels[data.index].State = 'remove';
                                                }

                                                DisplayFowstPriceLevels();
                                            })
                                        )
                                    )
                                )
                            )
                        )
                    }
                }

                if (isEmpty) {
                    lstFowstPriceLevel.append($('<a href="#" class="list-group-item list-group-item-action">').text('No results found'))
                }
            }

            function HideDivExcludedMC() {
                if (GetUrlParameters().mp == "mc") {
                    $('.divExcludedInMC').hide();
                }
                else {
                    $('.divExcludedInMC').show();
                }
            }

            function SearchCityPopUp(s) {
                var pData = {};

                pData.keywords = s;

                AjaxSend({
                    url: 'Browser/LibraryCities',
                    data: pData,
                    success: function (vData) {
                        $('#lstOfLibrary').empty();

                        if (vData.Results.Rows.length > 0) {
                            for (var p = 0; p < vData.Results.Rows.length; p++) {
                                $('#lstOfLibrary').append($('<li>')
                                    .append($('<a>')
                                        .attr('ID', vData.Results.Rows[p].ID)
                                        .append($('<br><p style="font-weight:bold;">')
                                            .text(vData.Results.Rows[p].Name)
                                        )
                                        .data('data', vData.Results.Rows[p])
                                        .click(function (e) {
                                            if (NameOfBtnForLibrary == "CompleteAddress") {
                                                $('#txtOrgFOWSTCityID').val($(this).data('data').ID);
                                                $('#btnOrgFOWSTCity').text($(this).data('data').Name);//.button('refresh');
                                            }
                                            else if (NameOfBtnForLibrary == "BusinessPermit") {
                                                $('#txtOrgFOWSTBPermitCityID').val($(this).data('data').ID);
                                                $('#btnOrgFOWSTBPermitCity').text($(this).data('data').Name);//.button('refresh');
                                            }

                                            $('#panSearchLibrary').popup('close');
                                        })
                                    ));
                            }

                        }
                        else {
                            $('#lstOfLibrary').append($('<li>').text('No Result(s) Found'));
                        }

                        $('#lstOfLibrary');//.listview('refresh');
                    }
                });
            }

            function SearchCountryPopUp(s) {
                var pData = {};

                pData.keywords = s;

                AjaxSend({
                    url: 'Browser/LibraryCountries',
                    data: pData,
                    success: function (vData) {
                        $('#lstOfLibrary').empty();

                        if (vData.Results.Rows.length > 0) {
                            for (var p = 0; p < vData.Results.Rows.length; p++) {
                                $('#lstOfLibrary').append($('<li>')
                                    .append($('<a>')
                                        .attr('ID', vData.Results.Rows[p].ID)
                                        .append($('<br><p style="font-weight:bold;">')
                                            .text(vData.Results.Rows[p].Name)
                                        )
                                        .data('data', vData.Results.Rows[p])
                                        .click(function (e) {
                                            if (NameOfBtnForLibrary == "CompleteAddress") {
                                                $('#txtOrgFOWSTCountryID').val($(this).data('data').ID);
                                                $('#btnOrgFOWSTCountry').text($(this).data('data').Name);//.button('refresh');
                                            }
                                            else if (NameOfBtnForLibrary == "BusinessPermit") {
                                                $('#txtOrgFOWSTBPermitCountryID').val($(this).data('data').ID);
                                                $('#btnOrgFOWSTBPermitCountry').text($(this).data('data').Name);//.button('refresh');
                                            }
                                            else if (NameOfBtnForLibrary == "Telephone") {
                                                $('#txtOrgFOWSTTelCountryID').val($(this).data('data').ID);
                                                $('#btnOrgFOWSTTelCountryCode').text($(this).data('data').Code + ' - ' + $(this).data('data').CallingCode);//.button('refresh');
                                            }
                                            else if (NameOfBtnForLibrary == "Mobile") {
                                                $('#txtOrgFOWSTMobileCountryID').val($(this).data('data').ID);
                                                $('#btnOrgFOWSTMobileCountryCode').text($(this).data('data').Code + ' - ' + $(this).data('data').CallingCode);//.button('refresh');
                                            }

                                            $('#panSearchLibrary').popup('close');
                                        })
                                    ));
                            }

                        }
                        else {
                            $('#lstOfLibrary').append($('<li>').text('No Result(s) Found'));
                        }

                        $('#lstOfLibrary');//.listview('refresh');
                    }
                });
            }

            function SearchAreaCodePopUp(s) {
                var pData = {};

                pData.keywords = s;

                AjaxSend({
                    url: 'Browser/LibraryAreaCodes',
                    data: pData,
                    success: function (vData) {
                        $('#lstOfLibrary').empty();

                        if (vData.Results.Rows.length > 0) {
                            for (var p = 0; p < vData.Results.Rows.length; p++) {
                                $('#lstOfLibrary').append($('<li>')
                                    .append($('<a>')
                                        .attr('ID', vData.Results.Rows[p].ID)
                                        .append($('<br><p style="font-weight:bold;">')
                                            .text(vData.Results.Rows[p].Area + ' - ' + vData.Results.Rows[p].CallingCode)
                                        )
                                        .data('data', vData.Results.Rows[p])
                                        .click(function (e) {
                                            if (NameOfBtnForLibrary == "Telephone") {
                                                $('#txtOrgFOWSTTelAreaCodeID').val($(this).data('data').ID);
                                                $('#btnOrgFOWSTTelAreaCode').text($(this).data('data').Area + ' - ' + $(this).data('data').CallingCode);//.button('refresh');
                                            }

                                            $('#panSearchLibrary').popup('close');
                                        })
                                    ));
                            }

                        }
                        else {
                            $('#lstOfLibrary').append($('<li>').text('No Result(s) Found'));
                        }

                        $('#lstOfLibrary');//.listview('refresh');
                    }
                });
            }

            function SearchTelcoCodePopUp(s) {
                var pData = {};

                pData.keywords = s;

                AjaxSend({
                    url: 'Browser/LibraryTelcoPrefixes',
                    data: pData,
                    success: function (vData) {
                        $('#lstOfLibrary').empty();

                        if (vData.Results.Rows.length > 0) {
                            for (var p = 0; p < vData.Results.Rows.length; p++) {
                                $('#lstOfLibrary').append($('<li>')
                                    .append($('<a>')
                                        .attr('ID', vData.Results.Rows[p].ID)
                                        .append($('<br><p style="font-weight:bold;">')
                                            .text(vData.Results.Rows[p].Prefix)
                                        )
                                        .append($('<p>')
                                            .text(vData.Results.Rows[p].TelCo)
                                        )
                                        .data('data', vData.Results.Rows[p])
                                        .click(function (e) {
                                            if (NameOfBtnForLibrary == "Mobile") {
                                                $('#txtOrgFOWSTTelCoPrefixID').val($(this).data('data').ID);
                                                $('#btnOrgFOWSTMobileTelCoPrefixCode').text($(this).data('data').TelCo + ' - ' + $(this).data('data').Prefix);//.button('refresh');
                                            }

                                            $('#panSearchLibrary').popup('close');
                                        })
                                    ));
                            }

                        }
                        else {
                            $('#lstOfLibrary').append($('<li>').text('No Result(s) Found'));
                        }

                        $('#lstOfLibrary');//.listview('refresh');
                    }
                });
            }

            function SearchFranchisorPopUp(s) {
                var pData = {};

                pData.keywords = s;
                pData.FilterFranchisor = 1;

                AjaxSend({
                    url: 'Browser/LibraryOrganization',
                    data: pData,
                    success: function (vData) {
                        $('#lstOfLibrary').empty();

                        if (vData.Results.Rows.length > 0) {
                            for (var p = 0; p < vData.Results.Rows.length; p++) {
                                $('#lstOfLibrary').append($('<li>')
                                    .append($('<a>')
                                        .attr('ID', vData.Results.Rows[p].ID)
                                        .append($('<br><p style="font-weight:bold;">')
                                            .text(vData.Results.Rows[p].CompanyName)
                                        )
                                        .data('data', vData.Results.Rows[p])
                                        .click(function (e) {
                                            if (NameOfBtnForLibrary == "Franchisor") {
                                                $('#txtOrgFOWSTFranchisorID').val($(this).data('data').ID);
                                                $('#btnOrgFOWSTFranchisor').text($(this).data('data').CompanyName);//.button('refresh');
                                            }

                                            $('#panSearchLibrary').popup('close');
                                        })
                                    ));
                            }

                        }
                        else {
                            $('#lstOfLibrary').append($('<li>').text('No Result(s) Found'));
                        }

                        $('#lstOfLibrary');//.listview('refresh');
                    }
                });
            }
            function SearchBinPopUp(s) {
                var pData = {};

                pData.keywords = s;
                pData.FowstID = fowstID;

                AjaxSend({
                    url: 'Browser/LibraryFowstBin',
                    data: pData,
                    success: function (vData) {
                        $('#lstOfLibrary').empty();

                        if (vData.Results.Rows.length > 0) {
                            for (var p = 0; p < vData.Results.Rows.length; p++) {
                                $('#lstOfLibrary').append($('<li>')
                                    .append($('<a>')
                                        .attr('ID', vData.Results.Rows[p].ID)
                                        .append($('<br><p style="font-weight:bold;">')
                                            .text(vData.Results.Rows[p].Name)
                                        )
                                        .data('data', vData.Results.Rows[p])
                                        .click(function (e) {
                                            if (NameOfBtnForLibrary == "Bin") {
                                                SaveFowstBinSection($(this).data('data').ID);
                                            }

                                            $('#panSearchLibrary').popup('close');
                                        })
                                    ));
                            }

                        }
                        else {
                            $('#lstOfLibrary').append($('<li>').text('No Result(s) Found'));
                        }

                        $('#lstOfLibrary');//.listview('refresh');
                    }
                });
            }
            function SearchWarehousePickerPopUp(s) {
                var pData = {};

                pData.keywords = s;
                pData.FowstID = fowstID;

                AjaxSend({
                    url: 'Browser/LibraryOrganizationEmployeeFOWST',
                    data: pData,
                    success: function (vData) {
                        $('#lstOfLibrary').empty();

                        if (vData.Results.Rows.length > 0) {
                            for (var p = 0; p < vData.Results.Rows.length; p++) {
                                $('#lstOfLibrary').append($('<li>')
                                    .append($('<a>')
                                        .attr('ID', vData.Results.Rows[p].ID)
                                        .append($('<br><p style="font-weight:bold;">')
                                            .text(vData.Results.Rows[p].PeopleName)
                                        )
                                        .data('data', vData.Results.Rows[p])
                                        .click(function (e) {
                                            if (NameOfBtnForLibrary == "WarehousePicker") {
                                                SaveFowstSectionPeople($(this).data('data').ID);
                                            }

                                            $('#panSearchLibrary').popup('close');
                                        })
                                    ));
                            }

                        }
                        else {
                            $('#lstOfLibrary').append($('<li>').text('No Result(s) Found'));
                        }

                        $('#lstOfLibrary');//.listview('refresh');
                    }
                });
            }

            function getOrgAddAssignedDevices(callback) {
                if (CStr2(sessionStorage.ViewID) != '') {
                    AjaxSend({
                        url: 'Browser/LibraryDeviceFowst',
                        data: { FOWSTID: sessionStorage.ViewID },
                        success: function (vData) {
                            var vRows = vData.Results.Rows;
                            var tbodyOrgFOWSTDevices = $('#tbodyOrgFOWSTDevices').empty();

                            if (vRows.length > 0) {
                                for (var p = 0; p <= vRows.length - 1; p++) {
                                    tbodyOrgFOWSTDevices.append($('<tr>')
                                        .append($('<td>').text((p + 1) + '.'))
                                        .append($('<td>').text(vRows[p].PhoneName))
                                        .append($('<td>').text(vRows[p].UUID))
                                        .append($('<td>').text(vRows[p].POSRegistrationNo))
                                    )
                                }
                            } else {
                                tbodyOrgFOWSTDevices.append($('<tr>')
                                    .append($('<td colspan="5" class="text-center">').text('No assigned devices found')))
                            }

                            callback()
                        },
                        error: function (vError) {
                            var pDummy;
                        }
                    });
                } else callback()
            }

            function getOrgAddTradeName(callback) {

                AjaxSend({
                    url: 'Organization/getOrgTradeNameList',
                    success: function (vData) {

                        var vRows = vData.OrganizationData.Rows;

                        $('#selOrgFOWSTTradeName').empty();

                        $('#selOrgFOWSTTradeName').append(new Option('Please Select', ''));
                        for (var p = 0; p <= vRows.length - 1; p++) {
                            $('#selOrgFOWSTTradeName').append(new Option(vRows[p].BusinessTradeName, vRows[p].ID, false, false));
                        }

                        callback()
                    },
                    error: function (vError) {
                        var pDummy;
                    }
                });

            }

            function getOrgReceiptSettings(callback) {

                AjaxSend({
                    url: "/ReceiptSettings/getReceiptSettingsList",
                    success: function (vData) {

                        var vRows = vData.ReceiptSettingsData.Rows;

                        $('#selOrgFOWSTReceiptSettings').empty();

                        $('#selOrgFOWSTReceiptSettings').append(new Option('Please Select', ''));
                        for (var p = 0; p <= vRows.length - 1; p++) {
                            $('#selOrgFOWSTReceiptSettings').append(new Option(vRows[p].BusinessTradeName + '(' + vRows[p].PaperSize + ')', vRows[p].ID, false, false));
                        }

                        callback()
                    },
                    error: function (vError) {
                        var pDummy;
                    }
                });

            }

            function getOrgAddFowstType(callback) {

                AjaxSend({
                    url: 'Browser/LibraryFowstType',
                    success: function (vData) {
                        var vRows = vData.Results.Rows;

                        $('#selOrgFOWSTType').empty();

                        $('#selOrgFOWSTType').append(new Option('Please Select', ''));
                        for (var p = 0; p <= vRows.length - 1; p++) {
                            $('#selOrgFOWSTType').append(new Option(vRows[p].Name, vRows[p].ID, false, false));
                        }

                        callback()
                    },
                    error: function (vError) {
                        var pDummy;
                    }
                });
            }

            function getOrgAddWarehouseType(callback) {
                AjaxSend({
                    url: 'Browser/LibraryFOWSTWarehouseType',
                    success: function (vData) {
                        var vRows = vData.Results.Rows;

                        $('#selOrgFOWSTWarehouseType').empty();

                        $('#selOrgFOWSTWarehouseType').append(new Option('Please Select', ''));
                        for (var p = 0; p <= vRows.length - 1; p++) {
                            $('#selOrgFOWSTWarehouseType').append(new Option(vRows[p].Name, vRows[p].ID, false, false));
                        }

                        callback()
                    },
                    error: function (vError) {
                        var pDummy;
                    }
                });
            }

            function getOrgAddRetailStoreType(callback) {
                AjaxSend({
                    url: 'Browser/LibraryFOWSTRetailStoreType',
                    success: function (vData) {
                        var vRows = vData.Results.Rows;

                        $('#selOrgFOWSTRetailStoreType').empty();

                        $('#selOrgFOWSTRetailStoreType').append(new Option('Please Select', ''));
                        for (var p = 0; p <= vRows.length - 1; p++) {
                            $('#selOrgFOWSTRetailStoreType').append(new Option(vRows[p].Name, vRows[p].ID, false, false));
                        }

                        callback()
                    },
                    error: function (vError) {
                        var pDummy;
                    }
                });
            }

            function getOrgAddStoreLocationType(callback) {
                AjaxSend({
                    url: 'Browser/LibraryFOWSTStoreLocationType',
                    success: function (vData) {
                        var vRows = vData.Results.Rows;

                        $('#selOrgFOWSTRetailStoreLocationType').empty();

                        $('#selOrgFOWSTRetailStoreLocationType').append(new Option('Please Select', ''));
                        for (var p = 0; p <= vRows.length - 1; p++) {
                            $('#selOrgFOWSTRetailStoreLocationType').append(new Option(vRows[p].LocationType, vRows[p].ID, false, false));
                        }

                        callback()
                    },
                    error: function (vError) {
                        var pDummy;
                    }
                });
            }

            function getOrgFOWSTQuota(callback) {
                if (CStr2(sessionStorage.ViewID) != '') {
                    AjaxSend({
                        url: 'Organization/GetFOWSTQuota',
                        data: { ID: sessionStorage.ViewID },
                        success: function (vData) {
                            DisplayFOWSTQuota(vData.Results.Rows);

                            callback()
                        }
                    });
                } else callback();
            }

            var tblFOWSTAssignedEmployees;
            function getFOWSTAssignedEmployees(callback) {
                if (CStr2(sessionStorage.ViewID) != '') {
                    AjaxSend({
                        url: 'Organization/GetFOWSTAssignedEmployees',
                        data: { ID: sessionStorage.ViewID },
                        success: function (vData) {
                            if ($.fn.DataTable.isDataTable('#tblFOWSTAssignedEmployees')) {                                
                                try {
                                    $('#tblFOWSTAssignedEmployees').DataTable().clear().destroy();
                                    //$('#tblFOWSTAssignedEmployees').empty();
                                } catch (e) {

                                }
                            }

                            var ctr = 0;

                            tblFOWSTAssignedEmployees = $('#tblFOWSTAssignedEmployees').DataTable({
                                paging: false,
                                "ordering": false,
                                "data": vData.Results.Rows,
                                "columns": [
                                    {
                                        "data": function (data) {
                                            ctr += 1;
                                            return ctr + '.';
                                        }, width: '50px'
                                    },
                                    { "data": "EmployeeName" },
                                    {
                                        "data": function (data) {
                                            return CStr2(data.Position);
                                        }
                                    },
                                    {
                                        data: null,
                                        createdCell: function (td, cellData, rowData, row, col) {
                                            var btn = $('<button data-role="none" class="btn btn-danger">').append('<span class="bi-trash-fill"></span>');
                                            $(td).data('data', rowData).css('text-align', 'center').html(btn)
                                                .click(function () {
                                                    var data = $(this).data('data');

                                                    arrRemoveAssignedEmp.push(data);
                                                    $(this).parents().closest('tr').remove();
                                                })
                                        }, width: '50px'
                                    }
                                ],
                                //dom: 'Brtip',
                                initComplete: function () {
                                    Bs.Loading('hide');
                                    $('#tblFOWSTAssignedEmployees').removeClass('dataTable');
                                    callback()
                                },
                                dom: '<"clear"><"table-fixed-container"<"table-fixed"t>><"bottom"ip><"clear">',
                            });

                            $('#txtSearchAssignedEmp').unbind().keydown(function (e) {
                                tblFOWSTAssignedEmployees.search($.trim($(this).val())).draw();
                            })
                        }
                    });
                } else callback();
            }

            function DisplayFOWSTQuota(vRows) {

                for (i = 0; i < vRows.length; i++) {
                    var month = ParseDate(vRows[i].MonthYear).getMonth() + 1 //cause getMonth start @ 0

                    switch (month) {
                        case 1:
                            $('#txtOrgTeamQuotaTier1Jan').val(vRows[i].Tier1Quota);
                            $('#txtOrgTeamIncentiveTier1Jan').val(vRows[i].Tier1Incentive);
                            break;
                        case 2:
                            $('#txtOrgTeamQuotaTier1Feb').val(vRows[i].Tier1Quota);
                            $('#txtOrgTeamIncentiveTier1Feb').val(vRows[i].Tier1Incentive);
                            break;
                        case 3:
                            $('#txtOrgTeamQuotaTier1Mar').val(vRows[i].Tier1Quota);
                            $('#txtOrgTeamIncentiveTier1Mar').val(vRows[i].Tier1Incentive);
                            break;
                        case 4:
                            $('#txtOrgTeamQuotaTier1Apr').val(vRows[i].Tier1Quota);
                            $('#txtOrgTeamIncentiveTier1Apr').val(vRows[i].Tier1Incentive);
                            break;
                        case 5:
                            $('#txtOrgTeamQuotaTier1May').val(vRows[i].Tier1Quota);
                            $('#txtOrgTeamIncentiveTier1May').val(vRows[i].Tier1Incentive);
                            break;
                        case 6:
                            $('#txtOrgTeamQuotaTier1Jun').val(vRows[i].Tier1Quota);
                            $('#txtOrgTeamIncentiveTier1Jun').val(vRows[i].Tier1Incentive);
                            break;
                        case 7:
                            $('#txtOrgTeamQuotaTier1Jul').val(vRows[i].Tier1Quota);
                            $('#txtOrgTeamIncentiveTier1Jul').val(vRows[i].Tier1Incentive);
                            break;
                        case 8:
                            $('#txtOrgTeamQuotaTier1Aug').val(vRows[i].Tier1Quota);
                            $('#txtOrgTeamIncentiveTier1Aug').val(vRows[i].Tier1Incentive);
                            break;
                        case 9:
                            $('#txtOrgTeamQuotaTier1Sept').val(vRows[i].Tier1Quota);
                            $('#txtOrgTeamIncentiveTier1Sept').val(vRows[i].Tier1Incentive);
                            break;
                        case 10:
                            $('#txtOrgTeamQuotaTier1Oct').val(vRows[i].Tier1Quota);
                            $('#txtOrgTeamIncentiveTier1Oct').val(vRows[i].Tier1Incentive);
                            break;
                        case 11:
                            $('#txtOrgTeamQuotaTier1Nov').val(vRows[i].Tier1Quota);
                            $('#txtOrgTeamIncentiveTier1Nov').val(vRows[i].Tier1Incentive);
                            break;
                        case 12:
                            $('#txtOrgTeamQuotaTier1Dec').val(vRows[i].Tier1Quota);
                            $('#txtOrgTeamIncentiveTier1Dec').val(vRows[i].Tier1Incentive);
                            break;
                    }

                }
            }

            function SetOrgAddSubPropertyAndPLantType() {

                $('#divOrgFOWSTRetailStoreTypeInfo, #divOrgFOWSTRetailStoreLocationTypeInfo').hide();
                $("#divOrgFOWSTWarehouseTypeInfo").hide();
                $('#divOrgFOWSTDeliveryEquipmentInfo').hide();
                $("#divOrgFOWSTSection").hide();

                //Branch
                if ($("#selOrgFOWSTType").val() == '6af9bc83-426c-4774-a051-044fc09c552b') {
                    $('#divOrgFOWSTRetailStoreTypeInfo, #divOrgFOWSTRetailStoreLocationTypeInfo').show();
                    $("#divOrgFOWSTSection").show();
                } //Office
                else if ($("#selOrgFOWSTType").val() == 'f58168f1-e00a-415a-a29e-5d4b92784b2b' ||
                    $("#selOrgFOWSTType").val() == '4bc157ce-69a2-4371-ba91-d4e9b6afb3f2') {
                    $("#divOrgFOWSTSection").show();

                } //Delivery Equipment
                else if ($("#selOrgFOWSTType").val() == 'ce791943-9d80-42fc-bebe-5bb52a5792e8') {
                    $('#divOrgFOWSTDeliveryEquipmentInfo').show();
                } //Warehouse
                else {
                    $('#divOrgFOWSTWarehouseTypeInfo').show();
                    $("#divOrgFOWSTSection").show();
                }
            }

            function ShowOrgAddHideFranchisorDetails() {

                if ($("#selOrgAddFranchisee").is(':checked')) {

                    $("#divFranchisorInfo").show();
                    $("#btnOrgFOWSTFranchisor").text(CStr2($("#btnOrgFOWSTFranchisor").text()) != '' ? $("#btnOrgFOWSTFranchisor").text() : 'Please Select');//.button('refresh');
                }
                else {
                    $("#divFranchisorInfo").hide();
                }
            }

            function SetOrgAddShowHideRentedInfo() {

                if ($.trim($("#selOrgFOWSTOwnershipType").val()) == "1") {
                    $("#divRentedOrLeasedInfo").show();
                }
                else {
                    $("#divRentedOrLeasedInfo").hide();
                }
            }

            function SetOrgAddShowHideMallInfo() {

                $("#divOrgFOWSTAyalaInfo").hide();
                $("#divOrgFOWSTSMInfo").hide();
                $("#divOrgFOWSTMegaworldInfo").hide();
                $("#divOrgFOWSTRobinsonInfo").hide();

                if ($.trim($("#selOrgFOWSTMallName").val()) == "Ayala Mall") {
                    $("#divOrgFOWSTAyalaInfo").show();
                }
                else if ($.trim($("#selOrgFOWSTMallName").val()) == "SM Mall") {
                    $("#divOrgFOWSTSMInfo").show();
                }
                else if ($.trim($("#selOrgFOWSTMallName").val()) == "Gaisano Mall") {
                    $("#divOrgFOWSTSMInfo").show();
                }
                else if ($.trim($("#selOrgFOWSTMallName").val()) == "Megaworld") {
                    $("#divOrgFOWSTMegaworldInfo").show();
                }
                else if ($.trim($("#selOrgFOWSTMallName").val()) == "Robinson") {
                    $("#divOrgFOWSTRobinsonInfo").show();
                }
                else if ($.trim($("#selOrgFOWSTMallName").val()) == "PITX") {
                    $("#divOrgFOWSTPITXInfo").show();
                }
            }

            function getOrgFOWSTList() {
                AjaxSend({
                    url: 'Organization/getOrgFOWSTList',
                    success: function (vData) {
                        LoadOrgFOWSTList(vData.OrganizationFOWSTData.Rows);
                    }
                });
            }

            function LoadOrgFOWSTList(vRows) {
                var pLV = $('#lstOrgFOWST');
                var pItem;

                $("#divOrgFOWSTRecordCount").text(" Number of record(s) found: " + vRows.length);

                pLV.html('');
                for (var p = 0; p <= vRows.length - 1; p++) {
                    pItem = $('<li>')
                        .append($('<a>')
                            .attr('ID', vRows[p].ID)
                            .click(function (e) {
                                e.preventDefault();
                                //$.mobile.changePage('#pageOrgFOWSTInfo');
                                $('#pageOrgFOWSTInfo').show();
                                $('#pageOrgFOWSTTruckRegInfo').hide();
                                $('#pageOrgFOWSTSectionInfo').hide();
                                $('#pageOrgFOWSTBusinessPermitInfo').hide();
                                $('#pageOrgFOWSTTelInfo').hide();
                                $('#pageOrgFOWSTMobileInfo').hide();
                                $('#pageOrgFOWSTEmailInfo').hide();
                                $('#pageOrgFOWSTOperationHRInfo').hide();
                                $('#pageOrgFOWSTPrinterInfo').hide();
                                $('#pageOrgFOWSTPrinterIP').hide(); $('#pageAddDevicePrinter').hide();
                                ShowOrgFOWSTInfo($(this).attr('ID'));
                            })
                            .append($('<h3>')
                                .attr("style", "white-space:normal")
                                .text(vRows[p].Name)
                            )
                            .append($('<p>')
                                .attr("style", "white-space:normal")
                                .text(vRows[p].CompleteFowstAddress)
                            )
                        );
                    pLV.append(pItem);
                };

                pLV;//.listview('refresh');
            };

            function ShowOrgFOWSTInfo(strID) {
                var pData = {};
                pData.ID = strID;

                fowstID = strID; //additional by janine

                ClearOrgFOWSTFields();

                AjaxSend({
                    url: 'Browser/LibraryHotButtonTemplate',
                    success: function (e) {
                        var vRows = e.Results.Rows;
                        var selDefaultHBTemplate = $('#selDefaultHBTemplate');

                        selDefaultHBTemplate.empty().append($('<option value="">').text('All Hot Button Templates'));

                        for (var i = 0; i < vRows.length; i++){
                            selDefaultHBTemplate.append($('<option value="' + vRows[i].ID + '">').text(vRows[i].Name));
                        }

                        selDefaultHBTemplate;//.selectmenu('refresh');

                        proj.CheckIfVDMFeaturesIsActivated(function (e) {
                            if (CBool(e.EnableVDMFeatures)) {
                                $('#selDefaultPosView').append(new Option('Vending Machine', 'Vending Machine'))
                            } else {
                                $('#selDefaultPosView option[value="Vending Machine"]').remove();
                            }

                            GetFowstInfo(pData);
                        })
                        
                    }
                })
            };

            function GetFowstInfo(pData) {
                AjaxSend({
                    url: "Organization/getOrgFOWSTInfo",
                    data: pData,
                    success: function (vData) {

                        if (vData.OrganizationFOWSTData.Rows.length > 0) {
                            var load = setInterval(function () {
                                if ($("#selOrgFOWSTType option:selected").val() == "") {
                                    $("#txtOrgFOWSTID").val(vData.OrganizationFOWSTData.Rows[0].ID), $("#selOrgFOWSTType").val(vData.OrganizationFOWSTData.Rows[0].FOWSTTypeID);//.selectmenu("refresh"), SetOrgAddSubPropertyAndPLantType(), $("#txtOrgFOWSTCode").val(vData.OrganizationFOWSTData.Rows[0].Code), $("#txtOrgFOWSTName").val(vData.OrganizationFOWSTData.Rows[0].Name), $("#txtOrgFOWSTDescription").val(vData.OrganizationFOWSTData.Rows[0].Description), $("#selOrgFOWSTTradeName").val(vData.OrganizationFOWSTData.Rows[0].BusinessTradeNameID);//.selectmenu("refresh"), $("#txtTINBranchCode").val(vData.OrganizationFOWSTData.Rows[0].TINBranchCode), $("#txtOrgFOWSTVATRate").val(vData.OrganizationFOWSTData.Rows[0].VATRate), $("#txtOrgFOWSTBuildingNo").val(vData.OrganizationFOWSTData.Rows[0].StreetNumber), $("#txtOrgFOWSTStreet").val(vData.OrganizationFOWSTData.Rows[0].Street), $("#txtOrgFOWSTCityID").val(vData.OrganizationFOWSTData.Rows[0].FowstCityID), $("#btnOrgFOWSTCity").text(vData.OrganizationFOWSTData.Rows[0].FowstCity || "Please Select");//.button("refresh"), $("#txtOrgFOWSTCountryID").val(vData.OrganizationFOWSTData.Rows[0].FowstCountryID), $("#btnOrgFOWSTCountry").text(vData.OrganizationFOWSTData.Rows[0].FowstCountry || "Philippines");//.button("refresh"), $("#selOrgAddFranchisee").val(vData.OrganizationFOWSTData.Rows[0].IsFranchisee || "0");//.slider("refresh"), ShowOrgAddHideFranchisorDetails(), $("#txtOrgFOWSTFranchisorID").val(vData.OrganizationFOWSTData.Rows[0].FranchisorID), $("#btnOrgFOWSTFranchisor").text(vData.OrganizationFOWSTData.Rows[0].Franchisor);//.button("refresh"), "1" == vData.OrganizationFOWSTData.Rows[0].IsFranchiseeActive && "1" == vData.OrganizationFOWSTData.Rows[0].IsFranchiseeConfirmed && "1" == vData.OrganizationFOWSTData.Rows[0].IsFranchisorConfirmed ? $("#divFranchiseeConfirmed").hide() : $("#divFranchiseeConfirmed").show(), $("#selOrgFOWSTOwnershipType").val(vData.OrganizationFOWSTData.Rows[0].OwnershipType || "0");//.selectmenu("refresh"), SetOrgAddShowHideRentedInfo(), $("#selOrgFOWSTContractStartMonth").val(vData.OrganizationFOWSTData.Rows[0].ContractStartMonth);//.selectmenu("refresh"), $("#selOrgFOWSTContractStartYear").val(vData.OrganizationFOWSTData.Rows[0].ContractStartYear);//.selectmenu("refresh"), setDayCollection("selOrgFOWSTContractStartMonth", "selOrgFOWSTContractStartDay", "selOrgFOWSTContractStartYear", !0), $("#selOrgFOWSTContractStartDay").val(vData.OrganizationFOWSTData.Rows[0].ContractStartDay);//.selectmenu("refresh"), $("#selOrgFOWSTContractEndMonth").val(vData.OrganizationFOWSTData.Rows[0].ContractEndMonth);//.selectmenu("refresh"), $("#selOrgFOWSTContractEndYear").val(vData.OrganizationFOWSTData.Rows[0].ContractEndYear);//.selectmenu("refresh"), setDayCollection("selOrgFOWSTContractEndMonth", "selOrgFOWSTContractEndDay", "selOrgFOWSTContractEndYear", !0), $("#selOrgFOWSTContractEndDay").val(vData.OrganizationFOWSTData.Rows[0].ContractEndDay);//.selectmenu("refresh"), $("#txtOrgFOWSTCostPerMonth").val(vData.OrganizationFOWSTData.Rows[0].CostPerMonth), $("#selOrgFOWSTIsMortgaged").val(vData.OrganizationFOWSTData.Rows[0].IsMortgaged || "1");//.slider("refresh"), $("#selOrgFOWSTInsideMall").val(vData.OrganizationFOWSTData.Rows[0].IsFOWSTInsideAMall || "0");//.slider("refresh"), "1" == $.trim($("#selOrgFOWSTInsideMall").val()) ? $("#divOrgFOWSTInsideMall").show() : $("#divOrgFOWSTInsideMall").hide(), $("#selOrgFOWSTMallName").val(vData.OrganizationFOWSTData.Rows[0].FOWSTMallName);//.selectmenu("refresh"), SetOrgAddShowHideMallInfo(), $("#txtOrgFOWSTAyalaMallRecID").val(vData.OrganizationFOWSTData.Rows[0].AyalaMallRecID), $("#txtOrgFOWSTAyalaMallTenantCode").val(vData.OrganizationFOWSTData.Rows[0].AyalaTenantCode), $("#txtOrgFOWSTAyalaMallTenantName").val(vData.OrganizationFOWSTData.Rows[0].AyalaTenantName), $("#txtOrgFOWSTSMMallRecID").val(vData.OrganizationFOWSTData.Rows[0].SMMallRecID), $("#txtOrgFOWSTSMMallCode").val(vData.OrganizationFOWSTData.Rows[0].SMMallCode), $("#txtOrgFOWSTSMMallTenantNo").val(vData.OrganizationFOWSTData.Rows[0].SMMallTenantNo), $("#txtOrgFOWSTSMMallClassCode").val(vData.OrganizationFOWSTData.Rows[0].SMMallClassCode), $("#txtOrgFOWSTSMMallTradeCode").val(vData.OrganizationFOWSTData.Rows[0].SMMallTradeCode), $("#txtOrgFOWSTSMMallOutletNo").val(vData.OrganizationFOWSTData.Rows[0].SMMallOutletNo), $("#txtOrgFOWSTSMMallSalesType").val(vData.OrganizationFOWSTData.Rows[0].SMMallSaleType), "6af9bc83-426c-4774-a051-044fc09c552b" == vData.OrganizationFOWSTData.Rows[0].FOWSTTypeID ? $("#selOrgFOWSTRetailStoreType").val(vData.OrganizationFOWSTData.Rows[0].SubFOWSTTypeID);//.selectmenu("refresh") : "247c961c-a219-45df-bda3-3c137b734abf" == vData.OrganizationFOWSTData.Rows[0].FOWSTTypeID && $("#selOrgFOWSTWarehouseType").val(vData.OrganizationFOWSTData.Rows[0].SubFOWSTTypeID);//.selectmenu("refresh"), $("#txtOrgFOWSTPlateNo").val(vData.OrganizationFOWSTData.Rows[0].PlateNo), $("#txtOrgFOWSTNoOfWheels").val(vData.OrganizationFOWSTData.Rows[0].NoOfWheels), $("#txtOrgFOWSTBodyColor").val(vData.OrganizationFOWSTData.Rows[0].BodyColor), $("#txtOrgFOWSTLoadLimitWeight").val(vData.OrganizationFOWSTData.Rows[0].Weight), $("#txtOrgFOWSTLoadLimitDimensions").val(vData.OrganizationFOWSTData.Rows[0].Dimension), $("#selOrgFOWSTIsInclude").val(CInt2(vData.OrganizationFOWSTData.Rows[0].IsIncludeSpecialReport));//.slider("refresh"), $("#txtAccountID").val(vData.OrganizationFOWSTData.Rows[0].AccountID), $("#txtStoreID").val(vData.OrganizationFOWSTData.Rows[0].StoreID), $("#txtAccountID2").val(vData.OrganizationFOWSTData.Rows[0].AccountID2), $("#txtStoreID2").val(vData.OrganizationFOWSTData.Rows[0].StoreID2), $("#selOrgFOWSTIsIncludeToShopperTrak").val(CInt2(vData.OrganizationFOWSTData.Rows[0].IsIncludeToShopperTrakReport));//.slider("refresh"), $("#txtShopperTrakAccountID").val(vData.OrganizationFOWSTData.Rows[0].ShopperTrakAccountID), pData.OrgFOWSTID = vData.OrganizationFOWSTData.Rows[0].ID;
                                    HideDivExcludedMC();
                                }
                                else {
                                    clearInterval(load);
                                }
                            }, 1500)

                            $("#txtOrgFOWSTID").val(vData.OrganizationFOWSTData.Rows[0].ID || '');

                            var str = vData.OrganizationFOWSTData.Rows[0].FOWSTStatus;

                            if (str == 'Available') {
                                str = 'Store and device are operational.';
                            } else if (str == 'Closed') {
                                str = 'Store is permanently closed.';
                            } else if (str == 'Temporary Closed') {
                                str = 'Store is temporarily closed for holiday.';
                            }

                            $("#selOrgFOWSTStatus").val(str);
                            $("#selOrgFOWSTType").val(vData.OrganizationFOWSTData.Rows[0].FOWSTTypeID || '');//.selectmenu('refresh');
                            $("#selOrgFOWSTClassification").val(vData.OrganizationFOWSTData.Rows[0].FOWSTClassification || '');//.selectmenu('refresh');
                            SetOrgAddSubPropertyAndPLantType();
                            HideDivExcludedMC();

                            $("#txtOrgFOWSTCode").val(vData.OrganizationFOWSTData.Rows[0].Code || '');
                            $("#txtOrgFOWSTName").val(vData.OrganizationFOWSTData.Rows[0].Name || '');
                            $("#txtOrgFOWSTDescription").val(vData.OrganizationFOWSTData.Rows[0].Description || '');
                            $("#selOrgFOWSTTradeName").val(vData.OrganizationFOWSTData.Rows[0].BusinessTradeNameID || '');//.selectmenu('refresh');
                            $("#selOrgFOWSTReceiptSettings").val(vData.OrganizationFOWSTData.Rows[0].ReceiptSettingsID || ''); 
                            $("#txtTINBranchCode").val(vData.OrganizationFOWSTData.Rows[0].TINBranchCode || '');

                            $("#txtOrgFOWSTVATRate").val(vData.OrganizationFOWSTData.Rows[0].VATRate || '');
                            $("#selOrgActiveFOWST").prop('checked', CInt2(vData.OrganizationFOWSTData.Rows[0].IsActive));//.slider('refresh');                            

                            $("#txtOrgFOWSTBuildingNo").val(vData.OrganizationFOWSTData.Rows[0].StreetNumber || '');
                            $("#txtOrgFOWSTStreet").val(vData.OrganizationFOWSTData.Rows[0].Street || '');
                            $("#txtOrgFOWSTCityID").val(vData.OrganizationFOWSTData.Rows[0].FowstCityID || '');
                            $("#btnOrgFOWSTCity").text(vData.OrganizationFOWSTData.Rows[0].FowstCity || 'Please Select');//.button('refresh');
                            $("#selOrgFOWSTRegion").val(vData.OrganizationFOWSTData.Rows[0].Region || '');//.selectmenu('refresh');
                            $("#txtOrgFOWSTCountryID").val(vData.OrganizationFOWSTData.Rows[0].FowstCountryID || 'e783ed13-aa98-4cc1-918e-d49b05c0167e');
                            $("#btnOrgFOWSTCountry").text(vData.OrganizationFOWSTData.Rows[0].FowstCountry || 'Philippines');//.button('refresh');

                            $("#selOrgAddFranchisee").prop('checked', CInt2(vData.OrganizationFOWSTData.Rows[0].IsFranchisee || 0));//.slider("refresh");
                            ShowOrgAddHideFranchisorDetails();
                            $("#txtOrgFOWSTFranchisorID").val(vData.OrganizationFOWSTData.Rows[0].FranchisorID || '');
                            $("#btnOrgFOWSTFranchisor").text(vData.OrganizationFOWSTData.Rows[0].Franchisor || '');//.button('refresh');
                            //additional by janine oct 16
                            if (vData.OrganizationFOWSTData.Rows[0].IsFranchiseeActive == '1' && vData.OrganizationFOWSTData.Rows[0].IsFranchiseeConfirmed == '1' && vData.OrganizationFOWSTData.Rows[0].IsFranchisorConfirmed == '1') {
                                $("#divFranchiseeConfirmed").hide();
                            } else {
                                $("#divFranchiseeConfirmed").show();
                            }


                            $("#selOrgFOWSTOwnershipType").val(vData.OrganizationFOWSTData.Rows[0].OwnershipType || '0');//.selectmenu("refresh");
                            SetOrgAddShowHideRentedInfo();

                            $("#selOrgFOWSTContractStartMonth").val(vData.OrganizationFOWSTData.Rows[0].ContractStartMonth || '');//.selectmenu('refresh');
                            $("#selOrgFOWSTContractStartYear").val(vData.OrganizationFOWSTData.Rows[0].ContractStartYear || '');//.selectmenu('refresh');
                            setDayCollection('selOrgFOWSTContractStartMonth', 'selOrgFOWSTContractStartDay', 'selOrgFOWSTContractStartYear', false);
                            $("#selOrgFOWSTContractStartDay").val(vData.OrganizationFOWSTData.Rows[0].ContractStartDay || '');//.selectmenu('refresh');

                            $("#selOrgFOWSTContractEndMonth").val(vData.OrganizationFOWSTData.Rows[0].ContractEndMonth || '');//.selectmenu('refresh');
                            $("#selOrgFOWSTContractEndYear").val(vData.OrganizationFOWSTData.Rows[0].ContractEndYear || '');//.selectmenu('refresh');
                            setDayCollection('selOrgFOWSTContractEndMonth', 'selOrgFOWSTContractEndDay', 'selOrgFOWSTContractEndYear', false);
                            $("#selOrgFOWSTContractEndDay").val(vData.OrganizationFOWSTData.Rows[0].ContractEndDay || '');//.selectmenu('refresh');

                            $("#txtOrgFOWSTCostPerMonth").val(vData.OrganizationFOWSTData.Rows[0].CostPerMonth || '');

                            $("#selOrgFOWSTIsMortgaged").prop('checked', CInt2(vData.OrganizationFOWSTData.Rows[0].IsMortgaged || 1));//.slider("refresh");
                            $("#selEnforceZRead").prop('checked', CInt2(vData.OrganizationFOWSTData.Rows[0].EnforceZRead || 0));//.slider("refresh");

                            $("#selOrgFOWSTInsideMall").prop('checked', CInt2(vData.OrganizationFOWSTData.Rows[0].IsFOWSTInsideAMall || 0));//.slider("refresh");
                            if ($("#selOrgFOWSTInsideMall").is(':checked')) {
                                $("#divOrgFOWSTInsideMall").show();
                            }
                            else {
                                $("#divOrgFOWSTInsideMall").hide();
                            }

                            $("#selOrgFOWSTMallName").val(vData.OrganizationFOWSTData.Rows[0].FOWSTMallName || '');//.selectmenu("refresh");
                            SetOrgAddShowHideMallInfo();
                            $("#txtOrgFOWSTFtpServer").val(vData.OrganizationFOWSTData.Rows[0].FtpServer || '');
                            $("#txtOrgFOWSTFtpUsername").val(vData.OrganizationFOWSTData.Rows[0].FtpUsername || '');
                            $("#txtOrgFOWSTFtpPassword").val(vData.OrganizationFOWSTData.Rows[0].FtpPassword || '');
                            $("#chkOrgFOWSTOperates247").prop('checked', CInt2(vData.OrganizationFOWSTData.Rows[0].Operates247 || 0));//.slider("refresh");

                            $("#txtOrgFOWSTAyalaMallRecID").val(vData.OrganizationFOWSTData.Rows[0].AyalaMallRecID || '');
                            $("#txtOrgFOWSTAyalaMallTenantCode").val(vData.OrganizationFOWSTData.Rows[0].AyalaTenantCode || '');
                            $("#txtOrgFOWSTAyalaMallTenantName").val(vData.OrganizationFOWSTData.Rows[0].AyalaTenantName || '');
                            $("#txtOrgFOWSTAyalaMallTenantNameForDBF").val(vData.OrganizationFOWSTData.Rows[0].TenantNameForDBF || '');
                            $("#txtOrgFOWSTAyalaMallMerchantsLeaseOutNumber").val(vData.OrganizationFOWSTData.Rows[0].MerchantsLeaseOutNumber || '');

                            $("#txtOrgFOWSTSMMallRecID").val(vData.OrganizationFOWSTData.Rows[0].SMMallRecID || '');
                            $("#txtOrgFOWSTSMMallCode").val(vData.OrganizationFOWSTData.Rows[0].SMMallCode || '');
                            $("#txtOrgFOWSTSMMallTenantNo").val(vData.OrganizationFOWSTData.Rows[0].SMMallTenantNo || '');
                            $("#txtOrgFOWSTSMMallClassCode").val(vData.OrganizationFOWSTData.Rows[0].SMMallClassCode || '');
                            $("#txtOrgFOWSTSMMallTradeCode").val(vData.OrganizationFOWSTData.Rows[0].SMMallTradeCode || '');
                            $("#txtOrgFOWSTSMMallOutletNo").val(vData.OrganizationFOWSTData.Rows[0].SMMallOutletNo || '');
                            $("#txtOrgFOWSTSMMallSalesType").val(vData.OrganizationFOWSTData.Rows[0].SMMallSaleType || '');

                            $("#txtOrgFOWSTMegaworldRecID").val(vData.OrganizationFOWSTData.Rows[0].MegaworldRecID || '');
                            $("#txtOrgFOWSTMegaworldTenantNo").val(vData.OrganizationFOWSTData.Rows[0].MegaworldTenantCode || '');
                            $("#txtOrgFOWSTMegaworldSalesType").val(vData.OrganizationFOWSTData.Rows[0].MegaworldSaleType || '');
                            $("#txtOrgFOWSTMegaworldNoOfPOSTerminal").val(vData.OrganizationFOWSTData.Rows[0].MegaworldTenantPOSTerminalNo || '');
                            
                            $("#txtOrgFOWSTRobinsonTenantId").val(vData.OrganizationFOWSTData.Rows[0].RobinsonTenantId || '');
                            $("#txtOrgFOWSTRobinsonPosTerminalNum").val(vData.OrganizationFOWSTData.Rows[0].RobinsonPosTerminalNum || '');

                            $("#txtOrgFOWSTPITXTenantId").val(vData.OrganizationFOWSTData.Rows[0].PITXTenantId || '');
                            $("#txtOrgFOWSTPITXPosTerminalID").val(vData.OrganizationFOWSTData.Rows[0].PITXPosTerminalID || '');
                                                        
                            //if (CStr2(vData.OrganizationFOWSTData.Rows[0].FOWSTTypeID).toUpperCase() == '6AF9BC83-426C-4774-A051-044FC09C552B') { //Branch
                                                                
                            //} else if (CStr2(vData.OrganizationFOWSTData.Rows[0].FOWSTTypeID).toUpperCase() == '247C961C-A219-45DF-BDA3-3C137B734ABF') { //Warehouse
                                
                            //}
                            $("#selOrgFOWSTRetailStoreType").val(vData.OrganizationFOWSTData.Rows[0].SubFOWSTTypeID || '');//.selectmenu('refresh');
                            $("#selOrgFOWSTRetailStoreLocationType").val(vData.OrganizationFOWSTData.Rows[0].StoreLocationTypeID || '');//.selectmenu('refresh');
                            $("#selOrgFOWSTWarehouseType").val(vData.OrganizationFOWSTData.Rows[0].SubFOWSTTypeID || '');//.selectmenu('refresh');

                            $("#txtOrgFOWSTPlateNo").val(vData.OrganizationFOWSTData.Rows[0].PlateNo || '');
                            $("#txtOrgFOWSTNoOfWheels").val(vData.OrganizationFOWSTData.Rows[0].NoOfWheels || '');
                            $("#txtOrgFOWSTBodyColor").val(vData.OrganizationFOWSTData.Rows[0].BodyColor || '');
                            $("#txtOrgFOWSTLoadLimitWeight").val(vData.OrganizationFOWSTData.Rows[0].Weight || '');
                            $("#txtOrgFOWSTLoadLimitDimensions").val(vData.OrganizationFOWSTData.Rows[0].Dimension || '');

                            $("#selOrgFOWSTIsInclude").prop('checked', CInt2(vData.OrganizationFOWSTData.Rows[0].IsIncludeSpecialReport));//.slider("refresh");
                            $("#txtAccountID").val(vData.OrganizationFOWSTData.Rows[0].AccountID);
                            $("#txtStoreID").val(vData.OrganizationFOWSTData.Rows[0].StoreID);
                            $("#txtAccountID2").val(vData.OrganizationFOWSTData.Rows[0].AccountID2);
                            $("#txtStoreID2").val(vData.OrganizationFOWSTData.Rows[0].StoreID2);

                            $("#selOrgFOWSTIsIncludeToShopperTrak").prop('checked', CInt2(vData.OrganizationFOWSTData.Rows[0].IsIncludeToShopperTrakReport));//.slider("refresh");
                            $("#txtShopperTrakAccountID").val(vData.OrganizationFOWSTData.Rows[0].ShopperTrakAccountID);
                            $("#selEnableHotButtonsContextualMenu").prop('checked', CBool(vData.OrganizationFOWSTData.Rows[0].EnableHotButtonsContextualMenu) ? 1 : 0);//.slider('refresh');
                            $("#selEnablePrintStickers").prop('checked', CBool(vData.OrganizationFOWSTData.Rows[0].PrintStickers) ? 1 : 0);//.slider('refresh');
                            $("#selPrintQueueNumbers").prop('checked', CBool(vData.OrganizationFOWSTData.Rows[0].PrintQueueNumbers) ? 1 : 0);//.slider('refresh');
                            $("#selDefaultPosView").val(vData.OrganizationFOWSTData.Rows[0].DefaultPosView);//.selectmenu('refresh');
                            $("#selDefaultHBTemplate").val((vData.OrganizationFOWSTData.Rows[0].DefaultHotButtonTemplateID).toLowerCase());//.selectmenu('refresh');
                            $("[name='rdoFowstVATType'][value='" + vData.OrganizationFOWSTData.Rows[0].IsVAT + "']").prop('checked', 1);//.slider("refresh");
                            mPosVendorID = vData.OrganizationFOWSTData.Rows[0].PosVendorID;
                            if (CStr2(mPosVendorID) == '') {
                                $('#radPosVendor1').prop('checked', 1);
                                $('#butPosVendor').text('Please Select a 3rd POS Vendor');
                            } else {
                                $('#radPosVendor2').prop('checked', 2);
                                $('#butPosVendor').text(vData.OrganizationFOWSTData.Rows[0].PosVendorName);
                                $('#div3rdPartyPosVendor').show();
                            }
                            pData.OrgFOWSTID = vData.OrganizationFOWSTData.Rows[0].ID;

                            //Get LTO
                            AjaxSend({
                                url: 'Organization/getOrgFOWSTLTOList',
                                data: pData,
                                success: function (vData) {
                                    LoadOrgAddLTO(vData.OrganizationFOWSTData.Rows);
                                }
                            });

                            //Get LTFRB
                            AjaxSend({
                                url: 'Organization/getOrgFOWSTLTFRBList',
                                data: pData,
                                success: function (vData) {
                                    LoadOrgAddLTFRB(vData.OrganizationFOWSTData.Rows);
                                }
                            });

                            //Get Section
                            AjaxSend({
                                url: 'Organization/getOrgFOWSTSectionList',
                                data: pData,
                                success: function (vData) {
                                    LoadOrgAddSection(vData.OrganizationFOWSTData.Rows);
                                }
                            });

                            //Get BPermit
                            AjaxSend({
                                url: 'Organization/getOrgFOWSTBPermitList',
                                data: pData,
                                success: function (vData) {
                                    LoadOrgAddBPermit(vData.OrganizationFOWSTData.Rows);
                                }
                            });

                            //Get Tel
                            AjaxSend({
                                url: 'Organization/getOrgFOWSTTelList',
                                data: pData,
                                success: function (vData) {
                                    LoadOrgAddTel(vData.OrganizationFOWSTData.Rows);
                                }
                            });

                            //Get mobile
                            AjaxSend({
                                url: 'Organization/getOrgFOWSTMobileList',
                                data: pData,
                                success: function (vData) {
                                    LoadOrgAddMobile(vData.OrganizationFOWSTData.Rows);
                                }
                            });

                            //Get Email
                            AjaxSend({
                                url: 'Organization/getOrgFOWSTEmailList',
                                data: pData,
                                success: function (vData) {
                                    LoadOrgAddEmail(vData.OrganizationFOWSTData.Rows);
                                }
                            });

                            //Get Operation HR
                            AjaxSend({
                                url: 'Organization/getOrgFOWSTOperationHRList',
                                data: pData,
                                success: function (vData) {
                                    LoadOrgAddOperationHR(vData.OrganizationFOWSTData.Rows);
                                }
                            });

                            //Get Printers
                            if (fowstID != '')
                                GetAllFOWSTPrinter();

                            //temp commented by arnie 10-11-2022 2:24 PM - wala pa ni na table sa server
                            //AjaxSend({
                            //    url: 'Browser/LibraryFOWSTPriceLevel',
                            //    data: { FowstID: fowstid },
                            //    success: function (vData) {
                            //        arrFowstPriceLevels = vData.Results.Rows;

                            //        $.grep(arrFowstPriceLevels, function (e) {
                            //            e.State = 'old';
                            //        });

                            //        DisplayFowstPriceLevels()
                            //    }
                            //});
                        }

                    }
                });
            }

            function LoadOrgAddLTO(vRows) {

                for (var p = 0; p <= vRows.length - 1; p++) {

                    arOrgAddLTO = new Array();
                    arOrgAddLTO[intOrgFowstLTORecID] = vRows[p].ID;
                    arOrgAddLTO[intOrgFowstLTOORNo] = vRows[p].ORNo;
                    arOrgAddLTO[intOrgFowstLTODateIssuedMonth] = vRows[p].DateIssuedMonth;
                    arOrgAddLTO[intOrgFowstLTODateIssuedDay] = vRows[p].DateIssuedDay;
                    arOrgAddLTO[intOrgFowstLTODateIssuedYear] = vRows[p].DateIssuedYear;
                    arOrgAddLTO[intOrgFowstLTODateExpirationMonth] = vRows[p].ExpirationDateMonth;
                    arOrgAddLTO[intOrgFowstLTODateExpirationDay] = vRows[p].ExpirationDateDay;
                    arOrgAddLTO[intOrgFowstLTODateExpirationYear] = vRows[p].ExpirationDateYear;
                    arOrgAddLTO[intOrgFowstLTOState] = "";

                    sessionStorage["OrgAddLTO-" + p] = JSON.stringify(arOrgAddLTO);

                };

                intOrgFowstLTOCntr = vRows.length;
                LoadOrgFOWSTLTOList();

            };

            function LoadOrgAddLTFRB(vRows) {

                for (var p = 0; p <= vRows.length - 1; p++) {

                    arOrgAddLTFRB = new Array();
                    arOrgAddLTFRB[intOrgFowstLTFRBRecID] = vRows[p].ID;
                    arOrgAddLTFRB[intOrgFowstLTFRBORNo] = vRows[p].ORNo;
                    arOrgAddLTFRB[intOrgFowstLTFRBDateIssuedMonth] = vRows[p].DateIssuedMonth;
                    arOrgAddLTFRB[intOrgFowstLTFRBDateIssuedDay] = vRows[p].DateIssuedDay;
                    arOrgAddLTFRB[intOrgFowstLTFRBDateIssuedYear] = vRows[p].DateIssuedYear;
                    arOrgAddLTFRB[intOrgFowstLTFRBDateExpirationMonth] = vRows[p].ExpirationDateMonth;
                    arOrgAddLTFRB[intOrgFowstLTFRBDateExpirationDay] = vRows[p].ExpirationDateDay;
                    arOrgAddLTFRB[intOrgFowstLTFRBDateExpirationYear] = vRows[p].ExpirationDateYear;
                    arOrgAddLTFRB[intOrgFowstLTFRBState] = "";

                    sessionStorage["OrgAddLTFRB-" + p] = JSON.stringify(arOrgAddLTFRB);

                };

                intOrgFowstLTFRBCntr = vRows.length;
                LoadOrgFOWSTLTFRBList();

            };

            function LoadOrgAddSection(vRows) {
                arrFowstSection = vRows;
                //for (var p = 0; p <= vRows.length - 1; p++) {

                //    arOrgAddSection = new Array();
                //    arOrgAddSection[intOrgFowstSecRecID] = vRows[p].ID;
                //    arOrgAddSection[intOrgFowstSecCode] = vRows[p].Code;
                //    arOrgAddSection[intOrgFowstSecName] = vRows[p].SecName;
                //    arOrgAddSection[intOrgFowstSecDescription] = vRows[p].Description;
                //    arOrgAddSection[intOrgFowstSecFloorArea] = vRows[p].FloorArea;
                //    arOrgAddSection[intOrgFowstSecSpecialFeatures] = vRows[p].SpecialFeatures;
                //    arOrgAddSection[intOrgFowstSecState] = "";

                //    sessionStorage["OrgAddSection-" + p] = JSON.stringify(arOrgAddSection);

                //};

                //intOrgFowstSectionCntr = vRows.length;
                LoadOrgFOWSTSectionList();

            };

            function LoadOrgAddBPermit(vRows) {
                arrFowstBusinessPermit = vRows;
                //for (var p = 0; p <= vRows.length - 1; p++) {

                //    arOrgAddBPermit = new Array();
                //    arOrgAddBPermit[intOrgFowstBPermitRecID] = vRows[p].ID;
                //    arOrgAddBPermit[intOrgFowstBPermitNo] = vRows[p].BusinessPermitNo;
                //    arOrgAddBPermit[intOrgFowstBPermitBusinessID] = vRows[p].BusinessID;
                //    arOrgAddBPermit[intOrgFowstBPermitTradeName] = vRows[p].TradeName;
                //    arOrgAddBPermit[intOrgFowstBPermitBuildingHouseNo] = vRows[p].BuildingHouseNo;
                //    arOrgAddBPermit[intOrgFowstBPermitStreet] = vRows[p].Street;
                //    arOrgAddBPermit[intOrgFowstBPermitCityID] = vRows[p].AddressCityID;
                //    arOrgAddBPermit[intOrgFowstBPermitCity] = vRows[p].AddressCity;
                //    arOrgAddBPermit[intOrgFowstBPermitCountryID] = vRows[p].AddressCountryID;
                //    arOrgAddBPermit[intOrgFowstBPermitCountry] = vRows[p].AddressCountry;
                //    arOrgAddBPermit[intOrgFowstBPermitORNo] = vRows[p].ORNo;
                //    arOrgAddBPermit[intOrgFowstBPermitBusinessPlateNo] = vRows[p].BusinessPlateNo;
                //    arOrgAddBPermit[intOrgFowstBPermitDatePaymentMonth] = vRows[p].DateOfPaymentMonth;
                //    arOrgAddBPermit[intOrgFowstBPermitDatePaymentDay] = vRows[p].DateOfPaymentDay;
                //    arOrgAddBPermit[intOrgFowstBPermitDatePaymentYear] = vRows[p].DateOfPaymentYear;
                //    arOrgAddBPermit[intOrgFowstBPermitDateIssuedMonth] = vRows[p].DateIssuedMonth;
                //    arOrgAddBPermit[intOrgFowstBPermitDateIssuedDay] = vRows[p].DateIssuedDay;
                //    arOrgAddBPermit[intOrgFowstBPermitDateIssuedYear] = vRows[p].DateIssuedYear;
                //    arOrgAddBPermit[intOrgFowstBPermitState] = "";

                //    sessionStorage["OrgAddBPermit-" + p] = JSON.stringify(arOrgAddBPermit);

                //};

                //intOrgFowstBPermitCntr = vRows.length;
                LoadOrgFOWSTBPermitList();

            };

            function LoadOrgAddTel(vRows) {
                arrFowstTel = vRows;
                //for (var p = 0; p <= vRows.length - 1; p++) {

                //    arOrgAddTel = new Array();
                //    arOrgAddTel[intOrgFowstTelRecID] = vRows[p].ID;
                //    arOrgAddTel[intOrgFowstTelCountryID] = vRows[p].CountryID;
                //    arOrgAddTel[intOrgFowstTelCountryCode] = vRows[p].Country;
                //    arOrgAddTel[intOrgFowstTelCallingCode] = vRows[p].CountryCode;
                //    arOrgAddTel[intOrgFowstTelAreaCodeID] = vRows[p].AreaCodeID;
                //    arOrgAddTel[intOrgFowstTelArea] = vRows[p].Area;
                //    arOrgAddTel[intOrgFowstTelAreaCode] = vRows[p].AreaCode;
                //    arOrgAddTel[intOrgFowstTelContactNo] = vRows[p].ContactNo;
                //    arOrgAddTel[intOrgFowstTelExtNo] = vRows[p].ExtensionNo;
                //    arOrgAddTel[intOrgFowstTelContactType] = vRows[p].ContactType;
                //    arOrgAddTel[intOrgFowstTelState] = "";

                //    sessionStorage["OrgAddTel-" + p] = JSON.stringify(arOrgAddTel);

                //};

                //intOrgFowstTelCntr = vRows.length;
                LoadOrgFOWSTTelList();

            };

            function LoadOrgAddMobile(vRows) {
                arrFowstMobile = vRows;
                //for (var p = 0; p <= vRows.length - 1; p++) {

                //    arOrgAddMobile = new Array();
                //    arOrgAddMobile[intOrgFowstMobileRecID] = vRows[p].ID;
                //    arOrgAddMobile[intOrgFowstMobileCountryID] = vRows[p].CountryID;
                //    arOrgAddMobile[intOrgFowstMobileCountryCode] = vRows[p].Country;
                //    arOrgAddMobile[intOrgFowstMobileCallingCode] = vRows[p].CountryCode;
                //    arOrgAddMobile[intOrgFowstMobileTelCoPrefixID] = vRows[p].TelCoPrefixID;
                //    arOrgAddMobile[intOrgFowstMobilePrefix] = vRows[p].Prefix;
                //    arOrgAddMobile[intOrgFowstMobileTelCo] = vRows[p].TelCo;
                //    arOrgAddMobile[intOrgFowstMobileContactNo] = vRows[p].ContactNo;
                //    arOrgAddMobile[intOrgFowstMobileContactType] = vRows[p].ContactType;
                //    arOrgAddMobile[intOrgFowstMobileState] = "";

                //    sessionStorage["OrgAddMobile-" + p] = JSON.stringify(arOrgAddMobile);

                //};

                //intOrgFowstMobileCntr = vRows.length;
                LoadOrgFOWSTMobileList();

            };

            function LoadOrgAddEmail(vRows) {
                arrFowstEmail = vRows;
                //for (var p = 0; p <= vRows.length - 1; p++) {

                //    arOrgAddEmail = new Array();
                //    arOrgAddEmail[intOrgFowstEmailRecID] = vRows[p].ID;
                //    arOrgAddEmail[intOrgFowstEmailAddress] = vRows[p].EmailAddress;
                //    arOrgAddEmail[intOrgFowstEmailType] = vRows[p].EmailType;
                //    arOrgAddEmail[intOrgFowstEmailState] = "";

                //    sessionStorage["OrgAddEmail-" + p] = JSON.stringify(arOrgAddEmail);

                //};

                //intOrgFowstEmailCntr = vRows.length;
                LoadOrgFOWSTEmailList();

            };

            function LoadOrgAddOperationHR(vRows) {

                for (var p = 0; p <= vRows.length - 1; p++) {

                    arOrgAddOptHR = new Array();
                    arOrgAddOptHR[intOrgFowstOptHRRecID] = vRows[p].ID;
                    arOrgAddOptHR[intOrgFowstOptHRWeekDay] = vRows[p].WeekDay;
                    arOrgAddOptHR[intOrgFowstOptHROpeningTimeHR] = vRows[p].OpeningTimeHour;
                    arOrgAddOptHR[intOrgFowstOptHROpeningTimeMin] = vRows[p].OpeningTimeMinute;
                    arOrgAddOptHR[intOrgFowstOptHROpeningTimeAMPM] = vRows[p].OpeningTimeAMPM;
                    arOrgAddOptHR[intOrgFowstOptHRClosingTimeHR] = vRows[p].ClosingTimeHour;
                    arOrgAddOptHR[intOrgFowstOptHRClosingTimeMin] = vRows[p].ClosingTimeMinute;
                    arOrgAddOptHR[intOrgFowstOptHRClosingTimeAMPM] = vRows[p].ClosingTimeAMPM;
                    arOrgAddOptHR[intOrgFowstOptHRIsClosed] = vRows[p].IsClosed;

                    sessionStorage["OrgAddOptHR-" + p] = JSON.stringify(arOrgAddOptHR);

                };

                intOrgFowstOptHRCntr = vRows.length;
                LoadOrgFOWSTOperationHRList();

            };

            function ShowOrgAddLTOInfo(arData, index) {

                //$.mobile.changePage('#pageOrgFOWSTTruckRegInfo');
                $('#pageOrgFOWSTInfo').hide();
                $('#pageOrgFOWSTTruckRegInfo').show();
                $('#pageOrgFOWSTSectionInfo').hide();
                $('#pageOrgFOWSTBusinessPermitInfo').hide();
                $('#pageOrgFOWSTTelInfo').hide();
                $('#pageOrgFOWSTMobileInfo').hide();
                $('#pageOrgFOWSTEmailInfo').hide();
                $('#pageOrgFOWSTOperationHRInfo').hide();
                $('#pageOrgFOWSTPrinterInfo').hide();
                $('#pageOrgFOWSTPrinterIP').hide(); $('#pageAddDevicePrinter').hide();

                ClearOrgAddTruckFields();

                intOrgFowstLTOIndex = index;

                $("#txtOrgFOWSTTRegID").val(arData[intOrgFowstLTORecID]);

                $("#txtOrgFOWSTTRegORNo").val(arData[intOrgFowstLTOORNo]);

                $("#selOrgFOWSTTRegIssuedMonth").val(arData[intOrgFowstLTODateIssuedMonth]);//.selectmenu('refresh');
                $("#selOrgFOWSTTRegIssuedDay").val(arData[intOrgFowstLTODateIssuedDay]);//.selectmenu('refresh');
                $("#selOrgFOWSTTRegIssuedYear").val(arData[intOrgFowstLTODateIssuedYear]);//.selectmenu('refresh');

                $("#selOrgFOWSTTRegExpirationMonth").val(arData[intOrgFowstLTODateExpirationMonth]);//.selectmenu('refresh');
                $("#selOrgFOWSTTRegExpirationDay").val(arData[intOrgFowstLTODateExpirationDay]);//.selectmenu('refresh');
                $("#selOrgFOWSTTRegExpirationYear").val(arData[intOrgFowstLTODateExpirationYear]);//.selectmenu('refresh');

                IsNewOrgAddLTO = false;

                $("#btnOrgFOWSTTRegRemove").show();

            };

            function ShowOrgAddLTFRBInfo(arData, index) {

                //$.mobile.changePage('#pageOrgFOWSTTruckRegInfo');
                $('#pageOrgFOWSTInfo').hide();
                $('#pageOrgFOWSTTruckRegInfo').show();
                $('#pageOrgFOWSTSectionInfo').hide();
                $('#pageOrgFOWSTBusinessPermitInfo').hide();
                $('#pageOrgFOWSTTelInfo').hide();
                $('#pageOrgFOWSTMobileInfo').hide();
                $('#pageOrgFOWSTEmailInfo').hide();
                $('#pageOrgFOWSTOperationHRInfo').hide();
                $('#pageOrgFOWSTPrinterInfo').hide();
                $('#pageOrgFOWSTPrinterIP').hide(); $('#pageAddDevicePrinter').hide();

                ClearOrgAddTruckFields();

                intOrgFowstLTFRBIndex = index;

                $("#txtOrgFOWSTTRegID").val(arData[intOrgFowstLTFRBRecID]);

                $("#txtOrgFOWSTTRegORNo").val(arData[intOrgFowstLTFRBORNo]);

                $("#selOrgFOWSTTRegIssuedMonth").val(arData[intOrgFowstLTFRBDateIssuedMonth]);//.selectmenu('refresh');
                $("#selOrgFOWSTTRegIssuedDay").val(arData[intOrgFowstLTFRBDateIssuedDay]);//.selectmenu('refresh');
                $("#selOrgFOWSTTRegIssuedYear").val(arData[intOrgFowstLTFRBDateIssuedYear]);//.selectmenu('refresh');

                $("#selOrgFOWSTTRegExpirationMonth").val(arData[intOrgFowstLTFRBDateExpirationMonth]);//.selectmenu('refresh');
                $("#selOrgFOWSTTRegExpirationDay").val(arData[intOrgFowstLTFRBDateExpirationDay]);//.selectmenu('refresh');
                $("#selOrgFOWSTTRegExpirationYear").val(arData[intOrgFowstLTFRBDateExpirationYear]);//.selectmenu('refresh');

                IsNewOrgAddLTFRB = false;

                $("#btnOrgFOWSTTRegRemove").show();

            };

            function ShowOrgAddSectionInfo(arData, index) {

                //$.mobile.changePage('#pageOrgFOWSTSectionInfo');
                $('#pageOrgFOWSTInfo').hide();
                $('#pageOrgFOWSTTruckRegInfo').hide();
                $('#pageOrgFOWSTSectionInfo').show();
                $('#pageOrgFOWSTBusinessPermitInfo').hide();
                $('#pageOrgFOWSTTelInfo').hide();
                $('#pageOrgFOWSTMobileInfo').hide();
                $('#pageOrgFOWSTEmailInfo').hide();
                $('#pageOrgFOWSTOperationHRInfo').hide();
                $('#pageOrgFOWSTPrinterInfo').hide();
                $('#pageOrgFOWSTPrinterIP').hide(); $('#pageAddDevicePrinter').hide();

                ClearOrgAddSectionFields();

                intOrgFowstSectionIndex = index;

                $("#txtOrgFOWSTSecID").val(arData.ID);

                $("#txtOrgFOWSTSecCode").val(arData.Code);
                $("#txtOrgFOWSTSecName").val(arData.Name);
                $("#txtOrgFOWSTSecDescription").val(arData.Description);

                $("#txtOrgFOWSTSecFloorArea").val(arData.FloorArea);
                $("#txtOrgFOWSTSecSpecFeatures").val(arData.SpecialFeatures);

                IsNewOrgAddSection = false;

                $("#btnOrgFOWSTSecRemove").show();

                GetOrgFOWSTSectionBinList();
                GetOrgFOWSTSectionPeopleList();
            };

            function ShowOrgAddBPermitInfo(arData, index) {

                //$.mobile.changePage('#pageOrgFOWSTBusinessPermitInfo');
                $('#pageOrgFOWSTInfo').hide();
                $('#pageOrgFOWSTTruckRegInfo').hide();
                $('#pageOrgFOWSTSectionInfo').hide();
                $('#pageOrgFOWSTBusinessPermitInfo').show();
                $('#pageOrgFOWSTTelInfo').hide();
                $('#pageOrgFOWSTMobileInfo').hide();
                $('#pageOrgFOWSTEmailInfo').hide();
                $('#pageOrgFOWSTOperationHRInfo').hide();
                $('#pageOrgFOWSTPrinterInfo').hide();
                $('#pageOrgFOWSTPrinterIP').hide(); $('#pageAddDevicePrinter').hide();

                ClearOrgAddBPermitFields();

                intOrgFowstBPermitIndex = index;

                $("#txtOrgFOWSTBPermitRecID").val(arData.ID);

                $("#txtOrgFOWSTBPermitNo").val(arData.BusinessPermitNo);
                $("#txtOrgFOWSTBPermitID").val(arData.BusinessID);
                $("#txtOrgFOWSTBPermitTradeName").val(arData.TradeName);

                $("#txtOrgFOWSTBPermitBuildingNo").val(arData.BuildingHouseNo);
                $("#txtOrgFOWSTBPermitStreet").val(arData.Street);

                $("#txtOrgFOWSTBPermitCityID").val(arData.AddressCityID);
                $("#btnOrgFOWSTBPermitCity").text(arData.AddressCity);//.button('refresh');
                $("#txtOrgFOWSTBPermitCountryID").val(arData.AddressCountryID);
                $("#btnOrgFOWSTBPermitCountry").text(arData.AddressCountry);//.button('refresh');

                $("#txtOrgFOWSTBPermitORNo").val(arData.ORNo);

                if (CStr2(arData.DatePayment) != "") {
                    dateofpayment = arData.DatePayment;
                    $('#btnDateOfPayment').text(arData.DatePayment);//.button('refresh');
                }

                if (CStr2(arData.DateIssued) != "") {
                    dateissued = arData.DateIssued;
                    $('#btnDateIssued').text(arData.DateIssued);//.button('refresh');
                }

                //$("#selOrgFOWSTBPermitDateOfPaymentMonth").val(arData.DateOfPaymentMonth);//.selectmenu('refresh');
                //$("#selOrgFOWSTBPermitDateOfPaymentDay").val(arData.DateOfPaymentDay);//.selectmenu('refresh');
                //$("#selOrgFOWSTBPermitDateOfPaymentYear").val(arData.DateOfPaymentYear);//.selectmenu('refresh');

                $("#txtOrgFOWSTBPermitBPlateNo").val(arData.BusinessPlateNo);

                //$("#selOrgFOWSTBPermitBPlateIssuedMonth").val(arData.DateIssuedMonth);//.selectmenu('refresh');
                //$("#selOrgFOWSTBPermitBPlateIssuedDay").val(arData.DateIssuedDay);//.selectmenu('refresh');
                //$("#selOrgFOWSTBPermitBPlateIssuedYear").val(arData.DateIssuedYear);//.selectmenu('refresh');

                IsNewOrgAddBPermit = false;

                $("#btnOrgFOWSTBPermitRemove").show();

            };

            function ShowOrgAddTelInfo(arData, index) {

                //$.mobile.changePage('#pageOrgFOWSTTelInfo');
                $('#pageOrgFOWSTInfo').hide();
                $('#pageOrgFOWSTTruckRegInfo').hide();
                $('#pageOrgFOWSTSectionInfo').hide();
                $('#pageOrgFOWSTBusinessPermitInfo').hide();
                $('#pageOrgFOWSTTelInfo').show();
                $('#pageOrgFOWSTMobileInfo').hide();
                $('#pageOrgFOWSTEmailInfo').hide();
                $('#pageOrgFOWSTOperationHRInfo').hide();
                $('#pageOrgFOWSTPrinterInfo').hide();
                $('#pageOrgFOWSTPrinterIP').hide(); $('#pageAddDevicePrinter').hide();

                ClearOrgAddTelFields();

                intOrgFowstTelIndex = index;

                $("#txtOrgFOWSTTelID").val(arData.ID);

                $("#txtOrgFOWSTTelCountryID").val(arData.CountryID);
                $("#btnOrgFOWSTTelCountryCode").text(arData.Country + ' - ' + arData.CountryCode);//.button('refresh');

                $("#txtOrgFOWSTTelAreaCodeID").val(arData.AreaCodeID);
                $("#btnOrgFOWSTTelAreaCode").text(arData.Area + ' - ' + arData.AreaCode);//.button('refresh');

                $("#txtOrgFOWSTTelNo").val(arData.ContactNo);
                $("#txtOrgFOWSTTelExtension").val(arData.ExtensionNo);

                $("#selOrgFOWSTTelType").val(arData.ContactType);//.selectmenu('refresh');

                IsNewOrgAddTelContact = false;

                $("#btnOrgFOWSTTelRemove").show();

            };

            function ShowOrgAddMobileInfo(arData, index) {

                //$.mobile.changePage('#pageOrgFOWSTMobileInfo');
                $('#pageOrgFOWSTInfo').hide();
                $('#pageOrgFOWSTTruckRegInfo').hide();
                $('#pageOrgFOWSTSectionInfo').hide();
                $('#pageOrgFOWSTBusinessPermitInfo').hide();
                $('#pageOrgFOWSTTelInfo').hide();
                $('#pageOrgFOWSTMobileInfo').show();
                $('#pageOrgFOWSTEmailInfo').hide();
                $('#pageOrgFOWSTOperationHRInfo').hide();
                $('#pageOrgFOWSTPrinterInfo').hide();
                $('#pageOrgFOWSTPrinterIP').hide(); $('#pageAddDevicePrinter').hide();

                ClearOrgAddMobileFields();

                intOrgFowstMobileIndex = index;

                $("#txtOrgFOWSTMobileID").val(arData.ID || '');

                $("#txtOrgFOWSTMobileCountryID").val(arData.CountryID || '');
                $("#btnOrgFOWSTMobileCountryCode").text((arData.CountryCode + ' - ' + arData.Country) || 'Country Code');//.button('refresh');

                $("#txtOrgFOWSTTelCoPrefixID").val(arData.TelCoPrefixID || '');
                $("#btnOrgFOWSTMobileTelCoPrefixCode").text((arData.TelCo + ' - ' + arData.Prefix) || 'Telco Code');//.button('refresh');

                $("#txtOrgFOWSTMobileNo").val(arData.ContactNo || '');
                $("#selOrgAddMobileType").val(arData.ContactType || '');//.selectmenu('refresh');

                IsNewOrgAddMobileContact = false;

                $("#btnOrgFOWSTMobileRemove").show();

            };

            function ShowOrgAddEmailInfo(arData, index) {

                //$.mobile.changePage('#pageOrgFOWSTEmailInfo');
                $('#pageOrgFOWSTInfo').hide();
                $('#pageOrgFOWSTTruckRegInfo').hide();
                $('#pageOrgFOWSTSectionInfo').hide();
                $('#pageOrgFOWSTBusinessPermitInfo').hide();
                $('#pageOrgFOWSTTelInfo').hide();
                $('#pageOrgFOWSTMobileInfo').hide();
                $('#pageOrgFOWSTEmailInfo').show();
                $('#pageOrgFOWSTOperationHRInfo').hide();
                $('#pageOrgFOWSTPrinterInfo').hide();
                $('#pageOrgFOWSTPrinterIP').hide(); $('#pageAddDevicePrinter').hide();

                ClearOrgAddEmailFields();

                intOrgFowstEmailIndex = index;

                $("#txtOrgFOWSTEmailID").val(arData.ID || '');

                $("#txtOrgFOWSTEmail").val(arData.EmailAddress || '');
                $("#selOrgFOWSTEmailType").val(arData.EmailType || '');//.selectmenu('refresh');

                IsNewOrgAddEmailContact = false;

                $("#btnOrgFOWSTEmailRemove").show();


            };

            function ShowOrgAddOperationHRInfo(arData, index) {

                //$.mobile.changePage('#pageOrgFOWSTOperationHRInfo');
                $('#pageOrgFOWSTInfo').hide();
                $('#pageOrgFOWSTTruckRegInfo').hide();
                $('#pageOrgFOWSTSectionInfo').hide();
                $('#pageOrgFOWSTBusinessPermitInfo').hide();
                $('#pageOrgFOWSTTelInfo').hide();
                $('#pageOrgFOWSTMobileInfo').hide();
                $('#pageOrgFOWSTEmailInfo').hide();
                $('#pageOrgFOWSTOperationHRInfo').show();
                $('#pageOrgFOWSTPrinterInfo').hide();
                $('#pageOrgFOWSTPrinterIP').hide(); $('#pageAddDevicePrinter').hide();

                ClearOrgAddOperationHRFields();

                intOrgFowstOptHRIndex = index;

                $("#txtOrgFOWSTOptHRID").val(arData[intOrgFowstOptHRRecID] || '');
                $("#selOrgFOWSTOptHRDay").val(arData[intOrgFowstOptHRWeekDay] || '');//.selectmenu('refresh');

                $("#selOrgFOWSTOptOpenHR").val(arData[intOrgFowstOptHROpeningTimeHR] || '');//.selectmenu('refresh');
                $("#selOrgFOWSTOptOpenMin").val(arData[intOrgFowstOptHROpeningTimeMin] || '');//.selectmenu('refresh');
                $("#selOrgFOWSTOptOpenAMPM").val(arData[intOrgFowstOptHROpeningTimeAMPM] || '');//.selectmenu('refresh');

                $("#selOrgFOWSTOptCloseHR").val(arData[intOrgFowstOptHRClosingTimeHR] || '');//.selectmenu('refresh');
                $("#selOrgFOWSTOptCloseMin").val(arData[intOrgFowstOptHRClosingTimeMin] || '');//.selectmenu('refresh');
                $("#selOrgFOWSTOptCloseAMPM").val(arData[intOrgFowstOptHRClosingTimeAMPM] || '');//.selectmenu('refresh');
                $("#isFOWSTClosed").prop('checked', CInt2(arData[intOrgFowstOptHRIsClosed]));

                IsNewOrgAddOptHR = false;

            };

            function ClearOrgFOWSTFields() {

                // dont clear
                // sessionStorage.clear();
                //proj.ClearSessionStorage(); // commented by arnie on 12202016, all sessionStorages are needed in erp

                //sessionStorage["OrgName"] = orgname;
                //sessionStorage["LibraryName"] = libname;
                //sessionStorage["LabelLibraryName"] = lbllibname;
                //sessionStorage["AddViewLibBCName"] = addlibname;
                //sessionStorage["ViewID"] = fowstID;
                //sessionStorage["PeopleID"] = peopleid;

                arrFowstTel = [];
                arrFowstMobile = [];
                arrFowstEmail = [];
                arrFowstSection = [];
                arrFowstBusinessPermit = [];
                arrFowstPriceLevels = [];
                arrRemoveAssignedEmp = [];
                
                $("#txtOrgFOWSTID").val('');

                $("#selOrgFOWSTType").val('');//.selectmenu('refresh');
                $("#selOrgFOWSTStatus").val('Available');//.selectmenu('refresh');
                $("#selOrgFOWSTClassification").val('New Store');//.selectmenu('refresh');

                $("#txtOrgFOWSTCode").val('');
                $("#txtOrgFOWSTName").val('');
                $("#txtOrgFOWSTDescription").val('');
                $("#selOrgFOWSTTradeName").val('');//.selectmenu('refresh');
                $("#selOrgFOWSTReceiptSettings").val('');
                $("#txtTINBranchCode").val('');

                $("#txtOrgFOWSTVATRate").val('');
                $("#selOrgActiveFOWST").prop('checked', 1);//.slider('refresh');

                $("#txtOrgFOWSTBuildingNo").val('');
                $("#txtOrgFOWSTStreet").val('');
                $("#txtOrgFOWSTCityID").val('');
                $("#btnOrgFOWSTCity").text('Please Select');//.button();//.button('refresh');
                $("#txtOrgFOWSTCountryID").val('e783ed13-aa98-4cc1-918e-d49b05c0167e');
                $("#selOrgFOWSTRegion").val('');//.selectmenu('refresh');
                $("#btnOrgFOWSTCountry").text('Philippines');//.button();//.button('refresh');

                $("#selOrgAddFranchisee").prop('checked', 0);//.slider("refresh");
                $("#txtOrgFOWSTFranchisorID").val('');
                $("#btnOrgFOWSTFranchisor").text('Please Select');//.button();//.button('refresh');
                ShowOrgAddHideFranchisorDetails();

                $("#selOrgFOWSTOwnershipType").val('0');//.selectmenu('refresh');
                SetOrgAddShowHideRentedInfo();

                $("#selOrgFOWSTContractStartMonth").val('');//.selectmenu('refresh');
                $("#selOrgFOWSTContractStartDay").val('');//.selectmenu('refresh');
                $("#selOrgFOWSTContractStartYear").val('');//.selectmenu('refresh');

                $("#selOrgFOWSTContractEndMonth").val('');//.selectmenu('refresh');
                $("#selOrgFOWSTContractEndDay").val('');//.selectmenu('refresh');
                $("#selOrgFOWSTContractEndYear").val('');//.selectmenu('refresh');

                $("#txtOrgFOWSTCostPerMonth").val('');

                $("#selOrgFOWSTIsMortgaged").prop('checked', 1);//.slider("refresh");
                $("#selEnforceZRead").prop('checked', 0);//.slider("refresh");

                $("#selOrgFOWSTInsideMall").prop('checked', 0);//.slider("refresh");
                $("#chkOrgFOWSTOperates247").prop('checked', 0);//.slider("refresh");
                $("#selOrgFOWSTMallName").val('');
                $("#txtOrgFOWSTFtpServer").val('');
                $("#txtOrgFOWSTFtpUsername").val('');
                $("#txtOrgFOWSTFtpPassword").val('');

                $("#txtOrgFOWSTAyalaMallRecID").val('');
                $("#txtOrgFOWSTAyalaMallTenantCode").val('');
                $("#txtOrgFOWSTAyalaMallTenantName").val('');
                $("#txtOrgFOWSTAyalaMallTenantNameForDBF").val('');
                $("#txtOrgFOWSTAyalaMallMerchantsLeaseOutNumber").val('');

                $("#txtOrgFOWSTSMMallRecID").val('');
                $("#txtOrgFOWSTSMMallCode").val('');
                $("#txtOrgFOWSTSMMallTenantNo").val('');
                $("#txtOrgFOWSTSMMallClassCode").val('');
                $("#txtOrgFOWSTSMMallTradeCode").val('');
                $("#txtOrgFOWSTSMMallOutletNo").val('');
                $("#txtOrgFOWSTSMMallSalesType").val('');

                $("#txtOrgFOWSTMegaworldRecID").val('');
                $("#txtOrgFOWSTMegaworldTenantNo").val('');
                $("#txtOrgFOWSTMegaworldSalesType").val('');
                $("#txtOrgFOWSTMegaworldNoOfPOSTerminal").val('');

                $("#txtOrgFOWSTRobinsonTenantId").val('');
                $("#txtOrgFOWSTRobinsonPosTerminalNum").val('');

                $("#txtOrgFOWSTPITXTenantId").val('');
                $("#txtOrgFOWSTPITXPosTerminalID").val('');

                $("#selOrgFOWSTWarehouseType").val('');//.selectmenu('refresh');
                $("#selOrgFOWSTRetailStoreType").val('');//.selectmenu('refresh');

                $("#txtOrgFOWSTPlateNo").val('');
                $("#txtOrgFOWSTNoOfWheels").val('');
                $("#txtOrgFOWSTBodyColor").val('');
                $("#txtOrgFOWSTLoadLimitWeight").val('');
                $("#txtOrgFOWSTLoadLimitDimensions").val('');
                $('#radPosVendor1').prop('checked', true);
                $('#div3rdPartyPosVendor').hide();

                LoadOrgFOWSTLTOList();
                LoadOrgFOWSTLTFRBList();
                LoadOrgFOWSTSectionList();
                LoadOrgFOWSTBPermitList();
                LoadOrgFOWSTTelList();
                LoadOrgFOWSTMobileList();
                LoadOrgFOWSTEmailList();
                LoadOrgFOWSTOperationHRList();
            };

            function ClearOrgAddTruckFields() {

                $("#txtOrgFOWSTTRegID").val('');

                $("#txtOrgFOWSTTRegORNo").val('');

                $("#selOrgFOWSTTRegIssuedMonth").val('');//.selectmenu('refresh');
                $("#selOrgFOWSTTRegIssuedDay").val('');//.selectmenu('refresh');
                $("#selOrgFOWSTTRegIssuedYear").val('');//.selectmenu('refresh');

                $("#selOrgFOWSTTRegExpirationMonth").val('');//.selectmenu('refresh');
                $("#selOrgFOWSTTRegExpirationDay").val('');//.selectmenu('refresh');
                $("#selOrgFOWSTTRegExpirationYear").val('');//.selectmenu('refresh');

                $("#btnOrgFOWSTTRegRemove").hide();

            }

            function ClearOrgAddSectionFields() {

                $("#txtOrgFOWSTSecID").val('');

                $("#txtOrgFOWSTSecCode").val('');
                $("#txtOrgFOWSTSecName").val('');
                $("#txtOrgFOWSTSecDescription").val('');
                $("#txtOrgFOWSTSecFloorArea").val('');
                $("#txtOrgFOWSTSecSpecFeatures").val('');

                $("#btnOrgFOWSTSecRemove").hide();

            }

            function ClearOrgAddBPermitFields() {
                dateofpayment = "";
                dateissued = "";

                $('#btnDateOfPayment').text('Set Date');//.button();//.button('refresh');
                $('#btnDateIssued').text('Set Date');//.button();//.button('refresh');

                $("#txtOrgFOWSTBPermitRecID").val('');

                $("#txtOrgFOWSTBPermitNo").val('');
                $("#txtOrgFOWSTBPermitID").val('');
                $("#txtOrgFOWSTBPermitTradeName").val('');

                $("#txtOrgFOWSTBPermitBuildingNo").val('');
                $("#txtOrgFOWSTBPermitStreet").val('');

                $("#txtOrgFOWSTBPermitCityID").val('');
                $("#btnOrgFOWSTBPermitCity").text('Please Select');//.button('refresh');

                $("#txtOrgFOWSTBPermitCountryID").val('');
                $("#btnOrgFOWSTBPermitCountry").text('Please Select');//.button('refresh');

                $("#txtOrgFOWSTBPermitORNo").val('');

                $("#selOrgFOWSTBPermitDateOfPaymentMonth").val('');//.selectmenu('refresh');
                $("#selOrgFOWSTBPermitDateOfPaymentDay").val('');//.selectmenu('refresh');
                $("#selOrgFOWSTBPermitDateOfPaymentYear").val('');//.selectmenu('refresh');

                $("#txtOrgFOWSTBPermitBPlateNo").val('');

                $("#selOrgFOWSTBPermitBPlateIssuedMonth").val('');//.selectmenu('refresh');
                $("#selOrgFOWSTBPermitBPlateIssuedDay").val('');//.selectmenu('refresh');
                $("#selOrgFOWSTBPermitBPlateIssuedYear").val('');//.selectmenu('refresh');

                $("#btnOrgFOWSTBPermitRemove").hide();

            }

            function ClearOrgAddTelFields() {

                $("#txtOrgFOWSTTelID").val('');

                $("#txtOrgFOWSTTelCountryID").val('');
                $("#btnOrgFOWSTTelCountryCode").text('Country Code');//.button();//.button('refresh');

                $("#txtOrgFOWSTTelAreaCodeID").val('');
                $("#btnOrgFOWSTTelAreaCode").text('Area Code');//.button();//.button('refresh');

                $("#txtOrgFOWSTTelNo").val('');
                $("#txtOrgFOWSTTelExtension").val('');

                $("#selOrgFOWSTTelType").val('');//.selectmenu('refresh');

                $("#btnOrgFOWSTTelRemove").hide();

            }

            function ClearOrgAddMobileFields() {
                $("#txtOrgFOWSTMobileID").val('');

                $("#txtOrgFOWSTMobileCountryID").val('');
                $("#btnOrgFOWSTMobileCountryCode").text('Country Code');//.button('refresh');

                $("#txtOrgFOWSTTelCoPrefixID").val('');
                $("#btnOrgFOWSTMobileTelCoPrefixCode").text('Telco Code');//.button('refresh');

                $("#txtOrgFOWSTMobileNo").val('');
                $("#selOrgAddMobileType").val('');//.selectmenu('refresh');

                $("#btnOrgFOWSTMobileRemove").hide();

            }

            function ClearOrgAddEmailFields() {
                $("#txtOrgFOWSTEmailID").val('');

                $("#txtOrgFOWSTEmail").val('');
                $("#selOrgFOWSTEmailType").val('');//.selectmenu('refresh');

                $("#btnOrgFOWSTEmailRemove").hide();

            }

            function ClearOrgAddOperationHRFields() {
                $("#txtOrgFOWSTOptHRID").val('');

                $("#selOrgFOWSTOptHRDay").val('');

                $("#selOrgFOWSTOptOpenHR").val('');//.selectmenu('refresh');
                $("#selOrgFOWSTOptOpenMin").val('');//.selectmenu('refresh');
                $("#selOrgFOWSTOptOpenAMPM").val('');//.selectmenu('refresh');

                $("#selOrgFOWSTOptCloseHR").val('');//.selectmenu('refresh');
                $("#selOrgFOWSTOptCloseMin").val('');//.selectmenu('refresh');
                $("#selOrgFOWSTOptCloseAMPM").val('');//.selectmenu('refresh');

            }


            function ClearOrgAddPrinterFields() {
                $("#txtPrinterID").val('');

                $("#txtPrinterName").val('');
                $("#txtPrinterIPAddress").val('');

                $("#btnOrgFOWSTPrinterRemove").hide();

            }

            function SaveOrgFOWST() {
                var pData = [];
                var pDataLTO = [];
                var pDataLTFRB = [];
                var pDataSection = [];
                var pDataBPermit = [];
                var pDataTel = [];
                var pDataMobile = [];
                var pDataEmail = [];
                var pDataOptHR = [];
                var pDataPrinter = [];
                var intIndex;

                //LTO
                intIndex = 0;
                for (var i = 0; i <= intOrgFowstLTOCntr; i++) {

                    if (sessionStorage["OrgAddLTO-" + i] != undefined) {

                        arOrgAddLTO = new Array();
                        arOrgAddLTO = JSON.parse(sessionStorage["OrgAddLTO-" + i]);

                        var dtDateIssued = "";
                        if (arOrgAddLTO[intOrgFowstLTODateIssuedMonth] != "" &&
                            arOrgAddLTO[intOrgFowstLTODateIssuedDay] != "" &&
                            arOrgAddLTO[intOrgFowstLTODateIssuedYear] != "") {

                            dtDateIssued = arOrgAddLTO[intOrgFowstLTODateIssuedMonth] + "/" + arOrgAddLTO[intOrgFowstLTODateIssuedDay] + "/" + arOrgAddLTO[intOrgFowstLTODateIssuedYear];
                        }
                        var dtExpirationDate = "";
                        if (arOrgAddLTO[intOrgFowstLTODateExpirationMonth] != "" &&
                            arOrgAddLTO[intOrgFowstLTODateExpirationDay] != "" &&
                            arOrgAddLTO[intOrgFowstLTODateExpirationYear] != "") {

                            dtExpirationDate = arOrgAddLTO[intOrgFowstLTODateExpirationMonth] + "/" + arOrgAddLTO[intOrgFowstLTODateExpirationDay] + "/" + arOrgAddLTO[intOrgFowstLTODateExpirationYear];
                        }

                        pDataLTO[intIndex] = {
                            ID: arOrgAddLTO[intOrgFowstLTORecID],
                            ORNo: arOrgAddLTO[intOrgFowstLTOORNo],
                            DateIssued: dtDatePayment,
                            ExpirationDate: dtExpirationDate,
                            RecState: arOrgAddLTO[intOrgFowstLTOState]
                        };

                    }
                }

                //LTFRB
                intIndex = 0;
                for (var i = 0; i <= intOrgFowstLTFRBCntr; i++) {

                    if (sessionStorage["OrgAddLTFRB-" + i] != undefined) {

                        arOrgAddLTFRB = new Array();
                        arOrgAddLTFRB = JSON.parse(sessionStorage["OrgAddLTFRB-" + i]);

                        var dtDateIssued = "";
                        if (arOrgAddLTFRB[intOrgFowstLTFRBDateIssuedMonth] != "" &&
                            arOrgAddLTFRB[intOrgFowstLTFRBDateIssuedDay] != "" &&
                            arOrgAddLTFRB[intOrgFowstLTFRBDateIssuedYear] != "") {

                            dtDateIssued = arOrgAddLTFRB[intOrgFowstLTFRBDateIssuedMonth] + "/" + arOrgAddLTFRB[intOrgFowstLTFRBDateIssuedDay] + "/" + arOrgAddLTFRB[intOrgFowstLTFRBDateIssuedYear];
                        }
                        var dtExpirationDate = "";
                        if (arOrgAddLTFRB[intOrgFowstLTFRBDateExpirationMonth] != "" &&
                            arOrgAddLTFRB[intOrgFowstLTFRBDateExpirationDay] != "" &&
                            arOrgAddLTFRB[intOrgFowstLTFRBDateExpirationYear] != "") {

                            dtExpirationDate = arOrgAddLTFRB[intOrgFowstLTFRBDateExpirationMonth] + "/" + arOrgAddLTFRB[intOrgFowstLTFRBDateExpirationDay] + "/" + arOrgAddLTFRB[intOrgFowstLTFRBDateExpirationYear];
                        }

                        pDataLTFRB[intIndex] = {
                            ID: arOrgAddLTFRB[intOrgFowstLTFRBRecID],
                            ORNo: arOrgAddLTFRB[intOrgFowstLTFRBORNo],
                            DateIssued: dtDatePayment,
                            ExpirationDate: dtExpirationDate,
                            RecState: arOrgAddLTFRB[intOrgFowstLTFRBState]
                        };

                    }
                }

                //Section
                intIndex = 0;
                for (var i = 0; i <= intOrgFowstSectionCntr; i++) {

                    if (sessionStorage["OrgAddSection-" + i] != undefined) {

                        arOrgAddSection = new Array();
                        arOrgAddSection = JSON.parse(sessionStorage["OrgAddSection-" + i]);

                        pDataSection[intIndex] = {
                            ID: arOrgAddSection[intOrgFowstSecRecID],
                            Code: arOrgAddSection[intOrgFowstSecCode],
                            Name: arOrgAddSection[intOrgFowstSecName],
                            Description: arOrgAddSection[intOrgFowstSecDescription],
                            FloorArea: arOrgAddSection[intOrgFowstSecFloorArea],
                            SpecialFeatures: arOrgAddSection[intOrgFowstSecSpecialFeatures],
                            RecState: arOrgAddSection[intOrgFowstSecState]
                        };

                        intIndex = intIndex + 1;

                    }
                }

                //BPermit
                intIndex = 0;
                for (var i = 0; i <= intOrgFowstBPermitCntr; i++) {

                    if (sessionStorage["OrgAddBPermit-" + i] != undefined) {

                        arOrgAddBPermit = new Array();
                        arOrgAddBPermit = JSON.parse(sessionStorage["OrgAddBPermit-" + i]);

                        var dtDatePayment = "";
                        if (arOrgAddBPermit[intOrgFowstBPermitDatePaymentMonth] != "" &&
                            arOrgAddBPermit[intOrgFowstBPermitDatePaymentDay] != "" &&
                            arOrgAddBPermit[intOrgFowstBPermitDatePaymentYear] != "") {

                            dtDatePayment = arOrgAddBPermit[intOrgFowstBPermitDatePaymentMonth] + "/" + arOrgAddBPermit[intOrgFowstBPermitDatePaymentDay] + "/" + arOrgAddBPermit[intOrgFowstBPermitDatePaymentYear];
                        }
                        var dtDateIssued = "";
                        if (arOrgAddBPermit[intOrgFowstBPermitDateIssuedMonth] != "" &&
                            arOrgAddBPermit[intOrgFowstBPermitDateIssuedDay] != "" &&
                            arOrgAddBPermit[intOrgFowstBPermitDateIssuedYear] != "") {

                            dtDateIssued = arOrgAddBPermit[intOrgFowstBPermitDateIssuedMonth] + "/" + arOrgAddBPermit[intOrgFowstBPermitDateIssuedDay] + "/" + arOrgAddBPermit[intOrgFowstBPermitDateIssuedYear];
                        }

                        pDataBPermit[intIndex] = {
                            ID: arOrgAddBPermit[intOrgFowstBPermitRecID],
                            PermitNo: arOrgAddBPermit[intOrgFowstBPermitNo],
                            BusinessID: arOrgAddBPermit[intOrgFowstBPermitBusinessID],
                            TradeName: arOrgAddBPermit[intOrgFowstBPermitTradeName],
                            BuildingHouseNo: arOrgAddBPermit[intOrgFowstBPermitBuildingHouseNo],
                            Street: arOrgAddBPermit[intOrgFowstBPermitStreet],
                            AddressCityID: arOrgAddBPermit[intOrgFowstBPermitCityID],
                            AddressCountryID: arOrgAddBPermit[intOrgFowstBPermitCountryID],
                            ORNo: arOrgAddBPermit[intOrgFowstBPermitORNo],
                            BusinessPlateNo: arOrgAddBPermit[intOrgFowstBPermitBusinessPlateNo],
                            DatePayment: dtDatePayment,
                            DateIssued: dtDateIssued,
                            RecState: arOrgAddBPermit[intOrgFowstBPermitState]
                        };

                        intIndex = intIndex + 1;

                    }
                }

                //Tel
                intIndex = 0;
                for (var i = 0; i <= intOrgFowstTelCntr; i++) {

                    if (sessionStorage["OrgAddTel-" + i] != undefined) {

                        arOrgAddTel = new Array();
                        arOrgAddTel = JSON.parse(sessionStorage["OrgAddTel-" + i]);

                        pDataTel[intIndex] = {
                            ID: arOrgAddTel[intOrgFowstTelRecID],
                            CountryID: arOrgAddTel[intOrgFowstTelCountryID],
                            AreaCodeID: arOrgAddTel[intOrgFowstTelAreaCodeID],
                            ContactNo: arOrgAddTel[intOrgFowstTelContactNo],
                            ExtensionNo: arOrgAddTel[intOrgFowstTelExtNo],
                            ContactType: arOrgAddTel[intOrgFowstTelContactType],
                            RecState: arOrgAddTel[intOrgFowstTelState]
                        };

                        intIndex = intIndex + 1;

                    }
                }

                //Mobile
                intIndex = 0;
                for (var i = 0; i <= intOrgFowstMobileCntr; i++) {

                    if (sessionStorage["OrgAddMobile-" + i] != undefined) {

                        arOrgAddMobile = new Array();
                        arOrgAddMobile = JSON.parse(sessionStorage["OrgAddMobile-" + i]);

                        pDataMobile[intIndex] = {
                            ID: arOrgAddMobile[intOrgFowstMobileRecID],
                            CountryID: arOrgAddMobile[intOrgFowstMobileCountryID],
                            TelCoPrefixID: arOrgAddMobile[intOrgFowstMobileTelCoPrefixID],
                            ContactNo: arOrgAddMobile[intOrgFowstMobileContactNo],
                            ContactType: arOrgAddMobile[intOrgFowstMobileContactType],
                            RecState: arOrgAddMobile[intOrgFowstMobileState]
                        };

                        intIndex = intIndex + 1;

                    }
                }

                //Email
                intIndex = 0;
                for (var i = 0; i <= intOrgFowstEmailCntr; i++) {

                    if (sessionStorage["OrgAddEmail-" + i] != undefined) {

                        arOrgAddEmail = new Array();
                        arOrgAddEmail = JSON.parse(sessionStorage["OrgAddEmail-" + i]);

                        pDataEmail[intIndex] = {
                            ID: arOrgAddEmail[intOrgFowstEmailRecID],
                            EmailAddress: arOrgAddEmail[intOrgFowstEmailAddress],
                            EmailType: arOrgAddEmail[intOrgFowstEmailType],
                            RecState: arOrgAddEmail[intOrgFowstEmailState]
                        };

                        intIndex = intIndex + 1;

                    }
                }

                //Operation HR
                intIndex = 0;
                for (var i = 0; i <= intOrgFowstOptHRCntr; i++) {

                    if (sessionStorage["OrgAddOptHR-" + i] != undefined) {

                        arOrgAddOptHR = new Array();
                        arOrgAddOptHR = JSON.parse(sessionStorage["OrgAddOptHR-" + i]);

                        //check muna if null para dli mg error sa server.

                        if (arOrgAddOptHR[intOrgFowstOptHROpeningTimeHR] != null) {
                            pDataOptHR[intIndex] = {
                                ID: arOrgAddOptHR[intOrgFowstOptHRRecID],
                                WeekDay: arOrgAddOptHR[intOrgFowstOptHRWeekDay],
                                OpeningTimeHR: arOrgAddOptHR[intOrgFowstOptHROpeningTimeHR],
                                OpeningTimeMin: arOrgAddOptHR[intOrgFowstOptHROpeningTimeMin],
                                OpeningTimeAMPM: arOrgAddOptHR[intOrgFowstOptHROpeningTimeAMPM],
                                ClosingTimeHR: arOrgAddOptHR[intOrgFowstOptHRClosingTimeHR],
                                ClosingTimeMin: arOrgAddOptHR[intOrgFowstOptHRClosingTimeMin],
                                ClosingTimeAMPM: arOrgAddOptHR[intOrgFowstOptHRClosingTimeAMPM],
                                IsClosed: arOrgAddOptHR[intOrgFowstOptHRIsClosed]
                            };
                            intIndex = intIndex + 1;
                        }
                    }
                }


                //Printer
                intIndex = 0;
                for (var i = 0; i <= intOrgFowstPrinterCntr; i++) {

                    if (sessionStorage["OrgAddPrinter-" + i] != undefined) {

                        arOrgAddPrinter = new Array();
                        arOrgAddPrinter = JSON.parse(sessionStorage["OrgAddPrinter-" + i]);

                        if (arOrgAddPrinter[intOrgFowstPrinterState] != "Remove") {

                            pDataPrinter[intIndex] = {
                                ID: arOrgAddPrinter[intOrgFowstPrinterID],
                                PrinterID: arOrgAddPrinter[intOrgFowstPrinterRoleID],
                                PrinterName: arOrgAddPrinter[intOrgFowstPrinterName],
                                PrinterIPAddress: arOrgAddPrinter[intOrgFowstPrinterIPAddress]
                            };
                            intIndex = intIndex + 1;
                        }



                    }
                }

                var dtContractStart = "";
                if ($("#selOrgFOWSTContractStartMonth").val() != "" &&
                    $("#selOrgFOWSTContractStartDay").val() != "" &&
                    $("#selOrgFOWSTContractStartYear").val() != "") {

                    dtContractStart = $("#selOrgFOWSTContractStartMonth").val() + "/" + $("#selOrgFOWSTContractStartDay").val() + "/" + $("#selOrgFOWSTContractStartYear").val();
                }
                var dtContractEnd = "";
                if ($("#selOrgFOWSTContractEndMonth").val() != "" &&
                    $("#selOrgFOWSTContractEndDay").val() != "" &&
                    $("#selOrgFOWSTContractEndYear").val() != "") {

                    dtContractEnd = $("#selOrgFOWSTContractEndMonth").val() + "/" + $("#selOrgFOWSTContractEndDay").val() + "/" + $("#selOrgFOWSTContractEndYear").val();
                }

                var strFOWSTSubTypeID = "";
                if ($("#selOrgFOWSTType").val().toUpperCase() == '6AF9BC83-426C-4774-A051-044FC09C552B') {
                    //Branch
                    strFOWSTSubTypeID = $("#selOrgFOWSTRetailStoreType").val();
                }
                else if ($("#selOrgFOWSTType").val().toUpperCase() == '247C961C-A219-45DF-BDA3-3C137B734ABF') {
                    //Warehouse
                    strFOWSTSubTypeID = $("#selOrgFOWSTWarehouseType").val();
                }


                pData = {
                    OrgFOWSTID: $("#txtOrgFOWSTID").val(),
                    FOWSTTypeID: $("#selOrgFOWSTType").val(),
                    FOWSTStatus: $("#selOrgFOWSTStatus").val(),
                    FOWSTClassification: $("#selOrgFOWSTClassification").val(),
                    FOWSTCode: $.trim($("#txtOrgFOWSTCode").val()),
                    FOWSTName: $.trim($("#txtOrgFOWSTName").val()),
                    FOWSTDescription: $.trim($("#txtOrgFOWSTDescription").val()),
                    FOWSTBusinessTradeNameID: $.trim($("#selOrgFOWSTTradeName").val()),
                    FOWSTReceiptSettingsID: $.trim($("#selOrgFOWSTReceiptSettings").val()),
                    FOWSTTINBranchCode: $.trim($("#txtTINBranchCode").val()),
                    IsVAT: CInt2($("#rdoFowstVAT").is(':checked')),
                    DefaultPosView: $.trim($("#selDefaultPosView").val()),
                    IsActive: CInt2($("#selOrgActiveFOWST").is(':checked')),
                    DefaultHotButtonTemplateID: $("#selDefaultHBTemplate").val(),

                    FOWSTStreetNumber: $.trim($("#txtOrgFOWSTBuildingNo").val()),
                    FOWSTStreet: $("#txtOrgFOWSTStreet").val(),
                    FOWSTCityID: $("#txtOrgFOWSTCityID").val(),
                    FOWSTCountryID: $("#txtOrgFOWSTCountryID").val(),
                    FOWSTRegion: $("#selOrgFOWSTRegion").val(),

                    FOWSTIsFranchisee: CInt2($("#selOrgAddFranchisee").is(':checked')),
                    FOWSTFranchisorID: $("#txtOrgFOWSTFranchisorID").val(),

                    FOWSTOwnershipType: $("#selOrgFOWSTOwnershipType").val(),
                    FOWSTContractStart: dtContractStart,
                    FOWSTContractEnd: dtContractEnd,
                    FOWSTCostPerMonth: $("#txtOrgFOWSTCostPerMonth").val(),

                    FOWSTIsInsideAMall: CInt2($("#selOrgFOWSTInsideMall").is(':checked')),
                    Operates247: CInt2($("#chkOrgFOWSTOperates247").is(':checked')),
                    PrintStickers: CInt2($("#selEnablePrintStickers").is(':checked')),
                    PrintQueueNumbers: CInt2($("#selPrintQueueNumbers").is(':checked')),
                    FOWSTMallName: $("#selOrgFOWSTMallName").val(),
                    FtpServer: $("#txtOrgFOWSTFtpServer").val(),
                    FtpUsername: $("#txtOrgFOWSTFtpUsername").val(),
                    FtpPassword: $("#txtOrgFOWSTFtpPassword").val(),

                    FOWSTAyalaRecID: $("#txtOrgFOWSTAyalaMallRecID").val(),
                    FOWSTAyalaTenantCode: $("#txtOrgFOWSTAyalaMallTenantCode").val(),
                    FOWSTAyalaTenantName: $("#txtOrgFOWSTAyalaMallTenantName").val(),
                    FOWSTAyalaTenantNameForDBF: $("#txtOrgFOWSTAyalaMallTenantNameForDBF").val(),

                    FOWSTAyalaMerchantsLeaseOutNumber: $("#txtOrgFOWSTAyalaMallMerchantsLeaseOutNumber").val(),
                    FOWSTSMRecID: $("#txtOrgFOWSTSMMallRecID").val(),
                    FOWSTSMMallCode: $("#txtOrgFOWSTSMMallCode").val(),
                    FOWSTSMMallTenantNo: $("#txtOrgFOWSTSMMallTenantNo").val(),
                    FOWSTSMMallClassCode: $("#txtOrgFOWSTSMMallClassCode").val(),
                    FOWSTSMMallTradeCode: $("#txtOrgFOWSTSMMallTradeCode").val(),
                    FOWSTSMMallOutletNo: $("#txtOrgFOWSTSMMallOutletNo").val(),
                    FOWSTSMMallSaleType: $("#txtOrgFOWSTSMMallSalesType").val(),

                    FOWSTMegaworldRecID: $("#txtOrgFOWSTMegaworldRecID").val(),
                    FOWSTMegaworldTenantCode: $("#txtOrgFOWSTMegaworldTenantNo").val(),
                    FOWSTMegaworldSaleType: $("#txtOrgFOWSTMegaworldSalesType").val(),
                    FOWSTMegaworldNoOfPOSTerminal: $("#txtOrgFOWSTMegaworldNoOfPOSTerminal").val(),

                    RobinsonTenantId: $("#txtOrgFOWSTRobinsonTenantId").val(),
                    RobinsonPosTerminalNum: $("#txtOrgFOWSTRobinsonPosTerminalNum").val(),

                    PITXTenantId: $("#txtOrgFOWSTPITXTenantId").val(),
                    PITXPosTerminalID: $("#txtOrgFOWSTPITXPosTerminalID").val(),

                    FOWSTIsMortgaged: CInt2($("#selOrgFOWSTIsMortgaged").is(':checked')),
                    EnforceZRead: CInt2($("#selEnforceZRead").is(':checked')),

                    FOWSTSubTypeID: strFOWSTSubTypeID,
                    StoreLocationType: $("#selOrgFOWSTRetailStoreLocationType").val(),

                    FOWSTPlateNo: $("#txtOrgFOWSTPlateNo").val(),
                    FOWSTNoOfWheels: $("#txtOrgFOWSTNoOfWheels").val(),
                    FOWSTBodyColor: $("#txtOrgFOWSTBodyColor").val(),
                    FOWSTWeight: $("#txtOrgFOWSTLoadLimitWeight").val(),
                    FOWSTDimension: $("#txtOrgFOWSTLoadLimitDimensions").val(),

                    FOWSTIsIncludeToReport: CInt2($("#selOrgFOWSTIsInclude").is(':checked')),
                    FOWSTAccountID: $('#txtAccountID').val(),
                    FOWSTStoreID: $('#txtStoreID').val(),
                    FOWSTAccountID2: $('#txtAccountID2').val(),
                    FOWSTStoreID2: $('#txtStoreID2').val(),

                    FOWSTIsIncludeToShopperTrakReport: CInt2($("#selOrgFOWSTIsIncludeToShopperTrak").is(':checked')),
                    FOWSTShopperTrakAccountID: $('#txtShopperTrakAccountID').val(),

                    OrgFOWSTLTOList: pDataLTO,
                    OrgFOWSTLTFRBList: pDataLTFRB,
                    OrgFOWSTSectionList: arrFowstSection, //pDataSection,
                    OrgFOWSTBPermitList: arrFowstBusinessPermit, //pDataBPermit,
                    OrgFOWSTTelList: arrFowstTel, //pDataTel,
                    OrgFOWSTMobileList: arrFowstMobile, //pDataMobile,
                    OrgFOWSTEmailList: arrFowstEmail, // pDataEmail,
                    OrgFOWSTOperationHRList: pDataOptHR,
                    OrgFOWSTPrinterList: pDataPrinter,

                    JanQuota: $('#txtOrgTeamQuotaTier1Jan').val(),
                    JanIncentive: $('#txtOrgTeamIncentiveTier1Jan').val(),
                    FebQuota: $('#txtOrgTeamQuotaTier1Feb').val(),
                    FebIncentive: $('#txtOrgTeamIncentiveTier1Feb').val(),
                    MarQuota: $('#txtOrgTeamQuotaTier1Mar').val(),
                    MarIncentive: $('#txtOrgTeamIncentiveTier1Mar').val(),
                    AprQuota: $('#txtOrgTeamQuotaTier1Apr').val(),
                    AprIncentive: $('#txtOrgTeamIncentiveTier1Apr').val(),
                    MayQuota: $('#txtOrgTeamQuotaTier1May').val(),
                    MayIncentive: $('#txtOrgTeamIncentiveTier1May').val(),
                    JunQuota: $('#txtOrgTeamQuotaTier1Jun').val(),
                    JunIncentive: $('#txtOrgTeamIncentiveTier1Jun').val(),
                    JulQuota: $('#txtOrgTeamQuotaTier1Jul').val(),
                    JulIncentive: $('#txtOrgTeamIncentiveTier1Jul').val(),
                    AugQuota: $('#txtOrgTeamQuotaTier1Aug').val(),
                    AugIncentive: $('#txtOrgTeamIncentiveTier1Aug').val(),
                    SepQuota: $('#txtOrgTeamQuotaTier1Sept').val(),
                    SepIncentive: $('#txtOrgTeamIncentiveTier1Sept').val(),
                    OctQuota: $('#txtOrgTeamQuotaTier1Oct').val(),
                    OctIncentive: $('#txtOrgTeamIncentiveTier1Oct').val(),
                    NovQuota: $('#txtOrgTeamQuotaTier1Nov').val(),
                    NovIncentive: $('#txtOrgTeamIncentiveTier1Nov').val(),
                    DecQuota: $('#txtOrgTeamQuotaTier1Dec').val(),
                    DecIncentive: $('#txtOrgTeamIncentiveTier1Dec').val(),

                    EnableHotButtonsContextualMenu: CInt2($('#selEnableHotButtonsContextualMenu').is(':checked')),
                    PriceLevels: arrFowstPriceLevels,
                    RemoveAssignedEmp: arrRemoveAssignedEmp
                };

                if ($('[name=radPosVendor]:checked').val())
                    pData.PosVendorID = mPosVendorID;

                AjaxSend({
                    url: 'Organization/CheckIfAutoApproveFranchisorOrFranchiseeRequest',
                    data: { FranchisorID: $("#txtOrgFOWSTFranchisorID").val() },
                    success: function (e) {
                        pData.IsAutoApproveFranchisorOrFranchiseeRequest = CBool(e.Result);

                        AjaxSend({
                            url: "Organization/SaveOrgFOWST",
                            data: pData,
                            success: function (vData) {

                                if (vData.OrganizationFOWSTData == "Invalid FOWSTBegBalDate") {
                                    NewMessageBox('Please set valid FOWST beginning balance date.', 'Beginning Balance Date');
                                } else if (vData.OrganizationFOWSTData == "Invalid FOWSTBegBal") {
                                    NewMessageBox('Please set valid FOWST beginning balance.', 'Beginning Balance');
                                } else if (vData.OrganizationFOWSTData == "Invalid FOWSTInvStartNo") {
                                    NewMessageBox('Please set valid FOWST Invoice Starting number.', 'Invoice Starting Number');
                                }
                                else {
                                    if (GetUrlParameters().mode == 'wizard') {

                                        if (isPrinterListEmpty) {
                                            NewMessageBox('Oops! You haven\'t added a printer yet. Please scroll to the bottom of the page to add a printer.', 'Device Printer Setup');
                                            return;
                                        } else {
                                            NewMessageBox('Next step: Add a new device or assign a POS printer to an existing device.', 'Device Printer Setup',
                                                [
                                                    {
                                                        Text: 'Ok',
                                                        Click: function () {
                                                            AjaxSend({
                                                                url: 'Login3/TestCookies',
                                                                success: function (vData) {
                                                                    window.pageSecuritySourcePage = $.mobile.activePage;
                                                                    PromptUserAccess('Devices', {
                                                                        OnClickOKSuccess: function () {
                                                                            //window.open(GetPathFormat("/Device") + "?OrgID=" + sessionStorage["OrgID"], "_self");
                                                                            //location = MergeUrlParameters({
                                                                            //    mode: 'wizard',
                                                                            //    OrgID: sessionStorage.OrgID,
                                                                            //    IsMobile: CBool(IsMobile) ? 1 : 0
                                                                            //}, '/Device');
                                                                            window.location.hash = 'LibraryPage';
                                                                            sessionStorage["LibraryName"] = "Authorized Devices";
                                                                            $('#tblHeader').empty();
                                                                            var HeaderData = ['#', 'Phone Name', 'Phone Model', 'IMEI', 'UUID', 'OS', 'Device Number'];
                                                                            for (var i = 0; i < HeaderData.length; i++) {
                                                                                $('#tblHeader').append('<th>' + HeaderData[i] + '</th>');
                                                                            }
                                                                            LibraryTable.Devices();
                                                                        }
                                                                    });
                                                                },
                                                                error: function (vError) {
                                                                    NewMessageBox('This feature requires internet connection.', 'Requires Internet Connection')
                                                                    return;
                                                                }

                                                            });
                                                        }
                                                    }
                                                ])
                                        } //end if
                                    } else {
                                        //                            ChangePage('/Organization/OrganizationAddress');
                                        //window.location = MergeUrlParameters({
                                        //    OrgID: sessionStorage.OrgID,
                                        //    IsMobile: CBool(IsMobile) ? 1 : 0
                                        //}, '/Organization/OrganizationAddress');
                                        //getOrgFOWSTList();
                                        if (sessionStorage.AddViewLibBCName == "View Device") {
                                            AjaxSend({
                                                ExactUrl: '/Device/DeviceFowst',
                                                type: 'GET',
                                                dataType: 'text',
                                                success: function (vData) {
                                                    isDeviceFowstPageLoaded = vData;
                                                    $('#UniversalDiv').html('');
                                                    $('#UniversalDiv').append(vData).trigger('create');
                                                    DeviceFowst.DeviceFowstList();
                                                    $('#pageDeviceFowst').hide();
                                                    $('#pageDeviceFowstInfo').show();
                                                }
                                            });
                                        }
                                        else {
                                            $('#tblBody').html('');
                                            window.location.hash = '';
                                            window.location.hash = 'LibraryPage';
                                            LibraryTable.Fowst();
                                        }

                                    } //end if mode
                                }
                            }
                        });
                    }
                })                
            };

            function SaveOrgFOWSTLTO() {

                arOrgAddLTO = new Array();
                arOrgAddLTO[intOrgFowstLTORecID] = $("#txtOrgFOWSTTRegID").val();
                arOrgAddLTO[intOrgFowstLTOORNo] = $("#txtOrgFOWSTTRegORNo").val();
                arOrgAddLTO[intOrgFowstLTODateIssuedMonth] = $("#selOrgFOWSTTRegIssuedMonth").val();
                arOrgAddLTO[intOrgFowstLTODateIssuedDay] = $("#selOrgFOWSTTRegIssuedDay").val();
                arOrgAddLTO[intOrgFowstLTODateIssuedYear] = $("#selOrgFOWSTTRegIssuedYear").val();
                arOrgAddLTO[intOrgFowstLTODateExpirationMonth] = $("#selOrgFOWSTTRegExpirationMonth").val();
                arOrgAddLTO[intOrgFowstLTODateExpirationDay] = $("#selOrgFOWSTTRegExpirationDay").val();
                arOrgAddLTO[intOrgFowstLTODateExpirationYear] = $("#selOrgFOWSTTRegExpirationYear").val();
                arOrgAddLTO[intOrgFowstLTOState] = (IsNewOrgAddLTO ? "New" : "Edit");

                sessionStorage["OrgAddLTO-" + intOrgFowstLTOIndex] = JSON.stringify(arOrgAddLTO);

                //$.mobile.changePage('#pageOrgFOWSTInfo');
                $('#pageOrgFOWSTInfo').show();
                $('#pageOrgFOWSTTruckRegInfo').hide();
                $('#pageOrgFOWSTSectionInfo').hide();
                $('#pageOrgFOWSTBusinessPermitInfo').hide();
                $('#pageOrgFOWSTTelInfo').hide();
                $('#pageOrgFOWSTMobileInfo').hide();
                $('#pageOrgFOWSTEmailInfo').hide();
                $('#pageOrgFOWSTOperationHRInfo').hide();
                $('#pageOrgFOWSTPrinterInfo').hide();
                $('#pageOrgFOWSTPrinterIP').hide(); $('#pageAddDevicePrinter').hide();

                LoadOrgFOWSTLTOList();
            };

            function SaveOrgFOWSTLTFRB() {

                arOrgAddLTFRB = new Array();
                arOrgAddLTFRB[intOrgFowstLTFRBRecID] = $("#txtOrgFOWSTTRegID").val();
                arOrgAddLTFRB[intOrgFowstLTFRBORNo] = $("#txtOrgFOWSTTRegORNo").val();
                arOrgAddLTFRB[intOrgFowstLTFRBDateIssuedMonth] = $("#selOrgFOWSTTRegIssuedMonth").val();
                arOrgAddLTFRB[intOrgFowstLTFRBDateIssuedDay] = $("#selOrgFOWSTTRegIssuedDay").val();
                arOrgAddLTFRB[intOrgFowstLTFRBDateIssuedYear] = $("#selOrgFOWSTTRegIssuedYear").val();
                arOrgAddLTFRB[intOrgFowstLTFRBDateExpirationMonth] = $("#selOrgFOWSTTRegExpirationMonth").val();
                arOrgAddLTFRB[intOrgFowstLTFRBDateExpirationDay] = $("#selOrgFOWSTTRegExpirationDay").val();
                arOrgAddLTFRB[intOrgFowstLTFRBDateExpirationYear] = $("#selOrgFOWSTTRegExpirationYear").val();
                arOrgAddLTFRB[intOrgFowstLTFRBState] = (IsNewOrgAddLTFRB ? "New" : "Edit");

                sessionStorage["OrgAddLTFRB-" + intOrgFowstLTFRBIndex] = JSON.stringify(arOrgAddLTFRB);

                //$.mobile.changePage('#pageOrgFOWSTInfo');
                $('#pageOrgFOWSTInfo').show();
                $('#pageOrgFOWSTTruckRegInfo').hide();
                $('#pageOrgFOWSTSectionInfo').hide();
                $('#pageOrgFOWSTBusinessPermitInfo').hide();
                $('#pageOrgFOWSTTelInfo').hide();
                $('#pageOrgFOWSTMobileInfo').hide();
                $('#pageOrgFOWSTEmailInfo').hide();
                $('#pageOrgFOWSTOperationHRInfo').hide();
                $('#pageOrgFOWSTPrinterInfo').hide();
                $('#pageOrgFOWSTPrinterIP').hide(); $('#pageAddDevicePrinter').hide();
                LoadOrgFOWSTLTFRBList();
            };

            function SaveOrgFOWSTSection() {
                var pData = {
                    ID: $.trim($("#txtOrgFOWSTSecID").val()),
                    Code: $.trim($("#txtOrgFOWSTSecCode").val()),
                    Name: $.trim($("#txtOrgFOWSTSecName").val()),
                    Description: $.trim($("#txtOrgFOWSTSecDescription").val()),
                    FloorArea: $.trim($("#txtOrgFOWSTSecFloorArea").val()),
                    SpecialFeatures: $.trim($("#txtOrgFOWSTSecSpecFeatures").val()),
                    RecState: (IsNewOrgAddSection ? "New" : "Edit")
                }

                if (IsNewOrgAddSection) {
                    arrFowstSection.push(pData);
                } else {
                    arrFowstSection[intOrgFowstSectionIndex] = pData;
                }

                //arOrgAddSection = new Array();
                //arOrgAddSection[intOrgFowstSecRecID] = $("#txtOrgFOWSTSecID").val();
                //arOrgAddSection[intOrgFowstSecCode] = $("#txtOrgFOWSTSecCode").val();
                //arOrgAddSection[intOrgFowstSecName] = $("#txtOrgFOWSTSecName").val();
                //arOrgAddSection[intOrgFowstSecDescription] = $("#txtOrgFOWSTSecDescription").val();
                //arOrgAddSection[intOrgFowstSecFloorArea] = $("#txtOrgFOWSTSecFloorArea").val();
                //arOrgAddSection[intOrgFowstSecSpecialFeatures] = $("#txtOrgFOWSTSecSpecFeatures").val();
                //arOrgAddSection[intOrgFowstSecState] = (IsNewOrgAddSection ? "New" : "Edit");

                //sessionStorage["OrgAddSection-" + intOrgFowstSectionIndex] = JSON.stringify(arOrgAddSection);

                //$.mobile.changePage('#pageOrgFOWSTInfo');
                $('#pageOrgFOWSTInfo').show();
                $('#pageOrgFOWSTTruckRegInfo').hide();
                $('#pageOrgFOWSTSectionInfo').hide();
                $('#pageOrgFOWSTBusinessPermitInfo').hide();
                $('#pageOrgFOWSTTelInfo').hide();
                $('#pageOrgFOWSTMobileInfo').hide();
                $('#pageOrgFOWSTEmailInfo').hide();
                $('#pageOrgFOWSTOperationHRInfo').hide();
                $('#pageOrgFOWSTPrinterInfo').hide();
                $('#pageOrgFOWSTPrinterIP').hide(); $('#pageAddDevicePrinter').hide();
                LoadOrgFOWSTSectionList();
            };

            function SaveOrgFOWSTBPermit() {
                var pData = {
                    ID: $.trim($("#txtOrgFOWSTBPermitRecID").val()),
                    BusinessPermitNo: $.trim($("#txtOrgFOWSTBPermitNo").val()),
                    BusinessID: $.trim($("#txtOrgFOWSTBPermitID").val()),
                    TradeName: $.trim($("#txtOrgFOWSTBPermitTradeName").val()),
                    BuildingHouseNo: $.trim($("#txtOrgFOWSTBPermitBuildingNo").val()),
                    Street: $.trim($("#txtOrgFOWSTBPermitStreet").val()),
                    AddressCityID: $.trim($("#txtOrgFOWSTBPermitCityID").val()),
                    AddressCity: $.trim($("#btnOrgFOWSTBPermitCity").val()),
                    AddressCountryID: $.trim($("#txtOrgFOWSTBPermitCountryID").val()),
                    AddressCountry: $.trim($("#btnOrgFOWSTBPermitCountry").val()),
                    ORNo: $.trim($("#txtOrgFOWSTBPermitORNo").val()),
                    BusinessPlateNo: $.trim($("#txtOrgFOWSTBPermitBPlateNo").val()),
                    DatePayment: dateofpayment,
                    DateIssued: dateissued,
                    RecState: (IsNewOrgAddBPermit ? "New" : "Edit")
                }

                if (IsNewOrgAddBPermit) {
                    arrFowstBusinessPermit.push(pData);
                } else {
                    arrFowstBusinessPermit[intOrgFowstBPermitIndex] = pData;
                }

                //arOrgAddBPermit = new Array();
                //arOrgAddBPermit[intOrgFowstBPermitRecID] = $("#txtOrgFOWSTBPermitRecID").val();
                //arOrgAddBPermit[intOrgFowstBPermitNo] = $("#txtOrgFOWSTBPermitNo").val();
                //arOrgAddBPermit[intOrgFowstBPermitBusinessID] = $("#txtOrgFOWSTBPermitID").val();
                //arOrgAddBPermit[intOrgFowstBPermitTradeName] = $("#txtOrgFOWSTBPermitTradeName").val();
                //arOrgAddBPermit[intOrgFowstBPermitBuildingHouseNo] = $("#txtOrgFOWSTBPermitBuildingNo").val();
                //arOrgAddBPermit[intOrgFowstBPermitStreet] = $("#txtOrgFOWSTBPermitStreet").val();
                //arOrgAddBPermit[intOrgFowstBPermitCityID] = $("#txtOrgFOWSTBPermitCityID").val();
                //arOrgAddBPermit[intOrgFowstBPermitCity] = $("#btnOrgFOWSTBPermitCity").text();
                //arOrgAddBPermit[intOrgFowstBPermitCountryID] = $("#txtOrgFOWSTBPermitCountryID").val();
                //arOrgAddBPermit[intOrgFowstBPermitCountry] = $("#btnOrgFOWSTBPermitCountry").text();
                //arOrgAddBPermit[intOrgFowstBPermitORNo] = $("#txtOrgFOWSTBPermitORNo").val();
                //arOrgAddBPermit[intOrgFowstBPermitBusinessPlateNo] = $("#txtOrgFOWSTBPermitBPlateNo").val();
                //arOrgAddBPermit[intOrgFowstBPermitDatePaymentMonth] = $("#selOrgFOWSTBPermitDateOfPaymentMonth").val();
                //arOrgAddBPermit[intOrgFowstBPermitDatePaymentDay] = $("#selOrgFOWSTBPermitDateOfPaymentDay").val();
                //arOrgAddBPermit[intOrgFowstBPermitDatePaymentYear] = $("#selOrgFOWSTBPermitDateOfPaymentYear").val();
                //arOrgAddBPermit[intOrgFowstBPermitDateIssuedMonth] = $("#selOrgFOWSTBPermitBPlateIssuedMonth").val();
                //arOrgAddBPermit[intOrgFowstBPermitDateIssuedDay] = $("#selOrgFOWSTBPermitBPlateIssuedDay").val();
                //arOrgAddBPermit[intOrgFowstBPermitDateIssuedYear] = $("#selOrgFOWSTBPermitBPlateIssuedYear").val();
                //arOrgAddBPermit[intOrgFowstBPermitState] = (IsNewOrgAddBPermit ? "New" : "Edit");

                //sessionStorage["OrgAddBPermit-" + intOrgFowstBPermitIndex] = JSON.stringify(arOrgAddBPermit);

                //$.mobile.changePage('#pageOrgFOWSTInfo');
                $('#pageOrgFOWSTInfo').show();
                $('#pageOrgFOWSTTruckRegInfo').hide();
                $('#pageOrgFOWSTSectionInfo').hide();
                $('#pageOrgFOWSTBusinessPermitInfo').hide();
                $('#pageOrgFOWSTTelInfo').hide();
                $('#pageOrgFOWSTMobileInfo').hide();
                $('#pageOrgFOWSTEmailInfo').hide();
                $('#pageOrgFOWSTOperationHRInfo').hide();
                $('#pageOrgFOWSTPrinterInfo').hide();
                $('#pageOrgFOWSTPrinterIP').hide(); $('#pageAddDevicePrinter').hide();
                LoadOrgFOWSTBPermitList();
            };

            function SaveOrgFOWSTTel() {
                var arTmpTel, arTmpArea;

                arTmpTel = $("#btnOrgFOWSTTelCountryCode").text().split("-");
                arTmpArea = $("#btnOrgFOWSTTelAreaCode").text().split("-");

                var pData = {
                    ID: $.trim($("#txtOrgFOWSTTelID").val()),
                    CountryID: $.trim($("#txtOrgFOWSTTelCountryID").val()),
                    CountryCode: $.trim(arTmpTel[1]),
                    Country: $.trim(arTmpTel[0]),

                    AreaCodeID: $.trim($("#txtOrgFOWSTTelAreaCodeID").val()),
                    Area: $.trim(arTmpArea[0]),
                    AreaCode: $.trim(arTmpArea[1]),

                    ContactNo: $.trim($("#txtOrgFOWSTTelNo").val()),
                    ExtensionNo: $.trim($("#txtOrgFOWSTTelExtension").val()),
                    ContactType: $.trim($("#selOrgFOWSTTelType").val()),
                    RecState: (IsNewOrgAddTelContact ? "New" : "Edit")
                };                

                if (IsNewOrgAddTelContact) {
                    arrFowstTel.push(pData)
                } else {
                    arrFowstTel[intOrgFowstTelIndex] = pData;
                }

                //arOrgAddTel = new Array();
                //arOrgAddTel[intOrgFowstTelRecID] = $("#txtOrgFOWSTTelID").val();
                
                //arOrgAddTel[intOrgFowstTelCountryID] = $("#txtOrgFOWSTTelCountryID").val();
                //arOrgAddTel[intOrgFowstTelCountryCode] = $.trim(arTmp[0]);
                //arOrgAddTel[intOrgFowstTelCallingCode] = $.trim(arTmp[1]);
                
                //arOrgAddTel[intOrgFowstTelAreaCodeID] = $("#txtOrgFOWSTTelAreaCodeID").val();
                //arOrgAddTel[intOrgFowstTelArea] = $.trim(arTmp[0]);
                //arOrgAddTel[intOrgFowstTelAreaCode] = $.trim(arTmp[1]);

                //arOrgAddTel[intOrgFowstTelContactNo] = $("#txtOrgFOWSTTelNo").val();
                //arOrgAddTel[intOrgFowstTelExtNo] = $("#txtOrgFOWSTTelExtension").val();
                //arOrgAddTel[intOrgFowstTelContactType] = $("#selOrgFOWSTTelType").val();
                //arOrgAddTel[intOrgFowstTelState] = (IsNewOrgAddTelContact ? "New" : "Edit");

                //sessionStorage["OrgAddTel-" + intOrgFowstTelIndex] = JSON.stringify(arOrgAddTel);

                //$.mobile.changePage('#pageOrgFOWSTInfo');
                $('#pageOrgFOWSTInfo').show();
                $('#pageOrgFOWSTTruckRegInfo').hide();
                $('#pageOrgFOWSTSectionInfo').hide();
                $('#pageOrgFOWSTBusinessPermitInfo').hide();
                $('#pageOrgFOWSTTelInfo').hide();
                $('#pageOrgFOWSTMobileInfo').hide();
                $('#pageOrgFOWSTEmailInfo').hide();
                $('#pageOrgFOWSTOperationHRInfo').hide();
                $('#pageOrgFOWSTPrinterInfo').hide();
                $('#pageOrgFOWSTPrinterIP').hide(); $('#pageAddDevicePrinter').hide();
                LoadOrgFOWSTTelList();
            };

            function SaveOrgFOWSTMobile() {
                
                var arTmpCountry, arTmpPrefix;

                arTmpCountry = $("#btnOrgFOWSTMobileCountryCode").text().split("-");
                arTmpPrefix = $("#btnOrgFOWSTMobileTelCoPrefixCode").text().split("-");

                var pData = {
                    ID: $.trim($("#txtOrgFOWSTMobileID").val()),
                    CountryID: $.trim($("#txtOrgFOWSTMobileCountryID").val()),
                    Country: $.trim(arTmpCountry[0]),
                    CountryCode: $.trim(arTmpCountry[1]),

                    TelCoPrefixID: $.trim($("#txtOrgFOWSTTelCoPrefixID").val()),
                    TelCo: $.trim(arTmpPrefix[0]),
                    Prefix: $.trim(arTmpPrefix[1]),

                    ContactNo: $.trim($("#txtOrgFOWSTMobileNo").val()),
                    ContactType: $.trim($("#selOrgAddMobileType").val()),
                    RecState: (IsNewOrgAddMobileContact ? "New" : "Edit")
                }

                if (IsNewOrgAddMobileContact) {
                    arrFowstMobile.push(pData);
                } else {
                    arrFowstMobile[intOrgFowstMobileIndex] = pData;
                }

                //arOrgAddMobile = new Array();

                //arOrgAddMobile[intOrgFowstMobileRecID] = $("#txtOrgFOWSTMobileID").val();
                
                //arOrgAddMobile[intOrgFowstMobileCountryID] = $("#txtOrgFOWSTMobileCountryID").val();
                //arOrgAddMobile[intOrgFowstMobileCountryCode] = $.trim(arTmpCountry[0]);
                //arOrgAddMobile[intOrgFowstMobileCallingCode] = $.trim(arTmpCountry[1]);

                
                //arOrgAddMobile[intOrgFowstMobileTelCoPrefixID] = $("#txtOrgFOWSTTelCoPrefixID").val();
                //arOrgAddMobile[intOrgFowstMobilePrefix] = $.trim(arTmpPrefix[1]);
                //arOrgAddMobile[intOrgFowstMobileTelCo] = $.trim(arTmpPrefix[0]);

                //arOrgAddMobile[intOrgFowstMobileContactNo] = $("#txtOrgFOWSTMobileNo").val();
                //arOrgAddMobile[intOrgFowstMobileContactType] = $("#selOrgAddMobileType").val();
                //arOrgAddMobile[intOrgFowstMobileState] = (IsNewOrgAddMobileContact ? "New" : "Edit");

                //sessionStorage["OrgAddMobile-" + intOrgFowstMobileIndex] = JSON.stringify(arOrgAddMobile);

                //$.mobile.changePage('#pageOrgFOWSTInfo');
                $('#pageOrgFOWSTInfo').show();
                $('#pageOrgFOWSTTruckRegInfo').hide();
                $('#pageOrgFOWSTSectionInfo').hide();
                $('#pageOrgFOWSTBusinessPermitInfo').hide();
                $('#pageOrgFOWSTTelInfo').hide();
                $('#pageOrgFOWSTMobileInfo').hide();
                $('#pageOrgFOWSTEmailInfo').hide();
                $('#pageOrgFOWSTOperationHRInfo').hide();
                $('#pageOrgFOWSTPrinterInfo').hide();
                $('#pageOrgFOWSTPrinterIP').hide(); $('#pageAddDevicePrinter').hide();
                LoadOrgFOWSTMobileList();
            };

            function SaveOrgFOWSTEmail() {
                var pData = {
                    ID: $.trim($("#txtOrgFOWSTEmailID").val()),
                    EmailAddress: $.trim($("#txtOrgFOWSTEmail").val()),
                    EmailType: $.trim($("#selOrgFOWSTEmailType").val()),
                    RecState: (IsNewOrgAddEmailContact ? "New" : "Edit")
                }

                if (IsNewOrgAddEmailContact) {
                    arrFowstEmail.push(pData);
                } else {
                    arrFowstEmail[intOrgFowstEmailIndex] = pData;
                }

                //arOrgAddEmail = new Array();
                //arOrgAddEmail[intOrgFowstEmailRecID] = $("#txtOrgFOWSTEmailID").val();
                //arOrgAddEmail[intOrgFowstEmailAddress] = $("#txtOrgFOWSTEmail").val();
                //arOrgAddEmail[intOrgFowstEmailType] = $("#selOrgFOWSTEmailType").val();
                //arOrgAddEmail[intOrgFowstEmailState] = (IsNewOrgAddEmailContact ? "New" : "Edit");

                //sessionStorage["OrgAddEmail-" + intOrgFowstEmailIndex] = JSON.stringify(arOrgAddEmail);

                //$.mobile.changePage('#pageOrgFOWSTInfo');
                $('#pageOrgFOWSTInfo').show();
                $('#pageOrgFOWSTTruckRegInfo').hide();
                $('#pageOrgFOWSTSectionInfo').hide();
                $('#pageOrgFOWSTBusinessPermitInfo').hide();
                $('#pageOrgFOWSTTelInfo').hide();
                $('#pageOrgFOWSTMobileInfo').hide();
                $('#pageOrgFOWSTEmailInfo').hide();
                $('#pageOrgFOWSTOperationHRInfo').hide();
                $('#pageOrgFOWSTPrinterInfo').hide();
                $('#pageOrgFOWSTPrinterIP').hide(); $('#pageAddDevicePrinter').hide();
                LoadOrgFOWSTEmailList();
            };

            function SaveOrgFOWSTOperationHR() {

                arOrgAddOptHR = new Array();
                arOrgAddOptHR[intOrgFowstOptHRRecID] = $("#txtOrgFOWSTOptHRID").val();
                arOrgAddOptHR[intOrgFowstOptHRWeekDay] = $("#selOrgFOWSTOptHRDay").val();
                arOrgAddOptHR[intOrgFowstOptHROpeningTimeHR] = $("#selOrgFOWSTOptOpenHR").val();
                arOrgAddOptHR[intOrgFowstOptHROpeningTimeMin] = $("#selOrgFOWSTOptOpenMin").val();
                arOrgAddOptHR[intOrgFowstOptHROpeningTimeAMPM] = $("#selOrgFOWSTOptOpenAMPM").val();
                arOrgAddOptHR[intOrgFowstOptHRClosingTimeHR] = $("#selOrgFOWSTOptCloseHR").val();
                arOrgAddOptHR[intOrgFowstOptHRClosingTimeMin] = $("#selOrgFOWSTOptCloseMin").val();
                arOrgAddOptHR[intOrgFowstOptHRClosingTimeAMPM] = $("#selOrgFOWSTOptCloseAMPM").val();
                arOrgAddOptHR[intOrgFowstOptHRIsClosed] = $("#isFOWSTClosed").is(':checked');

                sessionStorage["OrgAddOptHR-" + intOrgFowstOptHRIndex] = JSON.stringify(arOrgAddOptHR);

                //$.mobile.changePage('#pageOrgFOWSTInfo');
                $('#pageOrgFOWSTInfo').show();
                $('#pageOrgFOWSTTruckRegInfo').hide();
                $('#pageOrgFOWSTSectionInfo').hide();
                $('#pageOrgFOWSTBusinessPermitInfo').hide();
                $('#pageOrgFOWSTTelInfo').hide();
                $('#pageOrgFOWSTMobileInfo').hide();
                $('#pageOrgFOWSTEmailInfo').hide();
                $('#pageOrgFOWSTOperationHRInfo').hide();
                $('#pageOrgFOWSTPrinterInfo').hide();
                $('#pageOrgFOWSTPrinterIP').hide(); $('#pageAddDevicePrinter').hide();
                LoadOrgFOWSTOperationHRList();
            };

            function RemoveOrgFOWSTLTO() {

                arOrgAddLTO = new Array();
                arOrgAddLTO[intOrgFowstLTORecID] = $("#txtOrgFOWSTTRegID").val();
                arOrgAddLTO[intOrgFowstLTOORNo] = $("#txtOrgFOWSTTRegORNo").val();
                arOrgAddLTO[intOrgFowstLTODateIssuedMonth] = $("#selOrgFOWSTTRegIssuedMonth").val();
                arOrgAddLTO[intOrgFowstLTODateIssuedDay] = $("#selOrgFOWSTTRegIssuedDay").val();
                arOrgAddLTO[intOrgFowstLTODateIssuedYear] = $("#selOrgFOWSTTRegIssuedYear").val();
                arOrgAddLTO[intOrgFowstLTODateExpirationMonth] = $("#selOrgFOWSTTRegExpirationMonth").val();
                arOrgAddLTO[intOrgFowstLTODateExpirationDay] = $("#selOrgFOWSTTRegExpirationDay").val();
                arOrgAddLTO[intOrgFowstLTODateExpirationYear] = $("#selOrgFOWSTTRegExpirationYear").val();
                arOrgAddLTO[intOrgFowstLTOState] = "Remove";

                sessionStorage["OrgAddLTO-" + intOrgFowstLTOIndex] = JSON.stringify(arOrgAddLTO);

                //$.mobile.changePage('#pageOrgFOWSTInfo');
                $('#pageOrgFOWSTInfo').show();
                $('#pageOrgFOWSTTruckRegInfo').hide();
                $('#pageOrgFOWSTSectionInfo').hide();
                $('#pageOrgFOWSTBusinessPermitInfo').hide();
                $('#pageOrgFOWSTTelInfo').hide();
                $('#pageOrgFOWSTMobileInfo').hide();
                $('#pageOrgFOWSTEmailInfo').hide();
                $('#pageOrgFOWSTOperationHRInfo').hide();
                $('#pageOrgFOWSTPrinterInfo').hide();
                $('#pageOrgFOWSTPrinterIP').hide(); $('#pageAddDevicePrinter').hide();
                LoadOrgFOWSTLTOList();
            };

            function RemoveOrgFOWSTLTFRB() {

                arOrgAddLTFRB = new Array();
                arOrgAddLTFRB[intOrgFowstLTFRBRecID] = $("#txtOrgFOWSTTRegID").val();
                arOrgAddLTFRB[intOrgFowstLTFRBORNo] = $("#txtOrgFOWSTTRegORNo").val();
                arOrgAddLTFRB[intOrgFowstLTFRBDateIssuedMonth] = $("#selOrgFOWSTTRegIssuedMonth").val();
                arOrgAddLTFRB[intOrgFowstLTFRBDateIssuedDay] = $("#selOrgFOWSTTRegIssuedDay").val();
                arOrgAddLTFRB[intOrgFowstLTFRBDateIssuedYear] = $("#selOrgFOWSTTRegIssuedYear").val();
                arOrgAddLTFRB[intOrgFowstLTFRBDateExpirationMonth] = $("#selOrgFOWSTTRegExpirationMonth").val();
                arOrgAddLTFRB[intOrgFowstLTFRBDateExpirationDay] = $("#selOrgFOWSTTRegExpirationDay").val();
                arOrgAddLTFRB[intOrgFowstLTFRBDateExpirationYear] = $("#selOrgFOWSTTRegExpirationYear").val();
                arOrgAddLTFRB[intOrgFowstLTFRBState] = "Remove";

                sessionStorage["OrgAddLTFRB-" + intOrgFowstLTFRBIndex] = JSON.stringify(arOrgAddLTFRB);

                //$.mobile.changePage('#pageOrgFOWSTInfo');
                $('#pageOrgFOWSTInfo').show();
                $('#pageOrgFOWSTTruckRegInfo').hide();
                $('#pageOrgFOWSTSectionInfo').hide();
                $('#pageOrgFOWSTBusinessPermitInfo').hide();
                $('#pageOrgFOWSTTelInfo').hide();
                $('#pageOrgFOWSTMobileInfo').hide();
                $('#pageOrgFOWSTEmailInfo').hide();
                $('#pageOrgFOWSTOperationHRInfo').hide();
                $('#pageOrgFOWSTPrinterInfo').hide();
                $('#pageOrgFOWSTPrinterIP').hide(); $('#pageAddDevicePrinter').hide();
                LoadOrgFOWSTLTFRBList();
            };

            function RemoveOrgFOWSTSection() {

                arOrgAddSection = new Array();
                arOrgAddSection[intOrgFowstSecRecID] = $("#txtOrgFOWSTSecID").val();
                arOrgAddSection[intOrgFowstSecCode] = $("#txtOrgFOWSTSecCode").val();
                arOrgAddSection[intOrgFowstSecName] = $("#txtOrgFOWSTSecName").val();
                arOrgAddSection[intOrgFowstSecDescription] = $("#txtOrgFOWSTSecDescription").val();
                arOrgAddSection[intOrgFowstSecFloorArea] = $("#txtOrgFOWSTSecFloorArea").val();
                arOrgAddSection[intOrgFowstSecSpecialFeatures] = $("#txtOrgFOWSTSecSpecFeatures").val();
                arOrgAddSection[intOrgFowstSecState] = "Remove";

                sessionStorage["OrgAddSection-" + intOrgFowstSectionIndex] = JSON.stringify(arOrgAddSection);

                //$.mobile.changePage('#pageOrgFOWSTInfo');
                $('#pageOrgFOWSTInfo').show();
                $('#pageOrgFOWSTTruckRegInfo').hide();
                $('#pageOrgFOWSTSectionInfo').hide();
                $('#pageOrgFOWSTBusinessPermitInfo').hide();
                $('#pageOrgFOWSTTelInfo').hide();
                $('#pageOrgFOWSTMobileInfo').hide();
                $('#pageOrgFOWSTEmailInfo').hide();
                $('#pageOrgFOWSTOperationHRInfo').hide();
                $('#pageOrgFOWSTPrinterInfo').hide();
                $('#pageOrgFOWSTPrinterIP').hide(); $('#pageAddDevicePrinter').hide();
                LoadOrgFOWSTSectionList();
            };

            function RemoveOrgFOWSTBPermit() {

                arOrgAddBPermit = new Array();
                arOrgAddBPermit[intOrgFowstBPermitRecID] = $("#txtOrgFOWSTBPermitRecID").val();
                arOrgAddBPermit[intOrgFowstBPermitNo] = $("#txtOrgFOWSTBPermitNo").val();
                arOrgAddBPermit[intOrgFowstBPermitBusinessID] = $("#txtOrgFOWSTBPermitID").val();
                arOrgAddBPermit[intOrgFowstBPermitTradeName] = $("#txtOrgFOWSTBPermitTradeName").val();
                arOrgAddBPermit[intOrgFowstBPermitBuildingHouseNo] = $("#txtOrgFOWSTBPermitBuildingNo").val();
                arOrgAddBPermit[intOrgFowstBPermitStreet] = $("#txtOrgFOWSTBPermitStreet").val();
                arOrgAddBPermit[intOrgFowstBPermitCityID] = $("#txtOrgFOWSTBPermitCityID").val();
                arOrgAddBPermit[intOrgFowstBPermitCity] = $("#btnOrgFOWSTBPermitCity").text();
                arOrgAddBPermit[intOrgFowstBPermitCountryID] = $("#txtOrgFOWSTBPermitCountryID").val();
                arOrgAddBPermit[intOrgFowstBPermitCountry] = $("#btnOrgFOWSTBPermitCountry").text();
                arOrgAddBPermit[intOrgFowstBPermitORNo] = $("#txtOrgFOWSTBPermitORNo").val();
                arOrgAddBPermit[intOrgFowstBPermitBusinessPlateNo] = $("#txtOrgFOWSTBPermitBPlateNo").val();
                arOrgAddBPermit[intOrgFowstBPermitDatePaymentMonth] = $("#selOrgFOWSTBPermitDateOfPaymentMonth").val();
                arOrgAddBPermit[intOrgFowstBPermitDatePaymentDay] = $("#selOrgFOWSTBPermitDateOfPaymentDay").val();
                arOrgAddBPermit[intOrgFowstBPermitDatePaymentYear] = $("#selOrgFOWSTBPermitDateOfPaymentYear").val();
                arOrgAddBPermit[intOrgFowstBPermitDateIssuedMonth] = $("#selOrgFOWSTBPermitBPlateIssuedMonth").val();
                arOrgAddBPermit[intOrgFowstBPermitDateIssuedDay] = $("#selOrgFOWSTBPermitBPlateIssuedDay").val();
                arOrgAddBPermit[intOrgFowstBPermitDateIssuedYear] = $("#selOrgFOWSTBPermitBPlateIssuedYear").val();
                arOrgAddBPermit[intOrgFowstBPermitState] = "Remove";

                sessionStorage["OrgAddBPermit-" + intOrgFowstBPermitIndex] = JSON.stringify(arOrgAddBPermit);

                //$.mobile.changePage('#pageOrgFOWSTInfo');
                $('#pageOrgFOWSTInfo').show();
                $('#pageOrgFOWSTTruckRegInfo').hide();
                $('#pageOrgFOWSTSectionInfo').hide();
                $('#pageOrgFOWSTBusinessPermitInfo').hide();
                $('#pageOrgFOWSTTelInfo').hide();
                $('#pageOrgFOWSTMobileInfo').hide();
                $('#pageOrgFOWSTEmailInfo').hide();
                $('#pageOrgFOWSTOperationHRInfo').hide();
                $('#pageOrgFOWSTPrinterInfo').hide();
                $('#pageOrgFOWSTPrinterIP').hide(); $('#pageAddDevicePrinter').hide();
                LoadOrgFOWSTBPermitList();
            };

            function RemoveOrgFOWSTTel() {
                var arTmp;

                arOrgAddTel = new Array();
                arOrgAddTel[intOrgFowstTelRecID] = $("#txtOrgFOWSTTelID").val();

                arTmp = $("#btnOrgFOWSTTelCountryCode").text().split("-")
                arOrgAddTel[intOrgFowstTelCountryID] = $("#txtOrgFOWSTTelCountryID").val();
                arOrgAddTel[intOrgFowstTelCountryCode] = $.trim(arTmp[0]);
                arOrgAddTel[intOrgFowstTelCallingCode] = $.trim(arTmp[1]);

                arTmp = $("#btnOrgFOWSTTelAreaCode").text().split("-")
                arOrgAddTel[intOrgFowstTelAreaCodeID] = $("#txtOrgFOWSTTelAreaCodeID").val();
                arOrgAddTel[intOrgFowstTelArea] = $.trim(arTmp[0]);
                arOrgAddTel[intOrgFowstTelAreaCode] = $.trim(arTmp[1]);

                arOrgAddTel[intOrgFowstTelContactNo] = $("#txtOrgFOWSTTelNo").val();
                arOrgAddTel[intOrgFowstTelExtNo] = $("#txtOrgFOWSTTelExtension").val();
                arOrgAddTel[intOrgFowstTelContactType] = $("#selOrgFOWSTTelType").val();
                arOrgAddTel[intOrgFowstTelState] = "Remove";

                sessionStorage["OrgAddTel-" + intOrgFowstTelIndex] = JSON.stringify(arOrgAddTel);

                //$.mobile.changePage('#pageOrgFOWSTInfo');
                $('#pageOrgFOWSTInfo').show();
                $('#pageOrgFOWSTTruckRegInfo').hide();
                $('#pageOrgFOWSTSectionInfo').hide();
                $('#pageOrgFOWSTBusinessPermitInfo').hide();
                $('#pageOrgFOWSTTelInfo').hide();
                $('#pageOrgFOWSTMobileInfo').hide();
                $('#pageOrgFOWSTEmailInfo').hide();
                $('#pageOrgFOWSTOperationHRInfo').hide();
                $('#pageOrgFOWSTPrinterInfo').hide();
                $('#pageOrgFOWSTPrinterIP').hide(); $('#pageAddDevicePrinter').hide();
                LoadOrgFOWSTTelList();
            };

            function RemoveOrgFOWSTMobile() {
                var arTmp;

                arOrgAddMobile = new Array();

                arOrgAddMobile[intOrgFowstMobileRecID] = $("#txtOrgFOWSTMobileID").val();
                arTmp = $("#btnOrgFOWSTMobileCountryCode").text().split("-")
                arOrgAddMobile[intOrgFowstMobileCountryID] = $("#txtOrgFOWSTMobileCountryID").val();
                arOrgAddMobile[intOrgFowstMobileCountryCode] = $.trim(arTmp[0]);
                arOrgAddMobile[intOrgFowstMobileCallingCode] = $.trim(arTmp[1]);

                arTmp = $("#btnOrgFOWSTMobileTelCoPrefixCode").text().split("-")
                arOrgAddMobile[intOrgFowstMobileTelCoPrefixID] = $("#txtOrgFOWSTTelCoPrefixID").val();
                arOrgAddMobile[intOrgFowstMobilePrefix] = $.trim(arTmp[1]);
                arOrgAddMobile[intOrgFowstMobileTelCo] = $.trim(arTmp[0]);

                arOrgAddMobile[intOrgFowstMobileContactNo] = $("#txtOrgFOWSTMobileNo").val();
                arOrgAddMobile[intOrgFowstMobileContactType] = $("#selOrgAddMobileType").val();
                arOrgAddMobile[intOrgFowstMobileState] = "Remove";

                sessionStorage["OrgAddMobile-" + intOrgFowstMobileIndex] = JSON.stringify(arOrgAddMobile);

                //$.mobile.changePage('#pageOrgFOWSTInfo');
                $('#pageOrgFOWSTInfo').show();
                $('#pageOrgFOWSTTruckRegInfo').hide();
                $('#pageOrgFOWSTSectionInfo').hide();
                $('#pageOrgFOWSTBusinessPermitInfo').hide();
                $('#pageOrgFOWSTTelInfo').hide();
                $('#pageOrgFOWSTMobileInfo').hide();
                $('#pageOrgFOWSTEmailInfo').hide();
                $('#pageOrgFOWSTOperationHRInfo').hide();
                $('#pageOrgFOWSTPrinterInfo').hide();
                $('#pageOrgFOWSTPrinterIP').hide(); $('#pageAddDevicePrinter').hide();
                LoadOrgFOWSTMobileList();
            };

            function RemoveOrgFOWSTEmail() {

                arOrgAddEmail = new Array();
                arOrgAddEmail[intOrgFowstEmailRecID] = $("#txtOrgFOWSTEmailID").val();
                arOrgAddEmail[intOrgFowstEmailAddress] = $("#txtOrgFOWSTEmail").val();
                arOrgAddEmail[intOrgFowstEmailType] = $("#selOrgFOWSTEmailType").val();
                arOrgAddEmail[intOrgFowstEmailState] = "Remove";

                sessionStorage["OrgAddEmail-" + intOrgFowstEmailIndex] = JSON.stringify(arOrgAddEmail);

                //$.mobile.changePage('#pageOrgFOWSTInfo');
                $('#pageOrgFOWSTInfo').show();
                $('#pageOrgFOWSTTruckRegInfo').hide();
                $('#pageOrgFOWSTSectionInfo').hide();
                $('#pageOrgFOWSTBusinessPermitInfo').hide();
                $('#pageOrgFOWSTTelInfo').hide();
                $('#pageOrgFOWSTMobileInfo').hide();
                $('#pageOrgFOWSTEmailInfo').hide();
                $('#pageOrgFOWSTOperationHRInfo').hide();
                $('#pageOrgFOWSTPrinterInfo').hide();
                $('#pageOrgFOWSTPrinterIP').hide(); $('#pageAddDevicePrinter').hide();
                LoadOrgFOWSTEmailList();
            };

            function RemoveOrgFOWSTPrinter() {

                arOrgAddPrinter = new Array();
                arOrgAddPrinter[intOrgFowstPrinterID] = $("#txtPrinterFowstID").val();
                arOrgAddPrinter[intOrgFowstPrinterName] = $("#txtPrinterName").val();
                arOrgAddPrinter[intOrgFowstPrinterRole] = $("#txtPrinterRole").text();
                arOrgAddPrinter[intOrgFowstPrinterRoleID] = $("#txtPrinterRoleID").val();
                arOrgAddPrinter[intOrgFowstPrinterIPAddress] = $("#txtPrinterIPAddress").val();
                arOrgAddPrinter[intOrgFowstPrinterState] = "Remove";

                sessionStorage["OrgAddPrinter-" + intOrgFowstPrinterIndex] = JSON.stringify(arOrgAddPrinter);

                //$.mobile.changePage('#pageOrgFOWSTInfo');
                $('#pageOrgFOWSTInfo').show();
                $('#pageOrgFOWSTTruckRegInfo').hide();
                $('#pageOrgFOWSTSectionInfo').hide();
                $('#pageOrgFOWSTBusinessPermitInfo').hide();
                $('#pageOrgFOWSTTelInfo').hide();
                $('#pageOrgFOWSTMobileInfo').hide();
                $('#pageOrgFOWSTEmailInfo').hide();
                $('#pageOrgFOWSTOperationHRInfo').hide();
                $('#pageOrgFOWSTPrinterInfo').hide();
                $('#pageOrgFOWSTPrinterIP').hide(); $('#pageAddDevicePrinter').hide();
                LoadOrgPrinterList();
            };


            function LoadOrgFOWSTLTOList() {
                var pLV = $('#lstOrgFOWSTLTO');
                var pItem;

                pLV.html('');
                for (var i = 0; i <= intOrgFowstLTOCntr; i++) {

                    if (sessionStorage["OrgAddLTO-" + i] != undefined) {

                        arOrgAddLTO = new Array();
                        arOrgAddLTO = JSON.parse(sessionStorage["OrgAddLTO-" + i]);

                        if (arOrgAddLTO[intOrgFowstLTOState] != "Remove") {
                            pItem = $('<a href="#" class="list-group-item list-group-item-action">')
                                    .attr('id', 'lst' + i)
                                    .append($('<table style="width:100%;">')
                                        .append($('<tr>')
                                            .append($('<td style="word-break:break-all;">')
                                                .attr('ID', i)
                                                .data('Data', arOrgAddLTO)
                                                .append($('<h5>')
                                                    .attr("style", "white-space:normal")
                                                    .text(arOrgAddLTO[intOrgFowstLTOORNo])
                                                )
                                                .append($('<div>')
                                                    .attr("style", "white-space:normal")
                                                    .text("Date Issued : " + arOrgAddLTO[intOrgFowstLTODateIssuedMonth] + "/" + arOrgAddLTO[intOrgFowstLTODateIssuedDay] + "/" + arOrgAddLTO[intOrgFowstLTODateIssuedYear])
                                                )
                                                .append($('<div>')
                                                    .attr("style", "white-space:normal")
                                                    .text("Expiration Date : " + arOrgAddLTO[intOrgFowstLTODateExpirationMonth] + "/" + arOrgAddLTO[intOrgFowstLTODateExpirationDay] + "/" + arOrgAddLTO[intOrgFowstLTODateExpirationYear])
                                                )
                                                .click(function (e) {
                                                    e.preventDefault();
                                                    ShowOrgAddLTFRBInfo($(this).data('Data'), $(this).attr('ID'));
                                                })
                                            )
                                        )
                                    )

                            pLV.append(pItem);

                        }

                    }
                }

                pLV;//.listview('refresh');
            };

            function LoadOrgFOWSTLTFRBList() {
                var pLV = $('#lstOrgFOWSTLTFRB');
                var pItem;

                pLV.html('');
                for (var i = 0; i <= intOrgFowstLTFRBCntr; i++) {

                    if (sessionStorage["OrgAddLTFRB-" + i] != undefined) {

                        arOrgAddLTFRB = new Array();
                        arOrgAddLTFRB = JSON.parse(sessionStorage["OrgAddLTFRB-" + i]);

                        if (arOrgAddLTFRB[intOrgFowstLTFRBState] != "Remove") {
                            pItem = $('<a href="#" class="list-group-item list-group-item-action">')
                                    .attr('id', 'lst' + i)
                                    .append($('<table style="width:100%;">')
                                        .append($('<tr>')
                                            .append($('<td style="word-break:break-all;">')
                                                .attr('ID', i)
                                                .data('Data', arOrgAddLTFRB)
                                                .append($('<h5>')
                                                    .attr("style", "white-space:normal")
                                                    .text(arOrgAddLTFRB[intOrgFowstLTFRBORNo])
                                                )
                                                .append($('<div>')
                                                    .attr("style", "white-space:normal")
                                                    .text("Date Issued : " + arOrgAddLTFRB[intOrgFowstLTFRBDateIssuedMonth] + "/" + arOrgAddLTFRB[intOrgFowstLTFRBDateIssuedDay] + "/" + arOrgAddLTFRB[intOrgFowstLTFRBDateIssuedYear])
                                                )
                                                .append($('<div>')
                                                    .attr("style", "white-space:normal")
                                                    .text("Expiration Date : " + arOrgAddLTFRB[intOrgFowstLTFRBDateExpirationMonth] + "/" + arOrgAddLTFRB[intOrgFowstLTFRBDateExpirationDay] + "/" + arOrgAddLTFRB[intOrgFowstLTFRBDateExpirationYear])
                                                )
                                                .click(function (e) {
                                                    e.preventDefault();
                                                    ShowOrgAddLTFRBInfo($(this).data('Data'), $(this).attr('ID'));
                                                })
                                            )
                                        )
                                    )

                            pLV.append(pItem);

                        }

                    }
                }

                pLV;//.listview('refresh');
            };

            function LoadOrgFOWSTSectionList() {
                var pLV = $('#lstOrgFOWSTSection');
                var pItem, isEmpty = true;

                pLV.html('');
                for (var i = 0; i < arrFowstSection.length; i++) {
                    arrFowstSection[i].Index = i;

                    if (arrFowstSection[i].RecState != 'Remove') {
                        isEmpty = false;
                        pItem = $('<a href="#" class="list-group-item list-group-item-action">')
                                    .attr('id', 'lst' + i)
                                    .append($('<table style="width:100%;">')
                                        .append($('<tr>')
                                            .append($('<td style="word-break:break-all;">')
                                                .attr('ID', i)
                                                .data('Data', arrFowstSection[i])
                                                .append($('<h5>')
                                                    .attr("style", "white-space:normal")
                                                    .text(arrFowstSection[i].Name)
                                                )
                                                .click(function (e) {
                                                    e.preventDefault();
                                                    ShowOrgAddSectionInfo($(this).data('Data'), $(this).attr('ID'));
                                                })
                                            )
                                            .append($('<td style="width:50px; text-align:center;">').append($('<span class="btn btn-xs btn-danger">')
                                                    .append($('<span class="bi-trash-fill" aria-hidden="true">'))
                                                    .attr('dID', i)
                                                    .data('Data', arrFowstSection[i])
                                                    .click(function (e) {
                                                        e.preventDefault();
                                                        var data = $(this).data('Data');
                                                        var index = $(this).attr('dID');

                                                        NewMessageBox('Are you sure you want to remove this section?', 'Message', [
                                                            {
                                                                Text: 'Yes', Click: function () {
                                                                    if (data.RecState == "New") {
                                                                        arrFowstSection.splice(data.Index, 1);
                                                                    } else {
                                                                        arrFowstSection[data.Index].RecState = "Remove";
                                                                    }
                                                                    LoadOrgFOWSTSectionList();
                                                                }
                                                            }, { Text: 'No' }
                                                        ])
                                                    })
                                                )
                                            )
                                        )
                                    )

                        pLV.append(pItem);
                    }
                }

                if (isEmpty) {
                    pLV.append($('<a data-role="none" href="#" class="list-group-item list-group-item-action">').text('No results found'));
                }

                pLV;//.listview('refresh');
            };
            function SaveFowstBinSection(id) {
                if (CStr2($("#txtOrgFOWSTSecID").val()) != '') {
                    var data = {
                        FowstBinID: id,
                        SectionID: $("#txtOrgFOWSTSecID").val(),
                        IsMove: 0
                    };

                    AjaxSend({
                        url: 'Organization/SaveFowstBinSection',
                        data: data,
                        success: function (vData) {
                            if (vData.Result == 'ok') {
                                GetOrgFOWSTSectionBinList();
                            } else if (vData.Result == 'existing') {
                                NewMessageBox('This bin already exists in a different section. Do you want to move it to this section?', 'Existing Bin',
                                    [
                                        {
                                            Text: 'Yes',
                                            Click: function(){
                                                data = {
                                                    FowstBinID: id,
                                                    SectionID: $("#txtOrgFOWSTSecID").val(),
                                                    IsMove: 1
                                                };
                                                AjaxSend({
                                                    url: 'Organization/SaveFowstBinSection',
                                                    data: data,
                                                    success: function (vData) {
                                                        if (vData.Result == 'ok') {
                                                            GetOrgFOWSTSectionBinList();
                                                        }
                                                    }
                                                });
                                            }
                                        }, { Text: 'No'}
                                    ])

                            } else {
                                NewMessageBox('There was an error while adding section', 'Error');
                            }

                        }
                    });
                } else {
                    alert('Not yet implemented for new sections. Only existing sections can add bin. Please save this section first. Click "Ok" in this page then click "Save" in Organization Address page.');
                }
            }

            function GetOrgFOWSTSectionBinList() {
                var data = {
                    SectionID: $("#txtOrgFOWSTSecID").val(),
                    FowstID: fowstID
                };

                AjaxSend({
                    url: 'Organization/GetOrgFOWSTSectionBinList',
                    data: data,
                    success: function (vData) {

                        if (vData.Data != undefined)
                            LoadOrgFOWSTSectionBinList(vData.Data.Rows);
                    }
                });
            }
            function SaveFowstSectionPeople(id) {
                if (CStr2($("#txtOrgFOWSTSecID").val()) != '') {

                    var data = {
                        PeopleID: id,
                        FowstID: fowstID,
                        SectionID: $("#txtOrgFOWSTSecID").val()
                    };

                    AjaxSend({
                        url: 'Organization/SaveFowstSectionPeople',
                        data: data,
                        success: function (vData) {
                            if (vData.Result == 'ok')
                                GetOrgFOWSTSectionPeopleList();
                        }
                    });
                } else {
                    alert('Not yet implemented for new sections. Only existing sections can add section picker. Please save this section first. Click "Ok" in this page then click "Save" in Organization Address page.');
                }
            }


            function GetOrgFOWSTSectionPeopleList() {
                var data = {
                    SectionID: $("#txtOrgFOWSTSecID").val(),
                    FowstID: fowstID
                };

                AjaxSend({
                    url: 'Organization/GetOrgFOWSTSectionPeopleList',
                    data: data,
                    success: function (vData) {
                        if (vData.Data != undefined)
                            LoadOrgFOWSTSectionPeopleList(vData.Data.Rows);
                    }
                });
            }
            function LoadOrgFOWSTSectionBinList(vRows) {
                var pLV = $('#lstOrgFOWSTSectionBin');
                var pItem;

                pLV.html('');

                for (var i = 0; i < vRows.length; i++) {
                    pItem = $('<a href="#" class="list-group-item list-group-item-action">')
                                .attr('id', 'lst' + vRows[i].ID)
                                .append($('<table style="width:100%;">')
                                    .append($('<tr>')
                                        .append($('<td style="word-break:break-all;">')
                                            .append($('<h5>')
                                                .attr("style", "white-space:normal")
                                                .text(vRows[i].Name)
                                            )
                                        )
                                        .append($('<td style="width:50px; text-align:center;">').append($('<span class="btn btn-xs btn-danger">')
                                                .append($('<span class="bi-trash-fill" aria-hidden="true">'))
                                                .attr('ID', vRows[i].ID)
                                                .click(function (e) {
                                                    e.preventDefault();
                                                    var id = $(this).attr('ID');
                                                    NewMessageBox('Remove this record?', 'Remove',
                                                        [
                                                            {
                                                                Text: 'Yes',
                                                                Click: function(){
                                                                    AjaxSend({
                                                                        url: 'Organization/RemoveFowstBinSection',
                                                                        data: { ID: id },
                                                                        success: function (vData) {
                                                                            if (vData.Result == 'ok') {
                                                                                //$('#panOrgEmpMessage').popup('close');
                                                                                //                                                history.back();
                                                                                GetOrgFOWSTSectionBinList();
                                                                            }
                                                                        }
                                                                    });
                                                                }
                                                            }, { Text: 'No'}
                                                        ])
                                                })
                                            )
                                        )
                                    )
                                )

                    pLV.append(pItem);
                }

                pLV;//.listview('refresh');
            };


            $('#btnOKOrgEmp').text('Yes').click(function (e) {
                e.preventDefault();

                if (isDeleteSectionPicker) {
                    AjaxSend({
                        url: 'Organization/RemoveFowstSectionPeople',
                        data: { ID: id },
                        success: function (vData) {
                            if (vData.Result == 'ok') {
                                //$('#panOrgEmpMessage').popup('close');
                                //                                                history.back();
                                GetOrgFOWSTSectionPeopleList();
                            }
                        }
                    });
                }
            });


            var id;
            var isDeleteSectionPicker = false;

            function LoadOrgFOWSTSectionPeopleList(vRows) {
                var pLV = $('#lstOrgFOWSTSectionPeople');
                var pItem;

                pLV.html('');

                for (var i = 0; i < vRows.length; i++) {
                    pItem = $('<a href="#" class="list-group-item list-group-item-action">')
                                .attr('id', 'lst' + vRows[i].ID)
                                .append($('<table style="width:100%;">')
                                    .append($('<tr>')
                                        .append($('<td style="word-break:break-all;">')
                                            .append($('<h5>')
                                                .attr("style", "white-space:normal")
                                                .text(vRows[i].Name)
                                            )
                                        )
                                        .append($('<td style="width:50px; text-align:center;">').append($('<span class="btn btn-xs btn-danger">')
                                                .append($('<span class="bi-trash-fill" aria-hidden="true">'))
                                                .attr('ID', vRows[i].ID)
                                                .click(function (e) {
                                                    e.preventDefault();
                                                    var id = $(this).attr('ID');
                                                    NewMessageBox('Remove this record?', 'Remove',
                                                        [
                                                            {
                                                                Text: 'Yes',
                                                                Click: function () {
                                                                    AjaxSend({
                                                                        url: 'Organization/RemoveFowstSectionPeople',
                                                                        data: { ID: id },
                                                                        success: function (vData) {
                                                                            if (vData.Result == 'ok') {
                                                                                //$('#panOrgEmpMessage').popup('close');
                                                                                //                                                history.back();
                                                                                lstOrgFOWSTSectionPeople();
                                                                            }
                                                                        }
                                                                    });
                                                                }
                                                            }, { Text: 'No' }
                                                        ])
                                                })
                                            )
                                        )
                                    )
                                )

                    pLV.append(pItem);

                }

                pLV;//.listview('refresh');
            };
            function LoadOrgFOWSTBPermitList() {
                var pLV = $('#lstOrgFOWSTBusinessPermit');
                var pItem, isEmpty = true;

                pLV.html('');

                for (var i = 0; i < arrFowstBusinessPermit.length; i++) {
                    arrFowstBusinessPermit[i].Index = i;

                    if (arrFowstBusinessPermit[i].RecState != 'Remove') {
                        isEmpty = false;
                        pItem = $('<a href="#" class="list-group-item list-group-item-action">')
                                    .attr('id', 'lst' + i)
                                    .append($('<table style="width:100%;">')
                                        .append($('<tr>')
                                            .append($('<td style="word-break:break-all;">')
                                                .attr('ID', i)
                                                .data('Data', arrFowstBusinessPermit[i])
                                                .append($('<h5>')
                                                    .attr("style", "white-space:normal")
                                                    .text(arrFowstBusinessPermit[i].TradeName)
                                                )
                                                .append($('<div>')
                                                    .attr("style", "white-space:normal")
                                                    .text('Business Permit No.: ' + arrFowstBusinessPermit[i].BusinessPermitNo)
                                                )
                                                .append($('<div>')
                                                    .attr("style", "white-space:normal")
                                                    .text('Date Issued: ' + (CStr2(arrFowstBusinessPermit[i].DateIssued) == '' ? '' : arrFowstBusinessPermit[i].DateIssued))
                                                )
                                                .click(function (e) {
                                                    e.preventDefault();
                                                    ShowOrgAddBPermitInfo($(this).data('Data'), $(this).attr('ID'));
                                                })
                                            )
                                            .append($('<td style="width:50px; text-align:center;">').append($('<span class="btn btn-xs btn-danger">')
                                                    .append($('<span class="bi-trash-fill" aria-hidden="true">'))
                                                    .attr('dID', i)
                                                    .data('Data', arrFowstBusinessPermit[i])
                                                    .click(function (e) {
                                                        e.preventDefault();
                                                        var data = $(this).data('Data');
                                                        var index = $(this).attr('dID');

                                                        NewMessageBox('Are you sure you want to remove this business permit?', 'Message', [
                                                            {
                                                                Text: 'Yes', Click: function () {
                                                                    if (data.RecState == "New") {
                                                                        arrFowstBusinessPermit.splice(data.Index, 1);
                                                                    } else {
                                                                        arrFowstBusinessPermit[data.Index].RecState = "Remove";
                                                                    }
                                                                    LoadOrgFOWSTBPermitList();
                                                                }
                                                            }, { Text: 'No' }
                                                        ])
                                                    })
                                                )
                                            )
                                        )
                                    )

                        pLV.append(pItem);
                    }

                    //if (sessionStorage["OrgAddBPermit-" + i] != undefined) {                        
                    //    arOrgAddBPermit = new Array();
                    //    arOrgAddBPermit = JSON.parse(sessionStorage["OrgAddBPermit-" + i]);

                    //    if (arOrgAddBPermit[intOrgFowstBPermitState] != "Remove") {
                    //        isEmpty = false;
                    //        tbodyFowstBusinessPermit.append($('<tr>')
                    //                .append($('<td>')
                    //                    .text(arOrgAddBPermit[intOrgFowstBPermitTradeName])
                    //                )
                    //                .append($('<td>')
                    //                    .text(arOrgAddBPermit[intOrgFowstBPermitNo])
                    //                )
                    //                .append($('<td>')
                    //                    .text(CStr2(arOrgAddBPermit[intOrgFowstBPermitDateIssuedMonth]) == '' ? '' : arOrgAddBPermit[intOrgFowstBPermitDateIssuedMonth] + "/" + arOrgAddBPermit[intOrgFowstBPermitDateIssuedDay] + "/" + arOrgAddBPermit[intOrgFowstBPermitDateIssuedYear])
                    //                )
                    //                .attr('ID', i)
                    //                .data('Data', arOrgAddBPermit)
                    //                .click(function (e) {
                    //                    e.preventDefault();
                    //                    ShowOrgAddBPermitInfo($(this).data('Data'), $(this).attr('ID'));
                    //                })
                    //            )
                    //    }
                    //}
                }

                if (isEmpty) {
                    pLV.append($('<a data-role="none" href="#" class="list-group-item list-group-item-action">').text('No results found'));
                }

                pLV;//.listview('refresh');
            };

            function LoadOrgFOWSTTelList() {
                var pLV = $('#lstOrgFOWSTTelephone');
                var pItem, isEmpty = true;

                pLV.html('');
                for (var i = 0; i < arrFowstTel.length; i++) {

                    arrFowstTel[i].Index = i;

                    if (arrFowstTel[i].RecState != 'Remove') {
                        isEmpty = false;
                        pItem = $('<a href="#" class="list-group-item list-group-item-action">')
                                    .attr('id', 'lst' + i)
                                    .append($('<table style="width:100%;">')
                                        .append($('<tr>')
                                            .append($('<td style="word-break:break-all;">')
                                                .attr('ID', i)
                                                .data('Data', arrFowstTel[i])
                                                .append($('<h5>')
                                                    .attr("style", "white-space:normal")
                                                    .text("+" + arrFowstTel[i].CountryCode + "." + arrFowstTel[i].AreaCode + '.' + arrFowstTel[i].ContactNo + ((arrFowstTel[i].ExtensionNo || '') == '' ? '' : ", Ext. " + arrFowstTel[i].ExtensionNo))
                                                )
                                                .append($('<div>')
                                                    .attr("style", "white-space:normal")
                                                    .text(arrFowstTel[i].Area + ", " + arrFowstTel[i].Country)
                                                )
                                                .append($('<div>')
                                                    .attr("style", "white-space:normal")
                                                    .text(arrFowstTel[i].ContactType)
                                                )
                                                .click(function (e) {
                                                    e.preventDefault();
                                                    ShowOrgAddTelInfo($(this).data('Data'), $(this).attr('ID'));
                                                })
                                            )
                                            .append($('<td style="width:50px; text-align:center;">').append($('<span class="btn btn-xs btn-danger">')
                                                    .append($('<span class="bi-trash-fill" aria-hidden="true">'))
                                                    .attr('dID', i)
                                                    .data('Data', arrFowstTel[i])
                                                    .click(function (e) {
                                                        e.preventDefault();
                                                        var data = $(this).data('Data');
                                                        var index = $(this).attr('dID');

                                                        NewMessageBox('Are you sure you want to remove this telephone?', 'Message', [
                                                            {
                                                                Text: 'Yes', Click: function () {
                                                                    if (data.RecState == "New") {
                                                                        arrFowsTel.splice(data.Index, 1);
                                                                    } else {
                                                                        arrFowstTel[data.Index].RecState = "Remove";
                                                                    }
                                                                    LoadOrgFOWSTTelList();
                                                                }
                                                            }, { Text: 'No' }
                                                        ])
                                                    })
                                                )
                                            )
                                        )
                                    )

                        pLV.append(pItem);
                    }
                }

                if (isEmpty) {
                    pLV.append($('<a data-role="none" href="#" class="list-group-item list-group-item-action">').text('No results found'));
                }

                pLV;//.listview('refresh');
            };

            function LoadOrgFOWSTMobileList() {
                var pLV = $('#lstOrgFOWSTMobile');
                var pItem, isEmpty = true;

                pLV.html('');
                for (var i = 0; i < arrFowstMobile.length; i++) {
                    arrFowstMobile[i].Index = i;

                    if (arrFowstMobile[i].RecState != "Remove") {
                        isEmpty = false;
                        pItem = $('<a href="#" class="list-group-item list-group-item-action">')
                                    .attr('id', 'lst' + i)
                                    .append($('<table style="width:100%;">')
                                        .append($('<tr>')
                                            .append($('<td style="word-break:break-all;">')
                                                .attr('ID', i)
                                                .data('Data', arrFowstMobile[i])
                                                .append($('<h5>')
                                                    .attr("style", "white-space:normal")
                                                    .text("+" + arrFowstMobile[i].CountryCode + "." + arrFowstMobile[i].Prefix + "." + arrFowstMobile[i].ContactNo)
                                                )
                                                .append($('<div>')
                                                    .attr("style", "white-space:normal")
                                                    .text(arrFowstMobile[i].TelCo + " (" + arrFowstMobile[i].Country + ")")
                                                )
                                                .append($('<div>')
                                                    .attr("style", "white-space:normal")
                                                    .text(arrFowstMobile[i].ContactType)
                                                )
                                                .click(function (e) {
                                                    e.preventDefault();
                                                    ShowOrgAddMobileInfo($(this).data('Data'), $(this).attr('ID'));
                                                })
                                            )
                                            .append($('<td style="width:50px; text-align:center;">').append($('<span class="btn btn-xs btn-danger">')
                                                    .append($('<span class="bi-trash-fill" aria-hidden="true">'))
                                                    .attr('dID', i)
                                                    .data('Data', arrFowstMobile[i])
                                                    .click(function (e) {
                                                        e.preventDefault();
                                                        var data = $(this).data('Data');
                                                        var index = $(this).attr('dID');

                                                        NewMessageBox('Are you sure you want to remove this mobile number?', 'Message', [
                                                            {
                                                                Text: 'Yes', Click: function () {
                                                                    if (data.RecState == "New") {
                                                                        arrFowstMobile.splice(data.Index, 1);
                                                                    } else {
                                                                        arrFowstMobile[data.Index].RecState = "Remove";
                                                                    }
                                                                    LoadOrgFOWSTMobileList();
                                                                }
                                                            }, { Text: 'No' }
                                                        ])
                                                    })
                                                )
                                            )
                                        )
                                    )

                        pLV.append(pItem);
                    }
                }

                if (isEmpty) {
                    pLV.append($('<a data-role="none" href="#" class="list-group-item list-group-item-action">').text('No results found'));
                }

                pLV;//.listview('refresh');
            };


            function LoadOrgFOWSTEmailList() {
                var pLV = $('#lstOrgFOWSTEmail');
                var pItem, isEmpty = true;

                pLV.html('');
                for (var i = 0; i < arrFowstEmail.length; i++) {
                    arrFowstEmail[i].Index = i;

                    if (arrFowstEmail[i].RecState != 'Remove') {
                        isEmpty = false;
                        pItem = $('<a href="#" class="list-group-item list-group-item-action">')
                                    .attr('id', 'lst' + i)
                                    .append($('<table style="width:100%;">')
                                        .append($('<tr>')
                                            .append($('<td style="word-break:break-all;">')
                                                .attr('ID', i)
                                                .data('Data', arrFowstEmail[i])
                                                .append($('<h5>')
                                                    .attr("style", "white-space:normal")
                                                    .text(arrFowstEmail[i].EmailAddress)
                                                )
                                                .append($('<div>')
                                                    .attr("style", "white-space:normal")
                                                    .text(arrFowstEmail[i].EmailType)
                                                )
                                                .click(function (e) {
                                                    e.preventDefault();
                                                    ShowOrgAddEmailInfo($(this).data('Data'), $(this).attr('ID'));
                                                })
                                            )
                                            .append($('<td style="width:50px; text-align:center;">').append($('<span class="btn btn-xs btn-danger">')
                                                    .append($('<span class="bi-trash-fill" aria-hidden="true">'))
                                                    .attr('dID', i)
                                                    .data('Data', arrFowstEmail[i])
                                                    .click(function (e) {
                                                        e.preventDefault();
                                                        var data = $(this).data('Data');
                                                        var index = $(this).attr('dID');

                                                        NewMessageBox('Are you sure you want to remove this email address?', 'Message', [
                                                            {
                                                                Text: 'Yes', Click: function () {
                                                                    if (data.RecState == "New") {
                                                                        arrFowstEmail.splice(data.Index, 1);
                                                                    } else {
                                                                        arrFowstEmail[data.Index].RecState = "Remove";
                                                                    }
                                                                    LoadOrgFOWSTEmailList();
                                                                }
                                                            }, { Text: 'No' }
                                                        ])
                                                    })
                                                )
                                            )
                                        )
                                    )

                        pLV.append(pItem);
                    }
                }

                if (isEmpty) {
                    pLV.append($('<a data-role="none" href="#" class="list-group-item list-group-item-action">').text('No results found'));
                }

                pLV;//.listview('refresh');
            };

            function LoadOrgFOWSTOperationHRList() {
                var pLV = $('#lstOrgFOWSTOperationHR');
                var tbodyFowstOperationHours = $('#tbodyFowstOperationHours');
                var pItem;

                pLV.html('');
                tbodyFowstOperationHours.empty();

                for (var i = 0; i <= intOrgFowstOptHRCntr; i++) {

                    if (sessionStorage["OrgAddOptHR-" + i] != undefined) {

                        arOrgAddOptHR = new Array();
                        arOrgAddOptHR = JSON.parse(sessionStorage["OrgAddOptHR-" + i]);

                        var strDay = "";
                        if (arOrgAddOptHR[intOrgFowstOptHRWeekDay] == 1) {
                            strDay = "Sunday";
                        } else if (arOrgAddOptHR[intOrgFowstOptHRWeekDay] == 2) {
                            strDay = "Monday";
                        } else if (arOrgAddOptHR[intOrgFowstOptHRWeekDay] == 3) {
                            strDay = "Tuesday";
                        } else if (arOrgAddOptHR[intOrgFowstOptHRWeekDay] == 4) {
                            strDay = "Wednesday";
                        } else if (arOrgAddOptHR[intOrgFowstOptHRWeekDay] == 5) {
                            strDay = "Thursday";
                        } else if (arOrgAddOptHR[intOrgFowstOptHRWeekDay] == 6) {
                            strDay = "Friday";
                        } else if (arOrgAddOptHR[intOrgFowstOptHRWeekDay] == 7) {
                            strDay = "Saturday";
                        }

                        tbodyFowstOperationHours.append($('<tr>')
                                .append($('<td>')
                                    .text(strDay)
                                )
                                .append($('<td>')
                                    .text(CStr2(arOrgAddOptHR[intOrgFowstOptHROpeningTimeHR]) == '' ? '-' : ($.format.date(new Date("1/1/2013 " + arOrgAddOptHR[intOrgFowstOptHROpeningTimeHR] + ":" + arOrgAddOptHR[intOrgFowstOptHROpeningTimeMin] + " " + arOrgAddOptHR[intOrgFowstOptHROpeningTimeAMPM]), 'hh:mm a')))
                                )
                                .append($('<td>')
                                    .text((CStr2(arOrgAddOptHR[intOrgFowstOptHRClosingTimeHR]) == '' ? '-' : $.format.date(new Date("1/1/2013 " + arOrgAddOptHR[intOrgFowstOptHRClosingTimeHR] + ":" + arOrgAddOptHR[intOrgFowstOptHRClosingTimeMin] + " " + arOrgAddOptHR[intOrgFowstOptHRClosingTimeAMPM]), 'hh:mm a')))
                                )
                                .append($('<td>')
                                    .text((CBool(arOrgAddOptHR[intOrgFowstOptHRIsClosed]) ? 'Close' : 'Open'))
                                )
                                .attr('ID', i)
                                .data('Data', arOrgAddOptHR)
                                .click(function (e) {
                                    e.preventDefault();
                                    ShowOrgAddOperationHRInfo($(this).data('Data'), $(this).attr('ID'));
                                })
                            )
                    }
                }

                pLV;//.listview('refresh');
            };

            function GetAllFOWSTPrinter() {
                $("#lstOrgFOWSTPrinter").hide();

                AjaxSend({
                    url: 'DevicePrinter/getDevicePrinterListByFowstID',
                    data: { FowstID: fowstID },
                    success: function (vData) {
                        LoadDevicePrinterList(vData.DevicePrinterData.Rows, true);
                    }
                });
            }


            function GetAllFOWSTPrinterAvailable() {

                AjaxSend({
                    url: 'DevicePrinter/getDevicePrinterListAvailable',
                    success: function (vData) {
                        LoadDevicePrinterList(vData.DevicePrinterData.Rows, false);
                    }
                });
            }

            var isPrinterListEmpty = true;
            function LoadDevicePrinterList(vRows, include) {

                var pLV = '', tbodyFowstPrinter = $('#tbodyFowstPrinter');

                if (include) {
                    pLV = $('#lstOrgFOWSTPrinter');
                    tbodyFowstPrinter.empty();
                } else
                    pLV = $('#lstOrgFOWSTPrinterList');

                var pItem;

                $("#divDevicePrinterResults").hide();
                $("#divDevicePrinterSearchNoResults").hide();
                //                $("#btnAddNewDevicePrinter").hide();
                if (vRows.length > 0) {
                    isPrinterListEmpty = false;


                    pLV.html('');

                    if (include) {
                        $("#lstOrgFOWSTPrinter").show();

                        for (var p = 0; p <= vRows.length - 1; p++) {
                            tbodyFowstPrinter.append($('<tr>')
                                    .append($('<td>')
                                        .text(vRows[p].PrinterName)
                                    )
                                    .append($('<td>')
                                        .text(vRows[p].PrinterRole)
                                    )
                                    .append($('<td>')
                                        .text(vRows[p].PrinterIPAddress)
                                    )
                                    .append($('<td>')
                                        .append($('<button data-role="none" class=" btn custom-btn-theme-d btnCircle-xs">')
                                            .append($('<span class="glyphicon glyphicon-remove">'))
                                        )
                                        .data('data', vRows[p])
                                        .click(function (e) {
                                            e.stopPropagation();
                                            e.preventDefault();

                                            var data = $(this).data('data');

                                            NewMessageBox('Are you sure you want to remove printer <b>' + data.PrinterName + '</b>?', 'Message', [
                                                {
                                                    Text: 'Yes', Click: function () {
                                                        AjaxSend({
                                                            url: 'DevicePrinter/RemovePrinter',
                                                            data: { ID: data.PrinterFowstID },
                                                            success: function (e) {
                                                                if (e.Results == 'saved') {
                                                                    WebToast('Successfully deleted', 'top');
                                                                    GetAllFOWSTPrinter();
                                                                } else {
                                                                    NewMessageBox(e.Results, 'Error');
                                                                }
                                                            }
                                                        })
                                                    }
                                                }, { Text: 'No' }
                                            ])
                                        })
                                    )
                                    .data('ID', vRows[p].ID)
                                    .data('IPAddress', vRows[p].PrinterIPAddress)
                                    .data('PrinterRole', vRows[p].PrinterRole)
                                    .data('PrinterName', vRows[p].PrinterName)
                                    .data('PrinterFowstID', vRows[p].PrinterFowstID)
                                    .attr('ID', vRows[p].ID)
                                    .click(function (e) {
                                        e.preventDefault();

                                        $('#txtPrinterRoleID').val($(this).data('ID'));
                                        //                                $('#txtPrinterRole').show();
                                        $('#txtPrinterRole').text($(this).data('PrinterRole'));
                                        $('#txtPrinterName').val($(this).data('PrinterName'));
                                        $('#txtPrinterIPAddress').val($(this).data('IPAddress'));
                                        $('#txtPrinterFowstID').val($(this).data('PrinterFowstID'));


                                        //$.mobile.changePage('#pageOrgFOWSTPrinterIP');
                                        $('#pageOrgFOWSTInfo').hide();
                                        $('#pageOrgFOWSTTruckRegInfo').hide();
                                        $('#pageOrgFOWSTSectionInfo').hide();
                                        $('#pageOrgFOWSTBusinessPermitInfo').hide();
                                        $('#pageOrgFOWSTTelInfo').hide();
                                        $('#pageOrgFOWSTMobileInfo').hide();
                                        $('#pageOrgFOWSTEmailInfo').hide();
                                        $('#pageOrgFOWSTOperationHRInfo').hide();
                                        $('#pageOrgFOWSTPrinterInfo').hide();
                                        $('#pageOrgFOWSTPrinterIP').show(); $('#pageAddDevicePrinter').hide();

                                    })
                                )                            
                        };
                    } else {
                        $("#divDevicePrinterResults").show();
                        $("#divDevicePrinterRecordCount").text(" Number of record(s) found: " + vRows.length);
                        //                        $('#txtPrinterRole').hide();

                        for (var p = 0; p <= vRows.length - 1; p++) {
                            pItem = $('<a href="" class="list-group-item list-group-item-action">')
                                .data('ID', vRows[p].ID)
                                .data('PrinterRole', vRows[p].PrinterRole)
                                .attr('ID', vRows[p].ID)
                                .click(function (e) {
                                    e.preventDefault();
                                    $('#txtPrinterRoleID').val($(this).data('ID'));
                                    $("#txtPrinterID").val('');
                                    $('#txtPrinterIPAddress').val('');
                                    $('#txtPrinterFowstID').val('');
                                    $('#txtPrinterName').val('');
                                    $('#txtPrinterRole').text($(this).data('PrinterRole'));

                                    //$.mobile.changePage('#pageOrgFOWSTPrinterIP');
                                    $('#pageOrgFOWSTInfo').hide();
                                    $('#pageOrgFOWSTTruckRegInfo').hide();
                                    $('#pageOrgFOWSTSectionInfo').hide();
                                    $('#pageOrgFOWSTBusinessPermitInfo').hide();
                                    $('#pageOrgFOWSTTelInfo').hide();
                                    $('#pageOrgFOWSTMobileInfo').hide();
                                    $('#pageOrgFOWSTEmailInfo').hide();
                                    $('#pageOrgFOWSTOperationHRInfo').hide();
                                    $('#pageOrgFOWSTPrinterInfo').hide();
                                    $('#pageOrgFOWSTPrinterIP').show(); $('#pageAddDevicePrinter').hide();

                                })
                                .append($('<h5>')
                                    .attr("style", "white-space:normal")
                                    .text(vRows[p].PrinterRole)
                                )
                                .append($('<div>')
                                    .attr("style", "white-space:normal")
                                    .text(vRows[p].PrinterType)
                                )
                            pLV.append(pItem);
                        };

                    }

                    pLV;//.listview('refresh');
                } else {
                    $("#divDevicePrinterSearchNoResults").show();
                    $("#lstOrgFOWSTPrinter").hide();
                }

            } //end function

            function SaveFOWSTPrinterIP() {

                if (fowstID != '') {
                    AjaxSend({
                        url: 'DevicePrinter/CheckUniquePrinterName',
                        data: { PrinterName: $.trim($("#txtPrinterName").val()), PrinterFowstID: $("#txtPrinterFowstID").val(), FowstID: fowstID },
                        success: function (vData) {
                            if (vData.DevicePrinterData.Rows.length > 0) {
                                NewMessageBox('There is already an existing record with the same printer name.', 'Printer Name');
                                return;
                            } else {
                                AjaxSend({
                                    url: 'DevicePrinter/SaveFOWSTPrinterIP',
                                    data: {
                                        PrinterName: $.trim($("#txtPrinterName").val()),
                                        PrinterFowstID: $("#txtPrinterFowstID").val(),
                                        PrinterID: $("#txtPrinterRoleID").val(),
                                        PrinterIP: $.trim($("#txtPrinterIPAddress").val()),
                                        ProductDescriptionInReceipt: $.trim($("#selProductDescriptionInReceipt").val()),
                                        FowstID: fowstID,
                                        ConnectionType: $.trim($("#selConnectionType").val()),
                                        IsDefaultDispatchPrinter: IsDefaultDispatchPrinter
                                    },
                                    success: function (vData) {
                                        if (vData.DevicePrinterData.length > 0) {

                                            $('#txtPrinterIPAddress').val('');
                                            $('#txtPrinterFowstID').val('');

                                            //$.mobile.changePage('#pageOrgFOWSTInfo');
                                            $('#pageOrgFOWSTInfo').show();
                                            $('#pageOrgFOWSTTruckRegInfo').hide();
                                            $('#pageOrgFOWSTSectionInfo').hide();
                                            $('#pageOrgFOWSTBusinessPermitInfo').hide();
                                            $('#pageOrgFOWSTTelInfo').hide();
                                            $('#pageOrgFOWSTMobileInfo').hide();
                                            $('#pageOrgFOWSTEmailInfo').hide();
                                            $('#pageOrgFOWSTOperationHRInfo').hide();
                                            $('#pageOrgFOWSTPrinterInfo').hide();
                                            $('#pageOrgFOWSTPrinterIP').hide(); $('#pageAddDevicePrinter').hide();

                                            GetAllFOWSTPrinter();
                                        }
                                    }
                                });
                            }
                        }
                    });
                } else {
                    arOrgAddPrinter = new Array();
                    arOrgAddPrinter[intOrgFowstPrinterID] = $("#txtPrinterFowstID").val();
                    arOrgAddPrinter[intOrgFowstPrinterName] = $("#txtPrinterName").val();
                    arOrgAddPrinter[intOrgFowstPrinterRole] = $('#txtPrinterRole').text();
                    arOrgAddPrinter[intOrgFowstPrinterRoleID] = $('#txtPrinterRoleID').val();
                    arOrgAddPrinter[intOrgFowstPrinterIPAddress] = $("#txtPrinterIPAddress").val();
                    arOrgAddPrinter[intOrgFowstPrinterState] = (IsNewPrinter ? "New" : "Edit");

                    sessionStorage["OrgAddPrinter-" + intOrgFowstPrinterIndex] = JSON.stringify(arOrgAddPrinter);

                    //$.mobile.changePage('#pageOrgFOWSTInfo');
                    $('#pageOrgFOWSTInfo').show();
                    $('#pageOrgFOWSTTruckRegInfo').hide();
                    $('#pageOrgFOWSTSectionInfo').hide();
                    $('#pageOrgFOWSTBusinessPermitInfo').hide();
                    $('#pageOrgFOWSTTelInfo').hide();
                    $('#pageOrgFOWSTMobileInfo').hide();
                    $('#pageOrgFOWSTEmailInfo').hide();
                    $('#pageOrgFOWSTOperationHRInfo').hide();
                    $('#pageOrgFOWSTPrinterInfo').hide();
                    $('#pageOrgFOWSTPrinterIP').hide(); $('#pageAddDevicePrinter').hide();

                    LoadOrgPrinterList();


                }


            }

            function LoadOrgPrinterList() {
                var pLV = $('#lstOrgFOWSTPrinter');
                var pItem;

                pLV.html('');
                for (var i = 0; i <= intOrgFowstPrinterCntr; i++) {

                    if (sessionStorage["OrgAddPrinter-" + i] != undefined) {

                        arOrgAddPrinter = new Array();
                        arOrgAddPrinter = JSON.parse(sessionStorage["OrgAddPrinter-" + i]);

                        if (arOrgAddPrinter[intOrgFowstPrinterState] != "Remove") {
                            pItem = $('<a href="#" class="list-group-item list-group-item-action">')
                                    .attr('id', 'lst' + i)
                                    .append($('<table style="width:100%;">')
                                        .append($('<tr>')
                                            .append($('<td style="word-break:break-all;">')
                                                .attr('ID', i)
                                                .data('Data', arOrgAddPrinter)
                                                .append($('<h5>')
                                                    .attr("style", "white-space:normal")
                                                    .text(arOrgAddPrinter[intOrgFowstPrinterName])
                                                )
                                                .append($('<div>')
                                                    .attr("style", "white-space:normal")
                                                    .text(arOrgAddPrinter[intOrgFowstPrinterRole])
                                                )
                                                .append($('<div>')
                                                    .attr("style", "white-space:normal")
                                                    .text(arOrgAddPrinter[intOrgFowstPrinterIPAddress])
                                                )
                                                .click(function (e) {
                                                    e.preventDefault();
                                                    ShowOrgPrinterInfo($(this).data('Data'), $(this).attr('ID'));
                                                })
                                            )
                                        )
                                    )

                            pLV.append(pItem);
                        }
                    }
                }

                pLV;//.listview('refresh');
            };
            function ShowOrgPrinterInfo(arData, index) {

                //$.mobile.changePage('#pageOrgFOWSTPrinterIP');
                $('#pageOrgFOWSTInfo').hide();
                $('#pageOrgFOWSTTruckRegInfo').hide();
                $('#pageOrgFOWSTSectionInfo').hide();
                $('#pageOrgFOWSTBusinessPermitInfo').hide();
                $('#pageOrgFOWSTTelInfo').hide();
                $('#pageOrgFOWSTMobileInfo').hide();
                $('#pageOrgFOWSTEmailInfo').hide();
                $('#pageOrgFOWSTOperationHRInfo').hide();
                $('#pageOrgFOWSTPrinterInfo').hide();
                $('#pageOrgFOWSTPrinterIP').show(); $('#pageAddDevicePrinter').hide();

                ClearOrgAddPrinterFields();

                intOrgFowstPrinterIndex = index;

                $("#txtPrinterFowstID").val(arData[intOrgFowstPrinterID] || '');

                $("#txtPrinterName").val(arData[intOrgFowstPrinterName] || '');
                $("#txtPrinterRole").text(arData[intOrgFowstPrinterRole] || '');
                $("#txtPrinterRoleID").val(arData[intOrgFowstPrinterRoleID] || '');
                $("#txtPrinterIPAddress").val(arData[intOrgFowstPrinterIPAddress] || '');


                IsNewPrinter = false;

                $("#btnOrgFOWSTPrinterRemove").show();


            };



            //////////////////

            function CreateNewDevicePrinterEntryOrg(IsNew, strID) {
                var pData = {};
                pData.IsNew = IsNew;
                pData.ID = strID;

                window.pageOrig = $.mobile.activePage;

                AjaxSend({
                    url: 'DevicePrinter/setCreateNew',
                    data: pData,
                    success: function (vData) {

                        if ((vData.IsNew || '') == "TRUE") {

                            //   ChangePage('/DevicePrinter/DevicePrinterInfo');
                            //window.location = MergeUrlParameters({
                            //    OrgID: sessionStorage.OrgID,
                            //    IsMobile: CBool(IsMobile) ? 1 : 0
                            //}, '/DevicePrinter/DevicePrinterInfo');

                            AjaxSend({
                                ExactUrl: '/DevicePrinter/DevicePrinterInfo?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile,
                                type: 'GET',
                                dataType: 'text',
                                success: function (vData) {
                                    $('#pageAddDevicePrinter').empty();
                                    $('#pageAddDevicePrinter').append(vData).trigger('create');

                                    setTimeout(function () {
                                        if ($('#tblCategories tr').length == 0) {
                                            var pData = {};
                                            pData.IsNew = 1;
                                            pData.ID = '';

                                            AjaxSend({
                                                url: 'DevicePrinter/setCreateNew',
                                                data: pData,
                                                success: function (vData) {

                                                    if ((vData.IsNew || '') == "TRUE") {
                                                        AjaxSend({
                                                            url: 'DevicePrinter/IsNewEntry',
                                                            success: function (vData) {
                                                                if (vData.IsNew == 'TRUE') {

                                                                    $("#divCategories").show();
                                                                    GetAllCategories('');

                                                                }
                                                                else {
                                                                    ShowDevicePrinterInfo(vData.DevicePrinterID || '');
                                                                }
                                                            }
                                                        });
                                                    }
                                                    else {
                                                        if (GetUrlParameters().mode == 'wizard') {
                                                            window.open(GetPathFormat("/DevicePrinter/DevicePrinterSummary?mode=wizard"), "_self");
                                                        } else
                                                            window.open(GetPathFormat("/DevicePrinter/DevicePrinterSummary"), "_self");
                                                    }
                                                }
                                            });
                                        }
                                    }, 1000)
                                    App.Deferred.fulfill('FQ26E5TP76');
                                }
                            });
                        }
                        else {
                            window.open(GetPathFormat("/DevicePrinter/DevicePrinterSummary"), "_self");
                        }

                    }
                });

            }


            function validateIP(string) {
                var re = /^(\d|[1-9]\d|1\d\d|2([0-4]\d|5[0-5]))\.(\d|[1-9]\d|1\d\d|2([0-4]\d|5[0-5]))\.(\d|[1-9]\d|1\d\d|2([0-4]\d|5[0-5]))\.(\d|[1-9]\d|1\d\d|2([0-4]\d|5[0-5]))$/;

                return re.test(string);
            }

            function GetAllCategories(printerID) {
                data = { OrgID: sessionStorage.OrgID };

                AjaxSend({
                    url: 'DevicePrinter/GetAllCategories',
                    data: data,
                    success: function (vData) {
                        if (vData.Results.Rows.length > 0) {

                            if (printerID != '') {
                                AjaxSend({
                                    url: 'DevicePrinter/GetDevicePrinterCategories',
                                    data: { PrinterID: printerID },
                                    success: function (vDataCat) {

                                        if (vDataCat.Results.Rows.length > 0) {

                                            DisplayAllCategories(vData.Results.Rows, vDataCat.Results.Rows, printerID);
                                        } else {
                                            DisplayAllCategories(vData.Results.Rows, '', printerID);
                                        }
                                    }
                                });
                            } else {
                                DisplayAllCategories(vData.Results.Rows, '', '');
                            }

                        }
                    }
                });
            }

            var arrCategory = [];

            function DisplayAllCategories(vRows, vRowsCat, printerID) {
                var pLV = $("#tblCategories");

                $('#tblCategories').empty();

                var m_Ctr = 0;
                var categories = [];

                if (vRowsCat != '') {
                    for (var j = 0; j <= vRowsCat.length - 1; j++) {
                        categories[j] = vRowsCat[j].CategoryName;
                    }

                }


                for (var i = 0; i <= vRows.length - 1; i++) {

                    var checked = undefined;

                    if ((i % 2) == 0) {
                        pItem = $('<tr>')
                    }

                    if (categories.indexOf(vRows[i].Category) >= 0) {
                        checked = true;
                        pItem = pItem
                        .append($('<td>')
                            .append($('<input type="checkbox" />')
                                .attr('ID', i + 'chb')
                                .attr('checked', checked)
                                .attr('data-mini', 'true')
                                .data('data', vRows[i].Category)
                                .click(function () {
                                    if ($(this).is(':checked')) {
                                        AjaxSend({
                                            url: 'DevicePrinter/IsCategoryExist',
                                            data: { PrinterID: printerID, CategoryName: $(this).data('data') },
                                            success: function (vDataCat) {

                                                if (vDataCat.Results.Rows.length > 0) {
                                                    NewMessageBox('This category already belongs to a different printer role. Saving this will overwrite the previous setting.', 'Printer Role');

                                                }
                                            }
                                        });
                                    }
                                })

                            )
                            .append($('<label>')
                                .attr('for', i + 'chb')
                                .text(vRows[i].Category)
                            )
                        )//td                    


                        pLV.append(pItem);

                        try {
                            $('#' + i + 'chb').checkboxradio('refresh');
                        } catch (ex) {
                            $('#' + i + 'chb').checkboxradio();
                        }
                    } else {
                        pItem = pItem
                        .append($('<td>')
                            .append($('<input type="checkbox" />')
                                .attr('ID', i + 'chb')
                                .attr('checked', checked)
                                .attr('data-mini', 'true')
                                .data('data', vRows[i].Category)
                                .click(function () {
                                    if ($(this).is(':checked')) {
                                        AjaxSend({
                                            url: 'DevicePrinter/IsCategoryExist',
                                            data: { PrinterID: printerID, CategoryName: $(this).data('data') },
                                            success: function (vDataCat) {

                                                if (vDataCat.Results.Rows.length > 0) {
                                                    NewMessageBox('This category already belongs to a different printer role. Saving this will overwrite the previous setting.', 'Printer Role');

                                                }
                                            }
                                        });
                                    }
                                })

                            )
                            .append($('<label>')
                                .attr('for', i + 'chb')
                                .text(vRows[i].Category)
                            )
                        )//td                    


                        pLV.append(pItem);

                        try {
                            $('#' + i + 'chb').checkboxradio('refresh');
                        } catch (ex) {
                            $('#' + i + 'chb').checkboxradio();
                        }
                    }

                } //for

                try {
                    $('#lstCategoriesContainer');//.listview('refresh');
                }
                catch (e) {
                    $('#lstCategoriesContainer');//.listview();
                }
            }

            $('#formProductMapping').submit(function (e) {
                e.preventDefault();

                //grab form data
                var formData = new FormData($(this)[0]);
                mUploaderKey = NewID();
                formData.delete('OrgID'); //idunno ngano dili makuha ang value sa OrgID sa form, maong giappend nalang.
                formData.append('FowstID', fowstID);
                formData.append('UploaderKey', mUploaderKey);
                formData.append('OrgID', sessionStorage.OrgID);
                formData.append('selParseProductName', CInt2($('#selParseProductName').is(':checked')));
                formData.append('selDoNotOverwriteBomLandedCost', CInt2($('#selDoNotOverwriteBomLandedCost').is(':checked')));
                    
                $("#OrgID").val(sessionStorage.OrgID);

                Bs.Loading('show', 'Please wait ...');

                setTimeout(function () {
                    $.ajax({
                        url: '/ProductMapping/Index',
                        type: 'POST',
                        data: formData,
                        async: false,
                        cache: false,
                        contentType: false,
                        processData: false,
                        success: function (vData) {
                            vData = JSON.parse(vData);
                            if (vData.d.Error)
                                $('#productMappingResult').text('Product mapping failed. ' + vData.d.Error);
                            else if (vData.d.Result == 'ok') {
                                $('#productMappingResult').text('Product mapping was successful.');
                                if (vData.d.Warning)
                                    $('#productMappingResult').html('Product mapping was successful with some warnings. ' + HtmlEncode(vData.d.Warning).replace(/\r\n|\r|\n/g, "<br />"));
                                else
                                    $('#productMappingResult').text('Product mapping was successful.');
                            } else
                                $('#productMappingResult').text('Unexpected response. ref: js5H46SJ5ZNA!');
                        },
                        complete: function() {
                            Bs.Loading('hide');
                        }
                    });
                }, 1000)                    
            });

            $('#btnPIMUploadDownload').click(function() {
                Bs.Loading('show', 'Please wait ...');
                AjaxSend({
                    url: 'ProductMapping/CreateExcel',
                    data: { FowstID: fowstID },
                    success: function (vData) {
                        if (vData.Error)
                            NewMessageBox(vData.Error);
                        else if (vData.Result)
                            window.open('download/' + vData.Result, "_blank");
                    },
                    complete: function() {
                        Bs.Loading('hide');
                    }
                });
            });
           
        });
    });
