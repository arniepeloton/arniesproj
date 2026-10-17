require(['common'],
    function () {
        $(document).ready(function () {
            var mFollowUpInterval;
            var mLog = '', arrDropdownQtyCategories = [], qtychoicegroupID = '', ctrQtyInterval = 0, qtyintervalaction = '';

            window.EcommerceSettings = {
                LoadData: LoadData
            }

            LoadData();

            $('body')
            .on('click', '#btnCancelEcommerceGeneralSettings', function () {
                window.location.hash = 'SystemAdministration';
            })
            .on('click', '#btnSaveEcommerceGeneralSettings', function () {
                if ($.trim($('#selEcommerceDefaultFOWST').val()) == '') {
                    NewMessageBox('Ecommerce default FOWST is required');
                    return;
                }


                // not required 2020_05_08__13_33_54_5003
                //if (!ValidateIPandPort($.trim($('#txtEcommerceDataSourceIP').val()))) {
                //    NewMessageBox('Data source IP address is required or invalid');
                //    return;
                //}

                //if (!ValidateIPandPort($.trim($('#txtEcommerceOrderDataTargetIP').val()))) {
                //    NewMessageBox('Order data target IP address is required or invalid');
                //    return;
                //}
                if (!ValidateOnlineStoreName())
                    return;

                var qtyinterval = [];

                $('#tbodyDropdownQtyInterval').find('tr').each(function () {
                    qtyinterval.push($(this).data('data'));
                })

                AjaxSend({
                    url: 'Ecommerce/SaveEcommerceGeneralSettings',
                    data: {
                        EcommerceIsAllowBuyersSelectFromVendor: CInt2($('#selIsAllowBuyersSelectFromVendor').is(':checked')),
                        EcommerceShowVendorName: (CBool($.trim($('#selIsAllowBuyersSelectFromVendor').is(':checked'))) ? CInt2($('#selIsVendorName').is(':checked')) : 0),
                        EcommerceDefaultFOWST: $.trim($('#selEcommerceDefaultFOWST').val()),
                        EcommerceAcceptDeliveryOrders: CInt2($('#selAcceptDeliveryOrders').is(':checked')),
                        EcommerceAllowPickupOrders: CInt2($('#selAcceptPickUpOrders').is(':checked')),
                        EcommerceCheckoutMessage: $.trim($('#txtCheckoutMessage').val()),
                        EcommerceDataSourceServerIP: $.trim($('#txtEcommerceDataSourceIP').val()),
                        EcommerceOrderDataTargetServerIP: $.trim($('#txtEcommerceOrderDataTargetIP').val()),
                        EcommerceOnlineStoreName: $.trim($('#txtEcommerceOnlineStoreName').val()),
                        EcommercePaymentMethodPaypal: CInt2($('#selPMPaypal').is(':checked')),
                        QtyIntervalsTemplate: qtyinterval
                    }, success: function (e) {
                        if (e.Error)
                            NewMessageBox(e.Error);
                        else if (e.Result == 'Success') {
                            WebToast('Ecommerce General Settings has been saved', 'top');
                            window.location.hash = 'SystemAdministration';
                        }
                    }, error: function (e) {
                        NewMessageBox(e, 'Error');
                    }
                })
            })
            .on('change', '#selIsAllowBuyersSelectFromVendor', function () {
                if ($(this).is(':checked')) {
                    $('#divForShowVendorName').show();
                } else {
                    $('#divForShowVendorName').hide();
                }
            })
            .on('click', '#butEcommerceGeneralSettingsGetData', function () {
                $('#butEcommerceGeneralSettingsGetData').button('disable');
                $('#divEcommerceGeneralSettingsGetDataLogContainer').css('height', '200px');
                AddLog('Getting data. Please wait.');
                AjaxSend({
                    url: '/EcommerceDataSync/Start',
                    NoSplash: true,
                    dataType: 'text',
                    data: {
                        ForEcommerceServer: 1
                    },
                    success: function(vData) {
                        if (vData.Error)
                            NewMessageBox(vData.Error);
                    }
                });
                mFollowUpInterval = setInterval(FollowUpDataSyncStatus, 5000);
            })
            .on('click', '#btnAddDropdownQty', function () {
                ClearDropdownQtyFields();
                AddQtyField();
                qtyintervalaction = 'add';
                $('#popupDropdownQty').modal('show');
            })
            .on('click', '#btnAddQtyChoice', function () {
                AddQtyField();
            })
            .on('click', '#btnAddQtyCategory', function () {
                BrowsePopup('LibraryPimCategories', 'Select Categories', [
                    {
                        Message: 'You can select multiple categories',
                        AutoSearch: true,
                        SelectMultiple: true,
                        DisplayText: [{ Column: 'Category' }],
                        Click: function (e) {
                            for (var i = 0; i < e.length; i++){
                                DisplayCategories(e[i]);
                            }                            
                        }, Close: function () {
                            $('#popupDropdownQty').modal('show');
                        }
                    }
                ])
            })
            .on('click', '#btnOkQtyInterval', function () {
                var isValid = false, arrQtyChoices = [], arrCat = [];
                var qty = $('#tbodyQtyInterval').find('input.txtQtyChoices');
                var categories = $('#divQtyChoicesCategories').find('table');

                if (qty.length == 0){
                    WebToast('Please enter at least one Qty choice', 'top');
                    return;
                }

                qty.each(function () {
                    var dis = $(this);

                    if (dis.val() != ''){
                        isValid = true;
                        arrQtyChoices.push({
                            ID: CStr2(dis.attr('id')),
                            Status: CStr2(dis.attr('status')),
                            Qty: dis.val(),
                            Alias: dis.parent().next().find('input.txtQtyAlias').val()
                        });
                    }
                })

                if (!isValid) {
                    WebToast('Please enter at least one Qty choice', 'top');
                    return;
                }

                if (categories.length == 0) {
                    WebToast('Please select at least one Product Category', 'top');
                    return;
                }

                categories.each(function () {
                    arrCat.push($(this).data('Category'));
                })

                var isExistInTable = false, excat = [], tr;

                if (qtyintervalaction == 'add') {
                    ctrQtyInterval += 1;
                    tr = $('#tbodyDropdownQtyInterval').find('tr');
                } else {
                    tr = $('#tbodyDropdownQtyInterval').find('tr:not([id="'+ qtychoicegroupID +'"])');
                }
                
                tr.each(function () {
                    var data = $(this).data('data');
                    if (data != undefined) {
                        // check if naay nag-exist na category sa table na bago lang giadd na wala pa sa server
                        var existedcat = data.categories.filter(element => arrCat.includes(element));
                        if (existedcat.length > 0) {
                            $.merge(excat, existedcat);
                        }
                    }
                })

                if (excat.length > 0) {
                    NewMessageBox('The following categories are already applied to other Quantity Interval template: <br><br>' + excat.join('<br>')  + '<br><br> Only one Quantity Interval template is allowed for each Product Categories', 'Message', undefined, {
                        Close: function () {
                            $('#popupDropdownQty').modal('show');
                        }
                    })
                } else {
                    DisplayQtyInterval({
                        GroupID: qtychoicegroupID,
                        Status: qtyintervalaction,
                        choices: arrQtyChoices,
                        categories: arrCat
                    });
                    $('#popupDropdownQty').modal('hide');
                }

                //AjaxSend({
                //    url: 'Ecommerce/ValidateQtyIntervalCategories',
                //    data: { Categories: arrCat },
                //    success: function (e) {
                //        if (e.Error != undefined) {
                //            NewMessageBox(e.Error, 'Error');
                //            return;
                //        }

                        
                //    }
                //})
            })

            function DisplayQtyInterval(data) {
                var txtQy = '';

                for (var i = 0; i < data.choices.length; i++) {
                    if (i != 0) {
                        txtQy += '<br>';
                    }

                    txtQy += data.choices[i].Qty + (CStr2(data.choices[i].Alias) == '' ? '' : ' (' + data.choices[i].Alias + ')');
                }

                if (qtyintervalaction == 'edit'){
                    $('#tbodyDropdownQtyInterval').find('tr').each(function(){
                        var dis = $(this);
                        if (dis.data('data').GroupID == data.GroupID){
                            dis.find('[name="tdQtyChoices"]').html(txtQy)
                            dis.find('[name="tdQtyCategories"]').text(data.categories.join(', '))
                            dis.data('data', data);
                            return false;
                        }
                    })
                } else {
                    $('#tbodyDropdownQtyInterval').append($('<tr>')
                        .data('data', data)
                        .attr('id', data.GroupID)
                        .append($('<td>').text(ctrQtyInterval + '.'))
                        .append($('<td name="tdQtyChoices">').html(txtQy))
                        .append($('<td name="tdQtyCategories">').append($('<div>').text(data.categories.join(', '))))
                        .append($('<td style="width:40px; text-align:center;">').append($('<button data-role="none" class="btn btn-danger">')
                                .append($('<span class="bi-trash">'))
                                .click(function () {
                                    var dis = $(this);                                    

                                    NewMessageBox('Are you sure you want to remove this Qty Interval template?', 'Confirmation Message', [
                                        {
                                            Text: 'Yes', Click: function(){
                                                var tr = dis.parents().closest('tr');
                                                var data = tr.data('data');

                                                if (CStr2(tr.attr('id')) == '') {
                                                    tr.remove();
                                                } else {
                                                    data.Status = 'delete';
                                                    tr.data('data', data);
                                                    tr.hide();
                                                }
                                                
                                                ctrQtyInterval -=1;
                                            }
                                        }, { Text: 'No' }
                                    ])
                                })
                            )
                        )
                        .click(function(){
                            ClearDropdownQtyFields();

                            var data = $(this).data('data');
                            var choices = data.choices;
                            qtyintervalaction = 'edit';
                            qtychoicegroupID = data.GroupID;

                            for (var i = 0; i < choices.length; i++) {
                                AddQtyField();
                                var tr = $('#tbodyQtyInterval').find('tr:last');

                                tr.find('.txtQtyChoices').attr({
                                    id: choices[i].ID,
                                    status: choices[i].Status
                                }).val($.trim(choices[i].Qty));
                                tr.find('.txtQtyAlias').val($.trim(choices[i].Alias));
                            }

                            AddQtyField();

                            for (var i = 0; i < data.categories.length; i++) {
                                DisplayCategories({Category: data.categories[i]});
                            }

                            $('#popupDropdownQty').modal('show');
                        })
                    )
                }
            }

            function DisplayCategories(data) {
                if ($('[name="tdQtyCategory"]:contains(`Italian Favorites)').length > 0) {
                    return;
                }

                $('#divQtyChoicesCategories').append($('<table style="width:100%;">')
                    .data('Category', data.Category)
                    .append($('<tr>')
                        .append($('<td name="tdQtyCategory" style="border:1px solid #CCC; padding:10px;">').text(data.Category))
                        .append($('<td style="width:40px; text-align:center;">').append($('<button data-role="none" class="btn btn-danger">')
                                .append($('<span class="bi-trash">'))
                                .click(function () {
                                    $(this).parents().closest('table').remove();
                                })
                            )
                        )
                    )
                )
            }

            function AddQtyField() {
                var field = $('#tbodyQtyInterval').append($('<tr>')
                        .append($('<td>').append($('<input type="number" data-role="none" class="form-control txtQtyChoices" placeholder="Enter Qty here" style="margin-bottom:5px;">')
                                .attr('status', 'add')
                                .keydown(function (e) {
                                    if (e.which == 13) {
                                        AddQtyField();
                                        return;
                                    }

                                    if (!NumberDotOnly($(this), e)) {
                                        return;
                                    }
                                })
                            )
                        ).append($('<td>').append($('<input type="text" data-role="none" class="form-control txtQtyAlias" placeholder="Enter Alias" style="margin-bottom:5px;">')
                                .keydown(function (e) {
                                    if (e.which == 13) {
                                        AddQtyField();
                                        return;
                                    }
                                })
                            )
                        ).append($('<td style="width:40px; text-align:center;">').append($('<button data-role="none" class="btn btn-danger">')
                                .append($('<span class="bi-trash">'))
                                .click(function () {
                                    $(this).parents().closest('tr').remove();
                                })
                            )
                        )
                    )

                field.find('input.txtQtyChoices').focus();
            }

            function ClearDropdownQtyFields() {
                $('#tbodyQtyInterval, #divQtyChoicesCategories').empty();
                arrDropdownQtyCategories = [];
                qtychoicegroupID = '';
            }

            function FollowUpDataSyncStatus() {
                AjaxSend({
                    // url: '/EcommerceDataSync/StatusText', // old
                    url: '/EcommerceDataSync/Start', // new
                    dataType: 'text',
                    data: {
                        DontStart: true
                    },
                    success: function(vData) {
                        if (vData == 'done')
                            clearInterval(mFollowUpInterval);
                        AddLog(vData);
                        if (vData == 'done')
                            mLog = '';
                    }
                });
            }

            function AddLog(vText) {
                if (mLog != '')
                    mLog += '<br />';
                mLog += HtmlEncode(vText);
                $('#divEcommerceGeneralSettingsGetDataLog').html(mLog);
            }

            function ValidateIPandPort(input) {
                var parts = input.split(":");
                var ip = parts[0].split(".");
                var isIPValid = ip.length == 4 && ip.every(function (segment) {
                    return validateNum(segment, 0, 255);
                });

                if (parts.length > 1) {
                    var port = parts[1];
                    return validateNum(port, 1, 65535) && isIPValid
                } else {
                    return isIPValid
                }
            }

            function validateNum(input, min, max) {
                var num = +input;
                return num >= min && num <= max && input === num.toString();
            }

            function LoadData() {
                AjaxSend({
                    url: 'Browser/LibraryFOWST',
                    data: { IsAll: 1 },
                    success: function (e) {
                        var vRows = e.Results.Rows;
                        var selEcommerceDefaultFOWST = $('#selEcommerceDefaultFOWST');
                        selEcommerceDefaultFOWST.empty();

                        if (vRows.length > 0) {
                            selEcommerceDefaultFOWST.append(new Option('Please select FOWST', ''));

                            for (var i = 0 ; i < vRows.length; i++){
                                selEcommerceDefaultFOWST.append(new Option(vRows[i].FowstName, vRows[i].ID));
                            }
                        } else {
                            selEcommerceDefaultFOWST.append(new Option('No FOWST found', ''));
                        }

                        //selEcommerceDefaultFOWST.selectmenu('refresh');

                        GetEcommerceGeneralSettings();
                    }
                })
            }

            function GetEcommerceGeneralSettings() {
                AjaxSend({
                    url: 'Ecommerce/GetEcommerceGeneralSettings',
                    success: function (e) {
                        $('#selIsAllowBuyersSelectFromVendor').prop('checked', CBool(e.EcommerceIsAllowBuyersSelectFromVendor));//.slider('refresh');
                        if (CBool(e.EcommerceIsAllowBuyersSelectFromVendor)) {
                            $('#selIsVendorName').prop('checked', CBool(e.EcommerceShowVendorName));//.slider('refresh');
                            $('#divForShowVendorName').show();
                        } else {
                            $('#divForShowVendorName').hide();
                        }
                        $('#selAcceptDeliveryOrders').prop('checked', CBool(e.EcommerceAcceptDeliveryOrders));//.slider('refresh');
                        $('#selAcceptPickUpOrders').prop('checked', CBool(e.EcommerceAllowPickupOrders));//.slider('refresh');
                        $('#selPMPaypal').prop('checked', CBool(e.EcommercePaymentMethodPaypal));//.slider('refresh');
                        $('#selEcommerceDefaultFOWST').val(e.EcommerceDefaultFOWST);//.selectmenu('refresh');                        
                        $('#txtEcommerceDataSourceIP').val(e.EcommerceDataSourceServerIP);
                        $('#txtEcommerceOrderDataTargetIP').val(e.EcommerceOrderDataTargetServerIP);
                        $('#txtEcommerceOnlineStoreName').val(e.EcommerceOnlineStoreName);
                        $('#txtCheckoutMessage').val(e.EcommerceCheckoutMessage);
                        DisplayStoreUrl();
                        GetQtyInterval();
                    }
                })
            }

            function GetQtyInterval(){
                ctrQtyInterval = 0;
                qtyintervalaction = '';
                $('#tbodyDropdownQtyInterval').empty();

                AjaxSend({
                    url: 'Ecommerce/GetQtyInterval',
                    success: function (e) {
                        var vRows = e.Results;

                        for (var i = 0; i < vRows.length; i++){
                            ctrQtyInterval += 1;
                            DisplayQtyInterval(vRows[i]);
                        }
                    }
                })
            }

            function DisplayStoreUrl() {
                $('#divEcommerceOnlineStoreUrl').text(location.origin + '/' + $('#txtEcommerceOnlineStoreName').val());
            }

            function ValidateOnlineStoreName() {
                var pVal = $('#txtEcommerceOnlineStoreName').val();
                var pVal2 = pVal.replace(/[^A-Za-z0-9]/gi, '');
                if (pVal != pVal2) {
                    var pMsg = 'Your online store name contains invalid characters. Valid characters are alphanumeric. All other characters (symbols, spaces, underscores, etc.) are invalid.';
                    //$('#spanEcommerceOnlineStoreNameError')
                    //    .text(pMsg)
                    //    .show();
                    NewMessageBox(pMsg, 'Please Check Your Online Store Name');
                    return false;
                }
                $('#spanEcommerceOnlineStoreNameError').hide();
                return true;
            }

            function ShowDataSyncProgress() {
                AjaxSend({
                    url: '/EcommerceDataSync/IsBusy',
                    dataType: 'text',
                    data: {},
                    success: function(vData) {
                        if (vData == '1') {
                            AddLog('Data sync is in progress.');
                            $('#divEcommerceGeneralSettingsGetDataLogContainer').css('height', '200px');
                            mFollowUpInterval = setInterval(FollowUpDataSyncStatus, 5000);
                            $('#butEcommerceGeneralSettingsGetData').button('disable');
                        }
                    }
                });
            }

            ShowDataSyncProgress();
        })
    })