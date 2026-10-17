function DataTable(vData) {
    // this is a class

    var mColumns; // array of DataColumns

    function SubNew() {

        var p;
        var pColumn;

        if (vData) {
            mColumns = [];
            for(p in vData.Columns) {
                pColumn = new DataColumn();
                pColumn.Name = p;
                mColumns.push(pColumn);
            };
        };

    }; // SubNew

}; // DataTable

function DataColumn() {
    
    this.Name = null;

}; // DataColumns