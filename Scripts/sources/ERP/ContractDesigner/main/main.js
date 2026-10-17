require(['common',
    //'promise!/Scripts/PrintPDF/html2canvas.js',
    //'promise!/Scripts/PrintPDF/jspdf.debug.js'
], function () {
    var cssOverLay = {
        background: 'rgba(0, 0, 0, 0.5)',
        bottom: '0',
        left: '0',
        position: 'fixed',
        right: '0',
        top: '0',
        zIndex: '1099',
        overflow: 'auto'
    }
    var page = $('body');
    var popup = $('<div id="popupDesignerFreeType" data-role="popup" data-theme="a" data-dismissible="false" data-corners="false" style="visibility:hidden; display:none;">' +
        '<a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right btnCustomPopupClose">Close</a>' +
        '<center><h3 style="margin-left:10px; margin-right:10px;">Free Type Field(s)</h3></center>' +
        '</div>').appendTo(page).trigger('create')
        , content = $('<div data-role="content" class="ui-content ui-body-d jPopupContent" style="word-wrap:break-word;"> ' +
            'Please enter following information:<br><br>' +
            '<div id="divFreeType" style="line-height:1.3">' +

            '</div>' +

            '<br><br><a data-role="none" id="btnDesignerFreeTypeOK" class="btn custom-btn-theme-b" style="white-space:normal;">Ok</a><br><br>' +
            '<a data-role="none" id="btnDesignerFreeTypeCancel" class="btn custom-btn-theme-c" style="white-space:normal;">Cancel</a>' +
        '</div>').appendTo(popup);

    window.Contract = {
        Export: function (content, dataFields, arrFreeType) {
            popup.popup();
            //var divContents = content.html();
            //var printWindow = window.open('', '_self', '');
            //printWindow.document.write(divContents);
            //printWindow.document.close();
            //printWindow.focus();
            //printWindow.print();
            //printWindow.close();

            if (arrFreeType.length > 0) {
                var ul = $('<ul class="custom-list">')

                for (var i = 0; i < arrFreeType.length; i++) {
                    var label = arrFreeType[i].Label.match(/[^{\}]+(?=})/g)[0];

                    arrFreeType[i].Label = label;

                    ul.append($('<li>')
                            .append($('<div class="form-group">')
                                .append(label)
                                .append('<br>')
                                .append($('<textarea data-role="none" class="form-control freetype">')
                                    .data('data', arrFreeType[i])
                                    .blur(function () {
                                        var data = $(this).data('data');
                                        data.Value = $(this).val()
                                    })
                                    .val(CStr2(arrFreeType[i].Value))
                                )
                            )
                        )
                }

                $('#divFreeType').empty().append(ul);
                page.append($('<div>').addClass('popupCustomOverlayTheme').css(cssOverLay));
                $('#popupDesignerFreeType').css({ visibility: 'visible', display: 'block' }).popup('open');
                $('.popupCustomOverlayTheme, #btnDesignerFreeTypeCancel').unbind('click').click(function () {
                    $('div[data-role="popup"]').popup().popup('close');
                    $('.popupCustomOverlayTheme').remove();
                })

                $('#btnDesignerFreeTypeOK').unbind('click').click(function () {
                    var hasError = false;
                    $('.freetype').each(function () {
                        if ($(this).val() == '') {
                            hasError = true;
                            $(this).parent().addClass('has-error');
                        } else {
                            $(this).parent().removeClass('has-error');
                        }
                    })

                    if (!hasError) {
                        $('#popupDesignerFreeType').popup('close');
                        $('.popupCustomOverlayTheme').remove();
                        AjaxSend({
                            url: 'ContractDesigner/SaveFreeType',
                            data: { data: arrFreeType },
                            success: function (e) {
                                if (e.Results == 'saved') {
                                    for (var i = 0; i < arrFreeType.length; i++) {
                                        var data = {}
                                        data[arrFreeType[i].Label] = arrFreeType[i].Value;
                                        $.extend(dataFields, data)
                                    }

                                    ExportToPDF(content, dataFields);
                                }
                            }
                        })                        
                    }
                })
            } else {
                ExportToPDF(content, dataFields);
            }
        },
        AddToModule: function (data, module, callback) {
            AjaxSend({
                url: 'ContractDesigner/AddToModule',
                data: {
                    data: data,
                    Module: module
                },
                success: function (e) {
                    if(e.Results == 'saved'){
                        callback();
                    } else {
                        NewMessageBox(e.Results);
                    }
                }
            })
        },
        Populate: function (container, module) {
            var con = $(container);
            var table = $('<table data-role="table" style="margin-right:5px; display:table; width:100%" class="ui-table ui-responsive table-stroke table-stripe custom-tbl-responsive table table-light table-striped table-hover">')

            con.empty()

            table.append($('<thead>')
                    .append($('<tr class="custom-theme-f">')
                        .append($('<th style="width:20px;">').text('#'))
                        .append($('<th>').text('Document Name'))
                        .append($('<th style="width:150px;">').text('Added by'))
                        .append($('<th style="width:180px;">').text('Added on'))
                        .append($('<th style="width:50px;">').text(''))
                    )
                )

            AjaxSend({
                url: 'ContractDesigner/GetByModule',
                data: {
                    Module: module
                },
                success: function (e) {
                    var vRows = e.Results;
                    var body = $('<tbody>')

                    if(vRows.length > 0){
                        for (var i = 0; i < vRows.length; i++) {
                            body.append($('<tr>')
                                    .append($('<td>').text((i + 1) + '.'))
                                    .append($('<td>').text(vRows[i].TemplateName))
                                    .append($('<td>').text(vRows[i].AddedBy))
                                    .append($('<td>').text($.format.date(new Date(parseInt(vRows[i].LastUpdated.replace('/Date(', ''))), 'MMM dd yyyy hh:mm a')))
                                    .append($('<td style="text-align:center;">')
                                        .append($('<button data-role="none" class="btn custom-btn-theme-d btnCircle-xs btn-danger">')
                                            .append($('<span class="glyphicon glyphicon-remove bi-trash">'))
                                        )
                                        .data('data', vRows[i])
                                        .click(function () {
                                            var data = $(this).data('data');
                                            
                                            NewMessageBox('Are you sure you want to remove document <b>' + data.TemplateName + '</b>?', 'Message', [
                                                {
                                                    Text: 'Yes', Click: function () {
                                                        AjaxSend({
                                                            url: 'ContractDesigner/RemoveToModule',
                                                            data: { ID: data.ID },
                                                            success: function (e) {
                                                                if (e.Results == 'saved') {
                                                                    WebToast('Successfully removed', 'top');
                                                                    Contract.Populate(container, module);
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
                        }                        
                    } else {
                        body.append($('<tr>')
                                .append($('<td colspan="6" style="text-align:center">')
                                    .text('No result(s) found')
                                )
                            )
                    }

                    table.append(body)
                    con.append(table);
                }
            })
        }
    }

    function ExportToPDF(content, dataFields) {
        //var doc = new jsPDF('p', 'mm', dataFields.PaperSize);
        var margins = {
            top: 10,
            bottom: 10,
            left: 10,
            right: 10
        };
        var str = content;
        var arrField = str.match(/\{([^}]+)\}/g);

        if(arrField != null){
            for (var i = 0; i < arrField.length; i++){
                str = str.replace(arrField[i], dataFields[arrField[i].match(/[^{\}]+(?=})/g)[0]] || '');
            }
        }        

        WebPrint(str)
        //doc.fromHTML(str, margins.left, margins.top, {
        //    'width': 188
        //}, margins);
        ////doc.save('sample-file.pdf');
        //setTimeout(function () {
        //    var data = doc.output('datauri')
        //    $('iframe').attr('src', data)
        //}, 10)
    }
})