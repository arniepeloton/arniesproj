define([], function () {
    window.Business.LibraryOrganizationFOWST = new function (vParent) {
        this.ByID = function (vFowstID) {

            var pSql;
            var pDR;

            pSql = 'SELECT LibraryOrganizationFOWST.Name AS FowstName' + '\n' +
            ' ,LibraryOrganizationFOWST.IsFOWSTInsideAMall' + '\n' +
            ' ,LibraryOrganizationFOWST.ConsolidateOrderSlipPrinting' + '\n' +
            ' ,LibraryOrganizationFOWST.PrintStickers' + '\n' +
            ' ,LibraryOrganizationFOWST.PrintQueueNumbers' + '\n' +
            ' FROM LibraryOrganizationFOWST' + '\n' +
            ' WHERE LibraryOrganizationFOWST.ID  = ' + ReadyString(vFowstID) + '\n' ;
            pDR = Sql.ExecuteRow(pSql);

            return pDR;

        };

        this.GetData = function (vFowstID) {

            var pSql;
            var pDT;

            pSql = ' SELECT LibraryDeviceFowst.DeviceID' + '\n' +
            ' ,LibraryDeviceFowst.FowstID' + '\n' +
            ' ,LibraryOrganizationFOWST.IsFOWSTInsideAMall' + '\n' +
            ' ,LibraryOrganizationFOWST.Name AS FowstName' + '\n' +
            ' ,LibraryOrganizationFOWST.Code' + '\n' +
            ' ,IFNULL(LibraryOrganizationFOWST.IsVat, 1) as IsVat' + '\n' +
            ' FROM LibraryDevice ' + '\n' +
            ' JOIN LibraryDeviceFowst ON LibraryDeviceFowst.DeviceID = LibraryDevice.ID' + '\n' +
            ' JOIN LibraryOrganizationFOWST ON LibraryOrganizationFOWST.ID = LibraryDeviceFowst.FowstID' + '\n' +
            ' JOIN LibraryOrganizationEmployeeFOWST ON LibraryOrganizationEmployeeFOWST.FowstID = LibraryOrganizationFOWST.ID ' + '\n' +
            ' WHERE LibraryDevice.UUID = ' + ReadyString(Web.DeviceSerial()) + '\n' +
            ' AND LibraryOrganizationFOWST.OrganizationID = ' + ReadyString(sessionStorage.OrgID) + '\n' +
            ' AND LibraryOrganizationEmployeeFOWST.PeopleID = ' + ReadyString(Web.PeopleUserID()) + '\n' +
            ' AND LibraryDeviceFowst.IsActive = 1 \n';

            if (IsGuid(vFowstID))
                pSql += ' AND LibraryDeviceFowst.FowstID = ' + ReadyString(vFowstID);
            pDT = Sql.Execute(pSql);

            return pDT;

        };

        this.GetEmpDeviceFOWST = function () {
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
                ' LEFT JOIN LibraryDeviceFowst ON LibraryDeviceFowst.FOWSTID = LibraryOrganizationFOWST.ID ' +
                ' LEFT JOIN LibraryDevice ON LibraryDevice.ID = LibraryDeviceFowst.DeviceID ' +
                ' LEFT JOIN LibraryOrganizationEmployeeFOWST ON LibraryOrganizationEmployeeFOWST.FowstID = LibraryOrganizationFOWST.ID ' +
                ' WHERE LibraryOrganizationFOWST.OrganizationID = ' + ReadyString(sessionStorage.OrgID) +
                ' AND LibraryDevice.UUID = ' + ReadyString(Web.DeviceSerial()) +
                ' AND LibraryOrganizationEmployeeFOWST.PeopleID = ' + ReadyString(Web.PeopleUserID()) +
                ' ORDER BY LibraryOrganizationFOWST.LastUpdated DESC, OrgCompleteAddress DESC ';
            pDT = Sql.Execute(pSql);

            return pDT;
        }

        this.GetFranchiseeData = function (vFranchiseeOrgFowstID) {

            var pSql;
            var pDR;

            pSql = 'SELECT LibraryOrganizationFranchisee.IsPOExclusiveToPrincipal' + '\n' +
                ',LibraryOrganizationFranchisee.FranchisorOrgID' + '\n' +
                ',LibraryOrganization.CompanyName AS FranchisorCompanyName' + '\n' +
                ',LibraryOrganizationFranchisee.FranchisorOrgID' + '\n' +
                ',LibraryOrganizationFranchisee.DefaultSellingPriceLevel' + '\n' +
                ',LibraryOrganizationFranchisee.FranchiseGroupID' + '\n' +
                ',LibraryOrganizationFranchisee.POPriceLevel' + '\n' +
                ' FROM LibraryOrganizationFranchisee' + '\n' +
                ' JOIN LibraryOrganization ON LibraryOrganization.ID = LibraryOrganizationFranchisee.FranchisorOrgID\n' +
                ' WHERE LibraryOrganizationFranchisee.IsActive = 1 ' + '\n' +
                ' AND LibraryOrganizationFranchisee.IsFranchisorConfirmed  = 1 ' + '\n' +
                ' AND LibraryOrganizationFranchisee.IsFranchiseeConfirmed = 1' + '\n' +
                ' AND LibraryOrganizationFranchisee.FranchiseeOrgFowstID = ' + ReadyString(vFranchiseeOrgFowstID) + '\n' +
                ' AND LibraryOrganizationFranchisee.FranchiseGroupID IS NOT NULL\n';
            pDR = Sql.ExecuteRow(pSql) || [];

            return pDR;
        };

        this.GetContactNumbers = function(vID) {

            var pSql;
            var pDR;

            pSql = 'SELECT LibraryOrganizationFOWSTMobile.ContactNo' + '\n' +
                ' FROM LibraryOrganizationFOWSTMobile' + '\n' +
                ' WHERE LibraryOrganizationFOWSTMobile.FOWSTID = ' + ReadyString(vID) + '\n';
            pDR = Sql.Execute(pSql);

            return pDR;

        };

        this.GetContactNumbersJoined = function(vID) {
            
            var pOut = '';
            var pDT = this.GetContactNumbers(vID);
            var pItem;

            for (var p = 0; p <= pDT.Rows.length - 1; p++) {
                pItem = CStr2(pDT.Rows[p].ContactNo).trim();
                if (pItem == '')
                    continue;
                if (pOut != '')
                    pOut += '\n';
                pOut += pItem;
            }

            return pOut;
        }

        this.GetFowstBin = function (vID) {

            var pSql;
            var pDR;

            pSql = 'SELECT ID, Name, Description' + '\n' +
                ' FROM LibraryFowstBin ' + '\n' +
                ' WHERE Name <> \'Unnamed Bin\' AND FowstID = ' + ReadyString(vID) + '\n';
            pDR = Sql.Execute(pSql);

            return pDR;

        };

        this.GetOrgAllFowst = function () {

            var pSql;
            var pDR;

            pSql = 'SELECT fowst.Name as FowstName, fowst.ID FROM' + '\n' +
                ' LibraryOrganizationFowst as fowst INNER JOIN LibraryOrganization as org' + '\n' +
                '   ON org.ID = fowst.OrganizationID ' + '\n' +
                ' WHERE fowst.IsActive = 1 AND org.ID = ' + ReadyString(Android.GetSessionData().OrgID) + '\n' +
                ' ORDER BY fowst.Name ASC '
            pDR = Sql.Execute(pSql);

            return pDR;

        };

        return this;
    };

});