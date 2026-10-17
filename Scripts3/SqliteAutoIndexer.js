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
            };

        })();

        $('#butAnalyze').click(function() {

            var pSql;
            var pDT;
            var pTable; // html table
            var pResult;

            try {
                $('#divResult').hide();
                pSql = $('#txtSql').val();
                if (pSql.trim() == '') {
                    MessageBox('SQL must not be blank');
                    return;
                };

                $('#divElapsedMillieseconds').hide();
                try {
                    if ($('#chkExecute').prop('checked')) {
                        pDT = Sql.Execute(pSql);
                        $('#divElapsedMilliseconds').text('Elapsed time: ' + $.format.number(pDT.ElapsedMilliseconds, '#,###') + ' millisecond[s]').fadeIn(1000);

                        pTable = CreateTable(pDT, {
                
                        });

                        $('#divResult')
                            .html('')
                            .append(pTable);
                        pTable.table(); // required to make the table resposive on small displays
                        $('#divResult').fadeIn(1000);    
                    }
                } catch (e) {
                    $('#divResult').text(e).fadeIn(1000);    
                    return;
                };

                pResult = Sql.AnalyzeAutoIndex(pSql);
                if (pResult == undefined) {
                    MessageBox('No result');
                    return;
                }

                pSql = '';
                for (var p = 0; p <= pResult.ExistingIndices.length - 1; p++) {
                    pSql += HtmlEncode(pResult.ExistingIndices[p]) + ';<br />';
                }
                $('#divExisting').html(pSql);

                pSql = '';
                if (pResult.NeededIndices.length > 0) {
                    for (var p = 0; p <= pResult.NeededIndices.length - 1; p++) {
                        pSql += HtmlEncode(pResult.NeededIndices[p]) + ';<br />';
                    }
                } else
                    pSql = 'All possible auto indices have been applied.';
                $('#divNeeded').html(pSql);

                pSql = '';
                if (pResult.Recommendations.length > 0) {
                    for (var p = 0; p <= pResult.Recommendations.length - 1; p++) {
                        pSql += pResult.Recommendations[p] + ';\r\n' + Sql.Delimeter + '\r\n\r\n';
                    }
                } else
                    pSql = '/* All possible recommendations have been applied. */\r\n';

                if (pResult.IndexNames.length > 0) {
                    pSql += '/* This query uses existing indices. */\r\n';
                    pSql += '/* You may want to drop the following indices to measure query duration with and without indices. */\r\n\r\n';
                    for (var p = 0; p <= pResult.IndexNames.length - 1; p++)
                        pSql += 'DROP INDEX ' + pResult.IndexNames[p] + ';\r\n' + Sql.Delimeter + '\r\n\r\n';
                } else
                    pSql += '/* this query do not use any existing index */';
                $('#txtRecommendations').val(pSql).keyup();;
            } catch (e) {
                MessageBox(e.message);
            }

        }); // END $('#butAnalyze').click(function() {

        $('#butApplyRecommendations').click(function() {
            
            var pSql;

            pSql = $('#txtRecommendations').val();
            if (pSql.trim() == '') {
                MessageBox('Recommendations must not be blank');
                return;
            };

            $('#divElapsedMillieseconds').hide();
            $('#divResult').hide();
            try {
                Sql.ExecuteSql(pSql);
                // $('#divElapsedSeconds').text('Elapsed time: ' + pDT.ElapsedSeconds + ' second[s]').fadeIn(1000);
            } catch (e) {
                $('#divResult').text(e).show();
                $('html, body').animate({
                    scrollTop: $('#divResult').offset().top + 'px'
                }, 'fast');
                $('#divResult').fadeIn(1000);
                return;
            };

        }); // END $('#butApplyRecommendations').click(function() {

    }); // $(function () {
}); // require(['common'