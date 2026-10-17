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
                        url: 'VocabularyLabel/IsNewEntry',
                        success: function (vData) {

                            ShowVocabularyLabelInfo(vData.VocabularyLabelID || '')

                        }
                    });

                    window.IsPageLoaded = true;
                }
            };
            if (!mLoaded) PageShow();

            $('#btnVocabularyLabelLanguage').click(function () {

                Browser.LibraryCountryLanguage.Click = function (e) {

                    $('#txtVocabularyLabelLanguageID').val(e.ID)
                    $('#btnVocabularyLabelLanguage').text(e.Language)
                    .button('refresh');

                };
                Browser.LibraryCountryLanguage.Show();

            });

            $('#btnVocabularyLabelInfoDone').click(function (e) {
                e.preventDefault();

                if ($.trim($("#txtVocabularyLabelLanguageID").val()) == "") {
                    MessageBox("Please select language.",
                            "Language",
                            [
                                { Text: 'Ok' }
                            ]
                    );
                    return;
                }

                if ($.trim($("#txtVocabularyLabel").val()) == "") {
                    MessageBox("Please enter a label.",
                            "Label",
                            [
                                { Text: 'Ok' }
                            ]
                    );
                    return;
                }

                SaveVocabularyLabelInfo();

            });

            function ShowVocabularyLabelInfo(strID) {
                var pData = {};
                pData.VocabularyLabelID = strID;

                ClearVocabularyLabelFields();

                AjaxSend({
                    url: "VocabularyLabel/getVocabularyLabelInfo",
                    data: pData,
                    success: function (vData) {

                        if (vData.VocabularyLabelData.Rows.length > 0) {

                            $("#txtVocabularyLabelID").val(vData.VocabularyLabelData.Rows[0].ID || '');

                            $("#txtVocabularyLabelLanguageID").val(vData.VocabularyLabelData.Rows[0].LanguageID || '');
                            $("#btnVocabularyLabelLanguage").text(vData.VocabularyLabelData.Rows[0].Language || 'Please Select').button('refresh');

                            $("#txtVocabularyLabel").val(vData.VocabularyLabelData.Rows[0].Label || '');
                        }
                    }
                });

            };

            function ClearVocabularyLabelFields() {

                $("#txtVocabularyLabelID").val('');

                $("#txtVocabularyLabelLanguageID").val('');
                $("#btnVocabularyLabelLanguage").text('Please Select').button('refresh');

                $("#txtVocabularyLabel").val('');
            }

            function SaveVocabularyLabelInfo() {
                var pData = [];

                pData = {
                    VocabularyLabelID: $.trim($("#txtVocabularyLabelID").val()),
                    LanguageID: $.trim($("#txtVocabularyLabelLanguageID").val()),
                    Label: $.trim($("#txtVocabularyLabel").val())
                };

                AjaxSend({
                    url: "VocabularyLabel/CheckLanguageUniqueLabel",
                    data: pData,
                    success: function (vData) {

                        if (vData.VocabularyLabelData.Rows.length > 0) {
                            MessageBox("Label already exist.",
                            "Label",
                            [
                                { Text: 'Ok' }
                            ]
                        );

                        }
                        else {
                            AjaxSend({
                                url: "VocabularyLabel/SaveVocabularyLabelInfo",
                                data: pData,
                                success: function (vSaveData) {

                                    $("#txtVocabularyLabelID").val(vSaveData.VocabularyLabelData || '');

                                    $.mobile.changePage("#" + $(window.pageOrig).attr('id'));

                                }
                            });

                        }

                    }
                });

            };


        });
    });