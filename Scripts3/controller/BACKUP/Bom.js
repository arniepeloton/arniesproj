define(['/Scripts3/model/Bom.js?crc=',
    '/Scripts3/controller/BomContent.js?crc=',
    '/Scripts3/Browser/LibraryPIMProduct.js?crc='
], function () {

    var mDeferred = $.Deferred();
    var mHtml;

    (function staticInit() {

        AjaxSend({
            ExactUrl: '/Scripts3/view/Bom.html',
            type: 'GET',
            dataType: 'text',
            success: function (vData) {
                mHtml = vData;
                mDeferred.resolve();
            }
        });

    })();

    $(function () {

        CreateStringObject('App.controller.Bom', null, function () {

            var mOut = {};
            var mElement;
            var mData;
            var mComboInited = false;
            var mBomContent = new App.controller.BomContent();
            var mSpanProduct;
            var mTxtInstruction;

            mBomContent.parent = mOut;

            mOut.bomContent = mBomContent;
            mOut.model = new App.model.Bom();
            mOut.model.parent = this;

            mElement = $(mHtml);
            mOut.element = mElement;
            var mDivList = mOut.element.find('[name=divList]');
            var mLV;
            mOut.element.find('[name=butAddContents]').click(butAddContents_Click);
            mTxtInstruction = mOut.element.find('[name=txtInstruction]');
            mOut.element.find('[name=butSaveInstruction]').click(butSaveInstruction_Click);
            mSpanProduct = mOut.element.find('[name=spanProduct]');

            (function InitListView() {

                mLV = new CreateListView();
                mLV.Element.find('ul')
                    .attr('data-split-icon', 'minus');
                mLV.Element.appendTo(mDivList);
                mLV.OnItem = function (vDataRow, vItem, e) {
                    var pLnk = $('<a>')
                        .attr('href', '../Stock?id=' + vDataRow.ChildProductID)
                    .data('data', vDataRow)
                    .append($('<img>')
                            .attr('src', '../Content/images/box.png')
                    ).append($('<h3>')
                        .css('white-space', 'normal')
                        .text(vDataRow.ProductNo + ' ' + vDataRow.ProductName)
                    ).append($('<div>')
                        .text($.format.number(vDataRow.Quantity, proj.QuantityFormat) + ' ' + vDataRow.UnitMeasure)
                    ).click(function (e) {
                        e.preventDefault();
                        mBomContent.model.organizationID = mOut.model.organizationID;
                        mBomContent.model.parentProductID = mOut.model.productID;
                        mBomContent.model.childProductID = $(this).data('data').ChildProductID;
                        mBomContent.show();
                        return false;
                    });
                    vItem.append(pLnk);

                    pLnk = $('<a>')
                        .attr('href', '#')
                        .data('data', vDataRow)
                        .click(function(e) {
                            var pDR = $(this).data('data');
                            MessageBox('Are you sure you want to remove ' + pDR.ProductName + ' as a content of ' + mOut.model.productDescription + '?',
                                'Remove',
                                [
                                    { 
                                        Text: 'Yes',
                                        Click: function () {
                                            mOut.model.deleteContent({
                                                childProductID: pDR.ChildProductID,
                                                success: function() {
                                                    history.back();
                                                    mOut.getData({
                                                        success: function(vData) {
                                                            MessageBox('Content was successfully deleted.');
                                                        },
                                                        error: function(vMessage) {
                                                        }
                                                    });
                                                },
                                                error: function(vMessage) {
                                                    MessageBox(vMessage);
                                                }
                                            });
                                        }
                                    },
                                    { 
                                        Text: 'No',
                                        Click: function () {
                                            history.back();
                                        }
                                    }
                                ]);
                            return false;
                        });
                    vItem.append(pLnk);
                }; // mLV.OnItem

            })(); // END InitListView

            function butAddContents_Click(e) {

                Browser.LibraryPIMProduct.InputData.IsSearchMore = 0;
                Browser.LibraryPIMProduct.Click = PimBrowser_AfterSelect;
                Browser.LibraryPIMProduct.PageShow = function() {
                    Browser.LibraryPIMProduct.Page.find('.divBrowserContentTop').text('Please choose a content for ' + mOut.model.productDescription);
                };
                Browser.LibraryPIMProduct.Show();

            };

            function butSaveInstruction_Click() {

                mOut.model.saveInstruction({
                    bomInstruction: mTxtInstruction.val(),
                    success: function (vData) {
                        MessageBox('BOM instruction was successfully saved.');
                    },
                    error: function (vMessage) {
                        MessageBox(vMessage);
                    }
                });
            }

            function PimBrowser_AfterSelect(e, vOpt) {
                vOpt.DontClose = true;

                mBomContent.model.organizationID = mOut.model.organizationID;
                mBomContent.model.parentProductID = mOut.model.productID;
                mBomContent.model.childProductID = e.ID;
                mBomContent.show();

            };

            mOut.getData = function (vParams) {
                /*
                vParams = {
                success: function(vData) {
                },
                error: function(vMessage) {
                }
                };
                */

                mLV.ClearResults();
                mOut.model.getData({
                    success: function (vData) {
                        mSpanProduct.text(vData.parent.ProductNo + ' ' + vData.parent.ProductName);
                        mTxtInstruction.val(vData.parent.BomInstruction);
                        mLV.SetData(vData.contents.Rows);
                        vParams.success(vData);
                    },
                    error: function (vMessage) {
                        MessageBox(vMessage);
                        vParams.error(vMessage);
                    }
                });

            };

            return mOut;

        }); // END CreateStringObject('App.controller.PriceChange', null, function() {

    }); // END $(function () {

    return mDeferred.promise();

});