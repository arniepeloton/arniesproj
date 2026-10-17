require([
        'common'
], function () {
    require([
            (OfflineMode ? '/Scripts/sources/BOMProjectExcel/MaterialsRequisition_offline.js?crc=' : ''),
            (OfflineMode ? '/Scripts/sources/BOMProjectExcel/BOMMain_offline.js?crc=' : ''),
            (OfflineMode ? '/Scripts3/Offline/Browser/LibraryProject.js?crc=' : '')
    ], function () {

        var arrProjectBudgetByProject = [];
        var MaterialsRequisition = {};
        var BOMMain = {};
        var arrAccessRights = [];
        var arrVendorID = [];
        var isCreatePOFromApproved = false;

        $(document).ready(function () {
            var pPage = $.mobile.activePage;
            var pHeader = pPage.find("[data-role=header]");
            var pHeight = pHeader.height() + 10;

            //pPage.css('padding-top', pHeight + 'px');

            //            if ($.isFunction(pHeader.fixedtoolbar)) // jqm 1.4.4 doesn't have fixedtoolbar function
            //                pHeader.fixedtoolbar({ tapToggle: false,
            //                    updatePagePadding: false
            //                });
            //            App.Deferred.fulfill('R87MBPXGSQ');

            if (OfflineMode) {
                MaterialsRequisition = new App.model.MaterialsRequisition_Offline();
                BOMMain = new App.model.BOMMain_Offline();
            }

            window.MatReq = {
                GetProjectNameList: function () {
                    GetProjectNameList();
                }
            }

            GetProjectNameList();

            $('body')
            .on('click', '#btnHome', function () {
                location = MergeUrlParameters({
                    OrgID: sessionStorage.OrgID,
                    mp: 'mc',
                    IsMobile: CBool(IsMobile) ? 1 : 0
                }, '/SetupChecklist/BOMProjectExcel');
            })
            .on('click', '#btnProjectName', function () {
                BrowsePopup('LibraryProject', 'Project Name', [
                    {
                        AutoSearch: true,
                        DisplayText: [
                            { Column: 'FowstName' },
                            { Column: 'Address' }
                        ],
                        Click: function (vData) {
                            var pData = {
                                ProjectID: vData.ID,
                                OrgID: sessionStorage.OrgID
                            }

                            sessionStorage.ProjectID = vData.ID;
                            $('#txtProjecName').text(vData.FowstName);
                            $('#divMainHotButtons').show().empty();
                            $('#divSubHotButtons, #divItemCount').hide();
                            $('#divSubHotButtons').empty();
                            $('#divParentHotButtons button:not(":first")').remove();
                            GetProjectBudgetByProject(pData);
                            GetAccessRights();
                        }
                    }
                ])
            })
            .on('click', '#btnMainHotButton', function () {
                $('#divMainHotButtons').show();
                $('#divSubHotButtons, #divItemCount').hide();
                $('#divParentHotButtons button:not(":first")').remove();
                $(this).removeClass('custom-btn-theme-c').addClass('custom-btn-theme-b');
            })
            .on('click', '#btnSaveMaterialsRequisition', function () {
                if ($('#divItemCount').css('display') == 'block') {
                    var isNull = false;

                    //NewMessageBox('Please wait while saving ...');

                    $('#tBodyMaterialsCount').find('tr').each(function () {
                        if ($.trim($(this).find('td:eq(5)').find('input').val()) == "") {
                            isNull = true;
                        }
                        else {
                            isNull = false;
                            return false;
                        }
                    })

                    if (isNull) {
                        NewMessageBox('No data to save. Please check the text field.');
                    }
                    else {
                        SaveMaterialsRequisition();
                    }
                }
            })
            .on('change', '#selProjectNameMatReq', function () {
                var id = $('#selProjectNameMatReq option:selected').val();

                if(id == ''){
                    $('#divMainParentHotButton, #divItemCount').hide();
                    $('#divMainHotButtons, #divSubHotButtons').hide().empty();
                    return;
                }

                AjaxSend({
                    url: 'SetupChecklist/GetAccessRights',
                    data: { id: id },
                    success: function (vData) {
                        var isAllowed = false;

                        isAllowed = $.map(vData.Results.Rows, function (obj, index) {
                            if (obj.Description == "Make Materials Requisition") {
                                return true;
                            }
                        })

                        if (CBool(isAllowed) == false) {
                            $('#divMainParentHotButton, #divItemCount').hide();
                            $('#divMainHotButtons, #divSubHotButtons').hide().empty();
                            NewMessageBox('You don\'t have permission to make materials requisition for Project <b>' + $('#selProjectNameMatReq option:selected').text() + '</b>');
                            return;
                        }

                        if (id != "") {
                            var pData = {
                                ProjectID: id,
                                OrgID: sessionStorage.OrgID
                            }

                            sessionStorage.ProjectID = id;
                            $('#divMainParentHotButton').show();
                            $('#divMainHotButtons').show().empty();
                            $('#divSubHotButtons, #divItemCount').hide();
                            $('#divSubHotButtons').empty();
                            $('#divParentHotButtons button:not(":first")').remove();
                            GetProjectBudgetByProject(pData);
                            GetAccessRights();
                        }
                        else {
                            $('#divMainParentHotButton').hide();
                            $('#divMainHotButtons').show().empty().text('No Data');
                        }
                    }
                })
            })
            .on('change', '#selProjectNameDashboard', function () {
                GetRequestMaterialsHeader($(this).val(), sessionStorage.OrgID, false);
            })
            .on('click', '#btnDashboard', function () {
                $('#pageDashboardMaterialsRequisiton').pageshow();
            })
            .on('click', '#btnMaterialsRequisition', function () {
                //BreadCrumbs('.ItemsBreadCrumbs',
                //        [{
                //            Text: 'Material Requisition',
                //            Click: function () {
                //                $('#pageDashboardMaterialsRequisiton').pageshow();
                //            }
                //        }, { Text: 'New Request' }]
                //    )
                $('#pageNewMaterialsRequisiton').show();
            })
            .on('click', '#btnApprove', function () {
                AjaxSend({
                    url: 'SetupChecklist/GetAccessRights',
                    data: { id: $('.txtProjectID').val() },
                    success: function (vData) {
                        var isAllowed = false;

                        isAllowed = $.map(vData.Results.Rows, function (obj, index) {
                            if (obj.Description == "Approve Materials Requisition") {
                                return true;
                            }
                        })

                        if (CBool(isAllowed) == false) {
                            NewMessageBox('You don\'t have permission to approve/disapprove materials requisition');
                            return;
                        }

                        var arrApprovedMatReq = [];
                        arrVendorID = [];
                        var reqid = $('.txtRequisitionID').val();
                        var isVendorExist = false;
                        var ctr = 0;

                        $(this).attr('disabled', true);

                        $('#tbodyRequestedMaterials tr').each(function () {
                            var vid = $(this).find('td:eq(10)').attr('vid'); // column vendor

                            arrApprovedMatReq.push({
                                MatReqID: $(this).attr('matreqid'),
                                ApprovedRequestedQty: (isCreatePOFromApproved == true ? $('#tbodyAppRequestedMaterials tr').eq(ctr).find('td:eq(4)').find('b').remove('b').end().text() : $(this).find('td:eq(9)').find('input').val())
                            })

                            if (arrVendorID.length > 0) {
                                for (var i = 0; i < arrVendorID.length; i++) {
                                    if (vid == arrVendorID[i].VendorID) {
                                        isVendorExist = true;
                                    }
                                }

                                if (isVendorExist == false) {
                                    arrVendorID.push({
                                        VendorID: vid,
                                        ProjectID: $('.txtProjectID').val(),
                                        PONum: Math.floor(Math.random() * 1000),
                                        POID: NewID()
                                    })
                                }
                            }
                            else {
                                arrVendorID.push({
                                    VendorID: vid,
                                    ProjectID: $('.txtProjectID').val(),
                                    PONum: Math.floor(Math.random() * 1000),
                                    POID: NewID()
                                })
                            }

                            ctr = ctr + 1;
                        })

                        isCreatePOFromApproved = false;
                        SaveApprovedMaterialsRequisition(arrApprovedMatReq, reqid, "Approved");
                        $(this).attr('disabled', false);
                    }
                })
            })
            .on('click', '#btnDisapprove', function () {
                var arrApprovedMatReq = [];
                var reqid = $('.txtRequisitionID').val();
                NewMessageBox('You are about to disapprove the Material Requisition No. <b>' + $('#txtRequisitionNumber').text() + '</b>. Are you sure you want to continue?', 'Materials Requisition',
                [
                    {
                        Text: 'Yes',
                        Click: function () {
                            $(this).attr('disabled', true);
                            SaveApprovedMaterialsRequisition(arrApprovedMatReq, reqid, "Disapproved");
                            $(this).attr('disabled', false);
                        }
                    },
                    { Text: 'No' }
                ])
            })
            .on('click', '#btnCancelMatReq', function () {
                $('#popMaterialsRequisition').hide();
            })
            .on('click', '#btnPDF', function () {
                window.location = '/MaterialsRequisition/GeneratePDF';
                return;
                PrintNowCanvassForm();
            })
            .on('click', '#btnDeliveryDate', function () {
                SingleCalendar({
                    SetText: '#btnDeliveryDate',
                    Click: function (e) {
                        $('#popMakePO').popup('open');
                        $('#spanValidationDelDate').hide();
                    }
                })
            })
            .on('click', '#btnCancelMakePO', function () {
                $('#popMakePO').popup('close');
            })
            .on('click', '#btnOkMakePO', function () {
                if ($.trim($('#btnDeliveryDate').text()) == 'Set Date') {
                    $('#spanValidationDelDate').show();
                    return;
                }
                else {
                    $('#spanValidationDelDate').hide();
                }

                SavePORequiredDetails();
            })
            .on('click', '#btnCreatePO', function () {
                isCreatePOFromApproved = true;
                $('#btnApprove').click();
            })
            .on('click', '#btnCloseReq', function () {
                $('#popMaterialsRequisitionApproved').hide();
            })
            .on('click', '#btnDownloadReq', function () {
                $(this).addClass('btn-disabled').text('Downloading ...');
                MakePDF($('.txtRequisitionID').val(), false);
            })
            .on('keypress', '#srchReqMat', function (e) {
                if (e.keyCode == 13) {
                    var val = $.trim($(this).val());
                    GetRequestMaterialsHeader($('#selProjectNameDashboard option:selected').val(), sessionStorage.OrgID, true);
                }
            })
            .on('click', '#btnRefreshMaterialReq', function () {
                GetRequestMaterialsHeader('', sessionStorage.OrgID, false);
            })
        })

        function SavePORequiredDetails() {
            var pData = {};

            pData.Vendor = arrVendorID;
            pData.ReqID = $('#txtMakePOReqID').val();
            pData.CreatedBy = sessionStorage.PeopleID;
            pData.InvoiceType = $('#selInvoiceType option:selected').val();
            pData.DeliveryDate = $('#btnDeliveryDate').text();
            pData.BillingAddress = $('#selBillingAddress option:selected').text();
            pData.BillingAddressID = $('#selBillingAddress option:selected').val();
            pData.BillingContactNo = $('#selBillingAddress option:selected').attr('contactno');
            pData.DeliveryAddress = $('#selDeliveryAddress option:selected').text();
            pData.DeliveryAddressID = $('#selDeliveryAddress option:selected').val();
            pData.DeliveryContactNo = $('#selDeliveryAddress option:selected').attr('contactno');
            pData.Remarks = $('#txtRemarks').val();

            AjaxSend({
                url: 'MaterialsRequisition/SavePORequiredDetails',
                data: pData,
                success: function (vData) {
                    if (vData.Results == 'saved') {
                        $('#popMakePO').popup('close');
                        PrintPO(pData);
                    }
                    else {
                        NewMessageBox('ERROR: ' + vData.Results);
                    }
                }
            })
        }

        function SaveApprovedMaterialsRequisition(arr, reqid, isApp) {
            var data = {
                ReqID: reqid,
                IsApprove: isApp,
                ApprovedMatReq: arr,
                ApprovedBy: sessionStorage.PeopleID
            }

            AjaxSend({
                url: 'MaterialsRequisition/SaveApprovedMaterialsRequisition',
                data: data,
                success: function (vData) {

                    if (isApp == "Disapproved") {
                        $('#popMaterialsRequisition').hide();
                        GetRequestMaterialsHeader($('#selProjectNameDashboard option:selected').val(), sessionStorage.OrgID, false);
                    }
                    else {
                        if (vData.Results == "saved") {
                            $('#popMaterialsRequisition').hide();
                            $('#popMaterialsRequisitionApproved').hide();

                            NewMessageBox("Material Requisition Approval No. <b>" + $('#txtRequisitionNumber').text() + "</b> has been successfully saved. " +
                                "<br><br><span style='color:#F00' id='txtValidationError'></span>" +
                                "<label>" +
                                    "<input type='checkbox' value='po' name='chbApprovedMatReqOptions'/>" +
                                    "Purchase Order <br>" +
                                    "<span style='font-weight:normal'>Mobiz will make 1 PO for each unique vendor in the approved materials list.</span>" +
                                "</label>" +
                                "<label>" +
                                    "<input type='checkbox' value='list' name='chbApprovedMatReqOptions'/>" +
                                    "Approved Materials Requisition List <br>" +
                                    "<span style='font-weight:normal'>Mobiz will make 1 PDF document that contains all details in the approved materials requisition.</span>" +
                                "</label>"
                            , "Materials Requisition",
                            [
                                {
                                    Text: 'Ok',
                                    Validation: function () {
                                        if ($('input[name="chbApprovedMatReqOptions"]:checked').length == 0) {
                                            $('#txtValidationError').text('Please select at least one option.');
                                            return false;
                                        }

                                        return true;
                                    },
                                    Click: function () {
                                        $('#popMaterialsRequisition').hide();
                                        //GetRequestMaterialsHeader(($('#selProjectNameDashboard').val() == "" ? "" : $('#selProjectNameDashboard').val()), sessionStorage.OrgID)

                                        if ($('input[name="chbApprovedMatReqOptions"]:checked').length == 1) {
                                            if ($('input[name="chbApprovedMatReqOptions"]:checked').val() == 'po') {
                                                //PrintPO(reqid);
                                                AjaxSend({
                                                    url: 'SetupChecklist/GetAccessRights',
                                                    data: { id: $('.txtProjectID').val() },
                                                    success: function (vData) {
                                                        var isAllowed = false;

                                                        isAllowed = $.map(vData.Results.Rows, function (obj, index) {
                                                            if (obj.Description == "Make Purchase Orders") {
                                                                return true;
                                                            }
                                                        })

                                                        if (CBool(isAllowed) == false) {
                                                            NewMessageBox('You don\'t have permission to make purchase order');
                                                            return;
                                                        }

                                                        $('#popMakePO').popup('open');
                                                        GetAddresses(reqid);
                                                        $('#txtMakePOReqID').val(reqid);
                                                    }
                                                });
                                            }
                                            else {
                                                MakePDF(reqid, false);
                                            }
                                        }
                                        else if ($('input[name="chbApprovedMatReqOptions"]:checked').length == 2) {
                                            MakePDF(reqid, true);
                                        }
                                    }
                                },
                                {
                                    Text: 'Cancel',
                                    Click: function () {
                                        $('#popMaterialsRequisition').hide();
                                        GetRequestMaterialsHeader(($('#selProjectNameDashboard').val() == "" ? "" : $('#selProjectNameDashboard').val()), sessionStorage.OrgID, false)
                                    }
                                }
                            ], {
                                LiveValidation: function () {
                                    $('body')
                                    .on('change', 'input[name="chbApprovedMatReqOptions"]', function () {
                                        PopupReposition(); // NewMessageBox.js
                                    });
                                }
                            });
                        }
                        else {
                            NewMessageBox("Error: " + vData.Results, "Materials Requisition");
                        }
                    }
                }
            })
        }

        function GetAddresses(reqID) {
            AjaxSend({
                url: 'MaterialsRequisition/GetOrgDetails',
                data: { OrganizationID: sessionStorage.OrgID },
                success: function (vDataOrg) {
                    var vRowsOrgDetails = vDataOrg.OrgDetails.Rows;
                    var vRowsOrgTelDetails = vDataOrg.OrgTelDetails.Rows;
                    var orgAddress = (vRowsOrgDetails[0].BusinessAddressBuildingNo == '' ? '' : vRowsOrgDetails[0].BusinessAddressBuildingNo + ', ') +
                                    (vRowsOrgDetails[0].BusinessAddressStreet == '' ? '' : vRowsOrgDetails[0].BusinessAddressStreet + ', ') +
                                    (vRowsOrgDetails[0].BusinessAddressCity == '' ? '' : vRowsOrgDetails[0].BusinessAddressCity + ', ') +
                                    (vRowsOrgDetails[0].BusinessAddressCountry == '' ? '' : vRowsOrgDetails[0].BusinessAddressCountry)

                    $('#selBillingAddress').empty()
                        .append($('<option>')
                            .attr({
                                value: vRowsOrgDetails[0].ID,
                                contactno: (vRowsOrgTelDetails.length > 0 ? ('+' + vRowsOrgTelDetails[0].CountryCode + '.' + vRowsOrgTelDetails[0].AreaCode + '.' + vRowsOrgTelDetails[0].ContactNo + (vRowsOrgTelDetails[0].ExtensionNo == "" ? "" : '.' + vRowsOrgTelDetails[0].ExtensionNo)) : "")
                            })
                            .text(orgAddress)
                        )
                        .selectmenu('refresh');

                    $('#selDeliveryAddress').empty()
                        .append($('<option>')
                            .attr({
                                value: vRowsOrgDetails[0].ID,
                                contactno: (vRowsOrgTelDetails.length > 0 ? ('+' + vRowsOrgTelDetails[0].CountryCode + '.' + vRowsOrgTelDetails[0].AreaCode + '.' + vRowsOrgTelDetails[0].ContactNo + (vRowsOrgTelDetails[0].ExtensionNo == "" ? "" : '.' + vRowsOrgTelDetails[0].ExtensionNo)) : "")
                            })
                            .text(orgAddress)
                        )
                        .selectmenu('refresh');

                    PopupReposition(); // NewMessageBox.js

                    AjaxSend({
                        url: 'MaterialsRequisition/GetProjectDetails',
                        data: { reqID: reqID, projID: $('.txtProjectID').val() },
                        success: function (vDataProject) {
                            var vRowsProject = vDataProject.Results.Rows;

                            $('#selBillingAddress')
                                .append($('<option>')
                                    .attr({
                                        value: vRowsProject[0].ID,
                                        contactno: vRowsProject[0].ContactNo
                                    })
                                    .text(vRowsProject[0].Address)
                                )
                                .selectmenu('refresh');

                            $('#selDeliveryAddress')
                                .append($('<option>')
                                    .attr({
                                        value: vRowsProject[0].ID,
                                        contactno: vRowsProject[0].ContactNo
                                    })
                                    .text(vRowsProject[0].Address)
                                )
                                .selectmenu('refresh');

                            PopupReposition(); // NewMessageBox.js
                        }
                    })
                }
            })
        }

        function MakePDF(reqID, isMakePO) {
            AjaxSend({
                url: 'MaterialsRequisition/GetApprovedMaterials',
                data: { reqID: reqID, projID: $('.txtProjectID').val() },
                success: function (vData) {
                    CreatePDFNow(vData);
                    $('#btnDownloadReq').removeClass('btn-disabled').text('Download Picking List');

                    if (isMakePO == true) {
                        AjaxSend({
                            url: 'SetupChecklist/GetAccessRights',
                            data: { id: $('.txtProjectID').val() },
                            success: function (vData) {
                                var isAllowed = false;

                                isAllowed = $.map(vData.Results.Rows, function (obj, index) {
                                    if (obj.Description == "Make Purchase Orders") {
                                        return true;
                                    }
                                })

                                if (CBool(isAllowed) == false) {
                                    NewMessageBox('You don\'t have permission to make purchase order');
                                    return;
                                }

                                $('#popMakePO').popup('open');
                                GetAddresses(reqID);
                            }
                        });
                    }
                }
            })
        }

        function PrintPO(data) {
            AjaxSend({
                url: 'MaterialsRequisition/GetOrgDetails',
                data: { OrganizationID: sessionStorage.OrgID },
                success: function (vDataOrg) {
                    var vRowsOrgDetails = vDataOrg.OrgDetails.Rows;
                    var vRowsOrgTelDetails = vDataOrg.OrgTelDetails.Rows;

                    AjaxSend({
                        url: 'MaterialsRequisition/GetProjectDetails',
                        data: { reqID: data.ReqID, projID: $('.txtProjectID').val() },
                        success: function (vDataProject) {
                            var vRowsProject = vDataProject.Results.Rows;

                            AjaxSend({
                                url: 'MaterialsRequisition/GetEachVendor',
                                data: { reqID: data.ReqID },
                                success: function (vDataVendors) {
                                    var arrHeaderDetails = [];
                                    var arrSupplier = [];

                                    for (var i = 0; i < vDataVendors.Results.Rows.length; i++) {
                                        arrSupplier.push({ ID: vDataVendors.Results.Rows[i].ID, ReqID: data.ReqID })
                                        arrHeaderDetails.push([
                                            { "caseOperation": "purchaseOrder" },
                                            { "docType": "poNum" },
                                            { "docNum": data.InvoiceType + ' ' + data.Vendor[i].PONum },
                                            { "docTypeDate": $.format.date(new Date(), 'EEEE MMM dd, yyyy') },
                                            { "dateData": $.format.date(new Date(), 'EEEE MMM dd, yyyy') },
                                            { "docTypeDelivery": "poDeliveryDate" },
                                            { "deliveryDate": $.format.date(new Date(data.DeliveryDate), 'EEEE MMM dd, yyyy') },
                                            { "docTypeTotal": "poTotal" },
                                            { "totalData": "100,488.00" },
                                            { "orgNameRaw": vRowsOrgDetails[0].CompanyName },
                                            { "orgAddressRaw": vRowsOrgDetails[0].BusinessAddressBuildingNo + ' ' + vRowsOrgDetails[0].BusinessAddressStreet + ' ' + vRowsOrgDetails[0].BusinessAddressCity + ' ' + vRowsOrgDetails[0].BusinessAddressCountry },
                                            { "custNameRaw": vDataVendors.Results.Rows[i].SupplierOrgName },
                                            { "custAddressRaw": vDataVendors.Results.Rows[i].SupplierOrgAddress },
                                            { "billingAddressDetailsRaw": data.BillingAddress },
                                            { "billingContactDetailsRaw": data.BillingContactNo },
                                            { "billingContactOptionalRaw": '' },
                                            { "deliveryAddressDetailsRaw": data.DeliveryAddress },
                                            { "deliveryContactDetailsRaw": data.DeliveryContactNo },
                                            { "deliveryContactOptionalRaw": '' },
                                            { "deliveryRemarksRaw": data.Remarks },
                                            { "qrcode": data.Vendor[i].POID },
                                            { "countDate": "FORMOBIZPROJECT" }
                                        ]
                                        )
                                    }

                                    AjaxSend({
                                        url: 'MaterialsRequisition/GetItemPerVendor',
                                        data: { arrVendor: arrSupplier },
                                        success: function (vData) {
                                            PrintPDF(
                                                {
                                                    "operation": arrHeaderDetails
                                                }, {
                                                    "data": [
                                                               { "columns": ["Item Code:", "Item Description:", "Quantity", "UoM", "Unit Price", "Sub Total"] },
                                                               {
                                                                   "rows": vData.Results
                                                               }
                                                    ]
                                                }
                                            )
                                        }
                                    })
                                }
                            })
                        }
                    })
                }
            })
        }

        function GetEachVendor() {

        }

        function CreatePDFNow(vData) {
            var pdf = new jsPDF('l', 'pt', 'a4');
            var canvas = pdf.canvas;
            var prevCtrNewLine = 0;
            var curCtrNewLine = 0;
            var maxRow = 0;
            var pageNo = 1;
            var rowPosition = 180;
            var vRowsHeader = vData.HeaderDetails.Rows[0];
            var vRowsAppMat = vData.ApprovedMaterials.Rows;

            CreatePDFHeader(pdf, vRowsHeader, pageNo);

            for (var i = 0; i < vRowsAppMat.length; i++) {
                var str;

                pdf.setFontSize(12);
                pdf.text((i + 1).toString(), 30, rowPosition);

                str = WordWrapPDF(vRowsAppMat[i].ProjectGrouping, 25); // WordWrapPDF function /Scripts/sources/BOMProjectExcel/GeneratePDF.js
                curCtrNewLine = str.split("\n").length - 1;
                pdf.setFontSize(12);
                pdf.text(str, 55, rowPosition);
                prevCtrNewLine = curCtrNewLine;

                str = WordWrapPDF(vRowsAppMat[i].ProductName, 30);
                curCtrNewLine = str.split("\n").length - 1;
                pdf.setFontSize(12);
                pdf.text(str, 210, rowPosition);

                if (curCtrNewLine > prevCtrNewLine) {
                    prevCtrNewLine = curCtrNewLine;
                }

                str = WordWrapPDF(vRowsAppMat[i].BudgetedQty, 10);
                curCtrNewLine = str.split("\n").length - 1;
                pdf.setFontSize(12);
                pdf.text(str, 380, rowPosition);

                if (curCtrNewLine > prevCtrNewLine) {
                    prevCtrNewLine = curCtrNewLine;
                }

                str = WordWrapPDF(vRowsAppMat[i].RequestedValue, 10);
                curCtrNewLine = str.split("\n").length - 1;
                pdf.setFontSize(12);
                pdf.text(str, 480, rowPosition);

                if (curCtrNewLine > prevCtrNewLine) {
                    prevCtrNewLine = curCtrNewLine;
                }

                str = WordWrapPDF(vRowsAppMat[i].ApprovedRequestedQty, 9);
                curCtrNewLine = str.split("\n").length - 1;
                pdf.setFontSize(12);
                pdf.text(str, 580, rowPosition);

                if (curCtrNewLine > prevCtrNewLine) {
                    prevCtrNewLine = curCtrNewLine;
                }

                str = WordWrapPDF(vRowsAppMat[i].UnitMeasure, 7);
                curCtrNewLine = str.split("\n").length - 1;
                pdf.setFontSize(12);
                pdf.text(str, 660, rowPosition);

                if (curCtrNewLine > prevCtrNewLine) {
                    prevCtrNewLine = curCtrNewLine;
                }

                str = WordWrapPDF((vRowsAppMat[i].VendorOrg || vRowsAppMat[i].VendorPeople), 12);
                curCtrNewLine = str.split("\n").length - 1;
                pdf.setFontSize(12);
                pdf.text(str, 720, rowPosition);

                if (curCtrNewLine > prevCtrNewLine) {
                    prevCtrNewLine = curCtrNewLine;
                }

                rowPosition = rowPosition + 15;

                if (prevCtrNewLine >= curCtrNewLine) {
                    if (prevCtrNewLine == 1) {
                        rowPosition = rowPosition;
                    }
                    else if (prevCtrNewLine == 2) {
                        rowPosition = rowPosition + 20;
                        maxRow = maxRow + 1;
                    }
                    else if (prevCtrNewLine == 3) {
                        rowPosition = rowPosition + 40;
                        maxRow = maxRow + 2;
                    }
                    else if (prevCtrNewLine == 4) {
                        rowPosition = rowPosition + 60;
                        maxRow = maxRow + 3;
                    }
                    else if (prevCtrNewLine == 5) {
                        rowPosition = rowPosition + 80;
                        maxRow = maxRow + 4;
                    }
                    else if (prevCtrNewLine == 6) {
                        rowPosition = rowPosition + 100;
                        maxRow = maxRow + 5;
                    }
                    else if (prevCtrNewLine == 7) {
                        rowPosition = rowPosition + 120;
                        maxRow = maxRow + 6;
                    }
                    else if (prevCtrNewLine == 8) {
                        rowPosition = rowPosition + 140;
                        maxRow = maxRow + 7;
                    }
                    else if (prevCtrNewLine == 9) {
                        rowPosition = rowPosition + 160;
                        maxRow = maxRow + 8;
                    }
                    else if (prevCtrNewLine == 10) {
                        rowPosition = rowPosition + 180;
                        maxRow = maxRow + 9;
                    }
                }
                else {
                    rowPosition = rowPosition;
                }

                if (maxRow > 40) {
                    pageNo = pageNo + 1;
                    pdf.addPage();
                    CreatePDFHeader(pdf, vRowsHeader, pageNo);
                    rowPosition = 180;
                    maxRow = 0;
                }

                maxRow = maxRow + 1;
            }

            pdf.save('APM_' + $.format.date(new Date(), 'MMM_dd_yyyy') + '.pdf');
        }

        function CreatePDFHeader(pdf, vRows, pageNo) {
            pdf.setFontSize(16);
            pdf.text('Requisition No.: ' + vRows.RequisitionNumber, 30, 50);

            pdf.setFontSize(12);
            pdf.text('Project Name:                       ' + vRows.ProjectName, 30, 80);

            pdf.setFontSize(12);
            pdf.text('Requestion Date & Time:      ' + $.format.date(new Date(vRows.RequestedDate), 'MMM dd, yyyy hh:mm a'), 30, 95);

            pdf.setFontSize(12);
            pdf.text('Requested By:                      ' + vRows.RequestedBy, 30, 110);

            pdf.setFontSize(12);
            pdf.text('#      Project Grouping                   SKU Name                                 Budgeted Qty       Requested Qty       Approved Qty    UoM       Vendor', 30, 155);

            pdf.setLineWidth(2);
            pdf.line(30, 165, 800, 165);

            pdf.setFontSize(12);
            pdf.text('| Page ' + pageNo, 510, 815);
        }

        var dtRequisition;
        function GetRequestMaterialsHeader(projectID, orgID, isSearch) {
            var data = {
                ProjectID: projectID,
                OrgID: orgID,
                IsSearch: isSearch,
                SearchText: $('#srchReqMat').val()
            }

            var tbodyRequisition = $('#tbodyRequisition');

            tbodyRequisition.empty().append('<tr><td colspan="9" style="text-align:center;">Please wait ...</td></tr>');

            AjaxSend({
                url: 'MaterialsRequisition/GetRequestMaterialsHeader',
                data: data,
                success: function (vData) {
                    var tblRequisition = $('#tblRequisition');

                    if ($.fn.DataTable.isDataTable(tblRequisition)) {
                        tblRequisition.DataTable().clear().destroy();
                        //return;
                    }

                    dtRequisition = tblRequisition.DataTable({
                        "data": vData.Results.Rows,
                        "pageLength": 10,
                        "columns": [
                            {
                                "data": function (data, type, full, meta) {
                                    return '<b class="ui-table-cell-label">#</b>' + data.RowNumber;
                                },
                                "width": "30px"
                            },
                            {
                                "data": function (data, type, full, meta) {
                                    return '<b class="ui-table-cell-label">Requisition Number:</b>' + data.RequisitionNumber;
                                }
                            },
                            {
                                "data": function (data, type, full, meta) {
                                    return '<b class="ui-table-cell-label">Requisition Date And Time:</b>' + $.format.date(new Date(data.RequestedDate), 'MMM dd, yyyy HH:mm');
                                }
                            },
                            {
                                "data": function (data, type, full, meta) {
                                    return '<b class="ui-table-cell-label">Project Name:</b>' + data.ProjectName
                                }
                            },
                            {
                                "data": function (data, type, full, meta) {
                                    return '<b class="ui-table-cell-label">Requested By:</b>' + data.RequestedBy
                                }
                            },
                            {
                                "data": function (data, type, full, meta) {
                                    return '<b class="ui-table-cell-label">Requisition Value:</b>' + AccountingFormat(data.RequisitionValue)
                                }
                            },
                            {
                                "data": function (data, type, full, meta) {
                                    var txtColor;

                                    if (data.Status == "New") {
                                        txtColor = '#006400';
                                    }
                                    else if (data.Status == "Approved") {
                                        txtColor = '#2A78BB';
                                    }
                                    else {
                                        txtColor = '#D82F2F';
                                    }

                                    return '<b class="ui-table-cell-label">Status:</b><b style="color: ' + txtColor + '">' + data.Status + '</b>'
                                }
                            }
                        ],
                        "aoColumnDefs": [{ "bSortable": false, "aTargets": [0] }],
                        "responsive": true,
                        "columnDefs": [{ "targets": 1, "orderable": true }],
                        "sDom": '<"top">rt<"bottom"ip><"clear">'
                        //"stateSave": true
                    })

                    $('#tblRequisition tbody').unbind('click').on('click', 'tr', function (e) {
                        if (dtRequisition.row(this).data() != undefined) {
                            var data = dtRequisition.row(this).data();
                            if (data.Status == "Approved") {
                                $('#popMaterialsRequisitionApproved').show();
                                $('#btnCreatePO').show();
                                //                                        if (CBool(data.IsPOCreated) == false) {
                                //                                            $('#btnCreatePO').show();
                                //                                        }
                                //                                        else {
                                //                                            $('#btnCreatePO').hide();
                                //                                        }
                            }
                            else {
                                $('#popMaterialsRequisition').show();
                            }

                            GetRequestDetails(data.ID, data.ProjectID, data.Status);
                        }
                    });
                }
            })
        }

        function GetRequestDetails(reqID, projectID, status) {
            AjaxSend({
                url: 'MaterialsRequisition/GetRequestDetails',
                data: { ReqID: reqID },
                success: function (vData) {

                    if (status == "Approved") {
                        $('.txtRequisitionID').val(vData.Results.Rows[0].ID);
                        $('.txtProjectID').val(vData.Results.Rows[0].ProjectID);
                        $('#txtAppRequisitionNumber').text(vData.Results.Rows[0].RequisitionNumber);
                        $('#txtAppProjectName').text(vData.Results.Rows[0].ProjectName);
                        $('#txtAppDateTime').text($.format.date(new Date(vData.Results.Rows[0].RequestedDate), 'MMM dd, yyyy hh:mm a'));
                        $('#txtAppRequestedBy').text(vData.Results.Rows[0].RequestedBy);
                        $('#txtApprovedDate').text($.format.date(new Date(vData.Results.Rows[0].ApprovedDate), 'MMM dd, yyyy hh:mm a'));
                        $('#tbodyAppRequestedMaterials').empty();
                    }
                    else {
                        $('.txtRequisitionID').val(vData.Results.Rows[0].ID);
                        $('.txtProjectID').val(vData.Results.Rows[0].ProjectID);
                        $('#txtRequisitionNumber').text(vData.Results.Rows[0].RequisitionNumber);
                        $('#txtProjectName').text(vData.Results.Rows[0].ProjectName);
                        $('#txtDateTime').text($.format.date(new Date(vData.Results.Rows[0].RequestedDate), 'MMM dd, yyyy HH:mm'));
                        $('#txtRequestedBy').text(vData.Results.Rows[0].RequestedBy);
                        $('#txtStatus').text(vData.Results.Rows[0].Status).css('color', (vData.Results.Rows[0].Status == "New" ? '#006400' : '#D82F2F'))
                        $('#tbodyRequestedMaterials').empty();
                    }

                    var data = {
                        ProjectID: projectID,
                        RequisitionID: reqID,
                        Status: vData.Results.Rows[0].Status
                    }

                    GetRequestMaterials(data);
                }
            })
        }

        function GetRequestMaterials(data) {
            var tbodyRequestedMaterials = $('#tbodyRequestedMaterials');
            var tbodyAppRequestedMaterials = $('#tbodyAppRequestedMaterials');

            tbodyRequestedMaterials.empty().append('<tr><td colspan="10" style="text-align:center;">Please wait ...</td></tr>');
            tbodyAppRequestedMaterials.empty().append('<tr><td colspan="8" style="text-align:center;">Please wait ...</td></tr>');

            AjaxSend({
                url: 'MaterialsRequisition/GetRequestMaterials',
                data: data,
                success: function (vData) {
                    tbodyRequestedMaterials.empty();
                    tbodyAppRequestedMaterials.empty();

                    if (vData.Results.Rows.length > 0) {
                        for (var i = 0; i < vData.Results.Rows.length; i++) {
                            var bgColor = 'inherit';
                            var fgColor = '#000';
                            var remainingBudgetQty = CDec2(vData.Results.Rows[i].BudgetedQty) - CDec2(vData.Results.Rows[i].Accumulated) - CDec2(vData.Results.Rows[i].RequestedValue);

                            if (CDec2(vData.Results.Rows[i].RequestedValue) > CDec2(vData.Results.Rows[i].BudgetedQty)) {
                                bgColor = '#F76767';
                                fgColor = '#FFF';
                            }

                            if (data.Status == "Approved") {
                                tbodyAppRequestedMaterials.append($('<tr>')
                                    .attr('matreqid', vData.Results.Rows[i].MaterialsRequestedID)
                                    .append($('<td>')
                                        .text(i + 1 + '.')
                                    )
                                    .append($('<td>')
                                        .css('text-transform', 'capitalize')
                                        .text(vData.Results.Rows[i].ProjectGrouping)
                                    )
                                    .append($('<td>')
                                        .text(vData.Results.Rows[i].ProductName)
                                    )
                                    .append($('<td>')
                                        .css('text-align', 'right')
                                        .text(vData.Results.Rows[i].BudgetedQty)
                                    )
                                    .append($('<td>')
                                        .css('text-align', 'right')
                                        .text(vData.Results.Rows[i].RequestedValue)
                                    )
                                    .append($('<td>')
                                        .css('text-align', 'right')
                                        .text(vData.Results.Rows[i].ApprovedQty)
                                    )
                                    .append($('<td>')
                                        .text(vData.Results.Rows[i].UnitMeasure)
                                    )
                                    .append($('<td>')
                                        .attr('vid', vData.Results.Rows[i].VendorID)
                                        .text(($.trim(vData.Results.Rows[i].VendorPeople) == "" ? vData.Results.Rows[i].VendorOrg : vData.Results.Rows[i].VendorPeople))
                                    )
                                //.css('cursor', 'pointer')
                                    .data('data', vData.Results.Rows[i])
                                )
                            }

                            tbodyRequestedMaterials.append($('<tr>')
                                    .attr('matreqid', vData.Results.Rows[i].MaterialsRequestedID)
                                    .append($('<td>')
                                        .text(i + 1 + '.')
                                    )
                                    .append($('<td>')
                                        .css('text-transform', 'capitalize')
                                        .text(vData.Results.Rows[i].ProjectGrouping)
                                    )
                                    .append($('<td>')
                                        .text(vData.Results.Rows[i].ProductName)
                                    )
                                    .append($('<td>')
                                        .text(vData.Results.Rows[i].UnitMeasure)
                                    )
                                    .append($('<td>')
                                        .css('text-align', 'right')
                                        .text(vData.Results.Rows[i].BudgetedQty)
                                    )
                                    .append($('<td>')
                                        .data(vData.Results.Rows[i])
                                        .css('text-align', 'right')
                                        .html(vData.Results.Rows[i].AccumulatedRequestedQty + '<br>')
                                        .append((CInt2(vData.Results.Rows[i].PendingRequestedQty) == 1 ? '' : '<a data-role="none" class="custom-link" style="font-size:12px;">' + vData.Results.Rows[i].PendingRequestedQty + ' Pending </a>'))
                                        .click(function (e) {
                                            e.preventDefault();

                                            if (CInt2($(this).data().PendingRequestedQty) != 1) {
                                                ShowPendingRequest($(this).data().ProductID, $(this).data().ProductName)
                                            }
                                        })
                                    )
                                    .append($('<td>')
                                        .css('text-align', 'right')
                                        .text(vData.Results.Rows[i].Accumulated)
                                    )
                                    .append($('<td>')
                                        .css('text-align', 'right')
                                        .text('0')
                                    )
                                    .append($('<td>')
                                        .attr('rbq_', vData.Results.Rows[i].MaterialsRequestedID)
                                        .css({
                                            color: fgColor,
                                            backgroundColor: bgColor,
                                            textAlign: 'right'
                                        })
                                        .text(remainingBudgetQty)
                                    )
                                    .append($('<td>')
                                        .append($('<input>')
                                            .attr('type', 'number')
                                            .data('data', vData.Results.Rows[i])
                                            .val(vData.Results.Rows[i].RequestedValue)
                                            .keyup(function () {
                                                ComputeRemainingBudgetedQty($(this));
                                            })
                                            .change(function () {
                                                ComputeRemainingBudgetedQty($(this));
                                            })
                                        )
                                    )
                                    .append($('<td>')
                                        .attr('vid', vData.Results.Rows[i].VendorID)
                                        .text(($.trim(vData.Results.Rows[i].VendorPeople) == "" ? vData.Results.Rows[i].VendorOrg : vData.Results.Rows[i].VendorPeople))
                                    )
                            //.css('cursor', 'pointer')
                                    .data('data', vData.Results.Rows[i])
                                    .click(function () {

                                    })
                                )
                        }
                    }
                    else {
                        if (data.Status == "Approved") {
                            tbodyAppRequestedMaterials.append('<tr><td colspan="7" style="text-align:center;">No Result(s) Found</td></tr>');
                        }
                        tbodyRequestedMaterials.empty().append('<tr><td colspan="9" style="text-align:center;">Please wait ...</td></tr>');
                    }

                    tbodyRequestedMaterials.parent().trigger('create').table('refresh');

                    if (data.Status == "Approved") {
                        tbodyAppRequestedMaterials.parent().trigger('create').table('refresh');
                    }
                }
            })
        }

        function ShowPendingRequest(productID, name) {
            var tbodyPendingRequest = $('#tbodyPendingRequest');

            $('#txtReqMaterialName').text(name.toUpperCase())
            tbodyPendingRequest.empty().append('<tr><td colspan="6" style="text-align:center">Please wait ...</td></tr>');

            $('#popPendingRequest').show();
            AjaxSend({
                url: 'MaterialsRequisition/GetProductRequestLogs',
                data: { ProductID: productID },
                success: function (vData) {
                    tbodyPendingRequest.empty()
                    var vRows = vData.Results.Rows;

                    for (var i = 0; i < vRows.length; i++) {
                        tbodyPendingRequest.append($('<tr>')
                            .append($('<td>')
                                .text(i + 1 + '.')
                            )
                            .append($('<td>')
                                .text(vRows[i].RequisitionNumber)
                            )
                            .append($('<td>')
                                .text(vRows[i].RequestedValue)
                            )
                            .append($('<td>')
                                .text(vRows[i].Status)
                            )
                            .append($('<td>')
                                .text(vRows[i].RequestedBy)
                            )
                            .append($('<td>')
                                .text($.format.date(new Date(vRows[i].RequestedDate), 'MMM dd, yyyy'))
                            )
                        )
                    }
                }
            })
        }

        function ComputeRemainingBudgetedQty(dis) {
            var bgColor = 'inherit';
            var fgColor = '#000';
            var remainingBudgetQty = CDec2(dis.data('data').BudgetedQty) - CDec2(dis.data('data').Accumulated) - CDec2(dis.val());

            if (CDec2(dis.val()) > CDec2(dis.data('data').BudgetedQty)) {
                bgColor = '#F76767';
                fgColor = '#FFF';
            }

            dis.parent().closest('tr').find('td:eq(8)')
                                                .css({
                                                    color: fgColor,
                                                    backgroundColor: bgColor
                                                })
                                                .text(remainingBudgetQty)
        }

        function ChangePage(pageid) {
            $('div[custom-data-role="page"]').not(pageid).hide();
            $(pageid).show();
            $(window).scrollTop(0);
            window.location.hash = '';
        }

        function GetProjectNameList() {
            $('#divMatReqLoading').pageshow();

            if (OfflineMode) {
                DisplayProjectName(OfflineBrowser.LibraryProject.GetData({ FilterFowst: 1 }).Rows);
            }
            else {
                AjaxSend({
                    url: 'Browser/LibraryProject',
                    success: function (vData) {
                        DisplayProjectName(vData.Results.Rows);
                        $('#pageDashboardMaterialsRequisiton').pageshow();
                        GetRequestMaterialsHeader('', sessionStorage.OrgID, false);
                    }
                })
            }
        }

        function DisplayProjectName(vRows) {
            var selProjectName = $('.selProjectName');

            selProjectName.empty();

            $('#selProjectNameDashboard').append(new Option('All Projects', ''));
            $('#selProjectNameMatReq').append(new Option('Please Select', ''));

            for (var i = 0; i < vRows.length; i++) {
                selProjectName.append(new Option(vRows[i].FowstName, vRows[i].ID))
            }

            //$('#selProjectNameDashboard').selectmenu().selectmenu('refresh');
            $('#selProjectNameMatReq').selectmenu().selectmenu('refresh');
        }

        function SaveMaterialsRequisition() {
            var arrMaterialsRequisition = [];
            var reqValue = 0;

            $('#tBodyMaterialsCount').find('tr').each(function () {
                if ($(this).find('td:eq(5)').find('input').val() != "") {
                    reqValue = reqValue + (CDec2($(this).find('td:eq(5)').find('input').val()) * CDec2($(this).find('td:eq(6)').find('select option:selected').attr('cost')));
                    arrMaterialsRequisition.push({
                        ID: $(this).attr('dbid'),
                        OrgID: sessionStorage.OrgID,
                        ParentID: $(this).attr('pid'),
                        ProjectID: sessionStorage.ProjectID,
                        RequestedBy: sessionStorage.PeopleID,
                        RequestedValue: ($(this).find('td:eq(5)').find('input').val() == "" ? 0 : $(this).find('td:eq(5)').find('input').val()),
                        VendorID: $(this).find('td:eq(6)').find('select').val(),
                        DescriptionID: $(this).attr('id')
                    })
                }
            })

            $('#txtRequisitionValue').val(reqValue);

            if (OfflineMode) {
                if (MaterialsRequisition.SaveMaterialsRequisition(arrMaterialsRequisition, $('#txtRequisitionValue').val()) == 'saved') {
                    NewMessageBox('Requested Materials has been successfully Saved!', '',
                    [
                        {
                            Text: 'Ok',
                            Click: function () {
                                $('#btnMainHotButton').click();
                            }
                        }
                    ]);
                }
            }
            else {
                var data = {
                    reqArr: arrMaterialsRequisition,
                    reqVal: $('#txtRequisitionValue').val(),
                    reqNo: 'MR ' + $.format.date(new Date(), 'MMdd-yyyy-HHmm-ss'),
                    ProjectID: $('#selProjectNameMatReq').val(),
                    RequestedBy: sessionStorage.PeopleID,
                    OrgID: sessionStorage.OrgID
                }

                AjaxSend({
                    url: 'MaterialsRequisition/SaveMaterialsRequisition',
                    data: data,
                    success: function (vData) {
                        if (vData.Results == "saved") {
                            NewMessageBox('Requested Materials has been successfully Saved!', '',
                            [
                                {
                                    Text: 'Ok',
                                    Click: function () {
                                        $('#btnDashboard').click();
                                        GetRequestMaterialsHeader('', sessionStorage.OrgID, false);
                                    }
                                }
                            ]);
                        }
                        else {
                            NewMessageBox('ERROR: ' + vData.Results);
                        }
                    }
                })
            }
        }

        function GetProjectBudgetByProject(data) {
            $('#divMainHotButtons').css('text-align', 'center').text('Please wait while loading ...')

            if (OfflineMode) {
                arrProjectBudgetByProject = MaterialsRequisition.GetProjectBudgetByProject(data).Rows;
                LoadHotButtons(arrProjectBudgetByProject);
            }
            else {
                AjaxSend({
                    url: 'MaterialsRequisition/GetProjectBudgetByProject',
                    data: data,
                    success: function (vData) {
                        arrProjectBudgetByProject = vData.Results.Rows;
                        LoadHotButtons(arrProjectBudgetByProject);
                    }
                })
            }
        }

        function LoadHotButtons(data) {

            var arrParentsBudget = $.grep(data, function (v) {
                return v.IsParent == "1"
            })

            var ctr = 1;
            var hotButtons = $('#divMainHotButtons');
            var grid = $('<div class="ui-grid-b ui-responsive">');
            var block = 'a';
            var newGrid = true;

            hotButtons.empty();

            for (var i = 0; i < arrParentsBudget.length; i++) {
                var text = arrParentsBudget[i].ItemNumber + '<br> ' + arrParentsBudget[i].LineDescription;
                if (ctr == 1) {
                    block = 'a';
                }
                else if (ctr == 2) {
                    block = 'b';
                }
                else if (ctr == 3) {
                    block = 'c';
                }

                grid.append($('<div id="divHotButton_' + i + '" class="ui-block-' + block + '" style="padding:10px;">'))

                if (newGrid) {
                    hotButtons.append(grid);
                }

                $('#divHotButton_' + i).append($('<a class="btn custom-btn-theme-c border-c" style="width:100%;">')
                    .append($('<div style="padding: 10px;">')
                        .append($('<div style="padding: 10px;">')
                            .append($('<div style="width:100%; height:110px; line-height:105px; text-align: center; font-size:18px; font-weight:normal; white-space:normal;">')
                                .append($('<span style="display: inline-block; vertical-align: middle; line-height: normal; text-transform:capitalize">')
                                    .html(text)
                                )
                            )
                        )
                    )
                )
                .attr('nme', text)
                .data('data', arrParentsBudget[i])
                .click(function () {
                    if (CheckPermissions("Make Materials Requisition") == false) {
                        NewMessageBox('You don\'t have permission to make a material requisition');
                        return false;
                    }

                    var dis = $(this);
                    sessionStorage["ParentItemName"] = dis.attr('nme').replace('<br>', '');

                    $('#divParentHotButtons > button').removeClass('custom-btn-theme-b').addClass('custom-btn-theme-c');

                    $('#divParentHotButtons').append($('<button>')
                        .addClass('btn custom-btn-theme-b btn-sm btn-less-width')
                        .css('text-transform', 'capitalize')
                        .attr('title', dis.data('data').LineDescription)
                        .attr('ctr', 1)
                        .text(sessionStorage["ParentItemName"])
                        .click(function () {
                            if ($(this).closest('button').next().length != 0) {
                                $('.divSubHotButon, #divItemCount').hide();
                                $('#divSubHotButtons').show();
                                $('div[subno="1"]').show();
                                RemoveHotButton($(this));
                            }
                        })
                    ).scrollLeft(1500)

                    var data = FilterBySelectedItemNo(dis.data('data'));

                    if (data.HasChild) {
                        $('#divMainHotButtons, #divItemCount').hide();
                        $('#divSubHotButtons').show();
                        $('#divSubHotButtons').empty();
                        ShowSubChild(data.Arr);
                    }
                    else {
                        $('#txtRequisitionValue').val(0);
                        $('#divMainHotButtons, #divSubHotButtons').hide();
                        $('#divItemCount').show();
                        MaterialsCount(data.Arr);
                    }
                })

                ctr = ctr + 1;
                newGrid = false;
            }
        }

        function ShowSubChild(arrParentChild) {
            var subHotButtons = $('#divSubHotButtons');

            var ctr = 1;
            var ctrSubNo = $('#divSubHotButtons > div[subno]');
            var grid = $('<div class="ui-grid-b ui-responsive divSubHotButon" subno="' + (ctrSubNo.length > 0 ? ctrSubNo.length + 1 : 1) + '">');
            var block = 'a';
            var newGrid = true;
            var arrUngrouped = [];
            var isUngroupedExist = false;

            $('.divSubHotButon').hide();

            for (var i = 0; i < arrParentChild.length; i++) {
                var text = arrParentChild[i].ItemNumber + '<br> ' + arrParentChild[i].LineDescription;
                var arrItemChild = [];

                if (ctr == 1) {
                    block = 'a';
                }
                else if (ctr == 2) {
                    block = 'b';
                }
                else if (ctr == 3) {
                    block = 'c';
                }

                grid.append($('<div id="divSubHotButton_' + (ctrSubNo.length > 0 ? ctrSubNo.length + 1 : 1) + '_' + i + '" class="ui-block-' + block + '" style="padding:10px; display:block;">'))

                if (newGrid) {
                    subHotButtons.append(grid);
                }

                if (arrParentChild[i].HasChild == true) {
                    text = '<span style="color:#F00; float:left; font-size:30px;">*</span> ' + arrParentChild[i].ItemNumber + '<br> ' + arrParentChild[i].LineDescription;

                    $('#divSubHotButton_' + (ctrSubNo.length > 0 ? ctrSubNo.length + 1 : 1) + '_' + i).append($('<a class="btn custom-btn-theme-c border-c" style="width:100%;">')
                        .append($('<div style="padding: 10px;">')
                            .append($('<div style="padding: 10px;">')
                                .append($('<div style="width:100%; height:110px; line-height:110px; text-align: center; font-size:18px; font-weight:normal; white-space:normal;">')
                                    .append($('<span style="display: inline-block; vertical-align: middle; line-height: normal; text-transform:capitalize">')
                                        .html(text)
                                    )
                                )
                            )
                        )
                    )
                    .attr('nme', arrParentChild[i].ItemNumber + ' ' + arrParentChild[i].LineDescription)
                    .data('data', arrParentChild[i])
                    .click(function () {
                        if (CheckPermissions("Make Materials Requisition") == false) {
                            NewMessageBox('You don\'t have permission to make a material requisition');
                            return false;
                        }

                        var dis = $(this);
                        sessionStorage["ParentItemName"] = dis.attr('nme').replace('<br>', '');

                        $('#divParentHotButtons > button').removeClass('custom-btn-theme-b').addClass('custom-btn-theme-c');

                        $('#divParentHotButtons').append($('<button>')
                                    .addClass('btn custom-btn-theme-b btn-sm btn-less-width')
                                    .css('text-transform', 'capitalize')
                                    .css('margin-left', '4px')
                                    .attr('title', dis.data('data').LineDescription)
                                    .attr('ctr', (CInt2($('#divParentHotButtons > button:last').attr('ctr')) + 1))
                                    .text(sessionStorage["ParentItemName"])
                                    .click(function () {
                                        if ($(this).closest('button').next().length != 0) {
                                            $('.divSubHotButon, #divItemCount').hide();
                                            $('#divSubHotButtons').show();
                                            $('div[subno="' + $(this).attr('ctr') + '"]').show();
                                            RemoveHotButton($(this));
                                        }
                                    })
                                ).scrollLeft(1500)

                        var data = FilterBySelectedItemNo(dis.data('data'));

                        if (data.HasChild) {
                            $('#divMainHotButtons, #divItemCount').hide();
                            $('#divSubHotButtons').show();
                            ShowSubChild(data.Arr);
                        }
                        else {
                            $('#txtRequisitionValue').val(0);
                            $('#divMainHotButtons, #divSubHotButtons').hide();
                            $('#divItemCount').show();
                            MaterialsCount(data.Arr);
                        }
                    })

                    ctr = ctr + 1;
                }
                else {
                    if (isUngroupedExist == false) {
                        $('#divSubHotButton_' + (ctrSubNo.length > 0 ? ctrSubNo.length + 1 : 1) + '_' + i).append($('<a class="btn custom-btn-theme-c border-c" style="width:100%;">')
                            .append($('<div style="padding: 10px;">')
                                .append($('<div style="padding: 10px;">')
                                    .append($('<div style="width:100%; height:110px; line-height:105px; text-align: center; font-size:18px; font-weight:normal; white-space:normal;">')
                                        .append($('<span style="display: inline-block; vertical-align: middle; line-height: normal; text-transform:capitalize">')
                                            .html('Ungrouped Materials')
                                        )
                                    )
                                )
                            )
                        )
                        .attr('nme', 'Ungrouped Materials')
                        .data('data', arrParentChild[i])
                        .click(function () {
                            var dis = $(this);
                            sessionStorage["ParentItemName"] = dis.attr('nme');

                            $('#divParentHotButtons > button').removeClass('custom-btn-theme-b').addClass('custom-btn-theme-c');

                            $('#divParentHotButtons').append($('<button>')
                                        .addClass('btn custom-btn-theme-b btn-sm btn-less-width')
                                        .css('text-transform', 'capitalize')
                                        .css('margin-left', '4px')
                                        .attr('title', 'Ungrouped Materials')
                                        .attr('ctr', (CInt2($('#divParentHotButtons > button:last').attr('ctr')) + 1))
                                        .text('Ungrouped Materials')
                                        .click(function () {
                                            if ($(this).closest('button').next().length != 0) {
                                                $('.divSubHotButon, #divSubHotButtons').hide();
                                                $('#divItemCount').show();
                                            }
                                        })
                                    ).scrollLeft(1500)

                            $('#divMainHotButtons, #divSubHotButtons').hide();
                            $('#divItemCount').show();
                            MaterialsCount(arrUngrouped);
                        })

                        isUngroupedExist = true;
                        ctr = ctr + 1;
                    } else {
                        $('#divSubHotButton_' + (ctrSubNo.length > 0 ? ctrSubNo.length + 1 : 1) + '_' + i).remove();
                    }

                    arrUngrouped.push(arrParentChild[i]);
                }
                
                newGrid = false;
            }
        }

        function FilterBySelectedItemNo(dis) {
            var parentNo = (CBool(dis.IsParent) == true ? dis.ItemNumber : dis.ItemNumber.replace(/\./g, ''));
            var isHasAChild = false;

            arrParentChild = $.grep(arrProjectBudgetByProject, function (v) {
                var cntdot = (v.ItemNumber.match(/\./g) == null ? 1 : v.ItemNumber.match(/\./g).length);
                var childno = v.ItemNumber.replace(/\./g, '');

                if (CBool(dis.IsParent) == true) {
                    return childno.substr(0, 1) == parentNo && v.IsParent != "1";
                }
                else {
                    return childno.substr(0, cntdot) == parentNo && v.IsParent != "1";
                }
            })

            //Check if child have a child
            for (var i = 0; i < arrParentChild.length; i++) {
                var no = arrParentChild[i].ItemNumber;
                var ctr = 0;
                $.grep(arrParentChild, function (v) {
                    if (v.ItemNumber != no) {
                        if (v.ItemNumber.indexOf(no) > -1) {
                            arrParentChild[i].HasChild = true;
                        }
                    }
                })

                if (arrParentChild[i].HasChild == true) {
                    isHasAChild = true;
                }
            }

            //Remove the child of FIRST CHILD :)
            arrParentChild = $.grep(arrParentChild, function (v) {
                return v.ParentID.replace(/\./g, '') == parentNo;
            })

            //For Item that has no child - (this condition TEMP. EXCLUDED)
            if (arrParentChild.length == 0) {
                arrParentChild = $.grep(arrProjectBudgetByProject, function (v) {
                    return v.ItemNumber == dis.ItemNumber;
                })
            }

            var pData = {
                HasChild: isHasAChild,
                Arr: arrParentChild
            }

            return pData;
        }

        function RemoveHotButton(dis) {
            dis.removeClass('custom-btn-theme-c').addClass('custom-btn-theme-b');
            dis.closest('button').nextAll().each(function () {
                $('div[subno="' + $(this).attr('ctr') + '"]').remove();
                $(this).remove();
            })
        }

        function MaterialsCount(arr) {
            var tBodyMaterialsCount = $('#tBodyMaterialsCount');

            $('#txtParentItemName').text(sessionStorage.ParentItemName);
            tBodyMaterialsCount.empty().append('<tr><td colspan="9" style="text-align:center">Please wait while loading ...</td></tr>');

            if (arr.length > 0) {
                tBodyMaterialsCount.empty();
                for (var i = 0; i < arr.length; i++) {
                    tBodyMaterialsCount.append($('<tr>')
                        .attr('id', arr[i].ID)
                        .attr('pid', arr[i].ParentID)
                        .append($('<td>')
                            .text(arr[i].ItemNumber)
                        )
                        .append($('<td>')
                            .text(arr[i].LineDescription)
                        )
                        .append($('<td>')
                            .css('text-align', 'right')
                            .text(arr[i].ProjectedDMQty)
                        )
                        .append($('<td>')
                            .text(arr[i].DMUnitOfMeasure)
                        )
                        .append($('<td>')
                            .attr('id', 'ds_' + arr[i].ID)
                            .css('text-align', 'center')
                            .html('<img alt="loading" src="/Content/images/ajax-loader.gif" align="center">')
                        )
                        .append($('<td>')
                            .append($('<input type="number">')
                            )
                        )
                        .append($('<td>')
                            .append($('<select data-role="none" class="selMaterialsRequisition select custom-theme-c" id="sel_' + arr[i].ID + '">')
                                        .append(new Option('Please Select', ''))
                            )
                        )
                    )
                }

                var pData = {
                    ParentID: arr[0].ParentID, // 0 bcoz every loop are same parentID
                    ProjectID: sessionStorage.ProjectID,
                    OrgID: sessionStorage.OrgID
                }

                $('#tBodyMaterialsCount').trigger('create');
                GetMaterialsVendor(pData);
                GetTotalMaterialsRequisitionFromServer(pData, arr);
            }
            else {
                tBodyMaterialsCount.empty().append($('<tr>')
                        .append($('<td>')
                            .attr('colspan', 9)
                            .css('text-align', 'center')
                            .text('No Result(s) Found')
                        )
                    )
            }
        }

        function GetMaterialsVendor(pData) {
            AjaxSend({
                url: 'MaterialsRequisition/GetMaterialsVendor',
                data: pData,
                success: function (vData) {
                    if (vData.Results.Rows.length > 0) {
                        for (var i = 0; i < vData.Results.Rows.length; i++) {
                            $('#sel_' + vData.Results.Rows[i].ProjectBudgetID)
                                .append($('<option>', {
                                    text: (vData.Results.Rows[i].CompanyName || vData.Results.Rows[i].PeopleName),
                                    value: (vData.Results.Rows[i].CompanyID || vData.Results.Rows[i].PeopleID),
                                    selected: (vData.Results.Rows[i].IsPreferredCanvass == "1" ? true : false),
                                    cost: vData.Results.Rows[i].Cost
                                })
                                )
                        }
                    }

                    $('#tBodyMaterialsCount').trigger('create');
                    //$('select').selectmenu('refresh')
                }
            })
        }

        function GetTotalMaterialsRequisitionFromServer(data, arr) {
            AjaxSend({
                url: 'MaterialsRequisition/GetTotalMaterialsRequisitionFromServer',
                data: data,
                success: function (vData) {
                    if (vData.Results.Rows.length > 0) {
                        for (var i = 0; i < vData.Results.Rows.length; i++) {
                            var row = $('#tBodyMaterialsCount tr[id="' + vData.Results.Rows[i].ProductID + '"]');
                            row
                                .find('td:eq(4)')
                                .css('text-align', 'center')
                                .attr('fs', 1) // fs (fromserver)
                                .text(CDec2(row.find('td:eq(2)').text()) - CDec2(vData.Results.Rows[i].TotalNo));
                        }
                    }

                    for (var i = 0; i < arr.length; i++) {
                        var row = $('#tBodyMaterialsCount tr[id="' + arr[i].ID + '"]');
                        var isFromServer = row.find('td:eq(4)').attr('fs'); // fs (fromserver)

                        if (!CBool(isFromServer)) {
                            row.find('td:eq(4)').css('text-align', 'center').text(CDec2(row.find('td:eq(2)').text()) - CDec2(0));
                        }
                    }
                },
                error: function () {
                    for (var i = 0; i < arr.length; i++) {
                        var row = $('#tBodyMaterialsCount tr[id="' + arr[i].ID + '"]');
                        var isFromServer = row.find('td:eq(4)').attr('fs'); // fs (fromserver)

                        if (!CBool(isFromServer)) {
                            row.find('td:eq(4)').css('text-align', 'center').text(CDec2(row.find('td:eq(2)').text()) - CDec2(0));
                        }
                    }
                }
            })
        }

        function CheckPermissions(desc) {
            var isAllowed = true;

            if ($.inArray(desc, arrAccessRights) > -1) {
                isAllowed = true;
            }
            else {
                isAllowed = false;
            }

            return CBool(isAllowed);
        }

        function GetAccessRights() {
            if (OfflineMode) {
                arrAccessRights = [];
                var result = BOMMain.GetAccessRights().Rows;

                for (var i = 0; i < result.length; i++) {
                    arrAccessRights.push(result[i].Description);
                }
            }
            else {
                AjaxSend({
                    url: 'SetupChecklist/GetAccessRights',
                    data: { id: sessionStorage.ProjectID },
                    success: function (vData) {
                        arrAccessRights = [];
                        for (var i = 0; i < vData.Results.Rows.length; i++) {
                            arrAccessRights.push(vData.Results.Rows[i].Description);
                        }
                    }
                })
            }
        }
    })
})