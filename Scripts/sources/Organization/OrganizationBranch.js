require([
    'common'
    ], function () {

        $(function () {

            $(document).ready(function () {

                $("#btnAddBranch > span").attr("style", "text-align:left;white-space:normal")

                getBranchList();

            });

            $('#btnAddBranch').click(function (e) {

                e.preventDefault();

                setBranchDetails("");

            });

            $('#btnBranchNext').click(function (e) {
                e.preventDefault();

                window.open("OrganizationContact", "_self");

            });

            $('#btnBranchBack').click(function (e) {
                e.preventDefault();

                window.open("OrganizationInfo", "_self");

            });

            $('#btnBranchDone').click(function (e) {
                e.preventDefault();

                window.open("OrganizationInfoSummary", "_self");
            });

            function getBranchList() {
                AjaxSend({
                    url: "Organization/getOrgBranchList",
                    success: function (vDataBranch) {

                        LoadOrgBranchList(vDataBranch.OrgBranchList.Rows);

                    }
                });
            }

            function LoadOrgBranchList(vRows) {

                var pLV = $('#lstBranch');
                var pItem;

                pLV.html('');
                for (var p = 0; p <= vRows.length - 1; p++) {
                    pItem = $('<li>')
                        .append($('<a>')
                            .attr('ID', vRows[p].ID)
                            .click(function (e) {
                                e.preventDefault();
                                setBranchDetails($(this).attr('ID'));
                            })
                            .append($('<h3>')
                                .attr("style", "white-space:normal")
                                .text(vRows[p].PropertyAndPlant)
                            )
                        );
                    pLV.append(pItem);
                };

                pLV.listview('refresh');

            };

            function setBranchDetails(strID) {
                var pData = {};
                pData.BranchID = strID;

                AjaxSend({
                    url: "Organization/setOrgBranchInfo",
                    data: pData,
                    success: function (vData) {

                        if (vData.OrgBranchData == "TRUE") {
                            window.open("../PropertyAndPlant/AddEdit", "_self");
                        }
                    }
                });
            }


        });
    });
