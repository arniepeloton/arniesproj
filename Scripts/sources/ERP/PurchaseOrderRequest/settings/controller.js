define([
    '/Scripts/sources/ERP/Invoicing/settings/model.js',
    'promise!/Scripts/sources/ERP/Invoicing/settings/popDocumentControl/controller.js',
    'promise!/Scripts/sources/ERP/Invoicing/settings/CategoryGpm/controller.js'
], function () {

    var mDeferred = $.Deferred();
    var mHtml;

    (function staticInit() {

        AjaxSend({
            ExactUrl: '/Scripts/sources/ERP/Invoicing/settings/view.html',
            type: 'GET',
            dataType: 'text',
            success: function (vData) {
                mHtml = vData;
                ERP.Invoicing.settings.controller = ERP.Invoicing.settings.controller(); // singleton
                mDeferred.resolve();
            }
        });

    })();

    CreateStringObject('ERP.Invoicing.settings.controller', null, function () {

        var mOut = {};
        var mElement;

        mElement = $(mHtml);
        mOut.element = mElement;
        mOut.Model = new ERP.Invoicing.settings.model();

        var pPopDocumentControl = new ERP.Invoicing.settings.popDocumentControl.controller();
        var mChkAllowEncodingOfOrdersInInvoice = mElement.find('[name=AllowEncodingOfOrdersInInvoice]');
        var mChkAllowEncodingOfQuantitiesInInvoice = mElement.find('[name=AllowEncodingOfQuantitiesInInvoice]');
        var mTxtFreeTypeAPriceMinimumGpm = mElement.find('[name=txtFreeTypeAPriceMinimumGpm]');

        mElement.find('[name=butSave]').click(function() {
            if (!IsNumeric(mTxtFreeTypeAPriceMinimumGpm.val())
                || CDec2(mTxtFreeTypeAPriceMinimumGpm.val()) < 0) {
                NewMessageBox('Free Type A Price Minimum GPM must be a number equal to or greater than zero.');
                return;
            }

            mOut.Model.Save({
                AllowEncodingOfOrdersInInvoice: mChkAllowEncodingOfOrdersInInvoice.prop('checked'),
                AllowEncodingOfQuantitiesInInvoice: mChkAllowEncodingOfQuantitiesInInvoice.prop('checked'),
                FreeTypeAPriceMinimumGpm: mTxtFreeTypeAPriceMinimumGpm.val(),
                Success: function() {
                    NewMessageBox('Settings were successfully saved.');
                }
            });
        });

        mOut.Initialize = function() {
            $('#tabInvoicingSettings').html('').append(mElement);
            //mElement.trigger("create");
            mElement.find('#divUnlimitedCredit').trigger("create");
            mOut.Load();
            mOut.LoadDocumentControl();
            pPopDocumentControl.AfterSave = mOut.LoadDocumentControl;
        };

        mOut.Load = function() {
            mOut.Model.Load({
                callback: function(vData) {
                    mChkAllowEncodingOfOrdersInInvoice.prop('checked', vData.AllowEncodingOfOrdersInInvoice);
                    mChkAllowEncodingOfQuantitiesInInvoice.prop('checked', vData.AllowEncodingOfQuantitiesInInvoice);
                    mTxtFreeTypeAPriceMinimumGpm.val(vData.FreeTypeAPriceMinimumGpm);
                }
            });
        };

        var mDocumentControlTable = mElement.find('[name=tableDocumentControl]');
        var mDocumentControlRow = mDocumentControlTable.find('tbody tr');
        mDocumentControlRow.remove();
        mOut.LoadDocumentControl = function() {
            mOut.Model.LoadDocumentControl({
                callback: function(vData) {
                    DisplayDocumentControl(vData);
                }
            });
        };

        function DisplayDocumentControl(vData) {
            mDocumentControlTable.find('tbody tr').remove();
            for (var p = 0; p <= vData.Rows.length - 1; p++) {
                var pDR = vData.Rows[p];
                var pTR = mDocumentControlRow.clone();
                pTR.data('data', pDR);
                pTR.find('[name=DocumentName]').text(pDR.DocumentName);
                pTR.find('[name=ModuleSource]').text(pDR.ModuleSource);
                pTR.find('[name=PrintLimit]').text(PrintLimitText(pDR.PrintLimit));
                pTR.find('[name=CopiesPerPrint]').text(pDR.CopiesPerPrint);
                pTR.find('[name=LabelOfCopies]').text(pDR.LabelOfCopies);
                pTR.find('[name=Active]').text(CBool(pDR.IsActive) ? 'Yes' : 'No');
                pTR.click(function(e) {
                    var pDR = $(this).data('data');
                    DocumentControlItem_Click(pDR);
                });
                mDocumentControlTable.append(pTR);
            }
        }

        function PrintLimitText(vValue) {
            if (vValue == -1)
                return 'Print Many Times';
            else if (vValue == 1)
                return 'Print Once';
            else
                return vValue;
        }

        function DocumentControlItem_Click(vDR) {
            vDR = $.extend({}, vDR);
            NewMessageBox('', vDR.ProductName, [
                //{
                //    Text: 'Add Document',
                //    Click: function() {
                //        AddProduct();
                //    }
                //},
                {
                    Text: 'Edit Document',
                    Click: function() {
                        pPopDocumentControl.popup(vDR);
                    }
                },
                //{
                //    Text: 'Remove Document',
                //    Click: function() {
                //        RemoveFromCartPrompt(vProductID, vDR);
                //    }
                //}
            ]);
            
        }
        
        mElement.find('[name=butAddDocumentType]').click(function(e) {
            e.preventDefault();
            pPopDocumentControl.popup({});
            return false;
        });

        mElement.find('[name=divCategoryGpmPlaceholder]').append(ERP.Invoicing.settings.CategoryGpm.controller.element);

        mElement.find('#selUnlimitedCredit').on('change', function(e) {
            console.log('selUnlimitedCredit_change');
            NewPromptUserAccess('Set all customers to have unlimited credit limit', {
                ForcePrompt: true,
                OnClickOKSuccess: function () {
                    mOut.Model.SetAllCustomerUnlimitedCredit({
                        value: mElement.find('#selUnlimitedCredit').val(),
                        callback: function(vData) {
                            if (vData.Error)
                                NewMessageBox(vData.Error);
                            else
                                NewMessageBox('Settings has been successfully saved.');
                        }
                    });
                }
            });
        });

        mOut.Initialize();

        return mOut;
    }); // END CreateStringObject('App.controller.PriceChange', null, function() {

    return mDeferred.promise();

});