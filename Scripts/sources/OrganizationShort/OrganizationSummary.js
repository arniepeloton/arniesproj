require([
    'common'
    ], function () {

        $(function () {

            var mLoaded = false;
            var isIE = false || !!document.documentMode;
            var isOrgShortInfoPageLoaded = "";
            var isOrgShortFowstPageLoaded = "";
            var isOrgShortRepPageLoaded = "";
            var isOrgCustProdPageLoaded = "";
            var isOrgCustFowstProdPageLoaded = "";

            var ismob = CBool(IsMobile) ? 1 : 0

            $.mobile.activePage.bind('pageshow', PageShow);
            function PageShow() {
                mLoaded = true;
                AjaxSend({
                    url: 'OrganizationShort/IsNewEntry',
                    success: function (vData) {
                        ShowSumOrganizationInfo(vData.OrganizationID)
                    }
                });
            };
            if (!mLoaded) PageShow();

            window.OrgShort = {
                ShowOrgShortSummary: function () {
                    AjaxSend({
                        url: 'OrganizationShort/IsNewEntry',
                        success: function (vData) {
                            ShowSumOrganizationInfo(vData.OrganizationID)
                        }
                    });
                }
            }

            $('body').on('click', '#btnSumOrgInfo', function (e) {
                e.preventDefault();

                if (isOrgShortInfoPageLoaded == "") {
                    AjaxSend({
                        ExactUrl: '/OrganizationShort/OrganizationInfo?OrgID=' + sessionStorage.OrgID + '&IsMobile=' + ismob,
                        type: 'GET',
                        dataType: 'text',
                        success: function (vData) {
                            isOrgShortInfoPageLoaded = vData;
                            $('#UniversalDiv').html('');
                            $('#UniversalDiv').append(vData).trigger('create');
                        }
                    });
                }
                else {
                    $('#UniversalDiv').html('');
                    $('#UniversalDiv').append(isOrgShortInfoPageLoaded).trigger('create');
                    OrgShortInfo.ShowOrgInfo();
                }
            })
            .on('click', '#btnSumOrgFOWST', function (e) {
                e.preventDefault();

                if (isOrgShortFowstPageLoaded == "") {
                    AjaxSend({
                        ExactUrl: '/OrganizationShort/OrganizationAddress?OrgID=' + sessionStorage.OrgID + '&IsMobile=' + ismob,
                        type: 'GET',
                        dataType: 'text',
                        success: function (vData) {
                            isOrgShortFowstPageLoaded = vData;
                            $('#UniversalDiv').html('');
                            $('#UniversalDiv').append(vData).trigger('create');
                        }
                    });
                }
                else {
                    $('#UniversalDiv').html('');
                    $('#UniversalDiv').append(isOrgShortFowstPageLoaded).trigger('create');
                    OrgShortAdd.ShowOrgAdd();
                }
            })
            .on('click', '#btnSumOrgRepresentative', function (e) {
                e.preventDefault();

                if (isOrgShortRepPageLoaded == "") {
                    AjaxSend({
                        ExactUrl: '/OrganizationShort/OrganizationRepresentative?OrgID=' + sessionStorage.OrgID + '&IsMobile=' + ismob,
                        type: 'GET',
                        dataType: 'text',
                        success: function (vData) {
                            isOrgShortRepPageLoaded = vData;
                            $('#UniversalDiv').html('');
                            $('#UniversalDiv').append(vData).trigger('create');
                        }
                    });
                }
                else {
                    $('#UniversalDiv').html('');
                    $('#UniversalDiv').append(isOrgShortRepPageLoaded).trigger('create');
                    OrgShortRep.ShowOrgRep();
                }
            })
            .on('click', '#btnSumOrgCustProductList', function (e) {
                e.preventDefault();

                if (isOrgCustProdPageLoaded == "") {
                    AjaxSend({
                        ExactUrl: '/Organization/OrganizationCustomerProduct?OrgID=' + sessionStorage.OrgID + '&IsMobile=' + ismob,
                        type: 'GET',
                        dataType: 'text',
                        success: function (vData) {
                            isOrgCustProdPageLoaded = vData;
                            $('#UniversalDiv').html('');
                            $('#UniversalDiv').append(vData).trigger('create');
                        }
                    });
                }
                else {
                    $('#UniversalDiv').html('');
                    $('#UniversalDiv').append(isOrgCustProdPageLoaded).trigger('create');
                    OrgShortCusProd.ShowOrgCustProd();
                }
            })
            .on('click', '#btnSumOrgCustFOWSTProductList', function (e) {
                e.preventDefault();

                if (isOrgCustFowstProdPageLoaded == "") {
                    AjaxSend({
                        ExactUrl: '/Organization/OrganizationCustomerFOWST?OrgID=' + sessionStorage.OrgID + '&IsMobile=' + ismob,
                        type: 'GET',
                        dataType: 'text',
                        success: function (vData) {
                            isOrgCustFowstProdPageLoaded = vData;
                            $('#UniversalDiv').html('');
                            $('#UniversalDiv').append(vData).trigger('create');
                        }
                    });
                }
                else {
                    $('#UniversalDiv').html('');
                    $('#UniversalDiv').append(isOrgCustFowstProdPageLoaded).trigger('create');
                    OrgShortCusFowst.ShowOrgCusFowst();
                }
            })
            .on('click', '#btnOrgSummaryDone', function (e) {
                e.preventDefault();

                //////                $.mobile.changePage("#" + $(window.pageOrig).attr('id'));
                //                window.location = MergeUrlParameters({
                //                    OrgID: sessionStorage.OrgID,
                //                    IsMobile: CBool(IsMobile) ? 1 : 0
                //                }, '/Organization/OrganizationCustomer');
                window.location.hash = 'LibraryPage';
            })

            function ShowSumOrganizationInfo(strID) {
                var pData = {};
                pData.OrganizationID = strID;

                ClearSumOrganizationFields();

                AjaxSend({
                    url: "OrganizationShort/getOrganizationInfo",
                    data: pData,
                    success: function (vData) {

                        if (vData.OrganizationData.Rows.length > 0) {

                            $("#txtSumID").val(vData.OrganizationData.Rows[0].ID || '');
                            $("#txtSumCorporateName").val(vData.OrganizationData.Rows[0].IsOrganization == 1 ? (vData.OrganizationData.Rows[0].CompanyName || '') : (vData.OrganizationData.Rows[0].PersonName || ''));

                            getSumAddressList();
                            getSumRepresentativeList();

                        }
                    }
                });

            };

            function ClearSumOrganizationFields() {
                $("#txtSumID").val('');
                $("#txtSumCorporateName").val('');
            }

            function getSumAddressList() {
                AjaxSend({
                    url: 'OrganizationShort/getOrgFOWSTList',
                    success: function (vData) {
                        LoadSumAddressList(vData.OrganizationFowstData.Rows);
                    }
                });
            }

            function LoadSumAddressList(vRows) {
                var pLV = $('#lstSumAddress');
                var pItem;

                pLV.html('');
                for (var p = 0; p <= vRows.length - 1; p++) {
                    pItem = $('<li>')
                        .append($('<h3>')
                            .attr("style", "white-space:normal")
                            .text(vRows[p].Name)
                        )
                        .append($('<p>')
                            .attr("style", "white-space:normal")
                            .text(vRows[p].CompleteAddress)
                        );
                    pLV.append(pItem);
                };

                pLV.listview('refresh');
            };

            function getSumRepresentativeList() {
                AjaxSend({
                    url: 'OrganizationShort/getOrgRepList',
                    success: function (vData) {
                        LoadSumRepList(vData.OrganizationRepData.Rows);
                    }
                });
            }

            function LoadSumRepList(vRows) {
                var pLV = $('#lstSumRepresentative');
                var pItem;

                pLV.html('');
                for (var p = 0; p <= vRows.length - 1; p++) {
                    pItem = $('<li>')
                        .append($('<h3>')
                            .attr("style", "white-space:normal")
                            .text(vRows[p].RepresentativeName)
                        );
                    pLV.append(pItem);
                };

                pLV.listview('refresh');
            };


        });
    });


