require(['common'
    ], function() {

    $(function () {

        txtStockNumbers = $('#txtStockNumbers');

        $('#butTag').click(function(e) {

            if (txtStockNumbers.val().trim().length == 0) {
                MessageBox('Please enter at least 1 stock number');
                return;
            };

            AjaxSend({
                url: '/TagSku/TagData',
                data: {StockNumbers: txtStockNumbers.val().trim()},
                success: function(vData) {
                    MessageBox('Tagged stocks: ' + vData.UpdatedCount);
                },
                error: function(vError) {
                    MessageBox(vError);
                }
            });

        });
    });
});