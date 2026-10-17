define([
    'common',
    '/Scripts3/Offline/Browser/common.js?crc='
    ], function () {

        window.security_offline = function () {

            this.CheckSecurityCurrentUserAcccess = function (vData) {

                var pFinder = new Finder();
                var pFields = pFinder.Fields;
                var pField;
                
                pField = pFields.Add('IsModule');
                pField.Expression = 'md.IsModule';

                pField = pFields.Add('IsFeature');
                pField.Expression = 'md.IsFeature';

                pField = pFields.Add('IsHasAccess');
                pField.Expression = 'rmd.IsHasAccess';

                pField = pFields.Add('ApprovedByUserID');
                pField.Expression = 'usr.PeopleID';

                pFinder.SqlFrom = 'FROM LibrarySecurityRoleUser usr ' +
                    ' INNER JOIN LibrarySecurityRole r ON (usr.RoleID = r.ID) ' +
                    ' INNER JOIN LibrarySecurityRoleModules rmd ON (rmd.RoleID = r.ID) ' +
                    ' INNER JOIN LibrarySecurityModule md ON (rmd.ModuleID = md.ID) ';

                pFinder.SqlWhere = ' WHERE md.ModuleName = ' + ReadyString(vData.ModuleName) +
                    ' AND usr.PeopleID = ' + ReadyString(vData.UserID);
                if (!CBool(vData.IncludeNoAccess))
                    pFinder.SqlWhere += ' AND rmd.IsHasAccess = 1';

                return pFinder.Requery();

            }; // this.CheckSecurityCurrentUserAcccess = function() {

            this.CheckSecurityUserAcccess = function (vData) {
                var strUserID = "";
                var pSql = 'SELECT * FROM LibraryPeopleUserAccount WHERE Username = ' + ReadyString(vData.Username);
                var pDT = Sql.Execute(pSql);

                if (pDT.Error) {
                    MessageBox(pDT.Error);
                    return false;
                };

                if (pDT.Rows.length > 0) {

                    if (CBool(Android.VerifyHash(vData.UserPassword, pDT.Rows[0].UserPassword))) {

                        strUserID = pDT.Rows[0].PeopleID;

                    } //if (CBool(Android.VerifyHash(vData.UserPassword, pDT.Rows[0].UserPassword))) {

                } // if (pDT.Rows.length > 0) {

                return strUserID;

            }; // this.CheckSecurityUserAcccess = function() {

            return this;

        }; // window.security_offline = function() {

    });        // define

