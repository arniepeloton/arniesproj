// <BEGIN> put these code in a separate JS file
(function() {
    if (typeof window.test != 'function') {
        window.test = function(){};
        window.ok = function(){};
        window.equal = function(){};
        return;
    }

    QUnit.log(function( details ) {
        if (details.result)
            console.info( "QUnit: " + (details.message || '') + ' : ' + (details.name || ''));
        else {
            console.warn( "QUnit fail: " + (details.message || '') + ' : ' + (details.name || '') + '\r\n' + details.source);
        }
    });

    QUnit.done(function(vData) {
        if (QUnit.SessionResult == undefined)
            QUnit.SessionResult = {
                failed: 0,
                passed: 0,
                total: 0,
                runtime: 0
            };
        QUnit.SessionResult.failed = vData.failed;
        QUnit.SessionResult.passed = vData.passed;
        QUnit.SessionResult.total = vData.total;
        QUnit.SessionResult.runtime = vData.runtime;

        console.info('QUnit Session: ' + QUnit.SessionResult.failed + ' failed; ' +
            QUnit.SessionResult.passed + ' passed; ' +
            QUnit.SessionResult.total + ' total; ' +
            QUnit.SessionResult.runtime + ' runtime');
        
    });
})();
// </BEGIN> put these code in a separate JS file

