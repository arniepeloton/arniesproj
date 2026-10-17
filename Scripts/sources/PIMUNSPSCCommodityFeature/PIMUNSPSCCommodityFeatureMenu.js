require([
    'common'
    ], function () {

        $(function () {

            $('#frmPIMUNSPSCCommodityFeatureBrowser').submit(function (e) {
                e.preventDefault();

                ShowPIMUNSPSCCommodityFeature();

            });

            $('#btnAddNewPIMUNSPSCCommodityFeature').click(function () {

                CreateNewPIMUNSPSCCommFeatureEntry(1, '');

            });

            $('#btnPIMUNSPSCCommodityFeatureBackToHome').click(function () {

                window.open(GetPathFormat("/People/PeopleMenu"), "_self");

            }); 

            function ShowPIMUNSPSCCommodityFeature() {

                var pData = {};
                pData.SearchString = $("#txtPIMUNSPSCCommodityFeatureSearch").val();

                AjaxSend({
                    url: 'PIMUNSPSCCommodityFeature/getUNSPSCCommodityList',
                    data: pData,
                    success: function (vData) {
                        LoadPIMUNSPSCCommFeatureList(vData.PIMUNSPSCCommFeatureData.Rows);
                    }
                });

            }

            function LoadPIMUNSPSCCommFeatureList(vRows) {
                var pLV = $('#lstPIMUNSPSCCommodityFeature');
                var pItem;

                $("#divPIMUNSPSCCommodityFeatureSearchResults").hide();
                $("#divPIMUNSPSCCommodityFeatureSearchNoResults").hide();
                $("#btnAddNewPIMUNSPSCCommodityFeature").hide();

                if (vRows.length > 0) {
                    $("#divPIMUNSPSCCommodityFeatureSearchResults").show();
                    $("#divPIMUNSPSCCommodityFeatureRecordCount").text(" Number of record(s) found: " + vRows.length);

                    pLV.html('');
                    for (var p = 0; p <= vRows.length - 1; p++) {
                        pItem = $('<li>')
                        .append($('<a>')
                            .attr('ID', vRows[p].ID)
                            .click(function (e) {
                                e.preventDefault();

                                CreateNewPIMUNSPSCCommFeatureEntry(0, $(this).attr('ID'));

                            })
                            .append($('<h3>')
                                .attr("style", "white-space:normal")
                                .text(vRows[p].Commodity)
                            )
                            .append($('<p>')
                                .attr("style", "white-space:normal")
                                .text('Segment : ' + vRows[p].Segment)
                            )
                            .append($('<p>')
                                .attr("style", "white-space:normal")
                                .text('Family : ' + vRows[p].Family)
                            )
                            .append($('<p>')
                                .attr("style", "white-space:normal")
                                .text('Class: ' + vRows[p].Class)
                            )
                        );
                        pLV.append(pItem);
                    };

                    pLV.listview('refresh');
                }
                else {
                    $("#divPIMUNSPSCCommodityFeatureSearchNoResults").show();
//                    $("#btnAddNewPIMUNSPSCCommodityFeature").show();
                    $("#lstPIMUNSPSCCommodityFeature").text('The UNSPSC commodity "' + $("#txtPIMUNSPSCCommodityFeatureSearch").val() + '" does not exist. Please consider to search thoroughly using an alternate spelling before creating a new unspsc commodity to avoid duplication.');
                }
            };

            function CreateNewPIMUNSPSCCommFeatureEntry(IsNew, strID) {
                var pData = {};
                pData.IsNew = IsNew;
                pData.ID = strID;

                window.pageOrig = $.mobile.activePage;

                AjaxSend({
                    url: 'PIMUNSPSCCommodityFeature/setCreateNew',
                    data: pData,
                    success: function (vData) {

                        window.IsPageLoaded = false;
                        ChangePage('/PIMUNSPSCCommodityFeature/PIMUNSPSCCommodityFeatureInfo');
                    }
                });

            }





        });
    });