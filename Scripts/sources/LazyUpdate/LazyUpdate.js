require([
    'common',
    '/Scripts/sources/ImportExportData/ImportExportData_Offline.js?crc='
    ], function () {

        $(function () {

        });

        window.GetUserInformation = function () {
            AjaxSend({
                url: "People/getPeopleOrganizationList",
                success: function (vDataOrg) {
                    var vRows = vDataOrg.PeopleData.Rows;
                    if (vRows.length > 0) {

                        var IsExists;

                        for (i = 0; i < vRows.length; i++) {
                            IsExists = Transfers.ReadFile(vRows[i].OrganizationID + '.flg')
                            if (IsExists != undefined) {
                                if (Sql.IsExist('Mobiz')) {
                                    // database exists then download employee info
                                    AjaxSend({
                                        url: "People/GetPeopleInfo",
                                        success: function (vDataPeopleInfo) {
                                            if (vDataPeopleInfo.Results.Rows.length > 0) {
                                                vResults = ImportExportData_offline().SelectQuery('MobizLibraryPeople', vDataPeopleInfo.Results.Rows[0].ID);
                                                ImportExportData_offline().SaveMobizLibraryPeople((vResults.Rows.length > 0 ? false : true), vDataPeopleInfo.Results.Rows[0]);

                                                AjaxSend({
                                                    url: "People/getPeopleUserAccount",
                                                    success: function (vDataPeopleUserAccount) {
                                                        vResults = ImportExportData_offline().SelectQuery('MobizLibraryPeopleUserAccount', vDataPeopleUserAccount.PeopleUserAccountData.Rows[0].ID);
                                                        ImportExportData_offline().SaveMobizLibraryPeopleUserAccount((vResults.Rows.length > 0 ? false : true), vDataPeopleUserAccount.PeopleUserAccountData.Rows[0]);


                                                        AjaxSend({
                                                            url: "People/GetPeopleSecurityRole",
                                                            data: { OrgID: vRows[i].OrganizationID },
                                                            success: function (vDataPeopleSecurityRoles) {
                                                                if (vDataPeopleSecurityRoles.Results.Rows.length > 0) {
                                                                    Sql.AttachDatabase(vRows[i].OrganizationID);
                                                                    vResults = ImportExportData_offline().SelectQuery('LibrarySecurityRoleUser', vDataPeopleSecurityRoles.Results.Rows[0].ID);
                                                                    ImportExportData_offline().SaveLibrarySecurityRoleUser((vResults.Rows.length > 0 ? false : true), vDataPeopleSecurityRoles.Results.Rows[0]);

                                                                    AjaxSend({
                                                                        url: "People/GetOrganizationEmployeeFOWST",
                                                                        data: { OrgID: vRows[i].OrganizationID },
                                                                        success: function (vDataOrganizationEmployeeFOWST) {
                                                                            if (vDataOrganizationEmployeeFOWST.Results.Rows.length > 0) {
                                                                                for (j = 0; j <= vDataOrganizationEmployeeFOWST.Results.Rows.length - 1; j++) {
                                                                                    vResults = ImportExportData_offline().SelectQuery('LibraryOrganizationEmployeeFOWST', vDataOrganizationEmployeeFOWST.Results.Rows[j].ID);
                                                                                    ImportExportData_offline().SaveLibraryOrganizationEmployeeFOWST((vResults.Rows.length > 0 ? false : true), vDataOrganizationEmployeeFOWST.Results.Rows[j]);
                                                                                }

                                                                                AjaxSend({
                                                                                    url: "People/GetPeopleOrganizationInfo",
                                                                                    success: function (vDataPeopleOrganizationInfo) {
                                                                                        vResults = ImportExportData_offline().SelectQuery('MobizLibraryPeopleOrganization', vDataPeopleOrganizationInfo.Results.Rows[0].ID);
                                                                                        ImportExportData_offline().SaveMobizLibraryPeopleOrganization((vResults.Rows.length > 0 ? false : true), vDataPeopleOrganizationInfo.Results.Rows[0]);

                                                                                        AjaxSend({
                                                                                            url: "People/GetPeopleUserAccountInfo",
                                                                                            success: function (vDataPeopleUserAccount) {
                                                                                                if (vDataPeopleUserAccount.Results.Rows.length > 0) {
                                                                                                    vResults = ImportExportData_offline().SelectQuery('MobizLibraryPeopleUserAccount', vDataPeopleUserAccount.Results.Rows[0].ID);
                                                                                                    ImportExportData_offline().SaveMobizLibraryPeopleUserAccount((vResults.Rows.length > 0 ? false : true), vDataPeopleUserAccount.Results.Rows[0]);

                                                                                                    AjaxSend({
                                                                                                        url: "Organization/getOrgFOWSTList",
                                                                                                        success: function (vDataOrganizationAddress) {
                                                                                                            if (vDataOrganizationAddress.OrganizationFOWSTData.Rows.length > 0) {

                                                                                                                for (i = 0; i < vDataOrganizationAddress.OrganizationFOWSTData.Rows.length - 1; i++) {
                                                                                                                    AjaxSend({
                                                                                                                        url: "Organization/getOrgFOWSTInfo",
                                                                                                                        success: function (vDataOrganizationFowstInfo) {

                                                                                                                            if (vDataOrganizationFowstInfo.OrganizationFOWSTData.Rows.length > 0) {
                                                                                                                                vResults = ImportExportData_offline().SelectQuery('MobizLibraryOrganizationFOWST', vDataOrganizationFowstInfo.OrganizationFOWSTData.Rows[i].ID);
                                                                                                                                ImportExportData_offline().SaveMobizLibraryOrganizationFOWST((vResults.Rows.length > 0 ? false : true), vDataOrganizationFowstInfo.OrganizationFOWSTData.Rows[i]);
                                                                                                                                return true;
                                                                                                                            }
                                                                                                                        }
                                                                                                                    });
                                                                                                                }
                                                                                                            }
                                                                                                        }
                                                                                                    });
                                                                                                }
                                                                                            }
                                                                                        });
                                                                                    }
                                                                                });
                                                                            }
                                                                        }
                                                                    });
                                                                }
                                                            }
                                                        });
                                                    }
                                                });
                                            }
                                        }
                                    });
                                }
                            }
                        }
                    }
                }
            });
        }

        window.GetDeviceAndDeviceFowstInformation = function () {
            var pData;

            var pSql = '';

            pSql = 'Select ID from LibraryDevice ' +
                    ' WHERE LibraryDevice.UUID = ' + ReadyString(Web.DeviceSerial())

            pDT = Sql.Execute(pSql);

            AjaxSend({
                url: "Device/getDeviceInfoName",
                data: { DeviceID: pDT.Rows[0].ID },
                success: function (vDataDevice) {
                    if (vDataDevice.DeviceData.Rows.length > 0) {
                        Sql.AttachDatabase(sessionStorage.OrgID);
                        vResults = ImportExportData_offline().SelectQuery('LibraryDevice', vDataDevice.DeviceData.Rows[0].ID);
                        ImportExportData_offline().SaveLibraryDevice((vResults.Rows.length > 0 ? false : true), vDataDevice.DeviceData.Rows[0]);

                        AjaxSend({
                            url: 'Device/getDeviceFOWSTList',
                            success: function (vDataFowst) {
                                if (vDataFowst.DeviceFowstData.Rows.length > 0) {
                                    for (i = 0; i <= vDataFowst.DeviceFowstData.Rows.length; i++) {
                                        vResults = ImportExportData_offline().SelectQuery('LibraryDeviceFowst', vDataFowst.DeviceFowstData.Rows[0].ID);
                                        ImportExportData_offline().SaveLibraryDeviceFowst((vResults.Rows.length > 0 ? false : true), vDataFowst.DeviceFowstData.Rows[0]);
                                    }
                                }

                                AjaxSend({
                                    url: 'Device/GetLibraryDevicePrinter',
                                    success: function (vDataDevicePrinter) {
                                        if (vDataDevicePrinter.Results.Rows.length > 0) {
                                            for (i = 0; i <= vDataDevicePrinter.Results.Rows.length; i++) {
                                                vResults = ImportExportData_offline().SelectQuery('LibraryDeviceFowst', vDataDevicePrinter.Results.Rows[0].ID);
                                                ImportExportData_offline().SaveLibraryDevicePrinter((vResults.Rows.length > 0 ? false : true), vDataDevicePrinter.Results.Rows[0]);
                                            }

                                            AjaxSend({
                                                url: 'Device/GetLibraryDeviceFowstPrinter',
                                                success: function (vDataLibraryDeviceFowstPrinter) {
                                                    if (vDataLibraryDeviceFowstPrinter.Results.Rows.length > 0) {
                                                        for (i = 0; i <= vDataLibraryDeviceFowstPrinter.Results.Rows.length; i++) {
                                                            vResults = ImportExportData_offline().SelectQuery('LibraryDeviceFowst', vDataLibraryDeviceFowstPrinter.Results.Rows[0].ID);
                                                            ImportExportData_offline().SaveLibraryDeviceFowstPrinter((vResults.Rows.length > 0 ? false : true), vDataLibraryDeviceFowstPrinter.Results.Rows[0]);
                                                        }

                                                        AjaxSend({
                                                            url: 'Device/GetLibraryDevicePrinterFowst',
                                                            success: function (vDataLibraryDevicePrinterFowst) {
                                                                if (vDataLibraryDevicePrinterFowst.Results.Rows.length > 0) {
                                                                    for (i = 0; i <= vDataLibraryDevicePrinterFowst.Results.Rows.length; i++) {
                                                                        vResults = ImportExportData_offline().SelectQuery('LibraryDeviceFowst', vDataLibraryDevicePrinterFowst.Results.Rows[0].ID);
                                                                        ImportExportData_offline().SaveLibraryDevicePrinterFowst((vResults.Rows.length > 0 ? false : true), vDataLibraryDevicePrinterFowst.Results.Rows[0]);
                                                                    }

                                                                    var pParam = {};

                                                                    if (IsMobile)
                                                                        pParam.IsMobile = 1;
                                                                    pParam = $.param(pParam);
                                                                    window.location = "/People/PeopleMenu?" + pParam;
                                                                }
                                                            }
                                                        });

                                                    }
                                                }
                                            });

                                        }
                                    }
                                });

                            }
                        });
                    }
                }
            });
        }
    });