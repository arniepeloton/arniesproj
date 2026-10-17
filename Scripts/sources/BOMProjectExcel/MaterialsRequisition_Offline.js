define(['common'], function () {
    CreateStringObject('App.model.MaterialsRequisition_Offline', null, function () {
        var pOut = {};
        var pSql = '';
        var pIU;
        var pDT;

        pOut.GetProjectBudgetByProject = function (data) {
            pSql = 'SELECT * FROM LibraryProjectBudget WHERE OrgID = ' + ReadyString(data.OrgID) +
                    ' AND FOWSTID = ' + ReadyString(data.ProjectID) +
                    ' AND ProjectOwnerOrgPersonID IS NULL AND ProjectOwnerFowstID IS NULL' +
                    ' ORDER BY RowNumber ASC';

            pDT = Sql.Execute(pSql);

            return pDT;
        }

        pOut.SaveMaterialsRequisition = function (data, reqVal) {
            var reqID = NewID();
            var reqNo = 'MR ' + $.format.date(new Date(), 'MMdd-yyyy-HHmm-ss');
            pIU = new InsertUpdate("LibraryProjectRequisition");

            pIU.Add('ID', ReadyString(reqID));
            pIU.Add('ProjectID', ReadyString(sessionStorage.ProjectID));
            pIU.Add('RequisitionNumber', ReadyString(reqNo));
            pIU.Add('RequestedDate', proj.SqliteNow);
            pIU.Add('RequestedBy', ReadyString(sessionStorage.PeopleID));
            pIU.Add('RequisitionValue', ReadyString(reqVal));            
            pIU.Add('Status', ReadyString('New'));

            pSql = pIU.GetInsert();
            Sql.ExecuteSql(pSql);

            for (var i = 0; i < data.length; i++) {
                pIU = new InsertUpdate("LibraryProjectMaterialsRequisition");

                pIU.Add('OrgID', ReadyString(data[i].OrgID));
                pIU.Add('ProjectID', ReadyString(data[i].ProjectID));
                pIU.Add('ParentID', ReadyString(data[i].ParentID));
                pIU.Add('ProductID', ReadyString(data[i].DescriptionID));;
                pIU.Add('VendorID', ReadyString(data[i].VendorID));
                pIU.Add('RequisitionID', ReadyString(reqID));
                pIU.Add('RequestedValue', ReadyString(data[i].RequestedValue));
                pIU.Add('ApprovedRequestedQty', ReadyString(0));
                pIU.Add('LastUpdated', proj.SqliteNow);

                if (data[i].ID == undefined) {
                    pIU.Add('ID', ReadyString(NewID()));
                    pSql = pIU.GetInsert();
                }

                Sql.ExecuteSql(pSql);
            }

            return 'saved';
        }

        return pOut;
    })
})