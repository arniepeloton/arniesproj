require(['common',
    '/Scripts3/Browser/LibraryPIMProduct.js?crc='
    ], function () {

        $(function () {


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

            GetMenuTemplateName();
            divProductGroup.hide();
            GetData();

            function LoadBranch(vParentDiv, vData, level) {

                var pDiv;
                var pDivGroup;
                var pIndex;

                pDiv = divProductGroup.clone();
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


            $('#btnEditName').click(function (e) {
                e.preventDefault();

                $('#hdrTemplateName').hide();
                $('#divEditName').show();
                $('#txtEditName').val($('#hdrTemplateName').text());
                $('#txtEditName').focus();

                $('#btnSaveName').show();
                $('#btnEditName').hide();
                $('#btnCancel').show();
                $('#btnDeleteTemplate').hide();
            });

            $('#btnCancel').click(function (e) {
                e.preventDefault();

                $('#hdrTemplateName').show();
                $('#divEditName').hide();
                $('#txtEditName').val('');

                $('#btnSaveName').hide();
                $('#btnEditName').show();
                $('#btnCancel').hide();
                $('#btnDeleteTemplate').show();
            });

            $('#btnSaveName').click(function (e) {
                e.preventDefault();

                if ($.trim($("#txtEditName").val()) == "") {
                    MessageBox("Please enter a template name.",
                            "Template Name",
                            [
                                { Text: 'Ok' }
                            ]
                );
                    $("#txtEditName").focus();
                    return;
                }

                SaveTemplateName();
            });

            $('#btnAssignToFOWST').click(function (e) {
                e.preventDefault();

                location = MergeUrlParameters({
                    OrgID: sessionStorage.OrgID,
                    MenuID: querySt('MenuID'), //add menu id to query string
                    IsMobile: CBool(IsMobile) ? 1 : 0
                }, '/ProductGroup/AssignFOWST');
            });

            $('#btnDeleteTemplate').click(function (e) {
                e.preventDefault();

                DeleteItem(querySt('MenuID'), "template");

            });

            $('#btnHotButtonsTemplateDone').click(function (e) {
                e.preventDefault();

                location = MergeUrlParameters({
                    OrgID: sessionStorage.OrgID,
                    IsMobile: CBool(IsMobile) ? 1 : 0
                }, '/ProductGroup');
            });


            $('#butSave_ProductGroup').click(function (e) {
                e.preventDefault();
                if ($.trim(txtName.val()) == '') {
                    MessageBox('Please enter a name for this group.',
                                    'New Group',
                                    [
                                        { Text: 'OK' }
                                    ]
                                ); // End MessageBox
                    return;
                } else {

                    var mMenuID = querySt("MenuID");

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
                                mID = vData.ID;
                                mParentID = vData.ParentID;

                                txtName.val('');

                                location = MergeUrlParameters({
                                    OrgID: sessionStorage.OrgID,
                                    MenuID: querySt('MenuID'), //add menu id to query string
                                    IsMobile: CBool(IsMobile) ? 1 : 0
                                }, '/ProductGroup/MenuTemplate');


                            } else if (vData.Error != undefined)
                                MessageBox(vData.Error);
                        }
                    });
                }

            }); // END butSave.click


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
                    ID: querySt('MenuID'),
                    Name: $.trim($('#txtEditName').val())
                };
                AjaxSend({
                    url: 'ProductGroup/CreateTemplate',
                    data: data,
                    success: function (vData) {
                        if (vData.Result == 'ok') {
                            location.reload();

                        }
                        else {
                            MessageBox(vData.Error);
                        }
                    }
                });
            }

            function GetData() {
                var menuID;

                if (querySt("MenuID")) {
                    menuID = querySt("MenuID");
                } else {
                    menuID = '';
                }

                if (menuID != '') {
                    var pData = {};
                    pData.MenuID = menuID;

                    AjaxSend({
                        url: '/ProductGroup/GetTree',
                        data: pData,
                        success: function (vData) {
                            LoadBranch(null, vData);
                        }
                    }); // END Ajax
                } else {
                    MessageBox('Invalid ID');
                }

            }; // END LoadData

            function DeleteItem(id, item) {
                var msg;

                if (item == 'sku') {
                    msg = 'Are you sure you want to delete this item?';

                    DeleteThis(id, item);
                } else {

                    msg = 'Are you sure you want to delete this ' + item + ' and all the items under it?';

                    MessageBox(msg,
                                    'Delete Record',
                                    [
                                        { Text: 'Yes',
                                            Click: function () {

                                                DeleteThis(id, item);

                                            } // end click function
                                        }// end Text Yes
                                        ,
                                        { Text: 'No'
                                        }
                                    ]
                                ); // End MessageBox
                }


            } //End of DeleteItem

            function DeleteThis(id, item) {
                AjaxSend({
                    url: '/ProductGroup/DeleteItem',
                    data: {
                        ID: id,
                        Item: item
                    },
                    success: function (vData) {
                        if (vData.Result == 'ok') {
                            if (item == 'template') {
                                location = MergeUrlParameters({
                                    OrgID: sessionStorage.OrgID,
                                    IsMobile: CBool(IsMobile) ? 1 : 0
                                }, '/ProductGroup');

                            } else {
                                location = MergeUrlParameters({
                                    OrgID: sessionStorage.OrgID,
                                    MenuID: querySt('MenuID'), //add menu id to query string
                                    IsMobile: CBool(IsMobile) ? 1 : 0
                                }, '/ProductGroup/MenuTemplate');                                
                            }

                        } else if (vData.Error != undefined)
                            MessageBox(vData.Error);
                    } // end success
                }); //end of AjaxSend
            }

            function LoadProducts(vParentDiv, vData) {
                var pLV;

                pLV = new CreateListView();
                pLV.OnItem = function (vDataRow, vItem, e) {
                    //if no short name, force them to put shortname!

                    if (vDataRow.ShortName == undefined) {

                        $('#popupNewSKUName').popup('open');
                        $('#lblFullProductName').text(vDataRow.ProductName);
                        $('#hdnSKUid').val(vDataRow.ID);
                        $('#hdnGroupId').val(vDataRow.GroupID);

                    } else {
                        vItem.append($('<a>')
                            .text(vDataRow.ShortName + ' (' + vDataRow.ProductName + ')')
                            .data('data', vDataRow)
                            .attr('href', '#')
                            .click(function (e) {
                                var pData = $(e.target).data().data;

                                MessageBox('Are you sure you want to delete ' + vDataRow.ShortName + ' (' + vDataRow.ProductName + ')' + '?',
                                    'Delete SKU',
                                    [
                                        { Text: 'Yes',
                                            Click: function () {
                                                DeleteItem(vDataRow.ID, 'sku');
                                            }
                                        },
                                        { Text: 'No'
                                        }
                                    ]
                                ); //end messageBox
                            })
                        );
                    }
                }; // OnItem

                vParentDiv.append(pLV.Element);
                pLV.SetData(vData.Rows);

            }; // END LoadProducts

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
            $('#butAddProduct_ProductGroupFirst').click(function () {

                Browser.LibraryPIMProduct.Click = function (e, vOpt) {

                    mProducts.push({
                        ProductID: e.ID,
                        ProductName: e.ProductName
                    });


                    SaveProduct(e.ID, e.ProductName);

                };

                Browser.LibraryPIMProduct.Show();

            }); // END butAddProduct.click

            $('#butAddProduct_ProductGroup').click(function () {

                Browser.LibraryPIMProduct.Click = function (e, vOpt) {

                    mProducts.push({
                        ProductID: e.ID,
                        ProductName: e.ProductName
                    });


                    SaveProduct(e.ID, e.ProductName);

                };

                Browser.LibraryPIMProduct.Show();

            }); // END butAddProduct.click

            $('#butAddProduct_ProductSKU').click(function () {

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

            }); // END butAddProduct.click

            $('#butDelete_ProductGroup').click(function () {
                DeleteItem(mParentID, 'group');
            }); // END butAddProduct.click

            $('#butDelete_ProductSKU').click(function () {
                DeleteItem(mParentID, 'group');
            }); // END butAddProduct.click


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

            $('#butSave_SKU').click(function (e) {
                e.preventDefault();

                if ($.trim($("#txtName_SKU").val()) == "") {
                    MessageBox("Please enter a short name",
                            "SKU Short Name",
                            [
                                { Text: 'Ok',
                                    Click: function () {
                                        location = MergeUrlParameters({
                                            OrgID: sessionStorage.OrgID,
                                            MenuID: querySt('MenuID'), //add menu id to query string
                                            IsMobile: CBool(IsMobile) ? 1 : 0
                                        }, '/ProductGroup/MenuTemplate');
                                    }

                                } // end click function }
                            ]
                );
                    $("#txtName_SKU").focus();
                    return;
                }

                SaveProductName();

            }); // END 

            function SaveProductName() {


                AjaxSend({
                    url: '/ProductGroup/SaveProductName',
                    data: {
                        ID: ($('#hdnSKUid').val() == '' ? sessionStorage.PreviousProductID : $('#hdnSKUid').val()),
                        GroupID: $('#hdnGroupId').val(),
                        MenuID: querySt('MenuID'),
                        ShortName: $.trim($("#txtName_SKU").val())
                    },
                    success: function (vData) {
                        if (vData.Result == 'ok') {
                            location = MergeUrlParameters({
                                OrgID: sessionStorage.OrgID,
                                MenuID: querySt('MenuID'), //add menu id to query string
                                IsMobile: CBool(IsMobile) ? 1 : 0
                            }, '/ProductGroup/MenuTemplate');

                            isEnterShortName = 'false';
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
                        MenuID: querySt('MenuID')
                    },
                    success: function (vData) {
                        //                        if (vData.Error == undefined) {

                        $('#lblFullProductName').text(vProductName);
                        $('#hdnSKUid').val(vData.SKUData);
                        $('#hdnGroupId').val(mParentID);

                        if (vData.Error != undefined && sessionStorage.PreviousProductID != vProductID) {
                            MessageBox(vData.Error,
                               "New Product",
                                [
                                    { Text: 'Ok',
                                        Click: function () {
                                            location.reload();
                                        }
                                    }
                                ]
                            );
                            return;
                        } else {
                            if (sessionStorage.PreviousProductID == vProductID) {
                                location.reload();
                            } else {
                                $('#popupNewSKUName').popup('open');
                                isEnterShortName = 'true';
                            }
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
                if (querySt('MenuID')) {
                    AjaxSend({
                        url: '/ProductGroup/GetMenuTemplateName',
                        data: {
                            ID: querySt('MenuID')
                        },
                        success: function (vData) {
                            if (vData.Error == undefined) {
                                $('#hdrTemplateName').text(vData.Result);
                            }
                            else
                                MessageBox(vData.Error);
                        }
                    }); // END AjaxSend

                } else {
                    MessageBox('Invalid MenuID');
                }



            }; // END SaveProduct
        }); // $(function () {
    });                                                                                                                     // require(['common'