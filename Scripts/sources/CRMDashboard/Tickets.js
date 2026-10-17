require(['common', 'promise!/Scripts/jquery.filedownload.min.js'],
function () {
    $(document).ready(function () {
        var currentTicketNo = 0, rmaID = '', arrItems = [], isItemEdit = false, curItemIndex, itemState, isAddTicket = true;
        var dataTicketDetails = null, selectedClientFowstID = null;

        window.Tickets = {
            GetClientTicketSummary: function () {
                GetClientTicketSummary();
                IssueReporting();
            },
            GetAdditionalDataForTicket: function () {
                GetAdditionalDataForTicket();
            },
            GetTicket: function () {
                if (CBool(JSON.parse(sessionStorage.ClientInfo).ViewTicketFromMobiz) || sessionStorage.OrgID.toLowerCase() == 'cfe39df6-8809-4973-9515-810df75aed12' || sessionStorage.OrgID.toLowerCase() == '84e972ed-f208-4867-9506-5b8f9572562f') {
                    $('.divfg').show();
                    AjaxSend({
                        url: 'CRMDashboard/GetClientGroupFOWST',
                        data: { ClientID: JSON.parse(sessionStorage.ClientInfo).ID },
                        success: function (e) {
                            var selFilterFOWSTGroup = $('#selFilterFOWSTGroup');

                            selFilterFOWSTGroup.empty();
                            selFilterFOWSTGroup.append(new Option('All FOWST\'s Group', 'all'));

                            if (e.Results != undefined) {
                                var vRows = e.Results.Rows;
                                for (var i = 0; i < vRows.length; i++) {
                                    selFilterFOWSTGroup.append($('<option value="' + vRows[i].ID + '">').attr('isfranchisee', vRows[i].IsFranchisee).text(vRows[i].GroupName));
                                }
                            }

                            GetTicket();
                        }
                    })
                } else {
                    $('.divfg').hide();
                    GetTicket();
                }                

                if (CBool(sessionStorage.fromServiceTicket)) $('#popTicketDetails').modal('show');
            }
        }

        $('body')
        .on('click', '#btnNoteSearchKB', function () {
            BrowsePopup('LibraryKBCase', 'Search Topic', [{
                DisplayText: [{ Column: 'Case' }],
                Click: function (e) {
                    $('#txtTakeANote').val($('#txtTakeANote').val() + e.Case + '\n').focus();
                }
            }])
        })
        .on('click', '.btnNoteIssueTicket', function () {
            sessionStorage.AddTicketViaDashboard = false;
            if (arrNoteTicket.length > 0) {
                WebToast('You can only issue one ticket per note.', 'top');
            } else {
                PromptUserAccess('Add Issue Ticket', {
                    OnClickOKSuccess: function () {
                        ClearTicket();
                        arrNoteTicket = [];
                        isAddTicket = true;
                        sessionStorage.removeItem('ClientInfo');
                        $('#popIssueTicket').modal('show');
                        $('#tabTicketDetails').opentabmenu();
                        $('#radTicketOpen').prop('checked', true);//.checkboxradio('refresh');
                        $('#btnTabTicketComments, #btnTabTicketHistory, #btnTabTicketRMA, #btnTabTicketAttachments').hidetabmenu();
                        $('#divTicketNumber').hide();
                    }
                });
            }
        })
        .on('click', '#btnTicketTimeFrom, #btnTicketTimeTo', function () {
            var dis = $(this);
            CustomSetTime({
                Click: function (e) {
                    dis.text(e.time);//.button('refresh');
                }
            })
        })
        .on('click', '#btnTicketDueDate', function () {
            var dis = $(this);
            SingleCalendar({
                Click: function (e) {
                    dis.text(e);//.button('refresh');
                }
            })
        })
        .on('click', '#btnTicketAssignedTo', function () {
            BrowsePopup('LibraryPeople', 'Search Employee', [{
                AutoSearch: true,
                DisplayText: [{ Column: 'PeopleName' }],
                Click: function (e) {
                    $('#txtTicketAssignedToID').val(e.ID);
                    $('#btnTicketAssignedTo').text(e.PeopleName);//.button('refresh');
                }
            }], {EmployeesOnly: 1})
        })
        .on('change', '#selTicketType', function () {
            if ($(this).val() == "Others") {
                $('#divTicketOthersField').show();
            } else {
                $('#divTicketOthersField').hide();
            }
        })
        .on('change', '#selTicketRecommendedAction', function () {
            if ($(this).val() == "Others") {
                $('#divTicketOthersRecommendedField').show();
            } else {
                $('#divTicketOthersRecommendedField').hide();
            }
        })
        .on('click', '.btnSaveTicket', function () {
            SaveTicket($(this).attr('print'));
        })
        .on('click', '#btnTicketAddComment', function () {
            $('#popTicketNewComment').modal('show');
            $('#txtTicketNewComment').empty('');
            placeCaretAtEnd($('#txtTicketNewComment')[0]);
        })
        .on('click', '#btnSaveTicketComment', function () {
            //if ($.trim($('#txtTicketNewComment').val()) == "") {
            //    WebToast('Please type your comment', 'top');
            //    return;
            //}

            $('#txtTicketNewComment').find('img').removeAttr('src').attr('alt', 'Saving image...');

            var noOfPics = $('#txtTicketNewComment').find('.crmpic.new').length
            var hasPics = (noOfPics > 0 ? true : false);

            Bs.Loading('show');
            $('#btnSaveTicketComment').prop('disabled', true);

            AjaxSend({
                url: 'CRMDashboard/SaveTicketComment',
                data: { Comment: HTMLEncode($('#txtTicketNewComment').html()), Date: $.format.date(new Date(), 'MMMM dd,yyyy hh:mm a'), ClientID: JSON.parse(sessionStorage.ClientInfo).ID, TicketID: sessionStorage.ParentTicketID },
                success: function (vData) {
                    if (hasPics) {
                        var ctrSaving = 0;

                        Bs.Loading('show', "Please wait while uploading attachment(s) ...");

                        $('#txtTicketNewComment').find('.crmpic.new').each(function () {
                            var formdatas = new FormData();
                            formdatas.append('files', $(this).data('file'));

                            $.ajax({
                                url: '/CRMDashboard/SaveAttachFile?OrgID=' + sessionStorage.OrgID + '&RelatedID=' + vData.Results + '&ImgID=' + $(this).attr('imgid'),
                                type: 'POST', data: formdatas, async: true, cache: false, contentType: false, processData: false,
                                success: function (e) {
                                    ctrSaving += 1;

                                    if (e == "saved") {
                                        if (noOfPics == ctrSaving) {
                                            ConfirmationOfSavingTicketComment(vData)
                                        }
                                    } else {
                                        NewMessageBox(e, 'Message');
                                    }
                                }
                            });
                        })
                    } else {
                        ConfirmationOfSavingTicketComment(vData)
                    }
                }
            })

            function ConfirmationOfSavingTicketComment(vData) {
                if (IsGuid(vData.Results)) {
                    $('#popTicketNewComment').modal('hide');
                    GetTicketComments();
                } else {
                    NewMessageBox(vData.Results, 'Message');
                }
                $('#btnSaveTicketComment').removeAttr('disabled');
                Bs.Loading('hide');
            }
        })
        .on('click', '.btnTicketAttachImage', function () {
            $('#fileTicketPic').click();
        })
        .on('submit', '#formTicketPic', function (e) {
            e.preventDefault();
            var formData = new FormData($(this)[0]);

            var reader = new FileReader();

            reader.onload = function (e) {
                //setCaretPosition(caretPos.row, caretPos.col)
                var file;
                var imgID = NewID();

                $('#fileTicketPic')[0].files[0].name = imgID + '_' + $('#fileTicketPic')[0].files[0].name;
                placeCaretAtEnd($('#txtTicketNewComment')[0])
                insertAtCursor('img', 'img', e.target.result, imgID, 'uploading', $('#fileTicketPic')[0].files[0])
                placeCaretAtEnd($('#txtTicketNewComment')[0])
            };
            reader.readAsDataURL($('#fileTicketPic')[0].files[0]);
        })
        .on('change', '#fileTicketPic', function (e) {
            e.preventDefault();
            if ($(this).val() != "") {
                $('#formTicketPic').submit()
            }
        })
        .on('click', '#fileTicketPic', function (e) {
            caretPos = getCaretPosition();
        })
        .on('change', '#selFilterClientTicket', function () {
            GetTicket();
        })
        .on('change', '#selFilterClientTicketDetails', function () {
            sessionStorage.fromServiceTicket = true;
            GetTicket();
        })
        .on('click', '#btnTabTicketHistory', function () {
            GetTicketHistory();
        })
        .on('click', '#btnDashboardAddTicket', function () {
            PromptUserAccess('Add Issue Ticket', {
                OnClickOKSuccess: function () {
                    $('#divTicketClientList').show();
                    $('.btnNoteIssueTicket').click();
                    sessionStorage.AddTicketViaDashboard = true;
                }
            })            
        })
        .on('click', '#btnAddRMA', function () {
            $('#popRMA').modal('show');
            ClearRMAFields();
            $('#txtRMAReturnTo').focus();
            itemState = 'add';
            $('#btnTabRMAEditTrail').hide();
        })
        .on('click', '#txtRMADate, #txtRMAWBBlDate', function () {
            var dis = $(this)
            SingleCalendar({
                Click: function (e) {
                    dis.val(e);
                },
                Close: function () {
                    $('#popRMA').modal('show');
                }
            })
        })
        .on('click', '#btnSaveRMA', function () {
            SaveRMA();
        })
        .on('click', '#btnAddRMAItems', function () {
            $('#popRMAItems').modal('show');
            ClearRMAItemFields();
            $('#txtRMAItemsQty').focus();
            isItemEdit = false;
        })
        .on('click', '#btnOkItem', function () {
            var txtRMAItemsQty = $.trim($('#txtRMAItemsQty').val());
            var txtRMAItemsUnit = $.trim($('#txtRMAItemsUnit').val());
            var txtRMAItemsSerialNo = $.trim($('#txtRMAItemsSerialNo').val());
            var txtRMAItemDescription = $.trim($('#txtRMAItemDescription').val());
            var txtRMAItemsInvoiceNo = $.trim($('#txtRMAItemsInvoiceNo').val());
            var txtRMAItemRemark = $.trim($('#txtRMAItemRemark').val());

            if (txtRMAItemsQty == '') {
                WebToast('Please enter quantity.', 'top');
                return;
            }

            if (txtRMAItemsUnit == '') {
                WebToast('Please enter unit.', 'top');
                return;
            }

            if (txtRMAItemDescription == '') {
                WebToast('Please enter description.', 'top');
                return;
            }

            if (txtRMAItemsSerialNo == '') {
                WebToast('Please enter serial number.', 'top');
                return;
            }

            if (isItemEdit == false) {
                arrItems.push({
                    ID: '',
                    Qty: txtRMAItemsQty,
                    Unit: txtRMAItemsUnit,
                    Description: txtRMAItemDescription,
                    SerialNo: txtRMAItemsSerialNo,
                    InvoiceRefNo: txtRMAItemsInvoiceNo,
                    Remark: txtRMAItemRemark,
                    State: itemState
                })
            } else {
                arrItems[CInt2(curItemIndex)].Qty = txtRMAItemsQty;
                arrItems[CInt2(curItemIndex)].Unit = txtRMAItemsUnit;
                arrItems[CInt2(curItemIndex)].Description = txtRMAItemDescription;
                arrItems[CInt2(curItemIndex)].SerialNo = txtRMAItemsSerialNo;
                arrItems[CInt2(curItemIndex)].InvoiceRefNo = txtRMAItemsInvoiceNo;
                arrItems[CInt2(curItemIndex)].Remark = txtRMAItemRemark;
                arrItems[CInt2(curItemIndex)].State = itemState;
            }

            DisplayItemToReturn();
            $('#popRMAItems').modal('hide');
        })
        .on('click', '#btnTicketFileUpload', function () {
            $('#ticketfileUpload').click();
        })
        .on('change', '#ticketfileUpload', function (e) {
            e.preventDefault();
            var fileUpload = $(this).get(0);
            var files = fileUpload.files;
            var data = new FormData();

            for (var i = 0; i < files.length; i++) {
                data.append(files[i].ID, files[i]);
            }

            //CheckFileName(data, data)

            AjaxSend({
                url: '/CRMDashboard/UploadTicketFiles?OrgID=' + sessionStorage.OrgID + '&DateNow=' + $.format.date(new Date(), 'MMMM dd,yyyy hh:mm:ss a') + '&ticketid=' + $.trim($('#txtTicketID').val()),
                type: 'POST',
                data: data,
                async: true,
                cache: false,
                contentType: false,
                processData: false,
                success: function (vData) {
                    if (vData.Results == 'uploaded') {
                        NewMessageBoxClose();
                        WebToast('File(s) has been successfully uploaded', 'top');
                        GetTicketFiles();
                    } else {
                        NewMessageBox(vData.Results, 'Message');
                    }
                }
            });

            $(this).val('');
        })
        .on('click', '#btnDownloadClientIssueTickets', function () {
            $('#popDownloadIssueTickets').modal('show');
        })
        .on('click', '#btnDownloadDateReported', function () {
            var dis = $(this);
            PopupPeriod({
                Click: function (e) {
                    if (e.Period == '2 days ago' || e.Period == 'Yesterday' || e.Period == 'Today') {
                        dis.text(e.From);
                    } else {
                        dis.text(e.From + ' to ' + e.To);
                    }

                    $('#btnDownloadNowIssueTickets').unbind('click').click(function () {
                        $.fileDownload(MergeUrlParameters({
                            OrgID: sessionStorage.OrgID,
                            ClientID: JSON.parse(sessionStorage.ClientInfo).ID,
                            From: e.From,
                            To: e.To,
                            Filter: $('#selDownloadClientTicketStatus').val(),
                            ViewTicketFromMobiz: JSON.parse(sessionStorage.ClientInfo).ViewTicketFromMobiz
                        }, '/CRMDashboard/ExportClientIssueTickets'), {
                            prepareCallback: function () {
                                Bs.Loading('show', 'Preparing your file. Please wait ...');
                            },
                            successCallback: function () {
                                Bs.Loading('hide');
                            },
                            failCallback: function (responseHtml, url, error) {
                                var res = JSON.parse(responseHtml).d.Results;
                                NewMessageBox(res, 'Message');
                            }
                        });
                    })
                }
            })
        })
        .on('click', '#btnClientFOWST', function () {
            var clientID = CStr2(CBool(sessionStorage.AddTicketViaDashboard) ? $('#selTicketClientName').val() : JSON.parse(sessionStorage.ClientInfo).ID);

            if (clientID == '') {
                WebToast('Please select Client first', 'top');
                return;
            }

            BrowsePopup('/CRMDashboard/BrowseClientsFOWST', 'Select Client\'s FOWST', [
                {
                    AutoSearch: true,
                    DisplayText: [{ Column: 'Name' }],
                    Click: function (e) {
                        $('#radSelectFromClientFowst').prop('checked', true);
                        $('#radFreeTypeClientFowst').prop('checked', false);
                        selectedClientFowstID = e.ID;
                        $('#btnClientFOWST').text(e.Name);
                        $('#txtClientFowst').val('');
                    }
                }
            ], { ClientID: clientID })
        })
        .on('blur', '#txtClientFowst', function () {
            if ($(this).val().trim() != '') {
                $('#radSelectFromClientFowst').prop('checked', false);
                $('#radFreeTypeClientFowst').prop('checked', true);
                selectedClientFowstID = null;
                $('#btnClientFOWST').text('Search here');
            }            
        })
        .on('change', '#radNoClientFowst', function () {
            selectedClientFowstID = null;
            $('#btnClientFOWST').text('Search here');
            $('#txtClientFowst').val('');
        })
        .on('change', '#selFilterFOWSTGroup', function () {
            GetTicket();
        })
        .on('change', '[name="radTicketStatus"]', function () {
            var val = $(this).val();
            
            if (val.toLowerCase() != 'unresolved') {
                $('#divTicketStatusConfirmedBy').show();
                $('#lblTicketStatus').text(val);
            } else {
                $('#txtTicketStatusConfirmBy').val('');
                $('#divTicketStatusConfirmedBy').hide();
            }
        })

        var dtTicketFiles;
        function GetTicketFiles() {
            AjaxSend({
                url: '/CRMDashboard/GetTicketFiles',
                data: { ticketid: $.trim($('#txtTicketID').val()) },
                success: function (e) {
                    var vRows = e.Results.Rows, ctr = 0;
                    var tblTicketAttachment = $('#tblTicketAttachment');

                    if ($.fn.DataTable.isDataTable(tblTicketAttachment)) {
                        //empty the table first before destroying datatable para dili magdoble ang header
                        tblTicketAttachment.empty();
                        dtTicketFiles.destroy();
                    }

                    dtTicketFiles = tblTicketAttachment.DataTable({
                        "data": vRows,
                        "pageLength": 10,
                        "columns": [
                            {
                                "data": function (data, type, full, meta) {
                                    ctr += 1;
                                    return '<b class="ui-table-cell-label">#</b>' + ctr;
                                },
                                "width": "30px"
                            },
                            {
                                "data": function (data, type, full, meta) {
                                    return '<b class="ui-table-cell-label">File Name:</b>' + data.FileName;
                                }
                            },
                            {
                                "data": function (data, type, full, meta) {
                                    return '<b class="ui-table-cell-label">Date Uploaded:</b>' + data.DateUploaded;
                                }, width: '150px', className: 'text-center'
                            },
                            {
                                "data": function (data, type, full, meta) {
                                    return '<b class="ui-table-cell-label">Action:</b> <button data-role="none" class="btn btn-success" title="Download"><span class="bi-download"></span></button>'
                                }, width: '50px', className: 'text-center'
                            }
                        ],
                        "ordering": false,
                        "aoColumnDefs": [{ "bSortable": false, "aTargets": [0] }],
                        "responsive": true,
                        "columnDefs": [{ "targets": 1, "orderable": true }],
                        "sDom": '<"top">rt<"bottom"ip><"clear">'
                        //"stateSave": true
                    })

                    $('#tblTicketAttachment tbody').unbind('click').on('click', 'td', function () {
                        if (dtTicketFiles.row(this).data() != undefined) {
                            var data = dtTicketFiles.row(this).data();
                            if (dtTicketFiles.cell(this).index().columnVisible == 3) {
                                PromptUserAccess('Allow to download files', {
                                    OnClickOKSuccess: function () {
                                        $.fileDownload(MergeUrlParameters({
                                            TicketID: $.trim($('#txtTicketID').val()),
                                            FileName: data.FileName,
                                            OrgID: sessionStorage.OrgID
                                        }, '/CRMDashboard/DownloadTicket'), {
                                            prepareCallback: function () {
                                                Bs.Loading('show', 'Preparing your file. Please wait ...');
                                            },
                                            successCallback: function () {
                                                Bs.Loading('hide');
                                            },
                                            failCallback: function (responseHtml, url, error) {
                                                var res = JSON.parse(responseHtml).d.Results;
                                                NewMessageBox(res, 'Message');
                                            }
                                        });
                                    }
                                })
                            }
                        }
                    });
                }
            })
        }

        function DisplayItemToReturn() {
            var tbodyRMAItemsToReturn = $('#tbodyRMAItemsToReturn');
            var isRemoveAll = true;

            tbodyRMAItemsToReturn.empty()

            if(arrItems.length > 0){
                for (var i = 0; i < arrItems.length; i++){
                    arrItems[i].Index = i;

                    if (arrItems[i].State != 'remove') {
                        isRemoveAll = false;
                        tbodyRMAItemsToReturn.append($('<tr>')
                                .data('data', arrItems[i])
                                .append($('<td>')
                                    .text(arrItems[i].Qty)
                                )
                                .append($('<td>')
                                    .text(arrItems[i].Unit)
                                )
                                .append($('<td>')
                                    .text(arrItems[i].Description)
                                )
                                .append($('<td>')
                                    .text(arrItems[i].SerialNo)
                                )
                                .append($('<td>')
                                    .text(arrItems[i].InvoiceRefNo)
                                )
                                .append($('<td>')
                                    .text(arrItems[i].Remark)
                                )
                                .append($('<td>')
                                    .append($('<button data-role="none" title="Move to Bin" class="btn btn-danger">')
                                        .data('data', arrItems[i])
                                        .append($('<span class="bi-trash"></span>'))
                                        .click(function (e) {
                                            e.stopPropagation();
                                            var data = $(this).data('data');

                                            NewMessageBox('Are you sure you want to remove this record?', 'Message', [
                                                        {
                                                            Text: 'Yes',
                                                            Click: function () {
                                                                if(data.State == 'add'){
                                                                    arrItems = $.grep(arrItems, function (e) {
                                                                        return e.Index != data.Index;
                                                                    })
                                                                } else {
                                                                    arrItems[CInt2(data.Index)].State = 'remove';
                                                                }
                                                                DisplayItemToReturn();
                                                            }
                                                        }, { Text: 'No' }
                                            ])
                                        })
                                    )
                                )
                                .click(function () {
                                    var data = $(this).data('data');
                                    ClearRMAItemFields();
                                    curItemIndex = data.Index;
                                    itemState = data.State;
                                    isItemEdit = true;
                                    $('#txtRMAItemsQty').val(data.Qty);
                                    $('#txtRMAItemsUnit').val(data.Unit);
                                    $('#txtRMAItemDescription').val(data.Description);
                                    $('#txtRMAItemsSerialNo').val(data.SerialNo);
                                    $('#txtRMAItemsInvoiceNo').val(data.InvoiceRefNo);
                                    $('#txtRMAItemRemark').val(data.Remark);
                                    $('#popRMAItems').modal('show');
                                })
                            )
                    }
                }
            }

            if (isRemoveAll) {
                tbodyRMAItemsToReturn.append('<tr><td colspan="7" style="text-align:center;">No Result(s) found</td></tr>')
            }
        }

        function ClearRMAItemFields() {
            $('#txtRMAItemsQty').val('');
            $('#txtRMAItemsUnit').val('');
            $('#txtRMAItemDescription').val('');
            $('#txtRMAItemsSerialNo').val('');
            $('#txtRMAItemsInvoiceNo').val('');
            $('#txtRMAItemRemark').val('');
        }

        function SaveRMA() {
            var txtRMANo = $.trim($('#txtRMANo').val());
            var txtRMAReturnTo = $.trim($('#txtRMAReturnTo').val());
            var txtRMADate = $.trim($('#txtRMADate').val());
            var txtRMALogisticCompany = $.trim($('#txtRMALogisticCompany').val());
            var txtRMAWBBlRef = $.trim($('#txtRMAWBBlRef').val());
            var txtRMAWBBlDate = $.trim($('#txtRMAWBBlDate').val());
            var selRMATransport = $.trim($('#selRMATransport').val());
            var txtRMAWBBlAmount = $.trim($('#txtRMAWBBlAmount').val());

            if (txtRMAReturnTo == "") {
                WebToast('"Return To" field is required', 'top');
                return
            }

            if (txtRMADate == "") {
                WebToast('"RMA Date" field is required', 'top');
                return
            }

            var pData = {
                ID: rmaID,
                RMANumber: txtRMANo,
                ReturnTo: txtRMAReturnTo,
                RMADate: txtRMADate,
                LogisticCompany: txtRMALogisticCompany,
                WBBLRef: txtRMAWBBlRef,
                WBBLDate: txtRMAWBBlDate,
                ModeOfTransport: selRMATransport,
                WBBLAmount: txtRMAWBBlAmount,
                TicketID: $.trim($('#txtTicketID').val()),
                TicketNumber: currentTicketNo,
                Date: new Date(),
                Items: arrItems
            }
            $('#btnSaveRMA').prop('disabled', true);
            AjaxSend({
                url: 'CRMDashboard/SaveRMA',
                data: { data: pData },
                success: function (vData) {
                    var res = (vData.Results).split('||');
                    $('#btnSaveRMA').removeAttr('disabled');
                    $('#popRMA').modal('hide')
                    if (res[0] == "saved") {                        
                        NewMessageBox("RMA of issue ticket no. " + currentTicketNo + "  has been saved. <br><br>" +
                                "RMA No. is " + res[1], 'Message');
                        GetRMAOfTicket();
                    } else if (res[0] == "error") {
                        NewMessageBox(res[1], "Message");
                    } else if (res[0] == "updated") {
                        WebToast('Successfully updated', "top");
                        GetRMAOfTicket();
                    }
                }
            })
        }

        function GetRMAOfTicket() {
            var tbodyRMA = $('#tbodyRMA');
            tbodyRMA.empty();
            AjaxSend({
                url: 'CRMDashboard/GetRMAOfTicket',
                data: { TicketID: $.trim($('#txtTicketID').val()) },
                success: function (vData) {
                    var datas = vData.data;

                    if (datas.length > 0) {
                        for (var i = 0; i < datas.length; i++) {
                            var vRows = datas[i].Details;
                            
                            tbodyRMA.append($('<tr>')
                                    .data('items', datas[i].Items)
                                    .data('data', vRows)
                                    .append($('<td>')
                                        .text(vRows.RMANumber)
                                    )
                                    .append($('<td>')
                                        .text(vRows.ReturnTo)
                                    )
                                    .append($('<td>')
                                        .text($.format.date(new Date(parseInt((vRows.RMADate).substring(6))), 'MM-dd-yyyy'))
                                    )
                                    .append($('<td>')
                                        .text((CStr2(vRows.WBBLDate) == "" ? "" : $.format.date(new Date(parseInt((vRows.WBBLDate).substring(6))), 'MM-dd-yyyy')))
                                    )
                                    .append($('<td>')
                                        .text(vRows.NoOfItems)
                                    )
                                    .append($('<td>')
                                        .append($('<button data-role="none" title="Move to Bin" class="btn btn-danger">')
                                            .data('data', vRows)
                                            .append($('<span class="bi-trash"></span>'))
                                            .click(function (e) {
                                                e.stopPropagation();
                                                var data = $(this).data('data');

                                                NewMessageBox('Are you sure you want to remove this record?', 'Message', [
                                                            {
                                                                Text: 'Yes',
                                                                Click: function () {
                                                                    AjaxSend({
                                                                        url: 'CRMDashboard/RemoveRMA',
                                                                        data: { RMAID: (data.RelatedID || data.ID), Date: new Date() },
                                                                        success: function (e) {
                                                                            if (e.Results == "saved") {
                                                                                GetRMAOfTicket();
                                                                            } else {
                                                                                NewMessageBox(e.Results, 'Message');
                                                                            }
                                                                        }
                                                                    })
                                                                }
                                                            }, { Text: 'No' }
                                                ])
                                            })
                                        )
                                    )
                                    .click(function () {
                                        var data = $(this).data('data');

                                        ClearRMAFields();
                                        rmaID = data.ID;
                                        $('#txtRMANo').val(data.RMANumber);
                                        $('#txtRMAReturnTo').val(data.ReturnTo);
                                        $('#txtRMADate').val($.format.date(new Date(parseInt((data.RMADate).substring(6))), 'MM-dd-yyyy'));
                                        $('#txtRMALogisticCompany').val(data.LogisticCompany);
                                        $('#txtRMAWBBlRef').val(data.WBBLRef);
                                        $('#txtRMAWBBlDate').val((CStr2(data.WBBLDate) == "" ? "" : $.format.date(new Date(parseInt((data.WBBLDate).substring(6))), 'MM-dd-yyyy')));
                                        $('#selRMATransport').val(data.ModeOfTransport);
                                        $('#txtRMAWBBlAmount').val(data.WBBLAmount);

                                        arrItems = $(this).data('items');
                                        DisplayItemToReturn();
                                        GetRMAHistory(CStr2(data.RelatedID) == '' ? data.ID : data.RelatedID);

                                        $('#popRMA').modal('show');
                                        $('#tabRMADetails').opentabmenu();
                                        $('#btnTabRMAEditTrail').show();
                                    })
                                )
                        }
                    } else {
                        tbodyRMA.append('<tr><td colspan="7" style="text-align:center;">No Result(s) found</td></tr>')
                    }
                }
            })
        }

        function GetRMAHistory(id) {
            var tbodyRMAHistory = $('#tbodyRMAHistory');

            tbodyRMAHistory.empty();

            AjaxSend({
                url: 'CRMDashboard/GetRMAHistory',
                data: { RMAID: id },
                success: function (vData) {
                    var vRows = vData.Results.Rows;

                    if(vRows.length > 0){
                        for (var i = 0; i < vRows.length; i++){
                            tbodyRMAHistory.append($('<tr>')
                                    .data('data', vRows[i])
                                    .append($('<td>')
                                        .text($.trim(vRows[i].ModifiedBy))
                                    )
                                    .append($('<td>')
                                        .text($.format.date(new Date(vRows[i].LastUpdated)))
                                    )
                                    .click(function () {
                                        var data = $(this).data('data');
                                        $('#popReadOnlyRMA').modal('show');
                                        $('#lblRMAReturnTo').val(data.ReturnTo);
                                        $('#lblRMADate').val($.format.date(new Date(parseInt((data.RMADate).substring(6))), 'MM-dd-yyyy'));
                                        $('#lblRMALogisticCompany').val(data.LogisticCompany);
                                        $('#lblRMAWBBLRef').val(data.WBBLRef);
                                        $('#lblRMAWBBLDate').val((CStr2(data.WBBLDate) == '' ? '' : $.format.date(new Date(parseInt((data.WBBLDate).substring(6))), 'MM-dd-yyyy')));
                                        $('#lblRMAModeOfTransport').val(data.ModeOfTransport);
                                        $('#lblRMAWBBLAmount').val(CDec2(data.WBBLAmount));
                                    })
                                )
                        }
                    }
                }
            })
        }

        function ClearRMAFields() {
            $('#txtRMANo').val('');
            $('#txtRMAReturnTo').val('');
            $('#txtRMADate').val('');
            $('#txtRMALogisticCompany').val('');
            $('#txtRMAWBBlRef').val('');
            $('#txtRMAWBBlDate').val('');
            $('#selRMATransport').val('');
            $('#txtRMAWBBlAmount').val('');
            $('#tbodyRMAItemsToReturn').empty();

            rmaID = '';
            arrItems = [];
        }

        var tblDataTableTickets;
        function GetClientTicketSummary() {
            if ($.fn.DataTable.isDataTable('#tblDataTableTickets')) {
                tblDataTableTickets.draw();
                return
            }

            $.fn.dataTable.ext.errMode = 'none'; // hide jDatatable error popup

            tblDataTableTickets = $('#tblDataTableTickets').DataTable({
                retrieve: true,
                "order": [[1, "asc"]],
                "pageLength": 10,
                "bProcessing": true,
                "bServerSide": true,
                "ajax": {
                    url: "/CRMDashboard/GetClientTicketSummary?OrgID=" + sessionStorage.OrgID,
                    type: 'POST',
                    error: function (e) {
                        NewMessageBox('ERROR: ' + e.responseText, 'Message')
                    }
                },
                "autoWidth": false,
                "columns": [
                    {
                        "data": function (data, type, full, meta) {
                            return '<b class="ui-table-cell-label">#</b>' + data.RowNum;
                        },
                        "width": "30px"
                    },
                    {
                        "data": function (data, type, full, meta) {
                            return '<b class="ui-table-cell-label">Client Name</b>' + data.ClientName;
                        }
                    },
                    {
                        "data": function (data, type, full, meta) {
                            return '<b class="ui-table-cell-label">Unresolved</b>' + data.Unresolved;
                        }, className: 'text-center'
                    },
                    {
                        "data": function (data, type, full, meta) {
                            return '<b class="ui-table-cell-label">Unresolved</b>' + data.Resolved;
                        }, className: 'text-center'
                    },
                    {
                        "data": function (data, type, full, meta) {
                            return '<b class="ui-table-cell-label">Cancelled</b>' + data.Cancelled;
                        }, className: 'text-center'
                    },
                    {
                        "data": function (data, type, full, meta) {
                            return '<b class="ui-table-cell-label">Total</b>' + (CInt2(data.Unresolved) + CInt2(data.Resolved) + CInt2(data.Cancelled));
                        }, className: 'text-center'
                    }
                ],
                "aoColumnDefs": [{ "bSortable": false, "aTargets": [] }],
                "responsive": true,
                "columnDefs": [{ "targets": 0, "orderable": true }],
                "sDom": '<"top">rt<"bottom"ip><"clear">',
                "initComplete": function () {
                    $('#tblDataTableTickets').removeClass('dataTable');
                }
                //"stateSave": true
            })

            $('#tblDataTableTickets tbody').on('click', 'tr', function () {
                if (tblDataTableTickets.row(this).data() != undefined) {
                    var data = tblDataTableTickets.row(this).data();
                    $('[name="lblClientName"]').text(data.ClientName + '\'s users');
                    $('#divMyTickerReportSummary .nosum').text(0);
                    sessionStorage.fromServiceTicket = true;
                    sessionStorage.ClientInfo = JSON.stringify(data);

                    $('#popTicketDetails').modal('show');
                    Tickets.GetTicket();                    
                }
            });

            $("#tabCrmTicketService #txtSearchTicket").unbind().keypress(function (e) {
                if (e.keyCode == 13) {
                    tblDataTableTickets.search($.trim($(this).val())).draw();
                }
            });
        }

        function SaveTicket(isPrint) {
            var ticketClientID = $('#selTicketClientName').val();
            var txtTicketID = $.trim($('#txtTicketID').val());
            var lblTicketNumber = $.trim($('#lblTicketNumber').text());
            var selTicketType = $.trim($('#selTicketType').val());
            var txtTicketProductServices = $.trim($('#txtTicketProductServices').val());
            var txtTicketContactPersonID = $.trim($('#txtTicketContactPersonID').val());
            var txtTicketContactPerson = $.trim($('#txtTicketContactPerson').val());
            var txtTicketOthers = $.trim($('#txtTicketOthers').val());
            var txtTicketAddress = $.trim($('#txtTicketAddress').val());
            var txtTicketAssignedToID = $.trim($('#txtTicketAssignedToID').val());
            var btnTicketAssignedTo = $.trim($('#btnTicketAssignedTo').text());
            var selTicketRecommendedAction = $.trim($('#selTicketRecommendedAction').val());
            var txtTicketRecommendedActionOthers = $.trim($('#txtTicketRecommendedActionOthers').val());
            var btnTicketTimeFrom = $.trim($('#btnTicketTimeFrom').text());
            var btnTicketTimeTo = $.trim($('#btnTicketTimeTo').text());
            var btnTicketDueDate = $.trim($('#btnTicketDueDate').text());
            var txtTicketNote = $.trim($('#txtTicketNote').val());
            var days = '', pData = {};

            if (CStr2(sessionStorage.ClientInfo) != '') {
                if (CBool(JSON.parse(sessionStorage.ClientInfo).ViewTicketFromMobiz) && !CBool(dataTicketDetails.DelegateToClient)) {
                    //Kapag Tickets na gihimo sa mcr app, dili pwd makasave ang client ni mobiz, view lang.
                    return;
                }
            }            

            $('[name="radTicketDays"]:checked').each(function () {
                days += $(this).val() + ', '
            })

            if (selTicketType == "") {
                WebToast('Please select ticket type', 'top');
                return;
            }

            //if (selTicketType == 'Others') {
            //    if (txtTicketOthers == "") {
            //        WebToast('Please enter other ticket type.', 'top');
            //        return;
            //    }
            //}

            //if (selTicketRecommendedAction == 'Others') {
            //    if (txtTicketRecommendedActionOthers == "") {
            //        WebToast('Please enter other recommended action.', 'top');
            //        return;
            //    }
            //}

            //if (txtTicketAssignedToID == "") {
            //    WebToast('Please assign a person', 'top');
            //    return;
            //}

            //if (selTicketRecommendedAction == "") {
            //    WebToast('Please select recommended action', 'top');
            //    return;
            //}

            //if (btnTicketDueDate == "Set Date") {
            //    WebToast('Please set due date', 'top');
            //    return;
            //}

            if (CBool(sessionStorage.AddTicketViaDashboard)) {
                if (ticketClientID == "") {
                    WebToast('Please select client name.', 'top');
                    return;
                }
            }

            pData = {
                ID: txtTicketID,
                ClientID: (CBool(sessionStorage.AddTicketViaDashboard) ? ticketClientID : JSON.parse(sessionStorage.ClientInfo).ID),
                ProductServices: txtTicketProductServices,
                Type: selTicketType,
                OtherType: txtTicketOthers,
                ContactPerson: txtTicketContactPerson,
                ContactPersonID: txtTicketContactPersonID,
                Address: txtTicketAddress,
                Assigned: btnTicketAssignedTo,
                AssignedID: txtTicketAssignedToID,
                RecommendedAction: selTicketRecommendedAction,
                OtherRecommendedAction: txtTicketRecommendedActionOthers,
                Days: days,
                TimeFrom: (btnTicketTimeFrom.toLowerCase() == 'from' ? '' : btnTicketTimeFrom),
                TimeTo: (btnTicketTimeTo.toLowerCase() == 'to' ? '' : btnTicketTimeTo),
                DueDate: btnTicketDueDate,
                Notes: txtTicketNote,
                Date: $.format.date(new Date(), 'MMMM dd,yyyy hh:mm:ss a'),
                Status: $('[name="radTicketStatus"]:checked').val(),
                Fowst: selectedClientFowstID || $('#txtClientFowst').val(),
                StatusConfirmBy: $('#txtTicketStatusConfirmBy').val()
            }

            pData.ViewTicketFromMobiz = (CBool(sessionStorage.AddTicketViaDashboard) ? false : JSON.parse(sessionStorage.ClientInfo).ViewTicketFromMobiz);

            if (sessionStorage.OrgID.toLowerCase() == 'cfe39df6-8809-4973-9515-810df75aed12' || sessionStorage.OrgID.toLowerCase() == '84e972ed-f208-4867-9506-5b8f9572562f') {
                //para kay Mobiz lang ang option na ito                
                pData.DelegateToClient = $('#chbDelegateToMobizClient').is(':checked');
            } else {
                pData.DelegateToClient = (CBool(sessionStorage.AddTicketViaDashboard) ? false : dataTicketDetails.DelegateToClient);                
            }

            pData.DelegatedByMobiz = pData.DelegateToClient;

            if (CBool(sessionStorage.FromNote) == true) {
                if (arrNoteTicket.length > 0) { // only one ticket per note // just update the array
                    arrNoteTicket[0] = pData;
                } else {
                    arrNoteTicket.push(pData);
                }

                $('#divNoteTag').hide();

                DisplayTickets();
            } else {
                if (pData.Status != 'Unresolved') {
                    if (CBool(sessionStorage.IsTicketAlreadyClosed)) {
                        $('#popIssueTicket').modal('hide'); // we dont need to update the ticket if it is already closed.
                    } else {
                        NewMessageBox('Marking this support ticket as "' + pData.Status + '" will close this ticket. ' +
                            'You will not be able to edit this ticket after saving. ' +
                            'However, you can still add comments to this ticket in the future.<br><br> ' +
                            'Are you sure you want to mark this support ticket as ' + pData.Status + '?', 'Message',
                            [{
                                Text: 'Yes', Click: function () {
                                    ValidateTicket();
                                }
                            }, { Text: 'No' }])
                    }
                } else {
                    $('.btnSaveTicket').prop('disabled', true);
                    ValidateTicket();
                }
            }

            function ValidateTicket() {
                if (!isAddTicket) {
                    PromptUserAccess('Edit Issue Ticket', {
                        OnClickOKSuccess: function () {
                            SaveTicketNow();
                        }
                    })
                } else {
                    SaveTicketNow();
                }
            }

            function SaveTicketNow() {
                AjaxSend({
                    url: 'CRMDashboard/SaveTicket',
                    data: { data: pData },
                    contentType: "application/json",
                    success: function (vData) {
                        var result = vData.Results.split('||');
                        $('.btnSaveTicket').removeAttr('disabled');
                        if (IsGuid(result[0])) {
                            if (txtTicketID != '') {
                                WebToast('Successfully Updated', 'top');
                            } else {
                                NewMessageBox('Your issue ticket has been saved. Reference number for this issue is: ' +
                                '<br><br> <b>' + result[1] + '</b><br><br>' +
                                'Do you want to print support ticket?', 'Message', [
                                    {
                                        Text: 'Yes', Click: function () {
                                            pData.No = result[1];
                                            pData.LastUpdated = new Date();
                                            DisplayTicketDataToPrint(pData, true);
                                            WebPrint($('#divTicketToPrint').html());
                                        }
                                    }, { Text: 'No' }
                                ]);
                            }
                            $('#popIssueTicket').modal('hide');
                            if (CBool(sessionStorage.AddTicketViaDashboard)) {
                                GetClientTicketSummary();
                            } else {
                                GetTicket();
                            }                            
                        } else if (result[0] == 'same') {
                            $('#popIssueTicket').modal('hide');

                            if(CBool(isPrint)){
                                DisplayTicketDataToPrint(pData, true);
                                WebPrint($('#divTicketToPrint').html());
                            }
                        }
                        else {
                            NewMessageBox('ERROR: ' + result[0]);
                        }
                    }
                })
            }
        }

        function GenerateNumber() {
            AjaxSend({
                url: 'CRMDashboard/GenerateNumber',
                data: {Type: 'RMA'},
                success: function (vData) {
                    $('#lblTicketNumber').text(vData.Results);
                }
            })
        }

        function ClearTicket() {
            dataTicketDetails = null;
            selectedClientFowstID = null;
            $('#radNoClientFowst').prop('checked', true);
            $('#radSelectFromClientFowst').prop('checked', false);
            $('#radFreeTypeClientFowst').prop('checked', false);
            $('#btnClientFOWST').text('Search here');
            $('#txtClientFowst, #txtTicketStatusConfirmBy').val('');
            $('#txtTicketID').val('');
            $('#lblTicketNumber').text('');
            $('#txtTicketProductServices').val('');
            $('#selTicketType').val('');//.selectmenu('refresh');
            $('#txtTicketOthers').val('');
            $('#txtTicketContactPersonID').val('');
            $('#txtTicketContactPerson').val('');
            $('#txtTicketAddress').val('');
            $('#txtTicketAssignedToID').val('');
            $('#btnTicketAssignedTo').text('Search Employee');//.button('refresh');
            $('#selTicketRecommendedAction').val('');//.selectmenu('refresh');
            $('[name="radTicketDays"]:checked').each(function () {
                $(this).removeAttr('checked');//.checkboxradio('refresh')
            })
            $('#btnTicketTimeFrom').text('From');//.button('refresh');
            $('#btnTicketTimeTo').text('To');//.button('refresh');
            $('#btnTicketDueDate').text('Set Date');//.button('refresh');
            $('#txtTicketNote').val('');
            $('[name="radTicketStatus"]').removeAttr('checked');//.checkboxradio('refresh');
            $('#divTicketStatus, #divTicketOthersField, #divTicketOthersRecommendedField').hide();
            $('#chbDelegateToMobizClient').prop('checked', false);
            $('#divTicketStatusConfirmedBy').hide();
            $('#selTicketType, #selTicketRecommendedAction').removeAttr('disabled');
            $('[name="radTicketStatus"]').removeAttr('disabled');
            $('[name="radTicketDays"]').removeAttr('disabled');
            $('#txtTicketProductServices, #txtTicketContactPerson, #txtTicketAddress, #txtTicketNote').removeAttr('disabled');
            $('#btnSearchTicketContactPerson, #btnSearchTicketAddress, #btnTicketAssignedTo, #btnTicketTimeFrom, #btnTicketTimeTo, #btnTicketDueDate').removeAttr('disabled');
        }

        var dtTickets;
        function GetTicket() {
            var tbodyTickets, url, vOptions = {};
            if (!CBool(sessionStorage.fromServiceTicket)) {
                tbodyTickets = '#tblDataTableTicketsByClient';
                vOptions.Filter = $('#selFilterClientTicket').val();
                vOptions.IsFranchisee = 0;
            } else {
                tbodyTickets = '#tblDataTableTicketsByClientPopup';
                vOptions.Filter = $('#selFilterClientTicketDetails').val();
                vOptions.FowstGroupID = $('#selFilterFOWSTGroup').val();
                vOptions.IsFranchisee = $('#selFilterFOWSTGroup option:selected').attr('isfranchisee');
            }
            url = MergeUrlParameters($.extend({
                IsDatatable: true,
                OrgID: sessionStorage.OrgID,
                ClientID: JSON.parse(sessionStorage.ClientInfo).ID,
                Type: 'Details',
                ViewTicketFromMobiz: JSON.parse(sessionStorage.ClientInfo).ViewTicketFromMobiz
            }, vOptions), '/CRMDashboard/GetTickets');
            //tbodyTickets.empty().append('<tr><td colspan="10" style="text-align:center;">Loading ...</td></tr>');
            sessionStorage.AddTicketViaDashboard = false;
            MyTicketReportSummary();
            $('#divTicketClientList').hide();

            if ($.fn.DataTable.isDataTable(tbodyTickets)) {
                dtTickets.ajax.url(url).load();
                return;
            }

            dtTickets = $(tbodyTickets).DataTable({
                retrieve: true,
                "order": [[6, "desc"]],
                "pageLength": 20,
                "bProcessing": true,
                "bServerSide": true,
                "ajax": {
                    url: url,
                    type: 'POST',
                    error: function (e) {
                        NewMessageBox('ERROR: ' + e.responseText, 'Message')
                    }
                },
                "autoWidth": false,
                "columns": [
                    {
                        "data": function (data, type, full, meta) {
                            return '<b class="ui-table-cell-label">Ticket #</b>' + $.format.number(data.No, '000000');
                        },
                        "width": "30px"
                    },
                    {
                        "data": function (data, type, full, meta) {
                            return '<b class="ui-table-cell-label">Subject</b>' + data.Type;
                        }
                    },
                    {
                        "data": function (data, type, full, meta) {
                            var str = '';

                            if (CStr2(data.FowstName) != '') str += '<span class="badge bg-success" style="font-size:14px;" title="FOWST Name">F:</span> ' + CStr2(data.FowstName) + ' <br>';

                            if (CStr2(data.GroupName) != '') str += '<span class="badge bg-danger" style="font-size:14px;" title="Group Name">G:</span> ' + CStr2(data.GroupName) + ' <br>';

                            if (CStr2(data.RegionName) != '') str += '<span class="badge bg-warning text-dark" style="font-size:14px;" title="Region Name">R:</span> ' + CStr2(data.RegionName);

                            return str;
                        }
                    },
                    {
                        "data": function (data, type, full, meta) {
                            return '<b class="ui-table-cell-label">Remarks</b>' + CStr2(data.Notes);
                        }
                    },
                    {
                        "data": function (data, type, full, meta) {
                            return '<b class="ui-table-cell-label">Status</b>' + data.Status + (data.Status != 'Unresolved' ? ' <br><span style="font-size:12px;">(Confirmed by ' + (data.TicketStatusConfirmedBy || data.LastUpdatedByName) + ')</span>' : '');
                        }
                    },
                    {
                        "data": function (data, type, full, meta) {
                            if (CStr2(data.DueDate) == '') {
                                return '';
                            }
                            return '<b class="ui-table-cell-label">Due Date</b>' + $.format.date(new Date(parseInt((data.DueDate).substring(6))), 'MM-dd-yyyy');
                        }
                    },
                    {
                        "data": function (data, type, full, meta) {
                            return '<b class="ui-table-cell-label">Date Opened</b>' + $.format.date(new Date(parseInt((data.TicketOpenDate).substring(6))), 'MM-dd-yyyy');
                        }
                    },
                    {
                        "data": function (data, type, full, meta) {
                            return '<b class="ui-table-cell-label">Date Closed</b>' + (data.Status == 'Unresolved' ? '' : $.format.date(new Date(parseInt((data.LastUpdated).substring(6))), 'MM-dd-yyyy'));
                        }
                    },
                    {
                        "data": function (data, type, full, meta) {
                            return '<div style="display: -webkit-box; -webkit-line-clamp: 4; -webkit-box-orient: vertical; max-height: 100px; overflow: hidden; text-overflow: ellipsis;">' + CStr2(data.Comment) + '</div>';
                        }
                    },
                    {
                        "data": function (data, type, full, meta) {
                            return '<b class="ui-table-cell-label">Assigned To</b>' + CStr2(data.Assigned);
                        }
                    },
                    {
                        "data": function (data, type, full, meta) {
                            return '<b class="ui-table-cell-label">Encoded by</b>' + CStr2(data.OriginalCreatedBy || data.CreatedBy);
                        }
                    },
                    {
                        "data": null,
                        createdCell: function (td, cellData, rowData, row, col) {
                            var dropdown = $('<div class="dropdown" style="position:relative;">')
                                    .append($('<button data-role="none" class="btn btn-secondary btnCircle-xs dropdown-toggle" type="button" id="dd-' + rowData.ID + '" data-toggle="dropdown">')
                                        .append('<span class="bi-gear"></span>')
                                        //.click(function (e) {
                                        //    e.preventDefault();
                                        //    e.stopPropagation();

                                        //    var isOpen = $(this).parent().hasClass('open');

                                        //    if (CBool(isOpen)) {
                                        //        $(this).parent().removeClass('open');
                                        //    } else {
                                        //        $('.dropdown').removeClass('open');
                                        //        $(this).parent().addClass('open');
                                        //    }
                                        //})
                                    )

                            var options = $('<ul class="dropdown-menu dropdown-menu-right" aria-labelledby="dd-' + rowData.ID + '">')

                            options.append($('<li>')
                                                .append($('<a href="#" class="dropdown-item">').data('data', rowData).append('<span class="bi-clipboard" style="color: #f1c40f;"></span> &nbsp; Copy to plain text')
                                                    .click(function (e) {
                                                        e.preventDefault();
                                                        e.stopPropagation();
                                                        var data = JSON.parse(JSON.stringify(($(this).data('data')))), msg = '';
                                                        var txtarea = $('#txtTicketPlainText');

                                                        msg += 'Ticket No.:\t\t' + $.format.number(CInt2(data.No), '000000') + ' \n';
                                                        msg += 'Status:\t\t\t' + data.Status + (data.Status != 'Unresolved' ? ' (Confirmed by: ' + (data.TicketStatusConfirmedBy || data.LastUpdatedByName) + ')' : '') + ' \n';
                                                        msg += 'FOWST Location:\t' + (data.FowstName || 'None') + ' \n';
                                                        msg += 'Subject:\t\t\t' + data.Type + ' \n';
                                                        msg += 'Product:\t\t\t' + data.ProductServices + ' \n';
                                                        msg += 'Contact Person:\t' + (data.ContactPerson || 'None') + ' \n';

                                                        if (CStr2(data.Address) != '') msg += 'Address to service in case of visit:\t\t' + data.Address + ' \n';

                                                        msg += 'Assigned To:\t\t' + (data.Assigned || 'None') + ' \n';
                                                        msg += 'Recomm. Action:\t' + (data.OtherRecommendedAction || data.RecommendedAction) + ' \n';

                                                        if (CStr2(data.Days) != '') msg += 'Best day to service:\t\t' + data.Days + ' \n';
                                                        if (CStr2(data.TimeFrom) != '' && CStr2(data.TimeTo) != '') msg += 'Best time to service:\t\t' + $.format.date(CJsonDate(data.TimeFrom), 'hh:mm a') + ' - ' + $.format.date(CJsonDate(data.TimeTo), 'hh:mm a') + ' \n';

                                                        msg += 'Due Date:\t\t' + $.format.date(CJsonDate(data.DueDate), 'MMM-dd-yyyy') + ' \n';
                                                        msg += '\n *** REMARKS *** \n\n ' + data.Notes + ' \n';

                                                        txtarea.val(msg).select();
                                                        document.execCommand("copy");
                                                        WebToast('Copied to clipboard');
                                                    })
                                                )
                                            )
                                            .append($('<li>')
                                                .append($('<a href="#" class="dropdown-item">').data('data', rowData).append('<span class="bi-trash" style="color: #c1392b;"></span> &nbsp; Remove')
                                                    .click(function (e) {
                                                        e.preventDefault();
                                                        e.stopPropagation();
                                                        var data = $(this).data('data');
                                                        if (CBool(JSON.parse(sessionStorage.ClientInfo).ViewTicketFromMobiz)) {
                                                            NewMessageBox('Sorry! Only Mobiz can remove this ticket. Thank you!')
                                                            return;
                                                        }
                                                        PromptUserAccess('Deactivate Issue Ticket', {
                                                            OnClickOKSuccess: function () {
                                                                RemoveTicket(data)
                                                            }
                                                        })
                                                    })
                                                )
                                            )

                            dropdown.append(options)

                            $(td).css('text-align', 'right').html(dropdown);
                        },
                    }
                ],
                "aoColumnDefs": [{ "bSortable": false, "aTargets": [1, 2, 3, 8, 9, 10] }],
                "responsive": true,
                //"columnDefs": [{ "targets": [0, 1, 2, 4, 6, 7, 8], "orderable": false }],
                "sDom": '<"top">rt<"bottom"ip><"clear">',
                "initComplete": function () {
                    $(tbodyTickets).removeClass('dataTable');                    
                }
                //"stateSave": true
            })

            dtTickets.on('click', 'td', function () {
                if (dtTickets.row(this).data() != undefined) {
                    var data = dtTickets.row(this).data();
                    if (dtTickets.cell(this).index().columnVisible == 10) {                        
                        return;
                    }
                    PromptUserAccess('View Issue Ticket', {
                        OnClickOKSuccess: function () {
                            sessionStorage.IsTicketAlreadyClosed = (data.Status == 'Resolved' || data.Status == 'Cancelled' ? true : false);
                            sessionStorage.FromNote = false;
                            sessionStorage.ParentTicketID = data.ParentTicketID;
                            isAddTicket = false;
                            EditTicket(data);
                            GetTicketComments();
                            GetTicketFiles();
                            DisplayTicketDataToPrint(data, false);
                            $('#btnTabTicketComments, #btnTabTicketHistory, #btnTabTicketRMA, #btnTabTicketAttachments').showtabmenu();
                            $('#divTicketNumber').show();
                            $('#btnOKTicket').hide();
                        }
                    })
                }
            });

            $("#tabCrmTicketService #txtSearchTicket").unbind().keypress(function (e) {
                if (e.keyCode == 13) {
                    tblDataTableTickets.search($.trim($(this).val())).draw();
                }
            });
        }

        function MyTicketReportSummary() {
            AjaxSend({
                url: '/CRMDashboard/MyTicketReportSummary',
                data: {
                    ClientID: JSON.parse(sessionStorage.ClientInfo).ID,
                    ViewTicketFromMobiz: JSON.parse(sessionStorage.ClientInfo).ViewTicketFromMobiz
                }, success: function (e) {
                    $('#lblNoOfNewTicketsInLast7Days').text(CInt2(e.TotalNewTicketsInLast7Days));
                    $('#lblNoOfTickets').text(CInt2(e.TotalTickets));
                    $('#lblNoOfResolvedTicketsInLast7Days').text(CInt2(e.TotalResolvedInLast7Days));
                    $('#lblNoOfResolvedTickets').text(CInt2(e.TotalResolved));
                    $('#lblNoOfUnresolvedTicketsInLast7Days').text(CInt2(e.TotalUnresolvedInLast7Days));
                    $('#lblNoOfUnrsolvedTickets').text(CInt2(e.TotalUnresolved));
                }
            })
        }

        function RemoveTicket(data) {
            data.ActionName = "Ticket";
            NewMessageBox('Are you sure you want to remove this record?', 'Message', [
                    {
                        Text: 'Yes', Click: function () {
                            AjaxSend({
                                url: '/CRMDashboard/RemoveAction',
                                data: data,
                                success: function (vData) {
                                    if (vData.Results == "saved") {
                                        WebToast('Successfully moved to bin', 'top');
                                        GetTicket();
                                    } else {
                                        NewMessageBox(vData.Results, 'Message');
                                    }
                                }
                            })
                        }
                    }, { Text: 'No' }
            ])
        }

        function GetTicketHistory() {
            var tbodyTicketsHistory = $('#tbodyTicketsHistory');

            tbodyTicketsHistory.empty().append('<tr><td colspan="2" style="text-align:center;">Loading ...</td></tr>');

            AjaxSend({
                url: 'CRMDashboard/GetTicketsHistory',
                data: { TicketID: sessionStorage.ParentTicketID, Type: 'History' },
                success: function (vData) {
                    var vRows = vData.Results.Rows;
                    tbodyTicketsHistory.empty();
                    if (vRows.length > 0) {
                        for (var i = 0; i < vRows.length; i++) {
                            tbodyTicketsHistory.append($('<tr>')
                                .data('data', vRows[i])
                                .append($('<td>')
                                    .text(vRows[i].CreatedBy)
                                )
                                .append($('<td style="width:200px">')
                                    .text($.format.date(new Date(vRows[i].LastUpdated), 'MM-dd-yyyy hh:mm a'))
                                )
                                .click(function (e) {
                                    var data = $(this).data('data');
                                    $('#lblROTicketStatus').val(data.Status);
                                    $('#lblROTicketNumber').val(data.No);
                                    $('#lblROTicketType').val(data.Type + ' ' + (data.OtherType == "" ? '' : ' - ' + data.OtherType));
                                    $('#lblROTicketContactPerson').val(data.ContactPerson);
                                    $('#lblROTicketAddress').val(data.Address);
                                    $('#lblROTicketAssignedTo').val(data.Assigned);
                                    $('#lblROTicketRecommendedAction').val(data.RecommendedAction + ' ' + (data.OtherRecommendedAction == "" ? '' : ' - ' + data.OtherRecommendedAction));
                                    $('#lblROTicketServiceTime').val($.format.date(new Date('1-1-2016 ' + data.TimeFrom), 'hh:mm a') + ' to ' + $.format.date(new Date('1-1-2016 ' + data.TimeTo), 'hh:mm a'));
                                    $('#lblROTicketDueDate').val($.format.date(new Date(data.DueDate), 'MM-dd-yyyy'));
                                    $('#lblROTicketRemarks').val(data.Notes);
                                    $('#lblROTicketServiceDays').val(data.Days);

                                    $('#popTicketHistory').modal('show');
                                })
                            )
                        }
                    } else {
                        tbodyTicketsHistory.append($('<tr>').append($('<td colspan="2" style="text-align:center;">').text('No Result(s) Found')))
                    }
                }
            })
        }

        function GetTicketComments(id) {
            var listTicketComments = $('#listTicketComments');
            listTicketComments.empty();

            if (dataTicketDetails != null) {
                if (CBool(JSON.parse(sessionStorage.ClientInfo).ViewTicketFromMobiz) && !dataTicketDetails.DelegateToClient)
                    $('#btnTicketAddComment').hide();
            }

            AjaxSend({
                url: 'CRMDashboard/GetTicketComments',
                data: { ClientID: JSON.parse(sessionStorage.ClientInfo).ID, TicketID: sessionStorage.ParentTicketID, ViewTicketFromMobiz: CBool(JSON.parse(sessionStorage.ClientInfo).ViewTicketFromMobiz) },
                success: function (vData) {
                    var vRows = vData.data;

                    if (vRows.length > 0) {
                        for (var i = 0; i < vRows.length; i++) {
                            listTicketComments.append($('<li class="list-group-item">')
                                .append($('<div style="color:#333 !important">')
                                    .append($('<div style="font-size:12px; color:#757575; float:left;">')
                                        .html(vRows[i].ActionName)
                                    )
                                    .append($('<div style="font-size:12px; color:#757575; float:right;">')
                                        .html('By ' + vRows[i].CommentedBy + ' on ' + $.format.date(new Date(vRows[i].CommentedDate), 'MMM-dd-yyyy hh:mm a'))
                                    )
                                    .append('<br>' + HtmlDecode(vRows[i].Comment))
                                )
                            )

                            if (typeof LoadAttachments != 'undefined') LoadAttachments(vRows[i].Attachments);
                        }
                    }
                }
            })
        }

        function DisplayTickets() {
            var ul = $('#lstIssueTickets');
            var vRows = arrNoteTicket[0];
            ul.empty();

            ul.append($('<li>')
                    .append($('<div class="todoContainer" style="display:table; width:100%;">')
                    .css('cursor', 'pointer')
                    .append($('<div style="float:none; display:table-cell; width:auto; overflow:hidden; padding:8px; word-break:break-all;">')
                        .data(vRows)
                        .html('<b>Ticket Type:</b> ' + (vRows.Type == 'Others' ? vRows.OtherType : vRows.Type) + '<br><br>' +
                            '<b>Due Date:</b> ' + $.format.date(new Date(vRows.DueDate), 'MM-dd-yyyy')
                        )
                        .click(function () {
                            EditTicket($(this).data(), true)
                        })
                    )
                )
            )

            $('#popIssueTicket').modal('hide');
        }

        function EditTicket(data, isFromNote) {
            currentTicketNo = data.No;

            ClearTicket();
            selectedClientFowstID = data.FowstID;
            dataTicketDetails = data;
            $('#tabTicketDetails').opentabmenu();
            $('#txtTicketID').val(data.ID);
            $('#lblTicketNumber').text($.format.number(CInt2(data.No), '000000'));
            $('#selTicketType option:contains("' + data.Type + '")').attr('selected', 1);//.selectmenu('refresh');
            $('#txtTicketProductServices').val(data.ProductServices);
            $('#txtTicketOthers').val(data.OtherType);
            $('#txtTicketContactPersonID').val(data.ContactPersonID);
            $('#txtTicketContactPerson').val(data.ContactPerson);
            $('#txtTicketAddress').val(data.Address);
            $('#txtTicketAssignedToID').val(data.AssignedID);
            $('#btnTicketAssignedTo').text(CStr2(data.Assigned) == '' ? 'Search Employee' : data.Assigned);//.button('refresh');
            $('#selTicketRecommendedAction').val(data.RecommendedAction);//.selectmenu('refresh');
            $('#txtTicketRecommendedActionOthers').val(data.OtherRecommendedAction);
            $('[name="radTicketDays"]:checked').each(function () {
                $(this).removeAttr('checked');//.checkboxradio('refresh')
            })
            $('#btnTicketTimeFrom').text((CStr2(data.TimeFrom) == '' ? 'From' : $.format.date(new Date(parseInt((data.TimeFrom).substring(6))), 'hh:mm a')));//.button('refresh');
            $('#btnTicketTimeTo').text((CStr2(data.TimeTo) == '' ? 'To' : $.format.date(new Date(parseInt((data.TimeTo).substring(6))), 'hh:mm a')));//.button('refresh');
            if (CStr2(data.DueDate) != '') $('#btnTicketDueDate').text((IsDate(data.DueDate) ? data.DueDate : $.format.date(new Date(parseInt((data.DueDate).substring(6))), 'MM-dd-yyyy')));//.button('refresh');
            $('#txtTicketNote').val(data.Notes);
            $('[name="radTicketStatus"][value="' + data.Status + '"]').prop('checked', true);//.checkboxradio('refresh');
            $('#txtTicketStatusConfirmBy').val(data.TicketStatusConfirmedBy);
            $('#chbDelegateToMobizClient').prop('checked', CBool(data.DelegateToClient))
            $('#divTicketStatus, #btnTicketAddComment').show();

            if (CStr2(selectedClientFowstID) != '' || CStr2(data.FowstName) != '') {
                if (CStr2(selectedClientFowstID) != '') {
                    $('#btnClientFOWST').text(data.FowstName);
                    $('#radSelectFromClientFowst').prop('checked', true);
                } else {
                    $('#txtClientFowst').val(CStr2(data.FowstName));
                    $('#radFreeTypeClientFowst').prop('checked', true);
                }
            }            

            if (data.Type == 'Others') {
                $('#divTicketOthersField').show();
            }

            if (data.RecommendedAction == 'Others') {
                $('#divTicketOthersRecommendedField').show();
            }

            if (CStr2(data.Days) != '') {
                var days = (data.Days).split(',')

                for (var i = 0; i < days.length; i++) {
                    $('[name="radTicketDays"][value="' + $.trim(days[i]) + '"]').prop('checked', true);//.checkboxradio('refresh');
                }
            }

            if (data.Status != "Unresolved" || (CBool(JSON.parse(sessionStorage.ClientInfo).ViewTicketFromMobiz) && !CBool(data.DelegateToClient))) {
                $('#selTicketType, #selTicketRecommendedAction').attr('disabled', 'disabled');
                $('[name="radTicketStatus"]').attr('disabled', 'disabled');
                $('[name="radTicketDays"]').attr('disabled', 'disabled');
                $('[name="radClientFowst"], #btnClientFOWST, #txtClientFowst').attr('disabled', 'disabled');
                $('#txtTicketProductServices, #txtTicketContactPerson, #txtTicketAddress, #txtTicketNote, #txtTicketStatusConfirmBy').attr('disabled', 'disabled');
                $('#btnSearchTicketContactPerson, #btnSearchTicketAddress, #btnTicketAssignedTo, #btnTicketTimeFrom, #btnTicketTimeTo, #btnTicketDueDate').attr('disabled', 'disabled');
                $('#divTicketStatusConfirmedBy').show();
                $('#lblTicketStatus').text(data.Status);
            } else {
                $('#selTicketType, #selTicketRecommendedAction').removeAttr('disabled');
                $('[name="radTicketStatus"]').removeAttr('disabled');
                $('[name="radTicketDays"]').removeAttr('disabled');
                $('[name="radClientFowst"], #btnClientFOWST, #txtClientFowst').removeAttr('disabled');
                $('#txtTicketProductServices, #txtTicketContactPerson, #txtTicketAddress, #txtTicketNote, #txtTicketStatusConfirmBy').removeAttr('disabled');
                $('#btnSearchTicketContactPerson, #btnSearchTicketAddress, #btnTicketAssignedTo, #btnTicketTimeFrom, #btnTicketTimeTo, #btnTicketDueDate').removeAttr('disabled');
                $('#divTicketStatusConfirmedBy').hide();
                $('#lblTicketStatus').text('');
            }

            if (CBool(JSON.parse(sessionStorage.ClientInfo).ViewTicketFromMobiz) && !CBool(data.DelegateToClient)) {
                $('#lblTicketFYI').html('<b>Note:</b> This ticket was reported to Mobiz and only Mobiz can update this ticket. Thank you!');
                $('#chbDelegateToMobiz').parents().closest('.row').hide();
                $('#popIssueTicket .modal-footer').hide();
                $('#divTabTicket li:not(:eq(0), :eq(1))').hide();
            } else {
                $('#lblTicketFYI').html('');
                $('#chbDelegateToMobiz').parents().closest('.row').show();
                $('#popIssueTicket .modal-footer, #divTabTicket').show();

                if (CBool(data.DelegateToClient) && (sessionStorage.OrgID.toLowerCase() != 'cfe39df6-8809-4973-9515-810df75aed12' && sessionStorage.OrgID.toLowerCase() && '84e972ed-f208-4867-9506-5b8f9572562f')){
                    $('#divTabTicket li:not(:eq(0), :eq(1))').hide();
                } else {
                    $('#divTabTicket li:not(:eq(0), :eq(1))').show();
                }
            }

            $('#popIssueTicket').modal('show');

            if (!isFromNote) {
                GetRMAOfTicket();
            }            
        }

        function DisplayTicketDataToPrint(data, isNew) {
            $('.lblPrintTicketOrgName').text(sessionStorage.OrgName);
            $('.lblPrintTicketNumber').text($.format.number(CInt2(data.No), '000000'));
            $('.lblPrintTicketAssigned').text(data.Assigned);
            $('.lblPrintTicketType').text((data.Type == 'Others' ? data.OtherType : data.Type));
            $('.lblPrintTicketDueDate').text($.format.date(new Date((CBool(isNew) ? data.DueDate : parseInt($.trim(data.DueDate).replace('/Date(', '')))), 'MM-dd-yyyy'));
            $('.lblPrintTicketProductServices').text(data.ProductServices);
            $('.lblPrintTicketRecAction').text((data.RecommendedAction == 'Others' ? data.OtherRecommendedAction : data.RecommendedAction));
            $('.lblPrintTicketClientName').text((CBool(sessionStorage.AddTicketViaDashboard) ? $('#selTicketClientName option:selected').text() : JSON.parse(sessionStorage.ClientInfo).ClientName));
            $('.lblPrintTicketAddress').text(data.Address);
            $('.lblPrintTicketClientContact').text(data.ContactPerson);
            $('.lblPrintTicketDays').text(data.Days);
            $('.lblPrintTicketContactNumber').html((CBool(sessionStorage.AddTicketViaDashboard) ? '' : CStr2(JSON.parse(sessionStorage.ClientInfo).Telephone) + ' ' + CStr2(JSON.parse(sessionStorage.ClientInfo).MobileNos)));
            $('.lblPrintTicketTimes').text((data.TimeFrom == '00:00:00' || data.TimeTo == '00:00:00' || data.TimeFrom == 'From' || data.TimeTo == 'To' || CStr2(data.TimeFrom) == '' || CStr2(data.TimeTo) == '' ? '' : $.format.date(new Date((CBool(isNew) ? '1-1-2016 ' + data.TimeFrom : parseInt($.trim(data.TimeFrom).replace('/Date(', '')))), 'hh:ss a') + ' to ' + $.format.date(new Date((CBool(isNew) ? '1-1-2016 ' + data.TimeTo : parseInt($.trim(data.TimeTo).replace('/Date(', '')))), 'hh:ss a')));
            $('.lblPrintTicketIssuedBy').text(data.CreatedBy);
            $('.lblPrintTicketIssuedOn').text($.format.date(new Date((CBool(isNew) ? data.LastUpdated : parseInt($.trim(data.LastUpdated).replace('/Date(', '')))), 'MM-dd-yyyy'));
            $('.lblPrintTicketRemarks').html(CStr2(data.Notes).replace(/\r?\n|\r/g, '<br>'));
        }

        function GetAdditionalDataForTicket() {
            AjaxSend({
                url: 'Organization/setCreateNew', data: { IsNew: 0, ID: sessionStorage.OrgID },
                success: function (e) {
                    AjaxSend({
                        url: 'CRMDashboard/GetTicketPolicy',
                        success: function (e) {
                            if (e.Results.Rows.length > 0) {
                                $('.lblPrintTicketNotes').html((e.Results.Rows[0].Notes).replace(/\r?\n|\r/g, '<br>'));
                            } else {
                                $('.lblPrintTicketNotes').html('');
                            }
                        }
                    })

                    AjaxSend({
                        url: 'Organization/getOrganizationInfo', data: { OrganizationID: sessionStorage.OrgID },
                        success: function (e) {
                            $('.lblPrintTicketOrgAddress').html(e.OrganizationData.Rows[0].BusinessAddressBuildingNo + ' ' + e.OrganizationData.Rows[0].BusinessAddressStreet + ' ' + e.OrganizationData.Rows[0].BusinessAddressCity + ' ' + e.OrganizationData.Rows[0].BusinessAddressCountry);
                        }
                    })

                    AjaxSend({
                        url: 'Organization/getOrgContactTelList',
                        success: function (e) {
                            var orgcontactdetails = '';

                            if (e.OrganizationData.Rows.length > 0) {
                                var tel = '';
                                for (var i = 0; i < e.OrganizationData.Rows.length; i++) {
                                    tel = e.OrganizationData.Rows[i].AreaCode + '.' + e.OrganizationData.Rows[i].ContactNo;
                                }
                                orgcontactdetails = 'Tel: ' + tel;
                            }

                            AjaxSend({
                                url: 'Organization/getOrgContactMobileList',
                                success: function (e) {
                                    if (e.OrganizationData.Rows.length > 0) {
                                        var mobile = '';
                                        for (var i = 0; i < e.OrganizationData.Rows.length; i++) {
                                            mobile = '+' + e.OrganizationData.Rows[i].CountryCode + '.' + e.OrganizationData.Rows[i].Prefix + '.' + e.OrganizationData.Rows[i].ContactNo;
                                        }
                                        orgcontactdetails += ' Mobile: ' + mobile;
                                    }

                                    AjaxSend({
                                        url: 'Organization/getOrgContactEmailList',
                                        success: function (e) {
                                            if (e.OrganizationData.Rows.length > 0) {
                                                var email = '';
                                                for (var i = 0; i < e.OrganizationData.Rows.length; i++) {
                                                    email = e.OrganizationData.Rows[i].EmailAddress;
                                                }
                                                orgcontactdetails += ' Email: ' + email;
                                            }

                                            $('.lblPrintTicketOrgContacts').text(orgcontactdetails);
                                        }
                                    })
                                }
                            })
                        }
                    })

                    AjaxSend({
                        url: 'CRMDashboard/GetAllClientList', data: { OrganizationID: sessionStorage.OrgID },
                        success: function (e) {
                            var vRows = e.Results.Rows;
                            var selTicketClientName = $('#selTicketClientName');

                            selTicketClientName.empty();
                            selTicketClientName.append(new Option('Please Select', '', true))

                            for (var i = 0; i < vRows.length; i++) {
                                selTicketClientName.append(new Option(vRows[i].ClientName, vRows[i].ID))
                            }

                            selTicketClientName;//.selectmenu('refresh');
                        }
                    })
                }
            })
        }

        function IssueReporting() {
            if (sessionStorage.OrgID.toLowerCase() == 'cfe39df6-8809-4973-9515-810df75aed12' || sessionStorage.OrgID.toLowerCase() == '84e972ed-f208-4867-9506-5b8f9572562f') {
                $('#chbDelegateToMobizClient').parents().closest('.row').show();
                $('#chbDelegateToMobiz').parents().closest('.row').remove();
            } else {
                $('#chbDelegateToMobizClient').parents().closest('.row').remove();
                $('#chbDelegateToMobiz').parents().closest('.row').show();
            }
        }        
    })
})