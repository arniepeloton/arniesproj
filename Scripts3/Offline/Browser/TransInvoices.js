define([
    '/Scripts3/Offline/Browser/common.js?crc='
], function () {

    OfflineBrowser.TransInvoices = {
        GetData: function (vData) {

            var pFinder = OfflineBrowser.Base().CreateFinder(vData);
            var pFields = pFinder.Fields;
            var pField;
            var pNonZero;
            var pFOWSTID;

            // options:
            // vData.PoMode = 1;
            // vData.DeviceID

            pNonZero = vData.NonZero;
            pFOWSTID = vData.FOWSTID;

            pField = pFields.Add('ID');
            pField.Expression = 'TransInvoice.ID';
            pField.Searchable = false;

            pField = pFields.Add('CustomerID');
            pField.Expression = 'TransInvoice.CustomerID';
            pField.Searchable = false;

            pField = pFields.Add('FOWSTID');
            pField.Expression = 'TransInvoice.FOWSTID';
            pField.Searchable = false;

            pField = pFields.Add('InvoiceNum');
            pField.Expression = 'TransInvoice.InvoiceNumber';

            pField = pFields.Add('CompanyName');
            pField.Expression = " (CASE " +
                "     WHEN LibraryOrganization.IsOrganization = 1 THEN LibraryOrganization.CompanyName " +
                "     WHEN LibraryOrganization.IsOrganization = 0 THEN " +
                "         IFNULL(NULLIF(TRIM(LibraryOrganization.PersonSurname) || ', ',', '), '') || " +
                "         IFNULL(NULLIF(TRIM(LibraryOrganization.PersonFirstName) || ' ',' '), '') || " +
                "         IFNULL(NULLIF(TRIM(LibraryOrganization.PersonMiddleName) || ' ',' '), '') || " +
                "         IFNULL(NULLIF(', ' || TRIM(LibraryOrganization.PersonSuffixName),', '), '') " +
                " END)";

            pField = pFields.Add('InvoiceDate');
            pField.Expression = 'TransInvoice.InvoiceDate';

            pField = pFields.Add('Total');
            pField.Expression = '(SELECT CAST(SUM(SubTotal) as TEXT) FROM TransInvoiceItems WHERE InvoiceID = TransInvoice.ID)';
            pField.Searchable = false;

            pField = pFields.Add('SkuCount');
            pField.Expression = '(SELECT COUNT(ID) FROM TransInvoiceItems WHERE InvoiceID = TransInvoice.ID)';
            pField.Searchable = false;

            //additional by janine sept 5 2014 for sales order taker
            pField = pFields.Add('TotalOrder');
            pField.Expression = '(SELECT CAST(SUM(SubTotal) as TEXT) FROM TransInvoiceItems WHERE OrderedQuantity > 0 AND InvoiceID = TransInvoice.ID)';
            pField.Searchable = false;
           
            pField = pFields.Add('SkuCountOrder');
            pField.Expression = '(SELECT COUNT(ID) FROM TransInvoiceItems WHERE OrderedQuantity > 0 AND InvoiceID = TransInvoice.ID)';
            pField.Searchable = false;
            ///////////////
            pField = pFields.Add('SalesOrderNo');
            pField.Expression = 'IFNULL(SalesOrderNo,0)';

            pField = pFields.Add('UserID');
            pField.Expression = 'TransInvoice.UserID';
            pField.Searchable = false;

            pField = pFields.Add('EmployeeFullName');
            pField.Expression = 'LibraryPeople.Surname || \',\' || LibraryPeople.FirstName';

            pField = pFields.Add('FOWSTName');
            pField.Expression = 'LibraryOrganizationFOWST.Name';

            pField = pFields.Add('DeviceName');
            pField.Expression = 'LibraryDevice.PhoneName';

            pField = pFields.Add('CurrentUserID');
            pField.Expression = "'" + Web.PeopleUserID() + "'";
            pField.Searchable = false;

            pField = pFields.Add('MIN');
            pField.Expression = 'LibraryOrganizationFOWST.FOWSTNo || \'-\' || LibraryDevice.DeviceNo';

            pField = pFields.Add('TableNo');
            pField.Expression = 'COALESCE(LibraryTableTableInfo.TableName, TransInvoice.CustomerTableNo)';

            pField = pFields.Add('DeliveryMethod');
            pField.Expression = 'TransInvoice.DeliveryMethod';

            pField = pFields.Add('OrderMethod');
            pField.Expression = 'TransInvoice.OrderMethod';

            pField = pFields.Add('CustomerDemographicGender');
            pField.Expression = 'TransInvoice.CustomerDemographicGender';

            pField = pFields.Add('CustomerDemographicAge');
            pField.Expression = 'TransInvoice.CustomerDemographicAge';

            pField = pFields.Add('SearchSO');
            pField.Expression = " LibraryOrganizationFOWST.FOWSTNo || \'-\' || LibraryDevice.DeviceNo || \'-\' || IFNULL(SalesOrderNo,0) ";

            pField = pFields.Add('SalesOrderNoRemoveDash');
            pField.Expression = " LibraryOrganizationFOWST.FOWSTNo ||  LibraryDevice.DeviceNo ||  IFNULL(SalesOrderNo,0) ";

            pField = pFields.Add('PurchaserName');
            pField.Expression = 'TransInvoice.PurchaserName';

            pField = pFields.Add('IsDraft');
            pField.Expression = 'TransInvoice.IsDraft';

            pField = pFields.Add('PurchaseOrderNo');
            pField.Expression = 'TransInvoice.PurchaseOrderNo';

            pField = pFields.Add('CustomerFowstID');
            pField.Expression = 'TransInvoice.CustomerFowstID';

            pField = pFields.Add('CustomerFowst');
            pField.Expression = '(SELECT Name FROM LibraryOrganizationFowst WHERE ID = TransInvoice.CustomerFowstID)';

            pField = pFields.Add('CustomerBin');
            pField.Expression = 'IFNULL((SELECT Name FROM LibraryFowstBin WHERE ID = TransInvoice.CustomerBinID), "")';

            pField = pFields.Add('SupplierID');
            pField.Expression = 'tblOrgSupplier.ID';

            pField = pFields.Add('SupplierName');
            pField.Expression = 'tblOrgSupplier.CompanyName';

            pField = pFields.Add('SupplierFowstID');
            pField.Expression = 'TransInvoice.SupplierFowstID';

            pField = pFields.Add('SupplierFowst');
            pField.Expression = 'tblFowstSupplier.Name';

//            pFinder.SqlFrom = 'FROM TransInvoice' +
            //                ' JOIN LibraryOrganization ON TransInvoice.CustomerID = LibraryOrganization.ID';
            pFinder.SqlFrom = ' FROM TransInvoice INNER JOIN' +
                ' LibraryPeople ON TransInvoice.UserID = LibraryPeople.ID INNER JOIN' +
                ' LibraryDevice ON TransInvoice.DeviceID = LibraryDevice.ID INNER JOIN' +
                ' LibraryOrganizationFOWST ON TransInvoice.FowstID = LibraryOrganizationFOWST.ID' +
                ' JOIN LibraryOrganization ON TransInvoice.CustomerID = LibraryOrganization.ID\n' + 
                ' LEFT JOIN LibraryTableTableInfo ON LibraryTableTableInfo.InternalTableName = TransInvoice.CustomerTableNo\n' + 
                ' OR SUBSTR(TransInvoice.CustomerTableNo, 5) = LibraryTableTableInfo.InternalTableName\n' + 
                ' LEFT JOIN LibraryOrganization tblOrgSupplier ON TransInvoice.SupplierID = tblOrgSupplier.ID\n' +
                ' LEFT JOIN LibraryOrganizationFOWST tblFowstSupplier ON TransInvoice.SupplierFowstID = tblFowstSupplier .ID\n';
            pFinder.SqlWhere = ' WHERE TransInvoice.IsCancelled = 0' +
                ' AND EXISTS(SELECT TransInvoiceItems.ID' +
                ' FROM TransInvoiceItems' +
                ' WHERE TransInvoiceItems.InvoiceID = TransInvoice.ID)';
            if (CBool(vData.PoMode)) {
                pFinder.SqlWhere += ' AND IFNULL(TransInvoice.PurchaserName, \'\') <> \'\'';
                if (CBool(vData.IsFinalizeOnly)) {
                    pFinder.SqlWhere += ' AND TransInvoice.IsDraft = 0'
                    if (CBool(vData.IsFilterReceived)) {
                        pFinder.SqlWhere += ' AND TransInvoice.ReceivingID IS NULL'
                    }
                }
            }                
            else
                pFinder.SqlWhere += ' AND TransInvoice.InvoiceNumber ' + (CBool(pNonZero) ? '<>' : '=') + ' 0\n' +
                    ' AND IFNULL(TransInvoice.PurchaserName, \'\') = \'\'\n'; 
            
            if (pFOWSTID != undefined) {
                pFinder.SqlWhere += ' AND TransInvoice.FOWSTID = ' + ReadyString(pFOWSTID);
            }

            if (vData.DeviceID != undefined) 
                pFinder.SqlWhere += ' AND TransInvoice.DeviceID = ' + ReadyString(vData.DeviceID);
            
            if (vData.RestrictedTransactionLookupForSalesReturn) {
                pFinder.SqlWhere += ' AND CAST(TransInvoice.AmountDueNetofCharges - TransInvoice.DiscountAmount AS INTEGER) = ' + Math.trunc(CDec2(vData.InvoiceAmount)) + '\n';
            }

            pFinder.SqlOrder = ' ORDER BY TransInvoice.LastUpdated DESC';

            return pFinder.Requery();

        }, // Show

        GetInvoiceDetails: function (vData) {
            var pFinder = OfflineBrowser.Base().CreateFinder(vData);
            var pFields = pFinder.Fields;
            var pField;
            var pInvoiceID;

            pInvoiceID = vData.InvoiceID;

            pField = pFields.Add('FOWSTID');
            pField.Expression = 'LibraryOrganizationFOWST.ID';
            pField.Searchable = false;

            pField = pFields.Add('FOWST');
            pField.Expression = 'LibraryOrganizationFOWST.Name';

            pField = pFields.Add('InvoiceID');
            pField.Expression = 'TransInvoice.ID';

            pField = pFields.Add('InvoiceNo');
            pField.Expression = 'TransInvoice.InvoiceNumber';

            pField = pFields.Add('InvoiceDate');
            pField.Expression = 'TransInvoice.InvoiceDate';

            pField = pFields.Add('ProductDescription');
            pField.Expression = 'IFNULL(LibraryPIMProduct.ProductGroupNo,\'\') || \' - \' || IFNULL(LibraryPIMProduct.ProductNo,\'\') || \' - \' || IFNULL(LibraryPIMProduct.ProductName,\'\')';

            pField = pFields.Add('Qty');
            pField.Expression = 'TransInvoiceItems.Quantity';

            pField = pFields.Add('UnitMeasure');
            pField.Expression = 'TransInvoiceItems.UnitMeasure';

            pField = pFields.Add('Price');
            pField.Expression = 'TransInvoiceItems.UnitPrice';

            pField = pFields.Add('SubTotal');
            pField.Expression = 'TransInvoiceItems.SubTotal';

            pField = pFields.Add('Discount');
            pField.Expression = 'TransInvoiceItems.DiscountAmount';

            //pField = pFields.Add('SalesRepresentative');
            //pField.Expression = '(SELECT DISTINCT STUFF((SELECT DISTINCT \',\'+ IFNULL(CASE WHEN LibraryPeople.SurName = \'\' THEN \'N/A\' ' +
            //                    'ELSE LibraryPeople.FirstName + \' \' + LibraryPeople.SurName END,\'N/A\') ' +
            //                    'FROM TransInvoiceItemSalesReps LEFT join LibraryPeople on LibraryPeople.ID = TransInvoiceItemSalesReps.PeopleID ' +
            //                    'WHERE TransInvoiceItemSalesReps.ProductID=TransInvoiceItems.ProductID and TransInvoiceItemSalesReps.InvoiceID =TransInvoice.ID ' +
            //                    'FROM TransInvoiceItemSalesReps)';
            //FOR XML PATH(\'\')), 1, 1,\'\')
            pField = pFields.Add('Customer');
            pField.Expression = '(CASE WHEN LibraryOrganization.CompanyName  = \'\' THEN LibraryOrganization.PersonSurname || \', \' || LibraryOrganization.PersonFirstName ELSE LibraryOrganization.CompanyName END)';

            pField = pFields.Add('RebatesID');
            pField.Expression = 'TransInvoiceItemsRebates.ProductID';

            pField = pFields.Add('RebatesProduct');
            pField.Expression = 'IFNULL(p1.ProductGroupNo,\'\') || \' - \' || IFNULL(p1.ProductNo,\'\') || \' - \' || IFNULL(p1.ProductName,\'\')';

            pField = pFields.Add('QtyRebates');
            pField.Expression = 'TransInvoiceItemsRebates.Quantity';

            pField = pFields.Add('UnitMeasureRebates');
            pField.Expression = 'TransInvoiceItemsRebates.UnitMeasure';

            pField = pFields.Add('UnitMeasureRebates');
            pField.Expression = 'TransInvoiceItemsRebates.UnitMeasure';

            pField = pFields.Add('UpdatedByName');
            pField.Expression = 'IFNULL(NULLIF(LTRIM(RTRIM(tblUpdaterSuffix.Code)) || \' \', \' \'), \'\') || ' +
                                'IFNULL(NULLIF(LTRIM(RTRIM(tblUpdater.Surname)) || \', \',\', \'), \'\') || ' +
                                'IFNULL(NULLIF(LTRIM(RTRIM(tblUpdater.FirstName)) || \' \',\' \'), \'\') || ' +
                                'IFNULL(NULLIF(LTRIM(RTRIM(tblUpdater.MiddleName)) || \' \',\' \'), \'\') || ' +
                                'IFNULL(NULLIF(\', \' || LTRIM(RTRIM(tblUpdater.PersonalSuffixName)),\', \'), \'\')';

            pFinder.SqlFrom = '  FROM TransInvoiceItems INNER JOIN ' +
                                'LibraryPIMProduct ON TransInvoiceItems.ProductID = LibraryPIMProduct.ID INNER JOIN  ' +
                                'TransInvoice ON TransInvoiceItems.InvoiceID = TransInvoice.ID INNER JOIN  ' +
                                'LibraryOrganizationFOWST ON TransInvoice.FowstID = LibraryOrganizationFOWST.ID  ' +
                                'LEFT join TransInvoiceItemSalesReps on transinvoiceitems.InvoiceID = TransInvoiceItemSalesReps.InvoiceID  ' +
                                'AND transinvoiceitems.ProductID = TransInvoiceItemSalesReps.ProductID  ' +
                                'LEFT join LibraryPeople on LibraryPeople.ID = TransInvoiceItemSalesReps.PeopleID  ' +
                                'LEFT join LibraryOrganization on LibraryOrganization.ID = TransInvoice.CustomerID  ' +
                                'left join TransInvoiceItemsRebates on TransInvoiceItemsRebates.ModifierGroupID = TransInvoiceItems.ModifierGroupID  ' +
                                'LEFT JOIN LibraryPIMProduct p1 ON TransInvoiceItemsRebates.ProductID = p1.ID  ' +
                                'LEFT JOIN LibraryPeople tblUpdater ON tblUpdater.ID = TransInvoice.UserID ' +
                                'LEFT JOIN LibraryPeoplePreNominalSuffix tblUpdaterSuffix ON tblUpdater.TitleID = tblUpdaterSuffix.ID  ' 

            pFinder.SqlWhere = 'WHERE TransInvoiceItems.InvoiceID = ' + ReadyString(pInvoiceID)

            return pFinder.Requery();
        }
    }; // OfflineBrowser.TransInvoice

});