define([
    '/Scripts3/Offline/Browser/common.js?crc='
], function () {

    OfflineBrowser.LibraryOrganizationRepresentative = {
        GetData: function (vData) {

            var pFinder = OfflineBrowser.Base().CreateFinder(vData);
            var pFields = pFinder.Fields;
            var pField;

            pField = pFields.Add('ID');
            pField.Expression = 'orgrep.ID';
            pField.Searchable = false;

            pField = pFields.Add("RepresentativeName")
            pField.Expression = 'IFNULL(orgrep.Title, \'\') ' +
                                    '|| \',\' || IFNULL(orgrep.LastName, \'\') ' +
                                    '|| \',\' || IFNULL(orgrep.FirstName, \'\') ' +
                                    '|| \',\' || IFNULL(orgrep.MiddleName, \'\')'

            pField = pFields.Add("TelNo");
            pField.Expression = "''";

            pField = pFields.Add("MobileNo");
            pField.Expression = "''";

            pField = pFields.Add("Email");
            pField.Expression = "''";

            pField = pFields.Add("IsActive");
            pField.Expression = "orgrep.IsActive";

            pFinder.SqlFrom = " FROM LibraryOrganizationRepresentatives orgrep " +
                      "INNER JOIN LibraryOrganization org ON (orgrep.OrganizationID = org.ID)";

            pFinder.SqlWhere = " WHERE orgrep.OrganizationID = " + ReadyString(Android.GetSessionData().OrgID);

            pFinder.SqlOrder = "ORDER BY orgrep.LastName ASC, " +
                            " orgrep.FirstName ASC," +
                            " orgrep.MiddleName ASC"

            return pFinder.Requery();

        } // Show
    }; // OfflineBrowser.LibraryOrganization

});