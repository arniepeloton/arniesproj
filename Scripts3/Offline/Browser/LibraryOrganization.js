define([
    '/Scripts3/Offline/Browser/common.js?crc='
], function () {

    OfflineBrowser.LibraryOrganization = {
        GetData: function (vData) {
            //vData = {
                // ref: 2015_08_29__15_16_01_2525
            //    FilterPoSuppliers: boolean
            //}

            var pFinder = OfflineBrowser.Base().CreateFinder(vData);
            var pFields = pFinder.Fields;
            var pField;

            pFinder.Distinct = true;
            // pFinder.MaxRecords = 0;
            pField = pFields.Add('ID');
            pField.Expression = 'org.ID';
            pField.Searchable = false;

            pField = pFields.Add('CompanyName');
            pField.Expression = " (CASE " +
                                  "     WHEN org.IsOrganization = 1 THEN cast(org.CompanyName as text) " +
                                  "     WHEN org.IsOrganization = 0 THEN " +
                                  "         IFNULL(NULLIF(TRIM(org.PersonFullname) || ' ',', '), '') || " +
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

            pField = pFields.Add('IsOrganization');
            pField.Expression = 'org.IsOrganization';

            pField = pFields.Add('CompleteAddress');
            pField.Expression = 'IFNULL(NULLIF(TRIM(org.OfficeAddressBuildingNo) || \' \', \' \'), \'\') ||' + '\n' +
                ' IFNULL(NULLIF(TRIM(org.OfficeAddressStreet) || \', \',\', \'), \'\') || ' + '\n' +
                ' IFNULL(NULLIF(TRIM(LibraryCities.Name) || \', \',\', \'), \'\') || ' + '\n' +
                ' IFNULL(NULLIF(TRIM(LibraryCountries.Name) || \' \',\' \'), \'\') ' + '\n';

            if (vData.FilterCustomers == 0) {
                pFinder.SqlFrom = ' FROM LibraryOrganization org ';
            } else {
                pField = pFields.Add('DefaultHotButtonTemplateID');
                pField.Expression = 'orgcust.DefaultHotButtonTemplateID';

                pField = pFields.Add('NetSuiteSupplierFowstID');
                pField.Expression = 'orgcust.NetSuiteSupplierFowstID';
                pFinder.SqlFrom = ' FROM LibraryOrganization org\n';
                if (vData.IncludeCustomersAndSuppliers) {
                    pFinder.SqlFrom += ' INNER JOIN (' + '\n' + 
                        ' SELECT OrganizationID, NetSuiteSupplierFowstID ' + '\n' + 
                        ',LibraryOrganizationCustomer.DefaultHotButtonTemplateID' + '\n' + 
                        ' FROM LibraryOrganizationCustomer ' + '\n' + 
                        ' UNION' + '\n' + 
                        ' SELECT supplierid AS OrganizationID' + '\n' + 
                        ' ,ThirdPartyID AS NetSuiteSupplierFowstID ' + '\n' + 
                        ',NULL AS DefaultHotButtonTemplateID' + '\n' + 
                        ' FROM LibraryOrganizationSupplier' + '\n' + 
                        ' ) orgcust ON (org.ID = orgcust.OrganizationID)\n';
                } else
                    pFinder.SqlFrom += ' INNER JOIN LibraryOrganizationCustomer orgcust ON (org.ID = orgcust.OrganizationID) ';

            }

            pFinder.SqlFrom += ' LEFT JOIN LibraryCities ON (org.OfficeAddressCityID = LibraryCities .ID) ' + '\n' +
                        ' LEFT JOIN LibraryCountries ON (org.OfficeAddressCountryID = LibraryCountries .ID) '

            if (vData.FilterPoSuppliers) {
                if (!IsGuid(sessionStorage.FowstID))
                    Android.Toast('jsKD4QDRBAEY! sessionStorage.FowstID must be a valid GUID when calling OfflineBrowser.LibraryOrganization with FilterPoSuppliers');

                // ref: 2015_08_29__15_16_01_2525
                pField = pFields.Add('FowstName');
                pField.Expression = 'org.FowstName';

                pField = pFields.Add('FowstID');
                pField.Expression = 'org.FowstID';

                pFinder.SqlFrom = '   FROM ' + '\n' +
                    ' (' + '\n';

                pFinder.SqlFrom += ' SELECT org.*' + '\n' +
                    ' ,null AS FowstName' + '\n' +
                    ' ,null AS FowstID' + '\n' +
                    ' FROM LibraryOrganization org  ' + '\n' +
                    ' JOIN LibraryOrganizationFranchisee fran ON (org.ID = fran.FranchisorOrgID)   ' + '\n' +
                    ' WHERE fran.IsActive = 1 ' + '\n' +
                    ' AND fran.IsFranchisorConfirmed  = 1 ' + '\n' +
                    ' AND fran.IsFranchiseeConfirmed = 1' + '\n' +
                    ' AND fran.FranchiseeOrgFowstID = ' + ReadyString(sessionStorage.FowstID) + '\n' +
                    ' UNION' + '\n';

                var pDR = Business.LibraryOrganizationFOWST.GetFranchiseeData(sessionStorage.FowstID);
                if ((pDR == null)
                    || (!CBool(pDR.IsPOExclusiveToPrincipal))) {

                    pFinder.SqlFrom += ' SELECT LibraryOrganization.*' + '\n' +
                        ' ,null AS FowstName' + '\n' +
                        ' ,null AS FowstID' + '\n' +
                        ' FROM LibraryOrganizationSupplier' + '\n' +
                        ' JOIN LibraryOrganization ON LibraryOrganization.ID = LibraryOrganizationSupplier.SupplierID' + '\n' +
                        ' WHERE IFNULL(LibraryOrganizationSupplier.IsBan, 0) = 0' + '\n' +
                        ' AND IFNULL(LibraryOrganizationSupplier.Deactivated, 0) = 0' + '\n' +
                        ' UNION' + '\n';
                }

                pFinder.SqlFrom += ' SELECT LibraryOrganization.*' + '\n' + 
                    ' ,LibraryOrganizationFOWST.Name AS FowstName' + '\n' + 
                    ' ,LibraryOrganizationFOWST.ID AS FowstID' + '\n' + 
                    ' FROM LibraryOrganization' + '\n' + 
                    ' JOIN LibraryOrganizationFOWST ON LibraryOrganizationFOWST.OrganizationID = LibraryOrganization.ID' + '\n' + 
                    ' WHERE LibraryOrganizationFOWST.OrganizationID = ' + ReadyString(Web.OrgID()) + '\n' +
                    ' ) org';
            } else if (vData.FilterConfirmedFranchisor == 1) {
                //additional by janine june 24 2015 5pm
                //only for franchisors!
                pFinder.SqlFrom += ' INNER JOIN LibraryOrganizationFranchisee fran ON (org.ID = fran.FranchisorOrgID) ';

                pFinder.SqlWhere = ' WHERE fran.IsActive = 1 AND fran.IsFranchisorConfirmed  = 1 AND fran.IsFranchiseeConfirmed = 1 ';

            } else {

                if (vData.FilterTypePerson == 1) {
                    pFinder.SqlWhere = ' WHERE org.IsOrganization = 0 ';
                } else {
                    pFinder.SqlWhere = ' WHERE (org.IsActive = 1 OR IFNULL(org.IsActive, \'\') = \'\') ';
                }
            }

            pFinder.SqlOrder = 'ORDER BY TRIM(org.CompanyName)';

            return pFinder.Requery();

        } // Show
    }; // OfflineBrowser.LibraryOrganization

});