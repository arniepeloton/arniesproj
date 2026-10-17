require(['common'
    ], function() {

    $(function () {

        console.debug('Welcome to Sqlite Android');

        $('#butInsertUpdate').click(function() {

            var pIU = new InsertUpdate('myTable');
            var pSql;

            pIU.Add('Username', ReadyString('this is my \'new\' username'));
            pSql = pIU.GetInsert();

            $('#txtInsertUpdate').val(pSql);

            if ($('#txtExecSql').val() == '')
                $('#txtExecSql').val(pSql);

        }); // butInsertUpdate.Click

        $('#butExecSql').click(function() {

            console.debug('butExecSql.click');
            if (window.AndroidJs == undefined) {
                alert('This function is for Android only');
                return;
            };

            var pSql = $('#txtExecSql').val();
            var pInt;

            pInt = AndroidJs.ExecuteSql(pSql);
            alert('Affected records: ' + pInt);

        }); // butExecSql.Click

        $('#butServerToLocal').click(function() {

            console.debug('Fetching records...');
            AjaxSend({
                url: '/Browser/LibraryCountries',
                success: function(vData) {
                    try {
                        console.debug('saving to android');
                        var pStr = JSON.stringify(vData.Results);
                        AndroidJs.CreateTableWithData('LibraryCountries', pStr);
                    } catch (e) {
                        console.error(e.message, e);
                    };
                }
            });

        }); // butServerToLocal.Click

        $('#butFromLocal').click(function() {

            console.debug('Fetching records from local DB');
            $('#divTable').html('');
            var pSql = AndroidJs.Execute('SELECT * FROM LibraryCountries ORDER BY Name');
            // $('#divFromLocal').text(pSql);
            
            var pTable = CreateTable(JSON.parse(pSql));
            $('#divFromLocal').append(pTable);
            pTable.table();
            pTable.trigger('create');
                    
        }); // butFromLocal.Click        
    });
});