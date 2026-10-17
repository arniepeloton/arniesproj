define(['common'], // /Scripts3/model/TableView_Offline.js
function () {
    CreateStringObject('App.model.TableView', null, function () {        
        var pData = {};

        pData.GetTableTemplate = function (fowstID) {
            var pSql, pDT;
            var arrTemplateData = {}

            pSql = "SELECT temp.ID, temp.Name " + '\n' +
                    " FROM LibraryTableTemplate as temp " + '\n' +
                    " INNER JOIN LibraryTableFowst as tempfowst ON tempfowst.TemplateID = temp.ID " + '\n' +
                    " WHERE tempfowst.FowstID = " + ReadyString(fowstID);
            pDT = Sql.Execute(pSql);

            if (pDT.Rows.length > 0) {
                arrTemplateData.TemplateID = pDT.Rows[0].ID;
                arrTemplateData.TemplateName = pDT.Rows[0].Name;
                arrTemplateData.DiningArea = this.GetDiningArea(pDT.Rows[0].ID);
                return arrTemplateData
            } else {
                return null;
            }
        }

        pData.GetDiningArea = function (tempID) {
            var pSql, pDT;

            pSql = "SELECT * " + '\n' +
                    " FROM LibraryTableDiningArea " + '\n' +
                    " WHERE TemplateID = " + ReadyString(tempID) + '\n' +
                    " ORDER BY OrderNo ASC"
            pDT = Sql.Execute(pSql);

            if (pDT.Rows.length > 0) {
                for (var i = 0; i < pDT.Rows.length; i++){
                    do {
                        pDT.Rows[i].Tables = this.GetDiningTables(pDT.Rows[i].ID);
                        if (DeleteUnoccupiedSplitTables(pDT.Rows[i].Tables) == 0)
                            break;
                    } while (true)
                    
                }
                return pDT.Rows;
            } else {
                return null;
            }
        }

        function DeleteUnoccupiedSplitTables(vDiningTables) {
            var pOut = 0;
            for (var p = 0; p <= vDiningTables.length - 1; p++) {
                var pDR = vDiningTables[p];
                if (!CBool(pDR.IsOccupied)
                    && (CStr2(pDR.ParentID) != '')) {

                    var pSql = 'DELETE FROM LibraryTableTableInfo\n' + 
                        ' WHERE ID = ' + ReadyString(pDR.ID);
                    Sql.ExecuteSql(pSql);
                    pOut++;
                }
            }
            return pOut;
        }

        pData.GetDiningTables = function (diningID) {
            var pSql, pDT;
            //this id 'cd71a0f7-08a9-4601-876e-5f13c064f873' is equal to [CASH PAYMENT], no need to display this default customer name
            pSql = ' SELECT *' + '\n' + 
                '    ,(SELECT CASE cus.ID WHEN \'cd71a0f7-08a9-4601-876e-5f13c064f873\' THEN \'\' ELSE IFNULL(cus.PersonFullname, cus.PersonFirstName || \' \' || cus.PersonSurname) END' + '\n' + 
                '    FROM TransInvoice as inv' + '\n' + 
                '    LEFT JOIN LibraryOrganization as cus ON cus.ID = inv.CustomerID' + '\n' + 
                '    WHERE inv.ID = InvoiceID) AS CustomerName' + '\n' + 
                ' ,CASE WHEN InvoiceID IS NULL THEN 0 ELSE 1 END AS IsOccupied' + '\n' + 
                ' FROM (' + '\n' + 
                '    SELECT *' + '\n' + 
                '    ,(SELECT ID' + '\n' + 
                '       FROM TransInvoice' + '\n' + 
                '       WHERE CustomerTableNo = \'TBL_\' || LibraryTableTableInfo .InternalTableName' + '\n' + 
                '       AND IFNULL(TransInvoice.InvoiceNumber, 0) = 0' + '\n' + 
                '       AND TransInvoice.ItemCount > 0 ' + '\n' + 

                '      AND EXISTS (' + '\n' + 
                '            SELECT *' + '\n' + 
                '            FROM TransInvoiceItems' + '\n' + 
                '            WHERE InvoiceID = TransInvoice.ID' + '\n' + 
                '            AND ( \n' +
                '               (Quantity > 0 OR OrderedQuantity > 0)' + '\n' + 
                '               OR IFNULL(OrderedQuantity, 0) <> IFNULL(QuantityPrinted, 0)' + '\n' + 
                '            )\n' +
                '       )\n' +

                '       ORDER BY datetime(TransInvoice.LastUpdated) desc' + '\n' + 
                '       LIMIT 1' + '\n' + 
                '     ) AS InvoiceID' + '\n' + 
                '     ,(SELECT IsParent' + '\n' + 
                '     	FROM LibraryTableJoined' + '\n' + 
                '     	WHERE TableID = LibraryTableTableInfo.ID) as IsParent' + '\n' + 
                '    FROM LibraryTableTableInfo' + '\n' + 
                '    WHERE DiningAreaID = '  + ReadyString(diningID) + '\n' + 
                ' ) t' + '\n' + 
                '  ORDER BY OrderNo, TableName';

            //pSql = ' SELECT *' + '\n' + 
            //    ' ,(SELECT 1' + '\n' + 
            //    '     FROM TransInvoice' + '\n' + 
            //    '     WHERE CustomerTableNo = \'TBL_\' || LibraryTableTableInfo .InternalTableName' + '\n' + 
            //    '     AND IFNULL(TransInvoice.InvoiceNumber, 0) = 0' + '\n' + 
            //    '     AND EXISTS (' + '\n' + 
            //    '           SELECT *' + '\n' + 
            //    '           FROM TransInvoiceItems' + '\n' + 
            //    '           WHERE InvoiceID = TransInvoice.ID' + '\n' + 
            //    '           AND (Quantity > 0 OR OrderedQuantity > 0)' + '\n' + 
            //    '      )\n' +
            //    '     LIMIT 1' + '\n' + 
            //    ' ) AS IsOccupied' + '\n' +
            //    ' , (SELECT IsParent FROM LibraryTableJoined WHERE TableID = LibraryTableTableInfo.ID) as IsParent' + '\n' +
            //    ' , (SELECT CASE cus.ID WHEN \'cd71a0f7-08a9-4601-876e-5f13c064f873\' THEN \'\' ELSE IFNULL(cus.PersonFullname, cus.PersonFirstName || \' \' || cus.PersonSurname) END FROM TransInvoice as inv ' +
            //    '   LEFT JOIN LibraryOrganization as cus ON cus.ID = inv.CustomerID WHERE inv.CustomerTableNo =  \'TBL_\' || ' +
            //    '   LibraryTableTableInfo.InternalTableName ORDER BY datetime(inv.LastUpdated) desc) as CustomerName ' +
            //    ' FROM LibraryTableTableInfo ' + '\n' + 
            //    " WHERE DiningAreaID = " + ReadyString(diningID) + '\n' +
            //    ' ORDER BY OrderNo ASC';
            pDT = Sql.Execute(pSql);

            if (pDT.Rows.length > 0) {
                return pDT.Rows;
            } else {
                return null;
            }
        }

        pData.SaveJoinedTables = function (data) {
            var pIU = new InsertUpdate('LibraryTableJoined');
            var pSql;

            try {
                pIU.Add('FowstID', ReadyString(sessionStorage.FowstID))
                pIU.Add('SetID', ReadyString(data.TableSetID))
                pIU.Add('TableID', ReadyString(data.ID));
                pIU.Add('IsOccupied', ReadyString(data.IsOccupied));
                pIU.Add('IsParent', (data.IsParent == undefined ? ReadyString(false) : ReadyString(data.IsParent)));
                pIU.Add('UserID', ReadyString(Web.PeopleUserID()));
                pIU.Add('LastUpdated', proj.SqliteNow);

                if (data.State == 'add') {
                    pIU.Add('IsSplitTable', 0);
                    pIU.Add('ID', ReadyString(data.JoinedID))
                    pSql = pIU.GetInsert();
                }
                else if (data.State == 'update' || data.State == 'edit') {
                    pIU.Add('IsSplitTable', data.IsSplitTable);
                    pSql = pIU.GetUpdate('ID = ' + ReadyString(data.JoinedID));
                }
                else if (data.State == 'removed') {
                    pIU.Add('IsSplitTable', 1);
                    pSql = pIU.GetUpdate('ID = ' + ReadyString(data.JoinedID));
                    //pSql = "DELETE FROM LibraryTableJoined WHERE ID = " + ReadyString(data.JoinedID);
                }

                Sql.BeginTransaction();
                Sql.ExecuteSql(pSql);
                Sql.CommitTransaction();

                return 'saved';
            } catch(e){
                return e.Message;
            }            
        }

        pData.JoinedTables = function () {
            var pSql = "SELECT DISTINCT SetID FROM LibraryTableJoined WHERE FowstID = " + ReadyString(sessionStorage.FowstID) + ' AND IsSplitTable=0';
            var pDTSetTable = Sql.Execute(pSql).Rows;
            var pDTTable;
            var arrJoinedTable = []
            var arrToJoinTables = []

            if (pDTSetTable.length > 0) {
                for (var i = 0; i< pDTSetTable.length; i++){
                    pSql = "SELECT info.*, joined.IsParent, joined.ID as JoinedID, joined.SetID as TableSetID, 'edit' as State " + '\n' +
                        ',joined.IsSplitTable\n' +
                        ' ,(SELECT 1' + '\n' + 
                        '    FROM TransInvoice' + '\n' + 
                        '    JOIN LibraryTableTableInfo ON \'TBL_\' || LibraryTableTableInfo.InternalTableName = TransInvoice.CustomerTableNo' + '\n' + 
                        '    JOIN LibraryTableJoined ON LibraryTableJoined.TableID = LibraryTableTableInfo.ID' + '\n' + 
                        '    WHERE LibraryTableJoined.SetID = joined.SetID' + '\n' + 
                        '    AND IFNULL(TransInvoice.InvoiceNumber, 0) = 0' + '\n' + 
                        '    AND IsSplitTable=0 LIMIT 1' + '\n' +
                        '  ) AS IsOccupied\n' +
                        "   FROM LibraryTableJoined as joined " + '\n' +
                        "       INNER JOIN LibraryTableTableInfo as info " + '\n' +
                        "           ON info.ID = joined.TableID" + '\n' +
                        "   WHERE joined.SetID = " + ReadyString(pDTSetTable[i].SetID) + '\n' + 
                        ' AND IsSplitTable=0';
                    pDTTable = Sql.Execute(pSql).Rows;
                    
                    arrToJoinTables = [];

                    if(pDTTable.length > 0){
                        for (var m = 0; m < pDTTable.length; m++){
                            arrToJoinTables.push(pDTTable[m])
                        }
                    }

                    arrJoinedTable.push(arrToJoinTables)
                }
            }

            return arrJoinedTable;
        };

        pData.GetTableInfoByInternalName = function(vInternalTableName) {
            
            var pSql;
            var pOut;

            if (CStr2(vInternalTableName).toLowerCase().indexOf('tbl_') == 0)
                vInternalTableName = vInternalTableName.substr(4);
            pSql = 'SELECT TableName\n' +
                ' FROM LibraryTableTableInfo\n' +
                ' WHERE InternalTableName = ' + ReadyString(vInternalTableName);
            pOut = Sql.ExecuteScalar(pSql);

            return pOut;
        };

        return pData;
    })
})