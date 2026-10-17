require(['common',
    '/Scripts3/Offline/Web.js?crc=',
    '/Scripts3/Offline/Browser/common.js?crc='
    ], function () {

    if (window.CsvWriter == undefined)
        console.log('CsvWriter was not loaded');
    if (window.CsvReader  == undefined)
        console.log('CsvReader was not loaded');
    if (window.Finder  == undefined)
        console.log('Finder was not loaded');

    var mDivLog = $('#divLog');

    if (TestJs())
        EnsureMobile();

    function TestJs() {

        Test_MergeUrlParameters();

        function Test_MergeUrlParameters() {

            var pUrl;

            pUrl = MergeUrlParameters({
                '1st param': 'this is the first param',
                AnotherParam: 'another1',
                anotherparam: 'case sensitive',
                ThisShouldNotAppear: undefined,    
                ThisShouldAlsoNotAppear: null,
            }, 'http://192.168.1.1:8080/myWeb/');
            AddLog(pUrl == 'http://192.168.1.1:8080/myWeb/?1st+param=this+is+the+first+param&AnotherParam=another1&anotherparam=case+sensitive');

            pUrl = MergeUrlParameters({
                IsMobile: IsMobile
            });

            return true;

        }; // END Test_MergeUrlParameters
    }; // END TestJs

    function EnsureMobile()  {
        if (CDec2(GetUrlParameters().IsMobile) == 0) {
            location = MergeUrlParameters({
                IsMobile: 1
            });

            return;
        };

    }; // END EnsureMobile

    $('#butStartOverall').click(function() {
        
        Test();

    });

    function Test_old() {


        AddInput('Android.HideSplash()');

        // this will download and create a "Mobiz" database
//        Sql.DownloadAndApplySchema({
//            Success: function(vData) {
//                var pStr;
//                console.log('Success: DownloadAndApplySchema');

//                // expected result: empty string
//                AddLog('Sql.GetCurrentDatabase(): ' + Sql.GetCurrentDatabase()); // displays the current database. empty string if no open database.
//                
//                // this is how to open a database
//                Sql.OpenDatabase('Mobiz'); // if this database does not exist, error
//                
//                try {
//                    AddLog(Sql.AttachDatabase('non_existing_db')); // will error if non_existing_db does not exist. if already attached, no error
//                } catch (e) {
//                    // this is an expected error. for demo purposes
//                    console.log(e); 
//                };

//                AddLog(Sql.OpenOrCreateDatabase('MobizDUC')); // this create/update the schema of private database
//                AddLog(Sql.AttachDatabase('Mobiz'));

//                AddLog(Sql.Execute('SELECT name FROM sqlite_master'));
//                AddLog(Sql.Execute('PRAGMA database_list'));
//                SqlTest();
//                
//            },
//            Error: function(vData) {
//                console.log('Error in DownloadAndApplySchema: ' + vData);
//            }
//        });

//        function SqlTest() {
//            
//            var pSql;

//            pSql = ' SELECT LibraryOrganizationFOWST.Name AS FowstName' + '\n' + 
//                ' ,LibraryDeviceFowst.TerminalNo' + '\n' + 
//                ' FROM LibraryDeviceFowst' + '\n' + 
//                ' JOIN Mobiz.LibraryOrganizationFOWST LibraryOrganizationFOWST ON LibraryOrganizationFOWST.ID = LibraryDeviceFowst.FowstID';
//            AddLog(Sql.Execute(pSql));

//        };

//        Transfers.WriteFile('to be uploaded.zip', 'LibraryDevice.json', '{"Rows":[{"ID":"999576cd-917b-4c69-bed0-7614423b237d","LastUpdated":"2/19/2013 4:58:02 PM"}]}');
//        Transfers.Upload({
//            SourceFilename: 'to be uploaded.zip',
//            Key: '',
//            Success: function(vData) {
//                console.log('Success: Upload');
//            },
//            Error: function(vData) {
//                console.log('Error: Upload');
//            }
//        });

//        Transfers.Download({
//            Key: '42p3275xveg0nqb5hgfns2fi775szme37l4hnza7ml94i6l3kqi5shann4agftzqimfbbryha15',
//            Destination: 'downloaded.zip',
//            Success: function(vData) {
//                console.log('Results: ' + vData.Results + '; ' + Transfers.ReadFile('downloaded.zip', 'LibraryDevice.json'));
//            }
//        });

//        return;

//        AjaxSend({
//            url: 'MobizExport/RequestDataFromServer',
//            data: { OrgID: '9d4b1500-13c0-49f9-a8e5-a16c13ec899e', PeopleID: '616ed342-7e86-4b09-a747-02f12600f068', DeviceID: '049890c7' },
//            success: function (vData) {
//                Transfers.Download({
//                    Key: vData.Results,
//                    Destination: 'downloaded.zip',
//                    Success: function(vData) {
//                        Transfers.ReadFile('downloaded.zip', 'MobizLibraryPeople.json');
//                    }
//                });
//            },
//            error: function (vError) {
//                console.log('Error: MobizExport/RequestDataFromServer');
//            }
//        });

//        AddInput('NonExistingVariable.DummyFunction()');
//        AddInput('Android.This_is_what_you_get_for_an_invalid_command()');

//        AddInput('Android.AppVersion()');

//        AddInput('Android.GetServer()');
//        AddInput('Android.GetSessionData()');
//        AddInput('Android.SetSessionData({"Ma-overwrite ang existing data ani": "bitaw no"})');
//        AddInput('Android.GetSessionData()');
//        AddInput('Android.UpdateSessionData({"this is a new key": "new value"})');
//        AddInput('Android.GetSessionData()');
//        AddInput('Android.OfflineMode()');
//    
//        AddInput('Transfers.Download(\'This is the key\', \'This is the destination filename.zip\');');
//        AddInput('Transfers.WriteFile("ZipFileInAndroid.zip", "TextFileInsideZipFile.txt", "this is the content of the text file")');
//        AddInput('Transfers.ReadFile("ZipFileInAndroid.zip", "TextFileInsideZipFile.txt")');

//        AddInput('Transfers.WriteFile("ZipFileInAndroid.zip", "TextFileInsideZipFile.txt", "this is the content of the text file 2")');
//        AddInput('Transfers.ReadFile("ZipFileInAndroid.zip", "TextFileInsideZipFile.txt")');

//        AddInput('Transfers.WriteFile("ZipFileInAndroid.zip", "TextFileInsideZipFile2.txt", "text files contain text")');
//        AddInput('Transfers.ReadFile("ZipFileInAndroid.zip", "TextFileInsideZipFile2.txt")');

//        AddInput('Transfers.DeleteFile("ZipFileInAndroid.zip", "TextFileInsideZipFile2.txt")');
//        AddInput('Transfers.ReadFile("ZipFileInAndroid.zip", "TextFileInsideZipFile2.txt")');

//        AddInput('Transfers.DeleteFile("ZipFileInAndroid.zip")');
        
    };

    function AddInput(vCommand) {
        // PARAM:
        //  vCommand: will be eval'd

        var pResult;

        AddInputDiv(vCommand);
        try {
            pResult = eval(vCommand);
            AddLog(pResult);
        } catch (e) {
            AddLog(e.name + ': ' + e.message);
        };
    };

    function AddInputDiv(vCommand) {
        
        mDivLog.append(
            $('<div>')
                .addClass('DivInput')
                .append(
                    $('<pre>').text(vCommand)
                )
        );

    };

    function AddLog(vJson) {
    
        var pHtml = JSON.stringify(vJson, null, 2);

        console.log(pHtml);
        mDivLog.append(
            $('<div>')
                .addClass('DivLog')
                .append(
                    $('<pre>').html(pHtml)
                )
        );

    };
   
    function Test() {

        var mTestAttachedDbTrans = 0;
        var mTestDownloadAndApplySchema = 0;
        var mTestInitDataInServer = 0;
        var mTestDownload = 0;
        var mTestExtract = 0;
        var mTestExtractedToSqlite = 0;
        var mTestCsvWriter = 1;
        var mTestZip = 0;
        var mTestCsvWriterServer = 1;
        var mMaxRecords = 10;

        var mZipFilename = 'ZipCsvTest.zip';

        AddInput('Android.GetServer()');
        AddInput('Android.GetSessionData()');
        AddInput('Android.SetSessionData({"Ma-overwrite ang existing data ani": "bitaw no"})');
        AddInput('Android.GetSessionData()');
        AddInput('Android.UpdateSessionData({"this is a new key": "new value"})');
        AddInput('Android.GetSessionData()');
        AddInput('Android.OfflineMode()');

        AddLog('Sql.IsExist("non_existing"): ' + Sql.IsExist('non_existing')); // expected result: 0
        AddLog('Sql.IsExist("Mobiz"): ' + Sql.IsExist('Mobiz')); // case sensitive; expected result: 1

        TestAttachedDbTrans();
        function TestAttachedDbTrans() {

            if (!mTestAttachedDbTrans) {
                TestDownloadAndApplySchema();
                return;
            };

            var pSql;

            AddLog('BEGIN TestAttachedDbTrans');
            Sql.OpenOrCreateDatabase('Db1');
            
            pSql = 'DROP TABLE IF EXISTS TableTest1';
            Sql.ExecuteSql(pSql);

            pSql = 'CREATE TABLE TableTest1 (\'ID\', \'Name\')';
            Sql.ExecuteSql(pSql);

            Sql.AttachDatabase('Db2'); // 'private' is the default alias
            
            pSql = 'DROP TABLE IF EXISTS private.TableTest2';
            Sql.ExecuteSql(pSql);

            pSql = 'CREATE TABLE private.TableTest2 (\'ID\', \'Name\')';
            Sql.ExecuteSql(pSql);

            AddInput('Sql.InTransaction()');
            Sql.BeginTransaction();
            AddInput('Sql.InTransaction()');

            pSql = 'INSERT INTO TableTest1 (Name) VALUES (\'just a name\')';
            Sql.ExecuteSql(pSql);

            pSql = 'INSERT INTO private.TableTest2 (Name) VALUES (\'just a name\')';
            Sql.ExecuteSql(pSql);

            Sql.CommitTransaction();

            Sql.DeleteIfExist('Db1');
            Sql.DeleteIfExist('Db2');

            AddLog('END TestAttachedDbTrans');
            alert(2);
            TestDownloadAndApplySchema();

        }; // END TestAttachedDbTrans

        function TestDownloadAndApplySchema() {
            if (!mTestDownloadAndApplySchema) {
                TestInitDataInServer(); 
                return;
            };

            Android.Toast('Downloading and applying schema...');
            Sql.DownloadAndApplySchema({
                Success: function(vData) {
                    var pSql;
                    console.log('Success: DownloadAndApplySchema');

                    // expected result: empty string
                    AddLog('Sql.GetCurrentDatabase(): ' + Sql.GetCurrentDatabase()); // displays the current database. empty string if no open database.
                
                    // this is how to open a database
                    Sql.OpenDatabase('Mobiz'); // if this database does not exist, error
                
                    AddLog(Sql.OpenOrCreateDatabase('Mobiz')); // this create/update the schema of private database

                    // if MobizDUC does not exist, it will be created with the downloaded schema 
                    AddLog(Sql.AttachDatabase('MobizDUC')); // this is the secodary database. default alias is "private" (w/o quotes)

                    // this is how to query:
                    // prefixing the database name is optional. but include it (good programming practice).
                    // if the database, already exist, its schema will NOT be updated
                    pSql = 'SELECT *' + '\n' + 
                        'FROM LibraryOrganizationFOWST' + '\n' + 
                        'JOIN private.LibraryDeviceFowst ON private.LibraryDeviceFowst.FowstID = LibraryOrganizationFOWST.ID';
                    AddLog(pSql + ': \n' + Sql.Execute(pSql));

                    // this is how NOT to query.
                    // always prefix the non-primary database (e.g. private)
                    pSql = 'SELECT *' + '\n' + 
                        'FROM LibraryOrganizationFOWST' + '\n' + 
                        'JOIN LibraryDeviceFowst ON LibraryDeviceFowst.FowstID = LibraryOrganizationFOWST.ID';
                    AddLog(pSql + ': \n' + Sql.Execute(pSql));

                    AddLog(Sql.Execute('SELECT name FROM sqlite_master'));
                    AddLog(Sql.Execute('PRAGMA database_list'));
                    TestInitDataInServer();
                },
                Error: function(vData) {
                    Android.Toast('Error in DownloadAndApplySchema: ' + vData.Error);
                }
            }); // END Sql.DownloadAndApplySchema({

        }; // END TestDownloadAndApplySchema

        function TestInitDataInServer() {
            
            if (mTestInitDataInServer) {
                AddLog('BEGIN Getting data from AndroidJsTest/TestCsv');
                AjaxSend({
                    url: 'AndroidJsTest/TestCsv',
                    success: function(vData) {
                        AddLog('END Getting data from AndroidJsTest/TestCsv');
                        TestDownload();
                    }
                });
            } else
                TestDownload();

        }; // END TestInitDataInServer

        function TestDownload() {

            if (mTestExtractedToSqlite || mTestCsvWriter) {
                Sql.OpenDatabase('Mobiz'); // if this database does not exist, error
                Sql.AttachDatabase('MobizDUC');
            };

            if (mTestDownload) {
                AddLog('TestDownload: downloading ZipCsvTest');
                Transfers.Download({
                    Key: 'ZipCsvTest',
                    Destination: mZipFilename,
                    Success: function(vData) {
                        TestExtract();
                    }, 
                    Error: function(vData) {
                        console.log('CsvTest.Error');
                    }
                }); // END Transfers.Download
            } else  
                TestExtract();

        }; // END TestDownload

        function TestExtract() {
            if (!mTestExtract) {
                TestCsvWriter();
                return;
            };

            Transfers.ExtractZipFile({
                Source: mZipFilename,
                Success: function(vData) {
                    // extracted na dri
                    var pDR;
                    var pCsvReader = new CsvReader(vData.Destination); // extracted/{guid}
                    var pIU;
                    var pSql;
                    var pRowIndex;

                    // BEGIN
                    pIU = new InsertUpdate('SystemDefaults');
                    pRowIndex = 0;
                    pCsvReader.Open('SystemDefaults');
                    if (mTestExtractedToSqlite) {
                        pSql = 'DELETE FROM SystemDefaults';
                        Sql.ExecuteSql(pSql);
                    };
                    do {
                        pDR = pCsvReader.ReadLine();
                        if (pDR == undefined) break;

                        if (mTestExtractedToSqlite) {
                            pIU.Add('ID', ReadyString(pDR.ID));
                            pIU.Add('DefaultValue', ReadyString(pDR.DefaultValue));
                            pIU.Add('Description', ReadyString(pDR.Description));
                            pSql = 'SELECT COUNT(*) AS Count FROM SystemDefaults' +
                                ' WHERE ID = ' + ReadyString(pDR.ID);
                            if (Sql.Execute(pSql).Rows[0].Count == 0)
                                pSql = pIU.GetInsert();
                            else
                                pSql = pIU.GetUpdate('ID = ' + ReadyString(pDR.ID));
                            Sql.ExecuteSql(pSql);
                            pRowIndex++; 
                        }; // END mTestExtractedToSqlite
                        if (pRowIndex % 5 == 0)
                            Android.Toast('pCsvReader.ReadLine: ' + pRowIndex);
                    } while (true);
                    Android.Toast('CsvReader: No. of rows: ' + pRowIndex);
                    // END SystemDefaults

                    // BEGIN LibraryPIMProduct
                    pIU = new InsertUpdate('LibraryPIMProduct');
                    pRowIndex = 0;
                    pCsvReader.Open('LibraryPIMProduct');
                    if (mTestExtractedToSqlite) {
                        pSql = 'DELETE FROM LibraryPIMProduct';
                        Sql.ExecuteSql(pSql);
                    };
                    do {
                        pDR = pCsvReader.ReadLine();
                        if (pDR == undefined) break;

                        if (mTestExtractedToSqlite) {
                            pIU.Add('Brand', ReadyString(pDR.Brand));
                            pIU.Add('CustomClassification', ReadyString(pDR.CustomClassification));
                            pIU.Add('DateAdded', ReadyString(pDR.DateAdded));
                            pIU.Add('DivisionDescription', ReadyString(pDR.DivisionDescription));
                            pIU.Add('IsActive', ReadyString(pDR.IsActive));
                            pIU.Add('IsBrandOwner', ReadyString(pDR.IsBrandOwner));
                            pIU.Add('LastUpdated', ReadyString(pDR.LastUpdated));
                            pIU.Add('MarketingText', ReadyString(pDR.MarketingText));
                            pIU.Add('ModelNo', ReadyString(pDR.ModelNo));
                            pIU.Add('ProductFamilyID', ReadyString(pDR.ProductFamilyID));
                            pIU.Add('ProductGroupNo', ReadyString(pDR.ProductGroupNo));
                            pIU.Add('ProductName', ReadyString(pDR.ProductName));
                            pIU.Add('ProductNo', ReadyString(pDR.ProductNo));
                            pIU.Add('ProductSearchString', ReadyString(pDR.ProductSearchString));
                            pIU.Add('PublishedByID', ReadyString(pDR.PublishedByID));
                            pIU.Add('SRP', ReadyString(pDR.SRP));
                            pIU.Add('UNSPSCClassID', ReadyString(pDR.UNSPSCClassID));
                            pIU.Add('UNSPSCCommodityID', ReadyString(pDR.UNSPSCCommodityID));
                            pIU.Add('UNSPSCFamilyID', ReadyString(pDR.UNSPSCFamilyID));
                            pIU.Add('UNSPSCSegmentID', ReadyString(pDR.UNSPSCSegmentID));
                            pIU.Add('UploadedByID', ReadyString(pDR.UploadedByID));
                            pIU.Add('UserID', ReadyString(pDR.UserID));
                            pIU.Add('Warranty', ReadyString(pDR.Warranty));

                            pSql = 'SELECT COUNT(*) AS Count FROM LibraryPIMProduct' +
                                ' WHERE ID = ' + ReadyString(pDR.ID);
                            if (Sql.Execute(pSql).Rows[0].Count == 0)
                                pSql = pIU.GetInsert();
                            else
                                pSql = pIU.GetUpdate('ID = ' + ReadyString(pDR.ID));
                            Sql.ExecuteSql(pSql);
                            pRowIndex++; 
                        }; // END mTestExtractedToSqlite
                        if (pRowIndex % 5 == 0)
                            Android.Toast('pCsvReader.ReadLine: ' + pRowIndex);
                    } while (true);
                    // END LibraryPIMProduct

                    TestCsvWriter();
                }
            }); // END Transfers.OpenCsv            
        }; // END Extract

        function TestCsvWriter() {
            if (!mTestCsvWriter) {
                TestZip();
                return;
            };

            var pDT;
            var pSql;
            var pCsvWriter;
            var pDR;
            var p;

            Transfers.DeleteDirectory('zipped'); // the cache directory of zipped files

            pSql = 'SELECT * FROM SystemDefaults' +
                ' ORDER BY ID';
            if (CDec2(mMaxRecords) > 0)
                pSql += ' LIMIT ' + CDec2(mMaxRecords);
            pDT = Sql.Execute(pSql);
            pCsvWriter = new CsvWriter('TestCsvWriter.zip', 'SystemDefaults');
            pCsvWriter.Delete(); // this will delete the TestCsvWriter.zip folder
            for (p = 0; p <= pDT.Rows.length - 1; p++) {
                pDR = pDT.Rows[p];
                pCsvWriter.Write({
                    ID: pDR.ID,
                    DefaultValue: pDR.DefaultValue,
                    Description: pDR.Description
                });
                if (p % 5 == 0)
                    Android.Toast('TestCsvWriter: ' + p);
            };
            pCsvWriter.Commit();
            Android.Toast('TestCsvWriter: SystemDefaults Commit ' + p);

//            // BEGIN LibraryPIMProduct
//            pSql = 'SELECT * FROM LibraryPIMProduct' +
//                ' ORDER BY ID';
//            if (CDec2(mMaxRecords) > 0)
//                pSql += ' LIMIT ' + CDec2(mMaxRecords);
//            AddLog('BEGIN querying LibraryPIMProduct...');
//            pDT = Sql.Execute(pSql);
//            AddLog('END querying LibraryPIMProduct...');
//            pCsvWriter = new CsvWriter('TestCsvWriter.zip', 'LibraryPIMProduct');
//            for (p = 0; p <= pDT.Rows.length - 1; p++) {
//                pDR = pDT.Rows[p];
//                pCsvWriter.Write({
//                    ID: pDR.ID,
//                    Brand: pDR.Brand,
//                    CustomClassification: pDR.CustomClassification,
//                    DateAdded: pDR.DateAdded,
//                    DivisionDescription: pDR.DivisionDescription,
//                    IsActive: pDR.IsActive,
//                    IsBrandOwner: pDR.IsBrandOwner,
//                    LastUpdated: pDR.LastUpdated,
//                    MarketingText: pDR.MarketingText,
//                    ModelNo: pDR.ModelNo,
//                    ProductFamilyID: pDR.ProductFamilyID,
//                    ProductGroupNo: pDR.ProductGroupNo,
//                    ProductName: pDR.ProductName,
//                    ProductNo: pDR.ProductNo,
//                    ProductSearchString: pDR.ProductSearchString,
//                    PublishedByID: pDR.PublishedByID,
//                    SRP: pDR.SRP,
//                    UNSPSCClassID: pDR.UNSPSCClassID,
//                    UNSPSCCommodityID: pDR.UNSPSCCommodityID,
//                    UNSPSCFamilyID: pDR.UNSPSCFamilyID,
//                    UNSPSCSegmentID: pDR.UNSPSCSegmentID,
//                    UploadedByID: pDR.UploadedByID,
//                    UserID: pDR.UserID,
//                    Warranty: pDR.Warranty
//                });
//                if (p % 5 == 0)
//                    Android.Toast('TestCsvWriter: ' + p);
//            };
//            pCsvWriter.Commit();
//            Android.Toast('TestCsvWriter: LibraryPIMProduct Commit ' + p);
//            // END LibraryPIMProduct

            // BEGIN LibraryPIMProduct
            pSql = 'SELECT * FROM LibraryPIMProduct' +
                ' ORDER BY ID';
            if (CDec2(mMaxRecords) > 0)
                pSql += ' LIMIT ' + CDec2(mMaxRecords);
            AddLog('BEGIN querying LibraryPIMProduct...');
            pDT = Sql.Execute(pSql);
            AddLog('END querying LibraryPIMProduct...');
            pCsvWriter = new CsvWriter('TestCsvWriter.zip', 'LibraryPIMProduct');
            pCsvWriter.WriteTable(pDT);
            // END LibraryPIMProduct

            pCsvWriter.FinalCommit(); // this will zip and store the randomly-generated directory name

            TestZip(pCsvWriter);

        }; // END TestCsvWriter

        function TestZip(vCsvWriter ) {
            // PARAMS:
            //  vCsvWriter (optional)

            if (!mTestZip) {
                TestCsvWriterServer();
                return;
            };
            var pCsvWriter;

            if (vCsvWriter == undefined) {
                pCsvWriter = Transfers;
                pCsvWriter = new CsvWriter('TestCsvWriter.zip', 'dummy');
                pCsvWriter.Write({dummy: 'dummy'});
                pCsvWriter.FinalCommit();
            } else {
                pCsvWriter = vCsvWriter;
            };
            pCsvWriter.Upload({
                Data: {
                    myCustomData: 'ikaw ikaw nlng ani',
                    Effect: 'wla ni effect sa app pero makita ni sa Action'  
                },
                Success: function(vData) {
                    AddLog('SUCCESS: pCsvWriter.Upload');
                    TestCsvWriterServer(JSON.parse(vData.Results).TimeStamp);
                }, // END Success
                Error: function(vData) {
                    AddLog('ERROR: pCsvWriter.Upload. ' + vData.Error);
                }
            }); // END pCsvWriter.Upload
        }; // END TestZip

        function TestCsvWriterServer(vTimeStamp) {

            if (!mTestCsvWriterServer)
                return;

            AjaxSend({
                url: MergeUrlParameters({
                        OrgID: sessionStorage.OrgID
                    }, '/AndroidJsTest/TestCsvWriterServer'),
                data: {
                    TimeStamp: vTimeStamp, // kailangan ni xa para magkasinabot ang mga butang
                    'unsa ni': 'Pd ka mag-add dri'
                },
                success: function(vData) {
                    AddLog('SUCCESS: pCsvWriter.Upload -> AjaxSend');
                    var pMessage;

                    if (vData == undefined) {
                        pMessage = 'Success in TestCsvWriterServer but no vData.Results.';
                    } else {
                        try {
                            if (vData.Result == 'ok') {
                                pMessage = 'Success in TestCsvWriterServer: ' + 
                                    '\nNo. of lines: ' + vData.Count +
                                    '\nSample: ' + vData.Sample;
                            } else
                                pMessage = 'Unexpected result from success in TestCsvWriterServer';
                        } catch (e) {
                            pMessage = 'Success in TestCsvWriterServer but vData.Result is not a valid JSON.';
                        };
                    };
                    Android.Toast(pMessage);                        
                }
            }); // END AjaxSend
                                        
        }; // END TestCsvWriterServer

    }; // END Test

    function TestUpload() {

        MessageBox('wla pa ni xa');

    }; // END TestUpload

}); // define