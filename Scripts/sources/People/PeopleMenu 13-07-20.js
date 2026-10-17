require([
    'common'
    ], function () {

        $(function () {

            $(document).ready(function () {

                AjaxSend({
                    url: 'People/IsNewEntry',
                    success: function (vData) {

                        ShowPersonNameInfo(vData.PeopleID)

                    }
                });

            });

            $('#btnPeopleMenuPopUpMenu').click(function (e) {
                e.preventDefault();

                $("#divPeopleMenuPopUp").popup("open")

            });

            $('#popPeopleMenuPeopleInfoSummary').click(function (e) {
                e.preventDefault();

                window.open(GetPathFormat("PeopleInfoSummary"), "_self");

            });

            ///Dashboards and Reports --------------------------------------------------------------------------------------------------------------------------------------------
            $('#liSales').click(function (e) {
                e.preventDefault();

                $.mobile.changePage('#pagePopUpMenuPOSReports');

            });

            $('#popReportsPOS').click(function (e) {
                e.preventDefault();

                $.mobile.changePage('#pagePopUpMenuPOSReports');

            });

            $('#popPOSReportReprintReceipt').click(function (e) {
                e.preventDefault();

                window.pageSecuritySourcePage = $.mobile.activePage;
                PromptUserAccess('POS Receipt Reprint', {
                    OnClickOKSuccess: function () {
                        window.open(GetPathFormat("/SalesPayments/DuplicateReceipt") + "?OrgID=" + sessionStorage["OrgID"], "_self");
                    }
                });

            });

            $('#popPOSReportCashierSalesReport').click(function (e) {
                e.preventDefault();

                window.pageSecuritySourcePage = $.mobile.activePage;
                PromptUserAccess('Cashier Sales Report', {
                    OnClickOKSuccess: function () {
                        var pData = {};
                        pData.POSSalesReportType = 1;

                        AjaxSend({
                            url: 'ReportsPOS/setPOSSalesReportType',
                            data: pData,
                            success: function (vData) {

                                window.open(GetPathFormat("/ReportsPOS/SalesReport") + "?OrgID=" + sessionStorage["OrgID"], "_self");
                            }
                        });
                    }
                });

            });

            $('#popPOSReportManagerReport').click(function (e) {
                e.preventDefault();

                window.pageSecuritySourcePage = $.mobile.activePage;
                PromptUserAccess('Manager Sales Report', {
                    OnClickOKSuccess: function () {
                        var pData = {};
                        pData.POSSalesReportType = 0;

                        AjaxSend({
                            url: 'ReportsPOS/setPOSSalesReportType',
                            data: pData,
                            success: function (vData) {

                                window.open(GetPathFormat("/ReportsPOS/SalesReport") + "?OrgID=" + sessionStorage["OrgID"], "_self");
                            }
                        });
                    }
                });

            });

            $('#popPOSReportSalesAdjustment').click(function (e) {
                e.preventDefault();

                window.pageSecuritySourcePage = $.mobile.activePage;
                PromptUserAccess('Sales Adjustment Report', {
                    OnClickOKSuccess: function () {
                        window.open(GetPathFormat("/ReportsPOS/SalesAdjustmentReport") + "?OrgID=" + sessionStorage["OrgID"], "_self");
                    }
                });

            });

            $('#popPOSReportZReport').click(function (e) {
                e.preventDefault();

                window.pageSecuritySourcePage = $.mobile.activePage;
                PromptUserAccess('Z Report', {
                    OnClickOKSuccess: function () {
                        window.open(GetPathFormat("/ReportsPOS/ZReport") + "?OrgID=" + sessionStorage["OrgID"], "_self");
                    }
                });

            }); 

            $('#popPOSReportMallLessorReport').click(function (e) {
                e.preventDefault();

                $.mobile.changePage('#pagePopUpMenuPOSReportsMallLessorReport');

            });

            $('#popPOSReportMallLessorAyala').click(function (e) {
                e.preventDefault();

//                window.pageSecuritySourcePage = $.mobile.activePage;
//                PromptUserAccess('SFA', {
//                    OnClickOKSuccess: function () {

                        window.open(GetPathFormat("/MallLessorAyala") + "?OrgID=" + sessionStorage["OrgID"], "_self");
//                    }
//                });

            });

            $('#popPOSReportMallLessorSM').click(function (e) {
                e.preventDefault();

                alert("under construction!");
                //////                window.pageSecuritySourcePage = $.mobile.activePage;
                //////                PromptUserAccess('Sales Adjustment Report', {
                //////                    OnClickOKSuccess: function () {
                //////                        window.open(GetPathFormat("ReportsPOS/SalesAdjustmentReport"), "_self");
                //////                    }
                //////                });

            });

            $('#btnPOSReportBack').click(function (e) {
                e.preventDefault();

                $('#btnPOSReportMallLessorBackToHome').click();

            });

            $('#btnDashboardsAndReportsBack').click(function (e) {
                e.preventDefault();

                $('#btnPOSReportMallLessorBackToHome').click();

            });

            $('#btnPOSReportMallLessorBackToHome').click(function (e) {
                e.preventDefault();

                window.open(GetPathFormat("/People/PeopleMenu"), "_self");

            });
            ///END Dashboards and Reports --------------------------------------------------------------------------------------------------------------------------------------------


            ///SETTINGS --------------------------------------------------------------------------------------------------------------------------------------------
            $('#popSecuritySettings').click(function (e) {
                e.preventDefault();

                $.mobile.changePage('#pagePopUpMenuSecuritySettings');

            });

            $('#popSecuritySettingsRoles').click(function (e) {
                e.preventDefault();

                window.pageSecuritySourcePage = $.mobile.activePage;
                PromptUserAccess('Security Roles', {
                    OnClickOKSuccess: function () {
                        window.open(GetPathFormat("/SecurityRole") + "?OrgID=" + sessionStorage["OrgID"], "_self");
                    }
                });
            });

            $('#popSecuritySettingsDevices').click(function (e) {
                e.preventDefault();

                window.pageSecuritySourcePage = $.mobile.activePage;
                PromptUserAccess('Devices', {
                    OnClickOKSuccess: function () {
                        window.open(GetPathFormat("/Device") + "?OrgID=" + sessionStorage["OrgID"], "_self");
                    }
                });


            });

            $('#popCompanyProfile').click(function (e) {
                e.preventDefault();

                window.pageSecuritySourcePage = $.mobile.activePage;
                PromptUserAccess('Company Profile', {
                    OnClickOKSuccess: function () {
                        ShowOrganizationProfile();
                    }
                });

            });

            $('#popCompanyPlaces').click(function (e) {
                e.preventDefault();

                window.pageSecuritySourcePage = $.mobile.activePage;
                PromptUserAccess('Company Places (FOWST)', {
                    OnClickOKSuccess: function () {
                        ShowOrganizationPlaces();
                    }
                });

            });

            $('#popCompanyEmployees').click(function (e) {
                e.preventDefault();

                window.pageSecuritySourcePage = $.mobile.activePage;
                PromptUserAccess('Company Employee', {
                    OnClickOKSuccess: function () {
                        ShowOrganizationEmployees();
                    }
                });

            });

            $('#popCompanyProductsPrice').click(function (e) {
                e.preventDefault();

//                window.pageSecuritySourcePage = $.mobile.activePage;
//                PromptUserAccess('Company Employee', {
//                    OnClickOKSuccess: function () {
                window.open(GetPathFormat("/PIMProductPrice") + "?OrgID=" + sessionStorage["OrgID"], "_self");
                //                    }
//                });

            });

            $('#popCompanyCustomers').click(function (e) {
                e.preventDefault();

                window.pageSecuritySourcePage = $.mobile.activePage;
                PromptUserAccess('Company Customer', {
                    OnClickOKSuccess: function () {
                        ShowOrganizationCustomers();
                    }
                });

            });

            $('#popCompanySuppliers').click(function (e) {
                e.preventDefault();

                window.pageSecuritySourcePage = $.mobile.activePage;
                PromptUserAccess('Company Supplier', {
                    OnClickOKSuccess: function () {
                        ShowOrganizationSuppliers();
                    }
                });

            });

            $('#popReceiptSettings').click(function (e) {
                e.preventDefault();

                window.pageSecuritySourcePage = $.mobile.activePage;
                PromptUserAccess('POS Receipt Settings', {
                    OnClickOKSuccess: function () {
                        window.open(GetPathFormat("/ReceiptSettings") + "?OrgID=" + sessionStorage["OrgID"], "_self");
                    }
                });

            });

            $('#popSalesReturnsSettings').click(function (e) {
                e.preventDefault();

                window.pageSecuritySourcePage = $.mobile.activePage;
                PromptUserAccess('Sales Return Settings', {
                    OnClickOKSuccess: function () {
                        window.open(GetPathFormat("/SalesReturns/Settings") + "?OrgID=" + sessionStorage["OrgID"], "_self");
                    }
                });

            });

            $('#btnSettingsBack').click(function (e) {
                e.preventDefault();

                window.open(GetPathFormat("/People/PeopleMenu"), "_self");
            });

            $('#btnSecuritySettingsBack').click(function (e) {
                e.preventDefault();

                $('#btnSettingsBack').click();

            });

            ///END SETTINGS --------------------------------------------------------------------------------------------------------------------------------------------

            $('#btnSignOut').click(function () {

                SignOut();

            });

            $('#btnPeopleMenuPeopleImage').click(function (e) {
                e.preventDefault();

                $('#popPeopleMenuPeopleInfoSummary').click();

            });

            $('#lblName').click(function (e) {
                e.preventDefault();

                $('#popPeopleMenuPeopleInfoSummary').click();

            });

            $('#btnPeopleMsgT1Confirm').click(function () {

                ConfirmPeopleMsgT1();

            });

            $('#btnPeopleMsgT1Deny').click(function () {
                alert("Not yet implemented.");
            });

            $('#btnPeopleMsgT1Cancel').click(function () {
                $.mobile.changePage('#pageHome');
            });


            $('#btnPeopleMenuFindEstablishment').click(function (e) {
                e.preventDefault();

                window.open(GetPathFormat("/OrganizationShort"), "_self");

            });

            $('#tabHome').click(function () {

                $('#divHome').show();
                $('#divApps').hide();
                $('#divFind').hide();

                ShowNotifications();

            });

            $('#tabApps').click(function () {

                $('#divHome').hide();
                $('#divApps').show();
                $('#divFind').hide();

            });

            $('#tabFind').click(function () {

                $('#divHome').hide();
                $('#divApps').hide();
                $('#divFind').show();

            });

            function SignOut() {

                AjaxSend({
                    url: "People/setSignOut",
                    success: function (vData) {
                        window.open(GetPathFormat("/People"), "_self");
                    }
                });

            };

            function ShowPersonNameInfo(strID) {
                var pData = {};
                pData.PeopleID = strID;

                ClearPersonFields();

                AjaxSend({
                    url: "People/getPeopleInfoName",
                    data: pData,
                    success: function (vData) {

                        if (vData.PeopleNameData.Rows.length > 0) {

                            //Header Info
                            $("#lblName").text(vData.PeopleNameData.Rows[0].PeopleName || '');
                            $('#tabHome').addClass('ui-btn-active');

                            //Get People Messages
                            ShowPeopleMsgList();

                            //Get People Organization
                            ShowPeopleOrgList();

                        }
                    }
                });

            };

            function ShowPeopleMsgList() {

                AjaxSend({
                    url: "People/getPeopleMessageList",
                    success: function (vDataMsg) {

                        var pLV = $('#lstMessage');
                        var pItem;

                        pLV.html('');

                        if (vDataMsg.PeopleData.Rows.length > 0) {

                            for (intCntr = 0; intCntr < vDataMsg.PeopleData.Rows.length; intCntr++) {

                                pItem = $('<li>')
                                                    .append($('<a>')
                                                        .data('data', vDataMsg.PeopleData.Rows[intCntr])
                                                        .append($('<h3>')
                                                            .attr("style", "white-space:normal")
                                                            .text(vDataMsg.PeopleData.Rows[intCntr].Subject)
                                                        )
                                                        .append($('<p>')
                                                            .attr("style", "white-space:normal")
                                                            .text('Sender : ' + vDataMsg.PeopleData.Rows[intCntr].SenderName)
                                                        )
                                                        .append($('<p>')
                                                            .attr("style", "white-space:normal")
                                                            .text('Date/Time : ' + $.format.date(new Date(vDataMsg.PeopleData.Rows[intCntr].MessageDateTime), 'MMM dd, yyyy hh:mm a'))
                                                        )
                                                        .click(function (e) {
                                                            e.preventDefault();
                                                            ShowPeopleMsgDetails($(this).data('data'));
                                                        })
                                                    );


                                pLV.append(pItem);

                                pLV.trigger("create");

                            }

                            pLV.listview('refresh');
                        }
                    }
                });
            }

            function ShowPeopleOrgList() {

                AjaxSend({
                    url: "People/getPeopleOrganizationList",
                    success: function (vDataOrg) {

                        if (vDataOrg.PeopleData.Rows.length > 0) {

                            var pLV = $('#divOrgApps');
                            var pItem;

                            pLV.html('');
                            for (intCntr = 0; intCntr < vDataOrg.PeopleData.Rows.length; intCntr++) {

                                pItem = $('<div data-role="collapsible" data-collapsed="false" data-theme="b" data-content-theme="d" data-collapsed-icon="arrow-r" data-expanded-icon="arrow-d" data-inset="true">')
                                                .append($('<h2>')
                                                    .text(vDataOrg.PeopleData.Rows[intCntr].CompanyName)
                                                )
                                                .append($('<ul data-role="listview" id="lst' + vDataOrg.PeopleData.Rows[intCntr].OrganizationID + '">')
                                                    .append($('<li>')
                                                        .append($('<a id="btnPOS_' + vDataOrg.PeopleData.Rows[intCntr].OrganizationID + '">')
                                                            .text("Point of Sales (POS)")
                                                            .data('data', vDataOrg.PeopleData.Rows[intCntr])
                                                            .click(function (e) {
                                                                e.preventDefault();

                                                                sessionStorage["OrgID"] = $(this).data('data').OrganizationID;

                                                                window.pageSecuritySourcePage = $.mobile.activePage;
                                                                PromptUserAccess('Point Of Sales (POS)', {
                                                                    OnClickOKSuccess: function () {

                                                                        window.location = GetPathFormat("/SalesMenu?mode=pos&OrgID=" + sessionStorage.OrgID);
//                                                                        window.open(GetPathFormat("/SalesMenu") + "?mode=pos&OrgID=" + sessionStorage["OrgID"], "_self");
                                                                    }
                                                                });
                                                            })
                                                        )
                                                    )
                                                    .append($('<li>')
                                                        .append($('<a id="btnSalesReturns_' + vDataOrg.PeopleData.Rows[intCntr].OrganizationID + '">')
                                                            .text("Sales Returns")
                                                            .data('data', vDataOrg.PeopleData.Rows[intCntr])
                                                            .click(function (e) {
                                                                e.preventDefault();

                                                                sessionStorage["OrgID"] = $(this).data('data').OrganizationID;

                                                                window.pageSecuritySourcePage = $.mobile.activePage;
                                                                PromptUserAccess('Sales Return', {
                                                                    OnClickOKSuccess: function () {

                                                                        window.open(GetPathFormat("/SalesReturns") + "?OrgID=" + sessionStorage["OrgID"], "_self");
                                                                    }
                                                                });
                                                            })
                                                        )
                                                    )
                                                    .append($('<li>')
                                                        .append($('<a id="btnReceiving_' + vDataOrg.PeopleData.Rows[intCntr].OrganizationID + '">')
                                                            .attr("href", GetPathFormat("/Receiving") + '?'
                                                                + $.param({
                                                                    OrgID: vDataOrg.PeopleData.Rows[intCntr].OrganizationID
                                                                }))
                                                            .text("Receiving and Transfers")
                                                            .data('data', vDataOrg.PeopleData.Rows[intCntr])
                                                        )
                                                    )
                                                    .append($('<li>')
                                                        .append($('<a id="btnInventoryCount_' + vDataOrg.PeopleData.Rows[intCntr].OrganizationID + '">')
                                                            .attr("href", GetPathFormat("/InventoryCount") + '?'
                                                                + $.param({
                                                                    OrgID: vDataOrg.PeopleData.Rows[intCntr].OrganizationID
                                                                }))
                                                            .text("Inventory Count")
                                                            .data('data', vDataOrg.PeopleData.Rows[intCntr])
                                                        )
                                                    )
                                                    .append($('<li>')
                                                        .append($('<a id="btnDashboardsAndReports_' + vDataOrg.PeopleData.Rows[intCntr].OrganizationID + '">')
                                                            .attr("href", "#?"
                                                                + $.param({
                                                                    OrgID: vDataOrg.PeopleData.Rows[intCntr].OrganizationID
                                                                }))
                                                            .text("Dashboards and Reports")
                                                            .data('data', vDataOrg.PeopleData.Rows[intCntr])
                                                            .click(function (e) {
                                                                e.preventDefault();

                                                                sessionStorage["OrgID"] = $(this).data('data').OrganizationID;
                                                                $.mobile.changePage('#pagePopUpMenuDashboardsAndReports');

                                                            })
                                                        )
                                                    )
                                                    .append($('<li>')
                                                        .append($('<a id="btnSettings_' + vDataOrg.PeopleData.Rows[intCntr].OrganizationID + '">')
                                                            .text("Settings")
                                                            .data('data', vDataOrg.PeopleData.Rows[intCntr])
                                                            .click(function (e) {
                                                                e.preventDefault();

                                                                sessionStorage["OrgID"] = $(this).data('data').OrganizationID;
                                                                $.mobile.changePage('#pagePopUpMenuSettings');

                                                            })
                                                        )
                                                    )
                                //                                                    .append($('<li>')
                                //                                                        .append($('<a id="btnSFA_' + vDataOrg.PeopleData.Rows[intCntr].OrganizationID + '">')
                                //                                                            .text("SFA")
                                //                                                            .data('data', vDataOrg.PeopleData.Rows[intCntr])
                                //                                                            .click(function (e) {
                                //                                                                e.preventDefault();

                                //                                                                if ($(this).data('data') != undefined) {
                                //                                                                    pData.OrgID = $(this).data('data').OrganizationID;
                                //                                                                    AjaxSend({
                                //                                                                        url: "People/setSubscriberOrganization",
                                //                                                                        data: pData,
                                //                                                                        success: function (vData) {
                                //                                                                            if (vData.SubcriberOrg == "TRUE") {
                                //                                                                                window.pageSecuritySourcePage = $.mobile.activePage;
                                //                                                                                PromptUserAccess('SFA', {
                                //                                                                                    OnClickOKSuccess: function () {
                                //                                                                                        window.open(GetPathFormat("/SalesMenu") + "?mode=pos", "_self");
                                //                                                                                    }
                                //                                                                                });
                                //                                                                            }
                                //                                                                        }
                                //                                                                    });
                                //                                                                }

                                //                                                            })
                                //                                                        )
                                //                                                    )
                                //                                                    .append($('<li>')
                                //                                                        .append($('<a id="btnCRM_' + vDataOrg.PeopleData.Rows[intCntr].OrganizationID + '">')
                                //                                                            .text("CRM")
                                //                                                            .data('data', vDataOrg.PeopleData.Rows[intCntr])
                                //                                                        )
                                //                                                    )

                                                );


                                pLV.append(pItem);

                                pLV.trigger("create");

                            }
                        }
                    }
                });


            }

            function ShowPeopleMsgDetails(vData) {

                if (vData.MessageType == 1) {
                    ShowPeopleMsgT1(vData);
                }

            }

            function ShowPeopleMsgT1(vMsgData) {
                var pData = {};
                pData.MsgID = vMsgData.ID;

                ClearPeopleMsgT1();

                //Get Messages Details
                AjaxSend({
                    url: "People/getPeopleMessageT1Details",
                    data: pData,
                    success: function (vDataMsgDetails) {

                        if (vDataMsgDetails.PeopleData.Rows.length > 0) {

                            $('#txtPeopleMsgT1ID').val(vDataMsgDetails.PeopleData.Rows[0].ID);

                            $('#lblPeopleMsgT1Subject').text(vDataMsgDetails.PeopleData.Rows[0].Subject);
                            $('#lblPeopleMsgT1From').text('From : ' + vDataMsgDetails.PeopleData.Rows[0].SenderName);
                            $('#lblPeopleMsgT1DateTime').text('Date/Time : ' + $.format.date(new Date(vDataMsgDetails.PeopleData.Rows[0].MessageDateTime), 'MMM dd, yyyy hh:mm a'));

                            $('#lblPeopleMsgT1').text(vDataMsgDetails.PeopleData.Rows[0].Message);

                            $.mobile.changePage('#pagePeopleMsgType1Info');

                        }
                    }
                });

            }

            function ClearPeopleMsgT1() {
                $("#txtPeopleMsgT1ID").val('');

                $('#lblPeopleMsgT1Subject').text('');
                $('#lblPeopleMsgT1From').text('From : ');
                $('#lblPeopleMsgT1DateTime').text('Date/Time : ');

                $('#lblPeopleMsgT1').text('');
            }

            function ConfirmPeopleMsgT1() {
                var pData = {};
                pData.MsgID = $("#txtPeopleMsgT1ID").val();

                //Get Messages Details
                AjaxSend({
                    url: "People/ConfirmPeopleMessageT1Details",
                    data: pData,
                    success: function (vDataMsgDetails) {

                        $.mobile.changePage('#pageHome');
                        ShowPeopleMsgList();
                        ShowPeopleOrgList();

                    }
                });

            }

            function ShowNotifications() {
                //Load Notifications
            }

            function ClearPersonFields() {

                //Header Info
                $("#lblName").text('');
                $("#lblEmployer").text('');

            }

            function ShowOrganizationProfile() {
                var pData = {};
                pData.OrgID = sessionStorage["OrgID"];

                window.pageOrig = $.mobile.activePage;

                AjaxSend({
                    url: 'Organization/getSubscriberOrganization',
                    data: pData,
                    success: function (vDataOrg) {

                        if (vDataOrg.OrganizationID != "") {
                            pData.IsNew = 0;
                            pData.ID = vDataOrg.OrganizationID;

                            AjaxSend({
                                url: 'Organization/setCreateNew',
                                data: pData,
                                success: function (vData) {
                                    ChangePage('/Organization/OrganizationSummary');
                                }
                            });
                        }
                    }
                });

            }

            function ShowOrganizationPlaces() {
                var pData = {};
                pData.OrgID = sessionStorage["OrgID"];

                window.pageSourcePage = $.mobile.activePage;

                AjaxSend({
                    url: 'Organization/getSubscriberOrganization',
                    data: pData,
                    success: function (vDataOrg) {

                        if (vDataOrg.OrganizationID != "") {
                            pData.IsNew = 0;
                            pData.ID = vDataOrg.OrganizationID;

                            AjaxSend({
                                url: 'Organization/setCreateNew',
                                data: pData,
                                success: function (vData) {

                                    ChangePage('/Organization/OrganizationAddress');
                                }
                            });
                        }
                    }
                });

            }

            function ShowOrganizationEmployees() {
                var pData = {};
                pData.OrgID = sessionStorage["OrgID"];

                window.pageSourcePage = $.mobile.activePage;

                AjaxSend({
                    url: 'Organization/getSubscriberOrganization',
                    data: pData,
                    success: function (vDataOrg) {

                        if (vDataOrg.OrganizationID != "") {
                            pData.IsNew = 0;
                            pData.ID = vDataOrg.OrganizationID;

                            AjaxSend({
                                url: 'Organization/setCreateNew',
                                data: pData,
                                success: function (vData) {

                                    ChangePage('/Organization/OrganizationEmployee');
                                }
                            });
                        }
                    }
                });

            }

            function ShowOrganizationCustomers() {
                var pData = {};
                pData.OrgID = sessionStorage["OrgID"];

                window.pageSourcePage = $.mobile.activePage;

                AjaxSend({
                    url: 'Organization/getSubscriberOrganization',
                    data: pData,
                    success: function (vDataOrg) {

                        if (vDataOrg.OrganizationID != "") {
                            pData.IsNew = 0;
                            pData.ID = vDataOrg.OrganizationID;

                            AjaxSend({
                                url: 'Organization/setCreateNew',
                                data: pData,
                                success: function (vData) {

                                    ChangePage('/Organization/OrganizationCustomer');
                                }
                            });
                        }
                    }
                });
            }

            function ShowOrganizationSuppliers() {
                var pData = {};
                pData.OrgID = sessionStorage["OrgID"];

                window.pageSourcePage = $.mobile.activePage;

                AjaxSend({
                    url: 'Organization/getSubscriberOrganization',
                    data: pData,
                    success: function (vDataOrg) {

                        if (vDataOrg.OrganizationID != "") {
                            pData.IsNew = 0;
                            pData.ID = vDataOrg.OrganizationID;

                            AjaxSend({
                                url: 'Organization/setCreateNew',
                                data: pData,
                                success: function (vData) {

                                    ChangePage('/Organization/OrganizationSupplier');
                                }
                            });
                        }
                    }
                });

            }



        });
    });