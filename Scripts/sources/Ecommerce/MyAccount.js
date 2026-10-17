require(['common', 'promise!datatables.net'], function () {
    $(document).ready(function () {
        var mydetails, myemail, mycredentials, mysecq, busindid = '', isEditName = false, isChangePs = false, isEditAns1 = false, isEditAns2 = false;
        var accountype, companydetails, companyemail, companymobile;
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
        var txtMACompanyEmailAdd = $('#txtMACompanyEmailAdd');
        var selMACompanyTelcoPrefix = $('#selMACompanyTelcoPrefix');
        var txtMACompanyMobileNo = $('#txtMACompanyMobileNo');
        var txtMACompanyAddressBldgNo = $('#txtMACompanyAddressBldgNo');
        var txtMACompanyAddressStreetName = $('#txtMACompanyAddressStreetName');
        var selMACompanyAddressCity = $('#selMACompanyAddressCity');
        var selMACompanyAddressCountry = $('#selMACompanyAddressCountry');
        var mCustomerID;

        var addressID = '', addressAction = 'add';

        $(function () {
            AjaxSend({
                url: MergeUrlParameters(GetUrlParameters(), '/Ecommerce/MyAccountDetails'),
                success: function (e) {                    
                    LoadData(e);
                }
            })
        })

        $('body')
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
                $('#txtMAUsername, #txtMANewPassword').val('');
            })
            .on('click', '#btnMACancelNewPassword', function () {
                isChangePs = false;
                $('#divActualPassword').pageshow();
                $('#txtMANewPassword').val('');
            })
            .on('click', '#btnMASaveChanges', function (e) {
                if (!Validate()) {
                    e.stopPropagation();
                    e.preventDefault();
                } else {
                    $('#txtMACurrentPassword').val('');
                }
            })
            .on('click', '#btnMAContinueSave', function () {
                SaveNow();
            })
            .on('keypress', '#txtMACurrentPassword', function (e) {
                if(e.which == 13){                
                    SaveNow();
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

            .on('click', '#navMyAccount li', function () {
                $('#navMyAccount li a').removeClass('active');
                $(this).find('a').addClass('active');
            })

            .on('click', '#btnMAMyProfile', function () {
                $('#divMyProfile').pageshow();
            })
            .on('click', '#btnMAAddressBook', function () {
                $('#divMyAddressBook').pageshow();
                GetAddresses();
            })
            .on('click', '#btnMAOrderHistory, #linkMyOrderHistory', function () {
                $('#divMyOrderHistory, #divOrderList').pageshow();
                GetMyOrder();
            })
            .on('click', '#btnMAAccountsPayable', function () {
                if (mCustomerID == null) {
                    AjaxSend({
                        url: 'Ecommerce/GetEcommerceCustomerID',
                        success: function (e) {
                            mCustomerID = e.Results;
                            Proceed();
                        }
                    });
                } else
                    Proceed();

                function Proceed() {
                    require(['promise!/Scripts/sources/ERP/AccountDetails/controller.js?v=' + assetversion,
                        '/Scripts/sources/CreditAndCollection/CacPaymentPopup.js?v=' + assetversion], function() {
                        ERP.AccountDetails.controller.Mode = 'AR';
                        ERP.AccountDetails.controller.CustomerID = mCustomerID;
                        ERP.AccountDetails.controller.FromEcommerce = true;
                        //ERP.AccountDetails.controller.CustomerName = mCustomerName;
                        $('#divMAAccountsPayable').pageshow();
                        ERP.AccountDetails.controller.GetCustomerCreditDetails(mCustomerID, 'has payable');
                    });
                }
            })
            .on('click', '#btnMAAddAddress', function (e) {
                $('#txtCompleteAddress').val('').focus();
                addressID = '';
                addressAction = 'add';
            })
            .on('click', '#btnAddressSave', function (e) {
                e.preventDefault();
                e.stopPropagation();
                SaveAddress();
            })

        if (location.hash == '#accountspayable') {
            $('#btnMAAccountsPayable').click()
        } else if (location.hash == '#orderhistory') {
            $('#btnMAOrderHistory').click()
        } else if (location.hash == '#addressbook') {
            $('#btnMAAddressBook').click()
        } else if (location.hash == '#accountspayable') {
            $('#btnMAAccountsPayable').click()
        } else {
            $('#btnMAMyProfile').click()
        }

        var tblMyOrderList;
        function GetMyOrder() {
            AjaxSend({
                url: 'Ecommerce/GetEcommerceCustomerID',
                success: function (e) {
                    mCustomerID = e.Results;
                    AjaxSend({
                        url: 'Browser/TransInvoices',
                        data: {
                            CustomerID: e.Results
                        }, success: function (e) {
                            var vRows = e.Results.Rows;
                            var tbodyMyOrderList = $('#tbodyMyOrderList');
                            var ctr = 0;

                            tbodyMyOrderList.empty()

                            if ($.fn.DataTable.isDataTable("#tblMyOrderList")) {
                                //check if datatable is created 
                                $('#tblMyOrderList').DataTable().clear().destroy();
                            }

                            tblMyOrderList = $('#tblMyOrderList').DataTable({
                                retrieve: true,
                                "bdestroy": true,
                                "pageLength": 10,
                                "data": vRows,
                                "autoWidth": false,
                                "ordering": false,
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
                                            return 'SO-' + data.MIN + '-' + data.SalesOrderNo;
                                        }
                                    },
                                    {
                                        "data": function (data, type, full, meta) {
                                            return '<span title="Billing Address" class="badge ba">BA:</span> ' + (data.BillingAddress == undefined ? 'N/A' + '<br>' : 'N/A' + '<br>') + '<span title="Delivery Address" class="badge da">DA:</span> ' + (data.DeliveryAddress == undefined ? 'N/A' : 'N/A')
                                        }
                                    },
                                    {
                                        "data": function (data, type, full, meta) {
                                            return (data.SalesOrderDate == undefined ? '' : $.format.date(new Date(data.SalesOrderDate), 'MM-dd-yyyy hh:mm a'));
                                        }
                                    },
                                    {
                                        "data": function (data, type, full, meta) {
                                            return AccountingFormat(data.OrderedAmount);
                                        }
                                    }
                                ],
                                "initComplete": function (row, data) {
                                    $('#tblMyOrderList').removeClass('dataTable');
                                },
                                "aoColumnDefs": [{ "bSortable": false, "aTargets": ['_all'] }],
                                "responsive": true,
                                "columnDefs": [{ "orderable": false }],
                                "sDom": '<"top">rt<"bottom"ip><"clear">'
                                //"stateSave": true
                            })

                            $('#tblMyOrderList tbody').unbind('click').on('click', 'tr', function () {
                                var data = tblMyOrderList.row(this).data();
                                if (CStr2(data) != '') {
                                    $('#divOrderDetails').pageshow();
                                    GetOrderDetails(data);
                                }
                            });
                        }
                    })
                }
            })
        }

        function GetOrderDetails(data) {
            AjaxSend({
                url: 'Ecommerce/GetOrderDetails',
                data: { OrderID: data.ID },
                success: function (e) {
                    var details = e.Invoices.Rows[0];
                    var stocks = e.Stocks.Rows;
                    var orderTotal = 0;
                    var tbodyMyOrderSku = $('#tbodyMyOrderSku');

                    tbodyMyOrderSku.empty();

                    $('#lblODOrderNo').text('SO-' + details.MIN + '-' + details.SalesOrderNo);
                    $('#lblODBillingAddress').text('N/A'); //details.BillingAddress
                    $('#lblODDeliveryAddress').text('N/A'); //details.DeliveryAddress
                    $('#lblODOrderDate').text((details.SalesOrderDate == undefined ? '' : $.format.date(new Date(details.SalesOrderDate), 'MM-dd-yyyy hh:mm a')));
                    $('#lblODOrderTotal').text(AccountingFormat(details.OrderedAmount));

                    if (stocks[0].VendorCode == undefined) {
                        $('#thOHVendor').hide();
                    } else {
                        $('#thOHVendor').show();
                    }

                    for (var i = 0; i < stocks.length; i++) {
                        var tempPic = '/Content/Ecommerce/images/products/p1.jpg';
                        var lineTotal = Ecommerce.ApplyUpcharge(stocks[i]);
                        orderTotal += CDec2(lineTotal);

                        //if (CStr2(stocks[i].FileName) != '') {
                        //    tempPic = '/Content/MobizImagePIM/' + sessionStorage.OrgID + '/medium/' + stocks[i].FileName;
                        //}

                        tbodyMyOrderSku.append($('<tr>')
                                //.append($('<td>').append($('<img style="width:50px;">').attr('src', tempPic)))
                                .append($('<td class="cart-vendor-info" style="display:' + (CStr2(stocks[i].VendorCode) != '' ? 'block' : 'none') + '">')
                                    .append((CStr2(stocks[i].VendorCode) != '' ? 'Code: ' + stocks[i].VendorCode : '') +
                                            (CStr2(stocks[i].VendorName) != '' ? '<br>Name: ' + stocks[i].VendorName : '')
                                    )
                                )
                                .append($('<td>').text(stocks[i].ProductName))
                                .append($('<td style="text-align:center;">').text(CDec2(stocks[i].OrderedQuantity)))
                                .append($('<td style="text-align:right;">').text('P ' + AccountingFormat(stocks[i].UnitPrice) + ' per ' + stocks[i].UnitMeasure)
                                    .append($('<div id="divupcharge-' + stocks[i].ID + '">').append(CDec2(stocks[i].Upcharge) == 0 ? '' : '+ ' + CDec2(stocks[i].Upcharge) + '% upcharge for orders below 1 ' + stocks[i].UnitMeasure))
                                )
                                .append($('<td style="text-align:right;">').text('P ' + AccountingFormat(lineTotal)))
                            )
                    }

                    tbodyMyOrderSku.append($('<tr>')
                                .append($('<td colspan="5" style="text-align:right; font-weight:bold;">').text('TOTAL: P ' + AccountingFormat(orderTotal)))
                            )
                }
            })
        }

        function SaveAddress() {
            var txtCompleteAddress = $.trim($('#txtCompleteAddress').val());

            if(txtCompleteAddress == '' && addressAction != 'delete'){
                WebToast('Please enter complete address', 'top');
                return;
            }

            var data = {
                ID: addressID,
                Address: txtCompleteAddress,
                Action: addressAction
            }

            AjaxSend({
                url: 'Ecommerce/SaveAddress',
                data: { data: data },
                success: function (e) {
                    if (e.Results == 'saved') {
                        GetAddresses();
                        $('#popupNewAddress').modal('hide');
                    } else {
                        WebToast(e.Results, 'top');
                    }
                }
            })
        }

        var tblMyAddress;
        function GetAddresses() {
            var tbodyMyAddress = $('#tbodyMyAddress');

            tbodyMyAddress.empty();

            AjaxSend({
                url: 'Ecommerce/GetEcommerceCustomerID',
                success: function (e) {
                    AjaxSend({
                        url: 'Browser/LibraryFowst',
                        data: {
                            OrganizationID: e.Results,
                            HideNoAddress: 1
                        }, success: function (e) {
                            var vRows = e.Results.Rows;
                            var ctr = 0;

                            if ($.fn.DataTable.isDataTable("#tblMyAddress")) {
                                //check if datatable is created 
                                $('#tblMyAddress').DataTable().clear().destroy();
                            }

                            tblMyAddress = $('#tblMyAddress').DataTable({
                                retrieve: true,
                                "bdestroy": true,
                                "pageLength": 10,
                                "data": vRows,
                                "autoWidth": false,
                                "ordering": false,
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
                                            return data.Address;
                                        }
                                    },
                                    {
                                        data: null,
                                        createdCell: function (td, cellData, rowData, row, col) {
                                            var edit = $('<button data-role="none" class="btn btn-warning">')
                                                .append($('<span class="bi-pencil">'))
                                                .click(function () {
                                                    $('#popupNewAddress').modal('show');
                                                    $('#txtCompleteAddress').val(rowData.Address).focus();
                                                    addressID = rowData.ID;
                                                    addressAction = 'edit';
                                                });
                                            $(td).html(edit);
                                            $(td).append('&nbsp;')
                                            var remove = $('<button data-role="none" class="btn btn-danger">')
                                                .append($('<span class="bi-trash">'))
                                                .click(function () {
                                                    addressID = rowData.ID;
                                                    addressAction = 'delete';

                                                    NewMessageBox('Are you sure you want to remove this address (' + rowData.Address + ')?', 'Confirmation Message', [
                                                        {
                                                            Text: 'Yes', Click: function () {
                                                                SaveAddress();
                                                            }
                                                        }, { Text: 'No' }
                                                    ])
                                                });
                                            $(td).css('text-align', 'center').append(remove);
                                        },
                                        "width": "150px"
                                    }
                                ],
                                "initComplete": function (row, data) {
                                    $('#tblMyAddress').removeClass('dataTable');
                                },
                                "aoColumnDefs": [{ "bSortable": false, "aTargets": ['_all'] }],
                                "responsive": true,
                                "columnDefs": [{ "orderable": false }],
                                "sDom": '<"top">rt<"bottom"ip><"clear">'
                                //"stateSave": true
                            })
                        }
                    })
                }
            })            
        }
        
        function SaveNow() {
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
                data: { password: $.trim(txtMACurrentPassword.val()) },
                success: function (e) {
                    $('#btnMAContinueSave').text('CONTINUE').removeAttr('disabled');

                    if (e.Results == 'valid') {
                        var data = {
                            PeopleUserID: sessionStorage.PeopleUserID,
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
                            DateNow: $.format.date(new Date(), 'MMM dd, yyyy'),
                            AccountType: accountype
                        }

                        if (accountype == 'company') {
                            data.CompanyID = companydetails.ID;
                            data.CompanyName = $.trim(txtMACompanyName.val());
                            data.Tin = $.trim(txtMACompanyTIN.val());
                            data.BusinessIndustry = busindid;
                            data.CompanyEmailID = companyemail[0].ID || '';
                            data.CompanyEmailAdd = $.trim(txtMACompanyEmailAdd.val());
                            data.TelcoPrefixID = $.trim(selMACompanyTelcoPrefix.val());
                            data.CompanyMobileID = companymobile[0].ID || '';
                            data.CompanyMobileNo = $.trim(txtMACompanyMobileNo.val());
                            data.CompanyBldgNo = $.trim(txtMACompanyAddressBldgNo.val());
                            data.CompanyStreetName = $.trim(txtMACompanyAddressStreetName.val());
                            data.CompanyCity = $.trim(selMACompanyAddressCity.val());
                            data.CompanyCountry = $.trim(selMACompanyAddressCountry.val());
                        }

                        AjaxSend({
                            url: '/Ecommerce/SaveMyAccount',
                            data: { data: data },
                            success: function (e) {
                                if (e.Results == 'saved') {
                                    $('#btnMACancelSave').click();
                                    WebToast('Changes has been saved', 'top');
                                    AjaxSend({
                                        url: '/Ecommerce/MyAccountDetails',
                                        success: function (e) {
                                            LoadData(e);
                                        }
                                    })
                                } else if (e.Results == 'exist') {
                                    NewMessageBox('Your new username already exists', 'Message');
                                } else {
                                    NewMessageBox('There\'s an error while saving changes. Please contact your system administrator.', 'Message');
                                }
                            }
                        })

                    } else if(e.Results == 'invalid'){
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

        function Validate() {            
            var hasError = false;

            $('#divErrMsg').text('Please complete the required field(s)').hide();

            if (accountype == 'company'){
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

                if ($.trim(txtMACompanyEmailAdd.val()) == '') {
                    txtMACompanyEmailAdd.parent().addClass('has-error');
                    $('#divErrMsg').append('<br>').append('- Enter valid company email address').show();
                    hasError = true;
                } else {
                    if (!IsEmail($.trim(txtMACompanyEmailAdd.val()))) {
                        txtMACompanyEmailAdd.parent().addClass('has-error');
                        hasError = true;
                        $('#divErrMsg').append('<br>').append('- Enter valid company email address').show();
                    } else {
                        txtMACompanyEmailAdd.parent().removeClass('has-error');
                    }
                }

                if ($.trim(txtMACompanyMobileNo.val()) == '') {
                    txtMACompanyMobileNo.parent().addClass('has-error');
                    $('#divErrMsg').append('<br>').append('- Enter valid company mobile number').show();
                    hasError = true;
                } else {
                    if ($.trim(txtMACompanyMobileNo.val()).length < 10) {
                        txtMACompanyMobileNo.parent().addClass('has-error');
                        hasError = true;
                        $('#divErrMsg').append('<br>').append('- Enter valid company mobile number').show();
                    } else {
                        txtMACompanyMobileNo.parent().removeClass('has-error');
                    }
                }

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
            }

            if (isEditName){
                if ($.trim(txtMAFirstName.val()) == '') {
                    txtMAFirstName.parent().addClass('has-error');
                    hasError = true;
                } else {
                    txtMAFirstName.parent().removeClass('has-error');
                }

                if ($.trim(txtMALastName.val()) == '') {
                    txtMALastName.parent().addClass('has-error');
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
                    $('#divErrMsg').append('<br>').append('- Enter valid email address').show();                    
                    return false;
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

            if(isEditAns1){
                if ($.trim(txtMASecQuestion1Answer.val()) == '') {
                    txtMASecQuestion1Answer.parent().addClass('has-error');
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

            if(isEditAns2){
                if ($.trim(txtMASecQuestion2Answer.val()) == '') {
                    txtMASecQuestion2Answer.parent().addClass('has-error');
                    hasError = true;
                } else {
                    txtMASecQuestion2Answer.parent().removeClass('has-error');
                }
            }

            if (hasError) {
                $('#divErrMsg').show();
                return false;
            } else {
                return true;
            }
        }
        
        function LoadData(data) {
            //var data = myaccountdetails.d; // myaccountdetails declared in EcommerceLayout.vbhtml
            mydetails = data.Details[0];
            myemail = data.Email[0];
            mycredentials = data.Credentials[0];
            mysecq = data.SecQ;
            accountype = data.AccountType;

            isEditName = false;
            isChangePs = false;
            isEditAns1 = false;
            isEditAns2 = false;            

            selMABirthDay.empty().append(new Option('Day', ''));

            for (var i = 1; i <= 31; i++){
                selMABirthDay.append(new Option(i, i))
            }

            var selMACompanyAddressCity = $('#selMACompanyAddressCity');
            var selMACompanyAddressCountry = $('#selMACompanyAddressCountry');
            var selMAPersonAddressCity = $('#selMAPersonAddressCity');
            var selMAPersonAddressCountry = $('#selMAPersonAddressCountry');

            selMACompanyAddressCity.append(new Option('Please select City', ''));
            selMAPersonAddressCity.append(new Option('Please select City', ''));
            for (var i = 0; i < data.Cities.length; i++) {
                selMACompanyAddressCity.append(new Option(data.Cities[i].Name, data.Cities[i].ID));
                selMAPersonAddressCity.append(new Option(data.Cities[i].Name, data.Cities[i].ID));
            }

            for (var i = 0; i < data.Countries.length; i++) {
                selMACompanyAddressCountry.append(new Option(data.Countries[i].Name, data.Countries[i].ID));
                selMAPersonAddressCountry.append(new Option(data.Countries[i].Name, data.Countries[i].ID));
            }

            if(data.AccountType == 'company'){
                $('#tabCompanyProfile').opentabmenu();
                $('[href="#tabCompanyProfile"]').removeClass('hidetab');

                companydetails = data.CompanyDetails[0];
                companyemail = data.CompanyEmail;
                companymobile = data.CompanyMobile;

                busindid = CStr2(companydetails.IndustryClassID);

                $('#txtMACompanyName').val(companydetails.CompanyName);
                $('#txtMACompanyTIN').val(companydetails.TinNumber);
                $('#txtMABusInd').val(CStr2(companydetails.IndustryClassName));

                if (companyemail.length > 0) {
                    $('#txtMACompanyEmailAdd').val(companyemail[0].EmailAddress);
                }

                if (companyemail.length > 0) {
                    $('#txtMACompanyMobileNo').val(companymobile[0].ContactNo);
                }
                
                $('#txtMACompanyAddressBldgNo').val(companydetails.OfficeAddressBuildingNo);
                $('#txtMACompanyAddressStreetName').val(companydetails.OfficeAddressStreet);
                $('#selMACompanyAddressCity').val(companydetails.OfficeAddressCityID);
                $('#selMACompanyAddressCountry').val(companydetails.OfficeAddressCountryID);
            } else {
                $('#tabAdminInfo').opentabmenu();
                $('[custom-tab-content="#tabCompanyProfile"]').addClass('hidetab');
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

            AjaxSend({
                url: 'Browser/LibraryCities',
                data: { IsAll: 1 },
                success: function (e) {
                    var vRows = e.Results.Rows;
                    var selAddressCity = $('#selAddressCity');

                    selAddressCity.empty();
                    selAddressCity.append(new Option('Please select', ''));

                    for (var i = 0; i < vRows.length; i++){
                        selAddressCity.append(new Option(vRows[i].Name, vRows[i].ID));
                    }
                }
            })
        }
    })
})