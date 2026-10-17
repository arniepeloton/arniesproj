define([
    '/Scripts/sources/SetupChecklist/NewPromptUser.js?crc=',
    '/Scripts/sources/ERP/ContractDesigner/main/main.js?v=' + assetversion,
    //'bootstrap-tooltip'
], function () {

    var mDeferred = $.Deferred();
    var mHtml;
    var arrFieldsWithData = {};

    (function staticInit() {

        AjaxSend({
            ExactUrl: '/Scripts/sources/ERP/ContractDesigner/main/view.html?v=' + assetversion,
            type: 'GET',
            dataType: 'text',
            success: function (vData) {
                mHtml = vData;
                ERP.ContractDesigner.main.controller = ERP.ContractDesigner.main.controller()
                mDeferred.resolve();
            }
        });
    })();

    CreateStringObject('ERP.ContractDesigner.main.controller', null, function () {

        var mOut = {};
        var mElement, templateID = '', isNewContract = true, arrModuleFields, arrDesignerData;
        var arrModules = [{
            Module: 'Asset Management',
            Fields: [{ Text: 'Lessee/ Tenant/ Guest Company Name' }, { Text: 'Renter/ Lessee Company\'s Address' }, { Text: 'Lessee/ Tenant/ Guest First Name' }, { Text: 'Lessee/ Tenant/ Guest Middle Name' }, { Text: 'Lessee/ Tenant/ Guest Last Name' }, { Text: 'Lessee/ Tenant/ Guest Contact Number' },
                { Text: 'Lessee/ Tenant/ Guest Address' }, { Text: 'Lessee/ Tenant/ Guest Email Address' }, { Text: 'Name of Asset (to be) Rented/ Leased' }, { Text: 'Lease/ Rent Rate' }, { Text: 'Lease/ Rent Beginning Date' }, { Text: 'Lease/ Rent Ending Date' }]
        }, { Module: 'Purchase Order', Fields: [{ Text: 'P.O Number' }] },
            {
                Module: 'Lease Contract Universal Information',
                Fields: [{ Text: "Contract_Start_Date" },
                    { Text: "Our_Company_Name" },
                    { Text: "Our_Company_Address" },
                    { Text: "Our_Company_Mobile_Number" },
                    { Text: "Our_Company_Email" },
                    { Text: "2ndParty_Comp_Name" },
                    { Text: "2nd_Party_Head_Office_Address" },
                    { Text: "2nd_Party_Site_Address" },
                    { Text: "Rentable/Leasable_Floor_Area" },
                    { Text: "Address/Specific_Spot" },
                    { Text: "Monthly_Rental_Fee" },
                    { Text: "Monthly_Payment_Due_Day" },
                    { Text: "Min_Monthly_Sales_Quota" },
                    { Text: "Our_Company_Rep_Name" },
                    { Text: "Our_Company_Rep_Position" },
                    { Text: "2ndParty_Rep_Name" },
                    { Text: "2ndParty_Rep_Position" },
                    { Text: "Estimated_Peak_Electrical_Load" },
                    { Text: "Sub_Meter_Brand&Model" },
                    { Text: "Electrical_Tap_Location" },
                    { Text: "Our_Company_Electrical_Tap_Auth_Rep" },
                    { Text: "Our_Company_Electrical_Tap_Auth_Rep_Position" },
                    { Text: "2ndParty_Electrical_Tap_Authorizing_Rep" },
                    { Text: "2ndParty_Electrical_Tap_Authorizing_Rep_Position" },
                    { Text: "Our_Company_Rep_Signature_Date" },
                    { Text: "2ndParty_Rep_Signature_Date" },
                    { Text: "Vending_Machine_Install_Date" },
                    { Text: "Vending_Machine_Make/Model" },
                    { Text: "Serial_Number" },
                    { Text: "Machine_Dimensions" },
                    { Text: "Initial_Elec_Meter_Reading" },
                    { Text: "Our_Company_Delivery/Install_Rep" },
                    { Text: "Our_Company_Delivery/Install_Rep_Position" },
                    { Text: "Our_Company_Delivery_Date" },
                    { Text: "2ndParty_Company_Delivery_Acceptance_Rep" },
                    { Text: "2ndParty_Company_Delivery_Acceptance_Rep_Position" },
                    { Text: "2ndParty_Company_Delivery_Acceptance_Date" }
                ]
            }]

        mElement = $(mHtml);
        mOut.element = mElement;        

        $('#divErpContractDesigner').empty().append(mElement);

        LoadRequiredData();        

        $('#divContractEditor').summernote({
            //placeholder: 'Write someting here ...',
            minHeight: '300',
            toolbar: [
                ['style', ['bold', 'italic', 'underline', 'clear']],
                ['fontsize', ['fontsize']],
                ['color', ['color']],
                ['insert', ['picture']],
                ['para', ['ul', 'ol', 'paragraph']],
                ['table', ['table']]
            ],
            dialogsInBody: true
        })

        $('#btnCancelContract').click(function () {
            AddBreadCrumb({
                text: 'Contract and Letter Designer', Click: function () {
                    $('#divErpContractDesigner, #divContractDesignerList').pageshow();
                }
            }, '.ItemsBreadCrumbs')
            $('#divContractDesignerList').pageshow();
        })

        $('#btnAddContract').click(function () {
            ClearFields();
            AddBreadCrumb({
                Text: 'Designer', Click: function () {
                    $('#divNewContractDesigner').pageshow();
                }
            }, '.ItemsBreadCrumbs')
            $('#divNewContractDesigner').pageshow();
            $('#divContractEditor').summernote('focus');

            $('.note-editor [data-toggle="dropdown"]').each(function () {
                $(this).attr('data-bs-toggle', 'dropdown')
                    .removeAttr('data-toggle'); // Clean up the old attribute
            });
        })        

        $('#btnAddFields').click(function () {
            var isExist = false;

            $.grep(arrModuleFields, function (e) {
                if(e.Text == 'Free Type'){
                    isExist = true;
                }
            })
            
            if(!isExist){
                arrModuleFields.push({
                    Text: 'Free Type',
                    Click: function () {
                        var selectionRange = saveSelection();

                        $('#popContractDesignerFreeType').modal('show');
                        $('#txtContractDesignerFreeType').val('').focus();

                        $('#btnOkContractDesignerFreeType').unbind('click').click(function () {
                            $('#popContractDesignerFreeType').modal('hide');

                            restoreSelection(selectionRange);
                            var sel = window.getSelection && window.getSelection();
                            var element = document.createElement('freetype');
                            var txt = document.createTextNode('{' + $('#txtContractDesignerFreeType').val() + '}');
                            element.setAttribute('freetypeid', NewID())
                            element.appendChild(txt);
                            var range = sel.getRangeAt(0);
                            range.deleteContents();
                            range.insertNode(element);
                        })
                    }
                })
            }            

            for (var i = 0; i < arrModuleFields.length; i++) {
                if(arrModuleFields[i].Click == undefined){
                    arrModuleFields[i].Click = function (e) {
                        var sel = window.getSelection && window.getSelection();
                        var txt = document.createTextNode('{' + e.Text + '}')
                        var range = sel.getRangeAt(0);
                        range.deleteContents();
                        range.insertNode(txt);
                    }
                }                
                arrModuleFields[i].data = arrModuleFields[i];
            }

            PopupButtonList('', 'Select Field', arrModuleFields);
        })

        $('#btnSaveContract').click(function () {
            var txtContractTemplateName = $('#txtContractTemplateName');
            var selContractPaperSize = $('#selContractPaperSize');
            var selContractModule = $('#selContractModule');
            var arrFreeType = [];

            if ($.trim(txtContractTemplateName.val()) == '') {
                WebToast('Please enter template name', 'top');
                return;
            }

            $($('#divContractEditor').summernote('code')).find('[freetypeid]').each(function () {
                arrFreeType.push({
                    FreeTypeID: $(this).attr('freetypeid'),
                    Label: $(this).text()
                })
            })

            var data = {
                ID: templateID,
                TemplateName: $.trim(txtContractTemplateName.val()),
                PaperSize: '', //selContractPaperSize.val(),
                Module: selContractModule.val(),
                Content: HtmlEncode($('#divContractEditor').summernote('code')),
                IsNew: isNewContract,
                FreeType: arrFreeType
            }
            //return;
            AjaxSend({
                url: 'ContractDesigner/SaveTemplate',
                data: { data: data },
                success: function (e) {
                    if(e.Results == 'saved'){
                        GetContractDesignerTemplate();
                        WebToast('Successfully saved', 'top');
                        RemoveBreadCrumb('Designer', '.ItemsBreadCrumbs');
                        $('#divContractDesignerList').pageshow();
                    } else {
                        NewMessageBox(e.Results, 'Message');
                    }
                }
            })
        })

        $('#btnExport').click(function () {
            if(templateID == ''){

            } else {
                Contract.Export($('.note-editable').html(), {
                    'PaperSize': arrDesignerData.PaperSize,
                    'First Name': 'Arnie',
                    'Middle Name': 'Loste',
                    'Last Name': 'Peloton'
                }, JSON.parse(JSON.stringify(arrDesignerData.FreeTypeFields)))
            }            
        })

        function saveSelection() {
            if (window.getSelection) {
                var sel = window.getSelection();
                if (sel.getRangeAt && sel.rangeCount) {
                    return sel.getRangeAt(0);
                }
            } else if (document.selection && document.selection.createRange) {
                return document.selection.createRange();
            }
            return null;
        }

        function restoreSelection(range) {
            if (range) {
                if (window.getSelection) {
                    var sel = window.getSelection();
                    sel.removeAllRanges();
                    sel.addRange(range);
                } else if (document.selection && range.select) {
                    range.select();
                }
            }
        }

        function GetContractDesignerTemplate() {
            var tbodyContract = $('#tbodyContract');

            tbodyContract.empty();

            AjaxSend({
                url: 'ContractDesigner/GetContractDesignerTemplate',
                success: function (e) {
                    var vRows = e.Results;

                    if(vRows.length > 0){
                        for (var i = 0; i < vRows.length; i++){
                            tbodyContract.append($('<tr>')
                                    .data('data', vRows[i])
                                    .append($('<td>').text((i + 1) + '.'))
                                    .append($('<td>').text(vRows[i].TemplateName))
                                    .append($('<td>').text(vRows[i].Module))
                                    .append($('<td>').text(vRows[i].ModifiedByName))
                                    .append($('<td>').text($.format.date(new Date(parseInt(vRows[i].LastUpdated.replace('/Date(', ''))), 'MMM dd yyyy hh:mm a'))) //$.format.date(new Date(vRows[i].LastUpdated), 'MMM dd, yyyy hh:mm a')
                                    .click(function () {
                                        arrDesignerData = $(this).data('data');
                                        ClearFields();
                                        templateID = arrDesignerData.ID;
                                        isNewContract = false;
                                        AddBreadCrumb({
                                            Text: 'Designer', Click: function () {
                                                $('#divNewContractDesigner').pageshow();
                                            }
                                        }, '.ItemsBreadCrumbs')
                                        $('#divNewContractDesigner').pageshow();
                                        $('#txtContractTemplateName').val(arrDesignerData.TemplateName);
                                        $('#selContractPaperSize').val(arrDesignerData.PaperSize);
                                        $('#selContractModule').val(arrDesignerData.Module);
                                        $('#divContractEditor').summernote('code', HtmlDecode(arrDesignerData.Contents));
                                        arrModuleFields = $('#selContractModule option:selected').data('data').Fields;
                                        $('.note-editor [data-toggle="dropdown"]').each(function () {
                                            $(this).attr('data-bs-toggle', 'dropdown')
                                                .removeAttr('data-toggle'); // Clean up the old attribute
                                        });
                                    })
                                )
                        }
                    } else {
                        tbodyContract.append($('<tr>')
                                .append($('<td colspan="5" style="text-align:center;">')
                                    .text('No result(s) found')
                                )
                            )
                    }
                }
            })
        }

        function PopulateModules() {
            var selContractModule = $('#selContractModule');
            var lastSelectedModule;
            selContractModule.empty();

            for (var i = 0; i < arrModules.length; i++){
                selContractModule.append($('<option>')
                        .data('data', arrModules[i])
                        .text(arrModules[i].Module)                        
                    )
            }

            selContractModule.unbind('change').change(function () {
                var data = $(this).find('option:selected').data('data');

                NewMessageBox('Changing the applicable module for this document will result in the removal of the auto text fields that you may have inserted into the document body.' +
                    '<br><br>Are you sure you want to change the module where this document will be applied to?', 'Message', [
                        {
                            Text: 'Yes, change the module where this document will be applied to. This will clear/ remove any previously inserted auto text field(s)',
                            Click: function () {
                                arrModuleFields = data.Fields;
                                var content = $('#divContractEditor').summernote('code');
                                var arrAutoField = content.match(/\{([^}]+)\}/g);

                                if(arrAutoField.length > 0){ // remove auto textfields
                                    for (var i = 0; i < arrAutoField.length; i++){
                                        content = content.replace(arrAutoField[i], "");
                                    }
                                    $('#divContractEditor').summernote('reset');
                                    $('#divContractEditor').summernote('code', content);
                                }
                            }
                        }, {
                            Text: 'Oops, never mind!  Continue editing this document and do not change the applicable module.  Do not remove the auto text fields that have been inserted into the document',
                            Click: function () {
                                lastSelectedModule.attr('selected', true);
                                arrModuleFields = lastSelectedModule.data('data').Fields;
                            }
                        }
                    ], { AutoDismiss: false })
            })
            lastSelectedModule = selContractModule.find('option:selected');
            arrModuleFields = arrModules[0].Fields;
        }

        function LoadRequiredData() {
            LoadDashboard();
            GetContractDesignerTemplate();
            PopulateModules();

            //var tempinterval = setInterval(function () {
            //    if (typeof AddBreadCrumb != 'undefined') {
            //        clearInterval(tempinterval)
            //        $('.ItemsBreadCrumbs').empty();
            //        AddBreadCrumb({
            //            text: 'Main', Click: function () {
            //                $('#divContractDesignerList').pageshow();
            //            }
            //        }, '.ItemsBreadCrumbs')
            //    }
            //}, 100)
        }

        function LoadDashboard() {
            var row = $('<div class="row" style="margin-top: 20px; margin-bottom: 20px">');

            for (var i in arrModules) {
                var col = $('<div class="col-xs-12 col-sm-6 col-md-3">')
                    .data('data', arrModules[i])
                    .append($('<div class="dashboardGrid bg-light border p-3 rounded h-100">')
                        .append($('<div class="gHeader">').text(arrModules[i].Module))
                        .append($('<div class="gBody">').attr('id', 'lblCnt_' + arrModules[i].Module.replace(/\s+/g, '')).text(0))
                        .append($('<div class="gFooter">'))
                ).click(function () {
                    GetContractByModule($(this).data('data').Module);
                })

                row.append(col);
            }

            $('#tabCDDashboard').empty().append(row);
            GetContractsCount();
        }

        function GetContractByModule(module) {
            AjaxSend({
                url: 'ContractDesigner/GetContractByModule',
                data: { Module: module },
                success: function (e) {
                    var listModule = e.Results;
                    var popContractByModule = $('#popContractByModule');
                    var tblContractByModule = $('#tblContractByModule tbody').empty();
                    var ctr = 1;

                    for (var i in listModule) {
                        tblContractByModule.append($('<tr>')
                            .append('<td>' + ctr + '.</td>')
                            .append('<td>' + listModule[i].TemplateName + '</td>')
                            .append('<td class="text-center" style="width:200px;">' + $.format.date(CJsonDate(listModule[i].LastUpdated), 'MM-dd-yyyy hh:mm a') + '</td>')
                            .data('data', listModule[i])
                            .click(function () {
                                GeneratedContractByModule($(this).data('data'));
                            })
                        )

                        ctr++;
                    }

                    popContractByModule.find('.modal-title').text(module);
                    popContractByModule.modal('show');
                }
            })
        }

        function GeneratedContractByModule(data) {
            var popGeneratedContractByModule = $('#popGeneratedContractByModule');
            var cID = '';
            arrFieldsWithData = [];

            AjaxSend({
                url: 'ContractDesigner/GetGeneratedContract',
                data: { TemplateID: data.ID },
                success: function (e) {
                    var vRows = e.Results.Rows;
                    var tblGeneratedContractByModule = $('#tblGeneratedContractByModule tbody').empty();
                    var ctr = 1;                    

                    for (var i in vRows) {
                        tblGeneratedContractByModule.append($('<tr>')
                            .append('<td>' + ctr + '.</td>').css('width', '50px')
                            .append('<td>' + vRows[i].ContractName + '</td>')
                            .append('<td class="text-center" style="width:200px;">' + $.format.date(new Date(vRows[i].LastUpdated), 'MM-dd-yyyy hh:mm a') + '</td>')
                            .append($('<td>').css('width', '170px')
                                .append($('<button data-role="none" class="btn btn-sm bg-white border-warning text-warning mr-2" title="Edit">')
                                    .append('<span class="bi-pencil-fill"></span>')
                                    .data('data', vRows[i])
                                    .click(function () {
                                        var cData = $(this).data('data');
                                        cID = cData.ID;

                                        GetGeneratedContractValues(cID, function () {
                                            $('#btnCreateContract').click();
                                            $('#txtContractOwnerTemplate').val(cData.ContractName);
                                        })
                                    })
                                )
                                .append($('<button data-role="none" class="btn btn-sm bg-white border-primary text-primary">')
                                    .append('<span class="bi-stars"></span> Generate')
                                    .data('data', vRows[i])
                                    .click(function () {
                                        var cData = $(this).data('data');
                                        cID = cData.ID;

                                        GetGeneratedContractValues(cID, function () {
                                            Contract.Export(HtmlDecode(data.Contents), arrFieldsWithData, []);
                                        })
                                    })
                                )
                            )
                        )
                    }
                }
            })

            $('#btnCreateContract').unbind().click(function () {
                var tblDataFields = $('#tblDataFields tbody').empty();
                var arrFields = [...new Set(
                    data.Contents.match(/\{([^}]+)\}/g).filter(Boolean)
                )].sort();

                for (var i in arrFields) {
                    var fieldname = arrFields[i].replace(/[{}]/g, "");
                    tblDataFields.append($('<tr>')
                        .append('<td><span>' + fieldname + '</span>:</td>')
                        .append('<td><input data-role="none" type="text" class="form-control" value="' + ($.isEmptyObject(arrFieldsWithData) ? '' : arrFieldsWithData[fieldname]) + '"></td>')
                    )
                }
                $('#txtContractOwnerTemplate').val('');
                $('#popDataField').modal('show');
            })

            $('#btnSaveContractFields').unbind().click(function () {
                var arrFieldValue = [];

                $('#tblDataFields tbody tr').each(function () {
                    var dis = $(this);

                    arrFieldValue.push({
                        Field: dis.find('span').text(),
                        Value: $.trim(dis.find('input').val()),
                    })
                })

                Bs.Loading('show');
                AjaxSend({
                    url: 'ContractDesigner/SaveLeaseContractData',
                    data: { ID: cID, TemplateID: data.ID,  data: arrFieldValue, Owner: $.trim($('#txtContractOwnerTemplate').val()) },
                    success: function (e) {
                        Bs.Loading('hide');

                        $('#popDataField').modal('hide');

                        if (e.Result == 'saved') {
                            GeneratedContractByModule(data);
                        } else NewMessageBox(e.Result);
                    }
                })
            })

            popGeneratedContractByModule.find('.modal-title').text(data.TemplateName);
            popGeneratedContractByModule.modal('show');
        }

        function GetGeneratedContractValues(cID, callback) {
            AjaxSend({
                url: 'ContractDesigner/GetGeneratedContractValues',
                data: { ID: cID },
                success: function (e) {
                    var dataFields = e.Results.Rows;

                    for (var i in dataFields) {
                        arrFieldsWithData[dataFields[i].Field] = dataFields[i].Value;
                    }

                    callback(arrFieldsWithData)
                }
            })
        }

        function GetContractsCount() {
            AjaxSend({
                url: 'ContractDesigner/GetContractsCount',
                success: function (e) {
                    var cntRows = e.Result.Rows;

                    for (var i in cntRows) {
                        $('#lblCnt_' + cntRows[i].Module.replace(/\s+/g, '')).text(cntRows[i].Count);
                    }
                }
            })
        }

        function ClearFields() {
            templateID = '';
            isNewContract = true;
            $('#txtContractTemplateName').val('');
            $('#divContractEditor').summernote('reset');
            $('#divContractEditor').summernote({
                placeholder: 'Write someting here ...',
                minHeight: '300',
                toolbar: [
                    ['style', ['bold', 'italic', 'underline', 'clear']],
                    ['fontsize', ['fontsize']],
                    ['color', ['color']],
                    ['insert', ['picture']],
                    ['para', ['paragraph']],
                    ['table', ['table']]
                ],
                dialogsInBody: true
            })            
        }

        return mOut;

    }); // END CreateStringObject('App.controller.PriceChange', null, function() {

    return mDeferred.promise();

});