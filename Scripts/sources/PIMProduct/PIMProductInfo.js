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
                        url: 'PIMProduct/IsNewEntry',
                        success: function (vData) {

                            ShowPIMProductInfo(vData.PIMProductID || '')

                        }
                    });

                    window.IsPageLoaded = true;
                }
            };
            if (!mLoaded) PageShow();

            $('#btnPIMProductOrganization').click(function (e) {

                Browser.LibraryCustomers.Click = function (e) {

                    $('#txtPIMProductOrganizationID').val(e.ID)
                    $('#btnPIMProductOrganization').text(e.CompanyName)
                .button('refresh');

                };

                Browser.LibraryCustomers.Show();

            });

            $('#btnPIMProductFamily').click(function (e) {

                Browser.LibraryPIMProductFamily.Click = function (e) {

                    $('#txtPIMProductFamilyID').val(e.ID)
                    $('#btnPIMProductFamily').text(e.Label)
                .button('refresh');

                };

                Browser.LibraryPIMProductFamily.Show();

            });

            $('#btnPIMProductUNSPSCCommdity').click(function (e) {

                Browser.LibraryUNSPSCCommodity.Click = function (e) {

                    $('#txtPIMProductUNSPSCCommID').val(e.ID);
                    $('#btnPIMProductUNSPSCCommdity').text(e.Commodity).button('refresh');
                    $('#txtPIMProductUNSPSCSegmentID').val(e.SegmentID);
                    $('#txtPIMProductUNSPSCSegment').val(e.Segment);
                    $('#txtPIMProductUNSPSCFamilyID').val(e.FamilyID);
                    $('#txtPIMProductUNSPSCFamily').val(e.Family);
                    $('#txtPIMProductUNSPSCClassID').val(e.ClassID);
                    $('#txtPIMProductUNSPSCClass').val(e.Class);

                    ShowPIMUNSPSCCategoryFeatures(e.ID);
                };

                Browser.LibraryUNSPSCCommodity.Show();

            });

            $('#btnPIMProductInfoDone').click(function (e) {
                e.preventDefault();

                if ($.trim($("#txtPIMProductOrganizationID").val()) == "") {
                    MessageBox("Please select product owner.",
                            "Product Owner",
                            [
                                { Text: 'Ok' }
                            ]
                    );
                    return;
                }

                if ($.trim($("#txtPIMProductNo").val()) == "") {
                    MessageBox("Please enter product number.",
                            "Product Number",
                            [
                                { Text: 'Ok' }
                            ]
                    );
                    return;
                }

                if ($.trim($("#txtPIMProductName").val()) == "") {
                    MessageBox("Please enter product name.",
                            "Product Name",
                            [
                                { Text: 'Ok' }
                            ]
                    );
                    return;
                }

                if ($.trim($("#txtPIMProductFamilyID").val()) == "") {
                    MessageBox("Please select product family.",
                            "Product Family",
                            [
                                { Text: 'Ok' }
                            ]
                    );
                    return;
                }

                if ($.trim($("#txtPIMProductUNSPSCCommID").val()) == "") {
                    MessageBox("Please select product UNSPSC commodity.",
                            "Commodity",
                            [
                                { Text: 'Ok' }
                            ]
                    );
                    return;
                }

                SavePIMProductInfo();

            });

            function ShowPIMProductInfo(strID) {
                var pData = {};
                pData.PIMProductID = strID;

                ClearPIMProductFields();

                AjaxSend({
                    url: "PIMProduct/getProductInfo",
                    data: pData,
                    success: function (vData) {

                        if (vData.PIMProductData.Rows.length > 0) {

                            $("#txtPIMProductID").val(vData.PIMProductData.Rows[0].ID || '');

                            $("#txtPIMProductOrganizationID").val(vData.PIMProductData.Rows[0].OrganizationID || '');
                            $("#btnPIMProductOrganization").text(vData.PIMProductData.Rows[0].Organization || 'Please Select').button('refresh');

                            $("#txtPIMProductNo").val(vData.PIMProductData.Rows[0].ProductNo || '');
                            $("#txtPIMProductName").val(vData.PIMProductData.Rows[0].ProductName || '');
                            $('#chkPIMProductIsActive').attr("checked", (vData.PIMProductData.Rows[0].IsActive == 1 ? true : false)).checkboxradio("refresh");

                            $("#txtPIMProductFamilyID").val(vData.PIMProductData.Rows[0].ProductFamilyID || '');
                            $("#btnPIMProductFamily").text(vData.PIMProductData.Rows[0].ProductFamily || 'Please Select').button('refresh');

                            $("#txtPIMProductUNSPSCSegmentID").val(vData.PIMProductData.Rows[0].SegmentID || '');
                            $("#txtPIMProductUNSPSCSegment").val(vData.PIMProductData.Rows[0].Segment || '');
                            $("#txtPIMProductUNSPSCFamilyID").val(vData.PIMProductData.Rows[0].FamilyID || '');
                            $("#txtPIMProductUNSPSCFamily").val(vData.PIMProductData.Rows[0].Family || '');
                            $("#txtPIMProductUNSPSCClassID").val(vData.PIMProductData.Rows[0].ClassID || '');
                            $("#txtPIMProductUNSPSCClass").val(vData.PIMProductData.Rows[0].Class || '');
                            $("#txtPIMProductUNSPSCCommID").val(vData.PIMProductData.Rows[0].UNSPSCCommodityID || '');
                            $("#btnPIMProductUNSPSCCommdity").text(vData.PIMProductData.Rows[0].Commodity || 'Please Select').button('refresh');

                            $("#txtPIMProductUNSPSCSegmentIDOrig").val($("#txtPIMProductUNSPSCSegmentID").val());
                            $("#txtPIMProductUNSPSCFamilyIDOrig").val($("#txtPIMProductUNSPSCFamilyID").val());
                            $("#txtPIMProductUNSPSCClassIDOrig").val($("#txtPIMProductUNSPSCClassID").val());
                            $("#txtPIMProductUNSPSCCommIDOrig").val($("#txtPIMProductUNSPSCCommID").val());

                            ShowPIMUNSPSCCategoryFeatures(vData.PIMProductData.Rows[0].UNSPSCCommodityID);
                        } else {
                            var pLV = $('#divProductFeature');
                            pLV.html('');
                        }
                    }
                });

            };

            function ShowPIMUNSPSCCategoryFeatures() {
                var pData = {};
                pData.PIMUNSPSCSegmentID = $("#txtPIMProductUNSPSCSegmentID").val();
                pData.PIMUNSPSCFamilyID = $("#txtPIMProductUNSPSCFamilyID").val();
                pData.PIMUNSPSCClassID = $("#txtPIMProductUNSPSCClassID").val();
                pData.PIMUNSPSCCommodityID = $("#txtPIMProductUNSPSCCommID").val();
                pData.PIMProductID = $("#txtPIMProductID").val();

                AjaxSend({
                    url: "PIMProduct/getProductFeatureValueList",
                    data: pData,
                    success: function (vDataFeatureValue) {

                        AjaxSend({
                            url: "PIMUNSPSCCommodityFeature/getUNSPSCCommFeatureList",
                            data: pData,
                            success: function (vDataFeature) {

                                LoadPIMUNSPSCCommFeatureList(0, vDataFeature.PIMUNSPSCCommFeatureData.Rows, vDataFeatureValue.PIMProductData.Rows);

                            }
                        });

                    }
                });


            };

            function LoadPIMUNSPSCCommFeatureList(intCurIndex, vRows, vValueRows) {
                var pLV = $('#divProductFeature');
                var pItem;

                if (intCurIndex > vRows.length - 1) {
                    return;
                }

                if (intCurIndex == 0) {
                    pLV.html('');
                }

                var pData = {};
                pData.PIMFeatureID = vRows[intCurIndex].FeatureID;

                AjaxSend({
                    url: "PIMFeature/getFeatureInfo",
                    data: pData,
                    success: function (vData) {

                        if (vData.PIMFeatureData.Rows.length > 0) {

                            var objFeatureValue = GetFeatureValue(vRows[intCurIndex].FeatureID, vValueRows);
                            var vObjName = vRows[intCurIndex].FeatureID.replace(" ", "");
                            var vObjMeasureSign = vData.PIMFeatureData.Rows[0].FeatureMeasureSign;

                            if (vData.PIMFeatureData.Rows[0].FeatureType == "Comment/Essay Box") {

                                pItem = $('<div>')
                                    .append($('<div data-role="fieldcontain">')
                                        .append($('<label style = "font-weight:normal;">')
                                            .attr('for', "txtFeature" + vObjName)
                                            .text(vData.PIMFeatureData.Rows[0].FeatureLabel + (vObjMeasureSign.length > 0 ? ' ( ' + vObjMeasureSign + ' )' : ''))
                                        )
                                        .append($('<div>')
                                            .append($('<textarea>')
                                                .attr('id', "txtFeature" + vObjName)
                                                .data('PFeatureData', objFeatureValue)
                                            )
                                        )
                                    );

                                pLV.append(pItem);

                                pLV.trigger("create");

                                $("#txtFeature" + vObjName).val(((objFeatureValue != null || objFeatureValue != undefined) ? objFeatureValue.Value : ""));

                                LoadPIMUNSPSCCommFeatureList(intCurIndex + 1, vRows, vValueRows);

                            } else if (vData.PIMFeatureData.Rows[0].FeatureType == "Date Only") {

                                pItem = $('<div>')
                                    .append($('<div data-role="fieldcontain">')
                                        .append($('<label style = "font-weight:normal;">')
                                            .text(vData.PIMFeatureData.Rows[0].FeatureLabel + (vObjMeasureSign.length > 0 ? ' ( ' + vObjMeasureSign + ' )' : ''))
                                        )
                                        .append($('<fieldset data-role="controlgroup" data-type="horizontal">')
                                            .append($('<select>')
                                                .attr('id', "selFeature" + vObjName + "Month")
                                                .data('PFeatureData', objFeatureValue)
                                                .change(function (e) {
                                                    e.preventDefault();
                                                    setDayCollection("selFeature" + vObjName + "Month", "selFeature" + vObjName + "Day", "selFeature" + vObjName + "Year", true);
                                                })
                                                .append($('<option>')
                                                    .attr("value", "")
                                                    .text("MMM")
                                                )
                                            )
                                            .append($('<select>')
                                                .attr('id', "selFeature" + vObjName + "Day")
                                                .append($('<option>')
                                                    .attr("value", "")
                                                    .text("DD")
                                                )
                                            )
                                            .append($('<select>')
                                                .attr('id', "selFeature" + vObjName + "Year")
                                                .change(function (e) {
                                                    e.preventDefault();
                                                    setDayCollection("selFeature" + vObjName + "Month", "selFeature" + vObjName + "Day", "selFeature" + vObjName + "Year", true);
                                                })
                                                .append($('<option>')
                                                    .attr("value", "")
                                                    .text("YYYY")
                                                )
                                            )
                                        )
                                    );

                                pLV.append(pItem);

                                pLV.trigger("create");

                                setMonthCollection("selFeature" + vObjName + "Month");
                                setYearCollection("selFeature" + vObjName + "Year");

                                if (objFeatureValue.Value != null || objFeatureValue.Value != undefined) {
                                    $("#selFeature" + vObjName + "Month").val($.format.date(new Date(objFeatureValue.Value), 'M')).selectmenu('refresh');
                                    $("#selFeature" + vObjName + "Year").val($.format.date(new Date(objFeatureValue.Value), 'y')).selectmenu('refresh');
                                    setDayCollection("selFeature" + vObjName + "Month", "selFeature" + vObjName + "Day", "selFeature" + vObjName + "Year", true);
                                    $("#selFeature" + vObjName + "Day").val($.format.date(new Date(objFeatureValue.Value), 'd')).selectmenu('refresh');
                                }

                                LoadPIMUNSPSCCommFeatureList(intCurIndex + 1, vRows, vValueRows);

                            } else if (vData.PIMFeatureData.Rows[0].FeatureType == "Time Only") {
                                pItem = $('<div>')
                                    .append($('<div data-role="fieldcontain">')
                                        .append($('<label style = "font-weight:normal;">')
                                            .text(vData.PIMFeatureData.Rows[0].FeatureLabel + (vObjMeasureSign.length > 0 ? ' ( ' + vObjMeasureSign + ' )' : ''))
                                        )
                                        .append($('<fieldset data-role="controlgroup" data-type="horizontal">')
                                            .append($('<select>')
                                                .attr('id', "selFeature" + vObjName + "Hour")
                                                .data('PFeatureData', objFeatureValue)
                                                .append($('<option>')
                                                    .attr("value", "")
                                                    .text("HH")
                                                )
                                            )
                                            .append($('<select>')
                                                .attr('id', "selFeature" + vObjName + "Min")
                                                .append($('<option>')
                                                    .attr("value", "")
                                                    .text("MM")
                                                )
                                            )
                                            .append($('<select>')
                                                .attr('id', "selFeature" + vObjName + "AMPM")
                                                .append($('<option>')
                                                    .attr("value", "")
                                                    .text("AM/PM")
                                                )
                                                .append($('<option>')
                                                    .attr("value", "AM")
                                                    .text("AM")
                                                )
                                                .append($('<option>')
                                                    .attr("value", "PM")
                                                    .text("PM")
                                                )
                                            )
                                        )
                                    );

                                pLV.append(pItem);

                                pLV.trigger("create");

                                setHoursCollection("selFeature" + vObjName + "Hour");
                                setMinutesCollection("selFeature" + vObjName + "Min");

                                if (objFeatureValue.Value != null || objFeatureValue.Value != undefined) {
                                    $("#selFeature" + vObjName + "Hour").val($.format.date(new Date(objFeatureValue.Value), 'h')).selectmenu('refresh');
                                    $("#selFeature" + vObjName + "Min").val($.format.date(new Date(objFeatureValue.Value), 'm')).selectmenu('refresh');
                                    $("#selFeature" + vObjName + "AMPM").val($.format.date(new Date(objFeatureValue.Value), 'a')).selectmenu('refresh');
                                }

                                LoadPIMUNSPSCCommFeatureList(intCurIndex + 1, vRows, vValueRows);

                            } else if (vData.PIMFeatureData.Rows[0].FeatureType == "Date and Time") {
                                pItem = $('<div>')
                                    .append($('<div data-role="fieldcontain">')
                                        .append($('<label style = "font-weight:normal;">')
                                            .text(vData.PIMFeatureData.Rows[0].FeatureLabel + (vObjMeasureSign.length > 0 ? ' ( ' + vObjMeasureSign + ' )' : ''))
                                        )
                                        .append($('<fieldset data-role="controlgroup" data-type="horizontal">')
                                            .append($('<select>')
                                                .attr('id', "selFeature" + vObjName + "Month")
                                                .data('PFeatureData', objFeatureValue)
                                                .change(function (e) {
                                                    e.preventDefault();
                                                    setDayCollection("selFeature" + vObjName + "Month", "selFeature" + vObjName + "Day", "selFeature" + vObjName + "Year", true);
                                                })
                                                .append($('<option>')
                                                    .attr("value", "")
                                                    .text("MMM")
                                                )
                                            )
                                            .append($('<select>')
                                                .attr('id', "selFeature" + vObjName + "Day")
                                                .append($('<option>')
                                                    .attr("value", "")
                                                    .text("DD")
                                                )
                                            )
                                            .append($('<select>')
                                                .attr('id', "selFeature" + vObjName + "Year")
                                                .change(function (e) {
                                                    e.preventDefault();
                                                    setDayCollection("selFeature" + vObjName + "Month", "selFeature" + vObjName + "Day", "selFeature" + vObjName + "Year", true);
                                                })
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
                                                .attr('id', "selFeature" + vObjName + "Hour")
                                                .append($('<option>')
                                                    .attr("value", "")
                                                    .text("HH")
                                                )
                                            )
                                            .append($('<select>')
                                                .attr('id', "selFeature" + vObjName + "Min")
                                                .append($('<option>')
                                                    .attr("value", "")
                                                    .text("MM")
                                                )
                                            )
                                            .append($('<select>')
                                                .attr('id', "selFeature" + vObjName + "AMPM")
                                                .append($('<option>')
                                                    .attr("value", "")
                                                    .text("AM/PM")
                                                )
                                                .append($('<option>')
                                                    .attr("value", "AM")
                                                    .text("AM")
                                                )
                                                .append($('<option>')
                                                    .attr("value", "PM")
                                                    .text("PM")
                                                )
                                            )
                                        )
                                    );

                                pLV.append(pItem);

                                pLV.trigger("create");

                                setMonthCollection("selFeature" + vObjName + "Month");
                                setYearCollection("selFeature" + vObjName + "Year");
                                setHoursCollection("selFeature" + vObjName + "Hour");
                                setMinutesCollection("selFeature" + vObjName + "Min");

                                if (objFeatureValue.Value != null || objFeatureValue.Value != undefined) {

                                    $("#selFeature" + vObjName + "Month").val($.format.date(new Date(objFeatureValue.Value), 'M')).selectmenu('refresh');
                                    $("#selFeature" + vObjName + "Year").val($.format.date(new Date(objFeatureValue.Value), 'y')).selectmenu('refresh');
                                    setDayCollection("selFeature" + vObjName + "Month", "selFeature" + vObjName + "Day", "selFeature" + vObjName + "Year", true);
                                    $("#selFeature" + vObjName + "Day").val($.format.date(new Date(objFeatureValue.Value), 'd')).selectmenu('refresh');

                                    $("#selFeature" + vObjName + "Hour").val($.format.date(new Date(objFeatureValue.Value), 'h')).selectmenu('refresh');
                                    $("#selFeature" + vObjName + "Min").val($.format.date(new Date(objFeatureValue.Value), 'm')).selectmenu('refresh');
                                    $("#selFeature" + vObjName + "AMPM").val($.format.date(new Date(objFeatureValue.Value), 'a')).selectmenu('refresh');
                                }

                                LoadPIMUNSPSCCommFeatureList(intCurIndex + 1, vRows, vValueRows);

                            } else if (vData.PIMFeatureData.Rows[0].FeatureType == "Multiple Option") {

                                AjaxSend({
                                    url: "PIMFeature/getFeatureOptionList",
                                    data: pData,
                                    success: function (vDataFeatureOption) {

                                        if (vDataFeatureOption.PIMFeatureData.Rows.length > 0) {

                                            if (vData.PIMFeatureData.Rows[0].DisplayFormat == "Radio Button") {

                                                var pLVOptionValue;
                                                var pLVOption = $('<fieldset data-role="controlgroup">');

                                                for (i = 0; i < vDataFeatureOption.PIMFeatureData.Rows.length; i++) {
                                                    pLVOptionValue = $('<input type="radio">')
                                                                    .attr('id', "rdoFeature" + vObjName + i)
                                                                    .attr('name', vObjName)
                                                                    .data('data', vDataFeatureOption.PIMFeatureData.Rows[i])
                                                                    .data('PFeatureData', objFeatureValue)
                                                                    .attr("checked", (vDataFeatureOption.PIMFeatureData.Rows[i].ID == ((objFeatureValue != null || objFeatureValue != undefined) ? objFeatureValue.Value : "") ? true : false))

                                                                .append($('<label for="' + "rdoFeature" + vObjName + i + '">')
                                                                    .text(vDataFeatureOption.PIMFeatureData.Rows[i].StructuredValue)
                                                                );
                                                    pLVOption.append(pLVOptionValue);
                                                }


                                                pItem = $('<div>')
                                                    .append($('<div data-role="fieldcontain" >')
                                                        .append($('<label style = "font-weight:normal;">')
                                                        .text(vData.PIMFeatureData.Rows[0].FeatureLabel + (vObjMeasureSign.length > 0 ? ' ( ' + vObjMeasureSign + ' )' : ''))
                                                    )
                                                    .append(pLVOption)
                                                );

                                                pLV.append(pItem);

                                                pLV.trigger("create");

                                                LoadPIMUNSPSCCommFeatureList(intCurIndex + 1, vRows, vValueRows);

                                            } else if (vData.PIMFeatureData.Rows[0].DisplayFormat == "Drop Down List") {

                                                var pLVOptionValue;
                                                var pLVOption = $('<select id="selFeature' + vObjName + '">')
                                                                .data('PFeatureData', objFeatureValue);

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

                                                pItem = $('<div>')
                                                    .append($('<div data-role="fieldcontain" >')
                                                        .append($('<label style = "font-weight:normal;">')
                                                            .attr('for', "selFeature" + vObjName)
                                                            .text(vData.PIMFeatureData.Rows[0].FeatureLabel + (vObjMeasureSign.length > 0 ? ' ( ' + vObjMeasureSign + ' )' : ''))
                                                        )
                                                        .append($('<div>')
                                                            .append(pLVOption)
                                                        )
                                                    );

                                                pLV.append(pItem);

                                                pLV.trigger("create");

                                                $('#selFeature' + vObjName).val(((objFeatureValue != null || objFeatureValue != undefined) ? objFeatureValue.Value : "")).selectmenu('refresh');
                                                LoadPIMUNSPSCCommFeatureList(intCurIndex + 1, vRows, vValueRows);

                                            }

                                        }

                                    }
                                });


                            } else if (vData.PIMFeatureData.Rows[0].FeatureType == "Textbox") {
                                pItem = $('<div>')
                                    .append($('<div data-role="fieldcontain">')
                                        .append($('<label style = "font-weight:normal;">')
                                            .attr('for', "txtFeature" + vObjName)
                                            .text(vData.PIMFeatureData.Rows[0].FeatureLabel + (vObjMeasureSign.length > 0 ? ' ( ' + vObjMeasureSign + ' )' : ''))
                                        )
                                        .append($('<div>')
                                            .append($('<input>')
                                                .attr('id', "txtFeature" + vObjName)
                                                .attr('type', "text")
                                                .data('PFeatureData', objFeatureValue)
                                            )
                                        )
                                    );

                                pLV.append(pItem);

                                pLV.trigger("create");

                                $('#txtFeature' + vObjName).val(((objFeatureValue != null || objFeatureValue != undefined) ? objFeatureValue.Value : ""));

                                LoadPIMUNSPSCCommFeatureList(intCurIndex + 1, vRows, vValueRows);

                            } else if (vData.PIMFeatureData.Rows[0].FeatureType == "Textbox (Numeric)") {
                                pItem = $('<div>')
                                    .append($('<div data-role="fieldcontain">')
                                        .append($('<label style = "font-weight:normal;">')
                                            .attr('for', "txtFeature" + vObjName)
                                            .text(vData.PIMFeatureData.Rows[0].FeatureLabel + (vObjMeasureSign.length > 0 ? ' ( ' + vObjMeasureSign + ' )' : ''))
                                        )
                                        .append($('<div>')
                                            .append($('<input>')
                                                .attr('id', "txtFeature" + vObjName)
                                                .attr('type', "number")
                                                .data('PFeatureData', objFeatureValue)
                                                .keydown(function (e) {
                                                    if (!NumberOnly(e)) {
                                                        return;
                                                    }
                                                })
                                                .val(((objFeatureValue != null || objFeatureValue != undefined) ? objFeatureValue.Value : ""))
                                            )
                                        )
                                    );

                                pLV.append(pItem);

                                pLV.trigger("create");

                                $('#txtFeature' + vObjName).val(((objFeatureValue != null || objFeatureValue != undefined) ? objFeatureValue.Value : ""));

                                LoadPIMUNSPSCCommFeatureList(intCurIndex + 1, vRows, vValueRows);

                            } else if (vData.PIMFeatureData.Rows[0].FeatureType == "Textbox (Decimal)") {
                                pItem = $('<div>')
                                    .append($('<div data-role="fieldcontain">')
                                        .append($('<label style = "font-weight:normal;">')
                                            .attr('for', "txtFeature" + vObjName)
                                            .text(vData.PIMFeatureData.Rows[0].FeatureLabel + (vObjMeasureSign.length > 0 ? ' ( ' + vObjMeasureSign + ' )' : ''))
                                        )
                                        .append($('<div>')
                                            .append($('<input>')
                                                .attr('id', "txtFeature" + vObjName)
                                                .attr('type', "number")
                                                .data('PFeatureData', objFeatureValue)
                                                .keydown(function (e) {
                                                    if (e.keyCode != 110 && e.keyCode != 190) {
                                                        if (!NumberOnly(e)) return;
                                                    }
                                                })
                                            )
                                        )
                                    );

                                pLV.append(pItem);

                                pLV.trigger("create");

                                $('#txtFeature' + vObjName).val(((objFeatureValue != null || objFeatureValue != undefined) ? objFeatureValue.Value : ""));

                                LoadPIMUNSPSCCommFeatureList(intCurIndex + 1, vRows, vValueRows);
                            }

                        }

                    }
                });


            };

            function GetFeatureValue(strFeatureID, vValueRows) {

                if (vValueRows.length > 0) {

                    for (var intCntrVal = 0; intCntrVal < vValueRows.length; intCntrVal++) {

                        if (vValueRows[intCntrVal].CommodityFeatureID == strFeatureID) {
                            return vValueRows[intCntrVal];
                        }
                    }

                }

                return null;
            }

            function ClearPIMProductFields() {

                $("#txtPIMProductID").val('');

                $("#txtPIMProductOrganizationID").val('');
                $("#btnPIMProductOrganization").text('Please Select').button('refresh');

                $("#txtPIMProductNo").val('');
                $("#txtPIMProductName").val('');
                $('#chkPIMProductIsActive').attr("checked", true).checkboxradio("refresh");

                $("#txtPIMProductFamilyID").val('');
                $("#btnPIMProductFamily").text('Please Select').button('refresh');

                $("#txtPIMProductUNSPSCSegment").val('');
                $("#txtPIMProductUNSPSCFamily").val('');
                $("#txtPIMProductUNSPSCClass").val('');
                $("#txtPIMProductUNSPSCCommIDOrig").val('');
                $("#txtPIMProductUNSPSCCommID").val('');
                $("#btnPIMProductUNSPSCCommdity").text('Please Select').button('refresh');

            }

            function SavePIMProductInfo() {
                var pData = [];
                var pDataFeature = [];

                var pUNSPSCComm = {};
                pUNSPSCComm.PIMUNSPSCCommodityID = $('#txtPIMProductUNSPSCCommID').val();

                AjaxSend({
                    url: "PIMUNSPSCCommodityFeature/getUNSPSCCommFeatureList",
                    data: pUNSPSCComm,
                    success: function (vDataFeature) {

                        //Collect Feature Values
                        if (vDataFeature.PIMUNSPSCCommFeatureData.Rows.length > 0) {
                            for (i = 0; i < vDataFeature.PIMUNSPSCCommFeatureData.Rows.length; i++) {

                                var vObjName = vDataFeature.PIMUNSPSCCommFeatureData.Rows[i].FeatureID.replace(" ", "");
                                var vID = "";
                                var vValue = "";

                                if (vDataFeature.PIMUNSPSCCommFeatureData.Rows[i].FeatureType == "Comment/Essay Box" ||
                                    vDataFeature.PIMUNSPSCCommFeatureData.Rows[i].FeatureType == "Textbox" ||
                                    vDataFeature.PIMUNSPSCCommFeatureData.Rows[i].FeatureType == "Textbox (Numeric)" ||
                                    vDataFeature.PIMUNSPSCCommFeatureData.Rows[i].FeatureType == "Textbox (Decimal)") {

                                    if ($("#txtFeature" + vObjName).data('PFeatureData') != null &&
                                       $("#txtFeature" + vObjName).data('PFeatureData') != undefined) {

                                        vID = $("#txtFeature" + vObjName).data('PFeatureData').ID;
                                    }

                                    vValue = $("#txtFeature" + vObjName).val();

                                } else if (vDataFeature.PIMUNSPSCCommFeatureData.Rows[i].FeatureType == "Date Only") {

                                    if ($("#selFeature" + vObjName + "Month").data('PFeatureData') != null &&
                                       $("#selFeature" + vObjName + "Month").data('PFeatureData') != undefined) {

                                        vID = $("#selFeature" + vObjName + "Month").data('PFeatureData').ID;
                                    }

                                    vValue = $("#selFeature" + vObjName + "Month").val() + "/" + $("#selFeature" + vObjName + "Day").val() + "/" + $("#selFeature" + vObjName + "Year").val();

                                } else if (vDataFeature.PIMUNSPSCCommFeatureData.Rows[i].FeatureType == "Time Only") {

                                    if ($("#selFeature" + vObjName + "Hour").data('PFeatureData') != null &&
                                       $("#selFeature" + vObjName + "Hour").data('PFeatureData') != undefined) {

                                        vID = $("#selFeature" + vObjName + "Hour").data('PFeatureData').ID;
                                    }

                                    vValue = $("#selFeature" + vObjName + "Hour").val() + ":" + $("#selFeature" + vObjName + "Min").val() + " " + $("#selFeature" + vObjName + "AMPM").val();

                                } else if (vDataFeature.PIMUNSPSCCommFeatureData.Rows[i].FeatureType == "Date and Time") {

                                    if ($("#selFeature" + vObjName + "Month").data('PFeatureData') != null &&
                                       $("#selFeature" + vObjName + "Month").data('PFeatureData') != undefined) {

                                        vID = $("#selFeature" + vObjName + "Month").data('PFeatureData').ID;
                                    }

                                    vValue = $("#selFeature" + vObjName + "Month").val() + "/" + $("#selFeature" + vObjName + "Day").val() + "/" + $("#selFeature" + vObjName + "Year").val() + " " + $("#selFeature" + vObjName + "Hour").val() + ":" + $("#selFeature" + vObjName + "Min").val() + " " + $("#selFeature" + vObjName + "AMPM").val();

                                } else if (vDataFeature.PIMUNSPSCCommFeatureData.Rows[i].FeatureType == "Multiple Option") {
                                    var vID = "";
                                    var strMultiValue = "";
                                    if (vDataFeature.PIMUNSPSCCommFeatureData.Rows[i].DisplayFormat == "Radio Button") {

                                        var rdoDone = false;
                                        var r = 0;
                                        while ($("#rdoFeature" + vObjName + r) != undefined && !rdoDone) {
                                            if ($("#rdoFeature" + vObjName + r).is(":checked")) {

                                                if ($("#rdoFeature" + vObjName + r).data('PFeatureData') != null &&
                                                   $("#rdoFeature" + vObjName + r).data('PFeatureData') != undefined) {

                                                    vID = $("#rdoFeature" + vObjName + r).data('PFeatureData').ID;
                                                }

                                                vValue = $("#rdoFeature" + vObjName + r).data('data').ID;
                                                rdoDone = true;
                                            }
                                            r = r + 1;
                                        }
                                    } else if (vDataFeature.PIMUNSPSCCommFeatureData.Rows[i].DisplayFormat == "Drop Down List") {
                                        strMultiValue = $("#selFeature" + vObjName).val();

                                        if ($("#selFeature" + vObjName).data('PFeatureData') != null &&
                                           $("#selFeature" + vObjName).data('PFeatureData') != undefined) {

                                            vID = $("#selFeature" + vObjName).data('PFeatureData').ID;
                                        }

                                        vValue = $("#selFeature" + vObjName).val();
                                    }

                                }

                                pDataFeature[i] = {
                                    ID: vID,
                                    CommodityFeatureID: vDataFeature.PIMUNSPSCCommFeatureData.Rows[i].FeatureID,
                                    Value: vValue
                                };

                            }
                        }


                        pData = {
                            PIMProductID: $.trim($("#txtPIMProductID").val()),
                            OrganizationID: $.trim($("#txtPIMProductOrganizationID").val()),
                            ProductNo: $.trim($("#txtPIMProductNo").val()),
                            ProductName: $.trim($("#txtPIMProductName").val()),
                            ProductFamilyID: $.trim($("#txtPIMProductFamilyID").val()),
                            UNSPSCCommodityIDOrig: $.trim($("#txtPIMProductUNSPSCCommIDOrig").val()),
                            UNSPSCCommodityID: $.trim($("#txtPIMProductUNSPSCCommID").val()),
                            IsActive: ($("#chkPIMProductIsActive").is(":checked") ? 1 : 0),
                            ProductFeatureList: pDataFeature
                        };


                        AjaxSend({
                            url: "PIMProduct/CheckUniqueProduct",
                            data: pData,
                            success: function (vData) {

                                if (vData.PIMProductData.Rows.length > 0) {
                                    MessageBox("Product already exist.",
                                        "Product",
                                        [
                                            { Text: 'Ok' }
                                        ]
                                    );
                                }
                                else {
                                    AjaxSend({
                                        url: "PIMProduct/SaveProductInfo",
                                        data: pData,
                                        success: function (vSaveData) {

                                            $("#txtPIMProductID").val(vSaveData.PIMProductData || '');

                                            $.mobile.changePage("#" + $(window.pageOrig).attr('id'));

                                        }
                                    });

                                }

                            }
                        });


                    }
                });





            };


        });
    });