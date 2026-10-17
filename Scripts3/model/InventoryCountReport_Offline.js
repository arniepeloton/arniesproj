define(['common'
    ], function () {
        CreateStringObject('App.model.InventoryCountReport_Offline', null, function () {
            var pOut = {};

            pOut.GetInventoryCountReport = function (data) {
                var pSql = '';
                var pFinder = new Finder();
                var pFields = pFinder.Fields;
                var pField;
                var dateNow = new Date();
                var fromDate = '';
                var toDate = '';

                pField = pFields.Add('ProductNo');
                pField.Expression = 'LibraryPIMProduct.ProductNo';

                pField = pFields.Add('ProductGroupNo');
                pField.Expression = 'LibraryPIMProduct.ProductGroupNo';

                pField = pFields.Add('ProductName');
                pField.Expression = 'LibraryPIMProduct.ProductName';

                pField = pFields.Add('UnitMeasure');
                pField.Expression = 'LibraryCountriesLanguageLabel.Label';

                pField = pFields.Add('CountOnRecord');
                pField.Expression = 'TransInventoryProductCountStock.ExpectedValue';

                pField = pFields.Add('PhysicalCount');
                pField.Expression = 'TransInventoryProductCountStock.Quantity * TransInventoryProductCountStock.ActualQuantity';

                pField = pFields.Add('CountDate');
                pField.Expression = 'TransInventoryProductCount.LastUpdated';

                pField = pFields.Add('Fowst');
                pField.Expression = 'LibraryOrganizationFOWST.Name';

                pField = pFields.Add('Absolute');
                pField.Expression = 'TransInventoryProductCountStock.Variance';

                pField = pFields.Add('CountedBy');
                pField.Expression = 'ppl1.Surname || \', \' || ppl1.FirstName';

                pField = pFields.Add('TurnedOverTo');
                pField.Expression = 'ppl2.Surname || \', \' || ppl2.FirstName';

                pField = pFields.Add('TurnOverDate');
                pField.Expression = 'TransInventoryProductCountStock.TurnOverDate';

                pField = pFields.Add('Remarks');
                pField.Expression = 'TransInventoryProductCountStock.Remarks';

                pField = pFields.Add("Percentage")
                pField.Expression = 'CASE WHEN CAST(TransInventoryProductCountStock.ExpectedValue  AS float) * 100 = 0.00 THEN 0 ELSE (TransInventoryProductCountStock.Variance / CAST( TransInventoryProductCountStock.ExpectedValue AS float)* 100) END'
                
                pField = pFields.Add("Category")
                pField.Expression = 'LibraryPIMProductVariableFeatures001.C0020';

                pField = pFields.Add('ProductID');
                pField.Expression = 'LibraryPIMProduct.ID';
                
                pField = pFields.Add("Value")
                pField.Expression = 'ABS(TransInventoryProductCountStock.Variance) * (SELECT IFNULL(Price,0) AS Price ' +
                                    'FROM LibraryPIMProductPriceLevel  ' +
                                    'WHERE LibraryPIMProductPriceLevel.PriceLevelID = \'01000012-0003-0000-0000-000000000000\'' +
                                    'AND LibraryPIMProductPriceLevel.ProductID =  LibraryPIMProduct.ID  ' +
                                    'AND LibraryPIMProductPriceLevel.OrganizationID =  '+ ReadyString(sessionStorage.OrgID) +' limit 1) ' 

                pFinder.SqlFrom = 'FROM TransInventoryProductCount ' +
                                    'INNER JOIN TransInventoryProductCountStock ' +
                                    '   ON TransInventoryProductCount.ID = TransInventoryProductCountStock.InventoryProductCountID ' +
                                    'INNER JOIN LibraryOrganizationFOWST ' +
                                    '   ON TransInventoryProductCount.FowstID = LibraryOrganizationFOWST.ID ' +
                                    'INNER JOIN LibraryPeople ppl1' +
                                    '   ON TransInventoryProductCount.UserID = ppl1.ID ' +
                                    'left JOIN LibraryPeople ppl2' +
                                    '   ON TransInventoryProductCountStock.TurnOverToID = ppl2.ID ' +
                                    'INNER JOIN LibraryPIMProduct ' +
                                    '   ON TransInventoryProductCount.ProductID = LibraryPIMProduct.ID ' +
                                    'INNER JOIN LibraryPIMProductMeasurement ' +
                                    '   ON LibraryPIMProduct.ID = LibraryPIMProductMeasurement.ProductID ' +
                                    'INNER JOIN LibraryPIMMeasure ' +
                                    '   ON LibraryPIMProductMeasurement.UnitMeasureID = LibraryPIMMeasure.ID ' +
                                    'INNER JOIN LibraryCountriesLanguageLabel ' +
                                    '   ON LibraryPIMMeasure.MeasureNameID = LibraryCountriesLanguageLabel.ID ' +
                                    'INNER JOIN  LibraryPIMProductVariableFeatures001' +
                                    '   ON TransInventoryProductCount.ProductID = LibraryPIMProductVariableFeatures001.ProductID ';

                switch (data.Within) {
                    case "optThisWeek":
                        fromDate = $.format.date(new Date(dateNow.setDate(dateNow.getDate() - dateNow.getDay())), 'yyyy-MM-dd');
                        toDate = $.format.date(new Date(), 'yyyy-MM-dd');
                        console.log('Week to Date: ' + fromDate + ' to ' + toDate);
                        break;

                    case "optLastWeek":
                        var dateLastWeek = new Date(dateNow.getFullYear(), dateNow.getMonth(), dateNow.getDate() - 7);
                        fromDate = $.format.date(new Date(dateNow.setDate(dateLastWeek.getDate() - dateLastWeek.getDay())), 'yyyy-MM-dd');
                        toDate = $.format.date(new Date(dateNow.setDate((dateLastWeek.getDate() - dateLastWeek.getDay()) + 6)), 'yyyy-MM-dd');
                        console.log('Last Week: ' + fromDate + ' to ' + toDate);
                        break;

                    case "optLastTwoWeeks":
                        var dateLastTwoWeek = new Date(dateNow.getFullYear(), dateNow.getMonth(), dateNow.getDate() - 14);
                        fromDate = $.format.date(new Date(dateLastTwoWeek.getFullYear(), dateLastTwoWeek.getMonth(), dateLastTwoWeek.getDate() - dateLastTwoWeek.getDay()), 'yyyy-MM-dd');
                        toDate = $.format.date(new Date(dateLastTwoWeek.setDate((dateLastTwoWeek.getDate() - dateLastTwoWeek.getDay()) + 6)), 'yyyy-MM-dd');
                        //toDate = new Date(new Date(toDate).setDate(new Date(toDate).getMonth() - (dateLastTwoWeek.getMonth() != new Date(toDate).getMonth() ? 1 : 0), (dateLastTwoWeek.getDate() - dateLastTwoWeek.getDay()) + 6));
                        console.log('2 Weeks Ago: ' + fromDate + ' to ' + toDate);
                        break;

                    case "optLastThreeWeeks":
                        var dateLastThreeWeek = new Date(dateNow.getFullYear(), dateNow.getMonth(), dateNow.getDate() - 21);
                        fromDate = $.format.date(new Date(dateLastThreeWeek.getFullYear(), dateLastThreeWeek.getMonth(), dateLastThreeWeek.getDate() - dateLastThreeWeek.getDay()), 'yyyy-MM-dd');
                        toDate = $.format.date(new Date(dateLastThreeWeek.setDate((dateLastThreeWeek.getDate() - dateLastThreeWeek.getDay()) + 6)), 'yyyy-MM-dd');
                        //toDate = new Date(new Date(toDate).setDate(new Date(toDate).getMonth() - (dateLastTwoWeek.getMonth() != new Date(toDate).getMonth() ? 1 : 0), (dateLastTwoWeek.getDate() - dateLastTwoWeek.getDay()) + 6));
                        console.log('2 Weeks Ago: ' + fromDate + ' to ' + toDate);
                        break;

                    case "optLastMonth":
                        var dateLastMonth = new Date(dateNow.getFullYear(), dateNow.getMonth(), 1);
                        fromDate = $.format.date(new Date(dateNow.getFullYear(), (dateNow.getMonth() - 1), 1), 'yyyy-MM-dd');
                        toDate = $.format.date(new Date(dateLastMonth.setDate(dateLastMonth.getDate() - 1)), 'yyyy-MM-dd');
                        //toDate = new Date(new Date(toDate).setDate(new Date(toDate).getMonth() - (dateLastTwoWeek.getMonth() != new Date(toDate).getMonth() ? 1 : 0), (dateLastTwoWeek.getDate() - dateLastTwoWeek.getDay()) + 6));
                        console.log('2 Weeks Ago: ' + fromDate + ' to ' + toDate);
                        break;
                }

                pFinder.SqlWhere = ' WHERE LibraryOrganizationFOWST.ID = ' + ReadyString(data.FowstID) +
                                    ' AND date(TransInventoryProductCount.LastUpdated) Between ' + ReadyString(fromDate) + ' AND ' + ReadyString(toDate)

                return pFinder.Requery();
            }

            pOut.GetInventoryCountForApprovalHistory = function (data) {
                var pSql = '';
                var pFinder = new Finder();
                var pFields = pFinder.Fields;
                var pField;
                var dateNow = new Date();
                var fromDate = '';
                var toDate = '';

                pField = pFields.Add('ID');
                pField.Expression = 'private.TransInventoryProductCountStock.ID';

                pField = pFields.Add('CountDate');
                pField.Expression = 'private.TransInventoryProductCount.InventoryCountDate';

                pField = pFields.Add('ProductName');
                pField.Expression = 'LibraryPIMProduct.ProductName';

                pField = pFields.Add('BinID');
                pField.Expression = 'private.TransInventoryProductCountStock.BinID';

                pField = pFields.Add('Quantity');
                pField.Expression = 'private.TransInventoryProductCountStock.Quantity';

                pField = pFields.Add('FOWST');
                pField.Expression = 'LibraryOrganizationFOWST.name';

                pField = pFields.Add('Bin');
                pField.Expression = 'LibraryFowstBin.Name';

                pField = pFields.Add('Variance');
                pField.Expression = 'private.TransInventoryProductCountStock.Variance';

                pField = pFields.Add('UnitMeasure');
                pField.Expression = 'LibraryCountriesLanguageLabel.Label';

                pField = pFields.Add('ModifiedBy');
                pField.Expression = 'LibraryPeople.FirstName + \' \' + LibraryPeople.MiddleName + \' \' + LibraryPeople.Surname';

                pField = pFields.Add('ModifiedDate');
                pField.Expression = 'private.TransInventoryProductCountStock.VarianceApprovedDate';

                pField = pFields.Add('Status');
                pField.Expression = 'CASE WHEN private.TransInventoryProductCountStock.IsVarianceApproved = 1 THEN \'Approved\' ELSE \'Disapproved\' END';

                pFinder.SqlFrom = 'FROM private.TransInventoryProductCountStock ' +
                'INNER JOIN private.TransInventoryProductCount on private.TransInventoryProductCountStock.InventoryProductCountID=private.TransInventoryProductCount.ID  ' +
                'INNER JOIN LibraryPIMProductMeasurement ON TransInventoryProductCount.ProductID = LibraryPIMProductMeasurement.ProductID   ' +
                'INNER JOIN LibraryPIMMeasure ON LibraryPIMProductMeasurement.UnitMeasureID = LibraryPIMMeasure.ID   ' +
                'INNER JOIN LibraryCountriesLanguageLabel ON LibraryPIMMeasure.MeasureNameID = LibraryCountriesLanguageLabel.ID  ' +
                'INNER JOIN LibraryFowstBin on private.LibraryFowstBin.ID = private.TransInventoryProductCountStock.BinID  ' +
                'INNER JOIN LibraryOrganizationFOWST on LibraryOrganizationFOWST.ID = private.TransInventoryProductCount.FowstID ' +
                'LEFT JOIN LibraryPeople ON LibraryPeople.ID = private.TransInventoryProductCountStock.VarianceApprovedBy  ' +
                'LEFT JOIN LibraryPIMProduct ON LibraryPIMProduct.ID = private.TransInventoryProductCount.ProductID   ';

                pFinder.SqlWhere = ' Where private.TransInventoryProductCountStock.Variance <> 0 AND private.TransInventoryProductCountStock.IsActionTaken = 1 ' +
                                     'AND LibraryOrganizationFOWST.ID = ' + ReadyString(data.FowstID) 

                pFinder.OrderBy = 'ORDER BY private.TransInventoryProductCountStock.VarianceApprovedDate DESC'

                return pFinder.Requery();
            }

            return pOut;
        });
    })