require([
    'common'
    ], function () {

        $(function () {

            $(document).ready(function () {

                $("#btnAddDevicePrinterDevice > span").attr("style", "text-align:left;white-space:normal");

                getDevicePrinterDeviceList();
            });

            $('#btnAddDevicePrinterDevice').click(function (e) {

                $.mobile.changePage('#pageDevicePrinterDeviceInfo');
                ClearDevicePrinterDeviceFields();

            });

            $('#btnDevicePrinterDeviceDone').click(function (e) {
                e.preventDefault();

                window.open(GetPathFormat("/DevicePrinter/DevicePrinterSummary"), "_self");

            });

            $('#btnDevicePrinterDevice').click(function () {

                Browser.LibraryDeviceFowst.InputData.FOWSTID = sessionStorage.PrinterFowstID;
                Browser.LibraryDeviceFowst.Click = function (e) {

                    $('#txtDevicePrinterDeviceID').val(e.DeviceID)
                    $('#btnDevicePrinterDevice').text(e.PhoneName).button('refresh');
                    $('#txtDevicePrinterFowstID').val(e.FowstID)
                    $('#txtDevicePrinterFowst').val(e.FOWST)
                    $('#txtDevicePrinterDevicePOSRegNo').val(e.POSRegistrationNo)
                    $('#txtDevicePrinterDeviceTerminalNo').val(e.TerminalNo)

                };
                Browser.LibraryDeviceFowst.Show();

            });

            $('#btnDevicePrinterDeviceSave').click(function (e) {
                e.preventDefault();

                if ($.trim($("#txtDevicePrinterDeviceID").val()) == "") {
                    MessageBox("Please select device.",
                                "Device",
                                [
                                    { Text: 'Ok'
                                    }
                                ]
                    );
                    return;
                }

                SaveDevicePrinterDevice();

            });

            $('#btnDevicePrinterDeviceCancel').click(function (e) {
                e.preventDefault();

                $.mobile.changePage('#pageDevicePrinterDeviceList');

            });

            function getDevicePrinterDeviceList() {
                AjaxSend({
                    url: 'DevicePrinter/getDevicePrinterDeviceList',
                    success: function (vData) {
                        LoadDevicePrinterDeviceList(vData.DevicePrinterData.Rows);
                    }
                });
            }

            function LoadDevicePrinterDeviceList(vRows) {
                var pLV = $('#lstDevicePrinterDevice');
                var pItem;

                $("#divRecordCountDevicePrinterDevice").text(" Number of record(s) found: " + vRows.length);

                pLV.html('');
                for (var p = 0; p <= vRows.length - 1; p++) {
                    pItem = $('<li>')
                        .append($('<a>')
                            .attr('ID', vRows[p].ID)
                            .click(function (e) {
                                e.preventDefault();
                                $.mobile.changePage('#pageDevicePrinterDeviceInfo');
                                ShowDevicePrinterDeviceInfo($(this).attr('ID'));
                            })
                            .append($('<h3>')
                                .attr("style", "white-space:normal")
                                .text(vRows[p].PhoneName)
                            )
                            .append($('<p>')
                                .text('Phone Model : ' + vRows[p].PhoneModel)
                            )
                            .append($('<p>')
                                .text('IMEI : ' + vRows[p].IMEI)
                            )
                            .append($('<p>')
                                .text('UUID : ' + vRows[p].UUID)
                            )
                            .append($('<p>')
                                .text('Platform : ' + vRows[p].OS)
                            )
                            .append($('<p>')
                                .text('Active : ' + (vRows[p].IsActive == 1 ? "Yes" : "No"))
                            )
                        );

                    pLV.append(pItem);
                };

                pLV.listview('refresh');
            };

            function ShowDevicePrinterDeviceInfo(strID) {
                var pData = {};
                pData.DevicePrinterDeviceRecID = strID;

                ClearDevicePrinterDeviceFields();

                AjaxSend({
                    url: "DevicePrinter/getDevicePrinterDeviceInfo",
                    data: pData,
                    success: function (vData) {

                        if (vData.DevicePrinterData.Rows.length > 0) {

                            $("#txtDevicePrinterDeviceRecID").val(vData.DevicePrinterData.Rows[0].ID || '');

                            $("#txtDevicePrinterDeviceID").val(vData.DevicePrinterData.Rows[0].DeviceID || '');
                            $("#btnDevicePrinterDevice").text(vData.DevicePrinterData.Rows[0].PhoneName || '').button('refresh');

                            $("#txtDevicePrinterPhoneModel").val(vData.DevicePrinterData.Rows[0].PhoneModel || '');
                            $("#txtDevicePrinterDeviceIMEI").val(vData.DevicePrinterData.Rows[0].IMEI || '');
                            $("#txtDevicePrinterDeviceUUID").val(vData.DevicePrinterData.Rows[0].UUID || '');
                            $("#txtDevicePrinterDeviceOS").val(vData.DevicePrinterData.Rows[0].OS || '');
                            $('#chkDevicePrinterDeviceIsActive').attr("checked", (vData.DevicePrinterData.Rows[0].IsActive == 0 ? false : true)).checkboxradio("refresh");

                        }

                    }
                });

            };

            function SaveDevicePrinterDevice() {
                var pData = [];

                pData = {
                    DevicePrinterDeviceRecID: $("#txtDevicePrinterDeviceRecID").val(),
                    DeviceID: $("#txtDevicePrinterDeviceID").val(),
                    IsActive: ($("#chkDevicePrinterDeviceIsActive").is(":checked") ? 1 : 0)
                };

                AjaxSend({
                    url: "DevicePrinter/CheckUniquePrinterDevice",
                    data: pData,
                    success: function (vData) {

                        if (vData.DevicePrinterData.Rows.length > 0) {
                            MessageBox("This device already uses this printer.",
                                "Device Printer",
                                [
                                    { Text: 'Ok' }
                                ]
                            );

                        }
                        else {
                            AjaxSend({
                                url: "DevicePrinter/SaveDevicePrinterDeviceInfo",
                                data: pData,
                                success: function (vSaveData) {
                                    $.mobile.changePage('#pageDevicePrinterDeviceList');
                                    getDevicePrinterDeviceList();
                                }
                            });

                        }

                    }
                });

            };

            function ClearDevicePrinterDeviceFields() {

                $("#txtDevicePrinterDeviceRecID").val('');

                $("#txtDevicePrinterDeviceID").val('');
                $("#btnDevicePrinterDevice").text('Please Select');
                $("#btnDevicePrinterDevice").button('refresh');

                $("#txtDevicePrinterPhoneModel").val('');
                $("#txtDevicePrinterDeviceIMEI").val('');
                $("#txtDevicePrinterDeviceUUID").val('');
                $("#txtDevicePrinterDeviceOS").val('');

                $('#chkDevicePrinterDeviceIsActive').attr("checked", true).checkboxradio("refresh");
            }


        });
    });