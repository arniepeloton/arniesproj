require([
    'common'
    ], function () {

        $(function () {

            $('#frmPIMMeasureBrowser').submit(function (e) {
                e.preventDefault();

                ShowPIMMeasure();

            });

            $('#btnAddNewPIMMeasure').click(function () {

                CreateNewPIMMeasureEntry(1, '');

            });

            $('#btnPIMMeasureBackToHome').click(function () {

                window.open(GetPathFormat("/People/PeopleMenu"), "_self");

            }); 

            function ShowPIMMeasure() {

                var pData = {};
                pData.SearchString = $("#txtPIMMeasureSearch").val();

                AjaxSend({
                    url: 'PIMMeasure/getMeasureList',
                    data: pData,
                    success: function (vData) {
                        LoadPIMMeasureList(vData.PIMMeasureData.Rows);
                    }
                });

            }

            function LoadPIMMeasureList(vRows) {
                var pLV = $('#lstPIMMeasure');
                var pItem;

                $("#divPIMMeasureSearchResults").hide();
                $("#divPIMMeasureSearchNoResults").hide();
                $("#btnAddNewPIMMeasure").hide();

                if (vRows.length > 0) {
                    $("#divPIMMeasureSearchResults").show();
                    $("#divPIMMeasureRecordCount").text(" Number of record(s) found: " + vRows.length);

                    pLV.html('');
                    for (var p = 0; p <= vRows.length - 1; p++) {
                        pItem = $('<li>')
                        .append($('<a>')
                            .attr('ID', vRows[p].ID)
                            .click(function (e) {
                                e.preventDefault();

                                CreateNewPIMMeasureEntry(0, $(this).attr('ID'));

                            })
                            .append($('<h3>')
                                .attr("style", "white-space:normal")
                                .text(vRows[p].Measurement)
                            )
                            .append($('<p>')
                                .attr("style", "white-space:normal")
                                .text((vRows[p].Sign == "" ? "" : 'Sign : ' + vRows[p].Sign))
                            )
                        );
                        pLV.append(pItem);
                    };

                    pLV.listview('refresh');
                }
                else {
                    $("#divPIMMeasureSearchNoResults").show();
                    $("#btnAddNewPIMMeasure").show();
                    $("#lblPIMMeasureNotFound").text('The word "' + $("#txtPIMMeasureSearch").val() + '" does not exist. Please consider to search thoroughly using an alternate spelling before creating a new word to avoid duplication.');
                }
            };

            function CreateNewPIMMeasureEntry(IsNew, strID) {
                var pData = {};
                pData.IsNew = IsNew;
                pData.ID = strID;

                window.pageOrig = $.mobile.activePage;

                AjaxSend({
                    url: 'PIMMeasure/setCreateNew',
                    data: pData,
                    success: function (vData) {

                        window.IsPageLoaded = false;
                        ChangePage('/PIMMeasure/PIMMeasureInfo');
                    }
                });

            }





        });
    });