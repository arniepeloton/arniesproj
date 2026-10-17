require([
    'common'
    ], function () {

        $(function () {

            var IsNewFeatureOption = true;
            var arFeatureOption = new Array();
            var intFeatureOptionCntr = 0;
            var intFeatureOptionIndex = 0;

            var intFeatureOptionRecID = 0;
            var intFeatureOption = 1;
            var intFeatureOptionState = 2;

            var mLoaded = false;

            $.mobile.activePage.bind('pageshow', PageShow);
            function PageShow() {
                mLoaded = true;

                if (!window.IsPageLoaded) {

                    AjaxSend({
                        url: 'PIMFeature/IsNewEntry',
                        success: function (vData) {

                            ShowPIMFeatureInfo(vData.PIMFeatureID || '')

                        }
                    });

                    window.IsPageLoaded = true;
                }
            };
            if (!mLoaded) PageShow();

            $('#btnPIMFeatureLabel').click(function () {

                Browser.LibraryCountryLanguageVocabularyLabel.Click = function (e) {

                    $('#txtPIMFeatureLabelID').val(e.ID)
                    $('#btnPIMFeatureLabel').text(e.Label).button('refresh');

                };
                Browser.LibraryCountryLanguageVocabularyLabel.Show();


            });

            $('#btnPIMFeatureName').click(function () {

                Browser.LibraryCountryLanguageVocabularyLabel.Click = function (e) {

                    $('#txtPIMFeatureNameID').val(e.ID)
                    $('#btnPIMFeatureName').text(e.Label).button('refresh');

                };
                Browser.LibraryCountryLanguageVocabularyLabel.Show();


            });

            $('#btnPIMFeatureMeasure').click(function () {

                Browser.LibraryPIMMeasure.Click = function (e) {

                    $('#txtPIMFeatureMeasureID').val(e.ID)
                    $('#btnPIMFeatureMeasure').text(e.Measurement).button('refresh');
                    $('#txtPIMFeatureMeasureSign').val(e.Sign)

                };
                Browser.LibraryPIMMeasure.Show();


            });

            $('#selPIMFeatureType').change(function () {

                if ($(this).val() == "Multiple Option") {
                    $('#divPIMFeatureStructure').show();
                } else {
                    $('#divPIMFeatureStructure').hide();
                }

            });

            $('#selShowMeasurement').change(function (e) {
                e.preventDefault();

                if ($.trim($(this).val()) == "1") {
                    $("#divFeatureMeasurement").show();
                }
                else {
                    $("#divFeatureMeasurement").hide();
                }

            });

            $('#btnAddPIMFeatureOption').click(function (e) {
                e.preventDefault();

                IsNewFeatureOption = true
                intFeatureOptionCntr = intFeatureOptionCntr + 1;
                intFeatureOptionIndex = intFeatureOptionCntr;

                $.mobile.changePage('#pageFeatureOption');
                ClearFeatureOptionFields();

            });

            $('#btnPIMFeatureOptionSave').click(function (e) {
                e.preventDefault();

                if ($.trim($("#txtPIMFeatureOption").val()) == "") {
                    MessageBox("Please enter feature option.",
                           "Feature Option",
                            [
                                { Text: 'Ok'
                                }
                            ]
                );
                    return;
                }

                SaveFeatureOption();

            });

            $('#btnPIMFeatureOptionRemove').click(function (e) {
                e.preventDefault();

                MessageBox("Remove this record?",
                'Remove',
                [
                    { Text: 'Yes',
                        Click: function () {
                            RemoveFeatureOption();
                        }
                    }
                ,
                    { Text: 'No'
                    }
                ]
            );

            });

            $('#btnPIMFeatureOptionCancel').click(function (e) {
                e.preventDefault();

                ChangePage('/PIMFeature/PIMFeatureInfo');

            });

            $('#btnPIMFeatureInfoDone').click(function (e) {
                e.preventDefault();

                if ($.trim($("#txtPIMFeatureLabelID").val()) == "") {
                    MessageBox("Please select label.",
                            "Label",
                            [
                                { Text: 'Ok' }
                            ]
                    );
                    return;
                }

                if ($.trim($("#txtPIMFeatureNameID").val()) == "") {
                    MessageBox("Please select a feature name.",
                            "Feature",
                            [
                                { Text: 'Ok' }
                            ]
                    );
                    return;
                }

                if ($.trim($("#selPIMFeatureType").val()) == "") {
                    MessageBox("Please select feature type.",
                            "Feature Type",
                            [
                                { Text: 'Ok' }
                            ]
                    );
                    return;
                } else {
                    if ($.trim($("#selPIMFeatureType").val()) == "Multiple Option") {
                        if ($.trim($("#selPIMFeatureDisplayFormat").val()) == "") {
                            MessageBox("Please select display format.",
                                    "Display Format",
                                    [
                                        { Text: 'Ok' }
                                    ]
                            );
                            return;
                        }

                        if (intFeatureOptionCntr <= 0) {
                            MessageBox("Please enter feature options.",
                                    "Feature Options",
                                    [
                                        { Text: 'Ok' }
                                    ]
                            );
                            return;
                        }

                    }
                }

                if ($.trim($("#selShowMeasurement").val()) == "1") {
                    if ($.trim($("#txtPIMFeatureMeasureID").val()) == "") {
                        MessageBox("Please select a measurement.",
                                "Measurement",
                                [
                                    { Text: 'Ok' }
                                ]
                        );
                        return;
                    }
                }

                SavePIMFeatureInfo();

            });

            function ShowPIMFeatureInfo(strID) {
                var pData = {};
                pData.PIMFeatureID = strID;

                ClearPIMFeatureFields();

                AjaxSend({
                    url: "PIMFeature/getFeatureInfo",
                    data: pData,
                    success: function (vData) {

                        if (vData.PIMFeatureData.Rows.length > 0) {

                            $("#txtPIMFeatureID").val(vData.PIMFeatureData.Rows[0].ID || '');

                            $("#txtPIMFeatureLabelID").val(vData.PIMFeatureData.Rows[0].FeatureLabelID || '');
                            $("#btnPIMFeatureLabel").text(vData.PIMFeatureData.Rows[0].FeatureLabel || 'Please Select').button('refresh');

                            $("#txtPIMFeatureNameID").val(vData.PIMFeatureData.Rows[0].FeatureNameID || '');
                            $("#btnPIMFeatureName").text(vData.PIMFeatureData.Rows[0].Feature || 'Please Select').button('refresh');

                            $("#txtPIMFeatureMeasureID").val(vData.PIMFeatureData.Rows[0].FeatureMeasureID || '');
                            $("#btnPIMFeatureMeasure").text(vData.PIMFeatureData.Rows[0].FeatureMeasure || 'Please Select').button('refresh');
                            $("#txtPIMFeatureMeasureSign").val(vData.PIMFeatureData.Rows[0].FeatureMeasureSign || '');

                            if ($("#txtPIMFeatureMeasureID").val() == '') {
                                $("#selShowMeasurement").val('0').slider("refresh");
                                $("#divFeatureMeasurement").hide();
                            } else {
                                $("#selShowMeasurement").val('1').slider("refresh");
                                $("#divFeatureMeasurement").show();
                            }

                            $("#selPIMFeatureType").val(vData.PIMFeatureData.Rows[0].FeatureType || '').selectmenu('refresh');
                            $("#selPIMFeatureDisplayFormat").val(vData.PIMFeatureData.Rows[0].DisplayFormat || '').selectmenu('refresh');


                            if ($.trim($("#selPIMFeatureType").val()) == "Multiple Option") {
                                $("#divPIMFeatureStructure").show();

                                AjaxSend({
                                    url: "PIMFeature/getFeatureOptionList",
                                    data: pData,
                                    success: function (vDataFeatureOption) {

                                        LoadPIMFeatureOption(vDataFeatureOption.PIMFeatureData.Rows);

                                    }
                                });


                            } else {
                                $("#divPIMFeatureStructure").hide();
                            }

                        }
                    }
                });

            };

            function LoadPIMFeatureOption(vRows) {

                for (var p = 0; p <= vRows.length - 1; p++) {

                    arFeatureOption = new Array();
                    arFeatureOption[intFeatureOptionRecID] = vRows[p].ID;
                    arFeatureOption[intFeatureOption] = vRows[p].StructuredValue;
                    arFeatureOption[intFeatureOptionState] = "";

                    sessionStorage["PIMFeatureOption-" + p] = JSON.stringify(arFeatureOption);

                };

                intFeatureOptionCntr = vRows.length;
                LoadPIMFeatureOptionList();

            };

            function ShowPIMFeatureOptionInfo(arData, index) {

                $.mobile.changePage('#pageFeatureOption');

                ClearFeatureOptionFields();

                intFeatureOptionIndex = index;

                $("#txtPIMFeatureOptionID").val(arData[intFeatureOptionRecID] || '');
                $("#txtPIMFeatureOption").val(arData[intFeatureOption] || '');

                IsNewFeatureOption = false;

                $("#btnPIMFeatureOptionRemove").show();

            };

            function ClearPIMFeatureFields() {

                // dont clear
                // sessionStorage.clear();
                proj.ClearSessionStorage();

                $("#txtPIMFeatureID").val('');

                $("#txtPIMFeatureLabelID").val('');
                $("#btnPIMFeatureLabel").text('Please Select').button('refresh');

                $("#txtPIMFeatureNameID").val('');
                $("#btnPIMFeatureName").text('Please Select').button('refresh');

                $("#selShowMeasurement").val('0').slider("refresh");
                $("#divFeatureMeasurement").hide();

                $("#txtPIMFeatureMeasureID").val('');
                $("#btnPIMFeatureMeasure").text('Please Select').button('refresh');
                $("#txtPIMFeatureMeasureSign").val('');

                $("#selPIMFeatureType").val('').selectmenu('refresh');
                $("#selPIMFeatureDisplayFormat").val('').selectmenu('refresh');

                LoadPIMFeatureOptionList();
            }

            function ClearFeatureOptionFields() {
                $("#txtPIMFeatureOptionID").val('');
                $("#txtPIMFeatureOption").val('');

                $("#btnPIMFeatureOptionRemove").hide();

            }

            function SavePIMFeatureInfo() {
                var pData = [];
                var pDataFeatureOption = [];
                var intIndex;

                if ($.trim($("#selPIMFeatureType").val()) == "Multiple Option") {
                    intIndex = 0;
                    for (var i = 0; i <= intFeatureOptionCntr; i++) {

                        if (sessionStorage["PIMFeatureOption-" + i] != undefined) {

                            arFeatureOption = new Array();
                            arFeatureOption = JSON.parse(sessionStorage["PIMFeatureOption-" + i]);

                            pDataFeatureOption[intIndex] = {
                                ID: arFeatureOption[intFeatureOptionRecID],
                                FeatureOption: arFeatureOption[intFeatureOption],
                                RecState: arFeatureOption[intFeatureOptionState]
                            };

                            intIndex = intIndex + 1;

                        }
                    }
                }

                pData = {
                    PIMFeatureID: $.trim($("#txtPIMFeatureID").val()),
                    FeatureLabelID: $.trim($("#txtPIMFeatureLabelID").val()),
                    FeatureNameID: $.trim($("#txtPIMFeatureNameID").val()),
                    FeatureMeasureID: ($.trim($("#selShowMeasurement").val()) == "1" ? $.trim($("#txtPIMFeatureMeasureID").val()) : ''),
                    FeatureType: $.trim($("#selPIMFeatureType").val()),
                    DisplayFormat: $.trim($("#selPIMFeatureDisplayFormat").val()),
                    FeatureOptionList: pDataFeatureOption
                };

                AjaxSend({
                    url: "PIMFeature/CheckUniqueFeature",
                    data: pData,
                    success: function (vData) {

                        if (vData.PIMFeatureData.Rows.length > 0) {
                            MessageBox("Feature already exist.",
                            "Feature",
                            [
                                { Text: 'Ok' }
                            ]
                        );

                        }
                        else {
                            AjaxSend({
                                url: "PIMFeature/SaveFeatureInfo",
                                data: pData,
                                success: function (vSaveData) {

                                    $("#PIMFeatureID").val(vSaveData.PIMFeatureData || '');

                                    $.mobile.changePage("#" + $(window.pageOrig).attr('id'));

                                }
                            });

                        }

                    }
                });

            };

            function SaveFeatureOption() {

                arFeatureOption = new Array();
                arFeatureOption[intFeatureOptionRecID] = $("#txtPIMFeatureOptionID").val();
                arFeatureOption[intFeatureOption] = $("#txtPIMFeatureOption").val();
                arFeatureOption[intFeatureOptionState] = (IsNewFeatureOption ? "New" : "Edit");

                sessionStorage["PIMFeatureOption-" + intFeatureOptionIndex] = JSON.stringify(arFeatureOption);

                ChangePage('/PIMFeature/PIMFeatureInfo');
                LoadPIMFeatureOptionList();
            };

            function RemoveFeatureOption() {

                arFeatureOption = new Array();
                arFeatureOption[intFeatureOptionRecID] = $("#txtPIMFeatureOptionID").val();
                arFeatureOption[intFeatureOption] = $("#txtPIMFeatureOption").val();
                arFeatureOption[intFeatureOptionState] = "Remove";

                sessionStorage["PIMFeatureOption-" + intFeatureOptionIndex] = JSON.stringify(arFeatureOption);

                ChangePage('/PIMFeature/PIMFeatureInfo');
                LoadPIMFeatureOptionList();
            };

            function LoadPIMFeatureOptionList() {
                var pLV = $('#lstPIMFeatureOption');
                var pItem;

                pLV.html('');
                for (var i = 0; i <= intFeatureOptionCntr; i++) {

                    if (sessionStorage["PIMFeatureOption-" + i] != undefined) {

                        arFeatureOption = new Array();
                        arFeatureOption = JSON.parse(sessionStorage["PIMFeatureOption-" + i]);

                        if (arFeatureOption[intFeatureOptionState] != "Remove") {

                            pItem = $('<li id="lst' + i + '">')
                            .append($('<a>')
                                    .attr('ID', i)
                                    .data('Data', arFeatureOption)
                                    .click(function (e) {
                                        e.preventDefault();
                                        ShowPIMFeatureOptionInfo($(this).data('Data'), $(this).attr('ID'));
                                    })
                                    .append($('<h3>')
                                        .attr("style", "white-space:normal")
                                        .text(arFeatureOption[intFeatureOption])
                                    )
                            );


                            pLV.append(pItem);

                        }

                    }
                }

                pLV.listview('refresh');
            };

        });
    });