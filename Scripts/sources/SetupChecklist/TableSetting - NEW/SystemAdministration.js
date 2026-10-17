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
        var arrIndexSolution, arrIndexStep, arrAllowedTeam = [];

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

        window.GetKMData = {
            DisplaySolution: function (arr) {
                arrSolution = arr;
                DisplaySolution();
            }
        }

        $(document).ready(function () {
            $('#divContent').css('margin-top', $('.divHeader').height());

            var breadcrumbs = $('<div class="ItemsBreadCrumbs"></div>'),
                arrowR = ' <span class="ui-icon ui-icon-arrow-r" style="display:inline-block; margin:0px 10px 0px 10px;"></span>',
                lnkSysAdd = '<a data-role="none" id="btnSystemAdministration">System Administration</a>';

            var date = new Date();
            var month = ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"];
            //$('.lblPeriod').text(month[date.getMonth()] + ' ' + date.getDate() + ', ' + date.getFullYear());

            $(window).bind('hashchange', function () {
                if (window.location.hash == "#SystemAdministration" || window.location.hash == "") {
                    $('#divSystemAdDashboard').show();
                    $('#divSecond, #UniversalDiv, #divSubList, #divForPIM').hide();
                    $('#UniversalDiv').empty();
                    $('.ItemsBreadCrumbs').text('System Administration');
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
                }
                else if (window.location.hash == "#LibraryPage") {
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
                    $('#txtSearch').attr('placeholder', 'Type name of ' + sessionStorage.LabelLibraryName);

                    if (IsLibraryDisplayInTable == false) {
                        $('.ItemsBreadCrumbs').text('');

                        $('#divSystemAdDashboard, #UniversalDiv, #divSubList, #divForPIM').hide();
                        //$('#UniversalDiv').empty();
                        $('#divSecond').show();

                        if (sessionStorage.LibraryName == 'FOWST') {
                            breadcrumbs.prepend(arrowR, sessionStorage.LibraryName);
                            headerData = ['#', 'Name', 'Address', 'Last Modified By', 'Last Modified Date'];
                            datasort = ['int', 'string', 'string', 'string', 'date'];
                            DisplayTableLibrary(headerData, datasort);
                        }

                        else if (sessionStorage.LibraryName == 'Authorized Devices') {
                            breadcrumbs.prepend(arrowR, sessionStorage.LibraryName);
                            headerData = ['#', 'Phone Name', 'Phone Model', 'IMEI', 'UUID', 'Status', 'Last Modified By', 'Last Modified Date'];
                            datasort = ['int', 'string', 'string', 'string', 'string', 'string', 'string', 'date'];
                            DisplayTableLibrary(headerData, datasort);
                        }

                        else if (sessionStorage.LibraryName == 'Receipt Settings') {
                            breadcrumbs.prepend(arrowR, sessionStorage.LibraryName);
                            $('.lblAddLibraryName, .lblPopUpHeaderLibraryName').text(sessionStorage.LabelLibraryName);
                            $('#txtSearch').attr('placeholder', 'Type name of ' + sessionStorage.LabelLibraryName);
                            headerData = ['#', 'Name', 'Trade Name', 'Paper Size', 'Last Modified By', 'Last Modified Date'];
                            datasort = ['int', 'string', 'string', 'string', 'string', 'date'];
                            DisplayTableLibrary(headerData, datasort);
                        }

                        else if (sessionStorage.LibraryName == 'Security Roles') {
                            breadcrumbs.prepend(arrowR, sessionStorage.LibraryName);
                            headerData = ['#', 'Code Name', 'Role', 'Description', 'Last Modified By', 'Last Modified Date'];
                            datasort = ['int', 'string', 'string', 'string', 'string', 'date'];
                            DisplayTableLibrary(headerData, datasort);
                        }

                        else if (sessionStorage.LibraryName == 'Employees') {
                            breadcrumbs.prepend(arrowR, sessionStorage.LibraryName);
                            headerData = ['#', 'Name', 'Date Hired', 'Status', 'Banned'];
                            datasort = ['int', 'string', 'date', 'string', 'string'];
                            DisplayTableLibrary(headerData, datasort);
                        }

                        else if (sessionStorage.LibraryName == 'PIM') {
                            breadcrumbs.prepend(arrowR, sessionStorage.LibraryName);
                            headerData = ['#', 'Short Description', 'Product No.', 'Retail Price', 'Brand', 'Activation Date'];
                            datasort = ['int', 'string', 'string', 'int', 'string', 'date'];
                            DisplayTableLibrary(headerData, datasort);
                        }

                        else if (sessionStorage.LibraryName == 'Printer Roles') {
                            breadcrumbs.prepend(arrowR, sessionStorage.LibraryName);
                            headerData = ['#', 'Printer Role Name', 'Printer Type', 'Active', 'Last Modified By', 'Last Modified Date'];
                            datasort = ['int', 'string', 'string', 'string', 'string', 'date'];
                            DisplayTableLibrary(headerData, datasort);
                        }

                        else if (sessionStorage.LibraryName == 'Hot Buttons Settings') {
                            breadcrumbs.prepend(arrowR, sessionStorage.LibraryName);
                            headerData = ['#', 'Name', 'Created By', 'Last Updated'];
                            datasort = ['int', 'string', 'string', 'date'];
                            DisplayTableLibrary(headerData, datasort);
                        }

                        else if (sessionStorage.LibraryName == 'Group Settings') {
                            breadcrumbs.prepend(arrowR, sessionStorage.LibraryName);
                            headerData = ['#', 'Name', 'Description', 'Last Modified By', 'Last Modified Date'];
                            datasort = ['int', 'string', 'string', 'string', 'date'];
                            DisplayTableLibrary(headerData, datasort);
                        }

                        else if (sessionStorage.LibraryName == 'Customer / Client List') {
                            breadcrumbs.prepend(arrowR, sessionStorage.LibraryName);
                            headerData = ['#', 'Name', 'Activation Date', 'Status', 'Banned'];
                            datasort = ['int', 'string', 'date', 'string', 'string'];
                            DisplayTableLibrary(headerData, datasort);
                        }

                        else if (sessionStorage.LibraryName == 'Supplier / Vendor List') {
                            breadcrumbs.prepend(arrowR, sessionStorage.LibraryName);
                            headerData = ['#', 'Name', 'Activation Date', 'Status', 'Banned', 'Last Modified By'];
                            datasort = ['int', 'string', 'date', 'string', 'string', 'string'];
                            DisplayTableLibrary(headerData, datasort);
                        }

                        else if (sessionStorage.LibraryName == 'Sales Team') {
                            breadcrumbs.prepend(arrowR, sessionStorage.LibraryName);
                            headerData = ['#', 'Name', 'Description', 'Last Modified Date'];
                            datasort = ['int', 'string', 'string', 'date'];
                            DisplayTableLibrary(headerData, datasort);
                        }

                        else if (sessionStorage.LibraryName == 'Item Modifiers') {
                            breadcrumbs.prepend(arrowR, sessionStorage.LibraryName);
                            headerData = ['#', 'Item Modifier Name', 'Product Categories Applied To', 'Modified By', 'Modified Date'];
                            datasort = ['int', 'string', 'string', 'string', 'date'];
                            DisplayTableLibrary(headerData, datasort);
                        }

                        else if (sessionStorage.LibraryName == 'Printers') {
                            breadcrumbs.prepend(arrowR, sessionStorage.LibraryName);
                            headerData = ['#', 'Printer', 'Description', 'Last Updated'];
                            datasort = ['int', 'string', 'string', 'date'];
                            DisplayTableLibrary(headerData, datasort);
                        }

                        else if (sessionStorage.LibraryName == 'Table View Settings') {
                            breadcrumbs.prepend(arrowR, sessionStorage.LibraryName);
                            headerData = ['#', 'Table View', 'Locations', 'Modified By', 'Last Modified'];
                            datasort = ['int', 'string', , 'string', 'string', 'date'];
                            DisplayTableLibrary(headerData, datasort);
                        }

                        ResizeHeightOfRequiredData();
                    }
                    else {
                        $('.ItemsBreadCrumbs').text('');
                        breadcrumbs.prepend(arrowR, sessionStorage.LibraryName);
                        $('#divSystemAdDashboard, #UniversalDiv, #divSubList, #divForPIM').hide();
                        $('#UniversalDiv').empty();
                        $('#divSecond').show();
                    }

                    $('.BreadCrumbs').html(breadcrumbs.prepend(lnkSysAdd));
                    BreadCrumbsClickEvent();
                    $('body').animate({ scrollTop: 0 });
                }

                else if (window.location.hash == "#UniversalPage") {
                    $('.ItemsBreadCrumbs').text('');

                    if (sessionStorage.AddViewLibBCName != undefined && sessionStorage.SubListName == undefined) {
                        breadcrumbs.prepend(arrowR, sessionStorage.AddViewLibBCName);
                        breadcrumbs.prepend(arrowR, '<a data-role="none" id="btnLibraryPage">' + sessionStorage.LibraryName + '</a>');
                    }
                    else if (sessionStorage.SubListName != undefined && sessionStorage.SecondSubListName == undefined && sessionStorage.AddViewLibBCName == undefined) {
                        if (sessionStorage.SubListName != "Upload / Download Product List") {
                            breadcrumbs.prepend(arrowR, sessionStorage.SubListName);
                            breadcrumbs.prepend(arrowR, '<a data-role="none" id="btnSublistPage">' + sessionStorage.LibraryName + '</a>');
                        }
                    }
                    else if (sessionStorage.SubListName == undefined && sessionStorage.SecondSubListName == undefined && sessionStorage.AddViewLibBCName == undefined) {
                        breadcrumbs.prepend(arrowR, sessionStorage.LibraryName);
                    }

                    $('#divSystemAdDashboard, #divSecond, #divSubList, #divForPIM').hide();
                    $('#UniversalDiv').show();

                    if (IsAddLibraryDisplayInUnivDiv == false) {
                        GoToThisPage();
                    }

                    if (sessionStorage.LibraryName == "PIM" && sessionStorage.SubListName != undefined) {
                        $('.ItemsBreadCrumbs').text('');

                        if (sessionStorage.SubListName == "Upload / Download Product List") {
                            breadcrumbs.prepend(arrowR, sessionStorage.SubListName);
                        }

                        else if (sessionStorage.SubListName == "Company Wide Product List") {
                            breadcrumbs.prepend(arrowR, sessionStorage.AddViewLibBCName);
                            breadcrumbs.prepend(arrowR, '<a data-role="none" id="btnSublistParentPage">' + sessionStorage.SubListName + '</a>');
                        }

                        else if (sessionStorage.SubListName == "FOWST Product List") {
                            $('.ItemsBreadCrumbs').text('');
                            breadcrumbs.prepend(arrowR, sessionStorage.AddViewLibBCName);
                            breadcrumbs.prepend(arrowR, '<a data-role="none" id="btnSublistParentPage">' + sessionStorage.SubListName + '</a>');

                        }

                        else if (sessionStorage.SubListName == "Adjust Product Prices") {
                            $('.ItemsBreadCrumbs').text('');
                            breadcrumbs.prepend(arrowR, sessionStorage.SecondSubListName);
                            breadcrumbs.prepend(arrowR, '<a data-role="none" id="btnSublistParentPage">' + sessionStorage.SubListName + '</a>');
                        }

                        breadcrumbs.prepend(arrowR, '<a data-role="none" id="btnSublistPage">' + sessionStorage.LibraryName + '</a>');
                    }

                    $('.BreadCrumbs').html(breadcrumbs.prepend(lnkSysAdd));
                    BreadCrumbsClickEvent();
                    $('body').animate({ scrollTop: 0 });
                }

                else if (window.location.hash == "#SubListPage") {
                    $('.ItemsBreadCrumbs').text('');

                    if (sessionStorage.SubListName != undefined) {
                        breadcrumbs.prepend(arrowR, sessionStorage.SubListName);
                        breadcrumbs.prepend(arrowR, '<a data-role="none" id="btnSublistPage">' + sessionStorage.LibraryName + '</a>');
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
                $('#divContent').css('margin-top', $('.divHeader').height());
            });

            GetOrganizationName();
            ResizeHeightOfRequiredData();
            ShowCheckList();

            $('#btnPrintPOCSlip').click(function () {
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
                window.location = '/People/PeopleMenu';
                sessionStorage.removeItem("OrgName");
            })

            $('.btnMoreOption').click(function () {
                $('#panOption').popup('open');
            })

            $('#btnSetupOrganization, #popupbtnOrganization').click(function () {
                $('#panOption').popup('close');
                window.location.hash = '';
                ShowSumOrgInfo();
            })

            $('#btnSetupOrganizationFOWST, #popupbtnFowst').click(function () {
                $('#panOption').popup('close');
                window.location.hash = '';
                ShowOrgPlaces('FOWST');
            })

            $('#btnSetupDevice, #popupbtnAuthDevices').click(function () {
                AjaxSend({
                    url: 'Login3/TestCookies',
                    success: function (vData) {
                        NewPromptUserAccess('Devices', {
                            OnClickOKSuccess: function () {
                                $('#panOption').popup('close');
                                window.location.hash = '';
                                window.location.hash = "LibraryPage";
                                headerData = ['#', 'Phone Name', 'Phone Model', 'IMEI', 'UUID', 'Status', 'Last Modified By', 'Last Modified Date'];
                                datasort = ['int', 'string', 'string', 'string', 'string', 'string', 'string', 'date'];
                                sessionStorage["LibraryName"] = 'Authorized Devices';
                                sessionStorage["LabelLibraryName"] = 'Authorized Device';
                                sessionStorage["AddViewLibBCName"] = "Add Device";
                                DisplayTableLibrary(headerData, datasort);
                            }
                        })
                    },
                    error: function (vError) {
                        $('#TitleOrgEmp').text('Requires Internet Connection');
                        $('#MsgOrgEmp').text('This feature requires internet connection.');
                        $('#panOrgEmpMessage').popup('open');
                        $('#btnOKOrgEmp').text('Ok').click(function (e) {
                            e.preventDefault();
                            $('#panOrgEmpMessage').popup('close');
                        })
                        $('#divBtnCancel').hide();
                        return;
                    }
                });
            })

            $('#btnSetupReceiptSettings, #popupbtnReceiptSettings').click(function () {
                AjaxSend({
                    url: 'Login3/TestCookies',
                    success: function (vData) {
                        NewPromptUserAccess('POS Receipt Settings', {
                            OnClickOKSuccess: function () {
                                $('#panOption').popup('close');
                                window.location.hash = '';
                                window.location.hash = "LibraryPage";
                                headerData = ['#', 'Name', 'Trade Name', 'Paper Size', 'Last Modified By', 'Last Modified Date'];
                                datasort = ['int', 'string', 'string', 'string', 'string', 'date'];
                                sessionStorage["LibraryName"] = 'Receipt Settings';
                                sessionStorage["LabelLibraryName"] = 'Receipt Template';
                                sessionStorage["AddViewLibBCName"] = "Add Receipt Template";
                                DisplayTableLibrary(headerData, datasort);
                            }
                        })
                    },
                    error: function (vError) {
                        $('#TitleOrgEmp').text('Requires Internet Connection');
                        $('#MsgOrgEmp').text('This feature requires internet connection.');
                        $('#panOrgEmpMessage').popup('open');
                        $('#btnOKOrgEmp').text('Ok').click(function (e) {
                            e.preventDefault();
                            $('#panOrgEmpMessage').popup('close');
                        })
                        $('#divBtnCancel').hide();
                        return;
                    }
                });
            })

            $('#btnSetupSecurity, #popupbtnSecurityRoles').click(function () {
                AjaxSend({
                    url: 'Login3/TestCookies',
                    success: function (vData) {
                        NewPromptUserAccess('Security Roles', {
                            OnClickOKSuccess: function () {
                                $('#panOption').popup('close');
                                window.location.hash = "LibraryPage";
                                headerData = ['#', 'Code Name', 'Role', 'Description', 'Last Modified By', 'Last Modified Date'];
                                datasort = ['int', 'string', 'string', 'string', 'string', 'date'];
                                sessionStorage["LibraryName"] = 'Security Roles';
                                sessionStorage["LabelLibraryName"] = 'Security Role';
                                sessionStorage["AddViewLibBCName"] = "Add Security Role";
                                DisplayTableLibrary(headerData, datasort);
                            }
                        })
                    },
                    error: function (vError) {
                        $('#TitleOrgEmp').text('Requires Internet Connection');
                        $('#MsgOrgEmp').text('This feature requires internet connection.');
                        $('#panOrgEmpMessage').popup('open');
                        $('#btnOKOrgEmp').text('Ok').click(function (e) {
                            e.preventDefault();
                            $('#panOrgEmpMessage').popup('close');
                        })
                        $('#divBtnCancel').hide();
                        return;
                    }
                });
            })

            $('#btnSetupEmployee, #popupbtnEmployees').click(function () {
                AjaxSend({
                    url: 'Login3/TestCookies',
                    success: function (vData) {
                        NewPromptUserAccess('Company Employee', {
                            OnClickOKSuccess: function () {
                                window.location.hash = '';
                                window.location.hash = "LibraryPage";
                                headerData = ['#', 'Name', 'Date Hired', 'Status', 'Banned'];
                                datasort = ['int', 'string', 'date', 'string', 'string'];
                                sessionStorage["LibraryName"] = 'Employees';
                                sessionStorage["LabelLibraryName"] = 'Employee';
                                DisplayTableLibrary(headerData, datasort);
                            }
                        })
                    },
                    error: function (vError) {
                        $('#TitleOrgEmp').text('Requires Internet Connection');
                        $('#MsgOrgEmp').text('This feature requires internet connection.');
                        $('#panOrgEmpMessage').popup('open');
                        $('#btnOKOrgEmp').text('Ok').click(function (e) {
                            e.preventDefault();
                            $('#panOrgEmpMessage').popup('close');
                        })
                        $('#divBtnCancel').hide();
                        return;
                    }
                });
            })

            $('#btnSetupProducts2').click(function () {
                var ismob = CBool(IsMobile) ? 1 : 0;
                window.open('/People/PeopleMenu#pagePopUpMenuProducts', '_blank');
                $('#panOption').popup('close');
            })

            $('#btnSetupProducts, #popupbtnProducts').click(function () {
                $('#panOption').popup('close');
                window.location.hash = '';
                window.location.hash = "LibraryPage";
                headerData = ['#', 'Short Description', 'Product No.', 'Retail Price', 'Brand', 'Activation Date'];
                datasort = ['int', 'string', 'string', 'int', 'string', 'date'];
                sessionStorage["LibraryName"] = 'PIM';
                sessionStorage["LabelLibraryName"] = 'Product';
                sessionStorage["AddViewLibBCName"] = "Add Product";
                DisplayTableLibrary(headerData, datasort);
            })

            $('#btnPrinter, #popupbtnPrinter').click(function () {
                $('#panOption').popup('close');
                window.location.hash = '';
                window.location.hash = "LibraryPage";
                headerData = ['#', 'Printer Brand', 'Printer Model', 'Printer Name', 'Serial Number', 'Interface Type', 'IP Address', 'FOWST', 'List of devices that use this as default printer', 'Role', 'Categories'];
                datasort = ['int', 'string', 'string', 'date'];
                sessionStorage["LibraryName"] = 'Printers';
                sessionStorage["LabelLibraryName"] = 'Printer';
                sessionStorage["AddViewLibBCName"] = "Add Printer";
                DisplayTableLibrary(headerData, datasort);
            })

            $('#btnSalesReturnSettings, #popupbtnSalesReturn').click(function () {
                AjaxSend({
                    url: 'Login3/TestCookies',
                    success: function (vData) {
                        window.pageSecuritySourcePage = $.mobile.activePage;
                        NewPromptUserAccess('Sales Return Settings', {
                            OnClickOKSuccess: function () {
                                $('#panOption').popup('close');
                                window.location.hash = '';
                                sessionStorage["LibraryName"] = 'Sales Returns Settings';
                                GoToThisPage('Sales Returns Settings');
                            }
                        });
                    },
                    error: function (vError) {
                        $('#TitleOrgEmp').text('Requires Internet Connection');
                        $('#MsgOrgEmp').text('This feature requires internet connection.');
                        $('#panOrgEmpMessage').popup('open');
                        $('#btnOKOrgEmp').text('Ok').click(function (e) {
                            e.preventDefault();
                            $('#panOrgEmpMessage').popup('close');
                        })
                        $('#divBtnCancel').hide();
                        return;
                    }
                });
            })

            $('#btnDiscountSettings, #popupbtnDiscountSettings').click(function () {
                AjaxSend({
                    url: 'Login3/TestCookies',
                    success: function (vData) {
                        window.pageSecuritySourcePage = $.mobile.activePage;
                        NewPromptUserAccess('Discount Settings', {
                            OnClickOKSuccess: function () {
                                $('.ui-dialog').dialog('close');
                                $('#panOption').popup('close');
                                window.location.hash = "UniversalPage";
                                sessionStorage["LibraryName"] = 'Discount Settings';
                                GoToThisPage('Discount Settings');
                            }
                        });
                    },
                    error: function (vError) {
                        $('#TitleOrgEmp').text('Requires Internet Connection');
                        $('#MsgOrgEmp').text('This feature requires internet connection.');
                        $('#panOrgEmpMessage').popup('open');
                        $('#btnOKOrgEmp').text('Ok').click(function (e) {
                            e.preventDefault();
                            $('#panOrgEmpMessage').popup('close');
                        })
                        $('#divBtnCancel').hide();
                        return;
                    }

                });
            })

            $('#btnPrinterRoles, #popupbtnPrinterRoles').click(function (e) {
                AjaxSend({
                    url: 'Login3/TestCookies',
                    success: function (vData) {
                        window.pageSecuritySourcePage = $.mobile.activePage;
                        NewPromptUserAccess('Printer Roles', {
                            OnClickOKSuccess: function () {
                                $('.ui-dialog').dialog('close');
                                $('#panOption').popup('close');
                                window.location.hash = "LibraryPage";
                                headerData = ['#', 'Printer Role Name', 'Printer Type', 'Active', 'Last Modified By', 'Last Modified Date'];
                                datasort = ['int', 'string', 'string', 'string', 'string', 'date'];
                                sessionStorage["LibraryName"] = 'Printer Roles';
                                sessionStorage["LabelLibraryName"] = 'Printer Role';
                                DisplayTableLibrary(headerData, datasort);
                            }
                        });
                    },
                    error: function (vError) {
                        $('#TitleOrgEmp').text('Requires Internet Connection');
                        $('#MsgOrgEmp').text('This feature requires internet connection.');
                        $('#panOrgEmpMessage').popup('open');
                        $('#btnOKOrgEmp').text('Ok').click(function (e) {
                            e.preventDefault();
                            $('#panOrgEmpMessage').popup('close');
                        })
                        $('#divBtnCancel').hide();
                        return;
                    }

                });
            })

            $('#btnPrinterSetup').click(function () {
                window.location.hash = "UniversalPage";
                sessionStorage.LibraryName = "Device Printer Setup";
                GoToThisPage();
            })

            $('#btnPIM, #popupbtnPIM').click(function () {
                //window.location.hash = "LibraryPage";
                //sessionStorage["LibraryName"] = "PIM";
                var ismob = CBool(IsMobile) ? 1 : 0;
                window.open('/People/PeopleMenu#pagePopUpMenuProducts', '_blank');
                $('#panOption').popup('close');
            })

            $('#lnkProductList').click(function (e) {
                e.preventDefault();
                window.location = GetPathFormat("/Organization/DownloadCompanyWideProductList?OrgID=" + sessionStorage["OrgID"]);
            })

            $('#btnUploadDownloadProdList').click(function () {
                window.location.hash = "UniversalPage";
                sessionStorage.SubListName = "Upload / Download Product List";
                sessionStorage["PIMUsedBy"] = 'Upload';
                GoToThisPage();
            })

            $('#btnComWideProdList').click(function () {
                window.location.hash = "SubListPage";
                sessionStorage["SubListName"] = 'Company Wide Product List';
                sessionStorage["LabelLibraryName"] = 'Product';
            })

            $('#btnBarcodeSearch').click(function () {
                sessionStorage["AddViewLibBCName"] = "Scan Barcode / Search Product";
                window.location.hash = 'UniversalPage';
                LibraryTable.CompanyWideProductListPage();
            })

            $('#btnSubscribe').click(function () {
                sessionStorage["AddViewLibBCName"] = "Subscribe to Company Product List";
                window.location.hash = 'UniversalPage';
                LibraryTable.CompanyWideProductListPage();
            })

            $('#btnFowstProdList').click(function () {
                ShowOrgPlaces('FOWSTProdList');
            })

            $('#btnAdjustProdPrices').click(function () {
                $('#divAddProductPopup').popup('close');
                $('#divAdjustProdPricesPopup').popup('open');
            })

            $('#btnBackToOption').click(function () {
                $('#divAdjustProdPricesPopup').popup('close');
                $('#divAddProductPopup').popup('open');
            })

            $('#btnSinglePrice').click(function () {
                AjaxSend({
                    url: 'Login3/TestCookies',
                    success: function (vData) {
                        window.pageSecuritySourcePage = $.mobile.activePage;
                        NewPromptUserAccess('Product Price', {
                            OnClickOKSuccess: function () {
                                window.location.hash = "UniversalPage";
                                sessionStorage["AddViewLibBCName"] = 'Single Product Price Adjustment';
                                LibraryTable.SingleProductPriceAdjustmentPage();
                            }
                        });
                    },
                    error: function (vError) {
                        $('#TitleOrgEmp').text('Requires Internet Connection');
                        $('#MsgOrgEmp').text('This feature requires internet connection.');
                        $('#panOrgEmpMessage').popup('open');
                        $('#btnOKOrgEmp').text('Ok').click(function (e) {
                            e.preventDefault();
                            $('#panOrgEmpMessage').popup('close');
                        })
                        $('#divBtnCancel').hide();
                        return;
                    }
                });
            })

            $('#btnBulkPrice').click(function () {
                window.location.hash = "UniversalPage";
                sessionStorage["AddViewLibBCName"] = 'Bulk Price Adjustment';
                LibraryTable.BulkPriceAdjustmentPage();
            })

            $('#btnImportByExcel').click(function () {
                window.location.hash = "UniversalPage";
                sessionStorage["AddViewLibBCName"] = 'Import Products by Excel File';
                LibraryTable.ImportByExcelFilePage();
            })

            $('#btnPIMWebForm').click(function () {
                window.location.hash = "UniversalPage";
                sessionStorage["AddViewLibBCName"] = 'PIM Web Form';
                LibraryTable.PIMWebForm();
            })

            $('#btnQuantitySetting, #popupbtnQuantitySettings').click(function () {
                $('#panOption').popup('close');
                window.location.hash = '';
                sessionStorage["LibraryName"] = 'Decimals in Quantity Settings';
                GoToThisPage();
            })

            $('#btnPOSSetting, #popupbtnPOSSettings').click(function () {
                $('#panOption').popup('close');
                window.location.hash = '';
                sessionStorage["LibraryName"] = 'POS Settings';
                GoToThisPage();
            })

            $('#btnSalesOrderSetting, #popupbtnSalesOrder').click(function () {
                $('#panOption').popup('close');
                window.location.hash = '';
                sessionStorage["LibraryName"] = 'Sales Order Settings';
                GoToThisPage();
            })

            $('#btnReceivingAndTransferOutSetting, #popupbtnReceivingAndTransferOut').click(function () {
                $('#panOption').popup('close');
                window.location.hash = '';
                sessionStorage["LibraryName"] = 'Receiving And Transfer-Out Settings';
                GoToThisPage();
            })

            $('#btnInventoryCountSetting, #popupbtnInventoryCount').click(function () {
                $('#panOption').popup('close');
                window.location.hash = '';
                sessionStorage["LibraryName"] = 'Inventory Count Settings';
                GoToThisPage();
            })

            $('#btnDailyBeginningBalanceSetting, #popupbtnDailyBeginningBalance').click(function () {
                $('#panOption').popup('close');
                window.location.hash = '';
                sessionStorage["LibraryName"] = 'Daily Beginning Balance Settings';
                GoToThisPage();
            })

            $('#butPurchaseSettings').click(function () {
                $('#panOption').popup('close');
                window.location.hash = '';
                sessionStorage["LibraryName"] = 'Purchase Order Settings';
                GoToThisPage();
            });

            $('#btnPurchaseOrderSetting, #popupbtnPurchaseOrderSettings').click(function () {
                $('#panOption').popup('close');
                window.location.hash = '';
                sessionStorage["LibraryName"] = 'Purchase Order Settings';
                GoToThisPage();
            })

            $('#popupbtnLoyaltyProgram').click(function () {
                $('#panOption').popup('close');
                window.location.hash = '';
                sessionStorage["LibraryName"] = 'Loyalty Program Settings';
                GoToThisPage();
            })

            $('#btnHotButtons, #popupbtnHotButton').click(function (e) {
                e.preventDefault();

                AjaxSend({
                    url: 'Login3/TestCookies',
                    success: function (vData) {
                        window.pageSecuritySourcePage = $.mobile.activePage;
                        NewPromptUserAccess('Hot Buttons Settings', {
                            OnClickOKSuccess: function () {
                                $('.ui-dialog').dialog('close');
                                $('#panOption').popup('close');
                                window.location.hash = "LibraryPage";
                                headerData = ['#', 'Name', 'Last Modified By', 'Last Modified Date'];
                                datasort = ['int', 'string', 'string', 'date'];
                                sessionStorage["LibraryName"] = 'Hot Buttons Settings';
                                sessionStorage["LabelLibraryName"] = 'Hot Button';
                                DisplayTableLibrary(headerData, datasort);
                            }
                        });
                    },
                    error: function (vError) {
                        $('#TitleOrgEmp').text('Requires Internet Connection');
                        $('#MsgOrgEmp').text('This feature requires internet connection.');
                        $('#panOrgEmpMessage').popup('open');
                        $('#btnOKOrgEmp').text('Ok').click(function (e) {
                            e.preventDefault();
                            $('#panOrgEmpMessage').popup('close');
                        })
                        $('#divBtnCancel').hide();
                        return;
                    }
                });
            })

            $('#btnGroupSettings, #popupbtnGroupSettings').click(function () {
                AjaxSend({
                    url: 'Login3/TestCookies',
                    success: function (vData) {
                        window.pageSecuritySourcePage = $.mobile.activePage;
                        NewPromptUserAccess('Group Settings', {
                            OnClickOKSuccess: function () {
                                $('#panOption').popup('close');
                                window.location.hash = '';
                                window.location.hash = "LibraryPage";
                                headerData = ['#', 'Name', 'Description', 'Last Modified By', 'Last Modified Date'];
                                datasort = ['int', 'string', 'string', 'string', 'date'];
                                sessionStorage["LibraryName"] = 'Group Settings';
                                sessionStorage["LabelLibraryName"] = 'Group';
                                DisplayTableLibrary(headerData, datasort);
                            }
                        });
                    },
                    error: function (vError) {
                        $('#TitleOrgEmp').text('Requires Internet Connection');
                        $('#MsgOrgEmp').text('This feature requires internet connection.');
                        $('#panOrgEmpMessage').popup('open');
                        $('#btnOKOrgEmp').text('Ok').click(function (e) {
                            e.preventDefault();
                            $('#panOrgEmpMessage').popup('close');
                        })
                        $('#divBtnCancel').hide();
                        return;
                    }
                });
            })

            $('#btnSetupCustomer, #popupbtnCustomer').click(function () {
                AjaxSend({
                    url: 'Login3/TestCookies',
                    success: function (vData) {
                        NewPromptUserAccess('Company Customer', {
                            OnClickOKSuccess: function () {
                                $('#panOption').popup('close');
                                window.location.hash = '';
                                window.location.hash = "LibraryPage";
                                headerData = ['#', 'Name', 'Activation Date', 'Status', 'Banned'];
                                datasort = ['int', 'string', 'date', 'string', 'string'];
                                sessionStorage["LibraryName"] = 'Customer / Client List';
                                sessionStorage["LabelLibraryName"] = 'Customer';
                                DisplayTableLibrary(headerData, datasort);
                            }
                        })
                    },
                    error: function (vError) {
                        $('#TitleOrgEmp').text('Requires Internet Connection');
                        $('#MsgOrgEmp').text('This feature requires internet connection.');
                        $('#panOrgEmpMessage').popup('open');
                        $('#btnOKOrgEmp').text('Ok').click(function (e) {
                            e.preventDefault();
                            $('#panOrgEmpMessage').popup('close');
                        })
                        $('#divBtnCancel').hide();
                        return;
                    }
                });
            })

            $('#btnSetupSupplier, #popupbtnSupplier').click(function () {
                AjaxSend({
                    url: 'Login3/TestCookies',
                    success: function (vData) {
                        NewPromptUserAccess('Company Supplier', {
                            OnClickOKSuccess: function () {
                                $('#panOption').popup('close');
                                window.location.hash = '';
                                window.location.hash = "LibraryPage";
                                headerData = ['#', 'Name', 'Activation Date', 'Status', 'Banned', 'Last Modified By'];
                                datasort = ['int', 'string', 'date', 'string', 'string', 'string'];
                                sessionStorage["LibraryName"] = 'Supplier / Vendor List';
                                sessionStorage["LabelLibraryName"] = 'Supplier';
                                DisplayTableLibrary(headerData, datasort);
                            }
                        })
                    },
                    error: function (vError) {
                        $('#TitleOrgEmp').text('Requires Internet Connection');
                        $('#MsgOrgEmp').text('This feature requires internet connection.');
                        $('#panOrgEmpMessage').popup('open');
                        $('#btnOKOrgEmp').text('Ok').click(function (e) {
                            e.preventDefault();
                            $('#panOrgEmpMessage').popup('close');
                        })
                        $('#divBtnCancel').hide();
                        return;
                    }
                });
            })

            $('#btnSalesTeam, #popupbtnSalesTeam').click(function () {
                $('#panOption').popup('close');
                window.location.hash = '';
                window.location.hash = "LibraryPage";
                headerData = ['#', 'Name', 'Description', 'Last Modified Date'];
                datasort = ['int', 'string', 'string', 'date'];
                sessionStorage["LibraryName"] = 'Sales Team';
                sessionStorage["LabelLibraryName"] = 'Team';
                DisplayTableLibrary(headerData, datasort);
            })

            $('#btnItemModifiers, #popupbtnItemModifiers').click(function () {
                $('#panOption').popup('close');
                window.location.hash = '';
                window.location.hash = "LibraryPage";
                headerData = ['#', 'Item Modifier Name', 'Product Categories Applied To', 'Modified By', 'Modified Date'];
                datasort = ['int', 'string', 'string', 'string', 'date'];
                sessionStorage["LibraryName"] = 'Item Modifiers';
                sessionStorage["LabelLibraryName"] = 'Modifier';
                DisplayTableLibrary(headerData, datasort);
                $('#tblBody').empty();
            })

            $('#popupbtnKnowledgeManagementAndQuestionaire, #popupbtnKnowledgeManagementAndQuestionaire').click(function () {
                $('#panOption').popup('close');
                window.location.hash = '';
                window.location.hash = "LibraryPage";
                headerData = ['Problem', 'Solution', 'Topic', 'Product', 'Language', 'Edited By', 'Last Edit Date'];
                datasort = ['string', 'string', 'string', 'string', 'string', 'string', 'date'];
                sessionStorage["LibraryName"] = 'Knowledge Management';
                sessionStorage["LabelLibraryName"] = 'Case';
                DisplayTableLibrary(headerData, datasort);
                $('#tblBody').empty();
            })

            $('#btnTableViewSetting, #popupbtnTableView').click(function () {
                $('#panOption').popup('close');
                window.location.hash = '';
                window.location.hash = "LibraryPage";
                headerData = ['#', 'Table View', 'Locations', 'Modified By', 'Last Modified'];
                datasort = ['int', 'string', , 'string', 'string', 'date'];
                sessionStorage["LibraryName"] = 'Table View Settings';
                sessionStorage["LabelLibraryName"] = 'Table View';
                DisplayTableLibrary(headerData, datasort);
                $('#tblBody').empty();
            })
            
            $('#btnAddLibrary').click(function () {
                if (sessionStorage.LibraryName == "PIM") {
                    $('#divAddProductTitle').text('Add Products By');
                    $('#listForAdding').css({
                        visibility: 'visible',
                        display: 'block'
                    });
                    $('#listForOption').css({
                        visibility: 'hidden',
                        display: 'none'
                    });
                    $('#divAddProductPopup').popup('open');
                }
                else if (sessionStorage.LibraryName == "Item Modifiers") {
                    $('#popItemModifierName').popup('open');
                    //sessionStorage["AddViewLibBCName"] == "Item Modifiers Parameters";
                    //GoToThisPage();
                }
                //                else if (sessionStorage.LibraryName == "Knowledge Management") {
                //                    $('#panAddKnowledgeManagement').show();
                //                }
                else if (sessionStorage.LibraryName == "Printers") {
                    sessionStorage["AddViewLibBCName"] = "View Printer";
                    sessionStorage.removeItem('ViewID');
                    sessionStorage.removeItem('FowstTitle');

                    LibraryTable.AddPrinterPage();
                    window.location.hash = 'UniversalPage';
                    // InitVal();
                } else if (sessionStorage.LibraryName == "Table View Settings") {
                    sessionStorage["AddViewLibBCName"] = "New Table View Template";

                    LibraryTable.AddTableViewPage();
                    window.location.hash = 'UniversalPage';
                    // InitVal();
                } else {
                    $('#panAddLibrary').popup('open');
                    $('#txtSearch').focus();
                }
            });

            $('#butSaveItemModifier').click(function (e) {
                LibraryTable.AddItemModifier();
            });

            $('#butCancelItemModifier').click(function (e) {
                $('#popItemModifierName').popup('close');
                $('#txtItemModifierNewName').val('');
            });

            $('#btnCancelLibrary').click(function () {
                $('#panAddLibrary').popup('close');
                ClearAddLibraryFieldsPopUp();
            })

            $('.btnClosePopUp').click(function () {
                ClearAddLibraryFieldsPopUp();
            })

            $('#btnMDMSettings').click(function () {
                window.location.href = 'MDMSettings';
            })

            $('#btnSearchLibrary').click(function () {
                SearchLibrary($('#txtSearch').val());
            })

            $('#txtSearch').keyup(function (e) {
                if (e.keyCode == 13) {
                    SearchLibrary($(this).val());

                }
                else if ($(this).val() == "") {
                    ClearAddLibraryFieldsPopUp();
                }
            })

            $('#btnAddToLibrary').click(function () {
                $('#UniversalDiv').empty();
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
                else if (sessionStorage.LibraryName == "Employees") {
                    sessionStorage["AddViewLibBCName"] = "Add Employee";
                    sessionStorage["OptionToAdd"] = "Add Employee";
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

                GoToThisPage($('#txtSearch').val());
            })
            $('#btnOKOrgEmp, #btnSaveAndSend').click(function (e) {
                e.preventDefault();
                $('#panOrgEmpMessage').popup('close');
            })
            $('#btnCancelOrgEmp, #btnCancelEmail').click(function (e) {
                e.preventDefault();
                $('#panOrgEmpMessage').popup('close');
            })

            $('#btnSaveKM').click(function () {
                SaveCase();
            });

            $('#btnCancelKM').click(function () {
                $('#panAddKnowledgeManagement').hide();
            });

            $('#btnOptionAddProduct').click(function () {
                $('#divAddProductTitle').text('Options');
                $('#listForAdding').css({
                    visibility: 'hidden',
                    display: 'none'
                });
                $('#listForOption').css({
                    visibility: 'visible',
                    display: 'block'
                });
                $('#divAddProductPopup').popup('open');
            })

            $('.ui-overlay-a').click(function () {
                ClearAddLibraryFieldsPopUp();
            })

            $('#btnWorkSchedule').click(function (e) {
                AjaxSend({
                    url: 'Login3/TestCookies',
                    success: function (vData) {
                        window.location.hash = "UniversalPage";
                        sessionStorage["AddViewLibBCName"] = "Work Schedule";
                        GoToThisPage();
                    },
                    error: function (vError) {
                        $('#TitleOrgEmp').text('Requires Internet Connection');
                        $('#MsgOrgEmp').text('This feature requires internet connection.');
                        $('#panOrgEmpMessage').popup('open');
                        $('#btnOKOrgEmp').text('Ok').click(function (e) {
                            e.preventDefault();
                            $('#panOrgEmpMessage').popup('close');
                        })
                        $('#divBtnCancel').hide();
                        return;
                    }

                });

            })

            $('#formsearchdesktop, #formsearchmobile').submit(function (e) {
                e.preventDefault();
                SearchInTable($('.txtSearchdesktop').val() || $('.txtSearchmobile').val());
            })

            $('.btnSearch').click(function () {
                SearchInTable($('.txtSearchdesktop').val());
            })

            $('#btnSaveObject').click(function () {
                SaveObject();
            });

            $('#btnAddSolution').click(function () {
                $('#panAddSolution').show();
                $('#txtSolutionAndAnswer').val('').height('50px').focus();
                $('#listSolutionSteps').empty();
                ctrSolution = 0;
            })

            $('#btnSteps').click(function () {
                $('#panAddSteps').popup('open');
                $('#txtNewSteps').focus();
            })

            $('#btnOkAndAddNewSteps').click(function () {
                NewSteps();
                $('#txtNewSteps').val('').height('50px').focus();
            })

            $('#btnOkNewSteps').click(function () {
                NewSteps();
                $('#panAddSteps').popup('close');
            })

            $('#btnCancelNewSteps').click(function () {
                $('#panAddSteps').popup('close');
                $('#txtNewSteps').val('').height('50px');
                editSolutionStep = false;
            })

            $('#btnCancelSolution').click(function () {
                $('#panAddSolution').hide();
                ctrSolutionStep = 0;
                arrSolutionSteps = [];
                ClearSolutionField();
            })

            $('#btnOkSolution').click(function () {
                if ($('.mobilecontainer').length == 0) {
                    alert('Please add team');
                    return;
                }

                if ($.trim($('#txtSolutionAndAnswer').val()) == '') {
                    $('#txtSolutionAndAnswer').focus();
                    return;
                }

                $('#panAddSolution').hide();
                ctrSolutionStep = 0;
                if (editSolution == false) {
                    ctrSolution = ctrSolution + 1;
                    arrSolution.push({
                        //No: ctrSolution,
                        Solution: $('#txtSolutionAndAnswer').val(),
                        Step: arrSolutionSteps,
                        State: 'Add',
                        AllowedTeam: arrAllowedTeam,
                        IsPublic: ($('input[name="chbPrivate"]').is(':checked') == true ? false : true)
                    })
                }
                else {
                    arrSolution[arrIndexSolution].Solution = $('#txtSolutionAndAnswer').val();
                    arrSolution[arrIndexSolution].Step = arrSolutionSteps;
                    arrSolution[arrIndexSolution].AllowedTeam = arrAllowedTeam;
                    arrSolution[arrIndexSolution].IsPublic = ($('input[name="chbPrivate"]').is(':checked') == true ? false : true)
                }

                DisplaySolution();
                editSolution = false;
                arrSolutionSteps = [];
                arrAllowedTeam = [];
            })

            $('#btnPrivateSolution').click(function () {
                BrowsePopup('LibraryCRMTeam', 'Team',
                    [
                        {
                            AutoSearch: true,
                            AddButton: [
                                {
                                    Text: 'Create New Team',
                                    Click: function () {
                                        $('#panNewTeam').popup('open');
                                        $('#txtNewTeamName').val('').focus();
                                    }
                                }
                            ],
                            DisplayText: [{ Column: 'Name'}],
                            Validation: function (e) {
                                if ($('.mobilecontainer[id="' + e.ID + '"]').length >= 1) {
                                    return '"' + e.Name + '" already exist.'
                                }
                                return true;
                            },
                            Click: function (e) {
                                arrAllowedTeam.push({
                                    State: 'Add',
                                    Name: e.Name,
                                    TeamID: e.ID
                                });
                                DisplayAllowedTeam();
                            }
                        }
                    ]
                )
            })

            $('#btnOkNewTeam').click(function () {
                if ($.trim($('#txtNewTeamName').val()) == "") {
                    $('#txtNewTeamName').focus();
                    return;
                }

                AjaxSend({
                    url: 'KnowledgeManagementAndQuestionaire/SaveNewTeam',
                    data: { TeamName: $('#txtNewTeamName').val(), DateNow: new Date() },
                    success: function (vData) {
                        if (vData.Results == "True") {
                            $('#txtNewTeamName').val('');
                            $('#panNewTeam').popup('close');
                            $('#btnPrivateSolution').click();
                        }
                        else {
                            NewMessageBox(vData.Results, 'ERROR');
                        }
                    }
                })
            })

            $('#chbPrivate').change(function () {
                if ($(this).is(':checked')) {
                    $('#divAllowedTeam').show();
                }
                else {
                    $('#divAllowedTeam').hide();
                }
            })

        })// End document

        function DisplayAllowedTeam(e) {
            var listofAllowedTeam = $('#listofAllowedTeam');

            listofAllowedTeam.empty();

            for (var i = 0; i < arrAllowedTeam.length; i++) {
                arrAllowedTeam[i].No = i + 1;
                if (arrAllowedTeam[i].State != 'Remove') {
                    listofAllowedTeam.append($('<div class="mobilecontainer">')
                        .attr('id', arrAllowedTeam[i].ID)
                        .data(arrAllowedTeam[i])
                        .append($('<div class="mobilenumber">')
                            .text(arrAllowedTeam[i].Name)
                        )
                        .append($('<div class="closemobilenumber">')
                            .data(arrAllowedTeam[i])
                            .text('x')
                            .click(function () {
                                var no = $(this).data().No;

                                if ($(this).data().State == 'Add') {
                                    arrAllowedTeam = $.grep(arrAllowedTeam, function (e) {
                                        return e.No != no
                                    })
                                }
                                else {
                                    arrAllowedTeam[no - 1].State = 'Remove';
                                }

                                DisplayAllowedTeam()
                            })
                        )
                    )
                }
            }
        }

        function DisplaySolution() {
            var listSolution = $('#listSolution');

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
                                    console.log($(this).data())
                                    arrAllowedTeam = [];
                                    arrIndexSolution = CDec2($(this).data().No) - 1;
                                    editSolution = true;
                                    AddSolutionStep($(this).data().Step)
                                    arrSolutionSteps = $(this).data().Step;
                                    arrAllowedTeam = $(this).data().AllowedTeam;
                                    DisplayAllowedTeam(arrAllowedTeam);
                                    $('#panAddSolution').show();
                                    $('#txtSolutionAndAnswer').val($(this).data().Solution).focus();
                                    $('input[name="chbPrivate"]').prop('checked', (CBool($(this).data().IsPublic) == true ? false : true)).checkboxradio('refresh');

                                    if (CBool($(this).data().IsPublic) == false) {
                                        $('#divAllowedTeam').show();
                                    }
                                    else {
                                        $('#divAllowedTeam').hide();
                                    }
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
            if ($.trim($('#txtNewSteps').val()) == '') {
                $('#txtNewSteps').focus();
                return;
            }

            if (editSolutionStep == false) {
                ctrSolutionStep = ctrSolutionStep + 1;

                arrSolutionSteps.push({
                    StepID: '',
                    //No: ctrSolutionStep,
                    Step: $('#txtNewSteps').val(),
                    State: 'Add'
                })
            }
            else {
                arrSolutionSteps[arrIndexStep].Step = $('#txtNewSteps').val();
            }


            AddSolutionStep(arrSolutionSteps);
            $('#txtNewSteps').val('');
            editSolutionStep = false;
        }

        function AddSolutionStep(arrSolutionSteps) {
            var listSolutionSteps = $('#listSolutionSteps');

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
                                            $('#panAddSteps').popup('open');
                                            $('#txtNewSteps').val($(this).data().Step).focus();
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
            $('#txtSolutionAndAnswer').val('');
            $('#listSolutionSteps').empty();
        }

        function SearchInTable(s) {
            if (sessionStorage.LibraryName == "FOWST") {
                LibraryTable.Fowst(s);
            }
            else if (sessionStorage.LibraryName == "Authorized Devices") {
                LibraryTable.Devices(s);
            }
            else if (sessionStorage.LibraryName == "Receipt Settings") {
                LibraryTable.ReceiptSettings(s);
            }
            else if (sessionStorage.LibraryName == "Security Roles") {
                LibraryTable.SecurityRoles(s);
            }
            else if (sessionStorage.LibraryName == "Employees") {
                LibraryTable.Employees(s);
            }
            else if (sessionStorage.LibraryName == "PIM") {
                LibraryTable.PIMPage(s);
            }
            else if (sessionStorage.LibraryName == "Printer Roles") {
                LibraryTable.PrinterRoles(s);
            }
            else if (sessionStorage.LibraryName == "Hot Buttons Settings") {
                LibraryTable.HotButtons(s);
            }
            else if (sessionStorage.LibraryName == "Group Settings") {
                LibraryTable.GroupSettings(s);
            }
            else if (sessionStorage.LibraryName == "Customer / Client List") {
                LibraryTable.CustomerList(s);
            }
            else if (sessionStorage.LibraryName == "Supplier / Vendor List") {
                LibraryTable.SupplierList(s);
            }
            else if (sessionStorage.LibraryName == "Sales Team") {
                LibraryTable.SalesTeam(s);
            } else if (sessionStorage.LibraryName == 'Item Modifiers') {
                LibraryTable.SearchItemModifiers(s);
            } else if (sessionStorage.LibraryName == 'Printers') {
                LibraryTable.Printers(s);
            }
            else if (sessionStorage.LibraryName == "Table View Settings") {
                LibraryTable.TableView(s);
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
            $('#btnSystemAdministration').click(function () {
                window.location.hash = 'SystemAdministration';
            })

            $('#btnLibraryPage').click(function () {
                window.location.hash = 'LibraryPage';
                $('#btnMenuProductInformation').click();
                if (headerData.length != undefined) {
                    DisplayTableLibrary(headerData, datasort);
                }
            })

            $('#btnSublistPage').click(function () {
                sessionStorage.removeItem('SubListName');
                window.location.hash = '';
                window.location.hash = 'LibraryPage';
            })

            $('#btnSublistParentPage').click(function () {
                window.location.hash = 'LibraryPage';
            })

            $('#btnSublistSecondChildPage').click(function () {
                window.location.hash = 'LibraryPage';
            })
        }

        function ClearAddLibraryFieldsPopUp() {
            $('#lstSearchFound').empty();
            $('#divNoResultsFound, .divLibAddOption').hide();
            $('#txtSearch').val('');
        }

        function DisplayTableLibrary(HeaderData, DataSortType) {
            $('#tblHeader').empty();
            $('#tblHeaderDT').empty();
            $('#divNotDataTable').pageshow();
            $('.txtSearchdesktop, .txtSearchmobile').val('')

            for (var i = 0; i < HeaderData.length; i++) {
                $('#tblHeader').append($('<th>')
                                            .text(HeaderData[i])
                                            .attr('data-sort', DataSortType[i])
                                            .css('cursor', 'pointer')
                                        );
                $('#tblHeaderDT').append($('<th>')
                                            .text(HeaderData[i])
                                            .css('cursor', 'pointer')
                                        );
            }

            if (sessionStorage.LibraryName == "FOWST") {
                LibraryTable.Fowst();
            }
            else if (sessionStorage.LibraryName == "Authorized Devices") {
                LibraryTable.Devices();
            }
            else if (sessionStorage.LibraryName == "Receipt Settings") {
                $('#divDataTable').pageshow();
                LibraryTable.ReceiptSettings();
            }
            else if (sessionStorage.LibraryName == "Security Roles") {
                LibraryTable.SecurityRoles();
            }
            else if (sessionStorage.LibraryName == "Employees") {
                $('#divWorkSchedule').show();
                LibraryTable.Employees();
            }
            else if (sessionStorage.LibraryName == "Printer Roles") {
                LibraryTable.PrinterRoles();
            }
            else if (sessionStorage.LibraryName == "PIM") {
                LibraryTable.PIMPage();
            }
            else if (sessionStorage.LibraryName == "Hot Buttons Settings") {
                LibraryTable.HotButtons();
            }
            else if (sessionStorage.LibraryName == "Group Settings") {
                LibraryTable.GroupSettings();
            }
            else if (sessionStorage.LibraryName == "Customer / Client List") {
                LibraryTable.CustomerList();
            }
            else if (sessionStorage.LibraryName == "Supplier / Vendor List") {
                LibraryTable.SupplierList();
            }
            else if (sessionStorage.LibraryName == "Sales Team") {
                LibraryTable.SalesTeam();
            }
            else if (sessionStorage.LibraryName == "Item Modifiers") {
                LibraryTable.SearchItemModifiers('');
            }
            else if (sessionStorage.LibraryName == "Knowledge Management") {
                LibraryTable.SearchKnowledgeManagementAndQuestionaire('');
            }
            else if (sessionStorage.LibraryName == "Printers") {
                LibraryTable.Printers();
            }
            else if (sessionStorage.LibraryName == "Table View Settings") {
                LibraryTable.TableView();
            }

            IsLibraryDisplayInTable = true;

            if (sessionStorage.SubListName != undefined) {
                if (sessionStorage.SubListName == "FOWST Product List") {
                    $('#divAddLibrary').hide();
                    $('#divInfoForFowstProdList').show();
                }
                else {
                    $('#divAddLibrary').show();
                    $('#divInfoForFowstProdList').hide();
                }
            }
            else if (sessionStorage.SubListName == undefined) {
                $('#divAddLibrary').show();
                $('#divInfoForFowstProdList').hide();
            }

            if (sessionStorage.LibraryName == "Employees") {
                $('#divWorkSchedule').show();
            }
            else {
                $('#divWorkSchedule').hide();
            }

            if (sessionStorage.LibraryName == "PIM") {
                $('#divDownloadFilePIM').show();
                $('#divOptionAddProduct').show();
            }
            else {
                $('#divDownloadFilePIM').hide();
                $('#divOptionAddProduct').hide();
            }
        }

        function SearchLibrary(SearchText) {
            if (sessionStorage.LibraryName == "FOWST") {
                LibraryTable.FowstSearch(SearchText);
            }
            else if (sessionStorage.LibraryName == "Authorized Devices") {
                LibraryTable.DevicesSearch(SearchText);
            }
            else if (sessionStorage.LibraryName == "Receipt Settings") {
                LibraryTable.ReceiptSettingsSearch(SearchText);
            }
            else if (sessionStorage.LibraryName == "Security Roles") {
                LibraryTable.SecurityRolesSearch(SearchText);
            }
            else if (sessionStorage.LibraryName == "Employees") {
                LibraryTable.EmployeesSearch(SearchText);
            }
            else if (sessionStorage.LibraryName == "Printer Roles") {
                LibraryTable.PrinterRolesSearch(SearchText);
            }
            else if (sessionStorage.LibraryName == "PIM") {
                LibraryTable.PIMSearch(SearchText);
            }
            else if (sessionStorage.LibraryName == "Hot Buttons Settings") {
                LibraryTable.HotButtonsSearch(SearchText);
            }
            else if (sessionStorage.LibraryName == "Group Settings") {
                LibraryTable.GroupSettingsSearch(SearchText);
            }
            else if (sessionStorage.LibraryName == "Customer / Client List") {
                LibraryTable.CustomerListSearch(SearchText);
            }
            else if (sessionStorage.LibraryName == "Supplier / Vendor List") {
                LibraryTable.SupplierListSearch(SearchText);
            }
            else if (sessionStorage.LibraryName == "Sales Team") {
                LibraryTable.SalesTeamSearch(SearchText);
            }
            else if (sessionStorage.LibraryName == "Knowledge Management") {
                LibraryTable.KnowledgeManagementAndQuestionaire(SearchText);
            }
        }

        function GoToThisPage(AddDevice) {

            //sessionStorage["AddLibraryName"] = AddDevice;
            $('#UniversalDiv').html('<div style="width:100%; text-align:center;">Loading ...</div>');

            if (sessionStorage.LibraryName == "Organization") {
                window.location.hash = 'UniversalPage';
                LibraryTable.OrganizationPage();
            }
            else if (sessionStorage.LibraryName == "FOWST") {
                window.location.hash = 'UniversalPage';

                LibraryTable.AddFowstPage();
            }
            else if (sessionStorage.LibraryName == "Authorized Devices") {

                window.location.hash = 'UniversalPage';

                if (sessionStorage.AddViewLibBCName == "Add Device") {
                    LibraryTable.AddDevicePage();
                }
                else {
                    LibraryTable.ViewDevicePage();
                }
            }
            else if (sessionStorage.LibraryName == "Receipt Settings") {
                window.location.hash = 'UniversalPage';

                var pData = {};

                if (sessionStorage.AddViewLibBCName == "View Receipt Template") {
                    pData.IsNew = 0;
                    pData.ID = sessionStorage.ViewID;
                }
                else {
                    pData.IsNew = 1;
                    pData.ID = '';
                }

                AjaxSend({
                    url: 'ReceiptSettings/setCreateNew',
                    data: pData,
                    success: function (vData) {
                        LibraryTable.AddReceiptSettingsPage();
                    }
                });

            }
            else if (sessionStorage.LibraryName == "Security Roles") {
                window.location.hash = 'UniversalPage';

                if (sessionStorage.AddViewLibBCName == "Add Security Role") {
                    LibraryTable.AddSecurityRolePage();
                }
                else {
                    LibraryTable.ViewSecurityRolePage();
                }

            }
            else if (sessionStorage.LibraryName == "Employees") {
                window.location.hash = 'UniversalPage';

                if (sessionStorage.AddViewLibBCName == "Add Employee") {
                    LibraryTable.AddEmployeePage();
                }
                else if (sessionStorage.AddViewLibBCName == "Work Schedule") {
                    LibraryTable.WorkSchedulePage();
                }
                else {
                    LibraryTable.ViewEmployeePage();
                }
            }
            else if (sessionStorage.LibraryName == "Products") {
                window.location.hash = 'UniversalPage';
                LibraryTable.PIMPage();
            }
            else if (sessionStorage.LibraryName == "Sales Returns Settings") {
                window.location.hash = 'UniversalPage';
                LibraryTable.SalesReturnsSettingsPage();
            }
            else if (sessionStorage.LibraryName == "Discount Settings") {
                window.location.hash = 'UniversalPage';
                LibraryTable.DiscountSettingsPage();
            }
            else if (sessionStorage.LibraryName == "Printer Roles") {
                if (sessionStorage.AddViewLibBCName == "Add Printer Role") {
                    CreateNewDevicePrinterEntry(1, '');
                    sessionStorage["DevicePrinterInfoUsedby"] = "Printer Roles";
                }
                else {
                    LibraryTable.ViewPrinterRolesPage();
                }
            }
            else if (sessionStorage.LibraryName == "Device Printer Setup") {
                LibraryTable.DevicePrinterSetup();
            }
            else if (sessionStorage.LibraryName == "PIM") {
                $('#divAddProductPopup').popup('close');
                $('#divAdjustProdPricesPopup').popup('close');

                if (sessionStorage.AddViewLibBCName == "Import Products by Excel File") {
                    LibraryTable.ImportByExcelFilePage();
                }
                else if (sessionStorage.AddViewLibBCName == "FOWST Product List") {
                    LibraryTable.FowstProductList();
                }
                else if (sessionStorage.AddViewLibBCName == "Single Product Price Adjustment") {
                    LibraryTable.SingleProductPriceAdjustmentPage();
                }
                else if (sessionStorage.AddViewLibBCName == "Bulk Price Adjustment") {
                    LibraryTable.BulkPriceAdjustmentPage();
                }
                else if (sessionStorage.AddViewLibBCName == "Scan Barcode / Search Product"
                        || sessionStorage.AddViewLibBCName == "Subscribe To Company Product List"
                        || sessionStorage.AddViewLibBCName == "Add Product"
                        || sessionStorage.AddViewLibBCName == "SKU Record") {
                    LibraryTable.CompanyWideProductListPage();
                }

                if (sessionStorage.SubListName == "Company Wide Product List") {
                    $("#panAddLibrary").popup('close');
                    $("#divAddProductPopup").popup("open");
                }
                else if (sessionStorage.SubListName == "Upload / Download Product List") {
                    LibraryTable.UploadDownloadProdListPage();
                }

            }
            else if (sessionStorage.LibraryName == "Decimals in Quantity Settings") {
                window.location.hash = 'UniversalPage';
                LibraryTable.DecimalsInQuantitySettingsPage();
            }
            else if (sessionStorage.LibraryName == "POS Settings") {
                window.location.hash = 'UniversalPage';
                LibraryTable.POSSettingsPage();
            }
            else if (sessionStorage.LibraryName == "Sales Order Settings") {
                window.location.hash = 'UniversalPage';
                LibraryTable.SalesOrderSettingsPage();
            }
            else if (sessionStorage.LibraryName == "Receiving And Transfer-Out Settings") {
                window.location.hash = 'UniversalPage';
                LibraryTable.ReceivingAndTransferOutPage();
            }
            else if (sessionStorage.LibraryName == 'Inventory Count Settings') {
                window.location.hash = 'UniversalPage';
                LibraryTable.InventoryCount();
            }
            else if (sessionStorage.LibraryName == 'Daily Beginning Balance Settings') {
                window.location.hash = 'UniversalPage';
                LibraryTable.DailyBeginningBalance();
            }
            else if (sessionStorage.LibraryName == 'Purchase Order Settings') {
                window.location.hash = 'UniversalPage';
                LibraryTable.PurchaseOrderSettings();
            }
            else if (sessionStorage.LibraryName == 'Purchase Order Settings') {
                window.location.hash = 'UniversalPage';
                LibraryTable.PurchaseOrder();
            }
            else if (sessionStorage.LibraryName == "Hot Buttons Settings") {
                window.location.hash = 'UniversalPage';

                if (sessionStorage.AddViewLibBCName == "Add Hot Button") {
                    LibraryTable.AddHotButtonPage();
                }
                else {
                    LibraryTable.ViewHotButtonPage();
                }
            }
            else if (sessionStorage.LibraryName == "Group Settings") {
                sessionStorage["AddViewLibBCName"] = "Add Group";
                window.location.hash = 'UniversalPage';
                LibraryTable.AddGroupPage();
            }
            else if (sessionStorage.LibraryName == "Customer / Client List") {
                window.location.hash = 'UniversalPage';

                if (sessionStorage.AddViewLibBCName == "Add New Customer") {
                    LibraryTable.AddCustomerPage();
                }
                else {
                    LibraryTable.ViewCustomerPage();
                }

            }
            else if (sessionStorage.LibraryName == "Supplier / Vendor List") {
                window.location.hash = 'UniversalPage';

                if (sessionStorage.AddViewLibBCName == "Add New Supplier") {
                    LibraryTable.AddSupplierPage();
                }
                else {
                    LibraryTable.ViewSupplierPage();
                }

            }
            else if (sessionStorage.LibraryName == "Sales Team") {
                window.location.hash = 'UniversalPage';
                if (sessionStorage.AddViewLibBCName == "Add Team") {
                    LibraryTable.AddCRMPage();
                }
                else {
                    LibraryTable.ViewCRMPage();
                }
            }
            else if (sessionStorage.LibraryName == "Item Modifiers") {
                sessionStorage["AddViewLibBCName"] = "Item Modifier Parameters";
                window.location.hash = 'UniversalPage';
                LibraryTable.PageItemModifiers();
            }
            else if (sessionStorage.LibraryName == "Knowledge Management") {
                $('#panAddLibrary').popup('close');
                setTimeout(function () {
                    LibraryTable.GetKnowledgeManagementNumber();
                    $('#panAddKnowledgeManagement').show();
                    $('#txtSolution, #txtSteps').height('50px');
                    //GetAllTopics();
                    //GetAllProducts();
                    //GetAllLanguages();
                }, 500);
            }
            else if (sessionStorage.LibraryName == 'Loyalty Program Settings') {
                window.location.hash = 'UniversalPage';
                LibraryTable.LoyaltyProgram();
            }

            if (sessionStorage.SecondSubListName == "Single Product Price Adjustment") {
                LibraryTable.SingleProductPriceAdjustmentPage();
            }
            else if (sessionStorage.SecondSubListName == "Bulk Price Adjustment") {
                LibraryTable.BulkPriceAdjustmentPage();
            }
            else if (sessionStorage.LibraryName == "Printers") {
                window.location.hash = 'UniversalPage';

                LibraryTable.AddPrinterPage();
            }
            else if (sessionStorage.LibraryName == "Table View Settings") {
                LibraryTable.AddTableViewPage();
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

                                            $("#imgOrg").attr('src', '/Content/images/tick.png');
                                            for (var i = 0; i <= vDataCheck.PIMChecklistData.Rows.length - 1; i++) {
                                                if (vData.PIMChecklistData.Rows[p].ModuleType == vDataCheck.PIMChecklistData.Rows[i].ModuleType) {
                                                    $("#imgOrg").attr('src', '/Content/images/cross.png');
                                                }
                                            }
                                            $("#lblOrgLastUpdated").text(((vData.PIMChecklistData.Rows[p].LastUpdated || '') == '' ? '' : ($.format.date(new Date(vData.PIMChecklistData.Rows[p].LastUpdated), 'MMM dd, yyyy HH:mm') || '')));

                                        } else if (vData.PIMChecklistData.Rows[p].ModuleType == "FOWST") {
                                            $("#imgFOWST").attr('src', '/Content/images/tick.png');
                                            for (var i = 0; i <= vDataCheck.PIMChecklistData.Rows.length - 1; i++) {
                                                if (vData.PIMChecklistData.Rows[p].ModuleType == vDataCheck.PIMChecklistData.Rows[i].ModuleType) {
                                                    $("#imgFOWST").attr('src', '/Content/images/cross.png');
                                                }
                                            }
                                            $("#lblFOWSTCount").text(AccountingFormat(vData.PIMChecklistData.Rows[p].Cnt, 0));
                                            $("#lblFOWSTLastUpdated").text(((vData.PIMChecklistData.Rows[p].LastUpdated || '') == '' ? '' : ($.format.date(new Date(vData.PIMChecklistData.Rows[p].LastUpdated), 'MMM dd, yyyy HH:mm') || '')));

                                        } else if (vData.PIMChecklistData.Rows[p].ModuleType == "Authorized Device") {
                                            $("#imgDevice").attr('src', '/Content/images/tick.png');
                                            for (var i = 0; i <= vDataCheck.PIMChecklistData.Rows.length - 1; i++) {
                                                if (vData.PIMChecklistData.Rows[p].ModuleType == vDataCheck.PIMChecklistData.Rows[i].ModuleType) {
                                                    $("#imgDevice").attr('src', '/Content/images/cross.png');
                                                }
                                            }
                                            $("#lblDeviceCount").text(AccountingFormat(vData.PIMChecklistData.Rows[p].Cnt, 0));
                                            $("#lblDeviceLastUpdated").text(((vData.PIMChecklistData.Rows[p].LastUpdated || '') == '' ? '' : ($.format.date(new Date(vData.PIMChecklistData.Rows[p].LastUpdated), 'MMM dd, yyyy HH:mm') || '')));

                                        } else if (vData.PIMChecklistData.Rows[p].ModuleType == "Receipt Settings") {
                                            $("#imgReceiptSettings").attr('src', '/Content/images/tick.png');
                                            for (var i = 0; i <= vDataCheck.PIMChecklistData.Rows.length - 1; i++) {
                                                if (vData.PIMChecklistData.Rows[p].ModuleType == vDataCheck.PIMChecklistData.Rows[i].ModuleType) {
                                                    $("#imgReceiptSettings").attr('src', '/Content/images/cross.png');
                                                }
                                            }
                                            $("#lblReceiptSettingsCount").text(AccountingFormat(vData.PIMChecklistData.Rows[p].Cnt, 0));
                                            $("#lblReceiptSettingsLastUpdated").text(((vData.PIMChecklistData.Rows[p].LastUpdated || '') == '' ? '' : ($.format.date(new Date(vData.PIMChecklistData.Rows[p].LastUpdated), 'MMM dd, yyyy HH:mm') || '')));

                                        } else if (vData.PIMChecklistData.Rows[p].ModuleType == "Security Access") {
                                            $("#imgSecurityAccess").attr('src', '/Content/images/tick.png');
                                            for (var i = 0; i <= vDataCheck.PIMChecklistData.Rows.length - 1; i++) {
                                                if (vData.PIMChecklistData.Rows[p].ModuleType == vDataCheck.PIMChecklistData.Rows[i].ModuleType) {
                                                    $("#imgSecurityAccess").attr('src', '/Content/images/cross.png');
                                                }
                                            }
                                            $("#lblSecurityAccessCount").text(AccountingFormat(vData.PIMChecklistData.Rows[p].Cnt, 0));
                                            $("#lblSecurityAccessLastUpdated").text(((vData.PIMChecklistData.Rows[p].LastUpdated || '') == '' ? '' : ($.format.date(new Date(vData.PIMChecklistData.Rows[p].LastUpdated), 'MMM dd, yyyy HH:mm') || '')));

                                        } else if (vData.PIMChecklistData.Rows[p].ModuleType == "Employee") {
                                            $("#imgEmployeeAccess").attr('src', '/Content/images/tick.png');
                                            for (var i = 0; i <= vDataCheck.PIMChecklistData.Rows.length - 1; i++) {
                                                if (vData.PIMChecklistData.Rows[p].ModuleType == vDataCheck.PIMChecklistData.Rows[i].ModuleType) {
                                                    $("#imgEmployeeAccess").attr('src', '/Content/images/cross.png');
                                                }
                                            }
                                            $("#lblEmployeeCount").text(AccountingFormat(vData.PIMChecklistData.Rows[p].Cnt, 0));
                                            $("#lblEmployeeLastUpdated").text(((vData.PIMChecklistData.Rows[p].LastUpdated || '') == '' ? '' : ($.format.date(new Date(vData.PIMChecklistData.Rows[p].LastUpdated), 'MMM dd, yyyy HH:mm') || '')));

                                        } else if (vData.PIMChecklistData.Rows[p].ModuleType == "Products") {
                                            $("#imgProduct").attr('src', '/Content/images/tick.png');
                                            for (var i = 0; i <= vDataCheck.PIMChecklistData.Rows.length - 1; i++) {
                                                if (vData.PIMChecklistData.Rows[p].ModuleType == vDataCheck.PIMChecklistData.Rows[i].ModuleType) {
                                                    $("#imgProduct").attr('src', '/Content/images/cross.png');
                                                }
                                            }
                                            $("#lblProductCount").text(AccountingFormat(vData.PIMChecklistData.Rows[p].Cnt, 0));
                                            $("#lblProductLastUpdated").text(((vData.PIMChecklistData.Rows[p].LastUpdated || '') == '' ? '' : ($.format.date(new Date(vData.PIMChecklistData.Rows[p].LastUpdated), 'MMM dd, yyyy HH:mm') || '')));

                                        } else if (vData.PIMChecklistData.Rows[p].ModuleType == "Customers/Clients") {
                                            $("#imgCustomer").attr('src', '/Content/images/tick.png');
                                            for (var i = 0; i <= vDataCheck.PIMChecklistData.Rows.length - 1; i++) {
                                                if (vData.PIMChecklistData.Rows[p].ModuleType == vDataCheck.PIMChecklistData.Rows[i].ModuleType) {
                                                    $("#imgCustomer").attr('src', '/Content/images/cross.png');
                                                }
                                            }
                                            $("#lblCustomerCount").text(AccountingFormat(vData.PIMChecklistData.Rows[p].Cnt, 0));
                                            $("#lblCustomerLastUpdated").text(((vData.PIMChecklistData.Rows[p].LastUpdated || '') == '' ? '' : ($.format.date(new Date(vData.PIMChecklistData.Rows[p].LastUpdated), 'MMM dd, yyyy HH:mm') || '')));

                                        } else if (vData.PIMChecklistData.Rows[p].ModuleType == "Supplier/Vendor") {
                                            $("#imgSupplier").attr('src', '/Content/images/tick.png');
                                            for (var i = 0; i <= vDataCheck.PIMChecklistData.Rows.length - 1; i++) {
                                                if (vData.PIMChecklistData.Rows[p].ModuleType == vDataCheck.PIMChecklistData.Rows[i].ModuleType) {
                                                    $("#imgSupplier").attr('src', '/Content/images/cross.png');
                                                }
                                            }
                                            $("#lblSupplierCount").text(AccountingFormat(vData.PIMChecklistData.Rows[p].Cnt, 0));
                                            $("#lblSupplierLastUpdated").text(((vData.PIMChecklistData.Rows[p].LastUpdated || '') == '' ? '' : ($.format.date(new Date(vData.PIMChecklistData.Rows[p].LastUpdated), 'MMM dd, yyyy HH:mm') || '')));

                                        } else if (vData.PIMChecklistData.Rows[p].ModuleType == "Discount Settings") {

                                            $("#lblDiscountSettingsLastUpdated").text(((vData.PIMChecklistData.Rows[p].LastUpdated || '') == '' ? '' : ($.format.date(new Date(vData.PIMChecklistData.Rows[p].LastUpdated), 'MMM dd, yyyy HH:mm') || '')));

                                        } else if (vData.PIMChecklistData.Rows[p].ModuleType == "Device Printer") {

                                            $("#lblPrinterCount").text(AccountingFormat(vData.PIMChecklistData.Rows[p].Cnt, 0));
                                            $("#lblPrinterLastUpdated").text(((vData.PIMChecklistData.Rows[p].LastUpdated || '') == '' ? '' : ($.format.date(new Date(vData.PIMChecklistData.Rows[p].LastUpdated), 'MMM dd, yyyy HH:mm') || '')));

                                        } else if (vData.PIMChecklistData.Rows[p].ModuleType == "Hot Button") {

                                            $("#lblHotButtonCount").text(AccountingFormat(vData.PIMChecklistData.Rows[p].Cnt, 0));
                                            $("#lblHotButtonLastUpdated").text(((vData.PIMChecklistData.Rows[p].LastUpdated || '') == '' ? '' : ($.format.date(new Date(vData.PIMChecklistData.Rows[p].LastUpdated), 'MMM dd, yyyy HH:mm') || '')));

                                        } else if (vData.PIMChecklistData.Rows[p].ModuleType == "Group Setting") {

                                            $("#lblGroupSettingsCount").text(AccountingFormat(vData.PIMChecklistData.Rows[p].Cnt, 0));
                                            $("#lblGroupSettingsLastUpdated").text(((vData.PIMChecklistData.Rows[p].LastUpdated || '') == '' ? '' : ($.format.date(new Date(vData.PIMChecklistData.Rows[p].LastUpdated), 'MMM dd, yyyy HH:mm') || '')));

                                        } else if (vData.PIMChecklistData.Rows[p].ModuleType == "Team") {

                                            $("#lblSalesTeamCount").text(AccountingFormat(vData.PIMChecklistData.Rows[p].Cnt, 0));
                                            $("#lblSalesTeamLastUpdated").text(((vData.PIMChecklistData.Rows[p].LastUpdated || '') == '' ? '' : ($.format.date(new Date(vData.PIMChecklistData.Rows[p].LastUpdated), 'MMM dd, yyyy HH:mm') || '')));

                                        } else if (vData.PIMChecklistData.Rows[p].ModuleType == "PurchaseOrder") {

                                            $("#lblPurchaseOrderLastUpdated").text(((vData.PIMChecklistData.Rows[p].LastUpdated || '') == '' ? '' : ($.format.date(new Date(vData.PIMChecklistData.Rows[p].LastUpdated), 'MMM dd, yyyy HH:mm') || '')));

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
                                                    LibraryTable.FowstProductList();
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
                        LibraryTable.AddPrinterRolesPage();
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

        function SaveObject() {
            objID = NewID();

            var pData = {};
            pData.ID = objID;
            pData.Description = $('#txtObject').val();
            pData.TopicID = $('#selTopic').val();

            var lnk = '';

            if (objSelected == 'Topic') {
                lnk = 'SaveTopic';
            } else if (objSelected == 'Product') {
                lnk = 'SaveProduct';
            }
            else if (objSelected == 'Language') {
                lnk = 'SaveLanguage';
            } else if (objSelected == 'SubTopic') {
                lnk = 'SaveSubTopic';
            }

            AjaxSend({
                url: 'KnowledgeManagementAndQuestionaire/' + lnk,
                data: pData,
                success: function (vData) {

                    if ((vData.Results || '') == "True") {
                        if (objSelected == 'Topic') {
                            GetAllTopics();
                        } else if (objSelected == 'Product') {
                            GetAllProducts();
                        }
                        else if (objSelected == 'Language') {
                            GetAllLanguages();
                        } else if (objSelected == 'SubTopic') {
                            GetAllSubTopics();
                        }

                        $('#panAddObject').popup('close');
                    }
                    else {
                        MessageBox('An error occured while saving Case. Please contact Mobiz ASAP. Thank you.')
                    }
                }
            });
        }

        function SaveCase() {
            var pData = {};
            pData.CaseID = $('#txtCaseID').val();
            pData.Case = $('#txtCase').val();
            //pData.Solution = $('#txtSolution').val();
            //pData.Steps = $('#txtSteps').val();
            pData.Topic = $('#selTopic').val();
            pData.SubTopic = $('#selSubTopic').val();
            pData.Product = $('#selProduct').val();
            pData.Language = $('#selLanguage').val();
            pData.Tags = $('#txtTags').val();
            pData.Solution = arrSolution;
            pData.ProblemID = $('#txtProblemDBID').val();

            AjaxSend({
                url: 'KnowledgeManagementAndQuestionaire/SaveCase',
                data: pData,
                success: function (vData) {
                    if ((vData.Results || '') == "True") {
                        $('#panAddKnowledgeManagement').hide();
                        LibraryTable.SearchKnowledgeManagementAndQuestionaire('');
                    }
                    else {
                        MessageBox('An error occured while saving Case. Please contact Mobiz ASAP. Thank you.')
                    }

                }
            });
        }

        var objSelected = '';
        var objID = '';

        function GetAllTopics() {
            AjaxSend({
                url: 'KnowledgeManagementAndQuestionaire/GetAllTopics',
                success: function (vData) {
                    var vRows = vData.Results.Rows;

                    if (vRows.length > 0) {
                        $('#selTopic').empty();

                        for (var i = 0; i <= vRows.length - 1; i++) {
                            $('#selTopic').append(new Option(vRows[i].Topic, vRows[i].ID, false));
                        }

                        if (objID != '')
                            $('#selTopic').val(objID);
                        else
                            $('#selTopic').val(vRows[0].ID);
                    }

                    $('#selTopic').append(new Option('Add New Topic', 'AddNew', false));

                    try {
                        $('#selTopic').selectmenu('refresh');
                    } catch (e) {
                        $('#selTopic').selectmenu();
                    }
                }
            });
        }

        $('#selTopic').change(function (e) {
            if ($('#selTopic').val() == 'AddNew') {
                objSelected = 'Topic';
                $('#lblAddObject').html('Add Topic');
                $('#lblAddObjectTitle').html('Topic');
                $('#txtObject').val('');
                $('#panAddObject').popup('open');
            } else {
                GetAllSubTopics();
            }
        });

        function GetAllSubTopics() {
            var data = { TopicID: $('#selTopic').val() }
            AjaxSend({
                url: 'KnowledgeManagementAndQuestionaire/GetAllSubTopics',
                data: data,
                success: function (vData) {
                    var vRows = vData.Results.Rows;

                    $('#selSubTopic').empty();

                    if (vRows.length > 0) {
                        for (var i = 0; i <= vRows.length - 1; i++) {
                            $('#selSubTopic').append(new Option(vRows[i].SubTopic, vRows[i].ID, false));
                        }

                        if (objID != '')
                            $('#selSubTopic').val(objID);
                        else
                            $('#selSubTopic').val(vRows[0].ID);
                    } else
                        $('#selSubTopic').append(new Option('Select Topic', '', false));

                    $('#selSubTopic').append(new Option('Add New SubTopic', 'AddNew', false));

                    try {
                        $('#selSubTopic').selectmenu('refresh');
                    } catch (e) {
                        $('#selSubTopic').selectmenu();
                    }
                }
            });
        }

        $('#selSubTopic').change(function (e) {
            if ($('#selSubTopic').val() == 'AddNew') {
                objSelected = 'SubTopic';
                $('#lblAddObject').html('Add SubTopic');
                $('#lblAddObjectTitle').html('SubTopic');
                $('#txtObject').val('');
                $('#panAddObject').popup('open');
            }
        });

        function GetAllProducts() {
            AjaxSend({
                url: 'KnowledgeManagementAndQuestionaire/GetAllProducts',
                success: function (vData) {
                    var vRows = vData.Results.Rows;

                    if (vRows.length > 0) {
                        $('#selProduct').empty();

                        for (var i = 0; i <= vRows.length - 1; i++) {
                            $('#selProduct').append(new Option(vRows[i].Product, vRows[i].ID, false));
                        }

                        if (objID != '')
                            $('#selProduct').val(objID);
                        else
                            $('#selProduct').val(vRows[0].ID);
                    }

                    $('#selProduct').append(new Option('Add New Product', 'AddNew', false));

                    try {
                        $('#selProduct').selectmenu('refresh');
                    } catch (e) {
                        $('#selProduct').selectmenu();
                    }

                }
            });
        }

        $('#selProduct').change(function (e) {
            if ($('#selProduct').val() == 'AddNew') {
                objSelected = 'Product';
                $('#lblAddObject').html('Add Product');
                $('#lblAddObjectTitle').html('Product');
                $('#txtObject').val('');
                $('#panAddObject').popup('open');
            }
        });

        function GetAllLanguages() {
            AjaxSend({
                url: 'KnowledgeManagementAndQuestionaire/GetAllLanguages',
                success: function (vData) {
                    var vRows = vData.Results.Rows;

                    if (vRows.length > 0) {
                        $('#selLanguage').empty();

                        for (var i = 0; i <= vRows.length - 1; i++) {
                            $('#selLanguage').append(new Option(vRows[i].Language, vRows[i].ID, false));
                        }

                        if (objID != '')
                            $('#selLanguage').val(objID);
                        else
                            $('#selLanguage').val(vRows[0].ID);
                    }

                    $('#selLanguage').append(new Option('Add New Language', 'AddNew', false));

                    try {
                        $('#selLanguage').selectmenu('refresh');
                    } catch (e) {
                        $('#selLanguage').selectmenu();
                    }
                }
            });
        }

        $('#selLanguage').change(function (e) {
            if ($('#selLanguage').val() == 'AddNew') {
                objSelected = 'Language';
                $('#lblAddObject').html('Add Language');
                $('#lblAddObjectTitle').html('Language');
                $('#txtObject').val('');
                $('#panAddObject').popup('open');
            }
        });

        function clearKMFields() {
            $('#txtProblemDBID').val('');
            $('#txtCaseDBID').val('');
            $('#txtCase').val('');
            $('#txtSolution').val('');
            $('#txtSteps').val('');
            $('#selTopic').val('');
            $('#txtTags').val('');
            $('#txtSearch').val('');
            $('#listSolution').empty();
            arrSolution = [];
            arrSolutionSteps = [];
        }
    });
});