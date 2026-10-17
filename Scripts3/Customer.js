define(['common'], function() {
    
        var mCustomerID;

        $('#butCompany').click(function() {

            Browser.LibraryCustomers.Click = function(e) {
                $('#pCompanyName').text(e.CompanyName);
                mCustomerID = e.ID;
            };
            Browser.LibraryCustomers.Show();

        });

        $('#butOk').click(function() {

            if (CStr2(mCustomerID).length == 0) {
                MessageBox('Please select a customer first.');
            } else
                if (Customer && Customer.Callback)
                    Customer.Callback({
                        ID: mCustomerID,
                        CompanyName: $('#pCompanyName').text()
                        // todo: billing and delivery address
                    })

        });

});