define([
    '/Scripts3/Offline/Browser/common.js?crc='
], function() {

    OfflineBrowser.LibraryOrganizationBanks = {
        GetData: function(vData) {

            var pFinder = OfflineBrowser.Base().CreateFinder(vData);
            var pFields = pFinder.Fields;
            var pField;

            pField = pFields.Add('ID');
            pField.Searchable = false;
            pFields.Add('CompanyName');
            pFinder.SqlFrom = 'FROM LibraryOrganization';
            pFinder.SqlWhere = 'WHERE LibraryOrganization.IsABank = 1 ';
            if (CStr2(vData.keywords).trim() == '')
                pFinder.SqlOrder = 'ORDER BY CASE IFNULL(LibraryOrganization.BankRank,0)' + '\n' + 
                    ' WHEN 0 THEN 9999999' + '\n' + 
                    ' ELSE LibraryOrganization.BankRank' + '\n' + 
                    ' END,\n' +
                    '  LibraryOrganization.CompanyName';
            else
                pFinder.SqlOrder = 'ORDER BY LibraryOrganization.CompanyName';

            return pFinder.Requery();

        } // Show
    }; // OfflineBrowser.LibraryOrganization

});