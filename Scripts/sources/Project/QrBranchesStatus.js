$(function () {

    var divQrBranchesStatus = $('#divQrBranchesStatus');
    var butRefresh = divQrBranchesStatus.find('[name=butRefresh]')
        .click(butRefresh_Click);
    var divData = divQrBranchesStatus.find('[name=divData]');
    var chkShowBlank = $('#chkShowBlank').change(chkShowBlank_Change);
    var mRows; // orignal rows of datagrid

    butRefresh_Click();

    function butRefresh_Click() {

        divQrBranchesStatus.block();
        AjaxSend({
            url: '/Reports/QrBranchesStatusData',
            success: function (vData) {
                divQrBranchesStatus.unblock();
                if (!HandleSessionExpired(vData)) return;
                mRows = vData.d.Rows.slice(0);
                chkShowBlank_Change();
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                divQrBranchesStatus.unblock();
                ShowMessage(XMLHttpRequest.responseText);
            }
        });

    }; // END butRefresh_Click

    function LoadGrid(vData) {

        vData.Columns = {
            BranchName: {
                Label: 'Branch'
            },
            LastSoNum: {
                Label: 'Last Sales Order Number'
            },
            LastSoDate: {
                Label: 'Last Sales Order Date',
                MajorDataType: 'Date'
            },
            LastImportDate: {
                Label: 'Date of last imported POS',
                MajorDataType: 'Date'
            }
        };
        divData.ProWebGrid(vData);

    }; // END LoadGrid  

    function chkShowBlank_Change() {

        var p;
        var pData;

        pData = {};
        pData.Rows = mRows.slice(0); // http://www.xenoveritas.org/comment/1689
        if (chkShowBlank.prop('checked') == false) {
            p = 0;
            while (p <= pData.Rows.length - 1) {
                if (pData.Rows[p].LastImportDate == '')
                    pData.Rows.splice(p, 1);
                else
                    p++;
            };
        };
        LoadGrid(pData);

    } // END chkShowBlank_Change
});