require([
    'common', 'tablesort'
], function () {

    $(function () {

        var IsLibraryDisplayInTable = false;
        var IsAddLibraryDisplayInUnivDiv = false;
        var headerData = {};
        var datasort = {};
        var arrSolution = [];
        var ctrSolution = 0;
        var editSolution = false;
        var arrSolutionSteps = [];
        var ctrSolutionStep = 0;
        var editSolutionStep = false;
        var arrIndexSolution, arrIndexStep, BreadCrumbsContainer = '.ItemsBreadCrumbs';

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
        sessionStorage.SystemAdminFor = 'crm';

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
            $("#CRMmsg").text("Sorting index " + data.column)
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

        window.GetKMData = {
            DisplaySolution: function (arr) {
                arrSolution = arr;
                DisplaySolution();
            }
        }

        $(document).ready(function () {
            $('#CRMdivContent').css('margin-top', $('.divHeader').height());

            var breadcrumbs = $('<div class="ItemsBreadCrumbs"></div>'),
                arrowR = ' <span class="ui-icon ui-icon-arrow-r" style="display:inline-block; margin:0px 10px 0px 10px;"></span>',
                lnkSysAdd = '<a data-role="none" id="CRMbtnSystemAdministration">System Administration</a>';

            var date = new Date();
            var month = ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"];
            //$('.lblPeriod').text(month[date.getMonth()] + ' ' + date.getDate() + ', ' + date.getFullYear());
            location.hash = '';
            $(window).bind('hashchange', function () {
                if (sessionStorage.SystemAdminFor != 'crm') {
                    return;
                }
                if (window.location.hash == "#SystemAdministration" || window.location.hash == "") {
                    //$('#CRMdivSystemAdDashboard').show();
                    $('#CRMUniversalDiv, #CRMdivSubList, #CRMdivForPIM').hide(); //#CRMdivSecond,
                    $('#CRMUniversalDiv').empty();
                    //$(BreadCrumbsContainer).text('System Administration');
                    sessionStorage.removeItem("AddViewLibBCName");
                    sessionStorage.removeItem("AddLibraryName");
                    sessionStorage.removeItem("LabelLibraryName");
                    //sessionStorage.removeItem("LibraryName");
                    sessionStorage.removeItem("SubListName");
                    sessionStorage.removeItem("SecondSubListName");
                    sessionStorage.removeItem("OptionToAdd");
                    sessionStorage.removeItem("btnOrgEmployeeName");
                    sessionStorage.removeItem("DevicePrinterInfoUsedby");
                    sessionStorage.removeItem("PIMUsedby");
                    sessionStorage.removeItem("ViewID");
                    sessionStorage.removeItem("ScheduleID");
                    sessionStorage.removeItem("OrgFowstID");
                    sessionStorage.removeItem("EmpFirstName");
                    sessionStorage.removeItem("EmployeeStatus");
                    sessionStorage.removeItem("ItemModifierID");
                    sessionStorage.removeItem("ItemModifierName");
                    //AddBreadCrumb({
                    //    text: 'System Administration', Click: function () {
                    //        window.location.hash = 'SystemAdministration';
                    //    }
                    //}, BreadCrumbsContainer)
                }
                else if (window.location.hash == "#LibraryPage") {
                    $(window).scrollTop(0);
                    sessionStorage.removeItem("OptionToAdd");
                    sessionStorage.removeItem("DevicePrinterInfoUsedby");
                    sessionStorage.removeItem("AddViewLibBCName");
                    sessionStorage.removeItem("ViewID");
                    sessionStorage.removeItem("ScheduleID");
                    sessionStorage.removeItem("OrgFowstID");
                    sessionStorage.removeItem("EmpFirstName");
                    sessionStorage.removeItem("EmployeeStatus");
                    //$('.BreadCrumbs').html(breadcrumbs.prepend(lnkSysAdd));
                    //BreadCrumbsClickEvent();

                    $('.lblAddLibraryName, .lblPopUpHeaderLibraryName, .lblAddLinkLibraryName').text(sessionStorage.LabelLibraryName);
                    $('#CRMtxtSearch').attr('placeholder', 'Type name of ' + sessionStorage.LabelLibraryName);

                    if (IsLibraryDisplayInTable == false) {
                        //$(BreadCrumbsContainer).text('');

                        $('#CRMdivSystemAdDashboard, #CRMUniversalDiv, #CRMdivSubList, #CRMdivForPIM').hide();
                        //$('#CRMUniversalDiv').empty();
                        $('#CRMdivSecond').show();

                        if (sessionStorage.LibraryName == 'Sales Team') {
                            breadcrumbs.prepend(arrowR, sessionStorage.LibraryName);
                            headerData = ['#', 'Name', 'Description', 'Last Modified Date'];
                            datasort = ['int', 'string', 'string', 'date'];
                            DisplayTableLibrary(headerData, datasort);
                        }

                        else if (sessionStorage.LibraryName == 'Activity Quota') {
                            breadcrumbs.prepend(arrowR, sessionStorage.LibraryName);
                            headerData = ['Activity No.', 'Name', 'Date Covered', 'Last Updated', 'Modified By'];
                            datasort = ['int', 'string', 'date', 'date', 'string'];
                            DisplayTableLibrary(headerData, datasort);
                        }

                        else if (sessionStorage.LibraryName == 'Client Staging') {
                            breadcrumbs.prepend(arrowR, sessionStorage.LibraryName);
                            headerData = ['#', 'Name', 'Last Updated', 'Modified By'];
                            datasort = ['int', 'string', 'date', 'string'];
                            DisplayTableLibrary(headerData, datasort);
                        }

                        else if (sessionStorage.LibraryName == 'Survey Creator') {
                            breadcrumbs.prepend(arrowR, sessionStorage.LibraryName);
                            headerData = ['#', 'Name', 'Last Updated', 'Modified By'];
                            datasort = ['int', 'string', 'date', 'string'];
                            DisplayTableLibrary(headerData, datasort);
                        }

                        else if (sessionStorage.LibraryName == 'Ticket Notes') {
                            breadcrumbs.prepend(arrowR, sessionStorage.LibraryName);
                            headerData = ['#', 'Notes', 'Default', 'Last Updated', 'Modified By'];
                            datasort = ['int', 'string', 'string', 'date', 'string'];
                            DisplayTableLibrary(headerData, datasort);
                        }

                        else if (sessionStorage.LibraryName == 'Funnel Templates') {
                            breadcrumbs.prepend(arrowR, sessionStorage.LibraryName);
                            headerData = ['#', 'Template Name', 'Last Updated', 'Modified By'];
                            datasort = ['int', 'string', 'date', 'string'];
                            DisplayTableLibrary(headerData, datasort);
                        }

                        ResizeHeightOfRequiredData();
                    }
                    else {
                        //$(BreadCrumbsContainer).text('');
                        //breadcrumbs.prepend(arrowR, sessionStorage.LibraryName);
                        //AddBreadCrumb({
                        //    text: sessionStorage.LibraryName, Click: function () {
                        //        window.location.hash = 'LibraryPage';
                        //        $('#btnMenuProductInformation').click();
                        //        if (headerData.length != undefined) {
                        //            DisplayTableLibrary(headerData, datasort);
                        //        }
                        //    }
                        //}, BreadCrumbsContainer)
                        $('#CRMdivSystemAdDashboard, #CRMUniversalDiv, #CRMdivSubList, #CRMdivForPIM').hide();
                        $('#CRMUniversalDiv').empty();
                        $('#CRMdivSecond').show();
                    }

                    //$('.BreadCrumbs').html(breadcrumbs.prepend(lnkSysAdd));
                    //BreadCrumbsClickEvent();
                    $('body').animate({ scrollTop: 0 });
                }

                else if (window.location.hash == "#UniversalPage") {

                    $('#CRMdivSystemAdDashboard, #CRMdivSecond, #CRMdivSubList, #CRMdivForPIM').hide();
                    $('#CRMUniversalDiv').show();

                    if (IsAddLibraryDisplayInUnivDiv == false) {
                        GoToThisPage();
                    }

                    //$('.BreadCrumbs').html(breadcrumbs.prepend(lnkSysAdd));
                    //BreadCrumbsClickEvent();
                    $('body').animate({ scrollTop: 0 });
                }

                else if (window.location.hash == "#CRMSubListPage") {
                    $(BreadCrumbsContainer).text('');

                    if (sessionStorage.SubListName != undefined) {
                        breadcrumbs.prepend(arrowR, sessionStorage.SubListName);
                        breadcrumbs.prepend(arrowR, '<a data-role="none" id="CRMbtnSublistPage">' + sessionStorage.LibraryName + '</a>');
                    }

                    $('.BreadCrumbs').html(breadcrumbs.prepend(lnkSysAdd));
                    BreadCrumbsClickEvent();

                    window.location.hash = "LibraryPage";

                    $('body').animate({ scrollTop: 0 });
                }
                ClearAddLibraryFieldsPopUp();
            })
            .resize(function () {
                ResizeHeightOfRequiredData();
            })

            $(window)
            //.trigger('hashchange')
            .resize(function () {
                $('#CRMdivContent').css('margin-top', $('.divHeader').height());
            });

            if (location.pathname != '/erp') {
                //BreadCrumbsContainer = '.ItemsBreadCrumbs';
                BreadCrumbsContainer = '.erpItemBCContainer';
            } else {
                BreadCrumbsContainer = '.erpItemBCContainer';
            }

            //var tempinterval = setInterval(function () {
            //    if (typeof AddBreadCrumb != 'undefined') {
            //        $(BreadCrumbsContainer).empty();
            //        AddBreadCrumb({
            //            text: 'System Administration', Click: function () {
            //                window.location.hash = 'SystemAdministration';
            //            }
            //        }, BreadCrumbsContainer)
            //        clearInterval(tempinterval)
            //    }
            //}, 100)

            GetOrganizationName();
            ResizeHeightOfRequiredData();
            //ShowCheckList();

            $('#CRMbtnPrintPOCSlip').click(function () {
                var intFAFCTotalLenght = 43;
                var strPrinterIP = "192.168.1.219";
                var strPrint;

                NewPrintBuilder();

                strPrint = FormatTextToPrint(intFAFCTotalLenght, "Employee PoC Slip", intFAFCTotalLenght, '', 0, '', 0, 1);
                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 1, 1, 0, 0, 0);

                strPrint = '';
                strPrint = FormatTextToPrint(intFAFCTotalLenght, "(Purchase on Credit)", intFAFCTotalLenght, '', 0, '', 0, 1);
                ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                ReadyTextToPrint('\n', 3, 1, 1, 0, 0, 1, 0, 0, 0);
                ReadyTextToPrint('\n', 3, 1, 1, 0, 0, 1, 0, 0, 0);

                strPrint = '';
                strPrint = FormatTextToPrint(intFAFCTotalLenght, "Employee Name", 15, ' : Juan Dela Cruz', intFAFCTotalLenght, '', 0, 1);
                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                ReadyTextToPrint('\n', 3, 1, 1, 0, 0, 1, 0, 0, 0);

                strPrint = '';
                strPrint = FormatTextToPrint(intFAFCTotalLenght, "FOWST Name", 15, ' : Aficionado SM Lanang', intFAFCTotalLenght, '', 0, 1);
                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                ReadyTextToPrint('\n', 3, 1, 1, 0, 0, 1, 0, 0, 0);

                strPrint = '';
                strPrint = FormatTextToPrint(intFAFCTotalLenght, "Date & Time of  Purchase", 15, ' : June 30, 2015 - 5:35pm', intFAFCTotalLenght, '', 0, 1);
                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                ReadyTextToPrint('\n', 3, 1, 1, 0, 0, 1, 0, 0, 0);

                strPrint = '';
                strPrint = FormatTextToPrint(intFAFCTotalLenght, "SI Reference", 15, ' : 2954684216', intFAFCTotalLenght, '', 0, 1);
                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                ReadyTextToPrint('\n', 3, 1, 1, 0, 0, 1, 0, 0, 0);

                strPrint = '';
                strPrint = FormatTextToPrint(intFAFCTotalLenght, "Sales Invoice (SI) Amount", 15, ' : 10542.00', intFAFCTotalLenght, '', 0, 1);
                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                ReadyTextToPrint('\n', 3, 1, 1, 0, 0, 1, 0, 0, 0);

                strPrint = '';
                strPrint = FormatTextToPrint(intFAFCTotalLenght, "Purchase on Credit (PoC) Amount", 15, ' : 10542.00', intFAFCTotalLenght, '', 0, 1);
                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                ReadyTextToPrint('\n', 3, 1, 1, 0, 0, 1, 0, 0, 0);

                strPrint = '';
                strPrint = FormatTextToPrint(intFAFCTotalLenght, "Cashier Name", 15, ' : Maria J. Santos', intFAFCTotalLenght, '', 0, 1);
                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                ReadyTextToPrint('\n', 3, 1, 1, 0, 0, 1, 0, 0, 0);
                ReadyTextToPrint('\n', 3, 1, 1, 0, 0, 1, 0, 0, 0);
                ReadyTextToPrint('\n', 3, 1, 1, 0, 0, 1, 0, 0, 0);

                strPrint = '';
                strPrint = FormatTextToPrint(intFAFCTotalLenght, "Please sign below", intFAFCTotalLenght, '', 0, '', 0, 1);
                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                ReadyTextToPrint('\n\n\n', 3, 1, 1, 0, 0, 1, 0, 0, 0);

                strPrint = '';
                strPrint = FormatTextToPrint(intFAFCTotalLenght, "___________________________", intFAFCTotalLenght, '', 0, '', 0, 1);
                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);

                strPrint = '';
                strPrint = FormatTextToPrint(intFAFCTotalLenght, "Juan Dela Cruz", intFAFCTotalLenght, '', 0, '', 0, 1);
                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 1);

                PrintText(strPrinterIP);
                ClearCommandBuffer();
            })

            $('.btnHome').click(function () {
                location = MergeUrlParameters({
                    OrgID: sessionStorage.OrgID,
                    IsMobile: CBool(IsMobile) ? 1 : 0
                }, '/CRMDashboard');
                //window.location = '/People/PeopleMenu';
                //sessionStorage.removeItem("OrgName");
            })

            $('.btnMoreOption').click(function () {
                $('#CRMpanOption').modal('show');
            })

            

            $('#CRMbtnSetupSecurity, #CRMpopupbtnSecurityRoles').click(function () {
                //NewMessageBox('This feature is ongoing development', 'Message');
                //return;
                sessionStorage.IsCrm = true;
                NewPromptUserAccess('Security Roles', {
                    OnClickOKSuccess: function () {
                        //$('#CRMpanOption').modal('hide');
                        window.location.hash = "LibraryPage";
                        headerData = ['#', 'Code Name', 'Role', 'Description', 'Last Modified By', 'Last Modified Date'];
                        datasort = ['int', 'string', 'string', 'string', 'string', 'date'];
                        sessionStorage["LibraryName"] = 'Security Roles';
                        sessionStorage["LabelLibraryName"] = 'Security Role';
                        sessionStorage["AddViewLibBCName"] = "Add Security Role";
                        DisplayTableLibrary(headerData, datasort);
                    }
                })
            })

            $('#CRMbtnSalesTeam, #CRMpopupbtnSalesTeam').click(function () {
                $('#CRMpanOption').modal('hide');
                window.location.hash = '';
                window.location.hash = "LibraryPage";
                headerData = ['#', 'Name', 'Description', 'Last Modified Date'];
                datasort = ['int', 'string', 'string', 'date'];
                sessionStorage["LibraryName"] = 'Sales Team';
                sessionStorage["LabelLibraryName"] = 'Team';
                DisplayTableLibrary(headerData, datasort);
            })

            $('#CRMbtnCRMSettings, #CRMpopupbtnCRMSettings').click(function () {
                $('#CRMpanOption').modal('hide');
                window.location.hash = '';
                sessionStorage["LibraryName"] = 'CRM Settings';
                GoToThisPage();
            })

            $('#CRMbtnActivityQuota, #CRMpopupbtnActivityQuota').click(function () {
                NewPromptUserAccess('Activity Quota', {
                    OnClickOKSuccess: function () {
                        $('#CRMpanOption').modal('hide');
                        window.location.hash = '';
                        window.location.hash = "LibraryPage";
                        headerData = ['Activity No.', 'Name', 'Date Covered', 'Last Updated', 'Modified By'];
                        datasort = ['int', 'string', 'date', 'date', 'string'];
                        sessionStorage["LibraryName"] = 'Activity Quota';
                        sessionStorage["LabelLibraryName"] = 'Activity';
                        DisplayTableLibrary(headerData, datasort);
                    }
                })
            })

            $('#CRMbtnClientStaging, #CRMpopupbtnClientStaging').click(function () {
                NewPromptUserAccess('Client Staging', {
                    OnClickOKSuccess: function () {
                        //$('#CRMpanOption').modal('hide');
                        window.location.hash = '';
                        window.location.hash = "LibraryPage";
                        headerData = ['#', 'Name', 'Last Updated', 'Modified By'];
                        datasort = ['int', 'string', 'date', 'string'];
                        sessionStorage["LibraryName"] = 'Client Staging';
                        sessionStorage["LabelLibraryName"] = 'Stage';
                        DisplayTableLibrary(headerData, datasort);
                    }
                })
            })

            $('#CRMbtnSurveyCreator, #CRMpopupbtnSurveyCreator').click(function () {
                NewMessageBox('This feature is ongoing development', 'Message');
                return;
                NewPromptUserAccess('Survey Creator', {
                    OnClickOKSuccess: function () {
                        $('#CRMpanOption').modal('hide');
                        window.location.hash = '';
                        window.location.hash = "LibraryPage";
                        headerData = ['#', 'Name', 'Last Updated', 'Modified By'];
                        datasort = ['int', 'string', 'date', 'string'];
                        sessionStorage["LibraryName"] = 'Survey Creator';
                        sessionStorage["LabelLibraryName"] = 'Survey';
                        DisplayTableLibrary(headerData, datasort);
                    }
                })
            })

            $('#CRMbtnTicketNotes, #CRMpopupbtnTicketNotes').click(function () {
                NewPromptUserAccess('Ticket Notes', {
                    OnClickOKSuccess: function () {
                        $('#CRMpanOption').modal('hide');
                        window.location.hash = '';
                        window.location.hash = "LibraryPage";
                        headerData = ['#', 'Notes', 'Default', 'Last Updated', 'Modified By'];
                        datasort = ['int', 'string', 'string', 'date', 'string'];
                        sessionStorage["LibraryName"] = 'Ticket Notes';
                        sessionStorage["LabelLibraryName"] = 'Note';
                        DisplayTableLibrary(headerData, datasort);
                    }
                })
            })

            $('#CRMbtnFunnelTemplate, #CRMpopupbtnFunnelTemplate').click(function () {
                NewPromptUserAccess('Funnel Templates', {
                    OnClickOKSuccess: function () {
                        $('#CRMpanOption').modal('hide');
                        window.location.hash = '';
                        window.location.hash = "LibraryPage";
                        headerData = ['#', 'Template Name', 'Last Updated', 'Modified By'];
                        datasort = ['int', 'string', 'date', 'string'];
                        sessionStorage["LibraryName"] = 'Funnel Templates';
                        sessionStorage["LabelLibraryName"] = 'Template';
                        DisplayTableLibrary(headerData, datasort);
                    }
                })
            })

            $('#CRMpopupbtnKnowledgeManagementAndQuestionaire, #CRMpopupbtnKnowledgeManagementAndQuestionaire').click(function () {
                $('#CRMpanOption').modal('hide');
                window.location.hash = '';
                window.location.hash = "LibraryPage";
                headerData = ['Problem', 'Solution', 'Topic', 'Product', 'Language', 'Edited By', 'Last Edit Date'];
                datasort = ['string', 'string', 'string', 'string', 'string', 'string', 'date'];
                sessionStorage["LibraryName"] = 'Knowledge Management';
                sessionStorage["LabelLibraryName"] = 'Case';
                DisplayTableLibrary(headerData, datasort);
                $('#CRMtblBody').empty();
            })

            $('#CRMbtnAddLibrary').click(function () {
                if (sessionStorage.LibraryName == "Activity Quota") {
                    sessionStorage["AddViewLibBCName"] = "New Activity Quota";
                    window.location.hash = 'UniversalPage';
                    CRMLibraryTable.AddActivityQuotaPage();
                }
                else if (sessionStorage.LibraryName == "Client Staging") {
                    sessionStorage["AddViewLibBCName"] = "New Client Stage";
                    window.location.hash = 'UniversalPage';
                    CRMLibraryTable.AddClientStagePage();
                }
                else if (sessionStorage.LibraryName == "Sales Team") {
                    sessionStorage["AddViewLibBCName"] = "Add Team";
                    window.location.hash = 'UniversalPage';
                    CRMLibraryTable.AddCRMPage();
                }
                else if (sessionStorage.LibraryName == "Survey Creator") {
                    sessionStorage["AddViewLibBCName"] = "New Survey";
                    window.location.hash = 'UniversalPage';
                    CRMLibraryTable.AddSurveyPage();
                }
                else if (sessionStorage.LibraryName == "Ticket Notes") {
                    sessionStorage["AddViewLibBCName"] = "New Ticket Note";
                    window.location.hash = 'UniversalPage';
                    CRMLibraryTable.ViewTicketNote();
                }
                else if (sessionStorage.LibraryName == "Funnel Templates") {
                    sessionStorage["AddViewLibBCName"] = "New Template";
                    window.location.hash = 'UniversalPage';
                    CRMLibraryTable.ViewFunnelTemplate();
                }
                else {
                    $('#CRMpanAddLibrary').modal('show');
                    $('#CRMtxtSearch').focus();
                }
            })

            
            $('#CRMbtnCancelLibrary').click(function () {
                $('#CRMpanAddLibrary').modal('hide');
                ClearAddLibraryFieldsPopUp();
            })

            $('.btnClosePopUp').click(function () {
                ClearAddLibraryFieldsPopUp();
            })

            $('#CRMbtnSearchLibrary').click(function () {
                SearchLibrary($('#CRMtxtSearch').val());
            })

            $('#CRMtxtSearch').keyup(function (e) {
                if (e.keyCode == 13) {
                    SearchLibrary($(this).val());

                }
                else if ($(this).val() == "") {
                    ClearAddLibraryFieldsPopUp();
                }
            })

            $('#CRMbtnAddToLibrary').click(function () {
                $('#CRMUniversalDiv').empty();
                if (sessionStorage.LibraryName == "FOWST") {
                    sessionStorage["AddViewLibBCName"] = "Add FOWST";
                    sessionStorage["FowstTitle"] = "Organization Address";
                }
                else if (sessionStorage.LibraryName == "Authorized Devices") {
                    sessionStorage["AddViewLibBCName"] = "Add Device";
                }
                else if (sessionStorage.LibraryName == "Receipt Settings") {
                    sessionStorage["AddViewLibBCName"] = "Add Receipt Template";
                }
                else if (sessionStorage.LibraryName == "User") {
                    sessionStorage["AddViewLibBCName"] = "Add User";
                    sessionStorage["OptionToAdd"] = "Add User";
                }
                else if (sessionStorage.LibraryName == "Security Roles") {
                    sessionStorage["AddViewLibBCName"] = "Add Security Role";
                }
                else if (sessionStorage.LibraryName == "Hot Buttons Settings") {
                    sessionStorage["AddViewLibBCName"] = "Add Hot Button";
                }
                else if (sessionStorage.LibraryName == "Customer / Client List") {
                    sessionStorage["AddViewLibBCName"] = "Add New Customer";
                }
                else if (sessionStorage.LibraryName == "Supplier / Vendor List ") {
                    sessionStorage["AddViewLibBCName"] = "Add New Supplier";
                }
                else if (sessionStorage.LibraryName == "Printer Roles") {
                    sessionStorage["AddViewLibBCName"] = "Add Printer Role";
                }
                else if (sessionStorage.LibraryName == "Sales Team") {
                    sessionStorage["AddViewLibBCName"] = "Add Team";
                }
                else if (sessionStorage.LibraryName == "Knowledge Management") {
                    sessionStorage["AddViewLibBCName"] = "Knowledge Management";
                    clearKMFields();
                }

                GoToThisPage($('#CRMtxtSearch').val());
            })
            $('#CRMbtnOKOrgEmp, #CRMbtnSaveAndSend').click(function (e) {
                e.preventDefault();
                $('#CRMpanOrgEmpMessage').modal('hide');
            })
            $('#CRMbtnCancelOrgEmp, #CRMbtnCancelEmail').click(function (e) {
                e.preventDefault();
                $('#CRMpanOrgEmpMessage').modal('hide');
            })
                        
            $('#CRMformsearchdesktop, #CRMformsearchmobile').submit(function (e) {
                e.preventDefault();
                SearchInTable($('.txtSearchdesktop').val() || $('.txtSearchmobile').val());
            })

            $('.btnSearch').click(function () {
                SearchInTable($('.txtSearchdesktop').val());
            })

            var kburlid;
            $('#CRMbtnKnowledgebase').click(function () {
                Bs.Loading('show');

                AjaxSend({
                    url: 'CRMDashboard/KbUrl',
                    success: function (e) {
                        Bs.Loading('hide');
                        $('#CRMpanKbUrl').modal('show');
                        $('#CRMtxtKbUrl').val((e.Results.Rows.length > 0 ? CStr2(e.Results.Rows[0].DefaultValue) : '')).focus();
                        kburlid = (e.Results.Rows.length > 0 ? CStr2(e.Results.Rows[0].ID) : '');
                    }
                })
            })

            $('#CRMbtnSaveKbUrl').click(function () {
                if ($.trim($('#CRMtxtKbUrl').val()) == '') {
                    WebToast('Please enter your KB url', 'top');
                    return
                }

                if (!IsUrl($.trim($('#CRMtxtKbUrl').val()))) {
                    WebToast('Please enter valid url', 'top');
                    return
                }

                NewMessageBox('Please wait while saving ...', 'Message');

                AjaxSend({
                    url: 'CRMDashboard/SaveKbUrl',
                    data: {
                        Url: $.trim($('#CRMtxtKbUrl').val()),
                        ID: kburlid
                    },
                    success: function (e) {
                        if (e.Results == 'saved') {
                            NewMessageBoxClose();
                            WebToast('Successfully Saved', 'top');
                            $('#CRMpanKbUrl').modal('hide');
                        } else {
                            NewMessageBox(e.Results, 'Error');
                        }                        
                    }
                })
            })

            $('#CRMbtnSetupSecurity').click();
        })// End document

        function DisplaySolution() {
            var listSolution = $('#CRMlistSolution');

            listSolution.empty();

            if (arrSolution != null) {
                for (var i = 0; i < arrSolution.length; i++) {
                    arrSolution[i].No = i + 1;
                    if (arrSolution[i].State != 'Remove') {
                        var ol = $('<ol>');
                        if (arrSolution[i].Step.length > 0) {
                            for (var m = 0; m < arrSolution[i].Step.length; m++) {
                                if (arrSolution[i].Step[m].State != "Remove") {
                                    ol.append($('<li>')
                                    .html(arrSolution[i].Step[m].Step.replace(/\r\n|\r|\n/g, "<br />"))
                                )
                                }
                            }
                        }
                        else {
                            ol = '';
                        }
                        listSolution.append($('<li>')
                        .append($('<div class="todoContainer" style="display:table; width:100%;">')
                            .append($('<div style="float:none; display:table-cell; width:auto; overflow:hidden; padding:8px; word-break:break-all;">')
                                .data(arrSolution[i])
                                .html(arrSolution[i].Solution.replace(/\r\n|\r|\n/g, "<br />") + '<br><br>')
                                .append(ol)
                                .click(function () {
                                    arrIndexSolution = CDec2($(this).data().No) - 1;
                                    editSolution = true;
                                    $('#CRMpanAddSolution').show();
                                    $('#CRMtxtSolutionAndAnswer').val($(this).data().Solution).focus();
                                    AddSolutionStep($(this).data().Step)
                                    arrSolutionSteps = $(this).data().Step;
                                })
                            )
                            .append($('<div style="padding:10px; display:table-cell; width:30px; vertical-align:middle;">')
                            .data(arrSolution[i])
                            .append($('<button data-role="none" title="Remove" class="btn custom-btn-theme-c btnCircle-xs btnShadow">')
                                .append('<img alt="Remove" src="/Content/images/close-black.png" />')
                            )
                            .click(function () {
                                var no = $(this).data().No;

                                if ($(this).data().State == 'Add') {
                                    arrSolution = $.grep(arrSolution, function (e) {
                                        return e.No != no;
                                    })
                                }
                                else {
                                    arrSolution[no - 1].State = 'Remove';
                                }
                                DisplaySolution();
                            })
                        )
                        )
                    )
                    }
                }
            }
        }

        function NewSteps() {
            if ($.trim($('#CRMtxtNewSteps').val()) == '') {
                $('#CRMtxtNewSteps').focus();
                return;
            }

            if (editSolutionStep == false) {
                ctrSolutionStep = ctrSolutionStep + 1;

                arrSolutionSteps.push({
                    StepID: '',
                    //No: ctrSolutionStep,
                    Step: $('#CRMtxtNewSteps').val(),
                    State: 'Add'
                })
            }
            else {
                arrSolutionSteps[arrIndexStep].Step = $('#CRMtxtNewSteps').val();
            }


            AddSolutionStep(arrSolutionSteps);
            $('#CRMtxtNewSteps').val('');
            editSolutionStep = false;
        }

        function AddSolutionStep(arrSolutionSteps) {
            var listSolutionSteps = $('#CRMlistSolutionSteps');

            listSolutionSteps.empty();

            for (var i = 0; i < arrSolutionSteps.length; i++) {
                arrSolutionSteps[i].No = i + 1;
                if (arrSolutionSteps[i].State != 'Remove') {
                    listSolutionSteps.append($('<li>')
                                .append($('<div class="todoContainer" style="display:table; width:100%;">')
                                    .css('cursor', 'pointer')
                                    .append($('<div style="float:none; display:table-cell; width:auto; overflow:hidden; padding:8px; word-break:break-all;">')
                                        .data(arrSolutionSteps[i])
                                        .html(arrSolutionSteps[i].Step.replace(/\r\n|\r|\n/g, "<br />")
                                        )
                                        .click(function () {
                                            editSolutionStep = true;
                                            $('#CRMpanAddSteps').modal('show');
                                            $('#CRMtxtNewSteps').val($(this).data().Step).focus();
                                            arrIndexStep = CDec2($(this).data().No) - 1;
                                        })
                                    )
                                    .append($('<div style="padding:10px; display:table-cell; width:30px; vertical-align:middle;">')
                                        .data(arrSolutionSteps[i])
                                        .append($('<button data-role="none" title="Remove" class="btn custom-btn-theme-c btnCircle-xs btnShadow">')
                                            .append('<img alt="Remove" src="/Content/images/close-black.png" />')
                                        )
                                        .click(function () {
                                            var no = $(this).data().No;

                                            if ($(this).data().State == 'Add') {
                                                arrSolutionSteps = $.grep(arrSolutionSteps, function (e) {
                                                    return e.No != no;
                                                })
                                            }
                                            else {
                                                arrSolutionSteps[no - 1].State = 'Remove';
                                            }
                                            AddSolutionStep(arrSolutionSteps);
                                        })
                                    )
                                )
                            )
                }
            }
        }

        function ClearSolutionField() {
            $('#CRMtxtSolutionAndAnswer').val('');
            $('#CRMlistSolutionSteps').empty();
        }

        function SearchInTable(s) {
            if (sessionStorage.LibraryName == "FOWST") {
                CRMLibraryTable.Fowst(s);
            }
            else if (sessionStorage.LibraryName == "Authorized Devices") {
                CRMLibraryTable.Devices(s);
            }
            else if (sessionStorage.LibraryName == "Receipt Settings") {
                CRMLibraryTable.ReceiptSettings(s);
            }
            else if (sessionStorage.LibraryName == "Security Roles") {
                CRMLibraryTable.SecurityRoles(s);
            }
            else if (sessionStorage.LibraryName == "User") {
                CRMLibraryTable.Employees(s);
            }
            else if (sessionStorage.LibraryName == "PIM") {
                CRMLibraryTable.PIMPage(s);
            }
            else if (sessionStorage.LibraryName == "Printer Roles") {
                CRMLibraryTable.PrinterRoles(s);
            }
            else if (sessionStorage.LibraryName == "Hot Buttons Settings") {
                CRMLibraryTable.HotButtons(s);
            }
            else if (sessionStorage.LibraryName == "Group Settings") {
                CRMLibraryTable.GroupSettings(s);
            }
            else if (sessionStorage.LibraryName == "Customer / Client List") {
                CRMLibraryTable.CustomerList(s);
            }
            else if (sessionStorage.LibraryName == "Supplier / Vendor List") {
                CRMLibraryTable.SupplierList(s);
            }
            else if (sessionStorage.LibraryName == "Sales Team") {
                CRMLibraryTable.SalesTeam(s);
            } else if (sessionStorage.LibraryName == 'Item Modifiers') {
                CRMLibraryTable.SearchItemModifiers(s);
            }
        }

        function ResizeHeightOfRequiredData() {
            $('.divSetupOrganizationVal').height($('.divSetupOrganization').height());
            $('.divSetupOrganizationFOWSTVal').height($('.divSetupOrganizationFOWST').height());
            $('.divSetupDeviceVal').height($('.divSetupDevice').height());
            $('.divSetupReceiptSettingsVal').height($('.divSetupReceiptSettings').height());
            $('.divSetupSecurityVal').height($('.divSetupSecurity').height());
            $('.divSetupEmployeeVal').height($('.divSetupEmployee').height());
            $('.divSetupProductsVal').height($('.divSetupProducts').height());
            $('.divQuantitySettingVal').height($('.divQuantitySetting').height());
            $('.divPOSSettingVal').height($('.divPOSSetting').height());
            $('.divSalesOrderSettingVal').height($('.divSalesOrderSetting').height());
            $('.divHotButtonsVal').height($('.divHotButtons').height());
            $('.divGroupSettingsVal').height($('.divGroupSettings').height());
        }

        function BreadCrumbsClickEvent() {
            $('#CRMbtnSystemAdministration').click(function () {
                window.location.hash = 'SystemAdministration';
            })

            $('#CRMbtnLibraryPage').click(function () {
                window.location.hash = 'LibraryPage';
                $('#CRMbtnMenuProductInformation').click();
                if (headerData.length != undefined) {
                    DisplayTableLibrary(headerData, datasort);
                }
            })

            $('#CRMbtnSublistPage').click(function () {
                sessionStorage.removeItem('SubListName');
                window.location.hash = '';
                window.location.hash = 'LibraryPage';
            })

            $('#CRMbtnSublistParentPage').click(function () {
                window.location.hash = 'LibraryPage';
            })

            $('#CRMbtnSublistSecondChildPage').click(function () {
                window.location.hash = 'LibraryPage';
            })
        }

        function ClearAddLibraryFieldsPopUp() {
            $('#CRMlstSearchFound').empty();
            $('#CRMdivNoResultsFound, .divLibAddOption').hide();
            $('#CRMtxtSearch').val('');
        }

        function DisplayTableLibrary(HeaderData, DataSortType) {
            $('#CRMtblHeader').empty();

            for (var i = 0; i < HeaderData.length; i++) {
                $('#CRMtblHeader').append($('<th>')
                                            .text(HeaderData[i])
                                            .attr('data-sort', DataSortType[i])
                                            .css('cursor', 'pointer')
                                        );

            }

            if (sessionStorage.LibraryName == "FOWST") {
                CRMLibraryTable.Fowst();
            }
            else if (sessionStorage.LibraryName == "Authorized Devices") {
                CRMLibraryTable.Devices();
            }
            else if (sessionStorage.LibraryName == "Receipt Settings") {
                CRMLibraryTable.ReceiptSettings();
            }
            else if (sessionStorage.LibraryName == "Security Roles") {
                CRMLibraryTable.SecurityRoles();
            }
            else if (sessionStorage.LibraryName == "User") {
                $('#CRMdivWorkSchedule').show();
                CRMLibraryTable.Employees();
            }
            else if (sessionStorage.LibraryName == "Printer Roles") {
                CRMLibraryTable.PrinterRoles();
            }
            else if (sessionStorage.LibraryName == "PIM") {
                CRMLibraryTable.PIMPage();
            }
            else if (sessionStorage.LibraryName == "Hot Buttons Settings") {
                CRMLibraryTable.HotButtons();
            }
            else if (sessionStorage.LibraryName == "Group Settings") {
                CRMLibraryTable.GroupSettings();
            }
            else if (sessionStorage.LibraryName == "Customer / Client List") {
                CRMLibraryTable.CustomerList();
            }
            else if (sessionStorage.LibraryName == "Supplier / Vendor List") {
                CRMLibraryTable.SupplierList();
            }
            else if (sessionStorage.LibraryName == "Sales Team") {
                CRMLibraryTable.SalesTeam();
            }
            else if (sessionStorage.LibraryName == "Activity Quota") {
                CRMLibraryTable.ActivityQuota();
            }
            else if (sessionStorage.LibraryName == "Client Staging") {
                CRMLibraryTable.ClientStage();
            }
            else if (sessionStorage.LibraryName == "Survey Creator") {
                CRMLibraryTable.SurveyList();
            }
            else if (sessionStorage.LibraryName == "Item Modifiers") {
                CRMLibraryTable.SearchItemModifiers('');
            }
            else if (sessionStorage.LibraryName == "Knowledge Management") {
                CRMLibraryTable.SearchKnowledgeManagementAndQuestionaire('');
            }
            else if (sessionStorage.LibraryName == "Ticket Notes") {
                CRMLibraryTable.TicketNotes();
            }
            else if (sessionStorage.LibraryName == "Funnel Templates") {
                CRMLibraryTable.FunnelTemplates();
            }

            IsLibraryDisplayInTable = true;

            if (sessionStorage.SubListName != undefined) {
                if (sessionStorage.SubListName == "FOWST Product List") {
                    $('#CRMdivAddLibrary').hide();
                    $('#CRMdivInfoForFowstProdList').show();
                }
                else {
                    $('#CRMdivAddLibrary').show();
                    $('#CRMdivInfoForFowstProdList').hide();
                }
            }
            else if (sessionStorage.SubListName == undefined) {
                $('#CRMdivAddLibrary').show();
                $('#CRMdivInfoForFowstProdList').hide();
            }

            if (sessionStorage.LibraryName == "Employees") {
                $('#CRMdivWorkSchedule').show();
            }
            else {
                $('#CRMdivWorkSchedule').hide();
            }

            if (sessionStorage.LibraryName == "PIM") {
                $('#CRMdivDownloadFilePIM').show();
                $('#CRMdivOptionAddProduct').show();
            }
            else {
                $('#CRMdivDownloadFilePIM').hide();
                $('#CRMdivOptionAddProduct').hide();
            }
        }

        function SearchLibrary(SearchText) {
            if (sessionStorage.LibraryName == "FOWST") {
                CRMLibraryTable.FowstSearch(SearchText);
            }
            else if (sessionStorage.LibraryName == "Authorized Devices") {
                CRMLibraryTable.DevicesSearch(SearchText);
            }
            else if (sessionStorage.LibraryName == "Receipt Settings") {
                CRMLibraryTable.ReceiptSettingsSearch(SearchText);
            }
            else if (sessionStorage.LibraryName == "Security Roles") {
                CRMLibraryTable.SecurityRolesSearch(SearchText);
            }
            else if (sessionStorage.LibraryName == "User") {
                CRMLibraryTable.EmployeesSearch(SearchText);
            }
            else if (sessionStorage.LibraryName == "Printer Roles") {
                CRMLibraryTable.PrinterRolesSearch(SearchText);
            }
            else if (sessionStorage.LibraryName == "PIM") {
                CRMLibraryTable.PIMSearch(SearchText);
            }
            else if (sessionStorage.LibraryName == "Hot Buttons Settings") {
                CRMLibraryTable.HotButtonsSearch(SearchText);
            }
            else if (sessionStorage.LibraryName == "Group Settings") {
                CRMLibraryTable.GroupSettingsSearch(SearchText);
            }
            else if (sessionStorage.LibraryName == "Customer / Client List") {
                CRMLibraryTable.CustomerListSearch(SearchText);
            }
            else if (sessionStorage.LibraryName == "Supplier / Vendor List") {
                CRMLibraryTable.SupplierListSearch(SearchText);
            }
            else if (sessionStorage.LibraryName == "Sales Team") {
                CRMLibraryTable.SalesTeamSearch(SearchText);
            }
            else if (sessionStorage.LibraryName == "Knowledge Management") {
                CRMLibraryTable.KnowledgeManagementAndQuestionaire(SearchText);
            }
        }

        function GoToThisPage(AddDevice) {

            //sessionStorage["AddLibraryName"] = AddDevice;
            $('#CRMUniversalDiv').html('<div style="width:100%; text-align:center;">Loading ...</div>');

            window.LibraryTable = CRMLibraryTable; //gamiton ni sa mga modules sa crm systemadmin

            if (sessionStorage.LibraryName == "Security Roles") {
                window.location.hash = 'UniversalPage';

                if (sessionStorage.AddViewLibBCName == "Add Security Role") {
                    CRMLibraryTable.AddSecurityRolePage();
                }
                else {
                    CRMLibraryTable.ViewSecurityRolePage();
                }

            }
            if (sessionStorage.LibraryName == "CRM Settings") {
                window.location.hash = 'UniversalPage';
                CRMLibraryTable.CRMSettingsPage();
            }
            else if (sessionStorage.LibraryName == "Sales Team") {
                window.location.hash = 'UniversalPage';
                if (sessionStorage.AddViewLibBCName == "Add Team") {
                    CRMLibraryTable.AddCRMPage();
                }
                else {
                    CRMLibraryTable.ViewCRMPage();
                }
            }
            else if (sessionStorage.LibraryName == "Knowledge Management") {
                $('#CRMpanAddLibrary').modal('hide');
                setTimeout(function () {
                    CRMLibraryTable.GetKnowledgeManagementNumber();
                    $('#CRMpanAddKnowledgeManagement').show();
                    $('#CRMtxtSolution, #CRMtxtSteps').height('50px');
                    //GetAllTopics();
                    //GetAllProducts();
                    //GetAllLanguages();
                }, 500);
            }
            
            IsAddLibraryDisplayInUnivDiv = true;
        }

        function GetOrganizationName() {
            AjaxSend({
                url: 'DashboardsAndReports/GetOrganizationName',
                success: function (vData) {
                    if (vData.Results.Rows.length > 0) {
                        sessionStorage["OrgName"] = vData.Results.Rows[0].CompanyName;
                        $('.OrganizationNameHeader').text(vData.Results.Rows[0].CompanyName);
                    }
                }
            });
        }

        function ShowCheckList() {
            var pData = {};
            pData.OrgID = sessionStorage["OrgID"];

            window.pageOrig = $.mobile.activePage;

            AjaxSend({
                url: 'SetupChecklist/getModuleList',
                data: pData,
                success: function (vData) {

                    if (vData.PIMChecklistData.Rows.length > 0) {

                        AjaxSend({
                            url: 'SetupChecklist/getModuleRequiredDataList',
                            data: pData,
                            success: function (vDataCheck) {

                                if (vData.PIMChecklistData.Rows.length > 0) {
                                    for (var p = 0; p <= vData.PIMChecklistData.Rows.length - 1; p++) {

                                        if (vData.PIMChecklistData.Rows[p].ModuleType == "Organization") {

                                            $("#CRMimgOrg").attr('src', '/Content/images/tick.png');
                                            for (var i = 0; i <= vDataCheck.PIMChecklistData.Rows.length - 1; i++) {
                                                if (vData.PIMChecklistData.Rows[p].ModuleType == vDataCheck.PIMChecklistData.Rows[i].ModuleType) {
                                                    $("#CRMimgOrg").attr('src', '/Content/images/cross.png');
                                                }
                                            }
                                            $("#CRMlblOrgLastUpdated").text(((vData.PIMChecklistData.Rows[p].LastUpdated || '') == '' ? '' : ($.format.date(new Date(vData.PIMChecklistData.Rows[p].LastUpdated), 'MMM dd, yyyy HH:mm') || '')));

                                        } else if (vData.PIMChecklistData.Rows[p].ModuleType == "FOWST") {
                                            $("#CRMimgFOWST").attr('src', '/Content/images/tick.png');
                                            for (var i = 0; i <= vDataCheck.PIMChecklistData.Rows.length - 1; i++) {
                                                if (vData.PIMChecklistData.Rows[p].ModuleType == vDataCheck.PIMChecklistData.Rows[i].ModuleType) {
                                                    $("#CRMimgFOWST").attr('src', '/Content/images/cross.png');
                                                }
                                            }
                                            $("#CRMlblFOWSTCount").text(AccountingFormat(vData.PIMChecklistData.Rows[p].Cnt, 0));
                                            $("#CRMlblFOWSTLastUpdated").text(((vData.PIMChecklistData.Rows[p].LastUpdated || '') == '' ? '' : ($.format.date(new Date(vData.PIMChecklistData.Rows[p].LastUpdated), 'MMM dd, yyyy HH:mm') || '')));

                                        } else if (vData.PIMChecklistData.Rows[p].ModuleType == "Authorized Device") {
                                            $("#CRMimgDevice").attr('src', '/Content/images/tick.png');
                                            for (var i = 0; i <= vDataCheck.PIMChecklistData.Rows.length - 1; i++) {
                                                if (vData.PIMChecklistData.Rows[p].ModuleType == vDataCheck.PIMChecklistData.Rows[i].ModuleType) {
                                                    $("#CRMimgDevice").attr('src', '/Content/images/cross.png');
                                                }
                                            }
                                            $("#CRMlblDeviceCount").text(AccountingFormat(vData.PIMChecklistData.Rows[p].Cnt, 0));
                                            $("#CRMlblDeviceLastUpdated").text(((vData.PIMChecklistData.Rows[p].LastUpdated || '') == '' ? '' : ($.format.date(new Date(vData.PIMChecklistData.Rows[p].LastUpdated), 'MMM dd, yyyy HH:mm') || '')));

                                        } else if (vData.PIMChecklistData.Rows[p].ModuleType == "Receipt Settings") {
                                            $("#CRMimgReceiptSettings").attr('src', '/Content/images/tick.png');
                                            for (var i = 0; i <= vDataCheck.PIMChecklistData.Rows.length - 1; i++) {
                                                if (vData.PIMChecklistData.Rows[p].ModuleType == vDataCheck.PIMChecklistData.Rows[i].ModuleType) {
                                                    $("#CRMimgReceiptSettings").attr('src', '/Content/images/cross.png');
                                                }
                                            }
                                            $("#CRMlblReceiptSettingsCount").text(AccountingFormat(vData.PIMChecklistData.Rows[p].Cnt, 0));
                                            $("#CRMlblReceiptSettingsLastUpdated").text(((vData.PIMChecklistData.Rows[p].LastUpdated || '') == '' ? '' : ($.format.date(new Date(vData.PIMChecklistData.Rows[p].LastUpdated), 'MMM dd, yyyy HH:mm') || '')));

                                        } else if (vData.PIMChecklistData.Rows[p].ModuleType == "Security Access") {
                                            $("#CRMimgSecurityAccess").attr('src', '/Content/images/tick.png');
                                            for (var i = 0; i <= vDataCheck.PIMChecklistData.Rows.length - 1; i++) {
                                                if (vData.PIMChecklistData.Rows[p].ModuleType == vDataCheck.PIMChecklistData.Rows[i].ModuleType) {
                                                    $("#CRMimgSecurityAccess").attr('src', '/Content/images/cross.png');
                                                }
                                            }
                                            $("#CRMlblSecurityAccessCount").text(AccountingFormat(vData.PIMChecklistData.Rows[p].Cnt, 0));
                                            $("#CRMlblSecurityAccessLastUpdated").text(((vData.PIMChecklistData.Rows[p].LastUpdated || '') == '' ? '' : ($.format.date(new Date(vData.PIMChecklistData.Rows[p].LastUpdated), 'MMM dd, yyyy HH:mm') || '')));

                                        } else if (vData.PIMChecklistData.Rows[p].ModuleType == "Employee") {
                                            $("#CRMimgEmployeeAccess").attr('src', '/Content/images/tick.png');
                                            for (var i = 0; i <= vDataCheck.PIMChecklistData.Rows.length - 1; i++) {
                                                if (vData.PIMChecklistData.Rows[p].ModuleType == vDataCheck.PIMChecklistData.Rows[i].ModuleType) {
                                                    $("#CRMimgEmployeeAccess").attr('src', '/Content/images/cross.png');
                                                }
                                            }
                                            $("#CRMlblEmployeeCount").text(AccountingFormat(vData.PIMChecklistData.Rows[p].Cnt, 0));
                                            $("#CRMlblEmployeeLastUpdated").text(((vData.PIMChecklistData.Rows[p].LastUpdated || '') == '' ? '' : ($.format.date(new Date(vData.PIMChecklistData.Rows[p].LastUpdated), 'MMM dd, yyyy HH:mm') || '')));

                                        } else if (vData.PIMChecklistData.Rows[p].ModuleType == "Products") {
                                            $("#CRMimgProduct").attr('src', '/Content/images/tick.png');
                                            for (var i = 0; i <= vDataCheck.PIMChecklistData.Rows.length - 1; i++) {
                                                if (vData.PIMChecklistData.Rows[p].ModuleType == vDataCheck.PIMChecklistData.Rows[i].ModuleType) {
                                                    $("#CRMimgProduct").attr('src', '/Content/images/cross.png');
                                                }
                                            }
                                            $("#CRMlblProductCount").text(AccountingFormat(vData.PIMChecklistData.Rows[p].Cnt, 0));
                                            $("#CRMlblProductLastUpdated").text(((vData.PIMChecklistData.Rows[p].LastUpdated || '') == '' ? '' : ($.format.date(new Date(vData.PIMChecklistData.Rows[p].LastUpdated), 'MMM dd, yyyy HH:mm') || '')));

                                        } else if (vData.PIMChecklistData.Rows[p].ModuleType == "Customers/Clients") {
                                            $("#CRMimgCustomer").attr('src', '/Content/images/tick.png');
                                            for (var i = 0; i <= vDataCheck.PIMChecklistData.Rows.length - 1; i++) {
                                                if (vData.PIMChecklistData.Rows[p].ModuleType == vDataCheck.PIMChecklistData.Rows[i].ModuleType) {
                                                    $("#CRMimgCustomer").attr('src', '/Content/images/cross.png');
                                                }
                                            }
                                            $("#CRMlblCustomerCount").text(AccountingFormat(vData.PIMChecklistData.Rows[p].Cnt, 0));
                                            $("#CRMlblCustomerLastUpdated").text(((vData.PIMChecklistData.Rows[p].LastUpdated || '') == '' ? '' : ($.format.date(new Date(vData.PIMChecklistData.Rows[p].LastUpdated), 'MMM dd, yyyy HH:mm') || '')));

                                        } else if (vData.PIMChecklistData.Rows[p].ModuleType == "Supplier/Vendor") {
                                            $("#CRMimgSupplier").attr('src', '/Content/images/tick.png');
                                            for (var i = 0; i <= vDataCheck.PIMChecklistData.Rows.length - 1; i++) {
                                                if (vData.PIMChecklistData.Rows[p].ModuleType == vDataCheck.PIMChecklistData.Rows[i].ModuleType) {
                                                    $("#CRMimgSupplier").attr('src', '/Content/images/cross.png');
                                                }
                                            }
                                            $("#CRMlblSupplierCount").text(AccountingFormat(vData.PIMChecklistData.Rows[p].Cnt, 0));
                                            $("#CRMlblSupplierLastUpdated").text(((vData.PIMChecklistData.Rows[p].LastUpdated || '') == '' ? '' : ($.format.date(new Date(vData.PIMChecklistData.Rows[p].LastUpdated), 'MMM dd, yyyy HH:mm') || '')));

                                        } else if (vData.PIMChecklistData.Rows[p].ModuleType == "Discount Settings") {

                                            $("#CRMlblDiscountSettingsLastUpdated").text(((vData.PIMChecklistData.Rows[p].LastUpdated || '') == '' ? '' : ($.format.date(new Date(vData.PIMChecklistData.Rows[p].LastUpdated), 'MMM dd, yyyy HH:mm') || '')));

                                        } else if (vData.PIMChecklistData.Rows[p].ModuleType == "Device Printer") {

                                            $("#CRMlblPrinterCount").text(AccountingFormat(vData.PIMChecklistData.Rows[p].Cnt, 0));
                                            $("#CRMlblPrinterLastUpdated").text(((vData.PIMChecklistData.Rows[p].LastUpdated || '') == '' ? '' : ($.format.date(new Date(vData.PIMChecklistData.Rows[p].LastUpdated), 'MMM dd, yyyy HH:mm') || '')));

                                        } else if (vData.PIMChecklistData.Rows[p].ModuleType == "Hot Button") {

                                            $("#CRMlblHotButtonCount").text(AccountingFormat(vData.PIMChecklistData.Rows[p].Cnt, 0));
                                            $("#CRMlblHotButtonLastUpdated").text(((vData.PIMChecklistData.Rows[p].LastUpdated || '') == '' ? '' : ($.format.date(new Date(vData.PIMChecklistData.Rows[p].LastUpdated), 'MMM dd, yyyy HH:mm') || '')));

                                        } else if (vData.PIMChecklistData.Rows[p].ModuleType == "Group Setting") {

                                            $("#CRMlblGroupSettingsCount").text(AccountingFormat(vData.PIMChecklistData.Rows[p].Cnt, 0));
                                            $("#CRMlblGroupSettingsLastUpdated").text(((vData.PIMChecklistData.Rows[p].LastUpdated || '') == '' ? '' : ($.format.date(new Date(vData.PIMChecklistData.Rows[p].LastUpdated), 'MMM dd, yyyy HH:mm') || '')));

                                        } else if (vData.PIMChecklistData.Rows[p].ModuleType == "Team") {

                                            $("#CRMlblSalesTeamCount").text(AccountingFormat(vData.PIMChecklistData.Rows[p].Cnt, 0));
                                            $("#CRMlblSalesTeamLastUpdated").text(((vData.PIMChecklistData.Rows[p].LastUpdated || '') == '' ? '' : ($.format.date(new Date(vData.PIMChecklistData.Rows[p].LastUpdated), 'MMM dd, yyyy HH:mm') || '')));

                                        } else if (vData.PIMChecklistData.Rows[p].ModuleType == "PurchaseOrder") {

                                            $("#CRMlblPurchaseOrderLastUpdated").text(((vData.PIMChecklistData.Rows[p].LastUpdated || '') == '' ? '' : ($.format.date(new Date(vData.PIMChecklistData.Rows[p].LastUpdated), 'MMM dd, yyyy HH:mm') || '')));

                                        }

                                    };
                                }
                            },
                            error: function (vError) {
                                MessageBox("This feature requires internet connection.",
                                           "Requires Internet Connection",
                                            [
                                                { Text: 'Ok'
                                                }
                                            ]
                                    );
                                return;
                            }

                        });
                    }

                },
                error: function (vError) {
                    MessageBox("This feature requires internet connection.",
                                   "Requires Internet Connection",
                                    [
                                        { Text: 'Ok'
                                        }
                                    ]
                            );
                    return;
                }

            });

        }

        function ShowSumOrgInfo() {
            AjaxSend({
                url: 'Login3/TestCookies',
                success: function (vData) {
                    window.pageSecuritySourcePage = $.mobile.activePage;
                    NewPromptUserAccess('Company Profile', {
                        OnClickOKSuccess: function () {
                            var pData = {};
                            pData.OrgID = sessionStorage["OrgID"];

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
                                                // ChangePage('/Organization/OrganizationSummary');
                                                window.location.hash = "UniversalPage";
                                                sessionStorage["LibraryName"] = 'Organization';
                                                GoToThisPage('Organization');
                                            }
                                        });
                                    }
                                },
                                error: function (vError) {
                                    MessageBox("This feature requires internet connection.",
                                               "Requires Internet Connection",
                                                [
                                                    { Text: 'Ok'
                                                    }
                                                ]
                                        );
                                    return;
                                }

                            });
                        }
                    });
                },
                error: function (vError) {
                    MessageBox("This feature requires internet connection.",
                                   "Requires Internet Connection",
                                    [
                                        { Text: 'Ok'
                                        }
                                    ]
                            );
                    return;
                }

            });
        }

        function ShowOrgPlaces(s) {
            AjaxSend({
                url: 'Login3/TestCookies',
                success: function (vData) {
                    window.pageSecuritySourcePage = $.mobile.activePage;
                    NewPromptUserAccess('Company Places (FOWST)', {
                        OnClickOKSuccess: function () {
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
                                                if (s == "FOWST") {
                                                    window.location.hash = "LibraryPage";
                                                    headerData = ['#', 'Name', 'Address', 'Last Modified By', 'Last Modified Date'];
                                                    datasort = ['int', 'string', 'string', 'string', 'date'];
                                                    sessionStorage["LibraryName"] = 'FOWST';
                                                    sessionStorage["LabelLibraryName"] = 'FOWST';
                                                    DisplayTableLibrary(headerData, datasort);
                                                }
                                                else if (s == "FOWSTProdList") {
                                                    window.location.hash = "UniversalPage";
                                                    sessionStorage["AddViewLibBCName"] = 'FOWST Product List';
                                                    CRMLibraryTable.FowstProductList();
                                                }
                                                else {
                                                    return;
                                                }
                                            }
                                        });
                                    }
                                },
                                error: function (vError) {
                                    MessageBox("This feature requires internet connection.",
                                           "Requires Internet Connection",
                                            [
                                                { Text: 'Ok'
                                                }
                                            ]
                                    );
                                    return;
                                }

                            });
                        }
                    });
                },
                error: function (vError) {
                    MessageBox("This feature requires internet connection.",
                                   "Requires Internet Connection",
                                    [
                                        { Text: 'Ok'
                                        }
                                    ]
                            );
                    return;
                }

            });
        }

        function CreateNewDevicePrinterEntry(IsNew, strID) {
            var pData = {};
            pData.IsNew = IsNew;
            pData.ID = strID;

            window.pageOrig = $.mobile.activePage;

            AjaxSend({
                url: 'DevicePrinter/setCreateNew',
                data: pData,
                success: function (vData) {

                    if ((vData.IsNew || '') == "TRUE") {
                        sessionStorage["AddViewLibBCName"] = "Add Printer Role";
                        window.location.hash = 'UniversalPage';
                        CRMLibraryTable.AddPrinterRolesPage();
                    }
                    else {
                        if (GetUrlParameters().mode == 'wizard') {
                            window.open(GetPathFormat("/DevicePrinter/DevicePrinterSummary?mode=wizard"), "_self");
                        } else
                            window.open(GetPathFormat("/DevicePrinter/DevicePrinterSummary"), "_self");
                    }

                }
            });
        }
    });
});