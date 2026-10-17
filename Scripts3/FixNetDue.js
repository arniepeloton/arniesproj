require(['common'
    ], function() {

    $(function () {

        $('#formMain').submit(function() {

            var pInvoiceNums = $('#txtInvoiceNums').val();
            if (CStr2(pInvoiceNums).trim().length == 0) {
                MessageBox('Please enter an invoice number');
                return false;
            };

            AjaxSend({
                url: 'FixNetDue/Verify',
                data: {InvoiceNums: pInvoiceNums,
                    Update: $('#chkUpdate').prop('checked') ? 1 : 0
                },
                success: function (vData) {
                    if (vData.Message != undefined) {
                        var pStrs = vData.Message.split(/\r\n/g);
                        var pDisplay = '';
                        for (var p = 0; p <= pStrs.length - 1; p++) {
                            pDisplay += HtmlEncode(pStrs[p]) + '<br />';
                        };
                        $('#divResult').html(pDisplay);
                    } else if (vData.Error) {
                        $('#divResult').text(vData.Error);
                    };
                },
                error: function (vError) {
                    alert('Network error');                    
                }
            });

            return false;

        }); // $('#butOk').click(function() {

    });
});