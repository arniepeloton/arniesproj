require([
    'common'
    ], function () {

        $(function () {

            var IsNewOrgAddLTO = true;
            var arOrgAddLTO = new Array();
            var intOrgFOWSTLTOCntr = 0;
            var intOrgFOWSTLTOIndex = 0;

            var intOrgFOWSTLTORecID = 0;
            var intOrgFOWSTLTOORNo = 1;
            var intOrgFOWSTLTODateIssuedMonth = 2;
            var intOrgFOWSTLTODateIssuedDay = 3;
            var intOrgFOWSTLTODateIssuedYear = 4;
            var intOrgFOWSTLTODateExpirationMonth = 5;
            var intOrgFOWSTLTODateExpirationDay = 6;
            var intOrgFOWSTLTODateExpirationYear = 7;
            var intOrgFOWSTLTOState = 8;

            var IsNewOrgAddLTFRB = true;
            var arOrgAddLTFRB = new Array();
            var intOrgFOWSTLTFRBCntr = 0;
            var intOrgFOWSTLTFRBIndex = 0;

            var intOrgFOWSTLTFRBRecID = 0;
            var intOrgFOWSTLTFRBORNo = 1;
            var intOrgFOWSTLTFRBDateIssuedMonth = 2;
            var intOrgFOWSTLTFRBDateIssuedDay = 3;
            var intOrgFOWSTLTFRBDateIssuedYear = 4;
            var intOrgFOWSTLTFRBDateExpirationMonth = 5;
            var intOrgFOWSTLTFRBDateExpirationDay = 6;
            var intOrgFOWSTLTFRBDateExpirationYear = 7;
            var intOrgFOWSTLTFRBState = 8;

            var IsNewOrgAddSection = true;
            var arOrgAddSection = new Array();
            var intOrgFOWSTSectionCntr = 0;
            var intOrgFOWSTSectionIndex = 0;

            var intOrgFOWSTSecRecID = 0;
            var intOrgFOWSTSecCode = 1;
            var intOrgFOWSTSecName = 2;
            var intOrgFOWSTSecDescription = 3;
            var intOrgFOWSTSecFloorArea = 4;
            var intOrgFOWSTSecSpecialFeatures = 5;
            var intOrgFOWSTSecState = 6;

            var IsNewOrgAddBPermit = true;
            var arOrgAddBPermit = new Array();
            var intOrgFOWSTBPermitCntr = 0;
            var intOrgFOWSTBPermitIndex = 0;

            var intOrgFOWSTBPermitRecID = 0;
            var intOrgFOWSTBPermitNo = 1;
            var intOrgFOWSTBPermitBusinessID = 2;
            var intOrgFOWSTBPermitTradeName = 3;
            var intOrgFOWSTBPermitBuildingHouseNo = 4;
            var intOrgFOWSTBPermitStreet = 5;
            var intOrgFOWSTBPermitCityID = 6;
            var intOrgFOWSTBPermitCity = 7;
            var intOrgFOWSTBPermitCountryID = 8;
            var intOrgFOWSTBPermitCountry = 9;
            var intOrgFOWSTBPermitORNo = 10;
            var intOrgFOWSTBPermitBusinessPlateNo = 11;
            var intOrgFOWSTBPermitDatePaymentMonth = 12;
            var intOrgFOWSTBPermitDatePaymentDay = 13;
            var intOrgFOWSTBPermitDatePaymentYear = 14;
            var intOrgFOWSTBPermitDateIssuedMonth = 15;
            var intOrgFOWSTBPermitDateIssuedDay = 16;
            var intOrgFOWSTBPermitDateIssuedYear = 17;
            var intOrgFOWSTBPermitState = 18;

            var IsNewOrgAddTelContact = true;
            var arOrgAddTel = new Array();
            var intOrgFOWSTTelCntr = 0;
            var intOrgFOWSTTelIndex = 0;

            var intOrgFOWSTTelRecID = 0;
            var intOrgFOWSTTelCountryID = 1;
            var intOrgFOWSTTelCountryCode = 2;
            var intOrgFOWSTTelCallingCode = 3;
            var intOrgFOWSTTelAreaCodeID = 4;
            var intOrgFOWSTTelArea = 5;
            var intOrgFOWSTTelAreaCode = 6;
            var intOrgFOWSTTelContactNo = 7;
            var intOrgFOWSTTelExtNo = 8;
            var intOrgFOWSTTelContactType = 9;
            var intOrgFOWSTTelState = 10;

            var IsNewOrgAddMobileContact = true;
            var arOrgAddMobile = new Array();
            var intOrgFOWSTMobileCntr = 0;
            var intOrgFOWSTMobileIndex = 0;

            var intOrgFOWSTMobileRecID = 0;
            var intOrgFOWSTMobileCountryID = 1;
            var intOrgFOWSTMobileCountryCode = 2;
            var intOrgFOWSTMobileCallingCode = 3;
            var intOrgFOWSTMobileTelCoPrefixID = 4;
            var intOrgFOWSTMobilePrefix = 5;
            var intOrgFOWSTMobileTelCo = 6;
            var intOrgFOWSTMobileContactNo = 7;
            var intOrgFOWSTMobileContactType = 8;
            var intOrgFOWSTMobileState = 9;

            var IsNewOrgAddEmailContact = true;
            var arOrgAddEmail = new Array();
            var intOrgFOWSTEmailCntr = 0;
            var intOrgFOWSTEmailIndex = 0;

            var intOrgFOWSTEmailRecID = 0;
            var intOrgFOWSTEmailAddress = 1;
            var intOrgFOWSTEmailType = 2;
            var intOrgFOWSTEmailState = 3;

            var intOrgFOWSTIsLTO = true;
            var mLoaded = false;
            var isIE = false || !!document.documentMode;

            var libname, lbllibname, addlibname, orgid;

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

            getOrgAddPlantAndPropertyType();
            getOrgAddWarehouseType();
            getOrgAddRetailStoreType();

            $.mobile.activePage.bind('pageshow', PageShow);
            function PageShow() {
                mLoaded = true;
                lbllibname = sessionStorage.LabelLibraryName;
                libname = sessionStorage.LibraryName;
                addlibname = sessionStorage.AddViewLibBCName;
                orgid = sessionStorage.ViewID;
                getOrgFOWSTList();
            };
            if (!mLoaded) PageShow();

            window.OrgShortAdd = {
                ShowOrgAdd: function () {
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

                    getOrgAddPlantAndPropertyType();
                    getOrgAddWarehouseType();
                    getOrgAddRetailStoreType();

                    lbllibname = sessionStorage.LabelLibraryName;
                    libname = sessionStorage.LibraryName;
                    addlibname = sessionStorage.AddViewLibBCName;
                    orgid = sessionStorage.ViewID;

                    getOrgFOWSTList();
                }
            }

            $('body').on('click', '#btnOrgFOWSTAdd', function () {
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
                $('#pageOrgFOWSTPrinterIP').hide(); $('#pageAddDevicePrinter').hide(); $('#pageOrgFOWSTList').hide();
                ClearOrgFOWSTFields();
            })
            .on('change', '#selOrgFOWSTType', function () {
                SetOrgAddSubFOWSTType();
            })
            $('body').on('click', '#btnOrgFOWSTCity', function () {
                $('#TitleLibrary').text('CITY');
                $('#txtSearchKeyword').val('').attr('placeholder', 'Type name of City');
                $('#panSearchLibrary').popup('open');
                $('#lstOfLibrary').empty();
                NameOfBtnForLibrary = 'CompleteAddress';
            })
            .on('click', '#btnOrgFOWSTCountry', function () {
                $('#TitleLibrary').text('COUNTRY');
                $('#txtSearchKeyword').val('').attr('placeholder', 'Type name of Country');
                $('#panSearchLibrary').popup('open');
                $('#lstOfLibrary').empty();
                NameOfBtnForLibrary = 'CompleteAddress';
            })
            .on('change', '#selOrgAddFranchisee', function () {
                ShowOrgAddHideFranchisorDetails();
            })
            .on('click', '#btnOrgFOWSTFranchisor', function () {
                //filter to show only franchisors (must be a subscriber!)
                $('#TitleLibrary').text('FRANCHISOR');
                $('#txtSearchKeyword').val('').attr('placeholder', 'Type name of Franchisor');
                $('#panSearchLibrary').popup('open');
                $('#lstOfLibrary').empty();
            })
            .on('change', '#selOrgFOWSTOwnershipType', function () {
                SetOrgAddShowHideRentedInfo();
            })
            .on('change', '#selOrgFOWSTContractStartMonth', function () {
                setDayCollection('selOrgFOWSTContractStartMonth', 'selOrgFOWSTContractStartDay', 'selOrgFOWSTContractStartYear', true);
            })
            .on('change', '#selOrgFOWSTContractStartYear', function () {
                setDayCollection('selOrgFOWSTContractStartMonth', 'selOrgFOWSTContractStartDay', 'selOrgFOWSTContractStartYear', true);
            })
            .on('click', '#selOrgFOWSTContractStartYear', function () {
                if ($.trim($('#selOrgFOWSTContractStartYear').val()) == "") {
                    $('#selOrgFOWSTContractStartYear').val('2012');
                    $('#selOrgFOWSTContractStartYear').selectmenu('refresh');
                }
            })
            .on('change', '#selOrgFOWSTContractEndMonth', function () {
                setDayCollection('selOrgFOWSTContractEndMonth', 'selOrgFOWSTContractEndDay', 'selOrgFOWSTContractEndYear', true);
            })
            .on('change', '#selOrgFOWSTContractEndYear', function () {
                setDayCollection('selOrgFOWSTContractEndMonth', 'selOrgFOWSTContractEndDay', 'selOrgFOWSTContractEndYear', true);
            })
            .on('click', '#selOrgFOWSTContractEndYear', function () {
                if ($.trim($('#selOrgFOWSTContractEndYear').val()) == "") {
                    $('#selOrgFOWSTContractEndYear').val('2012');
                    $('#selOrgFOWSTContractEndYear').selectmenu('refresh');
                }
            })
            .on('click', '#btnOrgFOWSTSave', function () {
                if ($.trim($("#selOrgFOWSTType").val()) == "") {
                    $('#TitleOrgEmp').text('Street');
                    $('#MsgOrgEmp').text('Please select property and plant type.');
                    $('#panOrgEmpMessage').popup('open');
                    $('#btnOKOrgEmp').text('Ok').click(function (e) {
                        e.preventDefault();
                        $('#panOrgEmpMessage').popup('close');
                    })
                    $('#divBtnCancel').text('Cancel').hide();
                    return;
                }

                if ($.trim($("#txtOrgFOWSTName").val()) == "") {
                    $('#TitleOrgEmp').text('District');
                    $('#MsgOrgEmp').text('Please enter property and plant name.');
                    $('#panOrgEmpMessage').popup('open');
                    $('#btnOKOrgEmp').text('Ok').click(function (e) {
                        e.preventDefault();
                        $('#panOrgEmpMessage').popup('close');
                    })
                    $('#divBtnCancel').text('Cancel').hide();
                    return;
                }

                if ($.trim($("#txtOrgFOWSTStreet").val()) == "") {
                    $('#TitleOrgEmp').text('Street');
                    $('#MsgOrgEmp').text('Please enter street address.');
                    $('#panOrgEmpMessage').popup('open');
                    $('#btnOKOrgEmp').text('Ok').click(function (e) {
                        e.preventDefault();
                        $('#panOrgEmpMessage').popup('close');
                    })
                    $('#divBtnCancel').hide();
                    return;
                }

                if ($.trim($("#txtOrgFOWSTCityID").val()) == "") {
                    $('#TitleOrgEmp').text('City');
                    $('#MsgOrgEmp').text('Please select city address.');
                    $('#panOrgEmpMessage').popup('open');
                    $('#btnOKOrgEmp').text('Ok').click(function (e) {
                        e.preventDefault();
                        $('#panOrgEmpMessage').popup('close');
                    })
                    $('#divBtnCancel').hide();
                    return;
                }

                if ($.trim($("#txtOrgFOWSTCountryID").val()) == "") {
                    $('#TitleOrgEmp').text('Country');
                    $('#MsgOrgEmp').text('Please enter country address.');
                    $('#panOrgEmpMessage').popup('open');
                    $('#btnOKOrgEmp').text('Ok').click(function (e) {
                        e.preventDefault();
                        $('#panOrgEmpMessage').popup('close');
                    })
                    $('#divBtnCancel').hide();
                    return;
                }

                //Franchisee
                if ($.trim($("#selOrgAddFranchisee").val()) == "1") {

                    if ($.trim($("#txtOrgFOWSTFranchisorID").val()) == "") {
                        $('#TitleOrgEmp').text('Franchisor');
                        $('#MsgOrgEmp').text('Please select franchisor.');
                        $('#panOrgEmpMessage').popup('open');
                        $('#btnOKOrgEmp').text('Ok').click(function (e) {
                            e.preventDefault();
                            $('#panOrgEmpMessage').popup('close');
                        })
                        $('#divBtnCancel').hide();
                        return;
                    }

                }
                else {
                    $("#txtOrgFOWSTFranchisorID").val('');
                    $("#btnOrgFOWSTFranchisor").text('Please Select').button('refresh');
                }

                //Ownership Type
                if ($.trim($("#selOrgFOWSTOwnershipType").val()) == "1") {

                    if ($.trim($("#selOrgFOWSTContractStartMonth").val()) == "" ||
                        $.trim($("#selOrgFOWSTContractStartDay").val()) == "" ||
                        $.trim($("#selOrgFOWSTContractStartYear").val()) == "") {

                        $('#TitleOrgEmp').text('Contract Start Date');
                        $('#MsgOrgEmp').text('Please set the rent or leased contract start date.');
                        $('#panOrgEmpMessage').popup('open');
                        $('#btnOKOrgEmp').text('Ok').click(function (e) {
                            e.preventDefault();
                            $('#panOrgEmpMessage').popup('close');
                        })
                        $('#divBtnCancel').hide();
                        return;
                    }

                    if ($.trim($("#selOrgFOWSTContractEndMonth").val()) == "" ||
                        $.trim($("#selOrgFOWSTContractEndDay").val()) == "" ||
                        $.trim($("#selOrgFOWSTContractEndYear").val()) == "") {
                        $('#TitleOrgEmp').text('Contract End Date');
                        $('#MsgOrgEmp').text('Please set the rent or leased contract end date.');
                        $('#panOrgEmpMessage').popup('open');
                        $('#btnOKOrgEmp').text('Ok').click(function (e) {
                            e.preventDefault();
                            $('#panOrgEmpMessage').popup('close');
                        })
                        $('#divBtnCancel').hide();
                        return;
                    }
                }
                SaveOrgFOWST();
            })
            .on('click', '#btnOrgFOWSTCancel', function () {
                //ChangePage('/OrganizationShort/OrganizationAddress');
                //window.location = MergeUrlParameters({
                //    OrgID: sessionStorage.OrgID,
                //    IsMobile: CBool(IsMobile) ? 1 : 0
                //}, '/OrganizationShort/OrganizationAddress');
                window.location.hash = 'LibraryPage';
            })
            .on('click', '#btnOrgFOWSTBPermitCity', function () {
                $('#TitleLibrary').text('CITY');
                $('#txtSearchKeyword').val('').attr('placeholder', 'Type name of City');
                $('#panSearchLibrary').popup('open');
                $('#lstOfLibrary').empty();
                NameOfBtnForLibrary = 'BusinessPermit';
            })
            .on('click', '#btnOrgFOWSTBPermitCountry', function () {
                $('#TitleLibrary').text('COUNTRY');
                $('#txtSearchKeyword').val('').attr('placeholder', 'Type name of Country');
                $('#panSearchLibrary').popup('open');
                $('#lstOfLibrary').empty();
                NameOfBtnForLibrary = 'BusinessPermit';
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
                $('#pageOrgFOWSTPrinterIP').hide(); $('#pageAddDevicePrinter').hide(); $('#pageOrgFOWSTList').hide();

                IsNewOrgAddLTO = true
                intOrgFOWSTLTOCntr = intOrgFowstLTOCntr + 1;
                intOrgFOWSTLTOIndex = intOrgFowstLTOCntr;

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
                    $('#selOrgFOWSTBPermitDateOfPaymentYear').selectmenu('refresh');
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
                    $('#selOrgFOWSTBPermitBPlateIssuedYear').selectmenu('refresh');
                }
            })
            .on('click', '#btnOrgFOWSTTelCountryCode', function () {
                $('#TitleLibrary').text('COUNTRY');
                $('#txtSearchKeyword').val('').attr('placeholder', 'Type name of Country');
                $('#panSearchLibrary').popup('open');
                $('#lstOfLibrary').empty();
                NameOfBtnForLibrary = 'Telephone';
            })
            .on('click', '#btnOrgFOWSTTelAreaCode', function () {
                $('#TitleLibrary').text('AREA CODE');
                $('#txtSearchKeyword').val('').attr('placeholder', 'Type name or code of area');
                $('#panSearchLibrary').popup('open');
                $('#lstOfLibrary').empty();
                NameOfBtnForLibrary = 'Telephone';
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
            .on('click', '#btnOrgFOWSTMobileCountryCode', function () {
                $('#TitleLibrary').text('COUNTRY');
                $('#txtSearchKeyword').val('').attr('placeholder', 'Type name of Country');
                $('#panSearchLibrary').popup('open');
                $('#lstOfLibrary').empty();
                NameOfBtnForLibrary = 'Mobile';
            })
            .on('click', '#btnOrgFOWSTMobileTelCoPrefixCode', function () {
                $('#TitleLibrary').text('TELCO CODE');
                $('#txtSearchKeyword').val('').attr('placeholder', 'Type name or prefix number of sim provider');
                $('#panSearchLibrary').popup('open');
                $('#lstOfLibrary').empty();
                NameOfBtnForLibrary = 'Mobile';
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
                $('#pageOrgFOWSTPrinterIP').hide(); $('#pageAddDevicePrinter').hide(); $('#pageOrgFOWSTList').hide();

                IsNewOrgAddLTFRB = true
                intOrgFOWSTLTFRBCntr = intOrgFowstLTFRBCntr + 1;
                intOrgFOWSTLTFRBIndex = intOrgFowstLTFRBCntr;

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
                $('#pageOrgFOWSTPrinterIP').hide(); $('#pageAddDevicePrinter').hide(); $('#pageOrgFOWSTList').hide();

                IsNewOrgAddSection = true
                intOrgFOWSTSectionCntr = intOrgFOWSTSectionCntr + 1;
                intOrgFOWSTSectionIndex = intOrgFOWSTSectionCntr;

                ClearOrgAddSectionFields();
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
                $('#pageOrgFOWSTPrinterIP').hide();
                $('#pageAddDevicePrinter').hide();
                $('#pageOrgFOWSTList').hide();
                $('body').animate({ scrollTop: 0 }, 'fast');

                IsNewOrgAddBPermit = true
                intOrgFOWSTBPermitCntr = intOrgFOWSTBPermitCntr + 1;
                intOrgFOWSTBPermitIndex = intOrgFOWSTBPermitCntr;

                ClearOrgAddBPermitFields();
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
                $('#pageOrgFOWSTPrinterIP').hide(); $('#pageAddDevicePrinter').hide(); $('#pageOrgFOWSTList').hide();
                $('body').animate({ scrollTop: 0 }, 'fast');

                IsNewOrgAddTelContact = true
                intOrgFOWSTTelCntr = intOrgFOWSTTelCntr + 1;
                intOrgFOWSTTelIndex = intOrgFOWSTTelCntr;

                ClearOrgAddTelFields();
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
                $('#pageOrgFOWSTPrinterIP').hide(); $('#pageAddDevicePrinter').hide(); $('#pageOrgFOWSTList').hide();
                $('body').animate({ scrollTop: 0 }, 'fast');

                IsNewOrgAddMobileContact = true
                intOrgFOWSTMobileCntr = intOrgFOWSTMobileCntr + 1;
                intOrgFOWSTMobileIndex = intOrgFOWSTMobileCntr;

                ClearOrgAddMobileFields();
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
                $('#pageOrgFOWSTPrinterIP').hide(); $('#pageAddDevicePrinter').hide(); $('#pageOrgFOWSTList').hide();
                $('body').animate({ scrollTop: 0 }, 'fast');

                IsNewOrgAddEmailContact = true
                intOrgFOWSTEmailCntr = intOrgFOWSTEmailCntr + 1;
                intOrgFOWSTEmailIndex = intOrgFOWSTEmailCntr;

                ClearOrgAddEmailFields();
            })
            .on('click', '#btnOrgFOWSTTRegSave', function () {
                if ($.trim($("#txtOrgFOWSTTRegORNo").val()) == "") {
                    $('#TitleOrgEmp').text('OR Number');
                    $('#MsgOrgEmp').text('Please enter OR number.');
                    $('#panOrgEmpMessage').popup('open');
                    $('#btnOKOrgEmp').text('Ok').click(function (e) {
                        e.preventDefault();
                        $('#panOrgEmpMessage').popup('close');
                    })
                    $('#divBtnCancel').hide();
                    return;
                }

                if ($.trim($("#selOrgFOWSTTRegIssuedMonth").val()) != "" ||
                    $.trim($("#selOrgFOWSTTRegIssuedDay").val()) != "" ||
                    $.trim($("#selOrgFOWSTTRegIssuedYear").val()) != "") {

                    if ($.trim($("#selOrgFOWSTTRegIssuedMonth").val()) == "" ||
                        $.trim($("#selOrgFOWSTTRegIssuedDay").val()) == "" ||
                        $.trim($("#selOrgFOWSTTRegIssuedYear").val()) == "") {

                        $('#TitleOrgEmp').text('Issued Date');
                        $('#MsgOrgEmp').text('Please enter valid issued date.');
                        $('#panOrgEmpMessage').popup('open');
                        $('#btnOKOrgEmp').text('Ok').click(function (e) {
                            e.preventDefault();
                            $('#panOrgEmpMessage').popup('close');
                        })
                        $('#divBtnCancel').hide();
                        return;
                    }

                }

                if ($.trim($("#selOrgFOWSTTRegExpirationMonth").val()) != "" ||
                    $.trim($("#selOrgFOWSTTRegExpirationDay").val()) != "" ||
                    $.trim($("#selOrgFOWSTTRegExpirationYear").val()) != "") {

                    if ($.trim($("#selOrgFOWSTTRegExpirationMonth").val()) == "" ||
                        $.trim($("#selOrgFOWSTTRegExpirationDay").val()) == "" ||
                        $.trim($("#selOrgFOWSTTRegExpirationYear").val()) == "") {

                        $('#TitleOrgEmp').text('Expiration Date');
                        $('#MsgOrgEmp').text('Please enter valid expriration date.');
                        $('#panOrgEmpMessage').popup('open');
                        $('#btnOKOrgEmp').text('Ok').click(function (e) {
                            e.preventDefault();
                            $('#panOrgEmpMessage').popup('close');
                        })
                        $('#divBtnCancel').hide();
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
                $('#TitleOrgEmp').text('Remove');
                $('#MsgOrgEmp').text('Remove this record?');
                $('#panOrgEmpMessage').popup('open');
                $('#btnOKOrgEmp').text('Yes').click(function (e) {
                    e.preventDefault();
                    if (intOrgFowstIsLTO) {
                        RemoveOrgFOWSTLTO();
                    }
                    else {
                        RemoveOrgFOWSTLTFRB();
                    }
                    $('#panOrgEmpMessage').popup('close');
                })
                $('#divBtnCancel').show();
                $('#btnCancelOrgEmp').text('No').show().click(function () {
                    $('#panOrgEmpMessage').popup('close');
                })
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
                $('#pageOrgFOWSTPrinterIP').hide(); $('#pageAddDevicePrinter').hide(); $('#pageOrgFOWSTList').hide();
            })
            .on('click', '#btnOrgFOWSTSecSave', function () {
                if ($.trim($("#txtOrgFOWSTSecName").val()) == "") {
                    $('#TitleOrgEmp').text('Section Name');
                    $('#MsgOrgEmp').text('Please enter section name.');
                    $('#panOrgEmpMessage').popup('open');
                    $('#btnOKOrgEmp').text('Ok').click(function (e) {
                        e.preventDefault();
                        $('#panOrgEmpMessage').popup('close');
                    })
                    $('#divBtnCancel').hide();
                    return;
                }

                SaveOrgFOWSTSection();
            })
            .on('click', '#btnOrgFOWSTSecRemove', function () {
                $('#TitleOrgEmp').text('Remove');
                $('#MsgOrgEmp').text('Remove this record?');
                $('#panOrgEmpMessage').popup('open');
                $('#btnOKOrgEmp').text('Yes').click(function (e) {
                    e.preventDefault();
                    RemoveOrgFOWSTSection();
                })
                $('#divBtnCancel').show();
                $('#btnCancelOrgEmp').text('No').show().click(function () {
                    $('#panOrgEmpMessage').popup('close');
                })
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
                $('#pageOrgFOWSTPrinterIP').hide(); $('#pageAddDevicePrinter').hide(); $('#pageOrgFOWSTList').hide();
            })
            .on('click', '#btnOrgFOWSTBPermitSave', function () {
                if ($.trim($("#txtOrgFOWSTBPermitNo").val()) == "") {
                    $('#TitleOrgEmp').text('Business Permit No.');
                    $('#MsgOrgEmp').text('Please enter business permit number.');
                    $('#panOrgEmpMessage').popup('open');
                    return;
                }

                if ($.trim($("#txtOrgFOWSTBPermitTradeName").val()) == "") {
                    $('#TitleOrgEmp').text('Trade Name');
                    $('#MsgOrgEmp').text('Please enter trade name.');
                    $('#panOrgEmpMessage').popup('open');
                    return;
                }

                if ($.trim($("#txtOrgFOWSTBPermitStreet").val()) == "") {
                    $('#TitleOrgEmp').text('Street Address');
                    $('#MsgOrgEmp').text('Please enter street address.');
                    $('#panOrgEmpMessage').popup('open');
                    return;
                }

                if ($.trim($("#txtOrgFOWSTBPermitCityID").val()) == "") {
                    $('#TitleOrgEmp').text('City Address');
                    $('#MsgOrgEmp').text('Please select city address.');
                    $('#panOrgEmpMessage').popup('open');
                    return;
                }

                if ($.trim($("#txtOrgFOWSTBPermitCountryID").val()) == "") {
                    $('#TitleOrgEmp').text('Country Address');
                    $('#MsgOrgEmp').text('Please select country address.');
                    $('#panOrgEmpMessage').popup('open');
                    return;
                }

                if ($.trim($("#txtOrgFOWSTBPermitBPlateNo").val()) == "") {
                    $('#TitleOrgEmp').text('Business Plate No.');
                    $('#MsgOrgEmp').text('Please enter business plate number.');
                    $('#panOrgEmpMessage').popup('open');
                    return;
                }

                if ($.trim($("#selOrgFOWSTBPermitDateOfPaymentMonth").val()) != "" ||
                    $.trim($("#selOrgFOWSTBPermitDateOfPaymentDay").val()) != "" ||
                    $.trim($("#selOrgFOWSTBPermitDateOfPaymentYear").val()) != "") {

                    if ($.trim($("#selOrgFOWSTBPermitDateOfPaymentMonth").val()) == "" ||
                        $.trim($("#selOrgFOWSTBPermitDateOfPaymentDay").val()) == "" ||
                        $.trim($("#selOrgFOWSTBPermitDateOfPaymentYear").val()) == "") {

                        $('#TitleOrgEmp').text('Date Of Payment');
                        $('#MsgOrgEmp').text('Please enter valid date of payment.');
                        $('#panOrgEmpMessage').popup('open');
                        return;
                    }

                }

                if ($.trim($("#selOrgFOWSTBPermitBPlateIssuedMonth").val()) != "" ||
                    $.trim($("#selOrgFOWSTBPermitBPlateIssuedDay").val()) != "" ||
                    $.trim($("#selOrgFOWSTBPermitBPlateIssuedYear").val()) != "") {

                    if ($.trim($("#selOrgFOWSTBPermitBPlateIssuedMonth").val()) == "" ||
                        $.trim($("#selOrgFOWSTBPermitBPlateIssuedDay").val()) == "" ||
                        $.trim($("#selOrgFOWSTBPermitBPlateIssuedYear").val()) == "") {

                        $('#TitleOrgEmp').text('Issued Date');
                        $('#MsgOrgEmp').text('Please enter valid issued date.');
                        $('#panOrgEmpMessage').popup('open');
                        return;
                    }

                }

                SaveOrgFOWSTBPermit();
            })
            .on('click', '#btnOrgFOWSTBPermitRemove', function () {
                $('#TitleOrgEmp').text('Remove');
                $('#MsgOrgEmp').text('Remove this record?');
                $('#panOrgEmpMessage').popup('open');
                $('#btnOKOrgEmp').text('Yes').click(function (e) {
                    e.preventDefault();
                    RemoveOrgFOWSTBPermit();
                })
                $('#divBtnCancel').show();
                $('#btnCancelOrgEmp').text('No');
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
                $('#pageOrgFOWSTPrinterIP').hide(); $('#pageAddDevicePrinter').hide(); $('#pageOrgFOWSTList').hide();
            })
            .on('click', '#btnOrgFOWSTTelSave', function () {
                if ($.trim($("#txtOrgFOWSTTelCountryID").val()) == "") {
                    $('#TitleOrgEmp').text('Country Code');
                    $('#MsgOrgEmp').text('Please select country code.');
                    $('#panOrgEmpMessage').popup('open');
                    $('#btnOKOrgEmp').text('Ok').click(function (e) {
                        e.preventDefault();
                        $('#panOrgEmpMessage').popup('close');
                    })
                    $('#divBtnCancel').hide();
                    return;
                }

                if ($.trim($("#txtOrgFOWSTTelAreaCodeID").val()) == "") {
                    $('#TitleOrgEmp').text('Area Code');
                    $('#MsgOrgEmp').text('Please select area code.');
                    $('#panOrgEmpMessage').popup('open');
                    $('#btnOKOrgEmp').text('Ok').click(function (e) {
                        e.preventDefault();
                        $('#panOrgEmpMessage').popup('close');
                    })
                    $('#divBtnCancel').hide();
                    return;
                }

                if ($.trim($("#txtOrgFOWSTTelNo").val()) == "") {
                    $('#TitleOrgEmp').text('Telephone Number');
                    $('#MsgOrgEmp').text('Please enter 7 digit telephone number.');
                    $('#panOrgEmpMessage').popup('open');
                    $('#btnOKOrgEmp').text('Ok').click(function (e) {
                        e.preventDefault();
                        $('#panOrgEmpMessage').popup('close');
                    })
                    $('#divBtnCancel').hide();
                    return;
                }

                if ($.trim($("#txtOrgFOWSTTelNo").val()).length != 7) {
                    $('#TitleOrgEmp').text('Telephone Number');
                    $('#MsgOrgEmp').text('Please enter 7 digit telephone number.');
                    $('#panOrgEmpMessage').popup('open');
                    $('#btnOKOrgEmp').text('Ok').click(function (e) {
                        e.preventDefault();
                        $('#panOrgEmpMessage').popup('close');
                    })
                    $('#divBtnCancel').hide();
                    return;
                }

                if (!$.isNumeric($("#txtOrgFOWSTTelNo").val())) {
                    $('#TitleOrgEmp').text('Telephone Number');
                    $('#MsgOrgEmp').text('Please enter valid telephone number.');
                    $('#panOrgEmpMessage').popup('open');
                    $('#btnOKOrgEmp').text('Ok').click(function (e) {
                        e.preventDefault();
                        $('#panOrgEmpMessage').popup('close');
                    })
                    $('#divBtnCancel').hide();
                    return;
                }

                if ($.trim($("#txtOrgFOWSTTelExtension").val()).length > 0) {
                    if (!$.isNumeric($("#txtOrgFOWSTTelExtension").val())) {
                        $('#TitleOrgEmp').text('Telephone Number');
                        $('#MsgOrgEmp').text('Please enter valid extension or local number.');
                        $('#panOrgEmpMessage').popup('open');
                        $('#btnOKOrgEmp').text('Ok').click(function (e) {
                            e.preventDefault();
                            $('#panOrgEmpMessage').popup('close');
                        })
                        $('#divBtnCancel').hide();
                        return;
                    }
                }

                if ($.trim($("#selOrgFOWSTTelType").val()) == "") {
                    $('#TitleOrgEmp').text('Telephone Contact Type');
                    $('#MsgOrgEmp').text('Please select telephone contact type.');
                    $('#panOrgEmpMessage').popup('open');
                    $('#btnOKOrgEmp').text('Ok').click(function (e) {
                        e.preventDefault();
                        $('#panOrgEmpMessage').popup('close');
                    })
                    $('#divBtnCancel').hide();
                    return;
                }

                SaveOrgFOWSTTel();
            })
            .on('click', '#btnOrgFOWSTTelRemove', function () {
                $('#TitleOrgEmp').text('Remove');
                $('#MsgOrgEmp').text('Remove this record?');
                $('#panOrgEmpMessage').popup('open');
                $('#btnOKOrgEmp').text('Yes').click(function (e) {
                    e.preventDefault();
                    RemoveOrgFOWSTTel();
                })
                $('#divBtnCancel').show();
                $('#btnCancelOrgEmp').text('No').show().click(function () {
                    $('#panOrgEmpMessage').popup('close');
                })
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
                $('#pageOrgFOWSTPrinterIP').hide(); $('#pageAddDevicePrinter').hide(); $('#pageOrgFOWSTList').hide();
            })
            .on('click', '#btnOrgFOWSTMobileSave', function () {
                if ($.trim($("#txtOrgFOWSTMobileCountryID").val()) == "") {
                    $('#TitleOrgEmp').text('Country Code');
                    $('#MsgOrgEmp').text('Please select country code.');
                    $('#panOrgEmpMessage').popup('open');
                    $('#btnOKOrgEmp').text('Ok').click(function (e) {
                        e.preventDefault();
                        $('#panOrgEmpMessage').popup('close');
                    })
                    $('#divBtnCancel').hide();
                    return;
                }

                if ($.trim($("#txtOrgFOWSTTelCoPrefixID").val()) == "") {
                    $('#TitleOrgEmp').text('Prefix Number');
                    $('#MsgOrgEmp').text('Please select prefix number.');
                    $('#panOrgEmpMessage').popup('open');
                    $('#btnOKOrgEmp').text('Ok').click(function (e) {
                        e.preventDefault();
                        $('#panOrgEmpMessage').popup('close');
                    })
                    $('#divBtnCancel').hide();
                    return;
                }

                if ($.trim($("#txtOrgFOWSTMobileNo").val()) == "") {
                    $('#TitleOrgEmp').text('Mobile Number');
                    $('#MsgOrgEmp').text('Please enter 7 digit mobile number.');
                    $('#panOrgEmpMessage').popup('open');
                    $('#btnOKOrgEmp').text('Ok').click(function (e) {
                        e.preventDefault();
                        $('#panOrgEmpMessage').popup('close');
                    })
                    $('#divBtnCancel').hide();
                    return;
                }

                if ($.trim($("#txtOrgFOWSTMobileNo").val()).length != 7) {
                    $('#TitleOrgEmp').text('Mobile No.');
                    $('#MsgOrgEmp').text('Please enter 7 digit mobile number.');
                    $('#panOrgEmpMessage').popup('open');
                    $('#btnOKOrgEmp').text('Ok').click(function (e) {
                        e.preventDefault();
                        $('#panOrgEmpMessage').popup('close');
                    })
                    $('#divBtnCancel').hide();
                    return;
                }

                if (!$.isNumeric($("#txtOrgFOWSTMobileNo").val())) {
                    $('#TitleOrgEmp').text('Mobile Number');
                    $('#MsgOrgEmp').text('Please enter valid mobile number.');
                    $('#panOrgEmpMessage').popup('open');
                    $('#btnOKOrgEmp').text('Ok').click(function (e) {
                        e.preventDefault();
                        $('#panOrgEmpMessage').popup('close');
                    })
                    $('#divBtnCancel').hide();
                    return;
                }

                if ($.trim($("#selOrgAddMobileType").val()) == "") {
                    $('#TitleOrgEmp').text('Contact Type');
                    $('#MsgOrgEmp').text('Please select mobile contact type.');
                    $('#panOrgEmpMessage').popup('open');
                    $('#btnOKOrgEmp').text('Ok').click(function (e) {
                        e.preventDefault();
                        $('#panOrgEmpMessage').popup('close');
                    })
                    $('#divBtnCancel').hide();
                    return;
                }

                SaveOrgFOWSTMobile();
            })
            .on('click', '#btnOrgFOWSTMobileRemove', function () {
                $('#TitleOrgEmp').text('Remove');
                $('#MsgOrgEmp').text('Remove this record?');
                $('#panOrgEmpMessage').popup('open');
                $('#btnOKOrgEmp').text('Yes').click(function (e) {
                    e.preventDefault();
                    RemoveOrgFOWSTMobile();
                })
                $('#divBtnCancel').show();
                $('#btnCancelOrgEmp').text('No').show().click(function () {
                    $('#panOrgEmpMessage').popup('close');
                })
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
                $('#pageOrgFOWSTPrinterIP').hide(); $('#pageAddDevicePrinter').hide(); $('#pageOrgFOWSTList').hide();
            })
            .on('click', '#btnOrgFOWSTEmailSave', function () {
                if ($.trim($("#txtOrgFOWSTEmail").val()) == "") {
                    $('#TitleOrgEmp').text('Email Address');
                    $('#MsgOrgEmp').text('Please enter email address.');
                    $('#panOrgEmpMessage').popup('open');
                    return;
                }

                if (IsEmail($.trim($("#txtOrgFOWSTEmail").val())) == false) {
                    $('#TitleOrgEmp').text('Email Address');
                    $('#MsgOrgEmp').text('Please enter valid email address.');
                    $('#panOrgEmpMessage').popup('open');
                    return;
                }

                if ($.trim($("#selOrgFOWSTEmailType").val()) == "") {
                    $('#TitleOrgEmp').text('Email Type');
                    $('#MsgOrgEmp').text('Please select email type.');
                    $('#panOrgEmpMessage').popup('open');
                    return;
                }

                SaveOrgFOWSTEmail();
            })
            .on('click', '#btnOrgFOWSTEmailRemove', function () {
                $('#TitleOrgEmp').text('Remove');
                $('#MsgOrgEmp').text('Remove this record?');
                $('#panOrgEmpMessage').popup('open');
                $('#btnOKOrgEmp').text('Yes').click(function (e) {
                    e.preventDefault();
                    RemoveOrgFOWSTEmail();
                })
                $('#divBtnCancel').show();
                $('#btnCancelOrgEmp').text('No');
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
                $('#pageOrgFOWSTPrinterIP').hide(); $('#pageAddDevicePrinter').hide(); $('#pageOrgFOWSTList').hide();
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
                }
            })
            .on('click', '#btnOrgFOWSTDone', function (e) {
                e.preventDefault();

                //////                if (window.pageSourcePage != undefined) {
                //////                    $.mobile.changePage("#" + $(window.pageSourcePage).attr('id'));
                //////                }
                //////                else {
                //////                    ChangePage('/OrganizationShort/OrganizationSummary');
                //////                }
                //                window.location = MergeUrlParameters({
                //                    OrgID: sessionStorage.OrgID,
                //                    IsMobile: CBool(IsMobile) ? 1 : 0
                //                }, '/OrganizationShort/OrganizationSummary');
                window.location.hash = '';
                window.location.hash = 'UniversalPage';
                LibraryTable.ViewCustomerPage();
            })

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
                                                $('#btnOrgFOWSTCity').text($(this).data('data').Name).button('refresh');
                                            }
                                            else if (NameOfBtnForLibrary == "BusinessPermit") {
                                                $('#txtOrgFOWSTBPermitCityID').val($(this).data('data').ID);
                                                $('#btnOrgFOWSTBPermitCity').text($(this).data('data').Name).button('refresh');
                                            }

                                            $('#panSearchLibrary').popup('close');
                                        })
                                    ));
                            }

                        }
                        else {
                            $('#lstOfLibrary').append($('<li>').text('No Result(s) Found'));
                        }

                        $('#lstOfLibrary').listview('refresh');
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
                                                $('#btnOrgFOWSTCountry').text($(this).data('data').Name).button('refresh');
                                            }
                                            else if (NameOfBtnForLibrary == "BusinessPermit") {
                                                $('#txtOrgFOWSTBPermitCountryID').val($(this).data('data').ID);
                                                $('#btnOrgFOWSTBPermitCountry').text($(this).data('data').Name).button('refresh');
                                            }
                                            else if (NameOfBtnForLibrary == "Telephone") {
                                                $('#txtOrgFOWSTTelCountryID').val($(this).data('data').ID);
                                                $('#btnOrgFOWSTTelCountryCode').text($(this).data('data').Code + ' - ' + $(this).data('data').CallingCode).button('refresh');
                                            }
                                            else if (NameOfBtnForLibrary == "Mobile") {
                                                $('#txtOrgFOWSTMobileCountryID').val($(this).data('data').ID);
                                                $('#btnOrgFOWSTMobileCountryCode').text($(this).data('data').Code + ' - ' + $(this).data('data').CallingCode).button('refresh');
                                            }

                                            $('#panSearchLibrary').popup('close');
                                        })
                                    ));
                            }

                        }
                        else {
                            $('#lstOfLibrary').append($('<li>').text('No Result(s) Found'));
                        }

                        $('#lstOfLibrary').listview('refresh');
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
                                                $('#btnOrgFOWSTTelAreaCode').text($(this).data('data').Area + ' - ' + $(this).data('data').CallingCode).button('refresh');
                                            }

                                            $('#panSearchLibrary').popup('close');
                                        })
                                    ));
                            }

                        }
                        else {
                            $('#lstOfLibrary').append($('<li>').text('No Result(s) Found'));
                        }

                        $('#lstOfLibrary').listview('refresh');
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
                                                $('#btnOrgFOWSTMobileTelCoPrefixCode').text($(this).data('data').TelCo + ' - ' + $(this).data('data').Prefix).button('refresh');
                                            }

                                            $('#panSearchLibrary').popup('close');
                                        })
                                    ));
                            }

                        }
                        else {
                            $('#lstOfLibrary').append($('<li>').text('No Result(s) Found'));
                        }

                        $('#lstOfLibrary').listview('refresh');
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
                                            if (NameOfBtnForLibrary == "Mobile") {
                                                $('#txtOrgFOWSTFranchisorID').val($(this).data('data').ID);
                                                $('#btnOrgFOWSTFranchisor').text($(this).data('data').CompanyName).button('refresh');
                                            }

                                            $('#panSearchLibrary').popup('close');
                                        })
                                    ));
                            }

                        }
                        else {
                            $('#lstOfLibrary').append($('<li>').text('No Result(s) Found'));
                        }

                        $('#lstOfLibrary').listview('refresh');
                    }
                });
            }

            function getOrgAddPlantAndPropertyType() {

                AjaxSend({
                    url: 'Browser/LibraryFOWSTType',
                    success: function (vData) {
                        var vRows = vData.Results.Rows;

                        $('#selOrgFOWSTType').empty();

                        $('#selOrgFOWSTType').append(new Option('Please Select', ''));
                        for (var p = 0; p <= vRows.length - 1; p++) {
                            $('#selOrgFOWSTType').append(new Option(vRows[p].Name, vRows[p].ID, false, false));
                        }
                    },
                    error: function (vError) {
                        var pDummy;
                    }
                });
            }

            function getOrgAddWarehouseType() {
                AjaxSend({
                    url: 'Browser/LibraryFOWSTWarehouseType',
                    success: function (vData) {
                        var vRows = vData.Results.Rows;

                        $('#selOrgFOWSTWarehouseType').empty();

                        $('#selOrgFOWSTWarehouseType').append(new Option('Please Select', ''));
                        for (var p = 0; p <= vRows.length - 1; p++) {
                            $('#selOrgFOWSTWarehouseType').append(new Option(vRows[p].Name, vRows[p].ID, false, false));
                        }
                    },
                    error: function (vError) {
                        var pDummy;
                    }
                });
            }

            function getOrgAddRetailStoreType() {
                AjaxSend({
                    url: 'Browser/LibraryFOWSTRetailStoreType',
                    success: function (vData) {
                        var vRows = vData.Results.Rows;

                        $('#selOrgFOWSTRetailStoreType').empty();

                        $('#selOrgFOWSTRetailStoreType').append(new Option('Please Select', ''));
                        for (var p = 0; p <= vRows.length - 1; p++) {
                            $('#selOrgFOWSTRetailStoreType').append(new Option(vRows[p].Name, vRows[p].ID, false, false));
                        }
                    },
                    error: function (vError) {
                        var pDummy;
                    }
                });
            }

            function SetOrgAddSubFOWSTType() {

                $('#divOrgFOWSTRetailStoreTypeInfo').hide();
                $("#divOrgFOWSTWarehouseTypeInfo").hide();
                $('#divOrgFOWSTDeliveryEquipmentInfo').hide();
                $("#divOrgFOWSTSection").hide();

                //Branch
                if ($("#selOrgFOWSTType").val().toUpperCase() == '6AF9BC83-426C-4774-A051-044FC09C552B') {
                    $('#divOrgFOWSTRetailStoreTypeInfo').show();
                    $("#divOrgFOWSTSection").show();
                } //Office
                else if ($("#selOrgFOWSTType").val().toUpperCase() == 'F58168F1-E00A-415A-A29E-5D4B92784B2B' ||
                    $("#selOrgFOWSTType").val().toUpperCase() == '4BC157CE-69A2-4371-BA91-D4E9B6AFB3F2') {
                    $("#divOrgFOWSTSection").show();

                } //Delivery Equipment
                else if ($("#selOrgFOWSTType").val().toUpperCase() == 'CE791943-9D80-42FC-BEBE-5BB52A5792E8') {
                    $('#divOrgFOWSTDeliveryEquipmentInfo').show();
                } //Warehouse
                else {
                    $('#divOrgFOWSTWarehouseTypeInfo').show();
                    $("#divOrgFOWSTSection").show();
                }
            }

            function ShowOrgAddHideFranchisorDetails() {

                if ($.trim($("#selOrgAddFranchisee").val()) == "1") {
                    $("#divFranchisorInfo").show();
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

            function getOrgFOWSTList() {
                AjaxSend({
                    url: 'OrganizationShort/getOrgFOWSTList',
                    success: function (vData) {
                        LoadOrgFOWSTList(vData.OrganizationFowstData.Rows);
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
                                $('#pageOrgFOWSTPrinterIP').hide(); $('#pageAddDevicePrinter').hide(); $('#pageOrgFOWSTList').hide();
                                ShowOrgFOWSTInfo($(this).attr('ID'));
                            })
                            .append($('<h3>')
                                .attr("style", "white-space:normal")
                                .text(vRows[p].Name)
                            )
                            .append($('<p>')
                                .attr("style", "white-space:normal")
                                .text(vRows[p].CompleteAddress)
                            )
                        );
                    pLV.append(pItem);
                };

                pLV.listview('refresh');
            };

            function ShowOrgFOWSTInfo(strID) {
                var pData = {};
                pData.ID = strID;

                ClearOrgFOWSTFields();

                AjaxSend({
                    url: "OrganizationShort/getOrgFOWSTInfo",
                    data: pData,
                    success: function (vData) {

                        if (vData.OrganizationFowstData.Rows.length > 0) {

                            $("#txtOrgFOWSTID").val(vData.OrganizationFowstData.Rows[0].ID || '');

                            $("#selOrgFOWSTType").val(vData.OrganizationFowstData.Rows[0].FOWSTTypeID || '').selectmenu('refresh');
                            SetOrgAddSubFOWSTType();

                            $("#txtOrgFOWSTCode").val(vData.OrganizationFowstData.Rows[0].Code || '');
                            $("#txtOrgFOWSTName").val(vData.OrganizationFowstData.Rows[0].Name || '');
                            $("#txtOrgFOWSTDescription").val(vData.OrganizationFowstData.Rows[0].Description || '');

                            $("#txtOrgFOWSTBuildingNo").val(vData.OrganizationFowstData.Rows[0].StreetNumber || '');
                            $("#txtOrgFOWSTStreet").val(vData.OrganizationFowstData.Rows[0].Street || '');
                            $("#txtOrgFOWSTCityID").val(vData.OrganizationFowstData.Rows[0].AddressCityID || '');
                            $("#btnOrgFOWSTCity").text(vData.OrganizationFowstData.Rows[0].AddressCity || 'Please Select').button('refresh');
                            $("#txtOrgFOWSTCountryID").val(vData.OrganizationFowstData.Rows[0].AddressCountryID || '');
                            $("#btnOrgFOWSTCountry").text(vData.OrganizationFowstData.Rows[0].AddressCountry || 'Please Select').button('refresh');

                            $("#selOrgAddFranchisee").val(vData.OrganizationFowstData.Rows[0].IsFranchisee || '0').slider("refresh");
                            ShowOrgAddHideFranchisorDetails();
                            $("#txtOrgFOWSTFranchisorID").val(vData.OrganizationFowstData.Rows[0].FranchisorID || '');
                            $("#btnOrgFOWSTFranchisor").text(vData.OrganizationFowstData.Rows[0].Franchisor || '').button('refresh');

                            $("#selOrgFOWSTOwnershipType").val(vData.OrganizationFowstData.Rows[0].OwnershipType || '0').selectmenu("refresh");
                            SetOrgAddShowHideRentedInfo();

                            $("#selOrgFOWSTContractStartMonth").val(vData.OrganizationFowstData.Rows[0].ContractStartMonth || '').selectmenu('refresh');
                            $("#selOrgFOWSTContractStartYear").val(vData.OrganizationFowstData.Rows[0].ContractStartYear || '').selectmenu('refresh');
                            setDayCollection('selOrgFOWSTContractStartMonth', 'selOrgFOWSTContractStartDay', 'selOrgFOWSTContractStartYear', true);
                            $("#selOrgFOWSTContractStartDay").val(vData.OrganizationFowstData.Rows[0].ContractStartDay || '').selectmenu('refresh');

                            $("#selOrgFOWSTContractEndMonth").val(vData.OrganizationFowstData.Rows[0].ContractEndMonth || '').selectmenu('refresh');
                            $("#selOrgFOWSTContractEndYear").val(vData.OrganizationFowstData.Rows[0].ContractEndYear || '').selectmenu('refresh');
                            setDayCollection('selOrgFOWSTContractEndMonth', 'selOrgFOWSTContractEndDay', 'selOrgFOWSTContractEndYear', true);
                            $("#selOrgFOWSTContractEndDay").val(vData.OrganizationFowstData.Rows[0].ContractEndDay || '').selectmenu('refresh');

                            $("#txtOrgFOWSTCostPerMonth").val(vData.OrganizationFowstData.Rows[0].CostPerMonth || '');

                            $("#selOrgFOWSTIsMortgaged").val(vData.OrganizationFowstData.Rows[0].IsMortgaged || '1').slider("refresh");

                            if (vData.OrganizationFowstData.Rows[0].FOWSTTypeID != undefined) {
                                if (vData.OrganizationFowstData.Rows[0].FOWSTTypeID.toUpperCase() == '6AF9BC83-426C-4774-A051-044FC09C552B') //Branch
                                    $("#selOrgFOWSTRetailStoreType").val(vData.OrganizationFowstData.Rows[0].SubPropertyTypeID || '').selectmenu('refresh');
                                else if (vData.OrganizationFowstData.Rows[0].FOWSTTypeID.toUpperCase() == '247C961C-A219-45DF-BDA3-3C137B734ABF') //Warehouse
                                    $("#selOrgFOWSTWarehouseType").val(vData.OrganizationFowstData.Rows[0].SubPropertyTypeID || '').selectmenu('refresh');
                            }


                            $("#txtOrgFOWSTPlateNo").val(vData.OrganizationFowstData.Rows[0].PlateNo || '');
                            $("#txtOrgFOWSTNoOfWheels").val(vData.OrganizationFowstData.Rows[0].NoOfWheels || '');
                            $("#txtOrgFOWSTBodyColor").val(vData.OrganizationFowstData.Rows[0].BodyColor || '');
                            $("#txtOrgFOWSTLoadLimitWeight").val(vData.OrganizationFowstData.Rows[0].Weight || '');
                            $("#txtOrgFOWSTLoadLimitDimensions").val(vData.OrganizationFowstData.Rows[0].Dimension || '');

                            pData.OrgFOWSTID = vData.OrganizationFowstData.Rows[0].ID;

                            //Get LTO
                            AjaxSend({
                                url: 'OrganizationShort/getOrgFOWSTLTOList',
                                data: pData,
                                success: function (vData) {
                                    LoadOrgAddLTO(vData.OrganizationFowstData.Rows);
                                }
                            });

                            //Get LTFRB
                            AjaxSend({
                                url: 'OrganizationShort/getOrgFOWSTLTFRBList',
                                data: pData,
                                success: function (vData) {
                                    LoadOrgAddLTFRB(vData.OrganizationFowstData.Rows);
                                }
                            });

                            //Get Section
                            AjaxSend({
                                url: 'OrganizationShort/getOrgFOWSTSectionList',
                                data: pData,
                                success: function (vData) {
                                    LoadOrgAddSection(vData.OrganizationFowstData.Rows);
                                }
                            });

                            //Get BPermit
                            AjaxSend({
                                url: 'OrganizationShort/getOrgFOWSTBPermitList',
                                data: pData,
                                success: function (vData) {
                                    LoadOrgAddBPermit(vData.OrganizationFowstData.Rows);
                                }
                            });

                            //Get Tel
                            AjaxSend({
                                url: 'OrganizationShort/getOrgFOWSTTelList',
                                data: pData,
                                success: function (vData) {
                                    LoadOrgAddTel(vData.OrganizationFowstData.Rows);
                                }
                            });

                            //Get mobile
                            AjaxSend({
                                url: 'OrganizationShort/getOrgFOWSTMobileList',
                                data: pData,
                                success: function (vData) {
                                    LoadOrgAddMobile(vData.OrganizationFowstData.Rows);
                                }
                            });

                            //Get Email
                            AjaxSend({
                                url: 'OrganizationShort/getOrgFOWSTEmailList',
                                data: pData,
                                success: function (vData) {
                                    LoadOrgAddEmail(vData.OrganizationFowstData.Rows);
                                }
                            });
                        }

                    }
                });

            };

            function LoadOrgAddLTO(vRows) {

                for (var p = 0; p <= vRows.length - 1; p++) {

                    arOrgAddLTO = new Array();
                    arOrgAddLTO[intOrgFOWSTLTORecID] = vRows[p].ID;
                    arOrgAddLTO[intOrgFOWSTLTOORNo] = vRows[p].ORNo;
                    arOrgAddLTO[intOrgFOWSTLTODateIssuedMonth] = vRows[p].DateIssuedMonth;
                    arOrgAddLTO[intOrgFOWSTLTODateIssuedDay] = vRows[p].DateIssuedDay;
                    arOrgAddLTO[intOrgFOWSTLTODateIssuedYear] = vRows[p].DateIssuedYear;
                    arOrgAddLTO[intOrgFOWSTLTODateExpirationMonth] = vRows[p].ExpirationDateMonth;
                    arOrgAddLTO[intOrgFOWSTLTODateExpirationDay] = vRows[p].ExpirationDateDay;
                    arOrgAddLTO[intOrgFOWSTLTODateExpirationYear] = vRows[p].ExpirationDateYear;
                    arOrgAddLTO[intOrgFOWSTLTOState] = "";

                    sessionStorage["OrgAddLTO-" + p] = JSON.stringify(arOrgAddLTO);

                };

                intOrgFOWSTLTOCntr = vRows.length;
                LoadOrgFOWSTLTOList();

            };

            function LoadOrgAddLTFRB(vRows) {

                for (var p = 0; p <= vRows.length - 1; p++) {

                    arOrgAddLTFRB = new Array();
                    arOrgAddLTFRB[intOrgFOWSTLTFRBRecID] = vRows[p].ID;
                    arOrgAddLTFRB[intOrgFOWSTLTFRBORNo] = vRows[p].ORNo;
                    arOrgAddLTFRB[intOrgFOWSTLTFRBDateIssuedMonth] = vRows[p].DateIssuedMonth;
                    arOrgAddLTFRB[intOrgFOWSTLTFRBDateIssuedDay] = vRows[p].DateIssuedDay;
                    arOrgAddLTFRB[intOrgFOWSTLTFRBDateIssuedYear] = vRows[p].DateIssuedYear;
                    arOrgAddLTFRB[intOrgFOWSTLTFRBDateExpirationMonth] = vRows[p].ExpirationDateMonth;
                    arOrgAddLTFRB[intOrgFOWSTLTFRBDateExpirationDay] = vRows[p].ExpirationDateDay;
                    arOrgAddLTFRB[intOrgFOWSTLTFRBDateExpirationYear] = vRows[p].ExpirationDateYear;
                    arOrgAddLTFRB[intOrgFOWSTLTFRBState] = "";

                    sessionStorage["OrgAddLTFRB-" + p] = JSON.stringify(arOrgAddLTFRB);

                };

                intOrgFOWSTLTFRBCntr = vRows.length;
                LoadOrgFOWSTLTFRBList();

            };

            function LoadOrgAddSection(vRows) {

                for (var p = 0; p <= vRows.length - 1; p++) {

                    arOrgAddSection = new Array();
                    arOrgAddSection[intOrgFOWSTSecRecID] = vRows[p].ID;
                    arOrgAddSection[intOrgFOWSTSecCode] = vRows[p].Code;
                    arOrgAddSection[intOrgFOWSTSecName] = vRows[p].SecName;
                    arOrgAddSection[intOrgFOWSTSecDescription] = vRows[p].Description;
                    arOrgAddSection[intOrgFOWSTSecFloorArea] = vRows[p].FloorArea;
                    arOrgAddSection[intOrgFOWSTSecSpecialFeatures] = vRows[p].SpecialFeatures;
                    arOrgAddSection[intOrgFOWSTSecState] = "";

                    sessionStorage["OrgAddSection-" + p] = JSON.stringify(arOrgAddSection);

                };

                intOrgFOWSTSectionCntr = vRows.length;
                LoadOrgFOWSTSectionList();

            };

            function LoadOrgAddBPermit(vRows) {

                for (var p = 0; p <= vRows.length - 1; p++) {

                    arOrgAddBPermit = new Array();
                    arOrgAddBPermit[intOrgFOWSTBPermitRecID] = vRows[p].ID;
                    arOrgAddBPermit[intOrgFOWSTBPermitNo] = vRows[p].BusinessPermitNo;
                    arOrgAddBPermit[intOrgFOWSTBPermitBusinessID] = vRows[p].BusinessID;
                    arOrgAddBPermit[intOrgFOWSTBPermitTradeName] = vRows[p].TradeName;
                    arOrgAddBPermit[intOrgFOWSTBPermitBuildingHouseNo] = vRows[p].BuildingHouseNo;
                    arOrgAddBPermit[intOrgFOWSTBPermitStreet] = vRows[p].Street;
                    arOrgAddBPermit[intOrgFOWSTBPermitCityID] = vRows[p].AddressCityID;
                    arOrgAddBPermit[intOrgFOWSTBPermitCity] = vRows[p].AddressCity;
                    arOrgAddBPermit[intOrgFOWSTBPermitCountryID] = vRows[p].AddressCountryID;
                    arOrgAddBPermit[intOrgFOWSTBPermitCountry] = vRows[p].AddressCountry;
                    arOrgAddBPermit[intOrgFOWSTBPermitORNo] = vRows[p].ORNo;
                    arOrgAddBPermit[intOrgFOWSTBPermitBusinessPlateNo] = vRows[p].BusinessPlateNo;
                    arOrgAddBPermit[intOrgFOWSTBPermitDatePaymentMonth] = vRows[p].DateOfPaymentMonth;
                    arOrgAddBPermit[intOrgFOWSTBPermitDatePaymentDay] = vRows[p].DateOfPaymentDay;
                    arOrgAddBPermit[intOrgFOWSTBPermitDatePaymentYear] = vRows[p].DateOfPaymentYear;
                    arOrgAddBPermit[intOrgFOWSTBPermitDateIssuedMonth] = vRows[p].DateIssuedMonth;
                    arOrgAddBPermit[intOrgFOWSTBPermitDateIssuedDay] = vRows[p].DateIssuedDay;
                    arOrgAddBPermit[intOrgFOWSTBPermitDateIssuedYear] = vRows[p].DateIssuedYear;
                    arOrgAddBPermit[intOrgFOWSTBPermitState] = "";

                    sessionStorage["OrgAddBPermit-" + p] = JSON.stringify(arOrgAddBPermit);

                };

                intOrgFOWSTBPermitCntr = vRows.length;
                LoadOrgFOWSTBPermitList();

            };

            function LoadOrgAddTel(vRows) {

                for (var p = 0; p <= vRows.length - 1; p++) {

                    arOrgAddTel = new Array();
                    arOrgAddTel[intOrgFOWSTTelRecID] = vRows[p].ID;
                    arOrgAddTel[intOrgFOWSTTelCountryID] = vRows[p].CountryID;
                    arOrgAddTel[intOrgFOWSTTelCountryCode] = vRows[p].Country;
                    arOrgAddTel[intOrgFOWSTTelCallingCode] = vRows[p].CountryCode;
                    arOrgAddTel[intOrgFOWSTTelAreaCodeID] = vRows[p].AreaCodeID;
                    arOrgAddTel[intOrgFOWSTTelArea] = vRows[p].Area;
                    arOrgAddTel[intOrgFOWSTTelAreaCode] = vRows[p].AreaCode;
                    arOrgAddTel[intOrgFOWSTTelContactNo] = vRows[p].ContactNo;
                    arOrgAddTel[intOrgFOWSTTelExtNo] = vRows[p].ExtensionNo;
                    arOrgAddTel[intOrgFOWSTTelContactType] = vRows[p].ContactType;
                    arOrgAddTel[intOrgFOWSTTelState] = "";

                    sessionStorage["OrgAddTel-" + p] = JSON.stringify(arOrgAddTel);

                };

                intOrgFOWSTTelCntr = vRows.length;
                LoadOrgFOWSTTelList();

            };

            function LoadOrgAddMobile(vRows) {

                for (var p = 0; p <= vRows.length - 1; p++) {

                    arOrgAddMobile = new Array();
                    arOrgAddMobile[intOrgFOWSTMobileRecID] = vRows[p].ID;
                    arOrgAddMobile[intOrgFOWSTMobileCountryID] = vRows[p].CountryID;
                    arOrgAddMobile[intOrgFOWSTMobileCountryCode] = vRows[p].Country;
                    arOrgAddMobile[intOrgFOWSTMobileCallingCode] = vRows[p].CountryCode;
                    arOrgAddMobile[intOrgFOWSTMobileTelCoPrefixID] = vRows[p].TelCoPrefixID;
                    arOrgAddMobile[intOrgFOWSTMobilePrefix] = vRows[p].Prefix;
                    arOrgAddMobile[intOrgFOWSTMobileTelCo] = vRows[p].TelCo;
                    arOrgAddMobile[intOrgFOWSTMobileContactNo] = vRows[p].ContactNo;
                    arOrgAddMobile[intOrgFOWSTMobileContactType] = vRows[p].ContactType;
                    arOrgAddMobile[intOrgFOWSTMobileState] = "";

                    sessionStorage["OrgAddMobile-" + p] = JSON.stringify(arOrgAddMobile);

                };

                intOrgFOWSTMobileCntr = vRows.length;
                LoadOrgFOWSTMobileList();

            };

            function LoadOrgAddEmail(vRows) {

                for (var p = 0; p <= vRows.length - 1; p++) {

                    arOrgAddEmail = new Array();
                    arOrgAddEmail[intOrgFOWSTEmailRecID] = vRows[p].ID;
                    arOrgAddEmail[intOrgFOWSTEmailAddress] = vRows[p].EmailAddress;
                    arOrgAddEmail[intOrgFOWSTEmailType] = vRows[p].EmailType;
                    arOrgAddEmail[intOrgFOWSTEmailState] = "";

                    sessionStorage["OrgAddEmail-" + p] = JSON.stringify(arOrgAddEmail);

                };

                intOrgFOWSTEmailCntr = vRows.length;
                LoadOrgFOWSTEmailList();

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
                $('#pageOrgFOWSTPrinterIP').hide();
                $('#pageAddDevicePrinter').hide();
                $('#pageOrgFOWSTList').hide();

                ClearOrgAddTruckFields();

                intOrgFOWSTLTOIndex = index;

                $("#txtOrgFOWSTTRegID").val(arData[intOrgFOWSTLTORecID]);

                $("#txtOrgFOWSTTRegORNo").val(arData[intOrgFOWSTLTOORNo]);

                $("#selOrgFOWSTTRegIssuedMonth").val(arData[intOrgFOWSTLTODateIssuedMonth]).selectmenu('refresh');
                $("#selOrgFOWSTTRegIssuedDay").val(arData[intOrgFOWSTLTODateIssuedDay]).selectmenu('refresh');
                $("#selOrgFOWSTTRegIssuedYear").val(arData[intOrgFOWSTLTODateIssuedYear]).selectmenu('refresh');

                $("#selOrgFOWSTTRegExpirationMonth").val(arData[intOrgFOWSTLTODateExpirationMonth]).selectmenu('refresh');
                $("#selOrgFOWSTTRegExpirationDay").val(arData[intOrgFOWSTLTODateExpirationDay]).selectmenu('refresh');
                $("#selOrgFOWSTTRegExpirationYear").val(arData[intOrgFOWSTLTODateExpirationYear]).selectmenu('refresh');

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
                $('#pageOrgFOWSTPrinterIP').hide();
                $('#pageAddDevicePrinter').hide();
                $('#pageOrgFOWSTList').hide();

                ClearOrgAddTruckFields();

                intOrgFOWSTLTFRBIndex = index;

                $("#txtOrgFOWSTTRegID").val(arData[intOrgFOWSTLTFRBRecID]);

                $("#txtOrgFOWSTTRegORNo").val(arData[intOrgFOWSTLTFRBORNo]);

                $("#selOrgFOWSTTRegIssuedMonth").val(arData[intOrgFOWSTLTFRBDateIssuedMonth]).selectmenu('refresh');
                $("#selOrgFOWSTTRegIssuedDay").val(arData[intOrgFOWSTLTFRBDateIssuedDay]).selectmenu('refresh');
                $("#selOrgFOWSTTRegIssuedYear").val(arData[intOrgFOWSTLTFRBDateIssuedYear]).selectmenu('refresh');

                $("#selOrgFOWSTTRegExpirationMonth").val(arData[intOrgFOWSTLTFRBDateExpirationMonth]).selectmenu('refresh');
                $("#selOrgFOWSTTRegExpirationDay").val(arData[intOrgFOWSTLTFRBDateExpirationDay]).selectmenu('refresh');
                $("#selOrgFOWSTTRegExpirationYear").val(arData[intOrgFOWSTLTFRBDateExpirationYear]).selectmenu('refresh');

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
                $('#pageOrgFOWSTPrinterIP').hide();
                $('#pageAddDevicePrinter').hide();
                $('#pageOrgFOWSTList').hide();

                ClearOrgAddSectionFields();

                intOrgFOWSTSectionIndex = index;

                $("#txtOrgFOWSTSecID").val(arData[intOrgFOWSTSecRecID]);

                $("#txtOrgFOWSTSecCode").val(arData[intOrgFOWSTSecCode]);
                $("#txtOrgFOWSTSecName").val(arData[intOrgFOWSTSecName]);
                $("#txtOrgFOWSTSecDescription").val(arData[intOrgFOWSTSecDescription]);

                $("#txtOrgFOWSTSecFloorArea").val(arData[intOrgFOWSTSecFloorArea]);
                $("#txtOrgFOWSTSecSpecFeatures").val(arData[intOrgFOWSTSecSpecialFeatures]);

                IsNewOrgAddSection = false;

                $("#btnOrgFOWSTSecRemove").show();

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
                $('#pageOrgFOWSTPrinterIP').hide();
                $('#pageAddDevicePrinter').hide();
                $('#pageOrgFOWSTList').hide();

                ClearOrgAddBPermitFields();

                intOrgFOWSTBPermitIndex = index;

                $("#txtOrgFOWSTBPermitRecID").val(arData[intOrgFOWSTBPermitRecID]);

                $("#txtOrgFOWSTBPermitNo").val(arData[intOrgFOWSTBPermitNo]);
                $("#txtOrgFOWSTBPermitID").val(arData[intOrgFOWSTBPermitBusinessID]);
                $("#txtOrgFOWSTBPermitTradeName").val(arData[intOrgFOWSTBPermitTradeName]);

                $("#txtOrgFOWSTBPermitBuildingNo").val(arData[intOrgFOWSTBPermitBuildingHouseNo]);
                $("#txtOrgFOWSTBPermitStreet").val(arData[intOrgFOWSTBPermitStreet]);

                $("#txtOrgFOWSTBPermitCityID").val(arData[intOrgFOWSTBPermitCityID]);
                $("#btnOrgFOWSTBPermitCity").text(arData[intOrgFOWSTBPermitCity]).button('refresh');
                $("#txtOrgFOWSTBPermitCountryID").val(arData[intOrgFOWSTBPermitCountryID]);
                $("#btnOrgFOWSTBPermitCountry").text(arData[intOrgFOWSTBPermitCountry]).button('refresh');

                $("#txtOrgFOWSTBPermitORNo").val(arData[intOrgFOWSTBPermitORNo]);

                $("#selOrgFOWSTBPermitDateOfPaymentMonth").val(arData[intOrgFOWSTBPermitDatePaymentMonth]).selectmenu('refresh');
                $("#selOrgFOWSTBPermitDateOfPaymentDay").val(arData[intOrgFOWSTBPermitDatePaymentDay]).selectmenu('refresh');
                $("#selOrgFOWSTBPermitDateOfPaymentYear").val(arData[intOrgFOWSTBPermitDatePaymentYear]).selectmenu('refresh');

                $("#txtOrgFOWSTBPermitBPlateNo").val(arData[intOrgFOWSTBPermitBusinessPlateNo]);

                $("#selOrgFOWSTBPermitBPlateIssuedMonth").val(arData[intOrgFOWSTBPermitDateIssuedMonth]).selectmenu('refresh');
                $("#selOrgFOWSTBPermitBPlateIssuedDay").val(arData[intOrgFOWSTBPermitDateIssuedDay]).selectmenu('refresh');
                $("#selOrgFOWSTBPermitBPlateIssuedYear").val(arData[intOrgFOWSTBPermitDateIssuedYear]).selectmenu('refresh');

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
                $('#pageOrgFOWSTPrinterIP').hide();
                $('#pageAddDevicePrinter').hide();
                $('#pageOrgFOWSTList').hide();

                ClearOrgAddTelFields();

                intOrgFOWSTTelIndex = index;

                $("#txtOrgFOWSTTelID").val(arData[intOrgFOWSTTelRecID]);

                $("#txtOrgFOWSTTelCountryID").val(arData[intOrgFOWSTTelCountryID]);
                $("#btnOrgFOWSTTelCountryCode").text(arData[intOrgFOWSTTelCountryCode] + ' - ' + arData[intOrgFOWSTTelCallingCode]).button('refresh');

                $("#txtOrgFOWSTTelAreaCodeID").val(arData[intOrgFOWSTTelAreaCodeID]);
                $("#btnOrgFOWSTTelAreaCode").text(arData[intOrgFOWSTTelArea] + ' - ' + arData[intOrgFOWSTTelAreaCode]).button('refresh');

                $("#txtOrgFOWSTTelNo").val(arData[intOrgFOWSTTelContactNo]);
                $("#txtOrgFOWSTTelExtension").val(arData[intOrgFOWSTTelExtNo]);

                $("#selOrgFOWSTTelType").val(arData[intOrgFOWSTTelContactType]).selectmenu('refresh');

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
                $('#pageOrgFOWSTPrinterIP').hide();
                $('#pageAddDevicePrinter').hide();
                $('#pageOrgFOWSTList').hide();

                ClearOrgAddMobileFields();

                intOrgFOWSTMobileIndex = index;

                $("#txtOrgFOWSTMobileID").val(arData[intOrgFOWSTMobileRecID] || '');

                $("#txtOrgFOWSTMobileCountryID").val(arData[intOrgFOWSTMobileCountryID] || '');
                $("#btnOrgFOWSTMobileCountryCode").text((arData[intOrgFOWSTMobileCountryCode] + ' - ' + arData[intOrgFOWSTMobileCallingCode]) || 'Country Code').button('refresh');

                $("#txtOrgFOWSTTelCoPrefixID").val(arData[intOrgFOWSTMobileTelCoPrefixID] || '');
                $("#btnOrgFOWSTMobileTelCoPrefixCode").text((arData[intOrgFOWSTMobileTelCo] + ' - ' + arData[intOrgFOWSTMobilePrefix]) || 'Telco Code').button('refresh');

                $("#txtOrgFOWSTMobileNo").val(arData[intOrgFOWSTMobileContactNo] || '');
                $("#selOrgAddMobileType").val(arData[intOrgFOWSTMobileContactType] || '').selectmenu('refresh');

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
                $('#pageOrgFOWSTPrinterIP').hide();
                $('#pageAddDevicePrinter').hide();
                $('#pageOrgFOWSTList').hide();

                ClearOrgAddEmailFields();

                intOrgFOWSTEmailIndex = index;

                $("#txtOrgFOWSTEmailID").val(arData[intOrgFOWSTEmailRecID] || '');

                $("#txtOrgFOWSTEmail").val(arData[intOrgFOWSTEmailAddress] || '');
                $("#selOrgFOWSTEmailType").val(arData[intOrgFOWSTEmailType] || '').selectmenu('refresh');

                IsNewOrgAddEmailContact = false;

                $("#btnOrgFOWSTEmailRemove").show();


            };

            function ClearOrgFOWSTFields() {

                // dont clear
                // sessionStorage.clear();
                proj.ClearSessionStorage();

                sessionStorage["LibraryName"] = libname;
                sessionStorage["LabelLibraryName"] = lbllibname;
                sessionStorage["AddViewLibBCName"] = addlibname;
                sessionStorage["ViewID"] = orgid;

                $("#txtOrgFOWSTID").val('');

                $("#selOrgFOWSTType").val('').selectmenu('refresh');

                $("#txtOrgFOWSTCode").val('');
                $("#txtOrgFOWSTName").val('');
                $("#txtOrgFOWSTDescription").val('');

                $("#txtOrgFOWSTBuildingNo").val('');
                $("#txtOrgFOWSTStreet").val('');
                $("#txtOrgFOWSTCityID").val('');
                $("#btnOrgFOWSTCity").text('Please Select').button('refresh');
                $("#txtOrgFOWSTCountryID").val('');
                $("#btnOrgFOWSTCountry").text('Please Select').button('refresh');

                $("#selOrgAddFranchisee").val('0').slider("refresh");
                $("#txtOrgFOWSTFranchisorID").val('');
                $("#btnOrgFOWSTFranchisor").text('Please Select').button('refresh');
                ShowOrgAddHideFranchisorDetails();

                $("#selOrgFOWSTOwnershipType").val('0').selectmenu('refresh');
                SetOrgAddShowHideRentedInfo();

                $("#selOrgFOWSTContractStartMonth").val('').selectmenu('refresh');
                $("#selOrgFOWSTContractStartDay").val('').selectmenu('refresh');
                $("#selOrgFOWSTContractStartYear").val('').selectmenu('refresh');

                $("#selOrgFOWSTContractEndMonth").val('').selectmenu('refresh');
                $("#selOrgFOWSTContractEndDay").val('').selectmenu('refresh');
                $("#selOrgFOWSTContractEndYear").val('').selectmenu('refresh');

                $("#txtOrgFOWSTCostPerMonth").val('');

                $("#selOrgFOWSTIsMortgaged").val('1').slider("refresh");

                $("#selOrgFOWSTWarehouseType").val('').selectmenu('refresh');
                $("#selOrgFOWSTRetailStoreType").val('').selectmenu('refresh');

                $("#txtOrgFOWSTPlateNo").val('');
                $("#txtOrgFOWSTNoOfWheels").val('');
                $("#txtOrgFOWSTBodyColor").val('');
                $("#txtOrgFOWSTLoadLimitWeight").val('');
                $("#txtOrgFOWSTLoadLimitDimensions").val('');

                LoadOrgFOWSTLTOList();
                LoadOrgFOWSTLTFRBList();
                LoadOrgFOWSTSectionList();
                LoadOrgFOWSTBPermitList();
                LoadOrgFOWSTTelList();
                LoadOrgFOWSTMobileList();
                LoadOrgFOWSTEmailList();
            };

            function ClearOrgAddTruckFields() {

                $("#txtOrgFOWSTTRegID").val('');

                $("#txtOrgFOWSTTRegORNo").val('');

                $("#selOrgFOWSTTRegIssuedMonth").val('').selectmenu('refresh');
                $("#selOrgFOWSTTRegIssuedDay").val('').selectmenu('refresh');
                $("#selOrgFOWSTTRegIssuedYear").val('').selectmenu('refresh');

                $("#selOrgFOWSTTRegExpirationMonth").val('').selectmenu('refresh');
                $("#selOrgFOWSTTRegExpirationDay").val('').selectmenu('refresh');
                $("#selOrgFOWSTTRegExpirationYear").val('').selectmenu('refresh');

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

                $("#txtOrgFOWSTBPermitRecID").val('');

                $("#txtOrgFOWSTBPermitNo").val('');
                $("#txtOrgFOWSTBPermitID").val('');
                $("#txtOrgFOWSTBPermitTradeName").val('');

                $("#txtOrgFOWSTBPermitBuildingNo").val('');
                $("#txtOrgFOWSTBPermitStreet").val('');

                $("#txtOrgFOWSTBPermitCityID").val('');
                $("#btnOrgFOWSTBPermitCity").text('Please Select').button('refresh');

                $("#txtOrgFOWSTBPermitCountryID").val('');
                $("#btnOrgFOWSTBPermitCountry").text('Please Select').button('refresh');

                $("#txtOrgFOWSTBPermitORNo").val('');

                $("#selOrgFOWSTBPermitDateOfPaymentMonth").val('').selectmenu('refresh');
                $("#selOrgFOWSTBPermitDateOfPaymentDay").val('').selectmenu('refresh');
                $("#selOrgFOWSTBPermitDateOfPaymentYear").val('').selectmenu('refresh');

                $("#txtOrgFOWSTBPermitBPlateNo").val('');

                $("#selOrgFOWSTBPermitBPlateIssuedMonth").val('').selectmenu('refresh');
                $("#selOrgFOWSTBPermitBPlateIssuedDay").val('').selectmenu('refresh');
                $("#selOrgFOWSTBPermitBPlateIssuedYear").val('').selectmenu('refresh');

                $("#btnOrgFOWSTBPermitRemove").hide();

            }

            function ClearOrgAddTelFields() {

                $("#txtOrgFOWSTTelID").val('');

                $("#txtOrgFOWSTTelCountryID").val('');
                $("#btnOrgFOWSTTelCountryCode").text('Country Code').button('refresh');

                $("#txtOrgFOWSTTelAreaCodeID").val('');
                $("#btnOrgFOWSTTelAreaCode").text('Area Code').button('refresh');

                $("#txtOrgFOWSTTelNo").val('');
                $("#txtOrgFOWSTTelExtension").val('');

                $("#selOrgFOWSTTelType").val('').selectmenu('refresh');

                $("#btnOrgFOWSTTelRemove").hide();

            }

            function ClearOrgAddMobileFields() {
                $("#txtOrgFOWSTMobileID").val('');

                $("#txtOrgFOWSTMobileCountryID").val('');
                $("#btnOrgFOWSTMobileCountryCode").text('Country Code').button('refresh');

                $("#txtOrgFOWSTTelCoPrefixID").val('');
                $("#btnOrgFOWSTMobileTelCoPrefixCode").text('Telco Code').button('refresh');

                $("#txtOrgFOWSTMobileNo").val('');
                $("#selOrgAddMobileType").val('').selectmenu('refresh');

                $("#btnOrgFOWSTMobileRemove").hide();

            }

            function ClearOrgAddEmailFields() {
                $("#txtOrgFOWSTEmailID").val('');

                $("#txtOrgFOWSTEmail").val('');
                $("#selOrgFOWSTEmailType").val('').selectmenu('refresh');

                $("#btnOrgFOWSTEmailRemove").hide();

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
                var intIndex;

                //LTO
                intIndex = 0;
                for (var i = 0; i <= intOrgFOWSTLTOCntr; i++) {

                    if (sessionStorage["OrgAddLTO-" + i] != undefined) {

                        arOrgAddLTO = new Array();
                        arOrgAddLTO = JSON.parse(sessionStorage["OrgAddLTO-" + i]);

                        var dtDateIssued = "";
                        if (arOrgAddLTO[intOrgFOWSTLTODateIssuedMonth] != "" &&
                            arOrgAddLTO[intOrgFOWSTLTODateIssuedDay] != "" &&
                            arOrgAddLTO[intOrgFOWSTLTODateIssuedYear] != "") {

                            dtDateIssued = arOrgAddLTO[intOrgFOWSTLTODateIssuedMonth] + "/" + arOrgAddLTO[intOrgFOWSTLTODateIssuedDay] + "/" + arOrgAddLTO[intOrgFOWSTLTODateIssuedYear];
                        }
                        var dtExpirationDate = "";
                        if (arOrgAddLTO[intOrgFOWSTLTODateExpirationMonth] != "" &&
                            arOrgAddLTO[intOrgFOWSTLTODateExpirationDay] != "" &&
                            arOrgAddLTO[intOrgFOWSTLTODateExpirationYear] != "") {

                            dtExpirationDate = arOrgAddLTO[intOrgFOWSTLTODateExpirationMonth] + "/" + arOrgAddLTO[intOrgFOWSTLTODateExpirationDay] + "/" + arOrgAddLTO[intOrgFOWSTLTODateExpirationYear];
                        }

                        pDataLTO[intIndex] = {
                            ID: arOrgAddLTO[intOrgFOWSTLTORecID],
                            ORNo: arOrgAddLTO[intOrgFOWSTLTOORNo],
                            DateIssued: dtDatePayment,
                            ExpirationDate: dtExpirationDate,
                            RecState: arOrgAddLTO[intOrgFOWSTLTOState]
                        };

                    }
                }

                //LTFRB
                intIndex = 0;
                for (var i = 0; i <= intOrgFOWSTLTFRBCntr; i++) {

                    if (sessionStorage["OrgAddLTFRB-" + i] != undefined) {

                        arOrgAddLTFRB = new Array();
                        arOrgAddLTFRB = JSON.parse(sessionStorage["OrgAddLTFRB-" + i]);

                        var dtDateIssued = "";
                        if (arOrgAddLTFRB[intOrgFOWSTLTFRBDateIssuedMonth] != "" &&
                            arOrgAddLTFRB[intOrgFOWSTLTFRBDateIssuedDay] != "" &&
                            arOrgAddLTFRB[intOrgFOWSTLTFRBDateIssuedYear] != "") {

                            dtDateIssued = arOrgAddLTFRB[intOrgFOWSTLTFRBDateIssuedMonth] + "/" + arOrgAddLTFRB[intOrgFOWSTLTFRBDateIssuedDay] + "/" + arOrgAddLTFRB[intOrgFOWSTLTFRBDateIssuedYear];
                        }
                        var dtExpirationDate = "";
                        if (arOrgAddLTFRB[intOrgFOWSTLTFRBDateExpirationMonth] != "" &&
                            arOrgAddLTFRB[intOrgFOWSTLTFRBDateExpirationDay] != "" &&
                            arOrgAddLTFRB[intOrgFOWSTLTFRBDateExpirationYear] != "") {

                            dtExpirationDate = arOrgAddLTFRB[intOrgFOWSTLTFRBDateExpirationMonth] + "/" + arOrgAddLTFRB[intOrgFOWSTLTFRBDateExpirationDay] + "/" + arOrgAddLTFRB[intOrgFOWSTLTFRBDateExpirationYear];
                        }

                        pDataLTFRB[intIndex] = {
                            ID: arOrgAddLTFRB[intOrgFOWSTLTFRBRecID],
                            ORNo: arOrgAddLTFRB[intOrgFOWSTLTFRBORNo],
                            DateIssued: dtDatePayment,
                            ExpirationDate: dtExpirationDate,
                            RecState: arOrgAddLTFRB[intOrgFOWSTLTFRBState]
                        };

                    }
                }

                //Section
                intIndex = 0;
                for (var i = 0; i <= intOrgFOWSTSectionCntr; i++) {

                    if (sessionStorage["OrgAddSection-" + i] != undefined) {

                        arOrgAddSection = new Array();
                        arOrgAddSection = JSON.parse(sessionStorage["OrgAddSection-" + i]);

                        pDataSection[intIndex] = {
                            ID: arOrgAddSection[intOrgFOWSTSecRecID],
                            Code: arOrgAddSection[intOrgFOWSTSecCode],
                            Name: arOrgAddSection[intOrgFOWSTSecName],
                            Description: arOrgAddSection[intOrgFOWSTSecDescription],
                            FloorArea: arOrgAddSection[intOrgFOWSTSecFloorArea],
                            SpecialFeatures: arOrgAddSection[intOrgFOWSTSecSpecialFeatures],
                            RecState: arOrgAddSection[intOrgFOWSTSecState]
                        };

                        intIndex = intIndex + 1;

                    }
                }

                //BPermit
                intIndex = 0;
                for (var i = 0; i <= intOrgFOWSTBPermitCntr; i++) {

                    if (sessionStorage["OrgAddBPermit-" + i] != undefined) {

                        arOrgAddBPermit = new Array();
                        arOrgAddBPermit = JSON.parse(sessionStorage["OrgAddBPermit-" + i]);

                        var dtDatePayment = "";
                        if (arOrgAddBPermit[intOrgFOWSTBPermitDatePaymentMonth] != "" &&
                            arOrgAddBPermit[intOrgFOWSTBPermitDatePaymentDay] != "" &&
                            arOrgAddBPermit[intOrgFOWSTBPermitDatePaymentYear] != "") {

                            dtDatePayment = arOrgAddBPermit[intOrgFOWSTBPermitDatePaymentMonth] + "/" + arOrgAddBPermit[intOrgFOWSTBPermitDatePaymentDay] + "/" + arOrgAddBPermit[intOrgFOWSTBPermitDatePaymentYear];
                        }
                        var dtDateIssued = "";
                        if (arOrgAddBPermit[intOrgFOWSTBPermitDateIssuedMonth] != "" &&
                            arOrgAddBPermit[intOrgFOWSTBPermitDateIssuedDay] != "" &&
                            arOrgAddBPermit[intOrgFOWSTBPermitDateIssuedYear] != "") {

                            dtDateIssued = arOrgAddBPermit[intOrgFOWSTBPermitDateIssuedMonth] + "/" + arOrgAddBPermit[intOrgFOWSTBPermitDateIssuedDay] + "/" + arOrgAddBPermit[intOrgFOWSTBPermitDateIssuedYear];
                        }

                        pDataBPermit[intIndex] = {
                            ID: arOrgAddBPermit[intOrgFOWSTBPermitRecID],
                            PermitNo: arOrgAddBPermit[intOrgFOWSTBPermitNo],
                            BusinessID: arOrgAddBPermit[intOrgFOWSTBPermitBusinessID],
                            TradeName: arOrgAddBPermit[intOrgFOWSTBPermitTradeName],
                            BuildingHouseNo: arOrgAddBPermit[intOrgFOWSTBPermitBuildingHouseNo],
                            Street: arOrgAddBPermit[intOrgFOWSTBPermitStreet],
                            AddressCityID: arOrgAddBPermit[intOrgFOWSTBPermitCityID],
                            AddressCountryID: arOrgAddBPermit[intOrgFOWSTBPermitCountryID],
                            ORNo: arOrgAddBPermit[intOrgFOWSTBPermitORNo],
                            BusinessPlateNo: arOrgAddBPermit[intOrgFOWSTBPermitBusinessPlateNo],
                            DatePayment: dtDatePayment,
                            DateIssued: dtDateIssued,
                            RecState: arOrgAddBPermit[intOrgFOWSTBPermitState]
                        };

                        intIndex = intIndex + 1;

                    }
                }

                //Tel
                intIndex = 0;
                for (var i = 0; i <= intOrgFOWSTTelCntr; i++) {

                    if (sessionStorage["OrgAddTel-" + i] != undefined) {

                        arOrgAddTel = new Array();
                        arOrgAddTel = JSON.parse(sessionStorage["OrgAddTel-" + i]);

                        pDataTel[intIndex] = {
                            ID: arOrgAddTel[intOrgFOWSTTelRecID],
                            CountryID: arOrgAddTel[intOrgFOWSTTelCountryID],
                            AreaCodeID: arOrgAddTel[intOrgFOWSTTelAreaCodeID],
                            ContactNo: arOrgAddTel[intOrgFOWSTTelContactNo],
                            ExtensionNo: arOrgAddTel[intOrgFOWSTTelExtNo],
                            ContactType: arOrgAddTel[intOrgFOWSTTelContactType],
                            RecState: arOrgAddTel[intOrgFOWSTTelState]
                        };

                        intIndex = intIndex + 1;

                    }
                }

                //Mobile
                intIndex = 0;
                for (var i = 0; i <= intOrgFOWSTMobileCntr; i++) {

                    if (sessionStorage["OrgAddMobile-" + i] != undefined) {

                        arOrgAddMobile = new Array();
                        arOrgAddMobile = JSON.parse(sessionStorage["OrgAddMobile-" + i]);

                        pDataMobile[intIndex] = {
                            ID: arOrgAddMobile[intOrgFOWSTMobileRecID],
                            CountryID: arOrgAddMobile[intOrgFOWSTMobileCountryID],
                            TelCoPrefixID: arOrgAddMobile[intOrgFOWSTMobileTelCoPrefixID],
                            ContactNo: arOrgAddMobile[intOrgFOWSTMobileContactNo],
                            ContactType: arOrgAddMobile[intOrgFOWSTMobileContactType],
                            RecState: arOrgAddMobile[intOrgFOWSTMobileState]
                        };

                        intIndex = intIndex + 1;

                    }
                }

                //Email
                intIndex = 0;
                for (var i = 0; i <= intOrgFOWSTEmailCntr; i++) {

                    if (sessionStorage["OrgAddEmail-" + i] != undefined) {

                        arOrgAddEmail = new Array();
                        arOrgAddEmail = JSON.parse(sessionStorage["OrgAddEmail-" + i]);

                        pDataEmail[intIndex] = {
                            ID: arOrgAddEmail[intOrgFOWSTEmailRecID],
                            EmailAddress: arOrgAddEmail[intOrgFOWSTEmailAddress],
                            EmailType: arOrgAddEmail[intOrgFOWSTEmailType],
                            RecState: arOrgAddEmail[intOrgFOWSTEmailState]
                        };

                        intIndex = intIndex + 1;

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
                    FOWSTCode: $.trim($("#txtOrgFOWSTCode").val()),
                    FOWSTName: $.trim($("#txtOrgFOWSTName").val()),
                    FOWSTDescription: $.trim($("#txtOrgFOWSTDescription").val()),

                    FOWSTStreetNumber: $.trim($("#txtOrgFOWSTBuildingNo").val()),
                    FOWSTStreet: $("#txtOrgFOWSTStreet").val(),
                    FOWSTCityID: $("#txtOrgFOWSTCityID").val(),
                    FOWSTCountryID: $("#txtOrgFOWSTCountryID").val(),

                    FOWSTIsFranchisee: $("#selOrgAddFranchisee").val(),
                    FOWSTFranchisorID: $("#txtOrgFOWSTFranchisorID").val(),

                    FOWSTOwnershipType: $("#selOrgFOWSTOwnershipType").val(),
                    FOWSTContractStart: dtContractStart,
                    FOWSTContractEnd: dtContractEnd,
                    FOWSTCostPerMonth: $("#txtOrgFOWSTCostPerMonth").val(),
                    FOWSTIsMortgaged: $("#selOrgFOWSTIsMortgaged").val(),

                    FOWSTSubTypeID: strFOWSTSubTypeID,

                    FOWSTPlateNo: $("#txtOrgFOWSTPlateNo").val(),
                    FOWSTNoOfWheels: $("#txtOrgFOWSTNoOfWheels").val(),
                    FOWSTBodyColor: $("#txtOrgFOWSTBodyColor").val(),
                    FOWSTWeight: $("#txtOrgFOWSTLoadLimitWeight").val(),
                    FOWSTDimension: $("#txtOrgFOWSTLoadLimitDimensions").val(),

                    OrgFOWSTLTOList: pDataLTO,
                    OrgFOWSTLTFRBList: pDataLTFRB,
                    OrgFOWSTSectionList: pDataSection,
                    OrgFOWSTBPermitList: pDataBPermit,
                    OrgFOWSTTelList: pDataTel,
                    OrgFOWSTMobileList: pDataMobile,
                    OrgFOWSTEmailList: pDataEmail

                };

                AjaxSend({
                    url: "OrganizationShort/SaveOrgFOWST",
                    data: pData,
                    success: function (vData) {

                        //                        window.IsPageLoaded = false;
                        //                        window.location = MergeUrlParameters({
                        //                            OrgID: sessionStorage.OrgID,
                        //                            IsMobile: CBool(IsMobile) ? 1 : 0
                        //                        }, '/OrganizationShort/OrganizationAddress');
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
                        $('#pageAddDevicePrinter').hide();
                        $('#pageOrgFOWSTList').show();
                        getOrgFOWSTList();
                        //                        ChangePage('/OrganizationShort/OrganizationAddress');
                        //                        getOrgFOWSTList();

                    }
                });

            };

            function SaveOrgFOWSTLTO() {

                arOrgAddLTO = new Array();
                arOrgAddLTO[intOrgFOWSTLTORecID] = $("#txtOrgFOWSTTRegID").val();
                arOrgAddLTO[intOrgFOWSTLTOORNo] = $("#txtOrgFOWSTTRegORNo").val();
                arOrgAddLTO[intOrgFOWSTLTODateIssuedMonth] = $("#selOrgFOWSTTRegIssuedMonth").val();
                arOrgAddLTO[intOrgFOWSTLTODateIssuedDay] = $("#selOrgFOWSTTRegIssuedDay").val();
                arOrgAddLTO[intOrgFOWSTLTODateIssuedYear] = $("#selOrgFOWSTTRegIssuedYear").val();
                arOrgAddLTO[intOrgFOWSTLTODateExpirationMonth] = $("#selOrgFOWSTTRegExpirationMonth").val();
                arOrgAddLTO[intOrgFOWSTLTODateExpirationDay] = $("#selOrgFOWSTTRegExpirationDay").val();
                arOrgAddLTO[intOrgFOWSTLTODateExpirationYear] = $("#selOrgFOWSTTRegExpirationYear").val();
                arOrgAddLTO[intOrgFOWSTLTOState] = (IsNewOrgAddLTO ? "New" : "Edit");

                sessionStorage["OrgAddLTO-" + intOrgFOWSTLTOIndex] = JSON.stringify(arOrgAddLTO);

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
                $('#pageOrgFOWSTPrinterIP').hide();
                $('#pageAddDevicePrinter').hide();
                $('#pageOrgFOWSTList').hide();
                LoadOrgFOWSTLTOList();
            };

            function SaveOrgFOWSTLTFRB() {

                arOrgAddLTFRB = new Array();
                arOrgAddLTFRB[intOrgFOWSTLTFRBRecID] = $("#txtOrgFOWSTTRegID").val();
                arOrgAddLTFRB[intOrgFOWSTLTFRBORNo] = $("#txtOrgFOWSTTRegORNo").val();
                arOrgAddLTFRB[intOrgFOWSTLTFRBDateIssuedMonth] = $("#selOrgFOWSTTRegIssuedMonth").val();
                arOrgAddLTFRB[intOrgFOWSTLTFRBDateIssuedDay] = $("#selOrgFOWSTTRegIssuedDay").val();
                arOrgAddLTFRB[intOrgFOWSTLTFRBDateIssuedYear] = $("#selOrgFOWSTTRegIssuedYear").val();
                arOrgAddLTFRB[intOrgFOWSTLTFRBDateExpirationMonth] = $("#selOrgFOWSTTRegExpirationMonth").val();
                arOrgAddLTFRB[intOrgFOWSTLTFRBDateExpirationDay] = $("#selOrgFOWSTTRegExpirationDay").val();
                arOrgAddLTFRB[intOrgFOWSTLTFRBDateExpirationYear] = $("#selOrgFOWSTTRegExpirationYear").val();
                arOrgAddLTFRB[intOrgFOWSTLTFRBState] = (IsNewOrgAddLTFRB ? "New" : "Edit");

                sessionStorage["OrgAddLTFRB-" + intOrgFOWSTLTFRBIndex] = JSON.stringify(arOrgAddLTFRB);

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
                $('#pageOrgFOWSTPrinterIP').hide();
                $('#pageAddDevicePrinter').hide();
                $('#pageOrgFOWSTList').hide();
                LoadOrgFOWSTLTFRBList();
            };

            function SaveOrgFOWSTSection() {

                arOrgAddSection = new Array();
                arOrgAddSection[intOrgFOWSTSecRecID] = $("#txtOrgFOWSTSecID").val();
                arOrgAddSection[intOrgFOWSTSecCode] = $("#txtOrgFOWSTSecCode").val();
                arOrgAddSection[intOrgFOWSTSecName] = $("#txtOrgFOWSTSecName").val();
                arOrgAddSection[intOrgFOWSTSecDescription] = $("#txtOrgFOWSTSecDescription").val();
                arOrgAddSection[intOrgFOWSTSecFloorArea] = $("#txtOrgFOWSTSecFloorArea").val();
                arOrgAddSection[intOrgFOWSTSecSpecialFeatures] = $("#txtOrgFOWSTSecSpecFeatures").val();
                arOrgAddSection[intOrgFOWSTSecState] = (IsNewOrgAddSection ? "New" : "Edit");

                sessionStorage["OrgAddSection-" + intOrgFOWSTSectionIndex] = JSON.stringify(arOrgAddSection);

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
                $('#pageOrgFOWSTPrinterIP').hide();
                $('#pageAddDevicePrinter').hide();
                $('#pageOrgFOWSTList').hide();
                LoadOrgFOWSTSectionList();
            };

            function SaveOrgFOWSTBPermit() {

                arOrgAddBPermit = new Array();
                arOrgAddBPermit[intOrgFOWSTBPermitRecID] = $("#txtOrgFOWSTBPermitRecID").val();
                arOrgAddBPermit[intOrgFOWSTBPermitNo] = $("#txtOrgFOWSTBPermitNo").val();
                arOrgAddBPermit[intOrgFOWSTBPermitBusinessID] = $("#txtOrgFOWSTBPermitID").val();
                arOrgAddBPermit[intOrgFOWSTBPermitTradeName] = $("#txtOrgFOWSTBPermitTradeName").val();
                arOrgAddBPermit[intOrgFOWSTBPermitBuildingHouseNo] = $("#txtOrgFOWSTBPermitBuildingNo").val();
                arOrgAddBPermit[intOrgFOWSTBPermitStreet] = $("#txtOrgFOWSTBPermitStreet").val();
                arOrgAddBPermit[intOrgFOWSTBPermitCityID] = $("#txtOrgFOWSTBPermitCityID").val();
                arOrgAddBPermit[intOrgFOWSTBPermitCity] = $("#btnOrgFOWSTBPermitCity").text();
                arOrgAddBPermit[intOrgFOWSTBPermitCountryID] = $("#txtOrgFOWSTBPermitCountryID").val();
                arOrgAddBPermit[intOrgFOWSTBPermitCountry] = $("#btnOrgFOWSTBPermitCountry").text();
                arOrgAddBPermit[intOrgFOWSTBPermitORNo] = $("#txtOrgFOWSTBPermitORNo").val();
                arOrgAddBPermit[intOrgFOWSTBPermitBusinessPlateNo] = $("#txtOrgFOWSTBPermitBPlateNo").val();
                arOrgAddBPermit[intOrgFOWSTBPermitDatePaymentMonth] = $("#selOrgFOWSTBPermitDateOfPaymentMonth").val();
                arOrgAddBPermit[intOrgFOWSTBPermitDatePaymentDay] = $("#selOrgFOWSTBPermitDateOfPaymentDay").val();
                arOrgAddBPermit[intOrgFOWSTBPermitDatePaymentYear] = $("#selOrgFOWSTBPermitDateOfPaymentYear").val();
                arOrgAddBPermit[intOrgFOWSTBPermitDateIssuedMonth] = $("#selOrgFOWSTBPermitBPlateIssuedMonth").val();
                arOrgAddBPermit[intOrgFOWSTBPermitDateIssuedDay] = $("#selOrgFOWSTBPermitBPlateIssuedDay").val();
                arOrgAddBPermit[intOrgFOWSTBPermitDateIssuedYear] = $("#selOrgFOWSTBPermitBPlateIssuedYear").val();
                arOrgAddBPermit[intOrgFOWSTBPermitState] = (IsNewOrgAddBPermit ? "New" : "Edit");

                sessionStorage["OrgAddBPermit-" + intOrgFOWSTBPermitIndex] = JSON.stringify(arOrgAddBPermit);

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
                $('#pageOrgFOWSTPrinterIP').hide();
                $('#pageAddDevicePrinter').hide();
                $('#pageOrgFOWSTList').hide();
                LoadOrgFOWSTBPermitList();
            };

            function SaveOrgFOWSTTel() {
                var arTmp;

                arOrgAddTel = new Array();
                arOrgAddTel[intOrgFOWSTTelRecID] = $("#txtOrgFOWSTTelID").val();

                arTmp = $("#btnOrgFOWSTTelCountryCode").text().split("-")
                arOrgAddTel[intOrgFOWSTTelCountryID] = $("#txtOrgFOWSTTelCountryID").val();
                arOrgAddTel[intOrgFOWSTTelCountryCode] = $.trim(arTmp[0]);
                arOrgAddTel[intOrgFOWSTTelCallingCode] = $.trim(arTmp[1]);

                arTmp = $("#btnOrgFOWSTTelAreaCode").text().split("-")
                arOrgAddTel[intOrgFOWSTTelAreaCodeID] = $("#txtOrgFOWSTTelAreaCodeID").val();
                arOrgAddTel[intOrgFOWSTTelArea] = $.trim(arTmp[0]);
                arOrgAddTel[intOrgFOWSTTelAreaCode] = $.trim(arTmp[1]);

                arOrgAddTel[intOrgFOWSTTelContactNo] = $("#txtOrgFOWSTTelNo").val();
                arOrgAddTel[intOrgFOWSTTelExtNo] = $("#txtOrgFOWSTTelExtension").val();
                arOrgAddTel[intOrgFOWSTTelContactType] = $("#selOrgFOWSTTelType").val();
                arOrgAddTel[intOrgFOWSTTelState] = (IsNewOrgAddTelContact ? "New" : "Edit");

                sessionStorage["OrgAddTel-" + intOrgFOWSTTelIndex] = JSON.stringify(arOrgAddTel);

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
                $('#pageOrgFOWSTPrinterIP').hide();
                $('#pageAddDevicePrinter').hide();
                $('#pageOrgFOWSTList').hide();
                LoadOrgFOWSTTelList();
            };

            function SaveOrgFOWSTMobile() {
                var arTmp;

                arOrgAddMobile = new Array();

                arOrgAddMobile[intOrgFOWSTMobileRecID] = $("#txtOrgFOWSTMobileID").val();
                arTmp = $("#btnOrgFOWSTMobileCountryCode").text().split("-")
                arOrgAddMobile[intOrgFOWSTMobileCountryID] = $("#txtOrgFOWSTMobileCountryID").val();
                arOrgAddMobile[intOrgFOWSTMobileCountryCode] = $.trim(arTmp[0]);
                arOrgAddMobile[intOrgFOWSTMobileCallingCode] = $.trim(arTmp[1]);

                arTmp = $("#btnOrgFOWSTMobileTelCoPrefixCode").text().split("-")
                arOrgAddMobile[intOrgFOWSTMobileTelCoPrefixID] = $("#txtOrgFOWSTTelCoPrefixID").val();
                arOrgAddMobile[intOrgFOWSTMobilePrefix] = $.trim(arTmp[1]);
                arOrgAddMobile[intOrgFOWSTMobileTelCo] = $.trim(arTmp[0]);

                arOrgAddMobile[intOrgFOWSTMobileContactNo] = $("#txtOrgFOWSTMobileNo").val();
                arOrgAddMobile[intOrgFOWSTMobileContactType] = $("#selOrgAddMobileType").val();
                arOrgAddMobile[intOrgFOWSTMobileState] = (IsNewOrgAddMobileContact ? "New" : "Edit");

                sessionStorage["OrgAddMobile-" + intOrgFOWSTMobileIndex] = JSON.stringify(arOrgAddMobile);

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
                $('#pageOrgFOWSTPrinterIP').hide();
                $('#pageAddDevicePrinter').hide();
                $('#pageOrgFOWSTList').hide();
                LoadOrgFOWSTMobileList();
            };

            function SaveOrgFOWSTEmail() {

                arOrgAddEmail = new Array();
                arOrgAddEmail[intOrgFOWSTEmailRecID] = $("#txtOrgFOWSTEmailID").val();
                arOrgAddEmail[intOrgFOWSTEmailAddress] = $("#txtOrgFOWSTEmail").val();
                arOrgAddEmail[intOrgFOWSTEmailType] = $("#selOrgFOWSTEmailType").val();
                arOrgAddEmail[intOrgFOWSTEmailState] = (IsNewOrgAddEmailContact ? "New" : "Edit");

                sessionStorage["OrgAddEmail-" + intOrgFOWSTEmailIndex] = JSON.stringify(arOrgAddEmail);

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
                $('#pageOrgFOWSTPrinterIP').hide();
                $('#pageAddDevicePrinter').hide();
                $('#pageOrgFOWSTList').hide();
                LoadOrgFOWSTEmailList();
            };

            function RemoveOrgFOWSTLTO() {

                arOrgAddLTO = new Array();
                arOrgAddLTO[intOrgFOWSTLTORecID] = $("#txtOrgFOWSTTRegID").val();
                arOrgAddLTO[intOrgFOWSTLTOORNo] = $("#txtOrgFOWSTTRegORNo").val();
                arOrgAddLTO[intOrgFOWSTLTODateIssuedMonth] = $("#selOrgFOWSTTRegIssuedMonth").val();
                arOrgAddLTO[intOrgFOWSTLTODateIssuedDay] = $("#selOrgFOWSTTRegIssuedDay").val();
                arOrgAddLTO[intOrgFOWSTLTODateIssuedYear] = $("#selOrgFOWSTTRegIssuedYear").val();
                arOrgAddLTO[intOrgFOWSTLTODateExpirationMonth] = $("#selOrgFOWSTTRegExpirationMonth").val();
                arOrgAddLTO[intOrgFOWSTLTODateExpirationDay] = $("#selOrgFOWSTTRegExpirationDay").val();
                arOrgAddLTO[intOrgFOWSTLTODateExpirationYear] = $("#selOrgFOWSTTRegExpirationYear").val();
                arOrgAddLTO[intOrgFOWSTLTOState] = "Remove";

                sessionStorage["OrgAddLTO-" + intOrgFOWSTLTOIndex] = JSON.stringify(arOrgAddLTO);

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
                $('#pageOrgFOWSTPrinterIP').hide();
                $('#pageAddDevicePrinter').hide();
                $('#pageOrgFOWSTList').hide();
                LoadOrgFOWSTLTOList();
            };

            function RemoveOrgFOWSTLTFRB() {

                arOrgAddLTFRB = new Array();
                arOrgAddLTFRB[intOrgFOWSTLTFRBRecID] = $("#txtOrgFOWSTTRegID").val();
                arOrgAddLTFRB[intOrgFOWSTLTFRBORNo] = $("#txtOrgFOWSTTRegORNo").val();
                arOrgAddLTFRB[intOrgFOWSTLTFRBDateIssuedMonth] = $("#selOrgFOWSTTRegIssuedMonth").val();
                arOrgAddLTFRB[intOrgFOWSTLTFRBDateIssuedDay] = $("#selOrgFOWSTTRegIssuedDay").val();
                arOrgAddLTFRB[intOrgFOWSTLTFRBDateIssuedYear] = $("#selOrgFOWSTTRegIssuedYear").val();
                arOrgAddLTFRB[intOrgFOWSTLTFRBDateExpirationMonth] = $("#selOrgFOWSTTRegExpirationMonth").val();
                arOrgAddLTFRB[intOrgFOWSTLTFRBDateExpirationDay] = $("#selOrgFOWSTTRegExpirationDay").val();
                arOrgAddLTFRB[intOrgFOWSTLTFRBDateExpirationYear] = $("#selOrgFOWSTTRegExpirationYear").val();
                arOrgAddLTFRB[intOrgFOWSTLTFRBState] = "Remove";

                sessionStorage["OrgAddLTFRB-" + intOrgFOWSTLTFRBIndex] = JSON.stringify(arOrgAddLTFRB);

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
                $('#pageOrgFOWSTPrinterIP').hide();
                $('#pageAddDevicePrinter').hide();
                $('#pageOrgFOWSTList').hide();
                LoadOrgFOWSTLTFRBList();
            };

            function RemoveOrgFOWSTSection() {

                arOrgAddSection = new Array();
                arOrgAddSection[intOrgFOWSTSecRecID] = $("#txtOrgFOWSTSecID").val();
                arOrgAddSection[intOrgFOWSTSecCode] = $("#txtOrgFOWSTSecCode").val();
                arOrgAddSection[intOrgFOWSTSecName] = $("#txtOrgFOWSTSecName").val();
                arOrgAddSection[intOrgFOWSTSecDescription] = $("#txtOrgFOWSTSecDescription").val();
                arOrgAddSection[intOrgFOWSTSecFloorArea] = $("#txtOrgFOWSTSecFloorArea").val();
                arOrgAddSection[intOrgFOWSTSecSpecialFeatures] = $("#txtOrgFOWSTSecSpecFeatures").val();
                arOrgAddSection[intOrgFOWSTSecState] = "Remove";

                sessionStorage["OrgAddSection-" + intOrgFOWSTSectionIndex] = JSON.stringify(arOrgAddSection);

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
                $('#pageOrgFOWSTPrinterIP').hide();
                $('#pageAddDevicePrinter').hide();
                $('#pageOrgFOWSTList').hide();
                LoadOrgFOWSTSectionList();
            };

            function RemoveOrgFOWSTBPermit() {

                arOrgAddBPermit = new Array();
                arOrgAddBPermit[intOrgFOWSTBPermitRecID] = $("#txtOrgFOWSTBPermitRecID").val();
                arOrgAddBPermit[intOrgFOWSTBPermitNo] = $("#txtOrgFOWSTBPermitNo").val();
                arOrgAddBPermit[intOrgFOWSTBPermitBusinessID] = $("#txtOrgFOWSTBPermitID").val();
                arOrgAddBPermit[intOrgFOWSTBPermitTradeName] = $("#txtOrgFOWSTBPermitTradeName").val();
                arOrgAddBPermit[intOrgFOWSTBPermitBuildingHouseNo] = $("#txtOrgFOWSTBPermitBuildingNo").val();
                arOrgAddBPermit[intOrgFOWSTBPermitStreet] = $("#txtOrgFOWSTBPermitStreet").val();
                arOrgAddBPermit[intOrgFOWSTBPermitCityID] = $("#txtOrgFOWSTBPermitCityID").val();
                arOrgAddBPermit[intOrgFOWSTBPermitCity] = $("#btnOrgFOWSTBPermitCity").text();
                arOrgAddBPermit[intOrgFOWSTBPermitCountryID] = $("#txtOrgFOWSTBPermitCountryID").val();
                arOrgAddBPermit[intOrgFOWSTBPermitCountry] = $("#btnOrgFOWSTBPermitCountry").text();
                arOrgAddBPermit[intOrgFOWSTBPermitORNo] = $("#txtOrgFOWSTBPermitORNo").val();
                arOrgAddBPermit[intOrgFOWSTBPermitBusinessPlateNo] = $("#txtOrgFOWSTBPermitBPlateNo").val();
                arOrgAddBPermit[intOrgFOWSTBPermitDatePaymentMonth] = $("#selOrgFOWSTBPermitDateOfPaymentMonth").val();
                arOrgAddBPermit[intOrgFOWSTBPermitDatePaymentDay] = $("#selOrgFOWSTBPermitDateOfPaymentDay").val();
                arOrgAddBPermit[intOrgFOWSTBPermitDatePaymentYear] = $("#selOrgFOWSTBPermitDateOfPaymentYear").val();
                arOrgAddBPermit[intOrgFOWSTBPermitDateIssuedMonth] = $("#selOrgFOWSTBPermitBPlateIssuedMonth").val();
                arOrgAddBPermit[intOrgFOWSTBPermitDateIssuedDay] = $("#selOrgFOWSTBPermitBPlateIssuedDay").val();
                arOrgAddBPermit[intOrgFOWSTBPermitDateIssuedYear] = $("#selOrgFOWSTBPermitBPlateIssuedYear").val();
                arOrgAddBPermit[intOrgFOWSTBPermitState] = "Remove";

                sessionStorage["OrgAddBPermit-" + intOrgFOWSTBPermitIndex] = JSON.stringify(arOrgAddBPermit);

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
                $('#pageOrgFOWSTPrinterIP').hide();
                $('#pageAddDevicePrinter').hide();
                $('#pageOrgFOWSTList').hide();
                LoadOrgFOWSTBPermitList();
            };

            function RemoveOrgFOWSTTel() {
                var arTmp;

                arOrgAddTel = new Array();
                arOrgAddTel[intOrgFOWSTTelRecID] = $("#txtOrgFOWSTTelID").val();

                arTmp = $("#btnOrgFOWSTTelCountryCode").text().split("-")
                arOrgAddTel[intOrgFOWSTTelCountryID] = $("#txtOrgFOWSTTelCountryID").val();
                arOrgAddTel[intOrgFOWSTTelCountryCode] = $.trim(arTmp[0]);
                arOrgAddTel[intOrgFOWSTTelCallingCode] = $.trim(arTmp[1]);

                arTmp = $("#btnOrgFOWSTTelAreaCode").text().split("-")
                arOrgAddTel[intOrgFOWSTTelAreaCodeID] = $("#txtOrgFOWSTTelAreaCodeID").val();
                arOrgAddTel[intOrgFOWSTTelArea] = $.trim(arTmp[0]);
                arOrgAddTel[intOrgFOWSTTelAreaCode] = $.trim(arTmp[1]);

                arOrgAddTel[intOrgFOWSTTelContactNo] = $("#txtOrgFOWSTTelNo").val();
                arOrgAddTel[intOrgFOWSTTelExtNo] = $("#txtOrgFOWSTTelExtension").val();
                arOrgAddTel[intOrgFOWSTTelContactType] = $("#selOrgFOWSTTelType").val();
                arOrgAddTel[intOrgFOWSTTelState] = "Remove";

                sessionStorage["OrgAddTel-" + intOrgFOWSTTelIndex] = JSON.stringify(arOrgAddTel);

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
                $('#pageOrgFOWSTPrinterIP').hide();
                $('#pageAddDevicePrinter').hide();
                $('#pageOrgFOWSTList').hide();
                LoadOrgFOWSTTelList();
            };

            function RemoveOrgFOWSTMobile() {
                var arTmp;

                arOrgAddMobile = new Array();

                arOrgAddMobile[intOrgFOWSTMobileRecID] = $("#txtOrgFOWSTMobileID").val();
                arTmp = $("#btnOrgFOWSTMobileCountryCode").text().split("-")
                arOrgAddMobile[intOrgFOWSTMobileCountryID] = $("#txtOrgFOWSTMobileCountryID").val();
                arOrgAddMobile[intOrgFOWSTMobileCountryCode] = $.trim(arTmp[0]);
                arOrgAddMobile[intOrgFOWSTMobileCallingCode] = $.trim(arTmp[1]);

                arTmp = $("#btnOrgFOWSTMobileTelCoPrefixCode").text().split("-")
                arOrgAddMobile[intOrgFOWSTMobileTelCoPrefixID] = $("#txtOrgFOWSTTelCoPrefixID").val();
                arOrgAddMobile[intOrgFOWSTMobilePrefix] = $.trim(arTmp[1]);
                arOrgAddMobile[intOrgFOWSTMobileTelCo] = $.trim(arTmp[0]);

                arOrgAddMobile[intOrgFOWSTMobileContactNo] = $("#txtOrgFOWSTMobileNo").val();
                arOrgAddMobile[intOrgFOWSTMobileContactType] = $("#selOrgAddMobileType").val();
                arOrgAddMobile[intOrgFOWSTMobileState] = "Remove";

                sessionStorage["OrgAddMobile-" + intOrgFOWSTMobileIndex] = JSON.stringify(arOrgAddMobile);

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
                $('#pageOrgFOWSTPrinterIP').hide();
                $('#pageAddDevicePrinter').hide();
                $('#pageOrgFOWSTList').hide();
                LoadOrgFOWSTMobileList();
            };

            function RemoveOrgFOWSTEmail() {

                arOrgAddEmail = new Array();
                arOrgAddEmail[intOrgFOWSTEmailRecID] = $("#txtOrgFOWSTEmailID").val();
                arOrgAddEmail[intOrgFOWSTEmailAddress] = $("#txtOrgFOWSTEmail").val();
                arOrgAddEmail[intOrgFOWSTEmailType] = $("#selOrgFOWSTEmailType").val();
                arOrgAddEmail[intOrgFOWSTEmailState] = "Remove";

                sessionStorage["OrgAddEmail-" + intOrgFOWSTEmailIndex] = JSON.stringify(arOrgAddEmail);

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
                $('#pageOrgFOWSTPrinterIP').hide();
                $('#pageAddDevicePrinter').hide();
                $('#pageOrgFOWSTList').hide();
                LoadOrgFOWSTEmailList();
            };

            function LoadOrgFOWSTLTOList() {
                var pLV = $('#lstOrgFOWSTLTO');
                var pItem;

                pLV.html('');
                for (var i = 0; i <= intOrgFOWSTLTOCntr; i++) {

                    if (sessionStorage["OrgAddLTO-" + i] != undefined) {

                        arOrgAddLTO = new Array();
                        arOrgAddLTO = JSON.parse(sessionStorage["OrgAddLTO-" + i]);

                        if (arOrgAddLTO[intOrgFOWSTLTOState] != "Remove") {

                            pItem = $('<li id="lst' + i + '">')
                            .append($('<a>')
                                    .attr('ID', i)
                                    .data('Data', arOrgAddLTO)
                                    .click(function (e) {
                                        e.preventDefault();
                                        ShowOrgAddLTOInfo($(this).data('Data'), $(this).attr('ID'));
                                    })
                                    .append($('<h3>')
                                        .attr("style", "white-space:normal")
                                        .text(arOrgAddLTO[intOrgFOWSTLTOORNo])
                                    )
                                    .append($('<p>')
                                        .attr("style", "white-space:normal")
                                        .text("Date Issued : " + arOrgAddLTO[intOrgFOWSTLTODateIssuedMonth] + "/" + arOrgAddLTO[intOrgFOWSTLTODateIssuedDay] + "/" + arOrgAddLTO[intOrgFOWSTLTODateIssuedYear])
                                    )
                                    .append($('<p>')
                                        .attr("style", "white-space:normal")
                                        .text("Expiration Date : " + arOrgAddLTO[intOrgFOWSTLTODateExpirationMonth] + "/" + arOrgAddLTO[intOrgFOWSTLTODateExpirationDay] + "/" + arOrgAddLTO[intOrgFOWSTLTODateExpirationYear])
                                    )
                            );

                            pLV.append(pItem);

                        }

                    }
                }

                pLV.listview('refresh');
            };

            function LoadOrgFOWSTLTFRBList() {
                var pLV = $('#lstOrgFOWSTLTFRB');
                var pItem;

                pLV.html('');
                for (var i = 0; i <= intOrgFOWSTLTFRBCntr; i++) {

                    if (sessionStorage["OrgAddLTFRB-" + i] != undefined) {

                        arOrgAddLTFRB = new Array();
                        arOrgAddLTFRB = JSON.parse(sessionStorage["OrgAddLTFRB-" + i]);

                        if (arOrgAddLTFRB[intOrgFOWSTLTFRBState] != "Remove") {

                            pItem = $('<li id="lst' + i + '">')
                            .append($('<a>')
                                    .attr('ID', i)
                                    .data('Data', arOrgAddLTFRB)
                                    .click(function (e) {
                                        e.preventDefault();
                                        ShowOrgAddLTFRBInfo($(this).data('Data'), $(this).attr('ID'));
                                    })
                                    .append($('<h3>')
                                        .attr("style", "white-space:normal")
                                        .text(arOrgAddLTFRB[intOrgFOWSTLTFRBORNo])
                                    )
                                    .append($('<p>')
                                        .attr("style", "white-space:normal")
                                        .text("Date Issued : " + arOrgAddLTFRB[intOrgFOWSTLTFRBDateIssuedMonth] + "/" + arOrgAddLTFRB[intOrgFOWSTLTFRBDateIssuedDay] + "/" + arOrgAddLTFRB[intOrgFOWSTLTFRBDateIssuedYear])
                                    )
                                    .append($('<p>')
                                        .attr("style", "white-space:normal")
                                        .text("Expiration Date : " + arOrgAddLTFRB[intOrgFOWSTLTFRBDateExpirationMonth] + "/" + arOrgAddLTFRB[intOrgFOWSTLTFRBDateExpirationDay] + "/" + arOrgAddLTFRB[intOrgFOWSTLTFRBDateExpirationYear])
                                    )
                            );

                            pLV.append(pItem);

                        }

                    }
                }

                pLV.listview('refresh');
            };

            function LoadOrgFOWSTSectionList() {
                var pLV = $('#lstOrgFOWSTSection');
                var pItem;

                pLV.html('');
                for (var i = 0; i <= intOrgFOWSTSectionCntr; i++) {

                    if (sessionStorage["OrgAddSection-" + i] != undefined) {

                        arOrgAddSection = new Array();
                        arOrgAddSection = JSON.parse(sessionStorage["OrgAddSection-" + i]);

                        if (arOrgAddSection[intOrgFOWSTSecState] != "Remove") {

                            pItem = $('<li id="lst' + i + '">')
                            .append($('<a>')
                                    .attr('ID', i)
                                    .data('Data', arOrgAddSection)
                                    .click(function (e) {
                                        e.preventDefault();
                                        ShowOrgAddSectionInfo($(this).data('Data'), $(this).attr('ID'));
                                    })
                                    .append($('<h3>')
                                        .attr("style", "white-space:normal")
                                        .text(arOrgAddSection[intOrgFOWSTSecName])
                                    )
                            );

                            pLV.append(pItem);

                        }

                    }
                }

                pLV.listview('refresh');
            };

            function LoadOrgFOWSTBPermitList() {
                var pLV = $('#lstOrgFOWSTBusinessPermit');
                var pItem;

                pLV.html('');
                for (var i = 0; i <= intOrgFOWSTBPermitCntr; i++) {

                    if (sessionStorage["OrgAddBPermit-" + i] != undefined) {

                        arOrgAddBPermit = new Array();
                        arOrgAddBPermit = JSON.parse(sessionStorage["OrgAddBPermit-" + i]);

                        if (arOrgAddBPermit[intOrgFOWSTBPermitState] != "Remove") {

                            pItem = $('<li id="lst' + i + '">')
                            .append($('<a>')
                                    .attr('ID', i)
                                    .data('Data', arOrgAddBPermit)
                                    .click(function (e) {
                                        e.preventDefault();
                                        ShowOrgAddBPermitInfo($(this).data('Data'), $(this).attr('ID'));
                                    })
                                    .append($('<h3>')
                                        .attr("style", "white-space:normal")
                                        .text(arOrgAddBPermit[intOrgFOWSTBPermitTradeName])
                                    )
                                    .append($('<p>')
                                        .attr("style", "white-space:normal")
                                        .text("Business Permit No. : " + arOrgAddBPermit[intOrgFOWSTBPermitNo])
                                    )
                                    .append($('<p>')
                                        .attr("style", "white-space:normal")
                                        .text("Date Issued : " + arOrgAddBPermit[intOrgFOWSTBPermitDateIssuedMonth] + "/" + arOrgAddBPermit[intOrgFOWSTBPermitDateIssuedDay] + "/" + arOrgAddBPermit[intOrgFOWSTBPermitDateIssuedYear])
                                    )
                            );

                            pLV.append(pItem);

                        }

                    }
                }

                pLV.listview('refresh');
            };

            function LoadOrgFOWSTTelList() {
                var pLV = $('#lstOrgFOWSTTelephone');
                var pItem;

                pLV.html('');
                for (var i = 0; i <= intOrgFOWSTTelCntr; i++) {

                    if (sessionStorage["OrgAddTel-" + i] != undefined) {

                        arOrgAddTel = new Array();
                        arOrgAddTel = JSON.parse(sessionStorage["OrgAddTel-" + i]);

                        if (arOrgAddTel[intOrgFOWSTTelState] != "Remove") {

                            pItem = $('<li id="lst' + i + '">')
                            .append($('<a>')
                                    .attr('ID', i)
                                    .data('Data', arOrgAddTel)
                                    .click(function (e) {
                                        e.preventDefault();
                                        ShowOrgAddTelInfo($(this).data('Data'), $(this).attr('ID'));
                                    })
                                    .append($('<h3>')
                                        .attr("style", "white-space:normal")
                                        .text("+" + arOrgAddTel[intOrgFOWSTTelCallingCode] + "." + arOrgAddTel[intOrgFOWSTTelAreaCode] + '.' + arOrgAddTel[intOrgFOWSTTelContactNo] + ((arOrgAddTel[intOrgFOWSTTelExtNo] || '') == '' ? '' : ", Ext. " + arOrgAddTel[intOrgFOWSTTelExtNo]))
                                    )
                                    .append($('<p>')
                                        .attr("style", "white-space:normal")
                                        .text(arOrgAddTel[intOrgFOWSTTelArea] + ", " + arOrgAddTel[intOrgFOWSTTelCountryCode])
                                    )
                                    .append($('<p>')
                                        .attr("style", "white-space:normal")
                                        .text(arOrgAddTel[intOrgFOWSTTelContactType])
                                    )
                            );

                            pLV.append(pItem);

                        }

                    }
                }

                pLV.listview('refresh');
            };

            function LoadOrgFOWSTMobileList() {
                var pLV = $('#lstOrgFOWSTMobile');
                var pItem;

                pLV.html('');
                for (var i = 0; i <= intOrgFOWSTMobileCntr; i++) {

                    if (sessionStorage["OrgAddMobile-" + i] != undefined) {

                        arOrgAddMobile = new Array();
                        arOrgAddMobile = JSON.parse(sessionStorage["OrgAddMobile-" + i]);

                        if (arOrgAddMobile[intOrgFOWSTMobileState] != "Remove") {

                            pItem = $('<li id="lst' + i + '">')
                            .append($('<a>')
                                    .attr('ID', i)
                                    .data('Data', arOrgAddMobile)
                                    .click(function (e) {
                                        e.preventDefault();
                                        ShowOrgAddMobileInfo($(this).data('Data'), $(this).attr('ID'));
                                    })
                                    .append($('<h3>')
                                        .attr("style", "white-space:normal")
                                        .text("+" + arOrgAddMobile[intOrgFOWSTMobileCallingCode] + "." + arOrgAddMobile[intOrgFOWSTMobilePrefix] + "." + arOrgAddMobile[intOrgFOWSTMobileContactNo])
                                    )
                                    .append($('<p>')
                                        .attr("style", "white-space:normal")
                                        .text(arOrgAddMobile[intOrgFOWSTMobileTelCo] + " (" + arOrgAddMobile[intOrgFOWSTMobileCountryCode] + ")")
                                    )
                                    .append($('<p>')
                                        .attr("style", "white-space:normal")
                                        .text(arOrgAddMobile[intOrgFOWSTMobileContactType])
                                    )
                            );


                            pLV.append(pItem);

                        }

                    }
                }

                pLV.listview('refresh');
            };


            function LoadOrgFOWSTEmailList() {
                var pLV = $('#lstOrgFOWSTEmail');
                var pItem;

                pLV.html('');
                for (var i = 0; i <= intOrgFOWSTEmailCntr; i++) {

                    if (sessionStorage["OrgAddEmail-" + i] != undefined) {

                        arOrgAddEmail = new Array();
                        arOrgAddEmail = JSON.parse(sessionStorage["OrgAddEmail-" + i]);

                        if (arOrgAddEmail[intOrgFOWSTEmailState] != "Remove") {

                            pItem = $('<li id="lst' + i + '">')
                            .append($('<a>')
                                    .attr('ID', i)
                                    .data('Data', arOrgAddEmail)
                                    .click(function (e) {
                                        e.preventDefault();
                                        ShowOrgAddEmailInfo($(this).data('Data'), $(this).attr('ID'));
                                    })
                                    .append($('<h3>')
                                        .attr("style", "white-space:normal")
                                        .text(arOrgAddEmail[intOrgFOWSTEmailAddress])
                                    )
                                    .append($('<p>')
                                        .attr("style", "white-space:normal")
                                        .text(arOrgAddEmail[intOrgFOWSTEmailType])
                                    )
                            );


                            pLV.append(pItem);

                        }

                    }
                }

                pLV.listview('refresh');
            };


        });
    });
