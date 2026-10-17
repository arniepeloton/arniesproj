require([
    'common'
    ], function () {

        $(function () {

            var mLoaded = false;
            var isOrgShortCusFowstProdListLoaded = "";
            var ismob = CBool(IsMobile) ? 1 : 0;

            $.mobile.activePage.bind('pageshow', PageShow);
            function PageShow() {
                mLoaded = true;
                getOrgCustFOWSTList();
            };
            if (!mLoaded) PageShow();

            window.OrgShortCusFowst = {
                ShowOrgCusFowst: function () {
                    getOrgCustFOWSTList();
                }
            }

            $('body').on('click', '#btnOrgCustFOWSTProductDone', function (e) {
                e.preventDefault();

//                if (window.pageSourcePage != undefined) {
//                    $.mobile.changePage("#" + $(window.pageSourcePage).attr('id'));
//                }
//                else {
//                    window.open(GetPathFormat("/Organization/OrganizationCustomer") + "?OrgID=" + sessionStorage["OrgID"], "_self");
                //                }
                window.location.hash = '';
                window.location.hash = 'UniversalPage';
                LibraryTable.ViewCustomerPage();
            })

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

                                window.pageSourcePageFowst = $.mobile.activePage;
                                //////                                window.open(GetPathFormat("/Organization/OrganizationCustomerFOWSTProductList") + "?OrgFowstID=" + $(this).attr('ID'), "_self");

                                var pData = {};
                                pData.FowstID = $(this).attr('ID');

                                AjaxSend({
                                    url: "Organization/VerifyOrganizationCustomerFOWST",
                                    data: pData,
                                    success: function (vData) {
                                        // ChangePage('/Organization/OrganizationCustomerFOWSTProductList');
                                        //                                        window.location = MergeUrlParameters({
                                        //                                            OrgID: sessionStorage.OrgID,
                                        //                                            IsMobile: CBool(IsMobile) ? 1 : 0
                                        //                                        }, '/Organization/OrganizationCustomerFOWSTProductList');
                                        if (isOrgShortCusFowstProdListLoaded == "") {
                                            AjaxSend({
                                                ExactUrl: '/Organization/OrganizationCustomerFOWSTProductList?OrgID=' + sessionStorage.OrgID + '&IsMobile=' + ismob,
                                                type: 'GET',
                                                dataType: 'text',
                                                success: function (vData) {
                                                    isOrgShortCusFowstProdListLoaded = vData;
                                                    $('#UniversalDiv').html('');
                                                    $('#UniversalDiv').append(vData).trigger('create');
                                                }
                                            });
                                        }
                                        else {
                                            $('#UniversalDiv').html('');
                                            $('#UniversalDiv').append(isOrgShortCusFowstProdListLoaded).trigger('create');
                                            OrgShortCusFowstProdList.ShowOrgCusFowstProdList();
                                        }
                                    }
                                });

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