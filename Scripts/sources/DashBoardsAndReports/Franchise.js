require([
    'common', 'tablesort'
    ], function () {

        var selectedFowstID = '';
        var selectedDownloadCount = 0;
        var actualStatus = '';
        var selectedPrintCount = 0;
        var selectedViewCount = 0;
        var selectedStatus = '';
        var selectedStatusID = '';
        var selectedSalesOrderNo = '';
        var lastSearchString = '';
        var maxDownload = 0;
        var maxPrint = 0;

        var selectedFranchiseStatusID = '';
        var selectedFranchiseID = '';
        var selectedFranchiseeOrgFowstID;
        var selectedSalesOrderGroupNo;
        var selectedCutOffDate;
        var selectedGroupName = '';
        var selectedCutOffName = '';
        var franchiseeList;
        var franchiseeListCount;
        var selectedGroupGrossSales;
        var selectedCutOffStartDate;
        var selectedGroupCutOffID;
        var limitRedFlag;

        var ValueBtnBack = '/People/PeopleMenu?IsMobile=0';
        var TempValueBtnBack = null;
        var breadcrumbs = $('<div class="ItemsBreadCrumbs"></div>'),
            arrowR = ' <span class="ui-icon ui-icon-arrow-r" style="display:inline-block; margin:0px 10px 0px 10px;"></span> ';

        $(function () {



            getAllUnassigned();
            getGroups();
            getLimitRedFlag();

            //            GetMaxLimit();
            //            $('#btnFranchiseMoreOption').click(function () {
            //                $('#popFranchiseeMoreOptions').popup('open');

            //            })

            $('body')
            .on('click', '#btnFranchiseCentralDownloadFile', function () {
                GetFranchiseOrderDetails();
            })
            .on('click', '.btnSaveReasonRemoveFlag', function () {
                if ($.trim($('#txtReasonRemoveRedFlag').val()) == '') {
                    WebToast('Please provide a reason for unflagging.', 'top');
                    return;
                }
                SaveRemoveRedFlagReason();
            })
            .on('change', '#selGroupList', function (e) {
                refreshList();
            })
            .on('click', '#btnRefreshList', function (e) {
                refreshList();
            })
            .on('click', '#btnFranchiseDownloadPickingList', DownloadPickingList)
        }); //End Inner function

        function BreadCrumbsClickEvent() {

            if (sessionStorage.IsShowFirstPage == 'false') {
                $('#btnMainDashboard').click(function () {

                    $('#divSalesAnalytics, .btnPeriod, .lblPeriod').fadeIn();
                    //$('#btnPeriodMobile').addClass('btnPeriod').removeClass('btn-disable').on('click.disabled');
                    $('#divMainDashboard, #divOnPrintPOSSalesReport,' +
                    ' #divOrderCentral,' +
                    '#divSpecialReports, #divAuditTrail,' +
                    '#divSettings, .divSearchFilterDateRange,#panTransactionDetails').hide();
                    $('#divFranchiseRequest').hide();
                    $('#divSalesReplenishmentProcessing').hide();
                    $('#divSalesOrderProcessing').hide();
                    $('.ItemsBreadCrumbs').text('Main Dashboard');
                    TempValueBtnBack = null;
                    sessionStorage.SearchMode = 'false';
                    GetData();
                })
            }
            else {
                $('#btnMainDashboard').click(function () {

                    $('#divMainDashboard, .btnPeriod, .lblPeriod').fadeIn();
                    //$('#btnPeriodMobile').addClass('btnPeriod').removeClass('btn-disable').on('click.disabled');
                    $('#divSalesAnalytics, #divOnPrintPOSSalesReport,' +
                        ' #divOrderCentral,' +
                        '#divSpecialReports, #divAuditTrail,' +
                        '#divSettings, .divSearchFilterDateRange,#panTransactionDetails').hide();
                    $('#divFranchiseRequest').hide();
                    $('#divSalesReplenishmentProcessing').hide();
                    $('#divSalesOrderProcessing').hide();
                    $('.ItemsBreadCrumbs').text('Main Dashboard');
                    TempValueBtnBack = null;
                    sessionStorage.SearchMode = 'false';
                    GetData();
                })
            }



            //            $('#btnMainDashboard').click(function () {


            //                $('#divMainDashboard').fadeIn();
            //                $('#divFranchiseeOrders').show();
            //                $('#divSalesReplenishmentProcessing, #divSalesAnalytics').hide();

            //                $('#divProductBreakdown, #divSalesBreakdown,' +
            //                  '#divFranchiseeLastReceived, ' +
            //                  '#divFranchiseRequests').hide();

            //                $('.ItemsBreadCrumbs').text('Main Dashboard');
            //            });
            $('#btnBCStoreReplenishmentProcessing').click(function () {
                $('#divFranchiseeOrders').show();
                $('#divSalesBreakdown, #divProductBreakdown, #divFranchiseeLastReceived, #divFranchiseRequests').hide();

                $('.ItemsBreadCrumbs').text('');
                breadcrumbs.prepend(arrowR, 'Store Replenishment Processing & Analytics');
                $('.BreadCrumbs').html(breadcrumbs.prepend('<a id="btnMainDashboard">Main Dashboard</a>'));
                BreadCrumbsClickEvent();

                TempValueBtnBack = "BackToDashboard";
            });
        }
        function getAllUnassigned() {

            var m_DateFrom = new Date();
            m_DateFrom.setDate(m_DateFrom.getDate() - (m_DateFrom.getDate() - 1));
            var m_DateTo = new Date();

            AjaxSend({
                url: 'DashboardsAndReports/GetFranchiseeOrdersUnassigned',
                data: { From: m_DateFrom, To: m_DateTo },
                success: function (vDataUnassigned) {

                    DisplayFranchiseeOrders(vDataUnassigned.FranchiseData.Rows);
                } // end of success
            });

        }


        ///////////////////////////////////////
        //begin franchise central
        function refreshList() {
            //            $('#Nav1b').css('display', 'none');
            //            $('#Nav1a').css('display', 'block');

            //            $('#navGroup').addClass('ui-navbar-btn-active');
            //            $('#navGroup').addClass('ui-btn-active');

            //            $('#navCutOff').removeClass('ui-navbar-btn-active');
            //            $('#navCutOff').removeClass('ui-btn-active');

            $('#lblFowstName').text('');

            $('#lblNoOfFranchisees').text('');
            selectedGroupName = 'Group Name';
            if ($('#selGroupList option:selected').text() != 'Please Select')
                selectedGroupName = $('#selGroupList option:selected').text();


            $('#spnGroup').html(selectedGroupName);

            $('#tblFranchiseeCutOffBody').html('');
            $('#tblLastReceivedBody').html('');
            $('#tblSalesBreakdownBody').html('');

            $('#hdrTxt').text('Franchise Central Dashboard');

            $('#divOrderButtons').hide();
            var pLV = $("#tblFranchiseeOrdersBody");
            pLV.html('');


            if (CStr2($.trim($('#selGroupList').val())) != '') {
                AjaxSend({
                    url: 'DashboardsAndReports/GetFranchiseeGroupInfo',
                    data: { FranchiseGroupID: $('#selGroupList').val() },
                    success: function (vData) {

                        if (vData.FranchiseData.Rows.length > 0) {
                            selectedFranchiseeOrgFowstID = vData.FranchiseData.Rows[0].FranchiseeOrgFowstID;

                            $('#lblFowstName').text(vData.FranchiseData.Rows[0].FowstName);
                            $('#lblNoOfFranchisees').text(vData.FranchiseData.Rows[0].FranchiseeCount);
                            AjaxSend({
                                url: 'DashboardsAndReports/UpdateSalesOrderGroupNo',
                                data: { FranchiseGroupID: vData.FranchiseData.Rows[0].ID },
                                success: function (vDataFranchisorFowst) {


                                    if (CStr2(vDataFranchisorFowst.FranchiseData) != '')
                                        checkForRedFlag(vDataFranchisorFowst.FranchiseData);
                                    else
                                        $("#tblFranchiseeOrdersBody").append($('<tr>')
                                            .append($('<td>')
                                                .attr('colspan', 7)
                                                .css('text-align', 'center')
                                                .text('No Result(s) Found')
                                            )
                                        )

                                    //                                    showAllFranchiseeOrders('');
                                } // end of success
                            }); // ajax send
                        } else {
                            $("#tblFranchiseeOrdersBody").append($('<tr>').append($('<td colspan="7" style="text-align:center;">').text('No results found')));
                        }





                    } // end of success
                }); // ajax send
            } else {



                getAllUnassigned();
            }
        }


        function checkForRedFlag(groupCutOffID) {

            AjaxSend({
                url: 'DashboardsAndReports/CheckFranchiseRedFlag',
                data: { GroupCutOffID: groupCutOffID },
                success: function (vData) {

                    showAllFranchiseeOrders('');

                } // end of success
            });
        }

        function showAllFranchiseeOrders(sString) {

            AjaxSend({
                url: 'DashboardsAndReports/GetFranchiseeOrders',
                data: { GroupID: $('#selGroupList').val() },
                success: function (vData) {
                    DisplayFranchiseeOrders(vData.FranchiseData.Rows);


                } // end of success
            });
        }

        function ShowFranchiseeLastReceived(groupCutOffID) {
            //            if (actualStatus == '') {

            //                proj.UpdateOrderStatus();
            //            }
            //            sessionStorage.CartOrderID = orderID;
            //            sessionStorage.FowstID = fowst;
            //            sessionStorage.CustomerOrganizationID = client;

            $('#hdrTxt').text('Last Received by Franchisee for the Period Covered ' + selectedCutOffStartDate + ' - ' + selectedCutOffDate);

            var fromDate = new Date();
            fromDate.setDate(fromDate.getDate() - (fromDate.getDate() - 1));
            var toDate = new Date();

            var pData = {};
            pData.GroupCutOffID = groupCutOffID;
            pData.From = fromDate;
            pData.To = toDate;

            //end of additional
            AjaxSend({
                url: 'DashboardsAndReports/GetFranchiseeLastReceived',
                data: pData,
                success: function (vData) {
                    //                    $('#Nav1a').css('display', 'none');
                    //                    $('#Nav1b').css('display', 'block');


                    //                    $('#navCutOff').addClass('ui-navbar-btn-active');
                    //                    $('#navCutOff').addClass('ui-btn-active');

                    //                    $('#navGroup').removeClass('ui-navbar-btn-active');
                    //                    $('#navGroup').removeClass('ui-btn-active');

                    //                    $.mobile.changePage('#pageFranchiseeStatus');
                    DisplayFranchiseeLastReceived(vData.FranchiseData.Rows);


                },
                error: function (vError) {
                    var pDummy;
                }
            }); // AjaxSend

        }

        function DisplayFranchiseeOrders(vRows) {

            //            var m_DateFrom = new Date();
            //            m_DateFrom.setDate(m_DateFrom.getDate() - (m_DateFrom.getDate() - 1));
            //            var m_DateTo = new Date();

            var pLV = $("#tblFranchiseeOrdersBody");

            pLV.html('');

            if (vRows.length == 0) {
                pLV.append($('<tr>').append($('<td colspan="7" style="text-align:center;">').text('No results found')));
                return;
            }
            for (var i = 0; i <= vRows.length - 1; i++) {

                pItem = $('<tr style="cursor: pointer;">')
                        .append($('<td>')
                            .text(i + 1)
                        )
                        .append($('<td>')
                            .text(vRows[i].CutOffName)
                        )
                        .append($('<td data-sort-value="' + vRows[i].ActualCutOffDate + '">')
                            .html(CStr2(vRows[i].ActualCutOffStartDate) == '' ? '<i>Month to date (default)</i>' : $.format.date(new Date(vRows[i].ActualCutOffStartDate)) + ' - ' + $.format.date(new Date(vRows[i].ActualCutOffDate)))
                        )
                         .append($('<td>')
                            .text($.format.date(new Date(vRows[i].LastReceivedDate)))
                        )
                         .append($('<td style="text-align: center;">')
                            .text(vRows[i].SkuCount)
                        )
                        .append($('<td style="text-align: right;" data-sort-value="' + vRows[i].GrossSales + '">')
                            .text(AccountingFormat(vRows[i].GrossSales))
                        )
                        .append($('<td style="text-align: center;" data-sort-value="' + vRows[i].IsRedFlagActive + '">')
                            .html(vRows[i].IsRedFlagActive == 1 ? '<label style="background-color:#FF0000"> &nbsp; &nbsp; &nbsp; &nbsp; </label>' : '<label style="background-color:#73C774"> &nbsp; &nbsp; &nbsp; &nbsp; </label>')
                        )
                        .attr('ID', vRows[i].ID)
                            .attr('SalesOrderGroupNo', vRows[i].SalesOrderGroupNo)
                            .attr('CutOffName', vRows[i].CutOffName)
                             .attr('FranchisorFowstID', vRows[i].FranchisorFowstID)
                             .attr('ActualCutOffDate', $.format.date(new Date(vRows[i].ActualCutOffDate)))
                                .attr('ActualCutOffStartDate', $.format.date(new Date(vRows[i].ActualCutOffStartDate)))
                              .attr('GrossSales', vRows[i].GrossSales)
                              .attr('ID', vRows[i].ID)
                            .click(function (e) {

                                e.preventDefault();
                                selectedSalesOrderGroupNo = $(this).attr('SalesOrderGroupNo');
                                selectedCutOffDate = $(this).attr('ActualCutOffDate');
                                selectedCutOffStartDate = $(this).attr('ActualCutOffStartDate');
                                selectedFowstID = $(this).attr('FranchisorFowstID');

                                selectedCutOffName = $(this).attr('CutOffName');
                                selectedGroupGrossSales = $(this).attr('GrossSales');

                                selectedGroupCutOffID = CStr2($(this).attr('ID'));


                                PopupButtonList('', 'View Options', [
                                    {
                                        Text: 'Sales Breakdown', Click: function () {
                                            if (proj.IsBootstrapDesign()) {
                                                $('#divSalesBreakdown').contents().appendTo('#popupSalesBreakdowncontent-bs');
                                                $('#popupSalesBreakdown-bs').pageshow();
                                                AddBreadCrumb({
                                                    text: 'Sales Breakdown', Click: function () {
                                                        $('#popupSalesBreakdown-bs').pageshow();
                                                    }
                                                }, '.ItemsBreadCrumbs')
                                            } else {
                                                $('#divSalesBreakdown').contents().appendTo('#popupSalesBreakdowncontent');
                                                $('#popupSalesBreakdown').show();
                                            }
                                            
                                            ShowSalesBreakdown(selectedGroupCutOffID);
                                        }
                                    }, {
                                        Text: 'Product Breakdown', Click: function () {
                                            if (proj.IsBootstrapDesign()) {
                                                $('#divProductBreakdown').contents().appendTo('#popupProductBreakdowncontent-bs');
                                                $('#popupProductBreakdown-bs').pageshow();
                                                AddBreadCrumb({
                                                    text: 'Product Breakdown', Click: function () {
                                                        $('#popupProductBreakdown-bs').pageshow();
                                                    }
                                                }, '.ItemsBreadCrumbs')
                                            } else {
                                                $('#divProductBreakdown').contents().appendTo('#popupProductBreakdowncontent');
                                                $('#popupProductBreakdown').show();
                                            }
                                            
                                            ShowOrders(selectedGroupCutOffID);
                                        }
                                    }, {
                                        Text: 'Late Received Date', Click: function () {
                                            if (proj.IsBootstrapDesign()) {
                                                $('#divFranchiseeLastReceived').contents().appendTo('#popupFranchiseeLastReceivedcontent-bs');
                                                $('#popupFranchiseeLastReceived-bs').pageshow();
                                                AddBreadCrumb({
                                                    text: 'Late Received Date', Click: function () {
                                                        $('#popupFranchiseeLastReceived-bs').pageshow();
                                                    }
                                                }, '.ItemsBreadCrumbs')
                                            } else {
                                                $('#divFranchiseeLastReceived').contents().appendTo('#popupFranchiseeLastReceivedcontent');
                                                $('#popupFranchiseeLastReceived').show();
                                            }

                                            ShowFranchiseeLastReceived(selectedGroupCutOffID);
                                        }
                                    }
                                ])
                            })

                pLV.append(pItem)

            }

            pLV.parent()//.table("refresh");
        }


        function ShowOrders(groupCutOffID) {

            $('#hdrTxt').text('Product Breakdown for ' + selectedGroupName + ' Group for the Period Covered ' + selectedCutOffStartDate + ' - ' + selectedCutOffDate);
            var fromDate = new Date();
            fromDate.setDate(fromDate.getDate() - (fromDate.getDate() - 1));
            var toDate = new Date();

            var pData = {};
            pData.GroupCutOffID = groupCutOffID;
            pData.From = fromDate;
            pData.To = toDate;
            //end of additional
            AjaxSend({
                url: 'DashboardsAndReports/GetProductOrders',
                data: pData,
                success: function (vData) {
                    //                    $('#Nav1a').css('display', 'none');
                    //                    $('#Nav1b').css('display', 'block');


                    //                    $('#navCutOff').addClass('ui-navbar-btn-active');
                    //                    $('#navCutOff').addClass('ui-btn-active');

                    //                    $('#navGroup').removeClass('ui-navbar-btn-active');
                    //                    $('#navGroup').removeClass('ui-btn-active');

                    //$.mobile.changePage('#pageFranchiseeStatus');

                    $('#divOrderButtons').show();
                    DisplayFranchiseeProducts(vData.FranchiseData.Rows);


                },
                error: function (vError) {
                    var pDummy;
                }
            }); // AjaxSend

        }
        function DisplayFranchiseeProducts(vRows) {


            var pLV = $('#tblFranchiseeCutOffBody');

            pLV.html('');

            if (vRows.length > 0) {
                for (var i = 0; i <= vRows.length - 1; i++) {
                    var pItem = $('<tr style="cursor: pointer;">')
                        .append($('<td>')
                            .text(i + 1)
                        )
                        .append($('<td>')
                            .text(vRows[i].ProductNo)
                        )
                        .append($('<td>')
                            .text(vRows[i].ProductName)
                        )
                        .append($('<td style="text-align: center">')
                            .text($.format.number(CDec2(vRows[i].OrderedQuantity), '#,###'))
                        )
                        .append($('<td>')
                            .text(vRows[i].UnitMeasure)
                        )

                    pLV.append(pItem)
                }
            } else {
                pLV.append($('<tr>')
                            .append($('<td>')
                                .attr('colspan', 7)
                                .css('text-align', 'center')
                                .text('No Result(s) Found')
                            )
                        )
            }

            pLV.parent()//.table('refresh');

            //            $('#tblFranchiseeCutOffBody')//.table('refresh');

            //            $('#lblSKUCount').text('No. of SKU: ' + skuCount);
            //            $('#lblTotal').text('Total Amount Due: ' + totalValue);

        }

        function ShowSalesBreakdown(groupCutOffID) {

            $('#hdrTxt').text('Sales Breakdown for ' + selectedGroupName + ' Group for the Period Covered ' + selectedCutOffStartDate + ' - ' + selectedCutOffDate);

            var fromDate = new Date();
            fromDate.setDate(fromDate.getDate() - (fromDate.getDate() - 1));
            var toDate = new Date();

            var pData = {};
            pData.GroupCutOffID = groupCutOffID;
            pData.From = fromDate;
            pData.To = toDate;
            //end of additional
            AjaxSend({
                url: 'DashboardsAndReports/GetSalesBreakdownByFranchisee',
                data: pData,
                success: function (vData) {
                    //                    $('#Nav1a').css('display', 'none');
                    //                    $('#Nav1b').css('display', 'block');


                    //                    $('#navCutOff').addClass('ui-navbar-btn-active');
                    //                    $('#navCutOff').addClass('ui-btn-active');

                    //                    $('#navGroup').removeClass('ui-navbar-btn-active');
                    //                    $('#navGroup').removeClass('ui-btn-active');
                    //$.mobile.changePage('#pageFranchiseeStatus');

                    DisplaySalesBreakdown(vData.FranchiseData.Rows);


                },
                error: function (vError) {
                    var pDummy;
                }
            }); // AjaxSend

        }
        function DisplaySalesBreakdown(vRows) {
            var pLV = $('#tblSalesBreakdownBody');

            pLV.html('');

            if (vRows.length > 0) {
                for (var i = 0; i <= vRows.length - 1; i++) {
                    var pItem = $('<tr style="cursor: pointer;">')
                        .append($('<td>')
                            .text(i + 1)
                        )
                        .append($('<td>')
                            .text(vRows[i].FowstName)
                        )
                        .append($('<td style="text-align: right" data-sort-value="' + vRows[i].SubTotal + '">')
                            .text(AccountingFormat(vRows[i].SubTotal))
                        )
                        .append($('<td style="text-align: right" data-sort-value="' + AccountingFormat((parseFloat(vRows[i].SubTotal) / parseFloat(selectedGroupGrossSales)) * 100) + '">')
                            .text(AccountingFormat((parseFloat(vRows[i].SubTotal) / parseFloat(selectedGroupGrossSales)) * 100) + '%')
                        )

                    pLV.append(pItem)
                }
            } else {
                pLV.append($('<tr>')
                            .append($('<td>')
                                .attr('colspan', 7)
                                .css('text-align', 'center')
                                .text('No Result(s) Found')
                            )
                        )
            }

            pLV.parent()//.table('refresh');
        }

        function DisplayFranchiseeLastReceived(vRows) {
            var pLV = $('#tblLastReceivedBody');

            pLV.html('');

            if (vRows.length > 0) {
                for (var i = 0; i <= vRows.length - 1; i++) {
                    var pItem = $('<tr style="cursor: pointer;">')
                        .append($('<td>')
                            .text(i + 1)
                        )
                        .append($('<td>')
                            .text(vRows[i].FowstName)
                        )
                        .append($('<td>')
                            .text(CStr2(vRows[i].LastReceivedDate) == '' ? $.format.date(new Date(vRows[i].LastReceivedUnassigned)) : $.format.date(new Date(vRows[i].LastReceivedDate)))
                        )
                        .append($('<td style="text-align: center;" data-sort-value="' + vRows[i].IsRedFlagActive + '">')
                            .html(vRows[i].IsRedFlagActive == 1 ? '<label style="background-color:#FF0000"> &nbsp; &nbsp; &nbsp; &nbsp; </label>' : '<label style="background-color:#73C774"> &nbsp; &nbsp; &nbsp; &nbsp; </label>')
                        )
                        .append($('<td>')
                            .text(vRows[i].ReasonForRedFlag)
                        )
                        .append($('<td>')
                            .text(vRows[i].ReasonForRemoveFlag)
                        )
                        .attr('FranchiseeName', vRows[i].FowstName)
                        .attr('IsActive', vRows[i].IsRedFlagActive)
                        .attr('FranchiseeFowstID', vRows[i].ID)
                        .attr('ContactNo', vRows[i].ContactNumber)
                        .attr('ContactEmail', vRows[i].ContactEmail)
                        .click(function (e) {
                            if ($(this).attr('IsActive') == '1') {
                                
                                if (proj.IsBootstrapDesign()) {
                                    $('#divRemoveFlag').contents().appendTo('#divRemoveFlagcontent-bs');
                                    $('#popupRemoveFlag-bs').modal('show');
                                } else {
                                    $('#divRemoveFlag').contents().appendTo('#divRemoveFlagcontent');
                                    $('#popupRemoveFlag').popup('open');
                                }
                                
                                //  $.mobile.changePage('#dialogRemoveFlag', 'pop', true, true);
                                $('#txtReasonRemoveRedFlag').val('');
                                selectedFranchiseeOrgFowstID = $(this).attr('FranchiseeFowstID');
                                $('#lblRedFlagLimit').text(limitRedFlag);
                                $('#lblRedFlagFranchiseeName').text($(this).attr('FranchiseeName'));

                                $('#lblContactNo').text($(this).attr('ContactNo'));
                                $('#lblEmailAddress').text($(this).attr('ContactEmail'));
                            }
                        })

                    pLV.append(pItem)
                }
            } else {
                pLV.append($('<tr>')
                            .append($('<td>')
                                .attr('colspan', 7)
                                .css('text-align', 'center')
                                .text('No Result(s) Found')
                            )
                        )
            }

            pLV.parent()//.table('refresh');
        }




        function getGroups() {

            AjaxSend({
                url: 'DashboardsAndReports/GetFranchiseGroups',
                success: function (vData) {

                    var vRows = vData.FranchiseData.Rows;

                    $('#selGroupList').empty();


                    $('#selGroupList').append(new Option('Unassigned', ''));

                    for (var p = 0; p <= vRows.length - 1; p++) {
                        $('#selGroupList').append(new Option(vRows[p].GroupName, vRows[p].ID));

                    }


                    //                    $('#selGroupList option[value="' + sessionStorage.FowstID + '"]').prop('selected', true);
                    //$('#selGroupList').selectmenu('refresh', true);

                } // end of success
            });
        }

        function getLimitRedFlag() {

            AjaxSend({
                url: 'DashboardsAndReports/GetLimitRedFlag',
                success: function (vData) {
                    limitRedFlag = vData.FranchiseData;
                } // end of success
            });
        }

        function SaveRemoveRedFlagReason() {

            AjaxSend({
                url: 'DashboardsAndReports/SaveRemoveRedFlagReason',
                data: { GroupCutOffID: selectedGroupCutOffID, FranchiseeFowstID: selectedFranchiseeOrgFowstID, Reason: $.trim($('#txtReasonRemoveRedFlag').val()) },
                success: function (vData) {
                    if (vData.FranchiseData == 'ok') {
                        if (proj.IsBootstrapDesign()) {
                            $('#popupRemoveFlag-bs').modal('hide');
                        } else {
                            $('#popupRemoveFlag').popup('close');
                        }
                        refreshList();
                        ShowFranchiseeLastReceived();

                    }

                } // end of success
            });
        }

        function DownloadPickingList() {
            if (CStr2(selectedSalesOrderGroupNo) == '') {
                MessageBox('This feature is only available to orders belonging to a certain group cut off.', 'Download PDF');
            } else {
                AjaxSend({
                    url: '/DashboardsAndReports/GeneratePickingListPdf',
                    data: {
                        FowstID: selectedFowstID,
                        SalesOrderGroupNo: selectedSalesOrderGroupNo,
                        GroupID: $('#selGroupList').val()
                    },
                    success: function (vData) {
                        var pFilename;

                        if (vData.Result == 'ok')
                            console.log('jsD92YNTJZ3E!');
                        pFilename = vData.Filename;
                        if (CStr2(pFilename).trim() == '') {
                            console.log('Filename is required');
                        } else {
                            window.location = MergeUrlParameters({
                                vFilename: pFilename
                            }, '/Download');

                            setTimeout(function () {
                                franchiseeList = vData.FranchiseeList.Rows;
                                franchiseeListCount = 0;
                                DownloadSortingList();
                            }, 2000);

                        }
                    }
                });
            }
        } // END DownloadPickingList



        function DownloadSortingList() {

            if (franchiseeListCount < franchiseeList.length) {
                AjaxSend({
                    url: '/DashboardsAndReports/GenerateSortingListPdf',
                    data: {
                        FowstID: selectedFowstID,
                        SalesOrderGroupNo: selectedSalesOrderGroupNo,
                        GroupID: $('#selGroupList').val(),
                        FranchiseeID: franchiseeList[franchiseeListCount].FranchiseeOrgFowstID
                    },
                    success: function (vData) {
                        var pFilename;

                        if (vData.Result == 'ok')
                            console.log('jsD92YNTJZ3G!');
                        pFilename = vData.Filename;
                        if (CStr2(pFilename).trim() == '') {
                            console.log('Filename is required');
                        } else {
                            window.location = MergeUrlParameters({
                                vFilename: pFilename
                            }, '/Download');
                        }

                        franchiseeListCount = franchiseeListCount + 1;
                        DownloadSortingList();
                    }
                });
            }


        } // END DownloadSortingList




        function VerifyFranchisePrintOrder(pInvoiceID) {
            MessageBox("Were you able to successfully print a picking list?",
                            "Print Confirmation",
                            [
                                { Text: 'Yes',
                                    Click: function () {

                                        selectedStatus = '';
                                        selectedStatus = 'Printed';

                                        selectedPrintCount = selectedPrintCount + 1;

                                        //                                        proj.UpdateOrderStatus();
                                        history.back();
                                    }
                                },
                                { Text: "No, it didn't print",
                                    Click: function () {

                                        window.pageSecuritySourcePage = $.mobile.activePage;
                                        PromptUserAccess('Reprint Picking List', {
                                            OnClickOKSuccess: function () {

                                                //                                                $.mobile.changePage(window.pageSourcePage);
                                                PrintFranchiseOrderDetails(pInvoiceID);
                                            }
                                        });
                                    }
                                }
                            ], null, true
                );
        }


        function GetMaxLimit() {
            AjaxSend({
                url: 'DashBoardsAndReports/GetMaxLimit',
                success: function (vData) {
                    maxDownload = vData.MaxDownload;
                    maxPrint = vData.MaxPrint;
                }
            });
        }







        proj.UpdateOrderStatus = function () {
            var pData = {};

            pData.OrderID = selectedInvoiceID;
            pData.DownloadCount = selectedDownloadCount;
            pData.PrintCount = selectedPrintCount;
            pData.Status = selectedStatus;
            pData.StatusID = selectedStatusID;
            AjaxSend({
                url: 'DashBoardsAndReports/UpdateFranchiseOrderStatus',
                data: pData,
                success: function (vData) {

                    //                    GetSalesOrderReport(lastSearchString);
                    //                    $("#lblColumnName").text('Creation Date');


                    //                    GetUpdatedStatus();
                },
                error: function (vError) {
                    var pDummy;
                }
            }); // UpdateOrderStatus
        }; // proj.UpdateOrderStatus


        function GetUpdatedStatus() {

            $('#lblDownloadCount').text(selectedDownloadCount);
            $('#lblPrintCount').text(selectedPrintCount);


            AjaxSend({
                url: 'DashBoardsAndReports/GetUpdatedStatus',
                data: { InvoiceID: selectedInvoiceID },
                success: function (vData) {
                    if (vData.Result == 'ok') {
                        if (vData.StatusCount.Rows.length > 0) {
                            for (var i = 0; i < vData.StatusCount.Rows.length; i++) {
                                if (vData.StatusCount.Rows[i].Status == 'Downloaded') {

                                    selectedDownloadCount = CDec2(vData.StatusCount.Rows[i].Count);
                                    $('#lblDownloadCount').text(selectedDownloadCount);
                                }

                                if (vData.StatusCount.Rows[i].Status == 'Printed') {

                                    selectedPrintCount = CDec2(vData.StatusCount.Rows[i].Count);
                                    $('#lblPrintCount').text(selectedPrintCount);
                                }



                            }

                        }

                        DisplayStatus(vData.Status.Rows);

                    }

                },
                error: function (vError) {
                    var pDummy;
                }
            }); // 

        }


        function DisplayStatus(vRows) {


            var pLV = $('#tblStatusTable');

            pLV.html('');

            for (var i = 0; i <= vRows.length - 1; i++) {
                var pItem = $('<tr>')
                            .append($('<td>')
                                .text(vRows[i].LastUpdated)
                            )
                            .append($('<td>')
                                .text(vRows[i].Status)
                            )
                            .append($('<td>')
                                .text(vRows[i].UserName)
                            )

                pLV.append(pItem)
            }
            try {
                $('#tblStatusTable')//.table('refresh');
            } catch (ex) {
            }
        }


        function GetFranchiseOrderDetails() {
            if (CStr2(selectedSalesOrderGroupNo) == '') {
                MessageBox('This feature is only available to orders belonging to a certain group cut off.', 'Download CSV');
            } else {
                AjaxSend({
                    url: 'DashboardsAndReports/GetRawSalesOrderData',
                    data: { FowstID: selectedFowstID, InvoiceID: '', SalesOrderGroupNo: selectedSalesOrderGroupNo },
                    success: function (vData) {
                        if (vData.FranchiseData == "") {

                            MessageBox("No record(s) found.",
                                "Franchise Order Data",
                                [
                                    { Text: 'Ok' }
                                ]
                            );
                        } else {
                            window.location = GetPathFormat("/DashboardsAndReports/DownloadSalesOrderCsvFile?FileName=" + $.trim(vData.SalesOrderDetail));


                            selectedStatus = '';
                            selectedStatus = 'Downloaded';

                            selectedDownloadCount = selectedDownloadCount + 1;


                            //                        proj.UpdateOrderStatus();


                        }

                    }
                });

            }

        }

        function showAllFranchiseRequests() {

            AjaxSend({
                url: 'DashboardsAndReports/GetFranchiseRequests',
                success: function (vData) {

                    DisplayFranchiseRequestList(vData.FranchiseData.Rows);

                } // end of success
            });
        }




        function DisplayFranchiseRequestList(vRows) {
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

                            ShowFranchiseRequestPreview($(this).attr('ID'));


                        })

                pLV.append(pItem)

            }

            //            $('#tblFranchiseeManagerBody')//.table("refresh");
        }




        function ShowFranchiseRequestPreview(franchiseID) {

            var pData = {};

            pData.FranchiseID = franchiseID;

            AjaxSend({
                url: 'DashboardsAndReports/GetFranchisorStatus',
                data: pData,
                success: function (vData) {
                    if (vData.FranchiseData.Rows.length > 0) {
                        $('#dialogFranchiseRequestStatus').popup('open');
                        //                        $.mobile.changePage('#dialogFranchiseRequestStatus', 'pop', true, true);
                        $("#lblFranchisorName").text('');
                        $("#lblFranchisorName").text(vData.FranchiseData.Rows[0].OrganizationName);

                        $("#lblFranchiseRequestCurrentStatus").text(CStr2(vData.FranchiseData.Rows[0].DateActivated) != '' ? 'Active' : 'Pending');

                        if ($("#lblFranchiseRequestCurrentStatus").text() == 'Active') {
                            $("#divFranchiseRequestPending").hide();
                            $("#divFranchiseRequestPendingFranchisee").hide();
                            $("#lblFranchiseRequestRequestedBy").text(vData.FranchiseData.Rows[0].RequestedByName);
                            $("#lblFranchiseRequestDateRequested").text($.format.date(new Date(vData.FranchiseData.Rows[0].DateRequested)));

                            $("#divFranchiseRequestApproved").show();
                            $("#lblFranchiseRequestApprovedBy").text(vData.FranchiseData.Rows[0].ActivatedByName);
                            $("#lblFranchiseRequestDateActivated").text($.format.date(new Date(vData.FranchiseData.Rows[0].DateActivated)));


                        } else {

                            if (CDec2(vData.FranchiseData.Rows[0].IsFranchisorConfirmed) > 0) {
                                $("#divFranchiseRequestPending").show();
                                $("#divFranchiseRequestPendingFranchisee").hide();
                                $("#lblFranchiseRequestFranchisorNameLabel").text(vData.FranchiseData.Rows[0].OrganizationName);
                                $("#lblFranchiseRequestFranchiseeNameLabel").text(CStr2(vData.FranchiseData.Rows[0].FranchiseeFowstName) == '' ? 'this fowst' : vData.FranchiseData.Rows[0].FranchiseeFowstName);
                            }
                            else {
                                $("#divFranchiseRequestPendingFranchisee").show();
                                $("#divFranchiseRequestPending").hide();

                                $("#lblFranchiseRequestFranchisorNameLabel").text(vData.FranchiseData.Rows[0].OrganizationName);
                                $("#lblFranchiseRequestFranchiseeNameLabel").text(CStr2(vData.FranchiseData.Rows[0].FranchiseeFowstName) == '' ? 'this fowst' : vData.FranchiseData.Rows[0].FranchiseeFowstName);
                            }

                            $("#divFranchiseRequestApproved").hide();
                            $("#lblFranchiseRequestRequestedBy").text(vData.FranchiseData.Rows[0].RequestedByName);
                            $("#lblFranchiseRequestDateRequested").text($.format.date(new Date(vData.FranchiseData.Rows[0].DateRequested)));

                        }

                    } else {
                        MessageBox("There was an error while processing your request.");
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
