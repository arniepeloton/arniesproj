(function ($) {

    $.fn.ProDataGrid = function (vOptions) {

        function ProDataGrid(vDiv, vOptions) {

            var mBase = vDiv.ProWebGrid(vOptions);
            this.Base = mBase;

            ProDataGrid.prototype.CheckAll = function (vColumnName, vCheck) {
                // PARAMS:
                //  vCheck = optional, if null or 1, check all checkbox on the specified columnname

                var pIndex;
                var p;

                pIndex = mBase.GetColumnIndex(vColumnName);
                if (vCheck == null)
                    vCheck = 1;
                mBase.BodyTable()
                    .children('tbody')
                    .children('tr')
                    .find('td:eq(' + pIndex + ')') // "children" will only result in 1 element. So, use "find"
                    .children('.DataCellDiv')
                    .children(':checkbox')
                    .prop('checked', vCheck)
                for (p = 0; p <= mBase.DT().Rows.length - 1; p++) {
                    mBase.DT().Rows[p][vColumnName] = vCheck;
                };
            }; // END CheckAll

            ProDataGrid.prototype.GetCheckedRows = function (vColumnName, vOutputColumnNames) {

                var p, pCol;
                var pOut;
                var pRow;
                var pCols;

                pOut = [];
                if (!vOutputColumnNames) {
                    vOutputColumnNames = [];
                    pRow = mBase.DT().Rows;
                    if (pRow.length == 0) return pOut;
                    pRow = pRow[0];
                    pCols = GetProperties(pRow);
                    for (pCol = 0; pCol <= pCols.length - 1; pCol++) {
                        vOutputColumnNames.push(pCols[pCol]);
                    };
                };

                for (p = 0; p <= mBase.DT().Rows.length - 1; p++) {
                    if (mBase.DT().Rows[p][vColumnName]) {
                        pRow = {};
                        for (pCol = 0; pCol <= vOutputColumnNames.length - 1; pCol++) {
                            pRow[vOutputColumnNames[pCol]] = mBase.DT().Rows[p][vOutputColumnNames[pCol]];
                        };
                        pOut.push(pRow);
                    };
                };

                return pOut;

            }; // END this.GetCheckedRows

            this.AfterRowActivate = mBase.AfterRowActivate;
            this.AfterRowDoubleClick = mBase.AfterRowDoubleClick;
            this.RowData = mBase.RowData;

        }; // END ProDataGrid

        return new ProDataGrid($(this), vOptions);

    }; // $.fn.ProDataGrid

})(jQuery);

