define([
    'common',
    '/Scripts3/Offline/Browser/common.js?crc=',
    '/Scripts3/Offline/finder.js?crc='
    ], function () {

        window.PeopleMenu_offline = function () {

            this.getPeopleInfoName = function (vData) {

                //var pFinder = OfflineBrowser.Base().CreateFinder(vData);
                var pFinder = new Finder();
                var pFields = pFinder.Fields;
                var pField;


                pField = pFields.Add('ID');
                pField.Expression = 'ppol.ID';
                pField.Searchable = false;

                pField = pFields.Add('TitleID');
                pField.Expression = 'ppol.TitleID';

                pField = pFields.Add("Title")
                pField.Expression = 'ppolpresuffix.Code'

                pField = pFields.Add("Surname")
                pField.Expression = 'ppol.Surname'

                pField = pFields.Add("FirstName")
                pField.Expression = 'ppol.FirstName'

                pField = pFields.Add("MiddleName")
                pField.Expression = 'ppol.MiddleName'

                //                pField = pFields.Add("DateOfBirthMonth")
                //                pField.Expression = 'Month(ppol.DateOfBirth)'

                //                pField = pFields.Add("DateOfBirthDay")
                //                pField.Expression = 'Day(ppol.DateOfBirth)'

                //                pField = pFields.Add("DateOfBirthYear")
                //                pField.Expression = 'Year(ppol.DateOfBirth)'

                pField = pFields.Add("NickName")
                pField.Expression = 'ppol.NickName'

                pField = pFields.Add("MotherMaidenSurname")
                pField.Expression = 'ppol.MotherMaidenSurname'

                pField = pFields.Add("MotherMaidenFirstName")
                pField.Expression = 'ppol.MotherMaidenFirstName'

                pField = pFields.Add("MotherMaidenMiddleName")
                pField.Expression = 'ppol.MotherMaidenMiddleName'

                pField = pFields.Add("PeopleName")
                pField.Expression = 'IFNULL(NULLIF(LTRIM(RTRIM(ppolpresuffix.Code)) || \' \', \' \'), \'\') || ' +
                                  'IFNULL(NULLIF(LTRIM(RTRIM(ppol.Surname)) || \', \',\', \'), \'\') || ' +
                                  'IFNULL(NULLIF(LTRIM(RTRIM(ppol.FirstName)) || \' \',\' \'), \'\') || ' +
                                  'IFNULL(NULLIF(LTRIM(RTRIM(ppol.MiddleName)) || \' \',\' \'), \'\') || ' +
                                  'IFNULL(NULLIF(\', \' || LTRIM(RTRIM(ppol.PersonalSuffixName)),\', \'), \'\') ' //|| ' +
                //'IFNULL(NULLIF(\' \' || LTRIM(RTRIM(dbo.PeoplePostSuffixName(ppol.ID))),\' \'), \'\')'

                pFinder.Distinct = 1;

                pFinder.SqlFrom = 'FROM LibraryPeople ppol ' +
                    ' LEFT JOIN LibraryPeoplePreNominalSuffix ppolpresuffix ON (ppol.TitleID = ppolpresuffix.ID)';


                pFinder.SqlOrder = ' WHERE ppol.ID = ' + ReadyString(Android.GetSessionData().PeopleUserID);

                return pFinder.Requery();

            }; // this.CheckUser = function() {

            this.getPeopleOrganizationList = function (vParams) {
                //try {
                //    Sql.CloseConnection();
                //    Sql.OpenDatabase('Mobiz');
                //}
                //catch (e) {

                //}

                vParams = $.extend({
                    AllowTerminated: false
                }, vParams);

                var pFinder = new Finder();
                var pFields = pFinder.Fields;
                var pField;
                var pDT;

                pFinder.Distinct = true;

                pField = pFields.Add('PeopleID');
                pField.Expression = 'ppolorg.PeopleID';
                pField.Searchable = false;

                pField = pFields.Add('OrganizationID');
                pField.Expression = 'ppolorg.OrganizationID';

                pField = pFields.Add('CompanyName');
                pField.Expression = 'org.CompanyName';

                pField = pFields.Add('IsTerminated');
                pField.Expression = 'ppolorg.IsTerminated';

                pFinder.SqlFrom = 'FROM LibraryPeopleOrganization ppolorg ' +
                ' JOIN LibraryOrganization org ON (org.ID = ppolorg.OrganizationID) ' +
                ' INNER JOIN LibraryPeople emp ON (ppolorg.PeopleID = emp.ID) '

                pFinder.SqlWhere = ' WHERE emp.ID = ' + ReadyString(Android.GetSessionData().PeopleUserID) +
                    ' AND ppolorg.IsConfirmed = 1\n';
                if (!vParams.AllowTerminated)
                    pFinder.SqlWhere += ' AND ppolorg.IsTerminated = 0 ';

                pFinder.SqlOrder = ' ORDER BY org.CompanyName ASC';

                pDT = pFinder.Requery();
                for (var p = 0; p <= pDT.Rows.length - 1; p++) {
                    var pDR = pDT.Rows[p];
                    var pFileText = Transfers.ReadFile('data/' + pDR.OrganizationID + '.txt', null, true);
                    pDR.DbInfo = {};
                    if (CStr2(pFileText).trim() != '')
                        try {
                            pDR.DbInfo = JSON.parse(pFileText);
                        } catch (e) {
                            // just ignore any errors
                        } 
                    pFileText = Transfers.ReadFile('LastSync/' + pDR.OrganizationID + '.txt', null);
                    if (CStr2(pFileText).trim() != '')
                        pDR.LastSync = ParseDate(pFileText);
                }

                return pDT;
            };

            this.LoadOfflineMessages = function () {

                var pStr;

                try {
                    pStr = Transfers.ReadFile('MainMenuOfflineMessages.json');
                    if (CStr2(pStr).trim() == '')
                        return null;
                    return JSON.parse(pStr);
                } catch (e) {
                    console.trace(e.stack);
                    return null;
                }

            };

            this.SaveOfflineMessages = function (vJson) {

                var pStr;

                try {
                    if (vJson == undefined)
                        return false;

                    pStr = JSON.stringify(vJson);
                    if (CStr2(pStr).trim() != '') {
                        Transfers.WriteFile('MainMenuOfflineMessages.json', null, pStr);
                        return true;
                    }
                } catch (e) {
                    console.trace(e.stack);
                }

                return false;
            };

            this.LoadOfflineMessageList = function () {

                var pStr;

                try {
                    pStr = Transfers.ReadFile('MainMenuOfflineMessageList.json');
                    if (CStr2(pStr).trim() == '')
                        return null;
                    return JSON.parse(pStr);
                } catch (e) {
                    console.trace(e.stack);
                    return null;
                }

            };

            this.SaveOfflineMessageList = function (vJson) {

                var pStr;

                try {
                    if (vJson == undefined)
                        return false;

                    pStr = JSON.stringify(vJson);
                    if (CStr2(pStr).trim() != '') {
                        Transfers.WriteFile('MainMenuOfflineMessageList.json', null, pStr);
                        return true;
                    }
                } catch (e) {
                    console.trace(e.stack);
                }

                return false;

            };

            return this;

        }; // window.PeopleMenu_offline = function() {

    });      // define

