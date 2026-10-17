define([
    '/Scripts3/Offline/Browser/common.js?crc='
], function () {

    OfflineBrowser.LibraryOrganizationCustomer = {
        GetData: function (vData) {

            var pSql = " SELECT DISTINCT ID, Name as CompanyName FROM ( \n " +
                        " SELECT org.ID, \n " +
                        " CASE WHEN org.CompanyName = '' THEN org.PersonFirstName || ' ' || org.PersonMiddleName || ' ' || org.PersonSurname ELSE \n " +
                        " org.CompanyName END as Name, \n " +
                        " CASE WHEN org.CompanyName = '' THEN 0 ELSE 1 END as IsCompany, \n " +
                        " 'Customer' as Type \n " +
                        " FROM LibraryOrganizationCustomer as cusorg \n " +
                        " LEFT JOIN LibraryOrganization as org ON cusorg.OrganizationID = org.ID \n " +
                        " WHERE cusorg.Deactivated = '0' AND IFNULL(org.ID, '') <> '' \n " +
                        " ) as T \n " +
                        " WHERE Name LIKE '%" + vData.keywords + "%' " +
                        " GROUP BY ID, Name ORDER BY Name asc \n "

            return Sql.Execute(pSql);

        } // Show
    }; // OfflineBrowser.LibraryOrganizationCustomer

});