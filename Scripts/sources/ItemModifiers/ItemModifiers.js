require([
    'common',
    '/Scripts3/model/ItemModifiers.js?crc=',
    '/Scripts/jquery-ui-sortable.js'
], function () {

    var mModel = App.model.ItemModifiers();

    $(function () {

        $(document).ready(function () {
            var modifierData = null;
            var arrProductCategories = new Array();
            var isSearchNew = false;
            var isEditRebate = false;
            var modifierSKUID, modifierSKUName, modifierUomID, modifierUomName;
            var arrIAModifier = [], iaState, iaEditIndex, ctrIADeleted;
            var arrNIAModifier = [], niaState, niaEditIndex, ctrNIAActive;
            var arrRebates = [], rebateState, rebateEditIndex, ctrRebateDeleted;
            var arrProductCategoriesBackUp = [];
            var arrAssignedFowst = [], arrAssignedCustomer = [];
            var isSaveAs = false;

            window.ItemModifier = {
                DisplayItemModifierData: function () {
                    //$('#txtItemModifierName').text(sessionStorage.ItemModifierName);
                    $('#txtGroupModifierName').val(modifierData.Name);
                    $('#txtGroupModifierDescription').val(modifierData.Description);
                    $('#lblModifierType').text((CStr2(modifierData.Type) == "" ? 'Return For Rebates' : modifierData.Type)).pageshow();
                    $('#selModifierType').val((CStr2(modifierData.Type) == "" ? 'Return For Rebates' : modifierData.Type));
                    $('#selModifierRequired').prop('checked', CInt2(modifierData.IsRequired));//.slider('refresh');
                    $('#selActiveModifier').prop('checked', CInt2((CStr2(modifierData.IsActive) == '' ? '1' : modifierData.IsActive)));//.slider('refresh');
                    $('[name="radModApp"]').removeAttr('checked');//.checkboxradio('refresh');
                    $('[name="radModApp"][value="' + modifierData.PreferenceType + '"]').attr('checked', true);//.checkboxradio('refresh');
                    if (modifierData.PreferenceType == 'category') {
                        $('.lblModApp').text('Categories');
                    } else {
                        $('.lblModApp').text('SKUs');
                    }

                    if(modifierData.Type == 'Inventory Affecting'){
                        $('[name="chbOptIA"]').removeAttr('checked');//.checkboxradio('refresh');
                        $('[name="chbOptIA"][value="' + (modifierData.ChoicesReference || 'fromlistsku') + '"]').attr('checked', true);//.checkboxradio('refresh');
                        $('#txtSameSkuBuyQty').val(CStr2(modifierData.PromoBuyQty));
                        $('#txtSameSkuTakeQty').val(CStr2(modifierData.PromoTakeQty));
                        $('#txtSameSkuPrice').val(CStr2(modifierData.PromoTakePrice));
                        $('#txtSameSkuDiscount').val(CStr2(modifierData.PromoTakeDiscount));
                        $('#txtIAMin').val(CStr2(modifierData.Min));
                        $('#txtIAMax').val(CStr2(modifierData.Max));

                        if (CStr2(modifierData.PromoTakePrice) != '') {
                            $('#txtSameSkuDiscount').attr('disabled', true);
                        }

                        if (CStr2(modifierData.PromoTakeDiscount) != '') {
                            $('#txtSameSkuPrice').attr('disabled', true);
                        }

                        if ((modifierData.ChoicesReference || 'fromlistsku') == 'fromlistsku') {
                            $('#divForChooseFromSku').pageshow();
                        } else {
                            $('#divForSameSku').pageshow();
                        }
                    }

                    GetData();
                },
                Clear: function () {
                    $(window).scrollTop(0);
                    $('#txtGroupModifierName').val('');
                    $('#txtGroupModifierDescription').val('');
                    $('#selModifierType').val('Inventory Affecting').pageshow();
                    $('#selModifierRequired').prop('checked', 0);//.slider('refresh');
                    $('#selActiveModifier').prop('checked', 1);//.slider('refresh');
                    $('[name="affecting"]').prop('checked', false);//.checkboxradio('refresh');
                    $('[name="affecting"][value="radio"]').prop('checked', true);//.checkboxradio('refresh')
                    $('#tBodyCategories').empty().append('<tr><td colspan="2" style="text-align:center;">No Result(s) Found</td></tr>')
                    $('#divInventoryAffecting').pageshow();
                    $('#divOptionForMultipleSelect, #divForRestrictedModifier').hide();
                    $('[name="radModApp"]').removeAttr('checked');
                    $('[name="radModApp"][value="category"]').attr('checked', true);//.checkboxradio('refresh');
                    $('.lblModApp').text('Categories');
                    $('#txtSameSkuBuyQty, #txtSameSkuTakeQty').val('1');
                    $('.btnRestrictedTime, #txtSameSkuPrice, #txtSameSkuDiscount').val('');
                    $('#txtSameSkuPrice, #txtSameSkuDiscount').removeAttr('disabled');
                    $('#selRestrictedModifier').prop('checked', 0);//.slider('refresh');
                    $('[name="chbOptIA"]').removeAttr('checked');
                    $('[name="chbOptIA"][value="fromlistsku"]').attr('checked', true);//.checkboxradio('refresh');
                    $('.txtIMDatePeriod').val('');
                    $('#divForChooseFromSku').pageshow();

                    arrIAModifier = [];
                    arrNIAModifier = [];
                    arrRebates = [];
                    arrProductCategories = [];
                    arrProductCategoriesBackUp = [];
                    isSaveAs = false;
                    if (sessionStorage.ItemModifierData != undefined) {
                        modifierData = JSON.parse(sessionStorage.ItemModifierData);
                        this.DisplayItemModifierData();
                    } else {
                        modifierData = null;
                    }
                }
            }

            ItemModifier.Clear();            

            $('body')
            .on('click', '#listMenuIM > li', function () {
                var listMenu = $(this);
                var CurrentPageShowed;
                var NameCurrentActiveMenu = $('#listMenuIM').find('.active-menu').attr('id');
                var NameNewActiveMenu = listMenu.find('a').attr('id');
                var PageToShow = listMenu.find('a').attr('data-gotopage');
                $('#divContainerMenuPage > div').each(function () {
                    if ($(this).css('display') == 'block') {
                        CurrentPageShowed = $(this).attr('id');
                    }
                });

                $('#divContainerMenuPage').show();
                $('#' + NameCurrentActiveMenu + '').removeClass('active-menu');
                $('#' + NameNewActiveMenu + '').addClass('active-menu');
                $('#' + CurrentPageShowed + '').hide();
                $('#' + PageToShow + '').trigger('create').show();
            })
            .on('click', '#btnMobileMenuIM', function () {
                $('#divListMenuIM').slideToggle('fast');
            })
            .on('click', '#btnOKMsgBox', function () {
                $('#panMessageBox').modal('hide');
            })
            .on('click', '#divBtnCancel', function () {
                $('#panMessageBox').modal('hide');
            })
            .on('click', '#btnIMAddModifier', function () {
                $('#panIMAddOns').modal('show');
                modifierSKUID = null;
                modifierUomID = null;
                iaState = 'new';
                $('#txtIMSku').val('');
                $('#txtIMShortDescription').val('');
                $('#txtIMQuantity').val('');
                $('#txtIMUnitMeasure').val('');
                $('#txtIMUnitPrice').val('');
                $('#txtIMPremiumPrice').val('');
                $('#listIMStores').empty();
            })
            .on('click', '#btnIMRebates', function () {
                rebateState = 'new';
                SearchPIMProductPopUp();
                //$('#panIMRebates').modal('show');
                //$('#lstOfSKU').empty();
                //$('#divSearchSKUToReturn').show();
                //$('#divReturnedItem, #divSKUUnitMeasure, #divSKUPriceLevel').hide();
                //$('#txtIMReturnSKU').val('');
                //RepositionPopup();
            })
            .on('click', '#btnIMAddReplacements', function () {
                $('#panIMReplacements').modal('show');
            })
            .on('click', '#btnIMCustomerPreferences', function () {
                $('#panIMCustomerPreferences').modal('show');
            })
            .on('click', '#btnIMProductCategories', function () {
                //$('#panIMProductCategories').modal('show');
                if ($('[name="radModApp"]:checked').val() == 'category') {
                    BrowsePopup('PimCategories', 'Product Category', [
                    {
                        SelectMultiple: true,
                        Message: 'You can select multiple categories.',
                        DisplayText: [{ Column: 'Name' }],
                        Click: function (e) {
                            for (i = 0; i < e.length; i++) {
                                var isExist = false;

                                $.grep(arrProductCategories, function (d) {
                                    if (d.CategoryName == e[i].Name) {
                                        isExist = true;
                                        return false;
                                    }
                                })

                                if (!isExist) {
                                    arrProductCategories.push({
                                        CategoryName: e[i].Name,
                                        State: 'new'
                                    })
                                }
                            }

                            LoadCategoryList(arrProductCategories);
                        }
                    }
                    ])
                } else {
                    BrowsePopup('LibraryPIMProduct', 'Select SKUs', [
                        {
                            SelectMultiple: true,
                            Message: 'You can select multiple SKUs.',
                            DisplayText: [
                                { Column: 'ProductNo', Label: 'Product No' },
                                { Column: 'ProductName', Label: 'Name' },
                                {
                                    Column: function (e) {
                                        return AccountingFormat(e.RetailPrice);
                                    }, Label: 'Retail Price'
                                }
                            ],
                            Click: function (e) {
                                for (i = 0; i < e.length; i++) {
                                    var isExist = false;

                                    $.grep(arrProductCategories, function (d) {
                                        if(d.ProductID == e[i].ID){
                                            isExist = true;
                                            return false;
                                        }
                                    })

                                    if(!isExist){
                                        arrProductCategories.push({
                                            ProductID: e[i].ID,
                                            Name: e[i].ProductName,
                                            State: 'new'
                                        })
                                    }                                    
                                }

                                LoadCategoryList(arrProductCategories);
                            },
                            StyleFormat: 'table'
                        }
                    ])
                }
            })
            .on('click', '#btnCancelIMAddOns', function () {
                $('#panIMAddOns').modal('hide');
            })
            .on('click', '#btnCancelIMRebates', function () {
                $('#panIMRebates').modal('hide');
                $('#divSearchSKUToReturn').show();
                $('#divReturnedItem').hide();
                $('#txtIMRebatesAmount').val('');
                isEditRebate = false;
            })
            .on('click', '#btnCancelIMReplacements', function () {
                $('#panIMReplacements').modal('hide');
            })
            .on('click', '#btnCancelIMCustomerPreferences', function () {
                $('#panIMCustomerPreferences').modal('hide');
            })
            .on('click', '#btnCancelIMProductCategories', function () {
                $('#panIMProductCategories').modal('hide');
                $('#lstOfProductCategory').empty();
            })

            .on('keypress', '#txtIMCategoryPreference', function (e) {
                if (e.which == 13) {
                    SearchProductCategory($(this).val());
                }
            })
            .on('click', '#btnAddNewItem', function () {
                $('#panIMSelectedProductCategories').modal('hide');
                $('#panIMProductCategories').modal('show');
                isSearchNew = true;
            })
            .on('click', '#btnOkIMProductCategories', function () {
                var listSelectedProductCategory = $('#lstOfSelectedProductCategory');

                if (arrProductCategories.length == 0) {
                    $('#divErrorProductCategory').html('Please select at least one <br><br>').show();
                    return false;
                }

                $('#divErrorProductCategory').hide();

                $('#panIMProductCategories').modal('hide');
                $('#panIMSelectedProductCategories').modal('show');

                listSelectedProductCategory.empty();

                for (var i = 0; i < arrProductCategories.length; i++) {
                    listSelectedProductCategory.append($('<li>')
                                                            .append($('<a>')
                                                                .text(arrProductCategories[i])
                                                            )
                                                )
                }
                RepositionPopup();
                listSelectedProductCategory;//.listview('refresh');
                $('#lstOfProductCategory').empty();
                $('#txtIMCategoryPreference').val('');
            })
            .on('click', '#btnAddSelectedIMProductCategories', function () {
                $('#panIMSelectedProductCategories').modal('hide');
                mModel.ID = modifierData.ID;
                mModel.AddCategory({
                    CategoryNames: arrProductCategories,
                    success: function (vData) {
                        //console.log('jsLJ5CDQ4FCD! success');
                        NewMessageBox('Success saved', 'Category', [
                            {
                                Text: 'Ok' }
                        ], {
                            Close: function () {
                                GetData();
                                arrProductCategories = [];
                            }
                        })
                    },
                    error: function (vData) {
                        //console.log('jsCMHER4ZR75! ' + JSON.stringify(vData));
                        NewMessageBox(JSON.stringify(vData), 'Category', [
                            {
                                Text: 'Ok'
                            }
                        ], {
                            Close: function () {
                                arrProductCategories = [];
                            }
                        })
                    }
                });
            })
            .on('click', '#btnCancelSelectedIMProductCategories', function () {
                $('#panIMSelectedProductCategories').modal('hide');
                arrProductCategories = [];
            })
            .on('keypress', '#txtIMReturnSKU', function (e) {
                if (e.which == 13) {
                    SearchPIMProductPopUp($(this).val());
                }
            })
            .on('click', '#btnSKUName', function () {
                SearchPIMProductPopUp();
                //if (isEditRebate == false) {
                //    $('#divSearchSKUToReturn').show();
                //    $('#divReturnedItem').hide();
                //    $('#lstOfSKU').empty();
                //    $('#txtIMRebatesAmount').val('');
                //}
                //RepositionPopup();
            })
            .on('click', '#btnAddIMRebates', function () {
                $('#panIMRebates').modal('hide');
                var dataProduct = $('#btnSKUName').data('data');
                var txtSelectedSKUID = $.trim($('#txtSelectedSKUID').val());
                var txtIMReturnUnitQuantity = $.trim($('#txtIMReturnUnitQuantity').val());
                var txtIMReturnUnitMeasure = $.trim($('#txtIMReturnUnitMeasure').val());
                var txtIMReturnQuantity = $.trim($('#txtIMReturnQuantity').val());
                var txtIMRebatesAmount = $.trim($('#txtIMRebatesAmount').val());

                if (txtIMReturnQuantity == '') {
                    WebToast('Please enter quantity to be returned', 'top');
                    return;
                }

                if (txtIMRebatesAmount == '') {
                    WebToast('Please enter amount', 'top');
                    return;
                }

                if(rebateState == 'new'){
                    arrRebates.push({
                        ID: '',
                        ProductName: dataProduct.ProductName,
                        ProductNo: dataProduct.ProductNo,
                        ProductID: txtSelectedSKUID,
                        Amount: txtIMRebatesAmount,
                        UnitMeasure: txtIMReturnUnitMeasure,
                        UnitQuantity: txtIMReturnUnitQuantity,
                        Quantity: txtIMReturnQuantity,
                        State: rebateState
                    })
                } else {
                    arrRebates[CInt2(rebateEditIndex)].ProductName = dataProduct.ProductName;
                    arrRebates[CInt2(rebateEditIndex)].ProductNo = dataProduct.ProductNo;
                    arrRebates[CInt2(rebateEditIndex)].ProductID = txtSelectedSKUID;
                    arrRebates[CInt2(rebateEditIndex)].Amount = txtIMRebatesAmount;
                    arrRebates[CInt2(rebateEditIndex)].UnitMeasure = txtIMReturnUnitMeasure;
                    arrRebates[CInt2(rebateEditIndex)].UnitQuantity = txtIMReturnUnitQuantity;
                    arrRebates[CInt2(rebateEditIndex)].Quantity = txtIMReturnQuantity;
                }
                LoadRebatesList(arrRebates);
                //isEditRebate = false;
                //mModel.ID = modifierData.ID;
                //mModel.AddRebate({
                //    RebateID: ($('#txtRebateID').val() == "" ? '' : $('#txtRebateID').val()),
                //    ProductID: $('#txtSelectedSKUID').val(),
                //    Amount: $('#txtIMRebatesAmount').val(),
                //    UnitMeasure: $('#txtIMReturnUnitMeasure').val(),
                //    UnitQuantity: $('#txtIMReturnUnitQuantity').val(),
                //    Quantity: $('#txtIMReturnQuantity').val(),
                //    success: function (vData) {
                //        $('#panMessageBox').modal('show');
                //        $('#MsgBoxTitle').text('Category');
                //        $('#MsgBoxMessage').text('Successfully Saved');
                //        $('#btnOKMsgBox').unbind('click').click(function (e) {
                //            GetData();
                //            ClearRebatesFields();
                //        })
                //        RepositionPopup();
                //    },
                //    error: function (vData) {
                //        $('#panMessageBox').modal('show');
                //        $('#MsgBoxTitle').text('Category');
                //        $('#MsgBoxMessage').text(JSON.stringify(vData));
                //        RepositionPopup();
                //        ClearRebatesFields();
                //    }
                //})
            })
            .on('click', '#btnOKSKUUnitMeasure', function () {
                if ($('input[name="radSKUUnitMeasure"]:checked').val() == undefined) {
                    $('#errorSKUUnitMeasure').show();
                    return false;
                }

                $('#errorSKUUnitMeasure').hide();
                $('#divSKUUnitMeasure').hide();
                $('#divSKUPriceLevel').show();

                $('#radSKUPriceLevel').empty();

                App.model.ItemModifiers.GetPimData({
                    ProductID: $('#txtSelectedSKUID').val(),
                    success: function (vData) {
                        if (vData.data.Rows.length > 0) {
                            for (var i = 0; i < vData.data.Rows.length; i++) {
                                if (vData.data.Rows[i].UnitMeasure == $('input[name="radSKUUnitMeasure"]:checked').val()) {
                                    $('<input type="radio" name="radSKUPriceLevel" value="' + vData.data.Rows[i].DiscountedPrice + '" unitquantity="' + vData.data.Rows[i].Quantity + '" id="radio-choice-' + i + '"><label for="radio-choice-' + i + '">' + vData.data.Rows[i].PriceLevel + ' - ' + vData.data.Rows[i].DiscountedPrice + '</label>').appendTo("#radSKUPriceLevel");
                                    $("div").trigger('create');
                                }
                            }

                            if (vData.data.Rows.length == 1)
                                $("input[name='radSKUPriceLevel']:first").attr("checked", true);//.checkboxradio("refresh");
                        }
                        else {
                            $('<span>No Unit Measure found of the selected SKU</span>').appendTo('#radSKUPriceLevel');
                        }
                    },
                    error: function (vData) {
                        console.log('jsK5K86TCWYF! ' + JSON.stringify(vData));
                    }
                });
            })
            .on('click', '#btnOKSKUPriceLevel', function () {
                if ($('input[name="radSKUPriceLevel"]:checked').val() == undefined) {
                    $('#errorSKUPriceLevel').show();
                    return false;
                }

                $('#errorSKUPriceLevel').hide();
                $('#divSKUPriceLevel').hide();
                $('#divReturnedItem').show();

                $('#txtIMReturnUnitMeasure').val($('input[name="radSKUUnitMeasure"]:checked').val());
                $('#txtIMReturnUnitQuantity').val($('input[name="radSKUPriceLevel"]:checked').attr('unitquantity'));
                $('#txtIMRebatesAmount').val($('input[name="radSKUPriceLevel"]:checked').val());

                RepositionPopup();
                $('#txtIMReturnQuantity').focus();
            })
            .on('click', '.btnIMRebatesBack', function () {
                if ($('#divSKUUnitMeasure').css('display') == 'block') {
                    $('#divSearchSKUToReturn').show();
                    $('#divSKUUnitMeasure').hide();
                }
                else if ($('#divSKUPriceLevel').css('display') == 'block') {
                    $('#divSKUUnitMeasure').show();
                    $('#divSKUPriceLevel').hide();
                }
                RepositionPopup();
            })
            .on('click', '#butCancelRenameItemModifier', function () {
                $('#popRenameItemModifierName').modal('hide');
                $('#txtRenameItemModifier').val('');
            })
            .on('click', '#btnRenameItemModifier', function () {
                $('#popRenameItemModifierName').modal('show');
                $('#txtRenameItemModifier').val('');
            })
            .on('click', '#butSaveRenameItemModifier', function () {
                if ($('#txtRenameItemModifier').val() == "") {
                    $('#errorRenameItemModifier').show();
                    return false;
                }

                $('#errorRenameItemModifier').hide();
                $('#popRenameItemModifierName').modal('hide');
                mModel.ID = modifierData.ID;
                mModel.Rename({
                    NewName: $('#txtRenameItemModifier').val(),
                    success: function (vData) {
                        sessionStorage.ItemModifierName = $('#txtRenameItemModifier').val();
                        $('#txtItemModifierName').text($('#txtRenameItemModifier').val());
                        NewMessageBox('Successfully Saved');
                    },
                    error: function (vData) {
                        NewMessageBox(JSON.stringify(vData));
                    }
                });
            })
            .on('keypress', '#txtIMSku', function (e) {
                if (e.which == 13) {
                    BrowsePopup('LibraryPIMProduct', 'Search Product', [
                        {
                            StyleFormat: 'table',
                            DisplayText: [{ Label: 'Product No:', Column: 'ProductNo' }, { Label: 'Category', Column: 'Category' }, {Label: 'Product Name', Column: 'ProductName' }],
                            Click: function (e) {
                                var product = e;
                                modifierSKUName = product.ProductName;
                                modifierSKUID = product.ID;
                                $('#txtIMSku').val(product.ProductName);
                                $('#txtIMShortDescription').val((product.ProductNameOnly).substring(0, 15));
                                GetProductUom();
                            },
                            Search: $(this).val()
                        }
                    ])
                }
            })
            .on('click', '#txtIMUnitMeasure', function () {
                GetProductUom();
            })
            //.on('keydown', '#txtIMQuantity, #txtIMUnitPrice', function (e) {
            //    if (!NumberDotOnly($(this), e)) {
            //        return;
            //    }
            //})
            .on('click', '#btnAddIMAddOns', function () {
                var txtIMShortDescription = $.trim($('#txtIMShortDescription').val());
                var txtIMQuantity = $.trim($('#txtIMQuantity').val());
                var txtIMUnitPrice = CDec2($.trim($('#txtIMUnitPrice').val()));
                var txtIMPremiumPrice = CDec2($.trim($('#txtIMPremiumPrice').val()));
                var arrStore = [];
                //var txtIMMaxSelection = $.trim($('#txtIMMaxSelection').val());

                if (modifierSKUID == null) {
                    WebToast('Please search sku', 'top');
                    return;
                }

                if (txtIMShortDescription == '') {
                    WebToast('Please enter short description', 'top');
                    return;
                }

                if (CStr2(txtIMQuantity) == '') {
                    WebToast('Please enter quantity', 'top');
                    return;
                }

                if (modifierUomID == null) {
                    WebToast('Please search unit measure of selected sku', 'top');
                    return;
                }

                if (CStr2(txtIMUnitPrice) == '') {
                    WebToast('Please enter unit price', 'top');
                    return;
                }

                $('#listIMStores li').each(function () {
                    arrStore.push($(this).data('data'));
                })

                //if (txtIMMaxSelection == null) {
                //    WebToast('Please enter maximum selection', 'top');
                //    return;
                //}

                if(iaState == 'new'){
                    arrIAModifier.push({
                        ID: '',
                        ProductID: modifierSKUID,
                        ProductName: modifierSKUName,
                        Description: txtIMShortDescription,
                        Qty: txtIMQuantity,
                        UnitMeasureID: modifierUomID,
                        UnitMeasure: modifierUomName,
                        UnitPrice: txtIMUnitPrice,
                        PremiumPrice: txtIMPremiumPrice,
                        //Max: txtIMMaxSelection,
                        State: iaState,
                        Stores: arrStore
                    })
                } else {
                    arrIAModifier[CInt2(iaEditIndex)].ProductID = modifierSKUID;
                    arrIAModifier[CInt2(iaEditIndex)].ProductName = modifierSKUName;
                    arrIAModifier[CInt2(iaEditIndex)].Description = txtIMShortDescription;
                    arrIAModifier[CInt2(iaEditIndex)].Qty = txtIMQuantity;
                    arrIAModifier[CInt2(iaEditIndex)].UnitMeasureID = modifierUomID;
                    arrIAModifier[CInt2(iaEditIndex)].UnitMeasure = modifierUomName;
                    arrIAModifier[CInt2(iaEditIndex)].UnitPrice = txtIMUnitPrice;
                    arrIAModifier[CInt2(iaEditIndex)].PremiumPrice = txtIMPremiumPrice;
                    arrIAModifier[CInt2(iaEditIndex)].Stores = arrStore;
                    //arrIAModifier[CInt2(iaEditIndex)].Max = txtIMMaxSelection;
                }
                DisplayIAModifier();
                $('#panIMAddOns').modal('hide');
            })
            .on('change', '#selModifierType', function () {
                if ($(this).val() == 'Inventory Affecting') {
                    $('#divInventoryAffecting').pageshow();
                } else if ($(this).val() == 'Non-Inventory Affecting') {
                    $('#divNonInventoryAffecting').pageshow();
                } else {
                    $('#divItemRebates').pageshow();
                }
            })
            .on('click', '#btnIMAddNonInventoryModifier', function () {
                $('#panNonInventory').modal('show');
                $('#txtNIDescription').val('').focus();
                niaState = 'new';
            })
            .on('change', '[name="affecting"]', function () {
                if ($(this).val() == 'checkbox') {
                    $('#divOptionForMultipleSelect').show();
                } else {
                    $('#divOptionForMultipleSelect').hide();
                    $('#txtNIAMax').val('');
                }
            })
            .on('click', '#btnOkNI', function () {
                var desc = $.trim($('#txtNIDescription').val());
                if(desc == ''){
                    WebToast('Please enter description', 'top');
                    return;
                }
                if (niaState == 'new') {
                    arrNIAModifier.push({
                        ID: '',
                        Description: desc,
                        State: niaState,
                        OrderNo: ctrNIAActive + 1
                    })
                } else {
                    arrNIAModifier[CInt2(niaEditIndex)].Description = desc;
                }
                
                DisplayNIAModifier();
                $('#panNonInventory').modal('hide');
            })
            .on('click', '#btnCancelItemModifier', function () {
                location.hash = 'LibraryPage';
            })
            .on('click', '#btnSaveAsItemModifier', function () {
                isSaveAs = true;
                $('#btnSaveItemModifier').click()
            })
            .on('click', '#btnSaveItemModifier', function () {
                var txtGroupModifierName = $.trim($('#txtGroupModifierName').val());
                var txtGroupModifierDescription = $.trim($('#txtGroupModifierDescription').val());
                var selModifierType = $.trim($('#selModifierType').val());
                var selModifierRequired = $.trim(CInt2($('#selModifierRequired').is(':checked')));
                var selActiveModifier = $.trim(CInt2($('#selActiveModifier').is(':checked')));
                var inputType = $('[name="affecting"]:checked').val();
                var arrRestrictedDay = [];

                if (txtGroupModifierName == "") {
                    WebToast('Please edit group modifier name', 'top');
                    return;
                }

                if (selModifierType == 'Inventory Affecting') {
                    if ($('[name="chbOptIA"]:checked').val() == 'fromlistsku') {
                        if (arrIAModifier.length === 0 || ctrIADeleted === 0) {
                            WebToast('Please add at least one item', 'top');
                            return;
                        }

                        if ((CInt2($.trim($('#txtIAMin').val())) > CInt2($.trim($('#txtIAMax').val()))) && CInt2($.trim($('#txtIAMax').val())) != 0) {
                            WebToast('Max selection must be equal or greater than min selection', 'top');
                            return;
                        }
                    } else {
                        if (CDec2($.trim($('#txtSameSkuBuyQty').val())) < 1) {
                            WebToast('Please enter at least one buy quantity', 'top');
                            return;
                        }

                        if (CDec2($.trim($('#txtSameSkuTakeQty').val())) < 1) {
                            WebToast('Please enter at least one take quantity', 'top');
                            return;
                        }

                        if ($.trim($('#txtSameSkuPrice').val()) == '' && $.trim($('#txtSameSkuDiscount').val()) == '') {
                            WebToast('Please enter price or discount', 'top');
                            return;
                        }
                    }
                }
                else if (selModifierType == 'Non-Inventory Affecting') {
                    //if (arrNIAModifier.length === 0 || ctrNIAActive === 0) {
                    //    WebToast('Please add at least one item', 'top');
                    //    return;
                    //}
                } else if (selModifierType == 'Return For Rebates') {
                    if (arrRebates.length === 0 || ctrRebateDeleted === 0) {
                        WebToast('Please add at least one item', 'top');
                        return;
                    }
                }

                if (CBool($('#selRestrictedModifier').is(':checked'))) {
                    if ($('[name="radIMAvailability"]:checked').val() == 'byday') {
                        var isAllTimeEmpty = true;
                        $('#tableRestrictedTime input').each(function () {
                            if ($.trim($(this).val()) != '') {
                                isAllTimeEmpty = false;
                                return false;
                            }
                        })

                        if (isAllTimeEmpty) {
                            WebToast('Please select at least one restricted day and time', 'top');
                            return
                        }

                        var isInvalidTimeRange = false;
                        $('#tableRestrictedTime > .row:not(:eq(0))').each(function () {
                            isInvalidTimeRange = false;
                            var day = $(this).find('.col-2:eq(0)').text();
                            var time1 = $.trim($(this).find('input:eq(0)').val());
                            var time2 = $.trim($(this).find('input:eq(1)').val());

                            if (time1 != '' || time2 != '') {
                                if (time1 == '') {
                                    WebToast('Please set "Time From" for ' + day, 'top');
                                    isInvalidTimeRange = true;
                                    return false;
                                }

                                if (time2 == '') {
                                    WebToast('Please set "Time To" for ' + day, 'top');
                                    isInvalidTimeRange = true;
                                    return false;
                                }

                                if (!(new Date('01-01-2018 ' + time1) < new Date('01-01-2018 ' + time2))) {
                                    WebToast('Please set valid time range for ' + day, 'top');
                                    isInvalidTimeRange = true;
                                    return false;
                                }

                                arrRestrictedDay.push({
                                    Day: day,
                                    From: time1,
                                    To: time2
                                })
                            }
                        })

                        if (isInvalidTimeRange) {
                            return;
                        }

                        $('.txtIMDatePeriod').val('');
                    } else {
                        arrRestrictedDay = [];

                        if ($.trim($('#txtIMDatePeriodFrom').val()) == '' || $.trim($('#txtIMDatePeriodTo').val()) == '') {
                            WebToast('Please enter date period', 'top');
                            return
                        }
                    }
                }                

                //if (arrProductCategories.length === 0) {
                //    WebToast('Please add at least one category', 'top');
                //    return;
                //}
                
                var pData = {};
                pData.ID = CStr2(modifierData == undefined || isSaveAs ? '' : modifierData.ID);
                pData.GroupModifierName = txtGroupModifierName;
                pData.GroupModifierDescription = txtGroupModifierDescription;
                pData.ModifierType = selModifierType;
                pData.IsRequired = selModifierRequired;
                pData.IsActive = selActiveModifier;
                pData.ProductCategories = arrProductCategories;
                pData.PreferenceType = $('[name="radModApp"]:checked').val();
                pData.IsChangePreferenceType = (modifierData == undefined ? false : CStr2(modifierData.PreferenceType) == pData.PreferenceType ? false : true);
                pData.PrevProductCategories = arrProductCategoriesBackUp;
                pData.IsRestrictedModifier = CInt2($('#selRestrictedModifier').is(':checked'));
                pData.RestrictedDay = arrRestrictedDay;
                pData.DateFrom = pData.IsRestrictedModifier ? $.trim($('#txtIMDatePeriodFrom').val()) : '';
                pData.DateTo = pData.IsRestrictedModifier ? $.trim($('#txtIMDatePeriodTo').val()) : '';
                pData.AssignedFowst = arrAssignedFowst;
                pData.AssignedCustomer = arrAssignedCustomer;
                pData.IsSaveAs = isSaveAs;
                
                if (pData.ModifierType == 'Inventory Affecting') {
                    pData.Items = arrIAModifier;
                    pData.Min = CInt2($.trim($('#txtIAMin').val())) == 0 ? 1 : CInt2($.trim($('#txtIAMin').val()));
                    pData.Max = $.trim($('#txtIAMax').val());
                    pData.ChoicesReference = $('[name="chbOptIA"]:checked').val();
                    pData.PromoBuyQty = CDec2($('#txtSameSkuBuyQty').val());
                    pData.PromoTakeQty = CDec2($('#txtSameSkuTakeQty').val());
                    pData.PromoPrice = CStr2($.trim($('#txtSameSkuPrice').val()));
                    pData.PromoDiscount = CStr2($.trim($('#txtSameSkuDiscount').val()));
                } else if (pData.ModifierType == 'Non-Inventory Affecting') {
                    if (pData.ID == '') { // add OTHERS only in every new NIA Modifier
                        arrNIAModifier.push({
                            ID: '',
                            Description: 'Others',
                            OtherDescription: ($('#txtNIAOthersLabel').val() == '' ? 'Others' : $('#txtNIAOthersLabel').val()),
                            NoOfCharacters: CInt2($('#txtNIAOthersCharLimit').val()),
                            State: 'new',
                            OrderNo: ctrNIAActive + 1
                        })
                    } else {
                        $.grep(arrNIAModifier, function (param) {
                            if (param.Description == 'Others') {
                                param.OtherDescription = ($('#txtNIAOthersLabel').val() == '' ? 'Others' : $('#txtNIAOthersLabel').val());
                                param.NoOfCharacters = CInt2($('#txtNIAOthersCharLimit').val());
                            }
                        })
                    }
                    pData.Items = arrNIAModifier;
                    pData.Type = inputType;
                    pData.Max = $.trim($('#txtNIAMax').val());
                } else if(pData.ModifierType == 'Return For Rebates') {
                    pData.Items = arrRebates;
                }
                //console.log(pData.Items)
                //return;
                mModel.SaveItemModifier({
                    Data: pData,
                    success: function (vData) {
                        WebToast('Successfully saved', 'top');
                        $('#popModifier').hide();
                        LibraryTable.SearchItemModifiers();
                        window.location.hash = 'LibraryPage';
                    },
                    error: function (vData) {
                        NewMessageBox(JSON.stringify(vData), 'Message');
                    }
                })
            })
            .on('blur', '#txtNIAOthersLabel', function (e) {
                if ($.trim($(this).val()) == '') {
                    $('#txtNIAOthersCharLimit').prop('disabled', 1).val('');
                } else {
                    $('#txtNIAOthersCharLimit').removeAttr('disabled').val('');
                }
            })
            .on('change', '[name="radModApp"]', function (e) {
                arrProductCategories = [];
                $('#tBodyCategories').empty();

                if($(this).val() == 'category'){
                    $('.lblModApp').text('Categories');
                } else {
                    $('.lblModApp').text('SKUs');
                }

                if ($(this).val() == CStr2(sessionStorage.ItemModifierPreferenceType)) {
                    arrProductCategories = JSON.parse(JSON.stringify(arrProductCategoriesBackUp));
                    LoadCategoryList(arrProductCategories);
                }
            })
            .on('change', '#selRestrictedModifier', function (e) {
                if (CBool($(this).is(':checked'))) {
                    $('#divForRestrictedModifier').show();
                } else {
                    $('#divForRestrictedModifier').hide();
                }
            })
            .on('click', '.btnRestrictedTime', function () {
                var dis = $(this);

                CustomSetTime({
                    Click: function (e) {
                        dis.val(e.time);
                    }
                })
            })
            .on('click', '.btnRestrictedClearField', function () {
                $(this).parents().closest('.row').find('input[type="text"]').val('');
            })
            .on('change', '[name="chbOptIA"]', function () {
                $('#txtSameSkuBuyQty, #txtSameSkuTakeQty').val('1');
                $('#txtSameSkuPrice, #txtSameSkuDiscount').val('').removeAttr('disabled');
                if($(this).val() == 'fromlistsku'){
                    $('#divForChooseFromSku').pageshow();
                } else {
                    $('#divForSameSku').pageshow();
                }
            })
            .on('keydown', '#txtSameSkuBuyQty, #txtSameSkuTakeQty', function (e) {
                if(!NumberOnly(e)){
                    return;
                }
            })
            .on('keydown', '#txtSameSkuDiscount, #txtSameSkuPrice', function (e) {
                if (!NumberDotOnly($(this), e)) {
                    return;
                }
            })
            .on('blur', '#txtSameSkuPrice', function () {
                if($.trim($(this).val()) != ''){
                    $('#txtSameSkuDiscount').attr('disabled', true);
                } else {
                    $('#txtSameSkuDiscount').removeAttr('disabled');
                }
            })
            .on('blur', '#txtSameSkuDiscount', function () {
                if ($.trim($(this).val()) != '') {
                    $('#txtSameSkuPrice').attr('disabled', true);
                } else {
                    $('#txtSameSkuPrice').removeAttr('disabled');
                }
            })
            .on('click', '#butCancelItemModifier', function () {
                $('#popItemModifierName').modal('hide');
            })
            .on('click', '#btnIMAssignedFOWST', function () {
                PopupButtonList('', 'Options', [
                    {
                        Text: 'Select from Company\'s FOWST list', Click: function () {
                            BrowsePopup('LibraryFOWST', 'Select FOWST Locations', [
                                {
                                    DisplayText: [{ Column: 'FowstName' }],
                                    AutoSearch: true,
                                    SelectMultiple: true,
                                    Message: 'You can select multiple FOWST locations. Click OK when you\'re done.',
                                    Click: function (vData) {
                                        AddSelectedFOWST(vData);
                                    }
                                }
                            ])
                        }
                    }, {
                        Text: 'Select from Group\'s FOWST list', Click: function () {
                            BrowsePopup('LibraryFranchiseGroup', 'Select Group', [
                                {
                                    DisplayText: [{ Column: 'GroupName' }],
                                    AutoSearch: true,
                                    Message: '<b>Note:</b> FOWST location(s) of the selected group will be added on the list.',
                                    Click: function (e) {
                                        Bs.Loading('show');

                                        AjaxSend({
                                            url: 'ItemModifiers/GetFowstOfSelectedGroup',
                                            data: {
                                                GroupID: e.ID
                                            }, success: function (e) {
                                                Bs.Loading('hide');
                                                AddSelectedFOWST(e.Results.Rows);
                                            }
                                        })
                                    }
                                }
                            ])
                        }
                    }
                ])
            })
            .on('click', '#btnIMRemoveAllAssignedFOWST', function () {
                NewMessageBox('Are you sure you want to remove all assigned FOWSTs?', 'Confirmation Message', [
                    {
                        Text: 'Yes', Click: function () {
                            for (var i in arrAssignedFowst) {
                                arrAssignedFowst[i].State = 'delete';
                            }
                            DisplayAssignedFowst();
                        }
                    }, { Text: 'No' }
                ])
            })
            .on('click', '#btnIMAssignedCustomer', function () {
                BrowsePopup('LibraryOrganizationCustomer', 'Select Customers', [
                    {
                        DisplayText: [{ Column: 'CustomerName' }],
                        SelectMultiple: true,
                        Message: 'You can select multiple Customers. Click OK when you\'re done.',
                        Click: function (vData) {
                            for (var i = 0; i < vData.length; i++) {
                                var index = null;

                                $.grep(arrAssignedCustomer, function (e, ind) {
                                    if (e.ID == vData[i].ID) {
                                        index = ind;
                                    }
                                })

                                if (index == null) {
                                    vData[i].State = 'new';
                                    arrAssignedCustomer.push(vData[i]);
                                } else {
                                    arrAssignedCustomer[index].State = 'new';
                                }
                            }

                            DisplayAssignedCustomer();
                        }
                    }
                ])
            })
            .on('change', '[name="radIMAvailability"]', function () {
                if ($(this).val() == 'byday') {
                    $('#tableRestrictedTime').show();
                    $('#divIMByDatePeriod').hide();
                } else {
                    $('#tableRestrictedTime').hide();
                    $('#divIMByDatePeriod').show();
                }
            })
            .on('click', '.txtIMDatePeriod', function () {
                DualCalendar({
                    SetFromDate: $.trim($('#txtIMDatePeriodFrom').val()),
                    SetToDate: $.trim($('#txtIMDatePeriodTo').val()),
                    Click: function (e) {
                        $('#txtIMDatePeriodFrom').val(e.From);
                        $('#txtIMDatePeriodTo').val(e.To);
                    }
                })
            })
            .on('click', '#btnIMStores', function () {
                PopupButtonList('', 'Options', [
                    {
                        Text: 'Select from Company\'s FOWST list', Click: function () {
                            BrowsePopup('LibraryFOWST', 'Select FOWST Locations', [
                                {
                                    DisplayText: [{ Column: 'FowstName' }],
                                    AutoSearch: true,
                                    SelectMultiple: true,
                                    AllowToSelectAll: true,
                                    Message: 'You can select multiple stores. Click OK when you\'re done.',
                                    Click: function (vData) {
                                        PopulateIMStore(vData);
                                    }
                                }
                            ])
                        }
                    }, {
                        Text: 'Select from Group\'s FOWST list', Click: function () {
                            BrowsePopup('LibraryFranchiseGroup', 'Select Group', [
                                {
                                    DisplayText: [{ Column: 'GroupName' }],
                                    AutoSearch: true,
                                    Message: '<b>Note:</b> Stores of the selected group will be added on the list.',
                                    Click: function (e) {
                                        Bs.Loading('show');

                                        AjaxSend({
                                            url: 'ItemModifiers/GetFowstOfSelectedGroup',
                                            data: {
                                                GroupID: e.ID
                                            }, success: function (e) {
                                                Bs.Loading('hide');
                                                PopulateIMStore(e.Results.Rows);
                                            }
                                        })
                                    }
                                }
                            ])
                        }
                    }
                ])
            })
            .on('click', '#btnIMSelectAllStores', function () {
                $('.chbIMStores').prop('checked', 1)
            })

            function PopulateIMStore(vRows) {
                var listIMStores = $('#listIMStores');

                for (var i in vRows) {
                    var data = vRows[i];

                    if ($('#li' + data.ID).length > 0) continue;

                    data.State = 'new';

                    listIMStores.append($('<li class="list-group-item" id="li' + data.ID + '">')
                            .data('data', data)
                            .append($('<table style="width:100%">')
                                .append($('<tr>')
                                    .append($('<td style="width:40px;">')
                                        .append($('<input data-role="none" type="checkbox" class="form-check-input chbIMStores">')
                                            .change(function () {
                                                if ($('.chbIMStores:checked').length > 0) {
                                                    $('#btnIMRemoveStores').removeAttr('disabled').unbind('click').click(function () {
                                                        $('.chbIMStores:checked').each(function () {
                                                            $(this).parents().closest('li').remove();
                                                            $('#btnIMRemoveStores').prop('disabled', 1).unbind('click');
                                                        })
                                                    });
                                                } else {
                                                    $('#btnIMRemoveStores').prop('disabled', 1).unbind('click');
                                                }
                                            })
                                        )
                                    )
                                    .append($('<td>')
                                        .text(data.FowstName)
                                    )
                                )
                            )
                        )
                }
            }

            function AddSelectedFOWST(vData) {
                for (var i = 0; i < vData.length; i++) {
                    var index = null;

                    $.grep(arrAssignedFowst, function (e, ind) {
                        if (e.ID == vData[i].ID) {
                            index = ind;
                        }
                    })

                    if (index == null) {
                        vData[i].State = 'new';
                        arrAssignedFowst.push(vData[i]);
                    } else {
                        arrAssignedFowst[index].State = 'new';
                    }
                }

                DisplayAssignedFowst();
            }

            function DisplayAssignedCustomer() {
                var listIMAssignedCustomers = $('#listIMAssignedCustomers');

                listIMAssignedCustomers.empty();

                if (arrAssignedCustomer.length > 0) {
                    for (var i = 0; i < arrAssignedCustomer.length; i++) {
                        arrAssignedCustomer[i].Index = i;
                        if (arrAssignedCustomer[i].State != 'delete') {
                            listIMAssignedCustomers.append($('<li class="list-group-item">')
                                .append($('<table style="width:100%">')
                                    .append($('<tr>')
                                        .append($('<td style="width:50px;">')
                                            .text((i + 1) + '.')
                                        )
                                        .append($('<td>')
                                            .text(arrAssignedCustomer[i].CustomerName)
                                        )
                                        .append($('<td class="text-center" style="width:50px;">')
                                            .append($('<button data-role="none" class="btn btn-danger">')
                                                .append($('<span class="bi-trash">'))
                                                .data('data', arrAssignedCustomer[i])
                                                .click(function () {
                                                    var data = $(this).data('data');

                                                    NewMessageBox('Are you sure you want to remove <b>' + data.CustomerName + '</b>?', 'Confirmation Dialog', [
                                                        {
                                                            Text: 'Yes', Click: function () {
                                                                data.State = 'delete';
                                                                DisplayAssignedCustomer();
                                                            }
                                                        }, { Text: 'No' }
                                                    ])
                                                })
                                            )
                                        )
                                    )
                                )
                            )
                        }
                    }
                } else {
                    listIMAssignedCustomers.append('<li class="list-group-item">No assigned Customers</li>');
                }
            }

            function DisplayAssignedFowst() {
                var listIMAssignedFowstLocations = $('#listIMAssignedFowstLocations');

                listIMAssignedFowstLocations.empty();

                if (arrAssignedFowst.length > 0) {
                    for (var i = 0; i < arrAssignedFowst.length; i++) {
                        arrAssignedFowst[i].Index = i;
                        if (arrAssignedFowst[i].State != 'delete') {
                            listIMAssignedFowstLocations.append($('<li class="list-group-item">')
                                .append($('<table style="width:100%">')
                                    .append($('<tr>')
                                        .append($('<td style="width:50px;">')
                                            .text((i + 1) + '.')
                                        )
                                        .append($('<td>')
                                            .text(arrAssignedFowst[i].FowstName)
                                        )
                                        .append($('<td class="text-center" style="width:50px;">')
                                            .append($('<button data-role="none" class="btn btn-danger">')
                                                .append($('<span class="bi-trash">'))
                                                .data('data', arrAssignedFowst[i])
                                                .click(function () {
                                                    var data = $(this).data('data');

                                                    NewMessageBox('Are you sure you want to remove <b>' + data.FowstName + '</b>?', 'Confirmation Dialog', [
                                                        {
                                                            Text: 'Yes', Click: function () {
                                                                data.State = 'delete';
                                                                DisplayAssignedFowst();
                                                            }
                                                        }, { Text: 'No' }
                                                    ])
                                                })
                                            )
                                        )
                                    )
                                )
                            )
                        }                        
                    }
                } else {
                    listIMAssignedFowstLocations.append('<li class="list-group-item">No assigned FOWST locations</li>');
                }
            }

            function DisplayIAModifier() {
                var tBodyListOfModifiers = $('#tBodyListOfModifiers'), isDrag = false;
                tBodyListOfModifiers.empty();
                ctrIADeleted = arrIAModifier.length;

                if (arrIAModifier.length > 0) {
                    for (var i = 0; i < arrIAModifier.length; i++) {
                        arrIAModifier[i].Index = i;
                        if (arrIAModifier[i].State != 'remove') {
                            tBodyListOfModifiers.append($('<tr>')
                                .data('data', arrIAModifier[i])
                                .append($('<td class="linenumber">')
                                    .text((i + 1) + '.')
                                )
                                .append($('<td>')
                                    .text(arrIAModifier[i].ProductNo)
                                )
                                .append($('<td>')
                                    .text(arrIAModifier[i].ProductName)
                                )
                                .append($('<td>')
                                    .text(arrIAModifier[i].Description)
                                )
                                //.append($('<td>')
                                //    .css('text-align', 'right')
                                //    .text(CDec2(arrIAModifier[i].Max))
                                //)
                                .append($('<td>')
                                    .css('text-align', 'right')
                                    .text(CDec2(arrIAModifier[i].Qty))
                                )
                                .append($('<td>')
                                    .css('text-align', 'center')
                                    .text(arrIAModifier[i].UnitMeasure)
                                )
                                .append($('<td>')
                                    .css('text-align', 'right')
                                    .text(AccountingFormat(arrIAModifier[i].UnitPrice))
                                )
                                .append($('<td>')
                                    .css('text-align', 'center')
                                    .text((arrIAModifier[i].Stores.length > 0 ? 'Not available in ' + arrIAModifier[i].Stores.length + ' store(s)': ''))
                                )
                                .append($('<td>')
                                    .css('text-align', 'center')
                                    .data('data', arrIAModifier[i])
                                    .append($('<button data-role="none" class="btn btn-danger">')
                                        .append($('<span class="bi-trash">'))
                                    )
                                    .click(function (e) {
                                        e.stopPropagation();
                                        e.preventDefault();
                                        var data = $(this).data('data');
                                        NewMessageBox('Are you sure you want to remove this record?', 'Message', [
                                            {
                                                Text: 'Yes', Click: function () {
                                                    if (data.State == 'new') {
                                                        (arrIAModifier).splice(CInt2(data.Index), 1);
                                                    } else {
                                                        arrIAModifier[CInt2(data.Index)].State = 'remove';
                                                    }                                                    
                                                    DisplayIAModifier();
                                                    ctrIADeleted -= 1;
                                                }
                                            }, { Text: 'No' }
                                        ])
                                    })
                                )
                                .click(function (e) {
                                    if (isDrag) {
                                        isDrag = false
                                        return;
                                    }
                                    var data = $(this).data('data');
                                    iaState = 'edit';
                                    iaEditIndex = data.Index;
                                    modifierSKUID = data.ProductID;
                                    modifierSKUName = data.ProductName;
                                    modifierUomID = data.UnitMeasureID
                                    modifierUomName = data.UnitMeasure;
                                    $('#txtIMSku').val(modifierSKUName);
                                    $('#txtIMShortDescription').val(data.Description);
                                    $('#txtIMQuantity').val(CDec2(data.Qty));
                                    $('#txtIMUnitMeasure').val(modifierUomName);
                                    $('#txtIMUnitPrice').val(AccountingFormat(data.UnitPrice));
                                    $('#txtIMPremiumPrice').val(AccountingFormat(data.PremiumPrice));
                                    $('#txtIMMaxSelection').val(data.Max);
                                    $('#btnIMRemoveStores').prop('disabled', 1);
                                    $('#listIMStores').empty();
                                    PopulateIMStore(data.Stores);
                                    $('#panIMAddOns').modal('show');
                                })
                                .on('mousedown', function () {
                                    isDrag = true;
                                })
                                .on('mouseup', function () {
                                    isDrag = false;
                                })
                            )                            
                        }
                    }

                    tBodyListOfModifiers.parent();//.table('refresh');
                } else {
                    tBodyListOfModifiers.append('<tr><td colspan="8" style="text-align:center;">No Result(s) Found</td></tr>')
                }

                tBodyListOfModifiers.sortable({
                    list: '#tBodyListOfModifiers',
                    delay: 150,
                    start: function (event, ui) {
                        ui.item.parent().css('border', '1px solid #CCC');
                    },
                    stop: function (event, ui) {
                        ui.item.parent().css('border', '1px solid #FFF');
                    },
                    update: function (event, ui) {
                        var id = ui.item.parent().attr('id');
                        var newOrderNo = 1;
                        $('#' + id).find('> tr').each(function () {
                            $(this).attr('OrderNo', newOrderNo);
                            $(this).data('data').OrderNo = newOrderNo;
                            $(this).find('.linenumber').text(newOrderNo + '.')
                            newOrderNo += 1;
                        })
                    }
                });
            }

            function DisplayNIAModifier() {
                var lstNonInventoryModifiers = $('#lstNonInventoryModifiers');
                lstNonInventoryModifiers.empty();
                ctrNIAActive = arrNIAModifier.length;

                arrNIAModifier.sort(function (a, b) {
                    return parseFloat(a.OrderNo) - parseFloat(b.OrderNo);
                });

                for (var i = 0; i < arrNIAModifier.length; i++) {
                    arrNIAModifier[i].Index = i;
                    if(arrNIAModifier[i].Type != undefined){
                        $('[name="affecting"]').prop('checked', false);//.checkboxradio('refresh');
                        $('[name="affecting"][value="' + arrNIAModifier[i].Type + '"]').prop('checked', true);//.checkboxradio('refresh');
                        if (arrNIAModifier[i].Type == 'checkbox') {
                            $('#divOptionForMultipleSelect').show();
                        }
                    }

                    if (arrNIAModifier[i].OrderNo == undefined) {
                        arrNIAModifier[i].OrderNo = i + 1;
                    }

                    if (arrNIAModifier[i].State != 'remove') {
                        if (arrNIAModifier[i].Description != 'Others') {
                            // do not show OTHERS, it is for internal use only
                            lstNonInventoryModifiers.append($('<a href="#" class="list-group-item list-group-item-action">')
                                .attr('ID', arrNIAModifier[i].ID)
                                .append($('<table style="width:100%;">')
                                    .append($('<tr>')
                                        .append($('<td style="word-break:break-all;">')
                                            .addClass('main')
                                            .data('data', arrNIAModifier[i])
                                            .attr('orderno', arrNIAModifier[i].OrderNo)
                                            .text(arrNIAModifier[i].Description)
                                            .click(function (e) {
                                                var data = $(this).data('data');
                                                niaState = 'edit';
                                                niaEditIndex = data.Index;
                                                $('#txtNIDescription').val(data.Description);
                                                $('#panNonInventory').modal('show');
                                            })
                                        )
                                        .append($('<td style="width:50px; text-align:center;">').append($('<span class="btn btn-xs btn-danger">')
                                            .append($('<span class="bi-trash" aria-hidden="true">'))
                                            .data('data', arrNIAModifier[i])
                                            .click(function (e) {
                                                var data = $(this).data('data');
                                                NewMessageBox('Are you sure you want to remove this record?', 'Message', [
                                                    {
                                                        Text: 'Yes', Click: function () {
                                                            if (data.State == 'new') {
                                                                (arrNIAModifier).splice(CInt2(data.Index), 1);
                                                            } else {
                                                                arrNIAModifier[CInt2(data.Index)].State = 'remove';
                                                            }
                                                            DisplayNIAModifier();
                                                            ctrNIAActive -= 1;
                                                        }
                                                    }, { Text: 'No' }
                                                ])
                                            })
                                        )
                                        )
                                    )
                                )
                            )
                        } else {
                            $('#txtNIAOthersLabel').val((CStr2(arrNIAModifier[i].OthersDescription) == '' ? 'Others' : arrNIAModifier[i].OthersDescription));
                            $('#txtNIAOthersCharLimit').val((CStr2(arrNIAModifier[i].OthersDescription) == '' ? '' : CInt2(arrNIAModifier[i].NoOfCharacters)));
                        }
                    }                    
                }
                lstNonInventoryModifiers;//.listview();//.listview('refresh');
            }

            function GetProductUom() {
                if(modifierSKUID == null){
                    WebToast('Please search sku', 'top');
                    return;
                }
                BrowsePopup('LibraryPIMProductMeasure', 'Unit of Measure', [
                    {
                        DisplayText: [{ Column: 'Measurement' }],
                        Click: function (e) {
                            var uom = e;
                            modifierUomID = uom.ID;
                            modifierUomName = uom.Measurement;
                            $('#txtIMUnitMeasure').val(uom.Measurement);
                        },
                        AutoSearch: true,
                        Close: function () {
                            $('#panIMAddOns').modal('show');
                        },
                        Message: 'Please choose the typical unit of measure for <b>' + $.trim($('#txtIMSku').val()) + '</b>'
                    }
                ], { ProductID: modifierSKUID })
            }

            function GetData() {
                mModel.ID = modifierData.ID;
                mModel.GetData({
                    success: function (vData) {
                        var info = vData.Info.Rows[0];

                        $('#txtItemModifierName').text(info.Name);

                        if (CStr2(info.Type) == "" || info.Type == "Return For Rebates") {
                            $('#divItemRebates').pageshow();
                        } else if (info.Type == "Inventory Affecting") {
                            $('#divInventoryAffecting').pageshow();
                            $('#txtIAMin').val((CInt2(info.Min) == 0 ? 1 : CInt2(info.Min)));
                            $('#txtIAMax').val(CInt2(info.Max));
                        } else if (info.Type == "Non-Inventory Affecting") {
                            $('#divNonInventoryAffecting').pageshow();
                            $('#txtNIAMax').val(CInt2(info.Max));
                        }

                        arrIAModifier = vData.IA;
                        arrNIAModifier = vData.NIA.Rows;
                        arrRebates = vData.Rebates.Rows;
                        arrProductCategories = vData.Categories.Rows;
                        arrProductCategoriesBackUp = vData.Categories.Rows;
                        arrAssignedFowst = vData.AssignedFowst.Rows;
                        arrAssignedCustomer = vData.AssignedCustomer.Rows;
                        LoadCategoryList(arrProductCategories);
                        LoadRebatesList(arrRebates);
                        DisplayIAModifier();
                        DisplayNIAModifier();
                        DisplayAssignedFowst();
                        DisplayAssignedCustomer();

                        $('#lstNonInventoryModifiers').sortable({
                            list: '#lstNonInventoryModifiers',
                            delay: 150,
                            start: function (event, ui) {
                                ui.item.parent().css('border', '1px solid #CCC');
                            },
                            stop: function (event, ui) {
                                ui.item.parent().css('border', '1px solid #FFF');
                            },
                            update: function (event, ui) {
                                var id = ui.item.parent().attr('id');
                                var newOrderNo = 1;
                                $('#' + id).find('> li').each(function () {
                                    $(this).find('.main').attr('OrderNo', newOrderNo);
                                    $(this).find('.main').data('data').OrderNo = newOrderNo;
                                    newOrderNo += 1;
                                })
                            }
                        });

                        var info = vData.Info.Rows[0];

                        if (CBool(info.IsRestrictedModifier)) {
                            $('#selRestrictedModifier').prop('checked', 1);//.slider('refresh');
                            $('#divForRestrictedModifier').show();

                            $('#txtModifierMondayFrom').val(CStr2(info.MondayTimeFrom));
                            $('#txtModifierMondayTo').val(CStr2(info.MondayTimeTo));

                            $('#txtModifierTuesdayFrom').val(CStr2(info.TuesdayTimeFrom));
                            $('#txtModifierTuesdayTo').val(CStr2(info.TuesdayTimeTo));

                            $('#txtModifierWednesdayFrom').val(CStr2(info.WednesdayTimeFrom));
                            $('#txtModifierWednesdayTo').val(CStr2(info.WednesdayTimeTo));

                            $('#txtModifierThursdayFrom').val(CStr2(info.ThursdayTimeFrom));
                            $('#txtModifierThursdayTo').val(CStr2(info.ThursdayTimeTo));

                            $('#txtModifierFridayFrom').val(CStr2(info.FridayTimeFrom));
                            $('#txtModifierFridayTo').val(CStr2(info.FridayTimeTo));

                            $('#txtModifierSaturdayFrom').val(CStr2(info.SaturdayTimeFrom));
                            $('#txtModifierSaturdayTo').val(CStr2(info.SaturdayTimeTo));

                            $('#txtModifierSundayFrom').val(CStr2(info.SundayTimeFrom));
                            $('#txtModifierSundayTo').val(CStr2(info.SundayTimeTo));

                            var isByDay = false;
                            $('.btnRestrictedTime').each(function () {
                                if ($.trim($(this).val()) != '') {
                                    isByDay = true;
                                    return false
                                }
                            })

                            if (isByDay) {
                                $('#radAvailableByDay').prop('checked', 1);
                                $('#tableRestrictedTime').show();
                                $('#divIMByDatePeriod').hide();
                                $('.txtIMDatePeriod').val('');
                            } else {
                                $('.btnRestrictedTime').val('');
                                $('#txtIMDatePeriodFrom').val($.format.date(new Date(info.DateFrom), 'MMM dd, yyyy'));
                                $('#txtIMDatePeriodTo').val($.format.date(new Date(info.DateTo), 'MMM dd, yyyy'));
                                $('#radAvailableByDatePeriod').prop('checked', 1);
                                $('#tableRestrictedTime').hide();
                                $('#divIMByDatePeriod').show();
                            }
                        }
                    },
                    error: function (vData) {
                        console.log('jsLSMLZ68WAK! ' + JSON.stringify(vData));
                    }
                });
            }

            function LoadCategoryList(vData) {
                var pLV = $('#tBodyCategories');

                pLV.empty();

                if (vData.length > 0) {
                    for (var i = 0; i < vData.length; i++) {
                        vData[i].Index = i;
                        if(vData[i].State != 'remove'){
                            pLV.append($('<tr>')
                                .append($('<td>')
                                    .text(($('[name="radModApp"]:checked').val() == 'category' ? vData[i].CategoryName : vData[i].ProductNo + ' - ' + vData[i].Name))
                                )
                                .append($('<td>')
                                    .data('data', vData[i])
                                    .css({ textAlign: 'center' })
                                    .append($('<button data-role="none" class="btn btn-danger">')
                                        .append($('<span class="bi-trash">'))
                                    )
                                    .click(function (e) {
                                        var data = $(this).data('data');
                                        NewMessageBox('Are you sure you want to delete ' + (data.CategoryName || data.Name) + '?', 'Message', [
                                            {
                                                Text: 'Yes', Click: function () {
                                                    if (data.State == 'new') {
                                                        (arrProductCategories).splice(CInt2(data.Index), 1)
                                                    } else {
                                                        arrProductCategories[CInt2(data.Index)].State = 'remove';
                                                    }
                                                    LoadCategoryList(arrProductCategories);
                                                    //mModel.ID = modifierData.ID;
                                                    //mModel.DeleteCategories({
                                                    //    CategoryNames: [catname],
                                                    //    success: function (vData) {
                                                    //        //console.log('js75TJK9NASM! success');
                                                    //        $('#panMessageBox').modal('hide');
                                                    //        $('#divCancelMsgBox').hide();
                                                    //        GetData();
                                                    //    },
                                                    //    error: function (vData) {
                                                    //        console.log('jsZSRAGX3PZ6! ' + JSON.stringify(vData));
                                                    //    }
                                                    //});
                                                }
                                            }, { Text: 'No' }
                                        ])
                                    })
                                )
                            )
                        }                        
                    }

                    //$('.delete').button().button('refresh');
                }
                else {
                    pLV.append($('<tr>')
                                    .append($('<td colspan="3">')
                                        .text('No Result(s) Found')
                                    )
                        )
                }
            }

            function LoadRebatesList(vData) {
                var pLV = $('#tBodyItemsRebates');
                ctrRebateDeleted = vData.length;
                pLV.empty();

                if (vData.length > 0) {
                    for (var i = 0; i < vData.length; i++) {
                        vData[i].Index = i;
                        if(vData[i].State != 'remove'){
                            pLV.append($('<tr>')
                                .data('data', vData[i])
                                .css({ cursor: 'pointer' })
                                //.append($('<td>')
                                //    .text(CDec2(vData[i].UnitQuantity))
                                //)
                                .append($('<td>')
                                    .text(vData[i].UnitMeasure)
                                )
                                .append($('<td>')
                                    .text('(' + vData[i].ProductNo + ') ' + vData[i].ProductName)
                                )
                                .append($('<td>')
                                    .text(CDec2(vData[i].Quantity))
                                )
                                .append($('<td>')
                                    .text(CDec2(vData[i].Amount))
                                )
                                .append($('<td>')
                                    .data('data', vData[i])
                                    .attr('id', vData[i].ProductID)
                                    .attr('prodname', vData[i].ProductName)
                                    .append($('<button data-role="none" class="btn btn-danger">')
                                        .append($('<span class="bi-trash">'))
                                    )
                                    .click(function (e) {
                                        e.stopPropagation();
                                        var productname = $(this).attr('prodname');
                                        var data = $(this).data('data');
                                        NewMessageBox('Are you sure you want to delete "' + productname + '" ?', 'Message', [
                                        {
                                            Text: 'Yes', Click: function () {
                                                if (data.State == 'new') {
                                                    (arrRebates).splice(CInt2(data.Index), 1)
                                                } else {
                                                    arrRebates[CInt2(data.Index)].State = 'remove';
                                                }                                                
                                                LoadRebatesList(arrRebates);
                                                ctrRebateDeleted -= 1;
                                                //mModel.ID = modifierData.ID;
                                                //mModel.DeleteRebates({
                                                //    ProductIDs: [id],
                                                //    success: function (vData) {
                                                //        //console.log('jsAPYRBZR9CX! success');
                                                //        $('#panMessageBox').modal('hide');
                                                //        $('#divCancelMsgBox').hide();
                                                //        GetData();
                                                //    },
                                                //    error: function (vData) {
                                                //        console.log('jsK5K86TCWYF! ' + JSON.stringify(vData));
                                                //    }
                                                //});
                                            }
                                        }, { Text: 'No' }
                                        ])
                                    })
                                )
                                .data('data', vData[i])
                                .click(function (e) {
                                    ClearRebatesFields();
                                    isEditRebate = true;
                                    var data = $(this).data('data');
                                    $('#btnSKUName').data('data', data);
                                    $('#txtSelectedSKUID').val(data.ProductID);
                                    $('.txtSKUName').text(data.ProductName);
                                    $('#txtIMReturnUnitQuantity').val(CDec2(data.UnitQuantity));
                                    $('#txtIMReturnUnitMeasure').val(data.UnitMeasure);
                                    $('#txtIMReturnQuantity').val(CDec2(data.Quantity));
                                    $('#txtIMRebatesAmount').val(CDec2(data.Amount));
                                    $('#panIMRebates').modal('show');
                                    $('#divReturnedItem').show();
                                    $('#divSearchSKUToReturn').hide();
                                    RepositionPopup();
                                })
                        )
                        }
                    }
                }
                else {
                    pLV.append($('<tr>')
                                    .append($('<td colspan="6">')
                                        .css('text-align', 'center')
                                        .text('No Result(s) Found')
                                    )
                        )
                }
            }

            function SearchProductCategory(data) {
                var pLV = $('#lstOfProductCategory');

                pLV.empty();

                App.model.ItemModifiers.CategorySearch({
                    Keywords: data,
                    success: function (vData) {
                        if (vData.Results.Rows.length > 0) {
                            for (var i = 0; i < vData.Results.Rows.length; i++) {
                                pLV.append($('<li>')
                                                .append($('<a>')
                                                    .text(vData.Results.Rows[i].Name)
                                                )
                                                .data('data', vData.Results.Rows[i])
                                                .click(function (e) {
                                                    var Name = $(this).data('data').Name;
                                                    var list = $(this).closest('li').children('div');
                                                    $('#divErrorProductCategory').hide();

                                                    if (isSearchNew) {
                                                        if (arrProductCategories.length > 0) {
                                                            var isFound = $.grep(arrProductCategories, function (val) {
                                                                if (val == Name) return true;
                                                            });

                                                            if (isFound.length > 0) {
                                                                $('#divErrorProductCategory').html(Name + ' was already selected.<br><br>').show();
                                                                return false;
                                                            }
                                                        }
                                                    }

                                                    if (list.hasClass('selectedList')) {
                                                        list.removeClass('selectedList');
                                                        arrProductCategories = $.grep(arrProductCategories, function (val) {
                                                            return val != Name;
                                                        })
                                                    }
                                                    else {
                                                        list.addClass('selectedList');
                                                        arrProductCategories.push(
                                                            Name
                                                        )
                                                    }
                                                })
                                    )
                            }
                        }
                        else {
                            pLV.append($('<li>')
                                            .append($('<a>')
                                                .text('No Result(s) Found')
                                            )
                                )
                        }

                        pLV;//.listview('refresh');
                        RepositionPopup();
                    }
                });
            }

            function SearchPIMProductPopUp(s) {
                BrowsePopup('LibraryPIMProduct', 'Product', [
                    {
                        Message: 'Search SKU to be returned',
                        DisplayText: [{ Column: 'ProductName' }],
                        Click: function (e) {
                            $('#btnSKUName').data('data', e);
                            $('#txtSelectedSKUID').val(e.ID);
                            $('.txtSKUName').text(e.ProductName);
                            SKUUnitOfMeasure();
                        }
                    }
                ])
                return;
                var pData = {};

                pData.keywords = s;

                $('#lstOfSKU').empty();

                AjaxSend({
                    url: 'Browser/LibraryPIMProduct',
                    data: pData,
                    success: function (vData) {

                        if (vData.Results.Rows.length > 0) {
                            for (var p = 0; p < vData.Results.Rows.length; p++) {
                                $('#lstOfSKU').append($('<li>')
                                                .append($('<a>')
                                                    .attr('ID', vData.Results.Rows[p].ID)
                                                    .append($('<br><p style="font-weight:bold;">')
                                                        .text(vData.Results.Rows[p].ProductName)
                                                    )
                                                    .data('data', vData.Results.Rows[p])
                                                    .click(function (e) {
                                                        $('#txtSelectedSKUID').val($(this).attr('ID'));
                                                        $('.txtSKUName').text($(this).data('data').ProductName);
                                                        $('#divSearchSKUToReturn').hide();
                                                        $('#divSKUUnitMeasure').show();
                                                        $('.txtSKUName').text($(this).data('data').ProductName);
                                                        SKUUnitOfMeasure($(this).data('data').ID);
                                                        RepositionPopup();
                                                    })
                                                ));
                            }
                        }
                        else {
                            $('#lstOfSKU').append($('<li>').text('No Result(s) Found'));
                        }

                        $('#lstOfSKU');//.listview('refresh');
                        RepositionPopup();
                    }
                });
            }

            function SKUUnitOfMeasure(s) {
                BrowsePopup('LibraryPIMProductMeasure', 'Unit of Measure', [
                    {
                        Message: 'Please choose the typical unit of measure for buying the <b>' + $('.txtSKUName:first').text() + '</b>',
                        DisplayText: [{ Column: 'Measurement' }],
                        Click: function (e) {
                            $('#txtIMReturnUnitMeasure').val(e.Measurement);
                            GetPIMData();
                        },
                        WithSearchbox: false,
                        AutoSearch: true,
                        AddButton: [{
                            Text: 'Back',
                            Click: function () {
                                SearchPIMProductPopUp();
                            }
                        }]
                    }
                ], { ProductID: $('#txtSelectedSKUID').val() })
                return;
                var pData = {};

                pData.ProductID = s;

                $('#radSKUUnitMeasure').empty();

                AjaxSend({
                    url: 'Browser/LibraryPIMProductMeasure',
                    data: pData,
                    success: function (vData) {

                        if (vData.Results.Rows.length > 0) {
                            for (var p = 0; p < vData.Results.Rows.length; p++) {
                                $('<input type="radio" name="radSKUUnitMeasure" value="' + vData.Results.Rows[p].Measurement + '" id="radio-choice-' + p + '"><label for="radio-choice-' + p + '">' + vData.Results.Rows[p].Measurement + '</label>').appendTo("#radSKUUnitMeasure");
                                $("div").trigger('create');
                            }

                            if (vData.Results.Rows.length == 1)
                                $("input[name='radSKUUnitMeasure']:first").attr("checked", true);//.checkboxradio("refresh");
                        }
                        else {
                            $('<span>No Unit Measure found of the selected SKU</span>').appendTo('#radSKUUnitMeasure');
                        }
                        RepositionPopup();
                    }
                });
            }

            function GetPIMData() {
                BrowsePopup('ItemModifiers/GetPimData', 'Price', [
                    {
                        Message: 'Select a price for <b>' + $('.txtSKUName:first').text() + '</b>',
                        DisplayText: [{
                            Column: function (e) {
                                return e.PriceLevel + ' - ' + e.DiscountedPrice
                            }
                        }],
                        Click: function (e) {
                            $('#txtIMReturnUnitQuantity').val(e.Quantity);
                            $('#txtIMRebatesAmount').val(e.DiscountedPrice);
                        },
                        ReturnVariable: 'data.Rows',
                        WithSearchbox: false,
                        AutoSearch: true,
                        AddButton: [{
                            Text: 'Back',
                            Click: function () {
                                SKUUnitOfMeasure();
                            }
                        }],
                        Close: function () {
                            $('#panIMRebates').modal('show');
                            $('#divReturnedItem').show();
                        }
                    }
                ], { ProductID: $('#txtSelectedSKUID').val() })
            }

            function ClearRebatesFields() {
                $('#txtRebateID').val('');
                $('#txtSelectedSKUID').val('');
                $('.txtSKUName').text('');
                $('#txtIMReturnUnitQuantity').val('');
                $('#txtIMReturnUnitMeasure').val('');
                $('#txtIMReturnQuantity').val('');
                $('#txtIMRebatesAmount').val('');
            }

            function RepositionPopup() {
                $('div[data-role="popup"]').popup().popup('reposition', { positionTo: 'window' });
                $('div[data-role="popup"]').find('input[type="text"]:first').focus();
            }

        }) // end $(document)        

    }) // end $(function)

}) // end require