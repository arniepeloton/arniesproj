require(['common'], function () {

    $(document).on("mobileinit", function () {
        $.mobile.ajaxEnabled = false;
        $.mobile.loadingMessage = false;
    });

    require(['/Content/bootstrap-5.0.0/js/bootstrap.js'], function () {
        $(document).ready(function () {
            var arrBatchDocs = [], ctrMetadata = 0, ctrCheckInDoc = 0, batchDocData = {};
            var tempinterval = setInterval(function () {
                if (typeof AddBreadCrumb != 'undefined') {
                    clearInterval(tempinterval)
                    $('#divBreadCrumbs').empty();
                    AddBreadCrumb({
                        text: 'Document Batch List',
                        Click: function () {
                            $('#pageListOfBatch').pageshow();
                        }
                    }, '#divBreadCrumbs')
                }
            }, 100)            

            $('#btnAddbatch').click(function () {
                //AddBreadCrumb({
                //    text: 'Create Document Batch',
                //    Click: function () {
                //        $('#pageNewBatchDocs').pageshow();
                //    }
                //}, '#divBreadCrumbs')
                //ClearBatchDocsFields();
                //$('#pageNewBatchDocs').pageshow();
                ctrMetadata = 0;
                
                $('#divDocMetadata').empty();
                $('#popupDocumentBatch').modal('show');
            })

            $('#btnChooseDocs').click(function () {
                ClearCheckinDocsFields();
                $('#popupCheckInDocs').modal('show');
            })

            $('#btnAddMetadata').click(function () {
                $('#divDocMetadata').append(CreateMetadataField());
            })

            $('#btnSaveBatchDocument').click(function () {
                var txtBatchNumber = $('#txtBatchNumber').val();
                var txtOffice = $('#txtOffice').val();
                var txtBatchStatus = $('#txtBatchStatus').val();
                var txtDocumentCount = $('#txtDocumentCount').val();
                var selDocumentCategory = $('#selDocumentCategory').val();
                var selDocumentType = $('#selDocumentType').val();
                var divDocBatchErrorMsg = $('#divDocBatchErrorMsg');
                var hasError = false;
                var arrMetadata = [];

                divDocBatchErrorMsg.empty().hide();

                if (selDocumentCategory == '') {
                    divDocBatchErrorMsg.append($('<div>').text('Please select Document Category'));
                    hasError = true;
                }

                if (selDocumentType == '') {
                    divDocBatchErrorMsg.append($('<div>').text('Please select Document Type'));
                    hasError = true;
                }

                $('.tblMetadata').each(function () {
                    var dis = $(this);
                    var val = dis.find('select').val();

                    if (val != '') {
                        arrMetadata.push(val);
                    }                    
                })

                if (arrMetadata.length < 3) {
                    divDocBatchErrorMsg.append($('<div>').text('Please add at least 3 metadata'));
                    hasError = true;
                }

                if (hasError) {
                    divDocBatchErrorMsg.show();
                }

                arrBatchDocs.push({
                    BatchNumber: txtBatchNumber,
                    Office: txtOffice,
                    BatchStatus: txtBatchStatus,
                    DocumentCount: txtDocumentCount,
                    DocumentCategory: selDocumentCategory,
                    DocumentType: selDocumentType,
                    Metadata: arrMetadata
                })

                DisplayBatchDocuments();
                $('#popupDocumentBatch').modal('hide');
            })

            $('#btnAddDocs').click(function () {
                var txtDocumentID = $('#txtDocumentID').val();
                var isEmpty = false, arrCheckInDoc = {}, metadatafields = [];
                
                $('.txtmetadata').each(function () {
                    var val = $.trim($(this).val());
                    var checkindoc = {}
                    if (val == '') {
                        isEmpty = true;
                    } else {
                        checkindoc[$(this).attr('metadata')] = val;
                        metadatafields.push(checkindoc);
                    }
                })

                if (isEmpty) {
                    return
                }
                arrCheckInDoc.DocumentID = txtDocumentID;
                arrCheckInDoc.Metadata = metadatafields;
                $.merge(batchDocData.Documents, [arrCheckInDoc]);
                DisplayAddedDocument(arrCheckInDoc);
                $('#popupCheckInDocs').modal('hide');
            })

            $('#btnCancelAddedDocs').click(function () {
                AddBreadCrumb({
                    text: 'Document Batch List',
                    Click: function () {
                        $('#pageListOfBatch').pageshow();
                    }
                }, '#divBreadCrumbs')
                $('#pageListOfBatch').pageshow();
            })

            $('#btnSaveAddedDocs').click(function () {
                NewMessageBox('Check-In documents has been saved', 'Message', undefined, {
                    Close: function () {
                        AddBreadCrumb({
                            text: 'Document Batch List',
                            Click: function () {
                                $('#pageListOfBatch').pageshow();
                            }
                        }, '#divBreadCrumbs')
                        $('#pageListOfBatch').pageshow();
                    }
                })
            })

            function DisplayBatchDocuments() {
                var tbodyBatchDocuments = $('#tbodyBatchDocuments');
                tbodyBatchDocuments.empty();

                for (var i = 0; i < arrBatchDocs.length; i++) {
                    tbodyBatchDocuments.append($('<tr>')
                            .data('data', arrBatchDocs[i])
                            .append($('<td>').text(i + 1 + '.'))
                            .append($('<td>').text(arrBatchDocs[i].BatchNumber))
                            .append($('<td>').text(arrBatchDocs[i].DocumentCategory))
                            .append($('<td>').text(arrBatchDocs[i].DocumentType))
                            //.append($('<td>').append($('<pre>')
                            //        .text(JSON.stringify(arrBatchDocs[i].Metadata, null, 4))
                            //    )
                            //)
                            .click(function () {
                                MenuOptions($(this).data('data'));                                
                            })
                        )
                }
            }

            function MenuOptions(data) {
                PopupButtonList('Please select an option', 'Menu', [
                    {
                        Text: 'Check-in documents into this batch', Click: function () {
                            AddBreadCrumb({
                                text: 'Check-In Documents',
                                Click: function () {
                                    $('#pageCheckInDocs').pageshow();
                                }
                            }, '#divBreadCrumbs')

                            $('#txtCDBatchNumber').val(data.BatchNumber);
                            $('#txtCDOffice').val(data.Office);
                            $('#txtCDDocCategory').val(data.DocumentCategory);
                            $('#txtCDDocType').val(data.DocumentType);
                            $('#txtCDStatus').val(data.BatchStatus);
                            $('#txtCDDocCount').val(data.DocumentCount);

                            var thCheckInDocs = $('#thCheckInDocs');
                            var divMetadataCheckInDoc = $('#divMetadataCheckInDoc');
                            batchDocData = data;
                            ctrCheckInDoc = 0;

                            divMetadataCheckInDoc.empty();
                            thCheckInDocs.empty();
                            thCheckInDocs.append($('<td>').text('#'));
                            thCheckInDocs.append($('<td>').text('Document ID'));

                            if (batchDocData.Documents == undefined) {
                                batchDocData.Documents = [];
                            }

                            for (var i = 0; i < data.Metadata.length; i++) {
                                thCheckInDocs.append($('<td id="' + data.Metadata[i].replace(' ', '') + '">').text(data.Metadata[i]));
                                divMetadataCheckInDoc.append($('<div class="mb-3">')
                                        .append($('<label for="' + i + '-' + data.Metadata[i] + '" class="form-label">')
                                            .text(data.Metadata[i] + ':')
                                        )
                                        .append($('<input data-role="none" type="text" id="' + i + '-' + data.Metadata[i] + '" class="form-control txtmetadata" metadata="' + data.Metadata[i] + '">'))
                                    )
                            }

                            $('#tbodyCheckInDocs').empty()//.append($('<tr>').append($('<td colspan="6" style="text-align:center;">').text('Empty')));

                            $('#pageCheckInDocs').pageshow();
                        }
                    },
                    {
                        Text: 'Print document batch list', Click: function () {
                            NewMessageBox('Before printing document batch list, would you like to close the batch?', 'Message', [
                                {
                                    Text: 'Yes then print document batch list', Click: function () {

                                    }
                                },
                                {
                                    Text: 'No then print document batch list', Click: function () {

                                    }
                                }
                            ])
                        }
                    },
                    {
                        Text: 'Close this batch', Click: function () {
                            
                        }
                    },
                    {
                        Text: 'Delete this batch', Click: function () {
                            
                        }
                    }
                ])
            }

            function DisplayAddedDocument(arrCheckInDoc) {
                var tbodyCheckInDocs = $('#tbodyCheckInDocs');
                //tbodyCheckInDocs.empty();

                var tr = $('<tr>')
                        .append($('<td>').text(ctrCheckInDoc + '.'))
                        .append($('<td>').text(arrCheckInDoc.DocumentID))

                for (var m = 0; m < batchDocData.Metadata.length; m++) {
                    tr.append($('<td>'))
                }                

                for (var i = 0; i < arrCheckInDoc.Metadata.length; i++) {
                    tr.find('td:eq(' + $('#thCheckInDocs').find('#' + Object.keys(arrCheckInDoc.Metadata[i])[0].replace(' ', '')).index() + ')').text(arrCheckInDoc.Metadata[i][Object.keys(arrCheckInDoc.Metadata[i])[0]])
                }

                tbodyCheckInDocs.append(tr)
            }

            function CreateMetadataField() {
                ctrMetadata += 1;
                var div = $('<div class="mb-3">').append($('<table style="width:100%;" class="tblMetadata">')
                            .append($('<tr>')
                                .append($('<td style="width:50px;">')
                                    .text(ctrMetadata + '.')
                                )
                                .append($('<td>')
                                    .append($('<select class="form-select">')
                                        .append(new Option('Please select metadata', ''))
                                        .append(new Option('Metadata 1', 'Metadata 1'))
                                        .append(new Option('Metadata 2', 'Metadata 2'))
                                        .append(new Option('Metadata 3', 'Metadata 3'))
                                        .append(new Option('Metadata 4', 'Metadata 4'))
                                        .append(new Option('Metadata 5', 'Metadata 5'))
                                        .append(new Option('Metadata 6', 'Metadata 6'))
                                    )
                                )
                            )
                        )

                return div;
            }

            function GenerateDocumentID() {
                var randomChars = 'ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789';
                var result = '';
                for (var i = 0; i < 8; i++) {
                    result += randomChars.charAt(Math.floor(Math.random() * randomChars.length));
                }
                return result;
            }

            function ClearCheckinDocsFields() {
                $('#txtDocumentID').val(GenerateDocumentID());
                $('.txtmetadata').val('');
                ctrCheckInDoc += 1
                //$('#selDocumentCategory, #selDocumentType').val('');
                //$('#divDocMetadata').empty().append(CreateMetadataField());
            }

            function ClearBatchDocsFields() {
                $('#txtBatchNumber').val('1');
                $('#tbodyCheckInDocs').empty().append($('<tr>').append($('<td colspan="10" style="text-align:center;">').text('Empty')));
            }
        })
    })
})