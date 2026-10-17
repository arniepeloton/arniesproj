define([
    '/Scripts3/Offline/Browser/common.js?crc='
], function () {

    OfflineBrowser.TransReceivingAndTransfers = {
        GetData: function (vData) {

            var pFinder = OfflineBrowser.Base().CreateFinder(vData);
            var pFields = pFinder.Fields;
            var pField;
            var pNonZero;
            var pFOWSTID;

            pField = pFields.Add('ID');
            pField.Expression = 'TransReceiving.ID';
            pField.Searchable = false;

            pField = pFields.Add('ReceivingNo');
            pField = pFields.Add('IsReceiving');

            pField = pFields.Add('ReceivingPlaceID');
            pField.Expression = 'ReceivingPlaceID';
            pField.Searchable = false;

            pField = pFields.Add('ReceivingPlaceFowst');
            pField.Expression = 'LibraryOrganizationFOWST.Name';

            pField = pFields.Add('ReceivingPlaceThirdParty');
            pField.Expression = 'LibraryOrganization_2.CompanyName';

            pField = pFields.Add('SourceOfSKU1ID')
            pField.Expression = 'LibraryOrganizationFowst_1.ID';

            pField = pFields.Add('SourceOfSKU1')
            pField.Expression = 'LibraryOrganizationFowst_1.Name';

            pField = pFields.Add('SourceOfSKU2ID')
            pField.Expression = 'LibraryOrganization_1.ID';

            pField = pFields.Add('SourceOfSKU2')
            pField.Expression = 'LibraryOrganization_1.CompanyName';

            pField = pFields.Add('SKUCount')

            if (CBool(vData.IsCache)) {
                pField.Expression = '(SELECT COUNT(ProductID) FROM (SELECT ProductID FROM TransReceivingStockCountPerUom WHERE ReceivingID = TransReceiving.ID GROUP BY ProductID))';
            } else {
                pField.Expression = '(SELECT COUNT(*) FROM TransReceivingStock WHERE ReceivingID = TransReceiving.ID)';
            }

            pField = pFields.Add('DateReceived')
            pField = pFields.Add('DocumentDate')
            pField = pFields.Add('DocumentType')
            pField = pFields.Add('DocumentNo')
            pField = pFields.Add('IsDraft')
            pField = pFields.Add('TransportedBy')
            pField = pFields.Add('IsDraft')

            pField = pFields.Add('ReceiveInOrgID');
            pField.Expression = 'TransReceiving.ReceiveInOrgID';

            pField = pFields.Add('LastUpdated');
            pField.Expression = 'TransReceiving.LastUpdated';

            pField = pFields.Add('ReceiveInOutlet');
            pField.Expression = 'TransReceiving.ReceiveInOutlet';

            pField = pFields.Add('ReceiveInDepartment');
            pField.Expression = 'TransReceiving.ReceiveInDepartment';

            pField = pFields.Add('ReceiveFromOutlet');
            pField.Expression = 'TransReceiving.ReceiveFromOutlet';

            pField = pFields.Add('ReceiveFromDepartment');
            pField.Expression = 'TransReceiving.ReceiveFromDepartment';

            //pFinder.SqlFrom = "FROM LibraryOrganizationFOWST INNER JOIN " +
            //          "TransReceiving ON LibraryOrganizationFOWST.ID = TransReceiving.ReceivingPlaceID LEFT JOIN " +
            //          "LibraryOrganization ON TransReceiving.ShipperID = LibraryOrganization.ID INNER JOIN " +
            //          "LibraryPeople ON TransReceiving.UserID = LibraryPeople.ID LEFT OUTER JOIN " +
            //          "LibraryOrganization AS LibraryOrganization_1 ON TransReceiving.SupplierID = LibraryOrganization_1.ID LEFT OUTER JOIN " +
            //          "LibraryOrganizationFOWST AS LibraryOrganizationFowst_1 ON TransReceiving.SupplierID = LibraryOrganizationFowst_1.ID "

            //cache.TransReceiving table is for DRAFT transaction only, to be deleted once finalize - by Arnie 09052026

            pFinder.SqlFrom = "FROM " + (CBool(vData.IsCache) ? 'cache.' : 'private.') + "TransReceiving LEFT JOIN " +
                      "LibraryOrganizationFOWST  ON LibraryOrganizationFOWST.ID = TransReceiving.ReceivingPlaceID LEFT JOIN " +
                      "LibraryOrganization ON TransReceiving.ShipperID = LibraryOrganization.ID INNER JOIN " +
                      "LibraryPeople ON TransReceiving.UserID = LibraryPeople.ID LEFT OUTER JOIN " +
                      "LibraryOrganization AS LibraryOrganization_1 ON TransReceiving.SupplierID = LibraryOrganization_1.ID LEFT OUTER JOIN " +
                      "LibraryOrganizationFOWST AS LibraryOrganizationFowst_1 ON TransReceiving.SupplierID = LibraryOrganizationFowst_1.ID " +
                      "LEFT JOIN LibraryOrganization  AS LibraryOrganization_2 ON LibraryOrganization_2.ID = TransReceiving.ReceivingPlaceID "

            if (CBool(vData.IsReceiving)) {
                pFinder.SqlWhere = "WHERE TransReceiving.IsReceiving = 1 "                
            } else {
                pFinder.SqlWhere = "WHERE TransReceiving.IsReceiving = 0 "
            }            

            pFinder.SqlOrder = "ORDER BY IsDraft desc, ReceivingNo desc"

            return pFinder.Requery();

        }
    }
})