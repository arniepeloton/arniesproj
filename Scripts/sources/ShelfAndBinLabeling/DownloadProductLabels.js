require([
    'common'
], function () {

    var ProductBarcodeTable = $('#tblProductBarcode');
    var productbarcodeDatatable;
    var BinID;
    var ProductID;
    var ProductDescription;
    var AreaID;
    var AreaName;
    var BinName;
    var IsSelectAll = false;
    var IsAllProductsQuery = false;
    var BarcodeTags;

    $(document).ready(function () {

        $('body').on('dblclick', '#thBarcodeSelectAll', function () {
            if (IsSelectAll) {
                IsSelectAll = false;
                $('#thBarcodeSelectAll').text('Select All');
            }
            else {
                IsSelectAll = true;
                $('#thBarcodeSelectAll').text('Deselect All');
            }

            $('#tblProductBarcode tbody tr td input').each(function () {
                $(this).prop('checked', IsSelectAll);
            })
        }).on('click', '#tblProductBarcode tbody tr td', function () {

            var mData = [];
            if (productbarcodeDatatable.data().count() > 0) {
                if (productbarcodeDatatable.cell(this).index().columnVisible == 0) {
                    return;
                }

                ProductID = productbarcodeDatatable.row(this).data().ID;
                ProductDescription = productbarcodeDatatable.row(this).data().Description;
                mData.push({
                    Text: 'Add Barcode',
                    Click: function () {
                        $('#lblItemLabelHeader').text(ProductDescription);
                        $('#popAddItemBarcode').modal('show');
                    }
                });

                PopupButtonList('', 'Add Barcode',
                    mData, { AutoDismiss: true })
            }
        });

        $('#liCreateBarcodeLabels').click(function () {
            $('#popItemLabels').modal('show').unbind('shown.bs.modal').on('shown.bs.modal', function () {
                GetAllFowstArea();
            })
        })

        $('#btnProductBins').click(function () {
            GetNamedFowstBin();
        })

        $('#btnProductArea').click(function () {
            GetAllFowstArea();
        });

        $('#btnProductBarcodeFilter').click(function () {
            var mData = [];

            mData.push({
                Text: 'All Products',
                Click: function () {
                    $('#lblProductBarcodeFilter').text('All Products');
                    //if (IsAllProductsQuery) FilterBinTable('', false, false, false)
                    //else GetAllFowstBinProducts();
                    GetAllProductsWithOrWithoutBins();
                }
            });

            mData.push({
                Text: 'All Products In Named Bins',
                Click: function () {
                    $('#lblProductBarcodeFilter').text('All Products With Named Bins');
                    if (IsAllProductsQuery) FilterBinTable('', false, false, false)
                    else GetAllFowstBinProducts();
                }
            });
            
            mData.push({
                Text: 'All Products With Barcode',
                Click: function () {
                    $('#lblProductBarcodeFilter').text('Products With Barcode');
                    if (IsAllProductsQuery) FilterBinTable('^(?!\s*$).+', true, false, false)
                    else GetAllFowstBinProducts();
                }
            });

            mData.push({
                Text: 'All Products Without Barcode',
                Click: function () {
                    $('#lblProductBarcodeFilter').text('Products Without Barcode');
                    if (IsAllProductsQuery) FilterBinTable('^$', true, false, false)
                    else GetAllFowstBinProducts();
                }
            });

            mData.push({
                Text: 'All Products With Price Updates',
                Click: function () {
                    $('#lblProductBarcodeFilter').text('Products With Price Updates');
                    GetAllProductsWithPriceUpdates();
                    //FilterBinTable('', false, false, false);
                }
            });

            PopupButtonList('', 'Product Labels',
                mData, { AutoDismiss: true })
        })

        $('#btnDownloadProductLabelsPDF').click(function () {
            var fowstBinProductCount = 0;
            var mData = [];
            var HasBlankBarcode = false;

            if (productbarcodeDatatable == undefined) {
                return;
            }

            mData.push({
                Text: 'Download All Barcode Labels',
                Click: function () {
                    BarcodeTags = [];
                    var HasBlankBarcode = false;

                    for (var i = 0; i <= productbarcodeDatatable.rows().data().length - 1; i++) {
                        var BarCode;

                        if (productbarcodeDatatable.row(i).data().Barcode == '') {
                            if (productbarcodeDatatable.row(i).data().ProductNo == '') {
                                BarCode = '-';
                            } else {
                                BarCode = productbarcodeDatatable.row(i).data().ProductNo;
                            }
                        } else {
                            BarCode = productbarcodeDatatable.row(i).data().Barcode;
                        }

                        if (productbarcodeDatatable.row(i).data().Barcode == '')
                            HasBlankBarcode = true;

                        BarcodeTags.push({
                            ID: productbarcodeDatatable.row(i).data().ID,
                            ProductNo: productbarcodeDatatable.row(i).data().ProductNo,
                            BinID: productbarcodeDatatable.row(i).data().BinID,
                            ProductName: productbarcodeDatatable.row(i).data().ProductName,
                            UnitMeasure: productbarcodeDatatable.row(i).data().UnitMeasure,
                            Price: productbarcodeDatatable.row(i).data().Price,
                            Barcode: BarCode,
                            LastUpdated: productbarcodeDatatable.row(i).data().LastUpdated,
                        })
                        //}
                    }

                    if (HasBlankBarcode) {
                        NewMessageBox("Some of the items doesn't have barcode, do you want to assign Product Number as an alternative barcode for these items?", "Missing Barcode",
                           [
                               {
                                   Text: 'Yes, assign Product Number as Barcode',
                                   Click: function () {

                                   }
                               },
                               { Text: 'No, dont assign Product Number as Barcode' }
                           ]);
                    }

                    sessionStorage['BarcodeTags'] = JSON.stringify(BarcodeTags);

                    PopupButtonList('', 'Paper Options', [
                    {
                        HasImage: true,
                        Text: '<b>Barcode Printer(Recommended)</b> <br /> <div style="text-align:left !important; font-size:13px; font-weight:normal; width:100%;"><ul><li><strong>Description: </strong>Each label will be printed <br /> using a barcode printer or a <br />special purpose printer.</li><li><strong>Label Size: </strong>1.2inch(30.4mm) x 1inch(25.4mm)</li><li><strong>Paper Size: </strong>N/A</li><li><strong>Number of labels per sheet: </strong>N/A</li></ul></div>',
                        Click: function () {
                            sessionStorage.setItem('paperSize', "mediumSize");
                            var f = window.open('ShelfAndBinLabeling/GenerateBarcodeLabels', '_blank');
                            UpdateItemPrintDate();
                        }
                    },
                    {
                        HasImage: true,
                        Text: '<b>Desktop Printer</b> <br /> <div style="text-align:left !important; font-size:13px; font-weight:normal; width: 100%;"><ul><li><strong>Description: </strong>Multiple labels will be printed <br /> in one sheet using a desktop printer.</li><li><strong>Label Size: </strong>23.30mm x 88.90mm</li><li><strong>Paper Size: </strong>4.53in x 12.43in</li><li><strong>Number of labels per sheet: </strong>12</li></ul></div>',
                        Click: function () {
                            sessionStorage.setItem('paperSize', "BondPaperSize");
                            var f = window.open('ShelfAndBinLabeling/GenerateBarcodeLabels', '_blank');
                            UpdateItemPrintDate();
                        }
                    },
                    {
                        HasImage: true,
                        Text: '<b>Barcode Printer</b> <br /> <div style="text-align:left !important; font-size:13px; font-weight:normal; width:100%;"><ul><li><strong>Description: </strong>Each label will be printed <br /> using a barcode printer or a <br />special purpose printer.</li><li><strong>Label Size: </strong>38.1mm x 76.2mm</li><li><strong>Paper Size: </strong>N/A</li><li><strong>Number of labels per sheet: </strong>N/A</li></ul></div>',
                        Click: function () {
                            sessionStorage.setItem('paperSize', "largerSize");
                            var f = window.open('ShelfAndBinLabeling/GenerateBarcodeLabels', '_blank');
                            UpdateItemPrintDate();
                        }
                    },
                    {
                        HasImage: true,
                        Text: '<b>Barcode Printer</b> <br /> <div style="text-align:left !important; font-size:13px; font-weight:normal; width:100%;"><ul><li><strong>Description: </strong>Each label will be printed <br /> using a barcode printer or a <br />special purpose printer.</li><li><strong>Label Size: </strong>1.5inch(38.1mm) x 1inch(25.4mm)</li><li><strong>Paper Size: </strong>N/A</li><li><strong>Number of labels per sheet: </strong>N/A</li></ul></div>',
                        Click: function () {
                            sessionStorage.setItem('paperSize', "smallerSize");
                            var f = window.open('ShelfAndBinLabeling/GenerateBarcodeLabels', '_blank');
                            UpdateItemPrintDate();
                        }
                    }                    
                    ], { AutoDismiss: true });
                }
            });

            mData.push({
                Text: 'Download Selected Barcode Labels On This List',
                Click: function () {
                    BarcodeTags = [];

                    $('#tblProductBarcode tbody tr td input').each(function () {
                        if ($(this).prop('checked') == true) {
                            var BarCode;

                            if (productbarcodeDatatable.row($(this).closest('tr')).data().Barcode == '') {
                                if (productbarcodeDatatable.row($(this).closest('tr')).data().ProductNo == '') {
                                    BarCode = '-';
                                } else {
                                    BarCode = productbarcodeDatatable.row($(this).closest('tr')).data().ProductNo;
                                }
                            } else {
                                BarCode = productbarcodeDatatable.row($(this).closest('tr')).data().Barcode;
                            }

                            BarcodeTags.push({
                                ID: productbarcodeDatatable.row($(this).closest('tr')).data().ID,
                                ProductNo: productbarcodeDatatable.row($(this).closest('tr')).data().ProductNo,
                                BinID: productbarcodeDatatable.row($(this).closest('tr')).data().BinID,
                                ProductName: productbarcodeDatatable.row($(this).closest('tr')).data().ProductName,
                                UnitMeasure: productbarcodeDatatable.row($(this).closest('tr')).data().UnitMeasure,
                                Price: productbarcodeDatatable.row($(this).closest('tr')).data().Price,
                                Barcode: BarCode,
                                LastUpdated: productbarcodeDatatable.row($(this).closest('tr')).data().LastUpdated,

                            })
                        }
                    });

                    //if (HasBlankBarcode) {
                    //    NewMessageBox("Some of the items doesn't have barcode, do you want to assign Product Number as an alternative barcode for these items?", "Missing Barcode",
                    //       [
                    //           {
                    //               Text: 'Yes, assign Product Number as Barcode',
                    //               Click: function () {

                    //               }
                    //           },
                    //           { Text: 'No, dont assign Product Number as Barcode' }
                    //       ]);
                    //}

                    sessionStorage['BarcodeTags'] = JSON.stringify(BarcodeTags);


                    PopupButtonList('', 'Paper Options', [
                    {
                        HasImage: true,
                        Text: '<b>Barcode Printer(Recommended)</b> <br /> <div style="text-align:left !important; font-size:13px; font-weight:normal; width:100%;"><ul><li><strong>Description: </strong>Each label will be printed <br /> using a barcode printer or a <br />special purpose printer.</li><li><strong>Label Size: </strong>1.2inch(30.4mm) x 1inch(25.4mm)</li><li><strong>Paper Size: </strong>N/A</li><li><strong>Number of labels per sheet: </strong>N/A</li></ul></div>',
                        Click: function () {
                            sessionStorage.setItem('paperSize', "mediumSize");
                            var f = window.open('ShelfAndBinLabeling/GenerateBarcodeLabels', '_blank');
                            UpdateItemPrintDate();
                        }
                    },
                    {
                        HasImage: true,
                        Text: '<b>Desktop Printer</b> <br /> <div style="text-align:left !important; font-size:13px; font-weight:normal; width: 100%;"><ul><li><strong>Description: </strong>Multiple labels will be printed <br /> in one sheet using a desktop printer.</li><li><strong>Label Size: </strong>23.30mm x 88.90mm</li><li><strong>Paper Size: </strong>4.53in x 12.43in</li><li><strong>Number of labels per sheet: </strong>12</li></ul></div>',
                        Click: function () {
                            sessionStorage.setItem('paperSize', "BondPaperSize");
                            var f = window.open('ShelfAndBinLabeling/GenerateBarcodeLabels', '_blank');
                            UpdateItemPrintDate();
                        }
                    },
                    {
                        HasImage: true,
                        Text: '<b>Barcode Printer</b> <br /> <div style="text-align:left !important; font-size:13px; font-weight:normal; width:100%;"><ul><li><strong>Description: </strong>Each label will be printed <br /> using a barcode printer or a <br />special purpose printer.</li><li><strong>Label Size: </strong>38.1mm x 76.2mm</li><li><strong>Paper Size: </strong>N/A</li><li><strong>Number of labels per sheet: </strong>N/A</li></ul></div>',
                        Click: function () {
                            sessionStorage.setItem('paperSize', "largerSize");
                            var f = window.open('ShelfAndBinLabeling/GenerateBarcodeLabels', '_blank');
                            UpdateItemPrintDate();
                        }
                    },
                    {
                        HasImage: true,
                        Text: '<b>Barcode Printer</b> <br /> <div style="text-align:left !important; font-size:13px; font-weight:normal; width:100%;"><ul><li><strong>Description: </strong>Each label will be printed <br /> using a barcode printer or a <br />special purpose printer.</li><li><strong>Label Size: </strong>1.5inch(38.1mm) x 1inch(25.4mm)</li><li><strong>Paper Size: </strong>N/A</li><li><strong>Number of labels per sheet: </strong>N/A</li></ul></div>',
                        Click: function () {
                            sessionStorage.setItem('paperSize', "smallerSize");
                            var f = window.open('ShelfAndBinLabeling/GenerateBarcodeLabels', '_blank');
                            UpdateItemPrintDate();
                        }
                    }                    
                    ], { AutoDismiss: true });

                }
            });

            PopupButtonList('', 'Product Labels',
                mData, { AutoDismiss: true })
        })
    })

    function GetNamedFowstBin() {
        var data = {
            FowstID: sessionStorage.FowstID,
            AreaID: AreaID
        }

        if (OfflineMode) {
            var mData = '';
            mData = InventoryCount.LibraryFowstBin(data);
            DisplayNamedFowstBin(mData.Rows);
        }
        else {
            AjaxSend({
                url: 'Browser/LibraryFowstBin',
                data: data,
                success: function (vData) {
                    DisplayNamedFowstBin(vData.Results.Rows);
                }
            });
        }
    }

    function DisplayNamedFowstBin(vRows) {
        var mData = [];

        mData.push({
            Text: 'All',
            data: null,
            Click: function () {
                BinID = 'All';
                $('#lblProductBinAddress').text('All');
                GetAllFowstBinProducts();
            }
        })

        for (var i = 0; i <= vRows.length - 1; i++) {
            mData.push({
                Text: vRows[i].Name,
                data: vRows[i],
                Click: function () {
                    BinID = $(this).attr('data').ID;
                    BinName = $(this).attr('data').Name
                    $('#lblProductBinAddress').text('' + BinName);
                    GetAllFowstBinProducts();
                }
            })
        }

        PopupButtonList('', 'Bin Address',
                    mData, { AutoDismiss: true })
    }

    function GetAllFowstArea() {
        data = { FowstID: sessionStorage.FowstID }

        AjaxSend({
            url: 'ShelfAndBinLabeling/GetFowstArea',
            data: data,
            success: function (vData) {
                DisplayAllFowstArea(vData.Results.Rows);
            }
        });
    }

    function DisplayAllFowstArea(vRows) {
        var mData = [];

        mData.push({
            Text: 'All',
            data: null,
            Click: function () {
                AreaID = 'All';
                $('#lblProductArea').text('All');
                GetAllFowstBinProducts();
            }
        })

        for (var i = 0; i <= vRows.length - 1; i++) {
            mData.push({
                Text: vRows[i].AreaName,
                data: vRows[i],
                Click: function () {
                    AreaID = $(this).attr('data').ID;
                    AreaName = $(this).attr('data').AreaName
                    $('#lblProductArea').text('' + AreaName);
                    GetAllFowstBinProducts();
                }
            })
        }

        PopupButtonList('', 'FOWST Area',
                    mData, { AutoDismiss: true })
    }

    function GetAllProductsWithOrWithoutBins() {
        var pData = {};

        pData.OrgID = sessionStorage.OrgID;

        AjaxSend({
            url: 'ShelfAndBinLabeling/GetAllProductsWithOrWithoutBins',
            data: pData,
            success: function (vData) {
                DisplayFowstBinProducts(vData.Results.Rows)
            }
        })
        IsAllProductsQuery = false;
    }

    function GetAllFowstBinProducts() { 
        var pData = {};
        
        if (AreaID == undefined) {
            GetAllFowstArea();
            return;
        }

        pData.FowstID = sessionStorage.FowstID;
        pData.BinID = BinID;
        pData.OrgID = sessionStorage.OrgID;
        pData.AreaID = AreaID;
        pData.IsProductBarcode = 1;

        AjaxSend({
            url: 'ShelfAndBinLabeling/GetData',
            data: pData,
            success: function (vData) {
                DisplayFowstBinProducts(vData.Results.Rows)
            }
        })

        IsAllProductsQuery = true;
    }

    function GetAllProductsWithPriceUpdates() {
        var pData = {};

        if (AreaID == undefined) {
            GetAllFowstArea();
            return;
        }

        pData.FowstID = sessionStorage.FowstID;
        pData.BinID = BinID;
        pData.OrgID = sessionStorage.OrgID;
        pData.AreaID = AreaID;
        pData.IsProductBarcode = 1;

        AjaxSend({
            url: 'ShelfAndBinLabeling/GetAllProductsWithPriceUpdates',
            data: pData,
            success: function (vData) {
                DisplayFowstBinProducts(vData.Results.Rows)
            }
        })

        IsAllProductsQuery = false;
    }

    function DisplayFowstBinProducts(vData) {

        if ($.fn.DataTable.isDataTable(ProductBarcodeTable)) {
            //empty the table first before destroying datatabase para dili magdoble ang header
            ProductBarcodeTable.empty();
            productbarcodeDatatable.destroy();
        }

        productbarcodeDatatable = ProductBarcodeTable.DataTable({
            "data": vData,
            "columns": [
                { "": "", "width": "20px" },
                { "data": "ProductNo" },
                { "data": "ProductName" },
                { "data": "UnitMeasure" },
                { "data": "Price" },
                { "data": "Barcode" }
            ],
            "columnDefs": [
            {
                'targets': [0],
                'searchable': false,
                'orderable': false,
                'className': 'dt-body-center text-center',
                'render': function (data, type, full, meta) {
                    return '<input type="checkbox" data-role="none" class="form-check-input">';
                }
            }],
            "language": {
                "search": "Search Product "
            },
            "initComplete": function () {
                $('#tblProductBarcode').removeClass('dataTable');
            },
            "sDom": '<"top">rt<"bottom"ip><"clear">'
        });
    }

    function FilterBinTable(input, regex, smart, caseInsen) {
        productbarcodeDatatable.column(5)
            .search(input, regex, smart, caseInsen)
            .draw();        
    }

    function UpdateItemPrintDate() {
        //var data = {
        //    BarcodeTags: BarcodeTags,
        //    FowstID: sessionStorage.FowstID
        //}

        //AjaxSend({
        //    url: 'ShelfAndBinLabeling/UpdateItemPrintDate',
        //    data: data,
        //    success: function (vData) {

        //    }
        //})
    }
})

