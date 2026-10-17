require(['common'
    ], function () {
        var Multiplier = 0;
        var TableNumber = 0;
        var vTablesWithOrders;

        $(function () {
            
            RenameTables();
            ResetMainTables();

            $('#mtp0').css({
                'height': '60px',
                'margin': '5px',
                'border-radius': '50px',
                'text-align': 'center',
                'line-height': '60px',
                'background-color': '#3498db',
                'color': '#FFF',
                'cursor': 'pointer',
                'font-size': '15px'
            });

            $('#btnBack').click(function() {
                location = MergeUrlParameters({
                    IsMobile: IsMobile ? 1 : 0
                }, '/People/PeopleMenu');
            });

            //gicomment kay naa nay click event sa Bs_SalesMenu.js by arnie 1-11-21
            //$('#butViewMode').click(function () {
            //    NewMessageBox('', 'View Mode', [
            //            {
            //                Text: 'Retail Mode',
            //                Size: 4,
            //                Click: function () {
            //                    var pParams = GetUrlParameters();

            //                    location = MergeUrlParameters({
            //                        mode: pParams.mode,
            //                        OrgID: sessionStorage.OrgID,
            //                        rsd: pParams.rsd,
            //                        IsMobile: CBool(IsMobile) ? 1 : 0
            //                    }, '/SalesMenu');
            //                }
            //            },
            //            {
            //                Text: 'Hot Button Mode',
            //                Size: 4,
            //                Click: function () {
            //                    if (proj.PosMode) {
            //                        if ($(window).width() > proj.WideWidth) {
            //                            location = MergeUrlParameters({
            //                                OrgID: sessionStorage["OrgID"],
            //                                mode: 'pos',
            //                                IsMobile: IsMobile ? 1 : 0
            //                            }, '/SalesMenu/POSViewAll');
            //                        } else {
            //                            goToHotButtonPage();
            //                        }
            //                    } else {
            //                        goToHotButtonPage();
            //                    }
            //                }
            //            },
            //            {
            //                Text: 'Table View Mode',
            //                Size: 4,
            //                Click: function () {
                                
            //                }
            //            }
            //    ], {AutoDismiss: true, CloseButton: 'enabled'}
            //    )
            //})
            
            $('[name="btnTableNumber"]').click(function () {
                //TableNumber = GetTableNumber(this.id);
//                location = MergeUrlParameters({
//                    OrgID: sessionStorage["OrgID"],
//                    mode: 'pos',
//                    IsMobile: IsMobile ? 1 : 0,
//                    TableNo: $(this).text() //third span from the button since button also creates <span>
                //                }, '/SalesMenu/POSViewAll');

                var hasOrder = false;

                for (var i = 0; i <= vTablesWithOrders.length - 1; i++) {
                    if ($(this).text() == vTablesWithOrders[i].CustomerTableNo) {
                        hasOrder = true;
                        break;
                    }
                }

                if (hasOrder) {
                    if ($(window).width() > proj.WideWidth) { // 932 old . force non-widescreen
                        location = MergeUrlParameters({
                            OrgID: sessionStorage["OrgID"],
                            mode: 'pos',
                            IsMobile: IsMobile ? 1 : 0,
                            TableNo: $(this).text() //third span from the button since button also creates <span>
                        }, '/SalesMenu/POSViewAll');
                    } else {
                        //redirect to retail mode first
                        location = MergeUrlParameters({
                            OrgID: sessionStorage["OrgID"],
                            mode: 'pos',
                            IsMobile: IsMobile ? 1 : 0,
                            TableView: 'true',
                            TableNo: $(this).text() //third span from the button since button also creates <span>
                        }, '/SalesMenu');
                    }
                } else {
                    TableNumber = $(this).text();
                    $('#popupTotalNumberOfCustomer').popup('open');
                    $('#txtTotalNumberOfCustomer').focus();
                }
                
            });
            
            $('#btnSaveTotalNumberOfCustomer').live('click', function () {
                if (window.location.pathname.toLowerCase() != '/tableview/bsnewtable') { //WALAY LABOT ANG NEW TABLE VIEW UI by arnie 12/1/2020
                    sessionStorage.TotalNumberOfCustomer = $('#txtTotalNumberOfCustomer').val();
                    $('#popupTotalNumberOfCustomer').popup('close');

                    if ($(window).width() > proj.WideWidth) { // 932 old . force non-widescreen
                        location = MergeUrlParameters({
                            OrgID: sessionStorage["OrgID"],
                            mode: 'pos',
                            IsMobile: IsMobile ? 1 : 0,
                            TableNo: TableNumber //third span from the button since button also creates <span>
                        }, '/SalesMenu/POSViewAll');
                    } else {
                        //redirect to retail mode first
                        location = MergeUrlParameters({
                            OrgID: sessionStorage["OrgID"],
                            mode: 'pos',
                            IsMobile: IsMobile ? 1 : 0,
                            TableView: 'true',
                            TableNo: TableNumber //third span from the button since button also creates <span>
                        }, '/SalesMenu');
                    }

                    //$('#lnkTotalNumberOfCustomer').text($('#txtTotalNumberOfCustomer').val());
                }                
            });

            $('#mtp0').click(function () {
                Multiplier = 0;
                RenameTables();
                ResetMainTables();
                $('#mtp0').css({
                    'height': '60px',
                    'margin': '5px',
                    'border-radius': '50px',
                    'text-align': 'center',
                    'line-height': '60px',
                    'background-color': '#3498db',
                    'color': '#FFF',
                    'cursor': 'pointer',
                    'font-size': '15px'
                });
            })

            $('#mtp1').click(function () {
                Multiplier = 1;
                RenameTables();
                ResetMainTables();
                $('#mtp1').css({
                    'height': '60px',
                    'margin': '5px',
                    'border-radius': '50px',
                    'text-align': 'center',
                    'line-height': '60px',
                    'background-color': '#3498db',
                    'color': '#FFF',
                    'cursor': 'pointer',
                    'font-size': '15px'
                });
            })

            $('#mtp2').click(function () {
                Multiplier = 2;
                RenameTables();
                ResetMainTables();
                $('#mtp2').css({
                    'height': '60px',
                    'margin': '5px',
                    'border-radius': '50px',
                    'text-align': 'center',
                    'line-height': '60px',
                    'background-color': '#3498db',
                    'color': '#FFF',
                    'cursor': 'pointer',
                    'font-size': '15px'
                });
            })

            $('#mtp3').click(function () {
                Multiplier = 3;
                RenameTables();
                ResetMainTables();
                $('#mtp3').css({
                    'height': '60px',
                    'margin': '5px',
                    'border-radius': '50px',
                    'text-align': 'center',
                    'line-height': '60px',
                    'background-color': '#3498db',
                    'color': '#FFF',
                    'cursor': 'pointer',
                    'font-size': '15px'
                });
            })

            $('#mtp4').click(function () {
                Multiplier = 4;
                RenameTables();
                ResetMainTables();
                $('#mtp4').css({
                    'height': '60px',
                    'margin': '5px',
                    'border-radius': '50px',
                    'text-align': 'center',
                    'line-height': '60px',
                    'background-color': '#3498db',
                    'color': '#FFF',
                    'cursor': 'pointer',
                    'font-size': '15px'
                });
            })

        }); // $(function() {

        function goToHotButtonPage() {
            sessionStorage.PosMode = 'Hot Button Mode';
            proj.ProductID = '';

            $('#txtOrder').html('');
            $('#txtQtyUnit').html('');
            startclock('#clockHotButtons');
            proj.CartStock = [];
            ChangePage('/SalesMenu/HotButtons');

            //session storage cleared on peoplemenu js

            if (sessionStorage.IsHotButtonVisited == 'true') {
                $('#divSpace').hide();
                //                        $.mobile.changePage('#pageButtons');
                //                                        GetData(sessionStorage.HotButtonMenuID);
                $('#divSpace').css('height', '0px');
            }

            if (CStr2(sessionStorage.CustomerTableNo) != '') {

                $('#lblCustomerNameHotButtons').text(sessionStorage.CustomerTableNo);
                try {

                    $('#selTableNoHotButtons').val(sessionStorage.CustomerTableNo).selectmenu('refresh', true);
                } catch (e) {

                    $('#selTableNoHotButtons').val(sessionStorage.CustomerTableNo);
                    $('#selTableNoHotButtons').selectmenu();

                }

                $('#lblLabelHotButtons').html('Customer / Table No.:<br/>');

            }

            if (CStr2(sessionStorage.GasPumpNo) != '') {

                $('#lblCustomerNameHotButtons').text(sessionStorage.GasPumpNo);

                try {
                    $('#selGasPumpNoHotButtons').val(sessionStorage.GasPumpNo).selectmenu('refresh', true);
                } catch (e) {
                    $('#selGasPumpNoHotButtons').val(sessionStorage.GasPumpNo);
                    $('#selGasPumpNoHotButtons').selectmenu();

                }

                $('#lblLabelHotButtons').html('Gas Pump No.:<br/>');
            }

            if (sessionStorage.GasPumpNo == '' && sessionStorage.CustomerTableNo == '') {

                $('#lblCustomerNameHotButtons').text(sessionStorage.CustomerOrganizationName);

                try {
                    $('#selTableNoHotButtons').val('').selectmenu('refresh', true);

                } catch (e) {
                    $('#selTableNoHotButtons').val('');
                    $('#selTableNoHotButtons').selectmenu();

                }
                $('#lblLabelHotButtons').html('Customer:<br/>');

            }
        }

        function ResetMainTables() {
//            $('#mtp0').css('color', 'black');
//            $('#mtp1').css('color', 'black');
//            $('#mtp2').css('color', 'black');
//            $('#mtp3').css('color', 'black');
//            $('#mtp4').css('color', 'black');
            $('#mtp0, #mtp1, #mtp2, #mtp3, #mtp4').css({
                'height': '60px',
                'margin': '5px',
                'border-radius': '50px',
                'text-align': 'center',
                'line-height': '60px',
                'background-color': '#A5C7DD',
                'color': '#FFF',
                'cursor': 'pointer',
                'font-size': '15px'
            })
        }
        
        function RenameTables() {

            for (i = 1; i <= 20; i++) {
                $('#tbl' + i).text((Multiplier * 20) + i);
            }

//            $('#tbl1').text((Multiplier * 20) + 1);
//            $('#tbl2').text((Multiplier * 20) + 2);
//            $('#tbl3').text((Multiplier * 20) + 3);
//            $('#tbl4').text((Multiplier * 20) + 4);
//            $('#tbl5').text((Multiplier * 20) + 5);
//            $('#tbl6').text((Multiplier * 20) + 6);
//            $('#tbl7').text((Multiplier * 20) + 7);
//            $('#tbl8').text((Multiplier * 20) + 8);
//            $('#tbl9').text((Multiplier * 20) + 9);
//            $('#tbl10').text((Multiplier * 20) + 10);
//            $('#tbl11').text((Multiplier * 20) + 11);
//            $('#tbl12').text((Multiplier * 20) + 12);
//            $('#tbl13').text((Multiplier * 20) + 13);
//            $('#tbl14').text((Multiplier * 20) + 14);
//            $('#tbl15').text((Multiplier * 20) + 15);
//            $('#tbl16').text((Multiplier * 20) + 16);
//            $('#tbl17').text((Multiplier * 20) + 17);
//            $('#tbl18').text((Multiplier * 20) + 18);
//            $('#tbl19').text((Multiplier * 20) + 19);
//            $('#tbl20').text((Multiplier * 20) + 20);

            GetTableStatus();
        }
        function goToHotButtonPage() {
            sessionStorage.PosMode = 'Hot Button Mode';
            proj.ProductID = '';

            $('#txtOrder').html('');
            $('#txtQtyUnit').html('');
            startclock('#clockHotButtons');
            ChangePage('/SalesMenu/HotButtons');

            //session storage cleared on peoplemenu js

            if (sessionStorage.IsHotButtonVisited == 'true') {
                $('#divSpace').hide();
                //                        $.mobile.changePage('#pageButtons');
                //                                        GetData(sessionStorage.HotButtonMenuID);
                $('#divSpace').css('height', '0px');
            }

            if (CStr2(sessionStorage.CustomerTableNo) != '') {

                $('#lblCustomerNameHotButtons').text(sessionStorage.CustomerTableNo);
                try {

                    $('#selTableNoHotButtons').val(sessionStorage.CustomerTableNo).selectmenu('refresh', true);
                } catch (e) {

                    $('#selTableNoHotButtons').val(sessionStorage.CustomerTableNo);
                    $('#selTableNoHotButtons').selectmenu();

                }

                $('#lblLabelHotButtons').html('Customer / Table No.:<br/>');

            }

            if (CStr2(sessionStorage.GasPumpNo) != '') {

                $('#lblCustomerNameHotButtons').text(sessionStorage.GasPumpNo);

                try {
                    $('#selGasPumpNoHotButtons').val(sessionStorage.GasPumpNo).selectmenu('refresh', true);
                } catch (e) {
                    $('#selGasPumpNoHotButtons').val(sessionStorage.GasPumpNo);
                    $('#selGasPumpNoHotButtons').selectmenu();

                }

                $('#lblLabelHotButtons').html('Gas Pump No.:<br/>');
            }

            if (sessionStorage.GasPumpNo == '' && sessionStorage.CustomerTableNo == '') {

                $('#lblCustomerNameHotButtons').text(sessionStorage.CustomerOrganizationName);

                try {
                    $('#selTableNoHotButtons').val('').selectmenu('refresh', true);

                } catch (e) {
                    $('#selTableNoHotButtons').val('');
                    $('#selTableNoHotButtons').selectmenu();

                }
                $('#lblLabelHotButtons').html('Customer:<br/>');

            }
        } // end of gotohotbuttonpage

        function GetTableStatus() {
            var pSql = '';
            var vData;

            ResetTableStatus();
            
            pSql = "SELECT REPLACE(CustomerTableNo, 'TBL_', '') as CustomerTableNo from transinvoice " +
                    " LEFT JOIN TransCustomerPaymentInvoices on " +
                    " TransCustomerPaymentInvoices.InvoiceID = TransInvoice.ID " +
                    " WHERE CustomerTableNo <> '' " +
                    " AND TransCustomerPaymentInvoices.InvoiceID IS NULL " +
                    " AND CAST(REPLACE(CustomerTableNo, 'TBL_', '') as int) BETWEEN  " + CDec2((Multiplier * 20) + 1) + " AND " + CDec2((Multiplier * 20) + 20) +
                    '\n AND TransInvoice.FowstID = ' + ReadyString(sessionStorage.FowstID);

            vData = Sql.Execute(pSql);
            
            vTablesWithOrders = vData.Rows;

            if (vData.Rows.length > 0) {
                for (i = 0; i < vData.Rows.length; i++) {
                    $('.tablestatuscolor' + (CDec2(vData.Rows[i].CustomerTableNo) - CDec2(Multiplier * 20))).css({
                        'height': '60px',
                        'margin': '5px',
                        'border-radius': '50px',
                        'text-align': 'center',
                        'line-height': '60px',
                        'background-color': '#BD574C',
                        'color': '#FFF',
                        'cursor': 'pointer',
                        'font-size': '25px'
                    });
                }
            }
        }
        
        function ResetTableStatus() {
            for (i = 1; i <= 20; i++) {
                    $('.tablestatuscolor' + i).css({
                        'height': '60px',
                        'margin': '5px',
                        'border-radius': '50px',
                        'text-align': 'center',
                        'line-height': '60px',
                        'background-color': '#16a085',
                        'color': '#FFF',
                        'cursor': 'pointer',
                        'font-size': '25px'                        
                    });
                }
        }
        
        function GetTableNumber(TableID) {
            switch (TableID) {
                case "tbl1":
                    return 1; break;
                case "tbl2":
                    return 2; break;
                case "tbl3":
                    return 3; break;
                case "tbl4":
                    return 4; break;
                case "tbl5":
                    return 5; break;
                case "tbl6":
                    return 6; break;
                case "tbl7":
                    return 7; break;
                case "tbl8":
                    return 8; break;
                case "tbl9":
                    return 9; break;
                case "tbl10":
                    return 10; break;
                case "tbl11":
                    return 11; break;
                case "tbl12":
                    return 12; break;
                case "tbl13":
                    return 13; break;
                case "tbl14":
                    return 14; break;
                case "tbl15":
                    return 15; break;
                case "tbl16":
                    return 16; break;
                case "tbl17":
                    return 17; break;
                case "tbl18":
                    return 18; break;
                case "tbl19":
                    return 19; break;
                case "tbl20":
                    return 20; break;

            }
        }
    });                                                                                                                                                                                                                                                                                                                                                                                                                  // require require


