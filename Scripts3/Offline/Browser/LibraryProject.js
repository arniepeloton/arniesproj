define([
    '/Scripts3/Offline/Browser/common.js?crc='
], function () {

    OfflineBrowser.LibraryProject = {
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

            pField = pFields.Add('IsFOWSTInsideAMall');
            pField.Expression = 'fowst.IsFOWSTInsideAMall';

            pField = pFields.Add('FOWSTMallName');
            pField.Expression = 'fowst.FOWSTMallName';

            pField = pFields.Add("Address");
            pField.Expression = 'IFNULL(fowst.StreetNumber, \'\') ' +
                                    '|| \',\' || IFNULL(fowst.Street, \'\') ' +
                                    '|| \',\' || IFNULL(cty.Name, \'\') ' +
                                    '|| \',\' || IFNULL(ctry.Name, \'\')';

            pFinder.SqlFrom = ' FROM LibraryOrganizationFOWST fowst ' +
                ' LEFT OUTER JOIN LibraryCities cty ON (fowst.FowstCityID = cty.ID) ' +
                ' LEFT OUTER JOIN LibraryCountries ctry ON (fowst.FowstCountryID = ctry.ID)'

            pFinder.SqlWhere = ' WHERE fowst.OrganizationID = ' + ReadyString(sessionStorage.OrgID) + ' AND fowst.FOWSTTypeID = "dff0f087-4c2b-4b67-aa1d-502f02773e2b"';

            return pFinder.Requery();

        } // Show
    }; // OfflineBrowser.LibraryProject

});