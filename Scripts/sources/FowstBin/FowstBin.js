require([
    'common'
//    GetPathFormat('/Scripts3/Browser/LibraryFowstBin.js'),
//    GetPathFormat('/Scripts3/Browser/LibraryFowst.js')
    ], function () {
        var ID = '';
        var IsEdit = 0;
        var IsCustomerBin = 0;
        var FowstID = '';
        var ProductID = '';

        var InventoryCount = {};

        InventoryCount = new App.model.InventoryCount_Offline();

        $(function () {
            if (window.location.toString().indexOf('#') < 0) {
                GetFowst();
            }
            else if (window.location.toString().indexOf('AssignBinToStock') >= 0) {
                GetAllBin();
            }
            else {
                IsCustomerBin = sessionStorage.IsCustomerBin;
                FowstID = sessionStorage.FowstID;
                ProductID = sessionStorage.ProductID;
            }
            $('#btnNewFowstBin').click(function () {
                ClearFields();
            });

            $('#btnSaveFowstBin').click(function () {
                if ($('#txtFowstBinName').val() == '') {
                    MessageBox("Please put name to Bin",
                                "Bin",
                                [
                                    { Text: 'Ok' }
                                ]
                            );
                    return;
                }
                var data = {
                    ID: ID,
                    IsEdit: IsEdit,
                    FowstID: FowstID,
                    Name: $('#txtFowstBinName').val(),
                    Description: $('#txtFowstBinDescription').val()
                };
                SaveBin(data);
            });

            $('#btnSaveFowstBin2').click(function () {
                if ($('#txtFowstBinName2').val() == '') {
                    MessageBox("Please put name to Bin",
                                "Bin",
                                [
                                    { Text: 'Ok' }
                                ]
                            );
                    return;
                }

                if (ID == '') {
                    ID = NewID();
                }

                var data = {
                    ID: ID,
                    IsCustomerBin: IsCustomerBin,
                    IsEdit: IsEdit,
                    FowstID: FowstID,
                    Name: $('#txtFowstBinName2').val(),
                    Description: $('#txtFowstBinDescription2').val(),
                    ProductID: ProductID
                };


                SaveBin(data);                

               
            })
        });

        function SaveBin(data) {

            if (OfflineMode) {

                var mResult = InventoryCount.SaveBin(data);
                if (mResult) {

                    var pSql = 'Insert into LibraryFowstBinProduct(ID, FOWSTID, BINID, ProductID) values(' +
                           ReadyString(NewID()) + ',' +
                           ReadyString(data.FowstID) + ',' +
                           ReadyString(data.ID) + ',' +
                           ReadyString(data.ProductID) + ')'

                    Sql.ExecuteSql(pSql);

                    $.mobile.changePage(window.pageSourcePage);
                }
            }
            else {
                AjaxSend({
                    url: 'FowstBin/SaveBin',
                    data: data,
                    success: function (vData) {
                        if (vData.Save == "True") {
                            if (window.pageSourcePage != undefined) {
                                $.mobile.changePage(window.pageSourcePage);
                            }
                            else {
                                ClearFields();
                                ID = '';
                                IsEdit = 0;
                                GotoFirstPage();
                                GetOrganizationID();
                            }
                        }
                    }
                });
            }
        }
        function SaveBinStock(data) {
            AjaxSend({
                url: 'FowstBin/SaveBinStock',
                data: data,
                success: function (vData) {
                    if (vData.Save == "True") {
                        if (window.pageSourcePage != undefined) {
                            $.mobile.changePage(window.pageSourcePage);
                        }
                        else {
                            ClearFields();
                            ID = '';
                            IsEdit = 0;
                            GotoFirstPage();
                            GetOrganizationID();
                        }
                    }
                }
            });
        }
        //        function GetOrganizationID() {
        //            AjaxSend({
        //                url: 'FowstBin/GetOrganizationID',
        //                success: function (vData) {
        //                    GetFowst(vData.OrganizationID);
        //                }
        //            });
        //        }

        function GetFowst(OrgID) {
            Browser.LibraryFowst.Click = function (e) {
                FowstID = e.ID;
                FowstName = e.Name;
                GetAllBin();
            };

            Browser.LibraryFowst.Show();
        }

        function GetAllBin() {
            var data = { FowstID: FowstID }
            AjaxSend({
                url: 'Browser/LibraryFowstBin',
                data: data,
                success: function (vData) {
                    DisplayBin(vData.Results.Rows);
                    DisplayBinStockToBin(vData.Results.Rows);
                }
            });
        }

        function DisplayBin(vRows) {
            var pLV = $("#lstFowstBinList");

            pLV.empty();

            for (var i = 0; i <= vRows.length - 1; i++) {
                //unit measure
                pItem = $('<li>')
                    .append($('<a>')
                        .attr('ID', vRows[i].ID)
                        .data('data', vRows[i])
                        .click(function (e) {
                            IsEdit = 1;
                            ID = $(this).data('data').ID;
                            $('#txtFowstBinName').val($(this).data('data').Name)
                            $('#txtFowstBinDescription').val($(this).data('data').Description)
                            $.mobile.changePage('#pageMain')
                        })
                        .append($('<h3>')
                            .text(vRows[i].Name)
                        )
                        .append($('<p>')
                            .text(vRows[i].Description)
                        )
                    )

                pLV.append(pItem)
            }
            pLV.listview('refresh');
        }

        function DisplayBinStockToBin(vRows) {
            var pLV = $("#lstFowstBinStockList");

            pLV.empty();

            for (var i = 0; i <= vRows.length - 1; i++) {
                //unit measure
                pItem = $('<li>')
                    .append($('<a>')
                        .attr('ID', vRows[i].ID)
                        .data('data', vRows[i])
                        .click(function (e) {
                            var data = { BinID: $(this).data('data').ID }
                            SaveBinStock(data);

                        })
                        .append($('<h3>')
                            .text(vRows[i].Name)
                        )
                        .append($('<p>')
                            .text(vRows[i].Description)
                        )
                    )

                pLV.append(pItem)
            }
            pLV.listview('refresh');
        }

        function ClearFields() {
            $('#txtFowstBinName').val('');
            $('#txtFowstBinDescription').val('');
        }
    });
    
