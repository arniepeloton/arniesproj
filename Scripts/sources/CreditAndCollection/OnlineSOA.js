require(['common'],
function () {
    require([''], function(){
        $(document).ready(function () {
            sessionStorage.OrgID = GetUrlParameters().o;
            sessionStorage.FowstID = GetUrlParameters().f;
            sessionStorage.IsSOALink = true

            AjaxSend({
                url: '/CreditAndCollection/ValidateSoaLink',
                data: { Token: GetUrlParameters().t, CustomerID: GetUrlParameters().c, FowstID: GetUrlParameters().f },
                success: function (e) {
                    if (CBool(e.Invalid)) {
                        location = '/CreditAndCollection/InvalidSOALink?o=' + sessionStorage.OrgID
                    } else {
                        require(['promise!/Scripts/sources/ERP/AccountDetails/controller.js?v' + assetversion,
                        '/Scripts/sources/CreditAndCollection/CacPaymentPopup.js?v=' + assetversion], function () {
                            ERP.AccountDetails.controller.Mode = 'AR';
                            ERP.AccountDetails.controller.CustomerID = GetUrlParameters().c;
                            ERP.AccountDetails.controller.CustomerName = e.CompanyName;
                            ERP.AccountDetails.controller.FromEcommerce = true;
                            //$('#popupAccountDetailsContent').insertAfter($('#popupAccountDetails .popup-header'));
                            //$('#popupAccountDetails').modal('show');
                            ERP.AccountDetails.controller.GetCustomerCreditDetails(GetUrlParameters().c, 'has payable');

                            var from = $.format.date(new Date(CJsonDate(e.DateFrom)), 'MMM dd, yyyy');
                            var to = $.format.date(new Date(CJsonDate(e.DateTo)), 'MMM dd, yyyy');

                            $('#lblSOALinkDateFrom').text(from);
                            $('#lblSOALinkDateTo').text(to);
                            $('#divmain').pageshow();

                            $('#butGenerateBilling').click(function () {
                                AjaxSend({
                                    url: '/CreditAndCollection/CreateBilling',
                                    data: {
                                        CustomerID: GetUrlParameters().c
                                    },
                                    success: function (vData) {
                                        ViewReport(vData.Filename);
                                        if (vData.Filename) {
                                            NewMessageBox('You should be viewing the PDF by now. If you are unable to view the PDF, please check your web browser\'s popup settings.');
                                        }
                                    }
                                }); // END AjaxSend({

                                function ViewReport(vFilename) {
                                    var pUrl = MergeUrlParameters({
                                        Filename: vFilename
                                    }, '/CreditAndCollection/GetBilling');
                                    window.open(pUrl, '_blank',
                                        'width=' + window.innerWidth +
                                        ',height=' + window.innerHeight +
                                        ',scrollbars=auto');
                                }
                            })
                        });
                    }                    
                }
            })
        })
    })
})