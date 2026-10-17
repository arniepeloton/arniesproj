define([
    '/Scripts3/Offline/Browser/common.js?crc='
], function () {

    OfflineBrowser.LibraryPIMProduct = {
        GetData: function (vData) {

            var pFinder = OfflineBrowser.Base().CreateFinder(vData);
            var pFields = pFinder.Fields;
            var pField;
            var pWhere;
            var pHasFastTable;
            var pSearchString;
            var pMainTable;

            if (OfflineBrowser.LibraryPIMProduct.PriceLevelID == null)
                OfflineBrowser.LibraryPIMProduct.PriceLevelID = Business.SystemDefaults.GetRetailPriceID();

            function HasFastTable() {
                
                try {
                    var pSql = 'SELECT * FROM LibraryPIMProductFast LIMIT 0';
                    Sql.Execute(pSql);
                    return true;
                } catch (e) {
                    return false;
                }
            }
            pHasFastTable = HasFastTable();
            if (pHasFastTable)
                pMainTable = 'LibraryPIMProductFast';
            else
                pMainTable = 'LibraryPIMProduct';

            pFinder.Distinct = true;
            pField = pFields.Add('ID');
            if (!pHasFastTable)
                pField.Expression = 'LibraryPIMProduct.ID';
            pField.Searchable = false;

            pFields.Add('ProductNo');
            pFields.Add('ProductName');
            pFields.Add('ProductGroupNo');

            pField = pFields.Add('ProductSearchString');
            if (!pHasFastTable)
                pField.Expression = "IFNULL(LibraryPIMProduct.ProductSearchString, '')";

            pField = pFields.Add('PublishedBy');
            if (!pHasFastTable)
                pField.Expression = 'pub.CompanyName';

            pField = pFields.Add('UploadedBy');
            if (!pHasFastTable)
                pField.Expression = 'upl.CompanyName';
            pField.Searchable = false;

            if (pHasFastTable) {
                pField = pFields.Add('ReadyBarcode');
                pField.ExcludeFromSelect = true;
            }

            pField = pFields.Add('CategoryName');
            if (pHasFastTable) {
                pField.Expression = '(SELECT DISTINCT LibraryItemModifierCategories.CategoryName \n' +
                    ' FROM LibraryPIMProductVariableFeatures001 \n ' +
                    ' JOIN private.LibraryItemModifierCategories ON LibraryItemModifierCategories.CategoryName = LibraryPIMProductVariableFeatures001.C0020 \n ' +
                    ' WHERE LibraryPIMProductVariableFeatures001.ProductID = LibraryPIMProductFast.ID )';
            } else {
                pField.Expression = '(SELECT DISTINCT LibraryItemModifierCategories.CategoryName \n' +
                    ' FROM LibraryPIMProductVariableFeatures001 \n ' +
                    ' JOIN private.LibraryItemModifierCategories ON LibraryItemModifierCategories.CategoryName = LibraryPIMProductVariableFeatures001.C0020 \n ' +
                    ' WHERE LibraryPIMProductVariableFeatures001.ProductID = LibraryPIMProduct.ID )';
            }
            pField.Searchable = false;

            if ((IsGuid(OfflineBrowser.LibraryPIMProduct.PriceLevelID))
                && (IsGuid(sessionStorage.OrgID))) {

                pField = pFields.Add('DefaultPrice');
                pField.Expression = '(SELECT Price\n' +
                    ' FROM LibraryPIMProductPriceLevel\n' +
                    ' WHERE PriceLevelID =' + ReadyString(OfflineBrowser.LibraryPIMProduct.PriceLevelID) + '\n' +
                    ' AND ProductID = ' + pMainTable + '.ID\n' + 
                    ' AND OrganizationID = ' + ReadyString(ReadyGuid(sessionStorage.OrgID)) + '\n' +
                    ' ORDER BY LibraryPIMProductPriceLevel.LastUpdated DESC\n' +
                    ' LIMIT 1\n' +
                    ')';
                pField.Searchable = false;
            }

            if (!pHasFastTable) {
                pWhere = " WHERE LibraryPIMProduct.IsActive = 1 AND orgpim.IsDeactivated = 0\n";

                if (vData.ProductType == 'raw') {
                    pWhere += ' AND LibraryPIMProduct.IsRawMaterials = 1 \n';
                }                

                if (CStr2(vData.keywords).trim() != '') {
                    pWhere += ' AND (' + pFinder.CreateWhere() + ') OR \n' +
                        ' LibraryPIMProduct.ID IN \n' +
                        '   (SELECT LibraryPIMProductMeasurement.ProductID\n' +
                        '   FROM LibraryPIMProductMeasurementBarcode\n' +
                        '   JOIN LibraryPIMProductMeasurement ON LibraryPIMProductMeasurement.ID = LibraryPIMProductMeasurementBarcode.StockMeasurementsID\n' +
                        '   WHERE LibraryPIMProductMeasurementBarcode.ReadyBarcode = ' + ReadyString(ReadyBarcode(vData.keywords)) + '\n' + 
                        ")\n";
                };
            }
            pFinder.SqlWhereExact = pWhere;
            if (pHasFastTable) {
                pFinder.SqlFrom = 'FROM LibraryPIMProductFast\n';
                //pFinder.SqlOrder = 'ORDER BY LibraryPIMProductFast.IsBrandOwner DESC\n' +
                //    ', LibraryPIMProductFast.ProductName\n';
            } else {
                pFinder.SqlFrom = 'FROM LibraryPIMProduct\n' +
                    ' JOIN LibraryOrganizationPIM orgpim ON LibraryPIMProduct.ID = orgpim.ProductID\n' +
                    ' LEFT JOIN LibraryOrganization pub ON LibraryPIMProduct.PublishedByID = pub.ID\n' +
                    ' LEFT JOIN LibraryOrganization upl ON LibraryPIMProduct.UploadedByID = upl.ID\n';
                if (vData.WithContentsOnly) {
                    pFinder.SqlFrom += ' JOIN LibraryPIMProductContents ON LibraryPIMProductContents.ParentProductID = LibraryPIMProduct.ID\n' +
                        ' AND LibraryPIMProductContents.OrganizationID = ' + ReadyString(Web.OrgID()) + '\n';
                }
                //pFinder.SqlOrder = 'ORDER BY LibraryPIMProduct.IsBrandOwner DESC\n' +
                //    ', LibraryPIMProduct.ProductName\n';
            }

            var pSql;
            var pDT;
            var pSearchOnlyForBarcode = Business.SystemDefaults.SearchOnlyForBarcode();

            if (pHasFastTable && pSearchOnlyForBarcode) 
                pFinder.SqlWhereExact = ' WHERE LibraryPIMProductFast.ReadyBarcode = ' + ReadyString(pFinder.SearchString);
            if (pHasFastTable) {
                if ((vData.ProductType == 'raw') 
                    || (CBool(vData.IsInventoryCountItem))) {
                    pFinder.MaxRecords = 0;
                }
            }
            pSql = pFinder.CreateSql();

            if (pHasFastTable) {
                if (vData.ProductType == 'raw') {
                    pSql = 'SELECT T.* FROM (' + pSql + ') as T INNER JOIN LibraryOrganizationPIM ON LibraryOrganizationPIM.ProductID = T.ID WHERE LibraryOrganizationPIM.IsRawMaterials = 1';
                } else if (CBool(vData.IsInventoryCountItem)) {
                    pSql = 'SELECT T.* FROM (' + pSql + ') as T INNER JOIN LibraryOrganizationPIM ON LibraryOrganizationPIM.ProductID = T.ID WHERE LibraryOrganizationPIM.IsInventoryCountItem = 1';
                }                
            }

            pDT = Sql.Execute(pSql);

            return pDT; // pFinder.Requery();

        } // Show
    }; // OfflineBrowser.LibraryOrganization

});