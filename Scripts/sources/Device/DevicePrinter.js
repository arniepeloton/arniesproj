require([
    'common'
    ], function () {

        $(function () {

            $(document).ready(function () {

                $("#btnAddDevicePrinter > span").attr("style", "text-align:left;white-space:normal");

                getDevicePrinterList();
            });

            $('#btnAddDevicePrinter').click(function (e) {

                $.mobile.changePage('#pageDevicePrinterInfo');
                ClearDevicePrinterFields();

            });

            $('#btnDevicePrinter').click(function () {

                Browser.LibraryDevicePrinter.Click = function (e) {

                    $('#txtDevicePrinterID').val(e.ID)
                    $('#btnDevicePrinter').text(e.PrinterName).button('refresh');
                    $('#txtDevicePrinterType').val(e.PrinterType)
                    $('#txtDevicePrinterFOWST').val(e.Fowst)
                    $('#txtDevicePrinterIP').val(e.IPAddress)
                    $('#chkPrinterIsActive').attr("checked", (e.IsActive == 0 ? false : true)).checkboxradio("refresh");

                };
                Browser.LibraryDevicePrinter.Show();

            });

            $('#btnDevicePrinterSave').click(function (e) {
                e.preventDefault();

                if ($.trim($("#txtDevicePrinterID").val()) == "") {
                    MessageBox("Please select printer.",
                               "Printer",
                                [
                                    { Text: 'Ok'
                                    }
                                ]
                    );
                    return;
                }

                SaveDevicePrinter();

            });

//            $('#btnDevicePrinterRemove').click(function (e) {
//                e.preventDefault();

//                MessageBox("Remove this record?",
//                    'Remove',
//                    [
//                        { Text: 'Yes',
//                            Click: function () {
//                                RemoveDevicePrinter();
//                            }
//                        }
//                    ,
//                        { Text: 'No'
//                        }
//                    ]
//                );

//            });

            $('#btnDevicePrinterCancel').click(function (e) {
                e.preventDefault();

                $.mobile.changePage('#pageDevicePrinter');

            });

            $('#btnDevicePrinterDone').click(function (e) {
                e.preventDefault();

                window.open("DeviceInfoSummary", "_self");

            });

            function getDevicePrinterList() {
                AjaxSend({
                    url: 'Device/getDevicePrinterList',
                    success: function (vData) {
                        LoadDevicePrinterList(vData.DevicePrinterData.Rows);
                    }
                });
            }

            function LoadDevicePrinterList(vRows) {
                var pLV = $('#lstDevicePrinter');
                var pItem;

                $("#divRecordCountDevicePrinter").text(" Number of record(s) found: " + vRows.length);

                pLV.html('');
                for (var p = 0; p <= vRows.length - 1; p++) {
                    pItem = $('<li>')
                        .append($('<a>')
                            .attr('ID', vRows[p].ID)
                            .click(function (e) {
                                e.preventDefault();
                                $.mobile.changePage('#pageDevicePrinterInfo');
                                ShowDevicePrinterInfo($(this).attr('ID'));
                            })
                            .append($('<h3>')
                                .attr("style", "white-space:normal")
                                .text(vRows[p].PrinterName)
                            )
                            .append($('<p>')
                                .text('Printer Type : ' + vRows[p].PrinterType)
                            )
                            .append($('<p>')
                                .text('Printer FOWST Assignment : ' + vRows[p].FOWST)
                            )
                            .append($('<p>')
                                .text('Fixed IP Address : ' + vRows[p].IPAddress)
                            )
                            .append($('<p>')
                                .text('Active : ' + (vRows[p].IsActive == 1 ? "Yes" : "No"))
                            )
                        );

                    pLV.append(pItem);
                };

                pLV.listview('refresh');
            };

            function ShowDevicePrinterInfo(strID) {
                var pData = {};
                pData.DevicePrinterRecID = strID;

                ClearDevicePrinterFields();

                AjaxSend({
                    url: "Device/getDevicePrinterInfo",
                    data: pData,
                    success: function (vData) {

                        if (vData.DevicePrinterData.Rows.length > 0) {

                            $("#txtDevicePrinterDeviceRecID").val(vData.DevicePrinterData.Rows[0].ID || '');

                            $("#txtDevicePrinterID").val(vData.DevicePrinterData.Rows[0].PrinterID || '');
                            $("#btnDevicePrinter").text(vData.DevicePrinterData.Rows[0].PrinterName || '').button('refresh');

                            $("#txtDevicePrinterType").val(vData.DevicePrinterData.Rows[0].PrinterType || '');
                            $("#txtDevicePrinterFOWST").val(vData.DevicePrinterData.Rows[0].FOWST || '');
                            $("#txtDevicePrinterIP").val(vData.DevicePrinterData.Rows[0].IPAddress || '');

                            $('#chkPrinterIsActive').attr("checked", (vData.DevicePrinterData.Rows[0].IsActive == 0 ? false : true)).checkboxradio("refresh");
                        }

                    }
                });

            };

            function SaveDevicePrinter() {
                var pData = [];
                pData = {
                    DevicePrinterRecID: $("#txtDevicePrinterDeviceRecID").val(),
                    PrinterID: $("#txtDevicePrinterID").val(),
                    IsActive: ($("#chkPrinterIsActive").is(":checked") ? 1 : 0)
                };

                AjaxSend({
                    url: "Device/CheckUniqueDevicePrinter",
                    data: pData,
                    success: function (vData) {

                        if (vData.DevicePrinterData.Rows.length > 0) {
                            MessageBox("Printer already exist.",
                            "Printer Exist",
                            [
                                { Text: 'Ok' }
                            ]
                        );

                        }
                        else {
                            AjaxSend({
                                url: "Device/SaveDevicePrinterInfo",
                                data: pData,
                                success: function (vSaveData) {

                                    $.mobile.changePage('#pageDevicePrinter');
                                    getDevicePrinterList();

                                }
                            });

                        }

                    }
                });

            };

            function ClearDevicePrinterFields() {

                $("#txtDevicePrinterDeviceRecID").val('');

                $("#txtDevicePrinterID").val('');
                $("#btnDevicePrinter").text('Please Select');
                $("#btnDevicePrinter").button('refresh');

                $("#txtDevicePrinterType").val('');
                $("#txtDevicePrinterIP").val('');

                $('#chkPrinterIsActive').attr("checked", true).checkboxradio("refresh");
                
            }


        });
    });