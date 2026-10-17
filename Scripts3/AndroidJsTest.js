window.DbProvisioner = function() {

    var $this = {};

    $this.OrgID = '',
    $this.Done = null;

    $this.Execute = function() {

        if (!$.isFunction($this.Done))
            throw 'DbProvisioner.Done must be a function';

        if (!IsGuid($this.OrgID))
            throw 'DbProvisioner.OrgID must be a valid GUID';

        AjaxSend({
            url: 'DbProvisioner/Execute',
            data: {
                OrgID: $this.OrgID
            },
            success: function(vData) {
                console.log('DbProvisioner/Execute success js9C78LKS56M!');
                $this.Done(vData);
            },
            error: function() {
                console.log('DbProvisioner/Execute error jsEWQ69V38K3!');
            }
        });        
    }; // END Execute

    return $this;

}; // END DbProvisioner

require(['common',
    '/Scripts3/Offline/Web.js?crc=',
    '/Scripts3/Offline/Browser/common.js?crc=',
    '/Scripts3/Stock_offline.js?crc=',
    '/Scripts3/SalesMenu_offline.js?crc=',
    '/Scripts3/AndroidJsTestFlush.js?crc=',
    '/Scripts3/schemaSync/Sender.js?crc='
    ], function () {

    $(function() {
        require(['jasmineBoot'
        ], function() {
//            describe('DbProvisioner', function() {
//                var mDbProvisioner;

//                var pOriginalTimeout;

//                beforeEach(function() {
//                    pOriginalTimeout = jasmine.DEFAULT_TIMEOUT_INTERVAL;
//                    jasmine.DEFAULT_TIMEOUT_INTERVAL = 60000;
//                });

//                afterEach(function() {
//                    jasmine.DEFAULT_TIMEOUT_INTERVAL = pOriginalTimeout;
//                });

//                it('basic', function(vDone) {
//                    mDbProvisioner = new DbProvisioner();
//                    expect($.isFunction(mDbProvisioner.Execute)).toBeTruthy();
//                    mDbProvisioner.OrgID = 'C5DC91C0-1313-A556-B96B-3DDB9F3277AB';
//                    mDbProvisioner.Done = function(vData) {
//                        expect(vData).toBeDefined();
//                        expect(vData.Result).toEqual('ok');
//                        expect(vData.Error).not.toBeDefined();
//                        vDone();
//                    };
//                    mDbProvisioner.Execute();
//                });

//            }); // END DbProvisioner

//            jasmine.Exec();
//            return; // temporary only aug 2, 2014

            describe('basic JS', function() {
                it('CDec2', function() {
                    expect(CDec2('1.23456')).toEqual(1.23456);
                    expect(CDec2('1.78')).toEqual(1.78);
                    expect(CDec2('a1.23456')).toEqual(0);
                });

                it('CInt2', function() {
                    expect(CInt2('1.23456')).toEqual(1);
                    expect(CInt2('1.78')).toEqual(1);
                    expect(CInt2('a1.23456')).toEqual(0);
                });

                it('GetStringObject & CreateStringObject', function() {
                    expect(GetStringObject('qwe.asd.zxc')).not.toBeDefined();
                    expect(GetStringObject('proj.AjaxTimeout')).toBeDefined();
                    expect(GetStringObject('proj.AjaxTimeout.qwe')).not.toBeDefined();
                    expect(CreateStringObject('qwe.asd.zxc')).toBeDefined();
                    expect(qwe.asd.zxc).toBeDefined();
                    qwe.asd.zxc = 'rty';
                    expect(qwe.asd.zxc).toEqual('rty');
                });

                it('CsvWriter, CsvReader, Finder', function() {
                    expect(window.CsvWriter).toBeDefined();
                    expect(window.CsvReader).toBeDefined();
                    expect(window.Finder).toBeDefined();
                });

                it('endsWith', function() {
                    expect('12345'.endsWith('345')).toBeTruthy();
                    expect('12345}'.endsWith('}')).toBeTruthy();
                });

                it('GetFilenameWithoutExtension', function () {
                    expect(GetFilenameWithoutExtension('123')).toEqual('123');
                    expect(GetFilenameWithoutExtension('\\123')).toEqual('123');
                    expect(GetFilenameWithoutExtension('/123')).toEqual('123');
                    expect(GetFilenameWithoutExtension('qwe/123')).toEqual('123');
                    expect(GetFilenameWithoutExtension('qwe\\123')).toEqual('123');
                    expect(GetFilenameWithoutExtension(' qwe/123')).toEqual('123');
                    expect(GetFilenameWithoutExtension(' qwe\\123')).toEqual('123');
                    expect(GetFilenameWithoutExtension('qwe /123')).toEqual('123');
                    expect(GetFilenameWithoutExtension('qwe \\123')).toEqual('123');
                    expect(GetFilenameWithoutExtension(' qwe /123')).toEqual('123');
                    expect(GetFilenameWithoutExtension(' qwe \\123')).toEqual('123');

                    expect(GetFilenameWithoutExtension('123.')).toEqual('123');
                    expect(GetFilenameWithoutExtension('\\123.')).toEqual('123');
                    expect(GetFilenameWithoutExtension('/123.')).toEqual('123');
                    expect(GetFilenameWithoutExtension('qwe/123.')).toEqual('123');
                    expect(GetFilenameWithoutExtension('qwe\\123.')).toEqual('123');
                    expect(GetFilenameWithoutExtension(' qwe/123.')).toEqual('123');
                    expect(GetFilenameWithoutExtension(' qwe\\123.')).toEqual('123');
                    expect(GetFilenameWithoutExtension('qwe /123.')).toEqual('123');
                    expect(GetFilenameWithoutExtension('qwe \\123.')).toEqual('123');
                    expect(GetFilenameWithoutExtension(' qwe /123.')).toEqual('123');
                    expect(GetFilenameWithoutExtension(' qwe \\123.')).toEqual('123');

                    expect(GetFilenameWithoutExtension('123.456')).toEqual('123');
                    expect(GetFilenameWithoutExtension('\\123.456')).toEqual('123');
                    expect(GetFilenameWithoutExtension('/123.456')).toEqual('123');
                    expect(GetFilenameWithoutExtension('qwe/123.456')).toEqual('123');
                    expect(GetFilenameWithoutExtension('qwe\\123.456')).toEqual('123');
                    expect(GetFilenameWithoutExtension(' qwe/123.456')).toEqual('123');
                    expect(GetFilenameWithoutExtension(' qwe\\123.456')).toEqual('123');
                    expect(GetFilenameWithoutExtension('qwe /123.456')).toEqual('123');
                    expect(GetFilenameWithoutExtension('qwe \\123.456')).toEqual('123');
                    expect(GetFilenameWithoutExtension(' qwe /123.456')).toEqual('123');
                    expect(GetFilenameWithoutExtension(' qwe \\123.456')).toEqual('123');
                });

                it('String.repeat', function () {
                    expect('abc'.repeat(3)).toEqual('abcabcabc');
                    expect('b'.repeat(5)).toEqual('bbbbb');
                    expect(''.repeat(15)).toEqual('');
                });

                it('ParseDate', function () {
                    var pFormatted = '03/29/2014 01:30:50 PM';

                    expect(FormatDate(ParseDate(pFormatted))).toEqual(pFormatted);
                    expect(FormatDate(ParseDate(ParseDate(pFormatted)))).toEqual(pFormatted);
                    expect(FormatDate(ParseDate('2014-03-29T13:30:50.000Z'))).toEqual(pFormatted);
                    expect(FormatDate(ParseDate('2014-03-29 13:30:50.000'))).toEqual(pFormatted);
                    expect(FormatDate(ParseDate(ParseDate('2014-03-29T13:30:50.000Z')))).toEqual(pFormatted);

                    expect(IsDate(pFormatted)).toBeTruthy();
                    expect(IsDate('2014-03-29T13:30:50.000Z')).toBeTruthy();
                    expect(IsDate('2014-03-29 13:30:50.000 ')).toBeTruthy();
                    expect(IsDate('2014-03-29 13:30:50.000Z')).toBeTruthy();
                    expect(IsDate('2014-03-29T13:30:50.000 ')).toBeTruthy();
                    expect(IsDate(ParseDate('2014-03-29T13:30:50.000Z'))).toBeTruthy();
                });

                it('ReadyGuid', function () {
                    expect(ReadyGuid('{D6A157BF-CF9F-4566-9585-5D5BF2746FF8}')).toEqual('D6A157BF-CF9F-4566-9585-5D5BF2746FF8');
                    expect(ReadyGuid('D6A157BF-CF9F-4566-9585-5D5BF2746FF8')).toEqual('D6A157BF-CF9F-4566-9585-5D5BF2746FF8');
                    expect(ReadyGuid(null)).toEqual('');
                    expect(ReadyGuid(undefined)).toEqual('');
                    expect(ReadyGuid('{12345')).toEqual('{12345');
                    expect(ReadyGuid('12345}')).toEqual('12345}');
                });

                it('MergeUrlParameters', function() {
                    expect(
                        MergeUrlParameters({
                            IsWindows: 1,
                            IP: '192.168.1.202:8080'
                        }, '/SignIn')
                    ).toEqual('/SignIn?IsWindows=1&IP=192.168.1.202%3A8080');

                }); // END MergeUrlParameters
            });

            describe('Requires IsMobile', function() {
                it('basic', function() {
                    expect(IsMobile).toBeTruthy();
                });

                it('LibraryPIMProduct', function() {
                    expect(Business).toBeDefined();
                    expect(Business.LibraryPIMProduct).toBeDefined();
                });

                describe('Transfers', function () {
                    
                    var pOriginalTimeout;

                    beforeEach(function() {
                      pOriginalTimeout = jasmine.DEFAULT_TIMEOUT_INTERVAL;
                      jasmine.DEFAULT_TIMEOUT_INTERVAL = 60000;
                    });

                    afterEach(function() {
                      jasmine.DEFAULT_TIMEOUT_INTERVAL = pOriginalTimeout;
                    });

                    it('Download', function(vDone) {
                        var pDbName = 'test dummy db from server - deletable file'; // copy this from AndroidJsTestController.DownTestDatabase
                        var pZipFilename = 'test extract zip - deletetable file.zip';

                        // Sql.DeleteIfExist(pDbName + '.db');
                        Transfers.Download({
                            Source: 'AndroidJsTest/DownloadTestDatabase',
                            Destination: pZipFilename,
                            Success: function () {
                                expect(Transfers.IsValidZipFile(pZipFilename)).toBeTruthy();
                                Sql.CloseConnection();
                                Transfers.ExtractZipFile({
                                    Destination: 'data', // this is a folder
                                    ExtractToFiles: true,
                                    Source: pZipFilename,
                                    Success: function (vData) {
                                        var pSql;
                                        var pDT;
                                        var pFormatted = '01/02/2014 03:30:52 PM';
                                        var pIU;

                                        expect(Sql.IsExist(pDbName)).toBeTruthy();
                                        pSql = Sql.OpenDatabase(pDbName);
                                        //                            pSql = Sql.ExecuteSql('DELETE FROM Table1');
                                        //                            ok(IsNumeric(pSql), 'jsJDFEM6UJFK');
                                        pSql = 'SELECT COUNT(*) FROM Table1';
                                        pSql = Sql.ExecuteScalar(pSql);
                                        expect(2 <= CDec2(pSql)).toBeTruthy();
                                        pSql = 'SELECT * FROM Table1';
                                        pDT = Sql.Execute(pSql);
                                        //                        console.table(pDT.Rows);

                                        expect(FormatDate(pDT.Rows[0].DateTime1)).toEqual(pFormatted);
                                        expect(FormatDate(pDT.Rows[1].DateTime2)).toEqual(pFormatted);

                                        Sql.ExecuteSql('DELETE FROM Table1');
                                        pIU = new InsertUpdate('Table1');
                                        // pIU.Add('DateTime1', ReadyString(pFormatted)); // error; invalid date format
                                        pIU.Add('DateTime1', ReadyDate(pFormatted));
                                        pIU.Add('DateTime2', ReadyDate(pFormatted));
                                        pSql = pIU.GetInsert();
                                        pSql = Sql.ExecuteSql(pSql);
                                        expect(IsNumeric(pSql)).toBeTruthy();

                                        pSql = 'SELECT * FROM Table1';
                                        pDT = Sql.Execute(pSql);
                                        expect(FormatDate(pDT.Rows[0].DateTime1)).toEqual(pFormatted);
                                        expect(FormatDate(pDT.Rows[0].DateTime2)).toEqual(pFormatted);
                                        expect(FormatDate(pDT.Rows[0].DateTime1)).toEqual(FormatDate(pDT.Rows[0].DateTime2));

                                        pSql = 'SELECT * FROM Table1 LIMIT 0';
                                        var pDR = Sql.ExecuteRow(pSql);
                                        vDone();
                                    },
                                    Error: function () {
                                        console.trace('Error during extract M82LRHN9LR');
                                    }
                                });
                            },
                            Error: function () {
                                console.trace('Error during download ZWDDRFMJ3R');
                            }
                        });
                    }); // END it('Download', function() {
                }); // END describe Transfers

                describe('Requires Mobiz DUC database', function() {
                    var pOrgID = '9d4b1500-13c0-49f9-a8e5-a16c13ec899e';

                    it('HandleOrganization', function() {
                        expect(Sql.OpenDatabase('Mobiz')).toEqual('ok');
                        expect(Sql.AttachDatabase(pOrgID)).toEqual('ok');

                        Android.SetSessionData({
                            "PeopleUserID": "c530bdf3-ea03-4dd3-95cd-a384374aa570",
                            "Username": "dd",
                            "Password": "dd"
                        });
                        Web.SetOrgID(pOrgID);
                        sessionStorage.FowstID = "abe6d501-3cad-4e79-88ab-f81bbeac9a93";

                        expect(Web.HandleOrganization()).toBeTruthy();
                    });

                    it('Stock_offline', function() {
                        var pStock_offline = Stock_offline();
                        var pResult;

                        expect(pStock_offline).toBeDefined();
                        pResult = pStock_offline.UpdatePos({
                            "StockID": "9d952e0d-324c-43ac-bd80-74522d69124b",
                            "CustomerID": "cd71a0f7-08a9-4601-876e-5f13c064f873",
                            "NewCartMode": "1",
                            "FowstID": "abe6d501-3cad-4e79-88ab-f81bbeac9a93",
                            "Stocks": [
                                {
                                    "Qty": "4355",
                                    "UnitQty": "1",
                                    "PriceLevelID": "01000012-0005-0000-0000-000000000000"
                                }
                            ],
                            "Remarks": "",
                            "IsTestOnly": 1,
                            "SalesRep": [],
                            "ProductSerial": []
                        });
                        expect(pResult).toBeDefined();
                        expect(IsGuid(pResult.OrderID)).toBeTruthy();
                        expect(pResult.Result).toEqual('ok');

                        pResult = pStock_offline.UpdatePos({
                            "StockID": "9d952e0d-324c-43ac-bd80-74522d69124b",
                            "CustomerID": "cd71a0f7-08a9-4601-876e-5f13c064f873",
                            "OrderID": pResult.OrderID,
                            "FowstID": "abe6d501-3cad-4e79-88ab-f81bbeac9a93",
                            "Stocks": [
                                {
                                    "Qty": "555",
                                    "UnitQty": "1",
                                    "PriceLevelID": "01000012-0005-0000-0000-000000000000"
                                }
                            ],
                            "Remarks": "",
                            "IsTestOnly": 1,
                            "SalesRep": [],
                            "ProductSerial": []
                        });
                        expect(pResult).toBeDefined();
                        expect(IsGuid(pResult.OrderID)).toBeTruthy();
                        expect(pResult.Result).toEqual('ok');

                        var pSalesMenu_offline = SalesMenu_offline();
                        var pCart;

                        expect(pSalesMenu_offline).toBeDefined();
                        pCart = pSalesMenu_offline.PosCart({
                            OrderID: pResult.OrderID,
                            DontFetchIfBlank: true
                        });
                        expect(pCart).toBeDefined();
                        expect(pCart.Units.Rows.length).toEqual(1);
                        expect(pCart.Stocks.Rows.length).toEqual(1);

                    }); // END it('Stock_offline', function() {

                    var mRecentlyBought;

                    it('Recently bought', function(vDone) {

                        AjaxSend({
                            url: 'SalesMenu/RecentlyBought',
                            data: { 
                                CustomerID: 'cd71a0f7-08a9-4601-876e-5f13c064f873',
                                DateRangeType: 'Within last year'
                            },
                            success: function (vData) {
                                expect(vData.data.Rows.length).toBeGreaterThan(0);
                                mRecentlyBought = vData.data;
                                vDone();
                            },
                            error: function (vError) {
                                var pDummy;
                            }
                        });

                    }); // END it('Recently bought, AddMultiToCart', function() {

                    it('AddMultiToCart - new cart', function() {
                        var pSalesMenu_offline;
                        var pOut;
                        var pCart;
                        var pCount = 5;
                        var pCustomerID = 'cd71a0f7-08a9-4601-876e-5f13c064f873';
                        var pFowstID = 'abe6d501-3cad-4e79-88ab-f81bbeac9a93';

                        expect(mRecentlyBought).toBeDefined();
                        expect(mRecentlyBought.Rows.length).toBeGreaterThan(0);
                        pSalesMenu_offline = SalesMenu_offline();
                        sessionStorage.CustomerOrganizationID = pCustomerID;
                        Android.UpdateSessionData({FowstID: pFowstID});
                        localStorage.NewCartMode = 1;
                        expect($.isFunction(pSalesMenu_offline.AddMultiToCart)).toBeTruthy();
                        pOut = pSalesMenu_offline.AddMultiToCart(mRecentlyBought.Rows, {
                            Count: pCount
                        });
                        expect(pOut).toBeDefined();
                        expect($.isArray(pOut.NoUnit)).toBeTruthy();
                        expect(pOut.OrderID).toBeDefined();
                        expect(IsGuid(pOut.OrderID)).toBeTruthy();
                        expect(pOut.Success.length + pOut.NoUnit.length).not.toBeGreaterThan(pCount);
                        expect(pOut.Success.length + pOut.NoUnit.length).not.toBeGreaterThan(mRecentlyBought.Rows.length);
                        pCart = pSalesMenu_offline.PosCart({
                            OrderID: pOut.OrderID,
                            DontFetchIfBlank: true
                        });
                        expect(pCart.Stocks.Rows.length).toBeGreaterThan(0);
                        expect(pCart.Stocks.Rows.length).toEqual(pOut.Success.length);
                        expect(pCart.Master.CustomerID).toEqual(pCustomerID);
                        expect(pCart.Master.FowstID).toEqual(pFowstID);
                    }); // END AddMultiToCart - new cart

                    it('AddMultiToCart - existing cart', function() {
                        var pSalesMenu_offline;
                        var pOut;
                        var pCart;
                        var pCount = 5;
                        var pCustomerID = 'cd71a0f7-08a9-4601-876e-5f13c064f873';
                        var pFowstID = 'abe6d501-3cad-4e79-88ab-f81bbeac9a93';
                        var pResult;
                        var pStock_offline = Stock_offline();

                        expect(mRecentlyBought).toBeDefined();
                        expect(mRecentlyBought.Rows.length).toBeGreaterThan(0);

                        pSalesMenu_offline = SalesMenu_offline();

                        sessionStorage.CustomerOrganizationID = pCustomerID;
                        Android.UpdateSessionData({FowstID: pFowstID});

                        pResult = pStock_offline.UpdatePos({
                            "StockID": mRecentlyBought.Rows[0].ID,
                            "CustomerID": pCustomerID,
                            "NewCartMode": "1",
                            "Stocks": [
                                {
                                    "Qty": "555",
                                    "UnitQty": "1",
                                    "PriceLevelID": Business.SystemDefaults.GetRetailPriceID()
                                }
                            ],
                            "Remarks": "",
                            "IsTestOnly": 1,
                            "SalesRep": [],
                            "ProductSerial": []
                        });
                        expect(pResult).toBeDefined();
                        expect(IsGuid(pResult.OrderID)).toBeTruthy();
                        expect(pResult.Result).toEqual('ok');

                        expect($.isFunction(pSalesMenu_offline.AddMultiToCart)).toBeTruthy();
                        sessionStorage.CartOrderID = pResult.OrderID;
                        localStorage.NewCartMode = 0;
                        pOut = pSalesMenu_offline.AddMultiToCart(mRecentlyBought.Rows, {
                            Count: pCount
                        });
                        expect(pOut).toBeDefined();
                        expect($.isArray(pOut.NoUnit)).toBeTruthy();
                        expect(pOut.OrderID).toBeDefined();
                        expect(IsGuid(pOut.OrderID)).toBeTruthy();
                        expect(pOut.Success.length + pOut.NoUnit.length).not.toBeGreaterThan(pCount);
                        expect(pOut.Success.length + pOut.NoUnit.length).not.toBeGreaterThan(mRecentlyBought.Rows.length);
                        expect(pOut.Existing.length).toEqual(1);
                        pCart = pSalesMenu_offline.PosCart({
                            OrderID: pOut.OrderID,
                            DontFetchIfBlank: true
                        });
                        expect(pCart.Stocks.Rows.length).toBeGreaterThan(0);
                        expect(pCart.Stocks.Rows.length).toEqual(pOut.Success.length + pOut.Existing.length);
                        expect(pCart.Master.CustomerID).toEqual(pCustomerID);
                        expect(pCart.Master.FowstID).toEqual(pFowstID);
                        expect(pCart.Units.Rows.length).toBeGreaterThan(0);
                        
                        var pTotalQty = 0;
                        var pDR;
                        var pFoundExisting = false;

                        for (var pUnitIndex = 0; pUnitIndex <= pCart.Units.Rows.length - 1; pUnitIndex++) {   
                            pDR = pCart.Units.Rows[pUnitIndex];
                            if (ReadyGuid(pDR.ProductID) == ReadyGuid(mRecentlyBought.Rows[0].ID)) {
                                expect(CDec2(pDR.Quantity)).toEqual(555);
                                expect(pFoundExisting).toBeFalsy();
                                pFoundExisting = true;
                            };
                            pTotalQty += CDec2(pDR.Quantity);
                        }
                        expect(pFoundExisting).toBeTruthy();
                        expect(pTotalQty).toEqual(555 + pCount - 1);
                    }); // END AddMultiToCart - existing cart

                    AndroidJsTestFlush.Init();

                }); // END describe('Requires Mobiz DUC database', function() {

                describe('Requires Nike database', function() { // 9DA344F3-DADB-4675-B723-15F0C591DA3C
                    var pOrgID = '9DA344F3-DADB-4675-B723-15F0C591DA3C';

                    it('retail price id', function() {
                        Sql.OpenDatabase('Mobiz');    
                        Sql.AttachDatabase(pOrgID);

                        Android.SetSessionData({
                            "PeopleUserID": "c530bdf3-ea03-4dd3-95cd-a384374aa570",
                            "Username": "dd",
                            "Password": "dd"
                        });
                        Web.SetOrgID(pOrgID);
                        expect(Web.HandleOrganization()).toBeTruthy();
                        expect(CDec2(Business.LibraryPIMProduct.GetPrice('07434edd-c9f9-4a3e-97e5-08af2bb7ebc9', 'retail price id', '', '9DA344F3-DADB-4675-B723-15F0C591DA3C'))).toEqual(3450);
                    });               

                    it('SRP (Suggested Retail Price)', function() {
                        var pDT;
                        var pDR;
                        var pSql;

                        Sql.OpenDatabase('Mobiz');    
                        Sql.AttachDatabase(pOrgID);
                        pSql = ' SELECT PriceLevel, Price' + '\n' + 
                            ' ,LibraryPIMProduct.ProductNo' + '\n' + 
                            ' ,LibraryPIMProduct.ProductName' + '\n' + 
                            ' ,LibraryPIMProductPriceLevel.ProductID' + '\n' + 
                            ' FROM LibraryPIMProductPriceLevel' + '\n' + 
                            ' JOIN LibraryPIMProduct ON LibraryPIMProduct.ID = LibraryPIMProductPriceLevel.ProductID' + '\n' + 
                            ' JOIN LibraryPIMPriceLevel ON LibraryPIMPriceLevel.ID = LibraryPIMProductPriceLevel.PriceLevelID' + '\n' + 
                            ' WHERE LibraryPIMPriceLevel.PriceLevel = \'SRP (Suggested Retail Price)\'' + '\n' + 
                            ' AND LibraryPIMProductPriceLevel.OrganizationID = \'9DA344F3-DADB-4675-B723-15F0C591DA3C\'' + '\n' + 
                            ' ORDER BY LibraryPIMProduct.ProductName' + '\n' + 
                            ' LIMIT 10';
                        pDT = Sql.Execute(pSql);
                        expect(pDT.Rows.length).toBeGreaterThan(0);
                        pDR = pDT.Rows[0];
                        // ProductNo = N.LG.17.023.LG
                        expect(CDec2(Business.LibraryPIMProduct.GetSrp(pDR.ProductID, '9DA344F3-DADB-4675-B723-15F0C591DA3C'))).toEqual(CDec2(pDR.Price));
                        expect(CDec2(Business.LibraryPIMProduct.GetSrp('a9e0a13f-937e-4ac2-a055-c5c77894676d', '9DA344F3-DADB-4675-B723-15F0C591DA3C'))).toEqual(1095);

                        pSql = ' SELECT LibraryPIMProduct.ProductNo' + '\n' + 
                            ' ,LibraryPIMProduct.ProductName' + '\n' + 
                            ' FROM LibraryPIMProduct' + '\n' + 
                            ' JOIN LibraryPIMProductPriceLevel ' + '\n' + 
                            ' ON LibraryPIMProductPriceLevel.ProductID = LibraryPIMProduct.ID' + '\n' + 
                            ' AND LibraryPIMProductPriceLevel.OrganizationID = \'9DA344F3-DADB-4675-B723-15F0C591DA3C\'' + '\n' + 
                            ' WHERE LibraryPIMProduct.ProductNo <> \'\'' + '\n' + 
                            ' AND NOT LibraryPIMProduct.ID IN (' + '\n' + 
                            ' 	SELECT LibraryPIMProductPriceLevel.ProductID' + '\n' + 
                            '     FROM LibraryPIMProductPriceLevel' + '\n' + 
                            ' 	JOIN LibraryPIMPriceLevel ON LibraryPIMPriceLevel.ID = LibraryPIMProductPriceLevel.PriceLevelID' + '\n' + 
                            ' 	WHERE LibraryPIMPriceLevel.PriceLevel = \'SRP (Suggested Retail Price)\'' + '\n' + 
                            ' )' + '\n' + 
                            ' ORDER BY LibraryPIMProduct.ProductName' +
                            ' LIMIT 10';
                        pDT = Sql.Execute(pSql);
                        expect(pDT.Rows.length).toBeGreaterThan(0); // 884498193167
                    });

                    it('Stock_offline', function(vDone) {
                        require(['/Scripts3/Stock_offline.js?crc='
                            ], function() {

                            var pStock_offline = Stock_offline;
                            expect($.isFunction(pStock_offline)).toBeTruthy();
                            pStock_offline = pStock_offline();

                            var pInfo = pStock_offline.GetInfoPos({
                                ProductID: "a9e0a13f-937e-4ac2-a055-c5c77894676d", 
                                CustomerID: "cd71a0f7-08a9-4601-876e-5f13c064f873",
                                NewCartMode: 1
                            });
                            expect(pInfo.UnitMeasures.Rows.length).toBeGreaterThan(0);
                            expect(CDec2(pInfo.UnitMeasures.Rows[0].Srp)).toEqual(1095);

                            AjaxSend({
                                ExactUrl: '/Stock?cache=1',
                                dataType: 'text',
                                success: function (vData) {
                                    var pPage = $(vData);
                                    expect(pPage.find('[name=divSrp]').length).toBeGreaterThan(0);
                                    expect(pPage.find('[name=spanSrp]').length).toBeGreaterThan(0);
                                    vDone();
                                } // END success
                            }); // END AjaxSend({
                        }); // END require(['/Scripts3/Stock_offline.js'
                    }); // END it('Stock_offline', function(vDone) {
                }); // END describe('Requires Nike database', function() { // 9DA344F3-DADB-4675-B723-15F0C591DA3C
           
            }); // END describe('Requires IsMobile', function() {

            describe('Server side test', function() {
                it('Assemblies', function(vDone) {
                    AjaxSend({
                        ExactUrl: '/AndroidJsTest/GetAssemblies',
                        success: function(vData) {
                            expect(vData).not.toBeNull();
                            expect(vData.Result).not.toBeNull();
                            expect($.isArray(vData.Result.Rows)).toBeTruthy();
                            console.table(vData.Result.Rows);
                        }, 
                        complete: function() {
                            vDone();
                        }
                    });
                });
            });

            describe('SchemaSyncSender', function() {

                var pOriginalTimeout;

                beforeEach(function() {
                    pOriginalTimeout = jasmine.DEFAULT_TIMEOUT_INTERVAL;
                    jasmine.DEFAULT_TIMEOUT_INTERVAL = 60000 * 10;
                    App.schemaSync.Sender.remoteIp = 'http://localhost:8080';
                });

                afterEach(function() {
                    jasmine.DEFAULT_TIMEOUT_INTERVAL = pOriginalTimeout;
                });

                it('createSnapshot', function(vDone) {
                    App.schemaSync.Sender.privates.createSnapshot({
                        onProgress: function(vMessage) {
                            console.log(vMessage);
                        },
                        onSuccess: function() {
                            expect(true).toBeTruthy();
                            vDone();
                        },
                        onError: function() {
                            expect(false).toBeTruthy();
                            vDone();
                        }
                    });
                }); // END it('createSnapshot', function(vDone) {

                it('sendSnapshot', function(vDone) {
                    App.schemaSync.Sender.privates.sendSnapshot({
                        onProgress: function(vMessage) {
                            console.log(vMessage);
                        },
                        onSuccess: function() {
                            expect(true).toBeTruthy();
                            vDone();
                        },
                        onError: function() {
                            expect(false).toBeTruthy();
                            vDone();
                        }
                    });
                }); // END it('sendSnapshot', function(vDone) {

                it('compare', function(vDone) {
                    App.schemaSync.Sender.privates.compare({
                        onProgress: function(vMessage) {
                            console.log(vMessage);
                        },
                        onSuccess: function() {
                            expect(true).toBeTruthy();
                            vDone();
                        },
                        onError: function() {
                            expect(false).toBeTruthy();
                            vDone();
                        }
                    });
                }); // END it('compare', function(vDone) {

                it('followUp', function(vDone) {
                    App.schemaSync.Sender.privates.followUp({
                        onProgress: function(vData) {
                            console.log('App.schemaSync.Sender.privates.followUp.onProgress: ' + vData.message);
                        },
                        onSuccess: function() {
                            vDone();
                        },
                        onError: function() {
                            console.log('App.schemaSync.Sender.privates.followUp.onError');
                        }
                    });
                }); // END it('followUp', function(vDone) {

            }); // END describe('SchemaSyncSender', function() {

            jasmine.Exec();
        });
    });

}); // define