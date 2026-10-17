require([
    'common'
    ], function () {

        $(function () {

            var mLoaded = false;
            var isPageLoaded = "";

            $.mobile.activePage.bind('pageshow', PageShow);
            function PageShow() {
                mLoaded = true;
                getOrgFOWSTList();
            };
            if (!mLoaded) PageShow();

            window.OrgFowstProdList = {
                GetOrgFowstProdList: function () {
                    getOrgFOWSTList();
                }
            }

            $('body').on('click', '#btnOrgFOWSTProductDone', function (e) {
                e.preventDefault();

                //////                if (window.pageSourcePage != undefined) {
                //////                    //                $.mobile.changePage("#" + $(window.pageSourcePage).attr('id'));
                window.open(GetPathFormat("/People/PeopleMenu"), "_self");
                //////                }
                //////                else {
                //////                    ChangePage('/Organization/OrganizationSummary');
                //////                }
            })

            function getOrgFOWSTList() {

                AjaxSend({
                    url: 'Organization/getOrgFOWSTList',
                    success: function (vData) {
                        LoadOrgFOWSTList(vData.OrganizationFOWSTData.Rows);
                    }
                });
            }

            function LoadOrgFOWSTList(vRows) {
                var pLV = $('#lstOrgFOWSTProduct');
                var pItem;

                $("#divOrgFOWSTProductRecordCount").text(" Number of record(s) found: " + vRows.length);

                pLV.html('');
                for (var p = 0; p <= vRows.length - 1; p++) {
                    pItem = $('<a href="" class="list-group-item list-group-item-action">')
                            .attr('ID', vRows[p].ID)
                            .click(function (e) {
                                e.preventDefault();

                                //window.open(GetPathFormat("/Organization/OrganizationFOWSTProductList") + "?OrgID=" + sessionStorage["OrgID"] + "&OrgFowstID=" + $(this).attr('ID'), "_self");
                                //window.history.pushState('', '', '/SetupChecklist/SystemAdministration?OrgID=' + sessionStorage["OrgID"] + '&OrgFowstID=' + $(this).attr('ID'));
                                sessionStorage["OrgFowstID"] = $(this).attr('ID');
                                if (isPageLoaded == "") {
                                    AjaxSend({
                                        ExactUrl: '/Organization/OrganizationFOWSTProductList?cache=1&OrgID=' + sessionStorage.OrgID,
                                        type: 'GET',
                                        dataType: 'text',
                                        success: function (vData) {
                                            isPageLoaded = vData;
                                            $('#UniversalDiv').empty();
                                            $('#UniversalDiv').append(vData).trigger('create');
                                            App.Deferred.fulfill('FQ26E5TP76');
                                        }
                                    });
                                }
                                else {
                                    $('#UniversalDiv').empty();
                                    $('#UniversalDiv').append(isPageLoaded).trigger('create');
                                    OrgFowstProdList.GetOrgFowstProdList();
                                }

                            })
                            .append($('<h5>')
                                .attr("style", "white-space:normal")
                                .text(vRows[p].Name)
                            )
                            .append($('<div>')
                                .attr("style", "white-space:normal")
                                .text(vRows[p].CompleteFowstAddress)
                            )
                    pLV.append(pItem);
                };

                //pLV.listview('refresh');
            };

        });
    });