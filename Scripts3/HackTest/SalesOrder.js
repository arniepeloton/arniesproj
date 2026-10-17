$(function() {

    $('#butCustomer').click(function() {

        Browser.LibraryCustomers.Click = function(e) {

            // user clicked an item
            $('#butCustomer').text(e.CompanyName)
                .button('refresh');

        };
        Browser.LibraryCustomers.Show();

    });







    $('#formSearch').submit(function(e) {
        e.preventDefault(); // prevent form from submitting

        Browser.LibraryCustomers.Click = function(e) {

            // user clicked an item
            $('#txtSearch').val(e.CompanyName)

        };
        Browser.LibraryCustomers.Show($('#txtSearch').val()); // prevent form from submitting

        return false;
    });



    $('#pageDialog').bind('pagebeforeshow', function() {
        $('#pageDialog').find('[data-role=content]').append($('#divContent'));
    }).bind('pagebeforehide', function() {
        $('#divContentParent').append($('#divContent'));
    });

    // testing only
    Browser.LibraryCustomers.AfterResult = function(vResult) {
        
        Browser.LibraryCustomers.GetPage().find('[name=ulBrowser] li ap:nth-child(1)').each(
            function(vIndex, vItem) {
                console.log($(vItem).data('data').ID);
            }
        );

    };

});