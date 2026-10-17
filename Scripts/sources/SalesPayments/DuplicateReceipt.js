require([
    'common',
    '/Scripts/sources/SalesPayments/SalesPayments.js',
    'business',
    'jquery.plugin.html2canvas',    
    'promise!/Scripts3/Browser/TransInvoicesForReprint.js?crc=',
    'promise!/Scripts3/Browser/LibraryFowst.js?crc='
    ], function () {

        require([(OfflineMode ? '/Scripts/sources/SalesPayments/SalesPayments_offline.js?crc=' : ''), (OfflineMode ? '/Scripts3/SalesMenu_offline.js?crc=' : '')], function () {

            $(function () {

                var intFAFCTotalLenght = 40;
                var intFBTotalLenght = 27;
                var intFCDoubleTotalLenght = 18;

                var intDesktopPrintLenght = 45;

                var IsPrintReceipt = false;
                var IsEmailReceipt = false;

                var intSalesPaymentEmailCntr = -1;
                var strSalesPaymentEmail = "";
                var isSeniorCitizenDiscount = false;

                window.pageSourcePage = $.mobile.activePage;

                $(document).ready(function () {

                    if (!window.InAndroid) {
                        $('<applet name="jzebra" code="jzebra.PrintApplet.class" archive="/Scripts/jzebra.jar" width="10" height="10"></applet>').appendTo($('body'));
                    }

                    if (sessionStorage.FowstID == undefined || sessionStorage.FowstID == "" || sessionStorage.FowstID == null) {
                        GetFowst();
                    }

                    if (CBool(Business.SystemDefaults.IsEnableDiningMode())) {
                        sessionStorage.IsEnableDiningMode = 'true';
                    } else {
                        sessionStorage.IsEnableDiningMode = 'false';
                    }
                });

//                ///////// ADDITIONAL BY JANINE

//                var pParams = GetUrlParameters();
//                /// <reference path="../../../Controllers/ReceiptSettingsController.vb" />

//                if (pParams.ref == 'pos') {

//                    $('#divTransInvoice').show();
//                    $("#txtDuplicateReceiptReceiptNo").attr("disabled", true);

//                } else {
//                    $('#divTransInvoice').hide();
//                    $("#txtDuplicateReceiptReceiptNo").attr("disabled", false);
//                }

                $('#btnTransInvoice').click(function (e) {
                    e.preventDefault();

                    Browser.TransInvoicesForReprint.InputData.FOWSTID = sessionStorage.FowstID;
                    Browser.TransInvoicesForReprint.Click = function (e) {
                        $("#txtDuplicateReceiptReceiptNo").data('InvoiceID', e.ID);
                        $("#txtDuplicateReceiptReceiptNo").data('CustomerID', e.CustomerID);
                        $("#txtDuplicateReceiptReceiptNo").val(e.InvoiceNum);
                    };
                    Browser.TransInvoicesForReprint.Show('');
                    return false;

                });
                ///////// END OF ADDITIONAL BY JANINE


                //Not yet available in Offline Mode
                if (IsMobile) {
                    //$("#btnDuplicateReceiptEmail").hide();
                    $("#btnDuplicateReceiptPrintEmail").hide();
                }

                $("#txtDuplicateReceiptReceiptNo").keydown(function (e) {
                    if (e.keyCode != 110 && e.keyCode != 190) {
                        if (!NumberOnly(e)) return;
                    }
                });

                $('#btnDuplicateReceiptPrint').click(function (e) {
                    e.preventDefault();

                    if ($.trim($("#txtDuplicateReceiptReceiptNo").val()) == "") {
                        MessageBox("Please select Sales Invoice Number.",
                           "Sales Invoice No.",
                            [
                                { Text: 'Ok'
                                }
                            ]
                        );
                        return;
                    }

                    window.pageSecuritySourcePage = $.mobile.activePage;
                    PromptUserAccess('POS Receipt Reprint', {
                        OnClickOKSuccess: function () {

                            $.mobile.changePage(window.pageSecuritySourcePage);

                            IsPrintReceipt = true;
                            IsEmailReceipt = false;

                            PrintSalesInvoice();
                        },
                        OnFailed: function() {
                            NewMessageBox('Do you want to request authorization to reprint?', '', [
                                {
                                    Text: 'Yes',
                                    Click: function() {
                                        var p = {
                                            InvoiceNum: $.trim($("#txtDuplicateReceiptReceiptNo").val()),
                                            FowstID: sessionStorage.FowstID,
                                            DeviceSerial: Web.DeviceSerial()
                                        };
                                        AjaxSend({
                                            url: "SalesMenu/RequestReprint",
                                            data:p,
                                            success: function(vData) {
                                                if (vData.Error) {
                                                    NewMessageBox(vData.Error);
                                                } else if (vData.Success) 
                                                    NewMessageBox('Your request to reprint invoice was successfully saved.');
                                            }
                                        });
                                    }
                                },
                                {
                                    Text: 'No'
                                }
                            ]);
                        }
                    });

                });

                $('#btnDuplicateDeliveryPrint').click(function (e) {
                    e.preventDefault();

                    if ($.trim($("#txtDuplicateReceiptReceiptNo").val()) == "") {
                        MessageBox("Please select Sales Invoice Number.",
                           "Sales Invoice No.",
                            [
                                {
                                    Text: 'Ok'
                                }
                            ]
                        );
                        return;
                    }

                    window.pageSecuritySourcePage = $.mobile.activePage;
                    PromptUserAccess('POS Receipt Reprint', {
                        OnClickOKSuccess: function () {

                            $.mobile.changePage(window.pageSecuritySourcePage);

                            IsPrintReceipt = true;
                            IsEmailReceipt = false;

                            PrintDeliveryReceipt();
                        },
                        OnFailed: function () {
                            NewMessageBox('Do you want to request authorization to reprint?', '', [
                                {
                                    Text: 'Yes',
                                    Click: function () {
                                        var p = {
                                            InvoiceNum: $.trim($("#txtDuplicateReceiptReceiptNo").val()),
                                            FowstID: sessionStorage.FowstID,
                                            DeviceSerial: Web.DeviceSerial()
                                        };
                                        AjaxSend({
                                            url: "SalesMenu/RequestReprint",
                                            data: p,
                                            success: function (vData) {
                                                if (vData.Error) {
                                                    NewMessageBox(vData.Error);
                                                } else if (vData.Success)
                                                    NewMessageBox('Your request to reprint invoice was successfully saved.');
                                            }
                                        });
                                    }
                                },
                                {
                                    Text: 'No'
                                }
                            ]);
                        }
                    });

                });

                $('#btnDuplicateReceiptEmail').click(function (e) {
                    e.preventDefault();

                    if ($.trim($("#txtDuplicateReceiptReceiptNo").val()) == "") {
                        MessageBox("Please enter Sales Invoice Number.",
                           "Sales Invoice No.",
                            [
                                { Text: 'Ok'
                                }
                            ]
                    );
                        return;
                    }


                    window.pageSecuritySourcePage = $.mobile.activePage;
                    PromptUserAccess('POS Receipt Reprint', {
                        OnClickOKSuccess: function () {

                            $.mobile.changePage(window.pageSecuritySourcePage);

                            IsPrintReceipt = false;
                            IsEmailReceipt = true;

                            PrintSalesInvoice();
                        }
                    });

                });

                $('#btnDuplicateReceiptPrintEmail').click(function (e) {
                    e.preventDefault();

                    if ($.trim($("#txtDuplicateReceiptReceiptNo").val()) == "") {
                        MessageBox("Please enter Sales Invoice Number.",
                           "Sales Invoice No.",
                            [
                                { Text: 'Ok'
                                }
                            ]
                    );
                        return;
                    }


                    window.pageSecuritySourcePage = $.mobile.activePage;
                    PromptUserAccess('POS Receipt Reprint', {
                        OnClickOKSuccess: function () {

                            $.mobile.changePage(window.pageSecuritySourcePage);

                            IsPrintReceipt = true;
                            IsEmailReceipt = true;

                            PrintSalesInvoice();
                        }
                    });

                });

                $('#btnDuplicateReceiptEmailOK').click(function (e) {
                    e.preventDefault();

                    strSalesPaymentEmail = "";
                    for (var i = 0; i <= intSalesPaymentEmailCntr; i++) {

                        if ($("#chkEmail-" + i).is(":checked")) {
                            var pEmailData = $("#chkEmail-" + i).data('Data');

                            strSalesPaymentEmail = strSalesPaymentEmail + (strSalesPaymentEmail.length > 0 ? ", " : "") + pEmailData.EmailAddress
                        }

                    }

                    if ($.trim($("#txtDuplicateReceiptEmailAddress").val()) == "" && strSalesPaymentEmail == "") {
                        MessageBox("Please enter email address.",
                           "Email Address",
                            [
                                { Text: 'Ok'
                                }
                            ]
                    );
                        return;
                    } else {
                        if ($.trim($("#txtDuplicateReceiptEmailAddress").val()).length > 0) {
                            strSalesPaymentEmail = strSalesPaymentEmail + (strSalesPaymentEmail.length > 0 ? ", " : "") + $.trim($("#txtDuplicateReceiptEmailAddress").val())
                        }
                    }

                    doSendEmailReceipt();
                    //PrintSalesInvoice();

                });

                $('#btnDuplicateReceiptEmailCancel').click(function (e) {
                    e.preventDefault();

                    $.mobile.changePage('#pageSalesPaymentDuplicateReceipt');

                });

                $('#btnDuplicateReceiptDone').click(function (e) {

                    e.preventDefault();
                    history.back();
                    //                    window.open(GetPathFormat("/People/PeopleMenu"), "_self");
                });


                function GetFowst() {
                    var pData = { FilterFowst: 1 };
                    AjaxSend({
                        url: 'Browser/LibraryFowst',
                        data: pData,
                        success: function (vFowstData) {

                            if (vFowstData.Results.Rows.length == 1) {
                                sessionStorage.FowstID = vFowstData.Results.Rows[0].ID;
                            } else {
                                BrowseAddresses();
                            }
                        }
                    });
                };

                function BrowseAddresses() {

                    Browser.LibraryFowst.InputData.OrganizationID = sessionStorage.OrgID;
                    Browser.LibraryFowst.InputData.FilterFowst = 1;
                    Browser.LibraryFowst.Click = function (e) {
                        sessionStorage.FowstID = e.ID;
                    };
                    Browser.LibraryFowst.Show();

                };

                function LoadCustomerEmailList(vRows) {
                    var pLV = $('#fldsetDuplicateEmailEmail');
                    var pItem;

                    intSalesPaymentEmailCntr = vRows.length - 1;

                    pLV.html('');
                    for (var i = 0; i <= vRows.length - 1; i++) {

                        pItem = $('<input>')
                                .attr("type", 'checkbox')
                                .attr('id', 'chkEmail-' + i)
                                .attr('checked', false)
                                .data('Data', vRows[i])
                            .append($('<label>')
                                .attr('for', 'chkEmail-' + i)
                                .text((vRows[i].EmailAddress || 'No Email Address'))
                            );

                        pLV.append(pItem);
                    }

                    $('#divDuplicateReceiptEmail').trigger("create");

                    if (vRows.length <= 0) {
                        $("#divDuplicateReceiptEmailNone").show();
                    } else {
                        $("#divDuplicateReceiptEmailNone").hide();
                    }

                    $.mobile.changePage("#divDuplicateReceiptPromptEmail");
                    return;

                };

                function PrintSalesInvoice() {
                    var vInvoiceData;

                    if (IsEmailReceipt) {
                        if (strSalesPaymentEmail == "") {
                            vInvoiceData = SalesPayment_offline().getInvoiceDetailsByFOWSTDevice($.trim($("#txtDuplicateReceiptReceiptNo").val()));
                            
                            pDataEmail = {
                                CustomerID: vInvoiceData.Rows[0].CustomerID
                            };

                            AjaxSend({
                                url: "SalesPayments/getCustomerEmails",
                                data: pDataEmail,
                                success: function (vDataEmail) {

                                    LoadCustomerEmailList(vDataEmail.SalesPaymentData.Rows);
                                }
                            });

                            return;
                        }
                    } else {
                        if (OfflineMode) {

                            if ($("#txtDuplicateReceiptReceiptNo").data('InvoiceID') != undefined) {
                                PrintPOSReceipt($("#txtDuplicateReceiptReceiptNo").data('InvoiceID'), false);
                            } else {
                                vInvoiceData = SalesPayment_offline().getInvoiceDetailsByFOWSTDevice($.trim($("#txtDuplicateReceiptReceiptNo").val()));
                                if (vInvoiceData.Rows.length > 0) {

                                    //                            //Ask email
                                    //                            if (IsEmailReceipt) {
                                    //                                if (strSalesPaymentEmail == "") {
                                    //                                    LoadCustomerEmailList(SalesPayment_offline().getCustomerEmails(vInvoiceData.Rows[0].CustomerID).Rows);
                                    //                                    return;
                                    //                                }
                                    //                            }

                                    //                            //Check for valid email addresses
                                    //                            if (strSalesPaymentEmail != "") {
                                    //                                if (!VerifyEmail(strSalesPaymentEmail)) {
                                    //                                    MessageBox("Please enter valid email address.",
                                    //                                            "Invalid email",
                                    //                                            [
                                    //                                                { Text: 'Ok',
                                    //                                                    Click: function () {
                                    //                                                        $.mobile.changePage(window.pageSourcePage);
                                    //                                                        return;
                                    //                                                    }
                                    //                                                }
                                    //                                            ]
                                    //                                    );
                                    //                                    return;
                                    //                                }
                                    //                            }

                                    //                            if (IsEmailReceipt) {
                                    //                                doSendEmailReceipt(pData);
                                    //                            } else if (IsPrintReceipt) {

                                    if (CStr2(vInvoiceData.Rows[0].DiscountTypeID).toUpperCase() == '055A0A33-DD98-47EC-ADD3-8B2009F3944A' ||
                                        CStr2(vInvoiceData.Rows[0].DiscountTypeID).toUpperCase() == '674AD791-188B-4238-B95C-97B5C2B9D544') {
                                        isSeniorCitizenDiscount = true;
                                    }


                                    PrintPOSReceipt(vInvoiceData.Rows[0].ID, false);
                                    //                            }; 
                                } else {

                                    MessageBox("Unable to find Sales Invoice : " + $.trim($("#txtDuplicateReceiptReceiptNo").val()) + " in your device. Do you want to check it online?",
                                            "Reprint Invoice",
                                            [
                                                {
                                                    Text: 'Yes',
                                                    Click: function () {
                                                        PrintSaleInvoiceOnline();
                                                        return;
                                                    }
                                                }
                                            ,
                                                {
                                                    Text: 'No'
                                                }
                                            ]
                                    );
                                }
                            }

                        } else {
                            PrintSaleInvoiceOnline();
                        }
                    }
                };

                function PrintDeliveryReceipt() {
                    var vInvoiceData;

                    if (IsEmailReceipt) {
                        if (strSalesPaymentEmail == "") {
                            vInvoiceData = SalesPayment_offline().getInvoiceDetailsByFOWSTDevice($.trim($("#txtDuplicateReceiptReceiptNo").val()));

                            pDataEmail = {
                                CustomerID: vInvoiceData.Rows[0].CustomerID
                            };

                            AjaxSend({
                                url: "SalesPayments/getCustomerEmails",
                                data: pDataEmail,
                                success: function (vDataEmail) {

                                    LoadCustomerEmailList(vDataEmail.SalesPaymentData.Rows);
                                }
                            });

                            return;
                        }
                    } else {
                        if (OfflineMode) {

                            if ($("#txtDuplicateReceiptReceiptNo").data('InvoiceID') != undefined) {
                                PrintPOSDeliveryReceipt($("#txtDuplicateReceiptReceiptNo").data('InvoiceID'), false);
                            } else {
                                vInvoiceData = SalesPayment_offline().getInvoiceDetailsByFOWSTDevice($.trim($("#txtDuplicateReceiptReceiptNo").val()));
                                if (vInvoiceData.Rows.length > 0) {
                                    
                                    if (CStr2(vInvoiceData.Rows[0].DiscountTypeID).toUpperCase() == '055A0A33-DD98-47EC-ADD3-8B2009F3944A' ||
                                        CStr2(vInvoiceData.Rows[0].DiscountTypeID).toUpperCase() == '674AD791-188B-4238-B95C-97B5C2B9D544') {
                                        isSeniorCitizenDiscount = true;
                                    }


                                    PrintPOSDeliveryReceipt(vInvoiceData.Rows[0].ID, false);
                                    //                            }; 
                                } else {

                                    MessageBox("Unable to find Sales Invoice : " + $.trim($("#txtDuplicateReceiptReceiptNo").val()) + " in your device. Do you want to check it online?",
                                            "Reprint Invoice",
                                            [
                                                {
                                                    Text: 'Yes',
                                                    Click: function () {
                                                        PrintPOSDeliveryReceiptOnline();
                                                        return;
                                                    }
                                                }
                                            ,
                                                {
                                                    Text: 'No'
                                                }
                                            ]
                                    );
                                }
                            }

                        } else {
                            PrintPOSDeliveryReceiptOnline();
                        }
                    }
                };
                function PrintPOSDeliveryReceiptOnline() {
                    var vInvoiceData;
                    var pDataInv = [];
                    var pData = [];
                    var pDataEmail = [];
                    var pInvoiceID = '';

                    if ($("#txtDuplicateReceiptReceiptNo").data('InvoiceID') != undefined && $("#txtDuplicateReceiptReceiptNo").data('CustomerID') != undefined) {
                        
                        PrintPOSDeliveryReceipt($("#txtDuplicateReceiptReceiptNo").data('InvoiceID'), true);

                    } else {
                        pDataInv = {
                            InvoiceNo: $.trim($("#txtDuplicateReceiptReceiptNo").val()),
                            FowstID: sessionStorage.FowstID,
                            DeviceSerial: Web.DeviceSerial()
                        };
                        AjaxSend({
                            url: "SalesPayments/getInvoiceDetails",
                            data: pDataInv,
                            success: function (vData) {

                                if (vData.SalesPaymentData.Rows.length > 0) {

                                    PrintPOSDeliveryReceipt(vData.SalesPaymentData.Rows[0].ID, true);

                                } else {
                                    MessageBox("Unable to find Sales Invoice : " + $.trim($("#txtDuplicateReceiptReceiptNo").val()),
                                            "Reprint Invoice",
                                            [
                                                {
                                                    Text: 'Ok'
                                                }
                                            ]
                                    );
                                }
                            }

                        });
                    }
                }

                function PrintSaleInvoiceOnline() {
                    var vInvoiceData;
                    var pDataInv = [];
                    var pData = [];
                    var pDataEmail = [];
                    var pInvoiceID = '';

                    if ($("#txtDuplicateReceiptReceiptNo").data('InvoiceID') != undefined && $("#txtDuplicateReceiptReceiptNo").data('CustomerID') != undefined) {

                        PrintSaleInvoiceOnlineNow($("#txtDuplicateReceiptReceiptNo").data('InvoiceID'), $("#txtDuplicateReceiptReceiptNo").data('CustomerID'));

                    } else {
                        pDataInv = {
                            InvoiceNo: $.trim($("#txtDuplicateReceiptReceiptNo").val()),
                            FowstID: sessionStorage.FowstID,
                            DeviceSerial: Web.DeviceSerial()
                        };
                        AjaxSend({
                            url: "SalesPayments/getInvoiceDetails",
                            data: pDataInv,
                            success: function (vData) {

                                if (vData.SalesPaymentData.Rows.length > 0) {

                                    PrintSaleInvoiceOnlineNow(vData.SalesPaymentData.Rows[0].ID, vData.SalesPaymentData.Rows[0].CustomerID);

                                } else {
                                    MessageBox("Unable to find Sales Invoice : " + $.trim($("#txtDuplicateReceiptReceiptNo").val()),
                                            "Reprint Invoice",
                                            [
                                                { Text: 'Ok'
                                                }
                                            ]
                                    );
                                }
                            }

                        });
                    }
                }


                function PrintSaleInvoiceOnlineNow(pInvoiceID, pCustomerID) {
                    var vInvoiceData;
                    var pDataInv = [];
                    var pData = [];
                    var pDataEmail = [];
                    var pInvoiceID = '';

                    if ($("#txtDuplicateReceiptReceiptNo").data('InvoiceID') != undefined) {

                        //Ask email
                        if (IsEmailReceipt) {
                            if (strSalesPaymentEmail == "") {

                                pDataEmail = {
                                    CustomerID: pCustomerID
                                };

                                AjaxSend({
                                    url: "SalesPayments/getCustomerEmails",
                                    data: pDataEmail,
                                    success: function (vDataEmail) {

                                        LoadCustomerEmailList(vDataEmail.SalesPaymentData.Rows);
                                    }
                                });

                                return;
                            }
                        }

                        pInvoiceID = $("#txtDuplicateReceiptReceiptNo").data('InvoiceID');
                        pData = {
                            EmailAddress: (IsEmailReceipt ? $.trim(strSalesPaymentEmail) : ""),
                            InvoiceID: pInvoiceID
                        };

                        //Send Email
                        if (IsEmailReceipt) {

                            AjaxSend({
                                url: "SalesPayments/SendEmailReceipt",
                                data: pData,
                                success: function (vDataEmail) {
                                    if (vDataEmail.SalesPaymentData == "InvalidEmail") {
                                        MessageBox("Please enter valid email address.",
                                                       "Invalid email",
                                                        [
                                                            { Text: 'Ok',
                                                                Click: function () {
                                                                    return;
                                                                }
                                                            }
                                                        ]
                                                );
                                    } else {
                                        if (!IsPrintReceipt) {
                                            MessageBox("Receipt has been sent.",
                                                            "Email Receipt",
                                                            [
                                                                { Text: 'Ok',
                                                                    Click: function () {
                                                                        window.location = GetPathFormat("/SalesMenu?mode=pos&OrgID=" + sessionStorage.OrgID);
                                                                    }
                                                                }
                                                            ]
                                                    );
                                        }
                                    }
                                }
                            });
                        }

                        if (IsPrintReceipt) {
                            PrintPOSReceipt(pInvoiceID, true);
                        }

                    } else {
                        pDataInv = {
                            InvoiceNo: $.trim($("#txtDuplicateReceiptReceiptNo").val()),
                            FowstID: sessionStorage.FowstID,
                            DeviceSerial: Web.DeviceSerial()
                        };
                        AjaxSend({
                            url: "SalesPayments/getInvoiceDetails",
                            data: pDataInv,
                            success: function (vData) {

                                if (vData.SalesPaymentData.Rows.length > 0) {

                                    //Ask email
                                    if (IsEmailReceipt) {
                                        if (strSalesPaymentEmail == "") {

                                            pDataEmail = {
                                                CustomerID: pCustomerID
                                            };

                                            AjaxSend({
                                                url: "SalesPayments/getCustomerEmails",
                                                data: pDataEmail,
                                                success: function (vDataEmail) {

                                                    LoadCustomerEmailList(vDataEmail.SalesPaymentData.Rows);
                                                }
                                            });

                                            return;
                                        }
                                    }

                                    pInvoiceID = vData.SalesPaymentData.Rows[0].ID;
                                    pData = {
                                        EmailAddress: (IsEmailReceipt ? $.trim(strSalesPaymentEmail) : ""),
                                        InvoiceID: pInvoiceID
                                    };

                                    //Send Email
                                    if (IsEmailReceipt) {

                                        AjaxSend({
                                            url: "SalesPayments/SendEmailReceipt",
                                            data: pData,
                                            success: function (vDataEmail) {
                                                if (vDataEmail.SalesPaymentData == "InvalidEmail") {
                                                    MessageBox("Please enter valid email address.",
                                                       "Invalid email",
                                                        [
                                                            { Text: 'Ok',
                                                                Click: function () {
                                                                    return;
                                                                }
                                                            }
                                                        ]
                                                );
                                                } else {
                                                    if (!IsPrintReceipt) {
                                                        MessageBox("Receipt has been sent.",
                                                            "Email Receipt",
                                                            [
                                                                { Text: 'Ok',
                                                                    Click: function () {
                                                                        window.location = GetPathFormat("/SalesMenu?mode=pos&OrgID=" + sessionStorage.OrgID);
                                                                    }
                                                                }
                                                            ]
                                                    );
                                                    }
                                                }
                                            }
                                        });
                                    }

                                    if (IsPrintReceipt) {
                                        PrintPOSReceipt(pInvoiceID, true);
                                    }

                                } else {
                                    MessageBox("Unable to find Sales Invoice : " + $.trim($("#txtDuplicateReceiptReceiptNo").val()),
                                            "Reprint Invoice",
                                            [
                                                { Text: 'Ok'
                                                }
                                            ]
                                    );
                                }
                            }

                        });
                    }
                }

                function VerifyEmail(vSalesPaymentEmail) {
                    var strEmail;

                    if ($.trim(vSalesPaymentEmail) == "") {
                        return false;
                    } else {
                        strEmail = vSalesPaymentEmail.split(",");

                        for (var i = 0; i <= strEmail.length - 1; i++) {
                            if (!IsEmail(strEmail[i])) {
                                return false;
                            }
                        }
                    }

                    return true;
                };

                function PrintPOSDeliveryReceipt(pInvoiceID, IsOnline) {
                    var vDataPrint;
                    var vDataHeader;
                    var vDataInvHeader;
                    var vDataItem;
                    var vDataItemSerial;
                    var vDataPayment;
                    var vDataFooter;
                    var vDataMsg;
                    var pData = {};
                    pData.InvoiceID = pInvoiceID;

                    //Get Printer IP Address
                    if (OfflineMode && !IsOnline) {
                        vDataPrint = SalesPayment_offline().getOrgFOWSTPrinterIP();
                        //Get Header
                        vDataHeader = SalesPayment_offline().getReceiptHeaderInfo(pInvoiceID);
                        //Get Invoice Header
                        vDataInvHeader = SalesPayment_offline().getSalesPaymentInvoiceHeaderInfo(pInvoiceID);
                        //Get Invoice Items
                        vDataItem = SalesPayment_offline().getSalesPaymentReceiptBodyInfo(pInvoiceID);
                        //Get Payment Info
                        vDataPayment = SalesPayment_offline().getSalesPaymentReceiptPaymentInfo(pInvoiceID);
                        //Get Invoice Items Serial
                        vDataItemSerial = SalesPayment_offline().getSalesPaymentReceiptSerial(pInvoiceID);
                        //Get Footer Amounts
                        vDataFooter = SalesPayment_offline().getSalesPaymentReceiptFooterAmountsInfo(pInvoiceID);
                        //Get Footer Messages
                        vDataMsg = SalesPayment_offline().getSalesPaymentReceiptFooterMessagesInfo();

                        //PrintPOSReceiptViaWindowsPrintServer(pInvoiceID, vDataPrint, vDataHeader, vDataInvHeader, vDataItem, vDataItemSerial, vDataPayment, vDataFooter, vDataMsg)

                        if (window.InAndroid) { //Android Mobile, query to offline
                            var vDataPrinterType = SalesPayment_offline().getOrgFOWSTPrinterType();

                            if (vDataPrinterType.Rows.length > 0) {
                                if (vDataPrinterType.Rows[0].PrinterType == 'Epson TM-U220B Printer') {
                                    PrintDeliveryReceiptThermalPrinter(pInvoiceID, vDataPrint, vDataHeader, vDataInvHeader, vDataItem, vDataItemSerial, vDataPayment, vDataFooter, vDataMsg)
                                } else if (vDataPrinterType.Rows[0].PrinterType == '80MM Mini Thermal Printer') {

                                } else if (vDataPrinterType.Rows[0].PrinterType == 'Epson G Printer') {
                                    sessionStorage.Printer = 'GPrinter';
                                    PrintDeliveryReceiptThermalPrinter(pInvoiceID, vDataPrint, vDataHeader, vDataInvHeader, vDataItem, vDataItemSerial, vDataPayment, vDataFooter, vDataMsg)
                                } else if (vDataPrinterType.Rows[0].PrinterType == 'Windows Print Server') {
                                    PrintDeliveryReceiptViaWindowsPrintServer(pInvoiceID, vDataPrint, vDataHeader, vDataInvHeader, vDataItem, vDataItemSerial, vDataPayment, vDataFooter, vDataMsg)
                                } else {
                                    PrintDeliveryReceiptThermalPrinter(pInvoiceID, vDataPrint, vDataHeader, vDataInvHeader, vDataItem, vDataItemSerial, vDataPayment, vDataFooter, vDataMsg)
                                }
                            } else {
                                PrintDeliveryReceiptThermalPrinter(pInvoiceID, vDataPrint, vDataHeader, vDataInvHeader, vDataItem, vDataItemSerial, vDataPayment, vDataFooter, vDataMsg)
                            }
                            //PrintPOSReceiptNow(pInvoiceID, vDataPrint, vDataHeader, vDataInvHeader, vDataItem, vDataItemSerial, vDataPayment, vDataFooter, vDataMsg);
                        } else { //Mobile (window), query to offline
                            var japplet = document.jzebra;
                            var blnIsPrinterFound = false;

                            if (japplet != null) {
                                while (!blnIsPrinterFound) {

                                    if (FindDesktopPrinter("")) {
                                        blnIsPrinterFound = true;

                                        PrintPOSDesktopReceiptNow(pInvoiceID, vDataHeader, vDataInvHeader, vDataItem, vDataItemSerial, vDataPayment, vDataFooter, vDataMsg);

                                    };

                                }
                            } else {
                                MessageBox("Jzebra applet was not loaded.",
                                    "Jzebra",
                                    [
                                        {
                                            Text: 'Ok'
                                        }
                                    ]
                                );
                            }
                        }

                    } else {
                        AjaxSend({
                            url: "Organization/getOrgFOWSTPrinterIP",
                            success: function (vRetDataPrint) {
                                vDataPrint = vRetDataPrint.OrganizationFOWSTData;

                                //Get Header
                                AjaxSend({
                                    url: "SalesPayments/getReceiptHeaderInfo",
                                    data: pData,
                                    success: function (vRetDataHeader) {
                                        vDataHeader = vRetDataHeader.SalesPaymentData;

                                        //Get Invoice Header
                                        AjaxSend({
                                            url: "SalesPayments/getSalesPaymentInvoiceHeaderInfo",
                                            data: pData,
                                            success: function (vRetDataInvHeader) {
                                                vDataInvHeader = vRetDataInvHeader.SalesPaymentData;

                                                //Get Invoice Items
                                                AjaxSend({
                                                    url: "SalesPayments/getSalesPaymentReceiptBodyInfo",
                                                    data: pData,
                                                    success: function (vRetDataItem) {
                                                        vDataItem = vRetDataItem.SalesPaymentData;

                                                        //Get Invoice Items Serial
                                                        AjaxSend({
                                                            url: "SalesPayments/getSalesPaymentReceiptSerial",
                                                            data: pData,
                                                            success: function (vRetDataItemSerial) {
                                                                vDataItemSerial = vRetDataItemSerial.SalesPaymentData;

                                                                //Get Payment Info
                                                                AjaxSend({
                                                                    url: "SalesPayments/getSalesPaymentReceiptPaymentInfo",
                                                                    data: pData,
                                                                    success: function (vRetDataPayment) {
                                                                        vDataPayment = vRetDataPayment.SalesPaymentData;

                                                                        //Get Footer Amounts
                                                                        AjaxSend({
                                                                            url: "SalesPayments/getSalesPaymentReceiptFooterAmountsInfo",
                                                                            data: pData,
                                                                            success: function (vRetDataFooter) {
                                                                                vDataFooter = vRetDataFooter.SalesPaymentData;

                                                                                //Get Footer Messages
                                                                                AjaxSend({
                                                                                    url: "SalesPayments/getSalesPaymentReceiptFooterMessagesInfo",
                                                                                    data: pData,
                                                                                    success: function (vRetDataMsg) {
                                                                                        vDataMsg = vRetDataMsg.SalesPaymentData;

                                                                                        if (window.InAndroid) { //Android Mobile Or Mobile (window)
                                                                                            var vDataPrinterType = SalesPayment_offline().getOrgFOWSTPrinterType();

                                                                                            if (vDataPrinterType.Rows.length > 0) {
                                                                                                if (vDataPrinterType.Rows[0].PrinterType == 'Epson TM-U220B Printer') {
                                                                                                    PrintDeliveryReceiptThermalPrinter(pInvoiceID, vDataPrint, vDataHeader, vDataInvHeader, vDataItem, vDataItemSerial, vDataPayment, vDataFooter, vDataMsg);
                                                                                                } else if (vDataPrinterType.Rows[0].PrinterType == '80MM Mini Thermal Printer') {

                                                                                                } else if (vDataPrinterType.Rows[0].PrinterType == 'Epson G Printer') {
                                                                                                    sessionStorage.Printer = 'GPrinter';
                                                                                                    PrintDeliveryReceiptThermalPrinter(pInvoiceID, vDataPrint, vDataHeader, vDataInvHeader, vDataItem, vDataItemSerial, vDataPayment, vDataFooter, vDataMsg);
                                                                                                } else if (vDataPrinterType.Rows[0].PrinterType == 'Windows Print Server') {
                                                                                                    PrintDeliveryReceiptViaWindowsPrintServer(pInvoiceID, vDataPrint, vDataHeader, vDataInvHeader, vDataItem, vDataItemSerial, vDataPayment, vDataFooter, vDataMsg)
                                                                                                } else {
                                                                                                    PrintDeliveryReceiptThermalPrinter(pInvoiceID, vDataPrint, vDataHeader, vDataInvHeader, vDataItem, vDataItemSerial, vDataPayment, vDataFooter, vDataMsg);
                                                                                                }
                                                                                            } else {
                                                                                                PrintDeliveryReceiptThermalPrinter(pInvoiceID, vDataPrint, vDataHeader, vDataInvHeader, vDataItem, vDataItemSerial, vDataPayment, vDataFooter, vDataMsg);
                                                                                            }
                                                                                            //PrintPOSReceiptNow(pInvoiceID, vDataPrint, vDataHeader, vDataInvHeader, vDataItem, vRetDataItemSerial, vDataPayment, vDataFooter, vDataMsg);
                                                                                        } else { //Web App
                                                                                            var japplet = document.jzebra;
                                                                                            var blnIsPrinterFound = false;

                                                                                            if (japplet != null) {
                                                                                                while (!blnIsPrinterFound) {

                                                                                                    if (FindDesktopPrinter("")) {
                                                                                                        blnIsPrinterFound = true;

                                                                                                        PrintPOSDesktopReceiptNow(pInvoiceID, vDataHeader, vDataInvHeader, vDataItem, vDataItemSerial, vDataPayment, vDataFooter, vDataMsg);

                                                                                                    };

                                                                                                }
                                                                                            } else {
                                                                                                MessageBox("Jzebra applet was not loaded.",
                                                                                                    "Jzebra",
                                                                                                    [
                                                                                                        {
                                                                                                            Text: 'Ok'
                                                                                                        }
                                                                                                    ]
                                                                                                );
                                                                                            }
                                                                                        }


                                                                                    }
                                                                                });

                                                                            }
                                                                        });
                                                                    }
                                                                });

                                                            }
                                                        });
                                                    }
                                                });
                                            }
                                        });

                                    }
                                });

                            }
                        });
                    }

                    var pIU;

                    pIU = new InsertUpdate('private.TransActionLogs');

                    pIU.Add('ID', ReadyString(NewID()));
                    pIU.Add('ActionDate', proj.SqliteNow);
                    pIU.Add('EmployeeName', ReadyString(sessionStorage.CurrentUserFullName));
                    pIU.Add('FOWSTID', ReadyString(sessionStorage.FowstID));
                    pIU.Add('DeviceID', ReadyString(Device.Serial()));
                    pIU.Add('Action', ReadyString("Reprint Delivery Receipt"));
                    pIU.Add('OriginalValue', ReadyString($("#txtDuplicateReceiptReceiptNo").val()));

                    pSql = pIU.GetInsert();

                    Sql.ExecuteSql(pSql);
                }

                function PrintPOSReceipt(pInvoiceID, IsOnline) {
                    var vDataPrint;
                    var vDataHeader;
                    var vDataInvHeader;
                    var vDataItem;
                    var vDataItemSerial;
                    var vDataPayment;
                    var vDataFooter;
                    var vDataMsg;
                    var pData = {};
                    pData.InvoiceID = pInvoiceID;

                    //Get Printer IP Address
                    if (OfflineMode && !IsOnline) {
                        vDataPrint = SalesPayment_offline().getOrgFOWSTPrinterIP();
                        //Get Header
                        vDataHeader = SalesPayment_offline().getReceiptHeaderInfo(pInvoiceID);
                        //Get Invoice Header
                        vDataInvHeader = SalesPayment_offline().getSalesPaymentInvoiceHeaderInfo(pInvoiceID);
                        //Get Invoice Items
                        vDataItem = SalesPayment_offline().getSalesPaymentReceiptBodyInfo(pInvoiceID);
                        //Get Payment Info
                        vDataPayment = SalesPayment_offline().getSalesPaymentReceiptPaymentInfo(pInvoiceID);
                        //Get Invoice Items Serial
                        vDataItemSerial = SalesPayment_offline().getSalesPaymentReceiptSerial(pInvoiceID);
                        //Get Footer Amounts
                        vDataFooter = SalesPayment_offline().getSalesPaymentReceiptFooterAmountsInfo(pInvoiceID);
                        //Get Footer Messages
                        vDataMsg = SalesPayment_offline().getSalesPaymentReceiptFooterMessagesInfo();

                        //PrintPOSReceiptViaWindowsPrintServer(pInvoiceID, vDataPrint, vDataHeader, vDataInvHeader, vDataItem, vDataItemSerial, vDataPayment, vDataFooter, vDataMsg)

                        if (window.InAndroid) { //Android Mobile, query to offline
                            var vDataPrinterType = SalesPayment_offline().getOrgFOWSTPrinterType();

                            if (vDataPrinterType.Rows.length > 0) {
                                if (vDataPrinterType.Rows[0].PrinterType == 'Epson TM-U220B Printer') {
                                    PrintPOSReceiptDotMatrix(pInvoiceID, vDataPrint, vDataHeader, vDataInvHeader, vDataItem, vDataItemSerial, vDataPayment, vDataFooter, vDataMsg);
                                } else if (vDataPrinterType.Rows[0].PrinterType == '80MM Mini Thermal Printer') {
                                } else if (vDataPrinterType.Rows[0].PrinterType == 'Epson G Printer') {
                                    sessionStorage.Printer = 'GPrinter';
                                    PrintPOSReceiptNow(pInvoiceID, vDataPrint, vDataHeader, vDataInvHeader, vDataItem, vDataItemSerial, vDataPayment, vDataFooter, vDataMsg);
                                } else if (vDataPrinterType.Rows[0].PrinterType == 'Windows Print Server') {
                                    PrintPOSReceiptViaWindowsPrintServer(pInvoiceID, vDataPrint, vDataHeader, vDataInvHeader, vDataItem, vDataItemSerial, vDataPayment, vDataFooter, vDataMsg)
                                } else {
                                    PrintPOSReceiptNow(pInvoiceID, vDataPrint, vDataHeader, vDataInvHeader, vDataItem, vDataItemSerial, vDataPayment, vDataFooter, vDataMsg);
                                }
                            } else {
                                PrintPOSReceiptNow(pInvoiceID, vDataPrint, vDataHeader, vDataInvHeader, vDataItem, vDataItemSerial, vDataPayment, vDataFooter, vDataMsg);
                            }
                            //PrintPOSReceiptNow(pInvoiceID, vDataPrint, vDataHeader, vDataInvHeader, vDataItem, vDataItemSerial, vDataPayment, vDataFooter, vDataMsg);
                        } else { //Mobile (window), query to offline
                            var japplet = document.jzebra;
                            var blnIsPrinterFound = false;

                            if (japplet != null) {
                                while (!blnIsPrinterFound) {

                                    if (FindDesktopPrinter("")) {
                                        blnIsPrinterFound = true;

                                        PrintPOSDesktopReceiptNow(pInvoiceID, vDataHeader, vDataInvHeader, vDataItem, vDataItemSerial, vDataPayment, vDataFooter, vDataMsg);

                                    };

                                }
                            } else {
                                MessageBox("Jzebra applet was not loaded.",
                                    "Jzebra",
                                    [
                                        { Text: 'Ok'
                                        }
                                    ]
                                );
                            }
                        }

                    } else {
                        AjaxSend({
                            url: "Organization/getOrgFOWSTPrinterIP",
                            success: function (vRetDataPrint) {
                                vDataPrint = vRetDataPrint.OrganizationFOWSTData;

                                //Get Header
                                AjaxSend({
                                    url: "SalesPayments/getReceiptHeaderInfo",
                                    data: pData,
                                    success: function (vRetDataHeader) {
                                        vDataHeader = vRetDataHeader.SalesPaymentData;

                                        //Get Invoice Header
                                        AjaxSend({
                                            url: "SalesPayments/getSalesPaymentInvoiceHeaderInfo",
                                            data: pData,
                                            success: function (vRetDataInvHeader) {
                                                vDataInvHeader = vRetDataInvHeader.SalesPaymentData;

                                                //Get Invoice Items
                                                AjaxSend({
                                                    url: "SalesPayments/getSalesPaymentReceiptBodyInfo",
                                                    data: pData,
                                                    success: function (vRetDataItem) {
                                                        vDataItem = vRetDataItem.SalesPaymentData;

                                                        //Get Invoice Items Serial
                                                        AjaxSend({
                                                            url: "SalesPayments/getSalesPaymentReceiptSerial",
                                                            data: pData,
                                                            success: function (vRetDataItemSerial) {
                                                                vDataItemSerial = vRetDataItemSerial.SalesPaymentData;

                                                                //Get Payment Info
                                                                AjaxSend({
                                                                    url: "SalesPayments/getSalesPaymentReceiptPaymentInfo",
                                                                    data: pData,
                                                                    success: function (vRetDataPayment) {
                                                                        vDataPayment = vRetDataPayment.SalesPaymentData;

                                                                        //Get Footer Amounts
                                                                        AjaxSend({
                                                                            url: "SalesPayments/getSalesPaymentReceiptFooterAmountsInfo",
                                                                            data: pData,
                                                                            success: function (vRetDataFooter) {
                                                                                vDataFooter = vRetDataFooter.SalesPaymentData;

                                                                                //Get Footer Messages
                                                                                AjaxSend({
                                                                                    url: "SalesPayments/getSalesPaymentReceiptFooterMessagesInfo",
                                                                                    data: pData,
                                                                                    success: function (vRetDataMsg) {
                                                                                        vDataMsg = vRetDataMsg.SalesPaymentData;

                                                                                        if (window.InAndroid) { //Android Mobile Or Mobile (window)
                                                                                            var vDataPrinterType = SalesPayment_offline().getOrgFOWSTPrinterType();

                                                                                            if (vDataPrinterType.Rows.length > 0) {
                                                                                                if (vDataPrinterType.Rows[0].PrinterType == 'Epson TM-U220B Printer') {
                                                                                                    PrintPOSReceiptDotMatrix(pInvoiceID, vDataPrint, vDataHeader, vDataInvHeader, vDataItem, vDataItemSerial, vDataPayment, vDataFooter, vDataMsg);
                                                                                                } else if (vDataPrinterType.Rows[0].PrinterType == '80MM Mini Thermal Printer') {

                                                                                                } else if (vDataPrinterType.Rows[0].PrinterType == 'Epson G Printer') {
                                                                                                    sessionStorage.Printer = 'GPrinter';
                                                                                                    PrintPOSReceiptNow(pInvoiceID, vDataPrint, vDataHeader, vDataInvHeader, vDataItem, vDataItemSerial, vDataPayment, vDataFooter, vDataMsg);
                                                                                                } else if (vDataPrinterType.Rows[0].PrinterType == 'Windows Print Server') {
                                                                                                    PrintPOSReceiptViaWindowsPrintServer(pInvoiceID, vDataPrint, vDataHeader, vDataInvHeader, vDataItem, vDataItemSerial, vDataPayment, vDataFooter, vDataMsg)                                                                                                    
                                                                                                } else {
                                                                                                    PrintPOSReceiptNow(pInvoiceID, vDataPrint, vDataHeader, vDataInvHeader, vDataItem, vDataItemSerial, vDataPayment, vDataFooter, vDataMsg);
                                                                                                }
                                                                                            } else {
                                                                                                PrintPOSReceiptNow(pInvoiceID, vDataPrint, vDataHeader, vDataInvHeader, vDataItem, vDataItemSerial, vDataPayment, vDataFooter, vDataMsg);
                                                                                            }
                                                                                            //PrintPOSReceiptNow(pInvoiceID, vDataPrint, vDataHeader, vDataInvHeader, vDataItem, vRetDataItemSerial, vDataPayment, vDataFooter, vDataMsg);
                                                                                        } else { //Web App
                                                                                            var japplet = document.jzebra;
                                                                                            var blnIsPrinterFound = false;

                                                                                            if (japplet != null) {
                                                                                                while (!blnIsPrinterFound) {

                                                                                                    if (FindDesktopPrinter("")) {
                                                                                                        blnIsPrinterFound = true;

                                                                                                        PrintPOSDesktopReceiptNow(pInvoiceID, vDataHeader, vDataInvHeader, vDataItem, vDataItemSerial, vDataPayment, vDataFooter, vDataMsg);

                                                                                                    };

                                                                                                }
                                                                                            } else {
                                                                                                MessageBox("Jzebra applet was not loaded.",
                                                                                                    "Jzebra",
                                                                                                    [
                                                                                                        { Text: 'Ok'
                                                                                                        }
                                                                                                    ]
                                                                                                );
                                                                                            }
                                                                                        }


                                                                                    }
                                                                                });

                                                                            }
                                                                        });
                                                                    }
                                                                });

                                                            }
                                                        });
                                                    }
                                                });
                                            }
                                        });

                                    }
                                });

                            }
                        });
                    }

                    var pIU;

                    pIU = new InsertUpdate('private.TransActionLogs');

                    pIU.Add('ID', ReadyString(NewID()));
                    pIU.Add('ActionDate', proj.SqliteNow);
                    pIU.Add('EmployeeName', ReadyString(sessionStorage.CurrentUserFullName));
                    pIU.Add('FOWSTID', ReadyString(sessionStorage.FowstID));
                    pIU.Add('DeviceID', ReadyString(Device.Serial()));
                    pIU.Add('Action', ReadyString("Reprint Invoice Receipt"));
                    pIU.Add('OriginalValue', ReadyString($("#txtDuplicateReceiptReceiptNo").val()));

                    pSql = pIU.GetInsert();

                    Sql.ExecuteSql(pSql);

                }

                function PrintPOSReceiptNow(pInvoiceID, vDataPrint, vDataHeader, vDataInvHeader, vDataItem, vDataItemSerial, vDataPayment, vDataFooter, vDataMsg) {
                    var strTemp = '';
                    var strPrint = '';
                    var strPrinterIP = '';
                    var strStockID = '';
                    var IsShowSalesRep = false;
                    var pData = {};
                    pData.InvoiceID = pInvoiceID;

                    //Get Printer IP Address
                    if (vDataPrint.Rows.length > 0) {

                        strPrinterIP = vDataPrint.Rows[0].PrinterIPAddress;

                        //New set of text to print
                        NewPrintBuilder();

                        //Get Header
                        IsShowSalesRep = false;
                        if (vDataHeader.Rows.length > 0) {

                            IsShowSalesRep = (vDataHeader.Rows[0].IsShowSalesRep == 1 ? true : false);

                            strPrint = FormatTextToPrint(intFAFCTotalLenght, $.trim((vDataHeader.Rows[0].BusinessTradeName || '')), intFAFCTotalLenght, '', 0, '', 0, 1);
                            ReadyTextToPrint(strPrint, 3, 1, 1, 1, 1, 1, 0, 0, 0);

                            strPrint = '';
                            strTemp = FormatTextToPrint(intFAFCTotalLenght, $.trim((vDataHeader.Rows[0].CorporateName || '')), intFAFCTotalLenght, '', 0, '', 0, 1);
                            strPrint = strPrint + strTemp;

                            strTemp = FormatTextToPrint(intFAFCTotalLenght, 'TIN ' + $.trim((vDataHeader.Rows[0].TIN || '')), intFAFCTotalLenght, '', 0, '', 0, 1);
                            strPrint = strPrint + strTemp;

                            strTemp = FormatTextToPrint(intFAFCTotalLenght, $.trim((vDataHeader.Rows[0].CompleteAddress || '')), intFAFCTotalLenght, '', 0, '', 0, 1);
                            strPrint = strPrint + strTemp;

                            strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Tel. No.' + $.trim((vDataHeader.Rows[0].TelNo || '')), intFAFCTotalLenght, '', 0, '', 0, 1);
                            strPrint = strPrint + strTemp;

                            strTemp = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                            strPrint = strPrint + strTemp;

                            ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                            strPrint = '';
                            strTemp = FormatTextToPrint(intFAFCTotalLenght, SetPrefixPostfix('POS Accreditation', 20, ' ', 1), 19, ': ' + $.trim((vDataHeader.Rows[0].POSAccreditationNo || '')), intFAFCTotalLenght, '', 0, 0);
                            strPrint = strPrint + strTemp;

                            strTemp = FormatTextToPrint(intFAFCTotalLenght, SetPrefixPostfix('POS Registration No.', 20, ' ', 1), 19, ': ' + $.trim((vDataHeader.Rows[0].POSPermitNo || '')), intFAFCTotalLenght, '', 0, 0);
                            strPrint = strPrint + strTemp;

                            strTemp = FormatTextToPrint(intFAFCTotalLenght, SetPrefixPostfix('Machine ID', 20, ' ', 1), 19, ': ' + vDataHeader.Rows[0].MIN, intFAFCTotalLenght, '', 0, 0);
                            strPrint = strPrint + strTemp;

                            ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                            strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                            ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                            //Get Items
                            if (vDataInvHeader.Rows.length > 0) {
                                strPrint = '';
                                strTemp = FormatTextToPrint(intFAFCTotalLenght, SetPrefixPostfix('Customer', 8, ' ', 1), 7, ': ' + $.trim((vDataInvHeader.Rows[0].CustomerName || '')), intFAFCTotalLenght, '', 0, 0);
                                strPrint = strPrint + strTemp;

                                strTemp = FormatTextToPrint(intFAFCTotalLenght, SetPrefixPostfix('Address', 8, ' ', 1), 7, ': ' + $.trim((vDataInvHeader.Rows[0].CustomerAddress || '')), intFAFCTotalLenght, '', 0, 0);
                                strPrint = strPrint + strTemp;

                                strTemp = FormatTextToPrint(intFAFCTotalLenght, SetPrefixPostfix('TIN', 8, ' ', 1), 7, ': ' + $.trim((vDataInvHeader.Rows[0].CustomerTIN || '')), intFAFCTotalLenght, '', 0, 0);
                                strPrint = strPrint + strTemp;
                                ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                                strPrint = '';
                                strTemp = FormatTextToPrint(intFCDoubleTotalLenght, 'SI ' + SetPrefixPostfix($.trim((vDataInvHeader.Rows[0].InvoiceNumber || '')), 12, '0', 0), intFCDoubleTotalLenght, '', 0, '', 0, 1);
                                strPrint = strPrint + strTemp;

                                strTemp = FormatTextToPrint(intFCDoubleTotalLenght, ($.format.date(new Date(vDataInvHeader.Rows[0].InvoiceDate), 'MMM dd, yyyy HH:mm') || ''), intFCDoubleTotalLenght, '', 0, '', 0, 1);
                                strPrint = strPrint + strTemp;

                                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 1, 1, 0, 0, 0);

                                strPrint = FormatTextToPrint(intFAFCTotalLenght, 'Cashier: ' + ($.trim(vDataInvHeader.Rows[0].CashierName || '')), intFAFCTotalLenght, '', 0, '', 0, 0);
                                ReadyTextToPrint('\n' + strPrint, 3, 1, 1, 0, 0, 0, 0, 0);

                                strPrint = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 1);
                                ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                                //Get Items
                                var strProductID = "";
                                if (vDataItem.Rows.length > 0) {
                                    strStockID = '';
                                    strPrint = '';
                                    var intLimitPerBatchPrint = 0;
                                    for (var p = 0; p <= vDataItem.Rows.length - 1; p++) {

                                        var strStockDescription = $.trim((vDataItem.Rows[p].StockDescription || ''));
                                        var strUnitPrice = SetPrefixPostfix(AccountingFormat(vDataItem.Rows[p].UnitPrice || '0'), intFAFCTotalLenght - 30, ' ', 0);
                                        var strQty = SetPrefixPostfix(AccountingFormat((vDataItem.Rows[p].Quantity || '0'), 0), 8, ' ', 0);
                                        var strUnitMeasure = SetPrefixPostfix((vDataItem.Rows[p].UnitMeasure || ''), 10, ' ', 0);
                                        var strSubTotal = SetPrefixPostfix(AccountingFormat(vDataItem.Rows[p].SubTotal || '0'), intFAFCTotalLenght - 22, ' ', 0);

                                        strPrint = '';
                                        if (strProductID == "") {
                                            strProductID = vDataItem.Rows[p].StockID;
                                        }

                                        strTemp = FormatTextToPrint(intFAFCTotalLenght, strStockDescription, 28, strUnitPrice, intFAFCTotalLenght, '', 0, 0);
                                        strPrint = strPrint + strTemp;

                                        strTemp = FormatTextToPrint(intFAFCTotalLenght, strQty, 8, strUnitMeasure, 20, strSubTotal, intFAFCTotalLenght, 0);
                                        strPrint = strPrint + strTemp;


                                        //additional kay mag error kung wla 0ct 20
                                        if (vDataItemSerial.Rows != undefined) {
                                            if (vDataItemSerial.Rows.length > 0) {
                                                if (strProductID != vDataItem.Rows[p].StockID) { //Print Serials

                                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Serial(s) : ', intFAFCTotalLenght, '', 0, '', 0, 0);
                                                    strPrint = strPrint + strTemp;

                                                    for (var intSerial = 0; intSerial <= vDataItemSerial.Rows.length - 1; intSerial++) {

                                                        if (strProductID == vDataItemSerial.Rows[intSerial].ProductID && vDataItemSerial.Rows[intSerial].InvoiceID == pInvoiceID) {

                                                            var strSerial = $.trim((vDataItemSerial.Rows[intSerial].SerialNumber || ''));

                                                            strTemp = FormatTextToPrint(intFAFCTotalLenght, strSerial, intFAFCTotalLenght, '', 0, '', 0, 0);
                                                            strPrint = strPrint + strTemp;

                                                        }
                                                    }

                                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, '_______________________________________', intFAFCTotalLenght, '', 0, '', 0, 0);
                                                    strPrint = strPrint + strTemp;

                                                    strProductID = vDataItem.Rows[p].StockID;
                                                }
                                            }


                                            if (IsShowSalesRep) {

                                                strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Sales Person: ' + ($.trim(vDataItem.Rows[p].SalesPerson || '')), intFAFCTotalLenght, '', 0, '', 0, 0);
                                                strPrint = strPrint + strTemp;

                                            }

                                            if (vDataItemSerial.Rows.length > 0) {
                                                if (p == vDataItem.Rows.length - 1) {

                                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Serial(s) : ', intFAFCTotalLenght, '', 0, '', 0, 0);
                                                    strPrint = strPrint + strTemp;

                                                    for (var intSerial = 0; intSerial <= vDataItemSerial.Rows.length - 1; intSerial++) {

                                                        if (strProductID == vDataItemSerial.Rows[intSerial].ProductID && vDataItemSerial.Rows[intSerial].InvoiceID == pInvoiceID) {

                                                            var strSerial = $.trim((vDataItemSerial.Rows[intSerial].SerialNumber || ''));

                                                            strTemp = FormatTextToPrint(intFAFCTotalLenght, strSerial, intFAFCTotalLenght, '', 0, '', 0, 0);
                                                            strPrint = strPrint + strTemp;

                                                        }
                                                    }

                                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, '_______________________________________', intFAFCTotalLenght, '', 0, '', 0, 0);
                                                    strPrint = strPrint + strTemp;

                                                    strProductID = vDataItem.Rows[p].StockID;
                                                }
                                            } else {
                                                strTemp = FormatTextToPrint(intFAFCTotalLenght, '_______________________________________', intFAFCTotalLenght, '', 0, '', 0, 0);
                                                strPrint = strPrint + strTemp;
                                            }

                                        } else {
                                            strTemp = FormatTextToPrint(intFAFCTotalLenght, '_______________________________________', intFAFCTotalLenght, '', 0, '', 0, 0);
                                            strPrint = strPrint + strTemp;
                                        }


                                        ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                        intLimitPerBatchPrint = intLimitPerBatchPrint + 1;

                                        if (intLimitPerBatchPrint >= 100 && vDataItem.Rows.length > 100) {

                                            PrintText(strPrinterIP);
                                            ClearCommandBuffer();

                                            //New set of text to print because it causes error when you print directly all items at once.
                                            NewPrintBuilder();
                                            intLimitPerBatchPrint = 0;
                                        }
                                    };

                                    //Get Footer Amounts
                                    if (vDataPayment.Rows.length > 0) {

                                        //Get Footer Amounts
                                        if (vDataFooter.Rows.length > 0) {

                                            var strSKUCount = SetPrefixPostfix(AccountingFormat((vDataFooter.Rows[0].SKUCount || '0'), 0), intFAFCTotalLenght - 14, ' ', 0);
                                            var strTotalAmountDue = SetPrefixPostfix(AccountingFormat(vDataFooter.Rows[0].SubTotal || '0'), intFAFCTotalLenght - 22, ' ', 0);
                                            var strTotalDiscount = SetPrefixPostfix(AccountingFormat(vDataFooter.Rows[0].DiscountAmount || '0'), intFAFCTotalLenght - 22, ' ', 0);
                                            var strNetAmountDue = SetPrefixPostfix(AccountingFormat(CDec2(vDataFooter.Rows[0].NetAmountDue) + CDec2(vDataFooter.Rows[0].ChargeAmount) + CDec2(vDataFooter.Rows[0].TipAmount) + CDec2(vDataFooter.Rows[0].LocalTaxAmount)), intFAFCTotalLenght - 22, ' ', 0);

                                            var strTotalCharges = SetPrefixPostfix(vDataFooter.Rows[0].ChargeAmount || '0', intFAFCTotalLenght - 22, ' ', 0);
                                            var strTip = SetPrefixPostfix(vDataFooter.Rows[0].TipAmount || '0', intFAFCTotalLenght - 22, ' ', 0);

                                            var strVatSale = SetPrefixPostfix(AccountingFormat(vDataFooter.Rows[0].TotalAmountDue), intFAFCTotalLenght - 22, ' ', 0);
                                            var strVatAmount = SetPrefixPostfix(AccountingFormat(CDec2(vDataFooter.Rows[0].SubTotal) - CDec2(vDataFooter.Rows[0].TotalAmountDue)), intFAFCTotalLenght - 22, ' ', 0);
                                            var strOtherTax = SetPrefixPostfix(vDataFooter.Rows[0].LocalTaxAmount || '0', intFAFCTotalLenght - 22, ' ', 0);

                                            strPrint = '';
                                            strTemp = FormatTextToPrint(intFAFCTotalLenght, 'SKU Count: ', 12, strSKUCount, intFAFCTotalLenght, '', 0, 0);
                                            strPrint = strPrint + strTemp;

                                            strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Sub Total: ', 20, strTotalAmountDue, intFAFCTotalLenght, '', 0, 0);
                                            strPrint = strPrint + strTemp;

                                            if (isSeniorCitizenDiscount) {
                                                strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Less: ' + vDataFooter.Rows[0].TaxPercent + '% VAT ', 20, strVatAmount, intFAFCTotalLenght, '', 0, 0);
                                                strPrint = strPrint + strTemp;

                                                strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Total Amount: ', 20, strVatSale, intFAFCTotalLenght, '', 0, 0);
                                                strPrint = strPrint + strTemp;
                                            }

                                            strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Total Discount:   ', 20, strTotalDiscount, intFAFCTotalLenght, '', 0, 0);
                                            strPrint = strPrint + strTemp;

                                            if (CDec2(vDataFooter.Rows[0].ChargeAmount) > 0) {
                                                strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Total Charges:   ', 20, strTotalCharges, intFAFCTotalLenght, '', 0, 0);
                                                strPrint = strPrint + strTemp;
                                            }

                                            if (CDec2(vDataFooter.Rows[0].TipAmount) > 0) {
                                                strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Tip:   ', 20, strTip, intFAFCTotalLenght, '', 0, 0);
                                                strPrint = strPrint + strTemp;

                                            }

                                            if (CDec2(vDataFooter.Rows[0].LocalTaxAmount) > 0) {
                                                strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Local Tax:   ', 20, strOtherTax, intFAFCTotalLenght, '', 0, 0);
                                                strPrint = strPrint + strTemp;
                                            }


                                            strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Amount Due:   ', 20, strNetAmountDue, intFAFCTotalLenght, '', 0, 0);
                                            strPrint = strPrint + strTemp;
                                            ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 0, 0, 0, 0);

                                            if (vDataPayment.Rows.length > 0) {

                                                var strCashPayment = SetPrefixPostfix(AccountingFormat((vDataPayment.Rows[0].CashPayment || '0')), intFAFCTotalLenght - 7, ' ', 0);
                                                var strCheckPayment = SetPrefixPostfix(AccountingFormat(vDataPayment.Rows[0].CheckPaymentAmount || '0'), intFAFCTotalLenght - 8, ' ', 0);
                                                var strDebitCardAmount = SetPrefixPostfix(AccountingFormat(vDataPayment.Rows[0].DebitCardAmount || '0'), intFAFCTotalLenght - 14, ' ', 0);
                                                var strCreditCardAmount = SetPrefixPostfix(AccountingFormat(vDataPayment.Rows[0].CreditCardAmount || '0'), intFAFCTotalLenght - 14, ' ', 0);
                                                var strSRVAmount = SetPrefixPostfix(AccountingFormat(vDataPayment.Rows[0].SRVAmount || '0'), intFAFCTotalLenght - 6, ' ', 0);
                                                var strCustomerChangeCash = SetPrefixPostfix(AccountingFormat(vDataPayment.Rows[0].CustomerChangeCash || '0'), intFAFCTotalLenght - 9, ' ', 0);

                                                strPrint = '\n';
                                                if (vDataPayment.Rows[0].CashPayment > 0) {
                                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Cash:', 5, strCashPayment, intFAFCTotalLenght, '', 0, 0);
                                                    strPrint = strPrint + strTemp;
                                                }
                                                if (vDataPayment.Rows[0].CheckPaymentAmount > 0) {
                                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Check:', 6, strCheckPayment, intFAFCTotalLenght, '', 0, 0);
                                                    strPrint = strPrint + strTemp;
                                                }
                                                if (vDataPayment.Rows[0].DebitCardAmount > 0) {
                                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Debit Card:', 12, strDebitCardAmount, intFAFCTotalLenght, '', 0, 0);
                                                    strPrint = strPrint + strTemp;
                                                }
                                                if (vDataPayment.Rows[0].CreditCardAmount > 0) {
                                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Credit Card:', 12, strCreditCardAmount, intFAFCTotalLenght, '', 0, 0);
                                                    strPrint = strPrint + strTemp;
                                                }
                                                if (vDataPayment.Rows[0].SRVAmount > 0) {
                                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, 'SRV:', 4, strSRVAmount, intFAFCTotalLenght, '', 0, 0);
                                                    strPrint = strPrint + strTemp;
                                                }

                                                strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Change:', 7, strCustomerChangeCash, intFAFCTotalLenght, '', 0, 0);
                                                strPrint = strPrint + strTemp;

                                                ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                                if (vDataPayment.Rows[0].CheckPaymentAmount > vDataFooter.Rows[0].NetAmountDue) {
                                                    strPrint = FormatTextToPrint(intFAFCTotalLenght, '* Excess payments made through checks are not entitled to cash refunds.', intFAFCTotalLenght, '', 0, '', 0, 0);
                                                    ReadyTextToPrint('\n' + strPrint, 3, 1, 1, 0, 0, 0, 0, 0);
                                                }

                                            }

                                            strPrint = '';
                                            strTemp = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 0);
                                            strPrint = strPrint + strTemp;

                                            var strVATSales = SetPrefixPostfix(AccountingFormat(vDataFooter.Rows[0].TaxSales || '0'), intFAFCTotalLenght - 20, ' ', 0);
                                            var strVATAmount = SetPrefixPostfix(AccountingFormat(vDataFooter.Rows[0].TaxAmount || '0'), intFAFCTotalLenght - 15, ' ', 0);
                                            var strNonVAT = SetPrefixPostfix(AccountingFormat(vDataFooter.Rows[0].NonVATSale || '0'), intFAFCTotalLenght - 22, ' ', 0);
                                            var strVATExempt = SetPrefixPostfix(AccountingFormat(vDataFooter.Rows[0].VATExemptSale || '0'), intFAFCTotalLenght - 22, ' ', 0);
                                            var strVATZeroRated = SetPrefixPostfix(AccountingFormat(vDataFooter.Rows[0].ZeroRatedSale || '0'), intFAFCTotalLenght - 25, ' ', 0);


                                            strTemp = FormatTextToPrint(intFAFCTotalLenght, 'VAT-able Sales: ', 18, strVATSales, intFAFCTotalLenght, '', 0, 0);
                                            strPrint = strPrint + strTemp;

                                            strTemp = FormatTextToPrint(intFAFCTotalLenght, 'VAT Amount: ', 13, strVATAmount, intFAFCTotalLenght, '', 0, 0);
                                            strPrint = strPrint + strTemp;

                                            strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Non-VAT Sales: ', 20, strNonVAT, intFAFCTotalLenght, '', 0, 0);
                                            strPrint = strPrint + strTemp;

                                            strTemp = FormatTextToPrint(intFAFCTotalLenght, 'VAT Exempt Sales: ', 20, strVATExempt, intFAFCTotalLenght, '', 0, 0);
                                            strPrint = strPrint + strTemp;

                                            strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Zero-Rated VAT Sales: ', 23, strVATZeroRated, intFAFCTotalLenght, '', 0, 0);
                                            strPrint = strPrint + strTemp;

                                            strTemp = FormatTextToPrint(intFAFCTotalLenght, '________________________________________', intFAFCTotalLenght, '', 0, '', 0, 0);
                                            strPrint = strPrint + strTemp;

                                            ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 0, 0, 0, 0);

                                            //Get Footer Messages
                                            strPrint = '';
                                            //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Use for testing only. This is not an Official Receipt.', intFAFCTotalLenght, '', 0, '', 0, 1);
                                            //strTemp = FormatTextToPrint(intFAFCTotalLenght, 'This Serves as Your Official Receipt.', intFAFCTotalLenght, '', 0, '', 0, 1);
                                            strTemp = FormatTextToPrint(intFAFCTotalLenght, $.trim((vDataInvHeader.Rows[0].ReceiptFooterMsg || '')), intFAFCTotalLenght, '', 0, '', 0, 1);

                                            strPrint = strPrint + strTemp;

                                            if (vDataMsg.Rows.length > 0) {
                                                for (var p = 0; p <= vDataMsg.Rows.length - 1; p++) {
                                                    strTemp = FormatTextToPrint(intFAFCTotalLenght, $.trim((vDataMsg.Rows[p].MessageFooter || '')), intFAFCTotalLenght, '', 0, '', 0, 1);
                                                    strPrint = strPrint + strTemp;
                                                };

                                                ReadyTextToPrint(strPrint, 3, 1, 1, 1, 0, 1, 0, 0, 0);

                                                strPrint = FormatTextToPrint(intFAFCTotalLenght, 'This business is made more efficient by Mobiz. Visit us at www.mobiz-apps.com', intFAFCTotalLenght, '', 0, '', 0, 1);
                                                ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 0, 0);

                                                //Print Barcode
                                                strPrint = 'SI|' + vDataInvHeader.Rows[0].OrganizationID + '|' + vDataInvHeader.Rows[0].OrganizationBranchID + '|' + SetPrefixPostfix($.trim((vDataInvHeader.Rows[0].InvoiceNumber || '')), 12, '0', 0)
                                                ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 1, 0, 0);

                                                strPrint = SetPrefixPostfix($.trim((vDataInvHeader.Rows[0].InvoiceNumber || '')), 12, '0', 0);
                                                ReadyTextToPrint(strPrint, 3, 1, 1, 0, 0, 1, 0, 1, 0);

                                                ReadyTextToPrint('\n', 3, 1, 1, 0, 0, 0, 0, 0, 1);

                                                PrintText(strPrinterIP);
                                                ClearCommandBuffer();

                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else {
                        MessageBox("No printer assigned to this device. Please contact your system administrator for assistance.",
                                "Printer",
                                [
                                    { Text: 'Ok'
                                    }
                                ]
                        );
                    }

                    $('.ui-dialog').dialog('close');

                }

                function PrintPOSDesktopReceiptNow(pInvoiceID, vDataHeader, vDataInvHeader, vDataItem, vDataItemSerial, vDataPayment, vDataFooter, vDataMsg) {
                    var strTemp = '';
                    var strPrint = '';
                    var IsShowSalesRep = false;
                    var pData = {};
                    pData.InvoiceID = pInvoiceID;

                    var japplet = document.jzebra;

                    //Get Header
                    IsShowSalesRep = false;
                    if (vDataHeader.Rows.length > 0) {

                        IsShowSalesRep = (vDataHeader.Rows[0].IsShowSalesRep == 1 ? true : false);

                        strPrint = SetTextAlign($.trim(vDataHeader.Rows[0].BusinessTradeName || ''), 1, intDesktopPrintLenght);
                        japplet.append(strPrint + "\n");

                        strPrint = SetTextAlign($.trim(vDataHeader.Rows[0].CorporateName || ''), 1, intDesktopPrintLenght);
                        japplet.append(strPrint + "\n");

                        strPrint = SetTextAlign("TIN " + $.trim(vDataHeader.Rows[0].TIN || ''), 1, intDesktopPrintLenght);
                        japplet.append(strPrint + "\n");

                        strPrint = SetTextAlign($.trim(vDataHeader.Rows[0].CompleteAddress || ''), 1, intDesktopPrintLenght);
                        japplet.append(strPrint + "\n");

                        strPrint = SetTextAlign("Tel. No. " + $.trim(vDataHeader.Rows[0].TelNo || ''), 1, intDesktopPrintLenght);
                        japplet.append(strPrint + "\n");

                        strPrint = SetTextAlign('_____________________________________________', 1, intDesktopPrintLenght);
                        japplet.append(strPrint + "\n");

                        strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('POS Accred. No.', 0, 15), 15, ': ' + $.trim((vDataHeader.Rows[0].POSAccreditationNo || '')), intDesktopPrintLenght, '', 0);
                        japplet.append(strPrint + "\n");

                        strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('POS Reg. No.', 0, 12), 12, ': ' + $.trim((vDataHeader.Rows[0].POSPermitNo || '')), intDesktopPrintLenght, '', 0);
                        japplet.append(strPrint + "\n");

                        strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('Machine ID', 0, 10), 10, ': ' + vDataHeader.Rows[0].MIN, intDesktopPrintLenght, '', 0);
                        japplet.append(strPrint + "\n");

                        strPrint = SetTextAlign('_____________________________________________', 1, intDesktopPrintLenght);
                        japplet.append(strPrint + "\n");

                        //Get Invoice Header
                        if (vDataInvHeader.Rows.length > 0) {

                            strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('Customer            ', 0, 20), 20, ': ' + $.trim((vDataInvHeader.Rows[0].CustomerName || '')), intDesktopPrintLenght, '', 0);
                            japplet.append(strPrint + "\n");

                            strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('Address             ', 0, 20), 20, ': ' + $.trim((vDataInvHeader.Rows[0].CustomerAddress || '')), intDesktopPrintLenght, '', 0);
                            japplet.append(strPrint + "\n");

                            strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('TIN                 ', 0, 20), 20, ': ' + $.trim((vDataInvHeader.Rows[0].CustomerTIN || '')), intDesktopPrintLenght, '', 0);
                            japplet.append(strPrint + "\n");

                            if ($.trim((vDataInvHeader.Rows[0].CustomerTableNo || '')) != '') {
                                strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('Table #             ', 0, 20), 20, ': ' + $.trim((vDataInvHeader.Rows[0].CustomerTableNo || '')), intDesktopPrintLenght, '', 0);
                                japplet.append(strPrint + "\n");
                            }

                            strPrint = SetTextAlign('_____________________________________________', 1, intDesktopPrintLenght);
                            japplet.append(strPrint + "\n");

                            strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('Sales Inv. No. ', 0, 15), 15, ': ' + SetPrefixPostfix($.trim((vDataInvHeader.Rows[0].InvoiceNumber || '')), 12, '0', 0), intDesktopPrintLenght, '', 0);
                            japplet.append(strPrint + "\n");

                            strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('Date/Time      ', 0, 15), 15, ': ' + ($.format.date(new Date(vDataInvHeader.Rows[0].InvoiceDate), 'MMM dd, yyyy HH:mm') || ''), intDesktopPrintLenght, '', 0);
                            japplet.append(strPrint + "\n");

                            strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, SetTextAlign('Cashier        ', 0, 15), 15, ': ' + ($.trim(vDataInvHeader.Rows[0].CashierName || '')), intDesktopPrintLenght, '', 0);
                            japplet.append(strPrint + "\n");

                            strPrint = SetTextAlign('_____________________________________________', 1, intDesktopPrintLenght);
                            japplet.append(strPrint + "\n");

                            //Get Items
                            if (vDataItem.Rows.length > 0) {

                                var strProductID = "";
                                strPrint = '';
                                for (var p = 0; p <= vDataItem.Rows.length - 1; p++) {

                                    var strStockDescription = $.trim((vDataItem.Rows[p].StockDescription || ''));
                                    var strUnitPrice = SetPrefixPostfix(AccountingFormat(vDataItem.Rows[p].UnitPrice || '0'), intDesktopPrintLenght - 34, ' ', 0);
                                    var strQty = SetPrefixPostfix(AccountingFormat((vDataItem.Rows[p].Quantity || '0'), 0), 8, ' ', 0);
                                    var strUnitMeasure = SetPrefixPostfix((vDataItem.Rows[p].UnitMeasure || ''), 10, ' ', 0);
                                    var strSubTotal = SetPrefixPostfix(AccountingFormat(vDataItem.Rows[p].SubTotal || '0'), intDesktopPrintLenght - 26, ' ', 0);

                                    if (strProductID == "") {
                                        strProductID = vDataItem.Rows[p].StockID;
                                    }

                                    //Get Invoice Items Serial
                                    if (vDataItemSerial.Rows.length > 0) {
                                        if (strProductID != vDataItem.Rows[p].StockID) { //Print Serials

                                            strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, 'Serial(s) : ', intDesktopPrintLenght, '', 0, '', 0, 0);
                                            japplet.append(strPrint + "\n");

                                            for (var intSerial = 0; intSerial <= vDataItemSerial.Rows.length - 1; intSerial++) {

                                                if (strProductID == vDataItemSerial.Rows[intSerial].ProductID) {

                                                    var strSerial = $.trim((vDataItemSerial.Rows[intSerial].SerialNumber || ''));
                                                    strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, $.trim(strSerial || ''), intDesktopPrintLenght, '', 0, '', 0, 0);
                                                    japplet.append(strPrint + "\n");

                                                }
                                            }

                                            strPrint = SetTextAlign('_____________________________________________', 1, intDesktopPrintLenght);
                                            japplet.append(strPrint + "\n");

                                            strProductID = vDataItem.Rows[p].StockID;
                                        }
                                    }

                                    strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, strStockDescription, 32, strUnitPrice, intDesktopPrintLenght, '', 0);
                                    japplet.append(strPrint + "\n");

                                    strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, strQty, 8, strUnitMeasure, 24, strSubTotal, intDesktopPrintLenght);
                                    japplet.append(strPrint + "\n");

                                    if (IsShowSalesRep) {

                                        strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, 'Sales Person: ' + ($.trim(vDataItem.Rows[p].SalesPerson || '')), intDesktopPrintLenght, '', 0, '', 0, 0);
                                        japplet.append(strPrint + "\n");

                                    }

                                    if (vDataItemSerial.Rows.length > 0) {
                                        if (p == vDataItem.Rows.length - 1) { //Print Serials

                                            strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, 'Serial(s) : ', intDesktopPrintLenght, '', 0, '', 0, 0);
                                            japplet.append(strPrint + "\n");

                                            for (var intSerial = 0; intSerial <= vDataItemSerial.Rows.length - 1; intSerial++) {

                                                if (strProductID == vDataItemSerial.Rows[intSerial].ProductID) {

                                                    var strSerial = $.trim((vDataItemSerial.Rows[intSerial].SerialNumber || ''));
                                                    strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, $.trim(strSerial || ''), intDesktopPrintLenght, '', 0, '', 0, 0);
                                                    japplet.append(strPrint + "\n");

                                                }
                                            }

                                            strPrint = SetTextAlign('_____________________________________________', 1, intDesktopPrintLenght);
                                            japplet.append(strPrint + "\n");

                                            strProductID = vDataItem.Rows[p].StockID;
                                        }
                                    } else {
                                        strPrint = SetTextAlign('_____________________________________________', 1, intDesktopPrintLenght);
                                        japplet.append(strPrint + "\n");
                                    }

                                };

                                if (vDataPayment.Rows.length > 0) {

                                    //Get Footer Amounts
                                    if (vDataPayment.Rows.length > 0) {
                                        var strSKUCount = SetPrefixPostfix(AccountingFormat((vDataFooter.Rows[0].SKUCount || '0'), 0), intDesktopPrintLenght - 13, ' ', 0);
                                        var strTotalAmountDue = SetPrefixPostfix(AccountingFormat(vDataFooter.Rows[0].SubTotal || '0'), intDesktopPrintLenght - 20, ' ', 0);
                                        var strTotalDiscount = SetPrefixPostfix(AccountingFormat(vDataFooter.Rows[0].DiscountAmount || '0'), intDesktopPrintLenght - 20, ' ', 0);
                                        var strNetAmountDue = SetPrefixPostfix(AccountingFormat(CDec2(vDataFooter.Rows[0].NetAmountDue) + CDec2(vDataFooter.Rows[0].ChargeAmount) + CDec2(vDataFooter.Rows[0].TipAmount) + CDec2(vDataFooter.Rows[0].LocalTaxAmount)), intFAFCTotalLenght - 22, ' ', 0);

                                        var strTotalCharges = SetPrefixPostfix(vDataFooter.Rows[0].ChargeAmount || '0', intDesktopPrintLenght - 20, ' ', 0);
                                        var strTip = SetPrefixPostfix(vDataFooter.Rows[0].TipAmount || '0', intDesktopPrintLenght - 20, ' ', 0);

                                        var strVatSale = SetPrefixPostfix(AccountingFormat(vDataFooter.Rows[0].TotalAmountDue), intDesktopPrintLenght - 20, ' ', 0);
                                        var strVatAmount = SetPrefixPostfix(AccountingFormat(CDec2(vDataFooter.Rows[0].SubTotal) - CDec2(vDataFooter.Rows[0].TotalAmountDue)), intDesktopPrintLenght - 20, ' ', 0);
                                        var strOtherTax = SetPrefixPostfix(vDataFooter.Rows[0].LocalTaxAmount || '0', intFAFCTotalLenght - 22, ' ', 0);

                                        strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, 'SKU Count: ', 11, strSKUCount, intDesktopPrintLenght, '', 0);
                                        japplet.append(strPrint + "\n");

                                        strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, 'Sub Total: ', 18, strTotalAmountDue, intDesktopPrintLenght, '', 0);
                                        japplet.append(strPrint + "\n");

                                        if (isSeniorCitizenDiscount) {
                                            strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, 'Less: ' + vDataFooter.Rows[0].TaxPercent + '% VAT ', 18, strVatAmount, intDesktopPrintLenght, '', 0);
                                            japplet.append(strPrint + "\n");

                                            strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, 'Total Amount: ', 18, strVatSale, intDesktopPrintLenght, '', 0);
                                            japplet.append(strPrint + "\n");
                                        }
                                        strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Total Discount:   ', 20, strTotalDiscount, intFAFCTotalLenght, '', 0, 0);
                                        strPrint = strPrint + strTemp;

                                        if (CDec2(vDataFooter.Rows[0].ChargeAmount) > 0) {
                                            strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Total Charges:   ', 20, strTotalCharges, intFAFCTotalLenght, '', 0, 0);
                                            strPrint = strPrint + strTemp;
                                        }

                                        if (CDec2(vDataFooter.Rows[0].TipAmount) > 0) {
                                            strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Tip:   ', 20, strTip, intFAFCTotalLenght, '', 0, 0);
                                            strPrint = strPrint + strTemp;

                                        }

                                        if (CDec2(vDataFooter.Rows[0].LocalTaxAmount) > 0) {
                                            strTemp = FormatTextToPrint(intFAFCTotalLenght, 'Local Tax:   ', 20, strOtherTax, intFAFCTotalLenght, '', 0, 0);
                                            strPrint = strPrint + strTemp;
                                        }


                                        strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, 'Amount Due: ', 18, strNetAmountDue, intDesktopPrintLenght, '', 0);
                                        japplet.append(strPrint + "\n");

                                        if (vDataPayment.Rows.length > 0) {

                                            var strCashPayment = SetPrefixPostfix(AccountingFormat((vDataPayment.Rows[0].CashPayment || '0')), intDesktopPrintLenght - 8, ' ', 0);
                                            var strCheckPayment = SetPrefixPostfix(AccountingFormat(vDataPayment.Rows[0].CheckPaymentAmount || '0'), intDesktopPrintLenght - 9, ' ', 0);
                                            var strDebitCardAmount = SetPrefixPostfix(AccountingFormat(vDataPayment.Rows[0].DebitCardAmount || '0'), intDesktopPrintLenght - 15, ' ', 0);
                                            var strCreditCardAmount = SetPrefixPostfix(AccountingFormat(vDataPayment.Rows[0].CreditCardAmount || '0'), intDesktopPrintLenght - 15, ' ', 0);
                                            var strSRVAmount = SetPrefixPostfix(AccountingFormat(vDataPayment.Rows[0].SRVAmount || '0'), intDesktopPrintLenght - 7, ' ', 0);
                                            var strCustomerChangeCash = SetPrefixPostfix(AccountingFormat(vDataPayment.Rows[0].CustomerChangeCash || '0'), intDesktopPrintLenght - 10, ' ', 0);

                                            japplet.append("\n");

                                            if (vDataPayment.Rows[0].CashPayment > 0) {
                                                strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, 'Cash: ', 6, strCashPayment, intDesktopPrintLenght, '', 0);
                                                japplet.append(strPrint + "\n");
                                            }
                                            if (vDataPayment.Rows[0].CheckPaymentAmount > 0) {
                                                strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, 'Check: ', 7, strCheckPayment, intDesktopPrintLenght, '', 0);
                                                japplet.append(strPrint + "\n");
                                            }
                                            if (vDataPayment.Rows[0].DebitCardAmount > 0) {
                                                strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, 'Debit Card: ', 13, strDebitCardAmount, intDesktopPrintLenght, '', 0);
                                                japplet.append(strPrint + "\n");
                                            }
                                            if (vDataPayment.Rows[0].CreditCardAmount > 0) {
                                                strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, 'Credit Card: ', 13, strCreditCardAmount, intDesktopPrintLenght, '', 0);
                                                japplet.append(strPrint + "\n");
                                            }
                                            if (vDataPayment.Rows[0].SRVAmount > 0) {
                                                strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, 'SRV: ', 5, strSRVAmount, intDesktopPrintLenght, '', 0);
                                                japplet.append(strPrint + "\n");
                                            }

                                            strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, 'Change: ', 8, strCustomerChangeCash, intDesktopPrintLenght, '', 0);
                                            japplet.append(strPrint + "\n");

                                            if (vDataPayment.Rows[0].CheckPaymentAmount > vDataFooter.Rows[0].NetAmountDue) {
                                                strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, '* Excess payments made through checks are not entitled to cash refunds.', intDesktopPrintLenght, '', 0, '', 0);
                                                japplet.append(strPrint + "\n");
                                            }

                                        }

                                        strPrint = SetTextAlign('_____________________________________________', 1, intDesktopPrintLenght);
                                        japplet.append(strPrint + "\n");

                                        var strVATSales = SetPrefixPostfix(AccountingFormat(vDataFooter.Rows[0].TaxSales || '0'), intDesktopPrintLenght - 18, ' ', 0);
                                        var strVATAmount = SetPrefixPostfix(AccountingFormat(vDataFooter.Rows[0].TaxAmount || '0'), intDesktopPrintLenght - 14, ' ', 0);
                                        var strNonVAT = SetPrefixPostfix(AccountingFormat(vDataFooter.Rows[0].NonVATSale || '0'), intDesktopPrintLenght - 20, ' ', 0);
                                        var strVATExempt = SetPrefixPostfix(AccountingFormat(vDataFooter.Rows[0].VATExemptSale || '0'), intDesktopPrintLenght - 20, ' ', 0);
                                        var strVATZeroRated = SetPrefixPostfix(AccountingFormat(vDataFooter.Rows[0].ZeroRatedSale || '0'), intDesktopPrintLenght - 24, ' ', 0);

                                        strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, 'VAT-able Sales: ', 16, strVATSales, intDesktopPrintLenght, '', 0);
                                        japplet.append(strPrint + "\n");

                                        strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, 'VAT Amount: ', 12, strVATAmount, intDesktopPrintLenght, '', 0);
                                        japplet.append(strPrint + "\n");

                                        strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, 'Non-VAT Sales: ', 18, strNonVAT, intDesktopPrintLenght, '', 0);
                                        japplet.append(strPrint + "\n");

                                        strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, 'VAT Exempt Sales: ', 18, strVATExempt, intDesktopPrintLenght, '', 0);
                                        japplet.append(strPrint + "\n");

                                        strPrint = FormatTextToDesktopPrint(intDesktopPrintLenght, 'VAT Zero-Rated Sales: ', 22, strVATZeroRated, intDesktopPrintLenght, '', 0);
                                        japplet.append(strPrint + "\n");

                                        strPrint = SetTextAlign('_____________________________________________', 1, intDesktopPrintLenght);
                                        japplet.append(strPrint + "\n");

                                        //Get Footer Messages
                                        //strPrint = SetTextAlign('Use for testing only. This is not an Official Receipt.', 1, intDesktopPrintLenght);
                                        //strPrint = SetTextAlign('This Serves as Your Official Receipt.', 1, intDesktopPrintLenght);
                                        strPrint = SetTextAlign(vDataInvHeader.Rows[0].ReceiptFooterMsg, 1, intDesktopPrintLenght);

                                        japplet.append(strPrint + "\n");

                                        if (vDataMsg.Rows.length > 0) {
                                            for (var p = 0; p <= vDataMsg.Rows.length - 1; p++) {
                                                if (p <= vDataMsg.Rows.length - 1) {
                                                    strPrint = SetTextAlign($.trim((vDataMsg.Rows[p].MessageFooter || '')), 1, intDesktopPrintLenght);
                                                    japplet.append(strPrint + "\n");
                                                }
                                            };

                                            strPrint = SetTextAlign('This business is made more efficient by Mobiz', 1, intDesktopPrintLenght);
                                            japplet.append(strPrint + "\n");

                                            strPrint = SetTextAlign('Visit us at www.mobiz-apps.com', 1, intDesktopPrintLenght);
                                            japplet.append(strPrint + "\n");

                                            japplet.append("\n\n\n\n\n\n\n\n\n\n\n\n\n");

                                            japplet.print();

                                        } //Get Footer Messages

                                    } //Get Footer Amounts

                                } //Get Payment Info

                            } //Get Items

                        } //Get Invoice Header

                    } //Get Header

                }

                function doSendEmailReceipt(pData) {
                    //ClearCustomerSCPWDSession(); //additional by janine

                    

                    var vInvoiceData = SalesPayment_offline().getInvoiceDetailsByFOWSTDevice($.trim($("#txtDuplicateReceiptReceiptNo").val()));

                    pData = {
                        InvoiceID: vInvoiceData.Rows[0].ID,
                        FowstID: sessionStorage.FowstID,
                        EmailAddress: $.trim($("#txtDuplicateReceiptEmailAddress").val()),
                        DeviceID: Web.DeviceID()
                    }

                    MessageBox("Please wait while sending the receipt.",
                            "Email Receipt",
                            [
                                {
                                    Text: 'Ok',
                                    Click: function () {

                                        //$.mobile.changePage('');

                                        //if (window.location.pathname.toLowerCase() == '/salesmenu/posviewall')
                                        //    window.pageHeader.ShowPaymentsPage(false);
                                        $.mobile.changePage('#pageSalesPaymentDuplicateReceipt');

                                    }
                                }
                            ]
                    );

                    AjaxSend({
                        url: "SalesPayments/SendEmailReceipt",
                        data: pData,
                        success: function (vData) {
                            if (!IsPrintReceipt) {
                                MessageBox("Your transaction has been saved. An email receipt has been sent to the email address(es) that you specified.",
                                    "Email Receipt",
                                    [
                                        {
                                            Text: 'Ok',
                                            Click: function () {

                                                $.mobile.changePage('#pageSalesPaymentDuplicateReceipt');
                                                //if (!IsPrintReceipt) {
                                                //    goToSalesMenu();
                                                //    //                                                    window.location = GetPathFormat("/SalesMenu?mode=pos&OrgID=" + sessionStorage.OrgID);
                                                //}
                                            }
                                        }
                                    ]
                            );
                            } else {
                                $.mobile.changePage(window.pageSourcePage);
                                PrintReceipt(pInvoiceID);
                            }
                        },
                        error: function (vError) {
                            MessageBox("This feature requires internet connection.",
                                "Requires Internet Connection",
                                [
                                    {
                                        Text: 'Ok',
                                        Click: function () {

                                            $.mobile.changePage('');

                                            if (window.location.pathname.toLowerCase() == '/salesmenu/posviewall')
                                                window.pageHeader.ShowPaymentsPage(false);

                                        }
                                    }
                                ]
                        );
                            return;
                        }
                    });

                }

            });
        });
    });