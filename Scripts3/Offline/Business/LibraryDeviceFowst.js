define([], function() {
    window.Business.LibraryDeviceFowst = new function(vParent) {
        this.ByDeviceFowst = function(vDeviceID, vFowstID) {
            
            var pDR;
            var pSql;

            pSql = 'SELECT ID\n' +
                ',PrintServer\n' +
                ' FROM LibraryDeviceFowst\n' +
                ' WHERE DeviceID = ' + ReadyString(vDeviceID) + '\n' +
                ' AND FowstID = ' + ReadyString(vFowstID);
            pDR = Sql.ExecuteRow(pSql);

            return pDR;

        };
        return this;
    };
});