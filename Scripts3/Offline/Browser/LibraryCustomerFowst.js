define([
    '/Scripts3/Offline/Browser/common.js?crc='//,
//'/Scripts3/Offline/Business/Business.js?crc='
], function () {

    OfflineBrowser.LibraryCustomerFowst = {
        GetData: function (vData) {
            /***
            vData: {
                OrgID: 'org id to filter fowst from'
            };
            ***/
                
            var pFinder = OfflineBrowser.Base().CreateFinder(vData);
            pFinder.Distinct = true;
            var pFields = pFinder.Fields;
            var pField;
            if (IsNumeric(vData.MaxRecords))
                pFinder.MaxRecords = vData.MaxRecords;

            pField = pFields.Add('ID');
            pField.Expression = 'LibraryOrganizationFOWST.ID';
            pField.Searchable = false;

            pField = pFields.Add('FowstName');
            pField.Expression = 'LibraryOrganizationFOWST.Name';

            pField = pFields.Add("Address");
            pField.Expression = 'IFNULL(LibraryOrganizationFOWST.StreetNumber, \'\') ' +
                '|| \',\' || IFNULL(LibraryOrganizationFOWST.Street, \'\') ' +
                '|| \',\' || IFNULL(cty.Name, \'\') ' +
                '|| \',\' || IFNULL(ctry.Name, \'\')';

            pFinder.SqlFrom = 'FROM LibraryOrganizationFOWST' +
                ' LEFT JOIN LibraryCities AS cty ON LibraryOrganizationFOWST.FowstCityID = cty.ID' +
                ' LEFT JOIN LibraryCountries AS ctry ON LibraryOrganizationFOWST.FowstCountryID = ctry.ID';

            pFinder.SqlWhere = ' WHERE LibraryOrganizationFOWST.OrganizationID = ' + ReadyString(vData.OrgID) + 
                                ' AND IFNULL(LibraryOrganizationFOWST.IsActive, 1) = 1';
            pFinder.SqlOrder = ' ORDER BY FowstName, Address';

            return pFinder.Requery();

        } // Show
    }; // OfflineBrowser.LibraryOrganization

});