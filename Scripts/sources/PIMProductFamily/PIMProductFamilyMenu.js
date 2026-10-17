require([
    'common'
    ], function () {

        $(function () {

            $('#frmPIMProductFamilyBrowser').submit(function (e) {
                e.preventDefault();

                ShowPIMProductFamily();

            });

            $('#btnAddNewPIMProductFamily').click(function () {

                CreateNewPIMProductFamilyEntry(1, '');

            });

            function ShowPIMProductFamily() {

                var pData = {};
                pData.SearchString = $("#txtPIMProductFamilySearch").val();

                AjaxSend({
                    url: 'PIMProductFamily/getProductFamilyList',
                    data: pData,
                    success: function (vData) {
                        LoadPIMProductFamilyList(vData.ProductFamilyData.Rows);
                    }
                });

            }

            function LoadPIMProductFamilyList(vRows) {
                var pLV = $('#lstPIMProductFamily');
                var pItem;

                $("#divPIMProductFamilySearchResults").hide();
                $("#divPIMProductFamilySearchNoResults").hide();
                $("#btnAddNewPIMProductFamily").hide();

                if (vRows.length > 0) {
                    $("#divPIMProductFamilySearchResults").show();
                    $("#divPIMProductFamilyRecordCount").text(" Number of record(s) found: " + vRows.length);

                    pLV.html('');
                    for (var p = 0; p <= vRows.length - 1; p++) {
                        pItem = $('<li>')
                        .append($('<a>')
                            .attr('ID', vRows[p].ID)
                            .click(function (e) {
                                e.preventDefault();

                                CreateNewPIMProductFamilyEntry(0, $(this).attr('ID'));

                            })
                            .append($('<h3>')
                                .attr("style", "white-space:normal")
                                .text(vRows[p].Label)
                            )
                            .append($('<p>')
                                .attr("style", "white-space:normal")
                                .text('Language : ' + vRows[p].Language + ' (' + vRows[p].Country + ')')
                            )
                        );
                        pLV.append(pItem);
                    };

                    pLV.listview('refresh');
                }
                else {
                    $("#divPIMProductFamilySearchNoResults").show();
                    $("#btnAddNewPIMProductFamily").show();
                    $("#lblPIMProductFamilyNotFound").text('The product family "' + $("#txtPIMProductFamilySearch").val() + '" does not exist. Please consider to search thoroughly using an alternate spelling before creating a new product family to avoid duplication.');
                }
            };

            function CreateNewPIMProductFamilyEntry(IsNew, strID) {
                var pData = {};
                pData.IsNew = IsNew;
                pData.ID = strID;

                window.pageOrig = $.mobile.activePage;

                AjaxSend({
                    url: 'PIMProductFamily/setCreateNew',
                    data: pData,
                    success: function (vData) {

                        window.IsPageLoaded = false;
                        ChangePage('/PIMProductFamily/PIMProductFamilyInfo');
                    }
                });

            }





        });
    });