require([
    'common',
    GetPathFormat('/Scripts3/Browser/LibraryPIMMeasure.js?crc='),
    ], function () {
        var recno, data;
        $(function () {

            var arExcelMobizMatch = new Array();
            var intExcelMobizMatchCntr = 0;
            var intExcelMobizMatchIndex = 0;

            var intExcelMobizMatchExcelCol = 0;
            var intExcelMobizMatchMobizColID = 1;
            var intExcelMobizMatchMobizCol = 2;
            var intExcelMobizMatchDataType = 3;

            var strExcelMobizDataMatchingCol = '';
            var arExcelMobizDataMatching = new Array();
            var intExcelMobizDataMatchingCntr = 0;
            var intExcelMobizDataMatchingIndex = 0;

            var intExcelMobizDataMatchingCol = 0;
            var intExcelMobizDataMatchingMobizColFeatureID = 1;
            var intExcelMobizDataMatchingMobizColID = 2;
            var intExcelMobizDataMatchingMobizCol = 3;

            var TemplateData;
            var IsTempateExist = false;
            var intTempateIndex = false;
            var TemplateUnMatchCol;
            var mUploaderKey;

            $(document).ready(function () {
                //getPIMExcelColumns();
                $("#OrgID").val(sessionStorage.OrgID);
                $('body').on('submit', '#formupload', function (e) {
                    e.preventDefault();

                    //grab form data
                    var formData = new FormData($(this)[0]);
                    mUploaderKey = NewID();
                    formData.delete('OrgID'); //idunno ngano dili makuha ang value sa OrgID sa form, maong giappend nalang.
                    formData.append('UploaderKey', mUploaderKey);
                    formData.append('OrgID', sessionStorage.OrgID);
                    formData.append('selParseProductName', CInt2($('#selParseProductName').is(':checked')));
                    formData.append('selDoNotOverwriteBomLandedCost', CInt2($('#selDoNotOverwriteBomLandedCost').is(':checked')));
                    
                    $("#OrgID").val(sessionStorage.OrgID);

                    Bs.Loading('show', 'Please wait ...');

                    setTimeout(function () {
                        $.ajax({
                            url: '/PIMUpload/Index',
                            type: 'POST',
                            data: formData,
                            async: false,
                            cache: false,
                            contentType: false,
                            processData: false,
                            success: function (vData) {
                                getPIMExcelColumns(vData);
                            }
                        });
                    }, 1000)                    
                })
                .on('click', '#lnkDownloadGenTemplate', function (e) {
                    e.preventDefault();
                    window.location = MergeUrlParameters({
                        OrgID: sessionStorage["OrgID"]
                    }, '/PIMTemplate/DownloadTemplate');
                })
                .on('click', '#btnPIMUploadBackToHome', function (e) {
                    e.preventDefault();

                    //window.open(GetPathFormat("/People/PeopleMenu"), "_self");
                    if (CBool(sessionStorage.IsUploadPIMViaPriceCanvass)) {
                        RemoveBreadCrumb('Upload PIM Template', '.ItemsBreadCrumbs');
                        $('#divPriceCanvassPage').pageshow();
                        return;
                    }

                    $('#tblBody').empty();
                    window.location.hash = 'LibraryPage';
                    LibraryTable.PIMPage();
                })
                .on('keydown', '#txtSearchKeyword', function (e) {
                    if (e.keyCode == 13) {
                        SearchPIMFeaturePopUp($(this).val());
                    }
                })
                .on('click', '#btnOrgPIMUpload', function (e) {
                    e.preventDefault();
                    //                if ($.trim($("#txtPIMUploadTemplateName").val()) == "") {
                    //                    MessageBox("Please enter PIM template name.",
                    //                            "PIM Template Name",
                    //                            [
                    //                                { Text: 'Ok' }
                    //                            ]
                    //                    );
                    //                    return;
                    //                }

                    UploadPIMTemplate();
                })
                .on('click', '#btnOrgPIMUploadCancel', function (e) {
                    e.preventDefault();

                    //window.open(GetPathFormat("/PIMUpload") + "?OrgID=" + sessionStorage.OrgID, "_self");
                    //$.mobile.changePage("#pagePIMUpload");
                    $('#pagePIMUpload').show();
                    $('#pagePIMUploadAnalyze').hide();
                    $('#pagePIMExcelColDataMatching').hide();
                    $('#divPIMUploadPromptEmail').hide();
                    $('#divPIMUploadPromptPriceLevelDiscount').hide();
                })
                .on('click', '#btnOrgPIMUploadMatchDataOk', function (e) {
                    e.preventDefault();

                    SaveMobizColAKA();
                })
                .on('click', '#btnOrgPIMUploadMatchDataCancel', function (e) {
                    e.preventDefault();
                    strExcelMobizDataMatchingCol = '';
                    //$.mobile.changePage('#pagePIMUploadAnalyze');
                    $('#pagePIMUpload').hide();
                    $('#pagePIMUploadAnalyze').show();
                    $('#pagePIMExcelColDataMatching').hide();
                    $('#divPIMUploadPromptEmail').hide();
                    $('#divPIMUploadPromptPriceLevelDiscount').hide();
                })
                .on('click', '#btnPIMUploadEmailOK', function (e) {
                    e.preventDefault();

                    if ($.trim($("#txtPIMUploadEmailAddress").val()) == "") {
                        NewMessageBox("Please enter your email address.",
                                "Email Address",
                                [
                                    { Text: 'Ok'
                                    }
                                ]
                        );
                        return;
                    }

                    if (!VerifyEmail()) {
                        NewMessageBox("Please enter valid email address.",
                                "Invalid email",
                                [
                                    { Text: 'Ok',
                                        Click: function () {
                                            return;
                                        }
                                    }
                                ]
                        );
                    }

                    SendPIMUploadEmail();
                })
                .on('click', '#btnPIMUploadEmailCancel', function (e) {
                    e.preventDefault();
                    strExcelMobizDataMatchingCol = '';
                    //$.mobile.changePage('#pagePIMUploadAnalyze');
                    $('#pagePIMUpload').hide();
                    $('#pagePIMUploadAnalyze').show();
                    $('#pagePIMExcelColDataMatching').hide();
                    $('#divPIMUploadPromptEmail').hide();
                    $('#divPIMUploadPromptPriceLevelDiscount').hide();
                })
                .on('click', '#lnkPIMUploadTemplateFailedDL', function (e) {
                    e.preventDefault();
                    window.location = GetPathFormat("/PIMUpload/DownloadFailedTemplate?OrgID=" + $.trim(sessionStorage.OrgID));
                })
                .on('click', '#lnkPIMUploadAnalyzeTemplateFailedDL', function (e) {
                    e.preventDefault();
                    window.location = GetPathFormat("/PIMUpload/DownloadFailedTemplate?OrgID=" + $.trim(sessionStorage.OrgID));
                })
            });

            function getPIMExcelColumns(vData) {
                var data = JSON.parse(vData);

                try { // 2016_01_15__16_30_31_8727 added by arvin                    
                    var pJson = JSON.parse(vData);
                    pJson = pJson.d
                    if (!proj.HandleSessionExpired(pJson))
                        return;

                    if (pJson.Result == 'Empty') {
                        ShowMessage('Please choose an Excel file first.');
                        return;
                    }
                    if (pJson.Error != null) {
                        ShowMessage(pJson.Error);
                        return;
                    }
                } catch (e) {
                    ShowMessage(e);
                    return;
                }

                var pDiv = $('#divPIMUploadAnalyze');
                var pItem;
                var pData = [];
                var pParams = GetUrlParameters();

                $("#divFailedMessages").hide();

                pData = {
                    OrgID: $.trim(sessionStorage.OrgID),
                    IsSameProduct: location.hash == '#SameProduct' ? 1 : 0,
                    UploaderKey: mUploaderKey
                };

                AjaxSend({
                    url: 'PIMUpload/IsPIMExcelUploaded',
                    data: pData,
                    success: function (vData) {

                        IsTempateExist = false;
                        if (data.d.Result == 'NewTemplate') {
                            if (vData.PIMExcelUpload.Rows.length > 0) {
                                Bs.Loading('hide');
                                //$.mobile.changePage('#pagePIMUploadAnalyze');
                                $('#pagePIMUpload').hide();
                                $('#pagePIMUploadAnalyze').show();
                                $('#pagePIMExcelColDataMatching').hide();
                                $('#divPIMUploadPromptEmail').hide();
                                $('#divPIMUploadPromptPriceLevelDiscount').hide();
                                pDiv.html('');

                                //Header
                                pItem = $('<div>')
                                            .addClass('row')
                                            .append($('<div>')
                                                .addClass('col-4')
                                                .append($('<div>')
                                                    .attr('style', 'height:60px; font-weight:bold; text-align:center;')
                                                    .text('Excel Column')
                                                )
                                            )

                                            .append($('<div>')
                                                .addClass('col-4')
                                                .append($('<div>')
                                                    .attr('style', 'height:60px; font-weight:bold; text-align:center;')
                                                    .text('Mobiz Column')
                                                )
                                            )

                                            .append($('<div>')
                                                .addClass('col-4')
                                                .append($('<div>')
                                                    .attr('style', 'height:60px; font-weight:bold; text-align:center;')
                                                    .text('Data Type')
                                                )
                                            );
                                pDiv.append(pItem);

                                //Details
                                intExcelMobizMatchCntr = vData.PIMExcelUpload.Rows.length;
                                for (var p = 0; p <= vData.PIMExcelUpload.Rows.length - 1; p++) {

                                    //Store in Array
                                    arExcelMobizMatch = new Array();
                                    arExcelMobizMatch[intExcelMobizMatchExcelCol] = vData.PIMExcelUpload.Rows[p].ExcelColumn;
                                    arExcelMobizMatch[intExcelMobizMatchMobizCol] = '';
                                    arExcelMobizMatch[intExcelMobizMatchMobizColID] = '';
                                    arExcelMobizMatch[intExcelMobizMatchDataType] = '';

                                    sessionStorage["PIMExcelUpload-" + p] = JSON.stringify(arExcelMobizMatch);

                                    pItem = $('<div>')
                                            .addClass('row align-items-center mb-3')
                                            .append($('<div>')
                                                .addClass('col-4')
                                                .append($('<div>')
                                                    .text(vData.PIMExcelUpload.Rows[p].ExcelColumn)
                                                )
                                            )

                                            .append($('<div>')
                                                .addClass('col-4')
                                                .append($('<div>')
                                                    .append($('<button data-role="none" class="btn btn-secondary btn-block btn-lg" id="btnMobiz_' + p + '">')
                                                        .text("Please Select")
                                                        .data('recno', p)
                                                        .data('data', vData.PIMExcelUpload.Rows[p])
                                                        .click(function (e) {
                                                            e.preventDefault();
                                                            setExcelMobizMatch($(this).data('recno'), $(this).data('data'));
                                                        })
                                                    )
                                                )
                                            )

                                            .append($('<div>')
                                                .addClass('col-4')
                                                .append($('<div>')
                                                    .append($('<select data-role="none" class="form-select form-select-lg" id="selMobizType_' + p + '">')
                                                        .append(new Option("Please Select", ""))
                                                        .append(new Option("Comment/Essay Box", "Comment/Essay Box"))
                                                        .append(new Option("Date", "Date Only"))
                                                        .append(new Option("Time", "Time Only"))
                                                        .append(new Option("Date and Time", "Date and Time"))
                                                        .append(new Option("Multiple Option", "Multiple Option"))
                                                        .append(new Option("Textbox", "Textbox"))
                                                        .append(new Option("Textbox (Numeric)", "Textbox (Numeric)"))
                                                        .append(new Option("Textbox (Decimal)", "Textbox (Decimal)"))
                                                    )
                                                )
                                            );

                                    pDiv.append(pItem);

                                };

                                //pDiv.trigger('create');
                            }
                        } else if (data.d.Result == 'TemplateExist') {
                            if (vData.PIMExcelUpload.Rows.length > 0) {
                                IsTempateExist = true;
                                TemplateData = undefined;
                                intTempateIndex = 0;
                                TemplateData = vData.PIMExcelUpload;
                                CheckExcelColData();
                            } else {
                                Bs.Loading('hide');
                            }
                        } else if (data.d.Result == 'Success') {
                            Bs.Loading('hide');
                            NewMessageBox("Excel Data successfully uploaded.",
                                    "Excel Data",
                                    [
                                        { Text: 'Ok',
                                            Click: function () {
                                                $('#pagePIMUpload').show();
                                                $('#pagePIMUploadAnalyze').hide();
                                                $('#pagePIMExcelColDataMatching').hide();
                                                $('#divPIMUploadPromptEmail').hide();
                                                $('#divPIMUploadPromptPriceLevelDiscount').hide();
                                            }
                                        }
                                    ]
                            );
                        }
                    }
                });
            }

            function setExcelMobizMatch(vRecNo, vData) {
                recno = vRecNo;
                data = vData;

                BrowsePopup('LibraryPIMFeature', 'PIM Feature',
                    [
                        {
                            DisplayText: [{ Label: 'Name', Column: 'Feature' }, { Label: 'Type', Column: 'FeatureType'}],
                            Click: function (vData) {
                                $("#btnMobiz_" + recno).text(vData.Feature);//.button('refresh');
                                $("#selMobizType_" + recno).val(vData.FeatureType);//.selectmenu('refresh');

                                if (vData.Feature != "No Matching Column") {
                                    $("#selMobizType_" + recno).attr("disabled", true);
                                }

                                arExcelMobizMatch = new Array();
                                arExcelMobizMatch[intExcelMobizMatchExcelCol] = data.ExcelColumn;
                                arExcelMobizMatch[intExcelMobizMatchMobizColID] = vData.ID;
                                arExcelMobizMatch[intExcelMobizMatchMobizCol] = vData.Feature;
                                arExcelMobizMatch[intExcelMobizMatchDataType] = vData.FeatureType;

                                sessionStorage["PIMExcelUpload-" + recno] = JSON.stringify(arExcelMobizMatch);

                                SetMatchingDataLibrary(data.ExcelColumn, vData.ID, vData.Feature);
                            }
                        }
                    ]
                )

                //$('#panSearchLibrary').popup('open');
                //$('#TitleLibrary').text('PIM Feature');
                //$('#txtSearchKeyword').val('').focus().attr('placeholder', 'Search Here');                
                //$('#lstOfLibrary').empty();                

                //                Browser.LibraryPIMFeature.Click = function (e) {
                //                    $("#btnMobiz_" + vRecNo).text(e.Feature);//.button('refresh');
                //                    $("#selMobizType_" + vRecNo).val(e.FeatureType);//.selectmenu('refresh');

                //                    if (e.Feature != "No Matching Column") {
                //                        $("#selMobizType_" + vRecNo).attr("disabled", true);
                //                    }

                //                    arExcelMobizMatch = new Array();
                //                    arExcelMobizMatch[intExcelMobizMatchExcelCol] = vData.ExcelColumn;
                //                    arExcelMobizMatch[intExcelMobizMatchMobizColID] = e.ID;
                //                    arExcelMobizMatch[intExcelMobizMatchMobizCol] = e.Feature;
                //                    arExcelMobizMatch[intExcelMobizMatchDataType] = e.FeatureType;

                //                    sessionStorage["PIMExcelUpload-" + vRecNo] = JSON.stringify(arExcelMobizMatch);

                //                    SetMatchingDataLibrary(vData.ExcelColumn, e.ID, e.Feature);
                //                };
                //                Browser.LibraryPIMFeature.Show();

            }

            function SearchPIMFeaturePopUp(s) {
                var pData = {};

                pData.keywords = s;

                AjaxSend({
                    url: 'Browser/LibraryPIMFeature',
                    data: pData,
                    success: function (vData) {
                        $('#lstOfLibrary').empty();

                        if (vData.Results.Rows.length > 0) {
                            for (var p = 0; p < vData.Results.Rows.length; p++) {
                                $('#lstOfLibrary').append($('<li>')
                                    .append($('<a>')
                                        .attr('ID', vData.Results.Rows[p].ID)
                                        .append($('<br><p style="font-weight:bold;">')
                                            .text(vData.Results.Rows[p].Feature)
                                        )
                                        .append($('<p>')
                                            .text(vData.Results.Rows[p].FeatureType)
                                        )
                                        .data('data', vData.Results.Rows[p])
                                        .click(function (e) {
                                            $("#btnMobiz_" + recno).text($(this).data('data').Feature);//.button('refresh');
                                            $("#selMobizType_" + recno).val($(this).data('data').FeatureType);//.selectmenu('refresh');

                                            if (e.Feature != "No Matching Column") {
                                                $("#selMobizType_" + recno).attr("disabled", true);
                                            }

                                            arExcelMobizMatch = new Array();
                                            arExcelMobizMatch[intExcelMobizMatchExcelCol] = data.ExcelColumn;
                                            arExcelMobizMatch[intExcelMobizMatchMobizColID] = $(this).data('data').ID;
                                            arExcelMobizMatch[intExcelMobizMatchMobizCol] = $(this).data('data').Feature;
                                            arExcelMobizMatch[intExcelMobizMatchDataType] = $(this).data('data').FeatureType;

                                            sessionStorage["PIMExcelUpload-" + recno] = JSON.stringify(arExcelMobizMatch);

                                            SetMatchingDataLibrary(data.ExcelColumn, $(this).data('data').ID, $(this).data('data').Feature);
                                            //$('#panSearchLibrary').popup('close');
                                        })
                                    ));
                            }

                        }
                        else {
                            $('#lstOfLibrary').append($('<li>').text('No Result(s) Found'));
                        }

                        $('#lstOfLibrary')//.listview('refresh');
                    }
                });
            }

            function UploadPIMTemplate() {
                var pData = [];
                var pDataAKA = [];
                var arExcelMobizDataMatchingAll = new Array()
                var intExcelMobizDataMatchingAll = 0;
                var intIndex;
                var pDiv = $('#divFailedMessages');
                var blnIsAllColMatched = true;

                //Check for Excel Columns = Mobiz Data Column
                intIndex = 0;
                for (var i = 0; i < intExcelMobizMatchCntr; i++) {

                    if (sessionStorage["PIMExcelUpload-" + i] != undefined) {

                        arExcelMobizMatch = new Array();
                        arExcelMobizMatch = JSON.parse(sessionStorage["PIMExcelUpload-" + i]);

                        pData[intIndex] = {
                            ColumnName: arExcelMobizMatch[intExcelMobizMatchExcelCol],
                            PIMFeatureID: arExcelMobizMatch[intExcelMobizMatchMobizColID],
                            PIMFeature: arExcelMobizMatch[intExcelMobizMatchMobizCol],
                            FeatureType: arExcelMobizMatch[intExcelMobizMatchDataType]
                        };

                        if (arExcelMobizMatch[intExcelMobizMatchMobizColID] == undefined ||
                            $.trim(arExcelMobizMatch[intExcelMobizMatchMobizColID]) == "") {

                            blnIsAllColMatched = false;
                        }

                        if ($("#selMobizType_" + i).val() == undefined || $("#selMobizType_" + i).val() == "") {
                            blnIsAllColMatched = false;
                        }


                        intIndex = intIndex + 1;

                    }
                }

                if (!blnIsAllColMatched) {
                    NewMessageBox("Please specify matching Mobiz data field and data type for every excel data column.",
                            "Analyze PIM Template",
                            [
                                { Text: 'Ok' }
                            ]
                    );
                    return;
                }

                //Check for Mobiz Data AKA
                intIndex = 0;
                for (var i = 0; i <= intExcelMobizMatchCntr; i++) {

                    if (sessionStorage["PIMExcelUpload-" + i] != undefined) {

                        arExcelMobizMatch = new Array();
                        arExcelMobizMatch = JSON.parse(sessionStorage["PIMExcelUpload-" + i]);

                        if (sessionStorage[arExcelMobizMatch[intExcelMobizMatchExcelCol] + "PIMExcelDataMatching"] != undefined) {
                            arExcelMobizDataMatchingAll = new Array();
                            arExcelMobizDataMatchingAll = JSON.parse(sessionStorage[arExcelMobizMatch[intExcelMobizMatchExcelCol] + "PIMExcelDataMatching"]);

                            //get row Count
                            intExcelMobizDataMatchingAll = 0;
                            if (sessionStorage[arExcelMobizMatch[intExcelMobizMatchExcelCol] + "PIMExcelDataMatchingRow"] != undefined) {
                                intExcelMobizDataMatchingAll = sessionStorage[arExcelMobizMatch[intExcelMobizMatchExcelCol] + "PIMExcelDataMatchingRow"]
                            }

                            for (var j = 0; j < intExcelMobizDataMatchingAll; j++) {

                                arExcelMobizDataMatching = new Array();
                                arExcelMobizDataMatching = arExcelMobizDataMatchingAll[j];

                                pDataAKA[intIndex] = {
                                    ColumnData: arExcelMobizDataMatching["ColumnData"],
                                    PIMFeatureID: arExcelMobizDataMatching["PIMFeatureID"],
                                    PIMFeatureDataID: arExcelMobizDataMatching["PIMFeatureDataID"]
                                };

                                intIndex = intIndex + 1;

                            }
                        }

                    }
                }

                pData = {
                    OrganizationID: $.trim(sessionStorage.OrgID),
                    PIMTemplateName: $.trim($("#txtPIMUploadTemplateName").val()),
                    PIMTemplateColumnList: pData,
                    PIMTemplateColumnAKAList: pDataAKA,
                    EnableParsing: $('#selParseProductName').val(),
                    DoNotOverwriteBomLandedCost: $('#selDoNotOverwriteBomLandedCost').val(),
                    BeginningBalanceDateAndTime: $('#dtBegBalanceDateTime').val(),
                    IsAsset: false, // added by arnie 81217 default to FALSE, theres no option in excel file to determine a row record as asset
                    UploaderKey: mUploaderKey,
                    IsFromPOModule: CBool(sessionStorage.IsFromPOModule)
                };

                //                AjaxSend({
                //                    url: "PIMUpload/CheckUniqueTemplateName",
                //                    data: pData,
                //                    success: function (vData) {

                //                        if (vData.PIMExcelUpload.Rows.length > 0) {
                //                            MessageBox("PIM template name already exist.",
                //                                "PIM Template Name",
                //                                [
                //                                    { Text: 'Ok',
                //                                        Click: function () {
                //                                            //$('.ui-dialog').dialog('close');
                //                                        }
                //                                    }
                //                                ]
                //                            );
                //                        }
                //                        else {

                Bs.Loading('show');

                AjaxSend({
                    url: "PIMUpload/UploadPIMExcelTemplate",
                    data: pData,
                    success: function (vSaveData) {

                        if (vSaveData.PIMExcelUpload == "Success") {

                            AjaxSend({
                                url: "PIMUpload/CheckExcelFileData",
                                data: pData,
                                success: function (vCheckData) {

                                    if (vCheckData.PIMExcelUpload == "") {

                                        //Check if has conflicting Price Level discount
                                        ////////                                        if (vCheckData.PriceDiscountDiff != "") {

                                        ////////                                            $('#selPIMUploadPriceLevelDiscount').empty();
                                        ////////                                            $('#selPIMUploadPriceLevelDiscount').append(new Option("Please Select", ""));
                                        ////////                                            for (var p = 0; p <= vCheckData.PriceDiscountList.Rows.length - 1; p++) {
                                        ////////                                                $('#selPIMUploadPriceLevelDiscount').append(new Option(vCheckData.PriceDiscountList.Rows[p].PriceLevelID || '', vCheckData.PriceDiscountList.Rows[p].PriceLevel || ''));
                                        ////////                                            };

                                        ////////                                            $.mobile.changePage('#divPIMUploadPromptPriceLevel');
                                        ////////                                            return;
                                        ////////                                        }

                                        Bs.Loading('show', 'This might take a few moments. Please wait while the system verifying the Excel file data...');

                                        //NewMessageBox("",
                                        //        "Verifying Excel File Data",
                                        //        [
                                        //            { Text: 'Ok',
                                        //                Click: function () {
                                        //                    $('#pagePIMUpload').hide();
                                        //                    $('#pagePIMUploadAnalyze').show();
                                        //                    $('#pagePIMExcelColDataMatching').hide();
                                        //                    $('#divPIMUploadPromptEmail').hide();
                                        //                    $('#divPIMUploadPromptPriceLevelDiscount').hide();
                                        //                }
                                        //            }
                                        //        ]
                                        //);

                                        AjaxSend({
                                            url: "PIMUpload/SaveExcelFileData",
                                            data: pData,
                                            success: function (vFilekData) {

                                                if (vFilekData.PIMExcelUpload == "Success") {
                                                    Bs.Loading('hide');
                                                    NewMessageBox("Excel Data successfully uploaded.",
                                                            "Excel Data",
                                                            [
                                                                { Text: 'Ok',
                                                                    Click: function () {
                                                                        $('#pagePIMUpload').show();
                                                                        $('#pagePIMUploadAnalyze').hide();
                                                                        $('#pagePIMExcelColDataMatching').hide();
                                                                        $('#divPIMUploadPromptEmail').hide();
                                                                        $('#divPIMUploadPromptPriceLevelDiscount').hide();
                                                                    }
                                                                }
                                                            ]
                                                    );

                                                } else {
                                                    if (vFilekData.PIMExcelUploadRequired != undefined) {
                                                        pDiv.html('');

                                                        for (var p = 0; p <= vFilekData.PIMExcelUploadRequired.Rows.length - 1; p++) {
                                                            pItem = $('<div>')
                                                                .append($('<p>')
                                                                    .attr("style", "white-space:normal; color: #FF0000;")
                                                                    .text(vFilekData.PIMExcelUpload.Rows[p].ExcelColumn)
                                                                )
                                                            pDiv.append(pItem);
                                                        };

                                                        pDiv.trigger('create');
                                                        $("#divFailedMessages").show();
                                                        Bs.Loading('hide');
                                                    } else if (vFilekData.PIMExcelUpload == 'Failed') {
                                                        Bs.Loading('hide');
                                                        NewMessageBox("Encountered error while uploading data from Excel File.",
                                                                "Excel Data",
                                                                [
                                                                    { Text: 'Ok',
                                                                        Click: function () {
                                                                            $('#pagePIMUpload').show();
                                                                            $('#pagePIMUploadAnalyze').hide();
                                                                            $('#pagePIMExcelColDataMatching').hide();
                                                                            $('#divPIMUploadPromptEmail').hide();
                                                                            $('#divPIMUploadPromptPriceLevelDiscount').hide();
                                                                        }
                                                                    }
                                                                ]
                                                        );
                                                    } else if (vFilekData.PIMExcelUpload != '') {

                                                        setTimeout(FollowUpRequestFromServer, 5000);
                                                        Bs.Loading('show', 'Please do not reload the page while uploading your data <br>' + vFilekData.PIMExcelUpload);

                                                    }
                                                }
                                            }
                                        });

                                    } else if (vCheckData.PIMExcelUpload == "NeedRequiredFields") {

                                        //$('.ui-dialog').dialog('close');

                                        if (vCheckData.PIMExcelUploadRequired != undefined) {

                                            pDiv.html('');
                                            for (var p = 0; p <= vCheckData.PIMExcelUploadRequired.Rows.length - 1; p++) {
                                                pItem = $('<div>')
                                            .append($('<p>')
                                                .attr("style", "white-space:normal; color: #FF0000;")
                                                .text(vCheckData.PIMExcelUploadRequired.Rows[p].ExcelColumn)
                                            )
                                                pDiv.append(pItem);
                                            };

                                            pDiv.trigger('create');
                                            pDiv.show();
                                        }

                                    } else {
                                        $("#divPIMUploadAnalyzeFailed").show();
                                    }
                                }
                            });
                        } else {
                            Bs.Loading('hide');
                            if (vSaveData.PIMExcelUploadNM != undefined) {
                                if (vSaveData.PIMExcelUploadNM.Rows.length > 0) {

                                    TemplateUnMatchCol = vSaveData.PIMExcelUploadNM;

                                    NewMessageBox("Some excel column doesn't have equivalent Mobiz data fields. The unmatched data fields will have to be resolved by the Mobiz technical team before you can upload the excel file again.",
                                        "Unmatched Excel Columns",
                                        [
                                            { Text: 'Ok',
                                                Click: function () {
                                                    $('#pagePIMUpload').hide();
                                                    $('#pagePIMUploadAnalyze').hide();
                                                    $('#pagePIMExcelColDataMatching').hide();
                                                    $('#divPIMUploadPromptEmail').show();
                                                    $('#divPIMUploadPromptPriceLevelDiscount').hide();
                                                }
                                            }
                                        ]
                                    );
                                }
                            }
                        }



                    }
                });

                //                        }

                //                    }
                //                });

            }

            function CheckExcelColData() {
                var pData = [];
                var pDataAKA = [];
                var intIndex;
                var arExcelMobizDataMatchingAll = new Array()
                var intExcelMobizDataMatchingAll = 0;
                var pDiv = $('#divFailedMessagesMain');

                if (IsTempateExist) {
                    if (TemplateData.Rows[intTempateIndex] != undefined) {
                        SetMatchingDataLibrary(TemplateData.Rows[intTempateIndex].ExcelColumn, TemplateData.Rows[intTempateIndex].FeatureID, TemplateData.Rows[intTempateIndex].Feature);
                    } else {
                        Bs.Loading('show', 'This might take a few moments. Please wait while the system verifying the Excel file data...');
                        //NewMessageBox("This might take a few moments. Please wait while the system verifying the Excel file data...",
                        //        "Verifying Excel File Data",
                        //        [
                        //            { Text: 'Ok',
                        //                Click: function () {
                        //                    $('#pagePIMUpload').show();
                        //                    $('#pagePIMUploadAnalyze').hide();
                        //                    $('#pagePIMExcelColDataMatching').hide();
                        //                    $('#divPIMUploadPromptEmail').hide();
                        //                    $('#divPIMUploadPromptPriceLevelDiscount').hide();
                        //                }
                        //            }
                        //        ]
                        //);

                        //Check for Mobiz Data AKA
                        intIndex = 0;
                        for (var i = 0; i <= TemplateData.Rows.length - 1; i++) {
                            if (sessionStorage[TemplateData.Rows[i].ExcelColumn + "PIMExcelDataMatching"] != undefined) {
                                arExcelMobizDataMatchingAll = new Array();
                                arExcelMobizDataMatchingAll = JSON.parse(sessionStorage[TemplateData.Rows[i].ExcelColumn + "PIMExcelDataMatching"]);

                                //get row Count
                                intExcelMobizDataMatchingAll = 0;
                                if (sessionStorage[TemplateData.Rows[i].ExcelColumn + "PIMExcelDataMatchingRow"] != undefined) {
                                    intExcelMobizDataMatchingAll = sessionStorage[TemplateData.Rows[i].ExcelColumn + "PIMExcelDataMatchingRow"]
                                }

                                for (var j = 0; j < intExcelMobizDataMatchingAll; j++) {

                                    arExcelMobizDataMatching = new Array();
                                    arExcelMobizDataMatching = arExcelMobizDataMatchingAll[j];

                                    pDataAKA[intIndex] = {
                                        ColumnData: arExcelMobizDataMatching["ColumnData"],
                                        PIMFeatureID: arExcelMobizDataMatching["PIMFeatureID"],
                                        PIMFeatureDataID: arExcelMobizDataMatching["PIMFeatureDataID"]
                                    };

                                    intIndex = intIndex + 1;

                                }
                            }
                        }

                        pData = {
                            OrganizationID: $.trim(sessionStorage.OrgID),
                            PIMTemplateColumnAKAList: pDataAKA,
                            EnableParsing: $('#selParseProductName').val(),
                            DoNotOverwriteBomLandedCost: $('#selDoNotOverwriteBomLandedCost').val(),
                            BeginningBalanceDateAndTime: $('#dtBegBalanceDateTime').val(),
                            UploaderKey: mUploaderKey
                        };


                        AjaxSend({
                            url: "PIMUpload/CheckExcelFileData",
                            data: pData,
                            success: function (vCheckData) {                                
                                if (vCheckData.PIMExcelUpload == "") {
                                    AjaxSend({
                                        url: "PIMUpload/SaveTemplateAKA",
                                        data: pData,
                                        success: function (vSaveData) {                                            
                                            if (vSaveData.PIMExcelUpload == "Success") {
                                                AjaxSend({
                                                    url: "PIMUpload/SaveExcelFileData",
                                                    data: pData,
                                                    success: function (vSaveData) {                                                        
                                                        if (CStr2(vSaveData.PIMExcelUploadError) != '') {

                                                            //$('#panOrgEmpMessage').popup('close');
                                                            //$('#TitleOrgEmp').text('Excel Data');

                                                            //$('#panOrgEmpMessage').popup('open');

                                                            //check if has matching product no
                                                            if (vSaveData.PIMExcelUploadError.toLowerCase().indexOf("already exists") >= 0) {
                                                                //                                                                $('#btnOKOrgEmp').hide();
                                                                //$('#MsgOrgEmp').html();
                                                                Bs.Loading('hide');
                                                                NewMessageBox('Uploading failed.<br/>' + vSaveData.PIMExcelUploadError +
                                                                '<br/><br/> Click <b>Reupload</b> if the products reflected above are the same products you want to upload.' +
                                                                ' <br/>If not (therefore you are trying to upload a completely different product), please ' +
                                                                ' click <b>Cancel</b> and edit your excel file. Product No. must be UNIQUE. <br/><br/>TIP: You may add suffix to the Product No. ' +
                                                                ' to make it unique. Ex. ABC001-1', '',
                                                                    [
                                                                        {
                                                                            Text: 'Reupload',
                                                                            Click: function () {
                                                                                $('#pagePIMUpload').show();
                                                                                $('#pagePIMUploadAnalyze').hide();
                                                                                $('#pagePIMExcelColDataMatching').hide();
                                                                                $('#divPIMUploadPromptEmail').hide();
                                                                                $('#divPIMUploadPromptPriceLevelDiscount').hide();

                                                                                location.hash = "SameProduct";
                                                                                $('#formupload').submit();
                                                                            }
                                                                        },
                                                                        {
                                                                            Text: 'Cancel',
                                                                            Click: function () {
                                                                                $('#pagePIMUpload').show();
                                                                                $('#pagePIMUploadAnalyze').hide();
                                                                                $('#pagePIMExcelColDataMatching').hide();
                                                                                $('#divPIMUploadPromptEmail').hide();
                                                                                $('#divPIMUploadPromptPriceLevelDiscount').hide();
                                                                                location.hash = "UniversalPage";
                                                                            }
                                                                        }
                                                                    ]
                                                                )
                                                            } else {
                                                                Bs.Loading('hide');
                                                                NewMessageBox('Uploading failed. ' + vSaveData.PIMExcelUploadError, '',
                                                                    [
                                                                        {
                                                                            Text: 'Ok',
                                                                            Click: function () {
                                                                                $('#pagePIMUpload').show();
                                                                                $('#pagePIMUploadAnalyze').hide();
                                                                                $('#pagePIMExcelColDataMatching').hide();
                                                                                $('#divPIMUploadPromptEmail').hide();
                                                                                $('#divPIMUploadPromptPriceLevelDiscount').hide();
                                                                            }
                                                                        }
                                                                    ]
                                                                )
                                                            }

                                                        } else {

                                                            location.hash = 'UniversalPage';

                                                            if (vSaveData.PIMExcelUpload == "Success") {
                                                                Bs.Loading('hide');
                                                                NewMessageBox("Excel Data successfully uploaded.",
                                                                        "Excel Data",
                                                                        [
                                                                            { Text: 'Ok',
                                                                                Click: function () {
                                                                                    $('#tblBody').empty();
                                                                                    window.location.hash = 'LibraryPage';
                                                                                    LibraryTable.PIMPage();
                                                                                }
                                                                            }
                                                                        ]
                                                                );
                                                            } else if (vSaveData.PIMExcelUpload == "NeedRequiredFields") {
                                                                Bs.Loading('hide');
                                                                //$('.ui-dialog').dialog('close');

                                                                if (vSaveData.PIMExcelUploadRequired != undefined) {

                                                                    pDiv.html('');
                                                                    for (var p = 0; p <= vSaveData.PIMExcelUploadRequired.Rows.length - 1; p++) {
                                                                        pItem = $('<div>')
                                                                                .append($('<p>')
                                                                                    .attr("style", "white-space:normal; color: #FF0000;")
                                                                                    .text(vSaveData.PIMExcelUploadRequired.Rows[p].ExcelColumn)
                                                                                )
                                                                        pDiv.append(pItem);
                                                                    };

                                                                    pDiv.trigger('create');
                                                                    pDiv.show();
                                                                }
                                                            } else if (vSaveData.PIMExcelUpload == 'Failed') {
                                                                Bs.Loading('hide');
                                                                NewMessageBox("Encountered error while uploading data from Excel File.",
                                                                        "Excel Data",
                                                                        [
                                                                            { Text: 'Ok',
                                                                                Click: function () {
                                                                                    $('#pagePIMUpload').show();
                                                                                    $('#pagePIMUploadAnalyze').hide();
                                                                                    $('#pagePIMExcelColDataMatching').hide();
                                                                                    $('#divPIMUploadPromptEmail').hide();
                                                                                    $('#divPIMUploadPromptPriceLevelDiscount').hide();
                                                                                }
                                                                            }
                                                                        ]
                                                                );

                                                            } else if (vSaveData.PIMExcelUpload != '') {

                                                                setTimeout(FollowUpRequestFromServer(), 5000);
                                                                Bs.Loading('show', 'Please do not reload the page while uploading your data <br>' + vSaveData.PIMExcelUpload);
                                                            } else {
                                                                Bs.Loading('hide');
                                                                NewMessageBox("Error saving excel data.",
                                                                        "Excel Data",
                                                                        [
                                                                            { Text: 'Ok' }
                                                                        ]
                                                                );
                                                            }
                                                        }
                                                    }
                                                });
                                            } else {
                                                Bs.Loading('hide');
                                                NewMessageBox("Error saving excel data.",
                                                        "Excel Data",
                                                        [
                                                            { Text: 'Ok' }
                                                        ]
                                                );
                                            }
                                        }
                                    });

                                } else if (vCheckData.PIMExcelUpload == "NeedRequiredFields") {
                                    Bs.Loading('hide');
                                    //$('.ui-dialog').dialog('close');

                                    if (vCheckData.PIMExcelUploadRequired != undefined) {

                                        pDiv.html('');
                                        for (var p = 0; p <= vCheckData.PIMExcelUploadRequired.Rows.length - 1; p++) {
                                            pItem = $('<div>')
                                                    .append($('<p>')
                                                        .attr("style", "white-space:normal; color: #FF0000;")
                                                        .text(vCheckData.PIMExcelUploadRequired.Rows[p].ExcelColumn)
                                                    )
                                            pDiv.append(pItem);
                                        };

                                        pDiv.trigger('create');
                                        pDiv.show();
                                    }

                                } else {
                                    Bs.Loading('hide');
                                    //$('.ui-dialog').dialog('close');
                                    $("#divPIMUploadFailed").show();
                                }

                            }
                        });

                    }
                }

            }

            function SetMatchingDataLibrary(vExcelColumn, vFeatureID, vFeature) {
                var pDiv = $('#divPIMUploadColDataMatch');

                var pData = [];
                pData = {
                    OrgID: $.trim(sessionStorage.OrgID),
                    ExcelColumn: vExcelColumn,
                    FeatureID: vFeatureID,
                    Feature: vFeature
                };

                AjaxSend({
                    url: "PIMUpload/GetDistinctColumnData",
                    data: pData,
                    success: function (vData) {
                        if (vData.PIMExcelUpload.Rows.length > 0) {
                            //$.mobile.changePage('#pagePIMExcelColDataMatching');
                            $('#pagePIMUpload').hide();
                            $('#pagePIMUploadAnalyze').hide();
                            $('#pagePIMExcelColDataMatching').show();
                            $('#divPIMUploadPromptEmail').hide();
                            $('#divPIMUploadPromptPriceLevelDiscount').hide();
                            pDiv.html('');

                            //Header
                            pItem = $('<div>')
                                            .addClass('ui-grid-a')
                                            .append($('<div>')
                                                .addClass('ui-block-a')
                                                .append($('<div>')
                                                    .attr('style', 'height:60px; font-weight:bold; text-align:center;')
                                                    .text(vExcelColumn)
                                                )
                                            )

                                            .append($('<div>')
                                                .addClass('ui-block-b')
                                                .append($('<div>')
                                                    .attr('style', 'height:60px; font-weight:bold; text-align:center;')
                                                    .text(vFeature)
                                                )
                                            );
                            pDiv.append(pItem);

                            //Details
                            intExcelMobizDataMatchingCntr = vData.PIMExcelUpload.Rows.length;
                            for (var p = 0; p <= vData.PIMExcelUpload.Rows.length - 1; p++) {

                                //Store in Array
                                arExcelMobizDataMatching = new Array();
                                arExcelMobizDataMatching[intExcelMobizDataMatchingCol] = vData.PIMExcelUpload.Rows[p][vExcelColumn];
                                arExcelMobizDataMatching[intExcelMobizDataMatchingMobizColID] = vData.PIMExcelUpload.Rows[p][vFeature + "ID"];
                                arExcelMobizDataMatching[intExcelMobizDataMatchingMobizCol] = vData.PIMExcelUpload.Rows[p][vFeature];

                                sessionStorage[vExcelColumn + "PIMExcelDataMatching-" + p] = JSON.stringify(arExcelMobizDataMatching);

                                pItem = $('<div>')
                                            .addClass('ui-grid-a')
                                            .append($('<div>')
                                                .addClass('ui-block-a')
                                                .append($('<div>')
                                                    .addClass('ui-body ui-body-d')
                                                    .attr('style', 'height:60px;')
                                                    .append($('<legend>')
                                                        .attr('style', 'margin-top:25px;')
                                                        .text(vData.PIMExcelUpload.Rows[p][vExcelColumn])
                                                    )
                                                )
                                            )

                                            .append($('<div>')
                                                .addClass('ui-block-b')
                                                .append($('<div>')
                                                    .addClass('ui-body ui-body-d')
                                                    .attr('style', 'height:60px;')
                                                    .append($('<button id="btnMobizColData_' + p + '">')
                                                        .text((vData.PIMExcelUpload.Rows[p][vFeature] == '' ? "Please Select" : vData.PIMExcelUpload.Rows[p][vFeature]))
                                                        .data('recno', p)
                                                        .data('excelcol', vExcelColumn)
                                                        .data('data', vData.PIMExcelUpload.Rows[p])
                                                        .click(function (e) {
                                                            e.preventDefault();
                                                            setExcelMobizDataMatch($(this).data('recno'), $(this).data('excelcol'), vFeature, $(this).data('data'));
                                                        })
                                                    )
                                                )
                                            );

                                pDiv.append(pItem);

                            };

                            pDiv.trigger('create');
                        } else {
                            ///Check Excel Col data
                            intTempateIndex = intTempateIndex + 1;
                            CheckExcelColData();
                        }
                    }
                });

            }

            function setExcelMobizDataMatch(vRecNo, vExcelColumn, vFeature, vData) {

                if (vFeature == "Packing Size Unit") {
                    BrowsePopup('LibraryPIMMeasure', 'Packing Size Unit',
                        [
                            {
                                DisplayText: [{ Column: 'Measurement' }, { Label: 'Sign', Column: 'Sign'}],
                                Click: function (e) {
                                    $("#btnMobizColData_" + vRecNo).text(e.Measurement);//.button('refresh');

                                    strExcelMobizDataMatchingCol = vExcelColumn;
                                    arExcelMobizDataMatching = new Array();
                                    arExcelMobizDataMatching[intExcelMobizDataMatchingCol] = vData[vExcelColumn];
                                    arExcelMobizDataMatching[intExcelMobizDataMatchingMobizColFeatureID] = vData["FeatureID"];
                                    arExcelMobizDataMatching[intExcelMobizDataMatchingMobizColID] = e.ID;
                                    arExcelMobizDataMatching[intExcelMobizDataMatchingMobizCol] = e.Measurement;

                                    sessionStorage[vExcelColumn + "PIMExcelDataMatching-" + vRecNo] = JSON.stringify(arExcelMobizDataMatching);
                                }
                            }
                        ]
                    )
                }
            }

            function SaveMobizColAKA() {
                var pData = [];
                var intIndex;
                var blnIsAllColMatched = true;

                intIndex = 0;
                for (var i = 0; i <= intExcelMobizDataMatchingCntr; i++) {

                    if (sessionStorage[strExcelMobizDataMatchingCol + "PIMExcelDataMatching-" + i] != undefined) {

                        arExcelMobizDataMatching = new Array();
                        arExcelMobizDataMatching = JSON.parse(sessionStorage[strExcelMobizDataMatchingCol + "PIMExcelDataMatching-" + i]);

                        pData[intIndex] = {
                            ColumnData: arExcelMobizDataMatching[intExcelMobizDataMatchingCol],
                            PIMFeatureID: arExcelMobizDataMatching[intExcelMobizDataMatchingMobizColFeatureID],
                            PIMFeatureDataID: arExcelMobizDataMatching[intExcelMobizDataMatchingMobizColID]
                        };

                        if ($.trim(arExcelMobizDataMatching[intExcelMobizDataMatchingMobizColID]) == "" ||
                            arExcelMobizDataMatching[intExcelMobizDataMatchingMobizColID] == undefined) {

                            blnIsAllColMatched = false;
                        }

                        intIndex = intIndex + 1;

                    }
                }

                if (!blnIsAllColMatched || intIndex == 0) {
                    NewMessageBox("Please match all excel data to our Mobiz record.",
                            "Matching Excel Records",
                            [
                                { Text: 'Ok' }
                            ]
                    );
                    return;
                }
                sessionStorage[strExcelMobizDataMatchingCol + "PIMExcelDataMatching"] = JSON.stringify(pData);
                sessionStorage[strExcelMobizDataMatchingCol + "PIMExcelDataMatchingRow"] = intExcelMobizDataMatchingCntr;

                //Clear Session
                for (var i = 0; i <= intExcelMobizDataMatchingCntr; i++) {
                    if (sessionStorage[strExcelMobizDataMatchingCol + "PIMExcelDataMatching-" + i] != undefined) {
                        sessionStorage.removeItem(strExcelMobizDataMatchingCol + "PIMExcelDataMatching-" + i);
                    }
                }

                if (!IsTempateExist) {
                    //$.mobile.changePage('#pagePIMUploadAnalyze');
                    $('#pagePIMUpload').hide();
                    $('#pagePIMUploadAnalyze').show();
                    $('#pagePIMExcelColDataMatching').hide();
                    $('#divPIMUploadPromptEmail').hide();
                    $('#divPIMUploadPromptPriceLevelDiscount').hide();
                } else {
                    ///Check Excel Col data
                    intTempateIndex = intTempateIndex + 1;
                    CheckExcelColData();
                }

            }

            function GetMobizTypeIndex(vColumnText) {
                var intRet = 0;

                for (var i = 0; i <= intExcelMobizMatchCntr; i++) {

                    if ($("#lblMobiz_" + i).text() == vColumnText) {
                        intRet = i;
                    }
                }

                return intRet;
            }

            function SendPIMUploadEmail() {
                var pData = [];
                var pDataCol = [];
                var intIndex;
                var blnIsAllColMatched = true;

                for (var i = 0; i <= TemplateUnMatchCol.Rows.length - 1; i++) {
                    pDataCol[i] = {
                        ColumnName: TemplateUnMatchCol.Rows[i].ColumnName,
                        PIMFeatureID: TemplateUnMatchCol.Rows[i].FeatureID,
                        PIMFeature: TemplateUnMatchCol.Rows[i].Feature,
                        FeatureType: ($("#selMobizType_" + GetMobizTypeIndex(TemplateUnMatchCol.Rows[i].ColumnName)).val() == undefined ? TemplateUnMatchCol.Rows[i].FeatureType : $("#selMobizType_" + i).val())
                    };
                }

                pData = {
                    OrgID: $.trim(sessionStorage.OrgID),
                    EmailAddress: $.trim($("#txtPIMUploadEmailAddress").val()),
                    PIMTemplateColumnList: pDataCol
                };

                AjaxSend({
                    url: "PIMUpload/SendEmailUnMatchedCol",
                    data: pData,
                    success: function (vData) {
                        if (vData.PIMExcelUpload == "InvalidEmail") {
                            NewMessageBox("Please enter valid email address.",
                                    "Invalid email",
                                    [
                                        { Text: 'Ok'
                                        }
                                    ]
                            );
                        } else {
                            NewMessageBox("Email has been sent.",
                                    "Email Sent",
                                    [
                                        { Text: 'Ok',
                                            Click: function () {
                                                $('#pagePIMUpload').show();
                                                $('#pagePIMUploadAnalyze').hide();
                                                $('#pagePIMExcelColDataMatching').hide();
                                                $('#divPIMUploadPromptEmail').hide();
                                                $('#divPIMUploadPromptPriceLevelDiscount').hide();
                                            }
                                        }
                                    ]
                            );
                        }
                    }
                });
            }

            function VerifyEmail(vSalesPaymentEmail) {
                var strEmail;

                if ($.trim($("#txtPIMUploadEmailAddress").val()) == "") {
                    return false;
                } else {
                    strEmail = $.trim($("#txtPIMUploadEmailAddress").val()).split(",");

                    for (var i = 0; i < strEmail.length; i++) {
                        if (!IsEmail(strEmail[i])) {
                            return false;
                        }
                    }
                }

                return true;
            };

            function FollowUpRequestFromServer() {
                AjaxSend({
                    url: 'PIMUpload/UploadRequestFollowUp',
                    success: function (vData) {
                        Bs.Loading('hide');

                        if (vData.PIMExcelUpload == "Success") {
                            //isave ang mga brands and categories from excel file to libraries -- by arnie 12/10/2020 & requested by sir robert on 12/4/2020
                            AjaxSend({
                                url: 'Organization/SavePimBrandsToBrandsLibrary',
                                success: function (e) {
                                    if (e.Results != 'saved') {
                                        console.log('error while saving brands to its library')
                                    }
                                    AjaxSend({
                                        url: 'Organization/SavePimCategoriesToCategoriesLibrary',
                                        success: function (e) {
                                            //silent error or success lang ang importante naupload
                                            if (e.Results != 'saved') {
                                                console.log('error while saving categories to its library')
                                            }
                                        }
                                    })
                                }
                            })

                            //if from P.O. module
                            if (CBool(sessionStorage.IsFromPOModule)) {
                                //create PO
                                var pTxts = $('#tblBrowsePopup .qty');
                                var pData = [];
                                var pIndex;
                                var PIMPOProducts;

                                PIMPOProducts = vData.PIMPOProducts.Rows

                                for (var p = 0; p <= PIMPOProducts.length - 1; p++) {
                                    pData.push({
                                        ID: PIMPOProducts[p].ID,
                                        Qty: PIMPOProducts[p].POQuantity
                                    });
                                }

                                if (pData.length > 0) {
                                    $.mobile.showPageLoadingMsg();
                                    function Proceed() {
                                        var pParams = $.extend({
                                            Fail: function (vError) {
                                                $.mobile.hidePageLoadingMsg();
                                                NewMessageBox(vError);
                                                AfterProductSave();
                                            },
                                            Success: function () {
                                                pIndex++;
                                                if (pIndex <= pData.length - 1)
                                                    Proceed();
                                                else {
                                                    $.mobile.hidePageLoadingMsg();
                                                    AfterProductSave();
                                                }
                                            }
                                        }, pData[pIndex]);
                                        SavePreSelected(pParams);
                                    }
                                    pIndex = 0;
                                    Proceed();
                                }

                                NewMessageBox("Excel data successfully uploaded to Mobiz server and loaded to P.O. products table.",
                                       "PIM Upload"                                     
                               );
                            } else {
                                NewMessageBox("Excel Data successfully uploaded.",
                                        "Excel Data",
                                        [
                                            {
                                                Text: 'Ok',
                                                Click: function () {
                                                    if (CBool(sessionStorage.IsUploadPIMViaPriceCanvass)) {
                                                        RemoveBreadCrumb('Upload PIM Template', '.ItemsBreadCrumbs');
                                                        $('#divPriceCanvassPage').pageshow();
                                                        return;
                                                    }
                                                    $('#tblBody').empty();
                                                    window.location.hash = 'LibraryPage';
                                                    LibraryTable.PIMPage();
                                                }
                                            }
                                        ]
                                );
                            }
                        } else if ((vData.PIMExcelUpload == 'Failed')
                            || (vData.PIMExcelUploadError != null)) {
                            NewMessageBox("Encountered error while uploading data from Excel File.",
                                    "Excel Data",
                                    [
                                        { Text: 'Ok',
                                            Click: function () {
                                                $('#pagePIMUpload').show();
                                                $('#pagePIMUploadAnalyze').hide();
                                                $('#pagePIMExcelColDataMatching').hide();
                                                $('#divPIMUploadPromptEmail').hide();
                                                $('#divPIMUploadPromptPriceLevelDiscount').hide();
                                            }
                                        }
                                    ]
                            );
                        } else if (vData.PIMExcelUpload != '') {
                            setTimeout(FollowUpRequestFromServer, 5000);
                            Bs.Loading('show', 'Please do not reload the page while uploading your data <br>' + vData.PIMExcelUpload);
                        }
                    }
                });
            }

            $('#butDeleteColumnMapping').click(function() {
                NewMessageBox("Are you sure you want to delete all PIM upload column mappings",
                        "Delete Column Mappings",
                        [
                            { 
                                Text: 'Yes',
                                Click: function () {
                                    NewPromptUserAccess('Delete Column Mappings', {
                                        ForcePrompt: true,
                                        OnClickOKSuccess: function () {
                                            AjaxSend({
                                                url: 'PIMUpload/DeleteColumnMappings',
                                                success: function(vData) {
                                                    if (vData.Error)
                                                        NewMessageBox(vData.Error);
                                                    else if (CInt2(vData.AffectedCount) > 0)
                                                        NewMessageBox('Column mappings were successfully deleted.');
                                                    else
                                                        NewMessageBox('No column mappings to delete.');
                                                },
                                                error: function(vData) {
                                                    NewMessageBox(vData);
                                                }
                                            });
                                        }
                                    });
                                }
                            },
                            {
                                Text: 'No'
                            }
                        ]
                ); // END NewMessageBox("Encountered error while uploading data from Excel File.",
            });
        });

        function ShowMessage(vText) {
            Bs.Loading('hide');
            //$('#divPIMUploadFailed').text(vText).show();
            NewMessageBox(vText);
        }
    });