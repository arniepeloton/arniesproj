require(['common',
    '/Scripts3/Browser/LibraryPIMProduct.js?crc=',
    '/Scripts/jquery-ui-sortable.js'
    ], function () {

        $(function () {

            $(document).ready(function () {
                var divProductGroup = $('#divProductGroup');
                var txtName = $('#txtName_ProductGroup');
                var divProducts = $('#divProducts_ProductGroup');
                var divGroups = $('#divGroups_ProductGroup');
                var chkActive = $('#chkActive_ProductGroup');
                var mParentID;
                var mID;
                var mNewMode;
                var mProducts = [];
                var isIE = false || !!document.documentMode;
                var currentCell, currentRowIndex = 0; //currentRowIndex is the current active row [blue border]

                var arrHotButtons = [], arrAssignedFowst = [], arrSelectedValidFowst = [], arrSelectedInvalidFowst = [];
                var arrNewlyAddedFowst = [], IsEnableVDMFeatures = false, VDMNoOfMotors = 0, VDMNoOfBins = 0, arrMotorAssignment = [], arrAssignedMotor = [], arrAssignedBin = [];

                GetMenuTemplateName();
                divProductGroup.hide();
                GetData();
                GetOtherPayments();

                window.ProductGroup = {
                    GetProductGroupData: function () {                        
                        currentRowIndex = 0;
                        GetMenuTemplateName();
                        divProductGroup.hide();
                        GetData();
                        GetOtherPayments();
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

                            $('#btnAddGroup_' + vData.Data.ID).click(function () {

                                mParentID = vData.Data.ID;
                            });

                        } else {
                            pDiv.find('[name=btnAddGroupFirst]').hide();
                            pDiv.find('[name=btnAddGroup]').hide();
                            pDiv.find('[name=btnAddProduct]').show();
                            pDiv.find('[name=btnAddProduct]').attr('id', 'btnAddProduct_' + vData.Data.ID);

                            $('#btnAddProduct_' + vData.Data.ID).click(function () {

                                mParentID = vData.Data.ID;

                            });


                            $('#btnDelete_' + vData.Data.ID).click(function () {
                                mParentID = vData.Data.ID;

                                DeleteItem(mParentID, 'group');
                                //          
                            });
                            //FOR PRODUCTS
                            //                        pDiv.find('[name=btnAddGroup]').hide();
                            //                        pDiv.find('[name=btnDelete]').show();
                            //                        pDiv.find('[name=btnDelete]').attr('id', 'btnDelete_' + vData.Data.ID);

                            //                        $('#btnDelete_' + vData.Data.ID).click(function () {
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

                        $('#btnAddGroup_undefined').click(function () {

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

                $('body')
                .on('click', '#btnEditName', function (e) {
                    e.preventDefault();

                    $('#txtEditName').val($('#hdrTemplateName').text());
                    $('#popEditTemplateName').modal('show');
                })
                .on('click', '#btnCancel', function (e) {
                    e.preventDefault();

                    $('#hdrTemplateName').show();
                    $('#divEditName').hide();
                    $('#txtEditName').val('');

                    $('#btnSaveName').hide();
                    $('#btnEditName').show();
                    $('#btnCancel').hide();
                    $('#btnDeleteTemplate, #btnGenerateHB').show();
                })
                .on('click', '#btnSaveName', function (e) {
                    e.preventDefault();

                    if ($.trim($("#txtEditName").val()) == "") {
                        NewMessageBox("Please enter a template name.",
                                "Template Name",
                                [
                                    { Text: 'Ok' }
                                ]
                    );
                        $("#txtEditName").focus();
                        return;
                    }

                    SaveTemplateName();
                })
                .on('click', '#btnAssignToFOWST', function (e) {
                    e.preventDefault();

                    location = MergeUrlParameters({
                        OrgID: sessionStorage.OrgID,
                        MenuID: sessionStorage.ViewID, //add menu id to query string
                        IsMobile: CBool(IsMobile) ? 1 : 0
                    }, '/ProductGroup/AssignFOWST');
                })
                .on('click', '#btnDeleteTemplate', function (e) {
                    e.preventDefault();

                    DeleteItem(sessionStorage.ViewID, "template");
                })
                .on('click', '#btnDuplicateTemplate', function (e) {
                    e.preventDefault();

                    NewMessageBox('Are you sure you want to duplicate this template?', 'Confirmation Dialog', [
                        {
                            Text: 'Yes', Click: function () {
                                AjaxSend({
                                    url: '/ProductGroup/DuplicateHBTemplate',
                                    data: { MenuID: sessionStorage.ViewID },
                                    success: function (e) {
                                        if (e.Result == 'success') {
                                            NewMessageBox('Template has been successfully duplicated');
                                            window.location.hash = 'LibraryPage';
                                            LibraryTable.HotButtons();
                                        } else {
                                            NewMessageBox(e.Error, 'Error');
                                        }
                                    }
                                })
                            }
                        }, { Text: 'No' }
                    ])
                })
                .on('click', '#btnHotButtonsTemplateDone', function (e) {
                    e.preventDefault();
                    window.location.hash = 'LibraryPage';
                    //                location = MergeUrlParameters({
                    //                    OrgID: sessionStorage.OrgID,
                    //                    IsMobile: CBool(IsMobile) ? 1 : 0
                    //                }, '/ProductGroup');
                })
                .on('click', '#butSave_ProductGroup', function (e) {
                    e.preventDefault();
                    if ($.trim(txtName.val()) == '') {
                        NewMessageBox('Please enter a name for this group.', 'New Group',
                            [
                                {
                                    Text: 'Ok',
                                    Click: function () {
                                        //$('#panSearchLibrary').popup('close');
                                        //$('#popupNewGroup').popup('open');
                                        $("#txtName_ProductGroup").focus();
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
                            url: '/ProductGroup/SaveGroup',
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

                                    //$('#popupNewGroup').popup('close');
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
                .on('click', '#butAddProduct_ProductGroupFirst', function (e) {
                    $('#popupMenuFirst').popup('close');
                    $('#lstOfLibrary').empty();
                    $('#TitleLibrary').text('PIM');
                    $('#txtSearchKeyword').val('').focus().attr('placeholder', 'Search name of product');
                    $('#panSearchLibrary').popup('open');
                })
                .on('click', '#butAddProduct_ProductGroup', function (e) {
                    $('#popupMenu').popup('close');
                    $('#lstOfLibrary').empty();
                    $('#TitleLibrary').text('PIM');
                    $('#txtSearchKeyword').val('').focus().attr('placeholder', 'Search name of product');
                    $('#panSearchLibrary').popup('open');
                })
                .on('click', '#butAddProduct_ProductSKU', function (e) {
                    Browser.LibraryPIMProduct.Click = function (e, vOpt) {

                        mProducts.push({
                            ProductID: e.ID,
                            ProductName: e.ProductName
                        });
                        //                    alert('hi');
                        //                    $('#popupNewSKUName').popup('open');

                        SaveProduct(e.ID, e.ProductName);

                    };
                    Browser.LibraryPIMProduct.Show();
                })
                .on('click', '#butDelete_ProductGroup', function (e) {
                    DeleteItem(mParentID, 'group');
                })
                .on('click', '#butDelete_ProductSKU', function (e) {
                    DeleteItem(mParentID, 'group');
                })
                .on('click', '#butSave_SKU', function (e) {
                    e.preventDefault();

                    if ($.trim($("#txtName_SKU").val()) == "") {
                        NewMessageBox('Please enter a short name', 'SKU Short Name',
                            [
                                {
                                    Text: 'Ok',
                                    Click: function () {
                                        //$('#panSearchLibrary').popup('close');
                                        //$('#popupNewSKUName').popup('open');
                                        $("#txtName_SKU").focus();
                                    }
                                }
                            ]
                        )
                        return;
                    }

                    //SaveProductName();
                    SaveProduct($('#hdnSKUid').val(), $('#lblFullProductName').text());
                })
                .on('keyup', '#txtSearchKeyword', function (e) {
                    if (e.keyCode == 13) {
                        if ($('#TitleLibrary').text() == "PIM") {
                            SearchPIM($(this).val());
                        }
                    }
                })
                .on('click', '#tBodyHotButtons .tableCell', function () {
                    $('#tBodyHotButtons tr td').removeClass('activeCell');
                    $('#tBodyHotButtons tr').removeClass('activeRow');
                    $('#tBodyHotButtons tr').removeClass('rowSelected');
                    $(this).addClass('activeCell');
                    $(this).parent().addClass('activeRow');
                    $('.context-menu').remove();
                })
                .on('click', '#btnAddHotButton', function () {
                    //currentCell = $('#tBodyHotButtons tr td.activeCell').find('div');
                    //currentRowIndex = currentCell.closest('tr').index();
                    //var celltype = currentCell.attr('celltype');
                    //if (celltype == "home") {
                        $('#btnAddHotButtonGroup').show();
                        $('#btnAddHotButtonSKU').show();
                        $('#btnDeleteHotButtonSKUGroup, #btnEditHotButtonSKUGroup').hide();
                    //}
                    //else if (celltype == "group") {
                    //    $('#btnAddHotButtonGroup').show();
                    //    $('#btnAddHotButtonSKU').show();
                    //    $('#btnDeleteHotButtonSKUGroup, #btnEditHotButtonSKUGroup').show();

                    //    if (currentCell.attr('level') == "3") {
                    //        $('#btnAddHotButtonGroup').hide();
                    //    }
                    //}
                    //else if (celltype == "sku") {
                    //    $('#btnAddHotButtonGroup').hide();
                    //    $('#btnAddHotButtonSKU').hide();
                    //    $('#btnDeleteHotButtonSKUGroup, #btnEditHotButtonSKUGroup').show();
                    //}
                    //$('#lstMenuHotButton');//.listview('refresh');
                    //$('#popupMenuHotButton').popup('open');
                    PopupButtonList('', 'Options', [
                        {
                            Text: 'Add Group', Click: function () {
                                $('#btnAddGroup').click();
                            }
                        }, {
                            Text: 'Add SKU', Visible: !IsEnableVDMFeatures, Click: function () {
                                $('#btnAddSku').click();
                            }
                        }, {
                            Text: 'Edit Selected Lists', Click: function () {
                                var chbSelectedHB = $('.chbHBList:checked');
                                var tbodyRenameSelectedHB = $('#tbodyRenameSelectedHB').empty();
                                var ctrRow = 1, selectmenu = $('#selChangeDefaultOPT').clone().removeAttr('id');
                                var optgroup = $('<optgroup label="Select Default Other Payment Type">');
                                var btnOkRenameSelectedHB = $('#btnOkRenameSelectedHB').unbind('click');

                                optgroup.append(selectmenu.find('option').clone());
                                selectmenu.empty().append(optgroup);

                                if (chbSelectedHB.length > 0) {
                                    chbSelectedHB.each(function () {
                                        var data = $(this).data('data');
                                        tbodyRenameSelectedHB.append($('<tr>')
                                                .data('data', data)
                                                .append($('<td>').text(ctrRow + '.'))
                                                .append($('<td>').html((CBool(data.IsGroup) ? '(A Group Button)' : data.ProductNo + '<br>' + data.Name)))
                                                .append($('<td>').append($('<div class="input-group">')
                                                        .append($('<input data-role="none" type="text" class="form-control">').attr('value', (CBool(data.IsGroup) ? data.Name : data.ShortName)))
                                                        .append((CBool(data.IsGroup) ? selectmenu.clone().val(data.DefaultOtherPaymentID) : ''))
                                                    )
                                                )
                                            )

                                        ctrRow += 1;
                                    })

                                    btnOkRenameSelectedHB.click(function () {
                                        tbodyRenameSelectedHB.find('tr').each(function () {
                                            var dis = $(this), data = dis.data('data');
                                            var inp = dis.find('input').removeClass('is-invalid'), newname = inp.val();

                                            if ($.trim(newname) == '') {
                                                inp.addClass('is-invalid');
                                                WebToast('Please enter new name');
                                                return false;
                                            } else {
                                                if (CBool(data.IsGroup)) {
                                                    data.Name = newname;
                                                    data.DefaultOtherPaymentID = dis.find('select option:selected').val();
                                                } else {
                                                    data.ShortName = newname;
                                                }
                                                $('#name-' + data.ID).html((CBool(data.IsGroup) ? '<b><span class="required">*</span> ' + data.Name + '</b>' : '(' + data.ShortName + ') ' + data.Name));
                                            }
                                        })
                                        WebToast('Successfully updated')
                                        $('#popRenameSelectedHB').modal('hide');
                                    })

                                    $('#popRenameSelectedHB').modal('show');
                                } else {
                                    NewMessageBox('No checkbox has been selected.');
                                }                                
                            }
                        }, {
                            Text: 'Remove Selected Lists', Click: function () {
                                var chbSelectedHB = $('.chbHBList:checked');

                                if (chbSelectedHB.length > 0) {
                                    NewMessageBox('Are you sure you want to remove selected lists?', 'Confirmation Message', [
                                        {
                                            Text: 'Yes', Click: function () {
                                                chbSelectedHB.each(function () {
                                                    var data = $(this).data('data');
                                                    data.State = 'remove';
                                                    $('#list-' + data.ID).remove();
                                                })
                                                WebToast('Selected lists has been removed');
                                            }
                                        }, { Text: 'No' }
                                    ])                                    
                                } else {
                                    NewMessageBox('No checkbox has been selected.');
                                }
                            }
                        }
                    ])
                })
                .on('click', '#btnAddHotButtonGroup', function () {
                    mParentID = currentCell.attr('id');
                })
                .on('click', '#btnAddHotButtonSKU', function () {
                    mParentID = currentCell.attr('id');
                    BrowsePopup('LibraryPIMProduct', 'PIM',
                        [
                            {
                                StyleFormat: 'table',
                                DisplayText: [{ Label: 'Category:', Column: 'Category' }, { Label: 'Product Name', Column: 'ProductName' }, {
                                    Label: 'Retail Price', Column: function (e) {
                                        return AccountingFormat(e.RetailPrice);
                                    }
                                }],
                                Click: function (e) {
                                    mProducts.push({
                                        ProductID: e.ID,
                                        ProductName: e.ProductName
                                    });
                                    //SaveProduct(e.ID, e.ProductName); do not save to db immediately. need to set the short name first. commented by arnie 7-6-17
                                    $('#lblFullProductName').text(e.ProductName);
                                    //$('#popupNewSKUName').popup('open');                                    
                                    $('#hdnSKUid').val(e.ID);
                                    $('#txtName_SKU').val((e.ProductName).substring(0, 50)).focus();

                                }
                            }
                        ]
                    )
                })
                .on('click', '#btnDeleteHotButtonSKUGroup', function () {
                    var celltype = currentCell.attr('celltype');
                    $('#popupMenuHotButton').popup('close');
                    if (celltype == "home" || celltype == "group") {
                        DeleteItem(currentCell.attr('id'), 'group')
                    }
                    else if (celltype == "sku") {
                        DeleteItem(currentCell.attr('id'), 'sku')
                    }
                })
                .on('click', '#btnEditHotButtonSKUGroup', function () {
                    $('#popupMenuHotButton').popup('close');
                    $('#popupEditSKUName').popup('open');
                    var txt = $.trim(currentCell.contents().not(currentCell.children()).text().split('(')[0]);
                    $('#lblEditFullProductName').text(txt);
                    $('#txtEditName_SKU').val(txt).focus();
                    PopupReposition();
                })
                .on('click', '.btnClosePopup', function () {
                    NewMessageBoxClose();
                })
                .on('click', '#butEditSave_SKU', function () {
                    var txtEditName_SKU = $.trim($('#txtEditName_SKU').val())

                    if(txtEditName_SKU == ''){
                        WebToast('Please enter new short name', 'top');
                        return;
                    }

                    AjaxSend({
                        url: 'ProductGroup/RenameShortName',
                        data: { Name: txtEditName_SKU, ID: currentCell.attr('id'), Type: currentCell.attr('celltype') },
                        success: function (e) {
                            if(e.Results == 'saved'){
                                GetData(function () {
                                    $('#tBodyHotButtons tr td').removeClass('activeCell');
                                    $('#tBodyHotButtons tr').removeClass('activeRow rowSelected');
                                    $('#tBodyHotButtons tr:eq(' + currentRowIndex + ')').addClass('activeRow');
                                    $('#tBodyHotButtons tr:eq(' + currentRowIndex + ') > td').addClass('activeCell');
                                });
                                $('#popupEditSKUName').popup('close');                                
                            } else {
                                NewMessageBox(e.Results, 'Message');
                            }
                        }
                    })
                })
                .on('click', '#btnGenerateHB', function () {
                    function OtherOption(opt) {
                        PopupButtonList('', 'How would you like it to be generated?', [
                            {
                                Text: 'Generate All', Click: function () {
                                    GenerateNow({
                                        Option: opt,
                                        Type: 'all'
                                    })
                                }
                            }, {
                                Text: 'Custom Selection of SKU ' + (opt == 'brand' || opt == 'brandcategory' ? 'Brands' : 'Categories'), Click: function () {
                                    var lib;

                                    if (opt == 'brand' || opt == 'brandcategory') {
                                        lib = 'LibraryPimBrands';
                                    } else if (opt == 'category') {
                                        lib = 'LibraryPimCategories';
                                    }

                                    BrowsePopup(lib, 'Select SKU ' + (opt == 'brand' || opt == 'brandcategory' ? 'Brands' : 'Categories'), [
                                        {
                                            DisplayText: [{ Column: (opt == 'brand' || opt == 'brandcategory' ? 'Brand' : 'Category') }],
                                            AutoSearch: true,
                                            SelectMultiple: true,
                                            Message: 'You can select multiple records. Click "Ok" when you\'re done.',
                                            Click: function (e) {
                                                if (opt == 'brandcategory') {
                                                    Bs.Loading('show');
                                                    AjaxSend({
                                                        url: 'ProductGroup/GetCategoriesOfSelectedBrands',
                                                        data: { Brands: e },
                                                        success: function (e) {
                                                            Bs.Loading('hide');
                                                            var vRows = e.data, divBrandLists = $('#divBrandLists');

                                                            divBrandLists.empty();

                                                            for (var i in vRows) {
                                                                var dataBrand = vRows[i], accordionid = dataBrand.ID.replaceAll(' ', '').toLowerCase();
                                                                var div = $('<div class="accordion mb-3">')
                                                                            .append($('<div class="card">')
                                                                                .append($('<div class="card-header p-0">')
                                                                                    .append($('<div data-toggle="collapse" data-target="#div' + accordionid + '" class="p-3">')
                                                                                        .append($('<div class="form-check form-check-inline">')
                                                                                            .append($('<input data-role="none" type="checkbox" class="form-check-input chbBrand" id="chb' + accordionid + '" checked />')
                                                                                                .data('data', dataBrand)
                                                                                                .change(function (e) {
                                                                                                    e.preventDefault();
                                                                                                    e.stopPropagation();
                                                                                                })
                                                                                            )
                                                                                            .append('<label class="form-check-label font-weight-bold" for="chb' + accordionid + '">'+ dataBrand.Brand +'</label>')
                                                                                        )
                                                                                    )
                                                                                )
                                                                                .append($('<div id="div' + accordionid + '" class="collapse show">').append('<div class="card-body"></div>'))
                                                                            )

                                                                var ol = $('<ol class="mb-0" id="ol' + accordionid + '">');

                                                                for (var c in dataBrand.Categories) {
                                                                    var dataCategory = dataBrand.Categories[c], name = dataCategory.Name, vID = name.replaceAll(' ', '').toLowerCase();

                                                                    ol.append($('<li>')
                                                                            .append($('<div class="form-check form-check-inline">')
                                                                                .append($('<input data-role="none" type="checkbox" class="form-check-input" id="chb' + vID + '" checked />')
                                                                                    .data('data', dataCategory)
                                                                                    .change(function (e) {
                                                                                        e.preventDefault();
                                                                                        e.stopPropagation();
                                                                                    })
                                                                                )
                                                                                .append('<label class="form-check-label" for="chb' + vID + '">' + name + '</label>')
                                                                            )
                                                                        );
                                                                }

                                                                div.find('.card-body').append(ol);
                                                                divBrandLists.append(div);
                                                            }

                                                            $('#btnOkBrandCategorySelection').unbind('click').click(function () {
                                                                var arrSelectedBrands = [];

                                                                $('.chbBrand:checked').each(function () {
                                                                    var dis = $(this), data = dis.data('data');

                                                                    data.SelectedCategories = [];
                                                                    $('#ol' + data.ID).find('input:checked').each(function () {
                                                                        data.SelectedCategories.push($(this).data('data'))
                                                                    })

                                                                    arrSelectedBrands.push(data);
                                                                })

                                                                GenerateNow({
                                                                    Option: opt,
                                                                    Type: 'custom',
                                                                    Selection: arrSelectedBrands
                                                                })
                                                            })

                                                            $('#popBrandCategory').modal('show');
                                                        }
                                                    })
                                                } else {
                                                    GenerateNow({
                                                        Option: opt,
                                                        Type: 'custom',
                                                        Selection: e
                                                    })
                                                }                                                
                                            }
                                        }
                                    ], { IsAll: true })
                                }
                            }
                        ])
                    }

                    function GenerateNow(data) {
                        var url;
                        data.MenuID = sessionStorage.ViewID;
                        data.UserID = sessionStorage.PeopleID;

                        Bs.Loading('show', 'Please wait while generating hot buttons ... please do not refresh the page');

                        if (data.Option == 'brandcategory') {
                            url = 'ProductGroup/GenerateBrandsCategories';
                        } else {
                            url = 'ProductGroup/GenerateHB';
                        }

                        AjaxSend({
                            url: url,
                            data: data,
                            success: function (e) {
                                Bs.Loading('hide');
                                $('#popBrandCategory').modal('hide');
                                if (e.Results == 'saved') {
                                    GetData();
                                    WebToast('Successfully generated hot buttons. <br><br> Refreshing list, please wait ...', 'top');
                                } else {
                                    NewMessageBox(e.Results, 'Message');
                                }
                            }
                        })
                    }

                    PopupButtonList('', 'Select Options', [
                        {
                            Text: 'Generate SKU Brands and Categories', Click: function () {
                                OtherOption('brandcategory');
                            }
                        }, {
                            Text: 'Generate SKU Brands', Click: function () {
                                OtherOption('brand');
                            }
                        }, {
                            Text: 'Generate SKU Categories', Click: function () {
                                OtherOption('category');
                            }
                        }
                    ])
                    return
                    NewMessageBox('This will automatically generate hot buttons. Click "Generate Now" to continue.', 'Message', [
                        {
                            Text: 'Generate Now', Click: function () {
                                Bs.Loading('show', 'Please wait while generating hot buttons ... please do not refresh the page');

                                AjaxSend({
                                    url: 'ProductGroup/GenerateHB',
                                    data: {
                                        MenuID: sessionStorage.ViewID
                                    },
                                    success: function (e) {
                                        Bs.Loading('hide');
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

                .on('click', '#btnAddGroup', function () {
                    $('#popupMenuHotButton').modal('hide');
                    $('#popNewAddGroup').modal('show').unbind('shown.bs.modal').on('shown.bs.modal', function () {
                        $('#txtGroupName').val('').focus();
                        $('#selDefaultOPT').val('');
                    });

                    $('#btnOkGroupName').unbind('click').click(function () {
                        var txtGroupName = $.trim($('#txtGroupName').val());

                        if (txtGroupName == '') {
                            WebToast('Please enter group name', 'top');
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
                            DefaultOtherPaymentID: $('#selDefaultOPT').val()
                        }
                        arrHotButtons.push(data);
                        GenerateList(data, $('#lstHotButtonList'));
                        $('#popNewAddGroup').modal('hide');
                    })

                    $('#txtGroupName').unbind('keydown').keydown(function (e) {
                        if(e.which == 13){
                            $('#btnOkGroupName').click();
                        }
                    })
                })
                .on('click', '#btnAddSku', function () {
                    BrowseSkus(function (e) {
                        RenameSelectedSkus(e, arrHotButtons, $('#lstHotButtonList'));
                    })
                })
                .on('click', '#btnSaveHB', function () {
                    if (IsEnableVDMFeatures) {
                        var tbodyVDMMotorAssignments = $('#tbodyVDMMotorAssignments').empty();

                        for (var i in arrMotorAssignment) {
                            tbodyVDMMotorAssignments.append($('<tr>')
                                .append($('<td>').text((CInt2(i) + 1) + '.'))
                                .append($('<td>').text(arrMotorAssignment[i].GroupName))
                                .append($('<td>').text(arrMotorAssignment[i].ProductName))
                                .append($('<td class="text-center">').text((CStr2(arrMotorAssignment[i].VDMMotorNo) == '' ? '' : 'M - ' + arrMotorAssignment[i].VDMMotorNo) + (CStr2(arrMotorAssignment[i].VDMBinNo) == '' ? '' : 'B - ' + arrMotorAssignment[i].VDMBinNo)))                                
                            )
                        }

                        $('#popVDMMotorAssignments').modal('show');
                    } else {
                        SaveHotButtons();
                    }
                })
                .on('click', '#btnSaveHBVDMNow', function () {
                    SaveHotButtons();
                })
                .on('click', '#btnHBAddFOWST', function () {
                    arrSelectedValidFowst = [];
                    arrSelectedInvalidFowst = [];
                    function validate(e) {
                        for (var i = 0; i < e.length; i++) {
                            e[i].State = 'new';

                            if (CStr2(e[i].HBTemplateID) == '') {
                                arrSelectedValidFowst.push(e[i]);
                            } else {
                                arrSelectedInvalidFowst.push(e[i]);
                            }
                        }

                        if (arrSelectedInvalidFowst.length > 0) {
                            NewMessageBox('Some of selected FOWST locations are already assigned to another Hot Button Template.<br><br> What would you like to do?', 'Message', [
                                {
                                    Text: 'Add all selected FOWST locations to this template',
                                    Click: function () {
                                        ValidateSelectedFowst($.merge(arrSelectedValidFowst, arrSelectedInvalidFowst));
                                    }
                                },
                                {
                                    Text: 'Do not add selected FOWST locations that already have another assigned Hot Button template',
                                    Click: function () {
                                        ValidateSelectedFowst(arrSelectedValidFowst);
                                    }
                                },
                                {
                                    Text: 'Cancel'
                                }
                            ])
                        } else {
                            ValidateSelectedFowst(arrSelectedValidFowst);
                        }
                    }
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
                                            validate(vData);
                                        }
                                    }
                                ], { ShowHBTemplate: true, IsAll: true })
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
                                                    GroupID: e.ID,
                                                    ShowHBTemplate: true,
                                                }, success: function (e) {
                                                    Bs.Loading('hide');
                                                    validate(e.Results.Rows);
                                                }
                                            })
                                        }
                                    }
                                ])
                            }
                        }
                    ])
                })
                .on('click', '#btnHBAllFOWST', function () {
                    arrSelectedValidFowst = [];
                    arrSelectedInvalidFowst = [];

                    AjaxSend({
                        url: 'Browser/LibraryFowst',
                        data: { ShowHBTemplate: true, IsAll: true },
                        success: function (e) {
                            var vRows = e.Results.Rows;
                            for (var i = 0; i < vRows.length; i++) {
                                vRows[i].State = 'new';

                                if (vRows[i].HBTemplateID == '') {
                                    arrSelectedValidFowst.push(vRows[i]);
                                } else {
                                    arrSelectedInvalidFowst.push(vRows[i]);
                                }
                            }

                            if (arrSelectedInvalidFowst.length > 0) {
                                NewMessageBox('We found that some FOWST locations are already assigned to use another Hot Button template. <br><br> ' +
                                        'Mobiz allows only one hot button template to be assigned for each FOWST location. <br><br>' +
                                        'What would you like to do?', 'Message', [
                                    {
                                        Text: 'Assign all FOWST locations to this template',
                                        Click: function () {
                                            ValidateSelectedFowst($.merge(arrSelectedValidFowst, arrSelectedInvalidFowst));
                                        }
                                    },
                                    {
                                        Text: 'Do not apply this setting to FOWST locations that already have another assigned Hot Button template',
                                        Click: function () {
                                            ValidateSelectedFowst(arrSelectedValidFowst);
                                        }
                                    },
                                    {
                                        Text: 'Cancel'
                                    }
                                ])
                            } else {
                                ValidateSelectedFowst(arrSelectedValidFowst);
                            }
                        }
                    })
                })
                .on('click', '#btnHBFowstNoTemplate', function () {
                    $('#popHBFowstNoTemplate').modal('show');
                })
                .on('click', '#tbHBTempate', function () {
                    $('#btnAddHotButton').show();
                })
                .on('click', '#tabHBFowst', function () {
                    $('#btnAddHotButton').hide();
                })
                    .on('change', '[name="radRecomItem"]', function () {
                        if ($(this).val() == 'no recommendation') {
                            $('#txtRecomSKU').parent().hide();
                        } else $('#txtRecomSKU').parent().show();
                    })
                    .on('click', '#txtRecomSKU', function () {
                        var dis = $(this);
                        BrowseSkus(function (e) {
                            dis.attr('recomskuid', e.ID).val(e.ProductNo + ' - ' + e.ProductNameOnly)
                        }, false)
                    })
                function SaveHotButtons() {
                    $('#btnSaveHB').attr('disabled', true);
                    Bs.Loading('show');
                    console.log(arrHotButtons)
                    AjaxSend({
                        url: 'ProductGroup/SaveHotButtons',
                        data: {
                            MenuID: sessionStorage.ViewID,
                            HotButtons: arrHotButtons,
                            AssignedFowst: arrAssignedFowst,
                            IsEnableVDMFeatures: IsEnableVDMFeatures,
                            IsDisplayAllProducts: $('#chbIsDisplayAll').is(':checked')
                        },
                        success: function (e) {
                            Bs.Loading('hide');
                            $('#popVDMMotorAssignments').modal('hide');
                            $('#btnSaveHB').removeAttr('disabled');
                            if (e.Results == 'saved') {
                                WebToast('Successfully saved', 'top');
                                LibraryTable.HotButtons();
                                window.location.hash = 'LibraryPage';
                            } else {
                                NewMessageBox(e.Results, 'Message');
                            }
                        }
                    })
                }

                function AssignedFOWST(data) {
                    var listHBFowst = $('#listHBFowst');

                    if(data.length > 0){
                        for (var i = 0; i < data.length; i++) {
                            data[i].Index = i;
                            if(listHBFowst.find('a[fid="'+ data[i].ID +'"]').length == 0){
                                listHBFowst.append($('<a href="#" class="list-group-item list-group-item-action">')
                                    .attr('fid', data[i].ID)
                                    .append($('<table style="width:100%;">')
                                        .append($('<tr>')
                                            .append($('<td style="word-break:break-all;">')
                                                .data('data', data[i])
                                                .append($('<h5>')
                                                    .attr("style", "white-space:normal")
                                                    .text(data[i].FowstName)
                                                )
                                            )
                                            .append($('<td style="width:50px; text-align:center;">').append($('<span class="btn btn-xs btn-danger">')
                                                    .append($('<span class="bi-trash-fill" aria-hidden="true">'))
                                                    .data('data', data[i])
                                                    .click(function (e) {
                                                        e.preventDefault();
                                                        var dis = $(this);
                                                        var data = dis.data('data');

                                                        NewMessageBox('Are you sure you want to remove this FOWST location to this template?', 'Message', [
                                                            {
                                                                Text: 'Yes', Click: function () {
                                                                    dis.parents().closest('a').remove();
                                                                    if (data.State == 'new') {                                                            
                                                                        arrAssignedFowst.splice(data.Index, 1);
                                                                    } else {
                                                                        Bs.Loading('show');
                                                                        AjaxSend({
                                                                            url: '/ProductGroup/RemoveFOWSTHB',
                                                                            data: { FOWSTID: data.ID },
                                                                            success: function (e) {
                                                                                Bs.Loading('hide');
                                                                                if (e.Result == 'removed') {
                                                                                    dis.remove();
                                                                                    WebToast('Successfully removed')
                                                                                } else {
                                                                                    NewMessageBox(e.Error, 'Error')
                                                                                }
                                                                            }
                                                                        })
                                                                    }
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
                        listHBFowst.append($('<a href="#" class="list-group-item list-group-item-action">').text('No result(s) found'));
                    }

                    listHBFowst;//.listview('refresh');
                }

                function ValidateSelectedFowst(data) {
                    if (arrAssignedFowst.length > 0) {
                        for (var i = 0; i < data.length; i++) {
                            var isExist = false;

                            $.grep(arrAssignedFowst, function (a) {
                                if (a.ID == data[i].ID) {
                                    isExist = true;
                                    return false;
                                }
                            })

                            if (!isExist) {
                                arrNewlyAddedFowst.push(data[i]);
                            }
                        }
                    } else {
                        $('#listHBFowst').empty();
                        arrNewlyAddedFowst = data;
                    }

                    arrAssignedFowst = $.merge(arrAssignedFowst, arrNewlyAddedFowst);

                    AssignedFOWST(arrAssignedFowst);
                }

                function ListOfFowstNoTemplate(data) {
                    var listHBUnassignedFowst = $('#listHBUnassignedFowst');
                    listHBUnassignedFowst.empty();
                    if (data.length > 0) {
                        for (var i = 0; i < data.length; i++) {
                            listHBUnassignedFowst.append($('<a href="#" class="list-group-item list-group-item-action">')
                                    .text(data[i].FowstName)
                                )
                        }
                    } else {
                        listHBUnassignedFowst.append($('<a href="#" class="list-group-item list-group-item-action">').text('No result(s) found'));
                    }

                    listHBUnassignedFowst;//.listview('refresh');
                }

                function BrowseSkus(f, ismultiple) {
                    BrowsePopup('LibraryPIMProduct', 'SKU',
                        [
                            {
                                Message: ismultiple == undefined ? 'Note: You can select multiple products. Click OK when you\'re done.' : '',
                                SelectMultiple: ismultiple == undefined ? true : ismultiple,
                                StyleFormat: 'table',
                                DisplayText: [{ Label: 'Category:', Column: 'Category' }, { Label: 'Product No', Column: 'ProductNo' }, { Label: 'Product Name', Column: 'ProductName' }, {
                                    Label: 'Retail Price', Column: function (e) {
                                        return AccountingFormat(e.RetailPrice);
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
                    var tBodySelectedSkus = $('#tBodySelectedSkus'), isSelectedSKUExist = true;
                    var groupdata = $(container).parent().data('data');
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

                        var shortname = (arrSkus[i].ProductNameOnly).substring(0, 50);
                        arrSkus[i].Index = i;
                        arrSkus[i].ShortName = shortname;
                        arrSkus[i].ProductName = arrSkus[i].ProductNameOnly;
                        arrSkus[i].ProductID = arrSkus[i].ID;
                        arrSkus[i].GroupName = groupdata.Name;
                        arrSkus[i].Path = groupdata.Name + ' > ' + arrSkus[i].ProductNameOnly;
                        arrMotorAssignment.push(arrSkus[i]);
                        tBodySelectedSkus.append($('<tr>')
                                .append($('<td style="width:150px">')
                                    .append(arrSkus[i].ProductNo)
                                )
                                .append($('<td style="width:200px">')
                                    .append(arrSkus[i].ProductNameOnly)
                                )
                                .append($('<td>')
                                    .append($('<input data-role="none" class="form-control txtShortName" maxlength="50">')
                                        .data('data', arrSkus[i])
                                        .val(shortname)
                                        .blur(function () {
                                            var data = $(this).data('data');
                                            data.ShortName = $.trim($(this).val());
                                        })
                                    )
                                )
                                .append($('<td class="text-center">')
                                    .append($('<button data-role="none" class="btn btn-danger">')
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
                                            ], {
                                                Close: function () {
                                                    $('#popHBSelectedSkus').modal('show');
                                                }
                                            })
                                        })
                                    )
                                )
                            )
                    }

                    $('#btnOkSelectedSkus').unbind('click').click(function () {
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
                                    ProductNo: data.ProductNo,
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

                            $('#popHBSelectedSkus').modal('hide');
                        }
                    })

                    if (!isSelectedSKUExist) {
                        $('#popHBSelectedSkus').modal('show');
                    } else {
                        WebToast('All selected SKUs was already exist in the list', 'top');
                    }
                }

                function DisplayNewHB(data, container) {
                    for (var i = 0; i < data.length; i++){
                        GenerateList(data[i], container)
                    }
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
                    }, 100, id)
                }

                function GenerateList(data, container) {

                    if(data.State == 'remove'){
                        return;
                    }

                    if (data.StoreAssignment == undefined) {
                        data.StoreAssignment = [];
                    }

                    var div = $('<div class="align-items-center" style="display:flex; padding:10px !important;">')
                            .append($('<input data-role="none" type="checkbox" class="form-check-input chbHBList" data-toggle="tooltip" title="Check to remove or edit">')
                                .data('data', data)
                            )
                            .append('&nbsp;')
                            //.append($('<button data-role="none" class="btn btn-light btn-sm">').append($('<span class="bi-grip-vertical">')))
                            //.append('&nbsp;')
                        .append($('<button data-role="none" class="btn btn-light btn-sm text-left">').append($('<span id="name-' + data.ID + '">')
                            .append((CBool(data.IsGroup) ? '<b><span class="required">*</span> ' + data.Name + '</b>' : data.ShortName + '<br>(' + data.ProductNo + ' - ' + data.Name + ')') + (CStr2(data.VDMMotorNo) == '' ? '' : '<br> Motor No.: ' + data.VDMMotorNo) + (CStr2(data.VDMBinNo) == '' ? '' : '<br> Bin No.: ' + data.VDMBinNo))
                            ))
                            .append('&nbsp &nbsp; &nbsp;')
                    var li = $('<li id="list-' + data.ID + '" class="list-group-item m-0 p-0">')
                                .data('data', data)
                                .append(div)

                    var dropdown = $('<div class="dropdown">')
                                    .append($('<button data-role="none" class="btn btn-secondary btn-sm" data-toggle="dropdown" id="dd-' + data.ID + '">')
                                        .append('<span class="bi-gear-fill"></span>')
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

                    var options = $('<ul class="dropdown-menu shadow" aria-labelledby="dd-' + data.ID + '">')
                                    

                    if (CBool(data.IsGroup)) { // for groups only

                        if (CInt2(data.Level) < 3 && !IsEnableVDMFeatures){ // when in level 3, user must not allow to add another group 
                            options.append($('<li>')
                                        .append($('<a class="dropdown-item">').data('data', data).append('<span class="bi-plus" style="color: #16a085;"></span> &nbsp; Add Sub Group')
                                            .click(function (e) {
                                                var data = $(this).data('data');

                                                $('#popNewAddGroup').modal('show').unbind('shown.bs.modal').on('shown.bs.modal', function () {
                                                    $('#txtGroupName').val('').focus();
                                                    $('#selDefaultOPT').val('');
                                                });

                                                $('#btnOkGroupName').unbind('click').click(function () {
                                                    var txtGroupName = $.trim($('#txtGroupName').val());

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
                                                        DefaultOtherPaymentID: $('#selDefaultOPT').val()
                                                    }
                                                    data.SubGroup.push(newdata);
                                                    GenerateList(newdata, $('#ul-' + data.ID));
                                                    $('#popNewAddGroup').modal('hide');
                                                })

                                                $('#txtGroupName').unbind('keydown').keydown(function (e) {
                                                    if (e.which == 13) {
                                                        $('#btnOkGroupName').click();
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

                        options.append($('<li>')
                                        .append($('<a class="dropdown-item">').data('data', data).append('<span class="bi-plus" style="color: #16a085;"></span> &nbsp; Add Sub SKU by Brand')
                                            .click(function (e) {
                                                var data = $(this).data('data');                                                
                                                BrowseBrandsOrCategory('Brand', function (e) {
                                                    RenameSelectedSkus(e.Results.Rows, data.SubGroup, $('#ul-' + data.ID));
                                                })
                                            })
                                        )
                                    )

                        options.append($('<li>')
                                        .append($('<a class="dropdown-item">').data('data', data).append('<span class="bi-plus" style="color: #16a085;"></span> &nbsp; Add Sub SKU by Category')
                                            .click(function (e) {
                                                var data = $(this).data('data');
                                                BrowseBrandsOrCategory('Category', function (e) {
                                                    RenameSelectedSkus(e.Results.Rows, data.SubGroup, $('#ul-' + data.ID));
                                                })
                                            })
                                        )
                                    )
                    }

                    options.append($('<li>')
                                        .append($('<a class="dropdown-item">').data('data', data).append('<span class="bi-pencil-square" style="color: #f1c40f;"></span> &nbsp; Edit')
                                            .click(function (e) {
                                                var data = $(this).data('data');
                                                $('#popRenameHB').modal('show').unbind('shown.bs.modal').on('shown.bs.modal', function () {
                                                    $('#txtRenameHB').val((CBool(data.IsGroup) ? data.Name : data.ShortName)).focus()
                                                    $('#selChangeDefaultOPT').val(CStr2(data.DefaultOtherPaymentID));
                                                });
                                                $('#txtCurrentHBName').text(data.Name);
                                                $('#chbUseUOM').prop('checked', CInt2(data.IsUseUOM)).unbind().change(function () {
                                                    if ($(this).is(':checked')) {
                                                        $('#chbDoNotIncludeBaseUOM').prop('checked', 1);
                                                        $('#divSellingPreference').show();
                                                    } else $('#divSellingPreference').hide();
                                                });

                                                if (CBool(data.IsUseUOM)) $('#divSellingPreference').show();
                                                $('[name="radSellingPreference"][value="' + data.SellingPreference + '"]').prop('checked', 1);

                                                if (IsGuid(data.RecommendedSKU)) {
                                                    $('[name="radRecomItem"][value="with sku"]').prop('checked', 1);
                                                    $('#txtRecomSKU').parent().show();
                                                    $('#txtRecomSKU').val(data.RecommendedSKUName);
                                                } else {
                                                    $('[name="radRecomItem"][value="No Recommendation"]').prop('checked', 1);
                                                    $('#txtRecomSKU').parent().hide();
                                                    $('#txtRecomSKU').val('');
                                                }

                                                $('#chbDoNotIncludeBaseUOM').prop('checked', CInt2(data.IsDoNotIncludeBaseUOM));
                                                $('#txtMotorNo').val('');

                                                if (CBool(data.IsGroup)) {
                                                    $('#selChangeDefaultOPT').parent().show();
                                                    $('#lblRenameNote').text('Group Name: (maximum of 50 characters only)');
                                                    $('#txtRenameHB').attr('maxlength', 50);
                                                } else {
                                                    $('#selChangeDefaultOPT').parent().hide();
                                                    $('#lblRenameNote').text('Short Name: (maximum of 50 characters only)');
                                                    $('#txtRenameHB').attr('maxlength', 50);
                                                }

                                                if (IsEnableVDMFeatures) {
                                                    $('#selChangeDefaultOPT').parent().hide(); // always hide for VDM
                                                    //$('#txtMotorNo').parent().parent().show(); //no need ipakita, sa Planogram settings ang assigning sa motor per item
                                                    $('#txtMotorNo').val(data.VDMMotorNo);
                                                    //$('#txtBinNo').parent().parent().show(); //no need ipakita, sa Planogram settings ang assigning sa motor per item
                                                    $('#txtBinNo').val(data.VDMBinNo);
                                                    //PopulateVDMMotorsNo(data);
                                                }

                                                $('#btnHBMotorNoAssignment').unbind('click').click(function () {
                                                    PopulateVDMMotorsNo('motor', arrAssignedMotor, VDMNoOfMotors);
                                                })

                                                $('#btnHBBinNoAssignment').unbind('click').click(function () {
                                                    PopulateVDMMotorsNo('bin', arrAssignedBin, VDMNoOfBins);
                                                })

                                                $('#btnOkRename').unbind('click').click(function () {
                                                    var val = $.trim($('#txtRenameHB').val());

                                                    if(val == ''){
                                                        WebToast('Please enter new name', 'top');                                                        
                                                    } else {
                                                        if (IsEnableVDMFeatures) {
                                                            var isEnable = $('#chbUseUOM').is(':checked');
                                                            var IsDoNotIncludeBaseUOM = $('#chbDoNotIncludeBaseUOM').is(':checked');
                                                            var isExist = false, index = null, motorno = $('#txtMotorNo').val(), binno = $('#txtBinNo').val();

                                                            $.grep(arrMotorAssignment, function (d, ind) {
                                                                if (d.ProductID.toLowerCase() == data.ProductID.toLowerCase()) {
                                                                    isExist = true;
                                                                    index = ind;
                                                                    return false;
                                                                }
                                                            })

                                                            data.IsUseUOM = isEnable;
                                                            data.SellingPreference = isEnable ? $('[name="radSellingPreference"]:checked').val() : '';
                                                            data.RecommendedSKU = isEnable ? ($('[name="radRecomItem"]:checked').val() == 'No Recommendation' ? '' : $('#txtRecomSKU').attr('recomskuid')) : '';
                                                            data.IsDoNotIncludeBaseUOM = IsDoNotIncludeBaseUOM;
                                                            data.VDMMotorNo = motorno;
                                                            data.VDMBinNo = binno;

                                                            if (isExist) {
                                                                arrMotorAssignment[index].VDMMotorNo = motorno;
                                                                arrMotorAssignment[index].VDMBinNo = binno;
                                                            } else {
                                                                arrMotorAssignment.push(data);
                                                            }
                                                        } else {
                                                            data.IsUseUOM = false;
                                                            data.IsDonotIncludeBaseUom = false;
                                                        }

                                                        if(CBool(data.IsGroup)){
                                                            data.Name = val;
                                                            data.DefaultOtherPaymentID = $('#selChangeDefaultOPT').val();
                                                        } else {
                                                            data.ShortName = val;
                                                        }
                                                        $('#name-' + data.ID).html((CBool(data.IsGroup) ? '<b><span class="required">*</span> ' + data.Name + '</b>' : data.ShortName + '<br>(' + data.ProductNo + ' - ' + data.Name + ')') + (CStr2(data.VDMMotorNo) == '' ? '' : '<br> Motor No.: ' + data.VDMMotorNo) + (CStr2(data.VDMBinNo) == '' ? '' : '<br> Bin No.: ' + data.VDMBinNo));
                                                        $('#popRenameHB').modal('hide');
                                                    }                                                    
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
                                    .append($('<li>')
                                        .append($('<a class="dropdown-item">').data('data', data).append('<span class="bi-house-check text-info"></span> &nbsp; Store Assignment')
                                            .click(function (e) {
                                                var data = $(this).data('data');
                                                var popStoreAssignment = $('#popStoreAssignment');
                                                var lstHBAssignedStores = $('#lstHBAssignedStores').empty();

                                                popStoreAssignment.find('i').text(data.Name);

                                                for (var i = 0 in data.StoreAssignment) {
                                                    AddToFowstList(data.StoreAssignment[i]);
                                                }

                                                $('#btnAddStoreAssignment').unbind('click').click(function () {
                                                    PopupButtonList('', 'Options', [
                                                        {
                                                            Text: 'Select from Company\'s FOWST list', Click: function () {
                                                                BrowsePopup('LibraryFowst', 'Select FOWST Locations',
                                                                    [
                                                                        {
                                                                            AutoSearch: true,
                                                                            SelectMultiple: true,
                                                                            AllowToSelectAll: true,
                                                                            Message: 'You can select multiple records. Click OK when you\'re done.',
                                                                            DisplayText: [{ Column: 'FowstName' }, { Column: 'Address' }],
                                                                            Click: function (e) {
                                                                                for (var i = 0; i < e.length; i++) {
                                                                                    AddToFowstList(e[i]);
                                                                                }
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
                                                                                    for (var i = 0; i < e.Results.Rows.length; i++) {
                                                                                        AddToFowstList(e.Results.Rows[i]);
                                                                                    }
                                                                                }
                                                                            })
                                                                        }
                                                                    }
                                                                ])
                                                            }
                                                        }, {
                                                            Text: 'Select from Franchisee\'s FOWST list', Click: function () {
                                                                BrowsePopup('LibraryOrganizationFranchiseeFowst', 'Select Franchisee\'s FOWSTs', [
                                                                    {
                                                                        DisplayText: [{ Column: 'FowstName' }, { Column: 'Address' }],
                                                                        AutoSearch: true,
                                                                        SelectMultiple: true,
                                                                        Message: 'You can select multiple records. Click OK when you\'re done.',
                                                                        Click: function (e) {
                                                                            for (var i = 0; i < e.length; i++) {
                                                                                AddToFowstList(e[i]);
                                                                            }
                                                                        }
                                                                    }
                                                                ])
                                                            }
                                                        }
                                                    ])
                                                })

                                                $('#btnOkHBAssignedStores').unbind('click').click(function () {
                                                    var arrStores = [];

                                                    lstHBAssignedStores.find('li').each(function () {
                                                        arrStores.push($(this).data('data'));
                                                    })

                                                    data.StoreAssignment = []; //remove
                                                    data.StoreAssignment = arrStores;
                                                    popStoreAssignment.modal('hide');
                                                })

                                                popStoreAssignment.modal('show');
                                            })
                                        )
                                    )

                    dropdown.append(options)
                    div.append(dropdown)                       

                    if (CBool(data.IsGroup)) {
                        div.append('&nbsp;').append($('<button data-role="none" stat="hide" class="btn btn-light btn-sm" id="dd-' + data.ID + '">')
                            .data('data', data)
                            .append('<span class="bi-caret-down-fill"></span>')
                            .click(function () {
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
                                        url: 'ProductGroup/GetSubGroup',
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
                                                data.SubGroup[i].Path = (data.SubGroup[i].Path == undefined ? '' : data.SubGroup[i].Path + ' > ') + data.Name;
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

                        li.append($('<ul id="ul-' + data.ID + '" class="list-group draggableList" style="margin-left:35px; display:none;">'))

                        container.append(li)

                        for (var i = 0; i < data.SubGroup.length; i++){
                            GenerateList(data.SubGroup[i], $('#ul-' + data.ID))
                        }

                        SortList('ul-' + data.ID);
                    } else {
                        container.append(li)
                    }                    
                }

                function PopulateVDMMotorsNo(type, arrAssigned, motors) {
                    var arrMotorList = [], assignedMotor = [], container = type == 'motor' ? 'txtMotorNo' : 'txtBinNo';
                    var curSelectedMotorNo = $('#' + container).val().split(', ').map(Number);

                    for (var i = 1; i <= motors; i++) {
                        if (arrAssigned.includes(i) && !curSelectedMotorNo.includes(i)) {
                            continue;
                        }

                        arrMotorList.push({
                            Text: i,
                            data: { MotorNo: i },
                            ClassName: (curSelectedMotorNo.includes(i) ? 'browseSelected' : ''),
                            Click: function (motors) {
                                for (var i in motors) {
                                    var motorno = motors[i].data.MotorNo;
                                    if (!arrAssigned.includes(motorno)) {
                                        arrAssigned.push(motorno)
                                    }

                                    assignedMotor.push(motorno)
                                }

                                $('#' + container).val(assignedMotor.sort().join(', '))
                            }
                        })
                    }

                    PopupButtonList('Note: You can assign multiple VDM ' + (type == 'motor' ? 'motors' : 'bins') + '.', 'Select ' + (type == 'motor' ? 'Motors' : 'Bins'), arrMotorList, { SelectMultiple: true })
                }

                function AddToFowstList(data) {
                    var IsRoleExist = false;

                    $('#lstHBAssignedStores li').each(function (m_index) {
                        if ($.trim($(this).attr('ID')) == data.ID) {
                            IsRoleExist = true;
                        }
                    });

                    if (IsRoleExist) {
                        WebToast(data.FowstName + ' already exist in a list.', 'top');
                        return;
                    }

                    //$('#panSearchLibrary').popup('close');
                    $('#lstHBAssignedStores').append($('<li class="list-group-item list-group-item-action">')
                        .attr('ID', data.ID)
                        .data('data', data)
                        .append($('<table style="width:100%;">')
                            .append($('<tr>')
                                .append($('<td style="word-break:break-all;">')
                                    .append($('<h5>')
                                        .attr("style", "white-space:normal")
                                        .text(data.FowstName)
                                    )
                                )
                                .append($('<td style="width:50px; text-align:center;">').append($('<span class="btn btn-xs btn-danger">')
                                    .append($('<span class="bi-trash-fill" aria-hidden="true">'))
                                    .attr('fID', data.ID)
                                    .click(function (e) {
                                        e.preventDefault();
                                        $('#' + $(this).attr('fID')).remove();
                                    })
                                )
                                )
                            )
                        ));
                    $('#lstOrgEmployeeFowst');//.listview('refresh');
                }

                function BrowseBrandsOrCategory(type, callback) {
                    var url;

                    if (type == 'Brand') {
                        url = 'LibraryPimBrands';
                    } else {
                        url = 'LibraryPimCategories';
                    }

                    BrowsePopup(url, 'Search ' + type, [
                        {
                            DisplayText: [{ Column: type }],
                            AutoSearch: true,
                            SelectMultiple: true,
                            Message: 'You can select multiple records. Click "Ok" when you\'re done.',
                            Click: function (e) {
                                Bs.Loading('show');

                                var arrText = [], vData = { FilterByBrandAndCategory: true };

                                for (var i in e) {
                                    arrText.push(e[i][type]);
                                }

                                if (type == 'Brand') {
                                    vData.Brands = arrText;
                                } else {
                                    vData.Categories = arrText;
                                }

                                AjaxSend({
                                    url: 'Browser/LibraryPIMProduct',
                                    data: vData,
                                    success: function (e) {
                                        Bs.Loading('hide');
                                        var vRows = e.Results.Rows;

                                        if (vRows.length > 0) {
                                            callback(e);                                            
                                        } else {
                                            NewMessageBox('Sorry! The selected ' + type + ' may not have any products available, or it may have been deactivated.');
                                        }
                                    }
                                })
                            }
                        }
                    ])
                }

                function ReloadPage() {
                    window.location.hash = '';
                    window.location.hash = 'UniversalPage';
                    AjaxSend({
                        ExactUrl: '/ProductGroup/MenuTemplate?cache=1&OrgID=' + sessionStorage.OrgID + '&IsMobile=' + IsMobile,
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
                        Name: $.trim($('#txtEditName').val())
                    };
                    AjaxSend({
                        url: 'ProductGroup/CreateTemplate',
                        data: data,
                        success: function (vData) {
                            if (vData.Result == 'ok') {
                                //location.reload();
                                GetMenuTemplateName();
                                $('#popEditTemplateName').modal('hide');
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

                    AjaxSend({
                        url: 'ProductGroup/NewGetHotButtons',
                        data: { MenuID: sessionStorage.ViewID },
                        success: function (e) {
                            if ($.isArray(e.Results)) {
                                arrHotButtons = e.Results;
                                arrAssignedFowst = e.AssignedFowst;
                                DisplayNewHB(e.Results, $('#lstHotButtonList'));
                                AssignedFOWST(arrAssignedFowst);
                                ListOfFowstNoTemplate(e.UnAssignedFowst);                                
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
                            url: '/ProductGroup/GetTree',
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

                function GetOtherPayments() {
                    AjaxSend({
                        url: '/POSSettings/GetOtherPayments',
                        success: function (e) {
                            var selDefaultOPT = $('#selDefaultOPT');
                            var selChangeDefaultOPT = $('#selChangeDefaultOPT');

                            selDefaultOPT.empty().append(new Option('No default', ''));
                            selChangeDefaultOPT.empty().append(new Option('No default', ''));

                            for (var i = 0; i < e.Results.Rows.length; i++) {
                                selDefaultOPT.append(new Option(e.Results.Rows[i].Description, e.Results.Rows[i].Value));
                                selChangeDefaultOPT.append(new Option(e.Results.Rows[i].Description, e.Results.Rows[i].Value));
                            }
                        }
                    })
                }

                function LoadHotButtons(vData, level, txtIndent) {
                    var tbody = $('#tBodyHotButtons');

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
                        LoadHotButtonsProducts(vData.Products, (vData.Data == undefined ? undefined : $('#tBodyHotButtons tr td div[id="' + vData.Data.ID + '"]').attr('level')))
                    }
                }

                function LoadHotButtonsProducts(vData, level) {
                    var tbody = $('#tBodyHotButtons');
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

                        msg = 'Are you sure you want to delete this ' + item + ' and all the items under it?<br><br>If yes, it cannot be undone.';
                    }
                    //$('#popupMenu').popup('close');
                    //$('#popupMenuProduct').popup('close');

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
                        url: '/ProductGroup/DeleteItem',
                        data: {
                            ID: id,
                            Item: item
                        },
                        success: function (vData) {
                            //$('#panOrgEmpMessage').popup('close');
                            if (vData.Result == 'ok') {
                                if (item == 'template') {
                                    //                                location = MergeUrlParameters({
                                    //                                    OrgID: sessionStorage.OrgID,
                                    //                                    IsMobile: CBool(IsMobile) ? 1 : 0
                                    //                                }, '/ProductGroup');
                                    window.location.hash = 'LibraryPage';
                                    LibraryTable.HotButtons();
                                } else {
                                    //                                location = MergeUrlParameters({
                                    //                                    OrgID: sessionStorage.OrgID,
                                    //                                    MenuID: sessionStorage.ViewID, //add menu id to query string
                                    //                                    IsMobile: CBool(IsMobile) ? 1 : 0
                                    //                                }, '/ProductGroup/MenuTemplate');
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

                            //$('#popupNewSKUName').popup('open');
                            $('#lblFullProductName').text(vDataRow.ProductName);
                            var string = vDataRow.ProductName;
                            var length = 50;
                            var trimmedString = string.substring(0, length);
                            $('#txtName_SKU').val(trimmedString);


                            $('#hdnSKUid').val(vDataRow.ID);
                            $('#hdnGroupId').val(vDataRow.GroupID);

                        } else {
                            vItem.append($('<a>')
                            .text(vDataRow.ShortName + ' (' + vDataRow.ProductName + ')')
                            .data('data', vDataRow)
                            .attr('href', '#')
                            .click(function (e) {
                                var pData = $(e.target).data().data;
                                NewMessageBox('Are you sure you want to delete ' + vDataRow.ShortName + ' (' + vDataRow.ProductName + ')' + '?', 'Delete SKU', [
                                    {
                                        Text: 'Yes', Click: function () {
                                            DeleteItem(vDataRow.ID, 'sku');
                                        }
                                    }, { Text: 'No' }
                                ])
                            })
                        );
                        }
                    }; // OnItem

                    vParentDiv.append(pLV.Element);
                    pLV.SetData(vData.Rows);

                }; // END LoadProducts

                function ClearFields() {
                    $('#lstHotButtonList, #listHBFowst').empty();
                    arrHotButtons = [];
                    arrAssignedFowst = [];
                    SortList('lstHotButtonList');
                }

                //        p.collapsible({
                //                                collapsed: false,
                //                                contentTheme: $.mobile.page.prototype.options.contentTheme

                //                            });

                //        divProducts.hide();
                //        GetData();

                //        function ClearGroup() {

                //            txtName.val('').focus();
                //            chkActive.prop('checked', true);
                //            try {
                //                chkActive.checkboxradio('refresh');
                //            } catch (e) {};

                //            divMain.show();
                //            divGroups.hide();
                //            divProducts.hide();

                //        }; // END ClearGroup

                //        function GetData() {

                //            divMain.hide();
                //            divGroups.hide();
                //            divProducts.hide();

                //            ClearGroup();
                //            AjaxSend({
                //                url: '/ProductGroup/GetTree',
                //                data: {
                //                    ID: mID
                //                },
                //                success: function(vData) {
                //                    LoadData(vData.Data);
                //                    LoadGroup(vData.Subgroups);
                //                    if (vData.Products != undefined)
                //                        mProducts = vData.Products.Rows;
                //                    else
                //                        mProducts = [];
                //                    LoadProducts();
                //                }
                //            }); // END Ajax

                //        }; // END LoadData

                //            function LoadBranch(vParentDiv, vData) {
                //                alert(vData);
                //                                var pLV = $('#tblDashboardBodyDetails');

                //                                pLV.html('');

                //                                for (var i = 0; i <= vData.Details.Rows.length - 1; i++) {

                //                                    $('#hdrCompanyName').text(vData.Details.Rows[i].CompanyName);
                //                                    $('#lblOfficeAddress').text(vData.Details.Rows[i].OfficeAddressBuildingNo + '  ' + vData.Details.Rows[i].OfficeAddressStreet + ', ' + vData.Details.Rows[i].OfficeCityName);
                //                                    $('#lblBusinessAddress').text(vData.Details.Rows[i].BusinessAddressBuildingNo + '  ' + vData.Details.Rows[i].BusinessAddressStreet + ', ' + vData.Details.Rows[i].BusinessCityName);
                //                                    $('#lblPromoCode').text(vData.Details.Rows[i].PromoCode);
                //                                    $('#lblAgentName').text(vData.Details.Rows[i].AgentName);
                //                                    $('#lblSubscriptionDate').text(vData.Details.Rows[i].SubscriptionDate);

                //                                    pItem = $('<tr>')
                //                                                            .append($('<th>')
                //                                                                .text(vData.Details.Rows[i].ProductName)
                //                                                            )
                //                                                            .append($('<td>')
                //                                                                .text(vData.Details.Rows[i].Qty)
                //                                                            )
                //                                                            .append($('<td>')
                //                                                                .text(vData.Details.Rows[i].SubscriptionStatus)
                //                                                            )
                //                                                            .append($('<td>')
                //                                                                .text(vData.Details.Rows[i].ActivationDate)
                //                                                            )
                //                                    pLV.append(pItem)
                //                                } //end for
                //            };

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
                            chkActive.checkboxradio('refresh');
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



                //        $('#butAddSubgroup_ProductGroup').click(function() {

                //            mNewMode = true;
                //            mParentID = mID;
                //            mID = null;
                //            ClearGroup();

                //        }); // END butAddSubgroup.click



                //            function LoadProducts() {
                //                alert('hello');
                //                var pLV;
                //                var pDiv;

                //                pDiv = $('#divProductList_ProductGroup');
                //                pDiv.html('');
                //                divGroups.show();

                //                pLV = new CreateListView();
                //                pLV.OnItem = function (vDataRow, vItem, e) {
                //                    
                //                    vItem.append($('<a>')
                //                        .text(vDataRow.ProductName)
                //                        .data('data', vDataRow)
                //                        .attr('href', '#')
                //                        .click(function (e) {
                //                            var pData = $(e.target).data().data;

                //                            MessageBox('ProductName: ' + pData.ProductName);
                //                        })
                //                    );

                //                }; // OnItem

                //                pDiv.append(pLV.Element);

                //                if (mProducts == undefined)
                //                    return;

                //                pLV.SetData(mProducts);
                //            }; // END LoadProducts


                function SaveProductName() {
                    AjaxSend({
                        url: '/ProductGroup/SaveProductName',
                        data: {
                            ID: ($('#hdnSKUid').val() == '' ? sessionStorage.PreviousProductID : $('#hdnSKUid').val()),
                            GroupID: $('#hdnGroupId').val(),
                            MenuID: sessionStorage.ViewID,
                            ShortName: $.trim($("#txtName_SKU").val())
                        },
                        success: function (vData) {
                            if (vData.Result == 'ok') {
                                //                            location = MergeUrlParameters({
                                //                                OrgID: sessionStorage.OrgID,
                                //                                MenuID: sessionStorage.ViewID, //add menu id to query string
                                //                                IsMobile: CBool(IsMobile) ? 1 : 0
                                //                            }, '/ProductGroup/MenuTemplate');
                                //$('#popupNewSKUName').popup('close');
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
                        url: '/ProductGroup/SaveProduct',
                        data: {
                            GroupID: mParentID,
                            ProductID: vProductID,
                            MenuID: sessionStorage.ViewID
                        },
                        success: function (vData) {
                            //                        if (vData.Error == undefined) {

                            $('#lblFullProductName').text(vProductName);
                            var string = vProductName;
                            var length = 50;
                            var trimmedString = string.substring(0, length);
                            //$('#txtName_SKU').val(trimmedString);

                            $('#hdnSKUid').val(vData.SKUData);
                            $('#hdnGroupId').val(mParentID);

                            if (vData.Error != undefined) {
                                NewMessageBox(vData.Error, 'New Product')
                                return;
                            } else {
                                if (sessionStorage.PreviousProductID == vProductID) {
                                    //location.reload();
                                    //$('#panSearchLibrary').popup('close');
                                    //$('#popupNewSKUName').popup('open');
                                } else {
                                    //$('#panSearchLibrary').popup('close');
                                    //$('#popupNewSKUName').popup('open');
                                    isEnterShortName = 'true';
                                }
                                SaveProductName();
                            }
                            //                        try {

                            //                            if (vData.SKUData != undefined) {
                            //                                $('#popupNewSKUName').popup('open');
                            //                                isEnterShortName = 'true';

                            //                                return;
                            //                            }


                            //                        } catch (ex) {
                            //                            alert(ex.Message);
                            //                            if (vData.SKUData != undefined) {
                            //                                $('#popupNewSKUName').popup('open');
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
                            url: '/ProductGroup/GetMenuTemplateName',
                            data: {
                                ID: sessionStorage.ViewID
                            },
                            success: function (vData) {
                                if (vData.Error == undefined) {
                                    $('#hdrTemplateName').text(vData.Result.Rows[0].Name);
                                    $('#chbIsDisplayAll').prop('checked', CInt2(vData.Result.Rows[0].IsDisplayAllProducts));

                                    proj.CheckIfVDMFeaturesIsActivated(function (e) {
                                        IsEnableVDMFeatures = CBool(e.EnableVDMFeatures);
                                        if (IsEnableVDMFeatures && CStr2(vData.Result.Rows[0].VDMModelID) != '') {
                                            $('#selVDMModel').empty();
                                            for (var i in e.VDMMachines.Rows) {
                                                $('#selVDMModel').append(new Option(e.VDMMachines.Rows[i].Name, e.VDMMachines.Rows[i].ID));
                                            }
                                            VDMNoOfMotors = CInt2(vData.Result.Rows[0].VDMNoOfMotors);
                                            VDMNoOfBins = CInt2(vData.Result.Rows[0].VDMNoOfBins);
                                            arrMotorAssignment = vData.ArrMotorAssignment.Rows;
                                            arrAssignedMotor = [];
                                            arrAssignedBin = [];
                                            $('#selVDMModel').val(vData.Result.Rows[0].VDMModelID);
                                            $('.divVDMModel').show();
                                            //$('#btnSaveHB').text('View Motor Assignments');

                                            for (var i in arrMotorAssignment) {
                                                var motors = arrMotorAssignment[i].VDMMotorNo.split(', ');
                                                for (var j in motors) {
                                                    arrAssignedMotor.push(CInt2(motors[j]));
                                                }

                                                motors = arrMotorAssignment[i].VDMBinNo.split(', ');
                                                for (var j in motors) {
                                                    arrAssignedBin.push(CInt2(motors[j]));
                                                }
                                            }

                                            arrAssignedMotor = arrAssignedMotor.sort();
                                            arrAssignedBin = arrAssignedBin.sort();
                                        } else {
                                            IsEnableVDMFeatures = false;
                                            $('#btnSaveHB').text('Save');
                                            $('.divVDMModel').hide();
                                        }
                                    })
                                }
                                else
                                    NewMessageBox(vData.Error);
                            }
                        }); // END AjaxSend

                    } else {
                        NewMessageBox('Invalid MenuID');
                    }



                }; // END SaveProduct

                function CreateTableHotButtons() {
                    var i = 0;
                    $('#tBodyHotButtons').empty().append($('<tr id="' + i + '" rowtype="normal">')
                                .append(
                                    $('<td>')
                                    .append($('<div>')
                                        .css({ width: '100%', fontWeight: 'bold' })
                                        .html('HOME')
                                        .attr('data-role', 'none')
                                        .attr('columnName', 'description')
                                        .attr('state', 'add')
                                        .attr('celltype', 'home')
                                    )
                                    .addClass('tableCell')
                                )
                            )
                    $('#tBodyHotButtons tr:eq(0)').addClass('activeRow');
                    $('#tBodyHotButtons td:eq(0)').addClass('activeCell');
                }
            }) // end $(document)
            .keydown(function (e) {
                if (sessionStorage.AddViewLibBCName == "View Hot Button") {
                    var currentCellIndex = $('#tBodyHotButtons tr td.activeCell').closest('td').index();

                    if (e.which == 40) { // down arrow
                        $('#tBodyHotButtons tr td.activeCell').closest('tr').next().find('td:eq(' + currentCellIndex + ')').addClass('activeCell');
                        $('#tBodyHotButtons tr td.activeCell').closest('tr').prev().find('td:eq(' + currentCellIndex + ')').removeClass('activeCell');
                    }
                    else if (e.which == 38) { // up arrow
                        $('#tBodyHotButtons tr td.activeCell').closest('tr').prev().find('td:eq(' + currentCellIndex + ')').addClass('activeCell');
                        $('#tBodyHotButtons tr td.activeCell').closest('tr').next().find('td:eq(' + currentCellIndex + ')').removeClass('activeCell');
                    }
                    else {
                        e.stopPropagation();
                    }
                }
            })
        }); // $(function () {
    });                                                                                                                                                                                                          // require(['common'