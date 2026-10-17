
require([
         '/Scripts/jquery.widget.min.js'
],
     function () {
         require([
                 '/Scripts/metro.min.js'
         ],
             function () {
                 //metro-calendar.js, metro-datepicker.js needs jquery.widget.min.js andmetro.min.js  to load first before it can load itself
                 require([
                         '/Scripts/metro-calendar.js',
                         '/Scripts/metro-datepicker.js'
                 ],
                 function () {
                     require(['common'], function () {
                         $(document).ready(function () {

                             $(function () {
                                 GetOrganizationName();

                                 var timeout = setInterval(function () {
                                     if (typeof AddBreadCrumb != 'undefined') {
                                         clearInterval(timeout);
                                         AddBreadCrumb({
                                             UpdateLastchild: 1,
                                             text: 'Sales Report', Click: function () {
                                                 $('#divSalesReport').pageshow();
                                             }
                                         }, '.ItemsBreadCrumbs')
                                     }
                                 }, 100)
                             })

                             $('.btnhome').click(function () {
                                 location.href = '/People/PeopleMenu';
                             })

                             $('#btnhidepanel').click(function () {
                                 if ($(this).hasClass('show')) {
                                     $(this).removeClass('show');
                                     $('#divpanelmenu').hide();
                                 } else {
                                     $(this).addClass('show');
                                     $('#divpanelmenu').show();
                                 }
                             })

                             $('.nav-menu').click(function () {
                                 $('.nav-menu').removeClass('active');
                                 $(this).addClass('active');
                             })

                             //========================================== Inventory Movement =====================================

                             $('#btnCMSalesReport').click(function () {
                                 AddBreadCrumb({
                                     UpdateLastchild: 1,
                                     text: 'Sales Report', Click: function () {
                                         $('#divSalesReport').pageshow();
                                     }
                                 }, '.ItemsBreadCrumbs')

                                 GetSalesReport($('.lblSalesReportDate').text());
                                 $('#divSalesReport').pageshow();
                             })

                             $('#btnSalesReportDate').click(function () {
                                 SingleCalendar({
                                     Click: function (e) {
                                         $('.lblSalesReportDate').text(e);
                                         GetSalesReport($('.lblSalesReportDate').text());
                                     }
                                 })
                             })
                             //========================================== End of Inventory Movement =====================================


                             //========================================== Inventory Movement =====================================

                             $('#btnCMInventoryMovement').click(function () {
                                 AddBreadCrumb({
                                     UpdateLastchild: 1,
                                     text: 'Inventory Movement', Click: function () {
                                         $('#divInventoryMovement').pageshow();
                                     }
                                 }, '.ItemsBreadCrumbs')

                                 GetInventoryMovement($('.lblInventoryMovementDate').text());
                                 $('#divInventoryMovement').pageshow();
                             })

                             $('#btnInventoryMovementDate').click(function () {
                                 SingleCalendar({
                                     Click: function (e) {
                                         $('.lblInventoryMovementDate').text(e);
                                         GetInventoryMovement($('.lblInventoryMovementDate').text());
                                     }
                                 })
                             })

                             $('#btnInventoryMovementFOWST').click(function () {
                                 DisplayFowst('InventoryMovement');
                             })

                             //========================================== End of Inventory Movement =====================================

                             $('#btnCMCashFlow').click(function () {
                                 AddBreadCrumb({
                                     UpdateLastchild: 1,
                                     text: 'Cash Flow', Click: function () {
                                         $('#divCashFlow').pageshow();
                                     }
                                 }, '.ItemsBreadCrumbs')
                                 $('#divCashFlow').pageshow();
                             })

                             //========================================== Delivery Report=====================================

                             $('#btnCMDeliveryReport').click(function () {
                                 AddBreadCrumb({
                                     UpdateLastchild: 1,
                                     text: 'Delivery Report', Click: function () {
                                         $('#divDeliveryReport').pageshow();
                                     }
                                 }, '.ItemsBreadCrumbs')

                                 GetDeliveryReport($('.lblDeliveryReport').text());
                                 $('#divDeliveryReport').pageshow();
                             })

                             $('#btnDeliveryReportFOWST').click(function () {
                                 DisplayFowst('DeliveryReport');
                             })
                             

                             $('#btnDeliveryReport').click(function () {
                                 SingleCalendar({
                                     Click: function (e) {
                                         $('.lblDeliveryReport').text(e);
                                         GetDeliveryReport($('.lblDeliveryReport').text());
                                     }
                                 })
                             })

                             //========================================== End of Delivery Report=====================================

                             //========================================== Hourly Report=====================================
                             $('#btnCMHourlySalesReport').click(function () {
                                 AddBreadCrumb({
                                     UpdateLastchild: 1,
                                     text: 'Hourly Sales Report', Click: function () {
                                         $('#divHourlySalesReport').pageshow();
                                     }
                                 }, '.ItemsBreadCrumbs')

                                 GetHourlySalesReport($('.lblHourlySalesReport').text());
                                 $('#divHourlySalesReport').pageshow();
                             })

                             $('#btnHourlySalesReport').click(function () {
                                 SingleCalendar({
                                     Click: function (e) {
                                         $('.lblHourlySalesReport').text(e);
                                         GetHourlySalesReport($('.lblHourlySalesReport').text());
                                     }
                                 })
                             })

                             $('#btnHourlySalesReportFOWST').click(function () {
                                 DisplayFowst('HourlySalesReport');
                             })

                             //========================================== End of Hourly Report=====================================

                             //========================================== Monthly Sales Report=====================================
                             $('#btnMonthlySalesReport').click(function () {
                                 DualCalendar({
                                     Format: 'MM-dd-yyyy',
                                     Click: function (e) {
                                         mMonthlyReportFrom = e.From;
                                         mMonthlyReportTo = e.To;
                                         $('.lblMonthlySalesReport').text(mMonthlyReportFrom + ' to ' + mMonthlyReportTo)
                                         GetMonthlySalesReport();
                                     }
                                 })
                             })

                             $('#btnMonthlySalesReportFOWST').click(function () {
                                 DisplayFowst('MonthlyReport');
                             })

                             $('#btnMonthlySalesReportGroupFOWST').click(function () {
                                 DisplayGroupFOWST();
                             })

                             $('#btnCMMonthlySalesReport').click(function () {
                                 AddBreadCrumb({
                                     UpdateLastchild: 1,
                                     text: 'Monthly Sales Report', Click: function () {
                                         $('#divMonthlySalesReport').pageshow();
                                     }
                                 }, '.ItemsBreadCrumbs')

                                 GetMonthlySalesReport();
                                 $('#divMonthlySalesReport').pageshow();
                             })

                             //========================================== End of Monthly Sales Report=====================================

                             //========================================== Special Discount Report=====================================

                             $('#btnCMSpecialDiscountReport').click(function () {
                                 AddBreadCrumb({
                                     UpdateLastchild: 1,
                                     text: 'Special Discount Report', Click: function () {
                                         $('#divSpecialDiscountReport').pageshow();
                                     }
                                 }, '.ItemsBreadCrumbs')
                                 $('#divSpecialDiscountReport').pageshow();
                             })

                             $('#btnSpecialDiscountReportFOWST').click(function () {
                                 DisplayFowst('SpecialDiscountReport');
                             })

                             $('#btnSpecialDiscountReport').click(function () {
                                 DualCalendar({
                                     Format: 'MM-dd-yyyy',
                                     Click: function (e) {
                                         mSpecialDiscountReportFrom = e.From;
                                         mSpecialDiscountReportTo = e.To;
                                         $('.lblSpecialDiscountReport').text(mSpecialDiscountReportFrom + ' to ' + mSpecialDiscountReportTo)
                                         GetSpecialDiscountReport();
                                     }
                                 })
                             })

                             //========================================== End of Special Discount Report=====================================

                             $('#btnCMPOSSalesReport').click(function () {
                                 AddBreadCrumb({
                                     UpdateLastchild: 1,
                                     text: 'POS Sales Report', Click: function () {
                                         $('#divPOSSalesReport').pageshow();
                                     }
                                 }, '.ItemsBreadCrumbs')
                                 $('#divPOSSalesReport').pageshow();
                             })

                             $('#btnCMZReadingReport').click(function () {
                                 AddBreadCrumb({
                                     UpdateLastchild: 1,
                                     text: 'Z-Reading Report', Click: function () {
                                         $('#divZReadingReport').pageshow();
                                     }
                                 }, '.ItemsBreadCrumbs')
                                 $('#divZReadingReport').pageshow();
                             })                           

                             $('#btnCMVoidedSalesReport').click(function () {
                                 AddBreadCrumb({
                                     UpdateLastchild: 1,
                                     text: 'Voided Sales Report', Click: function () {
                                         $('#divVoidedSalesReport').pageshow();
                                     }
                                 }, '.ItemsBreadCrumbs')
                                 $('#divVoidedSalesReport').pageshow();
                             })

                             $('#btnCMShiftSummaryReport').click(function () {
                                 AddBreadCrumb({
                                     UpdateLastchild: 1,
                                     text: 'Shift Summary Report', Click: function () {
                                         $('#divShiftSummaryReport').pageshow();
                                     }
                                 }, '.ItemsBreadCrumbs')
                                 $('#divShiftSummaryReport').pageshow();
                             })

                             $('#btnCMPOSAcumaticaSalesReport').click(function () {
                                 AddBreadCrumb({
                                     UpdateLastchild: 1,
                                     text: 'POS Acumatica Sales Report', Click: function () {
                                         $('#divPOSAcumaticaSalesReport').pageshow();
                                     }
                                 }, '.ItemsBreadCrumbs')
                                 $('#divPOSAcumaticaSalesReport').pageshow();
                             })

                             $('#btnCMGenerateEJournalReport').click(function () {
                                 AddBreadCrumb({
                                     UpdateLastchild: 1,
                                     text: 'Generate E-Journal Report', Click: function () {
                                         $('#divGenerateEJournalReport').pageshow();
                                     }
                                 }, '.ItemsBreadCrumbs')
                                 $('#divGenerateEJournalReport').pageshow();
                             })

                             function GetOrganizationName() {
                                 AjaxSend({
                                     url: 'DashboardsAndReports/GetOrganizationName',
                                     success: function (vData) {
                                         if (vData.Results.Rows.length > 0) {
                                             $('.OrganizationNameHeader').text(vData.Results.Rows[0].CompanyName)
                                             $('#OrganizationName').html(vData.Results.Rows[0].CompanyNameInitial)
                                         }
                                     }
                                 });
                             }

                             //========================================== Sales Report =====================================
                             var mSalesReport;

                             function GetSalesReport(vDate) {
                                 if (vDate.trim() == 'Today') {
                                     vDate = new Date();
                                 }

                                 var data = {
                                     dateFrom: vDate,
                                     dateTo: vDate
                                 }

                                 Bs.Loading('show');
                                 AjaxSend({
                                     url: 'CincoCorporation/GetSalesReport',
                                     data: data,
                                     success: function (vData) {
                                         
                                             DisplaySalesReport(vData.Results.Rows);
                                         
                                             Bs.Loading('hide');
                                     }
                                 });
                             }

                             function DisplaySalesReport(vRows) {
                                 if ($.fn.DataTable.isDataTable($('#tblSalesReport'))) {
                                     $('#tblSalesReport').empty();
                                     mSalesReport.destroy();
                                 }

                                 var buttons = [];
                                 if (!IsMobile) {
                                     buttons.push({ extend: 'csvHtml5', text: 'Download CSV', title: 'Sales Report' })
                                 }

                                 mSalesReport = $('#tblSalesReport').DataTable({
                                     pageLength: 25,
                                     "ordering": false,
                                     "data": vRows,
                                     "columns": [
                                         { "data": "FOWSTName" },
                                         { "data": "Region" },
                                         { "data": "StoreType" },
                                         { "data": "TransactionCount" },
                                         {
                                             "data": function (data) {
                                                 return AccountingFormat(CDec2(data.GrossSales) / CDec2(data.TransactionCount));
                                             }
                                         },
                                         {
                                             "data": function (data) {
                                                 return AccountingFormat(data.PayMaya);
                                             }
                                         },
                                         {
                                             "data": function (data) {
                                                 return AccountingFormat(data.GCash);
                                             }
                                         },
                                         {
                                             "data": function (data) {
                                                 return AccountingFormat(data.GrossSales);
                                             }
                                         },
                                         {
                                             "data": function (data) {
                                                 return AccountingFormat(data.DiscountAmount);
                                             }
                                         },
                                         {
                                             "data": function (data) {
                                                 return AccountingFormat(CDec2(data.GrossSales) - CDec2(data.GrabFood) - CDec2(data.FoodPanda));
                                             }
                                         },
                                         {
                                             "data": function (data) {
                                                 return AccountingFormat(CDec2(data.GrabFood) + CDec2(data.FoodPanda));
                                             }
                                         },

                                         {
                                             "data": function (data) {
                                                 return AccountingFormat((CDec2(data.GrossSales) - CDec2(data.DiscountAmount)) / 1.12);
                                             }
                                         }
                                     ],
                                     dom: 'Brtip',
                                     buttons: buttons,
                                     initComplete: function () {
                                         $('#tblSalesReport').removeClass('dataTable');
                                     }
                                 });

                                 mSalesReport.on('click', 'td', function () {
                                     var data = mSalesReport.row(this).data();

                                 });
                             }

                             //========================================== End of Sales Report =====================================

                             //========================================== Hourly Sales Report=====================================
                             var mHourlySalesReport;

                             function GetHourlySalesReport(vDate) {
                                 if (vDate.trim() == 'Today') {
                                     vDate = new Date();
                                 }

                                 if (CStr2(sessionStorage.SelectedFOWST) == '') {
                                     return;
                                 }

                                 var data = {
                                     dateFrom: vDate,
                                     dateTo: vDate,
                                     FOWSTID: sessionStorage.SelectedFOWST
                                 }

                                 Bs.Loading('show');
                                 AjaxSend({
                                     url: 'CincoCorporation/GetHourlySalesReport',
                                     data: data,
                                     success: function (vData) {
                                         
                                             DisplayHourlySalesReport(vData.Results.Rows);
                                         
                                             Bs.Loading('hide');
                                     }
                                 });
                             }

                             function DisplayHourlySalesReport(vRows) {
                                 if ($.fn.DataTable.isDataTable($('#tblHourlySalesReport'))) {
                                     $('#tblHourlySalesReport').empty();
                                     mHourlySalesReport.destroy();
                                 }

                                 var buttons = [];
                                 if (!IsMobile) {
                                     buttons.push({ extend: 'csvHtml5', text: 'Download CSV', title: 'Hourly Sales' })
                                 }

                                 mHourlySalesReport = $('#tblHourlySalesReport').DataTable({
                                     pageLength: 25,
                                     "ordering": false,
                                     "data": vRows,
                                     "columns": [
                                         {
                                             "data": function (data) {
                                                 var mHour = new Date();
                                                 mHour.setHours(CInt2(data.HourOfDayInInteger));

                                                 return $.format.date(mHour, 'hh:00 a');
                                             }
                                         },
                                         { "data": "ItemCount" },
                                         { "data": "TransactionCount" },
                                         {
                                             "data": function (data) {
                                                 return 'PhP ' + AccountingFormat(CDec2(data.GrossSales) - CDec2(data.Discount));
                                             }
                                         }
                                     ],
                                     dom: 'Brtip',
                                     buttons: buttons,
                                     buttons: buttons,
                                     initComplete: function () {
                                         $('#tblHourlySalesReport').removeClass('dataTable');
                                     }
                                 });

                                 mHourlySalesReport.on('click', 'td', function () {
                                     var data = mHourlySalesReport.row(this).data();

                                 });
                             }

                             //========================================== End of Hourly Sales Report =====================================

                             //========================================== Inventory Movement =====================================

                             var mInventoryMovement;

                             function GetInventoryMovement(vDate) {

                                 if (vDate.trim() == 'Today') {
                                     vDate = new Date();
                                 }

                                 if (CStr2(sessionStorage.SelectedFOWST) == '') {
                                     return;
                                 }

                                 var data = {
                                     dateFrom: vDate,
                                     dateTo: vDate,
                                     FOWSTID: sessionStorage.SelectedFOWST
                                 }

                                 Bs.Loading('show');
                                 AjaxSend({
                                     url: 'CincoCorporation/GetInventoryMovement',
                                     data: data,
                                     success: function (vData) {
                                         
                                             DisplayInventoryMovement(vData.Results.Rows);
                                         
                                             Bs.Loading('hide');
                                     }
                                 });
                             }

                             function DisplayInventoryMovement(vRows) {
                                 if ($.fn.DataTable.isDataTable($('#tblInventoryMovementReport'))) {
                                     $('#tblInventoryMovementReport').empty();
                                     mInventoryMovement.destroy();
                                 }

                                 var buttons = [];
                                 if (!IsMobile) {
                                     buttons.push({ extend: 'csvHtml5', text: 'Download CSV', title: 'Inventory Movement' })
                                 }

                                 mInventoryMovement = $('#tblInventoryMovementReport').DataTable({
                                     pageLength: 25,
                                     "ordering": false,
                                     "data": vRows,
                                     "columns": [
                                         { "data": "ProductName" },
                                         {
                                             "data": function (data) {
                                                 return AccountingFormat(CDec2(data.Balance));
                                             }
                                         },
                                         {
                                             "data": function (data) {
                                                 return AccountingFormat(CDec2(data.Delivery));
                                             }
                                         },
                                         {
                                             "data": function (data) {
                                                 return AccountingFormat(CDec2(data.TransIn));
                                             }
                                         },
                                         {
                                             "data": function (data) {
                                                 return AccountingFormat(CDec2(data.TransOut));
                                             }
                                         },
                                         {
                                             "data": function (data) {
                                                 return AccountingFormat(CDec2(data.Ending));
                                             }
                                         },
                                         {
                                             "data": function (data) {
                                                 return AccountingFormat(CDec2(data.Usage));
                                             }
                                         }
                                     ],
                                     dom: 'Brtip',
                                     buttons: buttons,
                                     initComplete: function () {                                         
                                         $('#tblInventoryMovementReport').removeClass('dataTable');
                                     }
                                 });

                                 mInventoryMovement.on('click', 'td', function () {
                                     var data = dataTableTransactionHeader.row(this).data();
                                     if (data != undefined) {
                                     }
                                 });
                             }

                             //========================================== End of Inventory Movement =====================================

                             //========================================== Delivery =====================================

                             var mDeliveryReport;

                             function GetDeliveryReport(vDate) {

                                 if (vDate.trim() == 'Today') {
                                     vDate = new Date();
                                 }

                                 if (CStr2(sessionStorage.SelectedFOWST) == undefined) {
                                     return;
                                 }

                                 var data = {
                                     dateFrom: vDate,
                                     dateTo: vDate,
                                     FOWSTID: sessionStorage.SelectedFOWST
                                 }

                                 Bs.Loading('show');
                                 AjaxSend({
                                     url: 'CincoCorporation/GetDeliveryReport',
                                     success: function (vData) {
                                         
                                             DisplayDeliveryReport(vData.Results.Rows);
                                         
                                             Bs.Loading('hide');
                                     }
                                 });
                             }

                             function DisplayDeliveryReport(vRows) {
                                 if ($.fn.DataTable.isDataTable($('#tblDeliveryReport'))) {
                                     $('#tblDeliveryReport').empty();
                                     mDeliveryReport.destroy();
                                 }

                                 var buttons = [];
                                 if (!IsMobile) {
                                     buttons.push({ extend: 'csvHtml5', text: 'Download CSV', title: 'Inventory Movement' })
                                 }

                                 mDeliveryReport = $('#tblDeliveryReport').DataTable({
                                     pageLength: 25,
                                     "ordering": false,
                                     "data": vRows,
                                     "columns": [
                                         { "data": "ProductName" },
                                         { "data": "Quantity" }
                                     ],
                                     dom: 'Brtip',
                                     buttons: buttons,
                                     initComplete: function () {
                                         $('#tblDeliveryReport').removeClass('dataTable');
                                     }
                                 });

                                 mDeliveryReport.on('click', 'td', function () {
                                     var data = dataTableTransactionHeader.row(this).data();
                                     if (data != undefined) {
                                     }
                                 });
                             }

                             //========================================== End of Inventory Movement =====================================

                             //========================================== Monthly Sales Report=====================================
                             var mMonthlySalesReport='';
                             var mMonthlyReportFrom='';
                             var mMonthlyReportTo;

                             function GetMonthlySalesReport() {
                                 
                                 if (mMonthlyReportFrom.trim() == '') {
                                     return;
                                 }

                                 if (sessionStorage.SelectedFOWST == undefined) {
                                     return;
                                 }

                                 var data = {
                                     dateFrom: mMonthlyReportFrom,
                                     dateTo: mMonthlyReportTo,
                                     FOWSTID: sessionStorage.SelectedFOWST
                                 }

                                 Bs.Loading('show');
                                 AjaxSend({
                                     url: 'CincoCorporation/GetMonthlySalesReport',
                                     data:data,
                                     success: function (vData) {

                                         DisplayMonthlySalesReport(vData.Results.Rows);

                                         Bs.Loading('hide');
                                     }
                                 });
                             }

                             function DisplayMonthlySalesReport(vRows) {

                                 if ($.fn.DataTable.isDataTable($('#tblMonthlySalesReport'))) {
                                     $('#tblMonthlySalesReport').empty();
                                     mMonthlySalesReport.destroy();
                                 }

                                 var buttons = [];
                                 if (!IsMobile) {
                                     buttons.push({ extend: 'csvHtml5', text: 'Download CSV', title: 'Monthly Sales Report' })
                                 }

                                 mMonthlySalesReport = $('#tblMonthlySalesReport').DataTable({
                                     pageLength: 25,
                                     "ordering": false,
                                     "data": vRows,
                                     "columns": [
                                         { "data": "FOWSTName" },
                                         { "data": "Region" },
                                         { "data": "StoreType" },
                                         { "data": "TransactionCount" },
                                         {
                                             "data": function (data) {
                                                 //Target Sales
                                                 return AccountingFormat(data.TargetSales);
                                             }
                                         },
                                         {
                                             "data": function (data) {
                                                 return AccountingFormat(CDec2(data.GrossSales));
                                             }
                                         },
                                         {
                                             "data": function (data) {
                                                 return AccountingFormat(CDec2(data.DiscountAmount));
                                             }
                                         },
                                         {
                                             "data": function (data) {
                                                 return AccountingFormat(CDec2(data.GrossSales) - CDec2(data.DiscountAmount));
                                             }
                                         },
                                         {
                                             "data": function (data) {
                                                 // Achievement
                                                 return AccountingFormat(CDec2(data.GrossSales) / CDec2(data.TargetSales) * 100) + '%';
                                             }
                                         },
                                         {
                                             "data": function (data) {
                                                 return AccountingFormat(CDec2(data.FoodPanda));
                                             }
                                         },
                                         {
                                             "data": function (data) {
                                                 return AccountingFormat(CDec2(data.GrabFood));
                                             }
                                         },
                                         {
                                             "data": function (data) {
                                                 //Pood Trip
                                                 return AccountingFormat(0);
                                             }
                                         },
                                         {
                                             "data": function (data) {
                                                 //DeliRush
                                                 return AccountingFormat(0);
                                             }
                                         },
                                         {
                                             "data": function (data) {
                                                 //Local Delivery Partners
                                                 return AccountingFormat(0);
                                             }
                                         },
                                         {
                                             "data": function (data) {
                                                 //Shoppee E-Voucher
                                                 return AccountingFormat(0);
                                             }
                                         },
                                         {
                                             "data": function (data) {
                                                 //Gold Card
                                                 return AccountingFormat(0);
                                             }
                                         }

                                     ],
                                     dom: 'Brtip',
                                     buttons: buttons,
                                     initComplete: function () {
                                         $('#tblMonthlySalesReport').removeClass('dataTable');
                                     }
                                 });

                                 mMonthlySalesReport.on('click', 'td', function () {

                                 });
                             }

                             //========================================== End of Monthly Sales Report =====================================

                             //========================================== Special Discount Report=====================================
                             var mSpecialDiscountReport = '';
                             var mSpecialDiscountReportFrom = '';
                             var mSpecialDiscountReportTo;

                             function GetSpecialDiscountReport() {

                                 if (mSpecialDiscountReportFrom.trim() == '') {
                                     return;
                                 }

                                 if (sessionStorage.SelectedFOWST == undefined) {
                                     return;
                                 }

                                 var data = {
                                     dateFrom: mSpecialDiscountReportFrom,
                                     dateTo: mSpecialDiscountReportTo,
                                     FOWSTID: sessionStorage.SelectedFOWST
                                 }

                                 Bs.Loading('show');
                                 AjaxSend({
                                     url: 'CincoCorporation/GetSpecialDiscountReport',
                                     data: data,
                                     success: function (vData) {

                                         DisplaySpecialDiscountReport(vData.Results.Rows);

                                         Bs.Loading('hide');
                                     }
                                 });
                             }

                             function DisplaySpecialDiscountReport(vRows) {

                                 if ($.fn.DataTable.isDataTable($('#tblSpecialDiscountReport'))) {
                                     $('#tblSpecialDiscountReport').empty();
                                     mSpecialDiscountReport.destroy();
                                 }

                                 var buttons = [];
                                 if (!IsMobile) {
                                     buttons.push({ extend: 'csvHtml5', text: 'Download CSV', title: 'Monthly Sales Report' })
                                 }

                                 mSpecialDiscountReport = $('#tblSpecialDiscountReport').DataTable({
                                     pageLength: 25,
                                     "ordering": false,
                                     "data": vRows,
                                     "columns": [
                                         { "data": "InvoiceDate" },
                                         { "data": "InvoiceNo" },
                                         { "data": "ProductDescription" },
                                         {
                                             "data": function (data) {
                                                 if (data.DiscountTypeID.toUpperCase() == '055A0A33-DD98-47EC-ADD3-8B2009F3944A') {
                                                     return 'Senior Citizen/Person With Disability Discount';
                                                 } else if (data.DiscountTypeID.toUpperCase() == '21EBF55B-526C-4276-B3B7-0CBCA504F425') {
                                                     return 'Philippine National Sports Team Discount';
                                                 } else {
                                                     return '';
                                                 }
                                                 
                                             }
                                         },
                                         {
                                             "data": function (data) {
                                                 return AccountingFormat(data.SubTotal);
                                             }
                                         },
                                         {
                                             "data": function (data) {
                                                 return AccountingFormat(CDec2(data.Discount));
                                             }
                                         },
                                         {
                                             "data": function (data) {
                                                 return AccountingFormat(CDec2(data.SubTotal) - CDec2(data.Discount));
                                             }
                                         },
                                         {
                                             "data": function (data) {
                                                 return "Picture here with link " + CStr2(data.IDImage);
                                             }
                                         }

                                     ],
                                     dom: 'Brtip',
                                     buttons: buttons,
                                     initComplete: function () {
                                         $('#tblSpecialDiscountReport').removeClass('dataTable');
                                     }
                                 });

                                 mSpecialDiscountReport.on('click', 'td', function () {

                                 });
                             }

                             //========================================== End of Special Discount Report =====================================

                             function DisplayFowst(vReport) {
                                 BrowsePopup('LibraryFowst', 'Select FOWST', [
                                         {
                                             AutoSearch: true,
                                             DisplayText: [{ Column: 'FowstName' }],
                                             Click: function (e) {
                                                 sessionStorage.SelectedFOWST = e.ID;
                                                 sessionStorage.SelectedFOWSTName = e.FowstName;

                                                 if (vReport == 'InventoryMovement') {
                                                     $('.lblInventoryMovementFOWST').text(e.FowstName); 
                                                     GetInventoryMovement($('.lblInventoryMovementDate').text());
                                                 } else if (vReport == 'DeliveryReport') {
                                                     $('.lblDeliveryReportFOWST').text(e.FowstName);
                                                     GetDeliveryReport($('.lblDeliveryReport').text());
                                                 } else if (vReport == 'HourlySalesReport') {
                                                     $('.lblHourlySalesReportFOWST').text(e.FowstName);
                                                     GetHourlySalesReport($('.lblHourlySalesReport').text());
                                                 } else if (vReport == 'MonthlyReport') {
                                                     $('.lblMonthlySalesReportFOWST').text(e.FowstName);
                                                     GetMonthlySalesReport();
                                                 } else if (vReport == 'SpecialDiscountReport') {
                                                     $('.lblSpecialDiscountReportFOWST').text(e.FowstName);
                                                     GetSpecialDiscountReport();
                                                 }
                                             }
                                         }
                                 ], { IsAll: true, IsByEmployeeFOWST: true })
                             }

                             function DisplayGroupFOWST() {
                                 BrowsePopup('LibraryFranchiseGroup', 'Select Group', [
                                         {
                                             AutoSearch: true,
                                             DisplayText: [{ Column: 'GroupName' }],
                                             Click: function (e) {
                                                 sessionStorage.SelectedGroup = e.ID;
                                                 sessionStorage.SelectedGroupName = e.GroupName;

                                             }
                                         }
                                 ] //, { IsAll: true, IsByEmployeeFOWST: true }
                                 )
                             }
                         })
                     })

                 })
             })
     });