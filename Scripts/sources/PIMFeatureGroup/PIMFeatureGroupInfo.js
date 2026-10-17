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
                        url: 'PIMFeatureGroup/IsNewEntry',
                        success: function (vData) {

                            ShowPIMFeatureGroupInfo(vData.PIMFeatureGroupID || '')

                        }
                    });

                    window.IsPageLoaded = true;
                }
            };
            if (!mLoaded) PageShow();

            $('#btnPIMFeatureGroupLabel').click(function () {

                Browser.LibraryCountryLanguageVocabularyLabel.Click = function (e) {

                    $('#txtPIMFeatureGroupLabelID').val(e.ID)
                    $('#btnPIMFeatureGroupLabel').text(e.Label).button('refresh');
                    $('#txtPIMFeatureGroupLabelCountry').val(e.Country)
                    $('#txtPIMFeatureGroupLabelLanguage').val(e.Language)

                };
                Browser.LibraryCountryLanguageVocabularyLabel.Show();

            });

            $('#btnPIMFeatureGroupInfoDone').click(function (e) {
                e.preventDefault();

                if ($.trim($("#txtPIMFeatureGroupLabelID").val()) == "") {
                    MessageBox("Please select label.",
                            "Label",
                            [
                                { Text: 'Ok' }
                            ]
                    );
                    return;
                }

                SavePIMFeatureGroupInfo();

            });

            function ShowPIMFeatureGroupInfo(strID) {
                var pData = {};
                pData.FeatureGroupID = strID;

                ClearPIMFeatureGroupFields();

                AjaxSend({
                    url: "PIMFeatureGroup/getFeatureGroupInfo",
                    data: pData,
                    success: function (vData) {

                        if (vData.FeatureGroupData.Rows.length > 0) {

                            $("#txtPIMFeatureGroupID").val(vData.FeatureGroupData.Rows[0].ID || '');

                            $("#txtPIMFeatureGroupLabelID").val(vData.FeatureGroupData.Rows[0].FeatureGroupVocLabelID || '');
                            $("#btnPIMFeatureGroupLabel").text(vData.FeatureGroupData.Rows[0].Label || 'Please Select').button('refresh');

                            $("#txtPIMFeatureGroupLabelCountry").val(vData.FeatureGroupData.Rows[0].Country || '');
                            $("#txtPIMFeatureGroupLabelLanguage").val(vData.FeatureGroupData.Rows[0].Language || '');
                        }
                    }
                });

            };

            function ClearPIMFeatureGroupFields() {

                $("#txtPIMFeatureGroupID").val('');

                $("#txtPIMFeatureGroupLabelID").val('');
                $("#btnPIMFeatureGroupLabel").text('Please Select').button('refresh');

                $("#txtPIMFeatureGroupLabelCountry").val('');
                $("#txtPIMFeatureGroupLabelLanguage").val('');
            }

            function SavePIMFeatureGroupInfo() {
                var pData = [];

                pData = {
                    FeatureGroupID: $.trim($("#txtPIMFeatureGroupID").val()),
                    FeatureGroupLabelID: $.trim($("#txtPIMFeatureGroupLabelID").val())
                };

                AjaxSend({
                    url: "PIMFeatureGroup/CheckUniqueFeatureGroup",
                    data: pData,
                    success: function (vData) {

                        if (vData.FeatureGroupData.Rows.length > 0) {
                            MessageBox("Feature group already exist.",
                                "Feature Group",
                                [
                                    { Text: 'Ok' }
                                ]
                            );

                        }
                        else {
                            AjaxSend({
                                url: "PIMFeatureGroup/SaveFeatureGroupInfo",
                                data: pData,
                                success: function (vSaveData) {

                                    $("#txtPIMFeatureGroupID").val(vSaveData.FeatureGroupData || '');

                                    $.mobile.changePage("#" + $(window.pageOrig).attr('id'));

                                }
                            });

                        }

                    }
                });

            };


        });
    });