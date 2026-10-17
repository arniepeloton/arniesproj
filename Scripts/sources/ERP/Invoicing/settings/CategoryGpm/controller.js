define([
    'promise!/Scripts/sources/ERP/Invoicing/settings/CategoryGpm/model.js'
], function () {

    var mDeferred = $.Deferred();
    var mHtml;

    (function staticInit() {

        AjaxSend({
            ExactUrl: '/Scripts/sources/ERP/Invoicing/settings/CategoryGpm/view.html?v=' + assetversion,
            type: 'GET',
            dataType: 'text',
            success: function (vData) {
                mHtml = vData;
                ERP.Invoicing.settings.CategoryGpm.controller = ERP.Invoicing.settings.CategoryGpm.controller(); // singleton
                mDeferred.resolve();
                
            }
        });

    })();

    CreateStringObject('ERP.Invoicing.settings.CategoryGpm.controller', null, function () {

        var mOut = {};
        var mModel = new ERP.Invoicing.settings.CategoryGpm.model();
        var mElement;

        mElement = $(mHtml);
        mOut.element = mElement;
        
        var mTable = mElement.find('[name=tableCategories]');
        var mDataTable;

        mElement.find('[name=butAddCategory]').click(function(e) {
            e.preventDefault();
            AddCategory();
            return false;
        }); // END mElement.find('[name=butAddProduct]').click(function() {

        mElement.find('[name=butDeleteCategory]').click(function(e) {
            e.preventDefault();
            PromptDeleteCheckedItems();
            return false;
        }); // END mElement.find('[name=butAddProduct]').click(function() {

        function ClearTable() {
            if ($.fn.DataTable.isDataTable(mTable)) {
                //empty the table first before destroying datatabase para dili magdoble ang header
                mTable.empty();
                mDataTable.destroy();
            }
        }

        function LoadData() {
            mModel.GetData({
                Success: function(vData) {
                    ClearTable();    
                    DisplayTable(vData.Results.Rows);
                }
            });
        } // END function LoadData() {

        function DisplayTable(vRows) {
            mDataTable = mTable.DataTable({
                data: vRows,
                columns: [
                    {
                        'searchable':false,
                        'orderable':false,
                        'width':'1%',
                        'className': 'text-center',
                        'render': function (data, type, full, meta){
                            return '<input type="checkbox" class="form-check-input" name="chkSelector">';
                        }
                    },
                    //{
                    //    render: function(v1, v2, vData) {
                    //        if (vData.DepositStatus == '1')
                    //            return 'Deposited';
                    //        else if (vData.DepositStatus == '2')
                    //            return 'Returned';
                    //        else
                    //            return '';
                    //    }
                    //},
                    {
                        data: function(vData) {
                            return vData.Category;
                        }
                    },
                    {
                        data: function(vData) {
                            return AccountingFormat(vData.Gpm);
                        }
                    }
                ],
                'rowCallback': function(row, data, dataIndex) {
                    var pRow = $(row);
                    pRow.data('data', data);
                    pRow.click(function() {
                        CategoryPopup(data.Category, AccountingFormat(data.Gpm));
                    });
                    $(row).find('[name=chkSelector]').prop('checked', data.Checked);
                    $(row).find('[name=chkSelector]').unbind('click').click(CheckBox_OnClick);
                }, initComplete: function () {
                    mTable.removeClass('dataTable');
                }
            });

            mTable.find('[name=select_all]').unbind('click').click(function(e) {

                UnbindCheckBoxes();
                if(this.checked){
                    mTable.find('tbody input[type="checkbox"]:not(:checked)').trigger('click');
                } else {
                    mTable.find('tbody input[type="checkbox"]:checked').trigger('click');
                }
                
                e.stopPropagation();
            });
            BindCheckBoxes();
        }

        function BindCheckBoxes() {
            mTable.find('[name=chkSelector]').unbind('click').click(CheckBox_OnClick);
        }

        function CheckBox_OnClick(e) {
            e.stopPropagation();
            var pCheckedBox = $(this);
            var pTr = $(pCheckedBox).closest('tr');
            pTr = $(pTr);
            var pDR = pTr.data('data');
            pDR.Checked = this.checked;
        }

        function UnbindCheckBoxes() {
            mTable.find('[name=chkSelector]').unbind('click');
        }

        function AddCategory() {
            BrowsePopup('PimCategories', 'Category',
                [
                    {
                        AutoSearch: false,
                        DisplayText: [
                            { 
                                Column: 'Name', 
                                Label: 'Category' 
                            }
                        ],
                        Click: function (e) {
                            CategoryPopup(e.Name);
                        },
                        AllowEmptySearch: true,
                        StyleFormat: 'table'
                    }
                ], { }
            ); // END BrowsePopup('LibraryFowst', 'Fowst',
        } // END function AddCategory() {

        var mCheckedData;

        function PromptDeleteCheckedItems(vText, vValue) {
            
            var pMsg;

            mCheckedData = [];
            
            var pData = mDataTable.data();
            for (var p = 0; p <= pData.length - 1; p++) {
                var pDR = pData[p];
                if (pDR.Checked)
                    mCheckedData.push(pDR.Category);
            }

            if (mCheckedData.length == 0) {
                NewMessageBox('Please mark at least 1 check box.');
                return;
            }

            pMsg = 'Are you sure you want to remove the GPM from the ' + mCheckedData.length + ' marked items?';
            
            NewMessageBox(pMsg, 'Remove Category GPM', [
                {
                    Text: 'Yes',
                    Click: function() {
                        mModel.DeleteCategoryGpm({
                            Data: mCheckedData,
                            Success: function() {
                                NewMessageBox('Category GPM\'s were successfully deleted.');
                                LoadData();
                            }
                        });
                    }
                },
                {
                    Text: 'No'
                }
            ]);
        } // END function PromptDeleteCheckedItems

        // popup

        var mPopCategory = mElement.find('[name=popCategory]');
        var mButCategory = mPopCategory.find('[name=butCategory]');
        var mTxtCategoryGpm = mPopCategory.find('[name=txtCategoryGpm]');

        function CategoryPopup(vCategory, vGpm) {
            mButCategory.text(vCategory);
            mTxtCategoryGpm.val(CStr2(vGpm));
            mPopCategory.modal('show');
        }

        mPopCategory.find('[name=butOkCategoryGpm]').click(function() {
            var pGpm = mTxtCategoryGpm.val();
            if (!IsNumeric(pGpm)) {
                NewMessageBox('GPM must be numeric.');
                return;
            }

            pGpm = CDec2(pGpm);
            mModel.SaveCategoryGpm({
                Category: mButCategory.text(),
                Gpm: pGpm,
                Success: function () {
                    mPopCategory.modal('hide');
                    NewMessageBox('Category GPM was successfully saved.');
                    LoadData();
                },
                Fail: function(vData) {
                    NewMessageBox(vData);
                }
            });
        });

        LoadData();

        return mOut;

    }); // END CreateStringObject('App.controller.PriceChange', null, function() {

    return mDeferred.promise();

});