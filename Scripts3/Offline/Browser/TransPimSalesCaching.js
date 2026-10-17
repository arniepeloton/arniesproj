define([
    '/Scripts3/Offline/Browser/common.js?crc='
], function () {

    OfflineBrowser.TransPimSalesCaching = {
        GetData: function (vData) {

            var pFinder = OfflineBrowser.Base().CreateFinder(vData);
            pFinder.Distinct = true;
            var pFields = pFinder.Fields;
            var pField;

            pField = pFields.Add('ID');
            pField.Expression = 'fowst.ID';
            pField.Searchable = false;

            pField = pFields.Add('FowstName');
            pField.Expression = 'fowst.Name';

            pField = pFields.Add("Address");
            pField.Expression = 'IFNULL(fowst.StreetNumber, \'\') ' +
                                    '|| \',\' || IFNULL(fowst.Street, \'\') ' +
                                    '|| \',\' || IFNULL(cty.Name, \'\') ' +
                                    '|| \',\' || IFNULL(ctry.Name, \'\')';

            pField = pFields.Add('ProductNo');
            pField.Expression = 'LibraryPIMProduct.ProductNo';

            pField = pFields.Add('ProductName');
            pField.Expression = 'LibraryPIMProduct.ProductName';
            
            pField = pFields.Add('Qty');
            pField.Expression = 'TransPimSalesCaching.Qty';

            pField = pFields.Add('BomQty');
            pField.Expression = 'TransPimSalesCaching.BomQty';

            pField = pFields.Add('ItemModifierQty');
            pField.Expression = 'TransPimSalesCaching.ItemModifierQty';

            pField = pFields.Add('SalesQty');
            pField.Expression = 'TransPimSalesCaching.SalesQty';

            pField = pFields.Add('DateIntervalStart');
            pField.Expression = 'TransPimSalesCaching.DateIntervalStart';

            pField = pFields.Add('DateIntervalEnd');
            pField.Expression = 'TransPimSalesCaching.DateIntervalEnd';

            pField = pFields.Add('Interval');
            pField.Expression = 'TransPimSalesCaching.Interval';

            pFinder.SqlFrom = ' FROM TransPimSalesCaching\n' +
                ' LEFT JOIN LibraryPIMProduct ON LibraryPIMProduct.ID = TransPimSalesCaching.ProductID\n' +
                ' LEFT JOIN LibraryOrganizationFOWST fowst ON fowst.ID = TransPimSalesCaching.FowstID\n ' +
                ' LEFT OUTER JOIN LibraryCities cty ON (fowst.FowstCityID = cty.ID) \n ' +
                ' LEFT OUTER JOIN LibraryCountries ctry ON (fowst.FowstCountryID = ctry.ID) \n';

            pFinder.SqlWhere = ' WHERE TransPimSalesCaching.BomQty + TransPimSalesCaching.ItemModifierQty + TransPimSalesCaching.SalesQty';

            pFinder.SqlOrder = ' ORDER BY FowstName, Address';
            return pFinder.Requery();

        } // Show
    }; // OfflineBrowser.LibraryOrganization

});