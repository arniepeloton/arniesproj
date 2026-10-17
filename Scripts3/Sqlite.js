require(['common'
    ], function() {

    $(function () {

        (function Init() {
            if (CDec2(GetUrlParameters().IsMobile) == 0) {
                location = location.origin + location.pathname + '?' + $.param(
                    $.extend(GetUrlParameters(), 
                        {
                            IsMobile: 1
                        }
                    )
                );
                return;
            } else {
                butRefreshDbObjects2_Click();
                document.addEventListener('keyup', function(e) {
                    if (e.ctrlKey 
                        && (e.key == 'q'))
                        Run();
                });
                $('#txtSql').val(localStorage.SqliteSql);
            }
        })();

        $('#butRun').click(function() {
            Run();
        });

        function Run() {
            var pSql;
            var pDT;
            var pTable; // html table
            
            pSql = $('#txtSql').val();
            if (pSql.trim() == '') {
                MessageBox('SQL must not be blank');
                return;
            };
            localStorage.SqliteSql = pSql;

            $('#divElapsedSeconds').hide();
            try {
                pDT = Sql.Execute(pSql);
                $('#divElapsedSeconds').text('Elapsed time: ' + pDT.ElapsedMilliseconds + ' milliseconds').show();
            } catch (e) {
                $('#divResult').text(e);
                return;
            };

            pTable = CreateTable(pDT, {
                
            });

            $('#divResult')
                .html('')
                .append(pTable);
            pTable.table(); // required to make the table resposive on small displays
        }

        $('#butRefreshDbObjects2').click(butRefreshDbObjects2_Click);
        function butRefreshDbObjects2_Click() {

            var divDbsContainer = $('#divDbsContainer');
            var pSchema;

            divDbsContainer.html('');
            $.mobile.showPageLoadingMsg();

            setTimeout(function() {
                try {
                    pSchema = Sql.GetSchema();
                    pSchema = JSON.parse(pSchema);
                    for (var p = 0; p <= pSchema.Databases.length - 1; p++) {
                        PopDb(pSchema.Databases[p]);
                    };
                    if (divDbsContainer.attr('data-role') == 'collapsible-set')
                        divDbsContainer.collapsibleset("refresh");
                } catch (e) {
                    MessageBox(e);
                } finally {
                    $.mobile.hidePageLoadingMsg();
                };
            }, 250);

            function PopDb(vDb) {
                
                var pDiv;

                pDiv = $('<div>');
                pDiv.append(
                    $('<h3>')
                        .text(vDb.Name)
                ).append(
                    $('<h4>')
                        .text(vDb.Name)
                );
                PopTables(pDiv, vDb);
                pDiv.collapsible();
                divDbsContainer.append(pDiv);

            }; // PopDb

            function PopTables(vDiv, vDb) {
                
                var pList;
                var pListItem;

                pList = $('<ol>');
                for (var p = 0; p <= vDb.Tables.length - 1; p++) {
                    pListItem = $('<li>');
                    PopTable(pListItem, vDb.Tables[p]);
                    pList.append(pListItem);
                };
                vDiv.append(pList);

            }; // PopTables

            function PopTable(vListItem, vTable) {

                var pList;
                var pListItem;

                vListItem.append(
                    $('<span>')
                        .text(vTable.Name)
                );
                pList = $('<ol>');
                for (var p = 0; p <= vTable.Columns.length - 1; p++) {
                    pListItem = $('<li>');
                    pListItem.text(vTable.Columns[p]);
                    pList.append(pListItem);   
                };
                vListItem.append(pList);

            }; // PopTable
        }; // butRefreshDbObjects2_Click

    }); // $(function () {
}); // require(['common'