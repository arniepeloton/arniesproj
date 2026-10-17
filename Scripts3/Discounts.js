require(['common', GetPathFormat('/Scripts3/Browser/LibraryPeoplePreSuffix.js?crc='),
    GetPathFormat('/Scripts3/Browser/LibraryPeoplePostSuffix.js?crc=')], function () {

        $(function () {

            var result;
            //////////////////////////////////////////////////////////////////
            //additional by janine
            var showDiscountDetails = false;
            var isSeniorCitizenDiscount = false;
            var IsNewSalesPayCustomerIdentification = true;
            var arSalesPayCustomerIdentification = new Array();
            var intSalesPayCustomerIdentificationCntr = 0;
            var intSalesPayCustomerIdentificationIndex = 0;

            var intSalesPayCustomerIdentificationIDType = 0;
            var intSalesPayCustomerIdentificationIDOtherType = 1;
            var intSalesPayCustomerIdentificationIDText = 2;
            var intSalesPayCustomerIdentificationState = 3;

            var IsNewSalesPayCustomerName = true;
            var arSalesPayCustomerName = new Array();
            var intSalesPayCustomerNameCntr = 0;
            var intSalesPayCustomerNameIndex = 0;

            var intSalesPayCustomerNameID = 0;
            var intSalesPayCustomerNameTitleID = 1;
            var intSalesPayCustomerNameTitleName = 2;
            var intSalesPayCustomerNameSurname = 3;
            var intSalesPayCustomerNameFirstName = 4;
            var intSalesPayCustomerNameMiddleName = 5;
            var intSalesPayCustomerNameSuffix = 6;
            var intSalesPayCustomerNameDOBMonth = 7;
            var intSalesPayCustomerNameDOBDay = 8;
            var intSalesPayCustomerNameDOBYear = 9;
            var intSalesPayCustomerNameState = 10;
            var intSalesPayCustomerNameFullName = 11;

            var isSeniorCitizenPWDDiscount = false;
            sessionStorage.IsDiscountPageLoaded = 'true';
            sessionStorage.IsSeniorCitizenPWDPageLoaded = 'false';

            window.pageDiscounts = {};

            //            AjaxSend({
            //                ExactUrl: '/SalesPayments/Discounts?cache=1',
            //                type: 'GET',
            //                dataType: 'text',
            //                success: function (vData) {
            //                    $('#popupDiscounts').append(vData).trigger('create');
            //                
            //                    App.Deferred.fulfill('FQ26E5TP76');
            //                }
            //            });

            if ($(window).width() > proj.WideWidth)
                $('#lstCustomerContainerBilling').show();
            else
                $('#lstCustomerContainerBilling').hide();



            $('#btnAddNewCustomerBilling').live('click', function (e) {
                e.preventDefault();

                seniorCitizenDetailsPage('', '');

            });

            $('#frmCustomerSearchBilling').live('submit', function (e) {
                e.preventDefault();

                if (location.hash == '#payment')
                    sessionStorage.CurrentPage = 'payment';
                else
                    sessionStorage.CurrentPage = 'discount';

                window.pageSourcePage = $.mobile.activePage;

                sessionStorage.IsBrowserSearch = 1;
                //filter to show only person type customers
                Browser.LibraryOrganization.InputData.FilterTypePerson = 1;
                Browser.LibraryOrganization.InputData.FilterCustomers = 1;
                Browser.LibraryOrganization.Click = function (e) {
                    if (OfflineMode) {

                        //                            if (window.location.pathname.toLowerCase() == '/salesmenu/posviewall') {

                        //                            location.hash = '';
                        //                                                        $.mobile.back();

                        var vData = SalesPayment_offline().saveSeniorCitizenPWDCustomer(e.ID, sessionStorage.CartOrderID);


                        if (vData == 'ok') {
                            displaySeniorCitizenInfoList(false);
                        } else {
                            MessageBox(vData);
                        }

                        //                            }



                    } else {
                        MessageBox('Not yet implemented');

                    } // end if offline


                };

                Browser.LibraryOrganization.Show($('#txtCustomerSearch').val());

            });


            $('#btnSalesPayCustomerNameRemove').live('click', function (e) {
                e.preventDefault();

                MessageBox("Remove this record?",
                                'Remove',
                                [
                                    { Text: 'Yes',
                                        Click: function () {
                                            if (location.hash == '#payment') {
                                                sessionStorage.CurrentPage = 'payment';
                                                SalesPayment_offline().removeSeniorCitizenPWDCustomer($("#txtSCPWDCustomerID").val());
                                                proj.RemoveSalesPayCustomerName();
                                            } else {
                                                sessionStorage.CurrentPage = 'discount';

                                                RemoveSavedSalesPayCustomerName();
                                            }


                                        }
                                    }
                                ,
                                    { Text: 'No'
                                    }
                                ]
                            );


            });



            /////////////////////

            $('#btnTitle').live('click', function () {

                Browser.LibraryPeoplePreSuffix.Click = function (e) {

                    $('#txtTitleID').val(e.ID)
                    $('#btnTitle').text(e.Code)
                    .button('refresh');

                };
                Browser.LibraryPeoplePreSuffix.Show();

            });

            $('#selDOBMonth').live('change', function (e) {
                e.preventDefault();

                setDayCollection('selDOBMonth', 'selDOBDay', 'selDOBYear', true);

            });

            $('#selDOBYear').live('change', function (e) {
                e.preventDefault();

                setDayCollection('selDOBMonth', 'selDOBDay', 'selDOBYear', true);

            });

            $('#selDOBYear').live('click', function (e) {
                e.preventDefault();

                if ($.trim($('#selDOBYear').val()) == "") {
                    $('#selDOBYear').val('1980');
                    $('#selDOBYear').selectmenu('refresh');
                }

            });


            $('#selIDType').live('change', function (e) {
                e.preventDefault();

                $('#txtOtherIDType').val('');
                if ($(this).val() == 'Other')
                    $('#divOtherIDType').show();
                else
                    $('#divOtherIDType').hide();


                ///////////////////////////////

                for (var i = 0; i <= intSalesPayCustomerIdentificationCntr; i++) {

                    if (sessionStorage["SalesPayCustomerIdentification-" + i] != undefined) {

                        arSalesPayCustomerIdentification = new Array();
                        arSalesPayCustomerIdentification = JSON.parse(sessionStorage["SalesPayCustomerIdentification-" + i]);
                        if (arSalesPayCustomerIdentification[intSalesPayCustomerIdentificationState] != "Remove") {
                            IsNewSalesPayCustomerIdentification = false;

                            intSalesPayCustomerIdentificationIndex = i;

                            $('#txtIDText').val('');
                        }

                    }
                }

            });



            $('#btnSCPWDDiscountCustomerInfo').live('click', function (e) {
                e.preventDefault();

                if (window.location.pathname.toLowerCase() == '/salesmenu/posviewall')
                    window.pageHeader.ShowPaymentsPage(true);
                else
                    $.mobile.changePage('#pageSalesPayment');
                ClearSalesPaySeniorCitizenDiscountFields();

            });





            $('#btnSalesPayCustomerIdentificationAdd').live('click', function (e) {
                e.preventDefault();

                if (1 == CInt2($('#lstSalesPayCustomerIdentification a').length)) {
                    NewMessageBox('You\'ve already added an ID.');
                    return;
                }

                IsNewSalesPayCustomerIdentification = true;
                $('#btnSalesPayCustomerIdentificationRemove').hide();

                if (proj.IsBootstrapDesign()) {
                    $('#popupAddNewID').modal('toggle');
                } else {
                    $('#popupAddNewID').popup('open');
                }

                ClearSalesPayCustomerIdentificationFields();

                //                intSalesPayCustomerIdentificationCntr = intSalesPayCustomerIdentificationCntr + 1;
                //                intSalesPayCustomerIdentificationIndex = intSalesPayCustomerIdentificationCntr;

            });

            $('#txtFullname').live('keyup', function () {
                if ($.trim($(this).val()) == "") {
                    if (proj.IsBootstrapDesign()) {
                        $('#txtSurname').removeAttr('disabled');
                        $('#txtFirstName').removeAttr('disabled');
                        $('#txtMiddleName').removeAttr('disabled');
                    } else {
                        $('#txtSurname').textinput('enable');
                        $('#txtFirstName').textinput('enable');
                        $('#txtMiddleName').textinput('enable');
                    }                    
                } else {
                    if (proj.IsBootstrapDesign()) {
                        $('#txtSurname').attr('disabled', 1).val('');
                        $('#txtFirstName').attr('disabled', 1).val('');
                        $('#txtMiddleName').attr('disabled', 1).val('');
                    } else {
                        $('#txtSurname').textinput('disable').val('');
                        $('#txtFirstName').textinput('disable').val('');
                        $('#txtMiddleName').textinput('disable').val('');
                    }                    
                }
            })

            $('#txtSurname').live('keyup', function () {
                if ($.trim($(this).val()) == '' && $.trim($('#txtFirstName').val()) == "" && $.trim($('#txtMiddleName').val()) == "") {
                    if (proj.IsBootstrapDesign()) {
                        $('#txtFullname').removeAttr('disabled');
                    } else {
                        $('#txtFullname').textinput('enable');
                    }                    
                } else {
                    if (proj.IsBootstrapDesign()) {
                        $('#txtFullname').attr('disabled', 1).val('');
                    } else {
                        $('#txtFullname').textinput('disable').val('');
                    }
                }
            })
            $('#txtFirstName').live('keyup', function () {
                if ($.trim($(this).val()) == '' && $.trim($('#txtSurname').val()) == "" && $.trim($('#txtMiddleName').val()) == "") {
                    if (proj.IsBootstrapDesign()) {
                        $('#txtFullname').removeAttr('disabled');
                    } else {
                        $('#txtFullname').textinput('enable');
                    }
                } else {
                    if (proj.IsBootstrapDesign()) {
                        $('#txtFullname').attr('disabled', 1).val('');
                    } else {
                        $('#txtFullname').textinput('disable').val('');
                    }
                }
            })
            $('#txtMiddleName').live('keyup', function () {
                if ($.trim($(this).val()) == '' && $.trim($('#txtSurname').val()) == "" && $.trim($('#txtFirstName').val()) == "") {
                    if (proj.IsBootstrapDesign()) {
                        $('#txtFullname').removeAttr('disabled');
                    } else {
                        $('#txtFullname').textinput('enable');
                    }
                } else {
                    if (proj.IsBootstrapDesign()) {
                        $('#txtFullname').attr('disabled', 1).val('');
                    } else {
                        $('#txtFullname').textinput('disable').val('');
                    }
                }
            })

            $('#txtSCPWDTIN').live('keyup', function () {
                // 1. Remove all non-digits
                var value = $(this).val().replace(/\D/g, '');

                // 2. Limit to 12 digits (the length of 000-000-000-000 without hyphens)
                value = value.substring(0, 15);

                // 3. Add hyphens at the correct positions
                var formattedValue = "";
                for (var i = 0; i < value.length; i++) {
                    if (i === 3 || i === 6 || i === 9) {
                        formattedValue += "-";
                    }
                    formattedValue += value[i];
                }

                $(this).val(formattedValue);
            });

            $('#txtChildBirthdate').live('change', function () {
                var birth = new Date($(this).val());
                var target = new Date();

                var age = target.getFullYear() - birth.getFullYear();
                var m = target.getMonth() - birth.getMonth();

                if (m < 0 || (m === 0 && target.getDate() < birth.getDate())) {
                    age--;
                }

                $('#txtChildAge').val(age);
            })


            window.pageDiscounts.getCurrentDiscount = function () {

                if (OfflineMode) {
                    var discount = SalesPayment_offline().getCurrentDiscount(sessionStorage.CartOrderID);
                    if (discount.Rows != undefined) {
                        if (discount.Rows.length > 0) {

                            $('#selDiscountTypeBilling').val(discount.Rows[0].DiscountTypeID).selectmenu('refresh');
                            $('#txtDiscountPercentBilling').val(discount.Rows[0].DiscountPercentage);
                            $('#txtDiscountRemarksBilling').val(discount.Rows[0].DiscountRemarks);
                            $('#txtDiscountPercentCeilingBilling').val(discount.Rows[0].PercentDiscount);
                            $('#divAddCustomerDetailsBilling').hide();
                            $('#divSCPWDDiscountBilling').hide();
                            showDiscountDetails = false;
                            isSeniorCitizenPWDDiscount = false;
                            isSeniorCitizenDiscount = false;
                            $("#txtDiscountPercentBilling").removeAttr("disabled");
                            $("#txtDiscountAmountBilling").removeAttr("disabled");
                            if ($('#selDiscountTypeBilling').val().toUpperCase() == '35F954A0-C67C-499C-AF60-F08E64477FDC') {
                                strAccessType = "Allow to grant VIP Discount";
                            } else if ($('#selDiscountTypeBilling').val().toUpperCase() == '764EDACB-0131-4033-8E44-7B903368008C') {
                                strAccessType = "Allow to grant Regular Discount 1";
                            } else if ($('#selDiscountTypeBilling').val().toUpperCase() == 'C4199347-6A00-45C9-BE11-8934DD63ECD1') {
                                strAccessType = "Allow to grant Regular Discount 2";
                            } else if ($('#selDiscountTypeBilling').val().toUpperCase() == '8E2DCA9C-433F-42FC-A644-04476DBA056A') {
                                strAccessType = "Allow to grant Regular Discount 3";
                            } else if ($('#selDiscountTypeBilling').val().toUpperCase() == '7804A35A-B531-4380-945E-FC9643E99843') {
                                strAccessType = "Allow to grant Employee Discount";
                            } else if ($('#selDiscountTypeBilling').val().toUpperCase() == '055A0A33-DD98-47EC-ADD3-8B2009F3944A') {
                                strAccessType = "Allow to grant Senior Citizen (SC) Discount";
                                $('#divAddCustomerDetailsBilling').show();
                                $('#divSCPWDDiscountBilling').show();
                                showDiscountDetails = true;
                                isSeniorCitizenPWDDiscount = true;
                                isSeniorCitizenDiscount = true;
                                $('#txtDiscountPercentBilling').attr("disabled", "disabled");
                                $('#txtDiscountAmountBilling').attr("disabled", "disabled");

                                //                                $('#txtSalesPayNoOfSCBilling').val(CDec2($('#txtSalesPayNoOfSCBilling').val()) > 0 ? $('#txtSalesPayNoOfSCBilling').val() : (discount.Rows[0].NoOfSeniorCitizens || 1));
                                //                                //                                $('#txtSalesPayNoOfDinersBilling').val(discount.Rows[0].NoOfPeople || 1);

                                //                                if (CDec2($.trim($("#txtSalesPayNoOfDinersBilling").val())) > 0)
                                //                                    $("#txtSalesPayNoOfDinersBilling").val($("#txtSalesPayNoOfDinersBilling").val());
                                //                                else
                                //                                    $('#txtSalesPayNoOfDinersBilling').val(discount.Rows[0].NoOfPeople || 1);
                                $('#txtSalesPayNoOfSCBilling').val(discount.Rows[0].NoOfSeniorCitizens || 1);
                                $('#txtSalesPayNoOfDinersBilling').val(discount.Rows[0].NoOfPeople || 1);

                                var vDataShow = SalesPayment_offline().getSeniorCitizenPWDCustomer(sessionStorage.CartOrderID);

                                intSalesPayCustomerNameCntr = window.pageDiscounts.LoadSavedCustomerNameList(vDataShow.Rows, false);

                                if (window.pageDiscounts.checkDiscountDetails(showDiscountDetails)) {

                                }
                            } else if ($('#selDiscountTypeBilling').val().toUpperCase() == '34A4032F-A142-4C15-A868-4AC4410434DF') {
                                strAccessType = "Allow to grant Person With Disability (PWD) Discount";
                                $('#divAddCustomerDetailsBilling').show();
                                isSeniorCitizenPWDDiscount = true;
                                $('#txtDiscountPercentBilling').attr("disabled", "disabled");
                                $('#txtDiscountAmountBilling').attr("disabled", "disabled");

                                var vDataShow = SalesPayment_offline().getSeniorCitizenPWDCustomer(sessionStorage.CartOrderID);

                                intSalesPayCustomerNameCntr = window.pageDiscounts.LoadSavedCustomerNameList(vDataShow.Rows, false);

                                if (window.pageDiscounts.checkDiscountDetails(showDiscountDetails)) {

                                    //                                    //                                    getOrgDiscountPercentRefreshBilling($('#selDiscountTypeBilling').val());
                                    //                                    //                                    RefreshTotalDiscountBilling();
                                    //                                    window.pageDiscounts.RefreshSalesPaymentTotalBilling();
                                }
                            } else if ($('#selDiscountTypeBilling').val().toUpperCase() == '674AD791-188B-4238-B95C-97B5C2B9D544') {
                                strAccessType = "Allow to grant Senior Citizen (SC) Discount";
                                $('#divAddCustomerDetailsBilling').show();
                                isSeniorCitizenPWDDiscount = true;
                                isSeniorCitizenDiscount = true;
                                $('#txtDiscountPercentBilling').attr("disabled", "disabled");
                                $('#txtDiscountAmountBilling').attr("disabled", "disabled");

                                var vDataShow = SalesPayment_offline().getSeniorCitizenPWDCustomer(sessionStorage.CartOrderID);

                                intSalesPayCustomerNameCntr = window.pageDiscounts.LoadSavedCustomerNameList(vDataShow.Rows, false);


                                if (window.pageDiscounts.checkDiscountDetails(showDiscountDetails)) {

                                }
                            } else if ($('#selDiscountTypeBilling').val().toUpperCase() == 'C2218C86-0450-4CD3-B019-3EDD43E51741') {
                                strAccessType = "Allow to grant GPC Discount";
                            } else {
                            }


                            if (isSeniorCitizenDiscount)
                                $("#txtDiscountAmountBilling").val(AccountingFormat(CDec2($('#lblTotalAmountBilling').text().replace(/,/g, "")) * (CDec2(discount.Rows[0].DiscountPercentage)) / 100));
                            else
                                $("#txtDiscountAmountBilling").val(AccountingFormat(CDec2($('#lblSubTotalBilling').text().replace(/,/g, "")) * (CDec2(discount.Rows[0].DiscountPercentage)) / 100));

                        } else {
                            $('#selDiscountTypeBilling').val('').selectmenu('refresh');
                            $('#txtDiscountPercentBilling').val('');
                            $('#txtDiscountAmountBilling').val('');
                            $('#txtDiscountRemarksBilling').val('');
                        }


                    }
                } else {
                    MessageBox('Not yet implemented');
                }
            }


            window.pageDiscounts.RefreshSalesPaymentTotalBilling = function () {


                var pData = [];
                var pDataDiscount = [];
                var pDataSeniorCitizenPWD = [];
                var intIndex;
                var selDiscountType;
                var txtDiscountPercent;
                var txtDiscountRemarks;
                var txtSalesPayNoOfDiners;
                var txtSalesPayNoOfSC;
                var txtTotalDiscountAmount;

                if (sessionStorage.CurrentPage == 'payment') {
                    txtTotalDiscountAmount = $("#lblSalesPayTotalDiscount").val();
                    selDiscountType = $("#selDiscountType").val();
                    txtDiscountPercent = $("#txtDiscountPercent").val();
                    txtDiscountRemarks = $("#txtDiscountRemarks").val();
                    txtSalesPayNoOfDiners = $("#txtSalesPayNoOfDiners").val();
                    txtSalesPayNoOfSC = $("#txtSalesPayNoOfSC").val();
                } else {
                    txtTotalDiscountAmount = $("#lblSalesPayTotalDiscountBillingBilling").val();
                    selDiscountType = $("#selDiscountTypeBilling").val();
                    txtDiscountPercent = $("#txtDiscountPercentBilling").val();
                    txtDiscountRemarks = $("#txtDiscountRemarksBilling").val();
                    txtSalesPayNoOfDiners = $("#txtSalesPayNoOfDinersBilling").val();
                    txtSalesPayNoOfSC = $("#txtSalesPayNoOfSCBilling").val();
                }


                //Discount
                if (selDiscountType != "") {
                    intIndex = 0;
                    pDataDiscount[intIndex] = {
                        ID: selDiscountType,
                        DiscountPercent: txtDiscountPercent,
                        DiscountRemarks: txtDiscountRemarks
                    };


                    //Senior Citizen PWD
                    //check first if SCD or PWD
                    if (isSeniorCitizenPWDDiscount) {
                        intIndex = 0;

                        for (var i = 0; i <= intSalesPayCustomerNameCntr; i++) {

                            if (sessionStorage["SalesPayCustomerName-" + i] != undefined) {

                                arSalesPayCustomerName = new Array();
                                arSalesPayCustomerName = JSON.parse(sessionStorage["SalesPayCustomerName-" + i]);

                                if (arSalesPayCustomerName[intSalesPayCustomerNameState] != "Remove") {
                                    pDataSeniorCitizenPWD[intIndex] = {
                                        ID: arSalesPayCustomerName[intSalesPayCustomerNameID]
                                    };

                                    intIndex = intIndex + 1;
                                } //end if not remove
                            } //end if session storage
                        } //end for
                    }

                }


                var pData;

                if (CStr2(selDiscountType) == '') {
                    pData = {
                        InvoiceID: sessionStorage.CartOrderID
                    };
                    SalesPayment_offline().RemoveBillingDiscount(pData);
                    if (OfflineMode) {
                        return SalesPayment_offline().getSalesPaymentReceiptFooterAmountsInfo(sessionStorage.CartOrderID);
                    } else {
                        //Get Footer Amounts
                        AjaxSend({
                            url: "SalesPayments/getSalesPaymentReceiptFooterAmountsInfo",
                            data: { InvoiceID: sessionStorage.CartOrderID },
                            success: function (vRetDataFooter) {
                                return vRetDataFooter.SalesPaymentData;

                            }
                        });
                    }
                } else {
                    pData = {
                        InvoiceID: sessionStorage.CartOrderID,
                        TotalDiscountAmount: txtTotalDiscountAmount,
                        NoOfPeople: selDiscountType.toUpperCase() == '055A0A33-DD98-47EC-ADD3-8B2009F3944A' ? $.trim(txtSalesPayNoOfDiners) : "", //for senior citizen discount
                        NoOfSeniorCitizens: selDiscountType.toUpperCase() == '055A0A33-DD98-47EC-ADD3-8B2009F3944A' ? $.trim(txtSalesPayNoOfSC) : "", //for senior citizen discount
                        SalesPaymentDiscountList: pDataDiscount,
                        SalesPaymentSeniorCitizenPWDList: pDataSeniorCitizenPWD
                    };

                    if (OfflineMode) {
                        SalesPayment_offline().SaveBillingDiscount(pData);
                        return SalesPayment_offline().getSalesPaymentReceiptFooterAmountsInfo(sessionStorage.CartOrderID);
                    } else {

                        AjaxSend({
                            url: 'SalesPayments/SaveBillingDiscount',
                            data: pData,
                            success: function (vData) {
                                if (vData.SalesPaymentData == 'ok') {
                                    //Get Footer Amounts
                                    AjaxSend({
                                        url: "SalesPayments/getSalesPaymentReceiptFooterAmountsInfo",
                                        data: { InvoiceID: sessionStorage.CartOrderID },
                                        success: function (vRetDataFooter) {
                                            return vRetDataFooter.SalesPaymentData;

                                        }
                                    });
                                } else {
                                    MessageBox(vData.SalesPaymentData);
                                }
                            },
                            error: function (vError) {
                                MessageBox("This feature requires internet connection.",
                                                "Requires Internet Connection",
                                                [
                                                    { Text: 'Ok'
                                                    }
                                                ]
                                        );
                                return;
                            }

                        }); //ajaxsend
                    }

                }



            };



            function ClearSalesPayCustomerNameFields() {
                $("#txtID").val('');
                $("#txtSCPWDCustomerID").val('');

                $("#txtTitleID").val('');
                
                if (proj.IsBootstrapDesign()) {
                    $("#btnTitle").text('Please Select');
                    $("#txtFullname").removeAttr('disabled').val('');
                    $("#txtSurname").removeAttr('disabled').val('');
                    $("#txtFirstName").removeAttr('disabled').val('');
                    $("#txtMiddleName").removeAttr('disabled').val('');
                    $("#selPersonalSuffixName").val('');
                    $("#txtSalesPayCustomerBirthDate").val('');
                } else {
                    $("#btnTitle").text('Please Select').button('refresh');
                    $("#txtFullname").textinput('enable').val('');
                    $("#txtSurname").textinput('enable').val('');
                    $("#txtFirstName").textinput('enable').val('');
                    $("#txtMiddleName").textinput('enable').val('');
                    $("#selPersonalSuffixName").val('').selectmenu('refresh');
                    $("#selDOBMonth").val('').selectmenu('refresh');
                    $("#selDOBDay").val('').selectmenu('refresh');
                    $("#selDOBYear").val('').selectmenu('refresh');
                }                

                $("#btnSalesPayCustomerIdentificationRemove").hide();

                var pLV = $('#lstSalesPayCustomerIdentification');
                pLV.html('');

                if (!proj.IsBootstrapDesign()) {
                    pLV.listview('refresh');
                }

                ClearSalesPayCustomerIdentificationFields();

            }
            function ClearSalesPaySeniorCitizenDiscountFields() {
                $('#txtCustomerSearch').val('');
                $("#txtSalesPayNoOfSC").val('');
                $("#txtSalesPayNoOfDiners").val('');

                var pLV = $('#lstSalesPayCustomerName');
                pLV.html('');

                if (!proj.IsBootstrapDesign())
                    pLV.listview('refresh');


                ClearSalesPayCustomerNameFields();

            }
            function ClearSalesPayCustomerIdentificationFields() {
                if (proj.IsBootstrapDesign()){
                    $("#selIDType").val('');
                } else {
                    $("#selIDType").selectmenu();
                    $("#selIDType").val('').selectmenu('refresh');
                }
                
                $("#txtOtherIDType").val('');
                $("#txtIDText").val('');

                $("#btnSalesPayCustomerIdentificationRemove").hide();

            }


            window.pageDiscounts.LoadSavedCustomerNameList = function (vRows, isPayment) {
                //                //reset name list. clear

                //                for (var i = 0; i <= intSalesPayCustomerNameCntr; i++) {

                //                    if (sessionStorage["SalesPayCustomerName-" + i] != undefined) {
                //                        sessionStorage.removeItem("SalesPayCustomerName-" + i);

                //                    }
                //                }

                //                ////////////////////////////////////////////////////////////////////////////

                $('#txtCustomerSearch').val('');
                var pLV;

                if (isPayment)
                    pLV = $('#lstSalesPayCustomerName');
                else
                    pLV = $('#lstSalesPayCustomerNameBilling');

                var pItem;

                pLV.html('');

                for (var i = 0; i < vRows.length; i++) {
                    if (proj.IsBootstrapDesign()) {
                        pItem = $('<a data-role="none" class="list-group-item">')
                                           .text((CStr2(vRows[i].TitleName) == '' ? '' : vRows[i].TitleName) + ' ' +
                                            (CStr2(vRows[i].PersonFullName || vRows[i].Fullname) == "" ? vRows[i].Surname + ', ' +
                                            vRows[i].FirstName + ' ' +
                                            vRows[i].MiddleName : CStr2(vRows[i].PersonFullName || vRows[i].Fullname)))
                                           .data('ID', vRows[i].ID)
                                           .click(function (e) {
                                               window.pageDiscounts.ShowSavedCustomerNameInfo($(this).data('ID'));
                                           })
                    } else {
                        pItem = $('<li>')
                            .append($('<a>')
                                    .data('ID', vRows[i].ID)
                                    .click(function (e) {
                                        e.preventDefault();

                                        window.pageDiscounts.ShowSavedCustomerNameInfo($(this).data('ID'));
                                    })
                                    .append($('<h3>')
                                        .attr("style", "white-space:normal")
                                        .text((CStr2(vRows[i].TitleName) == '' ? '' : vRows[i].TitleName) + ' ' +
                                        (CStr2(vRows[i].PersonFullName || vRows[i].Fullname) == "" ? vRows[i].Surname + ', ' +
                                        vRows[i].FirstName + ' ' +
                                        vRows[i].MiddleName : CStr2(vRows[i].PersonFullName || vRows[i].Fullname)))
                                    )
                            );
                    }

                    pLV.append(pItem);
                }

                if (!proj.IsBootstrapDesign())
                    pLV.listview('refresh');
                //                $('#popupDiscounts').popup('open');
                //                    RefreshSalesPayDebitCardTotal();
                intSalesPayCustomerNameCntr = vRows.length;

                return vRows.length;
            };



            function displayCustomerNameInfo(vRows, id) {

                //                setMonthCollection("selDOBMonth");
                //                setYearCollection("selDOBYear");

                //                ClearSalesPayCustomerNameFields();

                $("#txtID").val(vRows[0].ID || '');
                $("#txtSCPWDCustomerID").val(id || '');

                $("#txtTitleID").val(vRows[0].PersonTitleID || '');

                $("#btnTitle").text(vRows[0].PersonTitle || 'Please Select').button('refresh');
                $("#txtFullname").val(vRows[0].PersonFullname || '');
                $("#txtSurname").val(vRows[0].PersonSurname || '');
                $("#txtFirstName").val(vRows[0].PersonFirstName || '');
                $("#txtMiddleName").val(vRows[0].PersonMiddleName || '');
                $("#selPersonalSuffixName").val(vRows[0].PersonSuffixName || '');

                if (!proj.IsBootstrapDesign())
                    $("#selPersonalSuffixName").selectmenu('refresh');

                if (CStr2(vRows[0].PersonDateOfBirth) != '') {
                    var prefix = vRows[0].PersonDateOfBirth.split('/');
                                        
                    if (!proj.IsBootstrapDesign()) {
                        $("#selDOBMonth").val(prefix[0]);
                        $("#selDOBMonth").selectmenu('refresh');

                        $("#selDOBYear").val(prefix[2]);
                        $("#selDOBYear").selectmenu('refresh');
                        setDayCollection('selDOBMonth', 'selDOBDay', 'selDOBYear', true);

                        $("#selDOBDay").val(prefix[1]);
                        $("#selDOBDay").selectmenu('refresh');
                    } else {
                        $("#txtSalesPayCustomerBirthDate").val($.format.date(new Date(vRows[0].PersonDateOfBirth), 'yyyy-MM-dd'));
                    }
                }


                IsNewSalesPayCustomerName = false;

                $("#btnSalesPayCustomerNameRemove").show();

                LoadSalesPayCustomerIdentificationList(vRows);



                //                ShowSavedCustomerIDList(id);
            }


            function LoadSalesPayCustomerIdentificationList(vRows) {

                var pLV = $('#lstSalesPayCustomerIdentification');
                var pItem;

                pLV.html('');

                if (vRows != '') {
                    //reset id list. clear

                    ////////////////////////////////////////////////////////////////////////////
                    IsNewSalesPayCustomerIdentification = false;
                    intSalesPayCustomerIdentificationCntr = 0;
                    intSalesPayCustomerIdentificationIndex = 0;



                    for (var i = 0; i < vRows.length; i++) {
                        if (CStr2(vRows[0].PersonSeniorCitizensIDNo) != '') {
                            arSalesPayCustomerIdentification = new Array();

                            arSalesPayCustomerIdentification[intSalesPayCustomerIdentificationIDType] = "Senior Citizens' ID";
                            arSalesPayCustomerIdentification[intSalesPayCustomerIdentificationIDOtherType] = '';
                            arSalesPayCustomerIdentification[intSalesPayCustomerIdentificationIDText] = vRows[0].PersonSeniorCitizensIDNo;
                            arSalesPayCustomerIdentification[intSalesPayCustomerIdentificationState] = (IsNewSalesPayCustomerIdentification ? "New" : "Edit");

                            sessionStorage["SalesPayCustomerIdentification-" + intSalesPayCustomerIdentificationIndex] = JSON.stringify(arSalesPayCustomerIdentification);
                            intSalesPayCustomerIdentificationCntr = intSalesPayCustomerIdentificationCntr + 1;
                            intSalesPayCustomerIdentificationIndex = intSalesPayCustomerIdentificationCntr;
                        }
                        if (CStr2(vRows[0].PersonPWDIDNo) != '') {
                            arSalesPayCustomerIdentification = new Array();

                            arSalesPayCustomerIdentification[intSalesPayCustomerIdentificationIDType] = "Persons With Disabilities (PWD) ID";
                            arSalesPayCustomerIdentification[intSalesPayCustomerIdentificationIDOtherType] = '';
                            arSalesPayCustomerIdentification[intSalesPayCustomerIdentificationIDText] = vRows[0].PersonPWDIDNo;
                            arSalesPayCustomerIdentification[intSalesPayCustomerIdentificationState] = (IsNewSalesPayCustomerIdentification ? "New" : "Edit");

                            sessionStorage["SalesPayCustomerIdentification-" + intSalesPayCustomerIdentificationIndex] = JSON.stringify(arSalesPayCustomerIdentification);
                            intSalesPayCustomerIdentificationCntr = intSalesPayCustomerIdentificationCntr + 1;
                            intSalesPayCustomerIdentificationIndex = intSalesPayCustomerIdentificationCntr;
                        }
                        if (CStr2(vRows[0].PersonPassportNo) != '') {
                            arSalesPayCustomerIdentification = new Array();

                            arSalesPayCustomerIdentification[intSalesPayCustomerIdentificationIDType] = "Passport";
                            arSalesPayCustomerIdentification[intSalesPayCustomerIdentificationIDOtherType] = '';
                            arSalesPayCustomerIdentification[intSalesPayCustomerIdentificationIDText] = vRows[0].PersonPassportNo;
                            arSalesPayCustomerIdentification[intSalesPayCustomerIdentificationState] = (IsNewSalesPayCustomerIdentification ? "New" : "Edit");

                            sessionStorage["SalesPayCustomerIdentification-" + intSalesPayCustomerIdentificationIndex] = JSON.stringify(arSalesPayCustomerIdentification);
                            intSalesPayCustomerIdentificationCntr = intSalesPayCustomerIdentificationCntr + 1;
                            intSalesPayCustomerIdentificationIndex = intSalesPayCustomerIdentificationCntr;
                        }
                        if (CStr2(vRows[0].PersonDriversLicenseNo) != '') {
                            arSalesPayCustomerIdentification = new Array();

                            arSalesPayCustomerIdentification[intSalesPayCustomerIdentificationIDType] = "Driver's License";
                            arSalesPayCustomerIdentification[intSalesPayCustomerIdentificationIDOtherType] = '';
                            arSalesPayCustomerIdentification[intSalesPayCustomerIdentificationIDText] = vRows[0].PersonDriversLicenseNo;
                            arSalesPayCustomerIdentification[intSalesPayCustomerIdentificationState] = (IsNewSalesPayCustomerIdentification ? "New" : "Edit");

                            sessionStorage["SalesPayCustomerIdentification-" + intSalesPayCustomerIdentificationIndex] = JSON.stringify(arSalesPayCustomerIdentification);
                            intSalesPayCustomerIdentificationCntr = intSalesPayCustomerIdentificationCntr + 1;
                            intSalesPayCustomerIdentificationIndex = intSalesPayCustomerIdentificationCntr;
                        }
                        if (CStr2(vRows[0].PersonSSSNo) != '') {
                            arSalesPayCustomerIdentification = new Array();

                            arSalesPayCustomerIdentification[intSalesPayCustomerIdentificationIDType] = "SSS ID";
                            arSalesPayCustomerIdentification[intSalesPayCustomerIdentificationIDOtherType] = '';
                            arSalesPayCustomerIdentification[intSalesPayCustomerIdentificationIDText] = vRows[0].PersonSSSNo;
                            arSalesPayCustomerIdentification[intSalesPayCustomerIdentificationState] = (IsNewSalesPayCustomerIdentification ? "New" : "Edit");

                            sessionStorage["SalesPayCustomerIdentification-" + intSalesPayCustomerIdentificationIndex] = JSON.stringify(arSalesPayCustomerIdentification);
                            intSalesPayCustomerIdentificationCntr = intSalesPayCustomerIdentificationCntr + 1;
                            intSalesPayCustomerIdentificationIndex = intSalesPayCustomerIdentificationCntr;
                        }
                        if (CStr2(vRows[0].PersonGSISIDNo) != '') {
                            arSalesPayCustomerIdentification = new Array();

                            arSalesPayCustomerIdentification[intSalesPayCustomerIdentificationIDType] = "GSIS ID";
                            arSalesPayCustomerIdentification[intSalesPayCustomerIdentificationIDOtherType] = '';
                            arSalesPayCustomerIdentification[intSalesPayCustomerIdentificationIDText] = vRows[0].PersonGSISIDNo;
                            arSalesPayCustomerIdentification[intSalesPayCustomerIdentificationState] = (IsNewSalesPayCustomerIdentification ? "New" : "Edit");

                            sessionStorage["SalesPayCustomerIdentification-" + intSalesPayCustomerIdentificationIndex] = JSON.stringify(arSalesPayCustomerIdentification);
                            intSalesPayCustomerIdentificationCntr = intSalesPayCustomerIdentificationCntr + 1;
                            intSalesPayCustomerIdentificationIndex = intSalesPayCustomerIdentificationCntr;
                        }
                        if (CStr2(vRows[0].PersonPRCIDNo) != '') {
                            arSalesPayCustomerIdentification = new Array();

                            arSalesPayCustomerIdentification[intSalesPayCustomerIdentificationIDType] = "PRC ID";
                            arSalesPayCustomerIdentification[intSalesPayCustomerIdentificationIDOtherType] = '';
                            arSalesPayCustomerIdentification[intSalesPayCustomerIdentificationIDText] = vRows[0].PersonPRCIDNo;
                            arSalesPayCustomerIdentification[intSalesPayCustomerIdentificationState] = (IsNewSalesPayCustomerIdentification ? "New" : "Edit");

                            sessionStorage["SalesPayCustomerIdentification-" + intSalesPayCustomerIdentificationIndex] = JSON.stringify(arSalesPayCustomerIdentification);
                            intSalesPayCustomerIdentificationCntr = intSalesPayCustomerIdentificationCntr + 1;
                            intSalesPayCustomerIdentificationIndex = intSalesPayCustomerIdentificationCntr;
                        }
                        if (CStr2(vRows[0].PersonPostalIDNo) != '') {
                            arSalesPayCustomerIdentification = new Array();

                            arSalesPayCustomerIdentification[intSalesPayCustomerIdentificationIDType] = "Postal ID";
                            arSalesPayCustomerIdentification[intSalesPayCustomerIdentificationIDOtherType] = '';
                            arSalesPayCustomerIdentification[intSalesPayCustomerIdentificationIDText] = vRows[0].PersonPostalIDNo;
                            arSalesPayCustomerIdentification[intSalesPayCustomerIdentificationState] = (IsNewSalesPayCustomerIdentification ? "New" : "Edit");

                            sessionStorage["SalesPayCustomerIdentification-" + intSalesPayCustomerIdentificationIndex] = JSON.stringify(arSalesPayCustomerIdentification);
                            intSalesPayCustomerIdentificationCntr = intSalesPayCustomerIdentificationCntr + 1;
                            intSalesPayCustomerIdentificationIndex = intSalesPayCustomerIdentificationCntr;
                        }
                        if (CStr2(vRows[0].PersonVotersIDNo) != '') {
                            arSalesPayCustomerIdentification = new Array();

                            arSalesPayCustomerIdentification[intSalesPayCustomerIdentificationIDType] = "Voter's ID";
                            arSalesPayCustomerIdentification[intSalesPayCustomerIdentificationIDOtherType] = '';
                            arSalesPayCustomerIdentification[intSalesPayCustomerIdentificationIDText] = vRows[0].PersonVotersIDNo;
                            arSalesPayCustomerIdentification[intSalesPayCustomerIdentificationState] = (IsNewSalesPayCustomerIdentification ? "New" : "Edit");

                            sessionStorage["SalesPayCustomerIdentification-" + intSalesPayCustomerIdentificationIndex] = JSON.stringify(arSalesPayCustomerIdentification);
                            intSalesPayCustomerIdentificationCntr = intSalesPayCustomerIdentificationCntr + 1;
                            intSalesPayCustomerIdentificationIndex = intSalesPayCustomerIdentificationCntr;
                        }

                        if (CStr2(vRows[0].PersonOtherIDType) != '') {
                            arSalesPayCustomerIdentification = new Array();

                            arSalesPayCustomerIdentification[intSalesPayCustomerIdentificationIDType] = "Other";
                            arSalesPayCustomerIdentification[intSalesPayCustomerIdentificationIDOtherType] = vRows[0].PersonOtherIDType;
                            arSalesPayCustomerIdentification[intSalesPayCustomerIdentificationIDText] = vRows[0].PersonOtherIDNo;
                            arSalesPayCustomerIdentification[intSalesPayCustomerIdentificationState] = (IsNewSalesPayCustomerIdentification ? "New" : "Edit");

                            sessionStorage["SalesPayCustomerIdentification-" + intSalesPayCustomerIdentificationIndex] = JSON.stringify(arSalesPayCustomerIdentification);
                            intSalesPayCustomerIdentificationCntr = intSalesPayCustomerIdentificationCntr + 1;
                            intSalesPayCustomerIdentificationIndex = intSalesPayCustomerIdentificationCntr;
                        }



                        ///////////////////

                        if (sessionStorage["SalesPayCustomerIdentification-" + i] != undefined) {

                            arSalesPayCustomerIdentification = new Array();
                            arSalesPayCustomerIdentification = JSON.parse(sessionStorage["SalesPayCustomerIdentification-" + i]);

                            if (arSalesPayCustomerIdentification[intSalesPayCustomerIdentificationState] != "Remove") {
                                if (proj.IsBootstrapDesign()) {
                                    pItem = $('<a data-role="none" class="list-group-item">')
                                            .append(arSalesPayCustomerIdentification[intSalesPayCustomerIdentificationIDOtherType] == '' ? arSalesPayCustomerIdentification[intSalesPayCustomerIdentificationIDType] : arSalesPayCustomerIdentification[intSalesPayCustomerIdentificationIDOtherType])
                                            .append('<br>')
                                            .append("No.: " + arSalesPayCustomerIdentification[intSalesPayCustomerIdentificationIDText])
                                            .data('Data', arSalesPayCustomerIdentification)
                                            .attr('ID', i)
                                            .click(function (e) {
                                                ShowSalesPayCustomerIdentificationInfo($(this).data('Data'), $(this).attr('ID'));
                                            })
                                } else {
                                    pItem = $('<li id="lst' + i + '">')
                                        .append($('<a>')
                                                .attr('ID', i)
                                                .data('Data', arSalesPayCustomerIdentification)
                                                .click(function (e) {
                                                    e.preventDefault();
                                                    ShowSalesPayCustomerIdentificationInfo($(this).data('Data'), $(this).attr('ID'));
                                                })
                                                .append($('<h3>')
                                                    .attr("style", "white-space:normal")
                                                    .text(arSalesPayCustomerIdentification[intSalesPayCustomerIdentificationIDOtherType] == '' ? arSalesPayCustomerIdentification[intSalesPayCustomerIdentificationIDType] : arSalesPayCustomerIdentification[intSalesPayCustomerIdentificationIDOtherType])
                                                )
                                                .append($('<p>')
                                                    .attr("style", "white-space:normal")
                                                    .text("No.: " + arSalesPayCustomerIdentification[intSalesPayCustomerIdentificationIDText])
                                                )
                                        );                                    
                                }

                                pLV.append(pItem);
                            }
                        }
                    }
                } else {

                    for (var i = 0; i <= intSalesPayCustomerIdentificationCntr; i++) {

                        if (sessionStorage["SalesPayCustomerIdentification-" + i] != undefined) {

                            arSalesPayCustomerIdentification = new Array();
                            arSalesPayCustomerIdentification = JSON.parse(sessionStorage["SalesPayCustomerIdentification-" + i]);

                            if (arSalesPayCustomerIdentification[intSalesPayCustomerIdentificationState] != "Remove") {
                                if (proj.IsBootstrapDesign()) {
                                    pItem = $('<a data-role="none" class="list-group-item">')
                                            .append(arSalesPayCustomerIdentification[intSalesPayCustomerIdentificationIDOtherType] == '' ? arSalesPayCustomerIdentification[intSalesPayCustomerIdentificationIDType] : arSalesPayCustomerIdentification[intSalesPayCustomerIdentificationIDOtherType])
                                            .append('<br>')
                                            .append("No.: " + arSalesPayCustomerIdentification[intSalesPayCustomerIdentificationIDText])
                                            .data('Data', arSalesPayCustomerIdentification)
                                            .attr('ID', i)
                                            .click(function (e) {
                                                ShowSalesPayCustomerIdentificationInfo($(this).data('Data'), $(this).attr('ID'));
                                            })
                                } else {
                                    pItem = $('<li id="lst' + i + '">')
                                        .append($('<a>')
                                                .attr('ID', i)
                                                .data('Data', arSalesPayCustomerIdentification)
                                                .click(function (e) {
                                                    e.preventDefault();
                                                    ShowSalesPayCustomerIdentificationInfo($(this).data('Data'), $(this).attr('ID'));
                                                })
                                                .append($('<h3>')
                                                    .attr("style", "white-space:normal")
                                                    .text(arSalesPayCustomerIdentification[intSalesPayCustomerIdentificationIDOtherType] == '' ? arSalesPayCustomerIdentification[intSalesPayCustomerIdentificationIDType] : arSalesPayCustomerIdentification[intSalesPayCustomerIdentificationIDOtherType])
                                                )
                                                .append($('<p>')
                                                    .attr("style", "white-space:normal")
                                                    .text("No.: " + arSalesPayCustomerIdentification[intSalesPayCustomerIdentificationIDText])
                                                )
                                        );                                    
                                }

                                pLV.append(pItem);
                            }
                        } //if undefined
                    } //for
                } // if else

                if (!proj.IsBootstrapDesign())
                    pLV.listview('refresh');

            };

            function ShowSalesPayCustomerIdentificationInfo(arData, index) {
                if (proj.IsBootstrapDesign()) {
                    $('#popupAddNewID').modal('toggle');
                } else {
                    $('#popupAddNewID').popup('open');
                }


                ClearSalesPayCustomerIdentificationFields();

                intSalesPayCustomerIdentificationIndex = index;

                if (proj.IsBootstrapDesign()) {
                    $("#selIDType").val(arData[intSalesPayCustomerIdentificationIDType] || '');
                } else {
                    $("#selIDType").selectmenu();
                    $("#selIDType").val(arData[intSalesPayCustomerIdentificationIDType] || '').selectmenu('refresh');
                }               

                if (arData[intSalesPayCustomerIdentificationIDOtherType] == '') {
                    $("#divOtherIDType").hide();
                } else {
                    $("#divOtherIDType").show();
                    $("#txtOtherIDType").val(arData[intSalesPayCustomerIdentificationIDOtherType] || '');

                }

                $("#txtIDText").val(arData[intSalesPayCustomerIdentificationIDText] || '');

                IsNewSalesPayCustomerIdentification = false;

                $("#btnSalesPayCustomerIdentificationRemove").show();


            };


            function seniorCitizenDetailsPage(vRows, id) {
                // IsNewSalesPayCustomerName = true;

                if (window.location.pathname.toLowerCase() == '/salesmenu/posviewall') {

                    $.mobile.changePage('#SeniorCitizenPWD');

                    if (CStr2(sessionStorage.IsSeniorCitizenPWDPageLoaded) != 'true') {
                        AjaxSend({
                            ExactUrl: '/SalesPayments/SeniorCitizenPWD?cache=1',
                            type: 'GET',
                            dataType: 'text',
                            success: function (vData) {

                                $('#SeniorCitizenPWD').append(vData).trigger('create');
                                sessionStorage.IsSeniorCitizenPWDPageLoaded = 'true';

                                App.Deferred.fulfill('FQ26E5TP78');

                                ClearSalesPayCustomerNameFields();
                                setMonthCollection("selDOBMonth");
                                setYearCollection("selDOBYear");

                                if (vRows != '')
                                    displayCustomerNameInfo(vRows, id);
                                else
                                    addNewSeniorCitizenPWDCustomer();

                            }
                        });
                    } else {

                        if (vRows != '') {
                            ClearSalesPayCustomerNameFields();
                            setMonthCollection("selDOBMonth");
                            setYearCollection("selDOBYear");
                            displayCustomerNameInfo(vRows, id);
                        }
                        else {

                            addNewSeniorCitizenPWDCustomer();
                        }

                    }

                    // window.pageHeader.ShowSeniorCitizenPWDPage();
                }
                else {
                    if (proj.IsBootstrapDesign()) {
                        $('#pageAddNewCustomer').pageshow();
                    } else {
                        $.mobile.changePage('#pageAddNewCustomer');
                    }                    
                    addNewSeniorCitizenPWDCustomer();
                    displayCustomerNameInfo(vRows, id);
                }



            }

            function addNewSeniorCitizenPWDCustomer() {

                ClearSalesPayCustomerNameFields();
                setMonthCollection("selDOBMonth");
                setYearCollection("selDOBYear");


                IsNewSalesPayCustomerIdentification = true;
                ClearSalesPayCustomerIdentificationFields();

                //                intSalesPayCustomerIdentificationCntr = intSalesPayCustomerIdentificationCntr + 1;
                //                intSalesPayCustomerIdentificationIndex = intSalesPayCustomerIdentificationCntr;
                $('#btnSalesPayCustomerIdentificationRemove').hide();

                $("#btnSalesPayCustomerNameRemove").hide();


                for (var i = 0; i <= intSalesPayCustomerIdentificationCntr; i++) {

                    if (sessionStorage["SalesPayCustomerIdentification-" + i] != undefined) {
                        sessionStorage.removeItem("SalesPayCustomerIdentification-" + i);

                    }

                }


                intSalesPayCustomerNameCntr = intSalesPayCustomerNameCntr + 1;
                intSalesPayCustomerNameIndex = intSalesPayCustomerNameCntr;


            }


            function RemoveSavedSalesPayCustomerName() {

                if (OfflineMode) {
                    var vData = SalesPayment_offline().removeSeniorCitizenPWDCustomer($("#txtSCPWDCustomerID").val());                    
                    RemoveSalesPayCustomerName($("#txtCustomerIndex").val());
                    if (vData == 'ok') {
                        if (location.hash != '#payment') {
                            $.mobile.changePage('#PreviewBill');
                            $('#popupDiscounts').popup('open');
                        }
                        $.mobile.changePage('');
                        $('#pageSalesPayment').pageshow();// for new UI
                        ClearSalesPayCustomerNameFields();
                        //displaySeniorCitizenInfoList(true); commented june 28, francis h

                        //                        var vDataShow = SalesPayment_offline().getSeniorCitizenPWDCustomer(sessionStorage.CartOrderID);

                        //                        window.pageDiscounts.LoadSavedCustomerNameList(vDataShow.Rows, false);
                        //window.pageDiscounts.checkDiscountDetails(true);
                    } else {

                        MessageBox(vData);
                    }

                } else {

                    MessageBox('Not yet implemented');

                }




            };


            function SaveSalesPayCustomerNameBilling() {
                var seniorCitizensIDNo;
                var pwdIDNo;
                var passportNo;
                var driversLicenseNo;
                var sssNo;
                var gsisIDNo;
                var prcIDNo;
                var postalIDNo;
                var votersIDNo;
                var otherIDNo;
                var otherIDType;

                for (var i = 0; i <= intSalesPayCustomerIdentificationCntr; i++) {

                    if (sessionStorage["SalesPayCustomerIdentification-" + i] != undefined) {

                        arIdentificationNumber = new Array();
                        arIdentificationNumber = JSON.parse(sessionStorage["SalesPayCustomerIdentification-" + i]);

                        if (arIdentificationNumber[intSalesPayCustomerIdentificationState] != "Remove") {

                            switch (arIdentificationNumber[intSalesPayCustomerIdentificationIDType]) {
                                case "Senior Citizens' ID":
                                    seniorCitizensIDNo = arIdentificationNumber[intSalesPayCustomerIdentificationIDText];
                                    break;
                                case "Persons With Disabilities (PWD) ID":
                                    pwdIDNo = arIdentificationNumber[intSalesPayCustomerIdentificationIDText];
                                    break;
                                case "Passport":
                                    passportNo = arIdentificationNumber[intSalesPayCustomerIdentificationIDText];
                                    break;
                                case "Driver's License":
                                    driversLicenseNo = arIdentificationNumber[intSalesPayCustomerIdentificationIDText];
                                    break;
                                case "SSS ID":
                                    sssNo = arIdentificationNumber[intSalesPayCustomerIdentificationIDText];
                                    break;
                                case "GSIS ID":
                                    gsisIDNo = arIdentificationNumber[intSalesPayCustomerIdentificationIDText];
                                    break;
                                case "PRC ID":
                                    prcIDNo = arIdentificationNumber[intSalesPayCustomerIdentificationIDText];
                                    break;
                                case "Postal ID":
                                    postalIDNo = arIdentificationNumber[intSalesPayCustomerIdentificationIDText];
                                    break;
                                case "Voter's ID":
                                    votersIDNo = arIdentificationNumber[intSalesPayCustomerIdentificationIDText];
                                    break;
                                case "Other":
                                    otherIDType = arIdentificationNumber[intSalesPayCustomerIdentificationIDOtherType];
                                    otherIDNo = arIdentificationNumber[intSalesPayCustomerIdentificationIDText];
                                    break;

                                    intSalesPayCustomerIdentificationCntr = intSalesPayCustomerIdentificationCntr + 1;
                            }
                        }

                    } // end if


                } //end for

                var pDataTradeName = [];
                var pDataCategory = [];

                var pDataPerson = {
                    IsOrganization: 0,
                    OrganizationID: $("#txtID").val(),
                    PersonFullName: $.trim($("#txtFullname").val()),
                    PersonFirstName: $.trim($("#txtFirstName").val()),
                    PersonMiddleName: $.trim($("#txtMiddleName").val()),
                    PersonSurname: $.trim($("#txtSurname").val()),
                    PersonSuffixName: $.trim($("#selPersonalSuffixName").val()),
                    PersonTitleID: $.trim($("#txtTitleID").val()),
                    PersonDOB: $("#selDOBMonth").val() + '/' + $("#selDOBDay").val() + '/' + $("#selDOBYear").val(),
                    PersonSeniorCitizensIDNo: seniorCitizensIDNo,
                    PersonPWDIDNo: pwdIDNo,
                    PersonPassportNo: passportNo,
                    PersonDriversLicenseNo: driversLicenseNo,
                    PersonSSSNo: sssNo,
                    PersonGSISIDNo: gsisIDNo,
                    PersonPRCIDNo: prcIDNo,
                    PersonPostalIDNo: postalIDNo,
                    PersonVotersIDNo: votersIDNo,
                    PersonOtherIDType: otherIDType,
                    PersonOtherIDNo: otherIDNo,
                    PersonMotherMaidenSurname: '',
                    PersonMotherMaidenFirstName: '',
                    PersonMotherMaidenMiddleName: '',
                    OrgTradeNameList: pDataTradeName,
                    OrgCategoryList: pDataCategory,
                    IsAddAsCustomer: (IsNewSalesPayCustomerName ? 1 : 0),
                    CompanyNameInitial: '',
                    IsFranchisor: 0,
                    TIN: $.trim($("#txtSCPWDTIN").val()),
                    ChildName: $.trim($("#txtChildName").val()),
                    ChildBirthdate: $.trim($("#txtChildBirthdate").val()),
                    ChildAge: $.trim($("#txtChildAge").val()),
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

                    var result = SalesPayment_offline().SaveOrganizationInfo(pDataPerson);
                    if (result != 'Existing') {
                        ClearSalesPayCustomerNameFields();
                        $("#txtID").val(result);

                        arSalesPayCustomerName = new Array();
                        arSalesPayCustomerName[intSalesPayCustomerNameID] = $("#txtID").val();
                        arSalesPayCustomerName[intSalesPayCustomerNameTitleID] = $.trim($("#txtTitleID").val());
                        arSalesPayCustomerName[intSalesPayCustomerNameTitleName] = $.trim($("#btnTitle").text());
                        arSalesPayCustomerName[intSalesPayCustomerNameFullName] = $.trim($("#txtFullname").val());
                        arSalesPayCustomerName[intSalesPayCustomerNameSurname] = $.trim($("#txtSurname").val());
                        arSalesPayCustomerName[intSalesPayCustomerNameFirstName] = $("#txtFirstName").val();
                        arSalesPayCustomerName[intSalesPayCustomerNameMiddleName] = $.trim($("#txtMiddleName").val());
                        arSalesPayCustomerName[intSalesPayCustomerNameSuffix] = $.trim($("#selPersonalSuffixName").val());
                        arSalesPayCustomerName[intSalesPayCustomerNameDOBMonth] = $("#selDOBMonth").val();
                        arSalesPayCustomerName[intSalesPayCustomerNameDOBDay] = $.trim($("#selDOBDay").val());
                        arSalesPayCustomerName[intSalesPayCustomerNameDOBYear] = $.trim($("#selDOBYear").val());
                        arSalesPayCustomerName[intSalesPayCustomerNameState] = (IsNewSalesPayCustomerName ? "New" : "Edit");

                        sessionStorage["SalesPayCustomerName-" + intSalesPayCustomerNameIndex] = JSON.stringify(arSalesPayCustomerName);
                        intSalesPayCustomerNameCntr = intSalesPayCustomerNameCntr + 1;
                        intSalesPayCustomerNameIndex = intSalesPayCustomerNameCntr;


                        displaySeniorCitizenInfoList(true);

                    } else {
                        MessageBox("Person with the same information already exists.", "Person Info");

                    } // end if existing
                } else {
                    if ($("#txtID").val() == '') {

                        AjaxSend({
                            url: "OrganizationShort/CheckPersonUniqueName",
                            data: { IsOrganization: 0,
                                OrganizationID: $("#txtID").val(),
                                PersonFullName: $.trim($("#txtFullname").val()),
                                PersonFirstName: $.trim($("#txtFirstName").val()),
                                PersonMiddleName: $.trim($("#txtMiddleName").val()),
                                PersonSurname: $.trim($("#txtSurname").val()),
                                PersonSuffixName: $.trim($("#selPersonalSuffixName").val()),
                                PersonTitleID: $.trim($("#txtTitleID").val()),
                                PersonDOB: $("#selDOBMonth").val() + '/' + $("#selDOBDay").val() + '/' + $("#selDOBYear").val(),
                                PersonMotherMaidenSurname: '',
                                PersonMotherMaidenFirstName: '',
                                PersonMotherMaidenMiddleName: '',
                                IsFull: false

                            },
                            success: function (vData) {

                                if (vData.OrganizationData.Rows.length > 0) {
                                    MessageBox("Person with the same information already exists.", "Person Info");

                                } else {
                                    AjaxSend({
                                        url: 'OrganizationShort/SaveOrganizationInfo',
                                        data: pDataPerson,
                                        success: function (vSaveData) {
                                            $("#txtID").val(vSaveData.OrganizationData);

                                            arSalesPayCustomerName = new Array();
                                            arSalesPayCustomerName[intSalesPayCustomerNameID] = $("#txtID").val();
                                            arSalesPayCustomerName[intSalesPayCustomerNameTitleID] = $.trim($("#txtTitleID").val());
                                            arSalesPayCustomerName[intSalesPayCustomerNameTitleName] = $.trim($("#btnTitle").text());
                                            arSalesPayCustomerName[intSalesPayCustomerNameFullName] = $.trim($("#txtFullname").val());
                                            arSalesPayCustomerName[intSalesPayCustomerNameSurname] = $.trim($("#txtSurname").val());
                                            arSalesPayCustomerName[intSalesPayCustomerNameFirstName] = $("#txtFirstName").val();
                                            arSalesPayCustomerName[intSalesPayCustomerNameMiddleName] = $.trim($("#txtMiddleName").val());
                                            arSalesPayCustomerName[intSalesPayCustomerNameSuffix] = $.trim($("#selPersonalSuffixName").val());
                                            arSalesPayCustomerName[intSalesPayCustomerNameDOBMonth] = $("#selDOBMonth").val();
                                            arSalesPayCustomerName[intSalesPayCustomerNameDOBDay] = $.trim($("#selDOBDay").val());
                                            arSalesPayCustomerName[intSalesPayCustomerNameDOBYear] = $.trim($("#selDOBYear").val());
                                            arSalesPayCustomerName[intSalesPayCustomerNameState] = (IsNewSalesPayCustomerName ? "New" : "Edit");

                                            sessionStorage["SalesPayCustomerName-" + intSalesPayCustomerNameIndex] = JSON.stringify(arSalesPayCustomerName);
                                            intSalesPayCustomerNameCntr = intSalesPayCustomerNameCntr + 1;
                                            intSalesPayCustomerNameIndex = intSalesPayCustomerNameCntr;

                                            displaySeniorCitizenInfoList(true);
                                        }
                                    });

                                }

                            }
                        }); // end ajaxsend
                    } else {
                        AjaxSend({
                            url: 'OrganizationShort/SaveOrganizationInfo',
                            data: pDataPerson,
                            success: function (vSaveData) {
                                $("#txtID").val(vSaveData.OrganizationData);

                                arSalesPayCustomerName = new Array();
                                arSalesPayCustomerName[intSalesPayCustomerNameID] = $("#txtID").val();
                                arSalesPayCustomerName[intSalesPayCustomerNameTitleID] = $.trim($("#txtTitleID").val());
                                arSalesPayCustomerName[intSalesPayCustomerNameTitleName] = $.trim($("#btnTitle").text());
                                arSalesPayCustomerName[intSalesPayCustomerNameFullName] = $.trim($("#txtFullname").val());
                                arSalesPayCustomerName[intSalesPayCustomerNameSurname] = $.trim($("#txtSurname").val());
                                arSalesPayCustomerName[intSalesPayCustomerNameFirstName] = $("#txtFirstName").val();
                                arSalesPayCustomerName[intSalesPayCustomerNameMiddleName] = $.trim($("#txtMiddleName").val());
                                arSalesPayCustomerName[intSalesPayCustomerNameSuffix] = $.trim($("#selPersonalSuffixName").val());
                                arSalesPayCustomerName[intSalesPayCustomerNameDOBMonth] = $("#selDOBMonth").val();
                                arSalesPayCustomerName[intSalesPayCustomerNameDOBDay] = $.trim($("#selDOBDay").val());
                                arSalesPayCustomerName[intSalesPayCustomerNameDOBYear] = $.trim($("#selDOBYear").val());
                                arSalesPayCustomerName[intSalesPayCustomerNameState] = (IsNewSalesPayCustomerName ? "New" : "Edit");

                                sessionStorage["SalesPayCustomerName-" + intSalesPayCustomerNameIndex] = JSON.stringify(arSalesPayCustomerName);
                                intSalesPayCustomerNameCntr = intSalesPayCustomerNameCntr + 1;
                                intSalesPayCustomerNameIndex = intSalesPayCustomerNameCntr;

                                ClearSalesPayCustomerNameFields();

                                displaySeniorCitizenInfoList(true);
                            }
                        });


                    } // end if id is empty
                } // end if offline


            };

            function displaySeniorCitizenInfoList(isBackOnce) {

                //                    if (window.location.pathname.toLowerCase() == '/salesmenu/posviewall') {

                if (CStr2($("#txtID").val()) != '') {


                    var vData = SalesPayment_offline().saveSeniorCitizenPWDCustomer($("#txtID").val(), sessionStorage.CartOrderID);


                    if (vData == 'ok') {

                        if (CStr2(sessionStorage.CurrentPage) != 'payment' && CDec2($("#lblSKUCountBilling").text()) > 0) {
                            location.hash = 'PreviewBill';
                            //                                //                            window.pageHeader.ShowPaymentsPage();
                            //                                if (isBackOnce) {
                            //                                    $.mobile.changePage('');
                            //                                    $.mobile.changePage('#PreviewBill');
                            //                                }
                            //                                else {
                            //                                    $.mobile.changePage("#PreviewBill", { changeHash: false });
                            //                                    //                                    $.mobile.changePage('#PreviewBill');
                            //                                }
                            $('#popupDiscounts').popup('open');

                            var vDataShow = SalesPayment_offline().getSeniorCitizenPWDCustomer(sessionStorage.CartOrderID);

                            intSalesPayCustomerNameCntr = window.pageDiscounts.LoadSavedCustomerNameList(vDataShow.Rows, false);

                        } else {

                            $.mobile.changePage('');

                            if (window.pageHeader != undefined)
                                window.pageHeader.ShowPaymentsPage(false);
                            var vDataShow = SalesPayment_offline().getSeniorCitizenPWDCustomer(sessionStorage.CartOrderID);

                            intSalesPayCustomerNameCntr = window.pageDiscounts.LoadSavedCustomerNameList(vDataShow.Rows, true);
                        }



                        if (window.pageDiscounts.checkDiscountDetails(true)) {
                            //                                RefreshTotalDiscountBilling();
                            window.pageDiscounts.RefreshSalesPaymentTotalBilling();
                        }



                    } else {
                        MessageBox(vData);
                    } //end  if (vData == 'ok')


                } else {

                    if (CStr2(sessionStorage.CurrentPage) != 'payment' && CDec2($("#lblSKUCountBilling").text()) > 0) {
                        if (isBackOnce) {
                            $.mobile.changePage('');
                            $.mobile.changePage('#PreviewBill');
                        } else {
                            $.mobile.changePage("#PreviewBill", { changeHash: false });
                            //                                $.mobile.changePage('#PreviewBill');
                        }


                        $('#popupDiscounts').popup('open');
                        var vDataShow = SalesPayment_offline().getSeniorCitizenPWDCustomer(sessionStorage.CartOrderID);

                        intSalesPayCustomerNameCntr = window.pageDiscounts.LoadSavedCustomerNameList(vDataShow.Rows, false);
                    } else {
                        $.mobile.changePage('');
                        if (window.pageHeader != undefined)
                            window.pageHeader.ShowPaymentsPage(false);
                        var vDataShow = SalesPayment_offline().getSeniorCitizenPWDCustomer(sessionStorage.CartOrderID);

                        intSalesPayCustomerNameCntr = window.pageDiscounts.LoadSavedCustomerNameList(vDataShow.Rows, true);

                    }


                } // end if (CStr2($("#txtID").val()) != '') 

                //                    }
                //                    else {
                //                        $.mobile.changePage('#pageSalesPayment');
                //                        //                        LoadSalesPayCustomerNameList();
                //                    } // end if (window.location.pathname.toLowerCase() == '/salesmenu/posviewall')


            }

            function countCustomerIdentification() {
                var cntr = 0;

                for (var i = 0; i <= intSalesPayCustomerIdentificationCntr; i++) {

                    if (sessionStorage["SalesPayCustomerIdentification-" + i] != undefined) {

                        arSalesPayCustomerIdentification = new Array();
                        arSalesPayCustomerIdentification = JSON.parse(sessionStorage["SalesPayCustomerIdentification-" + i]);

                        if (arSalesPayCustomerIdentification[intSalesPayCustomerIdentificationState] != "Remove") {
                            cntr = cntr + 1;
                        }

                    } //end if
                } //end for

                return cntr;

            } //countCustomer




            function SaveSalesPayCustomerIdentification() {

                arSalesPayCustomerIdentification = new Array();

                arSalesPayCustomerIdentification[intSalesPayCustomerIdentificationIDType] = $("#selIDType").val();
                arSalesPayCustomerIdentification[intSalesPayCustomerIdentificationIDOtherType] = $("#txtOtherIDType").val();
                arSalesPayCustomerIdentification[intSalesPayCustomerIdentificationIDText] = $.trim($("#txtIDText").val());
                arSalesPayCustomerIdentification[intSalesPayCustomerIdentificationState] = (IsNewSalesPayCustomerIdentification ? "New" : "Edit");

                sessionStorage["SalesPayCustomerIdentification-" + intSalesPayCustomerIdentificationIndex] = JSON.stringify(arSalesPayCustomerIdentification);

                if (proj.IsBootstrapDesign()) {
                    $('#popupAddNewID').modal('toggle');
                } else {
                    $('#popupAddNewID').popup('open');
                }

                LoadSalesPayCustomerIdentificationList('');

            };

            function RemoveSalesPayCustomerIdentification() {

                arSalesPayCustomerIdentification = new Array();
                arSalesPayCustomerIdentification[intSalesPayCustomerIdentificationIDType] = $("#selIDType").val();
                arSalesPayCustomerIdentification[intSalesPayCustomerIdentificationIDOtherType] = $("#txtOtherIDType").val();
                arSalesPayCustomerIdentification[intSalesPayCustomerIdentificationIDText] = $("#txtIDText").val();
                arSalesPayCustomerIdentification[intSalesPayCustomerIdentificationState] = "Remove";

                sessionStorage["SalesPayCustomerIdentification-" + intSalesPayCustomerIdentificationIndex] = JSON.stringify(arSalesPayCustomerIdentification);

                //                $.mobile.changePage('#pageAddNewCustomer');

                //                LoadSalesPayCustomerIdentificationList('');
                //                window.pageDiscounts.ShowSavedCustomerNameInfo($("#txtSCPWDCustomerID").val());

            };

            function RemoveSalesPayCustomerIdentificationSaved() {
                RemoveSalesPayCustomerIdentification();

                if (OfflineMode) {
                    var vData = SalesPayment_offline().removeSeniorCitizenPWDCustomerIdentification($("#txtID").val(), $("#selIDType").val());

                    if (vData == 'ok') {
                        $.mobile.changePage('#pageAddNewCustomer');

                        LoadSalesPayCustomerIdentificationList('');
                        window.pageDiscounts.ShowSavedCustomerNameInfo($("#txtSCPWDCustomerID").val());

                    } else {

                        MessageBox(vData);
                    }

                } else {

                    MessageBox('Not yet implemented');

                }



            };


            $('#btnBackSenior').live('click', function (e) {
                e.preventDefault();
                if (CStr2(sessionStorage.IsEnableDiningMode) == 'true') {
                    if (CStr2(sessionStorage.CurrentPage) != 'payment' && CDec2($("#lblSKUCountBilling").text()) > 0) {

                        $.mobile.changePage("#PreviewBill", { changeHash: false });

                        $('#popupDiscounts').popup('open');

                        var vDataShow = SalesPayment_offline().getSeniorCitizenPWDCustomer(sessionStorage.CartOrderID);

                        intSalesPayCustomerNameCntr = window.pageDiscounts.LoadSavedCustomerNameList(vDataShow.Rows, false);
                    } else {
                        try {
                            $.mobile.changePage('');

                            window.pageHeader.ShowPaymentsPage(true);
                            var vDataShow = SalesPayment_offline().getSeniorCitizenPWDCustomer(sessionStorage.CartOrderID);

                            intSalesPayCustomerNameCntr = window.pageDiscounts.LoadSavedCustomerNameList(vDataShow.Rows, true);
                        } catch (ex) {
                        }
                    }
                } else {
                    if (window.location.pathname.toLowerCase() == '/salesmenu/posviewall') {
                        location.hash = '';
                        window.pageHeader.ShowPaymentsPage(false);
                    } else
                        $.mobile.changePage('#pageSalesPayment');

                }

            });

            $('#btnSalesPayCustomerNameCancel').live('click', function (e) {
                e.preventDefault();

                if (CStr2(sessionStorage.IsEnableDiningMode) == 'true') {
                    if (CStr2(sessionStorage.CurrentPage) != 'payment' && CDec2($("#lblSKUCountBilling").text()) > 0) {

                        $.mobile.changePage("#PreviewBill", { changeHash: false });

                        $('#popupDiscounts').popup('open');

                        var vDataShow = SalesPayment_offline().getSeniorCitizenPWDCustomer(sessionStorage.CartOrderID);

                        intSalesPayCustomerNameCntr = window.pageDiscounts.LoadSavedCustomerNameList(vDataShow.Rows, false);
                    } else {
                        try {
                            if (proj.IsBootstrapDesign()) {
                                $('#pageSalesPayment').pageshow();
                            } else {
                                $.mobile.changePage('');
                            }                            

                            window.pageHeader.ShowPaymentsPage(true);
                            var vDataShow = SalesPayment_offline().getSeniorCitizenPWDCustomer(sessionStorage.CartOrderID);

                            intSalesPayCustomerNameCntr = window.pageDiscounts.LoadSavedCustomerNameList(vDataShow.Rows, true);
                        } catch (ex) {
                        }
                    }
                } else {
                    if (window.location.pathname.toLowerCase() == '/salesmenu/posviewall') {
                        location.hash = '';
                        window.pageHeader.ShowPaymentsPage(false);
                    } else {
                        if (proj.IsBootstrapDesign()) {
                            $('#pageSalesPayment').pageshow();
                        } else {
                            $.mobile.changePage('#pageSalesPayment');
                        }
                    }
                }

            });
            $('#btnSalesPayCustomerIdentificationSave').live('click', function (e) {

                e.preventDefault();

                //                    if (CStr2(sessionStorage.IsEnableDiningMode) == 'true') {
                if ($.trim($("#selIDType").val()) == "") {
                    $('#selIDType').focus();
                    if (OfflineMode)
                        Android.Toast('Please select ID Type');
                    return;
                } else {

                    if ($("#selIDType").val() == 'Other') {
                        if ($.trim($("#txtOtherIDType").val()) == "") {
                            $('#txtOtherIDType').focus();
                            if (OfflineMode)
                                Android.Toast('Please enter ID Type');
                            return;
                        }
                    }
                }

                if ($.trim($("#txtIDText").val()) == "") {
                    $('#txtIDText').focus();
                    if (OfflineMode)
                        Android.Toast('Please enter ID Number');
                    return;
                }

                //                        if (CStr2(sessionStorage.CurrentPage) == 'payment') {
                //                            proj.ClickEventSaveSalesPayCustomerIdentification();
                //                        } else {
                SaveSalesPayCustomerIdentification();

                //                        }


                //                    } else {

                //                        proj.ClickEventSaveSalesPayCustomerIdentification();
                //                    }
            });

            $('#btnSalesPayCustomerIdentificationRemove').live('click', function (e) {
                e.preventDefault();

                MessageBox("Remove this record?",
                            'Remove',
                            [
                                { Text: 'Yes',
                                    Click: function () {
                                        if (CStr2(sessionStorage.IsEnableDiningMode) == 'true') {


                                            if (CStr2(sessionStorage.CurrentPage) == 'payment') {
                                                proj.ClickEventRemoveSalesPayCustomerIdentification();
                                            } else {
                                                RemoveSalesPayCustomerIdentificationSaved();
                                            }

                                        } else {
                                            proj.ClickEventRemoveSalesPayCustomerIdentification();
                                        }
                                    }
                                }
                            ,
                                { Text: 'No'
                                }
                            ]
                        );

            });
            $('#btnSalesPayCustomerNameSave').live('click', function (e) {
                e.preventDefault();

                if ($.trim($("#txtMiddleName").val()) == "" && $.trim($("#txtFirstName").val()) == "" && $.trim($("#txtSurname").val()) == "") {
                    if ($.trim($("#txtFullname").val()) == "" || $.trim($("#txtFullname").val()).length <= 1) {
                        MessageBox("Please enter full name.",
                                    "Full Name",
                                    [
                                        { Text: 'Ok' }
                                    ]
                                );
                        return;
                    }
                }

                if ($.trim($("#txtFullname").val()) == "" || $.trim($("#txtFullname").val()).length <= 1) {
                    if ($.trim($("#txtSurname").val()) == "" || $.trim($("#txtSurname").val()).length <= 1) {
                        MessageBox("Please enter surname.",
                                    "Surname",
                                    [
                                        { Text: 'Ok' }
                                    ]
                                );
                        return;
                    }

                    if ($.trim($("#txtFirstName").val()) == "" || $.trim($("#txtFirstName").val()).length <= 1) {
                        MessageBox("Please enter first name.",
                                    "First Name",
                                    [
                                        { Text: 'Ok' }
                                    ]
                                );
                        return;
                    }

                    if ($.trim($("#txtMiddleName").val()) == "" || $.trim($("#txtMiddleName").val()).length <= 1) {
                        MessageBox("Please enter middle name.",
                                    "Middle Name",
                                    [
                                        { Text: 'Ok' }
                                    ]
                                );
                        return;
                    }
                }

                if (proj.IsBootstrapDesign()) {
                    if ($.trim($("#txtSalesPayCustomerBirthDate").val()) == "") {
                        MessageBox("Please enter valid date of birth.",
                                    "Date Of Birth",
                                    [
                                        { Text: 'Ok' }
                                    ]
                                );
                        return;
                    }
                } else {
                    if ($.trim($("#selDOBMonth").val()) == "") {
                        MessageBox("Please enter valid date of birth.",
                                    "Date Of Birth",
                                    [
                                        { Text: 'Ok' }
                                    ]
                                );
                        return;
                    }
                    if ($.trim($("#selDOBDay").val()) == "") {
                        MessageBox("Please enter valid date of birth.",
                                    "Date Of Birth",
                                    [
                                        { Text: 'Ok' }
                                    ]
                                );
                        return;
                    }
                    if ($.trim($("#selDOBYear").val()) == "") {
                        MessageBox("Please enter valid date of birth.",
                                    "Date Of Birth",
                                    [
                                        { Text: 'Ok' }
                                    ]
                                );
                        return;
                    }
                }

                if ($.trim($("#txtSCPWDTIN").val()) != '' && ($.trim($("#txtSCPWDTIN").val()).length < 12 || $.trim($("#txtSCPWDTIN").val()).length > 15)) {
                    NewMessageBox('Please enter valid 12-digit TIN');
                    return
                }

                if ($('#divSoloParentChildInfo').css('display') == 'block') {
                    if ($.trim($("#txtChildName").val()) == '') {
                        NewMessageBox('Please enter the child\'s name');
                        return
                    }

                    if ($.trim($("#txtChildBirthdate").val()) == '') {
                        NewMessageBox('Please enter the child\'s birthday');
                        return
                    }

                    if ($.trim($("#txtChildAge").val()) == '') {
                        NewMessageBox('Please enter the child\'s age');
                        return
                    }
                }

                if (CStr2(sessionStorage.IsEnableDiningMode) == 'true') {


                    if (countCustomerIdentification() < 1) {
                        MessageBox("Please add at least one valid identification number.",
                                "Identification Number",
                                [
                                    { Text: 'Ok' }
                                ]
                            );
                        return;
                    }


                    SaveSalesPayCustomerNameBilling();

                } else {
                    proj.ClickEventSaveSalesPayCustomerName();
                }

            });




            window.pageDiscounts.checkDiscountDetails = function (showDiscountDetails) {

                if (showDiscountDetails) {
                    if (CDec2($.trim($("#txtSalesPayNoOfSCBilling").val())) < 1) {
                        if (OfflineMode) {
                            Android.Toast("Please enter the number of Senior Citizens.");
                        } else {
                            MessageBox("Please enter the number of Senior Citizens.",
                               "Number of Senior Citizens",
                                [
                                    { Text: 'Ok'
                                    }
                                ]
                                );
                        }
                        return false;
                    } else {

                        if (CDec2($.trim($("#txtSalesPayNoOfSCBilling").val())) < window.pageDiscounts.countCustomer()) {
                            if (OfflineMode) {
                                Android.Toast("You've exceeded the number people you can list. Please remove a record.");
                            } else {
                                MessageBox("You've exceeded the number people you can list. Please remove a record.",
                                    "List of Senior Citizens / PWD Availing the Discount",
                                    [
                                        { Text: 'Ok'
                                        }
                                    ]
                                    );
                            }
                            return false;
                        }

                        if (CDec2($.trim($("#txtSalesPayNoOfSCBilling").val())) > CDec2($.trim($("#txtSalesPayNoOfDinersBilling").val()))) {
                            if (OfflineMode) {
                                Android.Toast("The number of senior citizens should be less than or equal to the total number of people.");
                            } else {
                                MessageBox("The number of senior citizens should be less than or equal to the total number of people.",
                                   "Number of People (including the Senior Citizen)",
                                    [
                                        { Text: 'Ok'
                                        }
                                    ]
                                    );
                            }
                            return false;
                        }

                    } //end if number of Senior Citizens

                    if (CDec2($.trim($("#txtSalesPayNoOfDinersBilling").val())) < 1) {
                        if (OfflineMode) {
                            Android.Toast("Please enter the number of people (including the Senior Citizen).");
                        } else {
                            MessageBox("Please enter the number of people (including the Senior Citizen).",
                           "Number of People (including the Senior Citizen)",
                            [
                                { Text: 'Ok'
                                }
                            ]
                            );
                        }
                        return false;
                    }

                    if (window.pageDiscounts.countCustomer() < parseInt($.trim($("#txtSalesPayNoOfSCBilling").val()))) {
                        if (OfflineMode) {
                            Android.Toast("Please add " + $.trim($("#txtSalesPayNoOfSCBilling").val()) + " customer record/s to the list.");
                        } else {
                            MessageBox("Please add " + $.trim($("#txtSalesPayNoOfSCBilling").val()) + " customer record/s to the list.",
                                "List of Senior Citizens / PWD Availing the Discount",
                                [
                                    { Text: 'Ok' }
                                ]
                            );
                        }
                        return false;
                    }
                } else {
                    if (window.pageDiscounts.countCustomer() > 1) {
                        if (OfflineMode) {
                            Android.Toast("You've exceeded the number people you can list. Please remove a record.");
                        } else {
                            MessageBox("You've exceeded the number people you can list. Please remove a record.",
                                   "List of Senior Citizens / PWD Availing the Discount",
                                    [
                                        { Text: 'Ok',
                                            Click: function () {

                                                $.mobile.changePage('');

                                                if (window.location.pathname.toLowerCase() == '/salesmenu/posviewall')
                                                    window.pageHeader.ShowPaymentsPage();

                                            }
                                        }
                                    ]
                                    );
                        }
                        return false;
                    }

                    if (window.pageDiscounts.countCustomer() < 1) {
                        if (OfflineMode) {
                            Android.Toast("Please add a customer to the listt.");
                        } else {
                            MessageBox("Please add a customer to the list.",
                                "List of Senior Citizens / PWD Availing the Discount",
                                [
                                    { Text: 'Ok',
                                        Click: function () {

                                            $.mobile.changePage('');

                                            if (window.location.pathname.toLowerCase() == '/salesmenu/posviewall')
                                                window.pageHeader.ShowPaymentsPage();

                                        }
                                    }
                                ]
                            );
                        }
                        return false;
                    }

                } // end if showDiscountDetails


                //                if (!window.pageDiscounts.isCompleteCustomerData()) {
                //                    if (OfflineMode) {
                //                        Android.Toast("Some customer/s in the list have incomplete data. Please fill in the Date of Birth and add at least one Valid ID.");
                //                    } else {
                //                        MessageBox("Some customer/s in the list have incomplete data. Please fill in the Date of Birth and add at least one Valid ID.",
                //                                "List of Senior Citizens / PWD Availing the Discount",
                //                                [
                //                                    { Text: 'Ok' }
                //                                ]
                //                            );
                //                    }
                //                    return false;

                //                }

                if (sessionStorage.IsPaymentJSLoaded == 'true')
                    proj.RefreshTotalDiscount();

                return true;
            }

            window.pageDiscounts.countCustomer = function () {

                var cntr = 0;

                if (OfflineMode) {
                    var vDataShow = SalesPayment_offline().getSeniorCitizenPWDCustomer(sessionStorage.CartOrderID);

                    if (vDataShow.Rows != undefined)
                        cntr = vDataShow.Rows.length;
                }

                return cntr;

            } //countCustomer

            window.pageDiscounts.isCompleteCustomerData = function () {

                var cntr = 0;

                for (var i = 0; i <= intSalesPayCustomerNameCntr; i++) {

                    if (sessionStorage["SalesPayCustomerName-" + i] != undefined) {

                        arSalesPayCustomerName = new Array();
                        arSalesPayCustomerName = JSON.parse(sessionStorage["SalesPayCustomerName-" + i]);

                        if (arSalesPayCustomerName[intSalesPayCustomerNameState] != "Remove") {
                            if (CDec2(arSalesPayCustomerName[intSalesPayCustomerNameDOBMonth]) == '')
                                return false;


                        }

                    } //end if
                } //end for


                return true;



            } //isCompleteCustomerData



            window.pageDiscounts.ShowSavedCustomerNameInfo = function (id) {
                if (OfflineMode) {
                    var vData = SalesPayment_offline().getOrganizationInfoBilling(id);

                    if (vData.OrganizationData != undefined) {

                        if (vData.OrganizationData.Rows.length > 0) {
                            seniorCitizenDetailsPage(vData.OrganizationData.Rows, id);
                        }
                    } else {

                        MessageBox('No data found.');
                    }

                } else {

                    MessageBox('Not yet implemented');

                }

            };
        }); // $(function() {]]

        // END require
    }); 