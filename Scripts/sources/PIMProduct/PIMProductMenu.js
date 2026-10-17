require([
    'common'
    ], function () {

        $(function () {

            $('#frmPIMProductBrowser').submit(function (e) {
                e.preventDefault();

                ShowPIMProduct();

            });

            $('#btnAddNewPIMProduct').click(function () {

                CreateNewPIMProductEntry(1, '');

            });

            function ShowPIMProduct() {

                var pData = {};
                pData.SearchString = $("#txtPIMProductSearch").val();

                AjaxSend({
                    url: 'PIMProduct/getProductList',
                    data: pData,
                    success: function (vData) {
                        LoadPIMProductList(vData.PIMProductData.Rows);
                    }
                });

            }

            function LoadPIMProductList(vRows) {
                var pLV = $('#lstPIMProduct');
                var pItem;

                $("#divPIMProductSearchResults").hide();
                $("#divPIMProductSearchNoResults").hide();
                $("#btnAddNewPIMProduct").hide();

                if (vRows.length > 0) {
                    $("#divPIMProductSearchResults").show();
                    $("#divPIMProductRecordCount").text(" Number of record(s) found: " + vRows.length);

                    pLV.html('');
                    for (var p = 0; p <= vRows.length - 1; p++) {
                        pItem = $('<li>')
                        .append($('<a>')
                            .attr('ID', vRows[p].ID)
                            .click(function (e) {
                                e.preventDefault();

                                CreateNewPIMProductEntry(0, $(this).attr('ID'));

                            })
                            .append($('<h3>')
                                .attr("style", "white-space:normal")
                                .text(vRows[p].ProductNo + ' - ' + vRows[p].ProductName)
                            )
                            .append($('<p>')
                                .attr("style", "white-space:normal")
                                .text('Brand Owner : ' + vRows[p].Organization)
                            )
                        );
                        pLV.append(pItem);
                    };

                    pLV.listview('refresh');
                }
                else {
                    $("#divPIMProductSearchNoResults").show();
                    $("#btnAddNewPIMProduct").show();
                    $("#lblPIMProductNotFound").text('The product "' + $("#txtPIMProductSearch").val() + '" does not exist. Please consider to search thoroughly using an alternate spelling before creating a new product to avoid duplication.');
                }
            };

            function CreateNewPIMProductEntry(IsNew, strID) {
                var pData = {};
                pData.IsNew = IsNew;
                pData.ID = strID;

                window.pageOrig = $.mobile.activePage;

                AjaxSend({
                    url: 'PIMProduct/setCreateNew',
                    data: pData,
                    success: function (vData) {

                        window.IsPageLoaded = false;
                        ChangePage('/PIMProduct/PIMProductInfo');
                    }
                });

            }





        });
    });