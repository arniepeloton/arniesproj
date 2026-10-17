define([], function() {
    window.Business.LibraryOrganizationCustomerFowstBinProduct = new function(vParent) {
        var $this = this;
            
        $this.GetCapsByFowstID = function(vFowstID, vProductID) {
            // PARAMS:
            //  vProductID: optional
            var pDT;
            var pSql;

            pSql = 'SELECT LibraryOrganizationCustomerFowstBinProduct.ProductID' + 
                ', SUM(LibraryOrganizationCustomerFowstBinProduct.MinPerProduct) AS MinPerProduct' +
                ', SUM(LibraryOrganizationCustomerFowstBinProduct.MaxPerProduct) AS MaxPerProduct' +
                ' FROM private.LibraryOrganizationCustomerFowstBinProduct' +
                ' WHERE LibraryOrganizationCustomerFowstBinProduct.CustomerFowstID = ' + ReadyString(vFowstID);

            if (vProductID != undefined)
                pSql += ' AND LibraryOrganizationCustomerFowstBinProduct.ProductID = ' + ReadyString(vProductID);

            pSql += ' GROUP BY LibraryOrganizationCustomerFowstBinProduct.ProductID';
            pDT = Sql.Execute(pSql);

            return pDT;

        }; // END $this.GetCapsByFowstID = function(vFowstID) {
        return $this;
    };
});