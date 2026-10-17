define([
    'promise!/Scripts/sources/ERP/ESalesReport/settings/controller.js',
    'promise!/Scripts/sources/ERP/ESalesReport/work/controller.js'
], function () {
    var mDeferred = $.Deferred();
    var mHtml;
    var dateFrom = new Date();
    var dateTo = new Date();

    (function staticInit() {

        AjaxSend({
            ExactUrl: '/Scripts/sources/ERP/ESalesReport/main/view.html?crc=' + assetversion,
            type: 'GET',
            dataType: 'text',
            success: function (vData) {
                mHtml = vData;
                ERP.ESalesReport.main.controller = ERP.ESalesReport.main.controller()
                mDeferred.resolve();
            }
        });
    })();

    CreateStringObject('ERP.ESalesReport.main.controller', null, function () {

        var mOut = {};
        var mElement;        

        mElement = $(mHtml);
        mOut.element = mElement;

        $('#divErpESalesReport').append(mElement);

        window.FinancialStatmentSettingsController = new ERP.FinancialStatement.settings.controller();
        window.FinancialStatmentWorkController = new ERP.FinancialStatement.work.controller();
        new ERP.ESalesReport.work.controller(); // singleton

        //dire sa ni kay gidali ang project

        $('#panIncomeStatementGroup').popup();
        //$('#panMoreDetails').popup();

        sessionStorage.PrintingOfflineData = false;

        $('#btnIncomeStatement').click(function () {
            $('#panIncomeStatement').modal('show');
        })

        $('#btnOrgGroupTotal2').click(function () {
            $('#panIncomeStatementGroup').popup('open');
        })

        $('#lblIncomeStatementGrossSales').click(function () {
            //$('#panMoreDetails').popup('open')
            NewMessageBox('Drill down [AutoText=Parent Variable Name] by: <br>' +
                        "1st: <select id='selGroup1' data-role='none' class='form-select'></select> <br>" +
                        "2nd: <select id='selGroup2' data-role='none' class='form-select'></select>", 'See more details for (Sales)', [
                      {
                          Text: 'Okay',
                          Click: function () {
                              $('#accBodyIncomeStatementGrossSales').html('');
                              GetGrossSales(false);
                          }
                      },
                      {
                          Text: 'Cancel'
                      }
                        ]);

            SetGroupValue();
        })

        $('#butESalesReport').click(function () {
            NewPromptUserAccess('E-Sales Report', {
                OnClickOKSuccess: function () {
            require(['promise!/Scripts/sources/ERP/FinancialStatement/main/controller.js?v=' + assetversion], function () {
                $('#divErpFinancialStatement').pageshow();
                AddBreadCrumb({
                    text: 'Financial Statements', Click: function () {
                        $('#divErpFinancialStatement').pageshow();
                    }
                }, '.ItemsBreadCrumbs')
            })
                }
            });
        })

        $('#lnkPeriod').click(function () {

            PopupPeriod({
                Click: function (e) {
                    $('#lnkPeriod').text($.format.date(new Date(e.From), 'MM/dd/yyyy') + ' to ' + $.format.date(new Date(e.To), 'MM/dd/yyyy'))

                    dateFrom = e.From;
                    dateTo = e.To;
                    $('#lblIncomeStatementGrossSales').text(AccountingFormat(0));
                    $('#accBodyIncomeStatementGrossSales').html('');

                    GetGrossSales(true);
                }
            })

        })

        $('#lnkPeriod').text($.format.date(new Date(), 'MM/dd/yyyy') + ' to ' + $.format.date(new Date(), 'MM/dd/yyyy'))
        GetGrossSales(true);

        //end of rush project
        $('#divTopSellingAndTopProfitableSkus').click(function() {
            require(['promise!/Scripts/sources/ERP/TopSellingAndTopProfitableSkus/controller.js?crc=' + assetversion], function () {
                new ERP.TopSellingAndTopProfitableSkus.controller().Popup();
            })
        });


        $('#divBir').click(function() {
            NewPromptUserAccess('BIR - Sales', {
                OnClickOKSuccess: function () {
                    require(['promise!/Scripts/sources/ERP/Bir/controller.js?crc=' + assetversion], function () {
                        ERP.Bir.Popup();
                    })
                }
            });
        });

        $('#divBirPurchases').click(function() {
            NewPromptUserAccess('BIR - Purchases', {
                OnClickOKSuccess: function () {
                    require(['promise!/Scripts/sources/ERP/Bir/controller.js?crc=' + assetversion], function () {
                        ERP.Bir.Popup(true);
                    })
                }
            });
        });

        $('#butESalesReportWork').click(function () {
            require(['promise!/Scripts/sources/ERP/ESalesReport/work/controller.js?v=' + assetversion], function () {
                $('#divErpESalesReport').pageshow();
                AddBreadCrumb({
                    text: 'E-Sales Report', Click: function () {
                        
                    }
                }, '.ItemsBreadCrumbs')
            })
        });

        return mOut;

    }); // END CreateStringObject('App.controller.PriceChange', null, function() {

    function GetGrossSales(IsGross) {

        var data = {            
            From: dateFrom,
            To: dateTo
        }

        if (!IsGross) {
            data.SliceBy = $('#selGroup1').val() + ',' + $('#selGroup2').val();
        }

        AjaxSend({
            url: 'DashboardsAndReports/GetIncomeStatement',
            data: data,
            success: function (vData) {
                if (vData.Results != undefined) {
                    if (data.SliceBy == undefined) {
                        $('#lblIncomeStatementGrossSales').text(AccountingFormat(vData.Results.Rows[0].Total));
                    } else {
                        var vRows = vData.Filter1.Rows;                        
                        var vColumnName = '';

                        var vRawData = vData.Results.Rows;
                        var vRawDataColumnName = '';
                        var vComparedColumn = '';

                        if (vRows.length > 0) {
                            vColumnName = vData.Filter1.ColumnsArray[0];
                        }

                        if (vRawData.length > 0) {
                            vRawDataColumnName = vData.Results.ColumnsArray[2]; //raw data column name
                            vComparedColumn = vData.Results.ColumnsArray[1]
                        }

                        for (var i = 0; i <= vRows.length - 1; i++) {
                            var vHTML = "";
                            var innervHTML = "";

                            innervHTML = "<div style='text-align:center'> <table width=100% class='table table-hover'>" +
                                              "<body>"

                            for (var j = 0; j <= vRawData.length - 1; j++) {
                              
                                if (vRows[i][vColumnName] == vRawData[j][vComparedColumn]) {
                                    innervHTML += "<tr>" +
                                                "<td style='text-align:left'>" +
                                                      vRawData[j][vRawDataColumnName] +
                                                        "</td>" +
                                                        "<td style='text-align:right'>" +
                                                            AccountingFormat(vRawData[j].Total) +
                                                        "</td>" +
                                                    "</tr>"
                                }                                      
                            }

                            innervHTML += "</body> </table> </div> "

                            vHTML = "<div class='accordion mb-3'>" +
                                        "<div class='card'>" +
                                            "<div class='card-header p-0'>" +
                                                "<button data-target='#acc" + i + "' class='btn btn-light btn-block text-left p-3 rounded-0' data-role='none' data-toggle='collapse'><b>" + vRows[i][vColumnName] + "<span style='float:right;'>" + AccountingFormat(vRows[i].TotalAmount) + "</span></b></button>" +
                                            "</div>" +
                                            "<div id='acc"+ i +"' class='collapse'>" +
                                                "<div class='card-body'>" +
                                                    innervHTML +
                                                "</div>" +
                                            "</div>" +
                                        "</div>" +
                                     "</div>" //end accordion
                            
                            $('#accBodyIncomeStatementGrossSales').append(vHTML)
                        }   
                    }                    
                }
            }
        });
    }

    function SetGroupValue() {
        var group1 = $('#selGroup1');
        var group2 = $('#selGroup2');

        group1.empty();
        group2.empty();

        group1.append(new Option('FOWST', 'FOWST', false));
        group1.append(new Option('Brand', 'Brand', false));
        group1.append(new Option('Sales Representative', 'SalesRepresentative', false));
        group1.append(new Option('Product', 'Product', false));
        group1.append(new Option('Delivery Method', 'DeliveryMethod', false));
        group1.append(new Option('Order Method', 'OrderMethod', false));
        //group1.append(new Option('Customer', 'Customer', false));
        group1.append(new Option('Encoder', 'Encoder', false));

        group2.append(new Option('Brand', 'Brand', false));
        group2.append(new Option('FOWST', 'FOWST', false));        
        group2.append(new Option('Sales Representative', 'SalesRepresentative', false));
        group2.append(new Option('Product', 'Product', false));
        group2.append(new Option('Delivery Method', 'DeliveryMethod', false));
        group2.append(new Option('Order Method', 'OrderMethod', false));
        //group2.append(new Option('Customer', 'Customer', false));
        group2.append(new Option('Encoder', 'Encoder', false));

        //try{
        //    $('#selGroup1').selectmenu('refresh');
        //    $('#selGroup2').selectmenu('refresh');
        //}catch(e){
        //    $('#selGroup1').selectmenu();
        //    $('#selGroup2').selectmenu();
        //}
    }

    return mDeferred.promise();
})