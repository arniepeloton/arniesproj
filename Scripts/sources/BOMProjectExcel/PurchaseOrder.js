require([
    'common'
], function () {
    var VendorID;
    var POID;

    $(document).ready(function () {

        window.PO = {
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
        .on('keypress', '#txtSearchPO', function (e) {
            if (e.keyCode == 13) {
                if ($.trim($(this).val()) != "") {
                    GetPO({ Searchable: true, filterBy: $(this).val(), SearchedText: $.trim($(this).val()) });
                }
                else {
                    GetPO({ Searchable: false, filterBy: $('#selProjectList').val(), SearchedText: '' });
                }
            }
        })
        .on('click', '#btnNewPO', function () {
            location = MergeUrlParameters({
                OrgID: sessionStorage.OrgID,
                mp: 'mc',
                IsMobile: CBool(IsMobile) ? 1 : 0
            }, '/MaterialsRequisition')
            return;
            PopupButtonList('', 'Create New PO',
            [
                {
                    Text: 'Create from Materials Requisition',
                    Click: function () {

                    }
                },
                {
                    Text: 'New Blank PO',
                    Click: function () {

                    }
                }
            ])
        })
        .on('click', '#btnCancelPO', function () {
            $('#popPO').hide();
        })
        .on('click', '#btnDownloadPO', function () {
            $(this).addClass('btn-disabled').text('Downloading ...');
            DownloadPOPDF($('#txtPOID').val());
        })
        .on('click', '#btnRefreshProjectPO', function () {
            GetPO({ Searchable: false, filterBy: $('#selProjectList').val(), SearchedText: '' });
        })
    }) // END $(document)

    function DownloadPOPDF(poid) {
        AjaxSend({
            url: 'MaterialsRequisition/GetOrgDetails',
            data: { OrganizationID: sessionStorage.OrgID },
            success: function (vDataOrg) {
                var vRowsOrgDetails = vDataOrg.OrgDetails.Rows;
                var vRowsOrgTelDetails = vDataOrg.OrgTelDetails.Rows;

                AjaxSend({
                    url: 'ProjectPurchaseOrder/GetPO',
                    data: { POID: POID, Searchable: false },
                    success: function (vDataPO) {
                        var vRowsProject = vDataPO.Results.Rows;

                        AjaxSend({
                            url: 'MaterialsRequisition/GetVendorItem',
                            data: { VendorID: VendorID, IsPO: true, POID: POID },
                            success: function (vDataItems) {
                                var arrHeaderDetails = [];
                                var arrSupplier = [];

                                arrHeaderDetails.push([
                                            { "caseOperation": "purchaseOrder" },
                                            { "docType": "poNum" },
                                            { "docNum": vRowsProject[0].InvoiceType + ' ' + vRowsProject[0].PONumber },
                                            { "docTypeDate": $.format.date(new Date(vRowsProject[0].DateCreated), 'EEEE MMM dd, yyyy') },
                                            { "dateData": $.format.date(new Date(vRowsProject[0].DateCreated), 'EEEE MMM dd, yyyy') },
                                            { "docTypeDelivery": "poDeliveryDate" },
                                            { "deliveryDate": $.format.date(new Date(vRowsProject[0].DeliveryDate), 'EEEE MMM dd, yyyy') },
                                            { "docTypeTotal": "poTotal" },
                                            { "totalData": "100,488.00" },
                                            { "orgNameRaw": vRowsOrgDetails[0].CompanyName },
                                            { "orgAddressRaw": vRowsOrgDetails[0].BusinessAddressBuildingNo + ' ' + vRowsOrgDetails[0].BusinessAddressStreet + ' ' + vRowsOrgDetails[0].BusinessAddressCity + ' ' + vRowsOrgDetails[0].BusinessAddressCountry },
                                            { "custNameRaw": vRowsProject[0].SupplierName },
                                            { "custAddressRaw": (vRowsProject[0].SupplierOrgAddress || vRowsProject[0].SupplierPeopleAddress) },
                                            { "billingAddressDetailsRaw": vRowsProject[0].BillingAddress },
                                            { "billingContactDetailsRaw": (vRowsProject[0].BillingOrgContact || vRowsProject[0].BillingFowstContact) },
                                            { "billingContactOptionalRaw": '' },
                                            { "deliveryAddressDetailsRaw": vRowsProject[0].DeliveryAddress },
                                            { "deliveryContactDetailsRaw": (vRowsProject[0].DeliveryOrgContact || vRowsProject[0].DeliveryFowstContact) },
                                            { "deliveryContactOptionalRaw": '' },
                                            { "deliveryRemarksRaw": vRowsProject[0].Remarks },
                                            { "qrcode": vRowsProject[0].ID },
                                            { "countDate": "FORMOBIZPROJECT" }
                                ]
                                        )
                                PrintPDF(
                                    {
                                        "operation": arrHeaderDetails
                                    }, {
                                        "data": [
                                                   { "columns": ["Item Code:", "Item Description:", "Quantity", "UoM", "Unit Price", "Sub Total"] },
                                                   {
                                                       "rows": [vDataItems.Results.Rows]
                                                   }
                                        ]
                                    }
                                )

                                $('#btnDownloadPO').removeClass('btn-disabled').text('Download P.O');
                            }
                        })
                    }
                })
            }
        })
    }

    function GetPO(data) {
        var tbodyPurchaseOrder = $('#tbodyPurchaseOrder');

        tbodyPurchaseOrder.empty().append('<tr><td colspan="9" style="text-align:center;">Please wait ...</td></tr>');

        AjaxSend({
            url: 'ProjectPurchaseOrder/GetPO',
            data: data,
            success: function (vData) {
                var vRows = vData.Results.Rows;

                DisplayPO(vRows);
            }
        })
    }

    var dtProjetPO;
    function DisplayPO(vRows) {
        var tblProjectPO = $('#tblProjectPO');
        var ctr = 0;

        if ($.fn.DataTable.isDataTable(tblProjectPO)) {
            tblProjectPO.DataTable().clear().destroy();
            //return;
        }

        dtProjetPO = tblProjectPO.DataTable({
            "data": vRows,
            "pageLength": 10,
            "columns": [
                {
                    "data": function (data, type, full, meta) {
                        ctr += 1;
                        return '<b class="ui-table-cell-label">#</b>' + ctr;
                    },
                    "width": "30px"
                },
                {
                    "data": function (data, type, full, meta) {
                        return '<b class="ui-table-cell-label">PO Number:</b>' + data.PONumber;
                    }
                },
                {
                    "data": function (data, type, full, meta) {
                        return '<b class="ui-table-cell-label">Requisition Number:</b>' + data.RequisitionNumber;
                    }
                },
                {
                    "data": function (data, type, full, meta) {
                        return '<b class="ui-table-cell-label">Project Name:</b>' + data.ProjectName
                    }
                },
                {
                    "data": function (data, type, full, meta) {
                        return '<b class="ui-table-cell-label">Supplier Name:</b>' + data.SupplierName
                    }
                },
                {
                    "data": function (data, type, full, meta) {
                        return '<b class="ui-table-cell-label">Requested By:</b>' + data.RequestedBy
                    }
                },
                {
                    "data": function (data, type, full, meta) {
                        return '<b class="ui-table-cell-label">Delivery Date:</b>' + $.format.date(new Date(data.DeliveryDate), 'MMM dd, yyyy')
                    }
                },
                {
                    "data": function (data, type, full, meta) {
                        return '<b class="ui-table-cell-label">Amount:</b>' + AccountingFormat(data.Amount)
                    }
                },
                {
                    "data": function (data, type, full, meta) {
                        return '<b class="ui-table-cell-label">Date and Time Created:</b>' + $.format.date(new Date(data.DateCreated), 'MMM dd, yyyy hh:mm a')
                    }
                }
            ],
            "aoColumnDefs": [{ "bSortable": false, "aTargets": [0] }],
            "responsive": true,
            "columnDefs": [{ "targets": 1, "orderable": true }],
            "sDom": '<"top">rt<"bottom"ip><"clear">'
            //"stateSave": true
        })

        $('#tblProjectPO tbody').unbind('click').on('click', 'tr', function (e) {
            if (dtProjetPO.row(this).data() != undefined) {
                var data = dtProjetPO.row(this).data();
                $('#txtPONumber').text(data.PONumber);
                $('#txtPOProjectName').text(data.ProjectName);
                $('#txtSupplierName').text(data.SupplierName);
                $('#txtSupplierAddress').text((data.SupplierOrgAddress || data.SupplierPeopleAddress));

                POID = data.ID;
                VendorID = data.SupplierID;

                $('#popPO').show();
                GetVendorItem({ VendorID: data.SupplierID, IsPO: true, POID: data.ID })
            }
        });
    }

    function GetVendorItem(data) {
        var tbodyPO = $('#tbodyPO');

        $('#txtTotalPO').text('');
        tbodyPO.empty().append('<tr><td colspan="7" style="text-align:center;">Please wait ...</td></tr>');

        AjaxSend({
            url: 'MaterialsRequisition/GetVendorItem',
            data: data,
            success: function (vData) {
                var vRows = vData.Results.Rows;
                var total = 0;

                if (vRows.length > 0) {
                    tbodyPO.empty();

                    for (var i = 0; i < vRows.length; i++) {
                        var subTotal = CDec2(vRows[i].Qty) * CDec2(vRows[i].Price);
                        total = total + subTotal;

                        tbodyPO.append($('<tr>')
                            .append($('<td>')
                                .text(i + 1 + '.')
                            )
                            .append($('<td>')
                                .text(vRows[i].ItemCode)
                            )
                            .append($('<td>')
                                .text(vRows[i].ItemDescription)
                            )
                            .append($('<td>')
                                .css('text-align', 'right')
                                .text(vRows[i].Qty)
                            )
                            .append($('<td>')
                                .text(vRows[i].UoM)
                            )
                            .append($('<td>')
                                .css('text-align', 'right')
                                .text(AccountingFormat(vRows[i].Price))
                            )
                            .append($('<td>')
                                .css('text-align', 'right')
                                .text(AccountingFormat(subTotal))
                            )
                            .data('data', vRows[i])
                        )
                    }

                    $('#txtTotalPO').text('TOTAL: ' + AccountingFormat(total))
                }
                else {
                    tbodyPO.empty().append($('<tr>')
                            .append($('<td>')
                                .attr('colspan', 7)
                                .css('text-align', 'center')
                                .text('No Result(s) Found')
                            )
                        )
                }

                tbodyPO.parent().table('refresh');
            }
        })
    }

    function GetProjectNameList() {
        $('#pagePOLoading').pageshow();

        if (OfflineMode) {
            DisplayProjectName(OfflineBrowser.LibraryProject.GetData({ FilterFowst: 1 }).Rows);
        }
        else {
            AjaxSend({
                url: 'Browser/LibraryProject',
                success: function (vData) {
                    DisplayProjectName(vData.Results.Rows);
                    $('#pageDashboardPurchaseOrder').pageshow();
                }
            })
        }
    }

    function DisplayProjectName(vRows) {
        var selProjectName = $('#selProjectList');
        var optGroup = $('<optgroup>').attr('label', 'Filter list by:');

        selProjectName.empty();

        optGroup.append(new Option('All Projects', 'all'));

        for (var i = 0; i < vRows.length; i++) {
            optGroup.append(new Option(vRows[i].FowstName, vRows[i].ID))
        }

        selProjectName.append(optGroup).change(function () {
            GetPO({ Searchable: false, filterBy: $(this).val(), SearchedText: '' });
        })

        GetPO({ Searchable: false, filterBy: 'all', SearchedText: '' });
    }
})