define([
    '/Scripts3/Offline/Browser/common.js?crc='
], function () {

    OfflineBrowser.LibraryOrganizationSupplier = {
        GetData: function (vData) {
            //vData = {
                // ref: 2015_08_29__15_16_01_2525
            //    FilterPoSuppliers: boolean
            //}

            var pFinder = OfflineBrowser.Base().CreateFinder(vData);
            var pFields = pFinder.Fields;
            var pField;

            pFinder.Distinct = true;
            pFinder.MaxRecords = 0;

            pField = pFields.Add('ID');
            pField.Expression = 'org.ID';
            pField.Searchable = false;

            pField = pFields.Add('CompanyName');
            pField.Expression = " (CASE " +
                                  "     WHEN org.IsOrganization = 1 THEN cast(org.CompanyName as text) " +
                                  "     WHEN org.IsOrganization = 0 THEN " +
                                  "         IFNULL(NULLIF(TRIM(org.PersonSurname) || ', ',', '), '') || " +
                                  "         IFNULL(NULLIF(TRIM(org.PersonFirstName) || ' ',' '), '') || " +
                                  "         IFNULL(NULLIF(TRIM(org.PersonMiddleName) || ' ',' '), '') || " +
                                  "         IFNULL(NULLIF(', ' || TRIM(org.PersonSuffixName),', '), '') " +
                                  " END)";

            pField = pFields.Add('TinNumber');
            pField.Expression = 'org.TinNumber';

            pField = pFields.Add('PersonSeniorCitizensIDNo');
            pField.Expression = 'org.PersonSeniorCitizensIDNo';

            pField = pFields.Add('PersonPWDIDNo');
            pField.Expression = 'org.PersonPWDIDNo';

            pField = pFields.Add('PersonPassportNo');
            pField.Expression = 'org.PersonPassportNo';

            pField = pFields.Add('PersonDriversLicenseNo');
            pField.Expression = 'org.PersonDriversLicenseNo';

            pField = pFields.Add('PersonSSSNo');
            pField.Expression = 'org.PersonSSSNo';

            pField = pFields.Add('PersonGSISIDNo');
            pField.Expression = 'org.PersonGSISIDNo';

            pField = pFields.Add('PersonPRCIDNo');
            pField.Expression = 'org.PersonPRCIDNo';

            pField = pFields.Add('PersonPostalIDNo');
            pField.Expression = 'org.PersonPostalIDNo';

            pField = pFields.Add('PersonVotersIDNo');
            pField.Expression = 'org.PersonVotersIDNo';

            pField = pFields.Add('PersonOtherIDNo');
            pField.Expression = 'org.PersonOtherIDNo';

            if (vData.FilterCustomers == 0) {
                pFinder.SqlFrom = ' FROM LibraryOrganization org ';
            } else {
                pFinder.SqlFrom = ' FROM LibraryOrganization org \n' +
                    ' JOIN LibraryOrganizationSupplier ON org.ID = LibraryOrganizationSupplier.SupplierID\n';
                pFinder.SqlWhere = "WHERE LibraryOrganizationSupplier.Deactivated = '0' AND IFNULL(org.ID, '') <> '' AND IFNULL(LibraryOrganizationSupplier.TaggedAs, '') = ''";
            }            

            pFinder.SqlOrder = 'ORDER BY TRIM(org.CompanyName)';

            return pFinder.Requery();

        } // Show
    }; // OfflineBrowser.LibraryOrganization

});