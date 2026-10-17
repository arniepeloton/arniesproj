Imports Aspose.Cells

Public Class Revenue
    Public Property MaxReturnRow As Integer = 50

    'Template PIM Query
    'Select * from MobizLibraryPimProduct
    'inner join TransInventoryLedger on MobizLibraryPimProduct.ID = TransInventoryLedger.ProductID

    Function SalesReportByCustomerAndSupplierProducts() As String
        Dim pSql As String
        Dim dtTemp As DataTable
        Dim SubscriberOrganization As DataTable
        Dim PIMJoinTable As String
        Dim PIMInvoiceTable As String
        Dim PIMInvoiceCustomerTable As String
        Dim PIMInvoiceVendorItems As String
        Dim PIMInvoiceVendor As String

        Dim pDtCrossTab As DataTable
        Dim pFinalTable As DataTable

        Dim pFS As IO.FileStream
        Dim pWB As Workbook
        Dim pWS As Worksheet

        Dim Suppliername As String = ""
        Dim ColumnTotals() As Double
        Dim pRowIndex As Integer
        Dim pColIndex As Integer
        Dim pFileName As String
        Dim pFileNameDestination As String
        Dim ReportDateReference As String


        SubscriberOrganization = GetSubscriberOrganizationInfo()
        PIMJoinTable = GetPIMJoinTable()
        PIMInvoiceTable = GetInvoiceJoinTable()
        PIMInvoiceCustomerTable = GetInvoiceCustomerTable()
        PIMInvoiceVendorItems = GetVendorItemsTable()
        PIMInvoiceVendor = GetInvoiceVendorTable()

        ReportDateReference = Common.WebRequest.Data("DateReference")

        pSql = "Select " & vbCrLf &
                "customer.CompanyName CustomerName " & vbCrLf &
                ", MobizLibraryPimProduct.ProductName ProductName " & vbCrLf &
                ", ISNULL(DATENAME(month, " & ReportDateReference & ") + ' (' + DATENAME(YEAR, " & ReportDateReference & ") + ')',DATENAME(month, TransInvoice.InvoiceDate) + ' (' + DATENAME(YEAR, TransInvoice.InvoiceDate) + ')') MonthSales " & vbCrLf &
                ", SUM(TransInvoiceItems.Subtotal) TotalSales " & vbCrLf &
                ", vendor.CompanyName Supplier " & vbCrLf &
                PIMJoinTable & vbCrLf &
                PIMInvoiceTable & vbCrLf &
                PIMInvoiceCustomerTable & vbCrLf &
                PIMInvoiceVendorItems & vbCrLf &
                PIMInvoiceVendor & vbCrLf

        pSql &= " WHERE TransInvoice.InvoiceNumber>0 and CustomerID = " & ReadyString(CStr2(Common.WebRequest.Data("CustomerID"))) & vbCrLf &
                " AND TransInvoice.InvoiceDate between " & ReadyString(CStr2(Common.WebRequest.Data("DateFrom"))) & " AND " & ReadyString(CStr2(Common.WebRequest.Data("DateTo"))) & vbCrLf

        pSql &= " Group By customer.CompanyName  " & vbCrLf &
                ", ISNULL(DATENAME(month, " & ReportDateReference & ") + ' (' + DATENAME(YEAR, " & ReportDateReference & ") + ')',DATENAME(month, TransInvoice.InvoiceDate) + ' (' + DATENAME(YEAR, TransInvoice.InvoiceDate) + ')') " & vbCrLf &
                ",vendor.CompanyName, MobizLibraryPimProduct.ProductName"

        dtTemp = Common.WebRequest.ProData.Execute(pSql)

        pDtCrossTab = CreateCrossTab(dtTemp, {"Supplier", "ProductName"}, {New CrossColumn("{MonthSales}", "TotalSales", "TOTAL (NET OF DISC)")})

        pDtCrossTab.DefaultView.Sort = "Supplier ASC, ProductName ASC"

        pFinalTable = pDtCrossTab.DefaultView.ToTable


        '------------- Create Template
        pFileName = Common.ServerPath & "/temp/RevenueReports/Template/Template.xls"
        pFileNameDestination = Common.ServerPath & "/temp/RevenueReports/" & WebRequest.Instance.OrgID & "/CustomerAndSupplierProducts.xls"

        If Not System.IO.Directory.Exists(Common.ServerPath & "/temp/RevenueReports") Then
            System.IO.Directory.CreateDirectory(Common.ServerPath & "/temp/RevenueReports")
        End If

        If Not System.IO.Directory.Exists(Common.ServerPath & "/temp/RevenueReports/" & WebRequest.Instance.OrgID) Then
            System.IO.Directory.CreateDirectory(Common.ServerPath & "/temp/RevenueReports/" & WebRequest.Instance.OrgID)
        End If

        Dim dtTemp2 As New DataTable

        pFS = New IO.FileStream(pFileName, IO.FileMode.Open)
        pWB = New Workbook
        pWB.Open(pFS)
        pWS = pWB.Worksheets(0)

        pWS.Cells(pRowIndex, pColIndex).PutValue(CStr2(SubscriberOrganization.Rows(0).Item("CompanyName")))
        pRowIndex += 1

        pWS.Cells(pRowIndex, pColIndex).PutValue("CUSTOMER SALES REPORT")
        pRowIndex += 1

        pWS.Cells(pRowIndex, pColIndex).PutValue("PER SUPPLIER/PER SKU")
        pRowIndex += 1

        pWS.Cells(pRowIndex, pColIndex).PutValue(CStr2(Common.WebRequest.Data("DateFrom")) & " - " & CStr2(Common.WebRequest.Data("DateTo")))
        pRowIndex += 1
        pRowIndex += 1

        pWS.Cells(pRowIndex, pColIndex).PutValue(CStr2(Common.WebRequest.Data("CustomerName")))
        pRowIndex += 1

        '-------------
        For i As Integer = 0 To pFinalTable.Rows.Count

            'if Suppliername not equal to pFinalTable.Rows(i).Item("Supplier") then its a new supplier

            If i = pFinalTable.Rows.Count OrElse Not Suppliername = pFinalTable.Rows(i).Item("Supplier") Then

                'if Suppliername.Length = 0 then its the start of pFinalTable loop and should not display totals
                If Suppliername.Length > 0 Then

                    pRowIndex += 1

                    pWS.Cells(pRowIndex, pColIndex).PutValue("Total")

                    For totalscount As Integer = 0 To ColumnTotals.Length - 2
                        pWS.Cells(pRowIndex, totalscount + 1).PutValue(ColumnTotals(totalscount))
                    Next
                End If

                If i = pFinalTable.Rows.Count Then Exit For

                pRowIndex += 1
                pRowIndex += 1
                'create new headers for new supplier

                'new supplier
                Suppliername = pFinalTable.Rows(i).Item("Supplier")

                'new header
                pWS.Cells(pRowIndex, pColIndex).PutValue(Suppliername)

                For colheader As Integer = 2 To pFinalTable.Columns.Count - 1
                    'column 3 sa datatable will be assign to column 1 sa excel. column start sa 0
                    pWS.Cells(pRowIndex, colheader - 1).PutValue(pFinalTable.Columns(colheader).ColumnName)
                Next

                pRowIndex += 1
                pRowIndex += 1
                ReDim ColumnTotals(pFinalTable.Columns.Count - 2)

            End If

            'display products info
            For productinfocount = 1 To pFinalTable.Columns.Count - 1
                'column 1 in datatable will be displayed to column 0 sa excel. column start sa 0
                pWS.Cells(pRowIndex, pColIndex + productinfocount - 1).PutValue(pFinalTable.Rows(i).Item(productinfocount))
            Next

            pRowIndex += 1

            'add totals per column
            For j As Integer = 0 To pFinalTable.Columns.Count - 3
                ColumnTotals(j) += CDec2(pFinalTable.Rows(i).Item(pFinalTable.Columns(j + 2)))
            Next

        Next

        Try
            pWB.Save(pFileNameDestination)
            If pFS IsNot Nothing Then pFS.Dispose()

            Dim pOut As New JsonDictionary

            pOut.Add("Report", "CustomerAndSupplierProducts")
            pOut.Add("Results", pFinalTable)

            Return pOut.Serialize

        Catch ex As Exception
        End Try

    End Function

    Function SalesReportByProductMeasurementQuantity() As String
        Dim pSql As String
        Dim dtTemp As DataTable
        Dim SubscriberOrganization As DataTable
        Dim PIMJoinTable As String
        Dim PIMInvoiceTable As String
        Dim PIMInvoiceCustomerTable As String
        Dim PIMInvoiceVendorItems As String
        Dim PIMInvoiceVendor As String

        Dim pDtCrossTab As DataTable
        Dim pFinalTable As DataTable

        Dim pFS As IO.FileStream
        Dim pWB As Workbook
        Dim pWS As Worksheet

        Dim Suppliername As String = ""
        Dim ColumnTotals() As Double
        Dim pRowIndex As Integer
        Dim pColIndex As Integer
        Dim pFileName As String
        Dim pFileNameDestination As String
        Dim ReportDateReference As String


        SubscriberOrganization = GetSubscriberOrganizationInfo()
        PIMJoinTable = GetPIMJoinTable()
        PIMInvoiceTable = GetInvoiceJoinTable()
        PIMInvoiceCustomerTable = GetInvoiceCustomerTable()
        PIMInvoiceVendorItems = GetVendorItemsTable()
        PIMInvoiceVendor = GetInvoiceVendorTable()

        ReportDateReference = Common.WebRequest.Data("DateReference")
        'reference

        'select 
        'SUM(TransInvoiceItems.Quantity) TotalQuantity 
        ', TransInvoiceItems.UnitMeasure UnitMeasure 
        ' from LibraryVendorPrices
        'inner join TransInvoiceItems 
        'on TransInvoiceItems.ProductID = LibraryVendorPrices.ProductID
        'inner join TransInvoice on TransInvoice.ID = TransInvoiceItems.InvoiceID
        'inner join TransInventoryLedger on TransInvoiceItems.ProductID = TransInventoryLedger.ProductID
        'where vendorID = 'C7E2DD98-EE9E-4717-99BB-76797F9E459C'
        'Group By TransInvoiceItems.UnitMeasure

        pSql = "Select " & vbCrLf &
                " vendor.CompanyName Supplier " & vbCrLf &
                ", MobizLibraryPimProduct.ProductNo ProductNo " & vbCrLf &
                ", MobizLibraryPimProduct.ProductName ProductName " & vbCrLf &
                ", SUM(TransInvoiceItems.Quantity) TotalQuantity " & vbCrLf &
                ", TransInvoiceItems.UnitMeasure UnitMeasure " & vbCrLf &
                PIMJoinTable & vbCrLf &
                PIMInvoiceTable & vbCrLf &
                PIMInvoiceVendorItems & vbCrLf &
                PIMInvoiceVendor & vbCrLf

        pSql &= " WHERE TransInvoice.InvoiceNumber>0 and vendor.ID = " & ReadyString(CStr2(Common.WebRequest.Data("VendorID"))) & vbCrLf &
                " AND TransInvoice.InvoiceDate between " & ReadyString(CStr2(Common.WebRequest.Data("DateFrom"))) & " AND " & ReadyString(CStr2(Common.WebRequest.Data("DateTo"))) & vbCrLf

        pSql &= " Group By vendor.CompanyName  " & vbCrLf &
                ",MobizLibraryPimProduct.ProductNo " & vbCrLf &
                ",MobizLibraryPimProduct.ProductName " & vbCrLf &
                ", TransInvoiceItems.UnitMeasure"

        dtTemp = Common.WebRequest.ProData.Execute(pSql)

        pDtCrossTab = CreateCrossTab(dtTemp, {"Supplier", "ProductNo", "ProductName"}, {New CrossColumn("{UnitMeasure}", "TotalQuantity")})

        pDtCrossTab.DefaultView.Sort = "Supplier ASC, ProductName ASC"

        pFinalTable = pDtCrossTab.DefaultView.ToTable


        '------------- Create Template
        pFileName = Common.ServerPath & "/temp/RevenueReports/Template/Template.xls"
        pFileNameDestination = Common.ServerPath & "/temp/RevenueReports/" & WebRequest.Instance.OrgID & "/SalesReportByProductMeasurementQuantity.xls"

        If Not System.IO.Directory.Exists(Common.ServerPath & "/temp/RevenueReports") Then
            System.IO.Directory.CreateDirectory(Common.ServerPath & "/temp/RevenueReports")
        End If

        If Not System.IO.Directory.Exists(Common.ServerPath & "/temp/RevenueReports/" & WebRequest.Instance.OrgID) Then
            System.IO.Directory.CreateDirectory(Common.ServerPath & "/temp/RevenueReports/" & WebRequest.Instance.OrgID)
        End If

        Dim dtTemp2 As New DataTable

        pFS = New IO.FileStream(pFileName, IO.FileMode.Open)
        pWB = New Workbook
        pWB.Open(pFS)
        pWS = pWB.Worksheets(0)

        pWS.Cells(pRowIndex, pColIndex).PutValue("Sales Report - PER QUANTITY/PER SKU")
        pRowIndex += 1

        pWS.Cells(pRowIndex, pColIndex).PutValue(CStr2(SubscriberOrganization.Rows(0).Item("CompanyName")))
        pRowIndex += 1

        pWS.Cells(pRowIndex, pColIndex).PutValue(CStr2(Common.WebRequest.Data("DateFrom")) & " - " & CStr2(Common.WebRequest.Data("DateTo")))


        '-------------
        For i As Integer = 0 To pFinalTable.Rows.Count

            'if Suppliername not equal to pFinalTable.Rows(i).Item("Supplier") then its a new supplier

            If i = pFinalTable.Rows.Count OrElse Not Suppliername = pFinalTable.Rows(i).Item("Supplier") Then

                'if Suppliername.Length = 0 then its the start of pFinalTable loop and should not display totals
                If Suppliername.Length > 0 Then

                    pRowIndex += 1

                    'pWS.Cells(pRowIndex, pColIndex).PutValue("Total")

                    'For totalscount As Integer = 0 To ColumnTotals.Length - 2
                    '    pWS.Cells(pRowIndex, totalscount + 1).PutValue(ColumnTotals(totalscount))
                    'Next
                End If

                If i = pFinalTable.Rows.Count Then Exit For

                pRowIndex += 1
                pRowIndex += 1
                'create new headers for new supplier

                'new supplier
                Suppliername = pFinalTable.Rows(i).Item("Supplier")

                'new header
                pWS.Cells(pRowIndex, pColIndex).PutValue(Suppliername)

                pRowIndex += 1
                pRowIndex += 1

                For colheader As Integer = 1 To pFinalTable.Columns.Count - 1
                    'column 3 sa datatable will be assign to column 1 sa excel. column start sa 0
                    pWS.Cells(pRowIndex, colheader - 1).PutValue(pFinalTable.Columns(colheader).ColumnName)
                Next

                pRowIndex += 1

                ReDim ColumnTotals(pFinalTable.Columns.Count - 2)

            End If

            'display products info
            For productinfocount = 1 To pFinalTable.Columns.Count - 1
                'column 1 in datatable will be displayed to column 0 sa excel. column start sa 0
                pWS.Cells(pRowIndex, pColIndex + productinfocount - 1).PutValue(pFinalTable.Rows(i).Item(productinfocount))
            Next

            pRowIndex += 1

        Next

        Try
            pWB.Save(pFileNameDestination)
            If pFS IsNot Nothing Then pFS.Dispose()

            Dim pOut As New JsonDictionary

            pOut.Add("Report", "SalesReportByProductMeasurementQuantity")
            pOut.Add("Results", pFinalTable)

            Return pOut.Serialize

        Catch ex As Exception
        End Try
    End Function

    Function SalesReportByProductQuantityPerMonth() As String
        Dim pSql As String
        Dim dtTemp As DataTable
        Dim SubscriberOrganization As DataTable
        Dim PIMJoinTable As String
        Dim PIMInvoiceTable As String
        Dim PIMInvoiceCustomerTable As String
        Dim PIMInvoiceVendorItems As String
        Dim PIMInvoiceVendor As String

        Dim pDtCrossTab As DataTable
        Dim pFinalTable As DataTable

        Dim pFS As IO.FileStream
        Dim pWB As Workbook
        Dim pWS As Worksheet

        Dim Suppliername As String = ""
        Dim ColumnTotals() As Double
        Dim pRowIndex As Integer
        Dim pColIndex As Integer
        Dim pFileName As String
        Dim pFileNameDestination As String
        Dim ReportDateReference As String

        SubscriberOrganization = GetSubscriberOrganizationInfo()
        PIMJoinTable = GetPIMJoinTable()
        PIMInvoiceTable = GetInvoiceJoinTable()
        PIMInvoiceCustomerTable = GetInvoiceCustomerTable()
        PIMInvoiceVendorItems = GetVendorItemsTable()
        PIMInvoiceVendor = GetInvoiceVendorTable()

        ReportDateReference = Common.WebRequest.Data("DateReference")

        pSql = "Select " & vbCrLf &
                " MobizLibraryPimProduct.ProductNo ProductNo " & vbCrLf &
                ", MobizLibraryPimProduct.ProductName ProductName " & vbCrLf &
                ", ISNULL(DATENAME(month, " & ReportDateReference & ") + ' (' + DATENAME(YEAR, " & ReportDateReference & ") + ')',DATENAME(month, TransInvoice.InvoiceDate) + ' (' + DATENAME(YEAR, TransInvoice.InvoiceDate) + ')') MonthSales " & vbCrLf &
                ", SUM(TransInventoryLedger.Qty * -1) TotalQuantityPerMonth " & vbCrLf &
                PIMJoinTable & vbCrLf &
                PIMInvoiceTable & vbCrLf

        pSql &= " WHERE TransInvoice.InvoiceNumber>0 " & vbCrLf &
                " AND TransInvoice.InvoiceDate between " & ReadyString(CStr2(Common.WebRequest.Data("DateFrom"))) & " AND " & ReadyString(CStr2(Common.WebRequest.Data("DateTo"))) & vbCrLf

        pSql &= " Group By MobizLibraryPimProduct.ProductNo " & vbCrLf &
                ", ISNULL(DATENAME(month, " & ReportDateReference & ") + ' (' + DATENAME(YEAR, " & ReportDateReference & ") + ')',DATENAME(month, TransInvoice.InvoiceDate) + ' (' + DATENAME(YEAR, TransInvoice.InvoiceDate) + ')') " & vbCrLf &
                ", MobizLibraryPimProduct.ProductName " & vbCrLf &
                ", YEAR(" & ReportDateReference & "), MONTH(" & ReportDateReference & ") "

        pSql &= " ORDER BY YEAR(" & ReportDateReference & "), MONTH(" & ReportDateReference & ") " & vbCrLf

        dtTemp = Common.WebRequest.ProData.Execute(pSql)

        pDtCrossTab = CreateCrossTab(dtTemp, {"ProductNo", "ProductName"}, {New CrossColumn("{MonthSales}", "TotalQuantityPerMonth", "Total Quantity")})

        pDtCrossTab.DefaultView.Sort = "ProductName ASC"

        pFinalTable = pDtCrossTab.DefaultView.ToTable


        '------------- Create Template
        pFileName = Common.ServerPath & "/temp/RevenueReports/Template/Template.xls"
        pFileNameDestination = Common.ServerPath & "/temp/RevenueReports/" & WebRequest.Instance.OrgID & "/SalesReportByProductQuantityPerMonth.xls"

        If Not System.IO.Directory.Exists(Common.ServerPath & "/temp/RevenueReports") Then
            System.IO.Directory.CreateDirectory(Common.ServerPath & "/temp/RevenueReports")
        End If

        If Not System.IO.Directory.Exists(Common.ServerPath & "/temp/RevenueReports/" & WebRequest.Instance.OrgID) Then
            System.IO.Directory.CreateDirectory(Common.ServerPath & "/temp/RevenueReports/" & WebRequest.Instance.OrgID)
        End If

        Dim dtTemp2 As New DataTable

        pFS = New IO.FileStream(pFileName, IO.FileMode.Open)
        pWB = New Workbook
        pWB.Open(pFS)
        pWS = pWB.Worksheets(0)

        pWS.Cells(pRowIndex, pColIndex).PutValue(CStr2(SubscriberOrganization.Rows(0).Item("CompanyName")))
        pRowIndex += 1

        pWS.Cells(pRowIndex, pColIndex).PutValue("Sales Report - By Products")
        pRowIndex += 1

        pWS.Cells(pRowIndex, pColIndex).PutValue("PER MONTH/PER QTY")
        pRowIndex += 1

        pWS.Cells(pRowIndex, pColIndex).PutValue(CStr2(Common.WebRequest.Data("DateFrom")) & " - " & CStr2(Common.WebRequest.Data("DateTo")))
        pRowIndex += 1
        pRowIndex += 1

        pWS.Cells(pRowIndex, pColIndex).PutValue(CStr2(SubscriberOrganization.Rows(0).Item("CompanyName")))
        pRowIndex += 1
        pRowIndex += 1
        '-------------

        For colheader As Integer = 0 To pFinalTable.Columns.Count - 1
            'column 3 sa datatable will be assign to column 1 sa excel. column start sa 0
            pWS.Cells(pRowIndex, colheader).PutValue(pFinalTable.Columns(colheader).ColumnName)
        Next

        pRowIndex += 1
        pRowIndex += 1

        ReDim ColumnTotals(pFinalTable.Columns.Count - 2)

        For i As Integer = 0 To pFinalTable.Rows.Count - 1

            'display products info
            For productinfocount = 0 To pFinalTable.Columns.Count - 1
                'column 1 in datatable will be displayed to column 0 sa excel. column start sa 0
                pWS.Cells(pRowIndex, pColIndex + productinfocount).PutValue(pFinalTable.Rows(i).Item(productinfocount))
            Next

            pRowIndex += 1

            'add totals per column
            For j As Integer = 0 To pFinalTable.Columns.Count - 3
                ColumnTotals(j) += CDec2(pFinalTable.Rows(i).Item(pFinalTable.Columns(j + 2)))
            Next

        Next

        pRowIndex += 1

        pWS.Cells(pRowIndex, pColIndex).PutValue("Total")

        For totalscount As Integer = 0 To ColumnTotals.Length - 2
            pWS.Cells(pRowIndex, totalscount + 2).PutValue(ColumnTotals(totalscount))
        Next

        Try
            pWB.Save(pFileNameDestination)
            If pFS IsNot Nothing Then pFS.Dispose()

            Dim pOut As New JsonDictionary

            pOut.Add("Report", "SalesReportByProductQuantityPerMonth")
            pOut.Add("Results", pFinalTable)

            Return pOut.Serialize

        Catch ex As Exception
        End Try

    End Function

    Function SalesReportByProductBaseQuantity() As String
        Dim pSql As String
        Dim dtTemp As DataTable
        Dim SubscriberOrganization As DataTable
        Dim PIMJoinTable As String
        Dim PIMInvoiceTable As String
        Dim PIMInvoiceCustomerTable As String
        Dim PIMInvoiceVendorItems As String
        Dim PIMInvoiceVendor As String

        Dim pDtCrossTab As DataTable
        Dim pFinalTable As DataTable

        Dim pFS As IO.FileStream
        Dim pWB As Workbook
        Dim pWS As Worksheet

        Dim Suppliername As String = ""
        Dim ColumnTotals() As Double
        Dim pRowIndex As Integer
        Dim pColIndex As Integer
        Dim pFileName As String
        Dim pFileNameDestination As String
        Dim ReportDateReference As String


        SubscriberOrganization = GetSubscriberOrganizationInfo()
        PIMJoinTable = GetPIMJoinTable()
        PIMInvoiceTable = GetInvoiceJoinTable()
        PIMInvoiceCustomerTable = GetInvoiceCustomerTable()
        PIMInvoiceVendorItems = GetVendorItemsTable()
        PIMInvoiceVendor = GetInvoiceVendorTable()

        ReportDateReference = Common.WebRequest.Data("DateReference")

        pSql = "Select " & vbCrLf &
                " vendor.CompanyName Supplier " & vbCrLf &
                ", MobizLibraryPimProduct.ProductNo ProductNo " & vbCrLf &
                ", MobizLibraryPimProduct.ProductName ProductName " & vbCrLf &
                ", SUM(TransInventoryLedger.Qty * -1) TotalQuantity " & vbCrLf &
                ", SUM(TransInvoiceItems.Subtotal) TotalAmount " & vbCrLf &
                PIMJoinTable & vbCrLf &
                PIMInvoiceTable & vbCrLf &
                PIMInvoiceVendorItems & vbCrLf &
                PIMInvoiceVendor & vbCrLf

        pSql &= " WHERE TransInvoice.InvoiceNumber>0 and vendor.ID = " & ReadyString(CStr2(Common.WebRequest.Data("VendorID"))) & vbCrLf &
                " AND TransInvoice.InvoiceDate between " & ReadyString(CStr2(Common.WebRequest.Data("DateFrom"))) & " AND " & ReadyString(CStr2(Common.WebRequest.Data("DateTo"))) & vbCrLf

        pSql &= " Group By vendor.CompanyName  " & vbCrLf &
                ",MobizLibraryPimProduct.ProductNo " & vbCrLf &
                ",MobizLibraryPimProduct.ProductName " & vbCrLf

        dtTemp = Common.WebRequest.ProData.Execute(pSql)

        pFinalTable = dtTemp

        '------------- Create Template
        pFileName = Common.ServerPath & "/temp/RevenueReports/Template/Template.xls"
        pFileNameDestination = Common.ServerPath & "/temp/RevenueReports/" & WebRequest.Instance.OrgID & "/SalesReportByProductBaseQuantity.xls"

        If Not System.IO.Directory.Exists(Common.ServerPath & "/temp/RevenueReports") Then
            System.IO.Directory.CreateDirectory(Common.ServerPath & "/temp/RevenueReports")
        End If

        If Not System.IO.Directory.Exists(Common.ServerPath & "/temp/RevenueReports/" & WebRequest.Instance.OrgID) Then
            System.IO.Directory.CreateDirectory(Common.ServerPath & "/temp/RevenueReports/" & WebRequest.Instance.OrgID)
        End If

        Dim dtTemp2 As New DataTable

        pFS = New IO.FileStream(pFileName, IO.FileMode.Open)
        pWB = New Workbook
        pWB.Open(pFS)
        pWS = pWB.Worksheets(0)

        pWS.Cells(pRowIndex, pColIndex).PutValue("Sales Report - PER QUANTITY/PER SKU")
        pRowIndex += 1

        pWS.Cells(pRowIndex, pColIndex).PutValue(CStr2(SubscriberOrganization.Rows(0).Item("CompanyName")))
        pRowIndex += 1

        pWS.Cells(pRowIndex, pColIndex).PutValue(CStr2(Common.WebRequest.Data("DateFrom")) & " - " & CStr2(Common.WebRequest.Data("DateTo")))


        '-------------
        For i As Integer = 0 To pFinalTable.Rows.Count

            'if Suppliername not equal to pFinalTable.Rows(i).Item("Supplier") then its a new supplier

            If i = pFinalTable.Rows.Count OrElse Not Suppliername = pFinalTable.Rows(i).Item("Supplier") Then

                'if Suppliername.Length = 0 then its the start of pFinalTable loop and should not display totals
                If Suppliername.Length > 0 Then

                    pRowIndex += 1

                    'pWS.Cells(pRowIndex, pColIndex).PutValue("Total")

                    'For totalscount As Integer = 0 To ColumnTotals.Length - 2
                    '    pWS.Cells(pRowIndex, totalscount + 1).PutValue(ColumnTotals(totalscount))
                    'Next
                End If

                If i = pFinalTable.Rows.Count Then Exit For

                pRowIndex += 1
                pRowIndex += 1
                'create new headers for new supplier

                'new supplier
                Suppliername = pFinalTable.Rows(i).Item("Supplier")

                'new header
                pWS.Cells(pRowIndex, pColIndex).PutValue(Suppliername)

                pRowIndex += 1
                pRowIndex += 1

                For colheader As Integer = 1 To pFinalTable.Columns.Count - 1
                    'column 3 sa datatable will be assign to column 1 sa excel. column start sa 0
                    pWS.Cells(pRowIndex, colheader - 1).PutValue(pFinalTable.Columns(colheader).ColumnName)
                Next

                pRowIndex += 1

                ReDim ColumnTotals(pFinalTable.Columns.Count - 2)

            End If

            'display products info
            For productinfocount = 1 To pFinalTable.Columns.Count - 1
                'column 1 in datatable will be displayed to column 0 sa excel. column start sa 0
                pWS.Cells(pRowIndex, pColIndex + productinfocount - 1).PutValue(pFinalTable.Rows(i).Item(productinfocount))
            Next

            pRowIndex += 1

        Next

        Try
            pWB.Save(pFileNameDestination)
            If pFS IsNot Nothing Then pFS.Dispose()

            Dim pOut As New JsonDictionary

            pOut.Add("Report", "SalesReportByProductBaseQuantity")
            pOut.Add("Results", pFinalTable)

            Return pOut.Serialize

        Catch ex As Exception
        End Try
    End Function

    Function InventoryMovementByFOWST() As String
        Dim pSql As String
        Dim dtProducts As DataTable
        Dim pFinalTable As New DataTable

        Dim pFS As IO.FileStream
        Dim pWB As Workbook
        Dim pWS As Worksheet

        Dim pRowIndex As Integer
        Dim pColIndex As Integer
        Dim pFileName As String
        Dim pFileNameDestination As String
        Dim SubscriberOrganization As DataTable
        Dim ColumnTotals() As Double


        SubscriberOrganization = GetSubscriberOrganizationInfo()

        pSql = "Select dbo.MobizLibraryPIMProduct.ID " & vbCrLf &
                ",dbo.MobizLibraryPIMProduct.ProductNo " & vbCrLf &
                ",dbo.MobizLibraryPIMProduct.ProductName " & vbCrLf &
                ",TransInventoryLedger.Qty Quantity " & vbCrLf &
                ",TransInventoryLedger.Balance " & vbCrLf &
                ",TransInventoryLedger.TransactionType AS Source " & vbCrLf &
                "FROM TransInventoryLedger " & vbCrLf &
                "JOIN MobizLibraryPIMProduct ON TransInventoryLedger.ProductID = MobizLibraryPIMProduct.ID " & vbCrLf &
                "JOIN MobizLibraryOrganizationFowst ON MobizLibraryOrganizationFowst.ID = TransInventoryLedger.FowstID " & vbCrLf &
                "JOIN MobizLibraryPeople on MobizLibraryPeople.ID = TransInventoryLedger.UserID " & vbCrLf &
                "LEFT JOIN LibraryFowstBin ON LibraryFowstBin.ID = TransInventoryLedger.BinID " & vbCrLf &
                "Where MobizLibraryOrganizationFowst.ID = " & ReadyString(WebRequest.Instance.Data("FOWSTID")) & vbCrLf &
                "AND convert(date, TransInventoryLedger.TransactionDate) between  " & vbCrLf &
                ReadyString(CDate(WebRequest.Instance.Data("DateFrom"))) & " AND " & ReadyString(CDate(WebRequest.Instance.Data("DateTo"))) & vbCrLf &
                "ORDER BY dbo.MobizLibraryPIMProduct.ProductName, TransInventoryLedger.TransactionDate, TransInventoryLedger.LastUpdated"

        dtProducts = Common.WebRequest.ProData.Execute(pSql)

        pFinalTable.Columns.Add("Product")
        pFinalTable.Columns.Add("BeginningQuantity")
        pFinalTable.Columns.Add("TotalStockIn")
        pFinalTable.Columns.Add("TotalStockOut")
        pFinalTable.Columns.Add("RemainingQuantity")

        ReDim ColumnTotals(pFinalTable.Columns.Count - 1)

        If dtProducts.Rows.Count > 0 Then
            Dim dtProductsID As DataTable
            Dim dtFiltered As DataTable

            Dim query = (From row In dtProducts.AsEnumerable() _
                            Select row.Field(Of String)("ProductNo")).Distinct()

            For Each pID In query
                Dim dr As DataRow

                dtProducts.DefaultView.RowFilter = "ProductNo = " & ReadyString(pID)

                dtFiltered = dtProducts.DefaultView.ToTable


                dr = pFinalTable.NewRow

                'get First Balance as beginning balance
                dr(0) = dtFiltered.Rows(0).Item("ProductName")

                'get the balanace before row 0 balance
                dr(1) = CInt2(dtFiltered.Rows(0).Item("Balance")) - CInt2(dtFiltered.Rows(0).Item("Quantity"))

                'get Last Balance as ending balance
                dr(4) = CInt2(dtFiltered.Rows(dtFiltered.Rows.Count - 1).Item("Balance"))

                Dim ProductIn As Integer = 0
                Dim ProductOut As Integer = 0

                For i As Integer = 0 To dtFiltered.Rows.Count - 1
                    If dtFiltered.Rows(i).Item("Source") = "Invoice" OrElse dtFiltered.Rows(i).Item("Source") = "TransferOut" Then
                        ProductOut += CInt2(dtFiltered.Rows(i).Item("Quantity")) * -1
                    ElseIf dtFiltered.Rows(i).Item("Source") = "Receiving" OrElse dtFiltered.Rows(i).Item("Source") = "Sales Returns" Then
                        ProductIn += CInt2(dtFiltered.Rows(i).Item("Quantity"))
                    End If
                    'compute balance based on transactiontype
                Next

                dr(2) = ProductIn
                dr(3) = ProductOut

                pFinalTable.Rows.Add(dr)
            Next

            '------------- Create Template
            pFileName = Common.ServerPath & "/temp/RevenueReports/Template/Template.xls"
            pFileNameDestination = Common.ServerPath & "/temp/RevenueReports/" & WebRequest.Instance.OrgID & "/InventoryMovementByFOWST.xls"

            If Not System.IO.Directory.Exists(Common.ServerPath & "/temp/RevenueReports") Then
                System.IO.Directory.CreateDirectory(Common.ServerPath & "/temp/RevenueReports")
            End If

            If Not System.IO.Directory.Exists(Common.ServerPath & "/temp/RevenueReports/" & WebRequest.Instance.OrgID) Then
                System.IO.Directory.CreateDirectory(Common.ServerPath & "/temp/RevenueReports/" & WebRequest.Instance.OrgID)
            End If

            Dim dtTemp2 As New DataTable

            pFS = New IO.FileStream(pFileName, IO.FileMode.Open)
            pWB = New Workbook
            pWB.Open(pFS)
            pWS = pWB.Worksheets(0)

            pWS.Cells(pRowIndex, pColIndex).PutValue("Sales Report - PER QUANTITY/PER SKU")
            pRowIndex += 1

            pWS.Cells(pRowIndex, pColIndex).PutValue(CStr2(SubscriberOrganization.Rows(0).Item("CompanyName")))
            pRowIndex += 1

            pWS.Cells(pRowIndex, pColIndex).PutValue(CStr2(Common.WebRequest.Data("DateFrom")) & " - " & CStr2(Common.WebRequest.Data("DateTo")))

            '-------------
            pRowIndex += 1
            pRowIndex += 1

            'if Suppliername not equal to pFinalTable.Rows(i).Item("Supplier") then its a new supplier

            For colheader As Integer = 0 To pFinalTable.Columns.Count - 1
                'column 3 sa datatable will be assign to column 1 sa excel. column start sa 0
                pWS.Cells(pRowIndex, colheader).PutValue(pFinalTable.Columns(colheader).ColumnName)
            Next

            pRowIndex += 1
            pRowIndex += 1

            For i As Integer = 0 To pFinalTable.Rows.Count - 1

                'display products info
                For productinfocount = 0 To pFinalTable.Columns.Count - 1
                    'column 1 in datatable will be displayed to column 0 sa excel. column start sa 0
                    pWS.Cells(pRowIndex, pColIndex + productinfocount).PutValue(pFinalTable.Rows(i).Item(productinfocount))
                Next

                pRowIndex += 1

            Next

            Try
                pWB.Save(pFileNameDestination)
                If pFS IsNot Nothing Then pFS.Dispose()

                Dim pOut As New JsonDictionary

                pOut.Add("Report", "InventoryMovementByFOWST")
                pOut.Add("Results", pFinalTable)

                Return pOut.Serialize

            Catch ex As Exception
            End Try
        End If
    End Function


    Function GetPIMJoinTable() As String
        Return " FROM MobizLibraryPimProduct  inner join TransInventoryLedger on MobizLibraryPimProduct.ID = TransInventoryLedger.ProductID"
    End Function

    Function GetInvoiceJoinTable() As String
        Return " inner join TransInvoice on TransInvoice.ID = TransInventoryLedger.TransactionID " & vbCrLf &
                " inner join TransInvoiceItems on TransInvoiceItems.InvoiceID = TransInvoice.ID "
    End Function

    Function GetInvoiceCustomerTable() As String
        Return " inner join MobizLibraryOrganization customer on customer.ID = TransInvoice.CustomerID "
    End Function

    Function GetVendorItemsTable() As String
        Return " inner join LibraryVendorPrices on LibraryVendorPrices.ProductID = TransInvoiceItems.ProductID " 
    End Function

    Function GetInvoiceVendorTable() As String
        Return " inner join MobizLibraryOrganization vendor on vendor.ID = LibraryVendorPrices.VendorID "
    End Function

    Function GetSubscriberOrganizationInfo() As DataTable
        Dim pSql As String

        pSql = "Select * from MobizLibraryOrganization where ID = " & ReadyString(WebRequest.Instance.OrgID)

        Return Common.WebRequest.ProData.Execute(pSql)
    End Function


End Class

