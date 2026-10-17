define([
    '/Scripts3/Offline/Browser/common.js?crc='
], function () {

    OfflineBrowser.TransInvoicesForReprint = {
        GetData: function (vData) {

            var pFinder = OfflineBrowser.Base().CreateFinder(vData);
            var pFields = pFinder.Fields;
            var pField;
            var pNonZero;

            pNonZero = vData.NonZero;

            pFinder.MaxRecords = 50
            pFinder.SearchString = '';

            pField = pFields.Add('ID');
            pField.Expression = 'TransInvoice.ID';
            pField.Searchable = false;

            pField = pFields.Add('CustomerID');
            pField.Expression = 'TransInvoice.CustomerID';
            pField.Searchable = false;

            pField = pFields.Add('InvoiceNum');
            pField.Expression = 'TransInvoice.InvoiceNumber';

            pField = pFields.Add('CompanyName');
            pField.Expression = " (CASE " +
                                  "     WHEN LibraryOrganization.IsOrganization = 1 THEN LibraryOrganization.CompanyName " +
                                  "     WHEN LibraryOrganization.IsOrganization = 0 THEN " +
                                  "         IFNULL(NULLIF(TRIM(LibraryOrganization.PersonSurname) || ', ',', '), '') || " +
                                  "         IFNULL(NULLIF(TRIM(LibraryOrganization.PersonFirstName) || ' ',' '), '') || " +
                                  "         IFNULL(NULLIF(TRIM(LibraryOrganization.PersonMiddleName) || ' ',' '), '') || " +
                                  "         IFNULL(NULLIF(', ' || TRIM(LibraryOrganization.PersonSuffixName),', '), '') " +
                                  " END)"

            pField = pFields.Add('InvoiceDate');
            pField.Expression = 'TransInvoice.InvoiceDate';

            pField = pFields.Add('Total');
            pField.Expression = '(SELECT CAST(SUM(SubTotal) as TEXT) FROM TransInvoiceItems WHERE InvoiceID = TransInvoice.ID)';
            pField.Searchable = false;

            pField = pFields.Add('SkuCount');
            pField.Expression = '(SELECT COUNT(ID) FROM TransInvoiceItems WHERE InvoiceID = TransInvoice.ID)';
            pField.Searchable = false;

            pField = pFields.Add('SalesOrderNo');
            pField.Expression = 'IFNULL(SalesOrderNo,0)';

            pField = pFields.Add('FOWSTName');
            pField.Expression = 'LibraryOrganizationFOWST.Name';

            pField = pFields.Add('DeviceName');
            pField.Expression = 'LibraryDevice.PhoneName';

            pField = pFields.Add('UserID');
            pField.Expression = 'TransInvoice.UserID';
            pField.Searchable = false;

            pField = pFields.Add('EmployeeFullName');
            pField.Expression = 'LibraryPeople.Surname || \',\' || LibraryPeople.FirstName';

            pField = pFields.Add('CurrentUserID');
            pField.Expression = "'" + Web.PeopleUserID() + "'";
            pField.Searchable = false;

            pField = pFields.Add('MIN');
            pField.Expression = 'LibraryOrganizationFOWST.FOWSTNo || \'-\' || LibraryDevice.DeviceNo';

            pField = pFields.Add('TableNo');
            pField.Expression = 'TransInvoice.CustomerTableNo';


            //            pFinder.SqlFrom = 'FROM TransInvoice' +
            //                ' JOIN LibraryOrganization ON TransInvoice.CustomerID = LibraryOrganization.ID';
            pFinder.SqlFrom = ' FROM TransInvoice INNER JOIN' +
                      ' LibraryPeople ON TransInvoice.UserID = LibraryPeople.ID INNER JOIN' +
                      ' LibraryDevice ON TransInvoice.DeviceID = LibraryDevice.ID INNER JOIN' +
                      ' LibraryOrganizationFOWST ON TransInvoice.FowstID = LibraryOrganizationFOWST.ID' +
                      ' JOIN LibraryOrganization ON TransInvoice.CustomerID = LibraryOrganization.ID'
            pFinder.SqlWhere = " WHERE TransInvoice.IsCancelled = 0 " +
                " AND TransInvoice.InvoiceNumber > 0 " +
                " AND TransInvoice.UserID = '" + Web.PeopleUserID() + "' " +
                (sessionStorage.FowstID != "" ? " AND TransInvoice.FowstID = '" + sessionStorage.FowstID + "'" : "");

            pFinder.SqlOrder = ' ORDER BY TransInvoice.InvoiceDate DESC';

            return pFinder.Requery();

        } // Show
    }; // OfflineBrowser.TransInvoicesForReprint

});