define([
    'common'
], function() {

    CreateStringObject('App.model.ReceivingCart_offline', null, function() {
        
        var pOut = {};

        pOut.CheckForMultipleSizes = function(vProductGroupNo) {
            /***
            vParams = {
                ProductGroupNo: 'string',
                success: function(vData) {
                }
            };
            ***/

            var pDT;
            var pSql;

            pSql = 'SELECT LibraryPIMProduct.ID' +
                ',LibraryPIMProduct.ProductNo || \' \' || LibraryPIMProduct.ProductName AS ProductName' +
                ',LibraryPIMProductVariableFeatures001.C0017 AS Size' +
                ',LibraryPIMproductPriceLevel.Price' +
                ' FROM LibraryPIMProduct' +
                ' JOIN LibraryPIMProductVariableFeatures001 ON LibraryPIMProductVariableFeatures001.ProductID = LibraryPIMProduct.ID' +
                ' JOIN LibraryPIMproductPriceLevel ON LibraryPIMproductPriceLevel.ProductID = LibraryPIMProduct.ID' +
                ' JOIN LibraryPIMPriceLevel ON LibraryPIMPriceLevel.ID = LibraryPIMproductPriceLevel.PriceLevelID' +
                ' WHERE ProductGroupNo = ' + ReadyString(vProductGroupNo) + 
                ' AND IsBrandOwner = 1' +
                ' AND LibraryPIMPriceLevel.PriceLevel = \'Retail Price\'';
            pDT = Sql.Execute(pSql);

            return pDT;
            
        }; // END CheckForMultipleSizes

        pOut.GetStockUnitMeasures = function(vParams) {
            /***
            vParams = {
                StockID: 'string',
                ReceivingID: 'string'
            };
            ***/

            var pDT;
            var pSql;

            pSql = "SELECT DISTINCT LibraryPIMMeasure.ID" +
                ",LibraryCountriesLanguageLabel.Label AS UnitMeasure" +
                ",LibraryPIMProductMeasurement.Quantity" +
                ",LibraryPIMproductPriceLevel.Price";
                
            if (vParams.ReceivingID != undefined) {
                pSql += ",TransReceivingStock.QuantityGood" +
                    ",TransReceivingStock.QuantityDamaged" +
                    ",BadOrderReason" +
                    ",AdditionalInfo" +
                    ",IsResaleable";
            }

            if (vParams.ReceivingID != undefined) {
                // error here
                pSql +=  " FROM private.TransReceivingStock" +
                    " JOIN LibraryStockMeasurements ON private.TransReceivingStock.StockID = LibraryStockMeasurements.StockID" +
                    " JOIN LibraryUnitMeasures ON LibraryStockMeasurements.UnitMeasureID = LibraryUnitMeasures.ID " +
                    " AND private.TransReceivingStock.UnitMeasureGood = LibraryUnitMeasures.UnitMeasure";

                pSql += " WHERE LibraryStockMeasurements.StockID = " + ReadyString(vParams.StockID) +
                    " AND private.TransReceivingStock.ReceivingID = " + ReadyString(vParams.ReceivingID);
            } else {
                pSql += " FROM LibraryPIMProductMeasurement" +
                    " JOIN LibraryPIMMeasure ON LibraryPIMProductMeasurement.UnitMeasureID = LibraryPIMMeasure.ID" +
                    " JOIN LibraryCountriesLanguageLabel ON LibraryPIMMeasure.MeasureNameID = LibraryCountriesLanguageLabel.ID " +
                    " JOIN LibraryPIMproductPriceLevel ON LibraryPIMproductPriceLevel.ProductID = LibraryPIMProductMeasurement.ProductID" +
                    " JOIN LibraryPIMPriceLevel ON LibraryPIMPriceLevel.ID = LibraryPIMproductPriceLevel.PriceLevelID";

                pSql += " WHERE LibraryPIMProductMeasurement.ProductID = " + ReadyString(vParams.StockID) + " AND LibraryPIMPriceLevel.PriceLevel = 'Retail Price' AND LibraryPIMproductPriceLevel.OrganizationID = " + ReadyString(sessionStorage.OrgID)
            }

            pSql += " ORDER BY Quantity";
            pDT = Sql.Execute(pSql);

            return pDT;

        }; // END pOut.GetStockUnitMeasures = function(vParams) {


        pOut.GetProductMeasurements = function (data) {
            var pFinder = new Finder();
            var pFields = pFinder.Fields;
            var pField;

            pFinder.Distinct = 1;

            pField = pFields.Add('ID');

            pFinder.SqlFrom = 'FROM LibraryPIMProductMeasurement'

            pFinder.SqlWhere = 'WHERE ProductID = ' + ReadyString(data.ProductID)

            return pFinder.Requery();
        };

        pOut.SaveProductBarcode = function (Barcode, data) {
            var pIU;
            var pSql;
            var ID = '';

            var pDT;

            for (var i = 0 ; i <= data.Rows.length - 1; i++) {
                pSql = 'Select * from LibraryPIMProductMeasurementBarcode where StockMeasurementsID = ' + ReadyString(data.Rows[i].ID) +
                        ' AND Barcode = ' + ReadyString(Barcode)
                pDT = Sql.Execute(pSql);

                
                    pIU = new InsertUpdate('LibraryPIMProductMeasurementBarcode');

                    ID = NewID();
                    pIU.Add('ID', ReadyString(ID));
                    pIU.Add('StockMeasurementsID', ReadyString(data.Rows[i].ID));
                    pIU.Add('Barcode', ReadyString(Barcode));
                    pIU.Add('ReadyBarcode ', ReadyString(ReadyBarcode(Barcode)));

                    if (pDT.Rows.length == 0) {
                        pSql = pIU.GetInsert();
                    }

                    else {
                        pSql = pIU.GetUpdate('ID = ' + ReadyString(pDT.Rows[0].ID));
                    }

                    Sql.ExecuteSql(pSql);
            }

            return true;
        };

        return pOut;

    }); // END CreateStringObject('App.model.ReceivingCart_offline', null, function() {

    App.model.ReceivingCart_offline = App.model.ReceivingCart_offline(); // singleton

}); // END require
