require(['common', '/Scripts/sources/crm/qrcodeNew.js', '/Scripts/sources/CRMDashboard/customcalendar.js'], function () {
    $('document').ready(function () {
        setTimeout(function () {
            $('.ItemsBreadCrumbs').empty();
            AddBreadCrumb({
                text: 'Home', Click: function () {
                    $('#divErpMain').pageshow();
                }
            }, '.ItemsBreadCrumbs')
        }, 200)

        var htmlCreditAndCollection;
        var htmlDTR;
        var htmlCrm;

        $('#btnErpRevenue').click(function () {
            $('#divErpRevenue').pageshow();
            AddBreadCrumb({
                text: 'Revenue', Click: function () {
                    $('#divErpRevenue, #divRevMainMenu').pageshow();
                }
            }, '.ItemsBreadCrumbs')
        })

        $('#btnErpDisbursement').click(function () {
            $('#divErpDisbursement').pageshow();
            AddBreadCrumb({
                text: 'Disbursement', Click: function () {
                    $('#divErpDisbursement, #divDisburseMainMenu').pageshow();
                }
            }, '.ItemsBreadCrumbs')
        })

        $('#btnErpDTR').click(function () {
            AddBreadCrumb({
                text: 'Daily Time Record', Click: function () {
                    $('#divDailyTimeRecord').pageshow();
                }
            }, '.ItemsBreadCrumbs')

            CallPage('#divDailyTimeRecord', htmlDTR, 'http://192.168.1.211:9090/DTRReports', function (e) {
                $('#pageCac').pageshow();
                htmlDTR = e;
            })
        })

        $('#btnErpCustomerSupport').click(function () {
            sessionStorage.CRMDashboardPage = 'leadgen';

            CallPage('#divDailyTimeRecord', htmlCrm, '/CRMDashboard/db', function (e) {
                $('#pageMainDashboard').pageshow();
                htmlCrm = e;

                if (typeof ShowProspectList !== 'undefined') {
                    ShowProspectList();
                }
            })
        })

        //BUTTONS FOR REVENUE
        $('#btnRevQuotation').click(function () {
            $('#divOrderProcessing').pageshow();
            AddBreadCrumb({
                text: 'Quotation', Click: function () {
                    $('#divQuotation, #divQuotationMain').pageshow();
                }
            }, '.ItemsBreadCrumbs')
        })

        $('#btnRevOrderProcessing').click(function () {
            $('#divOrderProcessing').pageshow();
            AddBreadCrumb({
                text: 'Order Processing', Click: function () {
                    $('#divOrderProcessing, #divOrderProcessingMain').pageshow();
                }
            }, '.ItemsBreadCrumbs')
        })

        $('#btnRevOrderFulfillment').click(function () {
            $('#divOrderFulfillment').pageshow();
            AddBreadCrumb({
                text: 'Order Fulfillment', Click: function () {
                    $('#divOrderFulfillment, #divOrderFulfillmentMain').pageshow();
                }
            }, '.ItemsBreadCrumbs')
        })

        $('#btnQuoteDateNeeded, #btnOPDateNeeded, #btnOPPODateNeeded, #btnOFPODate, #btnOFDateNeeded, #btnInvoicingSODate, ' +
            '#btnInvoicingQuoteDate, #btnInvoicingPODate, #btnInvoicingDateNeeded, ' +
            '#btnPurchasingQuoteDate, #btnPurchasingDateNeeded, #btnJOQuoteDate, #btnJODateNeeded').click(function () {
            var dis = $(this);
            SingleCalendar({
                Click: function (e) {
                    dis.val(e)
                }
            })
        })

        $('#btnRevInvoicing').click(function () {
            $('#divInvoicing').pageshow();
            AddBreadCrumb({
                text: 'Invoicing', Click: function () {
                    $('#divInvoicing, #divInvoicingMain').pageshow();
                }
            }, '.ItemsBreadCrumbs')
        })

        $('#btnRevCreditAndCollection').click(function () {
            AddBreadCrumb({
                text: 'Credit and Collection', Click: function () {
                    $('#divCreditAndCollection').pageshow();
                }
            }, '.ItemsBreadCrumbs')

            CallPage('#divCreditAndCollection', htmlCreditAndCollection, '/CreditAndCollection', function (e) {
                $('#pageCac').pageshow();
                htmlCreditAndCollection = e;

                if (typeof Cac !== 'undefined') {
                    Cac.Show();
                }
            })
        })
        //END REVENUE

        //BUTTONS FOR DISBURSEMENT
        $('#btnDisbursePurchasing').click(function () {
            $('#divPurchasing').pageshow();
            AddBreadCrumb({
                text: 'Purchasing', Click: function () {
                    $('#divPurchasing, #divPurchasingMain').pageshow();
                }
            }, '.ItemsBreadCrumbs')
        })

        $('#btnDisburseJO').click(function () {
            $('#divJO').pageshow();
            AddBreadCrumb({
                text: 'JO', Click: function () {
                    $('#divJO, #divJOMain').pageshow();
                }
            }, '.ItemsBreadCrumbs')
        })
        //END DISBURSEMENT

        $('.btnOngoing').click(function () {
            NewMessageBox('This feature is ongoing development', 'Message');
        })

        window.CallPage = function (container, htmlCarrier, url, exe){
            //exe is optional
            if(exe != undefined){
                if(!$.isFunction(exe)){
                    NewMessageBox('4th parameter must be a function', 'Message');
                    return;
                }
            }

            $(container).html('Loading ...').pageshow();

            if(CStr2(htmlCarrier) == ''){
                AjaxSend({
                    ExactUrl: url + '?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile,
                    type: 'GET',
                    dataType: 'text',
                    success: function (vData) {
                        htmlCarrier = vData;
                        $(container).html('');
                        $(container).append(vData).trigger('create');
                        exe(htmlCarrier);
                    }
                });
            } else {
                $(container).html('').append(htmlCarrier).trigger('create');
                exe(htmlCarrier);
            }
        }
    })
})