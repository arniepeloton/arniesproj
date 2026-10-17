require([
    'common'
    ], function () {

        $(function () {

            $('#frmPIMBrowser').submit(function (e) {
                e.preventDefault();

                ShowPIMProduct();

            });

            $('#btnAddNewPIMProduct').click(function () {

                CreateNewPIMEntry(1, '');

            });

            function ShowPIMProduct() {

                var pData = {};
                pData.SearchString = $("#txtPIMProductSearch").val();

                AjaxSend({
                    url: 'PIM/getPIMList',
                    data: pData,
                    success: function (vData) {
                        LoadPIMProductList(vData.PIMData.Rows);
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

                                CreateNewPIMEntry(0, $(this).attr('ID'));

                            })
                            .append($('<h3>')
                                .attr("style", "white-space:normal")
                                .text(vRows[p].CompanyName)
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

            function CreateNewPIMEntry(IsNew, strID) {
                var pData = {};
                pData.IsNew = IsNew;
                pData.ID = strID;

                window.pageOrig = $.mobile.activePage;

                AjaxSend({
                    url: 'PIM/setCreateNew',
                    data: pData,
                    success: function (vData) {

                        if ((vData.IsNew || '') == "TRUE") {
                            ChangePage('/PIM/PIMProductInfo');
                        }
                        else {
                            ChangePage('/PIM/PIMProductSummary');
                        }
                    }
                });

            }





        });
    });