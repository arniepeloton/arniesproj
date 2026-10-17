require([
    'common'
    ], function () {

        $(function () {

            $('#frmVocabularyLabelBrowser').submit(function (e) {
                e.preventDefault();

                ShowVocabularyLabel();

            });

            $('#btnAddNewVocabularyLabel').click(function () {

                CreateNewVocabularyLabelEntry(1, '');

            });

            $('#btnVocabularyLabelBackToHome').click(function () {

                window.open(GetPathFormat("/People/PeopleMenu"), "_self");

            }); 

            function ShowVocabularyLabel() {

                var pData = {};
                pData.SearchString = $("#txtVocabularyLabelSearch").val();

                AjaxSend({
                    url: 'VocabularyLabel/getVocabularyLabelList',
                    data: pData,
                    success: function (vData) {
                        LoadVocabularyLabelList(vData.VocabularyLabelData.Rows);
                    }
                });

            }

            function LoadVocabularyLabelList(vRows) {
                var pLV = $('#lstVocabularyLabel');
                var pItem;

                $("#divVocabularyLabelSearchResults").hide();
                $("#divVocabularyLabelSearchNoResults").hide();
                $("#btnAddNewVocabularyLabel").hide();

                if (vRows.length > 0) {
                    $("#divVocabularyLabelSearchResults").show();
                    $("#divVocabularyLabelRecordCount").text(" Number of record(s) found: " + vRows.length);

                    pLV.html('');
                    for (var p = 0; p <= vRows.length - 1; p++) {
                        pItem = $('<li>')
                        .append($('<a>')
                            .attr('ID', vRows[p].ID)
                            .click(function (e) {
                                e.preventDefault();

                                CreateNewVocabularyLabelEntry(0, $(this).attr('ID'));

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
                    $("#divVocabularyLabelSearchNoResults").show();
                    $("#btnAddNewVocabularyLabel").show();
                    $("#lblVocabularyLabelNotFound").text('The label "' + $("#txtVocabularyLabelSearch").val() + '" does not exist. Please consider to search thoroughly using an alternate spelling before creating a new label to avoid duplication.');
                }
            };

            function CreateNewVocabularyLabelEntry(IsNew, strID) {
                var pData = {};
                pData.IsNew = IsNew;
                pData.ID = strID;

                window.pageOrig = $.mobile.activePage;

                AjaxSend({
                    url: 'VocabularyLabel/setCreateNew',
                    data: pData,
                    success: function (vData) {

                        window.IsPageLoaded = false;
                        ChangePage('/VocabularyLabel/VocabularyLabelInfo');
                    }
                });

            }





        });
    });