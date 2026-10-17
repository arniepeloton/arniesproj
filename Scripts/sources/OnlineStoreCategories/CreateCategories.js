require(['common',
    //'/Scripts3/Browser/LibraryPIMProduct.js?crc=',
    '/Scripts/jquery-ui-sortable.js',
    'bootstrap-tooltip'
    ], function () {

        $(function () {

            $(document).ready(function () {
                var divProductGroup = $('#divProductGroup');
                var txtName = $('#txtStoreCatName_ProductGroup');
                var divProducts = $('#divProducts_ProductGroup');
                var divGroups = $('#divGroups_ProductGroup');
                var chkActive = $('#chkActive_ProductGroup');
                var mParentID;
                var mID;
                var mNewMode;
                var mProducts = [];
                var isIE = false || !!document.documentMode;
                var currentCell, currentRowIndex = 0; //currentRowIndex is the current active row [blue border]

                var arrHotButtons = [];
                var ctrFeature = 0;

                GetMenuTemplateName();
                divProductGroup.hide();
                GetData();

                window.CreateCategories = {
                    GetProductGroupData: function () {
                        currentRowIndex = 0;
                        GetMenuTemplateName();
                        divProductGroup.hide();
                        GetData();
                    }
                }

                function LoadBranch(vParentDiv, vData, level) {

                    var pDiv;
                    var pDivGroup;
                    var pIndex;

                    pDiv = $('#divProductGroup').clone();
                    pDiv.find('[data-role=collapsible_clone]')
                    .collapsible({
                        collapsed: false,
                        contentTheme: $.mobile.page.prototype.options.contentTheme
                    });
                    if (vParentDiv == undefined) {
                        divProductGroup.after(pDiv);
                    } else
                        vParentDiv.append(pDiv);


                    if (level == undefined)
                        level = 0;


                    if (vData.Data != undefined) {
                        pDiv.find('h3:first').find('.ui-btn-text:first').text(vData.Data.Name);

                        if (level < 3) {
                            pDiv.find('[name=btnAddGroupFirst]').hide();
                            pDiv.find('[name=btnAddGroup]').show();
                            pDiv.find('[name=btnAddProduct]').hide();
                            pDiv.find('[name=btnAddGroup]').attr('id', 'btnAddGroup_' + vData.Data.ID);

                            $('#btnStoreCatAddGroup_' + vData.Data.ID).click(function () {

                                mParentID = vData.Data.ID;
                            });

                        } else {
                            pDiv.find('[name=btnAddGroupFirst]').hide();
                            pDiv.find('[name=btnAddGroup]').hide();
                            pDiv.find('[name=btnAddProduct]').show();
                            pDiv.find('[name=btnAddProduct]').attr('id', 'btnAddProduct_' + vData.Data.ID);

                            $('#btnStoreCatAddProduct_' + vData.Data.ID).click(function () {

                                mParentID = vData.Data.ID;

                            });


                            $('#btnStoreCatDelete_' + vData.Data.ID).click(function () {
                                mParentID = vData.Data.ID;

                                DeleteItem(mParentID, 'group');
                                //          
                            });
                            //FOR PRODUCTS
                            //                        pDiv.find('[name=btnAddGroup]').hide();
                            //                        pDiv.find('[name=btnDelete]').show();
                            //                        pDiv.find('[name=btnDelete]').attr('id', 'btnDelete_' + vData.Data.ID);

                            //                        $('#btnStoreCatDelete_' + vData.Data.ID).click(function () {
                            //                            mParentID = vData.Data.ID;

                            //                            DeleteItem(mParentID, "single");
                            //          
                            //                        }); // end Click Function
                        }
                    } else {
                        pDiv.find('[name=btnAddGroupFirst]').show();
                        pDiv.find('[name=btnAddGroup]').hide();
                        pDiv.find('[name=btnAddProduct]').hide();
                        pDiv.find('[name=btnAddGroupFirst]').attr('id', 'btnAddGroup_undefined');

                        $('#btnStoreCatAddGroup_undefined').click(function () {

                            mParentID = null;

                        });
                    } // end if vdata undefined


                    pDiv.find('[name=butAddSubgroup_ProductGroup]:first').click(function () {
                        MessageBox('code for adding groups');
                    });

                    pDiv.find('[name=butAddProduct_ProductGroup]:first').click(function () {
                        MessageBox('code for adding groups');
                    });


                    if (vData.Subgroups != undefined) {
                        level = level + 1; //increment by the number of subgroups

                        pDivGroup = pDiv.find('[name=divSubgroups_ProductGroup]');
                        for (pIndex = 0; pIndex <= vData.Subgroups.length - 1; pIndex++) {
                            LoadBranch(pDivGroup, vData.Subgroups[pIndex], level);
                        }; // END for pIndex
                    };

                    if (vData.Products != undefined) {

                        pDivGroup = pDiv.find('[name=divProductList_ProductGroup]');
                        LoadProducts(pDivGroup, vData.Products);
                    };


                    pDiv.show();


                }; // END LoadBranch

                $('body').on('click', '#btnStoreCatEditName', function (e) {
                    e.preventDefault();

                    $('#hdrTemplateName').hide();
                    $('#divEditName').show();
                    $('#txtStoreCatEditName').val($('#hdrTemplateName').text());
                    $('#txtStoreCatEditName').focus();

                    $('#btnStoreCatSaveName').show();
                    $('#btnStoreCatEditName').hide();
                    $('#btnStoreCatCancel').show();
                    $('#btnStoreCatDeleteTemplate, #btnStoreCatGenerateHB').hide();
                })
                .on('click', '#btnStoreCatCancel', function (e) {
                    e.preventDefault();

                    $('#hdrTemplateName').show();
                    $('#divEditName').hide();
                    $('#txtStoreCatEditName').val('');

                    $('#btnStoreCatSaveName').hide();
                    $('#btnStoreCatEditName').show();
                    $('#btnStoreCatCancel').hide();
                    $('#btnStoreCatDeleteTemplate, #btnStoreCatGenerateHB').show();
                })
                .on('click', '#btnStoreCatSaveName', function (e) {
                    e.preventDefault();

                    if ($.trim($("#txtStoreCatEditName").val()) == "") {
                        NewMessageBox("Please enter a template name.",
                                "Template Name",
                                [
                                    { Text: 'Ok' }
                                ]
                    );
                        $("#txtStoreCatEditName").focus();
                        return;
                    }

                    SaveTemplateName();
                })
                .on('click', '#btnStoreCatAssignToFOWST', function (e) {
                    e.preventDefault();

                    location = MergeUrlParameters({
                        OrgID: sessionStorage.OrgID,
                        MenuID: sessionStorage.ViewID, //add menu id to query string
                        IsMobile: CBool(IsMobile) ? 1 : 0
                    }, '/OnlineStoreCategories/AssignFOWST');
                })
                .on('click', '#btnStoreCatDeleteTemplate', function (e) {
                    e.preventDefault();

                    DeleteItem(sessionStorage.ViewID, "template");
                })
                .on('click', '#btnStoreCatHotButtonsTemplateDone', function (e) {
                    e.preventDefault();
                    window.location.hash = 'LibraryPage';
                    //                location = MergeUrlParameters({
                    //                    OrgID: sessionStorage.OrgID,
                    //                    IsMobile: CBool(IsMobile) ? 1 : 0
                    //                }, '/OnlineStoreCategories');
                })
                .on('click', '#butStoreCatSave_ProductGroup', function (e) {
                    e.preventDefault();
                    if ($.trim(txtName.val()) == '') {
                        NewMessageBox('Please enter a name for this group.', 'New Group',
                            [
                                {
                                    Text: 'Ok',
                                    Click: function () {
                                        $('#panSearchLibrary').modal('hide');
                                        $('#popupStoreCatNewGroup').modal('show');
                                        $("#txtStoreCatName_ProductGroup").focus();
                                    }
                                }
                            ]
                        )
                        return;
                    } else {

                        var mMenuID = sessionStorage.ViewID;
                        console.log({
                            ID: mID,
                            ParentID: mParentID,
                            MenuID: mMenuID,
                            Name: $.trim(txtName.val()),
                            Active: 1 // CDec2(chkActive.prop('checked'))
                        })
                        //return 

                        AjaxSend({
                            url: '/OnlineStoreCategories/SaveGroup',
                            data: {
                                ID: mID,
                                ParentID: mParentID,
                                MenuID: mMenuID,
                                Name: $.trim(txtName.val()),
                                Active: 1 // CDec2(chkActive.prop('checked'))
                            },
                            success: function (vData) {

                                if (vData.Result == 'ok') {
                                    //mID = vData.ID;
                                    //mParentID = vData.ParentID;

                                    txtName.val('');

                                    $('#popupStoreCatNewGroup').modal('hide');
                                    GetData();
                                    //                                $('#UniversalDiv').html('');
                                    //                                LibraryTable.ViewHotButtonPage();
                                    //location.reload();                                    
                                } else if (vData.Error != undefined) {
                                    NewMessageBox(vData.Error, 'Error');
                                    txtName.val('');
                                }
                            }
                        });
                    }
                })
                .on('click', '#butStoreCatAddProduct_ProductGroupFirst', function (e) {
                    $('#popupStoreCatMenuFirst').modal('hide');
                    $('#lstStoreCatOfLibrary').empty();
                    $('#TitleLibrary').text('PIM');
                    $('#txtStoreCatSearchKeyword').val('').focus().attr('placeholder', 'Search name of product');
                    $('#panSearchLibrary').modal('show');
                })
                .on('click', '#butStoreCatAddProduct_ProductGroup', function (e) {
                    $('#popupStoreCatMenu').modal('hide');
                    $('#lstStoreCatOfLibrary').empty();
                    $('#TitleLibrary').text('PIM');
                    $('#txtStoreCatSearchKeyword').val('').focus().attr('placeholder', 'Search name of product');
                    $('#panSearchLibrary').modal('show');
                })
                .on('click', '#butStoreCatAddProduct_ProductSKU', function (e) {
                    Browser.LibraryPIMProduct.Click = function (e, vOpt) {

                        mProducts.push({
                            ProductID: e.ID,
                            ProductName: e.ProductName
                        });
                        //                    alert('hi');
                        //                    $('#popupStoreCatNewSKUName').modal('show');

                        SaveProduct(e.ID, e.ProductName);

                    };
                    Browser.LibraryPIMProduct.Show();
                })
                .on('click', '#butStoreCatDelete_ProductGroup', function (e) {
                    DeleteItem(mParentID, 'group');
                })
                .on('click', '#butStoreCatDelete_ProductSKU', function (e) {
                    DeleteItem(mParentID, 'group');
                })
                .on('click', '#butStoreCatSave_SKU', function (e) {
                    e.preventDefault();

                    if ($.trim($("#txtStoreCatName_SKU").val()) == "") {
                        NewMessageBox('Please enter a short name', 'SKU Short Name',
                            [
                                {
                                    Text: 'Ok',
                                    Click: function () {
                                        $('#panSearchLibrary').modal('hide');
                                        $('#popupStoreCatNewSKUName').modal('show');
                                        $("#txtStoreCatName_SKU").focus();
                                    }
                                }
                            ]
                        )
                        return;
                    }

                    //SaveProductName();
                    SaveProduct($('#hdnStoreCatSKUid').val(), $('#lblStoreCatFullProductName').text());
                })
                .on('keyup', '#txtStoreCatSearchKeyword', function (e) {
                    if (e.keyCode == 13) {
                        if ($('#TitleLibrary').text() == "PIM") {
                            SearchPIM($(this).val());
                        }
                    }
                })
                .on('click', '#tBodyStoreCatHotButtons .tableCell', function () {
                    $('#tBodyStoreCatHotButtons tr td').removeClass('activeCell');
                    $('#tBodyStoreCatHotButtons tr').removeClass('activeRow');
                    $('#tBodyStoreCatHotButtons tr').removeClass('rowSelected');
                    $(this).addClass('activeCell');
                    $(this).parent().addClass('activeRow');
                    $('.context-menu').remove();
                })
                .on('click', '#btnStoreCatAddHotButton', function () {
                    //currentCell = $('#tBodyStoreCatHotButtons tr td.activeCell').find('div');
                    //currentRowIndex = currentCell.closest('tr').index();
                    //var celltype = currentCell.attr('celltype');
                    //if (celltype == "home") {
                        $('#btnStoreCatAddHotButtonGroup').show();
                        $('#btnStoreCatAddHotButtonSKU').show();
                        $('#btnStoreCatDeleteHotButtonSKUGroup, #btnStoreCatEditHotButtonSKUGroup').hide();
                    //}
                    //else if (celltype == "group") {
                    //    $('#btnStoreCatAddHotButtonGroup').show();
                    //    $('#btnStoreCatAddHotButtonSKU').show();
                    //    $('#btnStoreCatDeleteHotButtonSKUGroup, #btnStoreCatEditHotButtonSKUGroup').show();

                    //    if (currentCell.attr('level') == "3") {
                    //        $('#btnStoreCatAddHotButtonGroup').hide();
                    //    }
                    //}
                    //else if (celltype == "sku") {
                    //    $('#btnStoreCatAddHotButtonGroup').hide();
                    //    $('#btnStoreCatAddHotButtonSKU').hide();
                    //    $('#btnStoreCatDeleteHotButtonSKUGroup, #btnStoreCatEditHotButtonSKUGroup').show();
                    //}
                    //$('#lstStoreCatMenuHotButton').listview('refresh');
                    //$('#popupStoreCatMenuHotButton').modal('show');

                    PopupButtonList('', 'Options', [
                        {
                            Text: 'Add Group', Click: function () {
                                $('#btnStoreCatAddGroup').click();
                            }
                        }, {
                            Text: 'Add SKU', Click: function () {
                                $('#btnStoreCatAddSku').click();
                            }
                        }
                    ])
                })
                .on('click', '#btnStoreCatAddHotButtonGroup', function () {
                    mParentID = currentCell.attr('id');
                })
                .on('click', '#btnStoreCatAddHotButtonSKU', function () {
                    mParentID = currentCell.attr('id');
                    BrowsePopup('LibraryPIMProduct', 'PIM',
                        [
                            {
                                DisplayText: [{ Column: 'ProductName' }, { Label: 'Retail Price', Column: 'RetailPrice'}],
                                Click: function (e) {
                                    mProducts.push({
                                        ProductID: e.ID,
                                        ProductName: e.ProductName
                                    });
                                    //SaveProduct(e.ID, e.ProductName); do not save to db immediately. need to set the short name first. commented by arnie 7-6-17
                                    $('#lblStoreCatFullProductName').text(e.ProductName);
                                    $('#popupStoreCatNewSKUName').modal('show');                                    
                                    $('#hdnStoreCatSKUid').val(e.ID);
                                    $('#txtStoreCatName_SKU').val((e.ProductName).substring(0, 23)).focus();

                                }
                            }
                        ]
                    )
                })
                .on('click', '#btnStoreCatDeleteHotButtonSKUGroup', function () {
                    var celltype = currentCell.attr('celltype');
                    $('#popupStoreCatMenuHotButton').modal('hide');
                    if (celltype == "home" || celltype == "group") {
                        DeleteItem(currentCell.attr('id'), 'group')
                    }
                    else if (celltype == "sku") {
                        DeleteItem(currentCell.attr('id'), 'sku')
                    }
                })
                .on('click', '#btnStoreCatEditHotButtonSKUGroup', function () {
                    $('#popupStoreCatMenuHotButton').modal('hide');
                    $('#popupStoreCatEditSKUName').modal('show');
                    var txt = $.trim(currentCell.contents().not(currentCell.children()).text().split('(')[0]);
                    $('#lblStoreCatEditFullProductName').text(txt);
                    $('#txtStoreCatEditName_SKU').val(txt).focus();
                    PopupReposition();
                })
                .on('click', '.btnClosePopup', function () {
                    NewMessageBoxClose();
                })
                .on('click', '#butStoreCatEditSave_SKU', function () {
                    var txtEditName_SKU = $.trim($('#txtStoreCatEditName_SKU').val())

                    if(txtEditName_SKU == ''){
                        WebToast('Please enter new short name', 'top');
                        return;
                    }

                    AjaxSend({
                        url: '/OnlineStoreCategories/RenameShortName',
                        data: { Name: txtEditName_SKU, ID: currentCell.attr('id'), Type: currentCell.attr('celltype') },
                        success: function (e) {
                            if(e.Results == 'saved'){
                                GetData(function () {
                                    $('#tBodyStoreCatHotButtons tr td').removeClass('activeCell');
                                    $('#tBodyStoreCatHotButtons tr').removeClass('activeRow rowSelected');
                                    $('#tBodyStoreCatHotButtons tr:eq(' + currentRowIndex + ')').addClass('activeRow');
                                    $('#tBodyStoreCatHotButtons tr:eq(' + currentRowIndex + ') > td').addClass('activeCell');
                                });
                                $('#popupStoreCatEditSKUName').modal('hide');                                
                            } else {
                                NewMessageBox(e.Results, 'Message');
                            }
                        }
                    })
                })
                .on('click', '#btnStoreCatGenerateHB', function () {
                    NewMessageBox('This will automatically generate store categories. Click "Generate Now" to continue.', 'Message', [
                        {
                            Text: 'Generate Now', Click: function () {
                                NewMessageBox('Please wait while generating store categories ... please do not refresh the page ', 'Message');

                                AjaxSend({
                                    url: '/OnlineStoreCategories/GenerateHB',
                                    data: {
                                        MenuID: sessionStorage.ViewID
                                    },
                                    success: function (e) {
                                        NewMessageBoxClose();
                                        if (e.Results == 'saved') {
                                            GetData();
                                            WebToast('Successfully generated hot buttons. <br><br> Refreshing list, please wait ...', 'top');
                                        } else {
                                            NewMessageBox(e.Results, 'Message');
                                        }
                                    }
                                })
                            }
                        }, { Text: 'Cancel' }
                    ])                    
                })

                .on('click', '#btnStoreCatAddGroup', function () {
                    //$('#popupStoreCatMenuHotButton').modal('hide');
                    $('#popupStoreCateNewAddGroup').modal('show');
                    $('#txtStoreCatGroupName').val('').focus();
                    $('#chbStoreCategoryFeature').removeAttr('checked')////.checkboxradio('refresh');
                    $('#divCategoryFeature').show();

                    $('#btnStoreCatOkGroupName').unbind('click').click(function () {
                        var txtGroupName = $.trim($('#txtStoreCatGroupName').val());
                        var isFeature = $('#chbStoreCategoryFeature').is(':checked');

                        if (txtGroupName == '') {
                            WebToast('Please enter group name', 'top');
                            return;
                        }

                        if(ctrFeature > 6 && isFeature){
                            WebToast('We only limit to 7 feature categories (<span class="bi-star-fill" style="color:#ffc624;"></span>). You can uncheck the feature category of the groups.');
                            return;
                        }

                        var data = {
                            ID: NewID(),
                            Name: txtGroupName,
                            OrderNo: 0,
                            State: 'new',
                            IsGroup: true,
                            SubGroup: [],
                            IsActive: true,
                            Level: 1,
                            IsFeature: isFeature
                        }
                        arrHotButtons.push(data);
                        GenerateList(data, $('#lstStoreCatHotButtonList'));
                        $('#popupStoreCateNewAddGroup').modal('hide');

                        if (CBool(isFeature)) {
                            ctrFeature += 1;
                        }
                    })

                    $('#txtStoreCatGroupName').unbind('keydown').keydown(function (e) {
                        if(e.which == 13){
                            $('#btnStoreCatOkGroupName').click();
                        }
                    })
                })
                .on('click', '#btnStoreCatAddSku', function () {
                    BrowseSkus(function (e) {
                        RenameSelectedSkus(e, arrHotButtons, $('#lstStoreCatHotButtonList'));
                    })
                })
                .on('click', '#btnStoreCatSaveHB', function () {
                    $('#btnStoreCatSaveHB').attr('disabled', true);
                    AjaxSend({
                        url: '/OnlineStoreCategories/SaveHotButtons',
                        data: { MenuID: sessionStorage.ViewID, HotButtons: arrHotButtons, IsDefault: $('#chbStoreCategoriesIsDefault').is(':checked') },
                        success: function (e) {
                            $('#btnStoreCatSaveHB').removeAttr('disabled');
                            if (e.Results == 'saved') {
                                WebToast('Successfully saved', 'top');
                                LibraryTable.OnlineStoreCategories();
                                window.location.hash = 'LibraryPage';
                            } else {
                                NewMessageBox(e.Results, 'Message');
                            }
                        }
                    })
                })

                function BrowseSkus(f) {
                    BrowsePopup('LibraryPIMProduct', 'SKU',
                        [
                            {
                                StyleFormat: 'table',
                                Message: 'Note: You can select multiple records.',
                                SelectMultiple: true,
                                DisplayText: [{ Column: 'ProductName' }, { Label: 'Product No.', Column: 'ProductNo' }, {
                                    Label: 'Retail Price', Column: function (e) {
                                        return AccountingFormat(e.RetailPrice)
                                    }
                                }],
                                Click: function (e) {
                                    f(e);
                                }
                            }
                        ]
                    )
                }

                function RenameSelectedSkus(arrSkus, data, container) {
                    var tBodySelectedSkus = $('#tBodyStoreCatSelectedSkus'), isSelectedSKUExist = true;

                    tBodySelectedSkus.empty()

                    for (var i = 0; i < arrSkus.length; i++) {
                        var isExist = false;

                        $.grep(data, function (e) {
                            if(e.ProductID == arrSkus[i].ID){
                                isExist = true;
                                return false; // stop
                            }
                        })

                        if (isExist) {
                            continue; // skip; do not add existing record in the list
                        }

                        isSelectedSKUExist = false;

                        var shortname = arrSkus[i].ProductName //().substring(0, 23);
                        arrSkus[i].Index = i;
                        arrSkus[i].ShortName = shortname;
                        tBodySelectedSkus.append($('<tr>')
                                .append($('<td style="width:200px">')
                                    .append(arrSkus[i].ProductName)
                                )
                                .append($('<td>')
                                    .append($('<input data-role="none" class="form-control txtShortName">')
                                        .data('data', arrSkus[i])
                                        .val(shortname)
                                        .blur(function () {
                                            var data = $(this).data('data');
                                            data.ShortName = $.trim($(this).val());
                                        })
                                    )
                                )
                                .append($('<td>')
                                    .append($('<button data-role="none" class="btn custom-btn-theme-d btnCircle-xs">')
                                        .data('data', arrSkus[i])
                                        .append($('<span class="bi-trash">'))
                                        .click(function () {
                                            var data = $(this).data('data');
                                            var tr = $(this).parent().parent();

                                            NewMessageBox('Are you sure you want to remove this record?', 'Message', [
                                                {
                                                    Text: 'Yes', Click: function () {
                                                        (arrSkus).splice(data.Index, 1);
                                                        tr.remove();
                                                    }
                                                }, { Text: 'No' }
                                            ])
                                        })
                                    )
                                )
                            )
                    }

                    $('#btnStoreCatOkSelectedSkus').unbind('click').click(function () {
                        var arrSelectedSkus = []; arrShortName = [], isDuplicate = false;
                        $('.txtShortName').each(function () {
                            var data = $(this).data('data');
                            var val = $.trim($(this).val());

                            if (arrShortName.indexOf(val) > -1) {
                                isDuplicate = true;
                                WebToast('Found duplicate shortname. Please change the other shortname.', 'top');
                                return false;
                            } else {
                                arrShortName.push(val)
                                arrSelectedSkus.push({
                                    ID: NewID(),
                                    ProductID: data.ID,
                                    Name: data.ProductName,
                                    ShortName: val,
                                    IsGroup: false,
                                    State: 'new',
                                    OrderNo: 0,
                                    IsActive: true
                                })
                            }
                        })
                        
                        if(!isDuplicate){
                            for (var i = 0; i < arrSelectedSkus.length; i++) {
                                data.push(arrSelectedSkus[i]);
                                GenerateList(arrSelectedSkus[i], container)
                            }

                            $('#popupStoreCatHBSelectedSkus').modal('hide');
                        }
                    })

                    if (!isSelectedSKUExist) {
                        $('#popupStoreCatHBSelectedSkus').modal('show');
                    } else {
                        WebToast('All selected SKUs was already exist in the list', 'top');
                    }
                }

                function DisplayNewHB(data, container) {
                    Bs.Loading('show', 'Displaying data, please wait ...');
                    setTimeout(function () {
                        for (var i = 0; i < data.length; i++) {
                            if (CBool(data[i].IsFeature)) {
                                ctrFeature += 1
                            }
                            GenerateList(data[i], container)
                        }
                        Bs.Loading('hide');
                    }, 200)
                }

                function SortList(id) {
                    setTimeout(function (e) {
                        $('#' + e).sortable({
                            list: '#' + e,
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
                                    $(this).data('data').OrderNo = newOrderNo;
                                    newOrderNo += 1;
                                })
                            }
                        });
                        //$('#' + e).disableSelection();
                    }, 0, id)
                }

                function GenerateList(data, container) {

                    if(data.State == 'remove'){
                        return;
                    }

                    var div = $('<div style="display:flex; padding:10px !important;">')
                            .append($('<button data-role="none" class="btn btn-light btn-sm">').append($('<span class="bi-grip-vertical">')))
                            .append('&nbsp;')
                            .append($('<button data-role="none" class="btn btn-light btn-sm">').append($('<span id="name-' + data.ID + '">')
                                .append((CBool(data.IsGroup) ? '<b><span class="required">*</span> ' + data.Name + '</b>' : '(' + data.ShortName + ') ' + data.Name))
                            ))
                            .append('&nbsp &nbsp; &nbsp;')
                    var li = $('<li id="list-' + data.ID + '" class="list-group-item m-0 p-0">')
                                .data('data', data)
                                .append(div)

                    var dropdown = $('<div class="dropdown">')
                                    .append($('<button data-role="none" class="btn btn-secondary btn-sm" data-toggle="dropdown" id="dd-' + data.ID + '">')
                                        .append('<span class="bi-gear-fill"></span>')
                                        .unbind('click')
                                        .click(function (e) {
                                            e.preventDefault();
                                            e.stopPropagation();

                                            var isOpen = $(this).parent().hasClass('open');

                                            if(CBool(isOpen)){
                                                $(this).parent().removeClass('open');
                                            } else {
                                                $(this).parent().addClass('open');
                                            }
                                        })
                                    )

                    var options = $('<ul class="dropdown-menu" aria-labelledby="dd-' + data.ID + '">')
                                    

                    if (CBool(data.IsGroup)) { // for groups only

                        if(CInt2(data.Level) < 3){ // when in level 3, user must not allow to add another group 
                            options.append($('<li>')
                                        .append($('<a class="dropdown-item">').data('data', data).append('<span class="bi-plus" style="color: #16a085;"></span> &nbsp; Add Sub Group')
                                            .click(function (e) {
                                                var data = $(this).data('data');

                                                $('#popupStoreCateNewAddGroup').modal('show');
                                                $('#txtStoreCatGroupName').val('').focus();
                                                $('#chbStoreCategoryFeature').removeAttr('checked')//.checkboxradio('refresh');

                                                $('#divCategoryFeature').hide();

                                                $('#btnStoreCatOkGroupName').unbind('click').click(function () {
                                                    var txtGroupName = $.trim($('#txtStoreCatGroupName').val());

                                                    if (txtGroupName == '') {
                                                        WebToast('Please enter group name', 'top');
                                                        return;
                                                    }

                                                    var newdata = {
                                                        ID: NewID(),
                                                        Name: txtGroupName,
                                                        OrderNo: 0,
                                                        State: 'new',
                                                        IsGroup: true,
                                                        SubGroup: [],
                                                        IsActive: true,
                                                        Level: CInt2(data.Level) + 1,
                                                        IsFeature: false
                                                    }
                                                    data.SubGroup.push(newdata);
                                                    GenerateList(newdata, $('#ul-' + data.ID));
                                                    $('#popupStoreCateNewAddGroup').modal('hide');
                                                })

                                                $('#txtStoreCatGroupName').unbind('keydown').keydown(function (e) {
                                                    if (e.which == 13) {
                                                        $('#btnStoreCatOkGroupName').click();
                                                    }
                                                })
                                            })
                                        )
                                    )
                        }

                        options.append($('<li>')
                                        .append($('<a class="dropdown-item">').data('data', data).append('<span class="bi-plus" style="color: #16a085;"></span> &nbsp; Add Sub SKU')
                                            .click(function (e) {
                                                var data = $(this).data('data');
                                                BrowseSkus(function (e) {
                                                    RenameSelectedSkus(e, data.SubGroup, $('#ul-' + data.ID));
                                                })
                                            })
                                        )
                                    )
                    }

                    options.append($('<li>')
                                        .append($('<a class="dropdown-item" href="#">').data('data', data).append('<span class="bi-pencil-square" style="color: #f1c40f;"></span> &nbsp; Edit')
                                            .click(function (e) {
                                                var data = $(this).data('data');
                                                $('#popupStoreCatRenameHB').modal('show');
                                                $('#txtStoreCatCurrentHBName').text(data.Name);
                                                $('#txtStoreCatRenameHB').val((CBool(data.IsGroup) ? data.Name : data.ShortName)).focus();

                                                if(data.Level == 1){
                                                    $('#divStoreCategoryFeature').show();
                                                } else {
                                                    $('#divStoreCategoryFeature').hide();
                                                }

                                                if(CBool(data.IsGroup)){
                                                    $('#chbEditStoreCategoryFeature').prop('checked', CBool(data.IsFeature))//.checkboxradio('refresh');
                                                }

                                                $('#btnStoreCatOkRename').unbind('click').click(function () {
                                                    var val = $.trim($('#txtStoreCatRenameHB').val());
                                                    var isFeature = $('#chbEditStoreCategoryFeature').is(':checked');

                                                    if(val == ''){
                                                        WebToast('Please enter new name', 'top');
                                                        return;
                                                    }                                                    

                                                    if (CBool(data.IsGroup)) {
                                                        if (CBool(data.IsFeature)) { // if already a feature category
                                                            if (!isFeature) { // if remove as feature category
                                                                ctrFeature -= 1;
                                                                $('#star-' + data.ID).remove();
                                                            }
                                                        } else {
                                                            if (isFeature) { // if tagged as feature category
                                                                if (ctrFeature > 6) {
                                                                    WebToast('We only limit to 7 feature categories (<span class="bi-star-fill" style="color:#ffc624;"></span>). You can uncheck the feature category of the groups.');
                                                                    return;
                                                                }
                                                                ctrFeature += 1;
                                                                $('#list-' + data.ID + ' > div').append('&nbsp;').append($('<span id="star-' + data.ID + '"  title="Tagged as Feature Category" class="bi-star-fill" style="color:#ffc624;">'));
                                                            }
                                                        }

                                                        data.Name = val;
                                                        data.IsFeature = $('#chbEditStoreCategoryFeature').is(':checked')
                                                    } else {
                                                        data.ShortName = val;
                                                    }
                                                    $('#name-' + data.ID).html((CBool(data.IsGroup) ? '<b><span class="required">*</span> ' + data.Name + '</b>' : '(' + data.ShortName + ') ' + data.Name));
                                                    $('#popupStoreCatRenameHB').modal('hide');
                                                })
                                            })
                                        )
                                    )
                                    .append($('<li>')
                                        .append($('<a class="dropdown-item">').data('data', data).append('<span class="bi-trash" style="color: #c1392b;"></span> &nbsp; Delete')
                                            .click(function (e) {
                                                var data = $(this).data('data');

                                                NewMessageBox('Are you sure you want to remove this record?', 'Message', [
                                                    {
                                                        Text: 'Yes', Click: function () {
                                                            data.State = 'remove';
                                                            $('#list-' + data.ID).remove();

                                                        }
                                                    }, { Text: 'No' }
                                                ])
                                            })
                                        )
                                    )

                    dropdown.append(options)
                    div.append(dropdown)                       

                    if (CBool(data.IsGroup)) {                        
                        div.append('&nbsp;').append($('<button data-role="none" stat="hide" class="btn btn-light btn-sm">')
                            .data('data', data)
                            .append('<span class="bi-caret-down-fill"></span>')
                            .click(function (e) {
                                e.preventDefault();
                                e.stopPropagation();

                                var stat = $(this).attr('stat');
                                var data = $(this).data('data');

                                if(stat == 'hide'){
                                    $(this).attr({
                                            'stat': 'show',
                                            'title': 'Hide sublist'
                                        })
                                        .find('> span').removeClass('bi-caret-down-fill').addClass('bi-caret-up-fill');
                                    $('#ul-' + data.ID).show();
                                } else {
                                    $(this).attr({
                                            'stat': 'hide',
                                            'title': 'Show sublist'
                                        })
                                        .find('> span').removeClass('bi-caret-up-fill').addClass('bi-caret-down-fill');
                                    $('#ul-' + data.ID).hide();
                                }

                                if (!CBool(data.IsFinishLoadData) && data.State != 'new') {
                                    $('#ul-' + data.ID).append($('<li>').append($('<a>').text('Getting products .... please wait')))

                                    AjaxSend({
                                        url: 'Ecommerce/GetSubGroup',
                                        data: {
                                            OrgID: sessionStorage.OrgID,
                                            MenuID: sessionStorage.ViewID,
                                            GroupID: data.ID,
                                            IncludeSubGroup: false,
                                            Level: CInt2(data.Level) + 1
                                        }, success: function (e) {
                                            data.SubGroup = $.merge(data.SubGroup, e.Results);
                                            data.IsFinishLoadData = true;

                                            $('#ul-' + data.ID).empty();

                                            for (var i = 0; i < data.SubGroup.length; i++) {
                                                GenerateList(data.SubGroup[i], $('#ul-' + data.ID))
                                            }
                                            //var container = $('<div class="yamm-content">');
                                            //GenerateSubGroupList(container, data.SubGroup, 2);
                                            //$('[catid="' + data.ID + '"]').empty().append($('<li>').append(container));
                                        }
                                    })
                                }
                            })
                        )

                        if (CBool(data.IsFeature)) {
                            div.append('&nbsp;').append($('<button data-role="none" class="btn btn-light btn-sm">').append($('<span id="star-' + data.ID + '"  title="Tagged as Feature Category" class="bi-star-fill" style="color:#ffc624;">')));
                        }

                        li.append($('<ul id="ul-' + data.ID + '" class="list-group draggableList" style="margin-left:35px; display:none;">').sortable({
                            list: '#ul-' + data.ID,
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
                                    $(this).data('data').OrderNo = newOrderNo;
                                    newOrderNo += 1;
                                })
                            }
                        }))

                        container.append(li)

                        for (var i = 0; i < data.SubGroup.length; i++){
                            GenerateList(data.SubGroup[i], $('#ul-' + data.ID))
                        }

                        //SortList('ul-' + data.ID);
                    } else {
                        container.append(li)
                    }                    
                }

                function ReloadPage() {
                    window.location.hash = '';
                    window.location.hash = 'UniversalPage';
                    AjaxSend({
                        ExactUrl: '/OnlineStoreCategories/MenuTemplate?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile,
                        type: 'GET',
                        dataType: 'text',
                        success: function (vData) {
                            $('#UniversalDiv').html('');
                            $('#UniversalDiv').append(vData).trigger('create');
                            ProductGroup.GetProductGroupData();
                            App.Deferred.fulfill('FQ26E5TP76');
                        }
                    });
                }

                function querySt(Key) {
                    var url = window.location.href;
                    KeysValues = url.split(/[\?&]+/);
                    for (i = 0; i < KeysValues.length; i++) {
                        KeyValue = KeysValues[i].split("=");
                        if (KeyValue[0] == Key) {
                            return KeyValue[1];
                        }
                    }
                }

                function SaveTemplateName() {
                    var data = {
                        ID: sessionStorage.ViewID,
                        Name: $.trim($('#txtStoreCatEditName').val())
                    };
                    AjaxSend({
                        url: '/OnlineStoreCategories/CreateTemplate',
                        data: data,
                        success: function (vData) {
                            if (vData.Result == 'ok') {
                                //location.reload();
                                GetMenuTemplateName();
                                $('#btnStoreCatCancel').click()
                            }
                            else {
                                MessageBox(vData.Error);
                            }
                        }
                    });
                }

                function GetData(callback) {
                    var menuID;

                    if (sessionStorage.ViewID) {
                        menuID = sessionStorage.ViewID;
                    } else {
                        menuID = '';
                    }

                    ClearFields();

                    Bs.Loading('show', 'Getting data, please wait ...');

                    AjaxSend({
                        url: '/OnlineStoreCategories/NewGetHotButtons',
                        data: { MenuID: sessionStorage.ViewID, IncludeSubGroup: false },
                        success: function (e) {
                            if ($.isArray(e.Results)) {
                                arrHotButtons = e.Results;
                                DisplayNewHB(e.Results, $('#lstStoreCatHotButtonList'));
                            } else {
                                NewMessageBox('There\'s an error encoutered while fetching the data', 'Message');
                                console.log(e.Results);
                            }
                        }
                    })

                    return
                    //this old HB design
                    if (menuID != '') {
                        var pData = {};
                        pData.MenuID = menuID;

                        AjaxSend({
                            url: '/OnlineStoreCategories/GetTree',
                            data: pData,
                            success: function (vData) {
                                //LoadBranch(null, vData);
                                LoadHotButtons(vData);

                                if ($.isFunction(callback)) {
                                    callback();
                                }
                            }
                        }); // END Ajax
                    } else {
                        MessageBox('Invalid ID');
                    }

                }; // END LoadData

                function LoadHotButtons(vData, level, txtIndent) {
                    var tbody = $('#tBodyStoreCatHotButtons');

                    if (level == undefined) {
                        level = 0;
                    }

                    if (vData.Data != undefined) {
                        if (level == 1) {
                            txtIndent = "30px";
                        }
                        else if (level == 2) {
                            txtIndent = "60px";
                        }
                        else if (level == 3) {
                            txtIndent = "90px";
                        }
                        else {
                            txtIndent = "120px";
                        }
                        tbody.append($('<tr>')
                                .append($('<td>')
                                    .append($('<div>')
                                        .css('text-indent', txtIndent)
                                        .attr('data-role', 'none')
                                        .attr('columnName', 'description')
                                        .attr('state', 'add')
                                        .attr('level', level)
                                        .attr('celltype', 'group')
                                        .addClass('font-bold')
                                        .attr('id', vData.Data.ID)
                                        .html('<span style="color:#F00; font-weight:bold;">*</span> ' + vData.Data.Name)
                                    )
                                    .addClass('tableCell')
                                )
                        )
                    }
                    else {
                        CreateTableHotButtons();
                    }

                    if (vData.Subgroups != undefined) {
                        level = level + 1;

                        for (var i = 0; i < vData.Subgroups.length; i++) {
                            LoadHotButtons(vData.Subgroups[i], level);
                        }
                    }

                    if (vData.Products != undefined) {
                        LoadHotButtonsProducts(vData.Products, (vData.Data == undefined ? undefined : $('#tBodyStoreCatHotButtons tr td div[id="' + vData.Data.ID + '"]').attr('level')))
                    }
                }

                function LoadHotButtonsProducts(vData, level) {
                    var tbody = $('#tBodyStoreCatHotButtons');
                    var txtIndent;

                    if (level == undefined) {
                        txtIndent = "30px";
                    }
                    else if (level == 1) {
                        txtIndent = "60px";
                    }
                    else if (level == 2) {
                        txtIndent = "90px";
                    }
                    else if (level == 3) {
                        txtIndent = "120px";
                    }

                    for (var i = 0; i < vData.Rows.length; i++) {
                        if (vData.Rows[i].GroupID == undefined) {
                            txtIndent = "30px";
                            tbody.append($('<tr>')
                                .append($('<td>')
                                    .append($('<div>')
                                        .html(vData.Rows[i].ShortName + ' (' + vData.Rows[i].ProductName + ')')
                                        .css('text-indent', txtIndent)
                                        .attr('data-role', 'none')
                                        .attr('columnName', 'description')
                                        .attr('state', 'add')
                                        .addClass('font-normal')
                                        .attr('celltype', 'sku')
                                        .attr('id', vData.Rows[i].ID)
                                    )
                                    .addClass('tableCell')
                                )
                            )
                        }
                        else {
                            tbody.append($('<tr>')
                                .append($('<td>')
                                    .append($('<div>')
                                        .html(vData.Rows[i].ShortName + ' (' + vData.Rows[i].ProductName + ')')
                                        .css('text-indent', txtIndent)
                                        .attr('data-role', 'none')
                                        .attr('columnName', 'description')
                                        .attr('state', 'add')
                                        .addClass('font-normal')
                                        .attr('celltype', 'sku')
                                        .attr('id', vData.Rows[i].ID)
                                    )
                                    .addClass('tableCell')
                                )
                            )
                        }
                    }
                }

                function DeleteItem(id, item) {
                    var msg;

                    if (item == 'sku') {
                        msg = 'Are you sure you want to delete this ' + item + '?';

                        //DeleteThis(id, item);
                    } else {

                        msg = 'Are you sure you want to delete this ' + item + ' and all the items under it?';
                    }
                    $('#popupStoreCatMenu').modal('hide');
                    $('#popupStoreCatMenuProduct').modal('hide');

                    NewMessageBox(msg, 'Delete Record',
                        [
                            {
                                Text: 'Yes',
                                Click: function () {
                                    DeleteThis(id, item);
                                }
                            },
                            { Text: 'No' }
                        ]
                    )
                } //End of DeleteItem

                function DeleteThis(id, item) {
                    AjaxSend({
                        url: '/OnlineStoreCategories/DeleteItem',
                        data: {
                            ID: id,
                            Item: item
                        },
                        success: function (vData) {
                            $('#panOrgEmpMessage').modal('hide');
                            if (vData.Result == 'ok') {
                                if (item == 'template') {
                                    //                                location = MergeUrlParameters({
                                    //                                    OrgID: sessionStorage.OrgID,
                                    //                                    IsMobile: CBool(IsMobile) ? 1 : 0
                                    //                                }, '/OnlineStoreCategories');
                                    window.location.hash = 'LibraryPage';
                                    LibraryTable.OnlineStoreCategories();
                                } else {
                                    //                                location = MergeUrlParameters({
                                    //                                    OrgID: sessionStorage.OrgID,
                                    //                                    MenuID: sessionStorage.ViewID, //add menu id to query string
                                    //                                    IsMobile: CBool(IsMobile) ? 1 : 0
                                    //                                }, '/OnlineStoreCategories/MenuTemplate');
                                    //                                $('#UniversalDiv').html('');
                                    //                                LibraryTable.ViewHotButtonPage();
                                    //location.reload();
                                    GetData();
                                }

                            } else if (vData.Error != undefined) {
                                NewMessageBox(vData.Error, 'Error');
                            }
                        } // end success
                    }); //end of AjaxSend
                }

                function LoadProducts(vParentDiv, vData) {
                    var pLV;

                    pLV = new CreateListView();
                    pLV.OnItem = function (vDataRow, vItem, e) {
                        //if no short name, force them to put shortname!

                        if (vDataRow.ShortName == undefined) {

                            $('#popupStoreCatNewSKUName').modal('show');
                            $('#lblStoreCatFullProductName').text(vDataRow.ProductName);
                            var string = vDataRow.ProductName;
                            var length = 23;
                            var trimmedString = string.substring(0, length);
                            $('#txtStoreCatName_SKU').val(trimmedString);


                            $('#hdnStoreCatSKUid').val(vDataRow.ID);
                            $('#hdnStoreCatGroupId').val(vDataRow.GroupID);

                        } else {
                            vItem.append($('<a>')
                            .text(vDataRow.ShortName + ' (' + vDataRow.ProductName + ')')
                            .data('data', vDataRow)
                            .attr('href', '#')
                            .click(function (e) {
                                var pData = $(e.target).data().data;
                                $('#TitleOrgEmp').text('Delete SKU');
                                $('#MsgOrgEmp').text('Are you sure you want to delete ' + vDataRow.ShortName + ' (' + vDataRow.ProductName + ')' + '?');
                                $('#panOrgEmpMessage').modal('show');
                                $('#btnStoreCatOKOrgEmp').text('Yes').click(function (e) {
                                    e.preventDefault();
                                    $('#panOrgEmpMessage').modal('hide');
                                    DeleteItem(vDataRow.ID, 'sku');
                                })
                                $('#divBtnCancel').show();
                                $('#btnStoreCatCancelOrgEmp').text('No').click(function () {
                                    $('#panOrgEmpMessage').modal('hide');
                                })
                            })
                        );
                        }
                    }; // OnItem

                    vParentDiv.append(pLV.Element);
                    pLV.SetData(vData.Rows);

                }; // END LoadProducts

                function ClearFields() {
                    $('#lstStoreCatHotButtonList').empty();
                    arrHotButtons = [];
                    ctrFeature = 0;
                    SortList('lstStoreCatHotButtonList');
                }

                function LoadData(vDR) {

                    if (vDR == undefined) {
                        divMain.hide();
                    } else {
                        divGroups.show();
                        divProducts.show();
                        mID = vDR.ID;
                        txtName.val(vDR.Name);
                        chkActive.prop('checked', vDR.IsActive);
                        try {
                            chkActive//.checkboxradio('refresh');
                        } catch (e) {
                        };
                    };

                }; // END LoadData

                function LoadGroup(vSubgroups) {

                    var pLV;
                    var pDiv;

                    pDiv = $('#divSubgroups_ProductGroup');
                    pDiv.html('');
                    divGroups.show();
                    if (vSubgroups == undefined)
                        return;
                    pLV = new CreateListView();
                    pLV.OnItem = function (vDataRow, vItem, e) {

                        vItem.append($('<a>')
                                .text(vDataRow.Name)
                                .data('ID', vDataRow.ID)
                                .data('Name', vDataRow.Name)
                                .attr('href', '#')
                                .click(function (e) {
                                    var pData = $(e.target).data();

                                    mID = pData.ID;
                                    GetData();
                                })
                            );

                    }; // OnItem

                    pDiv.append(pLV.Element);
                    pLV.SetData(vSubgroups.Rows);

                }; // END LoadGroup
                
                function SaveProductName() {
                    AjaxSend({
                        url: '/OnlineStoreCategories/SaveProductName',
                        data: {
                            ID: ($('#hdnStoreCatSKUid').val() == '' ? sessionStorage.PreviousProductID : $('#hdnStoreCatSKUid').val()),
                            GroupID: $('#hdnStoreCatGroupId').val(),
                            MenuID: sessionStorage.ViewID,
                            ShortName: $.trim($("#txtStoreCatName_SKU").val())
                        },
                        success: function (vData) {
                            if (vData.Result == 'ok') {
                                //                            location = MergeUrlParameters({
                                //                                OrgID: sessionStorage.OrgID,
                                //                                MenuID: sessionStorage.ViewID, //add menu id to query string
                                //                                IsMobile: CBool(IsMobile) ? 1 : 0
                                //                            }, '/OnlineStoreCategories/MenuTemplate');
                                $('#popupStoreCatNewSKUName').modal('hide');
                                //                            ReloadPage();
                                //location.reload();
                                //isEnterShortName = 'false';
                                GetData();
                            }
                            else
                                MessageBox(vData.Error);
                        }
                    }); // END AjaxSend
                }

                function SaveProduct(vProductID, vProductName) {


                    AjaxSend({
                        url: '/OnlineStoreCategories/SaveProduct',
                        data: {
                            GroupID: mParentID,
                            ProductID: vProductID,
                            MenuID: sessionStorage.ViewID
                        },
                        success: function (vData) {
                            //                        if (vData.Error == undefined) {

                            $('#lblStoreCatFullProductName').text(vProductName);
                            var string = vProductName;
                            var length = 23;
                            var trimmedString = string.substring(0, length);
                            //$('#txtStoreCatName_SKU').val(trimmedString);

                            $('#hdnStoreCatSKUid').val(vData.SKUData);
                            $('#hdnStoreCatGroupId').val(mParentID);

                            if (vData.Error != undefined) {
                                NewMessageBox(vData.Error, 'New Product')
                                return;
                            } else {
                                if (sessionStorage.PreviousProductID == vProductID) {
                                    //location.reload();
                                    $('#panSearchLibrary').modal('hide');
                                    $('#popupStoreCatNewSKUName').modal('show');
                                } else {
                                    $('#panSearchLibrary').modal('hide');
                                    $('#popupStoreCatNewSKUName').modal('show');
                                    isEnterShortName = 'true';
                                }
                                SaveProductName();
                            }
                            //                        try {

                            //                            if (vData.SKUData != undefined) {
                            //                                $('#popupStoreCatNewSKUName').modal('show');
                            //                                isEnterShortName = 'true';

                            //                                return;
                            //                            }


                            //                        } catch (ex) {
                            //                            alert(ex.Message);
                            //                            if (vData.SKUData != undefined) {
                            //                                $('#popupStoreCatNewSKUName').modal('show');
                            //                                isEnterShortName = 'true';

                            //                                return;
                            //                            }

                            //                        }

                            sessionStorage.PreviousProductID = vProductID;

                            //                        }
                            //                        else
                            //                            MessageBox(vData.Error);
                        }
                    }); // END AjaxSend

                }; // END SaveProduct\

                function GetMenuTemplateName() {
                    if (sessionStorage.ViewID) {
                        AjaxSend({
                            url: '/OnlineStoreCategories/GetMenuTemplateName',
                            data: {
                                ID: sessionStorage.ViewID
                            },
                            success: function (vData) {
                                if (vData.Error == undefined) {
                                    var vRows = vData.Result.Rows[0];
                                    $('#hdrTemplateName').text(vRows.Name);
                                    $('#chbStoreCategoriesIsDefault').attr('checked', CBool(vRows.IsDefault))//.checkboxradio('refresh');
                                }
                                else
                                    NewMessageBox(vData.Error);
                            }
                        }); // END AjaxSend

                    } else {
                        NewMessageBox('Invalid MenuID');
                    }



                };
            }) // end $(document)
            //.keydown(function (e) {
            //    if (sessionStorage.AddViewLibBCName == "View Hot Button") {
            //        var currentCellIndex = $('#tBodyStoreCatHotButtons tr td.activeCell').closest('td').index();

            //        if (e.which == 40) { // down arrow
            //            $('#tBodyStoreCatHotButtons tr td.activeCell').closest('tr').next().find('td:eq(' + currentCellIndex + ')').addClass('activeCell');
            //            $('#tBodyStoreCatHotButtons tr td.activeCell').closest('tr').prev().find('td:eq(' + currentCellIndex + ')').removeClass('activeCell');
            //        }
            //        else if (e.which == 38) { // up arrow
            //            $('#tBodyStoreCatHotButtons tr td.activeCell').closest('tr').prev().find('td:eq(' + currentCellIndex + ')').addClass('activeCell');
            //            $('#tBodyStoreCatHotButtons tr td.activeCell').closest('tr').next().find('td:eq(' + currentCellIndex + ')').removeClass('activeCell');
            //        }
            //        else {
            //            e.stopPropagation();
            //        }
            //    }
            //})
        }); // $(function () {
    });                                                                                                                                                                                                          // require(['common'