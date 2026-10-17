require(['common', 'business',
    'promise!/Scripts3/Browser/LibraryFowst.js?crc=',
    '/Scripts3/Browser/ReadyToInvoice.js?crc=',
    '/Scripts3/model/InvoicePickedItems.js?crc='
    ], function () {

        require([(OfflineMode ? '/Scripts3/SalesMenu_offline.js?crc=' : ''), (OfflineMode ? '/Scripts3/Offline/Business/SystemDefaults.js?crc=' : '')], function () {

            $(function () {

                var mInvoiceID;
                var mData;

                (function Init() {

                    var pParams = GetUrlParameters();
                    $('#divInvoiceData').hide();

                    if (pParams.mode == 'ipi') {
                        $('#lnkCheckout').html('');
                        $('#lblLabel').html('New <br /> Releasing');
                        $('#mainContent').hide();
                        $('#divCartEmpty').hide();
                        $('#divReleasingPanel').show();
                        startclock('#releaseClock');
                        $("#releaseDate").selectmenu("refresh");

                        // $('#btnPOSMode').html('');
                        $('#IsPOSHeader')
                            .text('Search invoice for printing')
                            .click(function(e) {
                                e.preventDefault();
                                Browser.TransInvoices.InputData.NonZero = 0;
                                Browser.TransInvoices.InputData.SalesOrder = 0;
                                Browser.TransInvoices.InputData.ForInvoicePrinting = 1;
                                Browser.TransInvoices.Click = function(e) {
                                    HandlePrint({
                                        InvoiceID: e.ID,
                                        Success: function(vData) {
                                            Business.TransInvoice.CreatePdf({
                                                ID: e.ID,
                                                Callback: function(vFilename) {
                                                    Android.Toast('The invoice PDF has been created. PDF will be printed.');
                                                    UploadToPrintServer({
                                                        PrintServer: vData.PrintServer,
                                                        InvoiceID: vData.InvoiceID,
                                                        Filename: 'temp/pdf_creation/' + vFilename + '.pdf'
                                                    });
                                                    //Js.Execute({
                                                    //    Filename: vFilename,
                                                    //    command: 'Android.Print'
                                                    //});
                                                } // END Callback: function(vFilename) {
                                            }); // END Business.TransInvoice.CreatePdf({
                                        } // END Success: function() {
                                    }); // END HandlePrint({
                                }; // END Browser.TransInvoices.Click = function(e) { 
                                Browser.TransInvoices.Show();
                            }); // END .click(function(e) {
                        $('#btnMoreOptions span')
                            .text('Reprint Requests')
                            .click(function() {
                                Browser.TransInvoices.InputData.NonZero = 0;
                                Browser.TransInvoices.InputData.SalesOrder = 0;
                                Browser.TransInvoices.InputData.ForInvoicePrinting = 1;
                                Browser.TransInvoices.InputData.PendingReprintRequest = 1;
                                Browser.TransInvoices.Click = function(e) {

                                    mInvoiceID = e.ID;
                                    NewMessageBox('What do you want to do with this reprint request?', 'Reprint Request', 
                                        [
                                            {
                                                Text: 'Approve',
                                                Click: function() {
                                                    history.back();
                                                    ApproveReprintRequest({
                                                        InvoiceID: mInvoiceID,
                                                        Approve: 1,
                                                        Success: function() {   
                                                            NewMessageBox('Reprint request was successfully approved.');
                                                        },
                                                        Fail: function(vMessage) {
                                                            NewMessageBox(vMessage);
                                                        }
                                                    });
                                                }
                                            },
                                            {
                                                Text: 'Deny',
                                                Click: function() {
                                                    history.back();
                                                    ApproveReprintRequest({
                                                        InvoiceID: mInvoiceID,
                                                        Approve: 0,
                                                        Success: function() {   
                                                            NewMessageBox('Reprint request was successfully denied.');
                                                        },
                                                        Fail: function(vMessage) {
                                                            NewMessageBox(vMessage);
                                                        }
                                                    });
                                                }
                                            },
                                            {
                                                Text: 'Nothing'
                                            }
                                        ]
                                    ); // END NewMessageBox
                                }; // END Browser.TransInvoices.Click = function(e) { 
                                Browser.TransInvoices.Show();                                
                            }); // END btnMoreOptions.click
                    } // END if (pParams.mode == 'ipi') {

                    if (!GetUser())
                        return;

                    startclock('#clock');

                })(); // END (function Init() {

                startclock('#clock');

                //Funtions beyond here are to get user and fowst details
                function GetUser() {
                    var pUser;
                    if (OfflineMode) {
                        pUser = SalesMenu_offline().GetUser();
                        if ((pUser != undefined) && (pUser.FowstID == undefined)) {
                            if (pUser.FowstCount == undefined) {
                                MessageBox('The device you are currently using is not registered to any FOWST in the current organization.', null, null, {
                                    OnClose: function () {
                                        var pParam = {};
                                        if (IsMobile)
                                            pParam.IsMobile = 1;
                                        pParam = $.param(pParam);
                                        window.location = "/People/PeopleMenu?" + pParam;
                                    }
                                });
                                return false;
                            }
                        }
                        LoadUser(pUser);
                        SalesMenu_offline().UpdateCounterFromOnline();
                    } else {
                        AjaxSend({
                            url: 'Login3/GetUser',
                            data: { FowstID: sessionStorage.FowstID },
                            success: function (vData) {
                                LoadUser(vData);
                            },
                            error: function (vError) {
                                console.log('Error getting Login/GetUser. Retrying...');
                                GetUser();
                            }
                        });
                    } 
                }

                function LoadUser(vData) {

                    var pUserMenu;
                    var pLink;

                    if (CStr2(vData).length == 0) return;
                    if (vData.Error) {
                        MessageBox(vData.Error);
                        MessageBox(vData.Error, '', null, {
                            OnClose: function () {
                                window.location = '/People/PeopleMenu';
                            }
                        });
                        return;
                    };

                    if (vData.FirstName != undefined) {
                        pUserMenu = $('#divUserMenu');
                        pUserMenu.html('');

                        pLink = $('<a>')
                    .attr('href', '#pageUserMenu')
                    .css('text-decoration', 'none')
                    .html('Hi, ' + HtmlEncode(vData.FirstName) + '&#x25BC;');
                        pUserMenu.append(pLink);
                        if (CStr2(vData.FowstID).length > 0) {
                            sessionStorage.FowstID = vData.FowstID;
                            VerifyDevice();
                            sessionStorage.FowstName = vData.FowstName;
                            if (IsMobile)
                                Transfers.WriteFile(sessionStorage.OrgID + 'LastFowst.flg', null, vData.FowstID);
                        } else {
                            if (OfflineMode) {
                                BrowseAddresses();
                            } else {
                                var pData = { FilterFowst: 1 };
                                AjaxSend({
                                    url: 'Browser/LibraryFowst',
                                    data: pData,
                                    success: function (vFowstData) {

                                        if (vFowstData.Results.Rows.length == 1) {
                                            sessionStorage.FowstID = vFowstData.Results.Rows[0].ID;
                                            VerifyDevice();
                                            sessionStorage.FowstName = vFowstData.Results.Rows[0].FowstName;
                                        } else {
                                            BrowseAddresses();
                                        }
                                    }
                                });
                            }
                        }
                    }
                }

                function VerifyDevice() {
                    if (OfflineMode) return;
                    AjaxSend({
                        url: '/Login3/VerifyDevice',
                        success: function (vData) {
                            if (vData.Error != undefined) {
                                MessageBox('This device is not assigned to this FOWST', 'FOWST',
                                            [
                                                {
                                                    Text: 'Ok',
                                                    Click: function () {
                                                        window.location = '/People/PeopleMenu';
                                                    }
                                                }
                                            ]
                                );
                            }
                        }
                    });
                }

                function BrowseAddresses() {

                    Browser.LibraryFowst.InputData.OrganizationID = sessionStorage.OrgID;
                    Browser.LibraryFowst.InputData.FilterFowst = 1;
                    Browser.LibraryFowst.AfterClick = function (e) {
                        sessionStorage.FowstID = e.ID;
                        VerifyDevice();
                        sessionStorage.FowstName = e.FowstName;
                        if (OfflineMode) {
                            Android.UpdateSessionData({
                                FowstID: sessionStorage.FowstID
                            }); // disconnected checkout
                            SalesMenu_offline().UpdateCounterFromOnline();
                        }
                    };
                    Browser.LibraryFowst.Show();

                }

                $('#btnHome').click(function (e) {
                    e.preventDefault();
                    window.location = '/People/PeopleMenu';
                });

                $('#formSearch').submit(function(e) {
                    e.preventDefault();

                    Browser.ReadyToInvoice.InputData.OrganizationID = sessionStorage.OrgID;
                    Browser.ReadyToInvoice.InputData.FilterFowst = 1;
                    Browser.ReadyToInvoice.Click = function (e) {
                        App.model.InvoicePickedItems.getData({
                            InvoiceID: e.InvoiceID,
                            success: function(vData) {
                                mInvoiceID = e.InvoiceID;
                                mData = vData;
                                DisplayData();
                            },
                            error: function(vData) {
                                NewMessageBox(vData);
                            }
                        });
                    };
                    Browser.ReadyToInvoice.Show($('#txtSearch').val());

                    return false;
                });

                function DisplayData() {
                    
                    DisplayInvoice(mData.Invoice);
                    DisplaySections(mData.Sections);

                    function DisplayInvoice(vData) {
                        
                        $('#spanSalesOrderNo').text(vData.MIN + '-' + vData.SalesOrderNo);
                        $('#spanCustomer').text(vData.CompanyName);
                        $('#spanInvoiceDate').text($.format.date(new Date(vData.InvoiceDate), proj.DateFormat));
                        $('#divInvoiceData').show();

                    } // END function DisplayInvoice(vData) {

                    function DisplaySections(vData) {
                        var pDiv = $('#divResults').html('').show();
                        var pDivItem;
                        var pDR;
                        var pID;

                        var pRows = vData.Rows;

                        for (var pY = 0; pY <= pRows.length - 1; pY++) {
                            
                            pDR = pRows[pY];
                            pID = 'chk' + NewID();
                            pDivItem = $('<div>')
                                .addClass('DivItem')
                                .data('data', pDR)
                                .append($('<input>')
                                    .attr('type', 'checkbox')
                                    .addClass('SectionCheckbox')
                                    .attr('id', pID)
                                )
                                .append($('<label>')
                                    .attr('for', pID)
                                    .text(pDR.FowstName + ' - ' + pDR.SectionName)
                                );
                            pDiv.append(pDivItem);
                        }
                        $('.SectionCheckbox').checkboxradio();
                    } // END function DisplaySections(vData) {
                    
                } // END function DisplayData(vData) {

                $('#butMakeInvoice').click(function(e) {
                    
                    var pMsg;
                    var pDR;
                    var pData = {
                        InvoiceID: mInvoiceID,
                        Sections: []
                    };
                    var pCheckBoxes = $('.SectionCheckbox:checked');
                    if (pCheckBoxes.length == 0) {
                        NewMessageBox('Please check at least 1 section before proceeding.');
                        return;
                    }

                    pMsg = 'Mobiz is about to generate an invoice for:<br /><br />';
                    pMsg += 'Sales Order Number: ' + mData.Invoice.MIN + '-' + mData.Invoice.SalesOrderNo + '<br />';
                    pMsg += 'Dated: ' + $.format.date(new Date(mData.Invoice.InvoiceDate), proj.DateFormat) + '<br />';
                    pMsg += 'For: ' + mData.Invoice.CompanyName + '<br />';
                    pMsg += 'Listing items picked in the following warehouse or FOWST sections:<br /><br />';
                    for (var p = 0; p <= pCheckBoxes.length - 1; p++) {
                        pDR = $(pCheckBoxes[p]).closest('.DivItem').data('data');
                        pMsg += pDR.FowstName + ' - ' + pDR.SectionName + '<br />';
                    }
                    // pMsg += '<br />';

                    NewMessageBox(pMsg, 'Create invoice?', [
                        {
                            Text: 'Ok',
                            Click: Proceed
                        },
                        {
                            Text: 'Cancel'
                        }
                    ]);

                    function Proceed() {
                        for (var p = 0; p <= pCheckBoxes.length - 1; p++) {
                            pDR = $(pCheckBoxes[p]).closest('.DivItem').data('data');

                            pData.Sections.push({
                                FowstID: pDR.FowstID,
                                SectionID: pDR.SectionID
                            });
                        }

                        AjaxSend({
                            url: '/InvoicePickedItems/CreateInvoice',
                            data:pData,
                            success: function(vData) {
                                AfterCreate(vData);
                            }
                        });
                    } // END function Proceed() {

                    function AfterCreate(vData) {
                        if (vData.Error) {
                            NewMessageBox(vData.Error);
                            return;
                        }

                        //var pMessage = 'No. of section that were tagged as "ready to invoicing": ' + vData.SectionCount + '<br />'
                        //    + 'PL No\'s of generated sorting lists: ' + vData.CreatedSortingList + '.';
                        var pMsg = 'This invoicing process might take 2-6 minutes depending on several factors.<br />' +
                            'Would you like to:';

                        NewMessageBox(pMsg, '', [
                            {
                                Text: 'Wait for the invoice.' + 
                                    'The generated invoice will be displayed on this screen. You can then print this invoice on a connected printer.',
                                Click: function() {
                                    NewMessageBox('This function is not yet implemented. Please click "Search invoice for printing" to print an invoice.');
                                }
                            },
                            {
                                Text: 'Go back to the main page and come back for the invoice.' + 
                                    'You can come back for the invoice by going back to "Invoice Picked Item" module and searching for the SO number or customer name.',
                                Click: function() {
                                    Android.Toast('jsLGKVTF9HNZ');
                                }
                            }
                        ]);
                        $('#divInvoiceData, #divResults').hide();
                    }; // END function AfterCreate() {
                }); // END $('#butMakeInvoice').click(function(e) {

                // this function has UI
                function HandlePrint(vParams) {
                    /*
                    vParams = {
                        InvoiceID: 'guid',
                        Success: function(vData) {}
                            vData = {
                                InvoiceID: 'guid'
                                PrintServer: 'string'
                            }
                    };
                    */

                    var pDR;
                    var pDeviceID = Web.DeviceID();

                    pDR = Business.LibraryDeviceFowst.ByDeviceFowst(pDeviceID, sessionStorage.FowstID);
                    if ($.isEmptyObject(pDR)) {
                        NewMessageBox('Device fowst does not exist.');
                        return;
                    }
                    if (CStr2(pDR.PrintServer).trim() == '') {
                        NewMessageBox('Please assign a print server IP address or URL to this device fowst.');
                        return;
                    }
                    mInvoiceID = vParams.InvoiceID;

                    AjaxSend({
                        url: '/SalesMenu/GetAndIncrementPrintCount',
                        data: {
                            InvoiceID: vParams.InvoiceID
                        },
                        success: function(vData) {
                            if (vData.Error) {
                                NewMessageBox(vData.Error);
                                return;
                            }

                            if (vData.PrintCount == 0) {
                                vParams.Success({
                                    InvoiceID: vParams.InvoiceID,
                                    PrintServer: pDR.PrintServer
                                });
                            } else {
                                NewMessageBox('You may not print this invoice. This invoice was already printed.', 'Already printed', 
                                    [
                                        {
                                            Text: 'Request for reprint',
                                            Click: function() {
                                                history.back();
                                                setTimeout(function() {
                                                    $('#txtReprintRequestRemark').val('');
                                                    $('#popRequestReprint').popup('open');
                                                }, 1000);
                                            }
                                        },
                                        {
                                            Text: 'Enter authorized username & password to reprint',
                                            Click: function() {
                                                console.log('jsTFZR6TDNRP! enter username and password here');
                                            }
                                        },
                                        {
                                            Text: 'Cancel'
                                        }
                                    ]
                                );
                            }
                        }
                    });
                } // END HandlePrint

                function UploadToPrintServer(vParams) {
                    /*
                    vParams = {
                        InvoiceID: 'guid',
                        Filename: 'string',
                        PrintServer: 'string'
                    }
                    */

                    var pDestination = vParams.PrintServer + '/SalesMenu/PrintPdf';

                    Android.AddLog('Uploading PDF to print server, ' + vParams);
                    Transfers.Upload({
                        SourceFilename: vParams.Filename,
                        RelativeToAppPath: true,
                        Destination: pDestination,
                        Data: {
                            Print: true,
                            InvoiceID: vParams.InvoiceID
                        }
                    });

                } // END function UploadToPrintServer(vParams) 

                $('#butReprintRequestSave').click(function() {
                    var pData = {
                        InvoiceID: mInvoiceID,
                        Remarks: $('#txtReprintRequestRemark').val()
                    };
                    AjaxSend({
                        url: '/SalesMenu/RequestReprint',
                        data: pData,
                        success: function(vData) {
                            if (vData.Error) 
                                NewMessageBox(vData.Error);
                            else if (vData.Success)
                                NewMessageBox('Reprint request was successfully saved.');
                            else
                                NewMessageBox('Unknown response from the server js2RFTJAY9VL!');
                        }
                    });
                });

                function ApproveReprintRequest(vParams) {
                    /*
                    vParams = {
                        InvoiceID: 'guid',
                        Approve: 1, // 1 for approve
                                    // 0 for deny
                        Success: function(vData) {
                        },
                        Fail: function(vMessage) {
                        }
                    }
                    */

                    AjaxSend({
                        url: '/SalesMenu/ApproveReprintRequest',
                        data: {
                            InvoiceID: vParams.InvoiceID,
                            Approve: vParams.Approve
                        },
                        success: function(vData) {
                            if (vData.Error)
                                vParams.Fail(vData.Error);
                            else if (vData.Success)
                                vParams.Success();
                            else
                                vParams.Fail('Unknown reply from server. js4MSK6LJVC3!');
                        }
                    }); // END AjaxSend({
                } // END function ApproveReprintRequest(vParams) {
            }); // $(function() {
        }); // require 
    });                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                      // require
