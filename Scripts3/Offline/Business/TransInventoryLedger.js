define([], function () {
    window.Business.TransInventoryLedger = new function (vParent) {
        this.Add = function (vStockID,
        vFowstID,
        vTransactionID,
        vTransactionType,
        vUserID,
        vQty,
        vBalance,
        vBinID,
        vSubTransactionType,
        vParentID,
        vInventoryCountDate,
        vValue) {

            //Dim pTransClass As TransClass
            var pIU;
            var pSql = '';
            var pQty;
            var pBalance;

            var pFinder = new Finder();
            var pDT;
            var pOut;

            if (vQty == undefined && vBalance == undefined) {
                //error Throw New ArgumentException("vQty and vBalance cannot be both nothing")
            }

            switch (vTransactionType) {
                case 'Invoice':
                    pTransClass = 'Invoice';
                    break;
                case 'InventoryCount':
                    pTransClass = 'InventoryCount';
                    break;
                case 'Receiving':
                    pTransClass = 'Receiving';
                    break;
                case 'TransferOut':
                    pTransClass = 'TransferOut';
                    break;
                case 'SalesReturn':
                    pTransClass = 'Sales Returns';
                    break;
                case 'ProductMaker':
                    pTransClass = 'Product Maker';
                    break;
                case 'CarryOver':
                    pTransClass = 'Carryover of ending balance of unnamed bin to new default bin';
                    break;
                case 'InventoryCountAdjustment':
                    pTransClass = 'Clearing ending balance of unnamed bin for carrying over to new default bin';
                    break;
                case 'BeginningBalance':
                    pTransClass = 'Beginning Balance';
                    break;
            }

            if (vBinID == undefined) {
                vBinID = GetOrAddDefaultBin(vFowstID, vStockID)
            }

            if (vQty == undefined) {
                pQty = CDec2(vBalance) - GetBalance(vStockID, vFowstID, vBinID)
            } else {
                pQty = vQty
            }

            if (vBalance == undefined) {
                pBalance = GetBalance(vStockID, vFowstID, vBinID) + CDec2(pQty)
            }
            else {
                pBalance = vBalance;
            }

            if (pQty == 0 && !(vTransactionType == 'InventoryCount' || vTransactionType == 'Beginning Balance'))
                return; // nothing to save

            if (vTransactionType == 'InventoryCount') {
                //if 0 then its possible that its a new bin since GetBalance() will result to 0
                if ((vBalance - pQty) == 0) {

                }
            }

            pIU = new InsertUpdate("private.TransInventoryLedger");
            pOut = NewID()

            pIU.Add('ID', ReadyString(pOut));
            pIU.Add('ProductID', ReadyString(vStockID));
            pIU.Add('FowstID', ReadyString(vFowstID));
            pIU.Add('TransactionID', ReadyString(vTransactionID));
            pIU.Add('TransactionType', ReadyString(pTransClass));
            pIU.Add('BinID', ReadyString(vBinID));
            //if vExpirationDate IsNot Nothing Then pIU("ExpirationDate") = ReadyString(vExpirationDate)            
            pIU.Add('Qty', pQty);
            pIU.Add('Balance', pBalance);
            //pIU("") = 
            pIU.Add('UserID', ReadyString(vUserID));

            if (vValue != undefined) {
                pIU.Add('Value', vValue);
            }
            
            //if vBadCount IsNot Nothing Then pIU("BadCount") = vBadCount
            //pIU.Add('Reason', ReadyString(vBadCountReason));
            if (CStr2(vSubTransactionType) != '') pIU.Add('SubtransactionType', ReadyString(vSubTransactionType))
            if (CStr2(vParentID) != '') pIU.Add('ParentID', ReadyString(vParentID)); 
            pIU.Add('LastUpdated', proj.GetZReadDateSql());
            //if(IsDate(vInventoryCountDate)) pIU("LastUpdated") = ReadyString(vInventoryCountDate)

            pSql = pIU.GetInsert()
            Sql.Execute(pSql)

            pIU = new InsertUpdate("private.LibraryFowstBinProduct");

            pIU.Add('Balance', pBalance)
            pIU.Add('DateImported', 'NULL')
            pIU.Add('LastUpdated', proj.SqliteNow);
            pIU.Add('MaxPerProduct', 1)

            pSql = pIU.GetUpdate(" FowstID = " + ReadyString(vFowstID) + " AND ProductID = " + ReadyString(vStockID) + " AND BinID = " + ReadyString(vBinID))
            Sql.Execute(pSql)

            if (vBinID != '10d4672a-0358-4e2d-9471-2ee42e464300') { //summation bin id                

                if (vTransactionType == 'InventoryCount') {
                    vTransactionType = 'Inv Count'
                    this.Add(
                    vStockID,
                    vFowstID,
                    vTransactionID,
                    vTransactionType,
                    vUserID,
                    pQty,
                    vBalance,
                    '10d4672a-0358-4e2d-9471-2ee42e464300',
                    vSubTransactionType,
                    vParentID,
                    vInventoryCountDate,
                    vValue)
                } else {
                    this.Add(
                    vStockID,
                    vFowstID,
                    vTransactionID,
                    vTransactionType,
                    vUserID,
                    vQty,
                    vBalance,
                    '10d4672a-0358-4e2d-9471-2ee42e464300',
                    vSubTransactionType,
                    vParentID,
                    vInventoryCountDate,
                    vValue)
                }           
            }
                
            //'''Organization PIM 
            //'''Added By Francis A. 
            //''' Check if Product not yet added to private LibraryOrganizationPIM, if yes, then insert new record
            //''' 08/14/2013
            //pFinder = New Finder
            //With pFinder
            //    .ProData = mProData
            //With .Fields
            //With .Add("ProductID", SqlDbType.UniqueIdentifier)
            //    .Expression = "orgpimprd.ProductID"
            //End With
            //End With

            //.SqlFrom = " FROM dbo.LibraryOrganizationPIM orgpimprd "

            //.SqlWhere = " WHERE orgpimprd.ProductID = " + ReadyString(vStockID)

            //pDT = .Requery
            //End With

            //If (pDT.Rows.Count <= 0) Then
            //pIU = New InsertUpdate("LibraryOrganizationPIM")
            //pIU("ProductID") = ReadyString(vStockID)
            //pIU("ActivationDate") = "getdate()"
            //pIU("IsDeactivated") = "0"
            //pIU("IsDoNotShareTransDataToBrandOwner") = "0"
            //pIU("LastUpdated") = "GetDate()"

            //pSql = pIU.GetInsert()
            //mProData.ExecuteSQL(pSql)

            //'pSql = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'MobizLibraryPIMProduct'"
            //'Common.WebRequest.ProData.ExecuteSQL(pSql)

            //pSql = "Update TablesLastUpdated Set LastUpdated = GetDate() Where TableName = 'LibraryOrganizationPIM'"
            //mProData.ExecuteSQL(pSql)
            //End If
            //'''END Organization PIM 

        },

        this.GetReport = function (vProductID, vFowstID, vDateFrom, vDateTo)
        {
            var pSql = ''
            var pDT;

            pSql = 'Select ' + 
                'Transinventoryledger.ID ' + 
                ', LibraryFowstBin.Name BinName' + 
                ', TransactionType TransName' +
                ', SubtransactionType' + 
                ', Qty Quantity' + 
                ', Balance' + 
                ', Transinventoryledger.LastUpdated' +
                ', Surname || \', \' || FirstName UpdatedBy ' +
                'from Transinventoryledger inner join LibraryFowstBin on Transinventoryledger.BinID = LibraryFowstBin.ID ' + 
                'inner join LibraryPeople on Transinventoryledger.UserID = LibraryPeople.ID ' +
                ' WHERE Transinventoryledger.ProductID = ' + ReadyString(vProductID) +
                ' AND Transinventoryledger.FowstID = ' + ReadyString(vFowstID) +
                ' AND date(Transinventoryledger.LastUpdated) BETWEEN ' + ReadyString($.format.date(vDateFrom, 'yyyy-MM-dd')) + ' AND ' + ReadyString($.format.date(vDateTo, 'yyyy-MM-dd')) +
                ' Order By Transinventoryledger.LastUpdated'

            pDT = Sql.Execute(pSql);
            return pDT;
        };
        
        this.TransPimSalesCachingCount = function(vFowstID, vDate) {
            var pSql = 'SELECT COUNT(*) AS Count\n' +
                ' FROM TransPimSalesCaching\n' +
                ' WHERE FowstID = ' + ReadyString(vFowstID) + '\n' +
                ' AND strftime(\'%Y/%m/%d\', DateIntervalStart) = ' + ReadyString(FormatDate(vDate, {Format: 'yyyy/MM/dd'}));
            pOut = Sql.ExecuteScalar(pSql)
            return CDec2(pOut)
        }

        return this;
    };


    function GetOrAddDefaultBin(vCustomerAddressID, vStockID) {

        var pOut;
        var pSql = '';
        var pIU;

        pSql = "SELECT BinID" +
            " FROM LibraryFowstBinProduct" +
            " WHERE FowstID = " + ReadyString(vCustomerAddressID) +
            " AND ProductID = " + ReadyString(vStockID) +
            " AND IsDefault = 1 "

        pOut = Sql.ExecuteScalar(pSql)

        if (pOut != undefined) {
            return pOut;
        } else {
            pSql = "SELECT BinID" +
            " FROM LibraryFowstBinProduct" +
            " WHERE FowstID = " + ReadyString(vCustomerAddressID) +
            " AND ProductID = " + ReadyString(vStockID) +
            " LIMIT 1"

            pOut = Sql.ExecuteScalar(pSql)

            if (pOut != undefined) {
                return pOut;
            } else {
                
                pIU = new InsertUpdate("private.LibraryFowstBin");
                pOut = NewID()

                pIU.Add('ID', ReadyString(pOut));
                pIU.Add('FowstID', ReadyString(vCustomerAddressID));
                pIU.Add('Name', ReadyString('Unnamed Bin'));
                pIU.Add('Description', ReadyString(''));
                pSql = pIU.GetInsert()
                Sql.ExecuteSql(pSql);

                pIU = new InsertUpdate("private.LibraryFowstBinProduct");
                pIU.Add('ID', ReadyString(NewID()));
                pIU.Add('FowstID', ReadyString(vCustomerAddressID));
                pIU.Add('BinID', ReadyString(pOut));
                pIU.Add('ProductID', ReadyString(vStockID));
                pIU.Add('IsDefault', 1);

                pSql = pIU.GetInsert()
                Sql.ExecuteSql(pSql);

                return pOut
            }
        }
         
    }

    function GetBalance(vStockID, vFowstID, vBinID) {

        var pSql = '';

        pSql = "SELECT Balance" +
            " FROM TransInventoryLedger" + 
            " WHERE ProductID = " + ReadyString(vStockID) +
            " AND FowstID = " + ReadyString(vFowstID) +
            " AND BinID = " + ReadyString(vBinID) ;

        pSql += " ORDER BY LastUpdated DESC " +
                " limit 1"

        pOut = Sql.ExecuteScalar(pSql)

        if (pOut == null) {
            pSql = "SELECT Balance" +
                " FROM LibraryFowstBinProduct " + 
                " WHERE ProductID = " + ReadyString(vStockID) +
                " AND FowstID = " + ReadyString(vFowstID) +
                " AND BinID = " + ReadyString(vBinID);
            pSql += " ORDER BY LastUpdated DESC " +
                " limit 1";
            pOut = Sql.ExecuteScalar(pSql)
        }

        return CDec2(pOut)
    }
   
});