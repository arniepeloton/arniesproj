require([
    'common'
    ], function () {

        $(function () {

            $('#frmOrgBrowser').submit(function (e) {
                e.preventDefault();

                ShowOrganization();

            });

            $('#btnAddNewOrganization').click(function () {

                CreateNewOrgEntry(1, '');

            });

            $('#btnOrgBackToHomeIcon').click(function () {

                window.open(GetPathFormat("/People/PeopleMenu"), "_self");

            });

            $('#btnOrgBackToHome').click(function () {

                $("#btnOrgBackToHomeIcon").click();

            });

            function ShowOrganization() {

                var pData = {};
                pData.SearchString = $("#txtOrgSearch").val();

                AjaxSend({
                    url: 'Organization/getOrganizationList',
                    data: pData,
                    success: function (vData) {
                        LoadOrganizationList(vData.OrganizationData.Rows);
                    }
                });

            }

            function LoadOrganizationList(vRows) {
                var pLV = $('#lstOrganization');
                var pItem;

                $("#divOrgSearchResults").hide();
                $("#divOrgSearchNoResults").hide();
                $("#btnAddNewOrganization").hide();

                if (vRows.length > 0) {
                    $("#divOrgSearchResults").show();
                    $("#divOrgRecordCount").text(" Number of record(s) found: " + vRows.length);

                    pLV.html('');
                    for (var p = 0; p <= vRows.length - 1; p++) {
                        pItem = $('<li>')
                        .append($('<a>')
                            .attr('ID', vRows[p].ID)
                            .click(function (e) {
                                e.preventDefault();

                                CreateNewOrgEntry(0, $(this).attr('ID'));

                            })
                            .append($('<h3>')
                                .attr("style", "white-space:normal")
                                .text(vRows[p].CompanyName)
                            )
                        );
                        pLV.append(pItem);
                    };

                    pLV.listview('refresh');
                }
                else {
                    $("#divOrgSearchNoResults").show();
                    $("#btnAddNewOrganization").show();
                    $("#lblOrgNotFound").text('The organization "' + $("#txtOrgSearch").val() + '" does not exist. Please consider to search thoroughly using an alternate spelling or trade name before creating a new organization to avoid duplication.');
                }
            };

            function CreateNewOrgEntry(IsNew, strID) {
                var pData = {};
                pData.IsNew = IsNew;
                pData.ID = strID;

                window.pageOrig = $.mobile.activePage;

                AjaxSend({
                    url: 'OrganizationShort/setCreateNew',
                    data: pData,
                    success: function (vData) {

                        if ((vData.IsNew || '') == "TRUE") {
                            window.IsPageLoaded = false;
                            //   ChangePage('/Organization/OrganizationInfo');
                            window.location = MergeUrlParameters({
                                OrgID: sessionStorage.OrgID,
                                IsMobile: CBool(IsMobile) ? 1 : 0
                            }, '/Organization/OrganizationInfo');
                        }
                        else {
                            window.IsPageLoaded = false;
                            //    ChangePage('/Organization/OrganizationSummary');
                            window.location = MergeUrlParameters({
                                OrgID: sessionStorage.OrgID,
                                IsMobile: CBool(IsMobile) ? 1 : 0
                            }, '/Organization/OrganizationSummary');
                        }
                    }
                });

            }





        });
    });