define(['']
    , function () {
        var mDeferred = $.Deferred();
        var mHtml;

        (function staticInit() {

            AjaxSend({
                ExactUrl: '/Scripts/sources/OrganizationShort/notes/view.html?v=' + assetversion,
                type: 'GET',
                dataType: 'text',
                success: function (vData) {
                    mHtml = vData;
                    ERP.Company.main.controller = ERP.Company.main.controller()
                    mDeferred.resolve();
                }
            });
        })();

        CreateStringObject('ERP.Company.main.controller', null, function () {

            var mOut = {};
            var mElement;
            var noteid = '';

            mElement = $(mHtml);
            mOut.element = mElement;            

            mOut.Load = function (vOptions) {
                if ($('[name="divCompanyNotes"]').length == 0) {
                    $('body').append(mElement)
                }

                if (vOptions.module == 'systemsettings') {
                    $('[name="divCompanyNotes"]').appendTo('#tabCompanyNotes');
                    $('[name="divListOfNotes"]').prependTo('[name="divCompanyNotes"]');
                } else {
                    $('[name="divCompanyNotes"]').appendTo('body');
                    $('[name="divListOfNotes"]').appendTo($('#popupCRMNote .modal-body'));
                    $('#popupCRMNote').modal('show');
                }

                $('[name="txtCRMNoteSearch"]').val('');
                mOut.GetNotes(vOptions);
            }

            mOut.GetNotes = function (vOptions) {
                Bs.Loading('show');
                AjaxSend({
                    url: '/CRMDashboard/GetTakeANote',
                    data: { ClientID: vOptions.CompanyID, SearchKey: $.trim($('[name="txtCRMNoteSearch"]').val()) },
                    success: function (e) {
                        Bs.Loading('hide');
                        var vRows = e.data;
                        var tbodyCRMNotes = $('#tbodyCRMNotes');
                        tbodyCRMNotes.empty();

                        if (vRows.length > 0) {
                            for (var i = 0; i < vRows.length; i++) {
                                var vData = vRows[i];
                                tbodyCRMNotes.append($('<tr>')
                                        .append($('<td>').text((i + 1) + '.'))
                                        .append($('<td>').html(HtmlDecode(vData.Note.replace(/<\/[^>]+(>|$)/g, ""))))
                                        .append($('<td>').html(vData.UpdatedBy + '<br> on ' + $.format.date(new Date(vData.LastUpdated), 'MM-dd-yy hh:mm a')))
                                        .data('data', vData)
                                        .click(function () {
                                            var mData = $(this).data('data');
                                            noteid = mData.ID;

                                            AjaxSend({
                                                url: '/CRMDashboard/GetNote',
                                                data: { NoteID: noteid },
                                                success: function (e) {
                                                    $('#popupCRMAddANote').modal('show');
                                                    $('#txtCRMNote').empty().html(HtmlDecode(e.Results.Rows[0].Note));
                                                }
                                            })                                            
                                        })
                                    )
                            }
                        } else {
                            tbodyCRMNotes.append($('<tr>')
                                        .append($('<td class="text-center" colspan="4">').text('No results found')))
                        }

                        $('[name="btnCRMAddNote"]').unbind('click').click(function () {
                            var txtCRMNote = $('#txtCRMNote');
                            noteid = '';
                            txtCRMNote.empty();
                            $('#popupCRMAddANote').modal('show');
                        })

                        $('[name="btnSaveCRMNote"]').unbind('click').click(function () {
                            var txtCRMNote = $.trim($('#txtCRMNote').html());

                            if (txtCRMNote == '') {
                                WebToast('Please enter note', 'top');
                                return
                            }

                            AjaxSend({
                                url: 'CRMDashboard/SaveTakeANote',
                                data: { ClientID: vOptions.CompanyID, TaggedType: 'General', TaggedID: '', Ticket: [], Note: HtmlEncode(txtCRMNote), State: (noteid == '' ? 'add' : 'edit'), NoteID: noteid, Date: $.format.date(new Date(), 'MMMM dd,yyyy hh:mm a') },
                                //data: formdatas,
                                success: function (vData) {
                                    var result = vData.Results
                                    if (IsGuid(result)) {
                                        WebToast('Successfully saved', 'top');
                                        mOut.GetNotes(vOptions);
                                        $('#popupCRMAddANote').modal('hide');
                                    } else {
                                        NewMessageBox(result, 'Error');
                                    }
                                }
                            })
                        })

                        $('[name="txtCRMNoteSearch"]').unbind('keydown').keydown(function (e) {
                            if (e.which == 13) {
                                mOut.GetNotes(vOptions);
                            }
                        })
                    }
                })
            }

            return mOut;

        }); // END CreateStringObject

        return mDeferred.promise();
    })