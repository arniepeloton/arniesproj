require([
    'common',
    '/Scripts3/model/ItemModifiers.js?crc='
], function () {

    var mModel = App.model.ItemModifiers();

    $(function () {
        
        var isSecurityRoleSummaryPageLoaded = "";
        var isSecurityRoleInfoPageLoaded = "";
        
        var isAddCRMPageLoaded = "";
        var isViewCRMPageLoaded = "";
        
        var isAddActivityQuotaPageLoaded = "";
        var isAddClientStagePageLoaded = "";
        var isAddSurveyCreatorPageLoaded = "";
        var isAddTicketNotePageLoaded = "";
        var isAddFunnelTemplatePageLoaded = "";
        var isCRMPageLoaded = "";
        
        var isSecRoleClicked = false;
        
        var isCRMSettingClicked = false;
        var isSalesTeamClicked = false;
        var isActivityQuotaClicked = false;
        var isTicketNoteClicked = false;
        var isFunnelTemplateClicked = false;
        var isClientStageClicked = false;
        var isSurveyCreatorClicked = false;
        var isDoneClicked = false;
        var pData = {};

        function InitVal() {
            isItemModifierClicked = true;
            isPIMClicked = true;
            isFOWSTClicked = true;
            isDeviceClicked = true;
            isReceiptClicked = true;
            isSecRoleClicked = true;
            isEmployeeClicked = true;
            isSalesReturnClicked = true;
            isDiscountClicked = true;
            isPrinterRoleClicked = true;
            isDecimalClicked = true;
            isPOSClicked = true;
            isSalesOrderClicked = true;
            isRecTransClicked = true;
            isInventoryCountClicked = true;
            isPurchaseOrderClicked = true;
            isHotButtonClicked = true;
            isGroupNameClicked = true;
            isCustomerClicked = true;
            isSupplierClicked = true;
            isCRMSettingClicked = true;
            isSalesTeamClicked = true;
            isActivityQuotaClicked = true;
            isClientStageClicked = true;
            isSurveyCreatorClicked = true;
            isTicketNoteClicked = true;
            isFunnelTemplateClicked = true;
        }

        window.CRMLibraryTable = {
            //Security Roles
            SecurityRoles: function (s) {
                $('#CRMtblBody').empty().append('<tr><td style="text-align:center" colspan="20">Please wait ...</td></tr>');
                pData.SearchString = s || '';

                AjaxSend({
                    url: 'SecurityRole/getSecurityRoleList',
                    data: pData,
                    success: function (vData) {
                        var count = 1;
                        $('#CRMtblBody').empty();
                        if (vData.SecurityRoleData.Rows.length > 0) {
                            for (var p = 0; p < vData.SecurityRoleData.Rows.length; p++) {
                                $('#CRMtblBody').append($('<tr>')
                                                        .attr('id', vData.SecurityRoleData.Rows[p].ID)
                                                        .css('cursor', 'pointer')
                                                        .append($('<td>')
                                                            .text(count)
                                                        )
                                                        .append($('<td>')
                                                            .text(vData.SecurityRoleData.Rows[p].Code)
                                                        )
                                                        .append($('<td>')
                                                            .text(vData.SecurityRoleData.Rows[p].Role)
                                                        )
                                                        .append($('<td>')
                                                            .text(vData.SecurityRoleData.Rows[p].Description)
                                                        )
                                                        .append($('<td>')
                                                            .text(vData.SecurityRoleData.Rows[p].ModifiedBy)
                                                        )
                                                        .append($('<td>')
                                                            .text($.format.date(new Date(vData.SecurityRoleData.Rows[p].LastUpdated), 'MMM dd, yyyy'))
                                                        )
                                                        .data('data', vData.SecurityRoleData.Rows[p])
                                                        .click(function () {
                                                            sessionStorage["AddViewLibBCName"] = "View Security Role";
                                                            window.location.hash = 'UniversalPage';
                                                            sessionStorage["ViewID"] = $(this).attr("ID");
                                                            if (isSecRoleClicked) {
                                                                CRMLibraryTable.ViewSecurityRolePage();
                                                            }
                                                            InitVal();
                                                        })
                                                    );

                                count += 1;
                            }
                        }
                        else {
                            $('#CRMtblBody').append('<tr><td colspan="6" style="text-align:center;">No Result(s) Found</td></tr>');
                        }
                        //$('#CRMtblBody').parent();//.table('refresh');
                    }
                });
            },

            //Search Security Roles
            SecurityRolesSearch: function (s) {

                var pData = {};
                pData.SearchString = s;

                AjaxSend({
                    url: 'SecurityRole/getSecurityRoleList',
                    data: pData,
                    success: function (vData) {
                        $('#CRMlstSearchFound').empty();
                        $('#CRMdivNoResultsFound').hide();

                        if (vData.SecurityRoleData.Rows.length > 0) {
                            for (var p = 0; p < vData.SecurityRoleData.Rows.length; p++) {
                                $('#CRMlstSearchFound').append($('<a href="#" class="list-group-item list-group-item-action">')
                                        .append($('<span>')
                                            .append($('<div style="font-weight:bold;">')
                                                .text(vData.SecurityRoleData.Rows[p].Code)
                                            )
                                            .append($('<div>')
                                                .text('Model: ' + vData.SecurityRoleData.Rows[p].Role)
                                            )
                                            .data('data', vData.SecurityRoleData.Rows[p])
                                            .click(function (e) {
                                                var id = $(this).data('data').ID;
                                                var IsFOWSTExist = false;

                                                $('#CRMtblBody tr').each(function (m_index) {
                                                    if ($.trim($(this).attr('ID')) == id) {
                                                        IsFOWSTExist = true;
                                                    }
                                                });

                                                if (IsFOWSTExist) { alert('This record already exists in your database. To add a new unique record, click on the link below.'); return };
                                            })
                                        )
                                    )
                            }

                        }
                        else {
                            $('#CRMdivNoResultsFound').show();
                        }

                        $('.divAddLibrary, .divLibAddOption').show();
                        $('#CRMlstSearchFound').listview('refresh');
                        Reposition();
                    }
                });
            },

            //Display Add Security Role Page
            AddSecurityRolePage: function () {
                $('#CRMUniversalDiv').html('<div class="divspinner text-center"><div class="spinner-border text-primary text-center" role="status"><span class="visually-hidden"></span></div></div>');
                if (isSecurityRoleInfoPageLoaded == "") {
                    AjaxSend({
                        ExactUrl: '/SecurityRole/SecurityRoleInfo?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile + '&v=' + assetversion,
                        type: 'GET',
                        dataType: 'text',
                        success: function (vData) {
                            isSecurityRoleInfoPageLoaded = vData;
                            $('#CRMUniversalDiv').empty();
                            $('#CRMUniversalDiv').append(vData);//.trigger('create');

                            if (sessionStorage.AddViewLibBCName == "Add Security Role") {
                                ShowSecurityRoleInfo('');
                            }
                            App.Deferred.fulfill('FQ26E5TP76');
                            isGroupNameClicked = true;
                        }
                    });
                }
                else {
                    $('#CRMUniversalDiv').empty();
                    $('#CRMUniversalDiv').append(isSecurityRoleInfoPageLoaded);//.trigger('create');
                    ShowSecurityRoleInfo('');
                }
                InitVal();
            },

            ViewSecurityRolePage: function () {
                var pData = {};

                pData.IsNew = 0;
                pData.ID = sessionStorage.ViewID;

                AjaxSend({
                    url: 'SecurityRole/setCreateNew',
                    data: pData,
                    success: function (vData) {

                        window.IsPageLoaded = false;
                        if ((vData.IsNew || '') == "TRUE") {
                            PromptUserAccess('Security Roles', {
                                OnClickOKSuccess: function () {
                                    location = MergeUrlParameters({
                                        OrgID: sessionStorage.OrgID,
                                        IsMobile: CBool(IsMobile) ? 1 : 0
                                    }, '/SecurityRole/SecurityRoleInfo');
                                }
                            });
                        }
                        else {
                            //window.open(GetPathFormat("/SecurityRole/SecurityRoleSummary"), "_self");
                            $('#CRMUniversalDiv').html('<div class="divspinner text-center"><div class="spinner-border text-primary text-center" role="status"><span class="visually-hidden"></span></div></div>');
                            if (isSecurityRoleSummaryPageLoaded == "") {
                                AjaxSend({
                                    ExactUrl: '/SecurityRole/SecurityRoleSummary',
                                    type: 'GET',
                                    dataType: 'text',
                                    success: function (vData) {
                                        isSecurityRoleSummaryPageLoaded = vData;
                                        $('#CRMUniversalDiv').html('');
                                        $('#CRMUniversalDiv').append(vData);//.trigger('create');

                                        CRMLibrary.View.ShowSecurityInfo();
                                        App.Deferred.fulfill('FQ26E5TP76');
                                    }
                                });
                            }
                            else {
                                $('#CRMUniversalDiv').html('');
                                $('#CRMUniversalDiv').append(isSecurityRoleSummaryPageLoaded);//.trigger('create');
                                CRMLibrary.View.ShowSecurityInfo();
                            }
                        }
                        isGroupNameClicked = true;
                    }
                });
                InitVal();
            },
            
            //Display CRM Settings Page
            CRMSettingsPage: function (s) {
                $('#CRMUniversalDiv').html('<div class="divspinner text-center"><div class="spinner-border text-primary text-center" role="status"><span class="visually-hidden"></span></div></div>');
                if (isCRMPageLoaded == "") {
                    AjaxSend({
                        ExactUrl: '/CRMDashboard/GeneralSettings?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile + '&v=' + assetversion,
                        type: 'GET',
                        dataType: 'text',
                        success: function (vData) {
                            isCRMPageLoaded = vData;
                            $('#CRMUniversalDiv').empty();
                            $('#CRMUniversalDiv').append(vData);//.trigger('create');
                            App.Deferred.fulfill('FQ26E5TP76');
                            InitVal();
                        }
                    });
                }
                else {
                    $('#CRMUniversalDiv').empty();
                    $('#CRMUniversalDiv').append(isCRMPageLoaded);//.trigger('create');
                    CRMSettings.Refresh();
                }
            },

            //Sales Team
            SalesTeam: function (s) {
                $('#CRMtblBody').empty().append('<tr><td style="text-align:center" colspan="20">Please wait ...</td></tr>');

                pData.SearchKeyword = s || '';
                pData.Type = 'team';

                AjaxSend({
                    url: 'crm/GetAllTeams',
                    data: pData,
                    success: function (vData) {
                        var count = 1;
                        $('#CRMtblBody').empty();
                        if (vData.Results.Rows.length > 0) {
                            for (var p = 0; p < vData.Results.Rows.length; p++) {
                                $('#CRMtblBody').append($('<tr>')
                                                        .attr('id', vData.Results.Rows[p].ID)
                                                        .css('cursor', 'pointer')
                                                        .append($('<td>')
                                                            .text(count)
                                                        )
                                                        .append($('<td>')
                                                            .text(vData.Results.Rows[p].TeamName)
                                                        )
                                                        .append($('<td>')
                                                            .text(vData.Results.Rows[p].Description)
                                                        )
                                //                                                        .append($('<td>')
                                //                                                            .text(vData.Results.Rows[p].ModifiedBy)
                                //                                                        )
                                                        .append($('<td>')
                                                            .text($.format.date(new Date(vData.Results.Rows[p].LastUpdated), 'MMM dd, yyyy'))
                                                        )
                                                        .data('data', vData.Results.Rows[p])
                                                        .click(function () {
                                                            sessionStorage["AddViewLibBCName"] = "View Team";
                                                            window.location.hash = 'UniversalPage';
                                                            sessionStorage["ViewID"] = $(this).attr('ID');
                                                            if (isSalesTeamClicked) {
                                                                CRMLibraryTable.ViewCRMPage();
                                                            }
                                                            InitVal();
                                                        })
                                                    );
                                count += 1;
                            }
                        }
                        else {
                            $('#CRMtblBody').append('<tr><td colspan="5" style="text-align:center;">No Result(s) Found</td></tr>')
                        }
                        $('#CRMtblBody').parent();//.table('refresh');
                    }
                });
            },

            //Search Customers List
            SalesTeamSearch: function (s) {

                var pData = {};
                pData.SearchKeyword = s;

                AjaxSend({
                    url: 'crm/GetAllTeams',
                    data: pData,
                    success: function (vData) {
                        $('#CRMlstSearchFound').empty();
                        $('#CRMdivNoResultsFound').hide();

                        if (vData.Results.Rows.length > 0) {
                            for (var p = 0; p < vData.Results.Rows.length; p++) {

                                $('#CRMlstSearchFound').append($('<li>')
                                    .append($('<a>')
                                        .append($('<br><p><span style="font-weight:bold;">')
                                            .text(vData.Results.Rows[p].TeamName)
                                        )
                                //                                        .data('data', vData.Results.Rows[p])
                                //                                        .click(function (e) {
                                //                                            var id = $(this).data('data').ID;
                                //                                            var IsFOWSTExist = false;

                                //                                            $('#CRMtblBody tr').each(function (m_index) {
                                //                                                if ($.trim($(this).attr('ID')) == id) {
                                //                                                    IsFOWSTExist = true;
                                //                                                }
                                //                                            });

                                //                                            if (IsFOWSTExist) { alert('This record already exists in your database. To add a new unique record, click on the link below.'); return };
                                //                                        })
                                    ));
                            }

                        }
                        else {
                            $('#CRMdivNoResultsFound').show();
                        }

                        $('.divAddLibrary, .divLibAddOption').show();
                        $('#CRMlstSearchFound').listview('refresh');
                        Reposition();
                    }
                });
            },

            KnowledgeManagementAndQuestionaire: function (s) {

                var pData = {};
                pData.SearchKeyword = s;

                AjaxSend({
                    url: 'KnowledgeManagementAndQuestionaire/GetAllKnowledgeManagementAndQuestionaire',
                    data: pData,
                    success: function (vData) {
                        $('#CRMlstSearchFound').empty();
                        $('#CRMdivNoResultsFound').hide();

                        if (vData.Results.Rows.length > 0) {
                            for (var p = 0; p < vData.Results.Rows.length; p++) {

                                $('#CRMlstSearchFound').append($('<li>')
                                    .append($('<a>')
                                        .append($('<br><p><span style="font-weight:bold;">')
                                            .text(vData.Results.Rows[p].CaseNo)
                                        )
                                        .append($('<br><p><span style="font-weight:bold;">')
                                            .text(vData.Results.Rows[p].CaseDescription)
                                        )
                                        .append($('<br><p><span style="font-weight:bold;">')
                                            .text(vData.Results.Rows[p].Solution)
                                        )
                                        .append($('<br><p><span style="font-weight:bold;">')
                                            .text(vData.Results.Rows[p].Topic)
                                        )
                                        .append($('<br><p><span style="font-weight:bold;">')
                                            .text(vData.Results.Rows[p].Product)
                                        )
                                        .append($('<br><p><span style="font-weight:bold;">')
                                            .text(vData.Results.Rows[p].Language)
                                        )
                                        .append($('<br><p><span style="font-weight:bold;">')
                                            .text(vData.Results.Rows[p].EditedBy)
                                        )
                                        .append($('<br><p><span style="font-weight:bold;">')
                                            .text(vData.Results.Rows[p].LastUpdated)
                                        )
                                    ));
                            }

                        }
                        else {
                            $('#CRMdivNoResultsFound').show();
                        }

                        $('.divAddLibrary, .divLibAddOption').show();
                        $('#CRMlstSearchFound').listview('refresh');
                        Reposition();
                    }
                });
            },
            //Display Add CRM Page
            AddCRMPage: function () {
                $('#CRMUniversalDiv').html('<div class="divspinner text-center"><div class="spinner-border text-primary text-center" role="status"><span class="visually-hidden"></span></div></div>');
                if (isAddCRMPageLoaded == "") {
                    AjaxSend({
                        ExactUrl: '/CRM/CRMIndex?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile + '&v=' + assetversion,
                        type: 'GET',
                        dataType: 'text',
                        success: function (vData) {
                            isAddCRMPageLoaded = vData;
                            $('#CRMUniversalDiv').empty();
                            $('#CRMUniversalDiv').append(vData);//.trigger('create');
                            App.Deferred.fulfill('FQ26E5TP76');
                            isGroupNameClicked = true;
                        }
                    });
                }
                else {
                    $('#CRMUniversalDiv').empty();
                    $('#CRMUniversalDiv').append(isAddCRMPageLoaded);//.trigger('create');
                }
            },

            //Display Add CRM Page
            ViewCRMPage: function () {
                $('#CRMUniversalDiv').html('<div class="divspinner text-center"><div class="spinner-border text-primary text-center" role="status"><span class="visually-hidden"></span></div></div>');
                if (isAddCRMPageLoaded == "") {
                    AjaxSend({
                        ExactUrl: '/CRM/CRMIndex?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile + '&v=' + assetversion,
                        type: 'GET',
                        dataType: 'text',
                        success: function (vData) {
                            isAddCRMPageLoaded = vData;
                            $('#CRMUniversalDiv').empty();
                            $('#CRMUniversalDiv').append(vData);//.trigger('create');
                            App.Deferred.fulfill('FQ26E5TP76');
                            isGroupNameClicked = true;
                        }
                    });
                }
                else {
                    $('#CRMUniversalDiv').empty();
                    $('#CRMUniversalDiv').append(isAddCRMPageLoaded);//.trigger('create');
                    CRM.ShowCRMData();
                }
            },            

            SearchKnowledgeManagementAndQuestionaire: function (vKeywords) {

                var pData = {};
                pData.SearchKeyword = vKeywords;

                AjaxSend({
                    url: 'KnowledgeManagementAndQuestionaire/GetAllKnowledgeManagementAndQuestionaire',
                    data: pData,
                    success: function (vData) {
                        GetAllLanguages();
                        GetAllProducts();
                        GetAllTopics();
                        if (vData.Results.Rows.length > 0) {
                            var pTable = CreateTable(vData.Results, {
                                Element: $('#CRMtblBody').parent(),
                                attr: {
                                    'class': 'ui-table ui-responsive table-stroke table-stripe ui-table-reflow',
                                    'data-column-btn-theme': 'b'
                                },
                                theadAttr: {
                                    'class': 'custom-theme-f'
                                },
                                AddAllColumns: false,
                                Columns: {
                                    Problem: {
                                        Text: 'Problem'
                                    },
                                    Solution: {
                                        Text: 'Solution'
                                    },
                                    Topic: {
                                        Text: 'Topic'
                                    },
                                    Product: {
                                        Text: 'Product'
                                    },
                                    Language: {
                                        Text: 'Language'
                                    },
                                    EditedBy: {
                                        Text: 'Edited By'
                                    },
                                    LastUpdated: {
                                        Text: 'Last Edit Date',
                                        DataType: 'date',
                                        Format: proj.DateFormat
                                    }
                                },
                                AfterRow: function (e) {
                                    e.Row.css('cursor', 'pointer');
                                    e.Row.click(function (e1) {
                                        var pDR = $($(e1.target).closest('tr')).data('data');
                                        GetSolution(pDR);
                                        $('#panAddKnowledgeManagement').show();
                                        InitVal();
                                    });
                                }
                            });
                            pTable;//.table();
                            pTable;//.trigger('create');

                            var table = $("table").stupidtable({

                                "date": function (a, b) {
                                    // Get these into date objects for comparison.

                                    aDate = new Date(a);
                                    bDate = new Date(b);

                                    return aDate - bDate;
                                }
                            });
                        }
                        else {
                            $('#CRMtblBody').append($('<tr>')
                                                    .append($('<td>')
                                                        .css('text-align', 'center')
                                                        .attr('colspan', '8')
                                                        .text('No Result(s) Found')
                                                    )
                            );

                            $('#CRMdivNoResultsFound').show();

                        }

                    }
                });


            }, // END SearchItemModifiers: function(vKeywords) {

            GetKnowledgeManagementNumber: function () {
                AjaxSend({
                    url: "KnowledgeManagementAndQuestionaire/GetLastKMNumber",
                    success: function (vData) {
                        if (vData.Results.Rows.length > 0) {
                            $('#txtCaseID').val(vData.Results.Rows[0].LastNumber);
                        } else {
                            $('#txtCaseID').val(0);
                        }
                    }
                });
            },

            //Acitivity Quota
            ActivityQuota: function (s) {
                $('#CRMtblBody').empty().append('<tr><td style="text-align:center" colspan="20">Please wait ...</td></tr>');

                pData.SearchKeyword = s || '';

                AjaxSend({
                    url: 'CRMDashboard/GetActivityQuota',
                    data: pData,
                    success: function (vData) {
                        var count = 1;
                        $('#CRMtblBody').empty();
                        if (vData.Results.Rows.length > 0) {
                            for (var p = 0; p < vData.Results.Rows.length; p++) {
                                $('#CRMtblBody').append($('<tr>')
                                                        .attr('id', vData.Results.Rows[p].ID)
                                                        .css('cursor', 'pointer')
                                                        .append($('<td>')
                                                            .text(vData.Results.Rows[p].RowNo)
                                                        )
                                                        .append($('<td>')
                                                            .text(vData.Results.Rows[p].TeamName)
                                                        )                                                        
                                                        .append($('<td>')
                                                            .text((vData.Results.Rows[p].DateFrom == vData.Results.Rows[p].DateTo ? vData.Results.Rows[p].DateFrom : $.format.date(new Date(vData.Results.Rows[p].DateFrom), 'MMM dd, yyyy') + ' to ' + $.format.date(new Date(vData.Results.Rows[p].DateTo), 'MMM dd, yyyy')))
                                                        )
                                                        .append($('<td>')
                                                            .text($.format.date(new Date(vData.Results.Rows[p].LastUpdated), 'MMM dd, yyyy'))
                                                        )
                                                        .append($('<td>')
                                                            .text(vData.Results.Rows[p].CreatedByName)
                                                        )
                                                        .data('data', vData.Results.Rows[p])
                                                        .click(function () {
                                                            sessionStorage["AddViewLibBCName"] = "View Activity Quota";
                                                            window.location.hash = 'UniversalPage';
                                                            sessionStorage["ViewID"] = $(this).attr('ID');
                                                            CRMLibraryTable.ViewActivityQuota();
                                                            InitVal();
                                                        })
                                                    );
                                count += 1;
                            }
                        }
                        else {
                            $('#CRMtblBody').append('<tr><td colspan="5" style="text-align:center;">No Result(s) Found</td></tr>')
                        }
                        $('#CRMtblBody').parent();//.table('refresh');
                    }
                });
            },

            //Display Add Activity Quota
            AddActivityQuotaPage: function () {
                $('#CRMUniversalDiv').html('<div class="divspinner text-center"><div class="spinner-border text-primary text-center" role="status"><span class="visually-hidden"></span></div></div>');
                if (isAddActivityQuotaPageLoaded == "") {
                    AjaxSend({
                        ExactUrl: '/CRMDashboard/ActivityQuota?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile + '&v=' + assetversion,
                        type: 'GET',
                        dataType: 'text',
                        success: function (vData) {
                            isAddActivityQuotaPageLoaded = vData;
                            $('#CRMUniversalDiv').empty();
                            $('#CRMUniversalDiv').append(vData);//.trigger('create');
                            App.Deferred.fulfill('FQ26E5TP76');
                            isActivityQuotaClicked = true;
                            InitVal();
                        }
                    });
                }
                else {
                    $('#CRMUniversalDiv').empty();
                    $('#CRMUniversalDiv').append(isAddActivityQuotaPageLoaded);//.trigger('create');
                    ActivityQuota.LoadData();
                }
            },

            //View Activity Quota Page
            ViewActivityQuota: function () {
                $('#CRMUniversalDiv').html('<div class="divspinner text-center"><div class="spinner-border text-primary text-center" role="status"><span class="visually-hidden"></span></div></div>');
                if (isAddActivityQuotaPageLoaded == "") {
                    AjaxSend({
                        ExactUrl: '/CRMDashboard/ActivityQuota?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile + '&v=' + assetversion,
                        type: 'GET',
                        dataType: 'text',
                        success: function (vData) {
                            isAddActivityQuotaPageLoaded = vData;
                            $('#CRMUniversalDiv').empty();
                            $('#CRMUniversalDiv').append(vData);//.trigger('create');
                            App.Deferred.fulfill('FQ26E5TP76');
                            isActivityQuotaClicked = true;
                            InitVal();
                        }
                    });
                }
                else {
                    $('#CRMUniversalDiv').empty();
                    $('#CRMUniversalDiv').append(isAddActivityQuotaPageLoaded);//.trigger('create');
                    ActivityQuota.ShowData();
                }
            },

            //Acitivity Quota
            ClientStage: function (s) {
                $('#CRMtblBody').empty().append('<tr><td style="text-align:center" colspan="20">Please wait ...</td></tr>');

                pData.SearchKeyword = s || '';

                AjaxSend({
                    url: 'CRMDashboard/GetGroupStage',
                    data: pData,
                    success: function (vData) {
                        var count = 1;
                        $('#CRMtblBody').empty();
                        if (vData.Results.Rows.length > 0) {
                            for (var p = 0; p < vData.Results.Rows.length; p++) {
                                $('#CRMtblBody').append($('<tr>')
                                                        .attr('id', vData.Results.Rows[p].ID)
                                                        .css('cursor', 'pointer')
                                                        .append($('<td>')
                                                            .text(p + 1)
                                                        )
                                                        .append($('<td>')
                                                            .text(vData.Results.Rows[p].Name)
                                                        )
                                                        .append($('<td>')
                                                            .text($.format.date(new Date(vData.Results.Rows[p].LastUpdated), 'MMM dd, yyyy'))
                                                        )
                                                        .append($('<td>')
                                                            .text(vData.Results.Rows[p].CreatedBy)
                                                        )
                                                        .data(vData.Results.Rows[p])
                                                        .click(function () {
                                                            sessionStorage["AddViewLibBCName"] = "View Stage";
                                                            window.location.hash = 'UniversalPage';
                                                            sessionStorage["ViewID"] = $(this).attr('ID');
                                                            sessionStorage["GroupStageName"] = $(this).data().Name;
                                                            CRMLibraryTable.ViewClientStagePage();
                                                            InitVal();
                                                        })
                                                    );
                                count += 1;
                            }
                        }
                        else {
                            $('#CRMtblBody').append('<tr><td colspan="5" style="text-align:center;">No Result(s) Found</td></tr>')
                        }
                        $('#CRMtblBody').parent();//.table('refresh');
                    }
                });
            },

            //Display Add Client Staging
            AddClientStagePage: function () {
                $('#CRMUniversalDiv').html('<div class="divspinner text-center"><div class="spinner-border text-primary text-center" role="status"><span class="visually-hidden"></span></div></div>');
                if (isAddClientStagePageLoaded == "") {
                    AjaxSend({
                        ExactUrl: '/CRMDashboard/ClientStage?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile + '&v=' + assetversion,
                        type: 'GET',
                        dataType: 'text',
                        success: function (vData) {
                            isAddClientStagePageLoaded = vData;
                            $('#CRMUniversalDiv').empty();
                            $('#CRMUniversalDiv').append(vData);//.trigger('create');
                            App.Deferred.fulfill('FQ26E5TP76');
                            isClientStageClicked = true;
                            InitVal();
                        }
                    });
                }
                else {
                    $('#CRMUniversalDiv').empty();
                    $('#CRMUniversalDiv').append(isAddClientStagePageLoaded);//.trigger('create');
                    ClientStage.LoadData();
                }
            },

            //Display View Client Staging
            ViewClientStagePage: function () {
                $('#CRMUniversalDiv').html('<div class="divspinner text-center"><div class="spinner-border text-primary text-center" role="status"><span class="visually-hidden"></span></div></div>');
                if (isAddClientStagePageLoaded == "") {
                    AjaxSend({
                        ExactUrl: '/CRMDashboard/ClientStage?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile + '&v=' + assetversion,
                        type: 'GET',
                        dataType: 'text',
                        success: function (vData) {
                            isAddClientStagePageLoaded = vData;
                            $('#CRMUniversalDiv').empty();
                            $('#CRMUniversalDiv').append(vData);//.trigger('create');
                            App.Deferred.fulfill('FQ26E5TP76');
                            isClientStageClicked = true;
                            InitVal();
                        }
                    });
                }
                else {
                    $('#CRMUniversalDiv').empty();
                    $('#CRMUniversalDiv').append(isAddClientStagePageLoaded);//.trigger('create');
                    ClientStage.ShowData();
                }
            },

            //Survey
            SurveyList: function (s) {
                $('#CRMtblBody').empty().append('<tr><td style="text-align:center" colspan="20">Please wait ...</td></tr>');

                pData.SearchKeyword = s || '';

                return;
                AjaxSend({
                    url: 'CRMDashboard/GetGroupStage',
                    data: pData,
                    success: function (vData) {
                        var count = 1;
                        $('#CRMtblBody').empty();
                        if (vData.Results.Rows.length > 0) {
                            for (var p = 0; p < vData.Results.Rows.length; p++) {
                                $('#CRMtblBody').append($('<tr>')
                                                        .attr('id', vData.Results.Rows[p].ID)
                                                        .css('cursor', 'pointer')
                                                        .append($('<td>')
                                                            .text(p + 1)
                                                        )
                                                        .append($('<td>')
                                                            .text(vData.Results.Rows[p].Name)
                                                        )
                                                        .append($('<td>')
                                                            .text($.format.date(new Date(vData.Results.Rows[p].LastUpdated), 'MMM dd, yyyy'))
                                                        )
                                                        .append($('<td>')
                                                            .text(vData.Results.Rows[p].CreatedBy)
                                                        )
                                                        .data(vData.Results.Rows[p])
                                                        .click(function () {
                                                            sessionStorage["AddViewLibBCName"] = "View Stage";
                                                            window.location.hash = 'UniversalPage';
                                                            sessionStorage["ViewID"] = $(this).attr('ID');
                                                            sessionStorage["GroupStageName"] = $(this).data().Name;
                                                            CRMLibraryTable.ViewClientStagePage();
                                                            InitVal();
                                                        })
                                                    );
                                count += 1;
                            }
                        }
                        else {
                            $('#CRMtblBody').append('<tr><td colspan="5" style="text-align:center;">No Result(s) Found</td></tr>')
                        }
                        $('#CRMtblBody').parent();//.table('refresh');
                    }
                });
            },

            //Display Add Survey
            AddSurveyPage: function () {
                $('#CRMUniversalDiv').html('<div class="divspinner text-center"><div class="spinner-border text-primary text-center" role="status"><span class="visually-hidden"></span></div></div>');
                if (isAddSurveyCreatorPageLoaded == "") {
                    AjaxSend({
                        ExactUrl: '/Survey/SurveyCreator?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile + '&v=' + assetversion,
                        type: 'GET',
                        dataType: 'text',
                        success: function (vData) {
                            isAddSurveyCreatorPageLoaded = vData;
                            $('#CRMUniversalDiv').empty();
                            $('#CRMUniversalDiv').append(vData);//.trigger('create');
                            App.Deferred.fulfill('FQ26E5TP76');
                            isSurveyCreatorClicked = true;
                            InitVal();
                        }
                    });
                }
                else {
                    $('#CRMUniversalDiv').empty();
                    $('#CRMUniversalDiv').append(isAddSurveyCreatorPageLoaded);//.trigger('create');
                    SurveyCreator.LoadData();
                }
            },

            //Ticket Notes
            TicketNotes: function (s) {
                $('#CRMtblBody').empty().append('<tr><td style="text-align:center" colspan="20">Please wait ...</td></tr>');

                pData.SearchKeyword = s || '';

                AjaxSend({
                    url: 'CRMDashboard/GetTicketNotes',
                    data: pData,
                    success: function (vData) {
                        var count = 1;
                        $('#CRMtblBody').empty();
                        if (vData.Results.Rows.length > 0) {
                            for (var p = 0; p < vData.Results.Rows.length; p++) {
                                $('#CRMtblBody').append($('<tr>')
                                                        .attr('id', vData.Results.Rows[p].ID)
                                                        .css('cursor', 'pointer')
                                                        .append($('<td>').css('width', '70px')
                                                            .text(p + 1 + '.')
                                                        )
                                                        .append($('<td>')
                                                            .text(vData.Results.Rows[p].Notes)
                                                        )
                                                        .append($('<td>').css('width', '70px')
                                                            .text((CBool(vData.Results.Rows[p].IsDefault) ? 'Yes' : 'No'))
                                                        )
                                                        .append($('<td>').css('width', '200px')
                                                            .text($.format.date(new Date(vData.Results.Rows[p].LastUpdated), 'MMM dd, yyyy hh:mm a'))
                                                        )
                                                        .append($('<td>').css('width', '150px')
                                                            .text(vData.Results.Rows[p].ModifiedBy)
                                                        )
                                                        .data('data', vData.Results.Rows[p])
                                                        .click(function () {
                                                            sessionStorage["AddViewLibBCName"] = "View Note";
                                                            window.location.hash = 'UniversalPage';
                                                            sessionStorage["ViewID"] = $(this).attr('ID');
                                                            CRMLibraryTable.ViewTicketNote();
                                                            InitVal();
                                                        })
                                                    );
                                count += 1;
                            }
                        }
                        else {
                            $('#CRMtblBody').append('<tr><td colspan="5" style="text-align:center;">No Result(s) Found</td></tr>')
                        }
                        $('#CRMtblBody').parent();//.table('refresh');
                    }
                });
            },

            //View Ticket Notes Page
            ViewTicketNote: function () {
                $('#CRMUniversalDiv').html('<div class="divspinner text-center"><div class="spinner-border text-primary text-center" role="status"><span class="visually-hidden"></span></div></div>');
                if (isAddTicketNotePageLoaded == "") {
                    AjaxSend({
                        ExactUrl: '/CRMDashboard/TicketNotes?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile + '&v=' + assetversion,
                        type: 'GET',
                        dataType: 'text',
                        success: function (vData) {
                            isAddTicketNotePageLoaded = vData;
                            $('#CRMUniversalDiv').empty();
                            $('#CRMUniversalDiv').append(vData);//.trigger('create');
                            App.Deferred.fulfill('FQ26E5TP76');
                            isTicketNoteClicked = true;
                            InitVal();
                        }
                    });
                }
                else {
                    $('#CRMUniversalDiv').empty();
                    $('#CRMUniversalDiv').append(isAddTicketNotePageLoaded);//.trigger('create');
                    TicketNotes.ShowData();
                }
            },

            //Funnel Templates
            FunnelTemplates: function (s) {
                $('#CRMtblBody').empty().append('<tr><td style="text-align:center" colspan="20">Please wait ...</td></tr>');

                pData.SearchKeyword = s || '';
                pData.IsFromSystemAdmin = true;

                AjaxSend({
                    url: 'CRMDashboard/GetFunnelTemplate',
                    data: pData,
                    success: function (vData) {
                        var count = 1;
                        var vRows = vData.data;
                        $('#CRMtblBody').empty();
                        if (vRows.length > 0) {
                            for (var p = 0; p < vRows.length; p++) {
                                $('#CRMtblBody').append($('<tr>')
                                                        .attr('id', vRows[p].ID)
                                                        .css('cursor', 'pointer')
                                                        .append($('<td>').css('width', '70px')
                                                            .text(p + 1 + '.')
                                                        )
                                                        .append($('<td>')
                                                            .text(vRows[p].TemplateName)
                                                        )
                                                        .append($('<td>').css('width', '150px')
                                                            .text($.format.date(new Date(parseInt((vRows[p].LastUpdated).substring(6))), 'MMM dd, yyyy'))
                                                        )
                                                        .append($('<td>').css('width', '150px')
                                                            .text(vRows[p].CreatedByName)
                                                        )                                                        
                                                        .data('data', vRows[p])
                                                        .click(function () {
                                                            sessionStorage["AddViewLibBCName"] = "View Template";
                                                            window.location.hash = 'UniversalPage';
                                                            sessionStorage["ViewID"] = $(this).attr('ID');
                                                            CRMLibraryTable.ViewFunnelTemplate();
                                                            InitVal();
                                                        })
                                                    );
                                count += 1;
                            }
                        }
                        else {
                            $('#CRMtblBody').append('<tr><td colspan="5" style="text-align:center;">No Result(s) Found</td></tr>')
                        }
                        $('#CRMtblBody').parent();//.table('refresh');
                    }
                });
            },

            //View Funnel Template Page
            ViewFunnelTemplate: function () {
                $('#CRMUniversalDiv').html('<div class="divspinner text-center"><div class="spinner-border text-primary text-center" role="status"><span class="visually-hidden"></span></div></div>');
                if (isAddFunnelTemplatePageLoaded == "") {
                    AjaxSend({
                        ExactUrl: '/CRMDashboard/FunnelTemplate?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile + '&v=' + assetversion,
                        type: 'GET',
                        dataType: 'text',
                        success: function (vData) {
                            isAddFunnelTemplatePageLoaded = vData;
                            $('#CRMUniversalDiv').empty();
                            $('#CRMUniversalDiv').append(vData);//.trigger('create');
                            App.Deferred.fulfill('FQ26E5TP76');
                            isFunnelTemplateClicked = true;
                            InitVal();
                        }
                    });
                }
                else {
                    $('#CRMUniversalDiv').empty();
                    $('#CRMUniversalDiv').append(isAddFunnelTemplatePageLoaded);//.trigger('create');
                    FunnelTemplate.ShowData();
                }
            }
        } // END window.CRMLibraryTable = {

        function GetSolution(data) {
            AjaxSend({
                url: 'KnowledgeManagementAndQuestionaire/GetProblemSolution',
                data: { ProblemID: data.ProblemID },
                success: function (vData) {
                    $('#txtProblemDBID').val(data.ProblemID);
                    $('#selTopic').val(data.TopicID).selectmenu('refresh');
                    $('#selSubTopic').val(data.SubTopicID).selectmenu('refresh');
                    $('#selProduct').val(data.ProductID).selectmenu('refresh');
                    $('#selLanguage').val(data.LanguageID).selectmenu('refresh');
                    $('#txtTags').val(data.Tags);
                    $('#txtCaseID').val(data.CaseNo);
                    $('#txtCase').val(data.Problem);
                    GetKMData.DisplaySolution(vData)
                }
            })
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

                    GetAllSubTopics();
                }
            });
        }

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

        function Reposition() {
            $('div[data-role="popup"]').popup().popup("reposition", { positionTo: 'window' });
            $('div[data-role="popup"]').find('input[type="text"]:first').focus();
        }

        function GetAllFowst() {
            AjaxSend({
                url: 'Browser/LibraryFowst',
                success: function (vData) {
                    DisplayFOWST(vData.Results.Rows);
                }
            });
        }

        function DisplayFOWST(vRows) {
            var pLV = $("#lstOrgEmployeeFowst");

            pLV.empty();

            for (var i = 0; i <= vRows.length - 1; i++) {
                //unit measure
                pItem = $('<li>')
                    .attr('ID', vRows[i].ID)
                    .append($('<a>')
                        .attr('ID', vRows[i].ID)
                        .data('data', vRows[i])
                        .click(function (e) {
                            //                            window.location = MergeUrlParameters({
                            //                                OrgID: sessionStorage.OrgID,
                            //                                IsMobile: CBool(IsMobile) ? 1 : 0,
                            //                                EmployeeName: $('#btnOrgEmployee').text(),
                            //                                EmployeeID: ID,
                            //                                FOWSTID: $(this).attr('ID'),
                            //                                FOWSTName: $(this).text()
                            //                            }, 'OrganizationEmployeeAssignWorkSchedule');
                        })
                        .append($('<h3>')
                            .text(vRows[i].FowstName)
                        )
                        .append($('<p>')
                            .text(vRows[i].Address)
                        )
                    )
                    .append($('<a>')
                        .attr('ID', vRows[i].ID)
                        .click(function () {
                            $('#' + $(this).attr('ID')).remove();
                        })
                    )
                pLV.append(pItem)
            }

            pLV.listview('refresh');
        }

        function getCallingCode() {

            AjaxSend({
                url: 'SignUp/getCallingCodeList',
                success: function (vData) {

                    var vRows = vData.CallingCodeData.Rows;

                    $('#selCallingCodeMobile').empty();
                    $('#selCallingCodeTel').empty();

                    $('#selCallingCodeMobile').append(new Option('Please Select Country Code', ''));
                    $('#selCallingCodeTel').append(new Option('Please Select Country Code', ''));
                    for (var p = 0; p <= vRows.length - 1; p++) {
                        $('#selCallingCodeMobile').append(new Option((vRows[p].CallingCode == undefined ? '' : vRows[p].CallingCode) + ' - ' + vRows[p].Name, vRows[p].ID, false, false));
                        $('#selCallingCodeTel').append(new Option((vRows[p].CallingCode == undefined ? '' : vRows[p].CallingCode) + ' - ' + vRows[p].Name, vRows[p].ID, false, false));
                    }

                    $("#selCallingCodeMobile option:contains('63 - Philippines')").attr('selected', 'selected');
                    $("#selCallingCodeTel option:contains('63 - Philippines')").attr('selected', 'selected');

                    getTelcoPrefix();
                    getAreaCode();
                    $('#spanCountryCodeMobile').hide();
                    $('#selCallingCodeMobile').removeClass('form-validation-error');

                    $('#spanCountryCodeTelNo').hide();
                    $('#selCallingCodeTel').removeClass('form-validation-error');


                },
                error: function (vError) {
                    var pDummy;
                }
            });

        }

        function getAreaCode() {

            AjaxSend({
                url: 'SignUp/getAreaCodeList',
                success: function (vData) {

                    var vRows = vData.AreaCodeData.Rows;

                    $('#selAreaCode').empty();

                    $('#selAreaCode').append(new Option('Please Select Area Code', ''));
                    for (var p = 0; p <= vRows.length - 1; p++) {
                        $('#selAreaCode').append(new Option(vRows[p].CallingCode + ' - ' + vRows[p].Area, vRows[p].ID, false, false));
                    }
                },
                error: function (vError) {
                    var pDummy;
                }
            });

        }

        function getTelcoPrefix() {

            AjaxSend({
                url: 'SignUp/getTelcoPrefixList',
                success: function (vData) {

                    var vRows = vData.TelcoPrefixData.Rows;

                    $('#selTelcoPrefix').empty();

                    $('#selTelcoPrefix').append(new Option('Please Select Mobile Number Prefix', ''));
                    for (var p = 0; p <= vRows.length - 1; p++) {
                        $('#selTelcoPrefix').append(new Option(vRows[p].Prefix + ' - ' + vRows[p].Name, vRows[p].ID, false, false));
                    }
                },
                error: function (vError) {
                    var pDummy;
                }
            });

        }

        function ClearSecurityRoleFields() {

            $("#txtSecurityRoleID").val('');
            $("#txtRoleCode").val('');
            $("#txtRole").val('');
            $("#txtRoleDescription").val('');
            $('#chkRoleIsActive').attr("checked", true).checkboxradio("refresh");
        }

        function ShowSecurityRoleInfo(strID) {
            var pLV;
            var pData = {};
            pData.SecurityRoleID = strID;

            ClearSecurityRoleFields();

            AjaxSend({
                url: "SecurityRole/getSecurityRoleInfo",
                data: pData,
                success: function (vData) {

                    if (vData.SecurityRoleData.Rows.length > 0) {

                        $("#txtSecurityRoleID").val(vData.SecurityRoleData.Rows[0].ID || '');
                        $("#txtRoleCode").val(vData.SecurityRoleData.Rows[0].Code || '');
                        $("#txtRole").val(vData.SecurityRoleData.Rows[0].Role || '');
                        $("#txtRoleDescription").val(vData.SecurityRoleData.Rows[0].Description || '');
                        $('#chkRoleIsActive').attr("checked", (vData.SecurityRoleData.Rows[0].IsActive == 1 ? true : false)).checkboxradio("refresh");

                    }

                    //Load POS
                    pData.MainModule = "POS";
                    AjaxSend({
                        url: "SecurityRole/getSecurityRoleModules",
                        data: pData,
                        success: function (vDataRoleModulePOS) {

                            $('#lstPOS').html('').listview('refresh');
                            pLV = $('#lstPOS');
                            LoadSecurityRoleModule(vDataRoleModulePOS.SecurityRoleData.Rows, pLV, 0, strID);

                        }
                    });

                    //Load Receiving and Transfers
                    pData.MainModule = "Receiving and Transfers";
                    AjaxSend({
                        url: "SecurityRole/getSecurityRoleModules",
                        data: pData,
                        success: function (vDataRoleModuleSFA) {

                            $('#lstReceivingAndTransfers').html('').listview('refresh');
                            pLV = $('#lstReceivingAndTransfers');
                            LoadSecurityRoleModule(vDataRoleModuleSFA.SecurityRoleData.Rows, pLV, 0, strID);

                        }
                    });

                    //Load Inventory count
                    pData.MainModule = "Inventory Count";
                    AjaxSend({
                        url: "SecurityRole/getSecurityRoleModules",
                        data: pData,
                        success: function (vDataRoleModuleSFA) {

                            $('#lstInvCount').html('').listview('refresh');
                            pLV = $('#lstInvCount');
                            LoadSecurityRoleModule(vDataRoleModuleSFA.SecurityRoleData.Rows, pLV, 0, strID);

                        }
                    });

                    //Load SFA
                    pData.MainModule = "SFA";
                    AjaxSend({
                        url: "SecurityRole/getSecurityRoleModules",
                        data: pData,
                        success: function (vDataRoleModuleSFA) {

                            $('#lstSFA').html('').listview('refresh');
                            pLV = $('#lstSFA');
                            LoadSecurityRoleModule(vDataRoleModuleSFA.SecurityRoleData.Rows, pLV, 0, strID);

                        }
                    });

                    //Load CRM
                    pData.MainModule = "CRM";
                    AjaxSend({
                        url: "SecurityRole/getSecurityRoleModules",
                        data: pData,
                        success: function (vDataRoleModuleCRM) {

                            $('#lstCRM').html('').listview('refresh');
                            pLV = $('#lstCRM');
                            LoadSecurityRoleModule(vDataRoleModuleCRM.SecurityRoleData.Rows, pLV, 0, strID);

                        }
                    });

                    //Load Dashboard and Reports
                    pData.MainModule = "Dashboard and Reports";
                    AjaxSend({
                        url: "SecurityRole/getSecurityRoleModules",
                        data: pData,
                        success: function (vDataRoleModuleDAR) {

                            $('#lstDashboardsAndReports').html('').listview('refresh');
                            pLV = $('#lstDashboardsAndReports');
                            LoadSecurityRoleModule(vDataRoleModuleDAR.SecurityRoleData.Rows, pLV, 0, strID);

                        }
                    });

                    pData.MainModule = "Requests and Approvals";
                    AjaxSend({
                        url: "SecurityRole/getSecurityRoleModules",
                        data: pData,
                        success: function (vDataRoleModuleDAR) {

                            $('#lstDashboardsAndReports').html('').listview('refresh');
                            pLV = $('#lstDashboardsAndReports');
                            LoadSecurityRoleModule(vDataRoleModuleDAR.SecurityRoleData.Rows, pLV, 0, strID);

                        }
                    });

                    pData.MainModule = "Sales Returns";
                    AjaxSend({
                        url: "SecurityRole/getSecurityRoleModules",
                        data: pData,
                        success: function (vDataRoleModuleDAR) {
                            $('#lstSalesReturns').html('').listview('refresh');
                            pLV = $('#lstSalesReturns');
                            LoadSecurityRoleModule(vDataRoleModuleDAR.SecurityRoleData.Rows, pLV, 0, strID);

                        }
                    });

                    //Load Settings
                    pData.MainModule = "Settings";
                    AjaxSend({
                        url: "SecurityRole/getSecurityRoleModules",
                        data: pData,
                        success: function (vDataRoleModuleSettings) {

                            $('#lstSettings').html('').listview('refresh');
                            pLV = $('#lstSettings');
                            LoadSecurityRoleModule(vDataRoleModuleSettings.SecurityRoleData.Rows, pLV, 0, strID, true);

                        }
                    });


                    //Load MDM
                    pData.MainModule = "MDM";
                    AjaxSend({
                        url: "SecurityRole/getSecurityRoleModules",
                        data: pData,
                        success: function (vDataRoleModuleMDM) {

                            $('#lstMDM').html('').listview('refresh');
                            pLV = $('#lstMDM');
                            LoadSecurityRoleModule(vDataRoleModuleMDM.SecurityRoleData.Rows, pLV, 0, strID, true);

                        }
                    });



                }
            });

        };

        function LoadSecurityRoleModule(vRows, pLV, intCurIndex, strRoleID, isClear, vIndentation) {

            var pItem;
            var strindentation = (vIndentation == undefined ? "" : vIndentation);

            if (intCurIndex > vRows.length - 1) {
                return;
            }

            if (isClear) {
                pLV.html('');
            }

            pItem = $('<fieldset class="ui-grid-a">')
                        .append($('<div class="ui-block-a" style="height:38px; width: 70%;">')
                            .append($('<label>')
                                .attr('id', "lbl" + vRows[intCurIndex].ModuleID)
                                .data('Data', vRows[intCurIndex])
                                .html(strindentation + vRows[intCurIndex].ModuleName)
                            )
                        );

            if (vRows[intCurIndex].IsHeader == 0) {
                pItem = pItem
                            .append($('<div class="ui-block-b" style="height:38px; width: 30%;">')
                            .append($('<fieldset data-role="controlgroup" data-type="horizontal" data-mini="true" style="float:right; margin-top:0px;">')
                                .append($('<input type="radio">')
                                    .attr('id', "rdoYes" + vRows[intCurIndex].ModuleID)
                                    .attr('name', "rdo" + vRows[intCurIndex].ModuleID)
                                    .attr('checked', (vRows[intCurIndex].IsHasAccess == 1 ? true : false))
                                )
                                .append($('<label>')
                                    .attr('for', "rdoYes" + vRows[intCurIndex].ModuleID)
                                    .text('Yes')
                                )
                                .append($('<input type="radio">')
                                    .attr('id', "rdoNo" + vRows[intCurIndex].ModuleID)
                                    .attr('name', "rdo" + vRows[intCurIndex].ModuleID)
                                    .attr('checked', (vRows[intCurIndex].IsHasAccess == 0 ? true : false))
                                )
                                .append($('<label>')
                                    .attr('for', "rdoNo" + vRows[intCurIndex].ModuleID)
                                    .text('No')
                                )
                            )
                        );

                pItem = $('<li>')
                    .append(pItem)

                pLV.append(pItem);
                pLV;//.trigger("create");
                pLV.listview('refresh');

                LoadSecurityRoleModule(vRows, pLV, intCurIndex + 1, strRoleID, false, strindentation);

            } else {

                pItem = $('<li>')
                                .append(pItem)

                pLV.append(pItem);
                pLV;//.trigger("create");
                pLV.listview('refresh');

                var pDataChild = {};
                pDataChild.SecurityRoleID = strRoleID;
                pDataChild.SecurityModuleHeaderID = vRows[intCurIndex].ModuleID;

                AjaxSend({
                    url: "SecurityRole/getSecurityRoleModulesChild",
                    data: pDataChild,
                    success: function (vDataRoleModuleChild) {

                        if (vDataRoleModuleChild.SecurityRoleData.Rows.length > 0) {
                            LoadSecurityRoleModule(vDataRoleModuleChild.SecurityRoleData.Rows, pLV, 0, strRoleID, false, strindentation + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;");
                        }

                        LoadSecurityRoleModule(vRows, pLV, intCurIndex + 1, strRoleID, false, strindentation);
                    }
                });
            }

        };

        function GetDecimalSettings() {
            AjaxSend({
                url: 'Decimals/GetDecimalSettings',
                data: { OrgID: sessionStorage.OrgID },
                success: function (vData) {
                    if (vData.Results.Rows.length > 0) {
                        DisplayGetDecimalSettings(vData.Results.Rows);
                    }
                }
            });
        }

        function DisplayGetDecimalSettings(vRows) {
            ID = vRows[0].ID;

            if (parseInt(vRows[0].IsAllowDecimal) == 1) {

                IsAllowDecimal = 1;

                $('#selSlider').val('optAllowYes');
                $('#dvIsAllowDecimal').css('visibility', 'visible');
                $('#dvIsAllowDecimal').css('display', 'block');

                if (parseInt(vRows[0].AllowDecimalMode) == 0) {
                    $("#rcAllowAllDecimals").prop("checked", true).checkboxradio("refresh");
                    $("#rcAllowOnSelected").prop("unchecked", true).checkboxradio("refresh");

                    $('#dvIsAllowOnSelectedCategories').css('visibility', 'hidden');
                    $('#dvIsAllowOnSelectedCategories').css('display', 'none');

                    DecimalMode = 0;
                }
                else {
                    $("#rcAllowOnSelected").prop("checked", true).checkboxradio("refresh");
                    $("#rcAllowAllDecimals").prop("unchecked", true).checkboxradio("refresh");

                    $('#dvIsAllowOnSelectedCategories').css('visibility', 'visible');
                    $('#dvIsAllowOnSelectedCategories').css('display', 'block');

                    DecimalMode = 1;
                }
            }
            else {

                IsAllowDecimal = 0;

                $('#selSlider').val('optAllowNo');
                $('#dvIsAllowDecimal').css('visibility', 'hidden');
                $('#dvIsAllowDecimal').css('display', 'none');
            }

            try {
                $('#selSlider').slider('refresh');
            }
            catch (e) {
                $('#selSlider').slider();
            }
        }

        function GetAllFeatures() {
            AjaxSend({
                url: 'Decimals/GetAllProductFeatures',
                data: { OrgID: sessionStorage.OrgID },
                success: function (vData) {
                    DisplayProductFeatures(vData.Results.Rows);
                }
            });
        }

        function DisplayProductFeatures(vRows) {
            var pLV = $('#tbCategories');
            var IsSelected = false;

            for (i = 0; i <= vRows.length - 1; i++) {
                IsSelected = false;

                var IsAllow = vRows[i].IsAllowDecimal;

                pItem = $('<tr>')
                            .append($('<td>')
                                .text(vRows[i].Label)
                            )
                            .append($('<td>')
                                .css('text-align', 'center')
                                .append($('<select>')
                                    .attr('ID', i)
                                    .attr('data', vRows[i].Label)
                                    .attr('data-role', 'slider')
                                    .append($('<option>')
                                        .attr('value', 'No')
                                        .text('No')
                                    )
                                    .append($('<option>')
                                        .attr('value', 'Yes')
                                        .text('Yes')
                                    )
                                )
                            )


                pLV.append(pItem);

                try {
                    if (IsAllow == 1) {
                        $('#' + i).val('Yes').slider('refresh');
                    }
                    else {
                        $('#' + i).slider('refresh');
                    }

                }
                catch (e) {
                    $('#' + i).slider();
                }
            }

            try {
                $('#lstCategories').listview('refresh');
            }
            catch (e) {
                $('#lstCategories').listview();
            }
        }

        function SaveOrganizationCustomer(pData2) {
            var pData = [];
            var Products = [];

            pData = pData2;

            AjaxSend({
                url: "Organization/CheckOrganizationCustomer",
                data: pData,
                success: function (vData) {

                    if (vData.OrgCustomerData.Rows.length > 0) {
                        $('#panAddLibrary').popup('close');
                        $('#TitleOrgEmp').text('Customer Exist');
                        $('#MsgOrgEmp').text('Customer being added already exist.');
                        $('#panOrgEmpMessage').popup('open');
                        $('#btnOKOrgEmp').click(function (e) {
                            e.preventDefault();
                            $('#panOrgEmpMessage').popup('close');
                            $('#panAddLibrary').popup('open');
                        })
                    }
                    else {
                        AjaxSend({
                            url: "Organization/SaveOrganizationCustomer",
                            data: pData,
                            success: function (vSaveData) {

                                if (vSaveData.OrgCustomerData == "ActivationDateError") {
                                    $('#panAddLibrary').popup('close');
                                    $('#TitleOrgEmp').text('Activation Date');
                                    $('#MsgOrgEmp').text('Please enter valid activation date.');
                                    $('#panOrgEmpMessage').popup('open');
                                    $('#btnOKOrgEmp').click(function (e) {
                                        e.preventDefault();
                                        $('#panOrgEmpMessage').popup('close');
                                        $('#panAddLibrary').popup('open');
                                    })
                                }
                                else if (vSaveData.OrgCustomerData == "DeactivationDateError") {
                                    $('#panAddLibrary').popup('close');
                                    $('#TitleOrgEmp').text('Deactivation Date');
                                    $('#MsgOrgEmp').text('Please enter valid deactivation date.');
                                    $('#panOrgEmpMessage').popup('open');
                                    $('#btnOKOrgEmp').click(function (e) {
                                        e.preventDefault();
                                        $('#panOrgEmpMessage').popup('close');
                                        $('#panAddLibrary').popup('open');
                                    })
                                }
                                else {
                                    CRMLibraryTable.CustomerList();
                                    $('#panAddLibrary').popup('close');
                                }

                            }
                        });

                    }

                }
            });
        };

        function getOrgAddTradeName() {

            AjaxSend({
                url: 'Organization/getOrgTradeNameList',
                success: function (vData) {

                    var vRows = vData.OrganizationData.Rows;

                    $('#selOrgFOWSTTradeName').empty();

                    $('#selOrgFOWSTTradeName').append(new Option('Please Select', ''));
                    for (var p = 0; p <= vRows.length - 1; p++) {
                        $('#selOrgFOWSTTradeName').append(new Option(vRows[p].BusinessTradeName, vRows[p].ID, false, false));
                    }
                },
                error: function (vError) {
                    var pDummy;
                }
            });

        }

        function getOrgAddFowstType() {

            AjaxSend({
                url: 'Browser/LibraryFowstType',
                success: function (vData) {
                    var vRows = vData.Results.Rows;

                    $('#selOrgFOWSTType').empty();

                    $('#selOrgFOWSTType').append(new Option('Please Select', ''));
                    for (var p = 0; p <= vRows.length - 1; p++) {
                        $('#selOrgFOWSTType').append(new Option(vRows[p].Name, vRows[p].ID, false, false));
                    }
                },
                error: function (vError) {
                    var pDummy;
                }
            });
        }

        function getOrgAddWarehouseType() {
            AjaxSend({
                url: 'Browser/LibraryFOWSTWarehouseType',
                success: function (vData) {
                    var vRows = vData.Results.Rows;

                    $('#selOrgFOWSTWarehouseType').empty();

                    $('#selOrgFOWSTWarehouseType').append(new Option('Please Select', ''));
                    for (var p = 0; p <= vRows.length - 1; p++) {
                        $('#selOrgFOWSTWarehouseType').append(new Option(vRows[p].Name, vRows[p].ID, false, false));
                    }
                },
                error: function (vError) {
                    var pDummy;
                }
            });
        }

        function getOrgAddRetailStoreType() {
            AjaxSend({
                url: 'Browser/LibraryFOWSTRetailStoreType',
                success: function (vData) {
                    var vRows = vData.Results.Rows;

                    $('#selOrgFOWSTRetailStoreType').empty();

                    $('#selOrgFOWSTRetailStoreType').append(new Option('Please Select', ''));
                    for (var p = 0; p <= vRows.length - 1; p++) {
                        $('#selOrgFOWSTRetailStoreType').append(new Option(vRows[p].Name, vRows[p].ID, false, false));
                    }
                },
                error: function (vError) {
                    var pDummy;
                }
            });
        }

        function GetAllCategories(printerID) {
            data = { OrgID: sessionStorage.OrgID };

            AjaxSend({
                url: 'DevicePrinter/GetAllCategories',
                data: data,
                success: function (vData) {
                    if (vData.Results.Rows.length > 0) {

                        if (printerID != '') {
                            AjaxSend({
                                url: 'DevicePrinter/GetDevicePrinterCategories',
                                data: { PrinterID: printerID },
                                success: function (vDataCat) {

                                    if (vDataCat.Results.Rows.length > 0) {

                                        DisplayAllCategories(vData.Results.Rows, vDataCat.Results.Rows, printerID);
                                    } else {
                                        DisplayAllCategories(vData.Results.Rows, '', printerID);
                                    }
                                }
                            });
                        } else {
                            DisplayAllCategories(vData.Results.Rows, '', '');
                        }

                    }
                }
            });
        }

        var arrCategory = [];

        function DisplayAllCategories(vRows, vRowsCat, printerID) {
            var pLV = $("#tblCategories");

            $('#tblCategories').empty();

            var m_Ctr = 0;
            var categories = [];

            if (vRowsCat != '') {
                for (var j = 0; j <= vRowsCat.length - 1; j++) {
                    categories[j] = vRowsCat[j].CategoryName;
                }

            }


            for (var i = 0; i <= vRows.length - 1; i++) {

                var checked = undefined;

                if ((i % 2) == 0) {
                    pItem = $('<tr>')
                }

                if (categories.indexOf(vRows[i].Category) >= 0) {
                    checked = true;
                    pItem = pItem
                        .append($('<td>')
                            .append($('<input type="checkbox" />')
                                .attr('ID', i + 'chb')
                                .attr('checked', checked)
                                .attr('data-mini', 'true')
                                .data('data', vRows[i].Category)
                                .click(function () {
                                    if ($(this).is(':checked')) {
                                        AjaxSend({
                                            url: 'DevicePrinter/IsCategoryExist',
                                            data: { PrinterID: printerID, CategoryName: $(this).data('data') },
                                            success: function (vDataCat) {

                                                if (vDataCat.Results.Rows.length > 0) {
                                                    MessageBox('This category already belongs to a different printer role. Saving this will overwrite the previous setting.', 'Printer Role');

                                                }
                                            }
                                        });
                                    }
                                })

                            )
                            .append($('<label>')
                                .attr('for', i + 'chb')
                                .text(vRows[i].Category)
                            )
                        )//td                    


                    pLV.append(pItem);

                    try {
                        $('#' + i + 'chb').checkboxradio('refresh');
                    } catch (ex) {
                        $('#' + i + 'chb').checkboxradio();
                    }
                } else {
                    pItem = pItem
                        .append($('<td>')
                            .append($('<input type="checkbox" />')
                                .attr('ID', i + 'chb')
                                .attr('checked', checked)
                                .attr('data-mini', 'true')
                                .data('data', vRows[i].Category)
                                .click(function () {
                                    if ($(this).is(':checked')) {
                                        AjaxSend({
                                            url: 'DevicePrinter/IsCategoryExist',
                                            data: { PrinterID: printerID, CategoryName: $(this).data('data') },
                                            success: function (vDataCat) {

                                                if (vDataCat.Results.Rows.length > 0) {
                                                    MessageBox('This category already belongs to a different printer role. Saving this will overwrite the previous setting.', 'Printer Role');

                                                }
                                            }
                                        });
                                    }
                                })

                            )
                            .append($('<label>')
                                .attr('for', i + 'chb')
                                .text(vRows[i].Category)
                            )
                        )//td                    


                    pLV.append(pItem);

                    try {
                        $('#' + i + 'chb').checkboxradio('refresh');
                    } catch (ex) {
                        $('#' + i + 'chb').checkboxradio();
                    }
                }

            } //for

            try {
                $('#lstCategoriesContainer').listview('refresh');
            }
            catch (e) {
                $('#lstCategoriesContainer').listview();
            }
        }

        function LoadOrgFOWSTProductList(vRows) {
            var pLV = $('#lstOrgFOWSTProductList');
            var pItem;

            $("#divOrgFOWSTProductRecordCount").text(" Number of record(s) found: " + vRows.length);

            pLV.html('');
            for (var p = 0; p <= vRows.length - 1; p++) {
                pItem = $('<li>')
                        .append($('<a>')
                            .attr('ID', vRows[p].ID)
                            .click(function (e) {
                                e.preventDefault();
                                $.mobile.changePage('#pageOrgFOWSTProductInfo');
                                ShowOrgFOWSTProductInfo($(this).attr('ID'));
                            })
                            .append($('<h3>')
                                .attr("style", "white-space:normal")
                                .text(vRows[p].ProductName)
                            )
                        );
                pLV.append(pItem);
            };

            pLV.listview('refresh');
        };

        function ShowReceiptSettingsInfo(strID) {
            var pData = {};
            pData.OrganizationID = strID;

            //ClearReceiptSettingsFields();

            AjaxSend({
                url: "ReceiptSettings/getReceiptSettingsInfo",
                data: pData,
                success: function (vData) {

                    if (vData.ReceiptSettingsData.Rows.length > 0) {

                        $("#txtReceiptSetInfoID").val(vData.ReceiptSettingsData.Rows[0].ID || '');

                        $("#txtReceiptSetInfoOrganization").val(vData.ReceiptSettingsData.Rows[0].CompanyName || '');
                        $("#cmbReceiptSetInfoOrgTradeName").val(vData.ReceiptSettingsData.Rows[0].OrgTradeNameID || '').selectmenu('refresh');
                        $("#cmbReceiptSetInfoOrgPaperSize").val(vData.ReceiptSettingsData.Rows[0].PaperSize || '').selectmenu('refresh');
                        $("#txtReceiptSetInfoTIN").val(vData.ReceiptSettingsData.Rows[0].TIN || '');

                        //                        AjaxSend({
                        //                            url: "ReceiptSettings/getMessageList",
                        //                            data: pData,
                        //                            success: function (vDataMessage) {

                        //                                LoadReceiptSetMessage(vDataMessage.ReceiptSettingsData.Rows);

                        //                            }
                        //                        });

                        AjaxSend({
                            url: "ReceiptSettings/getPlacesList",
                            data: pData,
                            success: function (vDataPlaces) {

                                LoadReceiptSetPlacesList(vDataPlaces.ReceiptSettingsData.Rows);

                            }
                        });

                    }
                }
            });

        };

        function LoadReceiptSetMessage(vRows) {

            for (var p = 0; p <= vRows.length - 1; p++) {

                arReceiptSetMessage = new Array();
                arReceiptSetMessage[intReceiptSetMessageRecID] = vRows[p].ID;
                arReceiptSetMessage[intReceiptSetMessageFooter] = vRows[p].MessageFooter;
                arReceiptSetMessage[intReceiptSetSortOrder] = vRows[p].SortOrder;
                arReceiptSetMessage[intReceiptSetMessageState] = "";

                sessionStorage["ReceiptSetMessage-" + p] = JSON.stringify(arReceiptSetMessage);

            };

            intReceiptSetMessageCntr = vRows.length;
            LoadReceiptSetMessageList();

        };

        function LoadReceiptSetMessageList() {
            var pLV = $('#lstReceiptSetInfoMessages');
            var pItem;

            pLV.html('');
            for (var i = 0; i <= intReceiptSetMessageCntr; i++) {

                if (sessionStorage["ReceiptSetMessage-" + i] != undefined) {

                    arReceiptSetMessage = new Array();
                    arReceiptSetMessage = JSON.parse(sessionStorage["ReceiptSetMessage-" + i]);

                    if (arReceiptSetMessage[intReceiptSetMessageState] != "Remove") {

                        pItem = $('<li id="lst' + i + '">')
                                    .append($('<a>')
                                    .attr('ID', i)
                                    .data('Data', arReceiptSetMessage)
                                    .click(function (e) {
                                        e.preventDefault();
                                        ShowReceiptSetMessageInfo($(this).data('Data'), $(this).attr('ID'));
                                    })
                                    .append($('<h3>')
                                        .attr("style", "white-space:normal")
                                        .text(arReceiptSetMessage[intReceiptSetMessageFooter])
                                    )
                            );


                        pLV.append(pItem);

                    }

                }
            }

            pLV.listview('refresh');
        };

        function LoadReceiptSetPlacesList(vRows) {
            var pLV = $('#fldsetPlaces');
            var pItem;

            intReceiptSetPlacesCntr = vRows.length - 1;

            pLV.html('');
            for (var i = 0; i <= vRows.length - 1; i++) {

                pItem = $('<input>')
                                .attr("type", 'checkbox')
                                .attr('id', 'chkPlaces-' + i)
                                .attr('checked', ((vRows[i].ID || '') == '' ? false : true))
                                .data('Data', vRows[i])
                            .append($('<label>')
                                .attr('for', 'chkPlaces-' + i)
                                .text((vRows[i].BranchName || 'No Branch Name'))
                            );

                pLV.append(pItem);
            }

            $('#divReceiptSettingsPlaces');//.trigger("create");
        };

    })

})