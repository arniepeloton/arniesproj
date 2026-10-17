define(['common',
    'business',
    '/Scripts3/Offline/Web.js?crc='
    ], function () {

        $('#btnSignInRedirect').click(function () {
            Android.Restart();
        });

        window.ImportExportData_offline = function () {

            this.SelectQuery = function (Tablename, ID) {

                pSql = 'SELECT ID ' +
                ',LastUpdated ' +
                ' FROM ' + (Tablename.indexOf('Mobiz') >= 0 ? Tablename.replace('Mobiz', '') : "private." + Tablename) +
                ' WHERE ID = ' + ReadyString(ID);

                try {
                    return Sql.Execute(pSql);
                }
                catch (e) {
                    pSql = 'SELECT ID ' +
                    ' FROM ' + (Tablename.indexOf('Mobiz') >= 0 ? Tablename.replace('Mobiz', '') : "private." + Tablename) +
                    ' WHERE ID = ' + ReadyString(ID);

                    return Sql.Execute(pSql);
                }
            };

            //Private System Defaults -----------------------
            //            this.getSystemDefaults = function (vID) {

            //                pSql = "SELECT ID " +
            //                ",Description " +
            //                ",DefaultValue " +
            //                " FROM SystemDefaults "

            //                if (vID != '') {
            //                    pSql = pSql + " WHERE ID = " + ReadyString(vID);
            //                }

            //                var pDT = Sql.Execute(pSql);
            //                if (pDT == undefined) return null;

            //                return pDT;

            //            }

            this.SaveSystemDefaults = function (IsNew, vData) {

                var pIU;
                var pSql;

                pIU = new InsertUpdate('private.SystemDefaults');
                pIU.Add('Description', ReadyString(vData.Description));
                pIU.Add('DefaultValue', ReadyString(vData.DefaultValue));
                pIU.Add('LastUpdated', ReadyString(vData.LastUpdated));

                if (IsNew) {
                    pIU.Add('ID', ReadyString(vData.ID));
                    pSql = pIU.GetInsert();
                } else {
                    pSql = pIU.GetUpdate('ID = ' + ReadyString(vData.ID));
                }

                Sql.ExecuteSql(pSql);

                pIU = null;

                return true;

            };
            //END Private System Defaults -----------------------

            //Public System Defaults -----------------------
            //            this.getMobizSystemDefaults = function (vID) {

            //                var pSql;

            //                pSql = "SELECT ID " +
            //                ",Description " +
            //                ",DefaultValue " +
            //                " FROM SystemDefaults "

            //                if (vID != '') {
            //                    pSql = pSql + " WHERE ID = " + ReadyString(vID);
            //                }

            //                var pDT = Sql.Execute(pSql);
            //                if (pDT == undefined) return null;

            //                return pDT;

            //            }

            this.SaveMobizSystemDefaults = function (IsNew, vData) {

                var pIU;
                var pSql;

                pIU = new InsertUpdate('SystemDefaults');
                pIU.Add('Description', ReadyString(vData.Description));
                pIU.Add('DefaultValue', ReadyString(vData.DefaultValue));
                pIU.Add('LastUpdated', ReadyString(vData.LastUpdated));

                if (IsNew) {
                    pIU.Add('ID', ReadyString(vData.ID));
                    pSql = pIU.GetInsert();
                } else {
                    pSql = pIU.GetUpdate('ID = ' + ReadyString(vData.ID));
                }

                Sql.ExecuteSql(pSql);

                return true;

            };
            //END Public System Defaults -----------------------

            //Public LibraryCountriesLanguageLabel -----------------------
            //            this.MobizLibraryCountriesLanguageLabel = function (vID) {

            //                var pSql;

            //                pSql = "SELECT ID " +
            //                ",LanguageID " +
            //                ",Label " +
            //                ",UserID " +
            //                ",LastUpdated " +
            //                " FROM LibraryCountriesLanguageLabel "

            //                if (vID != '') {
            //                    pSql = pSql + " WHERE ID = " + ReadyString(vID);
            //                }

            //                var pDT = Sql.Execute(pSql);
            //                if (pDT == undefined) return null;

            //                return pDT;

            //            };

            this.SaveMobizLibraryCountriesLanguageLabel = function (IsNew, vData) {

                var pIU;
                var pSql;

                pIU = new InsertUpdate('LibraryCountriesLanguageLabel');
                pIU.Add('LanguageID', ReadyString(vData.LanguageID));
                pIU.Add('Label', ReadyString(vData.Label));
                pIU.Add('UserID', ReadyString(vData.UserID));
                pIU.Add('LastUpdated', ReadyString(vData.LastUpdated));

                if (IsNew) {
                    pIU.Add('ID', ReadyString(vData.ID));
                    pSql = pIU.GetInsert();
                } else {
                    pSql = pIU.GetUpdate('ID = ' + ReadyString(vData.ID));
                }

                Sql.ExecuteSql(pSql);

                return true;

            };
            //END Public LibraryCountriesLanguageLabel -----------------------

            //Private LibraryDiscountSettings -----------------------
            //            this.LibraryDiscountSettings = function (vID) {

            //                var pSql;
            //                pSql = "SELECT ID " +
            //                ",DiscountType " +
            //                ",PercentDiscount " +
            //                ",UserID " +
            //                ",LastUpdated " +
            //                " FROM LibraryDiscountSettings "

            //                if (vID != '') {
            //                    pSql = pSql + " WHERE ID = " + ReadyString(vID);
            //                }

            //                var pDT = Sql.Execute(pSql);
            //                if (pDT == undefined) return null;

            //                return pDT;

            //            };

            this.SaveLibraryDiscountSettings = function (IsNew, vData) {

                var pIU;
                var pSql;

                pIU = new InsertUpdate('private.LibraryDiscountSettings');
                pIU.Add('DiscountType', ReadyString(vData.DiscountType));
                pIU.Add('PercentDiscount', CDec2(vData.PercentDiscount));
                pIU.Add('UserID', ReadyString(vData.UserID));
                pIU.Add('LastUpdated', ReadyString(vData.LastUpdated));

                if (IsNew) {
                    pIU.Add('ID', ReadyString(vData.ID));
                    pSql = pIU.GetInsert();
                } else {
                    pSql = pIU.GetUpdate('ID = ' + ReadyString(vData.ID));
                }

                Sql.ExecuteSql(pSql);

                return true;

            };
            //END Private LibraryDiscountSettings -----------------------

            //Private LibrarySecurityModule -----------------------
            //            this.LibrarySecurityModule = function (vID) {

            //                var pSql;
            //                pSql = "SELECT ID " +
            //                ",ModuleName " +
            //                ",SortOrder " +
            //                ",MainModuleID " +
            //                ",IsHeader " +
            //                ",HeaderID " +
            //                ",IsModule " +
            //                ",IsFeature " +
            //                " FROM LibrarySecurityModule "

            //                if (vID != '') {
            //                    pSql = pSql + " WHERE ID = " + ReadyString(vID);
            //                }

            //                var pDT = Sql.Execute(pSql);
            //                if (pDT == undefined) return null;

            //                return pDT;

            //            };

            this.SaveLibrarySecurityModule = function (IsNew, vData) {

                var pIU;
                var pSql;

                pIU = new InsertUpdate('private.LibrarySecurityModule');
                pIU.Add('ModuleName', ReadyString(vData.ModuleName));
                pIU.Add('SortOrder', vData.SortOrder);
                pIU.Add('MainModuleID', ReadyString(vData.MainModuleID));
                pIU.Add('IsHeader', CDec2(vData.IsHeader));
                pIU.Add('HeaderID', ReadyString(vData.HeaderID));
                pIU.Add('IsModule', CDec2(vData.IsModule));
                pIU.Add('IsFeature', CDec2(vData.IsFeature));

                if (IsNew) {
                    pIU.Add('ID', ReadyString(vData.ID));
                    pSql = pIU.GetInsert();
                } else {
                    pSql = pIU.GetUpdate('ID = ' + ReadyString(vData.ID));
                }

                Sql.ExecuteSql(pSql);

                return true;

            };
            //END Private LibrarySecurityModule -----------------------

            //Private LibrarySecurityModuleMain -----------------------
            //            this.LibrarySecurityModuleMain = function (vID) {

            //                var pSql;
            //                pSql = "SELECT ID " +
            //                ",MainModuleName " +
            //                ",SortOrder " +
            //                " FROM LibrarySecurityModuleMain "

            //                if (vID != '') {
            //                    pSql = pSql + " WHERE ID = " + ReadyString(vID);
            //                }

            //                var pDT = Sql.Execute(pSql);
            //                if (pDT == undefined) return null;

            //                return pDT;

            //            };

            this.SaveLibrarySecurityModuleMain = function (IsNew, vData) {

                var pIU;
                var pSql;

                pIU = new InsertUpdate('private.LibrarySecurityModuleMain');
                pIU.Add('MainModuleName', ReadyString(vData.ModuleName));
                pIU.Add('SortOrder', vData.SortOrder);
                pIU.Add('LastUpdated', ReadyString(vData.LastUpdated));

                if (IsNew) {
                    pIU.Add('ID', ReadyString(vData.ID));
                    pSql = pIU.GetInsert();
                } else {
                    pSql = pIU.GetUpdate('ID = ' + ReadyString(vData.ID));
                }

                Sql.ExecuteSql(pSql);

                return true;

            };
            //END Private LibrarySecurityModuleMain -----------------------

            //Private LibrarySecurityRole -----------------------
            //            this.LibrarySecurityRole = function (vID) {

            //                var pSql;
            //                pSql = "SELECT ID " +
            //                ",Code " +
            //                ",Role " +
            //                ",Description " +
            //                ",IsActive " +
            //                ",UserID " +
            //                ",LastUpdated " +
            //                " FROM LibrarySecurityRole "

            //                if (vID != '') {
            //                    pSql = pSql + " WHERE ID = " + ReadyString(vID);
            //                }

            //                var pDT = Sql.Execute(pSql);
            //                if (pDT == undefined) return null;

            //                return pDT;

            //            };

            this.SaveLibrarySecurityRole = function (IsNew, vData) {

                var pIU;
                var pSql;

                pIU = new InsertUpdate('private.LibrarySecurityRole');
                pIU.Add('Code', ReadyString(vData.Code));
                pIU.Add('Role', ReadyString(vData.Role));
                pIU.Add('Description', ReadyString(vData.Description));
                pIU.Add('IsActive', CDec2(vData.IsActive));
                pIU.Add('UserID', ReadyString(vData.UserID));
                pIU.Add('LastUpdated', ReadyString(vData.LastUpdated));

                if (IsNew) {
                    pIU.Add('ID', ReadyString(vData.ID));
                    pSql = pIU.GetInsert();
                } else {
                    pSql = pIU.GetUpdate('ID = ' + ReadyString(vData.ID));
                }

                Sql.ExecuteSql(pSql);

                return true;

            };
            //END Private LibrarySecurityRole -----------------------

            //Private LibrarySecurityRoleModules -----------------------
            //            this.LibrarySecurityRoleModules = function (vID) {

            //                var pSql;
            //                pSql = "SELECT ID " +
            //                ",RoleID " +
            //                ",ModuleID " +
            //                ",IsHasAccess " +
            //                " FROM LibrarySecurityRoleModules "

            //                if (vID != '') {
            //                    pSql = pSql + " WHERE ID = " + ReadyString(vID);
            //                }

            //                var pDT = Sql.Execute(pSql);
            //                if (pDT == undefined) return null;

            //                return pDT;

            //            };

            this.SaveLibrarySecurityRoleModules = function (IsNew, vData) {

                var pIU;
                var pSql;

                pIU = new InsertUpdate('private.LibrarySecurityRoleModules');
                pIU.Add('RoleID', ReadyString(vData.RoleID));
                pIU.Add('ModuleID', ReadyString(vData.ModuleID));
                pIU.Add('IsHasAccess', CDec2(vData.IsHasAccess));

                if (IsNew) {
                    pIU.Add('ID', ReadyString(vData.ID));
                    pSql = pIU.GetInsert();
                } else {
                    pSql = pIU.GetUpdate('ID = ' + ReadyString(vData.ID));
                }

                Sql.ExecuteSql(pSql);

                return true;

            };
            //END Private LibrarySecurityRoleModules -----------------------

            //Private LibrarySecurityRoleUser -----------------------
            //            this.LibrarySecurityRoleUser = function (vID) {

            //                var pSql;
            //                pSql = "SELECT ID " +
            //                ",RoleID " +
            //                ",PeopleID " +
            //                ",UserID " +
            //                ",LastUpdated " +
            //                " FROM LibrarySecurityRoleUser "

            //                if (vID != '') {
            //                    pSql = pSql + " WHERE ID = " + ReadyString(vID);
            //                }

            //                var pDT = Sql.Execute(pSql);
            //                if (pDT == undefined) return null;

            //                return pDT;

            //            };

            this.SaveLibrarySecurityRoleUser = function (IsNew, vData) {

                var pIU;
                var pSql;

                pIU = new InsertUpdate('private.LibrarySecurityRoleUser');
                pIU.Add('RoleID', ReadyString(vData.RoleID));
                pIU.Add('PeopleID', ReadyString(vData.PeopleID));
                pIU.Add('UserID', ReadyString(vData.UserID));
                pIU.Add('LastUpdated', ReadyString(vData.LastUpdated));

                if (IsNew) {
                    pIU.Add('ID', ReadyString(vData.ID));
                    pSql = pIU.GetInsert();
                } else {
                    pSql = pIU.GetUpdate('ID = ' + ReadyString(vData.ID));
                }

                Sql.ExecuteSql(pSql);

                return true;

            };
            //END Private LibrarySecurityRoleUser -----------------------

            //Private LibraryReceiptSettings -----------------------
            //            this.LibraryReceiptSettings = function (vID) {

            //                var pSql;
            //                pSql = "SELECT ID " +
            //                ",OrgTradeNameID " +
            //                ",PaperSize " +
            //                ",IsShowSalesRep " +
            //                ",UserID " +
            //                ",LastUpdated " +
            //                " FROM LibraryReceiptSettings "

            //                if (vID != '') {
            //                    pSql = pSql + " WHERE ID = " + ReadyString(vID);
            //                }

            //                var pDT = Sql.Execute(pSql);
            //                if (pDT == undefined) return null;

            //                return pDT;

            //            };

            this.SaveLibraryReceiptSettings = function (IsNew, vData) {

                var pIU;
                var pSql;

                pIU = new InsertUpdate('private.LibraryReceiptSettings');
                pIU.Add('OrgTradeNameID', ReadyString(vData.OrgTradeNameID));
                pIU.Add('PaperSize', ReadyString(vData.PaperSize));
                pIU.Add('IsShowSalesRep', CDec2(vData.IsShowSalesRep));
                pIU.Add('UserID', ReadyString(vData.UserID));
                pIU.Add('LastUpdated', ReadyString(vData.LastUpdated));

                if (IsNew) {
                    pIU.Add('ID', ReadyString(vData.ID));
                    pSql = pIU.GetInsert();
                } else {
                    pSql = pIU.GetUpdate('ID = ' + ReadyString(vData.ID));
                }

                Sql.ExecuteSql(pSql);

                return true;

            };
            //END Private LibraryReceiptSettings -----------------------

            //Private LibraryReceiptSettingsFowst -----------------------
            //            this.LibraryReceiptSettingsFowst = function (vID) {

            //                var pSql;
            //                pSql = "SELECT ID " +
            //                ",ReceiptSettingsID " +
            //                ",FowstID " +
            //                " FROM LibraryReceiptSettingsFowst "

            //                if (vID != '') {
            //                    pSql = pSql + " WHERE ID = " + ReadyString(vID);
            //                }

            //                var pDT = Sql.Execute(pSql);
            //                if (pDT == undefined) return null;

            //                return pDT;

            //            };

            this.SaveLibraryReceiptSettingsFowst = function (IsNew, vData) {

                var pIU;
                var pSql;

                pIU = new InsertUpdate('private.LibraryReceiptSettingsFowst');
                pIU.Add('ReceiptSettingsID', ReadyString(vData.ReceiptSettingsID));
                pIU.Add('FowstID', ReadyString(vData.FowstID));

                if (IsNew) {
                    pIU.Add('ID', ReadyString(vData.ID));
                    pSql = pIU.GetInsert();
                } else {
                    pSql = pIU.GetUpdate('ID = ' + ReadyString(vData.ID));
                }

                Sql.ExecuteSql(pSql);

                return true;

            };
            //END Private LibraryReceiptSettingsFowst -----------------------

            //Private LibraryReceiptSettingsMessage -----------------------
            //            this.LibraryReceiptSettingsMessage = function (vID) {

            //                var pSql;
            //                pSql = "SELECT ID " +
            //                ",ReceiptSettingsID " +
            //                ",MessageFooter " +
            //                ",SortOrder " +
            //                " FROM LibraryReceiptSettingsMessage "

            //                if (vID != '') {
            //                    pSql = pSql + " WHERE ID = " + ReadyString(vID);
            //                }

            //                var pDT = Sql.Execute(pSql);
            //                if (pDT == undefined) return null;

            //                return pDT;

            //            };

            this.SaveLibraryReceiptSettingsMessage = function (IsNew, vData) {

                var pIU;
                var pSql;

                pIU = new InsertUpdate('private.LibraryReceiptSettingsMessage');
                pIU.Add('ReceiptSettingsID', ReadyString(vData.ReceiptSettingsID));
                pIU.Add('MessageFooter', ReadyString(vData.MessageFooter));
                pIU.Add('SortOrder', CDec2(vData.SortOrder));

                if (IsNew) {
                    pIU.Add('ID', ReadyString(vData.ID));
                    pSql = pIU.GetInsert();
                } else {
                    pSql = pIU.GetUpdate('ID = ' + ReadyString(vData.ID));
                }

                Sql.ExecuteSql(pSql);

                return true;

            };
            //END Private LibraryReceiptSettingsMessage -----------------------

            //Private LibraryDevice -----------------------
            //            this.LibraryDevice = function (vID) {

            //                var pSql;
            //                pSql = "SELECT ID " +
            //                ",PhoneName " +
            //                ",PhoneModel " +
            //                ",IMEI " +
            //                ",UUID " +
            //                ",OS " +
            //                ",DeviceNo " +
            //                ",IsActive " +
            //                ",UserID " +
            //                ",DateEnrolled " +
            //                ",LastUpdated " +
            //                ",DevicePolicyProfileID " +
            //                ",DevicePolicyLastUpdate " +
            //                ",DevicePolicyLastDownload " +
            //                ",SDKVersion " +
            //                ",UserFullName " +
            //                ",IsCertificateValid " +
            //                ",ToWipeOut " +
            //                " FROM LibraryDevice "

            //                if (vID != '') {
            //                    pSql = pSql + " WHERE ID = " + ReadyString(vID);
            //                }

            //                var pDT = Sql.Execute(pSql);
            //                if (pDT == undefined) return null;

            //                return pDT;

            //            };

            this.SaveLibraryDevice = function (IsNew, vData) {

                var pIU;
                var pSql;

                pIU = new InsertUpdate('private.LibraryDevice');
                pIU.Add('PhoneName', ReadyString(vData.PhoneName));
                pIU.Add('PhoneModel', ReadyString(vData.PhoneModel));
                pIU.Add('IMEI', ReadyString(vData.IMEI));
                pIU.Add('UUID', ReadyString(vData.UUID));
                pIU.Add('OS', ReadyString(vData.OS));
                pIU.Add('DeviceNo', vData.DeviceNo);
                pIU.Add('IsActive', CDec2(vData.IsActive));
                pIU.Add('UserID', ReadyString(vData.UserID));
                pIU.Add('DateEnrolled', ReadyString(vData.DateEnrolled));
                pIU.Add('LastUpdated', ReadyString(vData.LastUpdated));
                pIU.Add('DevicePolicyProfileID', ReadyString(vData.DevicePolicyProfileID));
                pIU.Add('DevicePolicyLastUpdate', ReadyString(vData.DevicePolicyLastUpdate));
                pIU.Add('DevicePolicyLastDownload', ReadyString(vData.DevicePolicyLastDownload));
                pIU.Add('SDKVersion', ReadyString(vData.SDKVersion));
                pIU.Add('UserFullName', ReadyString(vData.UserFullName));
                pIU.Add('IsCertificateValid', CDec2(vData.IsCertificateValid));
                pIU.Add('ToWipeOut', CDec2(vData.ToWipeOut));

                if (IsNew) {
                    pIU.Add('ID', ReadyString(vData.ID));
                    pSql = pIU.GetInsert();
                } else {
                    pSql = pIU.GetUpdate('ID = ' + ReadyString(vData.ID));
                }

                Sql.ExecuteSql(pSql);

                return true;

            };
            //END Private LibraryDevice -----------------------

            //Private LibraryDeviceFowst -----------------------
            //            this.LibraryDeviceFowst = function (vID) {

            //                var pSql;
            //                pSql = "SELECT ID " +
            //                ",DeviceID " +
            //                ",FowstID " +
            //                ",POSRegistrationNo " +
            //                ",TerminalNo " +
            //                ",BegBalDate " +
            //                ",BegBalance " +
            //                ",BegBalSetDate " +
            //                ",POSLastORNo " +
            //                ",POSLastSRVNo " +
            //                ",POSLastRefNo " +
            //                ",SMMachineNo " +
            //                " FROM LibraryDeviceFowst "

            //                if (vID != '') {
            //                    pSql = pSql + " WHERE ID = " + ReadyString(vID);
            //                }

            //                var pDT = Sql.Execute(pSql);
            //                if (pDT == undefined) return null;

            //                return pDT;

            //            };

            this.SaveLibraryDeviceFowst = function (IsNew, vData) {

                var pIU;
                var pSql;

                pIU = new InsertUpdate('private.LibraryDeviceFowst');
                pIU.Add('DeviceID', ReadyString(vData.DeviceID));
                pIU.Add('FowstID', ReadyString(vData.FowstID));
                pIU.Add('POSRegistrationNo', ReadyString(vData.POSRegistrationNo));
                pIU.Add('TerminalNo', vData.TerminalNo || 0);
                pIU.Add('BegBalDate', ReadyString(vData.BegBalDate || ''));
                pIU.Add('BegBalance', vData.BegBalance || 0);
                pIU.Add('BegBalSetDate', ReadyString(vData.BegBalSetDate || ''));
                pIU.Add('POSLastORNo', vData.POSLastORNo || 0);
                pIU.Add('POSLastSRVNo', vData.POSLastSRVNo || 0);
                pIU.Add('POSLastRefNo', vData.POSLastRefNo || 0);
                pIU.Add('SMMachineNo', ReadyString(vData.SMMachineNo || 0));

                if (IsNew) {
                    pIU.Add('ID', ReadyString(vData.ID));
                    pSql = pIU.GetInsert();
                } else {
                    pSql = pIU.GetUpdate('ID = ' + ReadyString(vData.ID));
                }

                Sql.ExecuteSql(pSql);

                return true;

            };
            //END Private LibraryDeviceFowst -----------------------

            //Private LibraryDeviceFowstPrinter -----------------------
            //            this.LibraryDeviceFowstPrinter = function (vID) {

            //                var pSql;
            //                pSql = "SELECT ID " +
            //                ",DeviceID " +
            //                ",FowstID " +
            //                ",PrinterIPAddress " +
            //                " FROM LibraryDeviceFowstPrinter "

            //                if (vID != '') {
            //                    pSql = pSql + " WHERE ID = " + ReadyString(vID);
            //                }

            //                var pDT = Sql.Execute(pSql);
            //                if (pDT == undefined) return null;

            //                return pDT;

            //            };

            this.SaveLibraryDeviceFowstPrinter = function (IsNew, vData) {

                var pIU;
                var pSql;

                pIU = new InsertUpdate('private.LibraryDeviceFowstPrinter');
                pIU.Add('DeviceID', ReadyString(vData.DeviceID));
                pIU.Add('IsActive', CDec2(vData.IsActive));
                pIU.Add('UserID', ReadyString(vData.UserID));
                pIU.Add('LastUpdated', ReadyString(vData.LastUpdated));
                pIU.Add('PrinterFowstID', ReadyString(vData.PrinterFowstID));

                if (IsNew) {
                    pIU.Add('ID', ReadyString(vData.ID));
                    pSql = pIU.GetInsert();
                } else {
                    pSql = pIU.GetUpdate('ID = ' + ReadyString(vData.ID));
                }

                Sql.ExecuteSql(pSql);

                return true;

            };

            this.SaveLibraryDevicePrinterFowst = function (IsNew, vData) {

                var pIU;
                var pSql;

                pIU = new InsertUpdate('private.LibraryDevicePrinterFowst');
                pIU.Add('FowstID', ReadyString(vData.FowstID));
                pIU.Add('PrinterID', ReadyString(vData.DeviceID));
                pIU.Add('PrinterIPAddress', ReadyString(vData.PrinterIPAddress));
                pIU.Add('PrinterName', ReadyString(vData.DeviceID));

                if (IsNew) {
                    pIU.Add('ID', ReadyString(vData.ID));
                    pSql = pIU.GetInsert();
                } else {
                    pSql = pIU.GetUpdate('ID = ' + ReadyString(vData.ID));
                }

                Sql.ExecuteSql(pSql);

                return true;

            };

            this.SaveLibraryDevicePrinter = function (IsNew, vData) {

                var pIU;
                var pSql;

                pIU = new InsertUpdate('private.LibraryDevicePrinter');
                pIU.Add('PrinterType', ReadyString(vData.PrinterType));
                pIU.Add('PrinterRole', ReadyString(vData.PrinterRole));
                pIU.Add('IsActive', ReadyString(vData.IsActive));
                pIU.Add('UserID', ReadyString(vData.UserID));
                pIU.Add('LastUpdated', ReadyString(vData.LastUpdated));

                if (IsNew) {
                    pIU.Add('ID', ReadyString(vData.ID));
                    pSql = pIU.GetInsert();
                } else {
                    pSql = pIU.GetUpdate('ID = ' + ReadyString(vData.ID));
                }

                Sql.ExecuteSql(pSql);

                return true;

            };


            this.SaveLibraryDevicePrinterCategory = function (IsNew, vData) {

                var pIU;
                var pSql;

                pIU = new InsertUpdate('private.LibraryDevicePrinterCategory');
                pIU.Add('PrinterID', ReadyString(vData.PrinterID));
                pIU.Add('CategoryName', ReadyString(vData.CategoryName));

                if (IsNew) {
                    pIU.Add('ID', ReadyString(vData.ID));
                    pSql = pIU.GetInsert();
                } else {
                    pSql = pIU.GetUpdate('ID = ' + ReadyString(vData.ID));
                }

                Sql.ExecuteSql(pSql);

                return true;

            };

            //END Private LibraryDeviceFowstPrinter -----------------------

            //Public MobizLibraryBanks -----------------------
            //            this.MobizLibraryBanks = function (vID) {

            //                var pSql;
            //                pSql = "SELECT ID " +
            //                ",BankCode " +
            //                ",BankName " +
            //                ",IsActive " +
            //                ",UserID " +
            //                ",LastUpdated " +
            //                " FROM Mobiz.LibraryBanks "

            //                if (vID != '') {
            //                    pSql = pSql + " WHERE ID = " + ReadyString(vID);
            //                }

            //                var pDT = Sql.Execute(pSql);
            //                if (pDT == undefined) return null;

            //                return pDT;

            //            };

            this.SaveMobizLibraryBanks = function (IsNew, vData) {

                var pIU;
                var pSql;

                pIU = new InsertUpdate('LibraryBanks');
                pIU.Add('BankCode', ReadyString(vData.BankCode));
                pIU.Add('BankName', ReadyString(vData.BankName));
                pIU.Add('IsActive', CDec2(vData.IsActive));
                pIU.Add('UserID', ReadyString(vData.UserID));
                pIU.Add('LastUpdated', ReadyString(vData.LastUpdated));

                if (IsNew) {
                    pIU.Add('ID', ReadyString(vData.ID));
                    pSql = pIU.GetInsert();
                } else {
                    pSql = pIU.GetUpdate('ID = ' + ReadyString(vData.ID));
                }

                Sql.ExecuteSql(pSql);

                return true;

            };
            //END Public MobizLibraryBanks -----------------------

            //Public MobizLibraryBankBranch -----------------------
            //            this.MobizLibraryBankBranch = function (vID) {

            //                var pSql;
            //                pSql = "SELECT ID " +
            //                ",BankID " +
            //                ",BranchName " +
            //                ",Address " +
            //                ",UserID " +
            //                ",LastUpdated " +
            //                " FROM Mobiz.LibraryBankBranch "

            //                if (vID != '') {
            //                    pSql = pSql + " WHERE ID = " + ReadyString(vID);
            //                }

            //                var pDT = Sql.Execute(pSql);
            //                if (pDT == undefined) return null;

            //                return pDT;

            //            };

            this.SaveMobizLibraryBankBranch = function (IsNew, vData) {

                var pIU;
                var pSql;

                pIU = new InsertUpdate('LibraryBankBranch');
                pIU.Add('BankID', ReadyString(vData.BankID));
                pIU.Add('BranchName', ReadyString(vData.BranchName));
                pIU.Add('Address', ReadyString(vData.Address));
                pIU.Add('UserID', ReadyString(vData.UserID));
                pIU.Add('LastUpdated', ReadyString(vData.LastUpdated));

                if (IsNew) {
                    pIU.Add('ID', ReadyString(vData.ID));
                    pSql = pIU.GetInsert();
                } else {
                    pSql = pIU.GetUpdate('ID = ' + ReadyString(vData.ID));
                }

                Sql.ExecuteSql(pSql);

                return true;

            };
            //END Public MobizLibraryBankBranch -----------------------

            //Public MobizLibraryPeoplePreNominalSuffix -----------------------
            //            this.MobizLibraryPeoplePreNominalSuffix = function (vID) {

            //                var pSql;
            //                pSql = "SELECT ID " +
            //                ",Code " +
            //                ",Description " +
            //                ",Category " +
            //                ",Tags " +
            //                ",SortOrder " +
            //                " FROM Mobiz.LibraryPeoplePreNominalSuffix "

            //                if (vID != '') {
            //                    pSql = pSql + " WHERE ID = " + ReadyString(vID);
            //                }

            //                var pDT = Sql.Execute(pSql);
            //                if (pDT == undefined) return null;

            //                return pDT;

            //            };

            this.SaveMobizLibraryPeoplePreNominalSuffix = function (IsNew, vData) {

                var pIU;
                var pSql;

                pIU = new InsertUpdate('LibraryPeoplePreNominalSuffix');
                pIU.Add('Code', ReadyString(vData.Code));
                pIU.Add('Description', ReadyString(vData.Description));
                pIU.Add('Category', ReadyString(vData.Category));
                pIU.Add('Tags', ReadyString(vData.Tags));
                pIU.Add('SortOrder', vData.SortOrder);
                pIU.Add('LastUpdated', ReadyString(vData.LastUpdated));

                if (IsNew) {
                    pIU.Add('ID', ReadyString(vData.ID));
                    pSql = pIU.GetInsert();
                } else {
                    pSql = pIU.GetUpdate('ID = ' + ReadyString(vData.ID));
                }

                Sql.ExecuteSql(pSql);

                return true;

            };
            //END Public MobizLibraryPeoplePreNominalSuffix -----------------------

            //Public MobizLibraryPeople -----------------------
            //            this.MobizLibraryPeople = function (vID) {

            //                var pSql;
            //                pSql = "SELECT ID " +
            //                ",TitleID " +
            //                ",Surname " +
            //                ",FirstName " +
            //                ",MiddleName " +
            //                ",PersonalSuffixName " +
            //                ",NickName " +
            //                ",DateOfBirth " +
            //                ",MotherMaidenSurname " +
            //                ",MotherMaidenFirstName " +
            //                ",MotherMaidenMiddleName " +
            //                ",PlaceOfBirthCityID " +
            //                ",PlaceOfBirthCountryID " +
            //                ",Gender " +
            //                ",CitizenshipID " +
            //                ",Height " +
            //                ",HeightMeasurement " +
            //                ",Weight " +
            //                ",WeightMeasurement " +
            //                ",CivilStatus " +
            //                ",ResidenceCertNo " +
            //                ",ResidenceCertIssuedAtID " +
            //                ",ResidenceCertIssuedOn " +
            //                ",SSSNo " +
            //                ",PhilHealthNo " +
            //                ",TIN " +
            //                ",HDMFNo " +
            //                ",NBINo " +
            //                ",PassportNo " +
            //                ",PassportIssuedAtID " +
            //                ",PassportIssuedOn " +
            //                ",PassportExpirationDate " +
            //                ",UserID " +
            //                ",IsTermAgreed " +
            //                ",IsCertifiedCorrect " +
            //                ",IsFirstPartySignUp " +
            //                ",LastUpdated " +
            //                " FROM Mobiz.LibraryPeople "

            //                if (vID != '') {
            //                    pSql = pSql + " WHERE ID = " + ReadyString(vID);
            //                }

            //                var pDT = Sql.Execute(pSql);
            //                if (pDT == undefined) return null;

            //                return pDT;

            //            };

            this.SaveMobizLibraryPeople = function (IsNew, vData) {

                var pIU;
                var pSql;

                pIU = new InsertUpdate('LibraryPeople');
                pIU.Add('TitleID', ReadyString(vData.TitleID));
                pIU.Add('Surname', ReadyString(vData.Surname));
                pIU.Add('FirstName', ReadyString(vData.FirstName));
                pIU.Add('MiddleName', ReadyString(vData.MiddleName));
                pIU.Add('PersonalSuffixName', ReadyString(vData.PersonalSuffixName));
                pIU.Add('NickName', ReadyString(vData.NickName));
                pIU.Add('DateOfBirth', ReadyString(vData.DateOfBirth));
                pIU.Add('MotherMaidenSurname', ReadyString(vData.MotherMaidenSurname));
                pIU.Add('MotherMaidenFirstName', ReadyString(vData.MotherMaidenFirstName));
                pIU.Add('MotherMaidenMiddleName', ReadyString(vData.MotherMaidenMiddleName));
                pIU.Add('PlaceOfBirthCityID', ReadyString(vData.PlaceOfBirthCityID));
                pIU.Add('PlaceOfBirthCountryID', ReadyString(vData.PlaceOfBirthCountryID));
                pIU.Add('Gender', ReadyString(vData.Gender));
                pIU.Add('CitizenshipID', ReadyString(vData.CitizenshipID));
                pIU.Add('Height', CDec2(vData.Height));
                pIU.Add('HeightMeasurement', ReadyString(vData.HeightMeasurement));
                pIU.Add('Weight', CDec2(vData.Weight));
                pIU.Add('WeightMeasurement', ReadyString(vData.WeightMeasurement));
                pIU.Add('CivilStatus', ReadyString(vData.CivilStatus));
                pIU.Add('ResidenceCertNo', ReadyString(vData.ResidenceCertNo));
                pIU.Add('ResidenceCertIssuedAtID', ReadyString(vData.ResidenceCertIssuedAtID));
                pIU.Add('ResidenceCertIssuedOn', ReadyString(vData.ResidenceCertIssuedOn));
                pIU.Add('SSSNo', ReadyString(vData.SSSNo));
                pIU.Add('PhilHealthNo', ReadyString(vData.PhilHealthNo));
                pIU.Add('TIN', ReadyString(vData.TIN));
                pIU.Add('HDMFNo', ReadyString(vData.HDMFNo));
                pIU.Add('NBINo', ReadyString(vData.NBINo));
                pIU.Add('PassportNo', ReadyString(vData.PassportNo));
                pIU.Add('PassportIssuedAtID', ReadyString(vData.PassportIssuedAtID));
                pIU.Add('PassportIssuedOn', ReadyString(vData.PassportIssuedOn));
                pIU.Add('PassportExpirationDate', ReadyString(vData.PassportExpirationDate));
                pIU.Add('UserID', ReadyString(vData.UserID));
                pIU.Add('IsTermAgreed', CDec2(vData.IsTermAgreed));
                pIU.Add('IsCertifiedCorrect', CDec2(vData.IsCertifiedCorrect));
                pIU.Add('IsFirstPartySignUp', CDec2(vData.IsFirstPartySignUp));
                pIU.Add('LastUpdated', ReadyString(vData.LastUpdated));

                if (IsNew) {
                    pIU.Add('ID', ReadyString(vData.ID));
                    pSql = pIU.GetInsert();
                } else {
                    pSql = pIU.GetUpdate('ID = ' + ReadyString(vData.ID));
                }

                Sql.ExecuteSql(pSql);

                return true;

            };
            //END Public MobizLibraryPeople -----------------------

            //Public MobizLibraryPeopleUserAccount -----------------------
            //            this.MobizLibraryPeopleUserAccount = function (vID) {

            //                var pSql;
            //                pSql = "SELECT ID " +
            //                ",Username " +
            //                ",UserPassword " +
            //                " FROM Mobiz.LibraryPeopleUserAccount "

            //                if (vID != '') {
            //                    pSql = pSql + " WHERE ID = " + ReadyString(vID);
            //                }

            //                var pDT = Sql.Execute(pSql);
            //                if (pDT == undefined) return null;

            //                return pDT;

            //            };

            this.SaveMobizLibraryPeopleUserAccount = function (IsNew, vData) {

                var pIU;
                var pSql;

                pIU = new InsertUpdate('LibraryPeopleUserAccount');
                pIU.Add('Username', ReadyString(vData.Username));
                pIU.Add('UserPassword', ReadyString(vData.UserPassword));
                pIU.Add('PeopleID', ReadyString(vData.PeopleID));

                if (IsNew) {
                    pIU.Add('ID', ReadyString(vData.ID));
                    pSql = pIU.GetInsert();
                } else {
                    pSql = pIU.GetUpdate('ID = ' + ReadyString(vData.ID));
                }

                Sql.ExecuteSql(pSql);

                return true;

            };
            //END Public MobizLibraryPeopleUserAccount -----------------------

            //Public MobizLibraryPeopleUserAccount -----------------------
            //            this.MobizLibraryPeopleUserAccount = function (vID) {

            //                var pSql;
            //                pSql = "SELECT PeopleID " +
            //                ",Username " +
            //                ",UserPassword " +
            //                " FROM Mobiz.LibraryPeopleUserAccount "

            //                if (vID != '') {
            //                    pSql = pSql + " WHERE PeopleID = " + ReadyString(vID);
            //                }

            //                var pDT = Sql.Execute(pSql);
            //                if (pDT == undefined) return null;

            //                return pDT;

            //            };

            //            this.SaveMobizLibraryPeopleUserAccount = function (IsNew, vData) {

            //                var pIU;
            //                var pSql;

            //                pIU = new InsertUpdate('LibraryPeopleUserAccount');
            //                pIU.Add('Username', ReadyString(vData.Userame));
            //                pIU.Add('UserPassword', ReadyString(vData.UserPassword));

            //                if (IsNew) {
            //                    pIU.Add('PeopleID', ReadyString(vData.PeopleID));
            //                    pSql = pIU.GetInsert();
            //                } else {
            //                    pSql = pIU.GetUpdate('PeopleID = ' + ReadyString(vData.PeopleID));
            //                }

            //                Sql.ExecuteSql(pSql);

            //                return true;

            //            };
            //END Public MobizLibraryPeopleUserAccount -----------------------

            //Public MobizLibraryPeopleOrganization -----------------------
            //            this.MobizLibraryPeopleOrganization = function (vID) {

            //                var pSql;
            //                pSql = "SELECT ID " +
            //                ",PeopleID " +
            //                ",OrganizationID " +
            //                ",IsConfirmed " +
            //                ",DateTimeConfirmed " +
            //                ",DateTimeInvited " +
            //                ",IsTerminated " +
            //                ",DateTimeTerminated " +
            //                ",UserID " +
            //                " FROM Mobiz.LibraryPeopleOrganization "

            //                if (vID != '') {
            //                    pSql = pSql + " WHERE ID = " + ReadyString(vID);
            //                }

            //                var pDT = Sql.Execute(pSql);
            //                if (pDT == undefined) return null;

            //                return pDT;

            //            };

            this.SaveMobizLibraryPeopleOrganization = function (IsNew, vData) {

                var pIU;
                var pSql;

                pIU = new InsertUpdate('LibraryPeopleOrganization');
                pIU.Add('PeopleID', ReadyString(vData.PeopleID));
                pIU.Add('OrganizationID', ReadyString(vData.OrganizationID));
                pIU.Add('IsConfirmed', CDec2(vData.IsConfirmed));
                pIU.Add('DateTimeConfirmed', ReadyString(vData.DateTimeConfirmed));
                pIU.Add('DateTimeInvited', ReadyString(vData.DateTimeInvited));
                pIU.Add('IsTerminated', CDec2(vData.IsTerminated));
                pIU.Add('DateTimeTerminated', ReadyString(vData.DateTimeTerminated));
                pIU.Add('UserID', ReadyString(vData.UserID));

                if (IsNew) {
                    pIU.Add('ID', ReadyString(vData.ID));
                    pSql = pIU.GetInsert();
                } else {
                    pSql = pIU.GetUpdate('ID = ' + ReadyString(vData.ID));
                }

                Sql.ExecuteSql(pSql);

                return true;

            };
            //END Public MobizLibraryPeopleOrganization -----------------------

            //Public MobizLibraryOrganization -----------------------
            //            this.MobizLibraryOrganization = function (vID) {

            //                var pSql;
            //                pSql = "SELECT ID " +
            //                ",IsOrganization " +
            //                ",PersonTitleID " +
            //                ",PersonSurname " +
            //                ",PersonFirstName " +
            //                ",PersonMiddleName " +
            //                ",PersonSuffixName " +
            //                ",PersonDateOfBirth " +
            //                ",PersonMotherMaidenSurname " +
            //                ",PersonMotherMaidenFirstName " +
            //                ",PersonMotherMaidenMiddleName " +
            //                ",CompanyName " +
            //                ",CompanyNameInitial " +
            //                ",OwnershipType " +
            //                ",IsFranchisor " +
            //                ",RegistrationType " +
            //                ",RegistrationNo " +
            //                ",RegistrationDate " +
            //                ",OfficeAddressBuildingNo " +
            //                ",OfficeAddressStreet " +
            //                ",OfficeAddressCityID " +
            //                ",OfficeAddressCountryID " +
            //                ",BusinessAddressBuildingNo " +
            //                ",BusinessAddressStreet " +
            //                ",BusinessAddressCityID " +
            //                ",BusinessAddressCountryID " +
            //                ",Purpose " +
            //                ",BusinessScope " +
            //                ",NoOfRetailOutlet " +
            //                ",NoOfOffice " +
            //                ",NoOfWarehouse " +
            //                ",NoOfEmployees " +
            //                ",IsHasHoldingCompany " +
            //                ",IsABank " +
            //                ",UserID " +
            //                ",LastUpdated " +
            //                " FROM Mobiz.LibraryOrganization "

            //                if (vID != '') {
            //                    pSql = pSql + " WHERE ID = " + ReadyString(vID);
            //                }

            //                var pDT = Sql.Execute(pSql);
            //                if (pDT == undefined) return null;

            //                return pDT;

            //            };

            this.SaveMobizLibraryOrganization = function (IsNew, vData) {

                var pIU;
                var pSql;

                pIU = new InsertUpdate('LibraryOrganization');
                pIU.Add('IsOrganization', CDec2(vData.IsOrganization));
                pIU.Add('PersonTitleID', ReadyString(vData.PersonTitleID));
                pIU.Add('PersonSurname', ReadyString(vData.PersonSurname));
                pIU.Add('PersonFirstName', ReadyString(vData.PersonFirstName));
                pIU.Add('PersonMiddleName', ReadyString(vData.PersonMiddleName));
                pIU.Add('PersonSuffixName', ReadyString(vData.PersonSuffixName));
                pIU.Add('PersonDateOfBirth', ReadyString(vData.PersonDateOfBirth));
                pIU.Add('PersonMotherMaidenSurname', ReadyString(vData.PersonMotherMaidenSurname));
                pIU.Add('PersonMotherMaidenFirstName', ReadyString(vData.PersonMotherMaidenFirstName));
                pIU.Add('PersonMotherMaidenMiddleName', ReadyString(vData.PersonMotherMaidenMiddleName));
                pIU.Add('CompanyName', ReadyString(vData.CompanyName));
                pIU.Add('CompanyNameInitial', ReadyString(vData.CompanyNameInitial));
                pIU.Add('OwnershipType', ReadyString(vData.OwnershipType));
                pIU.Add('IsFranchisor', CDec2(vData.IsFranchisor));
                pIU.Add('RegistrationType', ReadyString(vData.RegistrationType));
                pIU.Add('RegistrationNo', ReadyString(vData.RegistrationNo));
                pIU.Add('RegistrationDate', ReadyString(vData.RegistrationDate));
                pIU.Add('OfficeAddressBuildingNo', ReadyString(vData.OfficeAddressBuildingNo));
                pIU.Add('OfficeAddressStreet', ReadyString(vData.OfficeAddressStreet));
                pIU.Add('OfficeAddressCityID', ReadyString(vData.OfficeAddressCityID));
                pIU.Add('OfficeAddressCountryID', ReadyString(vData.OfficeAddressCountryID));
                pIU.Add('BusinessAddressBuildingNo', ReadyString(vData.BusinessAddressBuildingNo));
                pIU.Add('BusinessAddressStreet', ReadyString(vData.BusinessAddressStreet));
                pIU.Add('BusinessAddressCityID', ReadyString(vData.BusinessAddressCityID));
                pIU.Add('BusinessAddressCountryID', ReadyString(vData.BusinessAddressCountryID));
                pIU.Add('Purpose', ReadyString(vData.Purpose));
                pIU.Add('BusinessScope', ReadyString(vData.BusinessScope));
                pIU.Add('NoOfRetailOutlet', ReadyString(vData.NoOfRetailOutlet));
                pIU.Add('NoOfOffice', ReadyString(vData.NoOfOffice));
                pIU.Add('NoOfWarehouse', ReadyString(vData.NoOfWarehouse));
                pIU.Add('NoOfEmployees', ReadyString(vData.NoOfEmployees));
                pIU.Add('IsHasHoldingCompany', CDec2(vData.IsHasHoldingCompany));
                pIU.Add('IsABank', CDec2(vData.IsABank));
                pIU.Add('UserID', ReadyString(vData.UserID));
                pIU.Add('LastUpdated', ReadyString(vData.LastUpdated));

                if (IsNew) {
                    pIU.Add('ID', ReadyString(vData.ID));
                    pSql = pIU.GetInsert();
                } else {
                    pSql = pIU.GetUpdate('ID = ' + ReadyString(vData.ID));
                }

                Sql.ExecuteSql(pSql);

                return true;

            };
            //END Public MobizLibraryOrganization -----------------------

            //Public MobizLibraryOrganizationContactEmail -----------------------
            //            this.MobizLibraryOrganizationContactEmail = function (vID) {

            //                var pSql;
            //                pSql = "SELECT ID " +
            //                ",OrganizationID " +
            //                ",EmailAddress " +
            //                ",UserID " +
            //                ",LastUpdated " +
            //                " FROM Mobiz.LibraryOrganizationContactEmail "

            //                if (vID != '') {
            //                    pSql = pSql + " WHERE ID = " + ReadyString(vID);
            //                }

            //                var pDT = Sql.Execute(pSql);
            //                if (pDT == undefined) return null;

            //                return pDT;

            //            };

            this.SaveMobizLibraryOrganizationContactEmail = function (IsNew, vData) {

                var pIU;
                var pSql;

                pIU = new InsertUpdate('LibraryOrganizationContactEmail');
                pIU.Add('OrganizationID', ReadyString(vData.OrganizationID));
                pIU.Add('EmailAddress', ReadyString(vData.EmailAddress));
                pIU.Add('UserID', ReadyString(vData.UserID));
                pIU.Add('LastUpdated', ReadyString(vData.LastUpdated));

                if (IsNew) {
                    pIU.Add('ID', ReadyString(vData.ID));
                    pSql = pIU.GetInsert();
                } else {
                    pSql = pIU.GetUpdate('ID = ' + ReadyString(vData.ID));
                }

                Sql.ExecuteSql(pSql);

                return true;

            };
            //END Public MobizLibraryOrganizationContactEmail -----------------------

            //Public MobizLibraryOrganizationTradeName -----------------------
            //            this.MobizLibraryOrganizationTradeName = function (vID) {

            //                var pSql;
            //                pSql = "SELECT ID " +
            //                ",OrganizationID " +
            //                ",BusinessTradeName " +
            //                ",BusinessTradeNameInitial " +
            //                " FROM Mobiz.LibraryOrganizationTradeName "

            //                if (vID != '') {
            //                    pSql = pSql + " WHERE ID = " + ReadyString(vID);
            //                }

            //                var pDT = Sql.Execute(pSql);
            //                if (pDT == undefined) return null;

            //                return pDT;

            //            };

            this.SaveMobizLibraryOrganizationTradeName = function (IsNew, vData) {

                var pIU;
                var pSql;

                pIU = new InsertUpdate('LibraryOrganizationTradeName');
                pIU.Add('OrganizationID', ReadyString(vData.OrganizationID));
                pIU.Add('BusinessTradeName', ReadyString(vData.BusinessTradeName));
                pIU.Add('BusinessTradeNameInitial', ReadyString(vData.BusinessTradeNameInitial));

                if (IsNew) {
                    pIU.Add('ID', ReadyString(vData.ID));
                    pSql = pIU.GetInsert();
                } else {
                    pSql = pIU.GetUpdate('ID = ' + ReadyString(vData.ID));
                }

                Sql.ExecuteSql(pSql);

                return true;

            };
            //END Public MobizLibraryOrganizationTradeName -----------------------

            //Public MobizLibraryOrganizationFOWST -----------------------
            //            this.MobizLibraryOrganizationFOWST = function (vID) {

            //                var pSql;
            //                pSql = "SELECT ID " +
            //                ",OrganizationID " +
            //                ",FOWSTTypeID " +
            //                ",Code " +
            //                ",Name " +
            //                ",Description " +
            //                ",BusinessTradeNameID " +
            //                ",StreetNumber " +
            //                ",Street " +
            //                ",FowstCityID " +
            //                ",FowstCountryID " +
            //                ",SubFOWSTTypeID " +
            //                ",PlateNo " +
            //                ",NoOfWheels " +
            //                ",BodyColor " +
            //                ",Weight " +
            //                ",Dimension " +
            //                ",OwnershipType " +
            //                ",ContractStart " +
            //                ",ContractEnd " +
            //                ",CostPerMonth " +
            //                ",IsMortgaged " +
            //                ",IsFranchisee " +
            //                ",FranchisorID " +
            //                ",IsFOWSTInsideAMall " +
            //                ",FOWSTMallName " +
            //                ",UserID " +
            //                ",LastUpdated " +
            //                ",FOWSTNo " +
            //                ",FulfillmentCenter " +
            //                " FROM Mobiz.LibraryOrganizationFOWST "

            //                if (vID != '') {
            //                    pSql = pSql + " WHERE ID = " + ReadyString(vID);
            //                }

            //                var pDT = Sql.Execute(pSql);
            //                if (pDT == undefined) return null;

            //                return pDT;

            //            };

            this.SaveMobizLibraryOrganizationFOWST = function (IsNew, vData) {

                var pIU;
                var pSql;

                pIU = new InsertUpdate('LibraryOrganizationFOWST');
                pIU.Add('OrganizationID', ReadyString(vData.OrganizationID));
                pIU.Add('FOWSTTypeID', ReadyString(vData.FOWSTTypeID));
                pIU.Add('Code', ReadyString(vData.Code));
                pIU.Add('Name', ReadyString(vData.Name));
                pIU.Add('Description', ReadyString(vData.Description));
                pIU.Add('BusinessTradeNameID', ReadyString(vData.BusinessTradeNameID));
                pIU.Add('StreetNumber', ReadyString(vData.StreetNumber));
                pIU.Add('Street', ReadyString(vData.Street));
                pIU.Add('FowstCityID', ReadyString(vData.FowstCityID));
                pIU.Add('FowstCountryID', ReadyString(vData.FowstCountryID));
                pIU.Add('SubFOWSTTypeID', ReadyString(vData.SubFOWSTTypeID));
                pIU.Add('PlateNo', ReadyString(vData.PlateNo));
                pIU.Add('NoOfWheels', ReadyString(vData.NoOfWheels));
                pIU.Add('BodyColor', ReadyString(vData.BodyColor));
                pIU.Add('Weight', CDec2(vData.Weight));
                pIU.Add('Dimension', ReadyString(vData.Dimension));
                pIU.Add('OwnershipType', CDec2(vData.OwnershipType));
                pIU.Add('ContractStart', ReadyString(vData.ContractStart));
                pIU.Add('ContractEnd', ReadyString(vData.ContractEnd));
                pIU.Add('CostPerMonth', CDec2(vData.CostPerMonth));
                pIU.Add('IsMortgaged', CDec2(vData.IsMortgaged));
                pIU.Add('IsFranchisee', CDec2(vData.IsFranchisee));
                pIU.Add('FranchisorID', ReadyString(vData.FranchisorID));
                pIU.Add('IsFOWSTInsideAMall', CDec2(vData.IsFOWSTInsideAMall));
                pIU.Add('FOWSTMallName', ReadyString(vData.FOWSTMallName));
                pIU.Add('UserID', ReadyString(vData.UserID));
                pIU.Add('LastUpdated', ReadyString(vData.LastUpdated));
                pIU.Add('FOWSTNo', CDec2(vData.FOWSTNo));
                pIU.Add('FulfillmentCenter', ReadyString(vData.FulfillmentCenter));

                if (IsNew) {
                    pIU.Add('ID', ReadyString(vData.ID));
                    pSql = pIU.GetInsert();
                } else {
                    pSql = pIU.GetUpdate('ID = ' + ReadyString(vData.ID));
                }

                Sql.ExecuteSql(pSql);

                return true;

            };
            //END Public MobizLibraryOrganizationFOWST -----------------------

            //Public MobizLibraryOrganizationFOWSTTel -----------------------
            //            this.MobizLibraryOrganizationFOWSTTel = function (vID) {

            //                var pSql;
            //                pSql = "SELECT ID " +
            //                ",FOWSTID " +
            //                ",CountryID " +
            //                ",AreaCodeID " +
            //                ",ContactNo " +
            //                ",ExtensionNo " +
            //                ",ContactType " +
            //                ",UserID " +
            //                ",LastUpdated " +
            //                " FROM Mobiz.LibraryOrganizationFOWSTTel "

            //                if (vID != '') {
            //                    pSql = pSql + " WHERE ID = " + ReadyString(vID);
            //                }

            //                var pDT = Sql.Execute(pSql);
            //                if (pDT == undefined) return null;

            //                return pDT;

            //            };

            this.SaveMobizLibraryOrganizationFOWSTTel = function (IsNew, vData) {

                var pIU;
                var pSql;

                pIU = new InsertUpdate('LibraryOrganizationFOWSTTel');
                pIU.Add('FOWSTID', ReadyString(vData.FOWSTID));
                pIU.Add('CountryID', ReadyString(vData.CountryID));
                pIU.Add('AreaCodeID', ReadyString(vData.AreaCodeID));
                pIU.Add('ContactNo', ReadyString(vData.ContactNo));
                pIU.Add('ExtensionNo', ReadyString(vData.ExtensionNo));
                pIU.Add('ContactType', ReadyString(vData.ContactType));
                pIU.Add('UserID', ReadyString(vData.UserID));
                pIU.Add('LastUpdated', ReadyString(vData.LastUpdated));

                if (IsNew) {
                    pIU.Add('ID', ReadyString(vData.ID));
                    pSql = pIU.GetInsert();
                } else {
                    pSql = pIU.GetUpdate('ID = ' + ReadyString(vData.ID));
                }

                Sql.ExecuteSql(pSql);

                return true;

            };
            //END Public MobizLibraryOrganizationFOWSTTel -----------------------

            //Public MobizLibraryOrganizationFOWSTEmail -----------------------
            //            this.MobizLibraryOrganizationFOWSTEmail = function (vID) {

            //                var pSql;
            //                pSql = "SELECT ID " +
            //                ",FOWSTID " +
            //                ",EmailAddress " +
            //                ",EmailType " +
            //                ",UserID " +
            //                ",LastUpdated " +
            //                " FROM Mobiz.LibraryOrganizationFOWSTEmail "

            //                if (vID != '') {
            //                    pSql = pSql + " WHERE ID = " + ReadyString(vID);
            //                }

            //                var pDT = Sql.Execute(pSql);
            //                if (pDT == undefined) return null;

            //                return pDT;

            //            };

            this.SaveMobizLibraryOrganizationFOWSTEmail = function (IsNew, vData) {

                var pIU;
                var pSql;

                pIU = new InsertUpdate('LibraryOrganizationFOWSTEmail');
                pIU.Add('FOWSTID', ReadyString(vData.FOWSTID));
                pIU.Add('EmailAddress', ReadyString(vData.EmailAddress));
                pIU.Add('EmailType', ReadyString(vData.EmailType));
                pIU.Add('UserID', ReadyString(vData.UserID));
                pIU.Add('LastUpdated', ReadyString(vData.LastUpdated));

                if (IsNew) {
                    pIU.Add('ID', ReadyString(vData.ID));
                    pSql = pIU.GetInsert();
                } else {
                    pSql = pIU.GetUpdate('ID = ' + ReadyString(vData.ID));
                }

                Sql.ExecuteSql(pSql);

                return true;

            };
            //END Public MobizLibraryOrganizationFOWSTTel -----------------------

            //Public MobizLibraryOrganizationFOWSTOperationHRs -----------------------
            //            this.MobizLibraryOrganizationFOWSTOperationHRs = function (vID) {

            //                var pSql;
            //                pSql = "SELECT ID " +
            //                ",FOWSTID " +
            //                ",WeekDay " +
            //                ",OpeningTime " +
            //                ",ClosingTime " +
            //                ",UserID " +
            //                ",LastUpdated " +
            //                " FROM Mobiz.LibraryOrganizationFOWSTOperationHRs "

            //                if (vID != '') {
            //                    pSql = pSql + " WHERE ID = " + ReadyString(vID);
            //                }

            //                var pDT = Sql.Execute(pSql);
            //                if (pDT == undefined) return null;

            //                return pDT;

            //            };

            this.SaveMobizLibraryOrganizationFOWSTOperationHRs = function (IsNew, vData) {

                var pIU;
                var pSql;

                pIU = new InsertUpdate('LibraryOrganizationFOWSTOperationHRs');
                pIU.Add('FOWSTID', ReadyString(vData.FOWSTID));
                pIU.Add('WeekDay', CDec2(vData.WeekDay));
                pIU.Add('OpeningTime', ReadyString(vData.OpeningTime));
                pIU.Add('ClosingTime', ReadyString(vData.ClosingTime));
                pIU.Add('UserID', ReadyString(vData.UserID));
                pIU.Add('LastUpdated', ReadyString(vData.LastUpdated));

                if (IsNew) {
                    pIU.Add('ID', ReadyString(vData.ID));
                    pSql = pIU.GetInsert();
                } else {
                    pSql = pIU.GetUpdate('ID = ' + ReadyString(vData.ID));
                }

                Sql.ExecuteSql(pSql);

                return true;

            };
            //END Public MobizLibraryOrganizationFOWSTOperationHRs -----------------------

            //Public MobizLibraryOrganizationRepresentatives -----------------------
            //            this.MobizLibraryOrganizationRepresentatives = function (vID) {

            //                var pSql;
            //                pSql = "SELECT ID " +
            //                ",OrganizationID " +
            //                ",LastName " +
            //                ",FirstName " +
            //                ",MiddleName " +
            //                ",Title " +
            //                ",GenerationalSuffix " +
            //                ",IsActive " +
            //                ",UserID " +
            //                ",LastUpdated " +
            //                " FROM Mobiz.LibraryOrganizationRepresentatives "

            //                if (vID != '') {
            //                    pSql = pSql + " WHERE ID = " + ReadyString(vID);
            //                }

            //                var pDT = Sql.Execute(pSql);
            //                if (pDT == undefined) return null;

            //                return pDT;

            //            };

            this.SaveMobizLibraryOrganizationRepresentatives = function (IsNew, vData) {

                var pIU;
                var pSql;

                pIU = new InsertUpdate('LibraryOrganizationRepresentatives');
                pIU.Add('OrganizationID', ReadyString(vData.OrganizationID));
                pIU.Add('LastName', ReadyString(vData.LastName));
                pIU.Add('FirstName', ReadyString(vData.FirstName));
                pIU.Add('MiddleName', ReadyString(vData.MiddleName));
                pIU.Add('Title', ReadyString(vData.Title));
                pIU.Add('GenerationalSuffix', ReadyString(vData.GenerationalSuffix));
                pIU.Add('IsActive', CDec2(vData.IsActive));
                pIU.Add('UserID', ReadyString(vData.UserID));
                pIU.Add('LastUpdated', ReadyString(vData.LastUpdated));

                if (IsNew) {
                    pIU.Add('ID', ReadyString(vData.ID));
                    pSql = pIU.GetInsert();
                } else {
                    pSql = pIU.GetUpdate('ID = ' + ReadyString(vData.ID));
                }

                Sql.ExecuteSql(pSql);

                return true;

            };
            //END Public MobizLibraryOrganizationRepresentatives -----------------------

            //Public MobizLibraryOrganizationRepresentativesEmail -----------------------
            //            this.MobizLibraryOrganizationRepresentativesEmail = function (vID) {

            //                var pSql;
            //                pSql = "SELECT ID " +
            //                ",OrgRepID " +
            //                ",EmailAddress " +
            //                ",EmailType " +
            //                " FROM Mobiz.LibraryOrganizationRepresentativesEmail "

            //                if (vID != '') {
            //                    pSql = pSql + " WHERE ID = " + ReadyString(vID);
            //                }

            //                var pDT = Sql.Execute(pSql);
            //                if (pDT == undefined) return null;

            //                return pDT;

            //            };

            this.SaveMobizLibraryOrganizationRepresentativesEmail = function (IsNew, vData) {

                var pIU;
                var pSql;

                pIU = new InsertUpdate('LibraryOrganizationRepresentativesEmail');
                pIU.Add('OrgRepID', ReadyString(vData.OrgRepID));
                pIU.Add('EmailAddress', ReadyString(vData.EmailAddress));
                pIU.Add('EmailType', ReadyString(vData.EmailType));

                if (IsNew) {
                    pIU.Add('ID', ReadyString(vData.ID));
                    pSql = pIU.GetInsert();
                } else {
                    pSql = pIU.GetUpdate('ID = ' + ReadyString(vData.ID));
                }

                Sql.ExecuteSql(pSql);

                return true;

            };
            //END Public MobizLibraryOrganizationRepresentatives -----------------------

            //Public MobizLibraryOrganizationRepresentativesMobile -----------------------
            //            this.MobizLibraryOrganizationRepresentativesMobile = function (vID) {

            //                var pSql;
            //                pSql = "SELECT ID " +
            //                ",OrgRepID " +
            //                ",CountryID " +
            //                ",TelCoPrefixID " +
            //                ",ContactNo " +
            //                ",ContactType " +
            //                " FROM Mobiz.LibraryOrganizationRepresentativesMobile "

            //                if (vID != '') {
            //                    pSql = pSql + " WHERE ID = " + ReadyString(vID);
            //                }

            //                var pDT = Sql.Execute(pSql);
            //                if (pDT == undefined) return null;

            //                return pDT;

            //            };

            this.SaveMobizLibraryOrganizationRepresentativesMobile = function (IsNew, vData) {

                var pIU;
                var pSql;

                pIU = new InsertUpdate('LibraryOrganizationRepresentativesMobile');
                pIU.Add('OrgRepID', ReadyString(vData.OrgRepID));
                pIU.Add('CountryID', ReadyString(vData.CountryID));
                pIU.Add('TelCoPrefixID', ReadyString(vData.TelCoPrefixID));
                pIU.Add('ContactNo', ReadyString(vData.ContactNo));
                pIU.Add('ContactType', ReadyString(vData.ContactType));

                if (IsNew) {
                    pIU.Add('ID', ReadyString(vData.ID));
                    pSql = pIU.GetInsert();
                } else {
                    pSql = pIU.GetUpdate('ID = ' + ReadyString(vData.ID));
                }

                Sql.ExecuteSql(pSql);

                return true;

            };
            //END Public MobizLibraryOrganizationRepresentativesMobile -----------------------

            //Public MobizLibraryOrganizationRepresentativesTel -----------------------
            //            this.MobizLibraryOrganizationRepresentativesTel = function (vID) {

            //                var pSql;
            //                pSql = "SELECT ID " +
            //                ",OrgRepID " +
            //                ",CountryID " +
            //                ",AreaCodeID " +
            //                ",ContactNo " +
            //                ",ExtensionNo " +
            //                ",ContactType " +
            //                " FROM Mobiz.LibraryOrganizationRepresentativesTel "

            //                if (vID != '') {
            //                    pSql = pSql + " WHERE ID = " + ReadyString(vID);
            //                }

            //                var pDT = Sql.Execute(pSql);
            //                if (pDT == undefined) return null;

            //                return pDT;

            //            };

            this.SaveMobizLibraryOrganizationRepresentativesTel = function (IsNew, vData) {

                var pIU;
                var pSql;

                pIU = new InsertUpdate('LibraryOrganizationRepresentativesTel');
                pIU.Add('OrgRepID', ReadyString(vData.OrgRepID));
                pIU.Add('CountryID', ReadyString(vData.CountryID));
                pIU.Add('AreaCodeID', ReadyString(vData.AreaCodeID));
                pIU.Add('ContactNo', ReadyString(vData.ContactNo));
                pIU.Add('ExtensionNo', ReadyString(vData.ExtensionNo));
                pIU.Add('ContactType', ReadyString(vData.ContactType));

                if (IsNew) {
                    pIU.Add('ID', ReadyString(vData.ID));
                    pSql = pIU.GetInsert();
                } else {
                    pSql = pIU.GetUpdate('ID = ' + ReadyString(vData.ID));
                }

                Sql.ExecuteSql(pSql);

                return true;

            };
            //END Public MobizLibraryOrganizationRepresentativesTel -----------------------

            //Private LibraryOrganizationCustomer -----------------------
            //            this.LibraryOrganizationCustomer = function (vID) {

            //                var pSql;
            //                pSql = "SELECT ID " +
            //                ",OrganizationID " +
            //                ",ActivationDate " +
            //                ",Deactivated " +
            //                ",DeactivationDate " +
            //                ",DeactivationReason " +
            //                ",Comments " +
            //                ",IsBan " +
            //                " FROM LibraryOrganizationCustomer "

            //                if (vID != '') {
            //                    pSql = pSql + " WHERE ID = " + ReadyString(vID);
            //                }

            //                var pDT = Sql.Execute(pSql);
            //                if (pDT == undefined) return null;

            //                return pDT;

            //            };

            this.SaveLibraryOrganizationCustomer = function (IsNew, vData) {

                var pIU;
                var pSql;

                pIU = new InsertUpdate('private.LibraryOrganizationCustomer');
                pIU.Add('OrganizationID', ReadyString(vData.OrganizationID));
                pIU.Add('ActivationDate', ReadyString(vData.ActivationDate));
                pIU.Add('Deactivated', CDec2(vData.Deactivated));
                pIU.Add('DeactivationDate', ReadyString(vData.DeactivationDate));
                pIU.Add('DeactivationReason', ReadyString(vData.DeactivationReason));
                pIU.Add('Comments', ReadyString(vData.Comments));
                pIU.Add('IsBan', CDec2(vData.IsBan));
                pIU.Add('LastUpdated', ReadyString(vData.LastUpdated));

                if (IsNew) {
                    pIU.Add('ID', ReadyString(vData.ID));
                    pSql = pIU.GetInsert();
                } else {
                    pSql = pIU.GetUpdate('ID = ' + ReadyString(vData.ID));
                }

                Sql.ExecuteSql(pSql);

                return true;

            };
            //END Public LibraryOrganizationCustomer -----------------------

            //Private LibraryOrganizationEmployee -----------------------
            //            this.LibraryOrganizationEmployee = function (vID) {

            //                var pSql;
            //                pSql = "SELECT ID " +
            //                ",PeopleID " +
            //                ",DateHired " +
            //                ",Deactivated " +
            //                ",DeactivationDate " +
            //                ",DeactivationReason " +
            //                ",Comments " +
            //                ",IsBan " +
            //                " FROM LibraryOrganizationEmployee "

            //                if (vID != '') {
            //                    pSql = pSql + " WHERE ID = " + ReadyString(vID);
            //                }

            //                var pDT = Sql.Execute(pSql);
            //                if (pDT == undefined) return null;

            //                return pDT;

            //            };

            this.SaveLibraryOrganizationEmployee = function (IsNew, vData) {

                var pIU;
                var pSql;

                pIU = new InsertUpdate('private.LibraryOrganizationEmployee');
                pIU.Add('PeopleID', ReadyString(vData.PeopleID));
                pIU.Add('DateHired', ReadyString(vData.DateHired));
                pIU.Add('Deactivated', CDec2(vData.Deactivated));
                pIU.Add('DeactivationDate', ReadyString(vData.DeactivationDate));
                pIU.Add('DeactivationReason', ReadyString(vData.DeactivationReason));
                pIU.Add('Comments', ReadyString(vData.Comments));
                pIU.Add('IsBan', CDec2(vData.IsBan));
                pIU.Add('LastUpdated', ReadyString(vData.LastUpdated));

                if (IsNew) {
                    pIU.Add('ID', ReadyString(vData.ID));
                    pSql = pIU.GetInsert();
                } else {
                    pSql = pIU.GetUpdate('ID = ' + ReadyString(vData.ID));
                }

                Sql.ExecuteSql(pSql);

                return true;

            };
            //END Private LibraryOrganizationEmployee -----------------------

            //Private LibraryOrganizationEmployeeFOWST -----------------------
            //            this.LibraryOrganizationEmployeeFOWST = function (vID) {

            //                var pSql;
            //                pSql = "SELECT ID " +
            //                ",PeopleID " +
            //                ",FowstID " +
            //                " FROM LibraryOrganizationEmployeeFOWST "

            //                if (vID != '') {
            //                    pSql = pSql + " WHERE ID = " + ReadyString(vID);
            //                }

            //                var pDT = Sql.Execute(pSql);
            //                if (pDT == undefined) return null;

            //                return pDT;

            //            };

            this.SaveLibraryOrganizationEmployeeFOWST = function (IsNew, vData) {

                var pIU;
                var pSql;

                pIU = new InsertUpdate('private.LibraryOrganizationEmployeeFOWST');
                pIU.Add('PeopleID', ReadyString(vData.PeopleID));
                pIU.Add('FowstID', ReadyString(vData.FowstID));
                pIU.Add('LastUpdated', ReadyString(vData.LastUpdated));

                if (IsNew) {
                    pIU.Add('ID', ReadyString(vData.ID));
                    pSql = pIU.GetInsert();
                } else {
                    pSql = pIU.GetUpdate('ID = ' + ReadyString(vData.ID));
                }

                Sql.ExecuteSql(pSql);

                return true;

            };
            //END Private LibraryOrganizationEmployeeFOWST -----------------------

            this.SaveLibraryOrganizationPIM = function (IsNew, vData) {

                var pIU;
                var pSql;

                pIU = new InsertUpdate('private.LibraryOrganizationPIM');
                pIU.Add('ProductID', ReadyString(vData.ProductID));
                pIU.Add('ActivationDate', ReadyString(vData.ActivationDate));
                pIU.Add('IsDeactivated', CDec2(vData.IsDeactivated));
                pIU.Add('DeactivationDate', ReadyString(vData.DeactivationDate));
                pIU.Add('DeactivationReason', ReadyString(vData.DeactivationReason));
                pIU.Add('Comments', ReadyString(vData.Comments));
                pIU.Add('IsBan', CDec2(vData.IsBan));
                pIU.Add('IsDoNotShareTransDataToBrandOwner', CDec2(vData.IsDoNotShareTransDataToBrandOwner));

                if (IsNew) {
                    pIU.Add('ID', ReadyString(vData.ID));
                    pSql = pIU.GetInsert();
                } else {
                    pSql = pIU.GetUpdate('ID = ' + ReadyString(vData.ID));
                }

                Sql.ExecuteSql(pSql);

                return true;

            };
            //END Private LibraryOrganizationPIM -----------------------


            //Private LibraryOrganizationPriceLevel -----------------------
            //            this.LibraryOrganizationPriceLevel = function (vID) {

            //                var pSql;
            //                pSql = "SELECT ID " +
            //                ",OrganizationID " +
            //                ",PriceLevelID " +
            //                ",UserID " +
            //                ",LastUpdated " +
            //                " FROM LibraryOrganizationPriceLevel "

            //                if (vID != '') {
            //                    pSql = pSql + " WHERE ID = " + ReadyString(vID);
            //                }

            //                var pDT = Sql.Execute(pSql);
            //                if (pDT == undefined) return null;

            //                return pDT;

            //            };

            this.SaveLibraryOrganizationPriceLevel = function (IsNew, vData) {

                var pIU;
                var pSql;

                pIU = new InsertUpdate('private.LibraryOrganizationPriceLevel');
                pIU.Add('OrganizationID', ReadyString(vData.OrganizationID));
                pIU.Add('PriceLevelID', ReadyString(vData.PriceLevelID));
                pIU.Add('UserID', ReadyString(vData.UserID));
                pIU.Add('LastUpdated', ReadyString(vData.LastUpdated));

                if (IsNew) {
                    pIU.Add('ID', ReadyString(vData.ID));
                    pSql = pIU.GetInsert();
                } else {
                    pSql = pIU.GetUpdate('ID = ' + ReadyString(vData.ID));
                }

                Sql.ExecuteSql(pSql);

                return true;

            };
            //END Private LibraryOrganizationPriceLevel -----------------------

            //Private MobizLibraryPIMMeasure -----------------------
            //            this.MobizLibraryPIMMeasure = function (vID) {

            //                var pSql;
            //                pSql = "SELECT ID " +
            //                ",MeasureNameID " +
            //                ",SignID " +
            //                ",IsStandardLength " +
            //                ",IsStandardTime " +
            //                ",IsStandardArea " +
            //                ",IsStandardVolCapacity " +
            //                ",IsStandardVolDry " +
            //                ",IsStandardWeight " +
            //                ",IsStandardCooking " +
            //                ",IsStandardElectricity " +
            //                ",IsActive " +
            //                ",UserID " +
            //                ",LastUpdated " +
            //                " FROM Mobiz.LibraryPIMMeasure "

            //                if (vID != '') {
            //                    pSql = pSql + " WHERE ID = " + ReadyString(vID);
            //                }

            //                var pDT = Sql.Execute(pSql);
            //                if (pDT == undefined) return null;

            //                return pDT;

            //            };

            this.SaveMobizLibraryPIMMeasure = function (IsNew, vData) {

                var pIU;
                var pSql;

                pIU = new InsertUpdate('LibraryPIMMeasure');
                pIU.Add('MeasureNameID', ReadyString(vData.MeasureNameID));
                pIU.Add('SignID', ReadyString(vData.SignID));
                pIU.Add('IsStandardLength', CDec2(vData.IsStandardLength));
                pIU.Add('IsStandardTime', CDec2(vData.IsStandardTime));
                pIU.Add('IsStandardArea', CDec2(vData.IsStandardArea));
                pIU.Add('IsStandardVolCapacity', CDec2(vData.IsStandardVolCapacity));
                pIU.Add('IsStandardVolDry', CDec2(vData.IsStandardVolDry));
                pIU.Add('IsStandardWeight', CDec2(vData.IsStandardWeight));
                pIU.Add('IsStandardCooking', CDec2(vData.IsStandardCooking));
                pIU.Add('IsStandardElectricity', CDec2(vData.IsStandardElectricity));
                pIU.Add('IsActive', CDec2(vData.IsActive));
                pIU.Add('UserID', ReadyString(vData.UserID));
                pIU.Add('LastUpdated', ReadyString(vData.LastUpdated));

                if (IsNew) {
                    pIU.Add('ID', ReadyString(vData.ID));
                    pSql = pIU.GetInsert();
                } else {
                    pSql = pIU.GetUpdate('ID = ' + ReadyString(vData.ID));
                }

                Sql.ExecuteSql(pSql);

                return true;

            };
            //END Private LibraryOrganizationPriceLevel -----------------------

            //Private MobizLibraryPIMMeasureSign -----------------------
            //            this.MobizLibraryPIMMeasureSign = function (vID) {

            //                var pSql;
            //                pSql = "SELECT ID " +
            //                ",Sign " +
            //                ",IsActive " +
            //                ",UserID " +
            //                ",LastUpdated " +
            //                " FROM Mobiz.LibraryPIMMeasureSign "

            //                if (vID != '') {
            //                    pSql = pSql + " WHERE ID = " + ReadyString(vID);
            //                }

            //                var pDT = Sql.Execute(pSql);
            //                if (pDT == undefined) return null;

            //                return pDT;

            //            };

            this.SaveMobizLibraryPIMMeasureSign = function (IsNew, vData) {

                var pIU;
                var pSql;

                pIU = new InsertUpdate('LibraryPIMMeasureSign');
                pIU.Add('Sign', ReadyString(vData.Sign));
                pIU.Add('IsActive', CDec2(vData.IsActive));
                pIU.Add('UserID', ReadyString(vData.UserID));
                pIU.Add('LastUpdated', ReadyString(vData.LastUpdated));

                if (IsNew) {
                    pIU.Add('ID', ReadyString(vData.ID));
                    pSql = pIU.GetInsert();
                } else {
                    pSql = pIU.GetUpdate('ID = ' + ReadyString(vData.ID));
                }

                Sql.ExecuteSql(pSql);

                return true;

            };
            //END Private MobizLibraryPIMMeasureSign -----------------------

            //Private MobizLibraryPIMProduct -----------------------
            //            this.MobizLibraryPIMProduct = function (vID) {

            //                var pSql;
            //                pSql = "SELECT ID " +
            //                ",PublishedByID " +
            //                ",UploadedByID " +
            //                ",IsBrandOwner " +
            //                ",ProductNo " +
            //                ",ProductGroupNo " +
            //                ",ProductName " +
            //                ",Brand " +
            //                ",DivisionDescription " +
            //                ",SRP " +
            //                ",MarketingText " +
            //                ",Warranty " +
            //                ",ModelNo " +
            //                ",ProductFamilyID " +
            //                ",UNSPSCSegmentID " +
            //                ",UNSPSCFamilyID " +
            //                ",UNSPSCClassID " +
            //                ",UNSPSCCommodityID " +
            //                ",CustomClassification " +
            //                ",ProductSearchString " +
            //                ",DateAdded " +
            //                ",IsActive " +
            //                ",UserID " +
            //                ",LastUpdated " +
            //                " FROM Mobiz.LibraryPIMProduct "

            //                if (vID != '') {
            //                    pSql = pSql + " WHERE ID = " + ReadyString(vID);
            //                }

            //                var pDT = Sql.Execute(pSql);
            //                if (pDT == undefined) return null;

            //                return pDT;

            //            };

            this.SaveMobizLibraryPIMProduct = function (IsNew, vData) {

                var pIU;
                var pSql;

                pIU = new InsertUpdate('LibraryPIMProduct');
                pIU.Add('PublishedByID', ReadyString(vData.PublishedByID));
                pIU.Add('UploadedByID', ReadyString(vData.UploadedByID));
                pIU.Add('IsBrandOwner', CDec2(vData.IsBrandOwner));
                pIU.Add('ProductNo', ReadyString(vData.ProductNo));
                pIU.Add('ProductGroupNo', ReadyString(vData.ProductGroupNo));
                pIU.Add('ProductName', ReadyString(vData.ProductName));
                pIU.Add('Brand', ReadyString(vData.Brand));
                pIU.Add('DivisionDescription', ReadyString(vData.DivisionDescription));
                pIU.Add('SRP', ReadyString(vData.SRP));
                pIU.Add('MarketingText', ReadyString(vData.MarketingText));
                pIU.Add('Warranty', ReadyString(vData.Warranty));
                pIU.Add('ModelNo', ReadyString(vData.ModelNo));
                pIU.Add('ProductFamilyID', ReadyString(vData.ProductFamilyID));
                pIU.Add('UNSPSCSegmentID', ReadyString(vData.UNSPSCSegmentID));
                pIU.Add('UNSPSCFamilyID', ReadyString(vData.UNSPSCFamilyID));
                pIU.Add('UNSPSCClassID', ReadyString(vData.UNSPSCClassID));
                pIU.Add('UNSPSCCommodityID', ReadyString(vData.UNSPSCCommodityID));
                pIU.Add('CustomClassification', ReadyString(vData.CustomClassification));
                pIU.Add('ProductSearchString', ReadyString(vData.ProductSearchString));
                pIU.Add('DateAdded', ReadyString(vData.DateAdded));
                pIU.Add('IsActive', CDec2(vData.IsActive));
                pIU.Add('UserID', ReadyString(vData.UserID));
                pIU.Add('LastUpdated', ReadyString(vData.LastUpdated));

                if (IsNew) {
                    pIU.Add('ID', ReadyString(vData.ID));
                    pSql = pIU.GetInsert();
                } else {
                    pSql = pIU.GetUpdate('ID = ' + ReadyString(vData.ID));
                }

                Sql.ExecuteSql(pSql);

                return true;

            };
            //END Private MobizLibraryPIMProduct -----------------------

            //Private MobizLibraryPIMProductMeasurement -----------------------
            //            this.MobizLibraryPIMProductMeasurement = function (vID) {

            //                var pSql;
            //                pSql = "SELECT ID " +
            //                ",ProductID " +
            //                ",MeasurementLevel " +
            //                ",UnitMeasureID " +
            //                ",Quantity " +
            //                ",ContentQty " +
            //                ",ContentUnit " +
            //                ",ProdDimensionLength " +
            //                ",ProdDimensionLengthUnit " +
            //                ",ProdDimensionWidth " +
            //                ",ProdDimensionWidthUnit " +
            //                ",ProdDimensionHeight " +
            //                ",ProdDimensionHeightUnit    " +
            //                ",ProductWeight " +
            //                ",ProductWeightUnit " +
            //                ",Discount " +
            //                ",UserID " +
            //                ",LastUpdated " +
            //                " FROM Mobiz.LibraryPIMProductMeasurement "

            //                if (vID != '') {
            //                    pSql = pSql + " WHERE ID = " + ReadyString(vID);
            //                }

            //                var pDT = Sql.Execute(pSql);
            //                if (pDT == undefined) return null;

            //                return pDT;

            //            };

            this.SaveMobizLibraryPIMProductMeasurement = function (IsNew, vData) {

                var pIU;
                var pSql;

                pIU = new InsertUpdate('LibraryPIMProductMeasurement');
                pIU.Add('ProductID', ReadyString(vData.ProductID));
                pIU.Add('MeasurementLevel', CDec2(vData.MeasurementLevel));
                pIU.Add('UnitMeasureID', ReadyString(vData.UnitMeasureID));
                pIU.Add('Quantity', CDec2(vData.Quantity));
                pIU.Add('ContentQty', CDec2(vData.ContentQty));
                pIU.Add('ContentUnit', CDec2(vData.ContentUnit));
                pIU.Add('ProdDimensionLength', CDec2(vData.ProdDimensionLength));
                pIU.Add('ProdDimensionLengthUnit', ReadyString(vData.ProdDimensionLengthUnit));
                pIU.Add('ProdDimensionWidth', CDec2(vData.ProdDimensionWidth));
                pIU.Add('ProdDimensionWidthUnit', ReadyString(vData.ProdDimensionWidthUnit));
                pIU.Add('ProdDimensionHeight', CDec2(vData.ProdDimensionHeight));
                pIU.Add('ProdDimensionHeightUnit', ReadyString(vData.ProdDimensionHeightUnit));
                pIU.Add('ProductWeight', CDec2(vData.ProductWeight));
                pIU.Add('ProductWeightUnit', ReadyString(vData.ProductWeightUnit));
                pIU.Add('Discount', CDec2(vData.Discount));
                pIU.Add('UserID', ReadyString(vData.UserID));
                pIU.Add('LastUpdated', ReadyString(vData.LastUpdated));

                if (IsNew) {
                    pIU.Add('ID', ReadyString(vData.ID));
                    pSql = pIU.GetInsert();
                } else {
                    pSql = pIU.GetUpdate('ID = ' + ReadyString(vData.ID));
                }

                Sql.ExecuteSql(pSql);

                return true;

            };
            //END Private MobizLibraryPIMProductMeasurement -----------------------

            //Private MobizLibraryPIMProductMeasurementBarcode -----------------------
            //            this.MobizLibraryPIMProductMeasurementBarcode = function (vID) {

            //                var pSql;
            //                pSql = "SELECT ID " +
            //                ",StockMeasurementsID " +
            //                ",Barcode " +
            //                ",ReadyBarcode " +
            //                " FROM Mobiz.LibraryPIMProductMeasurementBarcode "

            //                if (vID != '') {
            //                    pSql = pSql + " WHERE ID = " + ReadyString(vID);
            //                }

            //                var pDT = Sql.Execute(pSql);
            //                if (pDT == undefined) return null;

            //                return pDT;

            //            };

            this.SaveMobizLibraryPIMProductMeasurementBarcode = function (IsNew, vData) {

                var pIU;
                var pSql;

                pIU = new InsertUpdate('LibraryPIMProductMeasurementBarcode');
                pIU.Add('StockMeasurementsID', ReadyString(vData.StockMeasurementsID));
                pIU.Add('Barcode', ReadyString(vData.Barcode));
                pIU.Add('ReadyBarcode', ReadyString(vData.ReadyBarcode));
                pIU.Add('LastUpdated', ReadyString(vData.LastUpdated));

                if (IsNew) {
                    pIU.Add('ID', ReadyString(vData.ID));
                    pSql = pIU.GetInsert();
                } else {
                    pSql = pIU.GetUpdate('ID = ' + ReadyString(vData.ID));
                }

                Sql.ExecuteSql(pSql);

                return true;

            };
            //END Private MobizLibraryPIMProductMeasurementBarcode -----------------------

            //Private MobizLibraryCountries -----------------------
            //            this.MobizLibraryCountries = function (vID) {

            //                var pSql;
            //                pSql = "SELECT ID " +
            //                ",Code " +
            //                ",Name " +
            //                ",Alpha2Code " +
            //                ",NumericCode " +
            //                ",ISOCode " +
            //                ",CallingCode " +
            //                ",NoOfDigits " +
            //                ",VATRate " +
            //                ",IsVATReference " +
            //                ",IsActive " +
            //                ",UserID " +
            //                ",LastUpdated " +
            //                " FROM Mobiz.LibraryCountries "

            //                if (vID != '') {
            //                    pSql = pSql + " WHERE ID = " + ReadyString(vID);
            //                }

            //                var pDT = Sql.Execute(pSql);
            //                if (pDT == undefined) return null;

            //                return pDT;

            //            };

            this.SaveMobizLibraryCountries = function (IsNew, vData) {

                var pIU;
                var pSql;

                pIU = new InsertUpdate('LibraryCountries');
                pIU.Add('Code', ReadyString(vData.Code));
                pIU.Add('Name', ReadyString(vData.Name));
                pIU.Add('Alpha2Code', ReadyString(vData.Alpha2Code));
                pIU.Add('NumericCode', vData.NumericCode);
                pIU.Add('ISOCode', ReadyString(vData.ISOCode));
                pIU.Add('CallingCode', ReadyString(vData.CallingCode));
                pIU.Add('NoOfDigits', CDec2(vData.NoOfDigits));
                pIU.Add('VATRate', CDec2(vData.VATRate));
                pIU.Add('IsVATReference', CDec2(vData.IsVATReference));
                pIU.Add('IsActive', CDec2(vData.IsActive));
                pIU.Add('UserID', ReadyString(vData.UserID));
                pIU.Add('LastUpdated', ReadyString(vData.LastUpdated));

                if (IsNew) {
                    pIU.Add('ID', ReadyString(vData.ID));
                    pSql = pIU.GetInsert();
                } else {
                    pSql = pIU.GetUpdate('ID = ' + ReadyString(vData.ID));
                }

                Sql.ExecuteSql(pSql);

                return true;

            };
            //END Private MobizLibraryCountries -----------------------

            //Private MobizLibraryCities -----------------------
            //            this.MobizLibraryCities = function (vID) {

            //                var pSql;
            //                pSql = "SELECT ID " +
            //                ",Name " +
            //                ",Zip " +
            //                ",IsActive " +
            //                ",UserID " +
            //                ",LastUpdated " +
            //                " FROM Mobiz.LibraryCities "

            //                if (vID != '') {
            //                    pSql = pSql + " WHERE ID = " + ReadyString(vID);
            //                }

            //                var pDT = Sql.Execute(pSql);
            //                if (pDT == undefined) return null;

            //                return pDT;

            //            };

            this.SaveMobizLibraryCities = function (IsNew, vData) {

                var pIU;
                var pSql;

                pIU = new InsertUpdate('LibraryCities');
                pIU.Add('Name', ReadyString(vData.Name));
                pIU.Add('Zip', ReadyString(vData.Zip));
                pIU.Add('IsActive', CDec2(vData.IsActive));
                pIU.Add('UserID', ReadyString(vData.UserID));
                pIU.Add('LastUpdated', ReadyString(vData.LastUpdated));

                if (IsNew) {
                    pIU.Add('ID', ReadyString(vData.ID));
                    pSql = pIU.GetInsert();
                } else {
                    pSql = pIU.GetUpdate('ID = ' + ReadyString(vData.ID));
                }

                Sql.ExecuteSql(pSql);

                return true;

            };
            //END Private SaveMobizLibraryCities -----------------------

            //Private MobizLibraryAreaCodes -----------------------
            //            this.MobizLibraryAreaCodes = function (vID) {

            //                var pSql;
            //                pSql = "SELECT ID " +
            //                ",Area " +
            //                ",CallingCode " +
            //                ",CountryId " +
            //                ",IsActive " +
            //                ",UserID " +
            //                ",LastUpdated " +
            //                " FROM Mobiz.LibraryAreaCodes "

            //                if (vID != '') {
            //                    pSql = pSql + " WHERE ID = " + ReadyString(vID);
            //                }

            //                var pDT = Sql.Execute(pSql);
            //                if (pDT == undefined) return null;

            //                return pDT;

            //            };

            this.SaveMobizLibraryAreaCodes = function (IsNew, vData) {

                var pIU;
                var pSql;

                pIU = new InsertUpdate('LibraryAreaCodes');
                pIU.Add('Area', ReadyString(vData.Area));
                pIU.Add('CallingCode', ReadyString(vData.CallingCode));
                pIU.Add('CountryId', ReadyString(vData.CountryId));
                pIU.Add('IsActive', CDec2(vData.IsActive));
                pIU.Add('UserID', ReadyString(vData.UserID));
                pIU.Add('LastUpdated', ReadyString(vData.LastUpdated));

                if (IsNew) {
                    pIU.Add('ID', ReadyString(vData.ID));
                    pSql = pIU.GetInsert();
                } else {
                    pSql = pIU.GetUpdate('ID = ' + ReadyString(vData.ID));
                }

                Sql.ExecuteSql(pSql);

                return true;

            };
            //END Private MobizLibraryAreaCodes -----------------------

            //Private MobizLibraryTelcos -----------------------
            //            this.MobizLibraryTelcos = function (vID) {

            //                var pSql;
            //                pSql = "SELECT ID " +
            //                ",Name " +
            //                ",CountryID " +
            //                ",IsActive " +
            //                ",UserID " +
            //                ",LastUpdated " +
            //                " FROM Mobiz.LibraryTelcos "

            //                if (vID != '') {
            //                    pSql = pSql + " WHERE ID = " + ReadyString(vID);
            //                }

            //                var pDT = Sql.Execute(pSql);
            //                if (pDT == undefined) return null;

            //                return pDT;

            //            };

            this.SaveMobizLibraryTelcos = function (IsNew, vData) {

                var pIU;
                var pSql;

                pIU = new InsertUpdate('LibraryTelcos');
                pIU.Add('Name', ReadyString(vData.Name));
                pIU.Add('CountryID', ReadyString(vData.CountryID));
                pIU.Add('IsActive', CDec2(vData.IsActive));
                pIU.Add('UserID', ReadyString(vData.UserID));
                pIU.Add('LastUpdated', ReadyString(vData.LastUpdated));

                if (IsNew) {
                    pIU.Add('ID', ReadyString(vData.Id));
                    pSql = pIU.GetInsert();
                } else {
                    pSql = pIU.GetUpdate('ID = ' + ReadyString(vData.Id));
                }

                Sql.ExecuteSql(pSql);

                return true;

            };
            //END Private MobizLibraryTelcos -----------------------

            //Private MobizLibraryTelcoPrefixes -----------------------
            //            this.MobizLibraryTelcoPrefixes = function (vID) {

            //                var pSql;
            //                pSql = "SELECT ID " +
            //                ",Prefix " +
            //                ",TelcoId " +
            //                ",IsActive " +
            //                ",UserID " +
            //                ",LastUpdated " +
            //                " FROM Mobiz.LibraryTelcoPrefixes "

            //                if (vID != '') {
            //                    pSql = pSql + " WHERE ID = " + ReadyString(vID);
            //                }

            //                var pDT = Sql.Execute(pSql);
            //                if (pDT == undefined) return null;

            //                return pDT;

            //            };

            this.SaveMobizLibraryTelcoPrefixes = function (IsNew, vData) {

                var pIU;
                var pSql;

                pIU = new InsertUpdate('LibraryTelcoPrefixes');
                pIU.Add('Prefix', ReadyString(vData.Prefix));
                pIU.Add('TelcoId', ReadyString(vData.TelcoId));
                pIU.Add('IsActive', CDec2(vData.IsActive));
                pIU.Add('UserID', ReadyString(vData.UserID));
                pIU.Add('LastUpdated', ReadyString(vData.LastUpdated));

                if (IsNew) {
                    pIU.Add('ID', ReadyString(vData.ID));
                    pSql = pIU.GetInsert();
                } else {
                    pSql = pIU.GetUpdate('ID = ' + ReadyString(vData.ID));
                }

                Sql.ExecuteSql(pSql);

                return true;

            };
            //END Private MobizLibraryTelcoPrefixes -----------------------

            //Private MobizLibraryPIMProductPriceLevel -----------------------
            //            this.MobizLibraryPIMProductPriceLevel = function (vID) {

            //                var pSql;
            //                pSql = "SELECT ID " +
            //                ",OrganizationID " +
            //                ",ProductID " +
            //                ",PriceLevelID " +
            //                ",Price " +
            //                ",DeactivateDiscount " +
            //                ",UserID " +
            //                ",LastUpdated " +
            //                " FROM Mobiz.LibraryPIMProductPriceLevel "

            //                if (vID != '') {
            //                    pSql = pSql + " WHERE ID = " + ReadyString(vID);
            //                }

            //                var pDT = Sql.Execute(pSql);
            //                if (pDT == undefined) return null;

            //                return pDT;

            //            };

            this.SaveMobizLibraryPIMProductPriceLevel = function (IsNew, vData) {

                var pIU;
                var pSql;

                pIU = new InsertUpdate('LibraryPIMProductPriceLevel');
                pIU.Add('OrganizationID', ReadyString(vData.OrganizationID));
                pIU.Add('ProductID', ReadyString(vData.ProductID));
                pIU.Add('PriceLevelID', ReadyString(vData.PriceLevelID));
                pIU.Add('Price', CDec2(vData.Price));
                pIU.Add('DeactivateDiscount', CDec2(vData.DeactivateDiscount));
                pIU.Add('UserID', ReadyString(vData.UserID));
                pIU.Add('LastUpdated', ReadyString(vData.LastUpdated));

                if (IsNew) {
                    pIU.Add('ID', ReadyString(vData.ID));
                    pSql = pIU.GetInsert();
                } else {
                    pSql = pIU.GetUpdate('ID = ' + ReadyString(vData.ID));
                }

                Sql.ExecuteSql(pSql);

                return true;

            };
            //END Private MobizLibraryPIMProductPriceLevel -----------------------

            //Private MobizLibraryPIMPriceLevel -----------------------
            //            this.MobizLibraryPIMPriceLevel = function (vID) {

            //                var pSql;
            //                pSql = "SELECT ID " +
            //                ",PriceLevel " +
            //                ",IsActive " +
            //                ",UserID " +
            //                ",LastUpdated " +
            //                " FROM Mobiz.LibraryPIMPriceLevel "

            //                if (vID != '') {
            //                    pSql = pSql + " WHERE ID = " + ReadyString(vID);
            //                }

            //                var pDT = Sql.Execute(pSql);
            //                if (pDT == undefined) return null;

            //                return pDT;

            //            };

            this.SaveMobizLibraryPIMPriceLevel = function (IsNew, vData) {

                var pIU;
                var pSql;

                pIU = new InsertUpdate('LibraryPIMPriceLevel');
                pIU.Add('PriceLevel', ReadyString(vData.PriceLevel));
                pIU.Add('IsActive', CDec2(vData.IsActive));
                pIU.Add('UserID', ReadyString(vData.UserID));
                pIU.Add('LastUpdated', ReadyString(vData.LastUpdated));

                if (IsNew) {
                    pIU.Add('ID', ReadyString(vData.ID));
                    pSql = pIU.GetInsert();
                } else {
                    pSql = pIU.GetUpdate('ID = ' + ReadyString(vData.ID));
                }

                Sql.ExecuteSql(pSql);

                return true;

            };
            //END Private SaveMobizLibraryPIMPriceLevel -----------------------

            this.SaveTablesLastUpdated = function (IsNew, vData) {

                var pIU;
                var pSql;

                pIU = new InsertUpdate('private.TablesLastUpdated');
                pIU.Add('TableName', ReadyString(vData.TableName));
                pIU.Add('LastUpdated', ReadyString(vData.LastUpdated));

                if (IsNew) {
                    pIU.Add('ID', ReadyString(vData.ID));
                    pSql = pIU.GetInsert();
                } else {
                    pSql = pIU.GetUpdate('ID = ' + ReadyString(vData.ID));
                }

                Sql.ExecuteSql(pSql);

                pIU = null;

                return true;

            };

            return this;

        }; // SaveTablesLastUpdated


        // LibraryPimMenu
        this.SaveLibraryPimMenu = function (IsNew, vData) {

            var pIU;
            var pSql;

            pIU = new InsertUpdate('private.LibraryPimMenu');
            pIU.Add('Name', ReadyString(vData.Name));
            pIU.Add('UserID', ReadyString(vData.UserID));
            pIU.Add('IsActive', CDec2(vData.IsActive));
            pIU.Add('LastUpdated', ReadyString(vData.LastUpdated));

            if (IsNew) {
                pIU.Add('ID', ReadyString(vData.ID));
                pSql = pIU.GetInsert();
            } else {
                pSql = pIU.GetUpdate('ID = ' + ReadyString(vData.ID));
            }

            Sql.ExecuteSql(pSql);

            return true;

        };
        // end of LibraryPimMenu

        // LibraryPimGroups
        this.SaveLibraryPimGroups = function (IsNew, vData) {

            var pIU;
            var pSql;

            pIU = new InsertUpdate('private.LibraryPimGroups');
            pIU.Add('ParentID', ReadyString(vData.ParentID));
            pIU.Add('Name', ReadyString(vData.Name));
            pIU.Add('IsActive', CDec2(vData.IsActive));
            pIU.Add('UserID', ReadyString(vData.UserID));
            pIU.Add('MenuID', ReadyString(vData.MenuID));
            pIU.Add('LastUpdated', ReadyString(vData.LastUpdated));

            if (IsNew) {
                pIU.Add('ID', ReadyString(vData.ID));
                pSql = pIU.GetInsert();
            } else {
                pSql = pIU.GetUpdate('ID = ' + ReadyString(vData.ID));
            }

            Sql.ExecuteSql(pSql);

            return true;

        };
        // end of LibraryPimGroups

        // LibraryPimGroupPims
        this.SaveLibraryPimGroupPims = function (IsNew, vData) {

            var pIU;
            var pSql;

            pIU = new InsertUpdate('private.LibraryPimGroupPims');
            pIU.Add('GroupID', ReadyString(vData.GroupID));
            pIU.Add('ProductID', ReadyString(vData.ProductID));
            pIU.Add('MenuID', ReadyString(vData.MenuID));
            pIU.Add('ShortName', ReadyString(vData.ShortName));
            pIU.Add('IsActive', CDec2(vData.IsActive));

            if (IsNew) {
                pIU.Add('ID', ReadyString(vData.ID));
                pSql = pIU.GetInsert();
            } else {
                pSql = pIU.GetUpdate('ID = ' + ReadyString(vData.ID));
            }

            Sql.ExecuteSql(pSql);

            return true;

        };
        // end of LibraryPimGroupPims

        // LibraryPimGroupFowsts
        this.SaveLibraryPimGroupFowsts = function (IsNew, vData) {

            var pIU;
            var pSql;

            pIU = new InsertUpdate('private.LibraryPimGroupFowsts');
            pIU.Add('FowstID', ReadyString(vData.FowstID));
            pIU.Add('MenuID', ReadyString(vData.MenuID));
            pIU.Add('IsActive', CDec2(vData.IsActive));

            if (IsNew) {
                pIU.Add('ID', ReadyString(vData.ID));
                pSql = pIU.GetInsert();
            } else {
                pSql = pIU.GetUpdate('ID = ' + ReadyString(vData.ID));
            }

            Sql.ExecuteSql(pSql);

            return true;

        };
        // end of LibraryPimGroupFowsts
    });