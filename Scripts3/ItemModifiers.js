require([
    'common',
    (IsMobile ? '/Scripts3/Offline/Web.js?crc=' : ''),
    GetPathFormat('/Scripts3/common/common.js?v=' + assetversion)
    ], function () {
        var isCreatePopUp = false;
        var page = $('body');
        var popup = $('<div id="popupItemModifier" data-role="popup" data-theme="b" data-dismissible="false" data-corners="true" style="visibility:hidden;">' +
                        '<a href="#" id="btnCloseNewPromptUser" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">Close</a>' +
                        '<center><h3 style="margin-left:10px; margin-right:10px;" id="txtProductItemModifierName"></h3></center>' +
                        '</div>')
            , content = $('<div data-role="content" class="ui-content ui-body-d divContentItemModifierPopup"> ' +
                                '<div IM-data-role="page" id="divItemModifierMenu" style="display:block">' +

//                                    '<div class="custom-grid-solo">' +
//                                        '<div class="block-a">' +
//                                            '<a id="btnIMNoModifications" data-role="none" class="btn custom-btn-theme-b">' +
//                                                '<div style="text-align:center; overflow:hidden; padding:14px;">' +
//                                                    '<img alt="No Modifications" src="/Content/images/NoModificationsIcon.png" width="50px" align="center" />' +
//                                                    '<br /> <br /> No Modification' +
//                                                '</div>' +
//                                            '</a>' +
//                                        '</div>' +
//                                    '</div>' +

                                    '<div class="custom-grid-a">' +
                                        '<div class="block-a">' +
                                            '<a id="btnIMAddOns" data-role="none" class="btn custom-btn-theme-c">' +
                                                '<div style="text-align:center; overflow:hidden; padding:14px;">' +
                                                    '<img alt="Add-Ons (Cross-Sells)" src="/Content/images/AddOnsIcon.png" width="50px" align="center" />' +
                                                    '<br /> <br /> Add-Ons (Cross-Sells)' +
                                                '</div>' +
                                            '</a>' +
                                        '</div>' +

                                        '<div class="block-b">' +
                                            '<a id="btnIMRebatesReturns" data-role="none" class="btn custom-btn-theme-c">' +
                                                '<div style="text-align:center; overflow:hidden; padding:14.5px;">' +
                                                    '<img alt="Return for Rebates" src="/Content/images/RebatesReturnsIcon.png" width="50px" align="center" />' +
                                                    '<br /> <br />Return for Rebates' +
                                                '</div>' +
                                            '</a>' +
                                        '</div>' +
                                    '</div>' +

                                    '<div class="custom-grid-a">' +
                                        '<div class="block-a">' +
                                            '<a id="btnIMReplacements" data-role="none" class="btn custom-btn-theme-c">' +
                                                '<div style="text-align:center; overflow:hidden; padding:14px;">' +
                                                    '<img alt="Replacements (Up-Sells)" src="/Content/images/ReplacementsIcon.png" width="50px" align="center" />' +
                                                    '<br /> <br /> Replacements (Up-Sells)' +
                                                '</div>' +
                                            '</a>' +
                                        '</div>' +

                                        '<div class="block-b">' +
                                            '<a id="btnIMPreferences" data-role="none" class="btn custom-btn-theme-c">' +
                                                '<div style="text-align:center; overflow:hidden; padding:14px;">' +
                                                    '<img alt="Preferences" src="/Content/images/PreferencesIcon.png" width="50px" align="center" />' +
                                                    '<br /> <br /> Preferences' +
                                                '</div>' +
                                            '</a>' +
                                        '</div>' +
                                    '</div>' +
                                '</div>' +

                                '<div IM-data-role="page" id="divItemModifierRebates" style="visibility:hidden; display: none;">' +
                                    '<a style="margin-right:10px;" data-role="none" class="btn btn-less-width btn-transparent btnItemModifierBack">' +
                                        '<span class="ui-icon ui-icon-arrow-l icon-left" style="display:inline-block;"></span>' +
                                        'Back' +
                                    '</a>' +

                                    '<div>' +
                                        '<ul id="lstOfRebates" class="custom-list custom-list-plain">' +

                                        '</ul>' +
                                    '</div>' +

                                    '<div id="divItemModifierMenuButtons" style="display: block;">' +
                                        '' +
                                        '<div class="custom-grid-a">' +
                                            '<div class="block-b">' +
                                                '<br/>' +
                                                '<a data-role="none" class="btn custom-btn-theme-c btnItemModifierCancel">Cancel</a>' +
                                            '</div>' +
                                            '<div class="block-a">' +
                                                '<br/>' +
                                                '<a data-role="none" class="btn custom-btn-theme-b" id="btnItemModifierRebatesDone">Done</a>' +
                                            '</div>' +
                                        '</div>' +
                                    '</div>' +
                                '</div>' +
                            '</div>' +
                    '</div>').appendTo(popup);

        var ProductID, ProductName, imContainer, imPopup, imPopupHeader, imPopupContent, imPopupFooter, vOptions;

        if (!proj.IsBootstrapDesign()) {
            page.append(popup);

            imContainer = $('<div id="divNewPopupItemModifier" custom-data-role="popup">')
            imPopup = $('<div class="popup" style="font-family:sans-serif;">');
            imPopupHeader = $('<div class="popup-header" style="height:70px; padding:0px; text-decoration:none; text-align:left;">');
            imPopupContent = $('<div class="popup-content" style="top:70px; overflow:hidden; padding:0px;">');
            imPopupFooter = $('<div class="popup-footer align-center">');

            imPopupHeader.append($('<div class="divTable" style="bottom: 0; top: 0; padding: 0; margin: 0;">')
                .append($('<div class="divTableRow">')
                    .append($('<div class="divTableCell txt-middle" style="font-size:20px; padding-left:10px; overflow:auto; position:relative;">')
                        .append($('<div id="txtModifierProductName" style="position:absolute; top:10px;">'))
                    )
                    .append($('<div id="divSKUQty" class="divTableCell txt-middle" style="width:40%; padding:0px 10px;">')
                        .append(AddSubTextbox({ Max: 0 }, {}))
                    )
                )
            )

            imPopupContent.append($('<div class="divTable" style="font-size:18px;">')
                    .append($('<div class="divTableRow">')
                        .append($('<div class="divTableCell" style="position:relative; width:40%; border-right:1px solid #CCC;">')
                            .append($('<div style="position: absolute; bottom: 0px; overflow: auto; top: 0; right:0; left:0; padding:10px;">')
                                .append($('<ul id="listItemModifier" class="custom-list-menu txt-left disable">'))
                            )
                        )
                        .append($('<div class="divTableCell" style="position:relative;">')
                            .append($('<div style="position: absolute; bottom: 0px; overflow-x: hidden; overflow-y:auto; top: 0; right:0; left:0; padding:10px;">')
                                .append($('<div id="lstMemberOfModifier">'))
                            )
                        )
                    )
                )

            imPopupFooter.append($('<div class="divTable">')
                    .append($('<div class="divTableRow">')
                        .append($('<div class="divTableCell">')
                            .append($('<button data-role="none" class="btn custom-btn-theme-c btn-secondary">')
                                .text('Close')
                                .click(function () {
                                    if (proj.IsBootstrapDesign()) {
                                        $('body').removeClass('modal-open'); //para mawala ang scroll sa window
                                        $('#divNewPopupItemModifier').modal('toggle');
                                    } else {
                                        $('#divNewPopupItemModifier').hide();
                                    }
                                })
                            )
                        )
                        .append($('<div class="divTableCell">')
                            .append($('<button data-role="none" class="btn custom-btn-theme-b btn-success">')
                                .text('Ok')
                                .click(function () {
                                    OnSubmitModifier();
                                })
                            )
                        )
                    )
                )
        } else {
            imContainer = $('<div id="divNewPopupItemModifier" class="modal fade" tabindex="-1">')
            imPopup = $('<div class="modal-dialog modal-lg ' + (InAndroid ? '' : 'modal-dialog-scrollable') + ' modal-fullscreen-lg-down">');
            imPopupHeader = $('<div class="modal-header">');
            imPopupContent = $('<div class="modal-body" style="overflow-x:hidden;">');
            imPopupFooter = $('<div class="modal-footer">');

            var qtyField = AddSubTextbox({ Max: 0 }, {});
            $(qtyField[0]).find('[name="txtIMSKUQuantity"]').addClass('border');
            $(qtyField[0]).find('[name="btnModifierMinus"]').addClass('border-top-right-radius-0 border-bottom-right-radius-0');
            $(qtyField[0]).find('[name="btnModifierAdd"]').addClass('border-top-left-radius-0 border-bottom-left-radius-0');

            imPopupHeader.append($('<div class="row align-items-center">')
                .append($('<div class="col-sm mb-3 mb-sm-0">')
                        .append($('<div id="txtModifierProductName">'))
                    )
                .append($('<div id="divSKUQty" class="col-sm-6 txt-middle">')
                    .append(qtyField)
                )
            )

            imPopupContent.append($('<div>')
                                .append($('<div>').text('Please select modifier:'))
                                .append($('<div id="listItemModifier" style="overflow:auto; display:block; white-space:nowrap;" class="p-3 pl-1 pr-1">'))
                                .append($('<div style="text-align:center; font-size:12px;">').text('(swipe left or right to see more)'))
                            ).append($('<div id="lstMemberOfModifier" class="mt-3">'))

            imPopupFooter.append($('<div class="row">')
                    .append($('<div class="col-6">')
                            .append($('<button data-role="none" class="btn custom-btn-theme-c btn-secondary btn-block btn-lg">')
                                .text('Close')
                                .click(function () {
                                    if (proj.IsBootstrapDesign()) {
                                        $('body').removeClass('modal-open'); //para mawala ang scroll sa window
                                        $('#divNewPopupItemModifier').modal('toggle');
                                    } else {
                                        $('#divNewPopupItemModifier').hide();
                                    }
                                })
                            )
                    )
                    .append($('<div class="col-6">')
                            .append($('<button data-role="none" class="btn custom-btn-theme-b btn-success btn-block btn-lg">')
                                .text('Ok')
                                .click(function () {
                                    OnSubmitModifier();
                                })
                            )
                    )
                )
        }        

        if (!proj.IsBootstrapDesign()) {
            page.append(imContainer.append(imPopup.append(imPopupHeader).append(imPopupContent).append(imPopupFooter)));
        } else {
            page.append(imContainer.append(imPopup.append($('<div class="modal-content">').append(imPopupHeader).append(imPopupContent).append(imPopupFooter))));
        }

        window.NewItemModifier = function (vParams) {
            var im;
            if (CBool(vParams.EditModifier)) {
                im = vParams.ProductData.ItemModifier;
                DisplayData(vParams, im);
            } else {
                if (IsMobile) {
                    if (CStr2(vParams.ProductData.CategoryName).trim() == '')
                        im = window.Business.LibraryItemModifiers.GetItemModifiers(null, vParams.ProductData.ProductID);
                    else
                        im = window.Business.LibraryItemModifiers.GetItemModifiers(vParams.ProductData.CategoryName, vParams.ProductData.ProductID);

                    DisplayData(vParams, im);
                } else {
                    GetItemModifiers(vParams);
                }                
            }
        }

        function DisplayData(vParams, im){
            var txtIMSKUQuantity = $('[name="txtIMSKUQuantity"]');
            var listItemModifier = $('#listItemModifier');
            var lstMemberOfModifier = $('#lstMemberOfModifier');
            var activateFirstMenu = 'active-menu', showFirstMenuContent = 'block';
            vOptions = JSON.parse(JSON.stringify(vParams));
            vOptions.success = vParams.success;
            vOptions.TempItemModifier = im;
            if (window.Business != null)
                vOptions.AllowDecimal = Business.LibraryPIMProduct.GetAllowDecimal(vParams.ProductData.ProductID);

            $('#txtModifierProductName').html('<span class="text-secondary">(' + vParams.ProductData.ProductNo + ')</span> <span style="font-weight:bold;">' + vParams.ProductData.ProductName + ' @ ' + AccountingFormat(vParams.ProductData.DiscountedPrice || vParams.ProductData.DefaultPrice || vParams.ProductData.UnitPrice || vParams.ProductData.RetailPrice) + '</span>');
            txtIMSKUQuantity.val('1');

            if (CBool(vParams.AllowSKUQty)) {
                $('#divSKUQty').show();
            } else {
                $('#divSKUQty').hide();
            }

            listItemModifier.empty();
            lstMemberOfModifier.empty();

            for (var i = 0; i < im.length; i++) {
                var arrOther = {};

                if (proj.IsBootstrapDesign()) {
                    listItemModifier.append($('<button id="modbtn-' + im[i].ID + '" data-gotopage="div-' + im[i].ID + '" class="btn rounded-pill ' + (CBool(im[i].IsRequired) ? 'btn-outline-danger' : 'btn-outline-success') + ' ' + (i > 0 ? 'no-hover' : 'active') + ' modifierbtn" for="' + im[i].ID + '">').text(im[i].Name)
                                        .click(function () {
                                            $('.modifierbtn').addClass('no-hover').removeClass('active');
                                            $(this).removeClass('no-hover').addClass('active');
                                            $('#' + $(this).attr('data-gotopage')).pageshow();
                                            document.getElementById($(this).attr('id')).scrollIntoView({ behavior: "smooth", block: "center", inline: "center" });
                                        })
                                    )
                                    .append('&nbsp; &nbsp;')
                } else {
                    listItemModifier.append($('<li>')
                        .data('data', im[i])
                        .append($('<a data-gotopage="div-' + im[i].ID + '" class="ui-link ' + activateFirstMenu + '">')
                            .html(im[i].Name + ' ' + (CBool(im[i].IsRequired) ? '<span class="required">*</span>' : ''))
                            .click(function () {
                                $('#' + $(this).attr('data-gotopage')).pageshow();
                            })
                        )
                    )
                }                

                lstMemberOfModifier.append($('<div id="div-' + im[i].ID + '" subpage="lstMemberOfModifier" style="display:' + showFirstMenuContent + '">'))

                if (CBool(im[i].NotAvailableThisTime)) {
                    $('#div-' + im[i].ID).html(im[i].Message);
                    activateFirstMenu = '';
                    showFirstMenuContent = 'none';
                    continue;
                }

                if (CStr2(im[i].Type) == 'Non-Inventory Affecting') {
                    if (CInt2(im[i].Max) != 0 && (im[i].Items[m] != null) && CStr2(im[i].Items[m].Type) == "checkbox") {
                        $('#div-' + im[i].ID).html('<i>Please limit selection to <b>' + im[i].Max + '</b></i>').append('<br><br>');
                    }
                    $('#div-' + im[i].ID).append($('<div style="text-align:right;">')
                            .append($('<button data-role="none" class="btn custom-btn-theme-c btn-light border rounded-1 btn-sm btn-less-width">')
                                .data('data', im[i])
                                .html('<span class="bi-eraser-fill"></span> Clear Selection')
                                .click(function (e) {
                                    var data = $(this).data('data');
                                    var cb = $('.' + data.ID).attr('checked', false);
                                    $('.other-' + data.ID).val('');
                                    if (!IsMobile) {
                                        WebToast('Selection cleared');
                                    } else {
                                        Android.Toast('Selection cleared');
                                    }

                                    if (!proj.IsBootstrapDesign()) {
                                        cb.checkboxradio('refresh');
                                    }
                                })
                            )
                        )
                } else if (CStr2(im[i].Type) == 'Inventory Affecting') {
                    if (CInt2(im[i].Min) != 0 || CInt2(im[i].Max) != 0) {
                        $('#div-' + im[i].ID)//.html('<i>Please limit selection to <b>' + im[i].Max + '</b> per ' + CStr2(vParams.ProductData.UnitMeasure) + '</i>').append('<br><br>')
                            .append($('<div class="row mb-4">')
                                .append($('<div class="col text-center">').text('Min selection: ' + (CInt2(im[i].Min) == 0 ? 1 : CInt2(im[i].Min))))
                                .append($('<div class="col text-center">').html('Max selection: ' + (CInt2(im[i].Max) == 0 ? '<span class="bi-infinity"></span>' : CInt2(im[i].Max))))
                            )
                    }
                }

                var isLabelPremiumApplied = false;

                if (im[i].Items != null)
                    for (var m = 0; m < im[i].Items.length; m++) {
                        var itemData = im[i].Items[m];
                        if (CStr2(im[i].Type) == '' || CStr2(im[i].Type) == 'Return For Rebates') {
                            //$('#div-' + im[i].ID)
                        } else if (CStr2(im[i].Type) == 'Inventory Affecting') {
                            if (CDec2(itemData.PremiumPrice) > 0 && !isLabelPremiumApplied) {
                                $('#div-' + im[i].ID).append($('<div class="text-center mb-3 text-dark">').html('<hr>'));
                                isLabelPremiumApplied = true;
                            }

                            if (m == 0) {
                                $('#div-' + im[i].ID)
                                    .append($('<div class="divmod g-3 row ' + (window.innerWidth >= 500 ? 'row-cols-2' : 'row-cols-1') + '">'))
                            }

                            var qtyField = AddSubTextbox(im[i], itemData);
                            $(qtyField[0]).find('[name="txtIMSKUQuantity"]').addClass('form-control-sm form-control-plaintext').css('width', '40px');

                            $('#div-' + im[i].ID + ' > .divmod')
                                .append($('<div class="col">')
                                    .append($('<div class="border rounded-1 p-2 h-100">')
                                        .append($('<div class="row align-items-center">')
                                            .append($('<div class="col-12 text-center mb-2">')
                                                .html('<span class="text-success">' + (itemData.Description == '' ? itemData.ProductName : itemData.Description) + '</span>')
                                            )
                                            .append($('<div class="col">').html('<span class="text-secondary" style="font-size:14px;">@ ' + (CInt2(itemData.UnitPrice) == 0 ? '0.00' : AccountingFormat($.trim(itemData.UnitPrice))) + '</span>'))
                                            .append($('<div class="col-auto">')
                                                .append(qtyField)
                                            )
                                        )
                                    )                                    
                                )
                        } else if (CStr2(im[i].Type) == 'Non-Inventory Affecting') {
                            if (itemData.Description == 'Others') {
                                arrOther.Item = im[i];
                                arrOther.ItemModifier = itemData;
                            } else {
                                AppendField(im[i], itemData);
                            }                        
                        }

                        if (!$.isEmptyObject(arrOther) && (m + 1) == im[i].Items.length) { // place the OTHER field at the bottom of the list
                            AppendField(arrOther.Item, arrOther.ItemModifier);
                        }

                        if (!proj.IsBootstrapDesign()) {
                            $('.' + im[i].ID).checkboxradio().checkboxradio('refresh');
                        }                    
                    }

                activateFirstMenu = '';
                showFirstMenuContent = 'none';
            }

            if (im.length == 0) {
                //if inactive tanan Item Modifier sa SKU, no need to show popup
                vParams.success({ItemModifier: []})
                return;
            }

            if (CBool(vParams.EditModifier)) {
                EditNewItemModifier(vParams);
            }

            if (proj.IsBootstrapDesign()) {
                setTimeout(function () {
                    $('body').addClass('modal-open'); //para mawala ang scroll sa window
                }, 1000)
                $('#divNewPopupItemModifier').modal('show');
            } else {
                $('#divNewPopupItemModifier').show();
            }
        }

        async function GetItemModifiers(vParams) {
            var im = await GetIMData(vParams.ProductData);
            DisplayData(vParams, im.Results);
        }

        function GetIMData(vData){
            Bs.Loading('show');
            return new Promise(function (resolve, reject) {
                AjaxSend({
                    url: '/ItemModifiers/GetHBItemModifiers',
                    async: true,
                    data: {
                        CategoryName: vData.CategoryName,
                        ProductID: vData.ProductID,
                        CustomerID: sessionStorage.CustomerOrganizationID,
                        ZReadDate: $.format.date(new Date(sessionStorage.ZReadDate), 'MMM dd, yyyy')
                    }, success: function(e){
                        Bs.Loading('hide');
                        resolve(e)
                    }, error: function(e){
                        Bs.Loading('hide');
                        reject({Error: e});
                    }
                })
            })
        }

        window.ItemModifier = function (vParams) {

            var vParams = $.extend(vParams);
            ProductID = vParams.ProductID;
            ProductName = vParams.ProductName;

            if (!proj.IsBootstrapDesign()) {
                popup.remove();
                popup.popup().trigger('create');
                $('#popupItemModifier').css({ visibility: 'visible', display: 'block' }).popup('open');
                RepositionPopup();
            }            

            ChangePage('#divItemModifierMenu');            
            $('#txtProductItemModifierName').text(vParams.ProductName);

            $('#btnIMNoModifications').click(function () {
                vParams.OnClickNoModification();
                ClosePopUp();
            })

            $('#btnIMRebatesReturns').click(function () {
                vParams.OnClickRebates();
            })

            $('.btnItemModifierBack').click(function () {
                ChangePage('#divItemModifierMenu');
                RepositionPopup();
            })

            $('.btnItemModifierCancel').click(function () {
                ChangePage('#divItemModifierMenu');
                ClosePopUp();
            })

            $('#btnItemModifierRebatesDone').click(function() {
                var pLi = $("#lstOfRebates li div.selectedList");
                if (pLi.length == 0) {
                    var msg = 'Please select an item before clicking "Done"';
                    if (proj.IsBootstrapDesign()) {
                        WebToast(msg, 'top');
                    } else {
                        NewMessageBox(msg, "Message");
                    }
                    return;
                }

                if ($(window).width() > proj.WideWidth && window.location.pathname.toLowerCase() == '/salesmenu/posviewall') {
                    $("#popupLoad").popup("open");
                    setTimeout(function() {
                        if (CStr2(sessionStorage.CustomerTableNo) != '') {
                            $('#lblCustomerName').text(sessionStorage.CustomerTableNo);
                            $('#selTableNo').val(sessionStorage.CustomerTableNo);
                            $('#lblLabel').html('Customer / Table No.:<br/>');
                        }
                        if (CStr2(sessionStorage.GasPumpNo) != '') {
                            $('#lblCustomerName').text(sessionStorage.GasPumpNo);
                            $('#selGasPumpNo').val(sessionStorage.GasPumpNo);
                            $('#lblLabel').html('Gas Pump No.:<br/>');
                        }
                        if (CStr2(sessionStorage.GasPumpNo) != '' && CStr2(sessionStorage.CustomerTableNo) != '') {
                            $('#lblCustomerName').text(sessionStorage.CustomerOrganizationName);
                            $('#selTableNo').val('');
                            $('#lblLabel').html('Customer:<br/>');
                        }
                        startclock('#clock');
                        proj.SaveCart();

                        var pData = pLi.parent().data('data');
                        console.log(pData);
                        //alert($("#lstOfRebates li a.selectedList").attr('id'));
                        ClosePopUp();
                        vParams.OnClickRebatesDone(pData);
                        $("#popupLoad").popup("close");
                    }, 1000);
                } else {
                    var pData = pLi.parent().data('data');
                    console.log(pData);
                    //alert($("#lstOfRebates li a.selectedList").attr('id'));
                    ClosePopUp();
                    vParams.OnClickRebatesDone(pData);
                }
            });

            page.on('click', '.btnListRebates', function () {
                $("#lstOfRebates li").find('div').removeClass('selectedList');
                $(this).addClass('selectedList');
            })

        }

        window.ItemModifier_Rebates = function () {
            ChangePage('#divItemModifierRebates');
            var pDtRebates = window.Business.LibraryItemModifiers.GetRebates(ProductID);
            
            $("#lstOfRebates").empty();

            for (var i = 0; i < pDtRebates.Rows.length; i++) {
                var pDR = pDtRebates.Rows[i];
                var pLi = $('<li><div class="btnListRebates" id="' + pDR.ProductID + '">(' + HtmlEncode(pDR.ProductNo) + ') ' + pDR.ProductName + ' (Rebate amount: ' + AccountingFormat(pDR.Amount) + ')</div></li>');
                pLi.data('data', pDR);
                $('#lstOfRebates').append(pLi);
            }
            RepositionPopup();
        }

        window.ItemModifier_Inventory = function () {

        }        

        function AppendField(item, itemData) {
            var inp;

            if (proj.IsBootstrapDesign()) {
                inp = $('<div class="form-check mb-3">')
                    .append($('<input data-role="none" class="form-check-input ' + item.ID + '" type="' + itemData.Type + '" id="' + itemData.ID + '" otherid="other-' + itemData.ID + '" min="' + item.Min + '" max="' + item.Max +'" name="nia-' + item.ID + '" value="' + itemData.Description + '">'))
                    .append($('<label style="width:100%;" class="form-check-label" for="' + itemData.ID + '">').text((itemData.OthersDescription || itemData.Description))
                        .append('<br>')                        
                        .append((itemData.Description == 'Others' ? $('<textarea data-role="none" selecttype="' + itemData.Type + '"' + (CInt2(itemData.NoOfCharacters) > 0 ? 'maxlength="' + itemData.NoOfCharacters + '"' : '') + ' class="form-control mt-3 other-' + item.ID + '" id="other-' + itemData.ID + '" style="height:70px; resize:none;">').data('data', itemData) : ''))
                        .append((CInt2(itemData.NoOfCharacters) > 0 ? '<div class="mt-3" style="font-style:italic;">Character limit is ' + itemData.NoOfCharacters + '</div>' : ''))
                    )
            } else {
                inp = $('<div class="custom-' + itemData.Type + '" style="margin: 5px 0px;">')
                    .append($('<label class="' + itemData.Type + '-only">')
                        .append($('<input type="' + itemData.Type + '" id="' + itemData.ID + '" otherid="other-' + itemData.ID + '" min="' + item.Min + '" max="' + item.Max + '" name="nia-' + item.ID + '" class="' + item.ID + '" value="' + itemData.Description + '" data-iconpos="notext" />'))
                    )
                    .append($('<div style="width:100%;">').append(itemData.Description + '<br>')
                        .append(itemData.Description == 'Others' ?
                            $('<textarea data-role="none" selecttype="' + itemData.Type + '" min="' + item.Min + '" max="' + item.Max + '" class="form-control other-' + item.ID + '" id="other-' + itemData.ID + '" style="height:70px; resize:none;">').data('data', itemData): ''
                        )
                    )                
            }

            inp.find('input').change(function (e) {
                var dis = $(this);                
                var name = dis.attr('name');
                var isCheck = dis.is(':checked');
                var max = CInt2(dis.attr('max'));
                var cntChecked = $('[name="' + name + '"]:checked').length;

                if (isCheck) {
                    if (cntChecked > max && max != 0) {
                        dis.removeAttr('checked');
                        if (!IsMobile) {
                            WebToast('Max. selection is ' + max);
                        } else {
                            Android.Toast('Max. selection is ' + max);
                        }
                    } else {
                        dis.prop('checked', true);
                        $('#' + $(this).attr('otherid')).focus();
                    }
                } else {
                    dis.removeAttr('checked');                    
                }

                if (!proj.IsBootstrapDesign()) {
                    dis.checkboxradio('refresh');
                }
            })

            inp.find('textarea').click(function (e) {
                //e.stopPropagation()
                //e.preventDefault();
                var data = $(this).data('data');
                var cb = $('#' + data.ID);
                cb.prop('checked', true);

                if (!proj.IsBootstrapDesign()) {
                    cb.checkboxradio('refresh');
                }

                var isCheck = cb.is(':checked');
                var max = CInt2(cb.attr('max'));
                var cntChecked = $('[name="' + cb.attr('name') + '"]:checked').length;

                if (isCheck) {
                    if (cntChecked > max && max != 0) {
                        cb.removeAttr('checked');
                        if (!IsMobile) {
                            WebToast('Max. selection is ' + max);
                        } else {
                            Android.Toast('Max. selection is ' + max);
                        }
                        $(this).blur();
                    } else {
                        setTimeout(function () { cb.prop('checked', true); })
                    }
                } else {
                    cb.removeAttr('checked');
                    $(this).val('');
                }

                if (!proj.IsBootstrapDesign()) {
                    cb.checkboxradio('refresh');
                }
            })
            .blur(function () {
                if ($.trim($(this).val()) == '') {
                   var inp = $(this).parent().prev().find('input[type="' + $(this).attr('selecttype') + '"]').removeAttr('checked')

                    if (!proj.IsBootstrapDesign()) {
                        inp.checkboxradio('refresh');
                    }
                }
            })

            $('#div-' + item.ID).append(inp)            
        }

        function EditNewItemModifier(vParams) {
            $('[name="txtIMSKUQuantity"]:first').val(vParams.ProductData.Quantity);

            for (var i = 0; i < vParams.ProductData.ItemModifier.length; i++) {
                var im = vParams.ProductData.ItemModifier[i];
                if (im.SelectedModifier == undefined) {
                    continue
                }
                for (var m = 0; m < im.SelectedModifier.length; m++) {
                    var modifier = im.SelectedModifier[m];
                    if (im.Type == "Inventory Affecting") {
                        $('.' + modifier.ID).val(modifier.InputQty);
                    } else if (im.Type == "Non-Inventory Affecting") {
                        var inp = $('#' + modifier.ID).prop('checked', true);
                        if (!proj.IsBootstrapDesign()) {
                            inp.checkboxradio('refresh');
                        }
                        if (inp.val() == 'Others') {
                            $('#' + inp.attr('otherid')).val(modifier.Others);
                        }
                    }
                }
            }
        }

        function OnSubmitModifier() {
            var txtModifierProductName = $.trim($('#txtModifierProductName').val());
            var listItemModifier = $('#listItemModifier');
            var im = JSON.parse(JSON.stringify(vOptions.TempItemModifier));
            var selectedModifier = [], selectedItemModifierID = [];
            var pSkuQty = CDec2($.trim($('[name="txtIMSKUQuantity"]:first').val()));
            var isdecimal = false;

            if (!Number.isInteger(pSkuQty) && !CBool(vOptions.AllowDecimal)) {
                isdecimal = true;
            }

            for (var i = 0; i < im.length; i++) {
                var imData = im[i];
                var hasInput = false;
                var selectedItem = [];
                var totalInvAff = 0, max = 0, min = 1;

                if (CBool(imData.NotAvailableThisTime)) {
                    continue;
                }

                if (imData.Items != null)
                    for (m = 0; m < imData.Items.length; m++) {
                        var item = imData.Items[m];                    
                        item = JSON.parse(JSON.stringify(item)); // clone JS objects; http://heyjavascript.com/4-creative-ways-to-clone-objects/
                        if (CStr2(imData.Type) == "Inventory Affecting") {
                            var inputModifier = $('.' + item.ID);
                            min = CDec2(inputModifier.attr('min'));
                            max = CDec2(inputModifier.attr('max'));
                            if (CDec2($.trim(inputModifier.val())) != 0) {
                                var pModifierQtyMultiplier;
                                if (imData.ChoicesReference == 'buy1take1') {
                                    imData.PromoBuyQty = CDec2(imData.PromoBuyQty);
                                    if (imData.PromoBuyQty <= 0)
                                        imData.PromoBuyQty = 1;
                                    pModifierQtyMultiplier = Math.floor(pSkuQty / imData.PromoBuyQty);
                                } else
                                    pModifierQtyMultiplier = pSkuQty;
                                
                                item.OrderQty = CStr2(CDec2($.trim(inputModifier.val())) * pModifierQtyMultiplier); // must be a string to equal the value from Business.LibraryItemModifiers.GetItemModifiers
                                item.InputQty = CStr2(CDec2($.trim(inputModifier.val())));
                                totalInvAff += CDec2(item.InputQty);
                                selectedItem.push(item);
                                selectedItemModifierID.push(item.ID);
                                hasInput = true;

                                if (!Number.isInteger(CDec2(item.InputQty))) {
                                    isdecimal = true;
                                    break;
                                }
                            }
                        }
                        else if (CStr2(imData.Type) == "Non-Inventory Affecting") {
                            item.Others = '';
                            var inputModifier = $('#' + item.ID);
                            if (inputModifier.is(':checked')) {
                                if(inputModifier.val() == "Others"){
                                    var otherfield = $.trim($('#' + inputModifier.attr('otherid')).val());
                                    if (otherfield == "") {
                                        $('#' + inputModifier.attr('otherid')).focus();
                                        $('#div-' + item.ItemModifierID).pageshow();
                                        WebToast('Please enter description')
                                        return false;
                                    } else {
                                        item.Others = otherfield;
                                    }
                                }
                                selectedItem.push(item);
                                selectedItemModifierID.push(item.ID);
                                hasInput = true;
                            }
                        }
                    } // END for

                if (isdecimal) {
                    break;
                }

                var msg;
                if (CBool(imData.IsRequired)) {
                    if (selectedItem.length == 0) {
                        msg = "Please select at least " + (CDec2(imData.Min) == 0 ? 1 : imData.Min) + " option(s) for modifier \"" + imData.Name + "\"";

                        NewMessageBox(msg, "Message");
                        
                        $('[data-gotopage="div-' + imData.ID + '"]').click();
                        document.getElementById('modbtn-' + imData.ID).scrollIntoView();
                        return;
                    }
                }

                if (CDec2(max) != 0 && totalInvAff > max) {
                    msg = "Please limit selection to " + max + " for modifier \"" + imData.Name + "\"";
                    
                    NewMessageBox(msg, "Message");

                    $('[data-gotopage="div-' + imData.ID + '"]').click();
                    return;
                }

                if (CDec2(min) != 0 && totalInvAff >= 1 && totalInvAff < min) {
                    msg = "Please select at least " + min + " item(s) for modifier \"" + imData.Name + "\"";

                    NewMessageBox(msg, "Message");

                    $('[data-gotopage="div-' + imData.ID + '"]').click();
                    return;
                }

                imData.SelectedModifier = selectedItem;
                selectedModifier.push(imData);                
            }

            if (isdecimal) {
                NewMessageBox('Decimal quantity is not allowed.');
                return;
            }

            if (pSkuQty == 0) {
                NewMessageBox('Please enter at least 1 qty');
                $('[name="txtIMSKUQuantity"]:first').focus();
                return;
            }

            vOptions.ProductData.Quantity = CDec2($.trim($('[name="txtIMSKUQuantity"]:first').val()));
            vOptions.ProductData.ItemModifier = selectedModifier;
            vOptions.ProductData.SelectedItemModifierID = selectedItemModifierID;
            vOptions.ProductData.EditModifier = vOptions.EditModifier;

            vOptions.success(vOptions.ProductData);
            if (proj.IsBootstrapDesign()) {
                $('body').removeClass('modal-open'); //para mawala ang scroll sa window
                $('#divNewPopupItemModifier').modal('toggle');
            } else {
                $('#divNewPopupItemModifier').hide();
            }
        }

        function AddSubTextbox(parentData, data) {
            var inputgroup;

            if (proj.IsBootstrapDesign()) {
                inputgroup = $('<div class="input-group">')
                                .append($('<button data-role="none" name="btnModifierMinus" class="btn bg-danger-transparent text-danger d-flex align-items-center rounded-1" type="button" style="font-size:20px;">')
                                    .data('data', data)
                                    .append($('<span class="bi-dash-circle-fill">'))
                                )
                                .append($('<input data-role="none" type="number" id="qty-' + data.ID + '" name="txtIMSKUQuantity" style="font-size:18px; text-align:center;" class="form-control-plaintext" placeholder="0" readonly>').data('data', data))
                                .append($('<button data-role="none" name="btnModifierAdd" class="btn bg-success-transparent text-success d-flex align-items-center rounded-1" type="button" style="font-size:20px;">')
                                    .data('data', data)
                                    .append($('<span class="bi-plus-circle-fill">'))
                                )
            } else {
                inputgroup = $('<div class="input-group">')
                            .append($('<div class="input-group-btn">')
                                .append($('<button data-role="none" name="btnModifierMinus" class="btn custom-btn-theme-c btn-sm">')
                                    .data('data', data)
                                    .append($('<img alt="Subtract" class="img-minus-black">'))
                                )
                            )
                            .append($('<input name="txtIMSKUQuantity" data-role="none" type="number" id="qty-' + data.ID + '" class="form-control number-only text-center">').data('data', data))
                            .append($('<div class="input-group-btn">')
                                .append($('<button data-role="none" name="btnModifierAdd" class="btn custom-btn-theme-b btn-sm">')
                                    .data('data', data)
                                    .append($('<img alt="Add" class="img-plus-white">'))
                                )
                            )
            }

            inputgroup.find('[name="btnModifierMinus"]').click(function () {
                var data = $(this).data('data');
                var txtIMSKUQuantity = $('#qty-' + data.ID);
                if (CDec2(txtIMSKUQuantity.val()) == 0) {
                    return
                }
                txtIMSKUQuantity.val((CDec2(txtIMSKUQuantity.val()) - 1));
            })

            inputgroup.find('[name="txtIMSKUQuantity"]')
                        .attr({
                            'imid': (parentData.ID || 'none'),
                            'min': (CInt2(parentData.Min) == 0 ? 1 : parentData.Min),
                            'max': parentData.Max
                        })
                        .data('data', data)
                        .addClass(data.ID)
                        .keyup(function (e) {
                            var data = $(this).data('data');
                            if ($.isEmptyObject(data) == false) {
                                var max = CInt2($(this).attr('max'));
                                var disValue = $(this).val() //+ String.fromCharCode(e.which);
                                var totalInput = 0;
                                $('#div-' + $(this).attr('imid')).find('[type="number"]:not(.' + data.ID + ')').each(function () {
                                    totalInput += CDec2($.trim($(this).val()));
                                })
                                if (max != 0) {
                                    if ((totalInput + CDec2(disValue)) > max) {
                                        if (!IsMobile) {
                                            WebToast('Max. selection is ' + max);
                                        } else {
                                            Android.Toast('Max. selection is ' + max);
                                        }
                                        $(this).val('');
                                        e.preventDefault();
                                        return false;
                                    }
                                }
                            }
                        })
                        .blur(function () {
                            var dis = $(this);
                            if (!Number.isInteger(CDec2(dis.val())) && IsNumeric(dis.val())) {
                                WebToast('Decimal quantity is not allowed.');
                            }
                        })

            inputgroup.find('[name="btnModifierAdd"]')
                        .attr({
                            'imid': (parentData.ID || 'none'),
                            'max': parentData.Max
                        })
                        .click(function () {
                            var data = $(this).data('data');
                            var txtIMSKUQuantity = $('#qty-' + data.ID);
                            var max = CInt2($(this).attr('max'));
                            var totalInput = 0;
                            $('#div-' + $(this).attr('imid')).find('[type="number"]').each(function () {
                                totalInput += CDec2($.trim($(this).val()));
                            })
                            if (max != 0) {
                                if (totalInput >= max) {
                                    if (!IsMobile) {
                                        WebToast('Max. selection is ' + max);
                                    } else {
                                        Android.Toast('Max. selection is ' + max);
                                    }
                                    return;
                                }
                            }
                            txtIMSKUQuantity.val((CDec2(txtIMSKUQuantity.val()) + 1));
                        })

            return inputgroup;
        }

        function ClosePopUp() {
            $('div[data-role="popup"]').popup().popup('close');
        }

        function RepositionPopup() {
            $('div[data-role="popup"]').popup().popup("reposition", { positionTo: 'window' });
        }

        function ChangePage(pageid) {
            $('div[IM-data-role="page"]').not(pageid).css({ visibility: 'hidden', display: 'none' });
            $(pageid).css({ visibility: 'visible', display: 'block' });
        }
    })