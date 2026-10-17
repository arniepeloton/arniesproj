define(['common'], function() {

    window.CsvWriter = function (vZipFilename, vInternalFilename) {
        var mID;
        var mZipOutputDirectory;
        var mCount = 0;

        (function Init() {

            if (CStr2(vZipFilename).trim() == '')
                throw new Error('CsvWriter error: vZipFilename must not be blank');

            if (CStr2(vInternalFilename).trim() == '')
                throw new Error('CsvWriter error: vInternalFilename must not be blank');

        })(); // END Init

        this.Write = function (vDR) {

            var pID;

            if (mID == undefined)
                pID = NewID();
            else
                pID = mID;

            Transfers.CsvWrite({
                ID: pID,
                ZipFilename: vZipFilename,
                InternalFilename: vInternalFilename,
                Content: JSON.stringify(vDR)
            });
            mCount++;
            mID = pID;

        }; // END Write

        this.WriteTable = function(vDT) {

            var pDR;

            if ((vDT == undefined)
                || (vDT.Rows.length == 0))
                return;

            for (var pRowIndex = 0; pRowIndex <= vDT.Rows.length - 1; pRowIndex++) {
                pDR = {};
                for (var pColumnName in vDT.Columns) {
                    if (vDT.Columns.hasOwnProperty(pColumnName)) {
                        pDR[pColumnName] = vDT.Rows[pRowIndex][pColumnName];
                    };
                }; // for pColumn
                this.Write(pDR);
            }; // for pRowIndex
            this.Commit();

        }; // END WriteTable

        this.Commit = function() {
            
            var pID;

            if (mCount == 0) 
                return;
            if (mID == undefined)
                pID = NewID();
            else
                pID = mID;

            Transfers.CsvCommit(mID);
            mID = pID;

        };

        this.FinalCommit = function() {

            if (mID == undefined)
                mZipOutputDirectory = Transfers.CsvFinalCommit(null, vZipFilename);
            else
                mZipOutputDirectory = Transfers.CsvFinalCommit(mID);

        };

        this.Delete = function() {

            Transfers.CsvDelete(vZipFilename);

        };

        this.GetID = function() {

            return mID;

        };

        this.GetZipOutputDirectory = function() {
            
            return mZipOutputDirectory;

        };

        this.Upload = function(vOptions) {

            if (vOptions.SourceDirectoryName != undefined)
                mZipOutputDirectory = vOptions.SourceDirectoryName;
            if (CStr2(mZipOutputDirectory).trim() == '')
                throw 'Please call FinalCommit before calling Upload';
            else
                Transfers.Upload($.extend(vOptions, {SourceDirectoryName: mZipOutputDirectory}));

        };

        this.Count = function() {
            
            return mCount;

        };

        this.AddFile = function(vSourceFilename, vDestinationFilename) {
            
            var pID;

            if (mID == undefined)
                pID = NewID();
            else
                pID = mID;

            Transfers.CsvAddFile({
                ID: pID,
                ZipFilename: vZipFilename,
                InternalFilename: vInternalFilename,
                SourceFilename: vSourceFilename,
                DestinationFilename: vDestinationFilename || vSourceFilename
            });
            mCount++;
            mID = pID;
        };

        return this;

    }; // END CsvWriter
}); // define