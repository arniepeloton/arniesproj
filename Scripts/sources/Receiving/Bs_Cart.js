require([
    'common',
    '/Scripts3/Browser/LibraryPIMProduct.js',
    'promise!/Scripts3/model/ReceivingCart.js?crc=',
    '/Content/Ecommerce/js/lightbox-2.11.1.js',
], function () {
    var ctrItem = 0;
    var ProductID = '';
    var ProductName = '';
    var UnitMeasureValue = "";
    var BadOrderReason = "";
    var AdditionalInfo = "";
    var IsResaleable = 0;
        var selectedItem = '';
        var ShowBadCount = CBool(Business.SystemDefaults.GetSystemDefault('AllowSRBadCount'));

    $(function () {

        if (!IsMobile) {
            $('#btnReceiveByContinuousScanning').css('visibility', 'hidden');
            $('#btnReceiveByContinuousScanning').css('display', 'none');
        }

        $('#btnScanBarCode').click(function (e) {
            mobile.Scan({
                Success: function (vData) {
                    $('#txtSearchItem').val(vData.Value);
                    SearchItem();
                }
            });
        })

        $('#txtSearchItem').keypress(function (e) {            
            if (e.keyCode == 13) {
                $(this).blur();
                SearchItem();
                $(this).val('');
            }
        });

        $('#btnSearchItem').click(function (e) {
            SearchItem();
        });

        //$('#txtProductSerials').keypress(function (e) {
        //    if (e.keyCode == 13) {
        //        AddToSerialList($('#txtProductSerial').val());
        //    }
        //});

        $('#txtProductSerials').keydown(function (e) {
            if (e.which == 13) {
                AddToSerialList($('#txtProductSerials').val());
            }
        });
        
        $('#butSerialBarCode').click(function (e) {
            mobile.Scan({
                Success: function (vData) {
                    AddToSerialList(vData.Value);
                }
            });
        });

        $('#btnPlusOne').click(function () {
            $('#txtCount').val((parseInt($('#txtCount').val()) + 1).toString())
        });

        $('#btnMinusOne').click(function () {
            $('#txtCount').val((parseInt($('#txtCount').val()) - 1).toString())
        });

        $('#btnBackCounter').click(function () {
            if (parseInt($('#txtCount').val()) < 0) {
                NewMessageBox('Quantity must be a number greater than 0.',
                    "Invalid Quantity"
                );
                //$('#lblCountError').css('visibility', 'visible');
            }
            else {
                $('#txt' + UnitMeasureValue).val($('#txtCount').val());
                history.back();
            }

        });

        $('#btnReset').click(function () {
            NewMessageBox("Are you sure you want to reset count?",
                "Reset Count",
                [
                    {
                        Text: 'Yes',
                        Click: function () {
                            $('#txtCount').val('0');
                            $('.ui-dialog').dialog('close');
                        }
                    },
                    { Text: 'No' }
                ]
            );

        });

        $('#btnDoneUnitMeasure').click(function () {
            AddToList();
        });

        $('#btnDoneCart').click(function () {
            history.back();
        });

        $('#btnSaveSizesTallies-Detail').click(function () {
            var Items = [];
            var ctr = 0;

            if (CheckValuesForMultipleSizes()) {
                $('#lstSizesUnitMeasure tr td input').each(function (m_index) {
                    var data = {};
                    data.units = [];

                    if (parseInt($(this).val()) > 0) {

                        data.units.push({
                            ID: $(this).attr('ProductID'),
                            UnitMeasureID: $(this).data('data').ID,
                            ActualQuantity: parseInt($(this).data('data').Quantity),
                            Quantity: $(this).val(),
                            QuantityBad: '',
                            BadOrderReason: '',
                            AdditionalInfo: '',
                            IsResaleable: 0
                        });

                        ProductID = $(this).attr('ProductID');
                        ProductName = $(this).attr('ProductName');

                        var ReceivedBadCount = $(this).data('data').UnitMeasure + ': 0'
                        var ReceivedCount = $(this).data('data').UnitMeasure + ': ' + $(this).val() + ', '

                        AddItemToCart(data, ReceivedCount, ReceivedBadCount);
                    }
                    ctr++;
                });
            }

        });

        $('#btnUpdateBarcode').click(function () {
            if (IsMobile) {
                mobile.Scan({
                    Success: function (vData) {
                        UpdateProductBarcode(vData.Value);
                    }
                });
            } else {
                $('#panSetBarcode').popup('open');
            }
        });

        $('#btnSaveBarcodeOnline').click(function () {
            if ($('#txtBarcode').val() != '') {
                UpdateProductBarcode($('#txtBarcode').val());
            }
        });
        
        $('#btnReceiveByContinuousScanning').click(function () {
            ReceiveUsingBarcode();
            mUPCProduct = [];
            mUPCCount = 0;
        });

        $('.btnBackRecCart').click(function () {
            proj.ReceivingBackButton_Click();
        })

        $('.btnBackCount').click(function () {
            $('#pageCart').pageshow();
        })
    });


    function UpdateProductBarcode(vBarcode) {
        var mProductMeasurements = '';
        var data = { ProductID: ProductID, Barcode: vBarcode }

        if (IsMobile) {
            mProductMeasurements = App.model.ReceivingCart_offline.GetProductMeasurements(data);

            if (mProductMeasurements.Rows.length > 0) {
                App.model.ReceivingCart_offline.SaveProductBarcode(vBarcode, mProductMeasurements);
            }            
        } else {
            AjaxSend({
                url: 'Receiving/SaveProductBarcode',
                data: data,
                success: function (vData) {
                    if (vData.Results == 'True') {
                        NewMessageBox('You successfully set Barcode for this SKU');
                    } else {
                        NewMessageBox('There was a problem saving the barcode. Please try again later.');
                    }
                }
            });
        }

    }

    function CheckValuesForMultipleSizes() {
        var IsEmpty = true;
        var IsHasNegative = false;
        var isPassed = false;

        $('#lstSizesUnitMeasure tr td input').each(function (m_index) {
            if (parseInt($(this).val()) > 0) {
                IsEmpty = false;
            }
            else if (parseInt($(this).val()) < 0) {
                IsHasNegative = true;
                IsEmpty = false;
            }
        });

        if (IsEmpty) {
            NewMessageBox("Please enter a quantity.",
                            "Invalid Quantity",
                            [
                                { Text: 'Ok' }
                            ]
                        );
            return false;
        }
        else if (IsHasNegative) {
            NewMessageBox("Quantity must be a number greater than 0.",
                            "Invalid Quantity",
                            [
                                { Text: 'Ok' }
                            ]
                        );
            return false;
        }
        else {
            return true;
        }
    }

    var arrSelectedProduct = [];
    function SearchItem() {
        //arrSelectedProduct = [];
        //BrowsePopup('LibraryPIMProduct', 'Search Products',
        //                [
        //                    {
        //                        Message: 'You can add multiple products to the cart at once. Click OK when you\'re done.',
        //                        Search: $.trim($('#txtSearchItem').val()),
        //                        DisplayText: [
        //                            {
        //                                Column: function (e) {
        //                                    var lbl;

        //                                    lbl = '<b>No:</b> ' + e.ProductNo + '<br>';
        //                                    lbl += '<b>Name:</b> ' + e.ProductName;

        //                                    return lbl;
        //                                },
        //                                Label: 'Product'
        //                            },
        //                            {
        //                                Label: 'Good Count',
        //                                Style: {
        //                                    width: '100px'
        //                                },
        //                                Render: function (v1, v2, vData) {
        //                                    return '<div name="divGoodCount" />';
        //                                }
        //                            },
        //                            {
        //                                Label: 'Bad Count',
        //                                Style: {
        //                                    width: '100px'
        //                                },
        //                                Render: function (v1, v2, vData) {
        //                                    return '<div name="divBadCount" />';
        //                                }
        //                            }
        //                        ],
        //                        ShowPic: Business.SystemDefaults.ShowCondensedProductInformationInCart(),
        //                        Offline: true,
        //                        Validation: function (e) {
        //                            if (arrSelectedProduct.length == 0) {
        //                                WebToast('Please select Products');
        //                                return false;
        //                            } else {
        //                                return true;
        //                            }
        //                        },
        //                        Click: function (e) {
        //                            for (var i = 0; i < arrSelectedProduct.length; i++) {
        //                                var pUnit = Business.LibraryPIMProduct.BaseUnit(arrSelectedProduct[i].ID);
        //                                var ReceivedCount = pUnit.UnitMeasure + ': ' + CDec2(arrSelectedProduct[i].Quantity);
        //                                var ReceivedBadCount = pUnit.UnitMeasure + ': ' + CDec2(arrSelectedProduct[i].QuantityBad);
        //                                AddItemToCart({
        //                                    units: [{
        //                                        ID: arrSelectedProduct[i].ID,
        //                                        UnitMeasureID: pUnit.UnitMeasureID,
        //                                        ActualQuantity: pUnit.UnitQty, //* CInt2($(this).val()),
        //                                        Quantity: CDec2(arrSelectedProduct[i].Quantity),
        //                                        QuantityBad: CDec2(arrSelectedProduct[i].QuantityBad),
        //                                        BadOrderReason: '',
        //                                        AdditionalInfo: '',
        //                                        IsResaleable: 1,
        //                                        ProductName: arrSelectedProduct[i].ProductName,
        //                                        ProductSerials: []
        //                                    }]
        //                                }, ReceivedCount, ReceivedBadCount);
        //                            }
        //                        },
        //                        ShowOkButton: true,
        //                        AllowEmptySearch: false,
        //                        StyleFormat: 'table',
        //                        InitComplete: function (vTable) {
        //                            vTable.find('.qty').click(function (e) {
        //                                e.stopPropagation();
        //                            });
        //                        },
        //                        rowCallback: function (row, data, dataIndex) {
        //                            var goodcount = $(row).find('[name=divGoodCount]');
        //                            var badcount = $(row).find('[name=divBadCount]');
        //                            $(row).click(function (e) {
        //                                e.preventDefault();
        //                                return false;
        //                            });
        //                            goodcount.click(function (e) {
        //                                e.preventDefault();
        //                                return false;
        //                            });
        //                            badcount.click(function (e) {
        //                                e.preventDefault();
        //                                return false;
        //                            });

        //                            if (goodcount.find('input').length == 0) {
        //                                goodcount.append(AppendInputQty('good', data));
        //                            }
        //                            if (badcount.find('input').length == 0) {
        //                                badcount.append(AppendInputQty('bad', data));
        //                            }

        //                            //display current QTY of the product
        //                            $.grep(arrSelectedProduct, function (e, i) {
        //                                if (e.ID == data.ID) {
        //                                    data.Quantity = e.Quantity;
        //                                    data.QuantityBad = e.QuantityBad;
        //                                    goodcount.find('input').val(e.Quantity);
        //                                    badcount.find('input').val(e.QuantityBad);
        //                                }
        //                            })
        //                        }
        //                    }
        //                ]
        //            );
        //return;
        BrowsePopup('LibraryPIMProduct', 'Products', [
            {
                Offline: true,
                Search: $.trim($('#txtSearchItem').val()),
                DisplayText: [{ Label: 'Product No.', Column: 'ProductNo' }, { Label: 'Product Name', Column: 'ProductName' },
                    {
                        Label: 'Retail Price', Column: function (e) {
                            return AccountingFormat(e.DefaultPrice);
                        }
                    },
                    { Label: 'Published by', Column: 'PublishedBy' },
                    { Label: 'Uploaded by', Column: 'UploadedBy' }
                ],
                ShowPic: Business.SystemDefaults.ShowCondensedProductInformationInCart(),
                Options: {
                    Fields: [
                        $('<tr>')
                            .append($('<td>')
                                .append('Filter by: ')
                            )
                            .append($('<td class="pl-3">')
                                .append($('<div class="form-check">')
                                    .append($('<label class="form-check-label">')
                                        .text('All Products')
                                        .append($('<input type="radio" name="radProductType" value="all" class="form-check-input" checked>')
                                            .change(function () {
                                                BrowsePopupNow();
                                            })
                                        )
                                    )
                                )
                            )
                            .append($('<td class="pl-3">')
                                .append($('<div class="form-check">')
                                    .append($('<label class="form-check-label">')
                                        .text('Raw Products/Materials')
                                        .append($('<input type="radio" name="radProductType" value="raw" class="form-check-input">')
                                            .change(function () {
                                                BrowsePopupNow();
                                            })
                                        )
                                    )
                                )
                            )                            
                    ],
                    OnChange: function () {
                        return {
                            ProductType: $('[name="radProductType"]:checked').val()
                        }
                    }
                },
                Click: function (e) {
                    Bs.Loading('show');
                    setTimeout(function () {
                        ProductID = e.ID;
                        ProductName = e.ProductName;
                        $('#spanStockNumber').text(e.ProductNo);
                        $('#spanStockDescription').text(ProductName);

                        $('#spanStockNumberDetail').text(e.ProductNo);
                        $('#spanStockDescriptionDetail').text(ProductName);

                        $('#txtProductSerials').val('');
                        $('#lstProductSerial').empty();
                        CheckForMultipleSizes(e.ProductGroupNo);
                        DisplayProductImage(ProductID, 'imgReceivingStockInfo');
                        Bs.Loading('hide');
                    }, 200)                    
                }
            }
        ], { ProductType: $('[name="radProductType"]:checked').val(), IsInventoryCountItem: 1 })
    }

    function AppendInputQty(type, data) {
        return $('<input data-role="none" type="number" class="form-control" style="width:100%; min-width:100px;" />')
                .attr('counttype', type)
                .attr('pid', data.ID)
                .data('data', data)
                .keyup(function () {
                    var data = $(this).data('data');
                    var qty = CDec2($(this).val());
                    var index = null, type = $(this).attr('counttype');

                    $.grep(arrSelectedProduct, function (e, i) {
                        if (e.ID == data.ID) {
                            index = i;
                        }
                    })

                    if (type == 'good') {
                        data.Quantity = qty;
                    } else {
                        data.QuantityBad = qty;
                    }

                    if (index == null) {
                        arrSelectedProduct.push(data); // add
                    } else {
                        if (qty == 0) {
                            var goodqty = $('[pid="' + data.ID + '"][counttype="good"]').val();
                            var badqty = $('[pid="' + data.ID + '"][counttype="bad"]').val();

                            if (CDec2(goodqty) == 0 && CDec2(badqty) == 0) {
                                arrSelectedProduct.splice(index, 1); // remove
                            } else {
                                arrSelectedProduct[index] = data; // update
                            }
                        } else {
                            arrSelectedProduct[index] = data; // update
                        }
                    }
                })
    }

    function DisplayProductImage(ProductID, imghandler) {
        if (CBool(Business.SystemDefaults.ShowCondensedProductInformationInItemDetailsPage())) {
            AjaxSend({
                url: 'Organization/GetBulkPIMPictures',
                data: { data: [{ ID: ProductID }], size: 'medium', IsDefault: true },
                success: function (e) {
                    var vRows = e.data;
                    var sourceimg = Android.GetServer();

                    for (var i = 0; i < vRows.length; i++) {
                        if (vRows[i].Pics != '') {
                            vRows[i].Ctr = 1;
                            var pSrc = sourceimg + 'Content/MobizImagePIM/' + sessionStorage.OrgID + '/Medium/' + vRows[i].Pics[0].FileName;

                            $('#' + imghandler).attr('src', pSrc)
                                .attr('onerror', 'skupicerror(this)')
                                .attr('isimgerror', 0)
                                .attr('isimgviewer', 0)
                                .data('data', vRows[i])
                                .click(function (e) {
                                    e.preventDefault();
                                    e.stopPropagation();
                                    var data = $(this).data('data');

                                    $(this).parents().closest('li').removeClass('ui-btn-active');

                                    if (CInt2($(this).attr('isimgerror')) == 1) {
                                        NewMessageBox('Can\'t load images for SKU: <i>' + data.ProductName + '</i> or the server not responding.');
                                        return;
                                    }

                                    AjaxSend({
                                        url: 'Organization/GetBulkPIMPictures',
                                        data: { data: [data], size: 'big' },
                                        success: function (e) {
                                            vRows = e.data[0].Pics;

                                            var div = $('#divCartContent');

                                            if ($('#skupics').length == 0) {
                                                div.append($('<div id="skupics" style="display:none;">'));
                                            } else {
                                                $('#skupics').empty();
                                            }

                                            div = $('#skupics');

                                            for (var i = 0; i < vRows.length; i++) {
                                                vRows[i].Ctr = i + 1;
                                                vRows[i].ID = data.ID;
                                                vRows[i].ProductName = data.ProductName;
                                                var srcBig = sourceimg + 'Content/MobizImagePIM/' + sessionStorage.OrgID + '/Big/' + vRows[i].FileName;

                                                div.append($('<a data-role="none" data-lightbox="skupictures">')
                                                            .attr('id', 'pic_' + vRows[i].ID)
                                                            .data('data', vRows[i])
                                                            .attr('href', srcBig)
                                                            .append($('<img>')
                                                                .attr('isimgerror', 0)
                                                                .attr('isimgviewer', 1)
                                                                .attr('onerror', 'skupicerror(this)')
                                                                .data('data', vRows[i]).attr('src', srcBig)
                                                            )
                                                        )
                                            }

                                            var firstpic = $('#pic_' + vRows[0].ID);
                                            firstpic.on('click', function () {
                                                var dis = $(this);
                                                setTimeout(function () {
                                                    if (CInt2(dis.find('> img').attr('isimgerror')) == 1) {
                                                        NewMessageBox('Can\'t load images for SKU: <i>' + firstpic.data('data').ProductName + '</i> or the server not responding.');
                                                    }
                                                }, 100)
                                            })
                                            firstpic.trigger('click');
                                        }
                                    })
                                })
                        } else {
                            $('#' + imghandler).attr('src', '../Content/images/box.png');
                        }
                    }
                },
                error: function (e) {
                    Android.AddLog('Can\'t connect to the server, SKU image not successfully loaded');
                    $('#' + imghandler).attr('src', '../Content/images/box.png');
                }
            })
        }
    }

    function CheckForMultipleSizes(ProductGroupNo) {

        if (ProductGroupNo == '' || ProductGroupNo == undefined) {
            GetStockUnitMeasures(ProductID);
            return;
        }

        App.model.ReceivingCart.CheckForMultipleSizes({
            ProductGroupNo: ProductGroupNo,
            success: function (vData) {
                if (parseInt(vData.InventoryCount.Rows.length) > 1) {
                    NewMessageBox("The SKU you searched for has a product grouping.  Would you like to receive all SKUs within this product grouping?",
                            "Product",
                            [
                                {
                                    Text: 'Yes',
                                    Click: function () {
                                        DisplayProductSizes(vData.InventoryCount.Rows);
                                    }
                                },
                                {
                                    Text: 'No',
                                    Click: function () {
                                        //$('.ui-dialog').dialog('close');
                                        mInterval = setTimeout(function () {
                                            GetStockUnitMeasures(ProductID);
                                        }, 500);
                                    }
                                }
                            ]);
                }
                else {
                    GetStockUnitMeasures(ProductID);
                }
            }
        });
    }

    function DisplayProductSizes(vRows) {
        var pLV = $("#lstSizesUnitMeasure");
        var UnitMeasure = '';

        $("#tblSizesUnitMeasureFrame tbody tr").remove();

        //if (IsMobile) {
            App.model.ReceivingCart.GetStockUnitMeasures({
                ProductID: vRows[0].ID,
                success: function (vData) {
                    var pItem;
                    var vRowsUnitMeasure;
                    vRowsUnitMeasure = vData.UnitMeasure.Rows;

                    //sizes
                    for (var i = 0; i <= vRows.length - 1; i++) {
                        var m_Size = '';
                        var m_ProductID = vRows[i].ID;
                        var m_ProductName = vRows[i].ProductName;
                        //m_Size = vRows[i].ProductName.split(',');

                        //unit measure
                        for (var j = 0; j <= vRowsUnitMeasure.length - 1; j++) {
                            pItem = $('<tr>')
                                .append($('<th>')
                                    .text(vRows[i].ProductName)
                                )//td
                                .append($('<td>')
                                    .text(vRows[i].Size)
                                )//td
                                .append($('<td>')
                                    .text(vRowsUnitMeasure[j].UnitMeasure)
                                )//td
                                .append($('<td>')
                                    .text(AccountingFormat(vRowsUnitMeasure[j].Price))
                                )//td
                                .append($('<td>')
                                    .append($('<input type=number />')
                                    .attr('ID', vRowsUnitMeasure[j].ID)
                                    .attr('ProductID', m_ProductID)
                                    .attr('ProductName', m_ProductName)
                                    .data('data', vRowsUnitMeasure[j])
                                    .attr('class', 'ui-input-text ui-corner-all ui-btn-shadow ui-body-c')
                                        )
                                )//td
                        }
                        pLV.append(pItem)
                    }
                    $('#pageStockViewerDetail').pageshow();
                    $('#lstSizesUnitMeasureFrame');//.listview('refresh');

                }
            });
        //} else {

        //}
    }

    window.AddItemToCart = function (data, ReceivedCount, TotalBadOrder) {
        var pLV = $("#lstItems");

        if (data.units!=undefined){
            ProductName = data.units[0].ProductName
            ProductID = data.units[0].ID;
        } else if (data.ProductName != undefined) { //retrieving transfer out
            ProductName = data.ProductName;
            ProductID = data.ID;
        }

        $('.emptylist').remove();        
        $('#' + ProductID).remove();

        pItem = $('<li data-role="none" class="list-group-item list-group-item-action p-0">')
                    .attr('ID', ProductID)
                    .data(data)
                    .append($('<table style="width:100%;">')
                        .append($('<tr>')
                            .append($('<td style="width:100px">')
                                    .append($('<img style="width:100px;">')
                                        .attr('id', 'img-' + ProductID)
                                        .attr('src', '../Content/images/box.png')
                                    )
                                ).append($('<td style="padding:10px;">')
                                    .append($('<b>').text(ProductName))
                                    .append('<br><br>')
                                    .append('Receive Good: ' + ReceivedCount)
                                    .append('<br>')
                                    .append((ShowBadCount ? 'Receive Bad: ' + TotalBadOrder : ''))
                                    .append('<div class="divCartItemModifier" style="font-size:16px;">')
                                    .append('<div class="divItemTotal" style="font-weight:bold;">')                                    
                                    .data(data)
                                    .click(function (e) {
                                        Bs.Loading('show');
                                        var dis = $(this);
                                        setTimeout(function () {
                                            if (dis.data().ProductNo != undefined) {
                                                $('#spanStockNumber').text(dis.data().ProductNo);
                                                $('#spanStockDescription').text(dis.data().ProductName);
                                                ProductName = dis.data().ProductName;
                                            } else {
                                                $('#spanStockNumber').text(dis.data('units')[0].ProductNo);
                                                $('#spanStockDescription').text(dis.data('units')[0].ProductName);
                                                ProductName = dis.data('units')[0].ProductName;
                                            }

                                            ProductID = dis.data('units')[0].ID;
                                            GetStockUnitMeasures(dis.data('units')[0].ID);
                                            DisplayProductImage(ProductID, 'imgReceivingStockInfo');
                                            //DisplaySerialNo
                                            var ProductSerialNo = dis.data('units')[0].ProductSerials;

                                            $('#lstProductSerial').empty();

                                            for (var i = 0; i <= ProductSerialNo.length - 1; i++) {
                                                //Display
                                                AddToSerialList(ProductSerialNo[i].SerialNo);
                                            }

                                            for (var i = 0; i < dis.data('units').length; i++) {
                                                if (CDec2(dis.data('units')[i].Quantity) != 0) {
                                                    $('#txt' + dis.data('units')[i].UnitMeasureID).val(dis.data('units')[i].Quantity);
                                                }

                                                //if (CDec2(dis.data('units')[i].QuantityBad) != 0) {
                                                //    $('#txtbadorder' + dis.data('units')[i].UnitMeasureID).val(dis.data('units')[i].QuantityBad);
                                                //}
                                            }
                                            Bs.Loading('hide');
                                        }, 200)                                        
                                    })
                                ).append($('<td style="width:50px; text-align:center; font-size:20px; color:#FFF;" class="bg-danger">')
                                    .append('<span class="bi-trash"></span>')
                                    .attr('ID', 'close' + ProductID)
                                    .attr('productname', ProductName)
                                    .click(function (e) {
                                        var dis = $(this);

                                        NewMessageBox('Are you sure you want to remove <b>' + dis.attr('productname') + '</b>?', 'Confirmation Message', [
                                            {
                                                Text: 'Yes', Click: function () {
                                                    $("#" + dis.attr("ID").substr(5, 36)).remove();
                                                }
                                            }, { Text: 'No' }
                                        ])
                                    })
                                )
                        )
                    )

        pLV.append(pItem);
        console.log(data);
        try {
            pLV;//.listview('refresh');
        }
        catch (e) {
            try {
                pLV;//.listview();
            } catch (m) { console.log(m.message) }            
        }
        DisplayProductImage(ProductID, 'img-' + ProductID);
        $('#pageCart').pageshow();
    }

    window.GetStockUnitMeasures = function (m_ID, ID) {
        var data = { ProductID: m_ID, ReceivingID: ID }

        if (IsMobile) {
            App.model.ReceivingCart.GetStockUnitMeasures({
                ProductID: m_ID,
                ReceivingID: ID,
                success: function (vData) {
                    $('#pageStockViewer').pageshow();
                    DisplayUnitMeasures(vData.UnitMeasure.Rows, ID);
                }
            });
        } else {
            AjaxSend({
                url: 'Receiving/GetStockUnitMeasures',
                data: data,
                success: function (vData) {
                    $('#pageStockViewer').pageshow();
                    DisplayUnitMeasures(vData.UnitMeasure.Rows, ID);
                }
            });
        }
    }

    window.AddToList = function () {
        var data = {};
        var ctr = 0;
        var ReceivedCount = '';
        var ReceivedBadCount = '';
        data.units = [];
        var TotalSku = 0;
        var TotalBadOrderSku = 0;
        var isdecimal = false;

        $('#lstBadOrder input').each(function (m_index) {
            if (!Number.isInteger(CDec2($(this).val())) && IsNumeric($(this).val())) {
                isdecimal = true;
                return false;
            }
            TotalBadOrderSku += CDec2($(this).val());
            ReceivedBadCount = $(this).data('data').UnitMeasure + ': ' + TotalBadOrderSku;
        });

        if (TotalBadOrderSku < 0) {
            NewMessageBox("Quantity must be a number greater than 0.",
                            "Invalid Quantity",
                            [
                                { Text: 'Ok' }
                            ]
                        );
            return;
        }
        
        //get serial number
        var ProductSerials = [];
        var previousSerial = '';

        $('#lstProductSerial li').each(function () {
            if (previousSerial != $(this).data().SerialNo) {
                ProductSerials.push({ SerialNo: $(this).data().SerialNo });
            }
            previousSerial = $(this).data().SerialNo;
        });

        $('#lstUnitMeasure input').each(function (m_index) {
            if ($(this).text() != 'Receive Bad Orders') {
                if (IsNumeric($(this).val())) {
                    if (!Number.isInteger(CDec2($(this).val())) && IsNumeric($(this).val())) {
                        isdecimal = true;
                        return false;
                    }

                    if (parseInt($(this).val()) >= 0) {
                        data.units.push({
                            ID: ProductID,
                            UnitMeasureID: $(this).data("data").ID,
                            ActualQuantity: CInt2($(this).data("data").Quantity), //* CInt2($(this).val()),
                            Quantity: $(this).val(),
                            QuantityBad: (TotalBadOrderSku).toString(),
                            BadOrderReason: BadOrderReason,
                            AdditionalInfo: AdditionalInfo,
                            IsResaleable: IsResaleable,
                            ProductName: ProductName,
                            ProductSerials: ProductSerials
                        });
                        if ($(this).val() == '')
                            ReceivedCount += $(this).data('data').UnitMeasure + ': 0, '
                        else
                            ReceivedCount += $(this).data('data').UnitMeasure + ': ' + $(this).val() + ', '

                        TotalSku += CDec2($(this).val());
                    }
                    else {
                        TotalSku = -1;
                        return;
                    }
                };
            }

            ctr++;
        });

        if (isdecimal) {
            NewMessageBox('Decimal quantity is not allowed.');
            return;
        }

        if (TotalSku == 0 && TotalBadOrderSku == 0) {
            NewMessageBox("Please enter a quantity.",
                            "Invalid Quantity",
                            [
                                { Text: 'Ok' }
                            ]
                        );
            return;
        }
        else if (TotalSku < 0) {
            NewMessageBox("Quantity must be a number greater than 0.",
                            "Invalid Quantity",
                            [
                                { Text: 'Ok' }
                            ]
                        );
            return;
        }

        AddItemToCart(data, ReceivedCount, ReceivedBadCount);
    }

    function DisplayUnitMeasures(vRows, ID) {
        var pLV = $("#lstUnitMeasure");
        var pLV2 = $("#lstBadOrder");
        var BaseUnitMeasure = vRows[0].UnitMeasure;


        $("#lstUnitMeasure").empty();
        $("#lstBadOrder").empty();

        $('#selBadOrder').val(vRows[0].BadOrderReason);

        if (vRows[0].BadOrderReason == 0)
            $('#selResalableBadOrder').val("ResalableBadOrderYes");

        else
            $('#selResalableBadOrder').val("ResalableBadOrderNo");


        $('#txtAdditionalInfo').val(vRows[0].AdditionalInfo);

        for (var i = 0; i <= vRows.length - 1; i++) {
            //unit measure
            pItem = $('<div class="row mb-3">')
                        .append($('<div class="col-md-6 mb-3">')
                            .append($('<label for="txt' + vRows[i].ID + '">')
                                .text(vRows[i].UnitMeasure + ' ( ' + vRows[i].Quantity + ' ' + BaseUnitMeasure + ' ) ' + ' ( Retail Price: ' + AccountingFormat(vRows[i].Price) + ' ) ')
                            )
                        )
                        .append($('<div class="col-md-6">')
                            .append($('<div class="input-group">')
                                .append($('<button data-role="none" name="butUnitMinus" class="btn btn-danger btn-lg" type="button" style="font-size:20px;">').append($('<span class="bi-dash">'))
                                    .data('data', vRows[i])
                                    .click(function (e) {
                                        var data = $(this).data('data');
                                        var qty = $('#txt' + data.ID);

                                        if (CDec2(qty.val()) > 0) {
                                            qty.val(CDec2(qty.val()) - 1);
                                        } else {
                                            qty.val(0)
                                        }
                                    })
                                )
                                .append($('<input data-role="none" type="number" style="font-size:18px; text-align:center;" class="form-control form-control-lg" placeholder="Qty">')
                                    .attr('ID', 'txt' + vRows[i].ID)
                                    .attr('name', vRows[i].UnitMeasure)
                                    .data('data', vRows[i])
                                    .val(vRows[i].QuantityGood))
                                .append($('<button data-role="none" name="butUnitPlus" class="btn btn-success btn-lg" type="button" style="font-size:20px;">').append($('<span class="bi-plus">'))
                                    .data('data', vRows[i])
                                    .click(function (e) {
                                        var data = $(this).data('data');
                                        var qty = $('#txt' + data.ID);
                                        qty.val(CDec2(qty.val()) + 1);
                                    })
                                )
                            )
                        )
            
            pLV.append(pItem);

            //if ($('#selReceiveOrTransfer option:selected').val() == 'ReceiveItems') {
                pItemBadOrder = $('<div class="row mb-3">')
                        .append($('<div class="col-md-6 mb-3">')
                            .append($('<label for="txtbadorder' + vRows[i].ID + '">')
                                .text(vRows[i].UnitMeasure + ' ( ' + vRows[i].Quantity + ' ' + BaseUnitMeasure + ' ) ' + ' ( Retail Price: ' + AccountingFormat(vRows[i].Price) + ' ) ')
                            )
                        )
                        .append($('<div class="col-md-6">')
                            .append($('<div class="input-group">')
                                .append($('<button data-role="none" name="butUnitMinus" class="btn btn-danger btn-lg" type="button" style="font-size:20px;">').append($('<span class="bi-dash">'))
                                    .data('data', vRows[i])
                                    .click(function (e) {
                                        var data = $(this).data('data');
                                        var qty = $('#txtbadorder' + data.ID);

                                        if (CDec2(qty.val()) > 0) {
                                            qty.val(CDec2(qty.val()) - 1);
                                        } else {
                                            qty.val(0)
                                        }
                                    })
                                )
                                .append($('<input data-role="none" type="number" style="font-size:18px; text-align:center;" class="form-control form-control-lg" placeholder="Qty">')
                                    .attr('ID', 'txtbadorder' + vRows[i].ID)
                                    .attr('name', vRows[i].UnitMeasure)
                                    .data('data', vRows[i])
                                    .val(vRows[i].QuantityDamaged))
                                .append($('<button data-role="none" name="butUnitPlus" class="btn btn-success btn-lg" type="button" style="font-size:20px;">').append($('<span class="bi-plus">'))
                                    .data('data', vRows[i])
                                    .click(function (e) {
                                        var data = $(this).data('data');
                                        var qty = $('#txtbadorder' + data.ID);
                                        qty.val(CDec2(qty.val()) + 1);
                                    })
                                )
                            )
                        )
                pLV2.append(pItemBadOrder);
                $('#dvStockViewerUnitMeasure').css('display', 'block');
                $('#dvStockViewerUnitMeasure').css('visibility', 'visible');
            //} //end if
            //else {
            //    $('#dvStockViewerUnitMeasure').css('display', 'none');
            //    $('#dvStockViewerUnitMeasure').css('visibility', 'hidden');
            //}
        } //end for

    } //end function


    var mUPCProduct = [];
    var mUPCCount = 0;

    function ReceiveUsingBarcode() {
        mobile.Scan({
            Success: function (vData) {
                var isExist = false;

                for (i = 0; i < mUPCProduct.length; i++) {
                    if (mUPCProduct[i][0] == vData.Value) {
                        mUPCProduct[i][1]++;
                        isExist = true;
                    }

                };

                if (!isExist) {
                    mUPCProduct[mUPCCount] = [];
                    mUPCProduct[mUPCCount][0] = vData.Value;
                    mUPCProduct[mUPCCount][1] = 1;
                    mUPCCount++;
                }

                ReceiveUsingBarcode();
            },
            Error: function () {
                var tData = '';

                $('#reportTable').empty();

                for (i = 0; i < mUPCProduct.length; i++) {

                    var data = { barcode: mUPCProduct[i][0] };

                    mData = GetProductInformationByBarcode(data);

                    for (j = 0; j <= mData.Rows.length - 1; j++) {

                        var mData2 = '';

                        //data = { ProductID: mData.Rows[j].ID }; //para magamit ng mData2 sa query

                        data = { ProductID: mData.Rows[j].ID, ReceivingID: '' }
                        
                        mData2 = GetStockUnitMeasure(data);

                        var pData = {};
                        pData.units = [];

                        pData.units.push({
                            ID: data.ProductID,
                            ProductName: mData.Rows[j].ProductName,
                            UnitMeasureID: mData2.Rows[0].ID,
                            ActualQuantity: 1,
                            Quantity: mUPCProduct[i][1],
                            QuantityBad: 0,
                            BadOrderReason: '',
                            AdditionalInfo: '',
                            IsResaleable: 1
                        });

                        AddItemToCart(pData, mUPCProduct[i][1], 0);
                    };

                }
            }
        });
    }

    var IsAutomaticReceived = false;
    var IsShowQuantityOfStocks = false;
    
    window.DisplayReceivingSKU = function (vRows, IsAutomatic, IsShow, TransID) {

        IsAutomaticReceived = IsAutomatic;
        IsShowQuantityOfStocks = IsShow;

        var data = {};
        var ReceivedCount = '', ReceivedBadCount = '';
        var prevProdID = '';

        for (var i = 0; i <= vRows.length - 1; i++) {

            data.units = [];

            if (prevProdID != "" && prevProdID != vRows[i].ID) {
                ReceivedCount = "";
            }

            ReceivedCount += vRows[i].Label + ': ' + CDec2(vRows[i].QuantityGood) + ', ';

            ReceivedBadCount = CDec2(vRows[i].QuantityDamaged);
            
            //get product serial no
            var pSql = 'Select SerialNumber from TransReceivingStockSerials WHERE ReceivingID=' + ReadyString(TransID) + ' AND ProductID = ' + ReadyString(vRows[i].ID);
            var vSerialNo = Sql.Execute(pSql);
            var ProductSerials = [];

            if (vSerialNo.Rows.length > 0) {
                for (var j = 0; j <= vSerialNo.Rows.length-1; j++) {
                    ProductSerials.push({ SerialNo: vSerialNo.Rows[j].SerialNumber });
                }
            }

            //if (CBool(IsShowQuantityOfStocks)) {
                data.units.push({
                    ID: vRows[i].ID,
                    ProductName: vRows[i].ProductName,
                    UnitMeasureID: vRows[i].UnitMeasureGood,
                    ActualQuantity: vRows[i].ActualQuantityGood,
                    Quantity: CDec2(vRows[i].QuantityGood),
                    QuantityBad: CDec2(vRows[i].QuantityDamaged),
                    BadOrderReason: '',
                    AdditionalInfo: '',
                    IsResaleable: 0,
                    ProductSerials: ProductSerials
                });

                AddItemToCart(data, ReceivedCount, ReceivedBadCount);
                prevProdID = vRows[i].ID;

            //} else {
            //    AddItemToCart(vRows[i], '-', '-');
            //}

        }        
    }

    //for continuos scanning
    function GetStockUnitMeasure(data) {
        var pSql = '';
        var pFinder = new Finder();
        var pFields = pFinder.Fields;
        var pField;

        pFinder.Distinct = 1;

        pField = pFields.Add('ID');
        pField.Expression = 'LibraryPIMMeasure.ID';
        pField.Searchable = false;

        pField = pFields.Add('UnitMeasure');
        pField.Expression = 'LibraryCountriesLanguageLabel.Label';

        pField = pFields.Add('Quantity');
        pField.Expression = 'LibraryPIMProductMeasurement.Quantity';

        pField = pFields.Add('Price');
        pField.Expression = 'LibraryPIMproductPriceLevel.Price';

        pFinder.SqlFrom = 'FROM LibraryPIMProductMeasurement INNER JOIN ' +
                            'LibraryPIMMeasure ON LibraryPIMProductMeasurement.UnitMeasureID = LibraryPIMMeasure.ID INNER JOIN  ' +
                            'LibraryCountriesLanguageLabel ON LibraryPIMMeasure.MeasureNameID = LibraryCountriesLanguageLabel.ID  ' +
                            'inner join LibraryPIMproductPriceLevel on LibraryPIMproductPriceLevel.ProductID = LibraryPIMProductMeasurement.ProductID  ' +
                            'inner join LibraryPIMPriceLevel on LibraryPIMPriceLevel.ID = LibraryPIMproductPriceLevel.PriceLevelID'

        pFinder.SqlWhere = 'WHERE LibraryPIMProductMeasurement.ProductID = ' + ReadyString(data.ProductID) + ' and LibraryPIMPriceLevel.PriceLevel=\'Retail Price\' AND LibraryPIMproductPriceLevel.OrganizationID = ' + ReadyString(sessionStorage.OrgID)

        pField.OrderBy = ' Order by Quantity'

        return pFinder.Requery();
    }

    function GetProductInformationByBarcode(data) {
        var pSql = '';
        var pFinder = new Finder();
        var pFields = pFinder.Fields;
        var pField;

        pFinder.Distinct = 1;

        pField = pFields.Add('ID');
        pField.Expression = 'LibraryPIMProduct.ID';
        pField.Searchable = false;

        pField = pFields.Add('ProductName');
        pField.Expression = 'LibraryPIMProduct.ProductName';

        pField = pFields.Add('Size');
        pField.Expression = 'LibraryPIMProductVariableFeatures001.C0017';

        pField = pFields.Add('Price');
        pField.Expression = 'LibraryPIMproductPriceLevel.Price';

        pFinder.SqlFrom = 'FROM LibraryPIMProduct  ' +
                        'inner join LibraryPIMProductVariableFeatures001 on LibraryPIMProductVariableFeatures001.ProductID = LibraryPIMProduct.ID  ' +
                        'inner join LibraryPIMproductPriceLevel on LibraryPIMproductPriceLevel.ProductID = LibraryPIMProduct.ID  ' +
                        'inner join LibraryPIMPriceLevel on LibraryPIMPriceLevel.ID = LibraryPIMproductPriceLevel.PriceLevelID  '

        //' OR LibraryPIMProduct.ID IN ' + ***change 'OR' to 'AND'
        pFinder.SqlWhere = 'Where LibraryPIMPriceLevel.PriceLevel=\'Retail Price\' AND LibraryPIMproductPriceLevel.OrganizationID = ' + ReadyString(sessionStorage.OrgID) +
                            ' AND (LibraryPIMProduct.ID IN ' +
                            '   (SELECT LibraryPIMProductMeasurement.ProductID' +
                            '   FROM LibraryPIMProductMeasurementBarcode' +
                            '   JOIN LibraryPIMProductMeasurement ON LibraryPIMProductMeasurement.ID = LibraryPIMProductMeasurementBarcode.StockMeasurementsID' +
                            '   WHERE LibraryPIMProductMeasurementBarcode.ReadyBarcode = ' + ReadyString(ReadyBarcode(data.barcode)) + ')' +
                            '   OR productsearchstring like \' % ' + ReadyBarcode(data.barcode) + '% \' ' +
                            ")";

        return pFinder.Requery();
    }

    var SerialNoToBeRemove = '';

    function AddToSerialList(SerialNo) {
        var pLV = $('#lstProductSerial');
        var pItem;
        var data = { SerialNo: SerialNo }
     
        pItem = $('<li class="list-group-item">')
                    .data(data)
                    .attr('ID', 'lst' + SerialNo)
                    .append($('<div style="display:table; width:100%;">')
                        .append($('<div style="display:table-cell; text-align:left; vertical-align:middle;">')
                            .text(SerialNo)
                        )
                        .append($('<div style="display:table-cell; text-align:right; vertical-align:middle; width:50px;">')
                            .append($('<button data-role="none" class="btn btn-danger">')
                                .data(data)
                                .append($('<span class="bi-trash">'))
                                .click(function (e) {
                                    e.preventDefault();
                                    e.stopPropagation();
                                    SerialNoToBeRemove = $(this).data().SerialNo;

                                    NewMessageBox("Remove this record?",
                                        'Remove',
                                        [
                                            {
                                                Text: 'Yes',
                                                Click: function () {
                                                    $('#lst' + SerialNoToBeRemove).remove();
                                                }
                                            }
                                            ,
                                            {
                                                Text: 'No'
                                            }
                                        ]
                                    );
                                })
                            )
                        )
                    )

        pLV.append(pItem);

        pLV;//.listview('refresh');

        $('#txtProductSerials').val('');
        SerialNoToBeRemove = '';
    }
});
