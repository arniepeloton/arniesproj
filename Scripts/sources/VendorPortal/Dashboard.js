require(['promise!common'], function () {
    require(['/Content/Ecommerce/js/bootstrap.min.js', 
        'promise!/Content/bootstrap/js/bootstrap_modal.js?v=' + assetversion,
        'promise!/Scripts/jquery.filedownload.min.js',
        'bootstrap-tooltip']
    , function () {
        $(document).ready(function () {
            var logindetails;
            var htmlMcrDashboard = '';
            var mOrderFulfillmentWorkLoaded = false;

            $(function () {
                AjaxSend({
                    url: 'VendorPortal/LoginDetails',
                    success: function (e) {
                        logindetails = e.Results.Rows[0];
                        $('#lblUserName').text($.trim(logindetails.Name));
                        $('#menu-vendorname').prepend(sessionStorage.SupplierName);
                    }
                })
            })

            var tempinterval = setInterval(function () {
                if (typeof AddBreadCrumb != 'undefined') {
                    clearInterval(tempinterval)
                    $('.ItemsBreadCrumbs').empty();
                    AddBreadCrumb({
                        text: 'Dashboard', Click: function () {
                            $('#divVendorDashboard').pageshow();
                        }
                    }, '.ItemsBreadCrumbs')
                }
            }, 100)            

            $('body')
            .tooltip({ selector: '[data-toggle="tooltip"]', trigger: 'hover' }).on('click mousedown mouseup', '[data-toggle="tooltip"], [title]:not([data-toggle="popover"])', function () {
                $('[data-toggle="tooltip"], [title]:not([data-toggle="popover"])').tooltip('destroy');
            })
            .on('click', '#menu-myprofile', function () {
                require(['promise!/Scripts/sources/VendorPortal/MyProfile/main/controller.js?v=' + assetversion], function () {
                    AddBreadCrumb({
                        text: 'My Profile', Click: function () {
                            $('#divVendorProfile').pageshow();
                        }
                    }, '.ItemsBreadCrumbs')
                    $('#divVendorProfile').pageshow();
                })
            })
            .on('click', '#menu-logout', function () {
                location.href = '/VendorPortal/Logout?OrgID=' + sessionStorage.OrgID + '&url=' + location.origin
            })
            .on('click', '#menu-canvass', function () {
                require(['promise!/Scripts/sources/VendorPortal/Canvassing/main/controller.js?v=' + assetversion], function () {
                    AddBreadCrumb({
                        text: 'Canvass Forms', Click: function () {
                            $('#divVendorCanvass, #divVendorCanvassList').pageshow();
                        }
                    }, '.ItemsBreadCrumbs')
                    $('#divVendorCanvass, #divVendorCanvassList').pageshow();
                })
            })
            .on('click', '#menu-fulfullment-center', function () {
                if (mOrderFulfillmentWorkLoaded) {
                    HideModule();
                    $('#divOrderFulfillmentWork').show();
                } else
                    AjaxSend({
                        url: '/DashboardsAndReports/SalesOrder?OrgID=' + sessionStorage.OrgID,
                        type: 'GET',
                        dataType: 'text',
                        success: function (vData) {
                            $('#divOrderFulfillmentWork').empty();
                            $('#divOrderFulfillmentWork').append(vData).trigger('create');
                            mOrderFulfillmentWorkLoaded = true;
                            //$('#divOrderFulfillmentWork').show();
                            HideModule();
                            $('#divOrderFulfillmentWork').pageshow();
                        }
                    });
            })

            function HideModule() {
                $('#divVendorDashboard').hide();
                $('#divVendorProfile').hide();
                $('#divOrderFulfillmentWork').hide();
                $('#divVendorCanvass, #divVendorCanvassList').hide();
            }

            window.CallPage = function (container, htmlCarrier, url, exe) {
                //exe is optional
                if (exe != undefined) {
                    if (!$.isFunction(exe)) {
                        NewMessageBox('4th parameter must be a function', 'Message');
                        return;
                    }
                }

                if (CStr2(htmlCarrier) == '') {
                    if (CStr2(container) != '') {
                        $(container).html('Loading ...').pageshow();
                    } else {
                        NewMessageBox('Please wait ...', 'Message');
                    }

                    AjaxSend({
                        ExactUrl: url + '?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile + '&proj=eas&pid=' + sessionStorage.PeopleID,
                        type: 'GET',
                        dataType: 'text',
                        success: function (vData) {
                            htmlCarrier = $('<div>').append(vData);
                            if (CStr2(container) != '') {
                                $(container).html('').append(htmlCarrier).trigger('create');
                            } else {
                                NewMessageBoxClose();
                            }

                            if (exe != undefined) {
                                exe(vData);
                            }
                        }
                    });
                } else {
                    if (CStr2(container) != '') {
                        $(container).append(htmlCarrier);
                        $(container).pageshow(); //.html('').append(htmlCarrier).trigger('create');
                    }

                    if (exe != undefined) {
                        exe(htmlCarrier);
                    }
                }
            }
        })
    })
})