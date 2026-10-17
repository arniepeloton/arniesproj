define(['common'], function() {

    window.CsvReader = function (vPath) {
        var mFolderName;
        var mRowIndex;
        var mFileIndex;
        var mPath = vPath;
        var mDT;
        var mMyClass = this;
                    
        this.Open = function (vFolderName) {
                    
            mFolderName = vFolderName;
            mRowIndex = 0;
            mFileIndex = 1;
            mDT = null;
                        
        }; // END Open

        this.ReadTable = function() {
                    
            var pFilename = mPath + '/' + mFolderName + '/' + $.format.number(mFileIndex, '0000n') + '.csv';
            var pStr = Transfers.ReadFile(pFilename);
            if (CStr2(pStr) != '') {
                mDT = CsvToDataTable(pStr);
                mFileIndex++;
                mRowIndex = 0;
                return mDT;
            };
            mDT = null;

        }; // END ReadTable

        this.ReadLine = function() {
                        
            if (mDT == undefined)
                mDT = mMyClass.ReadTable();
            if (mDT != undefined) {
                if (mRowIndex <= mDT.Rows.length - 1) {
                    mRowIndex++;
                    return mDT.Rows[mRowIndex - 1];
                } else {
                    mDT = null;
                    return mMyClass.ReadLine();
                };
                                
            };

        }; // END ReadLine

        return this;

    }; // END CsvReader
}); // define