Public Class TransPlanogram
    Dim mProData As ProData
    Dim mParent As Business

    Sub New(ByRef rProData As ProData, ByVal vParent As Business)

        mProData = rProData
        mParent = vParent

    End Sub

    Public Function GetParams(ByVal vID As String) As DataTable

        Dim pDT As DataTable
        Dim pSql As String

        pSql = "SELECT " & _
               "    TransPlanogram.ID, " & _
               "    TransPlanogram.NoOfAisles, " & _
               "    TransPlanogram.TotalOfAisles, " & _
               "    TransPlanogram.Sections, " & _
               "    TransPlanogram.Shelves, " & _
               "    TransPlanogram.EndCaps, " & _
               "    TransPlanogram.SafetyStocks, " & _
               "    TransPlanogram.Minimums, " & _
               "    TransPlanogram.Maximums, " & _
               "    TransPlanogram.NiceCaps " & _
               "FROM " & _
               "    TransPlanogram LEFT JOIN DavaoUnifiedPayroll.dbo.LibraryBranch AS  LibraryBranch" & _
               "    ON TransPlanogram.BranchID = LibraryBranch.ID " & _
               "WHERE " & _
               "    TransPlanogram.BranchID = " & ReadyString(vID)

        pDT = mProData.Execute(pSql)

        Return pDT

    End Function
End Class
