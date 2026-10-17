define([], function() {
    window.SystemDefault = function(vParent) {
        this.GetSystemDefault = function(vDescription) {
            
            var pDT;
            var pSql;

            pSql = 'SELECT DefaultValue' +
                ' FROM SystemDefault' +
                ' WHERE Description = ' + ReadyString(vDescription);
            pDT = Sql.Execute(pSql);
            if (pDT == undefined) return '';
            if (pDT.Rows.length == 0) return '';
            return pDT.Rows.DefaultValue;

        };

        this.CustomerOrganizationID = function() {
            return this.GetSystemDefault("Customer Organization ID");
        };

        return this;
    };
});