require(['common',
    '/Scripts3/Offline/Web.js?crc=',
    '/Scripts3/Offline/Browser/common.js?crc='
    ], function () {

//    Transfers.MoveFile('/data/Mobiz.db', '/data/Mobiz.backup.db');
//    Transfers.MoveFile('/data/9d4b1500-13c0-49f9-a8e5-a16c13ec899e.db', '/data/9d4b1500-13c0-49f9-a8e5-a16c13ec899e.backup.db');

//    console.log('2');
//    Transfers.ExtractZipFile({
//        Source: 'ukjm3emufpp1d52gwzjw7dio5y725m6ogscggv6vnch5mvmkb0vijr2xq87bjhjqwhxlm3e4c79.zip',
//        Destination: 'data',
//        ExtractToFiles: true,
//        Success: function () {
//            console.log('c');
//            Sql.DownloadAndApplySchema({
//                ForceUpdate: true,
//                Success: function () {
//                    console.log('D2F3ARLB68');
//                    Sql.DownloadAndApplySchema({
//                        ForceUpdate: true,
//                        Success: function () {
//                            console.log('KATQSPB9SY');
//                        },
//                        Error: function () {
//                            console.log('M9GNY38C5X');
//                        }
//                    });
//                },
//                Error: function () {
//                    console.log('HQP2V7L8DH');
//                }
//            });
//        },
//        Error: function () {
//            alert(3);
//        }
//    });
//    console.log('123123123');
//    Transfers.ExtractZipFile({
//        Source: 'ukjm3emufpp1d52gwzjw7dio5y725m6ogscggv6vnch5mvmkb0vijr2xq87bjhjqwhxlm3e4c79.zip',
//        Destination: 'data',
//        ExtractToFiles: true,
//        Success: function () {
//            console.log('bbbbb');
//            Sql.OpenDatabase('Mobiz');
//            Sql.AttachDatabase('9d4b1500-13c0-49f9-a8e5-a16c13ec899e');
//            Sql.ExecuteSql('update librarydevice set id=id');
//            Sql.ExecuteSql('update LibraryDeviceFowst set id=id');
//            Sql.ExecuteSql('update TransInvoice set id=id');
//            Sql.ExecuteSql('update TransInvoiceItems set id=id');


//        },
//        Error: function () {
//            alert(2);
//        }
//    });

//    test('temp flush error', function() {
//        Transfers.Upload({
//            Destination: MergeUrlParameters({
//                    
//                }, '/OfflineData/Dump'),
//            Data: {
//                OrgID: Web.OrgID(),
//                DeviceID: Web.DeviceID()
//            },
//            Success: function(vData) {
//                console.log('jsRYYCK369XM!');
//            },
//            Error: function(vData) {
//                console.log('jsHHLBT7LFDK!');
//            }
//        });
//            
//    });

    asyncTest('Assemblies', function() {
        expect(1);
        AjaxSend({
            ExactUrl: '/AndroidJsTest/GetAssemblies',
            success: function(vData) {
                console.table(vData.Result.Rows);
                ok(1, 'jsDT9HV7DKWA');
            }, 
            complete: function() {
                start();
            }
        });
    });

    test('CDec2 QZUEHU5FXV', function () {
        equal(CDec2('1.23456'), 1.23456, 'LB524PR6TD');
        equal(CDec2('1.78'), 1.78, '5A4D6GGM3W');
        equal(CDec2('a1.23456'), 0, 'GSKEK46XTD');
    });

    test('CInt2 A89ZGG8GYJ', function () {
        equal(CInt2('1.23456'), 1, 'MGAM7M9QZA');
        equal(CInt2('1.78'), 1, 'DECAPZB5UX');
        equal(CInt2('a1.23456'), 0, '64R3ZRCPKS');
    });

    test('common Y7YWLVB7T3', function () {
        ok(IsEmail('some.one@mobiz-apps.com'), 'WWWQH2J262');
        equal(GetStringObject('qwe.asd.zxc'), undefined, 'DLFNPQ5CBJ');
        equal(GetStringObject('qwe.asd.zxc'), undefined, 'XUKPB32VC3');
        notEqual(GetStringObject('proj.AjaxTimeout'), undefined, '6TSCJSKJ7U');
        equal(GetStringObject('proj.AjaxTimeout.qwe'), undefined, 'JBNTYKMFVR');
        notEqual(CreateStringObject('qwe.asd.zxc'), undefined, '3F9YV593UW');
        notEqual(qwe.asd.zxc, undefined, '4L3S26V8PH');
        qwe.asd.zxc = 'rty';
        equal(qwe.asd.zxc, 'rty', 'PKFV5FLWXB');
    });

    test('basic', function () {
        ok(window.CsvWriter != undefined, 'window.CsvWriter != undefined QBKER9PHHI');
        ok(window.CsvReader != undefined, 'window.CsvReader != undefined 29SOLLWRY4');
        ok(window.Finder != undefined, 'window.Finder != undefined YWU5UYAGPJ');
        if (!CBool(window.InWindows))
            ok(CStr2(localStorage.IP) != '', 'localStorage.IP A9ZCZZAFQY');
        equal(IsMobile, true, 'IsMobile == 1 M09GD3YXJD');
    });

    test('endsWith', function () {
        ok('12345'.endsWith('345'), 'VJCD4ZKHJ1');
        ok('12345}'.endsWith('}'), 'E0PP2Z4BWJ');
    });

    test('GetFilenameWithoutExtension L8VYP9MKTJ', function () {
        equal(GetFilenameWithoutExtension('123'), '123', '');
        equal(GetFilenameWithoutExtension('\\123'), '123', '');
        equal(GetFilenameWithoutExtension('/123'), '123', '');
        equal(GetFilenameWithoutExtension('qwe/123'), '123', '');
        equal(GetFilenameWithoutExtension('qwe\\123'), '123', '');
        equal(GetFilenameWithoutExtension(' qwe/123'), '123', '');
        equal(GetFilenameWithoutExtension(' qwe\\123'), '123', '');
        equal(GetFilenameWithoutExtension('qwe /123'), '123', '');
        equal(GetFilenameWithoutExtension('qwe \\123'), '123', '');
        equal(GetFilenameWithoutExtension(' qwe /123'), '123', '');
        equal(GetFilenameWithoutExtension(' qwe \\123'), '123', '');

        equal(GetFilenameWithoutExtension('123.'), '123', '');
        equal(GetFilenameWithoutExtension('\\123.'), '123', '');
        equal(GetFilenameWithoutExtension('/123.'), '123', '');
        equal(GetFilenameWithoutExtension('qwe/123.'), '123', '');
        equal(GetFilenameWithoutExtension('qwe\\123.'), '123', '');
        equal(GetFilenameWithoutExtension(' qwe/123.'), '123', '');
        equal(GetFilenameWithoutExtension(' qwe\\123.'), '123', '');
        equal(GetFilenameWithoutExtension('qwe /123.'), '123', '');
        equal(GetFilenameWithoutExtension('qwe \\123.'), '123', '');
        equal(GetFilenameWithoutExtension(' qwe /123.'), '123', '');
        equal(GetFilenameWithoutExtension(' qwe \\123.'), '123', '');

        equal(GetFilenameWithoutExtension('123.456'), '123', '');
        equal(GetFilenameWithoutExtension('\\123.456'), '123', '');
        equal(GetFilenameWithoutExtension('/123.456'), '123', '');
        equal(GetFilenameWithoutExtension('qwe/123.456'), '123', '');
        equal(GetFilenameWithoutExtension('qwe\\123.456'), '123', '');
        equal(GetFilenameWithoutExtension(' qwe/123.456'), '123', '');
        equal(GetFilenameWithoutExtension(' qwe\\123.456'), '123', '');
        equal(GetFilenameWithoutExtension('qwe /123.456'), '123', '');
        equal(GetFilenameWithoutExtension('qwe \\123.456'), '123', '');
        equal(GetFilenameWithoutExtension(' qwe /123.456'), '123', '');
        equal(GetFilenameWithoutExtension(' qwe \\123.456'), '123', '');

    });

    test('String.repeat SLBTEML3S4', function () {
        equal('abc'.repeat(3), 'abcabcabc', 'U7MY7UFP9J');
        equal('b'.repeat(5), 'bbbbb', '4CWNSEM4GJ');
        equal(''.repeat(15), '', 'PGWUYPLYMG');
    });

    test('ParseDate', function () {
        var pFormatted = '03/29/2014 01:30:50 PM';

        equal(FormatDate(ParseDate(pFormatted)), pFormatted, 'N6S2E5ZDDU');
        equal(FormatDate(ParseDate(ParseDate(pFormatted))), pFormatted, 'NYXJA5SHGA');
        equal(FormatDate(ParseDate('2014-03-29T13:30:50.000Z')), pFormatted, 'ZES4AMZVDY');
        equal(FormatDate(ParseDate('2014-03-29 13:30:50.000')), pFormatted, 'M6L2CKC7D5');
        equal(FormatDate(ParseDate(ParseDate('2014-03-29T13:30:50.000Z'))), pFormatted, 'V5A3XWNDUR');

        ok(IsDate(pFormatted), 'KPI0UKMSHE');
        ok(IsDate('2014-03-29T13:30:50.000Z'), 'VJOALPATX9');
        ok(IsDate('2014-03-29 13:30:50.000 '), 'K2UWD4DYF9');
        ok(IsDate('2014-03-29 13:30:50.000Z'), 'E9WH8FEY8X');
        ok(IsDate('2014-03-29T13:30:50.000 '), 'NRARE7NYXH');
        ok(IsDate(ParseDate('2014-03-29T13:30:50.000Z')), 'W5CHELVAZY');
    });

    test('ReadyGuid', function () {
        equal(ReadyGuid('{D6A157BF-CF9F-4566-9585-5D5BF2746FF8}'), 'D6A157BF-CF9F-4566-9585-5D5BF2746FF8', '7Y06QTA3WN');
        equal(ReadyGuid('D6A157BF-CF9F-4566-9585-5D5BF2746FF8'), 'D6A157BF-CF9F-4566-9585-5D5BF2746FF8', 'CSLBK78ZEY');
        equal(ReadyGuid(null), '', 'YI5J33TS48');
        equal(ReadyGuid(undefined), '', 'AWESYAD5CP');
        equal(ReadyGuid('{12345'), '{12345', '5HTAHOC0KY');
        equal(ReadyGuid('12345}'), '12345}', '5RS4VJ2U6A');
    });

    // function below requires an Android device

    test('Transfers.MoveFile HYJRLP5FKJ', function () {
        var pSourceFilename = 'MoveFile_TestFile.txt';
        var pFileContent = 'this is the content of ' + pSourceFilename;
        var pDestinationFilename = 'MoveFile_TestFile2.txt';
        Transfers.DeleteFile(pSourceFilename);
        Transfers.WriteFile(pSourceFilename, null, pFileContent);
        Transfers.MoveFile('transfers/' + pSourceFilename, 'transfers/' + pDestinationFilename);
        ok(Transfers.ReadFile(pDestinationFilename) == pFileContent, 'F39G9WDJQV');
        Transfers.DeleteFile(pDestinationFilename);
    });

    test('Android DKL6K7TKAJ', function () {
        notEqual(Android.AppVersion(), undefined, 'JZGH6C7SNT');
        notEqual(Android.AppBuildDate(), undefined, '76C3J7RJWS');
        notEqual(Android.AppInstallDate(), undefined, 'QG3F8KFLQA');
    });

    test('Csv', function () {
        var pJsonStr = '{"Columns":{"ID":{"Name":"ID","MajorDataType":"String"},"_StockSortingListID":{"Name":"_StockSortingListID","MajorDataType":"String"},"_InvoiceDefaultID":{"Name":"_InvoiceDefaultID","MajorDataType":"String"},"InvoiceNumber":{"Name":"InvoiceNumber","MajorDataType":"Numeric"},"InvoiceDate":{"Name":"InvoiceDate","MajorDataType":"Date"},"_SalesOrderID":{"Name":"_SalesOrderID","MajorDataType":"String"},"CustomerID":{"Name":"CustomerID","MajorDataType":"String"},"BillingAddressID":{"Name":"BillingAddressID","MajorDataType":"String"},"DeliveryAddressID":{"Name":"DeliveryAddressID","MajorDataType":"String"},"DeliveryDate":{"Name":"DeliveryDate","MajorDataType":"Date"},"Remarks":{"Name":"Remarks","MajorDataType":"String"},"_BracketNumber":{"Name":"_BracketNumber","MajorDataType":"Numeric"},"TotalAmount":{"Name":"TotalAmount","MajorDataType":"Numeric"},"VAT":{"Name":"VAT","MajorDataType":"Numeric"},"_NumberOfPrints":{"Name":"_NumberOfPrints","MajorDataType":"Numeric"},"IsCancelled":{"Name":"IsCancelled","MajorDataType":"Numeric"},"_IsHasTaxWithHeld":{"Name":"_IsHasTaxWithHeld","MajorDataType":"Numeric"},"_IsServedBackOrder":{"Name":"_IsServedBackOrder","MajorDataType":"Numeric"},"_IsCustomerFirstOrder":{"Name":"_IsCustomerFirstOrder","MajorDataType":"Numeric"},"LastUpdated":{"Name":"LastUpdated","MajorDataType":"Date"},"_EncodedBy":{"Name":"_EncodedBy","MajorDataType":"Numeric"},"_IsExported":{"Name":"_IsExported","MajorDataType":"Numeric"},"_ExportCount":{"Name":"_ExportCount","MajorDataType":"Numeric"},"GP":{"Name":"GP","MajorDataType":"Numeric"},"GPM":{"Name":"GPM","MajorDataType":"Numeric"},"_InvoiceDateTime":{"Name":"_InvoiceDateTime","MajorDataType":"Date"},"_InvoiceNoExtension":{"Name":"_InvoiceNoExtension","MajorDataType":"String"},"_CancelledBy":{"Name":"_CancelledBy","MajorDataType":"Numeric"},"_CancelReasonID":{"Name":"_CancelReasonID","MajorDataType":"String"},"_PerfectOrder":{"Name":"_PerfectOrder","MajorDataType":"Numeric"},"_TeamID":{"Name":"_TeamID","MajorDataType":"Numeric"},"_PercentIncentive":{"Name":"_PercentIncentive","MajorDataType":"Numeric"},"_isCollected":{"Name":"_isCollected","MajorDataType":"Numeric"},"_IsComputedRewardPoints":{"Name":"_IsComputedRewardPoints","MajorDataType":"Numeric"},"_IsClaimedReward":{"Name":"_IsClaimedReward","MajorDataType":"Numeric"},"_DontAutoPrint":{"Name":"_DontAutoPrint","MajorDataType":"Numeric"},"_DontAutoPrintUncheckerID":{"Name":"_DontAutoPrintUncheckerID","MajorDataType":"Numeric"},"_BranchID":{"Name":"_BranchID","MajorDataType":"Numeric"},"_SubscriberBranchID":{"Name":"_SubscriberBranchID","MajorDataType":"String"},"DeviceID":{"Name":"DeviceID","MajorDataType":"String"},"CusRepresentativeID":{"Name":"CusRepresentativeID","MajorDataType":"String"},"PickUpLocationID":{"Name":"PickUpLocationID","MajorDataType":"String"},"IsDowntown":{"Name":"IsDowntown","MajorDataType":"Numeric"},"DeliverWithin":{"Name":"DeliverWithin","MajorDataType":"Numeric"},"DeliveryType":{"Name":"DeliveryType","MajorDataType":"String"},"PickupDate":{"Name":"PickupDate","MajorDataType":"Date"},"DeliveryMethod":{"Name":"DeliveryMethod","MajorDataType":"String"},"IsRushDelivery":{"Name":"IsRushDelivery","MajorDataType":"Numeric"},"NetInvoiceAmount":{"Name":"NetInvoiceAmount","MajorDataType":"Numeric"},"GrossSrvAmount":{"Name":"GrossSrvAmount","MajorDataType":"Numeric"},"GrossRefundAmount":{"Name":"GrossRefundAmount","MajorDataType":"Numeric"},"TotalAdjustments":{"Name":"TotalAdjustments","MajorDataType":"Numeric"},"UserID":{"Name":"UserID","MajorDataType":"String"},"FowstID":{"Name":"FowstID","MajorDataType":"String"},"PrintCount":{"Name":"PrintCount","MajorDataType":"Numeric"},"RefNum":{"Name":"RefNum","MajorDataType":"String"},"ReasonForReconstructingData":{"Name":"ReasonForReconstructingData","MajorDataType":"String"},"IsForGovReport":{"Name":"IsForGovReport","MajorDataType":"Numeric"},"DiscountAmount":{"Name":"DiscountAmount","MajorDataType":"Numeric"},"SalesOrderNo":{"Name":"SalesOrderNo","MajorDataType":"Numeric"},"SalesOrderDate":{"Name":"SalesOrderDate","MajorDataType":"Date"},"SalesOrderUserID":{"Name":"SalesOrderUserID","MajorDataType":"String"},"CustomerNameReferenceOnly":{"Name":"CustomerNameReferenceOnly","MajorDataType":"String"},"CustomerTableNo":{"Name":"CustomerTableNo","MajorDataType":"String"},"CustomerTableNoOfPeople":{"Name":"CustomerTableNoOfPeople","MajorDataType":"Numeric"},"CustomerReferenceNumber":{"Name":"CustomerReferenceNumber","MajorDataType":"Numeric"},"IsTestOnly":{"Name":"IsTestOnly","MajorDataType":"Numeric"},"SubNumber":{"Name":"SubNumber","MajorDataType":"Numeric"},"SalesOrderID":{"Name":"SalesOrderID","MajorDataType":"String"},"DateImported":{"Name":"DateImported","MajorDataType":"Date"}},"Rows":[{"ID":"f239099b-b44b-3ad0-fb47-07058f3f38c5","_StockSortingListID":"","_InvoiceDefaultID":"","InvoiceNumber":"1","InvoiceDate":"7/3/2014 9:06:53 AM","_SalesOrderID":"","CustomerID":"cd71a0f7-08a9-4601-876e-5f13c064f873","BillingAddressID":"{835C2038-6A44-4E9C-B432-FDEDCE6B49D7}","DeliveryAddressID":"{835C2038-6A44-4E9C-B432-FDEDCE6B49D7}","DeliveryDate":"","Remarks":"","_BracketNumber":"","TotalAmount":"7","VAT":"","_NumberOfPrints":"","IsCancelled":"0","_IsHasTaxWithHeld":"","_IsServedBackOrder":"","_IsCustomerFirstOrder":"","LastUpdated":"7/2/2014 5:00:12 PM","_EncodedBy":"","_IsExported":"","_ExportCount":"","GP":"","GPM":"","_InvoiceDateTime":"","_InvoiceNoExtension":"","_CancelledBy":"","_CancelReasonID":"","_PerfectOrder":"","_TeamID":"","_PercentIncentive":"","_isCollected":"","_IsComputedRewardPoints":"","_IsClaimedReward":"","_DontAutoPrint":"","_DontAutoPrintUncheckerID":"","_BranchID":"","_SubscriberBranchID":"","DeviceID":"9986f4f5-525a-49d6-be23-bc19d633f534","CusRepresentativeID":"","PickUpLocationID":"6f1ffed7-8c0f-4374-bea9-e204fb6e6acc","IsDowntown":"0","DeliverWithin":"0","DeliveryType":"","PickupDate":"","DeliveryMethod":"Pickup","IsRushDelivery":"0","NetInvoiceAmount":"","GrossSrvAmount":"","GrossRefundAmount":"","TotalAdjustments":"","UserID":"c530bdf3-ea03-4dd3-95cd-a384374aa570","FowstID":"6f1ffed7-8c0f-4374-bea9-e204fb6e6acc","PrintCount":"","RefNum":"","ReasonForReconstructingData":"","IsForGovReport":"0","DiscountAmount":"0","SalesOrderNo":"","SalesOrderDate":"","SalesOrderUserID":"","CustomerNameReferenceOnly":"","CustomerTableNo":"","CustomerTableNoOfPeople":"","CustomerReferenceNumber":"","IsTestOnly":"0","SubNumber":"","SalesOrderID":"","DateImported":""}]}';
        var pDT = JSON.parse(pJsonStr);
        var pCsvWriter = new CsvWriter('InvoiceUpload.zip', 'TransInvoice');
        pCsvWriter.Delete();
        pCsvWriter.WriteTable(pDT); // no need to commit
        var pContent = Transfers.ReadFile('zipcache/InvoiceUpload.zip/TransInvoice/00001.csv');
        equal(pContent, 'ID,_StockSortingListID,_InvoiceDefaultID,InvoiceNumber,InvoiceDate,_SalesOrderID,CustomerID,BillingAddressID,DeliveryAddressID,DeliveryDate,Remarks,_BracketNumber,TotalAmount,VAT,_NumberOfPrints,IsCancelled,_IsHasTaxWithHeld,_IsServedBackOrder,_IsCustomerFirstOrder,LastUpdated,_EncodedBy,_IsExported,_ExportCount,GP,GPM,_InvoiceDateTime,_InvoiceNoExtension,_CancelledBy,_CancelReasonID,_PerfectOrder,_TeamID,_PercentIncentive,_isCollected,_IsComputedRewardPoints,_IsClaimedReward,_DontAutoPrint,_DontAutoPrintUncheckerID,_BranchID,_SubscriberBranchID,DeviceID,CusRepresentativeID,PickUpLocationID,IsDowntown,DeliverWithin,DeliveryType,PickupDate,DeliveryMethod,IsRushDelivery,NetInvoiceAmount,GrossSrvAmount,GrossRefundAmount,TotalAdjustments,UserID,FowstID,PrintCount,RefNum,ReasonForReconstructingData,IsForGovReport,DiscountAmount,SalesOrderNo,SalesOrderDate,SalesOrderUserID,CustomerNameReferenceOnly,CustomerTableNo,CustomerTableNoOfPeople,CustomerReferenceNumber,IsTestOnly,SubNumber,SalesOrderID,DateImported' + '\r\n' +
            'f239099b-b44b-3ad0-fb47-07058f3f38c5,,,1,7/3/2014 9:06:53 AM,,cd71a0f7-08a9-4601-876e-5f13c064f873,{835C2038-6A44-4E9C-B432-FDEDCE6B49D7},{835C2038-6A44-4E9C-B432-FDEDCE6B49D7},,,,7,,,0,,,,7/2/2014 5:00:12 PM,,,,,,,,,,,,,,,,,,,,9986f4f5-525a-49d6-be23-bc19d633f534,,6f1ffed7-8c0f-4374-bea9-e204fb6e6acc,0,0,,,Pickup,0,,,,,c530bdf3-ea03-4dd3-95cd-a384374aa570,6f1ffed7-8c0f-4374-bea9-e204fb6e6acc,,,,0,0,,,,,,,,0,,,\r\n', 'jsVKAC8Q8D39');
        pCsvWriter.FinalCommit();

        pCsvWriter.Upload({
            Destination: MergeUrlParameters({

            }, '/OfflineData/Dump'),
            Data: {
                OrgID: 'whatever',
                DeviceID: 'whenever'
            },
            Success: function (vData) {
                    
            },
            Error: function (vData) {
            }
        });

        ok(true, 'jsNMDAYDM35U');
    });

    asyncTest('Transfers', function () {
        expect(9);

        var pDbName = 'test dummy db from server - deletable file'; // copy this from AndroidJsTestController.DownTestDatabase
        var pZipFilename = 'test extract zip - deletetable file.zip';

        // Sql.DeleteIfExist(pDbName + '.db');
        Transfers.Download({
            Source: 'AndroidJsTest/DownloadTestDatabase',
            Destination: pZipFilename,
            Success: function () {
                ok(Transfers.IsValidZipFile(pZipFilename), 'YS3EUVQ5M4');
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

                        ok(Sql.IsExist(pDbName), 'ZN87H53579');
                        pSql = Sql.OpenDatabase(pDbName);
                        //                            pSql = Sql.ExecuteSql('DELETE FROM Table1');
                        //                            ok(IsNumeric(pSql), 'jsJDFEM6UJFK');
                        pSql = 'SELECT COUNT(*) FROM Table1';
                        pSql = Sql.ExecuteScalar(pSql);
                        ok(2 <= CDec2(pSql), 'EVMDL9MDVH');
                        pSql = 'SELECT * FROM Table1';
                        pDT = Sql.Execute(pSql);
                        //                        console.table(pDT.Rows);

                        equal(FormatDate(pDT.Rows[0].DateTime1), pFormatted, 'HWE3TVS7RJ');
                        equal(FormatDate(pDT.Rows[1].DateTime2), pFormatted, 'PGNNSWZVCK');

                        Sql.ExecuteSql('DELETE FROM Table1');
                        pIU = new InsertUpdate('Table1');
                        // pIU.Add('DateTime1', ReadyString(pFormatted)); // error; invalid date format
                        pIU.Add('DateTime1', ReadyDate(pFormatted));
                        pIU.Add('DateTime2', ReadyDate(pFormatted));
                        pSql = pIU.GetInsert();
                        pSql = Sql.ExecuteSql(pSql);
                        ok(IsNumeric(pSql), 'jsBJ5B3RV8BJ');

                        pSql = 'SELECT * FROM Table1';
                        pDT = Sql.Execute(pSql);
                        equal(FormatDate(pDT.Rows[0].DateTime1), pFormatted, '244Y7XJ8UB');
                        equal(FormatDate(pDT.Rows[0].DateTime2), pFormatted, 'V7M6NJS9CV');
                        equal(FormatDate(pDT.Rows[0].DateTime1), FormatDate(pDT.Rows[0].DateTime2), 'Q3334DU2U3');

                        pSql = 'SELECT * FROM Table1 LIMIT 0';
                        var pDR = Sql.ExecuteRow(pSql);
                        start();
                    },
                    Error: function () {
                        console.trace('Error during extract M82LRHN9LR');
                        start();
                    }
                });
            },
            Error: function () {
                console.trace('Error during download ZWDDRFMJ3R');
                start();
            }
        });
    });

    asyncTest('DownloadAndApplySchema', function () {
        expect(1);
        Sql.DownloadAndApplySchema({
            ForceUpdate: true,
            Success: function (vData) {
                ok(true, 'SAWBWUDNP3');
                start();
            },
            Error: function (vData) {
                console.log('Error in DownloadAndApplySchema: ' + vData);
                ok(false, 'PZ3KVA2LJH');
                start();
            }
        });
    });

        //        test('sql', function () {
        //            var pHasMobiz;
        //            var pRecordCount;
        //            var pSql;
        //            var pDecimal;
        //            var pLandedCostID;
        //            var pDbs = Sql.GetDatabaseFiles();
        //            var pOrgID;

        //            ok(Array.isArray(pDbs), 'Sql.GetDatabaseFiles is array MFBQPSPYLH');
        //            for (var p = 0; p <= pDbs.length - 1; p++) {
        //                if (pDbs[p].toLowerCase() == 'mobiz.db') {
        //                    pHasMobiz = true;
        //                } else if (pOrgID == undefined) {
        //                    if (pDbs[p].toLowerCase().endsWith('.db')) {
        //                        pOrgID = pDbs[p].substring(0, pDbs[p].length - 3);
        //                        if (pOrgID)
        //                            if (pOrgID != '34901a22-064f-4d7a-a398-f8c4b55c0709') // Proton
        //                                pOrgID = undefined;
        //                    }
        //                }
        //            }

        //            ok(pHasMobiz, 'has mobiz db QQJ5Z9K4NS');
        //            Sql.OpenDatabase('Mobiz');
        //            ok(IsDate(Sql.ExecuteScalar('SELECT ' + proj.SqliteNow)), 'sqlite now is a valid date 3NX5FY4OY2');

        //            pSql = 'SELECT COUNT(*) FROM SystemDefaults';
        //            pRecordCount = Sql.ExecuteScalar(pSql);
        //            ok(pRecordCount > 0, 'SystemDefaults has records ' + pRecordCount + ' N9AG94KSSW');

        //            ok(typeof Business == 'object', 'typeof Business  R0PAJQK7O7');
        //            ok(typeof Business.SystemDefaults == 'object', 'typeof Business.SystemDefaults IUU8HTG7RA');
        //            ok(typeof Business.SystemDefaults.GetSystemDefault == 'function', 'typeof Business.SystemDefaults.GetSystemDefault WTIQEOMZP4');
        //            ok(typeof Business.SystemDefaults.GetPublicSystemDefault == 'function', 'typeof Business.SystemDefaults.GetPublicSystemDefault Q0G785KSLF');
        //            ok(typeof Business.LibraryPIMProduct.GetLandedCost == 'function', 'typeof Business.LibraryPIMProduct.GetLandedCost PYE68QQQZT');
        //            ok(typeof Business.LibraryPIMProduct.GetRetailPrice == 'function', 'typeof Business.LibraryPIMProduct.GetRetailPrice YX2J64CWIT');
        //            pLandedCostID = Business.SystemDefaults.GetPublicSystemDefault('Landed Cost ID');
        //            ok(IsGuid(pLandedCostID), 'IsGuid(pLandedCostID) XWKX5NB4NP');

        //            notEqual(pOrgID, null, 'OrgID != null 2TPSBMAXT9');
        //            if (pOrgID == undefined)
        //                return;

        //            // from here on requires OrgID
        //            ok(Sql.AttachDatabase(pOrgID), 'Sql.AttachDatabase(pOrgID) 5CIKLBY2DU');
        //            pDecimal = Business.LibraryPIMProduct.GetPrice('non existing product ID', 'invalid guid');
        //            equal(pDecimal, null, 'Business.LibraryPIMProduct.GetPrice EEJGCZ89MR');

        //            pDecimal = Business.LibraryPIMProduct.GetLandedCost('non existing product ID');
        //            equal(pDecimal, null, 'Business.LibraryPIMProduct.GetLandedCost RSBEAV1LVV');

        //            pDecimal = Business.LibraryPIMProduct.GetLandedCost('43e7f8d0-52a6-43bb-b607-d5d08f14f9da', pOrgID);
        //            notEqual(pDecimal, null, 'Business.LibraryPIMProduct.GetLandedCost 43e7f8d0-52a6-43bb-b607-d5d08f14f9da 74ZCOBWSQX');

        //            pDecimal = Business.LibraryPIMProduct.GetRetailPrice('43e7f8d0-52a6-43bb-b607-d5d08f14f9da', pOrgID);
        //            notEqual(pDecimal, null, 'Business.LibraryPIMProduct.GetRetailPrice 43e7f8d0-52a6-43bb-b607-d5d08f14f9da 5690KJZ87P');

        //        });

    });            // define