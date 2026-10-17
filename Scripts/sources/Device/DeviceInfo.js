require([
    'common',
    GetPathFormat('/Scripts3/Browser/LibraryFowst.js?crc=')
    ], function () {

        $(function () {
            var IsDone;

            $(document).ready(function () {
                $("#selDeviceDeactivate").val('1')//.slider("refresh");
                IsDone = false;

                AjaxSend({
                    url: 'Device/IsNewEntry',
                    success: function (vData) {

                        ShowDeviceInfo(vData.DeviceID);
                    }
                });
            });

            window.DeviceInfo = {
                ShowDeviceInfo: function () {
                    AjaxSend({
                        url: 'Device/IsNewEntry',
                        success: function (vData) {
                            ShowDeviceInfo(vData.DeviceID);
                        }
                    });
                }
            }

            $('body').on('click', '#btnDeviceInfoDone', function () {
                if ($.trim($("#txtPhoneName").val()) == "" && $.trim($("#txtPhoneName").val()).length <= 1) {
                    NewMessageBox("Please enter phone name.",
                            "Phone Name",
                            [
                                { Text: 'Ok',
                                    Click: function () {
                                        //history.back();
                                    }
                                }
                            ]
                    );
                    return;
                }

                if ($.trim($("#txtPhoneModel").val()) == "") {
                    NewMessageBox("Please enter phone model.",
                            "Phone Model",
                            [
                                { Text: 'Ok',
                                    Click: function () {
                                        //history.back();
                                    }
                                }
                            ]
                        );
                    return;
                }

                if ($.trim($("#txtIMEI").val()) == "") {
                    NewMessageBox("Please enter IMEI.",
                            "ID",
                            [
                                { Text: 'Ok',
                                    Click: function () {
                                        //history.back();
                                    }
                                }
                            ]
                        );
                    return;
                }

                if ($.trim($("#selMobizAppVersion").val()) == "") {
                    NewMessageBox("Please enter Mobiz app version.",
                            "ID",
                            [
                                { Text: 'Ok',
                                    Click: function () {
                                        //history.back();
                                    }
                                }
                            ]
                        );
                    return;
                }

                ValidateUniqueDeviceInfo();
            })
            .on('click', '#btnRetrieveDeviceInfoFromQR', function () {
                mobile.Scan({
                    Success: function (vData) {
                        ParseData(JSON.parse(vData.Value));
                    }
                });
            })
            .on('click', '#btnScanIMEI', function () {
                mobile.Scan({
                    Success: function (vData) {
                        $('#txtIMEI').val(vData.Value);
                    }
                });
            })
            .on('click', '#btnScanUUID', function () {
                mobile.Scan({
                    Success: function (vData) {
                        $('#txtUUID').val(vData.Value);
                    }
                });
            })
            .on('click', '#btnDeviceInfoCancel', function () {
                if (sessionStorage.AddViewLibBCName == "View Device") {
                    sessionStorage["AddViewLibBCName"] = "View Device";
                    window.location.hash = '';
                    window.location.hash = 'UniversalPage';
                    LibraryTable.ViewDevicePage();
                }
                else {
                    $('#tblBody').html('');
                    window.location.hash = '';
                    window.location.hash = 'LibraryPage';
                    LibraryTable.Devices();
                    ClearDeviceFields();
                }
            })

            $('#btnRetrieveDeviceInfo').click(function () {
                NewMessageBox("The information on this device will be used to fill up data fields required for enrolling a device. " + "\<br> " + "Would you like to proceed?",
                            "Device Information",
                            [
                                { Text: 'Yes',
                                    Click: function () {
                                        $('#txtPhoneName').val(Device.Name()); // device.name
                                        $('#txtPhoneModel').val(Device.Name()); // device.platform
                                        $('#txtUUID').val(Device.Serial()); // device.uuid
                                        $('#txtOS').val(Device.Platform()); // device.platform
                                        $('#txtIMEI').val(Device.Imei()); // window.pPrint.GetIMEI()
                                        $('.ui-dialog').dialog('close');
                                    }
                                },
                                { Text: 'No' }
                            ]
                          , {
                              EncodeHtml: false
                          });
            });

            //            $('#btnRetrieveDeviceInfoFromQR').click(function () {
            //                mobile.Scan({
            //                    Success: function (vData) {
            //                        ParseData(JSON.parse(vData.Value));
            //                    }
            //                });
            //            });

            //            $('#btnScanIMEI').click(function () {
            //                mobile.Scan({
            //                    Success: function (vData) {
            //                        $('#txtIMEI').val(vData.Value);
            //                    }
            //                });
            //            });

            //            $('#btnScanUUID').click(function () {
            //                mobile.Scan({
            //                    Success: function (vData) {
            //                        $('#txtUUID').val(vData.Value);
            //                    }
            //                });
            //            });

            //            $('#btnDeviceInfoDone').on('click', function (e) {
            //                e.preventDefault();


            //            });

            function ParseData(vData) {
                $('#txtPhoneName').val(vData.Name); // device.name
                $('#txtPhoneModel').val(vData.Name); // device.platform
                $('#txtUUID').val(vData.Serial); // device.uuid
                $('#txtOS').val(vData.Platform); // device.platform
                $('#txtIMEI').val(vData.Imei || '-'); // window.pPrint.GetIMEI()
            }

            function ShowDeviceInfo(strID) {
                var pData = {};
                pData.DeviceID = strID; //strID;

                ClearDeviceFields();

                AjaxSend({
                    url: "Device/getDeviceInfoName",
                    data: pData,
                    success: function (vData) {

                        if (vData.DeviceData.Rows.length > 0) {

                            $("#txtID").val(vData.DeviceData.Rows[0].ID || '');

                            $("#txtPhoneName").val(vData.DeviceData.Rows[0].PhoneName || '');
                            $("#txtPhoneModel").val(vData.DeviceData.Rows[0].PhoneModel || '');
                            $("#txtIMEI").val(vData.DeviceData.Rows[0].IMEI || '');
                            $("#txtBusinessName").val(vData.DeviceData.Rows[0].BusinessName || '');
                            $("#txtBusinessRegisteredName").val(vData.DeviceData.Rows[0].BusinessRegisteredName || '');
                            $("#txtUUID").val(vData.DeviceData.Rows[0].UUID || '');
                            $('#txtUuidDisplay').val(vData.DeviceData.Rows[0].UuidDisplay || ''); 
                            $('#txtTinDisplay').val(vData.DeviceData.Rows[0].TinDisplay || ''); 
                            $('#selAccreditation').val(vData.DeviceData.Rows[0].Accreditation || '');//.selectmenu('refresh'); 
                            $("#txtOS").val(vData.DeviceData.Rows[0].OS || '');
                            $("#txtDeviceNo").val(vData.DeviceData.Rows[0].DeviceNo || '');
                            $("#selMobizAppVersion").val(vData.DeviceData.Rows[0].MobizAppVersion || 'Version 1');
                            $("#selDeviceDeactivate").val(vData.DeviceData.Rows[0].IsActive || '0')//.slider("refresh");

                        }
                    }
                });

            };

            function ClearDeviceFields() {

                $("#txtID").val('');

                $("#txtPhoneName").val('');
                $("#txtPhoneModel").val('');
                $("#txtIMEI").val('');
                $("#txtUUID").val('');
                $('#txtUuidDisplay').val('');
                $('#txtTinDisplay').val('');
                $("#selAccreditation").val('');//.selectmenu("refresh");
                $("#txtOS").val('');
                $("#txtDeviceNo").val('');
                $("#selMobizAppVersion").val('Version 1');

            }

            function ValidateUniqueDeviceInfo() {

                var pData = {};
                pData.DeviceID = $.trim($("#txtID").val());
                pData.PhoneName = $.trim($("#txtPhoneName").val());
                pData.IMEI = $.trim($("#txtIMEI").val());
                pData.UUID = $.trim($("#txtUUID").val());

                $('#btnDeviceInfoDone').addClass('ui-disabled')
                WebToast('Saving ...')

                AjaxSend({
                    url: "Device/CheckDeviceUniquePhoneName",
                    data: pData,
                    success: function (vData) {

                        if (vData.DeviceData.Rows.length > 0) {
                            $('#btnDeviceInfoDone').removeClass('ui-disabled')

                            NewMessageBox("Device with the same phone name already exist.", "Phone Name");
                        }
                        else {

                            AjaxSend({
                                url: "Device/CheckDeviceUnique",
                                data: pData,
                                success: function (vData) {

                                    if (vData.DeviceData.Rows.length > 0) {
                                        $('#btnDeviceInfoDone').removeClass('ui-disabled')
                                        NewMessageBox("Device with the same UUID already exist.", "UUID");
                                    }
                                    else {
                                        SaveDeviceInfo();
                                    }

                                }
                            });

                        }

                    }
                });

            };

            function SaveDeviceInfo() {
                var pData = [];
                var ctr = 0;

                pData = {
                    DeviceID: $.trim($("#txtID").val()),
                    PhoneName: $.trim($("#txtPhoneName").val()),
                    PhoneModel: $.trim($("#txtPhoneModel").val()),
                    IMEI: $.trim($("#txtIMEI").val()),
                    UUID: $.trim($("#txtUUID").val()),
                    UuidDisplay: $.trim($("#txtUuidDisplay").val()),
                    TinDisplay: $.trim($("#txtTinDisplay").val()),
                    Accreditation: $.trim($("#selAccreditation").val()),
                    MobizAppVersion: $.trim($("#selMobizAppVersion").val()),
                    OS: $.trim($("#txtOS").val()),
                    IsActive: $("#selDeviceDeactivate").val(),
                    BusinessName: $.trim($('#txtBusinessName').val()),
                    BusinessRegName: $.trim($('#txtBusinessRegisteredName').val())
                };

                AjaxSend({
                    url: "Device/CheckDeviceUniquePhoneName",
                    data: pData,
                    success: function (vData) {

                        if (vData.DeviceData.Rows.length > 0) {
                            $('#btnDeviceInfoDone').removeClass('ui-disabled')
                            NewMessageBox("Device with the same phone name already exist.", "Phone Name");
                        }
                        else {

                            AjaxSend({
                                url: "Device/CheckDeviceUnique",
                                data: pData,
                                success: function (vData) {

                                    if (vData.DeviceData.Rows.length > 0) {
                                        $('#btnDeviceInfoDone').removeClass('ui-disabled')
                                        NewMessageBox("Device with the same UUID already exist.", "UUID");
                                    }
                                    else {
                                        AjaxSend({
                                            url: "Device/SaveDeviceInfoName",
                                            data: pData,
                                            success: function (vSaveData) {
                                                $('#btnDeviceInfoDone').removeClass('ui-disabled')
                                                //window.location = GetPathFormat("/Device/DeviceInfoSummary?OrgID=" + sessionStorage.OrgID);
                                                if (sessionStorage.AddViewLibBCName == "View Device") {
                                                    sessionStorage["AddViewLibBCName"] = "View Device";
                                                    window.location.hash = '';
                                                    window.location.hash = 'UniversalPage';
                                                    LibraryTable.ViewDevicePage();
                                                }
                                                else {
                                                    $('#tblBody').html('');
                                                    window.location.hash = '';
                                                    window.location.hash = 'LibraryPage';
                                                    LibraryTable.Devices();
                                                    ClearDeviceFields();
                                                }
                                            }
                                        });
                                    }

                                }
                            });

                        }

                    }
                });

            };

        });
    });