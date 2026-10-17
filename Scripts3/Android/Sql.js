define([], function () {

    window.Sql = {
        Delimeter: '/* LES2296HN6 this random string is an Sqlite delimeter HM5UCPHFBU */',

        Execute: function (vSql) {
            var pTimeStart = new Date();
            var pDT = Js.Execute({
                command: 'Sql.Execute',
                Sql: vSql
            });
            var pTimeEnd = new Date();
            var pElapsedSeconds = (pTimeEnd - pTimeStart) / 1000;
            if (pElapsedSeconds > 10)
                console.log('Sql.Execute elapsed seconds: ' + pElapsedSeconds + '. ref: jsYSF7EUKDD5!');
            if (vSql.toLowerCase().indexOf('LibraryPIMProductMeasurement'.toLowerCase()) != -1)
                console.log('Sql.Execute LibraryPIMProductMeasurement. ref: jsHKGZ6LBSKU!');
            if (vSql.toLowerCase().indexOf('FUNCTION'.toLowerCase()) != -1)
                console.log('Sql.Execute FUNCTION . ref: jsZRPP6XYG5Q!');
                
            if (pDT == undefined)
                return;

            try {
                if (pDT.Error == undefined) {
                    return pDT;
                };
            } catch (e) {
                alert('Error in SqlExecute');
                return;
            };

            if (pDT.Error != undefined) {
                console.error(pDT.Error);
                throw new Error(pDT.Error);
            }
        }, // Execute

        ExecuteRow: function (vSql) {
            var pDT = Js.Execute({
                command: 'Sql.ExecuteRow',
                Sql: vSql
            });

            if (pDT == undefined)
                return;

            try {
                if (pDT.Error == undefined) {
                    return pDT;
                };
            } catch (e) {
                alert('Error in SqlExecute');
                return;
            };

            if (pDT.Error != undefined)
                console.error(pDT.Error);
            throw new Error(pDT.Error);

        }, // Execute

        ExecuteScalar: function (vSql) {
            return Js.Execute({
                command: 'Sql.ExecuteScalar',
                Sql: vSql
            });

        }, // ExecuteScalar

        ExecuteSql: function (vSql) {
            var pDT = Js.Execute({
                command: 'Sql.ExecuteSql',
                Sql: vSql
            });

            if (pDT == undefined)
                return;

            try {
                if (pDT.Error == undefined) {
                    return pDT;
                };
            } catch (e) {
                alert('Error in ExecuteSql');
                return;
            };

            if (pDT.Error != undefined)
                console.error(pDT.Error);
            throw new Error(pDT.Error);

        }, // ExecuteSql

        BeginTransaction: function (vSql) {
            var pDT = Js.Execute({
                command: 'Sql.BeginTransaction',
                Sql: vSql
            });

        }, // BeginTransaction

        CommitTransaction: function (vSql) {
            var pDT = Js.Execute({
                command: 'Sql.CommitTransaction',
                Sql: vSql
            });

        }, // CommitTransaction

        RollbackTransaction: function (vSql) {
            var pDT = Js.Execute({
                command: 'Sql.RollbackTransaction',
                Sql: vSql
            });

        }, // RollbackTransaction
        AttachDatabase: function (vDbName, vAlias) {
            return Js.Execute({
                command: 'Sql.AttachDatabase',
                DbName: vDbName,
                Alias: vAlias || 'private'
            });
        }, // AttachDatabase
        OpenDatabase: function (vName) {
            return Js.Execute({
                command: 'Sql.OpenDatabase',
                Name: vName
            });
        }, // OpenDatabase
        OpenOrCreateDatabase: function (vName) {
            return Js.Execute({
                command: 'Sql.OpenOrCreateDatabase',
                Name: vName
            });
        }, // OpenOrCreateDatabase
        DownloadAndApplySchema: function (vOptions) {

            if (vOptions.Error == undefined) {
                vOptions.Error = function () {
                    Android.Toast('An error occured while doing DownloadAndApplySchema. This error message could use some details.');
                };
            };

            var pCallbacks = {
                Success: vOptions.Success,
                Error: vOptions.Error
            };

            vOptions.Command = 'DownloadAndApplySchema';
            mobile.CallWithCallback(vOptions, pCallbacks);

        }, // DownloadAndApplySchema
        GetCurrentDatabase: function () {
            return Js.Execute({
                command: 'Sql.GetCurrentDatabase'
            });
        }, // GetCurrentDatabase
        ReopenDatabase: function () {
            return Js.Execute({
                command: 'Sql.ReopenDatabase'
            });
        }, // ReopenDatabase
        GetSchema: function () {
            return Js.Execute({
                command: 'Sql.GetSchema'
            });
        }, // GetCurrentDatabase
        IsExist: function (vDbName) {
            var p = Js.Execute({
                command: 'Sql.IsExist',
                DbName: vDbName
            });
            return CBool(p);
        }, // IsExist
        DeleteIfExist: function (vDbName) {
            return Js.Execute({
                command: 'Sql.DeleteIfExist',
                DbName: vDbName
            });
        }, // DeleteIfExist
        InTransaction: function () {
            return Js.Execute({
                command: 'Sql.InTransaction'
            });
        }, // InTransaction
        GetDatabaseFiles: function () {
            return Js.Execute({
                command: 'Sql.GetDatabaseFiles'
            });
        }, // InTransaction
        AnalyzeAutoIndex: function (vSql) {
            return Js.Execute({
                command: 'Sql.AnalyzeAutoIndex',
                Sql: vSql
            });
        }, // END AutoIndexRecommendations
        CloseConnection: function () {
            return Js.Execute({
                command: 'Sql.CloseConnection'
            });
        }, // END CloseConnection
        ExportableDataCount: function () {
            return Js.Execute({
                command: 'Sql.ExportableDataCount'
            });
        }, // END CloseConnection
        CopyToTestDb: function () {
            return Js.Execute({
                command: 'Sql.CopyToTestDb'
            });
        }, // END CloseConnection
        UploadableDatabaseFiles: function() {
            return Js.Execute({
                command: 'Sql.UploadableDatabaseFiles'
            });
        },
        UploadDatabase: function(vFilenames) {
            return Js.Execute({
                command: 'Sql.UploadDatabase',
                Filenames: vFilenames.Filenames
            });
        },
        TableExists: function(vTableName, vDatabaseName) {
            var pSql;
            if (CStr2(vDatabaseName).trim() != '')
                pSql = ReadyString(vDatabaseName) + ".";
            pSql = "SELECT 1 FROM " + pSql + "sqlite_master WHERE type = 'table' AND lower(name) = " + ReadyString(vTableName.toLowerCase());
            var pDT = Sql.Execute(pSql);
            return pDT.Rows.length > 0;
        },
        CreateTableFromColumns: function(vTableName, vDatabaseName, vJson) {
            return Js.Execute({
                command: 'Sql.CreateTableFromColumns',
                TableName: vTableName,
                DatabaseName: vDatabaseName,
                Json: vJson
            });
        }
    }; // Sql

});  // define