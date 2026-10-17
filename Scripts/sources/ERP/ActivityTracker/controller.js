define([
    '/Scripts/sources/ERP/ActivityTracker/model.js?v=' + assetversion
], function () {

    var mDeferred = $.Deferred();
    var mHtml;

    (function staticInit() {

        AjaxSend({
            ExactUrl: '/Scripts/sources/ERP/ActivityTracker/view.html?v=' + assetversion,
            type: 'GET',
            dataType: 'text',
            success: function (vData) {
                mHtml = vData;
                ERP.ActivityTracker.controller = ERP.ActivityTracker.controller(); // singleton
                // mHtml.trigger('create');
                mDeferred.resolve();
            }
        });

    })();

    CreateStringObject('ERP.ActivityTracker.controller', null, function () {

        var mOut = {};
        var mElement;
        var mHasLoaded;

        mElement = $('<div>').append(mHtml);

        if (proj.IsBootstrapDesign()) {
            mElement.find('[data-role="content"] > *').appendTo(mElement.find('.modal-body'));
            mElement.find('[data-role="popup"]').remove();
            mElement = mElement.find('.modal');
        } else {
            mElement.find('.modal').remove();
            mElement = mElement.find('[data-role="popup"]');
        }

        mElement.attr('id', 'popActivityTracker' + NewID());
        
        $('body').append(mElement);
        mOut.Model = new ERP.ActivityTracker.model();

        mOut.element = mElement;
        
        var mTable = mElement.find('[name=table]');
        var mTR = mTable.find('tbody tr:first');

        //mOut.element.trigger('create');        

        mOut.popup = function(vParams) {

            if (!mHasLoaded) {
                if (proj.IsBootstrapDesign()) {
                    Bs.Loading('show');
                } else {
                    NewMessageBox('Loading. Please wait');
                }                
            }

            Clear();
            try {
                if (proj.IsBootstrapDesign()) {
                    mElement.modal('show');
                } else {
                    mElement.show().popup('open');
                }
            } catch (ex) {
                // just ignore the error
            }
            mOut.Model.Data.ID = vParams.ID;
            mOut.Model.Data.TransType = vParams.TransType;

            if (vParams.ID != null) {
                mOut.Model.GetData({
                    Success: function(vData) {
                        //vParams.Success();
                        DisplayData(vData);
                        try {
                            if (proj.IsBootstrapDesign()) {
                                mElement.modal('show');
                            } else {
                                mElement.show().popup('open');
                            }
                        } catch (ex) {
                            // just ignore the error
                        }
                    },
                    Fail: function(vMessage) {
                        NewMessageBox(vMessage);
                    }
                });
            }

            if (!mHasLoaded) {
                mHasLoaded = true;
                if (proj.IsBootstrapDesign()) {
                    Bs.Loading('hide');
                } else {
                    NewMessageBoxClose();
                }                
            }
        };        
        
        function Clear() {
            mTable.find('tbody tr').remove();
        }

        function DisplayData(vData) {
            var pTRs = [];

            if (vData.Rows.length == 0) {
                try {
                    if (proj.IsBootstrapDesign()) {
                        mElement.modal('hide');
                    } else {
                        mElement.show().popup('close');
                    }
                } catch(e) {
                    // just ignore this error
                }
                NewMessageBox('This transaction does not exist.');
            }

            mTable.find('tbody').empty();
            for (var p = 0; p <= vData.Rows.length - 1; p++) {
                var pDR = vData.Rows[p];
                AddRow(pDR);
            }
            
            function AddRow(vDR) {
                var pTR = mTR.clone();
                pTR.data('data', vDR);
                
                pTR.find('[name=activity]').text(vDR.Activity);
                pTR.find('[name=fowst]').text(vDR.FowstName);
                pTR.find('[name=ref]').text(vDR.RefNum);
                pTR.find('[name=date]').text(vDR.LastUpdated);
                pTR.find('[name=timeElapsed]').text(vDR.TimeElapsed);
                pTR.find('[name=lastModifiedBy]').text(vDR.LastModifiedByName);
                (function(vDR) {
                    pTR.click(function(e) {
                        e.preventDefault();
                        var mod = '';
                        if (vDR.Activity == 'Invoice')
                            mod = '&ShowInvoice=1'
                        else if (vDR.Activity == 'Quotation')
                            mod = '&ShowQuotation=1'
                        else if (vDR.Activity == 'Stock Receiving')
                            mod = '&ShowStockReceiving=1'
                        else if (vDR.Activity == 'Sales Order')
                            mod = '&ShowSalesOrder=1'

                        var win = window.open(location.origin + '/erp' + location.search + '&IsReadOnly=1&ID=' + vDR.ID + mod,
                            '_blank',
                            'width=' + screen.availWidth + ',height=' + screen.availWidth);
                        return
                        win.onload = function () {
                            function LoadRecord() {
                                if (win.Z9Q3C9ASDA) {
                                    var pFunc = win.Z9Q3C9ASDA.ShowSalesOrder;
                                    if (vDR.Activity == 'Invoice')
                                        pFunc = win.Z9Q3C9ASDA.ShowInvoice
                                    else if (vDR.Activity == 'Quotation')
                                        pFunc = win.Z9Q3C9ASDA.ShowQuotation
                                    else if (vDR.Activity == 'Stock Receiving')
                                        pFunc = win.Z9Q3C9ASDA.ShowStockReceiving
                                    pFunc({
                                        ID: vDR.ID,
                                        ReadOnly: true
                                    });
                                } else
                                    setTimeout(LoadRecord, 1000);
                            }
                            LoadRecord();    
                        }
                        
                        return false; // para DILI magclose ang popup
                    });
                })(vDR);        
                mTable.find('tbody').append(pTR);
            }
        } // END function DisplayData(vData) {

        $('#txtActivityTrackerSearch').on('keypress', function (e) {
            if (e.keyCode == 13) {
                Search();
            }
        });

        function Search() {
            var pKey = $('#txtActivityTrackerSearch').val();
            pKey = pKey.trim();
            if (pKey == '') {
                NewMessageBox('Please enter a search keyword', 'No search keyword');
                return;
            }
            mOut.Model.GetData({
                Serial: pKey,
                Success: function(vData) {
                    //vParams.Success();
                    DisplayData(vData);
                    try {
                        if (proj.IsBootstrapDesign()) {
                            mElement.modal('show');
                        } else {
                            mElement.show().popup('open');
                        }
                    } catch (ex) {
                        // just ignore the error
                    }
                },
                Fail: function(vMessage) {
                    NewMessageBox(vMessage);
                }
            });
        }

        $('#butActivityTrackerSearch').click(function() {
            Search();
        });

        mOut.Init = function() {
        };

        mOut.Init();

        return mOut;

    }); // END CreateStringObject('App.controller.PriceChange', null, function() {

    return mDeferred.promise();

});