require([
        'common'
    ], function () {
        $(function () {

            GetDefault();

            $('#btnSUN').click(function () {
                Transfers.WriteFile('IsSendSmS.flg', null, '')
            });

            $('#btnNONE').click(function () {
                Transfers.DeleteFile('IsSendSmS.flg');
            });
        });

        function GetDefault() {

            var IsSendSmS = Transfers.ReadFile('IsSendSmS.flg');

            if (IsSendSmS == undefined) {
                $('#btnNONE').attr("checked", true).checkboxradio("refresh");
            } else {
                $('#btnSUN').attr("checked", true).checkboxradio("refresh");
            }
        }
    });

