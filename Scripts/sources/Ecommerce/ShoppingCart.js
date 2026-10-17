require(['common'],
function () {    
    $(document).ready(function () {
        GetData();

        $('body')
        .on('click', '#btnShoppingCartContinueShopping', function () {
            location.href = '/Ecommerce?OrgID=' + sessionStorage.OrgID;
        })
        .on('click', '#btnMyCartCheckout', function () {
            window.location.href = '/Ecommerce/Checkout?OrgID=' + sessionStorage.OrgID;
        })

        function GetData() {
            AjaxSend({
                url: '/Ecommerce/EcommerceCart',
                data: {
                    FromEcommerce: 1
                },
                success: function(vData) {
                    LoadData(GetStringObject('Result.Rows', vData));
                }
            });
        }

        function LoadData(arrMyCart) {
            var tbodyMyCart = $('#tbodyMyCart');
            //var arrMyCart = JSON.parse(JSON.parse(JSON.stringify(sessionStorage.MyCart || "[]")));
            var grandtotal = 0;

            tbodyMyCart.empty();
            if (arrMyCart == null)
                return;

            if ((arrMyCart != null)
                && (arrMyCart.length > 0)) {
                if (arrMyCart[0].VendorCode == undefined) {
                    $('.cart-vendor').remove();
                }

                for (var i = 0; i < arrMyCart.length; i++) {
                    var subtotal = Ecommerce.ApplyUpcharge(arrMyCart[i]);
                    grandtotal += subtotal;

                    var img = '';

                    if (arrMyCart[i].VendorID != undefined) {
                        if (arrMyCart[i].FileName != undefined) {
                            img = ImageViewer.DefaultImage('/Content/MobizImagePIM/' + sessionStorage.OrgID.toLowerCase() + '/Medium/' + arrMyCart[i].VendorID + '/' + arrMyCart[i].FileName, arrMyCart[i]).css('width', '100px');
                        }
                    } else {
                        img = ImageViewer.DefaultImage((arrMyCart[i].FileName == undefined ? '/Content/Ecommerce/images/products/p1.jpg' : "/Content/MobizImagePIM/" + sessionStorage.OrgID + '/medium/' + arrMyCart[i].FileName), arrMyCart[i]).css('width', '100px');
                    }

                    tbodyMyCart.append($('<tr>')
                            .append($('<td class="romove-item text-center" style="cursor:pointer;">')
                                .data('data', arrMyCart[i])
                                .append($('<i class="bi-trash text-danger" style="font-size:30px;">'))
                                .click(function () {
                                    var data = $(this).data('data');
                                    //var temp = JSON.parse(JSON.parse(JSON.stringify(sessionStorage.MyCart)));
                                    var temp = arrMyCart;
                                    var isExist = false; ind = 0;

                                    NewMessageBox('Are you sure you want to remove this item?', 'Message', [
                                        {
                                            Text: 'Yes', Click: function () {
                                                //$.grep(temp, function (e, i) {
                                                //    if (e.ProductID == data.ProductID) {
                                                //        isExist = true;
                                                //        ind = i;
                                                //        return false; // stop
                                                //    }
                                                //})

                                                var pData = {
                                                    ProductID: data.ProductID,
                                                    TransInvItemID: data.ID,
                                                    VendorID: CStr2(data.VendorID)
                                                };
                                                AjaxSend({
                                                    url: '/Ecommerce/RemoveCartItem',
                                                    data: pData,
                                                    success: function(vData) {
                                                        if (vData.Error)
                                                            NewMessageBox(vData.Error);
                                                        else if (vData.Result == 'ok') {
                                                            GetData();
                                                            Ecommerce.DisplayDropdownCartTotal(); // ecommerce.js
                                                        } else
                                                            NewMessageBox('Unknown result. ref: jsMV2RGBT4YV!');
                                                    }
                                                });
                                                //temp.splice(ind, 1);
                                                //sessionStorage.MyCart = JSON.stringify(temp);
                                                //LoadData();
                                                //Ecommerce.DisplayDropdownCartTotal(); // ecommerce.js
                                            }
                                        }, { Text: 'No' }
                                    ])
                                })
                            )
                            .append($('<td style="text-align:center;">')
                                .data('data', arrMyCart[i])
                                .append(img)
                                //.click(function () {
                                //    var data = $(this).data('data');
                                //    var temp = JSON.parse(JSON.parse(JSON.stringify(sessionStorage.MyCart)));
                                //})
                            )
                            .append($('<td class="cart-vendor-info" style="display:' + (CStr2(arrMyCart[i].VendorCode) != '' ? 'block' : 'none') + '">')
                                .append((CStr2(arrMyCart[i].VendorCode) != '' ? 'Code: ' + arrMyCart[i].VendorCode : '') +
                                        (CStr2(arrMyCart[i].VendorName) != '' ? '<br>Name: ' + arrMyCart[i].VendorName : '')
                                )
                            )
                            .append($('<td class="cart-product-name-info">')
                                .append($('<h5 class="cart-product-description text-dark">')
                                    .append($('<a data-role="none">').text(arrMyCart[i].ProductName))
                                )
                            )
                            .append($('<td class="cart-product-quantity">')
                                .append($('<form autocomplete="off" style="position:relative;">')
                                    .append($('<input data-role="none" value="' + $.format.number(CDec2(arrMyCart[i].Qty), '#,###.##') + '" class="form-control unicase-form-control" type="text">')
                                        .data('data', arrMyCart[i])
                                        .focusin(function () {
                                            $(this).next().show();
                                        })
                                        .blur(function () {
                                            var dis = $(this);
                                            setTimeout(function () {
                                                dis.next().hide();
                                                dis.change();
                                            }, 150)                                            
                                        })
                                        .keydown(function (e) {
                                            if (!NumberDotOnly($(this), e)) {
                                                return;
                                            }
                                        })
                                        .change(function () {
                                            var data = $(this).data('data');
                                            var temp = arrMyCart;
                                            var isExist = false; ind = 0, grandtotal = 0;

                                            $.grep(temp, function (e, i) {
                                                if (e.ProductID == data.ProductID) {
                                                    isExist = true;
                                                    ind = i;
                                                    return false; // stop
                                                }
                                            })

                                            if (isExist) {
                                                temp[ind].Qty = $.trim($(this).val());
                                                sessionStorage.MyCart = JSON.stringify(temp);
                                                $(this).parents().closest('tr').find('.cart-sub-total-price').text(AccountingFormat(Ecommerce.ApplyUpcharge(temp[ind])));

                                                $.grep(temp, function (e, i) {
                                                    grandtotal += Ecommerce.ApplyUpcharge(e);
                                                })

                                                $('#lblCartGrandTotal').text('P ' + AccountingFormat(grandtotal));                                                

                                                var pData = {
                                                    FromEcommerce: 1,
                                                    //CustomerID: mOut.Model.Data.CustomerID,
                                                    TransType: 'SalesOrder',
                                                    DeleteExistingProduct: 0,
                                                    DontCheckCartValidity: 1,
                                                    //OrderID: mOut.Model.Data.ID,
                                                    //NewCartMode: !IsGuid(mOut.Model.Data.ID),
                                                    StockID: data.ProductID,
                                                    SalesRep: [],
                                                    ProductSerial: [],
                                                    UpdateOrderedQty: 1,
                                                    Stocks: [
                                                        {
                                                            Qty: $.trim($(this).val()),
                                                            UnitQty: 1
                                                        }
                                                    ]
                                                };

                                                if (CStr2(data.VendorID) != '') {
                                                    pData.Stocks[0].VendorID = data.VendorID;
                                                    pData.Stocks[0].EditPrice = data.Price;
                                                    pData.Stocks[0].UnitMeasure = data.UnitMeasure;
                                                }

                                                AjaxSend({
                                                    url: '/Stock/UpdatePos',
                                                    data: pData,
                                                    NoSplash: true,
                                                    success: function (vData) {
                                                        if (vData.Error) {
                                                            NewMessageBox(vData.Error);
                                                        } else if (vData.Result == 'ok') {
                                                            Ecommerce.DisplayDropdownCartTotal(); // ecommerce.js
                                                        } else {
                                                            NewMessageBox('Unknown status. ref: jsCSLRU5TDDJ!');
                                                        }

                                                    }
                                                });
                                            }
                                        })
                                        .keyup(function () {
                                            var keyword = $.trim($(this).val()).toLowerCase();
                                            var choices = $(this).data('data').QtyChoices;
                                            var listQtyInterval = $(this).next();
                                            listQtyInterval.empty();

                                            if (keyword != '') {
                                                $.grep(choices, function (e) {
                                                    if (e.QtyChoices.toLowerCase().indexOf(keyword) > -1) {
                                                        listQtyInterval.append(Ecommerce.DisplayQtyInterval(e))
                                                    }
                                                });
                                                listQtyInterval.show();
                                            } else {
                                                for (var i = 0; i < choices.length; i++) {
                                                    $('#' + $(this).data('data').ID).append(Ecommerce.DisplayQtyInterval(choices[i]))
                                                }
                                            }
                                        })
                                    )
                                    .append($('<div id="' + arrMyCart[i].ID + '" class="list-group list-autosuggest qtyintervallist" style="display:none;">'))
                                )
                            )
                            .append($('<td class="cart-product-price text-center">')
                                .append($('<span>')
                                    .text('P ' + AccountingFormat(arrMyCart[i].Price) + ' per ' + arrMyCart[i].UnitMeasure)
                                    .append($('<div id="divupcharge-' + arrMyCart[i].ID + '">').append(arrMyCart[i].Upcharge.length == 0 ? '' : '+ ' + CDec2(arrMyCart[i].Upcharge[0].Upcharge) + '% upcharge for orders below 1 ' + arrMyCart[i].UnitMeasure))
                                )
                            )
                            .append($('<td class="cart-product-sub-total">')
                                .append($('<span class="cart-sub-total-price">')
                                    .text('P ' + AccountingFormat(subtotal))
                                )
                            )
                        )

                    for (var q = 0; q < arrMyCart[i].QtyChoices.length; q++) {
                        $('#' + arrMyCart[i].ID).append(Ecommerce.DisplayQtyInterval(arrMyCart[i].QtyChoices[q]));
                    }
                }
            } else {
                tbodyMyCart.append($('<tr>')
                        .append($('<td colspan="7" style="text-align:center;">')
                            .text('Your cart is empty')
                        )
                    )
            }            

            $('#lblCartGrandTotal').text('P ' + AccountingFormat(grandtotal));
        }
    })
})