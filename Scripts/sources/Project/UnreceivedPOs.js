$(function () {

    var divUnreceivedPOs = $('#divUnreceivedPOs')
    var divTable = divUnreceivedPOs.find('[name=divTable]');
    var divPOsCount = divUnreceivedPOs.find('[name=divPOsCount]');
    var optExpired = divUnreceivedPOs.find('[name=optExpired]');
    var butRefresh = divUnreceivedPOs.find('[name=butRefresh]')
        .click(butRefresh_Click);
    var divResult = divUnreceivedPOs.find('[name=divResult]');
    var mSingleView = new SingleView();
    var mComposeEmailView = new ComposeEmailView();

    butRefresh_Click();

    function LoadGrid(vData) {
        
        vData.Columns = {
            LinkColumn: {
                Label: 'View',
                Control: 'Custom',
                CustomCallback: function(vData) {
                    return $('<a>')
                        .attr('href', '#')
                        .text('View')
                        .click(function(e) {
                            e.preventDefault();
                            mSingleView.GetPo(vData.ID);
                        });
                }
            },
//            Action: {
//                Label: 'Action',
//                Control: 'Custom',
//                CustomCallback: function(vData) {
//                    var pOut;

//                    pOut = $('<a>')
//                        .attr('href', '#')
//                        .text('Instant send email')
//                        .click(function(e) {
//                            e.preventDefault();
//                            ShowMessage('The code to send email to PO#' + vData.PONo + ' goes here', 'Not yet implemented');
//                        });
//                    pOut = pOut.add($('<span>').text(' | '));
//                    pOut = pOut.add(
//                        $('<a>')
//                            .attr('href', '#')
//                            .text('Compose email')
//                            .click(function(e) {
//                                e.preventDefault();
//                                mComposeEmailView.Show();
//                                //ShowMessage('The code to compose email to PO#' + vData.PONo + ' goes here', 'Not yet implemented');
//                            })
//                        );
//                    return pOut;
//                }
//            },
            'PONo': {
                'Label': 'PO #',
                'MajorDataType': 'Numeric'
            },
            'DatePO': {
                'Label': 'PO Date',
                'MajorDataType': 'Date',
                'Format': 'MMMM d, yyyy'
            },
            'SupplierName': {
                'Label': 'Supplier'
            },
            ContactNumber: {
                Label: 'Supplier contact #'
            },
            EmailAddress: {
                Label: 'Supplier email'
            },
            DateNeeded: {
                Label: 'Date needed',
                'MajorDataType': 'Date',
                'Format': 'MMMM d, yyyy'
            },
            CancellationDate: {
                Label: 'Cancellation date',
                'MajorDataType': 'Date',
                'Format': 'MMMM d, yyyy'
            },
            ShipperName: {
                Label: 'Shipper'
            },
            TotalAmount: {
                Label: 'PO amount of unserved line items',
                MajorDataType: 'Numeric',
                Format: '#,##0.00',
                Alignment: 'Right'
            },
            FollowUpRemarks: {
                Label: 'Last follow up remarks'
            },
            FollowUpDate: {
                Label: 'Last follow up date'
            },
            FollowUpUser: {
                Label: 'Last followed up by'
            },
            FollowUpsCount: {
                Label: '# of follow ups',
                MajorDataType: 'Numeric',
                Alignment: 'Right'
            }
        };
         
        divTable.ProWebGrid(vData);
        if (vData) 
            divPOsCount.text('No. of PO\'s: ' + vData.Rows.length);
        else {
            divPOsCount.text('No. of PO\'s: 0');
        };

    }; // END LoadGrid

    function butRefresh_Click() {

        var pData;

        pData = $('[name=optExpired]:checked').val();
        if (pData != 'ActiveOnly') 
            pData = {
                optExpired: pData
            };
        else
            pData = null;
        divUnreceivedPOs.block();
        divResult.empty();
        AjaxSend({
            url: '/Reports/UnreceivedPOsData',
            data: pData,
            success: function(vData) {
                divUnreceivedPOs.unblock();
                if (vData && vData.d) {
                    if (vData.d.Error) {
                        divResult.text(vData.d.Error).MakeError();
                        HandleSessionExpired(vData);
                    } else
                        LoadGrid(vData.d);
                } else 
                    divResult.text('Unknown error').MakeError();
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                divUnreceivedPOs.unblock();
                if (XMLHttpRequest.responseText)
                    divResult.text(XMLHttpRequest.responseText).MakeError();
                else
                    divResult.text('Unknown error').MakeError();
            }
        });
    }; // END butRefresh_Click

    function SingleView() { // class

        var divSingle = divUnreceivedPOs.find('[name=divSingle]')
            .hide();
        var lblHeader = divSingle.find('[name=lblHeader]');
        var spanPoNum = divSingle.find('[name=spanPoNum]');
        var spanPoDate = divSingle.find('[name=spanPoDate]');
        var spanSupplier = divSingle.find('[name=spanSupplier]');
        var spanSupplierContactNumber = divSingle.find('[name=spanSupplierContactNumber]'); 
        var spanSupplierEmail = divSingle.find('[name=spanSupplierEmail]'); 
        var spanLineItems = divSingle.find('[name=spanLineItems]');
        var spanTotalAmount = divSingle.find('[name=spanTotalAmount]');
        var divDetailsTable = divSingle.find('[name=divDetailsTable]');
        var butPopup = divSingle.find('[name=butPopup]')
            .hide()
            .click(butPopup_Click);
        var divDialog; // created dynamically
        var mPopupMode = true;
        var divFollowUpHistoryData = divSingle.find('[name=divFollowUpHistoryData]'); // data table
        var divSingleFollowUpHistoryResult = divSingle.find('[name=divSingleFollowUpHistoryResult]'); 
        var butFollowUpHistoryRefresh = divSingle.find('[name=butFollowUpHistoryRefresh]')
            .click(butFollowUpHistoryRefresh_Click);
        var txtFollowUpRemarks = divSingle.find('[name=txtFollowUpRemarks]');
        var butFollowUp = divSingle.find('[name=butFollowUp]')
            .click(butFollowUp_Click);;
        var divSingleFollowUpResult = divSingle.find('[name=divSingleFollowUpResult]');
        var mID;
        var mGridStocks;
        var mGridFollowUpHistory;

        $('#divSingleTabs')
            .tabs({
                show: function(event, ui) {
                    
                    switch (ui.tab.text) {
                        case 'Follow-up':
                            txtFollowUpRemarks.focus();
                            break;
                    };
                    
                } // END show
            });

        divDialog = $('<div>')
            .dialog({
                autoOpen: false,
                minWidth: 500,
                minHeight: 329,
                open: function() {
                    lblHeader.hide();
                    divDialog.append(divSingle);
                    mPopupMode = true;
                },
                beforeClose: function () {
                    butPopup.show();
                    lblHeader.show();
                    divTable.after(divSingle);
                    mPopupMode = false;
                }
            });

        this.GetPo = function(vID) {

            divSingle.block();
            AjaxSend({
                url: '/Reports/UnreceivedPoDataAjax',
                data: {ID: vID},
                success: function(vData) {
                    divSingle.unblock();
                    if (HandleSessionExpired(vData))
                        LoadData(vData.d)
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    divSingle.unblock();
                    if (XMLHttpRequest.responseText)
                        divResult.text(XMLHttpRequest.responseText).MakeError();
                    else
                        divResult.text('Unknown error').MakeError();
                }
            });

        }; // END this.GetPo

        function LoadData(vData) {

            divSingle.show();
            if (mPopupMode) butPopup_Click();
            LoadMaster(vData);
            LoadDetails(vData.Details);
            divDialog.dialog('option', 'title', lblHeader.text());
            txtFollowUpRemarks.val('');
            if (mGridFollowUpHistory)  mGridFollowUpHistory.ClearRows();

        }; // END LoadData

        function LoadMaster(vData) {

            var pData = vData.Master.Rows[0];

            mID = pData.ID;
            lblHeader.text('PO# ' + pData.PONo)
            spanPoNum.text(pData.PONo);
            spanPoDate.text($.format.date(new Date(pData.DatePO), 'MMM d, yyyy'));
            spanSupplier.text(pData.SupplierName);
            spanSupplierContactNumber.text(pData.ContactNumber);
            spanSupplierEmail.text(pData.EmailAddress);
            spanLineItems.text(vData.LineItems);
            spanTotalAmount.text($.format.number(vData.TotalAmount, '#,##0.00'));

        }; // END LoadMaster

        function LoadDetails(vData) {
            
            if (mGridStocks)
                mGridStocks.ReplaceDataRows(vData.Rows);
            else {
                vData.Columns = {
                    StockNumber: {
                        Label: 'Stock #'
                    },
                    StockDescription: {
                        Label: 'Stock description'
                    },
                    Quantity: {
                        Label: 'Qty',
                        MajorDataType: 'Numeric',
                        Format: '#,##0',
                        Alignment: 'Right'
                    },
                    UnitMeasure: {
                        Label: 'Unit measure'
                    },
                    UnitPrice: {
                        Label: 'Unit price',
                        MajorDataType: 'Numeric',
                        Format: '#,##0.00',
                        Alignment: 'Right'
                    },
                    SubTotal: {
                        Label: 'Subtotal',
                        MajorDataType: 'Numeric',
                        Format: '#,##0.00',
                        Alignment: 'Right'
                    },
                    OrderedQuantity: {
                        Label: 'Ordered Qty (in base unit)',
                        MajorDataType: 'Numeric',
                        Format: '#,##0',
                        Alignment: 'Right'
                    },
                    ReceivedQuantityGood: {
                        Label: 'Received Good Qty (in base unit)',
                        MajorDataType: 'Numeric',
                        Format: '#,##0',
                        Alignment: 'Right'
                    },
                    ReceivedQuantityDamaged: {
                        Label: 'Received Damaged Qty (in base unit)',
                        MajorDataType: 'Numeric',
                        Format: '#,##0',
                        Alignment: 'Right'
                    }
                };
                mGridStocks = divDetailsTable.ProWebGrid(vData);
            };

        }; // END LoadDetails

        function butPopup_Click() {

            butPopup.hide();
            divDialog.dialog('open');

        }; // END butPopup_Click

        function butFollowUp_Click(e) {
            
            if (txtFollowUpRemarks.val().trim() == '') {
                divSingleFollowUpResult.text('Please enter remarks').MakeError();
                txtFollowUpRemarks.focus();
                return;
            };
            divSingleFollowUpResult.hide();
            divSingle.block();
            AjaxSend({
                url: '/Reports/AddFollowUp',
                data: {
                    ID: mID,
                    Remarks: txtFollowUpRemarks.val().trim()
                },
                success: function(vData) {
                    divSingle.unblock();
                    if (HandleSessionExpired(vData) == false) return;
                    if (vData.d.Text == 'Ok') {
                        txtFollowUpRemarks.val('');
                        divSingleFollowUpResult.text('Data was successfully saved').MakeHighlight();
                    };
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    divSingle.unblock();
                    if (XMLHttpRequest.responseText)
                        divSingleFollowUpResult.text(XMLHttpRequest.responseText).MakeError();
                    else
                        divSingleFollowUpResult.text('Unknown error').MakeError();
                }
            });

        }; // END butFollowUp_Click

        function butFollowUpHistoryRefresh_Click() {

            divSingleFollowUpHistoryResult.hide();
            divSingle.block();
            AjaxSend({
                url: '/Reports/GetFollowUpHistory',
                data: {
                    ID: mID
                },
                success: function(vData) {
                    divSingle.unblock();
                    if (HandleSessionExpired(vData) == false) return;
                    if (mGridFollowUpHistory) 
                        mGridFollowUpHistory.ReplaceDataRows(vData.d.Rows);
                    else {
                        vData.d.Columns = {
                            Remarks: {
                                Label: 'Remarks'
                            },
                            FullName: {
                                Label: 'Followed up by'
                            },
                            LastUpdated: {
                                Label: 'Date time',
                                MajorDataType: 'Date'
                            }
                        };
                        mGridFollowUpHistory = divFollowUpHistoryData.ProWebGrid(vData.d);
                    };
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    divSingle.unblock();
                    if (XMLHttpRequest.responseText)
                        divSingleFollowUpHistoryResult.text(XMLHttpRequest.responseText).MakeError();
                    else
                        divSingleFollowUpHistoryResult.text('Unknown error').MakeError();
                }
            });

        }; // END butFollowUpHistoryRefresh_Click

    }; // SingleView

    function ComposeEmailView() { // class

        var divCompose = divUnreceivedPOs.find('[name=divCompose]')
            .hide();
        var divComposeWrapper = divUnreceivedPOs.find('[name=divComposeWrapper]');
        var lblHeader = divCompose.find('[name=lblHeader]');
        var butPopup = divCompose.find('[name=butPopup]')
            .hide()
            .click(butPopup_Click);
        var divDialog; // created dynamically
        var mPopupMode = true;
        
        divDialog = $('<div>')
            .dialog({
                autoOpen: false,
                minWidth: 500,
                minHeight: 329,
                open: function() {
                    lblHeader.hide();
                    divDialog.append(divCompose);
                    mPopupMode = true;
                },
                beforeClose: function () {
                    butPopup.show();
                    lblHeader.show();
                    divComposeWrapper.append(divCompose);
                    mPopupMode = false;
                }
            });

        this.Show = function() {
            
            divCompose.show();
            if (mPopupMode) butPopup_Click();

        }; // END this.Show

        function butPopup_Click() {

            butPopup.hide();
            divDialog.dialog('open');

        }; // END butPopup_Click

    }; // ComposeEmailView
});