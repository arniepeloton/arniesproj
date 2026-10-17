define([], function() {
    window.Business.SystemDefaults = new function(vParent) {
        this.GetSystemDefault = function(vDescription, vDefault) {
            
            var pDT;
            var pSql;

            if (vDefault == undefined)
                vDefault = '';
            pSql = 'SELECT DefaultValue' +
                ' FROM private.SystemDefaults' +
                ' WHERE Description = ' + ReadyString(vDescription);
            pDT = Sql.Execute(pSql);
            if (pDT == undefined) return vDefault;
            if (pDT.Rows.length == 0) return vDefault;
            return pDT.Rows[0].DefaultValue;

        };

        this.CustomerOrganizationID = function() {
            return this.GetSystemDefault("Customer Organization ID");
        };

        this.POSDeliveryAddressID = function() {
            return this.GetSystemDefault("POS Delivery Address ID");
        }; // POS Delivery Address ID

        this.POSBillingAddressID = function() {
            return this.GetSystemDefault("POS Billing Address ID");
        }; // POS Billing Address ID

        this.GetRetailPriceID = function() {

            return ReadyGuid(this.GetSystemDefault('Retail Price ID', '01000012-0005-0000-0000-000000000000'));

        };

        this.GetWholesalePriceID = function() {

            return ReadyGuid(this.GetSystemDefault('Wholesale Price ID', '01000012-0004-0000-0000-000000000000'));

        };

        this.GetLandedCostID = function() {

            return ReadyGuid(this.GetSystemDefault('Landed Cost ID', '01000012-0003-0000-0000-000000000000'));

        };

        this.IsEnableDiningMode = function () {
            return this.GetSystemDefault("Enable Dining Mode");
        };
        this.IsEnableBillingOption = function () {
            return this.GetSystemDefault("Enable Billing Option");
        };
        this.IsEnableVATExemptOption = function () {
            return this.GetSystemDefault("Enable VAT Exempt Option");
        };
        this.GetPublicSystemDefault = function(vDescription) {
            
            var pDT;
            var pSql;

            pSql = 'SELECT DefaultValue' +
                ' FROM SystemDefaults' +
                ' WHERE Description = ' + ReadyString(vDescription);
            pDT = Sql.Execute(pSql);
            if (pDT == undefined) return '';
            if (pDT.Rows.length == 0) return '';
            return pDT.Rows[0].DefaultValue;

        };

        this.SetSystemDefault = function(vDescription, vValue) {

            var pIU;
            var pSql;

            pIU = new InsertUpdate('private.SystemDefaults');
            pIU.Add('DefaultValue', ReadyString(vValue));
            pSql = 'Description = ' + ReadyString(vDescription);
            pSql = pIU.GetUpdate(pSql);
            pSql = Sql.ExecuteSql(pSql);
            if (pSql == 0) {
                pIU.Add('ID', ReadyString(NewID()));
                pIU.Add('Description', ReadyString(vDescription));
                pSql = pIU.GetInsert();
                pSql = Sql.ExecuteSql(pSql);
            }

        };

        this.AllowPriceAdjustment = function() {

            return CBool(this.GetSystemDefault('AllowPriceAdjustment'));

        };

        this.GetEnforceZRead = function() {

            return CBool(this.GetSystemDefault('EnforceZRead'));

        };

        this.Only1SalesRep = function() {

            return CBool(this.GetSystemDefault('Only1SalesRep'));

        };

        this.DontAllowGreaterFreeTypePrices = function() {

            return CBool(this.GetSystemDefault('DontAllowGreaterFreeTypePrices'));

        };

        this.AlwaysTestFreeTypeAPrice = function() {

            return CBool(this.GetSystemDefault('AlwaysTestFreeTypeAPrice'));

        };

        this.SearchOnlyForBarcode = function() {

            return CBool(this.GetSystemDefault('SearchOnlyForBarcode'));

        };

        this.PurchaseOrderSupplierSetting = function() {
            return CStr2(this.GetSystemDefault('PurchaseOrderSupplierSetting')).toLowerCase();
        };

        this.PurchaseOrderSupplierID = function() {
            return CStr2(this.GetSystemDefault('PurchaseOrderSupplierID'));
        };

        this.PurchaseOrderSupplierFowstID = function() {
            return CStr2(this.GetSystemDefault('PurchaseOrderSupplierFowstID'));
        };

        this.PurchaseOrderCustomerSetting = function() {
            return CStr2(this.GetSystemDefault('PurchaseOrderCustomerSetting')).toLowerCase();
        };

        this.PurchaseOrderCustomerID = function() {
            return CStr2(this.GetSystemDefault('PurchaseOrderCustomerID'));
        };

        this.PurchaseOrderCustomerFowstID = function() {
            return CStr2(this.GetSystemDefault('PurchaseOrderCustomerFowstID'));
        };

        this.ShowCondensedProductInformationInSearchResult = function() {
            return CBool(this.GetSystemDefault('ShowCondensedProductInformationInSearchResult'));
        };

        this.ShowCondensedProductInformationInCart = function() {
            return CBool(this.GetSystemDefault('ShowCondensedProductInformationInCart'));
        };

        this.ShowCondensedProductInformationInItemDetailsPage = function() {
            return CBool(this.GetSystemDefault('ShowCondensedProductInformationInItemDetailsPage'));
        };

        this.HideBarcodeScan = function() {
            return CBool(this.GetSystemDefault('HideBarcodeScan'));
        };

        this.HideHotButtonsButton = function() {
            return CBool(this.GetSystemDefault('HideHotButtonsButton'));
        };

        this.HideSearchWithinResults = function() {
            return CBool(this.GetSystemDefault('HideSearchWithinResults'));
        };

        this.ProductMakerDefaultHotButtonTemplateID = function() {
            return this.GetSystemDefault('ProductMakerDefaultHotButtonTemplateID');
        };

        this.IsAutoServiceCharge = function() {
            return CBool(this.GetSystemDefault('IsAutoServiceCharge'));
        };

        this.ServiceCharge = function() {
            return CDec2(this.GetSystemDefault('ServiceCharge'));
        };

        this.AllowOfflineSrvPayments = function() {
            return CBool(this.GetSystemDefault('AllowOfflineSrvPayments'));
        };
        
        this.SalesReturnRestockingFeePercent = function() {
            return CDec2(this.GetSystemDefault('Sales Return Restocking Fee Percent'));
        };

        this.SalesReturntMaximumReturnablePeriod = function() {
            return CDec2(this.GetSystemDefault('Days returnable'));
        };
        
        this.SalesReturntReturnedPrice = function() {
            return CStr2(this.GetSystemDefault('Sales Return Returned Price'));
        };
        
        this.SalesReturntSRVSettings = function() {
            return CStr2(this.GetSystemDefault('Sales Return Voucher Use Settings'));
        };

        this.SalesReturntPrintRefundSignUp = function() {
            return CBool(this.GetSystemDefault('Sales Returnt Print Refund SignUp'));
        };

        //

        this.ShowPaymentCheck = function () {
            return CBool(this.GetSystemDefault('ShowPaymentCheck'));
        };

        this.ShowPaymentDebit = function () {
            return CBool(this.GetSystemDefault('ShowPaymentDebit'));
        };

        this.ShowPaymentCredit = function () {
            return CBool(this.GetSystemDefault('ShowPaymentCredit'));
        };

        this.ShowPaymentSRV = function () {
            return CBool(this.GetSystemDefault('ShowPaymentSRV'));
        };

        this.ShowOtherPayment = function () {
            return CBool(this.GetSystemDefault('ShowOtherPayment'));
        };

        this.ShowPaymentOnCredit = function () {
            return CBool(this.GetSystemDefault('ShowPaymentOnCredit'));
        };

        this.ShowPaymentBillingInfo = function () {
            return CBool(this.GetSystemDefault('ShowPaymentBillingInfo'));
        };

        this.ShowPaymentDeliveryInfo = function () {
            return CBool(this.GetSystemDefault('ShowPaymentDeliveryInfo'));
        };

        this.ShowPaymentDiscounts = function () {
            return CBool(this.GetSystemDefault('ShowPaymentDiscounts'));
        };

        this.ShowPaymentCharges = function () {
            return CBool(this.GetSystemDefault('ShowPaymentCharges'));
        };

        this.ShowPaymentRewards = function () {
            return CBool(this.GetSystemDefault('ShowPaymentRewards'));
        };

        this.ShowPaymentSalesReprensentative = function () {
            return CBool(this.GetSystemDefault('ShowPaymentSalesReprensentative'));
        };

        this.ShowDeliveryPayment = function () {
            return CBool(this.GetSystemDefault('ShowDeliveryPayment'));
        };

        this.HideMerchandiseExchange = function () {
            return CBool(this.GetSystemDefault('HideMerchandiseExchange'));
        };

        this.ServiceChargeBasis = function () {
            // vatex (default) or vatin
            return this.GetSystemDefault('ServiceChargeBasis');
        };

        this.RoundUpSellingPrices = function () {
            // vatex (default) or vatin
            return this.GetSystemDefault('RoundUpSellingPrices');
        };

        this.RoundUpSellingPricesCentavo = function () {
            // vatex (default) or vatin
            return CDec2(this.GetSystemDefault('RoundUpSellingPricesCentavo'));
        };

        this.RestrictedTransactionLookupForSalesReturn = function () {
            return CBool(this.GetSystemDefault('RestrictedTransactionLookupForSalesReturn'));
        };

        this.AllowedReturnDuration = function () {
            return CDec2(this.GetSystemDefault('AllowedReturnDuration'));
        };

        return this;
    };
});