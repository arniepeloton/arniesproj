require([
    'common'
    ], function () {

        $(function () {

            var IsNewOrgCustFowstProductBinAdd = true;
            var arOrgCustFowstProductBin = new Array();
            var intOrgCustFowstProductBinCntr = 0;
            var intOrgCustFowstProductBinIndex = 0;

            var intOrgCustFowstProductBinRecID = 0;
            var intOrgCustFowstProductBinFowstID = 1;
            var intOrgCustFowstProductBinProductID = 2;
            var intOrgCustFowstProductBinID = 3;
            var intOrgCustFowstProductBinName = 4;
            var intOrgCustFowstProductBinMin = 5;
            var intOrgCustFowstProductBinMax = 6;
            var intOrgCustFowstProductBinState = 7;

            var mLoaded = false;

            $.mobile.activePage.bind('pageshow', PageShow);
            function PageShow() {
                mLoaded = true;
                ShowOrgCustFOWSTProductList();
            };
            if (!mLoaded) PageShow();

            window.OrgShortCusFowstProdList = {
                ShowOrgCusFowstProdList: function () {
                    ShowOrgCustFOWSTProductList();
                }
            }

            $('body').on('submit', '#frmOrgCustFowstProductSearch', function (e) {
                e.preventDefault();

                ShowOrgCustFOWSTProductList();

                return false;
            })
            .on('click', '#btnOrgCustFOWSTProductAdd', function (e) {
                e.preventDefault();

                //$.mobile.changePage('#pageOrgCustFOWSTProductInfo');
                $('#pageOrgCustFOWSTProductList').hide();
                $('#pageOrgCustFOWSTProductInfo').show();
                $('#pageOrgCustFOWSTProductBinInfo').hide();
                ClearOrgCustFowstProductFields();
            })
            .on('click', '#btnOrgCustFOWSTProduct', function (e) {
                $('#TitleLibrary').text('PRODUCT');
                $('#txtSearchKeyword').val('').attr('placeholder', 'Type name of Product');
                $('#panSearchLibrary').popup('open');
                $('#lstOfLibrary').empty();
                //                Browser.LibraryPIMProduct.InternalInputData.IsSearchMore = 1;
                //                Browser.LibraryPIMProduct.Click = function (e) {

                //                    $('#txtOrgCustFOWSTProductID').val(e.ID)
                //                    $('#btnOrgCustFOWSTProduct').text(e.ProductName).button('refresh');

                //                };

                //                Browser.LibraryPIMProduct.Show();
            })
            .on('click', '#btnOrgCustFOWSTProductSave', function (e) {
                e.preventDefault();

                if ($.trim($("#txtOrgCustFOWSTProductID").val()) == "") {
                    $('#TitleOrgEmp').text('Product');
                    $('#MsgOrgEmp').text('Please select product.');
                    $('#panOrgEmpMessage').popup('open');
                    $('#btnOKOrgEmp').text('Ok').click(function (e) {
                        e.preventDefault();
                        $('#panOrgEmpMessage').popup('close');
                    })
                    $('#divBtnCancel').hide();
                    return;
                }

                SaveOrganizationCustFowstProduct();
            })
            .on('click', '#btnOrgCustFOWSTProductCancel', function (e) {
                e.preventDefault();
                //$.mobile.changePage('#pageOrgCustFOWSTProductList');
                $('#pageOrgCustFOWSTProductList').show();
                $('#pageOrgCustFOWSTProductInfo').hide();
                $('#pageOrgCustFOWSTProductBinInfo').hide();
            })
            .on('click', '#btnOrgCustFOWSTProductListDone', function (e) {
                e.preventDefault();

                if (window.pageSourcePageFowst != undefined) {
                    $.mobile.changePage("#" + $(window.pageSourcePageFowst).attr('id'));
                }
                else {
                    window.open(GetPathFormat("/Organization/OrganizationCustomerFOWST") + "?OrgID=" + sessionStorage["OrgID"], "_self");
                }
            })
            .on('click', '#btnAddOrgCustFOWSTProductBin', function (e) {
                e.preventDefault();

                //$.mobile.changePage('#pageOrgCustFOWSTProductBinInfo');
                $('#pageOrgCustFOWSTProductList').hide();
                $('#pageOrgCustFOWSTProductInfo').hide();
                $('#pageOrgCustFOWSTProductBinInfo').show();

                IsNewOrgCustFowstProductBinAdd = true
                intOrgCustFowstProductBinCntr = intOrgCustFowstProductBinCntr + 1;
                intOrgCustFowstProductBinIndex = intOrgCustFowstProductBinCntr;

                ClearOrgFowstProductBinFields();
            })
            .on('keydown', '#txtOrgCustFOWSTBinMin', function (e) {
                if (!NumberOnly(e)) {
                    return;
                }
            })
            .on('keydown', '#txtOrgCustFOWSTBinMax', function (e) {
                if (!NumberOnly(e)) {
                    return;
                }
            })
            .on('click', '#btnOrgCustFOWSTProductBinSave', function (e) {
                e.preventDefault();

                if ($.trim($("#txtOrgCustFOWSTBin").val()) == "") {
                    $('#TitleOrgEmp').text('BIN Name');
                    $('#MsgOrgEmp').text('Please enter bin name.');
                    $('#panOrgEmpMessage').popup('open');
                    $('#btnOKOrgEmp').text('Ok').click(function (e) {
                        e.preventDefault();
                        $('#panOrgEmpMessage').popup('close');
                    })
                    $('#divBtnCancel').hide();
                    return;
                }

                SaveOrgFOWSTProductBin();
            })
            .on('click', '#btnOrgCustFOWSTProductBinCancel', function (e) {
                e.preventDefault();

                //$.mobile.changePage('#pageOrgCustFOWSTProductInfo');
                $('#pageOrgCustFOWSTProductList').hide();
                $('#pageOrgCustFOWSTProductInfo').show();
                $('#pageOrgCustFOWSTProductBinInfo').hide();
            })
            .on('keypress', '#txtSearchKeyword', function (e) {
                if (e.keyCode == 13) {
                    SearchProductPopUp($(this).val());
                }
            })

            function SearchProductPopUp(s) {
                var pData = {};

                pData.keywords = s;
                pData.IsSearchMore = 0;

                AjaxSend({
                    url: 'Browser/LibraryPIMProduct',
                    data: pData,
                    success: function (vData) {
                        $('#lstOfLibrary').empty();

                        if (vData.Results.Rows.length > 0) {
                            for (var p = 0; p < vData.Results.Rows.length; p++) {
                                $('#lstOfLibrary').append($('<li>')
                                    .append($('<a>')
                                        .attr('ID', vData.Results.Rows[p].ID)
                                        .append($('<br><p style="font-weight:bold;">')
                                            .text(vData.Results.Rows[p].ProductName)
                                        )
                                        .append($('<p>')
                                            .text('Published by: ' + vData.Results.Rows[p].PublishedBy)
                                        )
                                        .append($('<p>')
                                            .text('Uploaded by: ' + vData.Results.Rows[p].UploadedBy)
                                        )
                                        .data('data', vData.Results.Rows[p])
                                        .click(function (e) {
                                            $('#txtOrgCustFOWSTProductID').val($(this).attr('ID'));
                                            $('#btnOrgCustFOWSTProduct').text($(this).data('data').ProductName).button('refresh');

                                            $('#panSearchLibrary').popup('close');
                                        })
                                    ));
                            }

                        }
                        else {
                            $('#lstOfLibrary').append($('<li>').text('No Result(s) Found'));
                        }

                        $('#lstOfLibrary').listview('refresh');
                    }
                });
            }

            function ShowOrgCustFOWSTProductList() {
                var pData = {};

                pData.SearchKeyword = $("#txtOrgCustFowstProductSearch").val();

                AjaxSend({
                    url: "Organization/getOrganizationCustomerFOWSTProductList",
                    data: pData,
                    success: function (vData) {
                        LoadOrgCustFOWSTProductList(vData.OrgCustFowstProductData.Rows);
                    }
                });

            };

            function LoadOrgCustFOWSTProductList(vRows) {
                var pLV = $('#lstOrgCustFOWSTProductList');
                var pItem;

                $("#divOrgCustFOWSTProductRecordCount").text(" Number of record(s) found: " + vRows.length);

                pLV.html('');
                for (var p = 0; p <= vRows.length - 1; p++) {
                    pItem = $('<li>')
                        .append($('<a>')
                            .attr('ID', vRows[p].ID)
                            .click(function (e) {
                                e.preventDefault();
                                //$.mobile.changePage('#pageOrgCustFOWSTProductInfo');
                                $('#pageOrgCustFOWSTProductList').hide();
                                $('#pageOrgCustFOWSTProductInfo').show();
                                $('#pageOrgCustFOWSTProductBinInfo').hide();
                                ShowOrgFOWSTProductInfo($(this).attr('ID'));
                            })
                            .append($('<h3>')
                                .attr("style", "white-space:normal")
                                .text(vRows[p].ProductName)
                            )
                        );
                    pLV.append(pItem);
                };

                pLV.listview('refresh');
            };

            function ShowOrgFOWSTProductInfo(strID) {
                var pData = {};
                pData.ID = strID;

                ClearOrgCustFowstProductFields();

                AjaxSend({
                    url: "Organization/getOrganizationCustomerFowstProductInfo",
                    data: pData,
                    success: function (vData) {
                        if (vData.OrgCustFowstProductData.Rows.length > 0) {
                            $("#txtOrgCustFOWSTProdID").val(vData.OrgCustFowstProductData.Rows[0].ID || '');

                            $("#txtOrgCustFOWSTID").val(vData.OrgCustFowstProductData.Rows[0].FowstID || '');
                            $("#txtOrgCustFOWSTProductID").val(vData.OrgCustFowstProductData.Rows[0].ProductID || '');
                            $("#btnOrgCustFOWSTProduct").text(vData.OrgCustFowstProductData.Rows[0].ProductName || 'Please Select').button('refresh');

                            $("#txtOrgCustFOWSTProductReorderPoint").val(vData.OrgCustFowstProductData.Rows[0].ReorderPoint || '0');

                            //Get Product Bins
                            pData.FowstID = (vData.OrgCustFowstProductData.Rows[0].FowstID || '');
                            pData.ProductID = (vData.OrgCustFowstProductData.Rows[0].ProductID || '');
                            AjaxSend({
                                url: 'Organization/getOrganizationCustomerFowstProductBinList',
                                data: pData,
                                success: function (vData) {
                                    LoadOrgFowstProductBin(vData.OrgCustFowstProductData.Rows);
                                }
                            });

                        }
                    }
                });

            };

            function LoadOrgFowstProductBin(vRows) {

                for (var p = 0; p <= vRows.length - 1; p++) {

                    arOrgCustFowstProductBin = new Array();
                    arOrgCustFowstProductBin[intOrgCustFowstProductBinRecID] = vRows[p].ID;
                    arOrgCustFowstProductBin[intOrgCustFowstProductBinFowstID] = vRows[p].FowstID;
                    arOrgCustFowstProductBin[intOrgCustFowstProductBinProductID] = vRows[p].ProductID;
                    arOrgCustFowstProductBin[intOrgCustFowstProductBinID] = vRows[p].BinID;
                    arOrgCustFowstProductBin[intOrgCustFowstProductBinName] = vRows[p].BinName;
                    arOrgCustFowstProductBin[intOrgCustFowstProductBinMin] = vRows[p].MinPerProduct;
                    arOrgCustFowstProductBin[intOrgCustFowstProductBinMax] = vRows[p].MaxPerProduct;
                    arOrgCustFowstProductBin[intOrgCustFowstProductBinState] = "";

                    sessionStorage["OrgCustFowstProductBin-" + p] = JSON.stringify(arOrgCustFowstProductBin);

                };

                intOrgCustFowstProductBinCntr = vRows.length;
                LoadOrgFowstProductBinList();

            };

            function SaveOrganizationCustFowstProduct() {
                var pData = [];
                var pDataBin = [];
                var intIndex = 0;

                //Bin
                intIndex = 0;
                for (var i = 0; i <= intOrgCustFowstProductBinCntr; i++) {

                    if (sessionStorage["OrgCustFowstProductBin-" + i] != undefined) {

                        arOrgCustFowstProductBin = new Array();
                        arOrgCustFowstProductBin = JSON.parse(sessionStorage["OrgCustFowstProductBin-" + i]);

                        pDataBin[intIndex] = {
                            ID: arOrgCustFowstProductBin[intOrgCustFowstProductBinRecID],
                            FowstID: arOrgCustFowstProductBin[intOrgCustFowstProductBinFowstID],
                            ProductID: arOrgCustFowstProductBin[intOrgCustFowstProductBinProductID],
                            BinID: arOrgCustFowstProductBin[intOrgCustFowstProductBinID],
                            BinName: arOrgCustFowstProductBin[intOrgCustFowstProductBinName],
                            BinMin: arOrgCustFowstProductBin[intOrgCustFowstProductBinMin],
                            BinMax: arOrgCustFowstProductBin[intOrgCustFowstProductBinMax],
                            RecState: arOrgCustFowstProductBin[intOrgCustFowstProductBinState]
                        };

                        intIndex = intIndex + 1;

                    }
                }

                pData = {
                    OrgFowstProductID: $.trim($("#txtOrgCustFOWSTProdID").val()),
                    ProductID: $.trim($("#txtOrgCustFOWSTProductID").val()),
                    ReOrderPoint: $.trim($("#txtOrgCustFOWSTProductReorderPoint").val()),
                    OrgFOWSTProductBinList: pDataBin
                };

                AjaxSend({
                    url: "Organization/CheckOrganizationCustomerFOWSTProduct",
                    data: pData,
                    success: function (vData) {

                        if (vData.OrgCustFowstProductData.Rows.length > 0) {
                            $('#TitleOrgEmp').text('Product Exist');
                            $('#MsgOrgEmp').text('Product being added already exist.');
                            $('#panOrgEmpMessage').popup('open');
                            $('#btnOKOrgEmp').text('Ok').click(function (e) {
                                e.preventDefault();
                                $('#panOrgEmpMessage').popup('close');
                            })
                            $('#divBtnCancel').hide();
                        }
                        else {
                            AjaxSend({
                                url: "Organization/SaveOrganizationCustomerFowstProduct",
                                data: pData,
                                success: function (vSaveData) {

                                    //$.mobile.changePage('#pageOrgCustFOWSTProductList');
                                    $('#pageOrgCustFOWSTProductList').show();
                                    $('#pageOrgCustFOWSTProductInfo').hide();
                                    $('#pageOrgCustFOWSTProductBinInfo').hide();
                                    ShowOrgCustFOWSTProductList();

                                }
                            });

                        }

                    }
                });

            };

            function ShowOrgFowstProductBinInfo(arData, index) {

                //$.mobile.changePage('#pageOrgCustFOWSTProductBinInfo');
                $('#pageOrgCustFOWSTProductList').hide();
                $('#pageOrgCustFOWSTProductInfo').hide();
                $('#pageOrgCustFOWSTProductBinInfo').show();

                ClearOrgFowstProductBinFields();

                intOrgCustFowstProductBinIndex = index;

                $("#txtOrgCustFOWSTProductBinID").val(arData[intOrgCustFowstProductBinRecID]);

                $("#txtOrgCustFOWSTBinID").val(arData[intOrgCustFowstProductBinID]);
                $("#txtOrgCustFOWSTBin").val(arData[intOrgCustFowstProductBinName]);
                $("#txtOrgCustFOWSTBinMin").val(arData[intOrgCustFowstProductBinMin]);
                $("#txtOrgCustFOWSTBinMax").val(arData[intOrgCustFowstProductBinMax]);

                IsNewOrgCustFowstProductBinAdd = false;

            };

            function ClearOrgCustFowstProductFields() {
                var pParams = GetUrlParameters();

                $("#txtOrgCustFOWSTProdID").val('');

                $("#txtOrgCustFOWSTID").val('');

                $("#txtOrgCustFOWSTProductID").val('');
                $("#btnOrgCustFOWSTProduct").text('Please Select').button('refresh');

                $("#txtOrgCustFOWSTProductReorderPoint").val('0');


                for (var i = 0; i <= 100; i++) {

                    if (sessionStorage["OrgCustFowstProductBin-" + i] != undefined) {
                        sessionStorage.removeItem("OrgCustFowstProductBin-" + i);
                    }
                }

                LoadOrgFowstProductBinList();

            };

            function ClearOrgFowstProductBinFields() {

                $("#txtOrgCustFOWSTProductBinID").val('');

                $("#txtOrgCustFOWSTBinID").val('');
                $("#txtOrgCustFOWSTBin").val('');
                $("#txtOrgCustFOWSTBinMin").val('0');
                $("#txtOrgCustFOWSTBinMax").val('0');

            }

            function SaveOrgFOWSTProductBin() {

                arOrgCustFowstProductBin = new Array();
                arOrgCustFowstProductBin[intOrgCustFowstProductBinRecID] = $("#txtOrgCustFOWSTProductBinID").val();
                arOrgCustFowstProductBin[intOrgCustFowstProductBinFowstID] = $("#txtOrgCustFOWSTID").val();
                arOrgCustFowstProductBin[intOrgCustFowstProductBinProductID] = $("#txtOrgCustFOWSTProductID").val();
                arOrgCustFowstProductBin[intOrgCustFowstProductBinID] = $("#txtOrgCustFOWSTBinID").val();
                arOrgCustFowstProductBin[intOrgCustFowstProductBinName] = $("#txtOrgCustFOWSTBin").val();
                arOrgCustFowstProductBin[intOrgCustFowstProductBinMin] = $("#txtOrgCustFOWSTBinMin").val();
                arOrgCustFowstProductBin[intOrgCustFowstProductBinMax] = $("#txtOrgCustFOWSTBinMax").val();
                arOrgCustFowstProductBin[intOrgCustFowstProductBinState] = (IsNewOrgCustFowstProductBinAdd ? "New" : "Edit");

                sessionStorage["OrgCustFowstProductBin-" + intOrgCustFowstProductBinIndex] = JSON.stringify(arOrgCustFowstProductBin);

                //$.mobile.changePage('#pageOrgCustFOWSTProductInfo');
                $('#pageOrgCustFOWSTProductList').hide();
                $('#pageOrgCustFOWSTProductInfo').show();
                $('#pageOrgCustFOWSTProductBinInfo').hide();

                LoadOrgFowstProductBinList();
            };

            function RemoveOrgFOWSTSection() {

                arOrgCustFowstProductBin = new Array();
                arOrgCustFowstProductBin[intOrgCustFowstProductBinRecID] = $("#txtOrgCustFOWSTProductBinID").val();
                arOrgCustFowstProductBin[intOrgCustFowstProductBinFowstID] = $("#txtOrgCustFOWSTID").val();
                arOrgCustFowstProductBin[intOrgCustFowstProductBinProductID] = $("#txtOrgCustFOWSTProductID").val();
                arOrgCustFowstProductBin[intOrgCustFowstProductBinID] = $("#txtOrgCustFOWSTBinID").val();
                arOrgCustFowstProductBin[intOrgCustFowstProductBinName] = $("#txtOrgCustFOWSTBin").val();
                arOrgCustFowstProductBin[intOrgCustFowstProductBinMin] = $("#txtOrgCustFOWSTBinMin").val();
                arOrgCustFowstProductBin[intOrgCustFowstProductBinMax] = $("#txtOrgCustFOWSTBinMax").val();
                arOrgCustFowstProductBin[intOrgCustFowstProductBinState] = "Remove";

                sessionStorage["OrgCustFowstProductBin-" + intOrgCustFowstProductBinIndex] = JSON.stringify(arOrgCustFowstProductBin);

                //$.mobile.changePage('#pageOrgCustFOWSTProductInfo');
                $('#pageOrgCustFOWSTProductList').hide();
                $('#pageOrgCustFOWSTProductInfo').show();
                $('#pageOrgCustFOWSTProductBinInfo').hide();
                LoadOrgFowstProductBinList();
            };

            function LoadOrgFowstProductBinList() {
                var pLV = $('#lstOrgCustFOWSTProductBin');
                var pItem;

                pLV.html('');
                for (var i = 0; i <= intOrgCustFowstProductBinCntr; i++) {

                    if (sessionStorage["OrgCustFowstProductBin-" + i] != undefined) {

                        arOrgCustFowstProductBin = new Array();
                        arOrgCustFowstProductBin = JSON.parse(sessionStorage["OrgCustFowstProductBin-" + i]);

                        if (arOrgCustFowstProductBin[intOrgCustFowstProductBinState] != "Remove") {

                            pItem = $('<li id="lst' + i + '">')
                            .append($('<a>')
                                    .attr('ID', i)
                                    .data('Data', arOrgCustFowstProductBin)
                                    .click(function (e) {
                                        e.preventDefault();
                                        ShowOrgFowstProductBinInfo($(this).data('Data'), $(this).attr('ID'));
                                    })
                                    .append($('<h3>')
                                        .attr("style", "white-space:normal")
                                        .text(arOrgCustFowstProductBin[intOrgCustFowstProductBinName])
                                    )
                                    .append($('<p>')
                                        .attr("style", "white-space:normal")
                                        .text("Minimum Qty : " + arOrgCustFowstProductBin[intOrgCustFowstProductBinMin])
                                    )
                                    .append($('<p>')
                                        .attr("style", "white-space:normal")
                                        .text("Maximum Qty : " + arOrgCustFowstProductBin[intOrgCustFowstProductBinMax])
                                    )
                            );

                            pLV.append(pItem);

                        }

                    }
                }

                pLV.listview('refresh');
            };



        });
    });