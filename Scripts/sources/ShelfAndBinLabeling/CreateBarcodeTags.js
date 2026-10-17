require([
    'common',
     GetPathFormat('/Scripts3/Browser/LibraryPIMProduct.js?crc='),
     GetPathFormat('/Scripts3/Browser/LibraryFowstProductBarcode.js?crc=')
], function () {

    require([(OfflineMode ? '/Scripts3/Offline/Browser/LibraryFowst.js?crc=' : ''), '/Scripts/jquery.dataTables.min.js'], function () {
        $(document).ready(function () {
            var ctrSelectedProduct = 0;
            var transNum = undefined;

            //GetData();
            startclock('#clock');
            localStorage.setItem('barcodeTransaction', null);
            localStorage.setItem('noprintList', null);
            sessionStorage.setItem("Lookup", "false");
            sessionStorage.removeItem("changed");

            $('#btnHome').click(function (e) {
                e.preventDefault();
                window.location = '/People/PeopleMenu';
            })

            $('#btnAddBarcode').click(function (e) {
                e.preventDefault();                

                $('.newitemqty').each(function () {
                    var charCount = $(this).val().length;
                    if (charCount != 0) {
                        if (localStorage.getItem("barcodeTags") == "null") {
                            var tagCart = [];
                        } else {
                            var tagCart = JSON.parse(localStorage["barcodeTags"]);
                        }
                        var IsExist = "no";
                        for (var i = 0; i < tagCart.length; i++) {
                            if (tagCart[i][2] == $(this).parent().parent().attr('id')) {
                                tagCart[i][4] = $(this).val();
                                IsExist = "yes";
                            }
                        }
                        if (IsExist == "no") {
                            var tagArray = [];
                            tagArray[0] = $(this).parent().parent().parent().parent().parent().attr('id');
                            tagArray[1] = $(this).parent().parent().parent().parent().parent().attr('name');
                            tagArray[2] = $(this).parent().parent().attr('id');
                            tagArray[3] = $(this).parent().parent().attr('name');
                            tagArray[4] = $(this).val();
                            if (tagCart === null) {
                                tagCart.unshift(tagArray);
                            } else {
                                tagCart.push(tagArray);
                            }
                        }
                        localStorage["barcodeTags"] = JSON.stringify(tagCart);
                    }
                })

                if (localStorage["barcodeTags"] == "null") {
                    $('#txtMeasureRequired').show();
                    return;
                }

                $('#txtMeasureRequired').hide();

                //Check if the there is already a record of the transaction
                if (localStorage.getItem("barcodeTransaction") == "null") {
                    var mainCart = [];
                } else {
                    var mainCart = JSON.parse(localStorage["barcodeTransaction"]);
                }
                var barcodeCart = [];
                barcodeCart[0] = localStorage.getItem("ProductID");
                barcodeCart[1] = localStorage.getItem("ProductNo");
                barcodeCart[2] = localStorage.getItem("ProductName");
                barcodeCart[3] = JSON.parse(localStorage["barcodeTags"]);
                if (mainCart === null) {
                    mainCart.unshift(barcodeCart);
                } else {
                    mainCart.push(barcodeCart);
                }
                //Add all the details ot the transaction
                localStorage["barcodeTransaction"] = JSON.stringify(mainCart);

                var tbody;
                if (CBool(sessionStorage.getItem('Lookup'))) {
                    tbody = $('#tblBodyLookup')
                }
                else {
                    tbody = $('#tblBody');
                }
                
                //Populate the details to the table
                for (var i = 0; i < barcodeCart[3].length; i++) {
                    var existItem = tbody.find('tr[id="' + barcodeCart[0] + '"][barcodeid="' + barcodeCart[3][i][2] + '"]');
                    if (existItem.length == 0) {
                        tbody.append($('<tr>')
                            .attr('id', barcodeCart[0])
                            .attr('barcodeid', barcodeCart[3][i][2])
                            .append($('<td>')
                                .text(barcodeCart[1])
                            )
                            .append($('<td>')
                                .text(barcodeCart[2])
                            )
                            .append($('<td>')
                                .text(barcodeCart[3][i][1])
                            )
                            .append($('<td>')
                                .attr('class', "priceLevel")
                            )
                            .append($('<td>')
                                .attr('class', barcodeCart[3][i][2])
                                .text(barcodeCart[3][i][3])
                            )
                            .append($('<td>')
                                //.text(barcodeCart[3][i][4])
                                .append($('<input type="number" placeholder="Qty">')
                                        .attr('id', barcodeCart[3][i][2])
                                        .val(barcodeCart[3][i][4])
                                        .change(function () {
                                            var charCount = $(this).val().length;
                                            if (charCount != 0) {
                                                var storageCart = JSON.parse(localStorage["barcodeTransaction"]);
                                                for (var j = 0; j < storageCart.length; j++) {
                                                    for (var i = 0; i < storageCart[j][3].length; i++) {
                                                        if (storageCart[j][3][i][2] == $(this).attr('id')) {
                                                            storageCart[j][3][i][4] = $(this).val();
                                                        }
                                                    }
                                                }
                                                localStorage["barcodeTransaction"] = JSON.stringify(storageCart);
                                            }
                                        })
                                    )
                            )
                            .append($('<td>')
                                .append($('<a style="margin-top:8px;" class="delLabel" data-iconpos="notext" data-theme="b" data-role="button" data-icon="delete" title="Sign Out">')
                                    .text('Delete').button()
                                    .attr('barcodeid', barcodeCart[3][i][2])
                                    .attr('productid', barcodeCart[0])
                                    .data(barcodeCart[3][i])
                                    .click(function () {
                                        var data = DeleteItem($(this).attr('productid'), $(this).attr('barcodeid'));
                                        
                                        tbody.find('tr[id="' + data[0] + '"][barcodeid="' + data[1] + '"]').remove() //  remove row
                                    })
                                )
                            )
                        );
                    }
                    else {
                        var arrexistItem = JSON.parse(sessionStorage.getItem("BarcodeTags"))
                        var ind = $.map(arrexistItem, function (obj, index) {
                            if (obj.BarcodeID == barcodeCart[3][i][2] && obj.ProductID == barcodeCart[0]) {
                                return index; // get the index of existing barcode in array
                            }
                        })
                        arrexistItem[ind[0]].NumLabels = barcodeCart[3][i][4]; // update the old qty to new qty
                        sessionStorage['BarcodeTags'] = JSON.stringify(arrexistItem); // update the sessionStorage['BarcodeTags'] with the new qty
                        existItem.find('input[id="' + barcodeCart[3][i][2] + '"]').val(barcodeCart[3][i][4]); // set the new qty in textbox
                        localStorage["barcodeTags"] = $.grep(JSON.parse(localStorage["barcodeTags"]), function (e) {
                            return e[2] != barcodeCart[3][i][2]
                        })
                        if (localStorage["barcodeTags"] == "") {
                            localStorage["barcodeTags"] = "null"
                        }
                        DeleteItem(barcodeCart[0], barcodeCart[3][i][2]);
                    }
                }
                //Get the price label of the record added
                var pData = {};
                pData.ProductID = barcodeCart[0];
                AjaxSend({
                    url: 'ShelfAndBinLabeling/GetPriceLevel',
                    data: pData,
                    success: function (vData) {
                        var list = vData.Results.Rows;
                        var contents = "<option value=''>Select a Price Level</option>";
                        for (var i = 0; i < list.length; i++) {
                            contents = contents + "<option value='" + list[i].ID + "'>" + list[i].PriceLevel + "</option>"
                        }
                        tbody.find('tr').each(function () {
                            if ($(this).attr('id') == list[0].ProductID) {
                                $(this).find(".priceLevel").append($('<select>')
                                    .attr('disabled', ($('#chkPricing').is(':checked') || $('#chkPricingLookUp').is(':checked') ? false : true))
                                    .html(contents));
                            }
                        });
                    }

                });
                $('#tblBody').table().table('refresh');
                //GetData();
                $('#popupUoMBarcode').popup('close');
                ctrSelectedProduct = ctrSelectedProduct + 1;
            })

            $('#btnGenPriceLevel, #btnGenPriceLevelLookUp').click(function (e) {
                e.preventDefault();
                //if (!$('#chkPricing').is(':checked') || !$('#chkPricingLookUp').is(':checked')) {
                //    return;
                //}
                var tbody;
                if (CBool(sessionStorage.getItem('Lookup'))) {
                    tbody = $('#tblBodyLookup')
                }
                else {
                    tbody = $('#tblBody');
                }
                BrowsePopup('LibraryPIMPriceLevel', 'Search Price Level',
                    [
                        {
                            DisplayText: [
                                { Label: 'Price Level: ', Column: 'PriceLevel' }
                            ],
                            Click: function (e) {
                                tbody.find('tr').each(function () {
                                    $(this).find(".priceLevel").find("select").children().each(function () {
                                        if ($(this).text() == e.PriceLevel) {
                                            $(this).attr('selected', true);
                                        }
                                    });
                                });
                            }
                        }
                    ]
                )
                $('#popupPriceLevel').popup('open');
            })

            $('#lnkCheckout').click(function () {
                if (sessionStorage.getItem("Lookup") === 'false') {
                    if (localStorage["barcodeTransaction"] == "null") {
                        NewMessageBox('Please create at least one barcode label.', 'Message')
                        return;
                    }
                    else if (JSON.parse(localStorage["barcodeTransaction"]).length == 0) {
                        NewMessageBox('Please create at least one barcode label.', 'Message')
                        return;
                    }
                }

                PopupButtonList('', 'Printer Options', [
		        {
		            HasImage: true,
		            Text: '<img src="/Content/images/print.png" align="absmiddle" class="ui-li-thumb" style="width:50%;"/> <b>Desktop Printer</b> <br /> <div style="text-align:left !important; font-size:13px; font-weight:normal; width: 100%;"><ul><li><strong>Description: </strong>Multiple labels will be printed <br /> in one sheet using a desktop printer.</li><li><strong>Label Size: </strong>38mm x 21mm</li><li><strong>Paper Size: </strong>A4</li><li><strong>Number of labels per sheet: </strong>65</li></ul></div>',
		            Click: function () {
		                sessionStorage.setItem('paperSize', "largerSize");
		                Download();
		            }
		        },
		        {
		            HasImage: true,
		            Text: '<img src="/Content/images/excel.png" align="absmiddle" class="ui-li-thumb" style="width:50%;"/> <b>Barcode Printer</b> <br /> <div style="text-align:left !important; font-size:13px; font-weight:normal; width:100%;"><ul><li><strong>Description: </strong>Each label will be printed <br /> using a barcode printer or a <br />special purpose printer.</li><li><strong>Label Size: </strong>32mm x 20mm</li><li><strong>Paper Size: </strong>N/A</li><li><strong>Number of labels per sheet: </strong>N/A</li></ul></div>',
		            Click: function () {
		                PopupButtonList('Select barcode label format', 'Barcode Options',
                            [
                                {
                                    Text: '<table><tr><td>Label Size:</td><td style="font-weight:normal;">1in by 2in</td></tr><tr><td>Label Orientation:</td><td style="font-weight:normal;">Landscape</td></tr><tr><td>Labels per Row:</td><td style="font-weight:normal;">1 piece</td></tr><tr><td>Paper Type:</td><td style="font-weight:normal;">Roll or Fanfold</td></tr></table>',
                                    Click: function () {
                                        sessionStorage.setItem('paperSize', "smallerSize");
                                        sessionStorage.setItem('paperFormat', "1x1");
                                        Download();
                                    }
                                },
                                {
                                    Text: '<table><tr><td>Label Size:</td><td style="font-weight:normal;">3cm by 2cm</td></tr><tr><td>Label Orientation:</td><td style="font-weight:normal;">Landscape</td></tr><tr><td>Labels per Row:</td><td style="font-weight:normal;">3 pieces</td></tr><tr><td>Paper Type:</td><td style="font-weight:normal;">Roll or Fanfold</td></tr></table>',
                                    Click: function () {
                                        sessionStorage.setItem('paperSize', "smallerSize");
                                        sessionStorage.setItem('paperFormat', "1x3");
                                        Download();
                                    }
                                }
                            ]
                        )		                
		            }
		        }
		        ], { AutoDismiss: true });
            })

            $('#btnMoreOptions').click(function () {
                $('#popLookup').show();
                GetLookup()
                return;
                $('#addingContent').hide();
                $('#lookupContent').show();
                $('#tblBodyLookup').empty();
                $('#tblBody').empty();
                localStorage.setItem('barcodeTransaction', null);
                localStorage.setItem('noprintList', null);
                sessionStorage.removeItem("changed");
                Browser.LibraryFowstProductBarcode.Click = function (e) {
                    GetData(e.TransactionNum);
                    sessionStorage.setItem("Lookup", "true");
                };
                Browser.LibraryFowstProductBarcode.Show(' ');
            })

            $('#btnMainPage').click(function () {                
                $('#addingContent').show();
                $('#lookupContent').hide();
                $('#tblBody').empty();
                $('#tblBodyLookup').empty();
                $('#chkCompanyName, #chkPricing').prop('checked', false).checkboxradio('refresh');
                localStorage.setItem('barcodeTransaction', null);
                localStorage.setItem('noprintList', null);
                sessionStorage.setItem("Lookup", "false");
                sessionStorage.removeItem("changed");
            })

            $('#chkPricing').change(function () {
                if ($('#chkPricing').prop('checked')) {
                    $('.priceLevel select').attr('disabled', false);
                } else {
                    $('.priceLevel select').attr('disabled', true);
                }
            })

            $('#chkPricingLookUp').change(function () {
                if ($('#chkPricingLookUp').prop('checked')) {
                    $('.priceLevel select').attr('disabled', false);
                } else {
                    $('.priceLevel select').attr('disabled', true);
                }
            })

            $('#btnAddLabel').click(function () {
                $('#popSearchProduct').show();
                GetProductList()
            })

            $('#btnAddLabelLookUp').click(function () {
                $('#popSearchProduct').show();
                GetProductList()
            })
        })

        function DeleteItem(productid, barcodeid) {
            var productobj = $.grep(JSON.parse(localStorage["barcodeTransaction"]), function (e) {
                //return only the selected product array
                return e[0] == productid
            });

            var bcobj = $.grep(productobj[0][3], function (e) {
                //return only the selected barcode array
                return e[2] == barcodeid
            });

            var newumobj = $.grep(productobj[0][3], function (e) {
                //return all object except the deleted row
                return e[2] != bcobj[0][2];
            })

            var newbarcodeTransactionojb = JSON.parse(localStorage["barcodeTransaction"]); // copy existing  barcode trans to new variable obj
            var currentindexofselectedrow = $.map(newbarcodeTransactionojb, function (obj, index) {
                if (obj[0] == productid) {
                    return index;
                }
            })
            newbarcodeTransactionojb[currentindexofselectedrow][3] = []; // remove all obj index 3 from parent array
            newbarcodeTransactionojb[currentindexofselectedrow][3] = newumobj; // add all new um obj excluded deleted um

            

            if (newumobj.length == 0) {
                // if there's nothing left in index 3, must delete the parent array
                newbarcodeTransactionojb = $.grep(newbarcodeTransactionojb, function (e) {
                    return e[0] != productobj[0][0]
                })
            }

            localStorage["barcodeTransaction"] = JSON.stringify(newbarcodeTransactionojb); // overwrite existing barcode trans from new variable obj

            return [productobj[0][0], bcobj[0][2]]
        }

        function GetProductList() {
            var dataTableProductList = $('#dataTableProductList').DataTable({
                retrieve: true,
                "order": [[2, "asc"]],
                "bProcessing": true,
                "bServerSide": true,
                "ajax": {
                    url: "/ShelfAndBinLabeling/GetProductList?OrgID=" + sessionStorage.OrgID,
                    type: 'POST'
                },
                "autoWidth": false,
                "columns": [
                    {
                        "data": function (data, type, full, meta) {
                            return '<b class="ui-table-cell-label">#:</b>' + data.RowNum + '.'
                        },
                        "width": "200px"
                    },
                    {
                        "data": function (data, type, full, meta) {
                            return '<b class="ui-table-cell-label">Transaction #:</b>' + (data.ProductNo == null ? '' : data.ProductNo);
                        },
                        "width": "200px"
                    },
                    {
                        "data": function (data, type, full, meta) {
                            return '<b class="ui-table-cell-label">No. of Labels:</b>' + data.ProductName;
                        }
                    }
                ],
                "aoColumnDefs": [{ "bSortable": false, "aTargets": [0] }],
                "responsive": true,
                "columnDefs": [{ "targets": 0, "orderable": true }],
                "sDom": '<"top"lf>rt<"bottom"ip><"clear">'
                //"stateSave": true
            })
            $('#dataTableProductList tbody').unbind('click').on('click', 'tr', function () {                
                GetBarcode(dataTableProductList.row(this).data())
            });

            $("div.dataTables_filter input[aria-controls='dataTableProductList']").unbind().keypress(function (e) {
                if (e.keyCode == 13) {
                    dataTableProductList.search($(this).val()).draw();
                }
            });
        }

        function GetBarcode(e) {
            var pData = {};
            var isShowUMPopup = false;
            var isReadyUMPopup = false;
            pData.ProductID = e.ID;
            localStorage.setItem("ProductID", e.ID);
            localStorage.setItem("ProductNo", e.ProductNo);
            localStorage.setItem("ProductName", e.ProductName);
            localStorage.setItem('barcodeTags', null);
            //Get each measurement of the product            

            $('#popSearchProduct').hide();

            AjaxSend({
                url: 'ShelfAndBinLabeling/GetProductMeasurement',
                data: pData,
                success: function (vData) {

                    if (vData.Results.Rows.length > 0) {
                        var ctrMeasurementUM = 0; //
                        var uomData = []
                        //Populate all of the measures to a list
                        localStorage.setItem('charState', 'unchecked');
                        $('#lstUoM').empty();
                        for (var i = 0; i < vData.Results.Rows.length; i++) {
                            $('#lstUoM').append($('<li>')
                                .attr('id', vData.Results.Rows[i].ID)
                                .attr('name', vData.Results.Rows[i].Label)
                                .text(vData.Results.Rows[i].Label)
                                .append($('<table>')
                                    .attr('id', vData.Results.Rows[i].ID + '-table')
                                )
                            );
                            $('#lstUoM').listview('refresh');
                            $('#' + vData.Results.Rows[i].ID + '-table').table().table('refresh');
                            uomData.push({ ID: vData.Results.Rows[i].ID })
                        }

                        AjaxSend({
                            url: 'ShelfAndBinLabeling/GetMeasurementBarcode',
                            data: { uomData: uomData },
                            success: function (vData2) {
                                var barcodes = vData2.Results;

                                if (barcodes.length > 0) {
                                    for (var i = 0 ; i < barcodes.length; i++) {
                                        for (var j = 0 ; j < barcodes[i].length; j++) {
                                            $('#' + barcodes[i][j].StockMeasurementsID + '-table').append($('<tr>')
                                                .attr('id', barcodes[i][j].ID)
                                                .attr('name', barcodes[i][j].Barcode)
                                                .append($('<td>')
                                                    .text(barcodes[i][j].Barcode)
                                                )
                                                .append($('<td>')
                                                    .append($('<input type="number" placeholder="Qty">')
                                                        .addClass('newitemqty')
                                            //Bind an action that updates the quantity of the labels realtime
                                            //It should be able to update the storage data
                                                        .focusout(function () {
                                                            
                                                        })
                                                    )
                                                )
                                            );

                                            isShowUMPopup = true; //Showing the popup
                                        }
                                    }
                                }

                                isReadyUMPopup = true;
                            }
                        });
                    }
                    else {
                        isReadyUMPopup = true;
                    }
                },
                complete: function () {
                    NewMessageBox('Getting the barcode ... please wait');
                    var umpopup = setInterval(function () {
                        if (isReadyUMPopup) {
                            NewMessageBoxClose();
                            if (isShowUMPopup) {
                                $('#popupUoMBarcode').popup('open');
                            }
                            else {
                                NewMessageBox('No barcode was found. Do you want to search another product?', '',
                                        [
                                            {
                                                Text: 'Yes',
                                                Click: function () {
                                                    $('#btnAddLabel').click();
                                                }
                                            }, { Text: 'No' }
                                        ]
                                    )
                            }
                            isReadyUMPopup = false;
                            clearInterval(umpopup);
                        }
                    }, 500);
                }
            });
        }

        function GetLookup() {
            var datatable = $('#dataTableBarcodeList').DataTable({
                retrieve: true,
                "order": [[0, "desc"]],
                "bProcessing": true,
                "bServerSide": true,
                "ajax": {
                    url: "/ShelfAndBinLabeling/Lookup?OrgID=" + sessionStorage.OrgID,
                    type: 'POST'
                },
                "autoWidth": false,
                "columns": [
                    {
                        "data": function (data, type, full, meta) {
                            return '<b class="ui-table-cell-label">Transaction #:</b>' + data.TransactionNum;
                        },
                        "width": "200px"
                    },
                    {
                        "data": function (data, type, full, meta) {
                            return '<b class="ui-table-cell-label">No. of Labels:</b>' + data.NumLabels;
                        }
                    },
                    {
                        "data": function (data, type, full, meta) {
                            var LastUpdated = data.LastUpdated;
                            return '<b class="ui-table-cell-label">Date Modified</b>' + $.format.date(new Date(LastUpdated), 'MMM dd, yyyy hh:mm a')
                        }
                    }
                ],
                "aoColumnDefs": [{ "bSortable": false, "aTargets": [1] }],
                "responsive": true,
                "columnDefs": [{ "targets": 0, "orderable": true }],
                "sDom": '<"top"lf>rt<"bottom"ip><"clear">'
                //"stateSave": true
            })

            $('#dataTableBarcodeList tbody').on('click', 'tr', function () {
                $('#popLookup').hide();
                sessionStorage.setItem("TransID", datatable.row(this).data().TransactionID);
                GetData(datatable.row(this).data().TransactionNum);
                sessionStorage.setItem("Lookup", "true");
            });

            $("div.dataTables_filter input[aria-controls='dataTableBarcodeList']").unbind().keypress(function (e) {
                if (e.keyCode == 13) {
                    datatable.search($(this).val()).draw();
                }
            });
        }

        function Download() {           

            if (sessionStorage.getItem("Lookup") === 'false') {
                //Convert the storage data to a format that the controller requires
                transNum = undefined;
                var cartData = [];
                var emptyPrices = "false";
                var storageCart = JSON.parse(localStorage["barcodeTransaction"]);
                for (var i = 0; i < storageCart.length; i++) {
                    for (var j = 0; j < storageCart[i][3].length; j++) {
                        var tempData = {};
                        tempData.ID = NewID();
                        tempData.SkuDesc = storageCart[i][0];
                        tempData.UoM = storageCart[i][3][j][0];
                        tempData.NumLabels = storageCart[i][3][j][4];
                        tempData.Barcode = storageCart[i][3][j][2];
                        tempData.Price = "";
                        tempData.IsLookUp = CBool(sessionStorage.getItem("Lookup"));
                        tempData.TransactionID = "";
                        $("#tblBody tr").each(function () {
                            if ($(this).find("." + storageCart[i][3][j][2]).attr('class') == storageCart[i][3][j][2]) {
                                tempData.Price = $(this).find('.priceLevel').find('select').val();
                                if (tempData.Price == "") {
                                    emptyPrices = "true";
                                }
                            }
                        });
                        cartData.unshift(tempData);
                    }
                }
                if (emptyPrices == "true") {
                    NewMessageBox('Some prices are non-existent. Would you like to continue anyway?',
			                "Save",
			                [
			                    {
			                        Text: 'Yes',
			                        Click: function () {
			                            //Send to controller for saving
			                            SaveNewTrans(cartData);
			                        }
			                    },
			                    {
			                        Text: 'No'
			                    }
			        ]); // END MessageBox
                } else {
			        //Send to controller for saving
                    SaveNewTrans(cartData);
                }
            } else {
                //Check for optional values and user defined settings
                if ($('#chkCompanyNameLookUp').prop('checked')) {
                    sessionStorage.setItem("IncCo", "true");
                } else {
                    sessionStorage.setItem("IncCo", "false");
                }
                if ($('#chkPricingLookUp').prop('checked')) {
                    sessionStorage.setItem("IncPricing", "true");
                } else {
                    sessionStorage.setItem("IncPricing", "false");
                }
                if (localStorage.getItem("noprintList") != "null") {
                    //Check if there will be some labels that should not be included in reprinting
                    var tempCart = JSON.parse(sessionStorage.getItem("BarcodeTags"));
                    var finalCart = [];
                    var noprintCart = JSON.parse(localStorage["noprintList"]);
                    for (var i = 0; i < tempCart.length; i++) {
                        var hitList = "false";
                        for (var j = 0; j < noprintCart.length; j++) {
                            if (tempCart[i].BarcodeID == noprintCart[j]) {
                                hitList = "true";
                            }
                        }
                        if (hitList == "false") {
                            finalCart.push(tempCart[i]);
                        }
                    }
                    sessionStorage['BarcodeTags'] = JSON.stringify(finalCart);
                }
                
                var cartData = []
                var storageCart = JSON.parse(sessionStorage.getItem("BarcodeTags"));
                for (var i = 0; i < storageCart.length; i++) {
                    var tempData = {};
                    tempData.ID = NewID();
                    tempData.SkuDesc = storageCart[i].ProductID;
                    tempData.UoM = storageCart[i].UoMID;
                    tempData.NumLabels = storageCart[i].NumLabels;
                    tempData.Barcode = storageCart[i].BarcodeID;
                    tempData.Price = storageCart[i].PriceID;
                    tempData.IsLookUp = CBool(sessionStorage.getItem("Lookup"));
                    tempData.TransactionID = sessionStorage.getItem("TransID");
                    cartData.unshift(tempData);
                }

                var insertCartData = [];
                var storageCart = JSON.parse(localStorage["barcodeTransaction"]);

                if (storageCart != null) {
                    var emptyPrices = "false";
                    for (var i = 0; i < storageCart.length; i++) {
                        for (var j = 0; j < storageCart[i][3].length; j++) {
                            var tempData = {};
                            tempData.ID = NewID();
                            tempData.SkuDesc = storageCart[i][0];
                            tempData.UoM = storageCart[i][3][j][0];
                            tempData.NumLabels = storageCart[i][3][j][4];
                            tempData.Barcode = storageCart[i][3][j][2];
                            tempData.Price = "";
                            tempData.IsLookUp = CBool(sessionStorage.getItem("Lookup"));
                            tempData.TransactionID = "";
                            $("#tblBodyLookup tr").each(function () {
                                if ($(this).find("." + storageCart[i][3][j][2]).attr('class') == storageCart[i][3][j][2]) {
                                    tempData.Price = $(this).find('.priceLevel').find('select').val();
                                    if (tempData.Price == "") {
                                        emptyPrices = "true";
                                    }
                                }
                            });
                            insertCartData.unshift(tempData);
                        }
                    }
                }                

                if (emptyPrices == "true") {
                    NewMessageBox('Some prices are non-existent. Would you like to continue anyway?',
			                "Save",
			                [
			                    {
			                        Text: 'Yes',
			                        Click: function () {
			                            //Send to controller for saving
			                            SaveExistingTrans(cartData, insertCartData)
			                        }
			                    },
			                    {
			                        Text: 'No'
			                    }
			                ]); // END MessageBox
                } else {
                    //Send to controller for saving
                    SaveExistingTrans(cartData, insertCartData)
                }
            }
        }

        //Function for lookup
        //Gets all the data from the server and populate it to the table
        function GetData(TransNum) {
            var pData = {};
            pData.TransNum = TransNum;
            $('#addingContent').hide();
            $('#lookupContent').show();
            $('#tblBodyLookup').empty();
            $('#tblBody').empty();
            $('#chkCompanyNameLookUp, #chkPricingLookUp').prop('checked', false).checkboxradio('refresh');
            $('#txtTransNo').text(TransNum)
            localStorage.setItem('barcodeTransaction', null);
            localStorage.setItem('noprintList', null);
            sessionStorage.removeItem("changed");
            sessionStorage.removeItem("BarcodeTags");
            transNum = TransNum;
            AjaxSend({
                url: 'ShelfAndBinLabeling/LookupBarcodeData',
                data: pData,
                success: function (vData) {
                    sessionStorage['BarcodeTags'] = JSON.stringify(vData.Results.Rows);
                    var list = vData.Results.Rows;
                    $('#tblBodyLookup').empty();
                    if (list.length > 0) {
                        for (var i = 0; i < list.length; i++) {
                            $('#tblBodyLookup').append($('<tr>')
                                .attr('id', list[i].ProductID)
                                .attr('barcodeid', list[i].BarcodeID)
                                .append($('<td>')
                                    .text(list[i].ProductNo)
                                )
                                .append($('<td>')
                                    .text(list[i].SkuDesc)
                                )
                                .append($('<td>')
                                    .text(list[i].UoM)
                                )
                                .append($('<td>')
                                    .text(list[i].PriceLevel)
                                )
                                .append($('<td>')
                                    .text(list[i].Barcode)
                                )
                                .append($('<td>')
                            //Bind an action to update the quanitity of labels to be printed
                            //If changed, saved it as a new transaction
                                    .append($('<input type="number" placeholder="Qty">')
                                        .attr('id', list[i].BarcodeID)
                                        .val(list[i].NumLabels)
                                        .change(function () {
                                            sessionStorage.setItem("changed", "true");
                                            var charCount = $(this).val().length;
                                            if (charCount != 0) {
                                                var storageCart = JSON.parse(sessionStorage.getItem("BarcodeTags"));
                                                for (var j = 0; j < storageCart.length; j++) {
                                                    if (storageCart[j].BarcodeID == $(this).attr('id')) {
                                                        storageCart[j].NumLabels = $(this).val();
                                                    }
                                                }
                                                sessionStorage['BarcodeTags'] = JSON.stringify(storageCart);
                                            }
                                        })
                                    )
                                )
                            //Bind an action to check if this label will be included in reprinting
                                .append($('<td>')
                                    .append($('<input type="checkbox">')
                                        .attr('id', list[i].BarcodeID)
                                        .attr('class', "reprintList")
                                        .attr('checked', true)
                                        .change(function () {
                                            if ($(this).prop('checked')) {
                                                var noprintCart = JSON.parse(localStorage["noprintList"]);
                                                var tempArray = [];
                                                for (var i = 0; i < noprintCart.length; i++) {
                                                    if (noprintCart[i] != $(this).attr('id')) {
                                                        tempArray.push(noprintCart.pop());
                                                    }
                                                }
                                                noprintCart = tempArray;
                                                localStorage["noprintList"] = JSON.stringify(noprintCart);
                                            } else {
                                                if (localStorage.getItem("noprintList") == "null") {
                                                    var noprintCart = [];
                                                } else {
                                                    var noprintCart = JSON.parse(localStorage["noprintList"]);
                                                }
                                                if (noprintCart === null) {
                                                    noprintCart.unshift($(this).attr('id'));
                                                } else {
                                                    noprintCart.push($(this).attr('id'));
                                                }
                                                localStorage["noprintList"] = JSON.stringify(noprintCart);
                                            }
                                        })
                                    )
                                )
                            );
                        }
                    }
                }
            });
        }

        function SaveNewTrans(cartData) {
            var pData = {};
            pData.OrgID = sessionStorage.OrgID;
            pData.Cart = cartData;
            AjaxSend({
                url: 'ShelfAndBinLabeling/GetProductBarcodeData',
                data: pData,
                success: function (vData) {
                    //Check for optional values and user defined settings
                    if ($('#chkCompanyName').prop('checked')) {
                        sessionStorage.setItem("IncCo", "true");
                    } else {
                        sessionStorage.setItem("IncCo", "false");
                    }
                    if ($('#chkPricing').prop('checked')) {
                        sessionStorage.setItem("IncPricing", "true");
                    } else {
                        sessionStorage.setItem("IncPricing", "false");
                    }
                    sessionStorage['BarcodeTags'] = JSON.stringify(vData.Results.Rows);
                    var f = window.open('GenerateBarcodeLabels', '_blank');
                    NewMessageBox('Your Barcode labels have been saved. Were you able to download the PDF file?',
                        "Message",
                        [
                            {
                                Text: 'Yes',
                                Click: function () {
                                    $('#btnMainPage').click();
                                }
                            },
                            {
                                Text: 'No',
                                Click: function () {
                                    var f = window.open('GenerateBarcodeLabels', '_blank');
                                }
                            }
                        ]); // END MessageBox
                }
            });
        }

        function SaveExistingTrans(cartData, insertCartData) {
            var pData = {};
            pData.OrgID = sessionStorage.OrgID;
            pData.Cart = cartData;
            pData.NewItem = insertCartData;
            pData.TransNum = transNum;

            NewMessageBox('Please wait ...');

            AjaxSend({
                url: 'ShelfAndBinLabeling/GetProductBarcodeData',
                data: pData,
                success: function (vData) {
                    sessionStorage['BarcodeTags'] = JSON.stringify(vData.Results.Rows);
                    
                    var f = window.open('GenerateBarcodeLabels', '_blank');
                    NewMessageBox('Your Barcode labels have been saved. Were you able to download the PDF file?',
                        "Message",
                        [
                            {
                                Text: 'Yes',
                                Click: function () {
                                    $('#btnMainPage').click();
                                }
                            },
                            {
                                Text: 'No',
                                Click: function () {
                                    var f = window.open('GenerateBarcodeLabels', '_blank');
                                }
                            }
                        ]); // END MessageBox
                }
            });
            
        }
    });

})