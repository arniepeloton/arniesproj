define(['common'
    ], function () {

        CreateStringObject('App.model.InventoryCount_Offline', null, function () {

            var pOut = {};

            pOut.GetCountPIMProductGroupNo = function (data) {
                var pSql = '';
                var pFinder = new Finder();
                var pFields = pFinder.Fields;
                var pField;

                pField = pFields.Add('ID');
                pField.Expression = 'LibraryPIMProduct.ID';
                pField.Searchable = false;

                pField = pFields.Add('ProductName');
                pField.Expression = 'LibraryPIMProduct.ProductNo || \', \' || LibraryPIMProduct.ProductName';

                pField = pFields.Add('Size');
                pField.Expression = 'LibraryPIMProductVariableFeatures001.C0017';

                pField = pFields.Add('Price');
                pField.Expression = 'LibraryPIMproductPriceLevel.Price';

                pFinder.SqlFrom = 'FROM LibraryPIMProduct  ' +
                                'inner join LibraryPIMProductVariableFeatures001 on LibraryPIMProductVariableFeatures001.ProductID = LibraryPIMProduct.ID  ' +
                                'inner join LibraryPIMproductPriceLevel on LibraryPIMproductPriceLevel.ProductID = LibraryPIMProduct.ID  ' +
                                'inner join LibraryPIMPriceLevel on LibraryPIMPriceLevel.ID = LibraryPIMproductPriceLevel.PriceLevelID  '
                pFinder.SqlWhere = 'Where ProductGroupNo = ' + ReadyString(data.ProductGroupNo) + ' AND IsBrandOwner = 1  and LibraryPIMPriceLevel.PriceLevel=\'Retail Price\' AND LibraryPIMproductPriceLevel.OrganizationID = ' + ReadyString(sessionStorage.OrgID)

                return pFinder.Requery();
            };

            pOut.GetProductInformationByBarcode = function (data) {
                var pSql = '';
                var pFinder = new Finder();
                var pFields = pFinder.Fields;
                var pField;

                pField = pFields.Add('ID');
                pField.Expression = 'LibraryPIMProduct.ID';
                pField.Searchable = false;

                pField = pFields.Add('ProductName');
                pField.Expression = 'LibraryPIMProduct.ProductName';

                pField = pFields.Add('Size');
                pField.Expression = 'LibraryPIMProductVariableFeatures001.C0017';

                pField = pFields.Add('Price');
                pField.Expression = 'LibraryPIMproductPriceLevel.Price';

                pFinder.SqlFrom = 'FROM LibraryPIMProduct  ' +
                                'inner join LibraryPIMProductVariableFeatures001 on LibraryPIMProductVariableFeatures001.ProductID = LibraryPIMProduct.ID  ' +
                                'inner join LibraryPIMproductPriceLevel on LibraryPIMproductPriceLevel.ProductID = LibraryPIMProduct.ID  ' +
                                'inner join LibraryPIMPriceLevel on LibraryPIMPriceLevel.ID = LibraryPIMproductPriceLevel.PriceLevelID  '

                //' OR LibraryPIMProduct.ID IN ' + ***change 'OR' to 'AND'
                pFinder.SqlWhere = 'Where LibraryPIMPriceLevel.PriceLevel=\'Retail Price\' AND LibraryPIMproductPriceLevel.OrganizationID = ' + ReadyString(sessionStorage.OrgID) +
                                    ' AND (LibraryPIMProduct.ID IN ' +
                                    '   (SELECT LibraryPIMProductMeasurement.ProductID' +
                                    '   FROM LibraryPIMProductMeasurementBarcode' +
                                    '   JOIN LibraryPIMProductMeasurement ON LibraryPIMProductMeasurement.ID = LibraryPIMProductMeasurementBarcode.StockMeasurementsID' +
                                    '   WHERE LibraryPIMProductMeasurementBarcode.ReadyBarcode = ' + ReadyString(ReadyBarcode(data.barcode)) + ')' +
                                    '   OR productsearchstring like \' % ' + ReadyBarcode(data.barcode) + '% \' ' +
                                    ")";

                return pFinder.Requery();
            };

            pOut.SaveInventoryCount = function (data) {
                var pIU;
                var pSql;
                var ID = '';
                var SuggestedOrder = 0;

                Business.ZRead.HandleEnforceZRead();

                if (data.SuggestedOrder != undefined) {
                    SuggestedOrder = data.SuggestedOrder
                }
                //Update Invoice 
                pIU = new InsertUpdate('private.TransInventoryProductCount');

                pIU.Add('FowstID', ReadyString(data.FowstID));
                pIU.Add('ProductID', ReadyString(data.ProductID));
                pIU.Add('InventoryCountDate', proj.GetZReadDateSql());
                pIU.Add('SuggestedOrder', SuggestedOrder);
                pIU.Add('UserID', ReadyString(Web.PeopleUserID()));
                pIU.Add('IsCancelled', 0);
                pIU.Add('LastUpdated', proj.GetZReadDateSql());
                pIU.Add('BatchID', ReadyString(data.BatchID));
                
                if (parseInt(data.IsNew)) {
                    ID = NewID();
                    pIU.Add('ID', ReadyString(ID));
                } else {
                    ID = data.ID;
                }

                pSql = pIU.GetInsert();

                Sql.BeginTransaction();

                Sql.ExecuteSql(pSql);

                //if for approval
                pSql = "SELECT DefaultValue FROM private.SystemDefaults WHERE Description = 'Inventory Adjustment Effect'"

                var mAdjustmentEffect;

                mAdjustmentEffect = CBool(Sql.ExecuteScalar(pSql));


                for (i = 0; i < data.Count.length; i++) {
                    pIU = new InsertUpdate('private.TransInventoryProductCountStock');

                    pIU.Add('ID', ReadyString(NewID()));
                    pIU.Add('InventoryProductCountID', ReadyString(ID));
                    pIU.Add('BinID', ReadyString(data.Count[i].BinID));
                    pIU.Add('ExpirationDate', ReadyString(data.Count[i].ExpirationDate));
                    pIU.Add('UnitMeasureID', ReadyString(data.Count[i].UnitMeasureID));
                    pIU.Add('Quantity', ReadyString(data.Count[i].Quantity));
                    pIU.Add('ActualQuantity', ReadyString(data.Count[i].ActualQuantity));

                    if (data.Count[i].ExpectedValue != undefined) {
                        pIU.Add('ExpectedValue', data.Count[i].ExpectedValue);
                    }
                    
                    pIU.Add('Variance', (data.Count[i].Variance || 0));
                    pIU.Add('Remarks', ReadyString(data.Count[i].Remarks));
                    
                    pIU.Add('IsGoodStock', ReadyString(data.Count[i].IsGoodStock));
                    pIU.Add('BadCountReason', ReadyString(data.Count[i].BadCountReason));

                    if (mAdjustmentEffect) {
                        pIU.Add('IsActionTaken', 1);
                    }

                    pSql = pIU.GetInsert();

                    Sql.ExecuteSql(pSql);

                    
                    if (mAdjustmentEffect) {
                        Business.TransInventoryLedger.Add(data.ProductID, data.FowstID,
                                                    ID, 'InventoryCount', Web.PeopleUserID(), undefined,
                                                    data.Count[i].Quantity, data.Count[i].BinID, undefined, undefined, undefined);
                    }

                }
                Sql.CommitTransaction();

                return true;
            };

            pOut.SaveReorderPoint = function (data) {
                var pIU;
                var pSql;

                pIU = new InsertUpdate('private.LibraryFowstBinProduct');

                pIU.Add('MaxPerProduct', data.MaxCap);
                pIU.Add('ReorderPoint', data.ReorderPoint);
                pIU.Add('ReorderQty', data.ReorderQty);
                pIU.Add('DateImported', 'null');

                pSql = pIU.GetUpdate('FowstID=' + ReadyString(data.FowstID) + 
                                    ' AND BinID=' + ReadyString(data.BinID) + 
                                    ' AND ProductID=' + ReadyString(data.ProductID));

                Sql.ExecuteSql(pSql);

                return true;

            };

            pOut.GetStockUnitMeasures = function (data, isStockRecModule) {
                var pSql = '';
                var pFinder = new Finder();
                var pFields = pFinder.Fields;
                var pField;

                pFinder.Distinct = 1;

                pField = pFields.Add('ID');
                pField.Expression = 'LibraryPIMMeasure.ID';
                pField.Searchable = false;

                pField = pFields.Add('PIMProductMeasurementID');
                pField.Expression = 'LibraryPIMProductMeasurement.ID';

                pField = pFields.Add('UnitMeasure');
                pField.Expression = 'LibraryCountriesLanguageLabel.Label';

                pField = pFields.Add('Quantity');
                pField.Expression = 'LibraryPIMProductMeasurement.Quantity';

                pField = pFields.Add('ProductID');
                pField.Expression = 'LibraryPIMProductMeasurement.ProductID';

                //pField = pFields.Add('Price');
                //pField.Expression = 'LibraryPIMproductPriceLevel.Price';

                pFinder.SqlFrom = 'FROM LibraryPIMProductMeasurement INNER JOIN \n' +
                                    'LibraryPIMMeasure ON LibraryPIMProductMeasurement.UnitMeasureID = LibraryPIMMeasure.ID INNER JOIN \n' +
                                    'LibraryCountriesLanguageLabel ON LibraryPIMMeasure.MeasureNameID = LibraryCountriesLanguageLabel.ID \n' +
                                    '--inner join LibraryPIMproductPriceLevel on LibraryPIMproductPriceLevel.ProductID = LibraryPIMProductMeasurement.ProductID \n' +
                                    '--inner join LibraryPIMPriceLevel on LibraryPIMPriceLevel.ID = LibraryPIMproductPriceLevel.PriceLevelID \n'

                if (CBool(data.IsIncludeBarcode)) {
                    pField = pFields.Add('Barcode');
                    pField.Expression = 'IFNULL(LibraryPIMProductMeasurementBarcode.Barcode, "")';

                    pFinder.SqlFrom += ' \n left join librarypimproductmeasurementbarcode on LibraryPIMProductMeasurementBarcode.StockMeasurementsID = LibraryPIMProductMeasurement.ID'
                }

                pFinder.SqlWhere = '\n WHERE LibraryPIMProductMeasurement.ProductID = ' + ReadyString(data.ProductID) + ' /*and LibraryPIMPriceLevel.PriceLevel=\'Retail Price\' AND LibraryPIMproductPriceLevel.OrganizationID = ' + ReadyString(sessionStorage.OrgID) + '*/ \n'

                pFinder.SqlOrder = ' \n Order by LibraryPIMProductMeasurement.Quantity ' + (CBool(isStockRecModule) ? 'DESC' : '')

                return pFinder.Requery();
            }

            pOut.GetAllFowstBinProducts = function (data) {
                var pFinder = new Finder();
                var pFields = pFinder.Fields;
                var pField;

                pFinder.MaxRecords = 0

                pField = pFields.Add('ID');
                pField.Expression = 'LibraryFowstBin.ID';

                pField = pFields.Add('ProductID');
                pField.Expression = 'LibraryPimProduct.ID';

                pField = pFields.Add('Name');
                pField = pFields.Add('Description');
                pField = pFields.Add('MaxPerProduct');
                pField = pFields.Add('ReorderPoint');
                pField = pFields.Add('ReorderQty');

                pField = pFields.Add('ProductName');


                pFinder.SqlFrom = 'FROM LibraryFowstBin inner join LibraryFowstbinProduct on LibraryFowstBin.ID = LibraryFowstbinProduct.BinID ' +
                                    ' Left join LibraryFowstArea on LibraryFowstArea.ID = LibraryFowstBin.AreaID' +
                                    ' INNER JOIN LibraryPimProduct on LibraryPimProduct.ID = LibraryFowstbinProduct.ProductID '

                pFinder.SqlWhere = "WHERE LibraryFowstBin.FowstID = " + ReadyString(data.FowstID) + " AND Name <> \'Unnamed Bin\' "

                if (data.ProductID != '' && data.ProductID != undefined) {
                    pFinder.SqlWhere = pFinder.SqlWhere + " AND LibraryFowstbinProduct.ProductID = " + ReadyString(data.ProductID);
                }

                if (data.AreaID != '' && data.AreaID != undefined && data.AreaID != 'All') {
                    pFinder.SqlWhere = pFinder.SqlWhere + " AND LibraryFowstArea.ID = " + ReadyString(data.AreaID);
                }

                pFinder.SqlOrder = 'Order By Name'

                return pFinder.Requery();
            }

            pOut.LibraryFowstBinProduct = function (data) {
                var pFinder = new Finder();
                var pFields = pFinder.Fields;
                var pField;

                pFinder.MaxRecords = 0

                pField = pFields.Add('ID');
                pField.Expression = 'LibraryFowstBin.ID';

                pField = pFields.Add('Name');

                pField = pFields.Add('Description');

                pFinder.SqlFrom = 'FROM LibraryFowstBin inner join LibraryFowstbinProduct on LibraryFowstBin.ID = LibraryFowstbinProduct.BinID '

                pFinder.SqlWhere = "WHERE LibraryFowstBin.FowstID = " + ReadyString(data.FowstID) + " AND Name <> \'Unnamed Bin\' "

                if (data.ProductID != '') {
                    pFinder.SqlWhere = pFinder.SqlWhere + " AND LibraryFowstbinProduct.ProductID = " + ReadyString(data.ProductID);
                }                   

                pFinder.SqlOrder = 'Order By Name'

                return pFinder.Requery();
            }

            pOut.LibraryFowstBin = function (data) {
                var pFinder = new Finder();
                var pFields = pFinder.Fields;
                var pField;

                pFinder.MaxRecords = 0

                //START of ORIGINAL Query for Bin Location
                //gi change ang query para swak sa Vending Machine and para easy to use nalang pud, and since wala pay nagagamit ani na feature gi change sya
                // iuncomment lang if irestore ang original UI for Bin Loc
                // commented by Arnie Nov 18, 2024 11:15 am
                //pField = pFields.Add('ID');
                //pField.Expression = 'IFNULL(LibraryFowstBin.ID,"")';

                //pField = pFields.Add('AreaName');
                //pField.Expression = 'IFNULL(LibraryFowstArea.AreaName,"")';
                //pField = pFields.Add('Name');
                //pField = pFields.Add('Description');
                //pField.Expression = 'IFNULL(LibraryFowstBin.Description,"")';
                //pField = pFields.Add('IsActive');

                //pFinder.SqlFrom = 'FROM LibraryFowstBin LEFT join LibraryFowstArea on LibraryFowstArea.FowstID = LibraryFowstBin.FowstID and LibraryFowstArea.id = LibraryFowstBin.AreaID'

                //pFinder.SqlWhere = "WHERE LibraryFowstBin.FowstID = " + ReadyString(data.FowstID) + " AND Name <> \'Unnamed Bin\' "

                //if (data.ProductID != '') {
                //    pFinder.SqlWhere = pFinder.SqlWhere + " AND LibraryFowstbinProduct.ProductID = " + ReadyString(data.ProductID);
                //}

                //if (data.AreaID != '' && data.AreaID != undefined && data.AreaID != 'All') {
                //    pFinder.SqlWhere = pFinder.SqlWhere + " AND LibraryFowstbin.AreaID = " + ReadyString(data.AreaID);
                //}

                //pFinder.SqlOrder = 'Order By Name'
                // END of original query

                pField = pFields.Add('ID');
                pField.Expression = 'IFNULL(bin.ID,"")';

                pField = pFields.Add('BinNo');
                pField.Expression = 'IFNULL(bin.BinNo,"")';

                pField = pFields.Add('ProductID');
                pField.Expression = 'IFNULL(binprod.ProductID,"")';
                
                pField = pFields.Add('ProductNo');
                pField.Expression = 'IFNULL(binprod.ProductNo,"")';

                pField = pFields.Add('ProductName');
                pField.Expression = 'IFNULL(binprod.ProductName,"")';

                pField = pFields.Add('MeasurementID');
                pField.Expression = 'LibraryPIMProductMeasurement.ID';

                pField = pFields.Add('UnitMeasure');
                pField.Expression = 'LibraryCountriesLanguageLabel.Label';

                pField = pFields.Add('ProductCategory');
                pField.Expression = 'cat.C0020';

                pField = pFields.Add('Balance');
                pField.Expression = 'IFNULL(binprod.Balance, "")';

                pFinder.SqlFrom = 'FROM LibraryFowstBin as bin \n' +
                    'INNER JOIN LibraryFowstBinProduct as binprod ON bin.ID = binprod.BinID \n' +
                    'INNER JOIN LibraryPIMProduct ON binprod.ProductID = LibraryPIMProduct.ID \n' +
                    'INNER JOIN LibraryPIMProductMeasurement ON LibraryPIMProduct.ID = LibraryPIMProductMeasurement.ProductID \n' +
                    'INNER JOIN LibraryPIMMeasure ON LibraryPIMProductMeasurement.UnitMeasureID = LibraryPIMMeasure.ID \n' +
                    'INNER JOIN LibraryCountriesLanguageLabel ON LibraryPIMMeasure.MeasureNameID = LibraryCountriesLanguageLabel.ID \n' +
                    'INNER JOIN LibraryPIMProductVariableFeatures001 as cat ON cat.ProductID = binprod.ProductID '

                pFinder.SqlWhere = 'WHERE binprod.FOWSTID = ' + ReadyString(data.FowstID) + ' \n' +
                    ' AND bin.IsActive = 1 \n' +
                    ' AND binprod.IsActive = 1 \n' +
                    ' AND binprod.IsDefault = 1 \n' +
                    ' AND LibraryPIMProductMeasurement.Quantity = 1 \n'

                if (CBool(sessionStorage.IsVendingMachine)) {
                    //Kung unsa ang mga HB Items, mao lang pud pwede ma inventory
                    pFinder.SqlWhere += ' AND (LibraryPIMProduct.ID IN (SELECT ProductID FROM LibraryPimGroupPims WHERE MenuID = ' + ReadyString(sessionStorage.HotButtonDataCacheTemplateID) + ') \n'
                    pFinder.SqlWhere += ' OR LibraryPIMProduct.ID IN (SELECT DefaultValue FROM private.SystemDefaults WHERE [Description] IN ("VDM 25_cent", "VDM 1_peso", "VDM 5_peso", "VDM 10_peso", "VDM 20_peso", "VDM 50_peso", "VDM 100_peso", "VDM 200_peso", "VDM 500_peso", "VDM 1000_peso"))) \n'
                }

                pFinder.SqlOrder = 'ORDER BY bin.BinNo'

                return pFinder.Requery();
            }

            pOut.GetBinBalances = function (data) {
                var pFinder = new Finder();
                var pFields = pFinder.Fields;
                var pField;

                pFinder.MaxRecords = 0

                pField = pFields.Add('ID');
                pField.Expression = 'LibraryFowstBin.ID';

                pField = pFields.Add('Name');
                pField = pFields.Add('Description');

                pField = pFields.Add('balance');
                pField.Expression = '(Select distinct (Select balance from Transinventoryledger t1 ' +
                                    'where t1.productid = t2.productid AND FOWSTID = ' + ReadyString(data.FowstID) +
                                    ' and BINID = LibraryFowstBin.ID ' +
                                    ' order by lastupdated desc limit 1) Balance from transinventoryledger t2)'

                pField = pFields.Add('UnitMeasure');
                pField.Expression = '(SELECT DISTINCT LibraryCountriesLanguageLabel.Label ' +
                                    'FROM LibraryPIMProductMeasurement INNER JOIN LibraryPIMMeasure ON ' +
                                    'LibraryPIMProductMeasurement.UnitMeasureID = LibraryPIMMeasure.ID INNER JOIN  ' +
                                    'LibraryCountriesLanguageLabel ON LibraryPIMMeasure.MeasureNameID = LibraryCountriesLanguageLabel.ID   ' +
                                    'WHERE LibraryPIMProductMeasurement.ProductID = TransInventoryLedger.ProductID ' +
                                    'ORDER BY LibraryPIMProductMeasurement.Quantity limit 1 )'

                
                pFinder.SqlFrom = 'FROM LibraryFowstBin ' +
                                'inner join TransInventoryLedger on LibraryFowstBin.ID = TransInventoryLedger.BinID'

                pFinder.SqlWhere = 'WHERE LibraryFowstBin.FowstID = ' + ReadyString(data.FowstID) +
                                    ' AND TransInventoryLedger.ProductID = ' + ReadyString(data.ProductID);
                                    //' and LibraryFowstbinProduct.BINID <> \'10d4672a-0358-4e2d-9471-2ee42e464300\'';

                pFinder.SqlOrder = 'Order By Name'

                return pFinder.Requery();
            }

            pOut.GetBrands = function () {
                var pFinder = new Finder();
                var pFields = pFinder.Fields;
                var pField;

                pFinder.Distinct = 1;

                pField = pFields.Add('Brand');

                pFinder.SqlFrom = 'FROM LibraryOrganizationPIM INNER JOIN LibraryPIMproduct ON LibraryOrganizationPIM.ProductID = LibraryPIMproduct.ID'

                return pFinder.Requery();
            }

            pOut.GetBinLocation = function () {
                var pFinder = new Finder();
                var pFields = pFinder.Fields;
                var pField;

                pFinder.Distinct = 1;

                pField = pFields.Add('Name');

                pFinder.SqlFrom = 'FROM LibraryFowstBin'

                pFinder.SqlWhere = 'WHERE Name <> \'Unnamed Bin\''
                return pFinder.Requery();
            }

            pOut.SaveBin = function (data) {
                var pFinder = new Finder();
                var pFields = pFinder.Fields;
                var pField;

                pIU = new InsertUpdate('private.LibraryFowstBin');

                pIU.Add('ID', ReadyString(data.ID));
                pIU.Add('FowstID', ReadyString(sessionStorage.FowstID));
                pIU.Add('Name', ReadyString(data.Name));
                pIU.Add('Description', ReadyString(data.Description));
                pIU.Add('IsActive', 1);
                pIU.Add('LastUpdated', proj.SqliteNow);


                pSql = pIU.GetInsert();

                Sql.ExecuteSql(pSql);
                return true;
            }

            pOut.FowstBinProductMaxMinCap = function (data) {
                var pFinder = new Finder();
                var pFields = pFinder.Fields;
                var pField;

                pFinder.Distinct = 1;

                pField = pFields.Add('MinPerProduct');
                pField = pFields.Add('MaxPerProduct');

                pFinder.SqlFrom = 'FROM LibraryFowstBinProduct binpro'

                pFinder.SqlWhere = 'WHERE binpro.FowstID = ' + ReadyString(data.FowstID) +
                        ' AND binpro.BinID = ' + ReadyString(data.BinID) +
                        ' AND binpro.ProductID = ' + ReadyString(data.ProductID)

                return pFinder.Requery();
            }

            pOut.SaveFOWSTBinProductMaxMinCap = function (data) {
                var pFinder = new Finder();
                var pFields = pFinder.Fields;
                var pField;
                var pSql = '';
                var mData;

                pSql = 'Select ID from private.LibraryFowstBinProduct ' +
                    'Where FowstID = ' + ReadyString(data.FowstID) +
                    ' AND BinID = ' + ReadyString(data.BinID) +
                    ' AND ProductID = ' + ReadyString(data.ProductID)

                mData = Sql.Execute(pSql);

                pIU = new InsertUpdate('private.LibraryFowstBinProduct');

                pIU.Add('MaxPerProduct', data.MaxCap);
                pIU.Add('MinPerProduct', data.MinCap);
                pIU.Add('FowstID', ReadyString(data.FowstID));
                pIU.Add('BinID', ReadyString(data.BinID));
                pIU.Add('ProductID', ReadyString(data.ProductID));

                if (mData.Rows.length > 0) {
                    pIU.Add('DateImported', 'null')
                    pSql = pIU.GetUpdate('ID = ' + ReadyString(mData.Rows[0].ID));
                }
                else {
                    pIU.Add('ID', ReadyString(NewID()));
                    pSql = pIU.GetInsert();
                }

                Sql.ExecuteSql(pSql);
                return true;
            }

            pOut.GetAllProducts = function (Feature, vSearch, IsFeature, inventorytype, GroupSelected, isStockRecModule) {
                var pSql = '';
                var pFinder = new Finder();
                var pFields = pFinder.Fields;
                var pField;

                pFinder.MaxRecords = 0;
                pFinder.Distinct = true;

                pField = pFields.Add('ID');
                pField.Expression = 'LibraryPIMProduct.ID';
                pField.Searchable = false;
                
                pField = pFields.Add('MeasurementID');
                pField.Expression = '(SELECT DISTINCT LibraryPIMProductMeasurement.ID ' +
                                    'FROM LibraryPIMProductMeasurement INNER JOIN LibraryPIMMeasure ON ' +
                                    'LibraryPIMProductMeasurement.UnitMeasureID = LibraryPIMMeasure.ID INNER JOIN  ' +
                                    'LibraryCountriesLanguageLabel ON LibraryPIMMeasure.MeasureNameID = LibraryCountriesLanguageLabel.ID   ' +
                                    'WHERE LibraryPIMProductMeasurement.ProductID = LibraryPimProduct.ID  ' +
                                    'ORDER BY LibraryPIMProductMeasurement.Quantity ' + (CBool(isStockRecModule) ? 'DESC' : '') + ' limit 1 )'
                pField.Searchable = false;

                pField = pFields.Add('ActualQuantityGood');
                pField.Expression = '(SELECT DISTINCT LibraryPIMProductMeasurement.Quantity ' +
                    'FROM LibraryPIMProductMeasurement INNER JOIN LibraryPIMMeasure ON ' +
                    'LibraryPIMProductMeasurement.UnitMeasureID = LibraryPIMMeasure.ID INNER JOIN  ' +
                    'LibraryCountriesLanguageLabel ON LibraryPIMMeasure.MeasureNameID = LibraryCountriesLanguageLabel.ID   ' +
                    'WHERE LibraryPIMProductMeasurement.ProductID = LibraryPimProduct.ID  ' +
                    'ORDER BY LibraryPIMProductMeasurement.Quantity ' + (CBool(isStockRecModule) ? 'DESC' : '') + ' limit 1 )'
                pField.Searchable = false;

                pField = pFields.Add('ProductNo');
                pField.Expression = 'LibraryPIMProduct.ProductNo';

                pField = pFields.Add('ProductName');
                pField.Expression = 'LibraryPIMProduct.ProductName';

                pField = pFields.Add('LastCount');
                pField.Expression = '(SELECT TransInventoryProductCountStock.Quantity ' +
                                        'FROM TransInventoryProductCountStock inner join TransInventoryProductCount ' +
                                        'on TransInventoryProductCount.ID = TransInventoryProductCountStock.InventoryProductCountID ' +
                                        'WHERE TransInventoryProductCount.ProductID = LibraryPIMProductMeasurement.ProductID ' +
                                        ' Order By LastUpdated desc ' +
                                        'LIMIT 1)';
                
                pField = pFields.Add('Remarks');
                pField.Expression = 'TransInventoryProductCountStock.Remarks';

                pField = pFields.Add('UnitMeasure');
                pField.Expression = '(SELECT DISTINCT LibraryCountriesLanguageLabel.Label ' +
                                    'FROM LibraryPIMProductMeasurement INNER JOIN LibraryPIMMeasure ON ' +
                                    'LibraryPIMProductMeasurement.UnitMeasureID = LibraryPIMMeasure.ID INNER JOIN  ' +
                                    'LibraryCountriesLanguageLabel ON LibraryPIMMeasure.MeasureNameID = LibraryCountriesLanguageLabel.ID   ' +
                                    'WHERE LibraryPIMProductMeasurement.ProductID = LibraryPimProduct.ID ' +
                                    'ORDER BY LibraryPIMProductMeasurement.Quantity ' + (CBool(isStockRecModule) ? 'DESC' : '') + ' limit 1 )'
                
                pField = pFields.Add('UnitMeasureID');
                pField.Expression = '(SELECT DISTINCT LibraryPIMMeasure.ID ' +
                                    'FROM LibraryPIMProductMeasurement INNER JOIN LibraryPIMMeasure ON ' +
                                    'LibraryPIMProductMeasurement.UnitMeasureID = LibraryPIMMeasure.ID INNER JOIN  ' +
                                    'LibraryCountriesLanguageLabel ON LibraryPIMMeasure.MeasureNameID = LibraryCountriesLanguageLabel.ID   ' +
                                    'WHERE LibraryPIMProductMeasurement.ProductID = LibraryPimProduct.ID ' +
                                    'ORDER BY LibraryPIMProductMeasurement.Quantity ' + (CBool(isStockRecModule) ? 'DESC' : '') + ' limit 1 )'

                pField = pFields.Add('MinActualQuantityGood');
                pField.Expression = '(SELECT DISTINCT LibraryPIMProductMeasurement.Quantity ' +
                                    'FROM LibraryPIMProductMeasurement INNER JOIN LibraryPIMMeasure ON ' +
                                    'LibraryPIMProductMeasurement.UnitMeasureID = LibraryPIMMeasure.ID INNER JOIN  ' +
                                    'LibraryCountriesLanguageLabel ON LibraryPIMMeasure.MeasureNameID = LibraryCountriesLanguageLabel.ID   ' +
                                    'WHERE LibraryPIMProductMeasurement.ProductID = LibraryPimProduct.ID  ' +
                                    'ORDER BY LibraryPIMProductMeasurement.Quantity limit 1 )'

                pField = pFields.Add('MinUnitMeasure');
                pField.Expression = '(SELECT DISTINCT LibraryCountriesLanguageLabel.Label ' +
                                    'FROM LibraryPIMProductMeasurement INNER JOIN LibraryPIMMeasure ON ' +
                                    'LibraryPIMProductMeasurement.UnitMeasureID = LibraryPIMMeasure.ID INNER JOIN  ' +
                                    'LibraryCountriesLanguageLabel ON LibraryPIMMeasure.MeasureNameID = LibraryCountriesLanguageLabel.ID   ' +
                                    'WHERE LibraryPIMProductMeasurement.ProductID = LibraryPimProduct.ID ' +
                                    'ORDER BY LibraryPIMProductMeasurement.Quantity limit 1 )'

                pField = pFields.Add('MinUnitMeasureID');
                pField.Expression = '(SELECT DISTINCT LibraryPIMMeasure.ID ' +
                                    'FROM LibraryPIMProductMeasurement INNER JOIN LibraryPIMMeasure ON ' +
                                    'LibraryPIMProductMeasurement.UnitMeasureID = LibraryPIMMeasure.ID INNER JOIN  ' +
                                    'LibraryCountriesLanguageLabel ON LibraryPIMMeasure.MeasureNameID = LibraryCountriesLanguageLabel.ID   ' +
                                    'WHERE LibraryPIMProductMeasurement.ProductID = LibraryPimProduct.ID ' +
                                    'ORDER BY LibraryPIMProductMeasurement.Quantity limit 1 )'

                pField = pFields.Add('NoOfUnitMeasure');
                pField.Expression = '(SELECT COUNT(ID) ' +
                                    'FROM LibraryPIMProductMeasurement ' +
                                    'WHERE LibraryPIMProductMeasurement.ProductID = LibraryPimProduct.ID)'

                if (GroupSelected == 'ByInventoryCountItems') {
                    pField = pFields.Add('MaxQtyToBeRcvTfer');
                    pField.Expression = 'LibraryOrganizationPim.MaxQtyToBeRcvTfer '
                }                

                pFinder.SqlFrom = 'FROM LibraryPIMProductMeasurement ' +
                                    'INNER JOIN LibraryPIMMeasure ON LibraryPIMProductMeasurement.UnitMeasureID = LibraryPIMMeasure.ID ' +
                                    'INNER JOIN LibraryPimProduct on LibraryPimProduct.ID = LibraryPIMProductMeasurement.ProductID ' +
                                    'LEFT JOIN LibraryPIMProductMeasurementBarcode ON LibraryPIMProductMeasurement.ID = LibraryPIMProductMeasurementBarcode.StockMeasurementsID ' +
                                    'LEFT JOIN TransInventoryProductCount on TransInventoryProductCount.ProductID = LibraryPimProduct.ID ' +
                                    'AND TransInventoryProductCount.UserID = ' + ReadyString(Web.PeopleUserID()) + ' AND date(TransInventoryProductCount.LastUpdated) = ' + ReadyString($.format.date(new Date(), 'yyyy-MM-dd')) +
                                    ' LEFT JOIN  TransInventoryProductCountStock on TransInventoryProductCount.ID = TransInventoryProductCountStock.InventoryProductCountID '

                if (inventorytype == 'raw') {
                    pFinder.SqlFrom += ' INNER JOIN LibraryPIMProductContents ON LibraryPIMProductContents.ChildProductID = LibraryPimProduct.ID '
                }

                if (inventorytype == 'finished') {
                    pFinder.SqlFrom += ' INNER JOIN LibraryPIMProductContents ON LibraryPIMProductContents.ParentProductID = LibraryPimProduct.ID '
                }

                if (GroupSelected == 'ByInventoryCountItems') {
                    pFinder.SqlFrom += " INNER JOIN LibraryOrganizationPim ON LibraryOrganizationPim.ProductID = LibraryPimProduct.ID AND IFNULL(LibraryOrganizationPim.IsInventoryCountItem, 1) = 1 "
                }
                                    
                pFinder.SqlWhere = " WHERE LibraryPimProduct.ID NOT IN (Select ProductID from LibraryPIMProductParent where IFNULL(Jit, 0) =1 AND IFNULL(IsSeperateIntoComponentDuringSalesReturns, 0)=1 )"               

                if (IsFeature) {
                    pFinder.SqlFrom += 'INNER JOIN LibraryPIMProductVariableFeatures001 on LibraryPIMProductVariableFeatures001.ProductID = LibraryPimProduct.ID'
                    
                    if (Feature != '' && Feature != 'All Categories') {
                        pFinder.SqlWhere += ' AND LibraryPIMProductVariableFeatures001.C0020 = ' + ReadyString(Feature)
                    }                    
                                
                    if (vSearch != '') {
                        pFinder.SqlWhere += ' AND (LibraryPIMProduct.ID IN ' +
                                        '   (SELECT LibraryPIMProductMeasurement.ProductID' +
                                        '   FROM LibraryPIMProductMeasurementBarcode' +
                                        '   JOIN LibraryPIMProductMeasurement ON LibraryPIMProductMeasurement.ID = LibraryPIMProductMeasurementBarcode.StockMeasurementsID' +
                                        '   WHERE LibraryPIMProductMeasurementBarcode.ReadyBarcode = ' + ReadyString(ReadyBarcode(vSearch)) + ')' +
                                            ' OR LibraryPIMProduct.productsearchstring like \'%' + vSearch + '%\' )'
                    }
                } else {
                    if (data != '') {
                        pFinder.SqlWhere = ' AND (LibraryPIMProduct.ID IN ' +
                                        '   (SELECT LibraryPIMProductMeasurement.ProductID' +
                                        '   FROM LibraryPIMProductMeasurementBarcode' +
                                        '   JOIN LibraryPIMProductMeasurement ON LibraryPIMProductMeasurement.ID = LibraryPIMProductMeasurementBarcode.StockMeasurementsID' +
                                        '   WHERE LibraryPIMProductMeasurementBarcode.ReadyBarcode = ' + ReadyString(ReadyBarcode(data)) + ')' +
                                            ' OR LibraryPIMProduct.productsearchstring like \'%' + data + '%\' )'
                    }
                }
                
                if (GroupSelected == 'ByInventoryCountItems') {
                    pFinder.SqlOrder = 'Order by IFNULL(InventoryRankingNo,9999) '
                } else {
                    pFinder.SqlOrder = 'Order by LibraryPIMProduct.ProductNo asc'
                }
                

                //if (Filter == 'optRawMaterialsFilter') {

                //    pFinder.SqlWhere += ' AND EXISTS( ' +
                //    'SELECT * ' +
                //    'FROM LibraryPIMProductContents ' +
                //    'WHERE (LibraryPIMProductContents.ChildProductID = LibraryPIMProduct.ID ' +
                //    ') AND LibraryPIMProductContents.OrganizationID = ' + ReadyString(sessionStorage.OrgID) + ')'

                //} else if (Filter == 'optFinishedGoodsFilter') {

                //    pFinder.SqlWhere += ' OR EXISTS( ' +
                //    'SELECT * ' +
                //    'FROM LibraryPIMProductContents ' +
                //    'WHERE (LibraryPIMProductContents.ParentProductID = LibraryPIMProduct.ID ' +
                //    ') AND LibraryPIMProductContents.OrganizationID = ' + ReadyString(sessionStorage.OrgID) + ')'

                //} else {
                //    pFinder.SqlWhere += ' AND EXISTS( ' +
                //    'SELECT * ' +
                //    'FROM LibraryPIMProductContents ' +
                //    'WHERE (LibraryPIMProductContents.ParentProductID = LibraryPIMProduct.ID ' +
                //    ' OR LibraryPIMProductContents.ChildProductID = LibraryPIMProduct.ID )' +
                //    ' AND LibraryPIMProductContents.OrganizationID = ' + ReadyString(sessionStorage.OrgID) + ')'
                //}
                
                return pFinder.Requery();
            }

            pOut.GetDefaultBinID = function (data) {
                var pSql = '';
                var pFinder = new Finder();
                var pFields = pFinder.Fields;
                var pField;

                pField = pFields.Add('ID');
                pField.Expression = 'LibraryFowstBinProduct.BinID';
                pField.Searchable = false;
                
                pFinder.SqlFrom = 'FROM LibraryFowstBinProduct ' 

                pFinder.SqlWhere = 'WHERE LibraryFowstBinProduct.ProductID = ' + ReadyString(data.ProductID) +
                                    ' AND LibraryFowstBinProduct.FowstID = ' + ReadyString(data.FowstID) +
                                    ' AND LibraryFowstBinProduct.IsDefault = 1'


                return pFinder.Requery();
            }

            pOut.GetRandomBin = function (data) {
                var pSql = '';
                var pFinder = new Finder();
                var pFields = pFinder.Fields;
                var pField;
                
                pFinder.MaxRecords = 1

                pField = pFields.Add('ID');
                pField.Searchable = false;

                pFinder.SqlFrom = 'FROM LibraryFowstBin where Name <> ' + ReadyString('Unnamed Bin') + ' AND FowstID = ' + ReadyString(data.FowstID)

                return pFinder.Requery();
            }

            pOut.GetAllMyCounts = function (isTurnOverItems, CategoryType) {
                var pSql = '';
                var pFinder = new Finder();
                var pFields = pFinder.Fields;
                var pField;
                var mDateFrom = new Date();

                pFinder.Distinct = true;
                pFinder.MaxRecords = 0

                pField = pFields.Add('ID');
                pField.Expression = 'TransInventoryProductCountStock.ID';
                pField.Searchable = false;                

                pField = pFields.Add('InventoryProductCountID');
                pField.Expression = 'TransinventoryProductCount.ID';
                pField.Searchable = false;


                pField = pFields.Add('ProductName');
                pField.Expression = 'LibraryPIMProduct.ProductName';

                pField = pFields.Add('UnitMeasure');
                pField.Expression = 'LibraryCountriesLanguageLabel.Label';

                pField = pFields.Add('Quantity');
                pField.Expression = 'TransInventoryProductCountStock.Quantity';

                pField = pFields.Add('ExpectedValue');
                pField.Expression = 'TransInventoryProductCountStock.ExpectedValue';

                pField = pFields.Add('Variance');
                pField.Expression = 'TransInventoryProductCountStock.Variance';

                pField = pFields.Add('Remarks');
                pField.Expression = 'TransInventoryProductCountStock.Remarks';

                pFinder.SqlFrom = 'FROM TransInventoryProductCount ' +
                                'inner join TransInventoryProductCountStock on transinventoryproductcount.ID = TransInventoryProductCountStock.InventoryProductCountID ' +
                                'inner join LibraryPIMProduct on transinventoryproductcount.ProductID = LibraryPIMProduct.ID ' +
                                'INNER JOIN LibraryPIMProductMeasurement on LibraryPIMProductMeasurement.ProductID = LibraryPIMProduct.ID ' +
                                'INNER JOIN LibraryPIMMeasure ON LibraryPIMProductMeasurement.UnitMeasureID = LibraryPIMMeasure.ID INNER JOIN ' +  
                                'LibraryCountriesLanguageLabel ON LibraryPIMMeasure.MeasureNameID = LibraryCountriesLanguageLabel.ID   ' +
                                'inner join LibraryPIMproductPriceLevel on LibraryPIMproductPriceLevel.ProductID = LibraryPIMProductMeasurement.ProductID   ' +
                                'inner join LibraryPIMPriceLevel on LibraryPIMPriceLevel.ID = LibraryPIMproductPriceLevel.PriceLevelID ' 
                
                pFinder.SqlWhere = 'WHERE TransinventoryProductCount.UserID = ' + ReadyString(Web.PeopleUserID()) +
                                    ' AND date(TransInventoryProductCount.InventoryCountDate)= date(' + proj.GetZReadDateSql() + ')' +
                                    ' AND TurnOverToID IS NULL '

                if (isTurnOverItems == "true" || isTurnOverItems == true) {
                    pFinder.SqlFrom += "INNER JOIN LibraryPIMProductVariableFeatures001 ON LibraryPIMProductVariableFeatures001.ProductID = LibraryPIMProduct.ID ";
                    pFinder.SqlWhere += " AND LibraryPIMProductVariableFeatures001.C0020 = " + ReadyString(CategoryType);
                }
                
                return pFinder.Requery();
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
            
            pOut.GetFowstArea = function (data) {
                var pSql = '';
                var pFinder = new Finder();
                var pFields = pFinder.Fields;
                var pField;

                pFinder.MaxRecords = 0
                pFinder.Distinct = true;

                pField = pFields.Add('ID');
                pField = pFields.Add('AreaName');

                pFinder.SqlFrom = 'FROM LibraryFowstArea'

                pFinder.SqlWhere = 'WHERE FowstID = ' + ReadyString(data.FowstID);

                return pFinder.Requery();
            }

            pOut.UpdateInventoryCountTurnOver = function (data) {
                var pFinder = new Finder();
                var pFields = pFinder.Fields;
                var pField;
                var pSql = '';
                var mData;
                                
                pIU = new InsertUpdate('private.TransInventoryProductCountStock');
                pIU.Add('TurnOverToID', ReadyString(data.TurnOverToID));
                pIU.Add('TurnOverDate', proj.GetZReadDateSql());
                pSql = pIU.GetUpdate('ID = ' + ReadyString(data.ID));
                Sql.ExecuteSql(pSql);
                                
                pIU = new InsertUpdate('private.TransInventoryProductCount');
                pIU.Add('DateImported', 'null')
                pSql = pIU.GetUpdate('ID = ' + ReadyString(data.InventoryProductCountID));
                Sql.ExecuteSql(pSql);
                return true;
            }

            pOut.GetAllProductsInBin = function (data) {
                var pSql = '';
                var pFinder = new Finder();
                var pFields = pFinder.Fields;
                var pField;

                pFinder.MaxRecords = 0
                pFinder.Distinct = true;

                pField = pFields.Add('ID');
                pField.Expression = 'private.LibraryFowstBinProduct.ID';
                pField.Searchable = false;

                pField = pFields.Add('ProductID');
                pField.Expression = 'private.LibraryFowstBinProduct.ProductID';
                pField.Searchable = false;

                pField = pFields.Add('StockNumber');
                pField.Expression = 'LibraryPIMProduct.ProductNo';

                pField = pFields.Add('ProductName');
                pField.Expression = 'LibraryPIMProduct.ProductName';

                pField = pFields.Add('ProductSearchString');
                pField.Expression = 'IFNULL(LibraryPIMProduct.ProductSearchString,"")';

                pField = pFields.Add('MeasurementID');
                pField.Expression = 'LibraryPIMProductMeasurement.ID';

                pField = pFields.Add('UnitMeasure');
                pField.Expression = 'LibraryCountriesLanguageLabel.Label';

                pField = pFields.Add('MaxPerProduct');
                pField.Expression = 'private.LibraryFowstBinProduct.MaxPerProduct';

                pField = pFields.Add('ReorderPoint');
                pField.Expression = 'private.LibraryFowstBinProduct.ReorderPoint';

                pField = pFields.Add('ReorderQty');
                pField.Expression = 'private.LibraryFowstBinProduct.ReorderQty';

                pField = pFields.Add('Barcode');
                pField.Expression = 'IFNULL((SELECT LibraryPIMProductMeasurementBarcode.Barcode from LibraryPIMProductMeasurementBarcode ' +
                                    'where StockMeasurementsID = LibraryPIMProductMeasurement.ID limit 1),"")';

                pFinder.SqlFrom = 'FROM private.LibraryFowstBinProduct INNER JOIN LibraryPIMProduct ON private.LibraryFowstBinProduct.ProductID = LibraryPIMProduct.ID ' +
                                'INNER JOIN LibraryPIMProductMeasurement ON LibraryPIMProduct.ID = LibraryPIMProductMeasurement.ProductID  ' +
                                'INNER JOIN LibraryPIMMeasure ON LibraryPIMProductMeasurement.UnitMeasureID = LibraryPIMMeasure.ID  ' +
                                'INNER JOIN LibraryCountriesLanguageLabel ON LibraryPIMMeasure.MeasureNameID = LibraryCountriesLanguageLabel.ID ' +
                                'LEFT JOIN LibraryPIMProductMeasurementBarcode on LibraryPIMProductMeasurement.ID = LibraryPIMProductMeasurementBarcode.StockMeasurementsID '

                pFinder.SqlWhere = 'Where BinID = ' + ReadyString(data.BinID) +
                                    ' AND FOWSTID = ' + ReadyString(data.FOWSTID) +
                                    ' AND private.LibraryFowstBinProduct.IsActive=1 ' + 
                                    '  AND LibraryPIMProductMeasurement.Quantity=1 '

                return pFinder.Requery();
            }

            pOut.SaveProductBarcode = function (Barcode, StockMeasurementsID) {
                var pIU;
                var pSql;
                var ID = '';

                var pDT;

                
                pSql = 'Select * from LibraryPIMProductMeasurementBarcode where StockMeasurementsID = ' + ReadyString(StockMeasurementsID) +
                            ' AND Barcode = ' + ReadyString(Barcode)
                    pDT = Sql.Execute(pSql);


                    pIU = new InsertUpdate('LibraryPIMProductMeasurementBarcode');

                    ID = NewID();
                    pIU.Add('ID', ReadyString(ID));
                    pIU.Add('StockMeasurementsID', ReadyString(StockMeasurementsID));
                    pIU.Add('Barcode', ReadyString(Barcode));
                    pIU.Add('ReadyBarcode ', ReadyString(ReadyBarcode(Barcode)));

                    if (pDT.Rows.length == 0) {
                        pSql = pIU.GetInsert();
                    }

                    else {
                        pIU.Add('DateImported ', 'null');
                        pSql = pIU.GetUpdate('ID = ' + ReadyString(pDT.Rows[0].ID));
                    }

                    Sql.ExecuteSql(pSql);
                
                return true;
            }

            pOut.GetAllProductCategories = function (fowstid) {
                var vCategories = pOut.GetCategories('ByInventoryCountItems').Rows;
                var pSql;

                for (var i in vCategories) {
                    pSql = 'SELECT LibraryPimProduct.ID, ProductNo, ProductName, \n' +
                        ' IFNULL((SELECT Status FROM cache.TransInventoryCountItemPerUom WHERE CountedByUser = ' + ReadyString(Web.PeopleUserID()) +' AND FowstID = ' + ReadyString(fowstid) + ' AND ProductID = LibraryPimProduct.ID AND CountDate = ' + ReadyString($.format.date(new Date(), 'MM-dd-yyyy')) + ' ORDER BY UomState LIMIT 1), "Missing") as Status, \n' +
                        ' (SELECT IFNULL(SUM(Count * Qty), "") FROM cache.TransInventoryCountItemPerUom WHERE CountedByUser = ' + ReadyString(Web.PeopleUserID()) +' AND FowstID = ' + ReadyString(fowstid) + ' AND ProductID = LibraryPimProduct.ID AND UomState = "Full" AND CountDate = ' + ReadyString($.format.date(new Date(), 'MM-dd-yyyy')) + ') as Full, \n' +
                        ' (SELECT IFNULL(SUM(Count * Qty), "") FROM cache.TransInventoryCountItemPerUom WHERE CountedByUser = ' + ReadyString(Web.PeopleUserID()) +' AND FowstID = ' + ReadyString(fowstid) + ' AND ProductID = LibraryPimProduct.ID AND UomState = "Loose" AND CountDate = ' + ReadyString($.format.date(new Date(), 'MM-dd-yyyy')) + ') as Loose \n' +
                        ' , (SELECT DISTINCT LibraryCountriesLanguageLabel.Label ' +
                        '   FROM LibraryPIMProductMeasurement INNER JOIN LibraryPIMMeasure ON ' +
                        '   LibraryPIMProductMeasurement.UnitMeasureID = LibraryPIMMeasure.ID INNER JOIN  ' +
                        '   LibraryCountriesLanguageLabel ON LibraryPIMMeasure.MeasureNameID = LibraryCountriesLanguageLabel.ID   ' +
                        '   WHERE LibraryPIMProductMeasurement.ProductID = LibraryPimProduct.ID ' +
                        '   AND LibraryPIMProductMeasurement.Quantity = 1) as BaseUnitMeasure ' +
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
        });
    });