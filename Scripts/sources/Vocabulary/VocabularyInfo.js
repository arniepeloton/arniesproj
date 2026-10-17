require([
    'common'
    ], function () {

        $(function () {

            var mLoaded = false;

            $.mobile.activePage.bind('pageshow', PageShow);
            function PageShow() {
                mLoaded = true;

                if (!window.IsPageLoaded) {

                    AjaxSend({
                        url: 'Vocabulary/IsNewEntry',
                        success: function (vData) {

                            ShowVocabularyInfo(vData.VocabularyID || '')

                        }
                    });

                    window.IsPageLoaded = true;
                }
            };
            if (!mLoaded) PageShow();

            $('#btnVocabularyLanguage').click(function () {

                Browser.LibraryCountryLanguage.Click = function (e) {

                    $('#txtVocabularyLanguageID').val(e.ID)
                    $('#btnVocabularyLanguage').text(e.Language).button('refresh');

                };
                Browser.LibraryCountryLanguage.Show();

            });

            $('#btnVocabularyInfoDone').click(function (e) {
                e.preventDefault();

                if ($.trim($("#txtVocabularyLanguageID").val()) == "") {
                    MessageBox("Please select language.",
                            "Language",
                            [
                                { Text: 'Ok' }
                            ]
                    );
                    return;
                }

                if ($.trim($("#txtVocabularyWord").val()) == "") {
                    MessageBox("Please enter a word.",
                            "Word",
                            [
                                { Text: 'Ok' }
                            ]
                    );
                    return;
                }

                SaveVocabularyInfo();

            });

            function ShowVocabularyInfo(strID) {
                var pData = {};
                pData.VocabularyID = strID;

                ClearVocabularyFields();

                AjaxSend({
                    url: "Vocabulary/getVocabularyInfo",
                    data: pData,
                    success: function (vData) {

                        if (vData.VocabularyData.Rows.length > 0) {

                            $("#txtVocabularyID").val(vData.VocabularyData.Rows[0].ID || '');

                            $("#txtVocabularyLanguageID").val(vData.VocabularyData.Rows[0].LanguageID || '');
                            $("#btnVocabularyLanguage").text(vData.VocabularyData.Rows[0].Language || 'Please Select').button('refresh');

                            $("#txtVocabularyWord").val(vData.VocabularyData.Rows[0].Word || '');
                            $("#txtVocabularyDefinition").val(vData.VocabularyData.Rows[0].Definition || '');
                        }
                    }
                });

            };

            function ClearVocabularyFields() {

                $("#txtVocabularyID").val('');

                $("#txtVocabularyLanguageID").val('');
                $("#btnVocabularyLanguage").text('Please Select').button('refresh');

                $("#txtVocabularyWord").val('');
                $("#txtVocabularyDefinition").val('');
            }

            function SaveVocabularyInfo() {
                var pData = [];

                pData = {
                    VocabularyID: $.trim($("#txtVocabularyID").val()),
                    LanguageID: $.trim($("#txtVocabularyLanguageID").val()),
                    Word: $.trim($("#txtVocabularyWord").val()),
                    Definition: $.trim($("#txtVocabularyDefinition").val())
                };

                AjaxSend({
                    url: "Vocabulary/CheckLanguageUniqueWord",
                    data: pData,
                    success: function (vData) {

                        if (vData.VocabularyData.Rows.length > 0) {
                            MessageBox("Word already exist.",
                            "Word",
                            [
                                { Text: 'Ok' }
                            ]
                        );

                        }
                        else {
                            AjaxSend({
                                url: "Vocabulary/SaveVocabularyInfo",
                                data: pData,
                                success: function (vSaveData) {

                                    $("#txtVocabularyID").val(vSaveData.VocabularyData || '');

                                    $.mobile.changePage("#" + $(window.pageOrig).attr('id'));

                                }
                            });

                        }

                    }
                });

            };


        });
    });