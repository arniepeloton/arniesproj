define([
    '/Scripts/sources/ERP/Invoicing/settings/popDocumentControl/model.js?v=' + assetversion
], function () {

    var mDeferred = $.Deferred();
    var mHtml;

    (function staticInit() {

        AjaxSend({
            ExactUrl: '/Scripts/sources/ERP/Invoicing/settings/popDocumentControl/view.html?v=' + assetversion,
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

    CreateStringObject('ERP.Invoicing.settings.popDocumentControl.controller', null, function () {

        var mOut = {};
        var mElement;
        var mDivUnit;
        var mTransType;
        var mMaxCopies = 10;
        var popupid = NewID();

        mElement = $('<div>').append($.parseHTML(mHtml));
        mElement.find('[data-role="popup"]:first-child').attr('id', 'popDocumentControl' + popupid);
        $('body').append(mElement);
        mOut.element = mElement;
        mOut.Model = null;
        mOut.AfterSave = function(){};

        mOut.popup = function(vParams) {
            mOut.Model.Data.ID = vParams.ID;
            //mOut.Model.Data.CustomerID = vParams.CustomerID;
            //mTransType = vParams.TransType;
            //mElement.find('[name=title]').text(vParams.ProductName);
            Clear();
            $('#popDocumentControl' + popupid).show().popup().popup('open');
            if (mOut.Model.Data.ID != null)
                mOut.GetData();
        };

        mOut.GetData = function() {
            mOut.Model.GetData({
                Success: function(vData) {
                    DisplayData(vData.Rows[0]);
                },
                Fail: function(vMessage) {
                    NewMessageBox(vMessage);
                }
            });
        }; 

        function Clear() {
            
            //if (mDivUnit == null)
            //    mDivUnit = $(mElement.find('[name=divUnit]:first')).clone();
            //mElement.find('[name=divUnit]').remove();
            //mTable.find('tbody tr').remove();
            mElement.find('[name=fileTemplate]');
        }

        var pTxtDocumentName = mElement.find('[name=txtDocumentName]');
        var pCbModuleSource = mElement.find('[name=cbModuleSource]');
        var pCbPrintLimit = mElement.find('[name=cbPrintLimit]');
        var pCbSeriesType = mElement.find('[name=cbSeriesType]');
        var pTxtSeriesStartingNum = mElement.find('[name=txtSeriesStartingNum]');
        var pTxtPermitNum = mElement.find('[name=txtPermitNum]');
        var pTxtCopiesPerPrint = mElement.find('[name=txtCopiesPerPrint]');
        var pTxtMaxLineItemsPerPage = mElement.find('[name=txtMaxLineItemsPerPage]');
        pTxtCopiesPerPrint.on('textchange', pTxtCopiesPerPrint_Change);

        var pChkActive = mElement.find('[name=chkActive]');
        var pChkAutoPrint = mElement.find('[name=chkAutoPrint]');
        var pChkPreprinted = mElement.find('[name=chkPreprinted]');
        var pTxtFootnote = mElement.find('[name=txtFootnote]');
        var pTxtPrinterName = mElement.find('[name=txtPrinterName]');
        function DisplayData(vData) {
            mElement.find('form').trigger('reset'); // http://stackoverflow.com/a/24505433/334045
            pTxtDocumentName.val(vData.DocumentName);
            pCbModuleSource.find('option:contains("' + vData.ModuleSource + '")').prop('selected', true);
            pCbSeriesType.find('option[value="' + vData.SeriesType + '"]').prop('selected', true);
            pTxtSeriesStartingNum.val(vData.SeriesStartingNum);
            pTxtPermitNum.val(vData.PermitNum);
            pCbPrintLimit.val(vData.PrintLimit);
            pTxtCopiesPerPrint.val(vData.CopiesPerPrint);
            mElement.find('[name=txtLabelOfCopy]');
            pTxtFootnote.val(vData.Footnote);
            pTxtPrinterName.val(vData.PrinterName);
            pTxtMaxLineItemsPerPage.val(vData.MaxLineItemsPerPage);

            if (CBool(vData.IsActive)) {
                pChkActive.prop('checked', vData.IsActive);
            }

            if (CBool(vData.AutoPrint)) {
                pChkAutoPrint.prop('checked', vData.AutoPrint);
            }

            if (CBool(vData.Preprinted)) {
                pChkPreprinted.prop('checked', vData.Preprinted);
            }

            pTxtCopiesPerPrint_Change();

            var pCopies = pTxtCopiesPerPrint.val();
            pCopies = CDec2(pCopies);
            if (pCopies < 1)
                pCopies = 1;
            for (p = 1; p <= pCopies; p++) {
                var pName = $.format.number(p, '00');
                $('[name=txtLabelOfCopy' + pName + ']').val(vData['LabelOfCopy' + pName]);
            }
        } // END function DisplayData(vData) {

        function pTxtCopiesPerPrint_Change() {
            var pCopies;
            var pName;
            var p;
            pCopies = pTxtCopiesPerPrint.val();
            pCopies = CDec2(pCopies);
            if (pCopies < 1) {
                pCopies = 1;
            } else if (pCopies > mMaxCopies) {
                pCopies = mMaxCopies;
                pTxtCopiesPerPrint.val(pCopies);
                return;
            }
            for (p = 1; p <= pCopies; p++) {
                pName = $.format.number(p, '00');
                pName = '[name=txtLabelOfCopy' + pName + ']';
                $(pName).closest('.custom-ui-label').show();
                if ($(pName).val().trim() == '') {
                    if (p == 1)
                        $(pName).val('Original');
                    else if (p == 2)
                        $(pName).val('Duplicate');
                    else if (p == 3)
                        $(pName).val('Triplicate');
                    else
                        $(pName).val(p + 'th Copy');
                }
            }
            for (p = pCopies + 1; p <= mMaxCopies; p++) {
                pName = $.format.number(p, '00');
                pName = '[name=txtLabelOfCopy' + pName + ']';
                $($(pName)).closest('.custom-ui-label').hide();
            }
        }

        //mElement.find('[name=butOk]').click(function() {
        //    var pData = {
        //        ID: mOut.Model.Data.ID,
        //        DocumentName: pTxtDocumentName.val(),
        //        ModuleSource: pCbModuleSource.val(),
        //        PrintLimit: pCbPrintLimit.val(),
        //        CopiesPerPrint: pTxtCopiesPerPrint.val(),
        //        Footnote: pTxtFootnote.val(),
        //        IsActive: pChkActive.prop('checked')
        //    };

            

        //    mOut.Model.Save({
        //        data: pData,
        //        Success: function(vData) {
        //            mOut.AfterSave();
        //        }, 
        //        Fail: function(vMessage) {
        //            NewMessageBox(vMessage);
        //        }
        //    });
        //});

        mElement.find('[name="btnPrinterNameInstructions"]').click(function () {
            NewMessageBoxClose();
            $('#popupInstructionDeterminePrinter').show().popup().popup('open');
            AfterPopupClose(function () {
                $('#popDocumentControl' + popupid).show().popup().popup('open');
            })
        })

        mElement.find('form').submit(function(e) {
            var p;

            e.preventDefault();
            var formData = new FormData($(this)[0]);
            formData.append('OrgID', sessionStorage.OrgID);
            formData.append('ID', mOut.Model.Data.ID);
            formData.append('DocumentName', pTxtDocumentName.val());
            formData.append('ModuleSource', pCbModuleSource.val());
            formData.append('SeriesType', pCbSeriesType.val());
            formData.append('SeriesStartingNum', pTxtSeriesStartingNum.val());
            formData.append('PermitNum', pTxtPermitNum.val());
            formData.append('PrintLimit', pCbPrintLimit.val());
            formData.append('CopiesPerPrint', pTxtCopiesPerPrint.val());
            formData.append('Footnote', pTxtFootnote.val());
            formData.append('PrinterName', pTxtPrinterName.val());
            formData.append('IsActive', pChkActive.prop('checked'));
            formData.append('AutoPrint', pChkAutoPrint.prop('checked'));
            formData.append('Preprinted', pChkPreprinted.prop('checked'));
            formData.append('MaxLineItemsPerPage', pTxtMaxLineItemsPerPage.val());
            var pCopies = pTxtCopiesPerPrint.val();
            pCopies = CDec2(pCopies);
            if (pCopies < 1)
                pCopies = 1;
            for (p = 1; p <= pCopies; p++) {
                var pName = $.format.number(p, '00');
                formData.append('LabelOfCopy' + pName, $('[name=txtLabelOfCopy' + pName + ']').val());
            }

            $.ajax({
                url: '/ERP/SaveDocumentControl',
                type: 'POST',
                data: formData,
                async: false,
                cache: false,
                contentType: false,
                processData: false,
                success: function (vData) {
                    try {
                        vData = JSON.parse(vData);
                        if (vData.d)
                            vData = vData.d;
                    } catch (e) {
                        NewMessageBox('An error occured while parsing the server response to JSON: ' + e.message);
                        return;
                    }
                    if (vData.Error)
                        NewMessageBox(vData.Error);
                    else if (vData.Warning)
                        NewMessageBox(vData.Warning);
                    else if (vData.Result == 'ok')
                        NewMessageBox('The data was successfully saved.');
                    else
                        NewMessageBox('Unknown status. ref jsHCTRGFCAGZ!');
                    mOut.AfterSave();
                }
            });
        });

        mOut.Init = function() {
            var pOption;
            mOut.Model = new ERP.Invoicing.settings.popDocumentControl.model();

            pCbPrintLimit.html('');
            pOption = new Option('Print Once', 1);
            pCbPrintLimit.append(pOption);
            pOption = new Option('Print Many Times', -1);
            pCbPrintLimit.append(pOption);

            pCbModuleSource.html('');
            pOption = new Option('Disbursement > Stock Receiving');
            pCbModuleSource.append(pOption);
            pOption = new Option('Disbursement > PO');
            pCbModuleSource.append(pOption);
            pOption = new Option('Disbursement > Canvassing');
            pCbModuleSource.append(pOption);
            pOption = new Option('Revenue > Invoicing');
            pCbModuleSource.append(pOption);
            pOption = new Option('Revenue > Price Quote');
            pCbModuleSource.append(pOption);
            pOption = new Option('Revenue > Sales Ordering');
            pCbModuleSource.append(pOption);
        };

        mOut.Init();

        return mOut;

    }); // END CreateStringObject('App.controller.PriceChange', null, function() {

    return mDeferred.promise();

});