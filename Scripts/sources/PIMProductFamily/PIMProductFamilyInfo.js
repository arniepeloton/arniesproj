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
                        url: 'PIMProductFamily/IsNewEntry',
                        success: function (vData) {

                            ShowPIMProductFamilyInfo(vData.ProductFamilyID || '')

                        }
                    });

                    window.IsPageLoaded = true;
                }
            };
            if (!mLoaded) PageShow();

            $('#btnPIMProductFamilyLabel').click(function () {

                Browser.LibraryCountryLanguageVocabularyLabel.Click = function (e) {

                    $('#txtPIMProductFamilyLabelID').val(e.ID)
                    $('#btnPIMProductFamilyLabel').text(e.Label).button('refresh');
                    $('#txtPIMProductFamilyLabelCountry').val(e.Country)
                    $('#txtPIMProductFamilyLabelLanguage').val(e.Language)

                };
                Browser.LibraryCountryLanguageVocabularyLabel.Show();

            });

            $('#btnPIMProductFamilyInfoDone').click(function (e) {
                e.preventDefault();

                if ($.trim($("#txtPIMProductFamilyLabelID").val()) == "") {
                    MessageBox("Please select product family.",
                            "Product Family",
                            [
                                { Text: 'Ok' }
                            ]
                    );
                    return;
                }

                SavePIMProductFamilyInfo();

            });

            function ShowPIMProductFamilyInfo(strID) {
                var pData = {};
                pData.ProductFamilyID = strID;

                ClearPIMProductFamilyFields();

                AjaxSend({
                    url: "PIMProductFamily/getProductFamilyInfo",
                    data: pData,
                    success: function (vData) {

                        if (vData.ProductFamilyData.Rows.length > 0) {

                            $("#txtPIMProductFamilyID").val(vData.ProductFamilyData.Rows[0].ID || '');

                            $("#txtPIMProductFamilyLabelID").val(vData.ProductFamilyData.Rows[0].ProductFamilyVocLabelID || '');
                            $("#btnPIMProductFamilyLabel").text(vData.ProductFamilyData.Rows[0].Label || 'Please Select').button('refresh');

                            $("#txtPIMProductFamilyLabelCountry").val(vData.ProductFamilyData.Rows[0].Country || '');
                            $("#txtPIMProductFamilyLabelLanguage").val(vData.ProductFamilyData.Rows[0].Language || '');
                        }
                    }
                });

            };

            function ClearPIMProductFamilyFields() {

                $("#txtPIMProductFamilyID").val('');

                $("#txtPIMProductFamilyLabelID").val('');
                $("#btnPIMProductFamilyLabel").text('Please Select').button('refresh');

                $("#txtPIMProductFamilyLabelCountry").val('');
                $("#txtPIMProductFamilyLabelLanguage").val('');
            }

            function SavePIMProductFamilyInfo() {
                var pData = [];

                pData = {
                    ProductFamilyID: $.trim($("#txtPIMProductFamilyID").val()),
                    ProductFamilyLabelID: $.trim($("#txtPIMProductFamilyLabelID").val())
                };

                AjaxSend({
                    url: "PIMProductFamily/CheckUniqueProductFamily",
                    data: pData,
                    success: function (vData) {

                        if (vData.ProductFamilyData.Rows.length > 0) {
                            MessageBox("Product family already exist.",
                                "Product Family",
                                [
                                    { Text: 'Ok' }
                                ]
                            );
                        }
                        else {
                            AjaxSend({
                                url: "PIMProductFamily/SaveProductFamilyInfo",
                                data: pData,
                                success: function (vSaveData) {

                                    $("#txtPIMProductFamilyID").val(vSaveData.ProductFamilyData || '');

                                    $.mobile.changePage("#" + $(window.pageOrig).attr('id'));

                                }
                            });

                        }

                    }
                });

            };


        });
    });