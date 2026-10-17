require([
    'common', 'tablesort', '/Scripts/jquery-ui-sortable.js',
    '/Content/Ecommerce/js/lightbox-2.11.1.js'
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
        var arrPromoCodeProducts = [];

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

            ShowHideLibraries();

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

                        if (vData.length > 0) {
                            vData = 'However, we cannot find data in some products in the excel file. These products are ' + vData.substr(1, vData.length);
                        }

                        NewMessageBox('Uploaded products are now reactivated. ' + vData);
                    }
                });
            }).on('submit', '#formUploadEmployees', function (e) {
                e.preventDefault();

                $("#OrgID").val(sessionStorage.OrgID);

                //grab form data
                var formData = new FormData($(this)[0]);
                Bs.Loading('show', 'Uploading... please wait');
                $.ajax({
                    url: '/People/UploadEmployees?OrgID=' + sessionStorage.OrgID,
                    type: 'POST',
                    data: formData,
                    async: false,
                    cache: false,
                    contentType: false,
                    processData: false,
                    success: function (vData) {
                        Bs.Loading('hide');
                        vData = JSON.parse(vData).d;
                        $('#emptemplateobjFile').val('');

                        if (vData.Error != undefined) {
                            NewMessageBox(vData.Error, 'Invalid data');
                            return;
                        }

                        if (CBool(vData.Results)) {
                            NewMessageBox('Employees has been saved successfully.');
                            window.open('/People/DownloadFile?FileName=' + 'EmployeesTemplate/' + sessionStorage.OrgID + '/EmployeesTemporaryCredentials.xls' + '&OrgID=' + sessionStorage.OrgID, "_blank");
                        } else {
                            NewMessageBox('There are problems with your Employee Template file. Please check the data in your excel file.');
                        }
                    }
                });
            });

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
                            headerData = ['#', 'Image', 'Short Description', 'Product No.', 'Retail Price', 'Brand', 'Category', 'Activation Date', 'Last Modified Date', 'Status'];
                            datasort = ['int', '', 'string', 'string', 'int', 'string', 'string', 'date'];
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

                        else if (sessionStorage.LibraryName == 'Regional Group Settings') {
                            breadcrumbs.prepend(arrowR, sessionStorage.LibraryName);
                            headerData = ['#', 'Region Name', 'Status', 'Last Modified By', 'Last Modified Date'];
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
                            headerData = ['#', 'Group Modifier Name', , 'Modifier Type', 'Status', 'Modified By', 'Modified Date'];
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
                            headerData = ['#', 'Price Level Name', 'Availability', 'Active', 'Modified By', 'Last Modified Date'];
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
                                if (typeof canvasArea != "undefined" && canvasArea != null) {
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
                    location.hash = '';

                    AddBreadCrumb({
                        text: 'System Administration', Click: function () {
                            window.location.hash = 'SystemAdministration';
                        }
                    }, '.ItemsBreadCrumbs')
                    clearInterval(tempinterval)
                }
            }, 100)

            proj.CheckIfVDMFeaturesIsActivated(function (e) {
                if (CBool(e.EnableVDMFeatures)) {
                    $('#divForVendingMachine').show();
                } else {
                    $('#divForVendingMachine').hide();
                }

                GetOrganizationName();
                ResizeHeightOfRequiredData();
                ShowCheckList();
            })            

            $('#txtPLFromDate').attr('min', $.format.date(new Date(), 'yyyy-MM-ddT00:00'));

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

            $('#btnHomeSystemAdmin').click(function () {
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

            $('#btnEmpTemplateUpload').click(function () {
                $('#formUploadEmployees').submit();
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
                        NewPromptUserAccess('Allow access to the list of devices', {
                            Message: 'You don\'t have access to <b>Device setting</b>.',
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
                        NewMessageBox('This feature requires internet connection.');
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
                        NewMessageBox('This feature requires internet connection.');
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
                        NewMessageBox('This feature requires internet connection.');
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
                        NewMessageBox('This feature requires internet connection.');
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
                NewPromptUserAccess('PIM Settings', {
                    OnClickOKSuccess: function () {
                        window.location.hash = '';
                        window.location.hash = "LibraryPage";
                        headerData = ['#', 'Image', 'Product No.', 'Brand', 'Short Description', 'SRP', 'Retail Price', 'Wholesale Price', 'Category', 'Activation Date', 'Last Modified Date', 'Status'];
                        datasort = ['int', '', 'string', 'string', 'int', 'int', 'int', 'string', 'string', 'date'];
                        sessionStorage["LibraryName"] = 'PIM';
                        sessionStorage["LabelLibraryName"] = 'Product';
                        sessionStorage["AddViewLibBCName"] = "Add Product";
                        DisplayTableLibrary(headerData, datasort);
                    }
                })
            })

            $('#btnPrinter, #popupbtnPrinter').click(function () {
                NewMessageBoxClose();
                NewPromptUserAccess('Printers', {
                    OnClickOKSuccess: function () {
                        window.location.hash = '';
                        window.location.hash = "LibraryPage";
                        headerData = ['#', 'Printer Brand', 'Printer Model', 'Printer Name', 'Serial Number', 'Interface Type', 'IP Address', 'FOWST', 'List of devices that use this as default printer', 'Role', 'Categories'];
                        datasort = ['int', 'string', 'string', 'date'];
                        sessionStorage["LibraryName"] = 'Printers';
                        sessionStorage["LabelLibraryName"] = 'Printer';
                        sessionStorage["AddViewLibBCName"] = "Add Printer";
                        DisplayTableLibrary(headerData, datasort);
                    }
                })                
            })

            $('#btnGeneralSettings, #popupbtnGeneralSettings').click(function () {
                //NewPromptUserAccess('Sales Return Settings', {
                //    OnClickOKSuccess: function () {
                        
                //    }
                //});
                NewMessageBoxClose();
                NewPromptUserAccess('General Settings', {
                    OnClickOKSuccess: function () {
                        window.location.hash = '';
                        sessionStorage["LibraryName"] = 'General Settings';
                        GoToThisPage('General Settings');
                    }
                })                
            })

            $('#btnSetupProductBrands, #popupbtnProductBrands').click(function () {
                NewMessageBoxClose();
                NewPromptUserAccess('Brands Library', {
                    OnClickOKSuccess: function () {
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
                    }
                })
            })

            $('#btnSetupProductCategories, #popupbtnProductCategories').click(function () {
                NewMessageBoxClose();
                NewPromptUserAccess('Product Categories', {
                    OnClickOKSuccess: function () {
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
                        NewMessageBox('This feature requires internet connection.');
                    }

                });
            })

            $('#butPrintSettings, #popupbtnPrintSettings').click(function () {
                NewMessageBoxClose();
                NewPromptUserAccess('Print Settings', {
                    OnClickOKSuccess: function () {
                        window.location.hash = '';
                        sessionStorage["LibraryName"] = 'Print Settings';
                        GoToThisPage();
                    }
                })                
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
                        NewMessageBox('This feature requires internet connection.');
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
                        NewMessageBox('This feature requires internet connection.');
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
                        NewMessageBox('This feature requires internet connection.');
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

            $('#divPopPassword button[name=ok]').click(function () {
                AjaxSend({
                    url: '/Login3/VerifyPassword',
                    data: {
                        Password: $('#divPopPassword input[type=password]').val()
                    },
                    success: function (vData) {
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
                        success: function (vData) {
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
                            Click: function () {
                                DeactivateAllProducts();
                            }
                        },
                        {
                            Text: 'No'
                        }
                    ]);
                }
            });

            $('#divPopPassword button[name=close]').click(function () {
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
                        Click: function () {
                            PromptAccess();
                        }
                    },
                    {
                        Text: 'No'
                    }
                ]);
            });

            $('#btnDeactivateDormantSku').click(function (e) {
                IsAddLibraryDisplayInUnivDiv = false;
                sessionStorage["AddViewLibBCName"] = 'Dormant SKUs Settings';
                window.location.hash = "UniversalPage";                
            })

            $('#btnAdjustProdPrices').click(function () {
                $('#divAddProductPopup').modal('hide');
                $('#divAdjustProdPricesPopup').modal('show');
            })

            $('#btnBackToOption').click(function () {
                $('#divAdjustProdPricesPopup').modal('hide');
                $('#divAddProductPopup').modal('show');
            })

            $('#butErpToMobizExporter').click(function () {
                AjaxSend({
                    url: 'ErpToMobizExporter',
                    type: 'GET',
                    dataType: 'text',
                    success: function (vData) {
                        NewMessageBox(vData);
                    },
                    error: function (vError) {
                        NewMessageBox('This feature requires internet connection.');
                    }
                });
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
                        NewMessageBox('This feature requires internet connection.');
                    }
                });
            })

            $('#btnBulkPrice').click(function () {
                IsAddLibraryDisplayInUnivDiv = false;
                window.location.hash = "UniversalPage";
                sessionStorage["AddViewLibBCName"] = 'Bulk Price Adjustment';
                //LibraryTable.BulkPriceAdjustmentPage();
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

            $('#butPimSalesCaching').click(function () {
                IsAddLibraryDisplayInUnivDiv = true;
                window.location.hash = "UniversalPage";
                sessionStorage["AddViewLibBCName"] = 'Offtake / Sales Data Caching for Supply Chain Management';
                LibraryTable.PIMSalesCaching();
            })

            $('#butPimProductsAndContentsExcel').click(function () {
                AjaxSend({
                    url: 'Stock/ProductsAndContentsExcel',
                    success: function(vData) {
                        if (vData.Error) {
                            NewMessageBox(vData.Error);
                            return;
                        }

                        if (CStr2(vData.Filename).trim() == '') {
                            NewMessageBox('Expected data was not found. jsQJRYD46QW5!');
                            return;
                        }

                        var pUrl = MergeUrlParameters({
                            vFilename: vData.Filename
                        }, '/Download');

                        window.open(pUrl, '_blank', 
                            'width=' + window.innerWidth + 
                            ',height=' + window.innerHeight + 
                            ',scrollbars=auto');
                        NewMessageBox('A new browser windows should be opened by now. If not, please check your web browser\'s popup settings.');
                    }
                });
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

            $('#butBomIssues').click(function () {
                AjaxSend({
                    url: 'Stock/GetRecursiveContents',
                    success: function(vData) {
                        if (vData.Error)
                            NewMessageBox(vData.Error)
                        else if (vData.Issues)
                            NewMessageBox(vData.Issues);
                        else if (vData.Result == 'ok')
                            NewMessageBox('No issues has been found.', 'No Issues');
                    }
                });
            });

            $('#butPimMergeBrands').click(function () {
                $('#divAddProductPopup').modal('hide');
                require(['promise!/Scripts/sources/ERP/BrandMerger/controller.js'],
                    function () {
                        new ERP.BrandMerger.controller().Show();
                    }
                );
            });

            $('#btnDownloadCompanyProductList').click(function (e) {
                e.preventDefault();
                NewMessageBox('What to you want to download?', 'Download', [
                    {
                        Text: 'Active Products',
                        Click: function() {
                            var pUrl = MergeUrlParameters({
                                OrgID: sessionStorage.OrgID,
                                IncludeActive: 1,
                            }, '/Organization/DownloadCompanyProductList');

                            window.open(pUrl, '_blank', 
                                'width=' + window.innerWidth + 
                                ',height=' + window.innerHeight + 
                                ',scrollbars=auto');
                            NewMessageBox('A new browser windows should be opened by now. If not, please check your web browser\'s popup settings.');
                        }
                    },
                    {
                        Text: 'Inactive Products',
                        Click: function() {
                            var pUrl = MergeUrlParameters({
                                OrgID: sessionStorage.OrgID,
                                IncludeInactive: 1,
                            }, '/Organization/DownloadCompanyProductList');

                            window.open(pUrl, '_blank', 
                                'width=' + window.innerWidth + 
                                ',height=' + window.innerHeight + 
                                ',scrollbars=auto');
                            NewMessageBox('A new browser windows should be opened by now. If not, please check your web browser\'s popup settings.');
                        }
                    },
                    {
                        Text: 'Active And Inactive Products',
                        Click: function() {
                            var pUrl = MergeUrlParameters({
                                OrgID: sessionStorage.OrgID,
                                IncludeActive: 1,
                                IncludeInactive: 1,
                            }, '/Organization/DownloadCompanyProductList');

                            window.open(pUrl, '_blank', 
                                'width=' + window.innerWidth + 
                                ',height=' + window.innerHeight + 
                                ',scrollbars=auto');
                            NewMessageBox('A new browser windows should be opened by now. If not, please check your web browser\'s popup settings.');
                        }
                    }
                ]);
            })

            $('#btnQuantitySetting, #popupbtnQuantitySettings').click(function () {
                NewMessageBoxClose();
                NewPromptUserAccess('Decimal in Quantity Settings', {
                    OnClickOKSuccess: function () {
                        window.location.hash = '';
                        sessionStorage["LibraryName"] = 'Decimals in Quantity Settings';
                        GoToThisPage();
                    }
                })
            })

            $('#btnPOSSetting, #popupbtnPOSSettings').click(function () {
                NewMessageBoxClose();
                NewPromptUserAccess('POS Settings', {
                    OnClickOKSuccess: function () {
                        window.location.hash = '';
                        sessionStorage["LibraryName"] = 'POS Settings';
                        GoToThisPage();
                    }
                })                
            })

            $('#btnSalesOrderSetting, #popupbtnSalesOrder').click(function () {
                NewMessageBoxClose();
                NewPromptUserAccess('Sales Order Settings', {
                    OnClickOKSuccess: function () {
                        window.location.hash = '';
                        sessionStorage["LibraryName"] = 'Sales Order Settings';
                        GoToThisPage();
                    }
                })                
            })

            $('#butUpdateServerIp').click(function () {
                NewMessageBoxClose();
                window.location.hash = '';
                sessionStorage["LibraryName"] = 'Update Server IP';
                GoToThisPage();
            });

            $('#btnReceivingAndTransferOutSetting, #popupbtnReceivingAndTransferOut').click(function () {
                NewMessageBoxClose();
                NewPromptUserAccess('Receiving and Transfer-Out Settings', {
                    OnClickOKSuccess: function () {
                        window.location.hash = '';
                        sessionStorage["LibraryName"] = 'Receiving And Transfer-Out Settings';
                        GoToThisPage();
                    }
                })
            })

            $('#btnInventoryCountSetting, #popupbtnInventoryCount').click(function () {
                NewMessageBoxClose();
                NewPromptUserAccess('Inventory Count Settings', {
                    OnClickOKSuccess: function () {
                        window.location.hash = '';
                        sessionStorage["LibraryName"] = 'Inventory Count Settings';
                        GoToThisPage();
                    }
                })
            })

            $('#btnDailyBeginningBalanceSetting, #popupbtnDailyBeginningBalance').click(function () {
                NewMessageBoxClose();
                NewPromptUserAccess('Daily Beginning Balance Settings', {
                    OnClickOKSuccess: function () {
                        window.location.hash = '';
                        sessionStorage["LibraryName"] = 'Daily Beginning Balance Settings';
                        GoToThisPage();
                    }
                })
            })

            $('#butPurchaseSettings, #btnPurchaseOrderSetting, #popupbtnPurchaseOrderSettings').click(function () {
                NewMessageBoxClose();
                NewPromptUserAccess('Purchase Order Settings', {
                    OnClickOKSuccess: function () {
                        window.location.hash = '';
                        sessionStorage["LibraryName"] = 'Purchase Order Settings';
                        GoToThisPage();
                    }
                })
            });

            $('#butProductMakerSettings').click(function () {
                NewMessageBoxClose();
                NewPromptUserAccess('Product Maker Settings', {
                    OnClickOKSuccess: function () {
                        window.location.hash = '';
                        sessionStorage["LibraryName"] = 'Product Maker Settings';
                        GoToThisPage();
                    }
                })                
            });

            $('#btnGPSSettings').click(function () {
                $('#panOption').modal('hide');
                NewPromptUserAccess('GPS Settings', {
                    OnClickOKSuccess: function () {
                        window.location.hash = '';
                        sessionStorage["LibraryName"] = 'GPS Settings';
                        GoToThisPage();
                    }
                })
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
                        NewMessageBox('This feature requires internet connection.');
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
                        NewMessageBox('This feature requires internet connection.');
                    }
                });
            })

            $('#btnRegionalGroupSettings, #popupbtnRegionalGroupSettings').click(function () {
                NewPromptUserAccess('Regional Group Settings', {
                    OnClickOKSuccess: function () {
                        AjaxSend({
                            url: 'Login3/TestCookies',
                            success: function (vData) {
                                window.pageSecuritySourcePage = $.mobile.activePage;
                                //NewPromptUserAccess('Group Settings', {
                                //    OnClickOKSuccess: function () {
                                NewMessageBoxClose();
                                window.location.hash = '';
                                window.location.hash = "LibraryPage";
                                headerData = ['#', 'Region Name', 'Status', 'Last Modified By', 'Last Modified Date'];
                                datasort = ['int', 'string', 'string', 'string', 'date'];
                                sessionStorage["LibraryName"] = 'Regional Group Settings';
                                sessionStorage["LabelLibraryName"] = 'Regional Group';
                                DisplayTableLibrary(headerData, datasort);
                                //    }
                                //});
                            },
                            error: function (vError) {
                                NewMessageBox('This feature requires internet connection.');
                            }
                        });
                    }
                })                
            })

            $('#btnSetupCustomer, #popupbtnCustomer').click(function () {
                AjaxSend({
                    url: 'Login3/TestCookies',
                    success: function (vData) {
                        NewPromptUserAccess('Company Customer', {
                            OnClickOKSuccess: function () {
                                NewMessageBoxClose();
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
                        NewMessageBox('This feature requires internet connection.');
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
                        NewMessageBox('This feature requires internet connection.');
                    }
                });
            })

            $('#btnSalesTeam, #popupbtnSalesTeam').click(function () {
                NewMessageBoxClose();
                NewPromptUserAccess('Sales Team', {
                    OnClickOKSuccess: function () {
                        window.location.hash = '';
                        window.location.hash = "LibraryPage";
                        headerData = ['#', 'Name', 'Description', 'Last Modified Date'];
                        datasort = ['int', 'string', 'string', 'date'];
                        sessionStorage["LibraryName"] = 'Sales Team';
                        sessionStorage["LabelLibraryName"] = 'Team';
                        DisplayTableLibrary(headerData, datasort);
                    }
                })
            })

            $('#btnItemModifiers, #popupbtnItemModifiers').click(function () {
                NewMessageBoxClose();
                NewPromptUserAccess('Item Modifiers', {
                    OnClickOKSuccess: function () {
                        window.location.hash = '';
                        window.location.hash = "LibraryPage";
                        headerData = ['#', 'Group Modifier Name', 'Modifier Type', 'Status', 'Modified By', 'Modified Date'];
                        datasort = ['int', 'string', 'string', 'string', 'date'];
                        sessionStorage["LibraryName"] = 'Item Modifiers';
                        sessionStorage["LabelLibraryName"] = 'Modifier';
                        DisplayTableLibrary(headerData, datasort);
                        $('#tblBody').empty();
                    }
                })                
            })

            $('#btnPaymentTerms, #popupbtnPaymentTerms').click(function () {
                NewMessageBoxClose();
                NewPromptUserAccess('Payment Terms', {
                    OnClickOKSuccess: function () {
                        window.location.hash = '';
                        window.location.hash = "LibraryPage";
                        headerData = ['#', 'Term Name', 'Description', 'Term', 'Active', 'Modified By', 'Last Modified Date'];
                        datasort = ['int', 'string', 'string', 'string', 'string', 'string', 'date'];
                        sessionStorage["LibraryName"] = 'Payment Terms';
                        sessionStorage["LabelLibraryName"] = 'Payment Term';
                        DisplayTableLibrary(headerData, datasort);
                    }
                })
            })

            $('#btnAccountAdjustments, #popupbtnAccountAdjustments').click(function () {
                NewMessageBoxClose();
                NewPromptUserAccess('Account Adjustments', {
                    OnClickOKSuccess: function () {
                        window.location.hash = '';
                        sessionStorage["LibraryName"] = 'Account Adjustments';
                        GoToThisPage();
                    }
                })
            })

            $('#btnPriceLevel, #popupbtnPriceLevel').click(function () {
                NewMessageBoxClose();
                NewPromptUserAccess('Price Levels', {
                    OnClickOKSuccess: function () {
                        window.location.hash = '';
                        window.location.hash = "LibraryPage";
                        headerData = ['#', 'Price Level Name', 'Availability', 'Active', 'Modified By', 'Last Modified Date'];
                        datasort = ['int', 'string', 'string', 'string', 'date'];
                        sessionStorage["LibraryName"] = 'Price Levels';
                        sessionStorage["LabelLibraryName"] = 'Price Levels';
                        DisplayTableLibrary(headerData, datasort);
                    }
                })
            });

            $('#butSysAdminSalesRep').click(function () {
                NewMessageBoxClose();
                NewPromptUserAccess('Sales Representatives', {
                    OnClickOKSuccess: function () {
                        window.location.hash = '';
                        window.location.hash = "LibraryPage";
                        headerData = ['#', 'Sales Rep Name', 'Active', 'Modified By', 'Last Modified Date'];
                        datasort = ['int', 'string', 'string', 'string', 'date'];
                        sessionStorage["LibraryName"] = 'Sales Representatives';
                        sessionStorage["LabelLibraryName"] = 'Sales Representatives';
                        DisplayTableLibrary(headerData, datasort);
                    }
                })                
            })

            $('#btnProprietaryBarcodeTypes, #popupbtnProprietaryBarcodeTypes').click(function () {
                NewMessageBoxClose();
                NewPromptUserAccess('Proprietary Barcode Types', {
                    OnClickOKSuccess: function () {
                        window.location.hash = '';
                        window.location.hash = "LibraryPage";
                        headerData = ['#', 'Barcode Type', 'Sticker Dimensions', 'Last Modified Date'];
                        datasort = ['int', 'string', 'string', 'date'];
                        sessionStorage["LibraryName"] = 'Proprietary Barcode Type';
                        sessionStorage["LabelLibraryName"] = 'Barcode Type';
                        DisplayTableLibrary(headerData, datasort);
                    }
                })
            })

            $('#butVouchers').click(function () {
                NewMessageBoxClose();
                NewPromptUserAccess('Vouchers', {
                    OnClickOKSuccess: function () {
                        window.location.hash = '';
                        window.location.hash = "LibraryPage";
                        headerData = ['#', 'Serial No.', 'Description', 'Status', 'Uploaded On', 'Used On'];
                        datasort = ['int', 'string', 'string', 'string', 'date', 'date'];
                        sessionStorage["LibraryName"] = 'Vouchers';
                        sessionStorage["LabelLibraryName"] = 'Vouchers';
                        DisplayTableLibrary(headerData, datasort);
                        $('#tblBody').empty();
                    }
                })                
            })

            $('#butPromoCode').click(function () {
                NewMessageBoxClose();
                NewPromptUserAccess('Vouchers', {
                    OnClickOKSuccess: function () {
                        window.location.hash = '';
                        window.location.hash = "LibraryPage";
                        headerData = ['#', 'Campaign Name', 'Start', 'End', 'Last Updated'];
                        datasort = ['int', 'string', 'date', 'date', 'date'];
                        sessionStorage["LibraryName"] = 'Promo Code';
                        sessionStorage["LabelLibraryName"] = 'Promo Campaign';
                        DisplayTableLibrary(headerData, datasort);
                        $('#tblBody').empty();
                    }
                })                
            })

            $('#formVoucher').submit(function (e) {
                e.preventDefault();

                var formData = new FormData($(this)[0]);
                mUploaderKey = NewID();
                formData.append('OrgID', sessionStorage.OrgID);

                //$("#OrgID").val(sessionStorage.OrgID);

                Bs.Loading('show', 'Please wait ...');
                $('#popupVouchers').modal('hide');
                $('#fileVouchers').val('');

                setTimeout(function () {
                    $.ajax({
                        url: '/SetupChecklist/UploadVouchers?OrgID=' + sessionStorage.OrgID,
                        type: 'POST',
                        data: formData,
                        async: false,
                        cache: false,
                        contentType: false,
                        processData: false,
                        success: function (vData) {
                            UploadVoucherFollowup();
                        }
                    });
                }, 1000)
            })

            $('#formPromoCampaign').submit(function (e) {
                e.preventDefault();

                var formData = new FormData($(this)[0]), convertedPromoProducts = [];
                mUploaderKey = NewID();
                formData.append('OrgID', sessionStorage.OrgID);                
                formData.append('IsMultiplePromoCode', CBool($('#IsMultiplePromoCode').is(':checked')));

                for (var i in arrPromoCodeProducts) {
                    convertedPromoProducts.push({
                        ProductID: arrPromoCodeProducts[i].ID,
                        ProductNo: arrPromoCodeProducts[i].ProductNo,
                        ProductName: arrPromoCodeProducts[i].ProductName
                    })
                }

                formData.append('Products', JSON.stringify(convertedPromoProducts));

                //Bs.Loading('show', 'Please wait ...');

                setTimeout(function () {
                    $.ajax({
                        url: '/SetupChecklist/SavePromoCampaign?OrgID=' + sessionStorage.OrgID,
                        type: 'POST',
                        data: formData,
                        async: false,
                        cache: false,
                        contentType: false,
                        processData: false,
                        success: function (vData) {
                            UploadPromoCampaignFollowup();
                        }
                    });
                }, 1000)
            })

            $('#btnSavePromoCampaign').click(function () {
                $('#formPromoCampaign').submit();
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

            $('#butSystemAdministratorReportsFlushingReport').click(function() {
                NewMessageBoxClose();
                NewPromptUserAccess('Flushing Report', {
                    OnClickOKSuccess: function () {
                        window.location.hash = '';
                        sessionStorage["LibraryName"] = 'Flushing Report';
                        GoToThisPage();
                    }
                })
            });

            $('#butSystemAdministratorAutoDataArchiving').click(function () {
                NewMessageBoxClose();
                NewPromptUserAccess('Auto Data Archiving', {
                    OnClickOKSuccess: function () {
                        window.location.hash = '';
                        sessionStorage["LibraryName"] = 'Auto Data Archiving';
                        GoToThisPage();
                    }
                })                
            })

            $('#butWebPosSettings').click(function () {
                NewMessageBoxClose();
                window.location.hash = '';
                sessionStorage["LibraryName"] = 'WebPOS Settings';
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
                })
            })                    ;

            $('#btnVDMModels').click(function (e) {
                e.preventDefault();

                window.location.hash = "LibraryPage";
                headerData = ['#', 'Board Name', 'No. of Spring Coil Motors', 'No. of Direct Connection Motors', 'Created By/Date', 'Modified By/Date'];
                datasort = ['int', 'string', 'int', 'int', 'string', 'string'];
                sessionStorage["LibraryName"] = 'VMC Boards';
                sessionStorage["LabelLibraryName"] = 'VMC Board';
                DisplayTableLibrary(headerData, datasort);
            })

            $('#btnVMCListing').click(function (e) {
                e.preventDefault();

                window.location.hash = "LibraryPage";
                headerData = ['#', 'Machine Name', 'No. of Boards', 'Main Board', 'Modified By/Date'];
                datasort = ['int', 'string', 'int', 'string', 'string'];
                sessionStorage["LibraryName"] = 'VM Model with Controller Listing';
                sessionStorage["LabelLibraryName"] = 'VM Model';
                DisplayTableLibrary(headerData, datasort);
            })

            $('#btnVMPlanogramTemplate').click(function (e) {
                e.preventDefault();

                window.location.hash = "LibraryPage";
                headerData = ['#', 'Name', 'Modified By/Date'];
                datasort = ['int', 'string', 'string'];
                sessionStorage["LibraryName"] = 'VM Planogram Template';
                sessionStorage["LabelLibraryName"] = 'Template';
                DisplayTableLibrary(headerData, datasort);
            })

            $('#btnVDMAuthorizers').click(function (e) {
                e.preventDefault();

                window.location.hash = "LibraryPage";
                headerData = ['#', 'Employee Name', 'Updated By', 'Updated On'];
                datasort = ['int', 'string', 'string', 'date'];
                sessionStorage["LibraryName"] = 'VDM Authorizers';
                sessionStorage["LabelLibraryName"] = 'VDM Authorizer';
                DisplayTableLibrary(headerData, datasort);
            })

            $('#btnVDMPhysicalButtonSettings').click(function (e) {
                e.preventDefault();

                window.location.hash = "LibraryPage";
                headerData = ['#', 'Name', 'Description', 'Last Modified By', 'Last Modified On'];
                datasort = ['int', 'string', 'String', 'string', 'date'];
                sessionStorage["LibraryName"] = 'VDM Physical Button Settings';
                sessionStorage["LabelLibraryName"] = 'Physical Button';
                DisplayTableLibrary(headerData, datasort);
            })

            $('#btnVDMRiceDispensingGuidesSettings').click(function (e) {
                e.preventDefault();

                window.location.hash = "LibraryPage";
                headerData = ['#', 'Name', 'Description', 'Last Modified By', 'Last Modified On'];
                datasort = ['int', 'string', 'String', 'string', 'date'];
                sessionStorage["LibraryName"] = 'VDM Rice Dispensing Guides';
                sessionStorage["LabelLibraryName"] = 'Guide';
                DisplayTableLibrary(headerData, datasort);
            })

            $('#btnVDMProductSelectionGuidesSettings').click(function (e) {
                e.preventDefault();

                window.location.hash = "LibraryPage";
                headerData = ['#', 'Name', 'Description', 'Last Modified By', 'Last Modified On'];
                datasort = ['int', 'string', 'String', 'string', 'date'];
                sessionStorage["LibraryName"] = 'VDM Product Selection Guides';
                sessionStorage["LabelLibraryName"] = 'Guide';
                DisplayTableLibrary(headerData, datasort);
            })

            $('#btnVDMGeneralSettings').click(function () {
                //NewPromptUserAccess('Sales Return Settings', {
                //    OnClickOKSuccess: function () {

                //    }
                //});
                NewMessageBoxClose();
                window.location.hash = '';
                sessionStorage["LibraryName"] = 'VDM General Settings';
                GoToThisPage('VDM General Settings');
            })

            $('#btnVDMAdsSettings').click(function (e) {
                e.preventDefault();

                window.location.hash = "LibraryPage";
                headerData = ['#', 'Name', 'Description', 'Last Modified By', 'Last Modified On'];
                datasort = ['int', 'string', 'String', 'string', 'date'];
                sessionStorage["LibraryName"] = 'VDM Ads';
                sessionStorage["LabelLibraryName"] = 'Ads';
                DisplayTableLibrary(headerData, datasort);
            })

            $('#btnRevenueCenters, #popupbtnRevenueCenters').click(function (e) {
                e.preventDefault();
                NewPromptUserAccess('Revenue Centers', {
                    OnClickOKSuccess: function () {
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
                    }
                })
            })

            $('#btnCostCenters, #popupbtnCostCenters').click(function (e) {
                e.preventDefault();
                NewPromptUserAccess('Cost Centers', {
                    OnClickOKSuccess: function () {
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
                    }
                })
            })

            $('#btnAddLibrary').click(function () {                
                if (sessionStorage.LibraryName == "Employees") {
                    AjaxSend({
                        url: 'SetupChecklist/CheckAddLibrary',
                        data: { LibraryName: (sessionStorage.LibraryName).replaceAll(' ', '').toLowerCase() },
                        success: function (e) {
                            PopupButtonList('', 'Please Choose an Option', [
                               {
                                   Text: 'Add Employees Using Employees Template',
                                   Size: 2,
                                   Click: function () {
                                       $('#emptemplateobjFile').val('');
                                       $('#panPopUploadEmployeesTemplate').modal('show');

                                       $('[name="btnEmployeesTemplateDownload"]').unbind('click').click(function () {
                                           window.location = GetPathFormat("/Organization/DownloadEmployeesTemplate");
                                       })
                                   }
                               },
                              {
                                  Text: 'Add Employee Using Web Form',
                                  Size: 2,
                                  Click: function () {
                                      if (e.IsSearchRecords == 1) {
                                          $('#panAddLibrary').modal('show');
                                          $('#txtSearch').focus();
                                      } else {
                                          $('#btnAddToLibrary').click();
                                      }                                      
                                  }
                              }
                            ])
                        }
                    })
                    return;
                }
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
                    sessionStorage["AddViewLibBCName"] = "New Item Modifier";
                    LibraryTable.PageItemModifiers();
                    //$('#popupPOSModifier').show();
                    //sessionStorage["AddViewLibBCName"] == "Item Modifiers Parameters";
                    //GoToThisPage();
                }
                    //                else if (sessionStorage.LibraryName == "Knowledge Management") {
                    //                    $('#panAddKnowledgeManagement').show();
                    //                }
                else if (sessionStorage.LibraryName == "Printers") {
                    sessionStorage["AddViewLibBCName"] = "New Printer";
                    sessionStorage["IsNewSetup"] = true;
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
                    $('#chbPriceLevelAvailability').removeAttr('checked');
                    $('#divPLAvailability').hide();
                    $('#txtPLFromDate').val($.format.date(new Date(), 'yyyy-MM-ddT00:00'));
                    $('#txtPLToDate').val($.format.date(new Date(new Date().setDate(new Date().getDate() + 1)), 'yyyy-MM-ddT23:59'));
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
                } else if (sessionStorage.LibraryName == "VDM Physical Button Settings") {
                    sessionStorage["AddViewLibBCName"] = "Template";

                    LibraryTable.VDMPhysicalButtonSettingsPage();
                    window.location.hash = 'UniversalPage';
                    // InitVal();
                } else if (sessionStorage.LibraryName == "VDM Rice Dispensing Guides") {
                    sessionStorage["AddViewLibBCName"] = "Template";

                    LibraryTable.VDMRiceDispensingGuidesPage();
                    window.location.hash = 'UniversalPage';
                    // InitVal();
                } else if (sessionStorage.LibraryName == "VDM Product Selection Guides") {
                    sessionStorage["AddViewLibBCName"] = "Template";

                    LibraryTable.VDMProductSelectionGuidesPage();
                    window.location.hash = 'UniversalPage';
                    // InitVal();
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
                } else if (sessionStorage.LibraryName == "Regional Group Settings") {
                    RegionalGroupSettingsPopup();
                } else if (sessionStorage.LibraryName == "VMC Boards") {
                    LibraryTable.VDMModelPopup();
                } else if (sessionStorage.LibraryName == "VM Model with Controller Listing") {
                    LibraryTable.VDMModelWithListingPopup();
                } else if (sessionStorage.LibraryName == "VM Planogram Template") {
                    LibraryTable.VDMPlanogramTemplatePopup();
                } else if (sessionStorage.LibraryName == "VDM Authorizers") {
                    LibraryTable.VDMAuthorizersPop();
                } else if (sessionStorage.LibraryName == "Vouchers") {
                    $('#btnDownloadVoucherTemplate').unbind('click').click(function () {
                        window.location = GetPathFormat("/SetupChecklist/DownloadVoucherTemplate");
                    })

                    $('#popupVouchers').modal('show');
                }
                else if (sessionStorage.LibraryName == "Promo Code") {
                    $('#btnPromoCodeDownloadTemplate').unbind('click').click(function (e) {
                        e.preventDefault();
                        window.location = GetPathFormat("/SetupChecklist/DownloadPromoCodeTemplate");
                    })
                    $('#formPromoCampaign input, #formPromoCampaign textarea').val('');
                    $('#listPromoCampaignProducts').empty();
                    $('[name="PromoStart"], [name="PromoEnd"]').attr('min', $.format.date(new Date(), 'yyyy-MM-dd'))
                    arrPromoCodeProducts = [];
                    $('#popupPromoCampaign').modal('show');
                } else if (sessionStorage.LibraryName == "FOWST") {
                    PopupButtonList('', 'Add FOWST By', [
                        {
                            Text: 'Import data from an Excel File', Click: function () {
                                $('#popupFOWSTUpload').modal('show');

                                $('[name="btnFOWSTUpload"]').unbind('click').click(function () {
                                    $('#formFOWSTUpload').submit();
                                })

                                $('[name="btnFOWSTTemplateDownload"]').unbind('click').click(function () {
                                    window.location = GetPathFormat("/Organization/DownloadFOWSTTemplate");
                                })

                                $('#formFOWSTUpload').unbind('submit').submit(function (e) {
                                    e.preventDefault();
                                    var formData = new FormData($(this)[0]);
                                    //console.log(formData)
                                    //return 
                                    Bs.Loading('show', 'Uploading... please wait');
                                    setTimeout(function () {
                                        AjaxSend({
                                            url: '/Organization/UploadFOWST?OrgID=' + sessionStorage.OrgID + '&userid=' + sessionStorage.PeopleID,
                                            type: 'POST',
                                            data: formData,
                                            async: false,
                                            cache: false,
                                            contentType: false,
                                            processData: false,
                                            success: function (vData) {
                                                $('#fileFOWSTUpload').val('');
                                                Bs.Loading('hide');
                                                if (vData.Results == "saved") {
                                                    $('#popupFOWSTUpload').modal('hide');
                                                    LibraryTable.Fowst();
                                                    NewMessageBox('Successfully uploaded', 'Message');
                                                } else if (vData.Validation != undefined) {
                                                    NewMessageBox('While validating your file template, these are the error we found: <br> ' + vData.Validation, 'Validation Message');
                                                } else {
                                                    NewMessageBox(vData.Error, 'ERROR');
                                                }
                                            }
                                        });
                                    }, 1000)
                                })
                            }
                        }, {
                            Text: 'Using Web Form', Click: function () {
                                $('#btnAddToLibrary').click();
                            }
                        }
                    ])
                } else if (sessionStorage.LibraryName == "Authorized Devices") {
                    NewPromptUserAccess('Allow adding a new device', {
                        Message: 'You\'re not allowed to add new devices.',
                        OnClickOKSuccess: function () {
                            $('#btnAddToLibrary').click();
                        }
                    })                    
                } else {
                    AjaxSend({
                        url: 'SetupChecklist/CheckAddLibrary',
                        data: { LibraryName: (sessionStorage.LibraryName).replaceAll(' ', '').toLowerCase() },
                        success: function (e) {
                            if (e.IsSearchRecords == 1) {
                                $('#panAddLibrary').modal('show');
                                $('#txtSearch').focus();
                            } else {
                                $('#btnAddToLibrary').click();
                            }
                        }
                    })
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

                $('#popupPIMLib').modal('hide');

                if (txtPIMLibID == '') {
                    SavePIMLib();
                } else {
                    if ($.trim($('#txtPIMLib').attr('currentname')) == txtPIMLib) {
                        WebToast('Please edit the name if you want to save', 'top');
                    } else {
                        Bs.Loading('show', 'Please wait while validating ...', 'Message');
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

                                if (vRows.length > 0) {
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

                                Bs.Loading('hide');
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

                        if (e.Results == 'saved') {
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

            $('#chbPriceLevelAvailability').change(function () {
                if ($(this).is(':checked')) {
                    $('#divPLAvailability').show();
                } else {
                    $('#divPLAvailability').hide();
                }
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
                                        function () {
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

            $('#btnEmployeeStoreAssigments').click(function () {
                $('#panPopUploadEmployeeStoreAssignments').modal('show');
                $('[name="fileStoreAssignments"]').val('');
                $('[name="OrgID"]').val(sessionStorage.OrgID);
                $('[name="Action"]').val('validate');

                $('[name="btnEmployeeStoreAssignmentsDownload"]').unbind('click').click(function () {
                    window.location = GetPathFormat("/Organization/DownloadEmployeeStoreAssignments");
                })

                $('#formStoreAssignments').unbind('submit').submit(function (e) {
                    e.preventDefault();
                    var formData = new FormData($(this)[0]);
                    
                    $.ajax({
                        url: '/Organization/UploadEmployeeStoreAssignments?OrgID=' + sessionStorage.OrgID,
                        type: 'POST',
                        data: formData,
                        async: false,
                        cache: false,
                        contentType: false,
                        processData: false,
                        success: function (vData) {
                            Bs.Loading('hide');
                            vData = JSON.parse(vData).d;
                            //$('#emptemplateobjFile').val('');

                            if (vData.Result != undefined) {
                                $('#panPopUploadEmployeeStoreAssignments').modal('hide');
                                NewMessageBox(vData.Result)
                                return;
                            }

                            if (vData.Error != undefined) {
                                NewMessageBox(vData.Error, 'Error')
                                return;
                            }

                            if (vData.Findings != 'good') {
                                $('[name="errmsg"]').val(vData.Findings.replace(/<br\s*\/?>/gi, '\n').replace(/<\/?i>/gi, ''))
                                if (vData.NoOfMissingStores > 0 || vData.NoOfMissingGroups > 0) {
                                    NewMessageBox(vData.Findings, 'Error', [                                        
                                        {
                                            Text: 'Copy error message to clipboard', Click: function () {
                                                CopyToClipboard('[name="errmsg"]');
                                            }
                                        },
                                        {
                                            Text: 'Ok'
                                        }
                                    ]);
                                } else {
                                    NewMessageBox(vData.Findings, 'Error', [
                                        {
                                            Text: 'I\'ll review the file first'
                                        },
                                        {
                                            Text: 'Copy error message to clipboard', Click: function () {
                                                CopyToClipboard('[name="errmsg"]');
                                            }
                                        },                                        
                                        {
                                            Text: 'Replace the existing data with the data from the file', Click: function () {
                                                $('[name="Action"]').val('savenow');
                                                Bs.Loading('show', 'Updating... please wait');
                                                $('#formStoreAssignments').submit();
                                            }
                                        }
                                    ]);
                                }
                            } else {
                                $('[name="Action"]').val('savenow');
                                Bs.Loading('show', 'Updating... please wait');
                                $('#formStoreAssignments').submit();
                            }
                        }, error: function () {
                            Bs.Loading('hide');
                        }
                    });
                })
            })

            $('#btnUploadStoreAssignments').click(function () {
                Bs.Loading('show', 'Validating... please wait');
                $('#formStoreAssignments').submit();
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
                        NewMessageBox('This feature requires internet connection.');
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
                if (sessionStorage.LibraryName == 'Item Modifiers') {
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
                } else if (sessionStorage.LibraryName == 'Authorized Devices') {
                    LibraryTable.Devices();
                } else if (sessionStorage.LibraryName == 'Regional Group Settings') {
                    LibraryTable.RegionalGroup();
                } else if (sessionStorage.LibraryName == 'Security Roles') {
                    LibraryTable.SecurityRoles();
                } else if (sessionStorage.LibraryName == 'Vouchers') {
                    LibraryTable.Vouchers();
                } else if (sessionStorage.LibraryName == 'Group Settings') {
                    LibraryTable.GroupSettings();
                }
            })

            $('#selOptionFOWSTStatus').change(function () {
                if (sessionStorage.LibraryName == 'FOWST') {
                    LibraryTable.Fowst();
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

            $('#txtSearchFranchiseeRequests').keydown(function (e) {
                if (e.which == 13) {
                    GetFranchiseeRequests();
                }
            })

            $('#selStatusFranchiseeRequests').change(function (e) {
                GetFranchiseeRequests();
            })

            $('#btnAddFranchisee').click(function () {
                $('#txtFranchiseeRegistrationForm').val(location.host + '/Franchisee?OrgID=' + sessionStorage.OrgID);
                $('#popupFranchiseeReqLink').modal('show');
            })

            $('#btnCopyFRLink').click(function () {
                CopyToClipboard('#txtFranchiseeRegistrationForm');
            })

            $('#btnExportListAsCSV').click(function () {
                $('#btnExportToCSV').click(); //SystemAdministrationQueries.js
            })

            $('#tabFranchisesStores').click(function () {
                Bs.Loading('show');
                AjaxSend({
                    url: 'Browser/LibraryOrganizationFranchiseeFowst',
                    data: { IsAll: true },
                    success: function (vData) {
                        if ($.fn.DataTable.isDataTable('#tblFranchisesStores')) {
                            $('#tblFranchisesStores').DataTable().clear().destroy();
                        }

                        var ctr = 0;

                        var tblFranchisesStores = $('#tblFranchisesStores').DataTable({
                            paging: true,
                            "pageLength": 50,
                            "ordering": false,
                            "data": vData.Results.Rows,
                            "autoWidth": false,
                            "columns": [
                                {
                                    "data": function (data) {
                                        ctr += 1;
                                        return ctr + '.';
                                    }, width: '50px'
                                },
                                {
                                    "data": function (data) {
                                        return CStr2(data.FowstCode);
                                    }
                                },
                                { "data": "FowstName" },
                                { "data": "Address" },
                                {
                                    "data": function (data) {
                                        return $.format.date(new Date(data.LastUpdated), 'MM-dd-yyyy hh:mm a')
                                    }, className: 'text-center'
                                },
                            ],
                            "rowCallback": function (row, data, index) {
                                $(row).data('data', data).click(function () {
                                    Bs.Loading('show');
                                    var vData = $(this).data('data'), arrRemoveAssignedEmp = [];
                                    $('#popupFOWSTFranchisesStores .modal-title').text((CStr2(vData.FowstCode) == '' ? '' : CStr2(vData.FowstCode) + ' - ') + '' + CStr2(vData.FowstName));

                                    AjaxSend({
                                        url: 'Organization/GetFOWSTAssignedEmployees',
                                        data: { ID: vData.ID },
                                        success: function (e) {
                                            if ($.fn.DataTable.isDataTable('#tblFranchisesStoresAssignedEmp')) {
                                                $('#tblFranchisesStoresAssignedEmp').DataTable().clear().destroy();
                                            }

                                            var ctr = 0;

                                            var tblFranchisesStoresAssignedEmp = $('#tblFranchisesStoresAssignedEmp').DataTable({
                                                paging: false,
                                                "ordering": false,
                                                "data": e.Results.Rows,
                                                "autoWidth": false,
                                                "columns": [
                                                    {
                                                        "data": function (data) {
                                                            ctr += 1;
                                                            return ctr + '.';
                                                        }, width: '50px'
                                                    },
                                                    { "data": "EmployeeName" },
                                                    {
                                                        "data": function (data) {
                                                            return CStr2(data.Position);
                                                        }
                                                    },
                                                    {
                                                        data: null,
                                                        createdCell: function (td, cellData, rowData, row, col) {
                                                            var btn = $('<button data-role="none" class="btn btn-danger">').append('<span class="bi-trash-fill"></span>');
                                                            $(td).data('data', rowData).css('text-align', 'center').html(btn)
                                                                .click(function () {
                                                                    var data = $(this).data('data');

                                                                    arrRemoveAssignedEmp.push(data);
                                                                    $(this).parents().closest('tr').remove();
                                                                    WebToast(data.EmployeeName + ' has been removed from the list', 'top');
                                                                })
                                                        }, width: '50px'
                                                    }
                                                ],
                                                //dom: 'Brtip',
                                                initComplete: function () {
                                                    Bs.Loading('hide');
                                                    $('#tblFranchisesStoresAssignedEmp').removeClass('dataTable');
                                                    $('#popupFOWSTFranchisesStores').modal('show');

                                                    $('#btnRemoveFranchisesStoresAssignedEmp').unbind().click(function () {
                                                        if (arrRemoveAssignedEmp.length > 0) {
                                                            Bs.Loading('show');
                                                            AjaxSend({
                                                                url: 'Organization/RemoveFOWSTAssignedEmployees',
                                                                data: {
                                                                    FOWSTID: vData.ID,
                                                                    Employees: arrRemoveAssignedEmp
                                                                },
                                                                success: function (e) {
                                                                    Bs.Loading('hide');
                                                                    $('#popupFOWSTFranchisesStores').modal('hide');
                                                                    if (e.Result == 'saved') {
                                                                        NewMessageBox('Successfully saved');
                                                                    } else {
                                                                        NewMessageBox('There was an error while saving your data.');
                                                                    }
                                                                }
                                                            })
                                                        } else {
                                                            NewMessageBox('Please remove at least one employee before saving.')
                                                        }
                                                    })
                                                },
                                                //dom: '<"clear"><"table-fixed-container"<"table-fixed"t>><"bottom"ip><"clear">',
                                                dom: '<"clear">t<"clear">',
                                            });

                                            //$('#txtSearchAssignedEmp').unbind().keydown(function (e) {
                                            //    tblFranchisesStoresAssignedEmp.search($.trim($(this).val())).draw();
                                            //})
                                        }, error: function () {
                                            Bs.Loading('hide');
                                        }
                                    });
                                })
                            },
                            initComplete: function () {
                                Bs.Loading('hide');
                                $('#tblFranchisesStores').removeClass('dataTable');
                            },
                            dom: '<"clear"><"table-fixed-container"<"table-fixed"t>><"bottom"ip><"clear">',
                        });

                        $('#txtSearchFranchisesStores').unbind().keydown(function (e) {
                            tblFranchisesStores.search($.trim($(this).val())).draw();
                        })
                    }, error: function () {
                        Bs.Loading('hide');
                    }
                });
            })

            $('#btnPromoCampaignAddProduct').click(function (e) {
                e.preventDefault();
                BrowsePopup('LibraryPimProduct', 'Select Product', [
                    {
                        Message: 'You can select multiple products.',
                        SelectMultiple: true,
                        DisplayText: [{Column: 'ProductNo'}, {Column: 'ProductName'}],
                        Click: function (e) {
                            arrPromoCodeProducts = $.merge(arrPromoCodeProducts, e);
                            AddPromoCampaignProducts(e);
                        }
                    }
                ])
            })

        })// End document

        function AddPromoCampaignProducts(data) {
            var listPromoCampaignProducts = $('#listPromoCampaignProducts');

            for (var i in data) {
                if ($('#' + data[i].ID).length > 0) continue;

                listPromoCampaignProducts.append($('<li class="list-group-item" id="' + data[i].ID +'">')
                    .append($('<div class="row">')
                        .append('<div class="col-auto">' + (CInt2(i) + 1) + '.</div>')
                        .append('<div class="col">' + data[i].ProductNo + ' - ' + data[i].ProductName + '</div>')
                        .append($('<div class="col-auto">')
                            .append($('<button data-role="none" class="btn btn-light border text-danger">')
                                .data('data', data[i])
                                .append('<span class="bi-trash-fill"></span>')
                                .click(function (e) {
                                    $(this).parents().closest('li').remove();
                                })
                            )
                        )
                    )
                )
            }
        }

        function UploadPromoCampaignFollowup() {
            AjaxSend({
                url: '/SetupChecklist/UploadFollowUp',
                success: function (e) {
                    Bs.Loading('hide');
                    if (e.Progress == 'success') {
                        var msg = 'Promo Campaign have been successfully saved.';

                        if (CStr2(e.Advisory) != '') {
                            msg += '<br><br>';
                            msg += 'And also please check and fix the following data, then reupload the file once done.<br><br>';
                            msg += e.Advisory;
                        }

                        NewMessageBox(msg, 'Success');
                        LibraryTable.PromoCampaignCode();
                    } else if (e.Progress == 'nodata') {
                        NewMessageBox('No data has been uploaded');
                    } else if (e.Progress == 'invalid') {
                        NewMessageBox(e.Advisory, 'Invalid Data');
                    } else if (e.Error != undefined) {
                        NewMessageBox(e.Error)
                    } else {
                        setTimeout(UploadPromoCampaignFollowup, 1000);
                        Bs.Loading('show', 'Please do not reload the page while uploading your data <br>' + e.Progress);
                    }
                }
            })
        }

        function UploadVoucherFollowup() {
            AjaxSend({
                url: '/SetupChecklist/UploadFollowUp',
                success: function (e) {
                    Bs.Loading('hide');
                    if (e.Progress == 'success') {
                        var msg = 'Vouchers have been successfully uploaded.';

                        if (CStr2(e.Advisory) != '') {
                            msg += '<br><br>';
                            msg += 'And also please check and fix the following data, then reupload the file once done.<br><br>';
                            msg += e.Advisory;
                        }

                        NewMessageBox(msg, 'Success');
                        LibraryTable.Vouchers();
                    } else if (e.Progress == 'nodata') {
                        NewMessageBox('No data has been uploaded');
                    } else if (e.Error != undefined) {
                        NewMessageBox(e.Error)
                    } else {
                        setTimeout(UploadVoucherFollowup, 1000);
                        Bs.Loading('show', 'Please do not reload the page while uploading your data <br>' + e.Progress);
                    }
                }
            })
        }

        window.RegionalGroupSettingsPopup = function (rgData) {
            var popupRegionalGroupSettings = $('#popupRegionalGroupSettings');
            var input = popupRegionalGroupSettings.find('input[type="text"]');
            var chb = popupRegionalGroupSettings.find('input[type="checkbox"]');
            var list = popupRegionalGroupSettings.find('ul');
            var btnSave = popupRegionalGroupSettings.find('[name="btnSave"]');
            var btnAddGroup = popupRegionalGroupSettings.find('[name="btnAddGroup"]');
            rgData = rgData || {};

            input.val('');
            list.empty();

            btnAddGroup.unbind('click').click(function () {
                BrowsePopup('LibraryFranchiseGroup', 'Select Groups', [
                    {
                        DisplayText: [{ Column: 'GroupName' }],
                        AutoSearch: true,
                        SelectMultiple: true,
                        Click: function (e) {
                            for (var i in e) {
                                e[i].State = 'new';
                                DisplayRegionalGroupsList(list, e[i]);
                            }
                        }
                    }
                ])
            })

            btnSave.unbind('click').click(function () {
                if ($.trim(input.val()) == '') {
                    WebToast('Please enter name');
                    return;
                }

                if (list.find('li:not(.removed)').length == 0) {
                    WebToast('Please add at least one group');
                    return;
                }

                var data = {
                    ID: CStr2(rgData.ID),
                    Name: input.val(),
                    IsActive: chb.is(':checked'),
                    Groups: []
                }

                list.find('li').each(function () {
                    data.Groups.push($(this).data('data'));
                })

                AjaxSend({
                    url: '/SetupChecklist/SaveRegionalGroup',
                    data: data,
                    success: function (e) {
                        if (e.Result == 'saved') {
                            WebToast('Successfully saved');
                            popupRegionalGroupSettings.modal('hide');
                            LibraryTable.RegionalGroup();
                        } else {
                            NewMessageBox(e.Result, 'Message');
                        }
                    }
                })
            })

            popupRegionalGroupSettings.modal('show');

            if (CStr2(rgData.ID) != '') {
                input.val(rgData.RegionName);
                chb.prop('checked', CInt2(rgData.IsActive));

                AjaxSend({
                    url: '/Browser/LibraryGroupsOfRegion',
                    data: { ID: rgData.ID },
                    success: function (e) {
                        var vRows = e.Results.Rows;

                        for (var i in vRows) {
                            vRows[i].State = 'old';
                            DisplayRegionalGroupsList(list, vRows[i]);
                        }
                    }
                })
            }
        }

        function DisplayRegionalGroupsList(list, data) {
            if ($('#li' + data.ID).length > 0) return;

            list.append($('<li class="list-group-item" id="li' + data.ID + '">')
                .data('data', data)
                .append($('<table style="width:100%">')
                    .append($('<tr>')
                        .append($('<td>')
                            .text(data.GroupName)
                        )
                        .append($('<td class="text-center" style="width:50px;">')
                            .append($('<button data-role="none" class="btn btn-danger">')
                                .append($('<span class="bi-trash">'))
                                .data('data', data)
                                .click(function () {
                                    var data = $(this).data('data');

                                    NewMessageBox('Are you sure you want to remove <b>' + data.GroupName + '</b>?', 'Confirmation Dialog', [
                                        {
                                            Text: 'Yes', Click: function () {                                                
                                                if (data.State == 'old') {
                                                    $('#li' + data.ID).addClass('removed').hide();
                                                } else {
                                                    $('#li' + data.ID).remove();
                                                }
                                                data.State = 'delete';
                                            }
                                        }, { Text: 'No' }
                                    ])
                                })
                            )
                        )
                    )
                )
            )
        }

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
            var fromdate = '', todate = '';

            if ($.trim($('#txtPriceLevelName').val()) == "") {
                WebToast('Please enter name', 'top');
                return;
            }

            if ($('#chbPriceLevelAvailability').is(':checked')) {
                if (($('#txtPLFromDate').val() == '' || $('#txtPLToDate').val() == '')
                    || (new Date($('#txtPLFromDate').val()) > new Date($('#txtPLToDate').val()))) {
                    WebToast('Please set valid date range', 'top');
                    return;
                }

                fromdate = $.format.date(new Date($('#txtPLFromDate').val()), 'MMM dd, yyyy hh:mm a');
                todate = $.format.date(new Date($('#txtPLToDate').val()), 'MMM dd, yyyy hh:mm a');
            }

            $('#panAddPriceLevel').modal('hide');
            AjaxSend({
                url: 'SetupChecklist/SavePriceLevelCustom',
                data: {
                    ID: $.trim($('#txtPriceLevelCustomID').val()),
                    Name: $.trim($('#txtPriceLevelName').val()),
                    Type: $.trim($('[name="rad-pricelevel"]:checked').val()),
                    Status: $.trim(CInt2($('#selPriceLevelActive').is(':checked'))),
                    FromDate: fromdate,
                    ToDate: todate
                },
                success: function (vData) {
                    if (vData.Results == "saved") {
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
                    if (vData.Results == "saved") {
                        LibraryTable.SalesRepresentative();
                    } else {
                        NewMessageBox(vData.Results, 'Message');
                    }
                }
            })
        }

        function SearchInTable(s) {
            if (location.hash.toLowerCase() == '' || location.hash.toLowerCase() == '#systemadministration') {
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
            else if (sessionStorage.LibraryName == "Vouchers") {
                LibraryTable.Vouchers(s);
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
                                            .css('text-align', 'center')
                                            .addClass('fixed-col')
                                        );
            }

            $('#divSecondTabs').hide();
            $('#divSecondTabs li:not(:first-child)').hide();
            $('[data-target="#listofdata"]').tab('show');
            $('.colFOWSStatus').addClass('d-none');
            $('#btnExportToCSV').show();

            if (sessionStorage.LibraryName == "FOWST") {
                $('#selOptionStatus').val(1);//.selectmenu('refresh');
                $('#divOptionStatus, #divSecondTabs').show();
                $('#tabListOfData').text('FOWST Lists');
                $('#tabFranchiseerequest, #tabFranchisesStores').parent().show();
                $('#selStatusFranchiseeRequests').val('new');
                $('.colFOWSStatus').removeClass('d-none');
                LibraryTable.Fowst();
                GetFranchiseeRequests();
            }
            else if (sessionStorage.LibraryName == "Authorized Devices") {
                $('#selOptionStatus').val(1);
                $('#divOptionStatus').show();
                LibraryTable.Devices();
            }
            else if (sessionStorage.LibraryName == "Receipt Settings") {
                $('#divDataTable').pageshow();
                LibraryTable.ReceiptSettings();
            }
            else if (sessionStorage.LibraryName == "Security Roles") {
                $('#selOptionStatus').val(1);
                $('#divOptionStatus').show();
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
                $('#divSecondTabs').show();
                $('#tabListOfData').text('Product Categories');
                $('#tabSortingProductCategories').parent().show();
                $('#selOptionStatus').val(1);//.selectmenu('refresh');
                $('#divOptionStatus').show();
                LibraryTable.PIMCategories();
            }
            else if (sessionStorage.LibraryName == "Hot Buttons Settings") {
                LibraryTable.HotButtons();
            }
            else if (sessionStorage.LibraryName == "Group Settings") {
                $('#selOptionStatus').val(1);//.selectmenu('refresh');
                $('#divOptionStatus').show();
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
                $('#divOptionStatus, #divSecondTabs').show();
                $('#tabListOfData').text('Item Modifier Lists');
                $('#tabSortingItemModifiers').parent().show();
                LibraryTable.SearchItemModifiers('');
                GetSortingItemModifier();
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
            else if (sessionStorage.LibraryName == "VMC Boards") {
                LibraryTable.VDMModels();
            }
            else if (sessionStorage.LibraryName == "VM Model with Controller Listing") {
                LibraryTable.VMModelWithControllerListing();
            }
            else if (sessionStorage.LibraryName == "VM Planogram Template") {
                LibraryTable.VDMPlanogramTemplate();
            }
            else if (sessionStorage.LibraryName == "VDM Authorizers") {
                LibraryTable.VDMAuthorizers();
            }
            else if (sessionStorage.LibraryName == "VDM Physical Button Settings") {
                LibraryTable.VDMPhysicalButtonSettings();
            }
            else if (sessionStorage.LibraryName == "VDM Rice Dispensing Guides") {
                LibraryTable.VDMRiceDispensingGuides();
            }
            else if (sessionStorage.LibraryName == "VDM Product Selection Guides") {
                LibraryTable.VDMProductSelectionGuides();
            }
            else if (sessionStorage.LibraryName == "VDM Ads") {
                LibraryTable.VDMAds();
            }
            else if (sessionStorage.LibraryName == "Vouchers") {
                $('#divOptionStatus').show();
                LibraryTable.Vouchers();
            }
            else if (sessionStorage.LibraryName == "Promo Code") {
                $('#divOptionStatus').show();
                LibraryTable.PromoCampaignCode();
            }
            else if (sessionStorage.LibraryName == "Proprietary Barcode Type") {
                LibraryTable.ProprietaryBarcodeType();
            }
            else if (sessionStorage.LibraryName == "Revenue Centers" || sessionStorage.LibraryName == "Cost Centers") {
                $('#divOptionStatus').show();
                LibraryTable.Centers();
            }
            else if (sessionStorage.LibraryName == "Regional Group Settings") {
                $('#divOptionStatus').show();
                LibraryTable.RegionalGroup();
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

            if (sessionStorage.LibraryName == "FOWST") {
                $('#divAddFranchisee').show();
            }
            else {
                $('#divAddFranchisee').hide();
            }

            if (sessionStorage.LibraryName == "PIM") {
                $('#divDownloadFilePIM').show();
                $('#divOptionAddProduct').show();
                $('#btnExportToCSV').hide();
            }
            else if (sessionStorage.LibraryName == "Supplier / Vendor List") {
                $('#divOptionAddProduct').show();
            }
            else {
                $('#divDownloadFilePIM').hide();
                $('#divOptionAddProduct').hide();
            }
        }

        function GetSortingItemModifier() {
            AjaxSend({
                url: 'ItemModifiers/GetSortingItemModifier', success: function (e) {
                    var vRows = e.Results;
                    var divIMCatSku = $('#divIMCatSku');

                    divIMCatSku.empty();

                    if (vRows.length > 0) {
                        for (var i = 0; i < vRows.length; i++) {
                            var data = vRows[i];
                            var ul = $('<ul class="list-group" id="ulIMCatSku-' + data.SkuDetails.replace(' ', '') + '" style="cursor:move;">');

                            for (var im = 0; im < data.ItemModifiers.length; im++) {
                                var imdata = data.ItemModifiers[im];

                                ul.append($('<li class="list-group-item">').data('data', imdata)
                                        .append($('<table style="width:100%">')
                                            .append($('<tr>')
                                                .append($('<td style="width:50px;">')
                                                    .text((im + 1) + '.')
                                                )
                                                .append($('<td>')
                                                    .text(imdata.Name)
                                                )
                                            )
                                        )
                                    )
                            }


                            divIMCatSku.append($('<div class="mb-4">')
                                    .append($('<h4>').text(data.SkuName))
                                    .append($('<div class="row mt-3">')
                                        .append($('<div class="offset-sm-1 col-sm-9 col-lg-7 col-xl-4">')
                                            .append(ul)
                                        )
                                    )
                                )

                            SortList('ulIMCatSku-' + data.SkuDetails.replace(' ', ''));
                        }
                    } else {
                        divIMCatSku.append($('<h4>').text('No data available'))
                    }

                    Bs.Loading('hide');

                    $('#btnIMSortingRefresh').unbind('click').click(function () {
                        Bs.Loading('show', 'Refreshing list ...')
                        GetSortingItemModifier();
                    })

                    $('#btnIMSortingSave').unbind('click').click(function () {
                        if (vRows.length > 0) {
                            AjaxSend({
                                url: 'ItemModifiers/SaveSorting',
                                data: {
                                    data: vRows
                                }, success: function (e) {
                                    if (e.Results == 'saved') {
                                        NewMessageBox('Successfully saved');
                                        GetSortingItemModifier();
                                    } else {
                                        NewMessageBox(e.Results, 'Error');
                                    }
                                }
                            })
                        } else {
                            NewMessageBox('There is no data to save');
                        }
                    })
                }
            })
        }

        function SortList(id) {
            setTimeout(function (e) {
                $('#' + e).sortable({
                    list: '#' + e,
                    delay: 150,
                    start: function (event, ui) {
                        ui.item.parent().css('border', '1px solid #CCC');
                    },
                    stop: function (event, ui) {
                        ui.item.parent().css('border', '1px solid #FFF');
                    },
                    update: function (event, ui) {
                        var id = ui.item.parent().attr('id');
                        var newOrderNo = 1;
                        $('#' + id).find('> li').each(function () {
                            $(this).data('data').OrderNo = newOrderNo;
                            $(this).find('td:eq(0)').text(newOrderNo + '.');
                            newOrderNo += 1;
                        })
                    }
                });
                //$('#' + e).disableSelection();
            }, 100, id)
        }

        var tblFranchiseeRequests;

        function GetFranchiseeRequests() {
            var url = '/Franchisee/GetAllRequests?OrgID=' + sessionStorage.OrgID + '&status=' + $('#selStatusFranchiseeRequests option:selected').val()
            var txtSearchFranchiseeRequests = $('#txtSearchFranchiseeRequests').val();
            var ctrNew = null;

            if (CStr2(txtSearchFranchiseeRequests) != '') {
                $('#tblFranchiseeRequests').DataTable().ajax.url(url)
                tblFranchiseeRequests.search($.trim(txtSearchFranchiseeRequests)).draw();
                return;
            }

            $('#tblFranchiseeRequests tbody').empty()

            if ($.fn.DataTable.isDataTable("#tblFranchiseeRequests")) {
                //check if datatable is created 
                $('#tblFranchiseeRequests').DataTable().clear().destroy();
            }

            tblFranchiseeRequests = $('#tblFranchiseeRequests').DataTable({
                retrieve: true,
                "bdestroy": true,
                "pageLength": 50,
                "order": [[1, "asc"]],
                "bProcessing": true,
                "bServerSide": true,
                "ajax": {
                    url: url,
                    type: 'POST'
                },
                "autoWidth": false,
                "columns": [
                    {
                        "data": function (data, type, full, meta) {
                            return data.RowNum + '.';
                        },
                        "width": "30px"
                    },
                    {
                        "data": function (data, type, full, meta) {
                            return data.CompanyName;
                        }
                    },
                    {
                        "data": function (data, type, full, meta) {
                            return data.StoreName;
                        }
                    },
                    {
                        "data": function (data, type, full, meta) {
                            return data.Administrator;
                        }
                    },
                    {
                        "data": function (data, type, full, meta) {
                            return $.format.date(new Date(CJsonDate(data.ApplicationDate)), 'MM-dd-yyyy hh:mm a');
                        }
                    },
                    {
                        "data": function (data, type, full, meta) {
                            if (data.IsApproved == undefined) {
                                ctrNew = CDec2(ctrNew) + 1;
                                return 'New';
                            } else if (CDec2(data.IsApproved) == 1) {
                                return 'Approved by ' + data.UpdatedByName + ' on ' + $.format.date(new Date(CJsonDate(data.LastUpdated)), 'MM-dd-yyyy hh:mm a');
                            } else if (CDec2(data.IsApproved) == 0) {
                                return 'Rejected by ' + data.UpdatedByName + ' on ' + $.format.date(new Date(CJsonDate(data.LastUpdated)), 'MM-dd-yyyy hh:mm a');
                            }
                        }
                    },
                    {
                        "data": null,
                        createdCell: function (td, cellData, rowData, row, col) {
                            if (!CBool(rowData.IsApproved)) {
                                return '';
                            }

                            var str = CBool(rowData.IsDBCreated) ? '' : '<button type="button" class="btn btn-link">Create Database</button>';

                            $(td).addClass('text-align')
                                .data('data', rowData)
                                .html(str)
                                .click(function (e) {
                                    e.stopPropagation();
                                    var data = $(this).data('data');
                                    
                                    AjaxSend({
                                        url: '/Franchisee/GetDetails',
                                        data: { data: data },
                                        success: function (e) {
                                            var admin = e.Admin[0];

                                            data.Franchisor = sessionStorage.OrgName;
                                            data.AdminEmailAdd = admin.EmailAddress;
                                            data.AdminName = admin.FirstName + ' ' + admin.Surname;
                                            data.LoggedInUser = sessionStorage.CurrentUserFullName;

                                            AjaxSend({
                                                url: '/Franchisee/Approve',
                                                data: { data: data },
                                                success: function (e) {
                                                    Bs.Loading('hide')

                                                    if (e.Results == 'saved') {
                                                        NewMessageBox('Successfully approved!');
                                                        GetFranchiseeRequests();
                                                        LibraryTable.Fowst();
                                                    } else {
                                                        NewMessageBox(e.Results);
                                                    }
                                                }
                                            })
                                        }
                                    })
                                })
                        }
                    },
                ],
                "createdRow": function (row, data, index) {
                    $(row).data('data', data).click(function () {
                        var data = $(this).data('data');
                        
                        AjaxSend({
                            url: '/Franchisee/GetDetails',
                            data: { data: data },
                            success: function (e) {
                                var company = e.Company[0];
                                var store = e.Store[0];
                                var admin = e.Admin[0];

                                $('#lblFRCompanyName').text(company.CompanyName);
                                $('#lblFRCompanyTIN').text(company.TinNumber);
                                $('#lblFRBusinessIndustry').text(company.BusinessIndustry);
                                $('#lblFRCompanyEmailAddress').text(company.EmailAddress);
                                $('#lblFRCompanyMobileNo').text('0' + company.MobileNo);
                                $('#lblFRCompanyAddress').text(company.OfficeAddressBuildingNo + ', ' + company.OfficeAddressStreet + ', ' + company.CityName + ', ' + company.CountryName);

                                $('#lblFRStoreName').text(store.Name);
                                $('#lblFRStoreBusinessTradeName').text(store.BusinessTradeName);
                                $('#lblFRStoreAddress').text(store.StreetNumber + ', ' + store.Street + ', ' + store.CityName + ', ' + store.CountryName);

                                $('#lblFRAdminName').text(admin.FirstName + ' ' + admin.Surname);
                                $('#lblFRAdminEmailAddress').text(admin.EmailAddress);

                                if (CBool(data.IsApproved)) {
                                    $('#btnFRDisapprove, #btnFRApprove').parent().hide();
                                } else {
                                    $('#btnFRDisapprove, #btnFRApprove').parent().show();
                                }

                                $('#popupFranchiseeReqDetails').modal('show');

                                $('#btnFRApprove').unbind('click').click(function () {
                                    NewMessageBox('Are you sure you want to approve <b>' + company.CompanyName + '</b> as your franchisee? <br><br>' +
                                    'If you click YES: <br>' +
                                    '- <b>' + company.CompanyName + '</b> will be added to your Customer lists <br>' +
                                    '- It will create its own database <br>'
                                    //'- Administrator <b>' + admin.FirstName + ' ' + admin.Surname + '</b> will be added to your Employee lists'
                                    , 'Confirmation Dialog', [
                                        {
                                            Text: 'Yes', Click: function () {
                                                $('#popupFranchiseeReqDetails').modal('hide');
                                                Bs.Loading('show');

                                                data.Franchisor = sessionStorage.OrgName;
                                                data.AdminEmailAdd = admin.EmailAddress;
                                                data.AdminName = admin.FirstName + ' ' + admin.Surname;
                                                data.LoggedInUser = sessionStorage.CurrentUserFullName

                                                AjaxSend({
                                                    url: '/Franchisee/Approve',
                                                    data: { data: data },
                                                    success: function (e) {
                                                        Bs.Loading('hide')

                                                        if (e.Results == 'saved') {
                                                            NewMessageBox('Successfully approved!');
                                                            GetFranchiseeRequests();
                                                            LibraryTable.Fowst();
                                                        } else {
                                                            NewMessageBox(e.Results);
                                                        }
                                                    }
                                                })
                                            }
                                        }, { Text: 'No, maybe next time' }
                                    ])
                                })

                                $('#btnFRDisapprove').unbind('click').click(function () {
                                    NewMessageBox('Are you sure you want to reject <b>' + store.Name + '</b> as your franchisee?', 'Confirmation Dialog', [
                                        {
                                            Text: 'Yes', Click: function () {
                                                Bs.Loading('show');

                                                AjaxSend({
                                                    url: '/Franchisee/Disapprove',
                                                    data: { data: data },
                                                    success: function (e) {
                                                        Bs.Loading('hide')

                                                        if (e.Results == 'saved') {
                                                            NewMessageBox('Successfully rejected!');
                                                            GetFranchiseeRequests();
                                                        } else {
                                                            NewMessageBox(e.Results);
                                                        }
                                                    }
                                                })
                                            }
                                        }, { Text: 'No, maybe next time' }
                                    ])
                                })

                                if (sessionStorage.FRRepopulateData) {
                                    $('#btnFRRepopulateData').parent().show();
                                    $('#btnFRRepopulateData').unbind('click').click(function () {
                                        Bs.Loading('show');

                                        data.Franchisor = sessionStorage.OrgName;
                                        data.AdminEmailAdd = admin.EmailAddress;
                                        data.AdminName = admin.FirstName + ' ' + admin.Surname;
                                        data.LoggedInUser = sessionStorage.CurrentUserFullName

                                        AjaxSend({
                                            url: '/Franchisee/Repopulate',
                                            data: { data: data },
                                            success: function (e) {
                                                Bs.Loading('hide')

                                                if (e.Results == 'saved') {
                                                    NewMessageBox('Successfully approved!');
                                                    GetFranchiseeRequests();
                                                    LibraryTable.Fowst();
                                                } else {
                                                    NewMessageBox(e.Results);
                                                }
                                            }
                                        })
                                    })
                                }
                            }
                        })
                    })
                },
                "aoColumnDefs": [{ "bSortable": false, "aTargets": [0] }],
                rowReorder: {
                    selector: 'td:nth-child(1)'
                },
                "responsive": true,
                "columnDefs": [{ "targets": 0, "orderable": true }],
                "sDom": '<"top">rt<"bottom"ip><"clear">',
                "initComplete": function () {
                    $('#tblFranchiseeRequests').removeClass('dataTable');
                    if (ctrNew != null) {
                        $('#ctrnewfranchiseereq').text(ctrNew);
                    } else {
                        $('#ctrnewfranchiseereq').text('');
                    }
                }
                //"stateSave": true
            })
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
                else if (sessionStorage.AddViewLibBCName == "Dormant SKUs Settings") {
                    LibraryTable.DormantSkusSettings();
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
            } else if (sessionStorage.LibraryName == 'ERP Settings') {
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
            } else if (sessionStorage.LibraryName == "Auto Data Archiving") {
                window.location.hash = 'UniversalPage';
                LibraryTable.AutoDataArchivingPage();
            } else if (sessionStorage.LibraryName == "WebPOS Settings") {
                window.location.hash = 'UniversalPage';
                LibraryTable.WebPosSettingsPage();
            } else if (sessionStorage.LibraryName == "General Settings") {
                window.location.hash = 'UniversalPage';
                LibraryTable.GeneralSettingsPage();
            }
            else if (sessionStorage.LibraryName == "VDM General Settings") {
                window.location.hash = 'UniversalPage';
                LibraryTable.VDMGeneralSettingsPage();
            }
            else if (sessionStorage.LibraryName == "VDM Physical Button Settings") {
                window.location.hash = 'UniversalPage';
                LibraryTable.VDMPhysicalButtonSettingsPage();
            }
            else if (sessionStorage.LibraryName == "VDM Rice Dispensing Guides") {
                window.location.hash = 'UniversalPage';
                LibraryTable.VDMRiceDispensingGuidesPage();
            }
            else if (sessionStorage.LibraryName == "VDM Product Selection Guides") {
                window.location.hash = 'UniversalPage';
                LibraryTable.VDMProductSelectionGuidesPage();
            }
            else if (sessionStorage.LibraryName == "VDM Ads") {
                window.location.hash = 'UniversalPage';
                LibraryTable.VDMAdsPage();
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

                    AjaxSend({
                        url: 'Organization/getOrgTradeNameList',
                        success: function (vData) {

                            var vRows = vData.OrganizationData.Rows;

                            $('#selFOWSTUploadTradeName').empty();

                            for (var p = 0; p <= vRows.length - 1; p++) {
                                $('#selFOWSTUploadTradeName').append(new Option(vRows[p].BusinessTradeName, vRows[p].ID, false, false));
                            }
                        },
                        error: function (vError) {
                            var pDummy;
                        }
                    });
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
                                                {
                                                    Text: 'Ok'
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
                                        {
                                            Text: 'Ok'
                                        }
                                    ]
                            );
                    return;
                }

            });

            if (sessionStorage.OrgID == '892f84da-9ca1-42bf-af3d-a7ddcb9bc4ba') { //PotatoCorner Franchisee
                $('#butVouchers').parents().closest('.row').hide();
            } else {
                $('#butVouchers').parents().closest('.row').show();
            }
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
                                                    {
                                                        Text: 'Ok'
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
                                        {
                                            Text: 'Ok'
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
                                                {
                                                    Text: 'Ok'
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
                                        {
                                            Text: 'Ok'
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

        function ShowHideLibraries() {

            AjaxSend({
                url: 'SecurityRole/GetModulesWithNoAccess',
                success: function (vData) {

                    vRows = vData.Results.Rows

                    for (i = 0; i <= vRows.length - 1; i++) {

                        if (vRows[i].ModuleName == 'Company Profile') {
                            $('#btnSetupOrganization').parents().closest('.row').remove();
                        } else if (vRows[i].ModuleName == 'Company Places (FOWST)') {
                            $('#btnSetupOrganizationFOWST').parents().closest('.row').remove();
                        }
                        else if (vRows[i].ModuleName == 'Allow access to the list of devices') {
                            $('#btnSetupDevice').parents().closest('.row').remove();
                        }
                        else if (vRows[i].ModuleName == 'POS Receipt Configuration') {
                            $('#btnSetupReceiptSettings').parents().closest('.row').remove();
                        }
                        else if (vRows[i].ModuleName == 'Security Roles') {
                            $('#btnSetupSecurity').parents().closest('.row').remove();
                        }
                        else if (vRows[i].ModuleName == 'Company Employee') {
                            $('#btnSetupEmployee').parents().closest('.row').remove();
                        }
                        else if (vRows[i].ModuleName == 'PIM Settings') {
                            $('#btnSetupProducts').parents().closest('.row').remove();
                        }
                        else if (vRows[i].ModuleName == 'Company Customer') {
                            $('#btnSetupCustomer').parents().closest('.row').remove();
                        }
                        else if (vRows[i].ModuleName == 'Company Supplier') {
                            $('#btnSetupSupplier').parents().closest('.row').remove();
                        }
                        else if (vRows[i].ModuleName == 'Printers') {
                            $('#btnPrinter').parents().closest('.row').remove();
                        }
                        else if (vRows[i].ModuleName == 'Brands Library') {
                            $('#btnSetupProductBrands').parents().closest('.row').remove();
                        }
                        else if (vRows[i].ModuleName == 'Product Categories') {
                            $('#btnSetupProductCategories').parents().closest('.row').remove();
                        }
                        else if (vRows[i].ModuleName == 'Sales Return Settings') {
                            $('#btnSalesReturnSettings').parents().closest('.row').remove();
                        }
                        else if (vRows[i].ModuleName == 'Discount Settings') {
                            $('#btnDiscountSettings').parents().closest('.row').remove();
                        }
                        else if (vRows[i].ModuleName == 'Printer Roles') {
                            $('#btnPrinterRoles').parents().closest('.row').remove();
                        }
                        else if (vRows[i].ModuleName == 'General Settings') {
                            $('#btnGeneralSettings').parents().closest('.row').remove();
                        }
                        else if (vRows[i].ModuleName == 'Decimal in Quantity Settings') {
                            $('#btnQuantitySetting').parents().closest('.row').remove();
                        }
                        else if (vRows[i].ModuleName == 'POS Settings') {
                            $('#btnPOSSetting').parents().closest('.row').remove();
                        }
                        else if (vRows[i].ModuleName == 'Sales Order Settings') {
                            $('#btnSalesOrderSetting').parents().closest('.row').remove();
                        }
                        else if (vRows[i].ModuleName == 'Receiving and Transfer-Out Settings') {
                            $('#btnReceivingAndTransferOutSetting').parents().closest('.row').remove();
                        }
                        else if (vRows[i].ModuleName == 'Inventory Count Settings') {
                            $('#btnInventoryCountSetting').parents().closest('.row').remove();
                        }
                        else if (vRows[i].ModuleName == 'Purchase Order Settings') {
                            $('#btnPurchaseOrderSetting').parents().closest('.row').remove();
                        }
                        else if (vRows[i].ModuleName == 'Daily Beginning Balance Settings') {
                            $('#btnDailyBeginningBalanceSetting').parents().closest('.row').remove();
                        }
                        else if (vRows[i].ModuleName == 'Product Maker Settings') {
                            $('#butProductMakerSettings').parents().closest('.row').remove();
                        }
                        else if (vRows[i].ModuleName == 'Print Settings') {
                            $('#butPrintSettings').parents().closest('.row').remove();
                        }
                        else if (vRows[i].ModuleName == 'Company Supplier') {
                            $('#btnSetupSupplier').parents().closest('.row').remove();
                        }
                        else if (vRows[i].ModuleName == 'Hot Buttons Settings') {
                            $('#btnHotButtons').parents().closest('.row').remove();
                        }
                        else if (vRows[i].ModuleName == 'Group Settings') {
                            $('#btnGroupSettings').parents().closest('.row').remove();
                        }
                        else if (vRows[i].ModuleName == 'Company Supplier') {
                            $('#btnRegionalGroupSettings').parents().closest('.row').remove();
                        }
                        else if (vRows[i].ModuleName == 'Payment Terms') {
                            $('#btnPaymentTerms').parents().closest('.row').remove();
                        }
                        else if (vRows[i].ModuleName == 'Sales Representatives') {
                            $('#butSysAdminSalesRep').parents().closest('.row').remove();
                        }
                        else if (vRows[i].ModuleName == 'Proprietary Barcode Types') {
                            $('#btnProprietaryBarcodeTypes').parents().closest('.row').remove();
                        }
                        else if (vRows[i].ModuleName == 'Auto Data Archiving') {
                            $('#butSystemAdministratorAutoDataArchiving').parents().closest('.row').remove();
                        }
                        else if (vRows[i].ModuleName == 'Item Modifiers') {
                            $('#btnItemModifiers').parents().closest('.row').remove();
                        } 
                        else if (vRows[i].ModuleName == 'Account Adjustments') {
                            $('#btnAccountAdjustments').parents().closest('.row').remove();
                        }
                        else if (vRows[i].ModuleName == 'Regional Group Settings') {
                            $('#btnRegionalGroupSettings').parents().closest('.row').remove();
                        }
                        else if (vRows[i].ModuleName == 'Sales Team') {
                            $('#btnSalesTeam').parents().closest('.row').remove();
                        }
                        else if (vRows[i].ModuleName == 'Price Levels') {
                            $('#btnPriceLevel').parents().closest('.row').remove();
                        }
                        else if (vRows[i].ModuleName == 'Vouchers') {
                            $('#butVouchers').parents().closest('.row').remove();
                        }
                        else if (vRows[i].ModuleName == 'GPS Settings') {
                            $('#btnGPSSettings').parents().closest('.row').remove();
                        }
                    }

                    if ($('#divForRequiredData > .row:not(.d-none)').length == 0) {
                        $('#divForRequiredData').remove();
                    }

                    if ($('#divForOptionalData > .row:not(.d-none)').length == 0) {
                        $('#divForOptionalData').remove();
                    }

                    if ($('#divForSystemBehavior > .row:not(.d-none)').length == 0) {
                        $('#divForSystemBehavior').remove();
                    }

                    $('.section:eq(0)').remove();
                    $('.section:eq(0)').removeClass('d-none');
                }
            });
            //$('#butErpSettings').parents().closest('.row').remove();
            //$('#btnTableViewSetting').parents().closest('.row').remove();
            //$('#btnGPSSettings').parents().closest('.row').remove();
            //$('#btnSetupSupplier').parents().closest('.row').remove();                 
            $('#divForSystemAdministratorQS4VVLQPY2').remove();
            //$('#btnSalesTeam').parents().closest('.row').remove();
            //$('#btnItemModifiers').parents().closest('.row').remove();
            //$('#btnExternalServices').parents().closest('.row').remove();
            //$('#btnPaymentTerms').parents().closest('.row').remove();
            //$('#btnAccountAdjustments').parents().closest('.row').remove();
            //$('#btnPriceLevel').parents().closest('.row').remove();
            //$('#butSysAdminSalesRep').parents().closest('.row').remove();
            //$('#butUpdateServerIp').parents().closest('.row').remove();
            //$('#butWebPosSettings').parents().closest('.row').remove();
            
            $('#divForAccounting').remove();
            $('#divForEcommerce').remove();
            $('#divForSystemAdministratorReports').remove();
            $('#divForSystemAdministrator').remove();
            //$('#divSecond').remove();
            
        }
    });
});