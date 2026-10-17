require([
    'common'
    ], function () {

        $(function () {

            $('#frmVocabularyBrowser').submit(function (e) {
                e.preventDefault();

                ShowVocabularyWord();

            });

            $('#btnAddNewVocabulary').click(function () {

                CreateNewVocabularyWordEntry(1, '');

            });

            function ShowVocabularyWord() {

                var pData = {};
                pData.SearchString = $("#txtVocabularySearch").val();

                AjaxSend({
                    url: 'Vocabulary/getVocabularyWordList',
                    data: pData,
                    success: function (vData) {
                        LoadVocabularyList(vData.VocabularyData.Rows);
                    }
                });

            }

            function LoadVocabularyList(vRows) {
                var pLV = $('#lstVocabulary');
                var pItem;

                $("#divVocabularySearchResults").hide();
                $("#divVocabularySearchNoResults").hide();
                $("#btnAddNewVocabulary").hide();

                if (vRows.length > 0) {
                    $("#divVocabularySearchResults").show();
                    $("#divVocabularyRecordCount").text(" Number of record(s) found: " + vRows.length);

                    pLV.html('');
                    for (var p = 0; p <= vRows.length - 1; p++) {
                        pItem = $('<li>')
                        .append($('<a>')
                            .attr('ID', vRows[p].ID)
                            .click(function (e) {
                                e.preventDefault();

                                CreateNewVocabularyWordEntry(0, $(this).attr('ID'));

                            })
                            .append($('<h3>')
                                .attr("style", "white-space:normal")
                                .text(vRows[p].Word)
                            )
                            .append($('<p>')
                                .attr("style", "white-space:normal")
                                .text('Language : ' + vRows[p].Language + ' (' + vRows[p].Country + ')')
                            )
                            .append($('<p>')
                                .attr("style", "white-space:normal")
                                .text('Definition : ' + vRows[p].Definition)
                            )
                        );
                        pLV.append(pItem);
                    };

                    pLV.listview('refresh');
                }
                else {
                    $("#divVocabularySearchNoResults").show();
                    $("#btnAddNewVocabulary").show();
                    $("#lblVocabularyNotFound").text('The word "' + $("#txtCountrySearch").val() + '" does not exist. Please consider to search thoroughly using an alternate spelling before creating a new word to avoid duplication.');
                }
            };

            function CreateNewVocabularyWordEntry(IsNew, strID) {
                var pData = {};
                pData.IsNew = IsNew;
                pData.ID = strID;

                window.pageOrig = $.mobile.activePage;

                AjaxSend({
                    url: 'Vocabulary/setCreateNew',
                    data: pData,
                    success: function (vData) {

                        window.IsPageLoaded = false;
                        ChangePage('/Vocabulary/VocabularyInfo');
                    }
                });

            }





        });
    });