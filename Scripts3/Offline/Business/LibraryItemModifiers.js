define([], function() {
    window.Business.LibraryItemModifiers = new function(vParent) {
        
        var mThis = this;

        mThis.HasModifier = function(vProductID) {

            var pSql;
            var pDT;

            pSql = 'SELECT 1 FROM (' + ModifierSql(vProductID) + ') LIMIT 1';
            pDT = Sql.Execute(pSql);

            return (pDT.Rows.length > 0);
        };

        function ModifierSql(vProductID) {
            var pSql;
            var pProductIds = '';
            var pUniqueProductIds = [];

            if ($.isArray(vProductID)) {
                for (var pIndex = 0; pIndex <= vProductID.length - 1; pIndex++) {
                    var pProductID = vProductID[pIndex];
                    if (pUniqueProductIds.indexOf(pProductID) != -1)
                        continue;
                    pProductID = ReadyGuid(pProductID);
                    pUniqueProductIds.push(pProductID);
                    if (pProductIds != '')
                        pProductIds += ', ';
                    pProductIds += ReadyString(pProductID);
                }
            } else {
                pProductIds = ReadyString(ReadyGuid(vProductID));
            }

            pSql = ' SELECT DISTINCT LibraryItemModifierCategories.ItemModifierID, LibraryItemModifierCategories.CategoryName' + '\n' +
                ' ,LibraryItemModifierCategories.ProductID AS ProductID1' + '\n' + 
                ' ,LibraryPIMProductVariableFeatures001.ProductID AS ProductID2' + '\n' + 
                ' FROM private.LibraryItemModifierCategories' + '\n' + 
                ' LEFT JOIN LibraryPIMProductVariableFeatures001 ON LibraryItemModifierCategories.CategoryName = LibraryPIMProductVariableFeatures001.C0020' + '\n' + 
                ' WHERE LibraryPIMProductVariableFeatures001.ProductID IN (' + pProductIds + ')\n' +
                ' OR LibraryItemModifierCategories.ProductID IN (' + pProductIds + ')\n';

            return pSql;
        }

        mThis.ModifierSql = function (vProductID) {
            return ModifierSql(vProductID);
        }

        mThis.GetRebates = function(vProductID) {
            var pSql;
            var pDT;

            pSql = 'SELECT DISTINCT LibraryItemModifierRebates.ID\n' + 
                ',LibraryItemModifierRebates.ProductID\n' +
                ',LibraryItemModifierRebates.UnitMeasure\n' +
                ',LibraryItemModifierRebates.UnitQuantity\n' +
                ',LibraryItemModifierRebates.Quantity\n' + 
                ',LibraryItemModifierRebates.Amount\n' + 
                ',LibraryPIMProduct.ProductNo\n' + 
                ',LibraryPIMProduct.ProductName\n' + 
                ' FROM private.LibraryItemModifierRebates\n' +
                ' JOIN (' + ModifierSql(vProductID) + ') t ON t.ItemModifierID = LibraryItemModifierRebates.ItemModifierID\n' + 
                ' JOIN LibraryPIMProduct ON LibraryPIMProduct.ID = LibraryItemModifierRebates.ProductID';
            pDT = Sql.Execute(pSql);

            return pDT;

        };

        mThis.GetItemModifiers = function (categoryName, vProductID) {
            var pSql, subq, pDTItemModifier, arrItemModifier = [], currentDate = new Date(new Date().format('mmm dd, yyyy'));

            if (Business.SystemDefaults.GetEnforceZRead()) {
                currentDate = new Date(proj.GetZReadDate().format('mmm dd, yyyy'));
            }

            pSql = ' WHERE 1=0\n';
            if (CStr2(categoryName).trim() != '') {
                pSql += ' OR cat.CategoryName = ' + ReadyString(categoryName) + ' \n';
                subq = "(SELECT IFNULL(OrderNo, 0) FROM LibraryItemModifierSorting WHERE ItemModifierID = im.ID AND CategoryName =  " + ReadyString(categoryName) + ")"
            } else {
                subq = "(SELECT IFNULL(OrderNo, 0) FROM LibraryItemModifierSorting WHERE ItemModifierID = im.ID AND ProductID =  " + ReadyString(vProductID) + ")"
            }
                
            if (vProductID != null)                
                pSql += ' OR cat.ProductID = ' + ReadyString(vProductID) + ' \n';
                
            pSql = "SELECT * FROM (SELECT im.* \n" +
                    " ,im.IsRestrictedModifier \n" +
                    " ,IFNULL(" + subq + ", 100) as OrderNo \n" +
                    " FROM LibraryItemModifierCategories as cat \n" +
                    " INNER JOIN LibraryItemModifiers as im ON im.ID = cat.ItemModifierID \n" +
                    pSql +
                    " AND (im.Type <> '' OR im.Type <> 'Return For Rebates')) as T WHERE IFNULL(IsActive, 1) = 1 ORDER BY OrderNo ASC, Name ASC ";
            pDTItemModifier = Sql.Execute(pSql);

            for (var i = 0; i < pDTItemModifier.Rows.length; i++) {
                var imData = pDTItemModifier.Rows[i], arrItems;
                imData.ParentProductID = vProductID;

                pSql = "SELECT * FROM private.LibraryItemModifierAssignedFowst WHERE ItemModifierID = " + ReadyString(imData.ID)
                var fowstDT = Sql.Execute(pSql).Rows, fowstincludeIM = true;

                for (var f = 0; f < fowstDT.length; f++) {
                    if (fowstDT[f].FowstID.toLowerCase() == sessionStorage.FowstID.toLowerCase()) {
                        fowstincludeIM = true;
                        break
                    } else {
                        fowstincludeIM = false;
                    }
                }

                if (!fowstincludeIM) {
                    continue;
                }

                pSql = "SELECT * FROM private.LibraryItemModifierAssignedCustomer WHERE ItemModifierID = " + ReadyString(imData.ID)
                var cusDT = Sql.Execute(pSql).Rows, cusincludeIM = true;

                for (var f = 0; f < cusDT.length; f++) {
                    if (cusDT[f].CustomerOrgID.toLowerCase() == sessionStorage.CustomerOrganizationID.toLowerCase()) {
                        cusincludeIM = true;
                        break
                    } else {
                        cusincludeIM = false;
                    }
                }

                if (!cusincludeIM) {
                    continue;
                }

                if (CStr2(imData.DateFrom) != '' && CStr2(imData.DateTo) != '') {
                    if (currentDate >= new Date(new Date(imData.DateFrom).format('mmm dd, yyyy')) && currentDate <= new Date(new Date(imData.DateTo).format('mmm dd, yyyy'))) {
                        //show
                    } else {
                        continue; // hide
                    }
                } else {
                    if (!IsAllowedNow(imData)) {
                        var msg = '';
                        var arrDay = ['Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday', 'Sunday'];
                        for (var d = 0; d < arrDay.length; d++) {
                            if (pDTItemModifier.Rows[i][arrDay[d] + 'TimeFrom'] != undefined) {
                                msg += '<br> - ' + arrDay[d] + ' at ' + pDTItemModifier.Rows[i][arrDay[d] + 'TimeFrom'] + ' to ' + pDTItemModifier.Rows[i][arrDay[d] + 'TimeTo']
                            }
                        }
                        pDTItemModifier.Rows[i].NotAvailableThisTime = true;
                        pDTItemModifier.Rows[i].Message = 'The availability of this modifier is every: <br>' + msg;
                        arrItemModifier.push(pDTItemModifier.Rows[i]);
                        continue;
                    }
                }                                    

                if (CStr2(imData.ChoicesReference).toLowerCase() == 'buy1take1') {
                    imData.Max = imData.PromoTakeQty;
                    arrItems = GetBuyTake(imData);
                } else if (CStr2(imData.Type) == '' || CStr2(imData.Type) == 'Return For Rebates') {
                    arrItems = mThis.GetNewRebates(imData.ID).Rows;
                } else if (CStr2(imData.Type) == 'Inventory Affecting') {
                    arrItems = mThis.GetInventoryAffecting(imData.ID).Rows;
                } else if (CStr2(imData.Type) == 'Non-Inventory Affecting') {
                    arrItems = mThis.GetNonInventoryAffecting(imData.ID).Rows;
                }

                pDTItemModifier.Rows[i].Items = arrItems;
                arrItemModifier.push(pDTItemModifier.Rows[i]);
            }

            return arrItemModifier;
        }

        function IsAllowedNow(vImData) {
            if (!CBool(vImData.IsRestrictedModifier)) 
                return true;

            var pCurrentDateTime = new Date();
            var pWeekDay = $.format.date(pCurrentDateTime, 'EEEE');
            var pFieldNameFrom = pWeekDay + 'TimeFrom';
            var pFieldNameTo = pWeekDay + 'TimeTo';
            if (vImData[pFieldNameFrom] == null)
                return false;
            if (vImData[pFieldNameTo] == null)
                return false;

            var pSchedFrom = ParseDate('1/1/1 ' + vImData[pFieldNameFrom]);
            if (pSchedFrom == null) 
                console.error(pFieldNameFrom + ' must be a valid time. Value is ' + vImData[pFieldNameFrom] + '. ref: jsP3ECBNFYMH!');

            var pSchedTo = ParseDate('1/1/1 ' + vImData[pFieldNameTo]);
            if (pSchedTo == null) 
                console.error(pSchedTo + ' must be a valid time. Value is ' + vImData[pSchedTo] + '. ref: jsGXPJHWAQZC!');

            pSchedFrom = RemoveTime(pCurrentDateTime).getTime() + pSchedFrom.getTime() - ParseDate('1/1/1').getTime();
            pSchedFrom = new Date(pSchedFrom);

            pSchedTo = RemoveTime(pCurrentDateTime).getTime() + pSchedTo.getTime() - ParseDate('1/1/1').getTime();
            pSchedTo = new Date(pSchedTo);

            if ((pSchedFrom <= pCurrentDateTime)
                && (pCurrentDateTime <= pSchedTo)) {

                return true;
            }

            return false;
        }

        function GetBuyTake(vItemModifierData) {
            vItemModifierData.PromoBuyQty = CDec2(vItemModifierData.PromoBuyQty);
            if (CDec2(vItemModifierData.PromoBuyQty) <= 0)
                vItemModifierData.PromoBuyQty = 1;

            var pData = {
                ID: NewID(),
                ItemModifierID: vItemModifierData.ID,
                ProductID: vItemModifierData.ParentProductID,
                Description: '',
                Max: vItemModifierData.PromoTakeQty,
                UnitMeasureID: '',
                UnitMeasure: '',
                UnitPrice: '',
                ProductName: 'Buy ' + vItemModifierData.PromoBuyQty + ' Take ' + vItemModifierData.PromoTakeQty
            };
            
            if (CDec2(vItemModifierData.PromoTakePrice) > 0)
                pData.UnitPrice = vItemModifierData.PromoTakePrice;
            else {
                var pRetailPrice = Business.LibraryPIMProduct.GetRetailPrice(vItemModifierData.ParentProductID, sessionStorage.OrgID);
                pData.UnitPrice = pRetailPrice * vItemModifierData.PromoTakeDiscount / 100;
            }
            var pUnit = Business.LibraryPIMProduct.BaseUnit(vItemModifierData.ParentProductID);
            pData.UnitMeasureID = pUnit.UnitMeasureID;
            pData.UnitMeasure = pUnit.UnitMeasure;
            pData = [pData];

            return pData;
        }

        mThis.GetModifiersOfItem = function (vData) {
            if(vData.length > 0){
                for (var i = 0; i < vData.length; i++){
                    var pSql = "select CASE WHEN nia.Description = 'Others' THEN mod.OtherDescription \n" +
                                " ELSE IFNULL(pim.ProductName, nia.Description) \n " +
                                " END as Description \n" +
                                ", mod.OrderedQty \n" +
                                ", mod.ModifierPrice \n" +
                                ", mod.PremiumPrice \n" +
                                ", mod.SubTotal \n" +
                                ", ia.UnitMeasure \n" +
                                " from transinvoiceitemsmodifier as mod \n" +
                                " left join libraryitemmodifierinventoryaffecting as ia on mod.modifierid = ia.id \n" +
                                " left join librarypimproduct as pim on pim.ID = ia.productid \n" +
                                " left join libraryitemmodifiernoninventoryaffecting as nia on nia.id = mod.modifierid \n" +
                                " where mod.invoiceitemid = " + ReadyString(vData[i].ItemID);
                    vData[i].ItemModifier = Sql.Execute(pSql).Rows;
                }
            }
        }

        mThis.GetNonInventoryAffecting = function (id) {
            var pSql;

            pSql = "SELECT nia.* \n" +
                    " FROM LibraryItemModifierNonInventoryAffecting as nia \n" +
                    " WHERE nia.ItemModifierID = " + ReadyString(id) +
                    " ORDER BY OrderNo ASC "

            return Sql.Execute(pSql);
        }

        mThis.GetInventoryAffecting = function (id) {
            var pSql;

            pSql = "SELECT ia.*, prod.ProductName \n" +
                    " FROM LibraryItemModifierInventoryAffecting as ia \n" +
                    " INNER JOIN LibraryPIMProduct as prod ON prod.ID = ia.ProductID \n" +
                    " WHERE ia.ItemModifierID = " + ReadyString(id) +
                    " AND ia.ID NOT IN (SELECT InventoryAffectingID FROM LibraryItemModifierInventoryAffectingFowst WHERE FowstID = " + ReadyString(sessionStorage.FowstID.toLowerCase()) + ")" +
                    " ORDER BY CASE WHEN IFNULL(ia.PremiumPrice, 0) > 0 THEN 1 ELSE 0 END ASC, IFNULL(ia.OrderNo, 0) ASC, ia.Description ASC "

            return Sql.Execute(pSql);
        }

        mThis.GetNewRebates = function (id) {
            var pSql;

            pSql = "SELECT reb.*, prod.ProductName \n" +
                    " FROM LibraryItemModifierRebates as reb \n" +
                    " INNER JOIN LibraryPIMProduct as prod ON prod.ID = reb.ProductID \n" +
                    " WHERE reb.ItemModifierID = " + ReadyString(id)

            return Sql.Execute(pSql);
        }

        return this;
    };
});