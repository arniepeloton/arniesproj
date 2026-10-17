require([
    'common',
     GetPathFormat('/Scripts3/Browser/LibraryPIMProduct.js?crc=')
], function () {

    require([(OfflineMode ? '/Scripts3/Offline/Browser/LibraryFowst.js?crc=' : '')], function () {
        $(document).ready(function () {

            //if (OfflineMode) {
                //Sql.AttachDatabase(sessionStorage.OrgID);
                //if (Web.HandleOrganization() == false)
                //    return;
                //var vData = OfflineBrowser.LibraryFowst.GetData({
                //    FilterFowst: 1
                //});
                //DisplayFowst(vData);
                startclock('#clock');
                $('#lnkCheckout').html('');
            //}
            //else {
               
            //    //Hide the options that will not be used
            //    $('#btnSaveAndMap').hide();
            //    $('#btnSaveAndBackToMain').hide();
            //    $('#productDiv').hide();
            //    $('#btnScanBIN').hide();
            //    GetFowst();
            //    startclock('#clock');
            //}

            $('#btnHome').click(function (e) {
                e.preventDefault();
                window.location = '/People/PeopleMenu';
            })

            $('#lblDownload').click(function () {
                PopupButtonList('', 'Paper Options', [
		        {
		            HasImage: true,
		            Text: '<img src="/Content/images/print.png" align="absmiddle" class="ui-li-thumb" style="width:50%;"/> <b>Desktop Printer</b> <br /> <div style="text-align:left !important; font-size:13px; font-weight:normal; width: 100%;"><ul><li><strong>Description: </strong>Multiple labels will be printed <br /> in one sheet using a desktop printer.</li><li><strong>Label Size: </strong>23.30mm x 88.90mm</li><li><strong>Paper Size: </strong>4.53in x 12.43in</li><li><strong>Number of labels per sheet: </strong>12</li></ul></div>',
		            Click: function () {
		                sessionStorage.setItem('paperSize', "smallerSize");
		                Download();
		            }
		        },
		        {
		            HasImage: true,
		            Text: '<img src="/Content/images/excel.png" align="absmiddle" class="ui-li-thumb" style="width:50%;"/> <b>Barcode Printer</b> <br /> <div style="text-align:left !important; font-size:13px; font-weight:normal; width:100%;"><ul><li><strong>Description: </strong>Each label will be printed <br /> using a barcode printer or a <br />special purpose printer.</li><li><strong>Label Size: </strong>38.1mm x 76.2mm</li><li><strong>Paper Size: </strong>N/A</li><li><strong>Number of labels per sheet: </strong>N/A</li></ul></div>',
		            Click: function () {
		                sessionStorage.setItem('paperSize', "largerSize");
		                Download();
		            }
		        }
		        ], { AutoDismiss: true });
            })

            $('#btnSearchProduct').click(function () {
                SearchProduct();
            })

            $('#btnScanBIN').click(function () {
                mobile.Scan({
                    Success: function (vData) {
                        SearchBin(vData);
                    }
                });
            })

            $('#btnScanProduct').click(function () {
                mobile.Scan({
                    Success: function (vData) {
                        SearchProduct(vData);
                    }
                });
            })

            $('#selFOWST').change(function () {
                if (OfflineMode) {
                    $('#selBINLocation').empty();
                    var bin = GetBINLocation($('#selFOWST').val()).Rows;

                    if ($('#selFOWST').val() != "") {
                        $('#selBINLocation').selectmenu('enable');
                        if (bin.length > 0) {
                            $('#selBINLocation').append(new Option('Select', '', false));
                            for (var i = 0; i < bin.length; i++) {
                                $('#selBINLocation').append(new Option(bin[i].Name, bin[i].ID, false));
                            }
                        }
                        else {
                            $('#selBINLocation').append(new Option('No Result(s) Found', '', false));
                        }
                    }
                    else {
                        $('#selBINLocation').empty().selectmenu('disable');
                    }
                    $('#selBINLocation').selectmenu('refresh');
                }
                else {
                    GetBINLocationOnline();
                }
            })

            $('#selBINLocation').change(function () {
                if ($('#selFOWST').val() != "" && $(this).val() != "") {
                    DisplayProductList();
                }
            })

            $('#btnSaveAndMap').click(function () {
                if ($('#selFOWST').val() == "") {
                    MessageBox('Please Select FOWST');
                    return false;
                }

                if ($('#selBINLocation').val() == "") {
                    MessageBox('Please Select BIN');
                    return false;
                }

                if ($('#lstProductsInBIN li').length == 0) {
                    MessageBox('Please select at least one product');
                    return false;
                }

                SaveBINProducts();
                $('#selBINLocation').val('').selectmenu('refresh');
                $('#lstProductsInBIN').empty();
                MessageBox('Successfully Saved!');
            })

            $('#btnSaveAndBackToMain').click(function () {
                if ($('#selFOWST').val() == "") {
                    MessageBox('Please select FOWST');
                    return false;
                }

                if ($('#selBINLocation').val() == "") {
                    MessageBox('Please select Shelf BIN Location');
                    return false;
                }

                if ($('#lstProductsInBIN li').length == 0) {
                    MessageBox('Please select at least one product');
                    return false;
                }

                SaveBINProducts();

                MessageBox('Successfully Saved!', '', [{
                    Text: 'Ok',
                    Click: function () {
                        window.location = '/People/PeopleMenu';
                    }
                }]);
            })

            $('#btnCancel').click(function () {
                location = MergeUrlParameters({
                    OrgID: sessionStorage.OrgID,
                    IsMobile: CBool(IsMobile) ? 1 : 0
                }, '/ShelfAndBinLabeling');
            })
        })

        //Get the bin location online
        function GetBINLocationOnline() {
            var pData = {};
            pData.FowstID = $('#selFOWST').val();
            $('#selBINLocation').empty();
            AjaxSend({
                url: 'ShelfAndBinLabeling/GetBinLocationData',
                data: pData,
                success: function (vData) {
                    $('#selBINLocation').selectmenu('enable');
                    if (vData.Results.Rows.length > 0) {
                        $('#selBINLocation').append(new Option('Select', '', false));
                        for (var i = 0; i < vData.Results.Rows.length; i++) {
                            $('#selBINLocation').append(new Option(vData.Results.Rows[i].Name, vData.Results.Rows[i].ID, false));
                        }
                    }
                    else {
                        $('#selBINLocation').append(new Option('No Result(s) Found', '', false));
                    }
                    $('#selBINLocation').selectmenu('refresh');
                }
            })
        }

        //Getting the bin location when using a device
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

        function DisplayProductList() {
            $('#lstProductsInBIN').empty();
            if (OfflineMode) {
                var list = GetBinListOffline().Rows;

                if (list.length > 0) {
                    for (var i = 0; i < list.length; i++) {
                        $('#lstProductsInBIN').append($('<li>')
                                                            .attr('id', list[i].ID)
                                                            .attr('pimid', list[i].ProductID)
                                                            .append($('<a>')
                                                                .text(list[i].ProductName)
                                                            )
                                                            .append($('<a>')
                                                                .attr('pimid', list[i].ProductID)
                                                                .attr('data-icon', 'delete')
                                                                .click(function (e) {
                                                                    $('#lstProductsInBIN li[pimid="' + $(this).attr('pimid') + '"]').attr('state', 'delete').hide();
                                                                    $('#lstProductsInBIN').listview('refresh');
                                                                })
                                                            )
                                                            .attr('state', 'add')
                            ).listview('refresh');
                    }
                }
            }
            else {
                GetBinListOnline();
            }
        }

        //Getting the bin list online
        function GetBinListOnline() {
            var pData = {};
            pData.FowstID = $('#selFOWST').val();
            pData.BinID = $('#selBINLocation').val();

            AjaxSend({
                url: 'ShelfAndBinLabeling/GetShelfPriceTagsData',
                data: pData,
                success: function (vData) {
                    if (vData.Results.Rows.length > 0) {
                        for (var i = 0; i < vData.Results.Rows.length; i++) {
                            $('#lstProductsInBIN').append($('<li>')
                                                                .attr('id', vData.Results.Rows[i].ID)
                                                                .attr('pimid', vData.Results.Rows[i].ProductID)
                                                                .append($('<a>')
                                                                    .text(vData.Results.Rows[i].ProductName)
                                                                )
                                                                .attr('state', 'add')
                                ).listview('refresh');
                        }
                    }
                }
            })
        }

        //Get the bin list when using a device
        function GetBinListOffline() {
            var pSql = '';
            var mData;

            pSql = 'SELECT LibraryFowstBinProduct.ID, LibraryFowstBinProduct.ProductID, LibraryPIMProduct.ProductName FROM LibraryFowstBinProduct INNER JOIN LibraryPIMProduct ON LibraryFowstBinProduct.ProductID = LibraryPIMProduct.ID ' +
                    'Where FowstID = ' + ReadyString($('#selFOWST').val()) +
                    ' AND BinID = ' + ReadyString($('#selBINLocation').val()) +
                    ' AND LibraryFowstBinProduct.IsActive = "1"'

            mData = Sql.Execute(pSql);
            return mData;
        }

        //For downloading the shelf price tags
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

        //For searching bins via barcode
        function SearchBin(vData) {
            $('#selBINLocation option').each(function () {
                if (this.value == vData.Value) {
                    $('#selBINLocation').val(this.value);
                    $('#selBINLocation').selectmenu("refresh", true);
                    DisplayProductList();
                } else {
                    MessageBox('Bin Address does not exist!');
                }
            });
        }

        //For searching product via barcode
        function SearchProduct(vData) {
            Browser.LibraryPIMProduct.Click = function (e) {
                var isExist = false;

                $('#lstProductsInBIN li').each(function () {
                    if ($(this).attr('pimid') == e.ID) {
                        isExist = true;
                    }
                })

                if (isExist) {
                    alert('"' + e.ProductName + '" was already exist on the list');
                    return false;
                }

                $('#lstProductsInBIN').append($('<li>')
                                                    .attr('pimid', e.ID)
                                                    .append($('<a>')
                                                        .text(e.ProductName)
                                                    )
                                                    .append($('<a>')
                                                        .attr('pimid', e.ID)
                                                        .attr('data-icon', 'delete')
                                                        .click(function (e) {
                                                            console.log('hide');
                                                            $('#lstProductsInBIN li[pimid="' + $(this).attr('pimid') + '"]').attr('state', 'delete').hide();
                                                            $('#lstProductsInBIN').listview('refresh');
                                                        })
                                                    )
                                                    .attr('state', 'add')
                    ).listview('refresh');
            }

            if (vData != undefined) {
                Browser.LibraryPIMProduct.Show(vData.Value);
            }
            else {
                Browser.LibraryPIMProduct.Show();
            }
        }

        function SaveBINProducts() {
            $('#lstProductsInBIN li').each(function () {
                var state = $(this).attr('state');
                var id = $(this).attr('id');
                var productid = $(this).attr('pimid');
                var fowstid = $('#selFOWST').val();
                var binid = $('#selBINLocation').val();
                var pFinder = new Finder();
                var pFields = pFinder.Fields;
                var pField;
                var pSql = '';
                var mData;
                var pSql2 = '';
                var mData2;
                var pIU;

                pSql = 'Select ID from private.LibraryFowstBinProduct ' +
                    'Where FowstID = ' + ReadyString(fowstid) +
                    ' AND BinID = ' + ReadyString(binid) +
                    ' AND ProductID = ' + ReadyString(productid)

                mData = Sql.Execute(pSql);

                //Check if Product already exist in the BIN Location
                pSql2 = 'Select ID from private.LibraryFowstBinProduct ' +
                    'Where FowstID = ' + ReadyString(fowstid) +
                    ' AND BinID = ' + ReadyString(binid) +
                    ' AND IsDefault = "1" ' +
                    ' AND ProductID = ' + ReadyString(productid)

                mData2 = Sql.Execute(pSql2);

                pIU = new InsertUpdate('private.LibraryFowstBinProduct');

                pIU.Add('MaxPerProduct', 1);
                pIU.Add('MinPerProduct', 0);

                pIU.Add('FowstID', ReadyString(fowstid));
                pIU.Add('BinID', ReadyString(binid));
                pIU.Add('ProductID', ReadyString(productid));
                pIU.Add('LastUpdated', proj.SqliteNow);

                if (mData.Rows.length == 0) {
                    if (mData.Rows.length == 0) {
                        pIU.Add('IsDefault', 1);
                    }
                    else {
                        pIU.Add('IsDefault', 0);
                    }

                    if (state == "add") {
                        pIU.Add('IsActive', 1);
                        pIU.Add('ID', ReadyString(NewID()));
                        pSql = pIU.GetInsert();
                        Sql.ExecuteSql(pSql);
                    }
                }
                else {
                    if (state == "delete") {
                        pIU.Add('IsActive', 0);
                        pIU.Add('DateImported', 'NULL');
                        pSql = pIU.GetUpdate('ID = ' + ReadyString(id));
                        //pSql = "UPDATE private.LibraryFowstBinProduct SET IsActive = 0, DateImported = NULL WHERE ID = " + ReadyString(id);
                    }
                    else {
                        pIU.Add('IsActive', 1);
                        pIU.Add('DateImported', 'NULL');
                        pSql = pIU.GetUpdate('ID = ' + ReadyString(mData.Rows[0].ID));
                    }

                    Sql.ExecuteSql(pSql);
                }

                //return true;
            })
        }

        //For getting fowst online
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

        //For getting fowst when using a device
        function DisplayFowst(vData) {
            $('#selFOWST').empty();
            $('#selFOWST').append(new Option('Please Select', '', false));

            for (var i = 0; i <= vData.Rows.length - 1; i++) {
                $('#selFOWST').append(new Option(vData.Rows[i].FowstName, vData.Rows[i].ID, false));
            }

            try {
                $('#selFOWST').selectmenu('refresh');
            }
            catch (e) {
                $('#selFOWST').selectmenu();
            }
        }

    });

})