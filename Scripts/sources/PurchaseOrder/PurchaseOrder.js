require([
        'common'
], function () {
    $(function () {
        $(document).ready(function () {

            window.PurchaseOrder = {
                GetPOData: function () {
                    GetPOData();
                }
            }
            GetPOData();

            $('body')
            .on('click', '#btnSavePurchaseOrderSettings', function () {
                SavePurchaseOrderSettings();
            })
            .on('click', '#btnCancelPurchaseOrderSettings', function () {
                window.location.hash = 'SystemAdministration';
            })
        })

        function GetPOData() {
            AjaxSend({
                url: '/PurchaseOrder/GetPOData',
                success: function (vData) {
                    if (vData.Results.Rows.length > 0) {
                        $('#txtPurchaseOrderID').val(vData.Results.Rows[0].ID);
                        $('#selPurchaseOrderSlider').val(vData.Results.Rows[0].IsRestrict).slider('refresh');
                    }
                }
            })
        }

        function SavePurchaseOrderSettings() {
            var pData = {};

            pData.ID = $.trim($('#txtPurchaseOrderID').val());
            pData.IsRestrict = $.trim($('#selPurchaseOrderSlider').val());

            AjaxSend({
                url: '/PurchaseOrder/SavePOData',
                data: pData,
                success: function (vData) {
                    if (vData.Results == "saved") {
                        NewMessageBox('Successfully Saved', 'Purchase Order Settings', [
                            {
                                Text: 'Ok',
                                Click: function () {
                                    window.location.hash = 'SystemAdministration';
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
    })
})