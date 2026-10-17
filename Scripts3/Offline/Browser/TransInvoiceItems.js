define([
    '/Scripts3/Offline/Browser/common.js?crc='
], function () {

    OfflineBrowser.TransInvoiceItems = {
        GetData: function (vData) {

            var pFinder = OfflineBrowser.Base().CreateFinder(vData);
            var pFields = pFinder.Fields;
            var pField;
            var pNonZero;

            pNonZero = vData.NonZero;

            pField = pFields.Add('ProductNo');
            pField.Expression = 'LibraryPIMProduct.ProductNo';

            pField = pFields.Add('ProductName');
            pField.Expression = 'LibraryPIMProduct.ProductName';

            pField = pFields.Add('Quantity');
            pField.Expression = 'SUM(TransInvoiceItems.Quantity)';

            pField = pFields.Add('Subtotal');
            pField.Expression = 'SUM(TransInvoiceItems.Subtotal)';

            pFinder.SqlFrom = ' FROM private.TransInvoiceItems' + '\n' + 
                ' JOIN LibraryPIMProduct ON LibraryPIMProduct.ID = TransInvoiceItems.ProductID' + '\n' + 
                ' JOIN private.TransInvoice ON TransInvoice.ID = TransInvoiceItems.InvoiceID' + '\n' + 
                ' JOIN LibraryOrganizationFOWST ON LibraryOrganizationFOWST.ID = TransInvoice.FowstID' + '\n';

            //if (vData.InvoiceID)
            //    pFinder.SqlWhere = ' WHERE TransInvoiceItems.InvoiceID = ' + ReadyString(vData.InvoiceID);

            pFinder.SqlGroup = ' GROUP BY LibraryPIMProduct.ID' + '\n';

            pFinder.SqlOrder = ' ORDER BY LibraryPIMProduct.ProductName';

            return pFinder.Requery();

        } // Show
    }; // OfflineBrowser.TransInvoice

});