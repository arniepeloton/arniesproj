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
    //'/Scripts/sources/SignIn/SignInInfo.js?crc='
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
    var arrCountItems = [];
    var mSummaryMode = false;
    var mRows;

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
                $('#lblLoggedInUser').text(Sql.ExecuteScalar('SELECT FirstName || " " || Surname as Name FROM LibraryPeople WHERE ID = ' + ReadyString(Web.PeopleUserID())))

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

            $('body')
                .on('click', '#tblNamedBins tbody tr td', function () {
                    return;
                    // for original UI ni sya na option, no need ipakita sa new UI
                    // commented by Arnie on Nov 18, 2024
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
                            Bs.Loading('show');
                            setTimeout(function () {
                                getAllProductsInBin();
                                $('#popProductBinCount').modal('show');
                                $('#lblProductBinMappingLabel').text('Bin ' + BinName);
                                if (IsBinActive) {
                                    $('#btnAddProduct').css('visibility', 'visible');
                                    $('#btnSaveObservedCountByBin').css('visibility', 'visible');
                                } else {
                                    $('#btnAddProduct').css('visibility', 'hidden');
                                    $('#btnSaveObservedCountByBin').css('visibility', 'hidden');
                                }
                                Bs.Loading('hide');
                            }, 200)                                
                        }
                    });

                    PopupButtonList('', 'Bin ' + BinName,
                        mData, { AutoDismiss: true })                        
                }

                })
                .on('click', '#tblBinProducts tbody tr td', function () {

                var mData = [];

                if (productDatatable.data().count() > 0) {

                    if (productDatatable.cell(this).index().columnVisible == 3) {
                        return;
                    }
                                                
                    ProductID = productDatatable.row(this).data().ProductID;
                    productTableSelectedRow = productDatatable.row(this).index();
                    productQty = $(this).closest('tr').find('input').val();

                    if (IsBinActive) {
                        $('#popProductBinCount').modal('hide');

                        $('#txtMaxCap').val(productDatatable.row(this).data().MaxPerProduct);
                        $('#txtReorderPoint').val(productDatatable.row(this).data().ReorderPoint);
                        $('#txtReorderQty').val(productDatatable.row(this).data().ReorderQty);
                                                        
                        if (productDatatable.cell(this).index().columnVisible == 2) {
                            getAllProductsUnitMeasuresForBarcode();
                            $('#popProductBarcode').modal('show').unbind('hidden.bs.modal').on('hidden.bs.modal', function () {
                                $('#popProductBinCount').modal('show');
                            });
                        } else {
                            getAllProductsInBinUnitMeasures();
                            $('#popProductBinUnitMeasures').modal('show').unbind('hidden.bs.modal').on('hidden.bs.modal', function () {
                                $('#popProductBinCount').modal('show');
                            });
                        }
                            
                        $('#lblProductBinUnitMeasuresLabel').text(productDatatable.row(this).data().ProductName);
                    }                        
                }

                })
                .on('click', '#btnDoneProductBinUnitMeasures', function () {

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

                $('#popProductBinUnitMeasures').modal('hide');

                })
                .on('click', '#tblProductBarcode tbody tr td', function () {

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
                    
                })
                .on('click', '#btnDoneProductBarcode', function () {

                var TotalBaseBalance;
                var UnitMeasureID
                if (productBarcodeDatatable.data().count() > 0) {

                    $('#tblProductBarcode tbody tr td input').each(function () {

                        if ($(this).val() != '') {                                
                            InventoryCount.SaveProductBarcode($(this).val(), productBarcodeDatatable.row($(this).closest('tr')).data().PIMProductMeasurementID);
                        }
                    })
                }

                $('#popProductBarcode').modal('hide');

                })
                .on('click', '#tblShelfPrices tbody tr td', function () {

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
                        $('#popProductBinUnitMeasures').modal('show');
                        $('#lblProductBinUnitMeasuresLabel').text(fowstbinproductDatatable.row(this).data().ProductName);
                    //}
                }

                })
                .on('click', '#tblProductsInCategories tbody tr td', function () {

                var mData = [];

                if (ProductsInCategoriesDatatable.data().count() > 0) {

                    productTableSelectedRow = ProductsInCategoriesDatatable.row(this).index();

                    if (ProductsInCategoriesDatatable.cell(this).index().columnVisible != 1) {
                        return;
                    } else {
                        ProductID = ProductsInCategoriesDatatable.row(this).data().ID;

                        getAllProductsInBinUnitMeasuresByCategory()

                        $('#lblProductBinUnitMeasuresLabelByCategory').text(ProductsInCategoriesDatatable.row(this).data().ProductName);

                        $('#popProductBinUnitMeasuresByCategory').modal('show');

                        $('#btnDoneProductBinUnitMeasuresByCategory').unbind().click(function () {

                            //convert values to base unit
                            var TotalBaseBalance;
                            var UnitMeasureID
                            if (productUnitMeasureByCategoryDatatable.data().count() > 0) {

                                $('#tblProductBinUnitMeasuresByCategory tbody tr td input').each(function () {

                                    if (productUnitMeasureByCategoryDatatable.row($(this).closest('tr')).data().Quantity == 1) {
                                        UnitMeasureID = productUnitMeasureByCategoryDatatable.row($(this).closest('tr')).data().ID
                                    }

                                    if ($(this).val() != '') {
                                        TotalBaseBalance = CDec2(TotalBaseBalance) + (CDec2($(this).val()) * CDec2(productUnitMeasureByCategoryDatatable.row($(this).closest('tr')).data().Quantity));
                                    }
                                })

                                //display value to main table
                                if (TotalBaseBalance != undefined) {
                                    $('#tblProductsInCategories tbody tr td input').each(function () {
                                        if (ProductsInCategoriesDatatable.row($(this).closest('tr')).index() == productTableSelectedRow) {
                                            $(this).val(TotalBaseBalance);
                                            $(this).blur(); //para ma-add sa array for saving ang data
                                        }
                                    })

                                }
                            }

                            $('#popProductBinUnitMeasuresByCategory').modal('hide');
                        });
                    }
                }
                })
                .on('change', '#selInvCntOptions', function () {
                    var val = $(this).val();

                    if (val == 'By Inventory-Count Products') {
                        $('#ByInventoryCountItems').click();
                    } else if (val == 'By New Inventory-Count Products') {
                        InventoryCountByCategory();
                    } else if (val == 'By Product Category') {
                        $('#ByTableType').click();
                    } else if (val == 'By Product') {
                        $('#BySpatial').click();
                    } else if (val == 'By Bin Location') {
                        $('#ByBin').click();
                    }
                })

            $('#btnSaveObservedCountByBin').click(function () {
                SaveObservedCountByBin();
            })

            $('#lblWorkFowstName').change(function () {
                sessionStorage.FowstID = $(this).val();
                sessionStorage.FowstName = $('#lblWorkFowstName option:selected').text();
                sessionStorage.SelectedFOWST = $(this).val();
                GetNamedFowstBin();
                //DisplayFowstPopUp();
            })

            $('#btnAddProduct').click(function () {
                GetProducts();
            });

            $('#btnManuallyAddBinTags').click(function () {
                $('#popAddBinAddress').modal('show');
            })

            $('#btnAddBinAddress').click(function () {
                var data;
                var pSql;
                var mData;

                if ($('#txtBinAddress').val() == '') {
                    WebToast('Please enter Bin Address', 'top');
                    return;
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

                    $('#popAddBinAddress').modal('hide');

                    GetNamedFowstBin();
                } else {
                    WebToast('Bin Address already exist, please create a new bin address.', 'top');
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
                                "Select FOWST Area"); // END NewMessageBox
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
                //$('#btnBinLocSave').text('Next');
                $('.not-summary').show();
                $('.summary').hide();
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
                NewMessageBox("This feature is not yet available.",
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
                NewMessageBox("This feature is not yet available.",
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

            $('#ByInventoryCountItems').click(function () {
                GroupSelected = 'ByInventoryCountItems';
                SetInventoryCountFormat();
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
                    NewMessageBox("Please enter a Shelf/Bin name.",
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
                NewMessageBox("Please select Shelf/Bin Location.",
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
                NewMessageBox("Quantity must be a number greater than 0.",
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
                NewMessageBox("Quantity must be a number greater than 0.",
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
                NewMessageBox("Please select Shelf/Bin Location.",
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
                $(this).blur();
                var mHasValue = false;

                $('#tblInventoryCount tr td input').each(function (mindex) {
                    if ($(this).val() != '') {
                        mHasValue = true;
                        return 0;
                    }
                });

                if (mHasValue) {
                    NewMessageBox("There are pending counts waiting to be saved. Would you like to save them?",
                            "Pending Counts",
                            [
                                {
                                    Text: 'Yes',
                                    Click: function () {
                                        setTimeout(function () {
                                            SaveCountFromTableList();
                                        }, 500)
                                    }
                                },
                                {
                                    Text: 'No',
                                    Click: function () {
                                        CategoryType = '';
                                        Bs.Loading('show');
                                        GetAllProductsForTableType(CategoryType, $('#txtSearchProducts').val());
                                        //DisplayAllProductsForTableType(mData.Rows);
                                        $('.lblSelectedCategory').text('Select Category')
                                    }
                                }
                            ]
                        );
                } else {
                    CategoryType = '';
                    Bs.Loading('show');

                    var pRows = ProductsInCategoriesDatatable.data();
                    pRows = $.grep(pRows, function (vRow, i) {
                        //if (CDec2(vRow.LastCount) > 0)
                        //    return true;
                        //return false;
                        var pCountedRows = $.grep(arrCountItems, function (vCountItem, i) {
                            if (vRow.ID == vCountItem.ID) {
                                return true;
                            }
                        });
                        return pCountedRows.length > 0;
                    });
                    if (pRows.length > 0) {
                        if (mRows == null)
                            mRows = pRows;
                        else
                            mRows = pRows.concat(mRows);
                    }
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
        }).text('View');

        $('#btnSaveAndTurnOverObservedCountByTableList').click(function () {
            SaveCountFromTableList(1);
            $('#txtDepositBy').val(sessionStorage.CurrentUserFullName);
        });

        $('.btnBackToInventoryCountPage').click(function () {
            $("#pageMain").pageshow();
        });

        $('#btnCategoriesBack').click(function () {                
            if (mSummaryMode) {
                ProductsInCategoriesDatatable.clear();
                ProductsInCategoriesDatatable.rows.add(ProductsInCategoriesRows);
                ProductsInCategoriesDatatable.draw();
                $('#btnSaveObservedCountByTableList').text('View');
                mSummaryMode = false;
                $('.summary').hide();
                $('.not-summary').show();
            } else 
            $("#pageMain").pageshow();
        });

        $('#butInventoryCountBack').click(function() {
            if (mSummaryMode) {
                mSummaryMode = false;
                $('.summary').hide();
                $('.not-summary').show();
                $('#btnSaveObservedCountByTableList').text('View');
                DisplayAllProductsForTableType();
            } else {
                history.back();
            }
        });
        $('#btnTurnOverInventory').click(function () {
            GetAllMyCounts();
            $('#txtDepositBy').val(sessionStorage.CurrentUserFullName);
            $("#PageMyCounts").pageshow();
        });

        $('#btnTurnOverBack').click(function () {
            $('#PageCategories').pageshow();
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
                    $("#DialogUserAccess").modal('toggle');
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
                NewMessageBox('Do you want to switch user?', 'Switch User', [
                    {
                        Text: 'Yes', Click: function () {
                            SignInCheckUser(); // SignInInfo.js
                            sessionStorage.removeItem('TOUserName');
                            sessionStorage.removeItem('TOPassword');
                            sessionStorage.removeItem('IsFromTurnOverOfMoniesPage');
                        }
                    }, {
                        Text: 'No', Click: function () {
                            var pParam = {};

                            sessionStorage.removeItem('TOUserName');
                            sessionStorage.removeItem('TOPassword');
                            sessionStorage.removeItem('IsFromTurnOverOfMoniesPage');

                            pParam.IsMobile = 1;
                            pParam = $.param(pParam);
                            window.location = "/People/PeopleMenu?" + pParam;
                        }
                    }
                ])
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
                    $("#DialogUserAccess").modal('toggle');
                }
            }
        });

        $('#btnGetBalancesFromServer').click(function () {
            NewMessageBox("Downloading expected inventory balances from server(s) requires a live Internet connection. Please make sure that you are connected to the Internet and click on the OK button when done.",
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
                    ); //end NewMessageBox
        });

        $('#selProductFilter').change(function () {
            ProductFilter = $('#selProductFilter').val();
            if ($('#txtSearchProducts').val() == '') {
                GetAllProductsForTableType(CategoryType, '');
            } else {
                GetAllProductsForTableType(CategoryType, $('#txtSearchProducts').val());
            }
        });

        $('.btnSelectCategory').click(function () {                
            GetCategories();
        })

        $('#btnBinCount').click(function () {
            AddAllBins();
        })

        $('#btnBinLocSave').click(function () {
            if (true) { //mSummaryMode - save na daritso, for Vending Machine. so far VM pa lang nagagamit ani na inventory
                var vrows = $('#listBinCategorySku .txtBinCount:not([value=""])') /*BinsTable.find('tbody > tr:visible')*/, ctrBin = 1;

                if (CInt2($('#lblBinMissingCount').text()) > 0) {
                    NewMessageBox('Please enter the inventory counts for the uncounted items.', 'Warning')
                    return;
                }

                Bs.Loading('show', 'Saving...');
                setTimeout(function () {
                    vrows.each(function () {
                        var dis = $(this), data = dis.data('data');
                        InventoryCount.SaveInventoryCount({
                            ID: ID,
                            IsNew: 1,
                            FowstID: sessionStorage.FowstID,
                            ProductID: data.ProductID,
                            Count: [{
                                BinID: data.ID,
                                ExpirationDate: 'N/A',
                                UnitMeasureID: data.MeasurementID,
                                Quantity: CInt2(dis.val()),
                                ActualQuantity: 1,
                                IsGoodStock: 1
                                //,
                                //ExpectedValue: ($(this).attr('serverBalance') == 'N/A' ? 'NULL' : $(this).attr('serverBalance')), //serverBalance
                                //Variance: (CDec2(mBinsValue[i]) - CDec2($(this).attr('serverBalance')) || 'NULL')
                                //,
                                //Remarks: $('#td' + $(this).attr('ID').replace('txt', '')).attr('remarks')
                            }]
                            //,
                            //BatchID: BatchID
                        });

                        if (vrows.length == ctrBin) {
                            Bs.Loading('hide');
                            //$('#btnBinLocBack').click();
                            //BinsTable.find('tbody > tr').find('input').val('');
                            NewMessageBox('Successfully saved', 'Inventory');
                            GetNamedFowstBin();
                        }

                        ctrBin += 1;
                    })
                }, 1000)                    
            } else {
                BinsTable.find('tbody > tr').each(function () {
                    if ($(this).find('input').val() == '') {
                        $(this).hide();
                    }
                })
                mSummaryMode = true;
                $('.not-summary').hide();
                $('.summary').show();
                $(this).text('Save Now');
                $('#btnBinLocBack').text('Cancel');
                binDatatable.draw();
            }                
        })

        $('#btnBinLocBack').click(function () {
            if (mSummaryMode) {
                BinsTable.find('tr').show();
                mSummaryMode = false;
                $('.not-summary').show();
                $('.summary').hide();
                $('#btnBinLocSave').text('Next');
                $(this).text('Back');
                binDatatable.draw();
            } else {
                history.back();
            }
        })

        $('#btnSubmitIC').click(function () {
            var missingCountItems = [];

            $('.listProduct').each(function () {
                var dis = $(this), data = dis.data('data');
                if (dis.find('.countinfo').is(':empty')) {
                    missingCountItems.push(data.ProductNo + ' - ' + data.ProductName);
                }
            })

            if (missingCountItems.length > 0) {
                NewMessageBox('Missing Inventory Count for the following Inventory Items: <br><br><ol class="list-group list-group-flush list-group-numbered"><li class="list-group-item">' + missingCountItems.join('</li><li class="list-group-item">') + '</ol>', 'Missing Counts');
                return
            }

            $('.listProduct[status="Ready"]').each(function name(params) {
                var dis = $(this), data = dis.data('data');
                var smallestUom = {}, convertedCount = 0;

                for (var i in data.UnitMeasures) {
                    var uom = data.UnitMeasures[i];

                    convertedCount += CDec2(uom.Count) * CDec2(uom.Quantity);

                    if (uom.Quantity == 1) {
                        smallestUom.UnitMeasureID = uom.ID;
                        smallestUom.UnitMeasure = uom.UnitMeasure;
                    }
                }

                arrCountItems.push({
                    ID: data.ID, //ProductID
                    ProductName: data.ProductName,
                    ProductNo: data.ProductNo,
                    MeasurementID: smallestUom.UnitMeasureID,
                    UnitMeasureID: smallestUom.UnitMeasureID,
                    UnitMeasure: smallestUom.UnitMeasure,
                    Count: convertedCount,
                    serverBalance: 'N/A'
                })
            })
            mSummaryMode = true;
            SaveCountFromTableList()
        })

        $('#txtSearchICItems').on('input', function () {
            // Get the value of the search bar and convert to lowercase
            var value = $(this).val().toLowerCase();

            // Filter the list items
            $('#listICCategorySku .divProductInfo').filter(function () {
                // Toggle visibility based on whether the text matches
                $(this).parents().closest('.listProduct').toggle($(this).text().toLowerCase().indexOf(value) > -1);
            });
        });

        $('#btnBackIC').click(function () {
            if (CDec2($('#lblNoOfPendingItems').text()) == 0 && CDec2($('#lblNoOfItemsToBeSubmitted').text()) > 0) {
                NewMessageBox('You have ' + CDec2($('#lblNoOfItemsToBeSubmitted').text()) + ' item(s) ready for submission. Do you want to submit your inventory now?', 'Confirm Submission', [
                    {
                        Text: 'Yes, submit my inventory', Click: function () {
                            $('#btnSubmitIC').click();
                        }
                    }, {
                        Text: 'No, I\'ll do it later', Click: function () {
                            window.location = '/People/PeopleMenu';
                        }
                    }
                ])
            } else window.location = '/People/PeopleMenu';
        })

        setTimeout(function () {
            var defaultICOption = Business.SystemDefaults.GetSystemDefault('DefaultInventoryOption', '');
            $('#divICOption').hide();
            if (defaultICOption == 'By Inventory-Count Products') {
                //$('#ByInventoryCountItems').click(); // old UI
                InventoryCountByCategory(); // new UI, requested by PC
            } else if (defaultICOption == 'By Product Category') {
                $('#ByTableType').click();
            } else if (defaultICOption == 'By Product') {
                $('#BySpatial').click();
            } else if (defaultICOption == 'By Bin Location') {
                $('#ByBin').click();
            } else {
                $('#divICOption').show();
            }
        }, 1000)            
    }); //End Inner function

    function InventoryCountByCategory() {
        $('#PageInventoryCountByCategories').pageshow();

        var selectedFowstID = $('#selFOWST').val();
        var listICCategories = $('#listICCategories').empty();
        var listICCategorySku = $('#listICCategorySku').empty();
        var pCategories = $.merge([{ Category: 'All Categories' }], InventoryCount.GetAllProductCategories(selectedFowstID));
        var noofICItems = 0;
        console.log(pCategories)
        for (var i in pCategories) {
            listICCategories.append($('<li style="flex: 0 0 auto;">')
                .append($('<button data-role="none" class="btn btn-outline-dark rounded-pill btnCat">')
                    .text(pCategories[i].Category)
                    .data('data', pCategories[i])
                    .click(function () {
                        var dis = $(this), data = dis.data('data');

                        $('.btnCat').removeClass('active');
                        dis.addClass('active');

                        if (data.Category == 'All Categories') {
                            $('[subpage="iccategories"]').pageshow();
                        } else {
                            $('[catid="' + data.ID + '"]').pageshow();
                        }
                    })
                )             
            )

            if (pCategories[i].Category != 'All Categories') {
                var parent = $('<div>');

                parent.attr('catid', pCategories[i].ID);
                parent.attr('subpage', 'iccategories');
                parent.hide();

                parent.append($('<div class="text-secondary border-primary p-2 pl-3 pr-3 font-weight-bold" style="background-color: #F5F7FA;">').text(pCategories[i].Category));

                for (var s in pCategories[i].Sku) {
                    noofICItems += 1;
                    var skuData = pCategories[i].Sku[s], isCntData = skuData.Full != '' || skuData.Loose != '';

                    skuData.UnitMeasures = Sql.Execute('SELECT UnitMeasureID as ID, UnitMeasureName as UnitMeasure, Qty as Quantity, UomState, Count FROM cache.TransInventoryCountItemPerUom WHERE CountedByUser = ' + ReadyString(Web.PeopleUserID()) +' AND FowstID = ' + ReadyString(selectedFowstID) + ' AND ProductID = ' + ReadyString(skuData.ID) + ' AND CountDate = ' + ReadyString($.format.date(new Date(), 'MM-dd-yyyy'))).Rows;
                    parent.append($('<div class="p-3 border ' + (skuData.Status == 'Submitted' ? 'bg-success-light' : (skuData.Status == 'Ready' ? 'bg-primary-light' : 'bg-danger-light')) + ' listProduct" status="' + skuData.Status + '">')
                        .attr('productid', skuData.ID)
                        .append($('<div class="row align-items-center">')
                            .append($('<div class="col divProductInfo">')
                                .append('<div class="text-dark font-weight-bold">' + skuData.ProductNo + '</div>')
                                .append('<div class="text-secondary">' + skuData.ProductName + '</div>')
                                .append('<div class="text-secondary">Unit Measure: ' + skuData.BaseUnitMeasure + '</div>')
                                .append('<div class="countinfo ' + (skuData.Status == 'Submitted' ? 'text-success' : (skuData.Status == 'Ready' ? 'text-primary' : 'text-danger')) + '">' + (isCntData ? 'Full: ' + CDec2(skuData.Full) + ' <span class="bi-dot"></span> Loose: ' + CDec2(skuData.Loose) : '') + '</div>')
                            )
                            .append($('<div class="col-auto divStatus">')
                                .append($('<span class="badge rounded-pill ' + (skuData.Status == 'Submitted' ? 'bg-success-transparent text-success' : (skuData.Status == 'Ready' ? 'bg-primary-transparent text-primary' : 'bg-danger-transparent text-danger')) + ' px-2 py-1 d-inline-flex align-items-center gap-1">')
                                    .append((skuData.Status == 'Submitted' ? '<i class="bi bi-check-circle-fill mr-2"></i> Submitted' : (skuData.Status == 'Ready' ? '<i class="bi bi-check-circle mr-2"></i> Ready' : '<i class="bi bi-x-circle mr-2"></i> Missing')))
                                )
                            )
                        )
                        .data('data', skuData)
                        .click(function (e) {
                            var dis = $(this), dataProduct = dis.data('data');
                            var uom = InventoryCount.GetStockUnitMeasures({ ProductID: dataProduct.ID }).Rows;
                            var listProductUom = $('#listProductUom').empty();
                            console.log(dataProduct)
                            console.log(uom)
                            if (uom.length) {
                                var uomlen = uom.length;

                                for (var i in uom) {
                                    var dUom = uom[i];
                                    dUom.State = (uomlen == 1  ? 'Full' : 'Loose');
                                    dUom.Count = Sql.ExecuteScalar('SELECT IFNULL(SUM(Count), "") as [Count] FROM cache.TransInventoryCountItemPerUom WHERE FowstID = ' + ReadyString(selectedFowstID) + ' AND ProductID = ' + ReadyString(dataProduct.ID) + ' AND UnitMeasureID = ' + ReadyString(dUom.ID) + ' AND CountDate = ' + ReadyString($.format.date(new Date(), 'MM-dd-yyyy')))
                                    listProductUom.prepend($('<div class="' + (uom.length > 1 ? 'col-6' : 'col-12')+' mb-3">')
                                        .append(dUom.UnitMeasure + ' Qty (' + dUom.State + ')')
                                        .append($('<input data-role="none" type="number" class="form-control shadow-sm txtUomCountQty" oldvalue="' + dUom.Count + '" value="' + dUom.Count + '" />').data('data', dUom)
                                            .blur(function () {
                                                $('#divNegativeValues').hide()
                                                $('.txtUomCountQty').removeClass('is-invalid').each(function () {
                                                    if (0 > CDec2($(this).val())) {
                                                        $(this).addClass('is-invalid');
                                                        $('#divNegativeValues').show();
                                                    }
                                                })
                                            })
                                        )
                                    )

                                    uomlen -= 1;
                                }

                                $('.lblProductName').text(dataProduct.ProductName);
                                $('.lblProductNo').text(dataProduct.ProductNo);
                                $('.lblProductUom').text(dataProduct.BaseUnitMeasure);    
                                $('#divNegativeValues').hide();
                                $('#popProductUom').modal('show');
                                $('.txtUomCountQty:first').focus();

                                $('#btnConfirm').unbind().click(function () {
                                    if (ConfirmCount(dis, dataProduct, 'confirm')) $('#popProductUom').modal('hide');
                                })

                                $('#btnConfirmAndNext').unbind().click(function () {
                                    ConfirmCount(dis, dataProduct, 'confirmnext');                                    
                                })
                            }                            
                        })
                    )                        
                }

                listICCategorySku.append(parent);
            }
        }

        $('#lblNoOfICItems').text('/ ' + noofICItems);
        $('[subpage="iccategories"]').pageshow();
        $('.btnCat:eq(0)').addClass('active');
        CountStatus();
    }

    function ConfirmCount(listItemContainer, dataProduct, action) {
        var IsEmptyCount = false, full = 0, loose = 0, skuUom = [], hasChanges = false;
        var observedCount = 0;

        $('#divNegativeValues').hide();

        $('.txtUomCountQty').each(function () {
            var dis = $(this), uom = dis.data('data');
            if ($.trim(dis.val()) == '' || 0 > CDec2(dis.val())) {
                IsEmptyCount = true;
                dis.addClass('is-invalid');
            } else {
                dis.removeClass('is-invalid');                
            }

            if (0 > CDec2(dis.val())) $('#divNegativeValues').show();
        })

        if (IsEmptyCount) {
            Array.from(document.querySelectorAll('.txtUomCountQty')).find(input => input.value.trim() === '')?.focus();
            return false;
        }

        $('.txtUomCountQty').each(function () {
            var dis = $(this), uom = dis.data('data');

            uom.Count = CDec2(dis.val());

            if (uom.Count != CDec2(dis.attr('oldvalue'))) hasChanges = true;

            if (uom.State == 'Full') {
                full += CDec2(uom.Count) * CDec2(uom.Quantity)
            } else loose += CDec2(uom.Count) * CDec2(uom.Quantity)

            observedCount += CDec2(uom.Count) * CDec2(uom.Quantity);

            skuUom.push(uom);
            SaveAsDraft(dataProduct, uom);
        })

        dataProduct.Status = "Ready";
        dataProduct.UnitMeasures = skuUom;
        dataProduct.Count = observedCount;

        listItemContainer.attr('status', 'Ready');
        listItemContainer.find('.countinfo').removeClass('text-success text-danger').addClass('text-primary').html('Full: ' + full + '<span class="bi-dot"></span> Loose: ' + loose);
        listItemContainer.removeClass('bg-success-light bg-danger-light').addClass('bg-primary-light');
        listItemContainer.find('.divStatus').empty().append($('<span class="badge rounded-pill bg-primary-transparent text-primary px-2 py-1 d-inline-flex align-items-center gap-1">')
            .append('<i class="bi bi-check-circle mr-2"></i> Ready')
        )
        
        listItemContainer.data('data', dataProduct)

        CountStatus();

        if (action == 'confirmnext') {
            var curIndex = $('.listProduct').index(listItemContainer);
            var nextMissingProduct = $('.listProduct[status="Missing"]').filter(function () {
                return $('.listProduct').index(this) > curIndex;
            }).first();

            if (nextMissingProduct.length == 0) {
                var uncountedItem = $('.listProduct .countinfo:empty').first();
                if (uncountedItem.length == 0) {
                    $('#popProductUom').modal('hide');
                } else uncountedItem.click();
            } else nextMissingProduct.click();                    
        }

        return true;
    }

    function SaveAsDraft(dataProduct, uom) {
        var selectedFowstID = $('#selFOWST').val();
        var p = new InsertUpdate('cache.TransInventoryCountItemPerUom');
        var fowst = Sql.ExecuteRow('SELECT ID, Code, Name FROM LibraryOrganizationFOWST WHERE ID = ' + ReadyString(selectedFowstID))

        p.Add("FOWSTID", ReadyString(fowst.ID))
        p.Add("FOWSTCode", ReadyString(fowst.Code))
        p.Add("FOWSTName",ReadyString(fowst.Name)) 
        p.Add("ProductNo", ReadyString(dataProduct.ProductNo))
        p.Add("ProductName", ReadyString(dataProduct.ProductName))         
        p.Add("UnitMeasureName", ReadyString(uom.UnitMeasure))
        p.Add("Qty", ReadyString(uom.Quantity))
        p.Add("Count", ReadyString(uom.Count))
        p.Add("UomState", ReadyString(uom.State))
        p.Add("Status", ReadyString('Ready'))
        p.Add("CountedByUser", ReadyString(Web.PeopleUserID()))
        p.Add("LastUpdated", proj.SqliteNow)

        var savedData = Sql.ExecuteRow('SELECT ID FROM cache.TransInventoryCountItemPerUom WHERE FowstID = ' + ReadyString(selectedFowstID) + ' AND ProductID = ' + ReadyString(dataProduct.ID) + ' AND UnitMeasureID = ' + ReadyString(uom.ID) + ' AND CountDate = ' + ReadyString($.format.date(new Date(), 'MM-dd-yyyy')));
        if (savedData != undefined) {
            Sql.Execute(p.GetUpdate(' ID = ' + ReadyString(savedData.ID)))
        } else {
            p.Add("CountDate", ReadyString($.format.date(new Date(), 'MM-dd-yyyy')))
            p.Add("ProductID", ReadyString(dataProduct.ID))
            p.Add("UnitMeasureID", ReadyString(uom.ID))
            p.Add("ID", ReadyString(NewID()))

            Sql.Execute(p.GetInsert())
        }
    }

    function CountStatus() {
        $('#lblNoOfConfirmedItems').text($('.listProduct[status="Submitted"]').length);
        $('#lblNoOfItemsToBeSubmitted').text($('.listProduct[status="Ready"]').length);
        $('#lblNoOfPendingItems').text($('.listProduct[status="Missing"]').length);
    }

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
                        //$('#' + mData.Rows[0].ID).button();
                    } else {
                        //unrecognize barcode
                    }
                });
                    //$('#tblReportTable').table('refresh');
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

        $('#PageCategories').pageshow();
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
            NewMessageBox("\'Max Cap for this bin should be greater than or equal to Min Cap\'",
                            "Inventory Count",
                            [
                                { Text: 'Ok' }
                            ]
                        );
            return;
        }

        if ($('#selBinsStockViewer').val() == '') {
            NewMessageBox("Please select Shelf/Bin Location",
                            "Bin",
                            [
                                { Text: 'Ok' }
                            ]
                        );
            return;
        }

        if ($('#selBinsStockViewer').val() == 'Unnamed Bin') {
            $('#pageRenameBin').modal('show');
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
                        NewMessageBox("Stock and Shelf/Bin already exist in the tally.",
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
                        NewMessageBox("Stock and Shelf/Bin already exist in the tally.",
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
                        //    NewMessageBox('This product does not allow decimals. You entered ' + pQty);
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
            NewMessageBox("Quantity must be a number greater than 0.",
                            "Invalid Quantity",
                            [
                                { Text: 'Ok' }
                            ]
                        );
            return;
        }

        if (IsNegative) {
            NewMessageBox("Quantity must be a number greater than 0.",
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
            NewMessageBox("Your count has been successfully added to the tally.  If you are done counting this SKU, please click on the “Save Tallies” button below to confirm your count(s). " +
                        "Otherwise, you might want to count the contents of another Shelf/bin or shelf containing this SKU, or count this SKU with another expiration date grouping. " +
                        "If so, would you like to do another count for this SKU?",
                        "Bin",
                        [
                            { Text: 'Yes'
                            },
                            { Text: 'No',                                    
                            }
                        ]
                    ); //end NewMessageBox
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
            NewMessageBox("The SKU you searched for has a product grouping.  Would you like to count all SKUs within this product grouping?",
                            "Product Grouping",
                            [
                                { Text: 'Yes',
                                    Click: function () {
                                        $('#pageStockViewerDetail').pageshow();
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
                    Offline: true,
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
        $('#selFOWST, #lblWorkFowstName').empty();
        //$('#selFOWST').append(new Option('Select', '', false));

        for (var i = 0; i <= vData.Rows.length - 1; i++) {
            if (vData.Rows[i].ID == undefined) {//server side getting fowst
                $('#selFOWST, #lblWorkFowstName').append(new Option(vData.Rows[i].FowstName, vData.Rows[i].FowstID, false));
                $('#selFOWST, #lblWorkFowstName').val(vData.Rows[i].FowstID);                    
            } else {
                $('#selFOWST, #lblWorkFowstName').append(new Option(vData.Rows[i].FowstName, vData.Rows[i].ID, false));
                $('#selFOWST, #lblWorkFowstName').val(vData.Rows[i].ID);
            }                
        }

        //$('#selFOWST').append(new Option('Select Client\'s FOWST', 'Others', false));
        $('#selFOWST, #lblWorkFowstName').val(CStr2(sessionStorage.FowstID));

        if (!CBool(sessionStorage.IsVendingMachine)) {
            if (IsGuid($('#selFOWST').val())) {
                FowstID = $('#selFOWST').val();
                sessionStorage.FowstID = $('#selFOWST').val();
                sessionStorage.SelectedFOWST = $('#selFOWST').val();
                sessionStorage.FowstName = $('#selFOWST :selected').text();
                //HandleEnforceZRead();
            } // 2017_01_20__10_58_22_3861 braces added by Arvin

            try {
                CreateCacheInventoryTable();
                //Bs.Loading('show');
                //GroupSelected = 'ByInventoryCountItems';
                //GetAllProductsForTableType('', '');
                //$('#selFOWST').selectmenu('refresh');
            }
            catch (e) {
                console.log(e)
                //$('#selFOWST').selectmenu();
            }
        } else {
            GetNamedFowstBin();
        }        
    }

    function CreateCacheInventoryTable() {
        var colName = Sql.Execute('PRAGMA private.table_info(TransInventoryCountItemPerUom)').Rows;
        
        for (var pColIndex = 0; pColIndex <= colName.length - 1; pColIndex++) {
            var colname = colName[pColIndex].name;
            var existCol = Sql.Execute('PRAGMA cache.table_info(TransInventoryCountItemPerUom)').Rows
            if (existCol.length == 0) {
                //if not exist
                Sql.Execute('CREATE TABLE cache.TransInventoryCountItemPerUom (' + colname + ' TEXT)')
            } else {
                if (!existCol.some(column => column.name === colname)) {
                    Sql.Execute("ALTER TABLE cache.TransInventoryCountItemPerUom ADD COLUMN " + colname + " TEXT");
                }                
            }
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
                mData = mData.Rows;

                var arrBinProductCategories = []

                for (var i in mData) {
                    var existingCategory = arrBinProductCategories.find(
                        item => item.Category.toLowerCase() === mData[i].ProductCategory.toLowerCase()
                    );
                    if (existingCategory) {
                        existingCategory.Sku.push(mData[i])
                    } else {
                        arrBinProductCategories.push({
                            Category: mData[i].ProductCategory,
                            ID: NewID(),
                            Sku: [mData[i]]
                        })
                    }
                }
                arrBinProductCategories = $.merge([{ Category: 'All Categories', ID: NewID() }], arrBinProductCategories.sort((a, b) => a.Category.localeCompare(b.Category)));
                console.log(arrBinProductCategories)
                var listBinCategories = $('#listBinCategories').empty();
                var listBinCategorySku = $('#listBinCategorySku').empty();

                for (var i in arrBinProductCategories) {
                    listBinCategories.append($('<li style="flex: 0 0 auto;">')
                        .append($('<button data-role="none" id="' + arrBinProductCategories[i].ID +'" class="btn btn-outline-dark rounded-pill btnBinCat">')
                            .text(arrBinProductCategories[i].Category)
                            .data('data', arrBinProductCategories[i])
                            .click(function () {
                                var dis = $(this), data = dis.data('data');

                                $('.btnBinCat').removeClass('active');
                                dis.addClass('active');
                                document.getElementById(data.ID).scrollIntoView({ behavior: "smooth", block: "center", inline: "center" });
                                if (data.Category == 'All Categories') {
                                    $('[subpage="bincategories"]').pageshow();
                                } else {
                                    $('[catid="' + data.ID + '"]').pageshow();
                                }
                            })
                        )
                    )

                    if (arrBinProductCategories[i].Category != 'All Categories') {
                        var parent = $('<div>');

                        parent.attr('catid', arrBinProductCategories[i].ID);
                        parent.attr('subpage', 'bincategories');
                        //parent.hide();

                        parent.append($('<div class="text-secondary border-primary p-2 pl-3 pr-3 font-weight-bold" style="background-color: #F5F7FA;">').text(arrBinProductCategories[i].Category));

                        for (var s in arrBinProductCategories[i].Sku) {
                            var skuData = arrBinProductCategories[i].Sku[s];

                            parent.append($('<div class="row_container p-3 border ' + (CDec2(skuData.Balance) > 0 ? 'bg-success-light' : 'bg-danger-light') + ' listProduct">')
                                .attr('productid', skuData.ID)
                                .append($('<div class="row align-items-center">')
                                    .append($('<div class="col divProductInfo">')
                                        .append('<div class="text-dark font-weight-bold">' + skuData.ProductNo + '</div>')
                                        .append('<div class="text-secondary">' + skuData.ProductName + '</div>')
                                        .append('<div class="text-secondary">Unit Measure: ' + skuData.UnitMeasure + '</div>')
                                        .append((CBool(sessionStorage.IsVendingMachine) && skuData.BinNo != '' ? '<div class="text-secondary">Motor No.: ' + skuData.BinNo + '</div>' : ''))
                                    )
                                    .append($('<div class="col-auto">')
                                        .append($('<input data-role="none" type="number" class="form-control text-center txtBinCount" placeholder="Enter Count" value="' + (CDec2(skuData.Balance) || '') + '">')
                                            .data('data', skuData)
                                            .keyup(function (e) {
                                                var dis = $(this), parent = dis.parents().closest('.row_container');
                                                if (CDec2(dis.val()) == 0) {      
                                                    dis.attr('value', '');
                                                    parent.removeClass('bg-primary-light bg-success-light').addClass('bg-danger-light');
                                                } else {
                                                    dis.attr('value', dis.val());
                                                    parent.removeClass('bg-success-light bg-danger-light').addClass('bg-primary-light');
                                                }
                                                if (e.keyCode == 13) {
                                                    //enter
                                                    var curIndex = $('.listProduct').index(parent);
                                                    var nextMissingProduct = $('.listProduct .txtBinCount:empty').filter(function () {
                                                        return $('.listProduct .txtBinCount').index(this) > curIndex;
                                                    }).first();

                                                    if (nextMissingProduct.length == 0) {
                                                        var uncountedItem = $('.listProduct .txtBinCount:empty').first();
                                                        if (uncountedItem.length > 0) {
                                                            uncountedItem.focus();
                                                        }
                                                    } else nextMissingProduct.focus();
                                                }
                                                refreshBinCount();
                                            })
                                        )
                                    )
                                )
                                .data('data', skuData)
                            )
                        }

                        listBinCategorySku.append(parent);
                    }
                }

                refreshBinCount();
                //$('[subpage="bincategories"]').pageshow();
                $('.btnBinCat:eq(0)').addClass('active');
                return
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

    function refreshBinCount() {
        $('#lblBinMissingCount').text($('.row_container.bg-danger-light').length);
        $('#lblBinReadyCount').text($('.row_container.bg-primary-light').length);
        $('#lblBinSubmittedCount').text($('.row_container.bg-success-light').length);
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
            "initComplete": function () {
                $('#tblShelfPrices').removeClass('dataTable').css('width', '100%');
            },
            "sDom": '<"top">rt<"bottom"ip><"clear">',
            "pageLenght": 25
        });
    }

    function DisplayFowstBin(vData) {

        if ($.fn.DataTable.isDataTable(BinsTable)) {
            //empty the table first before destroying datatabase para dili magdoble ang header
            BinsTable.empty();
            binDatatable.destroy();
        }

        binDatatable = BinsTable.DataTable({
            paging: false,
            "data": vData.Rows,
            "columns": [
                { "data": "BinNo" },
                { "data": "ProductNo" },
                { "data": "ProductName" },
                { "data": "UnitMeasure" },
                {
                    "data": function (data, type, full, meta) {
                        return '<input data-role="none" type="number" class="form-control">'
                    }
                }
            ],
            "aoColumnDefs": [{ "bSortable": false, "aTargets": '_all' }],
            "columnDefs": [{ "targets": 0, "orderable": false }],
            "initComplete": function () {
                $('#tblNamedBins').removeClass('dataTable').css('width', '100%');
            },
            "rowCallback": function (row, data, index) {
                $(row).data('data', data);
            },
            "sDom": '<"top">rt<"bottom"i><"clear">' //hide search label and field
            //"fnFooterCallback": function (nRow, aaData, iStart, iEnd, aiDisplay) {
            //    if (aiDisplay.length == 1) {
            //        $('#tblNamedBins').click();
            //    }
            //}
        });
        return;

        //mao ni ang original UI for Bin Loc
        //changed by Arnie on Nov 18, 2024
        //icomment lang akong code sa taas para mabalik sa orig UI
        //gicomment para swak sa Vending Machine and para dali magamit
        binDatatable = BinsTable.DataTable({
            "pageLenght": 25,
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
            "initComplete": function (){
                $('#tblNamedBins').removeClass('dataTable').css('width', '100%');
            },
            "sDom": '<"top">rt<"bottom"ip><"clear">' //hide search label and field
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
                $('#pageBinContent').pageshow();
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
            $('#pageStockViewer').pageshow();
            DisplayUnitMeasures(mData);
        }
        else {
            AjaxSend({
                url: 'Receiving/GetStockUnitMeasures',
                data: data,
                success: function (vData) {
                    $('#pageStockViewer').pageshow();
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

                        $('#pageCountInformation').pageshow();

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
                        $('#pageIndex').pageshow();
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
                    NewMessageBox("Your count has been saved.",
                            "Save",
                            [
                                {
                                    Text: 'Ok',
                                    Click: function () {
                                        if (!(window.location.hash.substring(1).indexOf('PageCountByUPC') >= 0 || window.location.hash.substring(1).indexOf('PageCountByTableType') >= 0)) {
                                            SetInventoryCountFormat();
                                            //setTimeout(function () {
                                            //    $('#pageGroup').pageshow();
                                            //}, 3000);
                                        } else {
                                            //$('.ui-dialog').dialog('close');
                                            //$("#pageMain").pageshow();
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
                            NewMessageBox("Your count has been saved.",
                            "Save",
                            [
                                { Text: 'Ok',
                                    Click: function () {
                                        //GotoFirstPage();
                                        ClearFields();
                                        SetInventoryCountFormat();
                                        //$('#pageGroup').pageshow();
                                    }
                                }
                            ]);
                        }
                    } else if (CStr2(vData.Save) != '') {
                        if (!CBool(sessionStorage.IsInventoryCountMode)) {
                            NewMessageBox(vData.Save, 'Not saved');
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
                NewMessageBox("Please select a FOWST", "FOWST",
                            [
                                { Text: 'Ok' }
                            ]);
                return;
            }
        }

        arrCountItems = [];
        $('#divInventoryType').show();
        $('.btnSelectCategory').parents().closest('div.row').hide();

        if ($.fn.DataTable.isDataTable(ProductsInCategoriesTable)) {
            //empty the table first before destroying datatabase para dili magdoble ang header
            ProductsInCategoriesTable.empty();
            ProductsInCategoriesDatatable.destroy();
        }            

        switch (GroupSelected) {
            case 'ByNegativeBalances':
                {
                    IsHideGroup(true);
                    GetAllProducts();
                    $('#pageGroup').pageshow();
                    break;
                }
            case 'ByBin':
                {
                    $('.lblInventoryType').text('By Bin Location');
                    $('#pageCountByBinAndArea').pageshow();
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
                    $('#pageGroup').pageshow();
                    break;
                }
            case 'BySpatial':
                {
                    $('.lblInventoryType').text('By Product');
                    $('#PageCategories').pageshow();
                    //IsHideGroup(true);
                    //$('#pageGroup').pageshow();
                    break;
                }
            case 'ByInventoryCountItems':
                {
                    $('.lblInventoryType').text('');
                    $('#PageCategories').pageshow();
                    $('#divInventoryType').hide();
                    $('.lblSelectedCategory').text('Select Category');
                    Bs.Loading('show');
                    GetAllProductsForTableType('', '');
                    break
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
                    $('#pageGroup').pageshow();
                    break;
                }
            case 'ByUPCOrBarcode':
                {
                    GetNamedFowstBin();
                    $('.lblInventoryType').text('By Continuous Scanning of UPC/Barcode');
                    $('#PageCountByUPC').pageshow()
                        
                    break;
                }
            case 'ByTableType':
                {
                    $('.lblInventoryType').text('By Product Category');
                    $('#PageCategories').pageshow();
                    $('.btnSelectCategory').parents().closest('div.row').show();
                    //GetBalancesFromServer();
                    GetCategories();                        
                    break;
                }
            case 'ByArea':
                {
                    $('.lblInventoryType').text('By Area Location');
                    $('#pageCountByBinAndArea').pageshow();

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

            NewMessageBox("Please select " + m_type,
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
            //mData = InventoryCount.GetCategories();
            //DisplayCategories(mData.Rows);
            DisplayCategoriesPopupMode();//mData.Rows
        }
    }

    function GetAllProductsForTableType(Feature, vSearch) {
        setTimeout(function () {
            if (IsMobile) {
                //$('#PageCountByTableType').pageshow();
                mData = InventoryCount.GetAllProducts(Feature, vSearch, true, $('#selInventoryType option:selected').val(), GroupSelected);

                ProductsInCategoriesRows = mData.Rows;
                //if (ProductsInCategoriesDatatable != null)
                //    ProductsInCategoriesDatatable.rows.add(ProductsInCategoriesRows);
                DisplayAllProductsForTableType();
                Bs.Loading('hide');

                $('#selInventoryType').unbind('change').change(function () {
                    Bs.Loading('show');
                    setTimeout(function () {
                        GetAllProductsForTableType(Feature, vSearch);
                    })
                })
            }
        }, 300);
    }

    function DisplayCategoriesPopupMode(vRows) {
        window.ICGetCategories = InventoryCount.GetCategories
        var vdata = [{ Category: 'All Categories' }];

        vdata = $.merge(vdata, ICGetCategories(GroupSelected).Rows);

        for (var i = 0; i < vdata.length; i++) {
            vdata[i].Description = vdata[i].Category;
            vdata[i].Click = function (e) {
                Bs.Loading('show');

                data = {
                    FOWSTID: sessionStorage.SelectedFOWST,
                    DisplayType: 'fowst',
                    ProductID: ProductID,
                    Category: e.Category
                }

                $('.lblSelectedCategory').text(e.Category)
                $('#txtSearchProducts').val('');
                CategoryType = e.Category;
                GetAllProductsForTableType(e.Category, '');
            }
        }

        PopupButtonList('', 'Select Product Category', vdata, {
            AutoDismiss: false
        })
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
            "initComplete": function(){
                $('#tblProductsInCategories').removeClass('dataTable').css('width', '100%');
            },
            rowCallback: function (row, data, displayNum, displayIndex, dataIndex) {
                var inpcount = $(row).find('input'), currentCount = '';
                inpcount.data('data', data);

                $.grep(arrCountItems, function (e, i) {
                    if (e.ID == data.ID) {
                        currentCount = e.Count;
                        return false;
                    }
                })

                inpcount.val(currentCount);

                inpcount.unbind('blur').blur(function () {
                    var isExist = false, index = null;
                    var val = $(this).val();

                    $.grep(arrCountItems, function (e, i) {
                        if (e.ID == data.ID) {
                            isExist = true;
                            index = i;
                            return false;
                        }
                    })

                    if (val == '') {
                        if (isExist) {
                            arrCountItems.splice(index, 1);
                        }
                    } else {
                        if (isExist) {
                            arrCountItems[index].Count = val;
                        } else {
                            data.Count = val;
                            arrCountItems.push(data);
                        }
                    }
                })
            },
            "columns": [
                {
                    "data": function (data) {
                        return data.ProductNo + '-' + data.ProductName
                    }
                },
                {
                    "data": function (data) {                           
                        //return "<button data-role='none' class='btn btn-success'>" + data.UnitMeasure + "</button>";
                        return "<button data-role='none' class='btn btn-success'> Open </button>";
                    }
                },
                {
                    "data": function (data) {
                        return "<input type=number data-role='none' class='form-control' disabled />";
                    }
                }
                //{
                //    "data": function (data) {
                //        return AccountingFormat(data.balance)
                //    }
                //},
                //{
                //    "data": function (data) {
                //        return AccountingFormat(data.LastCount)
                //    }
                //},
                //{
                //    "data": function (data) {
                //        return '-'
                //    }
                //}
                //,
                //{
                //    "data": function (data) {
                //        return "<button class='btnRemarks' type='button'>Add</button>";
                //    }
                //}
            ],
            "bFilter": false,
            "sDom": '<"top">rt<"bottom"ip><"clear">'
        });

        try {
            //$(".btnRemarks").button().button('refresh');
        } catch (e) {
            //$(".btnRemarks").button().button();
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
        if (arrCountItems.length == 0) {
            NewMessageBox('No items selected.');
            return;
        }

        if (!mSummaryMode) {
            var pRows = ProductsInCategoriesDatatable.data();
            mRows = null; //inull kay naga double ang items sa VIEW cart.
            pRows = $.grep(pRows, function (vRow, i) {
                var pCountedRows = $.grep(arrCountItems, function (vCountItem, i) {
                    if (vRow.ID == vCountItem.ID) {
                        return true;
                    }
                });
                return pCountedRows.length > 0;
            });
            if (pRows.length > 0) {
                if (mRows == null)
                    mRows = pRows;
                else
                    mRows = pRows.concat(mRows);
            }
            ProductsInCategoriesDatatable.clear();
            ProductsInCategoriesDatatable.rows.add(mRows || []);
            ProductsInCategoriesDatatable.draw();
            $('#btnSaveObservedCountByTableList').text('Save');
            mSummaryMode = true;
            $('.summary').removeClass('d-none').show();
            $('.not-summary').hide();
            return;
        }
        var Count = [];
        var vBinID;
        var data;
        var mData;
        var BatchID = NewID();
        var vBinIDNew;
        var hasData = false;

        if (FowstID == '') {
            FowstID = $('#selFOWST').val();
        }

        Bs.Loading('show', 'Please wait while your inventory is being validated...');

        setTimeout(function () {
            var isvalid = true;
            console.log(arrCountItems)

            //VALIDATE first before saving
            for (var i = 0; i <= arrCountItems.length - 1; i++) {
                var val = arrCountItems[i].Count;

                if (!Number.isInteger(CDec2(val)) && IsNumeric(val)) {
                    isvalid = false;
                    break;
                }
            }

            if (!isvalid) {
                NewMessageBox('Decimal value is not allowed.', 'Observed Count');
                return;
            }

            if (arrCountItems.length == 0) {
                NewMessageBox('No data to save.');
            } else {
                for (var i = 0; i <= arrCountItems.length - 1; i++) {
                    Bs.Loading('show', 'Saving ' + (i + 1) + ' of ' + arrCountItems.length + ' counted item(s). Please wait...');
                    var val = arrCountItems[i].Count;

                    if (val != '') {
                        hasData = true;
                        mData = {
                            ProductID: arrCountItems[i].ID,
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
                            ProductID: arrCountItems[i].ID,
                            Count: Count,
                            BatchID: BatchID
                        }

                        Count[0] = {
                            BinID: (vBinIDNew || vBinIDTemp),
                            ExpirationDate: 'N/A',
                            UnitMeasureID: arrCountItems[i].MeasurementID,
                            Quantity: CDec2(val),
                            ActualQuantity: 1,
                            IsGoodStock: 1,
                            ExpectedValue: (arrCountItems[i].serverBalance == 'N/A' ? 'NULL' : arrCountItems[i].serverBalance), //serverBalance
                            Variance: (CDec2(val) - CDec2(arrCountItems[i].serverBalance) || 'NULL'),
                            Remarks: ''
                        };

                        //$('#' + $(this).attr('ID').replace('txt', '')).css('visibility', 'hidden');
                        //$('#' + $(this).attr('ID').replace('txt', '')).css('display', 'none');

                        //$('#' + $(this).attr('ID')).val('');

                        //Save(data, Count);

                        var IsSuccessful = false;

                        //return
                        if (OfflineMode) {
                            IsSuccessful = InventoryCount.SaveInventoryCount(data);
                        }

                        if (!IsSuccessful) {
                            //error message
                            return;
                        }
                    }
                }

                arrCountItems = [];
                mSummaryMode = false;
                NewMessageBox('<div class="text-center"><span class="bi-check-circle-fill text-success" style="font-size: 100px;"></span> <br><br>Inventory adjustment saved successfully.</div>');                
                Sql.Execute('UPDATE cache.TransInventoryCountItemPerUom SET Status = "Submitted" WHERE FOWSTID = ' + ReadyString(FowstID) + ' AND Status = "Ready" AND CountDate = ' + ReadyString($.format.date(new Date(), 'MM-dd-yyyy')))
                InventoryCountByCategory();
                SalesPaymentsExport_offline().FlushDeviceData({
                    OnProgress: function (vData) {
                        Bs.Loading('hide');
                    },
                });
                //commented by arnie 060226 - OLD Inventory
                //$('#tblProductsInCategories input').val('');
                //$('.summary').hide();
                //$('.not-summary').show();
                //$('#btnSaveObservedCountByTableList').text('View');
                //DisplayAllProductsForTableType();
            }
                
            Bs.Loading('hide');
        }, 200)            

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
            $('#PageMyCounts').pageshow();
            GetAllMyCounts('true', CategoryType);
        }
        else {
            //$("#pageMain");
        }
            
        //NewMessageBox("Your count has been saved.",
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
                                        NewMessageBox('Product balances has been updated. You can now proceed with inventory count.', 'Inventory Count')
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
                                    NewMessageBox('We\'re having problems connecting to Mobiz Server. Please try again later.')
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
            "pageLenght": 25,
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
                    return '<input type="number" data-role="none" class="form-control">';
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
            "initComplete": function () {
                $('#tblBinProducts').removeClass('dataTable').css('width', '100%');
            },
            "sDom": '<"top">rt<"bottom"ip><"clear">' //hide search label and field
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
        
    var ProductUnitMeasureByCategoryTable = $('#tblProductBinUnitMeasuresByCategory');
    var productUnitMeasureByCategoryDatatable;

    function getAllProductsInBinUnitMeasuresByCategory() {
        var data = {
            ProductID: ProductID
        }

        var mData = InventoryCount.GetStockUnitMeasures(data);

        DisplayAllProductsInBinUnitMeasuresByCategory(mData.Rows);
    }

    function DisplayAllProductsInBinUnitMeasuresByCategory(vData) {
        if ($.fn.DataTable.isDataTable(ProductUnitMeasureByCategoryTable)) {
            //empty the table first before destroying datatabase para dili magdoble ang header
            ProductUnitMeasureByCategoryTable.empty();
            productUnitMeasureByCategoryDatatable.destroy();
        }

        productUnitMeasureByCategoryDatatable = ProductUnitMeasureByCategoryTable.DataTable({
            "data": vData,
            "columns": [
                { "data": "UnitMeasure" },
                { "data": "" }                    
                //{ "data": "Quantity" }
            ],
            "columnDefs": [
            {
                'targets': [1],
                'searchable': false,
                'orderable': false,
                'className': 'dt-body-center',
                'render': function (data, type, full, meta) {
                    return '<input type="number" data-role="none" class="form-control">';
                }
            }],
            "order": [[1, "asc"]],
            "language": {
                "search": "Search Unit Measure"
            },
            "initComplete": function () {
                $('#tblProductBinUnitMeasuresByCategory').removeClass('dataTable').css('width', '100%');
            },
            "sDom": '<"top">rt<"bottom"><"clear">',
            "pageLenght": 25
        });

        //carry over qty
        if (productQty != undefined) {
            $('#tblProductBinUnitMeasures tbody tr td input').each(function () {
                if (productUnitMeasureByCategoryDatatable.row($(this).closest('tr')).data().Quantity == 1) {
                    $(this).val(productQty);
                }
            })
        }
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
                    return '<input type="number" data-role="none" class="form-control">';
                }
            }],
            "order": [[1, "asc"]],
            "language": {
                "search": "Search Unit Measure"                    
            },
            "initComplete": function () {
                $('#tblProductBinUnitMeasures').removeClass('dataTable').css('width', '100%');
            },
            "sDom": '<"top">rt<"bottom"ip><"clear">',
            "pageLenght": 25
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
                    return '<input type="number" data-role="none" class="form-control">';
                }
            },
            {
                'targets': [4],
                'searchable': false,
                'orderable': false,
                'className': 'dt-body-center',
                'render': function (data, type, full, meta) {
                    return '<button data-role="none" class="btn btn-secondary"><span class="bi-upc-scan"></span></button>';
                }
            }],
            "order": [[1, "asc"]],
            "language": {
                "search": "Search Unit Measure"
            },
            "initComplete": function () {
                $('#tblProductBarcode').removeClass('dataTable').css('width', '100%');
            },
            "sDom": '<"top">rt<"bottom"ip><"clear">',
            "pageLenght": 25
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
                    FowstID: sessionStorage.FowstID,
                    ProductID: productDatatable.row($(this).closest('tr')).data().ProductID,
                    Count: Count
                    //,
                    //BatchID: BatchID
                }                    

                if (OfflineMode) {
                    IsSuccessful = InventoryCount.SaveInventoryCount(data);
                    $('#popProductBinCount').modal('hide');
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
                        //$("#pageMain").pageshow();
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
        //$('#lblWorkFowstName').text(sessionStorage.FowstName);
        $('.btnFOWST').text('FOWST: ' + sessionStorage.FowstName);
                        
    }

    function SetByAreaSettings() {
        var tempinterval = setInterval(function () {
            if (typeof AddBreadCrumb != 'undefined') {
                $('.ItemsBreadCrumbs').empty();
                AddBreadCrumb({
                    text: 'Inventory Count', Click: function () {
                        //$("#pageMain").pageshow();
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
                        Bs.Loading('show', 'Saving...');
                        setTimeout(function () {
                            var data = {
                                ProductID: e.ID,
                                FowstID: sessionStorage.FowstID,
                                BinID: BinID
                            }
                            if (!CBool(IsFromBarcode)) {
                                SaveProductWithoutBarcode(e.ID);
                            }
                            SaveBINProducts(data);
                            Bs.Loading('hide');
                            $('#popProductBinCount').modal('show');
                        }, 200)                                    
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

        var pZReadDate = new Date();

        if (Business.SystemDefaults.GetEnforceZRead() || GetEnforceZReadByFOWST()) {
            pZReadDate = GetZReadDay();

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
                                            proj.SetZReadDate(pZReadDate);
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

            }
            //} // END if (CBool(vParams.silent)) {
        }

        function GetEnforceZReadByFOWST() {
            var pSql = 'Select EnforceZRead from LibraryOrganizationFOWST WHERE ID = ' + ReadyString(sessionStorage.FowstID);

            return CBool(Sql.ExecuteScalar(pSql));
        }

        proj.SetZReadDate(pZReadDate);
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
