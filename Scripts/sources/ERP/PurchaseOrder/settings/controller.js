define([
    '/Scripts/sources/ERP/PurchaseOrder/settings/model.js',
    'promise!/Scripts/sources/ERP/Invoicing/settings/popDocumentControl/'+ (proj.IsBootstrapDesign() ? 'Bs_' : '') +'controller.js'
], function () {

    var mDeferred = $.Deferred();
    var mHtml;

    (function staticInit() {

        AjaxSend({
            ExactUrl: '/Scripts/sources/ERP/PurchaseOrder/settings/view.html?v=' + assetversion,
            type: 'GET',
            dataType: 'text',
            success: function (vData) {
                mHtml = vData;
                ERP.PurchaseOrder.settings.controller = ERP.PurchaseOrder.settings.controller(); // singleton
                mDeferred.resolve();
            }
        });

    })();

    CreateStringObject('ERP.PurchaseOrder.settings.controller', null, function () {

        var mOut = {};
        var mElement;
        var employeeID = '', PLID = '';

        mElement = $(mHtml);
        mOut.element = mElement;
        mOut.Model = new ERP.PurchaseOrder.settings.model();

        var pPopDocumentControl = new ERP.Invoicing.settings.popDocumentControl.controller();
        var mChkAllowEncodingOfOrdersInInvoice = mElement.find('[name=AllowEncodingOfOrdersInInvoice]');
        var mChkAllowEncodingOfQuantitiesInInvoice = mElement.find('[name=AllowEncodingOfQuantitiesInInvoice]');

        mElement.find('[name=butSave]').click(function() {
            mOut.Model.Save({
                AllowEncodingOfOrdersInInvoice: mChkAllowEncodingOfOrdersInInvoice.prop('checked'),
                AllowEncodingOfQuantitiesInInvoice: mChkAllowEncodingOfQuantitiesInInvoice.prop('checked'),
                Success: function() {
                    NewMessageBox('Settings were successfully saved.');
                }
            });
        });

        mElement.find('#tabPOSettingsPOLink form').submit(function (e) {
            e.preventDefault();
            var formData = new FormData($(this)[0]);
            $.ajax({
                url: '/PurchaseOrder/SaveSettings?OrgID=' + sessionStorage.OrgID,
                type: 'POST',
                data: formData,
                async: false,
                cache: false,
                contentType: false,
                processData: false,
                success: function (vData) {
                    try {
                        vData = JSON.parse(vData);
                        if (vData.d)
                            vData = vData.d;
                    } catch (e) {
                        NewMessageBox('An error occured while parsing the server response to JSON: ' + e.message);
                        return;
                    }
                    if (vData.Error)
                        NewMessageBox(vData.Error);
                    else if (vData.Warning)
                        NewMessageBox(vData.Warning);
                    else if (vData.Results == 'ok' || vData.Results == 'saved')
                        NewMessageBox('The data was successfully saved.');
                    else
                        NewMessageBox('Unknown status. ref js8TWY3ZAGW9!');
                }
            });
        });

        mElement.find('[name=btnAddEmployee]').click(function () {
            var modal = mElement.find('[name="popupPurchaseLimits"]');
            
            modal.on('show.bs.modal', function () {
                modal.find('input').val('');
                employeeID = '';
                PLID = '';
            }).modal('show');

            modal.find('form').unbind('submit').submit(function (e) {
                e.preventDefault();
                var formData = new FormData($(this)[0]);

                formData.append('PLID', PLID);
                formData.append('EmployeeID', employeeID);
                formData.append('UpdatedBy', sessionStorage.PeopleID);

                $.ajax({
                    url: '/PurchaseOrder/SavePurchasingLimits?OrgID=' + sessionStorage.OrgID,
                    type: 'POST',
                    data: formData,
                    async: false,
                    cache: false,
                    contentType: false,
                    processData: false,
                    success: function (vData) {
                        try {
                            vData = JSON.parse(vData);
                            if (vData.d)
                                vData = vData.d;
                        } catch (e) {
                            NewMessageBox('An error occured while parsing the server response to JSON: ' + e.message);
                            return;
                        }

                        if (vData.Error != undefined)
                            NewMessageBox(vData.Error);
                        else if (vData.Exist != undefined)
                            NewMessageBox('The selected employee has already been added to the list.');
                        else {
                            mOut.GetSettings();
                            WebToast('Successfully saved');
                            modal.modal('hide');
                        }
                    }
                });
            })
        });

        mElement.find('[name=txtEmployeeName]').click(function () {
            BrowsePopup('LibraryOrganizationEmployee', 'Select Employee', [
                {
                    DisplayText: [{ Column: 'PeopleName' }],
                    Click: function (e) {
                        employeeID = e.ID;
                        mElement.find('[name="txtEmployeeName"]').val(e.PeopleName);
                    }
                }
            ])
        });

        mOut.Initialize = function() {
            $('#tabPurchasingSettings').html('').append(mElement);
            mOut.Load();
            mOut.LoadDocumentControl();
            pPopDocumentControl.AfterSave = mOut.LoadDocumentControl;
            mOut.GetSettings();
        };

        mOut.Load = function() {
            mOut.Model.Load({
                callback: function(vData) {
                    mChkAllowEncodingOfOrdersInInvoice.prop('checked', vData.AllowEncodingOfOrdersInInvoice);
                    mChkAllowEncodingOfQuantitiesInInvoice.prop('checked', vData.AllowEncodingOfQuantitiesInInvoice);
                }
            });
        };

        mOut.GetSettings = function () {
            mOut.Model.GetSettings({
                callback: function (vData) {
                    mElement.find('[name="expirationdaysforpolink"]').val(vData.ExpirationDaysForPOLink);
                    mElement.find('[name="polinkdomain"]').val(vData.POLinkDomainName);
                    mElement.find('[name="polinkexternalipaddress"]').val(vData.POLinkDomainExternalIPAddress);
                    mElement.find('[name="polinkinternalipaddress"]').val(vData.POLinkDomainInternalIPAddress);
                    DisplayPurchasingLimits(vData.PurchasingLimits.Rows);
                }
            });
        }

        var tblPOSettingsPOLimits;
        function DisplayPurchasingLimits(data) {
            tblPOSettingsPOLimits = mElement.find('[name="tblPOSettingsPOLimits"]'), ctr = 0;

            if ($.fn.DataTable.isDataTable(tblPOSettingsPOLimits)) {
                //empty the table first before destroying datatabase para dili magdoble ang header
                tblPOSettingsPOLimits.empty();
                tblPOSettingsPOLimitsDT.destroy();
            }

            tblPOSettingsPOLimitsDT = tblPOSettingsPOLimits.DataTable({
                pageLength: 10,
                "ordering": false,
                "data": data,
                "initComplete": function () {
                    tblPOSettingsPOLimits.removeClass('dataTable').css('width', '100%');

                    mElement.find('[name="txtSearch"]').unbind('keyup').keyup(function () {
                        tblPOSettingsPOLimitsDT.search($(this).val()).draw();
                    })
                },
                rowCallback: function (row, data, displayNum, displayIndex, dataIndex) {
                    $(row).data('data', data).click(function (e) {
                        e.stopPropagation();
                        var vData = $(this).data('data');

                        mElement.find('[name=btnAddEmployee]').click();

                        employeeID = vData.EmployeeID;
                        PLID = vData.ID;
                        mElement.find('[name="txtEmployeeName"]').val(vData.EmployeeName);
                        mElement.find('[name="txtPurchasingLimitsPerPO"]').val(CDec2(vData.LimitsPerPO));
                        mElement.find('[name="txtPurchasingLimitsPerMonth"]').val(CDec2(vData.LimitsPerMonth));
                    })
                },
                "columns": [
                    {
                        "data": function (data) {
                            ctr += 1;
                            return ctr + '.';
                        }, width: '50px'
                    },
                    {
                        "data": "EmployeeName"
                    },
                    {
                        "data": function(data) {
                            return AccountingFormat(data.LimitsPerPO);
                        }, className: 'text-center'
                    },
                    {
                        "data": function (data) {
                            return AccountingFormat(data.LimitsPerMonth);
                        }, className: 'text-center'
                    },
                    {
                        "data": function (data) {
                            return data.UpdatedByName + '<br>on ' + $.format.date(new Date(data.LastUpdated), 'MM-dd-yyyy hh:mm a');
                        }
                    },
                    {
                        "data": function (data) {
                            return "<button data-role='none' class='btn btn-danger'><span class='bi bi-trash'></span></button>";
                        },
                        createdCell: function (td, cellData, rowData, row, col) {
                            var btn = $(td).find('button');

                            rowData.RowIndex = row;

                            btn.data('data', rowData).unbind('click').click(function (e) {
                                e.stopPropagation();

                                NewMessageBox('Are you sure you want to remove Puchasing Limits for <i><b>' + rowData.EmployeeName + '</b></i>?', 'Confirmation Dialog', [
                                    {
                                        Text: 'Yes', Click: function () {
                                            AjaxSend({
                                                url: '/PurchaseOrder/DeletePurchasingLimit',
                                                data: { PLID: rowData.ID },
                                                success: function () {
                                                    WebToast('Successfully removed');
                                                    mOut.GetSettings();
                                                }
                                            })
                                        }
                                    }, { Text: 'No' }
                                ])
                            })
                        }, className: 'text-right'
                    }
                ],
                "sDom": '<"top">rt<"bottom"ip><"clear">'
            });
        }

        var mDocumentControlTable = mElement.find('[name=tableDocumentControl]');
        var mDocumentControlRow = mDocumentControlTable.find('tbody tr');
        mDocumentControlRow.remove();
        mOut.LoadDocumentControl = function() {
            mOut.Model.LoadDocumentControl({
                callback: function(vData) {
                    DisplayDocumentControl(vData);
                }
            });
        };

        function DisplayDocumentControl(vData) {
            mDocumentControlTable.find('tbody tr').remove();
            for (var p = 0; p <= vData.Rows.length - 1; p++) {
                var pDR = vData.Rows[p];
                var pTR = mDocumentControlRow.clone();
                pTR.data('data', pDR);
                pTR.find('[name=DocumentName]').text(pDR.DocumentName);
                pTR.find('[name=ModuleSource]').text(pDR.ModuleSource);
                pTR.find('[name=PrintLimit]').text(PrintLimitText(pDR.PrintLimit));
                pTR.find('[name=CopiesPerPrint]').text(pDR.CopiesPerPrint);
                pTR.find('[name=LabelOfCopies]').text(pDR.LabelOfCopies);
                pTR.find('[name=Active]').text(CBool(pDR.IsActive) ? 'Yes' : 'No');
                pTR.click(function(e) {
                    var pDR = $(this).data('data');
                    DocumentControlItem_Click(pDR);
                });
                mDocumentControlTable.append(pTR);
            }
        }

        function PrintLimitText(vValue) {
            if (vValue == -1)
                return 'Print Many Times';
            else if (vValue == 1)
                return 'Print Once';
            else
                return vValue;
        }

        function DocumentControlItem_Click(vDR) {
            vDR = $.extend({}, vDR);
            PopupButtonList('', vDR.DocumentName, [
                //{
                //    Text: 'Add Document',
                //    Click: function() {
                //        AddProduct();
                //    }
                //},
                {
                    Text: 'Edit Document',
                    Click: function() {
                        pPopDocumentControl.popup(vDR);
                    }
                },
                //{
                //    Text: 'Remove Document',
                //    Click: function() {
                //        RemoveFromCartPrompt(vProductID, vDR);
                //    }
                //}
            ]);
            
        }

        mOut.Initialize();

        return mOut;
    }); // END CreateStringObject('App.controller.PriceChange', null, function() {

    return mDeferred.promise();

});