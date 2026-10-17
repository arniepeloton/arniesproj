require([
    'common'
    ], function () {
        $(function () {
            getTeams();
        })

        function getTeams() {

            AjaxSend({
                url: 'crmsetup/getTeams',
                success: function (vData) {
                    LoadResult(vData.data.Rows);
                },
                error: function (vError) {
                    var pDummy;
                }
            });
        }

        function LoadResult(vRows) {

            var pLV = $('#lstTeams');
            var pItem;

            pLV.html('');
            for (var p = 0; p <= vRows.length - 1; p++) {
                pItem = $('<li>')
                    .append($('<a>')
                    .attr('href', '#')
                    .attr('ID', vRows[p].ID)
                    .attr('data', vRows[p].TeamName)
                    .click(function (e) {
                        e.preventDefault();
                        assignTeamID($(this).attr("ID"), $(this).attr("data"));
                    })
                    .append($('<h3>')
                        .text(vRows[p].TeamName)
                    )
                );
                pLV.append(pItem);
            };

            pLV.listview('refresh');
        };

        function assignTeamID(TeamID, TeamName) {
            var option = {
                m_TeamID: TeamID,
                m_TeamName: TeamName
            };

            AjaxSend({
                url: 'crmsetup/assignTeamID/',
                data: option,
                success: function (vData) {
                    window.open("Teams", "_self");
                },
                error: function (vError) {
                    var pDummy;
                }
            });
        }

    });