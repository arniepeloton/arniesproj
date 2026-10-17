Public Class BranchSalesDashboard
    Dim pSql As String = vbNullString
    Dim pDT As DataTable

#Region "Actual Sales and Sales Target"
    Public Function ActualSalesOnDateRange(ByVal DateStart As Date, ByVal DateEnd As Date, ByVal BranchID As String, Optional ByVal MachineNumber As String = vbNullString, Optional ByVal isWithMachineNumber As Boolean = False) As DataTable

        If MachineNumber = vbNullString And isWithMachineNumber = False Then
            pSql = " SELECT vwBranchDashboard_ActualSales.BranchID " & vbCrLf &
                   " , (SUM(vwBranchDashboard_ActualSales.TotalSales) - SUM(vwBranchDashboard_ActualSales.SRVAmount)) AS [TotalSales] " & vbCrLf &
                   " , DavaoUnifiedPayroll.dbo.LibraryBranch.Name " & vbCrLf &
                   " , ISNULL(LibraryBranches.SalesTargetPerDay, 0) AS [SalesTarget] " & vbCrLf &
                   " , vwBranchDashboard_ActualSales.TransactionDate" & vbCrLf &
                   " FROM vwBranchDashboard_ActualSales INNER JOIN " & vbCrLf &
                   " LibraryBranches ON LibraryBranches.BranchID = vwBranchDashboard_ActualSales.BranchID INNER JOIN " & vbCrLf &
                   " DavaoUnifiedPayroll.dbo.LibraryBranch ON DavaoUnifiedPayroll.dbo.LibraryBranch.ID = vwBranchDashboard_ActualSales.BranchID " & vbCrLf &
                   " WHERE vwBranchDashboard_ActualSales.BranchID " & BranchID & vbCrLf &
                   " AND (vwBranchDashboard_ActualSales.TransactionDate BETWEEN " & ReadyString(DateStart) & " AND " & ReadyString(DateEnd) & ")" & vbCrLf &
                   " GROUP BY vwBranchDashboard_ActualSales.BranchID, DavaoUnifiedPayroll.dbo.LibraryBranch.Name, LibraryBranches.SalesTargetPerDay,vwBranchDashboard_ActualSales.TransactionDate"
            With Common.WebRequest
                pDT = .ProData.Execute(pSql)
            End With
        Else
            pSql = " SELECT vwBranchDashboard_ActualSales.BranchID " & vbCrLf &
                   " , (SUM(vwBranchDashboard_ActualSales.TotalSales) - SUM(vwBranchDashboard_ActualSales.SRVAmount)) AS [TotalSales] " & vbCrLf &
                   " , DavaoUnifiedPayroll.dbo.LibraryBranch.Name " & vbCrLf &
                   " , ISNULL(LibraryBranches.SalesTargetPerDay, 0) AS [SalesTarget] " & vbCrLf &
                   " , vwBranchDashboard_ActualSales.TransactionDate" & vbCrLf &
                   " , vwBranchDashboard_ActualSales.MachineNumber" & vbCrLf &
                   " FROM vwBranchDashboard_ActualSales INNER JOIN " & vbCrLf &
                   " LibraryBranches ON LibraryBranches.BranchID = vwBranchDashboard_ActualSales.BranchID INNER JOIN " & vbCrLf &
                   " DavaoUnifiedPayroll.dbo.LibraryBranch ON DavaoUnifiedPayroll.dbo.LibraryBranch.ID = vwBranchDashboard_ActualSales.BranchID " & vbCrLf &
                   " WHERE vwBranchDashboard_ActualSales.BranchID " & BranchID & vbCrLf &
                   " AND (vwBranchDashboard_ActualSales.TransactionDate BETWEEN " & ReadyString(DateStart) & " AND " & ReadyString(DateEnd) & ")"
            If MachineNumber <> vbNullString Then
                pSql &= " AND vwBranchDashboard_ActualSales.MachineNumber " & ReadyString(MachineNumber)
            End If
            pSql &= " GROUP BY vwBranchDashboard_ActualSales.BranchID, DavaoUnifiedPayroll.dbo.LibraryBranch.Name, LibraryBranches.SalesTargetPerDay, vwBranchDashboard_ActualSales.MachineNumber, vwBranchDashboard_ActualSales.TransactionDate"
            With Common.WebRequest
                pDT = .ProData.Execute(pSql)
            End With
        End If
        Return pDT
    End Function

    Public Function GetAllBranches(ByVal BranchList As String) As DataTable

        pSql = " SELECT LibraryBranches.BranchID, DavaoUnifiedPayroll.dbo.LibraryBranch.Name, DavaoUnifiedPayroll.dbo.LibraryBranch.ID AS Num FROM LibraryBranches INNER JOIN " & vbCrLf &
               " DavaoUnifiedPayroll.dbo.LibraryBranch ON DavaoUnifiedPayroll.dbo.LibraryBranch.ID = LibraryBranches.BranchID " & vbCrLf &
               " WHERE LibraryBranches.BranchID " & BranchList

        With Common.WebRequest
            pDT = .ProData.Execute(pSql)
        End With

        Return pDT
    End Function

    Public Function ActualSalesMonthly(ByVal DateStart As Integer, ByVal DateEnd As Integer, ByVal BranchID As String) As DataTable
        pSql = " SELECT vwBranchDashboard_ActualSales.BranchID " & vbCrLf &
                   " , (SUM(vwBranchDashboard_ActualSales.TotalSales) - SUM(vwBranchDashboard_ActualSales.SRVAmount)) AS [TotalSales] " & vbCrLf &
                   " , DavaoUnifiedPayroll.dbo.LibraryBranch.Name " & vbCrLf &
                   " , ISNULL(LibraryBranches.SalesTargetPerDay, 0) AS [SalesTarget] " & vbCrLf &
                   " , DATEPART(month,vwBranchDashboard_ActualSales.TransactionDate) AS [TransactionMonth]" & vbCrLf &
                   " FROM vwBranchDashboard_ActualSales INNER JOIN " & vbCrLf &
                   " LibraryBranches ON LibraryBranches.BranchID = vwBranchDashboard_ActualSales.BranchID INNER JOIN " & vbCrLf &
                   " DavaoUnifiedPayroll.dbo.LibraryBranch ON DavaoUnifiedPayroll.dbo.LibraryBranch.ID = vwBranchDashboard_ActualSales.BranchID " & vbCrLf &
                   " WHERE vwBranchDashboard_ActualSales.BranchID " & BranchID & vbCrLf &
                   " AND (MONTH(vwBranchDashboard_ActualSales.TransactionDate) BETWEEN " & ReadyString(DateStart) & " AND " & ReadyString(DateEnd) & ")" & vbCrLf &
                   " GROUP BY vwBranchDashboard_ActualSales.BranchID, DavaoUnifiedPayroll.dbo.LibraryBranch.Name, LibraryBranches.SalesTargetPerDay,vwBranchDashboard_ActualSales.TransactionDate"
        With Common.WebRequest
            pDT = .ProData.Execute(pSql)
        End With
    End Function
    Public Function GetBranchByName(ByVal BranchName As String) As DataTable
        pSql = " SELECT LibraryBranches.BranchID, DavaoUnifiedPayroll.dbo.LibraryBranch.Name FROM LibraryBranches INNER JOIN " & vbCrLf &
               " DavaoUnifiedPayroll.dbo.LibraryBranch ON DavaoUnifiedPayroll.dbo.LibraryBranch.ID = LibraryBranches.BranchID " & vbCrLf &
               " WHERE DavaoUnifiedPayroll.dbo.LibraryBranch.Name = " & ReadyString(BranchName)

        With Common.WebRequest
            pDT = .ProData.Execute(pSql)
        End With
        Return pDT
    End Function
#End Region
End Class
