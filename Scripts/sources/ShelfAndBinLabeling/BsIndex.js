require([
    'common',
    GetPathFormat('/Scripts3/Browser/LibraryFowst.js?crc='),
    GetPathFormat('/Scripts/sources/ShelfAndBinLabeling/DownloadShelfPriceTags.js?crc='),
    GetPathFormat('/Scripts/sources/ShelfAndBinLabeling/DownloadProductLabels.js?crc='),
    (OfflineMode ? GetPathFormat('/Scripts3/model/InventoryCount_Offline.js?crc=') : '')

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
    var InventoryCount;
    var IsBATPage = false;
    var AreaID = '';

    if (OfflineMode) {
        InventoryCount = new App.model.InventoryCount_Offline();
    }

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

            //setTimeout(function () {
            //    if (sessionStorage.FowstID == undefined) {
            //        DisplayFowst();
            //    } else {
            //        $('#lblWorkFowstName').text(sessionStorage.FowstName);
            //        $('.btnFOWST').text(sessionStorage.FowstName)
            //        GetNamedFowstBin();
            //    }
            //}, 1000); // END setTimeout(function() {

            AjaxSend({
                url: '/ShelfAndBinLabeling/GetFowstArea',
                success: function (e) {
                    var vRows = e.Results.Rows;
                    var txtArea = $('#txtArea');
                    txtArea.empty();
                    txtArea.append(new Option('Please select', ''));

                    if (vRows.length > 0){
                        for (var i = 0; i < vRows.length; i++) {
                            txtArea.append(new Option(vRows[i].AreaName, vRows[i].ID));
                        }
                    }
                }
            })

            $('#btnShelfLabelBins').click(function () {
                if (AreaID == undefined) {
                    NewMessageBox('Please select FOWST Area first.',
                                "Select FOWST Area"); // END MessageBox
                } else {
                    GetListOfFOWSTBIN();
                }
            })

            $('#btnFOWST').click(function () {
                DisplayFowst();
            })

            $('#btnBATFOWST').click(function () {
                DisplayFowst();
            })            

            $('#btnShelfLabelArea').click(function () {
                GetAllFowstArea()
            })

            $('#btnBATArea').click(function () {
                GetAllFowstArea()
            })

            $('#btnShelfBack').click(function () {
                window.location = '/People/PeopleMenu';
            })

            $('#lblWorkFowstName').click(function () {
                DisplayFowst();
            })

            $('#txtSearchBins').keyup(function () {
                binDatatable.search($.trim($(this).val())).draw();
            });

            $('#btnAutoGenerateBinTags').click(function () {
                $('#popAutoGenerateBinTags').find('input, select').val('');
                $('#tbodyAutoGenerateBinTags').empty();
                $('#popAutoGenerateBinTags').modal('show');
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

            //================================================== BIN Address Tags ================================================== 

            $('#btnAutoGenerateBinAddresses').click(function () {
                //PopupButtonList('', 'Paper Options', [
                //        {
                //            HasImage: true,
                //            Text: '<b>Barcode Printer(Recommended)</b> <br /> <div style="text-align:left !important; font-size:13px; font-weight:normal; width:100%;"><ul><li><strong>Description: </strong>Each label will be printed <br /> using a barcode printer or a <br />special purpose printer.</li><li><strong>Label Size: </strong>1.2inch(30.4mm) x 1inch(25.4mm)</li><li><strong>Paper Size: </strong>N/A</li><li><strong>Number of labels per sheet: </strong>N/A</li></ul></div>',
                //            Click: function () {
                //                sessionStorage.setItem('paperSize', "mediumSize");
                //                var f = window.open('ShelfAndBinLabeling/GenerateBarcodeLabels', '_blank');
                //                GenerateBins();
                //            }
                //        },
		        //        {
		        //            HasImage: true,
		        //            Text: '<b>Desktop Printer</b> <br /> <div style="text-align:left !important; font-size:13px; font-weight:normal; width: 100%;"><ul><li><strong>Description: </strong>Multiple labels will be printed <br /> in one sheet using a desktop printer.</li><li><strong>Label Size: </strong>23.30mm x 88.90mm</li><li><strong>Paper Size: </strong>4.53in x 12.43in</li><li><strong>Number of labels per sheet: </strong>12</li></ul></div>',
		        //            Click: function () {
		        //                sessionStorage.setItem('paperSize', "BondPaperSize");
		        //                GenerateBins();
		        //                //GenerateQRCodeToPDF();
		        //            }
		        //        },
		        //        {
		        //            HasImage: true,
		        //            Text: '<b>Barcode Printer</b> <br /> <div style="text-align:left !important; font-size:13px; font-weight:normal; width:100%;"><ul><li><strong>Description: </strong>Each label will be printed <br /> using a barcode printer or a <br />special purpose printer.</li><li><strong>Label Size: </strong>38.1mm x 76.2mm</li><li><strong>Paper Size: </strong>N/A</li><li><strong>Number of labels per sheet: </strong>N/A</li></ul></div>',
		        //            Click: function () {
		        //                sessionStorage.setItem('paperSize', "largerSize");
		        //                GenerateBins();
		        //                //GenerateQRCodeToPDF();
		        //            }
		        //        },
                //        {
                //            HasImage: true,
                //            Text: '<b>Barcode Printer</b> <br /> <div style="text-align:left !important; font-size:13px; font-weight:normal; width:100%;"><ul><li><strong>Description: </strong>Each label will be printed <br /> using a barcode printer or a <br />special purpose printer.</li><li><strong>Label Size: </strong>1inch(25.4mm) x 2inch(50.8mm)</li><li><strong>Paper Size: </strong>N/A</li><li><strong>Number of labels per sheet: </strong>N/A</li></ul></div>',
                //            Click: function () {
                //                sessionStorage.setItem('paperSize', "smallerSize");
                //                GenerateBins();
                //                //GenerateQRCodeToPDF();
                //            }
                //        },
                //], { AutoDismiss: true });

                sessionStorage.setItem('paperSize', "largerSize");
                GenerateBins();
            })

            $('#btnDownloadBinTagsPDF').click(function () {
                var binCount = 0;
                var mData = [];

                if (binDatatable == undefined) {
                    return;
                }

                mData.push({
                    Text: 'Download All Bins Tags of ' + $('#lblWorkFowstName').text(),
                    Click: function () {
                        BinIDs = [];

                        for (var i = 0; i <= binDatatable.rows().data().length - 1; i++) {
                            if (CBool(binDatatable.row(i).data().IsActive)) {
                                BinIDs[binCount] = [];
                                BinIDs[binCount][0] = binDatatable.row(i).data().ID;
                                BinIDs[binCount][1] = binDatatable.row(i).data().BinLongName;
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

                        //PopupButtonList('', 'Paper Options', [
                        //{
                        //    HasImage: true,
                        //    Text: '<b>Barcode Printer(Recommended)</b> <br /> <div style="text-align:left !important; font-size:13px; font-weight:normal; width:100%;"><ul><li><strong>Description: </strong>Each label will be printed <br /> using a barcode printer or a <br />special purpose printer.</li><li><strong>Label Size: </strong>1.2inch(30.4mm) x 1inch(25.4mm)</li><li><strong>Paper Size: </strong>N/A</li><li><strong>Number of labels per sheet: </strong>N/A</li></ul></div>',
                        //    Click: function () {
                        //        sessionStorage.setItem('paperSize', "mediumSize");
                        //        var f = window.open('ShelfAndBinLabeling/GeneratePDF', '_blank');
                        //    }
                        //},
		                //{
		                //    HasImage: true,
		                //    Text: '<b>Desktop Printer</b> <br /> <div style="text-align:left !important; font-size:13px; font-weight:normal; width: 100%;"><ul><li><strong>Description: </strong>Multiple labels will be printed <br /> in one sheet using a desktop printer.</li><li><strong>Label Size: </strong>23.30mm x 88.90mm</li><li><strong>Paper Size: </strong>4.53in x 12.43in</li><li><strong>Number of labels per sheet: </strong>12</li></ul></div>',
		                //    Click: function () {
		                //        sessionStorage.setItem('paperSize', "BondPaperSize");
		                //        var f = window.open('ShelfAndBinLabeling/GeneratePDF', '_blank');
		                //    }
		                //},
		                //{
		                //    HasImage: true,
		                //    Text: '<b>Barcode Printer</b> <br /> <div style="text-align:left !important; font-size:13px; font-weight:normal; width:100%;"><ul><li><strong>Description: </strong>Each label will be printed <br /> using a barcode printer or a <br />special purpose printer.</li><li><strong>Label Size: </strong>38.1mm x 76.2mm</li><li><strong>Paper Size: </strong>N/A</li><li><strong>Number of labels per sheet: </strong>N/A</li></ul></div>',
		                //    Click: function () {
		                //        sessionStorage.setItem('paperSize', "largerSize");
		                //        var f = window.open('ShelfAndBinLabeling/GeneratePDF', '_blank');
		                //    }
		                //},
                        //{
                        //    HasImage: true,
                        //    Text: '<b>Barcode Printer</b> <br /> <div style="text-align:left !important; font-size:13px; font-weight:normal; width:100%;"><ul><li><strong>Description: </strong>Each label will be printed <br /> using a barcode printer or a <br />special purpose printer.</li><li><strong>Label Size: </strong>1.5inch(38.1mm) x 1inch(25.4mm)</li><li><strong>Paper Size: </strong>N/A</li><li><strong>Number of labels per sheet: </strong>N/A</li></ul></div>',
                        //    Click: function () {
                        //        sessionStorage.setItem('paperSize', "smallerSize");
                        //        var f = window.open('ShelfAndBinLabeling/GeneratePDF', '_blank');
                        //    }
                        //},
                        //], { AutoDismiss: true });

                        sessionStorage.setItem('paperSize', "largerSize");
                        var f = window.open('ShelfAndBinLabeling/GeneratePDF', '_blank');
                    }
                });

                mData.push({
                    Text: 'Download Selected Bins Tags In the Table',
                    Click: function () {
                        BinIDs = [];

                        $('#tblBins tbody tr td input').each(function () {
                            if ($(this).prop('checked') == true) {
                                BinIDs[binCount] = [];
                                BinIDs[binCount][0] = binDatatable.row($(this).closest('tr')).data().ID;
                                BinIDs[binCount][1] = binDatatable.row($(this).closest('tr')).data().BinLongName;
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

                        //PopupButtonList('', 'Paper Options', [
                        //{
                        //    HasImage: true,
                        //    Text: '<b>Barcode Printer(Recommended)</b> <br /> <div style="text-align:left !important; font-size:13px; font-weight:normal; width:100%;"><ul><li><strong>Description: </strong>Each label will be printed <br /> using a barcode printer or a <br />special purpose printer.</li><li><strong>Label Size: </strong>1.2inch(30.4mm) x 1inch(25.4mm)</li><li><strong>Paper Size: </strong>N/A</li><li><strong>Number of labels per sheet: </strong>N/A</li></ul></div>',
                        //    Click: function () {
                        //        sessionStorage.setItem('paperSize', "mediumSize");
                        //        var f = window.open('ShelfAndBinLabeling/GeneratePDF', '_blank');
                        //    }
                        //},
		                //{
		                //    HasImage: true,
		                //    Text: '<b>Desktop Printer</b> <br /> <div style="text-align:left !important; font-size:13px; font-weight:normal; width: 100%;"><ul><li><strong>Description: </strong>Multiple labels will be printed <br /> in one sheet using a desktop printer.</li><li><strong>Label Size: </strong>23.30mm x 88.90mm</li><li><strong>Paper Size: </strong>4.53in x 12.43in</li><li><strong>Number of labels per sheet: </strong>12</li></ul></div>',
		                //    Click: function () {
		                //        sessionStorage.setItem('paperSize', "BondPaperSize");
		                //        var f = window.open('ShelfAndBinLabeling/GeneratePDF', '_blank');
		                //    }
		                //},
		                //{
		                //    HasImage: true,
		                //    Text: '<b>Barcode Printer</b> <br /> <div style="text-align:left !important; font-size:13px; font-weight:normal; width:100%;"><ul><li><strong>Description: </strong>Each label will be printed <br /> using a barcode printer or a <br />special purpose printer.</li><li><strong>Label Size: </strong>38.1mm x 76.2mm</li><li><strong>Paper Size: </strong>N/A</li><li><strong>Number of labels per sheet: </strong>N/A</li></ul></div>',
		                //    Click: function () {
		                //        sessionStorage.setItem('paperSize', "largerSize");
		                //        var f = window.open('ShelfAndBinLabeling/GeneratePDF', '_blank');
		                //    }
		                //},
                        //{
                        //    HasImage: true,
                        //    Text: '<b>Barcode Printer</b> <br /> <div style="text-align:left !important; font-size:13px; font-weight:normal; width:100%;"><ul><li><strong>Description: </strong>Each label will be printed <br /> using a barcode printer or a <br />special purpose printer.</li><li><strong>Label Size: </strong>1.5inch(38.1mm) x 1inch(25.4mm)</li><li><strong>Paper Size: </strong>N/A</li><li><strong>Number of labels per sheet: </strong>N/A</li></ul></div>',
                        //    Click: function () {
                        //        sessionStorage.setItem('paperSize', "smallerSize");
                        //        var f = window.open('ShelfAndBinLabeling/GeneratePDF', '_blank');
                        //    }
                        //},
                        //], { AutoDismiss: true });

                        sessionStorage.setItem('paperSize', "largerSize");
                        var f = window.open('ShelfAndBinLabeling/GeneratePDF', '_blank');

                    }
                });

                PopupButtonList('', 'Shelf And Bin Labelling',
                    mData, { AutoDismiss: true })

            });

            function DeactivateAllBinsInArea(AreaName) {
                var data = {
                    AreaName: AreaName
                }

                AjaxSend({
                    url: 'FowstBin/DeactivateBinsInArea',
                    data: data,
                    success: function (vData) {
                        GetNamedFowstBin();
                    }
                });
            }

            //================================================== End of BIN Address Tags ================================================== 

            $('#btnCancelAutoGenerateBinAddresses').click(function () {
                $('#popAutoGenerateBinTags').modal('hide');
            });

            $('#btnFilterBinList').click(function () {
                PopupButtonList('', 'Filter Bin List', [
                    {
                        Text: 'Show Active Bins Only',
                        Click: function () {
                            $('#lblFilterBins').text('Active Bins')
                            FilterBinTable('Active');
                        }
                    },
                    {
                        Text: 'Show Inactive Bins Only',
                        Click: function () {
                            $('#lblFilterBins').text('Inactive Bins')
                            FilterBinTable('Inactive');
                        }
                    },
                    {
                        Text: 'Show All Bins',
                        Click: function () {
                            $('#lblFilterBins').text('None')
                            FilterBinTable('');
                        }
                    },
                ], { AutoDismiss: true });
            })
            //-----------old code


            $('#liCreateShelfOrBinID').click(function () {
                $('#popBinAddressTags').modal('show');
                IsBATPage = true;
                //if ($('#selFOWST').val() == '') {
                //    MessageBox("Please select a FOWST", "FOWST",
                //            [
                //                { Text: 'Ok' }
                //            ]);
                //    return;
                //} else {
                //    window.location = '/ShelfAndBinLabeling/AddShelfAndBinLabel';
                //}

            });

            //$('#tblBins').find('#thSelectAll').on('dblclick',function () {

            //we used this method because when the dataTable.js loads new data in the Table, event functions will not trigger

            $('body').on('click', '#thSelectAll', function () {
                if (IsSelectAll) {
                    IsSelectAll = false;
                    $('#thSelectAll').html('<a data-role="none" class="link-primary">Select All</a>');
                }
                else {
                    IsSelectAll = true;
                    $('#thSelectAll').html('<a data-role="none" class="link-primary">Deselect All</a>');
                }

                $('#tblBins tbody tr td input').each(function () {
                    $(this).prop('checked', IsSelectAll);
                })
            }).on('click', '#tblBins tbody tr td', function () {

                var mData = [];
                var AreaName = '';
                
                if (binDatatable.data().count() > 0) {
                    if (binDatatable.cell(this).index().columnVisible == 0) {
                        return;
                    }

                    BinID = binDatatable.row(this).data().ID;
                    BinName = binDatatable.row(this).data().Name;
                    AreaName = binDatatable.row(this).data().Area;

                    mData.push({
                        Text: 'Deactivate Bin Address',
                        Click: function () {
                            GetBinProducts();
                            $('.lblBinLabelHeader').text('Bin ' + BinName);
                            $('#popDeactivateBin').modal('show');
                        }
                    });

                    mData.push({
                        Text: 'Deactivate Bins ' + AreaName + ' Area',
                        Click: function () {
                            DeactivateAllBinsInArea(AreaName);                            
                            $('.lblBinLabelHeader').text('Area ' + AreaName);                            
                        }
                    });

                    mData.push({
                        Text: 'See Products Mapped to this Bin',
                        Click: function () {
                            getAllProductsInBin();
                            $('#popProductBinMapping').modal('show');
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
                $('#lblStartAtRackNo').css('display', 'none');
                $('#txtStartAtRackNo').css('display', 'none');
                $('#lblStartAtSectionNo').css('display', 'none');
                $('#txtStartAtSectionNo').css('display', 'none');

                //var element = document.getElementById("dvAislesOrRack2");
                //element.classList.remove('col-md-3')

                //element = document.getElementById("dvSection");
                //element.classList.remove('col-md-3')

                //element = document.getElementById("dvAislesOrRack2");
                //element.classList.add('col-md-9')

                //element = document.getElementById("dvSection");
                //element.classList.add('col-md-9')
            });

            $('#chkRack').click(function () {
                var element = document.getElementById("dvAislesOrRack2");
                element.classList.remove('col-md-9')

                //element = document.getElementById("dvSection");
                //element.classList.remove('col-md-9')

                //element = document.getElementById("dvAislesOrRack2");
                //element.classList.add('col-md-3')

                //element = document.getElementById("dvSection");
                //element.classList.add('col-md-3')

                $('#lblAislesOrRack1').text('rack');
                $('#lblAislesOrRack2').text('rack');

                $('#lblStartAtRackNo').css('display', 'flex');
                $('#txtStartAtRackNo').css('display', 'flex');
                $('#lblStartAtSectionNo').css('display', 'flex');
                $('#txtStartAtSectionNo').css('display', 'flex');               

            });

            $('#btnAddProduct').click(function () {
                //$('#popProductBinMapping').modal('show');
                GetProducts();
            });
            
            $('#btnDeleteProduct').click(function () {
                DeleteProducts();
            });            

            $('#btnDeactivateBin').click(function () {                
                DeactivateBinAndProducts(false);
            })

            $('#btnCancelDeactivationBin').click(function () {
                $('#popDeactivateBin').modal('hide');
            })

            $('#lnkBATExample').click(function () {
                $('#popAutoGenerateBinTags').modal('hide');
                $('#popBATExample').modal('show');
                $('#popBATExample').unbind('hidden.bs.modal').on('hidden.bs.modal', function () {
                    $('#popAutoGenerateBinTags').modal('show');
                    $('#popBATExample').unbind('hidden.bs.modal');
                })
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

            //$('#selFOWST').change(function () {
            //    sessionStorage.FowstID = $('#selFOWST').val();
            //});

            //$('#btnInventoryBack').click(function (e) {
            //    e.preventDefault();

            //    window.location = '/People/PeopleMenu';

            $('#btnGo').click(function () {
                var txtRackOrAisles = $('#txtRackOrAisles').val();
                var txtSection = $('#txtSection').val();
                var txtArea = $('#txtArea');

                if (txtArea.val() == '') {
                    NewMessageBox('Please select area name');
                    return
                }

                if (txtRackOrAisles == 0) {
                    NewMessageBox('Please enter number of ' + $('#lblAislesOrRack2').text());
                    return
                }

                if (txtSection == 0) {
                    NewMessageBox('Please enter number of sections per rack');
                    return
                }                

                DisplayInputShelves(txtArea.find('option:selected').text(), txtRackOrAisles, txtSection);
            })
        });
    });

    function DisplayInputShelves(areaname, noofracks, noofsections) {
        var tbodyAutoGenerateBinTags = $('#tbodyAutoGenerateBinTags'), rownum = 1;
        tbodyAutoGenerateBinTags.empty();

        for (var r = 0; r < noofracks; r++) {
            for (var s = 0; s < noofsections; s++) {
                tbodyAutoGenerateBinTags.append($('<tr>')
                    .append($('<td>').text(rownum + '.'))
                    .append($('<td>').text(sessionStorage.FowstName))
                    .append($('<td>').text(areaname))
                    .append($('<td class="text-center">').text((r + 1)))
                    .append($('<td class="text-center">').text((s + 1)))
                    .append($('<td>').append($('<input data-role="none" type="number" class="form-control">')
                            .attr({ rack: (r + 1), section: (s + 1) })
                            .keyup(function () {
                                var dis = $(this), rack = dis.attr('rack'), section = dis.attr('section');
                                var tag = rack + '.' + section + '.';
                                dis.parent().next().text(tag + 1 + ' to ' + tag + dis.val());
                            })
                        )
                    )
                    .append($('<td>'))
                )

                rownum += 1;
            }
        }        
    }

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
                $('#popDeactivateBin').modal('hide')
            }
        });
    }

    function GetProducts() {
        BrowsePopup('LibraryPIMProduct', 'Products',
                    [
                        {
                            AutoSearch: true,
                            HidePrevPopup: false,
                            DisplayText: [{ Column: 'ProductName' }],
                            Click: function (e) {
                                var data = {
                                    ProductID: e.ID,
                                    FowstID: sessionStorage.FowstID,
                                    BinID: BinID
                                }
                                AjaxSend({
                                    url: 'FowstBin/SaveBinStock',
                                    data: data,
                                    success: function (vData) {
                                        //getAllProductsInBin();
                                        GetAllFowstBinProductsForShelfLabels();
                                    }
                                });
                            }, Close: function () {
                                //$('#popProductBinMapping').modal('show');
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
                            HidePrevPopup: false,
                            DisplayText: [{ Column: 'FowstName' }],
                            Click: function (e) {
                                sessionStorage.FowstID = e.ID;
                                sessionStorage.FowstName = e.FowstName;
                                $('#lblWorkFowstName').text(sessionStorage.FowstName);
                                $('#lblFOWST').text(sessionStorage.FowstName);
                                $('.btnFOWST').text(sessionStorage.FowstName)
                                $('#lblBATFOWST').text(sessionStorage.FowstName)
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
        
        var data = {
        //    FowstID: sessionStorage.FowstID,
            //    ProductID: ProductID,
            AreaID: AreaID,
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
                'className': 'dt-body-center text-center',
                'render': function (data, type, full, meta) {
                    return '<input type="checkbox" data-role="none" class="form-check-input">';
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
            "searchable": true,
            "paging": false,
            "ordering": false,
            //"lengthMenu": [[25, 50, -1], [25, 50, "All"]],
            "language": {
                "search": "Search Bin "
            },
            "initComplete": function(){
                $('#tblBins').removeClass('dataTable');
            },
            //"info": false, //search entries
            //"scrollY": '50vh',            
            "sDom": '<"top">rt<"bottom"ip><"clear">' //hide search label and field
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
                'className': 'dt-body-center text-center',
                'render': function (data, type, full, meta) {
                    return '<button data-role="none" class="btn btn-danger"><span class="bi-trash"></button>';
                }
            }],            
            "language": {
                "search": "Search Product within the list "
            },
            "initComplete": function () {
                $('#tblBinProducts').removeClass('dataTable');
            },
            "sDom": '<"top">rt<"bottom"ip><"clear">'
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
            //"scrollY": '50vh',
            //"scrollY": "200px",
            //"scrollCollapse": true,
            "paging": false,
            "initComplete": function () {
                $('#tblFowstBinProduct').removeClass('dataTable');
            },
            "sDom": '<"top">rt<"bottom"ip><"clear">' //hide search label and field
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
                $('#popProductBinMapping').modal('show');
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
        var BinLongNames = [];
        var BinNamesCounter = 0;

        var mMax;
        var mValue;

        var Rackstartvalue = 1;
        var Sectionstartvalue = 1;

        if (CInt2($('#txtStartAtRackNo').val()) > 0) {
            Rackstartvalue = CInt2($('#txtStartAtRackNo').val())
        }

        if (CInt2($('#txtStartAtSectionNo').val()) > 0) {
            Sectionstartvalue = CInt2($('#txtStartAtSectionNo').val())
        }

        for (var a = Rackstartvalue; a < parseInt($('#txtRackOrAisles').val()) + Rackstartvalue; a++) {
                if ($('#chkRack').is(':checked') || $('#chkAisle').is(':checked')) {

                    if ($('#chkAisle').is(':checked')) {
                        mMax = 2;
                    } else {
                        //if (a == Rackstartvalue) {
                        //    Rackstartvalue = BinNamesCounter;
                        //}
                        //mMax = parseInt($('#txtRackOrAisles').val());
                        mMax = 1;
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
                        for (var c = Sectionstartvalue; c < parseInt($('#txtSection').val()) + Sectionstartvalue; c++) {
                                //if ($('#chkShelf').is(':checked')) {
                                for (var d = 1; d <= parseInt($('#txtShelf').val()) ; d++) {
                                    var BASE = "0000";

                                    if ($('#chkAisle').is(':checked')) {
                                        BinNames[BinNamesCounter] = a + '.' + mValue + '.' + c + '.' + d;
                                        BinLongNames[BinNamesCounter] = ('0000' + a).slice(-4) + '.' + mValue + '.' + ('0000' + c).slice(-4) + '.' + ('0000' + d).slice(-4);
                                    } else {
                                        BinNames[BinNamesCounter] = a + '.' + c + '.' + d;
                                        BinLongNames[BinNamesCounter] = ('0000' + a).slice(-4) + '.' + ('0000' + c).slice(-4) + '.' + ('0000' + d).slice(-4);
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
            BinLongNames: BinLongNames
        };

        AjaxSend({
            url: 'FowstBin/SaveBinBatch',
            data: data,
            success: function (vData) {

                for (var i = 0; i <= vData.BinName.length - 1; i++) {
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
   
    
    function GetAllFowstArea() {

        if (sessionStorage.FowstID == undefined) {
            DisplayFowst();
        }

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
                $('#lblArea').text('All');
                $('#lblBATArea').text('All');
                //if (PriceLevelID == undefined) {
                //    GetAllPriceLevels();
                //} else {
                //    GetAllFowstBinProducts();
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
                    $('#lblArea').text(AreaName);
                    $('#lblBATArea').text(AreaName);
                    sessionStorage.AreaName = AreaName;
                    if (IsBATPage) {
                        GetNamedFowstBin();
                    }
                }
            })
        }

        PopupButtonList('', 'FOWST Area',
                    mData, { AutoDismiss: true })
    }

    

    function GetListOfFOWSTBIN() {
        var data = {
            FowstID: sessionStorage.FowstID,
            AreaID: AreaID
        }

        if (OfflineMode) {
            var mData = '';
            mData = InventoryCount.LibraryFowstBin(data);
            DisplayFowstBinProduct(mData.Rows);
        }
        else {

            BrowsePopup('LibraryFowstBin', 'Bins', [
                {
                    DisplayText: [{ Column: 'Name' }],
                    HidePrevPopup: false,
                    Barcode: true,
                    Click: function (e) {
                        BinID = e.ID;
                        BinName = e.Name
                        $('#lblBinAddress').text(BinName);
                        GetAllFowstBinProductsForShelfLabels();
                    }
                }
            ], {
                AreaID: AreaID,
                FowstID: sessionStorage.FowstID
            })

            //AjaxSend({
            //    url: 'Browser/LibraryFowstBin',
            //    data: data,
            //    success: function (vData) {
            //        DisplayFowstBinProduct(vData.Results.Rows);
            //    }
            //});
        }
    }

    function DisplayFowstBinProduct(vRows) {
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
                    GetAllFowstBinProductsForShelfLabels();
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
                    $('#lblBinAddress').text(BinName);
                    GetAllFowstBinProductsForShelfLabels();
                }
            })
        }

        PopupButtonList('', 'Bin Address',
                    mData, { AutoDismiss: true })
    }


    function GetAllFowstBinProductsForShelfLabels() { 
        var pData = {};
        
        pData.FowstID = sessionStorage.FowstID;
        pData.BinID = BinID;
        pData.OrgID = sessionStorage.OrgID;
        //pData.PriceLevelID = CStr2(PriceLevelID);
        pData.AreaID = AreaID;
        //pData.FilterDisplay = FilterDisplay;

        AjaxSend({
            url: 'ShelfAndBinLabeling/GetData',
            data: pData,
            success: function (vData) {
                DisplayFowstBinProducts(vData.Results.Rows)
            }
        })

    }

    var FowstBinProductShelfLabelsTable = $('#tblShelfPrices');
    //var fowstbinproductDatatable;

    function DisplayFowstBinProducts(vData) {

        if ($.fn.DataTable.isDataTable(FowstBinProductShelfLabelsTable)) {
            //empty the table first before destroying datatabase para dili magdoble ang header
            FowstBinProductShelfLabelsTable.empty();
            fowstbinproductDatatable.destroy();
        }

        fowstbinproductDatatable = FowstBinProductShelfLabelsTable.DataTable({
            "data": vData,
            "columns": [
                { "": "", "width": "20px" },
                { "data": "Bin" },
                { "data": "ProductName" },
                { "data": "UnitMeasure" }
                //{ "data": "Price" },
                //{ "data": "LastUpdated" }
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
                $('#tblShelfPrices').removeClass('dataTable');
            },
            "paging": false,
            "sDom": '<"top">rt<"bottom"ip><"clear">'
        });
    }

    function DeleteProducts() {       
        var Products = [];
        var productCounter = 0;

        $('#tblShelfPrices tbody tr td input').each(function () {
            if ($(this).prop('checked') == true) {
                Products.push({
                    ID: fowstbinproductDatatable.row($(this).closest('tr')).data().ID,
                    BinID: BinID
                });

                productCounter++;
            }
        });
        
        if (productCounter > 0) {
            NewMessageBox("Are you sure you want to delete the selected products from its assigned bin?", "Delete Products",
                       [
                           {
                               Text: 'Yes, remove selected product',
                               Click: function () {
                                   DeleteProductsFromShelf(Products);
                               }
                           },
                           { Text: 'No, Cancel this Action ' }
                       ]);
        }       
    }

    function DeleteProductsFromShelf(Products) {
        var pData = {
            Products: Products,
            FOWSTID: sessionStorage.FowstID
        }

        AjaxSend({
            url: 'ShelfAndBinLabeling/DeleteProductsFromShelf',
            data: pData,
            success: function (vData) {
                GetAllFowstBinProductsForShelfLabels();
            }
        })
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