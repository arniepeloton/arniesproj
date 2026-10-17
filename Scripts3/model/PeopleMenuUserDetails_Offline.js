define(['common']
, function () {
    CreateStringObject('App.model.PeopleMenuUserDetails_offline', null, function () {
        var pOut = {};

        pOut.GetUserDetails = function (OrgID) {
            var existingDB = Sql.GetDatabaseFiles();

            if ($.inArray(OrgID + ".public.db", existingDB) == -1) 
                return false;

            if ($.inArray(OrgID + ".private.db", existingDB) == -1) 
                return false;

            Sql.OpenDatabase(OrgID + '.public.db');
            Sql.AttachDatabase(OrgID + '.private.db');

            var pSqlFowst = '';
            var pDTFowst;
            var pSqlRole = '';
            var pDTRole;
            var pSqlDevice = '';
            var pDTDevice;
            var data = new Array();

            pSqlFowst = 'SELECT DISTINCT ' +
                    ' fowst.ID as FowstID, ' +
                    ' fowst.Name as FowstName, ' +
                    ' IFNULL(fowst.StreetNumber, "") || \', \' || IFNULL(fowst.Street, "") || \', \' || IFNULL(cty.Name, "") || \', \' || IFNULL(ctry.Name, "") as Address ' +
                    ' FROM LibraryOrganizationFOWST fowst ' +
                    ' LEFT OUTER JOIN LibraryCities cty ON fowst.FowstCityID = cty.ID ' +
                    ' LEFT OUTER JOIN LibraryCountries ctry ON fowst.FowstCountryID = ctry.ID ' +
                    ' INNER JOIN LibraryOrganizationEmployeeFOWST ON LibraryOrganizationEmployeeFOWST.FowstID = fowst.ID ' +
                    ' WHERE LibraryOrganizationEmployeeFOWST.PeopleID = ' + ReadyString(Web.PeopleUserID())

            pDTFowst = Sql.Execute(pSqlFowst);


            pSqlRole = 'SELECT ' +
                        ' role.ID as RoleID, ' +
                        ' role.Code as Code, ' +
                        ' role.Role as Role, ' +
                        ' role.Description as Description ' +
                        ' FROM LibrarySecurityRoleUser roleusr ' +
                        ' INNER JOIN LibrarySecurityRole role ON (roleusr.RoleID = role.ID) ' +
                        ' WHERE roleusr.PeopleID = ' + ReadyString(Web.PeopleUserID())

            pDTRole = Sql.Execute(pSqlRole);

            pSqlDevice = 'SELECT LibraryDeviceFOWST.FowstID' +
                            ', LibraryOrganizationFOWST.Name ' +
                            ' FROM LibraryDeviceFOWST ' +
                            ' LEFT JOIN LibraryDevice ON LibraryDevice.ID = LibraryDeviceFOWST.DeviceID ' +
                            ' LEFT JOIN LibraryOrganizationFOWST ON LibraryOrganizationFOWST.ID = LibraryDeviceFOWST .FowstID ' +
                            ' WHERE LibraryDevice.UUID = ' + ReadyString(Web.DeviceSerial()) +
                            ' AND LibraryDeviceFOWST.IsActive = 1'

            pDTDevice = Sql.Execute(pSqlDevice);

            data.push({ 'Fowst': pDTFowst, 'Role': pDTRole, 'Device': pDTDevice });

            return data;
        }

        return pOut;
    })
})