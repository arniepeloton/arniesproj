define([
    '/Scripts3/Offline/Browser/common.js?crc='//,
//'/Scripts3/Offline/Business/Business.js?crc='
], function () {

    OfflineBrowser.LibraryPeople = {
        GetData: function (vData) {

            var pFinder = OfflineBrowser.Base().CreateFinder(vData);
            pFinder.Distinct = true;
            var pFields = pFinder.Fields;
            var pField;

            pField = pFields.Add('ID');
            pField.Expression = 'ppol.ID';
            pField.Searchable = false;

            pFields.Add('DateOfBirth', eMajorDataType.Date);

            pField = pFields.Add('PeopleName');
            pField.Expression = "IFNULL(NULLIF(LTRIM(RTRIM(ppolprenominal.Code)) || ' ', ' '), '') || " +
                "IFNULL(NULLIF(LTRIM(RTRIM(ppol.Surname)) || ', ',', '), '') || " +
                "IFNULL(NULLIF(LTRIM(RTRIM(ppol.FirstName)) || ' ',' '), '') || " +
                "IFNULL(NULLIF(LTRIM(RTRIM(ppol.MiddleName)) || ' ',' '), '') || " +
                "IFNULL(NULLIF(', ' || LTRIM(RTRIM(ppol.PersonalSuffixName)),', '), '')"; 
            // cant do this on sqlite:
            // "IFNULL(NULLIF(' ' || LTRIM(RTRIM(dbo.PeoplePostSuffixName(ppol.ID))),' '), '')";


            pFinder.SqlFrom = 'FROM LibraryPeople ppol\n';

            if (CBool(vData.EmployeesOnly))
                pFinder.SqlFrom += " JOIN private.LibraryOrganizationEmployee ON LibraryOrganizationEmployee.PeopleID = ppol.ID\n";

            pFinder.SqlFrom += " LEFT JOIN LibraryPeoplePreNominalSuffix ppolprenominal ON (ppol.TitleID = ppolprenominal.ID)";

            pFinder.SqlOrder = "ORDER BY (IFNULL(ppol.Surname,'') || ', ' || IFNULL(ppol.FirstName,'') || ' ' || IFNULL(ppol.MiddleName,'')) ASC";

            return pFinder.Requery();

        } // Show
    }; // OfflineBrowser.LibraryOrganization

});