require(['common'],
function () {
    $(document).ready(function () {

        window.PaymentTerms = {
            Load: function () {
                LoadData();
            }
        }

        LoadData();

        $('body')
        .on('keydown', '#txtCreditTerm', function (e) {
            if (!NumberOnly(e)) {
                return;
            }
        })
        .on('click', '#btnCancelPaymentTerms', function () {
            window.location.hash = 'LibraryPage';
        })
        .on('click', '#btnSavePaymentTerms', function () {
            SavePaymentTerms();
        })

        function SavePaymentTerms() {
            var txtTermID = $.trim($('#txtTermID').val());
            var txtTermName = $.trim($('#txtTermName').val());
            var txtTermDescription = $.trim($('#txtTermDescription').val());
            var txtCreditTerm = $.trim($('#txtCreditTerm').val());
            var selTermActive = $.trim(CInt2($('#selTermActive').is(':checked')));
            var pData = {};

            if(txtTermName == ""){
                WebToast('Please enter term name.', 'top');
                return;
            }

            if (txtCreditTerm == "") {
                WebToast('Please enter credit term.', 'top');
                return;
            }

            pData.ID = txtTermID;
            pData.Name = txtTermName;
            pData.Description = txtTermDescription;
            pData.Days = txtCreditTerm;
            pData.IsActive = selTermActive;

            //$('#btnSavePaymentTerms, #btnCancelPaymentTerms').button('disable');
            
            AjaxSend({
                url: 'PaymentTerms/SavePaymentTerms',
                data: pData,
                success: function (vData) {
                    WebToast('Sucessfully Saved', 'top');
                    //$('#btnSavePaymentTerms, #btnCancelPaymentTerms').button('enable');
                    if(vData.Results == 'saved'){
                        window.location.hash = 'LibraryPage';
                        LibraryTable.PaymentTermsList();
                    } else {
                        NewMessageBox(vData.Results, 'Message');
                    }
                }
            })
        }

        function LoadData() {
            ClearFields()

            if (sessionStorage.ViewID != undefined) {
                GetPaymentTerm();
            }
        }

        function GetPaymentTerm() {
            AjaxSend({
                url: 'PaymentTerms/GetPaymentTerm',
                data: {ID : sessionStorage.ViewID },
                success: function (vData) {
                    $('#txtTermID').val(vData.Results.Rows[0].ID);
                    $('#txtTermName').val(vData.Results.Rows[0].Name).focus();
                    $('#txtTermDescription').val(vData.Results.Rows[0].Description);
                    $('#txtCreditTerm').val(vData.Results.Rows[0].Days);
                    $('#selTermActive').prop('checked', CBool(vData.Results.Rows[0].IsActive));//.slider('refresh');
                }
            })
        }

        function ClearFields() {
            $('#txtTermID').val('');
            $('#txtTermName').val('').focus();
            $('#txtTermDescription').val('');
            $('#txtCreditTerm').val('');
            $('#selTermActive').prop('checked', true);//.slider('refresh');
        }
    })
})