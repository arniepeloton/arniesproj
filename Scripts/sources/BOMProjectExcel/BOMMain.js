require([
    'common'
], function () {
    require([
        (OfflineMode ? '/Scripts/sources/People/PeopleMenu_offline.js?crc=' : '')
    ], function () {
        $(document).ready(function () {
            var pParam;
            ShowPersonNameInfo(sessionStorage.PeopleID);
            var htmlMaterialRequisition = '';
            var htmlPurchaseOrder = '';

            $("#divMainParentHotButton").on("scroll", function () {
                var cur = $(this).scrollLeft();
                if (cur == 0) {
                    $('#divParentHotButtons').addClass('shadow-right').removeClass('shadow-left');
                }
                else {
                    var max = $(this)[0].scrollWidth - $(this).parent().width();
                    if (cur == max) {
                        $('#divParentHotButtons').addClass('shadow-left').removeClass('shadow-right');
                    } else {
                        $('#divParentHotButtons').addClass('shadow-right shadow-left');
                    }
                }
            });
            $("#divMainParentHotButton").trigger("scroll");

            $('body')
            .on('click', '.btnHome', function () {
                //window.location = '/People/PeopleMenu';
                AjaxSend({
                    url: 'Login3/TestCookies',
                    success: function (vData) {

                        AjaxSend({
                            url: "People/setSignOut",
                            success: function (vData) {
                                window.open(GetPathFormat("/Signin/Project?mode=logout"), "_self");
                            }
                        });

                    },
                    error: function (vError) {
                        AjaxSend({
                            url: "People/setSignOut",
                            success: function (vData) {
                                window.open(GetPathFormat("/Signin/Project?mode=logout"), "_self");
                            }
                        });
                    }

                });
            })
            .on('click', '.btnMaterialsRequisition', function () {
                BreadCrumbs('.ItemsBreadCrumbs',
                        [{ Text: 'Material Requisition' }]
                    )
                $('#pageMaterialRequisition').pageshow()
                if (htmlMaterialRequisition == "") {
                    AjaxSend({
                        ExactUrl: '/MaterialsRequisition?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile,
                        type: 'GET',
                        dataType: 'text',
                        success: function (vData) {
                            htmlMaterialRequisition = vData;
                            $('#pageMaterialRequisition').empty();
                            $('#pageMaterialRequisition').append(vData).trigger('create');
                            App.Deferred.fulfill('FQ26E5TP76');
                        }
                    });
                }
                else {
                    $('#pageMaterialRequisition').empty();
                    $('#pageMaterialRequisition').append(htmlMaterialRequisition).trigger('create');
                    MatReq.GetProjectNameList(); // /BOMProjectExcel/MaterialRequisition.js
                }
            })
            .on('click', '.btnCashAdvances', function () {

            })
            .on('click', '.btnApprovals', function () {

            })
            .on('click', '.btnPurchaseOrders', function () {
                BreadCrumbs('.ItemsBreadCrumbs',
                        [{ Text: 'Purchase Order' }]
                    )
                $('#pagePurchaseOrder').pageshow()
                if (htmlPurchaseOrder == "") {
                    AjaxSend({
                        ExactUrl: '/ProjectPurchaseOrder?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile,
                        type: 'GET',
                        dataType: 'text',
                        success: function (vData) {
                            htmlPurchaseOrder = vData;
                            $('#pagePurchaseOrder').empty();
                            $('#pagePurchaseOrder').append(vData).trigger('create');
                            App.Deferred.fulfill('FQ26E5TP76');
                        }
                    });
                }
                else {
                    $('#pagePurchaseOrder').empty();
                    $('#pagePurchaseOrder').append(htmlPurchaseOrder).trigger('create');
                    PO.GetProjectNameList(); // /BOMProjectExcel/PurchaseOrder.js
                }
            })
            .on('click', '.btnStockTransfers', function () {
                location = MergeUrlParameters({
                    OrgID: sessionStorage.OrgID,
                    IsReceiving: 0,
                    IsMobile: CBool(IsMobile) ? 1 : 0
                }, '/Project/Receiving');
            })
            .on('click', '.btnStockReceiving', function () {
                location = MergeUrlParameters({
                    OrgID: sessionStorage.OrgID,
                    IsReceiving: 1,
                    IsMobile: CBool(IsMobile) ? 1 : 0
                }, '/Project/Receiving');
            })

            function ShowPersonNameInfo(strID) {
                var pData = {};
                pData.PeopleID = strID;

                if (OfflineMode) {

                    vData = PeopleMenu_offline().getPeopleInfoName();

                    if (vData.Rows.length == 0) {
                        GotoSigninPage();
                        return;
                    };

                    //Header Info
                    $("#lblName").text(vData.Rows[0].PeopleName || '');
                    sessionStorage["CurrentUserFullName"] = (vData.Rows[0].PeopleName || '');
                    sessionStorage.PeopleID = vData.Rows[0].ID;

                    return true;
                }
                else {
                    AjaxSend({
                        url: "People/getPeopleInfoName",
                        data: pData,
                        success: function (vData) {

                            if (vData.PeopleNameData.Rows.length > 0) {

                                //Header Info
                                $("#lblName").text('Hi ' + vData.PeopleNameData.Rows[0].FirstName + '!' || '');
                                sessionStorage["CurrentUserFullName"] = (vData.PeopleNameData.Rows[0].PeopleName || '');
                            }
                        }
                    });
                };
            }

            function GotoSigninPage() {

                var pParam = {};
                if (IsMobile)
                    pParam.IsMobile = 1;
                pParam = $.param(pParam);
                window.location = '/bomprojectexcel?' + pParam;

            }
        })
    })
})