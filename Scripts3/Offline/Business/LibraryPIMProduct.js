define([], function () {
    window.Business.LibraryPIMProduct = new function (vParent) {
        this.ByIDs = function (vIDs) {

            var pDT;
            var pDR;
            var pSql;

            pSql = 'SELECT LibraryPIMProduct.ID' +
                ',LibraryPIMProduct.ProductNo' +
                ',LibraryPIMProduct.ProductName';
            pSql += ' FROM LibraryPIMProduct';
            pSql += ' WHERE LibraryPIMProduct.ID IN (' + vIDs + ')';
            pDT = Sql.Execute(pSql);

            return pDT;
        }; // ByIDs

        this.PriceTable = function (vID,
            vCustomerID, // optional
            vFranchiseGroupID,
            vReturnString, // optional. if true, this function will return a string. If false, this function will return a datatable
            vNoOrderBy,
            vDontPoMode
        ) {

            var pDT;
            var pSql;
            var pSqlCache;
            var pDrBase;
            var pSaveToCache = false; // bool
            var pArgs;

            if (proj.POMode) {
                pDT = VendorPriceTable.call(this, vID);
                return pDT;
            }

            if ((arguments.length > 0)
                && (typeof vID == 'object')) {

                pArgs = vID;
                vCustomerID = vID.CustomerID;
                vFranchiseGroupID = vID.FranchiseGroupID;
                vReturnString = vID.ReturnString;
                vNoOrderBy = vID.NoOrderBy;
                vDontPoMode = vID.DontPoMode;
                pSaveToCache = CBool(vID.SaveToCache);
                vID = vID.ID; // set this last
            }
            if ((CStr2(vCustomerID).trim() != '')
                && (CStr2(vFranchiseGroupID).trim() != '')) {

                pSql = this.PriceTable({
                    ID: vID,
                    CustomerID: vCustomerID,
                    ReturnString: true,
                    NoOrderBy: true
                });

                pSql += '\n UNION ALL \n'; // if you omit ALL, android sqlite will cause an unrelated error (column 'LibraryPIMProductPriceLevel.PriceLevelID' does not exist)

                pSql += this.PriceTable({
                    ID: vID,
                    FranchiseGroupID: vFranchiseGroupID,
                    ReturnString: true,
                    NoOrderBy: true
                });

                pSql +=  ' ORDER BY Quantity\n' + 
                    ',PriceLevel' + '\n' +
                    ',MeasurementLevel';

                pSql = 'SELECT DISTINCT * FROM \n(' + pSql + '\n)';
                pDT = Sql.Execute(pSql);
            
                return pDT;
            }

            pDrBase = this.BaseUnit(vID);

            //if purchase order, overwrite the customer to supplier id instead

            pSql = 'SELECT DISTINCT ' + '\n' +
                '  LibraryPIMProductPriceLevel.PriceLevelID' + '\n' +
                ' ,LibraryPIMProductMeasurement.MeasurementLevel' + '\n' +
                ',LibraryCountriesLanguageLabel.Label AS UnitMeasure' + '\n' +
                ' ,LibraryPIMProductMeasurement.Quantity' + '\n' +
                ' ,LibraryPIMPriceLevel.PriceLevel' + '\n' +
                ' ,(NOT LibraryPIMProductPriceLevel.DeactivateDiscount) * LibraryPIMProductMeasurement.Discount AS Discount' + '\n' +
                ' ,CAST(LibraryPIMProductMeasurement.Quantity * LibraryPIMProductPriceLevel.Price AS Decimal) AS OriginalPrice' + '\n' +
                ' ,LibraryPIMProductMeasurement.Quantity * LibraryPIMProductPriceLevel.Price - (LibraryPIMProductMeasurement.Quantity * LibraryPIMProductPriceLevel.Price) * (CAST(LibraryPIMProductMeasurement.Discount AS Float) / 100) * (NOT LibraryPIMProductPriceLevel.DeactivateDiscount) AS DiscountedPrice' + '\n' +
                ' ,LibraryPIMProductPriceLevel.Price - LibraryPIMProductPriceLevel.Price * (CAST(LibraryPIMProductMeasurement.Discount AS Float) / 100) * (NOT LibraryPIMProductPriceLevel.DeactivateDiscount) AS DiscountedPricePerUnit' + '\n' +
                ' ,LibraryPIMProductPriceLevel.LastUpdated' + '\n' +
                ' ,LibraryPIMProductMeasurement.Discount AS MeasurementDiscount' + '\n' +
                ' ,LibraryPIMProductPriceLevel.DeactivateDiscount' + '\n' +
                ',' + ReadyString(pDrBase.UnitMeasure) + ' AS BaseUnit' + '\n' +
                ',' + ReadyString(pDrBase.UnitQty) + ' AS BaseUnitQty, LibraryPIMProductMeasurement.UnitMeasureID' + '\n';

            if (CStr2(vCustomerID).trim() != '')
                pSql += ' ,LibraryOrganizationPriceLevel.OrganizationID AS OrganizationPriceLevelOrganizationID\n';
            else if (CStr2(vFranchiseGroupID).trim() != '')
                pSql += ' ,LibraryFranchiseGroupPriceLevels.OrgID AS OrganizationPriceLevelOrganizationID\n';

            pSql += ' ,LibraryPIMProductPriceLevel.OrganizationID AS ProductPriceLevelOrganizationID\n' +
                ' ,LibraryPIMProductMeasurement.ProductID\n' +
                ' FROM LibraryPIMProductMeasurement' + '\n' +
                ' JOIN LibraryPIMProductPriceLevel ON LibraryPIMProductMeasurement.ProductID = LibraryPIMProductPriceLevel.ProductID' + '\n' +
                ' JOIN LibraryPIMPriceLevel ON LibraryPIMProductPriceLevel.PriceLevelID = LibraryPIMPriceLevel.ID  ' + '\n' +
                ' AND (\n' + 
                '    LibraryPIMPriceLevel.FromDate IS NULL \n' +
                '    OR ' + ReadyDate(proj.GetZReadDate()) + ' BETWEEN LibraryPIMPriceLevel.FromDate AND LibraryPIMPriceLevel.ToDate' + '\n' +
                ')\n' +
                ' JOIN LibraryPIMMeasure ON LibraryPIMMeasure.ID = LibraryPIMProductMeasurement.UnitMeasureID ' + '\n' +
                ' JOIN LibraryCountriesLanguageLabel ON LibraryCountriesLanguageLabel.ID = LibraryPIMMeasure.MeasureNameID' + '\n';
            pSqlCache = 'SELECT DISTINCT  ' + '\n' +
                '  PriceLevelID' + '\n' +
                ' ,MeasurementLevel' + '\n' +
                ' ,UnitMeasure' + '\n' +
                ' ,Quantity' + '\n' +
                ' ,PriceLevel' + '\n' +
                ' ,Discount' + '\n' +
                ' ,OriginalPrice' + '\n' +
                ' ,DiscountedPrice' + '\n' +
                ' ,DiscountedPricePerUnit' + '\n' +
                ' ,LastUpdated' + '\n' +
                ' ,MeasurementDiscount' + '\n' +
                ' ,DeactivateDiscount' + '\n' +
                ' ,BaseUnit' + '\n' +
                ' ,BaseUnitQty' + '\n' +
                ' ,UnitMeasureID' + '\n' +
                ' ,OrganizationPriceLevelOrganizationID\n' + 
                ' ,ProductPriceLevelOrganizationID\n' +
                ' ,ProductID\n' +
                ' FROM private.CachePriceTablePos' + '\n';

            if (!CBool(vDontPoMode) && proj.POMode) {
                if (CStr2(sessionStorage.SupplierPriceLevelID) != '' && CStr2(sessionStorage.SupplierOrganizationID) != '') {
                    
                    pSql += ' WHERE LibraryPIMProductMeasurement.ProductID = ' + ReadyString(vID) + '\n' +
                        ' AND LibraryPIMProductPriceLevel.PriceLevelID = ' + ReadyString(sessionStorage.SupplierPriceLevelID) + '\n' +
                        ' AND LibraryPIMPriceLevel.IsActive = 1' + '\n' +
                        ' AND LibraryPIMProductPriceLevel.OrganizationID = ' + ReadyString(sessionStorage.SupplierOrganizationID) + '\n';
                    pSqlCache += ' WHERE ProductID = ' + ReadyString(vID) + '\n' +
                        ' AND PriceLevelID = ' + ReadyString(sessionStorage.SupplierPriceLevelID) + '\n' +
                        ' AND ProductPriceLevelOrganizationID = ' + ReadyString(sessionStorage.SupplierOrganizationID) + '\n';
                } else {
                    pSql += ' WHERE LibraryPIMProductMeasurement.ProductID = ' + ReadyString(vID) + '\n' +
                        ' AND LibraryPIMProductPriceLevel.PriceLevelID <> ' + ReadyGuid('01000012-0001-0000-0000-000000000000') + '\n' +
                        ' AND LibraryPIMProductPriceLevel.PriceLevelID <> ' + ReadyGuid('01000012-0002-0000-0000-000000000000') + '\n' +
                        ' AND LibraryPIMProductPriceLevel.PriceLevelID <> ' + ReadyGuid('01000012-0003-0000-0000-000000000000') + '\n' +
                        ' AND LibraryPIMPriceLevel.IsActive = 1' + '\n' +
                        ' AND LibraryPIMProductPriceLevel.OrganizationID = ' + ReadyString(sessionStorage.SupplierOrganizationID) + '\n';
                    pSqlCache += ' WHERE ProductID = ' + ReadyString(vID) + '\n' +
                        ' AND PriceLevelID <> ' + ReadyGuid('01000012-0001-0000-0000-000000000000') + '\n' +
                        ' AND PriceLevelID <> ' + ReadyGuid('01000012-0002-0000-0000-000000000000') + '\n' +
                        ' AND PriceLevelID <> ' + ReadyGuid('01000012-0003-0000-0000-000000000000') + '\n' +
                        ' AND ProductPriceLevelOrganizationID = ' + ReadyString(sessionStorage.SupplierOrganizationID) + '\n';
                }

            } else {
                if (CStr2(vCustomerID).trim() != '')
                    pSql += ' JOIN LibraryOrganizationPriceLevel ON LibraryOrganizationPriceLevel.PriceLevelID = LibraryPIMPriceLevel.ID' + '\n' +
                        ' AND LibraryOrganizationPriceLevel.OrganizationID = ' + ReadyString(vCustomerID) + '\n';
                else if (CStr2(vFranchiseGroupID).trim() != '')
                    pSql += ' JOIN LibraryFranchiseGroupPriceLevels ON LibraryFranchiseGroupPriceLevels.PriceLevelID = LibraryPIMPriceLevel.ID' + '\n' +
                        ' AND LibraryFranchiseGroupPriceLevels.GroupID = ' + ReadyString(vFranchiseGroupID) + '\n';
                        // dont the following condition
                        // ' AND LibraryFranchiseGroupPriceLevels.IsActive = 1';

                pSql += ' WHERE LibraryPIMProductMeasurement.ProductID = ' + ReadyString(vID) + '\n' +
                    ' AND LibraryPIMProductPriceLevel.PriceLevelID <> ' + ReadyGuid('01000012-0001-0000-0000-000000000000') + '\n' +
                    ' AND LibraryPIMProductPriceLevel.PriceLevelID <> ' + ReadyGuid('01000012-0002-0000-0000-000000000000') + '\n' +
                    ' AND LibraryPIMProductPriceLevel.PriceLevelID <> ' + ReadyGuid('01000012-0003-0000-0000-000000000000') + '\n' +
                    ' AND LibraryPIMProductPriceLevel.OrganizationID = ' + ReadyString(Web.OrgID()) + '\n';

                pSqlCache += ' WHERE ProductID = ' + ReadyString(vID) + '\n' +
                    ' AND PriceLevelID <> ' + ReadyGuid('01000012-0001-0000-0000-000000000000') + '\n' +
                    ' AND PriceLevelID <> ' + ReadyGuid('01000012-0002-0000-0000-000000000000') + '\n' +
                    ' AND PriceLevelID <> ' + ReadyGuid('01000012-0003-0000-0000-000000000000') + '\n' +
                    ' AND ProductPriceLevelOrganizationID = ' + ReadyString(Web.OrgID()) + '\n';
                if (CStr2(vCustomerID).trim() != '')
                    pSqlCache += ' AND OrganizationPriceLevelOrganizationID = ' + ReadyString(vCustomerID) + '\n';

                if ((CStr2(vCustomerID).trim() == '')
                    && (CStr2(vFranchiseGroupID).trim() == ''))
                    pSql += ' AND LibraryPIMPriceLevel.IsActive = 1' + '\n';
            }

            if (!vNoOrderBy) {
                pSql +=  ' ORDER BY LibraryPIMProductMeasurement.Quantity\n' + 
                    ',LibraryPIMPriceLevel.PriceLevel' + '\n' +
                    ',LibraryPIMProductMeasurement.MeasurementLevel';

                pSqlCache +=  ' ORDER BY Quantity\n' + 
                    ',PriceLevel' + '\n' +
                    ',MeasurementLevel';
            }

            if (vReturnString)
                return pSql;
            else {
                if (pSaveToCache
                    || (pArgs == null)) {

                    pDT = Sql.Execute(pSql);
                    if (pArgs == null)
                        return pDT;

                    if (pSaveToCache) {
                        if (Sql.TableExists('CachePriceTablePos', 'private')) {
                            var pIU = new InsertUpdate('private.CachePriceTablePos');
                            pIU.Add('PriceLevelID', 'PriceLevelID');
                            pIU.Add('MeasurementLevel', 'MeasurementLevel');
                            pIU.Add('UnitMeasure', 'UnitMeasure');
                            pIU.Add('Quantity', 'Quantity');
                            pIU.Add('PriceLevel', 'PriceLevel');
                            pIU.Add('Discount', 'Discount');
                            pIU.Add('OriginalPrice', 'OriginalPrice');
                            pIU.Add('DiscountedPrice', 'DiscountedPrice');
                            pIU.Add('DiscountedPricePerUnit', 'DiscountedPricePerUnit');
                            pIU.Add('LastUpdated', 'LastUpdated');
                            pIU.Add('MeasurementDiscount', 'MeasurementDiscount');
                            pIU.Add('DeactivateDiscount', 'DeactivateDiscount');
                            pIU.Add('BaseUnit', 'BaseUnit');
                            pIU.Add('BaseUnitQty', 'BaseUnitQty');
                            pIU.Add('UnitMeasureID', 'UnitMeasureID');
                            pIU.Add('OrganizationPriceLevelOrganizationID', 'OrganizationPriceLevelOrganizationID');
                            pIU.Add('ProductPriceLevelOrganizationID', 'ProductPriceLevelOrganizationID');
                            pIU.Add('ProductID', 'ProductID');
                            pIU.FromSql = ' FROM (' + pSql + ') t';
                            pSql = pIU.GetInsert(true);
                            Sql.ExecuteSql(pSql);
                            pArgs.SaveToCache = false;
                        } else {
                            pSql = 'CREATE TABLE private.CachePriceTablePos AS\n' + pSql;
                            Sql.ExecuteSql(pSql);

                            pSql = "SELECT sql FROM private.sqlite_master WHERE type='table' AND tbl_name='CachePriceTablePos'";
                            pSql = Sql.ExecuteScalar(pSql);

                            pSql = pSql.replace(/,/g, ' COLLATE NOCASE, ');
                            pSql = pSql.replace(/\)/g, ' COLLATE NOCASE )');
                            Sql.ExecuteSql('DROP TABLE private.CachePriceTablePos');
                            pSql = pSql.replace('CREATE TABLE CachePriceTablePos(', 'CREATE TABLE private.CachePriceTablePos(');
                            Sql.ExecuteSql(pSql);

                            pArgs.SaveToCache = true;
                        }
                        pDT = Business.LibraryPIMProduct.PriceTable(pArgs);
                        return pDT;
                    }
                } else {
                    if (HandleTableExistCachePriceTablePos()) {
                        pDT = Sql.Execute(pSqlCache);
                        if (pDT.Rows.length > 0)
                            return pDT;
                    }
                    pArgs.SaveToCache = true;
                    pDT = Business.LibraryPIMProduct.PriceTable(pArgs);
                    return pDT;
                }
            }

        }; // PriceTable

        function VendorPriceTable(vProductID) {
            // this must be called like VendorPriceTable.call(this, vID);

            var pDrBase = this.BaseUnit(vProductID);
            
            var pSql = ' WITH CTE AS (' + '\n' + 
                ' SELECT DISTINCT ' + '\n' + 
                '   LibraryPIMProductPriceLevel.PriceLevelID' + '\n' + 
                '  ,LibraryPIMPriceLevel.PriceLevel' + '\n' + 
                '  ,LibraryPIMProductPriceLevel.Price' + '\n' + 
                '  ,LibraryPIMProductPriceLevel.LastUpdated' + '\n' + 
                '  ,LibraryPIMProductPriceLevel.OrganizationID AS ProductPriceLevelOrganizationID' + '\n' + 
                '  FROM LibraryPIMProductPriceLevel ' + '\n' + 
                '  JOIN LibraryPIMPriceLevel ON LibraryPIMProductPriceLevel.PriceLevelID = LibraryPIMPriceLevel.ID  ' + '\n' + 
                '  WHERE LibraryPIMProductPriceLevel.ProductID = ' + ReadyString(vProductID) + '\n' + 
                '  AND LibraryPIMProductPriceLevel.PriceLevelID = \'01000012-0003-0000-0000-000000000000\'' + '\n' + 
                ' -- AND LibraryPIMProductPriceLevel.OrganizationID = ' + ReadyString(Web.OrgID()) + '\n' + 
                '  ORDER BY LibraryPIMPriceLevel.PriceLevel' + '\n' + 
                ' )' + '\n' + 
                ' SELECT DISTINCT ' + '\n' + 
                '  LibraryPIMProductMeasurement.MeasurementLevel' + '\n' + 
                ' ,LibraryCountriesLanguageLabel.Label AS UnitMeasure' + '\n' + 
                '  ,LibraryPIMProductMeasurement.Quantity' + '\n' + 
                ' ,\'PC\' AS BaseUnit' + '\n' + 
                ' ,\'1\' AS BaseUnitQty' + '\n' + 
                ' , LibraryPIMProductMeasurement.UnitMeasureID' + '\n' + 
                '  ,LibraryPIMProductMeasurement.ProductID' + '\n' + 
                ' ,CTE.Price * LibraryPIMProductMeasurement.Quantity AS OriginalPrice' + '\n' + 
                ' ,CTE.Price * LibraryPIMProductMeasurement.Quantity AS DiscountedPrice' + '\n' + 
                ' ,CTE.Price AS DiscountedPricePerUnit' + '\n' + 
                ' ,CTE.PriceLevelID' + '\n' + 
                ' ,CTE.PriceLevel' + '\n' + 
                ' ,CTE.LastUpdated' + '\n' + 
                ' ,0 AS Discount' + '\n' + 
                ' ,0 AS MeasurementDiscount' + '\n' + 
                ' ,0 AS DeactivateDiscount' + '\n' + 
                ',' + ReadyString(pDrBase.UnitMeasure) + ' AS BaseUnit' + '\n' +
                ',' + ReadyString(pDrBase.UnitQty) + ' AS BaseUnitQty' + '\n' +
                '  FROM LibraryPIMProductMeasurement' + '\n' + 
                '  JOIN LibraryPIMMeasure ON LibraryPIMMeasure.ID = LibraryPIMProductMeasurement.UnitMeasureID ' + '\n' + 
                '  JOIN LibraryCountriesLanguageLabel ON LibraryCountriesLanguageLabel.ID = LibraryPIMMeasure.MeasureNameID' + '\n' + 
                ' JOIN CTE ' + '\n' + 
                '  WHERE LibraryPIMProductMeasurement.ProductID = ' + ReadyString(vProductID) + '\n' + 
                '  ORDER BY LibraryPIMProductMeasurement.Quantity' + '\n' + 
                ' ,LibraryPIMProductMeasurement.MeasurementLevel'
            var pDT = Sql.Execute(pSql);

            return pDT;

        } // END function VendorPriceTable(vProductID, vVendorID) {

        function HandleTableExistCachePriceTablePos() {
            if (!Sql.TableExists('CachePriceTablePos', 'private'))
                return false;

            // return true; // uncomment this after 1 year. 2021_02_26__12_08_22_0144

            if (CBool(localStorage.DontHandleTableExistCachePriceTablePos))
                return true;

            var pSql = "SELECT sql FROM private.sqlite_master WHERE type='table' AND tbl_name='CachePriceTablePos'";
            pSql = Sql.ExecuteScalar(pSql);

            if (pSql.toUpperCase().indexOf('COLLATE NOCASE') == -1) {
                Sql.ExecuteSql('DROP TABLE private.CachePriceTablePos');
                return false;
            }
             
            localStorage.DontHandleTableExistCachePriceTablePos = 1;
            return true;
        } // HandleTableExistCachePriceTablePos

        //additional by janine. for data integrity test look at this function
        this.PriceTableForTEST = function (vID,
            vCustomerID // optional
        ) {

            var pDT;
            var pSql;
            var pDrBase;

            pDrBase = this.BaseUnit(vID);

            pSql = 'SELECT DISTINCT  ' + '\n' +
                '  LibraryPIMProductPriceLevel.PriceLevelID' + '\n' +
                ' ,LibraryPIMProductMeasurement.MeasurementLevel' + '\n' +
                ',LibraryCountriesLanguageLabel.Label AS UnitMeasure' + '\n' +
                ' ,LibraryPIMProductMeasurement.Quantity' + '\n' +
                ' ,LibraryPIMPriceLevel.PriceLevel' + '\n' +
                ' ,(NOT LibraryPIMProductPriceLevel.DeactivateDiscount) * LibraryPIMProductMeasurement.Discount AS Discount' + '\n' +
                ' ,CAST(LibraryPIMProductMeasurement.Quantity * LibraryPIMProductPriceLevel.Price AS Decimal) AS OriginalPrice' + '\n' +
                ' ,LibraryPIMProductMeasurement.Quantity * LibraryPIMProductPriceLevel.Price - (LibraryPIMProductMeasurement.Quantity * LibraryPIMProductPriceLevel.Price) * (CAST(LibraryPIMProductMeasurement.Discount AS Float) / 100) * (NOT LibraryPIMProductPriceLevel.DeactivateDiscount) AS DiscountedPrice' + '\n' +
                ' ,LibraryPIMProductPriceLevel.Price - LibraryPIMProductPriceLevel.Price * (CAST(LibraryPIMProductMeasurement.Discount AS Float) / 100) * (NOT LibraryPIMProductPriceLevel.DeactivateDiscount) AS DiscountedPricePerUnit' + '\n' +
                ' ,LibraryPIMProductPriceLevel.LastUpdated' + '\n' +
                ' ,LibraryPIMProductMeasurement.Discount AS MeasurementDiscount' + '\n' +
                ' ,LibraryPIMProductPriceLevel.DeactivateDiscount' + '\n' +
                ',' + ReadyString(pDrBase.UnitMeasure) + ' AS BaseUnit' + '\n' +
                ',' + ReadyString(pDrBase.UnitQty) + ' AS BaseUnitQty' + '\n' +
                ' FROM LibraryPIMProductMeasurement' + '\n' +
                ' JOIN LibraryPIMProductPriceLevel ON LibraryPIMProductMeasurement.ProductID = LibraryPIMProductPriceLevel.ProductID' + '\n' +
                ' JOIN LibraryPIMPriceLevel ON LibraryPIMProductPriceLevel.PriceLevelID = LibraryPIMPriceLevel.ID  ' + '\n' +
                ' JOIN LibraryPIMMeasure ON LibraryPIMMeasure.ID = LibraryPIMProductMeasurement.UnitMeasureID ' + '\n' +
                ' JOIN LibraryCountriesLanguageLabel ON LibraryCountriesLanguageLabel.ID = LibraryPIMMeasure.MeasureNameID' + '\n';

            if (CStr2(vCustomerID).trim() != '') {
                pSql += ' JOIN LibraryOrganizationPriceLevel ON LibraryOrganizationPriceLevel.PriceLevelID = LibraryPIMPriceLevel.ID' + '\n' +
                    ' AND LibraryOrganizationPriceLevel.OrganizationID = ' + ReadyString(vCustomerID) + '\n';
            };

            pSql += ' WHERE LibraryPIMProductMeasurement.ProductID = ' + ReadyString(vID) + '\n' +
                ' AND LibraryPIMProductPriceLevel.PriceLevelID <> ' + ReadyGuid('01000012-0001-0000-0000-000000000000') + '\n' +
                ' AND LibraryPIMProductPriceLevel.PriceLevelID <> ' + ReadyGuid('01000012-0002-0000-0000-000000000000') + '\n' +
                ' AND LibraryPIMProductPriceLevel.PriceLevelID <> ' + ReadyGuid('01000012-0003-0000-0000-000000000000') + '\n' +
                ' AND LibraryPIMPriceLevel.IsActive = 1' + '\n' +
                ' AND LibraryPIMProductPriceLevel.OrganizationID = ' + ReadyString(sessionStorage.ActualOrgID) + '\n' +
                ' ORDER BY LibraryPIMPriceLevel.PriceLevel' + '\n' +
                ',LibraryPIMProductMeasurement.MeasurementLevel';
            pDT = Sql.Execute(pSql);

            return pDT;

        }; // PriceTableForTEST
        //end of additional by janine


        this.BaseUnit = function (vID) {

            var pDR, pSql;

            pSql = 'SELECT LibraryPIMProductMeasurement.UnitMeasureID\n' +
                ',LibraryCountriesLanguageLabel.Label AS UnitMeasure' + '\n' +
                ' ,LibraryPIMProductMeasurement.Quantity AS UnitQty ' + '\n' +
                ' FROM LibraryPIMProductMeasurement' + '\n' +
                ' JOIN LibraryPIMMeasure ON LibraryPIMMeasure.ID = LibraryPIMProductMeasurement.UnitMeasureID ' + '\n' +
                ' JOIN LibraryCountriesLanguageLabel ON LibraryCountriesLanguageLabel.ID = LibraryPIMMeasure.MeasureNameID' + '\n' +
                ' WHERE LibraryPIMProductMeasurement.ProductID = ' + ReadyString(vID) + '\n' +
                ' ORDER BY LibraryPIMProductMeasurement.Quantity' + '\n' +
                ' LIMIT 1';
            pDR = Sql.ExecuteRow(pSql);

            return pDR;

        }; // BaseUnit

        this.LargestUnit = function (vID, vFranchiseGroupID, vFilter) {

            var pDR, pSql;

            pSql = 'SELECT LibraryPIMProductMeasurement.UnitMeasureID\n' +
                ',LibraryCountriesLanguageLabel.Label AS UnitMeasure' + '\n' +
                ' ,LibraryPIMProductMeasurement.Quantity AS UnitQty ' + '\n' +
                ' ,LibraryPIMProductMeasurement.Quantity * LibraryPIMProductPriceLevel.Price - (LibraryPIMProductMeasurement.Quantity * LibraryPIMProductPriceLevel.Price) * (CAST(LibraryPIMProductMeasurement.Discount AS Float) / 100) * (NOT LibraryPIMProductPriceLevel.DeactivateDiscount) AS DiscountedPrice ' + '\n' +
                ' ,LibraryPIMProductPriceLevel.Price - LibraryPIMProductPriceLevel.Price * (CAST(LibraryPIMProductMeasurement.Discount AS Float) / 100) * (NOT LibraryPIMProductPriceLevel.DeactivateDiscount) AS DiscountedPricePerUnit ' + '\n' +
                ' FROM LibraryPIMProductMeasurement' + '\n' +
                ' JOIN LibraryPIMProductPriceLevel ON LibraryPIMProductMeasurement.ProductID = LibraryPIMProductPriceLevel.ProductID' + '\n' +
                ' JOIN LibraryPIMPriceLevel ON LibraryPIMProductPriceLevel.PriceLevelID = LibraryPIMPriceLevel.ID  ' + '\n' +
                ' AND (\n' +
                '    LibraryPIMPriceLevel.FromDate IS NULL \n' +
                '    OR ' + ReadyDate(proj.GetZReadDate()) + ' BETWEEN LibraryPIMPriceLevel.FromDate AND LibraryPIMPriceLevel.ToDate' + '\n' +
                ')\n' +
                ' JOIN LibraryPIMMeasure ON LibraryPIMMeasure.ID = LibraryPIMProductMeasurement.UnitMeasureID ' + '\n' +
                ' JOIN LibraryCountriesLanguageLabel ON LibraryCountriesLanguageLabel.ID = LibraryPIMMeasure.MeasureNameID' + '\n';

            if (CStr2(vFranchiseGroupID).trim() != '') {
                pSql += ' JOIN LibraryFranchiseGroupPriceLevels ON LibraryFranchiseGroupPriceLevels.PriceLevelID = LibraryPIMPriceLevel.ID' + '\n' +
                    ' AND LibraryFranchiseGroupPriceLevels.GroupID = ' + ReadyString(vFranchiseGroupID) + '\n'
            }                

            pSql += ' WHERE LibraryPIMProductMeasurement.ProductID = ' + ReadyString(vID) + '\n' +
                ' AND LibraryPIMProductPriceLevel.OrganizationID = ' + ReadyString(sessionStorage.OrgID) + '\n'

            if (CBool(sessionStorage.IsVendingMachine)) {
                //FOR RICE
                pSql += ' AND LibraryPIMProductMeasurement.Quantity <= 1000 \n'
            }

            pSql += ' ORDER BY LibraryPIMProductMeasurement.Quantity DESC' + '\n' +
                ' LIMIT 1';
            pDR = Sql.ExecuteRow(pSql);

            return pDR;

        }; // BaseUnit

        this.GetPrice = function (vProductID, vPriceLevelText, vPriceLevelID, vOrgID) {

            var pSql;
            var pDT;
            var pOut;

            if (!IsGuid(vPriceLevelID))
                pSql = Business.SystemDefaults.GetPublicSystemDefault(vPriceLevelText);
            if (!IsGuid(pSql))
                pSql = vPriceLevelID;

            pSql = 'SELECT Price ' +
                ',LastUpdated' +
                ' FROM LibraryPIMProductPriceLevel ' +
                ' WHERE PriceLevelID =' + ReadyString(ReadyGuid(pSql)) +
                ' AND ProductID = ' + ReadyString(ReadyGuid(vProductID)) +
                ' AND OrganizationID = ' + ReadyString(ReadyGuid(vOrgID));
            pDT = Sql.Execute(pSql);
            if (pDT.Rows.length > 0)
                pOut = pDT.Rows[0].Price;
            if (IsNumeric(pOut))
                return pOut;

        };

        this.GetRetailPrice = function (vProductID, vOrgID) {

            return this.GetPrice(vProductID, "Retail Price ID", '01000012-0005-0000-0000-000000000000', vOrgID);

        };

        this.GetLandedCost = function (vProductID, vOrgID) {

            return this.GetPrice(vProductID, "Landed Cost ID", '01000012-0003-0000-0000-000000000000', vOrgID);

        };

        this.GetAllowDecimal = function (vProductID) {
            // requires the current organization

            var pSql;

            pSql = ' SELECT LibraryOrganizationPIM.AllowDecimal' + '\n' +
                ' FROM private.LibraryOrganizationPIM' + '\n' +
                ' WHERE LibraryOrganizationPIM.ProductID = ' + ReadyString(vProductID);
            return CBool(Sql.ExecuteScalar(pSql));

        };

        var mSrpPriceID;
        this.GetSrpPriceID = function () {

            var pSql;

            if (mSrpPriceID == undefined) {
                pSql = ' SELECT ID, PriceLevel' + '\n' +
                    ' FROM LibraryPIMPriceLevel' + '\n' +
                    ' WHERE PriceLevel = \'SRP (Suggested Retail Price)\'';
                mSrpPriceID = Sql.ExecuteScalar(pSql);
            }

            return mSrpPriceID;

        }; // END GetSrpPriceID

        this.GetSrp = function (vProductID, vOrgID) {

            return this.GetPrice(vProductID, null, this.GetSrpPriceID(), vOrgID);

        };

        this.GetPriceBreak = function(vParams) {
            
            /* 
            vParams = {
                ProductID: 'guid',
                UnitMeasureID: optional 'guid',
                Qty: optional 'integer'. if given, result will only include rows with Qty that is equal to or lesser than the parameter
            }
            */

            var pSql = '';
            var pDT;
            var pDesc = '';

            if (vParams.UnitMeasureID != null) 
                pSql += ' AND LibraryOrganizationPIMPriceBreak.UnitMeasureID = ' + ReadyString(vParams.UnitMeasureID);
            if (vParams.Qty != null) {
                pSql += ' AND LibraryOrganizationPIMPriceBreak.Qty <= ' + CDec2(vParams.Qty);
                pDesc = ' DESC';
            }

            pSql = ' SELECT LibraryOrganizationPIMPriceBreak.ID AS [ID]' + '\n' + 
                ' ,LibraryCountriesLanguageLabel.Label AS [UnitMeasure]' + '\n' + 
                ' ,LibraryOrganizationPIMPriceBreak.UnitMeasureID AS [UnitMeasureID]' + '\n' + 
                ' ,LibraryOrganizationPIMPriceBreak.Qty AS [Qty]' + '\n' + 
                ' ,LibraryOrganizationPIMPriceBreak.Discount AS [Discount]' + '\n' + 
                ' ,LibraryPIMProductMeasurement.Quantity AS UnitQty' + '\n' + 
                ' FROM private.LibraryOrganizationPIMPriceBreak' + '\n' + 
                ' JOIN LibraryPIMMeasure ON LibraryPIMMeasure.ID = private.LibraryOrganizationPIMPriceBreak.UnitMeasureID' + '\n' + 
                ' JOIN LibraryCountriesLanguageLabel ON LibraryCountriesLanguageLabel.ID = LibraryPIMMeasure.MeasureNameID' + '\n' + 
                ' JOIN LibraryPIMProductMeasurement' + '\n' + 
                '     ON LibraryPIMProductMeasurement.ProductID = private.LibraryOrganizationPIMPriceBreak.ProductID' + '\n' + 
                '     AND LibraryPIMProductMeasurement.UnitMeasureID = private.LibraryOrganizationPIMPriceBreak.UnitMeasureID' + '\n' + 
                ' WHERE LibraryOrganizationPIMPriceBreak.ProductID = ' + ReadyString(vParams.ProductID) + '\n' + 
                pSql + 
                ' ORDER BY UnitQty, Qty' + pDesc;

            pDT = Sql.Execute(pSql);

            return pDT;

        }; // END this.GetPriceBreak = function(vProductID) {
      
        var mRoundingMethod;
        var mRoundUpSellingPricesCentavo;

        this.RoundPrice = function(vPrice) {
            if (mRoundingMethod == null)
                mRoundingMethod = Business.SystemDefaults.RoundUpSellingPrices();
            if ((mRoundingMethod != 'always')
                && (mRoundingMethod != 'once'))
                return vPrice;

            if (mRoundUpSellingPricesCentavo == null) {
                mRoundUpSellingPricesCentavo = Business.SystemDefaults.RoundUpSellingPricesCentavo();
                if (mRoundUpSellingPricesCentavo <= 0)
                    mRoundUpSellingPricesCentavo = 0;
                else if (mRoundUpSellingPricesCentavo >= 100)
                    mRoundUpSellingPricesCentavo = 100;
            }
            var pOut = RoundDecimal(vPrice, mRoundUpSellingPricesCentavo / 100, 'up');
            return pOut;

        }; // END RoundPrice

        return this;
    };
});