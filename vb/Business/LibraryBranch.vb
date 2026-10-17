Public Class LibraryBranch


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
               "    LibraryBranch.ID, " &
               "    LibraryBranch.Name, " &
               "    LibraryBranch.Address " & _
               "FROM " & _
               "    DavaoUnifiedPayroll.dbo.LibraryBranch AS LibraryBranch " &
               "WHERE " & _
               "    CONVERT(varchar(36),LibraryBranch.ID) = " & ReadyString(vID)
        pDT = mProData.Execute(pSql)

        Return pDT

    End Function
End Class
