require([
    'common'
    ], function () {

        $(function () {

            var IsNewOrgFowstProductBinAdd = true;
            var arOrgFowstProductBin = new Array();
            var intOrgFowstProductBinCntr = 0;
            var intOrgFowstProductBinIndex = 0;

            var intOrgFowstProductBinRecID = 0;
            var intOrgFowstProductBinFowstID = 1;
            var intOrgFowstProductBinProductID = 2;
            var intOrgFowstProductBinID = 3;
            var intOrgFowstProductBinName = 4;
            var intOrgFowstProductBinMin = 5;
            var intOrgFowstProductBinMax = 6;
            var intOrgFowstProductBinState = 7;

            var mLoaded = false;

            $.mobile.activePage.bind('pageshow', PageShow);
            function PageShow() {
                mLoaded = true;
                ShowOrgFOWSTProductList();
            };
            if (!mLoaded) PageShow();

            $('body').on('click', '#btnOrgFOWSTProductAdd', function () {
                //$.mobile.changePage('#pageOrgFOWSTProductInfo');
                $('#pageOrgFOWSTProductList').hide();
                $('#pageOrgFOWSTProductInfo').show();
                $('#pageOrgFOWSTProductBinInfo').hide();
                ClearOrgFowstProductFields();
            })
            .on('click', '#btnOrgFOWSTProduct', function () {
                SearchPIMProduct();
            })
            .on('keypress', '#txtSearchKeyword', function (e) {
                if (e.keyCode == 13) {
                    SearchPIMProduct($('#txtSearchKeyword').val());
                }
            })
            .on('click', '#btnAddOrgFOWSTProductBin', function () {
                $('#pageOrgFOWSTProductList').hide();
                $('#pageOrgFOWSTProductInfo').hide();
                $('#pageOrgFOWSTProductBinInfo').show();

                IsNewOrgFowstProductBinAdd = true
                intOrgFowstProductBinCntr = intOrgFowstProductBinCntr + 1;
                intOrgFowstProductBinIndex = intOrgFowstProductBinCntr;

                ClearOrgFowstProductBinFields();
            })
            .on('click', '#btnOrgFOWSTProductBinSave', function () {
                if ($.trim($("#txtOrgFOWSTBin").val()) == "") {
                    NewMessageBox('Please enter bin name');
                    return;
                }

                SaveOrgFOWSTProductBin();
            })
            .on('click', '#btnOrgFOWSTProductCancel', function () {
                $('#pageOrgFOWSTProductList').show();
                $('#pageOrgFOWSTProductInfo').hide();
                $('#pageOrgFOWSTProductBinInfo').hide();
            })
            .on('click', '#btnOrgFOWSTProductListDone', function () {
                window.location.hash = 'LibraryPage';
            })
            .on('keydown', '#txtOrgFOWSTBinMin', function (e) {
                if (!NumberOnly(e)) {
                    return;
                }
            })
            .on('keydown', '#txtOrgFOWSTBinMax', function (e) {
                if (!NumberOnly(e)) {
                    return;
                }
            })
            .on('click', '#btnOrgFOWSTProductBinCancel', function () {
                $('#pageOrgFOWSTProductList').hide();
                $('#pageOrgFOWSTProductInfo').show();
                $('#pageOrgFOWSTProductBinInfo').hide();
            })
            .on('click', '#btnOrgFOWSTProductSave', function () {
                if ($.trim($("#txtOrgFOWSTProductID").val()) == "") {
                    NewMessageBox('Please select product');
                    return;
                }

                SaveOrganizationFowstProduct();
            })
            .on('keypress', '#txtOrgFowstProductSearch', function (e) {
                if (e.keyCode == 13) {
                    ShowOrgFOWSTProductList();

                    return false;
                }
            })

            function SearchPIMProduct(s) {
                BrowsePopup('LibraryPIMProduct', 'Search Product', [
                    {
                        DisplayText: [{ Column: 'ProductName' }],
                        Click: function (e) {
                            $('#txtOrgFOWSTProductID').val(e.ID);
                            $('#btnOrgFOWSTProduct').text(e.ProductName);
                        }
                    }
                ])
            }

            function ShowOrgFOWSTProductList() {
                var pData = {};
                var pParams = GetUrlParameters();

                pData.FowstID = sessionStorage.OrgFowstID; //pParams.OrgFowstID
                pData.SearchKeyword = $("#txtOrgFowstProductSearch").val();

                AjaxSend({
                    url: "Organization/getOrganizationFowstProductList",
                    data: pData,
                    success: function (vData) {
                        LoadOrgFOWSTProductList(vData.OrgFowstProductData.Rows);
                    }
                });

            };

            function LoadOrgFOWSTProductList(vRows) {
                var pLV = $('#lstOrgFOWSTProductList');
                var pItem;

                $("#divOrgFOWSTProductRecordCount").text(" Number of record(s) found: " + vRows.length);

                pLV.html('');
                for (var p = 0; p <= vRows.length - 1; p++) {
                    pItem = $('<a href="" class="list-group-item list-group-item-action">')
                            .attr('ID', vRows[p].ID)
                            .click(function (e) {
                                e.preventDefault();
                                //$.mobile.changePage('#pageOrgFOWSTProductInfo');
                                $('#pageOrgFOWSTProductList').hide();
                                $('#pageOrgFOWSTProductInfo').show();
                                $('#pageOrgFOWSTProductBinInfo').hide();
                                ShowOrgFOWSTProductInfo($(this).attr('ID'));
                            })
                            .append($('<h5>')
                                .attr("style", "white-space:normal")
                                .text(vRows[p].ProductName)
                            )
                    pLV.append(pItem);
                };

                pLV;//.listview('refresh');
            };

            function ShowOrgFOWSTProductInfo(strID) {
                var pData = {};
                pData.ID = strID;

                ClearOrgFowstProductFields();

                AjaxSend({
                    url: "Organization/getOrganizationFowstProductInfo",
                    data: pData,
                    success: function (vData) {
                        if (vData.OrgFowstProductData.Rows.length > 0) {
                            $("#txtOrgFOWSTProdID").val(vData.OrgFowstProductData.Rows[0].ID || '');

                            $("#txtOrgFOWSTID").val(vData.OrgFowstProductData.Rows[0].FowstID || '');
                            $("#txtOrgFOWSTProductID").val(vData.OrgFowstProductData.Rows[0].ProductID || '');
                            $("#btnOrgFOWSTProduct").text(vData.OrgFowstProductData.Rows[0].ProductName || 'Please Select');//.button('refresh');

                            $("#txtOrgFOWSTProductReorderPoint").val(vData.OrgFowstProductData.Rows[0].ReorderPoint || '0');

                            //Get Product Bins
                            pData.FowstID = (vData.OrgFowstProductData.Rows[0].FowstID || '');
                            pData.ProductID = (vData.OrgFowstProductData.Rows[0].ProductID || '');
                            AjaxSend({
                                url: 'Organization/getOrganizationFowstProductBinList',
                                data: pData,
                                success: function (vData) {
                                    LoadOrgFowstProductBin(vData.OrgFowstProductData.Rows);
                                }
                            });

                        }
                    }
                });

            };

            function LoadOrgFowstProductBin(vRows) {

                for (var p = 0; p <= vRows.length - 1; p++) {

                    arOrgFowstProductBin = new Array();
                    arOrgFowstProductBin[intOrgFowstProductBinRecID] = vRows[p].ID;
                    arOrgFowstProductBin[intOrgFowstProductBinFowstID] = vRows[p].FowstID;
                    arOrgFowstProductBin[intOrgFowstProductBinProductID] = vRows[p].ProductID;
                    arOrgFowstProductBin[intOrgFowstProductBinID] = vRows[p].BinID;
                    arOrgFowstProductBin[intOrgFowstProductBinName] = vRows[p].BinName;
                    arOrgFowstProductBin[intOrgFowstProductBinMin] = vRows[p].MinPerProduct;
                    arOrgFowstProductBin[intOrgFowstProductBinMax] = vRows[p].MaxPerProduct;
                    arOrgFowstProductBin[intOrgFowstProductBinState] = "";

                    sessionStorage["OrgFowstProductBin-" + p] = JSON.stringify(arOrgFowstProductBin);

                };

                intOrgFowstProductBinCntr = vRows.length;
                LoadOrgFowstProductBinList();

            };

            function SaveOrganizationFowstProduct() {
                var pData = [];
                var pDataBin = [];
                var intIndex = 0;
                var pParams = GetUrlParameters();

                //Bin
                intIndex = 0;
                for (var i = 0; i <= intOrgFowstProductBinCntr; i++) {

                    if (sessionStorage["OrgFowstProductBin-" + i] != undefined) {

                        arOrgFowstProductBin = new Array();
                        arOrgFowstProductBin = JSON.parse(sessionStorage["OrgFowstProductBin-" + i]);

                        pDataBin[intIndex] = {
                            ID: arOrgFowstProductBin[intOrgFowstProductBinRecID],
                            FowstID: arOrgFowstProductBin[intOrgFowstProductBinFowstID],
                            ProductID: arOrgFowstProductBin[intOrgFowstProductBinProductID],
                            BinID: arOrgFowstProductBin[intOrgFowstProductBinID],
                            BinName: arOrgFowstProductBin[intOrgFowstProductBinName],
                            BinMin: arOrgFowstProductBin[intOrgFowstProductBinMin],
                            BinMax: arOrgFowstProductBin[intOrgFowstProductBinMax],
                            RecState: arOrgFowstProductBin[intOrgFowstProductBinState]
                        };

                        intIndex = intIndex + 1;

                    }
                }

                pData = {
                    OrgFowstProductID: $.trim($("#txtOrgFOWSTProdID").val()),
                    FowstID: sessionStorage.OrgFowstID, //pParams.OrgFowstID,
                    ProductID: $.trim($("#txtOrgFOWSTProductID").val()),
                    ReOrderPoint: $.trim($("#txtOrgFOWSTProductReorderPoint").val()),
                    OrgFOWSTProductBinList: pDataBin
                };

                AjaxSend({
                    url: "Organization/CheckOrganizationFOWSTProduct",
                    data: pData,
                    success: function (vData) {

                        if (vData.OrgFowstProductData.Rows.length > 0) {
                            NewMessageBox('Product being added already exist');
                        }
                        else {
                            AjaxSend({
                                url: "Organization/SaveOrganizationFowstProduct",
                                data: pData,
                                success: function (vSaveData) {

                                    //$.mobile.changePage('#pageOrgFOWSTProductList');
                                    $('#pageOrgFOWSTProductList').show();
                                    $('#pageOrgFOWSTProductInfo').hide();
                                    $('#pageOrgFOWSTProductBinInfo').hide();
                                    ShowOrgFOWSTProductList();

                                }
                            });

                        }

                    }
                });

            };

            function ShowOrgFowstProductBinInfo(arData, index) {

                //$.mobile.changePage('#pageOrgFOWSTProductBinInfo');
                $('#pageOrgFOWSTProductList').hide();
                $('#pageOrgFOWSTProductInfo').hide();
                $('#pageOrgFOWSTProductBinInfo').show();

                ClearOrgFowstProductBinFields();

                intOrgFowstProductBinIndex = index;

                $("#txtOrgFOWSTProductBinID").val(arData[intOrgFowstProductBinRecID]);

                $("#txtOrgFOWSTBinID").val(arData[intOrgFowstProductBinID]);
                $("#txtOrgFOWSTBin").val(arData[intOrgFowstProductBinName]);
                $("#txtOrgFOWSTBinMin").val(arData[intOrgFowstProductBinMin]);
                $("#txtOrgFOWSTBinMax").val(arData[intOrgFowstProductBinMax]);

                IsNewOrgFowstProductBinAdd = false;

            };

            function ClearOrgFowstProductFields() {
                var pParams = GetUrlParameters();

                $("#txtOrgFOWSTProdID").val('');

                $("#txtOrgFOWSTID").val(sessionStorage.OrgFowstID); //pParams.OrgFowstID

                $("#txtOrgFOWSTProductID").val('');
                $("#btnOrgFOWSTProduct").text('Please Select');//.button('refresh');

                $("#txtOrgFOWSTProductReorderPoint").val('0');


                for (var i = 0; i <= 100; i++) {

                    if (sessionStorage["OrgFowstProductBin-" + i] != undefined) {
                        sessionStorage.removeItem("OrgFowstProductBin-" + i);
                    }
                }

                LoadOrgFowstProductBinList();

            };

            function ClearOrgFowstProductBinFields() {

                $("#txtOrgFOWSTProductBinID").val('');

                $("#txtOrgFOWSTBinID").val('');
                $("#txtOrgFOWSTBin").val('');
                $("#txtOrgFOWSTBinMin").val('0');
                $("#txtOrgFOWSTBinMax").val('0');

            }

            function SaveOrgFOWSTProductBin() {

                arOrgFowstProductBin = new Array();
                arOrgFowstProductBin[intOrgFowstProductBinRecID] = $("#txtOrgFOWSTProductBinID").val();
                arOrgFowstProductBin[intOrgFowstProductBinFowstID] = $("#txtOrgFOWSTID").val();
                arOrgFowstProductBin[intOrgFowstProductBinProductID] = $("#txtOrgFOWSTProductID").val();
                arOrgFowstProductBin[intOrgFowstProductBinID] = $("#txtOrgFOWSTBinID").val();
                arOrgFowstProductBin[intOrgFowstProductBinName] = $("#txtOrgFOWSTBin").val();
                arOrgFowstProductBin[intOrgFowstProductBinMin] = $("#txtOrgFOWSTBinMin").val();
                arOrgFowstProductBin[intOrgFowstProductBinMax] = $("#txtOrgFOWSTBinMax").val();
                arOrgFowstProductBin[intOrgFowstProductBinState] = (IsNewOrgFowstProductBinAdd ? "New" : "Edit");

                sessionStorage["OrgFowstProductBin-" + intOrgFowstProductBinIndex] = JSON.stringify(arOrgFowstProductBin);

                //$.mobile.changePage('#pageOrgFOWSTProductInfo');
                $('#pageOrgFOWSTProductList').hide();
                $('#pageOrgFOWSTProductInfo').show();
                $('#pageOrgFOWSTProductBinInfo').hide();
                LoadOrgFowstProductBinList();
            };

            function RemoveOrgFOWSTSection() {

                arOrgFowstProductBin = new Array();
                arOrgFowstProductBin[intOrgFowstProductBinRecID] = $("#txtOrgFOWSTProductBinID").val();
                arOrgFowstProductBin[intOrgFowstProductBinFowstID] = $("#txtOrgFOWSTID").val();
                arOrgFowstProductBin[intOrgFowstProductBinProductID] = $("#txtOrgFOWSTProductID").val();
                arOrgFowstProductBin[intOrgFowstProductBinID] = $("#txtOrgFOWSTBinID").val();
                arOrgFowstProductBin[intOrgFowstProductBinName] = $("#txtOrgFOWSTBin").val();
                arOrgFowstProductBin[intOrgFowstProductBinMin] = $("#txtOrgFOWSTBinMin").val();
                arOrgFowstProductBin[intOrgFowstProductBinMax] = $("#txtOrgFOWSTBinMax").val();
                arOrgFowstProductBin[intOrgFowstProductBinState] = "Remove";

                sessionStorage["OrgFowstProductBin-" + intOrgFowstProductBinIndex] = JSON.stringify(arOrgFowstProductBin);

                //$.mobile.changePage('#pageOrgFOWSTProductInfo');
                $('#pageOrgFOWSTProductList').hide();
                $('#pageOrgFOWSTProductInfo').show();
                $('#pageOrgFOWSTProductBinInfo').hide();
                LoadOrgFowstProductBinList();
            };

            function LoadOrgFowstProductBinList() {
                var pLV = $('#lstOrgFOWSTProductBin');
                var pItem;

                pLV.html('');
                for (var i = 0; i <= intOrgFowstProductBinCntr; i++) {

                    if (sessionStorage["OrgFowstProductBin-" + i] != undefined) {

                        arOrgFowstProductBin = new Array();
                        arOrgFowstProductBin = JSON.parse(sessionStorage["OrgFowstProductBin-" + i]);

                        if (arOrgFowstProductBin[intOrgFowstProductBinState] != "Remove") {

                            pItem = $('<a href="" class="list-group-item list-group-item-action">')
                                    .attr('ID', i)
                                    .data('Data', arOrgFowstProductBin)
                                    .click(function (e) {
                                        e.preventDefault();
                                        ShowOrgFowstProductBinInfo($(this).data('Data'), $(this).attr('ID'));
                                    })
                                    .append($('<h5>')
                                        .attr("style", "white-space:normal")
                                        .text(arOrgFowstProductBin[intOrgFowstProductBinName])
                                    )

                            pLV.append(pItem);

                        }

                    }
                }

                pLV;//.listview('refresh');
            };



        });
    });