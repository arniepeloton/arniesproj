require(['common'
    ], function() {

    $(function () {
        var ctrInsert = 0, rawData;

        $('#txtSqlPassword').val(sessionStorage.SqlPassword);
        $('#txtSql').val(localStorage.sql);

        $('#butRun').click(function() {
            var pSql = $('#txtSql').val();
            if (pSql.trim() == '') {
                MessageBox('SQL must not be blank');
                return;
            };

            var pPassword = $('#txtSqlPassword').val();
            if (pPassword.trim() == '') {
                MessageBox('Sql password must not be blank');
                return;
            }
            sessionStorage.SqlPassword = pPassword;

            var pData = {
                password: 'jsK73RJCWMWW',
                sql: pSql,
                password: pPassword,
                timeout: $('#txtTimeout').val()
            };

            localStorage.sql = pSql;
            $('#divElapsedSeconds').text('Executing query...');
            AjaxSend({
                url: '/WebSql/ExecuteSql',
                data: pData,
                success: function(vData) {
                    $('#divElapsedSeconds').text('Elapsed time: ' + vData.ElapsedMilliseconds + ' milliseconds').show();
                    if (vData.Error) {
                        $('#divResult').text(vData.Error);
                        return;
                    }
                    if (vData.Result) {
                        var pTable = CreateTable(vData.Result, {});
                        $('#divResult')
                            .html('')
                            .append(pTable);
                        pTable.table(); // required to make the table resposive on small displays
                    } else if (vData.AffectedRecords) 
                        $('#divResult')
                            .text('Affected records: ' + AccountingFormat(vData.AffectedRecords, 0));
                },
                error: function(vData) {
                    $('#divResult').text(vData);
                    $('#divElapsedSeconds').text('Error');
                },
                complete: function() {
                }
            });
        });

        $('#butInsert').click(function () {
            rawData = JSON.parse($('#txtSql').val());
            InsertRawData();
        });

        function InsertRawData() {
            var pSql = $('#txtSql').val();
            if (pSql.trim() == '') {
                MessageBox('SQL must not be blank');
                return;
            };

            var pPassword = $('#txtSqlPassword').val();
            if (pPassword.trim() == '') {
                MessageBox('Sql password must not be blank');
                return;
            }
            sessionStorage.SqlPassword = pPassword;

            var pData = {
                password: 'jsK73RJCWMWW',
                sql: 'USE MobizABC; ' + generateInsertQuery(rawData[ctrInsert]),
                password: pPassword,
                timeout: $('#txtTimeout').val()
            };

            localStorage.sql = pSql;
            $('#divElapsedSeconds').text('Executing query...');
            AjaxSend({
                url: '/WebSql/ExecuteSql',
                data: pData,
                success: function (vData) {
                    $('#divElapsedSeconds').text('Elapsed time: ' + vData.ElapsedMilliseconds + ' milliseconds').show();
                    if (vData.Error) {
                        $('#divResult').text(vData.Error);
                        return;
                    }
                    if (vData.Result) {
                        var pTable = CreateTable(vData.Result, {});
                        $('#divResult')
                            .html('')
                            .append(pTable);
                        pTable.table(); // required to make the table resposive on small displays
                    } else if (vData.AffectedRecords) {
                        $('#divResult').text('Affected records: ' + AccountingFormat(vData.AffectedRecords, 0));
                        ctrInsert += 1;
                        if (rawData[ctrInsert] != undefined) {
                            InsertRawData();
                        } else {
                            ctrInsert = 0;
                        }
                    }
                },
                error: function (vData) {
                    $('#divResult').text(vData);
                    $('#divElapsedSeconds').text('Error');
                },
                complete: function () {
                }
            });
        }

        function generateInsertQuery(data, tableName = 'CacheInvoicesDaily') {
            const columns = [];
            const values = [];
            let hasID = false;

            for (const key in data) {
                columns.push(`[${key}]`);

                if (key == 'ID') hasID = true;

                const value = data[key];

                // Check if value is a valid number and doesn't contain letters
                if (!isNaN(value) && value.trim() !== "" && /^[0-9\.\-]+$/.test(value)) {
                    values.push(value); // No quotes for numbers
                } else {
                    // Escape single quotes for SQL
                    const escapedValue = value.replace(/'/g, "''");
                    values.push(`'${escapedValue}'`);
                }
            }

            if (!hasID) {
                columns.push('[ID]');
                values.push(`'` + NewID() + `'`);
            }            

            const columnString = columns.join(', ');
            const valueString = values.join(', ');

            const sql = `INSERT INTO ${tableName} (${columnString}) VALUES (${valueString});`;
            return sql;
        }
    }); // $(function () {
}); // require(['common'