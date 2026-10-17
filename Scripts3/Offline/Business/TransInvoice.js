define([], function () {
    var mCounterFilename = 'Counters2.txt';
    var mDatedCounterFilename = 'DatedCounters.txt';

    window.Business.TransInvoice = new function (vParent) {
        this.IsValidCart = function (vID) {

            var pDT;
            var pSql;

            console.time('Business.TransInvoice.IsValidCart jsSPJWBDPLKG!');
            pSql = 'SELECT TransInvoice.ID' +
                ' FROM TransInvoice' +
                ' WHERE IFNULL(TransInvoice.IsCancelled, 0) = 0' +
                ' AND IFNULL(TransInvoice.InvoiceNumber, 0) = 0' +
                ' AND TransInvoice.ID = ' + ReadyString(vID);
            pDT = Sql.Execute(pSql);
            console.timeEnd('Business.TransInvoice.IsValidCart jsSPJWBDPLKG!');

            return pDT.Rows.length > 0;

        }; // IsValidCart
         
        this.GetCartID = function (vUserID, vFowstID) {

            var pDR;
            var pSql;

            pSql = 'SELECT TransInvoice.ID\n' +
                ' FROM TransInvoice\n' +
                ' WHERE TransInvoice.IsCancelled = 0\n' +
                ' AND TransInvoice.InvoiceNumber = 0\n' +
                ' AND EXISTS(SELECT TransInvoiceItems.ID\n' +
                ' 	FROM TransInvoiceItems\n' +
                '   WHERE TransInvoiceItems.InvoiceID = TransInvoice.ID)\n' +
                ' AND TransInvoice.UserID = ' + ReadyString(vUserID) + '\n' +
                ' AND TransInvoice.FowstID = ' + (CStr2(vFowstID).trim().length > 0 ? ReadyString(vFowstID) : 'NULL') + '\n' +
                ' ORDER BY TransInvoice.LastUpdated DESC\n' +
                ' LIMIT 1';
            pDR = Sql.ExecuteRow(pSql);
            if (pDR != undefined)
                return pDR.ID;

        }; // this.GetCartID = function(vUserID, vFowstID) {

        //edited by janine sept 9 2014 11am . added unitmeasure. Reason: i need to get the ordered qty before the item gets deleted. see function GetOrderedQty
        this.DeleteOrderStock = function (vInvoiceID, vProductID, vUnitMeasure, vModifierGroupID, vIsOrderSlipPrinted, vOptions) {

            var pSql;
            var pWhere;
            var pDT;

            vOptions = $.extend({
                createVoidEntries: false
            }, vOptions);

            pWhere = ' (SELECT TransInvoiceItems.ID' + '\n' +
                ' FROM TransInvoiceItems' + '\n' +
                ' WHERE TransInvoiceItems.InvoiceID = ' + ReadyString(vInvoiceID) + '\n' +
                ' AND TransInvoiceItems.ProductID = ' + ReadyString(vProductID) + '\n';
            if (CStr2(vUnitMeasure) != '')
                pWhere += ' AND TransInvoiceItems.UnitMeasure = ' + ReadyString(vUnitMeasure);
            if (CStr2(vModifierGroupID) == '')
                pWhere += ' AND IFNULL(TransInvoiceItems.ModifierGroupID, \'\') = \'\'\n';
            else
                pWhere += ' AND TransInvoiceItems.ModifierGroupID = ' + ReadyString(vModifierGroupID) + '\n';

            // 2016_08_25__09_09_15_5302 changes: delete invoice items regardless of vIsOrderSlipPrinted
            // pWhere += ' AND IFNULL(TransInvoiceItems.IsOrderSlipPrinted, 0) = ' + (CBool(vIsOrderSlipPrinted) ? 1 : 0) + '\n';
            pWhere += ' )';

            // get items to be deleted to save in TransInvoiceItemsVoided
            if (vOptions.createVoidEntries) {
                pSql = 'SELECT Quantity\n' +
                    ',ActualQuantity\n' +
                    ',UnitMeasure\n' +
                    ',UnitPrice\n' +
                    ' FROM TransInvoiceItems\n' +
                    ' WHERE TransInvoiceItems.ID IN ' + '\n' +
                    pWhere;
                pDT = Sql.Execute(pSql);
                for (var p = 0; p <= pDT.Rows.length - 1; p++) {
                    var pDR = pDT.Rows[p];
                    Business.TransInvoice.AddVoidItem({
                        InvoiceID: vInvoiceID,
                        ProductID: vProductID,
                        Quantity: pDR.Quantity,
                        ActualQuantity: pDR.ActualQuantity,
                        UnitMeasure: pDR.UnitMeasure,
                        UnitPrice: pDR.UnitPrice
                    });
                }
            } // END if (vOptions.createVoidEntries) {

            pSql = 'DELETE FROM TransInvoiceItemSalesReps' + '\n' +
                ' WHERE TransInvoiceItemSalesReps.InvoiceItemID IN ' + '\n' +
                pWhere;
            pSql = Sql.ExecuteSql(pSql);

            pSql = "DELETE FROM TransInvoiceItemsModifier WHERE InvoiceItemID = " + pWhere;
            pSql = Sql.ExecuteSql(pSql);

            pSql = 'DELETE FROM TransInvoiceItems\n' +
                ' WHERE TransInvoiceItems.ID IN ' + '\n' +
                pWhere;
            //additional by janine. if blank, ok lang delete all unit measure of that product, if not then delete only specific unit measure
            pSql = Sql.ExecuteSql(pSql);

            pSql = 'DELETE FROM TransInvoiceItemsRebates\n' +
                ' WHERE IFNULL(TransInvoiceItemsRebates.ModifierGroupID, \'\') = ' + ReadyString(vModifierGroupID) + '\n';
            pSql = Sql.ExecuteSql(pSql);

            // just a "clean up" code. not particular to this transaction.
            // delete all orphan TransInvoiceItemsRebates
            pSql = 'DELETE FROM TransInvoiceItemsRebates' + '\n' +
                ' WHERE IFNULL(TransInvoiceItemsRebates.ModifierGroupID, \'\') NOT IN ' + '\n' +
                '(SELECT IFNULL(ModifierGroupID, \'\') \n' +
                ' FROM TransInvoiceItems)';
            pSql = Sql.ExecuteSql(pSql);
            
            return true;

        }; // DeleteOrderStock

        this.GetByProductIDUnitMeasureModifierGroupID = function(vInvoiceID, vProductID, vUnitMeasure, vModifierGroupID) {

            var pSql;
            var pDT;

            pSql = 'SELECT TransInvoiceItems.ID' + '\n' +
                ',QuantityPrinted\n' +
                ' FROM TransInvoiceItems' + '\n' +
                ' WHERE TransInvoiceItems.InvoiceID = ' + ReadyString(vInvoiceID) + '\n' +
                ' AND TransInvoiceItems.ProductID = ' + ReadyString(vProductID) + '\n';
            if (CStr2(vUnitMeasure) != '')
                pSql += ' AND TransInvoiceItems.UnitMeasure = ' + ReadyString(vUnitMeasure);
            if (CStr2(vModifierGroupID) == '')
                pSql += ' AND IFNULL(TransInvoiceItems.ModifierGroupID, \'\') = \'\'\n';
            else
                pSql += ' AND TransInvoiceItems.ModifierGroupID = ' + ReadyString(vModifierGroupID) + '\n';
            pDT = Sql.Execute(pSql);

            return pDT;
        };

        this.GetQuantityPrintedByProductIDUnitMeasureModifierGroupID = function(vInvoiceID, vProductID, vUnitMeasure, vModifierGroupID) {
            
            var pDT = this.GetByProductIDUnitMeasureModifierGroupID(vInvoiceID, vProductID, vUnitMeasure, vModifierGroupID);
            var pOut = 0;

            for (var p = 0; p <= pDT.Rows.length - 1; p++)
                pOut += CDec2(pDT.Rows[p].QuantityPrinted);

            return pOut;
        };

        //additional by janine sept 8 2014 for sales order taker
        this.GetOrderedQty = function (vInvoiceID, vProductID, vUnitMeasure) {

            var pSql;
            var pDT;

            pSql = 'SELECT OrderedQuantity FROM TransInvoiceItems' +
            ' WHERE InvoiceID = ' + ReadyString(vInvoiceID) +
            ' AND ProductID = ' + ReadyString(vProductID) + ' AND UnitMeasure =' + ReadyString(vUnitMeasure);
            pDT = Sql.Execute(pSql);

            return pDT;


        }; // GetOrderedQty
        ///////////

        this.GetCartStock = function (vInvoiceID, vProductID, vPosMode, vModifierGroupID) {

            var pDT;
            var pSql;

            //additional by janine sept 8 2014 for sales order taker
            if (vPosMode == 'pos')
                pSql = 'SELECT SUM(TransInvoiceItems.Quantity) AS Quantity';
            else
                pSql = 'SELECT SUM(TransInvoiceItems.OrderedQuantity) AS Quantity';

            pSql += ',TransInvoiceItems.ActualQuantity' +
                ',TransInvoiceItems.QuantityPrinted' +
                ',TransInvoiceItems.PriceLevelID' +
                ',TransInvoiceItems.RequestedUnitPrice' +
                ',TransInvoiceItems.UnitPrice\n';

            pSql += ' FROM private.TransInvoiceItems\n';
            pSql += ' WHERE TransInvoiceItems.InvoiceID = ' + ReadyString(vInvoiceID) +
                ' AND TransInvoiceItems.ProductID = ' + ReadyString(vProductID) + '\n';
            if (IsGuid(vModifierGroupID))
                pSql += ' AND TransInvoiceItems.ModifierGroupID = ' + ReadyString(vModifierGroupID) + '\n';
            else
                pSql += ' AND IFNULL(TransInvoiceItems.ModifierGroupID, \'\') = \'\'\n';
            pSql += ' GROUP BY TransInvoiceItems.ActualQuantity' +
                ',TransInvoiceItems.PriceLevelID' +
                ',TransInvoiceItems.RequestedUnitPrice';
            pDT = Sql.Execute(pSql);

            return pDT;

        }; // GetCartStock

        this.GetCart = function (vID) {

            var pSql;
            var pInvoiceID;
            var pDS;
            var pUserID = Web.PeopleUserID();
            var pFowstID = sessionStorage.FowstID;

            console.time('Business.TransInvoice.GetCart jsAHFAH9LKDU!');
            pInvoiceID = vID;
            if (CStr2(pInvoiceID) == '') {
                pInvoiceID = this.GetCartID(pUserID, pFowstID);
                if (CStr2(pInvoiceID) == '') return;
            };

            pSql = ' WHERE TransInvoice.ID = ' + ReadyString(pInvoiceID);
            pDS = this.GetReportData(pSql);
            // no need to rename "InvoiceID" to "ID"

            this.GetItemModifier(pDS.Stocks.Rows);

            console.log('GetCart jsY5JPFYHLCH!');
            console.timeEnd('Business.TransInvoice.GetCart jsAHFAH9LKDU!');
            return pDS;

        }; // GetCart

        this.GetReportData = function (vWhere,
            vAddUnitExplaination) {

            var pDS;
            var pDT;
            var pDtInvoice;
            var pSql;
            var pItemFromSql;
            var pItemWhereSql;

            pDS = {};

            // invoice ==============================================================================================================================================
            pSql = 'SELECT TransInvoice.ID' + // in the vb code, this has "AS InvoiceID"
                ' ,LibraryOrganization.CompanyName AS CustomerName' +
                ' ,LibraryOrganization.TinNumber' +
                ' ,LibraryOrganizationRepresentatives.LastName + \', \' + LibraryOrganizationRepresentatives.FirstName + \' \' + LibraryOrganizationRepresentatives.MiddleName AS Representative' +
                ' ,IFNULL(LibraryOrganizationFOWST.StreetNumber, \'\') + \', \' + IFNULL(LibraryOrganizationFOWST.Street, \'\') AS DeliveryAddress' +
                ' ,TransInvoice.InvoiceNumber' +
                ' ,TransInvoice.InvoiceDate' +
                ' ,TransInvoice.CustomerTableNo' +
                ' ,TransInvoice.DeliveryDate' +
                ' ,TransInvoice.Remarks' +
                ' ,TransInvoice.CustomerID' +
                ' ,TransInvoice.LastUpdated' +
                ' ,TransInvoice.UserID' +
                ' ,TransInvoice.FowstID' +
                ' ,TransInvoice.DeviceID' +
                ' ,TransInvoice.RefNum' +
                ' ,TransInvoice.ReasonForReconstructingData' +
                ' ,TransInvoice.TotalAmount' +
                ' ,TransInvoice.BillingAddressID' +
                ' ,TransInvoice.DeliveryAddressID' + 
                ' ,TransInvoice.SalesOrderNo' +
                ' ,TransInvoice.PurchaseOrderNo' +
                ' ,TransInvoice.SalesOrderDate' + 
                ' ,TransInvoice.SupplierID' +
                ' ,TransInvoice.SupplierFowstID' +
                ' ,TransInvoice.SupplierBinID' +
                ' ,TransInvoice.CustomerID' +
                ' ,TransInvoice.CustomerFowstID' +
                ' ,TransInvoice.CustomerBinID';
            pSql += ' ,tblOrg.CompanyName AS OrgName' + '\n' +
                ' ,IFNULL(tblOrgFowst.StreetNumber, \'\') ' + '\n' +
                '     || \',\' || IFNULL(tblOrgFowst.Street, \'\') ' + '\n' +
                '     || \',\' || IFNULL(tblOrgCity.Name, \'\') AS OrgFowst\n';
            pSql += ',IFNULL(tblDelivery.StreetNumber, \'\') ' + '\n' +
                '     || \',\' || IFNULL(tblDelivery.Street, \'\') ' + '\n' +
                '     || \',\' || IFNULL(tblDeliveryCity.Name, \'\')' + '\n' +
                '     || \',\' || IFNULL(tblDeliveryCountry.Name, \'\') AS DeliveryAddress\n';

            pSql += ' ,TransInvoice.SupplierID' + '\n' + 
                ' ,TransInvoice.SupplierFowstID' + '\n' + 
                ' ,TransInvoice.SupplierBinID' + '\n' + 
                ' ,tblSupplierBin.Name AS SupplierBinName' + '\n' + 
                ' ,TransInvoice.CustomerFowstID' + '\n' + 
                ' ,TransInvoice.CustomerBinID' + '\n' + 
                ' ,tblCustomerBin .Name AS CustomerBinName\n';
            pSql += ' ,tblSupplierOrg.CompanyName AS SupplierOrgName' + '\n' + 
                ' ,tblSupplierFowst.Name AS SupplierFowstName\n' +
                ',tblCustomerFowst.Name AS CustomerFowstName\n';
            pSql += '  FROM TransInvoice \n' +
                '  JOIN LibraryOrganization ON TransInvoice.CustomerID = LibraryOrganization.ID \n' +
                '  LEFT JOIN LibraryOrganizationRepresentatives ON TransInvoice.CusRepresentativeID = LibraryOrganizationRepresentatives.ID \n' +
                '  LEFT JOIN LibraryOrganizationFOWST ON LibraryOrganizationFOWST.ID = TransInvoice.BillingAddressID\n' +
                '  LEFT JOIN LibraryCities ON LibraryCities.ID = LibraryOrganizationFOWST.FowstCityID\n' +
                '  LEFT JOIN LibraryCountries ON LibraryCountries.ID = LibraryOrganizationFOWST.FowstCountryID\n';
            pSql += ' LEFT JOIN LibraryOrganizationFOWST tblOrgFowst ON tblOrgFowst.ID = TransInvoice.FowstID' + '\n' +
                ' LEFT JOIN LibraryCities tblOrgCity ON tblOrgCity.ID = tblOrgFowst.FowstCityID' + '\n' +
                ' ,LibraryOrganization tblOrg\n';
            pSql += '  LEFT JOIN LibraryOrganizationFOWST tblDelivery ON tblDelivery.ID = TransInvoice.DeliveryAddressID\n' +
                '  LEFT JOIN LibraryCities tblDeliveryCity ON tblDeliveryCity.ID = tblDelivery.FowstCityID\n' +
                '  LEFT JOIN LibraryCountries tblDeliveryCountry ON tblDeliveryCountry.ID = tblDelivery.FowstCountryID\n';
            pSql += ' LEFT JOIN private.LibraryFowstBin tblSupplierBin ON tblSupplierBin.ID = TransInvoice.SupplierBinID' + '\n' + 
                ' LEFT JOIN private.LibraryFowstBin tblCustomerBin ON tblCustomerBin.ID = TransInvoice.CustomerBinID\n';
            pSql += ' LEFT JOIN LibraryOrganization tblSupplierOrg ON tblSupplierOrg.ID = TransInvoice.SupplierID' + '\n' + 
                ' LEFT JOIN LibraryOrganizationFOWST tblSupplierFowst ON tblSupplierFowst.ID = TransInvoice.SupplierFowstID\n' +
                ' LEFT JOIN LibraryOrganizationFOWST tblCustomerFowst ON tblCustomerFowst.ID = TransInvoice.CustomerFowstID\n';
            pSql += vWhere +
                ' AND tblOrg.ID = ' + ReadyString(sessionStorage.OrgID) +
                '  ORDER BY TransInvoice.InvoiceNumber';
            pDtInvoice = Sql.Execute(pSql);
            pDS.Invoices = pDtInvoice;

            // stocks ==============================================================================================================================================
            pSql = 'SELECT LibraryPIMProduct.ID\n' +
            ',LibraryPIMProduct.ProductNo' + '\n';
            pSql += ' ,CASE WHEN tblPimRebate.ProductName IS NULL THEN LibraryPIMProduct.ProductName' + '\n' +
                ' ELSE LibraryPIMProduct.ProductName || \' (Rebate of PHP\' || (TransInvoiceItemsRebates.Amount * TransInvoiceItems.Quantity) || \' in exchange for \' || (TransInvoiceItemsRebates.Quantity * TransInvoiceItems.Quantity) || \' \' || TransInvoiceItemsRebates.UnitMeasure || \' \' || tblPimRebate.ProductName || \')\'' + '\n' +
                ' END AS ProductName\n';
            pSql += ' ,TransInvoiceItems.OrderedQuantity' + '\n' +
            ' ,TransInvoiceItems.Quantity' + '\n' +
            ' ,TransInvoiceItems.UnitMeasure' + '\n' +
            ' ,CAST(TransInvoiceItems.TermDiscount AS STRING) + \'% \' + CAST(TransInvoiceItems.DiscountIfPaidWithIn AS STRING) + \' NET \' + CAST(TransInvoiceItems.NetDue AS STRING) AS Terms' + '\n' +
            ' ,TransInvoiceItems.UnitPrice' + '\n' +
            ' ,TransInvoiceItems.SubTotal' + '\n' +
            ' ,TransInvoiceItems.DiscountIfPaidWithIn' + '\n' +
            ' ,TransInvoiceItems.NetDue' + '\n' +
            ' ,TransInvoiceItems.TermNumericValue' + '\n' +
            ' ,TransInvoiceItems.TermDiscount AS TermDiscountNumeric' + '\n' +
            ' ,TransInvoiceItems.ActualQuantity' + '\n' +
            ' ,TransInvoiceItems.ProductID' + '\n' +
            ' ,TransInvoiceItems.InvoiceID' + '\n' +
            ' ,TransInvoiceItems.LineNumber' + '\n' +
            ' ,TransInvoiceItems.LineCost ' + '\n' +
            ' ,TransInvoiceItems.ModifierGroupID ' + '\n' +
            ' ,TransInvoiceItems.ModifierGroupID as GroupIdOfItemModifiers ' + '\n' +
            ' ,TransInvoiceItems.PriceLevelID ' + '\n' +
            ' ,TransInvoiceItems.ID AS ItemID' + '\n' +
            ' ,TransInvoiceItems.IsOrderSlipPrinted AS IsOrderSlipPrinted' + '\n' +
            ' ,TransInvoiceItems.TaxAmount' + '\n' +
            ' ,LibraryPIMProductVariableFeatures001.C0020 AS CategoryName ';

            pItemFromSql = ' FROM private.TransInvoiceItems ' + '\n' +
            ' JOIN private.TransInvoice ON TransInvoice.ID = TransInvoiceItems.InvoiceID ' + '\n' +
            ' LEFT JOIN LibraryPIMProduct ON TransInvoiceItems.ProductID = LibraryPIMProduct.ID ' + '\n';
            pItemWhereSql = vWhere + ' AND (TransInvoiceItems.Quantity > 0 OR TransInvoiceItems.OrderedQuantity > 0)' + '\n';
            pSql += pItemFromSql;
            pSql += ' LEFT JOIN private.TransInvoiceItemsRebates ON TransInvoiceItemsRebates.ModifierGroupID = TransInvoiceItems.ModifierGroupID' + '\n' +
                ' LEFT JOIN LibraryPIMProduct tblPimRebate ON tblPimRebate.ID = TransInvoiceItemsRebates.ProductID' + '\n' +
                ' LEFT JOIN private.LibraryItemModifierRebates ON LibraryItemModifierRebates.ID = TransInvoiceItemsRebates.RebateID' + '\n' +
                ' LEFT JOIN private.LibraryItemModifiers ON LibraryItemModifiers.ID = LibraryItemModifierRebates.ItemModifierID\n' +
                ' LEFT JOIN LibraryPIMProductVariableFeatures001 ON LibraryPIMProductVariableFeatures001.ProductID = TransInvoiceItems.ProductID \n';
            pSql += pItemWhereSql;
            pSql += ' ORDER BY TransInvoiceItems.LineNumber DESC';
            //edited by janine sept 5 for sales order taker OrderedQuantity
            pDS.Stocks = Sql.Execute(pSql);

            // Modifiers
            pSql = 'SELECT DISTINCT TransInvoiceItemsRebates.ID AS RebateRowID' + '\n' +
                ' ,TransInvoiceItemsRebates.ModifierGroupID' + '\n' +
                ' ,TransInvoiceItemsRebates.RebateID' + '\n' +
                ',LibraryItemModifiers.Name AS ModifierName\n';
            pSql += pItemFromSql;
            pSql += ' JOIN private.TransInvoiceItemsRebates ON TransInvoiceItemsRebates.ModifierGroupID = TransInvoiceItems.ModifierGroupID\n' +
                ' JOIN private.LibraryItemModifierRebates ON LibraryItemModifierRebates.ID = TransInvoiceItemsRebates.RebateID\n' +
                ' JOIN private.LibraryItemModifiers ON LibraryItemModifiers.ID = LibraryItemModifierRebates.ItemModifierID\n';
            pSql += pItemWhereSql;
            pDS.Modifiers = Sql.Execute(pSql);

            return pDS;

        }; // GetReportData

        this.GetTotalAmount = function (vID) {

            var pDR;
            var pSql;

            pSql = ' SELECT SUM(SubTotal) AS Total' + '\n' +
                ' FROM TransInvoiceItems' + '\n' +
                ' WHERE InvoiceID = ' + ReadyString(vID);
            pDR = Sql.ExecuteRow(pSql);

            return CDec2(pDR.Total); ;

        };

        this.UpdateTotalAmount = function (vID) {

            var pSql;
            //additional by janine sept 5 for sales order taker
            if (proj.PosMode) {
                pSql = ' UPDATE TransInvoice' + '\n' +
                ' SET TotalAmount = ' + '\n' +
                ' (SELECT SUM(TransInvoiceItems.Quantity * TransInvoiceItems.UnitPrice) ' + '\n' +
                '  + IFNULL((SELECT SUM(SubTotal) FROM TransInvoiceItemsModifier as mod WHERE mod.InvoiceItemID = TransInvoiceItems.ID), 0)\n ' +
                ' FROM TransInvoice ' + '\n' +
                ' JOIN TransInvoiceItems ON TransInvoiceItems.InvoiceID = TransInvoice.ID' + '\n' +
                ' WHERE TransInvoice .ID = ' + ReadyString(vID) + '\n' +
                ' AND TransInvoiceItems.Quantity > 0\n' +
                ' ) ' + '\n' +
                ' WHERE ID = ' + ReadyString(vID);
            } else {
                pSql = ' UPDATE TransInvoice' + '\n' +
                ' SET TotalAmount = ' + '\n' +
                ' (SELECT SUM(TransInvoiceItems.OrderedQuantity * TransInvoiceItems.OriginalUnitPrice) ' + '\n' +
                ' FROM TransInvoice ' + '\n' +
                ' JOIN TransInvoiceItems ON TransInvoiceItems.InvoiceID = TransInvoice.ID' + '\n' +
                ' WHERE TransInvoice .ID = ' + ReadyString(vID) + '\n' +
                ' ) ' + '\n' +
                ' WHERE ID = ' + ReadyString(vID);
            }

            pSql = CDec2(Sql.ExecuteSql(pSql));

            return pSql;

        };

        this.UpdateItemsModifier = function (itemID, itemQty, vData) {
            //SUBTOTAL FORMULA (Item Qty * Modifier Qty * Modifier Price)
            //if vData is undefined page nag.update using stock info
            if(vData != undefined){
                if (vData.length > 0) {
                    for (var i = 0; i < vData.length; i++) {
                        if (vData[i].SelectedModifier == undefined) {
                            continue
                        }
                        var idExclude = [];
                        for (var m = 0; m < vData[i].SelectedModifier.length; m++) {
                            var item = vData[i].SelectedModifier[m];
                            idExclude.push(item.ID) // get the available modifiers
                        }

                        if (idExclude.length > 0) { //delete all modifier except the available modifier
                            Sql.Execute("DELETE FROM TransInvoiceItemsModifier WHERE InvoiceItemID = " + ReadyString(itemID) + '\n' +
                                        " AND ItemModifierID = " + ReadyString(vData[i].ID) + '\n' +
                                        " AND ModifierID NOT IN (" + "'" + idExclude.join("','") + "'" + ")")
                        }

                        for (var m = 0; m < vData[i].SelectedModifier.length; m++) {
                            var item = vData[i].SelectedModifier[m];
                            var pIU = new InsertUpdate("TransInvoiceItemsModifier");
                            var pSql, pDT;

                            pIU.Add("InvoiceItemID", ReadyString(itemID));
                            pIU.Add("ItemModifierID", ReadyString(vData[i].ID));
                            pIU.Add("ModifierID", ReadyString(item.ID));
                            pIU.Add("OrderedQty", ReadyString(item.OrderQty));
                            pIU.Add("InputQty", ReadyString(item.InputQty));
                            pIU.Add("ModifierPrice", ReadyString(item.UnitPrice));
                            if (CBool(item.PremiumPriceApplied)) {
                                pIU.Add("PremiumPrice", ReadyString(item.PremiumPrice));
                            }
                            pIU.Add("SubTotal", ReadyString((CDec2(item.OrderQty) * CDec2(item.UnitPrice)) + (CBool(item.PremiumPriceApplied) ? CDec2(item.PremiumPrice) : 0))); //itemQty *
                            pIU.Add("OtherDescription", ReadyString(CStr2(item.Others)));

                            if (Sql.Execute("SELECT ID FROM TransInvoiceItemsModifier WHERE InvoiceItemID = " + ReadyString(itemID) + "\n" +
                                            " AND ModifierID = " + ReadyString(item.ID)).Rows.length > 0) { //check if theres existing record
                                pSql = pIU.GetUpdate(" InvoiceItemID = " + ReadyString(itemID) + " AND ModifierID = " + ReadyString(item.ID))
                            } else {
                                pIU.Add("ID", ReadyString(NewID()));
                                pSql = pIU.GetInsert();
                            }

                            Sql.Execute(pSql);
                        }
                    }
                }
            }            
        }

        this.GetItemModifier = function (vData) {
            console.time('Business.TransInvoice.GetItemModifier jsTSKEH69WF3!');
            if(vData.length > 0){
                var pProductIds = [];
                for (var i = 0; i < vData.length; i++) {
                    var pProductID = vData[i].ProductID || vData[i].StockID;
                    pProductIds.push(pProductID);
                }

                var pSql = Business.LibraryItemModifiers.ModifierSql(pProductIds);
                var pDtModifiers = Sql.Execute(pSql);
                    
                for (var i = 0; i < vData.length; i++) {
                    var pProductID = vData[i].ProductID || vData[i].StockID;
                    var arrItemModifier = [];
                    var pData = [];
                    for (var pModifierIndex = 0; pModifierIndex <= pDtModifiers.Rows.length - 1; pModifierIndex++) {
                        var pDrModifier = pDtModifiers.Rows[pModifierIndex];
                        if ((ReadyGuid(pDrModifier.ProductID1) == ReadyGuid(pProductID))
                            || (ReadyGuid(pDrModifier.ProductID1) == ReadyGuid(pProductID))) {

                            pData.push(pDrModifier);
                        }
                    }

                    if (pData.length > 0) {
                        arrItemModifier = Business.LibraryItemModifiers.GetItemModifiers(pData[0].CategoryName, vData[i].ProductID || vData[i].StockID);
                        for (var m = 0; m < arrItemModifier.length; m++){
                            this.GetModifiersOfInvoiceItem(vData[i].ItemID, arrItemModifier[m]);
                        }
                    }
                    vData[i].HasModifier = pData.length > 0; // CBool(Business.LibraryItemModifiers.HasModifier(vData[i].ProductID || vData[i].StockID));
                    vData[i].SelectedItemModifierID = this.GetOnlyIDOfSelectedModifier(vData[i].ItemID);
                    vData[i].ItemModifier = arrItemModifier;
                }
            }
            console.timeEnd('Business.TransInvoice.GetItemModifier jsTSKEH69WF3!');
        }

        this.GetModifiersOfInvoiceItem = function (itemID, vData) {
            if ((vData.Items != null)
                && (vData.Items.length > 0)) {

                var pSql = "";
                vData.SelectedModifier = [];
                if (vData.Type == "Inventory Affecting") {
                    pSql = "SELECT ia.*, pim.ProductName, " + "\n" +
                        " mod.OrderedQty as OrderQty, \n" +
                        " mod.InputQty as InputQty \n" +
                        " ,LibraryItemModifiers.ChoicesReference \n" +
                        " ,IFNULL(mod.ModifierPrice, ia.UnitPrice) AS UnitPrice \n" +
                        " FROM TransInvoiceItemsModifier as mod \n" +
                        " LEFT JOIN LibraryItemModifierInventoryAffecting as ia ON ia.ID = mod.ModifierID \n" +
                        " LEFT JOIN TransInvoiceItems ON TransInvoiceItems .ID = mod.InvoiceItemID  \n" +
                        " LEFT JOIN LibraryPIMProduct as pim \n" +
                        " ON (ia.ProductID = pim.ID \n" +
                        " AND ia.ProductID IS NOT NULL \n" +
                        " ) OR (TransInvoiceItems.ProductID = pim.ID \n" +
                        " AND ia.ProductID IS NULL \n" +
                        " ) \n" +
                        " JOIN LibraryItemModifiers ON LibraryItemModifiers.id = mod.ItemModifierID \n" +
                        " WHERE mod.ItemModifierID = " + ReadyString(vData.ID) + " \n" +
                        " AND InvoiceItemID = " + ReadyString(itemID)
                    vData.SelectedModifier = Sql.Execute(pSql).Rows;
                } else if (vData.Type == "Non-Inventory Affecting") {
                    pSql = "SELECT nia.*," + "\n" +
                        " mod.OtherDescription as Others \n" +
                        " FROM TransInvoiceItemsModifier as mod \n" +
                        " INNER JOIN LibraryItemModifierNonInventoryAffecting as nia ON nia.ID = mod.ModifierID \n" +
                        " WHERE mod.ItemModifierID = " + ReadyString(vData.ID) + " \n" +
                        " AND InvoiceItemID = " + ReadyString(itemID)
                    vData.SelectedModifier = Sql.Execute(pSql).Rows;
                }
            }            
        }

        this.CopyModifiers = function(vSourceItemID, vDestinationItemID) {
            var pSql = 'SELECT * FROM TransInvoiceItemsModifier\n' +
                ' WHERE InvoiceItemID = ' + ReadyString(vSourceItemID);
            var pDT = Sql.Execute(pSql);
            for (var p = 0; p <= pDT.Rows.length - 1; p++) {
                var pDR = pDT.Rows[p];
                var pIU = new InsertUpdate('TransInvoiceItemsModifier');
                pIU.Add('ID', ReadyString(NewID()));
                pIU.Add('InvoiceItemID', ReadyString(vDestinationItemID));
                pIU.Add('ItemModifierID	', ReadyString(pDR.ItemModifierID));
                pIU.Add('ModifierID	', ReadyString(pDR.ModifierID));
                pIU.Add('InputQty	', CDec2(pDR.InputQty));
                pIU.Add('OrderedQty	', CDec2(pDR.OrderedQty));
                pIU.Add('ModifierPrice	', CDec2(pDR.ModifierPrice));
                pIU.Add('SubTotal	', CDec2(pDR.SubTotal));
                pIU.Add('OtherDescription	', ReadyString(pDR.OtherDescription));
                pSql = pIU.GetInsert();
                Sql.ExecuteSql(pSql);
            }
        };

        this.GetModifiersOfItem = function (vData) {
            Business.LibraryItemModifiers.GetModifiersOfItem(vData);
        }

        this.GetOnlyIDOfSelectedModifier = function (itemid) {
            var pDT = Sql.Execute("SELECT ModifierID as ID FROM TransInvoiceItemsModifier WHERE InvoiceItemID = " + ReadyString(itemid));
            var arr = [];
            for (var i = 0; i < pDT.Rows.length; i++) {
                arr.push(pDT.Rows[i].ID);
            }
            return arr;
        }

        this.LastInvoiceNumber = function (vOptions) {
            // requires sessionStorage.FowstID
            // vOptions = {
            //    FieldName: default = InvoiceNumber
            //    DateFieldName: optional
            // }

            var pSql;
            var pCounter;
            var pLog;

            if (Web.HandleOrganization() == false) return;
            vOptions = $.extend({
                InvoiceFieldName: 'InvoiceNumber'
            }, vOptions);

            pSql = 'SELECT MAX(' + vOptions.InvoiceFieldName + ')' +
                ' FROM TransInvoice' +
                ' WHERE FowstID = ' + ReadyString(sessionStorage.FowstID) +
                ' AND DeviceID = ' + ReadyString(Web.DeviceID());
            pCounter = CDec2(Sql.ExecuteScalar(pSql));
            pLog = 'jsU6WQGVSR8V! Last invoice number for fowst (' + sessionStorage.FowstID + ') and device (' + Web.DeviceID() + ') is ' + pCounter + '.';
            if (proj.LogInvoiceNumberCounter)
                Android.AddLog(pLog);
            return pCounter;

        }; // END LastInvoiceNumber

        this.VerifyCounter = function (vOptions) {
            // requires sessionStorage.FowstID
            // vOptions = {
            //  FieldName: default = POSLastORNo
            //    FieldName: default = InvoiceNumber
            //    DateFieldName: optional
            //    DateValue: optional date
            // }
            var pSql;
            var pCounter;
            var pError;
            var pLog; // used only if proj.LogInvoiceNumberCounter

            if (Web.HandleOrganization() == false) return;
            vOptions = $.extend({
                FieldName: 'POSLastORNo'
            }, vOptions);

            if (vOptions.DateFieldName != null) {
                if (vOptions.DateValue == null) {
                    pError = 'jsY6U2E7PAUP! VerifyCounter: DateValue must not be null if DateFieldName is not null';
                    console.error(pError);
                    throw pError;
                }

                if (!IsDate(vOptions.DateValue)) {
                    pError = 'jsFR5H5ZWZL9! VerifyCounter: DateValue must be a valid date if DateFieldName is not null';
                    console.error(pError);
                    throw pError;
                }
            }
            pSql = 'SELECT MAX(' + vOptions.FieldName + ')' +
                ' FROM LibraryDeviceFowst' +
                ' WHERE FowstID = ' + ReadyString(sessionStorage.FowstID) +
                ' AND DeviceID = ' + ReadyString(Web.DeviceID());
            pLog = 'jsUH5ZPKKQF8! Getting counter for fowst (' + sessionStorage.FowstID + ') and device (' + Web.DeviceID() + ') from sqlite counter table. ';
            if (CStr2(vOptions.DateFieldName) != '') {
                pLog += 'jsGBCK6NJ9AR! DateFieldName was specifed as ' + vOptions.DateFieldName + '. ';
                var pDate = ParseDate(vOptions.DateValue);
                var pYear = pDate.getFullYear();
                var pMonth = pDate.getMonth() + 1; // getMonth is 0-based
                var pDay = pDate.getDate(); // 1-based
                // https://www.sqlite.org/lang_datefunc.html
                pSql += " AND strftime('%Y', " + vOptions.DateFieldName + ' = ' + pYear + ')\n';
                pSql += " AND strftime('%m', " + vOptions.DateFieldName + ' = ' + pMonth + ')\n';
                pSql += " AND strftime('%d', " + vOptions.DateFieldName + ' = ' + pDay + ')\n';
            } else
                pLog += 'DateFieldName was not specified. ';
            pCounter = CDec2(Sql.ExecuteScalar(pSql));
            pLog += 'Counter = ' + pCounter + '. ';
            if (proj.LogInvoiceNumberCounter)
                Android.AddLog(pLog);
            return pCounter;

        }; // END VerifyCounter

        this.UpdateCounter = function (vValue, vOptions) {
            // PARAMS:
            //  vValue (optional). if null, updates value based on max invoice number or POSLastORNo, whichever is higher
            // vOptions (optional):
            //  .FieldName: default = POSLastORNo

            var pIU;
            var pSql;
            var pCounter;
            var pRecords;
            var pInvoiceNumber = 0;
            var pFileCounter = 0;

            if ((vValue != undefined)
                && (!IsNumeric(vValue))) {

                throw 'UpdateCounter parameter must be a valid number. You passed ' + vValue + '. jsARZ5G8Y7JK';
            }
            vOptions = $.extend({
                FieldName: 'POSLastORNo',
                InvoiceFieldName: 'InvoiceNumber',
                DontUseFileCounter: false,
                DontUseTableCounter: false
            }, vOptions);

            vValue = CDec2(vValue);
            pCounter = window.Business.TransInvoice.VerifyCounter(vOptions);
            if (!CBool(vOptions.DontUseTableCounter))
                pInvoiceNumber = CDec2(window.Business.TransInvoice.LastInvoiceNumber(vOptions));
            if (!CBool(vOptions.DontUseFileCounter))
                pFileCounter = CDec2(GetCounterFileContent(sessionStorage.FowstID, Web.DeviceID(), vOptions));

            vValue = Math.max(vValue, pCounter, pInvoiceNumber, pFileCounter);
            if (vValue > pCounter) {
                pIU = new InsertUpdate('LibraryDeviceFowst');
                pIU.Add(vOptions.FieldName, CDec2(vValue));
                pIU.Add('FlushToServer', 1);
                pSql = pIU.GetUpdate('FowstID = ' + ReadyString(sessionStorage.FowstID) +
                    ' AND DeviceID = ' + ReadyString(Web.DeviceID()));
                pRecords = Sql.ExecuteSql(pSql);
                if (pRecords == 0) {
                    pIU.Add('FowstID', ReadyString(sessionStorage.FowstID));
                    pIU.Add('DeviceID', ReadyString(Web.DeviceID()));
                    pSql = pIU.GetInsert();
                    pRecords = Sql.ExecuteSql(pSql); // result is not used. assigned only for debugging
                }
            }

            if (!CBool(vOptions.DontUseFileCounter))
                UpdateCounterFile(sessionStorage.FowstID, Web.DeviceID(), vValue, vOptions);

            return true;

        }; // END UpdateCounter

        this.GetCounter = function (vOptions) {

            var pCounter;
            var pInvoiceNumber = 0;
            var pFileCounter = 0;
            var pValue;

            vOptions = $.extend({
                FieldName: 'POSLastORNo',
                InvoiceFieldName: 'InvoiceNumber',
                DontUseFileCounter: false,
                DontUseTableCounter: false
            }, vOptions);

            pCounter = CDec2(window.Business.TransInvoice.VerifyCounter(vOptions));
            if (!CBool(vOptions.DontUseTableCounter))
                pInvoiceNumber = CDec2(window.Business.TransInvoice.LastInvoiceNumber(vOptions));
            if (!CBool(vOptions.DontUseFileCounter))
                pFileCounter = CDec2(GetCounterFileContent(sessionStorage.FowstID, Web.DeviceID(), vOptions));
            pValue = Math.max(pCounter, pInvoiceNumber, pFileCounter);

            return pValue;

        }; // END GetCounter

        // silent errors

        this.GetCounterFileContent = GetCounterFileContent;
        function GetCounterFileContent(vFowstID, vDeviceID, vOptions) {

            var pLog;
            var pOrgID;
            var pOut;

            try {
                vOptions = $.extend({
                    FieldName: 'POSLastORNo'
                }, vOptions);

                vFowstID = ReadyGuid(vFowstID);
                vDeviceID = ReadyGuid(vDeviceID);
                pOrgID = Web.OrgID();

                var pFileText = Transfers.ReadFile(mCounterFilename);
                var pFileJson;

                try {
                    pFileJson = JSON.parse(pFileText);
                } catch (e) {
                    pFileJson = {};
                }

                pOut = GetStringObject(pOrgID + '.' + vFowstID + '.' + vDeviceID + '.' + vOptions.FieldName, pFileJson);
                pLog = 'jsXEDS482DET! Last file number for org (' + pOrgID + ') and fowst (' + vFowstID + ') and device (' + vDeviceID + ') and (' + vOptions.FieldName + ') is ' + pOut + '.';
                if (proj.LogInvoiceNumberCounter)
                    Android.AddLog(pLog);
                return pOut;

                //// 1st try getting the org-fowst-fieldname combo
                //pOut = GetStringObject(pOrgID + '.' + vFowstID + '.' + vOptions.FieldName, pFileJson);
                //if (IsNumeric(pOut)) {
                //    pLog = 'jsXEDS482DET! Last file number for org (' + pOrgID + ') and fowst (' + vFowstID + ') and (' + vOptions.FieldName + ') is ' + pOut + '.';
                //    if (proj.LogInvoiceNumberCounter)
                //        Android.AddLog(pLog);
                //    return pOut;
                //}

                //// 2nd, try getting the org-fowst combo for backwards compatibility. 2018_03_17__10_26_27_9691
                //pOut = GetStringObject(pOrgID + '.' + vFowstID, pFileJson);
                //if (IsNumeric(pOut)) {
                //    pLog = 'jsL28ZF289YW! Last file number for org (' + pOrgID + ') and fowst (' + vFowstID + ') is ' + pOut + '.';
                //    if (proj.LogInvoiceNumberCounter)
                //        Android.AddLog(pLog);
                //    return pOut;
                //}

                //// try the fowst-device combo for backwards compatibility
                //pOut = GetStringObject(vFowstID + '.' + vDeviceID, pFileJson);
                //pLog = 'jsHGZ4XW8EHV! Last file number for fowst (' + vFowstID + ') and device (' + vDeviceID + ') is ' + pOut + '.';
                //if (proj.LogInvoiceNumberCounter)
                //    Android.AddLog(pLog);
                //return pOut;
            } catch (e) {
                Android.Toast(e.message + '\n' + e.stack);
            }

        } // END GetCounterFileContent

        // silent errors
        this.GetDatedCounterFileContent = function (vFowstID, vDeviceID, vDate) {

            try {
                vFowstID = ReadyGuid(vFowstID);
                vDeviceID = ReadyGuid(vDeviceID);

                var pFileText = Transfers.ReadFile(mDatedCounterFilename);
                var pFileJson;

                try {
                    pFileJson = JSON.parse(pFileText);
                } catch (e) {
                    pFileJson = {};
                }

                var pOut = GetStringObject(vFowstID + '.' + vDeviceID, pFileJson);
                if (pOut == null)
                    return 0;
                if (pOut.Date == $.format.date(new Date(vDate), 'yMMdd'))
                    return CInt2(pOut.Value);
                return 0;
            } catch (e) {
                Android.Toast(e.message + '\n' + e.stack);
            }

        } // END GetCounterFileContent

        // silent errors
        function UpdateCounterFile(vFowstID, vDeviceID, vValue, vOptions) {

            var pFileCounter;
            var pOrgID;
         
            try {
                vOptions = $.extend({
                    FieldName: 'POSLastORNo'
                }, vOptions);
                pOrgID = Web.OrgID();
                vValue = CDec2(vValue);
                pFileCounter = GetCounterFileContent(vFowstID, vDeviceID, vOptions);
                if ((vValue > 0) 
                    && (vValue <= CDec2(pFileCounter)))
                    return;
                if ((pFileCounter != null) 
                    && (vValue <= CDec2(pFileCounter)))
                    return;
                pFileCounter = CDec2(pFileCounter);
                vFowstID = ReadyGuid(vFowstID);
                vDeviceID = ReadyGuid(vDeviceID);

                var pFileText = Transfers.ReadFile(mCounterFilename);
                var pFileJson;

                try {
                    pFileJson = JSON.parse(pFileText);
                } catch (e) {
                    pFileJson = {};
                }
                // 2018_03_17__10_27_17_0552
                if (vOptions.FieldName != 'POSLastORNo') {
                    var pValue = GetStringObject(pOrgID + '.' + vFowstID + '.' + vDeviceID, pFileJson);
                    if (IsNumeric(pValue)) { // old format
                        CreateStringObject(pOrgID + '.' + vFowstID + '.' + vDeviceID + '.' + vOptions.FieldName, pFileJson, pValue);
                    }
                }
                CreateStringObject(pOrgID + '.' + vFowstID + '.' + vDeviceID + '.' + vOptions.FieldName, pFileJson, vValue);

                pFileText = JSON.stringify(pFileJson, null, 4);
                Transfers.WriteFile(mCounterFilename, null, pFileText);

                return true;
            } catch (e) {
                Android.Toast(e.message + '\n' + e.stack);
            }

        } // END UpdateCounterFile

        // silent errors
        this.UpdateDatedCounterFile = function (vFowstID, vDeviceID, vDate, vValue) {

            var pFileCounter;
         
            try {
                vValue = CDec2(vValue);
                pFileCounter = CDec2(Business.TransInvoice.GetDatedCounterFileContent(vFowstID, vDeviceID, vDate));
                if (vValue <= pFileCounter)
                    return;

                vFowstID = ReadyGuid(vFowstID);
                vDeviceID = ReadyGuid(vDeviceID);

                var pFileText = Transfers.ReadFile(mDatedCounterFilename);
                var pFileJson;

                try {
                    pFileJson = JSON.parse(pFileText);
                } catch (e) {
                    pFileJson = {};
                }
                var pContent = {};
                pContent[vDeviceID] = {
                    Date: $.format.date(new Date(vDate), 'yMMdd'),
                    Value: vValue
                };
                CreateStringObject(vFowstID, pFileJson, pContent);
                pFileText = JSON.stringify(pFileJson, null, 4);
                Transfers.WriteFile(mDatedCounterFilename, null, pFileText);

                return true;
            } catch (e) {
                Android.Toast(e.message + '\n' + e.stack);
            }

        } // END UpdateDatedCounterFile

        this.GetByID = function (vID) {

            var pSql;
            var pDT;

            pSql = 'SELECT InvoiceNumber, InvoiceDate, RefNum ' +
                ',CustomerID\n' +
                ',SalesOrderNo\n' +
                ',OrderSlipNum\n' +
                ',IsDraft\n' +
                ',LastUpdated\n' +
                ',TotalAmount\n' +
                ',PhNatTeamDiscount\n' +
                ',SoloParentDiscount\n' +
                ',SCDiscount\n' +
                ',SCDiscountExempt\n' +
                ',ForegoneVAT\n' +
                ',VAT\n' +
                ' FROM TransInvoice' +
                ' WHERE ID = ' + ReadyString(vID);
            pDT = Sql.Execute(pSql);
            if (pDT.Rows.length > 0)
                return pDT;

        };

        this.GetMaxLineNumber = function (vID) {

            var pSql;

            pSql = 'SELECT MAX(TransInvoiceItems.LineNumber) AS LineNumber' +
                ' FROM private.TransInvoiceItems' +
                ' WHERE TransInvoiceItems.InvoiceID = ' + ReadyString(vID);
            return CDec2(Sql.ExecuteScalar(pSql));

        };

        this.GetLastLastUpdatedDate = function () {

            var pOut;
            var pSql;

            pSql = 'SELECT LastUpdated' +
                ' FROM TransInvoice' +
                ' ORDER BY LastUpdated DESC' +
                ' LIMIT 1';
            pOut = Sql.ExecuteScalar(pSql);

            return pOut;

        }; // END GetLastLastUpdatedDate

        this.HandleCorrectDate = function (vParams) {
            //            vParams = {
            //                Write: true,
            //                Success: function() {
            //                }
            //            }
            // requires: Web.HandleOrganization()

            vParams = $.extend({}, vParams);

            var pLastLastUpdatedDate = this.GetLastLastUpdatedDate();
            var pLastSavedDate = Transfers.ReadFile('LastSaveDate.txt');
            var pDate;
            var pMessage = 'The system date and time is incorrect. ';
            var pCurrentDate;
            var pMinDate = new Date(2014, 10, 18); // month is 0-based

            try {
                if (IsDate(pLastLastUpdatedDate)) {
                    pLastLastUpdatedDate = ParseDate(pLastLastUpdatedDate);
                    // pMessage += 'Date of Last Invoice Issued: ' + $.format.date(pLastLastUpdatedDate, 'MMM d, yyyy h:mm:ss a') + '. ';
                } else {
                    pLastLastUpdatedDate = new Date(0);
                    pMessage += 'No local invoice found. ';
                }

                if (IsDate(pLastSavedDate)) {
                    pLastSavedDate = ParseDate(pLastSavedDate);
                    // pMessage += 'Last payment date: ' + $.format.date(pLastSavedDate, 'MMM d, yyyy h:mm:ss a') + '. ';
                } else {
                    pLastSavedDate = new Date(0);
                    pMessage += 'No last saved date.';
                }

                if (pLastLastUpdatedDate < pLastSavedDate)
                    pDate = pLastSavedDate;
                else
                    pDate = pLastLastUpdatedDate;
                if (pDate < pMinDate)
                    pDate = pMinDate;

                pCurrentDate = new Date();
                // pCurrentDate = new Date(2000, 0, 1); // debug only
                var pThresholdSeconds = 120;
                var pDateDiff = pCurrentDate - pDate;
                pDateDiff /= 1000;
                if (pDateDiff + pThresholdSeconds < 0) {
                    var pMaxApprovedDeviceDate = new Date(2019, 4, 1);
                    var pApproveOverDate = false;
                    if (pMaxApprovedDeviceDate != null) {
                        if (pDate <= pMaxApprovedDeviceDate)
                            pApproveOverDate = true;
                    }

                    if (!pApproveOverDate) {
                        pMessage += 'Your last transaction was on ' + $.format.date(pDate, 'MMM d, yyyy h:mm:ss a') + '. ';
                        pMessage += 'The current system date is ' + $.format.date(pCurrentDate, 'MMM d, yyyy h:mm:ss a') + '. ';
                        pMessage += 'The system date must be LATER than the last transaction date. All devices must have the same date and time. Please adjust your system date and time in order to proceed. ';

                        if (pMaxApprovedDeviceDate != null)
                            pMessage += 'The latest approved over-date is ' + $.format.date(pMaxApprovedDeviceDate, 'MMM d, yyyy h:mm:ss a') + '. ';
                        DisplayMessage(pMessage);

                        return false;
                    }
                } else if (CBool(vParams.Write)) {
                    Transfers.WriteFile('LastSaveDate.txt', null, $.format.date(pCurrentDate, 'MMM d, yyyy h:mm:ss a'));
                }

                if ($.isFunction(vParams.Success))
                    vParams.Success();
                return true;
            } catch (e) {
                pMessage = 'Unable to determine the correct time. ' + e;
                DisplayMessage(pMessage);
                return false;
            }

            function DisplayMessage(vMessage) {

                MessageBox(vMessage, 'Incorrect Date or Time', [
                    {
                        Text: 'Adjust date + time',
                        Click: function () {
                            Device.ShowDateSettingsPage();
                            setTimeout(CheckDate, 1000);

                            function CheckDate() {

                                pCurrentDate = new Date();
                                // pCurrentDate = new Date(2000, 0, 1); // debug only
                                if (pCurrentDate < pDate) {
                                    setTimeout(CheckDate, 1000);
                                } else {
                                    history.back();
                                    if ($.isFunction(vParams.Success))
                                        vParams.Success();
                                }

                            } // END CheckDate
                        } // END Click
                    }
                ]); // END MessageBox

            } // END DisplayMessage

        }; // END HandleCorrectDate

        this.AssignSalesRepresentatives = function (vID, vSalesRepresentatives) {

            var pDtItems;
            var pDrItem;
            var pDtSalesRepresentatives;
            var pDrSalesRepresentative;
            var pSql;
            var pIndexItem;
            var pIndexSalesRepresentative;
            var pIndexNewSalesRepresentative;
            var pNewSalesRepresentative;
            var pFound = false;
            var pIU;
            var pIuCount = 0;

            try {
                pSql = 'SELECT ID' +
                    ',ProductID' +
                    ' FROM TransInvoiceItems' +
                    ' WHERE InvoiceID = ' + ReadyString(vID);
                pDtItems = Sql.Execute(pSql);

                pSql = 'SELECT PeopleID' +
                    ',InvoiceItemID' +
                    ' FROM TransInvoiceItemSalesReps' +
                    ' WHERE InvoiceID = ' + ReadyString(vID);
                pDtSalesRepresentatives = Sql.Execute(pSql);

                for (pIndexItem in pDtItems.Rows) {
                    pDrItem = pDtItems.Rows[pIndexItem];

                    for (pIndexNewSalesRepresentative in vSalesRepresentatives) {
                        pNewSalesRepresentative = vSalesRepresentatives[pIndexNewSalesRepresentative];
                        pFound = false;
                        for (pIndexSalesRepresentative in pDtSalesRepresentatives.Rows) {
                            pDrSalesRepresentative = pDtSalesRepresentatives.Rows[pIndexSalesRepresentative];
                            if (ReadyGuid(pDrSalesRepresentative.PeopleID) == ReadyGuid(pNewSalesRepresentative.ID)) {
                                pFound = true;
                                break;
                            }
                        } // END for (pIndexSalesRepresentative in pDtSalesRepresentatives) {

                        if (!pFound) {
                            pIU = new InsertUpdate('TransInvoiceItemSalesReps');
                            if (pIuCount == 0)
                                Sql.BeginTransaction();
                            pIuCount++;
                            pIU.Add('ID', ReadyString(NewID()));
                            pIU.Add('InvoiceItemID', ReadyString(pDrItem.ID));
                            pIU.Add('PeopleID', ReadyString(pNewSalesRepresentative.ID));
                            pIU.Add('InvoiceID', ReadyString(vID));
                            pIU.Add('ProductID', ReadyString(pDrItem.ProductID));
                            pSql = pIU.GetInsert();
                            Sql.Execute(pSql);
                        }
                    } // END for (pIndexNewSalesRepresentative in vSalesRepresentatives) {
                } // END for (pIndex in pDtItems.Rows) {

                if (pIuCount > 0) {
                    Business.TransInvoice.DeleteDuplicateSalesRepresentatives();
                    Sql.CommitTransaction();
                }
            } catch (e) {
                if (pIuCount > 0)
                    Sql.RollbackTransaction();
                throw e;
            }

        };

        this.DeleteDuplicateSalesRepresentatives = function () {

            var pSql;

            pSql = ' DELETE FROM TransInvoiceItemSalesReps' + '\n' +
                ' WHERE rowid NOT IN' + '\n' +
                '   (SELECT MIN(rowid)' + '\n' +
                '   FROM TransInvoiceItemSalesReps' + '\n' +
                '   GROUP BY InvoiceItemID' + '\n' +
                '   ,PeopleID' + '\n' +
                ' )';

            pSql = Sql.ExecuteSql(pSql);
            if (CDec2(pSql) > 0)
                Android.AddLog(pSql + ' duplicate records from TransInvoiceItemSalesReps were deleted.');

            return pSql;
        }; // END DeleteDuplicateSalesRepresentatives

        this.RemoveSalesRep = function (vData) {
            var pSql;

            pSql = "DELETE FROM TransInvoiceItemSalesReps WHERE PeopleID = " + ReadyString(vData.PeopleID) +
                    " AND InvoiceID = " + ReadyString(vData.InvoiceID);

            pSql = Sql.ExecuteSql(pSql);
        }; // END RemoveSalesRep

        this.GetRebates = function (vModifierGroupID) {
            var pSql;
            var pDT;

            pSql = 'SELECT *\n' +
                ' FROM TransInvoiceItemsRebates\n' +
                ' WHERE ModifierGroupID = ' + ReadyString(vModifierGroupID);
            pDT = Sql.Execute(pSql);
            if (pDT.Rows.length > 0)
                return pDT.Rows[0];

        }; // END GetRebates

        this.GetModifierGroupID = function (vInvoiceID, vProductID, vRebates) {

            var pDrCurrent;
            var pDrRebate;

            if (!$.isArray(vRebates))
                return;
            if (vRebates.length == 0)
                return;

            pSql = '';
            for (var pRebateIndex = 0; pRebateIndex <= vRebates.length - 1; pRebateIndex++) {
                pDrRebate = vRebates[pRebateIndex];
                if (pSql != '')
                    pSql += ', ';
                pSql += ReadyString(pDrRebate.ID);
            }

            pSql = 'SELECT COUNT(*) AS RecordCount\n' +
                ',ModifierGroupID\n' +
                ' FROM TransInvoiceItemsRebates\n' +
                ' WHERE ModifierGroupID IN (\n' +
                '   SELECT ModifierGroupID\n' +
                '   FROM TransInvoiceItems\n' +
                '   WHERE InvoiceID = ' + ReadyString(vInvoiceID) + '\n' +
                '   AND ProductID = ' + ReadyString(vProductID) + '\n' +
                ') AND RebateID IN (' + pSql + ')\n' +
                ' GROUP BY ModifierGroupID\n' +
                ' HAVING COUNT(*) = ' + vRebates.length;
            pDT = Sql.Execute(pSql);
            if (pDT.Rows.length == 0)
                return;
            return pDT.Rows[0].ModifierGroupID;

        }; // END GetModifierGroupID

        function UpdateDraft(vID, vIsDraft) {

            var pSql;
            var pIU = new InsertUpdate('TransInvoice');
            pIU.Add('IsDraft', CBool(vIsDraft) ? 1 : 0);
            pIU.Add('LastUpdated', proj.SqliteNow);
            pIU.Add('UserID', ReadyString(Web.PeopleUserID()));
            pSql = pIU.GetUpdate('ID = ' + ReadyString(vID));
            Sql.ExecuteSql(pSql);

        }

        this.SaveAsDraft = function (vID) {
            UpdateDraft(vID, 1);
        };

        this.SaveAndFinalize = function (vID) {
            UpdateDraft(vID, 0);
        };

        this.TestPrice = function (vParams) {
            /*
            vParams = {
            ID: 'guid',
            Callback: function(vResult) {
            vResult = {
            IsOk: true/false,
            Message: 'string'
            }
            }
            }
            */

            var pDS = Business.TransInvoice.GetCart(vParams.ID);
            var pData = {};
            var pProduct;

            if (pDS.Invoices.Rows.length == 0) {
                vParams.Callback({
                    IsOk: false,
                    Message: 'jsJ8CKBKFX7L! No invoice with that ID'
                });
            }

            if (pDS.Stocks.Rows.length == 0) {
                vParams.Callback({
                    IsOk: false,
                    Message: 'jsXHDQM2ULWF! That invoice has no products'
                });
            }

            pData.CustomerID = pDS.Invoices.Rows[0].CustomerID;
            pData.Products = [];
            for (var pProductIndex = 0; pProductIndex <= pDS.Stocks.Rows.length - 1; pProductIndex++) {
                pProduct = pDS.Stocks.Rows[pProductIndex];
                pData.Products.push({
                    StockID: pProduct.ProductID,
                    PriceLevelID: pProduct.PriceLevelID,
                    ActualQuantity: pProduct.ActualQuantity,
                    Quantity: (CDec2(pProduct.Quantity) > 0) ? pProduct.Quantity : pProduct.OrderedQuantity,
                    UnitPrice: pProduct.UnitPrice,
                    UnitMeasure: pProduct.UnitMeasure
                });
            }

            AjaxSend({
                url: '/SalesMenu/TestPrice',
                data: pData,
                success: function (vData) {
                    vParams.Callback(vData);
                },
                error: function () {
                    vParams.Callback({
                        Message: '"Test requested price" cannot connect to the server. This transaction requires a connection to the server.'
                    });
                }
            });
        };

        this.RemoveCartStock = function (vData) {
            // for hot buttons only

            var pItem;

            vData.ProductID = ReadyGuid(vData.ProductID);
            vData.ModifierGroupID = ReadyGuid(vData.ModifierGroupID);

            for (var pStockIndex in proj.CartStock) {
                if (proj.CartStock.hasOwnProperty(pStockIndex)) {
                    pItem = proj.CartStock[pStockIndex];
                    if ((pItem.ProductID == vData.ProductID)
                        && (pItem.ModifierGroupID == vData.ModifierGroupID)) {

                        // proj.CartStock.splice(pStockIndex, 1); // doesnt work
                        pItem.Quantity = 0;
                        break;
                    }
                }
            }

            for (var pDataIndex in GetStringObject('proj.CartData.Units.Rows')) {
                if (proj.CartData.Units.Rows.hasOwnProperty(pDataIndex)) {
                    pItem = proj.CartData.Units.Rows[pDataIndex];
                    if ((ReadyGuid(pItem.ProductID) == vData.ProductID)
                        && (ReadyGuid(pItem.ModifierGroupID) == vData.ModifierGroupID)) {

                        pItem.OrderedQuantity = 0;
                        pItem.Quantity = 0;
                    }
                }
            }

        }

        this.TagUntestedFreeTypeAPrice = function (vID) {

            var pIU;
            var pSql;

            pIU = new InsertUpdate('TransInvoice');
            pIU.Add('UntestedFreeTypeAPrice', 1);
            pIU.Add('IsDraft', 1);
            pSql = pIU.GetUpdate('ID = ' + ReadyString(vID));
            Sql.ExecuteSql(pSql);

        };

        this.HandleTestFreeTypeAPrice = function (vParams) {
            /*
            vParams = {
            Callback: function() {
            // allow the user to proceed
            }
            }
            */

            if (!Business.SystemDefaults.AlwaysTestFreeTypeAPrice()) {
                vParams.Callback();
                return;
            }

            if (!HasUntestedFreeTypeAPrice()) {
                vParams.Callback();
                return;
            }

            SalesMenu_offline().TestPrice({
                Callback: vParams.Callback
            });

            function HasUntestedFreeTypeAPrice() {
                var pSql;

                pSql = 'SELECT IFNULL(UntestedFreeTypeAPrice, 0)\n' +
                    ' FROM TransInvoice\n' +
                    ' WHERE ID = ' + ReadyString(sessionStorage.CartOrderID);
                pSql = Sql.ExecuteScalar(pSql);

                return CBool(pSql);
            }
        };

        this.CreatePdf = function(vParams) {
            /*
            vParams = {
                ID: 'invoice ID',
                callback: function() {
                }
            };
            */

            var pData = {};
            pData.PosMode = "pos";
            pData.OrderID = vParams.ID;

            AjaxSend({
                url: '/SalesMenu/PosCart',
                data: pData,
                success: function (vData) {
                    Create(vData);
                }
            }); // AjaxSend

            function Create(vData) {
                var pPdfOperation;
                var pPdfData = {
                    columns: [],
                    rows: []
                };
                var pInvoice = vData.Master;
                var pStr;

                pPdfOperation = [
                    {caseOperation: 'salesInvoice'},
                    {docType: 'siNum'},
                    {docNum: pInvoice.DucInvoiceNum},
                    {docTypeDate: 'siDate'},
                    {dateData: $.format.date(new Date(FormatDate(pInvoice.InvoiceDate)), 'EEEE, MMM d, yyyy')},
                    {docTypeDelivery: 'siDeliveryDate'},
                    {deliveryDate: $.format.date(new Date(FormatDate(pInvoice.DeliveryDate)), 'EEEE, MMM d, yyyy')},
                    {docTypeTotal: 'siTotal'},
                    {totalData: AccountingFormat(pInvoice.Subtotal)},
                    {siTotal: AccountingFormat(pInvoice.Subtotal)},
                    {siTotalSales: AccountingFormat(pInvoice.Subtotal / 1.12)},
                    {siTotalVat: AccountingFormat(pInvoice.Subtotal - pInvoice.Subtotal / 1.12)},
                    {orgNameRaw: pInvoice.OrgName}, 
                    {orgAddressRaw: pInvoice.OrgFowst}, 
                    {custNameRaw: pInvoice.CustomerName},
                    {custAddressRaw: pInvoice.DeliveryAddress}, 
                    {billingAddressDetailsRaw: pInvoice.BillingAddress}, 
                    {billingContactDetailsRaw: pInvoice.BillingContactInfo}, 
                    {billingContactOptionalRaw: ''},
                    {deliveryAddressDetailsRaw: pInvoice.DeliveryAddress}, 
                    {deliveryContactDetailsRaw: pInvoice.DeliveryContactInfo},
                    {deliveryContactOptionalRaw: ''},
                    {deliveryRemarksRaw: pInvoice.Remarks},
                    {countDate: '081515'},
                    {qrcode: 'SI|' + sessionStorage.OrgID + '|' + sessionStorage.FowstID + '|CHI-CAS ' + SetPrefixPostfix(pInvoice.DucInvoiceNum, 12, '0', 0)},
                    {userNote: 'BIR Computerized Accounting System Permit #: 0611-132-00089-000  Issued on June 23, 2011'}
                ];

                AddProductColumns(vData.Units.Rows, vData.Stocks.Rows);
                var pProducts = vData.Units.Rows;
                var pColumns = [
                    ['Item Code:', 'ProductNo'],
                    ['Item Description:', 'ProductName'],
                    ['Quantity', 'Quantity', true],
                    ['UoM', 'UnitMeasure'],
                    ['Unit Price', 'UnitPrice', true],
                    ['Sub Total', 'SubTotal', true]
                ];
                var pRow;

                for (var pX = 0; pX <= pColumns.length - 1; pX++) {
                    pPdfData.columns.push(pColumns[pX][0]);
                }

                for (var pY = 0; pY <= pProducts.length - 1; pY++) {
                    pRow = [];
                    for (var pX = 0; pX <= pColumns.length - 1; pX++) {
                        pStr = pProducts[pY][pColumns[pX][1]];
                        if (pColumns[pX][2])
                            pStr = AccountingFormat(pStr);
                        pRow.push(pStr);
                    }
                    pPdfData.rows.push(pRow);
                }

                var pCreation = Android.PDFCreation(
                    {
                        operation: pPdfOperation
                    }, 
                    {
                        data: [
                            {
                                columns: pPdfData.columns
                            },
                            {
                                rows: pPdfData.rows
                            }
                        ]
                    }
                );

                vParams.Callback(pCreation);

                function AddProductColumns(vUnits, vStocks) {
                    var pProduct;
                    var pUnit;

                    for (var p = 0; p <= vUnits.length - 1; p++) {
                        pUnit = vUnits[p];
                        pProduct = FindProduct(pUnit.ProductID, vStocks);
                        if (pProduct == null) {
                            console.log('jsW5CPPLHQNG! Product not found');
                        } else {
                            pUnit.ProductNo = pProduct.ProductNo;
                            pUnit.ProductName = pProduct.ProductName;
                        }
                    }
                } // END function AddProductColumns(vUnits, vStocks) {

                function FindProduct(vProductID, vSourceArray) {
                    vProductID = ReadyGuid(vProductID);
                    for (var p = 0; p <= vSourceArray.length - 1; p++) {
                        if (ReadyGuid(vSourceArray[p].ProductID) == vProductID)
                            return vSourceArray[p];
                    }
                } // END function FindProduct(vProductID, vSourceArray) {

            } // END function Create() {
        }; // END this.CreatePdf = function(vID) {

        this.SalesOrderHeaderDataForPrinting = function(vID) {
            // also for purchase order

            var pSql;
            var pDT;

            pSql = ' SELECT TransInvoice.ID\n' + 
                ',TransInvoice.SalesOrderNo' + '\n' + 
                ',TransInvoice.SalesOrderNo' + '\n' + 
                ' ,TransInvoice.SalesOrderDate' + '\n' + 
                ' ,TransInvoice.DeliveryDate' + '\n' + 
                ' ,TransInvoice.Remarks' + '\n' + 
                ' ,TransInvoice.CustomerTableNo' + '\n' + 
                ' , IFNULL(NULLIF(LibraryOrganizationFOWST.[StreetNumber] || \', \', \', \'), \'\')' + '\n' + 
                ' || IFNULL(NULLIF(LibraryOrganizationFOWST.[Street] || \', \', \', \'), \'\')' + '\n' + 
                ' || IFNULL(LibraryCities.[Name], \'\') || \', \'' + '\n' + 
                ' || IFNULL(LibraryCities.[Zip], \'\') || \', \'' + '\n' + 
                ' || LibraryCountries.[Name] AS BillingAddress' + '\n' + 
                ' ,(' + '\n' + 
                '    SELECT IFNULL(NULLIF(LibraryOrganizationFOWST.[StreetNumber] || \', \', \', \'), \'\')' + '\n' + 
                '    || IFNULL(NULLIF(LibraryOrganizationFOWST.[Street] || \', \', \', \'), \'\')' + '\n' + 
                '    || IFNULL(LibraryCities.[Name], \'\') || \', \'' + '\n' + 
                '    || IFNULL(LibraryCities.[Zip], \'\') || \', \'' + '\n' + 
                '    || LibraryCountries.[Name] AS BillingAddress' + '\n' + 
                '    FROM LibraryOrganizationFOWST' + '\n' + 
                '    LEFT JOIN LibraryCities ON LibraryOrganizationFOWST.[FowstCityID] = LibraryCities.[ID]' + '\n' + 
                '    LEFT JOIN LibraryCountries ON LibraryOrganizationFOWST.[FowstCountryID] = LibraryCountries.[ID]' + '\n' + 
                '    LEFT JOIN LibraryOrganizationFOWSTTel ON LibraryOrganizationFOWST.[ID] = LibraryOrganizationFOWSTTel.[FOWSTID]' + '\n' + 
                '    WHERE LibraryOrganizationFOWST.ID = TransInvoice.DeliveryAddressID' + '\n' + 
                ' ) AS DeliveryAddress' + '\n' + 
                ' , (' + '\n' + 
                '    SELECT LibraryOrganization.[CompanyName]' + '\n' + 
                '    FROM LibraryOrganizationFOWST' + '\n' + 
                '    LEFT JOIN LibraryOrganization ON LibraryOrganizationFOWST.[OrganizationID] = LibraryOrganization.[ID]' + '\n' + 
                '    WHERE LibraryOrganizationFOWST.[ID] = TransInvoice.FowstID' + '\n' + 
                ' ) AS OrgName' + '\n' + 
                ' , (' + '\n' + 
                '    SELECT IFNULL(NULLIF(LibraryOrganization.[OfficeAddressBuildingNo] || \', \', \', \'), \'\')' + '\n' + 
                '    || IFNULL(NULLIF(LibraryOrganization.[OfficeAddressStreet] || \', \', \', \'), \'\')' + '\n' + 
                '    || IFNULL(LibraryCities.[Name], \'\') || \', \'' + '\n' + 
                '    || IFNULL(LibraryCities.[Zip], \'\') || \', \'' + '\n' + 
                '    || LibraryCountries.[Name]' + '\n' + 
                '    FROM LibraryOrganizationFOWST' + '\n' + 
                '    LEFT JOIN LibraryOrganization ON LibraryOrganizationFOWST.[OrganizationID] = LibraryOrganization.[ID]' + '\n' + 
                '    LEFT JOIN LibraryCities ON LibraryOrganization.[OfficeAddressCityID] = LibraryCities.[ID]' + '\n' + 
                '    LEFT JOIN LibraryCountries ON LibraryOrganization.[OfficeAddressCountryID] = LibraryCountries.[ID]' + '\n' + 
                '    WHERE LibraryOrganizationFOWST.[ID] = TransInvoice.FowstID' + '\n' + 
                ' ) AS OrgAddress' + '\n' + 
                ' , (' + '\n' + 
                '    SELECT LibraryOrganization.[CompanyName]' + '\n' + 
                '    FROM LibraryOrganization ' + '\n' + 
                '    WHERE LibraryOrganization.ID = TransInvoice.CustomerID' + '\n' + 
                ' ) AS CustomerOrgName' + '\n' + 
                ' , (' + '\n' + 
                '    SELECT IFNULL(NULLIF(LibraryOrganization.[OfficeAddressBuildingNo] || \', \', \', \'), \'\')' + '\n' + 
                '    || IFNULL(NULLIF(LibraryOrganization.[OfficeAddressStreet] || \', \', \', \'), \'\')' + '\n' + 
                '    || IFNULL(LibraryCities.[Name], \'\') || \', \'' + '\n' + 
                '    || IFNULL(LibraryCities.[Zip], \'\') || \', \'' + '\n' + 
                '    || LibraryCountries.[Name]' + '\n' + 
                '    FROM LibraryOrganization' + '\n' + 
                '    LEFT JOIN LibraryCities ON LibraryOrganization.[OfficeAddressCityID] = LibraryCities.[ID]' + '\n' + 
                '    LEFT JOIN LibraryCountries ON LibraryOrganization.[OfficeAddressCountryID] = LibraryCountries.[ID]' + '\n' + 
                '    WHERE LibraryOrganization.ID = TransInvoice.CustomerID' + '\n' + 
                ' ) AS CustomerOrgAddress' + '\n' + 
                ' ,TransInvoice.[Remarks]' + '\n' + 
                ' ,IFNULL(TransInvoice.SalesOrderSignatory, \'\') AS SalesOrderSignatory' + '\n' + 
                ' ,TransInvoice.[TotalAmount]' + '\n' +
                ',LibraryPeople.FirstName || \' \' || LibraryPeople.Surname AS CreatedBy' + '\n' +
                '  ,(' + '\n' + 
                '     SELECT SUM(TransInvoiceItems.OrderedQuantity * TransInvoiceItems.UnitPrice)' + '\n' + 
                ' FROM TransInvoiceItems' + '\n' + 
                ' WHERE TransInvoiceItems.InvoiceID = TransInvoice.ID' + '\n' + 
                ' ) AS TotalOrderedAmount\n';

            // purchase order new fields
            pSql += ' ,TransInvoice.PurchaseOrderNo' + '\n'; 
            pSql += ' ,SupplierOrg.CompanyName AS SupplierOrgName' + '\n' + 
                ' ,SupplierFowst.Name AS SupplierFowstName' + '\n' + 
                ' ,SupplierBin.Name AS SupplierBinName' + '\n' + 
                ' ,CustomerFowst.Name AS CustomerFowstName' + '\n' + 
                ' ,CustomerBin.Name AS CustomerBinName\n';

            pSql += ' FROM TransInvoice' + '\n' + 
                ' LEFT JOIN LibraryOrganizationFOWST ON TransInvoice.[BillingAddressID] = LibraryOrganizationFOWST.[ID]' + '\n' + 
                ' LEFT JOIN LibraryCities ON LibraryOrganizationFOWST.[FowstCityID] = LibraryCities.[ID]' + '\n' + 
                ' LEFT JOIN LibraryCountries ON LibraryOrganizationFOWST.[FowstCountryID] = LibraryCountries.[ID]' + '\n';
                
            pSql += ' LEFT JOIN LibraryOrganization SupplierOrg ON SupplierOrg.ID = TransInvoice.SupplierID' + '\n' + 
                ' LEFT JOIN LibraryOrganizationFOWST SupplierFowst ON SupplierFowst.ID = TransInvoice.SupplierFowstID' + '\n' + 
                ' LEFT JOIN LibraryFowstBin SupplierBin ON SupplierBin.ID = TransInvoice.SupplierBinID' + '\n' + 
                ' LEFT JOIN LibraryOrganizationFOWST CustomerFowst ON CustomerFowst.ID = TransInvoice.CustomerFowstID' + '\n' + 
                ' LEFT JOIN LibraryFowstBin CustomerBin ON CustomerBin.ID = TransInvoice.CustomerBinID\n' + 
                ' LEFT JOIN LibraryPeople ON LibraryPeople.ID = TransInvoice.UserID\n';

            pSql +=' WHERE TransInvoice.[ID]= ' + ReadyString(vID);
            pDT = Sql.Execute(pSql);

            return pDT;

        };

        this.SalesOrderProductsDataForPrinting = function(vID) {

            var pDT;
            var pDS;

            pDS = this.GetReportData(' WHERE TransInvoice.ID = ' + ReadyString(vID));
            pDT = pDS.Stocks;

            return pDT;
        };

        this.CreateSalesOrderPdf = function(vID, vParams) {
            vParams = $.extend({
                IsPurchaseOrder: false
            }, vParams);
            var pDT = this.SalesOrderHeaderDataForPrinting(vID);
            var pData = {};

            if (pDT.Rows.length == 0)
                return false;

            pData.IsPurchaseOrder = CBool(vParams.IsPurchaseOrder);
            pData.Header = pDT.Rows[0];
            pData.Header = AccountingFormatColumns(pData.Header, ['TotalAmount']);
            pData.Header = FormatColumns(pData.Header, ['SalesOrderDate', 'DeliveryDate'],
                function(vValue) {
                    if (IsDate(vValue))
                        vValue = $.format.date(ParseDate(vValue), 'EEEE, MMM d, yyyy');
                    return vValue;
                }
            );
            pData.Header = pData.Header[0];

            pDT = this.SalesOrderProductsDataForPrinting(vID);
            pData.Products = pDT.Rows;
            pData.Products = AccountingFormatColumns(pData.Products, ['UnitPrice', 'SubTotal']);
            pData.Products = AccountingFormatColumns(pData.Products, ['OrderedQuantity'], 0);

            Android.CreatePdf('SalesOrder', pData);

        }; // END this.CreateSalesOrderPdf = function(vID) {

        this.GetCategories = function(vID, vOptions) {
            // PARAMS:
            //  vOptions (optional)
            var pSql;
            var pOut;

            vOptions = $.extend({
                PrintAsOrderSlip: false
            }, vOptions);

            pSql = ' SELECT LibraryPIMProductVariableFeatures001.C0020  AS Category' + '\n' + 
                ' FROM private.TransInvoiceItems   ' + '\n' + 
                ' LEFT JOIN LibraryPIMProductVariableFeatures001 ON LibraryPIMProductVariableFeatures001.ProductID = TransInvoiceItems .ProductID' + '\n' + 
                ' WHERE TransInvoiceItems .InvoiceID = ' + ReadyString(vID) + '\n';
             
            if (vOptions.PrintAsOrderSlip && !vOptions.IsReprintAll) 
                pSql += ' AND IFNULL(TransInvoiceItems.IsOrderSlipPrinted, 0) = 0\n';

            pSql += ' GROUP BY LibraryPIMProductVariableFeatures001.C0020' + '\n' + 
                ' ORDER BY LibraryPIMProductVariableFeatures001.C0020 ';
            pOut = Sql.Execute(pSql);

            return pOut;

        };

        // this should be in LibraryPrinters
        this.GetPrinters = function (vFowstID, vCategory, IsUsingWPS) {
            var pSql;
            var pDT;

            if (vCategory != null)
                pSql = '  JOIN private.LibraryDevicePrinterCategory ON LibraryDevicePrinterCategory.PrinterFowstID = LibraryDevicePrinterFowst.ID' + '\n' + 
                    ' AND LibraryDevicePrinterCategory.CategoryName = ' + ReadyString(vCategory) + '\n';

            pSql = ' SELECT LibraryDevicePrinterFowst.PrinterIPAddress' + '\n' + 
                ' ,LibraryDevicePrinterCategory.CategoryName' + '\n' + 
                ' ,LibraryDevicePrinterFowst.PrinterID' + '\n' + 
                ' ,LibraryDevicePrinterFowst.ConnectionType' + '\n' + 
                ' ,LibraryDevicePrinter.PrinterType' + '\n' +
                ' ,LibraryDevicePrinterFowst.PrinterName' + '\n' +
                '  FROM LibraryDevicePrinterFowst' + '\n' + 
                pSql +
                '  JOIN LibraryDevicePrinter ON LibraryDevicePrinter.ID = LibraryDevicePrinterFowst.PrinterID' + '\n' + 
                ' WHERE LibraryDevicePrinterFowst.FOWSTID = ' + ReadyString(vFowstID) + '\n';

            if (IsUsingWPS) {
                pSql += ' AND LibraryDevicePrinter.PrinterType = \'Windows Print Server\'';
            } else {
                pSql += ' AND LibraryDevicePrinter.PrinterType != \'Windows Print Server\'';
            }

            pDT = Sql.Execute(pSql);

            return pDT;
        };

        this.GetCategoryPrinters = function(vOptions) { 

            var pOut;
            var pSql;

            vOptions = $.extend({
                InvoiceID: null,
                PrintAsOrderSlip: false,
                FowstID: null
            }, vOptions);

            pSql = ' SELECT LibraryPIMProductVariableFeatures001.C0020  AS Category' + '\n' + 
                ', tblPrinters.PrinterIPAddress' + '\n' + 
                ' ,tblPrinters.CategoryName' + '\n' + 
                ' ,tblPrinters.PrinterID' + '\n' + 
                ' ,tblPrinters.ConnectionType' + '\n' + 
                ' ,tblPrinters.PrinterType' + '\n' +
                ' ,tblPrinters.PrinterName' + '\n' +
                ' FROM private.TransInvoiceItems   ' + '\n' + 
                ' LEFT JOIN LibraryPIMProductVariableFeatures001 ON LibraryPIMProductVariableFeatures001.ProductID = TransInvoiceItems .ProductID' + '\n';
            
            pSql += ' LEFT JOIN (SELECT LibraryDevicePrinterFowst.PrinterIPAddress' + '\n' + 
                ' ,LibraryDevicePrinterCategory.CategoryName' + '\n' + 
                ' ,LibraryDevicePrinterFowst.PrinterID' + '\n' + 
                ' ,LibraryDevicePrinterFowst.ConnectionType' + '\n' + 
                ' ,LibraryDevicePrinter.PrinterType' + '\n' +
                ' ,LibraryDevicePrinterFowst.PrinterName' + '\n' +
                '  FROM LibraryDevicePrinterFowst' + '\n' + 
                '  JOIN private.LibraryDevicePrinterCategory ON LibraryDevicePrinterCategory.PrinterFowstID = LibraryDevicePrinterFowst.ID' + '\n' + 
                ' AND LibraryDevicePrinterCategory.CategoryName IN ( ' + pSql + ')\n' +
                '  JOIN LibraryDevicePrinter ON LibraryDevicePrinter.ID = LibraryDevicePrinterFowst.PrinterID' + '\n' + 
                ' WHERE LibraryDevicePrinterFowst.FOWSTID = ' + ReadyString(vOptions.FowstID) + '\n' +
                ' ORDER BY LibraryDevicePrinterFowst.PrinterIPAddress\n' +
                ' ,LibraryDevicePrinterCategory.CategoryName\n' +
                ') tblPrinters ON tblPrinters.CategoryName = LibraryPIMProductVariableFeatures001.C0020\n';

            pSql += ' WHERE TransInvoiceItems .InvoiceID = ' + ReadyString(vOptions.InvoiceID) + '\n';

            pSql += ' AND IFNULL(TransInvoiceItems.IsOrderSlipPrinted, 0) = 0\n';

            pSql += ' GROUP BY LibraryPIMProductVariableFeatures001.C0020' + '\n' + 
                ' ORDER BY LibraryPIMProductVariableFeatures001.C0020 ';

            
            pOut = Sql.Execute(pSql);

            return pOut;
        };

        this.AddVoidItem = function(vParams) {
            var pSql;
            var pID;
            var pUserID;

            pUserID = Web.PeopleUserID();
            if (!IsGuid(pUserID))
                throw 'Web.PeopleUserID must be GUID jsV3BYBJECCR!';
            if (!IsGuid(vParams.InvoiceID))
                throw 'vParams.InvoiceID must be GUID jsUQMVMM4X92!';
            if (!IsGuid(vParams.ProductID))
                throw 'vParams.ProductID must be GUID jsEFLMUKUU3M!';
            if (!IsNumeric(vParams.Quantity))
                throw 'vParams.Quantity must be numeric js8UCFMCWTAX!';
            vParams.Quantity = CDec2(vParams.Quantity);
            if (!IsNumeric(vParams.ActualQuantity))
                throw 'vParams.ActualQuantity must be numeric jsV9YGXXCL4G!';
            if (CStr2(vParams.UnitMeasure) == '')
                throw 'vParams.UnitMeasure must not be blank js2H4T3A36WH!';
            if (vParams.UnitPrice == null)
                throw 'vParams.UnitPrice must not be blank js4ZRFPW9MVJ!';
            if (!IsGuid(sessionStorage.FowstID))
                throw 'sessionStorage.FowstID must be GUID jsN9JUDZ4X94!';
            if (!IsGuid(Web.DeviceID()))
                throw 'Web.DeviceID() must be GUID js6R8P4DLE5C!';

            // find and update the void number 
            var pDrDeviceFowst;
            pSql = 'SELECT MAX(VoidNumber) AS VoidNumber\n' +
                ' FROM TransInvoiceItemsVoided\n' +
                ' WHERE InvoiceID = ' + ReadyString(vParams.InvoiceID) + '\n';
            var pVoidNumber = CInt2(Sql.ExecuteScalar(pSql));
            if (pVoidNumber <= 0) {
                pSql = 'SELECT ID\n' + 
                    ',VoidNumber\n' +
                    ' FROM LibraryDeviceFowst\n' +
                    ' WHERE DeviceID = ' + ReadyString(Web.DeviceID()) + '\n' +
                    ' AND FowstID = ' + ReadyString(sessionStorage.FowstID);
                pDrDeviceFowst = Sql.ExecuteRow(pSql);
                if (pDrDeviceFowst == null)
                    pVoidNumber = 1;
                else 
                    pVoidNumber = CInt2(pDrDeviceFowst.VoidNumber) + 1;
            }

            var pIU;
            pIU = new InsertUpdate('LibraryDeviceFowst');
            pIU.Add('VoidNumber', pVoidNumber);
            pIU.Add('FlushToServer', 1);
            pSql = pIU.GetUpdate('DeviceID = ' + ReadyString(Web.DeviceID()) + '\n' +
                ' AND FowstID = ' + ReadyString(sessionStorage.FowstID));
            Sql.ExecuteSql(pSql);

            pIU = new InsertUpdate('TransInvoiceItemsVoided');
            pID = NewID();
            pIU.Add('ID', ReadyString(pID));
            pIU.Add('InvoiceID', ReadyString(vParams.InvoiceID));
            pIU.Add('ProductID', ReadyString(vParams.ProductID));
            pIU.Add('Quantity', vParams.Quantity);
            pIU.Add('ActualQuantity', vParams.ActualQuantity);
            pIU.Add('UnitMeasure', ReadyString(vParams.UnitMeasure));
            pIU.Add('DateTimeRemoved', ReadyString($.format.date(proj.GetZReadDate(), 'yyyy-MM-dd hh:mm a')));
            pIU.Add('RemovedByPeopleID', ReadyString(pUserID));
            pIU.Add('UnitPrice', CDec2(vParams.UnitPrice));
            pIU.Add('VoidNumber', pVoidNumber);
            pIU.Add('RemovedByPeopleID', ReadyString(pUserID));
            pIU.Add('FowstID', ReadyString(sessionStorage.FowstID));
            pIU.Add('DeviceID', ReadyString(Web.DeviceID()));
            pIU.Add('CashierID', ReadyString(Web.PeopleUserID()));
            pSql = pIU.GetInsert();
            Sql.ExecuteSql(pSql);

            AddVoidColumnToCacheTable('CacheCSR', vParams.Quantity, vParams.UnitPrice);
            AddVoidColumnToCacheTable('CacheMSR', vParams.Quantity, vParams.UnitPrice);
            AddVoidColumnToCacheTable('CacheZRead', vParams.Quantity, vParams.UnitPrice);

        }; // END AddVoidItem

        function AddVoidColumnToCacheTable(table, itemqty, itemprice) {
            var newCacheColumns = ['CurVoid', 'CurVoidItemQty'];
            var cacheDB = Sql.Execute("PRAGMA cache.table_info(" + table +")");

            for (var i in newCacheColumns) {
                var isExist = false;
                for (var m in cacheDB.Rows) {
                    if (cacheDB.Rows[m].name == newCacheColumns[i]) {
                        isExist = true;
                        break;
                    }
                }

                if (!isExist) {
                    pSql = " ALTER TABLE cache." + table +
                        " ADD " + newCacheColumns[i] + " money"
                    Sql.Execute(pSql);
                }
            }

            var pIU = new InsertUpdate('cache.' + table);

            pIU.Add("CurVoid", "IFNULL(CAST(CurVoid AS INTEGER), 0) + " + (CDec2(itemqty) * CDec2(itemprice)));
            pIU.Add("CurVoidItemQty", "IFNULL(CAST(CurVoidItemQty AS INTEGER), 0) + " + CDec2(itemqty));

            var pSql = pIU.GetUpdate('SalesDate = ' + ReadyString($.format.date(proj.GetZReadDate(), 'yyyy-MM-dd')) +
                ' AND FOWSTID = ' + ReadyString(sessionStorage.FowstID) +
                ' AND DeviceID = ' + ReadyString(Web.DeviceID()) +
                (table == 'CacheCSR' ? ' AND UserID = ' + ReadyString(Web.PeopleUserID()) : ''))

            if (CDec2(Sql.ExecuteSql(pSql)) == 0) {
                pIU.Add("ID", ReadyString(NewID()));
                pIU.Add("CurVoid", CDec2(itemprice));
                pIU.Add("CurVoidItemQty", "1");
                pIU.Add('SalesDate', ReadyString($.format.date(proj.GetZReadDate(), 'yyyy-MM-dd')));                
                pIU.Add('FowstID', ReadyString(sessionStorage.FowstID));
                pIU.Add('DeviceID', ReadyString(Web.DeviceID()));

                if (table != 'CacheZRead') {
                    pIU.Add('OrgID', ReadyString(sessionStorage.OrgID));
                } else {
                    pSql = "Select * FROM CacheZRead " +
                        " WHERE date(SalesDate) < " + ReadyString($.format.date(proj.GetZReadDate(), 'yyyy-MM-dd')) +
                        " AND FOWSTID = " + ReadyString(sessionStorage.FowstID) +
                        " AND DeviceID = " + ReadyString(Web.DeviceID()) +
                        " Order by SalesDate desc limit 1"

                    var dtPrevZRead = Sql.Execute(pSql);

                    if (dtPrevZRead.Rows.length > 0) {
                        if (CDec2(dtPrevZRead.Rows[0].PresentationPrevGrossSales) == 0) {
                            pIU.Add('PresentationPrevGrossSales', CDec2(dtPrevZRead.Rows[0].PrevGrossSales) + CDec2(dtPrevZRead.Rows[0].CurGrossSales))
                        } else {
                            pIU.Add('PresentationPrevGrossSales', CDec2(dtPrevZRead.Rows[0].PresentationPrevGrossSales) + CDec2(dtPrevZRead.Rows[0].CurGrossSales))
                        }

                        pIU.Add('PrevGrossSales', CDec2(dtPrevZRead.Rows[0].PrevGrossSales) + CDec2(dtPrevZRead.Rows[0].CurGrossSales))
                        pIU.Add('PrevTotalDiscount', CDec2(dtPrevZRead.Rows[0].PrevTotalDiscount) + CDec2(dtPrevZRead.Rows[0].CurTotalDiscount))
                        pIU.Add('PrevSalesReturnRefund', CDec2(dtPrevZRead.Rows[0].PrevSalesReturnRefund) + CDec2(dtPrevZRead.Rows[0].CurSalesReturnRefund))
                        pIU.Add('PrevSalesReturnExchange', CDec2(dtPrevZRead.Rows[0].PrevSalesReturnExchange) + CDec2(dtPrevZRead.Rows[0].CurSalesReturnExchange))
                        pIU.Add('PrevTotalVAT', CDec2(dtPrevZRead.Rows[0].PrevTotalVAT) + CDec2(dtPrevZRead.Rows[0].CurTotalVAT))
                        pIU.Add('PrevServiceCharge', CDec2(dtPrevZRead.Rows[0].PrevServiceCharge) + CDec2(dtPrevZRead.Rows[0].CurServiceCharge))
                        pIU.Add('PrevChargeAmount', CDec2(dtPrevZRead.Rows[0].PrevChargeAmount) + CDec2(dtPrevZRead.Rows[0].ChargeAmount))
                        pIU.Add('PrevSCDiscount', CDec2(dtPrevZRead.Rows[0].PrevSCDiscount) + CDec2(dtPrevZRead.Rows[0].CurSCDiscount))
                        pIU.Add('PrevSoloParentDiscount', CDec2(dtPrevZRead.Rows[0].PrevSoloParentDiscount) + CDec2(dtPrevZRead.Rows[0].CurSoloParentDiscount))
                        pIU.Add('PrevPhNatTeamDiscount', CDec2(dtPrevZRead.Rows[0].PrevPhNatTeamDiscount) + CDec2(dtPrevZRead.Rows[0].CurPhNatTeamDiscount))
                        pIU.Add('PrevSCDiscountExempt', CDec2(dtPrevZRead.Rows[0].PrevSCDiscountExempt) + CDec2(dtPrevZRead.Rows[0].CurSCDiscountExempt))
                        pIU.Add('PrevVATExempt', CDec2(dtPrevZRead.Rows[0].PrevVATExempt) + CDec2(dtPrevZRead.Rows[0].CurVATExempt))
                        pIU.Add('PrevSRVAmount', CDec2(dtPrevZRead.Rows[0].PrevSRVAmount) + CDec2(dtPrevZRead.Rows[0].SRVAmount));

                    }
                    else {
                        //new data

                        pIU.Add('PresentationPrevGrossSales', 0)
                        pIU.Add('PrevGrossSales', 0);
                        pIU.Add('PrevTotalDiscount', 0);
                        pIU.Add('PrevSalesReturnRefund', 0);
                        pIU.Add('PrevSalesReturnExchange', 0);
                        pIU.Add('PrevTotalVAT', 0);
                        pIU.Add('PrevServiceCharge', 0);
                        pIU.Add('PrevChargeAmount', 0);
                        pIU.Add('PrevSCDiscount', 0);
                        pIU.Add('PrevSoloParentDiscount', 0);
                        pIU.Add('PrevPhNatTeamDiscount', 0);
                        pIU.Add('PrevSCDiscountExempt', 0);
                        pIU.Add('PrevVATExempt', 0);
                        pIU.Add('PrevSRVAmount', 0);
                    }
                }

                if (table == 'CacheCSR') {
                    pIU.Add('UserID', ReadyString(Web.PeopleUserID()));
                }

                pSql = pIU.GetInsert();
                Sql.Execute(pSql);
            }
        }

        this.UpdateTableNum = function(vInvoiceID, vTableNum) {
            var pSql;
            var pIU = new InsertUpdate('TransInvoice');;
            pIU.Add('CustomerTableNo', ReadyString(vTableNum));
            pSql = pIU.GetUpdate('ID = ' + ReadyString(vInvoiceID));
            Sql.ExecuteSql(pSql);
        };

        this.UnflushedTableView = function() {
            var pSql;
            var pOut;

            pSql = ' -- display fowst, dining area, table during data sync' + '\n' + 
                ' SELECT TransInvoice.ID' + '\n' + 
                ' ,TransInvoice.CustomerTableNo' + '\n' + 
                ' ,LibraryTableTableInfo.TableName' + '\n' + 
                ' ,LibraryTableDiningArea.DiningAreaName' + '\n' + 
                ' ,LibraryOrganizationFOWST.Name AS FowstName' + '\n' + 
                '  FROM TransInvoice' + '\n' + 
                ' JOIN LibraryTableTableInfo ON \'TBL_\' || LibraryTableTableInfo.InternalTableName = TransInvoice.CustomerTableNo' + '\n' + 
                ' JOIN LibraryTableDiningArea ON LibraryTableDiningArea.ID = LibraryTableTableInfo.DiningAreaID' + '\n' + 
                ' JOIN LibraryOrganizationFOWST ON LibraryOrganizationFOWST.ID = TransInvoice.FowstID' + '\n' + 
                '  WHERE EXISTS (' + '\n' + 
                '      SELECT *' + '\n' + 
                '      FROM TransInvoiceItems' + '\n' + 
                '      WHERE InvoiceID = TransInvoice.ID' + '\n' + 
                '      AND (Quantity > 0 OR OrderedQuantity > 0)' + '\n' + 
                '  )' + '\n' + 
                '  AND DateImported IS NULL' + '\n' + 
                '  AND' + '\n' + 
                '  (' + '\n' + 
                '      TransInvoice.InvoiceNumber > 0' + '\n' + 
                '      OR' + '\n' + 
                '      (' + '\n' + 
                '          LENGTH(IFNULL(CustomerTableNo, \'\')) > 0' + '\n' + 
                '          AND IFNULL(TransInvoice.InvoiceNumber, 0) = 0' + '\n' + 
                '          AND EXISTS (' + '\n' + 
                '            SELECT *' + '\n' + 
                '            FROM LibraryTableTableInfo' + '\n' + 
                '            WHERE CustomerTableNo = \'TBL_\' || LibraryTableTableInfo .InternalTableName' + '\n' + 
                '          )' + '\n' + 
                '      )' + '\n' + 
                '  )'

            // old: 2019_08_29__11_32_44_0272
            //pSql = ' SELECT COUNT(*) AS COUNT' + '\n' + 
            //    ' FROM TransInvoice' + '\n' + 
            //    ' WHERE EXISTS (' + '\n' + 
            //    '     SELECT *' + '\n' + 
            //    '     FROM TransInvoiceItems' + '\n' + 
            //    '     WHERE InvoiceID = TransInvoice.ID' + '\n' + 
            //    '     AND (Quantity > 0 OR OrderedQuantity > 0)' + '\n' + 
            //    ' )' + '\n' + 
            //    ' AND DateImported IS NULL' + '\n' + 
            //    ' AND' + '\n' + 
            //    ' (' + '\n' + 
            //    '     TransInvoice.InvoiceNumber > 0' + '\n' + 
            //    '     OR' + '\n' + 
            //    '     (' + '\n' + 
            //    '         LENGTH(IFNULL(CustomerTableNo, \'\')) > 0' + '\n' + 
            //    '         AND IFNULL(TransInvoice.InvoiceNumber, 0) = 0' + '\n' + 
            //    '         AND EXISTS (' + '\n' + 
            //    '           SELECT * ' + '\n' + 
            //    '           FROM LibraryTableTableInfo ' + '\n' + 
            //    '           WHERE CustomerTableNo = \'TBL_\' || LibraryTableTableInfo .InternalTableName' + '\n' + 
            //    '         )\n' +
            //    '     )' + '\n' + 
            //    ' )';
            pOut = Sql.Execute(pSql);

            return pOut;
        }; // END UnflushedTableView

        this.IncrementPrintCount = function(vID) {
            var pSql;
            var pIU;

            pIU = new InsertUpdate('TransInvoice');
            pIU.Add('PrintCount', 'IFNULL(CAST(PrintCount AS INTEGER), 0) + 1');
            pIU.Add('IsPrinted', 1);
            pSql = pIU.GetUpdate('ID = ' + ReadyString(vID));
            Sql.ExecuteSql(pSql);

        }; // END IncrementPrintCount

        this.FixNoCustomers = function() {
            var pSql;

            pSql = Business.SystemDefaults.CustomerOrganizationID();
            pSql = ReadyString(pSql);
            pSql = ' UPDATE Transinvoice' + '\n' + 
                ' SET CustomerID = ' + pSql + '\n' + 
                ' WHERE ID IN (' + '\n' + 
                ' SELECT Transinvoice.ID' + '\n' + 
                ' FROM private.Transinvoice' + '\n' + 
                ' LEFT JOIN LibraryOrganization ON LibraryOrganization.ID = Transinvoice.CustomerID' + '\n' + 
                ' WHERE LibraryOrganization.ID IS NULL' + '\n' + 
                ' OR Transinvoice.CustomerID IS NULL' + '\n' + 
                ' )';
            pSql = Sql.ExecuteSql(pSql);
            if (CDec2(pSql) > 0)
                Android.AddLog(pSql + ' invoice[s] have invalid (or null) customers. Their customers are now updated to the default customer. ref: jsKQ8367KTBF');
        };

        this.FixNoItemCount = function() {
            var pSql = ' UPDATE TransInvoice' + '\n' + 
                ' SET ItemCount = (SELECT COUNT(*) FROM TransInvoiceItems WHERE InvoiceID = TransInvoice.ID)' + '\n' + 
                ' WHERE ItemCount IS NULL';
            pSql = Sql.ExecuteSql(pSql);
            if (CDec2(pSql) > 0)
                Android.AddLog(pSql + ' invoice[s] have updated their ItemCount. ref: js8W6Q6Y4HGY');
        };

        this.UpdateItemCount = function(vInvoiceID) {
            var pSql = ' UPDATE TransInvoice' + '\n' + 
                ' SET ItemCount = (SELECT COUNT(*) FROM TransInvoiceItems WHERE InvoiceID = TransInvoice.ID)' + '\n' + 
                ' WHERE ID = ' + ReadyString(vInvoiceID);
            pSql = Sql.ExecuteSql(pSql);
        };

        this.UpdateQty = function(vItemID, vQty) {
            var pSql = ' UPDATE TransInvoiceItems' + '\n' + 
                ' SET OrderedQty = ' + CDec2(vQty) + '\n' + 
                ' WHERE ID = ' + ReadyString(vItemID);
            pSql = Sql.ExecuteSql(pSql);
        };

        this.HasPrintableOrderSlip = function(vInvoiceID) {
            var pSql = 'SELECT COUNT(*)\n' +
                ' FROM TransInvoiceItems \n' +
                ' WHERE InvoiceID = ' + ReadyString(vInvoiceID) + '\n' +
                ' AND IFNULL(TransInvoiceItems.Quantity, 0) <> IFNULL(TransInvoiceItems.QuantityPrinted, 0)';
            var pCount = Sql.ExecuteScalar(pSql);
            return CInt2(pCount) > 0;
        };

        this.TagOrderSlipItemsAsPrinted = function (vParams) {
            var pSql = '';
            var pIU;
            var pIDs = '';

            vParams = $.extend({
                Rows: null
            }, vParams);

            if (vParams.Rows.length == 0)
                return;

            for (var p = 0; p <= vParams.Rows.length - 1; p++) {
                if (pIDs.length > 0)
                    pIDs += ', ';
                pIDs += ReadyString(vParams.Rows[p].ItemID);
            }
            
            pIU = new InsertUpdate('TransInvoiceItems');
            pIU.Add('IsOrderSlipPrinted', 1);
            pIU.Add('QuantityPrinted', 'Quantity');
            pSql = pIU.GetUpdate('ID IN (' + pIDs + ')');
            pSql = Sql.ExecuteSql(pSql); // output is used for debugging only
        }

        if (window.DebugMode) {
            this.AddDummyPrinters = function() {
                var pDeviceID = Web.DeviceID();
                var pFowstID = sessionStorage.FowstID;

                if (MustCreateDummy()) {
                    CreateDummy('192.168.1.219', 'Sandwiches');
                    CreateDummy('192.168.1.218', 'Appetizers');
                }

                function MustCreateDummy() {
                    var pDT;
                    var pSql;
                    pSql = ' SELECT LibraryDeviceFowstPrinter.ID  ' + '\n' + 
                        ' , LibraryDevicePrinterFowst.PrinterIPAddress  ' + '\n' + 
                        ' FROM private.LibraryDeviceFowstPrinter ' + '\n' + 
                        ' JOIN private.LibraryDevicePrinterFowst ON LibraryDevicePrinterFowst.ID = LibraryDeviceFowstPrinter.PrinterFowstID' + '\n' + 
                        ' WHERE LibraryDevicePrinterFowst.FOWSTID = ' + ReadyString(pFowstID) + '\n' + 
                        ' AND LibraryDeviceFowstPrinter.DeviceID = ' + ReadyString(pDeviceID);
                    pDT = Sql.Execute(pSql);
                    return pDT.Rows.length < 3;

                } // END function MustCreateDummy() {

                function CreateDummy(vPrinterIPAddress, vCategoryName) {
                    var pIU;
                    var pSql;
                    var pPrinterID = NewID(); // LibraryDevicePrinter
                    var pPrinterFowst = NewID(); // LibraryDevicePrinterFowst

                    pIU = new InsertUpdate('private.LibraryDevicePrinter');
                    pIU.Add('ID', ReadyString(pPrinterID));
                    pIU.Add('PrinterType', "'dummy PrinterType'");
                    pIU.Add('IsActive', 1);
                    pIU.Add('UserID', "'c530bdf3-ea03-4dd3-95cd-a384374aa570'");
                    pIU.Add('LastUpdated', proj.SqliteNow);
                    pIU.Add('PrinterRole', "'Category Printer'");
                    pSql = pIU.GetInsert();
                    Sql.ExecuteScalar(pSql);

                    pIU = new InsertUpdate('private.LibraryDevicePrinterFowst');
                    pIU.Add('ID', ReadyString(pPrinterFowst));
                    pIU.Add('FowstID', ReadyString(pFowstID));
                    pIU.Add('PrinterID', ReadyString(pPrinterID));
                    pIU.Add('PrinterIPAddress', ReadyString(vPrinterIPAddress));
                    pIU.Add('PrinterName', "'dummy PrinterName'");
                    pIU.Add('ConnectionType', "'Ethernet'");
                    pSql = pIU.GetInsert();
                    Sql.ExecuteScalar(pSql);

                    pIU = new InsertUpdate('private.LibraryDeviceFowstPrinter');
                    pIU.Add('ID', ReadyString(NewID()));
                    pIU.Add('DeviceID', ReadyString(pDeviceID));
                    pIU.Add('IsActive', 1);
                    pIU.Add('UserID', "'c530bdf3-ea03-4dd3-95cd-a384374aa570'");
                    pIU.Add('LastUpdated', proj.SqliteNow);
                    pIU.Add('PrinterFowstID', ReadyString(pPrinterFowst));
                    pSql = pIU.GetInsert();
                    Sql.ExecuteScalar(pSql);

                    pIU = new InsertUpdate('private.LibraryDevicePrinterCategory');
                    pIU.Add('ID', ReadyString(NewID()));
                    pIU.Add('PrinterID', ReadyString(pPrinterID));
                    pIU.Add('CategoryName', ReadyString(vCategoryName));
                    pSql = pIU.GetInsert();
                    Sql.ExecuteScalar(pSql);
                } // END function CreateDummy() {
            }; // END pOut.AddDummyPrinters = function() {
            
        } // END if (window.DebugMode) {

        this.GetZeroUnitQty = function(vInvoiceID) {
            var pSql = ' SELECT LibraryPIMProduct.ProductName' + '\n' + 
                ' FROM private.TransInvoiceItems' + '\n' + 
                ' LEFT JOIN LibraryPIMProduct ON LibraryPIMProduct.ID = TransInvoiceItems.ProductID' + '\n' + 
                ' WHERE IFNULL(TransInvoiceItems.ActualQuantity, 0) = 0' + '\n' + 
                ' AND TransInvoiceItems.InvoiceID = ' + ReadyString(vInvoiceID);
            var pDT = Sql.Execute(pSql);
            return pDT;
        };

        this.GetOtherPayments = function(vInvoiceID) {
            var pSql = 'SELECT SUM(TransCustomerPaymentOther.Amount) AS Amount\n' +
                ' ,TransCustomerPaymentOther.PaymentType\n' +
                ' FROM TransCustomerPaymentOther\n' +
                ' JOIN TransCustomerPaymentInvoices ON TransCustomerPaymentInvoices.CustomerPaymentID = TransCustomerPaymentOther.CustomerPaymentID\n' +
                ' WHERE TransCustomerPaymentInvoices.InvoiceID = ' + ReadyString(vInvoiceID) + '\n' +
                ' GROUP BY TransCustomerPaymentOther.PaymentType\n' +
                ' ORDER BY TransCustomerPaymentOther.PaymentType\n';
            var pDT = Sql.Execute(pSql);
            return pDT;
        }
        return this;
    };
});