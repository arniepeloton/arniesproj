define([
    //'/Scripts/sources/ERP/OrderProcessing/product/model.js'
], function () {

    var mDeferred = $.Deferred();
    var mHtml;    

    (function staticInit() {

        AjaxSend({
            ExactUrl: '/Scripts/sources/ERP/SplitBill/view.html',
            type: 'GET',
            dataType: 'text',
            success: function (vData) {
                mHtml = vData;
                // ERP.OrderProcessing.product.controller = ERP.OrderProcessing.product.controller(); // singleton
                // mHtml.trigger('create');
                mDeferred.resolve();
            }
        });

    })();

    CreateStringObject('ERP.SplitBill.controller', null, function () {

        var mOut = {};
        var mElement;
        var mDivUnit;
        var mTransType;
        var mProductName;
        //bootstrap
        var bspopup = $('<div class="modal fade" id="popupSplitBill" tabindex="-1" role="dialog">')
                        .append($('<div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable" role="document">')
                            .append($('<div class="modal-content">')
                                .append($('<div class="modal-header">')
                                    .append('<span data-role="none" name="title" style="font-size:18px;"></span>')
                                    .append('<button data-role="none" type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>')
                                )
                                .append($('<div class="modal-body" id="SplitContent">'))
                                .append($('<div class="modal-footer">')
                                    .append($('<div class="row">')
                                        .append($('<div class="col-6">')
                                            .append($('<button data-role="none" class="btn btn-secondary btn-block btn-lg" data-dismiss="modal">').text('Cancel'))
                                        )
                                        .append($('<div class="col-6">')
                                            .append($('<button data-role="none" name="butSave" class="btn btn-success btn-block btn-lg">').text('Save'))
                                        )
                                    )
                                )
                            )
                        )

        //jquery
        var jpopup = $('<div data-role="popup" id="popupSplitBill" data-overlay-theme="a" data-theme="a" data-corners="false" style="display:none">')
                        .append($('<a href="#" data-rel="back" data-role="button" data-theme="a" data-icon="delete" data-iconpos="notext" class="ui-btn-right">').text('Close'))
                        .append($('<center>').append($('<h3 name="title">').text('Split Bill')))
                        .append($('<div data-role="content" class="ui-content ui-body-d">')
                            .append($('<div class="modal-body" id="SplitContent">'))
                            .append('<br /><br />')
                            .append($('<a name="butSave" data-role="none" class="btn custom-btn-theme-b">').text('Save'))
                            .append('<br /><br />')
                            .append($('<a href="#" data-rel="back" data-role="none" class="btn custom-btn-theme-c">').text('Cancel'))
                        )

        if (proj.IsBootstrapDesign()) {            
            bspopup.find('#SplitContent').append(mHtml);
            bspopup.find('[name="tableProduct"]').addClass('table table-striped');
            mHtml = bspopup;
        } else {            
            jpopup.find('#SplitContent').append(mHtml);
            jpopup.find('[name="tableProduct"]').css({'margin-right': '5px', display: 'block'}).addClass('ui-table ui-responsive table-stroke table-stripe custom-tbl-responsive');
            mHtml = jpopup;
        }

        mElement = mHtml//$();
        //mElement.attr('id', 'popOrderInput' + NewID());
        $('body').append(mElement);
        mOut.element = mElement;
        mOut.Model = null;
        mOut.AfterSave = function(){};
        var mTable = mElement.find('[name=tableProduct]');
        var mTR = mTable.find('tbody tr:first');
        var mInvoiceID1, mInvoiceID2;
        var mParentTableID;
        var mAfterSave;

        mOut.popup = function(vParams) {
            mInvoiceID1 = vParams.InvoiceID1;
            mInvoiceID2 = vParams.InvoiceID2;
            mParentTableID = vParams.ParentTableID;
            mAfterSave = vParams.AfterSave;

            //mElement.find('.FreeTypePrice').hide();
            //mOut.Model.Data.ProductID = vParams.ProductID;
            //mOut.Model.Data.CustomerID = vParams.CustomerID;
            //mTransType = vParams.TransType;
            //if (mTransType == 'PurchaseOrder')
            //    mElement.find('.PriceLevel').hide();
            //else
            //    mElement.find('.PriceLevel').show();
                
            //mProductName = vParams.ProductName;
            //mElement.find('[name=title]').text(mProductName);
            //mElement.find('[name=spanProductNumber]').text(vParams.ProductNo);
            Clear();

            $('[name="title"]').text('Split Bill');

            if (proj.IsBootstrapDesign()) {
                mElement.modal('show');
            } else {
                $('#popupTableOptions').popup('close');
                mElement.trigger('create');
                mElement.show().popup().popup('open');
            }
            
            LoadData();
            
            //mOut.Model.GetData({
            //    Success: function(vData) {
            //        vParams.Success();
            //        DisplayData(vData);
            //        HandleFreeTypePriceElements();
            //    },
            //    Fail: function(vMessage) {
            //        NewMessageBox(vMessage);
            //    }
            //});
        };
        
        function Clear() {
            
            mTable.find('tbody tr').remove();

        }

        function LoadData() {
            // Business.TransInvoice.GetCart(mInvoiceID1); // old 2018_04_18__09_06_28_0687
            var pCart = SalesMenu_offline().PosCart({
                OrderID: mInvoiceID1
            });
            var pCart2;
            if (mInvoiceID2 != null)
                pCart2 = SalesMenu_offline().PosCart({
                    OrderID: mInvoiceID2
                });
            //var pMergedCart = MergeCarts(pCart, pCart2); 
            DisplayProducts(pCart, pCart2); 
        }

        //function MergeCarts(vCart1, vCart2) {
        //    for (var p = 0; p <= vCart1.Stocks.Rows.length - 1; p++) {
        //        var pDR = vCart1.Units.Rows[p];
        //        if (CDec2(pDR.OrderedQuantity) == 0)
        //            continue;
        //        var pTR = mTR.clone();
        //        pTR.data('Invoice1', pDR);
        //        pTR.find('[name=Product]').text(pDR.ProductName + GetModifierText(pDR));
        //        pTR.find('[name=txtTable1Qty]').val(pDR.OrderedQuantity);


        //        pTR.find('[name=txtTable2Qty]').val('');
        //        mTable.find('tbody').append(pTR);
        //    }
        //}

        function FindByProductIdAndModifiers(vDR, vRows) {
            var pModifierText = GetModifierText(vDR);
            for (var p = 0; p <= vRows.length - 1; p++) {
                var pDR = vRows[p];
                if (ReadyGuid(pDR.ProductID) == ReadyGuid(vDR.ProductID)) {
                    var pModifierText2 = GetModifierText(pDR);
                    if (pModifierText == pModifierText2)
                        return pDR;
                }
            }
        }

        function DisplayProducts(vCart1, vCart2) {

            for (var p = 0; p <= vCart1.Units.Rows.length - 1; p++) {
                var pDR = vCart1.Units.Rows[p];
                //if (CDec2(pDR.OrderedQuantity) == 0)
                //    continue;
                var pTR = mTR.clone();
                pTR.data('Invoice1', pDR);
                pTR.find('[name=Product]').text(pDR.ProductName + GetModifierText(pDR));
                pTR.find('[name=txtTable1Qty]').val(pDR.OrderedQuantity);

                if (vCart2 != null) {
                    var pDR2 = FindByProductIdAndModifiers(pDR, vCart2.Units.Rows);
                    if (pDR2 != null) {
                        pTR.data('Invoice2', pDR2);
                        pDR2.IsDisplayed = true;
                        if (CDec2(pDR2.OrderedQuantity) > 0)
                            pTR.find('[name=txtTable2Qty]').val(pDR2.OrderedQuantity);
                        else
                            pTR.find('[name=txtTable2Qty]').val('');
                    }
                } else
                    pTR.find('[name=txtTable2Qty]').val('');

                pTR.find('[name=btnTransferToTbl1]').click(function () {
                    TransferQty($(this), true);
                })

                pTR.find('[name=btnTransferToTbl2]').click(function () {
                    TransferQty($(this), false);
                })

                mTable.find('tbody').append(pTR);
            }

            // display items that are unique to the 2nd cart
            if (vCart2 != null) {
                for (var p = 0; p <= vCart2.Units.Rows.length - 1; p++) {
                    var pDR = vCart2.Units.Rows[p];
                    if (pDR.IsDisplayed)
                        continue;
                    if (CDec2(pDR.OrderedQuantity) == 0)
                        continue;
                    var pTR = mTR.clone();
                    pTR.data('Invoice2', pDR);
                    pTR.find('[name=Product]').text(pDR.ProductName + GetModifierText(pDR));
                    pTR.find('[name=txtTable1Qty]').val('');
                    pTR.find('[name=txtTable2Qty]').val(pDR.OrderedQuantity);

                    pTR.find('[name=btnTransferToTbl1]').click(function () {
                        TransferQty($(this), true);
                    })

                    pTR.find('[name=btnTransferToTbl2]').click(function () {
                        TransferQty($(this), false);
                    })
                    mTable.find('tbody').append(pTR);
                }
            } // END if (vCart2 != null) {
        }

        function TransferQty(dis, isMoveToTableQty1) {
            var parent = dis.parents().closest('tr');
            var tblQty1 = parent.find('[name="txtTable1Qty"]');
            var tblQty2 = parent.find('[name="txtTable2Qty"]');
            var tblQty1Val = CDec2(tblQty1.val());
            var tblQty2Val = CDec2(tblQty2.val());

            if (isMoveToTableQty1) {
                if (tblQty2Val != 0) {
                    tblQty1.val(tblQty1Val + 1);
                    tblQty2.val(tblQty2Val - 1);
                }
            } else {
                if (tblQty1Val != 0) {
                    tblQty1.val(tblQty1Val - 1);
                    tblQty2.val(tblQty2Val + 1);
                }
            }
        }

        function GetModifierText(vDR) {
            var pOut = '';
            if (!$.isArray(vDR.ItemModifier))
                return '';
            for (var pItemModifierIndex = 0; pItemModifierIndex <= vDR.ItemModifier.length - 1; pItemModifierIndex++) {
                var pItemModifier = vDR.ItemModifier[pItemModifierIndex];
                if (!$.isArray(pItemModifier.Items))
                    continue;
                if (!$.isArray(pItemModifier.SelectedModifier))
                    continue;
                for (var pSelectedModifierIndex = 0; pSelectedModifierIndex <= pItemModifier.SelectedModifier.length - 1; pSelectedModifierIndex++) {
                    var pSelectedModifier = pItemModifier.SelectedModifier[pSelectedModifierIndex];
                    if (pOut != '') 
                        pOut += '\n';
                    pOut += pSelectedModifier.Description + CStr2(pSelectedModifier.Others);
                }
            }

            if (pOut != '') 
                pOut = '\n' + pOut;
            return pOut + '';
        }

        mElement.find('[name=butSave]').click(function() {
            var pTRs = mTable.find('tbody tr');
            var pUpdatedItems = []; // 1st dimension = item; 2nd dimension (1-based) = table
            for (var p = 0; p <= pTRs.length - 1; p++) {
                var pTR = $(pTRs[p]);
                var pTableIndex = 1;
                var pTotalQty = 0;
                var pOriginalTotal = 0;
                var pDrItem = null;

                do {
                    var pTxt = pTR.find('[name=txtTable' + pTableIndex + 'Qty]');
                    if (pTxt.length == 0)
                        break;
                    var pDR = pTR.data('Invoice' + pTableIndex);
                    if (pDR != null) {
                        pDrItem = pDR;
                        break;                   
                    }
                    pTableIndex++;
                } while (true);
                pTableIndex = 1;
                do {
                    var pTxt = pTR.find('[name=txtTable' + pTableIndex + 'Qty]');
                    if (pTxt.length == 0)
                        break;
                    var pDR = pTR.data('Invoice' + pTableIndex);
                    if (pDR == null)
                        pDR = {
                            Qty: 0,
                            OriginalQty: 0,
                            UnitPrice: pDrItem.UnitPrice,
                            ItemID: pDrItem.ItemID,
                            ProductID: pDrItem.ProductID,
                            ProductName: pDrItem.ProductName
                        };
                    pOriginalTotal += CDec2(pDR.OrderedQuantity);
                    if (pTxt.val().trim() != '') {
                        if (!IsNumeric(pTxt.val())) {
                            NewMessageBox('Qty in Table ' + pTableIndex + ' must be numeric', null, null, {
                                Close: function () {
                                    if (proj.IsBootstrapDesign()) {
                                        setTimeout(function () {
                                            mElement.modal('show');
                                        }, 300)                                        
                                    } else {
                                        mElement.show().popup().popup('open');
                                    }
                                }
                            });
                            return;
                        }
                    } // END if (pTxt.val().trim() != '') {

                    if (pUpdatedItems[p] == null)
                        pUpdatedItems[p] = [];
                    pUpdatedItems[p][pTableIndex] = {
                        ItemID: pDR.ItemID,
                        Qty: CDec2(pTxt.val()),
                        OriginalQty: pDR.OrderedQuantity,
                        UnitPrice: pDR.UnitPrice
                    };

                    pTotalQty += CDec2(pTxt.val());
                    pTableIndex++;
                } while (true);
                if (Math.abs(pTotalQty - pOriginalTotal) > 0.001) {
                    NewMessageBox('The total ordered quantity in splitted bills must be equal to the original ordered quantity in row #' + (p + 1) + '.' + pDR.ProductName + '.', null, null, {
                        Close: function() {
                            mElement.show().popup().popup('open');
                        }
                    });
                    return;
                }
            }

            var pCart = SalesMenu_offline().PosCart({
                OrderID: mInvoiceID1
            });
            pCart.SaveAsNew = true;
            if (mParentTableID) {
                var pCloneResult = CloneTableInfo(mParentTableID);
                pCart.Master.CustomerTableNo = 'TBL_' + pCloneResult.InternalTableName;
            }
            proj.PosMode = true;

            // update table1
            (function() {
                var pUpdateCounts = 0;
                for (var pUpdatedIndex = 0; pUpdatedIndex <= pUpdatedItems.length - 1; pUpdatedIndex++) {
                    var pDrUpdated = pUpdatedItems[pUpdatedIndex][1]; // 2 (1-based) hardcode
                    if (pDrUpdated == null)
                        continue;
                    if (CDec2(pDrUpdated.Qty) == CDec2(pDrUpdated.OriginalQty))
                        continue;
                    var pIU = new InsertUpdate('TransInvoiceItems');
                    pIU.Add('OrderedQuantity', pDrUpdated.Qty);
                    pIU.Add('Quantity', pDrUpdated.Qty);
                    var pSubTotal = pDrUpdated.Qty * pDrUpdated.UnitPrice;
                    pIU.Add('SubTotal', pSubTotal);
                    pIU.Add('VATableSale', pSubTotal);
                    var pSql = pIU.GetUpdate('ID = ' + ReadyString(pDrUpdated.ItemID));
                    Sql.ExecuteSql(pSql);
                    pUpdateCounts++;
                }
                if (pUpdateCounts > 0) {
                    Business.TransInvoice.UpdateTotalAmount(mInvoiceID1);
                    Business.TransInvoice.UpdateItemCount(mInvoiceID1);
                }
            })();

            // update table2
            var pUnitRows = [];
            for (var pUpdatedIndex = 0; pUpdatedIndex <= pUpdatedItems.length - 1; pUpdatedIndex++) {
                var pDrUpdated = pUpdatedItems[pUpdatedIndex][2]; // 2 (1-based) hardcode
                if (pDrUpdated == null)
                    continue;
                if (CDec2(pDrUpdated.Qty) == 0)
                    continue;
                for (var pUnitIndex = 0; pUnitIndex <= pCart.Units.Rows.length - 1; pUnitIndex++) {
                    var pDrUnit = pCart.Units.Rows[pUnitIndex];
                    if (ReadyGuid(pDrUpdated.ItemID) != ReadyGuid(pDrUnit.ItemID))
                        continue;
                    pDrUnit.OrderedQuantity = pDrUpdated.Qty;
                    pDrUnit.Quantity = pDrUpdated.Qty;
                    pDrUnit.SubTotal = pDrUnit.UnitPrice * pDrUnit.OrderedQuantity;
                    pDrUnit.AfterSave = function(vData) {
                        Business.TransInvoice.CopyModifiers(vData.ItemID, vData.ID);
                    };
                    pUnitRows.push(pDrUnit);
                }
            }

            pCart.Units.Rows = pUnitRows;
            SalesMenu_offline().Import(pCart);

            NewMessageBox('Bill was successfully splitted.', null, null, {
                Close: function() {
                    //mElement.show().popup().popup('open');
                }
            });
            if (mAfterSave != null)
            mAfterSave();
        }); // mElement.find('[name=butSave]').click(function() {

        function CloneTableInfo(vTableID) {
            var pSql = 'SELECT * FROM LibraryTableTableInfo\n' +
                ' WHERE ID = ' + ReadyString(vTableID);
            var pDrSource = Sql.ExecuteRow(pSql);
            if (pDrSource == null)
                throw new Error('Cannot find the table to clone for split bill. ref: jsMWT8AUGSJG!');
            var pID = NewID();
            var pIU = new InsertUpdate('LibraryTableTableInfo');
            var pSubIndex = FindAvailableSplitBillTableSubIndex(pDrSource.TableName);
            var pInternalTableName = pDrSource.InternalTableName + '.' + pSubIndex;
            pIU.Add('ID', ReadyString(pID));
            pIU.Add('ParentID', ReadyString(vTableID));
            pIU.Add('InternalTableName', ReadyString(pInternalTableName));
            pIU.Add('UserID', ReadyString(pDrSource.UserID));
            pIU.Add('DiningAreaID', ReadyString(pDrSource.DiningAreaID));
            pIU.Add('TableName', ReadyString(pDrSource.TableName + '.' + pSubIndex));
            pIU.Add('OrderNo', pDrSource.OrderNo);
            pIU.Add('LastUpdated', proj.SqliteNow);
            pSql = pIU.GetInsert();
            Sql.ExecuteSql(pSql);

            return {
                ID: pID,
                SubIndex: pSubIndex,
                InternalTableName: pInternalTableName
            }
        }

        function FindAvailableSplitBillTableSubIndex(vTableName) {
            var pSubIndex = 1;
            do {
                var pSubName = vTableName + '.' + pSubIndex;
                var pSql = 'SELECT COUNT(*)\n' +
                    ' FROM LibraryTableTableInfo\n' +
                    ' WHERE TableName = ' + ReadyString(pSubName);
                var pCount = Sql.ExecuteScalar(pSql);
                if (pCount == 0)
                    return pSubIndex;
                pSubIndex++;
            } while (true)
        }

        return mOut;

    }); // END CreateStringObject('App.controller.PriceChange', null, function() {

    return mDeferred.promise();

});