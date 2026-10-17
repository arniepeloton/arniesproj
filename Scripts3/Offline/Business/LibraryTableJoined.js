define([], function() {
    window.Business.LibraryTableJoined = new function() {
        this.GetByID = function(vID) {

            var pDR;
            var pSql;

            pSql = 'SELECT LibraryTableJoined.ID\n' + 
                ',LibraryTableJoined.LastUpdated\n';

            pSql += ' FROM private.LibraryTableJoined\n';

            pSql += ' WHERE LibraryTableJoined.ID = ' + ReadyString(vID);
             
            pDR = Sql.ExecuteRow(pSql);

             return pDR;
        };

        return this;
    };
});