define(['common'], function () {
    CreateStringObject('App.model.BOMMain_Offline', null, function () {
        var pOut = {};
        var pSql = '';
        var pIU;
        var pDT;

        pOut.GetAccessRights = function () {
            pSql = 'SELECT DISTINCT jobper.PermissionID, per.Description ' +
                    ' FROM LibraryProjectJobRolePermission jobper ' +
                    ' LEFT JOIN (SELECT * FROM LibraryOrganizationEmployeeFOWST WHERE PeopleID = ' + ReadyString(Web.PeopleUserID()) +
                    '               AND FowstID = ' + ReadyString(sessionStorage.ProjectID) +
                    '           ) AS empfowst ON empfowst.JobRoleID = jobper.JobRoleID ' +
                    ' LEFT JOIN LibraryProjectJobRole jobrole ON jobrole.JobRoleID = jobper.JobRoleID ' +
                    ' LEFT JOIN LibraryProjectPermission per ON jobper.PermissionID = per.ID ' +
                    ' WHERE jobper.ProjectID = ' + ReadyString(sessionStorage.ProjectID) +
                    ' AND empfowst.JobRoleID = jobper.JobRoleID ' +
                    ' AND jobrole.JobRoleID = jobper.JobRoleID ' +
                    ' AND jobrole.ProjectID = ' + ReadyString(sessionStorage.ProjectID)

            pDT = Sql.Execute(pSql);

            return pDT;
        }

        return pOut;
    })
})