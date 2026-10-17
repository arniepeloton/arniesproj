define([
    
], function () {

    var mDeferred = $.Deferred();
    var mHtml;

    (function staticInit() {

        AjaxSend({
            ExactUrl: '/Scripts/sources/ERP/FinancialStatement/settings/view.html?v=' + assetversion,
            type: 'GET',
            dataType: 'text',
            success: function (vData) {
                mHtml = vData;
                mDeferred.resolve();
            }
        });
    })();

    CreateStringObject('ERP.FinancialStatement.settings.controller', null, function () {

        var mOut = {};

        mElement = $(mHtml);
        mOut.element = mElement;

        $('#tabFinacialStatementSettings').empty().append(mElement).trigger('create');
        var mDocumentControlTable = mElement.find('[name=tableDocumentControl]');
        var mDocumentControlRow = mDocumentControlTable.find('tbody tr');
        LoadDocumentControl();

        function LoadDocumentControl() {
            AjaxSend({
                url: '/ERP/GetDocumentControl',
                data: {
                    NoPo: true,
                    ModuleSource: 'Revenue > Invoicing'
                },
                success: function(vData) {
                    DisplayDocumentControl(vData);
                }
            });
        }

        function DisplayDocumentControl(vData) {
            mDocumentControlTable.find('tbody tr').remove();
            for (var p = 0; p <= vData.Rows.length - 1; p++) {
                var pDR = vData.Rows[p];
                var pTR = mDocumentControlRow.clone();
                pTR.data('data', pDR);
                pTR.find('[name=DocumentName]').text(pDR.DocumentName);
                pTR.find('[name=ModuleSource]').text(pDR.ModuleSource);
                pTR.find('[name=CopiesPerPrint]').text(pDR.CopiesPerPrint);
                pTR.find('[name=LabelOfCopies]').text(pDR.LabelOfCopies);
                pTR.find('[name=Active]').text(CBool(pDR.IsActive) ? 'Yes' : 'No');
                pTR.find('[name=PrintAutomatically]').text(CBool(pDR.AutoPrint) ? 'Yes' : 'No');
                pTR.click(function(e) {
                    var pDR = $(this).data('data');
                    DocumentControlItem_Click(pDR);
                });
                mDocumentControlTable.append(pTR);
            }
        }

        return mOut;

    }); // END CreateStringObject('App.controller.PriceChange', null, function() {

    return mDeferred.promise();

});