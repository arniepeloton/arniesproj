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
                        url: 'PIMMeasure/IsNewEntry',
                        success: function (vData) {

                            ShowPIMMeasureInfo(vData.PIMMeasureID || '')

                        }
                    });

                    window.IsPageLoaded = true;
                }
            };
            if (!mLoaded) PageShow();

            $('#btnPIMMeasureName').click(function () {

                Browser.LibraryCountryLanguageVocabularyLabel.Click = function (e) {

                    $('#txtPIMMeasureNameID').val(e.ID)
                    $('#btnPIMMeasureName').text(e.Label).button('refresh');

                };
                Browser.LibraryCountryLanguageVocabularyLabel.Show();


            });

            $('#btnPIMMeasureSign').click(function () {

                Browser.LibraryPIMMeasureSign.Click = function (e) {

                    $('#txtPIMMeasureSignID').val(e.ID)
                    $('#btnPIMMeasureSign').text(e.Sign).button('refresh');

                };
                Browser.LibraryPIMMeasureSign.Show();


            });

            $('#btnPIMMeasureInfoDone').click(function (e) {
                e.preventDefault();

                if ($.trim($("#txtPIMMeasureNameID").val()) == "") {
                    MessageBox("Please select a measurement.",
                            "Measurement",
                            [
                                { Text: 'Ok' }
                            ]
                    );
                    return;
                }

                SavePIMMeasureInfo();

            });

            function ShowPIMMeasureInfo(strID) {
                var pData = {};
                pData.PIMMeasureID = strID;

                ClearPIMMeasureFields();

                AjaxSend({
                    url: "PIMMeasure/getMeasureInfo",
                    data: pData,
                    success: function (vData) {

                        if (vData.PIMMeasureData.Rows.length > 0) {

                            $("#txtPIMMeasureID").val(vData.PIMMeasureData.Rows[0].ID || '');

                            $("#txtPIMMeasureNameID").val(vData.PIMMeasureData.Rows[0].MeasureNameID || '');
                            $("#btnPIMMeasureName").text(vData.PIMMeasureData.Rows[0].Measurement || 'Please Select').button('refresh');

                            $("#txtPIMMeasureSignID").val(vData.PIMMeasureData.Rows[0].SignID || '');
                            $("#btnPIMMeasureSign").text(vData.PIMMeasureData.Rows[0].Sign || 'Please Select').button('refresh');

                        }
                    }
                });

            };

            function ClearPIMMeasureFields() {

                $("#txtPIMMeasureID").val('');

                $("#txtPIMMeasureNameID").val('');
                $("#btnPIMMeasureName").text('Please Select').button('refresh');

                $("#txtPIMMeasureSignID").val('');
                $("#btnPIMMeasureSign").text('Please Select').button('refresh');
            }

            function SavePIMMeasureInfo() {
                var pData = [];

                pData = {
                    PIMMeasureID: $.trim($("#txtPIMMeasureID").val()),
                    MeasureNameID: $.trim($("#txtPIMMeasureNameID").val()),
                    MeasureSignID: $.trim($("#txtPIMMeasureSignID").val())
                };

                AjaxSend({
                    url: "PIMMeasure/CheckLanguageUniqueMeasure",
                    data: pData,
                    success: function (vData) {

                        if (vData.PIMMeasureData.Rows.length > 0) {
                            MessageBox("Measurement already exist.",
                            "Measurement",
                            [
                                { Text: 'Ok' }
                            ]
                        );

                        }
                        else {
                            AjaxSend({
                                url: "PIMMeasure/SaveMeasureInfo",
                                data: pData,
                                success: function (vSaveData) {

                                    $("#txtPIMMeasureID").val(vSaveData.PIMMeasureData || '');

                                    $.mobile.changePage("#" + $(window.pageOrig).attr('id'));

                                }
                            });

                        }

                    }
                });

            };


        });
    });