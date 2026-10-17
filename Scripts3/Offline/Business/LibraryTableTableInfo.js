define([], function() {
    window.Business.LibraryTableTableInfo = new function() {
        this.GetByID = function(vID) {

            var pDR;
            var pSql;

            pSql = 'SELECT LibraryTableTableInfo.ID\n' + 
                ',LibraryTableTableInfo.LastUpdated\n';

            pSql += ' FROM private.LibraryTableTableInfo\n';

            pSql += ' WHERE LibraryTableTableInfo.ID = ' + ReadyString(vID);
             
            pDR = Sql.ExecuteRow(pSql);

             return pDR;
        };

        return this;
    };
});