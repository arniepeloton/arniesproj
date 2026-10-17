require([
    'common', GetPathFormat('/Scripts3/Browser/LibraryPIMProduct.js?crc=')
    ], function () {

        $(function () {

            $('body').on('submit', '#frmPIMProductPriceBrowser', function (e) {
                e.preventDefault();
                SearchPIMPopUp($('#txtPIMProductPriceSearch').val());
                $('#txtPIMProductPriceSearch').val('');
            })
            .on('click', '#btnPIMPriceLevel', function (e) {
                SearchPriceLvlPopUp();
            })
            .on('keypress', '#txtSearchKeyword', function (e) {
                if (e.keyCode == 13) {
                    if ($('#TitleLibrary').text() == 'Price Level') {
                        SearchPriceLvlPopUp($(this).val());
                    }
                    else {
                        SearchPIMPopUp($(this).val());
                    }
                }
            })
            .on('click', '#btnPIMProductPriceSave', function (e) {
                var pResult = false;

                if ($.trim($("#txtPIMPriceLevelID").val()) == "") {
                }

                $('[name=txtProductPrice]').each(function (vItem, vIndex) {
                    var pThis = $(this);
                    var pVal = pThis.val();

                    if (CStr2(pVal).trim().length <= 0) {
                        NewMessageBox('Please enter product price', 'Invalid Price');
                        pResult = false;
                        return false;
                    }
                    else {
                        pResult = true;
                    }

                    if ((CStr2(pVal).trim().length > 0) && (!IsNumeric(pVal))) {
                        NewMessageBox('Please enter a valid price', 'Invalid Price');
                        pResult = false;
                        return false;
                    }
                    else {
                        pResult = true;
                    }

                    if (CDec2(pVal) < 0) {
                        NewMessageBox('Please enter a valid price', 'Invalid Price');
                        pResult = false;
                        return false;
                    }
                    else {
                        pResult = true;
                    }
                })

                if (pResult == true) {
                    SavePIMProductPrice();
                }

            })
            .on('click', '#btnPIMProductPriceCancel', function (e) {
                $('#txtPIMPriceLevelID').val('')
                $('#btnPIMPriceLevel').text('Please Select').button('refresh');

                //$.mobile.changePage("#pagePIMProductPriceList");
                $('#pagePIMProductPriceList').show();
                $('#pagePIMProductPriceInfo').hide();
            })
            .on('click', '#btnPIMProductPriceBackToHome', function (e) {
                //window.open(GetPathFormat("/People/PeopleMenu#pagePopUpMenuAdjustPrice"), "_self");
                window.location.hash = 'LibraryPage';
            })

            function SearchPIMPopUp(s) {
                BrowsePopup('LibraryPIMProduct', 'Search Product', [
                    {
                        Search: s || '',
                        DisplayText: [{
                            Column: function (e) {
                                return e.ProductNo + ' ' + e.ProductName
                            }
                        }, { Label: 'Published by', Column: 'PublishedBy' }, { Label: 'Uploaded by', Column: 'UploadedBy' }],
                        Click: function (e) {
                            LoadProductPrices(e.ID, '');
                        }
                    }
                ])
            }

            function SearchPriceLvlPopUp(s) {
                BrowsePopup('LibraryPIMPriceLevel', 'Search Price Level', [
                    {
                        Search: s || '',
                        DisplayText: [{
                            Column: 'PriceLevel'
                        }],
                        Click: function (e) {
                            $('#txtPIMPriceLevelID').val(e.ID);
                            $('#btnPIMPriceLevel').text(e.PriceLevel);

                            LoadProductPrices($('#txtPIMProductID').val(), e.ID)
                        }
                    }
                ])
            }

            function ShowPIMProduct() {

                var pData = {};
                pData.SearchString = $("#txtPIMProductPriceSearch").val();

                AjaxSend({
                    url: 'PIMProduct/getProductList',
                    data: pData,
                    success: function (vData) {
                        LoadPIMProductList(vData.PIMProductData.Rows);
                    }
                });

            }

            function LoadPIMProductList(vRows) {
                var pLV = $('#lstPIMProductPrice');
                var pItem;
                var pDivA;
                var pDivB;
                var pDivC;

                $("#divPIMProductPriceSearchResults").hide();
                $("#divPIMProductPriceSearchNoResults").hide();

                if (vRows.length > 0) {
                    $("#divPIMProductPriceSearchResults").show();
                    $("#divPIMProductPriceRecordCount").text(" Number of record(s) found: " + vRows.length);

                    pLV.html('');
                    for (var p = 0; p <= vRows.length - 1; p++) {

                        pDivA = $('<div>')
                            .addClass('ui-block-a')
                            .append($('<img>')
                                .css({
                                    'max-width': '80px',
                                    'max-height': '80px',
                                    'margin-right': '9px'
                                })
                                .attr('src', 'Content/images/box.png')
                             );

                        if (CDec2(vRows[p].IsBrandOwner) != 0) {
                            pDivB = $('<div>')
                            .addClass('ui-block-b')
                            .append($('<img>')
                                .css({
                                    'max-width': '80px',
                                    'max-height': '80px',
                                    'margin-right': '9px'
                                })
                                .attr('src', 'Content/images/OfficialItem.gif')
                             );
                        }

                        pDivC = $('<div>')
                            .addClass('ui-block-c')
                            .append($('<h3>')
                                .css('white-space', 'normal')
                                .text(vRows[p].ProductNo + ' ' + vRows[p].ProductName)
                            )
                            .append($('<p>')
                                .css('white-space', 'normal')
                                .text('Published by: ' + vRows[p].PublishedBy + (CDec2(vRows[p].IsBrandOwner) != 0 ? ' (Brand Owner)' : ''))
                            )
                            .append($('<p>')
                                .css('white-space', 'normal')
                                .text('Uploaded by: ' + vRows[p].UploadedBy)
                            );

                        pItem = $('<li>')
                            .append($('<a>')
                                .attr('ID', vRows[p].ID)
                                .click(function (e) {
                                    e.preventDefault();
                                    LoadProductPrices($(this).attr('ID'), '');
                                })
                                .append(pDivA)
                                .append(pDivB)
                                .append(pDivC)
                            );

                        pLV.append(pItem);

                    };

                    pLV.listview('refresh');
                }
                else {
                    $("#divPIMProductPriceSearchNoResults").show();
                    $("#lblPIMProductPriceNotFound").text('The product "' + $("#txtPIMProductPriceSearch").val() + '" does not exist. Please consider to search thoroughly using an alternate spelling before creating a new product to avoid duplication.');
                }
            };

            function LoadProductPrices(strID, strPriceLevel) {
                var pData = {};
                pData.PIMProductID = strID;
                pData.PIMPriceLevelID = strPriceLevel;

                AjaxSend({
                    url: 'PIMProductPrice/getProductPriceByStyleNumberColorCodeList',
                    data: pData,
                    success: function (vData) {

                        //$.mobile.changePage("#pagePIMProductPriceInfo");
                        $('#pagePIMProductPriceList').hide();
                        $('#pagePIMProductPriceInfo').show();

                        $("#txtPIMProductID").val(strID);
                        if (vData.PIMProductPriceData.Rows.length > 0) {
                            $("#txtPIMProduct").val(vData.PIMProductPriceData.Rows[0].ProductName);
                            //                            $("#txtPIMPriceLevelID").val(vData.PIMProductPriceData.Rows[0].PriceLevelID);
                            //                            $("#btnPIMPriceLevel").text(vData.PIMProductPriceData.Rows[0].PriceLevel).button('refresh');
                        }

                        $('#divProductPrice').html('');
                        LoadPIMProductPriceList(0, vData.PIMProductPriceData.Rows);
                    }
                });

            }

            function LoadPIMProductPriceList(intCurIndex, vRows) {
                var pLV = $('#divProductPrice');
                var pItem;

                if (intCurIndex > vRows.length - 1) {
                    return;
                }

                if (intCurIndex == 0) {
                    pLV.html('');
                }

                if (vRows.length > 0) {

                    var vObjName = vRows[intCurIndex].ProductID;

                    pItem = $('<div class="row">')
                        .append($('<div class="offset-2 col-sm-10">')
                            .append($('<input class="form-control">')
                                .attr('id', "txtProductPrice_" + vObjName)
                                .attr('name', "txtProductPrice")
                                //.attr('type', "text")
                                .keydown(function (e) {
                                    if (e.keyCode != 110 && e.keyCode != 190) {
                                        if (!NumberOnly(e)) return;
                                    }
                                })
                                .data('ProductData', vRows[intCurIndex])
                            )
                        );

                    pLV.append(pItem);

                    //pLV.trigger("create");

                    $("#txtProductPrice_" + vObjName).val(AccountingFormat(vRows[intCurIndex].Price || '0.00'));

                    LoadPIMProductPriceList(intCurIndex + 1, vRows);

                }

            };


            function SavePIMProductPrice() {
                var pData = [];
                var pDataProductPrice = [];

                var i = 0;
                $('[name=txtProductPrice]').each(function (vItem, vIndex) {
                    var pThis = $(this);
                    var pVal = pThis.val();
                    var pThisData = pThis.data('ProductData');

                    pDataProductPrice[i] = {
                        ID: CStr2(pThisData.ID) == '' ? '' : pThisData.ID,
                        ProductID: pThisData.ProductID,
                        PriceLevelID: $.trim($("#txtPIMPriceLevelID").val()),
                        Price: CDec2(pVal)
                    };

                    i = i + 1;
                })

                pData = {
                    ProductPrice: pDataProductPrice
                };

                AjaxSend({
                    url: "PIMProductPrice/SaveProductPriceInfo",
                    data: pData,
                    success: function (vSaveData) {
                        NewMessageBox('Product prices has been saved successfully', 'Product Price');
                        $('#txtPIMPriceLevelID').val('')
                        $('#btnPIMPriceLevel').text('Please Select').button('refresh');

                        //$.mobile.changePage("#pagePIMProductPriceList");
                        $('#pagePIMProductPriceList').show();
                        $('#pagePIMProductPriceInfo').hide();
                    }
                });
            };
        });
    });