require(['common'], function () {
    LoadRequiredData();

    $('body')
    .on('click', '#butContinueShopping', function () {
        location.href = '/Ecommerce?OrgID=' + sessionStorage.OrgID;
    })
    .on('click', '#butCheckouContinue', function (e) {
        e.preventDefault();
        var isValid = true;
        var selCheckoutBillingAddress = $('#selCheckoutBillingAddress');
        var selCheckoutDeliveryAddress = $('#selCheckoutDeliveryAddress');
        var txtCheckoutComments = $('#txtCheckoutComments');

        //if (selCheckoutBillingAddress.find('option:selected').val() == '') {
        //    selCheckoutBillingAddress.parent().addClass('has-error');
        //    isValid = false;
        //} else {
        //    selCheckoutBillingAddress.parent().removeClass('has-error');
        //}

        //if (selCheckoutDeliveryAddress.find('option:selected').val() == '') {
        //    selCheckoutDeliveryAddress.parent().addClass('has-error');
        //    isValid = false;
        //} else {
        //    selCheckoutDeliveryAddress.parent().removeClass('has-error');
        //}

        if(!isValid){
            $('#divCheckoutError').fadeIn();
            return;
        } else {
            $('#divCheckoutError').fadeOut();
        }

        AjaxSend({
            url: '/Ecommerce/CheckoutNow',
            data: {
                data: {
                    BillingAddressID: selCheckoutBillingAddress.find('option:selected').val(),
                    DeliveryAddressID: selCheckoutDeliveryAddress.find('option:selected').val(),
                    Comments: $.trim(txtCheckoutComments.val()),
                    DeliveryMethod: $('[name="radDeliveryMethod"]:checked').val()
                }
            },
            success: function(vData) {
                if (vData.Error) {
                    NewMessageBox(vData.Error);
                } else if (vData.Results == 'ok') {
                    sessionStorage.sonum = vData.son;
                    window.location.href = '/Ecommerce/OrderSuccess?OrgID=' + sessionStorage.OrgID;
                } else {
                    NewMessageBox('Unknown status. ref: jsCSLRU5TDDJ!');
                }

            }
        });
        return false;
    })
    .on('click', '#btnAddressSave', function (e) {
        e.preventDefault();
        e.stopPropagation();
        SaveAddress();
    })

    function LoadRequiredData() {
        var tbodyCheckoutCart = $('#tbodyCheckoutCart');

        tbodyCheckoutCart.empty();

        AjaxSend({
            url: '/Ecommerce/EcommerceCart',
            data: {
                FromEcommerce: 1
            },
            success: function (vData) {
                var vRows = vData.Result.Rows;
                var orderTotal = 0;

                if (vRows[0].VendorCode == undefined) {
                    $('#thCheckoutVendor').hide();
                } else {
                    $('#thCheckoutVendor').show();
                }

                for (var i = 0; i < vRows.length; i++) {
                    var img = '';
                    var lineTotal = Ecommerce.ApplyUpcharge(vRows[i]);
                    orderTotal += CDec2(lineTotal);

                    if (vRows[i].VendorID != undefined) {
                        if (vRows[i].FileName != undefined) {
                            img = ImageViewer.DefaultImage('/Content/MobizImagePIM/' + sessionStorage.OrgID.toLowerCase() + '/Medium/' + vRows[i].VendorID + '/' + vRows[i].FileName, vRows[i]).css('width', '50px');
                        }
                    } else {
                        img = ImageViewer.DefaultImage((vRows[i].FileName == undefined ? '/Content/Ecommerce/images/products/p1.jpg' : "/Content/MobizImagePIM/" + sessionStorage.OrgID + '/medium/' + vRows[i].FileName), vRows[i]).css('width', '50px');
                    }

                    tbodyCheckoutCart.append($('<tr>')
                            .append($('<td>').append(img))
                            .append($('<td style="display:' + (CStr2(vRows[i].VendorCode) != '' ? 'block' : 'none') + '">')
                                    .append((CStr2(vRows[i].VendorCode) != '' ? 'Code: ' + vRows[i].VendorCode : '') +
                                            (CStr2(vRows[i].VendorName) != '' ? '<br>Name: ' + vRows[i].VendorName : '')
                                    )
                                )
                            .append($('<td>').text(vRows[i].ProductName))
                            .append($('<td style="text-align:center;">').text(CDec2(vRows[i].Qty)))
                            .append($('<td style="text-align:center;">').text('P ' + AccountingFormat(vRows[i].Price) + ' per ' + vRows[i].UnitMeasure)
                                .append($('<div id="divupcharge-' + vRows[i].ID + '">').append(vRows[i].Upcharge.length == 0 ? '' : '+ ' + CDec2(vRows[i].Upcharge[0].Upcharge) + '% upcharge for orders below 1 ' + vRows[i].UnitMeasure))
                            )
                            .append($('<td style="text-align:right;">').text('P ' + AccountingFormat(lineTotal)))
                        )
                }

                tbodyCheckoutCart.append($('<tr>')
                            .append($('<td colspan="6" style="text-align:right; font-weight:bold;">').text('TOTAL: P ' + AccountingFormat(orderTotal)))
                        )
            }
        });

        AjaxSend({
            url: '/Ecommerce/GetEcommerceGeneralSettings',
            success: function (e) {
                if (CBool(e.EcommerceAcceptDeliveryOrders)) {
                    $('#divDeliveryMethod').show();
                } else {
                    $('#divDeliveryMethod').empty().hide();
                }

                if (CBool(e.EcommerceAllowPickupOrders)) {
                    $('#divStorePickup').show();
                } else {
                    $('#divStorePickup').empty().hide();
                }

                if ($.trim(CStr2(e.EcommerceCheckoutMessage)) != '') {
                    $('#divCheckoutMessage').html(CStr2(e.EcommerceCheckoutMessage).replace(/(?:\r\n|\n|\r)/g, '<br>')).show();
                } else {
                    $('#divCheckoutMessage').remove();
                }

                if (CBool(e.EcommercePaymentMethodPaypal)) {
                    $('#divPMPaypal').show();
                } else {
                    $('#divPMPaypal').remove();
                }
            }
        })

        GetAddresses();
    }

    function GetAddresses() {
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
                        var selCheckoutBillingAddress = $('#selCheckoutBillingAddress');
                        var selCheckoutDeliveryAddress = $('#selCheckoutDeliveryAddress');

                        selCheckoutBillingAddress.empty();
                        selCheckoutDeliveryAddress.empty();

                        if (vRows.length > 0) {
                            selCheckoutBillingAddress.append(new Option('Please select billing address', ''));
                            selCheckoutDeliveryAddress.append(new Option('Please select delivery address', ''));

                            for (var i = 0; i < vRows.length; i++) {
                                vRows[i].OrderNo = (i + 1)
                                selCheckoutBillingAddress.append(new Option(vRows[i].Address, vRows[i].ID))
                                selCheckoutDeliveryAddress.append(new Option(vRows[i].Address, vRows[i].ID));
                            }
                        } else {
                            selCheckoutBillingAddress.append(new Option('No billing addresses found. Click "+" to add.', ''));
                            selCheckoutDeliveryAddress.append(new Option('No delivery addresses found. Click "+" to add.', ''));
                        }
                    }
                })
            }
        })
    }

    function SaveAddress() {
        var txtCompleteAddress = $.trim($('#txtCompleteAddress').val());

        if (txtCompleteAddress == '' && addressAction != 'delete') {
            WebToast('Please enter complete address', 'top');
            return;
        }

        var data = {
            ID: '',
            Address: txtCompleteAddress,
            Action: 'add'
        }

        AjaxSend({
            url: 'Ecommerce/SaveAddress',
            data: { data: data },
            success: function (e) {
                if (e.Results == 'saved') {
                    WebToast('New address has been successfully added', 'top');
                    GetAddresses();
                    $('#popupNewAddress').modal('hide');
                } else {
                    WebToast(e.Results, 'top');
                }
            }
        })
    }
});