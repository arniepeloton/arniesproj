define([], function () {
    var mCounterFilename = 'Counters.txt';
    var mDatedCounterFilename = 'DatedCounters.txt';

    window.Business.TransSalesReturn = new function (vParent) {
        var mThis = this;
        this.GetContent = function (vParams) {

            vParams = $.extend({
                InvoiceID: null,
                ProductID: null,
                MemoID: null
            }, vParams);

            var pDT;
            var pSql;

            pSql = ' -- sqlite sales return invoice contents' + '\n' + 
                ' SELECT LibraryPIMProduct.ID AS ProductID' + '\n' + 
                '  ,LibraryPIMProduct.ProductNo' + '\n' + 
                ' -- ,LibraryPIMProduct.ProductName + IFNULL(\'(Rebate Item: \' + CONVERT(varchar(500), CONVERT(decimal(8,0),tblInvoice.Quantity * TransInvoiceItemsRebates.Quantity * TransInvoiceItemsRebates.UnitQuantity)) + \' \' + TransInvoiceItemsRebates.UnitMeasure + \' \' + mb2.ProductName + \')\',\'\') AS ProductName' + '\n' + 
                '  ,LibraryPIMProduct.ProductName AS ProductName' + '\n' + 
                '  ,SUM(tblInvoice.Quantity) AS Quantity' + '\n' + 
                '  ,tblInvoice.ActualQuantity' + '\n' + 
                '  ,tblInvoice.UnitMeasure' + '\n' + 
                '  ,tblInvoice.UnitPrice - tblInvoice.UnitDiscountAmount AS UnitPrice' + '\n' + 
                '  ,tblInvoice.UnitDiscountAmount' + '\n' + 
                '  ,SUM(tblInvoice.TaxAmount) AS TaxAmount' + '\n' + 
                '  ,SUM(tblInvoice.VATSales) AS VATSales' + '\n' +
                '  ,SUM(tblInvoice.VATExempt) AS VATExempt' + '\n' +
                '  ,SUM(tblInvoice.HasSCDiscount) AS HasSCDiscount' + '\n' +
                '  ,tblInvoice.InvoiceID' + '\n' + 
                '  ,SUM(tblInvoice.Quantity) AS QtyInvoiced' + '\n' + 
                '  ,(SELECT IFNULL(SUM(QuantityReturned), 0) + IFNULL(SUM(QuantityBad), 0)' + '\n' + 
                '  	FROM TransMemoSalesReturn' + '\n' + 
                '  	JOIN TransMemo ON TransMemo.ID = TransMemoSalesReturn.MemoID' + '\n' + 
                '  	LEFT JOIN TransInvoiceAdjustments ON TransInvoiceAdjustments.MemoID = TransMemoSalesReturn.MemoID' + '\n' + 
                '  	WHERE TransMemoSalesReturn.InvoiceID = tblInvoice.InvoiceID' + '\n' + 
                '  	AND TransMemoSalesReturn.ProductID = tblInvoice.ProductID' + '\n' + 
                '  	AND TransMemoSalesReturn.ActualQuantity = tblInvoice.ActualQuantity' + '\n' + 
                '  	AND IFNULL(TransMemo.IsCancelled, 0) = 0' + '\n' + 
                '    AND (TransInvoiceAdjustments.IsActive = 1' + '\n' + 
                '       OR TransInvoiceAdjustments.ID IS NULL' + '\n' + 
                '    )' + '\n';
                if (CStr2(vParams.MemoID).length > 0)
                    pSql += ' AND TransMemoSalesReturn.MemoID <> ' + ReadyString(vParams.MemoID) + '\n';
                pSql += '  ) AS QtyReturnedOthers' + '\n';

                if (CStr2(vParams.MemoID).length > 0)
                    pSql += ' ,(SELECT IFNULL(SUM(QuantityReturned), 0)\n' +
                        ' 	FROM TransMemoSalesReturn\n' +
                        ' 	JOIN TransMemo ON TransMemo.ID = TransMemoSalesReturn.MemoID\n' +
                        ' 	WHERE TransMemoSalesReturn.InvoiceID = tblInvoice.InvoiceID\n' +
                        ' 	AND TransMemoSalesReturn.ProductID = tblInvoice.ProductID\n' +
                        ' 	AND TransMemoSalesReturn.ActualQuantity = tblInvoice.ActualQuantity\n' +
                        ' 	AND IFNULL(TransMemo.IsCancelled, 0) = 0\n' +
                        ' 	AND TransMemoSalesReturn.MemoID = ' + ReadyString(vParams.MemoID) + '\n' +
                        ' )';
                else
                    pSql += ",0";

                pSql += ' AS QtyReturnedOwn' + '\n' + 
                ' ,0 AS QtyReturnedOwnBad' + '\n';
                if (CStr2(vParams.MemoID).length > 0)
                    pSql += ' ,(SELECT TransMemoSalesReturn.Reason\n' +
                        '  	FROM TransMemoSalesReturn\n' +
                        '  	JOIN TransMemo ON TransMemo.ID = TransMemoSalesReturn.MemoID\n' +
                        '  	WHERE TransMemoSalesReturn.InvoiceID = tblInvoice.InvoiceID\n' +
                        '  	AND TransMemoSalesReturn.ProductID = tblInvoice.ProductID\n' +
                        '  	AND TransMemoSalesReturn.ActualQuantity = tblInvoice.ActualQuantity\n' +
                        '  	AND IFNULL(TransMemo.IsCancelled, 0) = 0\n' +
                        '  	AND TransMemoSalesReturn.MemoID = ' + ReadyString(vParams.MemoID) + '\n' +
                        '   ORDER BY TransMemoSalesReturn.LastUpdated DESC\n' +
                        '   LIMIT 1\n' +
                        '  ) AS ReturnReason\n';
                else
                    pSql += ' ,\'\' AS ReturnReason' + '\n';
                pSql += '  FROM LibraryPIMProduct' + '\n' + 
                '  JOIN (' + '\n' + 
                '  SELECT ProductID' + '\n' + 
                '  ,ActualQuantity' + '\n' + 
                '  ,UnitMeasure' + '\n' + 
                '  ,UnitPrice' + '\n' + 
                '  ,UnitDiscountAmount' + '\n' + 
                '  ,TaxAmount' + '\n' + 
                '  ,InvoiceID' + '\n' + 
                '  ,SUM(Quantity) AS Quantity' + '\n' + 
                '  ,ModifierGroupID' + '\n' + 
                '  ,VATSales' + '\n' +
                '  ,VATExempt' + '\n' +
                '  ,HasSCDiscount' + '\n' +
                '  FROM (' + '\n' + 
                '  SELECT TransMemoSalesReturn.ProductID' + '\n' + 
                '  ,TransMemoSalesReturn.ActualQuantity' + '\n' + 
                '  ,TransMemoSalesReturn.UnitMeasure' + '\n' + 
                '  ,TransMemoSalesReturn.UnitPrice' + '\n' + 
                '  ,TransMemoSalesReturn.UnitDiscountAmount' + '\n' + 
                '  ,0 AS TaxAmount' + '\n' + 
                '  ,' + ReadyString(vParams.InvoiceID) + ' AS InvoiceID' + '\n' + 
                '  ,0 AS Quantity' + '\n' + 
                '  ,NULL AS ModifierGroupID' + '\n' + 
                " ,TransMemoSalesReturn.VATSales" + '\n' + 
                " ,TransMemoSalesReturn.VATExempt" + '\n' + 
                " ,NULL AS HasSCDiscount" + '\n' + 
                '  FROM TransMemoSalesReturn' + '\n' + 
                '  WHERE 1=1' + '\n' + 
                '  AND TransMemoSalesReturn.InvoiceID = ' + ReadyString(vParams.InvoiceID) + '\n' + 
                '  UNION' + '\n' + 
                '  SELECT  TransInvoiceItems.ProductID' + '\n' + 
                '  ,TransInvoiceItems.ActualQuantity' + '\n' + 
                '  ,TransInvoiceItems.UnitMeasure' + '\n' + 
                '  ,TransInvoiceItems.UnitPrice' + '\n' + 
                '  ,TransInvoiceItems.UnitPrice - \n' +
                'case when TransInvoiceItems.quantity=(0) then (0) when (((ifnull(TransInvoiceItems.vatablesale,(0))+ifnull(TransInvoiceItems.vatexemptsale,(0)))+ifnull(TransInvoiceItems.zeroratedsale,(0)))+ifnull(TransInvoiceItems.nonvatsale,(0)))=(0) then (TransInvoiceItems.subtotal-ifnull(CAST(TransInvoiceItems.discountamount AS REAL), 0))/TransInvoiceItems.quantity else ((((ifnull(TransInvoiceItems.vatablesale,(0))+ifnull(TransInvoiceItems.vatexemptsale,(0)))+ifnull(TransInvoiceItems.zeroratedsale,(0)))+ifnull(TransInvoiceItems.nonvatsale,(0)))-ifnull(CAST(TransInvoiceItems.discountamount AS REAL),0))/TransInvoiceItems.quantity end \n' + 
                ' AS UnitDiscountAmount' + '\n' + 
                '  ,TransInvoiceItems.TaxAmount' + '\n' + 
                '  ,' + ReadyString(vParams.InvoiceID) + ' AS InvoiceID' + '\n' + 
                '  ,TransInvoiceItems.Quantity' + '\n' + 
                '  ,TransInvoiceItems.ModifierGroupID' + '\n' + 
                '  ,TransInvoiceItems.VATableSale AS VATSales ' + '\n' +
                " ,CASE WHEN TransInvoice.SCDiscount > 0 OR TransInvoice.SoloParentDiscount > 0 THEN ROUND(TaxSales, 2) ELSE TransInvoiceItems.VATExemptSale END AS VATExempt \n" +
                " ,CASE WHEN IFNULL(TransInvoice.SCDiscount, 0) > 0 OR IFNULL(TransInvoice.SoloParentDiscount, 0) > 0 THEN 1 ELSE 0 END AS HasSCDiscount \n" +
                '  FROM TransInvoiceItems' + '\n' + 
                '  JOIN TransInvoice ON TransInvoiceItems.InvoiceID = TransInvoice.ID' + '\n' + 
                '  AND IFNULL(TransInvoice.IsCancelled, 0) = 0' + '\n' + 
                '  AND TransInvoiceItems.InvoiceID = ' + ReadyString(vParams.InvoiceID) + '\n' + 
                '  ) t' + '\n' + 
                '  GROUP BY ProductID' + '\n' + 
                '  ,ActualQuantity' + '\n' + 
                '  ,UnitMeasure' + '\n' + 
                '  ,UnitPrice' + '\n' + 
                '  ,TaxAmount' + '\n' + 
                '  ,InvoiceID' + '\n' + 
                '  ,ModifierGroupID' + '\n' + 
                '  ,VATSales' + '\n' +
                '  ,VATExempt' + '\n' +
                '  ,HasSCDiscount' + '\n' +
                '  ) tblInvoice ON tblInvoice.ProductID = LibraryPIMProduct.ID' + '\n' + 
                ' LEFT JOIN TransInvoiceItemsRebates on TransInvoiceItemsRebates.ModifierGroupID = tblInvoice.ModifierGroupID' + '\n' + 
                ' LEFT JOIN LibraryPIMProduct mb2 ON TransInvoiceItemsRebates.ProductID = mb2.ID\n' +
                ' WHERE 1 = 1' + '\n';
                if (CStr2(vParams.ProductID).length > 0)
                    pSql += ' AND LibraryPIMProduct.ID = ' + ReadyString(vParams.ProductID) + '\n';
                pSql += '  GROUP BY LibraryPIMProduct.ID' + '\n' + 
                '   ,LibraryPIMProduct.ProductNo' + '\n' + 
                ' --  ,LibraryPIMProduct.ProductName + IFNULL(\'(Rebate Item: \' + CONVERT(varchar(500), CONVERT(decimal(8,0),tblInvoice.Quantity * TransInvoiceItemsRebates.Quantity * TransInvoiceItemsRebates.UnitQuantity)) + \' \' + TransInvoiceItemsRebates.UnitMeasure + \' \' + mb2.ProductName + \')\',\'\')' + '\n' + 
                ' ,LibraryPIMProduct.ProductName' + '\n' + 
                '   ,tblInvoice.ActualQuantity' + '\n' + 
                '   ,tblInvoice.UnitMeasure' + '\n' + 
                '   ,tblInvoice.UnitPrice' + '\n' + 
                '   ,tblInvoice.InvoiceID' + '\n' + 
                '   ,tblInvoice.ProductID' + '\n' + 
                '  ORDER BY tblInvoice.ActualQuantity,tblInvoice.UnitMeasure'
            pDT = Sql.Execute(pSql);
            console.log(pDT.Rows)
            // get unique product ID's
            var pUniqueProducts = [];
            for (var pIndex = 0; pIndex <= pDT.Rows.length - 1; pIndex++) {
                var pDR = pDT.Rows[pIndex];
                pDR.ProductID = (pDR.ProductID);
                pDR.GrossUnitPrice = (pDR.UnitPrice);
                var pFound = pUniqueProducts.filter(function(vItem) {
                    if (vItem.ProductID == pDR.ProductID)
                        return vItem;
                });
                if (pFound.length == 0)
                    pUniqueProducts.push(pDR);
            }

            var pOut = {
                Units: pDT,
                Stocks: {
                    Rows: pUniqueProducts
                }
            };

            if (pDT.Rows.length > 0) {
                var pInvoiceID = pDT.Rows[0].InvoiceID;
                if (IsGuid(pInvoiceID)) {
                    var pDtInvoice = Business.TransInvoice.GetByID(pInvoiceID);
                    if (pDtInvoice != null) {
                        var pDrInvoice = pDtInvoice.Rows[0];
                        pOut.CustomerID = pDrInvoice.CustomerID;
                        var pTotalAmount = CDec2(pDrInvoice.TotalAmount);
                        var pScDiscount = CDec2(pDrInvoice.SCDiscount);
                        var pSPDiscount = CDec2(pDrInvoice.SoloParentDiscount);
                        var pNTDiscount = CDec2(pDrInvoice.PhNatTeamDiscount);
                        var pDiscountFactor = 0;
                        if ((pTotalAmount > 0)) {
                            if (pScDiscount > 0) {
                                pDiscountFactor = (pScDiscount + CDec2(pDrInvoice.ForegoneVAT)) / pTotalAmount;
                            }    

                            if (pSPDiscount > 0) {
                                pDiscountFactor = (pSPDiscount + CDec2(pDrInvoice.ForegoneVAT)) / pTotalAmount;
                            }    

                            if (pNTDiscount > 0) {
                                pDiscountFactor = (pNTDiscount + CDec2(pDrInvoice.ForegoneVAT)) / pTotalAmount;
                            }                            
                        }
                        if (pDiscountFactor > 0) {
                            for(var pIndex = 0; pIndex <= pDT.Rows.length - 1; pIndex++) {
                                var pDR = pDT.Rows[pIndex];
                                pDR.UnitPrice = CDec2(pDR.UnitPrice) * (1 - pDiscountFactor);
                            }
                        } // END if (pDiscountFactor > 0) 
                    } // END if (pDtInvoice != null) {
                }
            }

            return pOut;

        }; // 

        this.Update = function(vParams) {
            vParams = $.extend({
                MemoID: null,
                StockID: null,
                CustomerID: null,
                Stocks: []
            }, vParams);

            var pDrMemo = this.GetMemo(vParams.MemoID) || {};
            if (CStr2(pDrMemo.ApprovedBy).length > 0)
                return {
                    Error: 'This transaction was already approved. Modifications are not allowed anymore.'
                };
            if (CStr2(pDrMemo.DeniedBy).length > 0)
                return {
                    Error: 'This transaction was disapproved. Modifications are not allowed anymore.'
                };

            var pSql;
            Sql.BeginTransaction();
            if (CStr2(vParams.MemoID).length > 0) {
                pSql = 'DELETE FROM TransMemoSalesReturn\n' +
                    ' WHERE MemoID = ' + ReadyString(vParams.MemoID) + '\n' +
                    ' AND InvoiceID = ' + ReadyString(vParams.InvoiceID) + '\n' +
                    ' AND ProductID = ' + ReadyString(vParams.StockID);
                Sql.ExecuteSql(pSql);
            }

            var pMemoID;
            var pIU = new InsertUpdate('TransMemo');
            pIU.Add('UserID', ReadyString(Web.PeopleUserID()));
            pIU.Add('MemoDate', proj.SqliteNow);
            pIU.Add('LastUpdated', proj.SqliteNow);
            pIU.Add('Reason', "'Sales Returns'");
            if (CStr2(sessionStorage.FowstID).trim() == '') {
                Android.AddLog('sessionStorage.FowstID is blank. proj.SrvFowstID will be used instead');
                sessionStorage.FowstID = proj.SrvFowstID;
            }
            pIU.Add('FowstID', ReadyString(sessionStorage.FowstID));
            if (CStr2(vParams.MemoID).length > 0) {
                pMemoID = vParams.MemoID;
                pSql = pIU.GetUpdate("ID = " & ReadyString(vParams.MemoID));
            } else {
                pMemoID = NewID();
                pIU.Add('ID', ReadyString(pMemoID));
                pSql = pIU.GetInsert();
            }
            Sql.ExecuteSql(pSql);

            for (var pStockIndex = 0; pStockIndex <= vParams.Stocks.length - 1; pStockIndex++) {
                var pStock = vParams.Stocks[pStockIndex];
                pIU = new InsertUpdate('TransMemoSalesReturn');
                pIU.Add('ID', ReadyString(NewID()));
                pIU.Add('MemoID', ReadyString(pMemoID));
                pIU.Add('CustomerID', ReadyString(vParams.CustomerID));
                pIU.Add('LastUpdated', proj.SqliteNow);

                pIU.Add('InvoiceID', ReadyString(vParams.InvoiceID));
                pIU.Add('ProductID', ReadyString(vParams.StockID));
                pIU.Add('QuantityReturned', CDec2(pStock.Qty));
                pIU.Add('UnitMeasure', ReadyString(pStock.UnitMeasure));
                pIU.Add('ActualQuantity', CDec2(pStock.UnitQty));
                pIU.Add('UnitPrice', CDec2(pStock.UnitPrice));
                pIU.Add('TotalAmount', CDec2(pStock.UnitPrice) * CDec2(pStock.Qty));
                pIU.Add('UnitDiscountAmount', CDec2(pStock.UnitDiscountAmount));
                pIU.Add('TaxAmount', CDec2(pStock.TaxAmount));
                pIU.Add('VATSales', CDec2(pStock.VATSales));
                pIU.Add('VATExempt', CDec2(pStock.VATExempt));
                pIU.Add('Reason', ReadyString(pStock.ReturnReason));
                pSql = pIU.GetInsert();
                Sql.ExecuteSql(pSql);
            } // END for (var pStockIndex = 0; pStockIndex <= vParams.Stocks.length - 1; pStockIndex++) {

            Sql.CommitTransaction();
            return pMemoID;
        }; // END this.Update = function(vParams) {

        this.GetMemo = function(vMemoID) {
            var pSql = 'SELECT TransMemo.FowstID\n' +
                ',TransMemo.MemoNumber\n' +
                ',TransMemo.UserID\n' +
                ',TransMemo.ApprovedBy\n' +
                ',TransMemo.DateRequested\n' +
                ',TransMemo.DeniedBy\n' +
                ',TransMemo.CheckOutMethod\n' +
                ',TransMemo.DeviceID\n' +
                ',LibraryOrganizationFOWST.Name AS FowstName\n' +
                ' FROM TransMemo\n' +
                ' JOIN LibraryOrganizationFOWST ON LibraryOrganizationFOWST.ID = TransMemo.FowstID\n' +
                ' WHERE TransMemo.ID = ' + ReadyString(vMemoID);
            var pDR = Sql.ExecuteRow(pSql);

            return pDR;
        }; // END this.GetMemo = function(vMemoID) {

        this.GetCartMemoByInvoiceID = function(vInvoiceID) {
            
            var pSql = 'SELECT TransMemoSalesReturn.MemoID\n' +
            ' FROM TransMemoSalesReturn\n' +
            ' JOIN TransMemo ON TransMemo.ID = TransMemoSalesReturn.MemoID\n' +
            ' WHERE TransMemo.ApprovedBy IS NULL\n' +
            ' AND IFNULL(TransMemo.IsCancelled, 0) = 0\n' +
            ' AND TransMemoSalesReturn.InvoiceID = ' + ReadyString(vInvoiceID) + '\n' +
            ' ORDER BY  TransMemo.LastUpdated DESC\n' +
            ' LIMIT 1';
            var pDR = Sql.ExecuteRow(pSql);
            if (pDR != null)
                return pDR.MemoID;

        }; // END this.GetCartMemoByInvoiceID = function(vInvoiceID) {

        this.GetStocks = function(vMemoID) {
            var pSql = ' SELECT TransMemoSalesReturn.ID' + '\n' + 
                '  ,TransMemo.MemoNumber' + '\n' + 
                '  ,TransMemoSalesReturn.MemoID' + '\n' + 
                '  ,TransMemoSalesReturn.InvoiceID' + '\n' + 
                ' -- ,RIGHT(\'0000000\' + CONVERT(varchar, TransInvoice.InvoiceNumber), 7) AS InvoiceNum' + '\n' + 
                '  ,TransInvoice.InvoiceDate' + '\n' + 
                '  ,TransMemoSalesReturn.ProductID' + '\n' + 
                '  ,LibraryPIMProduct.ProductNo' + '\n' + 
                '  ,LibraryPIMProduct.ProductName' + '\n' + 
                '  ,TransMemoSalesReturn.QuantityReturned' + '\n' + 
                '  ,TransMemoSalesReturn.QuantityBad' + '\n' + 
                '  ,TransMemoSalesReturn.UnitMeasure' + '\n' + 
                '  ,TransMemoSalesReturn.UnitPrice' + '\n' + 
                '  ,TransMemoSalesReturn.TotalAmount' + '\n' + 
                '  ,TransMemoSalesReturn.Reason' + '\n' + 
                '  ,TransMemoSalesReturn.CustomerID' + '\n' + 
                '  ,LibraryOrganization.CompanyName AS CustomerName' + '\n' + 
                '  ,TransMemoSalesReturn.ActualQuantity' + '\n' + 
                '  /*,TransMemoSalesReturn.OrigQuantity * TransMemoSalesReturn.OrigActualQuantity - TransMemoSalesReturn.QuantityReturned * TransMemoSalesReturn.ActualQuantity AS QtyRemaining*/' + '\n' + 
                '  ' + '\n' + 
                '  FROM TransMemoSalesReturn' + '\n' + 
                '  JOIN TransMemo ON TransMemo.ID = TransMemoSalesReturn.MemoID' + '\n' + 
                '  LEFT JOIN TransInvoice ON TransMemoSalesReturn.InvoiceID = TransInvoice.ID' + '\n' + 
                '  JOIN LibraryPIMProduct ON TransMemoSalesReturn.ProductID = LibraryPIMProduct.ID' + '\n' + 
                '  JOIN LibraryOrganization ON TransMemoSalesReturn.CustomerID = LibraryOrganization.ID ' + '\n' + 
                '  WHERE TransMemoSalesReturn.MemoID = ' + ReadyString(vMemoID) + '\n' + 
                '  ORDER BY LibraryPIMProduct.ProductName';
            var pDT = Sql.Execute(pSql);

            if (pDT.Rows.length == 0)
                return {
                    Cart: 'no item'
                };

            var pTotalAmount = 0;
            var pSkuCount = 0;
            for (var pIndex = 0; pIndex <= pDT.Rows.length - 1; pIndex++) {
                var pDR = pDT.Rows[pIndex];
                if (CDec2(pDR.QuantityReturned) + CDec2(pDR.QuantityBad) > 0) {
                    pTotalAmount += CDec2(pDR.TotalAmount);
                   pSkuCount++;
                }
            }

            var pOut = {
                Units: pDT,
                Stocks: pDT,
                MemoID: vMemoID,
                TotalAmount: pTotalAmount,
                SkuCount: pSkuCount
            };

            return pOut;

        }; // END this.GetStocks = function(vMemoID) {

        this.GetMemoAmount = function(vMemoID) {
            var pSql = ' SELECT SUM(TransMemoSalesReturn.TotalAmount) AS TotalAmount' + '\n' + 
                ' ,TransMemoSalesReturn.InvoiceID' + '\n' + 
                ' ,TransMemoSalesReturn.ID AS MemoSalesReturnID' + '\n' + 
                ' FROM TransMemoSalesReturn' + '\n' + 
                ' JOIN TransMemo ON TransMemo.ID = TransMemoSalesReturn.MemoID' + '\n' + 
                ' WHERE IFNULL(TransMemo.IsCancelled, 0) = 0' + '\n' + 
                ' AND TransMemoSalesReturn.MemoID = ' + ReadyString(vMemoID) + '\n' + 
                ' GROUP BY TransMemoSalesReturn.InvoiceID';
            var pDR = Sql.ExecuteRow(pSql);

            return pDR;

        }; // END this.GetMemoAmount = function() {

        this.Approve = function (vMemoID, vIsRefund, vApprovedByUserID) {

            var pIU = new InsertUpdate('TransMemo');
            pIU.Add('LastUpdated', proj.SqliteNow);
            if (CBool(vIsRefund))
                pIU.Add('CheckOutMethod', "'Refund'");
            else
                pIU.Add('CheckOutMethod', "'SRV'");
            pIU.Add('ApprovedBy', ReadyString(vApprovedByUserID));
            pIU.Add('UserID', ReadyString(Web.PeopleUserID()));
            if (CStr2(sessionStorage.FowstID).trim() == '') {
                Android.AddLog('sessionStorage.FowstID is blank. proj.SrvFowstID will be used instead');
                sessionStorage.FowstID = proj.SrvFowstID;
            }
            pIU.Add('FowstID', ReadyString(sessionStorage.FowstID));
            pIU.Add('DeviceID', ReadyString(Web.DeviceID()));
            var pSql = pIU.GetUpdate('ID = ' + ReadyString(vMemoID));
            pSql = Sql.ExecuteSql(pSql); // output is used for debugging only


        }; // END this.Approve = function() {

        this.SaveToSalesReturnLedger = function(vMemoID, vIsRefund, vAdditionalData) {
            // counterpart of vb\Business\TransSalesReturnLedger.vb:Save

            var pDrSalesReturn = this.GetMemoAmount(vMemoID);
            if (pDrSalesReturn == null) {
                console.error('Sales Return not found, ' + vMemoID + '. ref: jsSTAB2FUYBW!');
                return;
            }
            var pSeriesFieldName; // POSLastSRVNo or POSLastRefNo
            var pUpdateFieldName; // RefundNum or SrvNum

            var pIU = new InsertUpdate('TransSalesReturnLedger');
            pIU.Add('ID', ReadyString(NewID()));
            pIU.Add('SourceMemoID', ReadyString(vMemoID));
            if (CStr2(sessionStorage.FowstID).trim() == '') {
                Android.AddLog('sessionStorage.FowstID is blank. proj.SrvFowstID will be used instead');
                sessionStorage.FowstID = proj.SrvFowstID;
            }
            pIU.Add('FowstID', ReadyString(sessionStorage.FowstID));
            if (vIsRefund) {
                pIU.Add('GrossRefundAmount', CDec2(pDrSalesReturn.TotalAmount).toFixed(2));
                pIU.Add('GrossSrvAmount', 0);
                pSeriesFieldName = 'POSLastRefNo';
                pUpdateFieldName = 'RefundNum';
                if ((vAdditionalData != null)
                    && (vAdditionalData.OtherPayments != null)
                    && (vAdditionalData.OtherPayments.length > 0)) {
                    
                    for (var pOtherPaymentIndex = 0; pOtherPaymentIndex <= vAdditionalData.OtherPayments.length - 1; pOtherPaymentIndex++) {
                        var pOtherPayment = vAdditionalData.OtherPayments[pOtherPaymentIndex];
                        pIU.Add('GrossRefundAmount' + pOtherPayment.PaymentType, pOtherPayment.Amount);
                        DeductOtherPaymentTypeToZRead(pOtherPayment.PaymentType, pOtherPayment.Amount)

                        function DeductOtherPaymentTypeToZRead(PaymentType, Amount) {
                            var pSql = "Update cache.CacheMSR SET OtherPayment" + PaymentType + "= OtherPayment" + PaymentType + " - " + CDec2(pOtherPayment.Amount).toFixed(2) +
                                       " WHERE SalesDate = " + ReadyString($.format.date((new Date()), 'yyyy-MM-dd')) +
                                       " AND FOWSTID = " + ReadyString(sessionStorage.FowstID) +
                                       " AND DeviceID = " + ReadyString(Web.DeviceID())

                            Sql.Execute(pSql);

                            pSql = "Update cache.CacheCSR SET OtherPayment" + PaymentType + "= OtherPayment" + PaymentType + " - " + CDec2(pOtherPayment.Amount).toFixed(2) +
                            " WHERE SalesDate = " + ReadyString($.format.date((new Date()), 'yyyy-MM-dd')) +
                                " AND FOWSTID = " + ReadyString(sessionStorage.FowstID) +
                                " AND DeviceID = " + ReadyString(Web.DeviceID()) +
                                " AND UserID = " + ReadyString(sessionStorage.PeopleID)

                            Sql.Execute(pSql);
                            
                            pSql = "Update CacheZRead set OtherTypeAmount = OtherTypeAmount - " + CDec2(pOtherPayment.Amount).toFixed(2) + " where SalesDate = " + ReadyString($.format.date(new Date(), 'yyyy-MM-dd'));

                            Sql.Execute(pSql);

                        }
                    }
                } else {
                    var pSql = "Update cache.CacheMSR SET Cash= Cash - " + CDec2(pDrSalesReturn.TotalAmount).toFixed(2) +
                    " WHERE SalesDate = " + ReadyString($.format.date((new Date()), 'yyyy-MM-dd')) +
                    " AND FOWSTID = " + ReadyString(sessionStorage.FowstID) +
                    " AND DeviceID = " + ReadyString(Web.DeviceID())

                    Sql.Execute(pSql);

                    pSql = "Update cache.CacheCSR SET Cash= Cash - " + CDec2(pDrSalesReturn.TotalAmount).toFixed(2) +
                    " WHERE SalesDate = " + ReadyString($.format.date((new Date()), 'yyyy-MM-dd')) +
                        " AND FOWSTID = " + ReadyString(sessionStorage.FowstID) +
                        " AND DeviceID = " + ReadyString(Web.DeviceID()) +
                        " AND UserID = " + ReadyString(sessionStorage.PeopleID)

                    Sql.Execute(pSql);

                    pSql = "Update CacheZRead SET CashAmount= CashAmount - " + CDec2(pDrSalesReturn.TotalAmount).toFixed(2) +
                    " WHERE SalesDate = " + ReadyString($.format.date((new Date()), 'yyyy-MM-dd')) +
                    " AND FOWSTID = " + ReadyString(sessionStorage.FowstID) +
                    " AND DeviceID = " + ReadyString(Web.DeviceID())

                    Sql.Execute(pSql);
                }
            } else {
                pIU.Add('GrossRefundAmount', 0);
                pIU.Add('GrossSrvAmount', CDec2(pDrSalesReturn.TotalAmount).toFixed(2));
                pSeriesFieldName = 'POSLastSRVNo';
                pUpdateFieldName = 'SrvNum';
            }
            pIU.Add('Fees', 0);
            pIU.Add('RefundFees', 0);
            
            pIU.Add('Amount', CDec2(pDrSalesReturn.TotalAmount));
            pIU.Add('UserID', ReadyString(Web.PeopleUserID()));
            if (CStr2(sessionStorage.FowstID).trim() == '') {
                Android.AddLog('sessionStorage.FowstID is blank. proj.SrvFowstID will be used instead');
                sessionStorage.FowstID = proj.SrvFowstID;
            }
            pIU.Add('FowstID', ReadyString(sessionStorage.FowstID));
            pIU.Add('DeviceID', ReadyString(Web.DeviceID()));
            pIU.Add('SourceInvoiceID', ReadyString(pDrSalesReturn.InvoiceID));
            pIU.Add('LastUpdated', proj.SqliteNow);
            var pPin = RandomString(6, 'ABCDEFGHJKLMNPQRSTUVWXYZ23456789').toUpperCase();
            pIU.Add('Pin', ReadyString(pPin));
            var pSrvNum = Business.TransInvoice.GetCounter({
                FieldName: pSeriesFieldName,
                InvoiceFieldName: 'dont use',
                DontUseTableCounter: true
            });
            pSrvNum = CInt2(pSrvNum);
            pIU.Add(pUpdateFieldName, pSrvNum + 1);

            var pDeviceFowstTransactionNumber = Business.TransInvoice.GetCounter({
                FieldName: 'DeviceFowstTransactionNumber',
                DontUseTableCounter: true
            }) + 1;
            pIU.Add('DeviceFowstTransactionNumber', pDeviceFowstTransactionNumber);

            var pSql = pIU.GetInsert();
            pSql = Sql.ExecuteSql(pSql); // output is used for debugging only

            pIU = new InsertUpdate('TransMemoSalesReturn');
            pIU.Add('Exportable', 1);
            pSql = pIU.GetUpdate('MemoID = ' + ReadyString(vMemoID));
            pSql = Sql.ExecuteSql(pSql); // output is used for debugging only

            pIU = new InsertUpdate('TransMemo');
            pIU.Add('Exportable', 1);
            pSql = pIU.GetUpdate('ID = ' + ReadyString(vMemoID));
            pSql = Sql.ExecuteSql(pSql); // output is used for debugging only

            Business.TransInvoice.UpdateCounter(pSrvNum + 1, {
                FieldName: pSeriesFieldName,
                InvoiceFieldName: 'dont use',
                DontUseTableCounter: true
            });

            Business.TransInvoice.UpdateCounter(pDeviceFowstTransactionNumber, {
                FieldName: 'DeviceFowstTransactionNumber',
                DontUseTableCounter: true
            });
        }; // END this.SaveToSalesReturnLedger = function() {
   
        this.GetRemainingValueByMemoID = function(vMemoID) {
            
            var pSql = 'SELECT SUM(IFNULL(Amount, 0)) AS Amount\n' +
                ',SUM(IFNULL(ForfeitedAmount, 0)) AS ForfeitedAmount\n' +
                ',COUNT(*) AS Count\n' +
                ' FROM TransSalesReturnLedger\n' +
                ' WHERE SourceMemoID = ' + ReadyString(vMemoID);
            var pDR = Sql.ExecuteRow(pSql);
            if (pDR == null)
                return 0;
            if (CDec2(pDR.Count) == 0) {
                var pDT = this.GetMemoAmount(vMemoID);
                var pTotal = 0;
                for (var pIndex = 0; pIndex <= pDT.Rows.length - 1; pIndex++) {
                    var pDR = pDT.Rows[pIndex];
                    pTotal += CDec2(pDR.TotalAmount);
                }
                return pTotal;
            } else {
                return CDec2(pDR.Amount) - CDec2(pDR.ForfeitedAmount);
            }

        }; // END this.GetRemainingValueByMemoID = function(vMemoID) {

        this.GetRemainingValueByPin = function(vSrvNum, vPin) {

            var pMemoID = this.GetMemoIDByPin(vSrvNum, vPin);
            if (CStr2(pMemoID) == '')
                return null;
            
            var pValue = this.GetRemainingValueByMemoID(pMemoID);
            return {
                MemoID: pMemoID,
                SRVAmount: pValue,
                DestinationPaymentCount: null // todo
            };

        }; // END this.GetMemoIDByPin = function(vSrvNum, vPin) {

        this.GetMemoIDByPin = function(vSrvNum, vPin) {

            var pSql = 'SELECT SourceMemoID\n' +
                ' FROM TransSalesReturnLedger\n' +
                ' WHERE SrvNum = ' + ReadyString(vSrvNum) + '\n' +
                ' AND Pin = ' + ReadyString(vPin);
            var pDR = Sql.ExecuteRow(pSql);
            if (pDR != null) {
                var pMemoID = CStr2(pDR.SourceMemoID);
                return pMemoID;
            }

        }; // END this.GetMemoIDByPin = function(vSrvNum, vPin) {

        this.GetByMemoID = function(vMemoID) {
            
            var pSql = 'SELECT ID\n' +
                ',SrvNum\n' +
                ',Pin\n' +
                ',FowstID\n' +
                ',DeviceID\n' +
                ' FROM TransSalesReturnLedger\n' +
                ' WHERE SourceMemoID = ' + ReadyString(vMemoID) + '\n' +
                ' AND DestinationPaymentID IS NULL'

            var pDR = Sql.ExecuteRow(pSql);

            return pDR;

        }; // END GetByMemoID

        this.Consume = function(vSourceMemoID, vDestinationPaymentID, vAmount) {

            var pDR = this.GetByMemoID(vSourceMemoID);
            if (pDR == null) {
                throw new Error('vSourceMemoID, ' + vSourceMemoID + ' does not exist. ref: js26C34TB3VF!');
            }

            var pIU = new InsertUpdate('TransSalesReturnLedger');
            pIU.Add('ID', ReadyString(NewID()));
            pIU.Add('SourceMemoID', ReadyString(vSourceMemoID));
            pIU.Add('DestinationPaymentID', ReadyString(vDestinationPaymentID));
            pIU.Add('Amount', -CDec2(vAmount));
            pIU.Add('UserID', ReadyString(Web.PeopleUserID()));
            pIU.Add('LastUpdated', proj.SqliteNow);
            pIU.Add('FowstID', ReadyString(pDR.FowstID));
            pIU.Add('DeviceID', ReadyString(pDR.DeviceID));
            pIU.Add('SrvNum', ReadyString(pDR.SrvNum));
            var pSql = pIU.GetInsert();
            Sql.ExecuteSql(pSql);

        }; // END Consume

        this.getSalesPaymentSRVRefundHeaderInfo = function(vSourceMemoID) {
            // offline version of SalesPaymentsController.getSalesPaymentSRVRefundHeaderInfo

            var pSql = ' SELECT srvledger.ID AS [ID]' + '\n' + 
                ' , inv.CustomerID AS [CustomerID]' + '\n' + 
                ' ,  (CASE cust.IsOrganization' + '\n' + 
                '      WHEN 1 THEN  cust.CompanyName' + '\n' + 
                '      ELSE' + '\n' + 
                '          IFNULL(NULLIF(LTRIM(RTRIM(cust.PersonSurname)) || \', \',\', \'), \'\') ||' + '\n' + 
                '          IFNULL(NULLIF(LTRIM(RTRIM(cust.PersonFirstName)) || \' \',\' \'), \'\') ||' + '\n' + 
                '          IFNULL(NULLIF(LTRIM(RTRIM(cust.PersonMiddleName)) || \' \',\' \'), \'\') ||' + '\n' + 
                '          IFNULL(NULLIF(\', \' + LTRIM(RTRIM(cust.PersonSuffixName)),\', \'), \'\')' + '\n' + 
                '  END)' + '\n' + 
                '  AS [CustomerName]' + '\n' + 
                ' , IFNULL(NULLIF(LTRIM(RTRIM(cust.OfficeAddressBuildingNo)) || \' \', \' \'), \'\') ||' + '\n' + 
                ' IFNULL(NULLIF(LTRIM(RTRIM(cust.OfficeAddressStreet)) || \', \',\', \'), \'\') ||' + '\n' + 
                ' IFNULL(NULLIF(LTRIM(RTRIM(custcty.Name)) || \', \',\', \'), \'\') ||' + '\n' + 
                ' IFNULL(NULLIF(LTRIM(RTRIM(custctry.Name)) || \', \',\', \'), \'\')' + '\n' + 
                '  AS [CustomerAddress]' + '\n' + 
                ' , cust.TinNumber AS [CustomerTIN]' + '\n' + 
                ' , srvledger.SourceMemoID AS [SourceMemoID]' + '\n' + 
                ' , srvledger.DestinationPaymentID AS [CustomerPaymentID]' + '\n' + 
                ' , srvledger.RefundNum' + '\n' + 
                ' , srvledger.RefundNum AS [SRVNumber]' + '\n' + 
                ' , srvledger.Pin AS [PinCode]' + '\n' + 
                ' , memo.LastUpdated AS [SRVDateTime]' + '\n' + 
                ' , retTo.OrganizationID AS [SubscriberOrganizationID]' + '\n' + 
                ' , inv.FowstID AS [BoughtToBranchID]' + '\n' + 
                ' , invsubsbr.[Name] AS [BoughtToBranchName]' + '\n' + 
                ' , srvledger.FowstID AS [ReturnToBranchID]' + '\n' + 
                ' , retTo.[Name] AS [ReturnToBranchName]' + '\n' + 
                ' , IFNULL(NULLIF(LTRIM(RTRIM(cshr.SurName)) || \', \',\', \'), \'\') ||' + '\n' + 
                ' IFNULL(NULLIF(LTRIM(RTRIM(cshr.FirstName)) || \' \',\' \'), \'\') ||' + '\n' + 
                ' IFNULL(NULLIF(LTRIM(RTRIM(cshr.MiddleName)) || \' \',\' \'), \'\')' + '\n' + 
                '  AS [CashierName]' + '\n' + 
                ' , IFNULL(NULLIF(LTRIM(RTRIM(approvedby.SurName)) || \', \',\', \'), \'\') ||' + '\n' +
                ' IFNULL(NULLIF(LTRIM(RTRIM(approvedby.FirstName)) || \' \',\' \'), \'\') ||' + '\n' +
                ' IFNULL(NULLIF(LTRIM(RTRIM(approvedby.MiddleName)) || \' \',\' \'), \'\')' + '\n' +
                '  AS [ApprovedBy]' + '\n' + 
                ' , inv.InvoiceNumber AS [InvoiceNumber]' + '\n' + 
                ' , IFNULL(' + '\n' + 
                '      (SELECT SUM(IFNULL(siitems.SubTotal,0) - IFNULL(siitems.DiscountAmount,0)) as InvoiceAmount' + '\n' + 
                '       FROM TransInvoiceItems siitems' + '\n' + 
                '       WHERE siitems.InvoiceID = inv.ID' + '\n' + 
                '      )' + '\n' + 
                ' ,0)' + '\n' + 
                '  AS [InvoiceAmount]' + '\n' + 
                ' , IFNULL(' + '\n' + 
                '      (SELECT SUM(IFNULL(sipayment.AmountPaid,0)) as AmountPaid' + '\n' + 
                '       FROM TransCustomerPaymentInvoices sipayment' + '\n' + 
                '       INNER JOIN TransCustomerPayment custpayment ON (sipayment.CustomerPaymentID = custpayment.ID)' + '\n' + 
                '       WHERE sipayment.InvoiceID = inv.ID' + '\n' + 
                '       AND custpayment.IsCancelled = 0' + '\n' + 
                '      )' + '\n' + 
                ' ,0)' + '\n' + 
                '  AS [InvoicePayment]' + '\n' + 
                ' , IFNULL(' + '\n' + 
                '      (SELECT IFNULL(SUM(GrossRefundAmount), 0) AS Amount' + '\n' +
                '       FROM TransSalesReturnLedger' + '\n' + 
                '       WHERE SourceMemoID = srvledger.SourceMemoID' + '\n' + 
                '       AND DestinationPaymentID IS NULL' + '\n' + 
                '      )' + '\n' + 
                ' ,0)' + '\n' + 
                '  AS [SRVOrigAmount]' + '\n' + 
                ' , IFNULL(' + '\n' + 
                '      (SELECT SUM(IFNULL(GrossRefundAmount, 0)) - SUM(IFNULL(RefundFees, 0)) AS Amount' + '\n' +
                '       FROM TransSalesReturnLedger' + '\n' + 
                '       WHERE SourceMemoID = srvledger.SourceMemoID' + '\n' + 
                '      )' + '\n' + 
                ' ,0)' + '\n' + 
                '  AS [SRVNewAmount]' + '\n' + 
                ' , IFNULL(' + '\n' + 
                '      (SELECT IFNULL(SUM(Fees), 0) AS Amount' + '\n' + 
                '       FROM TransSalesReturnLedger' + '\n' + 
                '       WHERE SourceMemoID = srvledger.SourceMemoID' + '\n' + 
                '       AND DestinationPaymentID IS NULL' + '\n' + 
                '      )' + '\n' + 
                ' ,0)' + '\n' + 
                '  AS [SRVRestockingFee]' + '\n' + 
                '  , srvledger.DeviceFowstTransactionNumber as TransactionNo ' + '\n' +
                '   FROM TransSalesReturnLedger srvledger' + '\n' + 
                '  INNER JOIN TransMemo memo ON (srvledger.SourceMemoID = memo.ID)' + '\n' + 
                '  INNER JOIN TransMemoSalesReturn memosret ON (srvledger.SourceMemoID = memosret.MemoID)' + '\n' + 
                '  INNER JOIN TransInvoice inv ON (memosret.InvoiceID = inv.ID)' + '\n' + 
                '  INNER JOIN LibraryOrganization cust ON (inv.CustomerID = cust.ID)' + '\n' + 
                '  LEFT JOIN LibraryCities custcty ON (cust.OfficeAddressCityID = custcty.ID)' + '\n' + 
                '  LEFT JOIN LibraryCountries custctry ON (cust.OfficeAddressCountryID = custctry.ID)' + '\n' + 
                '  LEFT JOIN LibraryOrganizationFOWST invsubsbr ON (inv.FowstID = invsubsbr.ID)' + '\n' + 
                '  LEFT JOIN LibraryOrganizationFOWST retTo ON (srvledger.FowstID = retTo.ID)' + '\n' + 
                '  LEFT JOIN LibraryPeople cshr ON (memo.UserID = cshr.ID)' + '\n' + 
                '  LEFT JOIN LibraryPeople approvedby ON (memo.ApprovedBy = approvedby.ID)' + '\n' +
                '   WHERE srvledger.SourceMemoID = ' + ReadyString(vSourceMemoID) + '\n' + 
                '  AND srvledger.DestinationPaymentID IS NULL';
            var pDT = Sql.Execute(pSql);

            return pDT;

        };

        this.getSalesPaymentSRVReceiptHeaderInfo = function(vSourceMemoID) {
            // offline version of SalesPaymentsController.getSalesPaymentSRVReceiptHeaderInfo

            var pSql = ' SELECT srvledger.ID AS [ID]' + '\n' + 
                ' , inv.CustomerID AS [CustomerID]' + '\n' + 
                ' ,  (CASE cust.IsOrganization' + '\n' + 
                '      WHEN 1 THEN  cust.CompanyName' + '\n' + 
                '      ELSE' + '\n' + 
                '          IFNULL(NULLIF(LTRIM(RTRIM(cust.PersonSurname)) || \', \',\', \'), \'\') ||' + '\n' + 
                '          IFNULL(NULLIF(LTRIM(RTRIM(cust.PersonFirstName)) || \' \',\' \'), \'\') ||' + '\n' + 
                '          IFNULL(NULLIF(LTRIM(RTRIM(cust.PersonMiddleName)) || \' \',\' \'), \'\') ||' + '\n' + 
                '          IFNULL(NULLIF(\', \' + LTRIM(RTRIM(cust.PersonSuffixName)),\', \'), \'\')' + '\n' + 
                '  END)' + '\n' + 
                '  AS [CustomerName]' + '\n' + 
                ' , IFNULL(NULLIF(LTRIM(RTRIM(cust.OfficeAddressBuildingNo)) || \' \', \' \'), \'\') ||' + '\n' + 
                ' IFNULL(NULLIF(LTRIM(RTRIM(cust.OfficeAddressStreet)) || \', \',\', \'), \'\') ||' + '\n' + 
                ' IFNULL(NULLIF(LTRIM(RTRIM(custcty.Name)) || \', \',\', \'), \'\') ||' + '\n' + 
                ' IFNULL(NULLIF(LTRIM(RTRIM(custctry.Name)) || \', \',\', \'), \'\')' + '\n' + 
                '  AS [CustomerAddress]' + '\n' + 
                ' , cust.TinNumber AS [CustomerTIN]' + '\n' + 
                ' , srvledger.SourceMemoID AS [SourceMemoID]' + '\n' + 
                ' , srvledger.DestinationPaymentID AS [CustomerPaymentID]' + '\n' + 
                ' , srvledger.RefundNum' + '\n' + 
                ' , srvledger.SrvNum AS [SRVNumber]' + '\n' + 
                ' , srvledger.Pin AS [PinCode]' + '\n' + 
                ' , memo.LastUpdated AS [SRVDateTime]' + '\n' + 
                ' , retTo.OrganizationID AS [SubscriberOrganizationID]' + '\n' + 
                ' , inv.FowstID AS [BoughtToBranchID]' + '\n' + 
                ' , invsubsbr.[Name] AS [BoughtToBranchName]' + '\n' + 
                ' , srvledger.FowstID AS [ReturnToBranchID]' + '\n' + 
                ' , retTo.[Name] AS [ReturnToBranchName]' + '\n' + 
                ' , IFNULL(NULLIF(LTRIM(RTRIM(cshr.SurName)) || \', \',\', \'), \'\') ||' + '\n' + 
                ' IFNULL(NULLIF(LTRIM(RTRIM(cshr.FirstName)) || \' \',\' \'), \'\') ||' + '\n' + 
                ' IFNULL(NULLIF(LTRIM(RTRIM(cshr.MiddleName)) || \' \',\' \'), \'\')' + '\n' + 
                '  AS [CashierName]' + '\n' + 
                ' , IFNULL(NULLIF(LTRIM(RTRIM(approvedby.SurName)) || \', \',\', \'), \'\') ||' + '\n' +
                ' IFNULL(NULLIF(LTRIM(RTRIM(approvedby.FirstName)) || \' \',\' \'), \'\') ||' + '\n' +
                ' IFNULL(NULLIF(LTRIM(RTRIM(approvedby.MiddleName)) || \' \',\' \'), \'\')' + '\n' +
                '  AS [ApprovedBy]' + '\n' +
                ' , inv.InvoiceNumber AS [InvoiceNumber]' + '\n' + 
                ' , IFNULL(' + '\n' + 
                '      (SELECT SUM(IFNULL(siitems.SubTotal,0) - IFNULL(siitems.DiscountAmount,0)) as InvoiceAmount' + '\n' + 
                '       FROM TransInvoiceItems siitems' + '\n' + 
                '       WHERE siitems.InvoiceID = inv.ID' + '\n' + 
                '      )' + '\n' + 
                ' ,0)' + '\n' + 
                '  AS [InvoiceAmount]' + '\n' + 
                ' , IFNULL(' + '\n' + 
                '      (SELECT SUM(IFNULL(sipayment.AmountPaid,0)) as AmountPaid' + '\n' + 
                '       FROM TransCustomerPaymentInvoices sipayment' + '\n' + 
                '       INNER JOIN TransCustomerPayment custpayment ON (sipayment.CustomerPaymentID = custpayment.ID)' + '\n' + 
                '       WHERE sipayment.InvoiceID = inv.ID' + '\n' + 
                '       AND custpayment.IsCancelled = 0' + '\n' + 
                '      )' + '\n' + 
                ' ,0)' + '\n' + 
                '  AS [InvoicePayment]' + '\n' + 
                ' , IFNULL(' + '\n' + 
                '      (SELECT IFNULL(SUM(GrossSrvAmount), 0) AS Amount' + '\n' + 
                '       FROM TransSalesReturnLedger' + '\n' + 
                '       WHERE SourceMemoID = srvledger.SourceMemoID' + '\n' + 
                '       AND DestinationPaymentID IS NULL' + '\n' + 
                '      )' + '\n' + 
                ' ,0)' + '\n' + 
                '  AS [SRVOrigAmount]' + '\n' + 
                ' , IFNULL(' + '\n' + 
                '      (SELECT SUM(IFNULL(Amount, 0)) - SUM(IFNULL(ForfeitedAmount, 0)) AS Amount' + '\n' + 
                '       FROM TransSalesReturnLedger' + '\n' + 
                '       WHERE SourceMemoID = srvledger.SourceMemoID' + '\n' + 
                '      )' + '\n' + 
                ' ,0)' + '\n' + 
                '  AS [SRVNewAmount]' + '\n' + 
                ' , IFNULL(' + '\n' + 
                '      (SELECT IFNULL(SUM(Fees), 0) AS Amount' + '\n' + 
                '       FROM TransSalesReturnLedger' + '\n' + 
                '       WHERE SourceMemoID = srvledger.SourceMemoID' + '\n' + 
                '       AND DestinationPaymentID IS NULL' + '\n' + 
                '      )' + '\n' + 
                ' ,0)' + '\n' + 
                '  AS [SRVRestockingFee]' + '\n' + 
                '  , srvledger.DeviceFowstTransactionNumber as TransactionNo ' + '\n' +
                '   FROM TransSalesReturnLedger srvledger' + '\n' + 
                '  INNER JOIN TransMemo memo ON (srvledger.SourceMemoID = memo.ID)' + '\n' + 
                '  INNER JOIN TransMemoSalesReturn memosret ON (srvledger.SourceMemoID = memosret.MemoID)' + '\n' + 
                '  INNER JOIN TransInvoice inv ON (memosret.InvoiceID = inv.ID)' + '\n' + 
                '  INNER JOIN LibraryOrganization cust ON (inv.CustomerID = cust.ID)' + '\n' + 
                '  LEFT JOIN LibraryCities custcty ON (cust.OfficeAddressCityID = custcty.ID)' + '\n' + 
                '  LEFT JOIN LibraryCountries custctry ON (cust.OfficeAddressCountryID = custctry.ID)' + '\n' + 
                '  LEFT JOIN LibraryOrganizationFOWST invsubsbr ON (inv.FowstID = invsubsbr.ID)' + '\n' + 
                '  LEFT JOIN LibraryOrganizationFOWST retTo ON (srvledger.FowstID = retTo.ID)' + '\n' + 
                '  LEFT JOIN LibraryPeople cshr ON (memo.UserID = cshr.ID)' + '\n' + 
                '  LEFT JOIN LibraryPeople approvedby ON (memo.ApprovedBy = approvedby.ID)' + '\n' +
                '   WHERE srvledger.SourceMemoID = ' + ReadyString(vSourceMemoID) + '\n' + 
                '  AND srvledger.DestinationPaymentID IS NULL';
            var pDT = Sql.Execute(pSql);

            return pDT;

        }; // END this.getSalesPaymentSRVReceiptHeaderInfo = function(vSourceMemoID) {

        this.getSalesPaymentSRVReceiptInvoices = function(vSourceMemoID) {
            var pSql = ' SELECT custpayinv.InvoiceID AS [InvoiceID]' + '\n' + 
                ' , inv.InvoiceNumber AS [InvoiceNumber]' + '\n' + 
                ' , inv.InvoiceDate AS [InvoiceDate]' + '\n' + 
                ' , (srledger.Amount * -1) AS [UsedAmount]' + '\n' + 
                '   FROM TransSalesReturnLedger srledger  ' + '\n' + 
                ' JOIN TransCustomerPaymentInvoices custpayinv ON (srledger.DestinationPaymentID = custpayinv.CustomerPaymentID)  ' + '\n' + 
                ' JOIN TransInvoice inv ON (custpayinv.InvoiceID = inv.ID)  ' + '\n' + 
                ' WHERE srledger.SourceMemoID = ' + ReadyString(vSourceMemoID) + '\n' + 
                ' AND srledger.DestinationPaymentID IS NOT NULL    ' + '\n' + 
                ' ORDER BY inv.InvoiceDate ASC';

            var pDT = Sql.Execute(pSql);

            return pDT;

        }; // END this.getSalesPaymentSRVReceiptInvoices = function()

        this.GetSettings = function() {
            // offline version of SalesReturnsController.GetSettings

            var pOut = {};
            pOut.RestockingFeePercent= Business.SystemDefaults.SalesReturnRestockingFeePercent();
            pOut.MaximumReturnablePeriod = Business.SystemDefaults.SalesReturntMaximumReturnablePeriod();
            pOut.ReturnedPrice = Business.SystemDefaults.SalesReturntReturnedPrice();
            pOut.SRVUseSettings = Business.SystemDefaults.SalesReturntSRVSettings();
            pOut.PrintRefundSignUp = Business.SystemDefaults.SalesReturntPrintRefundSignUp();
            pOut.AllowOfflineSrvPayments = Business.SystemDefaults.AllowOfflineSrvPayments();
            pOut.HideMerchandiseExchange = Business.SystemDefaults.HideMerchandiseExchange();
            pOut.RestrictedTransactionLookupForSalesReturn = Business.SystemDefaults.RestrictedTransactionLookupForSalesReturn();
            pOut.AllowedReturnDuration = Business.SystemDefaults.AllowedReturnDuration();

            return pOut;

        }; // END this.GetSettings = function() {

        this.ReturnInvoice = function (vInvoiceID) {
            var pReturnReason = 'Change of mind';
            var pCart = Business.TransInvoice.GetCart(vInvoiceID);
            Sql.BeginTransaction();
            try {
                if (pCart.Invoices.Rows.length == 0) 
                    throw new Error('Invoice not found. jsU4M6J5W3QP!');
                if (pCart.Stocks.Rows.length == 0) 
                    throw new Error('Invoice has no items. jsJ4WQVQGKRX!');
                var pInvoice = pCart.Invoices.Rows[0];
                var pMemoID = null;
                for(var pItemIndex = 0; pItemIndex <= pCart.Stocks.Rows.length - 1; pItemIndex++) {
                    var pStock = pCart.Stocks.Rows[pItemIndex];
                    var pData = {};
                    pData.MemoID = pMemoID;
                    pData.StockID = pStock.ID;
                    pData.CustomerID = pInvoice.CustomerID;
                    pData.InvoiceID = vInvoiceID;
                    pData.Stocks = [];
                    pData.Stocks.push({
                        Qty: pStock.Quantity,
                        UnitQty: pStock.ActualQuantity,
                        UnitMeasure: pStock.UnitMeasure,
                        UnitPrice: pStock.UnitPrice,
                        UnitDiscountAmount: pStock.UnitDiscountAmount, // todo
                        TaxAmount: pStock.TaxAmount,
                        NetDue: pStock.NetDue,
                        ReturnReason: pReturnReason
                    });
                    pMemoID = Business.TransSalesReturn.Update(pData);
                }
                mThis.Approve(pMemoID, true);
                mThis.SaveToSalesReturnLedger(pMemoID, true);
                Sql.CommitTransaction();
            } catch(e) {
                Sql.RollbackTransaction();
                throw e;
                console.log(e);
            }

        }; // END this.ReturnInvoice = function (vInvoiceID) {

        this.ReturnInvoice2 = function (vParams) {
            var pReturnReason = 'Change of mind';
            var vParams = $.extend({
                InvoiceID: null,
                /*
                Stocks: [
                    {
                        ProductID: '',
                        Qty: '',
                        UnitQty: '',
                        UnitMeasure: '',
                        UnitPrice: '',
                        TaxAmount: '',
                    }
                ]
                */
            }, vParams);

            if (CStr2(sessionStorage.FowstID).trim() == '')
                throw new Error('sessionStorage.FowstID is required. jsDXV3QEMY7U!');
            if (!IsGuid(sessionStorage.FowstID))
                throw new Error('sessionStorage.FowstID must be a GUID. jsVPAUWJX2MJ!')
            if (vParams.InvoiceID == null)
                throw new Error('InvoiceID is required. jsKYM4E6AYBV!');
            if (!IsGuid(vParams.InvoiceID))
                throw new Error('InvoiceID must be a GUID. jsBEW5CKZ2J5!');
            if (vParams.Stocks == null)
                throw new Error('Stocks is required. jsTTEGE5U2NJ');
            if (!$.isArray(vParams.Stocks))
                throw new Error('Stocks must be an arrary. jsED8CCEF8XV');
            for(var pItemIndex = 0; pItemIndex <= vParams.Stocks.length - 1; pItemIndex++) {
                var pStock = vParams.Stocks[pItemIndex];
                if (pStock.ProductID == null)
                    throw new Error('Stocks[' + pItemIndex + '].ProductID is required. jsTWX572GRBJ!');
                if (pStock.Quantity == null)
                    throw new Error('Stocks[' + pItemIndex + '].Quantity is required. jsFKTLB5BNT3!');
                if (pStock.ActualQuantity == null)
                    throw new Error('Stocks[' + pItemIndex + '].ActualQuantity is required. js6TLN7DQPY5!');
                if (pStock.UnitMeasure == null)
                    throw new Error('Stocks[' + pItemIndex + '].UnitMeasure is required. jsGULDJWBS8U!');
                if (pStock.UnitPrice == null)
                    throw new Error('Stocks[' + pItemIndex + '].UnitPrice is required. jsK359V8PHPV!');
                if (pStock.TaxAmount == null)
                    throw new Error('Stocks[' + pItemIndex + '].TaxAmount is required. jsQNZESSV2T6!');
            }
            var pInvoice = Business.TransInvoice.GetByID(vParams.InvoiceID);
            if (pInvoice.Rows.length == 0)
                throw new Error('Invoice not found. jsTF6GLM3A2V!');
            pInvoice = pInvoice.Rows[0];

            Sql.BeginTransaction();
            try {
                var pMemoID = null;
                for(var pItemIndex = 0; pItemIndex <= vParams.Stocks.length - 1; pItemIndex++) {
                    var pStock = vParams.Stocks[pItemIndex];
                    var pData = {};
                    pData.MemoID = pMemoID;
                    pData.StockID = pStock.ProductID;
                    pData.CustomerID = pInvoice.CustomerID;
                    pData.InvoiceID = vParams.InvoiceID;
                    pData.Stocks = [];
                    pData.Stocks.push({
                        Qty: pStock.Quantity,
                        UnitQty: pStock.ActualQuantity,
                        UnitMeasure: pStock.UnitMeasure,
                        UnitPrice: pStock.UnitPrice,
                        UnitDiscountAmount: pStock.UnitDiscountAmount, // todo
                        TaxAmount: pStock.TaxAmount,
                        NetDue: pStock.NetDue,
                        ReturnReason: pReturnReason
                    });
                    pMemoID = Business.TransSalesReturn.Update(pData);
                }
                mThis.Approve(pMemoID, true);
                mThis.SaveToSalesReturnLedger(pMemoID, true);
                Sql.CommitTransaction();
                return pMemoID;
            } catch(e) {
                Sql.RollbackTransaction();
                throw e;
                console.log(e);
                return '';
            }

        }; // END this.ReturnInvoice2 = function (vParams) {

        this.GetAllSaleReturns = function () {
            var pSql = "SELECT TransMemoSalesReturn.MemoID, InvoiceNumber, CheckoutMethod as CheckoutMethod, SUM(TransMemoSalesReturn.TotalAmount) as TotalAmount, SrvNum as SRVNum, RefundNum, Pin, TransMemoSalesReturn.LastUpdated \n" +
                " FROM TransMemoSalesReturn \n" +
                " INNER JOIN TransMemo ON TransMemo.ID = TransMemoSalesReturn.MemoID \n" +
                " INNER JOIN TransInvoice ON TransInvoice.ID = TransMemoSalesReturn.InvoiceID \n" +
                " INNER JOIN TransSalesReturnLedger ON TransSalesReturnLedger.SourceMemoID = TransMemo.ID \n" +
                " WHERE TransSalesReturnLedger.FowstID = " + ReadyString(sessionStorage.FowstID) + " \n" +
                " GROUP BY TransMemoSalesReturn.MemoID " +
                " ORDER BY TransMemoSalesReturn.LastUpdated DESC"
            return Sql.Execute(pSql).Rows
        }
        return this;
    };
});