define([], function() {
    window.Business.LibraryDevice = new function(vParent) {
        this.UpdateIsActive = function(vID, vIsActive) {

            var pSql;

            pSql = 'UPDATE LibraryDevice' +
                ' SET IsActive = ' + (CBool(vIsActive) ? 1 : 0) +
                ' WHERE ID = ' + ReadyString(vID);
            pSql = Sql.ExecuteSql(pSql);

        };

        this.ById = function(vID) {
            
            var pDR;
            var pSql;

            pSql = 'SELECT IsActive' +
                ',PhoneName' +
                ',PhoneModel' +
                ',UUID' +
                ',DeviceNo' +
                ' FROM LibraryDevice' +
                ' WHERE ID = ' + ReadyString(vID);
            pDR = Sql.ExecuteRow(pSql);

            return pDR;

        };

        this.HandleInactive = function(vDontQuery) {

            var pIsActive;
            var pDR;

            if (CBool(vDontQuery) == false) {
                pDR = this.ById(Web.DeviceID());
                if (pDR != null) 
                    pIsActive = pDR.IsActive;
            }
                
            if (CBool(pIsActive) == false)
                MessageBox('This device that you are using is not active.', 'Inactive', null, {
                    OnClose: function() {
                        location = MergeUrlParameters({
                            IsMobile: IsMobile ? 1 : 0
                        }, '/People/PeopleMenu');
                    }
                });            

            return CBool(pIsActive);
        };

        this.LoadBirAccreditation = function() {
            var pSql;
            var pAccreditation, pMobizAppVersion;

            pSql = ' SELECT Accreditation, MobizAppVersion' + '\n' + 
                ' FROM LibraryDevice' + '\n' + 
                ' WHERE UUID = ' + ReadyString(Web.DeviceSerial());
            pAccreditation = Sql.Execute(pSql).Rows[0].Accreditation;
            pMobizAppVersion = Sql.Execute(pSql).Rows[0].MobizAppVersion;

            proj.LoadBirAccreditation(pAccreditation, pMobizAppVersion);
        };

        this.GetMobizAppVersion = function() {
            var pSql = 'SELECT MobizAppVersion\n' +
                ' FROM LibraryDevice\n' +
                ' WHERE ID = ' + ReadyString(Web.DeviceID());
            var pOut = Sql.ExecuteScalar(pSql);
            pOut = CInt2(pOut);
            return pOut;
        };
        return this;
    };
});