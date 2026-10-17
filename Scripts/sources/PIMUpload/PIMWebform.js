require([
    'common'
], function () {
    $(document).ready(function () {
        var arExcelMobizMatch = new Array();
        var intExcelMobizMatchCntr = 22;
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

        ClearFields();
        GetPackingSizeUnit();

        window.PimWebForm = {
            GetPackingSizeUnit: function () {
                GetPackingSizeUnit();
            },
            ClearFields: function () {
                ClearFields();
            }
        }

        $('body')
        .on('click', '#btnWebFormCancel', function () {
            if (CBool(sessionStorage.IsGlobalPIMWebForm)) { // for adding SKU globally, NewMessageBox.js
                $('#globalpopupPIMWebForm').hide();
                $('#divGlobalPIMWebForm').empty();
            } else {
                $('#tblBody').empty();
                window.location.hash = 'LibraryPage';
                LibraryTable.PIMPage();
            }            
        })
        .on('keydown', '#txtWebFormCostFromVendor', function(e){
            AccountingNumberOnly($(this).val());
        })
        .on('keydown', '#txtWebFormShippingCost', function(e){
            AccountingNumberOnly($(this).val());
        })
        .on('keydown', '#txtWebFormLandedCost', function(e){
            AccountingNumberOnly($(this).val());
        })
        .on('keydown', '#txtWebFormSRP', function(e){
            AccountingNumberOnly($(this).val());
        })
        .on('keydown', '#txtWebFormRetailPrice', function(e){
            AccountingNumberOnly($(this).val());
        })
        .on('keydown', '#txtWebFormPackingSizeQty', function(e){
            AccountingNumberOnly($(this).val());
        })
        .on('keydown', '#txtWebFormContentQty', function(e){
            AccountingNumberOnly($(this).val());
        })
        .on('click', '#btnWebFormSave', function () {
            if($.trim($('#txtWebFormProductNo').val()) == ""){
                NewMessageBox('Product No. is required', 'PIM Web Form');
                return;
            }

            if($.trim($('#txtWebFormShortName').val()) == ""){
                NewMessageBox('Short Name is required', 'PIM Web Form');
                return;
            }

            if($.trim($('#txtWebFormCategory').val()) == ""){
                NewMessageBox('Category is required', 'PIM Web Form');
                return;
            }

            if($.trim($('#txtWebFormCostFromVendor').val()) == ""){
                NewMessageBox('Cost From Vendor is required', 'PIM Web Form');
                return;
            }

            //if($.trim($('#txtWebFormLandedCost').val()) == ""){
            //    NewMessageBox('Landed Cost is required', 'PIM Web Form');
            //    return;
            //}

            if($.trim($('#txtWebFormSRP').val()) == ""){
                NewMessageBox('SRP is required', 'PIM Web Form');
                return;
            }

            if($.trim($('#txtWebFormRetailPrice').val()) == ""){
                NewMessageBox('Retail Price is required', 'PIM Web Form');
                return;
            }

            if($.trim($('#txtWebFormPackingSizeQty').val()) == ""){
                NewMessageBox('Packing Size Qty is required', 'PIM Web Form');
                return;
            } else {
                if (CDec2($('#txtWebFormPackingSizeQty').val()) == 0) {
                    NewMessageBox('Packing Size Qty must be equal 1', 'PIM Web Form');
                    return;
                }
            }

            if($.trim($('#selWebFormPackingSizeUnit').val()) == ""){
                NewMessageBox('Packing Size Unit is required', 'PIM Web Form');
                return;
            }

            var webFormData = {
                ProductNo: $.trim($('#txtWebFormProductNo').val()),
                ShortName: $.trim($('#txtWebFormShortName').val()),
                Brand: $.trim($('#txtWebFormBrandName').val()),
                Category: $.trim($('#txtWebFormCategory').val()),
                UPCBarcode: $.trim($('#txtWebFormUPCBarcode').val()),
                CostFromVendor: $.trim($('#txtWebFormCostFromVendor').val()),
                ShippingCost: $.trim($('#txtWebFormShippingCost').val()),
                LandedCost: $.trim($('#txtWebFormLandedCost').val()),
                SRP: $.trim($('#txtWebFormSRP').val()),
                RetailPrice: $.trim($('#txtWebFormRetailPrice').val()),
                PackingSizeQty: $.trim($('#txtWebFormPackingSizeQty').val()),
                PackingSizeUnit: $.trim($('#selWebFormPackingSizeUnit').val()),
                ContentQty: $.trim($('#txtWebFormContentQty').val()),
                ContentUnit: $.trim($('#selWebFormContentUnit').val()),
                DimensionLength: $.trim($('#txtWebFormProductDimensionLength').val()),
                DimensionLengthUnit: $.trim($('#selWebFormProductDimensionLengthUnit').val()),
                DimensionWidth: $.trim($('#txtWebFormProductDimensionWidth').val()),
                DimensionWidthUnit: $.trim($('#selWebFormProductDimensionWidthUnit').val()),
                DimensionHeight: $.trim($('#txtWebFormProductDimensionHeight').val()),
                DimensionHeightUnit: $.trim($('#selWebFormProductDimensionHeightUnit').val()),
                ProductWeight: $.trim($('#txtWebFormProductWeight').val()),
                ProductWeightUnit: $.trim($('#selWebFormProductWeightUnit').val())
            }
            if (CDec2(webFormData.LandedCost) == 0)
                webFormData.LandedCost = CDec2(webFormData.CostFromVendor) + CDec2(webFormData.ShippingCost);
            ValidatePIMWebForm(webFormData)
        })
        .on('click', '#btnWebFormSearchBrand', function () {
            BrowsePopup('LibraryPimBrands', 'Search Product Brands', [
                {
                    DisplayText: [{ Column: 'Brand' }],
                    AutoSearch: true,
                    Click: function (e) {
                        $('#txtWebFormBrandName').val(e.Brand);
                    }
                }
            ])
        })
        .on('click', '#btnWebFormSearchCategory', function () {
            BrowsePopup('LibraryPimCategories', 'Search Product Categories', [
                {
                    DisplayText: [{ Column: 'Category' }],
                    AutoSearch: true,
                    Click: function (e) {
                        $('#txtWebFormCategory').val(e.Category);
                    }
                }
            ])
        })

        function AccountingNumberOnly(s) {
            var rgx = /^[0-9]*\.?[0-9]*$/;
            return s.match(rgx);
        }

        function ValidatePIMWebForm(webFormData) {
            var pDataHeader = ["Product No *", "Short Name *", "Brand", "Category *", "UPC (Barcode)", "Cost From Vendor *", "Shipping Cost", "Landed Cost *", "SRP *", "Retail Price *", "Packing Size Qty *", "Packing Size Unit *", "Content Qty", "Content Unit", "Product Dimension Length", "Product Dimension Length Unit", "Product Dimension Width", "Product Dimension Width Unit", "Product Dimension Height", "Product Dimension Height Unit", "Product Weight", "Product Weight Unit"];

            Bs.Loading('show', 'Validating your data ...');

            AjaxSend({
                url: 'PIMUpload/CheckIfHeadersExist',
                data: { s: pDataHeader, OrgID: sessionStorage.OrgID },
                success: function (vHeaderExist) {
                    var data = vHeaderExist;
                    var pDataIsExcelUploaded = {}
                    pDataIsExcelUploaded = {
                        OrgID: $.trim(sessionStorage.OrgID),
                        IsSameProduct: location.hash == '#SameProduct' ? 1 : 0
                    };

                    AjaxSend({
                        url: 'PIMUpload/IsPIMExcelUploaded',
                        data: pDataIsExcelUploaded,
                        success: function (vPIMExcelUploaded) {
                            IsTempateExist = false;

                            Bs.Loading('hide');

                            if (data.Result == 'NewTemplate'){
                                sessionStorage["PIMExcelUpload-0"] = JSON.stringify(["Product No *", "f9cb775d-5e60-4d92-98a3-2a8444d5f32a", "Product No", "Textbox"])
                                sessionStorage["PIMExcelUpload-1"] = JSON.stringify(["Short Name *", "dafbb390-ea84-4ec8-948b-74b028cbe996", "Short Product Name", "Textbox"])
                                sessionStorage["PIMExcelUpload-2"] = JSON.stringify(["Brand", "7b3524d2-effc-4921-b752-3cf7d84434d6", "Brand", "Multiple Option"])
                                sessionStorage["PIMExcelUpload-3"] = JSON.stringify(["Category *", "325a48b7-58f8-4678-b608-58cf43c59bfd", "Category", "Multiple Option"])
                                sessionStorage["PIMExcelUpload-4"] = JSON.stringify(["UPC (Barcode)", "a4b843a6-cbb9-4429-8bc7-2d571e7ab48c", "UPC", "Textbox"])
                                sessionStorage["PIMExcelUpload-5"] = JSON.stringify(["Cost From Vendor *", "e531191e-203e-41cc-ba9b-c46a27d10cd8", "Cost From Vendor", "Textbox (Decimal)"])
                                sessionStorage["PIMExcelUpload-6"] = JSON.stringify(["Shipping Cost", "2254751e-e1fc-4704-b494-a39fdba2a245", "Shipping Cost", "Textbox (Decimal)"])
                                sessionStorage["PIMExcelUpload-7"] = JSON.stringify(["Landed Cost *", "1eb487fc-8b91-4d0c-901f-16bf4b80a9a3", "Landed Cost", "Textbox (Decimal)"])
                                sessionStorage["PIMExcelUpload-8"] = JSON.stringify(["SRP *", "c5bd7b11-369d-404a-ad18-c53566744c99", "Price - Suggested Retail", "Textbox (Decimal)"])
                                sessionStorage["PIMExcelUpload-9"] = JSON.stringify(["Retail Price *", "c5bd7b11-369d-404a-ad18-c53566744c90", "Retail Price", "Textbox (Decimal)"])
                                sessionStorage["PIMExcelUpload-10"] = JSON.stringify(["Packing Size Qty *", "e1059bd9-b365-478f-a443-f465b604d251", "Packing Size Qty", "Multiple Option"])
                                sessionStorage["PIMExcelUpload-11"] = JSON.stringify(["Packing Size Unit *", "c91a5759-bb33-4f08-aac1-fc7b1f94e7b9", "Packing Size Unit", "Multiple Option"])
                                sessionStorage["PIMExcelUpload-12"] = JSON.stringify(["Content Qty", "443c66e2-f7a3-4c8b-a287-3d5e2d1945f4", "Content Qty", "Textbox"])
                                sessionStorage["PIMExcelUpload-13"] = JSON.stringify(["Content Unit", "dd4563d3-9fc5-41e4-88eb-12b45279820e", "Content Unit", "Textbox"])
                                sessionStorage["PIMExcelUpload-14"] = JSON.stringify(["Product Dimension Length", "682e19bd-c47a-4760-b120-3dd931d5bb49", "Product Dimension - Length", "Textbox (Decimal)"])
                                sessionStorage["PIMExcelUpload-15"] = JSON.stringify(["Product Dimension Length Unit", "9b32e0bc-e77e-4724-9bbc-c0c0989ef95b", "Product Dimension - Length Unit", "Textbox"])
                                sessionStorage["PIMExcelUpload-16"] = JSON.stringify(["Product Dimension Width", "e35110cc-a508-463d-b440-daa3b4cef792", "Product Dimension - Width", "Textbox (Decimal)"])
                                sessionStorage["PIMExcelUpload-17"] = JSON.stringify(["Product Dimension Width Unit", "97842368-d382-4d66-a00c-6c6f5250fe0f", "Product Dimension - Width Unit", "Textbox"])
                                sessionStorage["PIMExcelUpload-18"] = JSON.stringify(["Product Dimension Height", "aafae8e0-4aba-448a-a89e-cc5f2fcf0942", "Product Dimension - Height", "Textbox (Decimal)"])
                                sessionStorage["PIMExcelUpload-19"] = JSON.stringify(["Product Dimension Height Unit", "530efd37-3303-4903-b939-9b3ff19564cc", "Product Dimension - Height Unit", "Textbox"])
                                sessionStorage["PIMExcelUpload-20"] = JSON.stringify(["Product Weight", "e1de6123-65ed-422f-b275-c8c2a0fbca44", "Product Weight", "Textbox (Decimal)"])
                                sessionStorage["PIMExcelUpload-21"] = JSON.stringify(["Product Weight Unit", "770319ea-1948-4fe7-b4f1-cc669e947d53", "Product Weight Unit", "Textbox"])

                                UploadPIMTemplate();
                            }
                            else if(data.Result == 'TemplateExist'){
                                if (vPIMExcelUploaded.PIMExcelUpload.Rows.length > 0) {
                                    SavePIMWebForm(webFormData, pDataHeader)
                                }
                            }
                            else if(data.Result == 'Success'){

                            }
                        }
                    })
                }
            })
        }

        function SavePIMWebForm(webFormData, pDataHeader){
            
            Bs.Loading('show', 'Saving your data ...');

            AjaxSend({
                url: 'PIMUpload/SavePIMWebForm',
                data: { IsAsset: $('#chbAsset').is(':checked'), IsInventoryCountItem: $('#chbIsInventoryCountItem').is(':checked'), WebFormData: webFormData, OrgID: sessionStorage.OrgID, pDataHeader: pDataHeader },
                success: function (vData) {
                    Bs.Loading('hide');

                    if(CStr2(vData.PIMExcelUploadError) != ""){
                        NewMessageBox(vData.PIMExcelUploadError, 'PIM Web Form');
                        return;
                    }

                    if (CStr2(vData.PIMExcelUpload) == "Success") {
                        WebToast('Successfully Added', 'top');
                        if (CBool(sessionStorage.IsGlobalPIMWebForm)) { // for adding SKU globally, NewMessageBox.js
                            $('#globalpopupPIMWebForm').hide();
                            $('#divGlobalPIMWebForm').empty();
                        } else {                            
                            $('#tblBody').empty();
                            window.location.hash = 'LibraryPage';
                            LibraryTable.PIMPage();
                        }                        
                    }
                    else if(CStr2(vData.PIMExcelUpload) == "Success"){
                        NewMessageBox("Encountered error while saving your data.", "PIM Web Form");
                    }
                }
            })
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
                        blnIsAllColMatched = true;
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
                PIMTemplateColumnAKAList: pDataAKA
            };

            AjaxSend({
                url: "PIMUpload/UploadPIMExcelTemplate",
                data: pData,
                success: function (vSaveData) {

                    if (vSaveData.PIMExcelUpload == "Success") {
                        $('#btnWebFormSave').click();
                    } else {
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
        }

        function GetPackingSizeUnit() {
            var selWebFormPackingSizeUnit = $('#selWebFormPackingSizeUnit');
            selWebFormPackingSizeUnit.empty();//.selectmenu('refresh');

            AjaxSend({
                url: 'PIMUpload/GetPackingSizeUnit',
                success: function (vData) {
                    var vRows = vData.Results.Rows;
                    selWebFormPackingSizeUnit.append(new Option('Please Select', ''))
                    for (var i = 0; i < vRows.length; i++) {
                        selWebFormPackingSizeUnit.append(new Option(vRows[i].Measurement, vRows[i].Measurement))
                    }
                    selWebFormPackingSizeUnit.val('');//.selectmenu('refresh');

                    GetContentUnit();
                }
            })
        }

        function GetContentUnit() {
            var selWebFormContentUnit = $('#selWebFormContentUnit');
            selWebFormContentUnit.empty();//.selectmenu('refresh');

            AjaxSend({
                url: 'PIMUpload/GetContentUnit',
                success: function (vData) {
                    var vRows = vData.Results.Rows;
                    selWebFormContentUnit.append(new Option('Please Select', ''))
                    for (var i = 0; i < vRows.length; i++) {
                        selWebFormContentUnit.append(new Option(vRows[i].Measurement, vRows[i].Measurement))
                    }
                    selWebFormContentUnit.val('');//.selectmenu('refresh');

                    GetDimensionUnit();
                }
            })
        }

        function GetDimensionUnit() {
            var selWebFormProductDimensionLengthUnit = $('#selWebFormProductDimensionLengthUnit');
            var selWebFormProductDimensionWidthUnit = $('#selWebFormProductDimensionWidthUnit');
            var selWebFormProductDimensionHeightUnit = $('#selWebFormProductDimensionHeightUnit');
            selWebFormProductDimensionLengthUnit.empty();//.selectmenu('refresh');
            selWebFormProductDimensionWidthUnit.empty();//.selectmenu('refresh');
            selWebFormProductDimensionHeightUnit.empty();//.selectmenu('refresh');
            AjaxSend({
                url: 'PIMUpload/GetDimensionUnit',
                success: function (vData) {
                    var vRows = vData.Results.Rows;
                    selWebFormProductDimensionLengthUnit.append(new Option('Please Select', ''))
                    selWebFormProductDimensionWidthUnit.append(new Option('Please Select', ''))
                    selWebFormProductDimensionHeightUnit.append(new Option('Please Select', ''))
                    for (var i = 0; i < vRows.length; i++) {
                        selWebFormProductDimensionLengthUnit.append(new Option(vRows[i].Measurement, vRows[i].Measurement))
                        selWebFormProductDimensionWidthUnit.append(new Option(vRows[i].Measurement, vRows[i].Measurement))
                        selWebFormProductDimensionHeightUnit.append(new Option(vRows[i].Measurement, vRows[i].Measurement))
                    }
                    selWebFormProductDimensionLengthUnit.val('');//.selectmenu('refresh');
                    selWebFormProductDimensionWidthUnit.val('');//.selectmenu('refresh');
                    selWebFormProductDimensionHeightUnit.val('');//.selectmenu('refresh');

                    GetweightUnit();
                }
            })
        }

        function GetweightUnit() {
            var selWebFormProductWeightUnit = $('#selWebFormProductWeightUnit');
            selWebFormProductWeightUnit.empty();//.selectmenu('refresh');

            AjaxSend({
                url: 'PIMUpload/GetweightUnit',
                success: function (vData) {
                    var vRows = vData.Results.Rows;
                    selWebFormProductWeightUnit.append(new Option('Please Select', ''))
                    for (var i = 0; i < vRows.length; i++) {
                        selWebFormProductWeightUnit.append(new Option(vRows[i].Measurement, vRows[i].Measurement))
                    }
                    selWebFormProductWeightUnit.val('');//.selectmenu('refresh');
                }
            })
        }

        function GetPIMMeasure() {
            var selWebFormPackingSizeUnit = $('#selWebFormPackingSizeUnit');
            selWebFormPackingSizeUnit.empty();//.selectmenu('refresh');

            AjaxSend({
                url: 'PIMUpload/GetPIMMeasure',
                success: function (vData) {
                    var vRows = vData.Results.Rows;
                    selWebFormPackingSizeUnit.append(new Option('Please Select', ''))
                    for (var i = 0; i < vRows.length; i++) {
                        selWebFormPackingSizeUnit.append(new Option(vRows[i].Measurement, vRows[i].Measurement))
                    }
                    selWebFormPackingSizeUnit.val('');//.selectmenu('refresh');
                }
            })
        }

        function ClearFields() {
            $('#txtWebFormProductNo').val('');
            $('#txtWebFormShortName').val('');
            $('#txtWebFormBrandName').val('');
            $('#txtWebFormCategory').val('');
            $('#txtWebFormUPCBarcode').val('');
            $('#txtWebFormCostFromVendor').val('');
            $('#txtWebFormShippingCost').val('');
            $('#txtWebFormLandedCost').val('');
            $('#txtWebFormSRP').val('');
            $('#txtWebFormRetailPrice').val('');
            $('#txtWebFormPackingSizeQty').val('');
            $('#selWebFormPackingSizeUnit').val('');//.selectmenu('refresh');
            $('#txtWebFormContentQty').val('');
            $('#selWebFormContentUnit').val('');//.selectmenu('refresh');
            $('#txtWebFormProductDimensionLength').val('');
            $('#selWebFormProductDimensionLengthUnit').val('');//.selectmenu('refresh');
            $('#txtWebFormProductDimensionWidth').val('');
            $('#selWebFormProductDimensionWidthUnit').val('');//.selectmenu('refresh');
            $('#txtWebFormProductDimensionHeight').val('');
            $('#selWebFormProductDimensionHeightUnit').val('');//.selectmenu('refresh');
            $('#txtWebFormProductWeight').val('');
            $('#selWebFormProductWeightUnit').val('');//.selectmenu('refresh');
            $('#chbAsset').removeAttr('checked');//.checkboxradio('refresh');
            $('#chbIsInventoryCountItem').prop('checked', 1);
        }
    })
})