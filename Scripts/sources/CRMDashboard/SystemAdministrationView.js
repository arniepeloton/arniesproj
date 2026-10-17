require([
    'common'
], function () {
    $(function () {

        window.CRMLibrary = {};

        CRMLibrary.View = {
            ShowDeviceInfo: function () {
                AjaxSend({
                    url: 'Device/IsNewEntry',
                    success: function (vData) {
                        var pData = {};
                        pData.DeviceID = sessionStorage.ViewID;

                        AjaxSend({
                            url: "Device/getDeviceInfoName",
                            data: pData,
                            success: function (vData) {

                                if (vData.DeviceData.Rows.length > 0) {

                                    $("#txtSumDeviceID").val(vData.DeviceData.Rows[0].ID || '');

                                    $("#txtSumDevicePhoneName").val(vData.DeviceData.Rows[0].PhoneName || '');
                                    $("#txtSumDevicePhoneModel").val(vData.DeviceData.Rows[0].PhoneModel || '');
                                    $("#txtSumDeviceIMEI").val(vData.DeviceData.Rows[0].IMEI || '');
                                    $("#txtSumDeviceUUID").val(vData.DeviceData.Rows[0].UUID || '');
                                    $("#txtSumDeviceUuidDisplay").val(vData.DeviceData.Rows[0].UuidDisplay || '');
                                    $("#txtSumAccreditation").val(vData.DeviceData.Rows[0].AccreditationText || '');
                                    $("#txtSumDeviceOS").val(vData.DeviceData.Rows[0].OS || '');
                                    $("#txtSumDeviceNo").val(vData.DeviceData.Rows[0].DeviceNo || '');

                                    getSumDevicePlaceList();
                                }
                            }
                        });
                    }
                });
            },

            ShowSecurityInfo: function () {
                AjaxSend({
                    url: 'SecurityRole/IsNewEntry',
                    success: function (vData) {
                        var pData = {};
                        pData.SecurityRoleID = vData.SecurityRoleID;

                        AjaxSend({
                            url: "SecurityRole/getSecurityRoleInfo",
                            data: pData,
                            success: function (vData) {

                                if (vData.SecurityRoleData.Rows.length > 0) {

                                    $("#txtSumSecurityRoleID").val(vData.SecurityRoleData.Rows[0].ID || '');
                                    $("#txtSumRoleCode").val(vData.SecurityRoleData.Rows[0].Code || '');
                                    $("#txtSumRole").val(vData.SecurityRoleData.Rows[0].Role || '');
                                    $("#txtSumRoleDescription").val(vData.SecurityRoleData.Rows[0].Description || '');
                                    $('#chkSumRoleIsActive').attr("checked", (vData.SecurityRoleData.Rows[0].IsActive == 1 ? true : false));//.checkboxradio("refresh");

                                }

                                getSumSecRoleUserList();
                            }
                        });
                    }
                });
            },

            ShowDevicePrinterInfo: function () {
                AjaxSend({
                    url: 'DevicePrinter/IsNewEntry',
                    success: function (vData) {
                        var pData = {};
                        pData.DevicePrinterID = vData.DevicePrinterID;
                        AjaxSend({
                            url: "DevicePrinter/getDevicePrinterInfo",
                            data: pData,
                            success: function (vData) {

                                if (vData.DevicePrinterData.Rows.length > 0) {

                                    $("#txtSumDevicePrinterID").val(vData.DevicePrinterData.Rows[0].ID || '');
                                    $("#txtSumDevicePrinterRole").val(vData.DevicePrinterData.Rows[0].PrinterRole || '');
                                    $("#txtSumDevicePrinterType").val(vData.DevicePrinterData.Rows[0].PrinterType || '');

                                    $('#chkSumIsActive').attr("checked", (vData.DevicePrinterData.Rows[0].IsActive == 0 ? false : true));//.checkboxradio("refresh");

                                    getSumDevicePrinterCategories(vData.DevicePrinterData.Rows[0].ID);

                                    if (vData.DevicePrinterData.Rows[0].PrinterRole == 'Point of Sale (POS)' || vData.DevicePrinterData.Rows[0].PrinterRole == 'Sales Order Taker')
                                        $('#divSumCategoriesList').hide();
                                    else
                                        $('#divSumCategoriesList').show();

                                }
                            }
                        });
                    }
                });
            }
        }// End Library View

        function getSumDevicePlaceList() {
            var isFowstListEmpty = false;
            AjaxSend({
                url: 'Device/getDeviceFOWSTList',
                success: function (vData) {
                    var pLV = $('#lstSumDeviceFowst');
                    var pItem;

                    if (vData.DeviceFowstData.Rows.length < 1)
                        isFowstListEmpty = true;

                    pLV.html('');
                    for (var p = 0; p <= vData.DeviceFowstData.Rows.length - 1; p++) {
                        pItem = $('<li>')
                        .append($('<h3>')
                            .attr("style", "white-space:normal")
                            .text(vData.DeviceFowstData.Rows[p].FOWSTName)
                        );
                        pLV.append(pItem);
                    };
                    pLV.listview('refresh');
                }
            });
        }

        function getSumSecRoleUserList() {
            AjaxSend({
                url: 'SecurityRole/getSecurityRoleUserList',
                success: function (vData) {
                    var pLV = $('#lstSumSecurityRoleUser');
                    var pItem;

                    pLV.html('');
                    for (var i = 0; i <= vData.SecurityRoleData.Rows.length - 1; i++) {

                        pItem = $('<a href="#" class="list-group-item list-group-item-action">')
                            .attr('ID', vData.SecurityRoleData.Rows[i].ID)
                            .append($('<h4>')
                                .text(vData.SecurityRoleData.Rows[i].PeopleName)
                            );
                        pLV.append(pItem);
                    }
                    //pLV.listview('refresh');
                }
            });
        }

        function getSumDevicePrinterCategories(id) {

            AjaxSend({
                url: 'DevicePrinter/getDevicePrinterCategoryList',
                data: { PrinterID: id },
                success: function (vData) {
                    var pLV = $('#lstSumDevicePrinterCategories');
                    var pItem;

                    pLV.html('');
                    for (var p = 0; p <= vData.Categories.Rows.length - 1; p++) {
                        pItem = $('<li>')
                        .append($('<h3>')
                            .attr("style", "white-space:normal")
                            .text(vData.Categories.Rows[p].CategoryName)
                        );

                        pLV.append(pItem);
                    };

                    pLV.listview('refresh');
                }
            });
        }
    })
})