require([
    'common',
], function () {

    var ID = '';


    $(function () {

        window.LoyaltyProgram = {
            GetSystemBehavior: function () {
                GetSystemBehavior();
            }
        }

        GetSystemBehavior();

        $('#btnLoyaltyProgramSettings').click(function () {
            SaveLoyaltyProgramSetting();
        });

        $('#btnDone').click(function () {
            history.back();
        });
    });


    function SaveLoyaltyProgramSetting() {
        var data = {
            ID: ID,
            PointToCurrency: $('#txtPointEquivalence').val(),
            Cash: $('#txtCash').val(),
            CashPoint: $('#txtCashPoint').val(),
            Cheque: $('#txtCheque').val(),
            ChequePoint: $('#txtChequePoint').val(),
            DebitCard: $('#txtDebitCard').val(),
            DebitCardPoint: $('#txtDebitCardPoint').val(),
            CreditCard: $('#txtCreditCard').val(),
            CreditCardPoint: $('#txtCreditCardPoint').val(),
            SRV: $('#txtSRV').val(),
            SRVPoint: $('#txtSRVPoint').val(),
            GiftCertificate: $('#txtGiftCerificate').val(),
            GiftCertificatePoint: $('#txtGiftCerificatePoint').val()
        };

        AjaxSend({
            url: 'SystemBehaviorSettings/SaveLoyaltyProgram',
            data: data,
            success: function (vData) {
                if (vData.Result == "Success") {
                    history.back();
                }
            }
        });

    }

    function GetSystemBehavior() {
        AjaxSend({
            url: 'SystemBehaviorSettings/GetLoyaltyPointSetting',
            success: function (vData) {
                if (vData.Result.Rows.length > 0) {
                    ID = vData.Result.Rows[0].ID;
                    $('#txtPointEquivalence').val(CDec2(vData.Result.Rows[0].PointToCurrency));
                    $('#txtCash').val(CDec2(vData.Result.Rows[0].Cash));
                    $('#txtCashPoint').val(CDec2(vData.Result.Rows[0].CashPoint));
                    $('#txtCheque').val(CDec2(vData.Result.Rows[0].Cheque));
                    $('#txtChequePoint').val(CDec2(vData.Result.Rows[0].ChequePoint));
                    $('#txtDebitCard').val(CDec2(vData.Result.Rows[0].DebitCard));
                    $('#txtDebitCardPoint').val(CDec2(vData.Result.Rows[0].DebitCardPoint));
                    $('#txtCreditCard').val(CDec2(vData.Result.Rows[0].CreditCard));
                    $('#txtCreditCardPoint').val(CDec2(vData.Result.Rows[0].CreditCardPoint));
                    $('#txtSRV').val(CDec2(vData.Result.Rows[0].SRV));
                    $('#txtSRVPoint').val(CDec2(vData.Result.Rows[0].SRVPoint));
                    $('#txtGiftCerificate').val(CDec2(vData.Result.Rows[0].GiftCertificate));
                    $('#txtGiftCerificatePoint').val(CDec2(vData.Result.Rows[0].GiftCertificatePoint));
                }
            }
        });
    }
});