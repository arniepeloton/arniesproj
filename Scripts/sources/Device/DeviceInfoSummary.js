require([
    'common'
    ], function () {

        $(function () {

            var IsEdit = false;
            var isDevicePageLoaded = "";
            var isDeviceFowstPageLoaded = "";
            var mDeviceID;

            $(document).ready(function () {

                AjaxSend({
                    url: 'Device/IsNewEntry',
                    success: function (vData) {

                        ShowDeviceInfo(vData.DeviceID)

                    }
                });

                if (sessionStorage.OrgID != '565c260c-3844-4069-ac41-d26f0598e562') {
                    $('#butResetSalesHistory').removeClass('d-none');
                }

            });

            window.AuthorizedDevices = {
                ShowDeviceInfo: function () {
                    IsEdit = false;
                    ShowDeviceInfo(sessionStorage.ViewID)
                }
            }

            $('#btnBack').click(function () {
                history.back();
            });

            $('body').on('click', '#btnEditDevice', function (e) {
                e.preventDefault();
                IsEdit = true;
                NewPromptUserAccess('Allow editing of existing device information', {
                    Message: 'You\'re not allowed to edit device information.',
                    OnClickOKSuccess: function () {
                        //window.open(GetPathFormat("DeviceInfo"), "_self");
                        if (isDevicePageLoaded == "") {
                            AjaxSend({
                                ExactUrl: '/Device/DeviceInfo',
                                type: 'GET',
                                dataType: 'text',
                                success: function (vData) {
                                    isDevicePageLoaded = vData;
                                    $('#UniversalDiv').html('');
                                    $('#UniversalDiv').append(vData);//.trigger('create');
                                    App.Deferred.fulfill('FQ26E5TP76');
                                    //DeviceInfo.ShowDeviceInfo();
                                }
                            });
                        }
                        else {
                            $('#UniversalDiv').html('');
                            $('#UniversalDiv').append(isDevicePageLoaded);//.trigger('create');
                            DeviceInfo.ShowDeviceInfo();
                        }
                    }
                })                
            })
            .on('click', '#btnEditDeviceFowst', function (e) {
                e.preventDefault();
                IsEdit = true;
                NewPromptUserAccess('Allow editing of existing device information', {
                    Message: 'You\'re not allowed to edit device information.',
                    OnClickOKSuccess: function () {
                        //window.open(GetPathFormat("DeviceInfo"), "_self");
                        if (isDeviceFowstPageLoaded == "") {
                            AjaxSend({
                                ExactUrl: '/Device/DeviceFowst',
                                type: 'GET',
                                dataType: 'text',
                                success: function (vData) {
                                    isDeviceFowstPageLoaded = vData;
                                    $('#UniversalDiv').html('');
                                    $('#UniversalDiv').append(vData);//.trigger('create');
                                    App.Deferred.fulfill('FQ26E5TP76');
                                }
                            });
                        }
                        else {
                            $('#UniversalDiv').html('');
                            $('#UniversalDiv').append(isDeviceFowstPageLoaded);//.trigger('create');
                            DeviceFowst.DeviceFowstList();
                        }
                    }
                })                
            })
            .on('click', '#btnDeviceSummaryBack', function (e) {
                e.preventDefault();
                if (GetUrlParameters().mode == 'wizard') {
                    NewMessageBox('Please select a FOWST and assign a printer.', 'Device Printer Setup', [
                        {
                            Text: 'Ok', Click: function () {
                                if (GetUrlParameters().mode == 'wizard') {
                                    window.open(GetPathFormat("/Device/DeviceFowst?mode=wizard"), "_self");
                                } else {
                                    window.open(GetPathFormat("DeviceFowst"), "_self");
                                }
                            }
                        }
                    ])
                } else {
                    //window.location = GetPathFormat("/Device?OrgID=" + sessionStorage.OrgID);
                    $('#tblBody').html('');
                    window.location.hash = 'LibraryPage';
                    LibraryTable.Devices();
                }
            })
            .on('click', '#butResetSalesHistory', function (e) {
                e.preventDefault();
                $('#popupResetConfirmationPassword').modal('show');
            })
            .on('click','#btnProceedDeviceReset', function (e) {
                e.preventDefault();

                AjaxSend({
                    url: 'Device/ConfirmResetDevice',
                    data: { ResetPassword: $.trim($('#txtResetPassword').val()) },
                    success: function (e) {
                        if (e.Result == 'ok') {
                            var pData = {
                                DeviceID: mDeviceID
                            };
                            AjaxSend({
                                url: "Device/ResetTransactions",
                                data: pData,
                                success: function (vData) {
                                    $('#popupResetConfirmationPassword').modal('hide');
                                    if (vData.Error)
                                        NewMessageBox(vData.Error);
                                    else if (vData.Result == 'ok') {
                                        sessionStorage.ViewID = vData.NewDeviceID;
                                        ShowDeviceInfo(sessionStorage.ViewID);
                                        NewMessageBox('Changes have been saved. <br><br> Don\'t forget to encode the PTU details for this device, or you may ask for assistance from the Mobiz Team. <br><br> Thank you!');
                                    } else
                                        NewMessageBox(JSON.stringify(vData));
                                }
                            });
                        } else {
                            WebToast('Invalid password')
                        }
                    }
                })
            });

            $('#btnEditDevicePrinter').click(function (e) {
                e.preventDefault();

                window.open(GetPathFormat("DevicePrinter"), "_self");
            });

            function ShowDeviceInfo(strID) {
                var pData = {};
                pData.DeviceID = strID;
                mDeviceID = strID;
                ClearDeviceFields();

                AjaxSend({
                    url: "Device/getDeviceInfoName",
                    data: pData,
                    success: function (vData) {

                        if (vData.DeviceData.Rows.length > 0) {

                            if (IsEdit == false) {
                                $("#txtSumDeviceID").val(vData.DeviceData.Rows[0].ID || '');

                                $("#txtSumDevicePhoneName").val(vData.DeviceData.Rows[0].PhoneName || '');
                                $("#txtSumDevicePhoneModel").val(vData.DeviceData.Rows[0].PhoneModel || '');
                                $("#txtSumDeviceIMEI").val(vData.DeviceData.Rows[0].IMEI || '');
                                $("#txtSumDeviceUUID").val(vData.DeviceData.Rows[0].UUID || '');
                                $("#txtSumDeviceUuidDisplay").val(vData.DeviceData.Rows[0].UuidDisplay || '');
                                $("#txtSumDeviceTinDisplay").val(vData.DeviceData.Rows[0].TinDisplay || '');
                                $("#txtSumBusinessName").val(vData.DeviceData.Rows[0].BusinessName || '');
                                $("#txtSumBusinessRegisteredName").val(vData.DeviceData.Rows[0].BusinessRegisteredName || '');
                                $("#txtSumAccreditation").val(vData.DeviceData.Rows[0].AccreditationText || '');
                                $("#txtSumMobizAppVersion").val(vData.DeviceData.Rows[0].MobizAppVersion || '1');
                                $("#txtSumDeviceOS").val(vData.DeviceData.Rows[0].OS || '');
                                $("#txtSumDeviceNo").val(vData.DeviceData.Rows[0].DeviceNo || '');

                                getSumDevicePlaceList();
                                //                            getSumDevicePrinterList();
                            }
                            else {
                                $("#txtID").val(vData.DeviceData.Rows[0].ID || '');

                                $("#txtPhoneName").val(vData.DeviceData.Rows[0].PhoneName || '');
                                $("#txtPhoneModel").val(vData.DeviceData.Rows[0].PhoneModel || '');
                                $("#txtIMEI").val(vData.DeviceData.Rows[0].IMEI || '');
                                $("#txtUUID").val(vData.DeviceData.Rows[0].UUID || '');
                                $("#txtUuidDisplay").val(vData.DeviceData.Rows[0].UuidDisplay || '');
                                $("#txtTinDisplay").val(vData.DeviceData.Rows[0].TinDisplay || '');
                                $("#txtOS").val(vData.DeviceData.Rows[0].OS || '')
                                $("#txtDeviceNo").val(vData.DeviceData.Rows[0].DeviceNo || '')
                                $("#selDeviceDeactivate").val(vData.DeviceData.Rows[0].IsActive || '0').slider("refresh");
                            }

                        }
                    }
                });

            };

            function ClearDeviceFields() {

                $("#txtSumDeviceID").val('');

                $("#txtSumDevicePhoneName").val('');
                $("#txtSumDevicePhoneModel").val('');
                $("#txtSumDeviceIMEI").val('');
                $("#txtSumDeviceUUID").val('');
                $("#txtSumDeviceUuidDisplay").val('');
                $("#txtSumAccreditation").val('');
                $("#txtSumMobizAppVersion").val('');
                $("#txtSumDeviceOS").val('');
                $("#txtSumDeviceNo").val('');
                $("#txtSumBusinessName").val('');
                $("#txtSumBusinessRegisteredName").val('');

            }

            function getSumDevicePlaceList() {
                AjaxSend({
                    url: 'Device/getDeviceFOWSTList',
                    success: function (vData) {
                        LoadSumDeviceFowstList(vData.DeviceFowstData.Rows);
                    }
                });
            }

            var isFowstListEmpty = false;

            function LoadSumDeviceFowstList(vRows) {
                var pLV = $('#lstSumDeviceFowst');
                var pItem;

                if (vRows.length < 1)
                    isFowstListEmpty = true;

                pLV.html('');
                for (var p = 0; p <= vRows.length - 1; p++) {
                    pItem = $('<a href="#" class="list-group-item list-group-item-action">')
                        .append($('<div>')
                            .attr("style", "white-space:normal")
                            .text((p + 1) + '. ' + vRows[p].FOWSTName)
                        );

                    pLV.append(pItem);
                };

                //pLV.listview('refresh');
            };

            function getSumDevicePrinterList() {
                AjaxSend({
                    url: 'Device/getDevicePrinterList',
                    success: function (vData) {
                        LoadSumDevicePrinterList(vData.DevicePrinterData.Rows);
                    }
                });
            }

            function LoadSumDevicePrinterList(vRows) {
                var pLV = $('#lstSumDevicePrinter');
                var pItem;

                pLV.html('');
                for (var p = 0; p <= vRows.length - 1; p++) {
                    pItem = $('<li class="list-group-item">')
                        .append($('<div>')
                            .attr("style", "white-space:normal")
                            .text(vRows[p].PrinterName)
                        )
                        .append($('<div>')
                            .text('Printer Type : ' + vRows[p].PrinterType)
                        )
                        .append($('<div>')
                            .text('Printer FOWST Assignment : ' + vRows[p].FOWST)
                        )
                        .append($('<div>')
                            .text('Fixed IP Address : ' + vRows[p].IPAddress)
                        )
                        .append($('<div>')
                            .text('Active : ' + (vRows[p].IsActive == 1 ? "Yes" : "No"))
                        );

                    pLV.append(pItem);
                };

                //pLV.listview('refresh');
            };


        });
    });


