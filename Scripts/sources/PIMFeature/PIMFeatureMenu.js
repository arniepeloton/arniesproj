require([
    'common'
    ], function () {

        $(function () {

            $('#frmPIMFeatureBrowser').submit(function (e) {
                e.preventDefault();

                ShowPIMFeature();

            });

            $('#btnAddNewPIMFeature').click(function () {

                CreateNewPIMFeatureEntry(1, '');

            });

            $('#btnPIMFeatureBackToHome').click(function () {

                window.open(GetPathFormat("/People/PeopleMenu"), "_self");

            }); 

            function ShowPIMFeature() {

                var pData = {};
                pData.SearchString = $("#txtPIMFeatureSearch").val();

                AjaxSend({
                    url: 'PIMFeature/getFeatureList',
                    data: pData,
                    success: function (vData) {
                        LoadPIMFeatureList(vData.PIMFeatureData.Rows);
                    }
                });

            }

            function LoadPIMFeatureList(vRows) {
                var pLV = $('#lstPIMFeature');
                var pItem;

                $("#divPIMFeatureSearchResults").hide();
                $("#divPIMFeatureSearchNoResults").hide();
                $("#btnAddNewPIMFeature").hide();

                if (vRows.length > 0) {
                    $("#divPIMFeatureSearchResults").show();
                    $("#divPIMFeatureRecordCount").text(" Number of record(s) found: " + vRows.length);

                    pLV.html('');
                    for (var p = 0; p <= vRows.length - 1; p++) {
                        pItem = $('<li>')
                        .append($('<a>')
                            .attr('ID', vRows[p].ID)
                            .click(function (e) {
                                e.preventDefault();

                                CreateNewPIMFeatureEntry(0, $(this).attr('ID'));

                            })
                            .append($('<h3>')
                                .attr("style", "white-space:normal")
                                .text(vRows[p].Feature)
                            )
                        );
                        pLV.append(pItem);
                    };

                    pLV.listview('refresh');
                }
                else {
                    $("#divPIMFeatureSearchNoResults").show();
                    $("#btnAddNewPIMFeature").show();
                    $("#lblPIMFeatureNotFound").text('The feature "' + $("#txtPIMFeatureSearch").val() + '" does not exist. Please consider to search thoroughly using an alternate spelling before creating a new feature to avoid duplication.');
                }
            };

            function CreateNewPIMFeatureEntry(IsNew, strID) {
                var pData = {};
                pData.IsNew = IsNew;
                pData.ID = strID;

                window.pageOrig = $.mobile.activePage;

                AjaxSend({
                    url: 'PIMFeature/setCreateNew',
                    data: pData,
                    success: function (vData) {

                        window.IsPageLoaded = false;
                        ChangePage('/PIMFeature/PIMFeatureInfo');
                    }
                });

            }





        });
    });