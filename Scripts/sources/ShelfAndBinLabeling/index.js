require([
    'common',
    GetPathFormat('/Scripts3/Browser/LibraryFowst.js?crc='),
    GetPathFormat('/Scripts/sources/ShelfAndBinLabeling/DownloadShelfPriceTags.js?crc='),
    GetPathFormat('/Scripts/sources/ShelfAndBinLabeling/DownloadProductLabels.js?crc=')

], function () {

    var BinsTable = $('#tblBins');
    var ProductTable = $('#tblBinProducts');
    var FowstBinProductTable = $('#tblFowstBinProduct');
    var productDatatable;
    var binDatatable;
    var fowstbinDatatable;
    var FowstBinProductID = ''
    var BinID;
    var BinName;
    var BinIDs = [];
    var BinAreas = [];
    var BinNameOrder = [];
    var IsSelectAll = false;
    var BinOrder = [];
    var BinOrderCount = 0;

    require([(OfflineMode ? '/Scripts3/Offline/Browser/LibraryFowst.js?crc=' : '')], function () {

        $(function () {
            //if (OfflineMode) {
            //    GetFowst();
            //}
            //else {
            //    GetFowst();
            //}

            $('.OrganizationNameHeader').text(sessionStorage.OrgName);
            $('#txtLoggedUserName').text((sessionStorage.CurrentUserFullName).split(' ')[0]);
            setTimeout(function () {
                if (sessionStorage.FowstID == undefined) {
                    DisplayFowst();
                } else {
                    $('#lblWorkFowstName').text(sessionStorage.FowstName);
                    $('.btnFOWST').text('FOWST: ' + sessionStorage.FowstName)
                    GetNamedFowstBin();
                }
            }, 1000); // END setTimeout(function() {

            var tempinterval = setInterval(function () {
                if (typeof AddBreadCrumb != 'undefined') {
                    if (window.location.pathname.toLowerCase() != '/erp') {
                        $('.ItemsBreadCrumbs').empty();
                        AddBreadCrumb({
                            text: 'Home', Click: function () {
                                window.location = '/People/PeopleMenu';
                            }
                        }, '.ItemsBreadCrumbs')

                        AddBreadCrumb({
                            text: 'Shelf and Bin Labeling', Click: function () {
                                $('#divErpRevenue, #divRevMainMenu').pageshow();
                            }
                        }, '.ItemsBreadCrumbs')
                    }
                    clearInterval(tempinterval)
                }
            }, 100) //home link

            $('#lblWorkFowstName').click(function () {
                DisplayFowst();
            })

            $('#txtSearchBins').keyup(function () {
                binDatatable.search($.trim($(this).val())).draw();
            });

            $('#btnAutoGenerateBinTags').click(function () {
                $('#popAutoGenerateBinTags').show();
            });

            $('#btnManuallyAddBinTags').click(function () {
                NewMessageBox("This feature is under development", "Manully Add Bin Tags",
                            [
                                { Text: 'Ok' }
                            ]);
            });

            $('#btnDeactivateAllBins').click(function () {
                NewMessageBox("Warning! This function will deactivate <b>ALL</b> existing bins in " + sessionStorage.FowstName + " and zero-out quantities to its associated products. <br /> <br /> Are you sure you want to continue deactivating <b>ALL</b> existing bins in " + sessionStorage.FowstName + '?', "Manully Add Bin Tags",
                            [
                                {
                                    Text: 'Yes, Deactivate All Bins',
                                    Click: function () {
                                        DeactivateBinAndProducts(true);
                                    }
                                },
                                {
                                    Text: 'No, Cancel this Action '
                                }
                            ]);
            });

            $('#btnAutoGenerateBinAddresses').click(function () {
                PopupButtonList('', 'Paper Options', [
                        {
                            HasImage: true,
                            Text: '<img src="/Content/images/excel.png" align="absmiddle" class="ui-li-thumb" style="width:50%;"/> <b>Barcode Printer(Recommended)</b> <br /> <div style="text-align:left !important; font-size:13px; font-weight:normal; width:100%;"><ul><li><strong>Description: </strong>Each label will be printed <br /> using a barcode printer or a <br />special purpose printer.</li><li><strong>Label Size: </strong>1.2inch(30.4mm) x 1inch(25.4mm)</li><li><strong>Paper Size: </strong>N/A</li><li><strong>Number of labels per sheet: </strong>N/A</li></ul></div>',
                            Click: function () {
                                sessionStorage.setItem('paperSize', "mediumSize");
                                var f = window.open('ShelfAndBinLabeling/GenerateBarcodeLabels', '_blank');
                                GenerateBins();
                            }
                        },
		                {
		                    HasImage: true,
		                    Text: '<img src="/Content/images/print.png" align="absmiddle" class="ui-li-thumb" style="width:50%;"/> <b>Desktop Printer</b> <br /> <div style="text-align:left !important; font-size:13px; font-weight:normal; width: 100%;"><ul><li><strong>Description: </strong>Multiple labels will be printed <br /> in one sheet using a desktop printer.</li><li><strong>Label Size: </strong>23.30mm x 88.90mm</li><li><strong>Paper Size: </strong>4.53in x 12.43in</li><li><strong>Number of labels per sheet: </strong>12</li></ul></div>',
		                    Click: function () {
		                        sessionStorage.setItem('paperSize', "BondPaperSize");
		                        GenerateBins();
		                        //GenerateQRCodeToPDF();
		                    }
		                },
		                {
		                    HasImage: true,
		                    Text: '<img src="/Content/images/excel.png" align="absmiddle" class="ui-li-thumb" style="width:50%;"/> <b>Barcode Printer</b> <br /> <div style="text-align:left !important; font-size:13px; font-weight:normal; width:100%;"><ul><li><strong>Description: </strong>Each label will be printed <br /> using a barcode printer or a <br />special purpose printer.</li><li><strong>Label Size: </strong>38.1mm x 76.2mm</li><li><strong>Paper Size: </strong>N/A</li><li><strong>Number of labels per sheet: </strong>N/A</li></ul></div>',
		                    Click: function () {
		                        sessionStorage.setItem('paperSize', "largerSize");
		                        GenerateBins();
		                        //GenerateQRCodeToPDF();
		                    }
		                },
                        {
                            HasImage: true,
                            Text: '<img src="/Content/images/excel.png" align="absmiddle" class="ui-li-thumb" style="width:50%;"/> <b>Barcode Printer</b> <br /> <div style="text-align:left !important; font-size:13px; font-weight:normal; width:100%;"><ul><li><strong>Description: </strong>Each label will be printed <br /> using a barcode printer or a <br />special purpose printer.</li><li><strong>Label Size: </strong>1inch(25.4mm) x 2inch(50.8mm)</li><li><strong>Paper Size: </strong>N/A</li><li><strong>Number of labels per sheet: </strong>N/A</li></ul></div>',
                            Click: function () {
                                sessionStorage.setItem('paperSize', "smallerSize");
                                GenerateBins();
                                //GenerateQRCodeToPDF();
                            }
                        },
                ], { AutoDismiss: true });
            })

            $('#btnCancelAutoGenerateBinAddresses').click(function () {
                $('#popAutoGenerateBinTags').hide();
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
            //-----------old code


            //$('#liCreateShelfOrBinID').click(function () {
            //    if ($('#selFOWST').val() == '') {
            //        MessageBox("Please select a FOWST", "FOWST",
            //                [
            //                    { Text: 'Ok' }
            //                ]);
            //        return;
            //    } else {
            //        window.location = '/ShelfAndBinLabeling/AddShelfAndBinLabel';
            //    }

            //});

            //$('#tblBins').find('#thSelectAll').on('dblclick',function () {

            //we used this method because when the dataTable.js loads new data in the Table, event functions will not trigger

            $('body').on('dblclick', '#thSelectAll', function () {
                if (IsSelectAll) {
                    IsSelectAll = false;
                    $('#thSelectAll').text('Select All');
                }
                else {
                    IsSelectAll = true;
                    $('#thSelectAll').text('Deselect All');
                }

                $('#tblBins tbody tr td input').each(function () {
                    $(this).prop('checked', IsSelectAll);
                })
            }).on('click', '#tblBins tbody tr td', function () {

                var mData = [];
                if (binDatatable.data().count() > 0) {
                    if (binDatatable.cell(this).index().columnVisible == 0) {
                        return;
                    }

                    BinID = binDatatable.row(this).data().ID;
                    BinName = binDatatable.row(this).data().Name;
                    mData.push({
                        Text: 'Deactivate Bin Address',
                        Click: function () {
                            GetBinProducts();
                            $('.lblBinLabelHeader').text('Bin ' + BinName);
                            $('#popDeactivateBin').show();
                        }
                    });

                    mData.push({
                        Text: 'See Products Mapped to this Bin',
                        Click: function () {
                            getAllProductsInBin();
                            $('#popProductBinMapping').show();
                            $('#lblProductBinMappingLabel').text('Bin ' + BinName);
                        }
                    });

                    PopupButtonList('', 'Bin ' + BinName,
                        mData, { AutoDismiss: true })
                }
                //});
            }).on('click', '#tblBinProducts tbody tr td', function () {

                if (productDatatable.data().count() > 0) {
                    if (productDatatable.cell(this).index().columnVisible == 2) {

                        FowstBinProductID = productDatatable.row(this).data().ID;

                        NewMessageBox("Are you sure you want to unMap " + productDatatable.row(this).data().ProductName + ' in ' + BinName + ' bin?', "FOWST",
                                [
                                    {
                                        Text: 'Yes, Unmap this Product',
                                        Click: function () {
                                            RemoveProductFromBin(FowstBinProductID);
                                        }
                                    },
                                    { Text: 'No, Cancel this Action ' }
                                ]);

                    }
                }
            })

            $('#chkAisle').click(function () {
                $('#lblAislesOrRack1').text('aisles');
                $('#lblAislesOrRack2').text('aisles');
            });

            $('#chkRack').click(function () {
                $('#lblAislesOrRack1').text('rack');
                $('#lblAislesOrRack2').text('rack');
            });

            $('#btnAddProduct').click(function () {
                GetProducts();
            });

            $('#btnDeactivateBin').click(function () {                
                DeactivateBinAndProducts(false);
            })

            $('#btnCancelDeactivationBin').click(function () {
                $('#popDeactivateBin').hide();
            })

            $('#lnkBATExample').click(function () {
                $('#popBATExample').show();
            })

            //$('#liAssignProductsToBin').click(function () {
            //    location = MergeUrlParameters({
            //        OrgID: sessionStorage.OrgID,
            //        IsMobile: CBool(IsMobile) ? 1 : 0
            //    }, '/ShelfAndBinLabeling/CreateShelfPriceTags');
            //});

            //$('#liCreateBarcodeTags').click(function () {
            //    location = MergeUrlParameters({
            //        OrgID: sessionStorage.OrgID,
            //        IsMobile: CBool(IsMobile) ? 1 : 0
            //    }, '/ShelfAndBinLabeling/CreateBarcodeTags');
            //})

            $('#liDownloadShelfPriceTags').click(function () {
                location = MergeUrlParameters({
                    OrgID: sessionStorage.OrgID,
                    IsMobile: CBool(IsMobile) ? 1 : 0
                }, '/ShelfAndBinLabeling/DownloadShelfPriceTags');
            });

            $('#btnDownloadBinTagsPDF').click(function () {
                var binCount = 0;
                var mData = [];

                if (binDatatable == undefined) {
                    return;
                }

                mData.push({
                    Text: 'Download All Bins of ' + $('#lblWorkFowstName').text(),
                    Click: function () {
                        BinIDs = [];

                        for (var i = 0; i <= binDatatable.rows().data().length - 1; i++) {
                            if (CBool(binDatatable.row(i).data().IsActive)) {
                                BinIDs[binCount] = [];
                                BinIDs[binCount][0] = binDatatable.row(i).data().ID;
                                BinIDs[binCount][1] = binDatatable.row(i).data().Name;
                                BinAreas[binCount] = binDatatable.row(i).data().Area;
                                binCount++;
                            }                            
                        }

                        BinNameOrder[0] = [];
                        BinNameOrder[0][0] = 'Area';
                        BinNameOrder[1] = [];
                        BinNameOrder[1][0] = 'Side';
                        BinNameOrder[2] = [];
                        BinNameOrder[2][0] = 'Section';
                        BinNameOrder[3] = [];
                        BinNameOrder[3][0] = 'Shelf';

                        sessionStorage.BinIDs = JSON.stringify(BinIDs);
                        sessionStorage.BinOrder = JSON.stringify(BinNameOrder);
                        sessionStorage.AreaLabel = JSON.stringify(BinAreas);

                        PopupButtonList('', 'Paper Options', [
                        {
                            HasImage: true,
                            Text: '<img src="/Content/images/excel.png" align="absmiddle" class="ui-li-thumb" style="width:50%;"/> <b>Barcode Printer(Recommended)</b> <br /> <div style="text-align:left !important; font-size:13px; font-weight:normal; width:100%;"><ul><li><strong>Description: </strong>Each label will be printed <br /> using a barcode printer or a <br />special purpose printer.</li><li><strong>Label Size: </strong>1.2inch(30.4mm) x 1inch(25.4mm)</li><li><strong>Paper Size: </strong>N/A</li><li><strong>Number of labels per sheet: </strong>N/A</li></ul></div>',
                            Click: function () {
                                sessionStorage.setItem('paperSize', "mediumSize");
                                var f = window.open('ShelfAndBinLabeling/GeneratePDF', '_blank');
                            }
                        },
		                {
		                    HasImage: true,
		                    Text: '<img src="/Content/images/print.png" align="absmiddle" class="ui-li-thumb" style="width:50%;"/> <b>Desktop Printer</b> <br /> <div style="text-align:left !important; font-size:13px; font-weight:normal; width: 100%;"><ul><li><strong>Description: </strong>Multiple labels will be printed <br /> in one sheet using a desktop printer.</li><li><strong>Label Size: </strong>23.30mm x 88.90mm</li><li><strong>Paper Size: </strong>4.53in x 12.43in</li><li><strong>Number of labels per sheet: </strong>12</li></ul></div>',
		                    Click: function () {
		                        sessionStorage.setItem('paperSize', "BondPaperSize");
		                        var f = window.open('ShelfAndBinLabeling/GeneratePDF', '_blank');
		                    }
		                },
		                {
		                    HasImage: true,
		                    Text: '<img src="/Content/images/excel.png" align="absmiddle" class="ui-li-thumb" style="width:50%;"/> <b>Barcode Printer</b> <br /> <div style="text-align:left !important; font-size:13px; font-weight:normal; width:100%;"><ul><li><strong>Description: </strong>Each label will be printed <br /> using a barcode printer or a <br />special purpose printer.</li><li><strong>Label Size: </strong>38.1mm x 76.2mm</li><li><strong>Paper Size: </strong>N/A</li><li><strong>Number of labels per sheet: </strong>N/A</li></ul></div>',
		                    Click: function () {
		                        sessionStorage.setItem('paperSize', "largerSize");
		                        var f = window.open('ShelfAndBinLabeling/GeneratePDF', '_blank');
		                    }
		                },
                        {
                            HasImage: true,
                            Text: '<img src="/Content/images/excel.png" align="absmiddle" class="ui-li-thumb" style="width:50%;"/> <b>Barcode Printer</b> <br /> <div style="text-align:left !important; font-size:13px; font-weight:normal; width:100%;"><ul><li><strong>Description: </strong>Each label will be printed <br /> using a barcode printer or a <br />special purpose printer.</li><li><strong>Label Size: </strong>1.5inch(38.1mm) x 1inch(25.4mm)</li><li><strong>Paper Size: </strong>N/A</li><li><strong>Number of labels per sheet: </strong>N/A</li></ul></div>',
                            Click: function () {
                                sessionStorage.setItem('paperSize', "smallerSize");
                                var f = window.open('ShelfAndBinLabeling/GeneratePDF', '_blank');
                            }
                        },
                        ], { AutoDismiss: true });
                    }
                });

                mData.push({
                    Text: 'Download Selected Bins In the Table',
                    Click: function () {
                        BinIDs = [];

                        $('#tblBins tbody tr td input').each(function () {
                            if ($(this).prop('checked') == true) {
                                BinIDs[binCount] = [];
                                BinIDs[binCount][0] = binDatatable.row($(this).closest('tr')).data().ID;
                                BinIDs[binCount][1] = binDatatable.row($(this).closest('tr')).data().Name;
                                BinAreas[binCount] = binDatatable.row($(this).closest('tr')).data().Area;
                                binCount++;
                            }
                        });

                        BinNameOrder[0] = [];
                        BinNameOrder[0][0] = 'Area';
                        BinNameOrder[1] = [];
                        BinNameOrder[1][0] = 'Side';
                        BinNameOrder[2] = [];
                        BinNameOrder[2][0] = 'Section';
                        BinNameOrder[3] = [];
                        BinNameOrder[3][0] = 'Shelf';

                        sessionStorage.BinIDs = JSON.stringify(BinIDs);
                        sessionStorage.BinOrder = JSON.stringify(BinNameOrder);
                        sessionStorage.AreaLabel = JSON.stringify(BinAreas);

                        PopupButtonList('', 'Paper Options', [
                        {
                            HasImage: true,
                            Text: '<img src="/Content/images/excel.png" align="absmiddle" class="ui-li-thumb" style="width:50%;"/> <b>Barcode Printer(Recommended)</b> <br /> <div style="text-align:left !important; font-size:13px; font-weight:normal; width:100%;"><ul><li><strong>Description: </strong>Each label will be printed <br /> using a barcode printer or a <br />special purpose printer.</li><li><strong>Label Size: </strong>1.2inch(30.4mm) x 1inch(25.4mm)</li><li><strong>Paper Size: </strong>N/A</li><li><strong>Number of labels per sheet: </strong>N/A</li></ul></div>',
                            Click: function () {
                                sessionStorage.setItem('paperSize', "mediumSize");
                                var f = window.open('ShelfAndBinLabeling/GeneratePDF', '_blank');
                            }
                        },
		                {
		                    HasImage: true,
		                    Text: '<img src="/Content/images/print.png" align="absmiddle" class="ui-li-thumb" style="width:50%;"/> <b>Desktop Printer</b> <br /> <div style="text-align:left !important; font-size:13px; font-weight:normal; width: 100%;"><ul><li><strong>Description: </strong>Multiple labels will be printed <br /> in one sheet using a desktop printer.</li><li><strong>Label Size: </strong>23.30mm x 88.90mm</li><li><strong>Paper Size: </strong>4.53in x 12.43in</li><li><strong>Number of labels per sheet: </strong>12</li></ul></div>',
		                    Click: function () {
		                        sessionStorage.setItem('paperSize', "BondPaperSize");
		                        var f = window.open('ShelfAndBinLabeling/GeneratePDF', '_blank');
		                    }
		                },
		                {
		                    HasImage: true,
		                    Text: '<img src="/Content/images/excel.png" align="absmiddle" class="ui-li-thumb" style="width:50%;"/> <b>Barcode Printer</b> <br /> <div style="text-align:left !important; font-size:13px; font-weight:normal; width:100%;"><ul><li><strong>Description: </strong>Each label will be printed <br /> using a barcode printer or a <br />special purpose printer.</li><li><strong>Label Size: </strong>38.1mm x 76.2mm</li><li><strong>Paper Size: </strong>N/A</li><li><strong>Number of labels per sheet: </strong>N/A</li></ul></div>',
		                    Click: function () {
		                        sessionStorage.setItem('paperSize', "largerSize");
		                        var f = window.open('ShelfAndBinLabeling/GeneratePDF', '_blank');
		                    }
		                },
                        {
                            HasImage: true,
                            Text: '<img src="/Content/images/excel.png" align="absmiddle" class="ui-li-thumb" style="width:50%;"/> <b>Barcode Printer</b> <br /> <div style="text-align:left !important; font-size:13px; font-weight:normal; width:100%;"><ul><li><strong>Description: </strong>Each label will be printed <br /> using a barcode printer or a <br />special purpose printer.</li><li><strong>Label Size: </strong>1.5inch(38.1mm) x 1inch(25.4mm)</li><li><strong>Paper Size: </strong>N/A</li><li><strong>Number of labels per sheet: </strong>N/A</li></ul></div>',
                            Click: function () {
                                sessionStorage.setItem('paperSize', "smallerSize");
                                var f = window.open('ShelfAndBinLabeling/GeneratePDF', '_blank');
                            }
                        },
                        ], { AutoDismiss: true });

                    }
                });

                PopupButtonList('', 'Shelf And Bin Labelling',
                    mData, { AutoDismiss: true })

            });

            //$('#selFOWST').change(function () {
            //    sessionStorage.FowstID = $('#selFOWST').val();
            //});

            //$('#btnInventoryBack').click(function (e) {
            //    e.preventDefault();

            //    window.location = '/People/PeopleMenu';

        });
    });

    function DeactivateBinAndProducts(DeactivateAll) {
        var data = {
            FowstID: sessionStorage.FowstID,
            BinID: BinID,
            DeactivateAll: DeactivateAll
        }

        AjaxSend({
            url: 'FowstBin/DeactivateBin',
            data: data,
            success: function (vData) {
                GetNamedFowstBin();
                $('#popDeactivateBin').hide();
            }
        });
    }

    function GetProducts() {
        BrowsePopup('LibraryPIMProduct', 'Products',
                    [
                        {
                            AutoSearch: true,
                            DisplayText: [{ Column: 'ProductName' }],
                            Click: function (e) {
                                var data = {
                                    ProductID: e.ID,
                                    FowstID: sessionStorage.FOWSTID,
                                    BinID: BinID
                                }
                                AjaxSend({
                                    url: 'FowstBin/SaveBinStock',
                                    data: data,
                                    success: function (vData) {
                                        getAllProductsInBin();
                                    }
                                });
                            }
                        }
                    ], { FilterFowst: 1 }
                ); // END BrowsePopup('LibraryFowst', 'Fowst',
    }

    function GetBinProducts() {
        var data = {
            FowstID: sessionStorage.FowstID,
            BinID: BinID
        }

        AjaxSend({
            url: 'FowstBin/GetAllProductsByBin',
            data: data,
            success: function (vData) {
                DisplayFowstBinProduct(vData.Results.Rows);
            }
        });
    }

    function DisplayFowst() {
        var tempinterval = setInterval(function () {
            if (typeof BrowsePopup != 'undefined') {
                BrowsePopup('LibraryFowst', 'FOWST',
                    [
                        {
                            AutoSearch: true,
                            DisplayText: [{ Column: 'FowstName' }],
                            Click: function (e) {
                                sessionStorage.FowstID = e.ID;
                                sessionStorage.FowstName = e.FowstName;
                                $('#lblWorkFowstName').text(sessionStorage.FowstName);
                                $('.btnFOWST').text('FOWST: ' + sessionStorage.FowstName)
                                GetNamedFowstBin();
                            }
                        }
                    ], { FilterFowst: 1 }
                ); // END BrowsePopup('LibraryFowst', 'Fowst',
                clearInterval(tempinterval)
            }
        }, 100)
    }

    window.GetNamedFowstBin = function() { //ginawang public function para magamit sa downloadshelfpricetags.js
        //i use sessionStorage.FowstID cause this form is used in pos-inventory count mode
        //
        var data;
        var data = {
        //    FowstID: sessionStorage.FowstID,
        //    ProductID: ProductID,
            NoFilter: 1
        }

        if (OfflineMode) {
            var mData = '';
            mData = InventoryCount.LibraryFowstBin(data);
            DisplayFowstBin(mData)
        }
        else {
            AjaxSend({
                url: 'Browser/LibraryFowstBin',
                data: data,
                success: function (vData) {
                    DisplayFowstBin(vData);
                }
            });
        }
    }

    function DisplayFowstBin(vData) {
        
        IsSelectAll = false;

        if ($.fn.DataTable.isDataTable(BinsTable)) {
            //empty the table first before destroying datatabase para dili magdoble ang header
            BinsTable.empty();
            binDatatable.destroy();                        
        }        

        binDatatable = BinsTable.DataTable({            
            "data": vData.Results.Rows,
            "columns": [
                { "": "" },
                { "data": "Area" },
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
                }
            ],
            //add customer object ex. checkbox
            "columnDefs": [
            {
                'targets': [ 0 ],
                'searchable': false,
                'orderable': false,
                'className': 'dt-body-center',
                'render': function (data, type, full, meta) {
                    return '<input type="checkbox">';
                }
            },
            {
                'targets': [ 4 ],
            //},
            //{
            //    'targets': [ 4 ],
            //    'searchable': false,
            //    'orderable': false,
                //'className': 'dt-body-center',
                'filter': function (data, type, full, meta) {                    
                    return CBool(data.IsActive);
                }
                
            }],
            //hide column
            //{
            //    "targets": [ 0 ],
            //    "visible": false,
            //    "searchable": false
            //}
            //],
            //customized table
            //"scrollY": "200px",
            //"scrollCollapse": true,
            //"paging": false,
            "lengthMenu": [[25, 50, -1], [25, 50, "All"]],
            "language": {
                "search": "Search Bin "
            }
            
            //"info": false, //search entries
            //"scrollY": '50vh',            
            //"sDom": '<"top"l>rt<"bottom"ip><"clear">' //hide search label and field
        });

        FilterBinTable('Active');
    }

    function getAllProductsInBin() {
        var data = {
            FOWSTID: sessionStorage.FowstID,
            BinID: BinID
        }

        AjaxSend({
            url: 'InventoryCount/GetAllBinStock',
            data: data,
            success: function (vData) {
                DisplayProductsInBin(vData);
            }
        });        
    }

    function DisplayProductsInBin(vData) {

        if ($.fn.DataTable.isDataTable(ProductTable)) {
            //empty the table first before destroying datatabase para dili magdoble ang header
            ProductTable.empty();
            productDatatable.destroy();
        }

        productDatatable = ProductTable.DataTable({
            "data": vData.BinStock.Rows,
            "columns": [
                { "data": "StockNumber" },
                { "data": "ProductName" },
                { "data": "" }
            ],
            "columnDefs": [
            {
                'targets': [2],
                'searchable': false,
                'orderable': false,
                'className': 'dt-body-center',
                'render': function (data, type, full, meta) {
                    return '<a href="#"><img alt="Delete" class="img-cancel" style="width:18px;" /></a>';
                }
            }],            
            "language": {
                "search": "Search Product within the list "
            }
        });
    }

    function DisplayFowstBinProduct(vData) {

        if ($.fn.DataTable.isDataTable(FowstBinProductTable)) {
            //empty the table first before destroying datatabase para dili magdoble ang header
            FowstBinProductTable.empty();
            fowstbinDatatable.destroy();
        }

        fowstbinDatatable = FowstBinProductTable.DataTable({
            "data": vData,
            "columns": [                
                { "data": "ProductName" }
            ],
            "info": false, //search entries
            "scrollY": '50vh',
            "scrollY": "200px",
            "scrollCollapse": true,
            "paging": false,
            "sDom": '<"top"l>rt<"bottom"ip><"clear">' //hide search label and field
        });
    }

    function RemoveProductFromBin(FOWSTBINProductID) {
        var data = {
            ID: FOWSTBINProductID
        }

        AjaxSend({
            url: 'ShelfAndBinLabeling/RemoveProductFromBin',
            data: data,
            success: function (vData) {
                getAllProductsInBin();
                $('#popProductBinMapping').show();
                $('#lblProductBinMappingLabel').text('Bin ' + BinName);
            }
        });    
    }

    function GenerateBins() {
        var NewBinOrder = []; //useless sa sa karonn
        var NewBinOrderCounter = 0;
        var TotalBinLabel = '';

        var BinName = '';
        var BinNames = [];
        var BinAreas = [];
        var BinNamesCounter = 0;

        var mMax;
        var mValue;

        for (var a = 1; a <= parseInt($('#txtRackOrAisles').val()) ; a++) {
                if ($('#chkRack').is(':checked') || $('#chkAisle').is(':checked')) {

                    if ($('#chkAisle').is(':checked')) {
                        mMax = 2;
                    } else {
                        mMax = parseInt($('#txtRackOrAisles').val());
                    }

                    for (var b = 1; b <= mMax; b++) {

                        if ($('#chkAisle').is(':checked')) {
                            if (b == 1) {
                                mValue = 'L';
                            } else {
                                mValue = 'R';
                            }
                        } else {
                            //mValue = b;
                        }

                        //if ($('#chkSection').is(':checked')) {
                            for (var c = 1; c <= parseInt($('#txtSection').val()) ; c++) {
                                //if ($('#chkShelf').is(':checked')) {
                                for (var d = 1; d <= parseInt($('#txtShelf').val()) ; d++) {
                                    if ($('#chkAisle').is(':checked')) {
                                        BinNames[BinNamesCounter] = a + '.' + mValue + '.' + c + '.' + d;
                                    } else {
                                        BinNames[BinNamesCounter] = a + '.' + c + '.' + d;
                                    }

                                    if (!$('#txtArea').val() == '') {
                                        BinAreas[BinNamesCounter] = $('#txtArea').val();
                                    }

                                        BinNamesCounter++;
                                    }
                                //} //end Rack Shelf
                            }
                        //} //end Rack Section
                    }
                } //end Rack 
            }
        //} //end Area
        
        var BinID = [];        
        var binCount = 0;

        var binIds = [];
        for (var i = 0; i <= BinNames.length-1; i++) {
            ID = NewID();
            binIds.push(ID);
        }

        var data = {
            BinID: binIds,
            FowstID: sessionStorage.FowstID,
            Area: $('#txtArea').val(),
            BinNames: BinNames,
        };

        AjaxSend({
            url: 'FowstBin/SaveBinBatch',
            data: data,
            success: function (vData) {

                for (var i = 0; i <= vData.BinName.length-1; i++) {
                    if (vData.BinName[i] != null) {
                        BinID[binCount] = [];
                        BinID[binCount][0] = vData.BinID[i];
                        BinID[binCount][1] = vData.BinName[i];
                        binCount++;
                    }
                }

                BinNameOrder[0] = [];
                BinNameOrder[0][0] = ($('#chkRack').is(':checked') ? 'Rack' : 'Aisle');

                if ($('#chkRack').is(':checked')) {                    
                    BinNameOrder[1] = [];
                    BinNameOrder[1][0] = 'Section';
                    BinNameOrder[2] = [];
                    BinNameOrder[2][0] = 'Shelf';

                }
                else {
                    BinNameOrder[1] = [];
                    BinNameOrder[1][0] = 'Side';
                    BinNameOrder[2] = [];
                    BinNameOrder[2][0] = 'Section';
                    BinNameOrder[3] = [];
                    BinNameOrder[3][0] = 'Shelf';
                }
                

                sessionStorage.BinIDs = JSON.stringify(BinID);
                sessionStorage.BinOrder = JSON.stringify(BinNameOrder);
                sessionStorage.AreaLabel = JSON.stringify(BinAreas);
                
                var f = window.open('ShelfAndBinLabeling/GeneratePDF', '_blank');

                NewMessageBox('Your Bins has been saved. Were you able to download the PDF file?',
	                        "Save",
	                        [
	                            {
	                                Text: 'Yes',
	                                Click: function () {
	                                    GetNamedFowstBin();
	                                }
	                            },
	                            {
	                                Text: 'No',
	                                Click: function () {
	                                    var f = window.open('ShelfAndBinLabeling/GeneratePDF', '_blank');
	                                }
	                            }
	                        ]); // END MessageBox

            }
        });

        //CreateBins(BinNames);
    }


    function FilterBinTable(filterType) {
        binDatatable.column(4)
            .search(filterType,false,false,false)
            .draw();
        //if (filterType = 'Active') {

        //}
    }
    
    //function GetFowst() {
    //    var data = { FilterFowst: 1 }
    //    AjaxSend({
    //        url: 'Browser/LibraryFowst',
    //        data: data,
    //        success: function (vData) {
    //            DisplayFowst(vData.Results);
    //        }
    //    });
    //}

    //function DisplayFowst(vData) {
    //    $('#selFOWST').empty();
    //    $('#selFOWST').append(new Option('Select', '', false));

    //    for (var i = 0; i <= vData.Rows.length - 1; i++) {
    //        $('#selFOWST').append(new Option(vData.Rows[i].FowstName, vData.Rows[i].ID, false));
    //    }

    //    try {
    //        $('#selFOWST').selectmenu('refresh');
    //    }
    //    catch (e) {
    //        $('#selFOWST').selectmenu();
    //    }
    //}
});