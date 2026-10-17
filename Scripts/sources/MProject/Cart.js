require([
    'common',
    '/Scripts3/Browser/LibraryPIMProduct.js',
    'promise!/Scripts3/model/ReceivingCart.js?crc='

], function () {
    var ctrItem = 0;
    var ProductID = '';
    var ProductName = '';
    var UnitMeasureValue = "";
    var BadOrderReason = "";
    var AdditionalInfo = "";
    var IsResaleable = 0;
    var selectedItem = '';

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
                SearchItem();
            }
        });

        $('#btnSearchItem').click(function (e) {
            SearchItem();
        });

        $('#btnPlusOne').click(function () {
            $('#txtCount').val((parseInt($('#txtCount').val()) + 1).toString())
        });

        $('#btnMinusOne').click(function () {
            $('#txtCount').val((parseInt($('#txtCount').val()) - 1).toString())
        });

        $('#btnBackCounter').click(function () {
            if (parseInt($('#txtCount').val()) < 0) {
                MessageBox('Quantity must be a number greater than 0.',
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
            MessageBox("Are you sure you want to reset count?",
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
                        MessageBox('You successfully set Barcode for this SKU');
                    } else {
                        MessageBox('There was a problem saving the barcode. Please try again later.');
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
            MessageBox("Please enter a quantity.",
                            "Invalid Quantity",
                            [
                                { Text: 'Ok' }
                            ]
                        );
            return false;
        }
        else if (IsHasNegative) {
            MessageBox("Quantity must be a number greater than 0.",
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
    function SearchItem() {
        
        //Browser.OfflineMode = true;
        Browser.LibraryPIMProduct.AfterClick = function (e) {
            ProductID = e.ID;
            ProductName = e.ProductName;
            $('#spanStockNumber').text(e.ProductNo);
            $('#spanStockDescription').text(ProductName);

            $('#spanStockNumberDetail').text(e.ProductNo);
            $('#spanStockDescriptionDetail').text(ProductName);

            CheckForMultipleSizes(e.ProductGroupNo);
        };

        Browser.LibraryPIMProduct.Show($('#txtSearchItem').val());
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
                    MessageBox("The SKU you searched for has a product grouping.  Would you like to receive all SKUs within this product grouping?",
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
                                        $('.ui-dialog').dialog('close');
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
                    $.mobile.changePage('#pageStockViewerDetail');
                    $('#lstSizesUnitMeasureFrame').listview('refresh');

                }
            });
        //} else {

        //}
    }

    function AddItemToCart(data, ReceivedCount, TotalBadOrder) {
        var pLV = $("#lstItems");

        if (data.units!=undefined){
            ProductName = data.units[0].ProductName
            ProductID = data.units[0].ID;
        } else if (data.ProductName != undefined) { //retrieving transfer out
            ProductName = data.ProductName;
            ProductID = data.ID;
        }
        
        $('#' + ProductID).remove();

        pItem = $('<li>')
                .attr('ID', ProductID)
                .data(data)
                .append($('<a>')
                    .append($('<div>')
                        .css('display', 'table')
                        .append($('<div>')
                            .css({
                                display: 'table-cell',
                                verticalAlign: 'middle',
                                width: '80px'
                            })
                            .append($('<img>')
                                .attr('src', '/Content/images/box.png')
                                .css({
                                    'max-width': '80px',
                                    'max-height': '80px',
                                    'margin-right': '9px'
                                })
                            )
                        )
                        .append($('<div>')
                            .css({
                                display: 'table-cell',
                                verticalAlign: 'top'
                            })
                            .append($('<h3>')
                                .css('white-space', 'normal')
                                .text(ProductName)
                            )
                            .append($('<p>')
                                .css('white-space', 'normal')
                                .css('font-size', '14px')
                                .text('Receive Good: ' + ReceivedCount)
                            )
                            .append($('<p>')
                                .css('white-space', 'normal')
                                .css('font-size', '14px')
                                .text('Receive Bad: ' + TotalBadOrder)
                            )
                        )
                    )                    
                    .attr('href', '#')
                    .attr('ID', ProductID)
                    .data(data)
                    .click(function (e) {

                        if ($(this).data().ProductNo != undefined) {
                            $('#spanStockNumber').text($(this).data().ProductNo);
                            $('#spanStockDescription').text($(this).data().ProductName);
                            ProductName = $(this).data().ProductName;
                        } else {
                            $('#spanStockNumber').text($(this).data('units')[0].ProductNo);
                            $('#spanStockDescription').text($(this).data('units')[0].ProductName);
                            ProductName = $(this).data('units')[0].ProductName;
                        }
                        
                        ProductID = $(this).attr('ID');
                        GetStockUnitMeasures($(this).attr("ID"))
                    })                    
        //                        .append($('<h4>')
        //                            .text('Receive Bad' + TotalBadOrder)
        //                        )
                )
                .append($('<a>')
                    .attr('href', '#')
                    .attr('ID', 'close' + ProductID)
                    .attr('data-split-icon', 'minus')
                    .click(function () {
                        $(this).remove();
                        $("#" + $(this).attr("ID").substr(5, 36)).remove();
                        //$("#" + $(this).attr("ID")).hide();
                    })
                );

        pLV.append(pItem);

        try {
            pLV.listview('refresh');
        }
        catch (e) {
            pLV.listview();
        }

        $.mobile.changePage('#pageCart');
    }

    window.GetStockUnitMeasures = function (m_ID, ID) {
        var data = { ProductID: m_ID, ReceivingID: ID }

        if (IsMobile) {
            App.model.ReceivingCart.GetStockUnitMeasures({
                ProductID: m_ID,
                ReceivingID: ID,
                success: function (vData) {
                    $.mobile.changePage('#pageStockViewer');
                    DisplayUnitMeasures(vData.UnitMeasure.Rows, ID);
                }
            });
        } else {
            AjaxSend({
                url: 'Receiving/GetStockUnitMeasures',
                data: data,
                success: function (vData) {
                    $.mobile.changePage('#pageStockViewer');
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

        $('#lstBadOrder tr td input').each(function (m_index) {
            TotalBadOrderSku += CDec2($(this).val());
            ReceivedBadCount = $(this).data('data').UnitMeasure + ': ' + TotalBadOrderSku;
        });

        if (TotalBadOrderSku < 0) {
            MessageBox("Quantity must be a number greater than 0.",
                            "Invalid Quantity",
                            [
                                { Text: 'Ok' }
                            ]
                        );
            return;
        }
        
        $('#lstUnitMeasure tr td input').each(function (m_index) {
            if ($(this).text() != 'Receive Bad Orders') {
                if (IsNumeric($(this).val())) {
                    if (parseInt($(this).val()) >= 0) {
                        data.units.push({
                            ID: ProductID,
                            UnitMeasureID: $(this).data("data").ID,
                            ActualQuantity: $(this).data("data").Quantity,
                            Quantity: $(this).val(),
                            QuantityBad: (TotalBadOrderSku).toString(),
                            BadOrderReason: BadOrderReason,
                            AdditionalInfo: AdditionalInfo,
                            IsResaleable: IsResaleable,
                            ProductName: ProductName
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

        if (TotalSku == 0 && TotalBadOrderSku == 0) {
            MessageBox("Please enter a quantity.",
                            "Invalid Quantity",
                            [
                                { Text: 'Ok' }
                            ]
                        );
            return;
        }
        else if (TotalSku < 0) {
            MessageBox("Quantity must be a number greater than 0.",
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


        $("#lstUnitMeasure tbody tr").remove();
        $("#lstBadOrder tbody tr").remove();

        $('#selBadOrder').val(vRows[0].BadOrderReason);

        if (vRows[0].BadOrderReason == 0)
            $('#selResalableBadOrder').val("ResalableBadOrderYes");

        else
            $('#selResalableBadOrder').val("ResalableBadOrderNo");


        $('#txtAdditionalInfo').val(vRows[0].AdditionalInfo);

        for (var i = 0; i <= vRows.length - 1; i++) {
            //unit measure
            pItem = $('<tr>')
                        .append($('<td>')
                            .append($('<label for=' + vRows[i].UnitMeasure + '>')
                            .text(vRows[i].UnitMeasure + ' ( ' + vRows[i].Quantity + ' ' + BaseUnitMeasure + ' ) ' + ' ( Retail Price: ' + AccountingFormat(vRows[i].Price) + ' ) ')
                                )
                        )//td
                        .append($('<td>')
                            .append($('<input type=number />')
                            .css('width', '50%')
                            .attr('ID', 'txt' + vRows[i].ID)
                            .attr('name', vRows[i].UnitMeasure)
                            .data('data', vRows[i])
                            .attr('class', 'ui-input-text ui-shadow-inset ui-corner-all ui-btn-shadow ui-body-c')
                            .val(vRows[i].QuantityGood)
                                )
                        )//td
                        .append($('<td>')
                            .append($('<a>')
                                .attr('data-role', 'button')
                                .attr('ID', vRows[i].ID)
                                .attr('href', '#pageCounter')
                                .click(function (e) {
                                    UnitMeasureValue = $(this).attr('ID')
                                    if ($('#txt' + $(this).attr('ID')).val() == '') {
                                        $('#txtCount').val('0')
                                    }
                                    else {
                                        $('#txtCount').val($('#txt' + $(this).attr('ID')).val())
                                    }
                                })
                                .append($('<h5>')
                                    .text('Tap Count')
                                )
                                .attr('class', 'ui-btn ui-shadow ui-btn-corner-all ui-btn-up-d')
                            )
                        )//td
            pLV.append(pItem);

            if ($('#selReceiveOrTransfer option:selected').val() == 'ReceiveItems') {
                pItemBadOrder = $('<tr>')
                        .append($('<td>')
                            .append($('<label for=' + vRows[i].UnitMeasure + '>')
                            .text(vRows[i].UnitMeasure + ' ( ' + vRows[i].Quantity + ' ' + BaseUnitMeasure + ' ) ' + ' ( Retail Price: ' + AccountingFormat(vRows[i].Price) + ' ) ')
                                )
                        )//td
                        .append($('<td>')
                            .append($('<input type=number />')
                            .css('width', '50%')
                            .attr('ID', 'txtbadorder' + vRows[i].ID)
                            .attr('name', vRows[i].UnitMeasure)
                            .data('data', vRows[i])
                            .attr('class', 'ui-input-text ui-corner-all ui-btn-shadow ui-body-c')
                            .val(vRows[i].QuantityDamaged)
                                )
                        )//td
                        .append($('<td>')
                            .append($('<a>')
                                .attr('data-role', 'button')
                                .attr('ID', vRows[i].ID)
                                .attr('href', '#pageCounter')
                                .click(function (e) {
                                    UnitMeasureValue = 'badorder' + $(this).attr('ID')
                                    if ($('#txtbadorder' + $(this).attr('ID')).val() == '') {
                                        $('#txtCount').val('0')
                                    }
                                    else {
                                        $('#txtCount').val($('#txtbadorder' + $(this).attr('ID')).val())
                                    }
                                })
                                .append($('<h5>')
                                    .text('Tap Count')
                                )

                                .attr('class', 'ui-btn ui-shadow ui-btn-corner-all ui-btn-up-d')
                            )
                        )//td
                pLV2.append(pItemBadOrder);
                $('#dvStockViewerUnitMeasure').css('display', 'block');
                $('#dvStockViewerUnitMeasure').css('visibility', 'visible');
            } //end if
            else {
                $('#dvStockViewerUnitMeasure').css('display', 'none');
                $('#dvStockViewerUnitMeasure').css('visibility', 'hidden');
            }

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
    
    window.DisplayReceivingSKU = function (vRows, IsAutomatic, IsShow) {

        IsAutomaticReceived = IsAutomatic;
        IsShowQuantityOfStocks = IsShow;

        var data = {};

        for (var i = 0; i <= vRows.length - 1; i++) {

            data.units = [];
            
            //if (CBool(IsShowQuantityOfStocks)) {
                data.units.push({
                    ID: vRows[i].ID,
                    ProductName: vRows[i].ProductName,
                    UnitMeasureID: vRows[i].UnitMeasureGood,
                    ActualQuantity: vRows[i].ActualQuantityGood,
                    Quantity: vRows[i].QuantityGood,
                    QuantityBad: '',
                    BadOrderReason: '',
                    AdditionalInfo: '',
                    IsResaleable: 0
                });

                AddItemToCart(data, vRows[i].QuantityGood, '-');

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
    };

});
