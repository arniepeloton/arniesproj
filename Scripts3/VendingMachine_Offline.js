define(['common',
    '/Scripts3/model/InventoryCount_Offline.js?crc=',
    '/Scripts3/model/Receiving_offline.js?v='],
    function () {
        var pSql = '';
        var defaultPriceLevelID, IsVAT;
        var InventoryCount = new App.model.InventoryCount_Offline();
        var arrDenom = [];
        window.VendingMachine = {};
        sessionStorage.IsVendingMachine = true;

        Sql.Execute("CREATE TABLE IF NOT EXISTS cache.CacheCashDenominationBreakdown \n" +
                "( \n" +
                "ID uniqueidentifier NOT NULL, \n" +
                "SalesDate date NULL, \n" +
                "LastInvoiceID uniqueidentifier NULL, \n" +
                "FiveCents int DEFAULT 0, \n" +
                "TwentyFiveCents int DEFAULT 0, \n" +
                "FiftyCents int DEFAULT 0, \n" +
                "One int DEFAULT 0, \n" +
                "Five int DEFAULT 0, \n" +
                "Ten int DEFAULT 0, \n" +
                "Twenty int DEFAULT 0, \n" +
                "Fifty int DEFAULT 0, \n" +
                "OneHundred int DEFAULT 0, \n" +
                "TwoHundred int DEFAULT 0, \n" +
                "FiveHundred int DEFAULT 0, \n" +
                "OneThousand int DEFAULT 0, \n" +
                "LastUpdated datetime DEFAULT 0 \n" +
            ") ")

        Sql.Execute("CREATE TABLE IF NOT EXISTS cache.CacheSoldItems \n" +
                "( \n" +
                "ID uniqueidentifier NOT NULL, \n" +
                "SalesDate datetime NULL, \n" +
                "FowstID uniqueidentifier NULL, \n" +
                "DeviceID uniqueidentifier NULL, \n" +
                "InvoiceID uniqueidentifier NULL, \n" +
                "ProductID uniqueidentifier NULL, \n" +
                "ProductCategory text NULL, \n" +
                "ProductNo text NULL, \n" +
                "ProductName text NULL, \n" +
                "UnitPrice text NULL, \n" +
                "OrderedQuantity text NULL, \n" +
                "UnitMeasure text NULL, \n" +
                "LastUpdated datetime NULL \n" +
            ") ")

        VendingMachine.NoOfMotors = 0;
        VendingMachine.NoOfBins = 0;
        VendingMachine.Denom;

        VendingMachine.GetMenu = function () {
            defaultPriceLevelID = Sql.ExecuteScalar("SELECT IFNULL(DefaultSellingPriceLevel, '01000012-0005-0000-0000-000000000000') as DefaultSellingPriceLevel FROM LibraryOrganizationFranchisee WHERE FranchiseeOrgFowstID = " + ReadyString(sessionStorage.FowstID) + " AND FranchisorOrgID = " + ReadyString(sessionStorage.OrgID)); // retail price

            var store = Sql.Execute('SELECT DefaultHotButtonTemplateID, IFNULL(IsVAT, 1) as IsVAT FROM LibraryOrganizationFOWST WHERE ID = ' + ReadyString(sessionStorage.FowstID)).Rows[0];
            var hbtemplateid = store.DefaultHotButtonTemplateID;
            IsVAT = CBool(store.IsVAT);

            if (!IsGuid(hbtemplateid)) {
                NewMessageBox('No menu template has been selected for this store. Please request assistance from Mobiz IT. Thank you!')
                return false;
            }

            var VDMModel = Sql.Execute('SELECT * FROM private.LibraryVendingMachines WHERE ID = (SELECT VDMModelID FROM LibraryPimMenu WHERE ID = ' + ReadyString(hbtemplateid) + ')').Rows;

            if (VDMModel.length == 0) {
                NewMessageBox('The default Hot Button template for this machine is not assigned to any machine model.');
                return false;
            } else {
                VendingMachine.NoOfMotors = CInt2(VDMModel[0].NoOfMotors);
                VendingMachine.NoOfBins = CInt2(VDMModel[0].NoOfBins);
            }

            var menucategory = SalesMenu_offline().GetHotButtonsData(hbtemplateid, '', defaultPriceLevelID, false, false).Data.Rows; //Sql.Execute('SELECT ID, Name FROM LibraryPimGroups WHERE IsActive = 1 AND MenuID = ' + ReadyString(hbtemplateid) + ' ORDER BY OrderNo').Rows;

            for (var i = 0; i < menucategory.length; i++) {
                var mcData = menucategory[i];

                var menusku = " SELECT grouppim.ProductID, ShortName, Price, pic.GroupImageID as FileName, IFNULL(MarketingText, '') as Description, VDMMotorNo, PlanogramTempID \n " +
                                ' , LibraryCountriesLanguageLabel.Label as UnitMeasure, LibraryPIMProductMeasurement.UnitMeasureID as UnitMeasureID \n' +
                                ' , (SELECT SUM(Balance) FROM LibraryFowstBinProduct WHERE Balance > 0 ProductID = grouppim.ProductID AND FowstID = ' + ReadyString(sessionStorage.FowstID) + ') as Balance \n' +
                                " FROM LibraryPimGroupPims as grouppim \n " +
                                " INNER JOIN LibraryPIMProductPriceLevel as prclvl ON prclvl.ProductID = grouppim.ProductID \n " +
                                " INNER JOIN LibraryPIMProduct as pim ON pim.ID = grouppim.ProductID \n " +
                                " AND prclvl.OrganizationID = " + ReadyString(sessionStorage.OrgID) + " AND prclvl.PriceLevelID = " + ReadyString(defaultPriceLevelID) + "\n " +
                                " LEFT JOIN LibraryPIMPictures as pic ON pic.ProductID = grouppim.ProductID AND IsDefault = 1 AND Size = 'big' \n" +
                                ' LEFT JOIN LibraryPIMProductMeasurement ON grouppim.ProductID = LibraryPIMProductMeasurement.ProductID \n' +
                                ' LEFT JOIN LibraryPIMMeasure ON LibraryPIMProductMeasurement.UnitMeasureID = LibraryPIMMeasure.ID \n' +
                                ' LEFT JOIN LibraryCountriesLanguageLabel ON LibraryPIMMeasure.MeasureNameID = LibraryCountriesLanguageLabel.ID \n' +
                                " WHERE grouppim.IsActive = 1 AND GroupID = " + ReadyString(mcData.ID) + " \n " +
                                " ORDER BY grouppim.OrderNo \n "

                mcData.Sku = SalesMenu_offline().GetHotButtonsData(hbtemplateid, mcData.ID, defaultPriceLevelID, false, false).Data.Rows // Sql.Execute(menusku).Rows;

                for (var s in mcData.Sku) {
                    mcData.Sku[s].RemainingQty = Sql.ExecuteScalar('SELECT SUM(Balance) FROM LibraryFowstBinProduct WHERE Balance > 0 AND IsDefault = 1 AND ProductID = ' + ReadyString(mcData.Sku[s].ProductID) + ' AND FowstID = ' + ReadyString(sessionStorage.FowstID))

                    if (CBool(mcData.Sku[s].IsUseUOM)) {
                        mcData.Sku[s].ListOfUom = Stock_offline().GetInfoPos({
                            "ProductID": mcData.Sku[s].ProductID,
                            "ModifierGroupID": "",
                            "CustomerID": sessionStorage.CustomerOrganizationID,
                            "OrderID": null,
                            "NewCartMode": false,
                            "PosMode": "pos"
                        }).UnitMeasures.Rows;
                    }
                }
            }
            console.log(menucategory)
            arrDenom = Sql.Execute("select sys.*, LibraryCountriesLanguageLabel.Label as UnitMeasure, " +
                " LibraryPIMProductMeasurement.UnitMeasureID as UnitMeasureID, LibraryFowstBin.ID as BinID, " +
                " LibraryFowstBinProduct.Balance, LibraryPIMProduct.ProductNo, LibraryPIMProduct.ProductName " +
                " from private.SystemDefaults as sys " +
                " LEFT JOIN LibraryPIMProductMeasurement ON sys.DefaultValue = LibraryPIMProductMeasurement.ProductID " +
                " LEFT JOIN LibraryPIMProduct ON LibraryPIMProduct.ID = LibraryPIMProductMeasurement.ProductID " +
                " LEFT JOIN LibraryPIMMeasure ON LibraryPIMProductMeasurement.UnitMeasureID = LibraryPIMMeasure.ID " +
                " LEFT JOIN LibraryCountriesLanguageLabel ON LibraryPIMMeasure.MeasureNameID = LibraryCountriesLanguageLabel.ID " +
                " LEFT JOIN LibraryFowstBin ON LibraryFowstBin.FowstID = " + ReadyString(sessionStorage.FowstID) + " AND Name = substr(sys.Description, instr(sys.Description, ' ') + 1) " +
                " LEFT JOIN LibraryFowstBinProduct ON LibraryFowstBinProduct.BinID = LibraryFowstBin.ID " +
                " WHERE sys.Description IN ('VDM 25_cent', 'VDM 1_peso', 'VDM 5_peso', 'VDM 10_peso', 'VDM 20_pesos', 'VDM 50_peso', 'VDM 100_peso', 'VDM 200_peso', 'VDM 500_peso', 'VDM 1000_peso') AND LibraryPIMProductMeasurement.Quantity = 1 order by lastupdated desc").Rows;
            VendingMachine.Denom = arrDenom;
            console.log(arrDenom)
            return menucategory;
        }

        VendingMachine.SaveCart = function (vData, callback) {
            var arrBinProduct = [];
            console.log(vData)
            //return true
            try {
                var pIU = new InsertUpdate('TransInvoice');
                var pInvoiceID = NewID();
                var pInvoiceDate = proj.GetZReadDate();
                var pInvoiceNumber = 0;
                var pDeviceFowstTransactionNumber = 0;

                if (CBool(vData.SaveAsDraft)) {
                    pInvoiceNumber = 0;
                    pDeviceFowstTransactionNumber = 0;
                } else {
                    pInvoiceID = vData.InvoiceID;
                    pInvoiceNumber = Business.TransInvoice.GetCounter() + 1;
                    pDeviceFowstTransactionNumber = Business.TransInvoice.GetCounter({
                        FieldName: 'DeviceFowstTransactionNumber',
                        DontUseTableCounter: true
                    }) + 1;
                }

                pIU.Add('InvoiceNumber', pInvoiceNumber);
                pIU.Add('DeviceFowstTransactionNumber', pDeviceFowstTransactionNumber);
                pIU.Add('InvoiceDate', ReadyDate(pInvoiceDate));
                pIU.Add('FowstID', ReadyString(sessionStorage.FowstID));
                pIU.Add('DeviceID', ReadyString(Web.DeviceID()));
                pIU.Add('CustomerID', ReadyString(sessionStorage.CustomerOrganizationID));
                pIU.Add('LastUpdated', proj.SqliteNow);
                pIU.Add('DeliveryDate', proj.SqliteNow);
                pIU.Add('DateImported', 'null');
                pIU.Add('ID', ReadyString(pInvoiceID));
                pIU.Add('UserID', ReadyString(Web.PeopleUserID()));
                pIU.Add('IsPrinted', 0);
                pIU.Add('Vatable', vData.TotalAmountDue / (1 + (CDec2(12) / 100)));
                pIU.Add('VATExempt', 0);
                pIU.Add('VAT', vData.TotalAmountDue - (vData.TotalAmountDue / (1 + (CDec2(12) / 100))));
                pIU.Add('ForegoneVAT', 0);
                pIU.Add('SCDiscount', 0);
                pIU.Add('SCDiscountExempt', 0);
                pIU.Add('TotalNonSCDiscount', 0);
                pIU.Add('AmountDueBeforeCharges', vData.TotalAmountDue);
                pIU.Add('AmountDueNetofCharges', vData.TotalAmountDue + 0); //vData.TotalAmountDue + CDec2(GetInvoiceCharges(pInvoiceID))

                var pDtFowst;
                var pIncrementedInt;

                var pTransNum = $.format.date(pInvoiceDate, 'yyMMdd');
                pDtFowst = Business.LibraryOrganizationFOWST.GetData(sessionStorage.FowstID);
                if (pDtFowst.Rows.length > 0)
                    pTransNum += pDtFowst.Rows[0].Code;
                pIncrementedInt = Business.TransInvoice.GetDatedCounterFileContent(sessionStorage.FowstID, Web.DeviceID(), pInvoiceDate) + 1;
                pTransNum += $.format.number(pIncrementedInt, '000000');
                Business.TransInvoice.UpdateDatedCounterFile(sessionStorage.FowstID, Web.DeviceID(), pInvoiceDate, pIncrementedInt);
                pIU.Add('TransNum', ReadyString(pTransNum));
                var pSql;

                if (CBool(vData.SaveAsDraft)) {
                    pSql = pIU.GetInsert();
                } else {
                    pSql = pIU.GetUpdate(' ID = ' + ReadyString(pInvoiceID))
                }

                Sql.BeginTransaction();
                Sql.ExecuteSql(pSql);

                if (!CBool(vData.SaveAsDraft)) {
                    var pCustPayID = NewID();
                    pIU = new InsertUpdate('private.TransCustomerPayment');

                    pIU.Add('ReceiptType', ReadyString("OR"));
                    pIU.Add('ReceiptNumber', "(SELECT IFNULL(devfowst.POSLastORNo,0) + 1 FROM private.LibraryDeviceFowst devfowst WHERE devfowst.FowstID = " + ReadyString(sessionStorage.FowstID) + " AND devfowst.DeviceID = " + ReadyString(Web.DeviceID()) + ")");
                    pIU.Add('CustomerID', ReadyString(sessionStorage.CustomerOrganizationID));
                    pIU.Add('PaymentDate', ReadyDate(pInvoiceDate));
                    pIU.Add('CashAmount', CDec2(vData.CashAmount));
                    pIU.Add('DebitCardAmount', 0);
                    pIU.Add('CreditCardAmount', 0);
                    pIU.Add('CreditCardVat', 0);
                    pIU.Add('CheckPaymentAmount', 0);
                    pIU.Add('SRVAmount', 0);
                    pIU.Add('OtherTypeAmount', 0);
                    pIU.Add('OnCreditAmount', 0);
                    pIU.Add('ChargeAmount', 0);
                    pIU.Add('DiscountAmount', 0);
                    pIU.Add('TotalAmountDue', CDec2(vData.TotalAmountDue));
                    pIU.Add('AmountChange', "0");
                    pIU.Add('CustomerChangeCash', CDec2(vData.CustomerChangeCash));
                    pIU.Add('UserID', ReadyString(Web.PeopleUserID()));
                    pIU.Add('ID', ReadyString(pCustPayID));
                    pSql = pIU.GetInsert();
                    Sql.ExecuteSql(pSql);

                    pIU = new InsertUpdate('private.TransCustomerPaymentInvoices');
                    pIU.Add('CustomerPaymentID', ReadyString(pCustPayID));
                    pIU.Add('InvoiceID', ReadyString(pInvoiceID));
                    pIU.Add('InvoiceAmount', CDec2(vData.TotalAmountDue).toFixed(2));
                    pIU.Add('DiscountAmount', CDec2(0).toFixed(2)); //always 0 for VDM
                    pIU.Add('Balance', CDec2(vData.TotalAmountDue).toFixed(2));
                    pIU.Add('AmountPaid', (parseFloat(CDec2(vData.CashAmount)) - parseFloat(CDec2(vData.CustomerChangeCash))).toFixed(2));

                    pIU.Add('ID', ReadyString(NewID()));
                    pSql = pIU.GetInsert();

                    Sql.ExecuteSql(pSql);

                    Business.TransInvoice.UpdateCounter();
                }

                for (var i in vData.Items) {
                    var item = vData.Items[i];

                    //pSql = "select libraryfowstbinproduct.ID as BinProductID, libraryfowstbin.ID as BinID, libraryfowstbin.BinNo, Balance from libraryfowstbin \n" +
                    //    " inner join libraryfowstbinproduct ON libraryfowstbin.ID = libraryfowstbinproduct.BinID \n" +
                    //    " WHERE IsDefault = 1 AND libraryfowstbinproduct.IsActive = 1 AND productid = " + ReadyString(item.ProductID) +
                    //    " AND libraryfowstbinproduct.fowstid = " + ReadyString(sessionStorage.FowstID) + " AND Balance > 0 ORDER BY libraryfowstbin.BinNo ASC "
                    //var bin = Sql.Execute(pSql).Rows;
                    //var orderedQty = CDec2(item.OrderQty);
                    //var remainingQty = 0, totalBinQtySold = 0;

                    //for (var b in bin) {
                    //    var bindata = bin[b];

                    //    if (orderedQty > CDec2(bindata.Balance)) {
                    //        orderedQty = CDec2(bindata.Balance)
                    //    }

                    //    totalBinQtySold += orderedQty;
                    //    remainingQty = Math.abs(orderedQty - CDec2(item.OrderQty));

                    var pID = NewID();

                    pIU = new InsertUpdate('TransInvoiceItems');

                    pIU.Add('ID', ReadyString(pID));
                    pIU.Add('Quantity', CDec2(item.OrderQty));
                    pIU.Add('OrderedQuantity', CDec2(item.OrderQty));
                    pIU.Add('ActualQuantity', ReadyString(item.Quantity));
                    pIU.Add('UnitMeasure', ReadyString(item.UnitMeasure));
                    pIU.Add('UnitPrice', CDec2(item.DiscountedPrice));
                    pIU.Add('OriginalUnitPrice', CDec2(item.DiscountedPrice));
                    var pCost = Business.LibraryPIMProduct.GetLandedCost(item.ProductID, sessionStorage.OrgID);
                    if (pCost != undefined)
                        pIU.Add('Cost', pCost);
                    else
                        pIU.Add('Cost', 'null');
                    pIU.Add('PriceLevelID', ReadyString(defaultPriceLevelID));
                    pIU.Add('Subtotal', (CDec2(item.OrderQty)) * CDec2(pIU.GetItem('UnitPrice')) + (CDec2(item.TotalModifier)));
                    pIU.Add('TaxPercent', 12);

                    if (!CBool(IsVAT)) {
                        pIU.Add('VATableSale', '0');
                        pIU.Add('VATExemptSale', pIU.GetItem('Subtotal'));
                        pIU.Add('NonVATSale', '0');
                        pIU.Add('ZeroRatedSale', '0');
                        pIU.Add('TaxSales', '0');
                        pIU.Add('TaxAmount', '0');
                    } else {
                        pIU.Add('VATableSale', pIU.GetItem('Subtotal'));
                        pIU.Add('VATExemptSale', '0');
                        pIU.Add('NonVATSale', '0');
                        pIU.Add('ZeroRatedSale', '0');
                        pIU.Add('TaxSales', pIU.GetItem('Subtotal') / (1 + pIU.GetItem('TaxPercent') / 100));   // todo: round down to nearest 4 decimal
                        pIU.Add('TaxAmount', pIU.GetItem('Subtotal') - pIU.GetItem('TaxSales'));
                    }

                    if (CBool(vData.SaveAsDraft)) {
                        pIU.Add('InvoiceID', ReadyString(pInvoiceID));
                        pIU.Add('ProductID', ReadyString(item.ProductID));
                        pSql = pIU.GetInsert();
                        Sql.ExecuteSql(pSql);
                    } else {
                        pIU.Add('BinID', ReadyString(item.BinID));

                        var firstInvoiceItemID = CStr2(Sql.ExecuteScalar('SELECT ID FROM TransInvoiceItems WHERE InvoiceID = ' + ReadyString(pInvoiceID) + ' AND ProductID = ' + ReadyString(item.ProductID) + ' AND BinID IS NULL'));

                        if (IsGuid(firstInvoiceItemID)) {
                            pSql = pIU.GetUpdate(' ID = ' + ReadyString(firstInvoiceItemID));
                            Sql.ExecuteSql(pSql);
                        } else {
                            pIU.Add('InvoiceID', ReadyString(pInvoiceID));
                            pIU.Add('ProductID', ReadyString(item.ProductID));
                            pSql = pIU.GetInsert();
                            Sql.ExecuteSql(pSql);
                        }

                        Business.TransInventoryLedger.Add(item.ProductID, sessionStorage.FowstID, pID, 'Invoice', Web.PeopleUserID(), CDec2(item.OrderQty) * CDec2(item.Quantity), CDec2(item.Balance), item.BinID, 'Sale', undefined, undefined, undefined);

                        //arrBinProduct.push({
                        //    ProductID: item.ProductID,
                        //    ProductName: item.ProductName,
                        //    VDMMotorNo: bindata.BinNo,
                        //    OrderQty: orderedQty * CDec2(item.Quantity),
                        //    IsUseUOM: CBool(item.IsUseUOM)
                        //})

                        VendingMachine.ComputeBinProduct(item);
                        VendingMachine.CacheSoldItems(item, pInvoiceID, pInvoiceDate);
                    }

                    //    if (totalBinQtySold == CDec2(item.OrderQty)) break;

                    //    orderedQty = remainingQty;
                    //}                    

                    if (!CBool(vData.SaveAsDraft)) {
                        var mIsJIT;
                        pSql = " SELECT IFNULL(Jit, 0) AS Jit " +
                            " ,Instructions " +
                            " FROM LibraryPIMProductParent " +
                            " WHERE LibraryPIMProductParent.ProductID = " + ReadyString(item.ProductID) +
                            " AND LibraryPIMProductParent.OrganizationID = " + ReadyString(sessionStorage.OrgID)

                        mIsJIT = Sql.Execute(pSql);

                        if (mIsJIT.Rows.length > 0) {
                            if (CBool(mIsJIT.Rows[0].Jit)) {
                                Business.TransInventoryLedger.Add(item.ProductID, sessionStorage.FowstID,
                                    pID, 'Invoice', Web.PeopleUserID(), item.OrderQty * 1,
                                    undefined, undefined, undefined, 'Production', undefined, undefined, undefined);

                                var mContents;

                                pSql = 'SELECT LibraryPIMProductContents.ChildProductID ' +
                                    ' ,LibraryPIMProduct.ProductNo ' +
                                    ' ,LibraryPIMProduct.ProductName ' +
                                    ' ,LibraryPIMProductContents.UnitMeasureID ' +
                                    ' ,IFNULL(LibraryCountriesLanguageLabel.Label, LibraryPIMProductContents.UnitMeasure) AS UnitMeasure ' +
                                    ' ,LibraryPIMProductContents.Quantity ' +
                                    ' FROM LibraryPIMProductContents ' +
                                    ' JOIN LibraryPIMProduct ON LibraryPIMProduct.ID = LibraryPIMProductContents.ChildProductID ' +
                                    ' LEFT JOIN LibraryPIMMeasure ON LibraryPIMMeasure.ID = LibraryPIMProductContents.UnitMeasureID ' +
                                    ' LEFT JOIN LibraryCountriesLanguageLabel ON LibraryCountriesLanguageLabel.ID = LibraryPIMMeasure.MeasureNameID ' +
                                    ' WHERE LibraryPIMProductContents.ParentProductID = ' + ReadyString(item.ProductID) +
                                    ' AND LibraryPIMProductContents.OrganizationID = ' + ReadyString(sessionStorage.OrgID) +
                                    ' ORDER BY LibraryPIMProduct.ProductName';

                                mContents = Sql.Execute(pSql);

                                for (var j = 0; j <= mContents.Rows.length - 1; j++) {
                                    Business.TransInventoryLedger.Add(mContents.Rows[j].ChildProductID, sessionStorage.FowstID,
                                        pID, 'Invoice', Web.PeopleUserID(), mContents.Rows[j].Quantity * item.OrderQty * -1,
                                        undefined, undefined, undefined, undefined, undefined, undefined);
                                }
                            }
                        }
                    }
                }

                pIU = new InsertUpdate('TransVDMLogs');

                pIU.Add('InvoiceID', ReadyString(pInvoiceID));
                pIU.Add('DateImported', 'NULL');

                Sql.Execute(pIU.GetUpdate(' LogID = ' + ReadyString(sessionStorage.LogID)))

                if (!CBool(vData.SaveAsDraft)) {
                    if (!VendingMachine.SaveCash(vData, pInvoiceID)) {
                        throw new Error('SaveCash() error');
                    }
                    Business.TransInvoice.UpdateTotalAmount(pInvoiceID);
                    Business.TransInvoice.UpdateItemCount(pInvoiceID);
                    SalesPayment_offline().UpdateSalesTextFile(pInvoiceID);
                    SalesPayment_offline().UpdateZReadData(pInvoiceID);
                    SalesPayment_offline().UpdateSalesProductCount(pInvoiceID);

                    sessionStorage.HotButtonData = null; //clear this session variable to force to get latest available products
                }                

                Sql.CommitTransaction();
                
                callback({
                    InvoiceID: pInvoiceID,
                    Bin: arrBinProduct
                });
            } catch (e) {
                console.log('Saving error: ' + e.message)
                Sql.RollbackTransaction();
                MessageBox(e.message);
            }

            function PrevCashCount(denom, invoicedate) {
                return CDec2(Sql.ExecuteScalar("SELECT " + denom + " FROM cache.CacheCashDenominationBreakdown WHERE SalesDate = " + ReadyString($.format.date(invoicedate, 'MM-dd-yyyy'))))
            }
        }

        VendingMachine.CacheSoldItems = function (item , invoiceid, salesdate) {
            var pIU = new InsertUpdate('CacheSoldItems');

            pIU.Add('SalesDate', ReadyDate(salesdate));
            pIU.Add('FowstID', ReadyString(sessionStorage.FowstID));
            pIU.Add('DeviceID', ReadyString(Web.DeviceID()));
            pIU.Add('LastUpdated', proj.SqliteNow);
            pIU.Add('InvoiceID', ReadyString(invoiceid));
            pIU.Add('ProductID', ReadyString(item.ProductID));
            pIU.Add('ProductCategory', ReadyString(item.SKUCategory));
            pIU.Add('ProductNo', ReadyString(item.ProductNo));
            pIU.Add('ProductName', ReadyString(item.ProductName));
            pIU.Add('OrderedQuantity', CDec2(item.OrderQty));
            pIU.Add('UnitMeasure', ReadyString(item.UnitMeasure));
            pIU.Add('UnitPrice', CDec2(item.DiscountedPrice));

            if (CDec2(Sql.ExecuteSql(pIU.GetUpdate(' FowstID = ' + ReadyString(sessionStorage.FowstID) + ' \n' +
                ' AND DeviceID = ' + ReadyString(Web.DeviceID()) + '\n' +
                ' AND CAST(UnitPrice as Decimal) = ' + CDec2(item.DiscountedPrice) + '\n' +
                ' AND InvoiceID = ' + ReadyString(invoiceid) + '\n' +
                ' AND ProductID = ' + ReadyString(item.ProductID)))) == 0) {
                pIU.Add('ID', ReadyString(NewID()))
                Sql.ExecuteSql(pIU.GetInsert())
            }
        }

        VendingMachine.CheckBin = function (item) {
            pSql = "select libraryfowstbinproduct.ID as BinProductID, libraryfowstbin.ID as BinID, Balance \n" +
                " , (SELECT CAST(DispenserMotorNo as INTEGER) + (CASE WHEN DispenserType = 'Spring Coil' THEN 10 ELSE 0 END) FROM LibraryVendingMachinePlanogramTemplateList as plan WHERE plan.PlanogramTemplateID = " + ReadyString(item.PlanogramTempID) + " AND libraryfowstbin.BinNo = plan.MotorNo AND plan.ProductID = " + ReadyString(item.ProductID) +") as VDMMotorNo \n" +
                " --, (SELECT UUID FROM LibraryVendingMachinePlanogramTemplateList as plan INNER JOIN LibraryVendingMachineBoards as board ON board.ID = plan.BoardID WHERE plan.PlanogramTemplateID = " + ReadyString(item.PlanogramTempID) + " AND libraryfowstbin.BinNo = plan.MotorNo AND plan.ProductID = " + ReadyString(item.ProductID) +") as BoardUUID \n" +
                " from libraryfowstbin \n" +
                " inner join libraryfowstbinproduct ON libraryfowstbin.ID = libraryfowstbinproduct.BinID \n" +
                " WHERE IsDefault = 1 AND libraryfowstbinproduct.IsActive = 1 AND productid = " + ReadyString(item.ProductID) +
                " AND libraryfowstbinproduct.fowstid = " + ReadyString(sessionStorage.FowstID) + " AND Balance > 0 ORDER BY libraryfowstbin.BinNo ASC "
            var bin = Sql.Execute(pSql).Rows;

            for (var b in bin) {
                var bindata = bin[b];

                if (CDec2(item.OrderQty) > CDec2(bindata.Balance)) {
                    item.OrderQty = CDec2(bindata.Balance);
                }

                item.success($.extend(item, bindata));
                break;
            }
        }

        VendingMachine.ComputeBinProduct = function (item, isreceive) {
            var pIU = new InsertUpdate('LibraryFowstBinProduct');
            //Android.AddLog(JSON.stringify(item));
            if (CBool(isreceive)) {
                pIU.Add("Balance", CDec2(Math.abs((CDec2(item.OrderQty) * CDec2(item.Quantity)) + CDec2(item.Balance))));
            } else {
                pIU.Add("Balance", CDec2(Math.abs((CDec2(item.OrderQty) * CDec2(item.Quantity)) - CDec2(item.Balance))));
            }
            pIU.Add('LastUpdated', proj.SqliteNow);
            pIU.Add('DateImported', 'NULL')
            pIU.Add('MaxPerProduct', 1) // 1 para ma flush lang sya, not sure kung unsa basehan sa kani na column -- by Arnie 2-28-25
            pSql = pIU.GetUpdate(" ID = " + ReadyString(item.BinProductID));
            Sql.ExecuteSql(pSql);
        }

        VendingMachine.SaveCash = function (vData, pInvoiceID, callback) {
            try {
                if (vData.CashInsertedBreakdown != undefined) {
                    for (var i in vData.CashInsertedBreakdown) {
                        var cash = vData.CashInsertedBreakdown[i];
                        for (var d in arrDenom) {
                            var denom = JSON.parse(JSON.stringify(arrDenom[d]));

                            if (cash.Denom + '_peso' == arrDenom[d].Description.split(' ')[1]) {
                                var curBalance = Sql.ExecuteRow('SELECT Balance, ID FROM LibraryFowstBinProduct ' +
                                    ' WHERE ProductID = ' + ReadyString(denom.DefaultValue) +
                                    ' AND FOWSTID = ' + ReadyString(sessionStorage.FowstID) +
                                    ' AND BinID = ' + ReadyString(denom.BinID))

                                var pReceivingData = {
                                    IsNew: true,
                                    IsReceiving: 1,
                                    IsRefund: 0,
                                    ReceivingPlace: sessionStorage.FowstID,
                                    SourceOfSKU: 'CD71A0F7-08A9-4601-876E-5F13C064F873', // SupplierID = cash payment org id
                                    InvoiceID: pInvoiceID, // put your invoice ID here
                                    DateReceived: new Date(),
                                    DocumentDate: new Date(),                                    
                                    Items: [
                                        {
                                            ID: denom.DefaultValue,
                                            Quantity: cash.Count, // always positive
                                            ActualQuantity: 1,
                                            UnitMeasureID: denom.UnitMeasureID,
                                            BinID: denom.BinID,
                                            Balance: CDec2(curBalance.Balance)
                                        }
                                    ]
                                };
                                Android.AddLog('CASH INSERTED: ' + JSON.stringify(pReceivingData));
                                pReceivingData.data = pReceivingData;                                
                                App.model.Receiving_offline.Save(pReceivingData);                                

                                var binData = {
                                    OrderQty: cash.Count,
                                    Quantity: 1,
                                    Balance: curBalance.Balance,
                                    BinProductID: curBalance.ID
                                }
                                console.log(binData);
                                VendingMachine.ComputeBinProduct(binData, true)
                                break;
                            }
                        }

                        var pIU = new InsertUpdate('TransVDMLogs');

                        pIU.Add('InvoiceID', ReadyString(sessionStorage.InvoiceID));
                        pIU.Add('DateImported', 'NULL');
                        Sql.Execute(pIU.GetUpdate(' LogID = ' + ReadyString(sessionStorage.LogID)))
                    }
                }

                if (typeof callback == 'function') callback();

                return true;
            } catch (e) {
                Android.AddLog(e);
                return false;
            }
        }

        VendingMachine.CheckPasscode = function (desc, pin) {
            var IsValidPIN = Sql.Execute('SELECT ID, EmployeeID FROM LibraryVendingMachineAuthorizers WHERE PIN = ' + ReadyString(pin)).Rows;

            if (IsValidPIN.length > 0) {
                if (Sql.Execute('SELECT ID FROM LibraryVendingMachineAuthorizersAccessRights WHERE Description = ' + ReadyString(desc) + ' AND VDMAuthorizerID = ' + ReadyString(IsValidPIN[0].ID)).Rows.length > 0 || CStr2(desc) == '') {
                    VendingMachine.AddLog('Options', CStr2(desc) + ' - allowed', IsValidPIN[0].EmployeeID);
                    return 'allowed';
                } else {
                    VendingMachine.AddLog('Options', CStr2(desc) + ' - no access', IsValidPIN[0].EmployeeID);
                    return 'no access';
                }
            } else {
                return 'invalid';
            }
        }

        VendingMachine.GetBinPerProduct = function (arrOrderItems) {
            

            for (var i in arrOrderItems) {
                var item = arrOrderItems[i];

                pSql = "select libraryfowstbinproduct.ID as BinProductID, libraryfowstbin.ID as BinID, libraryfowstbin.BinNo, Balance from libraryfowstbin \n" +
                    " inner join libraryfowstbinproduct ON libraryfowstbin.ID = libraryfowstbinproduct.BinID \n" +
                    " WHERE IsDefault = 1 AND libraryfowstbinproduct.IsActive = 1 AND productid = " + ReadyString(item.ProductID) +
                    " AND libraryfowstbinproduct.fowstid = " + ReadyString(sessionStorage.FowstID) + " AND Balance > 0 ORDER BY libraryfowstbin.BinNo ASC "
                var bin = Sql.Execute(pSql).Rows;
                var orderedQty = CDec2(item.OrderQty);
                var remainingQty = 0, totalBinQtySold = 0;

                for (var b in bin) {
                    var bindata = bin[b];

                    if (orderedQty > CDec2(bindata.Balance)) {
                        orderedQty = CDec2(bindata.Balance)
                    }

                    totalBinQtySold += orderedQty;
                    remainingQty = Math.abs(orderedQty - CDec2(item.OrderQty));

                    if (totalBinQtySold == CDec2(item.OrderQty)) break;

                    orderedQty = remainingQty;
                }
            }
        }

        VendingMachine.GetGuides = function () {
            pSql = " SELECT TemplateID FROM LibraryVDMGuidesAssignedFowst WHERE FowstID = " + ReadyString(sessionStorage.FowstID);
            var templates = Sql.Execute(pSql).Rows;

            for (var i in templates) {
                pSql = " SELECT LibraryVDMGuides.*, DispensingType FROM LibraryVDMGuides INNER JOIN LibraryVDMGuidesTemplate ON LibraryVDMGuidesTemplate.ID = LibraryVDMGuides.GuideID WHERE GuideID = " + ReadyString(templates[i].TemplateID) + " ORDER BY DispensingType, OrderNo ASC"
                templates[i].Steps = Sql.Execute(pSql).Rows;
            }

            return templates;
        }

        VendingMachine.CashChange = function (data) {
            var param = {
                "action": "CASH_CHANGE",
                "uuid": "000078EE35E81F84",
                "coin1": 0, //0.25
                "coin2": 0, //1
                "coin3": 0, //5
                "coin4": 0 //20
            }

            var denominations = {
                20: 0, //no P10 change for now
                5: 0,
                1: 0,
                0.25: 0
            }
            //"25_cent": 0,
            //    "1_peso": 0,
            //        "5_peso": 0,
            //            "10_peso": 0
            var coins = ['VDM 25_cent', 'VDM 1_peso', 'VDM 5_peso', 'VDM 20_pesos',]; //, 'VDM 10_peso', 'VDM 50_peso', 'VDM 100_peso', 'VDM 200_peso', 'VDM 500_peso', 'VDM 100_peso'
            
            for (var d in arrDenom) {
                if (coins.indexOf(arrDenom[d].Description) > -1) {
                    denominations[(arrDenom[d].Description == 'VDM 25_cent' ? 0.25 : arrDenom[d].Description.match(/\d+(\.\d+)?/g).map(Number)[0])] = CDec2(Sql.ExecuteScalar('SELECT IFNULL(Balance, 0) from libraryfowstbinproduct where productid = ' + ReadyString(arrDenom[d].DefaultValue) + ' AND fowstid = ' + ReadyString(sessionStorage.FowstID) + ' AND BinID = ' + ReadyString(arrDenom[d].BinID)));
                    //var binid = Sql.ExecuteScalar('SELECT ID FROM LibraryFowstBin WHERE FowstID = ' + ReadyString(sessionStorage.FowstID) + ' AND Name = ' + ReadyString(arrDenom[d].Description.split(' ')[1]))
                    //denominations.find(o => (o.value == 0.25 ? 25 : o.value) === CDec2(arrDenom[d].Description.replace('VDM ', '').split('_')[0])).count = CDec2(Sql.ExecuteScalar('SELECT IFNULL(Balance, 0) from libraryfowstbinproduct where productid = ' + ReadyString(arrDenom[d].DefaultValue) + ' AND fowstid = ' + ReadyString(sessionStorage.FowstID) + ' AND BinID = ' + ReadyString(arrDenom[d].BinID)));
                }
            }

            var sortedDenominations = Object.keys(denominations).map(Number).sort((a, b) => b - a); //sort in desc order para ang pinaka dako ang una magamit.
            var remaining = data.Amount;            

            for (var denom of sortedDenominations) {
                var count = Math.min(Math.floor(remaining / denom), denominations[denom]);
                if (count > 0) {
                    if (denom != Math.floor(denom)) {
                        param['coin1'] = count;
                    }

                    if (denom == 1) param['coin2'] = count;
                    if (denom == 5) param['coin3'] = count;
                    if (denom == 20) param['coin4'] = count;
                    remaining -= count * denom;
                }
            }
            Android.AddLog("Denom Inventory: " + JSON.stringify(denominations));
            Android.AddLog("Change Breakdown: " + JSON.stringify(param));
            //return;

            // Check if we managed to break down the amount fully
            if (remaining > 0) {
                NewMessageBox("Unable to fully break down the amount with the available denominations.");
            } else {
                //return
                var denom = [], denomChange = [];
                for (var d in arrDenom) {
                    var curBalance = Sql.ExecuteRow('SELECT Balance, ID FROM LibraryFowstBinProduct ' +
                        ' WHERE ProductID = ' + ReadyString(arrDenom[d].DefaultValue) +
                        ' AND FOWSTID = ' + ReadyString(sessionStorage.FowstID) +
                        ' AND BinID = ' + ReadyString(arrDenom[d].BinID))

                    if (CDec2(param['coin1']) > 0 && arrDenom[d].Description == 'VDM 25_cent') {
                        denom.push({
                            ID: arrDenom[d].DefaultValue,
                            Quantity: CDec2(param['coin1']), // always positive
                            ActualQuantity: 1,
                            UnitMeasureID: arrDenom[d].UnitMeasureID,
                            BinID: arrDenom[d].BinID,
                            Balance: CDec2(curBalance.Balance)
                        })

                        denomChange.push({
                            OrderQty: CDec2(param['coin1']),
                            Quantity: 1,
                            Balance: curBalance.Balance,
                            BinProductID: curBalance.ID
                        })
                    }

                    if (CDec2(param['coin2']) > 0 && arrDenom[d].Description == 'VDM 1_peso') {
                        denom.push({
                            ID: arrDenom[d].DefaultValue,
                            Quantity: CDec2(param['coin2']), // always positive
                            ActualQuantity: 1,
                            UnitMeasureID: arrDenom[d].UnitMeasureID,
                            BinID: arrDenom[d].BinID,
                            Balance: CDec2(curBalance.Balance)
                        })

                        denomChange.push({
                            OrderQty: CDec2(param['coin2']),
                            Quantity: 1,
                            Balance: curBalance.Balance,
                            BinProductID: curBalance.ID
                        })
                    }

                    if (CDec2(param['coin3']) > 0 && arrDenom[d].Description == 'VDM 5_peso') {
                        denom.push({
                            ID: arrDenom[d].DefaultValue,
                            Quantity: CDec2(param['coin3']), // always positive
                            ActualQuantity: 1,
                            UnitMeasureID: arrDenom[d].UnitMeasureID,
                            BinID: arrDenom[d].BinID,
                            Balance: CDec2(curBalance.Balance)
                        })

                        denomChange.push({
                            OrderQty: CDec2(param['coin3']),
                            Quantity: 1,
                            Balance: curBalance.Balance,
                            BinProductID: curBalance.ID
                        })
                    }

                    if (CDec2(param['coin4']) > 0 && arrDenom[d].Description == 'VDM 20_peso') {
                        denom.push({
                            ID: arrDenom[d].DefaultValue,
                            Quantity: CDec2(param['coin4']), // always positive
                            ActualQuantity: 1,
                            UnitMeasureID: arrDenom[d].UnitMeasureID,
                            BinID: arrDenom[d].BinID,
                            Balance: CDec2(curBalance.Balance)
                        })

                        denomChange.push({
                            OrderQty: CDec2(param['coin4']),
                            Quantity: 1,
                            Balance: curBalance.Balance,
                            BinProductID: curBalance.ID
                        })
                    }
                }
                //console.log(denom)
                var pReceivingData = {
                    IsNew: true,
                    IsReceiving: 0,
                    IsRefund: data.IsRefund,
                    ReceivingPlace: sessionStorage.FowstID,
                    SourceOfSKU: 'CD71A0F7-08A9-4601-876E-5F13C064F873', // SupplierID = cash payment org id
                    InvoiceID: data.InvoiceID, // put your invoice ID here
                    DateReceived: new Date(),
                    DocumentDate: new Date(),
                    Items: denom
                };

                pReceivingData.data = pReceivingData;
                App.model.Receiving_offline.Save(pReceivingData);
                console.log(denomChange)
                for (var i in denomChange) {
                    VendingMachine.ComputeBinProduct(denomChange[i], false)
                }

                var pIU = new InsertUpdate('TransVDMLogs');

                pIU.Add('InvoiceID', ReadyString(sessionStorage.InvoiceID));
                pIU.Add('DateImported', 'NULL');
                Sql.Execute(pIU.GetUpdate(' LogID = ' + ReadyString(sessionStorage.LogID)))

                data.success(param)                
                NewMessageBoxClose();
            }
        }

        VendingMachine.GetAds = function () {
            pSql = " SELECT TemplateID FROM LibraryVDMAdsAssignedFowst WHERE FowstID = " + ReadyString(sessionStorage.FowstID);
            var templateID = Sql.ExecuteScalar(pSql);

            if (!IsGuid(templateID)) return [];

            pSql = " SELECT * FROM LibraryVDMAds WHERE TemplateID = " + ReadyString(templateID) + " ORDER BY OrderNo ASC"

            return Sql.Execute(pSql).Rows;
        }

        VendingMachine.GetProductSelectionGuides = function () {
            pSql = " SELECT * FROM LibraryVDMProductSelectionGuides "

            return Sql.Execute(pSql).Rows;
        }

        VendingMachine.AddLog = function (page, description, userid) {
            var pIU = new InsertUpdate('TransVDMLogs');

            pIU.Add('ID', ReadyString(NewID()));
            pIU.Add('FOWSTID', ReadyString(sessionStorage.FowstID));
            pIU.Add('DeviceID', ReadyString(Web.DeviceID()));
            pIU.Add('Page', ReadyString(page));
            pIU.Add('Action', ReadyString(description));
            pIU.Add('LastUpdated', proj.SqliteNow);
            pIU.Add('LogID', ReadyString(sessionStorage.LogID));
            pIU.Add('DateImported', 'NULL');

            if (IsGuid(userid)) {
                pIU.Add('UserID', ReadyString(userid));
            }

            Android.AddLog(page + ': ' + description);

            if (!Android.DebugMode()) Sql.Execute(pIU.GetInsert());

            if (IsGuid(sessionStorage.InvoiceID)) {
                var pIU = new InsertUpdate('TransVDMLogs');

                pIU.Add('InvoiceID', ReadyString(sessionStorage.InvoiceID));
                pIU.Add('DateImported', 'NULL');
                Sql.Execute(pIU.GetUpdate(' LogID = ' + ReadyString(sessionStorage.LogID)));
            }

            return true
        }

        VendingMachine.SalesRefund = function (data, callback) {
            var memoid = Business.TransSalesReturn.ReturnInvoice2(data);
            proj.SalesReturnMemoID = memoid;
            UpdateZReadCache(); //Bs_SalesReturns.js
            callback()
        }

        VendingMachine.Settings = [
            {
                Access: 'Lock 1: Merchandise Cabinet Lock',
                Label: 'Open Merchandise Cabinet Lock',
                Callback: function () {
                    UsbSerial_Send({
                        "action": "UNLOCK_LOCK",
                        "lock": 1
                    })
                }
            },
            {
                Access: 'Lock 2: Compute Node Cabinet Lock',
                Label: 'Open Compute Node Cabinet Lock',
                Callback: function () {
                    UsbSerial_Send({
                        "action": "UNLOCK_LOCK",
                        "lock": 2
                    })
                }
            },
            {
                Access: 'Lock 3: Bills and Coins Cabinet Lock',
                Label: 'Open Bills and Coins Cabinet Lock',
                Callback: function () {
                    UsbSerial_Send({
                        "action": "UNLOCK_LOCK",
                        "lock": 3
                    })
                }
            },
            {
                Access: 'Dispense Coins',
                Label: 'Dispense 0.25 cents',
                Callback: function () {
                    coin = 0.25;
                    $('#page_flushedcoins').pageshow();
                    $('#page_flushedcoins .desc').html('<div class="d-flex justify-content-center mb-5"><div class= "spinner-border" style = "width: 8rem; height: 8rem; border-width:10px;" role = "status"></div></div><h3>Please wait...</h3>')
                    UsbSerial_Send({
                        "action": "FLUSH_COINS",
                        "coinHopper": 1
                    })
                }
            },
            {
                Access: 'Dispense Coins',
                Label: 'Dispense P1 coins',
                Callback: function () {
                    coin = 1;
                    $('#page_flushedcoins').pageshow();
                    $('#page_flushedcoins .desc').html('<div class="d-flex justify-content-center mb-5"><div class= "spinner-border" style = "width: 8rem; height: 8rem; border-width:10px;" role = "status"></div></div><h3>Please wait...</h3>')
                    UsbSerial_Send({
                        "action": "FLUSH_COINS",
                        "coinHopper": 2
                    })
                }
            },
            {
                Access: 'Dispense Coins',
                Label: 'Dispense P5 coins',
                Callback: function () {
                    coin = 5;
                    $('#page_flushedcoins').pageshow();
                    $('#page_flushedcoins .desc').html('<div class="d-flex justify-content-center mb-5"><div class= "spinner-border" style = "width: 8rem; height: 8rem; border-width:10px;" role = "status"></div></div><h3>Please wait...</h3>')
                    UsbSerial_Send({
                        "action": "FLUSH_COINS",
                        "coinHopper": 3
                    })
                }
            },            
            {
                Access: 'Pin/Unpin App',
                Label: 'Enable fullscreen',
                Callback: function () {
                    Android.PinApp();
                }
            },
            {
                Access: 'Pin/Unpin App',
                Label: 'Disable fullscreen',
                Callback: function () {
                    Android.UnpinApp();
                }
            },
            {
                Access: 'Open/Close Sliding Door',
                Label: 'Open Sliding Door',
                Callback: function () {
                    UsbSerial_Send({
                        "action": "CONTROL_DOOR",
                        "state": 1
                    })
                }
            },
            {
                Access: 'Open/Close Sliding Door',
                Label: 'Close Sliding Door',
                Callback: function () {
                    UsbSerial_Send({
                        "action": "CONTROL_DOOR",
                        "state": 0
                    })
                }
            },
            {
                Label: 'Inventory Balance Report',
                Callback: function () {
                    VendingMachine.InventoryBalance()
                }
            },
            //{
            //    Label: 'Inventory Balance Report',
            //    Callback: function () {
            //        VendingMachine.InventoryBalance()
            //    }
            //},
            {
                Label: 'Test All Motors',
                Callback: function () {
                    $('#page_testallmotors').pageshow();
                    $('#tbodyTestAllMotors').empty();
                    proj.TestAllMotors();
                }
            },
            {
                Label: 'Open Serial',
                Callback: function () {
                    Js.Execute({ command: 'UsbSerial.Start' })
                }
            },            
            {
                Access: 'Exit to Main Menu',
                Label: 'Exit to Main Menu',
                Callback: function () {
                    Android.UnpinApp();
                    location.href = '/People/PeopleMenu?IsMobile=1';
                }
            },
        ]

        VendingMachine.GetSalesSummary = function (period) {
            var SalesData = {}, curDateFom, curDateTo, prevDateFrom, prevDateTo;

            if (period == 'Today') {
                curDateFom = $.format.date(new Date(), 'yyyy-MM-dd');
                curDateTo = curDateFom;
                prevDateFrom = $.format.date(new Date(new Date().setDate(new Date().getDate() - 1)), 'yyyy-MM-dd');
                prevDateTo = prevDateFrom;
            } else if (period == 'Yesterday') {
                curDateFom = $.format.date(new Date(new Date().setDate(new Date().getDate() - 1)), 'yyyy-MM-dd');
                curDateTo = curDateFom;
                prevDateFrom = $.format.date(new Date(new Date().setDate(new Date(curDateFom).getDate() - 1)), 'yyyy-MM-dd');
                prevDateTo = prevDateFrom;
            } else if (period == 'This Week') {
                var dates_of_week = getWeekRange(new Date(), 0);
                curDateFom = $.format.date(new Date(dates_of_week.start), 'yyyy-MM-dd')
                curDateTo = $.format.date(new Date(dates_of_week.end), 'yyyy-MM-dd');
                dates_of_week = getWeekRange(new Date(), 1);
                prevDateFrom = $.format.date(new Date(dates_of_week.start), 'yyyy-MM-dd')
                prevDateTo = $.format.date(new Date(dates_of_week.end), 'yyyy-MM-dd');
            } else if (period == 'This Month') {
                curDateFom = $.format.date(new Date(), 'yyyy-MM-01');
                curDateTo = $.format.date(new Date(new Date().getFullYear(), new Date().getMonth() + 1, 0), 'yyyy-MM-dd');
                prevDateFrom = $.format.date(new Date(new Date().getFullYear(), new Date().getMonth() - 1, 1), 'yyyy-MM-dd');
                prevDateTo = $.format.date(new Date(new Date().getFullYear(), new Date().getMonth(), 0), 'yyyy-MM-dd');
            } else if (period == 'This Year') {
                curDateFom = $.format.date(new Date(), 'yyyy-01-01');
                curDateTo = $.format.date(new Date(), 'yyyy-12-31')
                prevDateFrom = (new Date().getFullYear() - 1) + '-01-01';
                prevDateTo = (new Date().getFullYear() - 1) + '-12-31';
            }

            var pSql = 'SELECT CAST(SUM(CurGrossSales) as TEXT) as CurGrossSales, \n' +
                ' CAST(SUM(CurSalesReturnRefund) as TEXT) as CurSalesReturnRefund \n' +
                ' FROM cache.CacheZRead \n' +
                ' WHERE FOWSTID = ' + ReadyString(sessionStorage.FowstID) + ' \n' +
                ' AND DeviceID = ' + ReadyString(Web.DeviceID()) + ' \n' +
                ' AND DATE(SalesDate) BETWEEN ' + ReadyString(curDateFom) + ' AND ' + ReadyString(curDateTo);
            var sales = Sql.Execute(pSql).Rows;

            if (sales.length > 0) {
                SalesData.CurNetSales = CDec2(sales[0].CurGrossSales).toFixed(2) - CDec2(sales[0].CurSalesReturnRefund).toFixed(2);
            } else SalesData.CurNetSales = 0;

            pSql = 'SELECT CAST(SUM(CurGrossSales) as TEXT) as CurGrossSales, \n' +
                ' CAST(SUM(CurSalesReturnRefund) as TEXT) as CurSalesReturnRefund \n' +
                ' FROM cache.CacheZRead \n' +
                ' WHERE FOWSTID = ' + ReadyString(sessionStorage.FowstID) + ' \n' +
                ' AND DeviceID = ' + ReadyString(Web.DeviceID()) + ' \n' +
                ' AND DATE(SalesDate) BETWEEN ' + ReadyString(prevDateFrom) + ' AND ' + ReadyString(prevDateTo);

            sales = Sql.Execute(pSql).Rows;

            if (sales.length > 0) {
                SalesData.PrevNetSales = CDec2(sales[0].CurGrossSales).toFixed(2) - CDec2(sales[0].CurSalesReturnRefund).toFixed(2);
            } else SalesData.PrevNetSales = 0;

            SalesData.CurSalesPercentage = CDec2(CDec2(((SalesData.CurNetSales - SalesData.PrevNetSales) / SalesData.PrevNetSales) * 100).toFixed(2));

            pSql = ' SELECT SUM(CAST(OrderedQuantity as Decimal)) as OrderedQuantity FROM cache.CacheSoldItems \n ' +
                ' WHERE FowstID = ' + ReadyString(sessionStorage.FowstID) + ' \n' +
                ' AND DeviceID = ' + ReadyString(Web.DeviceID()) + ' \n' +
                ' AND DATE(SalesDate) BETWEEN ' + ReadyString(curDateFom) + ' AND ' + ReadyString(curDateTo);
            sales = Sql.Execute(pSql).Rows;

            if (sales.length > 0) {
                SalesData.CurSoldItems = CDec2(sales[0].OrderedQuantity);
            } else SalesData.CurSoldItems = 0;

            pSql = ' SELECT SUM(CAST(OrderedQuantity as Decimal)) as OrderedQuantity FROM cache.CacheSoldItems \n ' +
                ' WHERE FowstID = ' + ReadyString(sessionStorage.FowstID) + ' \n' +
                ' AND DeviceID = ' + ReadyString(Web.DeviceID()) + ' \n' +
                ' AND DATE(SalesDate) BETWEEN ' + ReadyString(prevDateFrom) + ' AND ' + ReadyString(prevDateTo);
            sales = Sql.Execute(pSql).Rows;

            if (sales.length > 0) {
                SalesData.PrevSoldItems = CDec2(sales[0].OrderedQuantity);
            } else SalesData.PrevSoldItems = 0;

            SalesData.CurSoldItemsPercentage = CDec2(CDec2(((SalesData.CurSoldItems - SalesData.PrevSoldItems) / SalesData.PrevSoldItems) * 100).toFixed(2));

            pSql = ' SELECT ProductCategory, ProductName, SUM(OrderedQuantity) as NoOfSoldItems FROM cache.CacheSoldItems \n ' +
                ' WHERE FowstID = ' + ReadyString(sessionStorage.FowstID) + ' \n' +
                ' AND DeviceID = ' + ReadyString(Web.DeviceID()) + ' \n' +
                ' AND DATE(SalesDate) BETWEEN ' + ReadyString(curDateFom) + ' AND ' + ReadyString(curDateTo) + ' \n' +
                ' GROUP BY ProductID ORDER BY NoOfSoldItems DESC LIMIT 5';
            sales = Sql.Execute(pSql).Rows;

            if (sales.length > 0) {
                SalesData.TopItemSoldName = sales[0].ProductName;
                SalesData.TopItemSoldCount = CDec2(sales[0].NoOfSoldItems);
                SalesData.MostItemsSold = sales
            } else {
                SalesData.TopItemSoldName = '-';
                SalesData.TopItemSoldCount = '-';
                SalesData.MostItemsSold = [];
            }

            SalesData.SalesOvertime = { Label: [], DataSets: [] };

            if (period == 'Today' || period == 'Yesterday') {
                pSql = ' SELECT strftime("%H", SalesDate) as Label, SUM(OrderedQuantity * UnitPrice) as Gross FROM cache.CacheSoldItems \n ' +
                    ' WHERE FowstID = ' + ReadyString(sessionStorage.FowstID) + ' \n' +
                    ' AND DeviceID = ' + ReadyString(Web.DeviceID()) + ' \n' +
                    ' AND DATE(SalesDate) BETWEEN ' + ReadyString(curDateFom) + ' AND ' + ReadyString(curDateTo) + ' \n' +
                    ' GROUP BY Label ORDER BY Label';
            } else if (period == 'This Week') {
                pSql = ' SELECT strftime("%w", SalesDate) as day_of_week, \n' +
                    ' CASE strftime("%w", SalesDate) \n ' +
                    '   WHEN "0" THEN "Sun" \n ' +
                    '   WHEN "1" THEN "Mon" \n ' +
                    '   WHEN "2" THEN "Tue" \n ' +
                    '   WHEN "3" THEN "Wed" \n ' +
                    '   WHEN "4" THEN "Thu" \n ' +
                    '   WHEN "5" THEN "Fri" \n ' +
                    '   WHEN "6" THEN "Sat" \n ' +
                    ' END as Label, \n ' +
                    ' SUM(OrderedQuantity * UnitPrice) as Gross FROM cache.CacheSoldItems \n ' +
                    ' WHERE FowstID = ' + ReadyString(sessionStorage.FowstID) + ' \n' +
                    ' AND DeviceID = ' + ReadyString(Web.DeviceID()) + ' \n' +
                    ' AND DATE(SalesDate) BETWEEN ' + ReadyString(curDateFom) + ' AND ' + ReadyString(curDateTo) + ' \n' +
                    ' GROUP BY day_of_week ORDER BY day_of_week';
            } else if (period == 'This Month') {
                pSql = ' WITH RECURSIVE week_series(week_start) AS (\n' +
                    '   SELECT date(' + ReadyString(curDateFom) + ', "-" || strftime("%w", ' + ReadyString(curDateFom) + ') || " days", \n' +
                    '       CASE WHEN strftime("%w", ' + ReadyString(curDateFom) + ') = "0" THEN "+0 days" ELSE "+7 days" END) \n' +
                    '   UNION ALL \n' +
                    '   SELECT date(week_start, "+7 days") \n' +
                    '       FROM week_series \n' +
                    '       WHERE week_start <= ' + ReadyString(curDateTo) + ' \n' +
                    '), \n' +
                    ' weeks AS ( \n' +
                    ' SELECT \n' +
                    '   "Week " || ROW_NUMBER() OVER (ORDER BY week_start) AS week_label, \n' +
                    '   week_start, \n' +
                    '   date(week_start, "+6 days") AS week_end \n' +
                    '   FROM week_series \n' +
                    '   WHERE week_start <= ' + ReadyString(curDateTo) + ' \n' +
                    '), \n' +
                    ' sales_by_week AS ( \n' +
                    ' SELECT \n' +
                    '   date(SalesDate, "-" || strftime("%w", SalesDate) || " days") AS week_start, \n' +
                    '   SUM(OrderedQuantity * UnitPrice) AS total_gross_sales \n' +
                    '   FROM cache.CacheSoldItems \n' +
                    '   WHERE FowstID = ' + ReadyString(sessionStorage.FowstID) + ' \n' +
                    '   AND DeviceID = ' + ReadyString(Web.DeviceID()) + ' \n' +
                    '   AND DATE(SalesDate) BETWEEN ' + ReadyString(curDateFom) + ' AND ' + ReadyString(curDateTo) + ' \n' +
                    '   GROUP BY week_start \n' +
                    ' ) \n' +
                    ' SELECT \n' +
                    '   w.week_label as Label, \n' +
                    '   w.week_start, \n' +
                    '   w.week_end, \n' +
                    '   COALESCE(s.total_gross_sales, 0) as [Gross] \n' +
                    ' FROM weeks w \n' +
                    ' LEFT JOIN sales_by_week s ON s.week_start = w.week_start \n' +
                    ' ORDER BY w.week_start; \n'
            } else if (period == 'This Year') {
                pSql = ' SELECT \n' +
                    ' CASE strftime("%m", SalesDate) \n ' +
                    '   WHEN "01" THEN "Jan" \n ' +
                    '   WHEN "02" THEN "Feb" \n ' +
                    '   WHEN "03" THEN "Mar" \n ' +
                    '   WHEN "04" THEN "Apr" \n ' +
                    '   WHEN "05" THEN "May" \n ' +
                    '   WHEN "06" THEN "Jun" \n ' +
                    '   WHEN "07" THEN "Jul" \n ' +
                    '   WHEN "08" THEN "Aug" \n ' +
                    '   WHEN "09" THEN "Sep" \n ' +
                    '   WHEN "10" THEN "Oct" \n ' +
                    '   WHEN "11" THEN "Nov" \n ' +
                    '   WHEN "12" THEN "Dec" \n ' +
                    ' END as Label \n ' +
                    ', SUM(OrderedQuantity * UnitPrice) as Gross FROM cache.CacheSoldItems \n ' +
                    ' WHERE FowstID = ' + ReadyString(sessionStorage.FowstID) + ' \n' +
                    ' AND DeviceID = ' + ReadyString(Web.DeviceID()) + ' \n' +
                    ' AND DATE(SalesDate) BETWEEN ' + ReadyString(curDateFom) + ' AND ' + ReadyString(curDateTo) + ' \n' +
                    ' GROUP BY Label ORDER BY Label';
            }

            sales = Sql.Execute(pSql).Rows;

            if (sales.length > 0) {
                for (var i in sales) {
                    SalesData.SalesOvertime.Label.push(sales[i].Label);
                    SalesData.SalesOvertime.DataSets.push(CDec2(CDec2(sales[i].Gross).toFixed(2)));
                }
            }

            pSql = 'SELECT ProductCategory, SUM(OrderedQuantity * UnitPrice) as Gross \n' +
                ' FROM cache.CacheSoldItems \n ' +
                ' WHERE FowstID = ' + ReadyString(sessionStorage.FowstID) + ' \n' +
                ' AND DeviceID = ' + ReadyString(Web.DeviceID()) + ' \n' +
                ' AND DATE(SalesDate) BETWEEN ' + ReadyString(curDateFom) + ' AND ' + ReadyString(curDateTo) + ' \n' +
                ' GROUP BY ProductCategory ORDER BY ProductCategory';

            SalesData.SalesByCategory = Sql.Execute(pSql).Rows;

            console.log(SalesData);
            return SalesData
        }

        function getWeekStart(date = new Date(), weeksAgo = 0) {
            var d = new Date(date);
            d.setHours(0, 0, 0, 0);

            // Move back to the most recent Sunday
            var day = d.getDay(); // 0 = Sunday, 6 = Saturday
            d.setDate(d.getDate() - day);

            // Go back additional weeks if needed
            d.setDate(d.getDate() - (weeksAgo * 7));

            return d;
        }

        function getWeekRange(date = new Date(), weeksAgo = 0) {
            var start = getWeekStart(date, weeksAgo);
            var end = new Date(start);
            end.setDate(start.getDate() + 6);
            return { start, end };
        }

        function getWeeksInMonth(year, month, includeOverflow = true) {
            // month is 1-indexed (8 = August)
            var weeks = [];

            var firstOfMonth = new Date(year, month - 1, 1);
            var lastOfMonth = new Date(year, month, 0); // last day of the month

            // Find the first Sunday on or after the 1st
            var cursor = new Date(firstOfMonth);
            var offsetToSunday = (7 - cursor.getDay()) % 7;
            cursor.setDate(cursor.getDate() + offsetToSunday);

            // Walk forward in 7-day blocks, as long as the week START falls within the month
            while (cursor <= lastOfMonth) {
                var weekEnd = new Date(cursor);
                weekEnd.setDate(cursor.getDate() + 6);

                weeks.push({ start: new Date(cursor), end: weekEnd });
                cursor.setDate(cursor.getDate() + 7);
            }

            return weeks;
        }
    })