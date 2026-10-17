require([
    'common'
    ], function () {

        $(function () {

            var mLoaded = false;

            $.mobile.activePage.bind('pageshow', PageShow);
            function PageShow() {
                mLoaded = true;
                getOrgCustFOWSTList();
            };
            if (!mLoaded) PageShow();

            $('#btnOrgCustFOWSTProductDone').click(function (e) {
                e.preventDefault();

//////                if (window.pageSourcePage != undefined) {
//////                    //                $.mobile.changePage("#" + $(window.pageSourcePage).attr('id'));
                    window.open(GetPathFormat("/People/PeopleMenu"), "_self");
//////                }
//////                else {
//////                    ChangePage('/Organization/OrganizationSummary');
//////                }

            });

            function getOrgCustFOWSTList() {
                AjaxSend({
                    url: 'Organization/getOrgFOWSTList',
                    success: function (vData) {
                        LoadOrgCustFOWSTList(vData.OrganizationFOWSTData.Rows);
                    }
                });
            }

            function LoadOrgCustFOWSTList(vRows) {
                var pLV = $('#lstOrgCustFOWSTProduct');
                var pItem;

                $("#divOrgCustFOWSTProductRecordCount").text(" Number of record(s) found: " + vRows.length);

                pLV.html('');
                for (var p = 0; p <= vRows.length - 1; p++) {
                    pItem = $('<li>')
                        .append($('<a>')
                            .attr('ID', vRows[p].ID)
                            .click(function (e) {
                                e.preventDefault();

                                var pParams = GetUrlParameters();
                                window.open(GetPathFormat("/Organization/OrganizationCustomerFOWSTProductList") + "?CustOrgID=" + pParams.CustOrgID + "&OrgFowstID=" + $(this).attr('ID'), "_self");

                            })
                            .append($('<h3>')
                                .attr("style", "white-space:normal")
                                .text(vRows[p].Name)
                            )
                            .append($('<p>')
                                .attr("style", "white-space:normal")
                                .text(vRows[p].CompleteFowstAddress)
                            )
                        );
                    pLV.append(pItem);
                };

                pLV.listview('refresh');
            };

        });
    });