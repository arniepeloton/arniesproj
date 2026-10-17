$(function () {

    var divPerfectOrdersReport = $('#divPerfectOrdersReport');
    var butRefresh = divPerfectOrdersReport.find('[name=butRefresh]')
        .click(butRefresh_Click);
    var divData = divPerfectOrdersReport.find('[name=divData]')

    butRefresh_Click();

    function butRefresh_Click() {

        divPerfectOrdersReport.block();
        AjaxSend({
            url: '/Reports/PerfectOrdersGetData',
            success: function (vData) {
                divPerfectOrdersReport.unblock();
                LoadGrid(vData.d);
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                divPerfectOrdersReport.unblock();
                ShowMessage(XMLHttpRequest.responseText);
            }
        });

    }; // END butRefresh_Click

    function LoadGrid(vData) {

        vData.Columns = {
            OrderNo: {
                Label: 'Order #'
            },
            OrderDate: {
                Label: 'Order Data',
                MajorDataType: 'Date',
                Format: 'M/d/yyyy'
            },
            InvoiceNum: {
                Label: 'Invoice #'
            },
            InvoiceDate: {
                Label: 'Invoice Date',
                MajorDataType: 'Date',
                Format: 'M/d/yyyy'
            },
            PerfectlySorted: {
                Label: 'Perfectly Sorted',
                MajorDataType: 'Numeric',
                Control: 'Checkbox',
                Alignment: 'Center'
            },
            LineCount: {
                Label: 'Ordered line count',
                MajorDataType: 'Numeric',
                Alignment: 'Right'
            },
            SortedLineCount: {
                Label: 'Sorted line count',
                MajorDataType: 'Numeric',
                Alignment: 'Right'
            }
        };
        divData.ProWebGrid(vData);

    }; // END LoadGrid
});
