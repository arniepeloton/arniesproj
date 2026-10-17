define([
    '/Scripts3/Offline/Browser/common.js?crc='
], function () {

    OfflineBrowser.TransBomMaker = {
        GetData: function (vData) {

            var pFinder = OfflineBrowser.Base().CreateFinder(vData);
            var pFields = pFinder.Fields;
            var pField;

            pField = pFields.Add('ID');
            pField.Expression = 'TransBomMaker.ID';
            pField.Searchable = false;

            pField = pFields.Add('Num');
            pField.Expression = 'TransBomMaker.Num';

            pField = pFields.Add('LastUpdated');
            pField.Expression = 'TransBomMaker.LastUpdated';

            pField = pFields.Add('Updater');
            pField.Expression = 'tblUpdater.Surname || \', \' || tblUpdater.FirstName ';

            pField = pFields.Add('FowstName');
            pField.Expression = 'LibraryOrganizationFOWST.Name';

            pField = pFields.Add('DateNeeded');
            pField.Expression = 'TransBomMaker.DateNeeded';

            //pField = pFields.Add('');
            //pField.Expression = '';

            pFinder.SqlFrom = ' FROM private.TransBomMaker' + '\n' + 
                ' LEFT JOIN LibraryPeople tblUpdater ON tblUpdater.ID = TransBomMaker.UserID' + '\n' + 
                ' LEFT JOIN LibraryPeople tblAssignedTo ON tblAssignedTo.ID = TransBomMaker.AssignedToID' + '\n' + 
                ' LEFT JOIN LibraryOrganizationFOWST ON LibraryOrganizationFOWST.ID = TransBomMaker.FowstID';
            pFinder.SqlOrder = 'ORDER BY TransBomMaker.LastUpdated DESC';

            return pFinder.Requery();

        } // Show
    }; // OfflineBrowser.LibraryCountries

});