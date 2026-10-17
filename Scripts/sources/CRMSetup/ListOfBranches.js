require([
    'common'
    ], function () {
        $(function () {
            getBranches();
        })

        function getBranches() {

            AjaxSend({
                url: 'crmsetup/getBranches/',
                success: function (vData) {
                    LoadResult(vData.data.Rows);
                },
                error: function (vError) {
                    var pDummy;
                }
            });
        }

        function LoadResult(vRows) {

            var pLV = $('#lstBranches');
            var pItem;

            pLV.html('');
            for (var p = 0; p <= vRows.length - 1; p++) {
                pItem = $('<li>')
                    .append($('<a>')
                    .attr('href', '#')
                    .attr('ID', vRows[p].ID)
                    .attr('data', vRows[p].Code + ' - ' + vRows[p].Name + ' - ' + vRows[p].Address)
                    .click(function (e) {
                        e.preventDefault();
                        assignBranchID($(this).attr("ID"), $(this).attr("data"));
                    })
                    .append($('<h3>')
                        .text(vRows[p].Code + ' - ' + vRows[p].Name + ' - ' + vRows[p].Address)
                    )
                );
                pLV.append(pItem);
            };

            pLV.listview('refresh');
        };

        function assignBranchID(BranchID, BranchName) {
            var option = {
                m_BranchID: BranchID,
                m_BranchName: BranchName
            };

            AjaxSend({
                url: 'crmsetup/assignBranchID',
                data: option,
                success: function (vData) {
                    window.open("Teams/", "_self");
                },
                error: function (vError) {
                    var pDummy;
                }
            });

            //    $.ajax({
            //        url: "assignBranchID/",
            //        dataType: "json",
            //        data: JSON.stringify(option),
            //        type: "POST",
            //        err: function (err) {
            //            alert("Error: " + err.toString());
            //        },
            //        beforeSend: function () {
            //        },
            //        success: function (data) {
            //            window.open("Teams/", "_self");
            //        }
            //    });
        }
    });