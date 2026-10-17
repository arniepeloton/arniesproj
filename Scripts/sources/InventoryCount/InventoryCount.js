require([
    'common',
    //GetPathFormat('/Scripts/jquery.dataTables.min.js?crc='), commented by arnie 51717 1034am kay giload na daan sa NewMessageBox.js ug dili pud mogana ang fxs sa nmsgbox.js
    GetPathFormat('/Scripts3/Browser/LibraryFowst.js?crc='),
    GetPathFormat('/Scripts3/Browser/LibraryFowstBin.js?crc='),
    GetPathFormat('/Scripts3/Browser/LibraryFowstBinStock.js?crc='),
    GetPathFormat('/Scripts3/Browser/LibraryPIMProduct.js?crc='),
    GetPathFormat('/Scripts3/Browser/LibraryOrganization.js?crc='),
    GetPathFormat('/Scripts3/model/InventoryCount_Offline.js?crc='),
    GetPathFormat('/Scripts3/Browser/LibraryPeople.js?crc='),   
    '/Scripts/sources/SalesPayments/SalesPaymentsExport_offline.js?crc=',
    '/Scripts/sources/SignIn/SignInInfo.js?crc='
    , 'promise!/Scripts3/NewMessageBox.js?crc='
    
    ], function () {

        var OrgID = '';
        var FowstName = '';
        var FowstID = '';
        var FowstBinID = '';
        var ProductID = '';
        var mAllowDecimal;
        var IsGoodStock = 1;
        var IsWithExpiration = 0;
        var ID = "";
        var IsNew = 1;
        var IsAdvanceView = 0;
        var GroupSelected = '';
        var CustomerID = '';
        var IsCustomer = 0;

        var InventoryCount = {};

        var ProductEndingBalance;
        var TurnOverToID;
        var CategoryType = '';
        var ProductFilter = 'optNoFilter';
        var SelectedProduct;

        var IsEmployeeVerified = false;

        var BinsTable = $('#tblNamedBins');
        var binDatatable;
        var BinID;
        var BinName;
        var AreaID;
        var AreaName;
        var PriceLevelID;
        var FowstBinProductTable = $('#tblShelfPrices');
        var fowstbinproductDatatable;

        var IsBinActive=false;
        var ProductTable = $('#tblBinProducts');
        var productDatatable;
        var productTableSelectedRow;
        var productQty;

        var ProductUnitMeasureTable = $('#tblProductBinUnitMeasures');
        var productUnitMeasureDatatable;

        var ProductBarcodeTable = $('#tblProductBarcode');
        var productBarcodeDatatable;

        var temp;
        var ProductsInCategoriesRows;
        var ProductsInCategoriesDatatable;
        var ProductsInCategoriesTable = $('#tblProductsInCategories');


        InventoryCount = new App.model.InventoryCount_Offline();

        require([(OfflineMode ? '/Scripts3/Offline/Browser/LibraryFowst.js?crc=' : '')], function () {
            $(function () {
                IsNew = 1;

                ClearFields();

                $('[data-role="DateCombo"]').DateCombo()

                if (FowstBinID != '' && FowstBinID != undefined) {
                    GetFowstBinProductMaxMinCap();
                }

                if (OfflineMode) {
                    vData = Business.LibraryOrganizationFOWST.GetData()
                    DisplayFowst(vData);                    
                }
                else {
                    GetFowst();
                }
                
                $('#pageSpatial').bind('pageshow', PageShow)
                $('#pageStockViewer').bind('pageshow', SetStockViewerBin)
                $('#Stock').bind('pageshow', SetStockPage)
                $('#pageStockViewerDetail').bind('pageshow', SetStockViewerBin)
                $('#pageGroup').bind('pageshow', SetPageGroup)
                $('#PageCountByUPC').bind('pageshow', PageShow)
                                
                //===========02/06/17
                
                if (window.location.hash == '#pageCountByBinAndArea') {
                    $('#pageCountByBin').css('visibility', 'visible');
                    $('#pageCountByBin').css('display', 'block');

                    $('#pageCountByArea').css('visibility', 'hidden');
                    $('#pageCountByArea').css('display', 'none');

                    if (CBool(sessionStorage.IsCountByArea)) {
                        SetByAreaSettings();
                    } else {
                        SetByBinSettings();
                    }
                }


                $('body').on('click', '#tblNamedBins tbody tr td', function () {

                    var mData = [];

                    if (binDatatable.data().count() > 0) {
                        BinID = binDatatable.row(this).data().ID;
                        BinName = binDatatable.row(this).data().Name;
                        IsBinActive = CBool(binDatatable.row(this).data().IsActive);

                        var mData = [];

                        mData.push({
                            Text: 'Deactivate Bin Address',
                            Click: function () {
                                NewMessageBox("This feature is under development. Please use \'Shelf and Bin Labeling -> Bin Address Tags\' module using browser to deactivate bin.", "Manully Add Bin Tags",
                                                            [
                                                                { Text: 'Ok' }
                                                            ]);
                            }
                        });

                        mData.push({
                            Text: 'See Products Mapped to this Bin',
                            Click: function () {
                                getAllProductsInBin();
                                $('#popProductBinCount').show();
                                $('#lblProductBinMappingLabel').text('Bin ' + BinName);
                                if (IsBinActive) {
                                    $('#btnAddProduct').css('visibility', 'visible');
                                    $('#btnSaveObservedCountByBin').css('visibility', 'visible');
                                } else {
                                    $('#btnAddProduct').css('visibility', 'hidden');
                                    $('#btnSaveObservedCountByBin').css('visibility', 'hidden');
                                }
                            }
                        });

                        PopupButtonList('', 'Bin ' + BinName,
                            mData, { AutoDismiss: true })

                        
                    }

                }).on('click', '#tblBinProducts tbody tr td', function () {

                    var mData = [];

                    if (productDatatable.data().count() > 0) {

                        if (productDatatable.cell(this).index().columnVisible == 3) {
                            return;
                        }
                                                
                        ProductID = productDatatable.row(this).data().ProductID;
                        productTableSelectedRow = productDatatable.row(this).index();
                        productQty = $(this).closest('tr').find('input').val();

                        if (IsBinActive) {

                            $('#txtMaxCap').val(productDatatable.row(this).data().MaxPerProduct);
                            $('#txtReorderPoint').val(productDatatable.row(this).data().ReorderPoint);
                            $('#txtReorderQty').val(productDatatable.row(this).data().ReorderQty);
                                                        
                            if (productDatatable.cell(this).index().columnVisible == 2) {
                                getAllProductsUnitMeasuresForBarcode();
                                $('#popProductBarcode').show();
                            } else {
                                getAllProductsInBinUnitMeasures();
                                $('#popProductBinUnitMeasures').show();
                            }
                            
                            $('#lblProductBinUnitMeasuresLabel').text(productDatatable.row(this).data().ProductName);
                        }                        
                    }

                }).on('click', '#btnDoneProductBinUnitMeasures', function () {

                    var TotalBaseBalance;
                    var UnitMeasureID
                    if (productUnitMeasureDatatable.data().count() > 0) {

                        $('#tblProductBinUnitMeasures tbody tr td input').each(function () {

                            if (productUnitMeasureDatatable.row($(this).closest('tr')).data().Quantity == 1) {
                                UnitMeasureID = productUnitMeasureDatatable.row($(this).closest('tr')).data().ID
                            }

                            if ($(this).val() != '') {
                                TotalBaseBalance = CDec2(TotalBaseBalance) + (CDec2($(this).val()) * CDec2(productUnitMeasureDatatable.row($(this).closest('tr')).data().Quantity));
                            }
                        })

                        if (TotalBaseBalance != undefined) {
                            if (CBool(sessionStorage.IsCountByArea)) {
                                var Count = [];

                                var data = {
                                    ID: ID,
                                    IsNew: IsNew,
                                    FowstID: FowstID,
                                    ProductID: ProductID,
                                    Count: Count
                                }

                                Count[0] = {
                                    BinID: BinID,
                                    ExpirationDate: 'N/A',
                                    UnitMeasureID: UnitMeasureID,
                                    Quantity: TotalBaseBalance,
                                    ActualQuantity: 1,
                                    IsGoodStock: 1
                                };

                                if (OfflineMode) {
                                    IsSuccessful = InventoryCount.SaveInventoryCount(data);                                    
                                }

                            } else {
                                $('#tblBinProducts tbody tr td input').each(function () {
                                    if (productDatatable.row($(this).closest('tr')).index() == productTableSelectedRow) {
                                        $(this).val(TotalBaseBalance);
                                    }
                                })
                            }
                        }

                        SaveReorderPoint();
                    }

                    $('#popProductBinUnitMeasures').hide();

                }).on('click', '#tblProductBarcode tbody tr td', function () {

                    if (productBarcodeDatatable.data().count() > 0) {
                        if (productBarcodeDatatable.cell(this).index().columnVisible == 4) {
                            temp = $(this).closest('tr');
                            mobile.Scan({
                                Success: function (vData) {
                                    temp.find('input').val(vData.Value);
                                }
                            });
                        }
                    }
                    
                }).on('click', '#btnDoneProductBarcode', function () {

                    var TotalBaseBalance;
                    var UnitMeasureID
                    if (productBarcodeDatatable.data().count() > 0) {

                        $('#tblProductBarcode tbody tr td input').each(function () {

                            if ($(this).val() != '') {                                
                                InventoryCount.SaveProductBarcode($(this).val(), productBarcodeDatatable.row($(this).closest('tr')).data().PIMProductMeasurementID);
                            }
                        })
                    }

                    $('#popProductBarcode').hide();

                }).on('click', '#tblShelfPrices tbody tr td', function () {

                    var mData = [];

                    if (fowstbinproductDatatable.data().count() > 0) {

                        if (fowstbinproductDatatable.cell(this).index().columnVisible == 2) {
                            return;
                        }

                        ProductID = fowstbinproductDatatable.row(this).data().ProductID;
                        productTableSelectedRow = fowstbinproductDatatable.row(this).index();
                        BinID = fowstbinproductDatatable.row(this).data().ID;
                        //if (IsBinActive) {

                            $('#txtMaxCap').val(fowstbinproductDatatable.row(this).data().MaxPerProduct);
                            $('#txtReorderPoint').val(fowstbinproductDatatable.row(this).data().ReorderPoint);
                            $('#txtReorderQty').val(fowstbinproductDatatable.row(this).data().ReorderQty);

                            getAllProductsInBinUnitMeasures();
                            $('#popProductBinUnitMeasures').show();
                            $('#lblProductBinUnitMeasuresLabel').text(fowstbinproductDatatable.row(this).data().ProductName);
                        //}
                    }

                })                                   

                $('#btnSaveObservedCountByBin').click(function () {
                    SaveObservedCountByBin();
                })

                $('#lblWorkFowstName').click(function () {
                    DisplayFowstPopUp();
                })

                $('#btnAddProduct').click(function () {
                    GetProducts();
                });

                $('#btnManuallyAddBinTags').click(function () {
                    $('#popAddBinAddress').show();
                })

                $('#btnAddBinAddress').click(function () {
                    var data;
                    var pSql;
                    var mData;

                    if ($('#txtBinAddress').val() == '') {
                        NewMessageBox('Please enter Bin Address', 'Bin Address')
                    }

                    pSql = 'Select * from private.LibraryFowstBin where FowstID=' + ReadyString(sessionStorage.FowstID) + ' AND Name = ' + ReadyString($('#txtBinAddress').val())
                    mData = Sql.Execute(pSql);

                    if (mData.Rows.length == 0) {
                        data = {
                            ID: NewID(),
                            Name: $('#txtBinAddress').val(),
                            Description: $('#txtBinRemarks').val()
                        }

                        InventoryCount.SaveBin(data);

                        $('#popAddBinAddress').hide();

                        GetNamedFowstBin();
                    } else {
                        NewMessageBox('Bin Address already exist, please create a new bin address.', 'Bin Address')
                    }
                });

                $('#btnFilterBinList').click(function () {
                    PopupButtonList('', 'Filter Bin List', [
                        {
                            Text: 'Show Active Bins Only',
                            Click: function () {
                                $('#lblFilterBins').text('Bin List Filtered By: Active Bins')
                                FilterBinTable('Active');
                            }
                        },
                        {
                            Text: 'Show Inactive Bins Only',
                            Click: function () {
                                $('#lblFilterBins').text('Bin List Filtered By: Inactive Bins')
                                FilterBinTable('Inactive');
                            }
                        },
                        {
                            Text: 'Show All Bins',
                            Click: function () {
                                $('#lblFilterBins').text('Bin List Filtered By: None')
                                FilterBinTable('');
                            }
                        },
                    ], { AutoDismiss: true });
                })


                $('#butBinBarcode').click(function () {
                    mobile.Scan({
                        Success: function (vData) {
                            if (!IsGuid(vData.Value)) {
                                vData = Base64ToGuid(vData.Value);
                            } else {
                                vData = vData.Value;
                            }

                            binDatatable.search(vData).draw();
                        }
                    });
                });

                $('#txtSearchNamedBin').keyup(function () {
                    binDatatable.search($.trim($(this).val())).draw();
                })

                $('#butBinProductBarcode').click(function () {
                    mobile.Scan({
                        Success: function (vData) {
                            if (!IsGuid(vData.Value)) {
                                vData = Base64ToGuid(vData.Value);
                            }else{
                                vData = vData.Value;
                            }

                            productDatatable.search(vData).draw();
                        }
                    });
                });

                $('#butAreaProductBarcode').click(function () {
                    mobile.Scan({
                        Success: function (vData) {
                            fowstbinproductDatatable.search(vData.Value).draw();
                        }
                    });
                });                

                $('#txtSearchBinProduct').keyup(function () {
                    productDatatable.search($.trim($(this).val())).draw();
                })

                $('#txtSearchAreaProduct').keyup(function () {
                    fowstbinproductDatatable.search($.trim($(this).val())).draw();
                })

                //============Count by AREA

                $('#btnBins').click(function () {
                    if (AreaID == undefined) {
                        NewMessageBox('Please select FOWST Area first.',
                                    "Select FOWST Area"); // END MessageBox
                    } else {
                        GetNamedFowstBin();
                    }
                })

                //$('#btnPriceLevel').click(function () {
                //    GetAllPriceLevels();
                //})

                $('#btnArea').click(function () {                    
                    GetAllFowstArea()
                })

                //===========

                $('#btnBack').click(function () {
                    GotoFirstPage();
                });

                $('#btnBackToMain').click(function (e) {
                    e.preventDefault();

                    window.location = '/People/PeopleMenu';

                });

                $('#btnInventoryBack').click(function (e) {
                    e.preventDefault();

                    window.location = '/People/PeopleMenu';

                });

                $('#selFOWST').change(function () {
                    if ($('#selFOWST').val() == 'Others') {
                        IsCustomer = 1;
                        Browser.LibraryOrganization.FilterCustomers = 1;
                        Browser.LibraryOrganization.Click = function (e) {
                            CustomerID = e.ID;
                            var CustomerName = e.CompanyName;
                            setTimeout(function () {
                                Browser.LibraryFowst.InputData.IsThirdPartyOrganization = 1;
                                Browser.LibraryFowst.InputData.ThirdPartyOrganizationID = CustomerID;
                                Browser.LibraryFowst.Click = function (e) {
                                    $('#selFOWST').val('Select');

                                    FowstID = e.ID;
                                    sessionStorage.FowstID = FowstID;
                                    sessionStorage.FowstName = e.FowstName;
                                    sessionStorage.SelectedFOWST = FowstID;
                                    $('#selFOWST :selected').text(CustomerName + '(' + e.FowstName + ')');

                                    try {
                                        $('#selFOWST').selectmenu('refresh');
                                    }
                                    catch (e) {
                                        $('#selFOWST').selectmenu();
                                    }
                                    HandleEnforceZRead();
                                };
                                Browser.LibraryFowst.Show();
                            }, 500);
                        };
                        Browser.LibraryOrganization.Show();
                    }
                    else {
                        IsCustomer = 0;
                        FowstID = $('#selFOWST').val();
                        sessionStorage.FowstID = FowstID
                        sessionStorage.SelectedFOWST = FowstID;
                        sessionStorage.FowstName = $('#selFOWST option:selected').text();
                        $('#selIntraOrganization').selectmenu('refresh');
                        HandleEnforceZRead();
                    }
                });

                $('#ByBin').click(function () {
                    sessionStorage.IsCountByArea = false;
                    GroupSelected = 'ByBin';
                    SetInventoryCountFormat();
                });

                $('#ByArea').click(function () {
                    sessionStorage.IsCountByArea = true;
                    GroupSelected = 'ByArea';
                    SetInventoryCountFormat();
                });

                $('#ByBrand').click(function () {
                    GroupSelected = 'ByBrand';
                    SetInventoryCountFormat();
                });

                $('#BySpatial').click(function () {
                    GroupSelected = 'BySpatial';
                    SetInventoryCountFormat();
                });

                $('#ByNegativeBalances').click(function () {
                    GroupSelected = 'ByNegativeBalances';
                    SetInventoryCountFormat();
                });

                $('#ByRank').click(function () {
                    MessageBox("This feature is not yet available.",
                                "Bin",
                                [
                                    { Text: 'Ok' }
                                ]
                            );
                });

                $('#ByRecentlyReceived').click(function () {
                    GroupSelected = 'ByRecentlyReceived';
                    SetInventoryCountFormat();
                });

                $('#ByVendor').click(function () {
                    MessageBox("This feature is not yet available.",
                                "Bin",
                                [
                                    { Text: 'Ok' }
                                ]
                            );
                });

                $('#ByUPCOrBarcode').click(function () {
                    GroupSelected = 'ByUPCOrBarcode';
                    SetInventoryCountFormat();
                });

                $('#ByTableType').click(function () {
                    GroupSelected = 'ByTableType';
                    SetInventoryCountFormat();
                    //GetBalancesFromServer();                    
                });

                $('#btnGoDateRange').click(function () {
                    GetAllProducts();
                });

                $('#btnSearchGroup').click(function () {
                    GetAllProducts();
                });

                $('#txtSearchItem').keypress(function (e) {
                    if (e.keyCode == 13) {
                        e.preventDefault();
                        SearchItem($('#txtSearchItem').val());
                    }
                });

                $('#txtSearchItem2').keypress(function (e) {
                    if (e.keyCode == 13) {
                        SearchItem2($('#txtSearchItem2').val());
                    }
                });

                $('#btnScanBarCode').click(function () {
                    mobile.Scan({
                        Success: function (vData) {
                            $('#txtSearchItem').val(vData.Value);
                            SearchItem(vData.Value);
                        }
                    });
                });

                $('#txtSKU').keypress(function (e) {
                    if (e.keyCode == 13) {
                        SearchItem($('#txtSKU').val());
                    }
                });

                $('#btnSKUGo').click(function () {
                    SearchItem($('#txtSKU').val());
                });

                $('#txtBINSearch').keypress(function (e) {
                    if (e.keyCode == 13) {
                        GetFowstBin($('#txtBINSearch').val(), '');
                    }
                });

                $('#btnBinGo').click(function () {
                    GetFowstBin($('#txtBINSearch').val(), '');
                });

                $('#rcChoiceGoodYes').click(function () {
                    IsGoodStock = 1;
                    $('#dvReasonForBadCount').css('visibility', 'hidden');
                    $('#dvReasonForBadCount').css('display', 'none');
                })

                $('#rcChoiceGoodNo').click(function () {
                    IsGoodStock = 0;
                    $('#dvReasonForBadCount').css('visibility', 'visible');
                    $('#dvReasonForBadCount').css('display', 'block');
                })

                $('#selBinsStockViewer').change(function (e) {
                    if ($('#selBinsStockViewer option:selected').val() == 'CreateNewBin') {
                        if (!OfflineMode) {
                            SetFowstID();
                        } else {
                            sessionStorage.FowstID = FowstID;
                            sessionStorage.ProductID = ProductID;
                            sessionStorage.SelectedFOWST = FowstID;
                            HandleEnforceZRead();
                        }

                        window.pageSourcePage = $.mobile.activePage;
                        ChangePage('/fowstbin/BinInfo');
                    }
                    else {
                        FowstBinID = $('#selBinsStockViewer option:selected').val();
                        GetFowstBinProductMaxMinCap();
                        sessionStorage.FowstBinID = FowstBinID //para magamit ng stock.js
                    }
                });

                $('#selBinsStockViewerMultipleSize').change(function (e) {
                    if ($('#selBinsStockViewerMultipleSize option:selected').val() == 'CreateNewBin') {
                        SetFowstID();
                        window.pageSourcePage = $.mobile.activePage;
                        ChangePage('/fowstbin/BinInfo');
                    }
                    else {
                        FowstBinID = $('#selBinsStockViewerMultipleSize option:selected').val();
                    }
                });


                $('#selBins').change(function (e) {
                    if ($('#selBins option:selected').val() == 'CreateNewBin') {
                        SetFowstID();
                        sessionStorage.ProductID = ProductID;
                        window.pageSourcePage = $.mobile.activePage;
                        ChangePage('/fowstbin/BinInfo');
                    }
                    else {
                        FowstBinID = $('#selBins option:selected').val();
                    }
                });

                $('#btnAddNewBin').click(function () {
                    $('#dvAddBin').css('visibility', 'visible');
                    $('#dvAddBin').css('display', 'block');
                });

                $('#rcChoiceExpirationYes').click(function () {
                    $('#dvDate').css('visibility', 'visible');
                    $('#dvDate').css('display', 'block');
                    IsWithExpiration = 1;
                });

                $('#rcChoiceExpirationNo').click(function () {
                    $('#dvDate').css('visibility', 'hidden');
                    $('#dvDate').css('display', 'none');
                    IsWithExpiration = 0;
                });

                $('#optMonthValue').change(function (e) {
                    e.preventDefault();
                    setDayCollection('optMonthValue', 'optDayValue', 'optYearValue', true);
                });

                $('#optYearValue').change(function (e) {
                    e.preventDefault();
                    setDayCollection('optMonthValue', 'optDayValue', 'optYearValue', true);
                });

                setMonthCollection("optMonthValue");
                setYearCollection("optYearValue");

                var m_Date = new Date();

                $('#optMonthValue').val(m_Date.getMonth() + 1);
                $('#optYearValue').val(m_Date.getFullYear());
                setDayCollection('optMonthValue', 'optDayValue', 'optYearValue');
                $('#optDayValue').val(m_Date.getDate());

                $('#btnSaveBinName').click(function () {
                    if ($('#txtNewBinName').val() == '') {
                        MessageBox("Please enter a Shelf/Bin name.",
                                "Bin",
                                [
                                    { Text: 'Ok' }
                                ]
                            );
                        return;
                    }

                    var data = {
                        FowstBinID: FowstBinID,
                        Name: $('#txtNewBinName').val()
                    };

                    AjaxSend({
                        url: 'InventoryCount/SaveBinName',
                        data: data,
                        success: function (vData) {
                            $('#txtStockViewerBIN').val($('#txtNewBinName').val());
                            $('.ui-dialog').dialog('close')
                        }
                    });
                });
            });

            $('#btnDoneUnitMeasure').click(function () {
                AddToList();
            });

            $('#optSimpleView').click(function () {
                IsAdvanceView = 0;
            });

            $('#optAdvanceView').click(function () {
                IsAdvanceView = 1;
            });

            $('#btnCountByUPC').click(function () {
                CountByUPC();
                mUPCProduct = [];
                mUPCCount = 0;
            });

            $('#btnSaveSizesTallies-Detail').click(function () {
                var Count = [];
                var ctr = 0;

                if ($('#selBinsStockViewerMultipleSize').val() == '') {
                    MessageBox("Please select Shelf/Bin Location.",
                                "Bin",
                                [
                                    { Text: 'Ok' }
                                ]
                            );
                    return;
                }

                var IsNumber = 1;

                $('#lstSizesUnitMeasure tr td input').each(function (m_index) {
                    if ($(this).val() != '') {
                        if (!IsNumeric($(this).val())) {
                            IsNumber = 0.0;
                            return;
                        }
                    }
                });

                if (!IsNumber) {
                    MessageBox("Quantity must be a number greater than 0.",
                                "Invalid Quantity",
                                [
                                    { Text: 'Ok' }
                                ]
                            );
                    return;
                }

                var IsNegative = 0;
                var BatchID = NewID();

                $('#lstSizesUnitMeasure tr td input').each(function (m_index) {
                    if ($(this).val() < 0) {
                        IsNegative = 1;
                        return;
                    }
                });

                if (IsNegative) {
                    MessageBox("Quantity must be a number greater than 0.",
                                "Invalid Quantity",
                                [
                                    { Text: 'Ok' }
                                ]
                            );
                    return;
                }

                $('#lstSizesUnitMeasure tr td input').each(function (m_index) {
                    var data = {
                        ID: ID,
                        IsNew: IsNew,
                        FowstID: FowstID,
                        ProductID: $(this).attr('ProductID'),
                        Count: Count,
                        BatchID: BatchID
                    }

                    Count[0] = { BinID: FowstBinID,
                        ExpirationDate: 'N/A',
                        UnitMeasureID: $(this).data('data').ID,
                        Quantity: CDec2($(this).val()).toString(),
                        ActualQuantity: parseInt($(this).data('data').Quantity),
                        IsGoodStock: 1
                    };

                    Save(data, Count);
                    ctr++;
                });


            });

            $('#btnSaveTallies').click(function () {
                var Count = [];
                var ctr = 0;
                var ctr2 = 0;
                var SuggestedOrderToBeDeducted = 0;
                var BatchID = NewID();

                $('#lstCounts li a').each(function (m_index) {
                    if ($(this).data('units') != undefined) {
                        if ($(this).data('units').ID != undefined) {
                            //we will subtract this to suggested order since suggested order of this data is already in the database (To Prevent double suggested order)
                            SuggestedOrderToBeDeducted += parseFloat($(this).data('units').SuggestedOrder);
                        } else {
                            //we save this data becase data with GUID ID is already in database
                            while (ctr < $(this).data('units').length) {
                                Count[ctr2] = {
                                    BinID: $(this).data('units')[ctr].FowstBinID,
                                    ExpirationDate: $(this).data('units')[ctr].ExpirationDate,
                                    UnitMeasureID: $(this).data('units')[ctr].UnitMeasureID,
                                    Quantity: $(this).data('units')[ctr].Quantity,
                                    ActualQuantity: $(this).data('units')[ctr].ActualQuantity,
                                    IsGoodStock: $(this).data('units')[ctr].IsGood,
                                    BadCountReason: $(this).data('units')[ctr].BadCountReason
                                };
                                ctr++;
                                ctr2++;
                            }
                            ctr = 0;
                        }
                    } //if ($(this).data('units') != undefined) {

                });

                if (ctr2 == 0) {
                    AddToList();
                    return;
                }

                if (CBool(sessionStorage.IsInventoryCountMode)) {
                    var data = {
                        ID: ID,
                        IsNew: IsNew,
                        FowstID: sessionStorage.CustomerOrganizationID,
                        ProductID: proj.ProductID,
                        SuggestedOrder: parseFloat($('#txtSuggestedOrder').val()) - SuggestedOrderToBeDeducted,
                        Count: Count,
                        BatchID: BatchID
                    }
                }
                else {
                    var data = {
                        ID: ID,
                        IsNew: IsNew,
                        FowstID: FowstID,
                        ProductID: ProductID,
                        Count: Count
                    }
                }
                Save(data, Count);
            });

            $('#btnSaveAllContinousCount').click(function () {

                if ($('#selBinsByUPC').val() == '') {
                    MessageBox("Please select Shelf/Bin Location.",
                                "Bin",
                                    [
                                        { Text: 'Ok' }
                                    ]
                                );
                    return;
                }

                if (FowstID == '') {

                }

                var BatchID = NewID();

                $('#reportTable tr').each(function () {
                    var Count = [];

                    Count[0] = {
                        BinID: $('#selBinsByUPC').val(),
                        ExpirationDate: 'N/A',
                        UnitMeasureID: $(this).data('data').UnitMeasureID,
                        Quantity: parseInt($(this).data('data').Quantity),
                        ActualQuantity: parseInt($(this).data('data').ActualQuantity),
                        IsGoodStock: 1
                    };

                    var data = {
                        ID: ID,
                        IsNew: 1,
                        FowstID: FowstID,
                        ProductID: $(this).data('data').ProductID,
                        Count: Count,
                        BatchID: BatchID
                    }

                    Save(data, Count);

                    $(this).css('visibility', 'hidden');
                })                        
            });
               
            $('#txtSearchProducts').keypress(function (e) {
                if (e.keyCode == 13) {

                    var mHasValue = false;

                    $('#tblInventoryCount tr td input').each(function (mindex) {
                        if ($(this).val() != '') {
                            mHasValue = true;
                            return 0;
                        }
                    });

                    if (mHasValue) {
                        p=MessageBox("There are pending counts waiting to be save. Would you like to save it? .",
                                "Pending Counts",
                                [
                                    {
                                        Text: 'Yes',
                                        Click: function () {                                            
                                            p.dialog('close');
                                            setTimeout(function () {
                                                SaveCountFromTableList();
                                            }, 500)
                                        }
                                    },
                                    {
                                        Text: 'No',
                                        Click: function () {
                                            $('ui-dialog').dialog('close');
                                            CategoryType = '';
                                            GetAllProductsForTableType(CategoryType, $('#txtSearchProducts').val());
                                            //DisplayAllProductsForTableType(mData.Rows);
                                            $('.lblSelectedCategory').text('Select Category')
                                        }
                                    }
                                ]
                            );
                    } else {
                        CategoryType = '';
                        GetAllProductsForTableType(CategoryType, $('#txtSearchProducts').val());
                        //DisplayAllProductsForTableType(mData.Rows);
                        $('.lblSelectedCategory').text('Select Category')
                    }
                }                
            });

            $('#btnSearchProductBarcode').click(function () {
                mobile.Scan({
                    Success: function (vData) {
                        GetAllProductsForTableType(CategoryType, vData.Value);
                    }
                });
            });
                        
            $('#btnSaveObservedCountByTableList').click(function () {
                SaveCountFromTableList(0);
            });

            $('#btnSaveAndTurnOverObservedCountByTableList').click(function () {
                SaveCountFromTableList(1);
                $('#txtDepositBy').val(sessionStorage.CurrentUserFullName);
            });

            $('#btnBackToInventoryCountPage').click(function () {
                $.mobile.changePage("#");
            });

            $('#btnCategoriesBack').click(function () {                
                $.mobile.changePage("#");
            });

            $('#btnTurnOverInventory').click(function () {
                GetAllMyCounts();
                $('#txtDepositBy').val(sessionStorage.CurrentUserFullName);
                $.mobile.changePage("#PageMyCounts");
            });

            $('#btnTurnOverBack').click(function () {
                $.mobile.changePage('#PageCategories');
            });

            $('#txtDepositTo').keypress(function (e) {
                if (e.keyCode == 13) {
                    if(OfflineMode){
                        Browser.LibraryPeople.OfflineMode = true;
                        Browser.OfflineMode = true;
                    }
                    Browser.LibraryPeople.Click = function (e) {
                        TurnOverToID = e.ID
                        $('#txtDepositTo').val(e.PeopleName);
                        $.mobile.changePage("#DialogUserAccess");
                    };
                    Browser.LibraryPeople.Show($('#txtDepositTo').val());
                }
            });

            $('#btnVerifyEmployee').click(function () {
                
                pSql = 'SELECT * FROM LibraryPeopleUserAccount WHERE PeopleID = ' + ReadyString(TurnOverToID);
                pDT = Sql.Execute(pSql);


                if (pDT.Error) {                    
                    IsEmployeeVerified = false;
                    $('#dvInvalidUsernameOrPassword').css('visibility', 'visible');
                    $('#dvInvalidUsernameOrPassword').css('display', 'block');
                };
                if (pDT.Rows.length == 0) {
                    IsEmployeeVerified = false;
                    $('#dvInvalidUsernameOrPassword').css('visibility', 'visible');
                    $('#dvInvalidUsernameOrPassword').css('display', 'block');
                } else if (CBool(Android.VerifyHash($('#txtPassword').val(), pDT.Rows[0].UserPassword))) {
                    IsEmployeeVerified = true;
                    $('#btnSaveTurnOver').click();
                    $.mobile.changePage("#dialogVerifySignOut");
                } else {
                    IsEmployeeVerified = false;
                    $('#dvInvalidUsernameOrPassword').css('visibility', 'visible');
                    $('#dvInvalidUsernameOrPassword').css('display', 'block');
                }
            });

            $('#btnSaveTurnOver').click(function () {
                if (TurnOverToID == '') {
                    
                } else {
                    if (IsEmployeeVerified) {
                        TagCountTurnOver();
                    } else {
                        $.mobile.changePage("#DialogUserAccess");
                    }
                }
            });

            $('#btnGetBalancesFromServer').click(function () {
                MessageBox("Downloading expected inventory balances from server(s) requires a live Internet connection. Please make sure that you are connected to the Internet and click on the OK button when done.",
                            "Inventory Balance",
                            [
                                {
                                    Text: 'Ok',
                                    Click: function () {
                                        $('.ui-dialog').dialog('close');

                                        setTimeout(function () {
                                            GetBalancesFromServer();
                                        }, 500);
                                                                                
                                    }
                                },
                                {
                                    Text: 'Cancel',
                                    Click: function () {
                                        $('.ui-dialog').dialog('close');
                                    }                                        
                                }
                            ]
                        ); //end messagebox
            });

            $('#selProductFilter').change(function () {
                ProductFilter = $('#selProductFilter').val();
                if ($('#txtSearchProducts').val() == '') {
                    GetAllProductsForTableType(CategoryType, '');
                } else {
                    GetAllProductsForTableType(CategoryType, $('#txtSearchProducts').val());
                }
            });

            $('#btnVerifySignOutYes').click(function () {                
                SignInCheckUser(); // SignInInfo.js
                sessionStorage.removeItem('TOUserName');
                sessionStorage.removeItem('TOPassword');
                sessionStorage.removeItem('IsFromTurnOverOfMoniesPage');
            });

            $('#btnVerifySignOutNo').click(function () {
                var pParam = {};

                sessionStorage.removeItem('TOUserName');
                sessionStorage.removeItem('TOPassword');
                sessionStorage.removeItem('IsFromTurnOverOfMoniesPage');

                pParam.IsMobile = 1;
                pParam = $.param(pParam);
                window.location = "/People/PeopleMenu?" + pParam;
            });

            $('.btnSelectCategory').click(function () {                
                GetCategories();
            })

            $('#btnBinCount').click(function () {
                AddAllBins();
            })
        }); //End Inner function


        var mUPCProduct = [];
        var mUPCCount = 0;
        
        function CountByUPC() {

            ContinuousBarcode({
                Click: function (e) {
                    var tData = '';

                    $('#reportTable').empty();

                    e.forEach(function (item) {

                        var data = { barcode: item.Barcode };
                        mData = GetProductInformationByBarcode(data);

                        var mData2 = '';

                        if (mData.Rows.length > 0) {


                            data = { ProductID: mData.Rows[0].ID }; //para magamit ng mData2 sa query

                            mData2 = InventoryCount.GetStockUnitMeasures(data);

                            data = {
                                ProductID: mData.Rows[0].ID,
                                UnitMeasureID: mData2.Rows[0].ID,
                                Quantity: item.Count,
                                ActualQuantity: mData2.Rows[0].Quantity
                            }

                            tData = $('<tr>')
                                .data('data', data)
                                .append($('<td>')
                                    .text(item.Barcode)
                                )
                                .append($('<td>')
                                    .text(mData.Rows[0].ProductName)
                                )
                                .append($('<td>')
                                    .text(mData2.Rows[0].UnitMeasure)
                                )
                                .append($('<td>')
                                    .text(item.Count)
                                )
                            $('#reportTable').append(tData);
                            $('#' + mData.Rows[0].ID).button();
                        } else {
                            //unrecognize barcode
                        }
                    });
                        $('#tblReportTable').table('refresh');
                },
                Cancel: function () {

                }
            });

            //mobile.Scan({
            //    Success: function (vData) {
            //        var isExist = false;

            //        for (i = 0; i < mUPCProduct.length; i++) {
            //            if (mUPCProduct[i][0] == vData.Value) {
            //                mUPCProduct[i][1]++;
            //                isExist = true;
            //                return;
            //            }

            //        };

            //        if (!isExist) {
            //            mUPCProduct[mUPCCount] = [];
            //            mUPCProduct[mUPCCount][0] = vData.Value;
            //            mUPCProduct[mUPCCount][1] = 1;
            //            mUPCCount++;
            //        }

            //        CountByUPC();
            //    },
            //    Error: function () {
            //        var tData = '';

            //        $('#reportTable').empty();
                                        
            //        for (i = 0; i < mUPCProduct.length; i++) {

            //            var data = { barcode: mUPCProduct[i][0] };
            //            mData = GetProductInformationByBarcode(data);

            //            for (j = 0; j <= mData.Rows.length - 1; j++) {

            //                var mData2 = '';

            //                data = { ProductID: mData.Rows[j].ID }; //para magamit ng mData2 sa query

            //                mData2 = InventoryCount.GetStockUnitMeasures(data);

            //                data = {
            //                    ProductID: mData.Rows[j].ID,
            //                    UnitMeasureID: mData2.Rows[0].ID,
            //                    Quantity: mUPCProduct[i][1],
            //                    ActualQuantity: mData2.Rows[0].Quantity
            //                }

            //                tData = $('<tr>')
            //                    .data('data', data)
            //                    .append($('<td>')
            //                        .text(mUPCProduct[i][0])
            //                    )
            //                    .append($('<td>')
            //                        .text(mData.Rows[j].ProductName)
            //                    )
            //                    .append($('<td>')
            //                        .text(mData2.Rows[0].UnitMeasure)
            //                    )
            //                    .append($('<td>')
            //                        .text(mUPCProduct[i][1])
            //                    )                               
            //                $('#reportTable').append(tData);
            //                $('#' + mData.Rows[j].ID).button();
            //            }
            //            $('#tblReportTable').table('refresh');
            //        };

            //    }
            //});
        }

        function TagCountTurnOver() {
            $('#tblMyInventoryCount tr').each(function (mindex) {
                data = {
                    ID: $(this).attr('ID'),
                    TurnOverToID: TurnOverToID,
                    InventoryProductCountID: $(this).data('data').InventoryProductCountID,
                }

                var IsSuccessful = false;

                if (OfflineMode) {
                    IsSuccessful = InventoryCount.UpdateInventoryCountTurnOver(data);
                }

            });

            $.mobile.changePage('#PageCategories');
        }

        function GetAllMyCounts(isTurnOverItems, CategoryType) {

            mData = InventoryCount.GetAllMyCounts(isTurnOverItems, CategoryType);

            if (mData.Rows.length > 0) {
                DisplayMyCount(mData.Rows);
            }

        }

        function DisplayMyCount(vRows) {
            pLV = $('#tblMyInventoryCount');

            pLV.html('');

            for (var i = 0; i <= vRows.length - 1; i++) {
                
                pItem = $('<tr>')
                        .attr('ID', vRows[i].ID)
                        .data('data', vRows[i])
                        .append($('<th>')
                            .append($('<span>')
                                .text(vRows[i].ProductName)
                            )
                        )
                        .append($('<td>')
                            .append($('<span>')
                                .text(vRows[i].UnitMeasure)
                            )
                        )
                        .append($('<td style="text-align:right">')
                            .append($('<span>')
                                .text(vRows[i].Quantity)
                            )
                        )
                        .append($('<td style="text-align:right">')
                                    .append($('<span>')
                                        .text(vRows[i].ExpectedValue)
                                    )
                                )
                        .append($('<td style="text-align:right">')
                                    .append($('<span>')
                                        .text(vRows[i].Variance)
                                    )
                                )
                        .append($('<td style="text-align:right">')
                                    .append($('<span>')
                                        .text(vRows[i].Remarks)
                                    )
                                )
                        
                pLV.append(pItem)

            }
        }

        function SetPageGroup() {
            if (OfflineMode) {
                FlushData();
            }
        }

        function FlushData() {
            SalesPaymentsExport_offline().Export({
                OnUploaded: function () {
                    //                                    FollowUp();
                },
                OnDone: function (vData) {
                    if (vData.Result == 'ok')
                        Android.AddLog('Done exporting js2CQHQ5B4QA');
                    else if (vData.Result == 'error')
                        Android.Toast(vData.Error);
                    App.Deferred.fulfill('KWXCETE5XN');
                }
            }); // END SalesPaymentsExport_offline().Export({
        }

        function PageShow() {
            GetNamedFowstBin();
            if (FowstBinID != undefined) {
                $('#selBins').val(FowstBinID);
                $('#selBinsByUPC').val(FowstBinID);
                
                try {
                    $('#selBins').selectmenu('refresh');
                    $('#selBinsByUPC').selectmenu('refresh');
                } catch (e) {
                    $('#selBins').selectmenu();
                    $('#selBinsByUPC').selectmenu();
                }
            }

        }

        function GetProductInformationByBarcode(data) {
            var mData = '';
            mData = InventoryCount.GetProductInformationByBarcode(data);
            return mData;
        }

        function IsHideGroup(IsHide) {

            $("#lstProducts").empty();

            if (IsHide) {

                $('#dvProductGroup').css('visibility', 'hidden');
                $('#dvProductGroup').css('display', 'none');

                $('#dvGroup').css('visibility', 'hidden');
                $('#dvGroup').css('display', 'none');
                $('#GroupTitle').css('visibility', 'hidden');
                $('#GroupTitle').css('display', 'none');
                $('#dvGroupDate').css('visibility', 'hidden');
                $('#dvGroupDate').css('display', 'none');
            }
            else {
                $('#selGroup').empty();
                $("#lstProducts").empty();

                $('#dvProductGroup').css('visibility', 'visible');
                $('#dvProductGroup').css('display', 'block');

                $('#dvGroup').css('visibility', 'visible');
                $('#dvGroup').css('display', 'block');
                $('#GroupTitle').css('visibility', 'visible');
                $('#GroupTitle').css('display', 'block');

                $('#dvGroupDate').css('visibility', 'hidden');
                $('#dvGroupDate').css('display', 'none');
            }
        }

        function SetStockPage() {

        }

        function SetStockViewerBin() {
            //            if (FowstID = '') {
            //                if (sessionStorage.FowstID != undefined) {
            //                    FowstID = sessionStorage.FowstID;
            //                }                
            //            }
            GetNamedFowstBin();
            if (FowstBinID != undefined) {
                GetFowstBinProductMaxMinCap();
                $('#selBinsStockViewer').val(FowstBinID);
                $('#selBinsStockViewerMultipleSize').val(FowstBinID);

                try {
                    $('#selBinsStockViewer').selectmenu('refresh');
                    $('#selBinsStockViewerMultipleSize').selectmenu('refresh');
                } catch (e) {
                    $('#selBinsStockViewer').selectmenu();
                    $('#selBinsStockViewerMultipleSize').selectmenu();
                }
            }
        }

        function ClearFields() {
            $('#txtSKU').val('');
            $('#txtBinMaxCap').val('');
            $('#txtBinMinCap').val('');
        }

        function AddToList() {
            var pLV = $('#lstCounts')
            var GoodStocks = '';
            var CountResult = '';
            var CountTotal = 0;
            var TotalSku = 0;
            var BaseUnitMeasure = ''
            var data = {};
            var ExpDate = '';
            var ctr = 0;
            var IsExist = 0;
            var pQty;

            var maxCap = 0;
            var minCap = 0;

            maxCap = parseFloat($('#txtBinMaxCap').val());
            minCap = parseFloat($('#txtBinMinCap').val());

            if (maxCap < minCap) {
                MessageBox("\'Max Cap for this bin should be greater than or equal to Min Cap\'",
                                "Inventory Count",
                                [
                                    { Text: 'Ok' }
                                ]
                            );
                return;
            }

            if ($('#selBinsStockViewer').val() == '') {
                MessageBox("Please select Shelf/Bin Location",
                                "Bin",
                                [
                                    { Text: 'Ok' }
                                ]
                            );
                return;
            }

            if ($('#selBinsStockViewer').val() == 'Unnamed Bin') {
                $.mobile.changePage('#pageRenameBin');
                return;
            }

            if (IsDate($('#txtExpirationDate').DateCombo('val')))
                ExpDate = $.format.date($('#txtExpirationDate').DateCombo('val'), 'MM/dd/yy');
            else
                ExpDate = 'N/A';

            $('#lstCounts a').each(function (m_index) {
                if ($(this).data('units') != undefined) {
                    if (IsGuid($(this).data('units').ID)) {
                        if ($(this).data('units').FowstBinID == FowstBinID && $(this).data('units').ProductID == ProductID && $(this).data('units').ExpirationDate == ExpDate && parseInt($(this).data('units').IsGood) == IsGoodStock) {
                            MessageBox("Stock and Shelf/Bin already exist in the tally.",
                                "Bin",
                                [
                                    { Text: 'Ok' }
                                ]
                            );
                            IsExist = 1;
                            return 0;
                        }
                    } else {
                        if ($(this).data('units')[ctr].FowstBinID == FowstBinID && $(this).data('units')[ctr].ProductID == ProductID && $(this).data('units')[ctr].ExpirationDate == ExpDate && $(this).data('units')[ctr].IsGood == IsGoodStock) {
                            MessageBox("Stock and Shelf/Bin already exist in the tally.",
                                "Bin",
                                [
                                    { Text: 'Ok' }
                                ]
                            );
                            IsExist = 1;
                            return 0;
                        }
                    }
                };
            });

            if (IsExist) {
                return;
            }

            if (IsGoodStock == 1)
                GoodStocks = 'Good Count';
            else
                GoodStocks = 'Bad Count';

            data.units = [];

            var IsNegative = 0;
            var IsNumber = 0;
            var mHasError;

            if (FowstBinID == undefined || FowstBinID == '') {
                FowstBinID = $('#selBinsStockViewer').val();
            }

            $('#lstUnitMeasure tr td input').each(function (m_index) {
                if ($(this).val() != '') {
                    pQty = $(this).val();
                    if (IsNumeric(pQty)) {
                        if (CDec2(pQty) >= 0) {
                            //comment date: 10/15/15
                            //if ((CDec2(pQty) != CInt2(pQty))
                            //    && (!mAllowDecimal)) {

                            //    mHasError = true;
                            //    MessageBox('This product does not allow decimals. You entered ' + pQty);
                            //    return;
                            //}
                            data.units.push({ ID: $(this).attr('ID'),
                                FowstID: FowstID,
                                FowstBinID: FowstBinID,
                                ProductID: ProductID,
                                ExpirationDate: ExpDate,
                                UnitMeasureID: $(this).data('data').ID,
                                Quantity: pQty,
                                ActualQuantity: parseInt($(this).data('data').Quantity),
                                IsGood: IsGoodStock,
                                MaxCap: $('#txtBinMaxCap').val(),
                                MinCap: $('#txtBinMinCap').val(),
                                BadCountReason: $('#txtBadCountReason').val()
                            });
                            IsNumber = 1;
                        }
                        else {
                            IsNegative = 1;
                            return;
                        };
                    } /*else {
                        IsNumber = 0;
                        return;
                    }*/
                }
                /*else {
                    IsNumber = 0;
                    return;
                } //$(this).val() != ''*/

                if ($(this).val() == '') {
                    CountResult += $(this).data('data').UnitMeasure + ': 0, ';
                    CountTotal += 0;
                }
                else{
                    CountResult += $(this).data('data').UnitMeasure + ': ' + $(this).val() + ', '
                    CountTotal += 0;
                }
            
                TotalSku += CDec2($(this).val()) * parseInt($(this).data('data').Quantity);
            });

            if (mHasError)
                return;

            if (!IsNumber) {
                MessageBox("Quantity must be a number greater than 0.",
                                "Invalid Quantity",
                                [
                                    { Text: 'Ok' }
                                ]
                            );
                return;
            }

            if (IsNegative) {
                MessageBox("Quantity must be a number greater than 0.",
                                "Invalid Quantity",
                                [
                                    { Text: 'Ok' }
                                ]
                            );
                return;
            }

            if (sessionStorage.IsInventoryCountMode) {
                var mTotal = parseFloat($('#txtBinMaxCap').val()) - parseFloat(CountTotal);

                if (!IsGoodStock) {
                    mTotal = 0;
                }
                else {
                    ComputeSuggestedOrder(mTotal);
                }

                pItem = $('<li>')
                    .attr('ID', IsGoodStock + FowstID + FowstBinID + ProductID)
                    .append($('<a>')
                        .attr('ID', IsGoodStock + FowstID + FowstBinID + ProductID + ExpDate)
                        .data(data)
                        .append($('<h3>')
                            .text('SKU Info')
                        )
                        .append($('<h4>')
                            .text('Shelf/Bin Location: ' + $('#selBinsStockViewer option:selected').text())
                        )
                        .append($('<h4>')
                            .text('Expiration Date: ' + ExpDate)
                        )
                        .append($('<h4>')
                            .text('Max Cap (Qty)' + ': ' + $('#txtBinMaxCap').val())
                        )
                        .append($('<h4>')
                            .text(GoodStocks + ': ' + CountResult)
                        )
                        .append($('<h4>')
                            .text('Max Cap - Good Count: ' + mTotal)
                        )
                    )
                    .append($('<a>')
                        .attr('ID', 'close' + IsGoodStock + FowstID + FowstBinID + ProductID)
                        .click(function () {
                            $('#' + $(this).attr('ID').substr(5, $(this).attr('ID').length)).remove();

                            if (IsGuid($(this).attr('ID').substr(5, $(this).attr('ID').length))) {
                                RemoveFromDatabase($(this).attr('ID').substr(5, $(this).attr('ID').length), $('#' + $(this).attr('ID').substr(5, $(this).attr('ID').length) + ' a').data('units').SuggestedOrder);
                            }
                            ComputeSuggestedOrder(0);
                        })
                    );
            }
            else {
                pItem = $('<li>')
                        .attr('ID', IsGoodStock + FowstID + FowstBinID + ProductID)
                        .append($('<a>')
                            .attr('ID', IsGoodStock + FowstID + FowstBinID + ProductID + ExpDate)
                            .data(data)
                            .append($('<h3>')
                                .text('SKU Info')
                            )
                            .append($('<h4>')
                                .text('Shelf/Bin Location: ' + $('#selBinsStockViewer option:selected').text())
                            )
                            .append($('<h4>')
                                .text(GoodStocks + ': ' + CountResult)
                            )
                            .append($('<h4>')
                                .text('Expires on: ' + ExpDate)
                            )
                        )
                        .append($('<a>')
                            .attr('ID', 'close' + IsGoodStock + FowstID + FowstBinID + ProductID)
                            .click(function () {
                                $('#' + $(this).attr('ID').substr(5, $(this).attr('ID').length)).remove();
                            })
                        )
            }


            pLV.append(pItem);

            try {
                pLV.listview('refresh');
            }
            catch (e) {
                pLV.listview();
            }

            if ($('#lstCounts li').length == 1) {
                ClearUnitMeasureFields();
                MessageBox("Your count has been successfully added to the tally.  If you are done counting this SKU, please click on the “Save Tallies” button below to confirm your count(s). " +
                            "Otherwise, you might want to count the contents of another Shelf/bin or shelf containing this SKU, or count this SKU with another expiration date grouping. " +
                            "If so, would you like to do another count for this SKU?",
                            "Bin",
                            [
                                { Text: 'Yes'
                                },
                                { Text: 'No',                                    
                                }
                            ]
                        ); //end messagebox
            }

            $('#txtBadCountReason').val('');

            if (CBool(sessionStorage.IsInventoryCountMode)) {
                UpdateFOWSTBinProductMaxMinCap();
            }
            
            $('#btnSaveRemark').click(function () {
                $('#panRemarkPopup').popup('close');
            });


        }

        function ClearUnitMeasureFields() {
            $('#lstUnitMeasure tr td input').each(function (m_index) {
                $(this).val('');
            });
        }

        function RemoveFromDatabase(vID, SuggestedOrder) {
            var pSql = '';

            pSql = 'Delete from TransInventoryProductCountStock WHERE InventoryProductCountID = ' + ReadyString(vID);
            Sql.ExecuteSql(pSql);

            pSql = 'Update TransInventoryProductCount set SuggestedOrder = SuggestedOrder - ' + SuggestedOrder + ' WHERE ID = ' + ReadyString(vID);
            Sql.ExecuteSql(pSql);
        }

        function SearchItem(Sku) {

            window.IsBrowserloaded = false;
            Browser.OfflineMode = true;
            Browser.LibraryPIMProduct.InputData.IsSearchMore = 0;
            Browser.LibraryPIMProduct.Click = function (e, vOpt) {
                mInterval = setTimeout(function () {
                    ProductID = e.ID;
                    GetFowstBinProductMaxMinCap();
                    mAllowDecimal = CBool(e.AllowDecimal);
                    $('#lblStockNumber').text(e.ProductNo);
                    $('#lblStockDescription').text(e.ProductName);

                    $('#lblStockNumber-Detail').text(e.ProductNo);
                    $('#lblStockDescription-Detail').text(e.ProductName);

                    $('#lstCounts').empty();

                    var data = { ProductGroupNo: e.ProductGroupNo };


                    GetProductGrouping(data);
                }, 500);
            };
            Browser.LibraryPIMProduct.Show(Sku);

            //            if (OfflineMode) {
            //                window.IsBrowserloaded = false;
            //                Browser.LibraryPIMProduct.InputData.IsSearchMore = 0;
            //                Browser.LibraryPIMProduct.Click = function (e, vOpt) {                  

            //                };
            //                Browser.LibraryPIMProduct.Show(Sku);
            //            }
            //            else {
            //                window.pageSourcePage = $.mobile.activePage;
            //                Browser.LibraryPIMProduct.Click = function (e) {
            //                    mInterval = setTimeout(function () {
            //                        ProductID = e.ID;
            //                        mAllowDecimal = CBool(e.AllowDecimal);
            //                        $('#lblStockNumber').text(e.ProductNo);
            //                        $('#lblStockDescription').text(e.ProductName);

            //                        $('#lblStockNumber-Detail').text(e.ProductNo);
            //                        $('#lblStockDescription-Detail').text(e.ProductName);

            //                        $('#lstCounts').empty();

            //                        var data = { ProductGroupNo: e.ProductGroupNo };

            //                        GetProductGrouping(data);
            //                    }, 500);
            //                };

            //                Browser.LibraryPIMProduct.Show(Sku);
            //            }
        }

        function GetProductGrouping(data) {

            if (OfflineMode) {
                var mData = '';
                mData = InventoryCount.GetCountPIMProductGroupNo(data);
                ProcessGroupData(mData);
            }
            else {
                AjaxSend({
                    url: 'InventoryCount/GetCountPIMProductGroupNo',
                    data: data,
                    success: function (vData) {
                        ProcessGroupData(vData.InventoryCount);
                    }
                }); //end ajaxsend else
            } //end else
        }

        function ProcessGroupData(vData) {
            if (vData.Rows.length > 1) {
                MessageBox("The SKU you searched for has a product grouping.  Would you like to count all SKUs within this product grouping?",
                                "Product Grouping",
                                [
                                    { Text: 'Yes',
                                        Click: function () {
                                            $.mobile.changePage('#pageStockViewerDetail');
                                            DisplayProductSizes(vData.Rows);
                                            $('#selBinsStockViewerMultipleSize').val(FowstBinID);
                                            try {
                                                $('#selBinsStockViewerMultipleSize').selectmenu('refresh');
                                            }
                                            catch (e) {
                                                $('#selBinsStockViewerMultipleSize').selectmenu();
                                            }
                                        }
                                    },
                                    { Text: 'No',
                                        Click: function () {
                                            GetStockUnitMeasures(ProductID);
                                        }
                                    }
                                ]);
            }
            else {
                GetStockUnitMeasures(ProductID);
                $('#selBinsStockViewer').val(FowstBinID);
                try {
                    $('#selBinsStockViewer').selectmenu('refresh');
                }
                catch (e) {
                    $('#selBinsStockViewer').selectmenu();
                }

            }
        }
        function SearchItem2(Sku) {

            Browser.OfflineMode = true;
            Browser.LibraryPIMProduct.Click = function (e) {
                ProductID = e.ID;
                mAllowDecimal = e.AllowDecimal;
                var data = { ProductID: e.ID,
                    FowstID: FowstID
                };

                AjaxSend({
                    url: 'InventoryCount/EmptyDefaultBin',
                    data: data,
                    success: function (vData) {
                    }
                });
            };

            Browser.LibraryPIMProduct.Show(Sku);
        }

        function DisplayFowstPopUp() {
            BrowsePopup('LibraryFowst', 'FOWST',
                [
                    {
                        AutoSearch: true,
                        DisplayText: [{ Column: 'FowstName' }],
                        Click: function (e) {
                            sessionStorage.FowstID = e.ID;
                            sessionStorage.FowstName = e.FowstName;
                            sessionStorage.SelectedFOWST = FowstID;
                            $('#lblWorkFowstName').text(sessionStorage.FowstName);
                            $('.btnFOWST').text('FOWST: ' + sessionStorage.FowstName)
                            GetNamedFowstBin();
                            HandleEnforceZRead();
                        }
                    }
                ], { FilterFowst: 1 }
            ); // END BrowsePopup('LibraryFowst', 'Fowst',
        }

        function GetFowst() {
            
            var data = { FilterFowst: 1 }
            AjaxSend({
                url: 'Browser/LibraryFowst',
                data: data,
                success: function (vData) {
                    DisplayFowst(vData.Results);
                }
            });
        }

        function DisplayFowst(vData) {
            $('#selFOWST').empty();
            //$('#selFOWST').append(new Option('Select', '', false));

            for (var i = 0; i <= vData.Rows.length - 1; i++) {
                if (vData.Rows[i].ID == undefined) {//server side getting fowst
                    $('#selFOWST').append(new Option(vData.Rows[i].FowstName, vData.Rows[i].FowstID, false));
                    $('#selFOWST').val(vData.Rows[i].FowstID);                    
                } else {
                    $('#selFOWST').append(new Option(vData.Rows[i].FowstName, vData.Rows[i].ID, false));
                    $('#selFOWST').val(vData.Rows[i].ID);
                }                
            }

            $('#selFOWST').append(new Option('Select Client\'s FOWST', 'Others', false));

            if (IsGuid($('#selFOWST').val())) {
                FowstID = $('#selFOWST').val();
                sessionStorage.FowstID = $('#selFOWST').val();
                sessionStorage.SelectedFOWST = $('#selFOWST').val();
                sessionStorage.FowstName = $('#selFOWST :selected').text();
                HandleEnforceZRead();
            } // 2017_01_20__10_58_22_3861 braces added by Arvin

            try {
                $('#selFOWST').selectmenu('refresh');
            }
            catch (e) {
                $('#selFOWST').selectmenu();
            }
        }

        window.GetNamedFowstBin = function () {
            //i use sessionStorage.FowstID cause this form is used in pos-inventory count mode
            //
            var data = {
                FowstID: sessionStorage.FowstID,
                ProductID: ProductID,
                NoFilter: 1,
                AreaID: AreaID
            }

            if (OfflineMode) {
                var mData = '';
                mData = InventoryCount.LibraryFowstBin(data);

                if (CBool(sessionStorage.IsCountByArea)) {
                    DisplayFowstBinByArea(mData.Rows)
                } else {
                    DisplayFowstBin(mData)
                }                
            }
            else {
                AjaxSend({
                    url: 'Browser/LibraryFowstBin',
                    data: data,
                    success: function (vData) {
                        DisplayFowstBin(vData.Results);
                    }
                });
            }
        }

        //function GetAllPriceLevels() {
        //    var pData = { OrgID: sessionStorage.OrgID }

        //    AjaxSend({
        //        url: 'ShelfAndBinLabeling/GetPriceLevel',
        //        data: pData,
        //        success: function (vData) {
        //            DisplayPriceLevels(vData.Results.Rows)
        //        }
        //    });
        //}

        //function DisplayPriceLevels(vRows) {
        //    var mData = [];

        //    for (var i = 0; i <= vRows.length - 1; i++) {
        //        mData.push({
        //            Text: vRows[i].PriceLevel,
        //            data: vRows[i],
        //            Click: function () {
        //                PriceLevelID = $(this).attr('data').ID;
        //                $('#lblPriceLevel').text('Price Level: ' + $(this).attr('data').PriceLevel);
        //                if (AreaID == undefined) {
        //                    GetAllFowstArea();
        //                } else {
        //                    GetAllFowstBinProducts();
        //                }

        //            }
        //        })
        //    }

        //    PopupButtonList('', 'Price Levels',
        //                mData, { AutoDismiss: true })
        //}


        function GetAllFowstArea() {
            data = { FowstID: sessionStorage.FowstID }

            var mData = InventoryCount.GetFowstArea(data);
            DisplayAllFowstArea(mData.Rows);
        }

        function DisplayAllFowstArea(vRows) {
            var mData = [];

            mData.push({
                Text: 'All',
                data: null,
                Click: function () {
                    AreaID = 'All';
                    $('#lblArea').text('Area: All');
                    //if (PriceLevelID == undefined) {
                    //    GetAllPriceLevels();
                    //} else {
                        GetAllFowstBinProducts();
                    //}
                }
            })


            for (var i = 0; i <= vRows.length - 1; i++) {
                mData.push({
                    Text: vRows[i].AreaName,
                    data: vRows[i],
                    Click: function () {
                        AreaID = $(this).attr('data').ID;
                        AreaName = $(this).attr('data').AreaName
                        $('#lblArea').text('Area: ' + AreaName);
                        //if (PriceLevelID == undefined) {
                        //    GetAllPriceLevels();
                        //} else {
                            GetAllFowstBinProducts();
                        //}
                    }
                })
            }

            PopupButtonList('', 'FOWST Area',
                        mData, { AutoDismiss: true })
        }

        function DisplayFowstBinByArea(vRows) {
            var mData = [];

            mData.push({
                Text: 'All',
                data: null,
                Click: function () {
                    BinID = 'All';
                    $('#lblBinAddress').text('Bin Address: All');
                    FilterAreaBinTable('None');
                }
            })


            for (var i = 0; i <= vRows.length - 1; i++) {
                mData.push({
                    Text: vRows[i].Name,
                    data: vRows[i],
                    Click: function () {
                        BinID = 'All';
                        $('#lblBinAddress').text('Bin Address: ' + $(this).attr('data').Name);
                        FilterAreaBinTable($(this).attr('data').Name);
                    }
                })
            }

            PopupButtonList('', 'Bin Address',
                        mData, { AutoDismiss: true })
        }

        function GetAllFowstBinProducts() {
            var pData = {};
            pData.FowstID = sessionStorage.FowstID;
            pData.AreaID = AreaID;

            var mData = InventoryCount.GetAllFowstBinProducts(pData);
            DisplayFowstBinProducts(mData.Rows);
        }

        function DisplayFowstBinProducts(vData) {

            if ($.fn.DataTable.isDataTable(FowstBinProductTable)) {
                //empty the table first before destroying datatabase para dili magdoble ang header
                FowstBinProductTable.empty();
                fowstbinproductDatatable.destroy();
            }

            fowstbinproductDatatable = FowstBinProductTable.DataTable({
                "data": vData,
                "columns": [
                    { "data": "Name" },
                    { "data": "Description" },
                    { "data": "ProductName" },
                ],
                "sDom": '<"top"l>rt<"bottom"ip><"clear">' //hide search label and field
            });
        }

        function DisplayFowstBin(vData) {

            if ($.fn.DataTable.isDataTable(BinsTable)) {
                //empty the table first before destroying datatabase para dili magdoble ang header
                BinsTable.empty();
                binDatatable.destroy();
            }

            binDatatable = BinsTable.DataTable({
                "data": vData.Rows,
                "columns": [
                    { "data": "AreaName" },
                    { "data": "Name" },
                    { "data": "Description" },
                    {
                        "data": function (data, type, full, meta) {
                            if (CBool(data.IsActive)) {
                                return 'Active';
                            } else {
                                return 'Inactive';
                            }
                        }
                    },
                    {
                        "data": function (data, type, full, meta) {
                            return 'To be deployed';
                        }
                    },
                    { "data": "ID" }
                ],                               
                //"language": {
                //    "search": "Search Bin "
                //},
                "columnDefs": [
                {
                    "targets": [ 5 ],
                    "visible": false,
                    "searchable": true
                }],
                "sDom": '<"top"l>rt<"bottom"ip><"clear">' //hide search label and field
                //"fnFooterCallback": function (nRow, aaData, iStart, iEnd, aiDisplay) {
                //    if (aiDisplay.length == 1) {
                //        $('#tblNamedBins').click();
                //    }
                //}
            });
            
            FilterBinTable('Active');
        }

        //function DisplayFowstBin(vData) {
        //    $('#selBinsStockViewer').empty();
        //    $('#selBinsStockViewerMultipleSize').empty();
        //    $('#selBinsByUPC').empty();
            
        //    $('#selBins').empty();

        //    if (GroupSelected == 'ByBin')
        //        $('#selGroup').append(new Option('Select Shelf/Bin', '', false));

        //    $('#selBinsStockViewer').append(new Option('Select Shelf/Bin', '', false));
        //    $('#selBinsStockViewerMultipleSize').append(new Option('Select Shelf/Bin', '', false));
        //    $('#selBins').append(new Option('Select Shelf/Bin', '', false));
        //    $('#selBinsByUPC').append(new Option('Select Shelf/Bin', '', false));
            
        //    var pLV = $("#lstBins");
            
        //    for (var i = 0; i <= vData.Rows.length - 1; i++) {
        //        if (vData.Rows[i].Name != 'Unnamed Bin') {
        //            if (GroupSelected == 'ByBin') {
        //                $('#selGroup').append(new Option(vData.Rows[i].Name, vData.Rows[i].ID, false));
        //            }

        //            $('#selBinsStockViewer').append(new Option(vData.Rows[i].Name, vData.Rows[i].ID, false));
        //            $('#selBinsStockViewerMultipleSize').append(new Option(vData.Rows[i].Name, vData.Rows[i].ID, false));
        //            $('#selBins').append(new Option(vData.Rows[i].Name, vData.Rows[i].ID, false));
        //            $('#selBinsByUPC').append(new Option(vData.Rows[i].Name, vData.Rows[i].ID, false));

        //            //---------------------05/16/06

        //            pItem = $('<tr>')
        //                       .append($('<th>')
        //                           .text(vData.Rows[i].Name)
        //                       )//td                               

        //            pLV.append(pItem)
        //        }
        //    }

        //    if (GroupSelected == 'ByBin') {
        //        $('#selBinsStockViewer').val($('#selGroup').val());
        //    }

        //    $('#selBins').append(new Option("Create New Bin", 'CreateNewBin', false));
        //    $('#selBinsStockViewer').append(new Option("Create New Bin", 'CreateNewBin', false));
        //    $('#selBinsStockViewerMultipleSize').append(new Option("Create New Bin", 'CreateNewBin', false));
        //    $('#selBinsByUPC').append(new Option("Create New Bin", 'CreateNewBin', false));
        //    //$('#selBins').val(FowstBinID);

        //    try {
        //        $('#selGroup').selectmenu('refresh');
        //        $('#selBins').selectmenu('refresh');
        //        $('#selBinsStockViewer').selectmenu('refresh');
        //        $('#selBinsStockViewerMultipleSize').selectmenu('refresh');
        //        $('#selBinsByUPC').selectmenu('refresh');
        //    }
        //    catch (e) {
        //        $('#selGroup').selectmenu();
        //        $('#selBinsStockViewer').selectmenu();
        //        $('#selBinsStockViewerMultipleSize').selectmenu();
        //        $('#selBins').selectmenu();
        //        $('#selBinsByUPC').selectmenu();

        //    }
        //}

        function GetFowstBin(m_Bin, txtField) {
            SetFowstID();
            Browser.LibraryFowstBin.InputData.FowstID = FowstID;
            
            window.pageSourcePage = $.mobile.activePage;
            Browser.LibraryFowstBin.Click = function (e) {
                FowstBinID = e.ID;

                if (txtField == '') {
                    //loading all items of bin
                    $('#txtBINSearch').val(e.Name);
                    GetAllBinStock(e.ID);
                }
                else {
                    //set bin name to textfield
                    $('#' + txtField).val(e.Name);
                }

            };

            Browser.LibraryFowstBin.Show(m_Bin);
        }

        window.GetFowstBinProductMaxMinCap = function () {
            var vFowstID;

            if (ProductID == undefined || ProductID == '') {
                ProductID = proj.ProductID
            }

            if (FowstBinID == undefined || FowstBinID == '') {
                FowstBinID = sessionStorage.FowstBinID
            }


            if (CBool(sessionStorage.IsInventoryCountMode)) {
                vFowstID = sessionStorage.CustomerOrganizationID;
            } else {
                vFowstID = sessionStorage.FowstID;
            }

            var data = { FowstID: vFowstID,
                BinID: FowstBinID,
                ProductID: ProductID
            }

            if (OfflineMode) {
                var mData = '';
                mData = InventoryCount.FowstBinProductMaxMinCap(data);

                if (mData.Rows.length > 0) {
                    $('#txtBinMaxCap').val(mData.Rows[0].MaxPerProduct);
                    $('#txtBinMinCap').val(mData.Rows[0].MinPerProduct);
                }
                else {
                    $('#txtBinMaxCap').val(0);
                    $('#txtBinMinCap').val(0);
                }
            }
            else {
                if (ProductID == undefined || FowstBinID == undefined) {
                    return;
                }

                AjaxSend({
                    url: 'Browser/LibraryFowstBinProduct',
                    data: data,
                    success: function (vData) {
                        if (vData.Results.Rows.length > 0) {
                            $('#txtBinMaxCap').val(vData.Results.Rows[0].MaxPerProduct);
                            $('#txtBinMinCap').val(vData.Results.Rows[0].MinPerProduct);
                        }
                        else {
                            $('#txtBinMaxCap').val(0);
                            $('#txtBinMinCap').val(0);
                        }
                    }
                });
            }
        }

        function GetAllBinStock(m_ID) {
            var data = { BinID: m_ID }

            AjaxSend({
                url: 'InventoryCount/GetAllBinStock',
                data: data,
                success: function (vData) {
                    $.mobile.changePage('#pageBinContent');
                    DisplayBinStock(vData.BinStock.Rows);
                }
            });
        }

        //        function DisplayBinStock(vRows) {
        //            var pLV = $("#lstBinItems");

        //            pLV.empty();

        //            for (var i = 0; i <= vRows.length - 1; i++) {
        //                //unit measure
        //                pItem = $('<li>')
        //                    .append($('<a>')
        //                        .attr('ID', vRows[i].ID)
        //                        .attr('href', '#pageStockViewer')
        //                        .data('data', vRows[i])
        //                        .click(function (e) {
        //                            $('#lblStockNumber').text($(this).data('data').StockNumber);
        //                            $('#lblStockDescription').text($(this).data('data').StockDescription);
        //                            $('#txtStockViewerBIN').val($('#txtBINSearch').val());
        //                            ProductID = $(this).data('data').ProductID;
        //                            $('#lstCounts').empty();
        //                            GetStockUnitMeasures($(this).data('data').ProductID);
        //                        })
        //                        .append($('<h3>')
        //                            .text(vRows[i].ProductName)
        //                        )
        //                    )

        //                pLV.append(pItem)

        //            }

        //            pLV.listview('refresh');

        //        }

        //        function GetStockSizes(ProductNumber) {
        //            var data = { keywords: ProductNumber }

        //            AjaxSend({
        //                url: 'Browser/LibraryPIMProduct',
        //                data: data,
        //                success: function (vData) {
        //                    DisplayProductSizes(vData.Results.Rows);
        //                }
        //            });
        //        }

        function DisplayProductSizes(vRows) {

            var UnitMeasure = '';

            var data = { ProductID: vRows[0].ID }
            var vRowsUnitMeasure;

            if (OfflineMode) {
                var mData = '';
                mData = InventoryCount.GetStockUnitMeasures(data);
                DisplayStockUnitMeasures(vRows, mData)
            }
            else {
                AjaxSend({
                    url: 'Receiving/GetStockUnitMeasures',
                    data: data,
                    success: function (vData) {
                        vRowsUnitMeasure = vData.UnitMeasure;
                        DisplayStockUnitMeasures(vRows, vRowsUnitMeasure)
                    }
                });
            }
        }



        function DisplayStockUnitMeasures(vRows, vRowsUnitMeasure) {
            var pLV = $("#lstSizesUnitMeasure");

            $("#tblSizesUnitMeasureFrame tbody tr").remove();


            //sizes
            for (var i = 0; i <= vRows.length - 1; i++) {
                var m_ProductID = vRows[i].ID;

                //unit measure
                for (var j = 0; j <= vRowsUnitMeasure.Rows.length - 1; j++) {
                    pItem = $('<tr>')
                                .append($('<th>')
                                    .text(vRows[i].ProductName)
                                )//td
                                .append($('<td>')
                                    .text(vRows[i].Size)
                                )//td
                                .append($('<td>')
                                    .text(vRowsUnitMeasure.Rows[j].UnitMeasure)
                                )//td
                                .append($('<td>')
                                    .append($('<input type=number />')
                                    .attr('ID', vRowsUnitMeasure.Rows[j].ID)
                                    .attr('ProductID', m_ProductID)
                                    .data('data', vRowsUnitMeasure.Rows[j])
                                    .attr('class', 'ui-input-text ui-corner-all ui-btn-shadow ui-body-c')
                                        )
                                )//td
                }

                pLV.append(pItem)

            }
        }


        window.GetStockUnitMeasures = function (m_ID) {
            var data = { ProductID: m_ID }

            if (OfflineMode) {
                var mData = '';
                mData = InventoryCount.GetStockUnitMeasures(data);
                $.mobile.changePage('#pageStockViewer');
                DisplayUnitMeasures(mData);
            }
            else {
                AjaxSend({
                    url: 'Receiving/GetStockUnitMeasures',
                    data: data,
                    success: function (vData) {
                        $.mobile.changePage('#pageStockViewer');
                        DisplayUnitMeasures(vData.UnitMeasure);
                    }
                });
            }

        }

        function DisplayUnitMeasures(vRows) {
            var pLV = $("#lstUnitMeasure");

            var BaseUnitMeasure = vRows.Rows[0].UnitMeasure;

            $("#lstUnitMeasure tbody tr").remove();

            for (var i = 0; i <= vRows.Rows.length - 1; i++) {
                //                unit measure               

                pItem = $('<tr>')
                            .append($('<td>')
                                .append($('<label for=' + vRows.Rows[i].UnitMeasure + '>')
                                .text(vRows.Rows[i].UnitMeasure + ' (' + vRows.Rows[i].Quantity + ')')
                                    )
                            )//td
                            .append($('<td>')
                                .append($('<input type=number />')
                                .attr('ID', 'txt' + vRows.Rows[i].ID)
                                .attr('name', vRows.Rows[i].UnitMeasure)
                                .data('data', vRows.Rows[i])
                                .attr('class', 'ui-input-text ui-corner-all ui-btn-shadow ui-body-c')
                                .val(vRows.Rows[i].QuantityGood)
                                    )
                            )//td                            

                pLV.append(pItem)

            }

            try {
                $('#lstUnitMeasureFrame').listview('refresh');
            }
            catch (e) {
                $('#lstUnitMeasureFrame').listview();
            }


        }

        function GetAllInventoryCount(m_data) {
            data = {
                data: m_data
            }
            AjaxSend({
                url: 'InventoryCount/GetAllInventoryCount',
                data: data,
                success: function (vData) {
                    DisplayInventoryCount(vData.InventoryCount.Rows)
                }
            });
        }

        function GetInventoryCountStocks(vID) {
            var data = { InventoryCountID: vID }
            AjaxSend({
                url: 'InventoryCount/GetInventoryCountStocks',
                data: data,
                success: function (vData) {
                    DisplayInventoryCountStocks(vData.InventoryCount.Rows)
                }
            });
        }

        function DisplayInventoryCount(vRows) {
            var pLV = $("#lstCountList");

            pLV.empty();

            for (var i = 0; i <= vRows.length - 1; i++) {
                pItem = $('<li>')
                    .attr('ID', vRows[i].ID)
                    .append($('<a>')
                        .attr('ID', vRows[i].ID)
                        .data('data', vRows[i])
                        .click(function (e) {
                            $('#lblFowstInfo').text($(this).data('data').Fowst);
                            $('#lblStockNumberInfo').text($(this).data('data').StockNumber);
                            $('#lblStockDescriptionInfo').text($(this).data('data').StockDescription);
                            $('#lblDateCountedInfo').text($(this).data('data').InventoryCountDate);

                            $.mobile.changePage('#pageCountInformation');

                            GetInventoryCountStocks($(this).attr('ID'));
                        })
                        .append($('<h3>')
                            .text('Fowst: ' + vRows[i].Fowst)
                        )
                        .append($('<p>')
                            .text("Item : " + vRows[i].StockDescription)
                        )
                        .append($('<p>')
                            .text("Date Counted : " + vRows[i].InventoryCountDate)
                        )
                    )
                pLV.append(pItem);
            }

            pLV.listview('refresh');
        }

        function DisplayInventoryCountStocks(vRows) {
            var pLV = $("#lstCountInformationDetails");

            pLV.empty();

            for (var i = 0; i <= vRows.length - 1; i++) {
                pItem = $('<li>')
                    .attr('ID', vRows[i].ID)
                    .append($('<a>')
                        .attr('ID', vRows[i].ID)
                        .data('data', vRows[i])
                        .click(function (e) {
                        })
                        .append($('<h3>')
                            .text('Shelf/Bin: ' + vRows[i].Name)
                        )
                        .append($('<p>')
                            .text("UnitMeasure : " + vRows[i].UnitMeasure)
                        )
                        .append($('<p>')
                            .text("Quantity : " + vRows[i].Quantity)
                        )
                    )
                pLV.append(pItem);
            }

            pItem = $('<li>')
                    .append($('<a>')
                        .click(function () {
                            GetAllInventoryCount();
                            $.mobile.changePage('#pageIndex');
                        })
                        .append($('<h3>')
                            .text('Back')
                        )
                    )
            pLV.append(pItem);
            pLV.listview('refresh');
        }

        function SetFowstID() {
            data = {
                FowstID: FowstID,
                ProductID: ProductID
            }
            AjaxSend({
                url: 'InventoryCount/SetFowstID',
                data: data,
                success: function (vData) {
                }
            });
        }

        function SetProductID() {
            data = {
                ProductID: ProductID
            }
            AjaxSend({
                url: 'InventoryCount/SetProductID',
                data: data,
                success: function (vData) {
                }
            });
        }

        function Save(data, Count) {
            var IsSuccessful = false;

            if (OfflineMode) {
                IsSuccessful = InventoryCount.SaveInventoryCount(data);
                if (IsSuccessful) {
                    ClearFields();
                    if (!CBool(sessionStorage.IsInventoryCountMode)) {
                        MessageBox("Your count has been saved.",
                                "Save",
                                [
                                    {
                                        Text: 'Ok',
                                        Click: function () {
                                            if (!(window.location.hash.substring(1).indexOf('PageCountByUPC') >= 0 || window.location.hash.substring(1).indexOf('PageCountByTableType') >= 0)) {
                                                SetInventoryCountFormat();
                                                //setTimeout(function () {
                                                //    $.mobile.changePage('#pageGroup');
                                                //}, 3000);
                                            } else {
                                                //$('.ui-dialog').dialog('close');
                                                $.mobile.changePage('#')                                                
                                            }
                                        }
                                    }
                                ]);
                    }
                }
            }
            else {
                AjaxSend({
                    url: 'InventoryCount/SaveInventoryCount',
                    data: data,
                    success: function (vData) {
                        if (vData.Save == "True") {
                            if (!CBool(sessionStorage.IsInventoryCountMode)) {
                                MessageBox("Your count has been saved.",
                                "Save",
                                [
                                    { Text: 'Ok',
                                        Click: function () {
                                            //GotoFirstPage();
                                            ClearFields();
                                            SetInventoryCountFormat();
                                            //$.mobile.changePage('#pageGroup');
                                        }
                                    }
                                ]);
                            }
                        } else if (CStr2(vData.Save) != '') {
                            if (!CBool(sessionStorage.IsInventoryCountMode)) {
                                MessageBox(vData.Save, 'Not saved');
                            }
                        } //end if
                    }
                });
            }

            //            if (sessionStorage.IsInventoryCountMode) {
            //                UpdatePOSCart();
            //            }
        }

        function SetInventoryCountFormat() {

            //            if ($('#selFOWST').val() != 'Others') {
            //                FowstID = $('#selFOWST').val();
            //            }

            if (FowstID == '') {
                FowstID = $('#selFOWST').val();
                if (FowstID == '') {
                    MessageBox("Please select a FOWST", "FOWST",
                                [
                                    { Text: 'Ok' }
                                ]);
                    return;
                }
            }

            switch (GroupSelected) {
                case 'ByNegativeBalances':
                    {
                        IsHideGroup(true);
                        GetAllProducts();
                        $.mobile.changePage('#pageGroup');
                        break;
                    }
                case 'ByBin':
                    {                        
                        $.mobile.changePage('#pageCountByBinAndArea');
                        $('#pageCountByBin').css('visibility', 'visible');
                        $('#pageCountByBin').css('display', 'block');

                        $('#pageCountByArea').css('visibility', 'hidden');
                        $('#pageCountByArea').css('display', 'none');

                        SetByBinSettings();                        
                        break;
                    }
                case 'ByBrand':
                    {
                        $('#GroupTitle').html('Please specify the brand that you would like to count.');
                        $('#lblGroup').text('Brand: ');
                        IsHideGroup(false);
                        GetAllBrands();
                        $.mobile.changePage('#pageGroup');
                        break;
                    }
                case 'BySpatial':
                    {
                        $.mobile.changePage('#PageCategories');                        
                        //IsHideGroup(true);
                        //$.mobile.changePage('#pageGroup');
                        break;
                    }
                case 'ByRecentlyReceived':
                    {
                        $('#GroupTitle').html('What receiving and transfer period would you like to cover?');
                        $("#lstProducts").empty();

                        $('#dvGroup').css('visibility', 'hidden');
                        $('#dvGroup').css('display', 'none');
                        $('#GroupTitle').css('visibility', 'visible');
                        $('#GroupTitle').css('display', 'block');

                        $('#dvGroupDate').css('visibility', 'visible');
                        $('#dvGroupDate').css('display', 'block');
                        $.mobile.changePage('#pageGroup');
                        break;
                    }
                case 'ByUPCOrBarcode':
                    {
                        GetNamedFowstBin();
                        $.mobile.changePage('#PageCountByUPC')
                        
                        break;
                    }
                case 'ByTableType':
                    {
                        $.mobile.changePage('#PageCategories');                        
                        //GetBalancesFromServer();
                        GetCategories();                        
                        break;
                    }
                case 'ByArea':
                    {
                        
                        $.mobile.changePage('#pageCountByBinAndArea');

                        $('#pageCountByArea').css('visibility', 'visible');
                        $('#pageCountByArea').css('display', 'block');

                        $('#pageCountByBin').css('visibility', 'hidden');
                        $('#pageCountByBin').css('display', 'none');

                        SetByAreaSettings();
                        break;
                        
                    }
            }

        }

        function GetAllBrands() {
            //            if (OfflineMode) {
            //                var mData = '';
            //                mData = InventoryCount.GetBrands();
            //                DisplayBrands(mData);
            //            }
            //            else {
            AjaxSend({
                url: 'InventoryCount/GetAllBrands',
                success: function (vData) {
                    DisplayBrands(vData.InventoryCount);
                }
            });
            //            }
        }

        function DisplayBrands(vRows) {

            $('#selGroup').append(new Option('Select Brand', '', false));

            for (var i = 0; i <= vRows.Rows.length - 1; i++) {
                $('#selGroup').append(new Option(vRows.Rows[i].Brand, vRows.Rows[i].Brand, false));
            }
            try {
                $('#selGroup').selectmenu('refresh');
            }
            catch (e) {
                $('#selGroup').selectmenu();
            }
        }

        function GetAllProducts() {           
                data = { GroupType: GroupSelected,
                    FowstID: FowstID,
                    Filter: $('#selGroup').val(),
                    From: $('#txtDateFrom').DateCombo('val'),
                    To: $('#txtDateTo').DateCombo('val')
                }

                AjaxSend({
                    url: 'InventoryCount/GetAllProducts',
                    data: data,
                    success: function (vData) {
                        DisplayProducts(vData.InventoryCount.Rows);
                    }
                });
            
        }

        function DisplayProducts(vRows) {
            var pLV = $("#lstProducts");

            pLV.empty();

            for (var i = 0; i <= vRows.length - 1; i++) {
                pItem = $('<li>')
                    .attr('ID', vRows[i].ID)
                    .append($('<a>')
                        .append($('<img>')
                            .attr('src', 'Content/images/box.png'))
                        .attr('href', '#')
                        .attr('ID', vRows[i].ID)
                        .data('data', vRows[i])
                        .click(function (e) {
                            $('#lblStockNumber').text($(this).data('data').ProductNo);
                            $('#lblStockDescription').text($(this).data('data').ProductName);
                            $('#lstCounts').empty();

                            ProductID = $(this).attr('ID');
                            mAllowDecimal = $(this).data('data').AllowDecimal;
                            if (mAllowDecimal == undefined)
                                console.trace('Include AllowDecimal in server side query');
                            mAllowDecimal = CBool(mAllowDecimal);

                            if (GroupSelected == 'ByBin') {
                                FowstBinID = $('#selGroup').val();
                            }

                            GetStockUnitMeasures(ProductID);
                        })
                        .append($('<h3>')
                            .text(vRows[i].ProductNo)
                        )
                        .append($('<h4>')
                            .text(vRows[i].ProductName)
                        )
                    )
                pLV.append(pItem);
            }
            pLV.listview('refresh');
        }

        function UpdateFOWSTBinProductMaxMinCap() {
            if (proj.ProductID != undefined) {
                ProductID = proj.ProductID;
            }

            var vFowstID = '';

            if (CBool(sessionStorage.IsInventoryCountMode)) {
                vFowstID = sessionStorage.CustomerOrganizationID;
            } else {
                vFowstID = sessionStorage.FowstID;
            }

            var data = { FowstID: vFowstID,
                BinID: FowstBinID,
                ProductID: ProductID,
                MaxCap: parseFloat($('#txtBinMaxCap').val()),
                MinCap: parseFloat($('#txtBinMinCap').val())
            }

            if (OfflineMode) {
                var mData = '';
                mData = InventoryCount.SaveFOWSTBinProductMaxMinCap(data);
            }
            else {
                AjaxSend({
                    url: 'InventoryCount/UpdateFowstBinProductMaxMinCap',
                    data: data,
                    success: function (vData) {
                        if (vData.Results != 'Success') {
                        }
                    }
                });
            }
        }

        function GetAllProducts() {
            if ($('#selGroup option:selected').val() == '' && GroupSelected == 'ByBrand' || $('#selGroup option:selected').val() == '' && GroupSelected == 'ByBin') {
                var m_type = '';

                switch (GroupSelected) {
                    case 'ByBin':
                        {
                            m_type = 'Shelf/Bin Location';
                            break;
                        }
                    case 'ByBrand':
                        {
                            m_type = 'Brand';
                            break;
                        }
                };

                MessageBox("Please select " + m_type,
                                "Inventoy Count",
                                [
                                    { Text: 'Ok' }
                                ]
                            );
            }
            else {
                data = {
                    GroupType: GroupSelected,
                    FowstID: FowstID,
                    Filter: $('#selGroup').val(),
                    From: $('#txtDateFrom').DateCombo('val'),
                    To: $('#txtDateTo').DateCombo('val')
                }

                AjaxSend({
                    url: 'InventoryCount/GetAllProducts',
                    data: data,
                    success: function (vData) {
                        DisplayProducts(vData.InventoryCount.Rows);
                    }
                });
            }
        }

        function GetCategories() {
            if (IsMobile) {
                mData = InventoryCount.GetCategories();
                DisplayCategories(mData.Rows);
                DisplayCategoriesPopupMode(mData.Rows);
            }
        }

        function GetAllProductsForTableType(Feature, vSearch) {
            if (IsMobile) {
                //$.mobile.changePage('#PageCountByTableType');
                
                mData = InventoryCount.GetAllProducts(Feature, vSearch, true);

                ProductsInCategoriesRows = mData.Rows;

                DisplayAllProductsForTableType();
            }        
        }

        function DisplayCategoriesPopupMode(vRows) {

            var mCategories = [];


            for (var i = 0; i <= vRows.length - 1; i++) {
                mCategories.push({
                    Text: vRows[i].Category,
                    Click: function () {
                        data = {
                            FOWSTID: sessionStorage.SelectedFOWST,
                            DisplayType: 'fowst',
                            ProductID: ProductID,
                            Category: $(this)[0].Text
                        }

                        $('.lblSelectedCategory').text('Category: ' + $(this)[0].Text)
                        $('#txtSearchProducts').val('');
                        CategoryType = $(this)[0].Text;
                        GetAllProductsForTableType($(this)[0].Text, '');

                    }
                })
            }

            PopupButtonList('', 'Categories',
                    mCategories, { AutoDismiss: true })
        }

        function DisplayCategories(vRows) {
            var pLV = $('#lstCategory');

            $('#pLV').empty();

            for (var i = 0; i <= vRows.length - 1; i++) {

                pItem = $('<li>')
                        .data('data', vRows[i])
                        .append($('<a>')
                            .attr('ID', vRows[i].ID)
                            .data('data', vRows[i])
                            .append($('<h3>')
                                .text(vRows[i].Category)
                            )
                        )
                        .click(function () {
                            $('#txtSearchProducts').val('');
                            CategoryType = $(this).data('data').Category;
                            GetAllProductsForTableType($(this).data('data').Category,'');
                        })

                pLV.append(pItem);
            }

            try {
                pLV.listview('refresh');
            }
            catch (e) {
                pLV.listview();
            }
        }

        function DisplayAllProductsForTableType() {

            if ($.fn.DataTable.isDataTable(ProductsInCategoriesTable)) {
                //empty the table first before destroying datatabase para dili magdoble ang header
                ProductsInCategoriesTable.empty();                
                ProductsInCategoriesDatatable.destroy();
            }


            ProductsInCategoriesDatatable = ProductsInCategoriesTable.DataTable({
                pageLength: 25,
                "ordering": false,
                "data": ProductsInCategoriesRows,
                "columns": [
                    {
                        "data": function (data) {
                            return data.ProductNo + '-' + data.ProductName
                        }
                    },
                    {
                        "data": function (data) {
                            return data.UnitMeasure
                        }
                    },
                    {
                        "data": function (data) {
                            return "<input type=number />";
                        }
                    },
                    {
                        "data": function (data) {
                            return AccountingFormat(data.balance)
                        }
                    },
                    {
                        "data": function (data) {
                            return AccountingFormat(data.LastCount)
                        }
                    },
                    {
                        "data": function (data) {
                            return '-'
                        }
                    }
                    //,
                    //{
                    //    "data": function (data) {
                    //        return "<button class='btnRemarks' type='button'>Add</button>";
                    //    }
                    //}
                ],
                "bFilter": false
            });

            try {
                $(".btnRemarks").button().button('refresh');
            } catch (e) {
                $(".btnRemarks").button().button();
            }

            //pLV = $('#tblBody');

            //pLV.html('');

            //for (var i = 0; i <= vRows.length - 1; i++) {

            //    var balance = 0;

            //    if (ProductEndingBalance != undefined) {
            //        for (j = 0; j <= ProductEndingBalance.Rows.length - 1; j++) {
            //            if (ProductEndingBalance.Rows[j].ProductID == vRows[i].ID) {
            //                balance = CDec2(ProductEndingBalance.Rows[j].Balance);
            //                break;
            //            }
            //        }
            //    } else {
            //        balance = 'N/A';
            //    }
                
                
            //    pItem = $('<tr>')
            //            .attr('ID', 'td' + vRows[i].ID)
            //            .data(vRows[i])
            //            .append($('<th>')
            //                .append($('<span>')
            //                    .text(vRows[i].ProductNo + '-' + vRows[i].ProductName)
            //                )
            //            )
            //            .append($('<td>')
            //                .append($('<span>')
            //                    .text(vRows[i].UnitMeasure)
            //                )
            //            )
            //            .append($('<td>')
            //                .append($('<input type=number ID = ' + vRows[i].ID + ' />')
            //                .data('data',vRows[i])
            //                .attr('class', 'ui-input-text ui-corner-all ui-btn-shadow ui-body-c')
            //                .attr('previouscount', vRows[i].LastCount)
            //                .attr('serverBalance', balance)
            //                .attr('tabindex', i)
            //                .click(function (e) {
            //                    e.stopPropagation();
            //                })
            //                .keypress(function (event) {
            //                	if (event.keyCode == 9 || event.keyCode == 13 || event.which == 9 || event.which == 13) {
            //                		$(this).parent().parent().next().children().children('input').focus();
            //                	}
            //                })
            //                .keydown(function (event) {
            //                	if (event.keyCode == 9 || event.keyCode == 13 || event.which == 9 || event.which == 13) {
            //                		event.preventDefault();
            //                		$(this).parent().parent().next().children().children('input').focus();
            //                	}
            //                })                            
            //                .focusout(function () {
            //                    if ($(this).attr('serverBalance') != 'N/A') {
            //                        if (CDec2($(this).attr('serverBalance')) != CDec2($(this).val())) {
            //                            $('#td' + $(this).attr('ID')).css('background-color', 'yellow')
            //                            $('#lblVariance' + $(this).attr('ID')).text(CDec2($(this).val()) - CDec2($(this).attr('serverBalance')))
            //                        } else {
            //                            $('#td' + $(this).attr('ID')).css('background-color', '')
            //                            $('#lblVariance' + $(this).attr('ID')).text('-')
            //                        }
            //                    }                                
            //                })
            //                )                            
            //            )
            //            .click(function (event) {
                            
            //                $('#panSelectInventoryBalance').popup('open');                            
            //                    //substr(2) to remove td in ID
            //                    $('#lblPopUpProductID').text($(this).attr('ID').substr(2));
            //                    $('#lblPopUpProductName').text($(this).data().ProductName);
            //                    GetBinsBalances($(this).attr('ID').substr(2));
            //            })
            //            .append($('<td style="text-align:right">')
            //                .append($('<span>')
            //                    .attr('ID', vRows[i].ID)
            //                    .text(balance)
            //                )
            //            )
            //            .append($('<td style="text-align:right">')
            //                .append($('<span>')
            //                    .text(vRows[i].LastCount)
            //                )
            //            )
            //            .append($('<td style="text-align:right">')
            //                .append($('<span>')
            //                    .attr('ID', 'lblVariance' + vRows[i].ID)
            //                    .text('-')
            //                )
            //            )
            //            .append($('<td style="text-align:center">')
            //                .append($("<button class='btnRemarks' type='button'>Add</button>")
            //                    .attr('ID', vRows[i].ID)
            //                    .click(function () {
            //                        SelectedProduct = $(this).attr('ID');
            //                        remarkPopup($(this).attr('ID'))
            //                    })
            //                )
            //            )
                        
                
            //    try {
            //        $(".btnRemarks").button().button('refresh');
            //    } catch (e) {
            //        $(".btnRemarks").button().button();
            //    }                

            //    pLV.append(pItem)

            //}

            //$(".btnRemarks").button().button('refresh');

            function remarkPopup(prodID) {
                $('#panRemarkPopup').popup('open');
                $('#remarkText')
                .blur(function () {
                    $('#remarkText').unbind('input propertychange');
                })
                .focus(function () {
                $('#remarkText')
                    .bind('input propertychange', function () {
                        var charCount = $('#remarkText').val().length;
                        var finalCharCount = 250 - charCount;
                        $('#remarkCount').text(finalCharCount + " char left");
                    });
                });
                //Check and load the remarks in the text area
                if (document.getElementById('td'+ SelectedProduct).hasAttribute("remarks")) {
                	$('#remarkText').val($('#td' + SelectedProduct).attr('remarks'));
                	var charCount = $('#remarkText').val().length;
                    var finalCharCount = 250 - charCount;
                    $('#remarkCount').text(finalCharCount + " char left");
                } else {
                	$('#remarkText').val('');
                	$('#remarkCount').text("250 char left");
                }


                $('#btnSaveRemark').click(function () {
                    $('#td' + SelectedProduct).attr('remarks', $('#remarkText').val());
                   $('#panRemarkPopup').popup('close');
                    //Check wheather there is a value in the remarks or not
                    //If there is, change the button text to View
                    var inp = $("#remarkText").val();
					if($.trim(inp).length > 0) {
	                    $('#td' + SelectedProduct).children().children().children('button').html('View');
	                    $(".btnRemarks").button().button('refresh');
                    } else {
                    	$('#td' + SelectedProduct).children().children().children('button').html('Add');
	                    $(".btnRemarks").button().button('refresh');
                    } 
                });

                $('#btnCancelRemark').click(function () {
                    $('#panRemarkPopup').popup('close');
                })

                $('#btnClearRemark').click(function () {
                    $('#remarkText').val()
                });

            }

            try {
                $('#tblBody').parent().table().table('refresh');
            } catch (e) {
                //$('#tblBody').parent().table().table('refresh');
            }
            

        }

        function SaveCountFromTableList(isGoToTurnOverItemPage) {
            var Count = [];
            var vBinID;
            var data;
            var mData;
            var BatchID = NewID();
            var vBinIDNew;

            if (FowstID == '') {
                FowstID = $('#selFOWST').val();
            }
            

            for (var i = 0; i <= ProductsInCategoriesDatatable.rows().data().length - 1; i++) {

                if (CDec2(ProductsInCategoriesDatatable.cell(i, 2).nodes().to$().find('input').val())) {


                    mData = {
                        ProductID: ProductsInCategoriesDatatable.row(i).data().ID,
                        FowstID: FowstID
                    }


                    vBinID = InventoryCount.GetDefaultBinID(mData);
                    if (vBinID.Rows[0] == undefined) {
                        vBinID = InventoryCount.GetRandomBin(mData);
                        if (vBinID.Rows[0] != undefined) {
                            vBinIDNew = vBinID.Rows[0].ID;
                        }
                    }
                    else {
                        vBinIDNew = vBinID.Rows[0].ID;
                    }

                    if (vBinIDNew == undefined) {
                        vBinIDTemp = NewID();
                        var pSql = "INSERT INTO LibraryFowstBin(ID, FowstID, Name, Description) VALUES('" + vBinIDTemp + "', '" + FowstID + "', 'Unnamed Bin', 'Unnamed Bin')";
                        Sql.Execute(pSql);
                    }

                    data = {
                        ID: ID,
                        IsNew: IsNew,
                        FowstID: FowstID,
                        ProductID: ProductsInCategoriesDatatable.row(i).data().ID,
                        Count: Count,
                        BatchID: BatchID
                    }

                    Count[0] = {
                        BinID: (vBinIDNew || vBinIDTemp),
                        ExpirationDate: 'N/A',
                        UnitMeasureID: ProductsInCategoriesDatatable.row(i).data().MeasurementID,
                        Quantity: CDec2(ProductsInCategoriesDatatable.cell(i, 2).nodes().to$().find('input').val()),
                        ActualQuantity: 1,
                        IsGoodStock: 1,
                        ExpectedValue: (ProductsInCategoriesDatatable.row(i).data().serverBalance == 'N/A' ? 'NULL' : ProductsInCategoriesDatatable.row(i).data().serverBalance), //serverBalance
                        Variance: (CDec2(ProductsInCategoriesDatatable.cell(i, 2).nodes().to$().find('input').val()) - CDec2(ProductsInCategoriesDatatable.row(i).data().serverBalance) || 'NULL'),
                        Remarks: ''
                    };

                    //$('#' + $(this).attr('ID').replace('txt', '')).css('visibility', 'hidden');
                    //$('#' + $(this).attr('ID').replace('txt', '')).css('display', 'none');

                    //$('#' + $(this).attr('ID')).val('');

                    //Save(data, Count);

                    var IsSuccessful = false;

                    if (OfflineMode) {
                        IsSuccessful = InventoryCount.SaveInventoryCount(data);
                    }

                    if (!IsSuccessful) {
                        //error message
                        return;
                    }
                }
            }

            //$('#tblProductsInCategories tbody tr td input').each(function (mindex) {
            //    if ($(this).val() != '') {
            //        var vBinIDTemp;
            //        var vBinIDNew;

            //        if ($(this).attr('Bins') != undefined) {
            //            var mBins = $(this).attr('Bins').split(',');
            //            var mBinsValue = $(this).attr('BinsValue').split(',');

            //            for (var i = 0; i < mBins.length; i++) {
            //                data = {
            //                    ID: ID,
            //                    IsNew: IsNew,
            //                    FowstID: FowstID,
            //                    ProductID: $(this).attr('ID').replace('txt', ''),
            //                    Count: Count,
            //                    BatchID: BatchID
            //                }

            //                Count[0] = {
            //                    BinID: mBins[i],
            //                    ExpirationDate: 'N/A',
            //                    UnitMeasureID: $(this).data('data').MeasurementID,
            //                    Quantity: mBinsValue[i],
            //                    ActualQuantity: 1,
            //                    IsGoodStock: 1,
            //                    ExpectedValue: ($(this).attr('serverBalance') == 'N/A' ? 'NULL' : $(this).attr('serverBalance')), //serverBalance
            //                    Variance: (CDec2(mBinsValue[i]) - CDec2($(this).attr('serverBalance')) || 'NULL'),
            //                    Remarks: $('#td' + $(this).attr('ID').replace('txt', '')).attr('remarks')
            //                };

            //                if (OfflineMode) {
            //                    IsSuccessful = InventoryCount.SaveInventoryCount(data);
            //                }
            //            }
            //        } else {

            //            mData = {
            //                ProductID: $(this).attr('ID').replace('txt', ''),
            //                FowstID: FowstID
            //            }


            //            vBinID = InventoryCount.GetDefaultBinID(mData);
            //            if (vBinID.Rows[0] == undefined) {
            //                vBinID = InventoryCount.GetRandomBin(mData);
            //                if (vBinID.Rows[0] != undefined) {
            //                    vBinIDNew = vBinID.Rows[0].ID;
            //                }
            //            }
            //            else {
            //                vBinIDNew = vBinID.Rows[0].ID;
            //            }

            //            if (vBinIDNew == undefined) {
            //                vBinIDTemp = NewID();
            //                var pSql = "INSERT INTO LibraryFowstBin(ID, FowstID, Name, Description) VALUES('" + vBinIDTemp + "', '" + FowstID + "', 'Unnamed Bin', 'Unnamed Bin')";
            //                Sql.Execute(pSql);
            //            }

            //            data = {
            //                ID: ID,
            //                IsNew: IsNew,
            //                FowstID: FowstID,
            //                ProductID: $(this).attr('ID').replace('txt', ''),
            //                Count: Count,
            //                BatchID: BatchID
            //            }

            //            Count[0] = {
            //                BinID: (vBinIDNew || vBinIDTemp),
            //                ExpirationDate: 'N/A',
            //                UnitMeasureID: $(this).data('data').MeasurementID,
            //                Quantity: CDec2($(this).val()).toString(),
            //                ActualQuantity: 1,
            //                IsGoodStock: 1,
            //                ExpectedValue: ($(this).attr('serverBalance') == 'N/A' ? 'NULL' : $(this).attr('serverBalance')), //serverBalance
            //                Variance: (CDec2($(this).val()) - CDec2($(this).attr('serverBalance')) || 'NULL'),
            //                Remarks: $('#td' + $(this).attr('ID').replace('txt', '')).attr('remarks')
            //            };

            //            $('#' + $(this).attr('ID').replace('txt', '')).css('visibility', 'hidden');
            //            $('#' + $(this).attr('ID').replace('txt', '')).css('display', 'none');

            //            $('#' + $(this).attr('ID')).val('');

            //            //Save(data, Count);
                    
            //            var IsSuccessful = false;

            //            if (OfflineMode) {
            //                IsSuccessful = InventoryCount.SaveInventoryCount(data);
            //            }

            //            if (!IsSuccessful) {
            //                //error message
            //                return;
            //            }
            //        }
            //    } else if(CDec2($(this).attr('previouscount'))==undefined) {
            //        mData = {
            //            ProductID: $(this).attr('ID').replace('txt', ''),
            //            FowstID: FowstID
            //        }

            //        vBinID = InventoryCount.GetDefaultBinID(mData);
            //        if (vBinID.Rows[0] == undefined) {
            //            vBinID = InventoryCount.GetRandomBin(mData);
            //        }

            //        data = {
            //            ID: ID,
            //            IsNew: IsNew,
            //            FowstID: FowstID,
            //            ProductID: $(this).attr('ID').replace('txt', ''),
            //            Count: Count,
            //            BatchID: BatchID
            //        }

            //        if(vBinID == undefined){
            //            return;
            //        }

            //        Count[0] = {
            //            BinID: vBinID.Rows[0].ID,
            //            ExpirationDate: 'N/A',
            //            UnitMeasureID: 'NULL',
            //            Quantity: 'NULL',
            //            ActualQuantity: 'NULL',
            //            IsGoodStock: 'NULL',
            //            ExpectedValue: 'NULL', //serverBalance
            //            Variance: 'NULL',
            //            Remarks: ''

            //        };

            //        $('#' + $(this).attr('ID').replace('txt', '')).css('visibility', 'hidden');
            //        $('#' + $(this).attr('ID').replace('txt', '')).css('display', 'none');

            //        $('#' + $(this).attr('ID')).val('');

            //        //Save(data, Count);

            //        var IsSuccessful = false;

            //        if (OfflineMode) {
            //            IsSuccessful = InventoryCount.SaveInventoryCount(data);
            //        }

            //        if (!IsSuccessful) {
            //            //error message
            //            return;
            //        }
            //    }
            //});

            if(CBool(isGoToTurnOverItemPage)){
                $.mobile.changePage('#PageMyCounts');
                GetAllMyCounts('true', CategoryType);
            }
            else {
                $.mobile.changePage('#');
            }
            
            //MessageBox("Your count has been saved.",
            //                    "Save",
            //                    [
            //                        {
            //                            Text: 'Ok'
            //                        }
            //                    ]);
        }

        function GetBalancesFromServer() {
            data = {
                DisplayType: 'fowst',
                FOWSTID: FowstID
            }
            
            if (OfflineMode) {
                var pSql;
                var pDT;
                var mCountIDs = [];

                try {
                    $("#dvpopup").popup("close");
                    $('#popMessage').html('Please wait while we\'re getting inventory updates from the server');
                    $("#dvpopup").popup("open");
                } catch (e) {

                }

                pSql = 'Select ID from TransInventoryProductCountstock where IsActionTaken IS NULL'
                pDT = Sql.Execute(pSql);

                if (pDT.Rows.length > 0) {

                    for (var l = 0; l <= pDT.Rows.length - 1; l++) {
                        mCountIDs[l] = { ID: pDT.Rows[l].ID};
                    }

                    var pData = {
                        ID: mCountIDs,
                        FOWSTID: FowstID
                    }

                    AjaxSend({
                        url: 'Login3/TestCookies',                        
                        success: function (vData) {

                            AjaxSend({
                                url: 'InventoryCount/GetApprovedAndDisapprovedCounts',
                                data: pData,
                                success: function (vData) {
                                    var vRows = vData.Results.Rows
                                    var mVariance;

                                    if (vRows.length > 0) {
                                        for (var i = 0; i <= vRows.length - 1; i++) {
                                            pSql = 'Update TransInventoryProductCountStock set IsActionTaken=1 ' +
                                                   ',VarianceApprovedBy  = ' + ReadyString(vRows[i].VarianceApprovedBy) +
                                                    ',VarianceApprovedDate   = ' + ReadyString(vRows[i].VarianceApprovedDate)

                                            if (CBool(vRows[i].IsVarianceApproved)) {
                                                pSql += ', IsVarianceApproved = 1 '
                                            } else if(CBool(vRows[i].IsVarianceDisApproved)){
                                                pSql += ', IsVarianceDisApproved = 1 '
                                            }

                                            pSql += 'WHERE ID =  ' + ReadyString(vRows[i].ID)

                                            Sql.Execute(pSql);

                                            if (CBool(vRows[i].IsVarianceApproved)) {
                                                pSql = 'Select * from TransInventoryProductCountstock ' +
                                                        'inner join TransInventoryProductCount on TransInventoryProductCount.ID = TransInventoryProductCountStock.InventoryProductCountID ' +
                                                        'WHERE TransInventoryProductCountStock.ID =  ' + ReadyString(vRows[i].ID);

                                                mVariance = Sql.Execute(pSql);

                                                if (mVariance.Rows.length > 0) {
                                                    Business.TransInventoryLedger.Add(mVariance.Rows[0].ProductID, mVariance.Rows[0].FowstID,
                                                        mVariance.Rows[0].ID, 'InventoryCount', Web.PeopleUserID(), mVariance.Rows[0].Variance,
                                                        undefined, mVariance.Rows[0].BinID, undefined, undefined, undefined);
                                                }
                                            }

                                        }
                                    }

                                    try{
                                        $("#dvpopup").popup("close");

                                        setTimeout(function () {
                                            MessageBox('Product balances has been updated. You can now proceed with inventory count.', 'Inventory Count')
                                        }, 500)
                                    } catch (e) {

                                    }
                                    
                                    GetOfflineBalances();
                                },
                                error: function (e) {
                                    try {
                                        $("#dvpopup").popup("close");
                                    } catch (e) {

                                    }

                                    setTimeout(function () {
                                        MessageBox('We\'re having problems connecting to Mobiz Server. Please try again later.')
                                    }, 500)
                                    
                                    GetOfflineBalances();
                                }
                            });
                        },
                        error: function (e) {
                            GetOfflineBalances();
                        }
                    });
                } else {
                    GetOfflineBalances();
                }
                
                function GetOfflineBalances() {
                    var pSql = 'Select distinct ProductID, (Select balance from Transinventoryledger t1 where t1.productid = t2.productid AND FOWSTID = ' + ReadyString(FowstID) + ' and BINID = \'10d4672a-0358-4e2d-9471-2ee42e464300\' order by lastupdated desc limit 1) Balance from transinventoryledger t2'

                    ProductEndingBalance = Sql.Execute(pSql);

                    if (window.location.hash.substring(1).indexOf('PageCountByTableType')>=0) {
                        if ($('#txtSearchProducts').val() == '') {
                            GetAllProductsForTableType(CategoryType, '');
                        } else {
                            GetAllProductsForTableType(CategoryType, $('#txtSearchProducts').val());
                        }
                    }
                }

            } else {

                AjaxSend({
                    url: 'Login3/TestCookies',
                    success: function (vData) {
                        AjaxSend({
                            url: 'InventoryLedger/GetReport',
                            data: data,
                            success: function (vData) {
                                ProductEndingBalance = vData.data;

                                if (window.location.hash.substring(1) == 'PageCountByTableType') {
                                    if ($('#txtSearchProducts').val() == '') {
                                        GetAllProductsForTableType(CategoryType, '');
                                    } else {
                                        GetAllProductsForTableType(CategoryType, $('#txtSearchProducts').val());
                                    }
                                }
                            },
                            error: function (e) {
                                if (window.location.hash.substring(1) == 'PageCountByTableType') {
                                    Android.Toast('Cannot connect to the internet')
                                }
                            }
                        });
                    },
                    error: function (e) {
                        if (window.location.hash.substring(1) == 'PageCountByTableType') {
                            Android.Toast('Cannot connect to the internet')
                        }
                    }
                });
            }
        }
        
        function GetBinsBalances(ProductID) {
            var data = {
                ProductID: ProductID,
                FowstID: sessionStorage.FowstID
            }

            var mData = InventoryCount.GetBinBalances(data);
            
            DisplayBinsBalances(mData.Rows);
        }

        function DisplayBinsBalances(vRows) {

            var pLV = $('#tblBins');

            pLV.empty();

            for (var i = 0; i <= vRows.length - 1; i++) {

                pItem = $('<tr>')
                            .attr('ID', vRows[i].ID)
                            .data('data', vRows[i])
                            .css('cursor', 'pointer')
                            .append($('<td>')
                                .text(vRows[i].Name)
                            )
                            .append($('<td>')
                                .text(vRows[i].UnitMeasure)
                            )                         
                            .append($('<td>')
                                    .append($('<input type=number ID = ' + vRows[i].ID + ' />')
                                    .data('data', vRows[i])
                                    .attr('class', 'ui-input-text ui-corner-all ui-btn-shadow ui-body-c')
                                    .attr('tabindex', i)
                                    .keypress(function (event) {
                                        if (event.keyCode == 9 || event.keyCode == 13 || event.which == 9 || event.which == 13) {
                                            $(this).parent().parent().next().children().children('input').focus();
                                        }
                                    })
                                    .keydown(function (event) {
                                        if (event.keyCode == 9 || event.keyCode == 13 || event.which == 9 || event.which == 13) {
                                            event.preventDefault();
                                            $(this).parent().parent().next().children().children('input').focus();
                                        }
                                    })                                    
                                    )
                                )
                            .click(function () {
                             
                            })

                pLV.append(pItem);
            }

            PopupReposition();
        }

        function AddAllBins() {
            var mTotal = 0;
            var mBins = [];
            var mBinsValue = [];
            var ctr = 0;
            $('#tblBins tr td input').each(function (mindex) {
                if ($(this).val() != '') {
                    var vBinIDTemp;
                    var vBinIDNew;
                    mBins[ctr] = $(this).attr('id');
                    mBinsValue[ctr] = $(this).val();
                    mTotal = mTotal + CDec2($(this).val())
                    ctr++;
                }
            });

            $('#panSelectInventoryBalance').popup('close');
            $('#' + $('#lblPopUpProductID').text()).attr('Bins', mBins);
            $('#' + $('#lblPopUpProductID').text()).attr('BinsValue', mBinsValue);
            $('#' + $('#lblPopUpProductID').text()).val(mTotal);
            
        }

        window.ComputeSuggestedOrder = function (mTotal) {
            var Total = 0;

            $('#lstCounts li a').each(function (m_index) {
                if ($(this).data('units') != undefined) {
                    if (IsGuid($(this).data('units').ID)) {
                        Total += parseFloat($(this).data('units').MaxCap) - parseFloat($(this).data('units').Quantity);
                    } else {
                        if (CBool($(this).data('units')[0].IsGood)) {
                            //if (parseFloat($(this).data('units')[0].MaxCap) - parseFloat($(this).data('units')[0].Quantity) > 0) {
                            Total += parseFloat($(this).data('units')[0].MaxCap) - parseFloat($(this).data('units')[0].Quantity);
                            //}
                        }
                    }
                }; //end if
            });


            Total += mTotal;


            $('#txtSuggestedOrder').val(Total);
        }


        //===================== 02/06/17
        function getAllProductsInBin() {
            var data = {
                FOWSTID: sessionStorage.FowstID,
                BinID: BinID
            }

            var mData;

            mData = InventoryCount.GetAllProductsInBin(data);

            DisplayProductsInBin(mData.Rows);

            //AjaxSend({
            //    url: 'InventoryCount/GetAllBinStock',
            //    data: data,
            //    success: function (vData) {
            //        DisplayProductsInBin(vData);
            //    }
            //});
        }

        function DisplayProductsInBin(vData) {

            if ($.fn.DataTable.isDataTable(ProductTable)) {
                //empty the table first before destroying datatabase para dili magdoble ang header
                ProductTable.empty();
                productDatatable.destroy();
            }

            productDatatable = ProductTable.DataTable({
                "data": vData,
                "columns": [
                    { "data": "StockNumber" },
                    { "data": "ProductName" },
                    { "data": "Barcode" },
                    { "data": "" },
                    { "data": "ProductSearchString" },
                    { "data": "ProductID" }
                ],
                "columnDefs": [
                    {
                        'targets': [2],
                        'searchable': false,
                        'orderable': false,
                        'className': 'dt-body-center',
                        'render': function (data, type, full, meta) {
                            if (data == '') {
                                return '<a data-role="none" href="#" class="custom-link">N/A</a>';
                            } else {
                                return '<a data-role="none" href="#" class="custom-link">' + data + '</a>';
                            }                            
                        }
                    },
                {
                    'targets': [3],
                    'searchable': false,
                    'orderable': false,
                    'className': 'dt-body-center',
                    'render': function (data, type, full, meta) {
                        return '<input type="number">';
                    }
                }, {
                    "targets": [4],
                    "visible": false,
                    "searchable": true
                }, {
                    "targets": [5],
                    "visible": false,
                    "searchable": true
                }],
                "sDom": '<"top"l>rt<"bottom"ip><"clear">' //hide search label and field
                //"language": {
                //    "search": "Search Product within the list &nbsp; &nbsp; <button style='padding:0px'> <img src='/Content/images/barcode96x96.png' style='height: 35px' id='butProductBarcode'/> </button> "
                //},                
                //"fnFooterCallback": function (nRow, aaData, iStart, iEnd, aiDisplay) {
                //    if (aiDisplay.length == 1) {
                //        $('#tblBinProducts').click();
                //    }
                //}
            });

           
        }

        function getAllProductsInBinUnitMeasures() {
            var data = {
                ProductID: ProductID
            }

            var mData = InventoryCount.GetStockUnitMeasures(data);

            DisplayAllProductsInBinUnitMeasures(mData.Rows);
        }

        function DisplayAllProductsInBinUnitMeasures(vData) {
            if ($.fn.DataTable.isDataTable(ProductUnitMeasureTable)) {
                //empty the table first before destroying datatabase para dili magdoble ang header
                ProductUnitMeasureTable.empty();
                productUnitMeasureDatatable.destroy();
            }

            productUnitMeasureDatatable = ProductUnitMeasureTable.DataTable({
                "data": vData,
                "columns": [
                    { "data": "" },
                    { "data": "UnitMeasure" },
                    { "data": "Quantity" }                    
                ],
                "columnDefs": [
                {
                    'targets': [0],
                    'searchable': false,
                    'orderable': false,
                    'className': 'dt-body-center',
                    'render': function (data, type, full, meta) {
                        return '<input type="number">';
                    }
                }],
                "order": [[1, "asc"]],
                "language": {
                    "search": "Search Unit Measure"                    
                }                
            });

            //carry over qty
            if (productQty != undefined) {
                $('#tblProductBinUnitMeasures tbody tr td input').each(function () {
                    if (productUnitMeasureDatatable.row($(this).closest('tr')).data().Quantity == 1) {
                        $(this).val(productQty);
                    }                    
                })
            }
        }


        function getAllProductsUnitMeasuresForBarcode() {
            var data = {
                ProductID: ProductID,
                IsIncludeBarcode: true
            }

            var mData = InventoryCount.GetStockUnitMeasures(data);

            DisplayAllProductsUnitMeasuresForBarcode(mData.Rows);
        }

        function DisplayAllProductsUnitMeasuresForBarcode(vData) {
            if ($.fn.DataTable.isDataTable(ProductBarcodeTable)) {
                //empty the table first before destroying datatabase para dili magdoble ang header
                ProductBarcodeTable.empty();
                productBarcodeDatatable.destroy();
            }

            productBarcodeDatatable = ProductBarcodeTable.DataTable({
                "data": vData,
                "columns": [                    
                    { "data": "UnitMeasure" },
                    { "data": "Quantity" },
                    { "data": "Barcode" },
                    { "data": "" }
                ],
                "columnDefs": [
                {
                    'targets': [3],
                    'searchable': false,
                    'orderable': false,
                    'className': 'dt-body-center',
                    'render': function (data, type, full, meta) {
                        return '<input type="number">';
                    }
                },
                {
                    'targets': [4],
                    'searchable': false,
                    'orderable': false,
                    'className': 'dt-body-center',
                    'render': function (data, type, full, meta) {
                        return '<img src="/Content/images/barcode96x96.png" style="height: 24px;" class="detailimage2" data-role="button" data-corners="false" />';
                    }
                }],
                "order": [[1, "asc"]],
                "language": {
                    "search": "Search Unit Measure"
                }
            });
        }

        function SaveObservedCountByBin() {
            var Count = [];

            $('#tblBinProducts tbody tr td input').each(function () {
                if ($(this).val() != '') {

                    Count[0] = {
                        BinID: BinID,
                        ExpirationDate: 'N/A',
                        UnitMeasureID: productDatatable.row($(this).closest('tr')).data().MeasurementID,
                        Quantity: CInt2($(this).val()),
                        ActualQuantity: 1,
                        IsGoodStock: 1
                        //,
                        //ExpectedValue: ($(this).attr('serverBalance') == 'N/A' ? 'NULL' : $(this).attr('serverBalance')), //serverBalance
                        //Variance: (CDec2(mBinsValue[i]) - CDec2($(this).attr('serverBalance')) || 'NULL')
                        //,
                        //Remarks: $('#td' + $(this).attr('ID').replace('txt', '')).attr('remarks')
                    };

                    var data = {
                        ID: ID,
                        IsNew: IsNew,
                        FowstID: FowstID,
                        ProductID: productDatatable.row($(this).closest('tr')).data().ProductID,
                        Count: Count
                        //,
                        //BatchID: BatchID
                    }                    

                    if (OfflineMode) {
                        IsSuccessful = InventoryCount.SaveInventoryCount(data);
                        $('#popProductBinCount').hide();
                    }
                }
            })
        }

        function SaveReorderPoint() {
            var data = {
                MaxCap: CDec2($('#txtMaxCap').val()),
                ReorderPoint: CDec2($('#txtReorderPoint').val()),
                ReorderQty: CDec2($('#txtReorderQty').val()),
                FowstID: sessionStorage.FowstID,
                BinID: BinID,
                ProductID: ProductID
            }

            IsSuccessful = InventoryCount.SaveReorderPoint(data);

        }

        function SetByBinSettings() {
            var tempinterval = setInterval(function () {
                if (typeof AddBreadCrumb != 'undefined') {
                    $('.ItemsBreadCrumbs').empty();
                    AddBreadCrumb({
                        text: 'Inventory Count', Click: function () {
                            $.mobile.changePage("#");
                        }
                    }, '.ItemsBreadCrumbs')
                    clearInterval(tempinterval)
                }
            }, 100) //home link

            //setTimeout(function () {
            //    AddBreadCrumb({
            //        text: 'Count By Bin', Click: function () {                        
            //        }
            //    }, '.ItemsBreadCrumbs')
            //}, 1000);

            GetNamedFowstBin();
            $('.OrganizationNameHeader').text(sessionStorage.OrgName);
            $('#lblWorkFowstName').text(sessionStorage.FowstName);
            $('.btnFOWST').text('FOWST: ' + sessionStorage.FowstName);
                        
        }

        function SetByAreaSettings() {
            var tempinterval = setInterval(function () {
                if (typeof AddBreadCrumb != 'undefined') {
                    $('.ItemsBreadCrumbs').empty();
                    AddBreadCrumb({
                        text: 'Inventory Count', Click: function () {
                            $.mobile.changePage("#");
                        }
                    }, '.ItemsBreadCrumbs')
                    clearInterval(tempinterval)
                }
            }, 100) //home link
            
            GetAllFowstArea();
            $('.OrganizationNameHeader').text(sessionStorage.OrgName);
            $('#lblWorkFowstName').text(sessionStorage.FowstName);
            $('.btnFOWST').text('FOWST: ' + sessionStorage.FowstName);

        }

        function GetProducts() {
            BrowsePopup('LibraryPIMProduct', 'Products',
                        [
                            {
                                //AutoSearch: true,
                                Barcode: true,
                                Offline: true,
                                DisplayText: [{ Column: 'ProductName' }],
                                Click: function (e, IsFromBarcode) {
                                    var data = {
                                        ProductID: e.ID,
                                        FowstID: sessionStorage.FowstID,
                                        BinID: BinID
                                    }
                                    if (!CBool(IsFromBarcode)) {
                                        SaveProductWithoutBarcode(e.ID);
                                    }
                                    SaveBINProducts(data);
                                }
                            }
                        ], { FilterFowst: 1 }
                    ); // END BrowsePopup('LibraryFowst', 'Fowst',
        }

        function SaveProductWithoutBarcode(vProductID) {
            var pSql;

            pSql = 'Insert into private.LibraryProductsWithoutBarcode(ID, ProductID) values(' +
                    ReadyString(NewID()) + ',' +
                    ReadyString(vProductID) + ')'

            Sql.Execute(pSql);
        }

        function SaveBINProducts(data) {
            
            var pFinder = new Finder();
            var pFields = pFinder.Fields;
            var pField;
            var pSql = '';
            var mData;
            var pSql2 = '';
            var mData2;
            var pIU;

            pSql = 'Select ID from private.LibraryFowstBinProduct ' +
                'Where FowstID = ' + ReadyString(data.FowstID) +
                ' AND BinID = ' + ReadyString(data.BinID) +
                ' AND ProductID = ' + ReadyString(data.ProductID)

            mData = Sql.Execute(pSql);

            //Check if Product already exist in the BIN Location
            pSql2 = 'Select ID from private.LibraryFowstBinProduct ' +
                'Where FowstID = ' + ReadyString(data.FowstID) +
                ' AND IsDefault = "1" ' +
                ' AND ProductID = ' + ReadyString(data.ProductID)

            mData2 = Sql.Execute(pSql2);

            pIU = new InsertUpdate('private.LibraryFowstBinProduct');

            pIU.Add('MaxPerProduct', 1);
            pIU.Add('MinPerProduct', 0);

            pIU.Add('FowstID', ReadyString(data.FowstID));
            pIU.Add('BinID', ReadyString(data.BinID));
            pIU.Add('ProductID', ReadyString(data.ProductID));
            pIU.Add('LastUpdated', proj.SqliteNow);

            if (mData.Rows.length == 0) {
                if (mData.Rows.length == 0) {
                    pIU.Add('IsDefault', 1);
                }
                else {
                    pIU.Add('IsDefault', 0);
                }

                pIU.Add('IsActive', 1);
                pIU.Add('ID', ReadyString(NewID()));
                pSql = pIU.GetInsert();
                Sql.ExecuteSql(pSql);
            }
            else {
                //if (state == "delete") {
                //    pIU.Add('IsActive', 0);
                //    pIU.Add('DateImported', 'NULL');
                //    pSql = pIU.GetUpdate('ID = ' + ReadyString(id));
                //    //pSql = "UPDATE private.LibraryFowstBinProduct SET IsActive = 0, DateImported = NULL WHERE ID = " + ReadyString(id);
                //}
                //else {
                pIU.Add('IsActive', 1);
                pIU.Add('DateImported', 'NULL');
                pSql = pIU.GetUpdate('ID = ' + ReadyString(mData.Rows[0].ID));
                //}

                Sql.ExecuteSql(pSql);
            }

            getAllProductsInBin();

        }

        function FilterBinTable(filterType) {
            binDatatable.column(3)
                .search(filterType, false, false, false)
                .draw();
            //if (filterType = 'Active') {

            //}
        }

        function FilterAreaBinTable(filterType) {

            if (filterType == 'All') {
                filterType = '';
            }

            fowstbinproductDatatable.column(0)
                .search(filterType, false, false, false)
                .draw();
            //if (filterType = 'Active') {

            //}
        }
        //=====================

        function HandleEnforceZRead() {

            if (!Business.SystemDefaults.GetEnforceZRead()) {
                // orginal startclock('#clockHotButtons, #clock', '');
                startclock('#clockMonies, #clock', 'C ');
            }

            startclock('#clockMonies, #clock', 'Z ');

            var pZReadDate = GetZReadDay();
            proj.SetZReadDate(pZReadDate);

            var pToday = RemoveTime(new Date());
            var pStr;

            if (+pZReadDate != +pToday) {
                // orignal startclock('#clockHotButtons, #clock', 'Z ');
                //if (CBool(vParams.silent)) {
                //    proj.SetZReadDate(pZReadDate);
                //    // orignal startclock('#clockHotButtons, #clock', 'Z ');
                //    pCallback();
                //} else {
                pStr = 'Your device date is ' + $.format.date(pToday, 'MMM d, yyyy') + ' but the Z-Read date is ' + $.format.date(pZReadDate, 'MMM d, yyyy');
                NewMessageBox(pStr, 'Closing Date', [
                        {
                            Text: 'Close the current Z-Read Date and move date forward by 1 day',
                            Click: function () {
                                NewMessageBox('You will no longer be able to post sales and other transactions to the current Z-Read Date after closing.', '', [
                                    {
                                        Text: 'Yes',
                                        Click: function () {
                                            IncrementZReadDay();
                                            HandleEnforceZRead();                                            
                                        }
                                    },
                                    {
                                        Text: 'No',
                                        Click: function () {
                                            HandleEnforceZRead();                                            
                                        }
                                    }
                                ]);

                            }
                        },
                        {
                            Text: 'Use ' + $.format.date(pZReadDate, 'MMM d, yyyy') + ' as the current date',
                            Click: function () {
                                proj.SetZReadDate(pZReadDate);
                                // orignal startclock('#clockHotButtons, #clock', 'Z ');
                                //pCallback();
                            }
                        }
                ]);
                //} // END if (CBool(vParams.silent)) {
            }
        }; // END HandleEnforceZRead

        function GetZReadDay() {
            if (!IsGuid(sessionStorage.OrgID)) {
                throw 'sessionStorage.OrgID must be a valid GUID before calling GetZReadDay. jsWZ3WVGHB7Y!';
            }

            if (!IsGuid(sessionStorage.SelectedFOWST)) {
                throw 'sessionStorage.FowstID must be a valid GUID before calling GetZReadDay. jsFXQVFSTGHL!';
            }

            var pData = LoadZReadDay();
            try {
                pData = JSON.parse(pData);
            } catch (e) {
                pData = {};
            }
            var pDate = GetStringObject(ReadyGuid(sessionStorage.OrgID) + '.' + ReadyGuid(sessionStorage.FowstID), pData);
            if (IsDate(pDate)) {

            } else {
                pDate = Android.AppInstallDate();
            }
            pDate = RemoveTime(pDate);

            return pDate;
        };

        function LoadZReadDay() {
            return Transfers.ReadFile('ZReadDay.txt');
        };

        function IncrementZReadDay() {
            var pDate = GetZReadDay();
            pDate.setDate(pDate.getDate() + 1);
            pDate = FormatDate(pDate);
            var pData = LoadZReadDay();
            try {
                pData = JSON.parse(pData);
            } catch (e) {
                pData = {};
            }
            var pPath = ReadyGuid(sessionStorage.OrgID) + '.' + ReadyGuid(sessionStorage.FowstID);
            CreateStringObject(pPath, pData, pDate);
            SaveZReadDay(pData);
        };

        function SaveZReadDay(vData) {
            Transfers.WriteFile('ZReadDay.txt', null, JSON.stringify(vData, null, 4));
        };
    });
