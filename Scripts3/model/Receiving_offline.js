define([
    'common',
    '/Scripts3/Offline/Browser/LibraryFowst.js?crc='
], function() {

    CreateStringObject('App.model.Receiving_offline', null, function() {
        
        var pOut = {};

        pOut.GetFowst = function(vParams) {
            /***
            vParams = {
                successFiltered: function(vData) {
                },
                success: function(vData) {
                }
            };
            ***/

            var pData;

            if (Web.HandleOrganization() == false)
                return;

            pData = OfflineBrowser.LibraryFowst.GetData({
                FilterFowst: 1
            });
            vParams.successFiltered({Results: pData});

            pData = OfflineBrowser.LibraryFowst.GetData({
                IsReceiving: 1
            });
            vParams.success({Results: pData});

        };

        pOut.Save = function(vParams) {
            /***
            vParams = {
                data: {},
                success: function(vData) {
                }
            };
            ***/

            var pIU;
            var pSql;
            var pData = vParams.data;
            var pIsNew = CBool(pData.IsNew);
            var pID = ReadyGuid(pData.ID);
            var pDR;
            var pQuantity;
            var pQuantityBad;
            var pStockID;
            var pTotal = 0;
            var pNum;

            Business.ZRead.HandleEnforceZRead();

            if (!IsGuid(pID)) {
                pID = ReadyGuid(NewID());
            }

            if (Web.HandleOrganization() == false)
                return;
            if (CStr2(Web.PeopleUserID()) == '')
                throw new Error('Web.PeopleUserID() must not be blank.');

            if (!CBool(pData.IsDraft) && IsGuid(pID)) {
                //finalize and save from cache to private db
                pIsNew = true;
            }

            pIU = new InsertUpdate((CBool(pData.IsDraft) ? 'cache' : 'private') + '.TransReceiving');
            if (pIsNew) {                
                pIU.Add('ID', ReadyString(pID));
            }
            if (vParams.IsRefund)
                pData.IsReceiving = 0;
            pIU.Add('IsReceiving', pData.IsReceiving);
            if (CStr2(pData.ReceivingPlace).trim() == '') {
                NewMessageBox('ReceivingPlaceID is blank. Please reload page then continue the transaction.', 'Invalid data');
                return;
            }
                
            pIU.Add('ReceivingPlaceID', ReadyString(pData.ReceivingPlace));

            pIU.Add('SupplierID', ReadyString(pData.SourceOfSKU));

            pIU.Add('DateReceived', ReadyDate(pData.DocumentDate));
            pIU.Add('DocumentDate', ReadyDate(pData.DocumentDate));
            pIU.Add('DocumentType', ReadyString(pData.DocumentType));
            pIU.Add('DocumentNo', ReadyString(pData.DocumentNo));
            pIU.Add('PONo', ReadyString(pData.PONo));
            pIU.Add('InvoiceID', ReadyString(pData.InvoiceID));
            pIU.Add('IsRefund', CInt2(pData.IsRefund));

            pIU.Add('Is3PL', CInt2(pData.Is3PL));
            pIU.Add('IsDraft', CInt2(pData.IsDraft));
            
            if (pData.ShipperCompany != undefined) {
                pIU.Add('ShipperID', ReadyString(pData.ShipperCompany));
                pIU.Add('WaybillNumber', ReadyString(pData.BLNo));
                pIU.Add('DatePrepared', ReadyDate(pData.DatePrepared));

                pIU.Add('VesselID', ReadyString(pData.Vessel));
                pIU.Add('VoyageNumber', ReadyString(pData.VoyageNumber));
                pIU.Add('InvoiceDate', ReadyDate(pData.InvoiceDate));
                pIU.Add('InvoiceNumber', ReadyString(pData.InvoiceNo));
                pIU.Add('FreightAndHandling', CDec2(pData.FAndH));
                pIU.Add('Arrastre', CDec2(pData.Arrastre));
                pIU.Add('Total', CDec2(pData.Total));
                pIU.Add('TotalCC', CInt2(pData.TotalCC));
                pIU.Add('TotalCBM', CInt2(pData.TotalCBM));
                pIU.Add('TotalWeightInKg', CInt2(pData.TotalWeightInKg));
                pIU.Add('TotalCostOfBundle', CDec2(pData.TotalCostBundle));
                pIU.Add('Bundles', ReadyString(pData.Bundles));
            }

            if (!CBool(pData.IsReceiving)) {
                pIU.Add('IsResaleable', CInt2(pData.IsResaleable));
                pIU.Add('ResaleableReason', ReadyString(pData.IsResaleableAnswer));
                pIU.Add('ResaleableRemarks', ReadyString(pData.IsResaleableRemarks));
            }

            if (pData.POID != '') {
                pIU.Add('POID', ReadyString(pData.POID));
            }

            if (pData.ReceiveInOrgID != '') {
                pIU.Add('ReceiveInOrgID', ReadyString(pData.ReceiveInOrgID));
            }

            if (pData.ReceiveInOutlet != '') {
                pIU.Add('ReceiveInOutlet', ReadyString(pData.ReceiveInOutlet));
            }

            if (pData.ReceiveInDepartment != '') {
                pIU.Add('ReceiveInDepartment', ReadyString(pData.ReceiveInDepartment));
            }

            if (pData.ReceiveFromOutlet != '') {
                pIU.Add('ReceiveFromOutlet', ReadyString(pData.ReceiveFromOutlet));
            }

            if (pData.ReceiveFromDepartment != '') {
                pIU.Add('ReceiveFromDepartment', ReadyString(pData.ReceiveFromDepartment));
            }

            pIU.Add('UserID', ReadyString(Web.PeopleUserID()));
            pIU.Add('DeviceID', ReadyString(Web.DeviceID()));

            pIU.Add('LastUpdated', proj.GetZReadDateSql());
            pIU.Add('TransportedBy', ReadyString(pData.TransportedBy));

            if (CBool(pData.IsFromTransferOut)) {
                pIU.Add('TransferOutToReceivingID', ReadyString(pData.ID));
            }
            
            Sql.BeginTransaction();

            if (pIsNew) {
                if (CBool(pData.IsDraft)) {
                    //draft
                    pIU.Add('ReceivingNo', 0);
                } else {
                    //finalized
                    pNum = pOut.GetNextNumber(pData.ReceivingPlace, pData.IsReceiving);
                    pOut.UpdateCounter(pNum, pData.ReceivingPlace, pData.IsReceiving);
                    pIU.Add('ReceivingNo', pNum);

                    var pDeviceFowstTransactionNumber = Business.TransInvoice.GetCounter({
                        FieldName: 'DeviceFowstTransactionNumber',
                        DontUseTableCounter: true
                    }) + 1;
                    pIU.Add('DeviceFowstTransactionNumber', pDeviceFowstTransactionNumber);

                    Business.TransInvoice.UpdateCounter(pDeviceFowstTransactionNumber, {
                        FieldName: 'DeviceFowstTransactionNumber',
                        DontUseTableCounter: true
                    });
                }                
                pSql = pIU.GetInsert();
            } else {                
                if (!CBool(pData.IsDraft)) {
                    //finalized
                    pNum = pOut.GetNextNumber(pData.ReceivingPlace, pData.IsReceiving);
                    pOut.UpdateCounter(pNum, pData.ReceivingPlace, pData.IsReceiving);
                    pIU.Add('ReceivingNo', pNum);

                    var pDeviceFowstTransactionNumber = Business.TransInvoice.GetCounter({
                        FieldName: 'DeviceFowstTransactionNumber',
                        DontUseTableCounter: true
                    }) + 1;
                    pIU.Add('DeviceFowstTransactionNumber', pDeviceFowstTransactionNumber);

                    Business.TransInvoice.UpdateCounter(pDeviceFowstTransactionNumber, {
                        FieldName: 'DeviceFowstTransactionNumber',
                        DontUseTableCounter: true
                    });
                }
                pSql = pIU.GetUpdate('ID = ' + ReadyString(pID));
            }
                

            pSql = Sql.ExecuteSql(pSql); // store the number of affected records to pSql for debugging. expected result = 1

            if (CStr2(pData.POID) != '' && CBool(pData.IsDraft) == false) {
                pSql = 'Update TransInvoice set DateImported = null, ReceivingID = ' + ReadyString(pID) + ' where ID = ' + ReadyString(pData.POID)
                pSql = Sql.ExecuteSql(pSql);
            }

            if (!pIsNew) {
                pSql = 'DELETE FROM private.TransReceivingStock WHERE ReceivingID = ' + ReadyString(pID);
                pSql = Sql.ExecuteSql(pSql); // store the number of affected records to pSql for debugging. expected result = 1

                var pSql = 'Delete from TransReceivingStockSerials WHERE ReceivingID = ' + ReadyString(pID);
                Sql.ExecuteSql(pSql);
            }

            if (CBool(pData.IsDraft)) {
                Sql.Execute('DELETE FROM cache.TransReceivingStockCountPerUom WHERE ReceivingID = ' + ReadyString(pID))
                for (var i in pData.Items) {
                    pIU = new InsertUpdate('cache.TransReceivingStockCountPerUom');

                    pIU.Add('ID', ReadyString(NewID()))
                    pIU.Add('ReceivingID', ReadyString(pID))
                    pIU.Add('ProductID', ReadyString(pData.Items[i].ProductID))
                    pIU.Add('UnitMeasureID', ReadyString(pData.Items[i].ID))
                    pIU.Add('Count', ReadyString(pData.Items[i].Count))

                    Sql.Execute(pIU.GetInsert())
                }
            } else {
                for (var pIndex = 0; pIndex <= pData.Items.length - 1; pIndex++) {
                    pDR = pData.Items[pIndex];
                    pQuantity = CDec2(pDR.Quantity);
                    pQuantityBad = CDec2(pDR.QuantityBad);
                    Android.AddLog('STOCK RECEIVING: ' + JSON.stringify(pDR));
                    if (pStockID == undefined)
                        pStockID = pDR.ID;
                    else {
                        // vb code:
                        // AddToInventoryLedger(ID, StockID, Total)

                        pStockID = pDR.ID;
                        pTotal = 0;
                    }

                    pIU = new InsertUpdate('TransReceivingStock');

                    if ((pQuantity > 0)
                        || (pQuantityBad > 0)) {

                        pIU.Add('ID', ReadyString(NewID()));
                        pIU.Add('ReceivingID', ReadyString(pID));
                        pIU.Add('ProductID', ReadyString(pDR.ID));
                        pIU.Add('QuantityGood', pQuantity);
                        pIU.Add('ActualQuantityGood', pDR.ActualQuantity);
                        pIU.Add('UnitMeasureGood', ReadyString(pDR.UnitMeasureID));
                        pIU.Add('QuantityDamaged', pQuantityBad);
                        pIU.Add('ActualQuantityDamaged', pDR.ActualQuantity);
                        pIU.Add('UnitMeasureDamaged', ReadyString(pDR.UnitMeasureID));
                        pIU.Add('BadOrderReason', ReadyString(pDR.BadOrderReason));
                        pIU.Add('AdditionalInfo', ReadyString(pDR.AdditionalInfo));
                        pIU.Add('IsResaleable', pDR.IsResaleable ? 1 : 0);
                        pIU.Add('IsShortLanded', pData.IsShortLanded ? 1 : 0);
                        pIU.Add('IsOverLanded', pData.IsOverLanded ? 1 : 0);
                        pSql = pIU.GetInsert();
                        Sql.ExecuteSql(pSql);

                        pTotal += CDec2(pQuantity);

                        if (pData.Items[pIndex].ProductSerials != null)
                            for (var pSerialIndex = 0; pSerialIndex <= pData.Items[pIndex].ProductSerials.length - 1; pSerialIndex++) {
                                pIU = new InsertUpdate('TransReceivingStockSerials');

                                pIU.Add('ID', ReadyString(NewID()));
                                pIU.Add('ReceivingID', ReadyString(pID));
                                pIU.Add('ProductID', ReadyString(pDR.ID));
                                pIU.Add('SerialNumber', ReadyString(pData.Items[pIndex].ProductSerials[pSerialIndex].SerialNo));

                                pSql = pIU.GetInsert();
                                Sql.ExecuteSql(pSql);
                            }

                        if (!CBool(pData.IsDraft)) {
                            if (!CBool(pData.IsReceiving)) {
                                Business.TransInventoryLedger.Add(pDR.ID, pData.ReceivingPlace, pID, 'TransferOut', Web.PeopleUserID(), pTotal * -1, pDR.Balance, pDR.BinID, undefined, undefined, undefined, undefined);
                            } else {
                                Business.TransInventoryLedger.Add(pDR.ID, pData.ReceivingPlace, pID, 'Receiving', Web.PeopleUserID(), pTotal, pDR.Balance, pDR.BinID, undefined, undefined, undefined, undefined);
                            }
                        }
                    }

                } // END for (var pIndex = 0; pIndex <= pData.Items.length - 1; pIndex++) {

                Sql.Execute('DELETE FROM cache.TransReceiving WHERE ID = ' + ReadyString(pID));
                Sql.Execute('DELETE FROM cache.TransReceivingStockCountPerUom WHERE ReceivingID = ' + ReadyString(pID));
            }          
            
            Sql.CommitTransaction();

            return pNum + '|' + pID;
        }; // END pOut.Save = function(vParams) {

        pOut.GetReceivingOrTransfersInfo = function (vParams) {

            var pFinder = OfflineBrowser.Base().CreateFinder(vParams);
            var pFields = pFinder.Fields;
            var pField;
            var pNonZero;
            var pFOWSTID;

            pField = pFields.Add('ID');
            pField.Expression = 'TransReceiving.ID';
            pField.Searchable = false;

            pField = pFields.Add('ReceivingNo');
            pField = pFields.Add('IsReceiving');

            pField = pFields.Add('ReceivingPlaceID');
            pField.Expression = 'ReceivingPlaceID';
            pField.Searchable = false;

            pField = pFields.Add('ReceivingPlaceFowst');
            pField.Expression = 'LibraryOrganizationFOWST.Name';

            pField = pFields.Add('ReceivingPlace');
            pField.Expression = 'LibraryOrganization_2.CompanyName';

            pField = pFields.Add('SourceOfSKU1ID')
            pField.Expression = 'LibraryOrganizationFowst_1.ID';

            pField = pFields.Add('SourceOfSKU1')
            pField.Expression = 'LibraryOrganizationFowst_1.Name';

            pField = pFields.Add('SourceOfSKU2ID')
            pField.Expression = 'LibraryOrganization_1.ID';

            pField = pFields.Add('SourceOfSKU2')
            pField.Expression = 'LibraryOrganization_1.CompanyName';

            pField = pFields.Add('DateReceived')
            pField = pFields.Add('DocumentDate')
            pField = pFields.Add('DocumentType')
            pField = pFields.Add('DocumentNo')
            pField = pFields.Add('POID')
            pField = pFields.Add('TransportedBy')
            pField = pFields.Add('PONo')

            pField = pFields.Add('ReceiveInOrgID');
            pField.Expression = 'TransReceiving.ReceiveInOrgID';

            pField = pFields.Add('ReceiveInOutlet');
            pField.Expression = 'TransReceiving.ReceiveInOutlet';

            pField = pFields.Add('ReceiveInDepartment');
            pField.Expression = 'TransReceiving.ReceiveInDepartment';

            pField = pFields.Add('ReceiveFromOutlet');
            pField.Expression = 'TransReceiving.ReceiveFromOutlet';

            pField = pFields.Add('ReceiveFromDepartment');
            pField.Expression = 'TransReceiving.ReceiveFromDepartment';

            pField = pFields.Add('DatePrepared')
            pField = pFields.Add('Is3PL')
            pField = pFields.Add('ShipperID')
            pField = pFields.Add('WaybillNumber')
            pField = pFields.Add('VesselID')
            pField = pFields.Add('VoyageNumber')
            pField = pFields.Add('InvoiceDate')
            pField = pFields.Add('InvoiceNumber')
            pField = pFields.Add('FreightAndHandling')
            pField = pFields.Add('Arrastre')
            pField = pFields.Add('Total')
            pField = pFields.Add('TotalCC')
            pField = pFields.Add('TotalCBM')
            pField = pFields.Add('TotalWeightInKg')
            pField = pFields.Add('TotalCostOfBundle')
            pField = pFields.Add('Bundles')

            pField = pFields.Add('Shipper');
            pField.Expression = "CASE WHEN LibraryOrganization.CompanyName = '' THEN LibraryOrganization.PersonFirstName || ' ' || LibraryOrganization.PersonMiddleName || ' ' || LibraryOrganization.PersonSurname ELSE LibraryOrganization.CompanyName END";

            //pFinder.SqlFrom = "FROM LibraryOrganizationFOWST INNER JOIN " +
            //          "TransReceiving ON LibraryOrganizationFOWST.ID = TransReceiving.ReceivingPlaceID LEFT JOIN " +
            //          "LibraryOrganization ON TransReceiving.ShipperID = LibraryOrganization.ID INNER JOIN " +
            //          "LibraryPeople ON TransReceiving.UserID = LibraryPeople.ID LEFT OUTER JOIN " +
            //          "LibraryOrganization AS LibraryOrganization_1 ON TransReceiving.SupplierID = LibraryOrganization_1.ID LEFT OUTER JOIN " +
            //          "LibraryOrganizationFOWST AS LibraryOrganizationFowst_1 ON TransReceiving.SupplierID = LibraryOrganizationFowst_1.ID "

            pFinder.SqlFrom = "FROM " + (CBool(vParams.IsDraft) ? 'cache.' : 'private.') + "TransReceiving LEFT JOIN " +
                      "LibraryOrganizationFOWST  ON LibraryOrganizationFOWST.ID = TransReceiving.ReceivingPlaceID LEFT JOIN " +
                      "LibraryOrganization ON TransReceiving.ShipperID = LibraryOrganization.ID INNER JOIN " +
                      "LibraryPeople ON TransReceiving.UserID = LibraryPeople.ID LEFT OUTER JOIN " +
                      "LibraryOrganization AS LibraryOrganization_1 ON TransReceiving.SupplierID = LibraryOrganization_1.ID LEFT OUTER JOIN " +
                      "LibraryOrganizationFOWST AS LibraryOrganizationFowst_1 ON TransReceiving.SupplierID = LibraryOrganizationFowst_1.ID " +
                      "LEFT JOIN LibraryOrganization  AS LibraryOrganization_2 ON LibraryOrganization_2.ID = TransReceiving.ReceivingPlaceID "

            pFinder.SqlWhere = "WHERE TransReceiving.ID = " + ReadyString(vParams.ID)

            pFinder.SqlOrder = "ORDER BY ReceivingNo desc"

            return pFinder.Requery();
        }

        pOut.GetReceivingDetails = function (vParams) {

            var pFinder = OfflineBrowser.Base().CreateFinder(vParams);
            var pFields = pFinder.Fields;
            var pField;
            var pNonZero;
            var pFOWSTID;

            pField = pFields.Add('ID');
            pField.Expression = 'TransReceivingStock.ProductID';
            pField.Searchable = false;

            pField = pFields.Add('ProductNo');
            pField.Expression = 'LibraryPIMProduct.ProductNo';
            
            pField = pFields.Add('PIMProductMeasurementQty');
            pField.Expression = 'LibraryPIMProductMeasurement.Quantity';

            pField = pFields.Add('UnitMeasureGood');
            pField = pFields.Add('ActualQuantityGood');

            pField = pFields.Add('ProductName');
            pField = pFields.Add('QuantityGood');
            pField = pFields.Add('QuantityDamaged');
            pField = pFields.Add('Label');
            pField = pFields.Add('BadOrderReason');
            pField = pFields.Add('IsResaleable');

            pField = pFields.Add('Quantity');
            pField.Expression = 'QuantityGood';
            pField = pFields.Add('QuantityBad');
            pField.Expression = 'QuantityDamaged';
            pField = pFields.Add('UnitMeasure');
            pField.Expression = 'Label';
            pFinder.SqlFrom = "FROM TransReceivingStock " +
                        "INNER JOIN librarypimproduct on librarypimproduct.id = TransReceivingStock.ProductID " +
                        "INNER JOIN LibraryPIMProductMeasurement on LibraryPIMProductMeasurement.ProductID = TransReceivingStock.ProductID " +
                        "INNER JOIN LibraryPIMMeasure ON LibraryPIMProductMeasurement.UnitMeasureID = LibraryPIMMeasure.ID AND LibraryPIMProductMeasurement.UnitMeasureID = TransReceivingStock.UnitMeasureGood " +
                        "INNER JOIN LibraryCountriesLanguageLabel ON LibraryPIMMeasure.MeasureNameID = LibraryCountriesLanguageLabel.ID"

            pFinder.SqlWhere = "WHERE TransReceivingStock.ReceivingID = " + ReadyString(vParams.ID)

            return pFinder.Requery();
        }

        pOut.FlushData = function() {
            SalesPaymentsExport_offline().Export({
                        OnUploaded: function () {
                    // FollowUp();
                }, // END OnUploaded
                OnDone: function (vData) {
                    if (vData.Result == 'ok')
                        Android.AddLog('Done exporting jsS9WMH9TS9A!');
                    else if (vData.Result == 'error')
                        Android.Toast(vData.Error);
                    App.Deferred.fulfill('KWXCETE5XN');
                } // END OnDone
            }); // END SalesPaymentsExport_offline().Export({
        }; // END FlushData

        pOut.GetReportHeaderInfo = function (vParams) {
            /***
            vParams = {
                successFiltered: function(vData) {
                },
                success: function(vData) {
                }
            };
            ***/

            var pData;

            if (Web.HandleOrganization() == false)
                return;
                        
        };

        pOut.GetNextNumber = function(vFowstID, vIsReceiving) {

            var pSql;
            var pNum, pNum2;
            var maxNumberInFile = 0;
            var mCounterFilename = '';

            if (CBool(vIsReceiving))
                mCounterFilename = 'Receiving' + vFowstID;
            else
                mCounterFilename = 'TransferOut' + vFowstID;
            
            var pFileText = Transfers.ReadFile(mCounterFilename + '.flg');

            if (pFileText != undefined) {
                maxNumberInFile = CInt2(pFileText) + 1;
            }

            //pSql = 'SELECT MAX(ReceivingNo)' + 
            //    ' FROM private.TransReceiving' +
            //    ' WHERE IsReceiving = ' + (CBool(vIsReceiving) ? 1 : 0); // enclose the CBool in parenthesis
            //pSql = Sql.ExecuteScalar(pSql);
            //pNum = CDec2(pSql) + 1;            
            pNum = 1;

            if (CBool(vIsReceiving))
                pSql = 'ReceivingNum';
            else
                pSql = 'ReceivingTransferNum';
            pSql = 'SELECT MAX(' + pSql + ')' +
                ' FROM private.LibraryDeviceFowst' +
                ' WHERE DeviceID = ' + ReadyString(Web.DeviceID()) +
                ' AND FowstID = ' + ReadyString(vFowstID);
            pSql = Sql.ExecuteScalar(pSql);
            pNum2 = CDec2(pSql) + 1;            

            pNum = Math.max(pNum, pNum2);

            if (maxNumberInFile > pNum) {
                pNum = maxNumberInFile;
            }

            return pNum;

        }; // END GetNextNumber

        pOut.UpdateCounter = function(vValue, vFowstID, vIsReceiving) {
            
            var pSql;
            var mCounterFilename = '';

            if (CBool(vIsReceiving)) {
                pSql = 'ReceivingNum';
                mCounterFilename = 'Receiving' + vFowstID;                
            }                
            else {
                pSql = 'ReceivingTransferNum';
                mCounterFilename = 'TransferOut' + vFowstID;
            }

            Transfers.WriteFile(mCounterFilename + '.flg', null, vValue);
            
            pSql = 'UPDATE private.LibraryDeviceFowst' + 
                ' SET ' + pSql + ' = ' + CDec2(vValue) +
                ' WHERE DeviceID = ' + ReadyString(Web.DeviceID()) +
                ' AND FowstID = ' + ReadyString(vFowstID) + 
                ' AND ' + pSql + ' < ' + CDec2(vValue);
            pSql = Sql.ExecuteSql(pSql);

            return pSql;

        };

        pOut.DuplicateTable = function () {
            var pSql = '';

            if (Sql.ExecuteScalar('PRAGMA cache.table_info(TransReceiving)') == undefined) {
                Sql.Execute("CREATE TABLE IF NOT EXISTS cache.TransReceiving \n" +
                    "( \n" +
                    "ID uniqueidentifier NOT NULL \n" +
                    ") ")
            }

            if (Sql.ExecuteScalar('PRAGMA cache.table_info(TransReceivingStock)') == undefined) {
                Sql.Execute("CREATE TABLE IF NOT EXISTS cache.TransReceivingStock \n" +
                    "( \n" +
                    "ID uniqueidentifier NOT NULL \n" +
                    ") ")
            }

            if (Sql.ExecuteScalar('PRAGMA cache.table_info(TransReceivingStockCountPerUom)') == undefined) {
                Sql.Execute("CREATE TABLE IF NOT EXISTS cache.TransReceivingStockCountPerUom \n" +
                    "( \n" +
                    "ID uniqueidentifier NOT NULL, \n" +
                    "ReceivingID uniqueidentifier NULL, \n" +
                    "ProductID uniqueidentifier NULL, \n" +
                    "UnitMeasureID uniqueidentifier NULL, \n" +
                    "Count text NULL \n" +
                    ") ")
            }

            var vRows = Sql.Execute('PRAGMA private.table_info(TransReceiving)').Rows;

            if (vRows.length > 0) {
                for (var i in vRows) {
                    var cnt = CInt2(Sql.ExecuteScalar("SELECT COUNT(*) FROM pragma_table_info('TransReceiving', 'cache') WHERE name = " + ReadyString(vRows[i].name)));
                    if (cnt == 0) {
                        Sql.Execute('ALTER TABLE cache.TransReceiving ADD COLUMN ' + vRows[i].name + ' ' + vRows[i].type);
                    }
                }
            }

            vRows = Sql.Execute('PRAGMA private.table_info(TransReceivingStock)').Rows;

            if (vRows.length > 0) {
                for (var i in vRows) {
                    var cnt = CInt2(Sql.ExecuteScalar("SELECT COUNT(*) FROM pragma_table_info('TransReceivingStock', 'cache') WHERE name = " + ReadyString(vRows[i].name)));
                    if (cnt == 0) {
                        Sql.Execute('ALTER TABLE cache.TransReceivingStock ADD COLUMN ' + vRows[i].name + ' ' + vRows[i].type);
                    }
                }
            }
        }

        pOut.GetCategories = function (GroupSelected) {
            var pSql = '';
            var pFinder = new Finder();
            var pFields = pFinder.Fields;
            var pField;

            pFinder.MaxRecords = 0
            pFinder.Distinct = true;

            pField = pFields.Add('Category');
            pField.Expression = 'LibraryPIMProductVariableFeatures001.C0020';

            pFinder.SqlFrom = 'FROM LibraryPIMProductVariableFeatures001 ' +
                'INNER JOIN private.LibraryOrganizationPIM on private.LibraryOrganizationPIM.ProductID = LibraryPIMProductVariableFeatures001.ProductID'

            if (GroupSelected == 'ByInventoryCountItems') {
                pField = pFields.Add('SortOrder');
                pField.Expression = '(SELECT SortOrder FROM LibraryPimCategories WHERE Category = LibraryPIMProductVariableFeatures001.C0020 LIMIT 1)';

                pFinder.SqlFrom += ' AND LibraryOrganizationPIM.IsInventoryCountItem = 1 '
            }

            pFinder.SqlWhere = 'WHERE LibraryPIMProductVariableFeatures001.ProductID NOT IN (Select ProductID from LibraryPIMProductParent where IFNULL(Jit, 0) = 1 AND IFNULL(IsSeperateIntoComponentDuringSalesReturns, 0)=1 ) '

            if (GroupSelected == 'ByInventoryCountItems') {
                pFinder.SqlOrder = 'Order By SortOrder asc'
            } else {
                pFinder.SqlOrder = 'Order By LibraryPIMProductVariableFeatures001.C0020 asc'
            }

            return pFinder.Requery();
        }

        pOut.GetAllProductCategories = function (fowstid) {
            var vCategories = pOut.GetCategories('ByInventoryCountItems').Rows;
            var pSql;

            for (var i in vCategories) {
                pSql = 'SELECT LibraryPimProduct.ID, ProductNo, ProductName, \n' +
                    ' --IFNULL((SELECT Status FROM cache.TransInventoryCountItemPerUom WHERE CountedByUser = ' + ReadyString(Web.PeopleUserID()) + ' AND FowstID = ' + ReadyString(fowstid) + ' AND ProductID = LibraryPimProduct.ID AND CountDate = ' + ReadyString($.format.date(new Date(), 'MM-dd-yyyy')) + ' ORDER BY UomState LIMIT 1), "Missing") as Status, \n' +
                    ' --(SELECT IFNULL(SUM(Count * Qty), "") FROM cache.TransInventoryCountItemPerUom WHERE CountedByUser = ' + ReadyString(Web.PeopleUserID()) + ' AND FowstID = ' + ReadyString(fowstid) + ' AND ProductID = LibraryPimProduct.ID AND UomState = "Full" AND CountDate = ' + ReadyString($.format.date(new Date(), 'MM-dd-yyyy')) + ') as Full, \n' +
                    ' --(SELECT IFNULL(SUM(Count * Qty), "") FROM cache.TransInventoryCountItemPerUom WHERE CountedByUser = ' + ReadyString(Web.PeopleUserID()) + ' AND FowstID = ' + ReadyString(fowstid) + ' AND ProductID = LibraryPimProduct.ID AND UomState = "Loose" AND CountDate = ' + ReadyString($.format.date(new Date(), 'MM-dd-yyyy')) + ') as Loose, \n' +
                    ' (SELECT DISTINCT LibraryCountriesLanguageLabel.Label ' +
                    '   FROM LibraryPIMProductMeasurement INNER JOIN LibraryPIMMeasure ON ' +
                    '   LibraryPIMProductMeasurement.UnitMeasureID = LibraryPIMMeasure.ID INNER JOIN  ' +
                    '   LibraryCountriesLanguageLabel ON LibraryPIMMeasure.MeasureNameID = LibraryCountriesLanguageLabel.ID   ' +
                    '   WHERE LibraryPIMProductMeasurement.ProductID = LibraryPimProduct.ID ' +
                    '   ORDER BY LibraryPIMProductMeasurement.Quantity DESC limit 1 ) as UnitMeasureName, \n' +
                    ' (SELECT DISTINCT LibraryPIMMeasure.ID ' +
                    '   FROM LibraryPIMProductMeasurement INNER JOIN LibraryPIMMeasure ON ' +
                    '   LibraryPIMProductMeasurement.UnitMeasureID = LibraryPIMMeasure.ID INNER JOIN  ' +
                    '   LibraryCountriesLanguageLabel ON LibraryPIMMeasure.MeasureNameID = LibraryCountriesLanguageLabel.ID   ' +
                    '   WHERE LibraryPIMProductMeasurement.ProductID = LibraryPimProduct.ID ' +
                    '   ORDER BY LibraryPIMProductMeasurement.Quantity DESC limit 1 ) as UnitMeasureID, \n' +
                    ' (SELECT DISTINCT LibraryPIMProductMeasurement.Quantity ' +
                    '   FROM LibraryPIMProductMeasurement INNER JOIN LibraryPIMMeasure ON ' +
                    '   LibraryPIMProductMeasurement.UnitMeasureID = LibraryPIMMeasure.ID INNER JOIN  ' +
                    '   LibraryCountriesLanguageLabel ON LibraryPIMMeasure.MeasureNameID = LibraryCountriesLanguageLabel.ID   ' +
                    '   WHERE LibraryPIMProductMeasurement.ProductID = LibraryPimProduct.ID ' +
                    '   ORDER BY LibraryPIMProductMeasurement.Quantity DESC limit 1 ) as ActualQuantityGood, \n' +
                    ' (SELECT DISTINCT LibraryCountriesLanguageLabel.Label ' +
                    '   FROM LibraryPIMProductMeasurement INNER JOIN LibraryPIMMeasure ON ' +
                    '   LibraryPIMProductMeasurement.UnitMeasureID = LibraryPIMMeasure.ID INNER JOIN  ' +
                    '   LibraryCountriesLanguageLabel ON LibraryPIMMeasure.MeasureNameID = LibraryCountriesLanguageLabel.ID   ' +
                    '   WHERE LibraryPIMProductMeasurement.ProductID = LibraryPimProduct.ID ' +
                    '   ORDER BY LibraryPIMProductMeasurement.Quantity limit 1 ) as MinUnitMeasureName, \n' +
                    ' (SELECT DISTINCT LibraryPIMMeasure.ID ' +
                    '   FROM LibraryPIMProductMeasurement INNER JOIN LibraryPIMMeasure ON ' +
                    '   LibraryPIMProductMeasurement.UnitMeasureID = LibraryPIMMeasure.ID INNER JOIN  ' +
                    '   LibraryCountriesLanguageLabel ON LibraryPIMMeasure.MeasureNameID = LibraryCountriesLanguageLabel.ID   ' +
                    '   WHERE LibraryPIMProductMeasurement.ProductID = LibraryPimProduct.ID ' +
                    '   ORDER BY LibraryPIMProductMeasurement.Quantity limit 1 ) as MinUnitMeasureID, \n' +
                    ' (SELECT DISTINCT LibraryPIMProductMeasurement.Quantity ' +
                    '   FROM LibraryPIMProductMeasurement INNER JOIN LibraryPIMMeasure ON ' +
                    '   LibraryPIMProductMeasurement.UnitMeasureID = LibraryPIMMeasure.ID INNER JOIN  ' +
                    '   LibraryCountriesLanguageLabel ON LibraryPIMMeasure.MeasureNameID = LibraryCountriesLanguageLabel.ID   ' +
                    '   WHERE LibraryPIMProductMeasurement.ProductID = LibraryPimProduct.ID ' +
                    '   ORDER BY LibraryPIMProductMeasurement.Quantity DESC limit 1 ) as MinActualQuantityGood, \n' +
                    ' (SELECT COUNT(ID) \n ' +
                    '   FROM LibraryPIMProductMeasurement \n' +
                    '   WHERE LibraryPIMProductMeasurement.ProductID = LibraryPimProduct.ID) as NoOfUnitMeasure \n'+
                    ' FROM LibraryPimProduct \n' +
                    ' INNER JOIN LibraryOrganizationPIM ON LibraryOrganizationPIM.ProductID = LibraryPimProduct.ID \n' +
                    ' WHERE IsInventoryCountItem = 1 AND LibraryPimProduct.ID IN (SELECT ProductID FROM LibraryPIMProductVariableFeatures001 WHERE C0020 = ' + ReadyString(vCategories[i].Category) + ') \n' +
                    ' ORDER BY ProductName ';

                vCategories[i].ID = NewID(); // random ID
                vCategories[i].Sku = Sql.Execute(pSql).Rows;
            }

            return vCategories;
        }

        return pOut;

    }); // END CreateStringObject('App.model.Receiving_offline', null, function() {

    App.model.Receiving_offline = App.model.Receiving_offline(); // singleton

}); // END require
