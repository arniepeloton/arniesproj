require(['common',
    (OfflineMode ? '/Scripts3/SalesMenu_offline.js?crc=' : '')
], function () {

    $(function () {

        var result;

        //        SetPOSMode(true);

        window.pageCustomerDetails = {};
        // popSalesMenuOrderTaker.OnSubmit = function() {};

        // 2017_06_14__09_31_49_2347 // ki-pang-comment para paspas mu load
        if (window.location.pathname.toLowerCase() != '/salesmenu/bsposviewall') {
            AjaxSend({
                ExactUrl: '/SalesMenu/BsCustomerDetails?cache=1',
                type: 'GET',
                dataType: 'text',
                success: function (vData) {
                    if (CBool(sessionStorage.IsWebPos)) {
                        vData = $('<div>').append(vData).find('#popupAddNewCustomer');
                        $('body').append(vData); //.trigger('create');
                        $('#popupAddNewCustomer').modal('show');
                    } else {
                        $('#pageCustomerOptions').append(vData); //.trigger('create');
                    }
                    
                    Events();
                    App.Deferred.fulfill('FQ26E5TP76');
                }
            });
        }

        window.pageCustomerDetails.Show = function () {

            if (window.location.pathname.toLowerCase() == '/salesmenu/bsposviewall')
                window.pageHeader.LoadCustomer(function () {
                    Events();
                });
            else
                //LoadCustomer();
                if (proj.IsBootstrapDesign()) {
                    $('#pageCustomerOptions').pageshow();
                } else 
                    $.mobile.changePage('#pageCustomerOptions');

            setTimeout(function () { $('#lnkTotalNumberOfCustomer').val(CStr2(sessionStorage.TotalNumberOfCustomer) == 'undefined' ? '' : sessionStorage.TotalNumberOfCustomer); }, 500);
            
        }; // END Show

        function Events() {
            getCityName();
            getCountryName();

            $('#btnOkCustomer').live('click', function () {
                if ((CStr2(sessionStorage.CartOrderID) != '') &&
                         (sessionStorage.DeliveryMethod != '' || sessionStorage.OrderMethod != '' ||
                                sessionStorage.Gender != '' || sessionStorage.Age != '')) {
                    var pData = {
                        ID: sessionStorage.CartOrderID,
                        DeliveryMethod: sessionStorage.DeliveryMethod,
                        OrderMethod: sessionStorage.OrderMethod,
                        Gender: sessionStorage.Gender,
                        Age: sessionStorage.Age
                    };
                    
                    if (OfflineMode) {
                        if (SalesPayment_offline().SaveCustomerDemographics(pData)) {
                            goBack();
                        }
                    } else {
                        SaveCustomerDemographics(pData);
                    }
                } else {
                    goBack();
                }
            });

            $('#btnCancelCustomer').live('click', function () {
                goBack();
            });

            $('#btnClient').live('click', function () {
                function BrowseCustomer() {
                    proj.BrowseCustomer({
                        Edit: EditCustomer
                    });
                }

                function EditCustomer(customerID) {
                    $('#btnAddCustomer').click();
                    sessionStorage.IsAddAsNewCustomer = false;
                    sessionStorage.CustomerOrganizationID = customerID;
                    var vData = Business.LibraryOrganization.ByID(customerID);

                    if (CBool(vData.IsOrganization)) {
                        $('#selCustomerType').val('Organization').prop('disabled', 1).change();
                        $('#txtCompanyName').val(vData.CompanyName);
                        $('#txtCustomerTIN').val(vData.TinNumber);
                        $('#txtCustomerOrgHouseNo').val(vData.OfficeAddressBuildingNo);
                        $('#txtCustomerOrgAddress').val(vData.OfficeAddressStreet);
                        $('#selOrgCity').val(vData.OfficeAddressCityID);
                        $('#selOrgCountry').val(vData.OfficeAddressCountryID);
                    } else {
                        $('#selCustomerType').val('Person').prop('disabled', 1).change();
                        $('#txtCustomerPersonFirstName').val(vData.PersonFirstName);
                        $('#txtCustomerPersonMiddleName').val(vData.PersonMiddleName);
                        $('#txtCustomerPersonSurname').val(vData.PersonSurname);
                        $('#txtCustomerPersonTIN').val(vData.TinNumber);
                    }
                }

                if (sessionStorage.CustomerOrganizationID.toLowerCase() == 'cd71a0f7-08a9-4601-876e-5f13c064f873' || sessionStorage.CustomerOrganizationID.toLowerCase() == '') {
                    //Default Customer: [CASH PAYMENT]
                    BrowseCustomer();
                } else {
                    PopupButtonList('', 'Options', [
                        {
                            Text: 'Choose other customer',
                            Click: function () {
                                BrowseCustomer();
                            }
                        }, 
                        {
                            Text: 'Edit Details',
                            Click: function () {
                                EditCustomer(sessionStorage.CustomerOrganizationID);
                            }
                        }
                    ])
                }
                
            });

            $('#btnTotalNumberOfCustomer').live('click', function () {
                $('#popupTotalNumberOfCustomer').popup('open');
                $('#txtTotalNumberOfCustomer').focus();
            });

            $('#btnAddCustomer').live('click', function () {
                sessionStorage.IsAddAsNewCustomer = true;
                $('#selCustomerType').val('Organization').removeAttr('disabled')
                $('#popupCustomerOptions').popup('close');
                $('#popupAddNewCustomer').modal('toggle');
                $('#txtCompanyName').focus();
            });

            $('#selCustomerType').live('change', function (e) {
                e.preventDefault();

                if ($('#selCustomerType').val() == 'Organization') {
                    $('#divCustomerOrg').show();
                    $('#divCustomerPerson').hide();
                } else {
                    $('#divCustomerOrg').hide();
                    $('#divCustomerPerson').show();
                }

            });

            $('#btnSaveTotalNumberOfCustomer').live('click', function () {
                if (window.location.pathname.toLowerCase() != '/tableview/bsnewtable') { //WALAY LABOT ANG NEW TABLE VIEW UI by arnie 12/1/2020
                    sessionStorage.TotalNumberOfCustomer = $('#txtTotalNumberOfCustomer').val();
                    $('#popupTotalNumberOfCustomer').popup('close');
                    $('#lnkTotalNumberOfCustomer').val(CStr2($('#txtTotalNumberOfCustomer').val()));
                }                
            });

            $('#btnCustomerNameSave').live('click', function () {
                if (CBool(sessionStorage.IsAddAsNewCustomer)) sessionStorage.CustomerOrganizationID = '';

                if ($('#selCustomerType').val() == 'Organization') {
                    var str = $.trim($('#txtCompanyName').val())

                    //gi comment para ang initials sa company name dili himuon ug 'proper case'

                    //str = str.toLowerCase().replace(/\b[a-z]/g, function (letter) {
                    //    return letter.toUpperCase();
                    //});

                    if (str == "" || str.length <= 1) {
                        if (OfflineMode)
                            Android.Toast("Company Name is required");

                        return;
                    }
                    if ($.trim($('#txtCustomerTIN').val()) == "" || $.trim($('#txtCustomerTIN').val()).length <= 1) {
                        if (OfflineMode)
                            Android.Toast("Company TIN is required");

                        return;
                    }

                    if ($.trim($('#txtCompanyBusinessStyle').val()) == "" || $.trim($('#txtCompanyBusinessStyle').val()).length <= 1) {
                        if (OfflineMode)
                            Android.Toast("Company Business Style is required");

                        return;
                    }

                    //                        $('#btnCustomerNameSave').attr('disabled', true);

                    var pDataTradeName = [];
                    var pDataCategory = [];
                    var pDataOrg = {
                        IsOrganization: 1,
                        OrganizationID: sessionStorage.CustomerOrganizationID,
                        PersonTitleID: '',
                        PersonDOB: '',
                        CompanyName: str,
                        TIN: $.trim($('#txtCustomerTIN').val()),
                        CompanyBusinessStyle: $.trim($('#txtCompanyBusinessStyle').val()),
                        OrgTradeNameList: pDataTradeName,
                        OrgCategoryList: pDataCategory,
                        IsAddAsCustomer: 1,
                        CompanyNameInitial: '',
                        IsFranchisor: 0,
                        OfficeAddressBuildingNo: $.trim($('#txtCustomerOrgHouseNo').val()),
                        OfficeAddressStreet: $.trim($('#txtCustomerOrgAddress').val()), //temporarily store the free type address here
                        OfficeAddressCityID: $('#selOrgCity').val(),
                        OfficeAddressCountryID: $('#selOrgCountry').val(),
                        BusinessAddressBuildingNo: $.trim($('#txtCustomerOrgHouseNo').val()),
                        BusinessAddressStreet: $.trim($('#txtCustomerOrgAddress').val()),
                        BusinessAddressCityID: $('#selOrgCity').val(),
                        BusinessAddressCountryID: $('#selOrgCountry').val()
                    };


                    if (OfflineMode) {
                        result = SalesPayment_offline().SaveOrganizationInfo(pDataOrg);
                        if (result != 'Existing') {
                            ClearCustomerFreeTypeSession();
                            $('#popupAddNewCustomer').modal('toggle');

                            sessionStorage.CustomerOrganizationID = result;
                            pageCustomerDetails.LoadCustomer();
                            SalesMenu_offline().SaveCustomer(result);
                            // NewMessageBox('Successfully saved customer details. Select the customer by clicking the Customer Record Lookup button.', 'Save Customer Details');

                        } else {
                            NewMessageBox("Organization with the same corporate name already exists.", "Organization Info");
                        }

                    } else {
                        var pUrl = '/OrganizationShort/CheckOrgUniqueOrgName';
                        pUrl = MergeUrlParameters({}, pUrl, proj.RemoteServer);
                        AjaxSend({
                            url: pUrl,
                            data: {
                                OrganizationID: '',
                                CompanyName: str
                            },
                            success: function (vData) {

                                if (vData.OrganizationData.Rows.length > 0) {
                                    NewMessageBox("Organization with the same corporate name already exists.", "Organization Info");

                                } else {
                                    var pUrl = '/OrganizationShort/SaveOrganizationInfo';
                                    pUrl = MergeUrlParameters({}, pUrl, proj.RemoteServer);
                                    AjaxSend({
                                        url: pUrl,
                                        data: pDataOrg,
                                        success: function (vSaveData) {

                                            ClearCustomerFreeTypeSession();
                                            $('#popupAddNewCustomer').modal('toggle');
                                            sessionStorage.CustomerOrganizationID = vSaveData.OrganizationData;
                                            pageCustomerDetails.LoadCustomer();
                                            // NewMessageBox('Successfully saved customer details. Select the customer by clicking the Customer Record Lookup button.', 'Save Customer Details');

                                        }
                                    });

                                }

                                //                                    $('#btnCustomerNameSave').removeAttr('disabled');
                            }
                        });

                    }
                } else {

                    var firstname = $.trim($('#txtCustomerPersonFirstName').val())
                    firstname = firstname.toLowerCase().replace(/\b[a-z]/g, function (letter) {
                        return letter.toUpperCase();
                    });
                    var middlename = $.trim($('#txtCustomerPersonMiddleName').val())
                    middlename = middlename.toLowerCase().replace(/\b[a-z]/g, function (letter) {
                        return letter.toUpperCase();
                    });
                    var surname = $.trim($('#txtCustomerPersonSurname').val())
                    surname = surname.toLowerCase().replace(/\b[a-z]/g, function (letter) {
                        return letter.toUpperCase();
                    });


                    if (firstname == "" || firstname.length <= 1) {
                        if (OfflineMode)
                            Android.Toast("First name is required");

                        return;
                    }
                    if (middlename == "" || middlename.length <= 1) {
                        if (OfflineMode)
                            Android.Toast("Full middle name is required");

                        return;
                    }
                    if (surname == "" || surname.length <= 1) {
                        if (OfflineMode)
                            Android.Toast("Surname is required");

                        return;
                    }


                    //                        $('#btnCustomerNameSave').attr('disabled', true);
                    var pDataTradeName = [];
                    var pDataCategory = [];
                    var pDataPerson = {
                        IsOrganization: 0,
                        OrganizationID: sessionStorage.CustomerOrganizationID,
                        PersonFirstName: firstname,
                        PersonMiddleName: middlename,
                        PersonSurname: surname,
                        PersonSuffixName: '',
                        PersonTitleID: '',
                        PersonDOB: '',
                        PersonMotherMaidenSurname: '',
                        PersonMotherMaidenFirstName: '',
                        PersonMotherMaidenMiddleName: '',
                        OrgTradeNameList: pDataTradeName,
                        OrgCategoryList: pDataCategory,
                        IsAddAsCustomer: 1,
                        CompanyNameInitial: '',
                        IsFranchisor: 0,
                        TIN: $.trim($('#txtCustomerPersonTIN').val()),
                        OfficeAddressBuildingNo: '',
                        OfficeAddressStreet: '',
                        OfficeAddressCityID: '',
                        OfficeAddressCountryID: '',
                        BusinessAddressBuildingNo: '',
                        BusinessAddressStreet: '',
                        BusinessAddressCityID: '',
                        BusinessAddressCountryID: ''
                    };

                    if (OfflineMode) {
                        result = SalesPayment_offline().SaveOrganizationInfo(pDataPerson);
                        if (result != 'Existing') {
                            ClearCustomerFreeTypeSession();
                            $('#popupAddNewCustomer').modal('toggle');
                            sessionStorage.CustomerOrganizationID = result;
                            pageCustomerDetails.LoadCustomer();
                            SalesMenu_offline().SaveCustomer(result);
                            //                                NewMessageBox('Successfully saved customer details. Select the customer by clicking the Customer Record Lookup button.', 'Save Customer Details');

                        } else {
                            NewMessageBox("Person with the same information already exists.", "Person Info");

                        }
                    } else {
                        AjaxSend({
                            url: "OrganizationShort/CheckPersonUniqueName",
                            data: {
                                IsOrganization: 0,
                                OrganizationID: '',
                                PersonFirstName: firstname,
                                PersonMiddleName: middlename,
                                PersonSurname: surname,
                                PersonSuffixName: '',
                                PersonTitleID: '',
                                PersonDOB: '',
                                PersonMotherMaidenSurname: '',
                                PersonMotherMaidenFirstName: '',
                                PersonMotherMaidenMiddleName: '',
                                IsFull: false

                            },
                            success: function (vData) {

                                if (vData.OrganizationData.Rows.length > 0) {
                                    NewMessageBox("Person with the same information already exists.", "Person Info");

                                } else {
                                    AjaxSend({
                                        url: 'OrganizationShort/SaveOrganizationInfo',
                                        data: pDataPerson,
                                        success: function (vSaveData) {

                                            ClearCustomerFreeTypeSession();
                                            $('#popupAddNewCustomer').modal('toggle');
                                            sessionStorage.CustomerOrganizationID = vSaveData.OrganizationData;
                                            pageCustomerDetails.LoadCustomer();
                                            //                                                NewMessageBox('Successfully saved customer details. Select the customer by clicking the Customer Record Lookup button.', 'Save Customer Details');

                                        }
                                    });

                                }

                                //                                    $('#btnCustomerNameSave').removeAttr('disabled');
                            }
                        });
                    }
                }
            });

            // TABLE NO ONCHANGE
            $('#lnkTotalNumberOfCustomer').on('keyup', function () {

                //$('#lblCustomerName').text($(this).val());

                if (CStr2($(this).val()) != '') {
                    sessionStorage.GasPumpNo = '';
                    $('#selGasPumpNo').val('');//.selectmenu('refresh', true);

                    sessionStorage.CustomerTableNo = $(this).val();
                    //$('#lblCustomerName').text(sessionStorage.CustomerTableNo);
                    $('#lblLabel').html('Customer / Table No.:<br/>');
                } else {
                    sessionStorage.CustomerTableNo = '';
                    //$('#lblCustomerName').text(sessionStorage.CustomerOrganizationName);
                    $('#lblLabel').html('<img alt="Customer" src="/Content/images/Customer.png" width="30px" align="center" /><br/>');

                }

                return;
            });
            $('#selGasPumpNo').live('change', function () {

                $('#lblCustomerName').text($(this).val());

                if (CStr2($(this).val()) != '') {
                    sessionStorage.CustomerTableNo = '';
                    $('#selTableNo').val('');//.selectmenu('refresh', true);

                    sessionStorage.GasPumpNo = $(this).val();
                    $('#lblCustomerName').text(sessionStorage.GasPumpNo);
                    $('#lblLabel').html('Gas Pump No.:<br/>');

                } else {
                    sessionStorage.GasPumpNo = '';
                    $('#lblCustomerName').text(sessionStorage.CustomerOrganizationName);
                    $('#lblLabel').html('<img alt="Customer" src="/Content/images/Customer.png" width="30px" align="center" /><br/>');

                }

                return;
            });

            $('#selDeliveryMethod').change(function () {
                var val = $(this).val();

                if (val == 'Dine In' && proj.CanChangeTableNumber()) {
                    window.pageCustomerDetails.SetPOSMode(true);
                }

                sessionStorage.DeliveryMethod = val;
            })

            $('#selOrderMethod').change(function (e) {
                sessionStorage.OrderMethod = $(this).val();
            })

            $('#selGender').change(function (e) {
                sessionStorage.Gender = $(this).val();
            })

            $('#selAge').change(function (e) {
                sessionStorage.Age = $(this).val();
            })

            $('#btnTableNoSave').live('click', function () {
                if (IsMobile) {
                    var pTableNum = sessionStorage.CustomerTableNo;
                    if (CStr2(pTableNum).trim() != '') {
                        if (pTableNum.indexOf('_') == -1)
                            pTableNum = 'TBL_' + pTableNum;
                        if (IsGuid(sessionStorage.CartOrderID))
                            Business.TransInvoice.UpdateTableNum(sessionStorage.CartOrderID, pTableNum);
                    }
                }
                $('#popupTableNo').modal('toggle');
            });

            $('#btnDineIn').live('click', function () {
                selectDineIn(true);
            });

            $('#btnTakeOut').live('click', function () {
                selectTakeOut();
            });

            $('#btnDelivery').live('click', function () {
                selectDelivery();            });

            $('#btnPickUp').live('click', function () {
                selectPickUp();
            });

            $('#btnWalkIn').live('click', function () {
                selectWalkIn();
            });

            $('#btnPhoneIn').live('click', function () {
                selectPhoneIn();

            });

            $('#btnTextIn').live('click', function () {
                selectTextIn();
            });

            $('#btnOnlineOrder').live('click', function () {
                selectOnlineOrder();
            });

            $('#btnBoy').live('click', function () {
                selectBoy();
            });

            $('#btnGirl').live('click', function () {
                selectGirl();
            });

            $('#btn3rdGender').live('click', function () {
                select3rdGender();
            });

            $('#btnFamily').live('click', function () {
                selectFamily();
            });

            $('#btnGroup').live('click', function () {
                selectGroup();
            });

            $('#btnNA').live('click', function () {
                selectNA();
            });

            $('#btnKid').live('click', function () {
                selectKid();
            });

            $('#btnTeenager').live('click', function () {
                selectTeenager();
            });

            $('#btn20s').live('click', function () {
                select20s();
            });

            $('#btn30s').live('click', function () {
                select30s();
            });

            $('#btn40s').live('click', function () {
                select40s();
            });

            $('#btn50s').live('click', function () {
                select50s();

            });

            $('#btn60sAndUp').live('click', function () {
                select60sandUp();

            });

            $('#btnMixedAgesNA').live('click', function () {
                selectMixedAgesNA();

            });
        }        

        function goBack() {
            if (window.location.pathname.toLowerCase() == '/salesmenu/bsposviewall'){
                window.pageHeader.LoadHotButtons();
                if (window.HotButtons)
                    window.HotButtons.LoadCustomerHotButtons();
            } else if (window.location.pathname.toLowerCase() == '/salesmenu/fmcgbasic') {
                $('#pageCart').pageshow()
                $('[name="txtQtyPlu"]').focus();
            } else {
                //history.back();
                if (sessionStorage.PosMode == 'Hot Button Mode') {
                    $('#pageHotButton').pageshow();
                    $('#pageHotButton').css('top', $('#divHotButtonHeader').height() + 'px');
                } else {
                    $('#pageMain').pageshow();
                }                
            }
        }

        //additional by janine customer demographics
        //DELIVERY METHOD

        function selectDineIn(isClicked) {
            $('#btnDineIn').buttonMarkup({ theme: "a" });
            $('#btnDineIn').attr("dotoggle", "1");

            sessionStorage.DeliveryMethod = 'Dine In';

            if (isClicked && proj.CanChangeTableNumber()) {
                //                if (CDec2(sessionStorage.CustomerTableNo) > 0)
                //                    $('#popupTableNo').modal('toggle');
                //                else
                window.pageCustomerDetails.SetPOSMode(true);
            }

            $('#btnDelivery').buttonMarkup({ theme: "d" });
            $('#btnDelivery').attr("dotoggle", "0");

            $('#btnPickUp').buttonMarkup({ theme: "d" });
            $('#btnPickUp').attr("dotoggle", "0");

            $('#btnTakeOut').buttonMarkup({ theme: "d" });
            $('#btnTakeOut').attr("dotoggle", "0");
        }

        function selectTakeOut() {
            $('#btnTakeOut').buttonMarkup({ theme: "a" });
            $('#btnTakeOut').attr("dotoggle", "1");

            sessionStorage.DeliveryMethod = 'Take Out';

            $('#btnDelivery').buttonMarkup({ theme: "d" });
            $('#btnDelivery').attr("dotoggle", "0");

            $('#btnPickUp').buttonMarkup({ theme: "d" });
            $('#btnPickUp').attr("dotoggle", "0");

            $('#btnDineIn').buttonMarkup({ theme: "d" });
            $('#btnDineIn').attr("dotoggle", "0");
        }

        function selectDelivery() {
            $('#btnDelivery').buttonMarkup({ theme: "a" });
            $('#btnDelivery').attr("dotoggle", "1");

            sessionStorage.DeliveryMethod = 'Deliver (Downtown)';

            $('#btnDineIn').buttonMarkup({ theme: "d" });
            $('#btnDineIn').attr("dotoggle", "0");

            $('#btnPickUp').buttonMarkup({ theme: "d" });
            $('#btnPickUp').attr("dotoggle", "0");

            $('#btnTakeOut').buttonMarkup({ theme: "d" });
            $('#btnTakeOut').attr("dotoggle", "0");
        }

        function selectPickUp() {
            $('#btnPickUp').buttonMarkup({ theme: "a" });
            $('#btnPickUp').attr("dotoggle", "1");

            sessionStorage.DeliveryMethod = 'PickUp';

            $('#btnDineIn').buttonMarkup({ theme: "d" });
            $('#btnDineIn').attr("dotoggle", "0");

            $('#btnDelivery').buttonMarkup({ theme: "d" });
            $('#btnDelivery').attr("dotoggle", "0");

            $('#btnTakeOut').buttonMarkup({ theme: "d" });
            $('#btnTakeOut').attr("dotoggle", "0");
        }

        switch (sessionStorage.DeliveryMethod) {
            case 'Dine In':
                selectDineIn(false);
                break;
            case 'Take Out':
                selectTakeOut();
                break;
            case 'Deliver (Downtown)':
                selectDelivery();
                break;
            case 'PickUp':
                selectPickUp();
                break;
            default:
                selectPickUp();
                break;
        }

        //ORDER METHOD

        function selectWalkIn() {
            $('#btnWalkIn').buttonMarkup({ theme: "a" });
            $('#btnWalkIn').attr("dotoggle", "1");

            sessionStorage.OrderMethod = 'Walk In';

            $('#btnPhoneIn').buttonMarkup({ theme: "d" });
            $('#btnPhoneIn').attr("dotoggle", "0");

            $('#btnTextIn').buttonMarkup({ theme: "d" });
            $('#btnTextIn').attr("dotoggle", "0");

            $('#btnOnlineOrder').buttonMarkup({ theme: "d" });
            $('#btnOnlineOrder').attr("dotoggle", "0");
        }

        function selectPhoneIn() {
            $('#btnPhoneIn').buttonMarkup({ theme: "a" });
            $('#btnPhoneIn').attr("dotoggle", "1");

            sessionStorage.OrderMethod = 'Phone-In';

            $('#btnWalkIn').buttonMarkup({ theme: "d" });
            $('#btnWalkIn').attr("dotoggle", "0");

            $('#btnTextIn').buttonMarkup({ theme: "d" });
            $('#btnTextIn').attr("dotoggle", "0");

            $('#btnOnlineOrder').buttonMarkup({ theme: "d" });
            $('#btnOnlineOrder').attr("dotoggle", "0");
        }

        function selectTextIn() {
            $('#btnTextIn').buttonMarkup({ theme: "a" });
            $('#btnTextIn').attr("dotoggle", "1");

            sessionStorage.OrderMethod = 'Text-In';

            $('#btnPhoneIn').buttonMarkup({ theme: "d" });
            $('#btnPhoneIn').attr("dotoggle", "0");

            $('#btnWalkIn').buttonMarkup({ theme: "d" });
            $('#btnWalkIn').attr("dotoggle", "0");

            $('#btnOnlineOrder').buttonMarkup({ theme: "d" });
            $('#btnOnlineOrder').attr("dotoggle", "0");
        }

        function selectOnlineOrder() {
            $('#btnOnlineOrder').buttonMarkup({ theme: "a" });
            $('#btnOnlineOrder').attr("dotoggle", "1");

            sessionStorage.OrderMethod = 'Online Order';

            $('#btnPhoneIn').buttonMarkup({ theme: "d" });
            $('#btnPhoneIn').attr("dotoggle", "0");

            $('#btnWalkIn').buttonMarkup({ theme: "d" });
            $('#btnWalkIn').attr("dotoggle", "0");

            $('#btnTextIn').buttonMarkup({ theme: "d" });
            $('#btnTextIn').attr("dotoggle", "0");
        }

        switch (sessionStorage.OrderMethod) {
            case 'Walk In':
                selectWalkIn();
                break;
            case 'Phone-In':
                selectPhoneIn();
                break;
            case 'Text-In':
                selectTextIn();
                break;
            case 'Online Order':
                selectOnlineOrder();
                break;
            default:
                selectWalkIn();
                break;
        }

        //CUSTOMER DEMOGRAPHICS

        function selectBoy() {
            $('#btnBoy').buttonMarkup({ theme: "a" });
            $('#btnBoy').attr("dotoggle", "1");

            sessionStorage.Gender = 'Boy';

            $('#btnGirl').buttonMarkup({ theme: "d" });
            $('#btnGirl').attr("dotoggle", "0");

            $('#btn3rdGender').buttonMarkup({ theme: "d" });
            $('#btn3rdGender').attr("dotoggle", "0");

            $('#btnFamily').buttonMarkup({ theme: "d" });
            $('#btnFamily').attr("dotoggle", "0");

            $('#btnGroup').buttonMarkup({ theme: "d" });
            $('#btnGroup').attr("dotoggle", "0");

            $('#btnNA').buttonMarkup({ theme: "d" });
            $('#btnNA').attr("dotoggle", "0");
        }

        function selectGirl() {
            $('#btnGirl').buttonMarkup({ theme: "a" });
            $('#btnGirl').attr("dotoggle", "1");

            sessionStorage.Gender = 'Girl';


            $('#btnBoy').buttonMarkup({ theme: "d" });
            $('#btnBoy').attr("dotoggle", "0");

            $('#btn3rdGender').buttonMarkup({ theme: "d" });
            $('#btn3rdGender').attr("dotoggle", "0");

            $('#btnFamily').buttonMarkup({ theme: "d" });
            $('#btnFamily').attr("dotoggle", "0");

            $('#btnGroup').buttonMarkup({ theme: "d" });
            $('#btnGroup').attr("dotoggle", "0");

            $('#btnNA').buttonMarkup({ theme: "d" });
            $('#btnNA').attr("dotoggle", "0");
        }

        function select3rdGender() {
            $('#btn3rdGender').buttonMarkup({ theme: "a" });
            $('#btn3rdGender').attr("dotoggle", "1");

            sessionStorage.Gender = '3rd Gender';

            $('#btnBoy').buttonMarkup({ theme: "d" });
            $('#btnBoy').attr("dotoggle", "0");

            $('#btnGirl').buttonMarkup({ theme: "d" });
            $('#btnGirl').attr("dotoggle", "0");

            $('#btnFamily').buttonMarkup({ theme: "d" });
            $('#btnFamily').attr("dotoggle", "0");

            $('#btnGroup').buttonMarkup({ theme: "d" });
            $('#btnGroup').attr("dotoggle", "0");

            $('#btnNA').buttonMarkup({ theme: "d" });
            $('#btnNA').attr("dotoggle", "0");
        }

        function selectFamily() {
            $('#btnFamily').buttonMarkup({ theme: "a" });
            $('#btnFamily').attr("dotoggle", "1");

            sessionStorage.Gender = 'Family';

            $('#btnBoy').buttonMarkup({ theme: "d" });
            $('#btnBoy').attr("dotoggle", "0");

            $('#btnGirl').buttonMarkup({ theme: "d" });
            $('#btnGirl').attr("dotoggle", "0");

            $('#btn3rdGender').buttonMarkup({ theme: "d" });
            $('#btn3rdGender').attr("dotoggle", "0");

            $('#btnGroup').buttonMarkup({ theme: "d" });
            $('#btnGroup').attr("dotoggle", "0");

            $('#btnNA').buttonMarkup({ theme: "d" });
            $('#btnNA').attr("dotoggle", "0");
        }

        function selectGroup() {
            $('#btnGroup').buttonMarkup({ theme: "a" });
            $('#btnGroup').attr("dotoggle", "1");

            sessionStorage.Gender = 'Group';

            $('#btnBoy').buttonMarkup({ theme: "d" });
            $('#btnBoy').attr("dotoggle", "0");

            $('#btnGirl').buttonMarkup({ theme: "d" });
            $('#btnGirl').attr("dotoggle", "0");

            $('#btn3rdGender').buttonMarkup({ theme: "d" });
            $('#btn3rdGender').attr("dotoggle", "0");

            $('#btnFamily').buttonMarkup({ theme: "d" });
            $('#btnFamily').attr("dotoggle", "0");

            $('#btnNA').buttonMarkup({ theme: "d" });
            $('#btnNA').attr("dotoggle", "0");
        }

        function selectNA() {
            $('#btnNA').buttonMarkup({ theme: "a" });
            $('#btnNA').attr("dotoggle", "1");

            sessionStorage.Gender = 'NA';

            $('#btnBoy').buttonMarkup({ theme: "d" });
            $('#btnBoy').attr("dotoggle", "0");

            $('#btnGirl').buttonMarkup({ theme: "d" });
            $('#btnGirl').attr("dotoggle", "0");

            $('#btn3rdGender').buttonMarkup({ theme: "d" });
            $('#btn3rdGender').attr("dotoggle", "0");

            $('#btnFamily').buttonMarkup({ theme: "d" });
            $('#btnFamily').attr("dotoggle", "0");

            $('#btnGroup').buttonMarkup({ theme: "d" });
            $('#btnGroup').attr("dotoggle", "0");
        }

        switch (sessionStorage.Gender) {
            case 'Boy':
                selectBoy();
                break;
            case 'Girl':
                selectGirl();
                break;
            case '3rd Gender':
                select3rdGender();
                break;
            case 'Family':
                selectFamily();
                break;
            case 'Group':
                selectGroup();
                break;
            case 'NA':
                selectNA();
                break;
            default:
                selectNA();
                break;
        }

        //2nd demographic

        function selectKid() {
            $('#btnKid').buttonMarkup({ theme: "a" });
            $('#btnKid').attr("dotoggle", "1");

            sessionStorage.Age = 'Kid';

            $('#btnTeenager').buttonMarkup({ theme: "d" });
            $('#btnTeenager').attr("dotoggle", "0");

            $('#btn20s').buttonMarkup({ theme: "d" });
            $('#btn20s').attr("dotoggle", "0");

            $('#btn30s').buttonMarkup({ theme: "d" });
            $('#btn30s').attr("dotoggle", "0");

            $('#btn40s').buttonMarkup({ theme: "d" });
            $('#btn40s').attr("dotoggle", "0");

            $('#btn50s').buttonMarkup({ theme: "d" });
            $('#btn50s').attr("dotoggle", "0");

            $('#btn60sAndUp').buttonMarkup({ theme: "d" });
            $('#btn60sAndUp').attr("dotoggle", "0");

            $('#btnMixedAgesNA').buttonMarkup({ theme: "d" });
            $('#btnMixedAgesNA').attr("dotoggle", "0");

        }

        function selectTeenager() {
            $('#btnTeenager').buttonMarkup({ theme: "a" });
            $('#btnTeenager').attr("dotoggle", "1");

            sessionStorage.Age = 'Teenager';

            $('#btnKid').buttonMarkup({ theme: "d" });
            $('#btnKid').attr("dotoggle", "0");

            $('#btn20s').buttonMarkup({ theme: "d" });
            $('#btn20s').attr("dotoggle", "0");

            $('#btn30s').buttonMarkup({ theme: "d" });
            $('#btn30s').attr("dotoggle", "0");

            $('#btn40s').buttonMarkup({ theme: "d" });
            $('#btn40s').attr("dotoggle", "0");

            $('#btn50s').buttonMarkup({ theme: "d" });
            $('#btn50s').attr("dotoggle", "0");

            $('#btn60sAndUp').buttonMarkup({ theme: "d" });
            $('#btn60sAndUp').attr("dotoggle", "0");

            $('#btnMixedAgesNA').buttonMarkup({ theme: "d" });
            $('#btnMixedAgesNA').attr("dotoggle", "0");

        }

        function select20s() {
            $('#btn20s').buttonMarkup({ theme: "a" });
            $('#btn20s').attr("dotoggle", "1");

            sessionStorage.Age = "20's";

            $('#btnKid').buttonMarkup({ theme: "d" });
            $('#btnKid').attr("dotoggle", "0");

            $('#btnTeenager').buttonMarkup({ theme: "d" });
            $('#btnTeenager').attr("dotoggle", "0");

            $('#btn30s').buttonMarkup({ theme: "d" });
            $('#btn30s').attr("dotoggle", "0");

            $('#btn40s').buttonMarkup({ theme: "d" });
            $('#btn40s').attr("dotoggle", "0");

            $('#btn50s').buttonMarkup({ theme: "d" });
            $('#btn50s').attr("dotoggle", "0");

            $('#btn60sAndUp').buttonMarkup({ theme: "d" });
            $('#btn60sAndUp').attr("dotoggle", "0");

            $('#btnMixedAgesNA').buttonMarkup({ theme: "d" });
            $('#btnMixedAgesNA').attr("dotoggle", "0");

        }

        function select30s() {
            $('#btn30s').buttonMarkup({ theme: "a" });
            $('#btn30s').attr("dotoggle", "1");

            sessionStorage.Age = "30's";

            $('#btnKid').buttonMarkup({ theme: "d" });
            $('#btnKid').attr("dotoggle", "0");

            $('#btnTeenager').buttonMarkup({ theme: "d" });
            $('#btnTeenager').attr("dotoggle", "0");

            $('#btn20s').buttonMarkup({ theme: "d" });
            $('#btn20s').attr("dotoggle", "0");

            $('#btn40s').buttonMarkup({ theme: "d" });
            $('#btn40s').attr("dotoggle", "0");

            $('#btn50s').buttonMarkup({ theme: "d" });
            $('#btn50s').attr("dotoggle", "0");

            $('#btn60sAndUp').buttonMarkup({ theme: "d" });
            $('#btn60sAndUp').attr("dotoggle", "0");

            $('#btnMixedAgesNA').buttonMarkup({ theme: "d" });
            $('#btnMixedAgesNA').attr("dotoggle", "0");
        }

        function select40s() {
            $('#btn40s').buttonMarkup({ theme: "a" });
            $('#btn40s').attr("dotoggle", "1");

            sessionStorage.Age = "40's";

            $('#btnKid').buttonMarkup({ theme: "d" });
            $('#btnKid').attr("dotoggle", "0");

            $('#btnTeenager').buttonMarkup({ theme: "d" });
            $('#btnTeenager').attr("dotoggle", "0");

            $('#btn20s').buttonMarkup({ theme: "d" });
            $('#btn20s').attr("dotoggle", "0");

            $('#btn30s').buttonMarkup({ theme: "d" });
            $('#btn30s').attr("dotoggle", "0");

            $('#btn50s').buttonMarkup({ theme: "d" });
            $('#btn50s').attr("dotoggle", "0");

            $('#btn60sAndUp').buttonMarkup({ theme: "d" });
            $('#btn60sAndUp').attr("dotoggle", "0");

            $('#btnMixedAgesNA').buttonMarkup({ theme: "d" });
            $('#btnMixedAgesNA').attr("dotoggle", "0");
        }

        function select50s() {
            $('#btn50s').buttonMarkup({ theme: "a" });
            $('#btn50s').attr("dotoggle", "1");

            sessionStorage.Age = "50's";

            $('#btnKid').buttonMarkup({ theme: "d" });
            $('#btnKid').attr("dotoggle", "0");

            $('#btnTeenager').buttonMarkup({ theme: "d" });
            $('#btnTeenager').attr("dotoggle", "0");

            $('#btn20s').buttonMarkup({ theme: "d" });
            $('#btn20s').attr("dotoggle", "0");

            $('#btn30s').buttonMarkup({ theme: "d" });
            $('#btn30s').attr("dotoggle", "0");

            $('#btn40s').buttonMarkup({ theme: "d" });
            $('#btn40s').attr("dotoggle", "0");

            $('#btn60sAndUp').buttonMarkup({ theme: "d" });
            $('#btn60sAndUp').attr("dotoggle", "0");

            $('#btnMixedAgesNA').buttonMarkup({ theme: "d" });
            $('#btnMixedAgesNA').attr("dotoggle", "0");

        }

        function select60sandUp() {
            $('#btn60sAndUp').buttonMarkup({ theme: "a" });
            $('#btn60sAndUp').attr("dotoggle", "1");

            sessionStorage.Age = "60's and Up";

            $('#btnKid').buttonMarkup({ theme: "d" });
            $('#btnKid').attr("dotoggle", "0");

            $('#btnTeenager').buttonMarkup({ theme: "d" });
            $('#btnTeenager').attr("dotoggle", "0");

            $('#btn20s').buttonMarkup({ theme: "d" });
            $('#btn20s').attr("dotoggle", "0");

            $('#btn30s').buttonMarkup({ theme: "d" });
            $('#btn30s').attr("dotoggle", "0");

            $('#btn40s').buttonMarkup({ theme: "d" });
            $('#btn40s').attr("dotoggle", "0");

            $('#btn50s').buttonMarkup({ theme: "d" });
            $('#btn50s').attr("dotoggle", "0");

            $('#btnMixedAgesNA').buttonMarkup({ theme: "d" });
            $('#btnMixedAgesNA').attr("dotoggle", "0");

        }

        function selectMixedAgesNA() {
            $('#btnMixedAgesNA').buttonMarkup({ theme: "a" });
            $('#btnMixedAgesNA').attr("dotoggle", "1");

            sessionStorage.Age = 'Mixed Ages/ NA';

            $('#btnKid').buttonMarkup({ theme: "d" });
            $('#btnKid').attr("dotoggle", "0");

            $('#btnTeenager').buttonMarkup({ theme: "d" });
            $('#btnTeenager').attr("dotoggle", "0");

            $('#btn20s').buttonMarkup({ theme: "d" });
            $('#btn20s').attr("dotoggle", "0");

            $('#btn30s').buttonMarkup({ theme: "d" });
            $('#btn30s').attr("dotoggle", "0");

            $('#btn40s').buttonMarkup({ theme: "d" });
            $('#btn40s').attr("dotoggle", "0");

            $('#btn50s').buttonMarkup({ theme: "d" });
            $('#btn50s').attr("dotoggle", "0");

            $('#btn60sAndUp').buttonMarkup({ theme: "d" });
            $('#btn60sAndUp').attr("dotoggle", "0");
        }


        switch (sessionStorage.Age) {
            case "Kid":
                selectKid();
                break;
            case "Teenager":
                selectTeenager();
                break;
            case "20's":
                select20s();
                break;
            case "30's":
                select30s();
                break;
            case "40's":
                select40s();
                break;
            case "50's":
                select50s();
                break;
            case "60's and Up":
                select60sandUp();
                break;
            case "Mixed Ages/ NA":
                selectMixedAgesNA();
                break;
            default:
                selectMixedAgesNA();
                break;
        }

        //end of additional

        function SaveCustomerDemographics(pData) {
            var pData;

            AjaxSend({
                url: 'SalesMenu/SaveCustomerDemographics',
                data: pData,
                success: function (vData) {
                    if (vData.Result == 'ok')
                        goBack();
                    else
                        alert('error');

                },
                error: function (vError) {
                    var pDummy;
                }
            });

        }

        function ClearCustomerFreeTypeSession() {

            $('#txtCustomerPersonFirstName').val('');
            $('#txtCustomerPersonMiddleName').val('');
            $('#txtCustomerPersonSurname').val('');
            $('#txtCompanyName').val('');
            $('#txtCustomerTIN, #txtCustomerPersonTIN').val('');
            $('#txtCustomerAddress').val('');

            $('#lblCustomerName').text(sessionStorage.CustomerOrganizationName);
            $('#lblCustomerNameHotButtons').text(sessionStorage.CustomerOrganizationName);

            try {
                $('#selTableNo').val('');//.selectmenu('refresh', true);
            } catch (e) {

                $('#selTableNo').val('');
                $('#selTableNo');//.selectmenu();

            }

            try {
                $('#selTableNoHotButtons').val('');//.selectmenu('refresh', true);
            } catch (e) {

                $('#selTableNoHotButtons').val('');
                $('#selTableNoHotButtons');//.selectmenu();

            }

            try {
                $('#selGasPumpNo').val('');//.selectmenu('refresh', true);
            } catch (e) {

                $('#selGasPumpNo').val('');
                $('#selGasPumpNo');//.selectmenu();

            }

            try {
                $('#selGasPumpNoHotButtons').val('');//.selectmenu('refresh', true);
            } catch (e) {

                $('#selGasPumpNoHotButtons').val('');
                $('#selGasPumpNoHotButtons');//.selectmenu();

            }
            sessionStorage.CustomerTableNo = '';
            sessionStorage.GasPumpNo = '';


        }

        function getCityName() {
            if (OfflineMode) {
                var vCity;
                vCity = SalesMenu_offline().getCityListPH();
                DisplayCities(vCity);
            } else {
                AjaxSend({
                    url: 'City/getCityListAll',
                    success: function (vData) {

                        DisplayCities(vData);

                    },
                    error: function (vError) {
                        var pDummy;
                    }
                });
            }

        }

        function getCountryName() {
            if (OfflineMode) {
                var vCountry;
                vCountry = SalesMenu_offline().getCountryListAll();
                DisplayCountries(vCountry);
            } else {
                AjaxSend({
                    url: 'Country/getCountryListAll',
                    success: function (vData) {

                        DisplayCountries(vData);

                    },
                    error: function (vError) {
                        var pDummy;
                    }
                });
            }

        }

        function DisplayCountries(vData) {
            var vRows = vData.CountryData.Rows;

            $('#selOrgCountry').empty();
            //                    $('#selPersonCountry').empty();

            $('#selOrgCountry').append(new Option('Please Select Country', ''));
            //                    $('#selPersonCountry').append(new Option('Please Select Country', ''));

            for (var p = 0; p <= vRows.length - 1; p++) {
                $('#selOrgCountry').append(new Option(vRows[p].Country, vRows[p].ID, false, false));
                //                        $('#selPersonCountry').append(new Option(vRows[p].Country, vRows[p].ID, false, false));
            }
            $("#selOrgCountry option:contains('Philippines')").attr('selected', 'selected');
            //                    $("#selPersonCountry option:contains('Philippines')").attr('selected', 'selected');

            try {
                //$('#selOrgCountry').selectmenu('refresh', true);
                //                        $('#selPersonCountry').selectmenu('refresh', true);
            } catch (e) {

                //$('#selOrgCountry').selectmenu();
                //$('#selOrgCountry').selectmenu('refresh', true);
                //                        $('#selPersonCountry').selectmenu();
                //                        $('#selPersonCountry').selectmenu('refresh', true);

            }
        }

        function DisplayCities(vData) {
            var vRows = vData.CityData.Rows;

            $('#selOrgCity').empty();
            //                    $('#selPersonCity').empty();

            $('#selOrgCity').append(new Option('Please Select City', ''));
            //                    $('#selPersonCity').append(new Option('Please Select City', ''));

            for (var p = 0; p <= vRows.length - 1; p++) {
                $('#selOrgCity').append(new Option(vRows[p].City, vRows[p].ID, false, false));
                //                        $('#selPersonCity').append(new Option(vRows[p].City, vRows[p].ID, false, false));
            }
            //                     $("#selOrgCity option:contains('Philippines')").attr('selected', 'selected');
            //                     $("#selPersonCity option:contains('Philippines')").attr('selected', 'selected');

            try {
                //$('#selOrgCity').selectmenu('refresh', true);
                //                        $('#selPersonCity').selectmenu('refresh', true);
            } catch (e) {

                //$('#selOrgCity').selectmenu();
                //$('#selOrgCity').selectmenu('refresh', true);
                //                        $('#selPersonCity').selectmenu();
                //                        $('#selPersonCity').selectmenu('refresh', true);
            }
        }

        window.pageCustomerDetails.SetPOSMode = function (isTableNo) {

            //        function SetPOSMode(isTableNo) {
            //                    if (sessionStorage.IsManualSelectRetailMode == 'true') {
            //                        sessionStorage.IsManualSelectRetailMode = 'false';
            //                        return;
            //                    }

            if (OfflineMode) {

                if (Business.SystemDefaults.GetSystemDefault('IsShowTableNo') == '1') {
                    $('#divCustomerDetailsRestaurant').show();
                    $('#selTableNo').empty();
                    $('#selTableNo').append(new Option('Please Select', ''));

                    var tableCount = Business.SystemDefaults.GetSystemDefault('NoOfTables');
                    if (CStr2(tableCount) != '') {

                        for (var i = 1; i <= parseInt(tableCount); i++) {
                            $('#selTableNo').append(new Option(i, i));
                        }

                    }

                    //                    if (CDec2(GetUrlParameters().TableNo) > 0) {
                    //                        sessionStorage.CustomerTableNo = GetUrlParameters().TableNo;
                    $('#selTableNo').val(sessionStorage.CustomerTableNo);
                    //                    }

                    try {
                        //$('#selTableNo').selectmenu('refresh', true);
                    } catch (e) {

                        //$('#selTableNo').selectmenu();

                    }
                    if (isTableNo)
                        $('#popupTableNo').modal('toggle');

                } else {
                    $('#divCustomerDetailsRestaurant').hide();
                }

                if (Business.SystemDefaults.GetSystemDefault('IsShowGasPumpNo') == '1') {
                    $('#divCustomerDetailsGasStation').show();
                    $('#selGasPumpNo').empty();
                    $('#selGasPumpNo').append(new Option('Please Select', ''));
                    var pumpCount = Business.SystemDefaults.GetSystemDefault('NoOfGasPumps');
                    if (CStr2(pumpCount) != '') {

                        for (var i = 1; i <= parseInt(pumpCount); i++) {
                            $('#selGasPumpNo').append(new Option(i, i));
                        }

                    }


                    try {
                        //$('#selGasPumpNo').selectmenu('refresh', true);
                    } catch (e) {

                        //$('#selGasPumpNo').selectmenu();

                    }
                } else {
                    $('#divCustomerDetailsGasStation').hide();
                }

                //                pageCustomerDetails.setCustomerInfo();


            } else {
                AjaxSend({
                    url: 'SalesMenu/GetPOSMode',
                    success: function (vData) {


                        if (vData.IsShowTableNo == 1) {
                            $("#divCustomerDetailsRestaurant").show();
                            $("#selTableNo").empty();
                            $('#selTableNo').append(new Option('Please Select', ''));

                            if (CStr2(vData.NoOfTables) != '') {

                                for (var i = 1; i <= vData.NoOfTables; i++) {
                                    $('#selTableNo').append(new Option(i, i));
                                }

                            }


                            try {
                                //$('#selTableNo').selectmenu('refresh', true);
                            } catch (e) {

                                //$('#selTableNo').selectmenu();

                            }

                            if (isTableNo)
                                $('#popupTableNo').modal('toggle');

                        } else {
                            $("#divCustomerDetailsRestaurant").hide();
                        }

                        if (vData.IsShowGasPumpNo == 1) {
                            $("#divCustomerDetailsGasStation").show();
                            $("#selGasPumpNo").empty();
                            $('#selGasPumpNo').append(new Option('Please Select', ''));

                            if (CStr2(vData.NoOfTables) != '') {

                                for (var i = 1; i <= vData.NoOfGasPumps; i++) {
                                    $('#selGasPumpNo').append(new Option(i, i));
                                }

                            }


                            try {
                                //$('#selGasPumpNo').selectmenu('refresh', true);
                            } catch (e) {

                                //$('#selGasPumpNo').selectmenu();

                            }
                        } else {
                            $("#divCustomerDetailsGasStation").hide();
                        }

                        //                        pageCustomerDetails.setCustomerInfo();

                        //                                if (vData.POSMode == "Hot Button Mode") {
                        //                                    goToHotButtonPage();

                        //                                } else {
                        //                                    sessionStorage.PosMode = 'Retail Mode';
                        //                                }


                    }
                });
            }
        }

        window.pageCustomerDetails.setCustomerInfo = function () {

            $('#btnClient').val(sessionStorage.CustomerOrganizationName);
            $('#selDeliveryMethod').val(sessionStorage.DeliveryMethod || 'Take Out');
            $('#selOrderMethod').val(sessionStorage.OrderMethod || 'Walk In');
            $('#selGender').val(sessionStorage.Gender || 'NA');
            $('#selAge').val(sessionStorage.Age || 'Mixed Ages/ NA');

            //this will set all customer name to the current selected
            //added by janine
            $('#lblCustomerNameRecent').text(sessionStorage.CustomerOrganizationName);
            $('#lblCustomerNameRecent2').text(sessionStorage.CustomerOrganizationName);


            if (CStr2(sessionStorage.CustomerTableNo) != '') {
                var pTableView = new App.model.TableView();
                var pTableNum;

                pTableNum = sessionStorage.CustomerTableNo;
                pTableNum = pTableView.GetTableInfoByInternalName(pTableNum) || pTableNum;
                $('#lblCustomerName').text(pTableNum);
                $('#lblCustomerNameHotButtons').text(pTableNum);

                try {
                    $('#selTableNo').val(sessionStorage.CustomerTableNo);//.selectmenu('refresh', true);

                } catch (e) {

                    $('#selTableNo').val(sessionStorage.CustomerTableNo);
                    //$('#selTableNo').selectmenu();

                }

                sessionStorage.GasPumpNo = '';
                try {
                    $('#selGasPumpNo').val('');//.selectmenu('refresh', true);
                } catch (e) {

                    $('#selGasPumpNo').val('');
                    //$('#selGasPumpNo').selectmenu();

                }
                $('#lblLabel').html('Customer / Table No.:<br/>');
            }



            if (CStr2(sessionStorage.GasPumpNo) != '') {

                $('#lblCustomerName').text(sessionStorage.GasPumpNo);
                $('#lblCustomerNameHotButtons').text(sessionStorage.GasPumpNo);

                try {
                    $('#selGasPumpNo').val(sessionStorage.GasPumpNo);//.selectmenu('refresh', true);

                } catch (e) {

                    $('#selGasPumpNo').val(sessionStorage.GasPumpNo);
                    $('#selGasPumpNo');//.selectmenu();

                }

                sessionStorage.CustomerTableNo = '';
                try {
                    $('#selTableNo').val('');//.selectmenu('refresh', true);
                } catch (e) {

                    $('#selTableNo').val('');
                    $('#selTableNo');//.selectmenu();

                }
                $('#lblLabel').html('Gas Pump No.:<br/>');

            }

            if (CStr2(sessionStorage.GasPumpNo) == '' && CStr2(sessionStorage.CustomerTableNo) == '') {

                $('#lblCustomerName').text(sessionStorage.CustomerOrganizationName);
                $('#lblCustomerNameHotButtons').text(sessionStorage.CustomerOrganizationName);

                try {
                    $('#selTableNo').val('');//.selectmenu('refresh', true);
                } catch (e) {

                    $('#selTableNo').val('');
                    $('#selTableNo');//.selectmenu();

                }

                try {
                    $('#selGasPumpNo').val('');//.selectmenu('refresh', true);
                } catch (e) {

                    $('#selGasPumpNo').val('');
                    $('#selGasPumpNo');//.selectmenu();

                }
                $('#lblLabel').html('<img alt="Customer" src="/Content/images/Customer.png" width="30px" align="center" /><br/>');

            }

        } //end of setcustomerinfo

        window.pageCustomerDetails.LoadCustomer = function () {

            if (proj.POMode) {
                sessionStorage.CustomerOrganizationID = sessionStorage["OrgID"];
            }


            if (OfflineMode) {
                

                DisplayCustomer(SalesMenu_offline().GetCustomer(sessionStorage.CustomerOrganizationID));
                if (window.location.pathname.toLowerCase() != '/billofmaterials') {
                    //                    SetPOSMode(false);
                    pageCustomerDetails.setCustomerInfo();
                }
                return true;
            } else {
                AjaxSend({
                    url: 'SalesMenu/GetCustomer',
                    data: { CustomerOrganizationID: sessionStorage.CustomerOrganizationID },
                    success: function (vData) {

                        if ((vData != undefined) && (vData.CompanyName != undefined)) {
                            DisplayCustomer(vData);
                        } else if (CStr2(proj.RemoteServer) != '') {
                            HandleOrgDownload(sessionStorage.CustomerOrganizationID, proj.RemoteServer);
                        } else {
                            proj.BrowseCustomer();
                        }; // if (vData.CompanyName != undefined) {
                        if (window.location.pathname.toLowerCase() != '/billofmaterials') {
                            //                            SetPOSMode(false);
                            pageCustomerDetails.setCustomerInfo();
                        }

                        return true;
                    }
                });
            };

            function HandleOrgDownload(vOrgID, vRemoteServer) {
                NewMessageBox('Downloading customer data from remote server. Please wait.', 'Getting Data');
                var pUrl = '/SalesMenu/ExportOrgToXmlZip';
                pUrl = MergeUrlParameters({}, pUrl, vRemoteServer);
                AjaxSend({
                    url: pUrl,
                    data: {
                        OrgIdToExport: vOrgID  
                    },
                    success: function(vData) {
                        if (vData.Error)
                            NewMessageBox(vData.Error);
                        else if (vData.Results == 'ok')
                            DownloadXmlZip();
                        else
                            NewMessageBox('Unexpected result. ref: jsZH3B2NDFQ7!');
                    }
                })

                function DownloadXmlZip() {
                    AjaxSend({
                        url: '/SalesMenu/DownloadOrgXmlZip',
                        data: {
                            Server: vRemoteServer,
                            OrgIdToExport: vOrgID
                        },
                        success: function(vData) {
                            if (vData.Error)
                                NewMessageBox(vData.Error);
                            else if (vData.Results == 'ok') {
                                window.pageCustomerDetails.LoadCustomer();
                                NewMessageBox('Customer data was successfully loaded.', 'Success');
                            } else
                                NewMessageBox('Unexpected result. ref: jsJAG6FHGMV5!');
                        }
                    });
                } // END function DownloadXmlZip() {
            } // END HandleInvoiceDownload

            function DisplayCustomer(vData) {

                if (vData == undefined) return;

                sessionStorage.CustomerOrganizationID = vData.ID;
                sessionStorage.CustomerOrganizationName = $.trim((CStr2(vData.CompanyName) != '') ? vData.CompanyName : (CStr2(vData.PersonFullname) != '' ? vData.PersonFullname : vData.PersonSurname + ', ' + vData.PersonFirstName + ' ' + vData.PersonMiddleName));

                if (CBool(sessionStorage.IsWebPos)) {
                    Bs.DoneAddingNewCustomer(vData); //bootstrap_modal.js
                } else {
                    $('#btnClient').val(sessionStorage.CustomerOrganizationName)
                            .data('id', sessionStorage.CustomerOrganizationID);
                }
                

                //                proj.LoadCartFromCustomerPage();

                //                pageCustomerDetails.setCustomerInfo();
            };

        }; // END LoadCustomer
    }); // $(function() {]]
});                                                                                      // END require