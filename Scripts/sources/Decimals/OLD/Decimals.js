require([
    'common',
    ], function () {
        var ID = '';
        var IsAllowDecimal = 0;
        var DecimalMode = 0;

        $(function () {
            GetDecimalSettings();
            GetAllFeatures();

            $('#selSlider').change(function () {
                if ($('#selSlider :selected').val() == 'optAllowYes') {
                    $('#dvIsAllowDecimal').css('visibility', 'visible');
                    $('#dvIsAllowDecimal').css('display', 'block');
                }
                else {
                    $('#dvIsAllowDecimal').css('visibility', 'hidden');
                    $('#dvIsAllowDecimal').css('display', 'none');
                }
            });

            $('#rcAllowAllDecimals').click(function () {
                DecimalMode = 0;
                $('#dvIsAllowOnSelectedCategories').css('visibility', 'hidden');
                $('#dvIsAllowOnSelectedCategories').css('display', 'none');
            });

            $('#rcAllowOnSelected').click(function () {
                DecimalMode = 1;
                $('#dvIsAllowOnSelectedCategories').css('visibility', 'visible');
                $('#dvIsAllowOnSelectedCategories').css('display', 'block');
            });

            //            $('#rcDoNotAllow').click(function () {
            //                $('#dvIsAllowOnSelectedCategories').css('visibility', 'hidden');
            //                $('#dvIsAllowOnSelectedCategories').css('display', 'none');
            //            });

            $('#btnSaveDecimalSettings').click(function () {
                SaveFeaturesWithDecimal();
            });
        });

        function GetDecimalSettings() {
            AjaxSend({
                url: 'Decimals/GetDecimalSettings',
                data: { OrgID: sessionStorage.OrgID },
                success: function (vData) {
                    if (vData.Results.Rows.length > 0) {
                        DisplayGetDecimalSettings(vData.Results.Rows);
                    }
                }
            });
        }

        function DisplayGetDecimalSettings(vRows) {
            ID = vRows[0].ID;

            if (parseInt(vRows[0].IsAllowDecimal) == 1) {

                IsAllowDecimal = 1;

                $('#selSlider').val('optAllowYes');
                $('#dvIsAllowDecimal').css('visibility', 'visible');
                $('#dvIsAllowDecimal').css('display', 'block');

                if (parseInt(vRows[0].AllowDecimalMode) == 0) {
                    $("#rcAllowAllDecimals").prop("checked", true).checkboxradio("refresh");
                    $("#rcAllowOnSelected").prop("unchecked", true).checkboxradio("refresh");

                    $('#dvIsAllowOnSelectedCategories').css('visibility', 'hidden');
                    $('#dvIsAllowOnSelectedCategories').css('display', 'none');

                    DecimalMode = 0;
                }
                else {
                    $("#rcAllowOnSelected").prop("checked", true).checkboxradio("refresh");
                    $("#rcAllowAllDecimals").prop("unchecked", true).checkboxradio("refresh");

                    $('#dvIsAllowOnSelectedCategories').css('visibility', 'visible');
                    $('#dvIsAllowOnSelectedCategories').css('display', 'block');

                    DecimalMode = 1;
                }
            }
            else {

                IsAllowDecimal = 0;

                $('#selSlider').val('optAllowNo');
                $('#dvIsAllowDecimal').css('visibility', 'hidden');
                $('#dvIsAllowDecimal').css('display', 'none');
            }

            try {
                $('#selSlider').slider('refresh');
            }
            catch (e) {
                $('#selSlider').slider();
            }
        }

        function GetAllFeatures() {
            AjaxSend({
                url: 'Decimals/GetAllProductFeatures',
                data: { OrgID: sessionStorage.OrgID },
                success: function (vData) {
                    DisplayProductFeatures(vData.Results.Rows);
                }
            });
        }

        function DisplayProductFeatures(vRows) {
            var pLV = $('#tbCategories');
            var IsSelected = false;

            for (i = 0; i <= vRows.length - 1; i++) {
                IsSelected = false;

                var IsAllow = vRows[i].IsAllowDecimal;

                pItem = $('<tr>')
                            .append($('<td>')
                                .text(vRows[i].Label)
                            )
                            .append($('<td>')
                                .css('text-align', 'center')
                                .append($('<select>')
                                    .attr('ID', i)
                                    .attr('data', vRows[i].Label)
                                    .attr('data-role', 'slider')
                                    .append($('<option>')
                                        .attr('value', 'No')
                                        .text('No')
                                    )
                                    .append($('<option>')
                                        .attr('value', 'Yes')
                                        .text('Yes')
                                    )
                                )
                            )


                pLV.append(pItem);

                try {
                    if (IsAllow==1) {
                        $('#' + i).val('Yes').slider('refresh');
                    }
                    else {
                        $('#' + i).slider('refresh');
                    }

                }
                catch (e) {
                    $('#' + i).slider();
                }
            }

            try {
                $('#lstCategories').listview('refresh');
            }
            catch (e) {
                $('#lstCategories').listview();
            }
        }

        function SaveFeaturesWithDecimal() {
            var FeatureID = '';

            if ($('#selSlider :selected').val() == 'optAllowYes') {

                IsAllowDecimal = 1;

                $('#tbCategories tr td select').each(function (m_index) {
                    if ($('#' + $(this).attr("ID") + ' :selected').val() == 'Yes') {
                        FeatureID = FeatureID + $(this).attr('data') + '|';
                    }
                });
            }
            else {
                IsAllowDecimal = 0;
            }

            data = { OrgID: sessionStorage.OrgID,
                ID: ID,
                IsAllowDecimal: IsAllowDecimal,
                DecimalMode: DecimalMode,
                FeatureID: FeatureID
            };

            AjaxSend({
                url: 'Decimals/SaveFeaturesWithDecimal',
                data: data,
                success: function (vData) {
                    if (vData.Results == "True") {
                        history.back();
                    }
                    else {
                        MessageBox('An error occured while saving decimal settings.', 'Decimal Settings');
                    }
                }
            });
        }
    });