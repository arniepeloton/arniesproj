require([
    'common'
    ], function () {
        $(function () {
            getTeamMembers();

            $("#txtEmployee").keydown(function (e) {
                if (e.keyCode == 13) {
                    window.open("../ListOfEmployees?EmployeeName=" + $("#txtEmployee").val(), "_self");
                }
            });

            $("#btnAdd").click(function () {
                window.open("../ListOfEmployees?EmployeeName=" + $("#txtEmployee").val(), "_self");
            });

            $("#btnNext").click(function () {
                window.open("../SalesQuotaPerMember/", "_self");
            });

        })

        var IsLeaderClick = 0;

        function getTeamMembers() {

            AjaxSend({
                url: 'crmsetup/getTeamMembers/',
                success: function (vData) {
                    LoadResult(vData.data.Rows);
                },
                error: function (vError) {
                    var pDummy;
                }
            });
        }

        function LoadResult(vRows) {

            var pLV = $('#lstTeamMembers');
            var pItem;
            var IsLeaderHtml = "";

            pLV.html('');
            for (var p = 0; p <= vRows.length - 1; p++) {
                if (vRows[p].IsLeader == 1) {
                    IsLeaderHtml = 'ui-li-count ui-icon ui-icon-star'
                }
                else {
                    IsLeaderHtml = 'ui-li-count ui-icon ui-icon-gear'
                }

                pItem = $('<li>')
                    .append($('<a>')
                    .attr('href', '#')
                    .attr('ID', vRows[p].ID)
                    .attr('data', vRows[p].Name)
                    .click(function (e) {
                        //                        e.preventDefault();
                        window.open("../RemoveEmployee?EmployeeID=" + $(this).attr("ID"), "_self");
                    })
                    .append($('<h3>')
                        .text(vRows[p].Name)
                    )
                    .append($('<a>')
                        .attr('ID', vRows[p].ID)
                        .attr('href', '#')
                        .addClass(IsLeaderHtml)
                        .click(function (e) {
                            assignAsTeamLeader($(this).attr("ID"));
                            return false;
                        })
                    )
                );
                pLV.append(pItem);
                IsLeaderHtml = "";
            };

            pLV.listview('refresh');
        };

        function assignAsTeamLeader(EmployeeID) {
            option = {
                EmpID: EmployeeID
            }

            AjaxSend({
                url: 'crmsetup/assignAsTeamLeader/',
                data: option,
                success: function (vData) {
                    window.open('../TeamMembers/', '_self');
                },
                error: function (vError) {
                    var pDummy;
                }
            });
        }
    });