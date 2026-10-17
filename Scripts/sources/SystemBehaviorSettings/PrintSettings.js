require([
    'common',
    'promise!/Scripts/sources/ERP/Invoicing/settings/popDocumentControl/Bs_controller.js?v=' + assetversion
], function () {

    var IsSaveDailyBeginningBalance = 0;
    var pPopDocumentControl = new ERP.Invoicing.settings.popDocumentControl.controller();

    $(function () {
        window.PrintSettings = {
            LoadRequiredData: function () {
                GetListOfInvoiceTypes()
            }
        }
        GetListOfInvoiceTypes();

        $('body')
            .on('click', '#btnSavePrintSettings', function () {
                SaveDailyBeginningBalanceSettings();
            })
            .on('click', '#btnPrintSettingsDone', function () {
                history.back();
            })
    });


    function SaveDailyBeginningBalanceSettings() {
        var data = {
            IsAutoPrintOrders: CInt2($('#selIsAutoPrintOrders').is(':checked'))
        };

        AjaxSend({
            url: 'SystemBehaviorSettings/SavePrintSettings',
            data: data,
            success: function (vData) {
                if (vData.Result == "saved") {
                    WebToast('Print Settings has been saved', 'top');
                    history.back();
                } else {
                    NewMessageBox(vData.Result);
                }
            }
        });

    }

    function GetPrinterSettings() {
        AjaxSend({
            url: 'SystemBehaviorSettings/GetPrinterSettings',
            success: function (vData) {

                $('#selIsAutoPrintOrders').prop('checked', CInt2(vData.IsAutoPrintOrders));//.slider('refresh');
            }
        });
    }

    function GetListOfInvoiceTypes() {
        AjaxSend({
            url: '/ERP/GetDocumentControl',
            success: function (e) {
                var vRows = e.Rows;
                var tblBodyAutoPrintInvoice = $('#tblBodyAutoPrintInvoice');

                tblBodyAutoPrintInvoice.empty();

                if (vRows.length > 0) {
                    for (var i = 0; i < vRows.length; i++) {
                        var tr = $('<tr>')
                                .data('data', vRows[i])
                                .append($('<td>').text(i + 1 + '.'))
                                .append($('<td>').text(vRows[i].DocumentName))
                                .append($('<td>').text(vRows[i].ModuleSource))
                                .append($('<td>').text(vRows[i].PrintLimit))
                                .append($('<td>').text(vRows[i].CopiesPerPrint))
                                .append($('<td>').text(vRows[i].LabelOfCopies))
                                .append($('<td>').text(CBool(vRows[i].IsActive) ? 'Yes' : 'No'))
                                .append($('<td>').text(CBool(vRows[i].AutoPrint) ? 'Yes' : 'No'))
                                .click(function () {
                                    pPopDocumentControl.popup($(this).data('data'));
                                    pPopDocumentControl.AfterSave = function () {
                                        GetListOfInvoiceTypes();
                                        pPopDocumentControl.AfterSave = function () { };
                                    }
                                })

                        tblBodyAutoPrintInvoice.append(tr);
                    }
                } else {
                    tblBodyAutoPrintInvoice.append($('<tr>')
                            .append($('<td colspan="5" class="txt-center">').text('No results found'))
                        )
                }

                GetPrinterSettings();
            }
        })
    }
});