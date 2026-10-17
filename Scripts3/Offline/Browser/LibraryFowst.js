define([
    '/Scripts3/Offline/Browser/common.js?crc='
], function () {

    OfflineBrowser.LibraryFowst = {
        GetData: function (vData) {

            var pFinder = OfflineBrowser.Base().CreateFinder(vData);
            pFinder.Distinct = true;
            var pFields = pFinder.Fields;
            var pField;

            pField = pFields.Add('ID');
            pField.Expression = 'fowst.ID';
            pField.Searchable = false;

            pField = pFields.Add('FowstName');
            pField.Expression = 'fowst.Name';

            pField = pFields.Add('IsFOWSTInsideAMall');
            pField.Expression = 'fowst.IsFOWSTInsideAMall';

            pField = pFields.Add('RobinsonTenantId');
            pField.Expression = 'fowst.RobinsonTenantId';

            pField = pFields.Add('FOWSTMallName');
            pField.Expression = 'fowst.FOWSTMallName';

            pField = pFields.Add('Operates247');
            pField.Expression = 'fowst.Operates247';

            pField = pFields.Add('TIN');
            pField.Expression = 'fowst.TINBranchCode';

            pField = pFields.Add('DefaultPosView');
            pField.Expression = 'fowst.DefaultPosView';
            pField.Searchable = false;

            pField = pFields.Add('DataImportationPort');
            pField.Expression = 'fowst.DataImportationPort';
            pField.Searchable = false;
	    
	    pField = pFields.Add('DefaultHotButtonTemplateID');
            pField.Expression = 'fowst.DefaultHotButtonTemplateID';
            pField.Searchable = false;

            pField = pFields.Add("EnableHotButtonsContextualMenu");
            pField.Expression = 'fowst.EnableHotButtonsContextualMenu';
            pField.Searchable = false;

            pField = pFields.Add("Address");
            pField.Expression = 'IFNULL(fowst.StreetNumber, \'\') ' +
                                    '|| \',\' || IFNULL(fowst.Street, \'\') ' +
                                    '|| \',\' || IFNULL(cty.Name, \'\') ' +
                                    '|| \',\' || IFNULL(ctry.Name, \'\')';

            if (vData.IsThirdPartyOrganization == '1') 
                pFinder.SqlFrom = ' FROM LibraryOrganizationFOWST fowst ' +
                ' LEFT OUTER JOIN LibraryCities cty ON (fowst.FowstCityID = cty.ID) ' +
                ' LEFT OUTER JOIN LibraryCountries ctry ON (fowst.FowstCountryID = ctry.ID)'
            else
                pFinder.SqlFrom = 'FROM LibraryOrganizationEmployee INNER JOIN ' +
                          'LibraryOrganizationEmployeeFOWST ON LibraryOrganizationEmployee.PeopleID = LibraryOrganizationEmployeeFOWST.PeopleID INNER JOIN ' +
                          'LibraryOrganizationFOWST AS fowst ON LibraryOrganizationEmployeeFOWST.FowstID = fowst.ID LEFT OUTER JOIN ' +
                          'LibraryCities AS cty ON fowst.FowstCityID = cty.ID LEFT OUTER JOIN ' +
                          'LibraryCountries AS ctry ON fowst.FowstCountryID = ctry.ID';

            if (vData.FilterFowst == '1') 
                pFinder.SqlFrom += ' JOIN LibraryDeviceFowst ON LibraryDeviceFowst.FowstID = fowst.ID\n' +
                    ' AND IFNULL(LibraryDeviceFowst.IsActive, 0) <> 0\n' +
                    ' JOIN LibraryDevice ON LibraryDevice.ID = LibraryDeviceFowst.DeviceID\n' +
                    ' AND LibraryDevice.UUID = ' + ReadyString(Web.DeviceSerial()) + '\n';

            if (vData.IsThirdPartyOrganization == '1')
                pFinder.SqlWhere = ' WHERE fowst.OrganizationID = ' + ReadyString(vData.ThirdPartyOrganizationID)
            else if(vData.IsReceiving == '1')
                pFinder.SqlWhere = ' WHERE fowst.OrganizationID = ' + ReadyString(Android.GetSessionData().OrgID) + ' AND LibraryOrganizationEmployee.Deactivated = 0';
            else
                pFinder.SqlWhere = ' WHERE fowst.OrganizationID = ' + ReadyString(Android.GetSessionData().OrgID) + ' AND LibraryOrganizationEmployee.PeopleID = ' + ReadyString(Android.GetSessionData().PeopleUserID) + ' AND LibraryOrganizationEmployee.Deactivated = 0';

            if (CStr2(vData.Mall) != '') {
                pFinder.SqlWhere += ' AND fowst.FOWSTMallName = ' + ReadyString(vData.Mall)
            }
            if (CStr2(vData.ID) != '')
                pFinder.SqlWhere += ' AND fowst.id = ' + ReadyString(vData.ID) + '\n';

            pFinder.SqlOrder = ' ORDER BY FowstName, Address';
            return pFinder.Requery();

        } // Show
    }; // OfflineBrowser.LibraryOrganization

});