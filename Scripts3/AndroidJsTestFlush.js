define(['common',
    '/Scripts3/Offline/Web.js?crc=',
    '/Scripts3/Offline/Browser/common.js?crc=',
    '/Scripts3/Stock_offline.js?crc=',
    '/Scripts3/SalesMenu_offline.js?crc=',
    '/Scripts/sources/SalesPayments/SalesPaymentsExport_offline.js?crc='
    ], function () {

    window.AndroidJsTestFlush = (function() {
        
        var mOrgID = '9d4b1500-13c0-49f9-a8e5-a16c13ec899e';

        function Init() {
          
          describe('AndroidJsTestFlush', function() {
            var pOriginalTimeout;

            beforeEach(function() {
                pOriginalTimeout = jasmine.DEFAULT_TIMEOUT_INTERVAL;
                jasmine.DEFAULT_TIMEOUT_INTERVAL = 60000;
            });

            afterEach(function() {
                jasmine.DEFAULT_TIMEOUT_INTERVAL = pOriginalTimeout;
            });

            it('init', function() {
                expect(Sql.OpenDatabase('Mobiz')).toEqual('ok');
                expect(Sql.AttachDatabase(mOrgID)).toEqual('ok');
                Web.SetOrgID(mOrgID);
            });

            it('untag invoices', function() {
                var pSql;

                pSql = 'update transinvoice set dateimported = null';
                pSql = Sql.ExecuteSql(pSql);
                expect(true).toBeTruthy();
            });

            it('create sales order', function() {
                var pProducts = [
                    {
                        "ID": "479bee72-8d32-4f14-a166-b813aaf518c7",
                        "ProductNo": "",
                        "ProductName": ", BAG, Backpack School Bag # 1052 Asst. Design",
                        "InvoiceDate": "8/12/2014 4:15:51 PM",
                        "SubTotal": "2020.0000"
                    },
                    {
                        "ID": "84f208f6-a12b-4770-a018-041e52ede858",
                        "ProductNo": "",
                        "ProductName": "GENERIC/ ORDINARY, SERVICES, Business Permit 3sheets per set, approx 1/2 of short bond paper, (2,000 sets per order for this price)",
                        "InvoiceDate": "8/12/2014 4:15:51 PM",
                        "SubTotal": "5197.5000"
                    }
                ];
                var pSalesMenu_offline = SalesMenu_offline();
                var pCustomerID = 'cd71a0f7-08a9-4601-876e-5f13c064f873';
                var pFowstID = 'abe6d501-3cad-4e79-88ab-f81bbeac9a93';

//                sessionStorage.CustomerOrganizationID = pCustomerID;
//                Android.UpdateSessionData({FowstID: pFowstID});
//                localStorage.NewCartMode = 1;
//                var pOut = pSalesMenu_offline.AddMultiToCart(pProducts);
//                expect(pOut.OrderID).toBeDefined();
//                expect(IsGuid(pOut.OrderID)).toBeTruthy();
//                expect(pOut.Success.length).toBeGreaterThan(0);

//                var pCart = pSalesMenu_offline.PosCart({
//                    OrderID: pOut.OrderID,
//                    DontFetchIfBlank: true
//                });
//                expect(pCart.Stocks.Rows.length).toBeGreaterThan(0);
//                expect(pCart.Units.Rows.length).toBeGreaterThan(0);
//                expect(IsGuid(pCart.Units.Rows[0].ItemID)).toBeTruthy();

//                // modify the unit price
//                var pSql;
//                pSql = 'UPDATE private.TransInvoiceItems' +
//                    ' SET UnitPrice = 666777.4848' +
//                    ',SubTotal = 666777.4848 * Quantity' +
//                    ' WHERE ID = ' + ReadyString(pCart.Units.Rows[0].ItemID);
//                pSql = Sql.ExecuteSql(pSql);
//                expect(CDec2(pSql)).toBeGreaterThan(0);
            });

            it('flush', function(vDone) {
                SalesPaymentsExport_offline().FlushDeviceData({
                    Success: function(vData) {
                        vDone();
                    }   
                }); // END FlushDeviceData
            });

          }); // END describe('AndroidJsTestFlush', function() {
            
        } // END function Init() {

        return {
            Init: Init
        };

    })(); // END window.AndroidJsTestFlush = (function() {

}); // define