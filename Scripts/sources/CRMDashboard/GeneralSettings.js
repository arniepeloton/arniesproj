require(['common'],
function () {
    $(document).ready(function () {
        window.CRMSettings = {
            Refresh: Refresh
        }

        Refresh();

        $('body')
        .on('click', '#btnSaveCRMSettings', function () {
            SaveSettings();
        })
        .on('click', '#btnCancelCRMSettings', function () {
            window.location.hash = 'LibraryPage';
        })

        function SaveSettings() {
            var pData = {
                IsAutoImport: CInt2($('#selCRMAutoImport').is(':checked')),
                IsAutoImportCrmToCustomer: CInt2($('#chbAutoImportToCustomer').is(':checked')),
                IsAutoImportCrmToSupplier: CInt2($('#chbAutoImportToSupplier').is(':checked')),
            }

            AjaxSend({
                url: 'CRMDashboard/SaveSystemBehavior',
                data: pData,
                success: function (e) {
                    if(e.Result == 'Success'){
                        WebToast('Settings was successfully saved', 'top');
                        window.location.hash = 'LibraryPage';
                    }
                }
            })
        }

        function Refresh() {
            ClearFields();

            AjaxSend({
                url: 'CRMDashboard/GetSystemBehavior',
                success: function (vData) {
                    $('#selCRMAutoImport').attr('checked', CBool(vData.IsAutoImport))//.slider('refresh');
                    $('#chbAutoImportToCustomer').attr('checked', CBool(vData.IsAutoImportCrmToCustomer))//.checkboxradio('refresh');
                    $('#chbAutoImportToSupplier').attr('checked', CBool(vData.IsAutoImportCrmToSupplier))//.checkboxradio('refresh');
                }
            })
        }

        function ClearFields() {
            $('#selCRMAutoImport').attr('checked', 1)//.slider('refresh');
            $('#chbAutoImportToCustomer').attr('checked', CBool(0))//.checkboxradio('refresh');
            $('#chbAutoImportToSupplier').attr('checked', CBool(0))//.checkboxradio('refresh');
        }
    })
})