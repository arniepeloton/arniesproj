require([
    'common'
    ], function () {

        $(function () {

            ////        $(document).ready(function () {

            ////        });

            window.RequestImportData = function (vOrgID) {
                var intCntr = 0;
                var IsRecordFound = false;
                var IsSuccess = false;
                var pData = {};
                pData.OrgID = vOrgID;

                AjaxSend({
                    url: 'MobizExport/RequestDataFromServer',
                    data: { OrgID: vOrgID },
                    success: function (vData) {
                        Transfers.Download({
                            Key: vData.Results,
                            Destination: 'RequestUpdate' + pData.OrgID + '.zip',
                            Success: function (vData) {

                                Sql.OpenOrCreateDatabase('MobizDUC');
                                Sql.AttachDatabase('Mobiz');

                                //Private  System Defaults
                                var vPvtSysDefData = JSON.parse(Transfers.ReadFile('RequestUpdate' + pData.OrgID + '.zip', 'SystemDefaults\/SystemDefaults.json'));
                                var vPvtSysDefDataNeeded = [];
                                if (vPvtSysDefData.d.Results.Rows.length > 0) {
                                    intCntr = 0;
                                    for (var iRow = 0; iRow <= vPvtSysDefData.d.Results.Rows.length - 1; iRow++) {
                                        vPvtSysDefDataNeeded[intCntr] = {
                                            ID: vPvtSysDefData.d.Results.Rows[iRow].ID
                                        };
                                        intCntr = intCntr + 1;
                                    }
                                }
                                //END System Defaults

                                //Private Mobiz System Defaults
                                var vPubMobizSysDefData = JSON.parse(Transfers.ReadFile('RequestUpdate' + pData.OrgID + '.zip', 'MobizSystemDefaults\/MobizSystemDefaults.json'));
                                var vPubMobizSysDefDataNeeded = [];
                                if (vPubMobizSysDefData.d.Results.Rows.length > 0) {
                                    intCntr = 0;
                                    for (var iRow = 0; iRow <= vPubMobizSysDefData.d.Results.Rows.length - 1; iRow++) {
                                        vPubMobizSysDefDataNeeded[intCntr] = {
                                            ID: vPubMobizSysDefData.d.Results.Rows[iRow].ID
                                        };
                                        intCntr = intCntr + 1;
                                    }
                                }
                                //END Mobiz System Defaults

                                //Public MobizLibraryCountriesLanguageLabel
                                var vPubLibraryCountriesLanguageLabel = JSON.parse(Transfers.ReadFile('RequestUpdate' + pData.OrgID + '.zip', 'MobizLibraryCountriesLanguageLabel\/MobizLibraryCountriesLanguageLabel.json'));
                                var vPubLibraryCountriesLanguageLabelOffline = ImportExportData_offline().MobizLibraryCountriesLanguageLabel('');
                                var vPubLibraryCountriesLanguageLabelNeeded = [];
                                if (vPubLibraryCountriesLanguageLabel.d.Results.Rows.length > 0) {
                                    intCntr = 0;
                                    for (var iRow = 0; iRow <= vPubLibraryCountriesLanguageLabel.d.Results.Rows.length - 1; iRow++) {
                                        IsRecordFound = false;
                                        for (var iLRow = 0; iLRow <= vPubLibraryCountriesLanguageLabelOffline.Rows.length - 1; iLRow++) {
                                            if (vPubLibraryCountriesLanguageLabel.d.Results.Rows[iRow].ID == vPubLibraryCountriesLanguageLabelOffline.Rows[iLRow].ID &&
                                                vPubLibraryCountriesLanguageLabel.d.Results.Rows[iRow].LastUpdated == vPubLibraryCountriesLanguageLabelOffline.Rows[iLRow].LastUpdated) {

                                                vPubLibraryCountriesLanguageLabelNeeded[intCntr] = {
                                                    ID: vPubLibraryCountriesLanguageLabel.d.Results.Rows[iRow].ID
                                                };
                                                intCntr = intCntr + 1;
                                                IsRecordFound = true;
                                            }
                                        }

                                        //If New Record
                                        if (IsRecordFound == false) {
                                            vPubLibraryCountriesLanguageLabelNeeded[intCntr] = {
                                                ID: vPubLibraryCountriesLanguageLabel.d.Results.Rows[iRow].ID
                                            };
                                            intCntr = intCntr + 1;
                                        }

                                    }
                                }
                                //END MobizLibraryCountriesLanguageLabel

                                //Public LibraryDiscountSettings
                                var vPvtLibraryDiscountSettings = JSON.parse(Transfers.ReadFile('RequestUpdate' + pData.OrgID + '.zip', 'LibraryDiscountSettings\/LibraryDiscountSettings.json'));
                                var vPvtLibraryDiscountSettingsOffline = ImportExportData_offline().LibraryDiscountSettings('');
                                var vPvtLibraryDiscountSettingsNeeded = [];
                                if (vPvtLibraryDiscountSettings.d.Results.Rows.length > 0) {
                                    intCntr = 0;
                                    for (var iRow = 0; iRow <= vPvtLibraryDiscountSettings.d.Results.Rows.length - 1; iRow++) {
                                        IsRecordFound = false;
                                        for (var iLRow = 0; iLRow <= vPvtLibraryDiscountSettingsOffline.Rows.length - 1; iLRow++) {
                                            if (vPvtLibraryDiscountSettings.d.Results.Rows[iRow].ID == vPvtLibraryDiscountSettingsOffline.Rows[iLRow].ID &&
                                                vPvtLibraryDiscountSettings.d.Results.Rows[iRow].LastUpdated == vPvtLibraryDiscountSettingsOffline.Rows[iLRow].LastUpdated) {

                                                vPvtLibraryDiscountSettingsNeeded[intCntr] = {
                                                    ID: vPvtLibraryDiscountSettings.d.Results.Rows[iRow].ID
                                                };
                                                intCntr = intCntr + 1;
                                                IsRecordFound = true;
                                            }
                                        }

                                        //If New Record
                                        if (IsRecordFound == false) {
                                            vPvtLibraryDiscountSettingsNeeded[intCntr] = {
                                                ID: vPvtLibraryDiscountSettings.d.Results.Rows[iRow].ID
                                            };
                                            intCntr = intCntr + 1;
                                        }
                                    }
                                }
                                //END LibraryDiscountSettings

                                //Public LibrarySecurityRole
                                var vPvtLibrarySecurityRole = JSON.parse(Transfers.ReadFile('RequestUpdate' + pData.OrgID + '.zip', 'LibrarySecurityRole\/LibrarySecurityRole.json'));
                                var vPvtLibrarySecurityRoleOffline = ImportExportData_offline().LibrarySecurityRole('');
                                var vPvtLibrarySecurityRoleNeeded = [];
                                if (vPvtLibrarySecurityRole.d.Results.Rows.length > 0) {
                                    intCntr = 0;
                                    for (var iRow = 0; iRow <= vPvtLibrarySecurityRole.d.Results.Rows.length - 1; iRow++) {
                                        IsRecordFound = false;
                                        for (var iLRow = 0; iLRow <= vPvtLibrarySecurityRoleOffline.Rows.length - 1; iLRow++) {
                                            if (vPvtLibrarySecurityRole.d.Results.Rows[iRow].ID == vPvtLibrarySecurityRoleOffline.Rows[iLRow].ID &&
                                                vPvtLibrarySecurityRole.d.Results.Rows[iRow].LastUpdated == vPvtLibrarySecurityRoleOffline.Rows[iLRow].LastUpdated) {

                                                vPvtLibrarySecurityRoleNeeded[intCntr] = {
                                                    ID: vPvtLibrarySecurityRole.d.Results.Rows[iRow].ID
                                                };
                                                intCntr = intCntr + 1;
                                                IsRecordFound = true;
                                            }
                                        }

                                        //If New Record
                                        if (IsRecordFound == false) {
                                            vPvtLibrarySecurityRoleNeeded[intCntr] = {
                                                ID: vPvtLibrarySecurityRole.d.Results.Rows[iRow].ID
                                            };
                                            intCntr = intCntr + 1;
                                        }
                                    }
                                }
                                //END LibrarySecurityRole

                                //Public LibraryReceiptSettings
                                var vPvtLibraryReceiptSettings = JSON.parse(Transfers.ReadFile('RequestUpdate' + pData.OrgID + '.zip', 'LibraryReceiptSettings\/LibraryReceiptSettings.json'));
                                var vPvtLibraryReceiptSettingsOffline = ImportExportData_offline().LibraryReceiptSettings('');
                                var vPvtLibraryReceiptSettingsNeeded = [];
                                if (vPvtLibraryReceiptSettings.d.Results.Rows.length > 0) {
                                    intCntr = 0;
                                    for (var iRow = 0; iRow <= vPvtLibraryReceiptSettings.d.Results.Rows.length - 1; iRow++) {
                                        IsRecordFound = false;
                                        for (var iLRow = 0; iLRow <= vPvtLibraryReceiptSettingsOffline.Rows.length - 1; iLRow++) {
                                            if (vPvtLibraryReceiptSettings.d.Results.Rows[iRow].ID == vPvtLibraryReceiptSettingsOffline.Rows[iLRow].ID &&
                                                vPvtLibraryReceiptSettings.d.Results.Rows[iRow].LastUpdated == vPvtLibraryReceiptSettingsOffline.Rows[iLRow].LastUpdated) {

                                                vPvtLibraryReceiptSettingsNeeded[intCntr] = {
                                                    ID: vPvtLibraryReceiptSettings.d.Results.Rows[iRow].ID
                                                };
                                                intCntr = intCntr + 1;
                                                IsRecordFound = true;
                                            }
                                        }

                                        //If New Record
                                        if (IsRecordFound == false) {
                                            vPvtLibraryReceiptSettingsNeeded[intCntr] = {
                                                ID: vPvtLibraryReceiptSettings.d.Results.Rows[iRow].ID
                                            };
                                            intCntr = intCntr + 1;
                                        }
                                    }

                                }
                                //END LibraryReceiptSettings

                                //Public LibraryDevice
                                var vPvtLibraryDevice = JSON.parse(Transfers.ReadFile('RequestUpdate' + pData.OrgID + '.zip', 'LibraryDevice\/LibraryDevice.json'));
                                var vPvtLibraryDeviceOffline = ImportExportData_offline().LibraryDevice('');
                                var vPvtLibraryDeviceNeeded = [];
                                if (vPvtLibraryDevice.d.Results.Rows.length > 0) {
                                    intCntr = 0;
                                    for (var iRow = 0; iRow <= vPvtLibraryDevice.d.Results.Rows.length - 1; iRow++) {
                                        IsRecordFound = false;
                                        for (var iLRow = 0; iLRow <= vPvtLibraryDeviceOffline.Rows.length - 1; iLRow++) {
                                            if (vPvtLibraryDevice.d.Results.Rows[iRow].ID == vPvtLibraryDeviceOffline.Rows[iLRow].ID &&
                                                vPvtLibraryDevice.d.Results.Rows[iRow].LastUpdated == vPvtLibraryDeviceOffline.Rows[iLRow].LastUpdated) {

                                                vPvtLibraryDeviceNeeded[intCntr] = {
                                                    ID: vPvtLibraryDevice.d.Results.Rows[iRow].ID
                                                };
                                                intCntr = intCntr + 1;
                                            }
                                        }

                                        //If New Record
                                        if (IsRecordFound == false) {
                                            vPvtLibraryDeviceNeeded[intCntr] = {
                                                ID: vPvtLibraryDevice.d.Results.Rows[iRow].ID
                                            };
                                            intCntr = intCntr + 1;
                                        }
                                    }
                                }
                                //END LibraryDevice

                                //Public MobizLibraryBanks
                                var vPubLibraryBanks = JSON.parse(Transfers.ReadFile('RequestUpdate' + pData.OrgID + '.zip', 'MobizLibraryBanks\/MobizLibraryBanks.json'));
                                var vPubLibraryBanksOffline = ImportExportData_offline().MobizLibraryBanks('');
                                var vPubLibraryBanksNeeded = [];
                                if (vPubLibraryBanks.d.Results.Rows.length > 0) {
                                    intCntr = 0;
                                    for (var iRow = 0; iRow <= vPubLibraryBanks.d.Results.Rows.length - 1; iRow++) {
                                        IsRecordFound = false;
                                        for (var iLRow = 0; iLRow <= vPubLibraryBanksOffline.Rows.length - 1; iLRow++) {
                                            if (vPubLibraryBanks.d.Results.Rows[iRow].ID == vPubLibraryBanksOffline.Rows[iLRow].ID &&
                                                vPubLibraryBanks.d.Results.Rows[iRow].LastUpdated == vPubLibraryBanksOffline.Rows[iLRow].LastUpdated) {
                                                vPubLibraryBanksNeeded[intCntr] = {
                                                    ID: vPubLibraryBanks.d.Results.Rows[iRow].ID
                                                };
                                                intCntr = intCntr + 1;
                                            }
                                        }

                                        //If New Record
                                        if (IsRecordFound == false) {
                                            vPubLibraryBanksNeeded[intCntr] = {
                                                ID: vPubLibraryBanks.d.Results.Rows[iRow].ID
                                            };
                                            intCntr = intCntr + 1;
                                        }
                                    }
                                }
                                //END MobizLibraryBanks

                                //Public MobizLibraryCountries
                                var vPubLibraryCountries = JSON.parse(Transfers.ReadFile('RequestUpdate' + pData.OrgID + '.zip', 'MobizLibraryCountries\/MobizLibraryCountries.json'));
                                var vPubLibraryCountriesOffline = ImportExportData_offline().MobizLibraryCountries('');
                                var vPubLibraryCountriesNeeded = [];
                                if (vPubLibraryCountries.d.Results.Rows.length > 0) {
                                    intCntr = 0;
                                    for (var iRow = 0; iRow <= vPubLibraryCountries.d.Results.Rows.length - 1; iRow++) {
                                        IsRecordFound = false;
                                        for (var iLRow = 0; iLRow <= vPubLibraryCountriesOffline.Rows.length - 1; iLRow++) {
                                            if (vPubLibraryCountries.d.Results.Rows[iRow].ID == vPubLibraryCountriesOffline.Rows[iLRow].ID &&
                                                vPubLibraryCountries.d.Results.Rows[iRow].LastUpdated == vPubLibraryCountriesOffline.Rows[iLRow].LastUpdated) {

                                                vPubLibraryCountriesNeeded[intCntr] = {
                                                    ID: vPubLibraryCountries.d.Results.Rows[iRow].ID
                                                };
                                                intCntr = intCntr + 1;
                                            }
                                        }

                                        //If New Record
                                        if (IsRecordFound == false) {
                                            vPubLibraryCountriesNeeded[intCntr] = {
                                                ID: vPubLibraryCountries.d.Results.Rows[iRow].ID
                                            };
                                            intCntr = intCntr + 1;
                                        }
                                    }
                                }
                                //END MobizLibraryCountries

                                //Public MobizLibraryCities
                                var vPubLibraryCities = JSON.parse(Transfers.ReadFile('RequestUpdate' + pData.OrgID + '.zip', 'MobizLibraryCities\/MobizLibraryCities.json'));
                                var vPubLibraryCitiesOffline = ImportExportData_offline().MobizLibraryCities('');
                                var vPubLibraryCitiesNeeded = [];
                                if (vPubLibraryCities.d.Results.Rows.length > 0) {
                                    intCntr = 0;
                                    for (var iRow = 0; iRow <= vPubLibraryCities.d.Results.Rows.length - 1; iRow++) {
                                        IsRecordFound = false;
                                        for (var iLRow = 0; iLRow <= vPubLibraryCitiesOffline.Rows.length - 1; iLRow++) {
                                            if (vPubLibraryCities.d.Results.Rows[iRow].ID == vPubLibraryCitiesOffline.Rows[iLRow].ID &&
                                                vPubLibraryCities.d.Results.Rows[iRow].LastUpdated == vPubLibraryCitiesOffline.Rows[iLRow].LastUpdated) {

                                                vPubLibraryCitiesNeeded[intCntr] = {
                                                    ID: vPubLibraryCities.d.Results.Rows[iRow].ID
                                                };
                                                intCntr = intCntr + 1;
                                            }
                                        }

                                        //If New Record
                                        if (IsRecordFound == false) {
                                            vPubLibraryCitiesNeeded[intCntr] = {
                                                ID: vPubLibraryCities.d.Results.Rows[iRow].ID
                                            };
                                            intCntr = intCntr + 1;
                                        }
                                    }
                                }
                                //END MobizLibraryCities

                                //Public MobizLibraryAreaCodes
                                var vPubLibraryAreaCodes = JSON.parse(Transfers.ReadFile('RequestUpdate' + pData.OrgID + '.zip', 'MobizLibraryAreaCodes\/MobizLibraryAreaCodes.json'));
                                var vPubLibraryAreaCodesOffline = ImportExportData_offline().MobizLibraryAreaCodes('');
                                var vPubLibraryAreaCodesNeeded = [];
                                if (vPubLibraryAreaCodes.d.Results.Rows.length > 0) {
                                    intCntr = 0;
                                    for (var iRow = 0; iRow <= vPubLibraryAreaCodes.d.Results.Rows.length - 1; iRow++) {
                                        IsRecordFound = false;
                                        for (var iLRow = 0; iLRow <= vPubLibraryAreaCodesOffline.Rows.length - 1; iLRow++) {
                                            if (vPubLibraryAreaCodes.d.Results.Rows[iRow].ID == vPubLibraryAreaCodesOffline.Rows[iLRow].ID &&
                                                vPubLibraryAreaCodes.d.Results.Rows[iRow].LastUpdated == vPubLibraryAreaCodesOffline.Rows[iLRow].LastUpdated) {

                                                vPubLibraryAreaCodesNeeded[intCntr] = {
                                                    ID: vPubLibraryAreaCodes.d.Results.Rows[iRow].ID
                                                };
                                                intCntr = intCntr + 1;
                                            }
                                        }

                                        //If New Record
                                        if (IsRecordFound == false) {
                                            vPubLibraryAreaCodesNeeded[intCntr] = {
                                                ID: vPubLibraryAreaCodes.d.Results.Rows[iRow].ID
                                            };
                                            intCntr = intCntr + 1;
                                        }
                                    }
                                }
                                //END MobizLibraryAreaCodes

                                //Public MobizLibraryTelcos
                                var vPubLibraryTelcos = JSON.parse(Transfers.ReadFile('RequestUpdate' + pData.OrgID + '.zip', 'MobizLibraryTelcos\/MobizLibraryTelcos.json'));
                                var vPubLibraryTelcosOffline = ImportExportData_offline().MobizLibraryTelcos('');
                                var vPubLibraryTelcosNeeded = [];
                                if (vPubLibraryTelcos.d.Results.Rows.length > 0) {
                                    intCntr = 0;
                                    for (var iRow = 0; iRow <= vPubLibraryTelcos.d.Results.Rows.length - 1; iRow++) {
                                        IsRecordFound = false;
                                        for (var iLRow = 0; iLRow <= vPubLibraryTelcosOffline.Rows.length - 1; iLRow++) {
                                            if (vPubLibraryTelcos.d.Results.Rows[iRow].ID == vPubLibraryTelcosOffline.Rows[iLRow].ID &&
                                                vPubLibraryTelcos.d.Results.Rows[iRow].LastUpdated == vPubLibraryTelcosOffline.Rows[iLRow].LastUpdated) {

                                                vPubLibraryTelcosNeeded[intCntr] = {
                                                    ID: vPubLibraryTelcos.d.Results.Rows[iRow].ID
                                                };
                                                intCntr = intCntr + 1;
                                            }
                                        }

                                        //If New Record
                                        if (IsRecordFound == false) {
                                            vPubLibraryTelcosNeeded[intCntr] = {
                                                ID: vPubLibraryTelcos.d.Results.Rows[iRow].ID
                                            };
                                            intCntr = intCntr + 1;
                                        }
                                    }
                                }
                                //END MobizLibraryTelcos

                                //Public MobizLibraryTelcoPrefixes
                                var vPubLibraryTelcoPrefixes = JSON.parse(Transfers.ReadFile('RequestUpdate' + pData.OrgID + '.zip', 'MobizLibraryTelcoPrefixes\/MobizLibraryTelcoPrefixes.json'));
                                var vPubLibraryTelcoPrefixesOffline = ImportExportData_offline().MobizLibraryTelcoPrefixes('');
                                var vPubLibraryTelcoPrefixesNeeded = [];
                                if (vPubLibraryTelcoPrefixes.d.Results.Rows.length > 0) {
                                    intCntr = 0;
                                    for (var iRow = 0; iRow <= vPubLibraryTelcoPrefixes.d.Results.Rows.length - 1; iRow++) {
                                        IsRecordFound = false;
                                        for (var iLRow = 0; iLRow <= vPubLibraryTelcoPrefixesOffline.Rows.length - 1; iLRow++) {
                                            if (vPubLibraryTelcoPrefixes.d.Results.Rows[iRow].ID == vPubLibraryTelcoPrefixesOffline.Rows[iLRow].ID &&
                                                vPubLibraryTelcoPrefixes.d.Results.Rows[iRow].LastUpdated == vPubLibraryTelcoPrefixesOffline.Rows[iLRow].LastUpdated) {

                                                vPubLibraryTelcoPrefixesNeeded[intCntr] = {
                                                    ID: vPubLibraryTelcoPrefixes.d.Results.Rows[iRow].ID
                                                };
                                                intCntr = intCntr + 1;
                                            }
                                        }

                                        //If New Record
                                        if (IsRecordFound == false) {
                                            vPubLibraryTelcoPrefixesNeeded[intCntr] = {
                                                ID: vPubLibraryTelcoPrefixes.d.Results.Rows[iRow].ID
                                            };
                                            intCntr = intCntr + 1;
                                        }
                                    }
                                }
                                //END MobizLibraryTelcoPrefixes

                                //Public MobizLibraryPIMPriceLevel
                                var vPubLibraryPIMPriceLevel = JSON.parse(Transfers.ReadFile('RequestUpdate' + pData.OrgID + '.zip', 'MobizLibraryPIMPriceLevel\/MobizLibraryPIMPriceLevel.json'));
                                var vPubLibraryPIMPriceLevelOffline = ImportExportData_offline().MobizLibraryPIMPriceLevel('');
                                var vPubLibraryPIMPriceLevelNeeded = [];
                                if (vPubLibraryPIMPriceLevel.d.Results.Rows.length > 0) {
                                    intCntr = 0;
                                    for (var iRow = 0; iRow <= vPubLibraryPIMPriceLevel.d.Results.Rows.length - 1; iRow++) {
                                        IsRecordFound = false;
                                        for (var iLRow = 0; iLRow <= vPubLibraryPIMPriceLevelOffline.Rows.length - 1; iLRow++) {
                                            if (vPubLibraryPIMPriceLevel.d.Results.Rows[iRow].ID == vPubLibraryPIMPriceLevelOffline.Rows[iLRow].ID &&
                                                vPubLibraryPIMPriceLevel.d.Results.Rows[iRow].LastUpdated == vPubLibraryPIMPriceLevelOffline.Rows[iLRow].LastUpdated) {

                                                vPubLibraryPIMPriceLevelNeeded[intCntr] = {
                                                    ID: vPubLibraryPIMPriceLevel.d.Results.Rows[iRow].ID
                                                };
                                                intCntr = intCntr + 1;
                                            }
                                        }

                                        //If New Record
                                        if (IsRecordFound == false) {
                                            vPubLibraryPIMPriceLevelNeeded[intCntr] = {
                                                ID: vPubLibraryPIMPriceLevel.d.Results.Rows[iRow].ID
                                            };
                                            intCntr = intCntr + 1;
                                        }
                                    }
                                }
                                //END MobizLibraryPIMPriceLevel

                                //Public MobizLibraryPIMMeasure
                                var vPubLibraryPIMMeasure = JSON.parse(Transfers.ReadFile('RequestUpdate' + pData.OrgID + '.zip', 'MobizLibraryPIMMeasure\/MobizLibraryPIMMeasure.json'));
                                var vPubLibraryPIMMeasureOffline = ImportExportData_offline().MobizLibraryPIMProductPriceLevel('');
                                var vPubLibraryPIMMeasureNeeded = [];
                                if (vPubLibraryPIMMeasure.d.Results.Rows.length > 0) {
                                    intCntr = 0;
                                    for (var iRow = 0; iRow <= vPubLibraryPIMMeasure.d.Results.Rows.length - 1; iRow++) {
                                        IsRecordFound = false;
                                        for (var iLRow = 0; iLRow <= vPubLibraryPIMMeasureOffline.Rows.length - 1; iLRow++) {
                                            if (vPubLibraryPIMMeasure.d.Results.Rows[iRow].ID == vPubLibraryPIMMeasureOffline.Rows[iLRow].ID &&
                                                vPubLibraryPIMMeasure.d.Results.Rows[iRow].LastUpdated == vPubLibraryPIMMeasureOffline.Rows[iLRow].LastUpdated) {

                                                vPubLibraryPIMMeasureNeeded[intCntr] = {
                                                    ID: vPubLibraryPIMMeasure.d.Results.Rows[iRow].ID
                                                };
                                                intCntr = intCntr + 1;
                                            }
                                        }

                                        //If New Record
                                        if (IsRecordFound == false) {
                                            vPubLibraryPIMMeasureNeeded[intCntr] = {
                                                ID: vPubLibraryPIMMeasure.d.Results.Rows[iRow].ID
                                            };
                                            intCntr = intCntr + 1;
                                        }
                                    }
                                }
                                //END MobizLibraryPIMMeasure

                                //Public LibraryPIMMeasureSign
                                var vPubLibraryPIMMeasureSign = JSON.parse(Transfers.ReadFile('RequestUpdate' + pData.OrgID + '.zip', 'MobizLibraryPIMMeasureSign\/MobizLibraryPIMMeasureSign.json'));
                                var vPubLibraryPIMMeasureSignOffline = ImportExportData_offline().MobizLibraryPIMProductPriceLevel('');
                                var vPubLibraryPIMMeasureSignNeeded = [];
                                if (vPubLibraryPIMMeasureSign.d.Results.Rows.length > 0) {
                                    intCntr = 0;
                                    for (var iRow = 0; iRow <= vPubLibraryPIMMeasureSign.d.Results.Rows.length - 1; iRow++) {
                                        IsRecordFound = false;
                                        for (var iLRow = 0; iLRow <= vPubLibraryPIMMeasureSignOffline.Rows.length - 1; iLRow++) {
                                            if (vPubLibraryPIMMeasureSign.d.Results.Rows[iRow].ID == vPubLibraryPIMMeasureSignOffline.Rows[iLRow].ID &&
                                                vPubLibraryPIMMeasureSign.d.Results.Rows[iRow].LastUpdated == vPubLibraryPIMMeasureSignOffline.Rows[iLRow].LastUpdated) {

                                                vPubLibraryPIMMeasureSignNeeded[intCntr] = {
                                                    ID: vPubLibraryPIMMeasureSign.d.Results.Rows[iRow].ID
                                                };
                                                intCntr = intCntr + 1;
                                            }
                                        }

                                        //If New Record
                                        if (IsRecordFound == false) {
                                            vPubLibraryPIMMeasureSignNeeded[intCntr] = {
                                                ID: vPubLibraryPIMMeasureSign.d.Results.Rows[iRow].ID
                                            };
                                            intCntr = intCntr + 1;
                                        }
                                    }
                                }
                                //END LibraryPIMMeasureSign

                                //Public MobizLibraryPeoplePreNominalSuffix
                                var vPubLibraryPeoplePreNominalSuffix = JSON.parse(Transfers.ReadFile('RequestUpdate' + pData.OrgID + '.zip', 'MobizLibraryPeoplePreNominalSuffix\/MobizLibraryPeoplePreNominalSuffix.json'));
                                var vPubLibraryPeoplePreNominalSuffixOffline = ImportExportData_offline().MobizLibraryPIMProductPriceLevel('');
                                var vPubLibraryPeoplePreNominalSuffixNeeded = [];
                                if (vPubLibraryPeoplePreNominalSuffix.d.Results.Rows.length > 0) {
                                    intCntr = 0;
                                    for (var iRow = 0; iRow <= vPubLibraryPeoplePreNominalSuffix.d.Results.Rows.length - 1; iRow++) {
                                        IsRecordFound = false;
                                        for (var iLRow = 0; iLRow <= vPubLibraryPeoplePreNominalSuffixOffline.Rows.length - 1; iLRow++) {
                                            if (vPubLibraryPeoplePreNominalSuffix.d.Results.Rows[iRow].ID == vPubLibraryPeoplePreNominalSuffixOffline.Rows[iLRow].ID &&
                                                vPubLibraryPeoplePreNominalSuffix.d.Results.Rows[iRow].LastUpdated == vPubLibraryPeoplePreNominalSuffixOffline.Rows[iLRow].LastUpdated) {

                                                vPubLibraryPeoplePreNominalSuffixNeeded[intCntr] = {
                                                    ID: vPubLibraryPeoplePreNominalSuffix.d.Results.Rows[iRow].ID
                                                };
                                                intCntr = intCntr + 1;
                                            }
                                        }

                                        //If New Record
                                        if (IsRecordFound == false) {
                                            vPubLibraryPeoplePreNominalSuffixNeeded[intCntr] = {
                                                ID: vPubLibraryPeoplePreNominalSuffix.d.Results.Rows[iRow].ID
                                            };
                                            intCntr = intCntr + 1;
                                        }
                                    }
                                }
                                //END MobizLibraryPeoplePreNominalSuffix

                                //Public MobizLibraryPeople
                                var vPubLibraryPeople = JSON.parse(Transfers.ReadFile('RequestUpdate' + pData.OrgID + '.zip', 'MobizLibraryPeople\/MobizLibraryPeople.json'));
                                var vPubLibraryPeopleOffline = ImportExportData_offline().MobizLibraryPeople('');
                                var vPubLibraryPeopleNeeded = [];
                                if (vPubLibraryPeople.d.Results.Rows.length > 0) {
                                    intCntr = 0;
                                    for (var iRow = 0; iRow <= vPubLibraryPeople.d.Results.Rows.length - 1; iRow++) {
                                        IsRecordFound = false;
                                        for (var iLRow = 0; iLRow <= vPubLibraryPeopleOffline.Rows.length - 1; iLRow++) {
                                            if (vPubLibraryPeople.d.Results.Rows[iRow].ID == vPubLibraryPeopleOffline.Rows[iLRow].ID &&
                                                vPubLibraryPeople.d.Results.Rows[iRow].LastUpdated == vPubLibraryPeopleOffline.Rows[iLRow].LastUpdated) {

                                                vPubLibraryPeopleNeeded[intCntr] = {
                                                    ID: vPubLibraryPeople.d.Results.Rows[iRow].ID
                                                };

                                                intCntr = intCntr + 1;
                                            }
                                        }

                                        //If New Record
                                        if (IsRecordFound == false) {
                                            vPubLibraryPeopleNeeded[intCntr] = {
                                                ID: vPubLibraryPeople.d.Results.Rows[iRow].ID
                                            };
                                            intCntr = intCntr + 1;
                                        }
                                    }
                                }
                                //END MobizLibraryPeople

                                //Public MobizLibraryOrganization
                                var vPubLibraryOrganization = JSON.parse(Transfers.ReadFile('RequestUpdate' + pData.OrgID + '.zip', 'MobizLibraryOrganization\/MobizLibraryOrganization.json'));
                                var vPubLibraryOrganizationOffline = ImportExportData_offline().MobizLibraryOrganization('');
                                var vPubLibraryOrganizationNeeded = [];
                                if (vPubLibraryOrganization.d.Results.Rows.length > 0) {
                                    intCntr = 0;
                                    for (var iRow = 0; iRow <= vPubLibraryOrganization.d.Results.Rows.length - 1; iRow++) {
                                        IsRecordFound = false;
                                        for (var iLRow = 0; iLRow <= vPubLibraryOrganizationOffline.Rows.length - 1; iLRow++) {
                                            if (vPubLibraryOrganization.d.Results.Rows[iRow].ID == vPubLibraryOrganizationOffline.Rows[iLRow].ID &&
                                                vPubLibraryOrganization.d.Results.Rows[iRow].LastUpdated == vPubLibraryOrganizationOffline.Rows[iLRow].LastUpdated) {

                                                vPubLibraryOrganizationNeeded[intCntr] = {
                                                    ID: vPubLibraryOrganization.d.Results.Rows[iRow].ID
                                                };

                                                intCntr = intCntr + 1;
                                            }
                                        }

                                        //If New Record
                                        if (IsRecordFound == false) {

                                            vPubLibraryOrganizationNeeded[intCntr] = {
                                                ID: vPubLibraryOrganization.d.Results.Rows[iRow].ID
                                            };

                                            intCntr = intCntr + 1;
                                        }
                                    }
                                }
                                //END MobizLibraryOrganization

                                //Public LibraryOrganizationCustomer
                                var vLibraryOrganizationCustomer = JSON.parse(Transfers.ReadFile('RequestUpdate' + pData.OrgID + '.zip', 'LibraryOrganizationCustomer\/LibraryOrganizationCustomer.json'));
                                var vLibraryOrganizationCustomerNeeded = [];
                                if (vLibraryOrganizationCustomer.d.Results.Rows.length > 0) {
                                    intCntr = 0;
                                    for (var iRow = 0; iRow <= vLibraryOrganizationCustomer.d.Results.Rows.length - 1; iRow++) {
                                        vLibraryOrganizationCustomerNeeded[intCntr] = {
                                            ID: vLibraryOrganizationCustomer.d.Results.Rows[iRow].ID
                                        };

                                        intCntr = intCntr + 1;
                                    }
                                }
                                //END LibraryOrganizationCustomer

                                //Public LibraryOrganizationEmployee
                                var vLibraryOrganizationEmployee = JSON.parse(Transfers.ReadFile('RequestUpdate' + pData.OrgID + '.zip', 'LibraryOrganizationEmployee\/LibraryOrganizationEmployee.json'));
                                var vLibraryOrganizationEmployeeNeeded = [];
                                if (vLibraryOrganizationEmployee.d.Results.Rows.length > 0) {
                                    intCntr = 0;
                                    for (var iRow = 0; iRow <= vLibraryOrganizationEmployee.d.Results.Rows.length - 1; iRow++) {
                                        vLibraryOrganizationEmployeeNeeded[intCntr] = {
                                            ID: vLibraryOrganizationEmployee.d.Results.Rows[iRow].ID
                                        };

                                        intCntr = intCntr + 1;
                                    }
                                }
                                //END LibraryOrganizationEmployee

                                //Public LibraryOrganizationEmployeeFOWST
                                var vLibraryOrganizationEmployeeFOWST = JSON.parse(Transfers.ReadFile('RequestUpdate' + pData.OrgID + '.zip', 'LibraryOrganizationEmployeeFOWST\/LibraryOrganizationEmployeeFOWST.json'));
                                var vLibraryOrganizationEmployeeFOWSTNeeded = [];
                                if (vLibraryOrganizationEmployeeFOWST.d.Results.Rows.length > 0) {
                                    intCntr = 0;
                                    for (var iRow = 0; iRow <= vLibraryOrganizationEmployeeFOWST.d.Results.Rows.length - 1; iRow++) {
                                        vLibraryOrganizationEmployeeFOWSTNeeded[intCntr] = {
                                            ID: vLibraryOrganizationEmployeeFOWST.d.Results.Rows[iRow].ID
                                        };

                                        intCntr = intCntr + 1;
                                    }
                                }
                                //END LibraryOrganizationEmployeeFOWST

                                //                                //Public MobizLibraryPIMProduct
                                //                                var vPubLibraryPIMProduct = JSON.parse(Transfers.ReadFile('RequestUpdate' + pData.OrgID + '.zip', 'MobizLibraryPIMProduct\MobizLibraryPIMProduct.json'));
                                //                                var vPubLibraryPIMProductOffline = ImportExportData_offline().MobizLibraryPIMProduct('');
                                //                                var vPubLibraryPIMProductNeeded = [];
                                //                                if (vPubLibraryPIMProduct.d.Results.Rows.length > 0) {
                                //                                    intCntr = 0;
                                //                                    for (var iRow = 0; iRow <= vPubLibraryPIMProduct.d.Results.Rows.length - 1; iRow++) {
                                //                                        IsRecordFound = false;
                                //                                        for (var iLRow = 0; iLRow <= vPubLibraryPIMProductOffline.Rows.length - 1; iLRow++) {
                                //                                            if (vPubLibraryPIMProduct.d.Results.Rows[iRow].ID == vPubLibraryPIMProductOffline.Rows[iLRow].ID &&
                                //                                                vPubLibraryPIMProduct.d.Results.Rows[iRow].LastUpdated == vPubLibraryPIMProductOffline.Rows[iLRow].LastUpdated) {

                                //                                                vPubLibraryPIMProductNeeded[intCntr] = {
                                //                                                    ID: vPubLibraryPIMProduct.d.Results.Rows[iRow].ID
                                //                                                };

                                //                                                intCntr = intCntr + 1;
                                //                                            }
                                //                                        }

                                //                                        //If New Record
                                //                                        if (IsRecordFound == false) {
                                //                                            vPubLibraryPIMProductNeeded[intCntr] = {
                                //                                                ID: vPubLibraryPIMProduct.d.Results.Rows[iRow].ID
                                //                                            };

                                //                                            intCntr = intCntr + 1;
                                //                                        }
                                //                                    }
                                //                                }
                                //                                //END MobizLibraryPIMProduct

                                //                                //Public MobizLibraryPIMProductMeasurementBarcode
                                //                                var vPvtMobizLibraryPIMProductMeasurementBarcodeData = JSON.parse(Transfers.ReadFile('RequestUpdate' + pData.OrgID + '.zip', 'MobizLibraryPIMProductMeasurementBarcode\MobizLibraryPIMProductMeasurementBarcode.json'));
                                //                                var vPvtMobizLibraryPIMProductMeasurementBarcodeDataNeeded = [];
                                //                                if (vPvtMobizLibraryPIMProductMeasurementBarcodeData.d.Results.Rows.length > 0) {
                                //                                    intCntr = 0;
                                //                                    for (var iRow = 0; iRow <= vPvtMobizLibraryPIMProductMeasurementBarcodeData.d.Results.Rows.length - 1; iRow++) {
                                //                                        vPvtMobizLibraryPIMProductMeasurementBarcodeDataNeeded[intCntr] = {
                                //                                            ID: vPvtMobizLibraryPIMProductMeasurementBarcodeData.d.Results.Rows[iRow].ID
                                //                                        };
                                //                                        intCntr = intCntr + 1;
                                //                                    }
                                //                                }
                                //                                //END MobizLibraryPIMProductMeasurementBarcode

                                //Public LibrarySecurityModuleMain
                                var vPvtLibrarySecurityModuleMain = JSON.parse(Transfers.ReadFile('RequestUpdate' + pData.OrgID + '.zip', 'LibrarySecurityModuleMain\/LibrarySecurityModuleMain.json'));
                                var vPvtLibrarySecurityModuleMainNeeded = [];
                                if (vPvtLibrarySecurityModuleMain.d.Results.Rows.length > 0) {
                                    intCntr = 0;
                                    for (var iRow = 0; iRow <= vPvtLibrarySecurityModuleMain.d.Results.Rows.length - 1; iRow++) {
                                        vPvtLibrarySecurityModuleMainNeeded[intCntr] = {
                                            ID: vPvtLibrarySecurityModuleMain.d.Results.Rows[iRow].ID
                                        };
                                        intCntr = intCntr + 1;
                                    }
                                }
                                //END LibraryPIMMeasureSign

                                Transfers.WriteFile("RequestFinal.zip", "SystemDefaults.json", "{\"Rows\":" + JSON.stringify(vPvtSysDefDataNeeded) + "}");
                                Transfers.WriteFile("RequestFinal.zip", "MobizSystemDefaults.json", "{\"Rows\":" + JSON.stringify(vPubMobizSysDefDataNeeded) + "}");
                                Transfers.WriteFile("RequestFinal.zip", "MobizLibraryCountriesLanguageLabel.json", "{\"Rows\":" + JSON.stringify(vPubLibraryCountriesLanguageLabelNeeded) + "}");
                                Transfers.WriteFile("RequestFinal.zip", "LibraryDiscountSettings.json", "{\"Rows\":" + JSON.stringify(vPvtLibraryDiscountSettingsNeeded) + "}");

                                Transfers.WriteFile("RequestFinal.zip", "LibrarySecurityRole.json", "{\"Rows\":" + JSON.stringify(vPvtLibrarySecurityRoleNeeded) + "}");
                                Transfers.WriteFile("RequestFinal.zip", "LibraryReceiptSettings.json", "{\"Rows\":" + JSON.stringify(vPvtLibraryReceiptSettingsNeeded) + "}");
                                Transfers.WriteFile("RequestFinal.zip", "LibraryDevice.json", "{\"Rows\":" + JSON.stringify(vPvtLibraryDeviceNeeded) + "}");

                                Transfers.WriteFile("RequestFinal.zip", "MobizLibraryBanks.json", "{\"Rows\":" + JSON.stringify(vPubLibraryBanksNeeded) + "}");
                                Transfers.WriteFile("RequestFinal.zip", "MobizLibraryCountries.json", "{\"Rows\":" + JSON.stringify(vPubLibraryCountriesNeeded) + "}");
                                Transfers.WriteFile("RequestFinal.zip", "MobizLibraryCities.json", "{\"Rows\":" + JSON.stringify(vPubLibraryCitiesNeeded) + "}");
                                Transfers.WriteFile("RequestFinal.zip", "MobizLibraryAreaCodes.json", "{\"Rows\":" + JSON.stringify(vPubLibraryAreaCodesNeeded) + "}");

                                Transfers.WriteFile("RequestFinal.zip", "MobizLibraryTelcos.json", "{\"Rows\":" + JSON.stringify(vPubLibraryTelcosNeeded) + "}");
                                Transfers.WriteFile("RequestFinal.zip", "MobizLibraryTelcoPrefixes.json", "{\"Rows\":" + JSON.stringify(vPubLibraryTelcoPrefixesNeeded) + "}");
                                Transfers.WriteFile("RequestFinal.zip", "MobizLibraryPIMPriceLevel.json", "{\"Rows\":" + JSON.stringify(vPubLibraryPIMPriceLevelNeeded) + "}");
                                Transfers.WriteFile("RequestFinal.zip", "MobizLibraryPIMMeasure.json", "{\"Rows\":" + JSON.stringify(vPubLibraryPIMMeasureNeeded) + "}");

                                Transfers.WriteFile("RequestFinal.zip", "MobizLibraryPIMMeasureSign.json", "{\"Rows\":" + JSON.stringify(vPubLibraryPIMMeasureSignNeeded) + "}");
                                Transfers.WriteFile("RequestFinal.zip", "MobizLibraryPeoplePreNominalSuffix.json", "{\"Rows\":" + JSON.stringify(vPubLibraryPeoplePreNominalSuffixNeeded) + "}");
                                Transfers.WriteFile("RequestFinal.zip", "MobizLibraryPeople.json", "{\"Rows\":" + JSON.stringify(vPubLibraryPeopleNeeded) + "}");
                                Transfers.WriteFile("RequestFinal.zip", "MobizLibraryOrganization.json", "{\"Rows\":" + JSON.stringify(vPubLibraryOrganizationNeeded) + "}");

                                Transfers.WriteFile("RequestFinal.zip", "LibraryOrganizationEmployee.json", "{\"Rows\":" + JSON.stringify(vLibraryOrganizationEmployeeNeeded) + "}");
                                Transfers.WriteFile("RequestFinal.zip", "LibraryOrganizationEmployeeFOWST.json", "{\"Rows\":" + JSON.stringify(vLibraryOrganizationEmployeeFOWSTNeeded) + "}");
                                Transfers.WriteFile("RequestFinal.zip", "LibraryOrganizationCustomer.json", "{\"Rows\":" + JSON.stringify(vLibraryOrganizationCustomerNeeded) + "}");

                                //                                Transfers.WriteFile("RequestFinal.zip", "MobizLibraryPIMProduct.json", "{\"Rows\":" + JSON.stringify(vPubLibraryPIMProductNeeded) + "}");
                                //                                Transfers.WriteFile("RequestFinal.zip", "MobizLibraryPIMPriceLevel.json", "{\"Rows\":" + JSON.stringify(vPubLibraryPIMPriceLevelNeeded) + "}");
                                //                                Transfers.WriteFile("RequestFinal.zip", "MobizLibraryPIMProductMeasurementBarcode.json", "{\"Rows\":" + JSON.stringify(vPvtMobizLibraryPIMProductMeasurementBarcodeDataNeeded) + "}");
                                Transfers.WriteFile("RequestFinal.zip", "LibrarySecurityModuleMain.json", "{\"Rows\":" + JSON.stringify(vPvtLibrarySecurityModuleMainNeeded) + "}");

                                //Upload to Server
                                Transfers.Upload({
                                    SourceFilename: 'RequestFinal.zip',
                                    Success: function (vDataUpload) {

                                        Transfers.Download({
                                            Key: JSON.parse(vDataUpload.Results).Results,
                                            Destination: 'RequestFinalUpdate' + pData.OrgID + '.zip',
                                            Success: function (vDataDLFinal) {

                                                //Private  System Defaults
                                                vPvtSysDefData = JSON.parse(Transfers.ReadFile('RequestFinalUpdate' + pData.OrgID + '.zip', 'SystemDefaults.json'));
                                                SaveSystemDefaults(vPvtSysDefData, 0);
                                                //                                                try {
                                                //                                                    vPvtSysDefData = JSON.parse(Transfers.ReadFile('RequestFinalUpdate' + pData.OrgID + '.zip', 'SystemDefaults.json'));
                                                //                                                    if (vPvtSysDefData.d.Results.Rows.length > 0) {
                                                //                                                        intCntr = 0;
                                                //                                                        for (var iRow = 0; iRow <= vPvtSysDefData.d.Results.Rows.length - 1; iRow++) {
                                                //                                                            SaveSystemDefaults(vPvtSysDefData.d.Results.Rows[iRow]);
                                                //                                                        }
                                                //                                                    }
                                                //                                                } catch (e) {
                                                //                                                    console.error(e);
                                                //                                                };
                                                //END System Defaults

                                                //Private Mobiz System Defaults
                                                var vPubMobizSysDefData = JSON.parse(Transfers.ReadFile('RequestFinalUpdate' + pData.OrgID + '.zip', 'MobizSystemDefaults.json'));
                                                if (vPubMobizSysDefData.d.Results.Rows.length > 0) {
                                                    intCntr = 0;
                                                    for (var iRow = 0; iRow <= 160; iRow++) {
                                                        var vPubMobizSysDefDataOffline = ImportExportData_offline().getMobizSystemDefaults(vPubMobizSysDefData.d.Results.Rows[iRow].ID);

                                                        if (iRow == 140) {
                                                            alert('test');
                                                        }
                                                        if (vPubMobizSysDefDataOffline != null) {
                                                            IsSuccess = ImportExportData_offline().SaveMobizSystemDefaults((vPubMobizSysDefDataOffline.Rows.length > 0 ? false : true), vPubMobizSysDefData.d.Results.Rows[iRow]);
                                                        }
                                                    }
                                                }
                                                //END Mobiz System Defaults

                                                //Public MobizLibraryCountriesLanguageLabel
                                                var vPubLibraryCountriesLanguageLabel = JSON.parse(Transfers.ReadFile('RequestFinalUpdate' + pData.OrgID + '.zip', 'MobizLibraryCountriesLanguageLabel.json'));
                                                if (vPubLibraryCountriesLanguageLabel.d.Results.Rows.length > 0) {
                                                    intCntr = 0;
                                                    for (var iRow = 0; iRow <= vPubLibraryCountriesLanguageLabel.d.Results.Rows.length - 1; iRow++) {
                                                        var vPubLibraryCountriesLanguageLabelOffline = ImportExportData_offline().MobizLibraryCountriesLanguageLabel(vPubLibraryCountriesLanguageLabel.d.Results.Rows[iRow].ID);

                                                        if (vPubLibraryCountriesLanguageLabelOffline != null) {
                                                            IsSuccess = ImportExportData_offline().SaveMobizLibraryCountriesLanguageLabel((vPubLibraryCountriesLanguageLabelOffline.Rows.length > 0 ? false : true), vPubLibraryCountriesLanguageLabel.d.Results.Rows[iRow]);
                                                        }
                                                    }
                                                }
                                                //END MobizLibraryCountriesLanguageLabel

                                                //Public LibraryDiscountSettings
                                                var vPvtLibraryDiscountSettings = JSON.parse(Transfers.ReadFile('RequestFinalUpdate' + pData.OrgID + '.zip', 'LibraryDiscountSettings.json'));
                                                if (vPvtLibraryDiscountSettings.d.Results.Rows.length > 0) {
                                                    intCntr = 0;
                                                    for (var iRow = 0; iRow <= vPvtLibraryDiscountSettings.d.Results.Rows.length - 1; iRow++) {
                                                        var vPvtLibraryDiscountSettingsOffline = ImportExportData_offline().LibraryDiscountSettings(vPvtLibraryDiscountSettings.d.Results.Rows[iRow].ID);

                                                        if (vPvtLibraryDiscountSettingsOffline != null) {
                                                            IsSuccess = ImportExportData_offline().SaveLibraryDiscountSettings((vPvtLibraryDiscountSettingsOffline.Rows.length > 0 ? false : true), vPvtLibraryDiscountSettings.d.Results.Rows[iRow]);
                                                        }
                                                    }
                                                }
                                                //END LibraryDiscountSettings

                                                //Public LibrarySecurityRole
                                                var vLibrarySecurityModuleMain = JSON.parse(Transfers.ReadFile('RequestFinalUpdate' + pData.OrgID + '.zip', 'LibrarySecurityModuleMain.json'));
                                                if (vLibrarySecurityModuleMain.d.Results.Rows.length > 0) {
                                                    intCntr = 0;
                                                    for (var iRow = 0; iRow <= vLibrarySecurityModuleMain.d.Results.Rows.length - 1; iRow++) {
                                                        var vLibrarySecurityModuleMainOffline = ImportExportData_offline().LibrarySecurityRole(vLibrarySecurityModuleMain.d.Results.Rows[iRow].ID);

                                                        if (vLibrarySecurityModuleMainOffline != null) {
                                                            IsSuccess = ImportExportData_offline().SaveLibrarySecurityModuleMain((vLibrarySecurityModuleMainOffline.Rows.length > 0 ? false : true), vLibrarySecurityModuleMain.d.Results.Rows[iRow]);
                                                        }
                                                    }
                                                }

                                                var vLibrarySecurityModule = JSON.parse(Transfers.ReadFile('RequestFinalUpdate' + pData.OrgID + '.zip', 'LibrarySecurityModule.json'));
                                                if (vLibrarySecurityModule.d.Results.Rows.length > 0) {
                                                    intCntr = 0;
                                                    for (var iRow = 0; iRow <= vLibrarySecurityModule.d.Results.Rows.length - 1; iRow++) {
                                                        var vLibrarySecurityModuleOffline = ImportExportData_offline().LibrarySecurityModule(vLibrarySecurityModule.d.Results.Rows[iRow].ID);

                                                        if (vLibrarySecurityModuleOffline != null) {
                                                            IsSuccess = ImportExportData_offline().SaveLibrarySecurityModule((vLibrarySecurityModuleOffline.Rows.length > 0 ? false : true), vLibrarySecurityModule.d.Results.Rows[iRow]);
                                                        }
                                                    }
                                                }

                                                var vPvtLibrarySecurityRole = JSON.parse(Transfers.ReadFile('RequestFinalUpdate' + pData.OrgID + '.zip', 'LibrarySecurityRole.json'));
                                                if (vPvtLibrarySecurityRole.d.Results.Rows.length > 0) {
                                                    intCntr = 0;
                                                    for (var iRow = 0; iRow <= vPvtLibrarySecurityRole.d.Results.Rows.length - 1; iRow++) {
                                                        var vPvtLibrarySecurityRoleOffline = ImportExportData_offline().LibrarySecurityRole(vPvtLibrarySecurityRole.d.Results.Rows[iRow].ID);

                                                        if (vPvtLibrarySecurityRoleOffline != null) {
                                                            IsSuccess = ImportExportData_offline().SaveLibrarySecurityRole((vPvtLibrarySecurityRoleOffline.Rows.length > 0 ? false : true), vPvtLibrarySecurityRole.d.Results.Rows[iRow]);
                                                        }
                                                    }
                                                }

                                                var vLibrarySecurityRoleModules = JSON.parse(Transfers.ReadFile('RequestFinalUpdate' + pData.OrgID + '.zip', 'LibrarySecurityRoleModules.json'));
                                                if (vLibrarySecurityRoleModules.d.Results.Rows.length > 0) {
                                                    intCntr = 0;
                                                    for (var iRow = 0; iRow <= vLibrarySecurityRoleModules.d.Results.Rows.length - 1; iRow++) {
                                                        var vLibrarySecurityRoleModulesOffline = ImportExportData_offline().LibrarySecurityRole(vLibrarySecurityRoleModules.d.Results.Rows[iRow].ID);

                                                        if (vLibrarySecurityRoleModulesOffline != null) {
                                                            IsSuccess = ImportExportData_offline().SaveLibrarySecurityRoleModules((vLibrarySecurityRoleModulesOffline.Rows.length > 0 ? false : true), vLibrarySecurityRoleModules.d.Results.Rows[iRow]);
                                                        }
                                                    }
                                                }

                                                var vLibrarySecurityRoleUser = JSON.parse(Transfers.ReadFile('RequestFinalUpdate' + pData.OrgID + '.zip', 'LibrarySecurityRoleUser.json'));
                                                if (vLibrarySecurityRoleUser.d.Results.Rows.length > 0) {
                                                    intCntr = 0;
                                                    for (var iRow = 0; iRow <= vLibrarySecurityRoleUser.d.Results.Rows.length - 1; iRow++) {
                                                        var vLibrarySecurityRoleUserOffline = ImportExportData_offline().LibrarySecurityRole(vLibrarySecurityRoleUser.d.Results.Rows[iRow].ID);

                                                        if (vLibrarySecurityRoleUserOffline != null) {
                                                            IsSuccess = ImportExportData_offline().SaveLibrarySecurityRoleUser((vLibrarySecurityRoleUserOffline.Rows.length > 0 ? false : true), vLibrarySecurityRoleUser.d.Results.Rows[iRow]);
                                                        }
                                                    }
                                                }
                                                //END LibrarySecurityRole

                                                //Public LibraryReceiptSettings
                                                var vPvtLibraryReceiptSettings = JSON.parse(Transfers.ReadFile('RequestFinalUpdate' + pData.OrgID + '.zip', 'LibraryReceiptSettings.json'));
                                                if (vPvtLibraryReceiptSettings.d.Results.Rows.length > 0) {
                                                    intCntr = 0;
                                                    for (var iRow = 0; iRow <= vPvtLibraryReceiptSettings.d.Results.Rows.length - 1; iRow++) {
                                                        var vPvtLibraryReceiptSettingsOffline = ImportExportData_offline().LibraryReceiptSettings(vPvtLibraryReceiptSettings.d.Results.Rows[iRow].ID);

                                                        if (vPvtLibraryReceiptSettingsOffline != null) {
                                                            IsSuccess = ImportExportData_offline().SaveLibraryReceiptSettings((vPvtLibraryReceiptSettingsOffline.Rows.length > 0 ? false : true), vPvtLibraryReceiptSettings.d.Results.Rows[iRow]);
                                                        }
                                                    }
                                                }

                                                var vLibraryReceiptSettingsFowst = JSON.parse(Transfers.ReadFile('RequestFinalUpdate' + pData.OrgID + '.zip', 'LibraryReceiptSettingsFowst.json'));
                                                if (vLibraryReceiptSettingsFowst.d.Results.Rows.length > 0) {
                                                    intCntr = 0;
                                                    for (var iRow = 0; iRow <= vLibraryReceiptSettingsFowst.d.Results.Rows.length - 1; iRow++) {
                                                        var vLibraryReceiptSettingsFowstOffline = ImportExportData_offline().LibraryReceiptSettingsFowst(vLibraryReceiptSettingsFowst.d.Results.Rows[iRow].ID);

                                                        if (vLibraryReceiptSettingsFowstOffline != null) {
                                                            IsSuccess = ImportExportData_offline().SaveLibraryReceiptSettingsFowst((vLibraryReceiptSettingsFowstOffline.Rows.length > 0 ? false : true), vLibraryReceiptSettingsFowst.d.Results.Rows[iRow]);
                                                        }
                                                    }
                                                }

                                                var vLibraryReceiptSettingsMessage = JSON.parse(Transfers.ReadFile('RequestFinalUpdate' + pData.OrgID + '.zip', 'LibraryReceiptSettingsMessage.json'));
                                                if (vLibraryReceiptSettingsMessage.d.Results.Rows.length > 0) {
                                                    intCntr = 0;
                                                    for (var iRow = 0; iRow <= vLibraryReceiptSettingsMessage.d.Results.Rows.length - 1; iRow++) {
                                                        var vLibraryReceiptSettingsMessageOffline = ImportExportData_offline().LibraryReceiptSettingsMessage(vLibraryReceiptSettingsMessage.d.Results.Rows[iRow].ID);

                                                        if (vLibraryReceiptSettingsMessageOffline != null) {
                                                            IsSuccess = ImportExportData_offline().SaveLibraryReceiptSettingsMessage((vLibraryReceiptSettingsMessageOffline.Rows.length > 0 ? false : true), vLibraryReceiptSettingsMessage.d.Results.Rows[iRow]);
                                                        }
                                                    }
                                                }
                                                //END LibraryReceiptSettings

                                                //Public LibraryDevice
                                                var vPvtLibraryDevice = JSON.parse(Transfers.ReadFile('RequestFinalUpdate' + pData.OrgID + '.zip', 'LibraryDevice.json'));
                                                if (vPvtLibraryDevice.d.Results.Rows.length > 0) {
                                                    intCntr = 0;
                                                    for (var iRow = 0; iRow <= vPvtLibraryDevice.d.Results.Rows.length - 1; iRow++) {
                                                        var vPvtLibraryDeviceOffline = ImportExportData_offline().LibraryReceiptSettingsMessage(vPvtLibraryDevice.d.Results.Rows[iRow].ID);

                                                        if (vPvtLibraryDeviceOffline != null) {
                                                            IsSuccess = ImportExportData_offline().SaveLibraryDevice((vPvtLibraryDeviceOffline.Rows.length > 0 ? false : true), vPvtLibraryDevice.d.Results.Rows[iRow]);
                                                        }
                                                    }
                                                }

                                                var vPvtLibraryDeviceFowst = JSON.parse(Transfers.ReadFile('RequestFinalUpdate' + pData.OrgID + '.zip', 'LibraryDeviceFowst.json'));
                                                if (vPvtLibraryDeviceFowst.d.Results.Rows.length > 0) {
                                                    intCntr = 0;
                                                    for (var iRow = 0; iRow <= vPvtLibraryDeviceFowst.d.Results.Rows.length - 1; iRow++) {
                                                        var vPvtLibraryDeviceFowstOffline = ImportExportData_offline().LibraryDeviceFowst(vPvtLibraryDeviceFowst.d.Results.Rows[iRow].ID);

                                                        if (vPvtLibraryDeviceFowstOffline != null) {
                                                            IsSuccess = ImportExportData_offline().SaveLibraryDeviceFowst((vPvtLibraryDeviceFowstOffline.Rows.length > 0 ? false : true), vPvtLibraryDeviceFowst.d.Results.Rows[iRow]);
                                                        }
                                                    }
                                                }

                                                var vLibraryDeviceFowstPrinter = JSON.parse(Transfers.ReadFile('RequestFinalUpdate' + pData.OrgID + '.zip', 'LibraryDeviceFowstPrinter.json'));
                                                if (vLibraryDeviceFowstPrinter.d.Results.Rows.length > 0) {
                                                    intCntr = 0;
                                                    for (var iRow = 0; iRow <= vLibraryDeviceFowstPrinter.d.Results.Rows.length - 1; iRow++) {
                                                        var vLibraryDeviceFowstPrinterOffline = ImportExportData_offline().LibraryDeviceFowst(vLibraryDeviceFowstPrinter.d.Results.Rows[iRow].ID);

                                                        if (vLibraryDeviceFowstPrinterOffline != null) {
                                                            IsSuccess = ImportExportData_offline().SaveLibraryDeviceFowstPrinter((vLibraryDeviceFowstPrinterOffline.Rows.length > 0 ? false : true), vLibraryDeviceFowstPrinter.d.Results.Rows[iRow]);
                                                        }
                                                    }
                                                }
                                                //END LibraryDevice

                                                //Public MobizLibraryBanks
                                                var vPubLibraryBanks = JSON.parse(Transfers.ReadFile('RequestFinalUpdate' + pData.OrgID + '.zip', 'MobizLibraryBanks.json'));
                                                if (vPubLibraryBanks.d.Results.Rows.length > 0) {
                                                    intCntr = 0;
                                                    for (var iRow = 0; iRow <= vPubLibraryBanks.d.Results.Rows.length - 1; iRow++) {
                                                        var vPubLibraryBanksOffline = ImportExportData_offline().MobizLibraryBanks(vPubLibraryBanks.d.Results.Rows[iRow].ID);

                                                        if (vPubLibraryBanksOffline != null) {
                                                            IsSuccess = ImportExportData_offline().SaveMobizLibraryBanks((vPubLibraryBanksOffline.Rows.length > 0 ? false : true), vPubLibraryBanks.d.Results.Rows[iRow]);
                                                        }
                                                    }
                                                }

                                                var vMobizLibraryBankBranch = JSON.parse(Transfers.ReadFile('RequestFinalUpdate' + pData.OrgID + '.zip', 'MobizLibraryBankBranch.json'));
                                                if (vMobizLibraryBankBranch.d.Results.Rows.length > 0) {
                                                    intCntr = 0;
                                                    for (var iRow = 0; iRow <= vMobizLibraryBankBranch.d.Results.Rows.length - 1; iRow++) {
                                                        var vMobizLibraryBankBranchOffline = ImportExportData_offline().MobizLibraryBankBranch(vMobizLibraryBankBranch.d.Results.Rows[iRow].ID);

                                                        if (vMobizLibraryBankBranchOffline != null) {
                                                            IsSuccess = ImportExportData_offline().SaveMobizLibraryBankBranch((vMobizLibraryBankBranchOffline.Rows.length > 0 ? false : true), vMobizLibraryBankBranch.d.Results.Rows[iRow]);
                                                        }
                                                    }
                                                }
                                                //END MobizLibraryBanks

                                                //Public MobizLibraryCountries
                                                var vPubLibraryCountries = JSON.parse(Transfers.ReadFile('RequestFinalUpdate' + pData.OrgID + '.zip', 'MobizLibraryCountries.json'));
                                                if (vPubLibraryCountries.d.Results.Rows.length > 0) {
                                                    intCntr = 0;
                                                    for (var iRow = 0; iRow <= vPubLibraryCountries.d.Results.Rows.length - 1; iRow++) {
                                                        var vPubLibraryCountriesOffline = ImportExportData_offline().MobizLibraryCountries(vPubLibraryCountries.d.Results.Rows[iRow].ID);

                                                        if (vPubLibraryCountriesOffline != null) {
                                                            IsSuccess = ImportExportData_offline().SaveMobizLibraryCountries((vPubLibraryCountriesOffline.Rows.length > 0 ? false : true), vPubLibraryCountries.d.Results.Rows[iRow]);
                                                        }
                                                    }
                                                }
                                                //END MobizLibraryCountries

                                                //Public MobizLibraryCities
                                                var vPubLibraryCities = JSON.parse(Transfers.ReadFile('RequestFinalUpdate' + pData.OrgID + '.zip', 'MobizLibraryCities.json'));
                                                if (vPubLibraryCities.d.Results.Rows.length > 0) {
                                                    intCntr = 0;
                                                    for (var iRow = 0; iRow <= vPubLibraryCities.d.Results.Rows.length - 1; iRow++) {
                                                        var vPubLibraryCitiesOffline = ImportExportData_offline().MobizLibraryCities(vPubLibraryCities.d.Results.Rows[iRow].ID);

                                                        if (vPubLibraryCitiesOffline != null) {
                                                            IsSuccess = ImportExportData_offline().SaveMobizLibraryCities((vPubLibraryCitiesOffline.Rows.length > 0 ? false : true), vPubLibraryCities.d.Results.Rows[iRow]);
                                                        }
                                                    }
                                                }
                                                //END MobizLibraryCities

                                                //Public MobizLibraryAreaCodes
                                                var vPubLibraryAreaCodes = JSON.parse(Transfers.ReadFile('RequestFinalUpdate' + pData.OrgID + '.zip', 'MobizLibraryAreaCodes.json'));
                                                if (vPubLibraryAreaCodes.d.Results.Rows.length > 0) {
                                                    intCntr = 0;
                                                    for (var iRow = 0; iRow <= vPubLibraryAreaCodes.d.Results.Rows.length - 1; iRow++) {
                                                        var vPubLibraryAreaCodesOffline = ImportExportData_offline().MobizLibraryAreaCodes(vPubLibraryAreaCodes.d.Results.Rows[iRow].ID);

                                                        if (vPubLibraryAreaCodesOffline != null) {
                                                            IsSuccess = ImportExportData_offline().SaveMobizLibraryAreaCodes((vPubLibraryAreaCodesOffline.Rows.length > 0 ? false : true), vPubLibraryAreaCodes.d.Results.Rows[iRow]);
                                                        }
                                                    }
                                                }
                                                //END MobizLibraryAreaCodes

                                                //Public MobizLibraryTelcos
                                                var vPubLibraryTelcos = JSON.parse(Transfers.ReadFile('RequestFinalUpdate' + pData.OrgID + '.zip', 'MobizLibraryTelcos.json'));
                                                if (vPubLibraryTelcos.d.Results.Rows.length > 0) {
                                                    intCntr = 0;
                                                    for (var iRow = 0; iRow <= vPubLibraryTelcos.d.Results.Rows.length - 1; iRow++) {
                                                        var vPubLibraryTelcosOffline = ImportExportData_offline().MobizLibraryTelcos(vPubLibraryTelcos.d.Results.Rows[iRow].ID);

                                                        if (vPubLibraryTelcosOffline != null) {
                                                            IsSuccess = ImportExportData_offline().SaveMobizLibraryTelcos((vPubLibraryTelcosOffline.Rows.length > 0 ? false : true), vPubLibraryTelcos.d.Results.Rows[iRow]);
                                                        }
                                                    }
                                                }
                                                //END MobizLibraryTelcos

                                                //Public MobizLibraryTelcoPrefixes
                                                var vPubLibraryTelcoPrefixes = JSON.parse(Transfers.ReadFile('RequestFinalUpdate' + pData.OrgID + '.zip', 'MobizLibraryTelcoPrefixes.json'));
                                                if (vPubLibraryTelcoPrefixes.d.Results.Rows.length > 0) {
                                                    intCntr = 0;
                                                    for (var iRow = 0; iRow <= vPubLibraryTelcoPrefixes.d.Results.Rows.length - 1; iRow++) {
                                                        var vPubLibraryTelcoPrefixesOffline = ImportExportData_offline().MobizLibraryTelcoPrefixes(vPubLibraryTelcoPrefixes.d.Results.Rows[iRow].Id);

                                                        if (vPubLibraryTelcoPrefixesOffline != null) {
                                                            IsSuccess = ImportExportData_offline().SaveMobizLibraryTelcoPrefixes((vPubLibraryTelcoPrefixesOffline.Rows.length > 0 ? false : true), vPubLibraryTelcoPrefixes.d.Results.Rows[iRow]);
                                                        }
                                                    }
                                                }
                                                //END MobizLibraryTelcoPrefixes

                                                //Public MobizLibraryPIMPriceLevel
                                                var vPubLibraryPIMPriceLevel = JSON.parse(Transfers.ReadFile('RequestFinalUpdate' + pData.OrgID + '.zip', 'MobizLibraryPIMPriceLevel.json'));
                                                if (vPubLibraryPIMPriceLevel.d.Results.Rows.length > 0) {
                                                    intCntr = 0;
                                                    for (var iRow = 0; iRow <= vPubLibraryPIMPriceLevel.d.Results.Rows.length - 1; iRow++) {
                                                        var vPubLibraryPIMPriceLevelOffline = ImportExportData_offline().MobizLibraryPIMPriceLevel(vPubLibraryPIMPriceLevel.d.Results.Rows[iRow].ID);

                                                        if (vPubLibraryPIMPriceLevelOffline != null) {
                                                            IsSuccess = ImportExportData_offline().SaveMobizLibraryPIMPriceLevel((vPubLibraryPIMPriceLevelOffline.Rows.length > 0 ? false : true), vPubLibraryPIMPriceLevel.d.Results.Rows[iRow]);
                                                        }
                                                    }
                                                }
                                                //END MobizLibraryPIMPriceLevel

                                                //Public MobizLibraryPIMMeasure
                                                var vPubLibraryPIMMeasure = JSON.parse(Transfers.ReadFile('RequestFinalUpdate' + pData.OrgID + '.zip', 'MobizLibraryPIMMeasure.json'));
                                                if (vPubLibraryPIMMeasure.d.Results.Rows.length > 0) {
                                                    intCntr = 0;
                                                    for (var iRow = 0; iRow <= vPubLibraryPIMMeasure.d.Results.Rows.length - 1; iRow++) {
                                                        var vPubLibraryPIMMeasureOffline = ImportExportData_offline().MobizLibraryPIMMeasure(vPubLibraryPIMMeasure.d.Results.Rows[iRow].ID);

                                                        if (vPubLibraryPIMMeasureOffline != null) {
                                                            IsSuccess = ImportExportData_offline().SaveMobizLibraryPIMMeasure((vPubLibraryPIMMeasureOffline.Rows.length > 0 ? false : true), vPubLibraryPIMMeasure.d.Results.Rows[iRow]);
                                                        }
                                                    }
                                                }
                                                //END MobizLibraryPIMMeasure

                                                //Public LibraryPIMMeasureSign
                                                var vPubLibraryPIMMeasureSign = JSON.parse(Transfers.ReadFile('RequestFinalUpdate' + pData.OrgID + '.zip', 'MobizLibraryPIMMeasureSign.json'));
                                                if (vPubLibraryPIMMeasureSign.d.Results.Rows.length > 0) {
                                                    intCntr = 0;
                                                    for (var iRow = 0; iRow <= vPubLibraryPIMMeasureSign.d.Results.Rows.length - 1; iRow++) {
                                                        var vPubLibraryPIMMeasureSignOffline = ImportExportData_offline().MobizLibraryPIMMeasureSign(vPubLibraryPIMMeasureSign.d.Results.Rows[iRow].ID);

                                                        if (vPubLibraryPIMMeasureSignOffline != null) {
                                                            IsSuccess = ImportExportData_offline().SaveMobizLibraryPIMMeasureSign((vPubLibraryPIMMeasureSignOffline.Rows.length > 0 ? false : true), vPubLibraryPIMMeasureSign.d.Results.Rows[iRow]);
                                                        }
                                                    }
                                                }
                                                //END LibraryPIMMeasureSign

                                                //Public MobizLibraryPeoplePreNominalSuffix
                                                var vPubLibraryPeoplePreNominalSuffix = JSON.parse(Transfers.ReadFile('RequestFinalUpdate' + pData.OrgID + '.zip', 'MobizLibraryPeoplePreNominalSuffix.json'));
                                                if (vPubLibraryPeoplePreNominalSuffix.d.Results.Rows.length > 0) {
                                                    intCntr = 0;
                                                    for (var iRow = 0; iRow <= vPubLibraryPeoplePreNominalSuffix.d.Results.Rows.length - 1; iRow++) {
                                                        var vPubLibraryPeoplePreNominalSuffixOffline = ImportExportData_offline().MobizLibraryPeoplePreNominalSuffix(vPubLibraryPeoplePreNominalSuffix.d.Results.Rows[iRow].ID);

                                                        if (vPubLibraryPeoplePreNominalSuffixOffline != null) {
                                                            IsSuccess = ImportExportData_offline().SaveMobizLibraryPeoplePreNominalSuffix((vPubLibraryPeoplePreNominalSuffixOffline.Rows.length > 0 ? false : true), vPubLibraryPeoplePreNominalSuffix.d.Results.Rows[iRow]);
                                                        }
                                                    }
                                                }
                                                //END MobizLibraryPeoplePreNominalSuffix

                                                //Public MobizLibraryPeople
                                                var vPubLibraryPeople = JSON.parse(Transfers.ReadFile('RequestFinalUpdate' + pData.OrgID + '.zip', 'MobizLibraryPeople.json'));
                                                if (vPubLibraryPeople.d.Results.Rows.length > 0) {
                                                    intCntr = 0;
                                                    for (var iRow = 0; iRow <= vPubLibraryPeople.d.Results.Rows.length - 1; iRow++) {
                                                        var vPubLibraryPeopleOffline = ImportExportData_offline().MobizLibraryPeople(vPubLibraryPeople.d.Results.Rows[iRow].ID);

                                                        if (vPubLibraryPeopleOffline != null) {
                                                            IsSuccess = ImportExportData_offline().SaveMobizLibraryPeople((vPubLibraryPeopleOffline.Rows.length > 0 ? false : true), vPubLibraryPeople.d.Results.Rows[iRow]);
                                                        }
                                                    }
                                                }

                                                var vMobizLibraryPeopleUserAccount = JSON.parse(Transfers.ReadFile('RequestFinalUpdate' + pData.OrgID + '.zip', 'MobizLibraryPeopleUserAccount.json'));
                                                if (vMobizLibraryPeopleUserAccount.d.Results.Rows.length > 0) {
                                                    intCntr = 0;
                                                    for (var iRow = 0; iRow <= vMobizLibraryPeopleUserAccount.d.Results.Rows.length - 1; iRow++) {
                                                        var vMobizLibraryPeopleUserAccountOffline = ImportExportData_offline().MobizLibraryPeopleUserAccount(vMobizLibraryPeopleUserAccount.d.Results.Rows[iRow].PeopleID);

                                                        if (vMobizLibraryPeopleUserAccountOffline != null) {
                                                            IsSuccess = ImportExportData_offline().SaveMobizLibraryPeopleUserAccount((vMobizLibraryPeopleUserAccountOffline.Rows.length > 0 ? false : true), vMobizLibraryPeopleUserAccount.d.Results.Rows[iRow]);
                                                        }
                                                    }
                                                }

                                                var vMobizLibraryPeopleOrganization = JSON.parse(Transfers.ReadFile('RequestFinalUpdate' + pData.OrgID + '.zip', 'MobizLibraryPeopleOrganization.json'));
                                                if (vMobizLibraryPeopleOrganization.d.Results.Rows.length > 0) {
                                                    intCntr = 0;
                                                    for (var iRow = 0; iRow <= vMobizLibraryPeopleOrganization.d.Results.Rows.length - 1; iRow++) {
                                                        var vMobizLibraryPeopleOrganizationOffline = ImportExportData_offline().MobizLibraryPeopleOrganization(vMobizLibraryPeopleOrganization.d.Results.Rows[iRow].ID);

                                                        if (vMobizLibraryPeopleOrganizationOffline != null) {
                                                            IsSuccess = ImportExportData_offline().SaveMobizLibraryPeopleOrganization((vMobizLibraryPeopleOrganizationOffline.Rows.length > 0 ? false : true), vMobizLibraryPeopleOrganization.d.Results.Rows[iRow]);
                                                        }
                                                    }
                                                }
                                                //END MobizLibraryPeople

                                                //Public MobizLibraryOrganization
                                                var vPubLibraryOrganization = JSON.parse(Transfers.ReadFile('RequestFinalUpdate' + pData.OrgID + '.zip', 'MobizLibraryOrganization.json'));
                                                if (vPubLibraryOrganization.d.Results.Rows.length > 0) {
                                                    intCntr = 0;
                                                    for (var iRow = 0; iRow <= vPubLibraryOrganization.d.Results.Rows.length - 1; iRow++) {
                                                        var vPubLibraryOrganizationOffline = ImportExportData_offline().MobizLibraryOrganization(vPubLibraryOrganization.d.Results.Rows[iRow].ID);

                                                        if (vPubLibraryOrganizationOffline != null) {
                                                            IsSuccess = ImportExportData_offline().SaveMobizLibraryOrganization((vPubLibraryOrganizationOffline.Rows.length > 0 ? false : true), vPubLibraryOrganization.d.Results.Rows[iRow]);
                                                        }
                                                    }
                                                }

                                                var vMobizLibraryOrganizationContactEmail = JSON.parse(Transfers.ReadFile('RequestFinalUpdate' + pData.OrgID + '.zip', 'MobizLibraryOrganizationContactEmail.json'));
                                                if (vMobizLibraryOrganizationContactEmail.d.Results.Rows.length > 0) {
                                                    intCntr = 0;
                                                    for (var iRow = 0; iRow <= vMobizLibraryOrganizationContactEmail.d.Results.Rows.length - 1; iRow++) {
                                                        var vMobizLibraryOrganizationContactEmailOffline = ImportExportData_offline().MobizLibraryOrganizationContactEmail(vMobizLibraryOrganizationContactEmail.d.Results.Rows[iRow].ID);

                                                        if (vMobizLibraryOrganizationContactEmailOffline != null) {
                                                            IsSuccess = ImportExportData_offline().SaveMobizLibraryOrganizationContactEmail((vMobizLibraryOrganizationContactEmailOffline.Rows.length > 0 ? false : true), vMobizLibraryOrganizationContactEmail.d.Results.Rows[iRow]);
                                                        }
                                                    }
                                                }

                                                var vMobizLibraryOrganizationTradeName = JSON.parse(Transfers.ReadFile('RequestFinalUpdate' + pData.OrgID + '.zip', 'MobizLibraryOrganizationTradeName.json'));
                                                if (vMobizLibraryOrganizationTradeName.d.Results.Rows.length > 0) {
                                                    intCntr = 0;
                                                    for (var iRow = 0; iRow <= vMobizLibraryOrganizationTradeName.d.Results.Rows.length - 1; iRow++) {
                                                        var vMobizLibraryOrganizationTradeNameOffline = ImportExportData_offline().MobizLibraryOrganizationTradeName(vMobizLibraryOrganizationTradeName.d.Results.Rows[iRow].ID);

                                                        if (vMobizLibraryOrganizationTradeNameOffline != null) {
                                                            IsSuccess = ImportExportData_offline().SaveMobizLibraryOrganizationTradeName((vMobizLibraryOrganizationTradeNameOffline.Rows.length > 0 ? false : true), vMobizLibraryOrganizationTradeName.d.Results.Rows[iRow]);
                                                        }
                                                    }
                                                }

                                                var vMobizLibraryOrganizationFOWST = JSON.parse(Transfers.ReadFile('RequestFinalUpdate' + pData.OrgID + '.zip', 'MobizLibraryOrganizationFOWST.json'));
                                                if (vMobizLibraryOrganizationFOWST.d.Results.Rows.length > 0) {
                                                    intCntr = 0;
                                                    for (var iRow = 0; iRow <= vMobizLibraryOrganizationFOWST.d.Results.Rows.length - 1; iRow++) {
                                                        var vMobizLibraryOrganizationFOWSTOffline = ImportExportData_offline().MobizLibraryOrganizationFOWST(vMobizLibraryOrganizationFOWST.d.Results.Rows[iRow].ID);

                                                        if (vMobizLibraryOrganizationFOWSTOffline != null) {
                                                            IsSuccess = ImportExportData_offline().SaveMobizLibraryOrganizationFOWST((vMobizLibraryOrganizationFOWSTOffline.Rows.length > 0 ? false : true), vMobizLibraryOrganizationFOWST.d.Results.Rows[iRow]);
                                                        }
                                                    }
                                                }

                                                var vMobizLibraryOrganizationFOWSTTel = JSON.parse(Transfers.ReadFile('RequestFinalUpdate' + pData.OrgID + '.zip', 'MobizLibraryOrganizationFOWSTTel.json'));
                                                if (vMobizLibraryOrganizationFOWSTTel.d.Results.Rows.length > 0) {
                                                    intCntr = 0;
                                                    for (var iRow = 0; iRow <= vMobizLibraryOrganizationFOWSTTel.d.Results.Rows.length - 1; iRow++) {
                                                        var vMobizLibraryOrganizationFOWSTTelOffline = ImportExportData_offline().MobizLibraryOrganizationFOWSTTel(vMobizLibraryOrganizationFOWSTTel.d.Results.Rows[iRow].ID);

                                                        if (vMobizLibraryOrganizationFOWSTTelOffline != null) {
                                                            IsSuccess = ImportExportData_offline().SaveMobizLibraryOrganizationFOWSTTel((vMobizLibraryOrganizationFOWSTTelOffline.Rows.length > 0 ? false : true), vMobizLibraryOrganizationFOWSTTel.d.Results.Rows[iRow]);
                                                        }
                                                    }
                                                }

                                                var vMobizLibraryOrganizationFOWSTEmail = JSON.parse(Transfers.ReadFile('RequestFinalUpdate' + pData.OrgID + '.zip', 'MobizLibraryOrganizationFOWSTEmail.json'));
                                                if (vMobizLibraryOrganizationFOWSTEmail.d.Results.Rows.length > 0) {
                                                    intCntr = 0;
                                                    for (var iRow = 0; iRow <= vMobizLibraryOrganizationFOWSTEmail.d.Results.Rows.length - 1; iRow++) {
                                                        var vMobizLibraryOrganizationFOWSTEmailOffline = ImportExportData_offline().MobizLibraryOrganizationFOWSTEmail(vMobizLibraryOrganizationFOWSTEmail.d.Results.Rows[iRow].ID);

                                                        if (vMobizLibraryOrganizationFOWSTEmailOffline != null) {
                                                            IsSuccess = ImportExportData_offline().SaveMobizLibraryOrganizationFOWSTEmail((vMobizLibraryOrganizationFOWSTEmailOffline.Rows.length > 0 ? false : true), vMobizLibraryOrganizationFOWSTEmail.d.Results.Rows[iRow]);
                                                        }
                                                    }
                                                }

                                                var vMobizLibraryOrganizationFOWSTOperationHRs = JSON.parse(Transfers.ReadFile('RequestFinalUpdate' + pData.OrgID + '.zip', 'MobizLibraryOrganizationFOWSTOperationHRs.json'));
                                                if (vMobizLibraryOrganizationFOWSTOperationHRs.d.Results.Rows.length > 0) {
                                                    intCntr = 0;
                                                    for (var iRow = 0; iRow <= vMobizLibraryOrganizationFOWSTOperationHRs.d.Results.Rows.length - 1; iRow++) {
                                                        var vMobizLibraryOrganizationFOWSTOperationHRsOffline = ImportExportData_offline().MobizLibraryOrganizationFOWSTOperationHRs(vMobizLibraryOrganizationFOWSTOperationHRs.d.Results.Rows[iRow].ID);

                                                        if (vMobizLibraryOrganizationFOWSTOperationHRsOffline != null) {
                                                            IsSuccess = ImportExportData_offline().SaveMobizLibraryOrganizationFOWSTOperationHRs((vMobizLibraryOrganizationFOWSTOperationHRsOffline.Rows.length > 0 ? false : true), vMobizLibraryOrganizationFOWSTOperationHRs.d.Results.Rows[iRow]);
                                                        }
                                                    }
                                                }

                                                var vMobizLibraryOrganizationRepresentatives = JSON.parse(Transfers.ReadFile('RequestFinalUpdate' + pData.OrgID + '.zip', 'MobizLibraryOrganizationRepresentatives.json'));
                                                if (vMobizLibraryOrganizationRepresentatives.d.Results.Rows.length > 0) {
                                                    intCntr = 0;
                                                    for (var iRow = 0; iRow <= vMobizLibraryOrganizationRepresentatives.d.Results.Rows.length - 1; iRow++) {
                                                        var vMobizLibraryOrganizationRepresentativesOffline = ImportExportData_offline().MobizLibraryOrganizationRepresentatives(vMobizLibraryOrganizationRepresentatives.d.Results.Rows[iRow].ID);

                                                        if (vMobizLibraryOrganizationRepresentativesOffline != null) {
                                                            IsSuccess = ImportExportData_offline().SaveMobizLibraryOrganizationRepresentatives((vMobizLibraryOrganizationRepresentativesOffline.Rows.length > 0 ? false : true), vMobizLibraryOrganizationRepresentatives.d.Results.Rows[iRow]);
                                                        }
                                                    }
                                                }

                                                var vMobizLibraryOrganizationRepresentativesEmail = JSON.parse(Transfers.ReadFile('RequestFinalUpdate' + pData.OrgID + '.zip', 'MobizLibraryOrganizationRepresentativesEmail.json'));
                                                if (vMobizLibraryOrganizationRepresentativesEmail.d.Results.Rows.length > 0) {
                                                    intCntr = 0;
                                                    for (var iRow = 0; iRow <= vMobizLibraryOrganizationRepresentativesEmail.d.Results.Rows.length - 1; iRow++) {
                                                        var vMobizLibraryOrganizationRepresentativesEmailOffline = ImportExportData_offline().MobizLibraryOrganizationRepresentatives(vMobizLibraryOrganizationRepresentativesEmail.d.Results.Rows[iRow].ID);

                                                        if (vMobizLibraryOrganizationRepresentativesEmailOffline != null) {
                                                            IsSuccess = ImportExportData_offline().SaveMobizLibraryOrganizationRepresentativesEmail((vMobizLibraryOrganizationRepresentativesEmailOffline.Rows.length > 0 ? false : true), vMobizLibraryOrganizationRepresentativesEmail.d.Results.Rows[iRow]);
                                                        }
                                                    }
                                                }

                                                var vMobizLibraryOrganizationRepresentativesMobile = JSON.parse(Transfers.ReadFile('RequestFinalUpdate' + pData.OrgID + '.zip', 'MobizLibraryOrganizationRepresentativesMobile.json'));
                                                if (vMobizLibraryOrganizationRepresentativesMobile.d.Results.Rows.length > 0) {
                                                    intCntr = 0;
                                                    for (var iRow = 0; iRow <= vMobizLibraryOrganizationRepresentativesMobile.d.Results.Rows.length - 1; iRow++) {
                                                        var vMobizLibraryOrganizationRepresentativesMobileOffline = ImportExportData_offline().MobizLibraryOrganizationRepresentativesMobile(vMobizLibraryOrganizationRepresentativesMobile.d.Results.Rows[iRow].ID);

                                                        if (vMobizLibraryOrganizationRepresentativesMobileOffline != null) {
                                                            IsSuccess = ImportExportData_offline().SaveMobizLibraryOrganizationRepresentativesMobile((vMobizLibraryOrganizationRepresentativesMobileOffline.Rows.length > 0 ? false : true), vMobizLibraryOrganizationRepresentativesMobile.d.Results.Rows[iRow]);
                                                        }
                                                    }
                                                }

                                                var vMobizLibraryOrganizationRepresentativesTel = JSON.parse(Transfers.ReadFile('RequestFinalUpdate' + pData.OrgID + '.zip', 'MobizLibraryOrganizationRepresentativesTel.json'));
                                                if (vMobizLibraryOrganizationRepresentativesTel.d.Results.Rows.length > 0) {
                                                    intCntr = 0;
                                                    for (var iRow = 0; iRow <= vMobizLibraryOrganizationRepresentativesTel.d.Results.Rows.length - 1; iRow++) {
                                                        var vMobizLibraryOrganizationRepresentativesTelOffline = ImportExportData_offline().MobizLibraryOrganizationRepresentativesTel(vMobizLibraryOrganizationRepresentativesTel.d.Results.Rows[iRow].ID);

                                                        if (vMobizLibraryOrganizationRepresentativesTelOffline != null) {
                                                            IsSuccess = ImportExportData_offline().SaveMobizLibraryOrganizationRepresentativesTel((vMobizLibraryOrganizationRepresentativesTelOffline.Rows.length > 0 ? false : true), vMobizLibraryOrganizationRepresentativesTel.d.Results.Rows[iRow]);
                                                        }
                                                    }
                                                }

                                                var vLibraryOrganizationCustomer = JSON.parse(Transfers.ReadFile('RequestFinalUpdate' + pData.OrgID + '.zip', 'LibraryOrganizationCustomer.json'));
                                                if (vLibraryOrganizationCustomer.d.Results.Rows.length > 0) {
                                                    intCntr = 0;
                                                    for (var iRow = 0; iRow <= vLibraryOrganizationCustomer.d.Results.Rows.length - 1; iRow++) {
                                                        var vLibraryOrganizationCustomerOffline = ImportExportData_offline().LibraryOrganizationCustomer(vLibraryOrganizationCustomer.d.Results.Rows[iRow].ID);

                                                        if (vLibraryOrganizationCustomerOffline != null) {
                                                            IsSuccess = ImportExportData_offline().SaveLibraryOrganizationCustomer((vLibraryOrganizationCustomerOffline.Rows.length > 0 ? false : true), vLibraryOrganizationCustomer.d.Results.Rows[iRow]);
                                                        }
                                                    }
                                                }

                                                var vLibraryOrganizationEmployee = JSON.parse(Transfers.ReadFile('RequestFinalUpdate' + pData.OrgID + '.zip', 'LibraryOrganizationEmployee.json'));
                                                if (vLibraryOrganizationEmployee.d.Results.Rows.length > 0) {
                                                    intCntr = 0;
                                                    for (var iRow = 0; iRow <= vLibraryOrganizationEmployee.d.Results.Rows.length - 1; iRow++) {
                                                        var vLibraryOrganizationEmployeeOffline = ImportExportData_offline().LibraryOrganizationEmployee(vLibraryOrganizationEmployee.d.Results.Rows[iRow].ID);

                                                        if (vLibraryOrganizationEmployeeOffline != null) {
                                                            IsSuccess = ImportExportData_offline().SaveLibraryOrganizationEmployee((vLibraryOrganizationEmployeeOffline.Rows.length > 0 ? false : true), vLibraryOrganizationEmployee.d.Results.Rows[iRow]);
                                                        }
                                                    }
                                                }

                                                var vLibraryOrganizationEmployeeFOWST = JSON.parse(Transfers.ReadFile('RequestFinalUpdate' + pData.OrgID + '.zip', 'LibraryOrganizationEmployeeFOWST.json'));
                                                if (vLibraryOrganizationEmployeeFOWST.d.Results.Rows.length > 0) {
                                                    intCntr = 0;
                                                    for (var iRow = 0; iRow <= vLibraryOrganizationEmployeeFOWST.d.Results.Rows.length - 1; iRow++) {
                                                        var vLibraryOrganizationEmployeeFOWSTOffline = ImportExportData_offline().LibraryOrganizationEmployeeFOWST(vLibraryOrganizationEmployeeFOWST.d.Results.Rows[iRow].ID);

                                                        if (vLibraryOrganizationEmployeeFOWSTOffline != null) {
                                                            IsSuccess = ImportExportData_offline().SaveLibraryOrganizationEmployeeFOWST((vLibraryOrganizationEmployeeFOWSTOffline.Rows.length > 0 ? false : true), vLibraryOrganizationEmployeeFOWST.d.Results.Rows[iRow]);
                                                        }
                                                    }
                                                }

                                                var vLibraryOrganizationPriceLevel = JSON.parse(Transfers.ReadFile('RequestFinalUpdate' + pData.OrgID + '.zip', 'LibraryOrganizationEmployeeFOWST.json'));
                                                if (vLibraryOrganizationPriceLevel.d.Results.Rows.length > 0) {
                                                    intCntr = 0;
                                                    for (var iRow = 0; iRow <= vLibraryOrganizationPriceLevel.d.Results.Rows.length - 1; iRow++) {
                                                        var vLibraryOrganizationPriceLevelOffline = ImportExportData_offline().LibraryOrganizationPriceLevel(vLibraryOrganizationPriceLevel.d.Results.Rows[iRow].ID);

                                                        if (vLibraryOrganizationPriceLevelOffline != null) {
                                                            IsSuccess = ImportExportData_offline().SaveLibraryOrganizationPriceLevel((vLibraryOrganizationPriceLevelOffline.Rows.length > 0 ? false : true), vLibraryOrganizationPriceLevel.d.Results.Rows[iRow]);
                                                        }
                                                    }
                                                }
                                                //END MobizLibraryOrganization

                                                //                                                //Public MobizLibraryPIMProduct
                                                //                                                var vPubLibraryPIMProduct = JSON.parse(Transfers.ReadFile('RequestFinalUpdate' + pData.OrgID + '.zip', 'MobizLibraryPIMProduct.json'));
                                                //                                                if (vPubLibraryPIMProduct.d.Results.Rows.length > 0) {
                                                //                                                    intCntr = 0;
                                                //                                                    for (var iRow = 0; iRow <= vPubLibraryPIMProduct.d.Results.Rows.length - 1; iRow++) {
                                                //                                                        var vPubLibraryPIMProductOffline = ImportExportData_offline().MobizLibraryPIMProduct(vPubLibraryPIMProduct.d.Results.Rows[iRow].ID);

                                                //                                                        if (vPubLibraryPIMProductOffline != null) {
                                                //                                                            IsSuccess = ImportExportData_offline().SaveMobizLibraryPIMProduct((vPubLibraryPIMProductOffline.Rows.length > 0 ? false : true), vPubLibraryPIMProduct.d.Results.Rows[iRow]);
                                                //                                                        }
                                                //                                                    }
                                                //                                                }

                                                //                                                var vMobizLibraryPIMProductMeasurement = JSON.parse(Transfers.ReadFile('RequestFinalUpdate' + pData.OrgID + '.zip', 'MobizLibraryPIMProductMeasurement.json'));
                                                //                                                if (vMobizLibraryPIMProductMeasurement.d.Results.Rows.length > 0) {
                                                //                                                    intCntr = 0;
                                                //                                                    for (var iRow = 0; iRow <= vMobizLibraryPIMProductMeasurement.d.Results.Rows.length - 1; iRow++) {
                                                //                                                        var vMobizLibraryPIMProductMeasurementOffline = ImportExportData_offline().MobizLibraryPIMProductMeasurement(vMobizLibraryPIMProductMeasurement.d.Results.Rows[iRow].ID);

                                                //                                                        if (vMobizLibraryPIMProductMeasurementOffline != null) {
                                                //                                                            IsSuccess = ImportExportData_offline().SaveMobizLibraryPIMProductMeasurement((vMobizLibraryPIMProductMeasurementOffline.Rows.length > 0 ? false : true), vMobizLibraryPIMProductMeasurement.d.Results.Rows[iRow]);
                                                //                                                        }
                                                //                                                    }
                                                //                                                }

                                                //                                                var vMobizLibraryPIMProductMeasurementBarcode = JSON.parse(Transfers.ReadFile('RequestFinalUpdate' + pData.OrgID + '.zip', 'MobizLibraryPIMProductMeasurementBarcode.json'));
                                                //                                                if (vMobizLibraryPIMProductMeasurementBarcode.d.Results.Rows.length > 0) {
                                                //                                                    intCntr = 0;
                                                //                                                    for (var iRow = 0; iRow <= vMobizLibraryPIMProductMeasurementBarcode.d.Results.Rows.length - 1; iRow++) {
                                                //                                                        var vMobizLibraryPIMProductMeasurementBarcodeOffline = ImportExportData_offline().MobizLibraryPIMProductMeasurementBarcode(vMobizLibraryPIMProductMeasurementBarcode.d.Results.Rows[iRow].ID);

                                                //                                                        if (vMobizLibraryPIMProductMeasurementBarcodeOffline != null) {
                                                //                                                            IsSuccess = ImportExportData_offline().SaveMobizLibraryPIMProductMeasurementBarcode((vLibraryPIMProductMeasurementBarcodeOffline.Rows.length > 0 ? false : true), vMobizLibraryPIMProductMeasurementBarcode.d.Results.Rows[iRow]);
                                                //                                                        }
                                                //                                                    }
                                                //                                                }

                                                //                                                var vMobizLibraryPIMProductPriceLevel = JSON.parse(Transfers.ReadFile('RequestFinalUpdate' + pData.OrgID + '.zip', 'MobizLibraryPIMProductPriceLevel.json'));
                                                //                                                if (vMobizLibraryPIMProductPriceLevel.d.Results.Rows.length > 0) {
                                                //                                                    intCntr = 0;
                                                //                                                    for (var iRow = 0; iRow <= vMobizLibraryPIMProductPriceLevel.d.Results.Rows.length - 1; iRow++) {
                                                //                                                        var vMobizLibraryPIMProductPriceLevelOffline = ImportExportData_offline().MobizLibraryPIMProductPriceLevel(vMobizLibraryPIMProductPriceLevel.d.Results.Rows[iRow].ID);

                                                //                                                        if (vMobizLibraryPIMProductPriceLevelOffline != null) {
                                                //                                                            IsSuccess = ImportExportData_offline().SaveMobizLibraryPIMProductPriceLevel((vMobizLibraryPIMProductPriceLevelOffline.Rows.length > 0 ? false : true), vMobizLibraryPIMProductPriceLevel.d.Results.Rows[iRow]);
                                                //                                                        }
                                                //                                                    }
                                                //                                                }
                                                //                                                //END MobizLibraryPIMProduct

                                            }
                                        });

                                    },
                                    Error: function (vDataUpload) {
                                        console.log('Error: Upload');
                                    }
                                });

                            },
                            error: function (vError) {
                                console.log('Error: MobizExport/Download');
                            }
                        });
                    },
                    error: function (vError) {
                        console.log('Error: MobizExport/RequestDataFromServer');
                    }
                });

                function SaveSystemDefaults(vPvtSysDefData, iRow) {
                    setTimeout(function () {
                        SaveSystemDefaultsNow(vPvtSysDefData.d.Results.Rows[iRow]);
                        iRow++;
                        if (iRow < 100)
                            SaveSystemDefaults(vPvtSysDefData, iRow);
                        else
                            return;
                    }, 4000)

                    //                    if (iRow > vPvtSysDefData.d.Results.Rows.length - 1) {
                    //                        return;
                    //                    }

                    //                    setInterval(SaveSystemDefaultsNow(vPvtSysDefData.d.Results.Rows[iRow]), 1000);
                    //                    SaveSystemDefaults(vPvtSysDefData, iRow + 1);

                };

                function SaveSystemDefaultsNow(vData) {

                    var vPvtSysDefDataOffline = ImportExportData_offline().getSystemDefaults(vData.ID);

                    if (vPvtSysDefDataOffline != null) {
                        IsSuccess = ImportExportData_offline().SaveSystemDefaults((vPvtSysDefDataOffline.Rows.length > 0 ? false : true), vData);
                    }

                    setInterval(this, 10000);

                };


            };

            //////     
            //////        function ValidateUniquePersonNameInfo() {

            //////            var pData = {};
            //////            pData.PeopleID = $.trim($("#txtID").val());
            //////            pData.TitleID = $.trim($("#txtTitleID").val());
            //////            pData.Surname = $.trim($("#txtSurname").val());
            //////            pData.FirstName = $.trim($("#txtFirstName").val());
            //////            pData.MiddleName = $.trim($("#txtMiddleName").val());
            //////            pData.PersonalSuffixName = $.trim($("#selPersonalSuffixName").val());
            //////            pData.DateOfBirth = $("#selDOBMonth").val() + '/' + $("#selDOBDay").val() + '/' + $("#selDOBYear").val();
            //////            pData.NickName = $.trim($("#txtNickName").val());

            //////            pData.MotherMaidenSurname = $.trim($("#txtMotherMaidenSurname").val());
            //////            pData.MotherMaidenFirstName = $.trim($("#txtMotherMaidenFirstName").val());
            //////            pData.MotherMaidenMiddleName = $.trim($("#txtMotherMaidenMiddleName").val());

            //////            pData.FullCheckName = "0";

            //////            AjaxSend({
            //////                url: "People/CheckPeopleUniqueName",
            //////                data: pData,
            //////                success: function (vData) {

            //////                    if (vData.PeopleNameData.Rows.length > 0) {
            //////                        $.mobile.changePage('#pagePeopleConflictList', { transition: 'pop', role: 'dialog' });
            //////                        LoadPeopleConflictList(vData.PeopleNameData.Rows);
            //////                    }
            //////                    else {

            //////                        SavePersonNameInfo();

            //////                    }

            //////                }
            //////            });

            //////        };


            function GetUpdateFromServer(vOrgID, vTable, vData, vRow) {

                if (vRow >= vData.Rows.length) {
                    return;
                }

                var pData = {};
                pData.OrgID = vOrgID;
                pData.Table = vTable;
                pData.ID = vData.ID;

                AjaxSend({
                    url: "MobizExportController/RequestDataFromTable",
                    data: pData,
                    success: function (vRetData) {

                        GetUpdateFromServer(vOrgID, vTable, vData, vRow + 1)

                    }
                });

            };








        });
    });