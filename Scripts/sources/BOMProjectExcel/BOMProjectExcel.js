require([
    'common'
], function () {

    require([
        '/Scripts/jquery.dataTables.min.js',
        'promise!/Scripts/jquery-ui-sortable.js'
    ], function () {

        var isPopupOpen = false;
        var isCanvassMaterials = true;
        var isAllowedToEditLine = true;

        $(document).ready(function () {
            var currentProjectName, currentProjectAddress;
            var setDateCal;
            var newVal;
            var itemno = 1;
            var currInputVal;
            var indentno = 0;
            var rowIDOfParent = "";
            var divid = "";
            var descriptionid = "";
            var child = "";
            var isFocusOut = false;
            var countParent = 0;
            var parentid = "";
            var itemid = "";
            var isparent = "";
            var currentCellIndex;
            var setDateTo;
            var NameOfBtnForLibrary;
            var intTelInfoCtr = 0;
            var intMobileInfoCtr = 0;
            var intEmailInfoCtr = 0;
            var TelState = "";
            var TelRankNo = "";
            var MobileState = "";
            var MobileRankNo = "";
            var EmailState = "";
            var EmailRankNo = "";
            var posAddOption;
            var isAddProject = true;
            var itemAssociateSKUID = "";
            var isHasAssociateSKUID = true;
            var isProjectBudgetDetails = false;
            var isCanvass = false;
            var isCanvassMaterialsAlready = false;
            var isCanvassLaborAlready = false;
            var isCreateNewProjectBudget = true;
            var isFinishedGettingCanvassMaterials = false;
            var isFinishedGettingCanvassLabor = false;
            var isCanvassForm = false;
            var isAddChildMultiple = false;
            var isPrintCanvass = false;
            var whatArr;
            var isFirstCountParent = true;
            var isNoResultSearchOrgPeople = false;
            var isAddNewVendorCanvassMaterials = false;
            var isAddNewVendorCanvassLabor = false;
            var isFinishLoadCanvassMaterials = false;
            var isFinishLoadCanvassLabor = false;
            var isOnCanvassPage = false;
            var isFinishedLoadTimeline = false;
            var isFinishedLoadJobRolePermission = false;
            var isAddChildType = '';
            var currentLineDescription = '';

            var arrProjectBudget = [];
            var arrOwnerList = [];
            var arrManagerList = [];
            var arrAsstManagerList = [];
            var arrTelInfoList = [];
            var arrMobileInfoList = [];
            var arrEmailInfoList = [];
            var arrCanvassMaterialsOrgPeople = [];
            var arrCanvassLaborOrgPeople = [];
            var arrChildDescription = [];
            var arrCanvassMaterials = [];
            var arrCanvassLabor = [];
            var arrCanvassMinPrice;
            var objCanvassMinPrice;
            var groupCanvassMinPrice;
            var arrBackUpCanvassMaterial = [];
            var arrBackUpCanvassLabor = [];
            var arrCanvassMaterialsVendor = [];
            var arrCanvassLaborVendor = [];
            var arrSelectedSKU = {};
            var arrTimeline = [];
            var arrProjectMember = [];
            var arrDefineRole = [];
            var arrTempDefineRole = [];
            var arrRolePermissions = [];
            var arrAccessRights = [];
            var arrPermissions = [{ 'Description': 'Add/Edit Labor Costing', 'ID': '71d865e5-77eb-41f2-ab97-e4abe94cb1de' },
                                    { 'Description': 'Add/Edit Materials Costing', 'ID': '352da83a-7667-44a9-8188-83900966ddcd' },
                                    { 'Description': 'Add/Edit Timeline', 'ID': '2049dbac-dee3-4f69-94dc-45b944e1d0f8' },
                                    { 'Description': 'Approve Cash Advances/Disbursements', 'ID': '68ef326f-9ec7-41dc-aed5-3b3290eb586c' },
                                    { 'Description': 'Approve Materials Requisition', 'ID': '8f656260-8b95-4427-87d3-376604d63fc8' },
                                    { 'Description': 'Edit Line Items In Project', 'ID': '670cc1f0-1b67-431f-93de-e3d027a87702' },
                                    { 'Description': 'Make Cash Advances/Disbursements', 'ID': 'bb19590a-2899-48e5-b770-37c6767b0b56' },
                                    { 'Description': 'Make Materials Requisition', 'ID': '7149b0ab-de2c-47c9-a143-3ad8bc142092' },
                                    { 'Description': 'Make Purchase Orders', 'ID': '0a2d199c-ae86-4def-852d-3ff9abc6ce2d' },
                                    { 'Description': 'Make Stock Transfers', 'ID': '26a24bf4-f2a6-4a14-ac9d-61485f37eb70' },
                                    { 'Description': 'Print Project Budget', 'ID': '2853ff55-e4e3-4456-a7dd-1032b39b6c26' }];

            var breadcrumbs = $('<div class="ItemsBreadCrumbs"></div>'),
                arrowR = ' <span class="ui-icon ui-icon-arrow-r" style="display:inline-block; margin:0px 10px 0px 10px;"></span>',
                lnkProject = '<a data-role="none" class="btnPageProjectList">Projects</a>';

            var htmlMaterialsReq = '', htmlPO = '', isMaterials = false, selPointPerson;
            var projectData = new FormData(), projectFilesFormData = new FormData(), arrProjectFiles = [];
            var isOnDrag = false;

            LoadRequiredFunctions();

            var pos = $('#divButtons').offset();
            $(window).resize(function () {
                AdjustContainerMarginTop();
                RefreshUnfreezeContent();
            })
            .scroll(function () {
                var windowpos = $(window).scrollTop();

                if ($(this).scrollTop()) {
                    $('.btnBackToTop').fadeIn();
                } else {
                    $('.btnBackToTop').fadeOut();
                }

                if (CBool(isProjectBudgetDetails)) {
                    // if (windowpos > pos.top) {
                    //     $('#divButtons').addClass("divButtonspositionFixed").css('top', $('#divHeaderForProjects').height());
                    // } else {
                    //     $('#divButtons').removeClass("divButtonspositionFixed"); 
                    // }
                }
            });

            setDateCal = $(".SetDateCalendar").calendar({
                format: "mmmm dd, yyyy",
                multiSelect: false,
                otherDays: true,
                getDates: function (data) {
                    $("#txtSetDate").text(data);
                }
            });

            window.RepositionPopup = function () {
                Reposition();
            }

            window.CheckPermissions = function (desc) {
                return CheckPermissions(desc);
            }

            $('#tBodyExcelLike').sortable({
                list: '#tBodyExcelLike',
                handle: '.tddraggable',
                delay: 150,
                start: function (event, ui) {
                    isOnDrag = true;
                    ui.item.css('border', '1px solid #CCC');
                    $('#tBodyExcelLike tr:last').hide();
                },
                stop: function (event, ui) {
                    isOnDrag = false;
                    ui.item.css('border', '1px solid #FFF');
                    $('#tBodyExcelLike tr:last').show();
                },
                update: function (event, ui) {
                    var tr = ui.item;
                    var newindentno = Math.max(tr.prev().find('.tddescription > div').attr('indentno'), tr.next().find('.tddescription > div').attr('indentno'))
                    tr.find('.tddescription > div').attr('indentno', newindentno);
                    ReGenerateItemNo();
                    UpdateRowIDs();
                }
            });

            $('#divProjectBudgetSummary').attachDragger();
            $('#SubdivNewProjectBudget').attachDragger({
                onDrag: function () {
                    return isOnDrag;
                }
            });
            $('#divCanvassMaterials').attachDragger();
            $('#divCanvassLabor').attachDragger();
            $('#divTimelineContainer').attachDragger();
            $('#divContentDefineRole').attachDragger();

            $('#tBodyExcelLike tr:eq(0)').addClass('activeRow');
            $('#tBodyExcelLike td:eq(2)').addClass('activeCell');

            $('#tBodyProjectBudget').on('contextmenu', 'td', function (e) {
                e.preventDefault();
                e.stopPropagation();
                $('.context-menu').remove();

                $('body').append($('<div>')
                        .append($('<ul>')
                            .append($('<a>')
                                .append($('<li>')
                                    .html('Edit This Document')
                                    .click(function (e) {
                                        e.stopPropagation();
                                        $('.context-menu').remove();
                                        $('#btnNewProjectBudget').click();
                                    })
                                )
                            )
                        )
                        .addClass('context-menu')
                        .css({ top: e.pageY + "px", left: e.pageX + "px" })
                    )
            })

            $('#tBodyExcelLike').on('contextmenu', '.tddescription', function (e) {
                e.preventDefault();
                e.stopPropagation();

                rowIDOfParent = $(this).parent().attr('id');
                divid = $(this).find('div').attr('id');
                parentid = $(this).find('div').attr('parentid');
                itemid = $(this).find('div').attr('itemid');
                isparent = $(this).find('div').attr('isparent');
                indentno = CDec2($(this).find('div').attr('indentno'));
                descriptionid = $(this).find('div').attr('descriptionid');
                child = "";
                isMaterials = CBool($(this).find('div').hasClass('rowMaterials'));

                if ($(this).has('div').length == 1 && $(this).parent().attr('rowType') == "normal") { //&& descriptionid != undefined
                    ContextMenu(this, e, divid, child);
                }
            })

            $('#tBodyExcelLike').on('contextmenu', '.tdremarks', function (e) {
                e.preventDefault();
                e.stopPropagation();

                rowIDOfParent = $(this).parent().attr('id');
                ContextMenuRemarks(this, e);
            })

            $('#tBodyExcelLike').on('contextmenu', '.tddependency', function (e) {
                e.preventDefault();
                e.stopPropagation();

                rowIDOfParent = $(this).parent().attr('id');
                ContextMenuTaskDependency(this, e);
            })

            $('#tBodyExcelLike').on('contextmenu', '.tdbudgetedduration', function (e) {
                e.preventDefault();
                e.stopPropagation();

                rowIDOfParent = $(this).parent().attr('id');
                ContextMenuBudgetedDuration(this, e);
            })

            $('#tBodyExcelLike').on('contextmenu', '.tddatestarted, .tddatecompleted', function (e) {
                e.preventDefault();
                e.stopPropagation();

                rowIDOfParent = $(this).parent().attr('id');
                ContextMenuSetDate(this, e);
            })

            $('#tBodyExcelLike').on('contextmenu', '.tdbudgetedqty', function (e) {
                e.preventDefault();
                e.stopPropagation();

                rowIDOfParent = $(this).parent().attr('id');
                ContextMenuBudgetedQty(this, e);
            })

            $('#tBodyExcelLike').on('contextmenu', '.tdreqqty, .tdaccreqqty', function (e) {
                e.preventDefault();
                e.stopPropagation();

                rowIDOfParent = $(this).parent().attr('id');
                ContextMenuAccReqQty(this, e);
            })

            $('#tBodyExcelLike').on('contextmenu', '.tdaccusedup', function (e) {
                e.preventDefault();
                e.stopPropagation();

                rowIDOfParent = $(this).parent().attr('id');
                ContextMenuAccUsedUp(this, e);
            })

            $('#tBodyExcelLike').on('contextmenu', '.tdlaborcost', function (e) {
                e.preventDefault();
                e.stopPropagation();

                rowIDOfParent = $(this).parent().attr('id');
                ContextMenuBudgetedLaborCost(this, e);
            })

            $('#tBodyExcelLike').on('contextmenu', '.tdaccreqlaborcost', function (e) {
                e.preventDefault();
                e.stopPropagation();

                rowIDOfParent = $(this).parent().attr('id');
                ContextMenuAccLaborCost(this, e);
            })

            $('#tBodyCanvassMaterial').on('contextmenu', '.tableCell', function (e) {
                e.preventDefault();
                $('.context-menu').remove();
                var activeCell = $(this);
                var rowID = $(this).parent().attr('id');
                $('#tBodyCanvassMaterial tr td').removeClass('activeCell');
                $('#tBodyCanvassMaterial tr').removeClass('activeRow');
                activeCell.addClass('activeCell');
                activeCell.parent().addClass('activeRow');
                if ($(this).hasClass('ReadOnly') == false && $(this).find('div').attr('columnName') == "cost") {
                    $('body').append($('<div>')
                            .append($('<ul>')
                                .append($('<a>')
                                    .append($('<li>')
                                        .html('Set as Preferred Vendor')
                                        .click(function (e) {
                                            e.stopPropagation();
                                            var selector;

                                            if (isCanvassMaterials) {
                                                selector = $('#tBodyCanvassMaterial');
                                            }
                                            else {
                                                selector = $('#tBodyCanvassLabor');
                                            }

                                            selector.find('tr[id="' + rowID + '"] td').removeClass('preferredVendor').removeClass('minPrice').removeAttr('isPreferredCanvass');
                                            selector.find('tr td.activeCell').attr('isPreferredCanvass', '1').addClass('preferredVendor');
                                            $('.context-menu').remove();
                                            SetColorMinPrice();
                                        })
                                    )
                                )
                            )
                            .addClass('context-menu')
                            .css({ top: e.pageY + "px", left: e.pageX + "px" })
                        )
                }
                else if ($(this).hasClass('ReadOnly') == true && $(this).find('div').attr('columnName') == "description") {
                    $('body').append($('<div>')
                            .append($('<ul>')
                                .append($('<a>')
                                    .append($('<li>')
                                        .html('<img src="/Content/images/link.png" align="absmiddle"> &nbsp;Link this item to a specific SKU')
                                        .click(function (e) {
                                            e.stopPropagation();
                                            $('.context-menu').remove();
                                            itemAssociateSKUID = $('#tBodyCanvassMaterial tr td.activeCell').find('div').attr('descriptionid');
                                            isPopupOpen = true;
                                            isHasAssociateSKUID = true;
                                            BrowsePopup('LibraryPIMProduct', 'List of SKUs', [
                                                {
                                                    Message: 'Please choose 1 specific product or service (SKU) to associate with ' + $('#tBodyCanvassMaterial tr td.activeCell').find('div').text(),
                                                    DisplayText: [{ Column: 'ProductName' }],
                                                    Click: function (e) {
                                                        arrSelectedSKU = {};
                                                        arrSelectedSKU.AssociateSKUID = e.ID;
                                                        arrSelectedSKU.SKUName = e.ProductName;

                                                        $('#txtSKUName').text(e.ProductName);
                                                        //$('#panSKUUnitOfMeasures').popup('open');
                                                        SKUUnitOfMeasure(e.ID);
                                                    }
                                                }
                                            ])                                            
                                        })
                                    )
                                )
                            )
                            .addClass('context-menu')
                            .css({ top: e.pageY + "px", left: e.pageX + "px" })
                        )
                }
            })

            $('#tBodyCanvassLabor').on('contextmenu', '.tableCell', function (e) {
                e.preventDefault();
                $('.context-menu').remove();
                if ($(this).hasClass('ReadOnly') == false) {
                    var activeCell = $(this);
                    var rowID = $(this).parent().attr('id');

                    $('#tBodyCanvassLabor tr td').removeClass('activeCell');
                    $('#tBodyCanvassLabor tr').removeClass('activeRow');
                    activeCell.addClass('activeCell');
                    activeCell.parent().addClass('activeRow');
                    $('body').append($('<div>')
                            .append($('<ul>')
                                .append($('<a>')
                                    .append($('<li>')
                                        .html('Set as Preferred Vendor')
                                        .click(function (e) {
                                            e.stopPropagation();
                                            var selector;

                                            if (isCanvassMaterials) {
                                                selector = $('#tBodyCanvassMaterial');
                                            }
                                            else {
                                                selector = $('#tBodyCanvassLabor');
                                            }

                                            selector.find('tr[id="' + rowID + '"] td').removeClass('preferredVendor').removeClass('minPrice').removeAttr('isPreferredCanvass');
                                            selector.find('tr td.activeCell').attr('isPreferredCanvass', '1').addClass('preferredVendor');
                                            $('.context-menu').remove();
                                            SetColorMinPrice();
                                        })
                                    )
                                )
                            )
                            .addClass('context-menu')
                            .css({ top: e.pageY + "px", left: e.pageX + "px" })
                        )
                }
            })

            $('.OrganizationNameHeader').text(sessionStorage.OrgName);

            $('body')
            .on('click', '#btnProjectMgmtWork', function () {
                //$('#pageProjectList').pageshow();
            })
            .on('click', '#btnBackProjectBudget', function () {
                $('#pageProjectList').pageshow();
            })
            .on('click', '#btnClearSetDate', function () {
                setDateCal.calendar('unsetDate', $('#txtSetDate').text())
            })
            .on('click', '.btnConstructMenu', function () {
                PopupButtonList('', 'Go To', [
                    {
                        Text: 'Dashboard', Click: function () {
                            $('#pageMainDashboard').pageshow();
                            if (location.pathname.toLowerCase() != '/erp') {
                                $('.ItemsBreadCrumbs').text('Main Dashboard');
                            }
                        }
                    }, {
                        Text: 'Projects List', Click: function () {
                            $('#pageProjectList').pageshow();
                            $('#divSKUMsg').hide();
                            $('.divCanvass').hide();
                            $('#divBtnNewProjectBudget').show();
                            if (location.pathname.toLowerCase() != '/erp') {
                                $('.ItemsBreadCrumbs').text('Projects');
                            }
                            $('#tHeadCanvassMaterialRow, #tBodyCanvassMaterial').empty();
                            $('#tHeadCanvassLaborRow, #tBodyCanvassLabor').empty();
                            isFinishedGettingCanvassMaterials = false;
                            isFinishedGettingCanvassLabor = false;
                            isProjectBudgetDetails = false;
                            arrCanvassMaterialsOrgPeople = [];
                            arrCanvassLaborOrgPeople = [];
                            GetProjectHeader();
                        }
                    }, {
                        Text: 'Materials Requisition', Click: function () {
                            NewMessageBox('This feature is under construction');
                        }
                    }, {
                        Text: 'Cash Advances/Disbursements', Click: function () {
                            NewMessageBox('This feature is under construction');
                        }
                    }, {
                        Text: 'Approvals', Click: function () {
                            NewMessageBox('This feature is under construction');
                        }
                    }, {
                        Text: 'Purchase Orders', Click: function () {
                            NewMessageBox('This feature is under construction');
                        }
                    }, {
                        Text: 'Stock Transfers', Click: function () {
                            NewMessageBox('This feature is under construction');
                        }
                    }, {
                        Text: 'Stock Receiving', Click: function () {
                            NewMessageBox('This feature is under construction');
                        }
                    }, {
                        Text: 'System Administration', Click: function () {
                            window.location.hash = '';
                            $('#pageSystemAdmin').pageshow();
                            $('#divSystemAdDashboard').show();
                            $('#divSecond').hide();
                            $('#UniversalDiv').hide();
                            if (location.pathname.toLowerCase() != '/erp') {
                                $('.ItemsBreadCrumbs').text('System Administration');
                            }
                            RequiredDataSystemAdmin();
                        }
                    }
                ])
            })
            .on('click', '.btnBackToTop', function () {
                $("html, body").animate({ scrollTop: 0 }, 100);
            })

            .on('click', '#btnAddProject', function () {
                isAddProject = true;
                isFinishedLoadJobRolePermission = false;
                arrDefineRole = [];
                arrRolePermissions = [];
                arrProjectMember = [];
                ClearAllFields();
                ClearAllVariables();
                $('#pageProjectList, #divSelectionRole, #divDefineRole').hide();
                $('#pageProjectDetails, #divProjectMember').show();
                $('#txtProjectName').focus();
                $('#btnSaveNewProject').removeAttr('disabled');

                if (location.pathname.toLowerCase() != '/erp') {
                    $('.ItemsBreadCrumbs').text('');
                    breadcrumbs.prepend(arrowR, "New Project");
                    $('.BreadCrumbs').html(breadcrumbs.prepend(lnkProject));
                }                        
            })

            .on('click', '#btnBCProjectBudget', function (e) {
                $('#pageProjectBudget').pageshow();
                $('.divNewProjectBudget').show();
                $('.divCanvass, .divTimeline').hide();
                if (location.pathname.toLowerCase() != '/erp') {
                    $('.ItemsBreadCrumbs').text('');
                    breadcrumbs.prepend(arrowR, "Project Budget");
                    $('.BreadCrumbs').html(breadcrumbs.prepend(lnkProject));
                }                        
            })

            .on('click', '#btnCancelNewProject', function () {
                if (isAddProject) {
                    $('#popupNewProject').modal('hide');
                    //$('#pageProjectList').pageshow();
                    //if (location.pathname.toLowerCase() != '/erp') {
                    //    $('.ItemsBreadCrumbs').text('Projects');
                    //}
                }
                else {
                    $('#popupNewProject').modal('hide');
                    if (location.pathname.toLowerCase() != '/erp') {
                        $('.ItemsBreadCrumbs').text('');
                        breadcrumbs.prepend(arrowR, "Project Budget");
                        $('.BreadCrumbs').html(breadcrumbs.prepend(lnkProject));
                    }                            
                }
            })

            .on('keypress', '#txtClientName', function (e) {
                if (e.which == 13) {
                    NameOfBtnForLibrary = "ProjectCustomer";
                    $('#panSearchProjectCustomer').popup('open');
                    $('#txtSearchProjectCustomer').val($(this).val());
                    SearchOrganization($(this).val());
                    RepositionPopup();
                }
            })

            .on('click', '#btnClientName', function () {
                BrowsePopup('LibraryOrganizationCustomer', 'Select Client', [
                    {
                        DisplayText: [{ Column: 'CustomerName' }],
                        Click: function (e) {
                            $('#txtClientID').val(e.ID);
                            $('#txtClientName').text(e.CustomerName);
                            //GetOrgFowst(e.ID);
                        },
                        AddButton: [
                            {
                                Text: 'Add New Client',
                                Click: function () {
                                    $('#selProjectNewClientType').val('org');
                                    $('#txtProjectNewClientOrgName, #txtProjectNewClientOrgTIN, '+
                                    '#txtProjectNewClientPersonFname, #txtProjectNewClientPersonMname,' +
                                    '#txtProjectNewClientPersonLname, #txtProjectNewClientHomeNo, ' +
                                    '#txtProjectNewClientStreetName, #txtProjectNewClientMobileNo, '+
                                    '#txtProjectNewClientEmail').val('');
                                    $('#selProjectNewClientCity').val('316a03d3-611a-4376-a2b2-7d0dfd7a4fbc');
                                    $('#selProjectNewClientCountry').val('e783ed13-aa98-4cc1-918e-d49b05c0167e');
                                    $('#divProjectNewClientOrg').pageshow();
                                    $('#popupProjectNewClient').modal('show');
                                }
                            }
                        ]
                    }
                ])
                return;
                $('#panSearchProjectCustomer').popup('open');
                $('#txtSearchProjectCustomer').focus().val('');
                $('#btnAddNewProjectCustomer').hide();
                $('#lstOfProjectCustomer').empty();
                RepositionPopup();
            })

            .on('keypress', '#txtSearchProjectCustomer', function (e) {
                if (e.which == 13) {
                    NameOfBtnForLibrary = "ProjectCustomer";
                    $('#btnAddNewProjectCustomer').show();
                    SearchOrganization($(this).val());
                    RepositionPopup();
                }
            })

            .on('click', '#btnAddNewProjectCustomer', function () {
                $('#panSearchProjectCustomer').popup('close');
                $('#popupNewProjectCustomer').modal('show');
                ClearNewProjectCustomerFields();
                GetCityListAll();
                GetCountryListAll();
                isOnCanvassPage = false;
            })

            .on('change', '#selCustomerType', function (e) {
                ClearNewProjectCustomerFields();

                if ($(this).val() == "Organization") {
                    $('#divCustomerOrg').show();
                    $('#divCustomerPerson').hide();
                }
                else {
                    $('#divCustomerOrg').hide();
                    $('#divCustomerPerson').show();
                }
            })

            .on('click', '#btnCustomerNameSave', function () {
                if ($('#selCustomerType').val() == 'Organization') {
                    if ($('#txtProjectCustomerCompanyName').val() == "") {
                        WebToast('Please enter company name', 'top');
                        return false;
                    }

                    if ($.trim($('#txtProjectCustomerTIN').val()) == "" || $.trim($('#txtProjectCustomerTIN').val()).length <= 1) {
                        WebToast('Please enter TIN number', 'top');
                        return false;
                    }

                }
                else {
                    if ($('#txtProjectCustomerFirstName').val() == "") {
                        WebToast('Please enter first name', 'top');
                        return false;
                    }

                    if ($('#txtProjectCustomerMiddleName').val() == "") {
                        WebToast('Please enter middle name', 'top');
                        return false;
                    }

                    if ($('#txtProjectCustomerLastName').val() == "") {
                        WebToast('Please enter last name', 'top');
                        return false;
                    }
                }

                SaveNewProjectCustomer();
            })

            .on('click', '#btnCustomerNameCancel', function () {
                if (isOnCanvassPage) {
                    $('#pageProjectBudget').pageshow();
                    $('#pageProjectDetails').hide();
                    $('#pageProjectBudget, .divCanvass').show();
                }
                else {
                    $('#popupNewProjectCustomer').modal('hide');
                }
            })

            .on('click', '.btnSetDate', function () {
                var numDays = 0;
                setDateTo = $(this).attr('setDateTo');
                SingleCalendar({
                    //MinDate: ($.trim($('#' + $(this).attr('startdate')).text()) == 'Set Date' ? new Date() : new Date($.trim($(this).text()))),
                    Click: function (e) {
                        $('#' + setDateTo).text(e);
                        if (setDateTo != undefined) {
                            if (setDateTo == "ProjStartDate" || setDateTo == "ProjCompDate") {
                                if ($('#ProjStartDate').text() != "Set Date" && $('#ProjCompDate').text() != "Set Date") {
                                    numDays = (new Date($('#ProjStartDate').text()) - new Date($('#ProjCompDate').text())) / (1000 * 60 * 60 * 24);
                                }
                                else {
                                    numDays = 0;
                                }
                                $('#txtProjectedTotalNumberofDays').text(CDec2(Math.abs(numDays)) > 0 ? CDec2(Math.abs(numDays)) + 1 : CDec2(Math.abs(numDays)));
                            }
                            else {
                                if ($('#ActualStartDate').text() != "Set Date" && $('#ActualCompDate').text() != "Set Date") {
                                    numDays = (new Date($('#ActualStartDate').text()) - new Date($('#ActualCompDate').text())) / (1000 * 60 * 60 * 24);
                                }
                                else {
                                    numDays = 0;
                                }
                                $('#txtActualTotalNumberofDays').text(CDec2(Math.abs(numDays)) > 0 ? CDec2(Math.abs(numDays)) + 1 : CDec2(Math.abs(numDays)));
                            }
                        }
                        else {
                            if ($('#ProjStartDate').text() != "Set Date" && $('#ProjCompDate').text() != "Set Date") {
                                numDays = (new Date($('#ProjStartDate').text()) - new Date($('#ProjCompDate').text())) / (1000 * 60 * 60 * 24);
                            }
                            else {
                                numDays = 0;
                            }
                            $('#txtProjectedTotalNumberofDays').text(CDec2(Math.abs(numDays)) > 0 ? CDec2(Math.abs(numDays)) + 1 : CDec2(Math.abs(numDays)));

                            if ($('#ActualStartDate').text() != "Set Date" && $('#ActualCompDate').text() != "Set Date") {
                                numDays = (new Date($('#ActualStartDate').text()) - new Date($('#ActualCompDate').text())) / (1000 * 60 * 60 * 24);
                            }
                            else {
                                numDays = 0;
                            }
                            $('#txtActualTotalNumberofDays').text(CDec2(Math.abs(numDays)) > 0 ? CDec2(Math.abs(numDays)) + 1 : CDec2(Math.abs(numDays)));
                        }
                    },
                    Clear: function (e) {
                        $('#' + setDateTo).text('Set Date');
                    }
                })                      
            })

            .on('click', '#btnOrgFOWSTCity2', function () {
                BrowsePopup('LibraryCities', 'City',
                    [
                        {
                            DisplayText: [{ Column: 'Name' }],
                            Click: function (e) {
                                $('#txtOrgFOWSTCityID2').val(e.ID);
                                $('#txtCityName').text(e.Name);
                            }
                        }
                    ]
                )
            })

            .on('click', '#btnOrgFOWSTCountry2', function () {
                BrowsePopup('LibraryCountries', 'Country',
                    [
                        {
                            DisplayText: [{ Column: 'Name' }],
                            Click: function (e) {
                                $('#txtOrgFOWSTCountryID2').val(e.ID);
                                $('#txtCountryName').text(e.Name);
                            }
                        }
                    ]
                )
            })

            .on('click', '#btnAddOptionPO', function () {
                posAddOption = 'Project Owner';
                $('#btnAddNewPerson').find('a').text(posAddOption);
                $('#panAddOption').popup('open');
            })
            .on('click', '#btnAddOptionPM', function () {
                posAddOption = 'Project Manager';
                $('#btnAddNewPerson').find('a').text(posAddOption);
                $('#panAddOption').popup('open');
            })
            .on('click', '#btnAddOptionAPM', function () {
                posAddOption = 'Assistant Project Manager';
                $('#btnAddNewPerson').find('a').text(posAddOption);
                $('#panAddOption').popup('open');
            })
            .on('click', '#btnAccessRights', function () {
                $('#panAddOption').popup('close');
                $('#panAccessRights').popup('open');
                $('#divCHBAccessRights input[type="checkbox"]').attr('checked', false);//.checkboxradio('refresh');
            })
            .on('click', '#btnCancelAccessRights', function () {
                $('#popDefineRole').modal('hide');
            })
            .on('click', '#btnOkAccessRights', function () {
                $('.divSelectionRole').hide();
                $('.divDefineRole').show();

                if (arrDefineRole.length == 0) {
                    arrDefineRole = [];
                    $('input[name="chbRoleName"]:checked').each(function () {
                        var labelname = $('#' + $(this).attr('id')).prev('label').text();

                        arrDefineRole.push({
                            RoleID: $(this).val(),
                            Label: labelname,
                            ID: ''
                        })
                    })
                }
                else {
                    //check if jobroleid is exist in array
                    $('input[name="chbRoleName"]:checked').each(function () {
                        var labelname = $('#' + $(this).attr('id')).prev('label').text();
                        var RoleID = $(this).attr('value');
                        var isExist = false;

                        var ind = $.map(arrDefineRole, function (obj, index) {
                            if (obj.RoleID == RoleID) {
                                isExist = true;
                                return index;
                            }
                        })

                        if (!CBool(isExist)) {
                            arrDefineRole.push({
                                RoleID: $(this).val(),
                                Label: labelname,
                                ID: ''
                            })
                        }
                        else {
                            arrDefineRole[ind].State = 'add';
                        }
                    })
                }

                DefineUserPermissions();

                //get allowed permissions per job role
                if (isAddProject == false) {
                    if (isFinishedLoadJobRolePermission == false) {
                        isFinishedLoadJobRolePermission = true;
                        GetAllowedPermissionsPerJobRole();
                    }
                }

                $('#panRoleNames').popup('close');
            })

            .on('change', '.chbJobRole', function (e) {
                e.preventDefault();
                var pThis = $(this);

                if (arrDefineRole.length > 0) {
                    var RoleID = pThis.attr('value');

                    if ($('.selMemberJobRole option[value="' + pThis.attr('value') + '"]:selected').length > 0) {
                        if (pThis.is(':checked') == false) {
                            pThis.prop('checked', true);//.checkboxradio('refresh');
                            NewMessageBox('Some members are assigned to this position. If you uncheck this position, you need to assign the member to another job position. Do you want to continue?', '', [
                                    {
                                        Text: 'Yes',
                                        Click: function () {
                                            pThis.prop('checked', false);//.checkboxradio('refresh');
                                            var ind = $.map(arrDefineRole, function (obj, index) {
                                                if (obj.RoleID == RoleID) {
                                                    return index;
                                                }
                                            })

                                            if (ind.length == 0) {
                                                return;
                                            }

                                            arrDefineRole[ind].State = (pThis.is(':checked') == true ? 'add' : 'delete');
                                        }
                                    }, { Text: 'No' }
                            ])
                            return false;
                        }
                    }

                    var ind = $.map(arrDefineRole, function (obj, index) {
                        if (obj.RoleID == RoleID) {
                            return index;
                        }
                    })

                    if (ind.length == 0) {
                        return;
                    }

                    arrDefineRole[ind].State = (pThis.is(':checked') == true ? 'add' : 'delete');
                }
            })

            .on('change', '.chbJobRoleHeader', function (e) {
                if ($(this).is(':checked')) {
                    $('.chbPermission[value="' + $(this).val() + '"]').prop('checked', 1);//.checkboxradio('refresh');
                } else {
                    $('.chbPermission[value="' + $(this).val() + '"]').prop('checked', 0);//.checkboxradio('refresh');
                }
            })

            .on('click', '#btnAddNewPerson', function () {
                if (posAddOption == 'Project Owner') {
                    $('#btnAddProjectOwner').click();
                }
                else if (posAddOption == 'Project Manager') {
                    $('#btnAddProjectManager').click();
                }
                else {
                    $('#btnAddAsstProjectManager').click();
                }
            })

            .on('click', '#btnAddMember', function () {
                BrowsePopup('LibraryPeople', 'Add Member', [
                        {
                            DisplayText: [
                                { Column: 'PeopleName' }
                            ],
                            Validation: function (vData) {
                                var tBody = $('#tbodyProjectMember');
                                var isExist = false;

                                tBody.find('tr').each(function () {
                                    if ($(this).attr('pplid') == vData.ID && ($(this).attr('state') == "edit" || $(this).attr('state') == "add")) {
                                        isExist = true;
                                    }
                                })

                                if (isExist) {
                                    return '<b>' + vData.PeopleName + '</b> already exist';
                                }
                                else {
                                    var ind = $.map(arrProjectMember, function (obj, index) {
                                        if (obj.ID == vData.ID) {
                                            return index;
                                        }
                                    })
                                    if (ind.length > 0) {
                                        if (arrProjectMember[ind].dbID != undefined) {
                                            arrProjectMember[ind].State = "edit";
                                        }
                                        else {
                                            arrProjectMember[ind].State = "add";
                                        }
                                    }
                                }

                                return true;
                            },
                            Click: function (vData) {
                                var ind = $.map(arrProjectMember, function (obj, index) {
                                    if (obj.ID == vData.ID) {
                                        return index;
                                    }
                                })

                                vData.State = 'add';

                                if (ind.length > 0) {
                                    if (arrProjectMember[ind].dbID == undefined) {
                                        arrProjectMember.push(vData);
                                    }
                                }
                                else {
                                    arrProjectMember.push(vData);
                                }

                                AddProjectMember();
                            }
                        }
                ])
            })

            .on('click', '#btnOptionProjectMember', function () {
                PopupButtonList('Options', [
                        {
                            Text: 'Add Member',
                            Click: function () {
                                BrowsePopup('LibraryPeople', 'Add Member', [
                                        {
                                            DisplayText: [
                                                { Column: 'PeopleName' }
                                            ],
                                            Validation: function (vData) {
                                                var tBody = $('#tbodyProjectMember');
                                                var isExist = false;

                                                tBody.find('tr').each(function () {
                                                    if ($(this).attr('pplid') == vData.ID) {
                                                        isExist = true;
                                                    }
                                                })

                                                if (isExist) {
                                                    return '<b>' + vData.PeopleName + '</b> already exist';
                                                }

                                                return true;
                                            },
                                            Click: function (vData) {
                                                arrProjectMember.push(vData);
                                                AddProjectMember();
                                            }
                                        }
                                ])
                            }
                        },
                        {
                            Text: 'Edit List',
                            Click: function () {
                                console.log('edit');
                            }
                        },
                        {
                            Text: 'Define Roles and Permissions for this Project',
                            Click: function () {
                                console.log('edit');
                            }
                        }
                ])
            })

            .on('click', '#btnDefine', function () {
                $('#popDefineRole').modal('show');
                $('.divSelectionRole').show();
                $('.divDefineRole').hide();

                if (arrDefineRole.length > 0) {
                    $('.chbJobRole').prop('checked', false);//.checkboxradio('refresh')
                    for (var i = 0; i < arrDefineRole.length; i++) {
                        if (arrDefineRole[i].State != 'delete') {
                            $('#chbJobRole_' + arrDefineRole[i].RoleID).attr('dbid', arrDefineRole[i].ID).prop("checked", true);//.checkboxradio("refresh");
                        }
                    }
                }
                else {
                    $('.chbJobRole').prop('checked', false);//.checkboxradio('refresh')
                }
                arrTempDefineRole = [];
                arrTempDefineRole = arrDefineRole;
                sessionStorage.TempDefineRole = JSON.stringify(arrTempDefineRole);
            })

            .on('click', '#btnSaveDefineRole', function () {
                TempSaveRolePermission();

                if (arrProjectMember.length == 0) {
                    BrowsePopup('LibraryPeople', 'Add Member', [
                            {
                                DisplayText: [
                                    { Column: 'PeopleName' }
                                ],
                                Validation: function (vData) {
                                    var tBody = $('#tbodyProjectMember');
                                    var isExist = false;

                                    tBody.find('tr').each(function () {
                                        if ($(this).attr('pplid') == vData.ID) {
                                            isExist = true;
                                        }
                                    })

                                    if (isExist) {
                                        return '<b>' + vData.PeopleName + '</b> already exist';
                                    }

                                    return true;
                                },
                                Click: function (vData) {
                                    vData.State = 'add';
                                    arrProjectMember.push(vData);
                                    AddProjectMember();
                                    $('#popDefineRole').modal('hide');
                                }
                            }
                    ])
                }
                else {
                    AddProjectMember();
                    $('#popDefineRole').modal('hide');
                }
            })

            .on('click', '#btnCancelDefineRole', function () {
                $('.divDefineRole').hide();
                $('.divSelectionRole').show();
                arrDefineRole = [];
                arrDefineRole = JSON.parse(sessionStorage.TempDefineRole);
            })

            .on('click', '#btnAddProjectOwner', function () {
                $('#panAddOption').popup('close');
                if ($('#lstProjectOwner li').length == 2) {
                    NewMessageBox('Maximum of 2 Project Owners', 'Project')
                    return false;
                }               
                SearchProjectOwnerManagerAsst('Project Owner');
            })

            .on('click', '#btnAddProjectManager', function () {
                $('#panAddOption').popup('close');
                if ($('#lstProjectManager li').length == 2) {
                    NewMessageBox('Maximum of 2 Project Managers', 'Project')
                    return false;
                }
                SearchProjectOwnerManagerAsst('Project Manager');
            })

            .on('click', '#btnAddAsstProjectManager', function () {
                $('#panAddOption').popup('close');
                if ($('#lstAsstProjectManager li').length == 3) {
                    NewMessageBox('Maximum of 3 Project Assistant Managers', 'Project');
                    return false;
                }
                SearchProjectOwnerManagerAsst('Asst. Project Manager');
            })

            .on('click', '#btnAddOrgFOWSTTelephone2', function () {
                ClearTelInfoFields();
                $('#popupProjectTelephone').modal('show');
            })

            .on('click', '#btnCancelOrgFOWSTTelephone', function () {
                $('#popupProjectTelephone').modal('hide');
            })

            .on('click', '#btnOrgFOWSTTelAreaCode2', function () {
                BrowsePopup('LibraryAreaCodes', 'Area Code', [
                        {
                            DisplayText: [
                                {
                                    Label: 'Area',
                                    Column: 'Area'
                                },
                                {
                                    Label: 'Code',
                                    Column: 'CallingCode'
                                }
                            ],
                            Click: function (vData) {
                                $('#txtOrgFOWSTTelAreaCodeID2').val(vData.ID);
                                $('#txtAreaCode').text(vData.Area + ' - ' + vData.CallingCode);
                            }
                        }
                ])
            })

            .on('click', '#btnSaveOrgFOWSTTelephone', function () {
                if ($('#txtAreaCode').text() == "Select") {
                    WebToast('Please select country code', 'top');
                    return false;
                }

                if ($('#txtTelephoneNum').val() == "") {
                    WebToast('Please enter telephone number', 'top');
                    return false;
                }

                if ($('#txtTelephoneNum').val().length != 7) {
                    WebToast('Enter 7 digit number', 'top');
                    return false;
                }

                if ($('#selOrgFOWSTTelType2').val() == "") {
                    WebToast('Please select contact type', 'top');
                    return false;
                }

                intTelInfoCtr = intTelInfoCtr + 1;

                if (TelState == "") {
                    arrTelInfoList.push({
                        "TelRankNo": intTelInfoCtr,
                        "CountryCodeID": $('#txtOrgFOWSTTelCountryID2').val(),
                        "CountryCode": $('#txtCountryCode').text(),
                        "AreaCode": $('#txtAreaCode').text(),
                        "AreaCodeID": $('#txtOrgFOWSTTelAreaCodeID2').val(),
                        "TelephoneNo": $('#txtTelephoneNum').val(),
                        "ExtensionNo": $('#txtTelephoneExt').val(),
                        "ContactType": $('#selOrgFOWSTTelType2').val(),
                        "TelState": "Add",
                        "IsNew": 1
                    })
                }
                else {
                    arrTelInfoList[sessionStorage.Telindex].TelRankNo = TelRankNo;
                    arrTelInfoList[sessionStorage.Telindex].CountryCodeID = $('#txtOrgFOWSTTelCountryID2').val();
                    arrTelInfoList[sessionStorage.Telindex].CountryCode = $('#txtCountryCode').text();
                    arrTelInfoList[sessionStorage.Telindex].AreaCodeID = $('#txtOrgFOWSTTelAreaCodeID2').val();
                    arrTelInfoList[sessionStorage.Telindex].AreaCode = $('#txtAreaCode').text();
                    arrTelInfoList[sessionStorage.Telindex].TelephoneNo = $('#txtTelephoneNum').val();
                    arrTelInfoList[sessionStorage.Telindex].ExtensionNo = $('#txtTelephoneExt').val();
                    arrTelInfoList[sessionStorage.Telindex].ContactType = $('#selOrgFOWSTTelType2').val();
                    arrTelInfoList[sessionStorage.Telindex].TelState = sessionStorage.TelState;
                    arrTelInfoList[sessionStorage.Telindex].IsNew = sessionStorage.TelIsNew;
                }

                AddTelInfo();
                TelState = "";
            })

            .on('keydown', '#txtTelephoneNum', function (e) {
                if (!NumberOnly(e)) {
                    return;
                }
            })

            .on('keydown', '#txtTelephoneExt', function (e) {
                if (!NumberOnly(e)) {
                    return;
                }
            })

            .on('click', '#btnAddOrgFOWSTMobile2', function () {
                ClearMobileInfoFields();
                $('#popupProjectMobile').modal('show');
            })

            .on('click', '#btnCancelOrgFOWSTMobile', function () {
                $('#popupProjectMobile').modal('hide');
            })

            .on('click', '#btnOrgFOWSTMobileTelCode2', function () {
                BrowsePopup('LibraryTelcoPrefixes', 'Telco Code', [
                        {
                            DisplayText: [
                                {
                                    Column: 'Prefix'
                                },
                                {
                                    Column: 'TelCo'
                                }
                            ],
                            Click: function (vData) {
                                $('#txtOrgFOWSTTelCoPrefixID2').val(vData.ID);
                                $('#txtTelCode').text(vData.TelCo + ' - ' + vData.Prefix);
                            }
                        }
                ])
            })

            .on('keydown', '#txtMobileNo2', function (e) {
                if (!NumberOnly(e)) {
                    return;
                }
            })

            .on('click', '#btnSaveOrgFOWSTMobile', function () {
                if ($('#txtTelCode').text() == "Select") {
                    WebToast('Please select country code', 'top');
                    return false;
                }

                if ($('#txtMobileNo2').val() == "") {
                    WebToast('Please enter mobile numer', 'top');
                    return false;
                }

                if ($('#txtMobileNo2').val().length != 7) {
                    WebToast('Enter 7 digit number', 'top');
                    return false;
                }

                if ($('#selOrgFOWSTMobileType2').val() == "") {
                    WebToast('Please select contact type', 'top');
                    return false;
                }

                intMobileInfoCtr = intMobileInfoCtr + 1;

                if (MobileState == "") {
                    arrMobileInfoList.push({
                        "MobileRankNo": intMobileInfoCtr,
                        "CountryCodeID": $('#txtOrgFOWSTMobileCountryID2').val(),
                        "CountryCode": $('#txtMobileCountryCode').text(),
                        "TelCode": $('#txtTelCode').text(),
                        "TelCodeID": $('#txtOrgFOWSTTelCoPrefixID2').val(),
                        "MobileNo": $('#txtMobileNo2').val(),
                        "ContactType": $('#selOrgFOWSTMobileType2').val(),
                        "MobileState": "Add",
                        "IsNew": 1
                    })
                }
                else {
                    arrMobileInfoList[sessionStorage.Mobileindex].MobileRankNo = MobileRankNo;
                    arrMobileInfoList[sessionStorage.Mobileindex].CountryCodeID = $('#txtOrgFOWSTMobileCountryID2').val();
                    arrMobileInfoList[sessionStorage.Mobileindex].CountryCode = $('#txtMobileCountryCode').text();
                    arrMobileInfoList[sessionStorage.Mobileindex].TelCodeID = $('#txtOrgFOWSTTelCoPrefixID2').val();
                    arrMobileInfoList[sessionStorage.Mobileindex].TelCode = $('#txtTelCode').text();
                    arrMobileInfoList[sessionStorage.Mobileindex].MobileNo = $('#txtMobileNo2').val();
                    arrMobileInfoList[sessionStorage.Mobileindex].ContactType = $('#selOrgFOWSTMobileType2').val();
                    arrMobileInfoList[sessionStorage.Mobileindex].MobileState = sessionStorage.MobileState;
                    arrMobileInfoList[sessionStorage.Mobileindex].IsNew = sessionStorage.MobileIsNew;
                }

                AddMobileInfo();
                MobileState = "";
            })

            .on('click', '#btnAddOrgFOWSTEmail2', function () {
                ClearEmailInfoFields();
                $('#popupProjectEmailAdd').modal('show');
            })

            .on('click', '#btnCancelOrgFOWSTEmailAddress', function () {
                $('#popupProjectEmailAdd').modal('hide');
            })

            .on('click', '#btnSaveOrgFOWSTEmailAddress', function () {
                if (IsEmail($.trim($("#txtEmailAdd2").val())) == false) {
                    WebToast('Please enter valid email address', 'top');
                    return false;
                }

                if ($.trim($("#txtEmailAdd2").val()) == "") {
                    WebToast('Please your email address', 'top');
                    return false;
                }

                if ($('#selOrgFOWSTEmailType2').val() == "") {
                    WebToast('Please select email type', 'top');
                    return false;
                }

                intEmailInfoCtr = intEmailInfoCtr + 1;

                if (EmailState == "") {
                    arrEmailInfoList.push({
                        "EmailRankNo": intEmailInfoCtr,
                        "EmailAddID": $('#txtOrgFOWSTEmailID2').val(),
                        "EmailAdd": $('#txtEmailAdd2').val(),
                        "EmailType": $('#selOrgFOWSTEmailType2').val(),
                        "EmailState": "Add",
                        "IsNew": 1
                    })
                }
                else {
                    arrEmailInfoList[sessionStorage.Emailindex].EmailRankNo = EmailRankNo;
                    arrEmailInfoList[sessionStorage.Emailindex].EmailAddID = $('#txtOrgFOWSTEmailID2').val();
                    arrEmailInfoList[sessionStorage.Emailindex].EmailAdd = $('#txtEmailAdd2').val();
                    arrEmailInfoList[sessionStorage.Emailindex].EmailType = $('#selOrgFOWSTEmailType2').val();
                    arrEmailInfoList[sessionStorage.Emailindex].EmailState = sessionStorage.EmailState;
                    arrEmailInfoList[sessionStorage.Emailindex].IsNew = sessionStorage.EmailIsNew;
                }

                AddEmailInfo();
                EmailState = "";
            })

            .on('keypress', '#txtSearchKeyword', function (e) {
                e.stopPropagation();

                if (e.keyCode == 13) {
                    if ($('#TitleLibrary').text() == "CITY") {
                        SearchCityPopUp2($(this).val());
                    }
                    else if ($('#TitleLibrary').text() == "COUNTRY") {
                        SearchCountryPopUp2($(this).val());
                    }
                    else if ($('#TitleLibrary').text() == "List of SKU") {
                        SearchPIMProductPopUp2($(this).val());
                    }
                    else if ($('#TitleLibrary').text() == "PROJECT OWNER" || $('#TitleLibrary').text() == "PROJECT MANAGER" || $('#TitleLibrary').text() == "ASSISTANT PROJECT MANAGER") {
                        SearchEmployeeName2($(this).val());
                        isCanvass = false;
                    }
                }
            })

            .on('click', '#btnSaveNewProject', function () {
                if ($('#txtClientID').val() == "") {
                    WebToast('Please select client', 'top');
                    return false;
                }

                if ($('#txtProjectName').val() == "") {
                    WebToast('Please enter project name', 'top');
                    return false;
                }

                if ($('#txtProjectCode').val() == "") {
                    WebToast('Please enter project code', 'top');
                    return false;
                }

                if ($('#btnClientName').text() == "") {
                    WebToast('Please select client', 'top');
                    return false;
                }

                if ($('#txtOrgFOWSTStreet2').val() == "") {
                    WebToast('Please enter street name', 'top');
                    return false;
                }

                if ($('#txtCityName').text() == "Please Select") {
                    WebToast('Please select city', 'top');
                    return false;
                }

                if ($('#txtCountryName').text() == "Please Select") {
                    WebToast('Please select country', 'top');
                    return false;
                }

                if ($('#ProjStartDate').text() == "Set Date") {
                    WebToast('Please select projected start date', 'top');
                    return false;
                }

                if ($('#ProjCompDate').text() == "Set Date") {
                    WebToast('Please select projected completion date', 'top');
                    return false;
                }

                var isMemberJobRoleNull = false
                $('.selMemberJobRole').each(function () {
                    if ($(this).parents().closest('tr').attr('state') != 'delete') {
                        if ($(this).val() == '') {
                            isMemberJobRoleNull = true;
                            WebToast('Please assign job role for <b>' + $(this).closest('tr').find('td:eq(0)').text() + '</b>', 'top');
                            return false;
                        }
                    }                            
                })

                if (CBool(isMemberJobRoleNull)) {
                    return;
                }

                SaveNewProject();
            })

            .on('click', '#btnEditProjectDetails', function () {
                if(location.pathname.toLowerCase() != '/erp'){
                    $('.ItemsBreadCrumbs').text('');
                    breadcrumbs.prepend(arrowR, "Edit Details");
                    breadcrumbs.prepend(arrowR, "<a data-role='none' id='btnBCProjectBudget'>Project Budget</a>");
                    $('.BreadCrumbs').html(breadcrumbs.prepend(lnkProject));
                }                        

                isAddProject = false;
                isFinishedLoadJobRolePermission = false;
                arrDefineRole = [];
                arrRolePermissions = [];
                arrProjectMember = [];
                ClearAllFields();
                ClearAllVariables();
                $('#popupNewProject').modal('show');
                $('#divNewProjectDetails, #divProjectMember').show();
                $('#divNewProjectCustomer, #divSelectionRole, #divDefineRole').hide();
                GetProjectOwnerManagerAssistant(sessionStorage.ProjectID);
            })

            .on('dblclick', '.InputTextOnly', function () {
                var currentVal = $(this).text();
                var replaceWithTextbox = $('<input type="text" class="InputText">');

                $(this).replaceWith(replaceWithTextbox.val(currentVal));
                replaceWithTextbox.focus();
            })
            .on('blur', '.InputText', function () {
                newVal = $('.InputText').val();
                $(this).replaceWith('<a class="InputTextOnly">' + newVal + '</a>');
            })
            .on('dblclick', '#tBodyExcelLike .tddescription', function () {
                var div = $(this).find('div');
                var thisAttr = div.attr('id');
                var thisVal = div.text();
                var isParent = CDec2(div.attr('isparent'))
                var thisIndentNo = div.attr('indentno');
                div.replaceWith($('<input>')
                                        .attr('type', 'text')
                                        .attr('value', $.trim(thisVal))
                                        .attr('id', 'typeText-' + thisAttr)
                                        .attr('isParent', isParent)
                                        .attr('isRequired', 1)
                                        .addClass('typeText')
                                        .addClass('inputTextInExcel')
                                        .attr('state', div.attr('datatype'))
                                        .attr('columnName', div.attr('columnName'))
                                        .attr('data-role', 'none')
                                        .attr('indentno', thisIndentNo)
                                    );
                $('.typeText').focus();
            })
            .on('blur', '#tBodyExcelLike .typeText', function (e) {
                if (isFocusOut == false) {
                    if ($(this).val() != "") {
                        if ($(this).attr('isrequired') == 1) {
                            TempSaveEditedDescription(this);
                        }
                        else {
                            TempSaveEditedCell(this);
                        }
                    }
                    else {
                        var id = $(this).attr('id').split('-')[1];
                        $(this).replaceWith($('<div>')
                                        .attr('id', id)
                                        .addClass('txtOnlyParent')
                                        .attr('isParent', 1)
                                        .attr('isRequired', 1)
                                        .attr('columnName', $(this).attr('columnName'))
                                        .attr('data-role', 'none')
                                        .attr('state', $(this).attr('state'))
                                        .data('data', $(this).data('data'))
                                        .html('&nbsp;')
                                    );
                    }

                    if ($.trim($('#tBodyExcelLike tr td.activeCell').closest('tr').find('td.tddescription').text()) != "") {
                        AddNewRow();
                    }
                }
                isFocusOut = false;
            })
            .on('keydown', '#tBodyExcelLike .typeText', function (e) {
                e.stopPropagation();
                if (e.which == 13 || e.which == 38 || e.which == 40) {
                    isFocusOut = true;
                    if ($(this).val() != "") {
                        if ($(this).attr('isrequired') == 1) {
                            TempSaveEditedDescription(this, e.which);
                        }
                        else {
                            TempSaveEditedCell(this, e.which);
                        }
                    }
                    else {
                        var id = $(this).attr('id').split('-')[1];
                        $(this).replaceWith($('<div>')
                                        .attr('id', id)
                                        .addClass('txtOnlyParent')
                                        .attr('isParent', 1)
                                        .attr('isRequired', 1)
                                        .attr('columnName', $(this).attr('columnName'))
                                        .attr('data-role', 'none')
                                        .attr('state', $(this).attr('state'))
                                        .html('&nbsp;')
                                    );
                    }

                    RefreshFreezePanesHeight($('#tBodyExcelLike tr td.activeCell').closest('tr').attr('id'))

                    if ($.trim($('#tBodyExcelLike tr td.activeCell').closest('tr').find('td.tddescription').text()) != "") {
                        AddNewRow();
                    }

                    if (e.which == 39) {
                        $('#tBodyExcelLike tr td.activeCell').closest('td').next().addClass('activeCell');
                        $('#tBodyExcelLike tr td.activeCell').closest('td').prev().removeClass('activeCell');
                    }
                    else {
                        currentCellIndex = $('#tBodyExcelLike tr td.activeCell').closest('td').index();
                        $('#tBodyExcelLike tr td.activeCell').closest('tr').next().find('td:eq(' + currentCellIndex + ')').addClass('activeCell');
                        $('#tBodyExcelLike tr td.activeCell').closest('tr').prev().find('td:eq(' + currentCellIndex + ')').removeClass('activeCell');
                    }                            
                }

                // if($(this).attr('datatype') == "numeric"){
                //     if (!NumberOnly(e)) {
                //         return;
                //     }
                // }
            })
            .on('click', '#tBodyExcelLike .tableCell', function (e) {
                if (e.ctrlKey) {
                    //$('#tBodyExcelLike tr td.activeCell').parent().addClass('rowSelected');
                    $(this).parent().addClass('rowSelected');
                    $('#tBodyExcelLike tr td').removeClass('activeCell');
                }
                else {
                    $('#tBodyExcelLike tr td').removeClass('activeCell');
                    $('#tBodyExcelLike tr').removeClass('activeRow');
                    $('#tBodyExcelLike tr').removeClass('rowSelected');
                    $(this).addClass('activeCell');
                    $(this).parent().addClass('activeRow');
                    $('.context-menu').remove();
                }
            })

            .on('dblclick', '#tBodyCanvassMaterial div', function () {
                var currentVal = $(this).text();
                var replaceWithTextbox = $('<input type="text" datatype="numeric" style="width:100%" class="typeText InputText inputTextInExcel">');

                if ($('#tBodyCanvassMaterial tr td.activeCell').hasClass('ReadOnly')) {
                    return false;
                }

                $(this).replaceWith($('<input>')
                            .attr('type', 'text')
                            .attr('value', $.trim(currentVal))
                            .attr('datatype', 'numeric')
                            .addClass('typeText')
                            .addClass('inputTextInExcel').focus()
                        );
                replaceWithTextbox.focus();
                isFocusOut = true;
            })
            .on('blur', '#tBodyCanvassMaterial .typeText', function (e) {
                if (isFocusOut == false) {
                    var indexNo = $(this).closest('td').index();
                    var skuid = $(this).closest('tr').find('td:eq(1)').find('div').attr('associateskuid');
                    $(this).replaceWith($('<div>')
                                        .attr('datatype', 'numeric')
                                        .attr('data-role', 'none')
                                        .attr('vid', $(this).attr('vid'))
                                        .attr('state', $(this).attr('state'))
                                        .attr('id', $(this).attr('id'))
                                        .css({ width: '100%' })
                                        .html($(this).val())
                                    );
                    $('#tBodyCanvassMaterial div[associateskuid="' + skuid + '"]').closest('tr').find('td:eq(' + indexNo + ') div').text($(this).val());
                    $('#tBodyCanvassMaterial div[associateskuid="' + skuid + '"]').closest('tr').find('td').removeClass('minPrice');
                    SetColorMinPrice();
                }
                isFocusOut = false;
            })
            .on('keypress', '#tBodyCanvassMaterial .typeText', function (e) {
                var indexNo = $(this).closest('td').index();
                var skuid = $(this).closest('tr').find('td:eq(1)').find('div').attr('associateskuid');
                if (skuid == undefined) {
                    WebToast('Item must link to SKU first', 'top');
                    return false;
                }

                if (e.which == 13 || e.which == 38 || e.which == 40 || e.which == 39) {
                    isFocusOut = true;
                    $(this).replaceWith($('<div>')
                                        .attr('datatype', 'numeric')
                                        .attr('data-role', 'none')
                                        .attr('vid', $(this).attr('vid'))
                                        .attr('state', $(this).attr('state'))
                                        .attr('id', $(this).attr('id'))
                                        .css({ width: '100%' })
                                        .html($(this).val())
                                    );
                    if (e.which == 39) {
                        $('#tBodyCanvassMaterial tr td.activeCell').closest('td').next().addClass('activeCell');
                        $('#tBodyCanvassMaterial tr td.activeCell').closest('td').prev().removeClass('activeCell');
                    }
                    else {
                        $('#tBodyCanvassMaterial tr td.activeCell').closest('tr').next().find('td:eq(' + $('#tBodyCanvassMaterial tr td.activeCell').closest('td').index() + ')').addClass('activeCell');
                        $('#tBodyCanvassMaterial tr td.activeCell').closest('tr').prev().find('td').removeClass('activeCell')
                    }

                    $('#tBodyCanvassMaterial div[associateskuid="' + skuid + '"]').closest('tr').find('td:eq(' + indexNo + ') div').text($(this).val());
                    $('#tBodyCanvassMaterial div[associateskuid="' + skuid + '"]').closest('tr').find('td').removeClass('minPrice');
                    SetColorMinPrice();
                }

                if (e.which != 13) {
                    return isNumberKey(e);
                }
            })
            .on('click', '#tBodyCanvassMaterial .tableCell', function (e) {
                $('#tBodyCanvassMaterial tr td').removeClass('activeCell');
                $('#tBodyCanvassMaterial tr').removeClass('activeRow');
                $('#tBodyCanvassMaterial tr').removeClass('rowSelected');
                $(this).addClass('activeCell');
                $(this).parent().addClass('activeRow');
            })

            .on('dblclick', '#tBodyCanvassLabor div', function () {
                var currentVal = $(this).text();
                var replaceWithTextbox = $('<input type="text" datatype="numeric" style="width:100%" class="typeText InputText inputTextInExcel">');

                if ($('#tBodyCanvassLabor tr td.activeCell').hasClass('ReadOnly')) {
                    return false;
                }

                $(this).replaceWith($('<input>')
                            .attr('type', 'text')
                            .attr('value', $.trim(currentVal))
                            .attr('datatype', 'numeric')
                            .addClass('typeText')
                            .addClass('inputTextInExcel').focus()
                        );
                replaceWithTextbox.focus();
                isFocusOut = true;
            })
            .on('blur', '#tBodyCanvassLabor .typeText', function (e) {
                if (isFocusOut == false) {
                    var indexNo = $(this).closest('td').index();
                    var skuid = $(this).closest('tr').find('td:eq(1)').find('div').attr('associateskuid');
                    $(this).replaceWith($('<div>')
                                        .attr('datatype', 'numeric')
                                        .attr('data-role', 'none')
                                        .attr('vid', $(this).attr('vid'))
                                        .attr('state', $(this).attr('state'))
                                        .attr('id', $(this).attr('id'))
                                        .css({ width: '100%' })
                                        .html($(this).val())
                                    );
                    $('#tBodyCanvassLabor div[associateskuid="' + skuid + '"]').closest('tr').find('td:eq(' + indexNo + ') div').text($(this).val());
                    $('#tBodyCanvassLabor div[associateskuid="' + skuid + '"]').closest('tr').find('td').removeClass('minPrice');
                    SetColorMinPrice();
                }
                isFocusOut = false;
            })
            .on('keypress', '#tBodyCanvassLabor .typeText', function (e) {
                e.stopPropagation();
                var indexNo = $(this).closest('td').index();
                var skuid = $(this).closest('tr').find('td:eq(1)').find('div').attr('associateskuid');

                if (e.which == 13 || e.which == 38 || e.which == 40 || e.which == 39) {
                    isFocusOut = true;
                    $(this).replaceWith($('<div>')
                                        .attr('datatype', 'numeric')
                                        .attr('data-role', 'none')
                                        .attr('vid', $(this).attr('vid'))
                                        .attr('state', $(this).attr('state'))
                                        .attr('id', $(this).attr('id'))
                                        .css({ width: '100%' })
                                        .html($(this).val())
                                    );
                    if (e.which == 39) {
                        $('#tBodyCanvassLabor tr td.activeCell').closest('td').next().addClass('activeCell');
                        $('#tBodyCanvassLabor tr td.activeCell').closest('td').prev().removeClass('activeCell');
                    }
                    else {
                        $('#tBodyCanvassLabor tr td.activeCell').closest('tr').next().find('td:eq(' + $('#tBodyCanvassLabor tr td.activeCell').closest('td').index() + ')').addClass('activeCell');
                        $('#tBodyCanvassLabor tr td.activeCell').closest('tr').prev().find('td').removeClass('activeCell')
                    }

                    $('#tBodyCanvassLabor div[associateskuid="' + skuid + '"]').closest('tr').find('td:eq(' + indexNo + ') div').text($(this).val());
                    $('#tBodyCanvassLabor div[associateskuid="' + skuid + '"]').closest('tr').find('td').removeClass('minPrice');
                    SetColorMinPrice();
                }
                if (e.which != 13) {
                    return isNumberKey(e);
                }
            })
            .on('click', '#tBodyCanvassLabor .tableCell', function (e) {
                $('#tBodyCanvassLabor tr td').removeClass('activeCell');
                $('#tBodyCanvassLabor tr').removeClass('activeRow');
                $('#tBodyCanvassLabor tr').removeClass('rowSelected');
                $(this).addClass('activeCell');
                $(this).parent().addClass('activeRow');
            })

            .on('keydown', '.txtChildDescription', function (e) {
                e.stopPropagation();

                if (e.which == 13) {
                    //$('#btnSaveChild').click();
                    $('#btnAddAnotherDescription').click();
                }
            })
            .on('click', '#btnAddAnotherDescription', function () {
                if ($('#lstChildDescription li:last').find('input').val() != "") {
                    $('#lstChildDescription').append($('<input data-role="none" type="text" class="form-control mb-3">')
                                                        .addClass('txtChildDescription')
                                                    )
                    $('.txtChildDescription');//.textinput();
                    isAddChildMultiple = true;
                    Reposition();
                }
                $('.txtChildDescription:last').focus();
            })
            .on('click', '#btnSaveChild', function () {
                $('.txtChildDescription').each(function () {
                    if ($(this).val() != "") {
                        if (isAddChildType == 'subtask') {
                            AddChild($(this).val(), '', false, false);
                        } else {
                            AddChild($(this).val(), '', true, false);
                        }                        
                    }
                })
                UpdateRowIDs();
                ReGenerateItemNo();
                RefreshTaskDependency();
                isAddChildMultiple = false;
                $('#lstChildDescription input:not(:first)').remove();
                $('#txtChildDescription').val('');
                $('#panAddChild').modal('hide')
            })
            .on('click', '#btnCancelProjectBudget', function () {
                $('#divViewProjectBudget').show();
                $('.divNewProjectBudget').hide();
                $('#btnSaveProjectBudget').hide();
                $('#divBtnNewProjectBudget').show();
                $('.btnDisplayAssociatedSKU').attr('state', 'show').text('Show Linked SKU');
                $('#divButtons').removeClass("divButtonspositionFixed").hide();
                isCreateNewProjectBudget = true;
                isFinishedGettingCanvassMaterials = false;
                isFinishedGettingCanvassLabor = false;
                isFinishLoadCanvassMaterials = false;
                isFinishLoadCanvassLabor = false;
                isFinishedLoadTimeline = false;
                isProjectBudgetDetails = false;
                arrCanvassMaterialsOrgPeople = [];
                arrCanvassLaborOrgPeople = [];
                GetProjectBudget(sessionStorage.ProjectID);
            })
            .on('click', '#btnSaveProjectBudget', function () {
                var arrProjectBudgetData = [];

                $('#tBodyExcelLike').find('tr:not(":last")').each(function (i, el) {
                    var dis = $(this);
                    var td = $(this).find('td');
                    var file = dis.find('td.tdremarks > div').data('files');

                    for(var i = 0; i < file.length; i++){
                        file[i].src = '';
                    }

                    arrProjectBudgetData.push({
                        "OrgID": sessionStorage.OrgID,
                        "FOWSTID": $.trim(sessionStorage.ProjectID),
                        "RowNumber": $.trim(td.parent().attr('id')),
                        "ItemNumber": $.trim(dis.find('td.tditemno > div').text()),
                        "IsParent": $.trim(dis.find('td.tddescription > div').attr('isparent')),
                        "ParentID": ($.trim(dis.find('td.tddescription > div').attr('parentid')) == "undefined" ? "" : $.trim(dis.find('td.tddescription > div').attr('parentid'))),
                        "LineDescription": $.trim(dis.find('td.tddescription > div').text()),
                        "AssociatedSKUs": ($(this).data('materials') == undefined ? [] : $(this).data('materials')),
                        "AssociateSKUID": $.trim(dis.find('td.tddescription > div').attr('associateskuid')),
                        "IsMaterial": dis.find('td.tddescription > div').hasClass('rowMaterials'),
                        "Qty": 0, //$.trim(td.eq(2).find('div').text()),
                        "Remarks": (dis.find('td.tdremarks > div').data('data') || []),
                        "Files": file,
                        "UnitMeasure": '', //$.trim(td.eq(3).find('div').text()), // to be deleted
                        "UnitMeasureID": $.trim(td.eq(3).find('div').attr('umid')),
                        "MaterialUnitCost": $.trim(td.eq(4).find('div').text()),
                        "MaterialUnitCostVendor": ($.trim(td.eq(4).find('div').attr('vid')) == "undefined" ? "" : $.trim(td.eq(4).find('div').attr('vid'))),
                        "MaterialAmount": 0, //($.trim(td.eq(5).find('div').text()) == "-" ? "0" : $.trim(td.eq(5).find('div').text())),
                        "LaborUnitCost": $.trim(td.eq(6).find('div').text()),
                        "LaborUnitCostVendor": ($.trim(td.eq(6).find('div').attr('vid')) == "undefined" ? "" : $.trim(td.eq(6).find('div').attr('vid'))),
                        "LaborAmount": ($.trim(td.eq(7).find('div').text()) == "-" ? "0" : $.trim(td.eq(7).find('div').text())),
                        "TotalUnitCost": $.trim(td.eq(8).find('div').text()),
                        "TotalAmount": ($.trim(td.eq(9).find('div').text()) == "-" ? "0" : $.trim(td.eq(9).find('div').text())),
                        "State": $.trim(dis.find('td.tddescription > div').attr('state')),
                        "ID": $.trim(td.parent().attr('descriptionrowid')),
                        "Status": (dis.find('td.tddescription > div').hasClass('disableCell') == true ? "Deactivate" : "Active"),
                        "RowType": $.trim(td.parent().attr('rowtype')),
                        "IsMarkAsCompleted": td.eq(1).hasClass('markascompleted'),

                        "IndentNo": CDec2(dis.find('td.tddescription > div').attr('indentno')),
                        "PointPersonID": $.trim(dis.find('td.tdpointperson > div > select').val()),
                        "Dependency": (dis.find('td.tddependency > div').data('data').Dependency || ''),
                        "BudgetedDuration": dis.find('td.tdbudgetedduration > div').data('data'),
                        "ActualDuration": $.trim(dis.find('td.tdactualduration > div').text()),
                        "DateStarted": $.trim(dis.find('td.tddatestarted > div').text()),
                        "DateCompleted": $.trim(dis.find('td.tddatecompleted > div').text()),
                        "Importance": $.trim(dis.find('td.tdimportance > div > select').val()),
                        "Urgency": $.trim(dis.find('td.tdurgency > div > select').val()),
                        "UnitMeasureID": $.trim(dis.find('td.tdunitmeasure > div > select').val()),
                        "BudgetedQty": dis.find('td.tdbudgetedqty > div').data('data'),
                        "AccReqQty": dis.find('td.tdreqqty > div').data('data'),
                        //"AccDeliveryQty": dis.find('td.tdaccdeliveryqty > div').data('data'),
                        "AccUsedUpQty": dis.find('td.tdaccusedup > div').data('data'),
                        "LaborCost": dis.find('td.tdlaborcost > div').data('data'),
                        "RequestedAdvances": dis.find('td.tdaccreqlaborcost > div').data('data')
                    });
                })
                //console.log(arrProjectBudgetData);
                //return
                if (arrProjectBudgetData.length == 0) {
                    NewMessageBox('No Data to save', 'Project');
                    return false;
                }

                SaveProjectBudget(arrProjectBudgetData);
            })
            .on('click', '.btnDisplayAssociatedSKU', function () {
                if ($(this).attr('state') == "show") {
                    $(this).attr('state', 'hidden').text('Hide Linked SKU');
                    $('.preAssociatedSKU').show();
                }
                else {
                    $(this).attr('state', 'show').text('Show Linked SKU');
                    $('.preAssociatedSKU').hide();
                }
                //$('#panMoreOption').modal('hide');
            })
            .on('click', '#btnDisplayAssociatedSKU', function () {
                if ($(this).attr('state') == "show") {
                    $(this).attr('state', 'hide').find('a').text('Hide SKU');
                    $('.preAssociatedSKU').show();
                }
                else {
                    $(this).attr('state', 'show').find('a').text('Show SKU');
                    $('.preAssociatedSKU').hide();
                }
                $('#panMoreOption').modal('hide');
            })
            .on('click', '#btnNewProjectBudget', function () {
                $('#divViewProjectBudget, .divTimeline').hide();
                $('.divNewProjectBudget').show();
                $('#btnSaveProjectBudget').show();
                $('#divBtnNewProjectBudget, #divButtons').hide();

                isProjectBudgetDetails = true;

                if (isCreateNewProjectBudget) {
                    arrCanvassMaterialsOrgPeople = [];
                    arrCanvassLaborOrgPeople = [];
                    isCanvassForm = false;
                    isCanvassMaterialsAlready = false;
                    isCanvassLaborAlready = false;
                    $('#tBodyExcelLike').empty();
                    CreateExcelTable();
                }
                else {
                    isCreateNewProjectBudget = false;
                    projectData = new FormData();
                    $('#tBodyExcelLike').empty();
                    arrProjectMember = [];
                    GetProjectOwnerManagerAssistant(sessionStorage.ProjectID, true, function () {
                        GetProjectBudget(sessionStorage.ProjectID);
                    });                            
                    //isCanvassMaterialsAlready = true;
                    //isCanvassLaborAlready = true;
                    RefreshUnfreezeContent();
                }
                $('#tBodyExcelLike tr:first td.tddescription').addClass('activeCell');
                $('.custom-btn-tab').removeClass('custom-btn-theme-b');
                $('#btnProjectSummary').removeClass('custom-btn-theme-c').addClass('custom-btn-theme-b');
            })
            .on('click', '.btnCanvassMaterials', function () {
                isCanvassMaterials = true;
                isAddNewVendorCanvassMaterials = false;
                isOnCanvassPage = true;

                $('#btnEditingModeCanvassNow').hide();
                $('#btnCanvassNow, #btnPrintCanvassSheets, #btnAddNewVendorCanvassSheets').show();
                $('#panCanvassOption').popup('close');

                if ($.trim($('#tBodyExcelLike tr td.activeCell').closest('tr').find('td:eq(2)').find('div').text()) == "") {
                    NewMessageBox('Enter a description first', 'Project');
                    return false;
                }
                else if (CheckPermissions("Add/Edit Materials Costing") == false) {
                    return false;
                }
                // else if ($('#tBodyExcelLike tr td.activeCell').closest('tr').find('td:eq(2)').find('div').attr('associateskuid') == undefined) {
                //     $('#divSKUMsg, .divNewProjectBudget').show();
                //     $('.divCanvass').hide();
                //     $('#panSearchLibrary').popup('open');
                //     $('#lstOfLibrary').empty();
                //     $('#txtSearchKeyword').removeAttr('placeholder').val('');
                //     $('#TitleLibrary').text('List of SKU');
                //     $('#txtDescriptionName').text($('#tBodyExcelLike tr td.activeCell').closest('tr').find('td:eq(2)').find('div').text());
                //     NameOfBtnForLibrary = "LinkSKU";
                //     itemAssociateSKUID = $('#tBodyExcelLike tr td.activeCell').closest('tr').find('td:eq(2)').find('div').attr('descriptionid');
                //     isHasAssociateSKUID = false;
                //     isPopupOpen = true;
                //     RepositionPopup();
                //     return false;
                // }

                if (isCanvassMaterialsAlready) {
                    $('.custom-btn-tab').removeClass('custom-btn-theme-b');
                    $('#btnSubCanvassMaterials').removeClass('custom-btn-theme-c').addClass('custom-btn-theme-b');
                    $('.divNewProjectBudget, #divCanvassLabor, .divTimeline').hide();
                    $('.divCanvass, #divCanvassMaterials').show();
                    AppendNewAddedRowToCanvassForm('tBodyCanvassMaterial');

                    if (isCreateNewProjectBudget == false) {
                        if (isFinishedGettingCanvassMaterials) {
                            isCanvassForm = true;
                            if (isFinishLoadCanvassMaterials == false) {
                                GetCanvassMaterials();
                                isFinishLoadCanvassMaterials = true;
                            }
                        }
                    }
                    return false;
                }

                $('#lstOfOrgEmp').empty();
                $('#lstOfOrgEmp').children('li').children('div').removeClass('selectedList');
                isPopupOpen = true;
                $('#divSKUMsg').hide();
                $('#TitleCanvass').text('CANVASS');
                $('#panSearchCanvass').modal('show');
                Reposition();
                CountVendorSelected();
            })
            .on('click', '.btnCanvassLabor', function () {
                isCanvassMaterials = false;
                isOnCanvassPage = true;
                isAddNewVendorCanvassLabor = false;
                arrBackUpCanvassLabor = arrCanvassLaborOrgPeople;
                $('#btnEditingModeCanvassNow').hide();
                $('#btnCanvassNow, #btnPrintCanvassSheets, #btnAddNewVendorCanvassSheets').show();
                $('#panCanvassOption').popup('close');
                $('.custom-btn-tab').removeClass('custom-btn-theme-b');
                $('#btnSubCanvassLabor').removeClass('custom-btn-theme-c').addClass('custom-btn-theme-b');

                if ($.trim($('#tBodyExcelLike tr td.activeCell').closest('tr').find('td:eq(2)').find('div').text()) == "") {
                    NewMessageBox('Enter a description first', 'Project');
                    return false;
                }
                else if (CheckPermissions("Add/Edit Labor Costing") == false) {
                    return false;
                }

                if (isCanvassLaborAlready) {
                    $('.divNewProjectBudget, #divCanvassMaterials, .divTimeline').hide();
                    $('.divCanvass, #divCanvassLabor').show();
                    AppendNewAddedRowToCanvassForm('tBodyCanvassLabor');

                    if (isCreateNewProjectBudget == false) {
                        if (isFinishedGettingCanvassLabor) {
                            isCanvassForm = true;
                            if (isFinishLoadCanvassLabor == false) {
                                GetCanvassLabor();
                                isFinishLoadCanvassLabor = true;
                            }
                        }
                    }
                    return false;
                }

                $('#lstOfOrgEmp').empty();
                $('#lstOfOrgEmp').children('li').children('div').removeClass('selectedList');
                isPopupOpen = true;
                $('#divSKUMsg').hide();
                $('#TitleCanvass').text('CANVASS');
                $('#panSearchCanvass').modal('show');
                Reposition();
                CountVendorSelected();
            })
            .on('keypress', '#txtSearchOrgEmp', function (e) {
                e.stopPropagation();

                if (e.keyCode == 13) {
                    if ($('#TitleCanvass').text() == "CANVASS") {
                        isCanvass = true;
                        SearchOrganization($(this).val());
                    }
                }
            })
            .on('click', '#btnCancelCanvassMaterials', function () {
                $('.divCanvass').hide();
                $('.divNewProjectBudget').show();
                if (location.pathname.toLowerCase() != '/erp') {
                    $('.ItemsBreadCrumbs').text('');
                    breadcrumbs.prepend(arrowR, "Project Budget");
                    $('.BreadCrumbs').html(breadcrumbs.prepend(lnkProject));
                }                        
            })
            .on('click', '#btnCanvassCancel', function () {
                $('#panListOfVendors').modal('hide');
                isPopupOpen = false;
            })
            .on('click', '.btnPopupClose', function () {
                isPopupOpen = false;
            })
            .on('click', '#btnClearVendorSelected', function () {
                $('#lstOfOrgEmp').children('li').children('div').removeClass('selectedList');
                if (isCanvassMaterials) {
                    arrCanvassMaterialsOrgPeople = [];
                }
                else {
                    arrCanvassLaborOrgPeople = [];
                }
                CountVendorSelected();
            })
            .on('click', '#btnCanvassNow', function () {
                if (isCanvassMaterials) {
                    whatArr = arrCanvassMaterialsOrgPeople;
                }
                else {
                    whatArr = arrCanvassLaborOrgPeople;
                }

                if (whatArr.length == 0) {
                    $('#divVendorValidation').html('<br>Please select at least one vendor<br><br>').show();
                    return false;
                }

                var target, targetTHead;
                isPopupOpen = false;

                $('#divVendorValidation').html('').show();
                $('#panListOfVendors').modal('hide');
                $('.divNewProjectBudget').hide();
                $('.divCanvass').show();

                if (isCanvassMaterials) {
                    $('#divCanvassMaterials').show();
                    $('#divCanvassLabor').hide();
                    target = $("#tBodyCanvassMaterial");
                    targetTHead = $('#tHeadCanvassMaterialRow');
                }
                else {
                    $('#divCanvassMaterials').hide();
                    $('#divCanvassLabor').show();
                    target = $("#tBodyCanvassLabor");
                    targetTHead = $('#tHeadCanvassLaborRow');
                }

                targetTHead.empty();
                target.empty();

                for (var i = 0; i < whatArr.length; i++) {
                    if (targetTHead.find('th:contains("' + whatArr[i].Name + '")').length == 0) {
                        targetTHead.append($('<th>')
                                            .attr('arrindex', i)
                                            .attr('id', whatArr[i].ID)
                                            .attr('title', whatArr[i].Name)
                                            .attr('state', whatArr[i].State)
                                            .text(whatArr[i].Name)
                                            .data('data', whatArr[i])
                                            .css({
                                                maxWidth: '100px',
                                                whiteSpace: 'nowrap',
                                                textOverflow: 'ellipsis',
                                                overflow: 'hidden',
                                                cursor: 'pointer'
                                            })
                                            .click(function (e) {
                                                var id = $(this).attr('ID');
                                                var state = $(this).attr('state');
                                                var arrIndex = $(this).attr('arrIndex');
                                                var titleName = $(this).attr('title');
                                                var newArr;

                                                PopupButtonList('', 'Options', [
                                                    {
                                                        Text: 'Delete', CLick: function () {
                                                            NewMessageBox('Are you sure want to delete ' + titleName + ' ?', 'Canvass Form', [
                                                            {
                                                                Text: 'Yes',
                                                                Click: function () {
                                                                    if (state == "edit") {
                                                                        if (isCanvassMaterials) {
                                                                            arrCanvassMaterialsOrgPeople[arrIndex].State = "delete"
                                                                            newArr = arrCanvassMaterialsOrgPeople;
                                                                        } else {
                                                                            arrCanvassLaborOrgPeople[arrIndex].State = "delete";
                                                                            newArr = arrCanvassLaborOrgPeople;
                                                                        }
                                                                    }
                                                                    else {
                                                                        newArr = $.grep(whatArr, function (val) {
                                                                            return val.ID != id;
                                                                        })
                                                                    }

                                                                    if (isCanvassMaterials) {
                                                                        arrCanvassMaterialsOrgPeople = newArr;
                                                                    }
                                                                    else {
                                                                        arrCanvassLaborOrgPeople = newArr;
                                                                    }

                                                                    RemoveFromCanvassSheet(arrIndex, state);
                                                                    CountVendorSelected();
                                                                    LoadSelectedOrgPeopleCanvass(newArr);
                                                                    SetColorMinPrice();
                                                                }
                                                            },
                                                            {
                                                                Text: 'No'
                                                            }
                                                            ]);
                                                        }
                                                    }, {
                                                        Text: 'Set as Preferred Vendor', CLick: function () {
                                                            var selector;

                                                            if (isCanvassMaterials) {
                                                                selector = $('#tBodyCanvassMaterial');
                                                            }
                                                            else {
                                                                selector = $('#tBodyCanvassLabor');
                                                            }

                                                            selector.find('tr td').removeClass('preferredVendor').removeClass('minPrice').removeAttr('isPreferredCanvass');

                                                            selector.find('tr').each(function () {
                                                                $(this).find('td:eq(' + (parseInt(arrIndex) + 4) + ')').attr('isPreferredCanvass', '1').addClass('preferredVendor')
                                                            })

                                                            SetColorMinPrice();
                                                        }
                                                    }
                                                ])
                                            })
                                        )
                    }
                }

                $("#tBodyExcelLike tr").not('tr:last').each(function () {
                    if ($(this).hasClass('rowSubtotal') != true) {
                        var tds = $(this).find('td'),
                            row = $("<tr>").attr('id', $(this).attr('id'));
                        row
                            .append(tds.eq(1).addClass('ReadOnly').clone())
                            .append(tds.eq(2).addClass('ReadOnly').clone().addClass('tableCellDisable'))
                            .append(tds.eq(3).addClass('ReadOnly').clone().addClass('tableCellDisable'))
                            .append(tds.eq(4).addClass('ReadOnly').clone().addClass('tableCellDisable'))
                            .appendTo(target);
                    }
                });

                AppendNewColumn();
                CountVendorSelected();
            })
            .on('click', '#btnSearchCanvassCancel', function () {
                $('#panSearchCanvass').modal('hide');
                $('#panListOfVendors').modal('show');
                if (isCanvassMaterials) {
                    LoadSelectedOrgPeopleCanvass(arrCanvassMaterialsOrgPeople);
                }
                else {
                    LoadSelectedOrgPeopleCanvass(arrCanvassLaborOrgPeople);
                }
            })
            .on('click', '#btnSearchCanvassOk', function () {
                if (isCanvassMaterials) {
                    whatArr = arrCanvassMaterialsOrgPeople;
                }
                else {
                    whatArr = arrCanvassLaborOrgPeople;
                }

                if (whatArr.length == 0) {
                    $('#divVendorValidation').html('<br>Please select at least one vendor<br><br>').show();
                    return false;
                }
                $('#txtSearchOrgEmp').val('');
                $('#panSearchCanvass').modal('hide');
                $('#panListOfVendors').modal('show');

                LoadSelectedOrgPeopleCanvass(whatArr);
            })
            .on('click', '#btnSKUUnitMeasureOk', function () {
                if ($('input[name="radSKUUnitMeasure"]:checked').val() == undefined) {
                    NewMessageBox('Please select a unit of measure', 'Project', [
                            {
                                Text: 'Ok',
                                Click: function () {
                                    $('#panSKUUnitOfMeasures').popup('open');
                                }
                            }
                    ]);
                    return false;
                }

                var currentActiveDiv = $('div[descriptionid="' + itemAssociateSKUID + '"]');
                var currentCell = currentActiveDiv.closest('tr').find('td');

                currentActiveDiv.attr('associateskuid', arrSelectedSKU.AssociateSKUID);
                currentActiveDiv.attr('skuname', "SKU Name: " + arrSelectedSKU.SKUName)
                currentActiveDiv.closest('td').removeClass('errorCell');
                currentCell.eq(2).addClass('hasAssociatedSKU');
                currentCell.eq(2).append('<pre class="preAssociatedSKU" style="width:' + $('#tBodyExcelLike tr td.activeCell').width() + 'px;">' + arrSelectedSKU.SKUName + '</pre>')
                currentCell.eq(4).find('div').text($('input[name="radSKUUnitMeasure"]:checked').val());
                currentCell.eq(4).find('div').attr('umid', $('input[name="radSKUUnitMeasure"]:checked').attr('umid'))
                $('#panSKUUnitOfMeasures').popup('close');
                isPopupOpen = false;

                if (isCanvassMaterials) {
                    AppendNewAddedRowToCanvassForm('tBodyCanvassMaterial');
                }
                else {
                    AppendNewAddedRowToCanvassForm('tBodyCanvassLabor');
                }
            })
            .on('click', '#btnSKUUnitMeasureCancel', function () {
                $('#panSKUUnitOfMeasures').popup('close');
                isPopupOpen = false;
            })
            .on('click', '#btnSKUUnitMeasureBack', function () {
                SearchSku(sessionStorage.skuname)
                // $('#panSKUUnitOfMeasures').popup('close');
                // $('#panSearchLibrary').popup('open');
                // RepositionPopup();
            })
            .on('click', '#btnAddVendor', function () {
                isPopupOpen = true;
                $('#divVendorValidation').hide();
                $('#panListOfVendors').modal('hide');
                $('#TitleCanvass').text('CANVASS');
                $('#panSearchCanvass').modal('show');
                $('#lstOfOrgEmp').empty();
                Reposition();
            })
            .on('click', '#btnSaveCanvassMaterials', function () {
                //$('#panCanvassOptions').popup('open');
                $('#btnDisplayMinPrice').click();
                //$('#btnComputeParentSubTotal').click();
            })
            .on('click', '#btnPreferredVendor', function () {
                $('#panCanvassOptions').popup('close');

                if (isCanvassMaterials) {
                    isCanvassMaterialsAlready = true;
                    whatArr = arrCanvassMaterialsOrgPeople;
                }
                else {
                    isCanvassLaborAlready = true;
                    whatArr = arrCanvassLaborOrgPeople;
                }

                GetCanvassData();

                if (whatArr.length > 1) {
                    $('#panChooseAVendor').modal('show');
                    $('#lstOfVendor').empty();

                    for (var i = 0; i < whatArr.length; i++) {
                        if (whatArr[i].State != "delete") {
                            $('#lstOfVendor').append($('<li class="list-group-item d-flex align-items-center">')
                                                    .append($('<p class="pr-2 m-0 flex-grow-1">')
                                                    .attr('id', whatArr[i].ID)
                                                    .attr('title', whatArr[i].Name)
                                                    .text(whatArr[i].Name)
                                                    .data('data', whatArr[i])
                                                    .click(function () {
                                                        if (isPrintCanvass) {
                                                            GetOrgDetailsToPrint($(this).data('data'));
                                                            $('#panChooseAVendor').modal('hide');
                                                        }
                                                        else {
                                                            DisplaySelVendorOnProjectBudget($(this).data('data'));
                                                            $('#panChooseAVendor').modal('hide');
                                                            $('.divCanvass').hide();
                                                            $('.divNewProjectBudget').show();
                                                            if (location.pathname.toLowerCase() != '/erp') {
                                                                $('.ItemsBreadCrumbs').text('');
                                                                breadcrumbs.prepend(arrowR, "Project Budget");
                                                                $('.BreadCrumbs').html(breadcrumbs.prepend(lnkProject));
                                                            }                                                                    
                                                        }
                                                    })
                                                )
                                        )
                        }
                    }

                    $('#lstOfVendor');//.listview('refresh');
                    Reposition();
                }
                else {
                    if (isPrintCanvass) {
                        GetOrgDetailsToPrint(whatArr[0]);
                    }
                    else {
                        DisplaySelVendorOnProjectBudget(whatArr[0]);
                        $('.divCanvass').hide();
                        $('.divNewProjectBudget').show();
                        if (location.pathname.toLowerCase() != '/erp') {
                            $('.ItemsBreadCrumbs').text('');
                            breadcrumbs.prepend(arrowR, "Project Budget");
                            $('.BreadCrumbs').html(breadcrumbs.prepend(lnkProject));
                        }
                                
                    }
                }
            })
            .on('click', '#btnDisplayMinPrice', function () {
                GetCanvassData();
                DisplayMinPriceOnProjectBudget();
                if (location.pathname.toLowerCase() != '/erp') {
                    $('.ItemsBreadCrumbs').text('');
                    breadcrumbs.prepend(arrowR, "Project Budget");
                    $('.BreadCrumbs').html(breadcrumbs.prepend(lnkProject));
                }                        
            })
            .on('mouseenter', '.TotalAmountHover', function () {
                $(this).parent().closest('tr').find('td').eq(6).addClass('findCell');
                $(this).parent().closest('tr').find('td').eq(8).addClass('findCell');
            })
            .on('mouseleave', '.TotalAmountHover', function () {
                $(this).parent().closest('tr').find('td').eq(6).removeClass('findCell');
                $(this).parent().closest('tr').find('td').eq(8).removeClass('findCell');
            })
            .on('mouseenter', '.TotalUnitCostHover', function () {
                $(this).parent().closest('tr').find('td').eq(5).addClass('findCell');
                $(this).parent().closest('tr').find('td').eq(7).addClass('findCell');
            })
            .on('mouseleave', '.TotalUnitCostHover', function () {
                $(this).parent().closest('tr').find('td').eq(5).removeClass('findCell');
                $(this).parent().closest('tr').find('td').eq(7).removeClass('findCell');
            })
            .on("mouseenter", 'td', function (event) {
                $(this).find("span.custom-tooltip").css({
                    top: '35px', //event.pageY + 5 + "px",
                    left: '0px' //event.pageX + 5 + "px"
                }).show();
            })
            .on("mouseleave", 'td', function () {
                $("span.custom-tooltip").hide();
            })
            .on('click', '#btnPrintCanvassSheets', function () {
                isPrintCanvass = true;
                $('#panMoreOption').modal('hide');
                $('#btnPreferredVendor').click();
            })
            .on('click', '#btnPrintProjectBudget', function () {
                if (CheckPermissions('Print Project Budget') == false) {
                    return;
                }

                $("#tBodyPrintProjectBudget").empty();
                $("#tBodyProjectBudget tr").each(function () {
                    var tds = $(this), //.find('td'),
                        row = $("<tr>").addClass('tRowStyle').attr('id', $(this).attr('id'));
                    row
                        .append(tds.find('td.tditemno').clone())
                        .append(tds.find('td.tddescription').clone().css({
                                fontWeight: 'normal'
                            })
                        )
                        .append(tds.find('td.tdpointperson').clone())
                        .append(tds.find('td.tdcompletionrate').clone())
                        .append(tds.find('td.tddependency').clone())
                        .append(tds.find('td.tddatestarted').clone())
                        .append(tds.find('td.tddatecompleted').clone())
                        .append(tds.find('td.tdimportance').clone())
                        .append(tds.find('td.tdurgency').clone())
                        .appendTo($('#tBodyPrintProjectBudget'));
                });
                $('#tBodyPrintProjectBudget tr td').removeClass('hasAssociatedSKU').removeAttr('skuname');
                $('#tBodyPrintProjectBudget tr td .preAssociatedSKU').show();
                PrintNowCanvassForm($('#divPrintProjectBudget').html());
            })
            .on('click', '#btnAddNewVendorCanvassSheets', function () {
                $('#panMoreOption').modal('hide');
                $('#lstOfOrgEmp').empty();
                $('#lstOfOrgEmp').children('li').children('div').removeClass('selectedList');
                isPopupOpen = true;
                $('#divSKUMsg').hide();
                $('#TitleCanvass').text('CANVASS');
                $('#panSearchCanvass').modal('show');
                Reposition();
                CountVendorSelected();

                $('#btnCanvassNow').hide();
                $('#btnEditingModeCanvassNow').show();
                if (isCanvassMaterials) {
                    isAddNewVendorCanvassMaterials = true;
                }
                else {
                    isAddNewVendorCanvassLabor = true;
                }
            })
            .on('click', '#btnEditingModeCanvassNow', function () {
                if (isCanvassMaterials) {
                    whatArr = arrCanvassMaterialsOrgPeople;
                }
                else {
                    whatArr = arrCanvassLaborOrgPeople;
                }

                if (whatArr.length == 0) {
                    $('#divVendorValidation').html('<br>Please select at least one vendor<br><br>').show();
                    return false;
                }

                var target, targetTHead;
                isPopupOpen = false;

                $('#divVendorValidation').html('').show();
                $('#panListOfVendors').modal('hide');
                $('.divNewProjectBudget').hide();
                $('.divCanvass').show();

                if (isCanvassMaterials) {
                    $('#divCanvassMaterials').show();
                    $('#divCanvassLabor').hide();
                    target = $("#tBodyCanvassMaterial");
                    targetTHead = $('#tHeadCanvassMaterialRow');
                }
                else {
                    $('#divCanvassMaterials').hide();
                    $('#divCanvassLabor').show();
                    target = $("#tBodyCanvassLabor");
                    targetTHead = $('#tHeadCanvassLaborRow');
                }

                for (var i = 0; i < whatArr.length; i++) {
                    if (targetTHead.find('th:contains("' + whatArr[i].Name + '")').length == 0) {
                        targetTHead.append($('<th>')
                                        .attr('arrindex', i)
                                        .attr('id', whatArr[i].ID)
                                        .attr('title', whatArr[i].Name)
                                        .attr('state', whatArr[i].State)
                                        .text(whatArr[i].Name)
                                        .data('data', whatArr[i])
                                        .css({
                                            maxWidth: '100px',
                                            whiteSpace: 'nowrap',
                                            textOverflow: 'ellipsis',
                                            overflow: 'hidden',
                                            cursor: 'pointer'
                                        })
                                        .click(function (e) {
                                            var id = $(this).attr('ID');
                                            var state = $(this).attr('state');
                                            var arrIndex = $(this).attr('arrIndex');
                                            var newArr;

                                            NewMessageBox('Are you sure want to delete ' + $(this).attr('title') + ' ?', 'Canvass Form', [
                                                    {
                                                        Text: 'Yes',
                                                        Click: function () {
                                                            if (state == "edit") {
                                                                if (isCanvassMaterials) {
                                                                    arrCanvassMaterialsOrgPeople[arrIndex].State = "delete"
                                                                    newArr = arrCanvassMaterialsOrgPeople;
                                                                } else {
                                                                    arrCanvassLaborOrgPeople[arrIndex].State = "delete";
                                                                    newArr = arrCanvassLaborOrgPeople;
                                                                }
                                                            }
                                                            else {
                                                                newArr = $.grep(whatArr, function (val) {
                                                                    return val.ID != id;
                                                                })
                                                            }

                                                            if (isCanvassMaterials) {
                                                                arrCanvassMaterialsOrgPeople = newArr;
                                                            }
                                                            else {
                                                                arrCanvassLaborOrgPeople = newArr;
                                                            }

                                                            RemoveFromCanvassSheet(arrIndex, state);
                                                            CountVendorSelected();
                                                            LoadSelectedOrgPeopleCanvass(newArr);
                                                        }
                                                    },
                                                    {
                                                        Text: 'No'
                                                    }
                                            ])
                                        })
                                    )
                        target.find('tr').each(function () {
                            $(this).append($('<td>')
                                            .append($('<div>')
                                                .css({ width: '100%' })
                                                .html('&nbsp;')
                                                .attr('columnName', 'cost')
                                                .attr('datatype', 'numeric')
                                                .attr('vid', whatArr[i].ID)
                                                .attr('state', 'add')
                                            )
                                            .addClass('tableCell')
                                );
                        })
                    }
                }

                //AppendNewColumn();
            })
            .on('click', '#btnComputeParentSubTotal', function () {
                var arrParentNo = [];

                $('#tBodyExcelLike tr:not(":last")').find('td:eq(2) > div[isparent="1"]').closest('td').prev().each(function () {
                    arrParentNo.push($(this).text());
                })

                for (var i = 0; i < arrParentNo.length; i++) {
                    var lastRowNumber, itemNo, currentRowNumber = 0;
                    var sumMaterialsAmount = 0, sumLaborAmount = 0, sumTotalAmount = 0, materialsAmount = 0, laborAmount = 0, totalAmount = 0;

                    $('#tBodyExcelLike div[descriptionid="' + arrParentNo[i] + '"]').closest('tr').nextAll().each(function () {
                        var currentRow;
                        var currentActiveCell = $(this).find('td:eq(1)').find('div');
                        materialsAmount = ($(this).find('td:eq(6)').find('div').hasClass('disableCell') == true ? 0 : CDec2($(this).find('td:eq(6)').find('div').text()));
                        laborAmount = ($(this).find('td:eq(8)').find('div').hasClass('disableCell') == true ? 0 : CDec2($(this).find('td:eq(8)').find('div').text()));
                        totalAmount = ($(this).find('td:eq(10)').find('div').hasClass('disableCell') == true ? 0 : CDec2($(this).find('td:eq(10)').find('div').text()));

                        if (currentActiveCell.text().indexOf('.') == 1) {
                            itemNo = currentActiveCell.text().split('.')[0]
                        }
                        else {
                            itemNo = currentActiveCell.text();
                        }

                        if (itemNo == arrParentNo[i]) {
                            lastRowNumber = $(this).attr('id');
                            sumMaterialsAmount = sumMaterialsAmount + materialsAmount;
                            sumLaborAmount = sumLaborAmount + laborAmount;
                            sumTotalAmount = sumTotalAmount + totalAmount;
                        }
                        else {
                            currentRow = $('#tBodyExcelLike tr[id="' + lastRowNumber + '"]');
                            if (currentRow.next().find('td:eq(2)').find('div:contains("SUBTOTAL")').length == 0) {
                                currentRow.after($('<tr>')
                                                    .attr('rowType', 'subtotal')
                                                    .addClass('rowSubtotal')
                                                    .append($('<td>').css('display', 'none'))
                                                    .append($('<td>').append($('<div>')))
                                                    .append($('<td>').append($('<div>').attr('state', 'add').html('<strong>SUBTOTAL ' + arrParentNo[i] + ' : </strong>')))
                                                    .append($('<td>').append($('<div>')))
                                                    .append($('<td>').append($('<div>')))
                                                    .append($('<td>').append($('<div>').text('')))
                                                    .append($('<td>').append($('<div>').addClass('NumericCell').text(AccountingFormat($.format.number(sumMaterialsAmount)))))
                                                    .append($('<td>').append($('<div>').text('')))
                                                    .append($('<td>').append($('<div>').addClass('NumericCell').text(AccountingFormat($.format.number(sumLaborAmount)))))
                                                    .append($('<td>').append($('<div>').text('')))
                                                    .append($('<td>').append($('<div>').addClass('NumericCell').text(AccountingFormat($.format.number(sumTotalAmount)))))

                                                )
                            }
                            else {
                                currentRow.next().find('td:eq(5)').find('div').text('');
                                currentRow.next().find('td:eq(7)').find('div').text('');
                                currentRow.next().find('td:eq(9)').find('div').text('');
                                currentRow.next().find('td:eq(6)').find('div').text(AccountingFormat($.format.number(sumMaterialsAmount)));
                                currentRow.next().find('td:eq(8)').find('div').text(AccountingFormat($.format.number(sumLaborAmount)));
                                currentRow.next().find('td:eq(10)').find('div').text(AccountingFormat($.format.number(sumTotalAmount)));
                            }

                            return false;
                        }

                    })
                }

                //Updating Row ID or Number
                $('#tBodyExcelLike tr:first').nextAll().each(function () {
                    var id = $(this).prop("id", function (index, idvalue) {
                        return currentRowNumber + 1;
                    });
                    currentRowNumber = currentRowNumber + 1;
                });
            })
            .on('click', '#btnCreateNewVendor', function () {
                $('#panSearchCanvass').modal('hide');
                $('#pageProjectDetails').pageshow();
                $('#divNewProjectDetails').hide();
                $('#divNewProjectCustomer').show();
                ClearNewProjectCustomerFields();
                GetCityListAll();
                GetCountryListAll();
            })
            .on('click', '#btnCanvassOptions', function () {
                $('#panCanvassOption').popup('open');
            })
            .on('click', '#btnTimeline', function () {
                if (CheckPermissions("Add/Edit Timeline") == false) {
                    return false;
                }

                $('.divNewProjectBudget, .divCanvass').hide();
                $('.divTimeline').show();

                if (isFinishedLoadTimeline == false) {
                    var ctr = 0;
                    $('#tBodyTimeline').empty();

                    $("#tBodyExcelLike tr").not('tr:last').each(function () {
                        if ($(this).hasClass('rowSubtotal') != true) {
                            var tds = $(this).find('td'),
                                row = $("<tr>").attr('descriptionrowid', $(this).attr('descriptionrowid'));
                            row
                                .append(tds.eq(1).addClass('ReadOnly').clone())
                                .append(tds.eq(2).addClass('ReadOnly').clone().addClass('tableCellDisable'))
                                .append($('<td>').append($('<select>')
                                                            .addClass('selTimeline')
                                                            .append('<option value="1-Not Very Important">1 - Not Very Important</option><option value="2-Not Important">2 - Not Important</option><option value="3-Neutral" selected>3 - Neutral</option><option value="4-Important">4 - Important</option><option value="5-Very Important">4 - Very Important</option>')
                                                        )
                                                .addClass('tableCell')
                                                .css('text-align', 'center')
                                )
                                .append($('<td>').append($('<select>')
                                                            .addClass('selTimeline')
                                                            .append('<option value="1-Not Very Urgent">1 - Not Very Urgent</option><option value="2-Not Urgent">2 - Not Urgent</option><option value="3-Neutral" selected>3 - Neutral</option><option value="4-Urgent">4 - Urgent</option><option value="5-Very Urgent">4 - Very Urgent</option>')
                                                        )
                                                .addClass('tableCell')
                                                .css('text-align', 'center')
                                )
                                .append($('<td>').append($('<button class="btnForTimeline" data-mini="true">')
                                                                .text('Set')
                                                                .attr('id', 'btnSet-' + ctr)
                                                                .click(function (e) {
                                                                    var thisButton = $(this);
                                                                    isPopupOpen = true;
                                                                    NewMessageBox('<input type="text" datatype="numeric" placeholder="Time" id="txtTimeValue">' +
                                                                            '<select id="selTimeBase"><option value="hour">Hour(s)</option><option value="minute">Minute(s)</option></select>',
                                                                            'Time Need to Complete',
                                                                            [
                                                                                {
                                                                                    Text: 'Ok',
                                                                                    Validation: function () {
                                                                                        if ($.trim($('#txtTimeValue').val()) == "") {
                                                                                            $('#txtTimeValue').focus();
                                                                                            return false;
                                                                                        }
                                                                                        return true;
                                                                                    },
                                                                                    Click: function () {
                                                                                        thisButton.text($('#txtTimeValue').val() + ' ' + $('#selTimeBase').val() + '(s)');//.button('refresh');
                                                                                        isPopupOpen = false;
                                                                                    },
                                                                                    Theme: 'b'
                                                                                },
                                                                                {
                                                                                    Theme: 'b',
                                                                                    Text: 'Clear Set Time',
                                                                                    Click: function () {
                                                                                        thisButton.text('Set');//.button('refresh');
                                                                                        isPopupOpen = false;
                                                                                    }
                                                                                },
                                                                                {
                                                                                    Text: 'Cancel',
                                                                                    Click: function () {
                                                                                        isPopupOpen = false;
                                                                                    }
                                                                                }
                                                                            ]
                                                                        )
                                                                })
                                                                .contextmenu(function (e) {
                                                                    e.preventDefault();
                                                                    var thisButton = $(this);
                                                                    $('body').append($('<div>')
                                                                                .append($('<ul>')
                                                                                    .append($('<a>')
                                                                                        .append($('<li>')
                                                                                            .html('Clear')
                                                                                            .click(function (e) {
                                                                                                thisButton.text('Set');//.button('refresh');
                                                                                            })
                                                                                        )
                                                                                    )
                                                                                )
                                                                                .addClass('context-menu')
                                                                                .css({ top: e.pageY + "px", left: e.pageX + "px" })
                                                                            )
                                                                })
                                                        ).addClass('tableCell'))
                                .append($('<td>').append($('<button class="btnForTimeline" data-mini="true">')
                                                                .text('Select')
                                                                .attr('id', 'btnAssigned-' + ctr)
                                                                .click(function (e) {
                                                                    var thisButton = $(this);
                                                                    isPopupOpen = true;
                                                                    BrowsePopup('LibraryPeople', 'Assigned To', [
                                                                            {
                                                                                AddButton: [
                                                                                    {
                                                                                        Text: 'Clear Button Field',
                                                                                        Click: function () {
                                                                                            thisButton.removeAttr('pplid').text('Select');//.button('refresh');
                                                                                            isPopupOpen = false;
                                                                                        }
                                                                                    }
                                                                                ],
                                                                                DisplayText: [
                                                                                    {
                                                                                        Column: 'PeopleName'
                                                                                    }
                                                                                ],
                                                                                Click: function (vData) {
                                                                                    thisButton.attr('pplid', vData.ID).text(vData.PeopleName);//.button('refresh');
                                                                                    isPopupOpen = false;
                                                                                }
                                                                            }
                                                                    ])
                                                                })
                                                                .contextmenu(function (e) {
                                                                    e.preventDefault();
                                                                    var thisButton = $(this);
                                                                    $('.context-menu').remove();
                                                                    $('body').append($('<div>')
                                                                                .append($('<ul>')
                                                                                    .append($('<a>')
                                                                                        .append($('<li>')
                                                                                            .html('Clear')
                                                                                            .click(function (e) {
                                                                                                thisButton.removeAttr('pplid').text('Select');//.button('refresh');
                                                                                            })
                                                                                        )
                                                                                    )
                                                                                )
                                                                                .addClass('context-menu')
                                                                                .css({ top: e.pageY + "px", left: e.pageX + "px" })
                                                                            )
                                                                })
                                                        ).addClass('tableCell'))
                                .append($('<td>').append($('<select>')
                                                            .addClass('selTimeline')
                                                            .append('<option value="Not Started">Not Started</option><option value="On Going">On Going</option><option value="Extended">Extended</option><option value="Delayed">Delayed</option><option value="Finished">Finished</option>')
                                                        )
                                                .addClass('tableCell')
                                                .css('text-align', 'center')
                                )
                                .append($('<td>').append($('<select>')
                                                            .addClass('selTimeline')
                                                            .addClass('selDependency')
                                                        )
                                                .addClass('tableCell')
                                                .css('text-align', 'center')
                                )
                                .append($('<td>').append($('<div>')
                                                            .css({ width: '100%' })
                                                            .html('&nbsp;')
                                                            .attr('data-role', 'none')
                                                            .attr('isRequired', 0)
                                                            .attr('state', 'add')
                                                        )
                                                .addClass('tableCell')
                                                .css('word-wrap', 'break-word')
                                )
                                .appendTo($('#tBodyTimeline'));
                            ctr = ctr + 1;
                        }
                    });

                    $('.selDependency').append('<option value="None">None</option>');
                    $('#tBodyTimeline tr').each(function () {
                        var itemNo = $(this).find('td:eq(0) div').text();
                        $('.selDependency').append('<option value="' + itemNo + '">' + itemNo + '</option>');
                    })
                    //$('.selTimeline');//.selectmenu();
                    $('.btnForTimeline');//.button();
                    isFinishedLoadTimeline = true;
                    GetProjectBudgetTimeline();
                }
            })
            .on('click', '#tBodyTimeline .tableCell', function () {
                $('#tBodyTimeline tr td').removeClass('activeCell');
                $('#tBodyTimeline tr').removeClass('activeRow');
                $('#tBodyTimeline tr').removeClass('rowSelected');
                $(this).addClass('activeCell');
                $(this).parent().addClass('activeRow');
            })
            .on('click', '#btnTimelineCancel', function () {
                $('.divTimeline').hide();
                $('#divProjectBudget, .divNewProjectBudget').show();
                if (location.pathname.toLowerCase() != '/erp') {
                    $('.ItemsBreadCrumbs').text('');
                    breadcrumbs.prepend(arrowR, "Project Budget");
                    $('.BreadCrumbs').html(breadcrumbs.prepend(lnkProject));
                }                        
            })
            .on('keypress', '#txtTimeValue', function (e) {
                return isNumberKey(e);
            })
            .on('blur', '#tBodyTimeline .typeText', function (e) {
                if (isFocusOut == false) {
                    $(this).replaceWith($('<div>')
                                        .attr('data-role', 'none')
                                        .attr('state', $(this).attr('state'))
                                        .css({ width: '100%' })
                                        .html($(this).val())
                                    );
                }
                isFocusOut = false;
            })
            .on('keypress', '#tBodyTimeline .typeText', function (e) {
                e.stopPropagation();

                if (e.which == 13 || e.which == 38 || e.which == 40 || e.which == 39) {
                    isFocusOut = true;
                    $(this).replaceWith($('<div>')
                                        .attr('data-role', 'none')
                                        .attr('state', $(this).attr('state'))
                                        .css({ width: '100%' })
                                        .html($(this).val())
                                    );
                    if (e.which == 39) {
                        $('#tBodyTimeline tr td.activeCell').closest('td').next().addClass('activeCell');
                        $('#tBodyTimeline tr td.activeCell').closest('td').prev().removeClass('activeCell');
                    }
                    else {
                        $('#tBodyTimeline tr td.activeCell').closest('tr').next().find('td:eq(' + $('#tBodyTimeline tr td.activeCell').closest('td').index() + ')').addClass('activeCell');
                        $('#tBodyTimeline tr td.activeCell').closest('tr').prev().find('td').removeClass('activeCell')
                    }
                }
            })
            .on('click', '#btnTimelineOk', function () {
                arrTimeline = [];

                $('#tBodyTimeline tr').each(function () {
                    var td = $(this).find('td');
                    arrTimeline.push({
                        "tlrowid": ($(this).attr('tlrowid') == undefined ? "" : $(this).attr('tlrowid')),
                        "OrgID": sessionStorage.OrgID,
                        "FOWSTID": $.trim(sessionStorage.ProjectID),
                        "ProductID": ($(this).attr('descriptionrowid') == undefined ? "" : $(this).attr('descriptionrowid')),
                        "Importance": td.eq(2).find('select').val(),
                        "Urgency": td.eq(3).find('select').val(),
                        "TimeToComplete": td.eq(4).find('button').text(),
                        "AssignedTo": td.eq(5).find('button').text(),
                        "AssignedToPeopleID": (td.eq(5).find('button').attr('pplid') == undefined ? "" : td.eq(5).find('button').attr('pplid')),
                        "Status": td.eq(6).find('select').val(),
                        "Dependency": td.eq(7).find('select').val(),
                        "Tags": $.trim(td.eq(8).find('div').text())
                    })
                })

                $('.divTimeline').hide();
                $('#divProjectBudget, .divNewProjectBudget').show();
                $('.custom-btn-tab').removeClass('custom-btn-theme-b');
                $('#btnProjectSummary').removeClass('custom-btn-theme-c').addClass('custom-btn-theme-b');
            })
            .on('click', '.btnMoreOptionProjectBudget', function () {
                $('#panMoreOption').modal('show');
            })
            .on('click', '#btnProjectSummary', function () {
                $('.divCanvass, #divPrintCanvass, .divTimeline, #btnPrintCanvassSheets, #btnAddNewVendorCanvassSheets').hide();
                $('.divNewProjectBudget, #btnDisplayAssociatedSKU').show();
            })
            .on('click', '.custom-btn-tab', function () {
                $('.custom-btn-tab').removeClass('custom-btn-theme-b');
                $(this).removeClass('custom-btn-theme-c').addClass('custom-btn-theme-b');
            })
            .on('click', '#btnTabMaterialsReq', function () {
                CallPage('#tabMaterialsReq', htmlMaterialsReq, '/MaterialsRequisition', function (e) {
                    htmlMaterialsReq = e;
                    $('#divHeaderMaterialRequisition').remove();
                })
            })
            .on('click', '#btnTabProjectPO', function () {
                CallPage('#tabProjectPO', htmlPO, '/ProjectPurchaseOrder', function (e) {
                    htmlPO = e;
                    $('#divHeaderProjectPO').remove();
                })
            })
            .on('click', '#btnRefreshProject', function () {
                GetProjectHeader();
            })
            .on('click', '#btnMultipleSKUUnitMeasureOk', function () {
                var trMultipleSKU = $('#tblBodyMultipleSKU tr');
                var isSelected = true;
                var arrSKUMaterials = [];

                trMultipleSKU.each(function () {
                    var data = JSON.parse(JSON.stringify($(this).data('data')));
                    var um = $(this).find('td:eq(1)').find('input:checked');
                    if (um.length == 0) {
                        WebToast('Please select unit measure for ' + data.ProductName, 'top');
                        isSelected = false;
                        return false;
                    } else {
                        arrSKUMaterials.push({
                            UomName: um.data('data').Measurement,
                            UomID: um.data('data').UnitMeasureID,
                            SkuID: data.ID,
                            SkuName: data.ProductName,
                            State: 'new'
                        })
                    }
                })
                        
                if (isSelected) {
                    $('div[descriptionid="' + itemAssociateSKUID + '"]').closest('tr')
                        .data('materials', arrSKUMaterials)
                    $('#popMultipleSKUUnitOfMeasures').popup('close');
                    isAddChildMultiple = true;
                    for (var i = 0; i < arrSKUMaterials.length; i++) {
                        AddChild(arrSKUMaterials[i].SkuName, arrSKUMaterials[i].UomName, true, true);
                        UpdateRowIDs();
                        //$('<tr class="rowMaterials">')
                        //    .append($('<td>'))
                        //    .append($('<td>').text(arrSKUMaterials[i].SkuName))
                        //    .insertAfter($('div[descriptionid="' + itemAssociateSKUID + '"]').closest('tr'))
                    }
                    isAddChildMultiple = false;
                }
            })
            .on('click', '#btnProjectNew', function () {
                NewMessageBox('Are you sure to want to create a new transaction? Any unsaved data will be lost.', 'Message', [
                    {
                        Text: 'Yes', Click: function (e) {
                            isAddProject = true;
                            ClearAllFields();
                            $('#popupNewProject').modal('show');
                        }
                    }, { Text: 'No' }
                ])                
            })
            .on('click', '#btnProjectLookUp', function () {
                BrowsePopup('/BOMProjectExcel/GetProjectHeader', 'List of Projects', [{
                    ModalSize: 'xl',
                    ServerSideProcessing: true,
                    DisplayText: [{ Label: '#', Column: 'RowNum' }, { Label: 'Project Name', Column: 'ProjectName' }
                        , { Label: 'Project Code', Column: 'ProjectCode' }
                        , {
                            Label: 'Project Start Date', Column: function (data) {
                                return $.format.date(new Date(data.ProjectedStartDate), 'MM-dd-yyyy')
                            }
                        }
                        , {
                            Label: 'Actual Start Date', Column: function (data) {
                                return data.ActualStartDate == null ? '' : $.format.date(new Date(data.ActualStartDate), 'MM-dd-yyyy')
                            }
                        }
                        , {
                            Label: 'Projected Completion Date ', Column: function (data) {
                                return $.format.date(new Date(data.ProjectedCompletionDate), 'MM-dd-yyyy')
                            }
                        }
                        , {
                            Label: 'Actual Completion Date', Column: function (data) {
                                return data.ActualCompletionDate == null ? '' : $.format.date(new Date(data.ActualCompletionDate), 'MM-dd-yyyy')
                            }
                        }
                        , {
                            Label: 'Last Updated', Column: function (data) {
                                return $.format.date(new Date(data.LastUpdated), 'MM-dd-yyyy hh:mm a')
                            }
                        }],
                    AutoSearch: true,
                    Click: function (e) {
                        ViewProject(e);
                    }
                }])
                //$('#popupProjectList').show();
            })
            .on('change', '#selFilterProjectSummary', function () {
                FilterProjectSummary()
            })
            .on('click', '#btnProjectAddRemarks', function () {
                var cell = $('td.activeCell').find('div');
                var celldata = cell.data('data');
                var celldatafiles = JSON.parse(JSON.stringify(cell.data('files') || []));
                var imgs = $('#txtProjectRemarks').find('img');
                var availableimgs = [];

                imgs.each(function (e) {
                    $(this).removeAttr('src');
                    availableimgs.push($(this).attr('imgid'));
                })

                for (var i = 0; i < celldatafiles.length; i++){
                    if (!availableimgs.includes(celldatafiles[i].ID)) {
                        if (celldatafiles[i].status == 'new') {
                            cell.data('files').splice(i, 0);
                        } else {
                            cell.data('files')[i].status = 'remove';
                        }                        
                    }
                }

                var data = {
                    ID: '',
                    Remarks: HtmlEncode($('#txtProjectRemarks').html()),
                    ModifiedByID: sessionStorage.PeopleID                            
                };

                if(celldata.length == 0){
                    celldata.push(data);
                } else {
                    var copydata = JSON.parse(JSON.stringify(celldata)).splice(-1)[0];
                    var lastindex = celldata.length - 1;

                    if (CStr2(celldata[lastindex].ID) == '') {
                        celldata[lastindex] = $.extend(copydata, data);
                    } else {
                        celldata.push(data);
                    }
                }

                $('#txtProjectRemarks').find('img').remove();
                cell.html(HtmlDecode($('#txtProjectRemarks').html()).substring(0, 50) + ' ...');
                $('#popupProjectRemarks').modal('hide');
                $('#txtProjectRemarks').val('');
                RefreshFreezePanesHeight($('#tBodyExcelLike tr td.activeCell').closest('tr').attr('id'));
            })
            .on('click', '#btnOkProjectDuration', function () {
                var cell = $('td.activeCell').find('div');
                var celldata = cell.data('data');
                var data = {
                    ID: '',
                    Duration: $.trim($('#txtProjectDuration').val()),
                    StateIn: $('#selProjectStateIn').val()
                }

                if(celldata.length == 0){
                    celldata.push(data)
                } else {
                    var copydata = JSON.parse(JSON.stringify(celldata[0]));
                    celldata[0] = $.extend(copydata, data);
                }

                cell.text(data.Duration + ' ' + data.StateIn);
                $('#popProjectDuration').modal('hide');
            })
            .on('click', '#btnOkMaterialBudgetQty', function () {
                var cell = $('td.activeCell').find('div');
                var celldata = cell.data('data');
                var txtMaterialBudgetQty = $('#txtMaterialBudgetQty');
                var selMaterialBudgetUom = $('#selMaterialBudgetUom');

                if (!IsNumeric($.trim(txtMaterialBudgetQty.val()))) {
                    WebToast('Please enter quantity', 'top');
                    return;
                }

                //if (selMaterialBudgetUom.val() == '') {
                //    WebToast('Please select unit of measure', 'top');
                //    return;
                //}

                var data = {
                    ID: '',
                    Qty: $.trim(txtMaterialBudgetQty.val()),
                    //UomID: $.trim(selMaterialBudgetUom.val()),
                    ModifiedBy: sessionStorage.PeopleID
                };

                if (celldata.length == 0) {
                    celldata.push(data);
                } else {
                    var copydata = JSON.parse(JSON.stringify(celldata)).splice(-1)[0];
                    var lastindex = celldata.length - 1;

                    if (CStr2(celldata[lastindex].ID) == '') {
                        celldata[lastindex] = $.extend(copydata, data);
                    } else {
                        celldata.push(data);
                    }
                }

                cell.text(data.Qty); // + ' ' + $('#selMaterialBudgetUom option:selected').text()
                $('#popupProjectMaterialBudgetQty').modal('hide');
            })
            .on('click', '#btnOkMaterialAccReqQty', function () {
                var cell;
                var isTdReqQty = $('td.activeCell').hasClass('tdreqqty');

                if (isTdReqQty) {
                    cell = $('td.activeCell').find('div');
                } else {
                    cell = $('td.activeCell').prev().find('div');
                }

                var celldata = cell.data('data');
                var txtAccReqQty = $('#txtAccReqQty');
                //var selAccReqUom = $('#selAccReqUom');
                var txtAccReqRemarks = $('#txtAccReqRemarks');
                var selAccReqBy = $('#selAccReqBy');
                var txtAccReqDateNeeded = $('#txtAccReqDateNeeded');
                var txtAccReqTimeFrom = $('#txtAccReqTimeFrom');
                var txtAccReqTimeTo = $('#txtAccReqTimeTo');

                if (!IsNumeric($.trim(txtAccReqQty.val()))) {
                    WebToast('Please enter quantity', 'top');
                    return;
                }

                if (CStr2($.trim(selAccReqBy.val())) == '') {
                    WebToast('Please select requested by', 'top');
                    return;
                }

                if (CStr2($.trim(txtAccReqDateNeeded.val())) == '') {
                    WebToast('Please set date needed', 'top');
                    return;
                }

                if (CStr2($.trim(txtAccReqTimeFrom.val())) != '' || CStr2($.trim(txtAccReqTimeTo.val())) != '') {
                    if (CStr2($.trim(txtAccReqTimeFrom.val())) != '' && CStr2($.trim(txtAccReqTimeTo.val())) == '') {
                        WebToast('Please set time To', 'top');
                        return;
                    }

                    if (CStr2($.trim(txtAccReqTimeFrom.val())) == '' && CStr2($.trim(txtAccReqTimeTo.val())) != '') {
                        WebToast('Please set time From', 'top');
                        return;
                    }
                }

                //if (selAccReqUom.val() == '') {
                //    WebToast('Please select unit of measure', 'top');
                //    return;
                //}

                var data = {
                    ID: '',
                    Qty: $.trim(txtAccReqQty.val()),
                    //UomID: $.trim(selAccReqUom.val()),
                    RequestedByID: $.trim(selAccReqBy.val()),
                    RequestedByName: $('#selAccReqBy').val(),
                    Remarks: $.trim(txtAccReqRemarks.val()),
                    DateNeeded: $.trim(txtAccReqDateNeeded.val()),
                    TimeNeededFrom: $.trim(txtAccReqTimeFrom.val()),
                    TimeNeededTo: $.trim(txtAccReqTimeTo.val()),
                    ModifiedBy: sessionStorage.PeopleID,
                    State: 'new'
                };

                if (celldata.length == 0) {
                    celldata.push(data);
                } else {
                    var copydata = JSON.parse(JSON.stringify(celldata)).splice(-1)[0];
                    var lastindex = celldata.length - 1;

                    if (CStr2(celldata[lastindex].ID) == '') {
                        celldata[lastindex] = $.extend(copydata, data);
                    } else {
                        celldata.push(data);
                    }
                }

                var total = 0;
                $.grep(celldata, function (e) {
                    if(e.State != 'cancelled'){
                        total += CDec2(e.Qty);
                    }                    
                })
                        
                if (isTdReqQty) {
                    cell.text(data.Qty);
                    $('td.activeCell').next().find('div').text(total);
                } else {
                    $('td.activeCell').prev().find('div').text(data.Qty);
                    $('td.activeCell').find('div').text(total);
                }

                $('#popupProjectMaterialAccReqQty').modal('hide');
            })
            .on('keydown', '#txtAccReqQty', function (e) {
                if(!NumberDotOnly($(this), e)){
                    return;
                }
            })
            .on('click', '#btnAccReqBy', function () {
                BrowsePopup('LibraryPeople', 'Requested By', [
                    {
                        DisplayText: [{ Column: 'PeopleName' }],
                        Click: function (e) {
                            $('#txtReqByID').val(e.ID);
                            $('#btnAccReqBy').text(e.PeopleName);//.button('refresh');
                        }
                    }
                ])
            })
            .on('click', '#btnOkMaterialAccUsedUp', function () {
                var cell = $('td.activeCell').find('div');
                var celldata = cell.data('data');
                var txtAccUsedUpQty = $('#txtAccUsedUpQty');
                //var selAccReqUom = $('#selAccReqUom');
                var txtAccUsedUpRemarks = $('#txtAccUsedUpRemarks');

                if (!IsNumeric($.trim(txtAccUsedUpQty.val()))) {
                    WebToast('Please enter quantity', 'top');
                    return;
                }

                //if (selAccReqUom.val() == '') {
                //    WebToast('Please select unit of measure', 'top');
                //    return;
                //}

                var data = {
                    ID: '',
                    Qty: $.trim(txtAccUsedUpQty.val()),
                    //UomID: $.trim(selAccReqUom.val()),
                    Remarks: $.trim(txtAccUsedUpRemarks.val()),
                    ModifiedBy: sessionStorage.PeopleID
                };

                if (celldata.length == 0) {
                    celldata.push(data);
                } else {
                    var copydata = JSON.parse(JSON.stringify(celldata)).splice(-1)[0];
                    var lastindex = celldata.length - 1;

                    if (CStr2(celldata[lastindex].ID) == '') {
                        celldata[lastindex] = $.extend(copydata, data);
                    } else {
                        celldata.push(data);
                    }
                }

                var total = 0;
                $.grep(celldata, function (e) {
                    total += CDec2(e.Qty)
                })

                cell.text(total);
                $('#popupProjectMaterialAccUsedUpQty').modal('hide');
            })
            .on('click', '#btnOkLaborDisburse', function () {
                var cell = $('td.activeCell').find('div');
                var celldata = cell.data('data');
                var txtLaborReqAmount = $('#txtLaborReqAmount');
                var selLaborRequestedBy = $('#selLaborRequestedBy');
                var txtLaborReqByID = $('#txtLaborReqByID');
                var txtLaborPaymentBalance = $('#txtLaborPaymentBalance');
                var txtLaborRemarks = $('#txtLaborRemarks');

                if (!IsNumeric($.trim(txtLaborReqAmount.val()))) {
                    WebToast('Please enter amount', 'top');
                    return;
                }

                if ($.trim(selLaborRequestedBy.val()) == '') {
                    WebToast('Please select requested by', 'top');
                    return;
                }

                var data = {
                    ID: '',
                    Amount: $.trim(txtLaborReqAmount.val()),
                    RequestedByID: $.trim(selLaborRequestedBy.val()),
                    RequestedByName: $.trim(selLaborRequestedBy.find('option:selected').val()),
                    Remarks: $.trim(txtLaborRemarks.val()),
                    PaymentBalance: $.trim(txtLaborPaymentBalance.val()),
                    ModifiedBy: sessionStorage.PeopleID,
                    State: 'requested'
                };

                if (celldata.length == 0) {
                    celldata.push(data);
                } else {
                    var copydata = JSON.parse(JSON.stringify(celldata[0]));

                    if (CStr2(celldata[0].ID) == '') {
                        celldata[0] = $.extend(copydata, data);
                    } else {
                        celldata.push(data);
                    }
                }

                var total = 0;
                $.grep(celldata, function (e) {
                    total += CDec2(e.Amount)
                })

                cell.text(total);
                $('#popupProjectAccDisbursedCost').modal('hide');
            })
            .on('click', '#btnLaborRequestedBy', function () {
                BrowsePopup('LibraryPeople', 'Requested By', [
                    {
                        DisplayText: [{ Column: 'PeopleName' }],
                        Click: function (e) {
                            $('#txtLaborReqByID').val(e.ID);
                            $('#btnLaborRequestedBy').text(e.PeopleName);//.button('refresh');
                        }
                    }
                ])
            })
            .on('keypress', '#txtProjectListSearch', function (e) {
                if(e.which == 13){
                    GetProjectHeader('', '', $.trim($(this).val()));
                }
            })
            .on('click', '#btnOkDependency', function () {
                var cell = $('td.activeCell').find('div');
                var dep = [];

                $('.chbProjectDependency:checked').each(function () {
                    dep.push($(this).val());
                })

                cell.data('data').Dependency = dep.join(', ');
                cell.html(dep.join(', '));
                $('#popupProjectTaskDependency').modal('hide');
                RefreshFreezePanesHeight($('#tBodyExcelLike tr td.activeCell').closest('tr').attr('id'));
            })
            .on('click', '.selProjectImportance', function (e) {
                if (CheckPermissions("Set/Change Task Importance") == false) {
                    e.preventDefault();
                    $(this).find('option').css('display', 'none');
                    return;
                } else {
                    $(this).find('option').css('display', 'block');
                }
            })
            .on('click', '.selProjectUrgency', function (e) {
                if (CheckPermissions("Set/Change Task Urgency") == false) {
                    e.preventDefault();
                    $(this).find('option').css('display', 'none');
                    return;
                } else {
                    $(this).find('option').css('display', 'block');
                }
            })
            .on('click', '.selProjectUom', function (e) {
                if (CheckPermissions("Set/Change Material Unit Of Measure") == false) {
                    e.preventDefault();
                    $(this).find('option').css('display', 'none');
                    return;
                } else {
                    $(this).find('option').css('display', 'block');
                }
            })
            .on('click', '#txtAccReqDateNeeded', function () {
                var dis = $(this);
                SingleCalendar({
                    ClearButton: true,
                    Click: function (e) {
                        dis.val(e);
                    }
                })
            })
            .on('click', '#btnAccReqClearTime', function () {
                $('.txtAccReqTime').val('');
            })
            .on('click', '.txtAccReqTime', function () {
                var dis = $(this);
                CustomSetTime({
                    Click: function (e) {
                        dis.val(e.time);
                    }
                })
            })
            .on('click', '#btnProjectRemarksAddPhotos', function () {
                $('#fileProjectRemarksPhotos').click()
            })
            .on('change', '#fileProjectRemarksPhotos', function (e) {
                var cell = $('td.activeCell').find('div');
                var itemno = $.trim($('td.activeCell').parents().closest('tr').find('td.tditemno').text());
                var datafiles = cell.data('files');
                var newphotosformdata = cell.data('newphotosformdata');
                var fileUpload = $(this).get(0);
                var files = fileUpload.files;

                for (var i = 0; i < files.length; i++) {
                    var tempid = NewID();

                    datafiles.push({
                        ID: tempid,
                        filename: files[i].name,
                        size: files[i].size,
                        status: 'new'
                    })
                    newphotosformdata.push(files[i]);

                    var reader = new FileReader();

                    reader.onload = function (e) {
                        placeCaretAtEnd($('#txtProjectRemarks')[0])
                        insertAtCursor('img', 'img', e.target)
                        placeCaretAtEnd($('#txtProjectRemarks')[0])

                        //DisplayRemarksPhotos({
                        //    src: e.target.result,
                        //    filename: e.target.fileName,
                        //    isnew: true
                        //});
                    }
                    reader.filestatus = 'new';
                    reader.fileid = tempid;
                    reader.fileName = itemno + '-' + files[i].name;
                    reader.readAsDataURL(files[i]);
                    projectData.append(itemno + '-' + files[i].name, files[i]);
                }
            })
            .on('click', '#btnProjectAttachments', function () {
                if (CStr2(sessionStorage.ProjectID) == '') {
                    WebToast('Please select Project first', 'top');
                    return;
                }
                $('#popupProjectAttachments').modal('show');
                GetProjectFiles()
            })
            .on('click', '#btnProjectFiles', function () {
                arrProjectFiles = [];
                projectFilesFormData = new FormData();
                $('#fileProjectFiles').val('');
                $('#fileProjectFiles').click()
            })
            .on('change', '#fileProjectFiles', function (e) {
                var fileUpload = $(this).get(0);
                var files = fileUpload.files;
                $('#tBodyProjecAttachmentsDetails').empty();
                DisplayProjectFilesToUpload(files)                
                $('#popupProjectAttachmentsDetails').modal('show');
            })
            .on('click', '#btnAddAnotherProjectFiles', function () {
                $('#fileAddAnotherProjectFiles').val('');
                $('#fileAddAnotherProjectFiles').click()
            })
            .on('change', '#fileAddAnotherProjectFiles', function (e) {
                var fileUpload = $(this).get(0);
                var files = fileUpload.files;
                DisplayProjectFilesToUpload(files)
            })
            .on('click', '#btnProjectAttachmentsUpload', function () {
                if (arrProjectFiles.length == 0) {
                    NewMessageBox('No file(s) to upload', 'Message');
                    return;
                }
                //console.log(arrProjectFiles)
                //return
                projectFilesFormData.delete('data');
                projectFilesFormData.delete('ProjectID');
                projectFilesFormData.append('data', JSON.stringify(arrProjectFiles));
                projectFilesFormData.append('ProjectID', sessionStorage.ProjectID);

                $(this).attr('disabled', true);
                NewMessageBox('Uploading file(s) ... please wait', 'Message');                

                AjaxSend({
                    url: 'BOMProjectExcel/UploadProjectFiles?OrgID=' + sessionStorage.OrgID,
                    data: projectFilesFormData,
                    async: true,
                    cache: false,
                    contentType: false,
                    processData: false,
                    dataType: 'json',
                    success: function (vResult) {
                        NewMessageBoxClose();
                        $('#btnProjectAttachmentsUpload').removeAttr('disabled');

                        if (vResult.Results == "saved") {
                            $('#popupProjectAttachmentsDetails').modal('hide');
                            WebToast('Successfully Saved', 'top');
                            GetProjectFiles();
                        }
                        else {
                            NewMessageBox(vResult.Results);
                        }
                    }
                })
            })
            .on('change', '#selProjectNewClientType', function () {
                if($(this).val() == 'org'){
                    $('#divProjectNewClientOrg').pageshow();
                    $('#txtProjectNewClientOrgName, #txtProjectNewClientOrgTIN').val('');
                } else {
                    $('#divProjectNewClientPerson').pageshow();
                    $('#txtProjectNewClientPersonFname, #txtProjectNewClientPersonMname, #txtProjectNewClientPersonLname').val('');
                }
            })
            .on('click', '#btnProjectSaveNewClient', function () {
                var selProjectNewClientType = $('#selProjectNewClientType').val();
                var txtProjectNewClientOrgName = $('#txtProjectNewClientOrgName').val();
                var txtProjectNewClientOrgTIN = $('#txtProjectNewClientOrgTIN').val();
                var txtProjectNewClientPersonFname = $('#txtProjectNewClientPersonFname').val();
                var txtProjectNewClientPersonMname = $('#txtProjectNewClientPersonMname').val();
                var txtProjectNewClientPersonLname = $('#txtProjectNewClientPersonLname').val();
                var txtProjectNewClientHomeNo = $('#txtProjectNewClientHomeNo').val();
                var txtProjectNewClientStreetName = $('#txtProjectNewClientStreetName').val();
                var selProjectNewClientCity = $('#selProjectNewClientCity').val();
                var selProjectNewClientCountry = $('#selProjectNewClientCountry').val();
                var selProjectNewClientCountryCode = $('#selProjectNewClientCountryCode').val();
                var txtProjectNewClientMobileNo = $('#txtProjectNewClientMobileNo').val();
                var txtProjectNewClientEmail = $('#txtProjectNewClientEmail').val();

                if (selProjectNewClientType == 'org') {
                    if ($.trim(txtProjectNewClientOrgName) == '') {
                        WebToast('Please enter Organization Name', 'top');
                        return;
                    }

                    if ($.trim(txtProjectNewClientOrgTIN) == '') {
                        WebToast('Please enter TIN', 'top');
                        return;
                    }
                } else if (selProjectNewClientType == 'person') {
                    if ($.trim(txtProjectNewClientPersonFname) == '') {
                        WebToast('Please enter first name', 'top');
                        return;
                    }

                    if ($.trim(txtProjectNewClientPersonMname) != '' && txtProjectNewClientPersonMname.length == 1) {
                        WebToast('Please enter complete middle name', 'top');
                        return;
                    }

                    if ($.trim(txtProjectNewClientPersonLname) == '') {
                        WebToast('Please enter last name', 'top');
                        return;
                    }
                }

                if ($.trim(txtProjectNewClientMobileNo) != '') {
                    if (!IsNumeric(txtProjectNewClientMobileNo)) {
                        WebToast('Please enter valid mobile number', 'top');
                        return;
                    }

                    if (txtProjectNewClientMobileNo.length > 10 || txtProjectNewClientMobileNo.length < 10) {
                        WebToast('Please enter 10 digit mobile number', 'top');
                        return;
                    }
                }

                if (!IsEmail($.trim(txtProjectNewClientEmail)) && $.trim(txtProjectNewClientEmail) != '') {
                    WebToast('Please enter valid email address', 'top');
                    return;
                }

                var data = {
                    ClientType: selProjectNewClientType,
                    OrgName: txtProjectNewClientOrgName,
                    TIN: txtProjectNewClientOrgTIN,
                    Firstname: txtProjectNewClientPersonFname,
                    Middlename: txtProjectNewClientPersonMname,
                    Lastname: txtProjectNewClientPersonLname,
                    HomeNo: $.trim(txtProjectNewClientHomeNo),
                    StreetName: $.trim(txtProjectNewClientStreetName),
                    City: selProjectNewClientCity,
                    Country: selProjectNewClientCountry,
                    CountryCode: selProjectNewClientCountryCode,
                    MobileNo: $.trim(txtProjectNewClientMobileNo),
                    EmailAdd: $.trim(txtProjectNewClientEmail)
                }

                AjaxSend({
                    url: 'BOMProjectExcel/SaveNewClient',
                    data: { data: data },
                    success: function (e) {
                        var res = e.Results.split('|')
                        var msg = res[0];
                        if (msg == 'saved') {
                            $('#txtClientID').val(res[1]);
                            $('#txtClientName').text($.trim(res[2]));
                            WebToast('New Client has been successfully saved', 'top');
                            $('#popupProjectNewClient').modal('hide');
                        } else {
                            NewMessageBox(msg);
                        }
                    }
                })
            })
            .on('keydown', '#txtProjectNewClientMobileNo', function (e) {
                if(!NumberOnly(e)){
                    return;
                }
            })

            function SearchProjectOwnerManagerAsst(title) {
                BrowsePopup('LibraryPeople', title, [
                    {
                        DisplayText: [{ Column: 'PeopleName' }],
                        Click: function (data) {
                            var isExist = false;
                            var position;

                            $('#lstProjectOwner li').find('a:first').each(function () {
                                if ($(this).attr('id') == data.ID) {
                                    isExist = true;
                                    position = "Project Owner";
                                }
                            })

                            $('#lstProjectManager li').find('a:first').each(function () {
                                if ($(this).attr('id') == data.ID) {
                                    isExist = true;
                                    position = "Project Manager";
                                }
                            })

                            $('#lstAsstProjectManager li').find('a:first').each(function () {
                                if ($(this).attr('id') == data.ID) {
                                    isExist = true;
                                    position = "Project Assistant Manager";
                                }
                            })

                            if (isExist) {
                                WebToast('<strong>' + data.PeopleName + '</strong> already a ' + position + '.', 'top');
                                return false;
                            }

                            if (title == "Project Owner") {
                                arrOwnerList.push({ "PeopleID": data.ID, "PeopleName": data.PeopleName, "IsProjectOwner": 1, "ProjectOwnerState": "Add" });
                                LoadOwnerList();
                            }
                            else if (title == "Project Manager") {
                                arrManagerList.push({ "PeopleID": data.ID, "PeopleName": data.PeopleName, "IsProjectManager": 1, "ProjectManagerState": "Add" });
                                LoadManagerList();
                            }
                            else if (title == "Asst. Project Manager") {
                                arrAsstManagerList.push({ "PeopleID": data.ID, "PeopleName": data.PeopleName, "IsProjectManager": 0, "ProjectAsstManagerState": "Add" });
                                LoadAsstManagerList();
                            }
                        }
                    }
                ])
            }

            function ReGenerateItemNo() {
                var ctr = 0;
                $('#tBodyExcelLike tr').each(function () {
                    var itemno = $(this).find('td.tditemno > div');
                    itemno.text(ctr += 1)
                })
            }

            function DisplayProjectFilesToUpload(files) {
                for (var i = 0; i < files.length; i++) {
                    var type = files[i].type.split('/')[0];
                    var pData = {
                        ID: NewID(),
                        filename: files[i].name,
                        filedescription: '',
                        size: files[i].size,
                        status: 'new',
                        counter: (i + 1),
                        IsImage: (type == 'image'),
                        Index: i,
                        rename: files[i].name.split('.')[0]
                    }

                    arrProjectFiles.push(pData)
                    projectFilesFormData.append(files[i].name, files[i]);

                    var reader = new FileReader();

                    reader.onload = function (e) {
                        var target = e.target;
                        var data = target.Data;
                        var row = $('<tr>')
                                .data('data', data)
                        //.append($('<td class="txt-middle">').text(data.counter))

                        if (data.IsImage) {
                            row.append($('<td class="txt-middle" style="width:100px;">').append($('<img style="width:100px;">').attr('src', target.result)))
                        } else {
                            row.append($('<td class="txt-middle">'))
                        }

                        row.append($('<td class="txt-middle">').append($('<input data-role="none" class="form-control">').data('data', data).val(data.filename.split('.')[0])
                                .blur(function () {
                                    var data = $(this).data('data');
                                    arrProjectFiles[data.Index].rename = $.trim($(this).val())
                                })
                            ))
                        row.append($('<td class="txt-middle">').append($('<textarea data-role="none" class="form-control" style="height:50px; resize:none;">')
                                    .val('')
                                    .data('data', data)
                                    .blur(function () {
                                        var data = $(this).data('data');
                                        arrProjectFiles[data.Index].filedescription = $.trim($(this).val())
                                    })
                                ))
                        row.append($('<td class="txt-middle">')
                                .append($('<button data-role="none" class="btn custom-btn-theme-d btnCircle-xs">')
                                    .data('data', data)
                                    .append($('<span class="bi bi-remove">'))
                                    .click(function (e) {
                                        var dis = $(this);
                                        var data = dis.data('data');
                                        NewMessageBox('Are you sure you want to remove file <b>' + data.filename + '</b>?', 'Message', [
                                            {
                                                Text: 'Yes', Click: function () {
                                                    var ind;
                                                    $.grep(arrProjectFiles, function (e, i) {
                                                        if(e.ID == data.ID){
                                                            ind = i;
                                                            return false;
                                                        }
                                                    })
                                                    arrProjectFiles.splice(ind, 1);
                                                    dis.parents().closest('tr').remove();
                                                }
                                            }, { Text: 'No' }
                                        ])
                                    })
                                )
                            )

                        $('#tBodyProjecAttachmentsDetails').append(row)
                    }
                    reader.Data = pData;
                    reader.readAsDataURL(files[i]);
                }
            }

            function GetProjectFiles() {
                var tBodyProjecFiles = $('#tBodyProjecFiles');
                tBodyProjecFiles.empty();

                AjaxSend({
                    url: 'BOMProjectExcel/GetProjectFiles?OrgID=' + sessionStorage.OrgID,
                    data: { ProjectID : sessionStorage.ProjectID },
                    success: function (vResult) {
                        var data = vResult.data;

                        if (data.length > 0) {
                            for (var i = 0; i < data.length; i++) {
                                var filename = data[i].filename.split('.')
                                var type = filename[filename.length - 1]
                                var preview = '';

                                if (CBool(data[i].IsImage)) {
                                    preview = $('<img style="max-width:50px;">')
                                                    .attr('src', data[i].src)
                                }

                                tBodyProjecFiles.append($('<tr>')
                                        .append($('<td>').append(preview)
                                            .css('width', '70px')
                                        )
                                        .append($('<td>')
                                            .html('<b>' + data[i].filename + '</b>')
                                            //.append('<br>' + data[i].filedescription)
                                        )
                                        .append($('<td>')
                                            .html(data[i].filedescription)
                                            //.append('<br>' + data[i].filedescription)
                                        )
                                        .append($('<td>')
                                            .css('width', '150px')
                                            .text(data[i].UploadedBy)
                                        )
                                        .append($('<td>')
                                            .css('width', '175px')
                                            .text($.format.date(new Date(data[i].DateUploaded), 'MMM dd, yyyy hh:mm a'))
                                        )
                                        .append($('<td>')
                                            .css({
                                                width: '70px',
                                                textAlign: 'center'
                                            })
                                            .append($('<button data-role="none" class="btn btn-secondary" title="Download File">')
                                                .append('<span class="bi bi-download"></span>')
                                                .data('data', data[i])
                                                .click(function (e) {
                                                    var data = $(this).data('data');

                                                    window.location = MergeUrlParameters({
                                                        OrgID: sessionStorage.OrgID,
                                                        ProjectID: sessionStorage.ProjectID,
                                                        FileName: data.filename,
                                                        DownloadFileName: data.filename
                                                    }, '/BOMProjectExcel/DownloadProjectFile');
                                                })
                                            )
                                        )
                                    )
                            }
                        } else {
                            tBodyProjecFiles.append($('<tr>')
                                    .append($('<td colspan="6" class="txt-center">').text('No file(s) available'))
                                )
                        }
                    }
                })
            }

            function placeCaretAtEnd(el) {
                el.focus();
                if (typeof window.getSelection != "undefined"
                        && typeof document.createRange != "undefined") {
                    var range = document.createRange();
                    range.selectNodeContents(el);
                    range.collapse(false);
                    var sel = window.getSelection();
                    sel.removeAllRanges();
                    sel.addRange(range);
                } else if (typeof document.body.createTextRange != "undefined") {
                    var textRange = document.body.createTextRange();
                    textRange.moveToElementText(el);
                    textRange.collapse(false);
                    textRange.select();
                }
            }

            function insertAtCursor(type, text, data) {
                var sel = window.getSelection && window.getSelection();
                if (sel) {
                    if (type == 'img') {
                        var img = document.createElement(text)
                        var br = document.createElement("br");
                        var range = sel.getRangeAt(0);
                        range.deleteContents();
                        range.insertNode(img);
                        img.setAttribute('src', data.result)
                        img.setAttribute('style', 'max-width:100%')
                        img.setAttribute('imgid', data.fileid)
                        img.setAttribute('status', data.filestatus)
                    } else {
                        var txt = document.createTextNode(text)
                        var range = sel.getRangeAt(0);
                        range.deleteContents();
                        range.insertNode(txt);
                    }
                }
            }

            function DisplayRemarksPhotos(data){
                $('#divRemarksPhotosContainer')
                    .append($('<div class="col-sm-4">')
                        .append($('<img>')
                            .css({
                                width: '100%',
                                margin: '5px'
                            })
                            .attr('src', data.src)
                        )
                        .append($('<button data-role="none" class="btn custom-btn-theme-x btnCircle-xs">')
                            .css({
                                position: 'absolute',
                                top: '-10px',
                                right: 0
                            })
                            .append($('<span class="bi bi-remove">'))
                            .data('data', data)
                            .click(function (e) {
                                var data = $(this).data('data');
                                var remarkfiles = $('td.activeCell').find('div').data('files');
                                var ind;

                                $.grep(remarkfiles, function (e, i) {
                                    if (e.filename == data.filename) {
                                        ind = i;
                                        return false;
                                    }
                                })
                                        
                                if(data.isnew){
                                    projectData.delete(data.filename);
                                    remarkfiles.splice(ind, 1);
                                } else {
                                    remarkfiles[ind].status = "remove";
                                }

                                $(this).parent().remove();
                            })
                        )
                    )                            
            }

            function IsShowPopupWhenClick() {
                return $('#chbShowPopup').is(':checked');
            }

            function FilterProjectSummary() {
                var val = $('#selFilterProjectSummary').val();

                if (val == 'overall') {
                    $('.cell-timespend, .cell-materialspend, .cell-laborspend').show();
                } else if (val == 'timespend') {
                    $('.cell-timespend').show();
                    $('.cell-materialspend, .cell-laborspend').hide();
                } else if (val == 'materialspend') {
                    $('.cell-materialspend').show();
                    $('.cell-timespend, .cell-laborspend').hide();
                } else if (val == 'laborspend') {
                    $('.cell-laborspend').show();
                    $('.cell-timespend, .cell-materialspend').hide();
                }
            }

            function ContextMenuAccLaborCost(elem, e) {
                $('#tBodyExcelLike tr td').removeClass('activeCell');
                $('#tBodyExcelLike tr').removeClass('activeRow');
                $(elem).addClass('activeCell');
                $(elem).parent().addClass('activeRow');
                $('.context-menu').remove();

                if (CheckPermissions("Add/Edit Labor Payment") == false) {
                    return;
                }

                if (!HasDescription()) {
                    return;
                }

                var data = $('td.activeCell').find('div').data('data');

                var tblBodyLaborPaymentHistory = $('#tblBodyLaborPaymentHistory');
                var hasLogs = false;

                tblBodyLaborPaymentHistory.empty();

                for (var i = 0; i < data.length; i++) {
                    if (CStr2(data[i].ID) != '') {
                        hasLogs = true;
                        tblBodyLaborPaymentHistory.prepend($('<tr>')
                            .append($('<td>').text(data[i].Amount))
                            .append($('<td>').text(data[i].RequestedByName))
                            .append($('<td>').text($.format.date(new Date(data[i].LastUpdated), 'MMM dd, yyyy hh:mm a')))
                            .append($('<td>').text(data[i].Remarks))
                            .append($('<td>').text(data[i].PaymentBalance))
                            .append($('<td>').text(data[i].State))
                        )
                    }
                }

                if (!hasLogs) {
                    tblBodyLaborPaymentHistory.append($('<tr>').append($('<td colspan="5">').text('No result(s) found')));
                }

                $('#txtLaborReqAmount').val('').focus();
                $('#txtLaborPaymentBalance').val('');
                $('#txtLaborRemarks').val('');
                $('#txtLaborReqByID').val('');
                $('#selLaborRequestedBy').val('');//.selectmenu('refresh');

                $('body').append($('<div>')
                        .append($('<ul>')
                            .append($('<a>')
                                .append($('<li>')
                                    .html('Request Payment')
                                    .click(function (e) {
                                        e.stopPropagation();
                                        $('.context-menu').remove();
                                        $('#popupProjectAccDisbursedCost').modal('show');
                                        $('#tabAddDisbursedCost').opentabmenu(function () {
                                            $('#tabAddDisbursedCost').parent().removeClass('.full-height');
                                        });

                                        //if (data.length > 0) {
                                        //    var hasNew = false;
                                        //    for (var i = 0; i < data.length; i++) {
                                        //        if (data[i].ID == '') {
                                        //            hasNew = true;
                                        //            $('#txtLaborReqAmount').val(data[i].Amount).focus();
                                        //            $('#txtLaborPaymentBalance').val(data[i].PaymentBalance);
                                        //            $('#txtLaborRemarks').val(data[i].Remarks);
                                        //            $('#txtLaborReqByID').val(data[i].RequestedByID);
                                        //            $('#selLaborRequestedBy').val(data[i].RequestedByName);//.selectmenu('refresh');
                                        //            break;
                                        //        }
                                        //    }

                                        //    if (!hasNew) {
                                        //        $('#txtLaborReqAmount').val(data[0].Amount).focus();
                                        //        $('#txtLaborPaymentBalance').val(data[0].PaymentBalance);
                                        //        $('#txtLaborRemarks').val(data[0].Remarks);
                                        //        $('#txtLaborReqByID').val(data[0].RequestedByID);
                                        //        $('#selLaborRequestedBy').val(data[0].RequestedByName);//.selectmenu('refresh');
                                        //    }
                                        //}
                                    })
                                )
                            )
                            .append($('<a>')
                                .append($('<li>')
                                    .html('Edit Request Payment')
                                    .click(function (e) {
                                        e.stopPropagation();
                                        $('.context-menu').remove();                                        

                                        if (data.length > 0) {
                                            var hasNew = false;
                                            for (var i = 0; i < data.length; i++) {
                                                if (data[i].ID == '') {
                                                    hasNew = true;
                                                    $('#popupProjectAccDisbursedCost').modal('show');
                                                    $('#tabAddDisbursedCost').opentabmenu(function () {
                                                        $('#tabAddDisbursedCost').parent().removeClass('.full-height');
                                                    });
                                                    $('#txtLaborReqAmount').val(data[i].Amount).focus();
                                                    $('#txtLaborPaymentBalance').val(data[i].PaymentBalance);
                                                    $('#txtLaborRemarks').val(data[i].Remarks);
                                                    $('#txtLaborReqByID').val(data[i].RequestedByID);
                                                    $('#selLaborRequestedBy').val(data[i].RequestedByName);//.selectmenu('refresh');
                                                    break;
                                                }
                                            }

                                            if (!hasNew) {
                                                NewMessageBox('Cannot edit Request. You can cancel last request from request history and/or create a new request', 'Message');
                                            }
                                        } else {
                                            WebToast('No request to edit', 'top');
                                        }
                                    })
                                )
                            )
                            .append($('<a>')
                                .append($('<li>')
                                    .html('Cancel Request')
                                    .click(function (e) {
                                        e.stopPropagation();
                                        $('.context-menu').remove();

                                        if (data.length > 0) {
                                            var lastindex = data.length - 1;
                                            if (data[lastindex].ID == '') {
                                                data.splice(lastindex, 1);
                                            } else {
                                                data[lastindex].State = 'cancelled';
                                            }

                                            var celltotalreq = $('#tBodyExcelLike tr td.activeCell').parent().find('td.tdaccreqqty > div');
                                            celltotalreq.text(Math.abs(CDec2(celltotalreq.text()) - CDec2(data[lastindex].Amount)));
                                            $('td.activeCell').parent().find('td.tdaccreqlaborcost > div').text('');
                                        }
                                    })
                                )
                            )
                            .append($('<a>')
                                .append($('<li>')
                                    .html('View Payment History')
                                    .click(function (e) {
                                        e.stopPropagation();
                                        $('.context-menu').remove();
                                        $('#popupProjectAccDisbursedCost').modal('show');
                                        $('#tabDisbursedHistory').opentabmenu(function () {
                                            $('#tabDisbursedHistory').parent().removeClass('.full-height');
                                        });
                                    })
                                )
                            )
                        )
                        .addClass('context-menu')
                        .css({ top: e.pageY + "px", left: e.pageX + "px" })
                    )
            }

            function ContextMenuBudgetedLaborCost(elem, e) {
                $('#tBodyExcelLike tr td').removeClass('activeCell');
                $('#tBodyExcelLike tr').removeClass('activeRow');
                $(elem).addClass('activeCell');
                $(elem).parent().addClass('activeRow');
                $('.context-menu').remove();

                if (CheckPermissions("Add/Edit Labor Costing") == false) {
                    return;
                }

                if (!HasDescription()) {
                    return;
                }

                var data = $('td.activeCell').find('div').data('data');

                var tblBodyBudgetedLaborCost = $('#tblBodyBudgetedLaborCost');
                var hasLogs = false;

                tblBodyBudgetedLaborCost.empty();

                for (var i = 0; i < data.length; i++) {
                    if (CStr2(data[i].ID) != '') {
                        hasLogs = true;
                        tblBodyBudgetedLaborCost.prepend($('<tr>')
                            .append($('<td>').text(data[i].Amount))
                            .append($('<td>').text($.format.date(new Date(data[i].LastUpdated), 'MMM dd, yyyy hh:mm a')))
                            .append($('<td>').text(data[i].ModifiedByName))
                        )
                    }
                }

                if (!hasLogs) {
                    tblBodyBudgetedLaborCost.append($('<tr>').append($('<td colspan="5">').text('No result(s) found')));
                }

                $('body').append($('<div>')
                        .append($('<ul>')
                            .append($('<a>')
                                .append($('<li>')
                                    .html('View History')
                                    .click(function (e) {
                                        e.stopPropagation();
                                        $('#popupProjectBudgetedLaborCost').modal('show');
                                        $('.context-menu').remove();
                                    })
                                )
                            )
                        )
                        .addClass('context-menu')
                        .css({ top: e.pageY + "px", left: e.pageX + "px" })
                    )
            }

            function ContextMenuAccUsedUp(elem, e) {
                $('#tBodyExcelLike tr td').removeClass('activeCell');
                $('#tBodyExcelLike tr').removeClass('activeRow');
                $(elem).addClass('activeCell');
                $(elem).parent().addClass('activeRow');
                $('.context-menu').remove();

                if (CheckPermissions("Add/Edit Material Used Up Qty") == false) {
                    return;
                }

                if ($(elem).hasClass('tableCellDisable')) {
                    return;
                }

                if (!HasDescription()) {
                    return;
                }

                var data = $('td.activeCell').find('div').data('data');

                var tblBodyAccUsedUp = $('#tblBodyAccUsedUp');
                var hasLogs = false;

                tblBodyAccUsedUp.empty();

                for (var i = 0; i < data.length; i++) {
                    if (CStr2(data[i].ID) != '') {
                        hasLogs = true;
                        tblBodyAccUsedUp.prepend($('<tr>')
                            .append($('<td>').text(data[i].Qty))
                            .append($('<td>').text($.format.date(new Date(data[i].LastUpdated), 'MMM dd, yyyy hh:mm a')))
                            .append($('<td>').text(data[i].ModifiedByName))
                            .append($('<td>').text(data[i].Remarks))
                        )
                    }
                }

                if (!hasLogs) {
                    tblBodyAccUsedUp.append($('<tr>').append($('<td colspan="5">').text('No result(s) found')));
                }

                $('#txtAccUsedUpQty').val('').focus();
                $('#txtAccUsedUpRemarks').val('');

                $('body').append($('<div>')
                        .append($('<ul>')
                            .append($('<a>')
                                .append($('<li>')
                                    .html('Declare Used Up Qty')
                                    .click(function (e) {
                                        e.stopPropagation();
                                        $('.context-menu').remove();
                                        $('#popupProjectMaterialAccUsedUpQty').modal('show');
                                        $('#tabAddMaterialAccUsedUpQty').opentabmenu(function () {
                                            $('#tabAddMaterialAccUsedUpQty').parent().removeClass('.full-height');
                                        });
                                    })
                                )
                            )
                            .append($('<a>')
                                .append($('<li>')
                                    .html('Edit Declared Used Up Qty')
                                    .click(function (e) {
                                        e.stopPropagation();
                                        $('.context-menu').remove();
                                        $('#popupProjectMaterialAccUsedUpQty').modal('show');
                                        $('#tabAddMaterialAccUsedUpQty').opentabmenu(function () {
                                            $('#tabAddMaterialAccUsedUpQty').parent().removeClass('.full-height');
                                        });

                                        if (data.length > 0) {
                                            var hasNew = false;
                                            for (var i = 0; i < data.length; i++) {
                                                if (data[i].ID == '') {
                                                    hasNew = true;
                                                    $('#txtAccUsedUpQty').val(data[i].Qty).focus();
                                                    $('#txtAccUsedUpRemarks').val(data[i].Remarks);
                                                    break;
                                                }
                                            }

                                            if (!hasNew) {
                                                NewMessageBox('Cannot edit last declared qty.', 'Message');
                                            }
                                        } else {
                                            WebToast('No data to edit', 'top');
                                        }
                                    })
                                )
                            )
                            .append($('<a>')
                                .append($('<li>')
                                    .html('View Usage History')
                                    .click(function (e) {
                                        e.stopPropagation();
                                        $('.context-menu').remove();
                                        $('#popupProjectMaterialAccUsedUpQty').modal('show');
                                        $('#tabMaterialAccUsedUpHistory').opentabmenu(function () {
                                            $('#tabMaterialAccUsedUpHistory').parent().addClass('.full-height');
                                        });
                                    })
                                )
                            )
                        )
                        .addClass('context-menu')
                        .css({ top: e.pageY + "px", left: e.pageX + "px" })
                    )
            }

            function ContextMenuAccReqQty(elem, e) {
                $('#tBodyExcelLike tr td').removeClass('activeCell');
                $('#tBodyExcelLike tr').removeClass('activeRow');
                $(elem).addClass('activeCell');
                $(elem).parent().addClass('activeRow');
                $('.context-menu').remove();

                if (CheckPermissions("Add/Edit Material Requested Qty") == false) {
                    return;
                }

                if ($(elem).hasClass('tableCellDisable')) {
                    return;
                }

                if (!HasDescription()) {
                    return;
                }

                var data = $('td.activeCell').find('div').data('data');

                var tblBodyAccReqHistory = $('#tblBodyAccReqHistory');
                var hasLogs = false;

                tblBodyAccReqHistory.empty()

                for (var i = 0; i < data.length; i++) {
                    if (CStr2(data[i].ID) != '') {
                        hasLogs = true;
                        tblBodyAccReqHistory.prepend($('<tr>')
                            .append($('<td>').text($.format.date(new Date(data[i].LastUpdated), 'MMM dd, yyyy hh:mm a')))
                            .append($('<td>').text(data[i].RequestedByName))
                            .append($('<td>').text(data[i].Qty))
                            .append($('<td>').text(CStr2(data[i].DateNeeded)))
                            .append($('<td>').text(CStr2(data[i].TimeNeededFrom) + ' ' + CStr2(data[i].TimeNeededTo)))
                            .append($('<td>').text(data[i].Remarks))
                            .append($('<td>').text(data[i].State))
                        )
                    }
                }

                if (!hasLogs) {
                    tblBodyAccReqHistory.append($('<tr>').append($('<td colspan="5">').text('No result(s) found')));
                }

                $('#txtAccReqQty').val('').focus();
                $('#txtReqByID, #txtAccReqDateNeeded, .txtAccReqTime').val('');
                $('#selAccReqBy').val('');//.selectmenu('refresh');
                $('#txtAccReqRemarks').val('');

                $('body').append($('<div>')
                        .append($('<ul>')
                            .append($('<a>')
                                .append($('<li>')
                                    .html('Request Material')
                                    .click(function (e) {
                                        e.stopPropagation();
                                        $('.context-menu').remove();
                                        if (!IsMaterialHasSku()) {
                                            NewMessageBox('Please link material to SKU first', 'Message');
                                            return;
                                        }
                                        $('#popupProjectMaterialAccReqQty').modal('show');
                                        $('#tabAddMaterialAccReqQty').opentabmenu(function () {
                                            $('#tabAddMaterialAccReqQty').parent().removeClass('.full-height');
                                            $('#txtAccReqQty').focus();
                                        });
                                    })
                                )
                            )
                            .append($('<a>')
                                .append($('<li>')
                                    .html('Edit Material')
                                    .click(function (e) {
                                        e.stopPropagation();
                                        $('.context-menu').remove();
                                        if (!IsMaterialHasSku()) {
                                            NewMessageBox('Please link material to SKU first', 'Message');
                                            return;
                                        }
                                        $('#tabAddMaterialAccReqQty').opentabmenu(function () {
                                            $('#tabAddMaterialAccReqQty').parent().removeClass('.full-height');
                                            $('#txtAccReqQty').focus();
                                        });
                                        var lastindex = data.length - 1;
                                        if (data.length > 0) {
                                            var hasNew = false;
                                            for (var i = 0; i < data.length; i++) {
                                                if (data[i].ID == '') {
                                                    hasNew = true;
                                                    $('#popupProjectMaterialAccReqQty').modal('show');
                                                    $('#txtAccReqQty').val(data[i].Qty).focus();
                                                    //$('#selAccReqUom').val(data[i].UomID);//.selectmenu('refresh');
                                                    $('#txtAccReqRemarks').val(data[i].Remarks);
                                                    $('#txtReqByID').val(data[i].RequestedByID);
                                                    $('#selAccReqBy').val(data[i].RequestedByName);//.selectmenu('refresh');
                                                    $('#txtAccReqDateNeeded').val(data[i].DateNeeded);
                                                    $('#txtAccReqTimeFrom').val(data[i].TimeNeededFrom);
                                                    $('#txtAccReqTimeTo').val(data[i].TimeNeededTo);
                                                    break;
                                                }
                                            }

                                            if (!hasNew) {
                                                NewMessageBox('Cannot edit Request. You can cancel last request from request history and/or create a new request', 'Message');
                                            }
                                        } else {
                                            WebToast('No request to edit', 'top');
                                        }
                                    })
                                )
                            )
                            .append($('<a>')
                                .append($('<li>')
                                    .html('Cancel Request')
                                    .click(function (e) {
                                        e.stopPropagation();
                                        $('.context-menu').remove();
                                        if (!IsMaterialHasSku()) {
                                            NewMessageBox('Please link material to SKU first', 'Message');
                                            return;
                                        }
                                        if(data.length > 0){
                                            var lastindex = data.length - 1;
                                            if (data[lastindex].ID == '') {
                                                data.splice(lastindex, 1);
                                            } else {
                                                data[lastindex].State = 'cancelled';
                                            }

                                            var celltotalreq = $('#tBodyExcelLike tr td.activeCell').parent().find('td.tdaccreqqty > div');
                                            celltotalreq.text(Math.abs(CDec2(celltotalreq.text()) - CDec2(data[lastindex].Qty)));
                                            $('td.activeCell').parent().find('td.tdreqqty > div').text('');
                                        }
                                    })
                                )
                            )
                            .append($('<a>')
                                .append($('<li>')
                                    .html('View Request History')
                                    .click(function (e) {
                                        e.stopPropagation();
                                        $('.context-menu').remove();
                                        $('#popupProjectMaterialAccReqQty').modal('show');
                                        $('#tabMaterialAccReqQty').opentabmenu(function () {
                                            $('#tabMaterialAccReqQty').parent().addClass('.full-height');
                                        });
                                    })
                                )
                            )
                        )
                        .addClass('context-menu')
                        .css({ top: e.pageY + "px", left: e.pageX + "px" })
                    )
            }

            function ContextMenuBudgetedQty(elem, e) {
                $('#tBodyExcelLike tr td').removeClass('activeCell');
                $('#tBodyExcelLike tr').removeClass('activeRow');
                $(elem).addClass('activeCell');
                $(elem).parent().addClass('activeRow');
                $('.context-menu').remove();

                if (CheckPermissions("Add/Edit Material Budgeted Qty") == false) {
                    return;
                }

                if ($(elem).hasClass('tableCellDisable')) {
                    return;
                }

                if (!HasDescription()) {
                    return;
                }

                var data = $('td.activeCell').find('div').data('data');

                var tblBodyMaterialBudgetAuditLog = $('#tblBodyMaterialBudgetAuditLog');

                tblBodyMaterialBudgetAuditLog.empty();
                var hasLogs = false;
                for (var i = 0; i < data.length; i++) {
                    if (CStr2(data[i].ID) != '') {
                        hasLogs = true;
                        tblBodyMaterialBudgetAuditLog.prepend($('<tr>')
                            .append($('<td>').text(data[i].Qty))
                            //.append($('<td>').text(data[i].UomName))
                            .append($('<td>').text($.format.date(new Date(data[i].LastUpdated), 'MMM dd, yyyy hh:mm a')))
                            .append($('<td>').text(data[i].ModifiedByName))
                        )
                    }
                }

                if (!hasLogs) {
                    tblBodyMaterialBudgetAuditLog.append($('<tr>').append($('<td colspan="5">').text('No result(s) found')));
                }

                $('#txtMaterialBudgetQty').val('').focus();
                //$('#selMaterialBudgetUom').val('');//.selectmenu('refresh');

                $('body').append($('<div>')
                        .append($('<ul>')
                            //.append($('<a>')
                            //    .append($('<li>')
                            //        .html('Encode / Edit Budget')
                            //        .click(function (e) {
                            //            e.stopPropagation();
                            //            $('.context-menu').remove();
                            //            $('#popupProjectMaterialBudgetQty').modal('show');
                            //            $('#tabAddMaterialBudgetQty').opentabmenu(function () {
                            //                $('#tabAddMaterialBudgetQty').parent().removeClass('.full-height');
                            //            });

                            //            if (data.length > 0) {
                            //                var hasNew = false;
                            //                for (var i = 0; i < data.length; i++) {
                            //                    if (data[i].ID == '') {
                            //                        hasNew = true;
                            //                        $('#txtMaterialBudgetQty').val(data[i].Qty).focus();
                            //                        //$('#selMaterialBudgetUom').val(data[i].UomID);//.selectmenu('refresh');
                            //                        break;
                            //                    }
                            //                }

                            //                if(!hasNew){
                            //                    $('#txtMaterialBudgetQty').val('').focus();
                            //                    //$('#selMaterialBudgetUom').val('');//.selectmenu('refresh');
                            //                }
                            //            }
                            //        })
                            //    )
                            //)
                            .append($('<a>')
                                .append($('<li>')
                                    .html('View History')
                                    .click(function (e) {
                                        e.stopPropagation();
                                        $('.context-menu').remove();
                                        $('#popupProjectMaterialBudgetQty').modal('show');
                                        $('#tabMaterialAuditLog').opentabmenu(function () {
                                            $('#tabMaterialAuditLog').parent().addClass('.full-height');
                                        });
                                    })
                                )
                            )
                        )
                        .addClass('context-menu')
                        .css({ top: e.pageY + "px", left: e.pageX + "px" })
                    )
            }

            function ContextMenuRemarks(elem, e) {
                $('#tBodyExcelLike tr td').removeClass('activeCell');
                $('#tBodyExcelLike tr').removeClass('activeRow');
                $(elem).addClass('activeCell');
                $(elem).parent().addClass('activeRow');
                $('.context-menu').remove();
                $('#txtProjectRemarks').html('');

                var data = $('td.activeCell').find('div').data('data');
                var tblBodyProjectRemarks = $('#tblBodyProjectRemarks');
                var hasLogs = false;

                tblBodyProjectRemarks.empty();

                if (CheckPermissions("Add/Edit Remarks") == false) {
                    return;
                }

                if (!HasDescription()) {
                    return;
                }

                var ul = $('<ul>')
                            .append($('<a>')
                                .append($('<li>')
                                    .html('Add Remarks')
                                    .click(function (e) {
                                        e.stopPropagation();
                                        $('.context-menu').remove();
                                        $('#popupProjectRemarks').modal('show');
                                        $('#lblRemarksLineDescription').text(currentLineDescription);
                                        $('#tabAddProjectRemarks').opentabmenu(function () {
                                            $('#tabAddProjectRemarks').parent().removeClass('.full-height');
                                            $('#txtProjectRemarks').focus();
                                        });
                                        GetRemarksPhotos();
                                    })
                                )
                            )
                                    
                if(data.length > 0){
                    for (var i = 0; i < data.length; i++) {
                        if (CStr2(data[i].ID) != '') {
                            hasLogs = true;
                            tblBodyProjectRemarks.prepend($('<tr>')
                                .data('data', data[i])
                                .data('photos', [])
                                .append($('<td>').html(HtmlToPlainText(HtmlDecode(data[i].Remarks)).substring(0, 50)))
                                .append($('<td>').text($.format.date(new Date(data[i].LastUpdated), 'MMM dd, yyyy hh:mm a')))
                                .append($('<td>').text(data[i].ModifiedByName))
                                .click(function () {
                                    var data = $(this).data('data');
                                    $('#lblRemarksInfo').text('By ' + data.ModifiedByName + ' on ' + $.format.date(new Date(data.LastUpdated), 'MMM dd, yyyy hh:mm a'))
                                    $('#divRemarksDetails').html(HtmlDecode(data.Remarks));
                                    $('#popupProjectRemarksDetails').modal('show');
                                    GetRemarksPhotos($(this));
                                })
                            )
                        }                                
                    }

                    if(!hasLogs){
                        tblBodyProjectRemarks.append($('<tr>').append($('<td colspan="5">').text('No result(s) found')));
                    }

                    if (data[data.length - 1].ID == '') {
                        $('#txtProjectRemarks').html(HtmlDecode(data[data.length - 1].Remarks)).focus();
                    }
                }

                ul.append($('<a>')
                            .append($('<li>')
                                .html('View History')
                                .click(function (e) {
                                    e.stopPropagation();
                                    $('.context-menu').remove();
                                    $('#popupProjectRemarks').modal('show');
                                    $('#lblRemarksLineDescription').text(currentLineDescription);
                                    $('#tabProjectRemarksHistory').opentabmenu();
                                    GetRemarksPhotos();
                                })
                            )
                        )

                $('body').append($('<div>')
                        .append(ul)
                        .addClass('context-menu')
                        .css({ top: e.pageY + "px", left: e.pageX + "px" })
                    )
            }

            function GetRemarksPhotos(dis) {
                var descid = $('td.activeCell').parent().attr('descriptionrowid');
                var tdremarks = $('[descriptionrowid="' + descid + '"]').find('.tdremarks');
                var tdfiles = tdremarks.find('div').data('files');
                var newphotos = $('td.activeCell').find('div').data('newphotosformdata');
                $('#divRemarksPhotosContainer').empty();

                for (var i = 0; i < newphotos.length; i++) {
                    var reader = new FileReader();

                    reader.onload = function (e) {
                        for (var i = 0; i < tdfiles.length; i++){
                            if (tdfiles[i].filename == e.target.filename) {
                                $('img[imgid="' + tdfiles[i].ID + '"]').attr('src', e.target.result);
                                break;
                            }
                        }
                        //DisplayRemarksPhotos(e.target.result);
                    }
                    reader.filename = newphotos[i].name;
                    reader.readAsDataURL(newphotos[i]);
                }

                if (CStr2(dis) == '') {
                    return;
                } else {
                    if (CBool(dis.attr('isloadphotos'))) {
                        var remarksphotos = dis.data('photos');
                        for (var i = 0; i < remarksphotos.length; i++) {
                            $('img[imgid="' + remarksphotos[i].ID + '"]').attr('src', remarksphotos[i].src);
                            //DisplayRemarksPhotos({
                            //    ID: tdfiles[i].ID,
                            //    filename: tdfiles[i].filename,
                            //    src: tdfiles[i].src, //'data:image/' + (tdfiles[i].filename).split('.')[1] + ';base64,' + tdfiles[i].src,
                            //    isnew: tdfiles[i].isnew
                            //});
                        }
                    } else {
                        AjaxSend({
                            url: 'BOMProjectExcel/GetRemarksPhotos',
                            data: { ProjectBudgetID: descid, RemarksID: dis.data('data').ID },
                            success: function (e) {                                        
                                var vRows = e.data;

                                for (var i = 0; i < vRows.length; i++) {
                                    dis.data('photos').push(vRows[i]);
                                    $('img[imgid="' + vRows[i].ID + '"]').attr('src', vRows[i].src);
                                    //DisplayRemarksPhotos({
                                    //    ID: vRows[i].ID,
                                    //    filename: vRows[i].filename,
                                    //    src: vRows[i].src, //'data:image/' + (vRows[i].filename).split('.')[1] + ';base64,' + vRows[i].src,
                                    //    isnew: vRows[i].isnew
                                    //});
                                }

                                dis.attr('isloadphotos', true)
                            }
                        })
                    }                            
                }
            }

            function ContextMenuBudgetedDuration(elem, e) {
                $('#tBodyExcelLike tr td').removeClass('activeCell');
                $('#tBodyExcelLike tr').removeClass('activeRow');
                $(elem).addClass('activeCell');
                $(elem).parent().addClass('activeRow');
                $('.context-menu').remove();

                if (CheckPermissions("Set/Change Task Duration") == false) {
                    return;
                }

                if (!HasDescription()) {
                    return;
                }

                if(IsShowPopupWhenClick()){
                    return;
                }

                var data = $('td.activeCell').find('div').data('data');

                $('body').append($('<div>')
                        .append($('<ul>')
                            .append($('<a>')
                                .append($('<li>')
                                    .html('Set Duration')
                                    .click(function (e) {
                                        e.stopPropagation();
                                        $('.context-menu').remove();
                                        ProjectDuration($(elem));
                                    })
                                )
                            )
                        )
                        .addClass('context-menu')
                        .css({ top: e.pageY + "px", left: e.pageX + "px" })
                    )
            }

            function ContextMenuSetDate(elem, e) {
                $('#tBodyExcelLike tr td').removeClass('activeCell');
                $('#tBodyExcelLike tr').removeClass('activeRow');
                $(elem).addClass('activeCell');
                $(elem).parent().addClass('activeRow');
                $('.context-menu').remove();

                if (CheckPermissions("Set/Change Task Date Started/Completed") == false) {
                    return;
                }

                if (!HasDescription()) {
                    return;
                }

                if (IsShowPopupWhenClick()) {
                    return;
                }

                var data = $('td.activeCell').find('div').data('data');

                $('body').append($('<div>')
                        .append($('<ul>')
                            .append($('<a>')
                                .append($('<li>')
                                    .html('Set Date')
                                    .click(function (e) {
                                        e.stopPropagation();
                                        $('.context-menu').remove();
                                        ProjectDatePicker($(elem));
                                    })
                                )
                            )
                        )
                        .addClass('context-menu')
                        .css({ top: e.pageY + "px", left: e.pageX + "px" })
                    )
            }

            function ContextMenuTaskDependency(elem, e) {
                $('#tBodyExcelLike tr td').removeClass('activeCell');
                $('#tBodyExcelLike tr').removeClass('activeRow');
                $(elem).addClass('activeCell');
                $(elem).parent().addClass('activeRow');
                $('.context-menu').remove();

                if (CheckPermissions("Set/Change Task Dependency") == false) {
                    return;
                }

                if (!HasDescription()) {
                    return;
                }

                if (IsShowPopupWhenClick()) {
                    return;
                }

                var data = $('td.activeCell').find('div').data('data');

                $('body').append($('<div>')
                        .append($('<ul>')
                            .append($('<a>')
                                .append($('<li>')
                                    .data('data', data)
                                    .html('Set Task Dependency')
                                    .click(function (e) {
                                        e.stopPropagation();                                                
                                        ModifyTaskDependency($(this).data('data'));
                                        $('.context-menu').remove();
                                    })
                                )
                            )
                        )
                        .addClass('context-menu')
                        .css({ top: e.pageY + "px", left: e.pageX + "px" })
                    )
            }

            function ModifyTaskDependency(data) {
                if (CheckPermissions("Set/Change Task Dependency") == false) {
                    return;
                }

                $('.listProjectDependency').show();
                $('.chbProjectDependency').removeAttr('checked');//.checkboxradio('refresh');
                        
                var ins = CStr2(data.Dependency).split(',');
                var itemno = $('td.activeCell').parent().find('td.tditemno > div').text();
                var groupnumber = itemno.substring(0, itemno.lastIndexOf('.') + 1).slice(0, -1);

                $('li[groupnumber="' + (groupnumber == '' ? itemno : groupnumber) + '"]').hide();

                for (var i = 0; i < ins.length; i++) {                    
                    $('[name="chbProjectDependency"][value="' + $.trim(ins[i]) + '"]').attr('checked', true);
                }

                $('.chbProjectDependency');//.checkboxradio('refresh');

                $('#popupProjectTaskDependency').modal('show');
            }

            function HasDescription() {
                currentLineDescription = $.trim($('td.activeCell').parent().find('td.tddescription').find('div').text())
                if (currentLineDescription == '') {
                    WebToast('Please enter line description.', 'top');
                    return false;
                } else {
                    return true;
                }
            }

            function IsMaterialHasSku() {
                return $('.activeRow').find('.tddescription').hasClass('hasAssociatedSKU');
            }

            function CheckPermissions(desc) {
                var isAllowed = true;

                if ($.inArray(desc, arrAccessRights) > -1) {
                    isAllowed = true;
                }
                else {
                    WebToast('You don\'t have permission to <b>' + desc + '</b>', 'top');
                    isAllowed = false;
                }

                return CBool(isAllowed);
            }

            function GetAllowedPermissionsPerJobRole() {
                AjaxSend({
                    url: 'BOMProjectExcel/GetAllowedPermissionsPerJobRole',
                    data: { ProjectID: sessionStorage.ProjectID },
                    success: function (vData) {
                        arrRolePermissions = vData.Results.Rows;
                        if (vData.Results.Rows.length > 0) {
                            for (var i = 0; i < vData.Results.Rows.length; i++) {
                                $('#chbPermission_' + vData.Results.Rows[i].PermissionID + '_' + vData.Results.Rows[i].RoleID).prop('checked', true)
                                    .attr('dbid', vData.Results.Rows[i].ID);//.checkboxradio('refresh');
                            }
                        }
                    }
                })
            }

            function TempSaveRolePermission() {
                arrRolePermissions = [];

                $('#tbodyDefineRole tr td:not(":eq(0)")').each(function () {
                    var chb = $(this).find('input[type="checkbox"]');
                    if (chb.attr('value') != undefined || chb.attr('perid') != undefined) {
                        arrRolePermissions.push({
                            'RoleID': chb.attr('value'),
                            'PermissionID': chb.attr('perid'),
                            'Description': chb.attr('label'),
                            'DbID': (chb.attr('dbid') == undefined ? '' : chb.attr('dbid')),
                            'IsChecked': CBool(chb.is(':checked'))
                        })
                    }
                })
            }

            function DefineUserPermissions() {
                var thead = $('#theadDefineRole');
                var tbody = $('#tbodyDefineRole');
                var tbodyContent = '';

                thead.empty();

                thead.append($('<th>').css({
                    'minWidth': '220px',
                    'maxWidth': '220px'
                })
                            );
                for (var i = 0; i < arrDefineRole.length; i++) {
                    if (arrDefineRole[i].State == undefined || arrDefineRole[i].State == 'add' || arrDefineRole[i].State == 'edit') {
                        if (thead.find('th[roleid="' + arrDefineRole[i].RoleID + '"]').length == 0) {
                            thead.append($('<th>')
                                            .css({
                                                'minWidth': '150px',
                                                'maxWidth': '150px',
                                                'textAlign': 'center'
                                            })
                                            .attr('roleid', arrDefineRole[i].RoleID)
                                            .append($('<div class="form-check">')
                                                .append($('<label class="form-check-label" for="chbJobRoleHeader_' + arrDefineRole[i].RoleID + '"><input type="checkbox" data-role="none" class="form-check-input chbJobRoleHeader" id="chbJobRoleHeader_' + arrDefineRole[i].RoleID + '" value="' + arrDefineRole[i].RoleID + '"></label>')
                                                    .append(arrDefineRole[i].Label)
                                                )
                                            )
                                        )
                        }
                    }
                    else if (arrDefineRole[i].State == 'delete') {
                        thead.find('th[roleid="' + arrDefineRole[i].RoleID + '"]').remove();
                    }
                }

                for (var m = 0; m < arrPermissions.length; m++) {
                    var column = '<td>' + arrPermissions[m].Description + '</td>';

                    for (var n = 0; n < arrDefineRole.length; n++) {
                        if (arrDefineRole[n].State == undefined || arrDefineRole[n].State == 'add' || arrDefineRole[n].State == 'edit') {
                            var checkbox = '<label class="form-check-label" for="chbPermission_' + arrPermissions[m].ID + '_' + arrDefineRole[n].RoleID + '"><input type="checkbox" data-role="none" label="' + arrDefineRole[n].Label + '" class="chbPermission form-check-input" id="chbPermission_' + arrPermissions[m].ID + '_' + arrDefineRole[n].RoleID + '" value="' + arrDefineRole[n].RoleID + '" perid="' + arrPermissions[m].ID + '"></label>';
                            if (CBool($('#chbPermission_' + arrPermissions[m].ID + '_' + arrDefineRole[n].RoleID + '').is(':checked'))) {
                                checkbox = '<label class="form-check-label" for="chbPermission_' + arrPermissions[m].ID + '_' + arrDefineRole[n].RoleID + '"><input type="checkbox" data-role="none" label="' + arrDefineRole[n].Label + '" checked="checked" class="chbPermission form-check-input" id="chbPermission_' + arrPermissions[m].ID + '_' + arrDefineRole[n].RoleID + '" value="' + arrDefineRole[n].RoleID + '" perid="' + arrPermissions[m].ID + '"></label>';
                            }
                            column += '<td style="text-align:center;" id="' + arrPermissions[m].ID + '_' + arrDefineRole[n].RoleID + '">' + checkbox + '</td>';
                        }
                        else if (arrDefineRole[n].State == 'delete') {
                            tbody.find('tr td[id="' + arrPermissions[m].ID + '_' + arrDefineRole[n].RoleID + '"]').remove();
                        }
                    }

                    tbodyContent += '<tr perid="' + arrPermissions[m].ID + '">' + column + '</tr>';
                }

                tbody.empty();
                tbody.append(tbodyContent)
                //$('input[type="checkbox"]');//.checkboxradio();//.checkboxradio('refresh');
            }

            function AddProjectMember() {
                var pTBody = $('#tbodyProjectMember');
                var selectedJobRole = GetSelectedJobRole();
                pTBody.empty();


                for (var i = 0; i < arrProjectMember.length; i++) {
                    if (arrProjectMember[i].State != "delete") {
                        pTBody.append($('<tr>')
                                .attr('state', arrProjectMember[i].State)
                                .attr('id', (arrProjectMember[i].dbID == undefined ? '' : arrProjectMember[i].dbID))
                                .attr('pplid', arrProjectMember[i].ID)
                                .append($('<td>')
                                    .text(arrProjectMember[i].PeopleName)
                                )
                                .append($('<td>')
                                    .append('<select data-role="none" class="select custom-theme-c selMemberJobRole form-select" id="jobrole_' + arrProjectMember[i].ID + '">' + selectedJobRole + '</select>')
                                )
                                .append($('<td>')
                                    .append('<select data-role="none" class="selPurchaseOrderSlider form-select" id="isactive_' + arrProjectMember[i].ID + '"><option value="0">Inactive</option><option value="1" selected>Active</option></select>')
                                )
                                .append($('<td>')
                                    .append($('<button class="btn btn-danger btnRemoveProjMember" data-role="none" title="Remove">')
                                        .append('<span class="bi bi-trash"></span>')
                                    )
                                    .data('data', arrProjectMember[i])
                                    .click(function (e) {
                                        e.stopPropagation();
                                        var data = $(this).data('data');
                                        var pplid = data.ID;
                                        var ind = $.map(arrProjectMember, function (obj, index) {
                                            if (obj.ID == pplid) {
                                                return index;
                                            }
                                        })

                                        NewMessageBox('Are you sure you want to remove <b>' + data.PeopleName + '</b> as member of this project?', 'Message', [
                                            {
                                                Text: 'Yes', Click: function () {
                                                    $('tr[pplid="' + data.ID + '"]').attr('state', 'delete').hide();
                                                    arrProjectMember[ind].State = "delete";
                                                }
                                            }, { Text: 'No' }
                                        ])                                                
                                    })
                                )
                            )
                        if (arrProjectMember[i].RoleID != undefined && arrProjectMember[i].IsActive != undefined) {
                            $('#jobrole_' + arrProjectMember[i].ID).val(arrProjectMember[i].RoleID);
                            $('#isactive_' + arrProjectMember[i].ID).val(arrProjectMember[i].IsActive);
                        }
                    }
                }
                //$('.btnRemoveProjMember').trigger('create');//.button();
                //$('.selPurchaseOrderSlider').slider();
                //pTBody.parent().table('refresh');
            }

            function GetSelectedJobRole() {
                var pOption = '<option value="">Please Select</option>';
                for (var i = 0; i < arrDefineRole.length; i++) {
                    if (arrDefineRole[i].State == undefined || arrDefineRole[i].State == 'add' || arrDefineRole[i].State == 'edit') {
                        pOption += '<option value="' + arrDefineRole[i].RoleID + '">' + arrDefineRole[i].Label + '</option>';
                    }
                }

                return pOption;
            }

            function GetJobRole() {
                AjaxSend({
                    url: 'BOMProjectExcel/GetJobRole',
                    data: { ProjectID: sessionStorage.ProjectID },
                    success: function (vData) {
                        for (var i = 0; i < vData.Results.Rows.length; i++) {
                            arrDefineRole.push({
                                ID: vData.Results.Rows[i].ID,
                                Label: vData.Results.Rows[i].Label,
                                LastUpdated: vData.Results.Rows[i].LastUpdated,
                                RoleID: vData.Results.Rows[i].RoleID,
                                UserID: vData.Results.Rows[i].UserID,
                                State: 'edit'
                            })
                        }

                        AddProjectMember();
                    }
                })
            }

            function SetColorMinPrice() {
                var tBody;
                if (isCanvassMaterials) {
                    tBody = $('#tBodyCanvassMaterial tr');
                }
                else {
                    tBody = $('#tBodyCanvassLabor tr');
                }

                tBody.each(function () {
                    var vals = $('td:not(:eq(0), :eq(1), :eq(2), :eq(3))', this).map(function () {
                        return parseInt($(this).text(), 10) ? parseInt($(this).text(), 10) : 0;
                    }).get();
                    // then find their minimum
                    var min = Math.min.apply(Math, vals);

                    // tag any cell matching the min value
                    $('td:not(:eq(0), :eq(1), :eq(2), :eq(3))', this).filter(function () {
                        if ($(this).hasClass('preferredVendor')) {
                            return;
                        }
                        return parseInt($(this).text(), 10) === min;
                    }).addClass('minPrice')
                });
            }

            function remove_duplicates(objectsArray) {
                var arr = [];

                $.each(objectsArray, function (index, value) {
                    if ($.inArray(value.ID, arr) == -1) {
                        arr.push(value.ID);
                    }
                });

                return arr;
            }

            function isNumberKey(evt) {
                var charCode = (evt.which) ? evt.which : event.keyCode
                if (charCode > 31 && (charCode < 48 || charCode > 57) && charCode != 46) {
                    return false;
                } else {
                    var len = $('input[datatype="numeric"]').val().length;
                    var index = $('input[datatype="numeric"]').val().indexOf('.');

                    if (index > 0 && charCode == 46) {
                        return false;
                    }

                }
                return true;
            }

            function PrintCanvassForm(VendorData) {
                var arrSKU = [];
                $('#txtPrintCanvassProjectName').text(currentProjectName);
                $('#txtPrintCanvassProjectAddress').text(currentProjectAddress);
                $('#txtPrintCanvassVendorName').text(VendorData.Name);
                $('#tBodyPrintCanvass').empty();
                $("#tBodyExcelLike tr").not('tr:last').each(function () {
                    if ($(this).hasClass('rowSubtotal') != true) {
                        var tds = $(this).find('td'),
                        row = $("<tr>").addClass('tRowStyle').attr('id', $(this).attr('id'));
                        row
                            .append(tds.eq(1).clone())
                            .append(tds.eq(2).clone())
                            .append(tds.eq(3).clone())
                            .append(tds.eq(4).clone())
                            .appendTo($('#tBodyPrintCanvass'));
                    }
                });
                $('#tBodyPrintCanvass tr').each(function () {
                    var isFoundSKU = false, currentSKUID = $(this).find('td:eq(1)').find('div').attr('associateskuid'), currentRowIndex = $(this).index();
                    $(this).append($('<td>')
                                .append($('<div>').css({ borderBottom: '1px solid #000', width: '100px' }).html('&nbsp;'))
                            );
                    if ($(this).find('td:eq(1)').find('div').attr('associateskuid') != undefined) {
                        $.grep(arrSKU, function (val) {
                            if (val.SKUID == currentSKUID) {
                                isFoundSKU = true;
                            }
                            else {
                                isFoundSKU = false;
                            }
                        })

                        if (isFoundSKU) {
                            arrSKU.push({ SKUID: currentSKUID, RowNumber: currentRowIndex, State: "Repeating" })
                        } else {
                            arrSKU.push({ SKUID: currentSKUID, RowNumber: currentRowIndex, State: "NonRepeating" })
                        }
                    }
                })
                $('#tBodyPrintCanvass tr td .preAssociatedSKU').show();
                SetValueOnRepeatingSKU(arrSKU);
            }

            function SetValueOnRepeatingSKU(arr) {
                for (var i = 0; i < arr.length; i++) {
                    if (arr[i].State == "Repeating") {
                        $('#tBodyPrintCanvass tr:eq(' + arr[i].RowNumber + ') td:eq(4)').find('div').css({ textAlign: 'center' }).text('x')
                    }
                }
                $('#tBodyPrintCanvass tr td').removeClass('hasAssociatedSKU').removeAttr('skuname');
                PrintNowCanvassForm($('#divPrintCanvass').html());
            }

            function PrintNowCanvassForm(data) {
                var PrintWindow = window.open('about:blank', 'Canvass From');
                PrintWindow.document.write('<html><head><title></title>');
                PrintWindow.document.write('<link rel="stylesheet" href="/Content/flatui-css-custom.css" type="text/css" />');
                PrintWindow.document.write('</head><body style="-webkit-print-color-adjust:exact;">');
                PrintWindow.document.write(data);
                PrintWindow.document.write('</body></html>');

                PrintWindow.document.close(); // necessary for IE >= 10
                PrintWindow.focus(); // necessary for IE >= 10

                setTimeout(function () {
                    PrintWindow.print();
                    PrintWindow.close();
                }, 100)


                return true;
            }

            function AppendNewAddedRowToCanvassForm(tbody) {
                var countRowtbodyExcel = $('#tBodyExcelLike tr:not(":last")').length;
                var countRowtbodyExcelTemp = $('#' + tbody + ' tr').length;
                var currentSameIndex, td1, td2, td3, td4;

                for (var i = 0; i < countRowtbodyExcel; i++) {
                    if (countRowtbodyExcelTemp == 0) {
                        $('#' + tbody + '').append($('<tr>').append($('#tBodyExcelLike tr:eq(' + i + ')').find('td:eq(1)').clone())
                            )
                    }
                    else {
                        if ($('#tBodyExcelLike tr:eq(' + i + ')').find('td:eq(1)').text() == $('#' + tbody + ' tr:eq(' + i + ')').find('td:eq(0)').text()) {
                            currentSameIndex = $('#' + tbody + ' tr:eq(' + i + ')').index();
                            td1 = $('#tBodyExcelLike tr:eq(' + i + ')').find('td:eq(1)').find('div').clone();
                            td2 = $('#tBodyExcelLike tr:eq(' + i + ')').find('td:eq(2)').find('div').clone();
                            td3 = $('#tBodyExcelLike tr:eq(' + i + ')').find('td:eq(3)').find('div').clone();
                            td4 = $('#tBodyExcelLike tr:eq(' + i + ')').find('td:eq(4)').find('div').clone();
                            $('#' + tbody + ' tr:eq(' + i + ')').find('td:eq(0)').empty().append(td1);
                            $('#' + tbody + ' tr:eq(' + i + ')').find('td:eq(1)').empty().append(td2);
                            $('#' + tbody + ' tr:eq(' + i + ')').find('td:eq(2)').empty().append(td3);
                            $('#' + tbody + ' tr:eq(' + i + ')').find('td:eq(3)').empty().append(td4);
                            if ($('#tBodyExcelLike tr:eq(' + i + ')').find('td:eq(2)').find('div').attr('associateskuid') != undefined) {
                                $('#' + tbody + ' tr:eq(' + i + ')').find('td:eq(1)').addClass('hasAssociatedSKU');
                            }
                        }
                        else {
                            $('#' + tbody + ' tr:eq(' + currentSameIndex + ')').after($('<tr>').append($('#tBodyExcelLike tr:eq(' + i + ')')
                                                                                                .find('td:eq(1)')
                                                                                                .clone()
                                                                                            )
                                                                                            .append($('#tBodyExcelLike tr:eq(' + i + ')')
                                                                                                .find('td:eq(2)')
                                                                                                .clone()
                                                                                                .addClass('tableCellDisable')
                                                                                            )
                                                                                            .append($('#tBodyExcelLike tr:eq(' + i + ')')
                                                                                                .find('td:eq(3)')
                                                                                                .clone()
                                                                                                .addClass('tableCellDisable')
                                                                                            )
                                                                                            .append($('#tBodyExcelLike tr:eq(' + i + ')')
                                                                                                .find('td:eq(4)')
                                                                                                .clone()
                                                                                            )
                            )
                            AddNewColumnToNewRow($('#' + tbody + ' tr:eq(' + currentSameIndex + ')').closest('tr').next().index())
                            currentSameIndex = currentSameIndex + 1;
                        }
                    }
                }
            }

            function LoadSelectedOrgPeopleCanvass(arr) {
                $('#lstOfSelectedVendors').empty();
                if (arr.length > 0) {
                    for (var i = 0; i < arr.length; i++) {
                        if (arr[i].State != "delete") {
                            if (arr[i].State != "edit") {
                                $('#lstOfSelectedVendors').append($('<li class="list-group-item d-flex align-items-center">')
                                                                .append($('<p class="pr-2 m-0 flex-grow-1">')
                                                                    .text(arr[i].Name)
                                                                )
                                                                .append($('<button data-role="none" class="btn btn-danger">')
                                                                    .append('<span class="bi-trash"></span>')
                                                                    .attr('id', arr[i].ID)
                                                                    .attr('state', arr[i].State)
                                                                    .attr('name', arr[i].Name)
                                                                    .attr('arrIndex', i)
                                                                    .click(function () {
                                                                        var id = $(this).attr('ID');
                                                                        var state = $(this).attr('state');
                                                                        var arrIndex = $(this).attr('arrIndex');
                                                                        var newArr;

                                                                        NewMessageBox('Are you sure want to delete ' + $(this).attr('Name') + ' ?', 'Canvass Form', [
                                                                                {
                                                                                    Text: 'Yes',
                                                                                    Click: function () {
                                                                                        if (state == "edit") {
                                                                                            if (isCanvassMaterials) {
                                                                                                arrCanvassMaterialsOrgPeople[arrIndex].State = "delete"
                                                                                                newArr = arrCanvassMaterialsOrgPeople;
                                                                                            } else {
                                                                                                arrCanvassLaborOrgPeople[arrIndex].State = "delete";
                                                                                                newArr = arrCanvassLaborOrgPeople;
                                                                                            }
                                                                                        }
                                                                                        else {
                                                                                            newArr = $.grep(arr, function (val) {
                                                                                                return val.ID != id;
                                                                                            })
                                                                                        }

                                                                                        if (isCanvassMaterials) {
                                                                                            arrCanvassMaterialsOrgPeople = newArr;
                                                                                        }
                                                                                        else {
                                                                                            arrCanvassLaborOrgPeople = newArr;
                                                                                        }

                                                                                        RemoveFromCanvassSheet(arrIndex, state);
                                                                                        CountVendorSelected();
                                                                                        LoadSelectedOrgPeopleCanvass(newArr);
                                                                                        $('#panListOfVendors').modal('show');
                                                                                    }
                                                                                },
                                                                                {
                                                                                    Text: 'No',
                                                                                    Click: function () {
                                                                                        $('#panListOfVendors').modal('show');
                                                                                    }
                                                                                }
                                                                        ])
                                                                    })
                                                                )
                                )
                            }
                        }
                    }
                }
                else {
                    $('#lstOfSelectedVendors').append($('<li>')
                                                        .text('Empty List')
                        )
                }

                $('#lstOfSelectedVendors');//.listview('refresh');
                Reposition();
            }

            function RemoveFromCanvassSheet(index, state) {
                var tHead, tBody, targetBody, targetHead;
                if (isCanvassMaterials) {
                    tHead = $('#tHeadCanvassMaterialRow');
                    tBody = $('#tBodyCanvassMaterial');
                }
                else {
                    tHead = $('#tHeadCanvassLaborRow');
                    tBody = $('#tBodyCanvassLabor');
                }

                if (state != "add") {
                    tHead.find('th:eq(' + CDec2(index) + ')').css({ display: 'none' });
                    tBody.find('tr').each(function () {
                        $(this).find('td:eq(' + (CDec2(index) + 4) + ')').css({ display: 'none' });
                    })
                }
                else {
                    tHead.find('th:eq(' + CDec2(index) + ')').remove();
                    tBody.find('tr').each(function () {
                        $(this).find('td:eq(' + (CDec2(index) + 4) + ')').remove();
                    })
                }
            }

            function DisplayMinPriceOnProjectBudget() {
                var arrExcludedDeletedVendor = new Array();
                var setCanvassToColumnUCIndex;
                var setCanvassToColumnAMIndex;
                var counterObjCanvassData = 0;
                var tdIndex = 4;
                var thead, tbody;

                $('.custom-btn-tab').removeClass('custom-btn-theme-b');
                $('#btnProjectSummary').removeClass('custom-btn-theme-c').addClass('custom-btn-theme-b');

                if (isCanvassMaterials) {
                    isCanvassMaterialsAlready = true;
                    setCanvassToColumnUCIndex = 5; // Column Unit Cost (Materials)
                    setCanvassToColumnAMIndex = 6;
                    whatArr = arrCanvassMaterialsOrgPeople;
                    thead = "tHeadCanvassMaterialRow";
                    tbody = "tBodyCanvassMaterial";
                    arrCanvassMaterials = new Array();
                    arrCanvassMaterials = arrCanvassMinPrice;
                }
                else {
                    isCanvassLaborAlready = true;
                    setCanvassToColumnUCIndex = 7; // Column Unit Cost (Labor)
                    setCanvassToColumnAMIndex = 8;
                    whatArr = arrCanvassLaborOrgPeople;
                    thead = "tHeadCanvassLaborRow";
                    tbody = "tBodyCanvassLabor";
                    arrCanvassLabor = new Array();
                    arrCanvassLabor = arrCanvassMinPrice;
                }

                //remove deleted vendor from arrCanvassMinPrice
                for (var i = 0; i < arrCanvassMinPrice.length; i++) {
                    var newGroupCanvassMinPrice = new Array();
                    for (var m = 0; m < arrCanvassMinPrice[i].length; m++) {
                        if (arrCanvassMinPrice[i][m][5] != "delete") {
                            newGroupCanvassMinPrice.push(arrCanvassMinPrice[i][m]);
                        }
                    }
                    arrExcludedDeletedVendor.push(newGroupCanvassMinPrice)
                }

                $('#tBodyExcelLike tr:not(":last"):not(".rowSubtotal")').each(function () {
                    var arrNum = arrExcludedDeletedVendor[counterObjCanvassData];
                    var sorted = arrNum.sort(SortMinPrice);

                    $(this).find('td').eq(setCanvassToColumnUCIndex).find('span.custom-tooltip').remove();
                    var appendedVendor = $('<span>', { 'class': 'custom-tooltip' });
                    for (var i = 0; i < sorted.length; i++) {
                        if (sorted[i][5] != "delete") {
                            appendedVendor.append($('<span>')
                                                .addClass('tooltip-container')
                                                .append($('<span>')
                                                        .addClass('grid-left')
                                                        .text(sorted[i][1])
                                                    )
                                                .append($('<span>')
                                                        .addClass('grid-right')
                                                        .text(sorted[i][0])
                                                    )
                            )
                        }
                    }
                    $(this).find('td').eq(setCanvassToColumnUCIndex)
                        .append(appendedVendor);

                    if (arrExcludedDeletedVendor[0].length != 0) {
                        var displayCost = sorted[0][0], displaySKUID = sorted[0][2];
                        for (var i = 0; i < sorted.length; i++) {
                            if (sorted[i][7] == 1) {
                                displayCost = sorted[i][0];
                                displaySKUID = sorted[i][2];
                            }
                        }
                        console.log(displayCost)
                        $(this).find('td').eq(setCanvassToColumnUCIndex).find('div').attr('vid', displaySKUID).text(displayCost == "0" ? "-" : AccountingFormat($.format.number(displayCost)));
                        $(this).find('td').eq(setCanvassToColumnAMIndex).find('div').attr('vid', displaySKUID).text(($.trim($(this).find('td').eq(3).text()) == "" ? "-" : AccountingFormat($.format.number($(this).find('td').eq(3).text() * displayCost))));
                    }
                    else {
                        $(this).find('td').eq(setCanvassToColumnUCIndex).find('div').attr('vid', displaySKUID).text(AccountingFormat(0));
                        $(this).find('td').eq(setCanvassToColumnAMIndex).find('div').attr('vid', displaySKUID).text(($.trim($(this).find('td').eq(3).text()) == "" ? "-" : AccountingFormat($.format.number($(this).find('td').eq(3).text() * displayCost))));
                    }

                    if (($.trim($(this).find('td').eq(5).find('div').text()) != "" && $.trim($(this).find('td').eq(7).find('div').text()) == "") ||
                        ($.trim($(this).find('td').eq(6).find('div').text()) != "" && $.trim($(this).find('td').eq(8).find('div').text()) == "")
                    ) {
                        $(this).find('td').eq(9).find('div').addClass('NumericCell').text($.trim($(this).find('td').eq(5).find('div').text()) == 0 ? "-" : AccountingFormat($.format.number(CDec2($(this).find('td').eq(5).find('div').text()))));
                        $(this).find('td').eq(10).find('div').addClass('NumericCell').text($.trim($(this).find('td').eq(6).find('div').text()) == 0 ? "-" : AccountingFormat($.format.number(CDec2($(this).find('td').eq(6).find('div').text()))));
                    }
                    else if (($.trim($(this).find('td').eq(5).find('div').text()) == "" && $.trim($(this).find('td').eq(7).find('div').text()) != "") ||
                            ($.trim($(this).find('td').eq(6).find('div').text()) == "" && $.trim($(this).find('td').eq(8).find('div').text()) != "")
                    ) {
                        $(this).find('td').eq(9).find('div').addClass('NumericCell').text($.trim($(this).find('td').eq(7).find('div').text()) == 0 ? "-" : AccountingFormat($.format.number(CDec2($(this).find('td').eq(7).find('div').text()))));
                        $(this).find('td').eq(10).find('div').addClass('NumericCell').text($.trim($(this).find('td').eq(8).find('div').text()) == 0 ? "-" : AccountingFormat($.format.number(CDec2($(this).find('td').eq(8).find('div').text()))));
                    }
                    else {
                        $(this).find('td').eq(9).find('div').addClass('NumericCell').text(CDec2($.trim($(this).find('td').eq(5).find('div').text())) + CDec2($.trim($(this).find('td').eq(7).find('div').text())) == 0 ? "-" : (AccountingFormat($.format.number(CDec2($(this).find('td').eq(5).find('div').text()) + CDec2($(this).find('td').eq(7).find('div').text())))));
                        $(this).find('td').eq(10).find('div').addClass('NumericCell').text(CDec2($.trim($(this).find('td').eq(6).find('div').text())) + CDec2($.trim($(this).find('td').eq(8).find('div').text())) == 0 ? "-" : (AccountingFormat($.format.number(CDec2($(this).find('td').eq(6).find('div').text()) + CDec2($(this).find('td').eq(8).find('div').text())))));
                    }

                    counterObjCanvassData = counterObjCanvassData + 1;
                })

                //GetCanvassData(tbody);
                $('#panCanvassOptions').popup('close');
                $('.divCanvass').hide();
                $('.divNewProjectBudget').show();
            }

            function SortMinPrice(a, b) {
                if (a[0] < b[0]) return -1;
                if (a[0] > b[0]) return 1;
                return 0;
            }

            function DisplaySelVendorOnProjectBudget(Vendor) {
                var currentVendorIndexTD;
                var setCanvassToColumnUCIndex;
                var setCanvassToColumnAMIndex;
                var counterObjCanvassData = 0;
                var arrCanvassData = new Array();
                var thead, tbody;

                if (isCanvassMaterials) {
                    thead = "tHeadCanvassMaterialRow";
                    tbody = "tBodyCanvassMaterial";
                    arrCanvassMaterials = new Array();
                    arrCanvassMaterials = arrCanvassMinPrice;
                }
                else {
                    thead = "tHeadCanvassLaborRow";
                    tbody = "tBodyCanvassLabor";
                    arrCanvassLabor = new Array();
                    arrCanvassLabor = arrCanvassMinPrice;
                }

                currentVendorIndexTD = $('#' + thead + ' th:contains("' + Vendor.Name + '")').index();

                $('#' + tbody + ' tr').each(function () {
                    arrCanvassData.push({
                        "Cost": $(this).find('td').eq(currentVendorIndexTD + 4).find('div').text()
                    })
                })

                if (isCanvassMaterials) {
                    setCanvassToColumnUCIndex = 5; // Column Unit Cost (Materials)
                    setCanvassToColumnAMIndex = 6;
                    whatArr = arrCanvassMaterialsOrgPeople;
                }
                else {
                    setCanvassToColumnUCIndex = 7; // Column Unit Cost (Labor)
                    setCanvassToColumnAMIndex = 8;
                    whatArr = arrCanvassLaborOrgPeople;
                }

                $('#tBodyExcelLike tr:not(":last")').each(function () {
                    var arrNum = arrCanvassMinPrice[counterObjCanvassData];
                    var sorted = arrNum.sort(SortMinPrice);

                    $(this).find('td').eq(setCanvassToColumnUCIndex).find('span.custom-tooltip').remove();

                    var appendedVendor = $('<span>', { 'class': 'custom-tooltip' });
                    for (var i = 0; i < sorted.length; i++) {
                        if (sorted[i][5] != "delete") {
                            appendedVendor.append($('<span>')
                                                .addClass('tooltip-container')
                                                .append($('<span>')
                                                        .addClass('grid-left')
                                                        .text(sorted[i][1])
                                                    )
                                                .append($('<span>')
                                                        .addClass('grid-right')
                                                        .text(sorted[i][0])
                                                    )
                            )
                        }
                    }
                    $(this).find('td').eq(setCanvassToColumnUCIndex)
                        .append(appendedVendor);

                    $(this).find('td').eq(setCanvassToColumnUCIndex).find('div').attr('vid', Vendor.ID).text(arrCanvassData[counterObjCanvassData].Cost == 0 ? "0" : AccountingFormat($.format.number(arrCanvassData[counterObjCanvassData].Cost)));
                    $(this).find('td').eq(setCanvassToColumnAMIndex).find('div').attr('vid', Vendor.ID).text(($.trim($(this).find('td').eq(3).text()) == "" ? "0" : AccountingFormat($.format.number($(this).find('td').eq(3).text() * arrCanvassData[counterObjCanvassData].Cost))));

                    if (($.trim($(this).find('td').eq(5).find('div').text()) != "" && $.trim($(this).find('td').eq(7).find('div').text()) == "") ||
                        ($.trim($(this).find('td').eq(6).find('div').text()) != "" && $.trim($(this).find('td').eq(8).find('div').text()) == "")
                    ) {
                        $(this).find('td').eq(9).find('div').addClass('NumericCell').text($.trim($(this).find('td').eq(5).find('div').text()) == 0 ? "0" : AccountingFormat($.format.number(CDec2($(this).find('td').eq(5).find('div').text()))));
                        $(this).find('td').eq(10).find('div').addClass('NumericCell').text($.trim($(this).find('td').eq(6).find('div').text()) == 0 ? "0" : AccountingFormat($.format.number(CDec2($(this).find('td').eq(6).find('div').text()))));
                    }
                    else if (($.trim($(this).find('td').eq(5).find('div').text()) == "" && $.trim($(this).find('td').eq(7).find('div').text()) != "") ||
                            ($.trim($(this).find('td').eq(6).find('div').text()) == "" && $.trim($(this).find('td').eq(8).find('div').text()) != "")
                    ) {
                        $(this).find('td').eq(9).find('div').addClass('NumericCell').text($.trim($(this).find('td').eq(7).find('div').text()) == 0 ? "0" : AccountingFormat($.format.number(CDec2($(this).find('td').eq(7).find('div').text()))));
                        $(this).find('td').eq(10).find('div').addClass('NumericCell').text($.trim($(this).find('td').eq(8).find('div').text()) == 0 ? "0" : AccountingFormat($.format.number(CDec2($(this).find('td').eq(8).find('div').text()))));
                    }
                    else {
                        $(this).find('td').eq(9).find('div').addClass('NumericCell').text(CDec2($.trim($(this).find('td').eq(5).find('div').text())) + CDec2($.trim($(this).find('td').eq(7).find('div').text())) == 0 ? "0" : (AccountingFormat($.format.number(CDec2($(this).find('td').eq(5).find('div').text()) + CDec2($(this).find('td').eq(7).find('div').text())))));
                        $(this).find('td').eq(10).find('div').addClass('NumericCell').text(CDec2($.trim($(this).find('td').eq(6).find('div').text())) + CDec2($.trim($(this).find('td').eq(8).find('div').text())) == 0 ? "0" : (AccountingFormat($.format.number(CDec2($(this).find('td').eq(6).find('div').text()) + CDec2($(this).find('td').eq(8).find('div').text())))));
                    }

                    counterObjCanvassData = counterObjCanvassData + 1;
                })
            }

            function GetCanvassData() {
                var thead, tbody, ctr = 0;
                arrCanvassMinPrice = new Array();

                if (isCanvassMaterials) {
                    thead = "tHeadCanvassMaterialRow";
                    tbody = "tBodyCanvassMaterial";
                    whatArr = arrCanvassMaterialsOrgPeople;
                }
                else {
                    thead = "tHeadCanvassLaborRow";
                    tbody = "tBodyCanvassLabor";
                    whatArr = arrCanvassLaborOrgPeople;
                }

                $('#' + tbody + '').find('tr').each(function () {
                    var td = $(this).find('td'), tdIndex = 4;
                    groupCanvassMinPrice = new Array();
                    for (var i = 0; i < $('#' + thead + ' th').length; i++) {
                        objCanvassMinPrice = new Array();

                        objCanvassMinPrice[0] = CDec2($.trim(td.eq(tdIndex).find('div').text())); // Cost Value
                        objCanvassMinPrice[1] = whatArr[i].Name; // Company or People Name
                        objCanvassMinPrice[2] = whatArr[i].ID; // Company or PeopleID
                        objCanvassMinPrice[3] = sessionStorage.OrgID;
                        objCanvassMinPrice[4] = sessionStorage.ProjectID;
                        objCanvassMinPrice[5] = whatArr[i].State; // Edit or Add
                        objCanvassMinPrice[6] = $.trim(td.eq(tdIndex).find('div').attr('id')) //whatArr[i].IDDB; // ID from DB
                        objCanvassMinPrice[7] = (td.eq(tdIndex).attr('isPreferredCanvass') == undefined ? "0" : td.eq(tdIndex).attr('isPreferredCanvass')) // is preferred vendor

                        groupCanvassMinPrice.push(objCanvassMinPrice);

                        tdIndex = tdIndex + 1;
                    }
                    arrCanvassMinPrice.push(groupCanvassMinPrice);
                })
            }

            function AddNewColumnToNewRow(index) {
                var appendcolumn;
                var tbody;

                if (isCanvassMaterials) {
                    tbody = "tBodyCanvassMaterial";
                    whatArr = arrCanvassMaterialsOrgPeople;
                }
                else {
                    tbody = "tBodyCanvassLabor";
                    whatArr = arrCanvassLaborOrgPeople;
                }

                var appendTo = $('#' + tbody + ' tr:eq(' + index + ')');

                for (var i = 0; i < whatArr.length; i++) {
                    appendTo.append($('<td>')
                                        .append($('<div>')
                                            .css({ width: '100%' })
                                            .html('&nbsp;')
                                            .attr('columnName', 'qty')
                                            .attr('datatype', 'numeric')
                                            .attr('vid', whatArr[0].ID)
                                            .attr('state', 'add')
                                        )
                                        .addClass('tableCell')
                            );
                }
            }

            function AppendNewColumn() {
                var appendcolumn;
                var tbody, thead;

                if (isCanvassMaterials) {
                    tbody = "tBodyCanvassMaterial";
                    thead = $('#tHeadCanvassMaterialRow');
                    whatArr = arrCanvassMaterialsOrgPeople;
                    if (isAddNewVendorCanvassMaterials) {
                        whatArr = $.grep(arrCanvassMaterialsOrgPeople, function (val) {
                            return val.State == "add"
                        })
                    }
                }
                else {
                    tbody = "tBodyCanvassLabor";
                    thead = $('#tHeadCanvassLaborRow');
                    whatArr = arrCanvassLaborOrgPeople;
                    if (isAddNewVendorCanvassLabor) {
                        whatArr = $.grep(arrCanvassLaborOrgPeople, function (val) {
                            return val.State == "add"
                        })
                    }
                }

                var newWhatArr = remove_duplicates(whatArr);
                for (var i = 0; i < newWhatArr.length; i++) {
                    $("#" + tbody + " tr").each(function () {
                        $(this).append($('<td>')
                                        .append($('<div>')
                                            .css({ width: '100%' })
                                            .html('&nbsp;')
                                            .attr('columnName', 'cost')
                                            .attr('datatype', 'numeric')
                                            .attr('vid', whatArr[i].ID)
                                            .attr('state', 'add')
                                        )
                                        .addClass('tableCell')
                            );
                    })
                }

                $("#" + tbody + " tr:first td:eq(4)").addClass('activeCell')
            }

            function LoadRequiredFunctions() {
                GetProjectHeader();
                //AdjustContainerMarginTop();
                getOrgAddFowstType();
                GetUomList();
                GetProjectPermissions();
                //window.location.hash = '';
                //$('#btnMainDashboard').click();

                AjaxSend({
                    url: 'Browser/LibraryCities',
                    data: { IsAll: true },
                    success: function (e) {
                        var vRows = e.Results.Rows;
                        var selProjectNewClientCity = $('#selProjectNewClientCity');
                        selProjectNewClientCity.empty();

                        for (var i = 0; i < vRows.length; i++) {
                            if (vRows[i].Name != "")
                                selProjectNewClientCity.append(new Option(vRows[i].Name, vRows[i].ID))
                        }

                        selProjectNewClientCity.val('316a03d3-611a-4376-a2b2-7d0dfd7a4fbc'); // default to Davao City
                    }
                })

                AjaxSend({
                    url: 'Browser/LibraryCountries',
                    data: { IsAll: 1 },
                    success: function (e) {
                        var vRows = e.Results.Rows;
                        var selProjectNewClientCountry = $('#selProjectNewClientCountry');
                        selProjectNewClientCountry.empty();

                        for (var i = 0; i < vRows.length; i++) {
                            selProjectNewClientCountry.append(new Option(vRows[i].Name, vRows[i].ID))
                        }

                        selProjectNewClientCountry.val('e783ed13-aa98-4cc1-918e-d49b05c0167e'); // default to Philippines
                    }
                })
            }

            function GetProjectPermissions() {
                AjaxSend({
                    url: 'BOMProjectExcel/GetProjectPermissions',
                    success: function (e) {
                        arrPermissions = e.Results.Rows;
                    }
                })
            }

            function GetUomList() {
                var selMaterialBudgetUom = $('#selMaterialBudgetUom');
                var selAccReqUom = $('#selAccReqUom');
                var selAccUsedUpUom = $('#selAccUsedUpUom');

                //selMaterialBudgetUom.empty();
                //selAccReqUom.empty();
                //selAccUsedUpUom.empty();

                AjaxSend({
                    url: 'Browser/LibraryPimMeasure',
                    data: { IsAll: true },
                    success: function (e) {
                        var vRows = e.Results.Rows;

                        for (var i = 0; i < vRows.length; i++) {
                            selMaterialBudgetUom.append(new Option(vRows[i].Measurement, vRows[i].ID));
                            selAccReqUom.append(new Option(vRows[i].Measurement, vRows[i].ID));
                            selAccUsedUpUom.append(new Option(vRows[i].Measurement, vRows[i].ID));
                        }
                    }
                })
            }

            function AddEmailInfo() {

                var pLV = $('#lstOrgFOWSTEmail2');
                var pItem;

                pLV.html('');
                for (var i = 0; i < arrEmailInfoList.length; i++) {
                    var arrindex = i;
                    pItem = $('<li class="list-group-item d-flex align-items-center">')
                                .append($('<p class="pr-2 m-0 flex-grow-1">')
                                        .attr('arrindex', i)
                                        .html($.trim(arrEmailInfoList[i].EmailAdd) +
                                            "<br>" + $.trim(arrEmailInfoList[i].EmailType)
                                        )
                                        .click(function (e) {
                                            EditEmailInfo($(this).attr('arrindex'));
                                        })
                                )
                                .append($('<button data-role="none" class="btn btn-danger">')
                                        .append('<span class="bi-trash"></span>')
                                        .attr('id', arrEmailInfoList[i].EmailRankNo)
                                        .data('data', arrEmailInfoList[i])
                                        .click(function (e) {
                                            var data = $(this).data('data');
                                            var EmailRankNo = data.EmailRankNo;

                                            NewMessageBox('Are you sure you want to remove <b>' + data.EmailAdd + '</b>?', 'Message', [
                                                {
                                                    Text: 'Yes', Click: function () {
                                                        if (data.IsNew == 1) {
                                                            //remove selected email
                                                            arrEmailInfoList = $.grep(arrEmailInfoList, function (val) {
                                                                return val.EmailRankNo != EmailRankNo;
                                                            })
                                                        }
                                                        else {
                                                            arrEmailInfoList[arrindex].EmailState = "Remove";
                                                        }

                                                        AddEmailInfo();
                                                    }
                                                }, { Text: 'No' }
                                            ])                                                    
                                        })
                                )

                    pLV.append(pItem);
                }

                pLV;//.listview('refresh');
                $('#popupProjectEmailAdd').modal('hide');
            }

            function EditEmailInfo(arrindex) {
                $('#popupProjectEmailAdd').modal('show');
                EmailRankNo = arrEmailInfoList[arrindex].EmailRankNo;
                EmailState = "Edit";
                sessionStorage["Emailindex"] = arrindex;
                sessionStorage["EmailIsNew"] = arrEmailInfoList[arrindex].IsNew;
                sessionStorage["EmailState"] = arrEmailInfoList[arrindex].EmailState;
                $('#txtOrgFOWSTEmailID2').val(arrEmailInfoList[arrindex].id == undefined ? "" : arrEmailInfoList[arrindex].id);
                $('#txtEmailAdd2').val(arrEmailInfoList[arrindex].EmailAdd);
                $('#selOrgFOWSTEmailType2').val(arrEmailInfoList[arrindex].EmailType);//.selectmenu('refresh');
            }

            function AddMobileInfo() {

                var pLV = $('#lstOrgFOWSTMobile2');
                var pItem;

                pLV.html('');
                for (var i = 0; i < arrMobileInfoList.length; i++) {
                    if (arrMobileInfoList[i].MobileState != "Remove") {
                        var arrindex = i;
                        pItem = $('<li class="list-group-item d-flex align-items-center">')
                                .append($('<p class="pr-2 m-0 flex-grow-1">')
                                        .attr('arrindex', i)
                                        .attr('ID', arrMobileInfoList[i].id)
                                        .html("+" + $.trim(arrMobileInfoList[i].CountryCode.split('-')[1]) + "." + $.trim(arrMobileInfoList[i].TelCode.split('-')[1]) + "." + $.trim(arrMobileInfoList[i].MobileNo) +
                                            "<br>" + $.trim(arrMobileInfoList[i].TelCode.split('-')[0]) + " (" + $.trim(arrMobileInfoList[i].CountryCode.split('-')[0]) + ") " +
                                            "<br>" + $.trim(arrMobileInfoList[i].ContactType)
                                        )
                                        .click(function (e) {
                                            EditMobileInfo($(this).attr('arrindex'));
                                        })
                                )
                                .append($('<button data-role="none" class="btn btn-danger">')
                                        .append('<span class="bi-trash"></span>')
                                        .attr('id', arrMobileInfoList[i].MobileRankNo)
                                        .data('data', arrMobileInfoList[i])
                                        .click(function (e) {
                                            var data = $(this).data('data');
                                            var MobileRankNo = data.MobileRankNo;

                                            NewMessageBox('Are you sure you want to remove this record?', 'Message', [
                                                {
                                                    Text: 'Yes', Click: function () {
                                                        if (data.IsNew == 1) {
                                                            //remove selected mobile
                                                            arrMobileInfoList = $.grep(arrMobileInfoList, function (val) {
                                                                return val.MobileRankNo != MobileRankNo;
                                                            })
                                                        }
                                                        else {
                                                            arrMobileInfoList[arrindex].MobileState = "Remove";
                                                        }

                                                        AddMobileInfo();
                                                    }
                                                }, { Text: 'No' }
                                            ])                                                    
                                        })
                                )

                        pLV.append(pItem);
                    }
                }

                pLV;//.listview('refresh');
                $('#popupProjectMobile').modal('hide');
            }

            function EditMobileInfo(arrindex) {
                $('#popupProjectMobile').modal('show');
                MobileRankNo = arrMobileInfoList[arrindex].MobileRankNo;
                MobileState = "Edit";
                sessionStorage["Mobileindex"] = arrindex;
                sessionStorage["MobileIsNew"] = arrMobileInfoList[arrindex].IsNew;
                sessionStorage["MobileState"] = arrMobileInfoList[arrindex].MobileState;
                $('#txtOrgFOWSTMobileID2').val(arrMobileInfoList[arrindex].id == undefined ? "" : arrMobileInfoList[arrindex].id);
                $('#txtOrgFOWSTTelCoPrefixID2').val(arrMobileInfoList[arrindex].TelCodeID);
                $('#txtOrgFOWSTMobileCountryID2').val(arrMobileInfoList[arrindex].CountryCodeID);
                $('#txtMobileCountryCode').text(arrMobileInfoList[arrindex].CountryCode);
                $('#txtTelCode').text(arrMobileInfoList[arrindex].TelCode);
                $('#txtMobileNo2').val(arrMobileInfoList[arrindex].MobileNo);
                $('#selOrgFOWSTMobileType2').val(arrMobileInfoList[arrindex].ContactType);//.selectmenu('refresh');
            }

            function AddTelInfo() {

                var pLV = $('#lstOrgFOWSTTelephone2');
                var pItem;

                pLV.html('');
                for (var i = 0; i < arrTelInfoList.length; i++) {
                    var arrindex = i;

                    if (arrTelInfoList[i].TelState != "Remove") {
                        pItem = $('<li class="list-group-item d-flex align-items-center">')
                                .append($('<p class="pr-2 m-0 flex-grow-1">')
                                        .attr('arrindex', i)
                                        .attr('ID', arrTelInfoList[i].id)
                                        .html("+" + $.trim(arrTelInfoList[i].CountryCode.split('-')[1]) + "." + $.trim(arrTelInfoList[i].AreaCode.split('-')[1]) + "." + $.trim(arrTelInfoList[i].TelephoneNo) +
                                            "<br>" + $.trim(arrTelInfoList[i].AreaCode.split('-')[0]) + ", " + $.trim(arrTelInfoList[i].CountryCode.split('-')[0]) +
                                            "<br>" + $.trim(arrTelInfoList[i].ContactType)
                                        )
                                        .data('data', arrTelInfoList[i])
                                        .click(function (e) {
                                            EditTelInfo($(this).attr('arrindex'));
                                        })
                                )
                                .append($('<button data-role="none" class="btn btn-danger">')
                                        .append('<span class="bi-trash"></span>')
                                        .attr('id', arrTelInfoList[i].TelRankNo)
                                        .data('data', arrTelInfoList[i])
                                        .click(function (e) {
                                            var data = $(this).data('data');
                                            var TelRankNo = data.TelRankNo;

                                            NewMessageBox('Are you sure you want to remove this record?', 'Message', [
                                                {
                                                    Text: 'Yes', Click: function () {
                                                        if (data.IsNew == 1) {
                                                            //remove selected telephone
                                                            arrTelInfoList = $.grep(arrTelInfoList, function (val) {
                                                                return val.TelRankNo != TelRankNo;
                                                            })
                                                        }
                                                        else {
                                                            arrTelInfoList[arrindex].TelState = "Remove";
                                                        }

                                                        AddTelInfo();
                                                    }
                                                }, { Text: 'No' }
                                            ])                                                    
                                        })
                                )

                        pLV.append(pItem);
                    }

                }

                pLV;//.listview('refresh');
                $('#popupProjectTelephone').modal('hide');
            }

            function EditTelInfo(arrindex) {
                $('#popupProjectTelephone').modal('show');

                TelRankNo = arrTelInfoList[arrindex].TelRankNo;
                TelState = "Edit";
                sessionStorage["Telindex"] = arrindex;
                sessionStorage["TelIsNew"] = arrTelInfoList[arrindex].IsNew;
                sessionStorage["TelState"] = arrTelInfoList[arrindex].TelState;
                $('#txtOrgFOWSTTelID2').val(arrTelInfoList[arrindex].id == undefined ? "" : arrTelInfoList[arrindex].id);
                $('#txtOrgFOWSTTelAreaCodeID2').val(arrTelInfoList[arrindex].AreaCodeID);
                $('#txtOrgFOWSTTelCountryID2').val(arrTelInfoList[arrindex].CountryCodeID);
                $('#txtCountryCode').text(arrTelInfoList[arrindex].CountryCode);
                $('#txtAreaCode').text(arrTelInfoList[arrindex].AreaCode);
                $('#txtTelephoneNum').val(arrTelInfoList[arrindex].TelephoneNo);
                $('#txtTelephoneExt').val(arrTelInfoList[arrindex].ExtensionNo);
                $('#selOrgFOWSTTelType2').val(arrTelInfoList[arrindex].ContactType);//.selectmenu('refresh');
            }

            function GetOrgDetailsToPrint(VendorData) {
                var pData = { OrgID: sessionStorage.OrgID };

                AjaxSend({
                    url: 'BOMProjectExcel/GetOrgDetailsToPrint',
                    data: pData,
                    success: function (vData) {
                        $('#txtCompanyName').text(vData.OrgDetails.Rows[0].CompanyName);
                        $('#txtBusinessAddress').text(vData.OrgDetails.Rows[0].BusinessAddress);
                        $('#txtTelephoneNo').text(vData.OrgDetails.Rows[0].TelephoneNo);
                        $('#txtFaxNo').text(vData.OrgDetails.Rows[0].FaxNo)
                        $('#txtMobileNum').text(vData.OrgDetails.Rows[0].MobileNo)
                        $('#txtBusinessEmail').text(vData.OrgDetails.Rows[0].EmailAddress);
                        PrintCanvassForm(VendorData);
                        isPrintCanvass = false;
                    }
                })
            }

            function SearchOrganization(s) {
                var pData = {};
                var pLV;

                if (NameOfBtnForLibrary == "ProjectCustomer") {
                    pLV = $('#lstOfProjectCustomer');
                }
                else {
                    pLV = $('#lstOfOrgEmp');
                }

                pData.keywords = s;

                pLV.empty();

                AjaxSend({
                    url: 'Browser/LibraryOrganization',
                    data: pData,
                    success: function (vData) {
                        if (vData.Results.Rows.length > 0) {
                            isNoResultSearchOrgPeople = false;
                            for (var p = 0; p < vData.Results.Rows.length; p++) {
                                pLV.append($('<li>')
                                    .append($('<a>')
                                        .attr('ID', vData.Results.Rows[p].ID)
                                        .append($('<br><p style="font-weight:bold;">')
                                            .text(vData.Results.Rows[p].CompanyName)
                                        )
                                        .data('data', vData.Results.Rows[p])
                                        .click(function (e) {
                                            var ID = $(this).attr('id')
                                            var CompanyName = $(this).data('data').CompanyName;
                                            var isFoundVendor;

                                            if (NameOfBtnForLibrary == "ProjectCustomer") {
                                                $('#txtClientID').val(ID);
                                                $('#panSearchProjectCustomer').popup('close');
                                                $('#txtClientName').text(CompanyName);
                                                NameOfBtnForLibrary = '';
                                                GetOrgFowst(ID);
                                            }
                                            else {
                                                $('#divVendorValidation').html('').hide();

                                                if (isCanvassMaterials) {

                                                    if (arrCanvassMaterialsOrgPeople.length > 0) {
                                                        isFoundVendor = $.grep(arrCanvassMaterialsOrgPeople, function (val) {
                                                            if (val.ID == ID) return true;
                                                        });
                                                        if (isFoundVendor.length > 0) {
                                                            $('#divVendorValidation').html('<br>' + $(this).data('data').CompanyName + ' already selected.<br><br>').show();
                                                            return false;
                                                        }
                                                    }

                                                    if ($(this).closest('li').children('div').hasClass('selectedList')) {
                                                        $(this).closest('li').children('div').removeClass('selectedList');
                                                        arrCanvassMaterialsOrgPeople = $.grep(arrCanvassMaterialsOrgPeople, function (val) {
                                                            return val.ID != ID;
                                                        })
                                                        CountVendorSelected();
                                                    }
                                                    else {
                                                        if (arrCanvassMaterialsOrgPeople.length > 9) {
                                                            $('#divVendorValidation').html('<br>Maximun of 10 Vendors only are allowed to canvass<br><br>').show();
                                                            return false;
                                                        }

                                                        $(this).closest('li').children('div').addClass('selectedList');
                                                        arrCanvassMaterialsOrgPeople.push({
                                                            "ID": ID,
                                                            "Name": $(this).data('data').CompanyName,
                                                            "State": "add"
                                                        })
                                                        CountVendorSelected();
                                                    }
                                                    //isCanvassMaterialsAlready = true;
                                                }
                                                else {
                                                    if (arrCanvassLaborOrgPeople.length > 0) {
                                                        isFoundVendor = $.grep(arrCanvassLaborOrgPeople, function (val) {
                                                            if (val.ID == ID) return true;
                                                        });
                                                        if (isFoundVendor.length > 0) {
                                                            $('#divVendorValidation').html('<br>' + $(this).data('data').CompanyName + ' already selected.<br><br>').show();
                                                            return false;
                                                        }
                                                    }

                                                    if ($(this).closest('li').children('div').hasClass('selectedList')) {
                                                        $(this).closest('li').children('div').removeClass('selectedList');
                                                        arrCanvassLaborOrgPeople = $.grep(arrCanvassLaborOrgPeople, function (val) {
                                                            return val.ID != ID;
                                                        })
                                                        CountVendorSelected();
                                                    }
                                                    else {
                                                        if (arrCanvassLaborOrgPeople.length > 9) {
                                                            $('#divVendorValidation').html('<br>Maximun of 10 Vendors only are allowed to canvass<br><br>').show();
                                                            return false;
                                                        }

                                                        $(this).closest('li').children('div').addClass('selectedList');
                                                        arrCanvassLaborOrgPeople.push({
                                                            "ID": ID,
                                                            "Name": $(this).data('data').CompanyName,
                                                            "State": "add"
                                                        })
                                                        CountVendorSelected();
                                                    }
                                                    //  isCanvassLaborAlready = true;
                                                }
                                            }


                                            $('#panSearchLibrary').popup('close');
                                        })
                                    ));
                            }
                        }
                        else {
                            //$('#lstOfOrgEmp').append($('<li>').text('No Result(s) Found'));
                            isNoResultSearchOrgPeople = true;
                        }

                        pLV;//.listview('refresh');
                        Reposition();

                        SearchEmployeeName2(s);
                    }
                });
            }

            function GetOrgFowst(id, FowstID) {
                $('#selFowstLocation');//.selectmenu('enable');
                $('#selFowstLocation').empty();
                $('#txtFowstLocation');//.textinput('enable');//.textinput();
                AjaxSend({
                    url: 'BOMProjectExcel/GetOrgFowst',
                    data: { OrgID: id },
                    success: function (vData) {
                        if (vData.Error == "Invalid organization") {
                            $('#selFowstLocation').append($('<option>', {
                                value: "",
                                text: "No Result(s) found for selected organization"
                            }))
                        }
                        else {
                            if (vData.Results.Rows.length > 0) {
                                $('#selFowstLocation').append($('<option>', {
                                    value: "",
                                    text: "Please Select"
                                }))

                                for (var i = 0; i < vData.Results.Rows.length; i++) {
                                    if (vData.Results.Rows[i].FowstName != undefined) {
                                        $('#selFowstLocation').append($('<option>', {
                                            value: vData.Results.Rows[i].ID,
                                            text: vData.Results.Rows[i].FowstName
                                        }))
                                    }
                                    else {
                                        $('#selFowstLocation').empty().append($('<option>', {
                                            value: "",
                                            text: "No Result(s) found for selected organization"
                                        }))
                                    }
                                }

                                $('#selFowstLocation').val(FowstID);
                                $('#txtFowstLocation').val(vData.Results.Rows[0].FowstName);
                                $('#txtFowstLocationID').val(vData.Results.Rows[0].ID);
                            }
                            else {
                                $('#selFowstLocation').append($('<option>', {
                                    value: "",
                                    text: "No Result(s) found for selected organization"
                                }))
                            }
                        }

                        $('#selFowstLocation');//.selectmenu('refresh');
                    }
                })
            }

            function CountVendorSelected() {
                if (isCanvassMaterials) {
                    whatArr = arrCanvassMaterialsOrgPeople;
                }
                else {
                    whatArr = arrCanvassLaborOrgPeople;
                }

                if (whatArr.length == 0) {
                    $('#divVendorSelection').hide();
                    $('#divVendorValidation').hide();
                    $('.divCountVendor').html('');
                }
                else if (whatArr.length == 1) {
                    $('#divVendorSelection').show();
                }
                else {
                    $('#divVendorSelection').show();
                    $('.divCountVendor').text(whatArr.length + ' of 10 Vendor to canvass');
                }
            }

            function SKUUnitOfMeasure(s) {
                var pData = {};

                pData.ProductID = s;

                AjaxSend({
                    url: 'BOMProjectExcel/GetPIMUoM',
                    data: pData,
                    success: function (e) {
                        console.log(e)
                        var currentActiveDiv = $('div[descriptionid="' + itemAssociateSKUID + '"]');
                        var currentCell = currentActiveDiv.closest('tr').find('td');

                        currentActiveDiv.attr('associateskuid', arrSelectedSKU.AssociateSKUID);
                        //currentActiveDiv.attr('skuname', "SKU Name: " + arrSelectedSKU.SKUName)
                        currentActiveDiv.attr('data-original-title', "SKU Name: " + arrSelectedSKU.SKUName)
                        currentActiveDiv.closest('td').removeClass('errorCell');
                        currentCell.eq(2).addClass('hasAssociatedSKU');
                        currentCell.eq(2).append('<pre class="preAssociatedSKU" style="width:' + $('#tBodyExcelLike tr td.activeCell').width() + 'px;">' + arrSelectedSKU.SKUName + '</pre>')
                        currentCell.eq(4).find('div').text(e.Unit.UnitMeasure);
                        currentCell.eq(4).find('div').attr('umid', e.Unit.UnitMeasureID)
                    }
                })

                return

                $('#radSKUUnitMeasure').empty();

                AjaxSend({
                    url: 'Browser/LibraryPIMProductMeasure',
                    data: pData,
                    success: function (vData) {

                        if (vData.Results.Rows.length > 0) {
                            for (var p = 0; p < vData.Results.Rows.length; p++) {
                                $('<input type="radio" name="radSKUUnitMeasure" umid="' + vData.Results.Rows[p].UnitMeasureID + '" value="' + vData.Results.Rows[p].Measurement + '" id="radio-choice-' + p + '"><label for="radio-choice-' + p + '">' + vData.Results.Rows[p].Measurement + '</label>').appendTo("#radSKUUnitMeasure");
                            }

                            if (vData.Results.Rows.length == 1) {
                                $("input[name='radSKUUnitMeasure']:first").attr("checked", true);
                            }
                            $('#panSKUUnitOfMeasures');//.trigger('create');
                        }
                        else {
                            $('<span>No Unit Measure found of the selected SKU</span>').appendTo('#radSKUUnitMeasure');
                        }
                        Reposition();
                    }
                });
            }

            function SearchCountryPopUp2(s) {
                var pData = {};

                pData.keywords = s;

                AjaxSend({
                    url: 'Browser/LibraryCountries',
                    data: pData,
                    success: function (vData) {
                        $('#lstOfLibrary').empty();

                        if (vData.Results.Rows.length > 0) {
                            for (var p = 0; p < vData.Results.Rows.length; p++) {
                                $('#lstOfLibrary').append($('<li>')
                                    .append($('<a>')
                                        .attr('ID', vData.Results.Rows[p].ID)
                                        .append($('<br><p style="font-weight:bold;">')
                                            .text(vData.Results.Rows[p].Name)
                                        )
                                        .data('data', vData.Results.Rows[p])
                                        .click(function (e) {
                                            if (NameOfBtnForLibrary == "Telephone") {
                                                $('#txtOrgFOWSTTelCountryID2').val($(this).data('data').ID);
                                                $('#txtCountryCode').text($(this).data('data').Code + ' - ' + $(this).data('data').CallingCode);
                                            }
                                            else if (NameOfBtnForLibrary == "Mobile") {
                                                $('#txtOrgFOWSTMobileCountryID2').val($(this).data('data').ID);
                                                $('#txtMobileCountryCode').text($(this).data('data').Code + ' - ' + $(this).data('data').CallingCode);
                                            }
                                            else if (NameOfBtnForLibrary == "CompleteAddress") {
                                                $('#txtOrgFOWSTCountryID2').val($(this).data('data').ID);
                                                $('#txtCountryName').text($(this).data('data').Name);
                                            }

                                            $('#panSearchLibrary').popup('close');
                                        })
                                    ));
                            }

                        }
                        else {
                            $('#lstOfLibrary').append($('<li>').text('No Result(s) Found'));
                        }

                        $('#lstOfLibrary');//.listview('refresh');
                        Reposition();
                    }
                });
            }

            function SearchEmployeeName2(s) {
                var pData = {};
                var pLV;

                if (NameOfBtnForLibrary == "ProjectCustomer") {
                    pLV = $('#lstOfProjectCustomer');
                }
                else {
                    if (isCanvass) {
                        pLV = $('#lstOfOrgEmp');
                    }
                    else {
                        pLV = $('#lstOfLibrary');
                        pLV.empty();
                    }
                }

                pData.keywords = s;

                AjaxSend({
                    url: 'Browser/LibraryPeople',
                    data: pData,
                    success: function (vData) {

                        if (vData.Results.Rows.length > 0) {
                            for (var p = 0; p < vData.Results.Rows.length; p++) {
                                pLV.append($('<li>')
                                .append($('<a>')
                                    .attr('ID', vData.Results.Rows[p].ID)
                                    .append($('<br><p style="font-weight:bold;">')
                                        .text(vData.Results.Rows[p].PeopleName)
                                    )
                                    .data('data', vData.Results.Rows[p])
                                    .click(function (e) {
                                        var chosenID = $(this).data('data').ID;
                                        var chosenPeopleName = $(this).data('data').PeopleName;

                                        if (NameOfBtnForLibrary == "ProjectCustomer") {
                                            $('#txtClientID').val(chosenID);
                                            $('#panSearchProjectCustomer').popup('close');
                                            $('#txtClientName').text(chosenPeopleName);
                                            NameOfBtnForLibrary = '';
                                            $('#selFowstLocation').empty().append('<option>Please Select</option>');//.selectmenu('disable');//.selectmenu('refresh');
                                        }
                                        else {
                                            if (isCanvass) {
                                                $('#divVendorValidation').html('').hide();

                                                if (isCanvassMaterials) {
                                                    if (arrCanvassMaterialsOrgPeople.length > 0) {
                                                        isFoundVendor = $.grep(arrCanvassMaterialsOrgPeople, function (val) {
                                                            if (val.ID == chosenID) return true;
                                                        });
                                                        if (isFoundVendor.length > 0) {
                                                            $('#divVendorValidation').html('<br>' + $(this).data('data').PeopleName + ' already selected.<br><br>').show();
                                                            return false;
                                                        }
                                                    }

                                                    if ($(this).closest('li').children('div').hasClass('selectedList')) {
                                                        $(this).closest('li').children('div').removeClass('selectedList');
                                                        arrCanvassMaterialsOrgPeople = $.grep(arrCanvassMaterialsOrgPeople, function (val) {
                                                            return val.ID != chosenID;
                                                        })
                                                        CountVendorSelected();
                                                    }
                                                    else {
                                                        if (arrCanvassMaterialsOrgPeople.length > 9) {
                                                            $('#divVendorValidation').html('<br>Maximun of 10 Vendors only are allowed to canvass<br><br>').show();
                                                            return false;
                                                        }

                                                        $(this).closest('li').children('div').addClass('selectedList');
                                                        arrCanvassMaterialsOrgPeople.push({
                                                            "ID": chosenID,
                                                            "Name": $(this).data('data').PeopleName,
                                                            "State": "add"
                                                        })
                                                        CountVendorSelected();
                                                    }
                                                }
                                                else {
                                                    if (arrCanvassLaborOrgPeople.length > 0) {
                                                        isFoundVendor = $.grep(arrCanvassLaborOrgPeople, function (val) {
                                                            if (val.ID == chosenID) return true;
                                                        });
                                                        if (isFoundVendor.length > 0) {
                                                            $('#divVendorValidation').html('<br>' + $(this).data('data').PeopleName + ' already selected.<br><br>').show();
                                                            return false;
                                                        }
                                                    }

                                                    if ($(this).closest('li').children('div').hasClass('selectedList')) {
                                                        $(this).closest('li').children('div').removeClass('selectedList');
                                                        arrCanvassLaborOrgPeople = $.grep(arrCanvassLaborOrgPeople, function (val) {
                                                            return val.ID != chosenID;
                                                        })
                                                        CountVendorSelected();
                                                    }
                                                    else {
                                                        if (arrCanvassLaborOrgPeople.length > 9) {
                                                            $('#divVendorValidation').html('<br>Maximun of 10 Vendors only are allowed to canvass<br><br>').show();
                                                            return false;
                                                        }

                                                        $(this).closest('li').children('div').addClass('selectedList');
                                                        arrCanvassLaborOrgPeople.push({
                                                            "ID": chosenID,
                                                            "Name": $(this).data('data').PeopleName,
                                                            "State": "add"
                                                        })
                                                        CountVendorSelected();
                                                    }
                                                }
                                            }
                                            else {
                                                var isExist = false;
                                                var position;

                                                $('#lstProjectOwner li').find('a:first').each(function () {
                                                    if ($(this).attr('id') == chosenID) {
                                                        isExist = true;
                                                        position = "Project Owner";
                                                    }
                                                })

                                                $('#lstProjectManager li').find('a:first').each(function () {
                                                    if ($(this).attr('id') == chosenID) {
                                                        isExist = true;
                                                        position = "Project Manager";
                                                    }
                                                })

                                                $('#lstAsstProjectManager li').find('a:first').each(function () {
                                                    if ($(this).attr('id') == chosenID) {
                                                        isExist = true;
                                                        position = "Project Assistant Manager";
                                                    }
                                                })

                                                if (isExist) {
                                                    // $('#panSearchLibrary').popup('close');
                                                    // $('#panMessageBox').popup('open');
                                                    // $('#MsgBoxTitle').text('Project');
                                                    // $('#MsgBoxMessage').html('<strong>' + chosenPeopleName + '</strong> already a ' + position + '.');
                                                    // $('#btnOKMsgBox').unbind('click').click(function (e) {
                                                    //     e.preventDefault();
                                                    //     e.stopPropagation();
                                                    //     $('#panMessageBox').popup('close');
                                                    // })
                                                    // Reposition();
                                                    NewMessageBox('<strong>' + chosenPeopleName + '</strong> already a ' + position + '.', 'Project');
                                                    return false;
                                                }

                                                if (NameOfBtnForLibrary == "ProjectOwner") {
                                                    arrOwnerList.push({ "PeopleID": $(this).data('data').ID, "PeopleName": $(this).data('data').PeopleName, "IsProjectOwner": 1, "ProjectOwnerState": "Add" });
                                                    LoadOwnerList();
                                                }
                                                else if (NameOfBtnForLibrary == "ProjectManager") {
                                                    arrManagerList.push({ "PeopleID": $(this).data('data').ID, "PeopleName": $(this).data('data').PeopleName, "IsProjectManager": 1, "ProjectManagerState": "Add" });
                                                    LoadManagerList();
                                                }
                                                else if (NameOfBtnForLibrary == "AsstProjectManager") {
                                                    arrAsstManagerList.push({ "PeopleID": $(this).data('data').ID, "PeopleName": $(this).data('data').PeopleName, "IsProjectManager": 0, "ProjectAsstManagerState": "Add" });
                                                    LoadAsstManagerList();
                                                }

                                                $('#panSearchLibrary').popup('close');
                                            }
                                        }
                                    })
                                ));
                            }

                        }
                        else {
                            if (isNoResultSearchOrgPeople) {
                                pLV.append($('<li>').text('No Result(s) Found'));
                            }
                        }

                        pLV;//.listview('refresh');
                        Reposition();
                    }
                });
            }

            function LoadAsstManagerList() {
                var pLV = $('#lstAsstProjectManager');
                var pItem;

                pLV.html('');
                for (var i = 0; i < arrAsstManagerList.length; i++) {
                    var arrindex = i;

                    if (arrAsstManagerList[i].ProjectAsstManagerState != "Remove") {
                        pItem = $('<li id="lst' + i + '" class="list-group-item d-flex align-items-center">')
                                .append($('<p class="pr-2 m-0 flex-grow-1">')
                                        .attr('ID', arrAsstManagerList[i].PeopleID)
                                        .append($('<h3>')
                                            .attr("style", "white-space:normal")
                                            .text(arrAsstManagerList[i].PeopleName)
                                        )
                                )
                                .append($('<a>')
                                        .attr('id', arrAsstManagerList[i].PeopleID)
                                        .data('data', arrAsstManagerList[i])
                                        .click(function (e) {
                                            var id = $(this).data('data').PeopleID;

                                            //remove selected manager
                                            arrAsstManagerList[arrindex].ProjectAsstManagerState = "Remove";

                                            LoadAsstManagerList();
                                        })
                                )

                        pLV.append(pItem);
                    }
                }

                pLV;//.listview('refresh');
            }

            function LoadManagerList() {
                var pLV = $('#lstProjectManager');
                var pItem;

                pLV.html('');
                for (var i = 0; i < arrManagerList.length; i++) {
                    var arrindex = i;

                    if (arrManagerList[i].ProjectManagerState != "Remove") {
                        pItem = $('<li id="lst' + i + '" class="list-group-item d-flex align-items-center">')
                                .append($('<p class="pr-2 m-0 flex-grow-1">')
                                        .attr('ID', arrManagerList[i].PeopleID)
                                        .append($('<h3>')
                                            .attr("style", "white-space:normal")
                                            .text(arrManagerList[i].PeopleName)
                                        )
                                )
                                .append($('<a>')
                                        .attr('id', arrManagerList[i].PeopleID)
                                        .data('data', arrManagerList[i])
                                        .click(function (e) {
                                            var id = $(this).data('data').PeopleID;

                                            //remove selected manager
                                            arrManagerList[arrindex].ProjectManagerState = "Remove";

                                            LoadManagerList();
                                        })
                                )

                        pLV.append(pItem);
                    }
                }

                pLV;//.listview('refresh');
            }

            function LoadOwnerList() {
                var pLV = $('#lstProjectOwner');
                var pItem;

                pLV.html('');
                for (var i = 0; i < arrOwnerList.length; i++) {
                    var arrindex = i;

                    if (arrOwnerList[i].ProjectOwnerState != "Remove") {
                        pItem = $('<li id="lst' + i + '" class="list-group-item d-flex align-items-center">')
                                .append($('<p class="pr-2 m-0 flex-grow-1">')
                                        .attr('ID', arrOwnerList[i].PeopleID)
                                        .append($('<h3>')
                                            .attr("style", "white-space:normal")
                                            .text(arrOwnerList[i].PeopleName)
                                        )
                                )
                                .append($('<a>')
                                        .attr('id', arrOwnerList[i].PeopleID)
                                        .data('data', arrOwnerList[i])
                                        .click(function (e) {
                                            var id = $(this).data('data').PeopleID;

                                            arrOwnerList[arrindex].ProjectOwnerState = "Remove";

                                            LoadOwnerList();
                                        })
                                )

                        pLV.append(pItem);
                    }
                }

                pLV;//.listview('refresh');
            }

            function GetCityListAll() {
                AjaxSend({
                    url: 'City/getCityListAll',
                    success: function (vData) {
                        $('#selOrgCity').empty();
                        $('#selOrgCity').append('<option value="">Please Select City</option>');

                        if (vData.CityData.Rows.length > 0) {
                            for (var p = 0; p < vData.CityData.Rows.length; p++) {
                                $('#selOrgCity').append($('<option>', {
                                    value: vData.CityData.Rows[p].ID,
                                    text: vData.CityData.Rows[p].City
                                }))
                            }
                        }
                        else {

                        }

                        $('#selOrgCity');//.selectmenu('refresh');
                    },
                    error: function (vError) {
                        console.log(vError);
                    }
                });
            }

            function GetCountryListAll() {
                AjaxSend({
                    url: 'Country/getCountryListAll',
                    success: function (vData) {
                        $('#selOrgCountry').empty();
                        $('#selOrgCountry').append('<option value="">Please Select Country</option>');

                        if (vData.CountryData.Rows.length > 0) {
                            for (var p = 0; p < vData.CountryData.Rows.length; p++) {
                                $('#selOrgCountry').append($('<option>', {
                                    value: vData.CountryData.Rows[p].ID,
                                    text: vData.CountryData.Rows[p].Country
                                }))
                            }
                        }
                        else {

                        }
                        $("#selOrgCountry option:contains('Philippines')").attr('selected', 'selected');
                        $('#selOrgCountry');//.selectmenu('refresh');

                    },
                    error: function (vError) {
                        console.log(vError);
                    }
                });
            }

            function TempSaveEditedCell(This) {
                var id = $(This).attr('id').split('-')[1];

                if ($(This).attr('columnName') == "qty") {
                    if ($.trim($(This).val()) != "") {
                        if ($.trim($(This).closest('tr').find('td:eq(5)').find('div').text()) != "" && $.trim($(This).closest('tr').find('td:eq(7)').find('div').text()) == "") {
                            $(This).closest('tr').find('td:eq(6)').find('div').text(AccountingFormat($.format.number(CDec2($(This).val()) * CDec2($.trim($(This).closest('tr').find('td:eq(5)').find('div').text())))));
                            $(This).closest('tr').find('td:eq(10)').find('div').text(AccountingFormat($.format.number($.trim($(This).closest('tr').find('td:eq(6)').find('div').text()))));
                        }
                        else if ($.trim($(This).closest('tr').find('td:eq(5)').find('div').text()) == "" && $.trim($(This).closest('tr').find('td:eq(7)').find('div').text()) != "") {
                            $(This).closest('tr').find('td:eq(8)').find('div').text(AccountingFormat($.format.number(CDec2($(This).val()) * CDec2($.trim($(This).closest('tr').find('td:eq(7)').find('div').text())))));
                            $(This).closest('tr').find('td:eq(10)').find('div').text(AccountingFormat($.format.number($.trim($(This).closest('tr').find('td:eq(8)').find('div').text()))));
                        }
                        else {
                            $(This).closest('tr').find('td:eq(6)').find('div').text(AccountingFormat($.format.number(CDec2($(This).val()) * CDec2($.trim($(This).closest('tr').find('td:eq(5)').find('div').text())))));
                            $(This).closest('tr').find('td:eq(8)').find('div').text(AccountingFormat($.format.number(CDec2($(This).val()) * CDec2($.trim($(This).closest('tr').find('td:eq(7)').find('div').text())))));
                            $(This).closest('tr').find('td:eq(10)').find('div').text(AccountingFormat($.format.number(CDec2($.trim($(This).closest('tr').find('td:eq(6)').find('div').text())) + CDec2($.trim($(This).closest('tr').find('td:eq(8)').find('div').text())))));
                        }
                    }
                }

                SaveCellData(This);

                $(This).replaceWith($('<div>')
                            .attr('id', id)
                            .attr('isRequired', $(This).attr('isRequired'))
                            .attr('data-role', 'none')
                            .attr('columnName', $(This).attr('columnName'))
                            .attr('datatype', $(This).attr('datatype'))
                            .attr('state', $(This).attr('state'))
                            .attr('indentno', $(This).attr('indentno'))
                            .data('data', $(This).data('data'))
                            .css({ width: '100%' })
                            .html($(This).val())
                            .addClass('NumericCell')
                        );
                //$('#btnComputeParentSubTotal').click();
            }

            function validateNumber(number) {
                var re = /^[0-9]{7}$/;
                return re.test(number);
            }

            function SaveProjectBudget(vData) {
                var pData = {};

                pData.ProjectBudget = vData;
                pData.ProjectCanvassMaterials = arrCanvassMaterials;
                pData.ProjectCanvassLabor = arrCanvassLabor;
                pData.ProjectTimeline = arrTimeline;
                pData.ProjectID = sessionStorage.ProjectID;

                $('#btnSaveProjectBudget').attr('disabled', 'disabled');
                Bs.Loading('show');
                        
                projectData.delete('data');
                projectData.append('data', JSON.stringify(pData))

                //return
                AjaxSend({
                    url: 'BOMProjectExcel/SaveProjectBudget?OrgID=' + sessionStorage.OrgID,
                    data: projectData, //pData,
                    async: true,
                    cache: false,
                    contentType: false,
                    processData: false,
                    dataType: 'json',
                    success: function (vResult) {
                        $('#btnSaveProjectBudget').removeAttr('disabled');
                        Bs.Loading('hide');
                        if (vResult.ProjectBudget == "saved") {
                            WebToast('Successfully Saved', 'top');
                            $(window).scrollTop(0);
                            $('#divViewProjectBudget').show();
                            $('.divNewProjectBudget, #divButtons').hide();
                            $('#btnSaveProjectBudget').hide();
                            $('#divBtnNewProjectBudget').show();                                    
                            isCreateNewProjectBudget = true;
                            isCanvassMaterialsAlready = false;
                            isCanvassLaborAlready = false;
                            arrCanvassMaterials = [];
                            arrCanvassLabor = [];
                            isFinishLoadCanvassMaterials = false;
                            isFinishLoadCanvassLabor = false;
                            isFinishedLoadTimeline = false;
                            GetProjectBudget(sessionStorage.ProjectID);
                        }
                        else {
                            NewMessageBox('ERROR: ' + vResult.ProjectBudget);
                        }
                    }
                })
            }

            function SaveNewProject() {
                var pData = {};
                var arrProjectMemberRoleDetails = [];

                $('#tbodyProjectMember tr').each(function () {
                    arrProjectMemberRoleDetails.push({
                        ID: ($(this).attr('id') == undefined ? '' : $(this).attr('id')),
                        MemberID: $(this).attr('pplid'),
                        MemberRoleID: $(this).find('td:eq(1) select option:selected').val(),
                        IsActive: $(this).find('td:eq(2) select option:selected').val(),
                        State: ($(this).attr('state') || "")
                    })
                })

                pData.OrgID = sessionStorage.OrgID;
                pData.FowstID = $('#txtProjectID').val(); //ProjectID aka FowstID for Mobiz Project only
                pData.ProjectTypeID = $('#selFowstType').val(); //PROJECT - Default Type for Mobiz Project AKA Excel Like
                pData.PreTypeFowstName = $('#txtFowstLocation').val();
                pData.PreTypeFowstNameID = $('#txtFowstLocationID').val();
                pData.ProjectName = $('#txtProjectName').val();
                pData.ProjectCode = $('#txtProjectCode').val();
                pData.ProjectDescription = $('#txtProjectDescription').val();
                pData.ProjectBusinessNameID = $('#txtBusinessName').val();
                pData.ProjectOwnerOrgPersonID = $('#txtProjectOwnerOrgPersonID').val();
                pData.ClientID = $('#txtClientID').val();
                pData.BranchFowstID = $('#selFowstLocation').val();

                pData.ProjectHouseNo = $('#txtOrgFOWSTBuildingNo2').val();
                pData.ProjectStreet = $('#txtOrgFOWSTStreet2').val();
                pData.ProjectCityID = $('#txtOrgFOWSTCityID2').val();
                pData.ProjectCountryID = $('#txtOrgFOWSTCountryID2').val();

                pData.JobRolePermission = arrRolePermissions;
                pData.JobRole = arrDefineRole;
                pData.ProjectMember = arrProjectMemberRoleDetails;

                pData.ProjectOwner = arrOwnerList;
                pData.ProjectManager = arrManagerList;
                pData.ProjectAssistantManager = arrAsstManagerList;

                pData.ProjectedStartDate = $('#ProjStartDate').text();
                pData.ActualStartDate = $('#ActualStartDate').text();
                pData.ProjectedCompletionDate = $('#ProjCompDate').text();
                pData.ActualCompletionDate = $('#ActualCompDate').text();
                pData.DaysProjected = $('#txtProjectedTotalNumberofDays').text();
                pData.DaysActual = $('#txtActualTotalNumberofDays').text();

                pData.ProjectTelephoneNumber = arrTelInfoList;
                pData.ProjectMobileNumber = arrMobileInfoList;
                pData.ProjectEmailAddress = arrEmailInfoList;

                // console.log(pData);
                // return;
                $('#btnSaveNewProject').attr('disabled', 'disabled');
                Bs.Loading('show', 'Validating your data ...')

                AjaxSend({
                    url: 'BOMProjectExcel/CheckProjectName',
                    data: pData,
                    success: function (vProjectName) {
                        if (vProjectName.ProjectName.Rows.length > 0) {
                            if (pData.FowstID == "") {
                                Bs.Loading('hide');
                                $('#btnSaveNewProject').removeAttr('disabled');
                                NewMessageBox('Project Name already exist.', 'Project', [
                                        {
                                            Text: 'Ok',
                                            Click: function () {
                                                $(window).scrollTop(0);
                                            }
                                        }
                                ])
                                return false;
                            }
                        }

                        AjaxSend({
                            url: 'BOMProjectExcel/CheckProjectCode',
                            data: pData,
                            success: function (vProjectName) {
                                if (vProjectName.ProjectCode.Rows.length > 0) {
                                    if (pData.FowstID == "") {
                                        Bs.Loading('hide');
                                        $('#btnSaveNewProject').removeAttr('disabled');
                                        NewMessageBox('Project Code already exist.', 'Project', [
                                                {
                                                    Text: 'Ok',
                                                    Click: function () {
                                                        $(window).scrollTop(0);
                                                    }
                                                }
                                        ])
                                        return false;
                                    }
                                }

                                Bs.Loading('show', 'Saving your data ...')

                                AjaxSend({
                                    url: 'BOMProjectExcel/SaveProjectHeader',
                                    data: pData,
                                    success: function (vData) {
                                        Bs.Loading('hide');
                                        $('#btnSaveNewProject').removeAttr('disabled');
                                        if (IsObject(JSON.parse(vData.ProjectHeader))) {
                                            WebToast('Successfully saved', 'top');
                                            $('#popupNewProject').modal('hide');
                                            if (isAddProject) {
                                                //if (location.pathname.toLowerCase() != '/erp') {
                                                //    $('.ItemsBreadCrumbs').text('Projects');
                                                //}
                                                //$('#popupNewProject').modal('hide');
                                                ViewProject(JSON.parse(vData.ProjectHeader).d);
                                                GetProjectHeader();
                                            }
                                            else {
                                                //if (location.pathname.toLowerCase() != '/erp') {
                                                //    $('.ItemsBreadCrumbs').text('');
                                                //    breadcrumbs.prepend(arrowR, "Project Budget");
                                                //    $('.BreadCrumbs').html(breadcrumbs.prepend(lnkProject));
                                                //}
                                                        
                                                GetProjectHeader(sessionStorage.ProjectID, 'View');
                                                GetAccessRights();
                                            }

                                            ClearAllVariables();
                                            ClearAllFields();
                                        }
                                        else {
                                            NewMessageBox('ERROR: ' + vData.ProjectHeader, 'Project');
                                        }
                                    }
                                })
                            }
                        })
                    }
                })
            }

            function SaveNewProjectCustomer() {

                $('#btnCustomerNameSave').text('Saving ...').attr('disabled', 'disabled');
                if ($('#selCustomerType').val() == "Organization") {
                    var str = $.trim($('#txtProjectCustomerCompanyName').val())
                    str = str.toLowerCase().replace(/\b[a-z]/g, function (letter) {
                        return letter.toUpperCase();
                    });

                    var pDataTradeName = [];
                    var pDataCategory = [];
                    var pDataOrg = {
                        IsOrganization: 1,
                        OrganizationID: '',
                        PersonTitleID: '',
                        PersonDOB: '',
                        CompanyName: str,
                        TIN: $.trim($('#txtProjectCustomerTIN').val()),
                        OrgTradeNameList: pDataTradeName,
                        OrgCategoryList: pDataCategory,
                        IsAddAsCustomer: 1,
                        CompanyNameInitial: '',
                        IsFranchisor: 0,
                        OfficeAddressBuildingNo: $.trim($('#txtCustomerOrgHouseNo').val()),
                        OfficeAddressStreet: $.trim($('#txtCustomerOrgAddress').val()), //temporary store the free type address here
                        OfficeAddressCityID: $('#selOrgCity').val(),
                        OfficeAddressCountryID: $('#selOrgCountry').val(),
                        BusinessAddressBuildingNo: $.trim($('#txtCustomerOrgHouseNo').val()),
                        BusinessAddressStreet: $.trim($('#txtCustomerOrgAddress').val()),
                        BusinessAddressCityID: $('#selOrgCity').val(),
                        BusinessAddressCountryID: $('#selOrgCountry').val()
                    }

                    AjaxSend({
                        url: "OrganizationShort/CheckOrgUniqueOrgName",
                        data: {
                            OrganizationID: '',
                            CompanyName: str
                        },
                        success: function (vData) {

                            if (vData.OrganizationData.Rows.length > 0) {
                                NewMessageBox('Organization with the same corporate name already exists.', 'Organization Info');
                                $('#btnCustomerNameSave').text('Save').removeAttr('disabled');
                            } else {
                                AjaxSend({
                                    url: 'OrganizationShort/SaveOrganizationInfo',
                                    data: pDataOrg,
                                    success: function (vSaveData) {
                                        WebToast('Organization Info successfully saved', 'top');
                                        if (isCanvassMaterials == true || isCanvassMaterials == false) {
                                            $('#pageProjectBudget').pageshow();
                                            $('#pageProjectDetails').hide();
                                            $('#pageProjectBudget, .divCanvass').show();
                                        }
                                        else {
                                            $('#popupNewProjectCustomer').modal('hide');
                                            $('#panMessageBox').popup('close');
                                            $('#txtClientID').val(vSaveData.OrganizationData);
                                            $('#txtClientName').text(str);
                                        }
                                        $('#btnCustomerNameSave').text('Save').removeAttr('disabled');
                                    }
                                });
                            }
                        }
                    });
                }
                else {
                    var firstname = $.trim($('#txtProjectCustomerFirstName').val())
                    firstname = firstname.toLowerCase().replace(/\b[a-z]/g, function (letter) {
                        return letter.toUpperCase();
                    });
                    var middlename = $.trim($('#txtProjectCustomerMiddleName').val())
                    middlename = middlename.toLowerCase().replace(/\b[a-z]/g, function (letter) {
                        return letter.toUpperCase();
                    });
                    var surname = $.trim($('#txtProjectCustomerSurname').val())
                    surname = surname.toLowerCase().replace(/\b[a-z]/g, function (letter) {
                        return letter.toUpperCase();
                    });

                    var pDataTradeName = [];
                    var pDataCategory = [];
                    var pDataPerson = {
                        IsOrganization: 0,
                        OrganizationID: '',
                        PersonFirstName: firstname,
                        PersonMiddleName: middlename,
                        PersonSurname: surname,
                        PersonSuffixName: '',
                        PersonTitleID: '',
                        PersonDOB: '',
                        PersonMotherMaidenSurname: '',
                        PersonMotherMaidenFirstName: '',
                        PersonMotherMaidenMiddleName: '',
                        OrgTradeNameList: pDataTradeName,
                        OrgCategoryList: pDataCategory,
                        IsAddAsCustomer: 1,
                        CompanyNameInitial: '',
                        IsFranchisor: 0,
                        TIN: $.trim($('#txtProjectCustomerPersonTIN').val()),
                        OfficeAddressBuildingNo: '',
                        OfficeAddressStreet: '',
                        OfficeAddressCityID: '',
                        OfficeAddressCountryID: '',
                        BusinessAddressBuildingNo: '',
                        BusinessAddressStreet: '',
                        BusinessAddressCityID: '',
                        BusinessAddressCountryID: ''
                    };

                    AjaxSend({
                        url: "OrganizationShort/CheckPersonUniqueName",
                        data: {
                            IsOrganization: 0,
                            OrganizationID: '',
                            PersonFirstName: firstname,
                            PersonMiddleName: middlename,
                            PersonSurname: surname,
                            PersonSuffixName: '',
                            PersonTitleID: '',
                            PersonDOB: '',
                            PersonMotherMaidenSurname: '',
                            PersonMotherMaidenFirstName: '',
                            PersonMotherMaidenMiddleName: '',
                            IsFull: false

                        },
                        success: function (vData) {

                            if (vData.OrganizationData.Rows.length > 0) {
                                NewMessageBox('Person with the same information already exists.', 'Person Info');
                            } else {
                                AjaxSend({
                                    url: 'OrganizationShort/SaveOrganizationInfo',
                                    data: pDataPerson,
                                    success: function (vSaveData) {
                                        WebToast('Person Info successfully saved', 'top');

                                        if (isCanvassMaterials == true || isCanvassMaterials == false) {
                                            $('#pageProjectBudget').pageshow();
                                            $('#pageProjectDetails').hide();
                                            $('#pageProjectBudget, .divCanvass').show();
                                        }
                                        else {
                                            $('#popupNewProjectCustomer').modal('hide');
                                            $('#panMessageBox').popup('close');
                                            $('#txtClientID').val(vSaveData.OrganizationData);
                                            $('#txtClientName').text(firstname + ' ' + middlename + ' ' + surname);
                                        }
                                        $('#btnCustomerNameSave').text('Save').removeAttr('disabled');
                                    }
                                });
                            }
                        }
                    });
                }
            }

            function GetProjectBudgetTimeline() {
                var pData = {};

                pData.ProjectID = sessionStorage.ProjectID;
                pData.OrgID = sessionStorage.OrgID;

                AjaxSend({
                    url: 'BOMProjectExcel/GetProjectBudgetTimeline',
                    data: pData,
                    success: function (vData) {
                        if (vData.Timeline.Rows.length > 0) {
                            for (var i = 0; i < vData.Timeline.Rows.length; i++) {
                                var currentRow = $('#tBodyTimeline tr[descriptionrowid="' + vData.Timeline.Rows[i].DescriptionID + '"]').attr('tlrowid', vData.Timeline.Rows[i].ID);

                                currentRow.find('td').eq(2).find('select').val(vData.Timeline.Rows[i].Importance);
                                currentRow.find('td').eq(3).find('select').val(vData.Timeline.Rows[i].Urgency);
                                currentRow.find('td').eq(4).find('button').text(vData.Timeline.Rows[i].TimeToComplete);//.button('refresh');
                                currentRow.find('td').eq(5).find('button').text(vData.Timeline.Rows[i].AssignedTo);//.button('refresh');

                                if (vData.Timeline.Rows[i].AssignedToPeopleID != undefined) {
                                    currentRow.find('td').eq(5).find('button').attr('pplid', vData.Timeline.Rows[i].AssignedToPeopleID);//.button('refresh');
                                }

                                currentRow.find('td').eq(6).find('select').val(vData.Timeline.Rows[i].Status);
                                currentRow.find('td').eq(7).find('select').val(vData.Timeline.Rows[i].Dependency);
                                currentRow.find('td').eq(8).find('div').html((vData.Timeline.Rows[i].Tags == "" ? "&nbsp;" : vData.Timeline.Rows[i].Tags));
                            }
                        }
                        else {

                        }
                    }
                })
            }

            function GetProjectBudget(ProjectID) {
                var pData = {}, tbody = (isCreateNewProjectBudget ? $('#tBodyProjectBudget') : $('#tBodyExcelLike'));

                pData.ProjectID = ProjectID;
                pData.OrgID = sessionStorage.OrgID;

                tbody.empty().append($('<tr>')
                                                .append($('<td>')
                                                        .attr('colspan', 10)
                                                        .css({ textAlign: 'center' })
                                                        .text('Please wait while getting all data')
                                                    )
                )

                AjaxSend({
                    url: 'BOMProjectExcel/GetProjectBudget',
                    data: pData,
                    success: function (vData) {
                        var vRows = vData.data;
                        arrProjectBudget = vRows;
                        tbody.empty();

                        if (vRows.length > 0) {
                            var ctrParent = 0;

                            for (var i = 0; i < vRows.length; i++) {
                                if (vRows[i].LineDescription.toLowerCase().indexOf("subtotal") == 0) {
                                    continue;
                                }

                                var parentFontWeigth = vRows[i].IsParent == 1 ? "bold !important" : "normal";
                                var textTransform = vRows[i].IsParent == 1 ? "uppercase" : "capitalize";
                                var noOfIndent = vRows[i].ParentID == "" ? "0px" : NumberOfIndent(vRows[i].ParentID);
                                var isDisableCell = vRows[i].Status == "Active" ? "" : "disableCell";
                                var attrs = { 'style': 'font-weight:' + parentFontWeigth };
                                var hasAssociatedSKU = {};
                                var isViewOnly;
                                var preAssociatedSKU = '';
                                var actualduration = '';
                                var totalreqqty = 0, totalusedupqty = 0, totalreqadvances = 0;

                                if (CStr2(vRows[i].DateStarted) != '' && CStr2(vRows[i].DateCompleted) != '') {
                                    actualduration = Math.abs(new Date(vRows[i].DateStarted) - new Date(vRows[i].DateCompleted)) / (1000 * 60 * 60 * 24) + ' day(s)';
                                }

                                if (vRows[i].SKUName != undefined) {
                                    //attrs['skuname'] = 'SKU Name: ' + vRows[i].SKUName;
                                    attrs['title'] = 'SKU Name: ' + vRows[i].SKUName;
                                    attrs['data-toggle'] = 'tooltip';
                                    attrs['data-placement'] = 'bottom';
                                    attrs['associateskuid'] = vRows[i].AssociateSKUID;
                                    hasAssociatedSKU['class'] = "hasAssociatedSKU";
                                    preAssociatedSKU = '<pre class="preAssociatedSKU">' + vRows[i].SKUName + '</pre>';
                                }

                                $.grep(vRows[i].AccReqQty, function (e) {
                                    if(e.Qty != 'cancelled'){
                                        totalreqqty += e.Qty
                                    }                                    
                                })

                                $.grep(vRows[i].AccUsedUpQty, function (e) {
                                    totalusedupqty += e.Qty;
                                })

                                $.grep(vRows[i].RequestedAdvances, function (e) {
                                    if(e.State != 'cancelled'){
                                        totalreqadvances += e.Amount;
                                    }                                    
                                })

                                if (isCreateNewProjectBudget == false) {
                                    isViewOnly = 0;
                                    $('#tBodyExcelLike').append($('<tr descriptionrowid="' + vRows[i].ID + '" id="' + vRows[i].RowNumber + '">')
                                            .addClass((vRows[i].LineDescription.toLowerCase().indexOf("subtotal") == 0 ? ' rowSubtotal ' : '') + (CBool(vRows[i].IsMaterial) ? ' rowMaterials ' : ''))
                                            .attr('rowType', (vRows[i].LineDescription.toLowerCase().indexOf("subtotal") == 0 ? 'subtotal' : 'normal'))
                                            .append(
                                                $('<td class="tddraggable">')
                                                .append($('<span class="bi bi-grip-vertical">'))
                                                .css({
                                                    backgroundColor: '#fff'
                                                })
                                            )
                                            .append(
                                                $('<td>')
                                                .append((vRows[i].IsParent == 1 ?
                                                            '<div id="txtItemNumber_' + ctrParent + '" columnName="item" class="tableCellReadOnly ' + isDisableCell + '" state="edit" style="width:100%;">' + vRows[i].ItemNumber + '</div>'
                                                        :
                                                            '<div state="edit" class="' + isDisableCell + '" style="width:100%;">' + vRows[i].ItemNumber + '</div>'
                                                        )
                                                )
                                                .css({
                                                    width: '85px'
                                                })
                                                .addClass('tableCellDisable tdfreeze tditemno')
                                            )
                                            .append(
                                                $('<td>')
                                                .attr(hasAssociatedSKU)
                                                .append((vRows[i].IsParent == 1 ?
                                                            $('<div>')
                                                                .attr('id', 'txtDescription_' + ctrParent)
                                                                .attr(attrs)
                                                                .attr('columnName', 'description')
                                                                .attr('data-role', 'none')
                                                                .attr('isrequired', 1)
                                                                .attr('itemid', vRows[i].ItemNumber)
                                                                .attr('descriptionid', vRows[i].ItemNumber)
                                                                .attr('indentno', vRows[i].IndentNo)
                                                                .attr('isParent', 1)
                                                                .attr('state', 'edit')
                                                                .addClass('txtOnlyParent')
                                                                .addClass(isDisableCell)
                                                                .text(vRows[i].LineDescription)
                                                        :
                                                            $('<div>')
                                                                .attr('id', 'txtDescription_' + ctrParent)
                                                                .attr(attrs)
                                                                .attr('columnName', 'description')
                                                                .attr('data-role', 'none')
                                                                .attr('isrequired', 1)
                                                                .attr('parentid', vRows[i].ParentID)
                                                                .attr('descriptionid', vRows[i].ItemNumber)
                                                                .attr('indentno', vRows[i].IndentNo)
                                                                .attr('isParent', 0)
                                                                .attr('state', 'edit')
                                                                //.css({ paddingLeft: NumberOfIndent(vRows[i].ParentID) })
                                                                .addClass('txtOnlyChild')
                                                                .addClass(isDisableCell + (CBool(vRows[i].IsMaterial) ? ' rowMaterials ' : ''))
                                                                .text(vRows[i].LineDescription)
                                                        )
                                                )
                                                .addClass('tableCell tddescription tdfreeze ' + (CBool(vRows[i].IsMaterial) ? 'rowMaterials ' : ' ') + (CBool(vRows[i].IsMarkAsCompleted) ? 'markascompleted ' : ' '))
                                                .append(preAssociatedSKU)
                                                .css({
                                                    //paddingRight: 0,
                                                    //paddingLeft: noOfIndent,
                                                    textTransform: textTransform,
                                                    //position: 'relative',
                                                    width: (316 - parseInt(noOfIndent)) + 'px',
                                                    left: '120px',
                                                    backgroundColor: '#fff'
                                                })
                                            )
                                            .append( // Point Person
                                                $('<td>')
                                                .append($('<div state="edit" data-role="none" isRequired="0">')
                                                        .addClass((vRows[i].IsParent == 1 ? 'tableCellReadOnly' : 'txtOnlyChild ' + isDisableCell + ' tableCellReadOnly'))
                                                        .data('data', [])
                                                        .append(PointPerson(CStr2(vRows[i].PointPersonID)))
                                                )
                                                .addClass('tableCell tdpointperson')
                                            )
                                            .append( //REMARKS
                                                $('<td>')
                                                .append(
                                                    $('<div state="edit" data-role="none" isRequired="0">')
                                                        .addClass((vRows[i].IsParent == 1 ? 'tableCellReadOnly' : 'txtOnlyChild ' + isDisableCell + ' tableCellReadOnly'))
                                                        .html((vRows[i].Remarks.length > 0 ? HtmlToPlainText(HtmlDecode(vRows[i].Remarks[vRows[i].Remarks.length - 1].Remarks)).substring(0, 50) + ' ...' : ''))
                                                        .data('data', vRows[i].Remarks)
                                                        .data('files', [])
                                                        .data('newphotosformdata', [])
                                                )
                                                .addClass('tableCell tdremarks')
                                                .addClass('tableCellReadOnly')
                                            )
                                            .append( //COMPLETION RATE
                                                $('<td>')
                                                .css({
                                                    'text-align': 'right',
                                                    'position': 'relative'
                                                })
                                                //.append((vRows[i].IsParent == 1 ?
                                                //            '<div vid="' + vRows[i].MaterialVendor + '" state="edit" id="txtMaterialUnitCost_' + ctrParent + '" class="NumericCell ' + isDisableCell + '" columnName="m_unitcost" data-role="none" isRequired="0">' + (vRows[i].MaterialUnitCost == 0 ? "-" : AccountingFormat($.format.number(vRows[i].MaterialUnitCost))) + '</div>'
                                                //        :
                                                //            '<div vid="' + vRows[i].MaterialVendor + '" state="edit" class="txtOnlyChild NumericCell ' + isDisableCell + '" data-role="none" isRequired="0">' + (vRows[i].MaterialUnitCost == 0 ? "-" : AccountingFormat($.format.number(vRows[i].MaterialUnitCost))) + '</div>'
                                                //        )
                                                //)
                                                .append((vRows[i].IsParent == 1 ?
                                                            '<div state="edit" id="txtMaterialUnitCost_' + ctrParent + '" class="NumericCell ' + isDisableCell + '" columnName="m_unitcost" data-role="none" isRequired="0"></div>'
                                                        :
                                                            '<div state="edit" class="txtOnlyChild NumericCell ' + isDisableCell + '" data-role="none" isRequired="0"></div>'
                                                        )
                                                )
                                                .addClass('tableCell tdcompletionrate cell-timespend')
                                                .addClass((CBool(vRows[i].IsMaterial) ? 'tableCellDisable' : ''))
                                                //.addClass('btnCanvassMaterials')
                                            )
                                            .append( // DEPENDENCY
                                                $('<td>')
                                                //.append((vRows[i].IsParent == 1 ?
                                                //            '<div state="edit" id="txtMaterialAmount_' + ctrParent + '" class="NumericCell ' + isDisableCell + '" columnName="m_amount" data-role="none" isRequired="0">' + (vRows[i].MaterialAmount == "0" ? "-" : AccountingFormat($.format.number(vRows[i].MaterialAmount))) + '</div>'
                                                //        :
                                                //            '<div state="edit" class="txtOnlyChild NumericCell ' + isDisableCell + '" data-role="none" isRequired="0">' + (vRows[i].MaterialAmount == "0" ? "-" : AccountingFormat($.format.number(vRows[i].MaterialAmount))) + '</div>'
                                                //        )
                                                //)
                                                .append($('<div state="edit" data-role="none" isRequired="0">')
                                                        .attr('id', (vRows[i].IsParent == 1 ? 'txtMaterialAmount_' + ctrParent : ''))
                                                        .addClass((vRows[i].IsParent == 1 ? ' ' : ' txtOnlyChild ') + isDisableCell + ' tableCellReadOnly')
                                                        .data('data', vRows[i])
                                                        .html(CStr2(vRows[i].Dependency).split(',').join(', '))
                                                        //.append(TaskDependency(CBool(vRows[i].IsMaterial), vRows[i].Dependency))
                                                )
                                                .addClass('tableCell cell-timespend tddependency')
                                                .addClass((CBool(vRows[i].IsMaterial) ? 'tableCellDisable' : 'tableCellReadOnly'))
                                                .data('data', vRows[i])
                                                //.addClass('btnCanvassMaterials')
                                                .click(function (e) {
                                                    var data = $(this).data('data');
                                                    setTimeout(function () { //dapat mauna ang activeCell indicator
                                                        if (!IsShowPopupWhenClick()) {
                                                            return;
                                                        }
                                                        ModifyTaskDependency(data);
                                                    }, 10)
                                                })
                                            )
                                            .append( // BUDGET DURATION
                                                $('<td>')
                                                .css({
                                                    'text-align': 'center',
                                                    'position': 'relative'
                                                })
                                                //.append((vRows[i].IsParent == 1 ?
                                                //            '<div vid="' + vRows[i].LaborOrgPeople + '" state="edit" id="txtLaborUnitCost_' + ctrParent + '" class="NumericCell ' + isDisableCell + '" columnName="l_unitcost" data-role="none" isRequired="0">' + (vRows[i].LaborUnitCost == 0 ? "-" : AccountingFormat($.format.number(vRows[i].LaborUnitCost))) + '</div>'
                                                //        :
                                                //            '<div vid="' + vRows[i].LaborOrgPeople + '" state="edit" class="txtOnlyChild NumericCell ' + isDisableCell + '" data-role="none" isRequired="0">' + (vRows[i].LaborUnitCost == 0 ? "-" : AccountingFormat($.format.number(vRows[i].LaborUnitCost))) + '</div>'
                                                //        )
                                                //)
                                                .append($('<div state="edit" data-role="none" isRequired="0">')
                                                        .attr('id', (vRows[i].IsParent == 1 ? 'txtLaborUnitCost_' + ctrParent : ''))
                                                        .addClass((vRows[i].IsParent == 1 ? ' ' : ' txtOnlyChild ') + isDisableCell + ' tableCellReadOnly')
                                                        .data('data', vRows[i].BudgetedDuration)
                                                        .text((vRows[i].BudgetedDuration.length > 0 ? vRows[i].BudgetedDuration[vRows[i].BudgetedDuration.length - 1].Duration + ' ' + vRows[i].BudgetedDuration[vRows[i].BudgetedDuration.length - 1].StateIn : ''))
                                                )
                                                .addClass('tableCell cell-timespend tdbudgetedduration')
                                                .addClass((CBool(vRows[i].IsMaterial) ? 'tableCellDisable' : ''))
                                                //.addClass('btnCanvassLabor')
                                                .click(function (e) {
                                                    if (!IsShowPopupWhenClick()) {
                                                        return;
                                                    }

                                                    ProjectDuration($(this));
                                                })
                                            )
                                            .append( // ACTUAL DURATION
                                                $('<td>')
                                                //.append((vRows[i].IsParent == 1 ?
                                                //            '<div state="edit" id="txtLaborAmount' + ctrParent + '" class="NumericCell ' + isDisableCell + '" columnName="l_amount" data-role="none" isRequired="0">' + (vRows[i].LaborAmount == "0" ? "-" : AccountingFormat($.format.number(vRows[i].LaborAmount))) + '</div>'
                                                //        :
                                                //            '<div state="edit" class="txtOnlyChild NumericCell ' + isDisableCell + '" data-role="none" isRequired="0">' + (vRows[i].LaborAmount == "0" ? "-" : AccountingFormat($.format.number(vRows[i].LaborAmount))) + '</div>'
                                                //        )
                                                //)
                                                .append($('<div state="edit" data-role="none" isRequired="0">')
                                                        .attr('id', (vRows[i].IsParent == 1 ? 'txtLaborAmount' + ctrParent : ''))
                                                        .addClass((vRows[i].IsParent == 1 ? ' ' : ' txtOnlyChild ') + isDisableCell + ' tableCellReadOnly')
                                                        .data('data', [])
                                                        .text(actualduration)
                                                )
                                                .addClass('tableCell tdactualduration cell-timespend')
                                                .addClass((CBool(vRows[i].IsMaterial) ? 'tableCellDisable' : ''))
                                                //.addClass('btnCanvassLabor')
                                            )
                                            .append( // DATE / TIME STARTED
                                                $('<td>')
                                                .css({
                                                    'text-align': 'center',
                                                    'position': 'relative'
                                                })
                                                //.append((vRows[i].IsParent == 1 ?
                                                //            '<div state="edit" id="txtTotalUnitCost_' + ctrParent + '" class="NumericCell ' + isDisableCell + '" columnName="t_unitcost" data-role="none" isRequired="0">' + (vRows[i].TotalUnitCost == 0 ? "-" : AccountingFormat($.format.number(vRows[i].TotalUnitCost))) + '</div>'
                                                //        :
                                                //            '<div state="edit" class="txtOnlyChild NumericCell ' + isDisableCell + '" data-role="none" isRequired="0">' + (vRows[i].TotalUnitCost == 0 ? "-" : AccountingFormat($.format.number(vRows[i].TotalUnitCost))) + '</div>'
                                                //        )
                                                //)
                                                .append($('<div state="edit" data-role="none" isRequired="0">')
                                                        .attr('id', (vRows[i].IsParent == 1 ? 'txtTotalUnitCost_' + ctrParent : ''))
                                                        .addClass((vRows[i].IsParent == 1 ? ' ' : ' txtOnlyChild ') + isDisableCell + ' tableCellReadOnly')
                                                        .data('data', CStr2(vRows[i].DateStarted))
                                                        .text((CStr2(vRows[i].DateStarted) == '' ? '' : $.format.date(new Date(vRows[i].DateStarted), 'MMM dd, yyyy hh:mm a')))
                                                )
                                                .data('data', vRows[i].DateStarted)
                                                .addClass('tableCell cell-timespend tddatestarted')
                                                .addClass((CBool(vRows[i].IsMaterial) ? 'tableCellDisable' : ''))
                                                .click(function (e) {
                                                    if (!IsShowPopupWhenClick()) {
                                                        return;
                                                    }

                                                    ProjectDatePicker($(this))
                                                })
                                                //.addClass('TotalUnitCostHover')
                                            )
                                            .append( // DATE/TIME COMPLETED
                                                $('<td>')
                                                .css({
                                                    'text-align': 'center',
                                                    'position': 'relative'
                                                })
                                                //.append((vRows[i].IsParent == 1 ?
                                                //            '<div state="edit" id="txtTotalAmount_' + ctrParent + '" class="NumericCell ' + isDisableCell + '" columnName="t_amount" data-role="none" isRequired="0">' + (vRows[i].TotalAmount == 0 ? "-" : AccountingFormat($.format.number(vRows[i].TotalAmount))) + '</div>'
                                                //        :
                                                //            '<div state="edit" class="txtOnlyChild NumericCell ' + isDisableCell + '" data-role="none" isRequired="0">' + (vRows[i].TotalAmount == 0 ? "-" : AccountingFormat($.format.number(vRows[i].TotalAmount))) + '</div>'
                                                //        )
                                                //)
                                                .append($('<div state="edit" data-role="none" isRequired="0">')
                                                        .attr('id', (vRows[i].IsParent == 1 ? 'txtTotalAmount_' + ctrParent : ''))
                                                        .addClass((vRows[i].IsParent == 1 ? ' ' : ' txtOnlyChild ') + isDisableCell + ' tableCellReadOnly')
                                                        .data('data', CStr2(vRows[i].DateCompleted))
                                                        .text((CStr2(vRows[i].DateCompleted) == '' ? '' : $.format.date(new Date(vRows[i].DateCompleted), 'MMM dd, yyyy hh:mm a')))
                                                )
                                                .data('data', CStr2(vRows[i].DateCompleted))
                                                .addClass('tableCell cell-timespend tddatecompleted')
                                                .addClass((CBool(vRows[i].IsMaterial) ? 'tableCellDisable' : ''))
                                                .click(function (e) {
                                                    if (!IsShowPopupWhenClick()) {
                                                        return;
                                                    }

                                                    ProjectDatePicker($(this))
                                                })
                                                //.addClass('TotalAmountHover')
                                            )
                                            .append( // IMPORTANCE
                                                $('<td>')
                                                .append($('<div state="edit" data-role="none" isRequired="0">')
                                                        .attr('id', (vRows[i].IsParent == 1 ? 'txtQty_' + ctrParent : ''))
                                                        .addClass((vRows[i].IsParent == 1 ? ' ' : ' txtOnlyChild ') + isDisableCell + ' tableCellReadOnly')
                                                        .data('data', [])
                                                        .append(ProjectImportance(CBool(vRows[i].IsMaterial), (vRows[i].Importance)))
                                                )
                                                .addClass('tableCell tdimportance cell-timespend')
                                                .addClass((CBool(vRows[i].IsMaterial) ? 'tableCellDisable' : ''))
                                            )
                                            .append( // URGENCY
                                                $('<td>')
                                                .append($('<div state="edit" data-role="none" isRequired="0">')
                                                        .attr('id', (vRows[i].IsParent == 1 ? 'txtQty_' + ctrParent : ''))
                                                        .addClass((vRows[i].IsParent == 1 ? ' ' : ' txtOnlyChild ') + isDisableCell + ' tableCellReadOnly')
                                                        .data('data', [])
                                                        .append(ProjectUrgency(CBool(vRows[i].IsMaterial), (vRows[i].Urgency)))
                                                )
                                                .addClass('tableCell tdurgency cell-timespend')
                                                .addClass((CBool(vRows[i].IsMaterial) ? 'tableCellDisable' : ''))
                                            )
                                            .append( // UNIT MEASURE
                                                $('<td>')
                                                .append($('<div state="edit" data-role="none" isRequired="0">')
                                                        .attr('id', (vRows[i].IsParent == 1 ? 'txtQty_' + ctrParent : ''))
                                                        .addClass((vRows[i].IsParent == 1 ? ' ' : ' txtOnlyChild ') + isDisableCell + ' tableCellReadOnly')
                                                        .data('data', vRows[i].BudgetedQty)
                                                        .append(ProjectUnitMeasure(CBool(vRows[i].IsMaterial), vRows[i].UnitMeasureID))
                                                )
                                                .addClass('tableCell tdunitmeasure cell-materialspend')
                                                .addClass((CBool(vRows[i].IsMaterial) ? '' : 'tableCellDisable'))
                                            )
                                            .append( // BUDGETED QTY
                                                $('<td>')
                                                .append($('<div state="edit" data-role="none" isRequired="0">')
                                                        .attr('id', (vRows[i].IsParent == 1 ? 'txtQty_' + ctrParent : ''))
                                                        .addClass((vRows[i].IsParent == 1 ? ' ' : ' txtOnlyChild ') + isDisableCell)
                                                        .data('data', vRows[i].BudgetedQty)
                                                        .text((vRows[i].BudgetedQty.length > 0 ? vRows[i].BudgetedQty[vRows[i].BudgetedQty.length - 1].Qty : ''))
                                                )
                                                .addClass('tableCell cell-materialspend tdbudgetedqty')
                                                .addClass((CBool(vRows[i].IsMaterial) ? '' : 'tableCellDisable'))
                                            )
                                            .append( // LATEST REQUESTED QTY
                                                $('<td>')
                                                .append($('<div state="edit" data-role="none" isRequired="0">')
                                                        .attr('id', (vRows[i].IsParent == 1 ? 'txtQty_' + ctrParent : ''))
                                                        .addClass((vRows[i].IsParent == 1 ? ' ' : ' txtOnlyChild ') + isDisableCell + ' tableCellReadOnly')
                                                        .data('data', vRows[i].AccReqQty)
                                                        .text((vRows[i].AccReqQty.length > 0 ? (vRows[i].AccReqQty[vRows[i].AccReqQty.length - 1].State == 'cancelled' ? '' : vRows[i].AccReqQty[vRows[i].AccReqQty.length - 1].Qty) : ''))
                                                )
                                                .addClass('tableCell cell-materialspend tdreqqty')
                                                .addClass((CBool(vRows[i].IsMaterial) ? '' : 'tableCellDisable'))
                                            )
                                            .append( // TOTAL REQUESTED QTY
                                                $('<td>')
                                                .append($('<div state="edit" data-role="none" isRequired="0">')
                                                        .attr('id', (vRows[i].IsParent == 1 ? 'txtQty_' + ctrParent : ''))
                                                        .addClass((vRows[i].IsParent == 1 ? ' ' : ' txtOnlyChild ') + isDisableCell + ' tableCellReadOnly')
                                                        .data('data', vRows[i].AccReqQty)
                                                        .text(totalreqqty)
                                                )
                                                .addClass('tableCell cell-materialspend tdaccreqqty')
                                                .addClass((CBool(vRows[i].IsMaterial) ? '' : 'tableCellDisable'))
                                            )
                                            .append( // ACC DELIVERY QTY
                                                $('<td>')
                                                .append($('<div state="edit" data-role="none" isRequired="0">')
                                                        .attr('id', (vRows[i].IsParent == 1 ? 'txtQty_' + ctrParent : ''))
                                                        .addClass((vRows[i].IsParent == 1 ? ' ' : ' txtOnlyChild ') + isDisableCell + ' tableCellReadOnly')
                                                        .data('data', [])
                                                )
                                                .addClass('tableCell tdaccdeliveryqty cell-materialspend')
                                                .addClass((CBool(vRows[i].IsMaterial) ? '' : 'tableCellDisable'))
                                            )
                                            .append( // ACC USED UP QTY
                                                $('<td>')
                                                .append($('<div state="edit" data-role="none" isRequired="0">')
                                                        .attr('id', (vRows[i].IsParent == 1 ? 'txtQty_' + ctrParent : ''))
                                                        .addClass((vRows[i].IsParent == 1 ? ' ' : ' txtOnlyChild ') + isDisableCell + ' tableCellReadOnly')
                                                        .data('data', vRows[i].AccUsedUpQty)
                                                        .text(totalusedupqty)
                                                )
                                                .addClass('tableCell cell-materialspend tdaccusedup')
                                                .addClass((CBool(vRows[i].IsMaterial) ? '' : 'tableCellDisable'))
                                            )
                                            .append( // BUDGETED LABOR COST
                                                $('<td>')
                                                .append($('<div state="edit" data-role="none" isRequired="0">')
                                                        .attr('id', (vRows[i].IsParent == 1 ? 'txtQty_' + ctrParent : ''))
                                                        .addClass((vRows[i].IsParent == 1 ? ' ' : ' txtOnlyChild ') + isDisableCell)
                                                        .data('data', vRows[i].LaborCost)
                                                        .text((vRows[i].LaborCost.length > 0 ? vRows[i].LaborCost[vRows[i].LaborCost.length - 1].Amount : ''))
                                                )
                                                .addClass('tableCell cell-laborspend tdlaborcost')
                                            )
                                            .append( // TOTAL REQUESTED ADVANCES
                                                $('<td>')
                                                .append($('<div state="edit" data-role="none" isRequired="0">')
                                                        .attr('id', (vRows[i].IsParent == 1 ? 'txtQty_' + ctrParent : ''))
                                                        .addClass((vRows[i].IsParent == 1 ? ' ' : ' txtOnlyChild ') + isDisableCell + ' tableCellReadOnly')
                                                        .data('data', vRows[i].RequestedAdvances)
                                                        .text(totalreqadvances)
                                                )
                                                .addClass('tableCell cell-laborspend tdaccreqlaborcost')
                                            )
                                            .append( // ACC DISBURSED LABOR COST
                                                $('<td>')
                                                .append($('<div state="edit" data-role="none" isRequired="0">')
                                                        .attr('id', (vRows[i].IsParent == 1 ? 'txtQty_' + ctrParent : ''))
                                                        .addClass((vRows[i].IsParent == 1 ? ' ' : ' txtOnlyChild ') + isDisableCell + ' tableCellReadOnly')
                                                        .data('data', [])
                                                        //.text((vRows[i].RequestedAdvances.length > 0 ? vRows[i].RequestedAdvances[vRows[i].RequestedAdvances.length - 1].Amount : ''))
                                                )
                                                .addClass('tableCell tddisbursedlaborcost cell-laborspend ')
                                            )
                                        )
                                    ctrParent = ctrParent + 1;
                                }
                                else {
                                    isViewOnly = 1;
                                    $('#tBodyProjectBudget').append($('<tr>')
                                                                    .addClass((vRows[i].LineDescription.toLowerCase().indexOf("subtotal") == 0 ? 'rowSubtotal ' : '') + (CBool(vRows[i].IsMaterial) ? ' rowMaterials ' : ''))
                                                                    .attr('descriptionrowid', vRows[i].ID)
                                                                    .attr('id', vRows[i].RowNumber)
                                                                    .append(
                                                                        $('<td>')
                                                                        .append($('<span class="bi bi-grip-vertical">'))
                                                                        .css({
                                                                            backgroundColor: '#fff'
                                                                        })
                                                                    )
                                                                    .append($('<td>')
                                                                        .addClass(isDisableCell + ' tableCellDisable tdfreeze tditemno')
                                                                        .css({
                                                                            width: '86px'
                                                                        })
                                                                        .text(vRows[i].ItemNumber)
                                                                    )
                                                                    .append($('<td>')
                                                                        .attr(hasAssociatedSKU)
                                                                        .attr(attrs)
                                                                        .addClass(isDisableCell + ' tdfreeze tddescription ' + (CBool(vRows[i].IsMaterial) ? 'rowMaterials ' : ' ') + (CBool(vRows[i].IsMarkAsCompleted) ? 'markascompleted ' : ' '))
                                                                        .css({
                                                                            paddingRight: 0,
                                                                            textTransform: textTransform,
                                                                            //position: 'relative',
                                                                            width: (315 - parseInt(noOfIndent)) + 'px',
                                                                            left: '121px',
                                                                            backgroundColor: '#fff'
                                                                        })
                                                                        .attr('indentno', vRows[i].IndentNo)
                                                                        .text(vRows[i].LineDescription)
                                                                        .append(preAssociatedSKU)
                                                                    )
                                                                    .append($('<td>')
                                                                        .css('text-align', 'left')
                                                                        .addClass(isDisableCell + ' tdpointperson')
                                                                        .text(CStr2(vRows[i].PointPersonName))
                                                                        //.text(vRows[i].Qty == 0 ? "" : vRows[i].Qty)
                                                                    )
                                                                    .append($('<td>')
                                                                        .css('text-align', 'left')
                                                                        .addClass(isDisableCell + ' tdremarks')
                                                                        .html((vRows[i].Remarks.length > 0 ? HtmlToPlainText(HtmlDecode(vRows[i].Remarks[vRows[i].Remarks.length - 1].Remarks)).substring(0, 50) + ' ...' : ''))
                                                                    )
                                                                    .append($('<td>')
                                                                        .css({
                                                                            'text-align': 'right',
                                                                            'position': 'relative'
                                                                        })
                                                                        .addClass(isDisableCell + ' cell-timespend tdcompletionrate' + (CBool(vRows[i].IsMaterial) ? 'tableCellDisable' : ''))
                                                                        //.text(vRows[i].MaterialUnitCost == 0 ? "-" : AccountingFormat($.format.number(vRows[i].MaterialUnitCost)))
                                                                    )
                                                                    .append($('<td>')
                                                                        .css('text-align', 'left')
                                                                        .addClass(isDisableCell + ' cell-timespend tddependency' + (CBool(vRows[i].IsMaterial) ? 'tableCellDisable' : ''))
                                                                        .html(CStr2(vRows[i].Dependency).split(',').join(', '))
                                                                        //.text(vRows[i].MaterialAmount == "0" ? "-" : AccountingFormat($.format.number(vRows[i].MaterialAmount)))
                                                                    )
                                                                    .append($('<td>')
                                                                        .css({
                                                                            'text-align': 'center',
                                                                            'position': 'relative'
                                                                        })
                                                                        .addClass(isDisableCell + ' cell-timespend tdbudgetedduration ' + (CBool(vRows[i].IsMaterial) ? 'tableCellDisable' : ''))
                                                                        .text((vRows[i].BudgetedDuration.length > 0 ? vRows[i].BudgetedDuration[vRows[i].BudgetedDuration.length - 1].Duration + ' ' + vRows[i].BudgetedDuration[vRows[i].BudgetedDuration.length - 1].StateIn : ''))
                                                                        //.text(vRows[i].LaborUnitCost == 0 ? "-" : AccountingFormat($.format.number(vRows[i].LaborUnitCost)))
                                                                    )
                                                                    .append($('<td>')
                                                                        .css('text-align', 'center')
                                                                        .addClass(isDisableCell + ' cell-timespend tdactualduration ' + (CBool(vRows[i].IsMaterial) ? 'tableCellDisable' : ''))
                                                                        .text(actualduration)
                                                                        //.text(vRows[i].LaborAmount == "0" ? "-" : AccountingFormat($.format.number(vRows[i].LaborAmount)))
                                                                    )
                                                                    .append($('<td>')
                                                                        .css('text-align', 'center')
                                                                        .addClass(isDisableCell + ' cell-timespend tddatestarted ' + (CBool(vRows[i].IsMaterial) ? 'tableCellDisable' : ''))
                                                                        .text((CStr2(vRows[i].DateStarted) == '' ? '' : $.format.date(new Date(vRows[i].DateStarted), 'MMM dd, yyyy')))
                                                                        //.text(vRows[i].TotalUnitCost == 0 ? "-" : AccountingFormat($.format.number(vRows[i].TotalUnitCost)))
                                                                    )
                                                                    .append($('<td>')
                                                                        .css('text-align', 'center')
                                                                        .addClass(isDisableCell + ' cell-timespend tddatecompleted ' + (CBool(vRows[i].IsMaterial) ? 'tableCellDisable' : ''))
                                                                        .text((CStr2(vRows[i].DateCompleted) == '' ? '' : $.format.date(new Date(vRows[i].DateCompleted), 'MMM dd, yyyy')))
                                                                        //.text(vRows[i].TotalAmount == 0 ? "-" : AccountingFormat($.format.number(vRows[i].TotalAmount)))
                                                                    )
                                                                    .append($('<td>')
                                                                        .css('text-align', 'center')
                                                                        .addClass(isDisableCell + ' cell-timespend tdimportance ' + (CBool(vRows[i].IsMaterial) ? 'tableCellDisable' : ''))
                                                                        .text(vRows[i].Importance)
                                                                    )
                                                                    .append($('<td>')
                                                                        .css('text-align', 'center')
                                                                        .addClass(isDisableCell + ' cell-timespend tdurgency ' + (CBool(vRows[i].IsMaterial) ? 'tableCellDisable' : ''))
                                                                        .text(vRows[i].Urgency)
                                                                    )
                                                                    .append($('<td>')
                                                                        .css('text-align', 'center')
                                                                        .addClass(isDisableCell + ' cell-materialspend tdunitmeasure ' + (CBool(vRows[i].IsMaterial) ? '' : 'tableCellDisable'))
                                                                        .text(CStr2(vRows[i].UnitMeasureName))
                                                                    )
                                                                    .append($('<td>')
                                                                        .css('text-align', 'center')
                                                                        .addClass(isDisableCell + ' cell-materialspend tdbudgetedqty ' + (CBool(vRows[i].IsMaterial) ? '' : 'tableCellDisable'))
                                                                        .text((vRows[i].BudgetedQty.length > 0 ? vRows[i].BudgetedQty[vRows[i].BudgetedQty.length - 1].Qty : ''))
                                                                    )                                                
                                                                    .append($('<td>') // LATEST REQUESTED QTY
                                                                        .css('text-align', 'center')
                                                                        .addClass(isDisableCell + ' cell-materialspend tdreqqty ' + (CBool(vRows[i].IsMaterial) ? '' : 'tableCellDisable'))
                                                                        .text((vRows[i].AccReqQty.length > 0 ? (vRows[i].AccReqQty[vRows[i].AccReqQty.length - 1].State == 'cancelled' ? '' : vRows[i].AccReqQty[vRows[i].AccReqQty.length - 1].Qty) : ''))
                                                                    )
                                                                    .append($('<td>') // TOTAL REQUESTED QTY
                                                                        .css('text-align', 'center')
                                                                        .addClass(isDisableCell + ' cell-materialspend tdaccreqqty ' + (CBool(vRows[i].IsMaterial) ? '' : 'tableCellDisable'))
                                                                        .text(totalreqqty)
                                                                    )
                                                                    .append($('<td>')
                                                                        .css('text-align', 'center')
                                                                        .addClass(isDisableCell + ' cell-materialspend tdaccdeliveryqty ' + (CBool(vRows[i].IsMaterial) ? '' : 'tableCellDisable'))
                                                                    )
                                                                    .append($('<td>') // TOTAL USED UP QTY
                                                                        .css('text-align', 'center')
                                                                        .addClass(isDisableCell + ' cell-materialspend tdaccusedup ' + (CBool(vRows[i].IsMaterial) ? '' : 'tableCellDisable'))
                                                                        .text(totalusedupqty)
                                                                    )
                                                                    .append($('<td>')
                                                                        .css('text-align', 'center')
                                                                        .addClass(isDisableCell + ' cell-laborspend tdlaborcost ')
                                                                        .text((vRows[i].LaborCost.length > 0 ? vRows[i].LaborCost[vRows[i].LaborCost.length - 1].Amount : ''))
                                                                    )
                                                                    .append($('<td>')
                                                                        .css('text-align', 'center')
                                                                        .addClass(isDisableCell + ' cell-laborspend tdaccreqlaborcost ')
                                                                        .text(totalreqadvances)
                                                                    )
                                                                    .append($('<td>')
                                                                        .css('text-align', 'center')
                                                                        .addClass(isDisableCell + ' cell-laborspend tddisbursedlaborcost ')
                                                                        .text('')
                                                                    )
                                                                    // .css('cursor', 'pointer')
                                                                    // .click(function(){
                                                                    //     NewMessageBox('Would you like to edit the project budget sheet?', 'Project Budget', [
                                                                    //             {
                                                                    //                 Text: 'Yes',
                                                                    //                 Click: function(){
                                                                    //                     $('#btnNewProjectBudget').click();
                                                                    //                 }
                                                                    //             },
                                                                    //             {
                                                                    //                 Text: 'No'
                                                                    //             }
                                                                    //         ])
                                                                    // })
                                    )                                            
                                }

                                RefreshFreezePanesHeight(vRows[i].RowNumber);

                                //for (var s = 0; s < vRows[i].AssociatedSKUs.length; s++) {
                                //    AppendRow((CBool(isCreateNewProjectBudget) ? $('#tBodyProjectBudget [descriptionrowid="' + vRows[i].ID + '"]') : $('#tBodyExcelLike [descriptionrowid="' + vRows[i].ID + '"]')),
                                //        vRows[i].RowNumber + 1, '',
                                //        CStr2(vRows[i].RowNumber + 1), vRows[i].AssociatedSKUs[s].SkuName,
                                //        vRows[i].AssociatedSKUs[s].UomName, true);
                                //}
                            }
                            if (isCreateNewProjectBudget == false) {
                                TaskDependency();
                                $('.preAssociatedSKU').width($('#tblNewProjectBudget thead th:contains("DESCRIPTION")').width() + 'px');
                            }
                            else {
                                $('.preAssociatedSKU').width($('#tblProjectBudget thead th:contains("DESCRIPTION")').width() + 'px');
                            }
                            $('.btnDisplayAssociatedSKU').attr('state', 'show').text('Show Linked SKU');
                            CreateExcelTable();
                            countParent = ($('#tBodyExcelLike div[isparent="1"]:not(":last"):last').closest('tr').find('td').eq(1).text() == "" ? 0 : parseInt($('#tBodyExcelLike div[isparent="1"]:not(":last"):last').closest('tr').find('td').eq(1).text()))
                            isCreateNewProjectBudget = false;
                            $('#tBodyExcelLike tr:first').closest('tr').find('td.tddescription').addClass('activeCell');
                            //GetProjectBudgetCanvassMaterials(isViewOnly);                                    
                        }
                        else {
                            isCreateNewProjectBudget = true;
                            $('#tBodyProjectBudget').append($('<tr>')
                                                                .append($('<td>')
                                                                        .attr('colspan', 10)
                                                                        .css({ textAlign: 'center' })
                                                                        .text('No Result(s) Found')
                                                                    )
                                                                .css('cursor', 'pointer')
                                                                .click(function () {
                                                                    $('#btnNewProjectBudget').click();
                                                                })
                                )
                        }

                        FilterProjectSummary()
                    }
                })
            }

            function HtmlToPlainText(str) {
                var tmp = document.createElement("DIV");
                tmp.innerHTML = str;
                return tmp.textContent || tmp.innerText;
            }

            function RefreshFreezePanesHeight(id) {
                var height = $('#' + id).outerHeight();
                $('#' + id + ' td').each(function () {
                    height = Math.max($(this).outerHeight(), height);
                })
                //height = (height - 19) + 'px';
                $('#' + id).css({ height: (height + 1) + 'px' });
                $('#' + id + ' td').css({ minHeight: (height - 19) + 'px' });
            }

            function RefreshUnfreezeContent() {
                return;
                var margin = 0;

                $('.thfreeze:visible').each(function () {
                    margin += $(this).outerWidth()
                })

                $('.divProjectExcelLike').css('margin-left', (margin - 11) + 'px');
            }

            function ProjectUnitMeasure(isDisable, val) {
                if (!isDisable) {
                    return;
                }

                var sel = $('<select data-role="none" class="form-select selProjectUom">').append($('#selMaterialBudgetUom').clone().html());
                sel.val(CStr2(val))
                sel.find('option').css('display', 'none');
                return sel;
            }

            function PointPerson(val) {
                selPointPerson = $('<select data-role="none" class="form-select">');
                selPointPerson.append(new Option('Please Select', ''));
                for (var i = 0; i < arrProjectMember.length; i++) {
                    selPointPerson.append(new Option(arrProjectMember[i].PeopleName, arrProjectMember[i].ID))
                }

                selPointPerson.val(val || '');
                return selPointPerson;
            }

            function ProjectUrgency(isDisable, val) {
                if (isDisable) {
                    return;
                }

                var sel = $('<select data-role="none" class="form-select selProjectUrgency">')
                            .append($('<option value="" style="display:none;">').text(''))
                            .append($('<option value="1 - Very Not Urgent" style="display:none;">').text('1 - Very Not Urgent'))
                            .append($('<option value="2 - Not Urgent" style="display:none;">').text('2 - Not Urgent'))
                            .append($('<option value="3 - Neutral" style="display:none;">').text('3 - Neutral'))
                            .append($('<option value="4 - Urgent" style="display:none;">').text('4 - Urgent'))
                            .append($('<option value="5 - Very Urgent" style="display:none;">').text('5 - Very Urgent'));

                sel.val(val)

                return sel;
            }

            function ProjectImportance(isDisable, val) {
                if (isDisable) {
                    return;
                }

                var sel = $('<select data-role="none" class="form-select selProjectImportance">')
                            .append($('<option value="" style="display:none;">').text(''))
                            .append($('<option value="1 - Very Not Important" style="display:none;">').text('1 - Very Not Important'))
                            .append($('<option value="2 - Not Important" style="display:none;">').text('2 - Not Important'))
                            .append($('<option value="3 - Neutral" style="display:none;">').text('3 - Neutral'))
                            .append($('<option value="4 - Important" style="display:none;">').text('4 - Important'))
                            .append($('<option value="5 - Very Important" style="display:none;">').text('5 - Very Important'));

                sel.val(val)

                return sel;
            }

            function ProjectDatePicker(dis, callback) {
                if (dis.hasClass('tableCellDisable')) {
                    return;
                }

                if (CStr2($.trim(dis.parent().find('td.tddescription').text())) == '') {
                    WebToast('Please enter description first before you can set date', 'top');
                    return;
                }

                var cell = dis.find('div');
                var celldata = cell.data('data');
                var data = {
                    ShowTime: true,
                    MinDate: new Date(),
                    SetTime: $.format.date(new Date(), 'hh:mm a'),
                    Click: function (e) {
                        cell.text(e);

                        if($.isFunction(callback)){
                            callback();
                        }
                    }
                }

                if ($.trim(cell.text()) != "") {
                    data.SetTime = $.format.date(new Date(cell.text()), 'hh:mm a');
                }

                SingleCalendar(data)
            }

            function ProjectDuration(dis) {
                if (dis.hasClass('tableCellDisable')) {
                    return;
                }

                if (CStr2($.trim(dis.parent().find('td.tddescription').text())) == '') {
                    WebToast('Please enter description first before you can set duration', 'top');
                    return;
                }                        

                var data = dis.find('div').data('data').length > 0 ? dis.find('div').data('data')[0] : [];
                var duration = (data.Duration || '');
                var statein = (data.StateIn || 'days');                        

                $('#popProjectDuration').modal('show');
                $('#txtProjectDuration').val(duration).focus();
                $('#selProjectStateIn').val(statein);//.selectmenu('refresh');
            }

            function TaskDependency(isDisable, dis) {
                if (isDisable) {
                    return '';
                }

                var listTaskDependency = $('#listTaskDependency');
                listTaskDependency.empty();

                for (var i = 0; i < arrProjectBudget.length; i++) {
                    if (arrProjectBudget[i].LineDescription.toLowerCase().indexOf("subtotal") == 0 ||
                        CBool(arrProjectBudget[i].IsMaterial)) {
                        continue;
                    }

                    var itemno = arrProjectBudget[i].ItemNumber;
                    var groupnumber = itemno.substring(0, itemno.lastIndexOf('.') + 1).slice(0, -1);

                    listTaskDependency.append($('<li class="list-group-item">')
                        .addClass('listProjectDependency')
                        .attr('groupnumber', (groupnumber == '' ? itemno : groupnumber ))
                        .append($('<div class="form-check">')
                            .append($('<label class="form-check-label">')
                                .append($('<input type="checkbox" data-role="none" class="form-check-input chbProjectDependency" name="chbProjectDependency" value="' + arrProjectBudget[i].ItemNumber + '">'))
                                .append(arrProjectBudget[i].ItemNumber + ' ' + arrProjectBudget[i].LineDescription)
                            )
                        )
                    )
                }

                $('.chbProjectDependency');//.checkboxradio();//.checkboxradio('refresh');
                //var select = $('<select data-role="none" class="form-select seldependency">')
                //        .append($('<option value="">').text('Please select'))

                //for (var i = 0; i < arrProjectBudget.length; i++) {
                //    if (arrProjectBudget[i].LineDescription.toLowerCase().indexOf("subtotal") == 0 ||
                //        CBool(arrProjectBudget[i].IsMaterial)) {
                //        continue;
                //    }

                //    select.append($('<option value="' + arrProjectBudget[i].ItemNumber + '">').text(arrProjectBudget[i].ItemNumber + ' - ' + arrProjectBudget[i].LineDescription))
                //}
                //select.val(val);
                //return select
            }

            function RefreshTaskDependency() {
                var seldependency = $('.seldependency:first'); // select on first dependency combo box
                var lastitemnumber;

                $('#tBodyExcelLike tr:not(.rowMaterials)').each(function () {
                    var td = $(this).find('td');
                    var itemnumber = $.trim(td.filter('.tditemno').text());
                    var desc = $.trim(td.filter('.tddescription').text());
                            
                    if (seldependency.find('option[value="' + itemnumber + '"]').length == 0) {
                        //add newly added description in all dependency combobox
                        $('.seldependency').find('option[value="' + lastitemnumber + '"]').after($('<option value="' + itemnumber + '">').text(itemnumber + ' - ' + desc))
                        lastitemnumber = itemnumber;
                    } else {
                        lastitemnumber = itemnumber;
                    }
                })
            }

            function GetProjectBudgetCanvassMaterials(isViewOnly) {
                var pData = {};

                pData.ProjectID = sessionStorage.ProjectID;
                pData.OrgID = sessionStorage.OrgID;

                AjaxSend({
                    url: 'BOMProjectExcel/GetProjectBudgetCanvassMaterials',
                    data: pData,
                    success: function (vData) {
                        if (vData.CanvassMaterials.Rows.length > 0) {
                            for (var i = 0; i < vData.CanvassMaterials.Rows.length; i++) {
                                if ((vData.CanvassMaterials.Rows[i].PeopleName != "" && vData.CanvassMaterials.Rows[i].CompanyName != "")
                                    || (vData.CanvassMaterials.Rows[i].PeopleName == "" && vData.CanvassMaterials.Rows[i].CompanyName != "")
                                    || (vData.CanvassMaterials.Rows[i].PeopleName != "" && vData.CanvassMaterials.Rows[i].CompanyName == "")
                                    ) {
                                    isCanvassMaterialsAlready = true;
                                }
                            }
                            isFinishedGettingCanvassMaterials = true;
                        }
                        else {
                            isFinishedGettingCanvassMaterials = false;
                        }
                        LoadCanvass(vData.CanvassMaterials, isViewOnly, 'materials');
                        GetProjectBudgetCanvassLabor(isViewOnly);
                    }
                })
            }

            function LoadCanvass(vData, isViewOnly, canvass) {
                var tHead, tBody, index, pseudoClass, headerName, isFinished, selector;

                if (isCanvassMaterials) {
                    isFinished = isFinishedGettingCanvassMaterials;
                }
                else {
                    isFinished = isFinishedGettingCanvassLabor;
                }

                if (isCanvassForm == false) {
                    if (isViewOnly == 1) {
                        tBody = '#tBodyProjectBudget';
                        if (canvass == "materials") {
                            index = 4;
                        }
                        else {
                            index = 6;
                        }
                        pseudoClass = '';
                    }
                    else {
                        tBody = '#tBodyExcelLike';
                        if (canvass == "materials") {
                            index = 5;
                        }
                        else {
                            index = 7;
                        }
                        pseudoClass = ':not(":last")';
                    }

                    if (vData.Rows.length > 0) {
                        $(tBody).find('tr').find('td:eq("' + index + '")').find('span.custom-tooltip').remove();

                        for (var i = 0; i < vData.Rows.length; i++) {
                            if (vData.Rows[i].CompanyName != "" || vData.Rows[i].PeopleName != "") {
                                var td = $(tBody + ' tr[id="' + vData.Rows[i].RowNumber + '"]').find('td:eq(' + index + ')');
                                headerName = vData.Rows[i].CompanyName == "" ? vData.Rows[i].PeopleName : vData.Rows[i].CompanyName;
                                //td.find('div').find('span.tooltip').remove();
                                if (td.find('span.custom-tooltip').length == 0) {
                                    td.append($('<span>')
                                                .addClass('custom-tooltip')
                                                .append($('<span>')
                                                            .addClass('tooltip-container')
                                                            .append($('<span>')
                                                                    .addClass('grid-left')
                                                                    .text(headerName)
                                                                )
                                                            .append($('<span>')
                                                                    .addClass('grid-right')
                                                                    .text(vData.Rows[i].Cost)
                                                                )
                                                )
                                        )
                                }
                                else {
                                    td.find('span.custom-tooltip').append($('<span>')
                                                                    .addClass('tooltip-container')
                                                                    .append($('<span>')
                                                                            .addClass('grid-left')
                                                                            .text(headerName)
                                                                        )
                                                                    .append($('<span>')
                                                                            .addClass('grid-right')
                                                                            .text(vData.Rows[i].Cost)
                                                                        )
                                                        )
                                }
                            }
                        }
                    }
                }
                else {
                    if (isCanvassMaterials) {
                        tBody = "#tBodyCanvassMaterial";
                        tHead = "#tHeadCanvassMaterial";
                    }
                    else {
                        tBody = "#tBodyCanvassLabor";
                        tHead = "#tHeadCanvassLabor";
                    }

                    for (var i = 0; i < vData.Rows.length; i++) {
                        headerName = vData.Rows[i].CompanyName == "" ? vData.Rows[i].PeopleName : vData.Rows[i].CompanyName;
                        index = $(tHead + ' th:contains("' + headerName + '")').index() + 4;
                        selector = $(tBody + ' div[descriptionid="' + vData.Rows[i].ItemNumber + '"]');

                        if (CBool(vData.Rows[i].IsPreferredCanvass)) {
                            selector.closest('tr').find('td:eq(' + index + ')').attr('isPreferredCanvass', '1').addClass('preferredVendor');
                        }

                        selector.closest('tr').find('td:eq(' + index + ')').find('div').attr('id', vData.Rows[i].ID).attr('state', 'edit').text(vData.Rows[i].Cost);

                        if (selector.hasClass('disableCell')) {
                            selector.closest('tr').find('td:eq(' + index + ')').find('div').addClass('disableCell');
                        }
                    }

                    isCanvassForm = false;
                    SetColorMinPrice();
                }
            }

            function GetProjectBudgetCanvassLabor(isViewOnly) {
                var pData = {};

                pData.ProjectID = sessionStorage.ProjectID;
                pData.OrgID = sessionStorage.OrgID;

                AjaxSend({
                    url: 'BOMProjectExcel/GetProjectBudgetCanvassLabor',
                    data: pData,
                    success: function (vData) {
                        if (vData.CanvassLabor.Rows.length > 0) {
                            for (var i = 0; i < vData.CanvassLabor.Rows.length; i++) {
                                if ((vData.CanvassLabor.Rows[i].PeopleName != "" && vData.CanvassLabor.Rows[i].CompanyName != "")
                                    || (vData.CanvassLabor.Rows[i].PeopleName == "" && vData.CanvassLabor.Rows[i].CompanyName != "")
                                    || (vData.CanvassLabor.Rows[i].PeopleName != "" && vData.CanvassLabor.Rows[i].CompanyName == "")) {
                                    isCanvassLaborAlready = true;
                                }
                            }
                            isFinishedGettingCanvassLabor = true;
                        }
                        else {
                            isFinishedGettingCanvassLabor = false;
                        }
                        LoadCanvass(vData.CanvassLabor, isViewOnly, "labor");
                    }
                })
            }

            function GetCanvassMaterials() {
                var pData = {};

                pData.ProjectID = sessionStorage.ProjectID;
                pData.OrgID = sessionStorage.OrgID;
                pData.CanvassForm = "yes";

                arrCanvassMaterialsOrgPeople = [];

                AjaxSend({
                    url: 'BOMProjectExcel/GetProjectBudgetCanvassMaterials',
                    data: pData,
                    success: function (vData) {

                        if (vData.CanvassMaterials.Rows.length > 0) {
                            for (var i = 0; i < vData.CanvassMaterials.Rows.length; i++) {
                                if (vData.CanvassMaterials.Rows[i].CompanyID != undefined) {
                                    if (vData.CanvassMaterials.Rows[i].PeopleName == "" && vData.CanvassMaterials.Rows[i].CompanyName != "") {
                                        arrCanvassMaterialsOrgPeople.push({
                                            "ID": (vData.CanvassMaterials.Rows[i].CompanyName != "" ? vData.CanvassMaterials.Rows[i].CompanyID : vData.CanvassMaterials.Rows[i].PeopleID),
                                            "Name": (vData.CanvassMaterials.Rows[i].CompanyName != "" ? vData.CanvassMaterials.Rows[i].CompanyName : vData.CanvassMaterials.Rows[i].PeopleName),
                                            "State": "edit",
                                            "IDDB": vData.CanvassMaterials.Rows[i].ID
                                        })
                                    }
                                }
                                else if (vData.CanvassMaterials.Rows[i].PeopleName != "") {
                                    arrCanvassMaterialsOrgPeople.push({
                                        "ID": vData.CanvassMaterials.Rows[i].PeopleID,
                                        "Name": vData.CanvassMaterials.Rows[i].PeopleName,
                                        "State": "edit",
                                        "IDDB": vData.CanvassMaterials.Rows[i].ID
                                    })
                                }
                            }
                            $('#btnCanvassNow').click();
                            GetProjectBudgetCanvassMaterials();
                        }
                        else {
                            isFinishedGettingCanvassMaterials = false;
                        }

                    }
                })
            }

            function GetCanvassLabor() {
                var pData = {};

                pData.ProjectID = sessionStorage.ProjectID;
                pData.OrgID = sessionStorage.OrgID;
                pData.CanvassForm = "yes";

                arrCanvassLaborOrgPeople = [];

                AjaxSend({
                    url: 'BOMProjectExcel/GetProjectBudgetCanvassLabor',
                    data: pData,
                    success: function (vData) {

                        if (vData.CanvassLabor.Rows.length > 0) {
                            for (var i = 0; i < vData.CanvassLabor.Rows.length; i++) {
                                if (vData.CanvassLabor.Rows[i].CompanyID != undefined) {
                                    if (vData.CanvassLabor.Rows[i].PeopleName == "" && vData.CanvassLabor.Rows[i].CompanyName != "") {
                                        arrCanvassLaborOrgPeople.push({
                                            "ID": (vData.CanvassLabor.Rows[i].CompanyName != "" ? vData.CanvassLabor.Rows[i].CompanyID : vData.CanvassLabor.Rows[i].PeopleID),
                                            "Name": (vData.CanvassLabor.Rows[i].CompanyName != "" ? vData.CanvassLabor.Rows[i].CompanyName : vData.CanvassLabor.Rows[i].PeopleName),
                                            "State": "edit",
                                            "IDDB": vData.CanvassLabor.Rows[i].ID
                                        })
                                    }
                                }
                                else if (vData.CanvassLabor.Rows[i].PeopleName != "") {
                                    arrCanvassLaborOrgPeople.push({
                                        "ID": vData.CanvassLabor.Rows[i].PeopleID,
                                        "Name": vData.CanvassLabor.Rows[i].PeopleName,
                                        "State": "edit",
                                        "IDDB": vData.CanvassLabor.Rows[i].ID
                                    })
                                }
                            }
                            $('#btnCanvassNow').click();
                            GetProjectBudgetCanvassLabor();

                        }
                        else {
                            isFinishedGettingCanvassLabor = false;
                        }

                    }
                })
            }

            var dtProject;
            function GetProjectHeader(FowstID, State, keywords) {
                var pData = { "OrganizationID": sessionStorage.OrgID, "ProjectFowstID": FowstID, keywords: CStr2(keywords) }
                var count = 1;

                if (State == undefined) {
                    $('.spantxtProjectName').text('');
                    $('.spantxtProjectCode').text('');
                    $('.spantxtProjectDescription').text('');
                    $('.spantxtProjectAddress').text('')
                    $('#tblBodyProjectHeader').empty().append('<tr><td colspan="10" style="text-align:center;">Please wait...</td></tr>');
                }

                if(CStr2(State) == ''){
                    if ($.fn.DataTable.isDataTable($('#tblListOfProjects'))) {
                        $('#tblListOfProjects').empty();
                        dtProject.destroy();
                        //return;
                    }

                    dtProject = $('#tblListOfProjects').DataTable({
                        ajax: {
                            url: "/BOMProjectExcel/GetProjectHeader?OrgID=" + sessionStorage.OrgID + "&ProjectFowstID=" + CStr2(FowstID) + '&keywords=' + CStr2(keywords),
                            type: 'POST'
                        },
                        "pageLength": 10,
                        "columns": [
                            {
                                "data": function (data, type, full, meta) {
                                    return '<b class="ui-table-cell-label">#</b>' + data.RowNum;
                                },
                                "width": "30px"
                            },
                            {
                                "data": function (data, type, full, meta) {
                                    return '<b class="ui-table-cell-label">Project Name:</b>' + data.ProjectName;
                                }
                            },
                            {
                                "data": function (data, type, full, meta) {
                                    return '<b class="ui-table-cell-label">Project Code:</b>' + data.ProjectName;
                                }
                            },
                            {
                                "data": function (data, type, full, meta) {
                                    return '<b class="ui-table-cell-label">Projected Start Date:</b>' + $.format.date(new Date(data.ProjectedStartDate), 'MMM dd, yyyy')
                                }
                            },
                            {
                                "data": function (data, type, full, meta) {
                                    return '<b class="ui-table-cell-label">Actual Start Date:</b>' + (data.ActualStartDate == null ? "--" : $.format.date(new Date(data.ActualStartDate), "MMM dd, yyyy"))
                                }
                            },
                            {
                                "data": function (data, type, full, meta) {
                                    return '<b class="ui-table-cell-label">Projected Completion Date:</b>' + $.format.date(new Date(data.ProjectedCompletionDate), 'MMM dd, yyyy')
                                }
                            },
                            {
                                "data": function (data, type, full, meta) {
                                    return '<b class="ui-table-cell-label">Actual Completion Date:</b>' + (data.ActualCompletionDate == null ? "--" : $.format.date(new Date(data.ActualCompletionDate), "MMM dd, yyyy"))
                                }
                            },
                            {
                                "data": function (data, type, full, meta) {
                                    return '<b class="ui-table-cell-label">Last Updated:</b>' + $.format.date(new Date(data.LastUpdated), 'MMM dd, yyyy')
                                }
                            }
                        ],
                        "bProcessing": true,
                        "bServerSide": true,
                        "order": [[7, "desc"]],
                        "aoColumnDefs": [{ "bSortable": false, "aTargets": [0] }],
                        "responsive": true,
                        "columnDefs": [{ "targets": 0, "orderable": true }],
                        "sDom": '<"top">rt<"bottom"ip><"clear">'
                        //"stateSave": true
                    })

                    $('#tblListOfProjects tbody').unbind('click').on('click', 'tr', function () {
                        if (CStr2(dtProject.row(this).data()) != '') {
                            var data = dtProject.row(this).data();
                            if (location.pathname.toLowerCase() != '/erp') {
                                $('.ItemsBreadCrumbs').text('');
                                breadcrumbs.prepend(arrowR, "Project Budget");
                                $('.BreadCrumbs').html(breadcrumbs.prepend(lnkProject));
                            }
                            ViewProject(data);
                        }
                    });
                } else {
                    AjaxSend({
                        url: 'BOMProjectExcel/GetProjectInfo',
                        data: pData,
                        success: function (vData) {
                            if (CStr2(State) == '') {
                                $('#tblBodyProjectHeader').empty();
                            }

                            if (vData.ProjectHeader.Rows.length > 0) {
                                for (var i = 0; i < vData.ProjectHeader.Rows.length; i++) {

                                    currentProjectName = vData.ProjectHeader.Rows[i].ProjectName;
                                    currentProjectAddress = vData.ProjectHeader.Rows[i].StreetNumber + " " + vData.ProjectHeader.Rows[i].Street + " " + vData.ProjectHeader.Rows[i].City + " " + vData.ProjectHeader.Rows[i].Country;

                                    if (State == "Edit") {
                                        $('#txtProjectID').val(vData.ProjectHeader.Rows[i].ID);
                                        $('#txtProjectName').val(vData.ProjectHeader.Rows[i].ProjectName);
                                        $('#txtProjectCode').val(vData.ProjectHeader.Rows[i].ProjectCode);
                                        $('#txtProjectDescription').val(vData.ProjectHeader.Rows[i].ProjectDescription);

                                        $('#txtOrgFOWSTBuildingNo2').val(vData.ProjectHeader.Rows[i].StreetNumber);
                                        $('#txtOrgFOWSTStreet2').val(vData.ProjectHeader.Rows[i].Street);
                                        $('#txtOrgFOWSTCityID2').val(vData.ProjectHeader.Rows[i].CityID);
                                        $('#txtCityName').text(vData.ProjectHeader.Rows[i].City);
                                        $('#txtOrgFOWSTCountryID2').val(vData.ProjectHeader.Rows[i].CountryID);
                                        $('#txtCountryName').text(vData.ProjectHeader.Rows[i].Country);

                                        $('#ProjStartDate').text($.format.date(new Date(vData.ProjectHeader.Rows[i].ProjectedStartDate), "MMM dd, yyyy"));
                                        $('#ActualStartDate').text((vData.ProjectHeader.Rows[i].ActualStartDate == undefined ? "Set Date" : $.format.date(new Date(vData.ProjectHeader.Rows[i].ActualStartDate), "MMM dd, yyyy")));
                                        $('#ProjCompDate').text($.format.date(new Date(vData.ProjectHeader.Rows[i].ProjectedCompletionDate), "MMM dd, yyyy"));
                                        $('#ActualCompDate').text((vData.ProjectHeader.Rows[i].ActualCompletionDate == undefined ? "Set Date" : $.format.date(new Date(vData.ProjectHeader.Rows[i].ActualCompletionDate), "MMM dd, yyyy")));
                                        //$('#btnSetDateDone').click();

                                        GetProjectOwnerOrgPerson(FowstID)
                                    }
                                    else if (State == "View") {
                                        $('.spantxtProjectName').text(vData.ProjectHeader.Rows[i].ProjectName);
                                        $('.spantxtProjectCode').text(vData.ProjectHeader.Rows[i].ProjectCode);
                                        $('.spantxtProjectDescription').text(vData.ProjectHeader.Rows[i].ProjectDescription);
                                        $('.spantxtProjectAddress').text((vData.ProjectHeader.Rows[i].StreetNumber == undefined ? "" : vData.ProjectHeader.Rows[i].StreetNumber + ", ") +
                                                                            vData.ProjectHeader.Rows[i].Street + ", " + vData.ProjectHeader.Rows[i].City + ", " +
                                                                            vData.ProjectHeader.Rows[i].Country
                                                                        );
                                    }
                                }
                            }
                            else {
                                $('#tblBodyProjectHeader').append($('<tr>')
                                                                    .append($('<td>')
                                                                            .attr('colspan', 8)
                                                                            .css('text-align', 'center')
                                                                            .text('No Result(s) Found')
                                                                        )
                                    )
                            }
                            $('#tblBodyProjectHeader').parent().table('refresh');
                        }
                    })
                }                        
            }

            function ViewProject(data) {
                $('#popupProjectList').hide();
                $('#pageProjectBudget').pageshow();
                $('#divViewProjectBudget').show();
                $('.divNewProjectBudget, #divButtons').hide();
                $('#divProjectTitle').empty()
                    .html(data.ProjectName + '<br>')
                    .append($('<a class="custom-link" style="float:right; font-size:14px;">').text('Edit Project Details')
                        .data('data', data)
                        .click(function (e) {
                            $('#btnEditProjectDetails').click()
                        })
                    )
                $('#listTaskDependency').empty();
                arrProjectMember = [];
                selPointPerson = undefined;
                projectData = new FormData();
                sessionStorage["ProjectID"] = data.ID;
                isCreateNewProjectBudget = true;
                isCanvassMaterialsAlready = false;
                isCanvassLaborAlready = false;
                isFinishedGettingCanvassMaterials = false;
                isFinishedGettingCanvassLabor = false;
                isFinishLoadCanvassMaterials = false;
                isFinishLoadCanvassLabor = false;                        
                GetProjectBudget(data.ID);
                GetProjectHeader(data.ID, 'View');
                GetAccessRights();
            }

            function GetAccessRights() {
                AjaxSend({
                    url: 'BOMProjectExcel/GetAccessRights',
                    data: { id: sessionStorage.ProjectID },
                    success: function (vData) {
                        arrAccessRights = [];
                        for (var i = 0; i < vData.Results.Rows.length; i++) {
                            arrAccessRights.push(vData.Results.Rows[i].Description);
                        }
                    }
                })
            }

            function GetProjectOwnerOrgPerson(FowstID) {
                var pData = { "OrganizationID": sessionStorage.OrgID, "FowstID": FowstID }

                AjaxSend({
                    url: 'BOMProjectExcel/GetProjectOwnerOrgPerson',
                    data: pData,
                    success: function (vData) {
                        if (vData.ProjectOwnerOrgPerson.Rows.length > 0) {
                            for (var i = 0; i < vData.ProjectOwnerOrgPerson.Rows.length; i++) {
                                $('#txtProjectOwnerOrgPersonID').val(vData.ProjectOwnerOrgPerson.Rows[i].ProjectBudgetID);
                                $('#txtClientID').val(vData.ProjectOwnerOrgPerson.Rows[i].ProjectOwnerOrgPersonID);
                                $('#txtClientName').text((vData.ProjectOwnerOrgPerson.Rows[i].CompanyName == "" ? vData.ProjectOwnerOrgPerson.Rows[i].PeopleName : vData.ProjectOwnerOrgPerson.Rows[i].CompanyName));
                                if (vData.ProjectOwnerOrgPerson.Rows[i].CompanyName != "") {
                                    GetOrgFowst(vData.ProjectOwnerOrgPerson.Rows[i].ProjectOwnerOrgPersonID, vData.ProjectOwnerOrgPerson.Rows[i].ProjectOwnerFowstID);
                                }
                            }
                        }
                        else {
                            $('#txtClientName').text("Please Select");
                        }
                    }
                })
            }

            function GetProjectOwnerManagerAssistant(FowstID, isProjectMembersOnly, callback) {
                $('#pageProjectDetails').show();
                $('#pageProjectList').hide();

                var pData = { "FowstID": FowstID }

                $('#lstProjectOwner').empty();
                $('#lstProjectManager').empty();
                $('#lstAsstProjectManager').empty();

                AjaxSend({
                    url: 'BOMProjectExcel/GetProjectOwnerManagerAssistant',
                    data: pData,
                    success: function (vData) {
                        var selReqByProjectMember = $('.selReqByProjectMember');
                        selReqByProjectMember.empty();

                        if (vData.ProjectPeopleDetails.Rows.length > 0) {
                            selReqByProjectMember.append(new Option('Please select', ''));

                            for (var i = 0; i < vData.ProjectPeopleDetails.Rows.length; i++) {
                                if (vData.ProjectPeopleDetails.Rows[i].IsProjectManager != undefined && vData.ProjectPeopleDetails.Rows[i].IsProjectOwner == undefined) {
                                    if (vData.ProjectPeopleDetails.Rows[i].IsProjectManager == 1) {
                                        arrManagerList.push({
                                            "id": vData.ProjectPeopleDetails.Rows[i].ID,
                                            "PeopleID": vData.ProjectPeopleDetails.Rows[i].PeopleID,
                                            "PeopleName": vData.ProjectPeopleDetails.Rows[i].PeopleName,
                                            "IsProjectManager": vData.ProjectPeopleDetails.Rows[i].IsProjectManager,
                                            "ProjectManagerState": "Edit"
                                        });
                                    }
                                    else {
                                        arrAsstManagerList.push({
                                            "id": vData.ProjectPeopleDetails.Rows[i].ID,
                                            "PeopleID": vData.ProjectPeopleDetails.Rows[i].PeopleID,
                                            "PeopleName": vData.ProjectPeopleDetails.Rows[i].PeopleName,
                                            "IsProjectManager": vData.ProjectPeopleDetails.Rows[i].IsProjectManager,
                                            "ProjectAsstManagerState": "Edit"
                                        });
                                    }
                                }
                                else {
                                    arrOwnerList.push({
                                        "id": vData.ProjectPeopleDetails.Rows[i].ID,
                                        "PeopleID": vData.ProjectPeopleDetails.Rows[i].PeopleID,
                                        "PeopleName": vData.ProjectPeopleDetails.Rows[i].PeopleName,
                                        "IsProjectOwner": vData.ProjectPeopleDetails.Rows[i].IsProjectOwner,
                                        "ProjectOwnerState": "Edit"
                                    });

                                    arrProjectMember.push({
                                        "dbID": vData.ProjectPeopleDetails.Rows[i].ID,
                                        "ID": vData.ProjectPeopleDetails.Rows[i].PeopleID,
                                        "PeopleName": vData.ProjectPeopleDetails.Rows[i].PeopleName,
                                        "IsActive": vData.ProjectPeopleDetails.Rows[i].IsActive,
                                        "RoleID": vData.ProjectPeopleDetails.Rows[i].MemberRoleID,
                                        "State": "edit"
                                    })

                                    selReqByProjectMember.append(new Option(vData.ProjectPeopleDetails.Rows[i].PeopleName, vData.ProjectPeopleDetails.Rows[i].PeopleID));
                                }
                            }
                        }

                        if (!CBool(isProjectMembersOnly)) {
                            LoadManagerList();
                            LoadOwnerList();
                            LoadAsstManagerList();

                            GetProjectHeader(FowstID, "Edit");
                            GetProjectTelephoneInfo(FowstID);
                            GetJobRole();
                        } else {
                            callback();
                        }
                    }
                })
            }

            function GetProjectTelephoneInfo(FowstID) {
                $('#pageProjectDetails').show();
                $('#pageProjectList').hide();

                var pData = { "FowstID": FowstID }

                intTelInfoCtr = intTelInfoCtr + 1;

                AjaxSend({
                    url: 'BOMProjectExcel/GetProjectTelephoneInfo',
                    data: pData,
                    success: function (vData) {
                        if (vData.ProjectTelephoneDetails.Rows.length > 0) {
                            for (var i = 0; i < vData.ProjectTelephoneDetails.Rows.length; i++) {
                                arrTelInfoList.push({
                                    "TelRankNo": intTelInfoCtr,
                                    "CountryCodeID": vData.ProjectTelephoneDetails.Rows[i].CountryID,
                                    "CountryCode": vData.ProjectTelephoneDetails.Rows[i].CountryCode + " - " + vData.ProjectTelephoneDetails.Rows[i].CountryCallingCode,
                                    "AreaCode": vData.ProjectTelephoneDetails.Rows[i].Area + " - " + vData.ProjectTelephoneDetails.Rows[i].AreaCallingCode,
                                    "AreaCodeID": vData.ProjectTelephoneDetails.Rows[i].AreaCodeID,
                                    "TelephoneNo": vData.ProjectTelephoneDetails.Rows[i].ContactNo,
                                    "ExtensionNo": vData.ProjectTelephoneDetails.Rows[i].ExtensionNo,
                                    "ContactType": vData.ProjectTelephoneDetails.Rows[i].ContactType,
                                    "TelState": "Edit",
                                    "IsNew": 0,
                                    "id": vData.ProjectTelephoneDetails.Rows[i].ID
                                })

                                intTelInfoCtr = intTelInfoCtr + 1;
                            }
                        }
                        AddTelInfo();
                        GetProjectMobileInfo(FowstID)
                    }
                })
            }

            function GetProjectMobileInfo(FowstID) {
                $('#pageProjectDetails').show();
                $('#pageProjectList').hide();

                var pData = { "FowstID": FowstID }

                intMobileInfoCtr = intMobileInfoCtr + 1;

                AjaxSend({
                    url: 'BOMProjectExcel/GetProjectMobileInfo',
                    data: pData,
                    success: function (vData) {
                        if (vData.ProjectMobileDetails.Rows.length > 0) {
                            for (var i = 0; i < vData.ProjectMobileDetails.Rows.length; i++) {
                                arrMobileInfoList.push({
                                    "MobileRankNo": intMobileInfoCtr,
                                    "CountryCodeID": vData.ProjectMobileDetails.Rows[i].CountryID,
                                    "CountryCode": vData.ProjectMobileDetails.Rows[i].CountryCode + " - " + vData.ProjectMobileDetails.Rows[i].CountryCallingCode,
                                    "TelCode": vData.ProjectMobileDetails.Rows[i].TelCoName + ' - ' + vData.ProjectMobileDetails.Rows[i].TelCoPrefix,
                                    "TelCodeID": vData.ProjectMobileDetails.Rows[i].TelCoPrefixID,
                                    "MobileNo": vData.ProjectMobileDetails.Rows[i].ContactNo,
                                    "ContactType": vData.ProjectMobileDetails.Rows[i].ContactType,
                                    "MobileState": "Edit",
                                    "IsNew": 0,
                                    "id": vData.ProjectMobileDetails.Rows[i].ID
                                })

                                intMobileInfoCtr = intMobileInfoCtr + 1;
                            }
                        }
                        AddMobileInfo();
                        GetProjectEmailInfo(FowstID);
                    }
                })
            }

            function GetProjectEmailInfo(FowstID) {
                $('#pageProjectDetails').show();
                $('#pageProjectList').hide();

                var pData = { "FowstID": FowstID }

                intEmailInfoCtr = intEmailInfoCtr + 1;

                AjaxSend({
                    url: 'BOMProjectExcel/GetProjectEmailInfo',
                    data: pData,
                    success: function (vData) {
                        if (vData.ProjectEmailDetails.Rows.length > 0) {
                            for (var i = 0; i < vData.ProjectEmailDetails.Rows.length; i++) {
                                arrEmailInfoList.push({
                                    "EmailRankNo": intEmailInfoCtr,
                                    "EmailAddID": vData.ProjectEmailDetails.Rows[i].ID,
                                    "EmailAdd": vData.ProjectEmailDetails.Rows[i].EmailAddress,
                                    "EmailType": vData.ProjectEmailDetails.Rows[i].EmailType,
                                    "EmailState": "Edit",
                                    "IsNew": 0,
                                    "id": vData.ProjectEmailDetails.Rows[i].ID
                                })

                                intEmailInfoCtr = intEmailInfoCtr + 1;
                            }
                        }
                        AddEmailInfo();
                    }
                })
            }

            function getOrgAddFowstType() {
                AjaxSend({
                    url: 'Browser/LibraryFowstType',
                    success: function (vData) {
                        var vRows = vData.Results.Rows;

                        $('#selFowstType').empty();

                        $('#selFowstType').append(new Option('Please Select', ''));
                        for (var p = 0; p <= vRows.length - 1; p++) {
                            $('#selFowstType').append(new Option(vRows[p].Name, vRows[p].ID, false, false));
                        }

                        $('#selFowstType').append(new Option('Project', 'DFF0F087-4C2B-4B67-AA1D-502F02773E2B'));
                        $('#selFowstType option:contains("Project")').attr('selected', 'selected');
                        $('#selFowstType');//.selectmenu('refresh');
                    },
                    error: function (vError) {
                        console.log(vError);
                    }
                });
            }

            function ClearAllFields() {
                $('#txtProjectID').val('');
                $('#txtProjectName').val('');
                $('#txtProjectCode').val('');
                $('#txtProjectDescription').val('');
                $('#txtOrgFOWSTBuildingNo2').val('');
                $('#txtProjectOwnerOrgPersonID').val('');
                $('#selFowstLocation').empty().append('<option value="">Please Select</option>');//.selectmenu('disable');//.selectmenu('refresh');
                $('#txtFowstLocation').val('');//.textinput('disable');//.textinput();
                $('#txtClientID').val('');
                $('#txtClientName').text('Please Select');
                $('#txtOrgFOWSTStreet2').val('');
                $('#txtOrgFOWSTCityID2').val('');
                $('#txtCityName').text('Please Select');
                $('#txtOrgFOWSTCountryID2').val('e783ed13-aa98-4cc1-918e-d49b05c0167e');
                $('#txtCountryName').text('Philippines');
                $('#lstProjectOwner').empty();
                $('#lstProjectManager').empty();
                $('#lstAsstProjectManager').empty();
                $('#ProjStartDate').text('Set Date');
                $('#ActualStartDate').text('Set Date');
                $('#ProjCompDate').text('Set Date');
                $('#ActualCompDate').text('Set Date');
                $('#lstOrgFOWSTTelephone2').empty();
                $('#txtOrgFOWSTTelID2').val('');
                $('#txtOrgFOWSTTelAreaCodeID2').val('');
                $('#txtAreaCode').text('Select');
                $('#txtTelephoneNum').val('');
                $('#txtTelephoneExt').val('');
                $('#selOrgFOWSTTelType2').val('');
                $('#lstOrgFOWSTMobile2').empty();
                $('#txtOrgFOWSTMobileID2').val('');
                $('#txtOrgFOWSTTelCoPrefixID2').val('');
                $('#txtTelCode').text('Select');
                $('#txtMobileNo2').val('');
                $('#selOrgFOWSTMobileType2').val('');
                $('#lstOrgFOWSTEmail2').empty();
                $('#txtEmailAdd2').val('');
                $('#selOrgFOWSTEmailType2').val('');
                $('#divAddTel').hide();
                $('#divAddMobile').hide();
                $('#divAddEmail').hide();
                $('#tbodyProjectMember').empty();
                $('#txtProjectedTotalNumberofDays, #txtActualTotalNumberofDays').text(0);
            }

            function ClearAllVariables() {
                intTelInfoCtr = 0;
                intMobileInfoCtr = 0;
                intEmailInfoCtr = 0;
                TelState = "";
                TelRankNo = "";
                MobileState = "";
                MobileRankNo = "";
                EmailState = "";
                EmailRankNo = "";
                arrOwnerList = [];
                arrManagerList = [];
                arrAsstManagerList = [];
                arrProjectMember = [];
                arrTelInfoList = [];
                arrMobileInfoList = [];
                arrEmailInfoList = [];
            }

            function ClearTelInfoFields() {
                TelState = '';
                $('#txtOrgFOWSTTelID2').val('');
                $('#txtOrgFOWSTTelAreaCodeID2').val('');
                $('#txtAreaCode').text('Select');
                $('#txtTelephoneNum').val('');
                $('#txtTelephoneExt').val('');
                $('#selOrgFOWSTTelType2').val('');//.selectmenu('refresh');
            }

            function ClearMobileInfoFields() {
                MobileState = '';
                $('#txtOrgFOWSTMobileID2').val('');
                $('#txtOrgFOWSTTelCoPrefixID2').val('');
                $('#txtTelCode').text('Select');
                $('#txtMobileNo2').val('');
                $('#selOrgFOWSTMobileType2').val('');//.selectmenu('refresh');
            }

            function ClearEmailInfoFields() {
                EmailState = '';
                $('#txtOrgFOWSTEmailID2').val('');
                $('#txtEmailAdd2').val('');
                $('#selOrgFOWSTEmailAdd').val('');//.selectmenu('refresh');
            }

            function ClearNewProjectCustomerFields() {
                $('#errorCompanyName').hide();
                $('#txtProjectCustomerCompanyName').val('');
                $('#errorTIN').hide();
                $('#txtProjectCustomerTIN').val('');
                $('#txtCustomerOrgHouseNo').val('');
                $('#txtCustomerOrgAddress').val('');
                $('#selOrgCity').val('');
                $('#selOrgCountry').val('');

                $('#errorFName').hide();
                $('#txtProjectCustomerFirstName').val('');
                $('#errorMName').hide();
                $('#txtProjectCustomerMiddleName').val('');
                $('#errorLName').hide();
                $('#txtProjectCustomerSurname').val('');
            }

            function AdjustContainerMarginTop() {
                if (location.pathname.toLowerCase() != '/erp') {
                    $('#mainContainer').css('margin-top', $('#divHeaderForProjects').height());
                }                        
            }

            function ChangePage(pageid) {
                $('div[custom-data-role="page"]').not(pageid).hide();
                $(pageid).show();
                $(window).scrollTop(0);
                AdjustContainerMarginTop();
                window.location.hash = '';
            }

            function Reposition() {
                $('div[data-role="popup"]').popup().popup("reposition", { positionTo: 'window' });
                $('div[data-role="popup"]').find('input[type="text"]:first').focus();
            }

            function AddChild(valueToAdd, uomname, isMaterial, hasSKU) {
                var addThisValue = (valueToAdd == undefined ? $('.txtChildDescription').val() : valueToAdd);

                //$('#tBodyExcelLike tr td.activeCell').closest('tr').next().find('td:eq(' + $('#tBodyExcelLike tr td.activeCell').closest('td').index() + ')').find('div').attr('descriptionid')
                if ($('div[parentid="' + descriptionid + '"]').length == 0) {
                    //if (isAddChildMultiple == false) {
                    //    $('#tBodyExcelLike tr td.activeCell').removeClass('activeCell');
                    //    $('#tBodyExcelLike tr.activeRow').removeClass('activeRow');
                    //}

                    AppendRow($('#tBodyExcelLike #' + rowIDOfParent), parseInt(rowIDOfParent) + 1, descriptionid, descriptionid, addThisValue, uomname, isMaterial, hasSKU);
                }
                else {
                    //var childnextid = $('div[parentid="' + descriptionid + '"]:last').attr('descriptionid').split('.')[1];
                    var currentChildID = $('#tBodyExcelLike tr td.activeCell').closest('tr').next().find('td:eq(' + $('#tBodyExcelLike tr td.activeCell').closest('td').index() + ')').find('div').attr('descriptionid');
                    var appendThisRow;

                    //$('#tBodyExcelLike tr td.activeCell').closest('tr').nextAll().each(function () {
                    //    if (!$(this).hasClass('rowMaterials')) {
                    //        currentChildID = $(this).find('td:eq(' + $('#tBodyExcelLike tr td.activeCell').closest('td').index() + ')').find('div').attr('descriptionid');
                    //        return false;
                    //    }
                    //})

                    if ($('div[parentid="' + descriptionid + '"]').attr('descriptionid') == undefined) {
                        appendThisRow = $('#tBodyExcelLike #' + rowIDOfParent)
                    }
                    else {
                        var currentRowIndex;
                        var currentParentID;

                        if (isparent == 1) {
                            currentParentID = itemid;
                        }
                        else {
                            currentParentID = descriptionid;
                        }

                        if(false){
                            currentRowIndex = $('div[descriptionid="' + currentParentID + '"]').closest('tr').index(); //rowIDOfParent;
                        } else {
                            currentRowIndex = $('div[parentid="' + currentParentID + '"]:last').parents().closest('tr').index(); //$('div[descriptionid="' + $('div[parentid="' + currentParentID + '"]:last').closest('td').prev().text() + '"]:last').closest('tr').index();

                            //$('#tBodyExcelLike tr:eq(' + $('#tBodyExcelLike div[descriptionid="' + $('div[parentid="' + currentParentID + '"]:last').attr('descriptionid') + '"').closest('tr').index() + ')')
                            //$('#tBodyExcelLike tr td.activeCell')
                            //.closest('tr').nextAll().each(function () {
                            //    if ($(this).hasClass('rowMaterials')) {
                            //        return true;
                            //    }
                            //    var text = $.trim($(this).find('td.tditemno').text());
                            //    var indexOf = 0;
                            //    if (text != "") {
                            //        if (currentParentID.match(/\./g) == null) {
                            //            //$(this).find('td.tditemno').text() == $('div[parentid="' + $('div[parentid="' + currentParentID + '"]:last').attr("descriptionid") + '"]:last').attr("descriptionid")
                            //            if ($(this).find('td.tditemno').text().split('.')[1] == $('div[parentid="' + currentParentID + '"]:last').closest('td').prev().text().split('.')[1]) {

                            //                if ($('div[parentid="' + currentParentID + '"]').length <= 1) {
                            //                    currentRowIndex = $(this).find('td.tditemno').closest('tr').index();
                            //                }
                            //                else {
                            //                    currentRowIndex = $(this).find('td.tditemno').closest('tr').index(); //$('div[parentid="' + $('div[parentid="' + currentParentID + '"]:last').attr("descriptionid") + '"]:last').closest('tr').index(); // $('div[parentid="' + currentParentID + '"]:last').closest('tr').index();
                            //                }
                            //            }
                            //        }
                            //        else {
                            //            if (currentParentID.match(/\./g).length == 1) {
                            //                indexOf = 1;
                            //            }
                            //            else if (currentParentID.match(/\./g).length == 2) {
                            //                indexOf = 2;
                            //            }
                            //            else if (currentParentID.match(/\./g).length == 3) {
                            //                indexOf = 3;
                            //            }
                            //            else if (currentParentID.match(/\./g).length == 4) {
                            //                indexOf = 4;
                            //            }
                            //            else if (currentParentID.match(/\./g).length == 5) {
                            //                indexOf = 5;
                            //            }

                            //            var firstchild = $('div[parentid="' + currentParentID + '"]:not(.rowMaterials):first');

                            //            //if(firstchild.length == 0){ // check kung wala pay child ang selected item no, walay labot ang mga materials
                            //            //    //kung walay anak, ang currentRowIndex kay ang row index sa pinakalast na materials sa item no.
                            //            //    currentRowIndex = $('div[parentid="' + currentParentID + '"].rowMaterials:last').closest('tr').index();
                            //            //    return false;
                            //            //}

                            //            var previtemno = firstchild.closest('td').prev().text()

                            //            //if (previtemno == '') { // exclude empty itemno for materials
                            //            //    return true;
                            //            //}

                            //            if ($(this).find('td.tditemno').text().split('.')[indexOf] == previtemno.split('.')[indexOf]) {
                            //                currentRowIndex = $(this).find('td.tditemno').closest('tr').index();
                            //            }
                            //            else {
                            //                return false;
                            //            }
                            //        }
                            //    }
                            //    else {
                            //        return false;
                            //    }

                            //})
                        }                                

                        appendThisRow = $('#tBodyExcelLike tr:eq(' + currentRowIndex + ')');
                    }

                    if (isAddChildMultiple == false) {
                        $('#tBodyExcelLike tr td.activeCell').removeClass('activeCell');
                        $('#tBodyExcelLike tr.activeRow').removeClass('activeRow');
                    }

                    //$('div[descriptionid="' + $('div[parentid="' + descriptionid + '"]:last').closest('td').prev().text() + '"]').closest('tr').attr('id')
                    var newRowID = (parseInt(appendThisRow.attr('id')) + 1);
                    AppendRow(appendThisRow, newRowID, currentChildID, descriptionid, addThisValue, uomname, isMaterial, hasSKU)                    
                }

                var currentCellIndex = $('#tBodyExcelLike tr td.activeCell').closest('td').index();
                $('#tBodyExcelLike tr td.activeCell').closest('tr').next().find('td:eq(' + currentCellIndex + ')').addClass('activeCell');
                $('#tBodyExcelLike tr td.activeCell').closest('tr').prev().find('td:eq(' + currentCellIndex + ')').removeClass('activeCell');
            }

            function AppendRow(row, rowID, childID, descID, value, uomname, isMaterial, hasSKU) {
                var itemno = CDec2($('#tBodyExcelLike tr td.activeCell').parent().find('.tditemno > div').text()) + 1;
                row.after($('<tr>')
                        .addClass((CBool(isMaterial) ? 'rowMaterials' : ''))
                        .attr('rowType', 'normal')
                        .attr('id', rowID)
                        .append(
                            $('<td class="tddraggable">')
                            .append($('<span class="bi bi-grip-vertical">'))
                            .css({
                                backgroundColor: '#fff'
                            })
                        )
                        .append($('<td>')
                            .append($('<div>')
                                .css({ width: '100%' })
                                //.text(ItemID(childID)) //parseInt(descriptionid) + '.' + (parseInt(childnextid) + 1)
                                .text(itemno)
                                .attr('state', 'add')
                            )
                            .addClass('tableCellDisable tdfreeze tditemno')
                            .css({
                                width: '85px'
                            })
                        )
                        .append($('<td>')
                            .addClass((CBool(isMaterial) ? 'rowMaterials' : ''))
                            .append($('<div>')                                
                                .attr({
                                    isParent: 0,
                                    parentid: descID,
                                    descriptionid: itemno,
                                    isRequired: 1,
                                    state: 'add',
                                    indentno: (indentno + 1)
                                })
                                .addClass('txtOnlyChild ' + (CBool(isMaterial) ? 'rowMaterials' : ''))
                                .data('data', [])
                                .html(value)
                            )
                            .addClass('tableCell tddescription tdfreeze ' + (CBool(hasSKU) ? 'hasAssociatedSKU' : ''))
                            .css({
                                paddingRight: 0,
                                //paddingLeft: NumberOfIndent(descID),
                                //position: 'relative',
                                //width: (316 - parseInt(NumberOfIndent(descID))) + 'px',
                                left: '120px',
                                backgroundColor: '#fff'
                            })
                        )
                        .append($("<td>").append($("<div>").append(PointPerson()).data('data', []).css({ width: "100%" }).attr("isRequired", 0).attr("columnName", "qty").addClass("NumericCell").attr('state', 'add')).addClass("tableCell tdpointperson"))
                        .append($("<td>").append($("<div>").data('files', []).data('newphotosformdata', []).data('data', []).css({ width: "100%" }).attr("isRequired", 0).attr("columnName", "unit").addClass('TextCell').addClass("NumericCell").attr('state', 'add')).addClass("tableCell tdremarks tableCellReadOnly"))
                        .append($("<td>").append($("<div>").data('data', []).css({ width: "100%" }).attr("isRequired", 0).attr("columnName", "m_unitcost").addClass("NumericCell").attr('state', 'add')).addClass("tableCell tdcompletionrate cell-timespend " + (CBool(isMaterial) ? 'tableCellDisable' : '')))
                        .append($("<td>").append($("<div>").data('data', []).css({ width: "100%" }).attr("isRequired", 0).attr("columnName", "m_amount").addClass("NumericCell").attr('state', 'add')).addClass("tableCell tddependency cell-timespend " + (CBool(isMaterial) ? 'tableCellDisable' : '')))
                        .append($("<td>").append($("<div>").data('data', []).css({ width: "100%" }).attr("isRequired", 0).attr("columnName", "l_unitcost").addClass("NumericCell").attr('state', 'add')).addClass("tableCell tdbudgetedduration cell-timespend " + (CBool(isMaterial) ? 'tableCellDisable' : '')).click(function (e) {
                            ProjectDuration($(this));
                        }))
                        .append($("<td>").append($("<div>").data('data', []).css({ width: "100%" }).attr("isRequired", 0).attr("columnName", "l_amount").addClass("NumericCell").attr('state', 'add')).addClass("tableCell tdactualduration cell-timespend " + (CBool(isMaterial) ? 'tableCellDisable' : '')))
                        .append($("<td>").append($("<div>").data('data', []).css({ width: "100%" }).attr("isRequired", 0).attr("columnName", "t_unitcost").addClass("NumericCell").attr('state', 'add')).addClass("tableCell tddatestarted cell-timespend " + (CBool(isMaterial) ? 'tableCellDisable' : '')).click(function (e) {
                            ProjectDatePicker($(this))
                        }))
                        .append($("<td>").append($("<div>").data('data', []).css({ width: "100%" }).attr("isRequired", 0).attr("columnName", "t_amount").addClass("NumericCell").attr('state', 'add')).addClass("tableCell tddatecompleted cell-timespend " + (CBool(isMaterial) ? 'tableCellDisable' : '')).click(function (e) {
                            ProjectDatePicker($(this))
                        }))
                        .append($("<td>").append($("<div>").data('data', []).css({ width: "100%" }).attr("isRequired", 0).attr("columnName", "t_unitcost").addClass("NumericCell").attr('state', 'add').append(ProjectImportance(CBool(isMaterial), '1 - Not Important'))).addClass("tableCell tdimportance cell-timespend " + (CBool(isMaterial) ? 'tableCellDisable' : '')))
                        .append($("<td>").append($("<div>").data('data', []).css({ width: "100%" }).attr("isRequired", 0).attr("columnName", "t_amount").addClass("NumericCell").attr('state', 'add').append(ProjectUrgency(CBool(isMaterial), '1 - Not Urgent'))).addClass("tableCell tdurgency cell-timespend " + (CBool(isMaterial) ? 'tableCellDisable' : '')))
                        .append($("<td>").append($("<div>").data('data', []).css({ width: "100%" }).attr("isRequired", 0).attr("columnName", "m_unitcost").addClass("NumericCell").attr('state', 'add').append(ProjectUnitMeasure(CBool(isMaterial), ''))).addClass("tableCell tdunitmeasure cell-materialspend " + (CBool(isMaterial) ? '' : 'tableCellDisable')))
                        .append($("<td>").append($("<div>").data('data', []).css({ width: "100%" }).attr("isRequired", 0).attr("columnName", "m_unitcost").addClass("NumericCell").attr('state', 'add')).addClass("tableCell cell-materialspend tdbudgetedqty " + (CBool(isMaterial) ? '' : 'tableCellDisable')))
                        .append($("<td>").append($("<div>").data('data', []).css({ width: "100%" }).attr("isRequired", 0).attr("columnName", "m_unitcost").addClass("NumericCell").attr('state', 'add')).addClass("tableCell cell-materialspend tdreqqty " + (CBool(isMaterial) ? '' : 'tableCellDisable')))
                        .append($("<td>").append($("<div>").data('data', []).css({ width: "100%" }).attr("isRequired", 0).attr("columnName", "m_unitcost").addClass("NumericCell").attr('state', 'add')).addClass("tableCell cell-materialspend tdaccreqqty " + (CBool(isMaterial) ? '' : 'tableCellDisable')))
                        .append($("<td>").append($("<div>").data('data', []).css({ width: "100%" }).attr("isRequired", 0).attr("columnName", "m_unitcost").addClass("NumericCell").attr('state', 'add')).addClass("tableCell cell-materialspend tdaccdeliveryqty " + (CBool(isMaterial) ? '' : 'tableCellDisable')))
                        .append($("<td>").append($("<div>").data('data', []).css({ width: "100%" }).attr("isRequired", 0).attr("columnName", "m_unitcost").addClass("NumericCell").attr('state', 'add')).addClass("tableCell cell-materialspend tdaccusedup " + (CBool(isMaterial) ? '' : 'tableCellDisable')))
                        .append($("<td>").append($("<div>").data('data', []).css({ width: "100%" }).attr("isRequired", 0).attr("columnName", "m_unitcost").addClass("NumericCell").attr('state', 'add')).addClass("tableCell cell-laborspend tdlaborcost"))
                        .append($("<td>").append($("<div>").data('data', []).css({ width: "100%" }).attr("isRequired", 0).attr("columnName", "m_unitcost").addClass("NumericCell").attr('state', 'add')).addClass("tableCell cell-laborspend tdaccreqlaborcost"))
                        .append($("<td>").append($("<div>").data('data', []).css({ width: "100%" }).attr("isRequired", 0).attr("columnName", "m_unitcost").addClass("NumericCell").attr('state', 'add')).addClass("tableCell cell-laborspend tddisbursedlaborcost"))
                    )

                RefreshFreezePanesHeight(rowID);
            }

            function ItemID(id) {
                var childnewid;

                if (id.match(/\./g) == null) {
                    return id + ".1";
                }

                if (isparent == 1) {
                    childnewid = $('div[parentid="' + itemid + '"]:last').attr('descriptionid').split('.');
                }
                else {
                    if ($('div[parentid="' + descriptionid + '"]:last').attr('descriptionid') == undefined) {
                        return descriptionid + '.1';
                    }
                    else {
                        childnewid = $('div[parentid="' + descriptionid + '"]:last').attr('descriptionid').split('.');
                    }
                }

                if (id.match(/\./g).length == 1) {
                    return childnewid[0] + '.' + (CInt2(childnewid[1]) + 1);
                }
                else if (id.match(/\./g).length == 2) {
                    return descriptionid + '.' + (CInt2(childnewid[2]) + 1);
                }
                else if (id.match(/\./g).length == 3) {
                    return descriptionid + '.' + (CInt2(childnewid[3]) + 1);
                }
                else if (id.match(/\./g).length == 4) {
                    return descriptionid + '.' + (CInt2(childnewid[4]) + 1);
                }
                else if (id.match(/\./g).length == 5) {
                    return descriptionid + '.' + (CInt2(childnewid[5]) + 1);
                }
            }

            function NumberOfIndent(id) {
                if (id.match(/\./g) == null) {
                    return "20px";
                }

                if (id.match(/\./g).length == 1) {
                    return "40px";
                }
                else if (id.match(/\./g).length == 2) {
                    return "60px";
                }
                else if (id.match(/\./g).length == 3) {
                    return "80px";
                }
                else if (id.match(/\./g).length == 4) {
                    return "100px";
                }
                else if (id.match(/\./g).length == 5) {
                    return "120px";
                }
            }

            function AddNewRow() {
                if ($('#tBodyExcelLike tr td.activeCell').closest('tr').next().length == 0) {
                    var arrNewRow = [];

                    $('#tBodyExcelLike').find('tr:last').each(function (i, el) {
                        var $tds = $(this).find('td');
                        arrNewRow.push([$tds.parent().attr('id'),
                                        $tds.eq(1).find('div').attr('id'),
                                        $tds.eq(2).find('div').attr('id'),
                                        $tds.eq(3).find('div').attr('id'),
                                        $tds.eq(4).find('div').attr('id'),
                                        $tds.eq(5).find('div').attr('id'),
                                        $tds.eq(6).find('div').attr('id'),
                                        $tds.eq(7).find('div').attr('id'),
                                        $tds.eq(8).find('div').attr('id'),
                                        $tds.eq(9).find('div').attr('id'),
                                        $tds.eq(10).find('div').attr('id')
                        ])
                    });

                    $('#tBodyExcelLike').append($('<tr id="' + (parseInt(arrNewRow[0][0]) + 1) + '" rowtype="normal">')
                    //.addClass('activeRow')
                        .append(
                            $('<td class="tddraggable">')
                            .append($('<span class="bi bi-grip-vertical">'))
                            .css({
                                backgroundColor: '#fff'
                            })
                        )
                        .append(
                            $('<td>')
                            .append($('<div>')
                                .attr('id', 'txtItemNumber_' + (parseInt(arrNewRow[0][1].split('_')[1]) + 1))
                                .css({ width: '100%' })
                                .attr('columnName', 'item')
                                .html(' ')
                                .attr('state', 'add')
                            )
                            .addClass('tableCellDisable tdfreeze tditemno')
                            .css({
                                width: '85px'
                            })
                        )
                        .append(
                            $('<td>')
                            .append($('<div>')
                                .attr('id', 'txtDescription_' + (parseInt(arrNewRow[0][2].split('_')[1]) + 1))
                                .addClass('txtOnlyParent')
                                .attr('isParent', 1)
                                .attr('isRequired', 1)
                                .attr('columnName', 'description')
                                .css({ width: '100%' })
                                .html('&nbsp;')
                                .attr('state', 'add')
                            )
                            .addClass('tableCell tddescription tdfreeze')
                            .css({
                                width: '305px',
                                left: '120px',
                                backgroundColor: '#fff'
                            })
                    //.addClass('activeCell')
                        )
                        .append(
                            $('<td>')
                            .append($('<div>')
                                .attr('id', 'txtQty_' + (parseInt(arrNewRow[0][3].split('_')[1]) + 1))
                                .css({ width: '100%' })
                                .attr('isRequired', 0)
                                .attr('columnName', 'qty')
                                .attr('state', 'add')
                                .data('data', [])
                                .append(PointPerson())
                            )
                            .addClass('tableCell tdpointperson')
                        )
                        .append(
                            $('<td>')
                            .append($('<div>')
                                .attr('id', 'txtUnit_' + (parseInt(arrNewRow[0][4].split('_')[1]) + 1))
                                .css({ width: '100%' })
                                .html('&nbsp;')
                                .attr('isRequired', 0)
                                .attr('columnName', 'unit')
                                .addClass('TextCell')
                                .attr('state', 'add')
                                .data('data', [])
                                .data('files', [])
                                .data('newphotosformdata', [])
                            )
                            .addClass('tableCell tdremarks')
                            //.addClass('tableCellDisable')
                        )
                        .append(
                            $('<td>')
                            .append($('<div>')
                                .attr('id', 'txtMaterialUnitCost_' + (parseInt(arrNewRow[0][5].split('_')[1]) + 1))
                                .css({ width: '100%' })
                                .html('&nbsp;')
                                .attr('isRequired', 0)
                                .attr('columnName', 'm_unitcost')
                                .addClass('NumericCell')
                                .attr('state', 'add')
                            )
                            .addClass('tableCell cell-timespend tdcompletionrate')
                            //.addClass('tableCellDisable')
                            //.addClass('btnCanvassMaterials')
                        )
                        .append(
                            $('<td>')
                            .append($('<div>')
                                .attr('id', 'txtMaterialAmount_' + (parseInt(arrNewRow[0][6].split('_')[1]) + 1))
                                .css({ width: '100%' })
                                .html('&nbsp;')
                                .attr('isRequired', 0)
                                .attr('columnName', 'm_amount')
                                .addClass('NumericCell')
                                .attr('state', 'add')
                                .data('data', [])
                            )
                            .addClass('tableCell cell-timespend tddependency')
                            //.addClass('tableCellDisable')
                            //.addClass('btnCanvassMaterials')
                        )
                        .append(
                            $('<td>')
                            .append($('<div>')
                                .attr('id', 'txtLaborUnitCost_' + (parseInt(arrNewRow[0][7].split('_')[1]) + 1))
                                .css({ width: '100%' })
                                .html('&nbsp;')
                                .attr('isRequired', 0)
                                .attr('columnName', 'l_unitcost')
                                .addClass('NumericCell')
                                .attr('state', 'add')
                                .data('data', [])
                            )
                            .addClass('tableCell cell-timespend tdbudgetedduration')
                            //.addClass('tableCellDisable')
                            //.addClass('btnCanvassLabor')
                        )
                        .append(
                            $('<td>')
                            .append($('<div>')
                                .attr('id', 'txtLaborAmount' + (parseInt(arrNewRow[0][8].split('_')[1]) + 1))
                                .css({ width: '100%' })
                                .html('&nbsp;')
                                .attr('isRequired', 0)
                                .attr('columnName', 'l_amount')
                                .addClass('NumericCell')
                                .attr('state', 'add')
                                .data('data', [])
                            )
                            .addClass('tableCell cell-timespend tdactualduration')
                            //.addClass('tableCellDisable')
                            //.addClass('btnCanvassLabor')
                        )
                        .append(
                            $('<td>')
                            .append($('<div>')
                                .attr('id', 'txtTotalUnitCost_' + (parseInt(arrNewRow[0][9].split('_')[1]) + 1))
                                .css({ width: '100%' })
                                .html('&nbsp;')
                                .attr('isRequired', 0)
                                .attr('columnName', 't_unitcost')
                                .addClass('NumericCell')
                                .attr('state', 'add')
                                .data('data', [])
                            )
                            .addClass('tableCell cell-timespend tddatestarted')
                            //.addClass('tableCellDisable')
                            //.addClass('TotalUnitCostHover')
                        )
                        .append(
                            $('<td>')
                            .append($('<div>')
                                .attr('id', 'txtTotalAmount_' + (parseInt(arrNewRow[0][10].split('_')[1]) + 1))
                                .css({ width: '100%' })
                                .html('&nbsp;')
                                .attr('isRequired', 0)
                                .attr('columnName', 't_amount')
                                .addClass('NumericCell')
                                .attr('state', 'add')
                                .data('data', [])
                            )
                            .addClass('tableCell cell-timespend tddatecompleted')
                            //.addClass('tableCellDisable')
                            //.addClass('TotalAmountHover')
                        )
                        .append(
                            $('<td>')
                            .append($('<div>')
                                .css({ width: '100%' })
                                .html('&nbsp;')
                                .attr('isRequired', 0)
                                .addClass('NumericCell')
                                .attr('state', 'add')
                                .data('data', [])
                            )
                            .addClass('tableCell cell-timespend tdimportance')
                        )
                        .append(
                            $('<td>')
                            .append($('<div>')
                                .css({ width: '100%' })
                                .html('&nbsp;')
                                .attr('isRequired', 0)
                                .addClass('NumericCell')
                                .attr('state', 'add')
                                .data('data', [])
                            )
                            .addClass('tableCell cell-timespend tdurgency')
                        )
                        .append(
                            $('<td>')
                            .append($('<div>')
                                .css({ width: '100%' })
                                .html('&nbsp;')
                                .attr('isRequired', 0)
                                .addClass('NumericCell')
                                .attr('state', 'add')
                                .data('data', [])
                            )
                            .addClass('tableCell cell-materialspend tdunitmeasure')
                        )
                        .append(
                            $('<td>')
                            .append($('<div>')
                                .css({ width: '100%' })
                                .html('&nbsp;')
                                .attr('isRequired', 0)
                                .addClass('NumericCell')
                                .attr('state', 'add')
                                .data('data', [])
                            )
                            .addClass('tableCell cell-materialspend tdbudgetedqty')
                        )
                        .append(
                            $('<td>')
                            .append($('<div>')
                                .css({ width: '100%' })
                                .html('&nbsp;')
                                .attr('isRequired', 0)
                                .addClass('NumericCell')
                                .attr('state', 'add')
                                .data('data', [])
                            )
                            .addClass('tableCell cell-materialspend tdreqqty')
                        )
                        .append(
                            $('<td>')
                            .append($('<div>')
                                .css({ width: '100%' })
                                .html('&nbsp;')
                                .attr('isRequired', 0)
                                .addClass('NumericCell')
                                .attr('state', 'add')
                                .data('data', [])
                            )
                            .addClass('tableCell cell-materialspend tdaccreqqty')
                        )
                        .append(
                            $('<td>')
                            .append($('<div>')
                                .css({ width: '100%' })
                                .html('&nbsp;')
                                .attr('isRequired', 0)
                                .addClass('NumericCell')
                                .attr('state', 'add')
                                .data('data', [])
                            )
                            .addClass('tableCell cell-materialspend tdaccdeliveryqty')
                        )
                        .append(
                            $('<td>')
                            .append($('<div>')
                                .css({ width: '100%' })
                                .html('&nbsp;')
                                .attr('isRequired', 0)
                                .addClass('NumericCell')
                                .attr('state', 'add')
                                .data('data', [])
                            )
                            .addClass('tableCell cell-materialspend tdaccusedup tableCellDisable')
                        )
                        .append(
                            $('<td>')
                            .append($('<div>')
                                .css({ width: '100%' })
                                .html('&nbsp;')
                                .attr('isRequired', 0)
                                .addClass('NumericCell')
                                .attr('state', 'add')
                                .data('data', [])
                            )
                            .addClass('tableCell cell-laborspend tdlaborcost')
                        )
                        .append(
                            $('<td>')
                            .append($('<div>')
                                .css({ width: '100%' })
                                .html('&nbsp;')
                                .attr('isRequired', 0)
                                .addClass('NumericCell')
                                .attr('state', 'add')
                                .data('data', [])
                            )
                            .addClass('tableCell cell-laborspend tdaccreqlaborcost')
                        )
                        .append(
                            $('<td>')
                            .append($('<div>')
                                .css({ width: '100%' })
                                .html('&nbsp;')
                                .attr('isRequired', 0)
                                .addClass('NumericCell')
                                .attr('state', 'add')
                                .data('data', [])
                            )
                            .addClass('tableCell cell-laborspend tddisbursedlaborcost')
                        )
                    )

                    RefreshFreezePanesHeight(parseInt(arrNewRow[0][0]) + 1);
                }
            }

            function UpdateRowIDs() {
                var currentRowNumber = parseInt($('#tBodyExcelLike tr:first').attr('id'));
                $('#tBodyExcelLike tr:first').nextAll().each(function () {
                    var id = $(this).prop("id", function (index, idvalue) {
                        return currentRowNumber + 1;
                    });
                    currentRowNumber = currentRowNumber + 1;
                });
            }

            function TempSaveEditedDescription(This, e) {
                currentCellIndex = $('#tBodyExcelLike tr td.activeCell').closest('td').index();

                var thisIsRequired = $(This).attr('isRequired');
                var thisAttr = $(This).attr('id').split('-')[1];
                var thisColumnName = $(This).attr('columnName');
                var thisDataType = $(This).attr('datatype');
                var thisState = $(This).attr('state');
                var thisIndentNo = $(This).attr('indentno');
                var thisData = $(This).data('data');
                var attr = {};

                if ($(This).attr('skuname') != undefined) {
                    attr["skuname"] = $(This).attr('skuname');
                    attr["associateskuid"] = $(This).attr('associateskuid');
                }

                SaveCellData(This);

                if ($('#tBodyExcelLike tr td.activeCell').find('input').attr('isParent') == 1 || $('#tBodyExcelLike tr td.activeCell').find('input').attr('isParent') == undefined) {
                    var thisVal = $(This).val() || '&nbsp;';

                    // if ($('#tBodyExcelLike tr td.activeCell').closest('tr').prev().find('td:eq(' + (parseInt($('#tBodyExcelLike tr td.activeCell').closest('td').index()) - 1) + ')').text() == ""
                    //     && $('#tBodyExcelLike tr td.activeCell').closest('td').prev().find('div').text() == " ") {
                    //     countParent = 1;
                    // }
                    // else {
                    //     countParent = countParent + 1;
                    // }

                    if (isCreateNewProjectBudget) {
                        if (isFirstCountParent) {
                            countParent = 1;
                            isFirstCountParent = false;
                        }
                        else {
                            countParent = countParent + 1;
                        }
                    }
                    else {
                        countParent = countParent + 1;
                    }

                    if ($.trim($('#tBodyExcelLike tr td.activeCell').closest('td').prev().find('div').text()) != "") {
                        countParent = parseInt($('#tBodyExcelLike tr td.activeCell').closest('td').prev().find('div').text());
                    }

                    countParent = CDec2($('#tBodyExcelLike tr td.activeCell').parent().prev().find('.tditemno > div').text()) + 1; // itemno must be in sequence starting Oct 1, 2018 by arnie

                    $('#tBodyExcelLike tr td.activeCell').closest('td').prev().find('div').text(countParent)

                    $(This).replaceWith($('<div>')
                        .html($.trim(thisVal))
                        .attr({
                            id: thisAttr,
                            isParent: 1,
                            descriptionid: countParent,
                            itemid: countParent,
                            isRequired: thisIsRequired,
                            columnName: thisColumnName,
                            datatype: thisDataType,
                            state: thisState,
                            indentno: thisIndentNo
                        })
                        .attr(attr)
                        .addClass('txtOnlyParent')
                    );
                }
                else {
                    var thisAttr = $(This).attr('id').split('-')[1];
                    var thisVal = $(This).val().replace(/ +(?= )/g, '');
                    var thisParentID = $(This).attr('parentid');
                    var thisChildID = $.trim($('#tBodyExcelLike tr td.activeCell').closest('td').prev().find('div').text())

                    $(This).replaceWith($('<div>')
                        .attr({
                            isParent: 0,
                            parentid: thisParentID,
                            descriptionid: thisChildID,
                            itemid: countParent,
                            isRequired: thisIsRequired,
                            columnName: thisColumnName,
                            datatype: thisDataType,
                            state: thisState,
                            indentno: thisIndentNo
                        })
                        .addClass('txtOnlyChild')
                        .data('data', thisData)
                        .attr(attr)
                        .html(thisVal)
                    );
                }

                $(this).remove();
                RefreshTaskDependency();
            }

            function SaveCellData(This) {
                var thisData = $(This).data('data');
                var isLaborCost = $(This).parent().hasClass('tdlaborcost');
                var isMatBudgetedQty = $(This).parent().hasClass('tdbudgetedqty');

                if (isLaborCost || isMatBudgetedQty) {
                    var data = {
                        ID: '',
                        ModifiedBy: sessionStorage.PeopleID
                    };

                    if (isLaborCost) {
                        data.Amount = CDec2($(This).val())
                    }

                    if (isMatBudgetedQty) {
                        data.Qty = CDec2($(This).val())
                    }

                    if (thisData.length == 0) {
                        thisData.push(data);
                    } else {
                        var copydata = JSON.parse(JSON.stringify(thisData[0]));

                        if (CStr2(thisData[0].ID) == '') {
                            thisData[0] = $.extend(copydata, data);
                        } else {
                            thisData.push(data);
                        }
                    }
                }
            }

            function ContextMenu(elem, e, divid, child) {
                var cmenu = $('<ul>');
                $('#tBodyExcelLike tr td').removeClass('activeCell');
                $('#tBodyExcelLike tr').removeClass('activeRow');
                $(elem).addClass('activeCell');
                $(elem).parent().addClass('activeRow');

                var parentItemNo = $(elem).closest('tr').find('td.tditemno').find('div').text().indexOf('.') == 1 ? $(elem).closest('tr').find('td.tditemno').find('div').text().split('.')[0] : $(elem).closest('tr').find('td.tditemno').find('div').text();
                //var cntItemNoDot = descriptionid.split('.').length - 1;
                var attrAddChild = {}, activeCell = $('#tBodyExcelLike tr td.activeCell > div');

                if (indentno == 5) {
                    attrAddChild = {
                        style: 'background-color: #CCC! important; cursor:not-allowed; border-left:3px solid #B3B3B3; color:#9B9B9B;'
                    }
                }

                $('.context-menu').remove();

                if(isMaterials){
                    cmenu.append($('<a>')
                                .append($('<li>')
                                    .attr(attrAddChild)
                                    .html('<span class="bi bi-link-45deg"></span> Link material to SKU')
                                    .click(function (e) {
                                        e.stopPropagation();
                                        $('.context-menu').remove();
                                        itemAssociateSKUID = $('#tBodyExcelLike tr td.activeCell').find('div').attr('descriptionid');
                                        isPopupOpen = true;
                                        isHasAssociateSKUID = true;
                                        sessionStorage.skuname = $('#tBodyExcelLike tr td.activeCell').find('div').text();
                                        SearchSku(sessionStorage.skuname);
                                    })
                                )
                            )
                            .append($('<a>')
                                .append($('<li>')
                                    .attr(attrAddChild)
                                    .html('<span class="bi bi-trash"></span> Delete linked SKU')
                                    .click(function (e) {
                                        e.stopPropagation();
                                        $('.context-menu').remove();
                                        NewMessageBox('Are you sure you want to remove the linked SKU?', 'Message', [
                                            {
                                                Text: 'Yes', Click: function () {
                                                    $('#tBodyExcelLike tr td.activeCell').find('div').removeAttr('associateskuid skuname');
                                                    $('#tBodyExcelLike tr td.activeCell').removeClass('hasAssociatedSKU');
                                                    $('#tBodyExcelLike tr td.activeCell').next().next().find('div').text('');
                                                    WebToast('Linked SKU deleted', 'top');
                                                }
                                            }, { Text: 'No' }
                                        ]);
                                    })
                                )
                            )             
                } else {
                    cmenu.append($('<a>')
                                .append($('<li>')
                                    .attr(attrAddChild)
                                    .html('<span class="bi bi-plus-lg"></span> Add Sub Task')
                                    .click(function (e) {
                                        e.stopPropagation();
                                        isAddChildType = 'subtask';
                                        $('#lblTitleAddChild').text('Add Sub Task');
                                        if (indentno != 5) {
                                            $('.context-menu').remove();

                                            if ($.trim($('#tBodyExcelLike tr td.activeCell').closest('tr').find('td').find('div').text()) == "") {
                                                NewMessageBox('No item was found', 'Project');
                                                return false;
                                            }

                                            $('#panAddChild').modal('show')
                                            $('#txtChildDescription').focus();
                                        }
                                    })
                                )
                            )
                            .append($('<a>')
                                .append($('<li>')
                                    .html('<span class="bi bi-plus-lg"></span> Add Material(s)')
                                    .click(function (e) {
                                        e.stopPropagation();
                                        isAddChildType = 'material';
                                        $('#lblTitleAddChild').text('Add Materials');
                                        if (indentno != 5) {
                                            $('.context-menu').remove();

                                            if ($.trim($('#tBodyExcelLike tr td.activeCell').closest('tr').find('td').find('div').text()) == "") {
                                                NewMessageBox('No item was found', 'Project');
                                                return false;
                                            }

                                            $('#panAddChild').modal('show')
                                            $('#txtChildDescription').focus();
                                        }
                                        //$('.context-menu').remove();
                                        //itemAssociateSKUID = $('#tBodyExcelLike tr td.activeCell').find('div').attr('descriptionid');
                                        //isPopupOpen = true;
                                        //isHasAssociateSKUID = true;
                                        //sessionStorage.skuname = $('#tBodyExcelLike tr td.activeCell').find('div').text();
                                        //SearchSku(sessionStorage.skuname)
                                    })
                                )
                            )
                            .append($('<a>')
                                .append($('<li>')
                                    .html('<span class="bi bi-trash"></span> Delete Line Item')
                                    .click(function (e) {
                                        e.stopPropagation();
                                        $('.context-menu').remove();

                                        if ($.trim($('#tBodyExcelLike tr td.activeCell').closest('tr').find('td').find('div').text()) == "") {
                                            NewMessageBox('No item to delete', 'Project')
                                            return false;
                                        }

                                        $('#panAddChild').modal('hide')
                                        NewMessageBox('Deleted items will be deactivated and no longer be available for materials requisition or cash disbursement. You may re-activate this line item later on if you wish.' + ' <br><br>' +
                                        'Are you sure you want to delete this line item?', 'Confirmation Dialog', [
                                            {
                                                Text: 'Yes', Click: function () {
                                                    var indexOf = 0;

                                                    $('#tBodyExcelLike tr td.activeCell').closest('tr').each(function () {
                                                        $(this).find('td').find('div').addClass('disableCell');
                                                    })
                                                    .nextAll().each(function () {
                                                        if (isparent == 1) {
                                                            if ($(this).find('td.tditemno').find('div').text().split('.')[indexOf] == descriptionid) {
                                                                $(this).find('td').find('div').addClass('disableCell');
                                                            }
                                                            else {
                                                                return false;
                                                            }
                                                        }
                                                        else {
                                                            if (descriptionid.match(/\./g) == null) {
                                                                return false;
                                                            }

                                                            if (descriptionid.match(/\./g).length == 1) {
                                                                indexOf = 1;
                                                            }
                                                            else if (descriptionid.match(/\./g).length == 2) {
                                                                indexOf = 2;
                                                            }
                                                            else if (descriptionid.match(/\./g).length == 3) {
                                                                indexOf = 3;
                                                            }
                                                            else if (descriptionid.match(/\./g).length == 4) {
                                                                indexOf = 4;
                                                            }
                                                            else if (descriptionid.match(/\./g).length == 5) {
                                                                indexOf = 5;
                                                            }

                                                            if ($(this).find('td.tditemno').find('div').text().split('.')[indexOf] == descriptionid.split('.')[indexOf]) {
                                                                $(this).find('td').find('div').addClass('disableCell');
                                                            }
                                                            else {
                                                                return false;
                                                            }
                                                        }
                                                    })
                                                }
                                            }, {
                                                Text: 'No', Click: function () {

                                                }
                                            }
                                        ])
                                    })
                                )
                            )
                            .append($('<a>')
                                .append($('<li>')
                                    .html('<span class="bi bi-arrow-90deg-left"></span> Undelete Line Item')
                                    .click(function (e) {
                                        e.stopPropagation();
                                        $('.context-menu').remove();
                                        $('#tBodyExcelLike tr td.activeCell').closest('tr').find('td').find('div').removeClass('disableCell');
                                        //$('#btnComputeParentSubTotal').click();
                                    })
                                )
                            )
                            .append($('<a>')
                                .append($('<li>')
                                    .html('&nbsp; &nbsp;&nbsp;Start this task')
                                    .click(function (e) {
                                        e.stopPropagation();
                                        $('.context-menu').remove();
                                        ProjectDatePicker($('#tBodyExcelLike tr td.activeCell').parent().find('td.tddatestarted'))
                                    })
                                )
                            )
                            .append($('<a>')
                                .append($('<li>')
                                    .html('&nbsp; &nbsp;&nbsp;Mark as Completed')
                                    .click(function (e) {
                                        e.stopPropagation();
                                        $('.context-menu').remove();
                                        ProjectDatePicker($('#tBodyExcelLike tr td.activeCell').parent().find('td.tddatecompleted'), function () {
                                            $('#tBodyExcelLike tr td.activeCell').addClass('markascompleted');
                                        })                                        
                                    })
                                )
                            )
                            .append($('<a>')
                                .append($('<li>')
                                    .html('&nbsp; &nbsp;&nbsp;Mark as Not Completed')
                                    .click(function (e) {
                                        e.stopPropagation();
                                        $('.context-menu').remove();
                                        $('#tBodyExcelLike tr td.activeCell').removeClass('markascompleted');
                                        $('#tBodyExcelLike tr td.activeCell').parent().find('td.tddatecompleted > div').text('');
                                    })
                                )
                            )
                }

                cmenu.append($('<a>')
                        .append($('<li>')
                            .html('<span class="bi bi-text-indent-left"></span> Increase Indent')
                            .click(function (e) {
                                e.stopPropagation();                                
                                if (CDec2(activeCell.attr('indentno')) < 5) {
                                    activeCell.attr('indentno', CDec2(activeCell.attr('indentno')) + 1)
                                }
                            })
                        )
                    )
                    .append($('<a>')
                        .append($('<li>')
                            .html('<span class="bi bi-text-indent-right"></span> Decrease Indent')
                            .click(function (e) {
                                e.stopPropagation();
                                if (CDec2(activeCell.attr('indentno')) <= 5 && CDec2(activeCell.attr('indentno')) != 0) {
                                    activeCell.attr('indentno', CDec2(activeCell.attr('indentno')) - 1)
                                }
                            })
                        )
                    )

                $('body').append($('<div>')
                        .append(cmenu)
                        .addClass('context-menu')
                        .css({ top: e.pageY + "px", left: e.pageX + "px" })
                    )
            }

            function SearchSku(name) {
                BrowsePopup('LibraryPIMProduct', 'Search SKU',
                    [
                        {
                            AddUsingWebForm: true,
                            //SelectMultiple: true,
                            Message: 'Please choose specific product or service (SKU) to associate with <b>' + name + '</b>',
                            DisplayText: [{ Column: 'ProductName' }],
                            Click: function (e) {
                                //var tblBodyMultipleSKU = $('#tblBodyMultipleSKU');
                                //tblBodyMultipleSKU.empty();                                        

                                //AjaxSend({
                                //    url: 'BOMProjectExcel/GetSKUsUnitMeasures',
                                //    data: { sku : e },
                                //    success: function (v) {
                                //        var d = v.data;

                                //        for (var i = 0; i < d.length; i++) {
                                //            var tdUom = $('<td>');

                                //            for (var m = 0; m < d[i].Uom.length; m++){
                                //                tdUom.append($('<div class="custom-radio">')
                                //                        .append($('<label class="radio-only">')
                                //                            .append($('<input type="radio" data-theme="c" name="'+ d[i].ID +'" value="' + d[i].Uom[m].UnitMeasureID + '">')
                                //                                .data('data', d[i].Uom[m])
                                //                            )
                                //                        )
                                //                        .append($('<div>').text(d[i].Uom[m].Measurement))
                                //                    )
                                //            }

                                //            tblBodyMultipleSKU.append($('<tr>')
                                //                    .data('data', d[i])
                                //                    .append($('<td>').text(d[i].ProductName))
                                //                    .append(tdUom)
                                //                )
                                //        }
                                //        $('#tblBodyMultipleSKU input[type="radio"]');//.checkboxradio();//.checkboxradio('refresh')
                                //        $('#popMultipleSKUUnitOfMeasures').popup('open');
                                //    }
                                //})
                                arrSelectedSKU = {};
                                arrSelectedSKU.AssociateSKUID = e.ID;
                                arrSelectedSKU.SKUName = e.ProductName;

                                //$('#txtSKUName').text(e.ProductName);
                                //$('#panSKUUnitOfMeasures').popup('open');
                                SKUUnitOfMeasure(e.ID);
                            }
                        }
                    ]
                )
            }

            function CreateExcelTable() {
                var i;

                if ($('#tBodyExcelLike tr').length == 0) {
                    i = 0;
                }
                else {
                    i = parseInt($('#tBodyExcelLike tr:last').attr('id')) + 1;
                }

                $('#tBodyExcelLike').append($('<tr id="' + i + '" rowtype="normal">')
                    .append(
                        $('<td class="tddraggable">')
                        .append($('<span class="bi bi-grip-vertical">'))
                        .css({
                            backgroundColor: '#fff'
                        })
                    )
                    .append(
                        $('<td>')
                        .append($('<div>')
                            .attr('id', 'txtItemNumber_' + i)
                            .css({ width: '100%' })
                            .html(' ')
                            .addClass('tableCellReadOnly')
                            .attr('columnName', 'item')
                            .attr('state', 'add')
                        )
                        .addClass('tableCellDisable tdfreeze tditemno')
                        .css({
                            width: '100px'
                        })
                    )
                    .append(
                        $('<td>')
                        .append($('<div>')
                            .attr({
                                id: 'txtDescription_' + i,
                                isParent: 1,
                                'data-role': 'none',
                                isRequired: 1,
                                columnName: 'description',
                                state: 'add',
                                indentno: 0
                            })
                            .addClass('txtOnlyParent')
                            .html('&nbsp;')
                            .data('data', [])
                        )
                        .addClass('tableCell tddescription tdfreeze')
                        .css({
                            width: '305px',
                            left: '120px',
                            backgroundColor: '#fff'
                        })
                    )
                    .append(
                        $('<td>')
                        .append($('<div>')
                            .attr('id', 'txtQty_' + i)
                            .css({ width: '100%' })
                            .attr('data-role', 'none')
                            .attr('isRequired', 0)
                            .attr('columnName', 'qty')
                            .attr('datatype', 'numeric')
                            .addClass('NumericCell')
                            .attr('state', 'add')
                            .data('data', [])
                            .append(PointPerson())
                        )
                        .addClass('tableCell tdpointperson')
                    )
                    .append(
                        $('<td>')
                        .append($('<div>')
                            .attr('id', 'txtUnit_' + i)
                            .css({ width: '100%' })
                            .html('&nbsp;')
                            .attr('data-role', 'none')
                            .attr('isRequired', 0)
                            .attr('columnName', 'unit')
                            .addClass('TextCell')
                            .attr('state', 'add')
                            .data('data', [])
                            .data('files', [])
                            .data('newphotosformdata', [])
                        )
                        .addClass('tableCell tdremarks')
                        //.addClass('tableCellDisable')
                    )
                    .append(
                        $('<td>')
                        .append($('<div>')
                            .attr('id', 'txtMaterialUnitCost_' + i)
                            .css({ width: '100%' })
                            .html('&nbsp;')
                            .attr('data-role', 'none')
                            .attr('isRequired', 0)
                            .attr('columnName', 'm_unitcost')
                            .addClass('NumericCell')
                            .attr('state', 'add')
                            .data('data', [])
                        )
                        .addClass('tableCell cell-timespend tdcompletionrate')
                        //.addClass('tableCellDisable')
                        //.addClass('btnCanvassMaterials')
                    )
                    .append(
                        $('<td>')
                        .append($('<div>')
                            .attr('id', 'txtMaterialAmount_' + i)
                            .css({ width: '100%' })
                            .attr('data-role', 'none')
                            .attr('isRequired', 0)
                            .attr('columnName', 'm_amount')
                            .addClass('NumericCell')
                            .attr('state', 'add')
                            .data('data', [])
                            //.append(TaskDependency(false))
                        )
                        .addClass('tableCell cell-timespend tddependency')
                        //.addClass('tableCellDisable')
                        //.addClass('btnCanvassMaterials')
                    )
                    .append(
                        $('<td>')
                        .append($('<div>')
                            .attr('id', 'txtLaborUnitCost_' + i)
                            .css({ width: '100%' })
                            .html('&nbsp;')
                            .attr('data-role', 'none')
                            .attr('isRequired', 0)
                            .attr('columnName', 'l_unitcost')
                            .addClass('NumericCell')
                            .attr('state', 'add')
                            .data('data', [])
                        )
                        .addClass('tableCell cell-timespend tdbudgetedduration')
                        .click(function (e) {
                            ProjectDuration($(this));
                        })
                        //.addClass('tableCellDisable')
                        //.addClass('btnCanvassLabor')
                    )
                    .append(
                        $('<td>')
                        .append($('<div>')
                            .attr('id', 'txtLaborAmount' + i)
                            .css({ width: '100%' })
                            .html('&nbsp;')
                            .attr('data-role', 'none')
                            .attr('isRequired', 0)
                            .attr('columnName', 'l_amount')
                            .addClass('NumericCell')
                            .attr('state', 'add')
                            .data('data', [])
                        )
                        .addClass('tableCell cell-timespend tdactualduration')
                        //.addClass('tableCellDisable')
                        //.addClass('btnCanvassLabor')
                    )
                    .append(
                        $('<td>')
                        .append($('<div>')
                            .attr('id', 'txtTotalUnitCost_' + i)
                            .css({ width: '100%' })
                            .html('&nbsp;')
                            .attr('data-role', 'none')
                            .attr('isRequired', 0)
                            .attr('columnName', 't_unitcost')
                            .addClass('NumericCell')
                            .attr('state', 'add')
                            .data('data', [])
                        )
                        .addClass('tableCell cell-timespend tddatestarted')
                        .click(function (e) {
                            ProjectDatePicker($(this))
                        })
                        //.addClass('tableCellDisable')
                        //.addClass('TotalUnitCostHover')
                    )
                    .append(
                        $('<td>')
                        .append($('<div>')
                            .attr('id', 'txtTotalAmount_' + i)
                            .css({ width: '100%' })
                            .html('&nbsp;')
                            .attr('data-role', 'none')
                            .attr('isRequired', 0)
                            .attr('columnName', 't_amount')
                            .addClass('NumericCell')
                            .attr('state', 'add')
                            .data('data', [])
                        )
                        .addClass('tableCell cell-timespend tddatecompleted')
                        .click(function (e) {
                            ProjectDatePicker($(this))
                        })
                        //.addClass('tableCellDisable')
                        //.addClass('TotalAmountHover')
                    )
                    .append(
                        $('<td>')
                        .append($('<div>')
                            .css({ width: '100%' })
                            .attr('isRequired', 0)
                            .addClass('NumericCell')
                            .attr('state', 'add')
                            .data('data', [])
                            .append(ProjectImportance(false))
                        )
                        .addClass('tableCell cell-timespend tdimportance')
                    )
                    .append(
                        $('<td>')
                        .append($('<div>')
                            .css({ width: '100%' })
                            .attr('isRequired', 0)
                            .addClass('NumericCell')
                            .attr('state', 'add')
                            .data('data', [])
                            .append(ProjectUrgency(false))
                        )
                        .addClass('tableCell cell-timespend tdurgency')
                    )
                    .append(
                        $('<td>')
                        .append($('<div>')
                            .css({ width: '100%' })
                            .html('&nbsp;')
                            .attr('isRequired', 0)
                            .addClass('NumericCell')
                            .attr('state', 'add')
                            .data('data', [])
                            .append(ProjectUnitMeasure(false))
                        )
                        .addClass('tableCell cell-materialspend tdunitmeasure tableCellDisable')
                    )
                    .append(
                        $('<td>')
                        .append($('<div>')
                            .css({ width: '100%' })
                            .html('&nbsp;')
                            .attr('isRequired', 0)
                            .addClass('NumericCell')
                            .attr('state', 'add')
                            .data('data', [])
                        )
                        .addClass('tableCell cell-materialspend tdbudgetedqty tableCellDisable')
                    )
                    .append(
                        $('<td>')
                        .append($('<div>')
                            .css({ width: '100%' })
                            .html('&nbsp;')
                            .attr('isRequired', 0)
                            .addClass('NumericCell')
                            .attr('state', 'add')
                            .data('data', [])
                        )
                        .addClass('tableCell cell-materialspend tdreqqty tableCellDisable')
                    )
                    .append(
                        $('<td>')
                        .append($('<div>')
                            .css({ width: '100%' })
                            .html('&nbsp;')
                            .attr('isRequired', 0)
                            .addClass('NumericCell')
                            .attr('state', 'add')
                            .data('data', [])
                        )
                        .addClass('tableCell cell-materialspend tdaccreqqty tableCellDisable')
                    )
                    .append(
                        $('<td>')
                        .append($('<div>')
                            .css({ width: '100%' })
                            .html('&nbsp;')
                            .attr('isRequired', 0)
                            .addClass('NumericCell')
                            .attr('state', 'add')
                            .data('data', [])
                        )
                        .addClass('tableCell cell-materialspend tdaccdeliveryqty tableCellDisable')
                    )
                    .append(
                        $('<td>')
                        .append($('<div>')
                            .css({ width: '100%' })
                            .html('&nbsp;')
                            .attr('isRequired', 0)
                            .addClass('NumericCell')
                            .attr('state', 'add')
                            .data('data', [])
                        )
                        .addClass('tableCell cell-materialspend tdaccusedup tableCellDisable')
                    )
                    .append(
                        $('<td>')
                        .append($('<div>')
                            .css({ width: '100%' })
                            .html('&nbsp;')
                            .attr('isRequired', 0)
                            .addClass('NumericCell')
                            .attr('state', 'add')
                            .data('data', [])
                        )
                        .addClass('tableCell cell-laborspend tdlaborcost')
                    )
                    .append(
                        $('<td>')
                        .append($('<div>')
                            .css({ width: '100%' })
                            .html('&nbsp;')
                            .attr('isRequired', 0)
                            .addClass('NumericCell')
                            .attr('state', 'add')
                            .data('data', [])
                        )
                        .addClass('tableCell cell-laborspend tdaccreqlaborcost')
                    )
                    .append(
                        $('<td>')
                        .append($('<div>')
                            .css({ width: '100%' })
                            .html('&nbsp;')
                            .attr('isRequired', 0)
                            .addClass('NumericCell')
                            .attr('state', 'add')
                            .data('data', [])
                        )
                        .addClass('tableCell cell-laborspend tddisbursedlaborcost')
                    )
                )

                RefreshFreezePanesHeight(i);
            }

        })
        .keydown(function (e) {
            if(IsPopupOpen()){
                return;
            }
            if ($('#divProjectManagement').css('display') == 'none') {
                return;
            }
            if (isAllowedToEditLine) {
                //ESC Key pressed
                if (e.which == 27) {
                    try {
                        $('div[data-role="popup"]').popup('close');
                        isPopupOpen = false;
                        return;
                    } catch (e) {

                    }
                }

                if ($('#pageProjectBudget').css('display') != "none") {
                    if ($('.divNewProjectBudget').css('display') != "none") {
                        var tdActiveCell = $('#tBodyExcelLike tr td.activeCell');
                        currentCellIndex = tdActiveCell.closest('td').index();

                        //navigate active cell using arrow keys
                        if (e.which == 39 || e.which == 9) { // right arrow
                            $('#tBodyExcelLike tr td.activeCell').closest('td').next().addClass('activeCell');
                            $('#tBodyExcelLike tr td.activeCell').closest('td').prev().removeClass('activeCell');
                        }
                        else if (e.which == 37 || (e.which === 9 && e.shiftKey)) { // left arrow
                            $('#tBodyExcelLike tr td.activeCell').closest('td').prev().addClass('activeCell');
                            $('#tBodyExcelLike tr td.activeCell').closest('td').next().removeClass('activeCell');
                        }
                        else if (e.which == 40) { // down arrow
                            $('#tBodyExcelLike tr td.activeCell').closest('tr').next().find('td:eq(' + currentCellIndex + ')').addClass('activeCell');
                            $('#tBodyExcelLike tr td.activeCell').closest('tr').prev().find('td:eq(' + currentCellIndex + ')').removeClass('activeCell');
                        }
                        else if (e.which == 38) { // up arrow
                            $('#tBodyExcelLike tr td.activeCell').closest('tr').prev().find('td:eq(' + currentCellIndex + ')').addClass('activeCell');
                            $('#tBodyExcelLike tr td.activeCell').closest('tr').next().find('td:eq(' + currentCellIndex + ')').removeClass('activeCell');
                        }

                        else {
                            e.stopPropagation();

                            if (isPopupOpen == false) {
                                if (tdActiveCell.find('> div').hasClass('disableCell') ||
                                    tdActiveCell.find('> div').hasClass('tableCellReadOnly') ||
                                    tdActiveCell.hasClass('tableCellDisable') ||
                                    e.which == 20 || // Caps Lock
                                    e.which == 16 || // Shift key
                                    e.which == 17 || // Ctrl Key
                                    e.which == 91 || // Windows Command Key
                                    e.which == 19 || // Pause Break
                                    e.which == 18 || // Alt Key
                                    (e.which >= 144 && e.which <= 145) // Num Lock, Scroll Lock
                                    ) {
                                    return false;
                                }
                                else if (CheckPermissions("Edit Line Items In Project") == false) {
                                    //NewMessageBox('You don\'t have permission to edit the line item(s)', 'Message', [
                                    //    {
                                    //        Text: 'Ok', Click: function () {
                                    //            setTimeout(function () {
                                    //                isAllowedToEditLine = true;
                                    //            }, 500);
                                    //        }
                                    //    }
                                    //]);
                                    //isAllowedToEditLine = false;
                                    return false;
                                }

                                if (tdActiveCell.hasClass('tdlaborcost') && !CheckPermissions("Add/Edit Labor Costing")) {
                                    return false;
                                }

                                if (tdActiveCell.hasClass('tdbudgetedqty') && !CheckPermissions("Add/Edit Material Budgeted Qty")) {
                                    return false;
                                }

                                var thisAttr = tdActiveCell.find('div').attr('id');
                                var thisVal = tdActiveCell.find('div').text();
                                var thisIsRequired = tdActiveCell.find('div').attr('isRequired');
                                var thisColumnName = tdActiveCell.find('div').attr('columnName');
                                var thisDataType = tdActiveCell.find('div').attr('datatype');
                                var thisAssociatedSKUID = tdActiveCell.find('div').attr('associateskuid');
                                var thisSKUName = tdActiveCell.find('div').attr('skuname');
                                var thisState = tdActiveCell.find('div').attr('state');
                                var thisIndentNo = tdActiveCell.find('div').attr('indentno');
                                var thisData = (tdActiveCell.find('div').data('data') || []);

                                if (e.which == 46) {
                                    thisVal = '';
                                }

                                if (thisDataType == "numeric") {
                                    if (e.which != 8) {
                                        if (!NumberOnly(e)) {
                                            return;
                                        }
                                    }
                                }

                                if (tdActiveCell.closest('tr').find('td.tddescription').find('div').text().toLowerCase().indexOf('subtotal') == 0) {
                                    return false;
                                }

                                if (tdActiveCell.find('div').attr('isParent') == 1 || tdActiveCell.find('div').attr('isParent') == undefined) {
                                    tdActiveCell.find('div').replaceWith($('<input>')
                                        .attr('type', 'text')
                                        .attr('value', $.trim(thisVal))
                                        .attr('id', 'typeText-' + thisAttr)
                                        .attr('isParent', 1)
                                        .attr('isRequired', thisIsRequired)
                                        .attr('columnName', thisColumnName)
                                        .attr('datatype', thisDataType)
                                        .attr('associateskuid', thisAssociatedSKUID)
                                        .attr('skuname', thisSKUName)
                                        .attr('state', thisState)
                                        .attr('indentno', thisIndentNo)
                                        .addClass('typeText')
                                        .addClass('inputTextInExcel')
                                        .data('data', thisData)
                                    );
                                }
                                else {
                                    var thisParentID = tdActiveCell.find('div').attr('parentid');

                                    tdActiveCell.find('div').replaceWith($('<input>')
                                        .attr('type', 'text')
                                        .attr('value', $.trim(thisVal))
                                        .attr('id', 'typeText-' + thisAttr)
                                        .attr('isParent', 0)
                                        .attr('parentid', thisParentID)
                                        .attr('isRequired', thisIsRequired)
                                        .attr('columnName', thisColumnName)
                                        .attr('datatype', thisDataType)
                                        .attr('associateskuid', thisAssociatedSKUID)
                                        .attr('skuname', thisSKUName)
                                        .attr('state', thisState)
                                        .attr('indentno', thisIndentNo)
                                        .addClass('typeText')
                                        .addClass('inputTextInExcel')
                                        .data('data', thisData)
                                    );
                                }

                                $('.typeText').focus();
                            }
                            else {
                                return true;
                            }
                        }
                    }
                    else if ($('.divCanvass').css('display') != "none") {
                        var tbody;

                        if (isCanvassMaterials) {
                            tbody = "tBodyCanvassMaterial";
                        }
                        else {
                            tbody = "tBodyCanvassLabor";
                        }

                        currentCellIndex = $('#' + tbody + ' tr td.activeCell').closest('td').index();

                        //navigate active cell using arrow keys
                        if (e.which == 39 || e.which == 9) { // right arrow
                            $('#' + tbody + ' tr td.activeCell').closest('td').next().addClass('activeCell');
                            $('#' + tbody + ' tr td.activeCell').closest('td').prev().removeClass('activeCell');
                        }
                        else if (e.which == 37 || (e.which === 9 && e.shiftKey)) { // left arrow
                            $('#' + tbody + ' tr td.activeCell').closest('td').prev().addClass('activeCell');
                            $('#' + tbody + ' tr td.activeCell').closest('td').next().removeClass('activeCell');
                        }
                        else if (e.which == 40) { // down arrow
                            $('#' + tbody + ' tr td.activeCell').closest('tr').next().find('td:eq(' + currentCellIndex + ')').addClass('activeCell');
                            $('#' + tbody + ' tr td.activeCell').closest('tr').prev().find('td:eq(' + currentCellIndex + ')').removeClass('activeCell');
                        }
                        else if (e.which == 38) { // up arrow
                            $('#' + tbody + ' tr td.activeCell').closest('tr').prev().find('td:eq(' + currentCellIndex + ')').addClass('activeCell');
                            $('#' + tbody + ' tr td.activeCell').closest('tr').next().find('td:eq(' + currentCellIndex + ')').removeClass('activeCell');
                        }

                        else {
                            e.stopPropagation();

                            if (isPopupOpen == false) {
                                var thisVal = $('#' + tbody + ' tr td.activeCell').find('div').text();
                                var thisDataType = $('#' + tbody + ' tr td.activeCell').find('div').attr('datatype');
                                var thisVID = $('#' + tbody + ' tr td.activeCell').find('div').attr('vid');
                                var thisState = $('#' + tbody + ' tr td.activeCell').find('div').attr('state');
                                var thisID = $('#' + tbody + ' tr td.activeCell').find('div').attr('id');

                                if (e.which == 46) {
                                    thisVal = '';
                                }

                                if ($('#' + tbody + ' tr td.activeCell div').hasClass('disableCell') ||
                                    $('#' + tbody + ' tr td.activeCell div').hasClass('tableCellReadOnly') ||
                                    $('#' + tbody + ' tr td.activeCell').hasClass('tableCellDisable') ||
                                    e.which == 20 || // Caps Lock
                                    e.which == 16 || // Shift key
                                    e.which == 17 || // Ctrl Key
                                    e.which == 91 || // Windows Command Key
                                    e.which == 19 || // Pause Break
                                    e.which == 18 || // Alt Key
                                    (e.which >= 144 && e.which <= 145) // Num Lock, Scroll Lock
                                    ) {
                                    return false;
                                }

                                if ($('#' + tbody + ' tr td.activeCell').hasClass('ReadOnly')) {
                                    return false;
                                }

                                $('#' + tbody + ' tr td.activeCell').find('div').replaceWith($('<input>')
                                    .attr('type', 'text')
                                    .attr('value', $.trim(thisVal))
                                    .attr('datatype', thisDataType)
                                    .attr('vid', thisVID)
                                    .attr('state', thisState)
                                    .attr('id', thisID)
                                    .addClass('typeText')
                                    .addClass('inputTextInExcel')
                                    .css({ width: '100px' })
                                );

                                $('.typeText').focus();
                            }
                            else {
                                return true;
                            }
                        }
                    }
                    else if ($('.divTimeline').css('display') != "none") {
                        if (isPopupOpen == false) {
                            var thisVal = $('#tBodyTimeline tr td.activeCell').find('div').text();
                            var thisState = $('#tBodyTimeline tr td.activeCell').find('div').attr('state');

                            if (e.which == 46) {
                                thisVal = '';
                            }

                            if ($('#tBodyTimeline tr td.activeCell div').hasClass('disableCell') ||
                                $('#tBodyTimeline tr td.activeCell div').hasClass('tableCellReadOnly') ||
                                $('#tBodyTimeline tr td.activeCell').hasClass('tableCellDisable') ||
                                e.which == 20 || // Caps Lock
                                e.which == 16 || // Shift key
                                e.which == 17 || // Ctrl Key
                                e.which == 91 || // Windows Command Key
                                e.which == 19 || // Pause Break
                                e.which == 18 || // Alt Key
                                (e.which >= 144 && e.which <= 145) // Num Lock, Scroll Lock
                                ) {
                                return false;
                            }

                            if ($('#tBodyTimeline tr td.activeCell').hasClass('ReadOnly')) {
                                return false;
                            }

                            $('#tBodyTimeline tr td.activeCell').find('div').replaceWith($('<input>')
                                .attr('type', 'text')
                                .attr('value', $.trim(thisVal))
                                .attr('state', thisState)
                                .addClass('typeText')
                                .addClass('inputTextInExcel')
                                .css({ width: '100%' })
                            );

                            $('.typeText').focus();
                        }
                        else {
                            return true;
                        }
                    }
                }
            }                    
        }).click(function () {
            $('.context-menu').remove();
        })

    }) // end 2nd require

}) // end 1st require