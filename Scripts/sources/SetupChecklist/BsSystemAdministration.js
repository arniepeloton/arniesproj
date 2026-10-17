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
        sessionStorage.SystemAdminFor = 'mcr';

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

            $('body').on('submit', '#formuploadReactivation', function (e) {
                e.preventDefault();

                $("#OrgID").val(sessionStorage.OrgID);

                //grab form data
                var formData = new FormData($(this)[0]);
                
                $.ajax({
                    url: '/PIMUpload/ReActivateProducts',
                    type: 'POST',
                    data: formData,
                    async: false,
                    cache: false,
                    contentType: false,
                    processData: false,
                    success: function (vData) {
                        LibraryTable.PIMPage();
                        $('#divPopReActivate').modal('hide');

                        if (vData.length>0){
                            vData = 'However, we cannot find data in some products in the excel file. These products are ' + vData.substr(1, vData.length);
                        } 

                        NewMessageBox('Uploaded products are now reactivated. ' + vData);
                    }
                });
            })

            $(window).bind('hashchange', function () {
                if (sessionStorage.SystemAdminFor != 'mcr') { // used in EAS
                    return;
                }
                if (window.location.hash == "#SystemAdministration" || window.location.hash == "") {
                    $('#divSystemAdDashboard').show();
                    $('#divSecond, #UniversalDiv, #divSubList, #divForPIM, #divOptionStatus').hide();
                    $('#UniversalDiv').empty();
                    $('#selOptionStatus').val('1');//.selectmenu('refresh');
                    //$('.ItemsBreadCrumbs').text('System Administration');
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
                    sessionStorage.removeItem("ItemModifierData");
                    IsAddLibraryDisplayInUnivDiv = false;
                    AddBreadCrumb({
                        text: 'System Administration', Click: function () {
                            window.location.hash = 'SystemAdministration';
                        }
                    }, '.ItemsBreadCrumbs');
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
                        //$('.ItemsBreadCrumbs').text('');

                        $('#divSystemAdDashboard, #UniversalDiv, #divSubList, #divForPIM').hide();
                        //$('#UniversalDiv').empty();
                        $('#divSecond').show();

                        if (sessionStorage.LibraryName == 'FOWST') {
                            breadcrumbs.prepend(arrowR, sessionStorage.LibraryName);
                            headerData = ['#', 'Name', 'Address', 'Last Modified By', 'Last Modified Date', 'Last Transaction Date', 'Status'];
                            datasort = ['int', 'string', 'string', 'string', 'date', 'string'];
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
                            headerData = ['#', 'Short Description', 'Product No.', 'Retail Price', 'Brand', 'Category', 'Activation Date', 'Last Modified Date', 'Status'];
                            datasort = ['int', 'string', 'string', 'int', 'string', 'string', 'date'];
                            DisplayTableLibrary(headerData, datasort);
                        }

                        else if (sessionStorage.LibraryName == 'Printer Roles') {
                            breadcrumbs.prepend(arrowR, sessionStorage.LibraryName);
                            headerData = ['#', 'Printer Role Name', 'Printer Type', 'Active', 'Last Modified By', 'Last Modified Date'];
                            datasort = ['int', 'string', 'string', 'string', 'string', 'date'];
                            DisplayTableLibrary(headerData, datasort);
                        }

                        else if (sessionStorage.LibraryName == 'Brands Library') {
                            breadcrumbs.prepend(arrowR, sessionStorage.LibraryName);
                            headerData = ['#', 'Brand Name', 'Updated By', 'Last Updated', 'Status'];
                            datasort = ['int', 'string', 'string', 'date', 'string'];
                            DisplayTableLibrary(headerData, datasort);
                        }

                        else if (sessionStorage.LibraryName == 'Product Categories') {
                            breadcrumbs.prepend(arrowR, sessionStorage.LibraryName);
                            headerData = ['#', 'Category Name', 'Updated By', 'Last Updated', 'Status'];
                            datasort = ['int', 'string', 'string', 'date', 'string'];
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
                            headerData = ['#', 'Name', 'Vendor Code', 'Status', 'Banned', 'Last Modified By', 'Last Modified'];
                            datasort = ['int', 'string', 'string', 'string', 'string', 'date'];
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
                            headerData = ['#', 'Group Modifier Name',, 'Modifier Type', 'Status', 'Modified By', 'Modified Date'];
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
                            headerData = ['#', 'Template', 'Locations', 'Modified By', 'Last Modified'];
                            datasort = ['int', 'string', , 'string', 'string', 'date'];
                            DisplayTableLibrary(headerData, datasort);
                        }

                        else if (sessionStorage.LibraryName == 'Payment Terms') {
                            breadcrumbs.prepend(arrowR, sessionStorage.LibraryName);
                            headerData = ['#', 'Term Name', 'Description', 'Term', 'Active', 'Modified By', 'Last Modified Date'];
                            datasort = ['int', 'string', 'string', 'string', 'string', 'string', 'date'];
                            DisplayTableLibrary(headerData, datasort);
                        }

                        else if (sessionStorage.LibraryName == 'Price Levels') {
                            breadcrumbs.prepend(arrowR, sessionStorage.LibraryName);
                            headerData = ['#', 'Price Level Name', 'Active', 'Modified By', 'Last Modified Date'];
                            datasort = ['int', 'string', 'string', 'string', 'date'];
                            DisplayTableLibrary(headerData, datasort);
                        }

                        else if (sessionStorage.LibraryName == 'Online Store Categories') {
                            breadcrumbs.prepend(arrowR, sessionStorage.LibraryName);
                            headerData = ['#', 'Name', 'Is Default Template?', 'Last Modified By', 'Last Updated'];
                            datasort = ['int', 'string', 'string', 'string', 'date'];
                            DisplayTableLibrary(headerData, datasort);
                        }
                        else if (sessionStorage.LibraryName == 'Revenue Centers') {
                            breadcrumbs.prepend(arrowR, sessionStorage.LibraryName);
                            headerData = ['#', 'Name', 'Description', 'Date Created', 'Modified Date', 'Last Modified By'];
                            datasort = ['int', 'string', 'string', 'date', 'date', 'string'];
                            DisplayTableLibrary(headerData, datasort);
                        }
                        else if (sessionStorage.LibraryName == 'Cost Centers') {
                            breadcrumbs.prepend(arrowR, sessionStorage.LibraryName);
                            headerData = ['#', 'Name', 'Description', 'Date Created', 'Modified Date', 'Last Modified By'];
                            datasort = ['int', 'string', 'string', 'date', 'date', 'string'];
                            DisplayTableLibrary(headerData, datasort);
                        }
                        
                        ResizeHeightOfRequiredData();
                    }
                    else {
                        //$('.ItemsBreadCrumbs').text('');
                        //breadcrumbs.prepend(arrowR, sessionStorage.LibraryName);
                        AddBreadCrumb({
                            text: sessionStorage.LibraryName, Click: function () {
                                window.location.hash = 'LibraryPage';
                                $('#btnMenuProductInformation').click();
                                if (typeof canvasArea != "undefined") {
                                    //for PIM picture cropping
                                    canvasArea.remove()
                                }

                                if (headerData.length != undefined) {
                                    //DisplayTableLibrary(headerData, datasort);
                                }
                            }
                        }, '.ItemsBreadCrumbs');
                        $('#divSystemAdDashboard, #UniversalDiv, #divSubList, #divForPIM').hide();
                        $('#UniversalDiv').empty();
                        $('#divSecond').show();
                    }

                    //$('.BreadCrumbs').html(breadcrumbs.prepend(lnkSysAdd));
                    //BreadCrumbsClickEvent();
                    $('body').animate({ scrollTop: 0 });
                }

                else if (window.location.hash == "#UniversalPage") {
                    //$('.ItemsBreadCrumbs').text('');

                    if (sessionStorage.AddViewLibBCName != undefined && sessionStorage.SubListName == undefined) {
                        //breadcrumbs.prepend(arrowR, sessionStorage.AddViewLibBCName);
                        //breadcrumbs.prepend(arrowR, '<a data-role="none" id="btnLibraryPage">' + sessionStorage.LibraryName + '</a>');
                        AddBreadCrumb({
                            text: sessionStorage.AddViewLibBCName, Click: function () {
                            }
                        }, '.ItemsBreadCrumbs')
                    }
                    else if (sessionStorage.SubListName != undefined && sessionStorage.SecondSubListName == undefined && sessionStorage.AddViewLibBCName == undefined) {
                        if (sessionStorage.SubListName != "Upload / Download Product List") {
                            breadcrumbs.prepend(arrowR, sessionStorage.SubListName);
                            breadcrumbs.prepend(arrowR, '<a data-role="none" id="btnSublistPage">' + sessionStorage.LibraryName + '</a>');
                        }
                    }
                    else if (sessionStorage.SubListName == undefined && sessionStorage.SecondSubListName == undefined && sessionStorage.AddViewLibBCName == undefined) {
                        //breadcrumbs.prepend(arrowR, sessionStorage.LibraryName);
                        AddBreadCrumb({
                            text: sessionStorage.LibraryName, Click: function () {
                                if (sessionStorage.LibraryName == 'Customer / Client List') {
                                    $('#pageCustomerMainDashboard').pageshow();
                                } else {
                                    GoToThisPage();
                                }
                            }
                        }, '.ItemsBreadCrumbs')
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

                    //$('.BreadCrumbs').html(breadcrumbs.prepend(lnkSysAdd));
                    //BreadCrumbsClickEvent();
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

            var tempinterval = setInterval(function () {
                if (typeof AddBreadCrumb != 'undefined') {
                    AddBreadCrumb({
                        text: 'System Administration', Click: function () {
                            window.location.hash = 'SystemAdministration';
                        }
                    }, '.ItemsBreadCrumbs')
                    clearInterval(tempinterval)
                }
            }, 100)

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

            $('#popupbtnZapatero').click(function () {
                $('#panExternalServices').modal('hide');
                location = 'http://localhost:7070/Login' +
                            '?Source=' + location.hostname +
                            '&Username=' + localStorage.Username +
                            '&Password=' + encodeURIComponent(localStorage.HashedPassword);
            })

            $('.btnHome').click(function () {
                if (sessionStorage.system == 'eas') {
                    location = '/erp?OrgID=' + sessionStorage.OrgID;
                } else {
                    window.location = '/People/PeopleMenu';
                    sessionStorage.removeItem("OrgName");
                }                
            })

            $('.btnMoreOption').click(function () {
                $('#panOption').modal('show');
            })

            $('#btnSetupOrganization, #popupbtnOrganization').click(function () {
                NewMessageBoxClose();
                window.location.hash = '';
                ShowSumOrgInfo();
            })

            $('#btnSetupOrganizationFOWST, #popupbtnFowst').click(function () {
                NewMessageBoxClose();
                window.location.hash = '';
                ShowOrgPlaces('FOWST');
            })

            $('#btnSetupDevice, #popupbtnAuthDevices').click(function () {
                AjaxSend({
                    url: 'Login3/TestCookies',
                    success: function (vData) {
                        NewPromptUserAccess('Devices', {
                            OnClickOKSuccess: function () {
                                NewMessageBoxClose();
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
                        $('#panOrgEmpMessage').modal('show');
                        $('#btnOKOrgEmp').text('Ok').click(function (e) {
                            e.preventDefault();
                            $('#panOrgEmpMessage').modal('hide');
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
                        NewPromptUserAccess('POS Receipt Configuration', { // old was: POS Receipt Settings
                            OnClickOKSuccess: function () {
                                NewMessageBoxClose();
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
                        $('#panOrgEmpMessage').modal('show');
                        $('#btnOKOrgEmp').text('Ok').click(function (e) {
                            e.preventDefault();
                            $('#panOrgEmpMessage').modal('hide');
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
                                NewMessageBoxClose();
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
                        $('#panOrgEmpMessage').modal('show');
                        $('#btnOKOrgEmp').text('Ok').click(function (e) {
                            e.preventDefault();
                            $('#panOrgEmpMessage').modal('hide');
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
                        $('#panOrgEmpMessage').modal('show');
                        $('#btnOKOrgEmp').text('Ok').click(function (e) {
                            e.preventDefault();
                            $('#panOrgEmpMessage').modal('hide');
                        })
                        $('#divBtnCancel').hide();
                        return;
                    }
                });
            })

            $('#btnSetupProducts2').click(function () {
                var ismob = CBool(IsMobile) ? 1 : 0;
                window.open('/People/PeopleMenu#pagePopUpMenuProducts', '_blank');
                NewMessageBoxClose();
            })

            $('#btnSetupProducts, #popupbtnProducts').click(function () {
                NewMessageBoxClose();
                window.location.hash = '';
                window.location.hash = "LibraryPage";
                headerData = ['#', 'Short Description', 'Product No.', 'Retail Price', 'Brand', 'Category', 'Activation Date', 'Last Modified Date', 'Status'];
                datasort = ['int', 'string', 'string', 'int', 'string', 'string', 'date'];
                sessionStorage["LibraryName"] = 'PIM';
                sessionStorage["LabelLibraryName"] = 'Product';
                sessionStorage["AddViewLibBCName"] = "Add Product";
                DisplayTableLibrary(headerData, datasort);
            })

            $('#btnPrinter, #popupbtnPrinter').click(function () {
                NewMessageBoxClose();
                window.location.hash = '';
                window.location.hash = "LibraryPage";
                headerData = ['#', 'Printer Brand', 'Printer Model', 'Printer Name', 'Serial Number', 'Interface Type', 'IP Address', 'FOWST', 'List of devices that use this as default printer', 'Role', 'Categories'];
                datasort = ['int', 'string', 'string', 'date'];
                sessionStorage["LibraryName"] = 'Printers';
                sessionStorage["LabelLibraryName"] = 'Printer';
                sessionStorage["AddViewLibBCName"] = "Add Printer";
                DisplayTableLibrary(headerData, datasort);
            })

            $('#btnSetupProductBrands, #popupbtnProductBrands').click(function () {
                NewMessageBoxClose();

                AjaxSend({
                    url: 'Organization/SavePimBrandsToBrandsLibrary',
                    success: function (e) {
                        if (e.Results == 'saved') {
                            window.location.hash = '';
                            window.location.hash = "LibraryPage";
                            headerData = ['#', 'Brand Name', 'Updated By', 'Last Updated', 'Status'];
                            datasort = ['int', 'string', 'string', 'date', 'string'];
                            sessionStorage["LibraryName"] = 'Brands Library';
                            sessionStorage["LabelLibraryName"] = 'Brand';
                            sessionStorage["AddViewLibBCName"] = "Add Brand";
                            DisplayTableLibrary(headerData, datasort);
                        } else {
                            NewMessageBox(e.Results, 'Message');
                        }
                    }
                })
            })

            $('#btnSetupProductCategories, #popupbtnProductCategories').click(function () {
                NewMessageBoxClose();

                AjaxSend({
                    url: 'Organization/SavePimCategoriesToCategoriesLibrary',
                    success: function (e) {
                        if (e.Results == 'saved') {
                            window.location.hash = '';
                            window.location.hash = "LibraryPage";
                            headerData = ['#', 'Category Name', 'Updated By', 'Last Updated', 'Status'];
                            datasort = ['int', 'string', 'String', 'date', 'string'];
                            sessionStorage["LibraryName"] = 'Product Categories';
                            sessionStorage["LabelLibraryName"] = 'Product Category';
                            sessionStorage["AddViewLibBCName"] = "Add Product Category";
                            DisplayTableLibrary(headerData, datasort);
                        } else {
                            NewMessageBox(e.Results, 'Message');
                        }
                    }
                })                
            })
            
            $('#btnGPSSettings, #popupbtnGPSSettings').click(function () {
                AjaxSend({
                    url: 'Login3/TestCookies',
                    success: function (vData) {
                        window.pageSecuritySourcePage = $.mobile.activePage;
                        $('.ui-dialog').dialog('close');
                        $('#panOption').modal('hide');
                        window.location.hash = "UniversalPage";
                        sessionStorage["LibraryName"] = 'GPS Settings';
                        GoToThisPage('GPS Settings');
                        //NewPromptUserAccess('GPS Settings', {
                        //    OnClickOKSuccess: function () {
                        //        $('.ui-dialog').dialog('close');
                        //        $('#panOption').modal('hide');
                        //        window.location.hash = "UniversalPage";
                        //        sessionStorage["LibraryName"] = 'GPS Settings';
                        //        GoToThisPage('GPS Settings');
                        //    }
                        //});
                    },
                    error: function (vError) {
                        $('#TitleOrgEmp').text('Requires Internet Connection');
                        $('#MsgOrgEmp').text('This feature requires internet connection.');
                        $('#panOrgEmpMessage').modal('show');
                        $('#btnOKOrgEmp').text('Ok').click(function (e) {
                            e.preventDefault();
                            $('#panOrgEmpMessage').modal('hide');
                        })
                        $('#divBtnCancel').hide();
                        return;
                    }

                });
            })

            $('#butPrintSettings, #popupbtnPrintSettings').click(function () {
                NewMessageBoxClose();
                window.location.hash = '';
                sessionStorage["LibraryName"] = 'Print Settings';
                GoToThisPage();
            })

            $('#btnSalesReturnSettings, #popupbtnSalesReturn').click(function () {
                AjaxSend({
                    url: 'Login3/TestCookies',
                    success: function (vData) {
                        window.pageSecuritySourcePage = $.mobile.activePage;
                        NewPromptUserAccess('Sales Return Settings', {
                            OnClickOKSuccess: function () {
                                NewMessageBoxClose();
                                window.location.hash = '';
                                sessionStorage["LibraryName"] = 'Sales Returns Settings';
                                GoToThisPage('Sales Returns Settings');
                            }
                        });
                    },
                    error: function (vError) {
                        $('#TitleOrgEmp').text('Requires Internet Connection');
                        $('#MsgOrgEmp').text('This feature requires internet connection.');
                        $('#panOrgEmpMessage').modal('show');
                        $('#btnOKOrgEmp').text('Ok').click(function (e) {
                            e.preventDefault();
                            $('#panOrgEmpMessage').modal('hide');
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
                                NewMessageBoxClose();
                                window.location.hash = "UniversalPage";
                                sessionStorage["LibraryName"] = 'Discount Settings';
                                GoToThisPage('Discount Settings');
                            }
                        });
                    },
                    error: function (vError) {
                        $('#TitleOrgEmp').text('Requires Internet Connection');
                        $('#MsgOrgEmp').text('This feature requires internet connection.');
                        $('#panOrgEmpMessage').modal('show');
                        $('#btnOKOrgEmp').text('Ok').click(function (e) {
                            e.preventDefault();
                            $('#panOrgEmpMessage').modal('hide');
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
                                NewMessageBoxClose();
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
                        $('#panOrgEmpMessage').modal('show');
                        $('#btnOKOrgEmp').text('Ok').click(function (e) {
                            e.preventDefault();
                            $('#panOrgEmpMessage').modal('hide');
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
                NewMessageBoxClose();
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

            $('#divPopPassword button[name=ok]').click(function() {
                AjaxSend({
                    url: '/Login3/VerifyPassword',
                    data: {
                        Password: $('#divPopPassword input[type=password]').val()
                    },
                    success: function(vData) {
                        if (vData.Error)
                            NewMessageBox(vData.Error);
                        else if (vData.Result == 'ok')
                            RePrompt();
                        else
                            NewMessageBox('Unknown error');
                    }
                });
                function DeactivateAllProducts() {
                    AjaxSend({
                        url: '/PIMProduct/DeactivateAllProducts',
                        success: function(vData) {
                            if (vData.Error)
                                NewMessageBox(vData.Error)
                            else if (vData.AffectedCount == '0')
                                NewMessageBox('All products were already deactivated.');
                            else if (vData.AffectedCount)
                                LibraryTable.PIMPage();
                                NewMessageBox('Number of products that were deactivated by this action: ' + vData.AffectedCount + '.');
                        }
                    });
                }
                function RePrompt() {
                    NewMessageBox('Are you really sure you want to deactivate all products?', null, [
                        {
                            Text: 'Yes',
                            Click: function() {
                                DeactivateAllProducts();
                            }
                        },
                        {
                            Text: 'No'
                        }
                    ]);
                }
            });
             
            $('#divPopPassword button[name=close]').click(function() {
                $('#divPopPassword').modal('hide');
            });

            $('#btnActivateAllSku').click(function () {
                PopupButtonList('', 'Re-activate Products', [
                        {
                            Text: 'Re-activate all products',
                            Click: function () {
                                ReactivateAllProducts();
                            }
                        },
                        {
                            Text: 'Re-activate using PIM Template',
                            Click: function () {
                                $('#divPopReActivate').modal('show');
                            }
                        }
                ]);
            })

            $('#lnkDownloadReActivationTemplate').click(function (e) {
                e.preventDefault();
                window.location = GetPathFormat("/PIMTemplate/DownloadReActivationTemplate");
            })

            $('#btnDeactivateAllSku').click(function () {
                function PromptPassword() {
                    $('#divPopPassword input[type=password]').val('');
                    $('#divPopPassword').modal('show');
                }
                function PromptAccess() {
                    NewPromptUserAccess('Group Settings', {
                        OnClickOKSuccess: function () {
                            PromptPassword();
                        }
                    });
                }
                NewMessageBox('Are you sure you want to deactivate all products?', null, [
                    {
                        Text: 'Yes',
                        Click: function() {
                            PromptAccess();
                        }
                    },
                    {
                        Text: 'No'
                    }
                ]);
            });
            $('#btnAdjustProdPrices').click(function () {
                $('#divAddProductPopup').modal('hide');
                $('#divAdjustProdPricesPopup').modal('show');
            })

            $('#btnBackToOption').click(function () {
                $('#divAdjustProdPricesPopup').modal('hide');
                $('#divAddProductPopup').modal('show');
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
                        $('#panOrgEmpMessage').modal('show');
                        $('#btnOKOrgEmp').text('Ok').click(function (e) {
                            e.preventDefault();
                            $('#panOrgEmpMessage').modal('hide');
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
                IsAddLibraryDisplayInUnivDiv = false;
                window.location.hash = "UniversalPage";
                sessionStorage["AddViewLibBCName"] = 'Import Products by Excel File';
                //LibraryTable.ImportByExcelFilePage();
            })

            $('#btnPIMWebForm').click(function () {
                IsAddLibraryDisplayInUnivDiv = true;
                window.location.hash = "UniversalPage";
                sessionStorage["AddViewLibBCName"] = 'PIM Web Form';
                LibraryTable.PIMWebForm();
            })

            $('#btnPimPriceAdjust').click(function () {
                IsAddLibraryDisplayInUnivDiv = true;
                window.location.hash = "UniversalPage";
                sessionStorage["AddViewLibBCName"] = 'PIM Price Adjustment';
                LibraryTable.PIMPriceAdjustment();
            })
            
            $('#butChangeProductAreas').click(function () {
                IsAddLibraryDisplayInUnivDiv = true;
                window.location.hash = "UniversalPage";
                sessionStorage["AddViewLibBCName"] = 'Change Product Areas Or Sections';
                LibraryTable.ChangeProductAreas();
            })

            $('#butDeduplicateProductNumbers').click(function () {
                setTimeout(function () {
                    AjaxSend({
                        url: 'Stock/DuplicateProductNumberCount',
                        success: function (vData) {
                            if (vData.Error)
                                NewMessageBox(vData.Error, 'Error');
                            else
                                NewMessageBox('We found ' + vData.Result + ' Product Numbers that are duplicated at least once.<br /><br />' +
                                    'This is an issue since uploading product information via Excel file relies on unique product numbers to identify, edit or add specific SKUs.<br /><br />' +
                                    'In case you choose to deduplicate, Mobiz will add unique counting suffixes to the duplicated product numbers. All of the SKUs’ transactional histories will be preserved despite the product number change since Mobiz saves GUID / ROW IDs instead of product numbers in all transactional data tables.<br /><br />' +
                                    'Would you like to continue with deduplication of Product Numbers?', 'PRODUCT NUMBER DEDUPLICATION',
                                    [
                                        {
                                            Text: 'Yes',
                                            Click: function () {
                                                AjaxSend({
                                                    url: 'Stock/DeduplicateProductNumberCount',
                                                    success: function (vData) {
                                                        if (vData.Error)
                                                            NewMessageBox(vData.Error, 'Error');
                                                        else
                                                            NewMessageBox('Product numbers that were deduplicated: ' + vData.Result, 'Success');
                                                    },
                                                    error: function (vError) {
                                                        NewMessageBox(vError, 'Server-Side or Connection Error');
                                                    }
                                                });
                                            }
                                        },
                                        {
                                            Text: 'Download Product List With Duplicate Product Number',
                                            Click: function () {
                                                var pUrl = MergeUrlParameters({
                                                    OrgID: sessionStorage.OrgID
                                                }, '/Stock/DownloadDuplicateProductNumbers');
                                                downloadURI(pUrl);
                                                NewMessageBox('You should be downloading the file by now. If not, please check your web browser\'s popup settings.');

                                                function downloadURI(uri, name) {
                                                    // https://stackoverflow.com/a/23013574/334045
                                                    var link = document.createElement("a");
                                                    // If you don't know the name or want to use
                                                    // the webserver default set name = ''
                                                    link.setAttribute('download', name);
                                                    link.href = uri;
                                                    document.body.appendChild(link);
                                                    link.click();
                                                    link.remove();
                                                }
                                            }
                                        },
                                        {
                                            Text: 'No'
                                        }
                                    ]
                                ); // END NewMessageBox('We found ' + vData.Result + ' Product Numbers that are duplicated at least once.<br /><br />' +
                        },
                        error: function (vError) {
                            NewMessageBox(vError, 'Server-Side or Connection Error');
                        }
                    }); // END AjaxSend({
                }, 400)                
            }); // END $('#butDeduplicateProductNumbers').click(function () {

            $('#butPimMergeBrands').click(function () {
                $('#divAddProductPopup').modal('hide');
                require(['promise!/Scripts/sources/ERP/BrandMerger/controller.js'],
                    function() {
                        new ERP.BrandMerger.controller().Show(); 
                    }
                );
            });

            $('#btnDownloadCompanyProductList').click(function (e) {
                e.preventDefault();
                window.location = GetPathFormat("/Organization/DownloadCompanyProductList?OrgID=" + sessionStorage["OrgID"]);
            })

            $('#btnQuantitySetting, #popupbtnQuantitySettings').click(function () {
                NewMessageBoxClose();
                window.location.hash = '';
                sessionStorage["LibraryName"] = 'Decimals in Quantity Settings';
                GoToThisPage();
            })

            $('#btnPOSSetting, #popupbtnPOSSettings').click(function () {
                NewMessageBoxClose();
                window.location.hash = '';
                sessionStorage["LibraryName"] = 'POS Settings';
                GoToThisPage();
            })

            $('#btnSalesOrderSetting, #popupbtnSalesOrder').click(function () {
                NewMessageBoxClose();
                window.location.hash = '';
                sessionStorage["LibraryName"] = 'Sales Order Settings';
                GoToThisPage();
            })

            $('#butUpdateServerIp').click(function () {
                NewMessageBoxClose();
                window.location.hash = '';
                sessionStorage["LibraryName"] = 'Update Server IP';
                GoToThisPage();
            });

            $('#btnReceivingAndTransferOutSetting, #popupbtnReceivingAndTransferOut').click(function () {
                NewMessageBoxClose();
                window.location.hash = '';
                sessionStorage["LibraryName"] = 'Receiving And Transfer-Out Settings';
                GoToThisPage();
            })

            $('#btnInventoryCountSetting, #popupbtnInventoryCount').click(function () {
                NewMessageBoxClose();
                window.location.hash = '';
                sessionStorage["LibraryName"] = 'Inventory Count Settings';
                GoToThisPage();
            })

            $('#btnDailyBeginningBalanceSetting, #popupbtnDailyBeginningBalance').click(function () {
                NewMessageBoxClose();
                window.location.hash = '';
                sessionStorage["LibraryName"] = 'Daily Beginning Balance Settings';
                GoToThisPage();
            })

            $('#butPurchaseSettings, #btnPurchaseOrderSetting, #popupbtnPurchaseOrderSettings').click(function () {
                NewMessageBoxClose();
                window.location.hash = '';
                sessionStorage["LibraryName"] = 'Purchase Order Settings';
                GoToThisPage();
            });

            $('#butProductMakerSettings').click(function () {
                NewMessageBoxClose();
                window.location.hash = '';
                sessionStorage["LibraryName"] = 'Product Maker Settings';
                GoToThisPage();
            });

            $('#btnGPSSettings').click(function () {
                $('#panOption').modal('hide');
                window.location.hash = '';
                sessionStorage["LibraryName"] = 'GPS Settings';
                GoToThisPage();
            });

            $('#butErpSettings').click(function () {
                NewMessageBoxClose();
                window.location.hash = '';
                sessionStorage["LibraryName"] = 'ERP Settings';
                GoToThisPage();
            })

            $('#popupbtnLoyaltyProgram').click(function () {
                NewMessageBoxClose();
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
                                NewMessageBoxClose();
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
                        $('#panOrgEmpMessage').modal('show');
                        $('#btnOKOrgEmp').text('Ok').click(function (e) {
                            e.preventDefault();
                            $('#panOrgEmpMessage').modal('hide');
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
                                NewMessageBoxClose();
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
                        $('#panOrgEmpMessage').modal('show');
                        $('#btnOKOrgEmp').text('Ok').click(function (e) {
                            e.preventDefault();
                            $('#panOrgEmpMessage').modal('hide');
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
                                NewMessageBoxClose();
                                window.location.hash = '';
                                window.location.hash = 'UniversalPage';
                                sessionStorage["LibraryName"] = 'Customer / Client List';

                                return;
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
                        $('#panOrgEmpMessage').modal('show');
                        $('#btnOKOrgEmp').text('Ok').click(function (e) {
                            e.preventDefault();
                            $('#panOrgEmpMessage').modal('hide');
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
                                NewMessageBoxClose();
                                window.location.hash = '';
                                window.location.hash = "LibraryPage";
                                headerData = ['#', 'Name', 'Vendor Code', 'Status', 'Banned', 'Last Modified By', 'Last Modified'];
                                datasort = ['int', 'string', 'string', 'string', 'string', 'date'];
                                sessionStorage["LibraryName"] = 'Supplier / Vendor List';
                                sessionStorage["LabelLibraryName"] = 'Supplier';
                                DisplayTableLibrary(headerData, datasort);
                            }
                        })
                    },
                    error: function (vError) {
                        $('#TitleOrgEmp').text('Requires Internet Connection');
                        $('#MsgOrgEmp').text('This feature requires internet connection.');
                        $('#panOrgEmpMessage').modal('show');
                        $('#btnOKOrgEmp').text('Ok').click(function (e) {
                            e.preventDefault();
                            $('#panOrgEmpMessage').modal('hide');
                        })
                        $('#divBtnCancel').hide();
                        return;
                    }
                });
            })

            $('#btnSalesTeam, #popupbtnSalesTeam').click(function () {
                NewMessageBoxClose();
                window.location.hash = '';
                window.location.hash = "LibraryPage";
                headerData = ['#', 'Name', 'Description', 'Last Modified Date'];
                datasort = ['int', 'string', 'string', 'date'];
                sessionStorage["LibraryName"] = 'Sales Team';
                sessionStorage["LabelLibraryName"] = 'Team';
                DisplayTableLibrary(headerData, datasort);
            })

            $('#btnItemModifiers, #popupbtnItemModifiers').click(function () {
                NewMessageBoxClose();
                window.location.hash = '';
                window.location.hash = "LibraryPage";
                headerData = ['#', 'Group Modifier Name', 'Modifier Type', 'Status', 'Modified By', 'Modified Date'];
                datasort = ['int', 'string', 'string', 'string', 'date'];
                sessionStorage["LibraryName"] = 'Item Modifiers';
                sessionStorage["LabelLibraryName"] = 'Modifier';
                DisplayTableLibrary(headerData, datasort);
                $('#tblBody').empty();
            })

            $('#btnPaymentTerms, #popupbtnPaymentTerms').click(function () {
                NewMessageBoxClose();
                window.location.hash = '';
                window.location.hash = "LibraryPage";
                headerData = ['#', 'Term Name', 'Description', 'Term', 'Active', 'Modified By', 'Last Modified Date'];
                datasort = ['int', 'string', 'string', 'string', 'string', 'string', 'date'];
                sessionStorage["LibraryName"] = 'Payment Terms';
                sessionStorage["LabelLibraryName"] = 'Payment Term';
                DisplayTableLibrary(headerData, datasort);
            })

            $('#btnAccountAdjustments, #popupbtnAccountAdjustments').click(function () {
                NewMessageBoxClose();
                window.location.hash = '';
                sessionStorage["LibraryName"] = 'Account Adjustments';
                GoToThisPage();
            })

            $('#btnPriceLevel, #popupbtnPriceLevel').click(function () {
                NewMessageBoxClose();
                window.location.hash = '';
                window.location.hash = "LibraryPage";
                headerData = ['#', 'Price Level Name', 'Active', 'Modified By', 'Last Modified Date'];
                datasort = ['int', 'string', 'string', 'string', 'date'];
                sessionStorage["LibraryName"] = 'Price Levels';
                sessionStorage["LabelLibraryName"] = 'Price Levels';
                DisplayTableLibrary(headerData, datasort);
            });

            $('#butSysAdminSalesRep').click(function () {
                NewMessageBoxClose();
                window.location.hash = '';
                window.location.hash = "LibraryPage";
                headerData = ['#', 'Sales Rep Name', 'Active', 'Modified By', 'Last Modified Date'];
                datasort = ['int', 'string', 'string', 'string', 'date'];
                sessionStorage["LibraryName"] = 'Sales Representatives';
                sessionStorage["LabelLibraryName"] = 'Sales Representatives';
                DisplayTableLibrary(headerData, datasort);
            })

            $('#btnProprietaryBarcodeTypes, #popupbtnProprietaryBarcodeTypes').click(function () {
                NewMessageBoxClose();
                window.location.hash = '';
                window.location.hash = "LibraryPage";
                headerData = ['#', 'Barcode Type', 'Sticker Dimensions', 'Last Modified Date'];
                datasort = ['int', 'string', 'string', 'date'];
                sessionStorage["LibraryName"] = 'Proprietary Barcode Type';
                sessionStorage["LabelLibraryName"] = 'Barcode Type';
                DisplayTableLibrary(headerData, datasort);
            })
            
            $('#btnExternalServices').click(function () {
                $('#panExternalServices').modal('show');                
            })

            $('#btnTableViewSetting, #popupbtnTableView').click(function () {
                NewMessageBoxClose();
                window.location.hash = '';
                window.location.hash = "LibraryPage";
                headerData = ['#', 'Template Name', 'Locations', 'Modified By', 'Last Modified'];
                datasort = ['int', 'string', , 'string', 'string', 'date'];
                sessionStorage["LibraryName"] = 'Table View Settings';
                sessionStorage["LabelLibraryName"] = 'Template';
                DisplayTableLibrary(headerData, datasort);
                $('#tblBody').empty();
            })

            $('#btnEcommerceGeneralSettings, #popupbtnbtnEcommerceGeneralSettings').click(function () {
                NewMessageBoxClose();
                window.location.hash = '';
                sessionStorage["LibraryName"] = 'Ecommerce General Settings';
                GoToThisPage();
            })

            $('#butSystemAdministratorReportsFlushingReport').click(function () {
                NewMessageBoxClose();
                window.location.hash = '';
                sessionStorage["LibraryName"] = 'Flushing Report';
                GoToThisPage();
            })

            $('#btnEcommerceVendorPortalSettings, #popupbtnbtnEcommerceVendorPortalSettings').click(function () {
                NewMessageBoxClose();
                window.location.hash = '';
                sessionStorage["LibraryName"] = 'Vendor Portal Settings';
                GoToThisPage();
            })

            $('#btnEcommerceBannerSettings, #popupbtnbtnEcommerceBannerSettings').click(function () {
                NewMessageBoxClose();
                window.location.hash = '';
                sessionStorage["LibraryName"] = 'Banner Settings';
                GoToThisPage();
            })

            $('#btnOnlineStoreCategories, #popupbtnOnlineStoreCategories').click(function (e) {
                e.preventDefault();

                AjaxSend({
                    url: 'Login3/TestCookies',
                    success: function (vData) {
                        window.pageSecuritySourcePage = $.mobile.activePage;
                        //NewPromptUserAccess('Hot Buttons Settings', {
                        //    OnClickOKSuccess: function () {
                                
                        //    }
                        //});
                        $('.ui-dialog').dialog('close');
                        NewMessageBoxClose();
                        window.location.hash = "LibraryPage";
                        headerData = ['#', 'Name', 'Is Default Template?', 'Last Modified By', 'Last Modified Date'];
                        datasort = ['int', 'string', 'string', 'string', 'date'];
                        sessionStorage["LibraryName"] = 'Online Store Categories';
                        sessionStorage["LabelLibraryName"] = 'Store Category';
                        DisplayTableLibrary(headerData, datasort);
                    },
                    error: function (vError) {
                        NewMessageBox('This feature requires internet connection.', 'Requires Internet Connection');
                    }
                });
            })

            $('#btnRevenueCenters, #popupbtnRevenueCenters').click(function (e) {
                e.preventDefault();

                AjaxSend({
                    url: 'Login3/TestCookies',
                    success: function (vData) {
                        window.pageSecuritySourcePage = $.mobile.activePage;
                        //NewPromptUserAccess('Hot Buttons Settings', {
                        //    OnClickOKSuccess: function () {

                        //    }
                        //});
                        $('.ui-dialog').dialog('close');
                        NewMessageBoxClose();
                        window.location.hash = "LibraryPage";
                        headerData = ['#', 'Name', 'Description', 'Date Created', 'Modified Date', 'Last Modified By'];
                        datasort = ['int', 'string', 'string', 'date', 'date', 'string'];
                        sessionStorage["LibraryName"] = 'Revenue Centers';
                        sessionStorage["LabelLibraryName"] = 'Revenue Center';
                        DisplayTableLibrary(headerData, datasort);
                    },
                    error: function (vError) {
                        NewMessageBox('This feature requires internet connection.', 'Requires Internet Connection');
                    }
                });
            })

            $('#btnCostCenters, #popupbtnCostCenters').click(function (e) {
                e.preventDefault();

                AjaxSend({
                    url: 'Login3/TestCookies',
                    success: function (vData) {
                        AjaxSend({
                            url: 'Expenses/GenerateDefaultCostCenter',
                            success: function (e) {
                                if (e.Error != undefined) {
                                    NewMessageBox(e.Error, 'Error');
                                }

                                window.pageSecuritySourcePage = $.mobile.activePage;
                                //NewPromptUserAccess('Hot Buttons Settings', {
                                //    OnClickOKSuccess: function () {

                                //    }
                                //});
                                $('.ui-dialog').dialog('close');
                                NewMessageBoxClose();
                                window.location.hash = "LibraryPage";
                                headerData = ['#', 'Name', 'Description', 'Date Created', 'Modified Date', 'Last Modified By'];
                                datasort = ['int', 'string', 'string', 'date', 'date', 'string'];
                                sessionStorage["LibraryName"] = 'Cost Centers';
                                sessionStorage["LabelLibraryName"] = 'Cost Center';
                                DisplayTableLibrary(headerData, datasort);
                            }
                        })                        
                    },
                    error: function (vError) {
                        NewMessageBox('This feature requires internet connection.', 'Requires Internet Connection');
                    }
                });
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
                    $('#divAddProductPopup').modal('show');
                }
                else if (sessionStorage.LibraryName == "Item Modifiers") {
                    //$('#popModifier').show();
                    window.location.hash = 'UniversalPage';
                    sessionStorage.removeItem("ItemModifierData");
                    LibraryTable.PageItemModifiers();
                    //$('#popupPOSModifier').show();
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
                } else if (sessionStorage.LibraryName == "Payment Terms") {
                    sessionStorage["AddViewLibBCName"] = "New Payment Terms";

                    LibraryTable.AddPaymentTerms();
                    window.location.hash = 'UniversalPage';
                    // InitVal();
                } else if (sessionStorage.LibraryName == "Brands Library") {
                    $('.lblPIMLib').text('Brand');
                    $('#popupPIMLib').modal('show');
                    $('#txtPIMLibID').val('');
                    $('#txtPIMLib').removeAttr('currentname').val('').focus();
                } else if (sessionStorage.LibraryName == "Product Categories") {
                    $('.lblPIMLib').text('Category');
                    $('#popupPIMLib').modal('show');
                    $('#txtPIMLibID').val('');
                    $('#txtPIMLib').removeAttr('currentname').val('').focus();
                } else if (sessionStorage.LibraryName == "Price Levels") {
                    $('#panAddPriceLevel').modal('show');
                    $('#txtPriceLevelCustomID').val('');
                    $('#txtPriceLevelName').val('').focus();
                    $('[name="rad-pricelevel"][value="Price"]').prop('checked', 1);//.checkboxradio('refresh');
                    $('#selPriceLevelActive').prop('checked', 1);//.slider('refresh');
                } else if (sessionStorage.LibraryName == "Sales Representatives") {
                    $('#panAddSalesRepresentative').modal('show');
                    $('#txtSalesRepresentativeCustomID').val('');
                    $('#txtSalesRepresentativeName').val('').focus();
                    $('#selSalesRepresentativeActive').val('1').slider('refresh');
                }
                //else if (sessionStorage.LibraryName == "Supplier / Vendor List") {
                //    sessionStorage["AddViewLibBCName"] = "New Supplier / Vendor";
                //    PopupButtonList('', 'Select Options', [
                //        {
                //            Text: 'Add Organization',
                //            Click: function () {                                
                //                window.location.hash = 'UniversalPage';
                //                sessionStorage.SupplierType = 'Org';
                //                LibraryTable.NewCustomerPage();
                //            }
                //        },
                //        {
                //            Text: 'Add Person',
                //            Click: function () {
                //                window.location.hash = 'UniversalPage';
                //                sessionStorage.SupplierType = 'Person';
                //                LibraryTable.NewCustomerPage();
                //            }
                //        },
                //        {
                //            Text: 'Upload Suppliers Using Excel Template',
                //            Click: function () {
                //                $('#SupplierobjFile').trigger('click');
                //            }
                //        },
                //        {
                //            Text: 'Download Suppliers Excel Template',
                //            Click: function () {
                //                window.location = GetPathFormat("/CRMDashboard/DownloadContactTemplate");
                //            }
                //        }
                //    ])
                //}
                else if (sessionStorage.LibraryName == "Proprietary Barcode Type") {
                    $('#panAddBarcodeType').modal('show');
                    $('#divRemoveBarcodeType').hide();
                    $('#txtBarcodeTypeName').val('').focus();
                    $('#txtBarcodeStickerDimensions, #txtBarcodeTypeID').val('');
                } else if (sessionStorage.LibraryName == "Revenue Centers") {
                    sessionStorage["AddViewLibBCName"] = "Revenue Center";

                    LibraryTable.CentersPage();
                    window.location.hash = 'UniversalPage';
                    // InitVal();
                } else if (sessionStorage.LibraryName == "Cost Centers") {
                    sessionStorage["AddViewLibBCName"] = "Cost Center";

                    LibraryTable.CentersPage();
                    window.location.hash = 'UniversalPage';
                    // InitVal();
                } else {
                    $('#panAddLibrary').modal('show');
                    $('#txtSearch').focus();
                }
            });

            $('#btnSavePIMLib').click(function () {
                var txtPIMLib = $.trim($('#txtPIMLib').val());
                var txtPIMLibID = $.trim($('#txtPIMLibID').val());
                var currentname = $('#txtPIMLib').attr('currentname');

                if (txtPIMLib == '') {
                    WebToast('Please enter name', 'top');
                    return;
                }

                function SavePIMLib() {
                    AjaxSend({
                        url: 'PIMProduct/SavePIMLib',
                        data: {
                            data: {
                                ID: txtPIMLibID,
                                type: sessionStorage.LibraryName,
                                name: txtPIMLib,
                                currentname: currentname
                            }
                        }, success: function (e) {
                            if (e.Results == 'saved') {
                                WebToast('Successfully saved', 'top');

                                if (sessionStorage.LibraryName == 'Brands Library') {
                                    LibraryTable.PIMBrands();
                                } else {
                                    LibraryTable.PIMCategories();
                                }

                                $('#popupPIMLib').modal('hide');
                                $('#popupPIMLibEditName').modal('hide');
                            } else {
                                NewMessageBox(e.Results, 'Error');
                            }
                        }
                    })
                }

                if (txtPIMLibID == '') {
                    SavePIMLib();
                } else {
                    if ($.trim($('#txtPIMLib').attr('currentname')) == txtPIMLib) {
                        WebToast('Please edit the name if you want to save', 'top');
                    } else {
                        NewMessageBox('Please wait while validating ...', 'Message');
                        AjaxSend({
                            url: 'PIMProduct/GetProducts',
                            data: {
                                data: {
                                    type: sessionStorage.LibraryName,
                                    currentname: currentname
                                }
                            }, success: function (e) {
                                var popupPIMLibEditName = $('#popupPIMLibEditName');
                                var tables = popupPIMLibEditName.find('table');
                                var tbody = popupPIMLibEditName.find('tbody');
                                var vRows = e.Results.Rows;

                                tbody.empty();

                                if (vRows.length > 0){
                                    for (var i = 0; i < vRows.length; i++) {
                                        tbody.append($('<tr>')
                                                .append($('<td style="width:50px;">').text(i + 1 + '.'))
                                                .append($('<td>').text(vRows[i].ProductNo))
                                                .append($('<td>').text(vRows[i].ProductName))
                                            )
                                    }
                                } else {
                                    tbody.append($('<tr>').append($('<td colspan="3">').text('No Products found for brand ' + currentname)))
                                }

                                $('#lblPIMLibOldName').text(currentname);
                                $('.lblPIMLibNewName').text(txtPIMLib);

                                $('#btnSavePIMLibEditName').unbind('click').click(function () {
                                    SavePIMLib();
                                })

                                if (tables.length > 1) {
                                    tables[1].remove(); // remove extra table, idk why naggenerate ug table
                                }
                                
                                NewMessageBoxClose();
                                popupPIMLibEditName.modal('show');
                            }
                        })
                    }                    
                }                
            })

            $('#btnSaveBarcodeType').click(function () {
                if ($.trim($('#txtBarcodeTypeName').val()) == '') {
                    WebToast('Please enter barcode type', 'top');
                    return;
                }

                $(this).text('Saving ...');
                $('#btnSaveBarcodeType, #btnRemoveBarcodeType').attr('disabled', true);

                AjaxSend({
                    url: 'Organization/SaveBarcodeType',
                    data: {
                        ID: $('#txtBarcodeTypeID').val(),
                        Type: $.trim($('#txtBarcodeTypeName').val()),
                        Dimensions: $.trim($('#txtBarcodeStickerDimensions').val()),
                        State: 'save'
                    },
                    success: function (e) {
                        $('#panAddBarcodeType').modal('hide');
                        $('#btnSaveBarcodeType').text('Save');
                        $('#btnSaveBarcodeType, #btnRemoveBarcodeType').removeAttr('disabled');

                        if(e.Results == 'saved'){
                            WebToast('Successfully saved', 'top');
                            LibraryTable.ProprietaryBarcodeType()
                        } else {
                            NewMessageBox(e.Results, 'Error');
                        }
                    }
                })
            })

            $('#btnRemoveBarcodeType').click(function () {
                NewMessageBox('Are you sure you want to remove this record?', 'Message', [
                    {
                        Text: 'Yes', Click: function () {
                            $('#btnRemoveBarcodeType').text('Removing ...');
                            $('#btnSaveBarcodeType, #btnRemoveBarcodeType').attr('disabled', true);
                            AjaxSend({
                                url: 'Organization/SaveBarcodeType',
                                data: { ID: $('#txtBarcodeTypeID').val(), State: 'remove' },
                                success: function (e) {
                                    $('#panAddBarcodeType').modal('hide');
                                    $('#btnRemoveBarcodeType').text('Remove');
                                    $('#btnSaveBarcodeType, #btnRemoveBarcodeType').removeAttr('disabled');

                                    if (e.Results == 'saved') {
                                        WebToast('Successfully deleted', 'top');
                                        LibraryTable.ProprietaryBarcodeType();
                                    } else {
                                        NewMessageBox(e.Results, 'Error');
                                    }
                                }
                            })
                        }
                    }, { Text: 'No' }
                ])                
            })

            $('#butSaveItemModifier').click(function (e) {
                LibraryTable.AddItemModifier();
            });

            $('#butCancelItemModifier').click(function (e) {
                $('#popItemModifierName').modal('hide');
                $('#txtItemModifierNewName').val('');
            });

            $('#btnCancelLibrary').click(function () {
                $('#panAddLibrary').modal('hide');
                ClearAddLibraryFieldsPopUp();
            })

            $('#btnPriceLevelSave').click(function () {
                SavePriceLevelCustom();
            })
            
            $('#butSalesRepresentativeSave').click(function () {
                SaveSalesRepresentativeCustom();
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
                else if (sessionStorage.LibraryName == "Online Store Categories") {
                    sessionStorage["AddViewLibBCName"] = "Add Store Category Template";
                }

                GoToThisPage($('#txtSearch').val());
            })
            $('#btnOKOrgEmp, #btnSaveAndSend').click(function (e) {
                e.preventDefault();
                $('#panOrgEmpMessage').modal('hide');
            })
            $('#btnCancelOrgEmp, #btnCancelEmail').click(function (e) {
                e.preventDefault();
                $('#panOrgEmpMessage').modal('hide');
            })

            $('#btnSaveKM').click(function () {
                SaveCase();
            });

            $('#btnCancelKM').click(function () {
                $('#panAddKnowledgeManagement').hide();
            });

            $('#btnOptionAddProduct').click(function () {
                if (sessionStorage.LibraryName == "Supplier / Vendor List") {
                    PopupButtonList('', 'Select Options', [
                        {
                            Text: 'Upload Suppliers Using Excel Template',
                            Click: function () {
                                $('#SupplierobjFile').trigger('click');
                            }
                        },
                        {
                            Text: 'Download Suppliers Excel Template',
                            Click: function () {
                                window.location = GetPathFormat("/CRMDashboard/DownloadContactTemplate");
                            }
                        },
                        {
                            Text: 'Deactivate all supplier records',
                            Click: function (e) {
                                ActionForCustomerRecords(true)
                            }
                        },
                        {
                            Text: 'Reactivate all supplier records',
                            Click: function (e) {
                                ActionForCustomerRecords(false);
                            }
                        },
                        {
                            Text: 'Merge duplicate suppliers...',
                            Click: function () {
                                PromptUserAccess('Merge Duplicate Suppliers', {
                                    OnClickOKSuccess: function () {
                                        require(['promise!/Scripts/sources/ERP/OrganizationMerger/controller.js'],
                                        function() {
                                           new ERP.OrganizationMerger.controller().ShowSuppliers(); 
                                        });
                                    }
                                });
                            }
                        }
                    ])
                } else {
                    $('#divAddProductTitle').text('Options');
                    $('#listForAdding').css({
                        visibility: 'hidden',
                        display: 'none'
                    });
                    $('#listForOption').css({
                        visibility: 'visible',
                        display: 'block'
                    });
                    $('#divAddProductPopup').modal('show');
                }                
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
                        $('#panOrgEmpMessage').modal('show');
                        $('#btnOKOrgEmp').text('Ok').click(function (e) {
                            e.preventDefault();
                            $('#panOrgEmpMessage').modal('hide');
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

            $('#selOptionStatus').change(function () {
                if(sessionStorage.LibraryName == 'Item Modifiers'){
                    LibraryTable.SearchItemModifiers($('.txtSearchdesktop').val() || $('.txtSearchmobile').val());
                } else if (sessionStorage.LibraryName == 'PIM') {
                    LibraryTable.PIMPage($('.txtSearchdesktop').val() || $('.txtSearchmobile').val());
                } else if (sessionStorage.LibraryName == 'FOWST') {
                    LibraryTable.Fowst();
                } else if (sessionStorage.LibraryName == 'Supplier / Vendor List') {
                    LibraryTable.SupplierList();
                } else if (sessionStorage.LibraryName == 'Revenue Centers' || sessionStorage.LibraryName == 'Cost Centers') {
                    LibraryTable.Centers();
                } else if (sessionStorage.LibraryName == 'Brands Library') {
                    LibraryTable.PIMBrands();
                } else if (sessionStorage.LibraryName == 'Product Categories') {
                    LibraryTable.PIMCategories();
                }
            })

            $('#Supplierformupload').submit(function (e) {
                e.preventDefault();
                var formData = new FormData($(this)[0]);
                NewMessageBox('This might take a few moments. Please wait while the system uploading your contacts ...', 'Message');
                setTimeout(function () {
                    $.ajax({
                        url: '/OrganizationShort/UploadContacts?OrgID=' + sessionStorage.OrgID,
                        type: 'POST',
                        data: formData,
                        async: false,
                        cache: false,
                        contentType: false,
                        processData: false,
                        success: function (vData) {
                            $('#SupplierobjFile').val('');
                            if (vData == "Saved") {
                                WebToast('Suppliers has been successfully uploaded', 'top');
                                LibraryTable.SupplierList();
                                NewMessageBoxClose();
                            }
                            else {
                                NewMessageBox(vData, 'ERROR');
                            }
                        }
                    });
                }, 1000)
            })

            $('#SupplierobjFile').change(function (e) {
                $('#txtSupplierUploadExcelOrgID').val(sessionStorage.OrgID)
                $('#Supplierformupload').submit();
            })

        })// End document

        function ActionForCustomerRecords(IsDeactivate) {
            NewMessageBox('Are you sure you want to ' + (IsDeactivate ? 'deactivate' : 'reactivate') + ' all supplier records?', 'Message', [
                {
                    Text: 'Yes',
                    Click: function () {
                        $('#popConfirmSupplierDeactivation').modal('show');
                        $('#txtSupplierDeactivatePassword').val('').focus()

                        $('#btnDeactivateSupplierNow').unbind('click').click(function () {
                            VerifyPassword()
                        })

                        $('#txtSupplierDeactivatePassword').unbind('keydown').keydown(function (e) {
                            if (e.which == 13) {
                                VerifyPassword()
                            }
                        })

                        function VerifyPassword() {
                            AjaxSend({
                                url: '/Login3/VerifyPassword',
                                data: {
                                    Password: $.trim($('#txtSupplierDeactivatePassword').val())
                                },
                                success: function (vData) {
                                    if (vData.Error) {
                                        WebToast(vData.Error, 'top');
                                        $('#txtSupplierDeactivatePassword').val('').focus()
                                    } else if (vData.Result == 'ok') {
                                        NewMessageBox('Please wait while ' + (IsDeactivate ? 'deactivating' : 'reactivating') + ' all supplier records ...', 'Message');

                                        AjaxSend({
                                            url: 'OrganizationShort/DeactivateAllSupplierRecords',
                                            data: { Status: CInt2(IsDeactivate) },
                                            success: function (e) {
                                                NewMessageBoxClose()

                                                if (e.Results == 'saved') {
                                                    WebToast('All supplier records has been successfully ' + (IsDeactivate ? 'deactivated' : 'reactivated'), 'top');
                                                    LibraryTable.SupplierList();
                                                } else {
                                                    NewMessageBox(e.Results, 'Message');
                                                }
                                            }
                                        })
                                    } else
                                        NewMessageBox('Unknown error');
                                }
                            });
                        }
                    }
                }, { Text: 'No' }
            ]);
        }

        function SavePriceLevelCustom() {
            if ($.trim($('#txtPriceLevelName').val()) == "") {
                WebToast('Please enter name', 'top');
                return;
            }

            $('#panAddPriceLevel').modal('hide');
            AjaxSend({
                url: 'SetupChecklist/SavePriceLevelCustom',
                data: {
                    ID: $.trim($('#txtPriceLevelCustomID').val()),
                    Name: $.trim($('#txtPriceLevelName').val()),
                    Type: $.trim($('[name="rad-pricelevel"]:checked').val()),
                    Status: $.trim(CInt2($('#selPriceLevelActive').is(':checked')))
                },
                success: function (vData) {
                    if(vData.Results == "saved"){
                        LibraryTable.PriceLevelCustom();
                    } else {
                        NewMessageBox(vData.Results, 'Message');
                    }
                }
            })
        }

        function SaveSalesRepresentativeCustom() {
            if ($.trim($('#txtSalesRepresentativeName').val()) == "") {
                WebToast('Please enter name', 'top');
                return;
            }

            $('#panAddSalesRepresentative').modal('hide');
            AjaxSend({
                url: 'SetupChecklist/SaveSalesRepresentativeCustom',
                data: {
                    ID: $.trim($('#txtSalesRepresentativeCustomID').val()),
                    Name: $.trim($('#txtSalesRepresentativeName').val()),
                    Status: $.trim(CInt2($('#selSalesRepresentativeActive').is(':checked')))
                },
                success: function (vData) {
                    if(vData.Results == "saved"){
                        LibraryTable.SalesRepresentative();
                    } else {
                        NewMessageBox(vData.Results, 'Message');
                    }
                }
            })
        }

        function SearchInTable(s) {
            if (location.hash.toLowerCase() == '' || location.hash.toLowerCase() ==  '#systemadministration') {
                return;
            }

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
            else if (sessionStorage.LibraryName == 'Brands Library') {
                LibraryTable.PIMBrands(s);
            }
            else if (sessionStorage.LibraryName == 'Product Categories') {
                LibraryTable.PIMCategories(s);
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
                LibraryTable.NewCustomerSearch(s); //LibraryTable.CustomerList(s);
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
            } else if (sessionStorage.LibraryName == "Table View Settings") {
                LibraryTable.TableView(s);
            } else if (sessionStorage.LibraryName == 'Payment Terms') {
                LibraryTable.PaymentTermsList(s);
            } else if (sessionStorage.LibraryName == 'Price Levels') {
                LibraryTable.PriceLevelCustom(s);
            } else if (sessionStorage.LibraryName == 'Sales Representatives') {
                LibraryTable.SalesRepresentative(s);
            } else if (sessionStorage.LibraryName == "Online Store Categories") {
                LibraryTable.OnlineStoreCategories(s);
            }

            if (sessionStorage.LibraryName != "Customer / Client List") {
                $('#divSystemAdDashboard, #UniversalDiv, #divSubList, #divForPIM').hide();
                $('#UniversalDiv').empty();
                $('#divSecond').show();
                window.location.hash = "LibraryPage";
                AddBreadCrumb({
                    text: sessionStorage.LibraryName,
                    UpdateLastchild: true,
                    Click: function () {

                    }
                }, '.ItemsBreadCrumbs')
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
            $('.divSetupProductBrandsVal').height($('.divSetupProductBrands').height());
            $('.divSetupProductCategoriesVal').height($('.divSetupProductCategroies').height());
            $('.divQuantitySettingVal').height($('.divQuantitySetting').height());
            $('.divPOSSettingVal').height($('.divPOSSetting').height());
            $('.divSalesOrderSettingVal').height($('.divSalesOrderSetting').height());
            $('.divHotButtonsVal').height($('.divHotButtons').height());
            $('.divGroupSettingsVal').height($('.divGroupSettings').height());
            $('.divRevenueCentersVal').height($('.divRevenueCenters').height());
            $('.divCostCentersVal').height($('.divCostCenters').height());
            $('.divChartOfAccountsVal').height($('.divChartOfAccounts').height());
            $('.divEcommerceGeneralSettingsVal').height($('.divEcommerceGeneralSettings').height());
            $('.divOnlineStoreCategoriesVal').height($('.divOnlineStoreCategories').height());
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
                $('#selOptionStatus').val(1);//.selectmenu('refresh');
                $('#divOptionStatus').show();
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
                $('#selOptionStatus').val(1);//.selectmenu('refresh');
                $('#divOptionStatus').show();
                LibraryTable.PIMPage();
            }
            else if (sessionStorage.LibraryName == "Brands Library") {
                $('#selOptionStatus').val(1);//.selectmenu('refresh');
                $('#divOptionStatus').show();
                LibraryTable.PIMBrands();
            }
            else if (sessionStorage.LibraryName == "Product Categories") {
                $('#selOptionStatus').val(1);//.selectmenu('refresh');
                $('#divOptionStatus').show();
                LibraryTable.PIMCategories();
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
                $('#selOptionStatus').val(1);//.selectmenu('refresh');
                $('#divOptionStatus').show();
                LibraryTable.SupplierList();
            }
            else if (sessionStorage.LibraryName == "Sales Team") {
                LibraryTable.SalesTeam();
            }
            else if (sessionStorage.LibraryName == "Item Modifiers") {
                $('#selOptionStatus').val(1);//.selectmenu('refresh');
                $('#divOptionStatus').show();
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
            else if (sessionStorage.LibraryName == "Payment Terms") {
                LibraryTable.PaymentTermsList();
            }
            else if (sessionStorage.LibraryName == "Price Levels") {
                LibraryTable.PriceLevelCustom();
            }
            else if (sessionStorage.LibraryName == "Sales Representatives") {
                LibraryTable.SalesRepresentative();
            }
            else if (sessionStorage.LibraryName == "Online Store Categories") {
                LibraryTable.OnlineStoreCategories();
            }
            else if (sessionStorage.LibraryName == "Proprietary Barcode Type") {
                LibraryTable.ProprietaryBarcodeType();
            }
            else if (sessionStorage.LibraryName == "Revenue Centers" || sessionStorage.LibraryName == "Cost Centers") {
                $('#divOptionStatus').show();
                LibraryTable.Centers();
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
            else if (sessionStorage.LibraryName == "Supplier / Vendor List") {
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
            else if (sessionStorage.LibraryName == "Online Store Categories") {
                LibraryTable.OnlineStoreCategoriesSearch(SearchText);
            }
        }

        function GoToThisPage(AddDevice) {

            //sessionStorage["AddLibraryName"] = AddDevice;
            $('#UniversalDiv').html('<div style="width:100%; text-align:center;">Loading ...</div>');

            if (sessionStorage.LibraryName == "Organization") {
                window.location.hash = 'UniversalPage';
                LibraryTable.OrganizationPage();
            }
            else if (sessionStorage.LibraryName == "Update Server IP") {
                window.location.hash = 'UniversalPage';
                LibraryTable.UpdateServerIpSettings();
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
                        //LibraryTable.AddReceiptSettingsPage();
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
            else if (sessionStorage.LibraryName == "GPS Settings") {
                window.location.hash = 'UniversalPage';
                LibraryTable.GPSSettingsPage();
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
                $('#divAddProductPopup').modal('hide');
                $('#divAdjustProdPricesPopup').modal('hide');

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
                    $("#panAddLibrary").modal('hide');
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
            else if (sessionStorage.LibraryName == 'Print Settings') {
                window.location.hash = 'UniversalPage';
                LibraryTable.PrintSettings();
            }
            else if (sessionStorage.LibraryName == 'Purchase Order Settings') {
                window.location.hash = 'UniversalPage';
                LibraryTable.PurchaseOrderSettings();
            }
            else if (sessionStorage.LibraryName == 'Purchase Order Settings') {
                window.location.hash = 'UniversalPage';
                LibraryTable.PurchaseOrder();
            } else if (sessionStorage.LibraryName == 'Product Maker Settings') {
                window.location.hash = 'UniversalPage';
                LibraryTable.ProductMakerSettings();
            }else if (sessionStorage.LibraryName == 'ERP Settings') {
                window.location.hash = 'UniversalPage';
                LibraryTable.ErpSettings();
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
                LibraryTable.NewCustomerPage();
                return;

                if (sessionStorage.AddViewLibBCName == "Add New Customer") {
                    LibraryTable.AddCustomerPage();
                }
                else {
                    LibraryTable.ViewCustomerPage();
                }

            }
            else if (sessionStorage.LibraryName == "Supplier / Vendor List") {
                sessionStorage.SupplierType = 'Org';
                window.location.hash = 'UniversalPage';
                LibraryTable.NewCustomerPage();
                return;
                //same lang ang ui ng supplier sa customer, mao ng giretain ang ui sa customer.
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
            else if (sessionStorage.LibraryName == "Payment Terms") {
                sessionStorage["AddViewLibBCName"] = "Payment Term";
                window.location.hash = 'UniversalPage';
                LibraryTable.AddPaymentTerms();
            }
            else if (sessionStorage.LibraryName == "Account Adjustments") {
                window.location.hash = 'UniversalPage';
                LibraryTable.AccountAdjustmentsPage();
            }
            else if (sessionStorage.LibraryName == "Knowledge Management") {
                $('#panAddLibrary').modal('hide');
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
            else if (sessionStorage.LibraryName == "Ecommerce General Settings") {
                window.location.hash = 'UniversalPage';
                LibraryTable.EcommerceGeneralSettingsPage();
            }
            else if (sessionStorage.LibraryName == "Banner Settings") {
                window.location.hash = 'UniversalPage';
                LibraryTable.EcommerceBannerSettingsPage();
            }
            else if (sessionStorage.LibraryName == "Vendor Portal Settings") {
                window.location.hash = 'UniversalPage';
                LibraryTable.EcommerceVendorPortalSettingsPage();
            }
            else if (sessionStorage.LibraryName == "Online Store Categories") {
                window.location.hash = 'UniversalPage';

                if (sessionStorage.AddViewLibBCName == "Add Store Category Template") {
                    LibraryTable.AddOnlineStoreCategoriesPage();
                }
                else {
                    LibraryTable.ViewOnlineStoreCategoriesPage();
                }
            }
            else if (sessionStorage.LibraryName == "Revenue Centers" || sessionStorage.LibraryName == "Cost Centers") {
                window.location.hash = 'UniversalPage';
                LibraryTable.CentersPage();
                //if (sessionStorage.AddViewLibBCName == "Revenue Center") {
                    
                //}
                //else {
                //    LibraryTable.ViewCenterPage();
                //}
            } else if (sessionStorage.LibraryName == "Flushing Report") {
                window.location.hash = 'UniversalPage';
                LibraryTable.FlushingReportPage();
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
                                NewMessageBox("This feature requires internet connection.",
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
                    NewMessageBox("This feature requires internet connection.",
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
                                                IsAddLibraryDisplayInUnivDiv = true;
                                                sessionStorage["LibraryName"] = 'Organization';
                                                GoToThisPage('Organization');
                                            }
                                        });
                                    }
                                },
                                error: function (vError) {
                                    NewMessageBox("This feature requires internet connection.",
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
                    NewMessageBox("This feature requires internet connection.",
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
                                                    headerData = ['#', 'Name', 'Address', 'Last Modified By', 'Last Modified Date', 'Last Transaction Date', 'Status'];
                                                    datasort = ['int', 'string', 'string', 'string', 'date', 'string'];
                                                    sessionStorage["LibraryName"] = 'FOWST';
                                                    sessionStorage["LabelLibraryName"] = 'FOWST';
                                                    DisplayTableLibrary(headerData, datasort);
                                                }
                                                else if (s == "FOWSTProdList") {
                                                    IsAddLibraryDisplayInUnivDiv = false;
                                                    window.location.hash = "UniversalPage";
                                                    sessionStorage["AddViewLibBCName"] = 'FOWST Product List';
                                                    //LibraryTable.FowstProductList();
                                                }
                                                else {
                                                    return;
                                                }
                                            }
                                        });
                                    }
                                },
                                error: function (vError) {
                                    NewMessageBox("This feature requires internet connection.",
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
                    NewMessageBox("This feature requires internet connection.",
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

        function ReactivateAllProducts() {
            AjaxSend({
                url: '/PIMProduct/ReactivateAllProducts',
                success: function (vData) {
                    if (vData.Error)
                        NewMessageBox(vData.Error)
                    else if (vData.AffectedCount == '0') {                        
                        NewMessageBox('All products were already reactivated.');
                    }
                    else if (vData.AffectedCount) {
                        LibraryTable.PIMPage();
                        NewMessageBox('Number of products that were reactivated by this action: ' + vData.AffectedCount + '.');                        
                    }
                }
            });
        }
        //prev code (about KB) removed by arnie 3222017
    });
});