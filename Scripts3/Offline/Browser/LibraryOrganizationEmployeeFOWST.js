define([
    '/Scripts3/Offline/Browser/common.js?crc='
], function () {

    OfflineBrowser.LibraryOrganizationEmployeeFOWST = {
        GetData: function (vData) {

            var pFinder = OfflineBrowser.Base().CreateFinder(vData);
            var pFields = pFinder.Fields;
            var pField;

            pField = pFields.Add('ID');
            pField.Expression = 'orgrep.PeopleID';
            pField.Searchable = false;

            pField = pFields.Add("PeopleName")
            pField.Expression = 'IFNULL(ppl.Surname, \'\') ' +
                                '|| \', \' || IFNULL(ppl.FirstName, \'\') ' +
                                '|| \', \' || IFNULL(ppl.MiddleName, \'\')'

            pFinder.SqlFrom = " FROM private.LibraryOrganizationEmployeeFOWST orgrep " +
                      "INNER JOIN LibraryPeople ppl ON (orgrep.PeopleID = ppl.ID)";

            pFinder.SqlWhere = " WHERE orgrep.FowstID = " + ReadyString(sessionStorage.FowstID);

            pFinder.SqlOrder = "ORDER BY ppl.Surname ASC, " +
                            " ppl.FirstName ASC," +
                            " ppl.MiddleName ASC"

            return pFinder.Requery();

        } // Show
    }; // OfflineBrowser.LibraryOrganizationEmployeeFOWST

});