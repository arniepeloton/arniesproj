require([
    'common'
    ], function () {

        $(function () {

            $('#frmPIMFeatureGroupBrowser').submit(function (e) {
                e.preventDefault();

                ShowPIMFeatureGroup();

            });

            $('#btnAddNewPIMFeatureGroup').click(function () {

                CreateNewPIMFeatureGroupEntry(1, '');

            });

            $('#btnPIMFeatureGroupBackToHome').click(function () {

                window.open(GetPathFormat("/People/PeopleMenu"), "_self");

            }); 

            function ShowPIMFeatureGroup() {

                var pData = {};
                pData.SearchString = $("#txtPIMFeatureGroupSearch").val();

                AjaxSend({
                    url: 'PIMFeatureGroup/getFeatureGroupList',
                    data: pData,
                    success: function (vData) {
                        LoadPIMFeatureGroupList(vData.FeatureGroupData.Rows);
                    }
                });

            }

            function LoadPIMFeatureGroupList(vRows) {
                var pLV = $('#lstPIMFeatureGroup');
                var pItem;

                $("#divPIMFeatureGroupSearchResults").hide();
                $("#divPIMFeatureGroupSearchNoResults").hide();
                $("#btnAddNewPIMFeatureGroup").hide();

                if (vRows.length > 0) {
                    $("#divPIMFeatureGroupSearchResults").show();
                    $("#divPIMFeatureGroupRecordCount").text(" Number of record(s) found: " + vRows.length);

                    pLV.html('');
                    for (var p = 0; p <= vRows.length - 1; p++) {
                        pItem = $('<li>')
                        .append($('<a>')
                            .attr('ID', vRows[p].ID)
                            .click(function (e) {
                                e.preventDefault();

                                CreateNewPIMFeatureGroupEntry(0, $(this).attr('ID'));

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
                    $("#divPIMFeatureGroupSearchNoResults").show();
                    $("#btnAddNewPIMFeatureGroup").show();
                    $("#lblPIMFeatureGroupNotFound").text('The label "' + $("#txtPIMFeatureGroupSearch").val() + '" does not exist. Please consider to search thoroughly using an alternate spelling before creating a new label to avoid duplication.');
                }
            };

            function CreateNewPIMFeatureGroupEntry(IsNew, strID) {
                var pData = {};
                pData.IsNew = IsNew;
                pData.ID = strID;

                window.pageOrig = $.mobile.activePage;

                AjaxSend({
                    url: 'PIMFeatureGroup/setCreateNew',
                    data: pData,
                    success: function (vData) {

                        window.IsPageLoaded = false;
                        ChangePage('/PIMFeatureGroup/PIMFeatureGroupInfo');
                    }
                });

            }





        });
    });