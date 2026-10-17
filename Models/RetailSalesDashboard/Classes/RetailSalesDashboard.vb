Public Class RetailSalesDashboard
    Implements InterfaceRetailSalesDashboard
    'Setting of Attributes
    'No Attribute Requires

    'Setting of Methods
    Public Function getPOSDesription(ByVal branchId As String) As DataTable Implements InterfaceRetailSalesDashboard.getPOSDesription
        Dim sqlQuery As String = vbNullString
        sqlQuery = "SELECT * FROM LibraryPOSDescriptions" &
                   "WHERE BranchID = " & branchId
        Return Common.WebRequest.ProData.Execute(sqlQuery)
    End Function
End Class
