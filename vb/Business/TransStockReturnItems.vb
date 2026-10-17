Public Class TransStockReturnItems
    Dim mProData As ProData
    Dim mParent As Business

    Sub New(ByRef rProData As ProData, ByVal vParent As Business)

        mProData = rProData
        mParent = vParent

    End Sub

    Public Function GetStocks(ByVal vID As String) As DataTable

        Dim pSql As String

        'pSql = "SELECT " & _
        '       "    TransStockReturns.ID, " & _
        '       "    TransStockReturns.LastUpdated, " & _
        '       "    TransStockReturns.UserID, " & _
        '       "    EmployeeProfiles.LastName + ', ' + " & _
        '       "    EmployeeProfiles.FirstName + ' ' + " & _
        '       "    EmployeeProfiles.MiddleName AS EmployeeName," & _
        '       "    TransStockReturns.BranchID, " & _
        '       "    LibraryBranch.Name, " & _
        '       "    TransStockReturnItems.StockID, " & _
        '       "    TransStockReturnItems.UnitMeasure, " & _
        '       "    TransStockReturnItems.ActualQuantity, " & _
        '       "    TransStockReturnItems.UnitPrice, " & _
        '       "    TransStockReturnItems.IsOverStock, " & _
        '       "    TransStockReturnItems.IsAboutToExpire, " & _
        '       "    TransStockReturnItems.ExpirationDate, " & _
        '       "    TransStockReturnItems.StockReturnID, " & _
        '       "    TransStockReturnItemReasons.ReturnItemsID, " & _
        '       "    TransStockReturnItemReasons.ReasonID, " & _
        '       "    LibraryStockReturnReasons.Reason " & _
        '       "FROM " & _
        '       "    LibraryBrands RIGHT JOIN ( " & _
        '       "        LibraryProducts RIGHT JOIN ( " & _
        '       "            LibraryStockReturnReasons RIGHT JOIN ( " & _
        '       "            TransStockReturnItemReasons RIGHT JOIN ( " & _
        '       "            LibraryStockInformation RIGHT JOIN ( " & _
        '       "                TransStockReturnItems RIGHT JOIN ( " & _
        '       "                    DavaoUnifiedPayroll.dbo.EmployeeProfiles EmployeeProfiles RIGHT JOIN ( " & _
        '       "                        DavaoUnifiedPayroll.dbo.LibraryBranch LibraryBranch RIGHT JOIN TransStockReturns " & _
        '       "                        ON LibraryBranch.ID = TransStockReturns.BranchID " & _
        '       "                    ) ON EmployeeProfiles.ID = TransStockReturns.UserID " & _
        '       "                ) ON TransStockReturns.ID = TransStockReturnItems.StockReturnID " & _
        '       "            ) ON LibraryStockInformation.ID = TransStockReturnItems.StockID " & _
        '       "            ) ON TransStockReturnItems.ID = TransStockReturnItemReasons.ReturnItemsID " & _
        '       "            ) ON LibraryStockReturnReasons.ID = TransStockReturnItemReasons.ReasonID " & _
        '       "        ) ON LibraryProducts.ID = LibraryStockInformation.ProductID " & _
        '       "    ) ON LibraryBrands.ID = LibraryStockInformation.BrandID " & _
        '       "GROUP BY " & _
        '       ""

        '"    DavaoUnifiedPayroll.dbo.EmployeeProfiles EmployeeProfiles RIGHT JOIN ( " & _
        '"        DavaoUnifiedPayroll.dbo.LibraryBranch LibraryBranch RIGHT JOIN ( " & _
        '"            LibraryStockInformation RIGHT JOIN ( " & _
        '"                TransStockReturnItems RIGHT JOIN TransStockReturns ON " & _
        '"                TransStockReturnItems.StockReturnID = TransStockReturns.ID " & _
        '"            ) ON LibraryStockInformation.ID = TransStockReturnItems.StockID " & _
        '"        ) ON LibraryBranch.ID = TransStockReturns.BranchID " & _
        '"    ) ON EmployeeProfiles.ID = TransStockReturns.UserID"


        pSql = "SELECT " & _
               "    TransStockReturnItems.ID, TransStockReturns.LastUpdated, " & _
               "    TransStockReturns.UserID, TransStockReturns.BranchID, " & _
               "    EmployeeProfiles.LastName + ', ' + " & _
               "    EmployeeProfiles.FirstName + ' ' + " & _
               "    EmployeeProfiles.MiddleName AS EmployeeName," & _
               "    LibraryBranch_1.Name AS BranchName, LibraryBranch_2.Name AS ReturnsTo, " & _
               "    TransStockReturns.Num, TransStockReturns.IsActive, " & _
               "    TransStockReturnItems.StockID, LibraryStockInformation.StockNumber, " & _
               "    ISNULL(LTRIM(RTRIM(LibraryBrands.BrandName)) + ', ', '') + " & _
               "    ISNULL(LTRIM(RTRIM(LibraryProducts.ProductName)) + ', ', '') + " & _
               "    ISNULL(LTRIM(RTRIM(LibraryStockInformation.Make)) + ', ', '') + " & _
               "    ISNULL(LTRIM(RTRIM(LibraryStockInformation.UnitSize)) + ', ', '') + " & _
               "    ISNULL(LTRIM(RTRIM(LibraryStockInformation.[Content])) + ', ', '') AS StockDescription, " & _
               "    TransStockReturnItems.UnitMeasure, TransStockReturnItems.ActualQuantity, " & _
               "    TransStockReturnItems.Quantity, TransStockReturnItems.UnitPrice, " & _
               "    TransStockReturnItems.IsOverStock, TransStockReturnItems.IsAboutToExpire, " & _
               "    TransStockReturnItems.ExpirationDate, TransStockReturnItems.StockReturnID, " & _
               "    dbo.Reason(" & ReadyString(vID) & ") AS Reason, TransStockReturnItems.UnitMeasure + " & _
               "    ' (' + CONVERT(varchar, TransStockReturnItems.ActualQuantity) + ')' AS UnitMeasure2 , " & _
               "    CASE " & _
               "        WHEN TransStockReturnItems.IsOverStock = 1 AND TransStockReturnItems.IsAboutToExpire = 1 THEN " & _
               "            'Overstock, About to Expire' " & _
               "        WHEN TransStockReturnItems.IsOverStock = 1 THEN " & _
               "            'Overstock' " & _
               "        WHEN TransStockReturnItems.IsAboutToExpire = 1 THEN " & _
               "            'About to Expire' " & _
               "        ELSE " & _
               "            dbo.Reason(TransStockReturnItems.ID) " & _
               "    END AS Reason2 " & _
               "FROM " & _
               "    LibraryBrands RIGHT OUTER JOIN " & _
               "    LibraryProducts RIGHT OUTER JOIN " & _
               "    LibraryStockInformation RIGHT OUTER JOIN " & _
               "    TransStockReturnItems RIGHT OUTER JOIN " & _
               "    DavaoUnifiedPayroll.dbo.EmployeeProfiles AS EmployeeProfiles RIGHT OUTER JOIN " & _
               "    DavaoUnifiedPayroll.dbo.LibraryBranch AS LibraryBranch_1 RIGHT OUTER JOIN " & _
               "    DavaoUnifiedPayroll.dbo.LibraryBranch AS LibraryBranch_2 RIGHT OUTER JOIN" & _
               "    TransStockReturns ON LibraryBranch_2.ID = TransStockReturns.DestinationBranchID ON LibraryBranch_1.ID = TransStockReturns.BranchID ON EmployeeProfiles.ID = TransStockReturns.UserID ON " & _
               "    TransStockReturns.ID = TransStockReturnItems.StockReturnID ON LibraryStockInformation.ID = TransStockReturnItems.StockID ON " & _
               "    LibraryProducts.ID = LibraryStockInformation.ProductID ON LibraryBrands.ID = LibraryStockInformation.BrandID " & _
               "WHERE " & _
               "    CONVERT(VARCHAR(36), TransStockReturnItems.StockReturnID) = " & ReadyString(vID)

        Return mProData.Execute(pSql)
        '"FROM " & _
        '"" & _
        '"ON LibraryBranch.ID = TransStockReturns.BranchID " & _
        '"ON EmployeeProfiles.ID = TransStockReturns.UserID " & _
        '"ON TransStockReturns.ID = TransStockReturnItems.StockReturnID " & _
        '"ON LibraryStockInformation.ID = TransStockReturnItems.StockID " & _
        '"ON TransStockReturnItems.ID = TransStockReturnItemReasons.ReturnItemsID " & _
        '"ON LibraryStockReturnReasons.ID = TransStockReturnItemReasons.ReasonID "
        '"ON LibraryProducts.ID = LibraryStockInformation.ProductID" & _
        '"ON LibraryBrands.ID = LibraryStockInformation.BrandID " & _
    End Function

    
End Class
