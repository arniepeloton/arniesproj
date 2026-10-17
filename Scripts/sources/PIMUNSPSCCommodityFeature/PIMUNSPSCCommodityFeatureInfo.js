require([
    'common'
    ], function () {

        $(function () {

            var IsNewCommFeature = true;
            var arCommFeature = new Array();
            var intCommFeatureCntr = -1;
            var intCommFeatureIndex = 0;

            var intCommFeatureRecID = 0;
            var intCommFeatureFeatureID = 1;
            var intCommFeatureFeature = 2;
            var intCommFeatureIsSearchable = 3;
            var intCommFeatureIsKeyFeature = 4;
            var intCommFeatureSortOrder = 5;
            var intCommFeatureOptionState = 6;

            var mLoaded = false;

            $.mobile.activePage.bind('pageshow', PageShow);
            function PageShow() {
                mLoaded = true;

                if (!window.IsPageLoaded) {

                    AjaxSend({
                        url: 'PIMUNSPSCCommodityFeature/IsNewEntry',
                        success: function (vData) {

                            ShowPIMUNSPSCCommodityFeatureInfo(vData.PIMUNSPSCCommodityID || '')

                        }
                    });

                    window.IsPageLoaded = true;
                }
            };
            if (!mLoaded) PageShow();

            $('#btnPIMUNSPSCCommFeature').click(function () {

                Browser.LibraryPIMFeature.Click = function (e) {

                    $('#txtPIMUNSPSCCommFeatureFeatureID').val(e.ID)
                    $('#btnPIMUNSPSCCommFeature').text(e.Feature).button('refresh');

                };
                Browser.LibraryPIMFeature.Show();


            });

            $("#txtPIMUNSPSCCommFeatureSortOrder").keydown(function (e) {
                if (!NumberOnly(e)) {
                    return;
                }
            });

            $('#btnAddPIMUNSPSCCommFeature').click(function (e) {
                e.preventDefault();

                IsNewCommFeature = true

                $.mobile.changePage('#pageUNSPSCCommFeature');
                ClearUNSPSCCommFeatureFeatureFields();

            });

            $('#btnPIMUNSPSCCommFeatureSave').click(function (e) {
                e.preventDefault();

                if ($.trim($("#txtPIMUNSPSCCommFeatureFeatureID").val()) == "") {
                    MessageBox("Please select a feature.",
                           "Feature",
                            [
                                { Text: 'Ok'
                                }
                            ]
                    );
                    return;
                }

                SaveUNSPSCCommFeatureFeature();

            });

            $('#btnPIMUNSPSCCommFeatureRemove').click(function (e) {
                e.preventDefault();

                MessageBox("Remove this record?",
                'Remove',
                [
                    { Text: 'Yes',
                        Click: function () {
                            RemoveUNSPSCCommFeatureFeature();
                        }
                    }
                ,
                    { Text: 'No'
                    }
                ]
            );

            });

            $('#btnPIMUNSPSCCommFeatureCancel').click(function (e) {
                e.preventDefault(); 1

                ChangePage('/PIMUNSPSCCommodityFeature/PIMUNSPSCCommodityFeatureInfo');

            });

            $('#btnPIMUNSPSCCommFeatureInfoDone').click(function (e) {
                e.preventDefault();

                if ($.trim($("#txtPIMUNSPSCCommodityFeatureRecID").val()) == "") {
                    MessageBox("Please select UNSPSC commodity.",
                            "UNSPSC Commodity",
                            [
                                { Text: 'Ok' }
                            ]
                    );
                    return;
                }

                SavePIMUNSPSCCommFeatureInfo();

            });

            function ShowPIMUNSPSCCommodityFeatureInfo(strID) {
                var pData = {};
                pData.PIMUNSPSCCommodityID = strID;

                ClearUNSPSCCommFeatureFields();

                AjaxSend({
                    url: "PIMUNSPSCCommodityFeature/getUNSPSCCommodityInfo",
                    data: pData,
                    success: function (vData) {

                        if (vData.PIMUNSPSCCommFeatureData.Rows.length > 0) {

                            $("#txtPIMUNSPSCCommodityFeatureRecID").val(vData.PIMUNSPSCCommFeatureData.Rows[0].ID || '');

                            $("#txtPIMUNSPSCComFeatureSegment").val(vData.PIMUNSPSCCommFeatureData.Rows[0].Segment || '');
                            $("#txtPIMUNSPSCComFeatureFamily").val(vData.PIMUNSPSCCommFeatureData.Rows[0].Family || '');
                            $("#txtPIMUNSPSCComFeatureClass").val(vData.PIMUNSPSCCommFeatureData.Rows[0].Class || '');
                            $("#txtPIMUNSPSCComFeatureCommodity").val(vData.PIMUNSPSCCommFeatureData.Rows[0].Commodity || '');

                            AjaxSend({
                                url: "PIMUNSPSCCommodityFeature/getUNSPSCCommFeatureList",
                                data: pData,
                                success: function (vDataFeature) {

                                    LoadPIMUNSPSCCommFeature(vDataFeature.PIMUNSPSCCommFeatureData.Rows);

                                }
                            });

                        } 
                    }
                });

            };

            function LoadPIMUNSPSCCommFeature(vRows) {

                for (var p = 0; p <= vRows.length - 1; p++) {

                    arCommFeature = new Array();
                    arCommFeature[intCommFeatureRecID] = vRows[p].ID;
                    arCommFeature[intCommFeatureFeatureID] = vRows[p].FeatureID;
                    arCommFeature[intCommFeatureFeature] = vRows[p].Feature;
                    arCommFeature[intCommFeatureIsSearchable] = vRows[p].IsSearchable;
                    arCommFeature[intCommFeatureIsKeyFeature] = vRows[p].IsKeyFeature;
                    arCommFeature[intCommFeatureSortOrder] = vRows[p].SortOrder;
                    arCommFeature[intCommFeatureOptionState] = "";

                    sessionStorage["PIMUNSPSCCommFeature-" + p] = JSON.stringify(arCommFeature);

                };

                intCommFeatureCntr = vRows.length;
                LoadPIMUNSPSCCommFeatureList(0);

            };

            function ShowPIMCommFeatureInfo(arData, index) {

                $.mobile.changePage('#pageUNSPSCCommFeature');

                ClearUNSPSCCommFeatureFeatureFields();

                intCommFeatureIndex = index;

                $("#txtPIMUNSPSCCommFeatureID").val(arData[intCommFeatureRecID] || '');
                $("#txtPIMUNSPSCCommFeatureFeatureID").val(arData[intCommFeatureFeatureID] || '');
                $("#btnPIMUNSPSCCommFeature").text(arData[intCommFeatureFeature] || '').button('refresh');
                $("#selKeyFeature").val(arData[intCommFeatureIsKeyFeature]).slider("refresh");
                $("#selSearchableFeature").val(arData[intCommFeatureIsSearchable]).slider("refresh");
                $("#txtPIMUNSPSCCommFeatureSortOrder").val(arData[intCommFeatureSortOrder] || '');

                IsNewCommFeature = false;

                $("#btnPIMUNSPSCCommFeatureRemove").show();

            };

            function ClearUNSPSCCommFeatureFields() {

                // dont clear
                // sessionStorage.clear();
                proj.ClearSessionStorage();

                $("#txtPIMUNSPSCCommodityFeatureRecID").val('');

                $("#txtPIMUNSPSCComFeatureSegment").val('');
                $("#txtPIMUNSPSCComFeatureFamily").val('');
                $("#txtPIMUNSPSCComFeatureClass").val('');
                $("#txtPIMUNSPSCComFeatureCommodity").val('');

                LoadPIMUNSPSCCommFeatureList(0);
            }

            function ClearUNSPSCCommFeatureFeatureFields() {
                $("#txtPIMUNSPSCCommFeatureID").val('');
                $("#txtPIMUNSPSCCommFeatureFeatureID").val('');
                $("#btnPIMUNSPSCCommFeature").text('Please Select').button('refresh');
                $("#selKeyFeature").val('0').slider("refresh");
                $("#selSearchableFeature").val('0').slider("refresh");
                $("#txtPIMUNSPSCCommFeatureSortOrder").val((intCommFeatureCntr < 0 ? 1 : intCommFeatureCntr + 1) + '');

                $("#btnPIMFeatureOptionRemove").hide();

            }

            function SavePIMUNSPSCCommFeatureInfo() {
                var pData = [];
                var pDataUNSPSCCommFeature = [];
                var intIndex;

                intIndex = 0;
                for (var i = 0; i <= intCommFeatureCntr; i++) {

                    if (sessionStorage["PIMUNSPSCCommFeature-" + i] != undefined) {

                        arCommFeature = new Array();
                        arCommFeature = JSON.parse(sessionStorage["PIMUNSPSCCommFeature-" + i]);

                        pDataUNSPSCCommFeature[intIndex] = {
                            ID: arCommFeature[intCommFeatureRecID],
                            FeatureID: arCommFeature[intCommFeatureFeatureID],
                            SortOrder: arCommFeature[intCommFeatureSortOrder],
                            IsKeyFeature: arCommFeature[intCommFeatureIsKeyFeature],
                            IsSearchable: arCommFeature[intCommFeatureIsSearchable],
                            RecState: arCommFeature[intCommFeatureOptionState]
                        };

                        intIndex = intIndex + 1;

                    }
                }

                pData = {
                    PIMUNSPSCCommodityID: $.trim($("#txtPIMUNSPSCCommodityFeatureRecID").val()),
                    UNSPSCCommFeatureList: pDataUNSPSCCommFeature
                };

                AjaxSend({
                    url: "PIMUNSPSCCommodityFeature/SaveUNSPSCommodityInfo",
                    data: pData,
                    success: function (vSaveData) {

                        $("#txtPIMUNSPSCCommodityFeatureRecID").val(vSaveData.PIMFeatureData || '');

                        $.mobile.changePage("#" + $(window.pageOrig).attr('id'));

                    }
                });

            };

            function SaveUNSPSCCommFeatureFeature() {

                if (IsNewCommFeature) {
                    intCommFeatureCntr = intCommFeatureCntr + 1;
                    intCommFeatureIndex = intCommFeatureCntr;
                }

                arCommFeature = new Array();
                arCommFeature[intCommFeatureRecID] = $("#txtPIMUNSPSCCommFeatureID").val();
                arCommFeature[intCommFeatureFeatureID] = $("#txtPIMUNSPSCCommFeatureFeatureID").val();
                arCommFeature[intCommFeatureFeature] = $("#btnPIMUNSPSCCommFeature").text();
                arCommFeature[intCommFeatureIsKeyFeature] = $("#selKeyFeature").val();
                arCommFeature[intCommFeatureIsSearchable] = $("#selSearchableFeature").val();
                arCommFeature[intCommFeatureSortOrder] = $("#txtPIMUNSPSCCommFeatureSortOrder").val();
                arCommFeature[intCommFeatureOptionState] = (IsNewCommFeature ? "New" : "Edit");

                sessionStorage["PIMUNSPSCCommFeature-" + intCommFeatureIndex] = JSON.stringify(arCommFeature);

                ChangePage('/PIMUNSPSCCommodityFeature/PIMUNSPSCCommodityFeatureInfo');
                LoadPIMUNSPSCCommFeatureList(0);
            };

            function RemoveUNSPSCCommFeatureFeature() {

                arCommFeature = new Array();
                arCommFeature[intCommFeatureRecID] = $("#txtPIMUNSPSCCommFeatureID").val();
                arCommFeature[intCommFeatureFeatureID] = $("#txtPIMUNSPSCCommFeatureFeatureID").val();
                arCommFeature[intCommFeatureFeature] = $("#btnPIMUNSPSCCommFeature").text();
                arCommFeature[intCommFeatureIsKeyFeature] = $("#selKeyFeature").val();
                arCommFeature[intCommFeatureIsSearchable] = $("#selSearchableFeature").val();
                arCommFeature[intCommFeatureSortOrder] = $("#txtPIMUNSPSCCommFeatureSortOrder").val();
                arCommFeature[intCommFeatureOptionState] = "Remove";

                sessionStorage["PIMUNSPSCCommFeature-" + intCommFeatureIndex] = JSON.stringify(arCommFeature);

                ChangePage('/PIMUNSPSCCommodityFeature/PIMUNSPSCCommodityFeatureInfo');
                LoadPIMUNSPSCCommFeatureList(0);
            };

            function LoadPIMUNSPSCCommFeatureList(intCurIndex) {
                var pLV = $('#lstPIMUNSPSCCommFeature');
                var pItem;

                if (intCurIndex > intCommFeatureCntr) {
                    return;
                }

                if (intCurIndex == 0) {
                    pLV.html('');
                }

                if (sessionStorage["PIMUNSPSCCommFeature-" + intCurIndex] != undefined) {

                    arCommFeature = new Array();
                    arCommFeature = JSON.parse(sessionStorage["PIMUNSPSCCommFeature-" + intCurIndex]);

                    if (arCommFeature[intCommFeatureOptionState] != "Remove") {


                        pItem = $('<li data-role="list-divider">')
                                    .append($('<div data-role="fieldcontain" style="height:5px;">')
                                        .text("Feature No. " + arCommFeature[intCommFeatureSortOrder] + " : " + arCommFeature[intCommFeatureFeature])
                                        .append($('<a data-role="button" data-mini="true" style="float:right; margin-top:-15px;">')
                                            .attr('ID', intCurIndex)
                                            .data('Data', arCommFeature)
                                            .click(function (e) {
                                                e.preventDefault();
                                                ShowPIMCommFeatureInfo($(this).data('Data'), $(this).attr('ID'));
                                            })
                                            .text("Edit")
                                        )
                                    );

                        pLV.append(pItem);

                        var pData = {};
                        pData.PIMFeatureID = arCommFeature[intCommFeatureFeatureID];

                        AjaxSend({
                            url: "PIMFeature/getFeatureInfo",
                            data: pData,
                            success: function (vData) {

                                if (vData.PIMFeatureData.Rows.length > 0) {

                                    if (vData.PIMFeatureData.Rows[0].FeatureType == "Comment/Essay Box") {
                                        pItem = $('<li>')
                                            .append($('<div data-role="fieldcontain">')
                                                .append($('<label style = "font-weight:normal;">')
                                                    .attr('for', "txtArea" + arCommFeature[intCommFeatureFeature])
                                                    .text(vData.PIMFeatureData.Rows[0].FeatureLabel)
                                                )
                                                .append($('<div>')
                                                    .append($('<textarea>')
                                                        .attr('id', "txtArea" + arCommFeature[intCommFeatureFeature])
                                                    )
                                                )
                                            );

                                        pLV.append(pItem);

                                        pLV.trigger("create");
                                        pLV.listview('refresh');

                                        LoadPIMUNSPSCCommFeatureList(intCurIndex + 1);

                                    } else if (vData.PIMFeatureData.Rows[0].FeatureType == "Date Only") {
                                        pItem = $('<li>')
                                            .append($('<div data-role="fieldcontain">')
                                                .append($('<label style = "font-weight:normal;">')
                                                    .text(vData.PIMFeatureData.Rows[0].FeatureLabel)
                                                )
                                                .append($('<fieldset data-role="controlgroup" data-type="horizontal">')
                                                    .append($('<select>')
                                                        .attr('id', "sel" + arCommFeature[intCommFeatureFeature] + "Month")
                                                        .append($('<option>')
                                                            .attr("value", "")
                                                            .text("MMM")
                                                        )
                                                    )
                                                    .append($('<select>')
                                                        .attr('id', "sel" + arCommFeature[intCommFeatureFeature] + "Day")
                                                        .append($('<option>')
                                                            .attr("value", "")
                                                            .text("DD")
                                                        )
                                                    )
                                                    .append($('<select>')
                                                        .attr('id', "sel" + arCommFeature[intCommFeatureFeature] + "Year")
                                                        .append($('<option>')
                                                            .attr("value", "")
                                                            .text("YYYY")
                                                        )
                                                    )
                                                )
                                            );

                                        pLV.append(pItem);

                                        pLV.trigger("create");
                                        pLV.listview('refresh');

                                        LoadPIMUNSPSCCommFeatureList(intCurIndex + 1);

                                    } else if (vData.PIMFeatureData.Rows[0].FeatureType == "Time Only") {
                                        pItem = $('<li>')
                                            .append($('<div data-role="fieldcontain">')
                                                .append($('<label style = "font-weight:normal;">')
                                                    .text(vData.PIMFeatureData.Rows[0].FeatureLabel)
                                                )
                                                .append($('<fieldset data-role="controlgroup" data-type="horizontal">')
                                                    .append($('<select>')
                                                        .attr('id', "sel" + arCommFeature[intCommFeatureFeature] + "Hour")
                                                        .append($('<option>')
                                                            .attr("value", "")
                                                            .text("HH")
                                                        )
                                                    )
                                                    .append($('<select>')
                                                        .attr('id', "sel" + arCommFeature[intCommFeatureFeature] + "Min")
                                                        .append($('<option>')
                                                            .attr("value", "")
                                                            .text("MM")
                                                        )
                                                    )
                                                    .append($('<select>')
                                                        .attr('id', "sel" + arCommFeature[intCommFeatureFeature] + "AMPM")
                                                        .append($('<option>')
                                                            .attr("value", "")
                                                            .text("AM/PM")
                                                        )
                                                    )
                                                )
                                            );

                                        pLV.append(pItem);

                                        pLV.trigger("create");
                                        pLV.listview('refresh');

                                        LoadPIMUNSPSCCommFeatureList(intCurIndex + 1);

                                    } else if (vData.PIMFeatureData.Rows[0].FeatureType == "Date and Time") {
                                        pItem = $('<li>')
                                            .append($('<div data-role="fieldcontain">')
                                                .append($('<label style = "font-weight:normal;">')
                                                    .text(vData.PIMFeatureData.Rows[0].FeatureLabel)
                                                )
                                                .append($('<fieldset data-role="controlgroup" data-type="horizontal">')
                                                    .append($('<select>')
                                                        .attr('id', "sel" + arCommFeature[intCommFeatureFeature] + "Month")
                                                        .append($('<option>')
                                                            .attr("value", "")
                                                            .text("MMM")
                                                        )
                                                    )
                                                    .append($('<select>')
                                                        .attr('id', "sel" + arCommFeature[intCommFeatureFeature] + "Day")
                                                        .append($('<option>')
                                                            .attr("value", "")
                                                            .text("DD")
                                                        )
                                                    )
                                                    .append($('<select>')
                                                        .attr('id', "sel" + arCommFeature[intCommFeatureFeature] + "Year")
                                                        .append($('<option>')
                                                            .attr("value", "")
                                                            .text("YYYY")
                                                        )
                                                    )
                                                )
                                            )
                                            .append($('<div data-role="fieldcontain">')
                                                .append($('<fieldset data-role="controlgroup" data-type="horizontal">')
                                                    .append($('<select>')
                                                        .attr('id', "sel" + arCommFeature[intCommFeatureFeature] + "Hour")
                                                        .append($('<option>')
                                                            .attr("value", "")
                                                            .text("HH")
                                                        )
                                                    )
                                                    .append($('<select>')
                                                        .attr('id', "sel" + arCommFeature[intCommFeatureFeature] + "Min")
                                                        .append($('<option>')
                                                            .attr("value", "")
                                                            .text("MM")
                                                        )
                                                    )
                                                    .append($('<select>')
                                                        .attr('id', "sel" + arCommFeature[intCommFeatureFeature] + "AMPM")
                                                        .append($('<option>')
                                                            .attr("value", "")
                                                            .text("AM/PM")
                                                        )
                                                    )
                                                )
                                            );

                                        pLV.append(pItem);

                                        pLV.trigger("create");
                                        pLV.listview('refresh');

                                        LoadPIMUNSPSCCommFeatureList(intCurIndex + 1);

                                    } else if (vData.PIMFeatureData.Rows[0].FeatureType == "Multiple Option") {

                                        AjaxSend({
                                            url: "PIMFeature/getFeatureOptionList",
                                            data: pData,
                                            success: function (vDataFeatureOption) {

                                                if (vDataFeatureOption.PIMFeatureData.Rows.length > 0) {

                                                    if (vData.PIMFeatureData.Rows[0].DisplayFormat == "Radio Button") {

                                                        var pLVOption = $('<fieldset data-role="controlgroup">');
                                                        var pLVOptionValue;
                                                        for (i = 0; i < vDataFeatureOption.PIMFeatureData.Rows.length; i++) {
                                                            pLVOptionValue = $('<input type="radio">')
                                                                            .attr('id', "rdo" + vDataFeatureOption.PIMFeatureData.Rows[i].ID)
                                                                            .attr('name', arCommFeature[intCommFeatureFeature])
                                                                        .append($('<label for="' + "rdo" + vDataFeatureOption.PIMFeatureData.Rows[i].ID + '">')
                                                                            .text(vDataFeatureOption.PIMFeatureData.Rows[i].StructuredValue)
                                                                        );
                                                            pLVOption.append(pLVOptionValue);
                                                        }


                                                        pItem = $('<li>')
                                                            .append($('<div data-role="fieldcontain" >')
                                                                .append($('<label style = "font-weight:normal;">')
                                                                .text(vData.PIMFeatureData.Rows[0].FeatureLabel)
                                                            )
                                                            .append(pLVOption)
                                                        );

                                                        pLV.append(pItem);

                                                        pLV.trigger("create");
                                                        pLV.listview('refresh');

                                                        LoadPIMUNSPSCCommFeatureList(intCurIndex + 1);

                                                    } else if (vData.PIMFeatureData.Rows[0].DisplayFormat == "Drop Down List") {

                                                        var pLVOption = $('<select id=' + arCommFeature[intCommFeatureFeature] + '>')
                                                        var pLVOptionValue;

                                                        pLVOptionValue = $('<option>')
                                                                        .attr("value", "")
                                                                        .text("Please select")
                                                        pLVOption.append(pLVOptionValue);

                                                        for (i = 0; i < vDataFeatureOption.PIMFeatureData.Rows.length; i++) {
                                                            pLVOptionValue = $('<option>')
                                                                            .attr("value", vDataFeatureOption.PIMFeatureData.Rows[i].ID)
                                                                            .text(vDataFeatureOption.PIMFeatureData.Rows[i].StructuredValue);
                                                            pLVOption.append(pLVOptionValue);
                                                        }

                                                        pItem = $('<li>')
                                                            .append($('<div data-role="fieldcontain" >')
                                                                .append($('<label style = "font-weight:normal;">')
                                                                    .text(vData.PIMFeatureData.Rows[0].FeatureLabel)
                                                                )
                                                                .append($('<div>')
                                                                    .append(pLVOption)
                                                                )
                                                            );

                                                        pLV.append(pItem);

                                                        pLV.trigger("create");
                                                        pLV.listview('refresh');

                                                        LoadPIMUNSPSCCommFeatureList(intCurIndex + 1);

                                                    }

                                                }

                                            }
                                        });


                                    } else if (vData.PIMFeatureData.Rows[0].FeatureType == "Textbox") {
                                        pItem = $('<li>')
                                            .append($('<div data-role="fieldcontain">')
                                                .append($('<label style = "font-weight:normal;">')
                                                    .attr('for', "txt" + arCommFeature[intCommFeatureFeature])
                                                    .text(vData.PIMFeatureData.Rows[0].FeatureLabel)
                                                )
                                                .append($('<div>')
                                                    .append($('<input>')
                                                        .attr('id', "txt" + arCommFeature[intCommFeatureFeature])
                                                        .attr('type', "text")
                                                    )
                                                )
                                            );

                                        pLV.append(pItem);

                                        pLV.trigger("create");
                                        pLV.listview('refresh');

                                        LoadPIMUNSPSCCommFeatureList(intCurIndex + 1);

                                    } else if (vData.PIMFeatureData.Rows[0].FeatureType == "Textbox (Numeric)") {
                                        pItem = $('<li>')
                                            .append($('<div data-role="fieldcontain">')
                                                .append($('<label style = "font-weight:normal;">')
                                                    .attr('for', "txt" + arCommFeature[intCommFeatureFeature])
                                                    .text(vData.PIMFeatureData.Rows[0].FeatureLabel)
                                                )
                                                .append($('<div>')
                                                    .append($('<input>')
                                                        .attr('id', "txt" + arCommFeature[intCommFeatureFeature])
                                                        .attr('type', "number")
                                                    )
                                                )
                                            );

                                        pLV.append(pItem);

                                        pLV.trigger("create");
                                        pLV.listview('refresh');

                                        LoadPIMUNSPSCCommFeatureList(intCurIndex + 1);

                                    } else if (vData.PIMFeatureData.Rows[0].FeatureType == "Textbox (Decimal)") {
                                        pItem = $('<li>')
                                            .append($('<div data-role="fieldcontain">')
                                                .append($('<label style = "font-weight:normal;">')
                                                    .attr('for', "txt" + arCommFeature[intCommFeatureFeature])
                                                    .text(vData.PIMFeatureData.Rows[0].FeatureLabel)
                                                )
                                                .append($('<div>')
                                                    .append($('<input>')
                                                        .attr('id', "txt" + arCommFeature[intCommFeatureFeature])
                                                        .attr('type', "number")
                                                    )
                                                )
                                            );

                                        pLV.append(pItem);

                                        pLV.trigger("create");
                                        pLV.listview('refresh');

                                        LoadPIMUNSPSCCommFeatureList(intCurIndex + 1);
                                    }

                                }

                            }
                        });

                    }


                } else {
                    if (intCurIndex <= intCommFeatureCntr) {
                        LoadPIMUNSPSCCommFeatureList(intCurIndex + 1);
                    }
                }

            };

        });
    });