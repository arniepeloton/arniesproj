define([], function () {
    var mDeferred = $.Deferred();
    var mHtml;

    (function staticInit() {

        AjaxSend({
            ExactUrl: '/Scripts/sources/VendorPortal/MyProfile/main/view.html?v=' + new Date().getTime(),
            type: 'GET',
            dataType: 'text',
            success: function (vData) {
                mHtml = vData;
                ERP.VendorProfile.main.controller = ERP.VendorProfile.main.controller();
                mDeferred.resolve();
            }
        });
    })();

    CreateStringObject('ERP.VendorProfile.main.controller', null, function () {

        var mOut = {}, mElement;

        mElement = $(mHtml);
        mOut.element = mElement;

        $('#divVendorProfile').empty().append(mElement).trigger('create');

        var mydetails, myemail, mycredentials, mysecq, busindid = '', isEditName = false, isChangePs = false, isEditAns1 = false, isEditAns2 = false;
        var accountype, companydetails, companyemail, companymobile, companytelephone;
        var txtMAFirstName = $('#txtMAFirstName');
        var txtMAMiddleName = $('#txtMAMiddleName');
        var txtMALastName = $('#txtMALastName');
        var selMAPersonGenSuffix = $('#selMAPersonGenSuffix');
        var selMABirthMonth = $('#selMABirthMonth');
        var selMABirthDay = $('#selMABirthDay');
        var txtMABirthYear = $('#txtMABirthYear');
        var txtMAEmailAddress = $('#txtMAEmailAddress');
        var txtMAUsername = $('#txtMAUsername');
        var txtMANewPassword = $('#txtMANewPassword');
        var selMASecQuestion1 = $('#selMASecQuestion1');
        var txtMASecQuestion1Answer = $('#txtMASecQuestion1Answer');
        var selMASecQuestion2 = $('#selMASecQuestion2');
        var txtMASecQuestion2Answer = $('#txtMASecQuestion2Answer');

        var txtMACompanyName = $('#txtMACompanyName');
        var txtMACompanyTIN = $('#txtMACompanyTIN');
        var txtMABusInd = $('#txtMABusInd');
        var txtMACompanyBackground = $('#txtMACompanyBackground');
        var txtMACompanyEmailAdd = $('#txtMACompanyEmailAdd');
        var selMACompanyTelcoPrefix = $('#selMACompanyTelcoPrefix');
        var txtMACompanyMobileNo = $('#txtMACompanyMobileNo');
        var txtMACompanyAddressBldgNo = $('#txtMACompanyAddressBldgNo');
        var txtMACompanyAddressStreetName = $('#txtMACompanyAddressStreetName');
        var selMACompanyAddressCity = $('#selMACompanyAddressCity');
        var selMACompanyAddressCountry = $('#selMACompanyAddressCountry');
        var addressID = '', addressAction = 'add', confirmpwfor = '';
        var sellingtype, sellingname = '', searchrequest = undefined, selectedsellingid = '', otherbusactid = '', sellingdbid = '';
        var arrBusinessActivities = [
            { Label: 'Brand Owner', Value: 'Brand Owner' },
            { Label: 'Importer', Value: 'Importer' },
            { Label: 'Dealer or Wholesaler', Value: 'Dealer or Wholesaler' },
            { Label: 'Manufacturer', Value: 'Manufacturer' },
            { Label: 'Exclusive Distributor', Value: 'Exclusive Distributor' },
            { Label: 'Retailer', Value: 'Retailer' },
            { Label: 'Manufacturer\'s Agent', Value: 'Manufacturer\'s Agent' },
            { Label: 'Distributor', Value: 'Distributor' },
            { Label: 'Sales Agent', Value: 'Sales Agent' },
            { Label: 'Product Developer', Value: 'Product Developer' },
            { Label: 'Value-Added Reseller', Value: 'Value-Added Reseller' },
            { Label: 'Installer', Value: 'Installer' },
            { Label: 'Exclusive Importer', Value: 'Exclusive Importer' },
            { Label: 'System Integrator', Value: 'System Integrator' },
            { Label: 'Technical <br> Support', Value: 'Technical Support' }
        ]

        $('body')
            .on('click', '#btnMABackToDashboard', function () {
                AddBreadCrumb({
                    text: 'Dashboard', Click: function () {
                        $('#divVendorDashboard').pageshow();
                    }
                }, '.ItemsBreadCrumbs')
                $('#divVendorDashboard').pageshow();
            })
            .on('click', '#btnMAEditName', function () {
                isEditName = true;
                $('#txtMAFirstName').val(mydetails.FirstName);
                $('#txtMAMiddleName').val(mydetails.MiddleName);
                $('#txtMALastName').val(mydetails.Surname);
                $('#selMAPersonGenSuffix').val(CStr2(mydetails.PersonalSuffixName));
                $('#divEditName').pageshow();
            })
            .on('click', '#btnMACancelName', function () {
                isEditName = false;
                $('#txtMAFirstName').val(mydetails.FirstName);
                $('#txtMAMiddleName').val(mydetails.MiddleName);
                $('#txtMALastName').val(mydetails.Surname);
                $('#selMAPersonGenSuffix').val(CStr2(mydetails.PersonalSuffixName));
                $('#divActualName').pageshow();
            })
            .on('click', '#btnMAChangePassword', function () {
                isChangePs = true;
                $('#divEditPassword').pageshow();
                $('#txtMAUsername, #txtMANewPassword').parent().removeClass('has-error');
                $('#txtMAUsername, #txtMANewPassword').val('');
            })
            .on('click', '#btnMACancelNewPassword', function () {
                isChangePs = false;
                $('#divActualPassword').pageshow();
                $('#txtMANewPassword').val('');
            })
            .on('click', '#btnMASaveChanges', function (e) {
                confirmpwfor = 'company';
                $('#lblConfirmPw').text('For the security of your company details, please re-enter your password to continue');
                if (!ValidateCompanyDetails()) {
                    e.stopPropagation();
                    e.preventDefault();
                } else {
                    $('#txtMACurrentPassword').val('');
                }
            })
            .on('click', '#btnSavePersonDetails', function (e) {
                $('#popupUserInfo').modal('hide');
                $('#lblConfirmPw').html('We want to make sure if your are really <i>' + $.trim(CStr2(mydetails.FirstName) + ' ' + CStr2(mydetails.MiddleName) + ' ' + CStr2(mydetails.Surname)) + '</i>. Please enter your current password.')
                confirmpwfor = 'user';
                if (!ValidatePersonDetails()) {
                    e.stopPropagation();
                    e.preventDefault();
                } else {
                    $('#txtMACurrentPassword').val('');
                }
            })
            .on('click', '#btnMAContinueSave', function () {
                if (confirmpwfor == 'company'){
                    SaveCompanyDetailsNow();
                } else {
                    SavePersonDetailsNow();
                }                
            })
            .on('keypress', '#txtMACurrentPassword', function (e) {
                if (e.which == 13) {
                    SaveCompanyDetailsNow();
                }
            })
            .on('change', '#selMASecQuestion1', function () {
                var val = $.trim($(this).val());
                $('#selMASecQuestion2').find('option').removeAttr('disabled');

                if (val != '') {
                    $('#selMASecQuestion2 option[value="' + $(this).val() + '"]').attr('disabled', true);
                }
            })
            .on('change', '#selMASecQuestion2', function () {
                var val = $.trim($(this).val());
                $('#selMASecQuestion1').find('option').removeAttr('disabled');

                if (val != '') {
                    $('#selMASecQuestion1 option[value="' + $(this).val() + '"]').attr('disabled', true);
                }
            })
            .on('click', '#btnMAEditSecQ1Ans', function () {
                isEditAns1 = true;
                $('#divEditSecQ1').pageshow();
                $('#txtMASecQuestion1Answer').val('').focus();
            })
            .on('click', '#btnMACancelEditSecQ1Ans', function () {
                isEditAns1 = false;
                $('#divStaticSecQ1').pageshow();
                $('#txtMASecQuestion1Answer').val('');
            })
            .on('click', '#btnMAEditSecQ2Ans', function () {
                isEditAns2 = true;
                $('#divEditSecQ2').pageshow();
                $('#txtMASecQuestion2Answer').val('').focus();
            })
            .on('click', '#btnMACancelEditSecQ2Ans', function () {
                isEditAns2 = false;
                $('#divStaticSecQ2').pageshow();
                $('#txtMASecQuestion2Answer').val('');
            })
            .on('click', '#txtMABusInd', function () {
                BrowsePopup('LibraryIndustryClassification', 'Business Industries', [
                    {
                        AutoSearch: true,
                        DisplayText: [{ Column: 'Class' }],
                        Click: function (e) {
                            busindid = e.ID;
                            $('#txtMABusInd').val(e.Class);
                        }
                    }
                ])
            })
            .on('click', '#btnMAAddCompanyEmailAddress', function () {
                $('#txtMACompanyEditEmailAdd').val('');
                $('#lblModalEmailAddress').text('New Email Address');

                $('#formPopupCompanyEmailAdd').unbind('submit').submit(function (e) {
                    e.preventDefault();
                    e.stopPropagation();

                    var emailadd = $.trim($('#txtMACompanyEditEmailAdd').val());

                    if (!IsEmail(emailadd)) {
                        WebToast('Enter valid email address', 'top');
                        return;
                    }

                    companyemail.push({
                        ID: '',
                        EmailAddress: emailadd,
                        OrganizationID: companydetails.ID,
                        State: 'add'
                    })

                    DisplayEmailList(companyemail);
                    $('#popupEditEmailAddress').modal('hide');
                })
            })
            .on('click', '#btnMAAddCompanyMobile', function () {
                $('#txtMACompanyEditMobileNo').val('');
                $('#lblModalMobileNumber').text('New Mobile Number');

                $('#formPopupCompanyMobileNumber').unbind('submit').submit(function (e) {
                    e.preventDefault();
                    e.stopPropagation();

                    var contactnumber = $.trim($('#txtMACompanyEditMobileNo').val());

                    if (!IsNumeric(contactnumber)) {
                        WebToast('Enter valid mobile number', 'top');
                        return;
                    }

                    companymobile.push({
                        ID: '',
                        ContactNo: contactnumber,
                        OrganizationID: companydetails.ID,
                        State: 'add'
                    })
                    DisplayMobileList(companymobile);
                    $('#popupEditMobileNumber').modal('hide');
                })
            })
            .on('click', '#btnMAAddCompanyTelephone', function () {
                $('#txtMACompanyTelEditAreaCode').attr('areacodeid', '').val('');
                $('#txtMACompanyTelEditTelephoneNo').val('');
                $('#txtMACompanyTelEditExtensionNo').val('');
                $('#lblModalTelephoneNumber').text('New Telephone Number');

                $('#formPopupCompanyTelephoneNumber').unbind('submit').submit(function (e) {
                    e.preventDefault();
                    e.stopPropagation();

                    var areacode = $('#txtMACompanyTelEditAreaCode');
                    var telephone = $.trim($('#txtMACompanyTelEditTelephoneNo').val());

                    if (CStr2(areacode.attr('areacodeid')) == '') {
                        WebToast('Select area code', 'top');
                        return;
                    }

                    if (!IsNumeric(telephone)) {
                        WebToast('Enter valid telephone number', 'top');
                        return;
                    }

                    companytelephone.push({
                        ID: '',
                        ContactNo: telephone,
                        ExtensionNo: $('#txtMACompanyTelEditExtensionNo').val(),
                        AreaCodeID: areacode.attr('areacodeid'),
                        AreaCode: areacode.val(),
                        CountryID: 'e783ed13-aa98-4cc1-918e-d49b05c0167e',
                        CountryCode: 'PHL - 63',
                        ContactType: 'Work Telephone',
                        OrganizationID: companydetails.ID,
                        State: 'add'
                    })
                    DisplayTelephoneList(companytelephone);
                    $('#popupEditTelephoneNumber').modal('hide');
                })
            })
            .on('click', '#txtMACompanyTelEditAreaCode', function () {
                BrowsePopup('LibraryAreaCodes', 'Search Area Code', [
                    {
                        DisplayText: [{
                            Column: function (e) {
                                return e.Area + ' - ' + e.CallingCode
                            }
                        }],
                        Click: function (e) {
                            $('#txtMACompanyTelEditAreaCode').attr('areacodeid', e.ID).val(e.Area + ' - ' + e.CallingCode);
                        }
                    }
                ])
            })
            .on('click', '#btnUploadCompanyPictures', function () {
                $('#fileUploadCanvassFormTemplate').val('')
                $('#fileUploadCanvassFormTemplate').click()
            })
            .on('change', '#fileUploadCanvassFormTemplate', function () {
                $('#formUploadCompanyPictures').submit()
            })
            .on('submit', '#formUploadCompanyPictures', function (e) {
                e.stopPropagation();
                e.preventDefault();

                var formdata = new FormData();
                var fileUpload = $('#fileUploadCanvassFormTemplate').get(0);
                var files = fileUpload.files;

                WebToast('Please wait while uploading your file', 'top');

                for (var i = 0; i < files.length; i++) {
                    formdata.append(files[i].ID, files[i]);
                }

                AjaxSend({
                    url: 'Organization/SaveOrganizationFiles?OrgID=' + sessionStorage.OrgID + '&id=' + sessionStorage.SupplierID,
                    data: formdata,
                    async: true,
                    cache: false,
                    contentType: false,
                    processData: false,
                    dataType: 'json',
                    success: function (vData) {
                        if (vData.Results == 'uploaded') {
                            WebToast('Files has been successfully uploaded', 'top');
                            GetFiles();
                        } else {
                            NewMessageBox(vData.Results, 'Error');
                        }
                    }
                })
            })
            .on('click', '#btnMASellingCancel', function () {
                $('#divMainMyProfile').pageshow();
            })
            .on('click', '#btnMACompanySellingCategory', function (e) {
                ClearFieldsSelling();
                sellingtype = 'category';
                $('.lblSelling').text('Category');
                $('#popupSelling').modal('show');
            })
            .on('click', '#btnMACompanySellingBrand', function (e) {
                ClearFieldsSelling();
                sellingtype = 'brand';
                $('.lblSelling').text('Brand Name');
                $('#popupSelling').modal('show');
            })
            .on('keyup', '#txtSelling', function (e) {
                var keyword = $.trim($(this).val()).toLowerCase();
                var listSelling = $('#listSelling');
                var divNewSelling = $('#divNewSelling');
                var stoploop = true;
                selectedsellingid = '';
                listSelling.empty();
                divNewSelling.empty();

                if (searchrequest != undefined) {
                    //searchrequest.abort();
                    clearTimeout(searchrequest);
                }                

                if (keyword != '') {
                    listSelling.append($('<a data-role="none" href="#" class="list-group-item">').text('Searching ...'))

                    searchrequest = setTimeout(function () {
                        $.ajax({
                            url: '/Browser/' + (sellingtype == 'category' ? 'LibraryCategoriesOfProductOrServices' : 'LibraryBrandNames') + '?OrgID=' + sessionStorage.OrgID,
                            type: 'POST',
                            data: { keywords: keyword },
                            success: function (e) {
                                var vRows = JSON.parse(e).d.Results.Rows;
                                stoploop = false;

                                listSelling.empty();
                                divNewSelling.empty();

                                if (vRows.length > 0) {
                                    for (var i = 0; i < vRows.length; i++) {
                                        if (stoploop) {
                                            break;
                                        }
                                        listSelling.append($('<a data-role="none" href="#" class="list-group-item">')
                                                .data('data', vRows[i])
                                                .text(sellingtype == 'category' ? vRows[i].CategoryName : vRows[i].BrandName)
                                                .click(function (e) {
                                                    e.stopPropagation();
                                                    e.preventDefault();
                                                    var data = $(this).data('data');
                                                    selectedsellingid = data.ID;
                                                    sellingname = sellingtype == 'category' ? data.CategoryName : data.BrandName;
                                                    $('#txtSelling').val(sellingname);
                                                    $('#divListSelling').hide();
                                                })
                                            )
                                    }
                                } else {
                                    listSelling.append($('<a data-role="none" href="#" class="list-group-item">').html('<i>No results found for ' + keyword + '.</i>'))
                                }

                                divNewSelling.append($('<a data-role="none" href="#" class="list-group-item" style="color:#FF5722;">').text('Can\'t find what you\'re looking for? Add new ' + sellingtype + ' instead.')
                                        .click(function () {
                                            $('#divListSelling').hide();
                                            if (sellingtype == 'category') {
                                                $('#lblTitleSellingType').text('Add New Category of Product or Service');
                                            } else {
                                                $('#lblTitleSellingType').text('Add New Brand Name');
                                            }
                                            $('#txtNewSelling').val('');
                                            $('#popupSelling').modal('hide');
                                            $('#popupNewSelling').modal('show');
                                        })
                                    )
                            }
                        })
                    }, 1500)
                    $('#divListSelling').show();
                }
                else {
                    $('#divListSelling').hide();
                }
            })
            .on('blur', '#txtSelling', function () {
                setTimeout(function () {
                    $('#divListSelling').hide();
                    $('#txtSelling').val(sellingname);
                }, 200)
            })
            .on('submit', '#formSelling', function (e) {
                e.stopPropagation();
                e.preventDefault();
                var arrBusAct = [], hasOther = false;

                $('[name="chbBusinessActivities"]:checked').each(function () {
                    if ($(this).val() == 'Other'){
                        if ($.trim($('#txtOtherBusinessActivity').val()) == '') {
                            WebToast('Please enter other business activity', 'top');
                            arrBusAct = [];
                            return;
                        }
                        hasOther = true;
                    }

                    arrBusAct.push($(this).val());
                })

                if (selectedsellingid == '') {
                    WebToast('Please search and select ' + sellingtype, 'top');
                    return;
                }

                if (arrBusAct.length == 0) {
                    WebToast('Please select at least one business activity', 'top');
                    return;
                }

                AjaxSend({
                    url: '/VendorPortal/' + (sellingtype == 'category' ? 'SaveOrgCategoryOfProductOrService' : 'SaveOrgBrandName'),
                    data: {
                        data: {
                            ID: sellingdbid,
                            SellingType: sellingtype,
                            OrganizationID: sessionStorage.SupplierID,
                            UserID: sessionStorage.PeopleUserID,
                            SellingID: selectedsellingid,
                            IsActive: $('[name="radSellingStatus"]:checked').val(),
                            BusinessActivities: arrBusAct.join(', '),
                            HasOther: hasOther,
                            OtherBusActID: otherbusactid,
                            OtherBusAct: $.trim($('#txtOtherBusinessActivity').val())
                        }
                    }, success: function (e) {
                        $('#popupSelling').modal('hide');

                        if (e.Results == 'saved'){
                            WebToast('Successfully saved', 'top');

                            if (sellingtype == 'category') {
                                GetOrgSellingByCategories();
                            } else {
                                GetOrgSellingByBrands();
                            }
                        } else {
                            NewMessageBox(e.Results, 'Error');
                        }
                    }
                })
            })
            .on('submit', '#formNewSelling', function (e) {
                e.stopPropagation();
                e.preventDefault();

                var txtNewSelling = $.trim($('#txtNewSelling').val());

                if (txtNewSelling == '') {
                    WebToast('Please enter name', 'top');
                    return;
                }

                AjaxSend({
                    url: '/Organization/' + (sellingtype == 'category' ? 'SaveNewCategoriesOfProductOrServices' : 'SaveNewBrandNames'),
                    data: {
                        ID: '',
                        Name: txtNewSelling,
                        OrganizationID: sessionStorage.SupplierID,
                        UserID: sessionStorage.PeopleUserID
                    }, success: function (e) {
                        $('#popupNewSelling').modal('hide');

                        if (e.Results == 'saved') {
                            selectedsellingid = e.ID.toLowerCase();
                            sellingname = txtNewSelling;
                            WebToast('Successfully saved', 'top');
                            $('#txtSelling').val(txtNewSelling);
                            $('#popupSelling').modal('show');
                        } else {
                            NewMessageBox(e.Results, 'Error');
                        }
                    }
                })
            })

        mOut.LoadProfile = function (callback) {
            AjaxSend({
                url: '/VendorPortal/CompanyProfile',
                data: {
                    OrganizationID: sessionStorage.SupplierID
                },
                success: function (data) {
                    accountype = data.AccountType;                    

                    var selMACompanyAddressCity = $('#selMACompanyAddressCity');
                    var selMACompanyAddressCountry = $('#selMACompanyAddressCountry');
                    var selMAPersonAddressCity = $('#selMAPersonAddressCity');
                    var selMAPersonAddressCountry = $('#selMAPersonAddressCountry');

                    selMACompanyAddressCity.empty().append(new Option('Please select City', ''));
                    selMAPersonAddressCity.empty().append(new Option('Please select City', ''));
                    for (var i = 0; i < data.Cities.length; i++) {
                        selMACompanyAddressCity.append(new Option(data.Cities[i].Name, data.Cities[i].ID));
                        selMAPersonAddressCity.append(new Option(data.Cities[i].Name, data.Cities[i].ID));
                    }

                    for (var i = 0; i < data.Countries.length; i++) {
                        selMACompanyAddressCountry.append(new Option(data.Countries[i].Name, data.Countries[i].ID));
                        selMAPersonAddressCountry.append(new Option(data.Countries[i].Name, data.Countries[i].ID));
                    }

                    $('#tabCompanyProfile').opentabmenu();
                    $('[custom-tab-content="#tabCompanyProfile"]').removeClass('hidetab');

                    companydetails = data.CompanyDetails[0];
                    companyemail = data.CompanyEmail;
                    companymobile = data.CompanyMobile;
                    companytelephone = data.CompanyTelephone;
                    busindid = CStr2(companydetails.IndustryClassID);

                    $('#txtMACompanyName').val(companydetails.CompanyName);
                    $('#txtMACompanyTIN').val(companydetails.TinNumber);
                    $('#txtMABusInd').val(CStr2(companydetails.IndustryClassName));
                    $('#txtMACompanyBackground').val(companydetails.Background);
                    $('#txtMACompanyAddressBldgNo').val(companydetails.OfficeAddressBuildingNo);
                    $('#txtMACompanyAddressStreetName').val(companydetails.OfficeAddressStreet);
                    $('#selMACompanyAddressCity').val(companydetails.OfficeAddressCityID);
                    $('#selMACompanyAddressCountry').val(companydetails.OfficeAddressCountryID);

                    DisplayEmailList(companyemail);
                    DisplayMobileList(companymobile);
                    DisplayTelephoneList(companytelephone);

                    if (typeof callback == 'function'){
                        callback();
                    }
                    
                    //AjaxSend({
                    //    url: 'Browser/LibraryCities',
                    //    data: { IsAll: 1 },
                    //    success: function (e) {
                    //        var vRows = e.Results.Rows;
                    //        var selAddressCity = $('#selAddressCity');

                    //        selAddressCity.empty();
                    //        selAddressCity.append(new Option('Please select', ''));

                    //        for (var i = 0; i < vRows.length; i++) {
                    //            selAddressCity.append(new Option(vRows[i].Name, vRows[i].ID));
                    //        }
                    //    }
                    //})
                }
            })
        }

        mOut.LoadRequiredData = function () {
            mOut.LoadProfile(function () {
                GetFiles();
                GenerateBusinessActivities();
                GetOrgSellingByCategories();
                GetOrgSellingByBrands();
                GetCompanyUsers();
            });            
        }

        mOut.LoadRequiredData();

        var tblCompanyUsers;
        function GetCompanyUsers() {
            AjaxSend({
                url: 'VendorPortal/GetCompanyUsers',
                data: { OrganizationID: sessionStorage.SupplierID },
                success: function (e) {
                    var vRows = e.Users;
                    var ctr = 0;

                    $('#tbodyCompanyUsers').empty();

                    if ($.fn.DataTable.isDataTable("#tblCompanyUsers")) {
                        //check if datatable is created 
                        $('#tblCompanyUsers').DataTable().clear().destroy();
                    }

                    tblCompanyUsers = $('#tblCompanyUsers').DataTable({
                        retrieve: true,
                        "bdestroy": true,
                        "pageLength": 10,
                        "data": vRows,
                        "autoWidth": false,
                        "ordering": true,
                        "columns": [
                            {
                                "data": function (data, type, full, meta) {
                                    ctr += 1;
                                    return ctr + '.';
                                },
                                "width": "30px"
                            },
                            {
                                "data": function (data, type, full, meta) {
                                    return data.FullName;
                                }
                            },
                            {
                                "data": function (data, type, full, meta) {
                                    return data.ModifiedByName;
                                }
                            },
                            {
                                "data": function (data, type, full, meta) {
                                    return $.format.date(new Date(parseInt(data.LastUpdated.substring(6))), 'MM-dd-yyyy hh:mm a');
                                }
                            }
                        ],
                        "rowCallback": function (row, data) {
                            var tr = $(row).data('data', data);

                            tr.click(function (e) {
                                var data = $(this).data('data');
                                
                                AjaxSend({
                                    url: 'VendorPortal/GetCompanyUserDteails',
                                    data: { UserID: data.PeopleID },
                                    success: function (e) {
                                        mydetails = e.Details[0];
                                        myemail = e.Email[0];
                                        mycredentials = e.Credentials[0];
                                        mysecq = e.SecQ;

                                        isEditName = false;
                                        isChangePs = false;
                                        isEditAns1 = false;
                                        isEditAns2 = false;

                                        selMABirthDay.empty().append(new Option('Day', ''));

                                        for (var i = 1; i <= 31; i++) {
                                            selMABirthDay.append(new Option(i, i))
                                        }

                                        $('#divActualName, #divActualPassword, #divStaticSecQ1, #divStaticSecQ2').pageshow();

                                        $('#txtMAFirstName, #txtMAMiddleName, #txtMALastName, #txtMANewPassword').val('');

                                        $('#txtMAFullName').text($.trim(mydetails.FirstName + ' ' + CStr2(mydetails.MiddleName) + ' ' + mydetails.Surname + ' ' + CStr2(mydetails.PersonalSuffixName)));

                                        if (mydetails.DateOfBirth != null) {
                                            $('#selMABirthMonth').val($.format.date(new Date(mydetails.DateOfBirth), 'MMMM'));
                                            $('#selMABirthDay').val($.format.date(new Date(mydetails.DateOfBirth), 'd'));
                                            $('#txtMABirthYear').val($.format.date(new Date(mydetails.DateOfBirth), 'yyyy'));
                                        }

                                        $('#txtMAEmailAddress').val(myemail.EmailAddress);
                                        $('#txtMAUsername').val(mycredentials.Username);

                                        $('#selMASecQuestion1').val(CStr2(mysecq[0].SecurityQuestion));
                                        $('#txtMASecQuestion1Answer').val('');
                                        $('#selMASecQuestion2').val(CStr2(mysecq[1].SecurityQuestion));
                                        $('#txtMASecQuestion2Answer').val('');

                                        $('#popupUserInfo').modal('show');
                                    }
                                })
                            })
                        },
                        "initComplete": function (row, data) {
                            $('#tblCompanyUsers').table('refresh');
                        },
                        "aoColumnDefs": [{ "bSortable": false, "aTargets": ['_all'] }],
                        "responsive": true,
                        "order": [[2, "asc"]],
                        "sDom": '<"top">rt<"bottom"ip><"clear">'
                    })
                }
            })
        }

        var tblSellingByCategories;
        function GetOrgSellingByCategories() {
            AjaxSend({
                url: 'VendorPortal/GetOrgSellingByCategories',
                data: { OrganizationID: sessionStorage.SupplierID },
                success: function (e) {
                    var vRows = e.Categories.Rows;
                    var ctr = 0;

                    $('#tbodySellingByCategories').empty()

                    if ($.fn.DataTable.isDataTable("#tblSellingByCategories")) {
                        //check if datatable is created 
                        $('#tblSellingByCategories').DataTable().clear().destroy();
                    }

                    tblSellingByCategories = $('#tblSellingByCategories').DataTable({
                        retrieve: true,
                        "bdestroy": true,
                        "pageLength": 10,
                        "data": vRows,
                        "autoWidth": false,
                        "ordering": true,
                        "columns": [
                            {
                                "data": function (data, type, full, meta) {
                                    ctr += 1;
                                    return ctr + '.';
                                },
                                "width": "30px"
                            },
                            {
                                "data": function (data, type, full, meta) {
                                    return data.CategoryName;
                                }
                            },
                            {
                                "data": function (data, type, full, meta) {
                                    return data.BusinessActivities;
                                }
                            },
                            {
                                "data": function (data, type, full, meta) {
                                    return CBool(data.IsActive) ? 'Active' : 'Inactive';
                                }
                            },
                            {
                                "data": function (data, type, full, meta) {
                                    return data.CreatedByUser + ' on ' + $.format.date(new Date(data.DateCreated), 'MM-dd-yyyy hh:mm a');
                                }
                            },
                            {
                                "data": function (data, type, full, meta) {
                                    if (data.ModifiedBy != undefined) {
                                        return data.ModifiedByUser + ' on ' + $.format.date(new Date(data.DateModified), 'MM-dd-yyyy hh:mm a');
                                    } else {
                                        return '';
                                    }                                    
                                }
                            }
                        ],
                        "rowCallback": function (row, data) {
                            var tr = $(row).data('data', data);

                            tr.click(function (e) {
                                var data = $(this).data('data');
                                EditSelling(data, 'category');
                            })
                        },
                        "initComplete": function (row, data) {
                            $('#tblSellingByCategories').table('refresh');
                        },
                        "aoColumnDefs": [{ "bSortable": false, "aTargets": ['_all'] }],
                        "responsive": true,
                        "order": [[2, "asc"]],
                        "sDom": '<"top">rt<"bottom"ip><"clear">'
                    })
                }
            })
        }

        var tblSellingByBrands;
        function GetOrgSellingByBrands() {
            AjaxSend({
                url: 'VendorPortal/GetOrgSellingByBrands',
                data: { OrganizationID: sessionStorage.SupplierID },
                success: function (e) {
                    var vRows = e.Brands.Rows;
                    var ctr = 0;

                    $('#tbodySellingByBrands').empty()

                    if ($.fn.DataTable.isDataTable("#tblSellingByBrands")) {
                        //check if datatable is created 
                        $('#tblSellingByBrands').DataTable().clear().destroy();
                    }

                    tblSellingByBrands = $('#tblSellingByBrands').DataTable({
                        retrieve: true,
                        "bdestroy": true,
                        "pageLength": 10,
                        "data": vRows,
                        "autoWidth": false,
                        "ordering": true,
                        "columns": [
                            {
                                "data": function (data, type, full, meta) {
                                    ctr += 1;
                                    return ctr + '.';
                                },
                                "width": "30px"
                            },
                            {
                                "data": function (data, type, full, meta) {
                                    return data.BrandName;
                                }
                            },
                            {
                                "data": function (data, type, full, meta) {
                                    return data.BusinessActivities;
                                }
                            },
                            {
                                "data": function (data, type, full, meta) {
                                    return CBool(data.IsActive) ? 'Active' : 'Inactive';
                                }
                            },
                            {
                                "data": function (data, type, full, meta) {
                                    return data.CreatedByUser + ' on ' + $.format.date(new Date(data.DateCreated), 'MM-dd-yyyy hh:mm a');
                                }
                            },
                            {
                                "data": function (data, type, full, meta) {
                                    if (data.ModifiedBy != undefined) {
                                        return data.ModifiedByUser + ' on ' + $.format.date(new Date(data.DateModified), 'MM-dd-yyyy hh:mm a');
                                    } else {
                                        return '';
                                    }
                                }
                            }
                        ],
                        "rowCallback": function (row, data) {
                            var tr = $(row).data('data', data);

                            tr.click(function (e) {
                                var data = $(this).data('data');
                                EditSelling(data, 'brand');
                            })
                        },
                        "initComplete": function (row, data) {
                            $('#tblSellingByBrands').table('refresh');
                        },
                        "aoColumnDefs": [{ "bSortable": false, "aTargets": ['_all'] }],
                        "responsive": true,
                        "order": [[2, "asc"]],
                        "sDom": '<"top">rt<"bottom"ip><"clear">'
                    })
                }
            })
        }

        function EditSelling(data, type) {
            ClearFieldsSelling();

            sellingtype = type;
            selectedsellingid = type == 'category' ? data.CategoryID : data.BrandNameID;
            sellingdbid = data.ID;
            sellingname = type == 'category' ? data.CategoryName : data.BrandName;

            $('.lblSelling').text(type.charAt(0).toUpperCase() + type.slice(1))
            $('#txtSelling').val(sellingname);
            $('[name="radSellingStatus"][value="' + CInt2(data.IsActive) + '"]').prop('checked', true);

            var splitBusAct = data.BusinessActivities.split(', ');

            for (var i = 0; i < splitBusAct.length; i++){
                $('[id="' + $.trim(splitBusAct[i]) + '"]').attr('checked', 'checked');
            }

            if (data.OtherBusinessActivityID != undefined) {
                $('#txtOtherBusinessActivity').val(data.OtherBusinessActivity).removeAttr('disabled');
            }

            $('#popupSelling').modal('show');
        }

        function ClearFieldsSelling() {
            sellingdbid = '';
            selectedsellingid = '';
            otherbusactid = '';
            sellingname = '';
            $('#txtSelling').val('');
            $('#radSellingActive').attr('checked', 'checked');
            $('.lblSelling').text('');
            $('[name="chbBusinessActivities"]').removeAttr('checked');
            $('#txtOtherBusinessActivity').val('').attr('disabled', 'disabled');
        }

        function GenerateBusinessActivities() {
            var divBusinessActivities = $('#divBusinessActivities');
            divBusinessActivities.empty();

            for (var i = 0; i < arrBusinessActivities.length; i++){
                divBusinessActivities.append($('<div class="col-sm-4">')
                        .append($('<div class="bs-checkbox">')
                            .append($('<input data-role="none" id="' + arrBusinessActivities[i].Label + '" name="chbBusinessActivities" type="checkbox" value="' + arrBusinessActivities[i].Value + '">'))
                            .append($('<label for="' + arrBusinessActivities[i].Label + '">').append(arrBusinessActivities[i].Label))
                        )                        
                    )
            }

            divBusinessActivities.append($('<div class="col-sm-12">')
                        .append($('<div class="bs-checkbox">')
                            .append($('<input data-role="none" id="Other Business Activity" name="chbBusinessActivities" type="checkbox" value="Other Business Activity">')
                                .change(function () {
                                    if ($(this).is(':checked')) {
                                        $('#txtOtherBusinessActivity').removeAttr('disabled').val('');
                                    } else {
                                        $('#txtOtherBusinessActivity').attr('disabled', 'disabled').val('');
                                    }                                    
                                })
                            )
                            .append($('<label for="Other Business Activity">').append('Other Business Activity'))
                            .append($('<div>')
                                .append($('<input data-role="none" id="txtOtherBusinessActivity" type="text" class="form-control unicase-form-control" disabled>'))
                            )
                        )
                    )
        }

        function GetFiles() {
            var divCompanyPicturesList = $('#divCompanyPicturesList');
            divCompanyPicturesList.empty();

            AjaxSend({
                url: 'Organization/GetOrganizationFiles',
                data: { ID: companydetails.ID, FileType: 'image' },
                success: function (e) {
                    var vRows = e.Results.Rows;

                    if (vRows.length > 0) {
                        for (var i = 0; i < vRows.length; i++) {
                            var src = '/FileTemplates/' + sessionStorage.OrgID + '/OrganizationFiles/' + companydetails.ID.toLowerCase() + '/' + vRows[i].FileName;

                            divCompanyPicturesList.append($('<div class="col-sm-3" id="divimg-'+ vRows[i].ID +'">')
                                    .append($('<div>')
                                        .append($('<a data-role="none" data-lightbox="companypictures">')
                                            .attr('href', src)
                                            .append($('<img class="img-thumbnail" style="max-width:100%; height:200px; object-fit:cover;">')
                                                .attr('src', src)
                                            )
                                        )                                        
                                    )
                                    .append($('<table style="width:100%; margin:10px 0px;">')
                                        .append($('<tr>')
                                            .append($('<td>')
                                                .append(vRows[i].FileName)
                                            )
                                            .append($('<td style="width:50px; text-align:right;">')
                                                .append($('<button data-role="none" class="btn btn-default btn-less-width">')
                                                    .data('data', vRows[i])
                                                    .append($('<span class="glyphicon glyphicon-remove">'))
                                                    .click(function () {
                                                        var data = $(this).data('data')

                                                        NewMessageBox('Are you sure you want to remove picture <b>' + data.FileName + '</b>?', 'Confirmation Message', [
                                                            {
                                                                Text: 'Yes', Click: function () {
                                                                    AjaxSend({
                                                                        url: 'Organization/RemoveOrganizationFiles',
                                                                        data: { ID: data.ID, CompanyID: companydetails.ID, FileName: data.FileName },
                                                                        success: function (vData) {
                                                                            if (vData.Results == 'deleted') {
                                                                                WebToast(data.FileName + ' has been successfully deleted', 'top');
                                                                                $('#divimg-' + data.ID).remove();
                                                                            } else {
                                                                                NewMessageBox(vData.Results, 'Error');
                                                                            }
                                                                        }
                                                                    })
                                                                }
                                                            }, { Text: 'No' }
                                                        ])
                                                    })
                                                )
                                            )
                                        )
                                    )
                                )
                        }
                    } else {
                        divCompanyPicturesList.append($('<div style="color:#cacaca; text-align:center; width:100%;">')
                                .append($('<span class="glyphicon glyphicon-picture" style="font-size: 25vh;">'))
                                .append('<br>')
                                .append($('<span style="font-size:30px;">').text('Gallery is empty'))
                            );
                    }
                }
            })
        }

        function DisplayTelephoneList(data) {
            var isEmpty = true;
            var divCompanyTelephoneList = $('#divCompanyTelephoneList');
            divCompanyTelephoneList.empty();

            for (var i = 0; i < data.length; i++) {
                data[i].Index = i;

                if (data[i].State == 'remove') {
                    continue;
                }
                isEmpty = false;
                divCompanyTelephoneList.append($('<li data-role="none" href="#" class="list-group-item">')
                        .text('+63 (' + $.trim(data[i].AreaCode.split('-')[1]) + ') ' + data[i].ContactNo + (CStr2(data[i].ExtensionNo) != '' ? '.' + data[i].ExtensionNo : ''))
                        .append($('<button class="btn btn-default btn-xs pull-right">')
                            .append('Remove')
                            .data('data', data[i])
                            .click(function () {
                                var data = $(this).data('data');

                                NewMessageBox('Are you sure you want to remove telephone number <b>+63 (' + $.trim(data.AreaCode.split('-')[1]) + ') ' + data.ContactNo + (CStr2(data.ExtensionNo) != '' ? '.' + data.ExtensionNo : '') + '</b>?', 'Confirmation Message', [
                                    {
                                        Text: 'Yes', Click: function () {
                                            if (data.State == 'add') {
                                                companytelephone.splice(data.Index, 1);
                                            } else {
                                                data.State = 'remove';
                                            }

                                            DisplayTelephoneList(companytelephone);
                                        }
                                    }, { Text: 'No' }
                                ])
                            })
                        )
                        .append($('<button class="btn btn-default btn-xs pull-right" style="margin-right:5px;">')
                            .append('Edit')
                            .data('data', data[i])
                            .click(function () {
                                var data = $(this).data('data');

                                $('#txtMACompanyTelEditAreaCode').attr('areacodeid', data.AreaCodeID).val(data.AreaCode);
                                $('#txtMACompanyTelEditTelephoneNo').val(data.ContactNo);
                                $('#txtMACompanyTelEditExtensionNo').val(data.ExtensionNo);
                                $('#lblModalTelephoneNumber').text('Edit Telephone Number');
                                $('#popupEditTelephoneNumber').modal('show');

                                $('#formPopupCompanyTelephoneNumber').unbind('submit').submit(function (e) {
                                    e.preventDefault();
                                    e.stopPropagation();

                                    var areacode = $('#txtMACompanyTelEditAreaCode');
                                    var telephone = $.trim($('#txtMACompanyTelEditTelephoneNo').val());

                                    if (CStr2(areacode.attr('areacodeid')) == '') {
                                        WebToast('Select area code', 'top');
                                        return;
                                    }

                                    if (!IsNumeric(telephone)) {
                                        WebToast('Enter valid telephone number', 'top');
                                        return;
                                    }

                                    data.ContactNo = telephone;
                                    data.AreaCodeID = areacode.attr('areacodeid');
                                    data.AreaCode = areacode.val();
                                    data.ExtensionNo = $('#txtMACompanyTelEditExtensionNo').val();

                                    DisplayTelephoneList(companytelephone);
                                    $('#popupEditTelephoneNumber').modal('hide');
                                })
                            })
                        )
                    )
            }

            if (isEmpty){
                divCompanyTelephoneList.append('No telephone number found')
            }
        }

        function DisplayMobileList(data) {
            var isEmpty = true;
            var divCompanyMobileList = $('#divCompanyMobileList');
            divCompanyMobileList.empty();

            for (var i = 0; i < data.length; i++) {
                data[i].Index = i;

                if (data[i].State == 'remove') {
                    continue;
                }
                isEmpty = false;
                divCompanyMobileList.append($('<li data-role="none" href="#" class="list-group-item">')
                        .text('+63' + data[i].ContactNo)
                        .append($('<button class="btn btn-default btn-xs pull-right">')
                            .append('Remove')
                            .data('data', data[i])
                            .click(function () {
                                var data = $(this).data('data');

                                NewMessageBox('Are you sure you want to remove mobile number <b>+63' + data.ContactNo + '</b>?', 'Confirmation Message', [
                                    {
                                        Text: 'Yes', Click: function () {
                                            if (data.State == 'add') {
                                                companymobile.splice(data.Index, 1);
                                            } else {
                                                data.State = 'remove';
                                            }

                                            DisplayMobileList(companymobile);
                                        }
                                    }, { Text: 'No' }
                                ])
                            })
                        )
                        .append($('<button class="btn btn-default btn-xs pull-right" style="margin-right:5px;">')
                            .append('Edit')
                            .data('data', data[i])
                            .click(function () {
                                var data = $(this).data('data');

                                $('#txtMACompanyEditMobileNo').val(data.ContactNo);
                                $('#lblModalMobileNumber').text('Edit Mobile Number');
                                $('#popupEditMobileNumber').modal('show');

                                $('#formPopupCompanyMobileNumber').unbind('submit').submit(function (e) {
                                    e.preventDefault();
                                    e.stopPropagation();

                                    var contactnumber = $.trim($('#txtMACompanyEditMobileNo').val());

                                    if (!IsNumeric(contactnumber)) {
                                        WebToast('Enter valid mobile number', 'top');
                                        return;
                                    }

                                    data.ContactNo = contactnumber;
                                    DisplayMobileList(companyemail);
                                    $('#popupEditMobileNumber').modal('hide');
                                })
                            })
                        )
                    )
            }

            if (isEmpty) {
                divCompanyMobileList.append('No mobile number found')
            }
        }

        function DisplayEmailList(data) {
            var isEmpty = true;
            var divCompanyEmailList = $('#divCompanyEmailList');
            divCompanyEmailList.empty();

            for (var i = 0; i < data.length; i++) {
                data[i].Index = i;

                if (data[i].State == 'remove'){
                    continue;
                }
                isEmpty = false;
                divCompanyEmailList.append($('<li data-role="none" href="#" class="list-group-item">')
                        .text(data[i].EmailAddress)
                        .append($('<button class="btn btn-default btn-xs pull-right">')
                            .append('Remove')
                            .data('data', data[i])
                            .click(function () {
                                var data = $(this).data('data');

                                NewMessageBox('Are you sure you want to remove email address <b>' + data.EmailAddress + '</b>?', 'Confirmation Message', [
                                    {
                                        Text: 'Yes', Click: function () {
                                            if (data.State == 'add') {
                                                companyemail.splice(data.Index, 1);
                                            } else {
                                                data.State = 'remove';
                                            }

                                            DisplayEmailList(companyemail);
                                        }
                                    }, { Text: 'No'}
                                ])
                            })
                        )
                        .append($('<button class="btn btn-default btn-xs pull-right" style="margin-right:5px;">')
                            .append('Edit')
                            .data('data', data[i])
                            .click(function () {
                                var data = $(this).data('data');

                                $('#txtMACompanyEditEmailAdd').val(data.EmailAddress);
                                $('#lblModalEmailAddress').text('Edit Email Address');
                                $('#popupEditEmailAddress').modal('show');

                                $('#formPopupCompanyEmailAdd').unbind('submit').submit(function (e) {
                                    e.preventDefault();
                                    e.stopPropagation();

                                    var emailadd = $.trim($('#txtMACompanyEditEmailAdd').val());

                                    if (!IsEmail(emailadd)) {
                                        WebToast('Enter valid email address', 'top');
                                        return;
                                    }

                                    data.EmailAddress = emailadd;
                                    DisplayEmailList(companyemail);
                                    $('#popupEditEmailAddress').modal('hide');
                                })
                            })
                        )
                    )
            }

            if (isEmpty) {
                divCompanyEmailList.append('No email address found')
            }
        }

        function SaveCompanyDetailsNow() {
            var txtMACurrentPassword = $('#txtMACurrentPassword');
            $('#divMAErrMsg').hide();

            if ($.trim(txtMACurrentPassword.val()) == '') {
                txtMACurrentPassword.parent().addClass('has-error');
                return;
            } else {
                txtMACurrentPassword.parent().removeClass('has-error');
            }

            $('#btnMAContinueSave').text('SAVING ...').attr('disable', true);

            AjaxSend({
                url: '/Ecommerce/CheckPassword',
                data: { password: $.trim(txtMACurrentPassword.val()), PeopleUserID: sessionStorage.PeopleUserID },
                success: function (e) {
                    $('#btnMAContinueSave').text('CONTINUE').removeAttr('disabled');

                    if (e.Results == 'valid') {
                        var data = {
                            //FirstName: $.trim(txtMAFirstName.val()),
                            //MiddleName: $.trim(txtMAMiddleName.val()),
                            //LastName: $.trim(txtMALastName.val()),
                            //GenSuffix: $.trim(selMAPersonGenSuffix.val()),
                            //DateOfBirth: $.format.date(new Date(selMABirthMonth.val() + ' ' + $.trim(selMABirthDay.val()) + ', ' + $.trim(txtMABirthYear.val()))),
                            //PersonEmailAdd: $.trim(txtMAEmailAddress.val()),
                            //Username: $.trim(txtMAUsername.val()),
                            //NewPassword: $.trim(txtMANewPassword.val()),
                            //SecQ1ID: mysecq[0].ID,
                            //SecQ1: $.trim(selMASecQuestion1.val()),
                            //SecQ1Ans: $.trim(txtMASecQuestion1Answer.val()),
                            //SecQ2ID: mysecq[1].ID,
                            //SecQ2: $.trim(selMASecQuestion2.val()),
                            //SecQ2Ans: $.trim(txtMASecQuestion2Answer.val()),
                            //IsChangePassword: isChangePs,
                            //IsEditName: isEditName,
                            //IsEditAns1: isEditAns1,
                            //IsEditAns2: isEditAns2,
                            DateNow: $.format.date(new Date(), 'MMM dd, yyyy  hh:mm:ss a'),
                            AccountType: accountype
                        }

                        if (accountype == 'company') {
                            data.CompanyID = companydetails.ID;
                            data.CompanyName = $.trim(txtMACompanyName.val());
                            data.Tin = $.trim(txtMACompanyTIN.val());
                            data.BusinessIndustry = busindid;
                            data.CompanyBackground = $.trim(txtMACompanyBackground.val());
                            //data.CompanyEmailID = companyemail[0].ID || '';
                            //data.CompanyEmailAdd = $.trim(txtMACompanyEmailAdd.val());
                            data.CompanyEmailAddresses = companyemail;
                            data.TelcoPrefixID = $.trim(selMACompanyTelcoPrefix.val());
                            //data.CompanyMobileID = companymobile.ID || '';
                            //data.CompanyMobileNo = $.trim(txtMACompanyMobileNo.val());
                            data.CompanyMobileNos = companymobile;
                            data.CompanyTelephoneNos = companytelephone;
                            data.CompanyBldgNo = $.trim(txtMACompanyAddressBldgNo.val());
                            data.CompanyStreetName = $.trim(txtMACompanyAddressStreetName.val());
                            data.CompanyCity = $.trim(selMACompanyAddressCity.val());
                            data.CompanyCountry = $.trim(selMACompanyAddressCountry.val());
                        }

                        AjaxSend({
                            url: '/Ecommerce/SaveMyAccount',
                            data: { DonotSavePersonDetails: true, data: data },
                            success: function (e) {
                                $('#confirmviapassword').modal('hide');

                                if (e.Results == 'saved') {                                    
                                    WebToast('Changes has been saved', 'top');
                                    mOut.LoadProfile();
                                } else if (e.Results == 'exist') {
                                    NewMessageBox('Your new username already exists', 'Message');
                                } else {
                                    NewMessageBox('There\'s was an error while saving your changes. Please contact your system administrator.', 'Message');
                                }
                            }
                        })

                    } else if (e.Results == 'invalid') {
                        $('#divMAErrMsg').show();
                    } else {
                        NewMessageBox('Your session has expired', 'Message', [{
                            Text: 'Go to login page', Click: function () {
                                location.href = '/Ecommerce/Signin?OrgID=' + sessionStorage.OrgID;
                            }
                        }])
                    }
                }
            })
        }

        function SavePersonDetailsNow() {
            var txtMACurrentPassword = $('#txtMACurrentPassword');
            $('#divMAErrMsg').hide();

            if ($.trim(txtMACurrentPassword.val()) == '') {
                txtMACurrentPassword.parent().addClass('has-error');
                return;
            } else {
                txtMACurrentPassword.parent().removeClass('has-error');
            }

            $('#btnMAContinueSave').text('SAVING ...').attr('disable', true);

            AjaxSend({
                url: '/Ecommerce/CheckPassword',
                data: { password: $.trim(txtMACurrentPassword.val()), PeopleUserID: mydetails.ID },
                success: function (e) {
                    $('#btnMAContinueSave').text('CONTINUE').removeAttr('disabled');

                    if (e.Results == 'valid') {
                        var data = {
                            PeopleUserID: mydetails.ID,
                            FirstName: $.trim(txtMAFirstName.val()),
                            MiddleName: $.trim(txtMAMiddleName.val()),
                            LastName: $.trim(txtMALastName.val()),
                            GenSuffix: $.trim(selMAPersonGenSuffix.val()),
                            DateOfBirth: $.format.date(new Date(selMABirthMonth.val() + ' ' + $.trim(selMABirthDay.val()) + ', ' + $.trim(txtMABirthYear.val()))),
                            PersonEmailAdd: $.trim(txtMAEmailAddress.val()),
                            Username: $.trim(txtMAUsername.val()),
                            NewPassword: $.trim(txtMANewPassword.val()),
                            SecQ1ID: mysecq[0].ID,
                            SecQ1: $.trim(selMASecQuestion1.val()),
                            SecQ1Ans: $.trim(txtMASecQuestion1Answer.val()),
                            SecQ2ID: mysecq[1].ID,
                            SecQ2: $.trim(selMASecQuestion2.val()),
                            SecQ2Ans: $.trim(txtMASecQuestion2Answer.val()),
                            IsChangePassword: isChangePs,
                            IsEditName: isEditName,
                            IsEditAns1: isEditAns1,
                            IsEditAns2: isEditAns2,
                            DateNow: $.format.date(new Date(), 'MMM dd, yyyy hh:mm:ss a'),
                            AccountType: accountype
                        }

                        AjaxSend({
                            url: '/Ecommerce/SaveMyAccount',
                            data: { DonotSaveCompanyDetails: true, data: data },
                            success: function (e) {
                                $('#confirmviapassword').modal('hide');

                                if (e.Results == 'saved') {
                                    WebToast('Changes has been saved', 'top');
                                    GetCompanyUsers();
                                } else if (e.Results == 'exist') {
                                    NewMessageBox('Your new username already exists', 'Message');
                                } else {
                                    NewMessageBox('There\'s was an error while saving your changes. Please contact your system administrator.', 'Message');
                                }
                            }
                        })

                    } else if (e.Results == 'invalid') {
                        $('#divMAErrMsg').show();
                    } else {
                        NewMessageBox('Your session has expired', 'Message', [{
                            Text: 'Go to login page', Click: function () {
                                location.href = '/Ecommerce/Signin?OrgID=' + sessionStorage.OrgID;
                            }
                        }])
                    }
                }
            })
        }

        function ValidateCompanyDetails() {
            var hasError = false;

            $('#divErrMsg').text('Please complete the required field(s)').hide();

            if ($.trim(txtMACompanyName.val()) == '') {
                txtMACompanyName.parent().addClass('has-error');
                $('#divErrMsg').append('<br>').append('- Enter company name').show();
                hasError = true;
            } else {
                txtMACompanyName.parent().removeClass('has-error');
            }

            if (busindid == '') {
                txtMABusInd.parent().addClass('has-error');
                $('#divErrMsg').append('<br>').append('- Enter your business industry').show();
                hasError = true;
            } else {
                txtMABusInd.parent().removeClass('has-error');
            }

            for (var i = 0; i < companyemail.length; i++){
                if (companyemail[i].State == 'remove') {
                    $('#divErrMsg').append('<br class="spanerremailadd">').append('<span class="spanerremailadd">- Enter at least one company email address</span>').show();
                    hasError = true;
                } else {
                    hasError = false;
                    $('.spanerremailadd').remove();
                    break;
                }
            }

            for (var i = 0; i < companymobile.length; i++) {
                if (companymobile[i].State == 'remove') {
                    $('#divErrMsg').append('<br class="spanerrmobile">').append('<span class="spanerrmobile">- Enter at least one company mobile number</span>').show();
                    hasError = true;
                } else {
                    hasError = false;
                    $('.spanerrmobile').remove();
                    break;
                }
            }

            //if ($.trim(txtMACompanyEmailAdd.val()) == '') {
            //    txtMACompanyEmailAdd.parent().addClass('has-error');
            //    $('#divErrMsg').append('<br>').append('- Enter valid company email address').show();
            //    hasError = true;
            //} else {
            //    if (!IsEmail($.trim(txtMACompanyEmailAdd.val()))) {
            //        txtMACompanyEmailAdd.parent().addClass('has-error');
            //        hasError = true;
            //        $('#divErrMsg').append('<br>').append('- Enter valid company email address').show();
            //    } else {
            //        txtMACompanyEmailAdd.parent().removeClass('has-error');
            //    }
            //}

            //if ($.trim(txtMACompanyMobileNo.val()) == '') {
            //    txtMACompanyMobileNo.parent().addClass('has-error');
            //    $('#divErrMsg').append('<br>').append('- Enter valid company mobile number').show();
            //    hasError = true;
            //} else {
            //    if ($.trim(txtMACompanyMobileNo.val()).length < 10) {
            //        txtMACompanyMobileNo.parent().addClass('has-error');
            //        hasError = true;
            //        $('#divErrMsg').append('<br>').append('- Enter valid company mobile number').show();
            //    } else {
            //        txtMACompanyMobileNo.parent().removeClass('has-error');
            //    }
            //}

            if ($.trim(txtMACompanyAddressBldgNo.val()) == '') {
                txtMACompanyAddressBldgNo.parent().addClass('has-error');
                $('#divErrMsg').append('<br>').append('- Enter company house/bldg number address').show();
                hasError = true;
            } else {
                txtMACompanyAddressBldgNo.parent().removeClass('has-error');
            }

            if ($.trim(txtMACompanyAddressStreetName.val()) == '') {
                txtMACompanyAddressStreetName.parent().addClass('has-error');
                $('#divErrMsg').append('<br>').append('- Enter company street address').show();
                hasError = true;
            } else {
                txtMACompanyAddressStreetName.parent().removeClass('has-error');
            }

            if ($.trim(selMACompanyAddressCity.find('option:selected').val()) == '') {
                selMACompanyAddressCity.parent().addClass('has-error');
                $('#divErrMsg').append('<br>').append('- Select company city address').show();
                hasError = true;
            } else {
                selMACompanyAddressCity.parent().removeClass('has-error');
            }

            if ($.trim(selMACompanyAddressCountry.find('option:selected').val()) == '') {
                selMACompanyAddressCountry.parent().addClass('has-error');
                $('#divErrMsg').append('<br>').append('- Select company city address').show();
                hasError = true;
            } else {
                selMACompanyAddressCountry.parent().removeClass('has-error');
            }

            if (hasError) {
                $('#divErrMsg').show();
                return false;
            } else {
                $('#divErrMsg').text('');
                return true;
            }
        }

        function ValidatePersonDetails() {
            var hasError = false;

            $('#divErrMsg').text('Please complete the required field(s)').hide();

            if (isEditName) {
                if ($.trim(txtMAFirstName.val()) == '') {
                    txtMAFirstName.parent().addClass('has-error');
                    $('#divErrMsg').append('<br>').append('- Enter your First Name').show();
                    hasError = true;
                } else {
                    txtMAFirstName.parent().removeClass('has-error');
                }

                if ($.trim(txtMALastName.val()) == '') {
                    txtMALastName.parent().addClass('has-error');
                    $('#divErrMsg').append('<br>').append('- Enter your Last Name').show();
                    hasError = true;
                } else {
                    txtMALastName.parent().removeClass('has-error');
                }
            }

            if ($.trim(selMABirthMonth.val()) == '' || $.trim(selMABirthDay.val()) == '' || $.trim(txtMABirthYear.val()) == '') {
                selMABirthMonth.parent().addClass('has-error');
                $('#divErrMsg').append('<br>').append('- Date of Birth is required').show();
                hasError = true;
            } else {
                selMABirthMonth.parent().removeClass('has-error');
            }

            if ($.trim(txtMAEmailAddress.val()) == '') {
                txtMAEmailAddress.parent().addClass('has-error');
                hasError = true;
            } else {
                if (!IsEmail($.trim(txtMAEmailAddress.val()))) {
                    txtMAEmailAddress.parent().addClass('has-error');
                    hasError = true;
                    $('#divErrMsg').append('<br>').append('- Enter valid personal email address').show();
                } else {
                    txtMAEmailAddress.parent().removeClass('has-error');
                }
            }

            if (isChangePs) {
                if ($.trim(txtMAUsername.val()) == '') {
                    txtMAUsername.parent().addClass('has-error');
                    $('#divErrMsg').append('<br>').append('Enter new username. Must be 8 characters or more.').show();
                    hasError = true;
                } else {
                    if ($.trim(txtMAUsername.val()).length < 8) {
                        txtMAUsername.parent().addClass('has-error');
                        hasError = true;
                        $('#divErrMsg').append('<br>').append('Username must be 8 characters or more').show();
                        return false;
                    } else {
                        txtMAUsername.parent().removeClass('has-error');
                    }
                }

                if ($.trim(txtMANewPassword.val()) == '') {
                    txtMANewPassword.parent().addClass('has-error');
                    $('#divErrMsg').append('<br>').append('Enter new password. Must be 8 characters or more.').show();
                    hasError = true;
                } else {
                    if ($.trim(txtMANewPassword.val()).length < 8) {
                        txtMANewPassword.parent().addClass('has-error');
                        hasError = true;
                        $('#divErrMsg').append('<br>').append('Password must be 8 characters or more').show();
                        return false;
                    } else {
                        txtMANewPassword.parent().removeClass('has-error');
                    }
                }
            }

            if ($.trim(selMASecQuestion1.val()) == '') {
                selMASecQuestion1.parent().addClass('has-error');
                hasError = true;
            } else {
                selMASecQuestion1.parent().removeClass('has-error');
            }

            if (isEditAns1) {
                if ($.trim(txtMASecQuestion1Answer.val()) == '') {
                    txtMASecQuestion1Answer.parent().addClass('has-error');
                    $('#divErrMsg').append('<br>').append('- Enter your answer for Security Question No. 1').show();
                    hasError = true;
                } else {
                    txtMASecQuestion1Answer.parent().removeClass('has-error');
                }
            }

            if ($.trim(selMASecQuestion2.val()) == '') {
                selMASecQuestion2.parent().addClass('has-error');
                hasError = true;
            } else {
                selMASecQuestion2.parent().removeClass('has-error');
            }

            if (isEditAns2) {
                if ($.trim(txtMASecQuestion2Answer.val()) == '') {
                    txtMASecQuestion2Answer.parent().addClass('has-error');
                    $('#divErrMsg').append('<br>').append('- Enter your answer for Security Question No. 2').show();
                    hasError = true;
                } else {
                    txtMASecQuestion2Answer.parent().removeClass('has-error');
                }
            }

            if (hasError) {
                $('#divErrMsg').show();
                return false;
            } else {
                $('#divErrMsg').text('');
                return true;
            }
        }

        return mOut;

    });

    return mDeferred.promise();
})