require([
    'common',
    ], function () {
        var ID = '';
        var IsAllowDecimal = 0;
        var DecimalMode = 0;

        $(function () {
            GetDecimalSettings();
            GetAllFeatures();

            window.Decimals = {
                GetDecimalsData: function () {
                    GetDecimalSettings();
                    GetAllFeatures();
                }
            }

            $('body').on('click', '#btnSaveDecimalSettings', function () {
                SaveFeaturesWithDecimal();
            })
            .on('change', '#selSlider', function () {
                if ($('#selSlider').is(':checked')) {
                    $('#dvIsAllowDecimal').css('visibility', 'visible');
                    $('#dvIsAllowDecimal').css('display', 'block');
                }
                else {
                    $('#dvIsAllowDecimal').css('visibility', 'hidden');
                    $('#dvIsAllowDecimal').css('display', 'none');
                }
            })
            .on('change', 'input[name="rcDecimals"]', function () {
                if ($(this).val() == "choice-1") {
                    DecimalMode = 0;
                    $('#dvIsAllowOnSelectedCategories').css('visibility', 'hidden');
                    $('#dvIsAllowOnSelectedCategories').css('display', 'none');
                }
                else {
                    DecimalMode = 1;
                    $('#dvIsAllowOnSelectedCategories').css('visibility', 'visible');
                    $('#dvIsAllowOnSelectedCategories').css('display', 'block');
                }
            })
            .on('click', '#btnCancelDecimalSettings', function () {
                window.location.hash = 'SystemAdministration';
            })

            //            $('#rcDoNotAllow').click(function () {
            //                $('#dvIsAllowOnSelectedCategories').css('visibility', 'hidden');
            //                $('#dvIsAllowOnSelectedCategories').css('display', 'none');
            //            });
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

                $('#selSlider').attr('checked', 1);
                $('#dvIsAllowDecimal').css('visibility', 'visible');
                $('#dvIsAllowDecimal').css('display', 'block');

                if (parseInt(vRows[0].AllowDecimalMode) == 0) {
                    $("#rcAllowAllDecimals").prop("checked", true)//.checkboxradio("refresh");
                    $("#rcAllowOnSelected").prop("unchecked", true)//.checkboxradio("refresh");

                    $('#dvIsAllowOnSelectedCategories').css('visibility', 'hidden');
                    $('#dvIsAllowOnSelectedCategories').css('display', 'none');

                    DecimalMode = 0;
                }
                else {
                    $("#rcAllowOnSelected").prop("checked", true)//.checkboxradio("refresh");
                    $("#rcAllowAllDecimals").prop("unchecked", true)//.checkboxradio("refresh");

                    $('#dvIsAllowOnSelectedCategories').css('visibility', 'visible');
                    $('#dvIsAllowOnSelectedCategories').css('display', 'block');

                    DecimalMode = 1;
                }
            }
            else {

                IsAllowDecimal = 0;

                $('#selSlider').removeAttr('checked');
                $('#dvIsAllowDecimal').css('visibility', 'hidden');
                $('#dvIsAllowDecimal').css('display', 'none');
            }

            //try {
            //    $('#selSlider').slider('refresh');
            //}
            //catch (e) {
            //    $('#selSlider').slider();
            //}
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
                            .append($('<div class="form-check form-switch" style="font-size:20px;">')                                    
                                .attr('data', vRows[i].Label)
                                .attr('data-role', 'slider')
                                .append($('<input class="form-check-input" data-role="none" type="checkbox">')
                                    .attr('data', vRows[i].Label)
                                    .attr('ID', i)
                                )
                            )
                        )

                pLV.append(pItem);

                try {
                    if (IsAllow == 1) {
                        $('#' + i).attr('checked', 1)//.slider('refresh');
                    }
                    else {
                        //$('#' + i).slider('refresh');
                    }

                }
                catch (e) {
                    //$('#' + i).slider();
                }
            }

            try {
                //$('#lstCategories').listview('refresh');
            }
            catch (e) {
                //$('#lstCategories').listview();
            }
        }

        function SaveFeaturesWithDecimal() {
            var FeatureID = '';

            if ($('#selSlider').is(':checked')) {

                IsAllowDecimal = 1;

                $('#tbCategories tr td input').each(function (m_index) {
                    if ($('#' + $(this).attr("ID")).is(':checked')) {
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
                        //history.back();
                        WebToast('Successfully saved', 'top');
                        window.location.hash = 'SystemAdministration';
                    }
                    else {
                        NewMessageBox('An error occured while saving decimal settings.', 'Decimal Settings');
                    }
                }
            });
        }
    });