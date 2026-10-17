require([
    'common'
], function () {
    var htmlLeadGen = '';
    var htmlCac = '';
    var htmlCacPayments = '';
    var htmlSalesGrinder = '';
    var htmlActivityReport = '';
    var arrTodoList = [];
    var opentodoStart = 11, opentodoEnd = 20, isFinishedOpenToDo = false;
    var openschedStart = 11, openschedEnd = 20, isFinishedOpenSched = false;

    $(document)
    .ready(function () {
        CheckSession()

        window.GetDashboardRequiredData = function () {
            GetDashboardRequiredData();
        }

        $('body')
        .on('click', '.btnPanel', function () {
            $('#divPanelMenu').toggle('fast').css('overflow', 'auto')
        })
        .on('click', '#btnPeopleMenu', function () {
            $('#popupPeopleMenu').toggle('fast')
        })
        .on('click', '#btnViewProfile', function () {
            AjaxSend({
                url: 'Login3/TestCookies',
                success: function (vData) {
                    //                            window.location = GetPathFormat("PeopleInfoSummary");
                    if (OfflineMode)
                        window.location = '/People/PeopleInfoSummary?IsMobile=1';
                    else
                        window.location = '/People/PeopleInfoSummary?p=crm';
                    //                            window.open(GetPathFormat("PeopleInfoSummary"), "_self");
                },
                error: function (vError) {
                    MessageBox("This feature requires internet connection.",
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
        })
        .on('click', '#btnLogOut', function () {
            SignOut();
        })
        .on('click', '#btnDBSettings', function () {
            location = MergeUrlParameters({
                OrgID: sessionStorage.OrgID,
                IsMobile: CBool(IsMobile) ? 1 : 0
            }, '/CRMDashboard/SystemAdministration');
            //$('#divPanelMenu').toggle('slide')
        })
        .on('click', '#btnListDashboard', function () {
            $('#divMainDB').pageshow();
            BreadCrumbs('.ItemsBreadCrumbs',
                [{ Text: 'Dashboard'}]
            );
            opentodoStart = 11; opentodoEnd = 20; isFinishedOpenToDo = false;
            openschedStart = 11; openschedEnd = 20; isFinishedOpenSched = false;
            GetDashboardRequiredData();
        })
        .on('click', '#btnBackDBOpenToDo', function () {
            $('#divMainDB').pageshow();
            GetDashboardRequiredData();
        })
        .on('click', '#btnListLeadGen', function () {
            $('.ItemsBreadCrumbs').text('');
            AddBreadCrumb({
                    Text: 'Lead Generation', Click: function () {
                        $('#pageMainDashboard').pageshow();
                    }
            }, '.ItemsBreadCrumbs'
            );
            GetPageLeadGen();
        })
        .on('click', '#btnListCac', function () {
            //location = MergeUrlParameters({
            //    OrgID: sessionStorage.OrgID,
            //    IsMobile: CInt2(IsMobile)
            //}, '/CreditAndCollection')
            //return
            BreadCrumbs('.ItemsBreadCrumbs',
                [{ Text: 'Credit And Collection' }]
            );
            GetPageCac();
        })
        .on('click', '#btnListCacPayments', function () {
            //location = MergeUrlParameters({
            //    OrgID: sessionStorage.OrgID,
            //    IsMobile: CInt2(IsMobile)
            //}, '/CreditAndCollection')
            //return
            BreadCrumbs('.ItemsBreadCrumbs',
                [{ Text: 'Customer Payments' }]
            );
            GetPageCacPayments();
        })
        .on('click', '#btnListSalesGrinder', function () {
            NewMessageBox('Sales Grinder is under construction.')
            return;
            $('#divPanelMenu').toggle('fast')
            $('#pageSalesGrinder').pageshow();
            BreadCrumbs('.ItemsBreadCrumbs',
                [{ Text: 'Sales Grinder'}]
            );
            //GetPageSalesGrinder();
        })
        .on('click', '#btnRequestData', function () {
            RequestData();
        })
        .on('keydown', '#txtSearchOpenToDo', function (e) {
            if (e.keyCode == 13 && $.trim($(this).val()) != "") {
                SearchOpenToDo($.trim($(this).val()))
            }
        })
        .on('click', '#btnOpenToDoSearch', function () {
            $('.divopentodosearch').pageshow();
            $('#txtSearchOpenToDo').val('').focus();
            $('#lstOpenToDoSearch').empty();
        })
        .on('click', '#btnBackOpenToDoSearch', function () {
            $('.divopentodolist').pageshow();
        })
        .on('keydown', '#txtSearchScheduledTask', function (e) {
            if (e.keyCode == 13 && $.trim($(this).val()) != "") {
                SearchScheduledTasks($.trim($(this).val()))
            }
        })
        .on('click', '#btnScheduledSearch', function () {
            $('.divscheduledtasksearch').pageshow();
            $('#txtSearchScheduledTask').val('').focus();
            $('#lstScheduleSearch').empty();
        })
        .on('click', '#btnBackScheduleSearch', function () {
            $('.divscheduledtasklist').pageshow();
        })
        .on('click', '#btnShowMoreOpenToDo', function () {
            if ($(this).text() == 'Show More') {
                if (isFinishedOpenToDo == false) {
                    $(this).text('Loading ...').attr('disabled', true);
                    AjaxSend({
                        url: 'CRMDashboard/GetOpenToDo',
                        data: { Start: opentodoStart, End: opentodoEnd },
                        success: function (vData) {
                            $('#btnShowMoreOpenToDo').text('Show More').attr('disabled', false);
                            opentodoStart = opentodoStart + 10;
                            opentodoEnd = opentodoEnd + 10;
                            DisplayOpenToDo($('#lstOpenToDo'), vData.Results.Rows, false);

                            if (vData.Results.Rows.length < 10) {
                                isFinishedOpenToDo = true;
                                $('#btnShowMoreOpenToDo').text('Show Less');
                            }
                        }
                    })
                }
                else {
                    $('.open-nonsticky').show();
                    $(this).text('Show Less');
                }
            }
            else {
                $('.open-nonsticky').hide();
                $(this).text('Show More');
            }
        })
        .on('click', '#btnShowMoreSched', function () {
            if ($(this).text() == 'Show More') {
                if (isFinishedOpenSched == false) {
                    $(this).text('Loading ...').attr('disabled', true);
                    AjaxSend({
                        url: 'CRMDashboard/GetOpenSchedule',
                        data: { Start: openschedStart, End: openschedEnd },
                        success: function (vData) {
                            $('#btnShowMoreSched').text('Show More').attr('disabled', false);
                            openschedStart = openschedStart + 10;
                            openschedEnd = openschedEnd + 10;
                            DisplayScheduledTask($('#lstSchedule'), vData.Results.Rows, false);

                            if (vData.Results.Rows.length < 10) {
                                isFinishedOpenSched = true;
                                $('#btnShowMoreSched').text('Show Less');
                            }
                        }
                    })
                }
                else {
                    $('.sched-nonsticky').show();
                    $(this).text('Show Less');
                }
            }
            else {
                $('.sched-nonsticky').hide();
                $(this).text('Show More');
            }
        })
        .on('submit', '#formcustomerupload', function(e){
            e.preventDefault();
            var formData = new FormData($(this)[0]);
            // AjaxSend({
            //     url: 'CRMDashboard/FollowupUpdateUploadContacts',
            //     success: function(vDataSource){
            //         console.log(vDataSource)
            //     }
            // })
            // return
            NewMessageBox('This might take a few moments. Please wait while the system checking the data ..')
            setTimeout(function(){
                $.ajax({
                    url: 'CRMDashboard/UploadCustomerInfo?OrgID=' + sessionStorage.OrgID,
                    type: 'POST',
                    data: formData,
                    async: false,
                    cache: false,
                    contentType: false,
                    processData: false,
                    success: function (vData) {
                        $('#objFileCustomer').val('');
                        if(vData.Error == ""){                            
                            WebToast('Your contacts has been Successfully uploaded', 'top');
                            NewMessageBoxClose();
                        }
                        else {
                            NewMessageBox((vData.Error).replace(/\r\n|\r|\n/g,"<br />"));
                        }
                    }
                });
            }, 1500)
        })
        .on('change', '#objFileCustomer', function(e){ 
            $('#txtUploadSalesGrinderDateTime').val($.format.date(new Date(), 'MMM dd, yyyy h:mm a'))
            $('#txtUploadSalesGrinderOrgID').val(sessionStorage.OrgID)           
            $('#formcustomerupload').submit();
        })
        .on('click', '#btnListActivityReport', function () {
            BreadCrumbs('.ItemsBreadCrumbs',
                [{ Text: 'Activity Report' }]
            );
            GetPageActivityReport();
        })

    })// end document
    .mouseup(function (e) {
        var container = $("#divPanelMenu");

        if (!container.is(e.target) // if the target of the click isn't the container...
            && container.has(e.target).length === 0 && container.css('display') == "block") // ... nor a descendant of the container
        {
            $('#divPanelMenu').toggle('fast')
        }
    });

    window.GetPageActivityReport = GetPageActivityReport

    function FollowUploadContactStatus(){
        AjaxSend({
            url: 'CRMDashboard/FollowupUpdateUploadContacts',
            success: function(vData){
                setTimeout(function(){ FollowUploadContactStatus()}, 5000);
                NewMessageBox(vData);
            }
        })
    }

    function RequestData() {
        AjaxSend({
            url: 'CRMDashboard/CheckLastCacheDate',
            success: function (vDataDate) {
                var pParam = $.param({ OrgID: sessionStorage.OrgID, DateFrom: $.format.date(new Date(vDataDate.Results.Rows[0].Date), 'yyyy-MM-dd'), DateTo: $.format.date(new Date(), 'yyyy-MM-dd') })
                AjaxSend({
                    url: 'DashboardsAndReports/GetCustomerTransactionInfoForCRM?' + pParam,
                    success: function (vDataSource) {
                        var p = {
                            d: vDataSource
                        }
                        AjaxSend({
                            url: 'CRMDashboard/SaveCustomerInfoFromDataSource',
                            data: {Source: JSON.stringify(p)},
                            success: function(){

                            }
                        })
                    }
                })
            }
        })
    }

    function SearchScheduledTasks(s) {
        AjaxSend({
            url: 'CRMDashboard/SearchScheduledTasks',
            data: { Search: s },
            success: function (vData) {
                $('#lstScheduleSearch').empty();
                DisplayScheduledTask($('#lstScheduleSearch'), vData.Schedule, true)
            }
        })
    }

    function SearchOpenToDo(s) {
        AjaxSend({
            url: 'CRMDashboard/SearchOpenToDo',
            data: { Search: s },
            success: function (vData) {
                $('#lstOpenToDoSearch').empty();
                DisplayOpenToDo($('#lstOpenToDoSearch'), vData.Results.Rows, true)
            }
        })
    }

    function GetPageCac() {
        if (htmlCac == '') {
            AjaxSend({
                ExactUrl: '/CreditAndCollection?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile,
                type: 'GET',
                dataType: 'text',
                success: function (vData) {
                    htmlCac = vData;
                    $('#pageCac').html('');
                    $('#pageCac').append(vData).trigger('create');
                    $('#pageCac').pageshow();
                    App.Deferred.fulfill('FQ26E5TP76');
                }
            });
        }
        else {
            $('#pageCac').html('').append(htmlCac).trigger('create');
            $('#pageCac').pageshow();
            Cac.Show();
        }
    }

    function GetPageCacPayments() {
        if (htmlCacPayments == '') {
            AjaxSend({
                ExactUrl: '/CreditAndCollection/Payments?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile,
                type: 'GET',
                dataType: 'text',
                success: function (vData) {
                    htmlCacPayments = vData;
                    $('#pageCacPayment').html('');
                    $('#pageCacPayment').append(vData).trigger('create');
                    $('#pageCacPayment').pageshow();
                    App.Deferred.fulfill('FQ26E5TP76');
                }
            });
        }
        else {
            $('#pageCacPayment').html('').append(htmlCacPayments).trigger('create');
            $('#pageCacPayment').pageshow();
        }
    }

    function GetPageLeadGen() {
        sessionStorage.CRMDashboardPage = 'leadgen';
        if (htmlLeadGen == '') {
            AjaxSend({
                ExactUrl: '/CRMDashboard/db?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile,
                type: 'GET',
                dataType: 'text',
                success: function (vData) {
                    htmlLeadGen = vData;
                    $('#pageLeadGen').html('');
                    $('#pageLeadGen').append(vData)//.trigger('create');
                    $('#pageLeadGen').pageshow();
                    App.Deferred.fulfill('FQ26E5TP76');
                }
            });
        }
        else {
            //$('#pageSalesGrinder').html('');
            //$('#pageLeadGen').html('').append(htmlLeadGen).trigger('create');
            //ShowProspectList();
            //GetDashboardRequiredData();
            $('#pageMainDashboard, #pageLeadGen').pageshow();
        }
    }

    function GetPageSalesGrinder() {
        sessionStorage.CRMDashboardPage = 'salesgrinder';
        if (htmlLeadGen == '') {
            AjaxSend({
                ExactUrl: '/CRMDashboard/db?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile,
                type: 'GET',
                dataType: 'text',
                success: function (vData) {
                    htmlLeadGen = vData;
                    $('#pageSalesGrinder').html('');
                    $('#pageSalesGrinder').append(vData).trigger('create');
                    App.Deferred.fulfill('FQ26E5TP76');
                }
            });
        }
        else {
            $('#pageLeadGen').html('');
            $('#pageSalesGrinder').html('').append(htmlLeadGen).trigger('create');
            ShowProspectList();
            $('#pageMainDashboard').pageshow();
        }
    }

    function CheckSession() {
        $('#divDBLoading').show();
        $('#divDBMainPage').hide();
        $('#btnShowMoreOpenToDo').hide();
        $('#btnShowMoreSched').hide();

        AjaxSend({
            url: 'CRMDashboard/IsNewEntry',
            success: function (vData) {
                if (vData.PeopleID == '') { //if people ID is blank, then redirect to sign in
                    proj.PrompLogin();
                    //$('#divDBLoading').text('Your session has expired. Redirecting to sign in page ...');
                    //setTimeout(function () {
                    //    pUrl = '/SignIn/CRM';
                    //    window.open(GetPathFormat(pUrl), '_self');
                    //}, 3000);
                }
                else {
                    $('#divDBLoading').hide();
                    $('#divDBMainPage').show();
                    $('.OrganizationNameHeader').text(sessionStorage.OrgName);
                    $('#txtGreetName').text(sessionStorage.CurrentUserFullName);
                    $('#mainContainer').css('margin-top', $('#divHeaderForProjects2').height());
                    if (window.location.pathname == '/CRMDashboard' || window.location.pathname.toLowerCase() == '/erp/crm') {
                        //temp sa . dapat mauna ug load ang NewMessageBox.js kung asa naka butang ang AddBreadCrumb function. walay choice need i setInterval :'(
                        var tempinterval = setInterval(function () {
                            if (typeof AddBreadCrumb != 'undefined') {
                                AddBreadCrumb({
                                    Text: 'Lead Generation', Click: function () {
                                        $('#pageMainDashboard').pageshow();
                                    }
                                }, '.ItemsBreadCrumbs');
                                clearInterval(tempinterval)
                            }
                        }, 100)
                    }                    
                    GetPageLeadGen();
                    //GetDashboardRequiredData();
                }
            }
        })
    }

    function GetDashboardRequiredData() {
        var date = new Date();
        var lstOpenToDo = $('#lstOpenToDo');
        var lstSchedule = $('#lstSchedule');

        lstOpenToDo.empty().text('Loading ...');
        lstSchedule.empty().text('Loading ...');
        $('#btnShowMoreOpenToDo').text('Show More');
        $('#btnShowMoreSched').text('Show More');
        AjaxSend({
            url: 'CRMDashboard/GetDashboardRequiredData',
            data: {
                From: $.format.date(new Date(date.getMonth() + 1 + ' 1, ' + date.getFullYear()), 'MM-dd-yyyy'),
                To: $.format.date(new Date(new Date().setMonth(new Date().getMonth() + 1, 0)), 'MM-dd-yyyy')
            },
            success: function (vData) {
                var vRowsToDo = vData.ToDo;
                var vRowsSched = vData.Schedule;
                lstOpenToDo.empty();
                lstSchedule.empty();
                DisplayOpenToDo(lstOpenToDo, vRowsToDo, true);
                DisplayScheduledTask(lstSchedule, vRowsSched, true);
                $('#btnShowMoreOpenToDo').css('display', (vRowsToDo.length > 9 ? 'block' : 'none'));
                $('#btnShowMoreSched').css('display', (vRowsSched.length > 9 ? 'block' : 'none'));
            }
        })
    }

    function DisplayOpenToDo(lstOpenToDo, vRowsToDo, isFirstLoad) {
        if (vRowsToDo.length > 0) {
            for (var i = 0; i < vRowsToDo.length; i++) {
                lstOpenToDo.append($('<li class="list-group-item text-dark bg-warning">')
                                        .addClass((isFirstLoad == true ? 'open-sticky' : 'open-nonsticky'))
                                        .css('cursor', 'pointer')
                                        .data(vRowsToDo[i])                                        
                                        .append($('<div class="row align-items-center">')
                                            .append($('<div class="col">')
                                                .text(vRowsToDo[i].Name)
                                            )
                                            .append($('<div class="col-auto">')
                                                .append($('<span class="badge bg-light text-dark">')
                                                    .text(vRowsToDo[i].Counts)
                                                )
                                            )                                            
                                        )
                                        .click(function () {
                                            $('#txtOpenToDoName').text($(this).data().Name);
                                            $('#divDashboardOpenToDo').modal('show');
                                            GetToDoItems($(this).data().ID)
                                        })
                                    )
            }
        }
        else {
            if (isFirstLoad) {
                lstOpenToDo.append($('<li class="list-group-item text-dark bg-warning">').text('To-Do List is empty'));
            }
            else {
                isFinishedOpenToDo = true;
                $('#btnShowMoreOpenToDo').text('Show Less')
            }
        }
    }

    function DisplayScheduledTask(lstSchedule, vRowsSched, isFirstLoad) {
        if (vRowsSched.length > 0) {
            for (var i = 0; i < vRowsSched.length; i++) {
                var dateFrom = new Date($.format.date(new Date(parseInt((vRowsSched[i].SchedActionDateFrom).substr(6))), 'MMM dd, yyyy'));
                var dateTo = new Date($.format.date(new Date(parseInt((vRowsSched[i].SchedActionDateTo).substr(6))), 'MMM dd, yyyy'));
                var datecompare = dateTo <= dateFrom;
                var time;

                if (datecompare) {
                    time = $.format.date(new Date(parseInt((vRowsSched[i].SchedActionDateFrom).substr(6))), 'MM/dd/yy hh:mm a') + ' to ' + $.format.date(new Date(parseInt((vRowsSched[i].SchedActionDateTo).substr(6))), ' hh:mm a')
                }
                else {
                    if (CBool(vRowsSched.IsActionAllDay)) {
                        time = '(All Day) ' + $.format.date(new Date(parseInt((vRowsSched[i].SchedActionDateFrom).substr(6))), 'MM/dd/yy') + ' to ' + $.format.date(new Date(parseInt((vRowsSched[i].SchedActionDateTo).substr(6))), 'MM/dd/yy')
                    }
                    else {
                        time = $.format.date(new Date(parseInt((vRowsSched[i].SchedActionDateFrom).substr(6))), 'MM/dd/yy hh:mm a') + ' to ' + $.format.date(new Date(parseInt((vRowsSched[i].SchedActionDateTo).substr(6))), 'MM/dd/yy hh:mm a')
                    }
                }
                lstSchedule.append($('<li class="list-group-item text-dark bg-info">')
                    .css('background-color', ($.format.date(new Date(parseInt((vRowsSched[i].SchedActionDateFrom).substr(6))), 'MM/dd/yy')) == $.format.date(new Date(), 'MM/dd/yy') ? '#65CFFF' : '#88DAFF')
                    .addClass((isFirstLoad == true ? 'sched-sticky' : 'sched-nonsticky'))
                    .data(vRowsSched[i])
                    .append($('<div>')
                        .html('<b>' + vRowsSched[i].ClientName + '</b><br>' + '<span style="font-size: 14px; color: #0E67AF;">- ' + time + '<br>- ' + vRowsSched[i].SchedActionName + '<br>- ' + vRowsSched[i].Subject + '</span>')
                    )
                    .click(function () {
                        //$('#divDashboardSched').pageshow();
                    })
                )
            }
        }
        else {
            if (isFirstLoad) {
                lstSchedule.append($('<li class="list-group-item text-dark bg-info">').text('No Schedule'));
            }
            else {
                isFinishedOpenSched = true;
                $('#btnShowMoreSched').text('Show Less')
            }
        }
    }

    function GetToDoItems(clientid) {
        var listDashboardToDo = $('#listCRMDashboardToDo');
        listDashboardToDo.empty().text('Loading ...');
        arrTodoList = [];
        AjaxSend({
            url: 'CRMDashboard/GetClientToDo',
            data: { ClientID: clientid, UnfinishToDo: true },
            success: function (vData) {
                var vRows = vData.Results.Rows;
                arrTodoList = vRows;
                listDashboardToDo.empty();
                for (var i = 0; i < vRows.length; i++) {
                    vRows[i].arrindex = i;
                    listDashboardToDo.append($('<li class="list-group-item">')
                            .append($('<div class="row">')
                                .css('cursor', 'pointer')
                                .append($('<div class="col-auto">')
                                    .data(vRows[i])
                                    .append($('<div class="form-check">')
                                    .append($('<label class="form-check-label" for="chbtodo-' + i + '">')
                                        .append($('<input type="checkbox" name="todo" id="chbdbtodo-' + i + '" data-role="none" class="form-check-input"/>')
                                        ).data(vRows[i])
                                        .change(function (e) {
                                            var dis = $(this);
                                            var isCheck = CBool($('#chbdbtodo-' + dis.data().arrindex).is(':checked'));
                                            PromptUserAccess('Edit To-do', {
                                                OnClickOKSuccess: function () {
                                                    $('#chbdbtodo-' + dis.data().arrindex).prop('checked', isCheck)//.checkboxradio()//.checkboxradio('refresh');
                                                    //isCheck = (isCheck ? false : true);
                                                    arrTodoList[dis.data().arrindex].ToDoFinish = isCheck;
                                                    SaveToDo(true);
                                                }
                                            });
                                        })
                                    )
                                    )                                    
                                )
                                .append($('<div class="col">')
                                    .data(vRows[i])
                                    .html('<b>' + vRows[i].ToDo + '</b><br><br>' +
                                            'Due Date: ' + (vRows[i].ToDoDueDate == '' ? '-' : $.format.date(new Date(vRows[i].ToDoDueDate), 'MMM dd, yyyy')) +
                                            (CStr2(vRows[i].TaggedID) == '' ? '' : '<br>File Under: ' + CStr2(vRows[i].TaggedDetails))
                                    )
                                )
                            )
                        )
                    $('#chbdbtodo-' + i).prop('checked', (vRows[i].ToDoFinish == "0" ? false : true))
                    $('input[type="checkbox"]')//.checkboxradio()//.checkboxradio('refresh');
                }
            }
        })
    }

    function SaveToDo(ClientID) {
        AjaxSend({
            url: 'CRMDashboard/SaveToDo',
            data: { ToDo: arrTodoList, DateNow: $.format.date(new Date(), 'MMMM dd,yyyy hh:mm a') },
            success: function (vData) {
                if (vData.Results == 'Saved') {
                    //GetToDoItems(ClientID);
                    WebToast('Saved', 'top');
                }
                else {
                    NewMessageBox('ERROR: ' + vData.Results);
                }
            }
        })
    }

    function GotoSigninPage() {

        var pParam = {};
        if (IsMobile)
            pParam.IsMobile = 1;
        pParam = $.param(pParam);
        window.location = '/SignIn/Crm?' + pParam;

    }; // GotoSigninPage

    function SignOut() {
        AjaxSend({
            url: 'Login3/TestCookies',
            success: function (vData) {

                AjaxSend({
                    url: "CRMDashboard/setSignOut",
                    success: function (vData) {
                        window.open(GetPathFormat("/SignIn/Crm?mode=logout"), "_self"); // window.open(GetPathFormat("/People"), "_self"); EDITED BY: JANINE , Aug 3 4pm
                    }
                });

            },
            error: function (vError) {
                GotoSigninPage();
            }
        })
    }

    function GetPageActivityReport() {
        if (htmlActivityReport == '') {
            AjaxSend({
                ExactUrl: '/CRMDashboard/ActivityReport?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile,
                type: 'GET',
                dataType: 'text',
                success: function (vData) {
                    htmlActivityReport = vData;
                    //if(location.pathname == '/erp'){
                        $('#tabCrmMyReport').html('');
                        $('#tabCrmMyReport').append(vData).trigger('create');
                    //} else {
                    //    $('#pageActivityReport').html('');
                    //    $('#pageActivityReport').append(vData).trigger('create');
                    //}
                    App.Deferred.fulfill('FQ26E5TP76');
                }
            });
        }
        else {
            //$('#pageActivityReport').html('').append(htmlActivityReport).trigger('create');
            $('#divMainActivityReportPage').pageshow();
            ActivityReport.LoadData();
        }
    }
}) // end require