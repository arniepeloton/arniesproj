define([], function() {
    window.Business.LibraryOrganization = new function(vParent) {
        this.ByID = function(vID) {

            var pDR;
            var pSql;

            pSql = 'SELECT LibraryOrganization.ID' +
                ',LibraryOrganization.CompanyName, LibraryOrganization.PersonSurname, LibraryOrganization.PersonFirstName, LibraryOrganization.PersonMiddleName, LibraryOrganization.PersonFullname' +
                ', BusinessAddressBuildingNo, BusinessAddressCityID, BusinessAddressCountryID, BusinessAddressStreet' +
                ', OfficeAddressBuildingNo, OfficeAddressCityID, OfficeAddressCountryID, OfficeAddressStreet, TinNumber, IsOrganization' +
                ' FROM LibraryOrganization' + 
                ' WHERE LibraryOrganization.ID = ' + ReadyString(vID);
             pDR = Sql.ExecuteRow(pSql);

             return pDR;
        };

        this.GetAddresses = function(vID) {
            
            var pDT;
            var pSql;

            pSql = ' SELECT LibraryOrganizationFOWST.ID AS ID' + '\n' + 
                ' ,IFNULL(NULLIF(TRIM(LibraryOrganizationFOWST.StreetNumber) || \' \', \' \'), \'\') ||' + '\n' + 
                ' IFNULL(NULLIF(TRIM(LibraryOrganizationFOWST.Street) || \', \',\', \'), \'\') || ' + '\n' + 
                ' IFNULL(NULLIF(TRIM(LibraryCities.Name) || \', \',\', \'), \'\') || ' + '\n' + 
                ' IFNULL(NULLIF(TRIM(LibraryCountries.Name) || \' \',\' \'), \'\') as OrgCompleteAddress ' + '\n' + 
                ' FROM LibraryOrganizationFOWST ' + '\n' + 
                ' LEFT JOIN LibraryCities ON (LibraryOrganizationFOWST .FowstCityID = LibraryCities .ID) ' + '\n' + 
                ' LEFT JOIN LibraryCountries ON (LibraryOrganizationFOWST .FowstCountryID = LibraryCountries .ID) ' +
                ' WHERE LibraryOrganizationFOWST.OrganizationID = ' + ReadyString(vID) +
                ' ORDER BY LibraryOrganizationFOWST.LastUpdated DESC, OrgCompleteAddress  DESC';
            pDT = Sql.Execute(pSql);

            return pDT;

        }; // END GetAddresses

        this.CanTestPrice = function() {

            return ReadyGuid(sessionStorage.OrgID) == '9D4B1500-13C0-49F9-A8E5-A16C13EC899E';

        };

        this.GetCustomerInfo = function(vID) {
            
            var pSql = 'SELECT ID\n' +
                ',UnlimitedCredit\n' +
                ' FROM LibraryOrganizationCustomer\n' +
                ' WHERE LibraryOrganizationCustomer.OrganizationID = ' + ReadyString(vID);
            var pDR = Sql.ExecuteRow(pSql);

            return pDR;

        };

        return this;
    };
});