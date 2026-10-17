require([
    'common', 'tablesort'
    ], function () {

        var selectedFranchiseStatusID = '';
        var selectedFranchiseID = '';
        var selectedFranchiseeOrgFowstID;

        $(function () {
            if (proj.IsBootstrapDesign()) {
                $('#dialogFranchiseeStatuscontent').contents().appendTo('#dialogFranchiseeStatuscontent-bs');
                $('#dialogFranchisorStatuscontent').contents().appendTo('#dialogFranchisorStatuscontent-bs');
            }

            getGroupList();
            showAllFranchisees('');

            $('#selFranchiseType').change(function (e) {
                if ($(this).val() == 'optFranchisor') {
                    $('#divFranchisorRequests').show();
                    $('#divFranchiseeRequests').hide();
                    showAllFranchiseRequests();
                } else {
                    $('#divFranchisorRequests').hide();
                    $('#divFranchiseeRequests').show();
                    showAllFranchisees('');
                }
            });


            $('#btnApproveRequestFranchisee').click(function (e) {
                if ($("#selFranchiseGroup").val() == '') {
                    WebToast('Please add this franchisee to a group', 'top');
                } else {
                    var data = {
                        FranchiseID: selectedFranchiseID,
                        FranchiseStatusID: selectedFranchiseStatusID,
                        IsFranchisee: 1,
                        GroupID: $("#selFranchiseGroup").val()
                    };

                    AjaxSend({
                        url: 'DashboardsAndReports/ActivateFranchisee',
                        data: data,
                        success: function (vData) {
                            $('#dialogFranchiseeStatus-bs').modal('hide');
                            if (vData.FranchiseData == 'ok') {
                                WebToast('Franchisee request has been approved', 'top'); // 
                                showAllFranchisees('');
                            } else {
                                NewMessageBox('There was an error while processing your request');
                            }
                        }
                    });

                }
            });

            $('#btnRejectRequestFranchisee').click(function (e) {
                var data = { FranchiseID: selectedFranchiseID,
                    FranchiseStatusID: selectedFranchiseStatusID,
                    IsFranchisee: 1
                };

                AjaxSend({
                    url: 'DashboardsAndReports/RejectFranchisee',
                    data: data,
                    success: function (vData) {
                        if (vData.FranchiseData == 'ok') {
                            NewMessageBox('Franchisee request has been rejected', 'top'); // 
                            showAllFranchisees('');
                        } else {
                            NewMessageBox('There was an error while processing your request');
                        }

                    } // end of success
                });
            });

            $('#btnFranchiseRequestApproveRequest').click(function (e) {
                var data = {
                    FranchiseID: selectedFranchiseID,
                    FranchiseStatusID: selectedFranchiseStatusID,
                    IsFranchisee: 0
                };
                Bs.Loading('show')
                AjaxSend({
                    url: 'DashboardsAndReports/ActivateFranchisee',
                    data: data,
                    success: function (vData) {
                        Bs.Loading('hide');
                        $('#dialogFranchisorStatus-bs').modal('hide');
                        if (vData.FranchiseData == 'ok') {
                            WebToast('Franchisor request has been approved', 'top'); //
                            showAllFranchiseRequests();
                        } else {
                            NewMessageBox('There was an error while processing your request');
                        }

                    }
                });
            });

            $('#btnFranchiseRequestRejectRequest').click(function (e) {
                var data = { FranchiseID: selectedFranchiseID,
                    FranchiseStatusID: selectedFranchiseStatusID,
                    IsFranchisee: 0
                };

                AjaxSend({
                    url: 'DashboardsAndReports/RejectFranchisee',
                    data: data,
                    success: function (vData) {
                        if (vData.FranchiseData == 'ok') {
                            WebToast('Franchisor request has been rejected', 'top'); // 
                            showAllFranchiseRequests();
                        } else {
                            NewMessageBox('There was an error while processing your request');
                        }

                    } // end of success
                });
            });
            $('#btnAddFranchiseGroup, .linkAddFranchiseGroup').click(function () {
                AjaxSend({
                    url: 'Login3/TestCookies',
                    success: function (vData) {

                        PromptUserAccess('Group Settings', {
                            OnClickOKSuccess: function () {
                                if (location.hash == '#&ui-state=dialog')
                                    history.back();

                                //                                                        location = MergeUrlParameters({
                                //                                                            OrgID: sessionStorage.OrgID
                                //                                                        }, '/Franchisor');
                                var ismob = CBool(IsMobile) ? 1 : 0;
                                window.open('/SetupChecklist/SystemAdministration?OrgID=' + sessionStorage.OrgID + '&IsMobile=' + ismob + '#LibraryPage', '_self')
                                sessionStorage["LibraryName"] = 'Group Settings';
                                sessionStorage["LabelLibraryName"] = 'Group';
                            }
                        });
                    },
                    error: function (vError) {
                        NewMessageBox("This feature requires internet connection.",
                                                    "Requires Internet Connection",
                                                    [
                                                        {
                                                            Text: 'Ok'
                                                        }
                                                    ]
                                                );
                        return;
                    }
                });
            });



            $('#btnFranchiseMessageCancel').click(function (e) {

                e.preventDefault();
                history.back();


            });

            $('#btnFranchiseCentralHome').click(function () {
                window.location = "/People/PeopleMenu";

            });

        }); //End Inner function


        function showAllFranchiseRequests() {

            AjaxSend({
                url: 'DashboardsAndReports/GetFranchiseRequests',
                success: function (vData) {

                    DisplayFranchisorList(vData.FranchiseData.Rows);

                } // end of success
            });
        }


        function showAllFranchisees(sString) {
            var data = { SearchString: sString
            };

            AjaxSend({
                url: 'DashboardsAndReports/GetFranchisees',
                data: data,
                success: function (vData) {

                    DisplayFranchiseeList(vData.FranchiseData.Rows);

                } // end of success
            });
        }
        function DisplayFranchiseeList(vRows) {
            var pLV = $("#tblFranchiseeManagerBody");

            pLV.html('');

            for (var i = 0; i <= vRows.length - 1; i++) {
                pItem = $('<tr style="cursor: pointer;">')

                         .append($('<td>')
                            .text(i + 1)
                        )
                        .append($('<td>')
                            .html(CStr2(vRows[i].FranchiseGroupName) != '' ? vRows[i].FranchiseGroupName : '<i>None</i>')

                        )
                        .append($('<td>')
                            .text(vRows[i].FranchiseeFowstName)

                        )
                        .append($('<td>')
                            .text(vRows[i].OrganizationName)
                        )
                        .append($('<td>')
                            .text(vRows[i].Address)
                        )
                         .append($('<td>')
                            .text($.format.date(new Date(vRows[i].LastUpdated)))
                        )
                        .append($('<td>')
                            .html(CStr2(vRows[i].DateActivated) != '' ? '<strong><font color="green">Active</font></strong>' : '<strong><font color="red">Pending</font></strong>')
                        )
                        .attr('ID', vRows[i].ID)
                        .attr('FranchiseStatusID', vRows[i].FranchiseStatusID)
                        .click(function (e) {

                            e.preventDefault();
                            selectedFranchiseID = $(this).attr('ID');
                            selectedFranchiseStatusID = $(this).attr('FranchiseStatusID');

                            ShowFranchiseePreview($(this).attr('ID'));


                        })

                pLV.append(pItem)

            }

            //            $('#tblFranchiseeManagerBody').table("refresh");
        }




        function ShowFranchiseePreview(franchiseID) {

            var pData = {};

            pData.FranchiseID = franchiseID;

            AjaxSend({
                url: 'DashboardsAndReports/GetFranchiseeStatus',
                data: pData,
                success: function (vData) {
                    if (vData.FranchiseData.Rows.length > 0) {
                        if (proj.IsBootstrapDesign()) {
                            $('#dialogFranchiseeStatus-bs').modal('show');
                        } else {
                            $('#dialogFranchiseeStatus').popup().popup('open');
                        }
                        
                        //                        $.mobile.changePage('#dialogFranchiseeStatus', 'pop', true, true);
                        $(".lblFranchiseeName").text('');
                        $(".lblFranchiseeName").text(vData.FranchiseData.Rows[0].FranchiseeFowstName);

                        $("#lblCurrentStatusFranchisee").text(CStr2(vData.FranchiseData.Rows[0].DateActivated) != '' ? 'Active' : 'Pending');
                        $("#selGroupList").val('');
                        $("#selFranchiseGroup").val(vData.FranchiseData.Rows[0].FranchiseGroupID);

                        if ($("#lblCurrentStatusFranchisee").text() == 'Active') {
                            $("#divPendingFranchiseeRequest").hide();
                            $("#divRequestApproval").hide();
                            $("#lblRequestedByFranchisee").text(vData.FranchiseData.Rows[0].RequestedByName);
                            $("#lblDateRequestedFranchisee").text($.format.date(new Date(vData.FranchiseData.Rows[0].DateRequested)));

                            $("#divApprovedFranchisee").show();
                            $("#btnAddFranchiseGroup").show();

                            $("#lblApprovedByFranchisee").text(vData.FranchiseData.Rows[0].ActivatedByName);
                            $("#lblDateActivatedFranchisee").text($.format.date(new Date(vData.FranchiseData.Rows[0].DateActivated)));

                        } else {

                            if (CDec2(vData.FranchiseData.Rows[0].IsFranchiseeConfirmed) > 0) {
                                $("#divRequestApproval").show();
                                $("#divPendingFranchiseeRequest").hide();
                                $("#btnAddFranchiseGroup").hide();

                                $("#lblFranchiseeNameLabelFranchisee").text(CStr2(vData.FranchiseData.Rows[0].FranchiseeFowstName) == '' ? 'this fowst' : vData.FranchiseData.Rows[0].FranchiseeFowstName);
                            }
                            else {
                                $("#divPendingFranchiseeRequest").show();
                                $("#divRequestApproval").hide();
                                $("#btnAddFranchiseGroup").show();
                                $("#lblPendingFranchiseeNameLabel").text(CStr2(vData.FranchiseData.Rows[0].FranchiseeFowstName) == '' ? 'this fowst' : vData.FranchiseData.Rows[0].FranchiseeFowstName);
                            }

                            $("#divApprovedFranchisee").hide();
                            $("#lblRequestedByFranchisee").text(vData.FranchiseData.Rows[0].RequestedByName);
                            $("#lblDateRequestedFranchisee").text($.format.date(new Date(vData.FranchiseData.Rows[0].DateRequested)));

                        }

                    } else {
                        NewMessageBox("There was an error while processing your request.");
                    }
                    //                    GetUpdatedStatus();
                },
                error: function (vError) {
                    var pDummy;
                }
            }); // AjaxSend

        }





        function getGroupList() {
            AjaxSend({
                url: 'Franchisor/GetAllFranchisorGroup',
                success: function (vData) {
                    var vRows = vData.Results.Rows;

                    $("#selFranchiseGroup").empty();

                    if (vRows.length <= 0) {

                        $("#divFranchiseeApproveReject").hide();

                    } else {
                        $("#divFranchiseeApproveReject").show();
                        for (var p = 0; p <= vRows.length - 1; p++) {
                            $("#selFranchiseGroup").append(new Option(vRows[p].GroupName, vRows[p].ID));

                        }
                        $("#selFranchiseGroup");//.selectmenu();
                        $("#selFranchiseGroup");//.selectmenu('refresh', true);
                    }

                } // end of success
            });
        }


        function DisplayFranchisorList(vRows) {
            var pLV = $("#tblFranchiseMessagesBody");

            pLV.html('');

            for (var i = 0; i <= vRows.length - 1; i++) {
                pItem = $('<tr style="cursor: pointer;">')

                         .append($('<td>')
                            .text(i + 1)
                        )
                        .append($('<td>')
                            .text($.format.date(new Date(vRows[i].LastUpdated)))
                        )
                        .append($('<td>')
                            .text(vRows[i].FowstName)
                        )
                        .append($('<td>')
                            .text(vRows[i].OrganizationName)
                        )
                        .append($('<td>')
                            .text(vRows[i].Address)
                        )
                        .append($('<td>')
                            .html(CStr2(vRows[i].DateActivated) != '' ? '<strong><font color="green">Active</font></strong>' : '<strong><font color="red">Pending</font></strong>')
                        )
                        .attr('ID', vRows[i].ID)
                        .attr('FranchiseStatusID', vRows[i].FranchiseStatusID)
                        .click(function (e) {

                            e.preventDefault();
                            selectedFranchiseID = $(this).attr('ID');
                            selectedFranchiseStatusID = $(this).attr('FranchiseStatusID');

                            ShowFranchisorPreview($(this).attr('ID'));


                        })

                pLV.append(pItem)

            }

            //            $('#tblFranchiseeManagerBody').table("refresh");
        }




        function ShowFranchisorPreview(franchiseID) {

            var pData = {};

            pData.FranchiseID = franchiseID;

            AjaxSend({
                url: 'DashboardsAndReports/GetFranchisorStatus',
                data: pData,
                success: function (vData) {
                    if (vData.FranchiseData.Rows.length > 0) {
                        if (proj.IsBootstrapDesign()) {
                            $('#dialogFranchisorStatus-bs').modal('show');
                        } else {
                            $('#dialogFranchisorStatus').popup().popup('open');
                        }
                        //                        $.mobile.changePage('#dialogFranchiseeStatus', 'pop', true, true);
                        $(".lblFranchisorName").text(vData.FranchiseData.Rows[0].OrganizationName);

                        $("#lblCurrentStatus").text(CStr2(vData.FranchiseData.Rows[0].DateActivated) != '' ? 'Active' : 'Pending');

                        if ($("#lblCurrentStatus").text() == 'Active') {
                            $("#divPending").hide();
                            $("#divPendingFranchisee").hide();
                            $("#lblRequestedBy").text(vData.FranchiseData.Rows[0].RequestedByName);
                            $("#lblDateRequested").text($.format.date(new Date(vData.FranchiseData.Rows[0].DateRequested)));

                            $("#divApproved").show();
                            $("#lblApprovedBy").text(vData.FranchiseData.Rows[0].ActivatedByName);
                            $("#lblDateActivated").text($.format.date(new Date(vData.FranchiseData.Rows[0].DateActivated)));


                        } else {

                            if (CDec2(vData.FranchiseData.Rows[0].IsFranchisorConfirmed) > 0) {
                                $("#divPending").show();
                                $("#divPendingFranchisee").hide();
                                $("#lblFranchisorNameLabel").text(vData.FranchiseData.Rows[0].OrganizationName);
                                $("#lblFranchiseeNameLabel").text(CStr2(vData.FranchiseData.Rows[0].FranchiseeFowstName) == '' ? 'this fowst' : vData.FranchiseData.Rows[0].FranchiseeFowstName);
                            }
                            else {
                                $("#divPendingFranchisee").show();
                                $("#divPending").hide();

                                $("#lblFranchisorNameLabel").text(vData.FranchiseData.Rows[0].OrganizationName);
                                $("#lblFranchiseeNameLabel").text(CStr2(vData.FranchiseData.Rows[0].FranchiseeFowstName) == '' ? 'this fowst' : vData.FranchiseData.Rows[0].FranchiseeFowstName);
                            }

                            $("#divApproved").hide();
                            $("#lblRequestedBy").text(vData.FranchiseData.Rows[0].RequestedByName);
                            $("#lblDateRequested").text($.format.date(new Date(vData.FranchiseData.Rows[0].DateRequested)));

                        }

                    } else {
                        NewMessageBox("There was an error while processing your request.");
                    }

                },
                error: function (vError) {
                    var pDummy;
                }
            }); // AjaxSend

        }

        //stupid table js


        // Helper function to convert a string of the form "Mar 15, 1987" into
        // a Date object.
        var date_from_string = function (str) {
            var months = ["jan", "feb", "mar", "apr", "may", "jun", "jul",
                          "aug", "sep", "oct", "nov", "dec"];
            var pattern = "^([a-zA-Z]{3})\\s*(\\d{2}),\\s*(\\d{4})$";
            var re = new RegExp(pattern);
            var DateParts = re.exec(str).slice(1);

            var Year = DateParts[2];
            var Month = $.inArray(DateParts[0].toLowerCase(), months);
            var Day = DateParts[1];
            return new Date(Year, Month, Day);
        }

        var moveBlanks = function (a, b) {

            if (a < b) {
                if (a == "")
                    return 1;
                else
                    return -1;
            }
            if (a > b) {
                if (b == "")
                    return -1;
                else
                    return 1;
            }
            return 0;
        };
        var moveBlanksDesc = function (a, b) {
            // Blanks are by definition the smallest value, so we don't have to
            // worry about them here
            if (a < b)
                return 1;
            if (a > b)
                return -1;
            return 0;
        };

        var table = $("table").stupidtable({

            "date": function (a, b) {
                // Get these into date objects for comparison.

                aDate = new Date(a);
                bDate = new Date(b);

                return aDate - bDate;
            },
            "moveBlanks": moveBlanks,
            "moveBlanksDesc": moveBlanksDesc
        });

        table.on("beforetablesort", function (event, data) {
            // data.column - the index of the column sorted after a click
            // data.direction - the sorting direction (either asc or desc)
            $("#msg").text("Sorting index " + data.column)
        });

        table.on("aftertablesort", function (event, data) {

            var th = $(this).find("th");
            th.find(".arrow").remove();
            var dir = $.fn.stupidtable.dir;

            var arrow = data.direction === dir.ASC ? "&uarr;" : "&darr;";
            th.eq(data.column).append('<span class="arrow">' + arrow + '</span>');




        });

        $("tr").slice(1).click(function () {

            $(".awesome").removeClass("awesome");
            $(this).addClass("awesome");
        });


        //end of stupid table


    });
