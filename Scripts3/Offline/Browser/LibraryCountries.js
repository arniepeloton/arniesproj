define([
    '/Scripts3/Offline/Browser/common.js?crc='
], function () {

    OfflineBrowser.LibraryCountries = {
        GetData: function (vData) {

            var pFinder = OfflineBrowser.Base().CreateFinder(vData);
            var pFields = pFinder.Fields;
            var pField;

            pField = pFields.Add('ID');
            pField.Expression = 'ID';
            pField.Searchable = false;

            pField = pFields.Add('Name');
            pField.Expression = 'Name';


            pFinder.SqlFrom = ' FROM LibraryCountries ';

            pFinder.SqlOrder = 'ORDER BY Name';

            return pFinder.Requery();

        } // Show
    }; // OfflineBrowser.LibraryCountries

});