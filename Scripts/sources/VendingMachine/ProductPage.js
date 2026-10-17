require(['common'], function () {
    $(document).ready(function () {
        $('#btnviewcart').click(function () {
            $('#divcart').pageshow();
        })

        $('#btnaddnewitem').click(function () {
            $('#divproducts').pageshow();
        })

        $('.btnproduct').click(function () {
            $('#divproductinfo').pageshow();
        })

        $('#btnPIBack').click(function () {
            $('#divproducts').pageshow();
        })

        $('#btnPIAddToCart').click(function () {
            $('#divcart').pageshow();
        })
    })
})