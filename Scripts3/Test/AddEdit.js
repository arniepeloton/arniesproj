$(function() {

    $('#butBrowse').click(function() {

            Browser.LibraryCustomers.Click = function(e) {

                // user clicked an item
    //            $('#butCustomer').text(e.CompanyName)
    //                .button('refresh');
        
            };
            Browser.LibraryCustomers.Show();

    });

    Browser.LibraryCustomers.ClickEdit = function(e) {

        $.mobile.changePage('#pageCustomer');

    };

    // from Customer page
    $('#butBackToBrowse').click(function() {

        $.mobile.changePage(Browser.LibraryCustomers.Page);
        // history.back();  // goes to main page

    });

    $('#butBackToPage').click(function() {

        $.mobile.changePage($('[data-role=page]:eq(0)'));

    });
    
});