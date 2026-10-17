require([
    'common'
], function () {

    var FowstBinProductTable = $('#tblShelfPrices');
    window.fowstbinproductDatatable;
    var PriceLevelID;
    var BinID;
    var AreaID;
    var AreaName;
    var BinName;
    var FilterDisplay;
    var SPT;
    var IsSelectAll = false;

    $(document).ready(function () {

        $('#btnBins').click(function () {
            if (AreaID == undefined) {
                NewMessageBox('Please select FOWST Area first.',
	                        "Select FOWST Area"); // END MessageBox
            } else {
                GetNamedFowstBin();
            }            
        })

        $('#btnPriceLevel').click(function () {
            GetAllPriceLevels();
        })

        $('#btnArea').click(function () {
            GetAllFowstArea()
        })

        $('#btnShowSPT').click(function () {
            FilterTableDisplay();
        });

        $('#btnDownloadShelfPriceTagsPDF').click(function () {
            var fowstBinProductCount = 0;
            var mData = [];

            //if (fowstbinproductDatatable == undefined) {
            //    return;
            //}

            mData.push({
                Text: 'Download All Shelf Labels',
                Click: function () {
                    SPT = [];

                    for (var i = 0; i <= fowstbinproductDatatable.rows().data().length - 1; i++) {
                        //if (CBool(fowstbinproductDatatable.row(i).data().IsActive)) {                            
                            SPT.push({
                                ID: fowstbinproductDatatable.row(i).data().FowstBinProductID,
                                BinID: fowstbinproductDatatable.row(i).data().BinID,
                                ProductName: fowstbinproductDatatable.row(i).data().ProductName,
                                UnitMeasure: fowstbinproductDatatable.row(i).data().UnitMeasure,
                                Price: CDec2(fowstbinproductDatatable.row(i).data().Price),
                                LastUpdated: '-', //$.format.date(new Date(fowstbinproductDatatable.row(i).data().LastUpdated), 'MM/dd/yyyy'),
                                MaxPerProduct: fowstbinproductDatatable.row(i).data().MaxPerProduct,
                                MinPerProduct: fowstbinproductDatatable.row(i).data().MinPerProduct
                            })
                        //}
                    }

                    sessionStorage['PriceTags'] = JSON.stringify(SPT);

                    PopupButtonList('', 'Paper Options', [
                    {
                        HasImage: true,
                        Text: '<b>Barcode Printer(Recommended)</b> <br /> <div style="text-align:left !important; font-size:13px; font-weight:normal; width:100%;"><ul><li><strong>Description: </strong>Each label will be printed <br /> using a barcode printer or a <br />special purpose printer.</li><li><strong>Label Size: </strong>1.2inch(30.4mm) x 1inch(25.4mm)</li><li><strong>Paper Size: </strong>N/A</li><li><strong>Number of labels per sheet: </strong>N/A</li></ul></div>',
                        Click: function () {
                            sessionStorage.setItem('paperSize', "mediumSize");
                            var f = window.open('ShelfAndBinLabeling/GeneratePriceTagsPDF', '_blank');
                        }
                    },
                    {
                        HasImage: true,
                        Text: '<b>Desktop Printer</b> <br /> <div style="text-align:left !important; font-size:13px; font-weight:normal; width: 100%;"><ul><li><strong>Description: </strong>Multiple labels will be printed <br /> in one sheet using a desktop printer.</li><li><strong>Label Size: </strong>23.30mm x 88.90mm</li><li><strong>Paper Size: </strong>4.53in x 12.43in</li><li><strong>Number of labels per sheet: </strong>12</li></ul></div>',
                        Click: function () {
                            sessionStorage.setItem('paperSize', "BondPaperSize");
                            var f = window.open('ShelfAndBinLabeling/GeneratePriceTagsPDF', '_blank');
                        }
                    },
                    {
                        HasImage: true,
                        Text: '<b>Barcode Printer</b> <br /> <div style="text-align:left !important; font-size:13px; font-weight:normal; width:100%;"><ul><li><strong>Description: </strong>Each label will be printed <br /> using a barcode printer or a <br />special purpose printer.</li><li><strong>Label Size: </strong>38.1mm x 76.2mm</li><li><strong>Paper Size: </strong>N/A</li><li><strong>Number of labels per sheet: </strong>N/A</li></ul></div>',
                        Click: function () {
                            sessionStorage.setItem('paperSize', "largerSize");
                            var f = window.open('ShelfAndBinLabeling/GeneratePriceTagsPDF', '_blank');
                        }
                    },
                    {
                        HasImage: true,
                        Text: '<b>Barcode Printer</b> <br /> <div style="text-align:left !important; font-size:13px; font-weight:normal; width:100%;"><ul><li><strong>Description: </strong>Each label will be printed <br /> using a barcode printer or a <br />special purpose printer.</li><li><strong>Label Size: </strong>1inch(38.1mm) x 2inch(25.4mm)</li><li><strong>Paper Size: </strong>N/A</li><li><strong>Number of labels per sheet: </strong>N/A</li></ul></div>',
                        Click: function () {
                            sessionStorage.setItem('paperSize', "smallerSize");
                            var f = window.open('ShelfAndBinLabeling/GeneratePriceTagsPDF', '_blank');
                        }
                    }                    
                    ], { AutoDismiss: true });
                }
            });

            mData.push({
                Text: 'Download Selected Shelf Labels On This List',
                Click: function () {
                    SPT = [];

                    $('#tblShelfPrices tbody tr td input').each(function () {
                        if ($(this).prop('checked') == true) {
                            SPT.push({
                                ID: fowstbinproductDatatable.row($(this).closest('tr')).data().FowstBinProductID,
                                BinID: fowstbinproductDatatable.row($(this).closest('tr')).data().BinID,
                                BinName: fowstbinproductDatatable.row($(this).closest('tr')).data().Bin,
                                ProductNo: fowstbinproductDatatable.row($(this).closest('tr')).data().ProductNo,
                                ProductName: fowstbinproductDatatable.row($(this).closest('tr')).data().ProductName,
                                UnitMeasure: fowstbinproductDatatable.row($(this).closest('tr')).data().UnitMeasure,
                                Price: CDec2(fowstbinproductDatatable.row($(this).closest('tr')).data().Price),
                                LastUpdated: '-', //fowstbinproductDatatable.row($(this).closest('tr')).data().LastUpdated,
                                MaxPerProduct: fowstbinproductDatatable.row($(this).closest('tr')).data().MaxPerProduct,
                                MinPerProduct: fowstbinproductDatatable.row($(this).closest('tr')).data().MinPerProduct,
                                BinArea: sessionStorage.AreaName
                            })
                        }
                    });

                    //$.format.date(new Date(fowstbinproductDatatable.row($(this).closest('tr')).data().LastUpdated), 'MM/dd/yyyy'),

                    sessionStorage['PriceTags'] = JSON.stringify(SPT);


                    //PopupButtonList('', 'Paper Options', [
                    //{
                    //    HasImage: true,
                    //    Text: '<b>Barcode Printer(Recommended)</b> <br /> <div style="text-align:left !important; font-size:13px; font-weight:normal; width:100%;"><ul><li><strong>Description: </strong>Each label will be printed <br /> using a barcode printer or a <br />special purpose printer.</li><li><strong>Label Size: </strong>1.2inch(30.4mm) x 1inch(25.4mm)</li><li><strong>Paper Size: </strong>N/A</li><li><strong>Number of labels per sheet: </strong>N/A</li></ul></div>',
                    //    Click: function () {
                    //        sessionStorage.setItem('paperSize', "mediumSize");
                    //        var f = window.open('ShelfAndBinLabeling/GeneratePriceTagsPDF', '_blank');
                    //    }

                    //},
                    //{
                    //    HasImage: true,
                    //    Text: '<b>Desktop Printer</b> <br /> <div style="text-align:left !important; font-size:13px; font-weight:normal; width: 100%;"><ul><li><strong>Description: </strong>Multiple labels will be printed <br /> in one sheet using a desktop printer.</li><li><strong>Label Size: </strong>23.30mm x 88.90mm</li><li><strong>Paper Size: </strong>4.53in x 12.43in</li><li><strong>Number of labels per sheet: </strong>12</li></ul></div>',
                    //    Click: function () {
                    //        sessionStorage.setItem('paperSize', "BondPaperSize");
                    //        var f = window.open('ShelfAndBinLabeling/GeneratePriceTagsPDF', '_blank');
                    //        UpdateSPTPrintDate();
                    //    }
                    //},
                    //{
                    //    HasImage: true,
                    //    Text: '<b>Barcode Printer</b> <br /> <div style="text-align:left !important; font-size:13px; font-weight:normal; width:100%;"><ul><li><strong>Description: </strong>Each label will be printed <br /> using a barcode printer or a <br />special purpose printer.</li><li><strong>Label Size: </strong>38.1mm x 76.2mm</li><li><strong>Paper Size: </strong>N/A</li><li><strong>Number of labels per sheet: </strong>N/A</li></ul></div>',
                    //    Click: function () {
                    //        sessionStorage.setItem('paperSize', "largerSize");
                    //        var f = window.open('ShelfAndBinLabeling/GeneratePriceTagsPDF', '_blank');
                    //        UpdateSPTPrintDate();
                    //    }
                    //},
                    //{
                    //    HasImage: true,
                    //    Text: '<b>Barcode Printer</b> <br /> <div style="text-align:left !important; font-size:13px; font-weight:normal; width:100%;"><ul><li><strong>Description: </strong>Each label will be printed <br /> using a barcode printer or a <br />special purpose printer.</li><li><strong>Label Size: </strong>1.5inch(38.1mm) x 1inch(25.4mm)</li><li><strong>Paper Size: </strong>N/A</li><li><strong>Number of labels per sheet: </strong>N/A</li></ul></div>',
                    //    Click: function () {
                    //        sessionStorage.setItem('paperSize', "smallerSize");
                    //        var f = window.open('ShelfAndBinLabeling/GeneratePriceTagsPDF', '_blank');
                    //        UpdateSPTPrintDate();
                    //    }
                    //}                    
                    //], { AutoDismiss: true });

                    sessionStorage.setItem('paperSize', "largerSize");
                    var f = window.open('ShelfAndBinLabeling/GeneratePriceTagsPDF', '_blank');
                    UpdateSPTPrintDate();

                }
            });

            PopupButtonList('', 'Shelf Label',
                mData, { AutoDismiss: true })
        })

        $('#liDownloadShelfOrBinID').click(function () {
            $('#popShelfLabels').modal('show').unbind('shown.bs.modal').on('shown.bs.modal', function () {
                //if (PriceLevelID == undefined) {
                //    GetAllPriceLevels();
                //} else if (BinID == undefined) {
                //    GetNamedFowstBin();
                //}
            })
        });

        $('body').on('dblclick', '#thShelfSelectAll', function () {
            if (IsSelectAll) {
                IsSelectAll = false;
                $('#thShelfSelectAll').text('Select All');
            }
            else {
                IsSelectAll = true;
                $('#thShelfSelectAll').text('Deselect All');
            }

            $('#tblShelfPrices tbody tr td input').each(function () {
                $(this).prop('checked', IsSelectAll);
            })
        })

        //GetFowst();

        //$('#selFOWST').change(function () {
        //    if ($(this).val() != "") {
        //        $('#selBINLocation').selectmenu('enable');
        //        DisplayBin();
        //    }
        //    else {
        //        $('#selBINLocation').empty();
        //        $('#selBINLocation').selectmenu('disable');
        //    }
        //})

        //$('#btnSaveAndMap').click(function () {
        //    GetAllProducts();
        //});

        //$('#btnCancel').click(function () {
        //    location = MergeUrlParameters({
        //        OrgID: sessionStorage.OrgID,
        //        IsMobile: CBool(IsMobile) ? 1 : 0
        //    }, '/ShelfAndBinLabeling');
        //})

        //$('#btnDownload').click(function () {
        //    Download();
        //})
    })

    //new code
    function GetAllPriceLevels() {
        var pData = { OrgID: sessionStorage.OrgID }

        AjaxSend({
            url: 'ShelfAndBinLabeling/GetPriceLevel',
            data: pData,
            success: function (vData) {
                DisplayPriceLevels(vData.Results.Rows)
            }
        });        
    }

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

    function DisplayPriceLevels(vRows) {
        var mData = [];

        for (var i = 0; i <= vRows.length - 1; i++) {
            mData.push({
                Text: vRows[i].PriceLevel,
                data: vRows[i],
                Click: function () {
                    PriceLevelID = $(this).attr('data').ID;
                    $('#lblPriceLevel').text('Price Level: ' + $(this).attr('data').PriceLevel);
                    if (AreaID == undefined) {
                        GetAllFowstArea();
                    } else {
                        GetAllFowstBinProducts();
                    }
                    
                }
            })
        }
        
        PopupButtonList('', 'Price Levels',
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
                $('#lblArea').text('Area: All');
                if (PriceLevelID == undefined) {
                    GetAllPriceLevels();
                } else {
                    GetAllFowstBinProducts();
                }
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
                    if (PriceLevelID == undefined) {
                        GetAllPriceLevels();
                    } else {
                        GetAllFowstBinProducts();
                    }
                }
            })
        }

        PopupButtonList('', 'FOWST Area',
                    mData, { AutoDismiss: true })
    }

    function DisplayNamedFowstBin(vRows) {
        var mData = [];

        mData.push({
            Text: 'All',
            data: null,
            Click: function () {
                BinID = 'All';
                $('#lblBinAddress').text('Bin Address: All');
                if (PriceLevelID == undefined) {
                    GetAllPriceLevels();
                } else {
                    GetAllFowstBinProducts();
                }
            }
        })

        for (var i = 0; i <= vRows.length - 1; i++) {
            mData.push({
                Text: vRows[i].Name,
                data: vRows[i],
                Click: function () {
                    BinID = $(this).attr('data').ID;
                    BinName = $(this).attr('data').Name
                    $('#lblBinAddress').text('Bin Address: ' + BinName);
                    if (PriceLevelID == undefined) {
                        GetAllPriceLevels();
                    } else {
                        GetAllFowstBinProducts();
                    }                    
                }
            })
        }

        PopupButtonList('', 'Bin Address',
                    mData, { AutoDismiss: true })
    }

    window.GetAllFowstBinProducts = function() { //gamiton sa index.js shelfpricetags if magchange ng fowst and current tab is SPT
        var pData = {};

        //if (PriceLevelID == undefined) {
        //    GetAllPriceLevels();
        //    return;
        //}

        //if (AreaID == undefined) {
        //    GetAllFowstArea();
        //    return;
        //}
        
        pData.FowstID = sessionStorage.FowstID;
        pData.BinID = BinID;
        pData.OrgID = sessionStorage.OrgID;
        pData.PriceLevelID = PriceLevelID;
        pData.AreaID = AreaID;
        pData.FilterDisplay = FilterDisplay;

        AjaxSend({
            url: 'ShelfAndBinLabeling/GetData',
            data: pData,
            success: function (vData) {
                DisplayFowstBinProducts(vData.Results.Rows)
            }
        })

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
                { "": "", "width" : "20px" },
                {"data": "Bin"},
                {"data": "ProductName"},
                {"data": "UnitMeasure"},
                {"data": "Price"},
                { "data": "LastUpdated" }
            ],
            "columnDefs": [
            {
                'targets': [ 0 ],
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
                $('#tblShelfPrices').removeClass('dataTable');
            },           
            "sDom": '<"top">rt<"bottom"ip><"clear">'
        });
    }

    //-------------
    function Download() {
        var pData = {};

        pData.FowstID = $('#selFOWST').val();
        pData.BinID = $('#selBINLocation').val();
        pData.OrgID = sessionStorage.OrgID;

        AjaxSend({
            url: 'ShelfAndBinLabeling/GetData',
            data: pData,
            success: function (vData) {
                sessionStorage['PriceTags'] = JSON.stringify(vData.Results.Rows);
                var f = window.open('GeneratePriceTagsPDF', '_blank');
            }
        })
    }

    function GetAllProducts() {
        //AjaxSend({
        //    url: 'Browser/LibraryFowstBin',
        //    data: { FowstID: $('#selFOWST').val() },
        //    success: function (vData) {})
    }

    function DisplayBin() {
        var data = { FOWSTID: $('#selFOWST').val() }
        AjaxSend({
            url: 'ShelfAndBinLabeling/GetLibraryFOWSTBin',
            data: data,
            success: function (vData) {

                var bin = vData.Results.Rows;

                if (bin.length > 0) {

                    $('#selBINLocation').append(new Option('Select', '', false));
                    for (var i = 0; i < bin.length; i++) {
                        $('#selBINLocation').append(new Option(bin[i].Name, bin[i].ID, false));
                    }
                }
                else {
                    $('#selBINLocation').append(new Option('No Result(s) Found', '', false));
                }

                $('#selBINLocation').selectmenu('refresh');
            }
        });
    }

    function GetBINLocation(vFOWSTID) {
        var pFinder = new Finder();
        var pFields = pFinder.Fields;
        var pField;

        pFinder.MaxRecords = 0

        pField = pFields.Add('ID');
        pField = pFields.Add('Name');
        pField = pFields.Add('Description');

        pFinder.SqlFrom = 'FROM LibraryFowstBin '

        pFinder.SqlWhere = 'WHERE FowstID = ' + ReadyString(vFOWSTID) + ' AND Name <> \'Unnamed Bin\''
        pFinder.SqlOrder = 'Order By Name'

        return pFinder.Requery();
    }

    function UpdateSPTPrintDate() {
        //var data = {
        //    SPT: SPT,
        //    FowstID: sessionStorage.FowstID,
        //    PriceLevelID: PriceLevelID
        //}

        //AjaxSend({
        //    url: 'ShelfAndBinLabeling/UpdateSPTPrintDate',
        //    data: data,
        //    success: function (vData) {
                
        //    }
        //})

    }

    function FilterTableDisplay() {
            var mData = [];

            mData.push({
                Text: 'All',
                data: null,
                Click: function () {
                    FilterDisplay = 'All';
                    $('#lblShowDisplay').text('All');
                    GetAllFowstBinProducts();
                }
            })

            mData.push({
                Text: 'All SPT With Price Update',
                data: null,
                Click: function () {
                    FilterDisplay = 'UpdatedPrices';
                    $('#lblShowDisplay').text('Unprinted Price Updates');
                    GetAllFowstBinProducts();
                }
            })

            PopupButtonList('', 'Filter SPT',
                   mData, { AutoDismiss: true })
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
})