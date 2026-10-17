define([], function() {
    window.Business.LibraryFowstBin = new function(vParent) {
        if (window.DebugMode) {
            
        } // END if (window.DebugMode) {

        this.UpdateIsActive = function(vID, vIsActive) {

            var pSql;

            pSql = 'UPDATE LibraryDevice' +
                ' SET IsActive = ' + (CBool(vIsActive) ? 1 : 0) +
                ' WHERE ID = ' + ReadyString(vID);
            pSql = Sql.ExecuteSql(pSql);

        };

        this.GetByFowstID = function(vFowstID) {
            
            var pSql;
            var pDT;

            /* removed from SELECT because to slow (4 seconds)
             ' ,(SELECT ID ' + '\n' + 
                '   FROM LibraryFowstBin t' + '\n' + 
                '   WHERE t.Name = LibraryFowstBin.Name' + '\n' + 
                '    LIMIT 1' + '\n' + 
                ' ) AS ID' + '\n' + 
            */

            pSql = ' SELECT DISTINCT Name' + '\n' + 
                ' FROM LibraryFowstBin' + '\n' + 
                ' WHERE IsActive = 1' + '\n' + 
                ' AND FowstID = ' + ReadyString(vFowstID) + '\n' + 
                ' AND Name <> \'Unnamed Bin\'' + '\n' + 
                ' ORDER BY Name\n' +
                ' LIMIT 100' + '\n';
            pDT = Sql.Execute(pSql);

            return pDT;

        }; // END GetByFowstID

        this.GetByName = function(vName, vFowstID) {
            
            var pSql;
            var pDT;

            if (vFowstID != null)
                pSql = ' AND FowstID = ' + ReadyString(vFowstID) + '\n';
            pSql = 'SELECT ID ' + '\n' + 
                ' FROM LibraryFowstBin' + '\n' + 
                ' WHERE Name = ' + ReadyString(vName) + '\n' + 
                pSql +
                ' LIMIT 1' + '\n';
            pDT = Sql.Execute(pSql);

            return pDT;

        }; // END GetByName

        return this;
    };
});