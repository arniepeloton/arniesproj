// offline functions
define(['common'
    ], function () {

        CreateStringObject('App.model.BomMaker', null, function () {

            var mOut = {};

            mOut.getContents = function (vParams) {
                /*
                vParams = {
                productID: 'guid',
                success: function(vData) {
                },
                error: function(vMessage) {
                }
                };
                */

                var pSql;
                var pDT;

                try {
                    pSql = ' SELECT DISTINCT LibraryPIMProductContents.ChildProductID\n' +
                    ',LibraryPIMProduct.ProductNo' + '\n' +
                    ' ,LibraryPIMProduct.ProductName' + '\n' +
                    ' ,LibraryPIMProductContents.Quantity' + '\n' +
                    ' ,LibraryPIMProductContents.UnitMeasureID ' + '\n' +
                    ' ,IFNULL(LibraryCountriesLanguageLabel.Label, LibraryPIMProductContents.UnitMeasure) AS UnitMeasure' + '\n' +
                    ' FROM LibraryPIMProduct' + '\n' +
                    ' JOIN LibraryPIMProductContents ON LibraryPIMProductContents.ChildProductID = LibraryPIMProduct.ID' + '\n' +
                    ' LEFT JOIN LibraryPIMMeasure ON LibraryPIMMeasure.ID = LibraryPIMProductContents.UnitMeasureID' + '\n' +
                    ' LEFT JOIN LibraryCountriesLanguageLabel ON LibraryCountriesLanguageLabel.ID = LibraryPIMMeasure.MeasureNameID\n' +
                    ' WHERE LibraryPIMProductContents.ParentProductID = ' + ReadyString(vParams.productID) + '\n' +
                    ' AND LibraryPIMProductContents.OrganizationID = ' + ReadyString(Web.OrgID());
                    pDT = Sql.Execute(pSql);
                    if (pDT.Rows.length == 0)
                        vParams.error('jsQ7MY49C9YJ! Product, ' + vParams.productID + ', has no contents');
                    vParams.success(pDT);
                } catch (e) {
                    vParams.error(e.message);
                }

            };

            mOut.save = function (vParams) {
                /*
                vParams = {
                id: 'guid of private.TransBomMaker', // optional (blank for new)
                assignedToID: guid, // optional
                dateNeeded: date, // optional
                products: [ // optional
                    {
                        productID: 'guid of parent (finished) product',
                        qty: 'decimal',
                        unitMeasureID: 'guid'
                    }
                ],
                success: function(vData) {
                    vData = {
                        status: 'added', // or
                        status: 'updated'
                    };
                },
                error: function(vMessage) {
                    }
                };
                */

                var pIU;
                var pSql;
                var pID;
                var pFowstID;

                try {
                    pFowstID = sessionStorage.FowstID;
                    if (!IsGuid(pFowstID)) {
                        vParams.error('sessionStorage.FowstID must be GUID. jsA5X3SYB852!');
                        return;
                    }

                    (function SaveBomMaker() {
                        var pNum;
                        pIU = new InsertUpdate('TransBomMaker');
                        pIU.Add('FowstID', ReadyString(pFowstID));
                        pIU.Add('DeviceID', ReadyString(Web.DeviceID()));
                        pIU.Add('UserID', ReadyString(Web.PeopleUserID()));
                        pIU.Add('LastUpdated', proj.SqliteNow);

                        if (CStr2(vParams.assignedToID).trim() != '') {
                            if (IsGuid(vParams.assignedToID))
                                pIU.Add('AssignedToID', ReadyString(vParams.assignedToID));
                            else
                                throw 'js5Z6XEY9UK7! assignedToID must be a guid or null';
                        }

                        if (CStr2(vParams.dateNeeded).trim() != '') {
                            if (IsDate(vParams.dateNeeded))
                                pIU.Add('DateNeeded', ReadyDate(vParams.dateNeeded));
                            else
                                throw 'jsFUZZGXE3YQ! dateNeeded must be a date or null';
                        }

                        if (vParams.id == undefined) {
                            pNum = Business.TransBomMaker.VerifyCounter() + 1;
                            pID = NewID();
                            pIU.Add('ID', ReadyString(pID));
                            pIU.Add('Num', pNum);
                            pSql = pIU.GetInsert();
                            Business.TransBomMaker.UpdateCounter(pNum);
                        } else {
                            pID = vParams.id;
                            pSql = pIU.GetUpdate('ID = ' + ReadyString(pID));
                        }
                        Sql.Execute(pSql);
                    })(); // END (function SaveBomMaker() {

                    (function SaveBomMakerProducts() {
                        var pItem;
                        var pProductID;

                        if (vParams.products == null)
                            return;

                        for (var p = 0; p <= vParams.products.length - 1; p++) {
                            pItem = vParams.products[p];
                            pSql = 'DELETE FROM TransBomMakerContents\n' +
                            ' WHERE TransBomMakerContents.ID IN \n' +
                            '(SELECT TransBomMakerContents.ID\n' +
                            ' FROM TransBomMakerContents\n' +
                            ' JOIN TransBomMakerProducts ON TransBomMakerProducts.ID = TransBomMakerContents.ParentID\n' +
                            ' WHERE TransBomMakerProducts.ParentID = ' + ReadyString(pID) + '\n' +
                            ' AND TransBomMakerProducts.ProductID = ' + ReadyString(pItem.productID) + '\n' +
                            ' AND TransBomMakerProducts.UnitMeasureID = ' + ReadyString(pItem.unitMeasureID) + '\n' +
                            ')';
                            Sql.Execute(pSql);

                            pSql = 'DELETE FROM TransBomMakerProducts\n' +
                            ' WHERE ParentID = ' + ReadyString(pID) + '\n' +
                            ' AND ProductID = ' + ReadyString(pItem.productID) + '\n' +
                            ' AND UnitMeasureID = ' + ReadyString(pItem.unitMeasureID);
                            Sql.Execute(pSql);

                            pIU = new InsertUpdate('TransBomMakerProducts');
                            pProductID = NewID();
                            pIU.Add('ID', ReadyString(pProductID));
                            pIU.Add('ParentID', ReadyString(pID));
                            pIU.Add('ProductID', ReadyString(pItem.productID));
                            pIU.Add('Quantity', CDec2(pItem.qty));
                            pIU.Add('UnitMeasureID', ReadyString(pItem.unitMeasureID));
                            pSql = pIU.GetInsert();
                            Sql.Execute(pSql);

                            (function SaveBomMakerContents() {
                                mOut.getContents({
                                    productID: pItem.productID,
                                    success: function (vData) {
                                        SaveContent({
                                            parentProductID: pProductID,
                                            qty: pItem.qty,
                                            contents: vData.Rows
                                        });
                                    },
                                    error: function (vMessage) {
                                        console.log('jsJC8YUA2JLV ' + vMessage);
                                        debugger;
                                    }
                                });

                                function SaveContent(vParams) {
                                    /*
                                    vParams = {
                                    parentProductID: 'guid',
                                    qty: 'decimal qty of parent product',
                                    contents: [
                                    {
                                    }
                                    ]
                                    };
                                    */
                                    var pContent;
                                    var pConvertion;

                                    for (var p = 0; p <= vParams.contents.length - 1; p++) {
                                        pContent = vParams.contents[p];
                                        pIU = new InsertUpdate('TransBomMakerContents');
                                        pIU.Add('ID', ReadyString(NewID()));
                                        pIU.Add('ParentID', ReadyString(vParams.parentProductID));
                                        pIU.Add('ProductID', ReadyString(pContent.ChildProductID));
                                        pIU.Add('Quantity', CDec2(pContent.Quantity) * CDec2(vParams.qty)); // sub qty
                                        if (CStr2(pContent.UnitMeasureID).trim().length > 0) {
                                            // todo: save to BaseUnitMeasureID & QtyInBaseUnit
                                            pIU.Add('UnitMeasureID', ReadyString(pContent.UnitMeasureID));
                                        } else {
                                            pIU.Add('UnitMeasure', ReadyString(pContent.UnitMeasure));
                                            pConvertion = fromStandard(pContent.ChildProductID, pContent.UnitMeasure);
                                            if (pConvertion != undefined) {
                                                pIU.Add('BaseUnitMeasureID', ReadyString(pConvertion.UnitMeasureID));
                                                pIU.Add('QtyInBaseUnit', CDec2(pContent.Quantity) * CDec2(vParams.qty) * pConvertion.Multiplier);
                                            }
                                        }
                                        pSql = pIU.GetInsert();
                                        Sql.Execute(pSql);
                                    }
                                }
                            })(); // END (function SaveBomMakerContents() {
                        } // END for (var p = 0; p <= vParams.products.length; p++) {
                    })(); // END (function SaveBomMakerProducts() {

                    vParams.success({
                        Result: 'ok',
                        id: pID,
                        OrderID: pID
                    });
                } catch (e) {
                    vParams.error(e.message + ' jsLDQG6HU8Q3!');
                }

            }; // END mOut.save = function(vParams) {

            mOut.load = function (vParams) {
                /*
                vParams = {
                id: 'guid TransBomMaker.ID',
                success: function(vData) {
                },
                error: function(vMessage) {
                }
                };
                */

                var pDT;
                var pSql;

                try {
                    pSql = '  SELECT TransBomMaker.DateNeeded' + '\n' +
                    ' ,TransBomMaker.Num' + '\n' +
                    ' ,TransBomMaker.FowstID' + '\n' +
                    ' ,TransBomMaker.LastUpdated' + '\n' +
                    ' ,LibraryOrganizationFOWST.Name AS FowstName' + '\n' +
                    ' ,TransBomMaker.UserID' + '\n' +
                    ' ,IFNULL(peopleUser.Surname, \'\') ' + '\n' +
                    ' || \', \' || IFNULL(peopleUser.FirstName, \'\')' + '\n' +
                    ' || \', \' || IFNULL(peopleUser.MiddleName, \'\') AS peopleUserFullName' + '\n' +
                    ' ,TransBomMaker.AssignedToID' + '\n' +
                    ' ,IFNULL(peopleAssignedTo .Surname, \'\') ' + '\n' +
                    ' || \', \' || IFNULL(peopleAssignedTo .FirstName, \'\')' + '\n' +
                    ' || \', \' || IFNULL(peopleAssignedTo .MiddleName, \'\') AS peopleAssignedToFullName' + '\n' +
                    ' FROM private.TransBomMaker' + '\n' +
                    ' JOIN LibraryPeople peopleUser ON peopleUser.ID = TransBomMaker.UserID' + '\n' +
                    ' LEFT JOIN LibraryPeople peopleAssignedTo ON peopleAssignedTo.ID = TransBomMaker.AssignedToID' + '\n' +
                    ' LEFT JOIN LibraryOrganizationFOWST ON LibraryOrganizationFOWST.ID = TransBomMaker.FowstID\n' +
                    ' WHERE TransBomMaker.ID = ' + ReadyString(vParams.id);
                    pDT = Sql.Execute(pSql);
                } catch (e) {
                    vParams.error(e.message);
                    return;
                }

                vParams.success(pDT); // dont put vParams.success inside a try catch

            };

            mOut.loadProducts = function (vParams) {
                /*
                vParams = {
                    id: 'guid TransBomMaker.ID',
                    parentProductID: 'guid optional',
                    dontCallback: false, // if true, this function will return immediately with the results
                    success: function(vData) {
                    },
                    error: function(vMessage) {
                    }
                };
                */

                var pDT;
                var pSql;

                pSql = ' SELECT DISTINCT TransBomMakerProducts.ID\n' +
                ',TransBomMakerProducts.ProductID AS ParentProductID' + '\n' +
                ' ,LibraryPIMProduct_Products.ProductNo AS ParentProductNo' + '\n' +
                ' ,LibraryPIMProduct_Products.ProductName AS ParentProductName' + '\n' +
                ' ,TransBomMakerProducts.Quantity AS ParentQuantity' + '\n' +
                ' ,TransBomMakerProducts.UnitMeasureID  AS ParentUnitMeasureID' + '\n' +
                ' ,LibraryCountriesLanguageLabel_Products.Label AS ParentUnitMeasure' + '\n' +
                ' ,LibraryPIMProduct_Products.BomInstruction' + '\n' +
                ' FROM TransBomMakerProducts' + '\n' +
                ' JOIN LibraryPIMProduct LibraryPIMProduct_Products ON LibraryPIMProduct_Products.ID = TransBomMakerProducts.ProductID' + '\n' +
                ' JOIN LibraryPIMMeasure LibraryPIMMeasure_Products ON LibraryPIMMeasure_Products.ID = TransBomMakerProducts.UnitMeasureID' + '\n' +
                ' JOIN LibraryCountriesLanguageLabel LibraryCountriesLanguageLabel_Products ON LibraryCountriesLanguageLabel_Products.ID = LibraryPIMMeasure_Products.MeasureNameID' + '\n' +
                ' WHERE TransBomMakerProducts.ParentID = ' + ReadyString(vParams.id);
                if (vParams.parentProductID != undefined)
                    pSql += ' AND TransBomMakerProducts.ProductID = ' + ReadyString(vParams.parentProductID);
                pSql += ' ORDER BY LibraryPIMProduct_Products.ProductName';
                pDT = Sql.Execute(pSql);
                if (vParams.dontCallback)
                    return pDT;

                vParams.success(pDT);
            };

            mOut.loadGroups = function (vParams) {
                /*
                vParams = {
                id: 'guid TransBomMaker.ID',
                success: function(vData) {
                },
                error: function(vMessage) {
                }
                };
                */

                var pDT;
                var pSql;

                pSql = ' SELECT DISTINCT TransBomMakerProducts.ProductID AS ParentProductID' + '\n' +
                ' ,LibraryPIMProduct_Products.ProductNo AS ParentProductNo' + '\n' +
                ' ,LibraryPIMProduct_Products.ProductName AS ParentProductName' + '\n' +
                ' ,TransBomMakerProducts.Quantity AS ParentQuantity' + '\n' +
                ' ,TransBomMakerProducts.UnitMeasureID  AS ParentUnitMeasureID' + '\n' +
                ' ,LibraryCountriesLanguageLabel_Products.Label AS ParentUnitMeasure' + '\n' +
                ' ,TransBomMakerContents.ProductID AS ContentProductID' + '\n' +
                ' ,LibraryPIMProduct_Contents.ProductNo AS ContentProductNo' + '\n' +
                ' ,LibraryPIMProduct_Contents.ProductName AS ContentProductName' + '\n' +
                ' ,TransBomMakerContents.Quantity AS ContentQuantity' + '\n' +
                ' ,TransBomMakerContents.UnitMeasureID  AS ContentUnitMeasureID' + '\n' +
                ' ,IFNULL(LibraryCountriesLanguageLabel_Contents.Label, TransBomMakerContents.UnitMeasure) AS ContentUnitMeasure' + '\n' +
                ' FROM TransBomMakerProducts' + '\n' +
                ' JOIN LibraryPIMProduct LibraryPIMProduct_Products ON LibraryPIMProduct_Products.ID = TransBomMakerProducts.ProductID' + '\n' +
                ' LEFT JOIN LibraryPIMMeasure LibraryPIMMeasure_Products ON LibraryPIMMeasure_Products.ID = TransBomMakerProducts.UnitMeasureID' + '\n' +
                ' LEFT JOIN LibraryCountriesLanguageLabel LibraryCountriesLanguageLabel_Products ON LibraryCountriesLanguageLabel_Products.ID = LibraryPIMMeasure_Products.MeasureNameID' + '\n' +
                ' LEFT JOIN TransBomMakerContents ON TransBomMakerContents.ParentID = TransBomMakerProducts.ID' + '\n' +
                ' LEFT JOIN LibraryPIMProduct LibraryPIMProduct_Contents ON LibraryPIMProduct_Contents.ID = TransBomMakerContents.ProductID' + '\n' +
                ' LEFT JOIN LibraryPIMMeasure LibraryPIMMeasure_Contents ON LibraryPIMMeasure_Contents.ID = TransBomMakerContents.UnitMeasureID' + '\n' +
                ' LEFT JOIN LibraryCountriesLanguageLabel LibraryCountriesLanguageLabel_Contents ON LibraryCountriesLanguageLabel_Contents.ID = LibraryPIMMeasure_Contents.MeasureNameID\n' +
                ' WHERE TransBomMakerProducts.ParentID = ' + ReadyString(vParams.id);
                pDT = Sql.Execute(pSql);

                vParams.success(pDT);
            };

            mOut.deleteProduct = function (vParams) {
                /*
                vParams = {
                makerProductID: 'guid TransBomMakerProduct.ID',
                success: function(vData) {
                },
                error: function(vMessage) {
                }
                };
                */

                var pSql;
                pSql = 'DELETE FROM TransBomMakerContents\n' +
                ' WHERE TransBomMakerContents.ID IN \n' +
                '(SELECT TransBomMakerContents.ID\n' +
                ' FROM TransBomMakerContents\n' +
                ' JOIN TransBomMakerProducts ON TransBomMakerProducts.ID = TransBomMakerContents.ParentID\n' +
                ' WHERE TransBomMakerProducts.ID = ' + ReadyString(vParams.makerProductID) + '\n' +
                ')';
                Sql.Execute(pSql);

                pSql = 'DELETE FROM TransBomMakerProducts\n' +
                ' WHERE ID = ' + ReadyString(vParams.makerProductID);
                Sql.Execute(pSql);

                vParams.success({});
            };

            mOut.ticket = function (vParams) {
                /*
                vParams = {
                id: 'guid TransBomMaker.ID',
                success: function(vData) {
                },
                error: function(vMessage) {
                }
                };
                */

                var pDrHeader;
                var pDtParents;
                var pDtContents;

                mOut.load({
                    id: vParams.id,
                    success: function (vData) {
                        if (vData.Rows.length == 0) {
                            vParams.success('No record found');
                            return;
                        }

                        pDrHeader = vData.Rows[0];
                        loadProducts();
                    }, // END success: function(vData) {
                    error: vParams.error
                }); // END mOut.load({

                function loadProducts() {
                    mOut.loadProducts({
                        id: vParams.id,
                        success: function (vData) {
                            pDtParents = vData;

                            mOut.loadGroups({
                                id: vParams.id,
                                success: function (vData) {
                                    var pOut;

                                    pDtContents = vData;

                                    try {
                                        for (var pParentIndex = 0; pParentIndex <= pDtParents.Rows.length - 1; pParentIndex++) {
                                            pDtParents.Rows[pParentIndex].Contents = [];
                                            for (var pContentIndex = 0; pContentIndex <= pDtContents.Rows.length - 1; pContentIndex++) {
                                                if (pDtContents.Rows[pContentIndex].ParentProductID == pDtParents.Rows[pParentIndex].ParentProductID)
                                                    pDtParents.Rows[pParentIndex].Contents.push(pDtContents.Rows[pContentIndex]);
                                            } // END for (var pContentIndex = 0; pContentIndex <= pDtContents.Rows.length - 1; pContentIndex) {
                                        } // END for (var pParentIndex = 0; pParentIndex <= pDtParents.Row.length - 1; pParentIndex++) {

                                        pOut = CreatePrintOut(pDtParents);
                                        vParams.success(pOut);
                                    } catch (e) {
                                        vParams.error(e.message);
                                    }
                                },
                                error: vParams.error
                            });
                        },
                        error: vParams.error
                    }); // END mOut.loadProducts({
                } // END function loadProducts() {

                function CreatePrintOut(vData) {
                    var arrRes = {};
                    var pOut = '';
                    var pDrParent;
                    var pDrContent;
                    var pDrUser;

                    arrRes.Header = [pDrHeader];
                    arrRes.Body = [pDtParents];
                    return arrRes;
                    //                pDrUser = Business.LibraryPeople.ByID(Web.PeopleUserID());
                    //                pOut += 'Product Maker Ticket\n';
                    //                pOut += '\n';
                    //                pOut += 'Date/Time Issued: ' + $.format.date(new Date(pDrHeader.LastUpdated), proj.DateFormat) + '\n';
                    //                pOut += 'PMT Issued By: ' + pDrHeader.peopleUserFullName + '\n';
                    //                pOut += 'PMT Assigned To: ' + pDrHeader.peopleAssignedToFullName + '\n';
                    //                pOut += '\n';
                    //                pOut += 'Date/Time Needed: ';
                    //                if (IsDate(pDrHeader.DateNeeded))
                    //                    pOut += $.format.date(new Date(pDrHeader.DateNeeded), proj.DateFormat);
                    //                else
                    //                    pOut += ''; // not specified
                    //                pOut += '\n';
                    //                pOut += '---------------------\n';

                    //                for (var pParentIndex = 0; pParentIndex <= pDtParents.Rows.length - 1; pParentIndex++) {
                    //                    pOut += '\nPlease Make:\n';
                    //                    pDrParent = pDtParents.Rows[pParentIndex];
                    //                    pOut += pDrParent.ParentQuantity + ' ' + pDrParent.ParentUnitMeasure + ' ' + pDrParent.ParentProductName + '\n';
                    //                    pOut += pDrParent.ParentProductNo + '\n';
                    //                    pOut += '---------------------\n';

                    //                    pOut += 'Bill of Materials\n';
                    //                    for (var pContentIndex = 0; pContentIndex <= pDrParent.Contents.length - 1; pContentIndex++) {
                    //                        pDrContent = pDrParent.Contents[pContentIndex];
                    //                        pOut += pDrContent.ContentQuantity + ' ' + pDrContent.ContentUnitMeasure + ' ' + pDrContent.ContentProductName + ' ' + pDrContent.ContentProductNo + '\n';
                    //                    } // END for (var pContentIndex = 0; pContentIndex <= pDrParent.Contents.length - 1; pContentIndex++) {

                    //                    pOut += '\n';
                    //                    pOut += 'Instructions:\n';
                    //                    pOut += HtmlEncode(pDrParent.BomInstruction);
                    //                } // END for (var pParentIndex = 0; pParentIndex <= pDtParents.Row.length - 1; pParentIndex++) {
                    //              return pout;
                } // END function CreatePrintOut(vData) {

            }; // END mOut.ticket = function(vParams) {

            // this function mimics SalesMenu_offline.PosCart
            mOut.bomCart = function(vParams) {
                var pOut = {};
                pOut.Master = {
                    ID: vParams.id
                };
                pOut.Units = mOut.loadProducts({
                    id: vParams.id,
                    dontCallback: true
                });
                for (var p = 0; p <= pOut.Units.Rows.length - 1; p++) {
                    var pDR = pOut.Units.Rows[p];
                    pDR.ProductID = pDR.ParentProductID;
                    pDR.ProductNo = pDR.ParentProductNo;
                    pDR.ProductName = pDR.ParentProductName;
                    pDR.Quantity = pDR.ParentQuantity;
                    pDR.UnitMeasure = pDR.ParentUnitMeasure;
                }
                pOut.Stocks = JSON.parse(JSON.stringify(pOut.Units)); // clone JS objects; http://heyjavascript.com/4-creative-ways-to-clone-objects/
                pOut.Stocks.Rows = GetDistinct(pOut.Stocks.Rows, ['ProductID', 'ProductNo', 'ProductName']); // new attempt
                
                return pOut;
            };

            function fromStandard(vProductID, vStandardUnitMeasure) {

                var pSql;
                var pOrgID = Web.OrgID();
                var pOut;
                var pDT;

                pSql = ' SELECT LibraryPIMMeasureDefinition.SourceMeasureID' + '\n' +
                '  ,LibraryPIMMeasureDefinition.Quantity' + '\n' +
                '  FROM LibraryPIMMeasureDefinition' + '\n' +
                ' WHERE LibraryPIMMeasureDefinition.ProductID = ' + ReadyString(vProductID) + '\n' +
                '  AND LibraryPIMMeasureDefinition.DestinationMeasure = ' + ReadyString(vStandardUnitMeasure) + '\n' +
                '  AND LibraryPIMMeasureDefinition.OrganizationID = ' + ReadyString(pOrgID);
                pDT = Sql.Execute(pSql);
                if (pDT.Rows.length > 0) {
                    // unit does not need convertion
                    pOut = {
                        UnitMeasureID: pDT.Rows[0].SourceMeasureID,
                        Multiplier: 1 / pDT.Rows[0].Quantity
                    };
                    return pOut;
                }

                // unit needs convertion
                pSql = ' SELECT LibraryPIMMeasureDefinition.SourceMeasureID' + '\n' +
                ' ,LibraryPIMMeasureDefinition.Quantity' + '\n' +
                ' ,LibraryPIMMeasureConversion.ConversionFactor' + '\n' +
                ' FROM LibraryPIMMeasureDefinition' + '\n' +
                ' JOIN LibraryPIMMeasureConversion ' + '\n' +
                ' ON LibraryPIMMeasureConversion.DestMeasure = LibraryPIMMeasureDefinition.DestinationMeasure' + '\n' +
                ' WHERE LibraryPIMMeasureDefinition.ProductID = ' + ReadyString(vProductID) + '\n' +
                ' AND LibraryPIMMeasureConversion.BaseMeasure = ' + ReadyString(vStandardUnitMeasure) + '\n' +
                ' AND LibraryPIMMeasureDefinition.OrganizationID = ' + ReadyString(pOrgID);
                pDT = Sql.Execute(pSql);
                if (pDT.Rows.length == 0) {
                    debugger;
                    return;
                }
                pOut = {
                    UnitMeasureID: pDT.Rows[0].SourceMeasureID,
                    Multiplier: pDT.Rows[0].ConversionFactor / pDT.Rows[0].Quantity
                };

                return pOut;

            }

            return mOut;
        });

    }); 